using BAMCIS.Lambda.Common.SageMaker.Serde;
using Newtonsoft.Json;

namespace BAMCIS.Lambda.Common.SageMaker.Model
{
    /// <summary>
    /// The binary classification prediction for an input
    /// </summary>
    public class BinaryClassificationPrediction : Prediction
    {
        #region Public Properties

        /// <summary>
        /// The confidence level
        /// </summary>
        [JsonProperty(PropertyName = "score")]
        public double Score { get; }

        /// <summary>
        /// The predicted label, 0 or 1
        /// </summary>
        [JsonProperty(PropertyName = "predicted_label")]
        [JsonConverter(typeof(PredictedLabelConverter))]
        public double PredictedLabel { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new binary classification prediction
        /// </summary>
        /// <param name="score"></param>
        /// <param name="predictedLabel"></param>
        [JsonConstructor()]
        public BinaryClassificationPrediction(double score, double predictedLabel)
        {
            this.Score = score;
            this.PredictedLabel = predictedLabel;
        }

        #endregion
    }
}
