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
    /// Test Automation code for MumUtil console application
    /// </summary>
    [TestClass]
    public class MumUtilBvtTestCases 
    {
        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\UtilitiesTestsConfig.xml");        

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static MumUtilBvtTestCases()
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
        /// Test case for running MumUtil with basic command line arguments and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMumUtilWithDefaultParametersAndVerbose()
        {
            ValidateMumUtil(Constants.MummerDefaultParamsNodeName, true);
        }
                
        /// <summary>
        /// Test case for running MumUtil with E-Coli data and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMumUtilWithEColiData()
        {
            ValidateMumUtil(Constants.MummerDefaultParamsWithEColiDataNode, false);
        }

                        
        /// <summary>
        /// Test case for running MumUtil with Max match command and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMumUtilWithMaxMatch()
        {
            ValidateMumUtil(Constants.MummerWithMaxMatchNode,false);
        }

        /// <summary>
        /// Validate MumUtil with Ambiguity and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMumUtilWithAmbiguity()
        {
            ValidateMumUtil(Constants.MummerWithAmbiguityNode,false);
        }
        
        /// <summary>
        /// Validate MumUtil with Mum switch and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMumUtilWithMumSwitch()
        {
            ValidateMumUtil(Constants.MummerWithMumSwitchNode,false);
        }

        /// <summary>
        /// Validate MumUtil with the switches length,both,Query position of reverse complement: "/c" and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMumUtilWithLengthAndBoth()
        {
            ValidateMumUtil(Constants.MummerWithLengthAndBothSwitchNode, false);
        }

        /// <summary>
        /// Validates MumUtil Help Output
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMumUtilHelp()
        {
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.MumUtilHelpCommandNodeName);
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            string output = Utility.standardOut;
            string expectedHelpFile = utilityObj.xmlUtil.GetTextValue(
                Constants.HelpValidationNodeName, Constants.MumUtilExpectedHelpNodeName);
            string expectedOutput = File.ReadAllText(expectedHelpFile);
            Assert.IsTrue(output.Contains(expectedOutput.Trim()));
        }

        #endregion Test cases

        # region Helper methods.
        /// <summary>
        /// General method to Validate MumUtil test cases.
        /// </summary>
        /// <param name="nodeName"> Parent node in Xml</param>
        public void ValidateMumUtil(string nodeName,bool verbose)
        {

            ApplicationLog.WriteLine("************************************** MUMUtil with Basic command - Start **************************************");
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CommandNode);

            // Run the Mumutil with the commands updated in the xml.
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            ApplicationLog.WriteLine("************************************** MUMUtil with Basic command - End **************************************");

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
                string[] verboseExpected=expectedVerboseResult.Split(',');
                string actualVerboseString = Utility.standardErr.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "").Trim();

                for (int i = 0; i < verboseExpected.Length; i++)
                {
                    Assert.IsTrue(actualVerboseString.Contains(verboseExpected[i]));
                }
            }
            
            ApplicationLog.WriteLine("MumUtil Console BVT : Successfully validated the results of the command");
            Console.WriteLine("MumUtil Console BVT : Successfully validated the results of the command");
        }

        # endregion Helper methods.
    }
}
