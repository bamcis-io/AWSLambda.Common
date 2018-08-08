using Newtonsoft.Json;

namespace BAMCIS.AWSLambda.Common.SageMaker.Model
{
    /// <summary>
    /// A prediction for a K-Nearest Neighbor algorithm
    /// </summary>
    public class KNearestNeighborPrediction : Prediction
    {
        #region Public Properties

        /// <summary>
        /// The predicted label for the nearest neighbor
        /// </summary>
        [JsonProperty(PropertyName = "predicted_label")]
        public double PredictedLabel { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new K-Nearest neighbor prediction
        /// </summary>
        /// <param name="predictedLabel"></param>
        public KNearestNeighborPrediction(double predictedLabel)
        {
            this.PredictedLabel = predictedLabel;
        }

        #endregion
    }
}
