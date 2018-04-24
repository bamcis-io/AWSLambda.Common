using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BAMCIS.AWSLambda.Common.Events
{
    /// <summary>
    /// Represents a scheduled event from CloudWatch
    /// 
    /// {
    ///   "version": "0",
    ///   "id": "125e7841-c049-462d-86c2-4efa5f64e293",
    ///   "detail-type": "Scheduled Event",
    ///   "source": "aws.events",
    ///   "account": "415720405880",
    ///   "time": "2016-12-16T19:55:42Z",
    ///   "region": "us-east-1",
    ///   "resources": [
    ///     "arn:aws:events:us-east-1:415720405880:rule/BackupTest-GetGetBackups-X2YM3334N4JN"
    ///   ],
    ///   "detail": {}
    /// }
    /// </summary>
    public class CloudWatchScheduledEvent
    {
        #region Public Properties

        /// <summary>
        /// The version of the event
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// The event Id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// The type of event, usually "Scheduled Event"
        /// </summary>
        [JsonProperty(PropertyName = "detail-type")]
        public string DetailType
        {
            get
            {
                return "Scheduled Event";
            }
        }

        /// <summary>
        /// The source of the event, usually aws.events
        /// </summary>
        public string Source {
            get
            {
                return "aws.events";
            }
        }

        /// <summary>
        /// The account Id
        /// </summary>
        public string Account { get; }

        /// <summary>
        /// The time the event occured
        /// </summary>
        public DateTime Time { get; }

        /// <summary>
        /// The AWS region the event occured in
        /// </summary>
        public string Region { get; }

        /// <summary>
        /// The arns of the events 
        /// </summary>
        public string[] Resources { get; }

        /// <summary>
        /// Additional information about the event
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public object Detail { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new scheduled CloudWatch event
        /// </summary>
        /// <param name="version">The version</param>
        /// <param name="id">The event id</param>
        /// <param name="account">The AWS Account Id</param>
        /// <param name="time">The time of the event</param>
        /// <param name="region">The region</param>
        /// <param name="resources">The event rule resource</param>
        /// <param name="detail">Event details</param>
        [JsonConstructor]
        public CloudWatchScheduledEvent(
            string version,
            Guid id,
            string account,
            DateTime time,
            string region,
            IEnumerable<string> resources,
            object detail
            )
        {
            if (String.IsNullOrEmpty(version))
            {
                throw new ArgumentNullException("version");
            }

            if (String.IsNullOrEmpty(account))
            {
                throw new ArgumentNullException("account");
            }

            if (String.IsNullOrEmpty(region))
            {
                throw new ArgumentNullException("region");
            }

            this.Version = version;
            this.Id = id;
            this.Account = account;
            this.Time = time;
            this.Region = region;
            this.Resources = resources.ToArray() ?? throw new ArgumentNullException("resources");
            this.Detail = detail;
        }

        #endregion
    }
}
