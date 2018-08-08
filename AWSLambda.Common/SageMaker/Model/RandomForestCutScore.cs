using Newtonsoft.Json;

namespace BAMCIS.AWSLambda.Common.SageMaker.Model
{
    /// <summary>
    /// The score from a Random Forest Cut algorithm
    /// </summary>
    public class RandomCutForestScore
    {
        #region Public Properties

        /// <summary>
        /// The RCF score
        /// </summary>
        [JsonProperty(PropertyName = "score")]
        public double Score { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new RCF score
        /// </summary>
        /// <param name="score"></param>
        [JsonConstructor()]
        public RandomCutForestScore(double score)
        {
            this.Score = score;
        }

        #endregion
    }
}
