using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using Bio.Registration;
using Bio.Util;
using Bio.Web.BioHPC;
using Bio.WebServiceHandlers.Properties;
using Bio.IO.FastA;

namespace Bio.Web.Blast
{
    /// <summary>
    /// This class implements IBlastService interface and defines all the atomic
    /// operation required by the interface. Each method necessarily 
    /// invokes/instantiates an atomic operation on the server (BioHPC Blast server).
    /// </summary>
    [RegistrableAttribute(true)]
    public class BioHPCBlastHandler : IBlastServiceHandler, IDisposable
    {
        #region public constants

        /// <summary>
        /// DNA Databases meta data type
        /// (Gets the list of databases)
        /// </summary>
        public const string MetadataDatabasesDna = "DatabasesDna";

        /// <summary>
        /// Protein Databases meta data type
        /// (Gets the list of databases)
        /// </summary>
        public const string MetadataDatabasesProt = "DatabasesProt";

        /// <summary>
        /// Filters meta data type
        /// (Gets the list of filters)
        /// </summary>
        public const string MetadataFilter = "Filter";

        /// <summary>
        /// Matrices meta data type
        /// (Gets the list of matrices)
        /// </summary>
        public const string MetadataMatrices = "MatrixName";

        /// <summary>
        /// Programs meta data type
        /// (Gets the list of programs)
        /// </summary>
        public const string MetadataPrograms = "Program";

        /// <summary>
        /// Output Formats meta data type
        /// (Gets the list of xml formats)
        /// </summary>
        public const string MetadataFormats = "Formats";

        #endregion

        #region private constants

        /// <summary>
        /// Default interval of time in seconds to check the status of job
        /// </summary>
        private const int RETRYINTERVAL = 10000;

        /// <summary>
        /// Default number of retries to be made to check the status - jobs may be longer try more times
        /// </summary>
        private const int NOOFRETRIES = 1000;

        /// <summary>
        /// Job Status is running
        /// </summary>
        private const string STATUSRUNNING = "RUNNING";

        /// <summary>
        /// Job status is completed successfully
        /// </summary>
        private const string STATUSFINISHED = "FINISHED";

        /// <summary>
        /// Job status is failed
        /// </summary>
        private const string STATUSFAILED = "FAILED";

        /// <summary>
        /// Job status is Queued
        /// </summary>
        private const string STATUSQUEUED = "QUEUED";

        /// <summary>
        /// Job status is Submitting
        /// </summary>
        private const string STATUSSUBMITTING = "SUBMITTING";

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

        /// <summary>
        /// Low complexity filter
        /// </summary>
        private const string PARAMETERFILTER = "FILTER";

        /// <summary>
        /// Maximum number of hits to be returned
        /// </summary>
        private const string PARAMETERALIGNMENTS = "ALIGNMENTS";

        /// <summary>
        /// Scoring matrix
        /// </summary>
        private const string PARAMETERMATRIXNAME = "MATRIX_NAME";

        /// <summary>
        /// E-value for BLAST search
        /// </summary>
        private const string PARAMETEREXPECT = "EXPECT";

        /// <summary>
        /// Minimum length of the query to be considered
        /// </summary>
        private const string PARAMETERMINQUERYLENGTH = "MINQUERYLENGTH";

        /// <summary>
        /// Whether or not to send e-mail notifications about job ("yes" or "no")
        /// </summary>
        private const string PARAMETEREMAIL = "EMAIL";

        /// <summary>
        /// Whether or not to send e-mail notifications about job ("yes" or "no")
        /// </summary>
        private const string PARAMETEREMAILNOTIFY = "EMAILNOTIFY";

        /// <summary>
        /// Default name of the BioHPC BLAST job
        /// </summary>
        private const string PARAMETERJOBNAME = "JOBNAME";

        /// <summary>
        /// String in BioHPC web service output used to detect an error condition
        /// </summary>
        private const string MsgError = "ERROR";

        #endregion

        #region MemberVariables

