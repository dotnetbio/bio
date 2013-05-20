namespace Bio.Web.ClustalW
{
    /// <summary>
    /// This class has the list of properties that needs to be passed on to
    /// the BackGroundWorker thread to execute the ClustalW request.
    /// </summary>
    public class ClustalWThreadContext
    {
        /// <summary>
        /// ClustalW Service parameters object
        /// </summary>
        private ServiceParameters parameters;

        /// <summary>
        /// Initializes a new instance of the ClustalWThreadContext class. 
        /// </summary>
        /// <param name="parameters">ClustalW Service</param>
        public ClustalWThreadContext(
                ServiceParameters parameters)
        {
            this.parameters = parameters;
        }

        /// <summary>
        /// Gets the ClustalW Service parameters
        /// </summary>
        public ServiceParameters Parameters
        {
            get { return parameters; }
        }
    }
}
