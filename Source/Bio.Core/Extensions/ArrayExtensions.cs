using System;
using System.Linq;
using System.Reflection;

namespace Bio.Core.Extensions
{
    /// <summary>
    /// Extension methods for arrays
    /// </summary>
    public static class ArrayExtensions
    {
        private static bool initialized;
        private static PropertyInfo longLengthProperty;
        private static MethodInfo longCopyMethod;

        /// <summary>
        /// This method provides access to the LongLength property in a 
        /// portable fashion by looking it up for the platform using reflection.
        /// The PropertyInfo is cached off for performance.
        /// </summary>
        /// <param name="data">Array</param>
        /// <returns>64-bit length</returns>
        public static long GetLongLength(this Array data)
        {
            Initialize();

            return (longLengthProperty != null)
                ? (long)longLengthProperty.GetValue(data)
                    : data.Length;
        }

        /// <summary>
        /// This method performs a 64-bit array copy if the platform supports it.
        /// </summary>
        /// <param name="source">Source array</param>
        /// <param name="startSource">Starting position</param>
        /// <param name="dest">Destination array</param>
        /// <param name="startDest">Starting position</param>
        /// <param name="count">Count</param>
        public static void LongCopy(Array source, long startSource,
            Array dest, long startDest, long count)
        {
            Initialize();

            if (longCopyMethod != null)
            {
                longCopyMethod.Invoke(null, new object[] { source, startSource, dest, startDest, count });
            }
            else
            {
                Array.Copy(source, (int)startSource, dest, (int)startDest, (int)count);
            }
        }

        /// <summary>
        /// Runs each item through a conversion and returns the produced array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="input"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static TOutput[] ConvertAll<T, TOutput>(this T[] input, Func<T, TOutput> converter)
        {
            if (converter == null)
                throw new ArgumentNullException("converter cannot be null.");

            var output = new TOutput[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = converter(input[i]);
            }

            return output;
        }

        /// <summary>
        /// Method to locate our cached reflection data.
        /// </summary>
        private static void Initialize()
        {
            if (!initialized)
            {
                Type t = typeof(Array);
                longLengthProperty = t.GetRuntimeProperties().FirstOrDefault(pi => pi.Name == "LongLength");
                longCopyMethod =
                    t.GetRuntimeMethods().FirstOrDefault(mi =>
                        mi.Name == "Copy" && mi.GetParameters().Length == 5
                        && mi.GetParameters().Last().ParameterType == typeof(long));

                initialized = true;
            }
        }

        /// <summary>
        /// Returns a new array with the specified range of values.
        /// </summary>
        /// <typeparam name="T">Array type.</typeparam>
        /// <param name="data">Source data.</param>
        /// <param name="startIndex">Index to begind sub array at.</param>
        /// <param name="length">Length of sub array.</param>
        /// <returns></returns>
        public static T[] GetRange<T>(this T[] data, int startIndex, int length)
        {
            if (data == null) throw new ArgumentNullException("data");
            var result = new T[length];

            int index = 0;
            for (int i = startIndex; i < startIndex + length; i++)
            {
                result[index++] = data[i];
            }

            return result;
        }

    }
}
