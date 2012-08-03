using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SD = System.Diagnostics;
using Bio.TestConsole.Util;
using Bio.Util.Logging;


namespace Bio.TestConsole
{
    /// <summary>
    /// Test automation code for LisUtil console application
    /// matches output on console with expected output stored in file
    /// </summary>
    [TestClass]
    public class LisUtilP1TestCases
    {
        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\UtilitiesTestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        public LisUtilP1TestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region Test cases

        /// <summary>
        /// Runs LisUtil with basic command line arguments and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateLisUtilWithDefaultParams()
        {
            ValidateLisUtil(Constants.LisUtilDefaultParamsNodeName);
        }

        /// <summary>
        /// Runs LisUtil using ecoli data with basic command line arguments and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateLisUtilWithEcoliDefaultParams()
        {
            ValidateLisUtil(Constants.LisUtilEcoliDefaultParamsNodeName);
        }

        /// <summary>
        /// Runs LisUtil with MaxMatch switch and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateLisUtilWithMaxMatch()
        {
            ValidateLisUtil(Constants.LisUtilWithMaxMatchNodeName);
        }

        /// <summary>
        /// Runs LisUtil with ReverseOnly switch and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateLisUtilWithReverseOnly()
        {
            ValidateLisUtil(Constants.LisUtilWithReverseOnlyNodeName);
        }

        /// <summary>
        /// Runs LisUtil with both switch and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateLisUtilWithBoth()
        {
            ValidateLisUtil(Constants.LisUtilWithBothNodeName);
        }

        /// <summary>
        /// Runs LisUtil with noAmbiguity switch and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateLisUtilWithNoAmbiguity()
        {
            ValidateLisUtil(Constants.LisUtilWithNoAmbiguityNodeName);
        }

        /// <summary>
        /// Runs LisUtil with complement and reverseonly switches and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateLisUtilWithComplementReverseOnly()
        {
            ValidateLisUtil(Constants.LisUtilWithComplementReverseOnlyNodeName);
        }

        /// <summary>
        /// Runs LisUtil with complement and both switches and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateLisUtilWithComplementBoth()
        {
            ValidateLisUtil(Constants.LisUtilWithComplementBothNodeName);
        }

        /// <summary>
        /// Runs LisUtil with showMatchingString switch and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateLisUtilWithShowMatchingString()
        {
            ValidateLisUtil(Constants.LisUtilWithShowMatchingStrNodeName);
        }

        #endregion Test cases

        # region Helper methods.
        /// <summary>
        /// General method to Validate LisUtil test cases.
        /// </summary>
        /// <param name="nodeName"> Parent node in Xml</param>
        public void ValidateLisUtil(string nodeName)
        {

            ApplicationLog.WriteLine("************************************** LisUtil with Basic command - Start **************************************");
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ConsoleCommandNode);

            // Run the Lisutil with the commands updated in the xml.
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            ApplicationLog.WriteLine("************************************** LisUtil with Basic command - End **************************************");

            // Gets the output file for validation
            string expectedOutputFile = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedOutputFileNode);

            // Compares the expected output file with the result on console
            string expectedOutput = File.ReadAllText(expectedOutputFile).Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace(">", "").Trim();
            string actualOutput = Utility.commandOutput.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace(">", "").Trim();
            Assert.IsTrue(actualOutput.Contains(expectedOutput));

            ApplicationLog.WriteLine("LisUtil Console P1 : Successfully validated the results of the command");
            Console.WriteLine("LisUtil Console P1 : Successfully validated the results of the command");
        }

        # endregion Helper methods.
    }
}
