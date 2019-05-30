using Newtonsoft.Json;
using System;

namespace BAMCIS.Lambda.Common.Events.KinesisFirehose
{
    /// <summary>
    /// The metadata associated with a record whose source to Kinesis
    /// Firehose was a Kinesis Stream
    /// </summary>
    public class KinesisStreamRecordMetadata
    {
        #region Public Properties

        /// <summary>
        /// The Kinesis Stream Shard Id
        /// </summary>
        public string ShardId { get; }

        /// <summary>
        /// The Kinsis Stream partition key
        /// </summary>
        public string PartitionKey { get; }

        /// <summary>
        /// The Unix timestamp of the approximate arrival of the 
        /// record into the Kinesis Stream
        /// </summary>
        public Int64 ApproximateArrivalTimestamp { get; }

        /// <summary>
        /// The record sequence number
        /// </summary>
        public string SequenceNumber { get; }

        /// <summary>
        /// The sub sequenc number, if one exists
        /// </summary>
        public string SubsequenceNumber { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Kinesis Stream Record Metadata object
        /// </summary>
        /// <param name="shardId"></param>
        /// <param name="partitionKey"></param>
        /// <param name="approximateArrivalTimestamp"></param>
        /// <param name="sequenceNumber"></param>
        /// <param name="subsequenceNumber"></param>
        [JsonConstructor()]
        public KinesisStreamRecordMetadata(
            string shardId,
            string partitionKey,
            Int64 approximateArrivalTimestamp,
            string sequenceNumber,
            string subsequenceNumber)
        {
            this.ShardId = shardId;
            this.PartitionKey = partitionKey;
            this.ApproximateArrivalTimestamp = approximateArrivalTimestamp;
            this.SequenceNumber = sequenceNumber;
            this.SubsequenceNumber = subsequenceNumber;
        }

        #endregion
    }
}
