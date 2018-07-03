using Amazon;
using Amazon.ElasticTranscoder;
using Amazon.ElasticTranscoder.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using BAMCIS.AWSLambda.Common;
using BAMCIS.AWSLambda.Common.Events;
using BAMCIS.AWSLambda.Common.Events.SNS;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using static BAMCIS.AWSLambda.Common.Events.CustomResourceResponse;

namespace AWSLambda.Common.Tests
{
    public class EventTests
    {
        [Fact]
        public void SNSS3EventTest()
        {
            // ARRANGE
            string Json = @"
{
""Records"":[
{
""eventVersion"":""2.0"",
""eventSource"":""aws:s3"",
""awsRegion"":""us-east-1"",
""eventTime"":""2018-05-02T12:11:00.000Z"",
""eventName"":""ObjectCreated:Put"",
""userIdentity"":{
""principalId"":""AIDAJDPLRKLG7UEXAMPLE""
},
""requestParameters"":{
""sourceIPAddress"":""127.0.0.1""
},
""responseElements"":{
""x-amz-request-id"":""C3D13FE58DE4C810"",
""x-amz-id-2"":""FMyUVURIY8/IgAtTv8xRjskZQpcIZ9KG4V5Wp6S7S/JRWeUWerMUE5JgHvANOjpD""
},
""s3"":{
""s3SchemaVersion"":""1.0"",
""configurationId"":""testConfigRule"",
""bucket"":{
""name"":""mybucket"",
""ownerIdentity"":{
""principalId"":""A3NL1KOZZKExample""
},
""arn"":""arn:aws:s3:::mybucket""
},
""object"":{
""key"":""HappyFace.jpg"",
""size"":1024,
""eTag"":""d41d8cd98f00b204e9800998ecf8427e"",
""versionId"":""096fKKXTRTtl3on89fVO.nfljtsv6qko"",
""sequencer"":""0055AED6DCD90281E5""
}
}
}
]
}";
            Json = Json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            SNSS3RecordSet RecordSet = JsonConvert.DeserializeObject<SNSS3RecordSet>(Json);

            JsonSerializerSettings settings = new JsonSerializerSettings();
            IsoDateTimeConverter dateConverter = new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'"
            };

            settings.Converters.Add(dateConverter);

            string Content = JsonConvert.SerializeObject(RecordSet, Formatting.None, settings);

            // ASSERT
            Assert.Equal(Json, Content, true, true, true);
        }

        [Fact]
        public void CloudWatchEventTest()
        {
            // ARRANGE
            string Json = @"
{
""version"":""0"",
""id"":""125e7841-c049-462d-86c2-4efa5f64e293"",""detail-type"":""Scheduled Event"",""source"":""aws.events"",
""account"":""415720405880"",
""time"":""2016-12-16T19:55:42Z"",
""region"":""us-east-1"",
""resources"":[
""arn:aws:events:us-east-1:415720405880:rule/BackupTest-GetGetBackups-X2YM3334N4JN""
],
""detail"":{}
}";
            Json = Json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            CloudWatchScheduledEvent Event = JsonConvert.DeserializeObject<CloudWatchScheduledEvent>(Json);
            string Content = JsonConvert.SerializeObject(Event, Formatting.None);

            // ASSERT
            Assert.Equal(Json, Content, true, true, true);
        }

