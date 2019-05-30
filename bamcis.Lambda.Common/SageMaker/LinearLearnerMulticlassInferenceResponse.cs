using BAMCIS.Lambda.Common.SageMaker.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BAMCIS.Lambda.Common.SageMaker
{
    /// <summary>
    /// A response from a Linear Learner algorithm using multiclass classification
    /// </summary>
    public class LinearLearnerMulticlassInferenceResponse : InferencePredictionResponse<MulticlassClassificationPrediction>
    {
        #region Constructors

        /// <summary>
        /// Creates the Linear Learner inference response
        /// </summary>
        [JsonConstructor()]
        public LinearLearnerMulticlassInferenceResponse(IEnumerable<MulticlassClassificationPrediction> predictions) : base(predictions) { }

        #endregion
    }
}
