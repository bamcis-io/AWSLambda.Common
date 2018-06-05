using Newtonsoft.Json;
using System;

namespace BAMCIS.AWSLambda.Common.Events.SNS
{
    /// <summary>
    /// Represents the S3 information provided in the SNS notification
    /// message
    /// </summary>
    public class S3Info
    {
        #region Public Properties

        /// <summary>
        /// The schema version of the notification
        /// </summary>
        public string S3SchemaVersion { get; }

        /// <summary>
        /// ID found in the bucket notification configuration
        /// </summary>
        public string ConfigurationId { get; }

        /// <summary>
        /// Information about the S3 bucket containing the object that
        /// triggered the notification
        /// </summary>
        public S3BucketInfo Bucket { get; }

        /// <summary>
        /// Information about the S3 object that triggered the notification
        /// </summary>
        public S3ObjectInfo Object { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor for the S3 info
        /// </summary>
        /// <param name="s3SchemaVersion">The schema version</param>
        /// <param name="configurationId">The configuration id</param>
        /// <param name="bucket">The S3 bucket info</param>
        /// <param name="object">The S3 object info</param>
        [JsonConstructor]
        public S3Info(
            string s3SchemaVersion,
            string configurationId,
            S3BucketInfo bucket,
            S3ObjectInfo @object
        )
        {
            this.S3SchemaVersion = ParameterTests.NotNullOrEmpty(s3SchemaVersion, "s3SchemaVersion");
            this.ConfigurationId = ParameterTests.NotNullOrEmpty(configurationId, "configurationId");
            this.Bucket = bucket ?? throw new ArgumentNullException("bucket");
            this.Object = @object ?? throw new ArgumentNullException("object");
        }

        #endregion
    }
}