        [Fact]
        public void CreateCustomResourceRequestTest()
        {
            // ARRANGE
            string Json = @"
{
""requestType"":""create"",
""responseUrl"":""https://s3.us-east-1.amazonaws.com/presigned-url/response.txt?X-Amz-Date=20180531T182534Z&X-Amz-SignedHeaders=host&X-Amz-Credential=AKIAIYLQNVRRFNZOCFFR%2F20170720%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Expires=604800&X-Amz-Security-Token=FQoDYXdzEJP%2F%2F%2F%2F%2F%2F%2F%2F%2F%2FwEaDOLWx95j90zPxGh7WSLdAVnoYoKC4gjrrR1xbokFWRRwutmuAmOxaIVcQqOy%2Fqxy%2FXQt3Iz%2FohuEEmI7%2FHPzShy%2BfgQtvfUeDaojrAx5q8fG9P1KuIfcedfkiU%2BCxpM2foyCGlXzoZuNlcF8ohm%2BaM3wh4%2BxQ%2FpShLl18cKiKEiw0QF1UQGj%2FsiEqzoM81vOSUVWL9SpTTkVq8EQHY1chYKBkBWt7eIQcxjTI2dQeYOohlrbnZ5Y1%2F1cxPgrbk6PkNFO3whAoliSjyRC8e4TSjIY2j3V6d9fUy4%2Fp6nLZIf9wuERL7xW9PjE6eZbKOHnw8sF&X-Amz-Signature=a14b3065ab822105e8d7892eb5dcc455ddd603c61e47520774a7289178af9ecc"",
""stackId"":""arn:aws:cloudformation:us-east-1:123456789012:stack/stack-name/f449b250-b969-11e0-a185-5081d0136786"",
""requestId"":""12345678"",
""resourceType"":""Custom::TestResource"",
""logicalResourceId"":""MyTestResource"",
""resourceProperties"":{
""name"":""Value"",
""digits"":[1,2,3],
""address"":{
""street"":""MyStreetName"",
""number"":1234,
""city"":""Washington D.C.""
}
}
}";

            Json = Json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            CustomResourceRequest Request = JsonConvert.DeserializeObject<CustomResourceRequest>(Json);
            string Content = JsonConvert.SerializeObject(Request, Formatting.None);

            // ASSERT
            Assert.Equal(Json, Content, true, true, true);
        }

        [Fact]
        public void UpdateCustomResourceRequestTest()
        {
            // ARRANGE
            string Json = @"
{
""requestType"":""update"",
""responseUrl"":""https://s3.us-east-1.amazonaws.com/presigned-url/response.txt?X-Amz-Date=20180531T182534Z&X-Amz-SignedHeaders=host&X-Amz-Credential=AKIAIYLQNVRRFNZOCFFR%2F20170720%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Expires=604800&X-Amz-Security-Token=FQoDYXdzEJP%2F%2F%2F%2F%2F%2F%2F%2F%2F%2FwEaDOLWx95j90zPxGh7WSLdAVnoYoKC4gjrrR1xbokFWRRwutmuAmOxaIVcQqOy%2Fqxy%2FXQt3Iz%2FohuEEmI7%2FHPzShy%2BfgQtvfUeDaojrAx5q8fG9P1KuIfcedfkiU%2BCxpM2foyCGlXzoZuNlcF8ohm%2BaM3wh4%2BxQ%2FpShLl18cKiKEiw0QF1UQGj%2FsiEqzoM81vOSUVWL9SpTTkVq8EQHY1chYKBkBWt7eIQcxjTI2dQeYOohlrbnZ5Y1%2F1cxPgrbk6PkNFO3whAoliSjyRC8e4TSjIY2j3V6d9fUy4%2Fp6nLZIf9wuERL7xW9PjE6eZbKOHnw8sF&X-Amz-Signature=a14b3065ab822105e8d7892eb5dcc455ddd603c61e47520774a7289178af9ecc"",
""stackId"":""arn:aws:cloudformation:us-east-1:123456789012:stack/stack-name/f449b250-b969-11e0-a185-5081d0136786"",
""requestId"":""12345678"",
""resourceType"":""Custom::TestResource"",
""logicalResourceId"":""MyTestResource"",
""physicalResourceId"":""TestResource1"",
""resourceProperties"":{
""name"":""Value"",
""digits"":[1,2,3],
""address"":{
""street"":""MyStreetName"",
""number"":1234,
""city"":""Washington D.C.""
}
},
""oldResourceProperties"":{
""name"":""Value"",
""digits"":[1,2,3],
""address"":{
""street"":""MyStreetName"",
""number"":5678,
""city"":""Washington D.C.""
}
}
}
";

            Json = Json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            CustomResourceRequest Request = JsonConvert.DeserializeObject<CustomResourceRequest>(Json);
            string Content = JsonConvert.SerializeObject(Request, Formatting.None);

            // ASSERT
            Assert.Equal(Json, Content, true, true, true);
        }

