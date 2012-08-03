using System;
using System.IO;
using System.Text;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SD = System.Diagnostics;
using Bio.TestConsole.Util;

namespace Bio.TestConsole
{
    /// <summary>
    /// Test Automation code for ComparativeUtilites console application
    /// </summary>
    [TestClass]
    public class ComparativeUtilitiesBvtTestCases
    {
        # region enum

        enum UtilityTypes
        {
            RepeatResolutionUtil,
            LayoutRefinementUtil,
            ConsensusUtil,
            ScaffoldUtil
        }

        # endregion enum

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\UtilitiesTestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static ComparativeUtilitiesBvtTestCases()
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
        /// Validate RepeatResolutionUtil with default params on E-Coli data  and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateRepeatResolutionUtilWithDefaultParams()
        {            
            ValidateComparativeUtilities(Constants.RepeatResolutionUtilNode);
        }

        /// <summary>
        /// Validate LayoutRefinementUtil with default params on E-Coli data and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateLayoutRefinementUtilWithDefaultParams()
        {
            ValidateComparativeUtilities(Constants.LayoutRefinementUtilNode);
        }

        /// <summary>
        /// Validate ConsensusUtil with default params on E-Coli data and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateConsensusUtilWithDefaultParams()
        {
            ValidateComparativeUtilities(Constants.ConsensusUtilNode);
        }

        /// <summary>
        /// Validate ScaffoldUtil with default params on E-Coli data and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateScaffoldUtilWithDefaultParams()
        {
            ValidateComparativeUtilities(Constants.ScaffoldUtilNode);
        }


        /// <summary>
        /// Validates ScaffoldUtil Help Output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateScaffoldUtilHelp()
        {
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.ScaffoldUtilHelpCommandNodeName);
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            string output = Utility.standardOut;
            string expectedHelpFile = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.ScaffoldUtilExpectedHelpNodeName);
            string expectedOutput = File.ReadAllText(expectedHelpFile);
            Assert.IsTrue(output.Contains(expectedOutput.Trim()));
        }

        /// <summary>
        /// Validates ConsensusUtil Help Output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateConsensusUtilHelp()
        {
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.ConsensusUtilHelpCommandNodeName);
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            string output = Utility.standardOut;
            string expectedHelpFile = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.ConsensusUtilExpectedHelpNodeName);
            string expectedOutput = File.ReadAllText(expectedHelpFile);
            Assert.IsTrue(output.Contains(expectedOutput.Trim()));
        }

        /// <summary>
        /// Validates LayoutRefinementUtil Help Output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateLayoutRefinementUtilHelp()
        {
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.LayoutRefinementUtilHelpCommandNodeName);
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            string output = Utility.standardOut;
            string expectedHelpFile = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.LayoutRefinementUtilExpectedHelpNodeName);
            string expectedOutput = File.ReadAllText(expectedHelpFile);
            Assert.IsTrue(output.Contains(expectedOutput.Trim()));
        }

        /// <summary>
        /// Validates RepeatResolutionUtil Help Output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateRepeatResolutionUtilHelp()
        {
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.RepeatResolutionUtilHelpCommandNodeName);
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            string output = Utility.standardOut;
            string expectedHelpFile = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.RepeatResolutionUtilExpectedHelpNodeName);
            string expectedOutput = File.ReadAllText(expectedHelpFile);
            Assert.IsTrue(output.Contains(expectedOutput.Trim()));
        }

        #endregion Test cases

        # region Helper Methods

        /// <summary>
        /// General method to validate all the Utilities in Comparative.
        /// </summary>
        /// <param name="nodeName">Parent node name in Xml.</param>
        /// <param name="utilityType">Utility type.</param>
        public void ValidateComparativeUtilities(string nodeName)
        {
            ApplicationLog.WriteLine("************************************** {0} with Basic command - Start **************************************",nodeName);
            
            string actualOutputString = string.Empty;
            string expectedOutputString = string.Empty;
            string utilCommandString = string.Empty;

            actualOutputString = Constants.ActualOutputFileNode;
            expectedOutputString = Constants.ExpectedOutputFileNode;
            utilCommandString = Constants.CommandNode;

            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                     nodeName, utilCommandString);

            // Run the Utilities with the commands updated in the xml.
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            ApplicationLog.WriteLine("************************************** {0} with Basic command - End **************************************", nodeName);

            // Gets the output file for validation
            string actualOutputFile = utilityObj.xmlUtil.GetTextValue(
                     nodeName, actualOutputString);
            string expectedOutputFile = utilityObj.xmlUtil.GetTextValue(
                     nodeName, expectedOutputString);

            // Compares the expected file with actual.            
            Assert.IsTrue(Utility.CompareFiles(expectedOutputFile, actualOutputFile));

            ApplicationLog.WriteLine("{0} Console BVT : Successfully validated the results of the command", nodeName);
            Console.WriteLine("{0} Console BVT : Successfully validated the results of the command", nodeName);
        }

        # endregion Helper Methods
    }
}
