using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BAMCIS.Lambda.Common.SageMaker.Model
{
    /// <summary>
    /// The response from an object detection algorithm
    /// </summary>
    public class ObjectDetectionInferenceResponse
    {
        #region Public Properties

        /// <summary>
        /// A set of arrays containing 6 numbers each:
        /// [predicted_class_label, score, xmin, ymin, xmax, ymax]
        /// </summary>
        [JsonProperty(PropertyName = "prediction")]
        public IEnumerable<double[]> Prediction { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new object detection inference response
        /// </summary>
        /// <param name="prediction"></param>
        [JsonConstructor()]
        public ObjectDetectionInferenceResponse(IEnumerable<double[]> prediction)
        {
            this.Prediction = prediction ?? throw new ArgumentNullException("prediction");
        }

        #endregion
    }
}
