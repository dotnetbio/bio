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
    public class ComparativeUtilitiesP1TestCases
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
        static ComparativeUtilitiesP1TestCases()
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
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateRepeatResolutionUtilWithDefaultParams()
        {            
            ValidateComparativeUtilities(Constants.RepeatResolutionUtilNode);
        }

        /// <summary>
        /// Validate LayoutRefinementUtil with default params on E-Coli data and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateLayoutRefinementUtilWithDefaultParams()
        {
            ValidateComparativeUtilities(Constants.LayoutRefinementUtilNode);
        }

        /// <summary>
        /// Validate ConsensusUtil with default params on E-Coli data and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateConsensusUtilWithDefaultParams()
        {
            ValidateComparativeUtilities(Constants.ConsensusUtilNode);
        }

        /// <summary>
        /// Validate ScaffoldUtil with default params on E-Coli data and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateScaffoldUtilWithDefaultParams()
        {
            ValidateComparativeUtilities(Constants.ScaffoldUtilNode);
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
            utilCommandString = Constants.ConsoleCommandNode;

            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                     nodeName, utilCommandString);

            // Run the Utilities with the commands updated in the xml.
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            ApplicationLog.WriteLine("************************************** {0} with Basic command - End **************************************", nodeName);

            // Gets the output file for validation
            string expectedOutputFile = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedOutputFileNode);

            // Compares the expected output file with the result on console
            string expectedOutput = File.ReadAllText(expectedOutputFile);
            if (nodeName == Constants.ScaffoldUtilNode)
            {
                string[] expectedSequences = File.ReadAllLines(expectedOutputFile);
                expectedOutput = expectedOutput.Replace(expectedSequences[0], "");
            }
            expectedOutput = expectedOutput.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace(">", "").Trim();
            string actualOutput = Utility.commandOutput.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace(">", "").Trim();
            Assert.IsTrue(actualOutput.Contains(expectedOutput));
            ApplicationLog.WriteLine("{0} Console P1 : Successfully validated the results of the command", nodeName);
            Console.WriteLine("{0} Console P1 : Successfully validated the results of the command", nodeName);
        }

        # endregion Helper Methods
    }
}
