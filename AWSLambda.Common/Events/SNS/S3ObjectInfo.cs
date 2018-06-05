using Newtonsoft.Json;
using System;
using System.Net;

namespace BAMCIS.AWSLambda.Common.Events.SNS
{
    /// <summary>
    /// Represents information about the S3 object that caused the event
    /// </summary>
    public class S3ObjectInfo
    {
        #region Public Properties

        /// <summary>
        /// The object key
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// The size of the object in bytes
        /// </summary>
        public long Size { get; }

        /// <summary>
        /// The object's ETag
        /// </summary>
        public string ETag { get; }

        /// <summary>
        /// The object version if bucket is versioning-enabled, otherwise null
        /// </summary>
        public string VersionId { get; }

        /// <summary>
        /// A string representation of a hexadecimal value used to determine 
        /// event sequence, only used with PUTs and DELETEs.
        /// 
        /// The sequencer key provides a way to determine the sequence of events. 
        /// Event notifications are not guaranteed to arrive in the order that the events occurred. 
        /// However, notifications from events that create objects (PUTs) and delete objects 
        /// contain a sequencer, which can be used to determine the order of events for a given object key.
        ///
        /// If you compare the sequencer strings from two event notifications on the same object key, 
        /// the event notification with the greater sequencer hexadecimal value is the event that occurred later. 
        /// If you are using event notifications to maintain a separate database or index of your Amazon S3 objects, 
        /// you will probably want to compare and store the sequencer values as you process each event notification.
        /// 
        /// Note that:
        ///  - 'sequencer' cannot be used to determine order for events on different object keys. 
        ///  - The sequencers can be of different lengths. So to compare these values, you first 
        ///    right pad the shorter value with zeros and then do lexicographical comparison. 
        /// </summary>
        public string Sequencer { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor for the S3 object info
        /// </summary>
        /// <param name="key">The object key</param>
        /// <param name="size">The object size in bytes</param>
        /// <param name="eTag">The object's eTag</param>
        /// <param name="versionId">The object's version Id if versioning is enabled</param>
        /// <param name="sequencer">The object's sequence identifier</param>
        [JsonConstructor]
        public S3ObjectInfo(
            string key,
            long size,
            string eTag,
            string versionId = null,
            string sequencer = null
        )
        {
            // Remove URL encoding from key
            this.Key = WebUtility.UrlDecode(ParameterTests.NotNullOrEmpty(key, "key"));
            this.Size = size > 0 ? size : throw new ArgumentOutOfRangeException("The object size cannot be less than or equal to 0.");
            this.ETag = ParameterTests.NotNullOrEmpty(eTag, "eTag");
            this.VersionId = versionId; // Can be null
            this.Sequencer = sequencer; // Can be null
        }

        #endregion
    }
}
