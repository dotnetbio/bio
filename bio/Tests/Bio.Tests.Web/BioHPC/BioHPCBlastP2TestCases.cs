/****************************************************************************
 * BioHPCBlastP2TestCases.cs
 * 
 * This file contains the BioHPC Blast Web Service P2 test cases.
 * 
******************************************************************************/

using System;
using System.Collections.Generic;
using Bio.TestAutomation.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Util.Logging;
using Bio.Web;
using Bio.Web.Blast;

namespace Bio.TestAutomation.Web.BioHPCBlast
{
    /// <summary>
    /// P2 Test cases for BioHPCBlast
    /// </summary>
    [TestClass]
    public class BioHPCBlastP2TestCases
    {
        #region Enum

        /// <summary>
        /// Submit methods of BioHPC Blast Web Service
        /// </summary>
        enum RequestType
        {
            StrSubmit,
            LstSubmit,
            DnalstSubmit,
            ProteinlstSubmit,
            DnaStrSubmit,
            ProteinStrSubmit,
            FetchSyncUsingDnaSeq,
            FetchSyncUsingProteinSeq,
            FetchASyncUsingDnaSeq,
            FetchASyncUsingProteinSeq,
        }

        #endregion Enum

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\BioHPCTestConfigs.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other 
        /// settings needed for test
        /// </summary>
        static BioHPCBlastP2TestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region BioHPCBlast P2 TestCases
        /// <summary>
        /// Invalidates Constructor BioHPCBlastHandler with null parameters
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateBioHPCBlastHandler()
        {
            try
            {
                IBlastServiceHandler handler = new BioHPCBlastHandler(null, new ConfigParameters());
                Assert.Fail();
            }
            catch (ArgumentNullException anex)
            {
                ApplicationLog.WriteLine("Successfully caught ArgumentNullException : " + anex.Message);
            }

            try
            {
                IBlastParser blastXmlParser = new BlastXmlParser();
                IBlastServiceHandler handler = new BioHPCBlastHandler(blastXmlParser, null);
                Assert.Fail();
            }
            catch(ArgumentNullException anex)
            {
                ApplicationLog.WriteLine("Successfully caught ArgumentNullException : " + anex.Message);
            }
        }

        /// <summary>
        /// Invalidates constroctor with null parameters
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateBioHPCCtor()
        {
            // Gets the search query parameter and their values.
            string alphabetName = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode,
                Constants.AlphabetNameNode);
            string querySequence = string.Empty;
            string queryDatabaseValue = string.Empty;
            string email = string.Empty;
            string queryProgramValue = string.Empty;
            string queryDatabaseParameter = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode,
                Constants.DatabaseParameter);
            string queryProgramParameter = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode,
                Constants.ProgramParameter);
            string expect = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode, Constants.Expectparameter);
            string emailNotify = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode, Constants.EmailNotifyParameterNode);
            string jobName = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode, Constants.JobNameParameterNode);
            string expectValue = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode, Constants.ExpectNode);
            string emailNotifyValue = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode, Constants.EmailNotifyNode);
            string jobNameValue = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode, Constants.JobNameNode);

            Sequence seq = new Sequence(Utility.GetAlphabet(alphabetName),
                querySequence);

            // create BioHPC Blast Web Service object.
            IBlastServiceHandler service = new BioHPCBlastHandler();
            ConfigParameters configParams = new ConfigParameters();
            configParams.EmailAddress = email;
            configParams.Password = string.Empty;
            service.Configuration = configParams;

            BlastParameters searchParams = new BlastParameters();

            // Set Request parameters.
            searchParams.Add(queryDatabaseParameter, queryDatabaseValue);
            searchParams.Add(queryProgramParameter, queryProgramValue);
            searchParams.Add(expect, expectValue);
            searchParams.Add(emailNotify, emailNotifyValue);
            searchParams.Add(jobName, jobNameValue);

            try
            {
                // Create a request without passing sequence.
                string reqId = service.SubmitRequest(seq, searchParams);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteLine("Successfully caught exception : " + ex.Message);
            }
            finally
            {
                if (service != null)
                    ((IDisposable)service).Dispose();
            }

        }

        /// <summary>
        /// Invalidates SubmitRequest with null parameters
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateSubmitRequestWithNullParam()
        {
            // Gets the search query parameter and their values.
            string alphabetName = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode, Constants.AlphabetNameNode);
            string querySequence = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode, Constants.QuerySequency);
            string queryDatabaseValue = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode, Constants.DatabaseValue);
            string email = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode, Constants.EmailAdress);
            string queryProgramValue = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode,
                Constants.ProgramValue);
            string queryDatabaseParameter = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode, Constants.DatabaseParameter);
            string queryProgramParameter = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode, Constants.ProgramParameter);
            string expect = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode, Constants.Expectparameter);
            string emailNotify = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode, Constants.EmailNotifyParameterNode);
            string jobName = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode, Constants.JobNameParameterNode);
            string expectValue = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode, Constants.ExpectNode);
            string emailNotifyValue = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode, Constants.EmailNotifyNode);
            string jobNameValue = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode, Constants.JobNameNode); 
            Sequence seq = new Sequence(Utility.GetAlphabet(alphabetName),
                querySequence);

            // create BioHPC Blast Web Service object.
            using (BioHPCBlastHandler service = new BioHPCBlastHandler())
            {
                ConfigParameters configParams = new ConfigParameters();
                configParams.EmailAddress = email;
                configParams.Password = string.Empty;
                service.Configuration = configParams;

                BlastParameters searchParams = new BlastParameters();

                // Set Request parameters.
                searchParams.Add(queryDatabaseParameter, queryDatabaseValue);
                searchParams.Add(queryProgramParameter, queryProgramValue);
                searchParams.Add(expect, expectValue);
                searchParams.Add(emailNotify, emailNotifyValue);
                searchParams.Add(jobName, jobNameValue);

                try
                {
                    // Create a request  passing sequence as null.
                    seq = null;
                    string reqId = service.SubmitRequest(seq, searchParams);
                    Assert.Fail();
                }
                catch (ArgumentNullException ex)
                {
                    ApplicationLog.WriteLine("Successfully caught ArgumentNullException : " + ex.Message);
                }

                try
                {
                    // Create a request  passing searchParams as null.
                    searchParams = null;
                    string reqId = service.SubmitRequest(seq, searchParams);
                    Assert.Fail();
                }
                catch (ArgumentNullException ex)
                {
                    ApplicationLog.WriteLine("Successfully caught ArgumentNullException : " + ex.Message);
                }

                List<ISequence> seqList = new List<ISequence>();
                try
                {
                    // Create a request  passing sequence as null.
                    seqList = null;
                    string reqId = service.SubmitRequest(seqList, searchParams);
                    Assert.Fail();
                }
                catch (Exception ex)
                {
                    ApplicationLog.WriteLine("Successfully caught Exception : " + ex.Message);
                }

                try
                {
                    // Create a request  passing searchParams as null.
                    searchParams = null;
                    string reqId = service.SubmitRequest(seqList, searchParams);
                    Assert.Fail();
                }
                catch (Exception ex)
                {
                    ApplicationLog.WriteLine("Successfully caught Exception : " + ex.Message);
                }
            }
        }

        #endregion BioHPCBlast P2 TestCases
    }
}
