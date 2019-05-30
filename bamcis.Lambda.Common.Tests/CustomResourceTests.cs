using Amazon;
using Amazon.ElasticTranscoder;
using Amazon.ElasticTranscoder.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Amazon.S3;
using Amazon.S3.Model;
using BAMCIS.Lambda.Common;
using BAMCIS.Lambda.Common.CustomResources;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static BAMCIS.Lambda.Common.CustomResources.CustomResourceResponse;

namespace Lambda.Common.Tests
{
    public class CustomResourceTests
    {
        [Fact]
        public async Task CreateCustomResourceWithHandlerTest()
        {
            // ARRANGE
            string AccountNumber = "123456789012";
            string Region = "us-east-1";
            string InputBucket = $"{Environment.UserName}-rawvideo";
            string OutputBucket = $"{Environment.UserName}-video";
            string PresignedUrlBucket = $"{Environment.UserName}-presigned-url-test";
            string ThumbnailBucket = $"{Environment.UserName}-thumbnails";
            string IAMRole = $"arn:aws:iam::{AccountNumber}:role/LambdaElasticTranscoderPipeline";
            string NotificationTopic = $"arn:aws:sns:{Region}:{AccountNumber}:ElasticTranscoderNotifications";
            string Key = "result.txt";

            AWSConfigs.AWSProfilesLocation = $"{Environment.GetEnvironmentVariable("UserProfile")}\\.aws\\credentials";

            Mock<IAmazonS3> s3Client = new Mock<IAmazonS3>();
            s3Client.Setup(x => x.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>())).Returns($"https://{PresignedUrlBucket}.s3.amazonaws.com/{Key}?AWSAccessKeyId=AKIA1234567890123456&Expires=1559247929&Signature=OTgL4H7i%2FQOcTFpLM%2AV2LsFjONE%3D");

            GetPreSignedUrlRequest preSignedUrlRequest = new GetPreSignedUrlRequest()
            {
                BucketName = PresignedUrlBucket,
                Key = Key,
                Expires = DateTime.Now.AddMinutes(2),
                Protocol = Protocol.HTTPS,
                Verb = HttpVerb.PUT
            };

            string PreSignedUrl = s3Client.Object.GetPreSignedURL(preSignedUrlRequest);
            string Json = $@"
{{
""requestType"":""create"",
""responseUrl"":""{PreSignedUrl}"",
""stackId"":""arn:aws:cloudformation:{Region}:{AccountNumber}:stack/stack-name/{Guid.NewGuid().ToString()}"",
""requestId"":""12345678"",
""resourceType"":""Custom::TestResource"",
""logicalResourceId"":""MyTestResource"",
""resourceProperties"":{{
""Role"":""{IAMRole}"",
""Name"":""TestPipeline"",
""InputBucket"":""{InputBucket}"",
""Notifications"":{{
""Error"": ""{NotificationTopic}"",
}},
""ContentConfig"":{{
""Bucket"":""{OutputBucket}""
}},
""ThumbnailConfig"":{{
""Bucket"":""{ThumbnailBucket}""
}}
}}
}}";

            Json = Json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            Func<CustomResourceRequest, ILambdaContext, Task<CustomResourceResponse>> Create = async (request, context) =>
            {
                try
                {
                    //AmazonElasticTranscoderConfig Config = new AmazonElasticTranscoderConfig();
                    //IAmazonElasticTranscoder Client = new AmazonElasticTranscoderClient(Config);
                    Mock<IAmazonElasticTranscoder> mockClient = new Mock<IAmazonElasticTranscoder>();                    
                    mockClient.Setup(x => x.CreatePipelineAsync(It.IsAny<CreatePipelineRequest>(), default(CancellationToken)))
                    .ReturnsAsync(new CreatePipelineResponse()
                    {
                        HttpStatusCode = HttpStatusCode.OK
                    });

                    context.LogInfo("Attempting to create a pipeline.");
                    CreatePipelineRequest PipelineRequest = JsonConvert.DeserializeObject<CreatePipelineRequest>(JsonConvert.SerializeObject(request.ResourceProperties));
                    CreatePipelineResponse CreateResponse = await mockClient.Object.CreatePipelineAsync(PipelineRequest);

                    if ((int)CreateResponse.HttpStatusCode < 200 || (int)CreateResponse.HttpStatusCode > 299)
                    {
                        return new CustomResourceResponse(CustomResourceResponse.RequestStatus.FAILED, $"Received HTTP status code {(int)CreateResponse.HttpStatusCode}.", request);
                    }
                    else
                    {
                        return new CustomResourceResponse(
                            CustomResourceResponse.RequestStatus.SUCCESS,
                            $"See the details in CloudWatch Log Stream: {context.LogStreamName}.",
                            CreateResponse.Pipeline.Id,
                            request.StackId,
                            request.RequestId,
                            request.LogicalResourceId,
                            false,
                            new Dictionary<string, object>()
                            {
                            {"Name", CreateResponse.Pipeline.Name },
                            {"Arn", CreateResponse.Pipeline.Arn },
                            {"Id", CreateResponse.Pipeline.Id }
                            }
                        );
                    }
                }
                catch (AmazonElasticTranscoderException e)
                {
                    context.LogError(e);

                    return new CustomResourceResponse(
                        CustomResourceResponse.RequestStatus.FAILED,
                        e.Message,
                        Guid.NewGuid().ToString(),
                        request.StackId,
                        request.RequestId,
                        request.LogicalResourceId
                    );
                }
                catch (Exception e)
                {
                    context.LogError(e);

                    return new CustomResourceResponse(
                        CustomResourceResponse.RequestStatus.FAILED,
                        e.Message,
                        Guid.NewGuid().ToString(),
                        request.StackId,
                        request.RequestId,
                        request.LogicalResourceId
                    );
                }
            };

