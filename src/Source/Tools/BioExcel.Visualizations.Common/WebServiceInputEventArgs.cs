using System;

using Bio.Web.Blast;

namespace BiodexExcel.Visualizations.Common
{
    /// <summary>
    /// WebServiceInput args describes the custom event Args which contains 
    /// the service parameter list selected by the user.
    /// </summary>
    public class WebServiceInputEventArgs : EventArgs
    {
        /// <summary>
        /// Web service name
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// BLAST parameters
        /// </summary>
        public BlastRequestParameters Parameters { get; set; }

        /// <summary>
        /// Initializes a new instance of the WebServiceInputEventArgs class
        /// </summary>
        /// <param name="serviceName">Web service name</param>
        /// <param name="parameters">the selected service parameters</param>
        public WebServiceInputEventArgs(string serviceName, BlastRequestParameters parameters)
        {
            this.ServiceName = serviceName;
            this.Parameters = parameters;
        }
    }
}
