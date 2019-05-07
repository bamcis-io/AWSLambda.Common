using Amazon.Lambda.CloudWatchEvents;
using Amazon.Lambda.SNSEvents;
using BAMCIS.AWSLambda.Common.Events.AutoScaling;
using BAMCIS.AWSLambda.Common.Events.SNS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AWSLambda.Common.Tests
{
    public class AutoScalingEventTests
    {
        #region Private Fields

        private static string instanceLaunchSuccess = @"
{
""version"": ""0"",
""id"": ""12345678-1234-1234-1234-123456789012"",
""detail-type"": ""EC2 Instance Launch Successful"",
""source"": ""aws.autoscaling"",
""account"": ""123456789012"",
""time"": ""2019-01-01"",
""region"": ""us-west-2"",
""resources"": [
""auto-scaling-group-arn"",
""instance-arn""
],
""detail"": {
""StatusCode"": ""InProgress"",
""Description"": ""Launching a new EC2 instance: i-12345678"",
""AutoScalingGroupName"": ""my-auto-scaling-group"",
""ActivityId"": ""87654321-4321-4321-4321-210987654321"",
""Details"": {
""Availability Zone"": ""us-west-2b"",
""Subnet ID"": ""subnet-12345678""
},
""RequestId"": ""12345678-1234-1234-1234-123456789012"",
""StatusMessage"": """",
""EndTime"": ""2019-01-01T01:01:01Z"",
""EC2InstanceId"": ""i-1234567890abcdef0"",
""StartTime"": ""2019-01-01T01:01:01Z"",
""Cause"": ""description-text""
}
}
";

        private static string instanceLaunchFailure = @"
{
""version"": ""0"",
""id"": ""12345678-1234-1234-1234-123456789012"",
""detail-type"": ""EC2 Instance Launch Unsuccessful"",
""source"": ""aws.autoscaling"",
""account"": ""123456789012"",
""time"": ""2019-01-01T01:01:01Z"",
""region"": ""us-west-2"",
""resources"": [
""auto-scaling-group-arn"",
""instance-arn""
],
""detail"": {
""StatusCode"": ""Failed"",
""AutoScalingGroupName"": ""my-auto-scaling-group"",
""ActivityId"": ""87654321-4321-4321-4321-210987654321"",
""Details"": {
""Availability Zone"": ""us-west-2b"",
""Subnet ID"": ""subnet-12345678""
},
""RequestId"": ""12345678-1234-1234-1234-123456789012"",
""StatusMessage"": ""message-text"",
""EndTime"": ""2019-01-01T01:01:01Z"",
""EC2InstanceId"": ""i-1234567890abcdef0"",
""StartTime"": ""2019-01-01T01:01:01Z"",
""Cause"": ""description-text""
}
}
";

        private static string instanceTerminateSuccess = @"
{
""version"": ""0"",
""id"": ""12345678-1234-1234-1234-123456789012"",
""detail-type"": ""EC2 Instance Terminate Successful"",
""source"": ""aws.autoscaling"",
""account"": ""123456789012"",
""time"": ""2019-01-01T01:01:01Z"",
""region"": ""us-west-2"",
""resources"": [
""auto-scaling-group-arn"",
""instance-arn""
],
""detail"": {
""StatusCode"": ""InProgress"",
""Description"": ""Terminating EC2 instance: i-12345678"",
""AutoScalingGroupName"": ""my-auto-scaling-group"",
""ActivityId"": ""87654321-4321-4321-4321-210987654321"",
""Details"": {
""Availability Zone"": ""us-west-2b"",
""Subnet ID"": ""subnet-12345678""
},
""RequestId"": ""12345678-1234-1234-1234-123456789012"",
""StatusMessage"": """",
""EndTime"": ""2019-01-01T01:01:01Z"",
""EC2InstanceId"": ""i-1234567890abcdef0"",
""StartTime"": ""2019-01-01T01:01:01Z"",
""Cause"": ""description-text""
}
}
";

        private static string instanceTerminateFailure = @"
{
""version"": ""0"",
""id"": ""12345678-1234-1234-1234-123456789012"",
""detail-type"": ""EC2 Instance Terminate Unsuccessful"",
""source"": ""aws.autoscaling"",
""account"": ""123456789012"",
""time"": ""2019-01-01T01:01:01Z"",
""region"": ""us-west-2"",
""resources"": [
""auto-scaling-group-arn"",
""instance-arn""
],
""detail"": {
""StatusCode"": ""Failed"",
""AutoScalingGroupName"": ""my-auto-scaling-group"",
""ActivityId"": ""87654321-4321-4321-4321-210987654321"",
""Details"": {
""Availability Zone"": ""us-west-2b"",
""Subnet ID"": ""subnet-12345678""
},
""RequestId"": ""12345678-1234-1234-1234-123456789012"",
""StatusMessage"": ""message-text"",
""EndTime"": ""2019-01-01T01:01:01Z"",
""EC2InstanceId"": ""i-1234567890abcdef0"",
""StartTime"": ""2019-01-01T01:01:01Z"",
""Cause"": ""description-text""
}
}
";

        private static string lifecycleInstanceLaunchEvent = @"
{
""version"": ""0"",
""id"": ""12345678-1234-1234-1234-123456789012"",
""detail-type"": ""EC2 Instance-launch Lifecycle Action"",
""source"": ""aws.autoscaling"",
""account"": ""123456789012"",
""time"": ""2019-01-01T01:01:01Z"",
""region"": ""us-west-2"",
""resources"": [
""auto-scaling-group-arn""
],
""detail"": { 
""LifecycleActionToken"": ""87654321-4321-4321-4321-210987654321"", 
""AutoScalingGroupName"": ""my-asg"", 
""LifecycleHookName"": ""my-lifecycle-hook"", 
""EC2InstanceId"": ""i-1234567890abcdef0"", 
""LifecycleTransition"": ""autoscaling:EC2_INSTANCE_LAUNCHING"",
""NotificationMetadata"": ""additional-info""
} 
}
";

        private static string lifecycleInstanceTerminateEvent = @"
{
""version"": ""0"",
""id"": ""12345678-1234-1234-1234-123456789012"",
""detail-type"": ""EC2 Instance-terminate Lifecycle Action"",
""source"": ""aws.autoscaling"",
""account"": ""123456789012"",
""time"": ""2019-01-01T01:01:01Z"",
""region"": ""us-west-2"",
""resources"": [
""auto-scaling-group-arn""
],
""detail"": { 
""LifecycleActionToken"":""87654321-4321-4321-4321-210987654321"", 
""AutoScalingGroupName"":""my-asg"", 
""LifecycleHookName"":""my-lifecycle-hook"", 
""EC2InstanceId"":""i-1234567890abcdef0"", 
""LifecycleTransition"":""autoscaling:EC2_INSTANCE_TERMINATING"", 
""NotificationMetadata"":""additional-info""
} 
}
";

        private static string snsEventASGLifecycleTerminate = @"
{
""Records"" : 
[
{
""EventSource"" : """",
""EventSubscriptionArn"" : """",
""EventVersion"" : ""1.0"",
""Sns"":
{
""Type"" : ""Notification"",
""MessageId"" : ""383d6d60-3b48-5af3-963d-382a64fc6493"",
""TopicArn"" : ""arn:aws:sns:us-east-1:123456789012:autoscaling"",
""Subject"" : ""Auto Scaling:  Lifecycle action 'TERMINATING' for instance i-076c7bf793337129f in progress."",
""Message"" : ""{\""LifecycleHookName\"":\""test\"",\""AccountId\"":\""123456789012\"",\""RequestId\"":\""4a7f8796-7ed7-4b21-bff7-d190f91afccf\"",\""LifecycleTransition\"":\""autoscaling:EC2_INSTANCE_TERMINATING\"",\""AutoScalingGroupName\"":\""test\"",\""Service\"":\""AWS Auto Scaling\"",\""Time\"":\""2019-05-06T16:14:44.397Z\"",\""EC2InstanceId\"":\""i-076c7bf793337129f\"",\""LifecycleActionToken\"":\""c71ea497-6931-4a15-b047-8ec014ee33a7\""}"",
""Timestamp"" : ""2019-05-06T16:14:44.423Z"",
""SignatureVersion"" : ""1"",
""Signature"" : ""S0axshTqc/cXYnkgYYfVxfACEjcTR9DOr3S/dCIT7wkKOHHOcrHpQFBR6uaBQmUGYjx7f4WQi97KRRR+Su0gLni20jlJRIlIbyqMc/ZnJjGwul8w/HnsWuGYUqvSani5D/tNTsV+GzPbBVfAQaGtC+VYwU7SoxJKcN+k9AGesqhwe0n0dE6GkRW32wC58oYe0dDuFh4/fwwEf2v3d2K6ljZiIcyRDlWdjBURZODGmhMbexSA5LhMmRLDU85jRG6vDiHAUi6JbCR+V9kKcez2QpE3b8M5OW+glo4sahI/p3To1NcOf3S8kMuJGm8zkCzKC0oI+5ZuZmXc16CGs36JJQ=="",
""SigningCertURL"" : ""https://sns.us-east-1.amazonaws.com/SimpleNotificationService-6aad65c2f9911b05cd53efda11f913f9.pem"",
""UnsubscribeURL"" : ""https://sns.us-east-1.amazonaws.com/?Action=Unsubscribe&SubscriptionArn=arn:aws:sns:us-east-1:123456789012:autoscaling:5a360887-1c6e-4ad6-8b44-9a029e58a09a""
}
}
]
}
";

        private static string snsEventASGLifecycleLaunch = @"
{
""Records"" : 
[
{
""EventSource"" : """",
""EventSubscriptionArn"" : """",
""EventVersion"" : ""1.0"",
""Sns"":
{
""Type"" : ""Notification"",
""MessageId"" : ""383d6d60-3b48-5af3-963d-382a64fc6493"",
""TopicArn"" : ""arn:aws:sns:us-east-1:123456789012:autoscaling"",
""Subject"" : ""Auto Scaling:  Lifecycle action 'LAUNCHING' for instance i-076c7bf793337129f in progress."",
""Message"" : ""{\""LifecycleHookName\"":\""test\"",\""AccountId\"":\""123456789012\"",\""RequestId\"":\""4a7f8796-7ed7-4b21-bff7-d190f91afccf\"",\""LifecycleTransition\"":\""autoscaling:EC2_INSTANCE_LAUNCHING\"",\""AutoScalingGroupName\"":\""test\"",\""Service\"":\""AWS Auto Scaling\"",\""Time\"":\""2019-05-06T16:14:44.397Z\"",\""EC2InstanceId\"":\""i-076c7bf793337129f\"",\""LifecycleActionToken\"":\""c71ea497-6931-4a15-b047-8ec014ee33a7\""}"",
""Timestamp"" : ""2019-05-06T16:14:44.423Z"",
""SignatureVersion"" : ""1"",
""Signature"" : ""S0axshTqc/cXYnkgYYfVxfACEjcTR9DOr3S/dCIT7wkKOHHOcrHpQFBR6uaBQmUGYjx7f4WQi97KRRR+Su0gLni20jlJRIlIbyqMc/ZnJjGwul8w/HnsWuGYUqvSani5D/tNTsV+GzPbBVfAQaGtC+VYwU7SoxJKcN+k9AGesqhwe0n0dE6GkRW32wC58oYe0dDuFh4/fwwEf2v3d2K6ljZiIcyRDlWdjBURZODGmhMbexSA5LhMmRLDU85jRG6vDiHAUi6JbCR+V9kKcez2QpE3b8M5OW+glo4sahI/p3To1NcOf3S8kMuJGm8zkCzKC0oI+5ZuZmXc16CGs36JJQ=="",
""SigningCertURL"" : ""https://sns.us-east-1.amazonaws.com/SimpleNotificationService-6aad65c2f9911b05cd53efda11f913f9.pem"",
""UnsubscribeURL"" : ""https://sns.us-east-1.amazonaws.com/?Action=Unsubscribe&SubscriptionArn=arn:aws:sns:us-east-1:123456789012:autoscaling:5a360887-1c6e-4ad6-8b44-9a029e58a09a""
}
}
]
}
";

        #endregion

        /// <summary>
        /// Test a CloudWatch Event for Auto Scaling Launch Success
        /// </summary>
        [Fact]
        public void TestCloudWatchEventLaunchSuccess()
        {
            // ARRANGE
            string json = instanceLaunchSuccess.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            CloudWatchEvent<AutoScalingInstanceStateEvent> evt = JsonConvert.DeserializeObject<CloudWatchEvent<AutoScalingInstanceStateEvent>>(json);
            string content = JsonConvert.SerializeObject(evt, Formatting.None);

            // ASSERT
            Assert.True(evt.Detail.IsSuccess());
            Assert.True(evt.Detail.IsInstanceLaunchSuccess());
        }

        /// <summary>
        /// Test a CloudWatch Event for Auto Scaling Launch Failure
        /// </summary>
        [Fact]
        public void TestCloudWatchEventLaunchFailure()
        {
            // ARRANGE
            string json = instanceLaunchFailure.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            CloudWatchEvent<AutoScalingInstanceStateEvent> evt = JsonConvert.DeserializeObject<CloudWatchEvent<AutoScalingInstanceStateEvent>>(json);

            // ASSERT
            Assert.True(evt.Detail.IsFailure());
            Assert.Equal(Guid.Parse("87654321-4321-4321-4321-210987654321"), evt.Detail.ActivityId);
        }

        /// <summary>
        /// Test a CloudWatch Event for Auto Scaling Terminate Success
        /// </summary>
        [Fact]
        public void TestCloudWatchTerminateSuccess()
        {
            // ARRANGE
            string json = instanceTerminateSuccess.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            CloudWatchEvent<AutoScalingInstanceStateEvent> evt = JsonConvert.DeserializeObject<CloudWatchEvent<AutoScalingInstanceStateEvent>>(json);

            // ASSERT
            Assert.True(evt.Detail.IsSuccess());
        }

        /// <summary>
        /// Test a CloudWatch Event for Auto Scaling Terminate Failure
        /// </summary>
        [Fact]
        public void TestCloudWatchTerminateFailure()
        {
            // ARRANGE
            string json = instanceTerminateFailure.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            CloudWatchEvent<AutoScalingInstanceStateEvent> evt = JsonConvert.DeserializeObject<CloudWatchEvent<AutoScalingInstanceStateEvent>>(json);

            // ASSERT
            Assert.True(evt.Detail.IsFailure());
        }

        /// <summary>
        /// Test a CloudWatch Event for an auto scaling lifecycle hook for launch
        /// </summary>
        [Fact]
        public void TestCloudWatchEventLifecycleLaunch()
        {
            // ARRANGE
            string json = lifecycleInstanceLaunchEvent.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            CloudWatchEvent<AutoScalingLifecycleEvent> msg = JsonConvert.DeserializeObject<CloudWatchEvent<AutoScalingLifecycleEvent>>(json);

            // ASSERT
            Assert.Equal("i-1234567890abcdef0", msg.Detail.EC2InstanceId);
            Assert.Equal("autoscaling:EC2_INSTANCE_LAUNCHING", msg.Detail.LifecycleTransition);
        }

        /// <summary>
        /// Test a CloudWatch Event for an auto scaling lifecycle hook for launch
        /// </summary>
        [Fact]
        public void TestCloudWatchEventLifecycleTerminate()
        {
            // ARRANGE
            string json = lifecycleInstanceTerminateEvent.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            CloudWatchEvent<AutoScalingLifecycleEvent> msg = JsonConvert.DeserializeObject<CloudWatchEvent<AutoScalingLifecycleEvent>>(json);

            // ASSERT
            Assert.Equal("i-1234567890abcdef0", msg.Detail.EC2InstanceId);
            Assert.Equal("autoscaling:EC2_INSTANCE_TERMINATING", msg.Detail.LifecycleTransition);
        }

        /// <summary>
        /// Test an SNS event for a lifecycle hook for launch
        /// </summary>
        [Fact]
        public void TestSNSASGLifecycleLaunchEvent()
        {
            // ARRANGE
            string json = snsEventASGLifecycleLaunch.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            SNSEvent sns = JsonConvert.DeserializeObject<SNSEvent>(json);
            SNSAutoScalingLifecycleHookMessage msg = JsonConvert.DeserializeObject<SNSAutoScalingLifecycleHookMessage>(sns.Records[0].Sns.Message);

            // ASSERT
            Assert.True(msg.IsLaunchEvent());
        }

        /// <summary>
        /// Test an SNS event for a lifecycle hook for terminate
        /// </summary>
        [Fact]
        public void TestSNSASGLifecycleTerminateEvent()
        {
            // ARRANGE
            string json = snsEventASGLifecycleTerminate.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            SNSEvent sns = JsonConvert.DeserializeObject<SNSEvent>(json);
            SNSAutoScalingLifecycleHookMessage msg = JsonConvert.DeserializeObject<SNSAutoScalingLifecycleHookMessage>(sns.Records[0].Sns.Message);

            // ASSERT
            Assert.True(msg.IsTerminateEvent());
        }
    }
}
