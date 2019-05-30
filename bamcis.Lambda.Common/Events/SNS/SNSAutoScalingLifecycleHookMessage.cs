using System;

namespace BAMCIS.Lambda.Common.Events.SNS
{
    /// <summary>
    /// Represents the Message property contents of an SNS Event. The message body is a JSON string that can be deserialized
    /// into this object for AutoScaling Lifecycle Hook notifications
    /// </summary>
    public class SNSAutoScalingLifecycleHookMessage
    {
        #region Public Properties

        /// <summary>
        /// The name of the Lifecycle hook
        /// </summary>
        public string LifecycleHookName { get; set; }

        /// <summary>
        /// The account id where the Auto Scaling Group lifecycle hook occured
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// The lifecycle hook request id
        /// </summary>
        public Guid RequestId { get; set; }

        /// <summary>
        /// The lifecycle transition action. This value is either "autoscaling:EC2_INSTANCE_TERMINATING" or "autoscaling:EC2_INSTANCE_LAUNCHING".
        /// </summary>
        public string LifecycleTransition { get; set; }

        /// <summary>
        /// The name of the Auto Scaling Group that triggered the lifecycle notification.
        /// </summary>
        public string AutoScalingGroupName { get; set; }

        /// <summary>
        /// The service that caused the lifecycle notification, usually "AWS Auto Scaling"
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// The time the lifecycle hook was triggered
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// The EC2 Instance Id that is the target of the lifecycle hook
        /// </summary>
        public string EC2InstanceId { get; set; }

        /// <summary>
        /// A universally unique identifier (UUID) that identifies a specific lifecycle action associated 
        /// with an instance. Amazon EC2 Auto Scaling sends this token to the notification target you 
        /// specified when you created the lifecycle hook. 
        /// </summary>
        public Guid LifecycleActionToken { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public SNSAutoScalingLifecycleHookMessage()
        { }

        /// <summary>
        /// Constructor with all properties and null/empty checking.
        /// </summary>
        /// <param name="lifecycleHookName"></param>
        /// <param name="accountId"></param>
        /// <param name="requestId"></param>
        /// <param name="lifecycleTransition"></param>
        /// <param name="autoScalingGroupName"></param>
        /// <param name="service"></param>
        /// <param name="time"></param>
        /// <param name="ec2InstanceId"></param>
        /// <param name="lifecycleActionToken"></param>
        public SNSAutoScalingLifecycleHookMessage(
            string lifecycleHookName,
            string accountId,
            Guid requestId,
            string lifecycleTransition,
            string autoScalingGroupName,
            string service,
            DateTime time,
            string ec2InstanceId,
            Guid lifecycleActionToken
        )
        {
            this.LifecycleHookName = ParameterTests.NotNullOrEmpty(lifecycleHookName, "lifecycleHookName");
            this.AccountId = ParameterTests.NotNullOrEmpty(accountId, "accountId");
            this.RequestId = requestId;
            this.LifecycleTransition = ParameterTests.NotNullOrEmpty(lifecycleTransition, "lifecycleTransition");
            this.AutoScalingGroupName = ParameterTests.NotNullOrEmpty(autoScalingGroupName, "autoScalingGroupName");
            this.Service = ParameterTests.NotNullOrEmpty(service, "service");
            this.Time = time;
            this.EC2InstanceId = ParameterTests.NotNullOrEmpty(ec2InstanceId, "ec2InstanceId");
            this.LifecycleActionToken = lifecycleActionToken;
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// Indicates if the event was for an EC2 Instance Launch Event.
        /// </summary>
        /// <returns></returns>
        public bool IsLaunchEvent()
        {
            return !String.IsNullOrEmpty(this.LifecycleTransition) ? this.LifecycleTransition.Equals("autoscaling:EC2_INSTANCE_LAUNCHING", StringComparison.OrdinalIgnoreCase) : false;                
        }

        /// <summary>
        /// Indicates if the event was for an EC2 Instance Terminate Event.
        /// </summary>
        /// <returns></returns>
        public bool IsTerminateEvent()
        {
            return !String.IsNullOrEmpty(this.LifecycleTransition) ? this.LifecycleTransition.Equals("autoscaling:EC2_INSTANCE_TERMINATING", StringComparison.OrdinalIgnoreCase) : false;
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

            SNSAutoScalingLifecycleHookMessage other = (SNSAutoScalingLifecycleHookMessage)obj;

            return this.Equals(other);
        }

        public bool Equals(SNSAutoScalingLifecycleHookMessage other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            else
            {
                return this.AccountId == other.AccountId &&
                this.AutoScalingGroupName == other.AutoScalingGroupName &&
                this.EC2InstanceId == other.EC2InstanceId &&
                this.LifecycleActionToken == other.LifecycleActionToken &&
                this.LifecycleHookName == other.LifecycleHookName &&
                this.LifecycleTransition == other.LifecycleTransition &&
                this.RequestId == other.RequestId &&
                this.Service == other.Service &&
                this.Time == other.Time;              
            }
        }

        public override int GetHashCode()
        {
            return Hashing.Hash(
                this.AccountId,
                this.AutoScalingGroupName,
                this.EC2InstanceId,
                this.LifecycleActionToken,
                this.LifecycleHookName,
                this.LifecycleTransition,
                this.RequestId,
                this.Service,
                this.Time);
        }

        public static bool operator ==(SNSAutoScalingLifecycleHookMessage left, SNSAutoScalingLifecycleHookMessage right)
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

        public static bool operator !=(SNSAutoScalingLifecycleHookMessage left, SNSAutoScalingLifecycleHookMessage right)
        {
            return !(left == right);
        }


        #endregion
    }
}
