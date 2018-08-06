using Newtonsoft.Json;
using System;
using System.Text;

namespace BAMCIS.AWSLambda.Common.Events.KinesisFirehose
{
    /// <summary>
    /// Represents a single record that Kinesis Firehose provides to a tranformation
    /// lambda function from sources like S3
    /// </summary>
    public class KinesisFirehoseRecord
    {
        #region Public Properties

        /// <summary>
        /// The record Id
        /// </summary>
        public string RecordId { get; }

        /// <summary>
        /// The base64 encoded data
        /// </summary>
        public string Data { get; }

        /// <summary>
        /// The approximate arrival Unix timestamp of when the message arrived 
        /// </summary>
        public Int64 ApproximateArrivalTimestamp { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Kinesis Firehose Record
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="data"></param>
        /// <param name="approximateArrivalTimestamp"></param>
        [JsonConstructor()]
        public KinesisFirehoseRecord(
            string recordId,
            string data,
            Int64 approximateArrivalTimestamp)
        {
            this.RecordId = recordId;
            this.Data = data;
            this.ApproximateArrivalTimestamp = approximateArrivalTimestamp;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a plain text string of the base64 encoded data. By default the function uses
        /// UTF-8 to convert the base64 bytes to a string
        /// </summary>
        /// <returns></returns>
        public string DecodeData(Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            return encoding.GetString(Convert.FromBase64String(this.Data));
        }

        #endregion
    }
}
