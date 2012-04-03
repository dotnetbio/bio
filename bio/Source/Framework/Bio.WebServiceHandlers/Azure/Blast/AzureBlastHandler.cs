using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.ServiceModel;
using System.Text;
using System.Threading;
using Bio.IO.FastA;
using Bio.WebServiceHandlers.Properties;
using Bio.Registration;
using Microsoft.CCF.BlastDemo;

namespace Bio.Web.Blast
{
    /// <summary>
    /// This class implements IBlastService interface and defines all the atomic
    /// operation required by the interface. Each method necessarily 
    /// invokes/instantiates an atomic operation on the server (Azure Blast server).
    /// </summary>
    [PartNotDiscoverable]
    public class AzureBlastHandler : IBlastServiceHandler, IDisposable
    {
        #region Constants

        /// <summary>
        /// Default interval of time in seconds to check the status of job
        /// </summary>
        private const int RETRYINTERVAL = 10000;

        /// <summary>
        /// Default number of retries to be made to check the status
        /// </summary>
        private const int NOOFRETRIES = 10;

        /// <summary>
        /// Job Status is running
        /// </summary>
        private const string STATUSRUNNING = "Running";

        /// <summary>
        /// Job status is completed successfully
        /// </summary>
        private const string STATUSSUCCEEDED = "Succeeded";

        /// <summary>
        /// Job status is failed
        /// </summary>
        private const string STATUSFAILED = "Failed";

        /// <summary>
        /// Sets the output format expected from Azure Blast
        /// 7 - Xml
        /// 8 - Html
        /// Expect the xml output
        /// </summary>
        private const string OPTIONMVALUE = "7";

        /// <summary>
        /// Number of partitions
        /// </summary>
        private const int PARTITIONVALUE = 25;

        /// <summary>
        /// Database parameter
        /// </summary>
        private const string PARAMETERDATABASE = "DATABASE";

        /// <summary>
        /// Program parameter
        /// </summary>
        private const string PARAMETERPROGRAM = "PROGRAM";

        /// <summary>
        /// Output format type parameter
        /// </summary>
        private const string PARAMETERFORMATTYPE = "FORMAT_TYPE";

        #endregion

        #region Member Variables

        /// <summary>
        /// Azure blast client object
        /// </summary>
        private BlastServiceClient blastClient;

        /// <summary>
        /// Parser object that can parse the Blast Output
        /// </summary>
        private IBlastParser blastParser;

        /// <summary>
        /// Background worker thread that tracks the status of job and notifies
        /// user on completion.
        /// </summary>
        private BackgroundWorker workerThread;

        /// <summary>
        /// Settings for web access, such as user-agent string and 
        /// proxy configuration
        /// </summary>
        private ConfigParameters configuration;

        /// <summary>
        /// Gets or sets the number of seconds between retries when a service request is pending. (This
        /// specifies the first interval, and subsequent retries occur at increasing multiples.)
        /// The caller can override the default by setting ConfigurationParameters.RetryInterval.
        /// </summary>
        private int RetryInterval;

        /// <summary>
        /// Gets or sets the number of times to retry when a service request is pending. The caller
        /// can override the default value by setting ConfigurationParameters.RetryCount.
        /// </summary>
        private int RetryCount;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the AzureBlastHandler class. 
        /// </summary>
        /// <param name="parser">Parser to parse the Blast output</param>
        /// <param name="configurations">Configuration Parameters</param>
        public AzureBlastHandler(
                IBlastParser parser,
                ConfigParameters configurations)
        {
            if (parser == null)
            {
                throw new ArgumentNullException("parser");
            }

            if (null == configurations)
            {
                throw new ArgumentNullException("configurations");
            }

            Configuration = configurations;
            blastParser = parser;
        }

        /// <summary>
        /// Initializes a new instance of the AzureBlastHandler class. 
        /// </summary>
        /// <param name="configurations">Configuration Parameters</param>
        public AzureBlastHandler(ConfigParameters configurations)
            : this(new BlastXmlParser(), configurations)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AzureBlastHandler class. 
        /// </summary>
        public AzureBlastHandler()
            : this(new BlastXmlParser(), new ConfigParameters())
        {
        }

        #endregion

        #region Events

