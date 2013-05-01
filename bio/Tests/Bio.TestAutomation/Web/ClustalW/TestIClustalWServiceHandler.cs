/****************************************************************************
 * TestIClustalWServiceHandler.cs
 * 
 * This file contains IClustalWServiceHandler implementation methods 
 * 
******************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Bio.TestAutomation.Util;
using Bio;
using Bio.Algorithms.Alignment;
using Bio.IO;
using Bio.IO.FastA;
using Bio.Web;
using Bio.Web.ClustalW;

namespace Bio.TestAutomation.Web.ClustalW
{
    /// <summary>
    /// Implementation of interface IClustalWServiceHandler
    /// </summary>
    [PartNotDiscoverable]
    class TestIClustalWServiceHandler : IClustalWServiceHandler
    {
        #region Constants
        private const int RETRYINTERVAL = 10000;
        private const int NOOFRETRIES = 10;
        private const string ERROR = "ERROR";
        private const string SUCCESS = "SUCCESS";
        private const string CONTROLID = "ControldId";
        private const string SUBMISSONRESULT = "SubmissonResult";
        private const string STOPPED = "stopped";

        #endregion

        #region Members
        private BioHPCWebService _baseClient;
        private ISequenceAlignmentParser _ClustalWParser;
        private BackgroundWorker _workerThread;
        private ConfigParameters _configuration;
        #endregion

        #region Constructor
        /// <summary>
        /// Initialize the parser and start the service using the config params
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="configparameters"></param>
        public TestIClustalWServiceHandler(ISequenceAlignmentParser parser, ConfigParameters configparameters)
        {
            _ClustalWParser = parser;
            Configuration = configparameters;
        }
        #endregion

        #region IClustalWServiceHandler Members

        public event EventHandler<ClustalWCompletedEventArgs> RequestCompleted;

        /// <summary>
        /// CreateJob() to get job id and control id and Submit job 
        /// using input sequence with job id and control id
        /// </summary>
        /// <param name="sequence">input sequences</param>
        /// <param name="parameters">input params</param>
        /// <returns>result params with job id and control id</returns>
        public ServiceParameters SubmitRequest(IList<ISequence> sequence, ClustalWParameters parameters)
        {
            ServiceParameters result = new ServiceParameters();
            if (null == sequence)
            {
                throw new ArgumentNullException("Sequence");
            }

            if (null == parameters)
            {
                throw new ArgumentNullException("Parameters");
            }

            // ClusterOption = biosim cbsum1 cbsum2k8 cbsusrv05 cbsum2 or Auto
            string[] output = _baseClient.CreateJob(tAppId.P_CLUSTALW, "test_BioHPC_Job", "1", parameters.Values[ClustalWParameters.Email].ToString(),
                 string.Empty, parameters.Values[ClustalWParameters.ClusterOption].ToString());

            if (!output[0].Contains(ERROR))
            {
                result.JobId = output[1];
                result.Parameters.Add(CONTROLID, output[2]);

                AppInputData inputData = _baseClient.InitializeApplicationParams(tAppId.P_CLUSTALW, "test_BioHPC_Job");
                StringBuilder inputSequence = new StringBuilder();

                foreach (ISequence seq in sequence)
                {
                    inputSequence.AppendLine(FastAFormatter.FormatString(seq));
                }

                inputData.clustalw.inputsource = QuerySrcType.paste;
                inputData.clustalw.inputstring = inputSequence.ToString();
                inputData.clustalw.isDNA = false;
                inputData.clustalw.action = (ClwActions)Enum.Parse(typeof(ClwActions),
                    parameters.Values[ClustalWParameters.ActionAlign].ToString());
                inputData.clustalw.email_notify = true;

                _baseClient.SubmitJob(result.JobId, result.Parameters[CONTROLID].ToString(), inputData);
                result.Parameters.Add(SUBMISSONRESULT, SUCCESS);

                // Only if the event is registered, invoke the thread
                if (null != RequestCompleted)
                {

                    // Start the BackGroundThread to check the status of job
                    _workerThread = new BackgroundWorker();
                    _workerThread.WorkerSupportsCancellation = true;
                    _workerThread.DoWork += new DoWorkEventHandler(ProcessRequestThread);
                    _workerThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompletedRequestThread);
                    _workerThread.RunWorkerAsync(result);
                }
            }
            else
            {
                result.Parameters.Add(SUBMISSONRESULT, output[0]);
            }

            return result;
        }

        /// <summary>
        /// Get the status of job using job id and control id after submitting the job
        /// </summary>
        /// <param name="parameters">job id, control id</param>
        /// <returns>result with status of job</returns>
        public Bio.Web.ServiceRequestInformation GetRequestStatus(ServiceParameters parameters)
        {
            if (null == parameters)
            {
                throw new ArgumentNullException("Parameters");
            }

            string jobStatus = _baseClient.GetJobInfo(parameters.JobId, parameters.Parameters[CONTROLID].ToString());

            string[] statusArray = jobStatus.Split('|');
            ServiceRequestInformation info = new ServiceRequestInformation();
            switch (statusArray[6])
            {
                case "FINISHED":
                    info.Status = ServiceRequestStatus.Ready;
                    break;
                case "ERROR":
                    info.Status = ServiceRequestStatus.Error;
                    break;
                default:
                    break;
            }

            return info;
        }

        /// <summary>
        /// Get the alignment results
        /// </summary>
        /// <param name="serviceParameters">job id, control id</param>
        /// <returns>alignment</returns>
        public ClustalWResult FetchResultsAsync(ServiceParameters serviceParameters)
        {
            IList<ISequenceAlignment> alignments = null;

            if (null == serviceParameters)
            {
                throw new ArgumentNullException("Parameters");
            }
            string output = _baseClient.GetOutputAsString(serviceParameters.JobId, serviceParameters.Parameters[CONTROLID].ToString());
            using (StringReader reader = new StringReader(output))
            {
                alignments = _ClustalWParser.Parse(reader);
            }

            return (new ClustalWResult(alignments[0]));
        }

        /// <summary>
        /// Check the status of job and if it is in ready status than get results
        /// </summary>
        /// <param name="serviceParameters">job id , control id</param>
        /// <returns>alignment</returns>
        public ClustalWResult FetchResultsSync(ServiceParameters serviceParameters)
        {
            ISequenceAlignment alignment = null;
            int retrycount = 0;
            ServiceRequestInformation info;

            if (null == serviceParameters)
            {
                throw new ArgumentNullException("Parameters");
            }
            do
            {
                info = GetRequestStatus(serviceParameters);
                if (info.Status == ServiceRequestStatus.Ready)
                {
                    break;
                }
                retrycount++;
                Thread.Sleep(Constants.ClusterRetryInterval * retrycount);
            }
            while (retrycount < 10);

            if (info.Status == ServiceRequestStatus.Ready)
            {
                IList<ISequenceAlignment> alignments = null;
                string output = _baseClient.GetOutputAsString(serviceParameters.JobId, serviceParameters.Parameters[CONTROLID].ToString());
                using (StringReader reader = new StringReader(output))
                {
                    alignments = _ClustalWParser.Parse(reader);
                }

                alignment = alignments[0];
            }

            return (new ClustalWResult(alignment));
        }

        /// <summary>
        /// Cancel the submitted job
        /// </summary>
        /// <param name="serviceParameters">job id, control id</param>
        /// <returns>Job is cancelled or not</returns>
        public bool CancelRequest(ServiceParameters serviceParameters)
        {
            if (_workerThread != null)
            {
                _workerThread.CancelAsync();
            }

            if (null == serviceParameters)
            {
                throw new ArgumentNullException("serviceParameters");
            }

            string errorresult = _baseClient.CancelJob(serviceParameters.JobId, serviceParameters.Parameters[CONTROLID].ToString());
            return errorresult.ToUpper(CultureInfo.CurrentCulture).Contains(STOPPED.ToUpper(CultureInfo.CurrentCulture));
        }

        #endregion

        #region IServiceHandler Members

        /// <summary>
        /// Gets user-friendly implementation description
        /// </summary>
        public string Description
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Gets user-friendly implementation name
        /// </summary>
        public string Name
        {
            get { return "BIOHPC Web Service"; }
        }

        /// <summary>
        /// Gets or sets settings for web access, such as user-agent string and 
        /// proxy configuration
        /// </summary>
        public Bio.Web.ConfigParameters Configuration
        {
            get
            {
                return _configuration;
            }
            set
            {
                _configuration = value;
                InitializeBioHPCClient();
            }
        }
        #endregion

        /// <summary>
        /// Initialize Azure Blast client
        /// </summary>
        private void InitializeBioHPCClient()
        {
            _baseClient = new BioHPCWebService();
        }

        /// <summary>
        /// Process the request. This method takes care of executing the rest of the steps
        /// to complete the blast search request in a background thread. Which involves
        /// 1. Submit the job to server
        /// 2. Ping the service with the request identifier to get the status of request.
        /// 3. Repeat step 1, at "RetryInterval" for "RetryCount" till a "success"/"failure" 
        ///     status.
        /// 4. If the status is a "failure" raise an completed event to notify the user 
        ///     with appropriate details.
        /// 5. If the status "success". Get the output of search from server in xml format.
        /// 6. Parse the xml and the framework object model.
        /// 7. Raise the completed event and notify user with the output.
        /// </summary>
        /// <param name="sender">Client request Azure Blast search</param>
        /// <param name="e">Thread event argument</param>
        private void ProcessRequestThread(object sender, DoWorkEventArgs e)
        {
            ServiceParameters serviceParameters = (ServiceParameters)e.Argument;
            IList<ISequenceAlignment> alignments = null;
            if (ERROR != serviceParameters.Parameters[SUBMISSONRESULT].ToString())
            {

                int retrycount = 0;
                ServiceRequestInformation info;
                do
                {
                    info = GetRequestStatus(serviceParameters);
                    if (info.Status == ServiceRequestStatus.Ready)
                    {
                        break;
                    }
                    retrycount++;
                }
                while (retrycount < 10);

                if (_workerThread.CancellationPending)
                {
                    e.Cancel = true;
                }
                else
                {
                    ClustalWCompletedEventArgs eventArgument = null;

                    if (info.Status == ServiceRequestStatus.Ready)
                    {
                        string output = _baseClient.GetOutputAsString(serviceParameters.JobId, serviceParameters.Parameters["ControldId"].ToString());
                        using (StringReader reader = new StringReader(output))
                        {
                            alignments = _ClustalWParser.Parse(reader);
                        }

                        eventArgument = new ClustalWCompletedEventArgs(
                               serviceParameters,
                               true,
                               new ClustalWResult(alignments[0]),
                               null,
                               string.Empty,
                               _workerThread.CancellationPending);

                        e.Result = eventArgument;
                    }
                }
            }
        }

        /// <summary>
        /// This method is invoked when request status is completed
        /// </summary>
        /// <param name="sender">Invoker of the event</param>
        /// <param name="e">Event arguments</param>
        private void CompletedRequestThread(object sender, RunWorkerCompletedEventArgs e)
        {
            if (null != RequestCompleted)
            {
                RequestCompleted(null, (ClustalWCompletedEventArgs)e.Result);
            }
        }
    }
}
