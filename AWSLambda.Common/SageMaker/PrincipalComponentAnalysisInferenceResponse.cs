using BAMCIS.AWSLambda.Common.SageMaker.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BAMCIS.AWSLambda.Common.SageMaker
{
    /// <summary>
    /// A response from a PCA inference transform
    /// </summary>
    public class PrincipalComponentAnalysisInferenceResponse
    {
        #region Public Properties

        /// <summary>
        /// The projections created by the PCA transform
        /// </summary>
        [JsonProperty(PropertyName = "projections")]
        public IEnumerable<PrincipalComponentAnalysisProjection> Projections { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new PCA inference response
        /// </summary>
        /// <param name="projections"></param>
        public PrincipalComponentAnalysisInferenceResponse(IEnumerable<PrincipalComponentAnalysisProjection> projections)
        {
            this.Projections = projections ?? throw new ArgumentNullException("projections");
        }

        #endregion
    }
}
