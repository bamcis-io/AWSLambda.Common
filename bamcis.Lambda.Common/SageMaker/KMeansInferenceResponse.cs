using BAMCIS.Lambda.Common.SageMaker.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BAMCIS.Lambda.Common.SageMaker
{
    /// <summary>
    /// The response from a K-Means prediction algorithm
    /// </summary>
    public class KMeansInferenceResponse : InferencePredictionResponse<KMeansPrediction>
    {
        #region Constructors

        /// <summary>
        /// Creates a new K-Means inference prediction response
        /// </summary>
        /// <param name="predictions"></param>
        [JsonConstructor()]
        public KMeansInferenceResponse(IEnumerable<KMeansPrediction> predictions) : base(predictions) { }

        #endregion
    }
}
