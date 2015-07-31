/****************************************************************************
 * CmdLineArgumentParserBvtTestCases.cs
 * 
 * This file contains the CmdLineArgumentParser BVT test cases.
 * 
******************************************************************************/

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Util.ArgumentParser;
using System.Threading;
using System.Collections;
using Bio.Util.Logging;

namespace Bio.TestAutomation.Util.ArgumentParser
{
    /// <summary>
    /// BVT Test Cases for CmdLineArgumentParser class
    /// </summary>
    [TestClass]
    public class CmdLineArgumentParserBvtTestCases
    {
        #region BVT Test Cases
        
        /// <summary>
        /// Validates CommandLineArguments
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateCommandLineArguments()
        {
            AggregateNumbers aggregateNos = new AggregateNumbers();
            CommandLineArguments parser = new CommandLineArguments();

            //add parameters
            parser.Parameter(ArgumentType.Required, "InputFile", ArgumentValueType.String, "i", "File containing numbers to add");
            parser.Parameter(ArgumentType.Required, "ResultFile", ArgumentValueType.String, "r", "File to store output");
            string inputfileName = Path.GetTempFileName().Replace(Path.GetTempPath(), "");
            string outputfileName = Path.GetTempFileName().Replace(Path.GetTempPath(), "");
            string[] args = { "-InputFile:" + inputfileName , "-ResultFile:" + outputfileName };
            parser.Parse(args, aggregateNos);
            Assert.IsNotNull(parser);
            Assert.IsTrue(aggregateNos.InputFile.Contains(inputfileName));
            Assert.IsTrue(aggregateNos.ResultFile.Contains(outputfileName));
        }

        /// <summary>
        /// Validates GetEnumerator
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGetEnumerator()
        {
            AggregateNumbers aggregateNos = new AggregateNumbers();
            CommandLineArguments parser = new CommandLineArguments();

            //add parameters
            parser.Parameter(ArgumentType.Required, "InputFile", ArgumentValueType.String, "i", "File containing numbers to add");
            parser.Parameter(ArgumentType.Required, "ResultFile", ArgumentValueType.String, "r", "File to store output");
            string inputfileName = Path.GetTempFileName().Replace(Path.GetTempPath(), "");
            string outputfileName = Path.GetTempFileName().Replace(Path.GetTempPath(), "");
            string[] args = { "-InputFile:" + inputfileName, "-ResultFile:" + outputfileName };
            parser.Parse(args, aggregateNos);
            IEnumerator parsedVals =  parser.GetEnumerator();
            Assert.IsNotNull(parsedVals);
            string current = string.Empty;
            int count = 0;
            parser.Reset();
            while (parser.MoveNext())
            {
                current = parser.Current.ToString();
                ApplicationLog.WriteLine(current);
                count++;
            }
            Assert.AreEqual(2, count);
        }


        /// <summary>
        /// Validates types in Parameter
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateTypesInParameter()
        {
            AggregateNumbers aggregateNos = new AggregateNumbers();
            CommandLineArguments parser = new CommandLineArguments();

            //add parameters
            parser.Parameter(ArgumentType.Required, "bValue", ArgumentValueType.Bool, "bv", "bool");
            parser.Parameter(ArgumentType.Required, "iValue", ArgumentValueType.Int, "iv", "int");
            parser.Parameter(ArgumentType.Required, "fValue", ArgumentValueType.Int, "fv", "float");
            parser.Parameter(ArgumentType.Required, "dValue", ArgumentValueType.Int, "dv", "double");
            parser.Parameter(ArgumentType.DefaultArgument, "usValues", ArgumentValueType.MultipleUniqueStrings, "usv", "Unique strings");

            string[] args = { "-bValue:true", "-iValue:5", "-fValue:3.45", "-dValue:78.9876", "Str1", "Str2","Str3"};
            parser.Parse(args, aggregateNos);
            Assert.IsNotNull(parser);
        }


        #endregion BVT Test Cases

        
    }

    #region Helper Classes and Interfaces
    /// <summary>
    /// Test interface to use in BVT test cases
    /// </summary>
    public interface IBinaryOperator
    {
        double Aggregate(double x, double y);
    }
    /// <summary>
    /// Test class to use in BVT test cases
    /// </summary>
    public class Sum : IBinaryOperator
    {
        public double Aggregate(double x, double y)
        {
            return x + y;
        }
    }
    /// <summary>
    /// Test class to use in BVT test cases
    /// </summary>
    public class Product : IBinaryOperator
    {
        public double Aggregate(double x, double y)
        {
            return x * y;
        }
    }
    /// <summary>
    /// Test class to use in BVT test cases
    /// </summary>
    public class Min : IBinaryOperator
    {
        public double Aggregate(double x, double y)
        {
            return Math.Min(x, y);
        }
    }

    /// <summary>
    /// Test class to use in BVT test cases
    /// </summary>
    public class Max : IBinaryOperator
    {
        public double Aggregate(double x, double y)
        {
            return Math.Max(x, y);
        }
    }


    /// <summary>
    /// Example distributable application that take a list of numbers from an input file and performs some aggregate operation. The result is stored in a Result file.
    /// </summary>
    public class AggregateNumbers
    {
        /// <summary>
        /// The input file must contain a list of doubles, comma delimited.
        /// </summary>
        public string InputFile;

        /// <summary>
        /// The operation you want done.
        /// </summary>
        //[Parse(ParseAction.Required)]
        private Sum Operator = new Sum();

        /// <summary>
        /// Where the results will live.
        /// </summary>
        public string ResultFile;

        /// <summary>
        /// How long to sleep between each step.
        /// </summary>
        private int SleepMilliseconds = 1000;

        //fields to check different types of values;
        public bool bValue;
        public float fValue;
        public int iValue;
        public double dValue;
        public int[] iArrValues;
        public bool[] bArrValues;
        //for unique values in array test
        public bool[] ubValues;
        public string[] usValues;


        public void RunTasks()
        {
            string[] nos;
            using (StreamReader reader = new StreamReader(InputFile))
            {
                nos = reader.ReadLine().Split(',');
            }
            double myTotal = double.NaN;
            bool isFirst = true;
            foreach (var num in nos)
            {
                double number = double.Parse(num, (IFormatProvider)null);
                Thread.Sleep(SleepMilliseconds); //make the work take longer
                if (isFirst)
                {
                    myTotal = number;
                    isFirst = false;
                }
                else
                {
                    myTotal = Operator.Aggregate(myTotal, number);
                }
            }

            if (!isFirst)   // make sure we actually did something.
            {
                using (TextWriter writer = File.CreateText(ResultFile))
                {
                    writer.WriteLine(myTotal);
                }
            }
        }

    }
    #endregion Helper Classes and Interfaces
}
