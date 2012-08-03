using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using Bio.IO.FastA;
using Bio.Util;
using Bio.WebServiceHandlers.Properties;
using Bio.Registration;
using System.Text;
using System.Web.UI;

namespace Bio.Web.Blast
{
    /// <summary>
    /// This class implements IBlastService interface and defines all the atomic
    /// operation required by the interface. NCBIQBlast will implement the client 
    /// side functionality required to perform Blast Search Requests against 
    /// the the NCBI QBlast system using their Blast URL APIs. It will need to 
    /// use HTTP encoded requests to talk to the NCBI web-server.
    /// </summary>
    [RegistrableAttribute(true)]
    public class NCBIBlastHandler : IBlastServiceHandler, IDisposable
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
        private const string STATUSWAITING = "WAITING";

        /// <summary>
        /// Job status is completed successfully
        /// </summary>
        private const string STATUSREADY = "READY";

        /// <summary>
        /// Database parameter
        /// </summary>
        private const string PARAMETERDATABASE = "DATABASE";

        /// <summary>
        /// Program parameter
        /// </summary>
        private const string PARAMETERPROGRAM = "PROGRAM";

        /// <summary>
        /// QUERY parameter
        /// </summary>
        private const string PARAMETERQUERY = "QUERY";

        /// <summary>
        /// FILTER parameter
        /// </summary>
        private const string PARAMETERFILTER = "FILTER";

        /// <summary>
        /// GENETIC CODE parameter
        /// </summary>
        private const string PARAMETERGENETICCODE = "GENETIC_CODE";

        /// <summary>
        /// Command type parameter
        /// </summary>
        private const string PARAMETERCMD = "CMD";

        /// <summary>
        /// Request Identifier type parameter
        /// </summary>
        private const string PARAMETERRID = "RID";

        /// <summary>
        /// Email type parameter
        /// </summary>
        private const string PARAMETEREMAIL = "EMAIL";

        /// <summary>
        /// STRAND type parameter
        /// </summary>
        private const string PARAMETERSTRAND = "STRAND";

        /// <summary>
        /// SENSITIVITY type parameter
        /// </summary>
        private const string PARAMETERSENSITIVITY = "SENSITIVITY";

        /// <summary>
        /// Output format type parameter
        /// </summary>
        private const string PARAMETERFORMATTYPE = "FORMAT_TYPE";

        /// <summary>
        /// Start of subsequence (one offset)
        /// </summary>
        private const string PARAMETERQUERYFROM = "QUERY_FROM";

        /// <summary>
        /// End of subsequence (one offset)
        /// </summary>
        private const string PARAMETERQUERYTO = "QUERY_TO";

        /// <summary>
        /// Command parameter
        /// </summary>
        private const string PARAMETERCOMMAND = "Command";

        /// <summary>
        /// Request Identifier parameter
        /// </summary>
        private const string PARAMETERJOBID = "RequestIdentifier";

        /// <summary>
        /// Format type parameter
        /// (expected fromat type parameter)
        /// </summary>
        private const string PARAMETERFORMAT = "FormatType";

        /// <summary>
        /// Put value for Command parameter
        /// </summary>
        private const string COMMANDPUT = "Put";

        /// <summary>
        /// Get value for Command parameter
        /// </summary>
        private const string COMMANDGET = "Get";

        /// <summary>
        /// Get value for format type parameter
        /// (XML output request)
        /// </summary>
        private const string FORMATXML = "XML";

        #endregion

        #region Member Variables

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
        /// Initializes a new instance of the NCBIBlastHandler class. 
        /// </summary>
        /// <param name="parser">Parser to parse the Blast output</param>
        /// <param name="configurations">Configuration Parameters</param>
        public NCBIBlastHandler(
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
        /// Initializes a new instance of the NCBIBlastHandler class. 
        /// </summary>
        /// <param name="configurations">Configuration Parameters</param>
        public NCBIBlastHandler(ConfigParameters configurations)
            : this(new BlastXmlParser(), configurations)
        {
        }

        /// <summary>
        /// Initializes a new instance of the NCBIBlastHandler class. 
        /// </summary>
        public NCBIBlastHandler()
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
            get { return Resources.NCBIQBLAST_DESCRIPTION; }
        }

