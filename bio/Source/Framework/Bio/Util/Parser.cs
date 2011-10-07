using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Bio.Util
{
    /// <summary>
    /// A class for parsing strings to values of a desired type.
    /// </summary>
    public sealed class Parser
    {
        private Parser() {} //do not let the compiler create a default constructor for a class w/ static members

        /// <summary>
        /// Tries to parse a sequence of strings into a list of values
        /// </summary>
        /// <typeparam name="T">The type of the values</typeparam>
        /// <param name="stringSequence">The sequence of strings to parse</param>
        /// <param name="result">The list of values</param>
        /// <returns>true, if parsing worked; false, otherwise.</returns>
        public static bool TryParseAll<T>(IEnumerable<string> stringSequence, out IList<T> result)
        {
            result = new List<T>();

            foreach (string s in stringSequence)
            {
                T value;
                if (TryParse<T>(s, out value))
                {
                    result.Add(value);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Creates a sequences of values from a sequence of strings
        /// </summary>
        /// <typeparam name="T">The type to parse into</typeparam>
        /// <param name="stringSequence">The sequence of strings to parse</param>
        /// <returns>A sequence of values</returns>
        public static IEnumerable<T> ParseAll<T>(IEnumerable<string> stringSequence)
        {
            foreach (string s in stringSequence)
            {
                yield return Parse<T>(s);
            }
        }

        // This method should be updated to use the rest of the methods in this class.

        /// <summary>
        /// Tries to parse a string into the type given
        /// </summary>
        /// <param name="field">The string to parse</param>
        /// <param name="type">The type to parse into</param>
        /// <returns>The parsed value</returns>
        public static object Parse(string field, Type type)
        {
            var potentialMethods = typeof(Parser).GetMethods().Where(m => m.Name == "Parse" && m.IsGenericMethod);
            MethodInfo parseInfo = potentialMethods.First().MakeGenericMethod(type);

            object[] parameters = new object[] { field };
            return parseInfo.Invoke(null, parameters);
        }

        /// <summary>
        /// Will parse s into T, provided T has a Parse(string) or TryParse(string s, out T t) method defined, or is one of the magical
        /// special cases we've implemented (including ICollection (comma delimited), Nullable and Enums).
        /// </summary>
        /// <typeparam name="T">The type to parse into</typeparam>
        /// <param name="s">the string to parse</param>
        /// <returns>the value</returns>
        public static T Parse<T>(string s)
        {
            T t;
            if (TryParse(s, out t))
            {
                return t;
            }
            else
            {
                throw new ArgumentException(string.Format("Could not parse {0} into an instance of type {1}", s, typeof(T)));
            }
        }

        /// <summary>
        /// Will parse s into T, provided T has a Parse(string) or TryParse(string s, out T t) method defined, or is one of the magical
        /// special cases we've implemented (including ICollection (comma delimited), Nullable and Enums).
        /// </summary>
        /// <typeparam name="T">the type to parse into</typeparam>
        /// <param name="s">the string to parse</param>
        /// <param name="t">the resulting value</param>
        /// <returns>true, if parsing worked; false, otherwise.</returns>
        public static bool TryParse<T>(string s, out T t)
        {
            Type type = typeof(T);
            if (s is T)
            {
                return StringTryParse(s, out t);
            }
            else if (type.IsEnum)
            {
                return EnumTryParse(s, out t);
            }
            else if (type.IsGenericType)
            {
                if (type.FindInterfaces(Module.FilterTypeNameIgnoreCase, "ICollection*").Length > 0)
                {
                    return CollectionsTryParse(s, out t);
                }
                else if (type.Name.StartsWith("Nullable"))
                {
                    return NullableTryParse(s, out t);
                }
            }

            return GenericTryParse(s, out t);
        }

        private static bool NullableTryParse<T>(string s, out T t)
        {
            t = default(T);
            if (string.IsNullOrEmpty(s) || s.Equals("null", StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }

            Type type = typeof(T);
            Type underlyingType = type.GetGenericArguments()[0];
            //underlyingType.TypeInitializer
            MethodInfo tryParse = typeof(Parser).GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static);
            MethodInfo genericTryParse = tryParse.MakeGenericMethod(underlyingType);

            object[] args = new object[] { s, Activator.CreateInstance(underlyingType) };

            bool success = (bool)genericTryParse.Invoke(null, args);
            if (success)
            {
                t = (T)args[1];
            }
            return success;
        }

        private static bool StringTryParse<T>(string s, out T t)
        {
            t = (T)(object)s;
            return true;
        }

        private static bool CollectionsTryParse<T>(string s, out T t)
        {
            Type type = typeof(T);
            Type genericType = type.GetGenericArguments()[0];

            MethodInfo collectionTryParse = typeof(Parser).GetMethod("GenericCollectionsTryParse", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo genericCollectionTryParse = collectionTryParse.MakeGenericMethod(type, genericType);
            t = default(T);
            object[] args = new object[] { s, t };

            bool success = (bool)genericCollectionTryParse.Invoke(null, args);
            if (success)
            {
                t = (T)args[1];
            }
            return success;
        }

        private static bool GenericCollectionsTryParse<T, S>(string s, out T t) where T : ICollection<S>, new()
        {
            t = new T();

            foreach (string itemAsString in s.Split(','))
            {
                S item;
                if (TryParse<S>(itemAsString, out item))
                {
                    t.Add(item);
                }
                else
                {
                    t = default(T);
                    return false;
                }
            }
            return true;
        }

        private static bool EnumTryParse<T>(string s, out T t)
        {
            int i;
            if (int.TryParse(s, out i))
            {
                t = (T)(object)i;
                return true;
            }

            try
            {
                t = (T)Enum.Parse(typeof(T), s, true);
                return true;
            }
            catch (ArgumentException)
            {
            }
            t = default(T);
            return false;
        }

        private static bool GenericTryParse<T>(string s, out T t)
        {
            // now the general one.
            bool success = false;
            t = default(T);
            Type type = typeof(T);

            MethodInfo tryParse = type.GetMethod("TryParse", new Type[] { typeof(string), type.MakeByRefType() });

            if (tryParse != null)
            {
                object[] args = new object[] { s, t };

                success = (bool)tryParse.Invoke(null, args);

                if (success)
                {
                    t = (T)args[1];
                }
            }
            else
            {
                MethodInfo parse = type.GetMethod("Parse", new Type[] { typeof(string) });
                Helper.CheckCondition(parse != null, "Cannot parse type {0}. It does not have a TryParse or Parse method defined", typeof(T));

                try
                {
                    object[] args = new object[] { s };
                    t = (T)parse.Invoke(null, args);
                    success = true;
                }
                catch { }
            }

            return success;
        }
    }
}
