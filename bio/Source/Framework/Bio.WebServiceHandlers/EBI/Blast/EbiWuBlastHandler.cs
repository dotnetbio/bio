using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Bio.IO.FastA;
using Bio.Util;
using Bio.WebServiceHandlers.Properties;
using Bio.Registration;

namespace Bio.Web.Blast
{
    /// <summary>
    /// This class implements IBlastService interface and defines all the atomic
    /// operation required by the interface. Each method necessarily 
    /// invokes/instantiates an atomic operation on the server (Ebi Wu server).
    /// </summary>
    [RegistrableAttribute(true)]
    public class EbiWuBlastHandler : IBlastServiceHandler, IDisposable
    {
        #region Constants

        /// <summary>
        /// Default interval of time in seconds to check the status of job
        /// </summary>
        private const int DefaultRetryInterval = 10000;

        /// <summary>
        /// Default number of retries to be made to check the status
        /// </summary>
        private const int DefaultNoOfRetries = 10;

        /// <summary>
        /// Job status is Queued
        /// </summary>
        private const string StatusPending = "PENDING";

        /// <summary>
        /// Job status is Running
        /// </summary>
        private const string StatusRunning = "RUNNING";

        /// <summary>
        /// Job Status is Completed successfully
        /// </summary>
        private const string StatusDone = "FINISHED";

        /// <summary>
        /// Database parameter
        /// </summary>
        private const string ParameterDatabase = "DATABASE";

        /// <summary>
        /// Program parameter
        /// </summary>
        private const string ParameterProgram = "PROGRAM";

        /// <summary>
        /// Sequence Type parameter
        /// </summary>
        private const string ParameterSequenceType = "SEQUENCETYPE";

        /// <summary>
        /// Alignment View Parameter
        /// </summary>
        private const string ParameterAlignmentView = "ALIGNMENTVIEW";

        /// <summary>
        /// Alignment View Parameter Key
        /// </summary>
        private const string ParameterKeyAlignmentView = "AlignmentView";

        /// <summary>
        /// EMAIL parameter
        /// </summary>
        private const string ParameterEmail = "EMAIL";

        /// <summary>
        /// FILTER parameter
        /// </summary>
        private const string ParameterFilter = "FILTER";

        /// <summary>
        /// Number of alignments to return parameter
        /// </summary>
        private const string ParameterAlignments = "ALIGNMENTS";

        /// <summary>
        /// Similarity Matrix name parameter
        /// </summary>
        private const string ParameterMatrixName = "MATRIX_NAME";

        /// <summary>
        /// Expect value parameter
        /// </summary>
        private const string ParameterExpect = "EXPECT";

        /// <summary>
        /// Type of input provided to blast service
        /// </summary>
        private const string SequenceType = "dna";

        /// <summary>
        /// Xml output type
        /// </summary>
        private const string AppXmlYes = "yes";

        /// <summary>
        /// Databases meta data type
        /// (Gets the list of databases)
        /// </summary>
        public const string MetadataDatabases = "database";

        /// <summary>
        /// Filters meta data type
        /// (Gets the list of filters)
        /// </summary>
        public const string MetadataFilter = "filter";

        /// <summary>
        /// Matrices meta data type
        /// (Gets the list of matrices)
        /// </summary>
        public const string MetadataMatrices = "matrix";

        /// <summary>
        /// Programs meta data type
        /// (Gets the list of programs)
        /// </summary>
        public const string MetadataPrograms = "program";

        /// <summary>
        /// Sensitivity meta data type
        /// (Gets the list of sensitivity)
        /// </summary>
        public const string MetadataSensitivity = "sensitivity";

        /// <summary>
        /// Sort meta data type
        /// (Gets the list of sort supported)
        /// </summary>
        public const string MetadataSort = "sort";

        /// <summary>
        /// Stats meta data type
        /// (Gets the list of Statistics)
        /// </summary>
        public const string MetadataStatistics = "stats";

        /// <summary>
        /// XmlFormats meta data type
        /// (Gets the list of xml formats)
        /// </summary>
        public const string MetadataXmlFormats = "XmlFormats";

