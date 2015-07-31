using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
    
namespace Bio.Util.ArgumentParser
{
    /// <summary>
    /// Constructor arguments take the form:
    /// <code>
    /// (option1:value,option2:value,...,req1,req2...)
    /// </code>
    /// Optionally, the opening paren can be preceded by a class name. For example:
    /// <code>
    /// LogisticRegression(option1:value,option2:value,...,req1,req2...)
    /// </code>
    /// This class name specifies what will be constructed when the Construct() method is called. Note that flags are treated
    /// exactly like options. So a boolean flag called verbose would be set using verbose:true. Note also that required
    /// arguments can be named, in which case they can be in any order.
    /// </summary>
    public class ConstructorArguments : ArgumentCollection
    {
        /// <summary>
        /// Argument Delimiter.
        /// </summary>
        public const char ArgumentDelimiter = ',';

        /// <summary>
        /// Constructor.
        /// </summary>
        public ConstructorArguments() : base(new string[0]) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="args">The Arguments.</param>
        public ConstructorArguments(string args) : base(args) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="args">The Arguments.</param>
        public ConstructorArguments(IEnumerable<string> args) : base(args) { }


        /// <summary>
        /// Constructs and instance of T, then runs it. This convenience method creates an instance of CommandArguments, then
        /// call ConstructAndRun on that result.
        /// </summary>
        /// <typeparam name="T">A //[Parsable] type that implements IExecutable.</typeparam>
        /// <param name="commandArguments">Constructor arguments</param>
        public static void ConstructAndRun<T>(string[] commandArguments) where T : IRunnable
        {
            if (commandArguments == null)
                ConstructAndRun<T>("help!");
            else
            {
                Helper.CheckCondition<ArgumentException>(commandArguments.Length <= 1, "Can't parse an array with more than one element into an instance of ConstructorArguments");
                ConstructAndRun<T>(commandArguments.Length == 0 ? "help!" : commandArguments[0]);
            }
        }

        /// <summary>
        /// Constructs and instance of T, then runs it. This convenience method creates an instance of CommandArguments, then
        /// call ConstructAndRun on that result.
        /// </summary>
        /// <typeparam name="T">A //[Parsable] type that implements IExecutable.</typeparam>
        /// <param name="constructorString">Constructor arguments</param>
        public static void ConstructAndRun<T>(string constructorString) where T : IRunnable
        {
            ConstructorArguments command = new ConstructorArguments(constructorString);
            command.ConstructAndRun<T>();
        }

        /// <summary>
        /// Simple wrapper that constructs an instance of type T from the command string. 
        /// See ArgumentCollection.Construct() for documentation.
        /// </summary>
        /// <typeparam name="T">The Parsable type to be constructed</typeparam>
        /// <param name="commandString">The string from which to construct</param>
        /// <returns>The fully instantiated object</returns>
        public static T Construct<T>(string commandString)
        {
            ConstructorArguments command = new ConstructorArguments(commandString);
            return command.Construct<T>();
        }

        /// <summary>
        /// Constructs and instance of ConstructorArguments from a parsable object. This is the inverse of Construct().
        /// Note: will always set SubtypeName to the Type of object.
        /// </summary>
        /// <param name="obj">The object from which to construct the ConstructorArguments</param>
        /// <param name="parseTypeOrNull">parseTypeOrNull</param>
        /// <param name="suppressDefaults">Specifies whether values that are equal to the defaults should be included in the resulting ArgumentCollection</param>
        /// <returns>The result</returns>
        public static ConstructorArguments FromParsable(object obj, Type parseTypeOrNull = null, bool suppressDefaults = true)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            ConstructorArguments constructor = new ConstructorArguments();
            constructor.SubtypeName = parseTypeOrNull == null || !parseTypeOrNull.Equals(obj.GetType()) ? obj.GetType().ToTypeString() : null;
            constructor.PopulateFromParsableObject(obj, suppressDefaults);
            return constructor;
        }

