using System;
using System.Collections.Generic;
using System.Text;

namespace BAMCIS.AWSLambda.Common
{
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
                throw new ArgumentNullException(parameterName, message);
            }
            else
            {
                return value;
            }
        }

        public static void OutOfRange(bool expression, string parameterName, string message = "")
        {
            if (expression == false)
            {
                throw new ArgumentOutOfRangeException(parameterName, message);
            }
        }

        public static void Check(bool expression, string message)
        {
            if (expression == false)
            {
                throw new ArgumentException(message);
            }
        }

        public static void NonNull<T>(T value, string parameterName, string message = "") where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName, message);
            }
        }

        #endregion
    }
}
