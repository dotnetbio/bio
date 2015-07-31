/****************************************************************************
 * BioHPCBlastBvtTestCases.cs
 * 
 * This file contains the BioHPC Blast Web Service BVT test cases.
 * 
******************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using Bio.TestAutomation.Util;
using Bio;
using Bio.Util.Logging;
using Bio.Web;
using Bio.Web.Blast;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Runtime.Serialization.Formatters.Binary;
using System.Net;

namespace Bio.TestAutomation.Web.BioHPCBlast
{
    /// <summary>
    /// Test Automation code for Bio BioHPC Blast Web Service 
    /// and BVT level validations.
    /// </summary>
    [TestClass]
    public class BioHPCBlastBvtTestCases
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
        static BioHPCBlastBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region BioHPCBlast Bvt TestCases

        /// <summary>
        /// Validate BioHPC Blast Web Service Constructor
        /// </summary>
        /// Input data : Valid alphabet and sequence
        /// OutPut data : Validate Request Id
        /// The test case is commented as the Web service is not responding at the time of release
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateBioHPCCtor()
        {
            // Gets the search query parameter and their values.
            string alphabetName = utilityObj.xmlUtil.GetTextValue(Constants.BioHPCAsynchronousResultsNode, Constants.AlphabetNameNode);
            string querySequence = utilityObj.xmlUtil.GetTextValue(Constants.BioHPCAsynchronousResultsNode, Constants.QuerySequency);
            string queryDatabaseValue = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode,
                Constants.DatabaseValue);
            string email = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode,
                Constants.EmailAdress);
            string queryProgramValue = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode,
                Constants.ProgramValue);
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
            //configParams.UseBrowserProxy = true;
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
                Assert.IsNotNull(reqId);
            }
            catch (WebException ex)
            {
                Assert.Fail();
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "BioHPC Blast Bvt : Connection not successful with error '{0}'",
                    ex.Message));
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "BioHPC Blast Bvt : Connection not successful with error '{0}'",
                    ex.Message));
            }
            finally
            {
                if (service != null)
                    ((IDisposable)service).Dispose();
            }
        }

        /// <summary>
        /// Validate fetching results asynchronous for DNA
        /// Input Data :Valid query sequence, email address, program and database value.
        /// Output Data : Validation of blast results by asynchronous fetching.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0"),
        TestCategory("Web")]
        [Ignore]//Test never seems to work on machine, not sure whate it is for...
        public void ValidateAsyncDnaResultsFetch()
        {
            ValidateBioHPCBlastResultsFetch(
                Constants.BioHPCAsynchronousResultsForDnaNode,
                false);
        }

        /// <summary>
        /// Validate fetching results Synchronous for DNA
        /// Input Data :Valid query sequence, email address, program and database value.
        /// Output Data : Validation of blast results by synchronous fetching.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSyncDnaResultsFetch()
        {
            ValidateBioHPCBlastResultsFetch(
                Constants.BioHPCAsynchronousResultsForDnaNode,
                 true);
        }

        /// <summary>
        /// Validate fetching results asynchronous for Protein 
        /// Input Data :Valid query sequence, email address, program and database value.
        /// Output Data : Validation of blast results by asynchronous fetching.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateAsyncProteinResultsFetch()
        {
            ValidateBioHPCBlastResultsFetch(
                Constants.BioHPCAsynchronousResultsForProteinNode,
                false);
        }

        /// <summary>
        /// Validate fetching results Synchronous for Protein
        /// Input Data :Valid query sequence, email address, program and database value.
        /// Output Data : Validation of blast results by synchronous fetching.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSyncProteinResultsFetch()
        {
            ValidateBioHPCBlastResultsFetch(
                Constants.BioHPCAsynchronousResultsForProteinNode,
                  true);
        }

        /// <summary>
        /// Validate Request status returned from BioHPC Blast Web Service 
        /// by passing request Identifier for DNA  sequence.
        /// Input Data :Valid search query, Database value and program value and email adress.
        /// Output Data : Validation of GetRequestStatus() method.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0"),TestCategory("Web")]
        [Ignore]
        public void ValidateBioHPCGetRequestStatusMethodForDna()
        {
            ValidateBioHPCGeneralGetRequestStatusMethod(
                Constants.BioHPCAsynchronousResultsForDnaNode, "Dna");
        }

        /// <summary>
        /// Validate Request status returned from BioHPC Blast Web Service
        /// by passing request Identifier for Protein  sequence.
        /// Input Data :Valid search query, Database value and program value and email adress.
        /// Output Data : Validation of GetRequestStatus() method.
        /// </summary>
        /// The test case is commented as the Web service is not responding at the time of release
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateBioHPCGetRequestStatusMethodForProtein()
        {
            ValidateBioHPCGeneralGetRequestStatusMethod(
                Constants.BioHPCAsynchronousResultsForProteinNode, "Protein");
        }

        /// <summary>
        /// Validate Cancelling Submitted request for Dna Sequence query.
        /// Input Data : Dna Sequence Query.
        /// Output Data : Validation of Cancelling Submitted job.
        /// </summary>
        /// The test case is commented as the Web service is not responding at the time of release
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateCancelRequestForDnaSequence()
        {
            ValidateCancelSubmittedJob(
                Constants.BioHPCBlastDnaSequenceCancelParametersNode,
                RequestType.StrSubmit);
        }

        /// <summary>
        /// Validate Cancelling Submitted request for Protein Sequence query.
        /// Input Data : Protein Sequence Query.
        /// Output Data : Validation of Cancelling Submitted job.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateCancelRequenstForProteinSequence()
        {
            ValidateCancelSubmittedJob(
                Constants.BioHPCAsynchronousResultsForProteinNode,
                RequestType.StrSubmit);
        }

        /// <summary>
        /// Validate Cancelling Submitted request for list of Dna Sequence query.
        /// Input Data : Dna Sequence Query.
        /// Output Data : Validation of Cancelling Submitted job.
        /// </summary>
        /// The test case is commented as the Web service is not responding at the time of release
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSubmitRequestForDnaSequence()
        {
            ValidateCancelSubmittedJob(
                Constants.BioHPCBlastDnaSequenceCancelParametersNode,
                RequestType.LstSubmit);
        }

        /// <summary>
        /// Validate Cancelling Submitted request for list of Protein Sequence query.
        /// Input Data : Protein Sequence Query.
        /// Output Data : Validation of Cancelling Submitted job.
        /// </summary>
        /// The test case is commented as the Web service is not responding at the time of release
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSubmitRequestForProteinSequence()
        {
            ValidateCancelSubmittedJob(
                Constants.BioHPCBlastProteinSequenceCancelParametersNode,
                RequestType.LstSubmit);
        }

        /// <summary>
        /// Validate BioHPC Blast Web Service Properties.
        /// Input Data : Valid Config Parameter.
        /// Output Data : Validation of BioHPC Blast Web Service properties.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateBioHPCBlastWebServiceProperties()
        {
            ConfigParameters configParams = new ConfigParameters();
            configParams.UseBrowserProxy = true;

            BioHPCBlastHandler service = null;
            try
            {
                service = new BioHPCBlastHandler(configParams);

                // Validate BioHPC Blast Web Service properties.
                Assert.AreEqual(Constants.BioHPCWebServiceDescription,
                    service.Description);
                Assert.AreEqual(Constants.BioHPCWebServiceName,
                    service.Name);
                Assert.IsNotNull(service.Configuration);
                Assert.IsNotNull(service.Parser);

                ApplicationLog.WriteLine(
                    "BioHPC Blast Bvt : Successfully validated the BioHPC Blast WebService Properties");
                Console.WriteLine(
                    "BioHPC Blast Bvt : Successfully validated the BioHPC Blast WebService Properties");
            }
            finally
            {
                if (service != null)
                    ((IDisposable)service).Dispose();
            }
        }

        /// <summary>
        /// Validates GetServiceMetadata
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGetServiceMetadata()
        {
            BioHPCBlastHandler service = null;
            try
            {
                service = new BioHPCBlastHandler();
                service.Configuration = new ConfigParameters();
                service.Configuration.EmailAddress = Constants.EmailForWS;
                service.Configuration.Password = "";
                string kind = BioHPCBlastHandler.MetadataFilter;
                IList<string> info = service.GetServiceMetadata(kind);
                Assert.IsNotNull(info);
                kind = BioHPCBlastHandler.MetadataDatabasesDna;
                info = service.GetServiceMetadata(kind);
                Assert.IsNotNull(info);
                kind = BioHPCBlastHandler.MetadataDatabasesProt;
                info = service.GetServiceMetadata(kind);
                Assert.IsNotNull(info);
                kind = BioHPCBlastHandler.MetadataFormats;
                info = service.GetServiceMetadata(kind);
                Assert.IsNotNull(info);
                kind = BioHPCBlastHandler.MetadataMatrices;
                info = service.GetServiceMetadata(kind);
                Assert.IsNotNull(info);
                kind = BioHPCBlastHandler.MetadataPrograms;
                info = service.GetServiceMetadata(kind);
                Assert.IsNotNull(info);
            }
            finally
            {
                if (service != null)
                    ((IDisposable)service).Dispose();
            }            
        }

        /// <summary>
        /// Validates ProcessRequestThread
        /// </summary>
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateProcessRequestThread()
        {
            // Gets the search query parameter and their values.
            string alphabetName = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode, Constants.AlphabetNameNode);
            string querySequence = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode, Constants.QuerySequency);
            string queryDatabaseValue = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode, Constants.DatabaseValue);
            string email = Constants.EmailForWS;
            string queryProgramValue = utilityObj.xmlUtil.GetTextValue(
                Constants.BioHPCAsynchronousResultsNode, Constants.ProgramValue);
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

            // Set Blast Parameters
            Sequence seq = new Sequence(Utility.GetAlphabet(alphabetName),
                querySequence);
                BioHPCBlastHandler service = new BioHPCBlastHandler();
            try
            {
                ConfigParameters configParameters = new ConfigParameters();
                configParameters.EmailAddress = email;
                configParameters.Password = String.Empty;
                configParameters.UseBrowserProxy = true;
                service.Configuration = configParameters;

                service.RequestCompleted += new EventHandler<BlastRequestCompletedEventArgs>(service_RequestCompleted);

                BlastParameters searchParams = new BlastParameters();

                // Set Request parameters.
                searchParams.Add(queryDatabaseParameter, queryDatabaseValue);
                searchParams.Add(queryProgramParameter, queryProgramValue);
                searchParams.Add(expect, expectValue);
                searchParams.Add(emailNotify, emailNotifyValue);
                searchParams.Add(jobName, jobNameValue);

                string reqId = String.Empty;

                // Waiting for the any previous request to get completed.
                Thread.Sleep(150000);

                // Create a request without passing sequence.
                reqId = service.SubmitRequest(seq, searchParams);

                // Cancel subitted job.
                bool result = service.CancelRequest(reqId);

                // validate the cancelled job.
                Assert.IsTrue(result);
            }
            finally
            {
                if (service != null)
                    service.Dispose();
            }
        }
        #endregion BioHPCBlast Bvt TestCases

        #region Supported Methods

        /// <summary>
        /// Validate general fetching results.by passing 
        /// differnt parameters for BioHPC Blast web service.
        /// <param name="nodeName">xml node name.</param>
        /// <param name="isFetchSynchronous">Is Fetch Synchronous?</param>
        /// </summary>
        void ValidateBioHPCBlastResultsFetch(
            string nodeName, bool isFetchSynchronous)
        {
            // Gets the search query parameter and their values.
            string alphabetName = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlphabetNameNode);
            string querySequence = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.QuerySequency);
            string queryDatabaseValue = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DatabaseValue);
            string email = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.EmailAdress);
            string queryProgramValue = utilityObj.xmlUtil.GetTextValue(
                nodeName,
                Constants.ProgramValue);
            string queryDatabaseParameter = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DatabaseParameter);
            string queryProgramParameter = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ProgramParameter);
            string expectedHitId = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.HitID);
            string expectedAccession = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.HitAccession);
            string expectedResultCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ResultsCount);
            string expectedHitsCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.HitsCount);
            string expectedEntropyStatistics =
                utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.EntropyStatistics);
            string expectedKappaStatistics = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.KappaStatistics);
            string expectedLambdaStatistics = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LambdaStatistics);
            string expectedLength = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Length);
            int maxAttempts = int.Parse(utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.MaxAttemptsNode), (IFormatProvider)null);
            int waitingTime = int.Parse(utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.WaitTimeNode), (IFormatProvider)null);
            string expect = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Expectparameter);
            string emailNotify = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.EmailNotifyParameterNode);
            string jobName = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.JobNameParameterNode);
            string expectValue = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectNode);
            string emailNotifyValue = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.EmailNotifyNode);
            string jobNameValue = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.JobNameNode);

            object responseResults = null;
            Sequence seq = new Sequence(
                Utility.GetAlphabet(alphabetName),
                querySequence);

            // create BioHPC Blast Web Service object.
            IBlastServiceHandler service = new BioHPCBlastHandler();
            ConfigParameters configPams = new ConfigParameters();
            configPams.EmailAddress = email;
            configPams.Password = String.Empty;
            configPams.UseBrowserProxy = true;
            service.Configuration = configPams;

            BlastParameters searchParams = new BlastParameters();

            // Set Request parameters.
            searchParams.Add(queryDatabaseParameter, queryDatabaseValue);
            searchParams.Add(queryProgramParameter, queryProgramValue);
            searchParams.Add(expect, expectValue);
            searchParams.Add(emailNotify, emailNotifyValue);
            searchParams.Add(jobName, jobNameValue);

            // Create a request without passing sequence.
            string reqId = service.SubmitRequest(seq, searchParams);

            // validate request identifier.
            Assert.IsNotNull(reqId);

            ServiceRequestInformation info = service.GetRequestStatus(reqId);
            if (info.Status != ServiceRequestStatus.Waiting
                && info.Status != ServiceRequestStatus.Ready
                && info.Status != ServiceRequestStatus.Queued)
            {
                string err =
                    ApplicationLog.WriteLine("Unexpected status: '{0}'",
                    info.Status);
                Assert.Fail(err);
            }

            // get async results, poll until ready
            int attempt = 1;
            while (attempt <= maxAttempts
                    && info.Status != ServiceRequestStatus.Error
                    && info.Status != ServiceRequestStatus.Ready)
            {
                ++attempt;
                if (isFetchSynchronous)
                {
                    info = service.GetRequestStatus(reqId);
                    Thread.Sleep(
                        info.Status == ServiceRequestStatus.Waiting
                        || info.Status == ServiceRequestStatus.Queued
                        ? waitingTime * attempt : 0);
                }
                else
                {
                    Thread.Sleep(
                        info.Status == ServiceRequestStatus.Waiting
                        || info.Status == ServiceRequestStatus.Queued ?
                        waitingTime * attempt : 0);
                    info = service.GetRequestStatus(reqId);
                }

            }

            IBlastParser blastXmlParser = new BlastXmlParser();
            responseResults = blastXmlParser.Parse(
                    new StringReader(service.GetResult(reqId, searchParams)));


            Assert.IsNotNull(responseResults);
            List<BlastResult> eBlastResults =
                responseResults as List<BlastResult>;
            Assert.IsNotNull(eBlastResults);
            Assert.AreEqual(eBlastResults.Count.ToString(
                (IFormatProvider)null), expectedResultCount);
            Assert.AreEqual(eBlastResults[0].Records.Count.ToString((
                IFormatProvider)null), expectedResultCount);
            BlastSearchRecord record = eBlastResults[0].Records[0];
            Assert.AreEqual(record.Statistics.Kappa.ToString(
                (IFormatProvider)null), expectedKappaStatistics);
            Assert.AreEqual(record.Statistics.Lambda.ToString(
                (IFormatProvider)null), expectedLambdaStatistics);
            Assert.AreEqual(record.Statistics.Entropy.ToString(
                (IFormatProvider)null), expectedEntropyStatistics);
            Assert.AreEqual(record.Hits.Count.ToString(
                (IFormatProvider)null), expectedHitsCount);
            Hit hit = record.Hits[0];
            Assert.AreEqual(hit.Accession, expectedAccession);
            Assert.AreEqual(hit.Length.ToString((IFormatProvider)null),
                expectedLength);
            Assert.AreEqual(hit.Id.ToString((IFormatProvider)null),
                expectedHitId);
            Assert.AreEqual(hit.Hsps.Count.ToString((IFormatProvider)null),
                expectedResultCount);
            Console.WriteLine(string.Format((IFormatProvider)null,
               "BioHPC Blast BVT : Hits count '{0}'.", eBlastResults.Count));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "BioHPC Blast BVT : Accession '{0}'.", hit.Accession));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "BioHPC Blast BVT : Hit Id '{0}'.", hit.Id));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "BioHPC Blast BVT : Hits Count '{0}'.", hit.Hsps.Count));
            // Validate the results Synchronously with the results got earlier.
            if (isFetchSynchronous)
            {
                IList<BlastResult> syncBlastResults =
                    service.FetchResultsSync(reqId, searchParams) as List<BlastResult>;
                Assert.IsNotNull(syncBlastResults);
                if (null != eBlastResults[0].Records[0].Hits
                    && 0 < eBlastResults[0].Records[0].Hits.Count
                    && null != eBlastResults[0].Records[0].Hits[0].Hsps
                    && 0 < eBlastResults[0].Records[0].Hits[0].Hsps.Count)
                {
                    Assert.AreEqual(
                        eBlastResults[0].Records[0].Hits[0].Hsps[0].QuerySequence,
                        syncBlastResults[0].Records[0].Hits[0].Hsps[0].QuerySequence);
                }
                else
                {
                    ApplicationLog.WriteLine(
                        "No significant hits found with the these parameters.");
                    Console.WriteLine(
                        "No significant hits found with the these parameters.");
                }
            }
        }

        /// <summary>
        /// Validate general http request status by
        /// differnt parameters for BioHPC Blast Web Service.
        /// <param name="nodeName">different alphabet node name</param>
        /// </summary>
        void ValidateBioHPCGeneralGetRequestStatusMethod(string nodeName,
            string moleculeType)
        {
            // Gets the search query parameter and their values.
            string alphabetName = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlphabetNameNode);
            string querySequence = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.QuerySequency);
            string queryDatabaseValue = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DatabaseValue);
            string queryProgramValue = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ProgramValue);
            string queryDatabaseParameter = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DatabaseParameter);
            string queryProgramParameter = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ProgramParameter);
            string email = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.EmailAdress);
            string expect = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Expectparameter);
            string emailNotify = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.EmailNotifyParameterNode);
            string jobName = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.JobNameParameterNode);
            string expectValue = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectNode);
            string emailNotifyValue = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.EmailNotifyNode);
            string jobNameValue = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.JobNameNode);

            // Create a sequence.
            Sequence seq = new Sequence(Utility.GetAlphabet(alphabetName),
                querySequence);

            // Set Service confiruration parameters true.
            IBlastServiceHandler service = new BioHPCBlastHandler();

            ConfigParameters configParameters = new ConfigParameters();
            configParameters.EmailAddress = email;
            configParameters.Password = String.Empty;
            configParameters.UseBrowserProxy = true;
            service.Configuration = configParameters;

            // Create search parameters object.
            BlastParameters queryParams = new BlastParameters();

            // Add mandatory parameter values to search query parameters.
            queryParams.Add(queryDatabaseParameter, queryDatabaseValue);
            queryParams.Add(queryProgramParameter, queryProgramValue);
            queryParams.Add(expect, expectValue);
            queryParams.Add(emailNotify, emailNotifyValue);
            queryParams.Add(jobName, jobNameValue);

            // Create a request 
            // Waiting for the any previous request to get completed.
            Thread.Sleep(150000);
            string reqId = service.SubmitRequest(seq, queryParams);

            // validate request identifier.
            Assert.IsNotNull(reqId);

            // submit request identifier and get the status
            ServiceRequestInformation reqInfo =
                service.GetRequestStatus(reqId);

            // Validate job status.
            if (reqInfo.Status != ServiceRequestStatus.Waiting
                && reqInfo.Status != ServiceRequestStatus.Ready
                && reqInfo.Status != ServiceRequestStatus.Queued)
            {
                string error = ApplicationLog.WriteLine(string.Concat(
                    "Unexpected error ", reqInfo.Status));
                Assert.Fail(error);
                Console.WriteLine(string.Concat(
                    "Unexpected error ", reqInfo.Status));
            }
            else
            {
                Console.WriteLine(string.Concat(
                    "Request status ", reqInfo.Status));
            }
        }

        /// <summary>
        /// Validate Cancel submitted job by passing job id.
        /// <param name="nodeName">different alphabet node name</param>
        /// </summary>
        void ValidateCancelSubmittedJob(
            string nodeName,
            RequestType type)
        {
            // Gets the search query parameter and their values.
            string alphabetName = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlphabetNameNode);
            string querySequence = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.QuerySequency);
            string queryDatabaseValue = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DatabaseValue);
            string email = Constants.EmailForWS; ;
            string queryProgramValue = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ProgramValue);
            string queryDatabaseParameter = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DatabaseParameter);
            string queryProgramParameter = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ProgramParameter);
            string expect = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Expectparameter);
            string emailNotify = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.EmailNotifyParameterNode);
            string jobName = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.JobNameParameterNode);
            string expectValue = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectNode);
            string emailNotifyValue = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.EmailNotifyNode);
            string jobNameValue = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.JobNameNode);

            // Set Blast Parameters
            Sequence seq = new Sequence(Utility.GetAlphabet(alphabetName),
                querySequence);

            IBlastServiceHandler service = new BioHPCBlastHandler();
            ConfigParameters configParameters = new ConfigParameters();
            configParameters.EmailAddress = email;
            configParameters.Password = String.Empty;
            configParameters.UseBrowserProxy = true;
            service.Configuration = configParameters;

            BlastParameters searchParams = new BlastParameters();

            // Set Request parameters.
            searchParams.Add(queryDatabaseParameter, queryDatabaseValue);
            searchParams.Add(queryProgramParameter, queryProgramValue);
            searchParams.Add(expect, expectValue);
            searchParams.Add(emailNotify, emailNotifyValue);
            searchParams.Add(jobName, jobNameValue);

            string reqId = String.Empty;

            // Waiting for the any previous request to get completed.
            Thread.Sleep(150000);

            // Create a request without passing sequence.
            switch (type)
            {
                case RequestType.StrSubmit:
                    reqId = service.SubmitRequest(seq, searchParams);
                    break;
                case RequestType.LstSubmit:
                    IList<ISequence> lstSeq = new List<ISequence>();
                    lstSeq.Add(seq);
                    reqId = service.SubmitRequest(lstSeq, searchParams);
                    break;
            }

            // Cancel subitted job.
            bool result = service.CancelRequest(reqId);

            // validate the cancelled job.
            Assert.IsTrue(result);

            Console.WriteLine(string.Concat(
                "BioHPC Blast Bvt : Submitted job cancelled was successfully. ",
                queryProgramValue));
        }

        void service_RequestCompleted(object sender, BlastRequestCompletedEventArgs e)
        {
            BlastResult result = e.SearchResult[0];
            Hit hit = result.Records[0].Hits[0];
            Assert.IsNotNull(hit);
        }

        #endregion Supported Methods
    }
}
