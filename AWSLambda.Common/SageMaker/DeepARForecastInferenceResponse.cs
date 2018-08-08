using BAMCIS.AWSLambda.Common.SageMaker.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BAMCIS.AWSLambda.Common.SageMaker
{
    /// <summary>
    /// A response from a DeepAR Forecast inference
    /// </summary>
    public class DeepARForecastInferenceResponse : InferencePredictionResponse<DeepARForecastPrediction>
    {
        #region Constructors

        /// <summary>
        /// Creates the DeepAR Forecast inference response
        /// </summary>
        [JsonConstructor()]
        public DeepARForecastInferenceResponse(IEnumerable<DeepARForecastPrediction> predictions) : base(predictions) { }

        #endregion
    }
}
