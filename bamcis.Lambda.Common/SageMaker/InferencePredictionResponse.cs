using BAMCIS.Lambda.Common.SageMaker.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BAMCIS.Lambda.Common.SageMaker
{
    /// <summary>
    /// The basic class for a response from a SageMaker endpoint that provides
    /// predictions like Linear Learner, Factorization Machine, and Neural Topic Model
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class InferencePredictionResponse<T> where T : Prediction
    {
        #region Public Properties

        /// <summary>
        /// The set of predictions
        /// </summary>
        [JsonProperty(PropertyName = "predictions")]
        public IEnumerable<T> Predictions { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new inference prediction response
        /// </summary>
        /// <param name="predictions"></param>
        [JsonConstructor()]
        public InferencePredictionResponse(IEnumerable<T> predictions)
        {
            this.Predictions = predictions ?? throw new ArgumentNullException("predictions");
        }

        #endregion
    }
}
