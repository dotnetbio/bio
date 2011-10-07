using System;

namespace Bio.Web
{
    /// <summary>
    /// Client configuration parameters for accessing a web service.
    /// </summary>
    public class ConfigParameters
    {
        /// <summary>
        /// URI for the web interface
        /// </summary>
        public Uri Connection { get; set; }

        /// <summary>
        /// Useragent string for authentication to the web-interface 
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Default timeout value
        /// </summary>
        public int DefaultTimeout { get; set; }

        /// <summary>
        /// User's Email address for connecting to the web-interface 
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Use Sync / Async calls
        /// </summary>
        public bool UseAsyncMode { get; set; }

        /// <summary>
        /// Use HTTP/HTTPS 
        /// </summary>
        public bool UseHttps { get; set; }

        /// <summary>
        /// Use default browser proxy settings for web access
        /// </summary>
        public bool UseBrowserProxy { get; set; }

        /// <summary>
        /// Number of times to requery a service when request is pending
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// Number of seconds between requeries when waiting for a service request
        /// </summary>
        public int RetryInterval { get; set; }

        /// <summary>
        /// Password string for authentication to the web service
        /// </summary>
        public string Password { get; set; }
    }
}
