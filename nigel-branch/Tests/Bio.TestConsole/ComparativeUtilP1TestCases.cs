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
    /// matches output on console with expected output stored in file
    /// </summary>
    [TestClass]
    public class ComparativeUtilP1TestCases
    {
        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\UtilitiesTestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        public ComparativeUtilP1TestCases()
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
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateComparativeUtilWithDefaultParameters()
        {
            ValidateComparativeUtil(Constants.ComparativeUtilDefaultParamsNodeName);
        }

        /// <summary>
        /// Runs ComparativeUtil using ecoli data with basic command line arguments and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateComparativeUtilWithEcoliDefaultParameters()
        {
            ValidateComparativeUtil(Constants.ComparativeUtilEcoliDefaultParamsNodeName);
        }

        /// <summary>
        /// Runs ComparativeUtil with scaffold using ecoli data with basic command line arguments and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateComparativeUtilWithScaffoldEcoli()
        {
            ValidateComparativeUtil(Constants.ComparativeUtilWithScaffoldEcoliNodeName);
        }

        /// <summary>
        /// Runs ComparativeUtil with KmerLength and MumLength Parameters and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateComparativeUtilWithKmerAndMumLengths()
        {
            ValidateComparativeUtil(Constants.ComparativeUtilWithKmerAndMumNodeName);
        }

        /// <summary>
        /// Runs ComparativeUtil with meanlengthofinsert and validates expected result file
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateComparativeUtilWithMeanLengthInsert()
        {
            ValidateComparativeUtil(Constants.ComparativeUtilWithMeanLengthInsertNodeName);
        }

        #endregion Test cases

        #region Helper Methods

        /// <summary>
        /// General method to Validate ComparativeUtil test cases.
        /// </summary>
        /// <param name="nodeName">Parent node in XML</param>
        /// <param name="verbose">Indicates whether verbose switch is used or not</param>
        public void ValidateComparativeUtil(string nodeName)
        {
            ApplicationLog.WriteLine("************************************** ComparativeUtil with Basic command - Start **************************************");
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ConsoleCommandNode);

            // Run the ComparativeUtil with the commands updated in the xml.
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            ApplicationLog.WriteLine("************************************** ComparativeUtil with Basic command - End **************************************");

            // Gets the output file for validation
            string expectedOutputFile = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedOutputFileNode);

            // Compares the expected output file with the result on console
            string expectedOutput = File.ReadAllText(expectedOutputFile).Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace(">", "").Trim();
            string actualOutput = Utility.commandOutput.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace(">", "").Trim();
            Assert.IsTrue(actualOutput.Contains(expectedOutput));


            ApplicationLog.WriteLine("ComparativeUtil Console P1 : Successfully validated the results of the command");
            Console.WriteLine("ComparativeUtil Console P1 : Successfully validated the results of the command");
        }

        #endregion Helper Methods
    }
}
