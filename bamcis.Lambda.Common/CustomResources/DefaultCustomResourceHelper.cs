using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static BAMCIS.Lambda.Common.CustomResources.CustomResourceRequest;

namespace BAMCIS.Lambda.Common.CustomResources
{
    /// <summary>
    /// Common code helper methods
    /// </summary>
    public class DefaultCustomResourceHelper : ICustomResourceHelper
    {
        #region Private Fields

        /// <summary>
        /// The default encoding that is used to produce the http content as a byte
        /// array from string
        /// </summary>
        private static Encoding _DefaultEncoding = Encoding.UTF8;

        private static HttpClientHandler handler;
        private static HttpClient client;

        #endregion

        #region Constructors

        static DefaultCustomResourceHelper()
        {
            handler = new HttpClientHandler();
            client = new HttpClient(handler);
        }

        public DefaultCustomResourceHelper()
        {         
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Removes any unnecessary fields from the json response depending on the stack operation
        /// and outcome of the custom resource action
        /// </summary>
        /// <param name="response">The response object to be modified</param>
        /// <param name="operation">The CloudFormation stack operation</param>
        /// <returns>The fixed up JSON string representing the response that needs to be uploaded to S3</returns>
        public string FixUpResponseJson(CustomResourceResponse response, StackOperation operation)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            // Fix up the JSON content by removing fields that are not accepted
            // for particular actions
            JObject Obj = JObject.FromObject(response);

            switch (operation)
            {
                case CustomResourceRequest.StackOperation.CREATE:
                case CustomResourceRequest.StackOperation.UPDATE:
                    {
                        if (response.Status == CustomResourceResponse.RequestStatus.FAILED)
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

                        if (response.Status == CustomResourceResponse.RequestStatus.SUCCESS)
                        {
                            Obj.Remove("Reason");
                        }

                        break;
                    }
            }

            return Obj.ToString();
        }

        /// <summary>
        /// Uploads the custom resource action response to the pre-signed s3 url. Use this method if
        /// you want to use the default handling for generating the CustomResourceResult object. This
        /// is the method used by the ExecuteAsync method in the base CustomResourceHandler abstract class.
        /// </summary>
        /// <param name="response">The response generated from performing the action</param>
        /// <param name="request">The request to initiate the custom resource action</param>
        /// <returns>The default method for generating the CustomResourceResult is used in this method</returns>
        public async Task<CustomResourceResult> PutCustomResourceResponseAsync(CustomResourceRequest request, CustomResourceResponse response)
        {
            try
            {
                HttpResponseMessage responseMessage = await this.UploadResponseToS3Async(request, response);

                if ((int)responseMessage.StatusCode < 200 || (int)responseMessage.StatusCode > 299)
                {
                    string message = $"Failed to submit response successfully: {(int)responseMessage.StatusCode}\n{await responseMessage.Content.ReadAsStringAsync()}";

                    return new CustomResourceResult(request, response, responseMessage, new HttpRequestException(message));
                }
                else
                {
                    return new CustomResourceResult(request, response, responseMessage);
                }
            }
            catch (Exception e)
            {
                return new CustomResourceResult(request, response, e);
            }
        }

        /// <summary>
        /// Uploads the custom resource action response to the pre-signed s3 url. Use this method if
        /// you want to handle generating the CustomResourceResult object yourself.
        /// </summary>
        /// <param name="response">The response generated from performing the action</param>
        /// <param name="request">The request to initiate the custom resource action</param>
        /// <returns>The HttpResponseMessage from S3 which can be used with the request and response
        /// objects to generate the required CustomResourceResult.</returns>
        public async Task<HttpResponseMessage> UploadResponseToS3Async(CustomResourceRequest request, CustomResourceResponse response)
        {
            return await StaticUploadResponseToS3Async(request.ResponseUrl, FixUpResponseJson(response, request.RequestType));
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Uploads the custom resource response to the pre-signed s3 url
        /// </summary>
        /// <param name="url">The pre-signed s3 url</param>
        /// <param name="content">The json serialized response</param>
        /// <returns></returns>
        internal static async Task<HttpResponseMessage> StaticUploadResponseToS3Async(Uri url, string content, Encoding encoding)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            if (String.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException("content");
            }

            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }

            // Must use ByteArrayContent or StreamContent instead of StringContent because
            // it sets the Content-Type header and causes a 403 error when submitting to the
            // pre-signed s3 url

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = new ByteArrayContent(encoding.GetBytes(content))
            })
            {
                return await client.SendAsync(request);
            }
        }

        /// <summary>
        /// Uploads the custom resource response to the pre-signed s3 url
        /// </summary>
        /// <param name="url">The pre-signed s3 url</param>
        /// <param name="content">The json serialized response</param>
        /// <returns></returns>
        internal static async Task<HttpResponseMessage> StaticUploadResponseToS3Async(Uri url, string content)
        {
            return await StaticUploadResponseToS3Async(url, content, _DefaultEncoding);
        }

        /// <summary>
        /// Uploads the custom resource response to the pre-signed s3 url
        /// </summary>
        /// <param name="url">The pre-signed s3 url</param>
        /// <param name="content">The json serialized response</param>
        /// <returns></returns>
        internal static async Task<HttpResponseMessage> StaticUploadResponseToS3Async(string url, string content)
        {
            return await StaticUploadResponseToS3Async(new Uri(url), content, _DefaultEncoding);
        }

        /// <summary>
        /// Uploads the custom resource response to the pre-signed s3 url
        /// </summary>
        /// <param name="url">The pre-signed s3 url</param>
        /// <param name="content">The json serialized response</param>
        /// <param name="encoding">The encoding to use to convert the string content to byte array</param>
        /// <returns></returns>
        internal static async Task<HttpResponseMessage> StaticUploadResponseToS3Async(string url, string content, Encoding encoding)
        {
            return await StaticUploadResponseToS3Async(new Uri(url), content, encoding);
        }

        #endregion
    }
}
