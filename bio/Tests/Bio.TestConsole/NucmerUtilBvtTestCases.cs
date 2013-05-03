using System;
using System.IO;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.TestConsole.Util;

namespace Bio.TestConsole
{
    /// <summary>
    /// Test Automation code for NucmerUtil console application
    /// </summary>
    [TestClass]
    public class NucmerUtilBvtTestCases
    {
        #region Global Variables

        readonly Utility utilityObj = new Utility(@"TestUtils\UtilitiesTestsConfig.xml");        

         #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static NucmerUtilBvtTestCases()
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
        /// validate NucmerUtil with default parameters and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNucmerUtilWithDefaultParametersAndVerbose()
        {
            ValidateNucmerUtil(Constants.NucmerUtilDefaultParamsNodeName, true);
        }

        /// <summary>
        /// Validate NucmerUtil with E-Coli data and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNucmerUtilWithEColiData()
        {
            ValidateNucmerUtil(Constants.NucmerUtilWithEColiDataNode, false);
        }       

        /// <summary>
        /// Validate NucmerUtil with switch '-l','-m','-c','d' and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNucmerUtilWithLengthAnchorClusterLengthAndDiagonalSwitches()
        {
            ValidateNucmerUtil(Constants.NucmerUtilLengthAnchorClusterLengthAndDiagonalSwitchesNode, false);
        }

        /// <summary>
        /// Validate NucmerUtil with switch '-b','-n','-r','x' and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNucmerUtilWithManySwitches()
        {
            ValidateNucmerUtil(Constants.NucmerUtilWithManySwitchesNode, false);
        }

        /// <summary>
        /// Validates NucmerUtil Help Output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNucmerUtilHelp()
        {
            string utilCommand = utilityObj.xmlUtil.GetTextValue(Constants.HelpValidationNodeName, Constants.NucmerUtilHelpCommandNodeName);
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);

            string output = Utility.TrimWhitespace(Utility.standardOut);
            string expectedHelpFile = utilityObj.xmlUtil.GetTextValue(Constants.HelpValidationNodeName, Constants.NucmerUtilExpectedHelpNodeName);
            string expectedOutput = Utility.TrimWhitespace(File.ReadAllText(expectedHelpFile));

            Assert.IsTrue(output.Contains(expectedOutput));
        }

        #endregion Test cases

        # region Helper Methods
        
        /// <summary>
        /// General method to Validate NucmerUtil test cases.
        /// </summary>        
        public void ValidateNucmerUtil(string nodeName, bool verbose)
        {
            string utilCommand = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.CommandNode);
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);

            // Gets the output file for validation
            string actualOutputFile = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ActualOutputFileNode);
            string expectedOutputFile = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedOutputFileNode);

            // Compares the results file
            Assert.IsTrue(Utility.CompareFiles(expectedOutputFile, actualOutputFile));

            if (verbose)
            {
                string expectedVerbose = Utility.TrimWhitespace(utilityObj.xmlUtil.GetTextValue(nodeName, Constants.VerboseResultNode));
                string[] verboseExpected = expectedVerbose.Split(',');
                string actualVerboseString = Utility.TrimWhitespace(Utility.standardErr);

                foreach (string value in verboseExpected)
                {
                    Assert.IsTrue(actualVerboseString.Contains(value));
                }
            }
        }

        # endregion Helper Methods       
    }
}
