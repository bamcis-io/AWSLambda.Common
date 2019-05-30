using BAMCIS.Lambda.Common.SageMaker.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BAMCIS.Lambda.Common.SageMaker
{
    /// <summary>
    /// A response from a Factorization Machine predictor with regression classification
    /// </summary>
    public class FactorizationMachineRegressionInferenceResponse : InferencePredictionResponse<RegressionClassificationPrediction>
    {
        #region Constructors

        /// <summary>
        /// Creates the Factorization Machine inference response
        /// </summary>
        [JsonConstructor()]
        public FactorizationMachineRegressionInferenceResponse(IEnumerable<RegressionClassificationPrediction> predictions) : base(predictions) { }

        #endregion
    }
}
