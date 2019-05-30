using BAMCIS.Lambda.Common.SageMaker.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BAMCIS.Lambda.Common.SageMaker
{
    /// <summary>
    /// A response from a Linear Learner algorithm using Regression
    /// </summary>
    public class LinearLearnerRegressionInferenceResponse : InferencePredictionResponse<RegressionClassificationPrediction>
    {
        #region Constructors

        /// <summary>
        /// Creates the Linear Learner inference response
        /// </summary>
        [JsonConstructor()]
        public LinearLearnerRegressionInferenceResponse(IEnumerable<RegressionClassificationPrediction> predictions) : base(predictions) { }

        #endregion
    }
}
