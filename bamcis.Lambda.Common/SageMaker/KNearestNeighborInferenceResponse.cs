using BAMCIS.Lambda.Common.SageMaker.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BAMCIS.Lambda.Common.SageMaker
{
    /// <summary>
    /// A response from a K-Nearest Neighbor inference
    /// </summary>
    public class KNearestNeighborInferenceResponse : InferencePredictionResponse<KNearestNeighborPrediction>
    {
        #region Constructors

        /// <summary>
        /// Creates the K-Nearest Neighbor inference response
        /// </summary>
        /// <param name="predictions"></param>
        [JsonConstructor()]
        public KNearestNeighborInferenceResponse(IEnumerable<KNearestNeighborPrediction> predictions) : base(predictions) { }

        #endregion
    }
}
