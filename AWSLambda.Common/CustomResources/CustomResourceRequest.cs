using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace BAMCIS.AWSLambda.Common.CustomResources
{
    /// <summary>
    /// These fields are sent in JSON requests from AWS CloudFormation to the custom resource provider
    /// </summary>
    public class CustomResourceRequest
    {
        #region Public Properties

        /// <summary>
        /// The request type is set by the AWS CloudFormation stack operation (create-stack, update-stack, or delete-stack) 
        /// that was initiated by the template developer for the stack that contains the custom resource.
        /// 
        /// Must be one of: Create, Update, or Delete.
        /// </summary>
        public StackOperation RequestType { get; }

        /// <summary>
        /// The response URL identifies a presigned S3 bucket that receives responses from the custom resource provider to AWS CloudFormation.
        /// </summary>
        public string ResponseUrl { get; }

        /// <summary>
        /// The Amazon Resource Name (ARN) that identifies the stack that contains the custom resource.
        /// 
        /// Combining the StackId with the RequestId forms a value that you can use to uniquely identify a request on a particular custom resource.
        /// </summary>
        public string StackId { get; }

        /// <summary>
        /// A unique ID for the request.
        /// 
        /// Combining the StackId with the RequestId forms a value that you can use to uniquely identify a request on a particular custom resource.
        /// </summary>
        public string RequestId { get; }

        /// <summary>
        /// The template developer-chosen resource type of the custom resource in the AWS CloudFormation template. 
        /// Custom resource type names can be up to 60 characters long and can include alphanumeric and the following characters: _@-.
        /// </summary>
        public string ResourceType { get; }

        /// <summary>
        /// The template developer-chosen name (logical ID) of the custom resource in the AWS CloudFormation template. 
        /// This is provided to facilitate communication between the custom resource provider and the template developer.
        /// </summary>
        public string LogicalResourceId { get; }

        /// <summary>
        /// A required custom resource provider-defined physical ID that is unique for that provider.
        /// 
        /// Always sent with Update and Delete requests; never sent with Create.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PhysicalResourceId { get; }

        /// <summary>
        /// This field contains the contents of the Properties object sent by the template developer. Its contents are defined by the custom resource provider.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, object> ResourceProperties { get; }

        /// <summary>
        /// Used only for Update requests. Contains the resource properties that were declared previous to the update request.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, object> OldResourceProperties { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for the custom resource request
        /// </summary>
        /// <param name="requestType">The request type</param>
        /// <param name="responseUrl">The response url</param>
        /// <param name="stackId">The stack id</param>
        /// <param name="requestId">The request id</param>
        /// <param name="resourceType">The resource type</param>
        /// <param name="logicalResourceId">The logical resource id</param>
        /// <param name="physicalResourceId">The physical resource id for update and delete requests</param>
        /// <param name="resourceProperties">The optional resource properties</param>
        /// <param name="oldResourceProperties">The old resource properties for an update operation</param>
        [JsonConstructor]
        public CustomResourceRequest(
            StackOperation requestType,
            string responseUrl,
            string stackId,
            string requestId,
            string resourceType,
            string logicalResourceId,
            string physicalResourceId,
            IDictionary<string, object> resourceProperties,
            IDictionary<string, object> oldResourceProperties
        )
        {
            this.RequestType = requestType;
            this.ResponseUrl = NullEmptyCheck(responseUrl, "responseUrl");
            this.StackId = NullEmptyCheck(stackId, "stackId");
            this.RequestId = NullEmptyCheck(requestId, "requestId");
            this.ResourceType = NullEmptyCheck(resourceType, "resourceType");
            this.LogicalResourceId = NullEmptyCheck(logicalResourceId, "logicalResourceId");
            
            if (this.RequestType != StackOperation.CREATE)
            {
                this.PhysicalResourceId = NullEmptyCheck(physicalResourceId, "physicalResourceId");
            }

            this.ResourceProperties = resourceProperties;

            if (this.RequestType == StackOperation.UPDATE)
            {
                this.OldResourceProperties = oldResourceProperties ?? throw new ArgumentNullException("oldResourceProperties");
            }
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
        /// The stack operation, i.e. create-stack, update-stack, delete-stack.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum StackOperation
        {
            CREATE,
            UPDATE,
            DELETE
        }

        #endregion

    }
}
