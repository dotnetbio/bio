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
    /// Test automation code for SamUtils console application
    /// matches output on console with expected output stored in file
    /// </summary>
    [TestClass]
    public class SamUtilsP1TestCases
    {
        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\UtilitiesTestsConfig.xml");        

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static SamUtilsP1TestCases()
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
        /// Runs SamUtils with option View and default parameters  and validates with expected output
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateSamUtilsWithViewDefaultParams()
        {
            ValidateSamUtils(Constants.SamUtilsWithViewDefaultParamsNodeName);
        }

        /// <summary>
        /// Runs SamUtils with option View and swith header for sam and validates with expected output
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateSamUtilsWithViewHeaderForSAM()
        {
            ValidateSamUtils(Constants.SamUtilsWithViewHeaderForSAMNodeName);
        }

        /// <summary>
        /// Runs SamUtils with option view and swtch Hex and validates with expected output
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateSamUtilsWithHEX()
        {
            ValidateSamUtils(Constants.SamUtilsWithViewHexNodeName);
        }

        /// <summary>
        /// Runs SamUtils with option View and switch Minimum Mapping Quality and validates with expected output
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateSamUtilsWithMinMapQuality()
        {
            ValidateSamUtils(Constants.SamUtilsWithViewMinMapQualityNodeName);
        }

        /// <summary>
        /// Runs SamUtils with option View and switch Output BAM and validates with expected output
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateSamUtilsWithViewOutputBAM()
        {
            ValidateSamUtils(Constants.SamUtilsWithViewOutputBAMNodeName);
        }

        /// <summary>
        /// Runs SamUtils with option View and switch SAM Format input and validates with expected output
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateSamUtilsWithViewSAMFormat()
        {
            ValidateSamUtils(Constants.SamUtilsWithViewSAMFormatNodeName);
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
                nodeName, Constants.ConsoleCommandNode);

            // Run the SamUtils with the commands updated in the xml.
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            ApplicationLog.WriteLine("************************************** SamUtils with Basic command - End **************************************");

            // Gets the output file for validation
            string expectedOutputFile = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedOutputFileNode);

            // Compares the expected output file with the result on console
            string expectedOutput = File.ReadAllText(expectedOutputFile).Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace(">", "").Trim();
            string actualOutput = Utility.commandOutput.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace(">", "").Trim();
            Assert.IsTrue(actualOutput.Contains(expectedOutput));

            ApplicationLog.WriteLine("SamUtils Console P1 : Successfully validated the results of the command");
            Console.WriteLine("SamUtils Console P1 : Successfully validated the results of the command");
        }

        # endregion Helper methods.
    }
}
