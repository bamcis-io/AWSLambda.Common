using Amazon.Lambda.Core;
using BAMCIS.AWSLambda.Common.Events;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BAMCIS.AWSLambda.Common
{
    /// <summary>
    /// Implements creating, updating, and deleting custom resources through a Lambda function in conjunction with
    /// CloudFormation. Simply provide the create, update, and delete methods upon creation, and then call Execute
    /// passing through the custom resource request and the Lambda context. 
    /// </summary>
    public class CustomResourceHandler
    {
        #region Public Properties

        /// <summary>
        /// The function that executes the resource creation logic
        /// </summary>
        public Func<CustomResourceRequest, ILambdaContext, Task<CustomResourceResponse>> Create { get; }

        /// <summary>
        /// The function that executes the resource update logic
        /// </summary>
        public Func<CustomResourceRequest, ILambdaContext, Task<CustomResourceResponse>> Update { get; }

        /// <summary>
        /// The function that executes th resource deletion logic
        /// </summary>
        public Func<CustomResourceRequest, ILambdaContext, Task<CustomResourceResponse>> Delete { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new custom resource handler with the provided create, update, and delete functions
        /// </summary>
        /// <param name="create"></param>
        /// <param name="update"></param>
        /// <param name="delete"></param>
        public CustomResourceHandler(
            Func<CustomResourceRequest, ILambdaContext, Task<CustomResourceResponse>> create,
            Func<CustomResourceRequest, ILambdaContext, Task<CustomResourceResponse>> update,
            Func<CustomResourceRequest, ILambdaContext, Task<CustomResourceResponse>> delete
        )
        {
            this.Create = create ?? throw new ArgumentNullException("create");
            this.Delete = delete ?? throw new ArgumentNullException("delete");
            this.Update = update ?? throw new ArgumentNullException("update");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Entrypoint for the Lambda function, calls the correct create, update, or delete function
        /// </summary>
        /// <param name="request">The custom resource request</param>
        /// <param name="context">The ILambdaContext object</param>
        /// <param name="throwInsteadOfLog">Indicates if the function should throw exceptions instead of logging errors to CloudWatch</param>
        /// <returns>A custom resource result with the included request, response, http respone, and any exception thrown</returns>
        public async Task<CustomResourceResult> Execute(CustomResourceRequest request, ILambdaContext context)
        {
            CustomResourceResponse Response = null;

            switch (request.RequestType)
            {
                case CustomResourceRequest.StackOperation.CREATE:
                    {
                        Response = await this.Create(request, context);
                        break;
                    }
                case CustomResourceRequest.StackOperation.DELETE:
                    {
                        Response = await this.Delete(request, context);
                        break;
                    }
                case CustomResourceRequest.StackOperation.UPDATE:
                    {
                        Response = await this.Update(request, context);
                        break;
                    }
                default:
                    {
                        throw new ArgumentException($"Unknown stack operation: {request.RequestType}.");
                    }
            }

            try
            {
                // Fix up the JSON content by removing fields that are not accepted
                // for particular actions
                JObject Obj = JObject.FromObject(Response);

                switch (request.RequestType)
                {
                    case CustomResourceRequest.StackOperation.CREATE:
                    case CustomResourceRequest.StackOperation.UPDATE:
                        {
                            if (Response.Status == CustomResourceResponse.RequestStatus.FAILED)
                            {
                                Obj.Remove("NoEcho");
                                Obj.Remove("Data");
                            }

                            break;
                        }
                    case CustomResourceRequest.StackOperation.DELETE:
                        {
                            Obj.Remove("NoEcho");
                            Obj.Remove("Data");

                            if (Response.Status == CustomResourceResponse.RequestStatus.SUCCESS)
                            {
                                Obj.Remove("Reason");
                            }

                            break;
                        }
                }

                string Content = Obj.ToString();

                context.LogInfo($"Attempting to send response to pre-signed s3 url: {Content}");

                HttpResponseMessage FinalResponse = await UploadResponse(request.ResponseUrl, Content);

                if ((int)FinalResponse.StatusCode < 200 || (int)FinalResponse.StatusCode > 299)
                {
                    string Message = $"Failed to submit response successfully: {(int)FinalResponse.StatusCode}\n{await FinalResponse.Content.ReadAsStringAsync()}";

                    return new CustomResourceResult(request, Response, FinalResponse, new HttpRequestException(Message));
                }
                else
                {
                    context.LogInfo($"Successfully submitted response: {(int)FinalResponse.StatusCode}");
                    return new CustomResourceResult(request, Response, FinalResponse);
                }
            }
            catch (HttpRequestException e)
            {
                context.LogError(e);
                return new CustomResourceResult(request, Response, e);
            }
            catch (Exception e)
            {
                context.LogError(e);
                return new CustomResourceResult(request, Response, e);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Uploads the custom resource response to the pre-signed s3 url
        /// </summary>
        /// <param name="url">The pre-signed s3 url</param>
        /// <param name="content">The json serialized response</param>
        /// <returns></returns>
        private static async Task<HttpResponseMessage> UploadResponse(Uri url, string content)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            if (String.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException("content");
            }

            // Must use ByteArrayContent or StreamContent instead of StringContent because
            // it sets the Content-Type header and causes a 403 error when submitting to the
            // pre-signed s3 url
            HttpClient Client = new HttpClient();
            HttpRequestMessage Request = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = new ByteArrayContent(Encoding.UTF8.GetBytes(content))
            };

            return await Client.SendAsync(Request);
        }

        /// <summary>
        /// Uploads the custom resource response to the pre-signed s3 url
        /// </summary>
        /// <param name="url">The pre-signed s3 url</param>
        /// <param name="content">The json serialized response</param>
        /// <returns></returns>
        private static async Task<HttpResponseMessage> UploadResponse(string url, string content)
        {
            return await UploadResponse(new Uri(url), content);
        }

        #endregion
    }
}
