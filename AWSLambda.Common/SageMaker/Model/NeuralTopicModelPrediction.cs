using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BAMCIS.AWSLambda.Common.SageMaker.Model
{
    /// <summary>
    /// Represents a Neural Topic Model predictions
    /// </summary>
    public class NeuralTopicModelPrediction : Prediction
    {
        #region Public Properties

        /// <summary>
        /// The topic weights
        /// </summary>
        [JsonProperty(PropertyName = "topic_weights")]
        public IEnumerable<double> TopicWeights { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Neural Topic Model prediction
        /// </summary>
        /// <param name="topicWeights"></param>
        [JsonConstructor()]
        public NeuralTopicModelPrediction(IEnumerable<double> topicWeights)
        {
            this.TopicWeights = topicWeights ?? throw new ArgumentNullException("topicWeights");
        }

        #endregion
    }
}
