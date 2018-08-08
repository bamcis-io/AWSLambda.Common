using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BAMCIS.AWSLambda.Common.SageMaker.Model
{
    /// <summary>
    /// A prediction from a DeepAR Forecast
    /// </summary>
    public class DeepARForecastPrediction : Prediction
    {
        #region Public Properties

        /// <summary>
        /// The quantile fields
        /// </summary>
        [JsonProperty(PropertyName = "quantiles")]
        public IDictionary<string, IEnumerable<double>> Quantiles { get; }

        /// <summary>
        /// The samples
        /// </summary>
        [JsonProperty(PropertyName = "samples")]
        public IEnumerable<double> Samples { get; }

        /// <summary>
        /// The mean values
        /// </summary>
        [JsonProperty(PropertyName = "mean")]
        public IEnumerable<double> Mean { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new DeepAR Forecast Prediction
        /// </summary>
        /// <param name="quantiles"></param>
        /// <param name="samples"></param>
        /// <param name="mean"></param>
        public DeepARForecastPrediction(IDictionary<string, IEnumerable<double>> quantiles, IEnumerable<double> samples, IEnumerable<double> mean)
        {
            this.Quantiles = quantiles ?? throw new ArgumentNullException("quantiles");
            this.Samples = samples ?? throw new ArgumentNullException("samples");
            this.Mean = mean ?? throw new ArgumentNullException("mean");
        }

        #endregion
    }
}
