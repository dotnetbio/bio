/****************************************************************************
 * EbiBlastP2TestCases.cs
 * 
 * This file contains the Ebi Blast Web Service P2 test cases.
 * 
******************************************************************************/

using System;

using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using Bio.Web;
using Bio.Web.Blast;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using Bio;

namespace Bio.TestAutomation.Web.EbiBlast
{
    /// <summary>
    /// Test Automation code for Bio Ebi Blast Web Service and P2 level validations.
    /// </summary>
    [TestClass]
    public class EbiBlastP2TestCases
    {

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static EbiBlastP2TestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region EbiBlast P2 Test Cases

        /// <summary>
        /// Invalidate Ebi Web Service by passing null config parameters to Ebi constructor.
        /// Input Data : Null config parameters
        /// Output Data : Invalid results  
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateBlastResultsUsingConstructorPam()
        {
            // create Ebi Blast service object.
            ConfigParameters configParams = null;

            IBlastServiceHandler service = null;
            // Validate EbiWebService ctor by passing null parser.
            try
            {
                service = new EbiWuBlastHandler(configParams);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                ApplicationLog.WriteLine(
                    "EbiWebService P2 : Successfully validated the Argument null exception");
                Console.WriteLine(
                    "EbiWebService P2 : Successfully validated the Argument null exception");
            }
            finally
            {
                if (service != null)
                    ((IDisposable)service).Dispose();
            }
        }

        /// <summary>
        /// InValidate Ebi Web Service by passing null config or null Blast parameters to Ebi constructor.
        /// Input Data : Null config parameters or null Blast parameters
        /// Output Data : Invalid results  
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateBlastResultsUsingConstructorPams()
        {
            // create Ebi Blast service object.
            ConfigParameters configParams = new ConfigParameters();
            configParams.UseBrowserProxy = true;

            EbiWuBlastHandler service = null;
            // Validate EbiWebService ctor by passing null parser.
            try
            {
                service = new EbiWuBlastHandler(null, configParams);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                ApplicationLog.WriteLine(
                    "EbiWebService P2 : Successfully validated the Argument null exception");
                Console.WriteLine(
                    "EbiWebService P2 : Successfully validated the Argument null exception");
            }

            // Validate EbiWebService ctor by passing null config.
            try
            {
                service = new EbiWuBlastHandler(new BlastXmlParser(), null);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                ApplicationLog.WriteLine(
                    "EbiWebService P2 : Successfully validated the Argument null exception");
                Console.WriteLine(
                    "EbiWebService P2 : Successfully validated the Argument null exception");
            }
            finally
            {
                if (service != null)
                    service.Dispose();
            }
        }

        /// <summary>
        /// Invalidate service meta data by passing null.
        /// Input Data : Null data
        /// Output Data : Invalid results
        /// </summary>
        /// Suppressing the Error "DoNotCatchGeneralExceptionTypes" because the exception is being thrown by DEV code
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateServiceMetaData()
        {
            EbiWuBlastHandler service = null;
            // Validate ServiceMeta ctor by passing null config.
            try
            {
                service = new EbiWuBlastHandler();
                service.GetServiceMetadata(null);
                Assert.Fail();
            }
            catch (Exception)
            {
                ApplicationLog.WriteLine(
                    "EbiWebService P2 : Successfully validated the exception");
                Console.WriteLine(
                    "EbiWebService P2 : Successfully validated the exception");
            }
            finally
            {
                if (service != null)
                    ((IDisposable)service).Dispose();
            }

        }

        /// <summary>
        /// Invalidate Cancel request by passing null request identifier.
        /// Input Data :Invalid Request Identifier.
        /// Output Data : Invalid results 
        /// </summary>
        /// Suppressing the Error "DoNotCatchGeneralExceptionTypes" because the exception is being thrown by DEV code
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateEbiCancelRequest()
        {
            IBlastServiceHandler service = null;
            // Validate ServiceMeta ctor by passing null config.
            try
            {
                service = new EbiWuBlastHandler();
                service.CancelRequest(null);
                Assert.Fail();
            }
            catch (Exception)
            {
                ApplicationLog.WriteLine(
                    "EbiWebService P2 : Successfully validated the exception");
                Console.WriteLine(
                    "EbiWebService P2 : Successfully validated the exception");
            }
            finally
            {
                if (service != null)
                    ((IDisposable)service).Dispose();
            }

        }

        /// <summary>
        /// Validate a Ebi Blast Service Request without setting any config parameters.
        /// Input : Invalid config parameters.
        /// Output : Invalidate request status.
        /// </summary>
        /// Suppressing the Error "DoNotCatchGeneralExceptionTypes" because the exception is being thrown by DEV code
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateEbiWebServiceRequestStatusWithoutConfigPams()
        {
            // Gets the search query parameter and their values.
            string alphabet = utilityObj.xmlUtil.GetTextValue(
                Constants.EbiBlastResultsNode, Constants.AlphabetNameNode);
            string querySequence = utilityObj.xmlUtil.GetTextValue(
                Constants.EbiBlastResultsNode, Constants.QuerySequency);

            Sequence seq = new Sequence(Utility.GetAlphabet(alphabet),
                querySequence);

            // Set Service confiruration parameters true.
            EbiWuBlastHandler service = new EbiWuBlastHandler();

            // Dispose Ebi Blast Handler.
            service.Dispose();

            ConfigParameters configParams = new ConfigParameters();
            configParams.UseBrowserProxy = true;
            service.Configuration = configParams;
            BlastParameters searchParams = new BlastParameters();

            // Get Request identifier from web service.
            try
            {
                service.SubmitRequest(seq, searchParams);
                Assert.Fail();
            }
            catch (Exception)
            {
                ApplicationLog.WriteLine(
                    "EbiWebService P2 : Successfully validated the exception");
                Console.WriteLine(
                    "EbiWebService P2 : Successfully validated the exception");
            }
        }

        #endregion EbiBlast P2 Test Cases
    }
}
