using BAMCIS.AWSLambda.Common.SageMaker.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BAMCIS.AWSLambda.Common.SageMaker
{
    /// <summary>
    /// A response from a Factorization Machine predictor with binary classification
    /// </summary>
    public class FactorizationMachineBinaryInferenceResponse : InferencePredictionResponse<BinaryClassificationPrediction>
    {
        #region Constructors

        /// <summary>
        /// Creates the Factorization Machine inference response
        /// </summary>
        [JsonConstructor()]
        public FactorizationMachineBinaryInferenceResponse(IEnumerable<BinaryClassificationPrediction> predictions) : base(predictions) { }

        #endregion
    }
}
