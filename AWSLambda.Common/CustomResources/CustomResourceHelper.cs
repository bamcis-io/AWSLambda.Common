using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static BAMCIS.AWSLambda.Common.CustomResources.CustomResourceRequest;

namespace BAMCIS.AWSLambda.Common.CustomResources
{
    /// <summary>
    /// Common code helper methods
    /// </summary>
    public class CustomResourceHelper
    {
        #region Private Fields

        /// <summary>
        /// The default encoding that is used to produce the http content as a byte
        /// array from string
        /// </summary>
        private static Encoding _DefaultEncoding = Encoding.UTF8;

        #endregion

        #region Public Methods

        /// <summary>
        /// Removes any unnecessary fields from the json response depending on the stack operation
        /// and outcome of the custom resource action
        /// </summary>
        /// <param name="response">The response object to be modified</param>
        /// <param name="operation">The CloudFormation stack operation</param>
        /// <returns></returns>
        public static string FixUpResponseJson(CustomResourceResponse response, StackOperation operation)
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

        #endregion

        #region Internal Methods

        /// <summary>
        /// Uploads the custom resource action response to the pre-signed s3 url
        /// </summary>
        /// <param name="response">The response generated from performing the action</param>
        /// <param name="request">The request to initiate the custom resource action</param>
        /// <returns></returns>
        internal static async Task<CustomResourceResult> UploadResponseAsync(CustomResourceResponse response, CustomResourceRequest request)
        {
            try
            {
                // Fix up the JSON content by removing fields that are not accepted
                // for particular actions
                string Content = CustomResourceHelper.FixUpResponseJson(response, request.RequestType);

                HttpResponseMessage FinalResponse = await CustomResourceHelper.UploadResponseAsync(request.ResponseUrl, Content);

                if ((int)FinalResponse.StatusCode < 200 || (int)FinalResponse.StatusCode > 299)
                {
                    string Message = $"Failed to submit response successfully: {(int)FinalResponse.StatusCode}\n{await FinalResponse.Content.ReadAsStringAsync()}";

                    return new CustomResourceResult(request, response, FinalResponse, new HttpRequestException(Message));
                }
                else
                {
                    return new CustomResourceResult(request, response, FinalResponse);
                }
            }
            catch (HttpRequestException e)
            {
                return new CustomResourceResult(request, response, e);
            }
            catch (Exception e)
            {
                return new CustomResourceResult(request, response, e);
            }
        }

        /// <summary>
        /// Uploads the custom resource response to the pre-signed s3 url
        /// </summary>
        /// <param name="url">The pre-signed s3 url</param>
        /// <param name="content">The json serialized response</param>
        /// <returns></returns>
        internal static async Task<HttpResponseMessage> UploadResponseAsync(Uri url, string content, Encoding encoding)
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
            HttpClient Client = new HttpClient();
            HttpRequestMessage Request = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = new ByteArrayContent(encoding.GetBytes(content))              
            };
           
            return await Client.SendAsync(Request);
        }

        /// <summary>
        /// Uploads the custom resource response to the pre-signed s3 url
        /// </summary>
        /// <param name="url">The pre-signed s3 url</param>
        /// <param name="content">The json serialized response</param>
        /// <returns></returns>
        internal static async Task<HttpResponseMessage> UploadResponseAsync(Uri url, string content)
        {
            return await UploadResponseAsync(url, content, _DefaultEncoding);
        }

        /// <summary>
        /// Uploads the custom resource response to the pre-signed s3 url
        /// </summary>
        /// <param name="url">The pre-signed s3 url</param>
        /// <param name="content">The json serialized response</param>
        /// <returns></returns>
        internal static async Task<HttpResponseMessage> UploadResponseAsync(string url, string content)
        {
            return await UploadResponseAsync(new Uri(url), content, _DefaultEncoding);
        }

        /// <summary>
        /// Uploads the custom resource response to the pre-signed s3 url
        /// </summary>
        /// <param name="url">The pre-signed s3 url</param>
        /// <param name="content">The json serialized response</param>
        /// <param name="encoding">The encoding to use to convert the string content to byte array</param>
        /// <returns></returns>
        internal static async Task<HttpResponseMessage> UploadResponseAsync(string url, string content, Encoding encoding)
        {
            return await UploadResponseAsync(new Uri(url), content, encoding);
        }

        #endregion
    }
}
