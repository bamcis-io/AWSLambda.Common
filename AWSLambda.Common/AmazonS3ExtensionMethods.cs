using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace BAMCIS.AWSLambda.Common
{
    /// <summary>
    /// Extension methods for use with the AWS S3 SDK.
    /// </summary>
    public static class AmazonS3ExtensionMethods
    {
        #region Private Fields

        private const long FIVE_MEBIBYTE = 5242880; // 5 MiB
        private const long FIVE_GIBIBYTE = 5368709120; // 5 GiB
        private const long MINIMUM_MULTIPART_SIZE = FIVE_MEBIBYTE; // 5 MiB
        private const long MINIMUM_MULTIPART_PART_SIZE = FIVE_MEBIBYTE;
        private const long MAXIMUM_MULTIPART_PART_SIZE = FIVE_GIBIBYTE;
        private const int MAXIMUM_PARTS = 10000;

        #endregion

        /// <summary>
        /// Copies the properties from the provided source object into a new object of the specified destination
        /// type. The properties that are copied are matched by property name, so if both objects have a "Name" property for example,
        /// then the value in the source is set on the destination "Name" property. Properties that cannot be set or 
        /// assigned to in the destination or cannot be read from the source are omitted.
        /// </summary>
        /// <typeparam name="TDestination">The destination type to construct and assign to.</typeparam>
        /// <param name="source">The source object whose properties will be copied</param>
        /// <returns>A new object with the copied property values from matching property names in the source.</returns>
        public static TDestination CopyProperties<TDestination>(this object source) where TDestination : class, new()
        {
            PropertyInfo[] Properties = source.GetType().GetProperties();

            TDestination Destination = new TDestination();
            Type DestinationType = Destination.GetType();

            foreach (PropertyInfo Info in Properties)
            {
                try
                {
                    // If the property can't be read, just move on to the
                    // next item in the foreach loop
                    if (!Info.CanRead)
                    {
                        continue;
                    }

                    PropertyInfo DestinationProperty = DestinationType.GetProperty(Info.Name);

                    // If the destination is null (property doesn't exist on the object), 
                    // can't be written, or isn't assignable from the source, move on to the next
                    // property in the foreach loop
                    if (DestinationProperty == null ||
                        !DestinationProperty.CanWrite ||
                        (DestinationProperty.GetSetMethod() != null && (DestinationProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0) ||
                        !DestinationProperty.PropertyType.IsAssignableFrom(Info.PropertyType))
                    {
                        continue;
                    }

                    DestinationProperty.SetValue(Destination, Info.GetValue(source, null));
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return Destination;
        }

        /// <summary>
        /// Converts a CopyObjectRequest object to the specified type by copying over
        /// property values from the source to a new destination object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public static T ConvertTo<T>(this CopyObjectRequest request) where T : class, new()
        {
            return request.CopyProperties<T>();
        }

        /// <summary>
        /// Provides the actual implementation to move or copy an S3 object
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <param name="partSize"></param>
        /// <param name="deleteSource"></param>
        /// <param name="useMultipart"></param>
        /// <returns></returns>
        private static async Task<CopyObjectResponse> CopyOrMoveObjectAsync(this IAmazonS3 client, CopyObjectRequest request, long partSize, bool deleteSource, Func<long, long, bool> useMultipart)
        {
            ParameterTests.NonNull(request, "request");
            ParameterTests.OutOfRange(partSize >= MINIMUM_MULTIPART_PART_SIZE, "partSize", $"The part size must be at least {MINIMUM_MULTIPART_PART_SIZE} bytes.");
            ParameterTests.OutOfRange(partSize <= MAXIMUM_MULTIPART_PART_SIZE, "partSize", $"The part size cannot exceed {MAXIMUM_MULTIPART_PART_SIZE} bytes.");

            // Get the size of the object.
            GetObjectMetadataRequest MetadataRequest = new GetObjectMetadataRequest
            {
                BucketName = request.SourceBucket,
                Key = request.SourceKey
            };

            GetObjectMetadataResponse MetadataResponse = await client.GetObjectMetadataAsync(MetadataRequest);
            long ObjectSize = MetadataResponse.ContentLength; // Length in bytes.

            CopyObjectResponse Response = null;

            if (useMultipart.Invoke(ObjectSize, partSize))
            {
                // If it takes more than a 5 GiB part to make 10000 or less parts, than this operation
                // isn't supported for an object this size
                if (ObjectSize / partSize > MAXIMUM_PARTS)
                {
                    throw new NotSupportedException($"The object size, {ObjectSize}, cannot be broken into fewer than {MAXIMUM_PARTS} parts using a part size of {partSize} bytes.");
                }

                List<Task<CopyPartResponse>> CopyResponses = new List<Task<CopyPartResponse>>();

                InitiateMultipartUploadRequest InitiateRequest = request.ConvertTo<InitiateMultipartUploadRequest>();
                InitiateRequest.BucketName = request.DestinationBucket;
                InitiateRequest.Key = request.DestinationKey;

                InitiateMultipartUploadResponse InitiateResponse = await client.InitiateMultipartUploadAsync(InitiateRequest);

                try
                {   
                    long BytePosition = 0;
                    int Counter = 1;

                    // Launch all of the copy parts
                    while (BytePosition < ObjectSize)
                    {
                        CopyPartRequest CopyRequest = request.ConvertTo<CopyPartRequest>();
                        CopyRequest.UploadId = InitiateResponse.UploadId;
                        CopyRequest.FirstByte = BytePosition;
                        // If we're on the last part, the last byte is the object size minus 1, otherwise the last byte is the part size minus one
                        // added to the current byte position
                        CopyRequest.LastByte = ((BytePosition + partSize - 1) >= ObjectSize) ? ObjectSize - 1 : BytePosition + partSize - 1;
                        CopyRequest.PartNumber = Counter++;

                        CopyResponses.Add(client.CopyPartAsync(CopyRequest));

                        BytePosition += partSize;
                    }

                    IEnumerable<CopyPartResponse> Responses = (await Task.WhenAll(CopyResponses)).OrderBy(x => x.PartNumber);

                    // Set up to complete the copy.
                    CompleteMultipartUploadRequest CompleteRequest = new CompleteMultipartUploadRequest
                    {
                        BucketName = request.DestinationBucket,
                        Key = request.DestinationKey,
                        UploadId = InitiateResponse.UploadId
                    };

                    CompleteRequest.AddPartETags(Responses);

                    // Complete the copy.
                    CompleteMultipartUploadResponse CompleteUploadResponse = await client.CompleteMultipartUploadAsync(CompleteRequest);

                    Response = CompleteUploadResponse.CopyProperties<CopyObjectResponse>();
                    Response.SourceVersionId = MetadataResponse.VersionId;
                }
                catch (AmazonS3Exception e)
                {
                    AbortMultipartUploadRequest AbortRequest = new AbortMultipartUploadRequest()
                    {
                        BucketName = request.DestinationBucket,
                        Key = request.DestinationKey,
                        UploadId = InitiateResponse.UploadId
                    };

                    await client.AbortMultipartUploadAsync(AbortRequest);

                    throw e;
                }
            }
            else
            {
                Response = await client.CopyObjectAsync(request);
            }

            if (Response.HttpStatusCode != HttpStatusCode.OK)
            {
                throw new AmazonS3Exception($"Could not copy object from s3://{request.SourceBucket}/{request.SourceKey} to s3://{request.DestinationBucket}/{request.DestinationKey}. Received response : {(int)Response.HttpStatusCode}");
            }
            else
            {
                if (deleteSource)
                {
                    DeleteObjectRequest DeleteRequest = new DeleteObjectRequest()
                    {
                        BucketName = request.SourceBucket,
                        Key = request.SourceKey
                    };

                    DeleteObjectResponse DeleteResponse = await client.DeleteObjectAsync(DeleteRequest);

                    if (DeleteResponse.HttpStatusCode != HttpStatusCode.NoContent)
                    {
                        throw new AmazonS3Exception($"Could not delete s3://{request.SourceBucket}/{request.SourceKey}. Received response : {(int)DeleteResponse.HttpStatusCode}");
                    }
                }

                return Response;
            }
        }

        /// <summary>
        /// Copies or moves an S3 object to another location. This method prefers using a multipart copy as long as the specified part size is less
        /// than the source object's size. If the file exists at the destination, if will be overwritten with no warning.
        /// </summary>
        /// <param name="client">The Amazon S3 Client</param>
        /// <param name="request">The Copy Object Request</param>
        /// <param name="partSize">The size of the part to use for a multipart copy.</param>
        /// <param name="deleteSource">If set to true, the source object will be deleted if it was successfully copied. The default is false.</param>
        /// <returns>The copy object response</returns>
        public static async Task<CopyObjectResponse> CopyOrMoveObjectMultipartAsync(this IAmazonS3 client, CopyObjectRequest request, long partSize, bool deleteSource = false)
        {
            // Use multi part as long as the part size is less than the object size
            // and the object is at least 5 MiB, which is the minimum object size for multipart
            return await CopyOrMoveObjectAsync(client, request, partSize, deleteSource, (objectSize, chunkSize) => { return objectSize >= MINIMUM_MULTIPART_SIZE && chunkSize < objectSize; });
        }

        /// <summary>
        /// Copies or moves an S3 object to another location. This method prefers using a multipart copy as long as the default part size, 5 MiB, is less
        /// than the source object's size. If the file exists at the destination, if will be overwritten with no warning.
        /// </summary>
        /// <param name="client">The Amazon S3 Client</param>
        /// <param name="request">The Copy Object Request</param>
        /// <param name="deleteSource">If set to true, the source object will be deleted if it was successfully copied. The default is false.</param>
        /// <returns>The copy object response</returns>
        public static async Task<CopyObjectResponse> CopyOrMoveObjectMultipartAsync(this IAmazonS3 client, CopyObjectRequest request, bool deleteSource = false)
        {
            // Use multi part as long as the part size is less than the object size
            // and the object is at least 5 MiB, which is the minimum object size for multipart
            return await CopyOrMoveObjectAsync(client, request, FIVE_MEBIBYTE, deleteSource, (objectSize, chunkSize) => { return objectSize >= MINIMUM_MULTIPART_SIZE && chunkSize < objectSize; });
        }

        /// <summary>
        /// Copies or moves an S3 object to another location. If the object is over 5 GiB, the method automatically handles
        /// using a multipart copy using the part size specified. If the object is under 5 GiB, a single copy operation is performed.
        /// If the file exists at the destination, if will be overwritten with no warning.
        /// </summary>
        /// <param name="client">The Amazon S3 Client</param>
        /// <param name="request">The Copy Object Request</param>
        /// <param name="partSize">The size of the part to use for a multipart copy</param>
        /// <param name="deleteSource">If set to true, the source object will be deleted if it was successfully copied. The default is false.</param>
        /// <param name="preferMultipart">If set to true, the method will use a multipart copy as long as the part size is less than the object size for any object, even
        /// those under 5 GiB.</param>
        /// <returns>The copy object response</returns>
        public static async Task<CopyObjectResponse> CopyOrMoveObjectAsync(this IAmazonS3 client, CopyObjectRequest request, long partSize, bool deleteSource = false, bool preferMultipart = false)
        {
            if (preferMultipart)
            {
                // Use multi part as long as the part size is less than the object size
                return await CopyOrMoveObjectMultipartAsync(client, request, partSize, deleteSource);
            }
            else
            {
                // Only use multi part if required due to object size
                return await CopyOrMoveObjectAsync(client, request, partSize, deleteSource, (objectSize, chunkSize) => { return objectSize > FIVE_GIBIBYTE; });
            }
        }

        /// <summary>
        /// Copies or moves an S3 object to another location. If the object is over 5 GiB, the method automatically handles
        /// using a multipart copy using the default part size of 5 MiB. If the file exists at the destination, if will be 
        /// overwritten with no warning.
        /// </summary>
        /// <param name="client">The Amazon S3 Client</param>
        /// <param name="request">The Copy Object Request</param>
        /// <param name="deleteSource">If set to true, the source object will be deleted if it was successfully copied</param>
        /// <param name="preferMultipart">If set to true, the method will use a multipart copy as long as the part size is less than the object size for any object, even
        /// those under 5 GiB.</param>
        /// <returns>The copy object response</returns>
        public static async Task<CopyObjectResponse> CopyOrMoveObjectAsync(this IAmazonS3 client, CopyObjectRequest request, bool deleteSource = false, bool preferMultipart = false)
        {
            if (preferMultipart)
            {
                // Use multi part as long as the part size is less than the object size
                return await CopyOrMoveObjectMultipartAsync(client, request, FIVE_MEBIBYTE, deleteSource);
            }
            else
            {
                // Use multi part when the object is over 5 GiB
                return await CopyOrMoveObjectAsync(client, request, FIVE_MEBIBYTE, deleteSource);
            }
        }
    }
}