        /// <summary>
        /// BioHPC blast client object
        /// </summary>
        private BioHPCClient blastClient;

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
        /// Names of DNA databases available to the user - will be filled up by SubmitRequest
        /// </summary>
        private string[] dnaDatabases;

        /// <summary>
        /// Names of protein databases available to the user - will be filled up by SubmitRequest
        /// </summary>
        private string[] protDatabases;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the BioHPCBlastHandler class. 
        /// </summary>
        /// <param name="parser">Parser to parse the Blast output</param>
        /// <param name="configurations">Configuration Parameters</param>
        public BioHPCBlastHandler(
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

            this.Configuration = configurations;
            this.blastParser = parser;
        }

        /// <summary>
        /// Initializes a new instance of the BioHPCBlastHandler class. 
        /// </summary>
        /// <param name="configurations">Configuration Parameters</param>
        public BioHPCBlastHandler(ConfigParameters configurations)
            : this(new BlastXmlParser(), configurations)
        {
        }

        /// <summary>
        /// Initializes a new instance of the BioHPCBlastHandler class. 
        /// </summary>
        public BioHPCBlastHandler()
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
                return this.configuration;
            }

            set
            {
                this.configuration = value;
                this.InitializeConfiguration();
                this.InitializeBlastClient();
            }
        }

        /// <summary>
        /// Gets user-friendly implementation description
        /// </summary>
        public string Description
        {
            get { return Resources.BIOHPC_BLAST_DESCRIPTION; }
        }

        /// <summary>
        /// Gets user-friendly implementation name
        /// </summary>
        public string Name
        {
            get { return Resources.BIOHPC_BLAST_NAME; }
        }

        /// <summary>
        /// Gets an instance of object that can parse the Blast Output
        /// </summary>
        public IBlastParser Parser
        {
            get { return this.blastParser; }
        }

        /// <summary>
        /// Gets or sets the number of seconds between retries when a service request is pending. (This
        /// specifies the first interval, and subsequent retries occur at increasing multiples.)
        /// The caller can override the default by setting ConfigurationParameters.RetryInterval.
        /// </summary>
        private int RetryInterval { get; set; }

        /// <summary>
        /// Gets or sets the number of times to retry when a service request is pending. The caller
        /// can override the default value by setting ConfigurationParameters.RetryCount.
        /// </summary>
        private int RetryCount { get; set; }

        /// <summary>
        /// BioHPC job credentials
        /// </summary>
        private struct JobInfo
        {
            /// <summary>
            /// ID of the BioHPC job
            /// </summary>
            public string Jobid;

            /// <summary>
            /// job control number
            /// </summary>
            public string Cntrl;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Return the status of a submitted job.
        /// </summary>
        /// <param name="requestIdentifier">Identifier for the request of interest.</param>
        /// <returns>The status of the request.</returns>
        public ServiceRequestInformation GetRequestStatus(string requestIdentifier)
        {
            JobInfo jobidcntrl = ConvertRequestId(requestIdentifier);
            ServiceRequestInformation status = new ServiceRequestInformation();
            status.StatusInformation = this.blastClient.GetJobInfo(jobidcntrl.Jobid, jobidcntrl.Cntrl);

            switch (status.StatusInformation.Split("|".ToCharArray())[6])
            {
                case STATUSFINISHED:
                    status.Status = ServiceRequestStatus.Ready;
                    break;
                case STATUSQUEUED:
                    status.Status = ServiceRequestStatus.Queued;
                    break;
                case STATUSSUBMITTING:
                    status.Status = ServiceRequestStatus.Queued;
                    break;
                case STATUSRUNNING:
                    status.Status = ServiceRequestStatus.Waiting;
                    break;
                default:
                    status.Status = ServiceRequestStatus.Error;
                    break;
            }

            return status;
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
        /// <returns>Unique Search ID generated by Bio</returns>
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

            List<ISequence> seqlist = new List<ISequence> { sequence };
            return SubmitRequest(seqlist, parameters);
        }

        /// <summary>
        /// Submit the search request with the user supplied configuration parameters and sequence list.
        /// Implementation should make use of the Bio.IO formatters to convert the sequence into 
        /// the web interface compliant sequence format
        /// </summary>
        /// <remarks>An exception is thrown if the request does not succeed.</remarks>
        /// <param name="sequences">List of sequence to search with</param>
        /// <param name="parameters">Blast input parameters</param>
        /// <returns>Unique Search ID generated by Bio</returns>
        public string SubmitRequest(IList<ISequence> sequences, BlastParameters parameters)
        {
            if (sequences == null)
            {
                throw new Exception(Resources.BIOHPCNOSEQUENCE);
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            string requestIdentifier;

            // Start of BioHPC-specific code

            // we are submitting BLAST and give the job a name
            tAppId appID = tAppId.P_BLAST;
            string jobname = BlastParameters.Parameters["JobName"].DefaultValue;
            if (parameters.Settings.ContainsKey(PARAMETERJOBNAME))
            {
                if (!String.IsNullOrEmpty(parameters.Settings[PARAMETERJOBNAME]))
                {
                    jobname = parameters.Settings[PARAMETERJOBNAME];
                }
            }

            if (parameters.Settings.ContainsKey(PARAMETEREMAIL))
            {
                if (string.IsNullOrEmpty(Configuration.EmailAddress))
                {
                    Configuration.EmailAddress = parameters.Settings[PARAMETEREMAIL];
                }
            }

            // initialize input parameters with defaults
            AppInputData pars = blastClient.InitializeApplicationParams(appID, jobname);

            // Retrieve database names for easy access by parameter validator
            dnaDatabases = GetServiceMetadata(MetadataDatabasesDna);
            protDatabases = GetServiceMetadata(MetadataDatabasesProt);

            // Validate the parameter
            ParameterValidationResult valid = ValidateParameters(parameters, pars);
            if (!valid.IsValid)
            {
                throw new Exception(valid.ValidationErrors);
            }

            // ValidateParameters updated some of the entries in pars and put them
            // into its ParametersObject - we need to fetch the updated pars
            pars = valid.ParametersObject as AppInputData;

            // Set some remaining parameters...
            pars.blast.querysource = QuerySrcType.paste;

            // We request XML as format, since this is what we can parse...
            pars.blast.options.format = OutputFormat.XML;

            // Query sequence should be in full Fasta format.
            // We concatenate all the sequences from the list into a single string.
            pars.blast.query = String.Empty;
            foreach (ISequence auxseq in sequences)
            {
                pars.blast.query += FastAFormatter.FormatString(auxseq) + "\n";
            }

            pars.blast.query = pars.blast.query.Substring(0, pars.blast.query.Length - 1); // remobe trailing newline...

            // Run parameters and query string are ready. Submit the job to server:
            // Create a new BLAST job. 
            string jobid = String.Empty;
            string cntrl = String.Empty;
            try
            {
                string[] outtab = blastClient.CreateJob(appID, jobname, "1", Configuration.EmailAddress, Configuration.Password, "Auto");
                if (outtab[0].IndexOf(MsgError, StringComparison.Ordinal) != -1)
                {
                    throw new Exception(String.Format(CultureInfo.InvariantCulture, Resources.BIOHPCJOBNOTCREATED, outtab[0]));
                }

                jobid = outtab[1];
                cntrl = outtab[2];
                requestIdentifier = jobid + "_" + cntrl;
            }
            catch
            {
                throw new Exception(Resources.BIOHPCSERVERABSENT);
            }

            // Finally, we can submit the job
            try
            {
                string result = blastClient.SubmitJob(jobid, cntrl, pars);
                if (result.IndexOf(MsgError, StringComparison.Ordinal) != -1)
                {
                    throw new Exception(String.Format(CultureInfo.InvariantCulture, Resources.BIOHPCJOBNOTSUBMITTED, jobid, result));
                }
            }
            catch
            {
                throw new Exception(Resources.BIOHPCSERVERABSENT);
            }

            // end of BioHPC-specific code

            // Only if the event is registered, invoke the thread
            if (null != RequestCompleted)
            {
                // ThreadParameter wants a single sequence - nor sure what this is for.
                // We'll give it the first sequence from the list, i.e., sequence[0]
                BlastThreadParameter threadParameter = new BlastThreadParameter(
                        requestIdentifier,
                        sequences[0],
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
        /// Gets the search results for the pertinent request identifier.
        /// Implementation should have dedicated parsers to format the received results into Bio framework
        /// </summary>
        /// <remarks>An exception is thrown if the request does not succeed.</remarks>
        /// <param name="requestIdentifier">Identifier for the request of interest.</param>
        /// <param name="parameters">not needed - included only for compatibility with the interface</param>
        /// <returns>The search results</returns>
        public string GetResult(string requestIdentifier, BlastParameters parameters)
        {
            string response = string.Empty;
            JobInfo jobidcntrl = ConvertRequestId(requestIdentifier);
            response = blastClient.GetOutputAsString(jobidcntrl.Jobid, jobidcntrl.Cntrl);

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

            JobInfo jobidcntrl = ConvertRequestId(requestIdentifier);
            try
            {
                return blastClient.CancelJob(jobidcntrl.Jobid, jobidcntrl.Cntrl).IndexOf(MsgError, StringComparison.Ordinal) == -1;
            }
            catch
            {
                throw new Exception(Resources.BIOHPCSERVERABSENT);
            }
        }

        /// <summary>
        /// Get metadata of various sorts exposed by the service.
        /// </summary>
        /// <param name="kind">The kind of metadata to fetch.</param>
        /// <returns>A list of strings.</returns>
        public string[] GetServiceMetadata(string kind)
        {
            string[] data;
            switch (kind)
            {
                case MetadataDatabasesDna:
                    data = blastClient.GetBlastDatabases(Configuration.EmailAddress, Configuration.Password, "n");
                    break;
                case MetadataDatabasesProt:
                    data = blastClient.GetBlastDatabases(Configuration.EmailAddress, Configuration.Password, "p");
                    break;
                case MetadataFilter:
                    data = Enum.GetNames(typeof(LowCompFilter));
                    break;
                case MetadataMatrices:
                    data = Enum.GetNames(typeof(Bio.Web.BioHPC.Matrix));
                    break;
                case MetadataPrograms:
                    data = Enum.GetNames(typeof(BLASTprogram));
                    break;
                case MetadataFormats:
                    data = Enum.GetNames(typeof(OutputFormat));
                    break;
                default: data = blastClient.GetBlastDatabases(Configuration.EmailAddress, Configuration.Password, "n");
                    break;
            }

            return data;
        }

        #endregion

        #region Threading Control

        /// <summary>
        /// Process the request. This method takes care of executing the rest of the steps
        /// to complete the blast search request in a background thread. Which involves
        /// 1. Ping the service with the request identifier to get the status of request.
        /// 2. Repeat step 1, at "RetryInterval" for "RetryCount" till a "success"/"failure" 
        ///     status.
        /// 3. If the status is a "failure" raise an completed event to notify the user 
        ///     with appropriate details.
        /// 4. If the status "success". Get the output of search from server in xml format.
        /// 5. Parse the xml and the framework object model.
        /// 6. Raise the completed event and notify user with the output.
        /// </summary>
        /// <param name="sender">Client request BioHPC Blast search</param>
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

        #endregion

        #region Private Methods

        /// <summary>
        /// Convert a single, "_"-delimited BioHPC BLAST job identifier string to jobid and cntrl
        /// </summary>
        /// <param name="requestId">single, "_"-delimited BioHPC BLAST job identifier</param>
        /// <returns>result.jobid, result.cntrl</returns>
        private static JobInfo ConvertRequestId(string requestId)
        {
            if (String.IsNullOrEmpty(requestId))
            {
                throw new Exception(Resources.BIOHPCREQIDEMPTY);
            }

            if (requestId.IndexOf("_", StringComparison.Ordinal) == -1)
            {
                throw new Exception(Resources.BIOHPCREQIDBAD);
            }

            string[] aux = requestId.Split("_".ToCharArray());
            JobInfo result = new JobInfo();
            result.Jobid = aux[0];
            result.Cntrl = aux[1];
            return result;
        }

        /// <summary>
        /// Check if the selected BLAST program works with DNA or protein databases
        /// </summary>
        /// <param name="prgm">BLAST program</param>
        /// <returns>true if the program is DNA-based; false othertwise</returns>
        private static bool IsProgramDna(BLASTprogram prgm)
        {
            bool isDNA = false;
            isDNA = isDNA || prgm == BLASTprogram.blastn;
            isDNA = isDNA || prgm == BLASTprogram.tblastn;
            isDNA = isDNA || prgm == BLASTprogram.tblastx;
            return isDNA;
        }

        /// <summary>
        /// Check if the "|"-delimited string consists of DNA database names
        /// </summary>
        /// <param name="dbstring">"|"-delimited string</param>
        /// <returns>true if all databases in dbstring are DNA, false otherwise</returns>
        private bool IsDbstringDNA(string dbstring)
        {
            bool isDNA = true;
            string[] aux = dbstring.Split("|".ToCharArray());
            foreach (string tst in aux)
            {
                isDNA = isDNA && Helper.StringHasMatch(tst, dnaDatabases);
            }

            return isDNA;
        }

        /// <summary>
        /// Check if the "|"-delimited string consists of protein database names
        /// </summary>
        /// <param name="dbstring">"|"-delimited string</param>
        /// <returns>true if all databases in dbstring are protein, false otherwise</returns>
        private bool IsDbstringProt(string dbstring)
        {
            bool isProt = true;
            string[] aux = dbstring.Split("|".ToCharArray());
            foreach (string tst in aux)
            {
                isProt = isProt && Helper.StringHasMatch(tst, protDatabases);
            }

            return isProt;
        }

        /// <summary>
        /// Check the currently set parameters for validity
        /// </summary>
        /// <param name="parameters">Blast input parameters</param>
        /// <param name="pars">BLAST parameters in the BioHPC service format</param>
        /// <returns>Validation result</returns>
        private ParameterValidationResult ValidateParameters(BlastParameters parameters, AppInputData pars)
        {
            ParameterValidationResult result = new ParameterValidationResult();
            result.IsValid = true;

            // Make sure e-mail address is configured
            if (string.IsNullOrEmpty(Configuration.EmailAddress))
            {
                result.IsValid = false;
                result.ValidationErrors += Resources.PARAMETEREMAILREQUIRED;
            }

            // check required BLAST parameters            
            if (!parameters.Settings.ContainsKey(PARAMETERPROGRAM))
            {
                result.IsValid = false;
                result.ValidationErrors += Resources.PARAMETERPROGRAMREQUIRED;
            }
            else
            {
                string prgm = parameters.Settings[PARAMETERPROGRAM].ToLower(CultureInfo.InvariantCulture);
                if (Helper.StringHasMatch(prgm, Enum.GetNames(typeof(BLASTprogram))))
                {
                    pars.blast.program = (BLASTprogram)Enum.Parse(typeof(BLASTprogram), prgm);
                }
                else
                {
                    result.IsValid = false;
                    result.ValidationErrors += Resources.PARAMETERPROGRAMREQUIRED;
                }
            }

            if (!parameters.Settings.ContainsKey(PARAMETERDATABASE))
            {
                result.IsValid = false;
                result.ValidationErrors += Resources.PARAMETERDATABASEREQUIRED;
            }
            else
            {
                string dbname = parameters.Settings[PARAMETERDATABASE];
                if (IsProgramDna(pars.blast.program))
                {
                    if (IsDbstringDNA(dbname))
                    {
                        pars.blast.database.database = dbname;
                    }
                    else
                    {
                        result.IsValid = false;
                        result.ValidationErrors += Resources.BIOHPCNODNADB;
                    }
                }
                else
                {
                    if (IsDbstringProt(dbname))
                    {
                        pars.blast.database.database = dbname;
                    }
                    else
                    {
                        result.IsValid = false;
                        result.ValidationErrors += Resources.BIOHPCNOPROTDB;
                    }
                }
            }

            // Allowed parameters
            if (parameters.Settings.ContainsKey(PARAMETERFILTER))
            {
                // If the supplied filter parameter makes sense, use it; otherwise the default will be used.
                string fltr = parameters.Settings[PARAMETERFILTER];
                if (Helper.StringHasMatch(fltr, Enum.GetNames(typeof(LowCompFilter))))
                {
                    pars.blast.options.lcompfilter = (LowCompFilter)Enum.Parse(typeof(LowCompFilter), fltr);
                }
            }

            if (parameters.Settings.ContainsKey(PARAMETERALIGNMENTS))
            {
                pars.blast.options.maxtargets = int.Parse(parameters.Settings[PARAMETERALIGNMENTS], CultureInfo.InvariantCulture);
            }

            if (parameters.Settings.ContainsKey(PARAMETERMATRIXNAME))
            {
                // If the supplied matrix parameter makes sense, use it; otherwise the default will be used.
                string mtrx = parameters.Settings[PARAMETERMATRIXNAME].ToUpper(CultureInfo.InvariantCulture);
                if (Helper.StringHasMatch(mtrx, Enum.GetNames(typeof(Bio.Web.BioHPC.Matrix))))
                {
                    pars.blast.options.matrix = (Bio.Web.BioHPC.Matrix)Enum.Parse(typeof(Bio.Web.BioHPC.Matrix), mtrx);
                }
            }

            if (parameters.Settings.ContainsKey(PARAMETEREXPECT))
            {
                pars.blast.options.ecut = double.Parse(parameters.Settings[PARAMETEREXPECT], CultureInfo.InvariantCulture);
            }

            if (parameters.Settings.ContainsKey(PARAMETERMINQUERYLENGTH))
            {
                pars.blast.options.minquerylength = int.Parse(parameters.Settings[PARAMETERMINQUERYLENGTH], CultureInfo.InvariantCulture);
            }

            if (parameters.Settings.ContainsKey(PARAMETEREMAILNOTIFY))
            {
                pars.blast.runparams.email_notify = parameters.Settings[PARAMETEREMAILNOTIFY] == "yes";
            }

            // Any other unknown parameters
            foreach (KeyValuePair<string, string> parameter in parameters.Settings)
            {
                switch (parameter.Key)
                {
                    // These are either handled above, or allowed
                    case PARAMETERDATABASE:
                    case PARAMETERPROGRAM:
                    case PARAMETERFORMATTYPE:
                    case PARAMETERFILTER:
                    case PARAMETERALIGNMENTS:
                    case PARAMETERMATRIXNAME:
                    case PARAMETEREXPECT:
                    case PARAMETERMINQUERYLENGTH:
                    case PARAMETEREMAILNOTIFY:
                    case PARAMETERJOBNAME:
                    case PARAMETEREMAIL:
                        // Allow PARAMETEREMAIL so that if Configuration.EmailAddress is empty
                        // then this parameter can be set to Configuration.EmailAddress
                        break;

                    default:
                        result.IsValid = false;
                        result.ValidationErrors += string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.PARAMETRUNKNOWNBIOHPC,
                            parameter.Key);
                        break;
                }
            }

            if (result.IsValid)
            {
                result.ParametersObject = pars;
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
        /// Initialize BioHPC Blast client
        /// </summary>
        private void InitializeBlastClient()
        {
            blastClient = new BioHPCClient();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// If the BioHPCBlastHandler was opened by this object, dispose it.
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
