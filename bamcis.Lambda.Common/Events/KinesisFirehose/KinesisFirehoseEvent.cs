using Newtonsoft.Json;
using System.Collections.Generic;

namespace BAMCIS.Lambda.Common.Events.KinesisFirehose
{
    /// <summary>
    /// The input data to a Kinesis Firehose Transofrmation Lambda function 
    /// </summary>
    public class KinesisFirehoseEvent<T> where T : KinesisFirehoseRecord
    {
        #region Public Properties

        /// <summary>
        /// The Kinesis stream records to transform
        /// </summary>
        public IEnumerable<T> Records { get; }

        /// <summary>
        /// The region of the Firehose Delivery Stream
        /// </summary>
        public string Region { get; }

        /// <summary>
        /// The ARN of the Firehose Delivery Stream
        /// </summary>
        public string DeliveryStreamArn { get; }

        /// <summary>
        /// The Id of the Lambda function invocation
        /// </summary>
        public string InvocationId { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Kinesis Firehose Event
        /// </summary>
        /// <param name="records"></param>
        /// <param name="region"></param>
        /// <param name="deliveryStreamArn"></param>
        /// <param name="invocationId"></param>
        [JsonConstructor()]
        public KinesisFirehoseEvent(
            IEnumerable<T> records,
            string region,
            string deliveryStreamArn,
            string invocationId)
        {
            this.Records = records;
            this.Region = region;
            this.DeliveryStreamArn = deliveryStreamArn;
            this.InvocationId = invocationId;
        }

        #endregion
    }

    /// <summary>
    /// Implements the default type for the records as KinesisFirehoseRecord
    /// </summary>
    public class KinesisFirehoseEvent : KinesisFirehoseEvent<KinesisFirehoseRecord>
    {
        /// <summary>
        /// Constructs a new new KinesisFirehoseEvent
        /// </summary>
        /// <param name="records"></param>
        /// <param name="region"></param>
        /// <param name="deliveryStreamArn"></param>
        /// <param name="invocationId"></param>
       public KinesisFirehoseEvent(
            IEnumerable<KinesisFirehoseRecord> records,
            string region,
            string deliveryStreamArn,
            string invocationId
        ) : base(records, region, deliveryStreamArn, invocationId)
        { }
    }
}
