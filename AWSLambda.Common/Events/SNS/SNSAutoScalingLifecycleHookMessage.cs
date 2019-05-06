using System;
using System.Collections.Generic;
using System.Text;

namespace BAMCIS.AWSLambda.Common.Events.SNS
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
    }
}
