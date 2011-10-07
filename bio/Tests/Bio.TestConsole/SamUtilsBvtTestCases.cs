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
    /// Test case for running SamUtils with basic command line arguments and validate the expected results file.
    /// </summary>
    [TestClass]
    public class SamUtilsBvtTestCases
    {
        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\UtilitiesTestsConfig.xml");        

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static SamUtilsBvtTestCases()
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
        /// Runs SamUtils with import option to convert SAM to BAM and validates with expected output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSamUtilsWithImportSAMtoBAM()
        {
            ValidateSamUtils(Constants.SamUtilsWithImportSAMtoBAMNodeName);
        }

        /// <summary>
        /// Runs SamUtils with import option to convert SAM to BAM and validates with expected output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSamUtilsWithImportBAMtoSAM()
        {
            ValidateSamUtils(Constants.SamUtilsWithImportBAMtoSAMNodeName);
        }

        /// <summary>
        /// Runs SamUtils with option sort and validates with expected output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSamUtilsWithSort()
        {
            ValidateSamUtils(Constants.SamUtilsWithSortNodeName);
        }

        /// <summary>
        /// Runs SamUtils with option sort (sort by read names) and  and validates with expected output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSamUtilsWithSortByReadNames()
        {
            ValidateSamUtils(Constants.SamUtilsWithSortByReadNamesNodeName);
        }

        /// <summary>
        /// Runs SamUtils with option index and validates with expected output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSamUtilsWithIndex()
        {
            ValidateSamUtils(Constants.SamUtilsWithIndexNodeName);
        }

        /// <summary>
        /// Runs SamUtils with option View and default parameters  and validates with expected output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSamUtilsWithViewDefaultParams()
        {
            ValidateSamUtils(Constants.SamUtilsWithViewDefaultParamsNodeName);
        }

        /// <summary>
        /// Runs SamUtils with option View and swith header for sam and validates with expected output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSamUtilsWithViewHeaderForSAM()
        {
            ValidateSamUtils(Constants.SamUtilsWithViewHeaderForSAMNodeName);
        }

        /// <summary>
        /// Runs SamUtils with option view and swtch Hex and validates with expected output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSamUtilsWithHEX()
        {
            ValidateSamUtils(Constants.SamUtilsWithViewHexNodeName);
        }

        /// <summary>
        /// Runs SamUtils with option View and switch Minimum Mapping Quality and validates with expected output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSamUtilsWithMinMapQuality()
        {
            ValidateSamUtils(Constants.SamUtilsWithViewMinMapQualityNodeName);
        }

        /// <summary>
        /// Runs SamUtils with option View and switch Output BAM and validates with expected output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSamUtilsWithViewOutputBAM()
        {
            ValidateSamUtils(Constants.SamUtilsWithViewOutputBAMNodeName);
        }

        /// <summary>
        /// Runs SamUtils with option View and switch SAM Format input and validates with expected output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSamUtilsWithViewSAMFormat()
        {
            ValidateSamUtils(Constants.SamUtilsWithViewSAMFormatNodeName);
        }

        /// <summary>
        /// Runs SamUtils with option View and switch compressed BAM and validates with expected output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSamUtilsWithViewUncompressedBAM()
        {
            ValidateSamUtils(Constants.SamUtilsWithViewCompressedBAMNodeName);
        }

        /// <summary>
        /// Validates SamUtils Help Output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSamUtilsHelp()
        {
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.SamUtilsHelpCommandNodeName);
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            string output = Utility.commandOutput;
            string expectedHelpFile = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.SamUtilsExpectedHelpNodeName);
            string expectedOutput = File.ReadAllText(expectedHelpFile);
            Assert.IsTrue(output.Contains(expectedOutput.Trim()));
        }

        /// <summary>
        /// Validates SamUtils Import Help Output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSamUtilsImportHelp()
        {
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.SamUtilsImportHelpCommandNodeName);
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            string output = Utility.commandOutput;
            string expectedHelpFile = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.SamUtilsImportExpectedHelpNodeName);
            string expectedOutput = File.ReadAllText(expectedHelpFile);
            Assert.IsTrue(output.Contains(expectedOutput.Trim()));
        }

        /// <summary>
        /// Validates SamUtils Merge Help Output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSamUtilMergesHelp()
        {
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.SamUtilsMergeHelpCommandNodeName);
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            string output = Utility.commandOutput;
            string expectedHelpFile = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.SamUtilsMergeExpectedHelpNodeName);
            string expectedOutput = File.ReadAllText(expectedHelpFile);
            Assert.IsTrue(output.Contains(expectedOutput.Trim()));
        }

        /// <summary>
        /// Validates SamUtils Index Help Output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSamUtilsIndexHelp()
        {
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.SamUtilsIndexHelpCommandNodeName);
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            string output = Utility.commandOutput;
            string expectedHelpFile = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.SamUtilsIndexExpectedHelpNodeName);
            string expectedOutput = File.ReadAllText(expectedHelpFile);
            Assert.IsTrue(output.Contains(expectedOutput.Trim()));
        }

        /// <summary>
        /// Validates SamUtils View Help Output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSamUtilsViewHelp()
        {
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.SamUtilsViewHelpCommandNodeName);
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            string output = Utility.commandOutput;
            string expectedHelpFile = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.SamUtilsViewExpectedHelpNodeName);
            string expectedOutput = File.ReadAllText(expectedHelpFile);
            Assert.IsTrue(output.Contains(expectedOutput.Trim()));
        }

        /// <summary>
        /// Validates SamUtils Sort Help Output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSamUtilsSortHelp()
        {
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.SamUtilsSortHelpCommandNodeName);
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            string output = Utility.commandOutput;
            string expectedHelpFile = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.SamUtilsSortExpectedHelpNodeName);
            string expectedOutput = File.ReadAllText(expectedHelpFile);
            Assert.IsTrue(output.Contains(expectedOutput.Trim()));
        }

        #endregion Test cases

        # region Helper methods.
        /// <summary>
        /// General method to Validate SamUtils test cases.
        /// </summary>
        /// <param name="nodeName"> Parent node in Xml</param>
        public void ValidateSamUtils(string nodeName)
        {

            ApplicationLog.WriteLine("************************************** SamUtils with Basic command - Start **************************************");
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CommandNode);

            // Run the SamUtils with the commands updated in the xml.
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            ApplicationLog.WriteLine("************************************** SamUtils with Basic command - End **************************************");

            // Gets the output file for validation
            string actualOutputFile = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ActualOutputFileNode);
            string expectedOutputFile = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedOutputFileNode);

            // Compares the results file
            Assert.IsTrue(Utility.CompareFiles(expectedOutputFile, actualOutputFile));

            ApplicationLog.WriteLine("SamUtils Console BVT : Successfully validated the results of the command");
            Console.WriteLine("SamUtils Console BVT : Successfully validated the results of the command");
        }

        # endregion Helper methods.
    }
}
