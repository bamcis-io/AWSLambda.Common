using BAMCIS.AWSLambda.Common.SageMaker.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BAMCIS.AWSLambda.Common.SageMaker
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
