/****************************************************************************
 * ApplicationLogBvtTestCases.cs
 * 
 * This file contains the ApplicationLog BVT test cases.
 * 
******************************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using Bio.Util.Logging;
using Bio.TestAutomation.Util;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio;
using System.Globalization;

#if (SILVERLIGHT == false)
namespace Bio.TestAutomation.Util.Logging
#else
   namespace Bio.SilverLight.TestAutomation
#endif
{
    /// <summary>
    /// Test Automation code for Bio ApplicationLog and BVT level validations.
    /// </summary>
    [TestClass]
    public class ApplicationLogBvtTestCases
    {

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static ApplicationLogBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region ApplicationLog Bvt TestCases

        /// <summary>
        /// Validate All property.
        /// Input Data : Valid All ApplicationLog.
        /// Output Data : Validate all Alphabet types.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateApplicationLogProperties()
        {
            Assert.IsTrue(ApplicationLog.Autoflush);
            Assert.IsTrue(ApplicationLog.Ready);
            ApplicationLog.Autoflush = false;
            Assert.IsFalse(ApplicationLog.Autoflush);

            ApplicationLog.WriteLine(string.Concat(
                  "ApplicationLog BVT: Validation of All property completed successfully."));
        }

        /// <summary>
        /// Validate Open method of ApplicationLog by passing valid log file.
        /// Input Data : Valid Log File. 
        /// Output Data : Validate File.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateOpenForValidLogFile()
        {
            string output = utilityObj.xmlUtil.GetTextValue(
                Constants.ApplicationLogNode, Constants.ExpectedOutputNode);

            Assert.IsTrue(ApplicationLog.Ready);
            string expectedOutput = ApplicationLog.WriteLine(output);
            Assert.AreEqual(expectedOutput.Trim(), output.Trim());
            Assert.IsTrue(ApplicationLog.Ready);

            ApplicationLog.WriteLine(string.Concat(
                  "ApplicationLog BVT: Validation of Open method for File completed successfully."));
        }

        /// <summary>
        /// Validate Reopen method of ApplicationLog by passing valid log file.
        /// Input Data : Valid Log File. 
        /// Output Data : Validate File.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateReopenForValidLogFile()
        {
            string[] outputs = utilityObj.xmlUtil.GetTextValue(
                Constants.ApplicationLogNode, Constants.ExpectedOutputNode).Split(';');

            //// Open the file for write.
            string expectedOutput = ApplicationLog.WriteLine(outputs[0]);
            Assert.AreEqual(outputs[0].Trim(), expectedOutput.Trim());

            //// ReOpen the file for write.
            ApplicationLog.Reopen("bio.automation.log");
            Assert.IsTrue(ApplicationLog.Ready);
            expectedOutput = ApplicationLog.WriteLine(outputs[1]);
            Assert.AreEqual(expectedOutput.Trim(), outputs[1].Trim());

            ApplicationLog.WriteLine(string.Concat(
                  "ApplicationLog BVT: Validation of ReOpen method for File completed successfully."));
        }

        /// <summary>
        /// Validate Write method of ApplicationLog by passing valid log file.
        /// Input Data : Valid Log File. 
        /// Output Data : Validate File.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateWriteForValidLogFile()
        {
            string output = utilityObj.xmlUtil.GetTextValue(
                 Constants.ApplicationLogNode, Constants.ExpectedOutputNode);

            //// Open the file for write.
            String expectedOutput = ApplicationLog.Write(output);
            Assert.IsTrue(ApplicationLog.Ready);
            Assert.AreEqual(output, expectedOutput);

            ApplicationLog.WriteLine(string.Concat(
                  "ApplicationLog BVT: Validation of Write method for File completed successfully."));
        }

        /// <summary>
        /// Validate Write method with params of ApplicationLog by passing valid log file.
        /// Input Data : Valid Log File. 
        /// Output Data : Validate File.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateWriteParamForValidLogFile()
        {
            string output = utilityObj.xmlUtil.GetTextValue(
                Constants.ApplicationLogNode, Constants.ExpectedOutputNode);
            string[] args = utilityObj.xmlUtil.GetTextValue(
                Constants.ApplicationLogNode, Constants.AdditionalArgumentsNode).Split(';');

            //// Open the file for write.
            String expectedOutput = ApplicationLog.Write(output, args);
            Assert.IsTrue(ApplicationLog.Ready);
            Assert.AreEqual(output, expectedOutput);

            ApplicationLog.WriteLine(string.Concat(
                  "ApplicationLog BVT: Validation of Write with params method for File completed successfully."));
        }

        /// <summary>
        /// Validate WriteTime with params method of ApplicationLog by passing valid log file.
        /// Input Data : Valid Log File. 
        /// Output Data : Validate File.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateWriteTimeParamForValidLogFile()
        {
            string output = utilityObj.xmlUtil.GetTextValue(
                Constants.ApplicationLogNode, Constants.ExpectedOutputNode);
            string[] args = utilityObj.xmlUtil.GetTextValue(
                Constants.ApplicationLogNode, Constants.AdditionalArgumentsNode).Split(';');

            //// Open the file for write.
            string[] expectedOutput = ApplicationLog.WriteTime(output, args).Split(':');
            Assert.AreEqual(4, expectedOutput.Length);
            Assert.AreEqual(output.Trim(), expectedOutput[3].Trim());

            ApplicationLog.WriteLine(string.Concat(
                  "ApplicationLog BVT: Validation of WriteTime method for File completed successfully."));
        }

        #endregion ApplicationLog Bvt TestCases
    }
}