        /// <summary>
        /// This event is raised when Blast search is complete. It could be either a success or failure.
        /// </summary>
        public event EventHandler<BlastRequestCompletedEventArgs> RequestCompleted;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets settings for web access, such as user-agent string and 
        /// proxy configuration
        /// </summary>
        public ConfigParameters Configuration
        {
            get
            {
                return configuration;
            }

            set
            {
                configuration = value;
                InitializeConfiguration();
            }
        }

        /// <summary>
        /// Gets user-friendly implementation description
        /// </summary>
        public string Description
        {
            get { return Resources.AZURE_BLAST_DESCRIPTION; }
        }

        /// <summary>
        /// Gets user-friendly implementation name
        /// </summary>
        public string Name
        {
            get { return Resources.AZURE_BLAST_NAME; }
        }

        /// <summary>
        /// Gets an instance of object that can parse the Blast Output
        /// </summary>
        public IBlastParser Parser
        {
            get { return blastParser; }
        }
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Submit the search request with the user supplied configuration parameters 
        /// and sequence. Implementation should make use of the Bio.IO formatters 
        /// to convert the sequence into the web interface compliant sequence format.
        /// This method performs parameter validation and throw Exception on invalid input.
        /// </summary>
        /// <remarks>An exception is thrown if the request does not succeed.</remarks>
        /// <param name="sequence">The sequence to search with</param>
        /// <param name="parameters">Blast input parameters</param>
        /// <returns>Request Identifier</returns>
        public string SubmitRequest(ISequence sequence, BlastParameters parameters)
        {
            if (null == sequence)
            {
                throw new ArgumentNullException("sequence");
            }

            if (null == parameters)
            {
                throw new ArgumentNullException("parameters");
            }

            string requestIdentifier;

            // Create blast client object if not created already or if connection string has changed.
            if (blastClient == null || blastClient.Endpoint.Address.Uri != configuration.Connection)
            {
                InitializeBlastClient();
            }

            // Validate the Parameter
            ParameterValidationResult valid = ValidateParameters(parameters);
            if (!valid.IsValid)
            {
                throw new Exception(valid.ValidationErrors);
            }

            // Submit the job to server
            BlastSerivceRequest blastRequest = GetRequestParameter(
                    sequence,
                    parameters);
            try
            {
                requestIdentifier = blastClient.SubmitJob(blastRequest).ToString();
            }
            catch (FaultException<BlastFault> fault)
            {
                throw new Exception(fault.Message);
            }

            // Only if the event is registered, invoke the thread
            if (null != RequestCompleted)
            {
                BlastThreadParameter threadParameter = new BlastThreadParameter(
                        requestIdentifier,
                        sequence,
                        parameters);

                // Start the BackGroundThread to check the status of job
                workerThread = new BackgroundWorker();
                workerThread.WorkerSupportsCancellation = true;
                workerThread.DoWork += new DoWorkEventHandler(ProcessRequestThread);
                workerThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompletedRequestThread);
                workerThread.RunWorkerAsync(threadParameter);
            }

            return requestIdentifier;
        }

