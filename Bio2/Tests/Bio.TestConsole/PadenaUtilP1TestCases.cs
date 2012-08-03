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
    /// Test automation code for PadenaUtil console application
    /// matches output on console with expected output stored in file
    /// </summary>
    [TestClass]
    public class PadenaUtilP1TestCases
    {
        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\UtilitiesTestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static PadenaUtilP1TestCases()
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
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaUtilWithAssemble()
        {
            ValidatePadenaUtil(Constants.PadenaUtilDefaultParamsNodeName);
        }

        /// <summary>
        /// Validate PadenaUtil with Scaffold command and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaUtilWithScaffold()
        {
            ValidatePadenaUtil(Constants.PadenaUtilWithScaffoldNode);
        }
        
        /// <summary>
        /// Validate PadenaUtil with AssembleWithScaffold command and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaUtilAssembleWithScaffold()
        {
            ValidatePadenaUtil(Constants.PadenaUtilAssembleWithScaffoldNode);
        }

        /// <summary>
        /// Validate PadenaUtil with Assemble command on E-Coli data and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaUtilAssembleOnEColiData()
        {
            ValidatePadenaUtil(Constants.PadenaUtilAssembleOnEColiNode);
        }

        #endregion Test cases

        # region Helper Methods

        /// <summary>
        /// General method to Validate PadenaUtil test cases.
        /// </summary>        
        public void ValidatePadenaUtil(string nodeName)
        {
            ApplicationLog.WriteLine("************************************** PadenaUtil with Basic command - Start **************************************");
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                     nodeName, Constants.ConsoleCommandNode);

            // Run the PadenaUtil with the commands updated in the xml.
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            ApplicationLog.WriteLine("************************************** PadenaUtil with Basic command - End **************************************");

            // Gets the output file for validation
            string expectedOutputFile = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedOutputFileNode);

            string expectedOutput = File.ReadAllText(expectedOutputFile);
            string actualOutput = Utility.commandOutput;
            Assert.IsTrue(this.MatchFiles(expectedOutput, actualOutput));


            ApplicationLog.WriteLine("PadenaUtil Console P1 : Successfully validated the results of the command");
            Console.WriteLine("PadenaUtil Console P1 : Successfully validated the results of the command");
        }

        /// <summary>
        /// Since the sequences are not always in the same order MatchFiles() will look for each 
        /// sequence individually in the expected output
        /// </summary>
        /// <param name="expectedOutput"></param>
        /// <param name="actualOutput"></param>
        private bool MatchFiles(string expectedOutput, string actualOutput)
        {
            bool match = false;
            string[] outputSequences = actualOutput.Replace("\r", "").Split('\n');
            for (int i = 0; i < outputSequences.Length; i++)
            {
                //ignore utility name, blank lines and version & copyright info when matching sequences
                if (!outputSequences[i].Contains("Padena") && 
                    !outputSequences[i].Contains("Copyright") &&
                    outputSequences[i].Trim().Length>0)
                {
                    if (expectedOutput.Contains(outputSequences[i]))
                    {
                        match = true;
                    }
                    else
                    {
                        match = false;
                    }
                }

            }
            return match;
        }

        # endregion Helper Methods
    }
}