        #endregion

        #region Member Variables

        /// <summary>
        /// WSWUB blast client object
        /// </summary>
        private JDispatcherService blastClient;

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
        /// Initializes a new instance of the EbiWuBlastHandler class.
        /// </summary>
        /// <param name="parser">Parser to parse the Blast output</param>
        /// <param name="configurations">Configuration Parameters</param>
        public EbiWuBlastHandler(
                IBlastParser parser,
                ConfigParameters configurations)
        {
            if (null == parser)
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
        /// Initializes a new instance of the EbiWuBlastHandler class.
        /// </summary>
        /// <param name="configurations">Configuration Parameters</param>
        public EbiWuBlastHandler(ConfigParameters configurations)
            : this(new BlastXmlParser(), configurations)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EbiWuBlastHandler class.
        /// </summary>
        public EbiWuBlastHandler()
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
                InitializeBlastClient();
            }
        }

        /// <summary>
        /// Gets user-friendly implementation name
        /// </summary>
        public string Name
        {
            get { return Resources.EBIWUBLAST_NAME; }
        }

        /// <summary>
        /// Gets user-friendly implementation description
        /// </summary>
        public string Description
        {
            get { return Resources.EBIWUBLAST_DESCRIPTION; }
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
            List<ISequence> sequences = new List<ISequence>();
            sequences.Add(sequence);
            return SubmitRequest(sequences, parameters);
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
            string emailAddress = string.Empty;

            if (null == sequences)
            {
                throw new ArgumentNullException("sequences");
            }

            if (null == parameters)
            {
                throw new ArgumentNullException("parameters");
            }

            string tempEmail;

            if (parameters.Settings.TryGetValue(ParameterEmail, out tempEmail) && 
                !string.IsNullOrEmpty(tempEmail))
            {
                emailAddress = tempEmail;
            }

            string requestIdentifier = string.Empty;

            // Validate the Parameter
            ParameterValidationResult valid = ValidateParameters(parameters);
            if (!valid.IsValid)
            {
                throw new Exception(valid.ValidationErrors);
            }

            // Submit the job to server
            InputParameters blastRequest = GetRequestParameter(parameters);

            StringBuilder seqBuilder = new StringBuilder();
            foreach (ISequence sequence in sequences)
            {
                byte[] buffer = new byte[80];
                int bufferIndex = 0, maxLineSize=80;
                
                seqBuilder.AppendLine(">" + sequence.ID);

                for (long index = 0; index < sequence.Count; index += maxLineSize)
                {
                    for (bufferIndex = 0; bufferIndex < maxLineSize && index + bufferIndex < sequence.Count; bufferIndex++)
                    {
                        buffer[bufferIndex] = sequence[index + bufferIndex];
                    }

                    string line = ASCIIEncoding.ASCII.GetString(buffer, 0, bufferIndex);
                    seqBuilder.AppendLine(line);
                }
                
            }

            blastRequest.sequence = seqBuilder.ToString();

            requestIdentifier = blastClient.run(emailAddress, string.Empty, blastRequest);

            // Only if the event is registered, invoke the thread
            if (null != RequestCompleted)
            {
                BlastThreadParameter threadParameter = new BlastThreadParameter(
                        requestIdentifier,
                        null,
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
        /// Return the status of a submitted job.
        /// </summary>
        /// <param name="requestIdentifier">Identifier for the request of interest.</param>
        /// <returns>The status of the request.</returns>
        public ServiceRequestInformation GetRequestStatus(string requestIdentifier)
        {
            ServiceRequestInformation status = new ServiceRequestInformation();
            status.StatusInformation = blastClient.getStatus(requestIdentifier);

            switch (status.StatusInformation)
            {
                case StatusDone:
                    status.Status = ServiceRequestStatus.Ready;
                    break;

                //case StatusPending:
                //    status.Status = ServiceRequestStatus.Queued;
                //    break;

                case StatusRunning:
                    status.Status = ServiceRequestStatus.Waiting;
                    break;

                default:
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
            wsResultType[] resultTypes = blastClient.getResultTypes(requestIdentifier);

            if (resultTypes == null)
            {
                throw new Exception(Resources.EBIWURESULTTYPEFAILED);
            }

            string response = string.Empty;
            foreach (wsResultType resultType in resultTypes)
            {
                if (resultType.mediaType == "application/xml")
                {
                    byte[] content = blastClient.getResult(requestIdentifier, resultType.identifier, null);
                    ASCIIEncoding enc = new ASCIIEncoding();
                    response = enc.GetString(content);
                }
            }

            if (string.IsNullOrEmpty(response))
            {
                throw new Exception(
                        String.Format(CultureInfo.InvariantCulture,
                            Resources.EMPTYRESPONSE,
                            requestIdentifier));
            }

            // we have XML results, parse them
            return response;
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

            blastClient.Abort();

            return true;
        }

        /// <summary>
        /// Get metadata of various sorts exposed by the service.
        /// </summary>
        /// <param name="kind">The kind of metadata to fetch.</param>
        /// <returns>A list of strings.</returns>
        public IList<string> GetServiceMetadata(string kind)
        {
            wsParameterDetails data = new wsParameterDetails();
            
            data = blastClient.getParameterDetails(kind);
            List<string> ret = new List<string>();
            if (data != null && data.values != null)
            {
                foreach (wsParameterValue paramValue in data.values)
                {
                    ret.Add(paramValue.value);
                }
            }

            return ret;
        }

        #endregion

        #region Private Static Method

        /// <summary>
        /// Get the blast service request object with all the request parameter set
        /// </summary>
        /// <param name="parameters">Blast parameters</param>
        /// <returns>Blast service request object</returns>
        private static InputParameters GetRequestParameter(
                BlastParameters parameters)
        {
            InputParameters blastParameter = new InputParameters();

            // check required parameters:
            if (parameters.Settings[ParameterDatabase].Contains(","))
            {
                blastParameter.database = parameters.Settings[ParameterDatabase].Split(",".ToCharArray());
            }
            else
            {
                blastParameter.database = new string[1];
                blastParameter.database[0] = parameters.Settings[ParameterDatabase];
            }

            // force program to lowercase, per EBI docs (though the service seems
            // to work fine regardless of the case of this parameter)
            blastParameter.program = parameters.Settings[ParameterProgram].ToLower(CultureInfo.CurrentCulture);

            // set the sequence Type.
            blastParameter.stype = parameters.Settings[ParameterSequenceType].ToLower(CultureInfo.CurrentCulture);
            
            // Set the Alignment View property.
            if (parameters.Settings.ContainsKey(ParameterAlignmentView))
            {
                blastParameter.align = (int?)int.Parse(parameters.Settings[ParameterAlignmentView]);
                blastParameter.alignSpecified = true;
            }
            else
            {
                blastParameter.align = int.Parse(BlastParameters.Parameters[ParameterKeyAlignmentView].DefaultValue);
                blastParameter.alignSpecified = true;
            }

            //// note: query is not part of the inputParams class, so the caller will
            //// need to handle it separately.
            //blastParameter.email = parameters.Settings[ParameterEmail];

            // apply any addition validation logic and set remaining supported parameters:
            // validate filters here, since QBLAST uses a different set:
            if (parameters.Settings.ContainsKey(ParameterFilter))
            {
                blastParameter.filter = parameters.Settings[ParameterFilter];
            }

            if (parameters.Settings.ContainsKey(ParameterAlignments))
            {
                blastParameter.alignments = (int?)int.Parse(parameters.Settings[ParameterAlignments], CultureInfo.InvariantCulture);
            }

            if (parameters.Settings.ContainsKey(ParameterMatrixName))
            {
                blastParameter.matrix = parameters.Settings[ParameterMatrixName];
            }

            if (parameters.Settings.ContainsKey(ParameterExpect))
            {
                blastParameter.exp = parameters.Settings[ParameterExpect];
            }
            
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
        /// <param name="sender">Client request EBIWU Blast search</param>
        /// <param name="argument">Thread event argument</param>
        private void ProcessRequestThread(object sender, DoWorkEventArgs argument)
        {
            BlastThreadParameter threadParameter = (BlastThreadParameter)argument.Argument;
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
                    argument.Cancel = true;
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

                        argument.Result = eventArgument;
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

                        argument.Result = eventArgument;
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

                        argument.Result = eventArgument;
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

                argument.Result = eventArgument;
            }
        }

        /// <summary>
        /// This method is invoked when request status is completed
        /// </summary>
        /// <param name="sender">Invoker of the event</param>
        /// <param name="eventArgument">Event arguments</param>
        private void CompletedRequestThread(
                object sender,
                RunWorkerCompletedEventArgs eventArgument)
        {
            if (null != RequestCompleted && !eventArgument.Cancelled)
            {
                RequestCompleted(null, (BlastRequestCompletedEventArgs)eventArgument.Result);
            }
        }

        /// <summary>
        /// Check the currently set parameters for validity
        /// </summary>
        /// <param name="parameters">Blast input parameters</param>
        /// <returns>Validation result</returns>
        public static ParameterValidationResult ValidateParameters(BlastParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            ParameterValidationResult result = new ParameterValidationResult();
            result.IsValid = true;

            // check required parameters:
            if (!parameters.Settings.ContainsKey(ParameterDatabase))
            {
                result.IsValid = false;
                result.ValidationErrors += Resources.PARAMETERDATABASEREQUIRED;
            }

            if (!parameters.Settings.ContainsKey(ParameterProgram))
            {
                result.IsValid = false;
                result.ValidationErrors += Resources.PARAMETERPROGRAMREQUIRED;
            }

            if (!parameters.Settings.ContainsKey(ParameterSequenceType))
            {
                result.IsValid = false;
                result.ValidationErrors += Resources.PARAMETERSEQUENCETYPEREQUIRED;
            }

            //// note: query is not part of the inputParams class, so the caller will
            //// need to handle it separately.
            //if (!parameters.Settings.ContainsKey(ParameterEmail))
            //{
            //    result.IsValid = false;
            //    result.ValidationErrors += Resources.PARAMETEREMAILREQUIRED;
            //}

            if (parameters.Settings.ContainsKey(ParameterFilter))
            {
                string filter = parameters.Settings[ParameterFilter];
                if (!Helper.StringHasMatch(filter, "none", "seg", "xnu", "seg+xnu", "dust"))
                {
                    result.IsValid = false;
                    result.ValidationErrors += string.Format(CultureInfo.InvariantCulture, Resources.INVALIDBLASTFILTER, filter, "'none, 'seg', 'xnu', 'seg+xnu', 'dust'");
                }
            }

            // Any other unknown parameters
            foreach (KeyValuePair<string, string> parameter in parameters.Settings)
            {
                switch (parameter.Key)
                {
                    case ParameterDatabase:
                    case ParameterProgram:
                    case ParameterSequenceType:
                    case ParameterEmail:
                    case ParameterFilter:
                    case ParameterAlignments:
                    case ParameterMatrixName:
                    case ParameterExpect:
                    case ParameterAlignmentView:
                        // These are valid parameter, so allow them.
                        break;

                    default:
                        result.IsValid = false;
                        result.ValidationErrors += string.Format(CultureInfo.InvariantCulture,
                            Resources.PARAMETERUNKNOWNEBIWU,
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
                    : DefaultNoOfRetries;

            RetryInterval = Configuration.RetryInterval > 0
                    ? Configuration.RetryInterval
                    : DefaultRetryInterval;
        }

        /// <summary>
        /// Initialize EBIWU Blast client
        /// </summary>
        private void InitializeBlastClient()
        {
            blastClient = new JDispatcherService();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// If the EbiWuBlastHandler was opened by this object, dispose it.
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
                    blastClient.Dispose();
                    blastClient = null;
                }
            }
        }

        #endregion
    }
}
