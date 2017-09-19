using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Bio.Extensions;

namespace Bio.Util
{
    /// <summary>
    /// A class for parsing strings to values of a desired type.
    /// </summary>
    public static class Parser
    {
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
                if (TryParse(s, out value))
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
            return stringSequence.Select(Parse<T>);
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
            var potentialMethods = typeof(Parser).GetRuntimeMethods().Where(m => m.Name == "Parse" && m.IsGenericMethod);
            MethodInfo parseInfo = potentialMethods.First().MakeGenericMethod(type);
            return parseInfo.Invoke(null, new object[] { field });
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
            
            throw new ArgumentException(string.Format("Could not parse {0} into an instance of type {1}", s, typeof(T)));
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
            if (s is T)
            {
                return StringTryParse(s, out t);
            }

            Type type = typeof(T);
            TypeInfo typeInfo = typeof(T).GetTypeInfo();

            if (typeInfo.IsEnum)
            {
                return EnumTryParse(s, out t);
            }
            
            if (typeInfo.IsGenericType)
            {
                if (type.GetInterfaces().Any(intf => intf.Name.StartsWith("ICollection")))
                {
                    return CollectionsTryParse(s, out t);
                }

                if (typeInfo.Name.StartsWith("Nullable"))
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
            Type underlyingType = type.GetTypeInfo().GenericTypeArguments[0];

            MethodInfo tryParse = typeof(Parser).GetRuntimeMethods().First(mi => mi.Name == "TryParse" && mi.IsStatic && mi.IsPublic);
            MethodInfo genericTryParse = tryParse.MakeGenericMethod(underlyingType);

            object[] args = { s, Activator.CreateInstance(underlyingType) };

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
            Type genericType = type.GetTypeInfo().GenericTypeArguments[0];

            MethodInfo collectionTryParse = typeof(Parser).GetRuntimeMethods().First(mi => mi.Name == "GenericCollectionsTryParse" && mi.IsStatic && mi.IsPublic);
            MethodInfo genericCollectionTryParse = collectionTryParse.MakeGenericMethod(type, genericType);
            
            t = default(T);
            object[] args = { s, t };

            bool success = (bool)genericCollectionTryParse.Invoke(null, args);
            if (success)
            {
                t = (T)args[1];
            }
            return success;
        }

        internal static bool GenericCollectionsTryParse<T, S>(string s, out T t) where T : ICollection<S>, new()
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

            MethodInfo tryParse = type.GetRuntimeMethod("TryParse", new[] { typeof(string), type.MakeByRefType() });

            if (tryParse != null)
            {
                object[] args = { s, t };

                success = (bool)tryParse.Invoke(null, args);

                if (success)
                {
                    t = (T)args[1];
                }
            }
            else
            {
                MethodInfo parse = type.GetRuntimeMethod("Parse", new[] { typeof(string) });
                Helper.CheckCondition(parse != null, "Cannot parse type {0}. It does not have a TryParse or Parse method defined", typeof(T));

                try
                {
                    object[] args = { s };
                    t = (T)parse.Invoke(null, args);
                    success = true;
                }
                catch { }
            }

            return success;
        }
    }
}
