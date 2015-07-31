/****************************************************************************
 * TraceP2TestCases.cs
 * 
 * This file contains the Trace P2 test cases.
 * 
******************************************************************************/

using System;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;


#if (SILVERLIGHT == false)
namespace Bio.TestAutomation.Util.Logging
#else
   namespace Bio.SilverLight.TestAutomation
#endif
{
    /// <summary>
    /// Test Automation code for Bio Trace and P2 level validations.
    /// </summary>
    [TestClass]
    public class TraceP2TestCases
    {

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static TraceP2TestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region Trace P2 TestCases

        /// <summary>
        /// Validate Report method of Trace by passing Message.   
        /// Input Data : Invalid Message(NUll Message).
        /// Output Data : Validate of Exception.
        /// </summary>
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateTraceReportForNullMessage()
        {
            string data = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilTraceNode,
                Constants.DataParameterNode);
            string context = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilTraceNode,
                Constants.ContextParameterNode);

            try
            {
                Trace.Report(context, null, data);
                Assert.Fail();
            }
            catch (ArgumentNullException exception)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Trace P2: Trace Report Null exception was validated successfully {0}",
                exception.Message));
            }
        }

        /// <summary>
        /// Validate Report method of Trace by passing Message.   
        /// Input Data : Invalid Data(NUll Data).
        /// Output Data : Validate of Exception.
        /// </summary>
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateTraceReportForNullData()
        {
            string message = utilityObj.xmlUtil.GetTextValue(
               Constants.UtilTraceNode,
               Constants.MessageNode);
            string context = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilTraceNode,
                Constants.ContextParameterNode);

            try
            {
                Trace.Report(context, message, null);
                Assert.Fail();
            }
            catch (ArgumentNullException exception)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Trace P2: Trace Report Null exception was validated successfully {0}",
                exception.Message));
            }
        }

        /// <summary>
        /// Validate Report method of Trace by passing Message.   
        /// Input Data : Invalid Context(NUll Context).
        /// Output Data : Validate of Exception.
        /// </summary>
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateTraceReportForNullContext()
        {
            string message = utilityObj.xmlUtil.GetTextValue(
               Constants.UtilTraceNode,
               Constants.MessageNode);
            string data = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilTraceNode,
                Constants.DataParameterNode);

            try
            {
                Trace.Report(null, message, data);
                Assert.Fail();
            }
            catch (ArgumentNullException exception)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null, "Trace P2: Trace Report Null exception was validated successfully {0}", 
                    exception.Message));
            }
        }

        /// <summary>
        /// Validate Report method of Trace by passing Message.   
        /// Input Data : Invalid Message(Empty Message).
        /// Output Data : Validate of Exception.
        /// </summary>
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateTraceReportForEmptyMessage()
        {
            string context = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilTraceNode,
                Constants.ContextParameterNode);
            string data = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilTraceNode,
                Constants.DataParameterNode);

            try
            {
                Trace.Report(context, string.Empty, data);
                Assert.Fail();
            }
            catch (ArgumentNullException exception)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Trace P2: Trace Report Null exception was validated successfully {0}",
                exception.Message));
            }
        }

        #endregion Trace P2 TestCases
    }
}
