using BAMCIS.Lambda.Common.SageMaker.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BAMCIS.Lambda.Common.SageMaker
{
    /// <summary>
    /// The response from an RCF algorithm
    /// </summary>
    public class RandomCutForestInferenceResponse
    {
        #region Public Properties

        /// <summary>
        /// The random cut scores
        /// </summary>
        [JsonProperty(PropertyName = "scores")]
        public IEnumerable<RandomCutForestScore> Scores { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates the RCF inference response
        /// </summary>
        public RandomCutForestInferenceResponse(IEnumerable<RandomCutForestScore> scores)
        {
            this.Scores = scores ?? throw new ArgumentNullException("scores");
        }

        #endregion
    }
}
