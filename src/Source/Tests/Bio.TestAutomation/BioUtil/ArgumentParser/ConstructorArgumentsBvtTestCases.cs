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
    /// BVT Test Cases for ConstructorArguments class
    /// </summary>
    [TestClass]
    public class ConstructorArgumentsBvtTestCases
    {
        // <summary>
        /// Validates ConstructAndRun<T> - string[]
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateConstructAndRunTStrArr()
        {
            string[] args = { "(option1:2,option2:4,2,5)" };
            ConstructorArguments.ConstructAndRun<AggregateNumbers>(args);
        }

        // <summary>
        /// Validates ConstructAndRun<T> - string
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateConstructAndRunTStr()
        {
            string args = "(option1:2,option2:4,2,5)";
            ConstructorArguments.ConstructAndRun<AggregateNumbers>(args);
        }


        /// <summary>
        /// Validates Construct<T>
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateConstructT()
        {
            string fileName = Path.GetTempFileName();
            fileName = fileName.Replace(Path.GetTempPath(), "");
            string fileName1 = Path.GetTempFileName();
            fileName1 = fileName1.Replace(Path.GetTempPath(), "");
            using (File.Create(fileName)) { }
            using (File.Create(fileName1)) { }
            string args = "(ResultFile:(" + fileName + "),(" + fileName1 + "))";
            AggregateNumbers aggregateNums = ConstructorArguments.Construct<AggregateNumbers>(args);
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
            Type parseType = typeof(AggregateNumbers);
            bool suppressDefaults = true;
            ConstructorArguments ctorArgs = ConstructorArguments.FromParsable(obj, parseType, suppressDefaults);
            Assert.IsNotNull(ctorArgs);
        }


        /// <summary>
        /// Validates ToString with params
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateToStringWithParams()
        {
            object parsableObject = new AggregateNumbers();
            bool suppressDefaults = false;
            string str = ConstructorArguments.ToString(parsableObject, suppressDefaults);
            Assert.IsTrue(str.Contains("AggregateNumbers"));
            Assert.IsTrue(str.Contains("Cleanup"));
            Assert.IsTrue(str.Contains("ResultFile"));
            Assert.IsTrue(str.Contains("InputFile"));
        }
        
        /// <summary>
        /// Validates ToString 
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateToString()
        {
            object obj = new AggregateNumbers();
            Type parseType = typeof(AggregateNumbers);
            bool suppressDefaults = true;
            ConstructorArguments ctorArgs = ConstructorArguments.FromParsable(obj, parseType, suppressDefaults);
            string str = ctorArgs.ToString();
            Assert.IsTrue(str.Contains("ResultFile"));
            Assert.IsTrue(str.Contains("InputFile"));
        }

    }
}
