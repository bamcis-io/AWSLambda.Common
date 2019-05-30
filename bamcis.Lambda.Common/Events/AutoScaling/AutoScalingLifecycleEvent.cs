using System;

namespace BAMCIS.Lambda.Common.Events.AutoScaling
{
    /// <summary>
    /// The CloudWatch Event details for an auto-scaling lifecycle event
    /// </summary>
    public class AutoScalingLifecycleEvent
    {
        #region Public Properties

        /// <summary>
        /// The name of the Lifecycle hook
        /// </summary>
        public string LifecycleHookName { get; set; }

        /// <summary>
        /// The lifecycle transition action. This value is either "autoscaling:EC2_INSTANCE_TERMINATING" or "autoscaling:EC2_INSTANCE_LAUNCHING".
        /// </summary>
        public string LifecycleTransition { get; set; }

        /// <summary>
        /// The name of the Auto Scaling Group that triggered the lifecycle notification.
        /// </summary>
        public string AutoScalingGroupName { get; set; }

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

        /// <summary>
        /// Any additional info specified by the user in the event
        /// </summary>
        public string NotificationMetadata { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public AutoScalingLifecycleEvent()
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
        public AutoScalingLifecycleEvent(
            string lifecycleHookName,
            string lifecycleTransition,
            string autoScalingGroupName,
            string ec2InstanceId,
            Guid lifecycleActionToken,
            string notificationMetadata
        )
        {
            this.LifecycleHookName = ParameterTests.NotNullOrEmpty(lifecycleHookName, "lifecycleHookName");
            this.LifecycleTransition = ParameterTests.NotNullOrEmpty(lifecycleTransition, "lifecycleTransition");
            this.AutoScalingGroupName = ParameterTests.NotNullOrEmpty(autoScalingGroupName, "autoScalingGroupName");
            this.EC2InstanceId = ParameterTests.NotNullOrEmpty(ec2InstanceId, "ec2InstanceId");
            this.LifecycleActionToken = lifecycleActionToken;
            this.NotificationMetadata = notificationMetadata;
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
            return !String.IsNullOrEmpty(this.LifecycleTransition) ? this.LifecycleTransition.Equals("autoscaling:EC2_INSTANCE_LAUNCHING", StringComparison.OrdinalIgnoreCase) : false;
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

            AutoScalingLifecycleEvent other = (AutoScalingLifecycleEvent)obj;

            return this.Equals(other);
        }

        public bool Equals(AutoScalingLifecycleEvent other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            else
            {
                return this.AutoScalingGroupName == other.AutoScalingGroupName &&
                this.EC2InstanceId == other.EC2InstanceId &&
                this.LifecycleActionToken == other.LifecycleActionToken &&
                this.LifecycleHookName == other.LifecycleHookName &&
                this.LifecycleTransition == other.LifecycleTransition &&
                this.NotificationMetadata == other.NotificationMetadata;
            }
        }

        public override int GetHashCode()
        {
            return Hashing.Hash(
                this.AutoScalingGroupName,
                this.EC2InstanceId,
                this.LifecycleActionToken,
                this.LifecycleHookName,
                this.LifecycleTransition,
                this.NotificationMetadata);
        }

        public static bool operator ==(AutoScalingLifecycleEvent left, AutoScalingLifecycleEvent right)
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

        public static bool operator !=(AutoScalingLifecycleEvent left, AutoScalingLifecycleEvent right)
        {
            return !(left == right);
        }

        #endregion
    }
}
