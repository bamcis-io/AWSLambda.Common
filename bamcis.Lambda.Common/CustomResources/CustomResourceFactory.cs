﻿using Amazon.Lambda.Core;
using System;
using System.Threading.Tasks;

namespace BAMCIS.Lambda.Common.CustomResources
{
    /// <summary>
    /// A basic implementation of the CustomResourceHandler. Simply provide the 
    /// create, update, and delete methods upon creation, and then call ExecuteAsync
    /// passing through the custom resource request and the Lambda context. 
    /// </summary>
    public class CustomResourceFactory : CustomResourceHandler, ICustomResourceHandler
    {
        #region Public Properties

        /// <summary>
        /// The function that executes the resource creation logic
        /// </summary>
        private Func<CustomResourceRequest, ILambdaContext, Task<CustomResourceResponse>> _CreateAsync { get; }

        /// <summary>
        /// The function that executes the resource update logic
        /// </summary>
        private Func<CustomResourceRequest, ILambdaContext, Task<CustomResourceResponse>> _UpdateAsync { get; }

        /// <summary>
        /// The function that executes th resource deletion logic
        /// </summary>
        private Func<CustomResourceRequest, ILambdaContext, Task<CustomResourceResponse>> _DeleteAsync { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new custom resource handler with the provided create, update, and delete functions.
        /// The default CustomResourceHelper is used.
        /// </summary>
        /// <param name="create"></param>
        /// <param name="update"></param>
        /// <param name="delete"></param>
        public CustomResourceFactory(
            Func<CustomResourceRequest, ILambdaContext, Task<CustomResourceResponse>> create,
            Func<CustomResourceRequest, ILambdaContext, Task<CustomResourceResponse>> update,
            Func<CustomResourceRequest, ILambdaContext, Task<CustomResourceResponse>> delete
        ) : this(create, update, delete, new DefaultCustomResourceHelper())
        {           
        }

        /// <summary>
        /// Creates a new custom resource handler with the provided create, update, and delete functions.
        /// The specified ICustomResourceHelper is used during execution.
        /// </summary>
        /// <param name="create"></param>
        /// <param name="update"></param>
        /// <param name="delete"></param>
        /// <param name="helper">The custom resource helper that will fix up the json that is being uploaded
        /// to S3 to signal completion, as well as perform that upload to the presigned url.</param>
        public CustomResourceFactory(
            Func<CustomResourceRequest, ILambdaContext, Task<CustomResourceResponse>> create,
            Func<CustomResourceRequest, ILambdaContext, Task<CustomResourceResponse>> update,
            Func<CustomResourceRequest, ILambdaContext, Task<CustomResourceResponse>> delete,
            ICustomResourceHelper helper
        ) : base(helper)
        {
            this._CreateAsync = create ?? throw new ArgumentNullException("create");
            this._DeleteAsync = delete ?? throw new ArgumentNullException("delete");
            this._UpdateAsync = update ?? throw new ArgumentNullException("update");
        }

        #endregion

        #region Public Methods

        public override async Task<CustomResourceResponse> CreateAsync(CustomResourceRequest request, ILambdaContext context)
        {
            return await _CreateAsync(request, context);
        }

        public override async Task<CustomResourceResponse> UpdateAsync(CustomResourceRequest request, ILambdaContext context)
        {
            return await _UpdateAsync(request, context);
        }

        public override async Task<CustomResourceResponse> DeleteAsync(CustomResourceRequest request, ILambdaContext context)
        {
            return await _DeleteAsync(request, context);
        }

        #endregion
    }
}
