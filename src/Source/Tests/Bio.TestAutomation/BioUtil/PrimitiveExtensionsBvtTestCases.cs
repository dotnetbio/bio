/****************************************************************************
 * PrimitiveExtensionsBvtTestCases.cs
 * 
 * This file contains the PrimitiveExtensions BVT test cases.
 * 
******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bio.TestAutomation.Util;
using Bio.Util;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if (SILVERLIGHT == false)
namespace Bio.TestAutomation.Util
#else
   namespace Bio.SilverLight.TestAutomation
#endif
{
    /// <summary>
    /// Test Automation code for Bio PrimitiveExtensions and BVT level validations.
    /// </summary>
    [TestClass]
    public class PrimitiveExtensionsBvtTestCases
    {
        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static PrimitiveExtensionsBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region Private Variables

        /// <summary>
        /// Declares variables for enforce value.
        /// </summary>
        private bool enforce;

        #endregion Private Variables

        #region PrimitiveExtensions Bvt TestCases

        /// <summary>
        /// Validate Enforce method of PrimitiveExtensions by passing bool value.
        /// Input Data : Valid value
        /// Output Data : Validate of value.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateEnforceForValidValue()
        {
            enforce = true;
            enforce.Enforce();
            Assert.IsTrue(enforce);
            ApplicationLog.WriteLine(string.Concat(
                  "PrimitiveExtensions BVT: Validation of Enforce method for value."));
        }

        /// <summary>
        /// Validate Enforce method of PrimitiveExtensions by passing bool value and message.
        /// Input Data : Valid message.
        /// Output Data : Validate of value.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateEnforceForMessage()
        {
            string errorMessage = utilityObj.xmlUtil.GetTextValue(
                Constants.PrimitiveExtensionsNode,
                Constants.MessageNode);
            enforce = true;
            enforce.Enforce(errorMessage);
            Assert.IsTrue(enforce);
            ApplicationLog.WriteLine(string.Concat(
                  "PrimitiveExtensions BVT: Validation of Enforce method for error message."));
        }

        /// <summary>
        /// Validate Enforce method of PrimitiveExtensions by passing bool value and parameters.
        /// Input Data : Valid message and parameters.
        /// Output Data : Validate of value.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateEnforceForParams()
        {
            string errorMessage = utilityObj.xmlUtil.GetTextValue(
                Constants.PrimitiveExtensionsNode,
                Constants.MessageNode);
            enforce = true;
            enforce.Enforce(errorMessage, Constants.LargeMediumUniqueSymbolCount);
            Assert.IsTrue(enforce);
            ApplicationLog.WriteLine(string.Concat(
                  "PrimitiveExtensions BVT: Validation of Enforce method for params and message."));
        }

        /// <summary>
        /// Validate Enforce method of PrimitiveExtensions by passing bool value and parameters.
        /// Input Data : Valid message and parameters.
        /// Output Data : Validate of value.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateEnforceExceptionForParams()
        {
            string errorMessage = utilityObj.xmlUtil.GetTextValue(
                Constants.PrimitiveExtensionsNode,
                Constants.MessageNode);
            enforce = true;
            enforce.Enforce<Exception>(errorMessage, Constants.LargeMediumUniqueSymbolCount);
            Assert.IsTrue(enforce);
            ApplicationLog.WriteLine(string.Concat(
                  "PrimitiveExtensions BVT: Validation of Enforce method for params and message."));
        }

        /// <summary>
        /// Validate Enforce method of PrimitiveExtensions by passing bool value.
        /// Input Data : Valid value
        /// Output Data : Validate of value.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateEnforceExceptionForValidValue()
        {
            enforce = true;
            enforce.Enforce<Exception>();
            Assert.IsTrue(enforce);
            ApplicationLog.WriteLine(string.Concat(
                  "PrimitiveExtensions BVT: Validation of Enforce method for value."));
        }

        /// <summary>
        /// Validate Enforce method of PrimitiveExtensions by passing bool value and message.
        /// Input Data : Valid message.
        /// Output Data : Validate of value.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateEnforceExceptionForMessage()
        {
            string errorMessage = utilityObj.xmlUtil.GetTextValue(
                Constants.PrimitiveExtensionsNode,
                Constants.MessageNode);
            enforce = true;
            enforce.Enforce<Exception>(errorMessage);
            Assert.IsTrue(enforce);
            ApplicationLog.WriteLine(string.Concat(
                  "PrimitiveExtensions BVT: Validation of Enforce method for error message."));
        }

        #endregion PrimitiveExtensions Bvt TestCases
    }

}