        /// <summary>
        /// Gets user-friendly implementation name
        /// </summary>
        public string Name
        {
            get { return Resources.NCBIQBLAST_NAME; }
        }

        /// <summary>
        /// Gets an instance of object that can parse the Blast Output
        /// </summary>
        public IBlastParser Parser
        {
            get { return blastParser; }
        }

        /// <summary>
        /// Get the base URI to use for requests to the service. By default, 
        /// this is the BaseUri property, but caller can override by specifying
        /// a different URI in the ConfigurationParameters structure.
        /// </summary>
        public Uri ServiceUri
        {
            get
            {
                Uri uri = new Uri(Resources.DEFAULTNCBIBLASTSERVICEURI);

                if ((null != Configuration.Connection)
                        && !string.IsNullOrEmpty(Configuration.Connection.AbsoluteUri))
                {
                    uri = Configuration.Connection;
                }

                return uri;
            }
        }

        #endregion

        #region Public Methods

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
            if (null == parameters)
            {
                throw new ArgumentNullException("parameters");
            }

            if (null != sequences)
            {
                StringBuilder sb = new StringBuilder();
                foreach (ISequence seq in sequences)
                {
                    sb.Append(FastAFormatter.FormatString(seq));
                    sb.Append("\n");
                }
                parameters.Add("Query", sb.ToString());
            }

            if (!string.IsNullOrEmpty(Configuration.EmailAddress))
            {
                if (!parameters.Settings.ContainsKey(PARAMETEREMAIL))
                {
                    parameters.Add(PARAMETEREMAIL, Configuration.EmailAddress);
                }
            }

            string requestIdentifier = string.Empty;

            // Validate the Parameter
            ParameterValidationResult valid = ValidateParameters(parameters);
            if (!valid.IsValid)
            {
                throw new Exception(valid.ValidationErrors);
            }

            parameters.Add(PARAMETERCOMMAND, COMMANDPUT);

            WebAccessor accessor = new WebAccessor();
            WebAccessorResponse webAccessorResponse;

            if (Configuration.UseBrowserProxy)
            {
                accessor.GetBrowserProxy();
            }

            webAccessorResponse = accessor.SubmitHttpRequest(
                ServiceUri,
                true,                       // do POST
                parameters.Settings);       // request parameters
            if (!webAccessorResponse.IsSuccessful)
            {
                // failed
                accessor.Close();
                throw new Exception(String.Format(CultureInfo.InvariantCulture,
                        Resources.HTTPSUBMITFAILED,
                        webAccessorResponse.StatusDescription));
            }

            string info = ExtractInfoSection(webAccessorResponse.ResponseString);
            if (!String.IsNullOrEmpty(info))
            {
                int ridStart = info.IndexOf("RID = ", StringComparison.OrdinalIgnoreCase);
                if (ridStart >= 0)
                {
                    ridStart += "RID = ".Length;
                    int ridEnd = info.IndexOf('\n', ridStart);
                    if (ridEnd >= 0)
                    {
                        requestIdentifier = info.Substring(ridStart, ridEnd - ridStart);
                    }
                }
            }

            accessor.Close();
            if (string.IsNullOrEmpty(requestIdentifier))
            {
                string message = String.Format(CultureInfo.InvariantCulture,
                        Resources.RIDEXTRACTFAILED,
                        ExtractError(webAccessorResponse.ResponseString));
                throw new Exception(message);
            }

