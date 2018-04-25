using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using BAMCIS.AWSLambda.Common;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Threading.Tasks;
using Xunit;

namespace AWSLambda.Common.Tests
{
    public class ExtensionMethodTests
    {

        #region Logging

        [Fact]
        public void TestWriteInfo()
        {
            // ARRANGE
            TestLambdaLogger TestLogger = new TestLambdaLogger();
            TestClientContext ClientContext = new TestClientContext();

            ILambdaContext Context = new TestLambdaContext()
            {
                FunctionName = "Common",
                FunctionVersion = "1",
                Logger = TestLogger,
                ClientContext = ClientContext
            };

            // ACT
            Context.LogInfo("This is an info message");

            // ASSERT
            Assert.True(true);
        }

        [Fact]
        public void TestWriteWarning()
        {
            // ARRANGE
            TestLambdaLogger TestLogger = new TestLambdaLogger();
            TestClientContext ClientContext = new TestClientContext();

            ILambdaContext Context = new TestLambdaContext()
            {
                FunctionName = "Common",
                FunctionVersion = "1",
                Logger = TestLogger,
                ClientContext = ClientContext
            };

            // ACT
            Context.LogWarning("This is a warning message");

            // ASSERT
            Assert.True(true);
        }

        [Fact]
        public void TestWriteDebug()
        {
            // ARRANGE
            TestLambdaLogger TestLogger = new TestLambdaLogger();
            TestClientContext ClientContext = new TestClientContext();

            ILambdaContext Context = new TestLambdaContext()
            {
                FunctionName = "Common",
                FunctionVersion = "1",
                Logger = TestLogger,
                ClientContext = ClientContext
            };

            // ACT
            Context.LogDebug("This is debug message");

            // ASSERT
            Assert.True(true);
        }

        [Fact]
        public void TestWriteError()
        {
            // ARRANGE
            TestLambdaLogger TestLogger = new TestLambdaLogger();
            TestClientContext ClientContext = new TestClientContext();

            ILambdaContext Context = new TestLambdaContext()
            {
                FunctionName = "Common",
                FunctionVersion = "1",
                Logger = TestLogger,
                ClientContext = ClientContext
            };

            // ACT
            Context.LogError("This is an error message");

            // ASSERT
            Assert.True(true);
        }

        [Fact]
        public void TestWriteErrorAndException()
        {
            // ARRANGE
            TestLambdaLogger TestLogger = new TestLambdaLogger();
            TestClientContext ClientContext = new TestClientContext();

            ILambdaContext Context = new TestLambdaContext()
            {
                FunctionName = "Common",
                FunctionVersion = "1",
                Logger = TestLogger,
                ClientContext = ClientContext
            };

            // ACT
            Context.LogError("This is an error message", new Exception("This is bad", new FormatException("Poorly formatted", new IndexOutOfRangeException("Not in range"))));

            // ASSERT
            Assert.True(true);
        }

        [Fact]
        public void TestWriteException()
        {
            // ARRANGE
            TestLambdaLogger TestLogger = new TestLambdaLogger();
            TestClientContext ClientContext = new TestClientContext();

            ILambdaContext Context = new TestLambdaContext()
            {
                FunctionName = "Common",
                FunctionVersion = "1",
                Logger = TestLogger,
                ClientContext = ClientContext
            };

            // ACT
            try
            {
                int Num = 0;
                int Result = 100 / Num;
            }
            catch (Exception ex)
            {
                Context.LogError(ex);
            }

            // ASSERT
            Assert.True(true);
        }

        [Fact]
        public void TestWriteAggregateException()
        {
            // ARRANGE
            TestLambdaLogger TestLogger = new TestLambdaLogger();
            TestClientContext ClientContext = new TestClientContext();

            ILambdaContext Context = new TestLambdaContext()
            {
                FunctionName = "Common",
                FunctionVersion = "1",
                Logger = TestLogger,
                ClientContext = ClientContext
            };

            // ACT
            try
            {
                Task T = Task.Factory.StartNew(() =>
                {
                    int Num = 0;
                    int Result = 100 / Num;
                });

                T.Wait();
            }
            catch (AggregateException ex)
            {
                Context.LogError(ex);
            }

            // ASSERT
            Assert.True(true);
        }

        [Fact]
        public async Task TestWriteTaskCancelledException()
        {
            // ARRANGE
            HttpClient Client = new HttpClient()
            {
                Timeout = TimeSpan.FromMilliseconds(500)

            };

            TestLambdaLogger TestLogger = new TestLambdaLogger();
            TestClientContext ClientContext = new TestClientContext();

            ILambdaContext Context = new TestLambdaContext()
            {
                FunctionName = "Common",
                FunctionVersion = "1",
                Logger = TestLogger,
                ClientContext = ClientContext
            };

            // ACT
            try
            {
                HttpRequestMessage Request = new HttpRequestMessage(HttpMethod.Head, "http://www.bamcis.io");
                HttpResponseMessage Res = await Client.SendAsync(Request);
            }
            catch (TaskCanceledException ex)
            {
                Context.LogError(ex);
            }
 
            // ASSERT
            Assert.True(true);
        }

        #endregion

        #region Async

        [Fact]
        public async Task TestInterleaved()
        {
            // ARRANGE
            Task<int>[] Tasks = new[] {
                Task.Delay(3000).ContinueWith(_ => 3),
                Task.Delay(1000).ContinueWith(_ => 1),
                Task.Delay(2000).ContinueWith(_ => 2),
                Task.Delay(5000).ContinueWith(_ => 5),
                Task.Delay(4000).ContinueWith(_ => 4),
            };

            int[] Results = new int[Tasks.Length];
            int Counter = 0;

            // ACT
            foreach (Task<int> CompletedTask in Tasks.Interleaved())
            {
                int Result = await CompletedTask;
                Console.WriteLine($"{DateTime.Now.ToString()}: {Results}");
                Results[Counter++] = Result;
            }

            // ASSERT
            for (int i = 1; i <= Results.Length; i++)
            {
                Assert.Equal(i, Results[i - 1]);
            }
        }

        #endregion
    }
}
