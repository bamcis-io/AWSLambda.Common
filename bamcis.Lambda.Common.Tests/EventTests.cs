using Amazon.Lambda.TestUtilities;
using BAMCIS.Lambda.Common.CustomResources;
using BAMCIS.Lambda.Common.Events.KinesisFirehose;
using BAMCIS.Lambda.Common.Events.SNS;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Lambda.Common.Tests
{
    public class EventTests
    {
        string asgLifecycleSNSMessageData = @"
{
""LifecycleHookName"":""test2"",
""AccountId"":""252486826203"",
""RequestId"":""4a7f8796-7ed7-4b21-bff7-d190f91afccf"",
""LifecycleTransition"":""autoscaling:EC2_INSTANCE_TERMINATING"",
""AutoScalingGroupName"":""test"",
""Service"":""AWS Auto Scaling"",
""Time"":""2019-05-06T16:14:44.397Z"",
""EC2InstanceId"":""i-076c7bf793337129f"",
""LifecycleActionToken"":""c71ea497-6931-4a15-b047-8ec014ee33a7""
}";


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
        public void SNSS3EventTest2()
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
        public void SNSTestMessageEvent()
        {
            // ARRANGE
            string Json = @"{
""Service"":""Amazon S3"",
""Event"":""s3:TestEvent"",
""Time"":""2014-10-13T15:57:02.089Z"",
""Bucket"":""bucketname"",
""RequestId"":""5582815E1AEA5ADF"",
""HostId"":""8cLeGAmw098X5cv4Zkwcmo8vvZa3eH3eKxsPzbB9wrR+YstdA6Knx4Ip8EXAMPLE""
}";
            Json = Json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            S3TestMessage Message = JsonConvert.DeserializeObject<S3TestMessage>(Json);

            JsonSerializerSettings settings = new JsonSerializerSettings();
            IsoDateTimeConverter dateConverter = new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'"
            };

            settings.Converters.Add(dateConverter);

            string Content = JsonConvert.SerializeObject(Message, Formatting.None, settings);

            // ASSERT
            Assert.Equal(Json, Content, true, true, true);
        }

        [Fact]
        public void SNSTestMessageTestEventTrue()
        {
            // ARRANGE
            string Json = @"{
""Service"":""Amazon S3"",
""Event"":""s3:TestEvent"",
""Time"":""2014-10-13T15:57:02.089Z"",
""Bucket"":""bucketname"",
""RequestId"":""5582815E1AEA5ADF"",
""HostId"":""8cLeGAmw098X5cv4Zkwcmo8vvZa3eH3eKxsPzbB9wrR+YstdA6Knx4Ip8EXAMPLE""
}";
           
            // ACT
           

            // ASSERT
            Assert.True(S3TestMessage.IsTestMessage(Json));
        }

        [Fact]
        public void SNSTestMessageTestEventFalse()
        {
            // ARRANGE
            string Json = @"
""Event"":""s3:TestEvent""
}";

            // ACT


            // ASSERT
            Assert.False(S3TestMessage.IsTestMessage(Json));
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
        public void KinesisFirehoseEventTest()
        {
            // ARRANGE
            KinesisFirehoseEvent Event = new KinesisFirehoseEvent(
                new List<KinesisFirehoseRecord>() {
                    new KinesisFirehoseRecord(
                        "1",
                        Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"name\":\"Mike\",\"age\":34}")),
                        1234567890)
                    },
                "us-east-1",
                "arn:aws:firehose:us-east-1:123456789012:deliverystream/delivery-stream-name",
                "123"
            );

            // ACT
            string Json = JsonConvert.SerializeObject(Event);

            // ASSERT
            Assert.True(JsonConvert.DeserializeObject<KinesisFirehoseEvent>(Json) != null);
        }
     
        [Fact]
        public void KinesisFirehoseResponseLambdaFunctionTest()
        {
            // ARRANGE
            TestLambdaLogger TestLogger = new TestLambdaLogger();
            TestClientContext ClientContext = new TestClientContext();

            TestLambdaContext Context = new TestLambdaContext()
            {
                FunctionName = "KinesisFirehoseTest",
                FunctionVersion = "1",
                Logger = TestLogger,
                ClientContext = ClientContext
            };

            KinesisFirehoseEvent Event = new KinesisFirehoseEvent(
               new List<KinesisFirehoseRecord>() {
                    new KinesisFirehoseRecord(
                        "1",
                        Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"name\":\"Mike\",\"age\":34}")),
                        1234567890)
                   },
               "us-east-1",
               "arn:aws:firehose:us-east-1:123456789012:deliverystream/delivery-stream-name",
               "123"
           );

            KinesisFirehoseEventLambdaFunction Function = new KinesisFirehoseEventLambdaFunction();

            string Json = "";

            // ACT
            KinesisFirehoseTransformResponse Response = Function.Exec(Event, Context);
            using (MemoryStream MStream = new MemoryStream())
            {
                using (StreamReader Reader = new StreamReader(MStream))
                {
                    Amazon.Lambda.Serialization.Json.JsonSerializer Serde = new Amazon.Lambda.Serialization.Json.JsonSerializer();
                    Serde.Serialize(Response, MStream);
                    MStream.Position = 0;
                    Json = Reader.ReadToEnd();
                }                
            }

            // ASSERT
            Assert.True(!String.IsNullOrEmpty(Json));
        }

        [Fact]
        public async Task KinesisFirehoseResponseLambdaFunctionAsyncTest()
        {
            // ARRANGE
            TestLambdaLogger TestLogger = new TestLambdaLogger();
            TestClientContext ClientContext = new TestClientContext();

            TestLambdaContext Context = new TestLambdaContext()
            {
                FunctionName = "KinesisFirehoseTest",
                FunctionVersion = "1",
                Logger = TestLogger,
                ClientContext = ClientContext
            };

            KinesisFirehoseEvent Event = new KinesisFirehoseEvent(
               new List<KinesisFirehoseRecord>() {
                    new KinesisFirehoseRecord(
                        "1",
                        Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"name\":\"Mike\",\"age\":34}")),
                        1234567890)
                   },
               "us-east-1",
               "arn:aws:firehose:us-east-1:123456789012:deliverystream/delivery-stream-name",
               "123"
           );

            KinesisFirehoseEventLambdaFunction Function = new KinesisFirehoseEventLambdaFunction();

            string Json = "";

            // ACT
            KinesisFirehoseTransformResponse Response = await Function.ExecAsync(Event, Context);
            using (MemoryStream MStream = new MemoryStream())
            {
                using (StreamReader Reader = new StreamReader(MStream))
                {
                    Amazon.Lambda.Serialization.Json.JsonSerializer Serde = new Amazon.Lambda.Serialization.Json.JsonSerializer();
                    Serde.Serialize(Response, MStream);
                    MStream.Position = 0;
                    Json = Reader.ReadToEnd();
                }
            }

            // ASSERT
            Assert.True(!String.IsNullOrEmpty(Json));
        }

        [Fact]
        public void SNSAutoScalingLifecycleHookMessage()
        {
            // ARRANGE
           
            string Json = asgLifecycleSNSMessageData.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            SNSAutoScalingLifecycleHookMessage Response = JsonConvert.DeserializeObject<SNSAutoScalingLifecycleHookMessage>(Json);
            string Content = JsonConvert.SerializeObject(Response, Formatting.None);

            // ASSERT
            Assert.Equal(Json, Content, true, true, true);
        }
    }
}
