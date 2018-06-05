# BAMCIS AWS Lambda Common Utilities

Provides basic Extension Methods for the ILambdaContext to simplify logging messages and exception in Lambda
functions. Also, provides an extension method to handle processing async tasks as they finish to help speed
up execution time inside a Lambda function.

## Table of Contents
- [Usage](#usage)
	* [Logging](#logging)
	* [Interleaved](#interleaved)
	* [Events](#events)
		+ [CloudWatch Scheduled Event](#cloudwatch-scheduled-event)
		+ [Custom Resources](#custom-resources)
		+ [SNS S3 Events](#sns-s3-events)
- [Revision History](#revision-history)

## Usage

Basic usage examples:

### Logging

Given an ILambdaContext such as

    public Function(CloudWatchScheduledEvent @event, ILambdaContext context)
	{
		...
	}

Logging to CloudWatch logs is simply:

    context.LogInfo("This is an informational message.");

Which will produce

    "[INFO]		: This is an informational message."

You can also you it with exceptions:

    try
    {
      int Result = 100 / 0;
    }
    catch (Exception ex)
    {
      context.LogError(ex);
    }

Which will produce

    {
	  "ClassName":"System.DivideByZeroException",
	  "Message":"Attempted to divide by zero.",
	  "Data":null,
	  "InnerException":null,
	  "HelpURL":null,
	  "StackTraceString":"   at AWSLambda.Common.Tests.ExtensionMethodTests.TestWriteException() in C:\\Workspaces\\AWSLambda.Common\\AWSLambda.Common.Tests\\ExtensionMethodTests.cs:line 144",
	  "RemoteStackTraceString":null,
	  "RemoteStackIndex":0,
	  "ExceptionMethod":null,
	  "HResult":-2147352558,
	  "Source":"AWSLambda.Common.Tests",
	  "WatsonBuckets":null
	}

### Interleaved

    Task<int>[] Tasks = new[] {
      Task.Delay(3000).ContinueWith(_ => 3),
      Task.Delay(1000).ContinueWith(_ => 1),
      Task.Delay(2000).ContinueWith(_ => 2),
      Task.Delay(5000).ContinueWith(_ => 5),
      Task.Delay(4000).ContinueWith(_ => 4),
    };

    foreach (Task<int> CompletedTask in Tasks.Interleaved())
    {
      int Result = await CompletedTask;
      Console.WriteLine($"{DateTime.Now.ToString()}: {Result}");
    }

If the start time is 4/23/2018 08:00:00 AM, this will print

4/23/2018 08:00:01 AM: 1
4/23/2018 08:00:02 AM: 2
4/23/2018 08:00:03 AM: 3
4/23/2018 08:00:04 AM: 4
4/23/2018 08:00:05 AM: 5

Instead of printing:

4/23/2018 08:00:03 AM: 3
4/23/2018 08:00:03 AM: 1
4/23/2018 08:00:03 AM: 2
4/23/2018 08:00:05 AM: 5
4/23/2018 08:00:05 AM: 4

This method was adapted from [here](https://blogs.msdn.microsoft.com/pfxteam/2012/08/02/processing-tasks-as-they-complete/ "processing-tasks-as-they-complete")

### Events

#### CloudWatch Scheduled Event

This is the event type that can be used when a Lambda function is triggered by a scheduled event.

#### Custom Resources

The `CustomResourceRequest` class is the event that is provided when a custom resource is created 
with CloudFormation via a Lambda function. The `CustomResourceResponse` class is used as the item to
be serialized and delivered to the pre-signed S3 url. 

#### SNS S3 Events

The provided classes represent the collection of records inside a SNS message body. For example, the 
`SNSEvent` has a set of records, and each of those records has a `Message` property:

    public void Entrypoint(SNSEvent event, ILambdaContext context)
	{
	    foreach (SNSRecord record in event.Records)
		{
			string message = record.Message;
		}
	}

That `message` string looks like the following JSON:

    {
        "Records": [
            {
                "eventVersion": "2.0",
                "eventSource": "aws:s3",
                "awsRegion": "us-east-1",
                "eventTime": "2018-06-05T04:01:15.272Z",
                "eventName": "ObjectCreated:Put",
                "userIdentity": {
                    "principalId": "AWS:AIDAIFIOCARLEJVXBHEGY"
                },
                "requestParameters": {
                    "sourceIPAddress": "127.0.0.1"
                },
                "responseElements": {
                    "x-amz-request-id": "B89D4CD2EC54A1F6",
                    "x-amz-id-2": "ndIxkfT6tFgOak888pORruzoh6hgIyHLKsaae9VBA1TfwI2cqI0Pe57CIeAUmjeFCjuWWtpuKoU="
                },
                "s3": {
                    "s3SchemaVersion": "1.0",
                    "configurationId": "574fe7f2-ab07-4e6c-b15a-b1428a2b89aa",
				    "bucket": {
                        "name": "test-bucket",
                        "ownerIdentity": {
                            "principalId": "A3KQUKVMLKLRA9"
                        },
                        "arn": "arn:aws:s3:::test-bucket"
                    },
                    "object": {
                        "key": "test.webm",
                        "size": 134112,
                        "eTag": "aeb6838658de200fffa404a863bb1a7f",
                        "sequencer": "005B160B0B3AB574F1"
                    }
                }
            }
        ]
    }

Now you can perform a simple deserialization of that message body:

    SNSS3RecordSet RecordSet = JsonConvert.DeserializeObject<SNSS3RecordSet>(message);

Keep in mind before serializing, you should check to make sure that the message is not a test message that looks like:

    {  
       "Service":"Amazon S3",
       "Event":"s3:TestEvent",
       "Time":"2014-10-13T15:57:02.089Z",
       "Bucket":"bucketname",
       "RequestId":"5582815E1AEA5ADF",
       "HostId":"8cLeGAmw098X5cv4Zkwcmo8vvZa3eH3eKxsPzbB9wrR+YstdA6Knx4Ip8EXAMPLE"
    }

## Revision History

### 1.2.0
Added SNS event records triggered by S3 actions.

### 1.1.2
Fixed the project setting that was defaulting to .NETStandard 1.6.1 and set it to 1.6.0.

### 1.1.1
Fixed Newtonsoft.Json dependency to not target .NETStandard 1.6.1.

### 1.1.0
Added the Custom Resource types and retargetted framework to netstandard1.6.

### 1.0.6
Updated error log output format.

### 1.0.5
Fixed exception handling when an Exception cannot be deserialized by Json.NET successfully.

### 1.0.4
Downgraded Json.NET dependency to only require .NETStandard 1.6.0.

### 1.0.3
Made compatible with .NET Core 1.0

### 1.0.2
Allowed a empty/null input for version in CloudWatchScheduledEvent.

### 1.0.1
Updated package info.

### 1.0.0
Initial release of the application.
