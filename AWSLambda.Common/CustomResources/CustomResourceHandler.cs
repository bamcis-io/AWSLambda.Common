using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BAMCIS.AWSLambda.Common.CustomResources
{
    /// <summary>
    /// A base class for custom resource handlers. Provides the implementation for the ExecuteAsync
    /// method, which cannot be overriden
    /// </summary>
    public abstract class CustomResourceHandler : ICustomResourceHandler
    {
        #region Public Abstract Methods

        public abstract Task<CustomResourceResponse> CreateAsync(CustomResourceRequest request, ILambdaContext context);

        public abstract Task<CustomResourceResponse> UpdateAsync(CustomResourceRequest request, ILambdaContext context);

        public abstract Task<CustomResourceResponse> DeleteAsync(CustomResourceRequest request, ILambdaContext context);

        #endregion

        #region Public Methods

        /// <summary>
        /// Entrypoint for the Lambda function, calls the correct create, update, or delete function. While this method
        /// can be overridden, you will probably not need to. 
        /// </summary>
        /// <param name="request">The custom resource request</param>
        /// <param name="context">The ILambdaContext object</param>
        /// <returns>A custom resource result with the included request, response, http respone, and any exception thrown</returns>
        public virtual async Task<CustomResourceResult> ExecuteAsync(CustomResourceRequest request, ILambdaContext context)
        {
            CustomResourceResponse Response = null;

            switch (request.RequestType)
            {
                case CustomResourceRequest.StackOperation.CREATE:
                    {
                        Response = await this.CreateAsync(request, context);
                        break;
                    }
                case CustomResourceRequest.StackOperation.DELETE:
                    {
                        Response = await this.DeleteAsync(request, context);
                        break;
                    }
                case CustomResourceRequest.StackOperation.UPDATE:
                    {
                        Response = await this.UpdateAsync(request, context);
                        break;
                    }
                default:
                    {
                        throw new ArgumentException($"Unknown stack operation: {request.RequestType}.");
                    }
            }

            CustomResourceResult Result = await CustomResourceHelper.UploadResponseAsync(Response, request);

            if (!Result.IsSuccess)
            {
                context.LogError(JsonConvert.SerializeObject(Result.Response));

                if (Result.S3Response != null)
                {
                    context.LogError(JsonConvert.SerializeObject(Result.S3Response));
                }

                if (Result.Exception != null)
                {
                    context.LogError(Result.Exception);
                }
            }

            return Result;
        }

        #endregion        
    }
}