        /// <summary>
        /// Shortcut for ConstructorArguments.FromParsable(obj).ToString().  Note that Construct(ToString(obj)) == obj.
        /// </summary>
        /// <param name="parsableObject">An obejct with the //[Parsable] attribute.</param>
        /// <param name="suppressDefaults">Specifies whether values that are equal to the defaults should be included in the resulting ArgumentCollection</param>
        /// <returns>A Constructor string that could be used to reconstruct parsableObject.</returns>
        public static string ToString(object parsableObject, bool suppressDefaults = false)
        {
            return FromParsable(parsableObject, parseTypeOrNull: null, suppressDefaults: suppressDefaults).ToString();
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
            string typeName = SubtypeName ?? constructingType.ToTypeString();
            string baseString = string.Format("{0}([OPTIONS],{1})\n{2}", typeName, requireds.Select(member => member.Name).StringJoin(","), "\nOptions are specifed using command-delimited name:value pairs.");
            if (null != requiredParamsOrNull)
            {
                string opName = requiredParamsOrNull.Name.EndsWith("s", StringComparison.CurrentCultureIgnoreCase) ?
                    requiredParamsOrNull.Name.Substring(0, requiredParamsOrNull.Name.Length - 1) :
                    requiredParamsOrNull.Name;

                baseString += string.Format(" {0}_1[,{0}_2,...]", opName);
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
            if (lineToParse.EndsWith(")") && !lineToParse.StartsWith("("))
            {
                int idx = lineToParse.IndexOf('(');
                Helper.CheckCondition<ParseException>(idx >= 0, string.Format(CultureInfo.CurrentCulture, Properties.Resource.UnbalancedParanthesis));
                if (idx > 0)
                {
                    string result = lineToParse.Substring(0, idx);
                    lineToParse = lineToParse.Substring(idx);
                    return result;
                }
            }
            else if (!lineToParse.Contains('('))
            {
                string result = lineToParse;
                lineToParse = string.Empty;
                return result;
            }
            return null;
        }

        /// <summary>
        /// Create Argument List.
        /// </summary>
        /// <param name="constructorArgsAsString">Constructor Args As String.</param>
        /// <returns>Created Argument List.</returns>
        protected override IEnumerable<string> CreateArgList(string constructorArgsAsString)
        {
            var argList = new List<string>();
            if (string.IsNullOrEmpty(constructorArgsAsString))
                return argList;

            if (constructorArgsAsString.StartsWith("(") && constructorArgsAsString.EndsWith(")"))
                constructorArgsAsString = constructorArgsAsString.Substring(1, constructorArgsAsString.Length - 2);
            //Split on ',' that are not inside '('...')'.
//            var argList = new List<string>();
            int depth = 0;
            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < constructorArgsAsString.Length; ++index)
            {
                char c = constructorArgsAsString[index];
                switch (c)
                {
                    case '(':
                        ++depth;
                        sb.Append(c);
                        break;
                    case ')':
                        --depth;
                        Helper.CheckCondition<ParseException>(depth >= 0, "Expect parens to be balanced, but they are not in: " + constructorArgsAsString);
                        sb.Append(c);
                        break;
                    case ':':
                    case ArgumentDelimiter:
                        if (depth == 0)
                        {
                            if (c == ':') // keep the comma so we know it's a flag.
                                sb.Append(c);

                            //If we hit a comma at the top-level, even if SB is empty to add it to argList.
                            argList.Add(sb.ToString());
                            sb = new StringBuilder();
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            Helper.CheckCondition<ParseException>(depth == 0, "Expect parens to be balanced, but they are not in: " + constructorArgsAsString);

            //After we reach the end, we add the contents of SB to the argList only if it is not empty.
            if (sb.Length > 0)
            {
                argList.Add(sb.ToString());
            }

            return argList;
        }

        /// <summary>
        /// Add Optional Flag.
        /// </summary>
        /// <param name="argumentName">Argument Name.</param>
        public override void AddOptionalFlag(string argumentName)
        {
            AddOptional(argumentName, true);
        }

        /// <summary>
        /// Create Flag String.
        /// </summary>
        /// <param name="flagBase">The flag Base.</param>
        /// <returns>Created Flag String.</returns>
        protected override string CreateFlagString(string flagBase)
        {
            return IsFlag(flagBase) ? flagBase : flagBase + ":";
        }

        /// <summary>
        /// Matches Flag.
        /// </summary>
        /// <param name="query">The Query.</param>
        /// <param name="flagBase">The Flag base.</param>
        /// <returns>True if flag matches.</returns>
        public override bool MatchesFlag(string query, string flagBase)
        {
            if (string.IsNullOrEmpty(query))
                return false;
            return query.Equals(CreateFlagString(flagBase), StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Is Flag.
        /// </summary>
        /// <param name="query">The Query.</param>
        /// <returns>True if flag is set.</returns>
        public override bool IsFlag(string query)
        {
            if (string.IsNullOrEmpty(query))
                return false;
            return query.EndsWith(":");
        }

        /// <summary>
        /// Extract Optional Flag Internal.
        /// </summary>
        /// <param name="flag">The Flag.</param>
        /// <param name="removeFlag">Remove flag or Not.</param>
        /// <returns>True if Extract Optional Flag Internal.</returns>
        protected override bool ExtractOptionalFlagInternal(string flag, bool removeFlag)
        {
            return ExtractOptional<bool>(flag, false);
        }

        /// <summary>
        /// To String for Argument list.
        /// </summary>
        /// <returns>String form of Argument list.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            string[] args = GetUnderlyingArray();
            for (int i = 0; i < args.Length; i++)
            {
                sb.Append(args[i]);
                if (!IsFlag(args[i]) && i < args.Length - 1)
                    sb.Append(',');
            }
            bool hasMultArgs = args.Length > 1;//sb.ToString().Contains(',');
            bool hasSubtypeName = !string.IsNullOrEmpty(SubtypeName) ;
            bool includeParens = hasMultArgs ||hasSubtypeName && sb.Length>0;

            string result = string.Format("{0}{1}{2}{3}", SubtypeName ?? "", includeParens ? "(" : "", sb.ToString(), includeParens ? ")" : "");
            return string.IsNullOrEmpty(result) ? "()" : result;
        }
    }
}
