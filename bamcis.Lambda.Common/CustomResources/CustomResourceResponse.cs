using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace BAMCIS.Lambda.Common.CustomResources
{
    /// <summary>
    /// The following are properties that the custom resource provider includes when it sends the JSON file to the presigned URL.
    /// </summary>
    public class CustomResourceResponse
    {
        #region Public Properties

        /// <summary>
        /// The status value sent by the custom resource provider in response to an AWS CloudFormation-generated request.
        /// 
        /// Must be either SUCCESS or FAILED.
        /// </summary>
        public RequestStatus Status { get; }

        /// <summary>
        /// Describes the reason for a failure response.
        /// 
        /// Required if Status is FAILED. It's optional otherwise.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Reason { get; }

        /// <summary>
        /// This value should be an identifier unique to the custom resource vendor, and can be up to 1 Kb in size. 
        /// The value must be a non-empty string and must be identical for all responses for the same resource.
        /// </summary>
        public string PhysicalResourceId { get; }

        /// <summary>
        /// The Amazon Resource Name (ARN) that identifies the stack that contains the custom resource. 
        /// This response value should be copied verbatim from the request.
        /// </summary>
        public string StackId { get; }

        /// <summary>
        /// A unique ID for the request. This response value should be copied verbatim from the request.
        /// </summary>
        public string RequestId { get; }

        /// <summary>
        /// The template developer-chosen name (logical ID) of the custom resource in the AWS CloudFormation template. 
        /// This response value should be copied verbatim from the request.
        /// </summary>
        public string LogicalResourceId { get; }

        /// <summary>
        /// Optional. Indicates whether to mask the output of the custom resource when retrieved by using the Fn::GetAtt function. 
        /// If set to true, all returned values are masked with asterisks (*****). The default value is false.
        /// </summary>
        public bool NoEcho { get; }

        /// <summary>
        /// Optional. The custom resource provider-defined name-value pairs to send with the response. 
        /// You can access the values provided here by name in the template with Fn::GetAtt.
        /// 
        /// If the name-value pairs contain sensitive information, you should use the NoEcho field to mask the output of the custom resource. 
        /// Otherwise, the values are visible through APIs that surface property values (such as DescribeStackEvents).
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, object> Data { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor for creating a custom resource response
        /// </summary>
        /// <param name="status"></param>
        /// <param name="reason"></param>
        /// <param name="physicalResourceId"></param>
        /// <param name="stackId"></param>
        /// <param name="requestId"></param>
        /// <param name="logicalResourceId"></param>
        /// <param name="noEcho"></param>
        /// <param name="data"></param>
        [JsonConstructor]
        public CustomResourceResponse(
            RequestStatus status,
            string reason,
            string physicalResourceId,
            string stackId,
            string requestId,
            string logicalResourceId,
            bool noEcho = false,
            IDictionary<string, object> data = null
        )
        {
            this.Status = status;
            
            if (this.Status == RequestStatus.FAILED)
            {
                this.Reason = NullEmptyCheck(reason, "reason");
            }
            else
            {
                this.Reason = reason;
            }

            this.PhysicalResourceId = NullEmptyCheck(physicalResourceId, "physicalResourceId");
            this.StackId = NullEmptyCheck(stackId, "stackId");
            this.RequestId = NullEmptyCheck(requestId, "requestId");
            this.LogicalResourceId = NullEmptyCheck(logicalResourceId, "logicalResourceId");
            this.NoEcho = noEcho;
            this.Data = data;
        }

        /// <summary>
        /// Creates a response using a custom resource request
        /// </summary>
        /// <param name="status"></param>
        /// <param name="reason"></param>
        /// <param name="request"></param>
        /// <param name="noEcho"></param>
        /// <param name="data"></param>
        public CustomResourceResponse(
            RequestStatus status,
            string reason,
            CustomResourceRequest request,
            bool noEcho = false,
            IDictionary<string, object> data = null
            )
        {
            this.Status = status;

            if (this.Status == RequestStatus.FAILED)
            {
                this.Reason = NullEmptyCheck(reason, "reason");
            }
            else
            {
                this.Reason = reason;
            }

            this.PhysicalResourceId = request.PhysicalResourceId;
            this.StackId = request.StackId;
            this.RequestId = request.RequestId;
            this.LogicalResourceId = request.LogicalResourceId;
            this.NoEcho = noEcho;
            this.Data = data;
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Checks if the value is null or empty and throws an ArgumentNullException if it is
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <param name="name">The parameter name being checked</param>
        /// <returns>The value of the string if it's not null or empty</returns>
        private string NullEmptyCheck(string value, string name)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(name);
            }
            else
            {
                return value;
            }
        }

        #endregion

        #region Internal Classes

        /// <summary>
        /// The status value sent by the custom resource provider in response to an AWS CloudFormation-generated request.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum RequestStatus
        {
            SUCCESS,
            FAILED
        }
        #endregion
    }
}
