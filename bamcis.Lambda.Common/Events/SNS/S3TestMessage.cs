using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace BAMCIS.Lambda.Common.Events.SNS
{
    /// <summary>
    /// Represents the test message generated when an S3 bucket is first configured to send
    /// SNS notifications for events.
    /// 
    /// This is the contents of the SNSEvent.Records[0].Message property:
    /// {  
    ///     "Service":"Amazon S3",
    ///     "Event":"s3:TestEvent",
    ///     "Time":"2014-10-13T15:57:02.089Z",
    ///     "Bucket":"bucketname",
    ///     "RequestId":"5582815E1AEA5ADF",
    ///     "HostId":"8cLeGAmw098X5cv4Zkwcmo8vvZa3eH3eKxsPzbB9wrR+YstdA6Knx4Ip8EXAMPLE"
    /// }
    /// </summary>
    public class S3TestMessage
    {
        #region Private Fields

        private static Regex _TestMessage = new Regex("^{.*,\\s*\"Event\"\\s*:\\s*\"s3:TestEvent\"\\s*,.*}$", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        #endregion

        #region Public Properties

        /// <summary>
        /// The service sending the test message, Amazon S3
        /// </summary>
        public string Service { get; }

        /// <summary>
        /// The S3 event, s3:TestEvent
        /// </summary>
        public string Event { get; }

        /// <summary>
        /// The timestamp the message was sent
        /// </summary>
        public DateTime Time { get; }

        /// <summary>
        /// The bucket that initiated the test message
        /// </summary>
        public string Bucket { get; }

        /// <summary>
        /// The request Id
        /// </summary>
        public string RequestId { get; }

        /// <summary>
        /// The host Id
        /// </summary>
        public string HostId { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new test message
        /// </summary>
        /// <param name="service"></param>
        /// <param name="event"></param>
        /// <param name="time"></param>
        /// <param name="bucket"></param>
        /// <param name="requestId"></param>
        /// <param name="hostId"></param>
        [JsonConstructor()]
        public S3TestMessage(
            string service,
            string @event,
            DateTime time,
            string bucket,
            string requestId,
            string hostId
        )
        {
            this.Service = service;
            this.Event = @event;
            this.Time = time;
            this.Bucket = bucket;
            this.RequestId = requestId;
            this.HostId = hostId;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Performs a regex match on the json string data to look for the \"Event\": \"s3:TestEvent\" 
        /// content in the json object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static bool IsTestMessage(string json)
        {
            return _TestMessage.IsMatch(json);
        }

        #endregion
    }
}
