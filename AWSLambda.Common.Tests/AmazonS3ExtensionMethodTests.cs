using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using BAMCIS.AWSLambda.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AWSLambda.Common.Tests
{
    public class AmazonS3ExtensionMethodTests
    {
        [Fact]
        public void TestConvertRequest()
        {
            // ARRANGE
            CopyObjectRequest CopyRequest = new CopyObjectRequest()
            {
                DestinationBucket = "destination-bucket",
                SourceBucket = "source-bucket",
                SourceKey = "source/key/test.txt",
                DestinationKey = "destination/key/test.txt",
                TagSet = new List<Tag>() { new Tag() { Key = "tag1", Value = "myval"} }
            };

            // ACT
            CopyPartRequest Request = CopyRequest.ConvertTo<CopyPartRequest>();

            // ASSERT
            Assert.NotNull(Request);
            Assert.Equal(CopyRequest.DestinationBucket, Request.DestinationBucket);
            Assert.Equal(CopyRequest.SourceBucket, Request.SourceBucket);
            Assert.Equal(CopyRequest.DestinationKey, Request.DestinationKey);
            Assert.Equal(CopyRequest.SourceKey, Request.SourceKey);
            Assert.Equal(CopyRequest.SourceVersionId, Request.SourceVersionId);

        }

        [Fact]
        public async Task CopyObjectTest()
        {
            // ARRANGE
            CopyObjectRequest Req = new CopyObjectRequest()
            {
                DestinationBucket = "mhaken-demo-cf",
                SourceBucket = "mhaken-demo-cf",
                SourceKey = "test.txt",
                DestinationKey = "test/file.txt",
            };

            AWSConfigs.AWSProfilesLocation = $"{Environment.GetEnvironmentVariable("UserProfile")}\\.aws\\credentials";

            IAmazonS3 Client = new AmazonS3Client();

            GetObjectMetadataRequest MetaReq = new GetObjectMetadataRequest()
            {
                BucketName = Req.SourceBucket,
                Key = Req.SourceKey
            };

            GetObjectMetadataResponse Meta = await Client.GetObjectMetadataAsync(MetaReq);

            // ACT
            CopyObjectResponse Response = await Client.CopyOrMoveObjectAsync(Req, 16777216, false);


            // ASSERT
            Assert.Equal(HttpStatusCode.OK, Response.HttpStatusCode);
            Assert.Equal(Meta.ETag, Response.ETag);
        }
    }
}
