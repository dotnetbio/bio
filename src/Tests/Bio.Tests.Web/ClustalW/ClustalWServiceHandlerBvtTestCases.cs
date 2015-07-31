/****************************************************************************
 * ClustalWServiceHandlerBvtTestCases.cs
 * 
 * This file contains the Bio HPC Web Service BVT test cases.
 * 
******************************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Bio.Algorithms.Alignment;
using Bio.IO.ClustalW;
using Bio.IO.FastA;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using Bio.Web;
using Bio.Web.ClustalW;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.TestAutomation.Web.ClustalW
{
    /// <summary>
    /// BVT test cases to validate ClustalW service integration classes
    /// </summary>  
    /// Disabling the test cases as the ClustalW web service is down at the time of release
    [TestClass]
    public class ClustalWServiceHandlerBvtTestCases
    {

        #region Global Variables

        AutoResetEvent _resetEvent = new AutoResetEvent(false);

        #endregion Global Variables

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\ClustalWTestData\ClustalWTestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static ClustalWServiceHandlerBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion

        #region Test Cases

        /// <summary>
        /// Validate the FetchResultSync() using multiple input sequences
        /// Input : 4 dna sequences
        /// Output : aligned sequences
        /// </summary>
        /// Disabling the test cases as the ClustalW web service is down at the time of release
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFetchResultSync()
        {
            ValidateFetchResultSync(Constants.DefaultOptionNode);
        }

        /// <summary>
        /// Validate the FetchResultAsync() using multiple input sequences
        /// Input : 4 dna sequences
        /// Output : aligned sequences
        /// </summary>
        /// Disabling the test cases as the ClustalW web service is down at the time of release
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFetchResultAsync()
        {
            ValidateFetchResultAsync(Constants.DefaultOptionNode);
        }

        /// <summary>
        /// Validate GetResults using event handler with multiple input sequences
        /// Input : 4 dna sequences
        /// Output : aligned sequences
        /// </summary>
        /// Disabling the test cases as the ClustalW web service is down at the time of release
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFetchResultUsingEvent()
        {
            ValidateFetchResultsUsingEvent(Constants.DefaultOptionNode);
        }

        /// <summary>
        /// Validate Cancel Request after submitting job
        /// Input: Submit job and start the service
        /// Output: job is cancelled
        /// </summary>
        /// Disabling the test cases as the ClustalW web service is down at the time of release
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateCancelRequest()
        {
            ValidateCancelRequest(Constants.DefaultOptionNode);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Validate Submit Job and Fetch ResultSync() using multiple input sequences
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        void ValidateFetchResultSync(string nodeName)
        {
            // Read input from config file
            string filepath = utilityObj.xmlUtil.GetTextValue(
              nodeName, Constants.FilePathNode);
            string emailId = utilityObj.xmlUtil.GetTextValue(
              nodeName, Constants.EmailIDNode);
            string clusterOption = utilityObj.xmlUtil.GetTextValue(
              nodeName, Constants.ClusterOptionNode);
            string actionAlign = utilityObj.xmlUtil.GetTextValue(
              nodeName, Constants.ActionAlignNode);

            // Initialize with parser and config params
            ConfigParameters configparams = new ConfigParameters();
            ClustalWParser clustalparser = new ClustalWParser();
            configparams.UseBrowserProxy = true;
            TestIClustalWServiceHandler handler =
              new TestIClustalWServiceHandler(clustalparser, configparams);
            ClustalWParameters parameters = new ClustalWParameters();
            parameters.Values[ClustalWParameters.Email] = emailId;
            parameters.Values[ClustalWParameters.ClusterOption] = clusterOption;
            parameters.Values[ClustalWParameters.ActionAlign] = actionAlign;

            IEnumerable<ISequence> sequence = null;

            // Get the input sequences
            using (FastAParser parser = new FastAParser(filepath))
            {
                sequence = parser.Parse();

                // Submit job and validate it returned valid job id and control id 
                ServiceParameters svcparameters =
                  handler.SubmitRequest(sequence.ToList(), parameters);
                Assert.IsFalse(string.IsNullOrEmpty(svcparameters.JobId));
                ApplicationLog.WriteLine(string.Concat("JobId", svcparameters.JobId));
                foreach (string key in svcparameters.Parameters.Keys)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(svcparameters.Parameters[key].ToString()));
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null, "{0}:{1}",
                      key, svcparameters.Parameters[key].ToString()));
                }

                // Get the results and validate it is not null.
                ClustalWResult result = handler.FetchResultsSync(svcparameters);
                Assert.IsNotNull(result);
                Assert.IsNotNull(result.SequenceAlignment);
                foreach (IAlignedSequence alignSeq in result.SequenceAlignment.AlignedSequences)
                {
                    ApplicationLog.WriteLine("Aligned Sequence Sequences :");
                    foreach (ISequence seq in alignSeq.Sequences)
                    {
                        ApplicationLog.WriteLine(string.Concat("Sequence:", seq.ToString()));
                    }
                }
            }
            ApplicationLog.WriteLine(@"ClustalWServiceHandler BVT : Submit job and Get Results is successfully completed using FetchResultSync()");
        }

        /// <summary>
        /// Validate submit job and FetchResultAsync() using multiple input sequences
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        void ValidateFetchResultAsync(string nodeName)
        {
            // Read input from config file
            string filepath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string emailId = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.EmailIDNode);
            string clusterOption = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ClusterOptionNode);
            string actionAlign = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ActionAlignNode);

            ConfigParameters configparams = new ConfigParameters();
            ClustalWParser clustalparser = new ClustalWParser();
            configparams.UseBrowserProxy = true;
            TestIClustalWServiceHandler handler = new TestIClustalWServiceHandler(clustalparser, configparams);

            ClustalWParameters parameters = new ClustalWParameters();
            parameters.Values[ClustalWParameters.Email] = emailId;
            parameters.Values[ClustalWParameters.ClusterOption] = clusterOption;
            parameters.Values[ClustalWParameters.ActionAlign] = actionAlign;

            // Get input sequences
            using (FastAParser parser = new FastAParser(filepath))
            {
                IEnumerable<ISequence> sequence = parser.Parse();

                // Submit job and validate it returned valid job id and control id 
                ServiceParameters svcparameters = handler.SubmitRequest(sequence.ToList(), parameters);
                Assert.IsFalse(string.IsNullOrEmpty(svcparameters.JobId));
                foreach (string key in svcparameters.Parameters.Keys)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(svcparameters.Parameters[key].ToString()));
                }

                // Get the results and validate it is not null.
                int retrycount = 0;
                ServiceRequestInformation info;
                do
                {
                    info = handler.GetRequestStatus(svcparameters);
                    if (info.Status == ServiceRequestStatus.Ready)
                    {
                        break;
                    }

                    Thread.Sleep(
                        info.Status == ServiceRequestStatus.Waiting
                        || info.Status == ServiceRequestStatus.Queued ?
                        Constants.ClusterRetryInterval * retrycount : 0);

                    retrycount++;
                }
                while (retrycount < 10);

                ClustalWResult result = null;
                if (info.Status == ServiceRequestStatus.Ready)
                {
                    result = handler.FetchResultsAsync(svcparameters);
                }

                Assert.IsNotNull(result, "Failed to retrieve results from submitted task, Server may be offline or slow.");
                Assert.IsNotNull(result.SequenceAlignment);
                foreach (IAlignedSequence alignSeq in result.SequenceAlignment.AlignedSequences)
                {
                    ApplicationLog.WriteLine("Aligned Sequence Sequences : ");
                    foreach (ISequence seq in alignSeq.Sequences)
                    {
                        ApplicationLog.WriteLine(string.Concat("Sequence:", seq.ToString()));
                    }
                }
            }
            ApplicationLog.WriteLine(@"ClustalWServiceHandler BVT : Submit job and Get Results is successfully completed using FetchResultAsync()");
        }

        /// <summary>
        /// Validate submit job and Get Results using event handler
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        void ValidateFetchResultsUsingEvent(string nodeName)
        {
            // Read input from config file
            string filepath = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.FilePathNode);
            string emailId = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.EmailIDNode);
            string clusterOption = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.ClusterOptionNode);
            string actionAlign = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.ActionAlignNode);

            ClustalWParameters parameters = new ClustalWParameters();
            parameters.Values[ClustalWParameters.Email] = emailId;
            parameters.Values[ClustalWParameters.ClusterOption] = clusterOption;
            parameters.Values[ClustalWParameters.ActionAlign] = actionAlign;

            IEnumerable<ISequence> sequence = null;

            // Get the input sequences
            using (FastAParser parser = new FastAParser(filepath))
            {
                sequence = parser.Parse();

                // Register event and submit request
                ConfigParameters configparams = new ConfigParameters();
                ClustalWParser clustalparser = new ClustalWParser();
                configparams.UseBrowserProxy = true;
                TestIClustalWServiceHandler handler =
                  new TestIClustalWServiceHandler(clustalparser, configparams);
                handler.RequestCompleted +=
                  new EventHandler<ClustalWCompletedEventArgs>(handler_RequestCompleted);
                ServiceParameters svcparams = handler.SubmitRequest(sequence.ToList(), parameters);
                WaitHandle[] aryHandler = new WaitHandle[1];
                aryHandler[0] = _resetEvent;
                WaitHandle.WaitAny(aryHandler);

                // Validate the submit job results
                Assert.IsFalse(string.IsNullOrEmpty(svcparams.JobId));
                foreach (string key in svcparams.Parameters.Keys)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(svcparams.Parameters[key].ToString()));
                }

                _resetEvent.Close();
                _resetEvent.Dispose();
            }
        }

        /// <summary>
        /// Validate the results using RequestCompleted event
        /// </summary>
        /// <param name="sender">ClustalW</param>
        /// <param name="e"></param>
        void handler_RequestCompleted(object sender, ClustalWCompletedEventArgs e)
        {
            // Validate the get results
            Assert.IsNotNull(e.SearchResult.SequenceAlignment);
            foreach (IAlignedSequence alignSeq in e.SearchResult.SequenceAlignment.AlignedSequences)
            {
                ApplicationLog.WriteLine("Aligned Sequence Sequences :");
                foreach (ISequence seq in alignSeq.Sequences)
                {
                    ApplicationLog.WriteLine(string.Concat("Sequence:", seq.ToString()));
                }
            }

            _resetEvent.Set();

            ApplicationLog.WriteLine(@"ClustalWServiceHandler BVT : Submit job and Get Results is successfully completed using event");
        }

        /// <summary>
        /// Validate the CancelRequest()
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        [TestCategory("WebServices")]
        [Ignore]
        void ValidateCancelRequest(string nodeName)
        {
            // Read input from config file
            string filepath = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.FilePathNode);
            string emailId = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.EmailIDNode);
            string clusterOption = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.ClusterOptionNode);
            string actionAlign = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.ActionAlignNode);

            ClustalWParameters parameters = new ClustalWParameters();
            parameters.Values[ClustalWParameters.Email] = emailId;
            parameters.Values[ClustalWParameters.ClusterOption] = clusterOption;
            parameters.Values[ClustalWParameters.ActionAlign] = actionAlign;

            IEnumerable<ISequence> sequence = null;
            // Get the input sequences
            using (FastAParser parser = new FastAParser(filepath))
            {
                sequence = parser.Parse();

                // Submit job and cancel job
                // Validate cancel job is working as expected
                ConfigParameters configparams = new ConfigParameters();
                ClustalWParser clustalparser = new ClustalWParser();
                configparams.UseBrowserProxy = true;
                TestIClustalWServiceHandler handler =
                  new TestIClustalWServiceHandler(clustalparser, configparams);
                ServiceParameters svcparams = handler.SubmitRequest(sequence.ToList(), parameters);
                bool result = handler.CancelRequest(svcparams);

                Assert.IsTrue(result);
                ApplicationLog.WriteLine(string.Concat("JobId:", svcparams.JobId));
                Assert.IsFalse(string.IsNullOrEmpty(svcparams.JobId));
                foreach (string key in svcparams.Parameters.Keys)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(svcparams.Parameters[key].ToString()));
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null, "{0} : {1}",
                      key, svcparams.Parameters[key].ToString()));
                }
            }
            ApplicationLog.WriteLine(
              "ClustalWServiceHandler BVT : Cancel job is submitted as expected");
        }

        #endregion
    }
}
