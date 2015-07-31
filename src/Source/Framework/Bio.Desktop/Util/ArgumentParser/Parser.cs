using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Bio.Extensions;

namespace Bio.Util.ArgumentParser
{
    /// <summary>
    /// Parser Class.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Checks if this type has a Parse or TryParse static method that takes a string as the argument. 
        /// </summary>
        /// <param name="type">The Type.</param>
        /// <returns>True if has parsed method properly.</returns>
        public static bool HasParseMethod(this Type type)
        {
            if (type == null)
                return false;

            MethodInfo tryParse = type.GetMethod("TryParse", new Type[] { typeof(string), type.MakeByRefType() });
            if (tryParse != null && tryParse.IsStatic)
                return true;

            MethodInfo parse = type.GetMethod("Parse", new Type[] { typeof(string) });
            if (parse != null && parse.IsStatic)
                return true;

            return false;
        }

        /// <summary>
        /// Try Parse All.
        /// </summary>
        /// <typeparam name="T">Generic Type.</typeparam>
        /// <param name="values">The Values.</param>
        /// <param name="result">The List result.</param>
        /// <returns>True if Parsed all.</returns>
        public static bool TryParseAll<T>(IEnumerable<string> values, out IList<T> result)
        {
            result = new List<T>();
            if (values == null)
                return false;

            foreach (string s in values)
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
        /// Parse All.
        /// </summary>
        /// <typeparam name="T">Generic type.</typeparam>
        /// <param name="values">The Values.</param>
        /// <returns>List of Parsed types.</returns>
        public static IEnumerable<T> ParseAll<T>(IEnumerable<string> values)
        {
            foreach (string s in values)
            {
                yield return Parse<T>(s);
            }
        }

        /// <summary>
        /// This method should be updated to use the rest of the methods in this class.
        /// </summary>
        /// <param name="field">the Field.</param>
        /// <param name="type">Type.</param>
        /// <returns>Parsed Object.</returns>
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
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <returns></returns>
        public static T Parse<T>(string text)
        {
            if (string.IsNullOrEmpty(text))
                return default(T);

            text = text.Trim();
            T t;
            if (TryParse(text, out t))
            {
                return t;
            }
            else
            {
                throw new ParseException(string.Format("Could not parse \"{0}\" into an instance of type {1}", text, typeof(T)));
            }
        }

        /// <summary>
        /// Will parse s into T, provided T has a Parse(string) or TryParse(string s, out T t) method defined, or is one of the magical
        /// special cases we've implemented (including ICollection (comma delimited), Nullable and Enums).
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="s">s</param>
        /// <param name="t">t</param>
        /// <returns>bool</returns>
        public static bool TryParse<T>(string s, out T t)
        {
            Type type = typeof(T);
            if ((String.IsNullOrEmpty(s)) || (s.Equals("null", StringComparison.CurrentCultureIgnoreCase) && type.IsClass))
            {
                t = default(T);  // return null
                return true;
            }

            s = s.Trim();
            if (s.Equals("help", StringComparison.CurrentCultureIgnoreCase))
            {
                throw ArgumentCollection.GetHelpOnKnownSubtypes(type);
            }
            
            if (s.Equals("help!", StringComparison.CurrentCultureIgnoreCase))
            {
                throw ArgumentCollection.CreateHelpMessage(type);
            }
            
            if (s is T)
            {
                return StringTryParse(s, out t);
            }
            
            if (type.IsEnum)
            {
                return EnumTryParse(s, out t);
            }
            
            if (type.IsGenericType)
            {
                if (type.ParseAsCollection())
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

        /// <summary>
        /// Nullable Try Parse.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
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

        /// <summary>
        /// String Try Parse.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static bool StringTryParse<T>(string s, out T t)
        {
            t = (T)(object)s;
            return true;
        }

        /// <summary>
        /// Collections Try Parse.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static bool CollectionsTryParse<T>(string s, out T t)
        {
            Type type = typeof(T);
            Helper.CheckCondition<ParseException>(type.HasPublicDefaultConstructor(), "Type must must have a constructor, for example, List<string> rather than IList<string>");
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

        /// <summary>
        /// Generic Collections Try Parse.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static bool GenericCollectionsTryParse<T, S>(string s, out T t) where T : ICollection<S>, new()
        {
            t = new T();

            // remove wrapping parens if present
            if (s.StartsWith("(") && s.EndsWith(")"))
                s = s.Substring(1, s.Length - 2);

            //If the string is empty, then the list will be empty
            if (string.IsNullOrEmpty(s))
            {
                return true;
            }

            foreach (string itemAsString in s.ProtectedSplit('(', ')', false, ','))
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

        /// <summary>
        /// Enum Try Parse.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Generic Try Parse.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static bool GenericTryParse<T>(string s, out T t)
        {
            return GenericParser<T>.TryParse(s, out t);
        }

        /// <summary>
        /// Generic Parser class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private static class GenericParser<T>
        {
            private static MethodInfo _tryParse, _parse;

            static GenericParser()
            {
                Type type = typeof(T);
                _tryParse = type.GetMethod("TryParse", new Type[] { typeof(string), type.MakeByRefType() });
                if (_tryParse != null && !_tryParse.IsStatic)
                    _tryParse = null;

                if (_tryParse == null)
                {
                    _parse = type.GetMethod("Parse", new Type[] { typeof(string) });
                    if (_parse != null && !_parse.IsStatic)
                        _parse = null;
                }
            }

            /// <summary>
            /// Is Parsable.
            /// </summary>
            /// <returns></returns>
            public static bool IsParsable()
            {
                return _tryParse != null || _parse != null;
            }

            /// <summary>
            /// Try Parse.
            /// </summary>
            /// <param name="s"></param>
            /// <param name="t"></param>
            /// <returns></returns>
            public static bool TryParse(string s, out T t)
            {
                //Helper.CheckCondition(IsParsable(), "Cannot parse type {0}. It does not have a TryParse or Parse method defined", typeof(T));
                // now the general one.
                bool success = false;
                t = default(T);

                if (_tryParse != null)
                {
                    object[] args = new object[] { s, t };

                    success = (bool)_tryParse.Invoke(null, args);

                    if (success)
                    {
                        t = (T)args[1];
                    }
                }
                else if (_parse != null)
                {
                    try
                    {
                        object[] args = new object[] { s };
                        t = (T)_parse.Invoke(null, args);
                        success = true;
                    }
                    catch (Exception) { }
                }
                else
                {
                    ConstructorArguments constLine = new ConstructorArguments(s);
                    try
                    {
                        t = constLine.Construct<T>();
                        success = true;
                    }
                    catch (HelpException)
                    {
                        throw;
                    }
                    catch (ParseException)
                    {
                        throw;
                    }
                }

                return success;
            }
        }
    }

}
