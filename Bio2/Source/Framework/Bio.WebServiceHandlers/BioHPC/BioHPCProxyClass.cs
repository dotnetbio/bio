namespace Bio.Web.BioHPC {
    using System.Xml.Serialization;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Diagnostics;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="BioHPCWebServiceSoap", Namespace="http://BioHPC.org/")]
    public partial class BioHPCWebService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback HelloWorldOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetJobInfoOperationCompleted;
        
        private System.Threading.SendOrPostCallback CreateJobOperationCompleted;
        
        private System.Threading.SendOrPostCallback SubmitJobOperationCompleted;
        
        private System.Threading.SendOrPostCallback InitializeApplicationParamsOperationCompleted;
        
        private System.Threading.SendOrPostCallback InitializeInputParamsOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetOutputAsStringOperationCompleted;
        
        private System.Threading.SendOrPostCallback UploadFileChunkOperationCompleted;
        
        private System.Threading.SendOrPostCallback DownloadFileChunk0OperationCompleted;
        
        private System.Threading.SendOrPostCallback DownloadFileChunkOperationCompleted;
        
        private System.Threading.SendOrPostCallback QueryFileLengthOperationCompleted;
        
        private System.Threading.SendOrPostCallback RequestOutFileNamesOperationCompleted;
        
        private System.Threading.SendOrPostCallback CancelJobOperationCompleted;
        
        private System.Threading.SendOrPostCallback ListMyJobsOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetBlastDatabasesOperationCompleted;
        
        /// <remarks/>
        public BioHPCWebService() {
            this.Url = "http://cbsuapps.tc.cornell.edu/CBSUAppsWebSvc/BioHPCWebSvc.asmx";
        }
        
        /// <remarks/>
        public event HelloWorldCompletedEventHandler HelloWorldCompleted;
        
        /// <remarks/>
        public event GetJobInfoCompletedEventHandler GetJobInfoCompleted;
        
        /// <remarks/>
        public event CreateJobCompletedEventHandler CreateJobCompleted;
        
        /// <remarks/>
        public event SubmitJobCompletedEventHandler SubmitJobCompleted;
        
        /// <remarks/>
        public event InitializeApplicationParamsCompletedEventHandler InitializeApplicationParamsCompleted;
        
        /// <remarks/>
        public event InitializeInputParamsCompletedEventHandler InitializeInputParamsCompleted;
        
        /// <remarks/>
        public event GetOutputAsStringCompletedEventHandler GetOutputAsStringCompleted;
        
        /// <remarks/>
        public event UploadFileChunkCompletedEventHandler UploadFileChunkCompleted;
        
        /// <remarks/>
        public event DownloadFileChunk0CompletedEventHandler DownloadFileChunk0Completed;
        
        /// <remarks/>
        public event DownloadFileChunkCompletedEventHandler DownloadFileChunkCompleted;
        
        /// <remarks/>
        public event QueryFileLengthCompletedEventHandler QueryFileLengthCompleted;
        
        /// <remarks/>
        public event RequestOutFileNamesCompletedEventHandler RequestOutFileNamesCompleted;
        
        /// <remarks/>
        public event CancelJobCompletedEventHandler CancelJobCompleted;
        
        /// <remarks/>
        public event ListMyJobsCompletedEventHandler ListMyJobsCompleted;
        
        /// <remarks/>
        public event GetBlastDatabasesCompletedEventHandler GetBlastDatabasesCompleted;

        /// <summary>
        /// Test the service
        /// </summary>
        /// <returns>Greating from BioHPC</returns>
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://BioHPC.org/HelloWorld", RequestNamespace="http://BioHPC.org/", ResponseNamespace="http://BioHPC.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string HelloWorld() {
            object[] results = this.Invoke("HelloWorld", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginHelloWorld(System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("HelloWorld", new object[0], callback, asyncState);
        }
        
        /// <remarks/>
        public string EndHelloWorld(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void HelloWorldAsync() {
            this.HelloWorldAsync(null);
        }
        
        /// <remarks/>
        public void HelloWorldAsync(object userState) {
            if ((this.HelloWorldOperationCompleted == null)) {
                this.HelloWorldOperationCompleted = new System.Threading.SendOrPostCallback(this.OnHelloWorldOperationCompleted);
            }
            this.InvokeAsync("HelloWorld", new object[0], this.HelloWorldOperationCompleted, userState);
        }
        
        private void OnHelloWorldOperationCompleted(object arg) {
            if ((this.HelloWorldCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.HelloWorldCompleted(this, new HelloWorldCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        /// <summary>
        /// Provides informatiojn about the job.
        /// </summary>
        /// <param name="jobid">ID of the job</param>
        /// <param name="cntrl">Control number</param>
        /// <returns>A "|"-delimited string containing the following fields:
        /// jobID, control number, application name, job name, cluster,
        /// email, job status, submission date, start date,
        /// end date (for running jobs - anticipated), timeout (in minutes);</returns>
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://BioHPC.org/GetJobInfo", RequestNamespace="http://BioHPC.org/", ResponseNamespace="http://BioHPC.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetJobInfo(string jobid, string cntrl) {
            object[] results = this.Invoke("GetJobInfo", new object[] {
                        jobid,
                        cntrl});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginGetJobInfo(string jobid, string cntrl, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("GetJobInfo", new object[] {
                        jobid,
                        cntrl}, callback, asyncState);
        }
        
        /// <remarks/>
        public string EndGetJobInfo(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GetJobInfoAsync(string jobid, string cntrl) {
            this.GetJobInfoAsync(jobid, cntrl, null);
        }
        
        /// <remarks/>
        public void GetJobInfoAsync(string jobid, string cntrl, object userState) {
            if ((this.GetJobInfoOperationCompleted == null)) {
                this.GetJobInfoOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetJobInfoOperationCompleted);
            }
            this.InvokeAsync("GetJobInfo", new object[] {
                        jobid,
                        cntrl}, this.GetJobInfoOperationCompleted, userState);
        }
        
        private void OnGetJobInfoOperationCompleted(object arg) {
            if ((this.GetJobInfoCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetJobInfoCompleted(this, new GetJobInfoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        /// <summary>
        /// Allocate a new job on BioHPC server.
        /// </summary>
        /// <param name="appID">Application to run</param>
        /// <param name="jobname">Name of the job</param>
        /// <param name="nodes">Number of processors to allocate (null or empty string 
        /// will allocate default number of processors</param>
        /// <param name="email">E-mail address of the submitted (valid address is required)</param>
        /// <param name="password">Password(for registered users only; otherwise - supply empty string)</param>
        /// <param name="cluster">Requested cluster ("Auto" for automatic assignment)</param>
        /// <returns>Array of strings with the following elements: [0] - output from job creation,
        /// [1] - job ID of the created job, [2] - control number of the created job, [3] - cluster allocated to the job.
        /// In case of error, element [0] contains string "ERROR" and explanation.</returns>
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://BioHPC.org/CreateJob", RequestNamespace="http://BioHPC.org/", ResponseNamespace="http://BioHPC.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string[] CreateJob(tAppId appID, string jobname, string nodes, string email, string password, string cluster) {
            object[] results = this.Invoke("CreateJob", new object[] {
                        appID,
                        jobname,
                        nodes,
                        email,
                        password,
                        cluster});
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginCreateJob(tAppId appID, string jobname, string nodes, string email, string password, string cluster, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("CreateJob", new object[] {
                        appID,
                        jobname,
                        nodes,
                        email,
                        password,
                        cluster}, callback, asyncState);
        }
        
        /// <remarks/>
        public string[] EndCreateJob(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public void CreateJobAsync(tAppId appID, string jobname, string nodes, string email, string password, string cluster) {
            this.CreateJobAsync(appID, jobname, nodes, email, password, cluster, null);
        }
        
        /// <remarks/>
        public void CreateJobAsync(tAppId appID, string jobname, string nodes, string email, string password, string cluster, object userState) {
            if ((this.CreateJobOperationCompleted == null)) {
                this.CreateJobOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCreateJobOperationCompleted);
            }
            this.InvokeAsync("CreateJob", new object[] {
                        appID,
                        jobname,
                        nodes,
                        email,
                        password,
                        cluster}, this.CreateJobOperationCompleted, userState);
        }
        
        private void OnCreateJobOperationCompleted(object arg) {
            if ((this.CreateJobCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CreateJobCompleted(this, new CreateJobCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <summary>
        ///  Submits job allocated earlier by CreateJob
        /// </summary>
        /// <param name="jobid">ID of the job (as returned by CreateJob)</param>
        /// <param name="cntrl">Control number (as returned by CreateJob)</param>
        /// <param name="pars">Structure with job parameters</param>
        /// <returns>Report from job submission. In case of error, the report contains string "ERROR".</returns>
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://BioHPC.org/SubmitJob", RequestNamespace="http://BioHPC.org/", ResponseNamespace="http://BioHPC.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string SubmitJob(string jobid, string cntrl, AppInputData pars) {
            object[] results = this.Invoke("SubmitJob", new object[] {
                        jobid,
                        cntrl,
                        pars});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginSubmitJob(string jobid, string cntrl, AppInputData pars, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("SubmitJob", new object[] {
                        jobid,
                        cntrl,
                        pars}, callback, asyncState);
        }
        
        /// <remarks/>
        public string EndSubmitJob(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void SubmitJobAsync(string jobid, string cntrl, AppInputData pars) {
            this.SubmitJobAsync(jobid, cntrl, pars, null);
        }
        
        /// <remarks/>
        public void SubmitJobAsync(string jobid, string cntrl, AppInputData pars, object userState) {
            if ((this.SubmitJobOperationCompleted == null)) {
                this.SubmitJobOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSubmitJobOperationCompleted);
            }
            this.InvokeAsync("SubmitJob", new object[] {
                        jobid,
                        cntrl,
                        pars}, this.SubmitJobOperationCompleted, userState);
        }
        
        private void OnSubmitJobOperationCompleted(object arg) {
            if ((this.SubmitJobCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SubmitJobCompleted(this, new SubmitJobCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <summary>
        /// Initializes input parameters to defaults for application appId,
        /// </summary>
        /// <param name="appId">Application</param>
        /// <param name="jobname">Name of the job</param>
        /// <returns>Initialized input parameter structure</returns>
        /// <remarks>The field InputFileName contains the name of the input file the server expects.</remarks>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://BioHPC.org/InitializeApplicationParams", RequestNamespace="http://BioHPC.org/", ResponseNamespace="http://BioHPC.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public AppInputData InitializeApplicationParams(tAppId appId, string jobname) {
            object[] results = this.Invoke("InitializeApplicationParams", new object[] {
                        appId,
                        jobname});
            return ((AppInputData)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginInitializeApplicationParams(tAppId appId, string jobname, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("InitializeApplicationParams", new object[] {
                        appId,
                        jobname}, callback, asyncState);
        }
        
        /// <remarks/>
        public AppInputData EndInitializeApplicationParams(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((AppInputData)(results[0]));
        }
        
        /// <remarks/>
        public void InitializeApplicationParamsAsync(tAppId appId, string jobname) {
            this.InitializeApplicationParamsAsync(appId, jobname, null);
        }
        
        /// <remarks/>
        public void InitializeApplicationParamsAsync(tAppId appId, string jobname, object userState) {
            if ((this.InitializeApplicationParamsOperationCompleted == null)) {
                this.InitializeApplicationParamsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnInitializeApplicationParamsOperationCompleted);
            }
            this.InvokeAsync("InitializeApplicationParams", new object[] {
                        appId,
                        jobname}, this.InitializeApplicationParamsOperationCompleted, userState);
        }
        
        private void OnInitializeApplicationParamsOperationCompleted(object arg) {
            if ((this.InitializeApplicationParamsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.InitializeApplicationParamsCompleted(this, new InitializeApplicationParamsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <summary>
        /// Initializes input parameters to defaults for a job with known jobID and control number.
        /// </summary>
        /// <remarks>The field InputFileName in the initialized input structure 
        /// contains the name of the input file the server expects.</remarks>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://BioHPC.org/InitializeInputParams", RequestNamespace="http://BioHPC.org/", ResponseNamespace="http://BioHPC.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public AppInputData InitializeInputParams(string jobid, string cntrl) {
            object[] results = this.Invoke("InitializeInputParams", new object[] {
                        jobid,
                        cntrl});
            return ((AppInputData)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginInitializeInputParams(string jobid, string cntrl, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("InitializeInputParams", new object[] {
                        jobid,
                        cntrl}, callback, asyncState);
        }
        
        /// <remarks/>
        public AppInputData EndInitializeInputParams(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((AppInputData)(results[0]));
        }
        
        /// <remarks/>
        public void InitializeInputParamsAsync(string jobid, string cntrl) {
            this.InitializeInputParamsAsync(jobid, cntrl, null);
        }
        
        /// <remarks/>
        public void InitializeInputParamsAsync(string jobid, string cntrl, object userState) {
            if ((this.InitializeInputParamsOperationCompleted == null)) {
                this.InitializeInputParamsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnInitializeInputParamsOperationCompleted);
            }
            this.InvokeAsync("InitializeInputParams", new object[] {
                        jobid,
                        cntrl}, this.InitializeInputParamsOperationCompleted, userState);
        }
        
        private void OnInitializeInputParamsOperationCompleted(object arg) {
            if ((this.InitializeInputParamsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.InitializeInputParamsCompleted(this, new InitializeInputParamsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <summary>
        /// Get the main output file of the job as string
        /// </summary>
        /// <param name="jobid">job ID</param>
        /// <param name="cntrl">control number</param>
        /// <returns></returns>
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://BioHPC.org/GetOutputAsString", RequestNamespace="http://BioHPC.org/", ResponseNamespace="http://BioHPC.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetOutputAsString(string jobid, string cntrl) {
            object[] results = this.Invoke("GetOutputAsString", new object[] {
                        jobid,
                        cntrl});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginGetOutputAsString(string jobid, string cntrl, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("GetOutputAsString", new object[] {
                        jobid,
                        cntrl}, callback, asyncState);
        }
        
        /// <remarks/>
        public string EndGetOutputAsString(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GetOutputAsStringAsync(string jobid, string cntrl) {
            this.GetOutputAsStringAsync(jobid, cntrl, null);
        }
        
        /// <remarks/>
        public void GetOutputAsStringAsync(string jobid, string cntrl, object userState) {
            if ((this.GetOutputAsStringOperationCompleted == null)) {
                this.GetOutputAsStringOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetOutputAsStringOperationCompleted);
            }
            this.InvokeAsync("GetOutputAsString", new object[] {
                        jobid,
                        cntrl}, this.GetOutputAsStringOperationCompleted, userState);
        }
        
        private void OnGetOutputAsStringOperationCompleted(object arg) {
            if ((this.GetOutputAsStringCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetOutputAsStringCompleted(this, new GetOutputAsStringCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <summary>
        /// Uploads a chunk of an input file to the newly allocated job's directory on BioHPC server.
        /// </summary>
        /// <param name="jobid">job ID</param>
        /// <param name="cntrl">control number</param>
        /// <param name="f">array of bytes to be transferred</param>
        /// <param name="fileName">name of the remote file to append data to</param>
        /// <param name="newfile">if true, a new file will be created on server, otherwise the data will be appended to the existing file</param>
        /// <returns>"OK" or an error message</returns>
        /// <remarks>Will exit with error if the job has already been submitted.</remarks>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://BioHPC.org/UploadFileChunk", RequestNamespace="http://BioHPC.org/", ResponseNamespace="http://BioHPC.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string UploadFileChunk(string jobid, string cntrl, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] f, string fileName, bool newfile) {
            object[] results = this.Invoke("UploadFileChunk", new object[] {
                        jobid,
                        cntrl,
                        f,
                        fileName,
                        newfile});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginUploadFileChunk(string jobid, string cntrl, byte[] f, string fileName, bool newfile, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("UploadFileChunk", new object[] {
                        jobid,
                        cntrl,
                        f,
                        fileName,
                        newfile}, callback, asyncState);
        }
        
        /// <remarks/>
        public string EndUploadFileChunk(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void UploadFileChunkAsync(string jobid, string cntrl, byte[] f, string fileName, bool newfile) {
            this.UploadFileChunkAsync(jobid, cntrl, f, fileName, newfile, null);
        }
        
        /// <remarks/>
        public void UploadFileChunkAsync(string jobid, string cntrl, byte[] f, string fileName, bool newfile, object userState) {
            if ((this.UploadFileChunkOperationCompleted == null)) {
                this.UploadFileChunkOperationCompleted = new System.Threading.SendOrPostCallback(this.OnUploadFileChunkOperationCompleted);
            }
            this.InvokeAsync("UploadFileChunk", new object[] {
                        jobid,
                        cntrl,
                        f,
                        fileName,
                        newfile}, this.UploadFileChunkOperationCompleted, userState);
        }
        
        private void OnUploadFileChunkOperationCompleted(object arg) {
            if ((this.UploadFileChunkCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.UploadFileChunkCompleted(this, new UploadFileChunkCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <summary>Obsolete test function</summary>
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://BioHPC.org/DownloadFileChunk0", RequestNamespace="http://BioHPC.org/", ResponseNamespace="http://BioHPC.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] DownloadFileChunk0(string FName) {
            object[] results = this.Invoke("DownloadFileChunk0", new object[] {
                        FName});
            return ((byte[])(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginDownloadFileChunk0(string FName, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("DownloadFileChunk0", new object[] {
                        FName}, callback, asyncState);
        }
        
        /// <remarks/>
        public byte[] EndDownloadFileChunk0(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((byte[])(results[0]));
        }
        
        /// <remarks/>
        public void DownloadFileChunk0Async(string FName) {
            this.DownloadFileChunk0Async(FName, null);
        }
        
        /// <remarks/>
        public void DownloadFileChunk0Async(string FName, object userState) {
            if ((this.DownloadFileChunk0OperationCompleted == null)) {
                this.DownloadFileChunk0OperationCompleted = new System.Threading.SendOrPostCallback(this.OnDownloadFileChunk0OperationCompleted);
            }
            this.InvokeAsync("DownloadFileChunk0", new object[] {
                        FName}, this.DownloadFileChunk0OperationCompleted, userState);
        }
        
        private void OnDownloadFileChunk0OperationCompleted(object arg) {
            if ((this.DownloadFileChunk0Completed != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DownloadFileChunk0Completed(this, new DownloadFileChunk0CompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <summary>
        /// Retrieves a chunk of an output file from job's directory on BioHPC server to local byte array.
        /// </summary>
        /// <param name="jobid">job ID</param>
        /// <param name="cntrl">control number</param>
        /// <param name="FName">name of the remote file (without path)</param>
        /// <param name="curpos">position in the remote file to start transfer from</param>
        /// <param name="nb2tran">number of bytes to transfer</param>
        /// <returns>byte array containing the file chunk; null on error</returns>
        /// <remarks>If the number of bytes from curpos to the end of the file is less than nb2tran, all remaining bytes will be returned.</remarks>
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://BioHPC.org/DownloadFileChunk", RequestNamespace="http://BioHPC.org/", ResponseNamespace="http://BioHPC.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] DownloadFileChunk(string jobid, string cntrl, string FName, long curpos, long nb2tran) {
            object[] results = this.Invoke("DownloadFileChunk", new object[] {
                        jobid,
                        cntrl,
                        FName,
                        curpos,
                        nb2tran});
            return ((byte[])(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginDownloadFileChunk(string jobid, string cntrl, string FName, long curpos, long nb2tran, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("DownloadFileChunk", new object[] {
                        jobid,
                        cntrl,
                        FName,
                        curpos,
                        nb2tran}, callback, asyncState);
        }
        
        /// <remarks/>
        public byte[] EndDownloadFileChunk(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((byte[])(results[0]));
        }
        
        /// <remarks/>
        public void DownloadFileChunkAsync(string jobid, string cntrl, string FName, long curpos, long nb2tran) {
            this.DownloadFileChunkAsync(jobid, cntrl, FName, curpos, nb2tran, null);
        }
        
        /// <remarks/>
        public void DownloadFileChunkAsync(string jobid, string cntrl, string FName, long curpos, long nb2tran, object userState) {
            if ((this.DownloadFileChunkOperationCompleted == null)) {
                this.DownloadFileChunkOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDownloadFileChunkOperationCompleted);
            }
            this.InvokeAsync("DownloadFileChunk", new object[] {
                        jobid,
                        cntrl,
                        FName,
                        curpos,
                        nb2tran}, this.DownloadFileChunkOperationCompleted, userState);
        }
        
        private void OnDownloadFileChunkOperationCompleted(object arg) {
            if ((this.DownloadFileChunkCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DownloadFileChunkCompleted(this, new DownloadFileChunkCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <summary>
        /// Get the lenght of the job's output file (in bytes)
        /// </summary>
        /// <param name="jobid">job ID</param>
        /// <param name="cntrl">control number</param>
        /// <param name="FName">Name of the file</param>
        /// <returns>length of the file in bytes</returns>
        /// <remarks>0 (zero) returned if file empty or does not exist</remarks>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://BioHPC.org/QueryFileLength", RequestNamespace="http://BioHPC.org/", ResponseNamespace="http://BioHPC.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public long QueryFileLength(string jobid, string cntrl, string FName) {
            object[] results = this.Invoke("QueryFileLength", new object[] {
                        jobid,
                        cntrl,
                        FName});
            return ((long)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginQueryFileLength(string jobid, string cntrl, string FName, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("QueryFileLength", new object[] {
                        jobid,
                        cntrl,
                        FName}, callback, asyncState);
        }
        
        /// <remarks/>
        public long EndQueryFileLength(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((long)(results[0]));
        }
        
        /// <remarks/>
        public void QueryFileLengthAsync(string jobid, string cntrl, string FName) {
            this.QueryFileLengthAsync(jobid, cntrl, FName, null);
        }
        
        /// <remarks/>
        public void QueryFileLengthAsync(string jobid, string cntrl, string FName, object userState) {
            if ((this.QueryFileLengthOperationCompleted == null)) {
                this.QueryFileLengthOperationCompleted = new System.Threading.SendOrPostCallback(this.OnQueryFileLengthOperationCompleted);
            }
            this.InvokeAsync("QueryFileLength", new object[] {
                        jobid,
                        cntrl,
                        FName}, this.QueryFileLengthOperationCompleted, userState);
        }
        
        private void OnQueryFileLengthOperationCompleted(object arg) {
            if ((this.QueryFileLengthCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.QueryFileLengthCompleted(this, new QueryFileLengthCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <summary>
        /// Get the names of the job's output files
        /// </summary>
        /// <param name="jobid">job ID</param>
        /// <param name="cntrl">control number</param>
        /// <returns>Names of the output files (collected in structure OutFileNames)</returns>
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://BioHPC.org/RequestOutFileNames", RequestNamespace="http://BioHPC.org/", ResponseNamespace="http://BioHPC.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public OutFileNames RequestOutFileNames(string jobid, string cntrl) {
            object[] results = this.Invoke("RequestOutFileNames", new object[] {
                        jobid,
                        cntrl});
            return ((OutFileNames)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginRequestOutFileNames(string jobid, string cntrl, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("RequestOutFileNames", new object[] {
                        jobid,
                        cntrl}, callback, asyncState);
        }
        
        /// <remarks/>
        public OutFileNames EndRequestOutFileNames(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((OutFileNames)(results[0]));
        }
        
        /// <remarks/>
        public void RequestOutFileNamesAsync(string jobid, string cntrl) {
            this.RequestOutFileNamesAsync(jobid, cntrl, null);
        }
        
        /// <remarks/>
        public void RequestOutFileNamesAsync(string jobid, string cntrl, object userState) {
            if ((this.RequestOutFileNamesOperationCompleted == null)) {
                this.RequestOutFileNamesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRequestOutFileNamesOperationCompleted);
            }
            this.InvokeAsync("RequestOutFileNames", new object[] {
                        jobid,
                        cntrl}, this.RequestOutFileNamesOperationCompleted, userState);
        }
        
        private void OnRequestOutFileNamesOperationCompleted(object arg) {
            if ((this.RequestOutFileNamesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RequestOutFileNamesCompleted(this, new RequestOutFileNamesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <summary>Cancels a job</summary>
        /// <param name="jobid">job ID</param>
        /// <param name="cntrl">control number</param>
        /// <returns>report from the operation</returns>
        /// <remarks>On error, the report contains string "ERROR".</remarks>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://BioHPC.org/CancelJob", RequestNamespace="http://BioHPC.org/", ResponseNamespace="http://BioHPC.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string CancelJob(string jobid, string cntrl) {
            object[] results = this.Invoke("CancelJob", new object[] {
                        jobid,
                        cntrl});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginCancelJob(string jobid, string cntrl, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("CancelJob", new object[] {
                        jobid,
                        cntrl}, callback, asyncState);
        }
        
        /// <remarks/>
        public string EndCancelJob(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void CancelJobAsync(string jobid, string cntrl) {
            this.CancelJobAsync(jobid, cntrl, null);
        }
        
        /// <remarks/>
        public void CancelJobAsync(string jobid, string cntrl, object userState) {
            if ((this.CancelJobOperationCompleted == null)) {
                this.CancelJobOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCancelJobOperationCompleted);
            }
            this.InvokeAsync("CancelJob", new object[] {
                        jobid,
                        cntrl}, this.CancelJobOperationCompleted, userState);
        }
        
        private void OnCancelJobOperationCompleted(object arg) {
            if ((this.CancelJobCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CancelJobCompleted(this, new CancelJobCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <summary>
        /// Obtain a list of a registered user's jobs
        /// </summary>
        /// <param name="email">E-mail address</param>
        /// <param name="password">Password</param>
        /// <param name="topjobs">Number of most recent jobs to display</param>
        /// <param name="appid">Application</param>
        /// <param name="ssubdate">Submitted after this date</param>
        /// <param name="esubdate">Submitted before this date</param>
        /// <returns>"\n"-delimited string with each record corresponding to one job and formatted
        /// as in output from GetJobInfo; the first record contains info about the number of jobs found.</returns>
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://BioHPC.org/ListMyJobs", RequestNamespace="http://BioHPC.org/", ResponseNamespace="http://BioHPC.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string[] ListMyJobs(string email, string password, int topjobs, tAppId appid, string ssubdate, string esubdate) {
            object[] results = this.Invoke("ListMyJobs", new object[] {
                        email,
                        password,
                        topjobs,
                        appid,
                        ssubdate,
                        esubdate});
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginListMyJobs(string email, string password, int topjobs, tAppId appid, string ssubdate, string esubdate, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("ListMyJobs", new object[] {
                        email,
                        password,
                        topjobs,
                        appid,
                        ssubdate,
                        esubdate}, callback, asyncState);
        }
        
        /// <remarks/>
        public string[] EndListMyJobs(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public void ListMyJobsAsync(string email, string password, int topjobs, tAppId appid, string ssubdate, string esubdate) {
            this.ListMyJobsAsync(email, password, topjobs, appid, ssubdate, esubdate, null);
        }
        
        /// <remarks/>
        public void ListMyJobsAsync(string email, string password, int topjobs, tAppId appid, string ssubdate, string esubdate, object userState) {
            if ((this.ListMyJobsOperationCompleted == null)) {
                this.ListMyJobsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnListMyJobsOperationCompleted);
            }
            this.InvokeAsync("ListMyJobs", new object[] {
                        email,
                        password,
                        topjobs,
                        appid,
                        ssubdate,
                        esubdate}, this.ListMyJobsOperationCompleted, userState);
        }
        
        private void OnListMyJobsOperationCompleted(object arg) {
            if ((this.ListMyJobsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ListMyJobsCompleted(this, new ListMyJobsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <summary>
        /// Get array of names of BLAST databases available to the user
        /// </summary>
        /// <param name="email">E-mail address</param>
        /// <param name="password">Password (for registered users only)</param>
        /// <param name="dtype">"n" for DNA, "p" for protein databases</param>
        /// <remarks>If password is empty, databases available to guest users will be returned.</remarks>
        /// <returns></returns>
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://BioHPC.org/GetBlastDatabases", RequestNamespace="http://BioHPC.org/", ResponseNamespace="http://BioHPC.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string[] GetBlastDatabases(string email, string password, string dtype) {
            object[] results = this.Invoke("GetBlastDatabases", new object[] {
                        email,
                        password,
                        dtype});
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginGetBlastDatabases(string email, string password, string dtype, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("GetBlastDatabases", new object[] {
                        email,
                        password,
                        dtype}, callback, asyncState);
        }
        
        /// <remarks/>
        public string[] EndGetBlastDatabases(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public void GetBlastDatabasesAsync(string email, string password, string dtype) {
            this.GetBlastDatabasesAsync(email, password, dtype, null);
        }
        
        /// <remarks/>
        public void GetBlastDatabasesAsync(string email, string password, string dtype, object userState) {
            if ((this.GetBlastDatabasesOperationCompleted == null)) {
                this.GetBlastDatabasesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetBlastDatabasesOperationCompleted);
            }
            this.InvokeAsync("GetBlastDatabases", new object[] {
                        email,
                        password,
                        dtype}, this.GetBlastDatabasesOperationCompleted, userState);
        }
        
        private void OnGetBlastDatabasesOperationCompleted(object arg) {
            if ((this.GetBlastDatabasesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetBlastDatabasesCompleted(this, new GetBlastDatabasesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
    }
    
    /// <summary>Enumeration representing applications in BioHPC</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public enum tAppId {
        
        /// <remarks/>
        unknown,
        
        /// <remarks/>
        DBREG,
        
        /// <remarks/>
        ANY,
        
        /// <remarks/>
        P_BLAST,
        
        /// <remarks/>
        P_HMMER,
        
        /// <remarks/>
        P_IPRSCAN,
        
        /// <remarks/>
        LOOPP,
        
        /// <remarks/>
        RepeatFinder,
        
        /// <remarks/>
        MDIV,
        
        /// <remarks/>
        GenomeSeqAlign,
        
        /// <remarks/>
        URMS,
        
        /// <remarks/>
        PathogenTracker,
        
        /// <remarks/>
        iDeCAL,
        
        /// <remarks/>
        PPDB,
        
        /// <remarks/>
        P_CLUSTALW,
        
        /// <remarks/>
        MODELLER,
        
        /// <remarks/>
        MKPRF,
        
        /// <remarks/>
        MrBayes,
        
        /// <remarks/>
        MOIL,
        
        /// <remarks/>
        MKPRF_Mixed,
        
        /// <remarks/>
        RetrieveSeq,
        
        /// <remarks/>
        DNAML,
        
        /// <remarks/>
        IM,
        
        /// <remarks/>
        InStruct,
        
        /// <remarks/>
        rproj,
        
        /// <remarks/>
        t_coffee,
        
        /// <remarks/>
        parentage,
        
        /// <remarks/>
        Structure,
        
        /// <remarks/>
        migrate,
        
        /// <remarks/>
        DYRESM,
        
        /// <remarks/>
        plink,
        
        /// <remarks/>
        blastdb,
        
        /// <remarks/>
        cleanJOBS,
        
        /// <remarks/>
        IMa,
        
        /// <remarks/>
        StructuRama,
        
        /// <remarks/>
        tess,
        
        /// <remarks/>
        sfs,
        
        /// <remarks/>
        msvar,
        
        /// <remarks/>
        beast,
        
        /// <remarks/>
        omegamap,
        
        /// <remarks/>
        gbmrkv,
        
        /// <remarks/>
        best,
        
        /// <remarks/>
        slim,
        
        /// <remarks/>
        clumpp,
        
        /// <remarks/>
        stretcher,
        
        /// <remarks/>
        HlaCompletion,
        
        /// <remarks/>
        CreateEpitome,
        
        /// <remarks/>
        EpitopePrediction,
        
        /// <remarks/>
        HlaAssignment,
        
        /// <remarks/>
        PhyloD,
        
        /// <remarks/>
        FalseDiscoveryRate,
        
        /// <remarks/>
        ELAND,
        
        /// <remarks/>
        IMa2,
        
        /// <remarks/>
        lamarc,
        
        /// <remarks/>
        BayeScan,
        
        /// <remarks/>
        Colony,
        
        /// <remarks/>
        blat,
        
        /// <remarks/>
        MCMCcoal,
    }
    
    /// <summary>Input parameters of an BioHPC job - individual fields are data structures for various applications</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class AppInputData {
        
        private sfsInputData sfsField;
        
        private mdivInputData mdivField;
        
        private tcoffeeInputData tcoffeeField;
        
        private tessInputData tessField;
        
        private BLASTInputData blastField;
        
        private HMMERInputData hmmerField;
        
        private IprscanInputData iprscanField;
        
        private CreateEpitomeInputData createepitomeField;
        
        private clustalwInputData clustalwField;
        
        private EpipredInputData epipredField;

        private IMa2InputData ima2Field;
        
        private lamarcInputData lamarcField;
        
        private BayeScanInputData bayeScanField;
        
        private ColonyInputData colonyField;
        
        private MCMCcoalInputData mCMCcoalField;
        
        /// <remarks/>
        public sfsInputData sfs {
            get {
                return this.sfsField;
            }
            set {
                this.sfsField = value;
            }
        }
        
        /// <remarks/>
        public mdivInputData mdiv {
            get {
                return this.mdivField;
            }
            set {
                this.mdivField = value;
            }
        }
        
        /// <remarks/>
        public tcoffeeInputData tcoffee {
            get {
                return this.tcoffeeField;
            }
            set {
                this.tcoffeeField = value;
            }
        }
        
        /// <remarks/>
        public tessInputData tess {
            get {
                return this.tessField;
            }
            set {
                this.tessField = value;
            }
        }
        
        /// <remarks/>
        public BLASTInputData blast {
            get {
                return this.blastField;
            }
            set {
                this.blastField = value;
            }
        }
        
        /// <remarks/>
        public HMMERInputData hmmer {
            get {
                return this.hmmerField;
            }
            set {
                this.hmmerField = value;
            }
        }
        
        /// <remarks/>
        public IprscanInputData iprscan {
            get {
                return this.iprscanField;
            }
            set {
                this.iprscanField = value;
            }
        }
        
        /// <remarks/>
        public CreateEpitomeInputData createepitome {
            get {
                return this.createepitomeField;
            }
            set {
                this.createepitomeField = value;
            }
        }
        
        /// <remarks/>
        public clustalwInputData clustalw {
            get {
                return this.clustalwField;
            }
            set {
                this.clustalwField = value;
            }
        }
        
        /// <remarks/>
        public EpipredInputData epipred {
            get {
                return this.epipredField;
            }
            set {
                this.epipredField = value;
            }
        }

	    /// <remarks/>
        public IMa2InputData ima2 {
            get {
                return this.ima2Field;
            }
            set {
                this.ima2Field = value;
            }
        }
        
        /// <remarks/>
        public lamarcInputData lamarc {
            get {
                return this.lamarcField;
            }
            set {
                this.lamarcField = value;
            }
        }

	    /// <remarks/>
        public BayeScanInputData BayeScan {
            get {
                return this.bayeScanField;
            }
            set {
                this.bayeScanField = value;
            }
        }
        
        /// <remarks/>
        public ColonyInputData Colony {
            get {
                return this.colonyField;
            }
            set {
                this.colonyField = value;
            }
        }

	    /// <remarks/>
        public MCMCcoalInputData MCMCcoal {
            get {
                return this.mCMCcoalField;
            }
            set {
                this.mCMCcoalField = value;
            }
        }
    }
    
    /// <summary>Input data structure for SFS_CODE</summary>
    /// <remarks>See program web page for explanation</remarks>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class sfsInputData {
        
        private int npopField;
        
        private int nrepField;
        
        private int niterField;
        
        private string optionsField;
        
        private bool email_notifyField;
        
        /// <remarks/>
        public int Npop {
            get {
                return this.npopField;
            }
            set {
                this.npopField = value;
            }
        }
        
        /// <remarks/>
        public int Nrep {
            get {
                return this.nrepField;
            }
            set {
                this.nrepField = value;
            }
        }
        
        /// <remarks/>
        public int Niter {
            get {
                return this.niterField;
            }
            set {
                this.niterField = value;
            }
        }
        
        /// <remarks/>
        public string options {
            get {
                return this.optionsField;
            }
            set {
                this.optionsField = value;
            }
        }
        
        /// <remarks/>
        public bool email_notify {
            get {
                return this.email_notifyField;
            }
            set {
                this.email_notifyField = value;
            }
        }
    }
    
    /// <summary>Names of various output file names a job can produce</summary>
    /// <remarks>The exact meaning of various types of files depends on the application</remarks>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class OutFileNames {
        
        private string inputField;
        
        private string logField;
        
        private string output1Field;
        
        private string output2Field;
        
        private string outfinalField;
        
        private string htmField;
        
        private string outfinalzipField;
        
        private string unknownField;
        
        private string specialField;
        
        private string outlistField;
        
        /// <summary>Input file</summary>
        /// <remarks/>
        public string input {
            get {
                return this.inputField;
            }
            set {
                this.inputField = value;
            }
        }
        
        /// <summary>A log file - usually stdout from an application</summary>
        /// <remarks/>
        public string log {
            get {
                return this.logField;
            }
            set {
                this.logField = value;
            }
        }
        
        /// <summary>The "main" output file</summary>
        /// <remarks/>
        public string output1 {
            get {
                return this.output1Field;
            }
            set {
                this.output1Field = value;
            }
        }
        
        /// <summary>Secondary output file</summary>
        /// <remarks/>
        public string output2 {
            get {
                return this.output2Field;
            }
            set {
                this.output2Field = value;
            }
        }
        
        /// <remarks/>
        public string outfinal {
            get {
                return this.outfinalField;
            }
            set {
                this.outfinalField = value;
            }
        }
        
        /// <remarks/>
        public string htm {
            get {
                return this.htmField;
            }
            set {
                this.htmField = value;
            }
        }
        
        /// <summary>A zip archive of a job containing all relevant files</summary>
        /// <remarks/>
        public string outfinalzip {
            get {
                return this.outfinalzipField;
            }
            set {
                this.outfinalzipField = value;
            }
        }
        
        /// <remarks/>
        public string unknown {
            get {
                return this.unknownField;
            }
            set {
                this.unknownField = value;
            }
        }
        
        /// <remarks/>
        public string special {
            get {
                return this.specialField;
            }
            set {
                this.specialField = value;
            }
        }
        
        /// <remarks/>
        public string outlist {
            get {
                return this.outlistField;
            }
            set {
                this.outlistField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class MCMCcoalInputData {
        
        private QuerySrcType inputsourceField;
        
        private string run_typeField;
        
        private bool email_notifyField;
        
        private string jobnameField;
        
        private string inputFileField;
        
        private string outputFileField;
        
        private string controlFileField;
        
        private string heredityFileField;
        
        private string locusRateFileField;
        
        /// <remarks/>
        public QuerySrcType inputsource {
            get {
                return this.inputsourceField;
            }
            set {
                this.inputsourceField = value;
            }
        }
        
        /// <remarks/>
        public string run_type {
            get {
                return this.run_typeField;
            }
            set {
                this.run_typeField = value;
            }
        }
        
        /// <remarks/>
        public bool email_notify {
            get {
                return this.email_notifyField;
            }
            set {
                this.email_notifyField = value;
            }
        }

        /// <remarks/>
        public string jobname {
            get {
                return this.jobnameField;
            }
            set {
                this.jobnameField = value;
            }
        }
        
        /// <remarks/>
        public string InputFile {
            get {
                return this.inputFileField;
            }
            set {
                this.inputFileField = value;
            }
        }
        
        /// <remarks/>
        public string OutputFile {
            get {
                return this.outputFileField;
            }
            set {
                this.outputFileField = value;
            }
        }
        
        /// <remarks/>
        public string ControlFile {
            get {
                return this.controlFileField;
            }
            set {
                this.controlFileField = value;
            }
        }
        
        /// <remarks/>
        public string HeredityFile {
            get {
                return this.heredityFileField;
            }
            set {
                this.heredityFileField = value;
            }
        }

        /// <remarks/>
        public string LocusRateFile
        {
            get
            {
                return this.locusRateFileField;
            }
            set
            {
                this.locusRateFileField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class ColonyInputData {
        
        private QuerySrcType inputsourceField;
        
        private bool email_notifyField;
        
        private string jobnameField;
        
        private string inputFileField;
        
        private string outputFileField;
        
        /// <remarks/>
        public QuerySrcType inputsource {
            get {
                return this.inputsourceField;
            }
            set {
                this.inputsourceField = value;
            }
        }
        
        /// <remarks/>
        public bool email_notify {
            get {
                return this.email_notifyField;
            }
            set {
                this.email_notifyField = value;
            }
        }
        
        /// <remarks/>
        public string jobname {
            get {
                return this.jobnameField;
            }
            set {
                this.jobnameField = value;
            }
        }
        
        /// <remarks/>
        public string InputFile {
            get {
                return this.inputFileField;
            }
            set {
                this.inputFileField = value;
            }
        }

        /// <remarks/>
        public string OutputFile
        {
            get
            {
                return this.outputFileField;
            }
            set
            {
                this.outputFileField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class BayeScanInputData {
        
        private QuerySrcType inputsourceField;
        
        private string run_typeField;
        
        private bool email_notifyField;
        
        private string jobnameField;
        
        private string inputFileField;
        
        private string m_nField;
        
        private string m_thinField;
        
        private string m_burnField;
        
        private string m_nbpField;
        
        private string m_pilotField;
        
        private string m_lb_fisField;
        
        private string m_hb_fisField;
        
        private string m_beta_fisField;
        
        private string m_m_fisField;
        
        private string m_sd_fisField;
        
        private bool have_betaField;
        
        private bool have_meanField;
        
        private bool have_sdField;
        
        private bool m_out_pilotField;
        
        private bool m_out_alleleField;

        /// <remarks/>
        public QuerySrcType inputsource {
            get {
                return this.inputsourceField;
            }
            set {
                this.inputsourceField = value;
            }
        }
        
        /// <remarks/>
        public string run_type {
            get {
                return this.run_typeField;
            }
            set {
                this.run_typeField = value;
            }
        }
        
        /// <remarks/>
        public bool email_notify {
            get {
                return this.email_notifyField;
            }
            set {
                this.email_notifyField = value;
            }
        }
        
        /// <remarks/>
        public string jobname {
            get {
                return this.jobnameField;
            }
            set {
                this.jobnameField = value;
            }
        }
        
        /// <remarks/>
        public string InputFile {
            get {
                return this.inputFileField;
            }
            set {
                this.inputFileField = value;
            }
        }

         /// <remarks/>
        public string m_n {
            get {
                return this.m_nField;
            }
            set {
                this.m_nField = value;
            }
        }
        
        /// <remarks/>
        public string m_thin {
            get {
                return this.m_thinField;
            }
            set {
                this.m_thinField = value;
            }
        }
        
        /// <remarks/>
        public string m_burn {
            get {
                return this.m_burnField;
            }
            set {
                this.m_burnField = value;
            }
        }
        
        /// <remarks/>
        public string m_nbp {
            get {
                return this.m_nbpField;
            }
            set {
                this.m_nbpField = value;
            }
        }
        
        /// <remarks/>
        public string m_pilot {
            get {
                return this.m_pilotField;
            }
            set {
                this.m_pilotField = value;
            }
        }

        /// <remarks/>
        public string m_lb_fis {
            get {
                return this.m_lb_fisField;
            }
            set {
                this.m_lb_fisField = value;
            }
        }
        
        /// <remarks/>
        public string m_hb_fis {
            get {
                return this.m_hb_fisField;
            }
            set {
                this.m_hb_fisField = value;
            }
        }
        
        /// <remarks/>
        public string m_beta_fis {
            get {
                return this.m_beta_fisField;
            }
            set {
                this.m_beta_fisField = value;
            }
        }
        
        /// <remarks/>
        public string m_m_fis {
            get {
                return this.m_m_fisField;
            }
            set {
                this.m_m_fisField = value;
            }
        }
        
        /// <remarks/>
        public string m_sd_fis {
            get {
                return this.m_sd_fisField;
            }
            set {
                this.m_sd_fisField = value;
            }
        }

        /// <remarks/>
        public bool have_beta
        {
            get
            {
                return this.have_betaField;
            }
            set
            {
                this.have_betaField = value;
            }
        }

        /// <remarks/>
        public bool have_mean
        {
            get
            {
                return this.have_meanField;
            }
            set
            {
                this.have_meanField = value;
            }
        }

        /// <remarks/>
        public bool have_sd
        {
            get
            {
                return this.have_sdField;
            }
            set
            {
                this.have_sdField = value;
            }
        }

        /// <remarks/>
        public bool m_out_pilot
        {
            get
            {
                return this.m_out_pilotField;
            }
            set
            {
                this.m_out_pilotField = value;
            }
        }

        /// <remarks/>
        public bool m_out_allele
        {
            get
            {
                return this.m_out_alleleField;
            }
            set
            {
                this.m_out_alleleField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class lamarcInputData {
        
        private QuerySrcType inputsourceField;
        
        private string run_typeField;
        
        private bool email_notifyField;
        
        private string jobnameField;
        
        private string inputFileField;
        
        private string outputFileField;
        
        private string inputSummaryFileField;
        
        /// <remarks/>
        public QuerySrcType inputsource {
            get {
                return this.inputsourceField;
            }
            set {
                this.inputsourceField = value;
            }
        }
        
        /// <remarks/>
        public string run_type {
            get {
                return this.run_typeField;
            }
            set {
                this.run_typeField = value;
            }
        }
        
        /// <remarks/>
        public bool email_notify {
            get {
                return this.email_notifyField;
            }
            set {
                this.email_notifyField = value;
            }
        }

        /// <remarks/>
        public string jobname
        {
            get
            {
                return this.jobnameField;
            }
            set
            {
                this.jobnameField = value;
            }
        }

        /// <remarks/>
        public string InputFile
        {
            get
            {
                return this.inputFileField;
            }
            set
            {
                this.inputFileField = value;
            }
        }

        /// <remarks/>
        public string OutputFile
        {
            get
            {
                return this.outputFileField;
            }
            set
            {
                this.outputFileField = value;
            }
        }

        /// <remarks/>
        public string InputSummaryFile
        {
            get
            {
                return this.inputSummaryFileField;
            }
            set
            {
                this.inputSummaryFileField = value;
            }
        }
    }

     /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class IMa2InputData {
        
        private QuerySrcType inputsourceField;
        
        private string run_typeField;
        
        private bool email_notifyField;
        
        private string jobnameField;
        
        private string inputFileField;
        
        private string outputFileField;
        
        private string optionsOtherField;
        
        private string optionsMField;
        
        private string optionsLField;
        
        private string tifnameField;
        
        private string tifname_extField;
        
        private string prior_nameField;
        
        private string mcf_nameField;
        
        private string nested_nameField;
        
        /// <remarks/>
        public QuerySrcType inputsource {
            get {
                return this.inputsourceField;
            }
            set {
                this.inputsourceField = value;
            }
        }
        
        /// <remarks/>
        public string run_type {
            get {
                return this.run_typeField;
            }
            set {
                this.run_typeField = value;
            }
        }

        /// <remarks/>
        public bool email_notify
        {
            get
            {
                return this.email_notifyField;
            }
            set
            {
                this.email_notifyField = value;
            }
        }

        /// <remarks/>
        public string jobname
        {
            get
            {
                return this.jobnameField;
            }
            set
            {
                this.jobnameField = value;
            }
        }

        /// <remarks/>
        public string InputFile
        {
            get
            {
                return this.inputFileField;
            }
            set
            {
                this.inputFileField = value;
            }
        }

        /// <remarks/>
        public string OutputFile
        {
            get
            {
                return this.outputFileField;
            }
            set
            {
                this.outputFileField = value;
            }
        }

        /// <remarks/>
        public string optionsOther
        {
            get
            {
                return this.optionsOtherField;
            }
            set
            {
                this.optionsOtherField = value;
            }
        }

        /// <remarks/>
        public string optionsM
        {
            get
            {
                return this.optionsMField;
            }
            set
            {
                this.optionsMField = value;
            }
        }

        /// <remarks/>
        public string optionsL
        {
            get
            {
                return this.optionsLField;
            }
            set
            {
                this.optionsLField = value;
            }
        }

        /// <remarks/>
        public string tifname
        {
            get
            {
                return this.tifnameField;
            }
            set
            {
                this.tifnameField = value;
            }
        }

        /// <remarks/>
        public string tifname_ext
        {
            get
            {
                return this.tifname_extField;
            }
            set
            {
                this.tifname_extField = value;
            }
        }

        /// <remarks/>
        public string prior_name
        {
            get
            {
                return this.prior_nameField;
            }
            set
            {
                this.prior_nameField = value;
            }
        }

        /// <remarks/>
        public string mcf_name
        {
            get
            {
                return this.mcf_nameField;
            }
            set
            {
                this.mcf_nameField = value;
            }
        }

        /// <remarks/>
        public string nested_name
        {
            get
            {
                return this.nested_nameField;
            }
            set
            {
                this.nested_nameField = value;
            }
        }
    }


    
    /// <summary>Input data structure for Epitope Prediciton</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class EpipredInputData {
        
        private QuerySrcType inputsourceField;
        
        private string inputstringField;
        
        private bool useHasHeaderField;
        
        private bool useMerLengthField;
        
        private EppMerLength merLengthField;
        
        private bool useWithinDField;
        
        private int withinDField;
        
        private bool useHLASetField;
        
        private EppHLASet hLASetField;
        
        private bool useModelOnlyField;
        
        private bool useShowByField;
        
        private EppShowBy showByField;
        
        private bool useLANLIED03062007Field;
        
        private bool email_notifyField;
        
        private string inputFileField;
        
        /// <summary>Source of the input</summary>
        /// <remarks/>
        public QuerySrcType inputsource {
            get {
                return this.inputsourceField;
            }
            set {
                this.inputsourceField = value;
            }
        }
        
        /// <summary>If inputsource is "Paste", this string should contain the input</summary>
        /// <remarks/>
        public string inputstring {
            get {
                return this.inputstringField;
            }
            set {
                this.inputstringField = value;
            }
        }
        
        /// <summary>True if the inout contains a header line</summary>
        /// <remarks/>
        public bool useHasHeader {
            get {
                return this.useHasHeaderField;
            }
            set {
                this.useHasHeaderField = value;
            }
        }
        
        /// <summary>If true, mer length has to be specified</summary>
        /// <remarks/>
        public bool useMerLength {
            get {
                return this.useMerLengthField;
            }
            set {
                this.useMerLengthField = value;
            }
        }
        
        /// <summary>Mer lenghth selection</summary>
        /// <remarks/>
        public EppMerLength MerLength {
            get {
                return this.merLengthField;
            }
            set {
                this.merLengthField = value;
            }
        }

        /// <summary>Limits the scan for epitopes to mers such that at least one edge 
        /// of the mer is withing D amino acids of the center of the peptide given for scanning.  
        /// If the input peptide has even length, we use left of center for center.</summary>
        /// <remarks/>
        public bool useWithinD {
            get {
                return this.useWithinDField;
            }
            set {
                this.useWithinDField = value;
            }
        }
        
        /// <summary>Radius D referred to in option useWithinD</summary>
        /// <remarks/>
        public int WithinD {
            get {
                return this.withinDField;
            }
            set {
                this.withinDField = value;
            }
        }
        
        /// <summary>Specifies whether or not to use an HLA set</summary>
        /// <remarks/>
        public bool useHLASet {
            get {
                return this.useHLASetField;
            }
            set {
                this.useHLASetField = value;
            }
        }
        
        /// <summary>Which HLA set to use</summary>
        /// <remarks/>
        public EppHLASet HLASet {
            get {
                return this.hLASetField;
            }
            set {
                this.hLASetField = value;
            }
        }
        
        /// <summary>If given then uses the model's probability even if a peptide is on a source list.
        /// Otherwise, peptides on a source list are given a probability of 1.0.  
        /// (The "source list" is the list of epitopes and HLAs used to train the model).</summary>
        /// <remarks/>
        public bool useModelOnly {
            get {
                return this.useModelOnlyField;
            }
            set {
                this.useModelOnlyField = value;
            }
        }
        
        /// <summary>Enforce format of the maximum probability prediction(s).</summary>
        /// <remarks/>
        public bool useShowBy {
            get {
                return this.useShowByField;
            }
            set {
                this.useShowByField = value;
            }
        }
        
        /// <summary>Format of the maximum probability prediction(s).</summary>
        /// <remarks/>
        public EppShowBy ShowBy {
            get {
                return this.showByField;
            }
            set {
                this.showByField = value;
            }
        }
        
        /// <remarks/>
        public bool useLANLIED03062007 {
            get {
                return this.useLANLIED03062007Field;
            }
            set {
                this.useLANLIED03062007Field = value;
            }
        }
        
        /// <summary>Whether or not to send e-mail notifications about the job</summary>
        /// <remarks/>
        public bool email_notify {
            get {
                return this.email_notifyField;
            }
            set {
                this.email_notifyField = value;
            }
        }
        
        /// <summary>If inputsource is Upload", the name of the input file expected on server side.</summary>
        /// <remarks/>
        public string InputFile {
            get {
                return this.inputFileField;
            }
            set {
                this.inputFileField = value;
            }
        }
    }
    
    /// <summary>Possible mer lengths in Epitope Prediciton application</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public enum EppMerLength {
        
        /// <summary>Scan over mers of varoous lengths</summary>
        /// <remarks/>
        scan,
        
        /// <remarks/>
        l_8,
        
        /// <remarks/>
        l_9,
        
        /// <remarks/>
        l_10,
        
        /// <remarks/>
        l_11,

        /// <summary>No scanning; the whole input peptide is scored. </summary>
        /// <remarks/>
        given,
    }
    
    /// <summary>How to select HLAs for scanning. 
    /// Among the hlas considered, the best will be reported.</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public enum EppHLASet {

        /// <summary>use the hla from the input file</summary>
        /// <remarks/>
        singleton,
        
        /// <summary>use the  supertype from the file, scanning all its hlas</summary>
        /// <remarks/>
        supertype,

        /// <summary>scan all know hlas</summary>
        /// <remarks/>
        all,
    }
    
    /// <summary>How to report maximum probability predictions</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public enum EppShowBy {
        
        /// <summary>Report for each input line</summary>
        /// <remarks/>
        all,
        
        /// <summary>Report for each input line and hla</summary>
        /// <remarks/>
        hla,

        /// <summary>Report for each input line and peptide length</summary>
        /// <remarks/>
        length,

        /// <summary>Report for each input line and hla and peptide length</summary>
        /// <remarks/>
        hlaAndLength,

        /// <summary>Report every predicition for each input line</summary>
        /// <remarks/>
        doNotGroup,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class ClwStrGapPenalty {
        
        private double helixField;
        
        private double strandField;
        
        private double loopField;
        
        private double terminalField;
        
        private int helixInsidePositionField;
        
        private int helixOutsidePositionField;
        
        private int strandInsidePositionField;
        
        private int strandOutsidePositionField;
        
        /// <remarks/>
        public double helix {
            get {
                return this.helixField;
            }
            set {
                this.helixField = value;
            }
        }
        
        /// <remarks/>
        public double strand {
            get {
                return this.strandField;
            }
            set {
                this.strandField = value;
            }
        }
        
        /// <remarks/>
        public double loop {
            get {
                return this.loopField;
            }
            set {
                this.loopField = value;
            }
        }
        
        /// <remarks/>
        public double terminal {
            get {
                return this.terminalField;
            }
            set {
                this.terminalField = value;
            }
        }
        
        /// <remarks/>
        public int HelixInsidePosition {
            get {
                return this.helixInsidePositionField;
            }
            set {
                this.helixInsidePositionField = value;
            }
        }
        
        /// <remarks/>
        public int HelixOutsidePosition {
            get {
                return this.helixOutsidePositionField;
            }
            set {
                this.helixOutsidePositionField = value;
            }
        }
        
        /// <remarks/>
        public int StrandInsidePosition {
            get {
                return this.strandInsidePositionField;
            }
            set {
                this.strandInsidePositionField = value;
            }
        }
        
        /// <remarks/>
        public int StrandOutsidePosition {
            get {
                return this.strandOutsidePositionField;
            }
            set {
                this.strandOutsidePositionField = value;
            }
        }
    }
    
    /// <summary>Parameters of structure alignment in ClustalW</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class ClwStrAlignment {
        
        private bool use1stProfileField;
        
        private bool use2ndProfileField;
        
        private ClwStrGapPenalty strAlnGapField;
        
        /// <summary>whether or not to use the first profile</summary>
        /// <remarks/>
        public bool Use1stProfile {
            get {
                return this.use1stProfileField;
            }
            set {
                this.use1stProfileField = value;
            }
        }
        
        /// <summary>whether or not to use the second profile</summary>
        /// <remarks/>
        public bool Use2ndProfile {
            get {
                return this.use2ndProfileField;
            }
            set {
                this.use2ndProfileField = value;
            }
        }
        
        /// <summary>Gap penalty parameters for structure alignment</summary>
        /// <remarks/>
        public ClwStrGapPenalty StrAlnGap {
            get {
                return this.strAlnGapField;
            }
            set {
                this.strAlnGapField = value;
            }
        }
    }
    
    /// <summary>File names for the profiles used by ClustalW</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class ClwProfileParams {
        
        private string profile1Field;
        
        private string secFile1Field;
        
        private string profile2Field;
        
        private string secFile2Field;
        
        /// <remarks/>
        public string Profile1 {
            get {
                return this.profile1Field;
            }
            set {
                this.profile1Field = value;
            }
        }
        
        /// <remarks/>
        public string SecFile1 {
            get {
                return this.secFile1Field;
            }
            set {
                this.secFile1Field = value;
            }
        }
        
        /// <remarks/>
        public string Profile2 {
            get {
                return this.profile2Field;
            }
            set {
                this.profile2Field = value;
            }
        }
        
        /// <remarks/>
        public string SecFile2 {
            get {
                return this.secFile2Field;
            }
            set {
                this.secFile2Field = value;
            }
        }
    }
    
    /// <summary>Tree parameters used by ClustalW</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class ClwTreeParams {
        
        private bool kimuraField;
        
        private int bootstrapField;
        
        private ClwLabel labelField;
        
        private ClwOutTreeFormat outtreeformatField;
        
        /// <summary>whether or not to use Kimura correction</summary>
        /// <remarks/>
        public bool kimura {
            get {
                return this.kimuraField;
            }
            set {
                this.kimuraField = value;
            }
        }
        
        /// <summary>Number of bootstrap steps</summary>
        /// <remarks/>
        public int bootstrap {
            get {
                return this.bootstrapField;
            }
            set {
                this.bootstrapField = value;
            }
        }
        
        /// <remarks/>
        public ClwLabel label {
            get {
                return this.labelField;
            }
            set {
                this.labelField = value;
            }
        }
        
        /// <summary>Output format of the tree</summary>
        /// <remarks/>
        public ClwOutTreeFormat outtreeformat {
            get {
                return this.outtreeformatField;
            }
            set {
                this.outtreeformatField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public enum ClwLabel {
        
        /// <remarks/>
        Default,
        
        /// <remarks/>
        Node,
        
        /// <remarks/>
        Branch,
    }
    
    /// <summary>tree output formats in ClustalW</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public enum ClwOutTreeFormat {
        
        /// <remarks/>
        ClustalW,
        
        /// <remarks/>
        Philip,
        
        /// <remarks/>
        Nexus,
    }
    
    /// <summary>Gap penalty parameters in CLustalW</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class ClwGapPenalty {
        
        private double openingField;
        
        private double extensionField;
        
        private int separationField;
        
        private bool endsField;
        
        private bool hydrophylicField;
        
        /// <remarks/>
        public double opening {
            get {
                return this.openingField;
            }
            set {
                this.openingField = value;
            }
        }
        
        /// <remarks/>
        public double extension {
            get {
                return this.extensionField;
            }
            set {
                this.extensionField = value;
            }
        }
        
        /// <remarks/>
        public int separation {
            get {
                return this.separationField;
            }
            set {
                this.separationField = value;
            }
        }
        
        /// <remarks/>
        public bool ends {
            get {
                return this.endsField;
            }
            set {
                this.endsField = value;
            }
        }
        
        /// <remarks/>
        public bool hydrophylic {
            get {
                return this.hydrophylicField;
            }
            set {
                this.hydrophylicField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class clustalwInputData {
        
        private QuerySrcType inputsourceField;
        
        private string inputstringField;
        
        private bool isDNAField;
        
        private ClwActions actionField;
        
        private ClwOutFormat outputformatField;
        
        private ClwResultOrder resultorderField;
        
        private ClwProteinMatrix proteinmatrixField;
        
        private ClwDNAMatrix dnamatrixField;
        
        private ClwGapPenalty gappenaltyField;
        
        private ClwTreeParams treeparamsField;
        
        private ClwProfileParams profileparamsField;
        
        private ClwStrAlignment structaloptsField;
        
        private string inputFileField;
        
        private bool email_notifyField;
        
        /// <summary>How the main input (FASTA file with sequences to be aligned) is provided</summary>
        /// <remarks/>
        public QuerySrcType inputsource {
            get {
                return this.inputsourceField;
            }
            set {
                this.inputsourceField = value;
            }
        }
        
        /// <summary>String containing the zsequences (in FASAT format) to be aligned. 
        /// Relevant if inputsource is set to paste. </summary>
        /// <remarks/>
        public string inputstring {
            get {
                return this.inputstringField;
            }
            set {
                this.inputstringField = value;
            }
        }
        
        /// <remarks/>
        public bool isDNA {
            get {
                return this.isDNAField;
            }
            set {
                this.isDNAField = value;
            }
        }
        
        /// <summary>action to be executed by ClustalW</summary>
        /// <remarks/>
        public ClwActions action {
            get {
                return this.actionField;
            }
            set {
                this.actionField = value;
            }
        }
        
        /// <summary>format of the output from ClustalW</summary>
        /// <remarks/>
        public ClwOutFormat outputformat {
            get {
                return this.outputformatField;
            }
            set {
                this.outputformatField = value;
            }
        }
        
        /// <remarks/>
        public ClwResultOrder resultorder {
            get {
                return this.resultorderField;
            }
            set {
                this.resultorderField = value;
            }
        }
        
        /// <remarks/>
        public ClwProteinMatrix proteinmatrix {
            get {
                return this.proteinmatrixField;
            }
            set {
                this.proteinmatrixField = value;
            }
        }
        
        /// <remarks/>
        public ClwDNAMatrix dnamatrix {
            get {
                return this.dnamatrixField;
            }
            set {
                this.dnamatrixField = value;
            }
        }
        
        /// <summary>Gap penalty parameteres (non-structural alignment)</summary>
        /// <remarks/>
        public ClwGapPenalty gappenalty {
            get {
                return this.gappenaltyField;
            }
            set {
                this.gappenaltyField = value;
            }
        }
        
        /// <summary>Tree parameters (relevant if selected action is "tree"
        /// )</summary>
        /// <remarks/>
        public ClwTreeParams treeparams {
            get {
                return this.treeparamsField;
            }
            set {
                this.treeparamsField = value;
            }
        }
        
        /// <remarks/>
        public ClwProfileParams profileparams {
            get {
                return this.profileparamsField;
            }
            set {
                this.profileparamsField = value;
            }
        }
        
        /// <summary>Options ofr structre alignment</summary>
        /// <remarks/>
        public ClwStrAlignment structalopts {
            get {
                return this.structaloptsField;
            }
            set {
                this.structaloptsField = value;
            }
        }
        
        /// <summary>Name of the input file (with sequences ot be aligned) as expected on the server</summary>
        /// <remarks/>
        public string InputFile {
            get {
                return this.inputFileField;
            }
            set {
                this.inputFileField = value;
            }
        }
        
        /// <summary>whether or not to send e-mail notifications about the job</summary>
        /// <remarks/>
        public bool email_notify {
            get {
                return this.email_notifyField;
            }
            set {
                this.email_notifyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public enum ClwActions {
        
        /// <remarks/>
        align,
        
        /// <remarks/>
        profile,
        
        /// <remarks/>
        tree,
        
        /// <remarks/>
        bootstrap,
    }
    
    /// <summary>output format for alignment produced by ClustalW</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public enum ClwOutFormat {
        
        /// <remarks/>
        ClustalW,
        
        /// <remarks/>
        CGC,
        
        /// <remarks/>
        CDE,
        
        /// <remarks/>
        phillip,
        
        /// <remarks/>
        pir,
        
        /// <remarks/>
        nexus,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public enum ClwResultOrder {
        
        /// <remarks/>
        DEFAULT,
        
        /// <remarks/>
        INPUT,
        
        /// <remarks/>
        ALIGNED,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public enum ClwProteinMatrix {
        
        /// <remarks/>
        BLOSUM,
        
        /// <remarks/>
        PAM,
        
        /// <remarks/>
        GONNET,
        
        /// <remarks/>
        ID,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public enum ClwDNAMatrix {
        
        /// <remarks/>
        IOB,
        
        /// <remarks/>
        ClustalW,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class CreateEpitomeInputData {
        
        private QuerySrcType inputsourceField;
        
        private int stoplengthField;
        
        private bool email_notifyField;
        
        private string inputstringField;
        
        private string inputFileField;
        
        /// <summary>Upload inout file or paste in</summary>
        /// <remarks/>
        public QuerySrcType inputsource {
            get {
                return this.inputsourceField;
            }
            set {
                this.inputsourceField = value;
            }
        }
        
        /// <summary>See program description</summary>
        /// <remarks/>
        public int stoplength {
            get {
                return this.stoplengthField;
            }
            set {
                this.stoplengthField = value;
            }
        }
        
        /// <summary>Whether or not to send job notification e-mails</summary>
        /// <remarks/>
        public bool email_notify {
            get {
                return this.email_notifyField;
            }
            set {
                this.email_notifyField = value;
            }
        }
        
        /// <summary>If inoutsource is "paste", put the inout string here</summary>
        /// <remarks/>
        public string inputstring {
            get {
                return this.inputstringField;
            }
            set {
                this.inputstringField = value;
            }
        }
        
        /// <summary>The name of the input file the server expexts as input 
        /// (relevnt if inputsource is set to "upload")</summary>
        /// <remarks/>
        public string InputFile {
            get {
                return this.inputFileField;
            }
            set {
                this.inputFileField = value;
            }
        }
    }
    
    /// <summary>Specifies how the input is provided</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public enum QuerySrcType {
        
        /// <summary>Upload file</summary>
        /// <remarks/>
        upload,
        
        /// <summary>Copy from a location on BioHPC server (local drive or ftp server)</summary>
        /// <remarks/>
        copy,
        
        /// <summary>Paste input as string</summary>
        /// <remarks/>
        paste,
    }
    
    /// <summary>Run parameters for InterProScan</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class IprscanRunparams {
        
        private int intimeout1seqField;
        
        private int timeoutfactorField;
        
        private Time4Fctr t4factorField;
        
        private bool keeptimeoutField;
        
        private bool keeptifaverageField;
        
        private bool email_notifyField;
        
        /// <summary>Initial timeout for processing of one sequence</summary>
        /// <remarks/>
        public int intimeout1seq {
            get {
                return this.intimeout1seqField;
            }
            set {
                this.intimeout1seqField = value;
            }
        }
        
        /// <summary>Obtain 1-sequence timeout dynamically by multiplying the 
        /// average or maximum processing time by this factor</summary>
        /// <remarks/>
        public int timeoutfactor {
            get {
                return this.timeoutfactorField;
            }
            set {
                this.timeoutfactorField = value;
            }
        }
        
        /// <summary>Obtain 1-sequence timeout dynamically by multiplying this 
        /// time by timeoutfactor</summary>
        /// <remarks/>
        public Time4Fctr t4factor {
            get {
                return this.t4factorField;
            }
            set {
                this.t4factorField = value;
            }
        }
        
        /// <summary>Keep the initial 1-sequence timeout - do NOT adjust dynamically</summary>
        /// <remarks/>
        public bool keeptimeout {
            get {
                return this.keeptimeoutField;
            }
            set {
                this.keeptimeoutField = value;
            }
        }
        
        /// <summary>If dynamically adjusted 1-sequence timeout is set as multiple of
        /// average 1-sequence execution time, constrain it to be  longer or equal
        /// to the initial timeout.</summary>
        /// <remarks/>
        public bool keeptifaverage {
            get {
                return this.keeptifaverageField;
            }
            set {
                this.keeptifaverageField = value;
            }
        }
        
        /// <summary>Whether or not to send e-mail notifications about the job</summary>
        /// <remarks/>
        public bool email_notify {
            get {
                return this.email_notifyField;
            }
            set {
                this.email_notifyField = value;
            }
        }
    }
    
    /// <summary>1-task execution time to base the 1-task timeout on (for parallel applications)</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public enum Time4Fctr {
        
        /// <remarks/>
        average,
        
        /// <remarks/>
        maximum,
    }
    
    /// <summary>Advanced options of InterProScan</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class AdvancedOptins {
        
        private bool iprlookupField;
        
        private bool gotermsField;
        
        private bool nocrcField;
        
        /// <remarks/>
        public bool iprlookup {
            get {
                return this.iprlookupField;
            }
            set {
                this.iprlookupField = value;
            }
        }
        
        /// <remarks/>
        public bool goterms {
            get {
                return this.gotermsField;
            }
            set {
                this.gotermsField = value;
            }
        }
        
        /// <remarks/>
        public bool nocrc {
            get {
                return this.nocrcField;
            }
            set {
                this.nocrcField = value;
            }
        }
    }
    
    /// <summary>Scanning applications available to run within InterProScan</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class IprscanApps {
        
        private bool blastprodomField;
        
        private bool coilsField;
        
        private bool gene3dField;
        
        private bool hmmpantherField;
        
        private bool hmmpirField;
        
        private bool hmmpfamField;
        
        private bool hmmsmartField;
        
        private bool hmmtigrField;
        
        private bool fprintscanField;
        
        private bool patternscanField;
        
        private bool profilescanField;
        
        private bool superfamilyField;
        
        private bool segField;
        
        /// <remarks/>
        public bool blastprodom {
            get {
                return this.blastprodomField;
            }
            set {
                this.blastprodomField = value;
            }
        }
        
        /// <remarks/>
        public bool coils {
            get {
                return this.coilsField;
            }
            set {
                this.coilsField = value;
            }
        }
        
        /// <remarks/>
        public bool gene3d {
            get {
                return this.gene3dField;
            }
            set {
                this.gene3dField = value;
            }
        }
        
        /// <remarks/>
        public bool hmmpanther {
            get {
                return this.hmmpantherField;
            }
            set {
                this.hmmpantherField = value;
            }
        }
        
        /// <remarks/>
        public bool hmmpir {
            get {
                return this.hmmpirField;
            }
            set {
                this.hmmpirField = value;
            }
        }
        
        /// <remarks/>
        public bool hmmpfam {
            get {
                return this.hmmpfamField;
            }
            set {
                this.hmmpfamField = value;
            }
        }
        
        /// <remarks/>
        public bool hmmsmart {
            get {
                return this.hmmsmartField;
            }
            set {
                this.hmmsmartField = value;
            }
        }
        
        /// <remarks/>
        public bool hmmtigr {
            get {
                return this.hmmtigrField;
            }
            set {
                this.hmmtigrField = value;
            }
        }
        
        /// <remarks/>
        public bool fprintscan {
            get {
                return this.fprintscanField;
            }
            set {
                this.fprintscanField = value;
            }
        }
        
        /// <remarks/>
        public bool patternscan {
            get {
                return this.patternscanField;
            }
            set {
                this.patternscanField = value;
            }
        }
        
        /// <remarks/>
        public bool profilescan {
            get {
                return this.profilescanField;
            }
            set {
                this.profilescanField = value;
            }
        }
        
        /// <remarks/>
        public bool superfamily {
            get {
                return this.superfamilyField;
            }
            set {
                this.superfamilyField = value;
            }
        }
        
        /// <remarks/>
        public bool seg {
            get {
                return this.segField;
            }
            set {
                this.segField = value;
            }
        }
    }
    
    /// <summary>Options to InterProScan search</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class IprscanOptions {
        
        private int minquerylengthField;
        
        private IOutputFormat formatField;
        
        private Codon codonField;
        
        private IprscanApps applicationsField;
        
        private SequenceType seqtypeField;
        
        private AdvancedOptins advoptField;
        
        /// <summary>Minumum length of a query sequence to be considered</summary>
        /// <remarks/>
        public int minquerylength {
            get {
                return this.minquerylengthField;
            }
            set {
                this.minquerylengthField = value;
            }
        }
        
        /// <summary>Output format selection</summary>
        /// <remarks/>
        public IOutputFormat format {
            get {
                return this.formatField;
            }
            set {
                this.formatField = value;
            }
        }
        
        /// <summary>Codon table (for DNA queries)</summary>
        /// <remarks/>
        public Codon codon {
            get {
                return this.codonField;
            }
            set {
                this.codonField = value;
            }
        }
        
        /// <summary>Scanning applications to run</summary>
        /// <remarks/>
        public IprscanApps applications {
            get {
                return this.applicationsField;
            }
            set {
                this.applicationsField = value;
            }
        }
        
        /// <summary>Query type. If set to "DNA", a 6-frame translation will be performed
        /// before executing the IprScan search.</summary>
        /// <remarks/>
        public SequenceType seqtype {
            get {
                return this.seqtypeField;
            }
            set {
                this.seqtypeField = value;
            }
        }
        
        /// <summary>Advanced options</summary>
        /// <remarks/>
        public AdvancedOptins advopt {
            get {
                return this.advoptField;
            }
            set {
                this.advoptField = value;
            }
        }
    }
    
    /// <summary>Requested InterProScan output format</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public enum IOutputFormat {
        
        /// <remarks/>
        Text,
        
        /// <remarks/>
        Raw,
        
        /// <remarks/>
        XML,
        
        /// <remarks/>
        HTML,
        
        /// <remarks/>
        ebixml,
    }
    /// <summary>Possible selections of the codon table used by InterProScan
    /// when the query is a DNA sequence.</summary>
    /// <remarks></remarks>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public enum Codon {
        
        /// <summary>Standard</summary>
        /// <remarks />
        c_0,

        /// <summary>Standard with alternative initiation codons</summary>
        /// <remarks />
        c_1,
        
        /// <summary>Vertebrate Mitochodrial</summary>
        /// <remarks/>
        c_2,
        
        /// <summary>Yeast Mitochondrial</summary>
        /// <remarks/>
        c_3,

        /// <summary>Mold, Protozoan, Coelenterate Mitochondrial, Myco/Spiroplasma</summary>
        /// <remarks/>
        c_4,

        /// <summary>Invertebrate Mitochondrial</summary>
        /// <remarks/>
        c_5,

        /// <summary>Ciliate Macronuclear and Dasycladacean</summary>
        /// <remarks/>
        c_6,

        /// <summary>Echinoderm Mitochondrial</summary>
        /// <remarks/>
        c_9,

        /// <summary>Euplotid Nuclear</summary>
        /// <remarks/>
        c_10,

        /// <summary>Bacterial</summary>
        /// <remarks/>
        c_11,

        /// <summary>Alternative Yeast Nuclear</summary>
        /// <remarks/>
        c_12,

        /// <summary>Ascidian Mitochondrial</summary>
        /// <remarks/>
        c_13,

        /// <summary> Flatworm Mitochondrial</summary>
        /// <remarks/>
        c_14,

        /// <summary> Blepharisma Macronuclear</summary>
        /// <remarks/>
        c_15,

        /// <summary>Chlorophycean Mitochondrial</summary>
        /// <remarks/>
        c_16,

        /// <summary>Trematode Mitochondrial</summary>
        /// <remarks/>
        c_21,

        /// <summary>Scenedesmus obliquus</summary>
        /// <remarks/>
        c_22,

        /// <summary>Thraustochytrium Mitochondrial</summary>
        /// <remarks/>
        c_23,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public enum SequenceType {
        
        /// <remarks/>
        Protein,
        
        /// <remarks/>
        DNA,
    }
    
    /// <summary>Input data structure for InterProScan</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class IprscanInputData {
        
        private QuerySrcType querysourceField;
        
        private IprscanOptions optionsField;
        
        private IprscanRunparams runparamsField;
        
        private string inputFileNameField;
        
        private string queryField;
        
        /// <summary>How to pass the sequence to the server</summary>
        /// <remarks/>
        public QuerySrcType querysource {
            get {
                return this.querysourceField;
            }
            set {
                this.querysourceField = value;
            }
        }
        
        /// <summary>Options to InterProScan</summary>
        /// <remarks/>
        public IprscanOptions options {
            get {
                return this.optionsField;
            }
            set {
                this.optionsField = value;
            }
        }
        
        /// <summary>Additional, BioHPC-specific options to InterProScan</summary>
        /// <remarks/>
        public IprscanRunparams runparams {
            get {
                return this.runparamsField;
            }
            set {
                this.runparamsField = value;
            }
        }
        
        /// <summary>Name of the inout file the server will expect if querysource is seto "upload"</summary>
        /// <remarks/>
        public string InputFileName {
            get {
                return this.inputFileNameField;
            }
            set {
                this.inputFileNameField = value;
            }
        }
        
        /// <summary>Query string (in FASTA format) - used if querysource is se tio "paste"</summary>
        /// <remarks/>
        public string query {
            get {
                return this.queryField;
            }
            set {
                this.queryField = value;
            }
        }
    }
    
    /// <summary>Additional parameters of HMMER run</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class HMMERRunparams {
        
        private int intimeout1seqField;
        
        private int timeoutfactorField;
        
        private Time4Fctr t4factorField;
        
        private bool keeptimeoutField;
        
        private bool email_notifyField;
        
        /// <summary>Initial timeout for processing of a single sequence</summary>
        /// <remarks/>
        public int intimeout1seq {
            get {
                return this.intimeout1seqField;
            }
            set {
                this.intimeout1seqField = value;
            }
        }
        
        /// <summary>Set 1-sequence timeout dynamically as "timeoutfactor" times ther average or 
        /// the maximum 1-sequence processing time. </summary>
        /// <remarks/>
        public int timeoutfactor {
            get {
                return this.timeoutfactorField;
            }
            set {
                this.timeoutfactorField = value;
            }
        }

        /// <summary>Set 1-sequence timeout dynamically as "timeoutfactor" times ther average or 
        /// the maximum 1-sequence processing time. </summary>
        /// <remarks/>
        public Time4Fctr t4factor {
            get {
                return this.t4factorField;
            }
            set {
                this.t4factorField = value;
            }
        }
        
        /// <summary>Keep 1-sequence timeout always eqault to the initial value (do not adjust dynamically)</summary>
        /// <remarks/>
        public bool keeptimeout {
            get {
                return this.keeptimeoutField;
            }
            set {
                this.keeptimeoutField = value;
            }
        }
        
        /// <summary>Whether or not to send e-mail notifications about the job</summary>
        /// <remarks/>
        public bool email_notify {
            get {
                return this.email_notifyField;
            }
            set {
                this.email_notifyField = value;
            }
        }
    }
    
    /// <summary>Options used by hmmpfam executable of HMMER</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class HMMEROptions {
        
        private double ecutField;
        
        private int maxtargetsField;
        
        private string extraoptionsField;
        
        private HMMEROutputFormat formatField;
        
        /// <summary>Cutoff e-value</summary>
        /// <remarks/>
        public double ecut {
            get {
                return this.ecutField;
            }
            set {
                this.ecutField = value;
            }
        }
        
        /// <summary>Maximum number of hist to report (per query sequence)</summary>
        /// <remarks/>
        public int maxtargets {
            get {
                return this.maxtargetsField;
            }
            set {
                this.maxtargetsField = value;
            }
        }
        
        /// <summary>Extra HMMER options</summary>
        /// <remarks/>
        public string extraoptions {
            get {
                return this.extraoptionsField;
            }
            set {
                this.extraoptionsField = value;
            }
        }
        
        /// <summary>Requested format of HMMER output</summary>
        /// <remarks/>
        public HMMEROutputFormat format {
            get {
                return this.formatField;
            }
            set {
                this.formatField = value;
            }
        }
    }
    
    /// <summary>Requested HMMER output format</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public enum HMMEROutputFormat {
        
        /// <remarks/>
        CBSU,
        
        /// <remarks/>
        RAW,
    }
    
    /// <summary>Input data for a HMMER run</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class HMMERInputData {
        
        private QuerySrcType querysourceField;
        
        private HMMEROptions optionsField;
        
        private HMMERRunparams runparamsField;
        
        private HMMERDatabase databaseField;
        
        private string inputFileNameField;
        
        private string queryField;
        
        /// <summary>How the query is passed on to the server</summary>
        /// <remarks/>
        public QuerySrcType querysource {
            get {
                return this.querysourceField;
            }
            set {
                this.querysourceField = value;
            }
        }
        
        /// <summary>HMMER options to use</summary>
        /// <remarks/>
        public HMMEROptions options {
            get {
                return this.optionsField;
            }
            set {
                this.optionsField = value;
            }
        }
        
        /// <summary>Other parameters of a HMMER run (for use by experts only - otherwise please use default)</summary>
        /// <remarks/>
        public HMMERRunparams runparams {
            get {
                return this.runparamsField;
            }
            set {
                this.runparamsField = value;
            }
        }
        
        /// <summary>HMMER profile database to use</summary>
        /// <remarks/>
        public HMMERDatabase database {
            get {
                return this.databaseField;
            }
            set {
                this.databaseField = value;
            }
        }
        
        /// Name of rthe input file the server expacts if querysource is set to "upload"
        /// <remarks/>
        public string InputFileName {
            get {
                return this.inputFileNameField;
            }
            set {
                this.inputFileNameField = value;
            }
        }
        
        /// <summary>String with query (in FASTA format) if querysource is set to "paste"</summary>
        /// <remarks/>
        public string query {
            get {
                return this.queryField;
            }
            set {
                this.queryField = value;
            }
        }
    }
    
    /// <summary>Selection of HMMER profile database to use</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public enum HMMERDatabase {
        
        /// <remarks/>
        Pfam_ls,
        
        /// <remarks/>
        Pfam_fs,
    }
    
    /// <summary>Describes the database to be used in a BLAST search</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class BLASTDatabase {
        
        private string databaseField;
        
        private string dbfilenameField;
        
        private bool formatoTField;
        
        /// <summary>A "|"-delimited string containing names of databases to be used</summary>
        /// <remarks/>
        public string database {
            get {
                return this.databaseField;
            }
            set {
                this.databaseField = value;
            }
        }
        
        /// <summary>If user's own database is to be used, this is the name of the fasta file containing this database</summary>
        /// <remarks/>
        public string dbfilename {
            get {
                return this.dbfilenameField;
            }
            set {
                this.dbfilenameField = value;
            }
        }
        
        /// <summary>whether or not to format the database with -o T option</summary>
        /// <remarks/>
        public bool formatoT {
            get {
                return this.formatoTField;
            }
            set {
                this.formatoTField = value;
            }
        }
    }
    
    /// <summary>Some other, BioHPC-specific parameters of a BLAST job. For experts only.</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class BLASTRunparams {
        
        private int blocksizeField;
        
        private int intimeout1seqField;
        
        private int timeoutfactorField;
        
        private Time4Fctr t4factorField;
        
        private bool keeptimeoutField;
        
        private bool email_notifyField;
        
        /// <summary>How many sequences to send to a worker process at ne time</summary>
        /// <remarks/>
        public int blocksize {
            get {
                return this.blocksizeField;
            }
            set {
                this.blocksizeField = value;
            }
        }
        
        /// <summary>Initial timeout for 1-sequence search</summary>
        /// <remarks/>
        public int intimeout1seq {
            get {
                return this.intimeout1seqField;
            }
            set {
                this.intimeout1seqField = value;
            }
        }
        
        /// <summary>Singel-sequence timeout will be dynamically adjusted as this multiple of the maximum encountered or average run time.</summary>
        /// <remarks/>
        public int timeoutfactor {
            get {
                return this.timeoutfactorField;
            }
            set {
                this.timeoutfactorField = value;
            }
        }
        
        /// <summary>When adjusting timeout dynamically, use the average or maximum encountered running time.</summary>
        /// <remarks/>
        public Time4Fctr t4factor {
            get {
                return this.t4factorField;
            }
            set {
                this.t4factorField = value;
            }
        }
        
        /// <summary>Keep the initial timeout throughout the job</summary>
        /// <remarks/>
        public bool keeptimeout {
            get {
                return this.keeptimeoutField;
            }
            set {
                this.keeptimeoutField = value;
            }
        }
        
        /// <summary>Sent e-mail notifications about the job</summary>
        /// <remarks/>
        public bool email_notify {
            get {
                return this.email_notifyField;
            }
            set {
                this.email_notifyField = value;
            }
        }
    }
    
    /// <summary>BLAST options (mostly those passed on to blastall)</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class BLASTOptions {
        
        private double ecutField;
        
        private int maxtargetsField;
        
        private int minquerylengthField;
        
        private LowCompFilter lcompfilterField;
        
        private string extraoptionsField;
        
        private OutputFormat formatField;
        
        private double percent_idField;
        
        private Matrix matrixField;
        
        private bool iterativeField;
        
        /// <remarks/>
        public double ecut {
            get {
                return this.ecutField;
            }
            set {
                this.ecutField = value;
            }
        }
        
        /// <summary>Maximum number of hits to report (per query sequence per databases scanned)</summary>
        /// <remarks/>
        public int maxtargets {
            get {
                return this.maxtargetsField;
            }
            set {
                this.maxtargetsField = value;
            }
        }
        
        /// <summary>Query sequences shorter than this will be ignored</summary>
        /// <remarks/>
        public int minquerylength {
            get {
                return this.minquerylengthField;
            }
            set {
                this.minquerylengthField = value;
            }
        }
        
        /// <summary>Low complexity filter to apply</summary>
        /// <remarks/>
        public LowCompFilter lcompfilter {
            get {
                return this.lcompfilterField;
            }
            set {
                this.lcompfilterField = value;
            }
        }
        
        /// <summary>String with extra BLAST options (not covered in this class)</summary>
        /// <remarks/>
        public string extraoptions {
            get {
                return this.extraoptionsField;
            }
            set {
                this.extraoptionsField = value;
            }
        }
        
        /// <summary>Requested output format</summary>
        /// <remarks/>
        public OutputFormat format {
            get {
                return this.formatField;
            }
            set {
                this.formatField = value;
            }
        }
        
        /// <summary>Requested persent identity (relevant for megablast only)</summary>
        /// <remarks/>
        public double percent_id {
            get {
                return this.percent_idField;
            }
            set {
                this.percent_idField = value;
            }
        }
        
        /// <summary>Similarity matrix for protein BLAST</summary>
        /// <remarks/>
        public Matrix matrix {
            get {
                return this.matrixField;
            }
            set {
                this.matrixField = value;
            }
        }
        
        /// <summary>Is tis run iterative (not really implemented yet)</summary>
        /// <remarks/>
        public bool iterative {
            get {
                return this.iterativeField;
            }
            set {
                this.iterativeField = value;
            }
        }
    }
    
    /// <summary>Low complexity filter to use in a BLAST run</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public enum LowCompFilter {
        
        /// <remarks/>
        Yes,
        
        /// <remarks/>
        No,
        
        /// <remarks/>
        SeedsOnly,
    }
    
    /// <summary>Requested BLAST output format</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public enum OutputFormat {
        
        /// <remarks/>
        m8,
        
        /// <remarks/>
        m0,
        
        /// <remarks/>
        CBSUStandard,
        
        /// <remarks/>
        CBSUStandard2,
        
        /// <remarks/>
        CBSU_m8,
        
        /// <remarks/>
        CBSU_m8_extended,
        
        /// <remarks/>
        m1,
        
        /// <remarks/>
        m2,
        
        /// <remarks/>
        m3,
        
        /// <remarks/>
        m4,
        
        /// <remarks/>
        m5,
        
        /// <remarks/>
        m6,
        
        /// <remarks/>
        XML,
        
        /// <remarks/>
        HTML,
    }
    
    /// <summary>Amino acid similarity matrix</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public enum Matrix {
        
        /// <remarks/>
        PAM30,
        
        /// <remarks/>
        PAM70,
        
        /// <remarks/>
        BLOSUM80,
        
        /// <remarks/>
        BLOSUM62,
        
        /// <remarks/>
        BLOSUM45,
    }
    
    /// <summary>Describes input to a BLAST job</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class BLASTInputData {
        
        private QuerySrcType querysourceField;
        
        private BLASTprogram programField;
        
        private BLASTOptions optionsField;
        
        private BLASTRunparams runparamsField;
        
        private BLASTDatabase databaseField;
        
        private string inputFileNameField;
        
        private string queryField;
        
        /// <summary>How the query is provided (uploaded, copied from a location on BioHPC server, or as a string)</summary>
        /// <remarks/>
        public QuerySrcType querysource {
            get {
                return this.querysourceField;
            }
            set {
                this.querysourceField = value;
            }
        }
        
        /// <summary>BLAST program to use</summary>
        /// <remarks/>
        public BLASTprogram program {
            get {
                return this.programField;
            }
            set {
                this.programField = value;
            }
        }
        
        /// <summary>BLAST options (mostly of blastall)</summary>
        /// <remarks/>
        public BLASTOptions options {
            get {
                return this.optionsField;
            }
            set {
                this.optionsField = value;
            }
        }
        
        /// <summary>Other parameters of a BLAST run</summary>
        /// <remarks/>
        public BLASTRunparams runparams {
            get {
                return this.runparamsField;
            }
            set {
                this.runparamsField = value;
            }
        }
        
        /// <summary>BLAST database specification</summary>
        /// <remarks/>
        public BLASTDatabase database {
            get {
                return this.databaseField;
            }
            set {
                this.databaseField = value;
            }
        }
        
        /// <summary>Server-side name of the query file to use while uploading input</summary>
        /// <remarks/>
        public string InputFileName {
            get {
                return this.inputFileNameField;
            }
            set {
                this.inputFileNameField = value;
            }
        }
        
        /// <summary>String containing query in FASTA format - used only ifquerysource is "paste"</summary>
        /// <remarks/>
        public string query {
            get {
                return this.queryField;
            }
            set {
                this.queryField = value;
            }
        }
    }
    
    /// <remarks>Selection of BLAST program</remarks>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public enum BLASTprogram {
        
        /// <remarks/>
        blastn,
        
        /// <remarks/>
        blastp,
        
        /// <remarks/>
        blastx,
        
        /// <remarks/>
        tblastn,
        
        /// <remarks/>
        tblastx,
        
        /// <remarks/>
        megablast,
    }
    
    /// <summary>Input data structure for TESS</summary>
    /// <remarks>See program description for explanation</remarks>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class tessInputData {
        
        private bool withadmixtureField;
        
        private int repeatsField;
        
        private string optionsField;
        
        private bool email_notifyField;
        
        /// <remarks/>
        public bool withadmixture {
            get {
                return this.withadmixtureField;
            }
            set {
                this.withadmixtureField = value;
            }
        }
        
        /// <summary>How many times to repeat the run
        /// (with the same input and different random number seeds)</summary>
        /// <remarks/>
        public int repeats {
            get {
                return this.repeatsField;
            }
            set {
                this.repeatsField = value;
            }
        }
        
        /// <summary>user-supplied TESS options</summary>
        /// <remarks/>
        public string options {
            get {
                return this.optionsField;
            }
            set {
                this.optionsField = value;
            }
        }
        
        /// <remarks/>
        public bool email_notify {
            get {
                return this.email_notifyField;
            }
            set {
                this.email_notifyField = value;
            }
        }
    }
    
    /// <summary>Input structure for TCOFFEE</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class tcoffeeInputData {
        
        private bool align_on_existingField;
        
        private bool sequence_type_autoField;
        
        private bool sequence_type_DNAField;
        
        private string queryField;
        
        private string existing_alignmentField;
        
        private bool realign_everythingField;
        
        private string add_optionsField;
        
        private bool email_notifyField;
        
        /// <summary>"true" is alignment on existing alignment is to be made, 
        /// "false" if a new alignment is reequested</summary>
        /// <remarks/>
        public bool align_on_existing {
            get {
                return this.align_on_existingField;
            }
            set {
                this.align_on_existingField = value;
            }
        }
        
        /// <summary>Whether or not to automatically detect sequence type</summary>
        /// <remarks/>
        public bool sequence_type_auto {
            get {
                return this.sequence_type_autoField;
            }
            set {
                this.sequence_type_autoField = value;
            }
        }
        
        /// <summary>"true" if sequence type is DNA</summary>
        /// <remarks/>
        public bool sequence_type_DNA {
            get {
                return this.sequence_type_DNAField;
            }
            set {
                this.sequence_type_DNAField = value;
            }
        }
        
        /// <summary>String with sequences to be aligned</summary>
        /// <remarks/>
        public string query {
            get {
                return this.queryField;
            }
            set {
                this.queryField = value;
            }
        }
        
        /// <summary>Existing alignment (if "align_on_existing" is "true")</summary>
        /// <remarks/>
        public string existing_alignment {
            get {
                return this.existing_alignmentField;
            }
            set {
                this.existing_alignmentField = value;
            }
        }
        
        /// <summary>Realign all sequences, including the ones from the existing alignment (if present)</summary>
        /// <remarks/>
        public bool realign_everything {
            get {
                return this.realign_everythingField;
            }
            set {
                this.realign_everythingField = value;
            }
        }
        
        /// <summary>String containing any additional user options accepted by T_COFFEE</summary>
        /// <remarks/>
        public string add_options {
            get {
                return this.add_optionsField;
            }
            set {
                this.add_optionsField = value;
            }
        }
        
        /// <summary>Whether or not to send e-mail notifications about the job</summary>
        /// <remarks/>
        public bool email_notify {
            get {
                return this.email_notifyField;
            }
            set {
                this.email_notifyField = value;
            }
        }
    }
    
    /// <summary>Input data structure for MDIV</summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BioHPC.org/")]
    public partial class mdivInputData {
        
        private int modelField;
        
        private long seedField;
        
        private int numrepField;
        
        private int burnintimeField;
        
        private double mmaxField;
        
        private double tmaxField;
        
        private double qmaxField;
        
        private string inputFileNameField;
        
        private int runrepsField;
        
        private bool email_notifyField;
        
        /// <remarks/>
        public int model {
            get {
                return this.modelField;
            }
            set {
                this.modelField = value;
            }
        }
        
        /// <summary>Random number seed to use</summary>
        /// <remarks/>
        public long seed {
            get {
                return this.seedField;
            }
            set {
                this.seedField = value;
            }
        }
        
        /// <summary>Number of Monte Carlo steps to perform</summary>
        /// <remarks/>
        public int numrep {
            get {
                return this.numrepField;
            }
            set {
                this.numrepField = value;
            }
        }
        
        /// <summary>Number of bur-in steps to perform before collecting sample</summary>
        /// <remarks/>
        public int burnintime {
            get {
                return this.burnintimeField;
            }
            set {
                this.burnintimeField = value;
            }
        }
        
        /// <summary>Maximum scaled migration rate</summary>
        /// <remarks/>
        public double Mmax {
            get {
                return this.mmaxField;
            }
            set {
                this.mmaxField = value;
            }
        }
        
        /// <summary>Maximum Theta parameter</summary>
        /// <remarks/>
        public double Tmax {
            get {
                return this.tmaxField;
            }
            set {
                this.tmaxField = value;
            }
        }
        
        /// <summary>Maximum time from divergence</summary>
        /// <remarks/>
        public double Qmax {
            get {
                return this.qmaxField;
            }
            set {
                this.qmaxField = value;
            }
        }
        
        /// <summary>Input file name expected by the server</summary>
        /// <remarks/>
        public string InputFileName {
            get {
                return this.inputFileNameField;
            }
            set {
                this.inputFileNameField = value;
            }
        }
        
        /// <summary> How many times to repeat the run (with same input and parameters
        /// but different random number seeds) </summary>
        /// <remarks/>
        public int runreps {
            get {
                return this.runrepsField;
            }
            set {
                this.runrepsField = value;
            }
        }
        
        /// <summary>Whether or not to send e-mail notifications about the job</summary>
        /// <remarks/>
        public bool email_notify {
            get {
                return this.email_notifyField;
            }
            set {
                this.email_notifyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void HelloWorldCompletedEventHandler(object sender, HelloWorldCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class HelloWorldCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal HelloWorldCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void GetJobInfoCompletedEventHandler(object sender, GetJobInfoCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetJobInfoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetJobInfoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void CreateJobCompletedEventHandler(object sender, CreateJobCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CreateJobCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CreateJobCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void SubmitJobCompletedEventHandler(object sender, SubmitJobCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SubmitJobCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal SubmitJobCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void InitializeApplicationParamsCompletedEventHandler(object sender, InitializeApplicationParamsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class InitializeApplicationParamsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal InitializeApplicationParamsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public AppInputData Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((AppInputData)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void InitializeInputParamsCompletedEventHandler(object sender, InitializeInputParamsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class InitializeInputParamsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal InitializeInputParamsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public AppInputData Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((AppInputData)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void GetOutputAsStringCompletedEventHandler(object sender, GetOutputAsStringCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetOutputAsStringCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetOutputAsStringCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void UploadFileChunkCompletedEventHandler(object sender, UploadFileChunkCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class UploadFileChunkCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal UploadFileChunkCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void DownloadFileChunk0CompletedEventHandler(object sender, DownloadFileChunk0CompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DownloadFileChunk0CompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal DownloadFileChunk0CompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public byte[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void DownloadFileChunkCompletedEventHandler(object sender, DownloadFileChunkCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DownloadFileChunkCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal DownloadFileChunkCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public byte[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void QueryFileLengthCompletedEventHandler(object sender, QueryFileLengthCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class QueryFileLengthCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal QueryFileLengthCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public long Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((long)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void RequestOutFileNamesCompletedEventHandler(object sender, RequestOutFileNamesCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RequestOutFileNamesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal RequestOutFileNamesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public OutFileNames Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((OutFileNames)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void CancelJobCompletedEventHandler(object sender, CancelJobCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CancelJobCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CancelJobCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void ListMyJobsCompletedEventHandler(object sender, ListMyJobsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ListMyJobsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ListMyJobsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void GetBlastDatabasesCompletedEventHandler(object sender, GetBlastDatabasesCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetBlastDatabasesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetBlastDatabasesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string[])(this.results[0]));
            }
        }
    }
}
