using Newtonsoft.Json;

namespace BAMCIS.AWSLambda.Common.SageMaker.Model
{
    /// <summary>
    /// Represents a Linear Learner regression classification
    /// </summary>
    public class RegressionClassificationPrediction : Prediction
    {
        #region Public Properties

        /// <summary>
        /// The prediction score
        /// </summary>
        [JsonProperty(PropertyName = "score")]
        public double Score { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new regression classification predication
        /// </summary>
        /// <param name="score"></param>
        [JsonConstructor()]
        public RegressionClassificationPrediction(double score)
        {
            this.Score = score;
        }

        #endregion
    }
}
