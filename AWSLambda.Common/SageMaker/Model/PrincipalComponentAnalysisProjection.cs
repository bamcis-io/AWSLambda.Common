using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BAMCIS.AWSLambda.Common.SageMaker.Model
{
    /// <summary>
    /// A project for a PCA transform
    /// </summary>
    public class PrincipalComponentAnalysisProjection
    {
        #region Public Properties

        /// <summary>
        /// The projection values
        /// </summary>
        [JsonProperty(PropertyName = "projection")]
        public IEnumerable<double> Projection { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new PCA projection
        /// </summary>
        /// <param name="projection"></param>
        [JsonConstructor()]
        public PrincipalComponentAnalysisProjection(IEnumerable<double> projection)
        {
            this.Projection = projection ?? throw new ArgumentNullException("projection");
        }

        #endregion
    }
}
