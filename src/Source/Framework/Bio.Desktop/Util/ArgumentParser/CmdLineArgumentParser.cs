namespace Bio.Util.ArgumentParser
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Reflection;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Command Line Argument value types.
    /// </summary>
    public enum ArgumentValueType
    {
        /// <summary>
        /// String type argument.
        /// </summary>
        String,

        /// <summary>
        /// String type argument.
        /// </summary>
        OptionalString,

        /// <summary>
        /// Integer type argument.
        /// </summary>
        Int,

        /// <summary>
        /// Integer type data. Used for optional parameter.
        /// </summary>
        OptionalInt,
        
        /// <summary>
        /// Boolean type argument.
        /// </summary>
        Bool,

        /// <summary>
        /// Inidicates that integer value may be specified more than once.
        /// Only valid if the argument is a collection.
        /// </summary>
        MultipleInts,

        /// <summary>
        /// Inidicates that string value may be specified more than once.
        /// If duplicate values are found an exception is raised.
        /// </summary>
        MultipleUniqueStrings
    }

    /// <summary>
    /// Used to control parsing of command line arguments. 
    /// </summary>
    public enum ArgumentType
    {
        /// <summary>
        /// Indicates that argument is not required.
        /// </summary>
        Optional,

        /// <summary>
        /// Indicates that the argument is mandatory.
        /// </summary>
        Required,

        /// <summary>
        /// Indicates the arugment is the default. If the parameter name is not found this param
        /// </summary>
        DefaultArgument
    }

    /// <summary>
    /// This class parses all the command line arguments.
    /// </summary>
    public class CommandLineArguments : IEnumerable, IEnumerator
    {
        /// <summary>
        /// All the parameters and values passed from commandline are stored after parsing.
        /// </summary>
        private StringDictionary parsedArguments;

        /// <summary>
        /// The Command Line enumerator.
        /// </summary>
        private IEnumerator enumerator;

        /// <summary>
        /// Contains the mapping between parameter name and the alias (shortName).
        /// </summary>
        private Dictionary<string, string> paramNameAliasMap = new Dictionary<string, string>();

        /// <summary>
        /// The target object to which the command line arguments are to be set.
        /// </summary>
        private object targetObject;

        /// <summary>
        /// All the required and optional parameters from commandline are stored.
        /// </summary>
        private SortedDictionary<string, Argument> argumentList;

        /// <summary>
        /// Initializes a new instance of the CommandLineArguments class.
        /// </summary>
        public CommandLineArguments()
        {
            this.parsedArguments = new StringDictionary();
            this.argumentList = new SortedDictionary<string, Argument>(StringComparer.OrdinalIgnoreCase);
            this.enumerator = this.parsedArguments.GetEnumerator();

            this.ArgumentSeparator = "-";
            this.AllowAdditionalArguments = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether additional arguments are reqiured or not.
        /// </summary>
        public bool AllowAdditionalArguments { get; set; }

        /// <summary>
        /// Gets or sets the argument separator character.
        /// </summary>
        public string ArgumentSeparator { get; set; }

        /// <summary>
        /// Gets the current found parameter from enumerator.
        /// </summary>
        public DictionaryEntry Current
        {
            get
            {
                return ((DictionaryEntry)this.enumerator.Current);
            }
        }

        /// <summary>
        /// Gets current value from enumerator.
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                return this.enumerator.Current;
            }
        }

        /// <summary>
        /// Defines the argument that the commandline utility supports.
        /// </summary>
        /// <param name="argType">Argument type.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="argValueType">Defines the argument value type (int or string or bool) for the argument.</param>
        /// <param name="shortName">ShortName or alias for the argument.</param>
        /// <param name="helpDesc">Description of the argument.</param>
        public void Parameter(ArgumentType argType, string parameterName, ArgumentValueType argValueType, string shortName, string helpDesc)
        {
            // For the first value without parameter name only type string is accepted.
            if (string.IsNullOrEmpty(parameterName) && argValueType != ArgumentValueType.String)
            {
                throw new Exception("For the first value (without parameter name) only type ValueType.String is accepted! ");
            }

            Argument param = new Argument(parameterName, argType, argValueType, shortName, helpDesc);
            this.argumentList.Add(param.Name, param);
            if (!string.IsNullOrEmpty(param.ShortName))
            {
                this.paramNameAliasMap.Add(shortName, parameterName);
            }
        }

        /// <summary>
        /// Parses the command line arguments passed from the utility.
        /// </summary>
        /// <param name="arguments">Arguments to be parsed.</param>
        /// <param name="destination">Resulting parsed arguments.</param>
        public void Parse(string[] arguments, object destination)
        {
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }

            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            this.targetObject = destination;
            string defaultArgs = string.Empty;
            string defaultArgValues = string.Empty;
            
            // get the default parameter name
            foreach (KeyValuePair<string, Argument> arg in this.argumentList)
            {
                if (arg.Value.AllowType == ArgumentType.DefaultArgument)
                {
                    defaultArgs = string.Concat(this.ArgumentSeparator, arg.Value.Name, "=");
                    break;
                }
            }

            string args = string.Empty;
            foreach (string s in arguments)
            {
                string val;
                if (s.StartsWith(this.ArgumentSeparator, StringComparison.OrdinalIgnoreCase) )
                {
                    int endIndex = s.IndexOfAny(new char[] { ':', '=' }, 1);
                    string parameter = s.Substring(1, endIndex == -1 ? s.Length - 1 : endIndex - 1);
                    string paramVal;
                    if (parameter.Length + 1 == s.Length)
                    {
                        paramVal = null;
                    }
                    else if (s.Length > 1 + parameter.Length && (s[1 + parameter.Length] == ':' || s[1 + parameter.Length] == '='))
                    {
                        paramVal = s.Substring(parameter.Length + 2);
                    }
                    else
                    {
                        paramVal = s.Substring(parameter.Length + 1);
                    }

                    val = this.ArgumentSeparator + parameter;
                    string encodedValue = EncodeValue(paramVal);
                    if (!string.IsNullOrEmpty(encodedValue))
                    {
                        val = string.Concat(val, "=", encodedValue);
                    }

                    args += val + " ";
                }
                else 
                {
                    if (!string.IsNullOrEmpty(defaultArgs))
                    {
                        defaultArgValues = string.Concat(defaultArgValues, EncodeValue(s), " ");
                    }
                    else
                    {
                        throw new ArgumentParserException(string.Format(CultureInfo.CurrentCulture, "Could not associate any parameter for the value {0}", s));
                    }
                }
            }

            if (!string.IsNullOrEmpty(defaultArgValues) && !string.IsNullOrEmpty(defaultArgs))
            {
                args = string.Concat(args, defaultArgs, defaultArgValues, " ");
            }

            // parse the arguments
            this.Parse(args);

            // assign the parsed values to the target object properties.
            this.AssignTargetObjectProperties();
        }
        
        /// <summary>
        /// Returns a enumerator which walks through the dictionary of found parameters.
        /// </summary>
        /// <returns>Enumerator of dictionary of found parameters.</returns>
        public IEnumerator GetEnumerator()
        {
            this.enumerator = this.parsedArguments.GetEnumerator();
            return this.enumerator;
        }

        /// <summary>
        /// Sets the enumerator to the next found parameter.
        /// </summary>
        /// <returns>true if there is a next found parameter, else false.</returns>
        public bool MoveNext()
        {
            return this.enumerator.MoveNext();
        }
       
        /// <summary>
        /// Resets the enumerator to the initial position in front of the first found parameter.
        /// </summary>
        public void Reset()
        {
            this.enumerator.Reset();
        }

        /// <summary>
        /// Returns the position of the mismatch of arguments.
        /// </summary>
        /// <param name="regExpr">Regular expression used for matching.</param>
        /// <param name="srchExpr">Expression to be searched.</param>
        /// <returns>Returns the character position where there is a mismatch.</returns>
        private static int GetMismatchPosition(string regExpr, string srchExpr)
        {
            // split the regular expression using opening parenthesis and respective closing parenthesis.
            SortedDictionary<int, int> validateParenthesis = new SortedDictionary<int, int>();
            Stack<int> openParenthesis = new Stack<int>();
            try
            {
                for (int i = 0; i < regExpr.Length; i++)
                {
                    if (regExpr[i] == '(')
                    {
                        // Make sure that this ( is not escaped!
                        if (!((i == 1 && regExpr[i - 1] == '\\') ||
                               (i > 1 && regExpr[i - 1] == '\\' && regExpr[i - 2] != '\\')))
                        {
                            openParenthesis.Push(i);
                        }
                    }
                    else if (regExpr[i] == ')')
                    {
                        // Make sure that this ) is not escaped!
                        if (!((i == 1 && regExpr[i - 1] == '\\') ||
                               (i > 1 && regExpr[i - 1] == '\\' && regExpr[i - 2] != '\\')))
                        {
                            int pop = openParenthesis.Pop();
                            validateParenthesis.Add(pop, i);
                        }
                    }
                }

                // In a ideal situation this should not happen.
                if (openParenthesis.Count != 0)
                {
                    throw new Exception("Error in regular expression, parenthesis are not balanced");
                }
            }
            catch (Exception)
            {
                // since RegEx should be valid, this can never happen.
                throw new Exception("Internal Exception: Parentesis not balanced!");
            }

            // Parenthesis contains all parenthesis matches ordered by the position of the opening parenthesis
            IEnumerator e = validateParenthesis.GetEnumerator();
            int prevCorrectPosition = 0;
            while (e.MoveNext())
            {
                KeyValuePair<int, int> c = (KeyValuePair<int, int>)e.Current;

                // Get sub-regular-expression of parenthesis grouping.
                string subRegEx = regExpr.Substring(c.Key, c.Value - c.Key + 1);
                Regex sub = null;
                try
                {
                    sub = new Regex(subRegEx);
                }
                catch (Exception)
                {
                    // This should never happen since subexpression of a valid regex should still be valid.
                    throw new Exception("Internal Exception: SubRegEx invalid: " + subRegEx.ToString());
                }

                Match m = sub.Match(srchExpr);
                if (m.Success == true)
                {
                    // If there is a match this subexpression matches the SearchStr and the mismatch must
                    // follow afterwards.
                    // find the end position of the match and increase LastCorrectPosition count to that position.
                    // (warning: here the wrong match might be detected,
                    // but since its is unlikely that commandline argument contains several identical parts,
                    // this potential problem is ignored.)
                    int newLastCorrectPosition = srchExpr.IndexOf(m.Value, StringComparison.OrdinalIgnoreCase) + m.Value.Length;
                    if (newLastCorrectPosition > prevCorrectPosition)
                    {
                        prevCorrectPosition = newLastCorrectPosition;
                    }
                }
            }

            return prevCorrectPosition;
        }

        /// <summary>
        /// Encode Value.
        /// </summary>
        /// <param name="value">The Value.</param>
        /// <returns>The Encoded Value.</returns>
        private static string EncodeValue(string value)
        {
            string encodedVal = value;
            if (string.IsNullOrEmpty(value))
            {
                encodedVal = string.Empty;
            }
            else
            {
                if (value.StartsWith("-", StringComparison.OrdinalIgnoreCase))
                {
                    encodedVal = "\"" + value + "\"";
                }
                else if (value.Contains("-"))
                {
                    encodedVal = value.Replace("-", ">");
                }
                else
                {
                    // if the parmeter value has space encode it with "|" and decode it while setting the value
                    encodedVal = value.Trim().Replace(" ", "|");
                }
            }

            return encodedVal;
        }

        /// <summary>
        /// Parse Value.
        /// </summary>
        /// <param name="type">The Type of value.</param>
        /// <param name="stringData">String Data.</param>
        /// <param name="value">The object Value.</param>
        /// <returns>True if value parsed.</returns>
        private static bool ParseValue(Type type, string stringData, out object value)
        {
            // null is only valid for bool variables
            // empty string is never valid
            if ((stringData != null || type == typeof(bool)) && (stringData == null || stringData.Length > 0))
            {
                try
                {
                    if (type == typeof(string))
                    {
                        string tempVal = stringData;
                        tempVal = (tempVal.IndexOf(">", StringComparison.OrdinalIgnoreCase) > -1) ? tempVal.Replace(">", "-") : tempVal;
                        tempVal = (tempVal.IndexOf("|", StringComparison.OrdinalIgnoreCase) > -1) ? stringData.Replace("|", " ") : tempVal;
                        value = tempVal;
                        return true;
                    }
                    else if (type == typeof(bool))
                    {
                        if (stringData == null || stringData == "+")
                        {
                            value = true;
                            return true;
                        }
                        else if (stringData == "-")
                        {
                            value = false;
                            return true;
                        }
                    }
                    else if (type == typeof(int))
                    {
                        value = int.Parse(stringData, CultureInfo.CurrentCulture);
                        return true;
                    }
                    else if (type == typeof(uint))
                    {
                        value = int.Parse(stringData, CultureInfo.CurrentCulture);
                        return true;
                    }
                    else if (type == typeof(double))
                    {
                        value = double.Parse(stringData, CultureInfo.CurrentCulture);
                        return true;
                    }
                    else if (type == typeof(float))
                    {
                        value = float.Parse(stringData, CultureInfo.CurrentCulture);
                        return true;
                    }
                    else
                    {
                        bool valid = false;
                        foreach (string name in Enum.GetNames(type))
                        {
                            if (name == stringData)
                            {
                                valid = true;
                                break;
                            }
                        }

                        if (valid)
                        {
                            value = Enum.Parse(type, stringData, true);
                            return true;
                        }
                    }
                }
                catch
                {
                    // catch parse errors
                }
            }

            value = null;
            return false;
        }
        
        /// <summary>
        /// Assign Target Object Properties.
        /// </summary>
        private void AssignTargetObjectProperties()
        {
            object value;
            foreach (FieldInfo field in this.targetObject.GetType().GetFields())
            {
                if (this.parsedArguments.ContainsKey(field.Name))
                {
                    if (field.FieldType.IsArray)
                    {
                        Argument arg = this.argumentList[field.Name];
                        ArrayList valList = new ArrayList();

                        // Add the data to a array list and set it to field.
                        Regex spaceExpr = new Regex("[\\s]+");
                        foreach (string val in spaceExpr.Split(this.parsedArguments[field.Name]))
                        {
                            if (ParseValue(field.FieldType.GetElementType(), val, out value))
                            {
                                if (arg.AllowType == ArgumentType.DefaultArgument && valList.Contains(value))
                                {
                                    throw new DuplicateArgumentValueException(
                                        string.Format(
                                        CultureInfo.CurrentCulture,
                                        "Duplicate values are passed to the parameter {0}", 
                                        field.Name));
                                }

                                valList.Add(value);
                            }
                        }

                        field.SetValue(this.targetObject, valList.ToArray(field.FieldType.GetElementType()));
                    }
                    else
                    {
                        if (ParseValue(field.FieldType, this.parsedArguments[field.Name], out value))
                        {
                            field.SetValue(this.targetObject, value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Parses the command line arguments passed from the utility.
        /// </summary>
        /// <exception cref="ArgumentParserException">Thrown when any error is found during parsing.</exception>
        /// <param name="arguments">Arguments passed via command line utility.</param>
        private void Parse(string arguments)
        {
            // regular expression to split the arguments.
            string parserExpression = "^([\\s]*)([-](?<name>[^\\s-/:=]+)([:=]?)([\\s]*)(?<value>(\"[^\"]*\")|('[^']*')|([\\s]*[^/-][^\\s]+[\\s]*)|([^/-]+)|)?([\\s]*))*$";

            RegexOptions ro = new RegexOptions();
            ro = ro | RegexOptions.IgnoreCase;
            ro = ro | RegexOptions.Multiline;
            Regex cmdLineParseExpr = new Regex(parserExpression, ro);

            Match m = cmdLineParseExpr.Match(arguments.ToString());
            if (m.Success == false)
            {
                // Regular expression did not match arguments. 
                int lastCorrectPosition = GetMismatchPosition(parserExpression, arguments);
                string errorExpr = arguments.Substring(lastCorrectPosition);
                throw new ArgumentSyntaxException(
                    string.Format(
                    CultureInfo.CurrentCulture,
                    "{0}{1}{2}{3}{4}{5}",
                    Properties.Resource.CmdLineParserException,
                    Properties.Resource.CmdLineParserExceptionSyntaxError,
                    arguments,
                    Properties.Resource.CmdLineParserExceptionSyntaxError2,
                    errorExpr,
                    Properties.Resource.CmdLineParserExceptionSyntaxError3));
            }
            else
            {
                // No issues with the syntax.
                Group unknownGroupValues = m.Groups["unknownvalues"];
                if (!string.IsNullOrEmpty(unknownGroupValues.Value))
                {
                    string unknownParamValue = unknownGroupValues.Value.Trim();
                    Regex quotesExpr = new Regex("^(\".*\")|('.*')$");
                    Match e = quotesExpr.Match(unknownParamValue);
                    if (e.Length != 0)
                    {
                        unknownParamValue = unknownParamValue.Substring(1, unknownParamValue.Length - 2);
                    }

                    this.AddParsedParameter(string.Empty, unknownParamValue);
                }

                Group param_grp = m.Groups["name"];
                Group value_grp = m.Groups["value"];
                if (param_grp == null || value_grp == null)
                {
                    // this should never happen.
                    throw new Exception("Internal Exception: Commandline parameter(s) incorrect.");
                }

                // RegEx find always pairs of name- and value-group. their count should thus always match.
                if (param_grp.Captures.Count != value_grp.Captures.Count)
                {
                    throw new Exception("Internal Exception: Number of parameters and number of values is not equal. This should never happen.");
                }

                // add each parameter and the respective value.
                for (int i = 0; i < param_grp.Captures.Count; i++)
                {
                    // if there are spaces at either side of value or param, trim those.
                    string value = value_grp.Captures[i].ToString().Trim();
                    string param = param_grp.Captures[i].ToString().Trim();
                    Regex quoteExpr = new Regex("^(\".*\")|('.*')$");
                    Match e = quoteExpr.Match(value);
                    if (e.Length != 0)
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    // if alias is passed get the actual name and add it to parsed list
                    if (this.paramNameAliasMap.ContainsKey(param))
                    {
                        param = this.paramNameAliasMap[param];
                    }

                    this.AddParsedParameter(param, value);
                }
            }

            this.CheckRequiredParameters();
        }

        /// <summary>
        /// Adds the parsed parameter and the value to the parsed argument list.
        /// </summary>
        /// <param name="paramName">The new parameter which is to be added to FoundParameters.</param>
        /// <param name="paramValue">Value which corresponds to NewParam.</param>
        private void AddParsedParameter(string paramName, string paramValue)
        {
            if (paramName == null)
            {
                throw new ArgumentNullException("paramName");
            }

            if (paramValue == null)
            {
                throw new ArgumentNullException("paramValue");
            }

            if (string.IsNullOrEmpty(paramName) && !this.argumentList.ContainsKey(paramName) && this.AllowAdditionalArguments == false)
            {
                string message = string.Concat(
                    Properties.Resource.CmdLineParserException,
                    Properties.Resource.CmdLineParserExceptionValueWithoutParameterFound,
                    paramValue,
                    Properties.Resource.CmdLineParserExceptionValueWithoutParameterFound2);
                throw new ArgumentNullValueException(message);
            }

            // if /? is passed set help parameter to true
            if (paramName.Equals("?", StringComparison.OrdinalIgnoreCase))
            {
                paramName = "help";
            }

            if (!this.argumentList.ContainsKey(paramName) && this.AllowAdditionalArguments == false)
            {
                string message = string.Concat(
                    Properties.Resource.CmdLineParserException,
                    Properties.Resource.CmdLineParserExceptionUnknownParameterFound,
                    paramName,
                    Properties.Resource.CmdLineParserExceptionUnknownParameterFound2);
                throw new ArgumentNotFoundException(message);
            }
            else if (!this.argumentList.ContainsKey(paramName) && this.AllowAdditionalArguments == true)
            {
                this.parsedArguments.Add(paramName, paramValue);
            }
            else if (this.argumentList.ContainsKey(paramName))
            {
                // the parameter is available, check for each ValueType.
                switch (this.argumentList[paramName].ValueType)
                {
                    // boolean parameters do not accept any value.
                    case ArgumentValueType.Bool:
                        if (string.IsNullOrEmpty(paramValue))
                        {
                            paramValue = "+";
                        }

                        break;
                    case ArgumentValueType.OptionalInt:
                    case ArgumentValueType.Int:
                        if (string.IsNullOrEmpty(paramValue))
                        {
                            paramValue = "0";
                        }

                        object val;
                        FieldInfo intField = this.targetObject.GetType().GetField(paramName);
                        if (!ParseValue(intField.FieldType, paramValue, out val))
                        {
                            string message = string.Concat(
                                Properties.Resource.CmdLineParserException,
                                Properties.Resource.CmdLineParserExceptionInvalidValueFound,
                                paramName,
                                Properties.Resource.CmdLineParserExceptionInvalidValueFoundInt);
                            throw new InvalidArgumentValueException(message);
                        }

                        break;
                    case ArgumentValueType.MultipleInts:
                        // split the value and add it.
                        FieldInfo field = this.targetObject.GetType().GetField(paramName);
                        Regex multiValueSeparator = new Regex("[\\s]+");
                        foreach (string value in multiValueSeparator.Split(paramValue))
                        {
                            object fieldVal;
                            if (!ParseValue(field.FieldType.GetElementType(), value, out fieldVal))
                            {
                                string message = string.Concat(
                                        Properties.Resource.CmdLineParserException,
                                        Properties.Resource.CmdLineParserExceptionInvalidValueFound,
                                        paramName,
                                        Properties.Resource.CmdLineParserExceptionInvalidValueFoundInts);
                                throw new InvalidArgumentValueException(message);
                            }
                        }

                        break;
                    case ArgumentValueType.String:
                    case ArgumentValueType.MultipleUniqueStrings:
                        if (string.IsNullOrEmpty(paramValue))
                        {
                            string message = string.Concat(
                                Properties.Resource.CmdLineParserException,
                                Properties.Resource.CmdLineParserExceptionInvalidValueFound,
                                paramName,
                                Properties.Resource.CmdLineParserExceptionInvalidValueFoundString);
                            throw new InvalidArgumentValueException(message);
                        }

                        break;
                    case ArgumentValueType.OptionalString:
                        break;
                    default: throw new Exception("Internal Exception: Unmatch case in AddNewFoundParameter()!");
                }

                if (this.parsedArguments.ContainsKey(paramName))
                {
                    string message = string.Concat(
                        Properties.Resource.CmdLineParserException,
                        Properties.Resource.CmdLineParserExceptionRepeatedParameterFound,
                        paramName,
                        Properties.Resource.CmdLineParserExceptionRepeatedParameterFoundOnce);
                    throw new ArgumentRepeatedException(message);

                }
                else
                {
                    this.parsedArguments.Add(paramName, paramValue);
                }
            }
        }

        /// <summary>
        /// Check if the values for required parameters are passed or not.
        /// </summary>
        private void CheckRequiredParameters()
        {
            if (this.parsedArguments.ContainsKey("help"))
            {
                return;
            }

            foreach (KeyValuePair<string, Argument> argument in this.argumentList)
            {
                if ((argument.Value.AllowType == ArgumentType.Required || argument.Value.AllowType == ArgumentType.DefaultArgument) 
                    && (!this.parsedArguments.ContainsKey(argument.Key)))
                {
                    if (string.IsNullOrEmpty(argument.Key))
                    {
                        string message = string.Concat(
                            Properties.Resource.CmdLineParserException,
                            Properties.Resource.CmdLineParserExceptionRequiredFirstParameterMissing);
                        throw new RequiredArgumentMissingException(message);
                    }
                    else
                    {
                        string message = string.Concat(
                            Properties.Resource.CmdLineParserException,
                            Properties.Resource.CmdLineParserExceptionRequiredParameterMissing,
                            argument.Key,
                            Properties.Resource.CmdLineParserExceptionRequiredParameterMissing2);
                        throw new RequiredArgumentMissingException(message);
                    }
                }
            }
        }

        /// <summary>
        /// This class saves the details of a single argument.
        /// </summary>
        private class Argument
        {
            public Argument(string parameterName, ArgumentType allowType, ArgumentValueType valueType, string shortName, string parameterHelp)
            {
                this.Name = parameterName;
                this.AllowType = allowType;
                this.ValueType = valueType;
                this.Help = parameterHelp;
                this.ShortName = shortName;
            }

            public string Name { get; set; }
            public ArgumentType AllowType { get; set; }
            public ArgumentValueType ValueType { get; set; }
            public string Help { get; set; }
            public string ShortName { get; set; }
        }
    }
}