        /// <summary>
        /// Submit the search request with the user supplied configuration parameters and sequence
        /// Implementation should make use of the Bio.IO formatters to convert the sequence into 
        /// the web interface compliant sequence format
        /// </summary>
        /// <remarks>An exception is thrown if the request does not succeed.</remarks>
        /// <param name="sequences">List of sequence to search with</param>
        /// <param name="parameters">Blast input parameters</param>
        /// <returns>Request Identifier</returns>
        public string SubmitRequest(IList<ISequence> sequences, BlastParameters parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the status of a submitted job.
        /// </summary>
        /// <param name="requestIdentifier">Identifier for the request of interest.</param>
        /// <returns>The status of the request.</returns>
        public ServiceRequestInformation GetRequestStatus(string requestIdentifier)
        {
            ServiceRequestInformation status = new ServiceRequestInformation();
            JobStatusResponse jobStatus = blastClient.QueryJobStatus(
                    new Guid(requestIdentifier));

            switch (jobStatus.Status)
            {
                case STATUSRUNNING:
                    status.StatusInformation = jobStatus.Status;
                    status.Status = ServiceRequestStatus.Waiting;
                    break;

                case STATUSSUCCEEDED:
                    status.StatusInformation = jobStatus.Status;
                    status.Status = ServiceRequestStatus.Ready;
                    break;

                case STATUSFAILED:
                    status.StatusInformation = jobStatus.Status;
                    status.Status = ServiceRequestStatus.Error;
                    break;
            }

            return status;
        }

        /// <summary>
        /// Gets the search results for the pertinent request identifier.
        /// Implementation should have dedicated parsers to format the received results into Bio
        /// </summary>
        /// <remarks>An exception is thrown if the request does not succeed.</remarks>
        /// <param name="requestIdentifier">Identifier for the request of interest.</param>
        /// <param name="parameters">Blast input parameters</param>
        /// <returns>The search results</returns>
        public string GetResult(
                string requestIdentifier,
                BlastParameters parameters)
        {
            string resultUri = blastClient.GetJobResult(new Guid(requestIdentifier));

            WebAccessor accessor = new WebAccessor();
            WebAccessorResponse webAccessorResponse = null;

            if (Configuration.UseBrowserProxy)
            {
                accessor.GetBrowserProxy();
            }

            webAccessorResponse = accessor.SubmitHttpRequest(
                new Uri(resultUri),
                false,          // POST request 
                new Dictionary<string, string>());
            if (!webAccessorResponse.IsSuccessful)
            {
                // failure
                accessor.Close();
                return null;
            }

            accessor.Close();
            return webAccessorResponse.ResponseString;
        }

        /// <summary>
        /// Fetch the search results synchronously for the pertinent request identifier.
        /// This is a synchronous method and will not return until the results are 
        /// available.
        /// Implementation should have dedicated parsers to format the received results into
        /// Bio
        /// </summary>
        /// <remarks>
        /// An exception is thrown if the request does not succeed.
        /// </remarks>
        /// <param name="requestIdentifier">Identifier for the request of interest</param>
        /// <param name="parameters">Blast input parameters</param>
        /// <returns>The search results</returns>
        public IList<BlastResult> FetchResultsSync(
                string requestIdentifier,
                BlastParameters parameters)
        {
            IList<BlastResult> result = null;

            ServiceRequestInformation requestInfo = new ServiceRequestInformation();
            requestInfo.Status = ServiceRequestStatus.Queued;
            int retryCount = 0;

            do
            {
                requestInfo = GetRequestStatus(requestIdentifier);

                if (requestInfo.Status == ServiceRequestStatus.Ready
                        || requestInfo.Status == ServiceRequestStatus.Error)
                {
                    break;
                }

                retryCount++;
                Thread.Sleep(RetryInterval * retryCount);
            }
            while (retryCount < RetryCount);

            string message;

            if (requestInfo.Status == ServiceRequestStatus.Ready)
            {
                string output = GetResult(
                        requestIdentifier,
                        parameters);

                result = Parser.Parse(new StringReader(output));
            }
            else if (requestInfo.Status == ServiceRequestStatus.Error)
            {
                message = String.Format(CultureInfo.InvariantCulture,
                        Resources.BLASTREQUESTFAILED,
                        requestIdentifier,
                        requestInfo.Status,
                        requestInfo.StatusInformation);

                throw new Exception(message);
            }
            else
            {
                message = String.Format(CultureInfo.InvariantCulture,
                        Resources.BLASTRETRIESEXCEEDED,
                        requestIdentifier,
                        requestInfo.Status,
                        requestInfo.StatusInformation);

                throw new Exception(message);
            }

            return result;
        }

        /// <summary>
        /// Cancels the submitted job.
        /// </summary>
        /// <param name="requestIdentifier">Identifier for the request of interest.</param>
        /// <returns>Is the job cancelled.</returns>
        public bool CancelRequest(string requestIdentifier)
        {
            if (null != workerThread)
            {
                workerThread.CancelAsync();
            }

            try
            {
                return blastClient.KillJob(new Guid(requestIdentifier));
            }
            catch (FaultException<BlastFault> fault)
            {
                throw new Exception(fault.Detail.Detail);
            }
        }

        #endregion

        #region Private Static Method

        /// <summary>
        /// Get the blast service request object with all the request parameter set
        /// </summary>
        /// <param name="sequence">Input sequece</param>
        /// <param name="parameters">Blast parameters</param>
        /// <returns>Blast service request object</returns>
        private static BlastSerivceRequest GetRequestParameter(
                ISequence sequence,
                BlastParameters parameters)
        {
            BlastSerivceRequest blastParameter = new BlastSerivceRequest();

            // Sets the format of output expected from Azure Blast service
            blastParameter.OptionM = OPTIONMVALUE;

            // Sets the name of Job owner
            blastParameter.Owner = Resources.OWNERVALUE;
            blastParameter.ParitionNumber = PARTITIONVALUE;

            // Convert string to byte
            string inputContent = FastAFormatter.FormatString(sequence);
            ASCIIEncoding asciiEncoding = new ASCIIEncoding();
            blastParameter.InputContent = asciiEncoding.GetBytes(inputContent);

            // Other parameters
            // Set the Title of Job
            blastParameter.Title = sequence.ID;

            // Name of the database to be searched in
            blastParameter.DatabaseName = parameters.Settings[PARAMETERDATABASE];

            // Type of search program to be executed
            blastParameter.ProgramName = parameters.Settings[PARAMETERPROGRAM];

            return blastParameter;
        }

        #endregion

        #region Private Method

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
            BlastThreadParameter threadParameter = (BlastThreadParameter)e.Argument;
            string requestIdentifier = threadParameter.RequestIdentifier;
            try
            {
                ServiceRequestInformation requestInfo = new ServiceRequestInformation();
                requestInfo.Status = ServiceRequestStatus.Queued;
                int retryCount = 0;

                do
                {
                    requestInfo = GetRequestStatus(requestIdentifier);

                    if (requestInfo.Status == ServiceRequestStatus.Ready
                            || requestInfo.Status == ServiceRequestStatus.Error
                            || workerThread.CancellationPending)
                    {
                        break;
                    }

                    retryCount++;
                    Thread.Sleep(RetryInterval * retryCount);
                }
                while (retryCount < RetryCount);

                if (workerThread.CancellationPending)
                {
                    e.Cancel = true;
                }
                else
                {
                    BlastRequestCompletedEventArgs eventArgument = null;
                    string message;

                    if (requestInfo.Status == ServiceRequestStatus.Ready)
                    {
                        string output = GetResult(
                                requestIdentifier,
                                threadParameter.Parameters);

                        IList<BlastResult> result = Parser.Parse(new StringReader(output));

                        eventArgument = new BlastRequestCompletedEventArgs(
                                requestIdentifier,
                                true,
                                result,
                                null,
                                string.Empty,
                                workerThread.CancellationPending);

                        e.Result = eventArgument;
                    }
                    else if (requestInfo.Status == ServiceRequestStatus.Error)
                    {
                        message = String.Format(CultureInfo.InvariantCulture,
                                Resources.BLASTREQUESTFAILED,
                                requestIdentifier,
                                requestInfo.Status,
                                requestInfo.StatusInformation);

                        eventArgument = new BlastRequestCompletedEventArgs(
                                requestIdentifier,
                                false,
                                null,
                                new Exception(message),
                                message,
                                workerThread.CancellationPending);

                        e.Result = eventArgument;
                    }
                    else
                    {
                        message = String.Format(CultureInfo.InvariantCulture,
                                Resources.BLASTRETRIESEXCEEDED,
                                requestIdentifier,
                                requestInfo.Status,
                                requestInfo.StatusInformation);

                        eventArgument = new BlastRequestCompletedEventArgs(
                                requestIdentifier,
                                false,
                                null,
                                new TimeoutException(message),
                                message,
                                workerThread.CancellationPending);

                        e.Result = eventArgument;
                    }
                }
            }
            catch (Exception ex)
            {
                BlastRequestCompletedEventArgs eventArgument = new BlastRequestCompletedEventArgs(
                        string.Empty,
                        false,
                        null,
                        ex,
                        ex.Message,
                        workerThread.CancellationPending);

                e.Result = eventArgument;
            }
        }

