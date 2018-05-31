using BAMCIS.AWSLambda.Common.Events;
using Newtonsoft.Json;
using Xunit;

namespace AWSLambda.Common.Tests
{
    public class EventTests
    {
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
    }
}
