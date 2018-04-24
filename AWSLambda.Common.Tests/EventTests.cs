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
    }
}
