using System;
using Bio.Properties;

namespace Bio.IO.AppliedBiosystems.DataParsers
{
    /// <summary>
    /// Provides helper methods for converting raw data to .Net data types.
    /// </summary>
    public static class ParserHelper
    {
        /// <summary>
        /// All ab1 arrays are space seperate strings.  Helper function to convert to a property array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parse"></param>
        /// <returns></returns>
        public static T[] ToArray<T>(string value, Func<string, T> parse)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (parse == null) throw new ArgumentNullException("parse");
            string[] values = value.Split(' ');
            var result = new T[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                result[i] = parse(values[i]);
            }

            return result;
        }

        /// <summary>
        /// Segments the array and enumerates those segments.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="count"></param>
        /// <param name="flip">If true flips the order of the segments</param>
        /// <exception cref="ArgumentException">Thrown if the number of values is not divisible by the segment count.</exception>
        /// <returns></returns>
        public static T[][] SegmentArray<T>(T[] values, int count, bool flip)
        {
            if (values == null) throw new ArgumentNullException("values");
            if (values.Length % count != 0)
                throw new ArgumentException(Resource.SegmentByteArrayInvalidCount, "count");

            var result = new T[values.Length / count][];

            int index = 0;
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new T[count];

                for (int j = 0; j < count; j++)
                {
                    if (flip)
                    {
                        result[i][count - 1 - j] = values[index + j];
                    }
                    else
                    {
                        result[i][j] = values[index + j];
                    }
                }

                index += count;
            }

            return result;
        }

        /// <summary>
        /// Converts the array of segments into a array of values.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="segments"></param>
        /// <param name="convert"></param>
        /// <returns></returns>
        public static TOut[] ConvertSegmentsToArray<TIn, TOut>(TIn[][] segments, Func<TIn[], TOut> convert)
        {
            if (segments == null) throw new ArgumentNullException("segments");
            if (convert == null) throw new ArgumentNullException("convert");
            var result = new TOut[segments.Length];

            for (int i = 0; i < segments.Length; i++)
            {
                result[i] = convert(segments[i]);
            }

            return result;
        }
    }
}
