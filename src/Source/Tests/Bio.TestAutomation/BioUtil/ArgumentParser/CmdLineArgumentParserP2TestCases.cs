/****************************************************************************
 * CmdLineArgumentParserP2TestCases.cs
 * 
 * This file contains the CmdLineArgumentParser P2 test cases.
 * 
******************************************************************************/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Util.ArgumentParser;
using System.Threading;
using System.Collections;
using Bio.Util.Logging;

namespace Bio.TestAutomation.Util.ArgumentParser
{
    /// <summary>
    /// P2 Test Cases for CmdLineArgumentParser
    /// </summary>
    [TestClass]
    public class CmdLineArgumentParserP2TestCases
    {

        /// <summary>
        /// Invalidates Parameter()
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateParameter()
        {
            //AggregateNumbers aggregateNos = new AggregateNumbers();
            CommandLineArguments parser = new CommandLineArguments();
            try
            {
                //add parameters
                parser.Parameter(ArgumentType.Required, null, ArgumentValueType.String, "i", "File containing numbers to add");
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.WriteLine("Successfully caught ArgumentNullException : " + ex.Message);
            }
        }

        /// <summary>
        /// Invalidates Parse()
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateParse()
        {
            AggregateNumbers aggregateNos = new AggregateNumbers();
            CommandLineArguments parser = new CommandLineArguments();

            //add parameters
            parser.Parameter(ArgumentType.Required, "InputFile", ArgumentValueType.String, "i", "File containing numbers to add");
            parser.Parameter(ArgumentType.Required, "ResultFile", ArgumentValueType.String, "r", "File to store output");
            string outputfileName = Path.GetTempFileName().Replace(Path.GetTempPath(), "");
            //pass parameter without value
            try
            {
                string[] args = { "/InputFile:", "/ResultFile:" + outputfileName };
                parser.Parse(args, aggregateNos);
                Assert.Fail();
            }
            catch (ArgumentParserException ex)
            {
                ApplicationLog.WriteLine("CommandLineArguments P2: Successfully caught InvalidArgumentValueException : " + ex.Message);
            }

            //pass invalid starting character
            try
            {
                string[] args = { "*/ResultFile:" + outputfileName };
                parser.Parse(args, aggregateNos);
                Assert.Fail();
            }
            catch (ArgumentParserException ex)
            {
                ApplicationLog.WriteLine("CommandLineArguments P2: Successfully caught ArgumentNullException : " + ex.Message);
            }
        }


        /// <summary>
        /// Invalidate Parameter for null parameter name and bool 
        /// Input Data : Null Parameter name and Argument type is bool.
        /// Output Data : Validation of Exception by parameter type and Argument type.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateParameterForNullParameterName()
        {
            CommandLineArguments commandLineArguments = new CommandLineArguments();

            try
            {
                commandLineArguments.Parameter(ArgumentType.Optional, null, ArgumentValueType.OptionalInt, "ParameterForNullParameterName", "");
                Assert.Fail("CommandLineArguments P2: Not validated parameter Method for null value");
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteLine("CommandLineArguments P2: Successfully validated Parameter Method for null value:", ex.Message);
            }

            try
            {
                commandLineArguments.Parameter(ArgumentType.Optional, null, ArgumentValueType.Bool, "ParameterForNullParameterName", "");
                Assert.Fail("CommandLineArguments P2: Not validated parameter Method for bool value");
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteLine("CommandLineArguments P2: Successfully validated Parameter Method for bool value:", ex.Message);
            }
        }

        /// <summary>
        /// Invalidate Parameter for null arguments. 
        /// Input Data : Null arguments type.
        /// Output Data : Validation of ArgumentNullException Exception.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateParseForNullArguments()
        {
            AggregateNumbers aggregateNos = new AggregateNumbers();
            CommandLineArguments parser = new CommandLineArguments();

            try
            {
                parser.Parse(null, aggregateNos);
                Assert.Fail("CommandLineArguments P2: Not validated parameter Method for null value");
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.WriteLine("CommandLineArguments P2: Successfully validated Parameter Method for null value:", ex.Message);
            }
        }

        /// <summary>
        /// Invalidate Parameter for null arguments. 
        /// Input Data : Null arguments type.
        /// Output Data : Validation of ArgumentNullException Exception.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateParseForNullArgumentsNo()
        {
            CommandLineArguments parser = new CommandLineArguments();

            //add parameters
            parser.Parameter(ArgumentType.Required, "InputFile", ArgumentValueType.String, "i", "File containing numbers to add");
            parser.Parameter(ArgumentType.Required, "ResultFile", ArgumentValueType.String, "r", "File to store output");

            string inputfileName = Path.GetTempFileName().Replace(Path.GetTempPath(), "");
            string outputfileName = Path.GetTempFileName().Replace(Path.GetTempPath(), "");
            string[] args = { "/InputFile:" + inputfileName, "/ResultFile:" + outputfileName };

            try
            {
                parser.Parse(args, null);
                Assert.Fail("CommandLineArguments P2: Not validated parameter Method for null value");
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.WriteLine("CommandLineArguments P2: Successfully validated Parameter Method for null value:", ex.Message);
            }
        }

