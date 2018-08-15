using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BAMCIS.AWSLambda.Common
{
    /// <summary>
    /// Common extension methods for AWS Lambda functions
    /// </summary>
    public static class AWSLambdaExtensionMethods
    {
        #region Public Extension Methods

        /// <summary>
        /// Logs INFO level messages to CloudWatch 
        /// </summary>
        /// <param name="context">The ILambdaContext</param>
        /// <param name="message">The message to log</param>
        /// <returns>The ILambdaContext that was used to log the message</returns>
        public static ILambdaContext LogInfo(this ILambdaContext context, string message)
        {
            return context.LogMessage(message, LogLevel.INFO);
        }

        /// <summary>
        /// Logs DEBUG level messages to CloudWatch 
        /// </summary>
        /// <param name="context">The ILambdaContext</param>
        /// <param name="message">The message to log</param>
        /// <returns>The ILambdaContext that was used to log the message</returns>
        public static ILambdaContext LogDebug(this ILambdaContext context, string message)
        {
            return context.LogMessage(message, LogLevel.DEBUG);
        }

        /// <summary>
        /// Logs WARNING level messages to CloudWatch 
        /// </summary>
        /// <param name="context">The ILambdaContext</param>
        /// <param name="message">The message to log</param>
        /// <returns>The ILambdaContext that was used to log the message</returns>
        public static ILambdaContext LogWarning(this ILambdaContext context, string message)
        {
            return context.LogMessage(message, LogLevel.WARNING);
        }

        /// <summary>
        /// Logs ERROR level messages to CloudWatch 
        /// </summary>
        /// <param name="context">The ILambdaContext</param>
        /// <param name="ex">The exception that will be flattened into json to log</param>
        /// <returns>The ILambdaContext that was used to log the message</returns>
        public static ILambdaContext LogError(this ILambdaContext context, Exception ex)
        {
            return context.LogError(String.Empty, ex);
        }

        /// <summary>
        /// Logs ERROR level messages to CloudWatch 
        /// </summary>
        /// <param name="context">The ILambdaContext</param>
        /// <param name="message">The message to log</param>
        /// <param name="ex">The exception that will be flattened into json and appended to the message</param>
        /// <returns>The ILambdaContext that was used to log the message</returns>
        public static ILambdaContext LogError(this ILambdaContext context, string message, Exception ex = null)
        {
            if (String.IsNullOrEmpty(message) && ex == null)
            {
                throw new ArgumentException("Both the message and exception cannot be null.");
            }

            if (!String.IsNullOrEmpty(message) && ex != null)
            {
                string Message = String.Empty;

                try
                {
                    Message = JsonConvert.SerializeObject(
                        new
                        {
                            Message = message,
                            Exception = ex
                        }
                    );                   
                }
                catch (Exception)
                {
                    Message = JsonConvert.SerializeObject(
                        new
                        {
                            Message = message,
                            Exception = new
                            {
                                ClassName = ex.GetType().FullName,
                                Message = ex.Message,
                                InnerException = (ex.InnerException == null) ? "" : ex.InnerException.Message,
                                StackTrackString = ex.StackTrace
                            }
                        }
                    );
                }

                return context.LogMessage(Message, LogLevel.ERROR);
            }
            else if (!String.IsNullOrEmpty(message))
            {
                return context.LogMessage(message, LogLevel.ERROR);
            }
            else
            {
                try
                {
                    return context.LogMessage(JsonConvert.SerializeObject(ex), LogLevel.ERROR);
                }
                // This can occur in dotnetcore1.0 using Json.NET 10.0.1
                // when trying to serialize a TaskCanceledException, it will
                // throw a JsonSerializationException that there was an error
                // getting the value of Result on System.Threading.Tasks.Task`1[TYPE]
                catch (Exception)
                {
                    string Message = JsonConvert.SerializeObject(
                        new
                        {
                            ClassName = ex.GetType().FullName,
                            Message = ex.Message,
                            InnerException = (ex.InnerException == null) ? "" : ex.InnerException.Message,
                            StackTrackString = ex.StackTrace
                        }
                    );

                    return context.LogMessage(Message, LogLevel.ERROR);
                }
            }
        }

        /// <summary>
        /// Provides an IEnumerable of tasks that will be yielded as the tasks finish so each
        /// item can be processed in the order it finishes, not in the order that were added to the
        /// input IEnumerable or in the order they were started.
        /// 
        /// https://blogs.msdn.microsoft.com/pfxteam/2012/08/02/processing-tasks-as-they-complete/
        /// </summary>
        /// <typeparam name="T">The type of the return value from the async task</typeparam>
        /// <param name="tasks">The tasks that were launched and awaiting finish</param>
        /// <returns>The tasks that were input ordered by their finish time</returns>
        public static IEnumerable<Task<T>> Interleaved<T>(this IEnumerable<Task<T>> tasks)
        {
            List<Task<T>> InputTasks = tasks.ToList();

            TaskCompletionSource<T>[] Buckets = new TaskCompletionSource<T>[InputTasks.Count];
            Task<T>[] Results = new Task<T>[Buckets.Length];

            for (int i = 0; i < Buckets.Length; i++)
            {
                Buckets[i] = new TaskCompletionSource<T>();
                Results[i] = Buckets[i].Task;
            }

            int nextTaskIndex = -1;

            foreach (Task<T> InputTask in InputTasks)
            {
                InputTask.ContinueWith(completed =>
                {
                    TaskCompletionSource<T> Bucket = Buckets[Interlocked.Increment(ref nextTaskIndex)];

                    if (completed.IsFaulted)
                    {
                        if (completed.Exception != null)
                        {
                            Bucket.TrySetException(completed.Exception.InnerExceptions);
                        }
                    }
                    else if (completed.IsCanceled)
                    {
                        Bucket.TrySetCanceled();
                    }
                    else
                    {
                        Bucket.TrySetResult(completed.Result);
                    }

                }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            }

            return Results;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Private method to actually perform the logging
        /// </summary>
        /// <param name="context">The ILambdaContext to use to log</param>
        /// <param name="message">The message to log</param>
        /// <param name="level">The log level</param>
        /// <returns></returns>
        private static ILambdaContext LogMessage(this ILambdaContext context, string message, LogLevel level)
        {
            context.Logger.LogLine($"[{level.ToString()}]\t\t: {message}");
            return context;
        }

        #endregion
    }
}
