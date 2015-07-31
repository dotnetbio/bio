/****************************************************************************
 * TraceBvtTestCases.cs
 * 
 * This file contains the Trace BVT test cases.
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
using Bio.Util;

#if (SILVERLIGHT == false)
namespace Bio.TestAutomation.Util.Logging
#else
   namespace Bio.SilverLight.TestAutomation
#endif
{
    /// <summary>
    /// Test Automation code for Bio Trace and BVT level validations.
    /// </summary>
    [TestClass]
    public class TraceBvtTestCases
    {

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static TraceBvtTestCases()
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
        /// Contains Messages.
        /// </summary>
        private string message;
        /// <summary>
        /// Contains context.
        /// </summary>
        private string context;

        /// <summary>
        /// Contains Data.
        /// </summary>
        private string data;

        #endregion Private Variables

        #region Trace Bvt TestCases

        /// <summary>
        /// Validate All properties of Trace.
        /// Input Data : Valid All Trace.
        /// Output Data : Validate properties.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateTraceProperties()
        {
            Assert.AreEqual(1, Convert.ToInt32(Trace.SeqWarnings));
            Assert.AreEqual(2, Convert.ToInt32(Trace.AssemblyDetails));
            Assert.AreEqual(20, Trace.DefaultMaxMessages);

            ApplicationLog.WriteLine(string.Concat(
                  "Trace BVT: Validation of all Trace properties completed successfully."));
        }

        /// <summary>
        /// Validate Report method of Trace by passing TraceMessage.
        /// Input Data : Valid TraceMessage.
        /// Output Data : Validate Trace Message.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateTraceReportForTraceMessage()
        {
            message = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilTraceNode,
                Constants.MessageNode);
            data = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilTraceNode,
                Constants.DataParameterNode);
            context = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilTraceNode,
                Constants.ContextParameterNode);
            TraceMessage traceMessage = new TraceMessage(context, message, data);

            Trace.Report(traceMessage);

            ////Validate Trace Message.
            TraceMessage queueMsg = Trace.GetMessage(0);
            Assert.IsNotNull(queueMsg);
            ValidateTraceMessages(traceMessage, queueMsg);

            ApplicationLog.WriteLine(string.Concat(
                 "Trace BVT: Validation of Report method for tracemessage completed successfully."));
        }

        /// <summary>
        /// Validate LatestMessage method of Trace by passing TraceMessage.
        /// Input Data : Valid TraceMessage.
        /// Output Data : Validate Trace Message.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateTraceLatestMessage()
        {
            message = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilTraceNode,
                Constants.MessageNode);
            data = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilTraceNode,
                Constants.DataParameterNode);
            context = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilTraceNode,
                Constants.ContextParameterNode);
            TraceMessage traceMessage = new TraceMessage(context, message, data);

            Trace.Report(traceMessage);

            ////Validate Trace Message.
            TraceMessage queueMsg = Trace.LatestMessage();
            Assert.IsNotNull(queueMsg);
            ValidateTraceMessages(traceMessage, queueMsg);

            ApplicationLog.WriteLine(string.Concat(
                  "Trace BVT: Validation of Latest method for tracemessage completed successfully."));
        }

        /// <summary>
        /// Validate Report method of Trace by passing Message.   
        /// Input Data : Valid Message.
        /// Output Data : Validate Trace Message.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateTraceLatestMessageForMessage()
        {
            message = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilTraceNode,
                Constants.MessageNode);

            TraceMessage traceMessage = new TraceMessage(String.Empty, message, String.Empty);
            Trace.Report(message);

            ////Validate Trace Message.
            TraceMessage queueMsg = Trace.GetMessage(0);
            Assert.IsNotNull(queueMsg);
            ValidateTraceMessages(traceMessage, queueMsg);

            ApplicationLog.WriteLine(string.Concat(
                  "Trace BVT: Validation of Report method for message completed successfully."));
        }

        /// <summary>
        /// Validate Report method of Trace by passing Message.   
        /// Input Data : Valid Message.
        /// Output Data : Validate Trace Message.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateTraceReportForParams()
        {
            message = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilTraceNode,
                Constants.MessageNode);
            data = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilTraceNode,
                Constants.DataParameterNode);
            context = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilTraceNode,
                Constants.ContextParameterNode);
            TraceMessage traceMessage = new TraceMessage(context, message, data);
            Trace.Report(context, message, data);

            ////Validate Trace Message.
            TraceMessage queueMsg = Trace.GetMessage(0);
            Assert.IsNotNull(queueMsg);
            ValidateTraceMessages(traceMessage, queueMsg);

            ApplicationLog.WriteLine(string.Concat(
                  "Trace BVT: Validation of Report method for context, message and data completed successfully."));
        }

        /// <summary>
        /// Validate Want method of Trace by passing flag.   
        /// Input Data : Valid flag, encoded in a single bit in a ulong.
        /// Output Data : Validate flag.
        /// </summary>
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateTraceWant()
        {
            ulong traceSetting = 0x1;
            Trace.Set(traceSetting);

            bool check = Trace.Want(traceSetting);
            Assert.IsTrue(check);

            Trace.Clear(traceSetting);
            check = Trace.Want(traceSetting);
            Assert.IsFalse(check);

            ApplicationLog.WriteLine(string.Concat(
                  "Trace BVT: Validation of Want method for trace setting completed successfully."));
        }

        #endregion Trace Bvt TestCases

        #region Supporting Method

        /// <summary>
        /// General Method to validate Data.
        /// </summary>
        /// <param name="traceData">Data created using trace class.</param>
        /// <param name="traceMessageData">Data created using trace message class.</param>
        private static void ValidateTraceMessages(TraceMessage traceMessageData, TraceMessage traceData)
        {
            PrivateObject readTrace = new PrivateObject(traceData);
            PrivateObject readTraceMessage = new PrivateObject(traceMessageData);

            Assert.AreEqual(readTrace.GetField(Constants.MessageNode), readTraceMessage.GetField(Constants.MessageNode));
            Assert.AreEqual(readTrace.GetField(Constants.ContextParameterNode), readTraceMessage.GetField(Constants.ContextParameterNode));
            Assert.AreEqual(readTrace.GetField(Constants.DataParameterNode), readTraceMessage.GetField(Constants.DataParameterNode));
        }

        #endregion
    }
}
