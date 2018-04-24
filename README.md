# BAMCIS AWS Lambda Common Utilities

Provides basic Extension Methods for the ILambdaContext to simplify logging messages and exception in Lambda
functions. Also, provides an extension method to handle processing async tasks as they finish to help speed
up execution time inside a Lambda function.

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

## Revision History

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
