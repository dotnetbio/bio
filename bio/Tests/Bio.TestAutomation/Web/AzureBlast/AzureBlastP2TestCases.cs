/****************************************************************************
 * AzureBlastP2TestCases.cs
 * 
 * This file contains the Azure Blast Web Service P2 test cases.
 * 
******************************************************************************/

using System;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using Bio.Web;
using Bio.Web.Blast;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace Bio.TestAutomation.Web.AzureBlast
{
    /// <summary>
    /// Test Automation code for Bio Azure Blast Web Service and P2 level validations.
    /// </summary>
    public class AzureBlastP2TestCases
    {

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\AzureBlastConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static AzureBlastP2TestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region Azure Blast P2 Test Cases

        /// <summary>
        /// Validate the Azure Blast Service Request status Queued.
        /// Input : Invalid request parameters.
        /// Output : Invalidate request status.
        /// </summary>
        public void InvalidateAzureWebServiceRequestStatus()
        {
            // Gets the search query parameter and their values.
            string queryProgramValue = utilityObj.xmlUtil.GetTextValue(
                Constants.AzureBlastResultsNode, Constants.ProgramValue);
            string queryDatabaseParameter = utilityObj.xmlUtil.GetTextValue(
                Constants.AzureBlastResultsNode, Constants.DatabaseParameter);
            string queryDatabaseValue = utilityObj.xmlUtil.GetTextValue(
                Constants.AzureBlastResultsNode, Constants.DatabaseValue);
            string queryProgramParameter = utilityObj.xmlUtil.GetTextValue(
                Constants.AzureBlastResultsNode, Constants.ProgramParameter);
            string reqId = string.Empty;

            ConfigParameters configParams = new ConfigParameters();
            configParams.UseBrowserProxy = true;
            configParams.DefaultTimeout = 1;
            configParams.RetryInterval = 10;
            configParams.RetryCount = 1;
            configParams.Connection = new Uri(Constants.AzureUri);

            // Create search parameters object.
            BlastParameters searchParams = new BlastParameters();

            // Add mandatory parameter values to search query parameters.
            searchParams.Add(queryProgramParameter, queryProgramValue);
            searchParams.Add(queryDatabaseParameter, queryDatabaseValue);

            IBlastServiceHandler service = null;
            try
            {
                service = new AzureBlastHandler(configParams);

                // Set Service confiruration parameters true.
                service.Configuration = configParams;

                // Get Request identifier from web service.
                //reqId = service.SubmitRequest(seq, searchParams);
            }
            catch (WebException ex)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "AzureWebService P2 : Connection Failed with the error '{0}'", ex.Message));
                Assert.Inconclusive("Test case ignored due to connection failure");
            }

            finally
            {
                if (service != null)
                    ((IDisposable)service).Dispose();
            }
            try
            {
                object responseResults = service.FetchResultsSync(reqId, searchParams);
                Assert.IsNotNull(responseResults);
                Assert.Fail();
            }
            catch (WebException)
            {
                ApplicationLog.WriteLine("AzureWebService P2 : Successfully validated the exception");
            }
        }

        /// <summary>
        /// Validate a Azure Blast Service constructor with invalid parameters.
        /// Input : Invalid service config parameters.
        /// Output : Invalidate Azure web service constructor.
        /// </summary>
        public void InvalidateAzureWebHandlerCtor()
        {
            ConfigParameters configParams = new ConfigParameters();
            configParams.UseBrowserProxy = true;
            configParams.DefaultTimeout = 10;
            configParams.Connection = new Uri(Constants.AzureUri);
            BlastXmlParser parser = new BlastXmlParser();

            // Validate AzureWebService ctor by passing null parser.
            IBlastServiceHandler service = null;
            try
            {
                service = new AzureBlastHandler(null, configParams);
                Assert.IsNotNull(service);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                ApplicationLog.WriteLine(
                    "AzureWebService P2 : Successfully validated the Argument null exception");
            }
            finally
            {
                if (service != null)
                    ((IDisposable)service).Dispose();
            }

            // Validate AzureWebService ctor by passing null configuration parameters.
            try
            {
                service = new AzureBlastHandler(parser, null);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                ApplicationLog.WriteLine(
                    "AzureWebService P2 : Successfully validated the Argument null exception");
            }
            finally
            {
                if (service != null)
                    ((IDisposable)service).Dispose();
            }
        }

        /// <summary>
        /// Validate a Azure Blast Service Request status for invalid Sequence.
        /// Input : Invalid sequence.
        /// Output : Valdiate request status for invalid sequence.
        /// </summary>
        public void InvalidateAzureWebServiceRequestStatusForInvalidConfigPams()
        {
            // Gets the search query parameter and their values.
            string alphabet = utilityObj.xmlUtil.GetTextValue(
                Constants.AzureBlastResultsNode, Constants.AlphabetNameNode);
            string querySequence = utilityObj.xmlUtil.GetTextValue(
                Constants.AzureBlastResultsNode, Constants.QuerySequency);
            string queryDatabaseValue = utilityObj.xmlUtil.GetTextValue(
                Constants.AzureBlastResultsNode, Constants.DatabaseValue);
            string queryProgramValue = utilityObj.xmlUtil.GetTextValue(
                Constants.AzureBlastResultsNode, Constants.ProgramValue);
            string queryDatabaseParameter = utilityObj.xmlUtil.GetTextValue(
                Constants.AzureBlastResultsNode, Constants.DatabaseParameter);
            string queryProgramParameter = utilityObj.xmlUtil.GetTextValue(
                Constants.AzureBlastResultsNode, Constants.ProgramParameter);

            Sequence seq = new Sequence(Utility.GetAlphabet(alphabet), querySequence);

            // Set Service confiruration parameters true.
            AzureBlastHandler service = new AzureBlastHandler();

            // Dispose Azure Blast Handler.
            service.Dispose();

            ConfigParameters configParams = new ConfigParameters();
            configParams.UseBrowserProxy = true;
            configParams.DefaultTimeout = 10;
            configParams.Connection = new Uri(Constants.AzureUri);
            service.Configuration = configParams;

            // Create search parameters object.
            BlastParameters searchParams = new BlastParameters();

            // Add mandatory parameter values to search query parameters.
            searchParams.Add(queryProgramParameter, queryProgramValue);
            searchParams.Add(queryDatabaseParameter, queryDatabaseValue);

            // Get Request identifier from web service.
            try
            {
                service.SubmitRequest(seq, null);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                ApplicationLog.WriteLine(
                    "AzureWebService P2 : Successfully validated the exception");
            }
        }

        /// <summary>
        /// Validate a Azure Blast Service Request without setting any config parameters.
        /// Input : Invalid config parameters.
        /// Output : Invalidate request status.
        /// </summary>
        public void InvalidateAzureWebServiceRequestStatusWithoutConfigPams()
        {
            // Gets the search query parameter and their values.
            string alphabet = utilityObj.xmlUtil.GetTextValue(
                Constants.AzureBlastResultsNode, Constants.AlphabetNameNode);
            string querySequence = utilityObj.xmlUtil.GetTextValue(
                Constants.AzureBlastResultsNode, Constants.QuerySequency);

            Sequence seq = new Sequence(Utility.GetAlphabet(alphabet), querySequence);
            string reqId = string.Empty;

            // Set Service confiruration parameters true.
            AzureBlastHandler service = new AzureBlastHandler();

            // Dispose Azure Blast Handler.
            service.Dispose();

            ConfigParameters configParams = new ConfigParameters();
            configParams.UseBrowserProxy = true;
            configParams.DefaultTimeout = 1;
            configParams.Connection = new Uri(Constants.AzureUri);
            service.Configuration = configParams;
            BlastParameters searchParams = new BlastParameters();

            // Get Request identifier from web service.
            try
            {
                reqId = service.SubmitRequest(seq, searchParams);
                Assert.IsTrue(string.IsNullOrEmpty(reqId));
                Assert.Fail();
            }
            catch (WebException)
            {
                ApplicationLog.WriteLine("AzureWebService P2 : Successfully validated the exception");
            }
        }

        #endregion Azure Blast P2 Test Cases
    }
}