        /// <summary>
        /// Invalidates parse with empty parameter name
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateParseWithEmptyParam()
        {
            try
            {
                AggregateNumbers aggregateNos = new AggregateNumbers();
                CommandLineArguments parser = new CommandLineArguments();

                //add parameters
                parser.Parameter(ArgumentType.Required, "bValue", ArgumentValueType.Bool, "bv", "bool");
                parser.Parameter(ArgumentType.Required, "iValue", ArgumentValueType.Int, "iv", "int");

                string[] args = { "-:true", "-iValue:5" };
                parser.Parse(args, aggregateNos);
                Assert.Fail();
            }
            catch (ArgumentSyntaxException ex)
            {
                ApplicationLog.Write("Successfully caught ArgumentSyntaxException : " + ex.Message);
            }
        }

        /// <summary>
        /// Invalidates parse with invalid values
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateParseWithInvalidValue()
        {
            try
            {
                AggregateNumbers aggregateNos = new AggregateNumbers();
                CommandLineArguments parser = new CommandLineArguments();

                //add parameters
                parser.Parameter(ArgumentType.Required, "bValue", ArgumentValueType.Bool, "bv", "bool");
                parser.Parameter(ArgumentType.Required, "iValue", ArgumentValueType.Int, "iv", "int");
                parser.Parameter(ArgumentType.Required, "bArrValues", ArgumentValueType.Bool, "bmv", "boolArrValues");
                //pass char in integer array
                string[] args = { "/bValue:true", "/iValue:x", "/bArrValues:true", "false" };
                parser.Parse(args, aggregateNos);
                Assert.Fail();
            }
            catch (ArgumentParserException ex)
            {
                ApplicationLog.Write("Successfully caught InvalidArgumentValueException : " + ex.Message);
            }
        }

        /// <summary>
        /// Invalidates parse with wrong param name
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateParseWithWrongParamName()
        {
            try
            {
                AggregateNumbers aggregateNos = new AggregateNumbers();
                CommandLineArguments parser = new CommandLineArguments();

                //add parameters
                parser.Parameter(ArgumentType.Required, "bValue", ArgumentValueType.Bool, "bv", "bool");
                parser.Parameter(ArgumentType.Required, "iValue", ArgumentValueType.Int, "iv", "int");
                parser.Parameter(ArgumentType.Required, "bArrValues", ArgumentValueType.Bool, "bmv", "boolArrValues");
                //pass char in integer array
                string[] args = { "/wrongname:true", "/iValue:5", "/bArrValues:true", "false" };
                parser.Parse(args, aggregateNos);
                Assert.Fail();
            }
            catch (ArgumentParserException ex)
            {
                ApplicationLog.Write("Successfully caught ArgumentNotFoundException : " + ex.Message);
            }
        }

        /// <summary>
        /// Invalidates parse with duplicate string values
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateParseWithDuplicateString()
        {
            try
            {
                AggregateNumbers aggregateNos = new AggregateNumbers();
                CommandLineArguments parser = new CommandLineArguments();

                //add parameters
                parser.Parameter(ArgumentType.Required, "bValue", ArgumentValueType.Bool, "bv", "bool");
                parser.Parameter(ArgumentType.Required, "iValue", ArgumentValueType.Int, "iv", "int");
                parser.Parameter(ArgumentType.Required, "fValue", ArgumentValueType.Int, "fv", "float");
                parser.Parameter(ArgumentType.Required, "dValue", ArgumentValueType.Int, "dv", "double");
                parser.Parameter(ArgumentType.DefaultArgument, "usValues", ArgumentValueType.MultipleUniqueStrings, "usv", "Unique strings");

                string[] args = { "-bValue:true", "-iValue:5", "-fValue:3.45", "-dValue:78.9876", "Str1", "Str2","Str1"};
                parser.Parse(args, aggregateNos);
            }
            catch (DuplicateArgumentValueException ex)
            {
                ApplicationLog.WriteLine("Successfully caught DuplicateArgumentValueException : " + ex.Message);
            }
        }

        /// <summary>
        /// Invalidates parse with missing required param
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateParseWithMissingRequiredParam()
        {
            try
            {
                AggregateNumbers aggregateNos = new AggregateNumbers();
                CommandLineArguments parser = new CommandLineArguments();

                //add parameters
                parser.Parameter(ArgumentType.Required, "bValue", ArgumentValueType.Bool, "bv", "bool");
                parser.Parameter(ArgumentType.Required, "iValue", ArgumentValueType.Int, "iv", "int");
                parser.Parameter(ArgumentType.Required, "bArrValues", ArgumentValueType.Bool, "bmv", "boolArrValues");
                parser.Parameter(ArgumentType.Required, "fValue", ArgumentValueType.Int, "fv", "float");
                parser.Parameter(ArgumentType.Required, "dValue", ArgumentValueType.Int, "dv", "double");
                parser.Parameter(ArgumentType.DefaultArgument, "usValues", ArgumentValueType.MultipleUniqueStrings, "usv", "Unique strings");

                //not including the first required param
                string[] args = { "-iValue:5", "-bArrValues:true" ,"false",
                            "-fValue:3.45", "-dValue:78.9876", "Str1", "Str2","Str3"};
                parser.Parse(args, aggregateNos);
                Assert.Fail();
            }
            catch (RequiredArgumentMissingException ex)
            {
                ApplicationLog.WriteLine("Successfully caught RequiredArgumentMissingException : " + ex.Message);
            }
        }
    }

}

