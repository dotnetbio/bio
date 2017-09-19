using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
#if !SILVERLIGHT
using System.Xml.Linq;
#endif
using System.Collections.Concurrent;
using Bio.Util;
using System.Globalization;

namespace Bio.Util.ArgumentParser
{
    /// <summary>
    /// Supports declarative and strongly typed parsing. Use Construct() to convert an argument collection to an instance of an object
    /// </summary>
    public abstract class ArgumentCollection : ICloneable, IEnumerable<string>
    {
        private const string NO_DOCUMENTATION_STRING = "[No documentation]";
        private List<string> _argList;
        /// <summary>
        /// SubtypeName
        /// </summary>
        public string SubtypeName { get; set; }

        /// <summary>
        /// True/False whether to generate help page
        /// </summary>
        public bool GenerateHelpPage { get; set; }

        /// <summary>
        /// Count in Argument List.
        /// </summary>
        public int Count
        {
            get
            {
                return _argList.Count;
            }
        }

        /// <summary>
        /// Enumerates all flag-value pairs. In case in which there are back to back flags, the first flag is enumerated with null as the value.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> FlagValuePairs
        {
            get
            {
                for (int i = 0; i < _argList.Count - 1; i++)
                {
                    if (IsFlag(_argList[i]))
                    {
                        if (IsFlag(_argList[i + 1]))
                            yield return new KeyValuePair<string, string>(_argList[i], null);
                        else
                            yield return new KeyValuePair<string, string>(_argList[i++], _argList[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="argList">Argument list.</param>
        protected ArgumentCollection(IEnumerable<string> argList)
        {
            if (argList != null)
                _argList = argList.ToList();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lineToParse">Line to Parse.</param>
        protected ArgumentCollection(string lineToParse)
        {
            if (!string.IsNullOrEmpty(lineToParse))
                ParseString(lineToParse);
        }

        /// <summary>
        /// Parse string Function.
        /// </summary>
        /// <param name="lineToParse">Line To parse.</param>
        private void ParseString(string lineToParse)
        {
            SubtypeName = ExtractSubtypeName(ref lineToParse);
            _argList = CreateArgList(lineToParse).ToList();
        }

        /// <summary>
        /// Create Usage String.
        /// </summary>
        /// <param name="requireds">Required member Info.</param>
        /// <param name="requiredParamsOrNull">Required Params or null.</param>
        /// <param name="constructingType">Constructing Type.</param>
        /// <returns></returns>
        abstract protected string CreateUsageString(IEnumerable<MemberInfo> requireds, MemberInfo requiredParamsOrNull, Type constructingType);

        /// <summary>
        /// Extract Sub Type Name.
        /// </summary>
        /// <param name="lineToParse">Line To parse.</param>
        /// <returns>Sub Type Name.</returns>
        abstract protected string ExtractSubtypeName(ref string lineToParse);

        /// <summary>
        /// Create Argument List.
        /// </summary>
        /// <param name="lineToParse">Line To Parse.</param>
        /// <returns>List of Arguments.</returns>
        abstract protected IEnumerable<string> CreateArgList(string lineToParse);

        /// <summary>
        /// Extract Optional Flag Internal.
        /// </summary>
        /// <param name="flag">The flag.</param>
        /// <param name="removeFlag">Remove Flag.</param>
        /// <returns>True if found.</returns>
        abstract protected bool ExtractOptionalFlagInternal(string flag, bool removeFlag);

        /// <summary>
        /// Matches Flag.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="flagBase">Flag Base.</param>
        /// <returns>True if found.</returns>
        abstract public bool MatchesFlag(string query, string flagBase);

        /// <summary>
        /// Is flag.
        /// </summary>
        /// <param name="query">The Query.</param>
        /// <returns>True if found.</returns>
        abstract public bool IsFlag(string query);

        /// <summary>
        /// Create Flag String.
        /// </summary>
        /// <param name="flagBase">Flag Base.</param>
        /// <returns>True if found.</returns>
        abstract protected string CreateFlagString(string flagBase);

        /// <summary>
        /// Add Optional Flag.
        /// </summary>
        /// <param name="argumentName">The argument Name.</param>
        abstract public void AddOptionalFlag(string argumentName);

        /// <summary>
        /// Extract Optional Flag.
        /// </summary>
        /// <param name="flag">The Flag.</param>
        /// <returns>True if found.</returns>
        public bool ExtractOptionalFlag(string flag)
        {
            return ExtractOptionalFlagInternal(flag, true);
        }

        /// <summary>
        /// Peek Optional Flag.
        /// </summary>
        /// <param name="flag">The Flag.</param>
        /// <returns></returns>
        public bool PeekOptionalFlag(string flag)
        {
            return ExtractOptionalFlagInternal(flag, false);
        }

        /// <summary>
        /// Extract Optional.
        /// </summary>
        /// <typeparam name="T">Generic type.</typeparam>
        /// <param name="flag">The Flag.</param>
        /// <param name="defaultValue">Default Value.</param>
        /// <returns>The type of default value.</returns>
        public T ExtractOptional<T>(string flag, T defaultValue)
        {
            return ExtractOptionalInternal<T>(flag, defaultValue, true, null);
        }

        /// <summary>
        /// Peek Optional.
        /// </summary>
        /// <typeparam name="T">Generic Type.</typeparam>
        /// <param name="flag">The Flag.</param>
        /// <param name="defaultValue">Default Value.</param>
        /// <returns>The type of default value.</returns>
        public T PeekOptional<T>(string flag, T defaultValue)
        {
            return ExtractOptionalInternal<T>(flag, defaultValue, false, null);
        }

        /// <summary>
        /// Extract Optional Internal.
        /// </summary>
        /// <typeparam name="T">Generic type.</typeparam>
        /// <param name="flag">The flag.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <param name="removeFlagAndValue">Remove flag and value.</param>
        /// <param name="defaultParseArgsOrNull">Default Parse Arguments Or Null</param>
        /// <returns>The type of default value.</returns>
        protected T ExtractOptionalInternal<T>(string flag, T defaultValue, bool removeFlagAndValue, string defaultParseArgsOrNull)
        {
            int argIndex = FindFlag(flag);

            if (argIndex == -1)
            {
                return defaultValue;
            }

            Helper.CheckCondition<ParseException>(argIndex < _argList.Count - 1, @"Expect a value after ""{0}""", flag);

            if (removeFlagAndValue)
                RemoveAt(argIndex);
            else
                argIndex++;

            return ExtractAtInternal<T>(flag, argIndex, defaultParseArgsOrNull, removeFlagAndValue);
        }

        /// <summary>
        /// Extract the argument name from position specified.
        /// </summary>
        /// <typeparam name="T">Generic type.</typeparam>
        /// <param name="argumentName">Argument Name.</param>
        /// <param name="argPosition">Argument position.</param>
        /// <returns>True if found.</returns>
        public T ExtractAt<T>(string argumentName, int argPosition)
        {
            return ExtractAtInternal<T>(argumentName, argPosition, null);
        }

        /// <summary>
        /// Extract At Internal.
        /// </summary>
        /// <typeparam name="T">Generic Type.</typeparam>
        /// <param name="argumentName">Argument Name.</param>
        /// <param name="argPosition">Argument position.</param>
        /// <param name="defaultParseArgsOrNull">Default Parse Argument Or Null.</param>
        /// <param name="removeValue">Remove value.</param>
        /// <returns>True if found.</returns>
        protected T ExtractAtInternal<T>(string argumentName, int argPosition, string defaultParseArgsOrNull, bool removeValue = true)
        {
            Helper.CheckCondition<ParseException>(_argList.Count > argPosition, "Expect {0} at position {1}. Only {2} arguments remain to be parsed.", argumentName, argPosition, Count);
            T t;
            CheckForHelp<T>(_argList[argPosition]);

            if (!string.IsNullOrWhiteSpace(defaultParseArgsOrNull)) // we know we're parsing via ConstructorArguments. 
            {
                if (!defaultParseArgsOrNull.StartsWith("(")) defaultParseArgsOrNull = "(" + defaultParseArgsOrNull + ")";
                ConstructorArguments defaultArgs = new ConstructorArguments(defaultParseArgsOrNull);
                ConstructorArguments baseArgs = new ConstructorArguments(_argList[argPosition]);
                t = baseArgs.Construct<T>(defaultArgsOrNull: defaultArgs, checkComplete: true);
            }
            else
            {
                Parser.TryParse(_argList[argPosition], out t).Enforce<ParseException>(@"Expect value for ""{0}"" to be a {1}. Read {2}", argumentName, typeof(T).ToTypeString(), _argList[argPosition]);
            }

            if (removeValue)
                RemoveAt(argPosition);

            return t;
        }

        /// <summary>
        /// Check That Empty.
        /// </summary>
        public void CheckThatEmpty()
        {
            if (_argList.Count != 0)
                throw new ParseException(@"Unknown arguments found. Check the spelling of flags. {0}", _argList.StringJoin(" "));
        }

        /// <summary>
        /// Check for No More Options.
        /// </summary>
        /// <param name="numberOfRequiredArgumentsOrNull">Number Of Required Arguments Or Null.</param>
        /// <param name="parseObjectTypeOrNull">Parse Object Type Or Null.</param>
        public void CheckNoMoreOptions(int? numberOfRequiredArgumentsOrNull, string parseObjectTypeOrNull)
        {
            foreach (string arg in _argList)
            {
                if (IsFlag(arg)) throw new ParseException(string.Format(CultureInfo.CurrentCulture, Properties.Resource.UnknownOption,  arg,
                   string.IsNullOrEmpty(parseObjectTypeOrNull) ? string.Empty : string.Format(CultureInfo.CurrentCulture, Properties.Resource.ParsingError, parseObjectTypeOrNull)));
            }

            if (null != numberOfRequiredArgumentsOrNull)
            {
                if (_argList.Count != (int)numberOfRequiredArgumentsOrNull)
                    throw new ParseException(string.Format(CultureInfo.CurrentCulture, Properties.Resource.ExpectedArguments,
                        numberOfRequiredArgumentsOrNull, _argList.Count, ToString(), string.IsNullOrEmpty(parseObjectTypeOrNull) ? "" : string.Format(CultureInfo.CurrentCulture, Properties.Resource.ParsingError, parseObjectTypeOrNull)));
            }
        }

        /// <summary>
        /// Force Optional Flag.
        /// </summary>
        /// <param name="optionalFlag">Optional Flag.</param>
        public void ForceOptionalFlag(string optionalFlag)
        {
            this.ExtractOptionalFlag(optionalFlag);
            this.AddOptionalFlag(optionalFlag);
        }

        /// <summary>
        /// Force Optional.
        /// </summary>
        /// <typeparam name="T">Generic Type.</typeparam>
        /// <param name="argumentName">Argument Name.</param>
        /// <param name="argumentValue">Argument Value.</param>
        public void ForceOptional<T>(string argumentName, T argumentValue)
        {
            this.ExtractOptional<T>(argumentName, argumentValue);
            this.AddOptional(argumentName, argumentValue);
        }

        /// <summary>
        /// Extract Next Argument Name.
        /// </summary>
        /// <typeparam name="T">Generic Type.</typeparam>
        /// <param name="argumentName">Argument Name.</param>
        /// <returns>True if found.</returns>
        public T ExtractNext<T>(string argumentName)
        {
            Helper.CheckCondition<ParseException>(_argList.Count > 0, @"Expect ""{0}"" value", argumentName);
            return ExtractAt<T>(argumentName, 0);
        }

        /// <summary>
        /// Contains Optional Flag.
        /// </summary>
        /// <param name="flag">The flag.</param>
        /// <returns>True if contains optional flag.</returns>
        public bool ContainsOptionalFlag(string flag)
        {
            return FindFlag(flag) > -1;
        }

        /// <summary>
        /// Find if the Flag is present.
        /// </summary>
        /// <param name="flag">The Flag.</param>
        /// <returns>True if flag found.</returns>
        public int FindFlag(string flag)
        {
            int idx = -1;
            for (int i = 0; i < _argList.Count; i++)
            {
                if (MatchesFlag(_argList[i], flag))
                {
                    Helper.CheckCondition<ParseException>(idx < 0, () => string.Format(CultureInfo.CurrentCulture, Properties.Resource.RepeatedFlags, flag));
                    idx = i;
                }
            }
            return idx;
        }

        /// <summary>
        /// Get Underlying Array.
        /// </summary>
        /// <returns>Argument List.</returns>
        public string[] GetUnderlyingArray()
        {
            return _argList.ToArray();
        }

        /// <summary>
        /// Insert argument name with specified index.
        /// </summary>
        /// <param name="idx">The Index.</param>
        /// <param name="argumentName">Argument Name.</param>
        public void Insert(int idx, string argumentName)
        {
            _argList.Insert(idx, argumentName);
        }

        /// <summary>
        /// Add Argument to Argument List.
        /// </summary>
        /// <param name="argument">Argument to be added to list.</param>
        public void Add(object argument)
        {
            _argList.Add((argument ?? "null").ToString());
        }

        /// <summary>
        /// Add Optional.
        /// </summary>
        /// <param name="argumentName">Argument Name.</param>
        /// <param name="argumentValue">Argument Value.</param>
        public void AddOptional(string argumentName, object argumentValue)
        {
            _argList.Add(CreateFlagString(argumentName));
            _argList.Add((argumentValue ?? "null").ToString());
        }

        /// <summary>
        /// Equals.
        /// </summary>
        /// <param name="obj">The object to be compared.</param>
        /// <returns>True if equal.</returns>
        public override bool Equals(object obj)
        {
            if ((this == null) && (obj == null))
                return true;
            if (this == null || obj == null)
                return false;

            return obj.GetType().Equals(this.GetType()) && _argList.SequenceEqual(((ArgumentCollection)obj)._argList);
        }

        /// <summary>
        /// Get Hash Code.
        /// </summary>
        /// <returns>Hash Code.</returns>
        public override int GetHashCode()
        {
            return _argList.StringJoin(";").GetHashCode();
        }

        #region ICloneable Members

        /// <summary>
        /// Copy of object.
        /// </summary>
        /// <returns>Clone of object.</returns>
        public object Clone()
        {
            ArgumentCollection result = (ArgumentCollection)MemberwiseClone();
            result._argList = new List<string>(_argList);
            return result;
        }

        #endregion

        #region IEnumerable<string> Members

        /// <summary>
        /// Enumerator of the Argument List.
        /// </summary>
        /// <returns>Enumerator of Argument list.</returns>
        public IEnumerator<string> GetEnumerator()
        {
            return _argList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Gets enumerator.
        /// </summary>
        /// <returns>Enumerable Argument list.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Removes arguments at specified index from Argument list.
        /// </summary>
        /// <param name="idx">The Index.</param>
        protected void RemoveAt(int idx)
        {
            _argList.RemoveAt(idx);
        }

        #region Type construction

        /// <summary>
        /// Constructs T, then runs it. Catches Help and Parse exceptions and write's their messages to Console.Error. 
        /// All other exceptions are allowed to pass on through. Note that if a help exception is caught, the ExitCode
        /// will be set to 10022 (Operation canceled by user); if a parse exception is caught, the ExitCode is set
        /// to 1223 (Invalid argument). In either case, there is no apparent affect on the console, but the cluster
        /// will mark the task as failed.
        /// </summary>
        /// <typeparam name="T">A parsable type that implements IRunnable.</typeparam>
        public void ConstructAndRun<T>() where T : IRunnable
        {
            try
            {
                try
                {
                    T runnable = Construct<T>();
                    runnable.Run();
                }
                catch (Exception e)
                {
                    throw (e is ParseException || e is HelpException) ? e :
                        (e.InnerException is ParseException || e.InnerException is HelpException) ?
                            e.InnerException :
                            new Exception("Run failure.", e);
                }
            }
            catch (HelpException help)
            {
                if (GenerateHelpPage)
                {
                    Console.Error.WriteLine(help.Message);
                    Environment.ExitCode = 10022; // "The operation was canceled by the user"
                }
                else
                    throw; // Rethrow
            }
            catch (ParseException parse)
            {
                Console.Error.WriteLine("Parse Error: " + parse.Message);
                Environment.ExitCode = 1223; // "An invalid argument was supplied"
            }

        }

        /// <summary>
        /// Constructs an instance of type T from this ArgumentCollection. If SubtypeName is not null, will construct an instance 
        /// of the type SubtypeName, which is constructed by looking in all referenced assemblies for a type with the corresponding name.
        /// If SubtypeName is not a subtype of T, then an exception will be thrown. Whatever type is constructed must have a parameterless 
        /// constructor. Also, the type must have the //[Parsable] attribute. By default all of the type's public fields will be optional parameters. 
        /// Non-public fields can be marked as parsable, as can properties. Public fields can be hidden from parsing. Any field or property can 
        /// be marked as required. Mark fields as properties using the Parse attribute. The three Parse attributers are:
        /// [Parse(ParseAction.Optional)]  (mark as optional. public fields default to this)
        /// [Parse(ParseAction.Required)]  (mark as requried.)
        /// [Parse(ParseAction.Ignore)]    (ignore this public field. has no effect on non-public members or non-fields)
        /// </summary>
        /// <exception cref="HelpException">HelpException is thrown if a help request is encountered. The message will contain the help string.</exception>
        /// <exception cref="ParseException">ParseException is thrown if a type is not able to be parsed.</exception>
        /// <typeparam name="T">The type to construct. If SubtypeName is not null, will attempt to construct an instance of SubtypeName. If that is not 
        /// and instance of T, an exception will be thrown.</typeparam>
        /// <param name="checkComplete">If true, will make sure ArgumentCollection is empty when done parsing and will throw an exception if it is not.
        /// Set to false if you want to construct a class from ArgumentCollection and you expect arguments to be left over.</param>
        /// <param name="defaultArgsOrNull">defaultArgsOrNull</param> 
        /// <returns>Instance of type T from this ArgumentCollection.</returns>
        public T Construct<T>(bool checkComplete = true, ArgumentCollection defaultArgsOrNull = null)
        {
            Type tType = typeof(T);

            object result = CreateInstance<T>();
            if (result == null)
                return default(T); 

            Helper.CheckCondition<ParseException>(result is T, "Constructed an instance of {0}, which is not an instance of {1}", tType.ToTypeString(), typeof(T).ToTypeString());

            ParseInto((T)result, checkComplete, defaultArgsOrNull);

            return (T)result;
        }

        /// <summary>
        /// Parses an instance of type T from this ArgumentCollection.
        /// </summary>
        /// <typeparam name="T">The type to Parse. If SubtypeName is not null, will attempt to Parse an instance of SubtypeName. If that is not 
        /// and instance of T, an exception will be thrown.</typeparam>
        /// <param name="parseResult">Parse Result.</param>
        /// <param name="checkComplete">If true, will make sure ArgumentCollection is empty when done parsing and will throw an exception if it is not.
        /// Set to false if you want to construct a class from ArgumentCollection and you expect arguments to be left over.</param>
        /// <param name="defaultArgsOrNull">Default Arguments Or Null.</param>
        public void ParseInto<T>(T parseResult, bool checkComplete = true, ArgumentCollection defaultArgsOrNull = null)
        {
            object result = parseResult;
            Type tType = result.GetType();   // update type in case we constructed a derived type
            tType.IsConstructable().Enforce("Type {0} does not have a public default constructor and so cannot be parsed.", tType);

            if (HelpIsRequested())
            {
                HelpException helpMsg = CreateHelpMessage(result, includeDateStamp: true);
                throw helpMsg;
            }

            AddDefaultArgsIfMissing(defaultArgsOrNull);

            List<MemberInfo> optionals, requireds, constructingStrings;
            MemberInfo requiredParams;
            GetParsableMembers(tType, out optionals, out requireds, out constructingStrings, out requiredParams);

            // if the user wants to know the exact string used to construct this object, set these fields.
            string constString = this.ToString();
            constructingStrings.ForEach(member => SetFieldOrPropertyValue(ref result, member, constString));

            LoadOptionalArguments(ref result, optionals);
            LoadRequiredArguments(ref result, ref requiredParams, requireds, checkComplete && requiredParams == null);
            if (requiredParams != null)
                LoadRequiredParams(ref result, requiredParams);

            if (checkComplete) CheckThatEmpty();

            if (result is IParsable)
                ((IParsable)result).FinalizeParse();

            //return (T)result;
        }

        /// <summary>
        /// Looks at all the flag-value pairs in defaultArgsOrNull and adds any to the current collection that are not already there.
        /// </summary>
        /// <param name="defaultArgsOrNull">Default Arguments Or Null.</param>
        private void AddDefaultArgsIfMissing(ArgumentCollection defaultArgsOrNull)
        {
            if (defaultArgsOrNull == null)
                return;

            foreach (var flagAndValue in defaultArgsOrNull.FlagValuePairs)
            {
                if (!this.ContainsOptionalFlag(flagAndValue.Key))
                {
                    if (flagAndValue.Value == null) // is a flag from a CommandArguments
                        AddOptionalFlag(flagAndValue.Key);
                    else
                        AddOptional(flagAndValue.Key, flagAndValue.Value);
                }
            }
        }

        /// <summary>
        /// Help Is Requested.
        /// </summary>
        /// <returns>True if displayed.</returns>
        private bool HelpIsRequested()
        {
            // Allow for "/h", "/help", "help!", "-help" or "-h"
            return ExtractOptionalFlag("help") 
                || ExtractOptionalFlag("h") 
                || _argList.Any(arg => arg.Equals("help!", StringComparison.CurrentCultureIgnoreCase));
        }

        //private bool IsParsable(Type type)
        //{
        //    return Attribute.IsDefined(type, typeof(ParsableAttribute));
        //}

        /// <summary>
        /// Create an instance of type T.
        /// </summary>
        /// <typeparam name="T">The type to Create. If SubtypeName is not null, will attempt to Create an instance of SubtypeName. If that is not 
        /// and instance of T, an exception will be thrown.</typeparam>
        /// <returns>Created Instance.</returns>
        private T CreateInstance<T>()
        {
            CheckForHelp<T>(SubtypeName);
            Type t = typeof(T);
            string subtypeName = SubtypeName;

            // first, see if subtypeName is simply refering to T
            if (subtypeName != null && subtypeName.Equals(t.ToTypeString(), StringComparison.CurrentCultureIgnoreCase))
                subtypeName = null;

            // now check to see if the users wants a null reference
            if ("null".Equals(subtypeName, StringComparison.CurrentCultureIgnoreCase))
            {
                Helper.CheckCondition<ParseException>(!t.IsValueType, "Cannot construct a null instance of a non-ref type. {0} is a value type.", t.ToTypeString());
                return default(T);
            }

            // now try creating an instance of type T out of the subtypeName
            Type subtype;
            if (subtypeName != null)
            {
                List<string> subtypeNamesToTry = new List<string> { subtypeName };
                if (t.IsGenericType && !SubtypeName.Contains('<'))
                    subtypeNamesToTry.Add(subtypeName + "<" + t.GetGenericArguments().Select(arg => arg.Name).StringJoin(",") + ">");

                foreach (string subtypeNameToTry in subtypeNamesToTry)
                {
                    if (TypeFactory.TryGetType(subtypeNameToTry, t, out subtype) && subtype.HasPublicDefaultConstructor())
                        return (T)Activator.CreateInstance(subtype);
                }

                // If there were no arguments, then it's a good chance subtypeName was supposed to be a single required argument
                Helper.CheckCondition<ParseException>(_argList.Count == 0, "Cannot construct an instance of type {0} from the string {1}", t.ToTypeString(), subtypeName);
                _argList.Add(SubtypeName);
                subtypeName = null;
            }

            // let's just make an instance of this type.
            Helper.CheckCondition<ParseException>(!t.IsAbstract && !t.IsInterface, "Can't create an instance of  abstract type or interface {1}. Please specify a valid subtype name, or use help for options. Input string: {0}", this, t.ToTypeString());
            return Activator.CreateInstance<T>();
        }

        // ref so that this works with structs
        /// <summary>
        /// Load Required Params.
        /// </summary>
        /// <param name="result">The result object.</param>
        /// <param name="requiredParamsArg">Required parameter argument.</param>
        private void LoadRequiredParams(ref object result, MemberInfo requiredParamsArg)
        {
            //Helper.CheckCondition<ParseException>(_argList.Count > 0, "Expected at least one remaining argument for the params argument {0}", requiredParamsArg.Name);
            if (_argList.Count == 1 || _argList.Count == 2 && IsFlag(_argList[0]))
            {
                this.LoadArgument(ref result, requiredParamsArg, isOptional: _argList.Count == 2);
            }
            else
            {
                string remainingArgsAsList = string.Format("({0})", _argList.StringJoin(ConstructorArguments.ArgumentDelimiter.ToString()));
                _argList.Clear();
                _argList.Add(remainingArgsAsList);
                this.LoadArgument(ref result, requiredParamsArg, isOptional: false);
            }
        }


        // ref so that this works with structs
        /// <summary>
        /// Load Required Params.
        /// </summary>
        /// <param name="result">The result object.</param>
        /// <param name="requiredParams">Required parameters.</param>
        /// <param name="requireds">List of required parameters.</param>
        /// <param name="checkComplete">Check Complete.</param>
        private void LoadRequiredArguments(ref object result, ref MemberInfo requiredParams, IEnumerable<MemberInfo> requireds, bool checkComplete)
        {
            List<MemberInfo> unparsedRequireds = new List<MemberInfo>();
            foreach (var member in requireds)
            {
                if (!LoadArgument(ref result, member, true))    // try to load it as an optional arg first.
                    unparsedRequireds.Add(member);
            }

            if (requiredParams != null)
                if (LoadArgument(ref result, requiredParams, true))
                    requiredParams = null;

            if (checkComplete) CheckNoMoreOptions(unparsedRequireds.Count, result.GetType().ToTypeString());
            else if (unparsedRequireds.Count > 0) CheckNoMoreOptions(null, result.GetType().ToTypeString()); // you have to at least check that there are no more options if there are unnamed required arguments to parse.

            foreach (var member in unparsedRequireds)
                LoadArgument(ref result, member, false);
        }

        // ref so that this works with structs
        /// <summary>
        /// Load Optional Params.
        /// </summary>
        /// <param name="result">The result object.</param>
        /// <param name="optionals">List of optional parameters.</param>
        private void LoadOptionalArguments(ref object result, IEnumerable<MemberInfo> optionals)
        {
            foreach (MemberInfo member in optionals)
            {
                string flag = member.Name;
                if (TreatOptionAsFlag(result, member))  // load as a flag if and only if it's a boolean and the default is false.
                {
                    LoadFlag(ref result, member);
                }
                else
                {
                    LoadArgument(ref result, member, true);
                }
            }
        }

        /// <summary>
        /// Treat Option As Flag.
        /// </summary>
        /// <param name="defaultObject">Default Object.</param>
        /// <param name="member">The Member.</param>
        /// <returns></returns>
        private static bool TreatOptionAsFlag(object defaultObject, MemberInfo member)
        {
            Type memberType = GetActualParsingFieldOrPropertyType(member);
            return memberType.Equals(typeof(bool)) && !(bool)GetFieldOrPropertyValue(defaultObject, member);
        }

        // ref so that this works with structs
        /// <summary>
        /// Load Flag.
        /// </summary>
        /// <param name="result">The result object.</param>
        /// <param name="member">The member.</param>
        private void LoadFlag(ref object result, MemberInfo member)
        {
            bool value = ExtractOptionalFlag(member.Name);

            SetFieldOrPropertyValue(ref result, member, value);
        }

        /// <summary>
        /// Set Field O rProperty Value.
        /// </summary>
        /// <param name="obj">The Object.</param>
        /// <param name="member">The Member.</param>
        /// <param name="value">Object Value.</param>
        private static void SetFieldOrPropertyValue(ref object obj, MemberInfo member, object value)
        {
            Type declaredType = GetFieldOrPropertyType(member);
            value = ImplicitlyCastValueToType(value, declaredType);

            try
            {
                FieldInfo field = member as FieldInfo;
                if (field != null)
                {
                    field.SetValue(obj, value);
                }
                else
                {
                    PropertyInfo property = member as PropertyInfo;
                    Helper.CheckCondition<ParseException>(property != null, "Invalid member type {0}", member.MemberType);
                    property.SetValue(obj, value, null);
                }
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException is ParseException || e.InnerException is HelpException)
                {
                    throw e.InnerException;
                }
            }
        }

        /// <summary>
        /// Implicitly Cast Value To Type.
        /// </summary>
        /// <param name="value">The Object Value.</param>
        /// <param name="destinationTypeOrNull">Destination Type Or Null.</param>
        /// <returns>Implicitly casted object.</returns>
        public static object ImplicitlyCastValueToType(object value, Type destinationTypeOrNull)
        {
            object result;
            if (TryImplicitlyCastValueToType(value, destinationTypeOrNull, out result))
                return result;
            else
                throw new ParseException("The class {0} must define an implicit operator for converting from {1}. See the C# keyword 'implicit'.",
                                destinationTypeOrNull.ToTypeString(), value.GetType().ToTypeString());

        }

        /// <summary>
        /// Try Implicitly Cast Value To Type.
        /// </summary>
        /// <param name="value">The Object Value.</param>
        /// <param name="destinationTypeOrNull">Destination Type Or Null.</param>
        /// <param name="result">The result object.</param>
        /// <returns>True if implicitly casted.</returns>
        public static bool TryImplicitlyCastValueToType(object value, Type destinationTypeOrNull, out object result)
        {
            if (value == null || destinationTypeOrNull == null)
            {
                result = value;
                return true;
            }

            Type sourceType = value.GetType();

            if (sourceType.Equals(destinationTypeOrNull) || sourceType.IsSubclassOfOrImplements(destinationTypeOrNull))
            {
                result = value;
                return true;
            }
            MethodInfo castMethod = destinationTypeOrNull.GetMethods(BindingFlags.Public | BindingFlags.Static).Append(sourceType.GetMethods(BindingFlags.Public | BindingFlags.Static))
                .FirstOrDefault(method =>
                    {
                        if ((method.Name == "op_Implicit" || method.Name == "op_Explicit") && method.ReturnType.IsInstanceOf(destinationTypeOrNull))
                        {
                            var parameters = method.GetParameters();
                            return parameters.Length == 1 && parameters[0].ParameterType == sourceType;
                        }
                        return false;
                    }
                );


            if (null != castMethod)
            {
                result = castMethod.Invoke(null, new object[] { value });
                return true;
            }
            else if (value is ICollection && destinationTypeOrNull.Implements(typeof(ICollection)))
            {
                Type nestedDestinationType = destinationTypeOrNull.GetGenericArguments()[0];
                result = Activator.CreateInstance(destinationTypeOrNull);
                var addMethod = result.GetType().GetMethod("Add", new Type[] { nestedDestinationType });
                foreach (object obj in ((ICollection)value))
                {
                    object destinationObject = ImplicitlyCastValueToType(obj, nestedDestinationType);
                    addMethod.Invoke(result, new object[] { destinationObject });
                }
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Get Field Or Property Value.
        /// </summary>
        /// <param name="obj">The Object.</param>
        /// <param name="member">The Member.</param>
        /// <returns>Object that contains field or property value.</returns>
        private static object GetFieldOrPropertyValue(object obj, MemberInfo member)
        {
            FieldInfo field = member as FieldInfo;
            object value;
            if (field != null)
            {
                value = field.GetValue(obj);
            }
            else
            {
                PropertyInfo property = member as PropertyInfo;
                Helper.CheckCondition<ParseException>(property != null, "Invalid member type {0}", member.MemberType);
                value = property.GetValue(obj, null);
            }

            return value;

        }

        /// <summary>
        /// Get Actual Parsing Field Or Property Type.
        /// </summary>
        /// <param name="member">The Member.</param>
        /// <returns>Type of Parsing field or property.</returns>
        private static Type GetActualParsingFieldOrPropertyType(MemberInfo member)
        {
            return member.GetParseTypeOrNull() ?? GetFieldOrPropertyType(member);
        }

        /// <summary>
        /// Get Field Or Property Type.
        /// </summary>
        /// <param name="memberInfo">Member info.</param>
        /// <returns>Type of field or property.</returns>
        private static Type GetFieldOrPropertyType(MemberInfo memberInfo)
        {
            FieldInfo field = memberInfo as FieldInfo;
            if (field != null)
            {
                return field.FieldType;
            }
            else
            {
                PropertyInfo property = memberInfo as PropertyInfo;
                Helper.CheckCondition<ParseException>(property != null, "Invalid member type {0}", memberInfo.MemberType);
                return property.PropertyType;
            }
        }

        // ref so that this works with structs
        /// <summary>
        /// Load argument.
        /// </summary>
        /// <param name="result">The result object.</param>
        /// <param name="member">The member.</param>
        /// <param name="isOptional">Is Optional.</param>
        /// <returns>True if the value was loaded from the ArgumentCollection. False otherwise (only can be false if isOption is true)</returns>
        private bool LoadArgument(ref object result, MemberInfo member, bool isOptional)
        {
            bool isField = member.MemberType == MemberTypes.Field;
            object defaultValue = GetFieldOrPropertyValue(result, member);
            Type parseTypeOrNull = member.GetParseTypeOrNull();

            defaultValue = ImplicitlyCastValueToType(defaultValue, parseTypeOrNull);

            MethodInfo argCollectionExtractOption = this.GetType().GetMethod(isOptional ? "ExtractOptionalInternal" : "ExtractAtInternal", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo genericExtractOption = argCollectionExtractOption.MakeGenericMethod(GetActualParsingFieldOrPropertyType(member));

            object[] args = isOptional ?
                new object[] { member.Name, defaultValue, true /*remove flag and value*/, member.GetDefaultParametersOrNull() } :
                new object[] { member.Name, 0 /* remove the next item*/, member.GetDefaultParametersOrNull(), true /* remove value */ };

            bool flagIsPresent = FindFlag(member.Name) >= 0;

            object newValue = null;
            try
            {
                newValue = genericExtractOption.Invoke(this, args);
            }
            catch (TargetInvocationException e)
            {
                Exception eToThrow = e;
                do
                {
                    eToThrow = eToThrow.InnerException;
                } while (eToThrow is TargetInvocationException && eToThrow.InnerException != null);

                throw eToThrow; //Should be a HelpException.
            }

            if (newValue != defaultValue)    // no point unless it's different. also, if null, could cause problems in some cases.
                SetFieldOrPropertyValue(ref result, member, newValue);

            return flagIsPresent || !isOptional;
        }

        /// <summary>
        /// Get Parsable Members.
        /// </summary>
        /// <param name="tType">Type of member.</param>
        /// <param name="optionals">Optional memberInfo.</param>
        /// <param name="requireds">List of required members.</param>
        /// <param name="constructingStrings">Constructing Strings Member.</param>
        /// <param name="requiredParams">Required Parameters.</param>
        private static void GetParsableMembers(Type tType, out List<MemberInfo> optionals, out List<MemberInfo> requireds, out List<MemberInfo> constructingStrings, out MemberInfo requiredParams)
        {
            optionals = new List<MemberInfo>();
            requireds = new List<MemberInfo>();
            constructingStrings = new List<MemberInfo>();
            requiredParams = null;
            Type[] typeInheritanceHierarchy = tType.GetInheritanceHierarchy();

            foreach (MemberInfo memInfo in tType.GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                ParseAttribute parseAttribute = memInfo.GetParseAttribute(typeInheritanceHierarchy);

                switch (parseAttribute.Action)
                {
                    case ParseAction.Optional:
                        optionals.Add(memInfo); break;
                    case ParseAction.Required:
                        requireds.Add(memInfo); break;
                    case ParseAction.ArgumentString:
                        constructingStrings.Add(memInfo);
                        Helper.CheckCondition<ParseException>(GetActualParsingFieldOrPropertyType(memInfo).Equals(typeof(string)), "Attribute [Parse({0})] must be set on a field or property of type string.", parseAttribute.Action);
                        break;
                    case ParseAction.Ignore:
                        break;
                    case ParseAction.Params:
                        Helper.CheckCondition<ParseException>(requiredParams == null, "Can only have one parameter of labeled as RequiredParams.");
                        requiredParams = memInfo;
                        break;
                    default:
                        throw new NotImplementedException("Forgot to implement action for " + parseAttribute.Action);

                }
            }
        }
        #endregion

        /// <summary>
        /// Populate From Parsable Object.
        /// </summary>
        /// <param name="obj">The Object.</param>
        /// <param name="suppressDefaults">Suppress Defaults.</param>
        protected void PopulateFromParsableObject(object obj, bool suppressDefaults = true)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            Type type = obj.GetType();
            type.IsConstructable().Enforce("object of type {0} is not parsable. Missing public default constructor.", type);

            List<MemberInfo> optionals, requireds, constructingStrings;
            MemberInfo requiredParams;
            GetParsableMembers(type, out optionals, out requireds, out constructingStrings, out requiredParams);

            string constructingStringOrNull;
            if (constructingStrings.Count() > 0 && null != (constructingStringOrNull = (string)GetFieldOrPropertyValue(obj, constructingStrings.First())))
            {
                constructingStrings.All(member => constructingStringOrNull.Equals(GetFieldOrPropertyValue(obj, member))).Enforce("For some reason this object has multiple constructing strings and they disagree with each other.");
                ParseString(constructingStringOrNull);
            }
            else
            {

                object defaultObj = null;
                int paramCount = optionals.Count + requireds.Count;
                foreach (var member in optionals)
                {
                    AddMemberToCollection(ref defaultObj, obj, member, isOptional: true, suppressDefaults: suppressDefaults, labelRequireds: paramCount > 1);
                }
                foreach (var member in requireds)
                {
                    AddMemberToCollection(ref defaultObj, obj, member, isOptional: false, suppressDefaults: suppressDefaults, labelRequireds: paramCount > 1);
                }
                if (requiredParams != null)
                {
                    AddRequiredParamsToCollection(obj, requiredParams, suppressDefaults);
                }
            }
        }

        /// <summary>
        /// Try Value As Collection String.
        /// </summary>
        /// <param name="value">The Value.</param>
        /// <param name="member">The Member.</param>
        /// <param name="suppressDefaults">Suppress Defaults.</param>
        /// <returns>True if value as collection string.</returns>
        private bool TryValueAsCollectionString(ref object value, MemberInfo member, bool suppressDefaults = true)
        {
            return TryValueAsCollectionString(ref value, GetFieldOrPropertyType(member), member.GetParseTypeOrNull(), suppressDefaults);
        }

        /// <summary>
        /// Try Value As Collection String.
        /// </summary>
        /// <param name="value">The Value.</param>
        /// <param name="baseType">Base type.</param>
        /// <param name="parseTypeOrNull">Parse Type Or Null.</param>
        /// <param name="suppressDefaults">Suppress Defaults.</param>
        /// <returns>True if value as collection string.</returns>
        private bool TryValueAsCollectionString(ref object value, Type baseType, Type parseTypeOrNull, bool suppressDefaults = true)
        {
            if (value != null)
            {
                Type valueType = value.GetType();
                if ((parseTypeOrNull ?? baseType).ParseAsCollection())// || (parseTypeOrNull == null && valueType.FindInterfaces(Module.FilterTypeNameIgnoreCase, "ICollection*").Length > 0))
                {
                    List<string> memberStrings = new List<string>();
                    foreach (object o in (IEnumerable)value)
                    {
                        string s = o.GetType().IsConstructable() ? ConstructorArguments.ToString(o, suppressDefaults) : o.ToString();
                        memberStrings.Add(s);
                    }
                    value = string.Format("{0}({1})", baseType.Equals(valueType) ? "" : value.GetType().ToTypeString(), memberStrings.StringJoin(","));
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Add Required Params To Collection.
        /// </summary>
        /// <param name="obj">The Object.</param>
        /// <param name="requiredParams">Required parameters.</param>
        /// <param name="suppressDefaults">Suppresses Defaults.</param>
        private void AddRequiredParamsToCollection(object obj, MemberInfo requiredParams, bool suppressDefaults = true)
        {
            object paramList = GetFieldOrPropertyValue(obj, requiredParams);
            if (paramList == null)  // e.g. constructing Help
                return;
            Type listType = paramList.GetType();
            Helper.CheckCondition<ParseException>(listType.ToTypeString().StartsWith("List<"), "The required params attribute must be placed on a member of type List<T>", listType.ToTypeString());
            Type genericType = listType.GetGenericArguments().Single();

            foreach (object item in (IEnumerable)paramList)
            {
                object valueToAdd = item;
                if (!genericType.HasParseMethod() && !TryValueAsCollectionString(ref valueToAdd, genericType, null, suppressDefaults) && genericType.IsConstructable())
                {
                    object valueAsParseType = ImplicitlyCastValueToType(valueToAdd, genericType);
                    ConstructorArguments constructor = ConstructorArguments.FromParsable(valueAsParseType, parseTypeOrNull: genericType, suppressDefaults: suppressDefaults);
                    valueToAdd = constructor.ToString();
                }
                Add(valueToAdd);
            }
        }

        /// <summary>
        /// Add Member To Collection.
        /// </summary>
        /// <param name="defaultObjOrNull">Default Object Or Null.</param>
        /// <param name="obj">The Object.</param>
        /// <param name="member">The Member.</param>
        /// <param name="isOptional">Is Optional.</param>
        /// <param name="suppressDefaults">Suppress Defaults.</param>
        /// <param name="labelRequireds">Required params Label.</param>
        private void AddMemberToCollection(ref object defaultObjOrNull, object obj, MemberInfo member, bool isOptional, bool suppressDefaults = true, bool labelRequireds = true)
        {
            string name = member.Name;
            object value = GetFieldOrPropertyValue(obj, member);
            Type parseType = GetActualParsingFieldOrPropertyType(member);

            if (defaultObjOrNull == null && isOptional && (value is bool || suppressDefaults))
                defaultObjOrNull = Activator.CreateInstance(obj.GetType());

            object defaultValue = defaultObjOrNull == null ? null : GetFieldOrPropertyValue(defaultObjOrNull, member);
            bool valueIsDefault = value == defaultValue || value != null && value.Equals(defaultValue) || defaultValue != null && defaultValue.Equals(value);
            bool needToWriteValue = !suppressDefaults || !isOptional || !valueIsDefault;

            if (needToWriteValue && !parseType.HasParseMethod() && !TryValueAsCollectionString(ref value, member, suppressDefaults) && value != null && parseType.IsConstructable() && value.GetType().IsConstructable())
            {
                object valueAsParseType = ImplicitlyCastValueToType(value, member.GetParseTypeOrNull());
                ConstructorArguments constructor = ConstructorArguments.FromParsable(valueAsParseType, parseTypeOrNull: parseType, suppressDefaults: suppressDefaults);
                if (string.IsNullOrEmpty(constructor.SubtypeName))
                    constructor.SubtypeName = parseType.ToTypeString();
                value = constructor.ToString();
            }

            
            TryValueAsCollectionString(ref defaultValue, member, suppressDefaults);
            if (needToWriteValue)
            {
                if (isOptional && value is bool && TreatOptionAsFlag(defaultObjOrNull, member))
                {
                    if ((bool)value)
                        AddOptionalFlag(name);
                }
                else if (!isOptional && !labelRequireds)
                {
                    Add(value);
                }
                else
                {
                    AddOptional(name, value);
                }
            }
        }

        /// <summary>
        /// Create Help Message.
        /// </summary>
        /// <param name="t">Type passed to create help message.</param>
        /// <param name="includeDateStamp">Include Date Stamp.</param>
        /// <returns>Created Help exception.</returns>
        public static HelpException CreateHelpMessage(Type t, bool includeDateStamp = true)
        {
            if (t == null)
                throw new ArgumentNullException("t");

            object defaultInstance = Activator.CreateInstance(t);
            ConstructorArguments args = new ConstructorArguments();
            return args.CreateHelpMessage(defaultInstance, includeDateStamp);
        }

        /// <summary>
        /// Create Help Message.
        /// </summary>
        /// <typeparam name="T">Help message type.</typeparam>
        /// <param name="includeDateStamp">Include Date Stamp</param>
        /// <returns>Created Help exception.</returns>
        public HelpException CreateHelpMessage<T>(bool includeDateStamp = true)
        {
            T result = CreateInstance<T>();
            return CreateHelpMessage(result, includeDateStamp);
        }

        /// <summary>
        ///  Create Help Message.
        /// </summary>
        /// <param name="defaultInstance">Default instance of object.</param>
        /// <param name="includeDateStamp">Include Date Stamp if set to true.</param>
        /// <returns>Created Help exception.</returns>
        private HelpException CreateHelpMessage(object defaultInstance, bool includeDateStamp)
        {
#if SILVERLIGHT
            throw new NotImplementedException("Silverlight doesn't have XDocument, so we don't support help.");
#else
            _argList.Clear();
            PopulateFromParsableObject(defaultInstance, suppressDefaults: false);

            Type type = defaultInstance.GetType();

            List<MemberInfo> optionals, requireds, constructingStrings;
            MemberInfo requiredParams;
            GetParsableMembers(type, out optionals, out requireds, out constructingStrings, out requiredParams);
            XDocument docFile = LoadXmlCodeDocumentationFile(type);

            StringBuilder helpMsg = new StringBuilder("Help for parsing type " + defaultInstance.GetType().ToTypeString());
            helpMsg.AppendFormat("<br><br>USAGE: " + CreateUsageString(requireds, requiredParams, type));
            helpMsg.Append("<br>Use help as the value for complex options for more info. Required arguments can be named like optionals.");
            helpMsg.Append("<br><br>" + GetXmlDocumentation(type, docFile));


            helpMsg.Append("<br><br>REQUIRED:");
            if (requireds.Count == 0)
            {
                helpMsg.Append(" [NONE]");
            }
            helpMsg.Append("<indent>");
            foreach (MemberInfo requirement in requireds)
            {
                helpMsg.Append("<br>" + CreateHelpMessage(defaultInstance, requirement, false));
                helpMsg.Append("<br><indent>" + GetXmlDocumentation(requirement, docFile) + "</indent><br>");
            }
            if (requiredParams != null)
            {
                helpMsg.Append("<br>" + CreateHelpMessage(defaultInstance, requiredParams, false));
                helpMsg.Append("<br><indent> PARAMS: Can specify arguments as a single list wrapped in () or as consecutive single arguments. Either way, these must be the last arguments. If there are more than one, none can be named.");
                helpMsg.Append("<br><indent>" + GetXmlDocumentation(requiredParams, docFile) + "</indent><br>");
            }
            helpMsg.Append("</indent>");
            helpMsg.Append("<br><br>OPTIONS:");
            if (optionals.Count == 0)
            {
                helpMsg.Append(" [NONE]");
            }
            helpMsg.Append("<indent>");
            foreach (MemberInfo option in optionals)
            {
                helpMsg.Append("<br>" + CreateHelpMessage(defaultInstance, option, true));
                helpMsg.Append("<br><indent>" + GetXmlDocumentation(option, docFile) + "</indent><br>");
            }
            helpMsg.Append("</indent>");

            return new HelpException(helpMsg.ToString(), includeDateStamp);
#endif
        }

#if !SILVERLIGHT

        /// <summary>
        /// Get Xml Documentation.
        /// </summary>
        /// <param name="type">Type of doc.</param>
        /// <param name="xmlDoc">Xml document.</param>
        /// <returns></returns>
        private static string GetXmlDocumentation(Type type, XDocument xmlDoc)
        {
            string xmlTagName = "T:" + type.FullName;
            return GetXmlDocumentation(xmlTagName, xmlDoc);
        }

        /// <summary>
        /// Get Xml Documentation.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="xmlDoc">Xml Doc.</param>
        /// <returns>Xml Documentation.</returns>
        private static string GetXmlDocumentation(MemberInfo member, XDocument xmlDoc)
        {
            FieldInfo field = member as FieldInfo;
            PropertyInfo property = member as PropertyInfo;

            string xmlTagName = field != null ?
                "F:" + field.DeclaringType.FullName + "." + field.Name :
                "P:" + property.DeclaringType.FullName + "." + property.Name;

            return GetXmlDocumentation(xmlTagName, xmlDoc);
        }

        /// <summary>
        /// Double New Line RegEx.
        /// </summary>
        static Regex _doubleNewLineRegEx = new Regex(@"\n[\s]*\n", RegexOptions.Compiled);

        /// <summary>
        /// Get Xml Documentation.
        /// </summary>
        /// <param name="xmlTagName">Xml Tag Name.</param>
        /// <param name="xmlDoc">Xml Doc.</param>
        /// <returns>Xml Documentation.</returns>
        private static string GetXmlDocumentation(string xmlTagName, XDocument xmlDoc)
        {
            if (null == xmlDoc)
            {
                return NO_DOCUMENTATION_STRING;
            }
            var xmlElements = xmlDoc.Elements("doc").Elements("members").Elements("member").Where(node => node.Attribute("name").Value == xmlTagName).ToList();

            if (xmlElements.Count > 0)
            {
                Helper.CheckCondition<ParseException>(xmlElements.Count == 1, "Problem with xml documentation file: there are {0} entries for type {1}", xmlElements.Count, xmlTagName);
                XElement summaryElement = xmlElements[0].Element("summary");

                if (null != summaryElement)
                {
                    string docText = summaryElement.Value.Trim();

                    docText = _doubleNewLineRegEx.Replace(docText, "<br><br>");
                    return docText;
                }
            }
            return NO_DOCUMENTATION_STRING;
        }

        /// <summary>
        /// Xml Document Cache.
        /// </summary>
        private static ConcurrentDictionary<Type, XDocument> xmlDocumentCache = new ConcurrentDictionary<Type, XDocument>();

        /// <summary>
        /// Load Xml Code Documentation File.
        /// </summary>
        /// <param name="type">Type of Doc.</param>
        /// <returns>Xml document.</returns>
        private static XDocument LoadXmlCodeDocumentationFile(Type type)
        {
            XDocument xmlDoc = xmlDocumentCache.GetOrAdd(type, (t) =>
            {
                string xmlFile = Path.ChangeExtension(t.Assembly.Location, "xml");
                if (!File.Exists(xmlFile))
                    return null;

                return XDocument.Load(xmlFile);
            });
            return xmlDoc;
        }

#endif

        /// <summary>
        /// Create Help Message.
        /// </summary>
        /// <param name="defaultInstance">Default Instance.</param>
        /// <param name="member">the Member.</param>
        /// <param name="isOption">Is Option flag.</param>
        /// <returns>Created Help Message.</returns>
        private string CreateHelpMessage(object defaultInstance, MemberInfo member, bool isOption)
        {
            string flag = CreateFlagString(member.Name);//isOption ? CreateFlagString(member.Name) : member.Name;
            object value = GetFieldOrPropertyValue(defaultInstance, member);
            Type memberType = GetActualParsingFieldOrPropertyType(member);
            string memberTypeString = memberType.ToTypeString();


            if (!TryValueAsCollectionString(ref value, member, suppressDefaults: true) && value != null)
                value = value.ToParseString(memberType, suppressDefaults: true);

            string helpMsg = isOption ?
                (this is CommandArguments && value is bool && !(bool)value ?    // is this a boolean flag for CommandArguments?
                    string.Format("{0} <BooleanFlag> [false if absent]", flag) :
                    string.Format("{0} <{1}> default: {2}", flag, memberTypeString, value == null ? "null" : value.ToString())) :
                string.Format("{0} <{1}>", flag, memberTypeString);

            return helpMsg;
        }

        /// <summary>
        /// Check For Help.
        /// </summary>
        /// <typeparam name="T">Type for check for help.</typeparam>
        /// <param name="value">Value for Check for help.</param>
        private static void CheckForHelp<T>(string value)
        {
            if (value != null && (value.Equals("help", StringComparison.CurrentCultureIgnoreCase) || value.Equals("help!", StringComparison.CurrentCultureIgnoreCase)))
            {
                Type type = typeof(T);

                HelpException help = GetHelpOnKnownSubtypes(type);
                throw help;
            }
        }

        /// <summary>
        /// Get Help On Known Subtypes.
        /// </summary>
        /// <param name="type">Type of Help exception.</param>
        /// <param name="includeDateStamp">Optional Include Date Stamp.</param>
        /// <returns>Help Exception.</returns>
        public static HelpException GetHelpOnKnownSubtypes(Type type, bool includeDateStamp = true)
        {
            
#if SILVERLIGHT
            throw new NotImplementedException("Silverlight doesn't have XDocument, so we don't support help.");
#else
            if (type == null)
                throw new ArgumentNullException("type");

            StringBuilder sb = new StringBuilder();

            XDocument xmlDoc = LoadXmlCodeDocumentationFile(type);
            string typeDocumentation = GetXmlDocumentation(type, xmlDoc).Replace(NO_DOCUMENTATION_STRING, "");
            if (type.IsEnum)
            {
                sb.AppendFormat("Enum type {0}: {1}", type.ToTypeString(), typeDocumentation);
                sb.Append("<br>OPTIONS:<br><indent>");
                foreach (var member in type.GetFields().Where(f => f.IsStatic))
                {
                    string docstring = GetXmlDocumentation(member, xmlDoc);
                    sb.Append(member.Name + "<br>");
                    if (docstring != NO_DOCUMENTATION_STRING)
                        sb.AppendFormat("<indent>{0}</indent><br>", docstring);
                }
                sb.Append("</indent>");
            }
            else if (type.Implements(typeof(ICollection)))
            {
                sb.Append("Help for type " + type.ToTypeString());
                sb.Append("<br><br>Collections can be specified using a comma delimited list, wrapped in parentheses.<br>");
                sb.Append("To get help on the nested type, using help! in the list. For example:");
                sb.Append("<br><indent>(help)");
                sb.Append("<br>(item1,item2,help)</indent>");

            }
            else
            {
                IEnumerable<Type> implementingTypes = null;
                if (type.IsInterface)
                {
                    implementingTypes = type.GetImplementingTypes().Where(t => !t.IsAbstract && !t.IsInterface && t.IsConstructable());
                    sb.AppendFormat("Interface {0}: {1}<br>", type.ToTypeString(), typeDocumentation);
                }
                else// if (type.IsAbstract)
                {
                    implementingTypes = type.GetDerivedTypes().Where(t => !t.IsAbstract && !t.IsInterface && t.IsConstructable());

                    if (type.IsAbstract)
                        sb.AppendFormat("Abstract class  {0}: {1}<br>", type.ToTypeString(), typeDocumentation);
                    else
                        implementingTypes = type.AsSingletonEnumerable().Concat(implementingTypes);
                }

                sb.AppendFormat("Types that implement {0}:<br>", type.ToTypeString());
                sb.Append("<indent>");
                foreach (var implementingType in implementingTypes.OrderBy(t => t.ToTypeString()))
                {
                    string docstring = GetXmlDocumentation(implementingType, LoadXmlCodeDocumentationFile(implementingType));
                    sb.Append(implementingType.ToTypeString() + "<br>");
                    if (docstring != NO_DOCUMENTATION_STRING)
                        sb.AppendFormat("<indent>{0}</indent><br>", docstring);
                }
                sb.Append("</indent>");
            }
            return new HelpException(sb.ToString(), includeDateStamp);
#endif
        }

        /// <summary>
        /// Enumerate Values Of Type From Parsable.
        /// </summary>
        /// <typeparam name="T">Type of Parsable.</typeparam>
        /// <param name="values">The Values.</param>
        /// <returns>List of types.</returns>
        public static IEnumerable<T> EnumerateValuesOfTypeFromParsable<T>(object values)
        {
            if (values == null)
                return null;
            return EnumerateValuesOfTypeFromParsable<T>(values, values.GetType());
        }

        /// <summary>
        /// Enumerate Values Of Type From Parsable.
        /// </summary>
        /// <typeparam name="T">Type of Parsable.</typeparam>
        /// <param name="values">The Values.</param>
        /// <param name="parseTypeOfObj">parse Type Of Obj.</param>
        /// <returns>List of types.</returns>
        private static IEnumerable<T> EnumerateValuesOfTypeFromParsable<T>(object values, Type parseTypeOfObj)
        {
            Type targetType = typeof(T);
            if (targetType.IsInstanceOfType(values))
            {
                yield return (T)values;
            }

            parseTypeOfObj.IsConstructable().Enforce("object of type {0} is not parsable.", parseTypeOfObj);

            Type[] typeInheritanceHierarchy = parseTypeOfObj.GetInheritanceHierarchy();

            var fields = parseTypeOfObj.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Cast<MemberInfo>();
            var properties = parseTypeOfObj.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Where(prop => prop.GetIndexParameters().Length == 0).Cast<MemberInfo>();

            foreach (MemberInfo member in fields.Append(properties))
            {
                var parseAction = member.GetParseAttribute(typeInheritanceHierarchy).Action;
                Type parseType = GetActualParsingFieldOrPropertyType(member);
                if (parseAction == ParseAction.Ignore && !parseType.IsInstanceOf(targetType) && !parseType.IsCollection<T>())
                {
                    continue;
                }

                object value = GetFieldOrPropertyValue(values, member);
                if (value == null) continue;

                object valueAsParseType;
                if (!TryImplicitlyCastValueToType(value, parseType, out valueAsParseType)) continue;

                if (parseType.IsCollection<T>())
                {
                    foreach (T enumerableResult in (IEnumerable<T>)valueAsParseType)
                    {
                        yield return enumerableResult;
                    }
                }

                // a collection may itself have an argument of the desired type. 
                // !!! note though that this will enumerate duplicates in some rare cases.
                // only recurse on parsable members.
                if (parseAction != ParseAction.Ignore && parseType.IsConstructable() && !parseType.HasParseMethod())
                {
                    foreach (T nestedResult in EnumerateValuesOfTypeFromParsable<T>(valueAsParseType))
                        yield return nestedResult;
                }
                else if (targetType.IsInstanceOfType(valueAsParseType)) // the recursive call will enumerate it. Only enumerate if we don't go into the recursive call.
                    yield return (T)valueAsParseType;
            }
        }
    }

}
