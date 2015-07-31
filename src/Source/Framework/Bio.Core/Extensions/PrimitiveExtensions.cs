using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Bio.Util
{
    /// <summary>
    /// PrimitiveExtensions
    /// </summary>
    public static class PrimitiveExtensions
    {
        /// <summary>
        /// Enforce
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>bool</returns>
        public static bool Enforce(this bool value)
        {
            return value.Enforce("Value was expected to be true, but is false");
        }

        /// <summary>
        /// Enforce
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="errorMsg">errorMsg</param>
        /// <returns>bool</returns>
        public static bool Enforce(this bool value, string errorMsg)
        {
            if (!value)
                throw new ArgumentException(errorMsg);
            return value;
        }

        /// <summary>
        /// Enforce
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="messageToFormat">messageToFormat</param>
        /// <param name="formatValues">formatValues</param>
        /// <returns>bool</returns>
        public static bool Enforce(this bool condition, string messageToFormat, params object[] formatValues)
        {
            if (!condition)
            {
                Enforce(condition, string.Format(messageToFormat, formatValues));
            }
            return condition;
        }

        /// <summary>
        /// Confirms that a condition is true. Raise an exception of type T if it is not.
        /// </summary>
        /// <param name="condition">The condition to check</param>
        /// <typeparam name="T">The type of exception that will be raised.</typeparam>
        public static bool Enforce<T>(this bool condition) where T : Exception, new()
        {
            return Enforce<T>(condition, string.Format(CultureInfo.CurrentCulture, Properties.Resource.ConditionFailed));
        }

        /// <summary>
        /// Confirms that a condition is true. Raise an exception of type T if it is not.
        /// </summary>
        /// <remarks>
        /// Warning: The message with be evaluated even if the condition is true, so don't make it's calculation slow.
        ///           Avoid this with the "messageFunction" version.
        /// </remarks>
        /// <param name="condition">The condition to check</param>
        /// <param name="message">A message for the exception</param>
        /// <typeparam name="T">The type of exception that will be raised.</typeparam>
        public static bool Enforce<T>(this bool condition, string message) where T : Exception, new()
        {
            if (!condition)
            {
                Type t = typeof(T);
                var constructor = t.GetTypeInfo().DeclaredConstructors.FirstOrDefault(ci => ci.GetParameters()[0].ParameterType == typeof(string));
                if (constructor != null)
                {
                    throw (T)constructor.Invoke(new object[] { message });
                }
                throw new T();
            }
            return condition;
        }

        /// <summary>
        /// Confirms that a condition is true. Raise an exception if it is not.
        /// </summary>
        /// <remarks>
        /// Warning: The message with be evaluated even if the condition is true, so don't make it's calculation slow.
        ///           Avoid this with the "messageFunction" version.
        /// </remarks>
        /// <param name="condition">The condition to check</param>
        /// <param name="messageToFormat">A message for the exception</param>
        /// <param name="formatValues">Values for the exception's message.</param>
        /// <typeparam name="T">The type of exception that will be raised.</typeparam>
        public static bool Enforce<T>(this bool condition, string messageToFormat, params object[] formatValues) where T : Exception, new()
        {
            if (!condition)
            {
                Enforce<T>(condition, string.Format(messageToFormat, formatValues));
            }
            return condition;
        }
    }
}
