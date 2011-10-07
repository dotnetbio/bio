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
    /// Test automation code for ComparativeUtil console application
    /// </summary>
    [TestClass]
    public class ComparativeUtilBvtTestCases
    {
        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\UtilitiesTestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        public ComparativeUtilBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region Test Cases
        /// <summary>
        /// Runs ComparativeUtil with basic command line arguments and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateComparativeUtilWithDefaultParameters()
        {
            ValidateComparativeUtil(Constants.ComparativeUtilDefaultParamsNodeName, false);
        }

        /// <summary>
        /// Runs ComparativeUtil using ecoli data with basic command line arguments and validates expected result file
        /// </summary>
        /// Due to capacity improvement, this test cases are now taking more than 30 mins. So commenting this test case
        /// the similar validation is now being performed in Bio.TestAutomation test cases
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateComparativeUtilWithEcoliDefaultParameters()
        {
            ValidateComparativeUtil(Constants.ComparativeUtilEcoliDefaultParamsNodeName, false);
        }

        /// <summary>
        /// Runs ComparativeUtil with scaffold using ecoli data with basic command line arguments and validates expected result file
        /// </summary>
        /// Due to capacity improvement, this test cases are now taking more than 30 mins. So commenting this test case
        /// the similar validation is now being performed in Bio.TestAutomation test cases
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateComparativeUtilWithScaffoldEcoli()
        {
            ValidateComparativeUtil(Constants.ComparativeUtilWithScaffoldEcoliNodeName, false);
        }


        /// <summary>
        /// Runs ComparativeUtil with verbose switch and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateComparativeUtilWithVerbose()
        {
            ValidateComparativeUtil(Constants.ComparativeUtilVerboseNodeName, true);
        }

        /// <summary>
        /// Runs ComparativeUtil with KmerLength and MumLength Parameters and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateComparativeUtilWithKmerAndMumLengths()
        {
            ValidateComparativeUtil(Constants.ComparativeUtilWithKmerAndMumNodeName, false);
        }

        /// <summary>
        /// Runs ComparativeUtil with meanlengthofinsert and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateComparativeUtilWithMeanLengthInsert()
        {
            ValidateComparativeUtil(Constants.ComparativeUtilWithMeanLengthInsertNodeName, false);
        }

        /// <summary>
        /// Validates Comparative Help Output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateComparativeUtilHelp()
        {
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.ComparativeUtilHelpCommandNodeName);
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            string output = Utility.commandOutput.Replace('\r',' ').Replace('\n',' ').Replace('\t',' ');

            string expectedHelpFile = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.ComparativeUtilExpectedHelpNodeName);
            string expectedOutput = File.ReadAllText(expectedHelpFile);
            Assert.IsTrue(output.Contains(expectedOutput.Trim()));
        }

        #endregion Test cases

        #region Helper Methods

        /// <summary>
        /// General method to Validate ComparativeUtil test cases.
        /// </summary>
        /// <param name="nodeName">Parent node in XML</param>
        /// <param name="verbose">Indicates whether verbose switch is used or not</param>
        public void ValidateComparativeUtil(string nodeName, bool verbose)
        {
            ApplicationLog.WriteLine("************************************** ComparativeUtil with Basic command - Start **************************************");
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CommandNode);

            // Run the ComparativeUtil with the commands updated in the xml.
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            ApplicationLog.WriteLine("************************************** ComparativeUtil with Basic command - End **************************************");

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
                string actualVerboseString = Utility.verboseOutput.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "").Trim();

                for (int i = 0; i < verboseExpected.Length; i++)
                {
                    Assert.IsTrue(actualVerboseString.Contains(verboseExpected[i]));
                }
            }
            
            ApplicationLog.WriteLine("ComparativeUtil Console BVT : Successfully validated the results of the command");
            Console.WriteLine("ComparativeUtil Console BVT : Successfully validated the results of the command");
        }

        #endregion Helper Methods
    }
}