        [Fact]
        public void DeleteCustomResourceRequestTest()
        {
            // ARRANGE
            string Json = @"
{
""requestType"":""delete"",
""responseUrl"":""https://s3.us-east-1.amazonaws.com/presigned-url/response.txt?X-Amz-Date=20180531T182534Z&X-Amz-SignedHeaders=host&X-Amz-Credential=AKIAIYLQNVRRFNZOCFFR%2F20170720%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Expires=604800&X-Amz-Security-Token=FQoDYXdzEJP%2F%2F%2F%2F%2F%2F%2F%2F%2F%2FwEaDOLWx95j90zPxGh7WSLdAVnoYoKC4gjrrR1xbokFWRRwutmuAmOxaIVcQqOy%2Fqxy%2FXQt3Iz%2FohuEEmI7%2FHPzShy%2BfgQtvfUeDaojrAx5q8fG9P1KuIfcedfkiU%2BCxpM2foyCGlXzoZuNlcF8ohm%2BaM3wh4%2BxQ%2FpShLl18cKiKEiw0QF1UQGj%2FsiEqzoM81vOSUVWL9SpTTkVq8EQHY1chYKBkBWt7eIQcxjTI2dQeYOohlrbnZ5Y1%2F1cxPgrbk6PkNFO3whAoliSjyRC8e4TSjIY2j3V6d9fUy4%2Fp6nLZIf9wuERL7xW9PjE6eZbKOHnw8sF&X-Amz-Signature=a14b3065ab822105e8d7892eb5dcc455ddd603c61e47520774a7289178af9ecc"",
""stackId"":""arn:aws:cloudformation:us-east-1:123456789012:stack/stack-name/f449b250-b969-11e0-a185-5081d0136786"",
""requestId"":""12345678"",
""resourceType"":""Custom::TestResource"",
""logicalResourceId"":""MyTestResource"",
""physicalResourceId"":""TestResource1"",
""resourceProperties"":{
""name"":""Value"",
""digits"":[1,2,3],
""address"":{
""street"":""MyStreetName"",
""number"":1234,
""city"":""Washington D.C.""
}
}
}";

            Json = Json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            CustomResourceRequest Request = JsonConvert.DeserializeObject<CustomResourceRequest>(Json);
            string Content = JsonConvert.SerializeObject(Request, Formatting.None);

            // ASSERT
            Assert.Equal(Json, Content, true, true, true);
        }

        [Fact]
        public void SuccessCustomResourceResponseTest()
        {
            // ARRANGE
            string Json = @"
{
""status"":""SUCCESS"",
""reason"":""The request succeeded"",
""physicalResourceId"":""TestResource1"",
""stackId"":""arn:aws:cloudformation:us-east-1:123456789012:stack/stack-name/f449b250-b969-11e0-a185-5081d0136786"",
""requestId"":""12345678"",
""logicalResourceId"":""MyTestResource"",
""noEcho"":true,
""data"":{
""OutputName1"":""Value1"",
""OutputName2"":""Value2""
}
}";
            Json = Json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            CustomResourceResponse Response = JsonConvert.DeserializeObject<CustomResourceResponse>(Json);
            string Content = JsonConvert.SerializeObject(Response, Formatting.None);

            // ASSERT
            Assert.Equal(Json, Content, true, true, true);
        }

