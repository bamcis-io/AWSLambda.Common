using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace BAMCIS.AWSLambda.Common.Events.KinesisFirehose
{
    /// <summary>
    /// The object that holds the results of the transformation for a single 
    /// record. This class uses the "Build" method to create new transformed records
    /// </summary>
    public class KinesisFirehoseTransformedRecord
    {
        #region Public Properties

        // Kinesis firehose is case sensitive in its JSON parsing, so this
        // needs to be lower case

        /// <summary>
        /// The Id of the record, this must match the source record
        /// </summary>
        [JsonProperty(PropertyName = "recordId")]
        public string RecordId { get; }

        /// <summary>
        /// The base64 encoded data
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public string Data { get; }

        /// <summary>
        /// The result of transformation
        /// </summary>
        [JsonProperty(PropertyName = "result")]
        public TransformationResultStatus Result { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new transformed record
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="data">The data should already be Base64 encoded</param>
        /// <param name="result"></param>
        [JsonConstructor()]
        private KinesisFirehoseTransformedRecord(
            string recordId,
            string data,
            TransformationResultStatus result)
        {
            this.RecordId = recordId;
            this.Data = data;
            this.Result = result;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new transformed record decoding the base64 data in the kinesis firehose record with UTF8
        /// </summary>
        /// <param name="record"></param>
        /// <param name="transform"></param>
        /// <returns>
        /// The output of the transformation function, or if an unhandled exception occurs,
        /// the exception type and message in a processing failed record.
        /// </returns>
        public static KinesisFirehoseTransformedRecord Build(KinesisFirehoseRecord record, Func<string, TransformationResult> transform)
        {
            return Build(record, transform, Encoding.UTF8);
        }

        /// <summary>
        /// Creates a new transformed record
        /// </summary>
        /// <param name="record"></param>
        /// <param name="transform"></param>
        /// <param name="encoding">The encoding to use to convert from the record's base64 string</param>
        /// <returns>
        /// The output of the transformation function, or if an unhandled exception occurs,
        /// the exception type and message in a processing failed record.
        /// </returns>
        public static KinesisFirehoseTransformedRecord Build(KinesisFirehoseRecord record, Func<string, TransformationResult> transform, Encoding encoding)
        {
            try
            {
                TransformationResult Result = transform.Invoke(record.DecodeData(encoding));
                return new KinesisFirehoseTransformedRecord(record.RecordId, Result.Data, Result.Result);
            }
            catch (Exception e)
            {
                return new KinesisFirehoseTransformedRecord(record.RecordId, Convert.ToBase64String(Encoding.UTF8.GetBytes($"{e.GetType().FullName} : {e.Message}")), TransformationResultStatus.PROCESSING_FAILED);
            }
        }

        /// <summary>
        /// Creates a new transformed record from an async function
        /// </summary>
        /// <param name="record"></param>
        /// <param name="transform"></param>
        /// <returns>
        /// The output of the transformation function, or if an unhandled exception occurs,
        /// the exception type and message in a processing failed record.
        /// </returns>
        public static async Task<KinesisFirehoseTransformedRecord> BuildAsync(KinesisFirehoseRecord record, Func<string, Task<TransformationResult>> transform)
        {
            return await BuildAsync(record, transform, Encoding.UTF8);
        }

        /// <summary>
        /// Creates a new transformed record from an async function
        /// </summary>
        /// <param name="record"></param>
        /// <param name="transform"></param>
        /// <returns>
        /// The output of the transformation function, or if an unhandled exception occurs,
        /// the exception type and message in a processing failed record.
        /// </returns>
        public static async Task<KinesisFirehoseTransformedRecord> BuildAsync(KinesisFirehoseRecord record, Func<string, Task<TransformationResult>> transform, Encoding encoding)
        {
            try
            {
                TransformationResult Result = await transform.Invoke(record.DecodeData(encoding));
                return new KinesisFirehoseTransformedRecord(record.RecordId, Result.Data, Result.Result);
            }
            catch (AggregateException e)
            {
                return new KinesisFirehoseTransformedRecord(record.RecordId, Convert.ToBase64String(Encoding.UTF8.GetBytes($"{e.InnerException.GetType().FullName} : {e.InnerException.Message}")), TransformationResultStatus.PROCESSING_FAILED);
            }
            catch (Exception e)
            {
                return new KinesisFirehoseTransformedRecord(record.RecordId, Convert.ToBase64String(Encoding.UTF8.GetBytes($"{e.GetType().FullName} : {e.Message}")), TransformationResultStatus.PROCESSING_FAILED);
            }
        }

        #endregion
    }
}
