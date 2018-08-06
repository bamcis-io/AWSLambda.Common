using Newtonsoft.Json;
using System;

namespace BAMCIS.AWSLambda.Common.Events.KinesisFirehose
{
    /// <summary>
    /// A Kinesis Firehose record that was sourced from a Kinesis Stream
    /// </summary>
    public class KinesisFirehoseKinesisStreamRecord : KinesisFirehoseRecord
    {
        #region Public Properties

        /// <summary>
        /// The metadata of the source Kinesis Stream
        /// </summary>
        public KinesisStreamRecordMetadata KinesisRecordMetadata { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Kinesis Firehose Kinesis Stream Record object
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="data"></param>
        /// <param name="approximateArrivalTimestamp"></param>
        /// <param name="kinesisRecordMetadata"></param>
        [JsonConstructor()]
        public KinesisFirehoseKinesisStreamRecord(
           string recordId,
           string data,
           Int64 approximateArrivalTimestamp,
           KinesisStreamRecordMetadata kinesisRecordMetadata) : base(recordId, data, approximateArrivalTimestamp)
        {
          this.KinesisRecordMetadata = kinesisRecordMetadata;
        }

        #endregion
    }
}
