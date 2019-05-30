using Newtonsoft.Json;

namespace BAMCIS.Lambda.Common.SageMaker.Model
{
    /// <summary>
    /// A prediction for a K-Means algorithm
    /// </summary>
    public class KMeansPrediction : Prediction
    {
        #region Public Properties

        /// <summary>
        /// The closest cluster
        /// </summary>
        [JsonProperty(PropertyName = "closest_cluster")]
        public double ClosestCluster { get; }

        /// <summary>
        /// The distance to the closest cluster
        /// </summary>
        [JsonProperty(PropertyName = "distance_to_cluster")]
        public double DistanceToCluster { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new K-Means prediction
        /// </summary>
        /// <param name="closestCluster"></param>
        /// <param name="distanceToCluster"></param>
        [JsonConstructor()]
        public KMeansPrediction(double closestCluster, double distanceToCluster)
        {
            this.ClosestCluster = closestCluster;
            this.DistanceToCluster = distanceToCluster;
        }

        #endregion
    }
}
