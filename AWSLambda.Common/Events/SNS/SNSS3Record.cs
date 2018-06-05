using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BAMCIS.AWSLambda.Common.Events.SNS
{
    /// <summary>
    /// This class represents an individual JSON message
    /// record that is sent from S3 to SNS. It is used to
    /// help in deserializing the JSON records from the SNS
    /// message body.
    /// 
    /// {  
    ///     "eventVersion":"2.0",
    ///     "eventSource":"aws:s3",
    ///     "awsRegion":"us-east-1",
    ///     "eventTime":The time, in ISO-8601 format, for example, 1970-01-01T00:00:00.000Z, when S3 finished processing the request,
    ///     "eventName":"event-type",
    ///     "userIdentity":{  
    ///        "principalId":"Amazon-customer-ID-of-the-user-who-caused-the-event"
    ///     },
    ///     "requestParameters":{  
    ///        "sourceIPAddress":"ip-address-where-request-came-from"
    ///     },
    ///     "responseElements":{  
    ///        "x-amz-request-id":"Amazon S3 generated request ID",
    ///        "x-amz-id-2":"Amazon S3 host that processed the request"
    ///     },
    ///     "s3":{  
    ///        "s3SchemaVersion":"1.0",
    ///        "configurationId":"ID found in the bucket notification configuration",
    ///        "bucket":{  
    ///           "name":"bucket-name",
    ///           "ownerIdentity":{  
    ///              "principalId":"Amazon-customer-ID-of-the-bucket-owner"
    ///           },
    ///           "arn":"bucket-ARN"
    ///        },
    ///        "object":{  
    ///           "key":"object-key",
    ///           "size":object-size,
    ///           "eTag":"object eTag",
    ///           "versionId":"object version if bucket is versioning-enabled, otherwise null",
    ///           "sequencer": "a string representation of a hexadecimal value used to determine event sequence, 
    ///               only used with PUTs and DELETEs"            
    ///        }
    ///     }
    ///  }
    /// </summary>
    public class SNSS3Record
    {
        #region Public Properties

        /// <summary>
        /// The event version
        /// </summary>
        public string EventVersion { get; }

        /// <summary>
        /// The service that produced the event, this should be "aws:s3"
        /// </summary>
        public string EventSource { get; }

        /// <summary>
        /// The AWS region the event occured in
        /// </summary>
        public string AwsRegion { get; }

        /// <summary>
        /// The time of the event
        /// </summary>
        public DateTime EventTime { get; }

        /// <summary>
        /// The S3 event that triggered the notification like "ObjectCreated:Put"
        /// </summary>
        public string EventName { get; }

        /// <summary>
        /// Should only contain 1 key, 'principalId' whose value is the 
        /// principal Id of the user who performed the action on the S3 object
        /// that caused the event
        /// </summary>
        public IDictionary<string, string> UserIdentity { get; }

        /// <summary>
        /// Should only contain 1 key 'sourceIPAddress`
        /// </summary>
        public IDictionary<string, string> RequestParameters { get; }

        /// <summary>
        /// The responseElements key value is useful if you want to trace the request by following up with Amazon S3 
        /// support. Both x-amz-request-id and x-amz-id-2 help Amazon S3 to trace the individual request. These values 
        /// are the same as those that Amazon S3 returned in the response to your original PUT request, 
        /// which initiated the event. 
        /// </summary>
        public IDictionary<string, string> ResponseElements { get; }

        /// <summary>
        /// Information about the S3 object and bucket that caused the event
        /// </summary>
        public S3Info S3 { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor for creating the record
        /// </summary>
        /// <param name="eventVersion">The event version</param>
        /// <param name="eventSource">The event source</param>
        /// <param name="awsRegion">The AWS region</param>
        /// <param name="eventTime">The event time</param>
        /// <param name="eventName">The event name</param>
        /// <param name="userIdentity">The user identity object</param>
        /// <param name="requestParameters">The request parameters</param>
        /// <param name="responseElements">The response elements</param>
        /// <param name="s3">The S3 object and bucket info</param>
        [JsonConstructor]
        public SNSS3Record(
            string eventVersion,
            string eventSource,
            string awsRegion,
            DateTime eventTime,
            string eventName,
            IDictionary<string, string> userIdentity,
            IDictionary<string, string> requestParameters,
            IDictionary<string, string> responseElements,
            S3Info s3
            )
        {
            this.EventVersion = ParameterTests.NotNullOrEmpty(eventVersion, "eventVersion");
            this.EventSource = ParameterTests.NotNullOrEmpty(eventSource, "eventSource");
            this.AwsRegion = ParameterTests.NotNullOrEmpty(awsRegion, "awsRegion");
            this.EventTime = eventTime;
            this.EventName = ParameterTests.NotNullOrEmpty(eventName, "eventName");
            this.UserIdentity = userIdentity ?? throw new ArgumentNullException("userIdentity");
            this.RequestParameters = requestParameters ?? throw new ArgumentNullException("requestParameters");
            this.ResponseElements = responseElements ?? throw new ArgumentNullException("responseElements");
            this.S3 = s3 ?? throw new ArgumentNullException("s3");
        }

        #endregion
    }
}
