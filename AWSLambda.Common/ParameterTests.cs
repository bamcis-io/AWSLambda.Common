using System;

namespace BAMCIS.AWSLambda.Common
{
    /// <summary>
    /// Convenience methods for common checks on function input parameters
    /// </summary>
    public static class ParameterTests
    {
        #region Public Methods

        /// <summary>
        /// Provides a simple test to use for string parameters to see if they
        /// are null or empty and throws an ArgumentNullException if the string
        /// is null or empty.
        /// </summary>
        /// <param name="value">The value to test</param>
        /// <param name="parameterName">The name of the parameter to include
        /// in the exception</param>
        /// <returns>The original value of the string if not null or empty</returns>
        public static string NotNullOrEmpty(string value, string parameterName, string message = "")
        {
            if (String.IsNullOrEmpty(value))
            {
                if (parameterName == null)
                {
                    parameterName = String.Empty;
                }

                if (message == null)
                {
                    message = String.Empty;
                }

                throw new ArgumentNullException(parameterName, message);
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Tests if the provided expression is true, and if not throws an out of range exception
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="parameterName"></param>
        /// <param name="message"></param>
        public static void OutOfRange(bool expression, string parameterName, string message = "")
        {
            if (expression == false)
            {
                if (parameterName == null)
                {
                    parameterName = String.Empty;
                }

                if (message == null)
                {
                    message = String.Empty;
                }

                throw new ArgumentOutOfRangeException(parameterName, message);
            }
        }

        /// <summary>
        /// Checks that the provided expression is true, and if not, throws
        /// an ArgumentException
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="message"></param>
        public static void Check(bool expression, string message)
        {
            if (expression == false)
            {
                if (message == null)
                {
                    message = String.Empty;
                }

                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// Checks that the provided value is not null and if it is, throws an ArgumentNullException
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static T NonNull<T>(T value, string parameterName, string message = "") where T : class
        {
            if (value == null)
            {
                if (parameterName == null)
                {
                    parameterName = String.Empty;
                }

                if (message == null)
                {
                    message = String.Empty;
                }

                throw new ArgumentNullException(parameterName, message);
            }
            else
            {
                return value;
            }
        }

        #endregion
    }
}