            Func<CustomResourceRequest, ILambdaContext, Task<CustomResourceResponse>> Update = async (request, context) =>
            {
                try
                {
                    context.LogInfo("Initiating update for pipeline.");

                    UpdatePipelineRequest PipelineRequest = JsonConvert.DeserializeObject<UpdatePipelineRequest>(JsonConvert.SerializeObject(request.ResourceProperties));

                    ListPipelinesRequest Listing = new ListPipelinesRequest();

                    List<Pipeline> Pipelines = new List<Pipeline>();
                    ListPipelinesResponse Pipes;

                    AmazonElasticTranscoderConfig Config = new AmazonElasticTranscoderConfig();
                    IAmazonElasticTranscoder Client = new AmazonElasticTranscoderClient(Config);

                    do
                    {
                        Pipes = await Client.ListPipelinesAsync(Listing);

                        Pipelines.AddRange(Pipes.Pipelines.Where(x => x.Name.Equals(request.ResourceProperties["Name"] as string) &&
                            x.InputBucket.Equals(request.ResourceProperties["InputBucket"]) &&
                            x.Role.Equals(request.ResourceProperties["Role"])
                        ));

                    } while (Pipes.NextPageToken != null);

                    if (Pipelines.Count > 1)
                    {
                        context.LogWarning($"{Pipelines.Count} pipelines were found matching the Name, InputBucket, and Role specified.");
                    }

                    if (Pipelines.Count > 0)
                    {
                        PipelineRequest.Id = Pipelines.First().Id;

                        UpdatePipelineResponse UpdateResponse = await Client.UpdatePipelineAsync(PipelineRequest);

                        if ((int)UpdateResponse.HttpStatusCode < 200 || (int)UpdateResponse.HttpStatusCode > 299)
                        {
                            return new CustomResourceResponse(CustomResourceResponse.RequestStatus.FAILED, $"Received HTTP status code {(int)UpdateResponse.HttpStatusCode}.", request);
                        }
                        else
                        {
                            return new CustomResourceResponse(
                                CustomResourceResponse.RequestStatus.SUCCESS,
                                $"See the details in CloudWatch Log Stream: {context.LogStreamName}.",
                                request,
                                false,
                                new Dictionary<string, object>()
                                {
                                {"Name", UpdateResponse.Pipeline.Name },
                                {"Arn", UpdateResponse.Pipeline.Arn },
                                {"Id", UpdateResponse.Pipeline.Id }
                                }
                            );
                        }
                    }
                    else
                    {
                        return new CustomResourceResponse(
                            CustomResourceResponse.RequestStatus.FAILED,
                            "No pipelines could be found with the matching characteristics.",
                            request
                        );
                    }
                }
                catch (AmazonElasticTranscoderException e)
                {
                    return new CustomResourceResponse(
                        CustomResourceResponse.RequestStatus.FAILED,
                        e.Message,
                        request
                    );
                }
                catch (Exception e)
                {
                    return new CustomResourceResponse(
                        CustomResourceResponse.RequestStatus.FAILED,
                        e.Message,
                        request
                    );
                }
            };

