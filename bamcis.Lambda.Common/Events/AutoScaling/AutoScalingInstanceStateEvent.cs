using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace BAMCIS.Lambda.Common.Events.AutoScaling
{
    /// <summary>
    /// Represents state change data whenever an instance sucessfully or unsuccessfully
    /// starts or terminates. This is the data contained in the "detail" property of the
    /// CloudWatch Event.
    /// </summary>
    public class AutoScalingInstanceStateEvent : IEquatable<AutoScalingInstanceStateEvent>
    {
        #region Public Properties

        /// <summary>
        /// The status of the activity that triggered the event, either InProgress or Failed
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// The description of the event, failure notifications may not have a description.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        /// <summary>
        /// The name of the Auto Scaling Group where the event occured
        /// </summary>
        public string AutoScalingGroupName { get; set; }

        /// <summary>
        /// A unique id for this scaling activity
        /// </summary>
        public Guid ActivityId { get; set; }

        /// <summary>
        /// The location details for the scaling activity
        /// </summary>
        public AutoScalingInstanceStateEventDetails Details { get; set; }

        /// <summary>
        /// The unique request id
        /// </summary>
        public Guid RequestId { get; set; }

        /// <summary>
        /// The status message for the event, usually empty for successful actions
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string StatusMessage { get; set; }

        /// <summary>
        /// The time the event ended
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// The EC2 instance Id the event relates to
        /// </summary>
        public string EC2InstanceId { get; set; }

        /// <summary>
        /// The start time of the event
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// The cause of the event
        /// </summary>
        public string Cause { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Specifies if the event notification was about a successful Auto
        /// Scaling action
        /// </summary>
        /// <returns></returns>
        public bool IsSuccess()
        {
            return !this.IsFailure();
        }

        /// <summary>
        /// Determines if the event was about a successful instance launch
        /// </summary>
        /// <returns></returns>
        public bool IsInstanceLaunchSuccess()
        {
            return Regex.IsMatch(this.Description, "^Launching a new EC2 instance: i.*$");
        }

        /// <summary>
        /// Determines if the event was about a failed Auto Scaling action
        /// </summary>
        /// <returns></returns>
        public bool IsFailure()
        {
            return this.StatusCode.Equals("Failed", StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            AutoScalingInstanceStateEvent other = (AutoScalingInstanceStateEvent)obj;

            return this.Equals(other);
        }

        public bool Equals(AutoScalingInstanceStateEvent other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            else
            {
                return this.ActivityId == other.ActivityId &&
                this.AutoScalingGroupName == other.AutoScalingGroupName &&
                this.Cause == other.Cause &&
                this.Description == other.Description &&
                this.Details == other.Details &&
                this.EC2InstanceId == other.EC2InstanceId &&
                this.EndTime == other.EndTime &&
                this.RequestId == other.RequestId &&
                this.StartTime == other.StartTime &&
                this.StatusCode == other.StatusCode &&
                this.StatusMessage == other.StatusMessage;
            }
        }

        public override int GetHashCode()
        {
            return Hashing.Hash(
                this.ActivityId,
                this.AutoScalingGroupName,
                this.Cause,
                this.Description,
                this.Details,
                this.EC2InstanceId,
                this.EndTime,
                this.RequestId,
                this.StartTime,
                this.StatusCode,
                this.StatusMessage);
        }

        public static bool operator ==(AutoScalingInstanceStateEvent left, AutoScalingInstanceStateEvent right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (right is null || left is null)
            {
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(AutoScalingInstanceStateEvent left, AutoScalingInstanceStateEvent right)
        {
            return !(left == right);
        }


        #endregion

        #region Internal Class

        /// <summary>
        /// The Auto Scaling event details
        /// </summary>
        public class AutoScalingInstanceStateEventDetails : IEquatable<AutoScalingInstanceStateEventDetails>
        {
            #region Public Properties

            /// <summary>
            /// The availability zone where the instance event occured
            /// </summary>
            [JsonProperty(PropertyName = "Availability Zone")]
            public string AvailabilityZone { get; set; }

            /// <summary>
            /// The subnet Id where the instance event occured
            /// </summary>
            [JsonProperty(PropertyName = "Subnet ID")]
            public string SubnetID { get; set; }

            #endregion

            #region Constructors

            /// <summary>
            /// Default constructor
            /// </summary>
            public AutoScalingInstanceStateEventDetails()
            {
            }

            /// <summary>
            /// Constructor with all properties and appropriate value checks
            /// </summary>
            /// <param name="availabilityZone"></param>
            /// <param name="subnetID"></param>
            public AutoScalingInstanceStateEventDetails(
                string availabilityZone,
                string subnetID)
            {
                this.AvailabilityZone = ParameterTests.NotNullOrEmpty(availabilityZone, "availabilityZone");
                this.SubnetID = ParameterTests.NotNullOrEmpty(subnetID, "subnetID");
            }

            #endregion

            #region Public Methods

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj == null || this.GetType() != obj.GetType())
                {
                    return false;
                }

                AutoScalingInstanceStateEventDetails other = (AutoScalingInstanceStateEventDetails)obj;

                return this.Equals(other);
            }

            public bool Equals(AutoScalingInstanceStateEventDetails other)
            {
                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                else
                {
                    return this.AvailabilityZone == other.AvailabilityZone &&
                    this.SubnetID == other.SubnetID;
                }
            }

            public override int GetHashCode()
            {
                return Hashing.Hash(
                    this.AvailabilityZone,
                    this.SubnetID);
            }

            public static bool operator ==(AutoScalingInstanceStateEventDetails left, AutoScalingInstanceStateEventDetails right)
            {
                if (ReferenceEquals(left, right))
                {
                    return true;
                }

                if (right is null || left is null)
                {
                    return false;
                }

                return left.Equals(right);
            }

            public static bool operator !=(AutoScalingInstanceStateEventDetails left, AutoScalingInstanceStateEventDetails right)
            {
                return !(left == right);
            }

            #endregion
        }

        #endregion
    }
}
