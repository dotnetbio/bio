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
    /// Test case for running LisUtil with basic command line arguments and validate the expected results file.
    /// </summary>
    [TestClass]
    public class LisUtilBvtTestCases
    {
        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\UtilitiesTestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static LisUtilBvtTestCases()
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
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateLisUtilWithDefaultParams()
        {
            ValidateLisUtil(Constants.LisUtilDefaultParamsNodeName, false);
        }

        /// <summary>
        /// Runs LisUtil using ecoli data with basic command line arguments and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateLisUtilWithEcoliDefaultParams()
        {
            ValidateLisUtil(Constants.LisUtilEcoliDefaultParamsNodeName, false);
        }

        /// <summary>
        /// Runs LisUtil with verbose switch and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateLisUtilWithVerbose()
        {
            ValidateLisUtil(Constants.LisUtilVerboseNodeName, true);
        }

        /// <summary>
        /// Runs LisUtil with MaxMatch switch and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateLisUtilWithMaxMatch()
        {
            ValidateLisUtil(Constants.LisUtilWithMaxMatchNodeName, false);
        }

        /// <summary>
        /// Runs LisUtil with ReverseOnly switch and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateLisUtilWithReverseOnly()
        {
            ValidateLisUtil(Constants.LisUtilWithReverseOnlyNodeName, false);
        }

        /// <summary>
        /// Runs LisUtil with both switch and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateLisUtilWithBoth()
        {
            ValidateLisUtil(Constants.LisUtilWithBothNodeName, false);
        }

        /// <summary>
        /// Runs LisUtil with noAmbiguity switch and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateLisUtilWithNoAmbiguity()
        {
            ValidateLisUtil(Constants.LisUtilWithNoAmbiguityNodeName, false);
        }

        /// <summary>
        /// Runs LisUtil with complement and reverseonly switches and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateLisUtilWithComplementReverseOnly()
        {
            ValidateLisUtil(Constants.LisUtilWithComplementReverseOnlyNodeName, false);
        }

        /// <summary>
        /// Runs LisUtil with complement and both switches and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateLisUtilWithComplementBoth()
        {
            ValidateLisUtil(Constants.LisUtilWithComplementBothNodeName, false);
        }

        /// <summary>
        /// Runs LisUtil with showMatchingString switch and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateLisUtilWithShowMatchingString()
        {
            ValidateLisUtil(Constants.LisUtilWithShowMatchingStrNodeName, false);
        }

        /// <summary>
        /// Validates LisUtil Help Output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateLisUtilHelp()
        {
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.LisUtilHelpCommandNodeName);
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            string output = Utility.standardOut;
            string expectedHelpFile = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.LisUtilExpectedHelpNodeName);
            string expectedOutput = File.ReadAllText(expectedHelpFile);
            Assert.IsTrue(output.Contains(expectedOutput.Trim()));
        }

        #endregion Test cases

        # region Helper methods.
        /// <summary>
        /// General method to Validate LisUtil test cases.
        /// </summary>
        /// <param name="nodeName"> Parent node in Xml</param>
        public void ValidateLisUtil(string nodeName, bool verbose)
        {

            ApplicationLog.WriteLine("************************************** LisUtil with Basic command - Start **************************************");
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CommandNode);

            // Run the Lisutil with the commands updated in the xml.
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            ApplicationLog.WriteLine("************************************** LisUtil with Basic command - End **************************************");

            // Gets the output file for validation
            string actualOutputFile = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ActualOutputFileNode);
            string expectedOutputFile = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedOutputFileNode);

            // Compares the results file
            Assert.IsTrue(Utility.CompareFiles(expectedOutputFile, actualOutputFile));

            if (verbose)
            {
                string expectedVerbose = utilityObj.xmlUtil.GetTextValue(
                        nodeName, Constants.VerboseResultNode);
                string expectedVerboseResult = expectedVerbose.Replace(" ", "");
                string[] verboseExpected = expectedVerboseResult.Split(',');
                string actualVerboseString = Utility.standardErr.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "").Trim();

                for (int i = 0; i < verboseExpected.Length; i++)
                {
                    Assert.IsTrue(actualVerboseString.Contains(verboseExpected[i]));
                }
            }

            ApplicationLog.WriteLine("LisUtil Console BVT : Successfully validated the results of the command");
            Console.WriteLine("LisUtil Console BVT : Successfully validated the results of the command");
        }

        # endregion Helper methods.
    }
}
