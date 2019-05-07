# BAMCIS AWS Lambda Common Utilities

Provides basic Extension Methods for the ILambdaContext to simplify logging messages and exception in Lambda
functions. Also, provides an extension method to handle processing async tasks as they finish to help speed
up execution time inside a Lambda function.

New functionality includes Lambda event sources not included in the AWS .NET SDK, like a scheduled event or a Kinesis Firehose transformation. New deserialization classes have been added for SageMaker so you can process responses from a SageMaker endpoint in a Lambda function.

## Table of Contents
- [Usage](#usage)
	* [Logging](#logging)
	* [Events](#events)
		+ [Custom Resources](#custom-resources)
		+ [SNS S3 Events](#sns-s3-events)
        + [Kinesis Firehose Events](#kinesis-firehose-events)
	* [Custom Resource Handler](#custom-resource-handler)
    * [SageMaker](#sagemaker)
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

    "[INFO]	: This is an informational message."

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

### Events

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

#### Kinesis Firehose Events

AWS provides an SDK for this now, but I don't find it as comprehensive. For example, a Kinesis Stream data record that is sent to Kinesis Firehose has additional data than just the normal Firehose Record. Additionally, this also handles transformation of those records to send back to the Firehose stream.

Provides classes for accepting events in a Lambda function originating from a Kinesis Firehose Transformation. There are two major categories of records in a Kinesis Firehose, records originating from a Kinesis Stream and all of the others. The Kinesis Stream records has an additional property in the record `KinesisRecordMetadata`. For those types of events configure your entrypoint like this:

    public KinesisFirehoseTransformResponse Entrypoint(KinesisFirehoseEvent<KinesisFirehoseKinesisStreamRecord> firehoseEvent, ILambdaContext context)

For all of the other types of events configure your entrypoint like this:

	 public KinesisFirehoseTransformResponse Entrypoint(KinesisFirehoseEvent firehoseEvent, ILambdaContext context)
  
This defaults the records in the `firehoseEvent` object to be of type `KinesisFirehoseRecord`.

After performing the transform return a `KinesisFirehoseTransformResponse` constructed with an `IEnumerable<KinesisFirehoseTranformationRecord>`. Each `KinesisFirehoseTranformationRecord` represents the transform of a single `KinesisFirehoseRecord`. An overall example of a Lambda function tranformation might look like the following. This transformation takes JSON data in a Kinesis Stream and converts it to CSV.

    public KinesisFirehoseTransformResponse Exec(KinesisFirehoseEvent request, ILambdaContext context)
    {
        List<KinesisFirehoseTransformedRecord> TransformedRecords = new List<KinesisFirehoseTransformedRecord>();

        foreach (KinesisFirehoseRecord Record in request.Records)
        {
            try
            {
                string Data = Record.DecodeData();
                JObject Obj = JObject.Parse(Data);
                string Row = Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Join("|", Obj.ToObject<Dictionary<string, string>>().Select(x => x.Value)) + "\n"));

                KinesisFirehoseTransformedRecord Transform = new KinesisFirehoseTransformedRecord(Record.RecordId, Row, TransformationResultStatus.OK);
                TransformedRecords.Add(Transform);
            }
            catch (Exception e)
            {
                KinesisFirehoseTransformedRecord Transform = new KinesisFirehoseTransformedRecord(Record.RecordId, Record.Data, TransformationResultStatus.PROCESSING_FAILED);
                TransformedRecords.Add(Transform);
            }
        }

        return new KinesisFirehoseTransformResponse(TransformedRecords);
    }

### Custom Resource Handler

In addition to the classes for serializing the custom resource requests and responses, there are classes that support aiding
the automation of creating a custom resource Lambda function. The `CustomResourceHandler` class is an abstract class that you 
can inherit in your class the Lambda function handler targets.

    using ...

    // Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
    [assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

    namespace MyLambdaFunction
    {
        public class Entrypoint : CustomResourceHandler
        {
            ...

Then, you just need to implement 3 methods, `CreateAsync`, `UpdateAsync`, and `DeleteAsync`. You shouldn't need to override the
`ExecuteAsync` method, but you can if you want to do something different. I'd reccomend looking at what the default code is before
you override it to make sure you cover all of the cases it manages. The `ExecuteAsync` method can be the direct target of the Lambda
invocation handler, so in the case above you'd specify "MyLambdaFunction::MyLamdbaFunction.Entrypoint::ExecuteAsync".

In addition to the abstract `CustomResourceHandler` class, there is a concrete implementation in the `CustomResourceFactory` class. 
Instead of implementing the 3 required methods in the class, you can specify them as delegates in the class constructor and call the
`ExecuteAsync` method in your entrypoint. For example, your AWS Lambda function might look like this:

	using ...

    // Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
    [assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

    namespace MyLambdaFunction
    {
        public class Entrypoint
        {
	        private ICustomResourceHandler _Handler;

            public Entrypoint() 
            { 
                Func<CustomResourceRequest, ILambdaContext, Task<CustomResourceResponse>> createAsync = async (request, context) => {
                    // Do stuff here to create and return
                    // a CustomResourceResponse object
	            };

                Func<CustomResourceRequest, ILambdaContext, Task<CustomResourceResponse>> updateAsync = async (request, context) => {
                    // Do stuff here to update and return
                    // a CustomResourceResponse object
                };

                Func<CustomResourceRequest, ILambdaContext, Task<CustomResourceResponse>> deleteAsync = async (request, context) => {
                    // Do stuff here to delete and return
                    // a CustomResourceResponse object
                };

                this._Handler = new CustomResourceFactory(
                    createAsync,
                    updateAsync,
                    deleteAsync
                );
           }

           public async Task Run(CustomResourceRequest request, ILambdaContext context) 
           {
               CustomResourceResult result = this._Handler.ExecuteAsync(request, context);

               if (result.IsSuccess)
               {
                   // Do something good here
               }
               else
               {
                   // Log some pertinent info here
               }
           }
        }
    }

All of the request and response processing is performed in the handler, in either implementation you want to use, 
so all you have to do is write the delegate functions or member functions that will implement the actual create, update, and delete logic. 

The `CustomResourceResult` object contains the original request, the response that was generated to 
be sent to the pre-signed S3 url, the http response from S3, and any exception that may have been thrown. The handler also does some basic logging
to CloudWatch during execution to help troubleshoot, but your create, update, and delete functions should log along the way as well.

I recommend constructing the handler in the Lambda function's constructor so you save the latency on building the object on subsequent invocations of
your function.

### SageMaker
Provides classes that can be used to deserialize JSON responses from SageMaker such as Linear Learner Binary Classification, K-Means, or Random Cut Forest. Make sure you have specified your output from the SageMaker endpoint as `application/json`.


This is an example of converting the response from SageMaker for a Linear Learner Binary Classification prediction model.

    LinearLearnerBinaryInferenceResponse Response = JsonConvert.DeserializeObject<LinearLearnerBinaryInferenceResponse>(Json);

## Revision History

### 2.0.0
Removed `CloudWatchScheduledEvent` class, AWS now provides an SDK for these events. Added `SNSAutoScalingLifecycleHookMessage` for receiving ASG lifecycle hook events through SNS. Removed the `AmazonS3ExtensionMethods` class from this package, they are now a standalone library. Removed the `Interleaved` extension method and moved to its own package.

### 1.7.2
Added `S3TestMessage` object for SNS events.

### 1.7.1
Bug fix in `S3ObjectInfo` class.

### 1.7.0
Added extension methods to IAmazonS3 that allow you to copy or move an S3 object using
either a sinlge copy operation or using multipart upload.

### 1.6.3
Added `DecodeData` method to `KinesisFirehoseTransformedRecord`.

### 1.6.2
Updated the `Build` and `BuildAsync` methods for `KinesisFirehoseTransformedRecord`. Now you have the ability to select whether or not the data is decoded from Base64 before being sent to the transformation function.

### 1.6.1
Added the ability to provide an Encoding parameter to the `Build` method of `KinesisFirehoseTransformedRecord`.

### 1.6.0
Added SageMaker response objects.

### 1.5.0
New event object and response object for Kinesis Firehose transformation events, both for records sourced from locations like S3 as well as Kinesis Streams.

### 1.4.0
Added more functionality to the CustomResourceHandler framework. All classes related to custom resource handling have
all been moved into their own namespace.

### 1.3.0
Added the CustomResourceHandler functionality.

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
