using Newtonsoft.Json;

namespace BAMCIS.Lambda.Common.Events.KinesisFirehose
{
    /// <summary>
    /// The result of a tranformation function on a Kinesis Firehose record
    /// </summary>
    public class TransformationResult
    {
        #region Public Properties

        /// <summary>
        /// The Base64 encoded transformed data record, this might be null or empty
        /// if the result status is Dropped or Processing Failed
        /// </summary>
        public string Data { get; }

        /// <summary>
        /// The result status of the tranformation
        /// </summary>
        public TransformationResultStatus Result { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new transformation result
        /// </summary>
        /// <param name="data"></param>
        /// <param name="result"></param>
        [JsonConstructor()]
        public TransformationResult(string data, TransformationResultStatus result)
        {
            this.Data = data;
            this.Result = result;
        }

        #endregion
    }
}