            // Only if the event is registered, invoke the thread
            if (null != RequestCompleted)
            {
                BlastThreadParameter threadParameter = new BlastThreadParameter(
                        requestIdentifier,
                        null, // Sequence parameter is not used any where, hence passing null.
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
            List<ISequence> seqList = new List<ISequence>();
            seqList.Add(sequence);
            return SubmitRequest(seqList, parameters);
        }

        /// <summary>
        /// Return the status of a submitted job.
        /// </summary>
        /// <param name="requestIdentifier">Identifier for the request of interest.</param>
        /// <returns>The status of the request.</returns>
        public ServiceRequestInformation GetRequestStatus(string requestIdentifier)
        {
            string information = string.Empty;
            string errorInformation = string.Empty;
            WebAccessor accessor = new WebAccessor();
            WebAccessorResponse webAccessorResponse = null;
            ServiceRequestInformation status = new ServiceRequestInformation();
            Dictionary<string, string> settings = new Dictionary<string, string>();
            settings.Add(PARAMETERCMD, "GET");
            settings.Add(PARAMETERRID, System.Web.HttpUtility.UrlEncode(requestIdentifier));

            if (Configuration.UseBrowserProxy)
            {
                accessor.GetBrowserProxy();
            }

            webAccessorResponse = accessor.SubmitHttpRequest(
                ServiceUri,
                true,
                settings);
            if (!webAccessorResponse.IsSuccessful)
            {
                // failure
                accessor.Close();
                status.Status = ServiceRequestStatus.Error;
                status.StatusInformation = webAccessorResponse.StatusDescription;
                return status;
            }

            webAccessorResponse.StatusDescription = string.Empty;
            information = ExtractInfoSection(webAccessorResponse.ResponseString);
            if (String.IsNullOrEmpty(information))
            {
                status.Status = ServiceRequestStatus.Error;
                // see if we got an error message
                errorInformation = ExtractBlastErrorSection(webAccessorResponse.ResponseString);
                if (string.IsNullOrEmpty(errorInformation))
                {
                    status.StatusInformation = "An unknown server error has occurred.";
                }
                else
                {
                    status.StatusInformation = errorInformation;
                }

                return status;
            }
            else
            {
                int statusStart = information.IndexOf("Status=", StringComparison.OrdinalIgnoreCase);
                if (statusStart >= 0)
                {
                    statusStart += "Status=".Length;
                    int statusEnd = information.IndexOf('\n', statusStart);
                    if (statusEnd >= 0)
                    {
                        webAccessorResponse.StatusDescription = information.Substring(statusStart, statusEnd - statusStart);
                    }
                }
            }

            if (webAccessorResponse.StatusDescription == STATUSWAITING)
            {
                status.Status = ServiceRequestStatus.Waiting;
                return status;
            }
            else if (webAccessorResponse.StatusDescription == STATUSREADY)
            {
                status.Status = ServiceRequestStatus.Ready;
                return status;
            }

            status.Status = ServiceRequestStatus.Error;
            status.StatusInformation = webAccessorResponse.StatusDescription;

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
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            string status = string.Empty;
            string information = string.Empty;
            WebAccessor accessor = new WebAccessor();
            WebAccessorResponse webAccessorResponse = null;

            parameters.Add(PARAMETERCOMMAND, COMMANDGET);
            parameters.Add(PARAMETERJOBID, requestIdentifier);
            parameters.Add(PARAMETERFORMAT, FORMATXML);

            if (Configuration.UseBrowserProxy)
            {
                accessor.GetBrowserProxy();
            }

            webAccessorResponse = accessor.SubmitHttpRequest(
                ServiceUri,
                true,           // POST request 
                parameters.Settings);
            if (!webAccessorResponse.IsSuccessful)
            {
                // failure
                accessor.Close();
                return null;
            }

            accessor.Close();

            information = ExtractInfoSection(webAccessorResponse.ResponseString);

            if (!String.IsNullOrEmpty(information))
            {
                int statusStart = information.IndexOf("Status=", StringComparison.OrdinalIgnoreCase);
                if (statusStart >= 0)
                {
                    statusStart += "Status=".Length;
                    int statusEnd = information.IndexOf('\n', statusStart);
                    if (statusEnd >= 0)
                    {
                        status = information.Substring(statusStart, statusEnd - statusStart);
                    }
                }
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (status == STATUSWAITING)
                {
                    return null;
                }
                else
                {
                    string message = String.Format(CultureInfo.InvariantCulture,
                            Resources.INVALIDNCBISTATUS,
                            status);
                    throw new Exception(message);
                }
            }

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

            return true;
        }

        #endregion

        #region Private Static Method

        /// <summary>
        /// Find the QBlastInfoBegin section where the request ID is stored
        /// </summary>
        /// <param name="response">Web response string</param>
        /// <returns>Information section string</returns>
        private static string ExtractInfoSection(string response)
        {
            const string startTag = "QBlastInfoBegin";
            const string endTag = "QBlastInfoEnd";

            int startInfo = response.IndexOf(startTag, StringComparison.OrdinalIgnoreCase);
            if (startInfo >= 0)
            {
                startInfo += startTag.Length;
                int endInfo = response.IndexOf(endTag, startInfo, StringComparison.OrdinalIgnoreCase);
                if (endInfo >= 0)
                {
                    return response.Substring(startInfo, endInfo - startInfo);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Look for a blast error message in the response. Try to be robust
        /// with respect to possible changes in formatting, etc.
        /// </summary>
        /// <param name="response">Web response string</param>
        /// <returns>Error section string</returns>
        private static string ExtractBlastErrorSection(string response)
        {
            const string preTag = "p id=\"blastErr\"";
            const string startTag = "Informational Message: ";
            const string endTag = ") persists";
            const string altStartTag = "Error:";
            const string altEndTag = "<";

            int startInfo = response.IndexOf(preTag, StringComparison.OrdinalIgnoreCase);
            if (startInfo >= 0)
            {
                startInfo += preTag.Length;
                int startMessage = response.IndexOf(startTag, startInfo, StringComparison.OrdinalIgnoreCase);
                if (startMessage >= 0)
                {
                    startMessage += startTag.Length;
                    int endMessage = response.IndexOf(endTag, startMessage, StringComparison.OrdinalIgnoreCase);
                    if (endMessage >= 0)
                    {
                        return response.Substring(startMessage, endMessage - startMessage);
                    }
                }
            }
            else
            {
                // look for other variant
                startInfo = response.IndexOf(altStartTag, StringComparison.OrdinalIgnoreCase);
                {
                    if (startInfo >= 0)
                    {
                        int endInfo = response.IndexOf(altEndTag, StringComparison.OrdinalIgnoreCase);
                        if (endInfo >= 0)
                        {
                            return response.Substring(startInfo, endInfo - startInfo);
                        }
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Error message is contained in list element with id msgR
        /// Here is the html structure
        /// <ul id="msgR" class="msg">
        ///     <li class="error">
        ///         <div class="error msInf">
        ///             {Error message}
        ///         </div>
        ///     </li>
        /// </ul>
        /// </summary>
        /// <param name="response">Response string</param>
        /// <returns>Error message</returns>
        private static string ExtractError(string response)
        {
            const string errorSectionStartTag = "<ul id=\"msgR\"";
            const string errorSectionEndTag = "</ul>";
            const string paraStartTag = "<p";
            const string endTag = ">";
            const string startTag = "<";
            string errorMessage = string.Empty;

            int startIndex = response.IndexOf(errorSectionStartTag, StringComparison.OrdinalIgnoreCase);
            if (0 <= startIndex)
            {
                int endIndex = 0;
                endIndex = response.IndexOf(errorSectionEndTag, startIndex, StringComparison.OrdinalIgnoreCase);
                string errorSection = response.Substring(
                        startIndex,
                        endIndex - startIndex);
                // find the index of div tag
                startIndex = errorSection.IndexOf(paraStartTag, StringComparison.OrdinalIgnoreCase);
                if (0 <= startIndex)
                {
                    // move to the end of div starttag
                    startIndex = errorSection.IndexOf(endTag, startIndex, StringComparison.OrdinalIgnoreCase);
                    if (0 <= startIndex)
                    {
                        startIndex++;
                        // End at the start of next index.
                        endIndex = errorSection.IndexOf(startTag, startIndex, StringComparison.OrdinalIgnoreCase);
                        if (0 <= endIndex)
                        {
                            // Error message has irregular spacing. Reform the 
                            // error message with regular spacing.
                            errorMessage = errorSection.Substring(
                                    startIndex,
                                    endIndex - startIndex).Trim();
                            errorMessage = string.Join(" ", errorMessage.Split(
                                    new char[] { ' ' },
                                    StringSplitOptions.RemoveEmptyEntries));
                        }
                    }
                }
            }
            return errorMessage;
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
        /// <param name="sender">Client request NCBI Blast search</param>
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
        /// <param name="argument">Event arguments</param>
        private void CompletedRequestThread(object sender, RunWorkerCompletedEventArgs argument)
        {
            if (null != RequestCompleted && !argument.Cancelled)
            {
                RequestCompleted(null, (BlastRequestCompletedEventArgs)argument.Result);
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
            else
            {
                // force program to lowercase (NCBI QBlast does require this)
                parameters.Settings[PARAMETERPROGRAM] =
                        parameters.Settings[PARAMETERPROGRAM].ToLowerInvariant();
            }

            // verify that we have a valid query
            if (parameters.Settings.ContainsKey(PARAMETERQUERY))
            {
                if (string.IsNullOrEmpty(parameters.Settings[PARAMETERQUERY]))
                {
                    result.IsValid = false;
                    result.ValidationErrors += Resources.PARAMETERSEQUENCEEMPTY;
                }
            }
            else
            {
                result.IsValid = false;
                result.ValidationErrors += Resources.PARAMETERSEQUENCEREQUIRED;
            }

            // apply any addition validation logic to the set of parameters:
            // validate filters here, since EBI BLAST uses a different set:
            if (parameters.Settings.ContainsKey(PARAMETERFILTER))
            {
                string filter = parameters.Settings[PARAMETERFILTER];
                if (!Helper.StringHasMatch(filter, "T", "F", "m", "L", "R", "S", "D"))
                {
                    result.IsValid = false;
                    result.ValidationErrors += string.Format(CultureInfo.InvariantCulture, 
                        Resources.INVALIDBLASTFILTER, 
                        filter,
                        "'T', 'F', 'm', 'L', 'R', 'S', 'D'");
                }
            }

            if (parameters.Settings.ContainsKey(PARAMETERGENETICCODE))
            {
                int geneticCode = int.Parse(parameters.Settings[PARAMETERGENETICCODE], CultureInfo.InvariantCulture);
                if (geneticCode < 1 || geneticCode > 22 ||
                    (geneticCode > 16 && geneticCode < 21))
                {
                    result.IsValid = false;
                    result.ValidationErrors += Resources.INVALIDGENETICCODE;
                }
            }

            int queryFrom = 0;
            if (parameters.Settings.ContainsKey(PARAMETERQUERYFROM))
            {
                if (!int.TryParse(parameters.Settings[PARAMETERQUERYFROM], out queryFrom))
                {
                    result.IsValid = false;
                }
            }

            int queryTo = 0;
            if (parameters.Settings.ContainsKey(PARAMETERQUERYTO))
            {
                if (!int.TryParse(parameters.Settings[PARAMETERQUERYTO], out queryTo))
                {
                    result.IsValid = false;
                }
            }

            if (((queryFrom == queryTo) && (queryFrom != 0))
                    || (queryFrom > queryTo))
            {
                result.IsValid = false;
                result.ValidationErrors += Resources.PARAMETERQUERYTOINVALID;
            }

            // check disallowed parameters:
            foreach (KeyValuePair<string, string> parameter in parameters.Settings)
            {
                switch (parameter.Key)
                {
                    case PARAMETERCMD:
                    case PARAMETERRID:
                    case PARAMETEREMAIL:
                    case PARAMETERSTRAND:
                    case PARAMETERSENSITIVITY:
                    case PARAMETERFORMATTYPE:
                        result.IsValid = false;
                        result.ValidationErrors += string.Format(CultureInfo.InvariantCulture,
                            Resources.PARAMETERUNKNOWNNCBI,
                            parameter.Key);
                        break;

                    default:
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

        #endregion

        #region IDisposable Members

        /// <summary>
        /// If the NCBIBlastHandler was opened by this object, dispose it.
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
            }
        }

        #endregion
    }
}
