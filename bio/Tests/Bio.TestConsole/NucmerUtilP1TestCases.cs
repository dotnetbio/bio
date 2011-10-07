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
    /// Test automation code for NucmerUtil console application
    /// matches output on console with expected output stored in file
    /// </summary>
    [TestClass]
    public class NucmerUtilP1TestCases
    {
        #region Global Variables

         Utility utilityObj = new Utility(@"TestUtils\UtilitiesTestsConfig.xml");        

         #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
         static NucmerUtilP1TestCases()
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
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNucmerUtilWithDefaultParametersAndVerbose()
        {
            ValidateNucmerUtil(Constants.NucmerUtilDefaultParamsNodeName);
        }

        /// <summary>
        /// Validate NucmerUtil with E-Coli data and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNucmerUtilWithEColiData()
        {
            ValidateNucmerUtil(Constants.NucmerUtilWithEColiDataNode);
        }       

        /// <summary>
        /// Validate NucmerUtil with switch '-l','-m','-c','d' and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNucmerUtilWithLengthAnchorClusterLengthAndDiagonalSwitches()
        {
            ValidateNucmerUtil(Constants.NucmerUtilLengthAnchorClusterLengthAndDiagonalSwitchesNode);
        }

        /// <summary>
        /// Validate NucmerUtil with switch '-b','-n','-r','x' and validate the expected results file.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNucmerUtilWithManySwitches()
        {
            ValidateNucmerUtil(Constants.NucmerUtilWithManySwitchesNode);
        }

        #endregion Test cases

        # region Helper Methods
        
        /// <summary>
        /// General method to Validate NucmerUtil test cases.
        /// </summary>        
        public void ValidateNucmerUtil(string nodeName)
        {
            ApplicationLog.WriteLine("************************************** NucmerUtil with Basic command - Start **************************************");
            string utilCommand = utilityObj.xmlUtil.GetTextValue(
                     nodeName, Constants.ConsoleCommandNode);

            // Run the NucmerUtil with the commands updated in the xml.
            Utility.RunProcess(@".\TestUtils\RunUtil.cmd", utilCommand);
            ApplicationLog.WriteLine("************************************** NucmerUtil with Basic command - End **************************************");

            // Gets the output file for validation
            string expectedOutputFile = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedOutputFileNode);

            // Compares the expected output file with the result on console
            string expectedOutput = File.ReadAllText(expectedOutputFile).Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace(">", "").Trim();
            string actualOutput = Utility.commandOutput.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace(">", "").Trim();
            Assert.IsTrue(actualOutput.Contains(expectedOutput));

            ApplicationLog.WriteLine("NucmerUtil Console P1 : Successfully validated the results of the command");
            Console.WriteLine("NucmerUtil Console P1 : Successfully validated the results of the command");
        }

        # endregion Helper Methods       
    }
}