            Func<CustomResourceRequest, ILambdaContext, Task<CustomResourceResponse>> Delete = async (request, context) =>
            {
                try
                {
                    context.LogInfo("Attempting to delete a pipeline.");

                    ListPipelinesRequest Listing = new ListPipelinesRequest();

                    List<Pipeline> Pipelines = new List<Pipeline>();
                    ListPipelinesResponse Pipes;

                    AmazonElasticTranscoderConfig Config = new AmazonElasticTranscoderConfig();
                    IAmazonElasticTranscoder Client = new AmazonElasticTranscoderClient(Config);

                    do
                    {
                        Pipes = await Client.ListPipelinesAsync(Listing);

                        Pipelines.AddRange(Pipes.Pipelines.Where(x => x.Name.Equals(request.ResourceProperties["Name"] as string) &&
                            x.InputBucket.Equals(request.ResourceProperties["InputBucket"]) &&
                            x.Role.Equals(request.ResourceProperties["Role"])
                        ));

                    } while (Pipes.NextPageToken != null);

                    if (Pipelines.Count > 1)
                    {
                        context.LogWarning($"{Pipelines.Count} pipelines were found matching the Name, InputBucket, and Role specified.");
                    }

                    if (Pipelines.Count > 0)
                    {
                        DeletePipelineRequest PipelineRequest = new DeletePipelineRequest()
                        {
                            Id = Pipelines.First().Id
                        };

                        DeletePipelineResponse DeleteResponse = await Client.DeletePipelineAsync(PipelineRequest);

                        if ((int)DeleteResponse.HttpStatusCode < 200 || (int)DeleteResponse.HttpStatusCode > 299)
                        {
                            return new CustomResourceResponse(CustomResourceResponse.RequestStatus.FAILED, $"Received HTTP status code {(int)DeleteResponse.HttpStatusCode}.", request);
                        }
                        else
                        {
                            return new CustomResourceResponse(
                                CustomResourceResponse.RequestStatus.SUCCESS,
                                $"See the details in CloudWatch Log Stream: {context.LogStreamName}.",
                                request,
                                false
                            );
                        }
                    }
                    else
                    {
                        return new CustomResourceResponse(
                            CustomResourceResponse.RequestStatus.SUCCESS,
                            "No pipelines could be found with the matching characteristics.",
                            request
                        );
                    }
                }
                catch (AmazonElasticTranscoderException e)
                {
                    // If the pipeline doesn't exist, consider it deleted
                    if (e.StatusCode == HttpStatusCode.NotFound)
                    {
                        return new CustomResourceResponse(
                            CustomResourceResponse.RequestStatus.SUCCESS,
                            $"See the details in CloudWatch Log Stream: {context.LogStreamName}.",
                            request
                        );
                    }
                    else
                    {
                        return new CustomResourceResponse(
                            CustomResourceResponse.RequestStatus.FAILED,
                            e.Message,
                            request
                        );
                    }
                }
                catch (Exception e)
                {
                    return new CustomResourceResponse(
                        CustomResourceResponse.RequestStatus.FAILED,
                        e.Message,
                        request
                    );
                }
            };

            CustomResourceRequest customResourceRequest = JsonConvert.DeserializeObject<CustomResourceRequest>(Json);
            Mock<ICustomResourceHelper> mockHelper = new Mock<ICustomResourceHelper>();
            mockHelper.Setup(x => x.PutCustomResourceResponseAsync(It.IsAny<CustomResourceRequest>(), It.IsAny<CustomResourceResponse>()))
                .ReturnsAsync(new CustomResourceResult(customResourceRequest, new CustomResourceResponse(RequestStatus.SUCCESS, "", customResourceRequest), new HttpResponseMessage(HttpStatusCode.OK)));

            ICustomResourceHandler Handler = new CustomResourceFactory(Create, Update, Delete, mockHelper.Object);

            TestLambdaLogger TestLogger = new TestLambdaLogger();
            TestClientContext ClientContext = new TestClientContext();

            TestLambdaContext Context = new TestLambdaContext()
            {
                FunctionName = "ElasticTranscoderPipelineCreation",
                FunctionVersion = "1",
                Logger = TestLogger,
                ClientContext = ClientContext,
                LogGroupName = "aws/lambda/ElasticTranscoderPipeline",
                LogStreamName = Guid.NewGuid().ToString()
            };

            // ACT

            CustomResourceResult Response = await Handler.ExecuteAsync(customResourceRequest, Context);

            // ASSERT
            Assert.NotNull(Response);
            Assert.NotNull(Response.Response);
            Assert.NotNull(Response.S3Response);
            Assert.True(Response.IsSuccess);
        }

        [Fact]
        public void TestFixUpJsonResponse()
        {
            // ARRANGE

            CustomResourceResponse Response = new CustomResourceResponse(
                CustomResourceResponse.RequestStatus.SUCCESS,
                $"See the details in CloudWatch Log Stream: ",
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                false,
                new Dictionary<string, object>()
                {
                    {"Name", "name" },
                    {"Arn", "arn:aws:s3:us-east-1:123456789012:mybucket" },
                    {"Id", "mybucket" }
                }
            );

            ICustomResourceHelper helper = new DefaultCustomResourceHelper();

            // ACT

            string Content = helper.FixUpResponseJson(Response, CustomResourceRequest.StackOperation.DELETE);

            // ASSERT

            Assert.True(!String.IsNullOrEmpty(Content));
        }
    }
}
