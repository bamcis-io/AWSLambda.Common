using Amazon.Lambda.Core;
using System.Threading.Tasks;

namespace BAMCIS.Lambda.Common.CustomResources
{
    /// <summary>
    /// The set of methods every custom resource handler must implement
    /// </summary>
    public interface ICustomResourceHandler
    {
        /// <summary>
        /// Executes the action specified in the request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<CustomResourceResult> ExecuteAsync(CustomResourceRequest request, ILambdaContext context);

        /// <summary>
        /// Creates the custom resource specified in the request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<CustomResourceResponse> CreateAsync(CustomResourceRequest request, ILambdaContext context);

        /// <summary>
        /// Updates the custom resource specified in the request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<CustomResourceResponse> UpdateAsync(CustomResourceRequest request, ILambdaContext context);

        /// <summary>
        /// Deletes the custom resource specified in the request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<CustomResourceResponse> DeleteAsync(CustomResourceRequest request, ILambdaContext context);
    }
}
