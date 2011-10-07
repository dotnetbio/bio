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
    /// Test Automation code for MumUtil console application
    /// compares the output on console with the expected output stored in file
    /// </summary>
    [TestClass]
    public class MumUtilP1TestCases
    {
        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\UtilitiesTestsConfig.xml");        

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static MumUtilP1TestCases()
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
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateMumUtilWithDefaultParameters()
        {
            ValidateMumUtil(Constants.MummerDefaultParamsNodeName);
        }
                
        /// <summary>
        /// Test case for running MumUtil with E-Coli data and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateMumUtilWithEColiData()
        {
            ValidateMumUtil(Constants.MummerDefaultParamsWithEColiDataNode);
        }

                        
        /// <summary>
        /// Test case for running MumUtil with Max match command and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateMumUtilWithMaxMatch()
        {
            ValidateMumUtil(Constants.MummerWithMaxMatchNode);
        }

        /// <summary>
        /// Validate MumUtil with Ambiguity and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateMumUtilWithAmbiguity()
        {
            ValidateMumUtil(Constants.MummerWithAmbiguityNode);
        }
        
        /// <summary>
        /// Validate MumUtil with Mum switch and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateMumUtilWithMumSwitch()
        {
            ValidateMumUtil(Constants.MummerWithMumSwitchNode);
        }

        /// <summary>
        /// Validate MumUtil with the switches length,both,Query position of reverse complement: "/c" and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateMumUtilWithLengthAndBoth()
        {
            ValidateMumUtil(Constants.MummerWithLengthAndBothSwitchNode);
        }

        #endregion Test cases

        # region Helper methods.
        /// <summary>
        /// General method to Validate MumUtil test cases.
        /// </summary>
        /// <param name="nodeName"> Parent node in Xml</param>
        public void ValidateMumUtil(string nodeName)
        {

            ApplicationLog.WriteLine("************************************** MUMUtil with Basic command - Start **************************************");
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ConsoleCommandNode);

            // Run the Mumutil with the commands updated in the xml.
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            ApplicationLog.WriteLine("************************************** MUMUtil with Basic command - End **************************************");

            // Gets the output file for validation
            string expectedOutputFile = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedOutputFileNode);

            // Compares the expected output file with the result on console
            string expectedOutput = File.ReadAllText(expectedOutputFile).Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace(">", "").Trim();
            string actualOutput = Utility.commandOutput.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace(">", "").Trim();
            Assert.IsTrue(actualOutput.Contains(expectedOutput));
                                                
            ApplicationLog.WriteLine("MumUtil Console P1 : Successfully validated the results of the command");
            Console.WriteLine("MumUtil Console P1 : Successfully validated the results of the command");
        }

        # endregion Helper methods.
    }
}
