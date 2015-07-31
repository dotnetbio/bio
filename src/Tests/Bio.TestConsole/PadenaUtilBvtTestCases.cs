using System.IO;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.TestConsole.Util;

namespace Bio.TestConsole
{
    /// <summary>
    /// Test Automation code for PadenaUtil console application
    /// </summary>
    [TestClass]
    public class PadenaUtilBvtTestCases
    {
        #region Global Variables

        readonly Utility utilityObj = new Utility(@"TestUtils\UtilitiesTestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static PadenaUtilBvtTestCases()
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
        /// Validate PadenaUtil with Assemble command and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        [Ignore]
        public void ValidatePadenaUtilWithAssembleAndVerbose()
        {
            ValidatePadenaUtil(Constants.PadenaUtilDefaultParamsNodeName, true);
        }

        /// <summary>
        /// Validate PadenaUtil with Scaffold command and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaUtilWithScaffoldAndVerbose()
        {
            ValidatePadenaUtil(Constants.PadenaUtilWithScaffoldNode, false);
        }
        
        /// <summary>
        /// Validate PadenaUtil with AssembleWithScaffold command and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        [Ignore]
        public void ValidatePadenaUtilAssembleWithScaffoldAndVerbose()
        {
            ValidatePadenaUtil(Constants.PadenaUtilAssembleWithScaffoldNode, true);
        }

        /// <summary>
        /// Validate PadenaUtil with AssembleWithScaffold command and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        [Ignore]
        public void ValidatePadenaUtilAssembleWithScaffoldAndQuiet()
        {
            ValidatePadenaUtil(Constants.PadenaUtilAssembleWithScaffoldNode, false);
        }

        /// <summary>
        /// Validate PadenaUtil with Assemble command on E-Coli data and validate the expected results file.
        /// </summary>
        /// Due to capacity improvement, this test cases are now taking more than 30 mins. So commenting this test case
        /// the similar validation is now being performed in Bio.TestAutomation test cases
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaUtilAssembleOnEColiData()
        {
            ValidatePadenaUtil(Constants.PadenaUtilAssembleOnEColiNode, false);
        }

        /// <summary>
        /// Validates PadenaUtil Help Output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaUtilHelp()
        {
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.PadenaUtilHelpCommandNodeName);
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            string output = Utility.standardOut.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", "");
            string expectedHelpFile = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.PadenaUtilExpectedHelpNodeName);
            string expectedOutput = File.ReadAllText(expectedHelpFile).Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", "");
            Assert.IsTrue(output.Contains(expectedOutput.Trim()));
        }

        #endregion Test cases

        #region Helper Methods

        /// <summary>
        /// General method to Validate PadenaUtil test cases.
        /// </summary>        
        public void ValidatePadenaUtil(string nodeName, bool verbose)
        {
            ApplicationLog.WriteLine("************************************** PadenaUtil with Basic command - Start **************************************");
            string utilCommand = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.CommandNode);

            // Run the PadenaUtil with the commands updated in the xml.
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            ApplicationLog.WriteLine("************************************** PadenaUtil with Basic command - End **************************************");

            // Gets the output file for validation
            string actualOutputFile = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ActualOutputFileNode);
            string expectedOutputFile = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedOutputFileNode);
            
            // Compares the scaffolds of expected file with actual.
            Assert.IsTrue(Utility.ValidateScaffoldsInAFile(expectedOutputFile, actualOutputFile));

            if (verbose)
            {
                string expectedVerbose = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.VerboseResultNode);
                string expectedVerboseResult = expectedVerbose.Replace(" ", "");
                string[] verboseExpected = expectedVerboseResult.Split(',');
                string actualVerboseString = Utility.standardOut.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "").Trim();

                foreach (string text in verboseExpected)
                {
                    Assert.IsTrue(actualVerboseString.Contains(text));
                }
            }

            ApplicationLog.WriteLine("PadenaUtil Console BVT : Successfully validated the results of the command");
        }

        # endregion Helper Methods
    }
}
