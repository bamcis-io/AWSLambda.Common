using BAMCIS.AWSLambda.Common.SageMaker.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BAMCIS.AWSLambda.Common.SageMaker
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
