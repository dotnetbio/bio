using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Bio.Util.ArgumentParser
{
    /// <summary>
    /// Command line arguments take the form:
    /// <code>
    /// -flag 1 ... -option1 value -option2 value ... required1 required 2
    /// </code>
    /// Note that the presence of the flag indicates that the optional value is true.
    /// Note also that required arguments can be named, in which case they can be in any order.
    /// </summary>
    public class CommandArguments : ArgumentCollection
    {
        //!!! This one doesn't seem good with .NET 4 "\u00AD"
        private static string[] FLAG_PREFIXES = new string[] { "/", /*a windows dash*/ "\xFB", /*every unicode hyphen*/ "\u002D", "\u2010", "\u2011", "\u2012", "\u2013", "\u2014", "\u2015", "\u2212" };

        /// <summary>
        /// Constructor.
        /// </summary>
        public CommandArguments()
            : base(new string[0])
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="args"></param>
        public CommandArguments(string args)
            : base(args)
        {
            if (Count == 0) AddOptionalFlag("help");
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="args"></param>
        public CommandArguments(IEnumerable<string> args)
            : base(args)
        {
            if (Count == 0) AddOptionalFlag("help");
        }

        /// <summary>
        /// Constructs and instance of T, then runs it. This convenience method creates an instance of CommandArguments, then
        /// call ConstructAndRun on that result.
        /// </summary>
        /// <typeparam name="T">A //[Parsable] type that implements IExecutable.</typeparam>
        /// <param name="commandArgs">Command line arguments.</param>
        public static void ConstructAndRun<T>(string[] commandArgs) where T : IRunnable
        {
            ConstructAndRun<T>(commandArgs, true);
        }

        /// <summary>
        /// Constructs and instance of T, then runs it. This convenience method creates an instance of CommandArguments, then
        /// call ConstructAndRun on that result.
        /// </summary>
        /// <typeparam name="T">A //[Parsable] type that implements IExecutable.</typeparam>
        /// <param name="commandArgs">Command line arguments.</param>
        /// <param name="generateHelpPage">True/False whether to generate help or throw HelpException</param>
        public static void ConstructAndRun<T>(string[] commandArgs, bool generateHelpPage) where T : IRunnable
        {
            CommandArguments command = new CommandArguments(commandArgs);
            command.GenerateHelpPage = generateHelpPage;
            command.ConstructAndRun<T>();
        }

        /// <summary>
        /// Simple wrapper that constructs an instance of type T from the command line array. 
        /// See ArgumentCollection.Construct() for documentation.
        /// </summary>
        /// <typeparam name="T">The Parsable type to be constructed</typeparam>
        /// <param name="commandArgs">The string array from which to construct</param>
        /// <returns>The fully instantiated object</returns>
        public static T Construct<T>(string[] commandArgs)
        {
            CommandArguments command = new CommandArguments(commandArgs);
            return command.Construct<T>();
        }

        /// <summary>
        /// Simple wrapper that constructs an instance of type T from the command line string. 
        /// See ArgumentCollection.Construct() for documentation.
        /// </summary>
        /// <typeparam name="T">The Parsable type to be constructed</typeparam>
        /// <param name="commandString">The string from which to construct</param>
        /// <returns>The fully instantiated object</returns>
        public static T Construct<T>(string commandString)
        {
            CommandArguments command = new CommandArguments(commandString);
            return command.Construct<T>();
        }

        /// <summary>
        /// Constructs and instance of CommandArguments from a parsable object. This is the inverse of Construct().
        /// Will include all default arguments.
        /// </summary>
        /// <param name="obj">The object from which to construct the ConstructorArguments</param>
        /// <returns>The result</returns>
        public static CommandArguments FromParsable(object obj)
        {
            return FromParsable(obj, false);
        }

        /// <summary>
        /// Constructs and instance of CommandArguments from a parsable object. This is the inverse of Construct().
        /// </summary>
        /// <param name="obj">The object from which to construct the ConstructorArguments</param>
        /// <param name="suppressDefaults">Specifies whether values that are equal to the defaults should be included in the resulting ArgumentCollection</param>
        /// <returns>The result</returns>
        public static CommandArguments FromParsable(object obj, bool suppressDefaults = true)
        {
            CommandArguments cmd = new CommandArguments();
            cmd.PopulateFromParsableObject(obj, suppressDefaults);
            return cmd;
        }

        /// <summary>
        /// Shortcut for CommandArguments.FromParsable(obj).ToString().  Note that Construct(ToString(obj)) == obj.
        /// </summary>
        /// <param name="parsableObject">An obejct with the //[Parsable] attribute.</param>
        /// <param name="suppressDefaults">Specifies whether values that are equal to the defaults should be included in the resulting ArgumentCollection</param>
        /// <param name="protectWithQuotes">protectWithQuotes</param>
        /// <returns>A Command string that could be used to reconstruct parsableObject.</returns>
        public static string ToString(object parsableObject, bool suppressDefaults = true, bool protectWithQuotes = true)
        {
            return FromParsable(parsableObject, suppressDefaults).ToString(protectWithQuotes);
        }

        /// <summary>
        /// Create Usage String.
        /// </summary>
        /// <param name="requireds">Required members.</param>
        /// <param name="requiredParamsOrNull">Required Params Or Null.</param>
        /// <param name="constructingType">Constructing Type.</param>
        /// <returns>Created Usage String.</returns>
        protected override string CreateUsageString(IEnumerable<System.Reflection.MemberInfo> requireds, System.Reflection.MemberInfo requiredParamsOrNull, Type constructingType)
        {
            string exeName = Path.GetFileName(Assembly.
#if !SILVERLIGHT
                GetEntryAssembly().Location);
#else
                GetExecutingAssembly().Location);
#endif
            string baseString = string.Format("{0} [OPTIONS] {1}", exeName, requireds.Select(member => member.Name).StringJoin(" "));
            if (null != requiredParamsOrNull)
            {
                string opName = requiredParamsOrNull.Name.EndsWith("s", StringComparison.CurrentCultureIgnoreCase) ?
                    requiredParamsOrNull.Name.Substring(0, requiredParamsOrNull.Name.Length - 1) :
                    requiredParamsOrNull.Name;

                baseString += string.Format(" {0}_1[ {0}_2 ...]", opName);
            }
            return baseString;
        }

        /// <summary>
        /// Extract Subtype Name.
        /// </summary>
        /// <param name="lineToParse">Line To Parse.</param>
        /// <returns>Extracted Subtype Name.</returns>
        protected override string ExtractSubtypeName(ref string lineToParse)
        {
            return null;
        }

        /// <summary>
        /// Create Argument List.
        /// </summary>
        /// <param name="lineToParse">Line To Parse.</param>
        /// <returns>Created Argument List.</returns>
        protected override IEnumerable<string> CreateArgList(string lineToParse)
        {
            if (string.IsNullOrEmpty(lineToParse))
                return null;

            return lineToParse.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Extract Optional Flag Internal.
        /// </summary>
        /// <param name="flag">The Flag.</param>
        /// <param name="removeFlag">Remove flag or Not.</param>
        /// <returns>True if extracted Optional Flag Internal.</returns>
        protected override bool ExtractOptionalFlagInternal(string flag, bool removeFlag)
        {
            int argIndex = FindFlag(flag);

            if (argIndex == -1)
            {
                return false;
            }

            if (removeFlag)
                RemoveAt(argIndex);

            return true;
        }

        /// <summary>
        /// Matches Flag.
        /// </summary>
        /// <param name="query">The Query.</param>
        /// <param name="flagBase">The Flag Base.</param>
        /// <returns>True if Matches Flag.</returns>
        public override bool MatchesFlag(string query, string flagBase)
        {
            if (string.IsNullOrEmpty(query))
                return false;

            foreach (var pre in FLAG_PREFIXES)
                if (query.Equals(pre + flagBase, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        /// <summary>
        /// Is Flag.
        /// </summary>
        /// <param name="query">The Query.</param>
        /// <returns>True if flag set.</returns>
        public override bool IsFlag(string query)
        {
            if (string.IsNullOrEmpty(query))
                return false;

            if (query.Length < 2)
                return false;

            double dummy;
            if (string.IsNullOrWhiteSpace(query) || double.TryParse(query, out dummy))
                return false;

            foreach (var pre in FLAG_PREFIXES)
                if (query.StartsWith(pre, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        /// <summary>
        /// Create Flag String.
        /// </summary>
        /// <param name="flagBase">The Flag Base.</param>
        /// <returns>Created Flag String.</returns>
        protected override string CreateFlagString(string flagBase)
        {
            return IsFlag(flagBase) ? flagBase : "-" + flagBase;
        }

        /// <summary>
        /// Add Optional Flag.
        /// </summary>
        /// <param name="argumentName">The argumentName.</param>
        public override void AddOptionalFlag(string argumentName)
        {
            Add(CreateFlagString(argumentName));
        }

        /// <summary>
        /// To String for Argument list.
        /// </summary>
        /// <param name="protectWithQuotes">Protect With Quotes optional.</param>
        /// <returns>String form of Argument list. </returns>
        public string ToString(bool protectWithQuotes = true)
        {
            if (protectWithQuotes)
                return GetUnderlyingArray().Select(s => "\"" + s + "\"").StringJoin(" ");
            else
                return GetUnderlyingArray().StringJoin(" ");
        }
    }
}
