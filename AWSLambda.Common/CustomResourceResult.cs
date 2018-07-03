using BAMCIS.AWSLambda.Common.Events;
using System;
using System.Net.Http;

namespace BAMCIS.AWSLambda.Common
{
    /// <summary>
    /// Contains the relevant request, response, and http responses objects from a custom
    /// resource create, update, or delete request
    /// </summary>
    public class CustomResourceResult
    {
        #region Public Properties

        /// <summary>
        /// The request for the custom resource create, update, or delete
        /// </summary>
        public CustomResourceRequest Request { get; }

        /// <summary>
        /// The response to the create, update, or delete request that will be
        /// sent to the S3 pre-signed url
        /// </summary>
        public CustomResourceResponse Response { get; }

        /// <summary>
        /// The response from S3 after attempting to post the custom resource response
        /// </summary>
        public HttpResponseMessage S3Response { get; }

        /// <summary>
        /// Indicates if the entire set of operations was successful
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Any exception thrown when submitting the response to S3 or in processing the request
        /// </summary>
        public Exception Exception { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new custom resource result with the success being determined by the custom
        /// resource response status and http status code from S3.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="s3response"></param>
        public CustomResourceResult(
            CustomResourceRequest request, 
            CustomResourceResponse response, 
            HttpResponseMessage s3response
        )
        {
            this.Request = request ?? throw new ArgumentNullException("request");
            this.Response = response ?? throw new ArgumentNullException("response");
            this.S3Response = s3response ?? throw new ArgumentNullException("s3response");
            this.IsSuccess = (response.Status == CustomResourceResponse.RequestStatus.SUCCESS && s3response.IsSuccessStatusCode);
            this.Exception = null;
        }

        /// <summary>
        /// Creates a new custom resource result that assumes the request to S3 failed
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="exception"></param>
        public CustomResourceResult(
            CustomResourceRequest request,
            CustomResourceResponse response,
            Exception exception
        )
        {
            this.Request = request ?? throw new ArgumentNullException("request");
            this.Response = response ?? throw new ArgumentNullException("response");
            this.S3Response = null;
            this.IsSuccess = false;
            this.Exception = exception ?? throw new ArgumentNullException("exception");
        }

        /// <summary>
        /// Creates a new custom resource result that assumes the request to S3 failed
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="exception"></param>
        public CustomResourceResult(
            CustomResourceRequest request,
            CustomResourceResponse response,
            HttpResponseMessage s3response,
            Exception exception
        ) : this(request, response, s3response)
        {           
            this.IsSuccess = false;
            this.Exception = exception ?? throw new ArgumentNullException("exception");
        }


        #endregion
    }
}
