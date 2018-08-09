using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace BAMCIS.AWSLambda.Common.Events.KinesisFirehose
{
    /// <summary>
    /// The object that holds the results of the transformation for a single 
    /// record. This class uses the "Build" method to create new transformed records
    /// </summary>
    public class KinesisFirehoseTransformedRecord
    {
        #region Public Properties

        // Kinesis firehose is case sensitive in its JSON parsing, so this
        // needs to be lower case

        /// <summary>
        /// The Id of the record, this must match the source record
        /// </summary>
        [JsonProperty(PropertyName = "recordId")]
        public string RecordId { get; }

        /// <summary>
        /// The base64 encoded data
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public string Data { get; }

        /// <summary>
        /// The result of transformation
        /// </summary>
        [JsonProperty(PropertyName = "result")]
        public TransformationResultStatus Result { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new transformed record
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="data">The data should already be Base64 encoded</param>
        /// <param name="result"></param>
        [JsonConstructor()]
        private KinesisFirehoseTransformedRecord(
            string recordId,
            string data,
            TransformationResultStatus result)
        {
            this.RecordId = recordId;
            this.Data = data;
            this.Result = result;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new transformed record. By default the Kinesis Firehose Base64 data is decoded into UTF8 before being passed to the tranform function.
        /// </summary>
        /// <param name="record">The Kinesis Firehose record</param>
        /// <param name="transform">The function that will transform the data from the Kinesis Firehose record</param>
        /// <param name="useDefaultEncoding">Specifies if the data in the Kinesis Firehose record should be decoded using
        /// the default text encoding, UTF8. If this is false, the data will be passed to the transform function as a Base64 encoded string</param>
        /// <returns>The transformed record</returns>
        public static KinesisFirehoseTransformedRecord Build(KinesisFirehoseRecord record, Func<string, TransformationResult> transform, bool useDefaultEncoding = true)
        {
            ParameterTests.NonNull(record, "record");
            ParameterTests.NonNull(transform, "transform");

            try
            {
                string Data = record.Data;

                if (useDefaultEncoding)
                {
                    Data = record.DecodeData();
                }

                TransformationResult Result = transform.Invoke(Data);
                return new KinesisFirehoseTransformedRecord(record.RecordId, Result.Data, Result.Result);

            }
            catch (Exception e)
            {
                return new KinesisFirehoseTransformedRecord(record.RecordId, Convert.ToBase64String(Encoding.UTF8.GetBytes($"{e.GetType().FullName} : {e.Message}")), TransformationResultStatus.PROCESSING_FAILED);
            }
        }

        /// <summary>
        /// Creates a new transformed record. The Kinesis Firehose Base64 data is decided using the provided encoding parameter before being passed to the
        /// transform function.
        /// </summary>
        /// <param name="record">The Kinesis Firehose record</param>
        /// <param name="transform">The function that will transform the data from the Kinesis Firehose record</param>
        /// <param name="encoding">The encoding used to convert the bytes from the Base64 string into a readable string</param>
        /// <returns>The transformed record</returns>
        public static KinesisFirehoseTransformedRecord Build(KinesisFirehoseRecord record, Func<string, TransformationResult> transform, Encoding encoding)
        {
            ParameterTests.NonNull(record, "record");
            ParameterTests.NonNull(transform, "transform");
            ParameterTests.NonNull(encoding, "encoding");

            try
            {
                TransformationResult Result = transform.Invoke(record.DecodeData(encoding));
                return new KinesisFirehoseTransformedRecord(record.RecordId, Result.Data, Result.Result);
            }
            catch (Exception e)
            {
                return new KinesisFirehoseTransformedRecord(record.RecordId, Convert.ToBase64String(Encoding.UTF8.GetBytes($"{e.GetType().FullName} : {e.Message}")), TransformationResultStatus.PROCESSING_FAILED);
            }
        }

        /// <summary>
        /// Creates a new transformed record. By default the Kinesis Firehose Base64 data is decoded into UTF8 before being passed to the tranform function.
        /// </summary>
        /// <param name="record">The Kinesis Firehose record</param>
        /// <param name="transform">The function that will transform the data from the Kinesis Firehose record</param>
        /// <param name="useDefaultEncoding">Specifies if the data in the Kinesis Firehose record should be decoded using
        /// the default text encoding, UTF8. If this is false, the data will be passed to the transform function as a Base64 encoded string</param>
        /// <returns>The transformed record</returns>
        public static async Task<KinesisFirehoseTransformedRecord> BuildAsync(KinesisFirehoseRecord record, Func<string, Task<TransformationResult>> transform, bool useDefaultEncoding = true)
        {
            ParameterTests.NonNull(record, "record");
            ParameterTests.NonNull(transform, "transform");

            try
            {
                string Data = record.Data;

                if (useDefaultEncoding)
                {
                    Data = record.DecodeData();
                }

                TransformationResult Result = await transform.Invoke(Data);
                return new KinesisFirehoseTransformedRecord(record.RecordId, Result.Data, Result.Result);
            }
            catch (AggregateException e)
            {
                return new KinesisFirehoseTransformedRecord(record.RecordId, Convert.ToBase64String(Encoding.UTF8.GetBytes($"{e.InnerException.GetType().FullName} : {e.InnerException.Message}")), TransformationResultStatus.PROCESSING_FAILED);
            }
            catch (Exception e)
            {
                return new KinesisFirehoseTransformedRecord(record.RecordId, Convert.ToBase64String(Encoding.UTF8.GetBytes($"{e.GetType().FullName} : {e.Message}")), TransformationResultStatus.PROCESSING_FAILED);
            }
        }

        /// <summary>
        /// Creates a new transformed record. The Kinesis Firehose Base64 data is decided using the provided encoding parameter before being passed to the
        /// transform function.
        /// </summary>
        /// <param name="record">The Kinesis Firehose record</param>
        /// <param name="transform">The function that will transform the data from the Kinesis Firehose record</param>
        /// <param name="encoding">The encoding used to convert the bytes from the Base64 string into a readable string</param>
        /// <returns>The transformed record</returns>
        public static async Task<KinesisFirehoseTransformedRecord> BuildAsync(KinesisFirehoseRecord record, Func<string, Task<TransformationResult>> transform, Encoding encoding)
        {
            ParameterTests.NonNull(record, "record");
            ParameterTests.NonNull(transform, "transform");
            ParameterTests.NonNull(encoding, "encoding");

            try
            {
                TransformationResult Result = await transform.Invoke(record.DecodeData(encoding));
                return new KinesisFirehoseTransformedRecord(record.RecordId, Result.Data, Result.Result);
            }
            catch (AggregateException e)
            {
                return new KinesisFirehoseTransformedRecord(record.RecordId, Convert.ToBase64String(Encoding.UTF8.GetBytes($"{e.InnerException.GetType().FullName} : {e.InnerException.Message}")), TransformationResultStatus.PROCESSING_FAILED);
            }
            catch (Exception e)
            {
                return new KinesisFirehoseTransformedRecord(record.RecordId, Convert.ToBase64String(Encoding.UTF8.GetBytes($"{e.GetType().FullName} : {e.Message}")), TransformationResultStatus.PROCESSING_FAILED);
            }
        }

        #endregion
    }
}
