using BAMCIS.Lambda.Common.SageMaker.Model;
using System.Collections.Generic;

namespace BAMCIS.Lambda.Common.SageMaker
{
    /// <summary>
    /// A response from an NTM inference
    /// </summary>
    public class NeuralTopicModelInferenceResponse : InferencePredictionResponse<NeuralTopicModelPrediction>
    {
        #region Constructors

        /// <summary>
        /// Creates the Neural Topic Model inference response
        /// </summary>
        public NeuralTopicModelInferenceResponse(IEnumerable<NeuralTopicModelPrediction> predictions) : base(predictions) { }

        #endregion
    }
}