        [Fact]
        public void SuccessWithoutOptionalCustomResourceResponseTest()
        {
            // ARRANGE
            string Json = @"
{
""status"":""SUCCESS"",
""physicalResourceId"":""TestResource1"",
""stackId"":""arn:aws:cloudformation:us-east-1:123456789012:stack/stack-name/f449b250-b969-11e0-a185-5081d0136786"",
""requestId"":""12345678"",
""logicalResourceId"":""MyTestResource"",
""noEcho"":false
}";
            Json = Json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            CustomResourceResponse Response = JsonConvert.DeserializeObject<CustomResourceResponse>(Json);
            string Content = JsonConvert.SerializeObject(Response, Formatting.None);

            // ASSERT
            Assert.Equal(Json, Content, true, true, true);
        }

        [Fact]
        public void FailureCustomResourceResponseTest()
        {
            // ARRANGE
            string Json = @"
{
""status"":""FAILED"",
""reason"":""The request failed"",
""physicalResourceId"":""TestResource1"",
""stackId"":""arn:aws:cloudformation:us-east-1:123456789012:stack/stack-name/f449b250-b969-11e0-a185-5081d0136786"",
""requestId"":""12345678"",
""logicalResourceId"":""MyTestResource"",
""noEcho"":false
}";
            Json = Json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            CustomResourceResponse Response = JsonConvert.DeserializeObject<CustomResourceResponse>(Json);
            string Content = JsonConvert.SerializeObject(Response, Formatting.None);

            // ASSERT
            Assert.Equal(Json, Content, true, true, true);
        }

        [Fact]
        public void CreateResponseFromRequestTest()
        {
            // ARRANGE
            string Json = @"
{
""requestType"":""update"",
""responseUrl"":""https://s3.us-east-1.amazonaws.com/presigned-url/response.txt?X-Amz-Date=20180531T182534Z&X-Amz-SignedHeaders=host&X-Amz-Credential=AKIAIYLQNVRRFNZOCFFR%2F20170720%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Expires=604800&X-Amz-Security-Token=FQoDYXdzEJP%2F%2F%2F%2F%2F%2F%2F%2F%2F%2FwEaDOLWx95j90zPxGh7WSLdAVnoYoKC4gjrrR1xbokFWRRwutmuAmOxaIVcQqOy%2Fqxy%2FXQt3Iz%2FohuEEmI7%2FHPzShy%2BfgQtvfUeDaojrAx5q8fG9P1KuIfcedfkiU%2BCxpM2foyCGlXzoZuNlcF8ohm%2BaM3wh4%2BxQ%2FpShLl18cKiKEiw0QF1UQGj%2FsiEqzoM81vOSUVWL9SpTTkVq8EQHY1chYKBkBWt7eIQcxjTI2dQeYOohlrbnZ5Y1%2F1cxPgrbk6PkNFO3whAoliSjyRC8e4TSjIY2j3V6d9fUy4%2Fp6nLZIf9wuERL7xW9PjE6eZbKOHnw8sF&X-Amz-Signature=a14b3065ab822105e8d7892eb5dcc455ddd603c61e47520774a7289178af9ecc"",
""stackId"":""arn:aws:cloudformation:us-east-1:123456789012:stack/stack-name/f449b250-b969-11e0-a185-5081d0136786"",
""requestId"":""12345678"",
""resourceType"":""Custom::TestResource"",
""logicalResourceId"":""MyTestResource"",
""physicalResourceId"":""TestResource1"",
""resourceProperties"":{
""name"":""Value"",
""digits"":[1,2,3],
""address"":{
""street"":""MyStreetName"",
""number"":1234,
""city"":""Washington D.C.""
}
},
""oldResourceProperties"":{
""name"":""Value"",
""digits"":[1,2,3],
""address"":{
""street"":""MyStreetName"",
""number"":5678,
""city"":""Washington D.C.""
}
}
}
";
            Json = Json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");
            CustomResourceRequest Request = JsonConvert.DeserializeObject<CustomResourceRequest>(Json);

            // ACT
            CustomResourceResponse Response = new CustomResourceResponse(CustomResourceResponse.RequestStatus.SUCCESS, null, Request);

            // ASSERT    
        }

