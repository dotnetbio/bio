using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Util.ArgumentParser;
using System.Collections.ObjectModel;
using Bio.Util.Logging;
using System.IO;
using Bio;
using System.Threading;
using Bio.Util.Distribute;
using Bio.Util;

namespace Bio.TestAutomation.BioUtil.ArgumentParser
{
    /// <summary>
    /// BVT Test cases for CommandArguments Class
    /// </summary>
    [TestClass]
    public class CommandArgumentsBvtTestCases
    {

        /// <summary>
        /// Validates ConstructAndRun<T>
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateConstructAndRunT()
        {
            string fileName = Path.GetTempFileName();
            fileName = fileName.Replace(Path.GetTempPath(), "");
            string[] args = { fileName, "Sum" };
            CommandArguments.ConstructAndRun<AggregateNumbers>(args);
        }


        /// <summary>
        /// Validates CommandArguments Constructor
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateCommandArguments()
        {
            string fileName = Path.GetTempFileName();
            fileName = fileName.Replace(Path.GetTempPath(), "");
            CommandArguments cmdArgs = new CommandArguments();
            Assert.IsNotNull(cmdArgs);
            cmdArgs = new CommandArguments(fileName);
            Assert.IsNotNull(cmdArgs);
            string[] args = { fileName, "Sum" };
            cmdArgs = new CommandArguments(args);
            Assert.IsNotNull(cmdArgs);
        }


        /// <summary>
        /// Validates Construct<T> - string[]
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateConstructTStringArr()
        {
            string fileName = Path.GetTempFileName();
            fileName = fileName.Replace(Path.GetTempPath(), "");
            using (File.Create(fileName)) { }
            string[] args = { "(" + fileName + ")" };
            AggregateNumbers aggregateNums = CommandArguments.Construct<AggregateNumbers>(args);
            Assert.IsNotNull(aggregateNums);
        }

        /// <summary>
        /// Validates Construct<T> - string
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateConstructTString()
        {
            string fileName = Path.GetTempFileName();
            fileName = fileName.Replace(Path.GetTempPath(), "");
            using (File.Create(fileName)) { }
            string args = "(" + fileName + ")";
            AggregateNumbers aggregateNums = CommandArguments.Construct<AggregateNumbers>(args);
            Assert.IsNotNull(aggregateNums);
        }


        /// <summary>
        /// Validates FromParsable
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFromParsable()
        {
            object obj = new AggregateNumbers();
            CommandArguments cmdArgs = CommandArguments.FromParsable(obj);
            Assert.IsNotNull(cmdArgs);
        }


        /// <summary>
        /// Validates ToString1
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateToString1()
        {
            string fileName = Path.GetTempFileName();
            fileName = fileName.Replace(Path.GetTempPath(), "");
            string[] args = { fileName, "Sum" };
            object parsableObject = new CommandArguments(args);
            bool suppressDefaults = true;
            bool protectWithQuotes = true;
            string str = CommandArguments.ToString(parsableObject, suppressDefaults, protectWithQuotes);
            Assert.IsNotNull(str);
        }

        /// <summary>
        /// Validates ToString2
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateToString2()
        {
            string fileName = Path.GetTempFileName();
            fileName = fileName.Replace(Path.GetTempPath(), "");
            string[] args = { fileName, "Sum" };
            CommandArguments parsableObject = new CommandArguments(args);
            bool protectWithQuotes = true;
            string str = parsableObject.ToString(protectWithQuotes);
            Assert.IsNotNull(str);
        }

        
    }

    #region Helper Classes and Interfaces

    public interface IBinaryOperator
    {
        double Aggregate(double x, double y);
    }

    public class Sum : IBinaryOperator
    {
        public double Aggregate(double x, double y)
        {
            return x + y;
        }
    }

    public class Product : IBinaryOperator
    {
        public double Aggregate(double x, double y)
        {
            return x * y;
        }
    }

    public class Min : IBinaryOperator
    {
        public double Aggregate(double x, double y)
        {
            return Math.Min(x, y);
        }
    }


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
    public class AggregateNumbers : SelfDistributable
    {
        /// <summary>
        /// The input file must contain a list of doubles, one per line.
        /// </summary>
        [Parse(ParseAction.Required, typeof(InputFile))]
        private FileInfo InputFile = new FileInfo(Path.GetTempFileName());

        /// <summary>
        /// The operation you want done.
        /// </summary>
        //[Parse(ParseAction.Required)]
        private Sum Operator = new Sum();

        /// <summary>
        /// Where the results will live.
        /// </summary>
        [Parse(ParseAction.Optional, typeof(OutputFile))]
        private FileInfo ResultFile = new FileInfo("results.txt");

        /// <summary>
        /// How long to sleep between each step.
        /// </summary>
        private int SleepMilliseconds = 1000;

        /// <summary>
        /// Method to run tasks specifying the task name and count
        /// </summary>
        /// <param name="tasksToRun">name of task</param>
        /// <param name="taskCount">count of task</param>
        public override void RunTasks(RangeCollection tasksToRun, long taskCount)
        {
            var allTheWorkQuery = InputFile.ReadEachLine();
            var myWorkAndAnIndex = SpecialFunctions.DivideWork(allTheWorkQuery, tasksToRun, taskCount, 1, new RangeCollection());

            double myTotal = double.NaN;
            bool isFirst = true;
            foreach (var numberAndIndex in myWorkAndAnIndex)
            {
                double number = double.Parse(numberAndIndex.Key, (IFormatProvider)null);
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
                var myUniqueResultFileName = GetFileTaskFileName(tasksToRun.ToString());

                using (TextWriter writer = File.CreateText(myUniqueResultFileName))
                {
                    writer.WriteLine(myTotal);
                }
            }
        }

        /// <summary>
        /// Cleanup method for the class
        /// </summary>
        /// <param name="taskCount">count of counts</param>
        public override void Cleanup(long taskCount)
        {
            var taskFilePattern = GetFileTaskFileName("*");
            var taskFiles = new DirectoryInfo(".").GetFiles(taskFilePattern).ToList();
            double myTotal = double.NaN;
            bool isFirst = true;

            foreach (var file in taskFiles)
            {
                double number = double.Parse(file.ReadEachLine().Single(), (IFormatProvider)null);
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

            using (var writer = ResultFile.CreateText())
            {
                writer.WriteLine(myTotal);
            }

            foreach (var file in taskFiles)
            {
                file.Delete();
            }
        }

        /// <summary>
        /// method for getting the task file name
        /// </summary>
        /// <param name="taskLabel">task label for which name is to be retrieved</param>
        /// <returns></returns>
        private string GetFileTaskFileName(string taskLabel)
        {
            var relativeDirectory = Path.GetDirectoryName(ResultFile.ToString());

            var myUniqueResultFileName = string.Format((IFormatProvider)null ,"{0}\\{1}.{2}{3}",
                string.IsNullOrEmpty(relativeDirectory) ? "." : relativeDirectory,
                Path.GetFileNameWithoutExtension(ResultFile.Name),
                taskLabel,
                Path.GetExtension(ResultFile.Name));
            return myUniqueResultFileName;
        }


    }
    #endregion Helper Classes and Interfaces
}
