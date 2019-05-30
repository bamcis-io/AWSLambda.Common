using BAMCIS.Lambda.Common.SageMaker.Serde;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BAMCIS.Lambda.Common.SageMaker.Model
{
    /// <summary>
    /// A Linear Learner multiclass prediction
    /// </summary>
    public class MulticlassClassificationPrediction : Prediction
    {
        #region Public Properties

        /// <summary>
        /// The multi class scores
        /// </summary>
        [JsonProperty(PropertyName = "score")]
        public IEnumerable<double> Score { get; }

        /// <summary>
        /// The predicted label
        /// </summary>
        [JsonProperty(PropertyName = "predicted_label")]
        [JsonConverter(typeof(PredictedLabelConverter))]
        public double PredictedLabel { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new multiclass classification prediction
        /// </summary>
        /// <param name="score"></param>
        /// <param name="predictedLabel"></param>
        [JsonConstructor()]
        public MulticlassClassificationPrediction(IEnumerable<double> score, double predictedLabel)
        {
            this.Score = score;
            this.PredictedLabel = predictedLabel;
        }

        #endregion
    }
}
