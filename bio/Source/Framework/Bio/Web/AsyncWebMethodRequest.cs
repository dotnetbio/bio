using System;
using System.Collections.Generic;
using System.Net;

namespace Bio.Web
{
    /// <summary>
    /// This class contains the inputs that are required to instantiate and invoke web method.
    /// </summary>
    public class AsyncWebMethodRequest
    {
        /// <summary>
        /// Uri of web reqeust.
        /// </summary>
        private Uri url = null;

        /// <summary>
        /// Credential to be used for web request.
        /// </summary>
        private ICredentials credential = null;

        /// <summary>
        /// Parameters to be passed in web request header.
        /// </summary>
        private Dictionary<string, string> parameter = null;

        /// <summary>
        /// Post data string for web request.
        /// </summary>
        private string postData = string.Empty;

        /// <summary>
        /// Function pointer to be invoked after the completion of web request.
        /// </summary>
        private AsyncWebMethodCompleted callback = null;

        /// <summary>
        /// State to be web request.
        /// </summary>
        private object state = null;

        /// <summary>
        /// Constructor: Initialize the instance of type WebMethodInput
        /// </summary>
        /// <param name="url">Uri of web reqeust.</param>
        /// <param name="credential">Credential to be used for web request.</param>
        /// <param name="parameters">Request parameters.</param>
        /// <param name="postData">Post data string for web request.</param>
        /// <param name="callback">Function pointer to be invoked after the completion of web request.</param>
        /// <param name="state">State of the Async web method</param>
        public AsyncWebMethodRequest(Uri url, 
            ICredentials credential, 
            Dictionary<string, string> parameters, 
            string postData, 
            AsyncWebMethodCompleted callback,
            object state)
        {
            this.url = url;
            this.credential = credential;
            this.parameter = parameters;
            this.postData = postData;
            this.callback = callback;
            this.state = state;
        }

        /// <summary>
        /// Gets the Uri of web reqeust.
        /// </summary>
        public Uri Url
        {
            get { return url; }
        }

        /// <summary>
        /// Gets the credential to be used for web request.
        /// </summary>
        public ICredentials Credential
        {
            get { return credential; }
        }

        /// <summary>
        /// Gets parameters to be passed in web request header.
        /// </summary>
        public Dictionary<string, string> Parameter
        {
            get { return parameter; }
        }

        /// <summary>
        /// Gets the post data string for web request.
        /// </summary>
        public string PostData
        {
            get { return postData; }
        }

        /// <summary>
        /// Gets the function pointer to be invoked after the completion of web request.
        /// </summary>
        public AsyncWebMethodCompleted Callback
        {
            get { return callback; }
        }

        /// <summary>
        /// Gets or sets the webrequest instance.
        /// </summary>
        public WebRequest Request { get; set; }

        /// <summary>
        /// Gets the state of Web request
        /// </summary>
        public object State 
        {
            get { return state; }
        }
    }
}
