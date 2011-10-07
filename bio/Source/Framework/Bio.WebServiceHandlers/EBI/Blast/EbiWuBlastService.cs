namespace Bio.Web.Blast
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Web.Services;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "JDispatcherServiceHttpBinding", Namespace = "http://soap.jdispatcher.ebi.ac.uk")]
    public partial class JDispatcherService : System.Web.Services.Protocols.SoapHttpClientProtocol
    {

        private System.Threading.SendOrPostCallback runOperationCompleted;

        private System.Threading.SendOrPostCallback getStatusOperationCompleted;

        private System.Threading.SendOrPostCallback getResultTypesOperationCompleted;

        private System.Threading.SendOrPostCallback getResultOperationCompleted;

        private System.Threading.SendOrPostCallback getParametersOperationCompleted;

        private System.Threading.SendOrPostCallback getParameterDetailsOperationCompleted;

        /// <remarks/>
        public JDispatcherService()
        {
            this.Url = "http://www.ebi.ac.uk/Tools/services/soap/wublast";
        }

        /// <remarks/>
        public event runCompletedEventHandler runCompleted;

        /// <remarks/>
        public event getStatusCompletedEventHandler getStatusCompleted;

        /// <remarks/>
        public event getResultTypesCompletedEventHandler getResultTypesCompleted;

        /// <remarks/>
        public event getResultCompletedEventHandler getResultCompleted;

        /// <remarks/>
        public event getParametersCompletedEventHandler getParametersCompleted;

        /// <remarks/>
        public event getParameterDetailsCompletedEventHandler getParameterDetailsCompleted;

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:Run", RequestNamespace = "http://soap.jdispatcher.ebi.ac.uk", ResponseNamespace = "http://soap.jdispatcher.ebi.ac.uk", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("jobId", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string run([System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)] string email, [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)] string title, [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)] InputParameters parameters)
        {
            object[] results = this.Invoke("run", new object[] {
                    email,
                    title,
                    parameters});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult Beginrun(string email, string title, InputParameters parameters, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("run", new object[] {
                    email,
                    title,
                    parameters}, callback, asyncState);
        }

        /// <remarks/>
        public string Endrun(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void runAsync(string email, string title, InputParameters parameters)
        {
            this.runAsync(email, title, parameters, null);
        }

        /// <remarks/>
        public void runAsync(string email, string title, InputParameters parameters, object userState)
        {
            if ((this.runOperationCompleted == null))
            {
                this.runOperationCompleted = new System.Threading.SendOrPostCallback(this.OnrunOperationCompleted);
            }
            this.InvokeAsync("run", new object[] {
                    email,
                    title,
                    parameters}, this.runOperationCompleted, userState);
        }

        private void OnrunOperationCompleted(object arg)
        {
            if ((this.runCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.runCompleted(this, new runCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:GetStatus", RequestNamespace = "http://soap.jdispatcher.ebi.ac.uk", ResponseNamespace = "http://soap.jdispatcher.ebi.ac.uk", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("status", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string getStatus([System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)] string jobId)
        {
            object[] results = this.Invoke("getStatus", new object[] {
                    jobId});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BegingetStatus(string jobId, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("getStatus", new object[] {
                    jobId}, callback, asyncState);
        }

        /// <remarks/>
        public string EndgetStatus(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void getStatusAsync(string jobId)
        {
            this.getStatusAsync(jobId, null);
        }

        /// <remarks/>
        public void getStatusAsync(string jobId, object userState)
        {
            if ((this.getStatusOperationCompleted == null))
            {
                this.getStatusOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetStatusOperationCompleted);
            }
            this.InvokeAsync("getStatus", new object[] {
                    jobId}, this.getStatusOperationCompleted, userState);
        }

        private void OngetStatusOperationCompleted(object arg)
        {
            if ((this.getStatusCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getStatusCompleted(this, new getStatusCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:GetResultTypes", RequestNamespace = "http://soap.jdispatcher.ebi.ac.uk", ResponseNamespace = "http://soap.jdispatcher.ebi.ac.uk", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlArrayAttribute("resultTypes", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [return: System.Xml.Serialization.XmlArrayItemAttribute("type", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public wsResultType[] getResultTypes([System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)] string jobId)
        {
            object[] results = this.Invoke("getResultTypes", new object[] {
                    jobId});
            return ((wsResultType[])(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BegingetResultTypes(string jobId, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("getResultTypes", new object[] {
                    jobId}, callback, asyncState);
        }

        /// <remarks/>
        public wsResultType[] EndgetResultTypes(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((wsResultType[])(results[0]));
        }

        /// <remarks/>
        public void getResultTypesAsync(string jobId)
        {
            this.getResultTypesAsync(jobId, null);
        }

        /// <remarks/>
        public void getResultTypesAsync(string jobId, object userState)
        {
            if ((this.getResultTypesOperationCompleted == null))
            {
                this.getResultTypesOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetResultTypesOperationCompleted);
            }
            this.InvokeAsync("getResultTypes", new object[] {
                    jobId}, this.getResultTypesOperationCompleted, userState);
        }

        private void OngetResultTypesOperationCompleted(object arg)
        {
            if ((this.getResultTypesCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getResultTypesCompleted(this, new getResultTypesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:GetResult", RequestNamespace = "http://soap.jdispatcher.ebi.ac.uk", ResponseNamespace = "http://soap.jdispatcher.ebi.ac.uk", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("output", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "base64Binary", IsNullable = true)]
        public byte[] getResult([System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)] string jobId, [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)] string type, [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)] [System.Xml.Serialization.XmlArrayItemAttribute("parameter", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)] wsRawOutputParameter[] parameters)
        {
            object[] results = this.Invoke("getResult", new object[] {
                    jobId,
                    type,
                    parameters});
            return ((byte[])(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BegingetResult(string jobId, string type, wsRawOutputParameter[] parameters, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("getResult", new object[] {
                    jobId,
                    type,
                    parameters}, callback, asyncState);
        }

        /// <remarks/>
        public byte[] EndgetResult(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((byte[])(results[0]));
        }

        /// <remarks/>
        public void getResultAsync(string jobId, string type, wsRawOutputParameter[] parameters)
        {
            this.getResultAsync(jobId, type, parameters, null);
        }

        /// <remarks/>
        public void getResultAsync(string jobId, string type, wsRawOutputParameter[] parameters, object userState)
        {
            if ((this.getResultOperationCompleted == null))
            {
                this.getResultOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetResultOperationCompleted);
            }
            this.InvokeAsync("getResult", new object[] {
                    jobId,
                    type,
                    parameters}, this.getResultOperationCompleted, userState);
        }

        private void OngetResultOperationCompleted(object arg)
        {
            if ((this.getResultCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getResultCompleted(this, new getResultCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:GetParameters", RequestNamespace = "http://soap.jdispatcher.ebi.ac.uk", ResponseNamespace = "http://soap.jdispatcher.ebi.ac.uk", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlArrayAttribute("parameters", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [return: System.Xml.Serialization.XmlArrayItemAttribute("id", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public string[] getParameters()
        {
            object[] results = this.Invoke("getParameters", new object[0]);
            return ((string[])(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BegingetParameters(System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("getParameters", new object[0], callback, asyncState);
        }

        /// <remarks/>
        public string[] EndgetParameters(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string[])(results[0]));
        }

        /// <remarks/>
        public void getParametersAsync()
        {
            this.getParametersAsync(null);
        }

        /// <remarks/>
        public void getParametersAsync(object userState)
        {
            if ((this.getParametersOperationCompleted == null))
            {
                this.getParametersOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetParametersOperationCompleted);
            }
            this.InvokeAsync("getParameters", new object[0], this.getParametersOperationCompleted, userState);
        }

        private void OngetParametersOperationCompleted(object arg)
        {
            if ((this.getParametersCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getParametersCompleted(this, new getParametersCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:GetParameterDetails", RequestNamespace = "http://soap.jdispatcher.ebi.ac.uk", ResponseNamespace = "http://soap.jdispatcher.ebi.ac.uk", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("parameterDetails", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public wsParameterDetails getParameterDetails([System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)] string parameterId)
        {
            object[] results = this.Invoke("getParameterDetails", new object[] {
                    parameterId});
            return ((wsParameterDetails)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BegingetParameterDetails(string parameterId, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("getParameterDetails", new object[] {
                    parameterId}, callback, asyncState);
        }

        /// <remarks/>
        public wsParameterDetails EndgetParameterDetails(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((wsParameterDetails)(results[0]));
        }

        /// <remarks/>
        public void getParameterDetailsAsync(string parameterId)
        {
            this.getParameterDetailsAsync(parameterId, null);
        }

        /// <remarks/>
        public void getParameterDetailsAsync(string parameterId, object userState)
        {
            if ((this.getParameterDetailsOperationCompleted == null))
            {
                this.getParameterDetailsOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetParameterDetailsOperationCompleted);
            }
            this.InvokeAsync("getParameterDetails", new object[] {
                    parameterId}, this.getParameterDetailsOperationCompleted, userState);
        }

        private void OngetParameterDetailsOperationCompleted(object arg)
        {
            if ((this.getParameterDetailsCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getParameterDetailsCompleted(this, new getParameterDetailsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://soap.jdispatcher.ebi.ac.uk")]
    public partial class InputParameters
    {

        private string programField;

        private string expField;

        private System.Nullable<int> alignmentsField;

        private bool alignmentsFieldSpecified;

        private System.Nullable<int> scoresField;

        private bool scoresFieldSpecified;

        private System.Nullable<int> alignField;

        private bool alignFieldSpecified;

        private string matrixField;

        private string statsField;

        private string sensitivityField;

        private string topcombonField;

        private System.Nullable<bool> viewfilterField;

        private bool viewfilterFieldSpecified;

        private string filterField;

        private string strandField;

        private string sortField;

        private string stypeField;

        private string sequenceField;

        private string[] databaseField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string program
        {
            get
            {
                return this.programField;
            }
            set
            {
                this.programField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public string exp
        {
            get
            {
                return this.expField;
            }
            set
            {
                this.expField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public System.Nullable<int> alignments
        {
            get
            {
                return this.alignmentsField;
            }
            set
            {
                this.alignmentsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool alignmentsSpecified
        {
            get
            {
                return this.alignmentsFieldSpecified;
            }
            set
            {
                this.alignmentsFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public System.Nullable<int> scores
        {
            get
            {
                return this.scoresField;
            }
            set
            {
                this.scoresField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool scoresSpecified
        {
            get
            {
                return this.scoresFieldSpecified;
            }
            set
            {
                this.scoresFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public System.Nullable<int> align
        {
            get
            {
                return this.alignField;
            }
            set
            {
                this.alignField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool alignSpecified
        {
            get
            {
                return this.alignFieldSpecified;
            }
            set
            {
                this.alignFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public string matrix
        {
            get
            {
                return this.matrixField;
            }
            set
            {
                this.matrixField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public string stats
        {
            get
            {
                return this.statsField;
            }
            set
            {
                this.statsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public string sensitivity
        {
            get
            {
                return this.sensitivityField;
            }
            set
            {
                this.sensitivityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public string topcombon
        {
            get
            {
                return this.topcombonField;
            }
            set
            {
                this.topcombonField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public System.Nullable<bool> viewfilter
        {
            get
            {
                return this.viewfilterField;
            }
            set
            {
                this.viewfilterField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool viewfilterSpecified
        {
            get
            {
                return this.viewfilterFieldSpecified;
            }
            set
            {
                this.viewfilterFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public string filter
        {
            get
            {
                return this.filterField;
            }
            set
            {
                this.filterField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public string strand
        {
            get
            {
                return this.strandField;
            }
            set
            {
                this.strandField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public string sort
        {
            get
            {
                return this.sortField;
            }
            set
            {
                this.sortField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string stype
        {
            get
            {
                return this.stypeField;
            }
            set
            {
                this.stypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public string sequence
        {
            get
            {
                return this.sequenceField;
            }
            set
            {
                this.sequenceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string[] database
        {
            get
            {
                return this.databaseField;
            }
            set
            {
                this.databaseField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://soap.jdispatcher.ebi.ac.uk")]
    public partial class wsProperty
    {

        private string keyField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string key
        {
            get
            {
                return this.keyField;
            }
            set
            {
                this.keyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://soap.jdispatcher.ebi.ac.uk")]
    public partial class wsParameterValue
    {

        private string labelField;

        private string valueField;

        private bool defaultValueField;

        private wsProperty[] propertiesField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string label
        {
            get
            {
                return this.labelField;
            }
            set
            {
                this.labelField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public bool defaultValue
        {
            get
            {
                return this.defaultValueField;
            }
            set
            {
                this.defaultValueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("property", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public wsProperty[] properties
        {
            get
            {
                return this.propertiesField;
            }
            set
            {
                this.propertiesField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://soap.jdispatcher.ebi.ac.uk")]
    public partial class wsParameterDetails
    {

        private string nameField;

        private string descriptionField;

        private string typeField;

        private wsParameterValue[] valuesField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("value", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public wsParameterValue[] values
        {
            get
            {
                return this.valuesField;
            }
            set
            {
                this.valuesField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://soap.jdispatcher.ebi.ac.uk")]
    public partial class wsRawOutputParameter
    {

        private string nameField;

        private string[] valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string[] value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://soap.jdispatcher.ebi.ac.uk")]
    public partial class wsResultType
    {

        private string descriptionField;

        private string fileSuffixField;

        private string identifierField;

        private string labelField;

        private string mediaTypeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public string description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string fileSuffix
        {
            get
            {
                return this.fileSuffixField;
            }
            set
            {
                this.fileSuffixField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string identifier
        {
            get
            {
                return this.identifierField;
            }
            set
            {
                this.identifierField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public string label
        {
            get
            {
                return this.labelField;
            }
            set
            {
                this.labelField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string mediaType
        {
            get
            {
                return this.mediaTypeField;
            }
            set
            {
                this.mediaTypeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void runCompletedEventHandler(object sender, runCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class runCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal runCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void getStatusCompletedEventHandler(object sender, getStatusCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getStatusCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal getStatusCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void getResultTypesCompletedEventHandler(object sender, getResultTypesCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getResultTypesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal getResultTypesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public wsResultType[] Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((wsResultType[])(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void getResultCompletedEventHandler(object sender, getResultCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getResultCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal getResultCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public byte[] Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void getParametersCompletedEventHandler(object sender, getParametersCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getParametersCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal getParametersCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string[] Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string[])(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void getParameterDetailsCompletedEventHandler(object sender, getParameterDetailsCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getParameterDetailsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal getParameterDetailsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public wsParameterDetails Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((wsParameterDetails)(this.results[0]));
            }
        }
    }
}