        [Fact]
        public async Task CreateCustomResourceWithHandlerTest()
        {
            // ARRANGE
            string AccountNumber = "123456789012";
            string Region = "us-east-1";
            string InputBucket = $"{Environment.UserName}-rawvideo";
            string OutputBucket = $"{Environment.UserName}-video";
            string ThumbnailBucket = $"{Environment.UserName}-thumbnails";
            string IAMRole = $"arn:aws:iam::{AccountNumber}:role/LambdaElasticTranscoderPipeline";
            string NotificationTopic = $"arn:aws:sns:{Region}:{AccountNumber}:ElasticTranscoderNotifications";

            string Json = $@"
{{
""requestType"":""create"",
""responseUrl"":""https://s3.{Region}.amazonaws.com/presigned-url/response.txt?X-Amz-Date=20180531T182534Z&X-Amz-SignedHeaders=host&X-Amz-Credential=AKIAIYLQNVRRFNZOCFFR%2F20170720%2F{Region}%2Fs3%2Faws4_request&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Expires=604800&X-Amz-Security-Token=FQoDYXdzEJP%2F%2F%2F%2F%2F%2F%2F%2F%2F%2FwEaDOLWx95j90zPxGh7WSLdAVnoYoKC4gjrrR1xbokFWRRwutmuAmOxaIVcQqOy%2Fqxy%2FXQt3Iz%2FohuEEmI7%2FHPzShy%2BfgQtvfUeDaojrAx5q8fG9P1KuIfcedfkiU%2BCxpM2foyCGlXzoZuNlcF8ohm%2BaM3wh4%2BxQ%2FpShLl18cKiKEiw0QF1UQGj%2FsiEqzoM81vOSUVWL9SpTTkVq8EQHY1chYKBkBWt7eIQcxjTI2dQeYOohlrbnZ5Y1%2F1cxPgrbk6PkNFO3whAoliSjyRC8e4TSjIY2j3V6d9fUy4%2Fp6nLZIf9wuERL7xW9PjE6eZbKOHnw8sF&X-Amz-Signature=a14b3065ab822105e8d7892eb5dcc455ddd603c61e47520774a7289178af9ecc"",
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
                    AmazonElasticTranscoderConfig Config = new AmazonElasticTranscoderConfig();
                    AmazonElasticTranscoderClient Client = new AmazonElasticTranscoderClient(Config);

                    context.LogInfo("Attempting to create a pipeline.");
                    CreatePipelineRequest PipelineRequest = JsonConvert.DeserializeObject<CreatePipelineRequest>(JsonConvert.SerializeObject(request.ResourceProperties));
                    CreatePipelineResponse CreateResponse = await Client.CreatePipelineAsync(PipelineRequest);

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
                    AmazonElasticTranscoderClient Client = new AmazonElasticTranscoderClient(Config);

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
                    AmazonElasticTranscoderClient Client = new AmazonElasticTranscoderClient(Config);

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

            CustomResourceRequest Request = JsonConvert.DeserializeObject<CustomResourceRequest>(Json);
            CustomResourceHandler Handler = new CustomResourceHandler(Create, Update, Delete);

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

            AWSConfigs.AWSProfilesLocation = $"{Environment.GetEnvironmentVariable("UserProfile")}\\.aws\\credentials";

            // ACT

            CustomResourceResult Response = await Handler.Execute(Request, Context);

            // ASSERT
            Assert.NotNull(Response);
            Assert.NotNull(Response.Response);
            Assert.Equal(RequestStatus.SUCCESS, Response.Response.Status);
            Assert.NotNull(Response.S3Response);
            Assert.True(Response.S3Response.IsSuccessStatusCode);
        }
    }
}
