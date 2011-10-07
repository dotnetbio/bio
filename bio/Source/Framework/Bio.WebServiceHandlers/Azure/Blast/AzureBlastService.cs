namespace Microsoft.CCF.BlastDemo
{
    using System.Runtime.Serialization;
    
    /// <summary>
    /// Empty summary tags added to all public methods and properties to 
    /// avoid compile time warnings (or) errors
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="BlastSerivceRequest", Namespace="http://schemas.datacontract.org/2004/07/Microsoft.CCF.BlastDemo")]
    public partial class BlastSerivceRequest : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string DatabaseNameField;
        
        private string InputBlobField;
        
        private string InputContaienrField;
        
        private byte[] InputContentField;
        
        private string OptionMField;
        
        private string OwnerField;
        
        private int ParitionNumberField;
        
        private string ProgramNameField;
        
        private string TitleField;
        
        /// <summary>
        /// 
        /// </summary>
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DatabaseName
        {
            get
            {
                return this.DatabaseNameField;
            }
            set
            {
                this.DatabaseNameField = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string InputBlob
        {
            get
            {
                return this.InputBlobField;
            }
            set
            {
                this.InputBlobField = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string InputContaienr
        {
            get
            {
                return this.InputContaienrField;
            }
            set
            {
                this.InputContaienrField = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte[] InputContent
        {
            get
            {
                return this.InputContentField;
            }
            set
            {
                this.InputContentField = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string OptionM
        {
            get
            {
                return this.OptionMField;
            }
            set
            {
                this.OptionMField = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Owner
        {
            get
            {
                return this.OwnerField;
            }
            set
            {
                this.OwnerField = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ParitionNumber
        {
            get
            {
                return this.ParitionNumberField;
            }
            set
            {
                this.ParitionNumberField = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProgramName
        {
            get
            {
                return this.ProgramNameField;
            }
            set
            {
                this.ProgramNameField = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Title
        {
            get
            {
                return this.TitleField;
            }
            set
            {
                this.TitleField = value;
            }
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="JobStatusResponse", Namespace="http://schemas.datacontract.org/2004/07/Microsoft.CCF.BlastDemo")]
    public partial class JobStatusResponse : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private System.Nullable<System.DateTime> DateCompletedField;
        
        private System.DateTime DateCreatedField;
        
        private System.Nullable<System.DateTime> DateStartedField;
        
        private float ProgressField;
        
        private string StatusField;
        
        private string TitleField;
        
        /// <summary>
        /// 
        /// </summary>
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<System.DateTime> DateCompleted
        {
            get
            {
                return this.DateCompletedField;
            }
            set
            {
                this.DateCompletedField = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime DateCreated
        {
            get
            {
                return this.DateCreatedField;
            }
            set
            {
                this.DateCreatedField = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<System.DateTime> DateStarted
        {
            get
            {
                return this.DateStartedField;
            }
            set
            {
                this.DateStartedField = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public float Progress
        {
            get
            {
                return this.ProgressField;
            }
            set
            {
                this.ProgressField = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Status
        {
            get
            {
                return this.StatusField;
            }
            set
            {
                this.StatusField = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Title
        {
            get
            {
                return this.TitleField;
            }
            set
            {
                this.TitleField = value;
            }
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="BlastFault", Namespace="http://schemas.datacontract.org/2004/07/Microsoft.CCF.BlastDemo")]
    public partial class BlastFault : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string DetailField;
        
        private string ProblemTypeField;
        
        /// <summary>
        /// 
        /// </summary>
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Detail
        {
            get
            {
                return this.DetailField;
            }
            set
            {
                this.DetailField = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProblemType
        {
            get
            {
                return this.ProblemTypeField;
            }
            set
            {
                this.ProblemTypeField = value;
            }
        }
    }
}

/// <summary>
/// 
/// </summary>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(ConfigurationName="IBlastService")]
public interface IBlastService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBlastService/SubmitJob", ReplyAction="http://tempuri.org/IBlastService/SubmitJobResponse")]
    [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.CCF.BlastDemo.BlastFault), Action="http://tempuri.org/IBlastService/SubmitJobBlastFaultFault", Name="BlastFault", Namespace="http://schemas.datacontract.org/2004/07/Microsoft.CCF.BlastDemo")]
    System.Guid SubmitJob(Microsoft.CCF.BlastDemo.BlastSerivceRequest request);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBlastService/QueryJobStatus", ReplyAction="http://tempuri.org/IBlastService/QueryJobStatusResponse")]
    Microsoft.CCF.BlastDemo.JobStatusResponse QueryJobStatus(System.Guid jobId);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBlastService/GetJobResult", ReplyAction="http://tempuri.org/IBlastService/GetJobResultResponse")]
    string GetJobResult(System.Guid jobId);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBlastService/KillJob", ReplyAction="http://tempuri.org/IBlastService/KillJobResponse")]
    bool KillJob(System.Guid jobId);
}

/// <summary>
/// 
/// </summary>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
public interface IBlastServiceChannel : IBlastService, System.ServiceModel.IClientChannel
{
}

/// <summary>
/// 
/// </summary>
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
public partial class BlastServiceClient : System.ServiceModel.ClientBase<IBlastService>, IBlastService
{

    /// <summary>
    /// 
    /// </summary>
    public BlastServiceClient()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="endpointConfigurationName"></param>
    public BlastServiceClient(string endpointConfigurationName) :
        base(endpointConfigurationName)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="endpointConfigurationName"></param>
    /// <param name="remoteAddress"></param>
    public BlastServiceClient(string endpointConfigurationName, string remoteAddress) :
        base(endpointConfigurationName, remoteAddress)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="endpointConfigurationName"></param>
    /// <param name="remoteAddress"></param>
    public BlastServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
        base(endpointConfigurationName, remoteAddress)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="binding"></param>
    /// <param name="remoteAddress"></param>
    public BlastServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
        base(binding, remoteAddress)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public System.Guid SubmitJob(Microsoft.CCF.BlastDemo.BlastSerivceRequest request)
    {
        return base.Channel.SubmitJob(request);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    public Microsoft.CCF.BlastDemo.JobStatusResponse QueryJobStatus(System.Guid jobId)
    {
        return base.Channel.QueryJobStatus(jobId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    public string GetJobResult(System.Guid jobId)
    {
        return base.Channel.GetJobResult(jobId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    public bool KillJob(System.Guid jobId)
    {
        return base.Channel.KillJob(jobId);
    }
}