        /// <summary>
        /// This method is invoked when request status is completed
        /// </summary>
        /// <param name="sender">Invoker of the event</param>
        /// <param name="e">Event arguments</param>
        private void CompletedRequestThread(object sender, RunWorkerCompletedEventArgs e)
        {
            if (null != RequestCompleted && !e.Cancelled)
            {
                RequestCompleted(null, (BlastRequestCompletedEventArgs)e.Result);
            }
        }

        /// <summary>
        /// Check the currently set parameters for validity
        /// </summary>
        /// <param name="parameters">Blast input parameters</param>
        /// <returns>Validation result</returns>
        private static ParameterValidationResult ValidateParameters(BlastParameters parameters)
        {
            ParameterValidationResult result = new ParameterValidationResult();
            result.IsValid = true;

            // check required parameters:
            if (!parameters.Settings.ContainsKey(PARAMETERDATABASE))
            {
                result.IsValid = false;
                result.ValidationErrors += Resources.PARAMETERDATABASEREQUIRED;
            }

            if (!parameters.Settings.ContainsKey(PARAMETERPROGRAM))
            {
                result.IsValid = false;
                result.ValidationErrors += Resources.PARAMETERPROGRAMREQUIRED;
            }

            // check disallowed parameters:
            if (parameters.Settings.ContainsKey(PARAMETERFORMATTYPE))
            {
                result.IsValid = false;
                result.ValidationErrors += Resources.PARAMETERFORMATTYPENOTALLOWED;
            }

            // Any other unknown parameters
            foreach (KeyValuePair<string, string> parameter in parameters.Settings)
            {
                switch (parameter.Key)
                {
                    case PARAMETERDATABASE:
                    case PARAMETERPROGRAM:
                    case PARAMETERFORMATTYPE:
                        break;

                    default:
                        result.IsValid = false;
                        result.ValidationErrors += string.Format(CultureInfo.InvariantCulture,
                            Resources.PARAMETERUNKNOWNAZURE,
                            parameter.Key);
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Initialize the configuration properties
        /// </summary>
        private void InitializeConfiguration()
        {
            RetryCount = Configuration.RetryCount > 0
                    ? Configuration.RetryCount
                    : NOOFRETRIES;

            RetryInterval = Configuration.RetryInterval > 0
                    ? Configuration.RetryInterval
                    : RETRYINTERVAL;
        }

        /// <summary>
        /// Initialize Azure Blast client
        /// </summary>
        private void InitializeBlastClient()
        {
            if (configuration == null)
            {
                throw new Exception(Resources.AZURE_BLAST_NULL_CONFIGURATION);
            }
            else
            {
                if (configuration.Connection == null)
                {
                    throw new Exception(Resources.AZURE_BLAST_INVALID_URI);
                }
            }

            BasicHttpBinding httpBinding = new BasicHttpBinding();
            httpBinding.Name = "BasicHttpBinding_IBlastService";
            httpBinding.CloseTimeout = new TimeSpan(0, Configuration.DefaultTimeout, 0);
            httpBinding.OpenTimeout = new TimeSpan(0, Configuration.DefaultTimeout, 0);
            httpBinding.ReceiveTimeout = new TimeSpan(0, Configuration.DefaultTimeout, 0);
            httpBinding.SendTimeout = new TimeSpan(0, Configuration.DefaultTimeout, 0);
            httpBinding.AllowCookies = false;
            httpBinding.BypassProxyOnLocal = false;
            httpBinding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            httpBinding.MaxBufferSize = 65536;
            httpBinding.MaxBufferPoolSize = 524288;
            httpBinding.MaxReceivedMessageSize = 65536;
            httpBinding.MessageEncoding = WSMessageEncoding.Text;
            httpBinding.TextEncoding = System.Text.Encoding.UTF8;
            httpBinding.TransferMode = TransferMode.Buffered;
            httpBinding.UseDefaultWebProxy = true;

            httpBinding.ReaderQuotas.MaxDepth = 32;
            httpBinding.ReaderQuotas.MaxStringContentLength = 8192;
            httpBinding.ReaderQuotas.MaxArrayLength = 16384;
            httpBinding.ReaderQuotas.MaxBytesPerRead = 4096;
            httpBinding.ReaderQuotas.MaxNameTableCharCount = 16384;

            httpBinding.Security.Mode = BasicHttpSecurityMode.None;
            httpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            httpBinding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
            httpBinding.Security.Transport.Realm = string.Empty;

            httpBinding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;

            EndpointAddress address = new EndpointAddress(configuration.Connection);

            blastClient = new BlastServiceClient(httpBinding, address);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// If the AzureBlastHandler was opened by this object, dispose it.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the managed resource
        /// </summary>
        /// <param name="disposing">If disposing equals true, dispose all resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (null != workerThread)
                {
                    workerThread.Dispose();
                    workerThread = null;
                }

                if (null != blastClient)
                {
                    if (blastClient.State != CommunicationState.Closed)
                    {
                        blastClient.Close();
                    }

                    blastClient = null;
                }
            }
        }

        #endregion
    }
}
