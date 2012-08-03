namespace SequenceAssembler
{
    #region -- Using Directives --
    using System;
    using Bio.Web;
    using Bio.Web.Blast;
    #endregion
    
    /// <summary>
    /// WebServiceInput args describes the custom event Args which contains 
    /// the service parameter list selected by the user.
    /// </summary>
    public class WebServiceInputEventArgs : EventArgs
    {
        #region -- Private members --

        /// <summary>
        /// Describes the service parameters selected by the user
        /// </summary>
        private BlastParameters serviceParam;

        /// <summary>
        /// Describes the name of the web service selected.
        /// </summary>
        private string webServiceName;

        /// <summary>
        /// Configuration parameters of service.
        /// </summary>
        private ConfigParameters configuration;
        
        #endregion

        #region -- Constructor(s) --

        /// <summary>
        /// Initializes a new instance of the WebServiceInputEventArgs class
        /// </summary>
        /// <param name="parameters">the selected service parameters</param>
        /// <param name="webserviceName">Name of the webservice used.</param>
        /// <param name="configuration">Configuration of service</param>
        public WebServiceInputEventArgs(
                BlastParameters parameters,
                string webServiceName,
                ConfigParameters configuration)
        {
            this.serviceParam = parameters;
            this.webServiceName = webServiceName;
            this.configuration = configuration;
        }

        #endregion

        #region -- Public Properties --

        /// <summary>
        /// Gets the the service parameters selected by the user
        /// </summary>
        public BlastParameters ServiceParameters
        {
            get
            {
                return this.serviceParam;
            }
        }

        /// <summary>
        /// Gets the name of the web service selected.
        /// </summary>
        public string WebServiceName
        {
            get
            {
                return this.webServiceName;
            }
        }

        /// <summary>
        /// Gets the the configuration parameters selected by the user
        /// </summary>
        public ConfigParameters Configuration
        {
            get
            {
                return this.configuration;
            }
        }

        #endregion
    }
}
