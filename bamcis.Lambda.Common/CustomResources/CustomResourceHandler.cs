using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BAMCIS.Lambda.Common.CustomResources
{
    /// <summary>
    /// A base class for custom resource handlers. Provides the implementation for the ExecuteAsync
    /// method, which you probably do not need to override, but can.
    /// </summary>
    public abstract class CustomResourceHandler : ICustomResourceHandler
    {
        #region Private Fields

        private ICustomResourceHelper helper;

        #endregion

        #region Constructors

        public CustomResourceHandler()
        {
            this.helper = new DefaultCustomResourceHelper();
        }

        public CustomResourceHandler(ICustomResourceHelper helper)
        {
            this.helper = helper ?? throw new ArgumentNullException("helper");
        }

        #endregion

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
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            CustomResourceResponse response = null;

            switch (request.RequestType)
            {
                case CustomResourceRequest.StackOperation.CREATE:
                    {
                        response = await this.CreateAsync(request, context);
                        break;
                    }
                case CustomResourceRequest.StackOperation.DELETE:
                    {
                        response = await this.DeleteAsync(request, context);
                        break;
                    }
                case CustomResourceRequest.StackOperation.UPDATE:
                    {
                        response = await this.UpdateAsync(request, context);
                        break;
                    }
                default:
                    {
                        throw new ArgumentException($"Unknown stack operation: {request.RequestType}.");
                    }
            }

            CustomResourceResult result = await helper.PutCustomResourceResponseAsync(request, response);

            if (!result.IsSuccess)
            {
                context.LogError(JsonConvert.SerializeObject(result.Response));

                if (result.S3Response != null)
                {
                    context.LogError(JsonConvert.SerializeObject(result.S3Response));
                }

                if (result.Exception != null)
                {
                    context.LogError(result.Exception);
                }
            }

            return result;
        }

        #endregion        
    }
}
