using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BAMCIS.Lambda.Common.Events.SNS
{
    /// <summary>
    /// Represents information about the S3 bucket that contains
    /// the object the caused the event
    /// </summary>
    public class S3BucketInfo
    {
        #region Public Properties

        /// <summary>
        /// The name of the bucket
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Should contain 1 property, "principalId" whose value
        /// is the Amazon customer Id of the bucket owner
        /// </summary>
        public IDictionary<string, string> OwnerIdentity { get; }

        /// <summary>
        /// The bucket's Arn
        /// </summary>
        public string Arn { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor for the S3 Bucket info
        /// </summary>
        /// <param name="name">The bucket name</param>
        /// <param name="ownerIdentity">The owner identity data</param>
        /// <param name="arn">The bucket Arn</param>
        [JsonConstructor]
        public S3BucketInfo(
            string name,
            IDictionary<string, string> ownerIdentity,
            string arn
        )
        {
            this.Name = ParameterTests.NotNullOrEmpty(name, "name");
            this.OwnerIdentity = ownerIdentity ?? throw new ArgumentNullException("ownerIdentity");
            this.Arn = ParameterTests.NotNullOrEmpty(arn, "arn");
        }

        #endregion
    }
}
