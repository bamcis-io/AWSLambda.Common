using BAMCIS.Lambda.Common.SageMaker.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BAMCIS.Lambda.Common.SageMaker
{
    /// <summary>
    /// A response from a Linear Learner algorithm using binary classification
    /// </summary>
    public class LinearLearnerBinaryInferenceResponse : InferencePredictionResponse<BinaryClassificationPrediction>
    {
        #region Constructors

        /// <summary>
        /// Creates the Linear Learner inference response
        /// </summary>
        [JsonConstructor()]
        public LinearLearnerBinaryInferenceResponse(IEnumerable<BinaryClassificationPrediction> predictions) : base(predictions) { }

        #endregion
    }
}
