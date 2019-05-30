using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using BAMCIS.Lambda.Common;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Lambda.Common.Tests
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
                HttpRequestMessage Request = new HttpRequestMessage(HttpMethod.Head, "http://github.com");
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
    }
}
