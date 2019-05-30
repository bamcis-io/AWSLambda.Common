using Newtonsoft.Json;
using System.Collections.Generic;

namespace BAMCIS.Lambda.Common.Events.KinesisFirehose
{
    /// <summary>
    /// The result containing all of the transformed records
    /// </summary>
    public class KinesisFirehoseTransformResponse
    {
        #region Public Properties
        // Kinesis firehose is case sensitive in its JSON parsing, so this
        // needs to be lower case
        [JsonProperty(PropertyName = "records")]
        public IEnumerable<KinesisFirehoseTransformedRecord> Records { get; }

        #endregion

        #region Constructors

        public KinesisFirehoseTransformResponse(IEnumerable<KinesisFirehoseTransformedRecord> records)
        {
            this.Records = records;
        }

        #endregion
    }
}
