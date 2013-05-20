using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using Bio.Util;

namespace Bio.Web
{
    /// <summary>
    /// A WebAccessor manages the process of downloading information from a URL.
    /// </summary>
    public class WebAccessor
    {
        #region Member variables

        private HttpWebResponse _webResponse;

        private const string PostMethod = "POST";
        private const string GetMethod = "GET";
        private const string ContentType = "application/x-www-form-urlencoded";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets WebProxy object that will be used for HTTP requests.
        /// </summary>
        public IWebProxy Proxy { get; set; }

        #endregion

        #region Constructors
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public WebAccessor()
        {
            Proxy = null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get and store the default browser proxy in effect
        /// </summary>
        public void GetBrowserProxy()
        {
            Proxy = WebRequest.DefaultWebProxy;
        }

        /// <summary>
        /// Restore the default proxy
        /// </summary>
        public void GetDefaultProxy()
        {
            Proxy = new WebProxy();
        }

        /// <summary>
        /// Submit a parameterized HTTP request by either GET or POST. The 
        /// caller can ask for the response either as a string or as a stream.
        /// </summary>
        /// <remarks>
        /// If getResponse = false, the responseStream can be used by the caller
        /// to read the response. The caller must call Close() when done with the stream.
        /// If getResponse = true, the stream will be null, and Close() should not be called.
        /// </remarks>
        /// <param name="url">The URL to request</param>
        /// <param name="doPost">POST if true, GET if false.</param>
        /// <param name="requestParameters">A set of parameter/value pairs, in unencoded form.</param>
        /// <returns>Response from Web.</returns>
        public WebAccessorResponse SubmitHttpRequest(
            Uri url,
            bool doPost,
            Dictionary<string, string> requestParameters)
        {
            WebAccessorResponse webAccessorResponse = new WebAccessorResponse();
            try
            {
                WebRequest request = null;
                string queryString = BuildQueryString(requestParameters);

                if (doPost)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] postBytes = encoding.GetBytes(queryString);
                    request = CreatePostRequest(url, CredentialCache.DefaultCredentials, postBytes.Length);
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(postBytes, 0, postBytes.Length);
                    requestStream.Close();
                }
                else
                {
                    request = CreateGetRequest(url, queryString, CredentialCache.DefaultCredentials);
                }

                Close();    // get rid of any old response
                _webResponse = (HttpWebResponse)request.GetResponse();
                webAccessorResponse.StatusDescription = _webResponse.StatusDescription;
                if (webAccessorResponse.StatusDescription == "OK")
                {
                    Stream s = _webResponse.GetResponseStream();

                    using (StreamReader reader = new StreamReader(s))
                    {
                        webAccessorResponse.ResponseString = reader.ReadToEnd();
                    }

                    Close();
                    webAccessorResponse.IsSuccessful = true;
                    return webAccessorResponse;
                }

                webAccessorResponse.IsSuccessful = false;
                return webAccessorResponse;
            }
            catch (WebException we)
            {
                HttpWebResponse response = (HttpWebResponse)we.Response;
                if (response == null)
                {
                    webAccessorResponse.StatusDescription = string.Format(CultureInfo.InvariantCulture,
                            "{0}{1}",
                            webAccessorResponse.StatusDescription,
                            we.Message);
                    if (we.InnerException != null)
                    {
                        webAccessorResponse.StatusDescription = string.Format(CultureInfo.InvariantCulture,
                                "{0}\n{1}",
                                webAccessorResponse.StatusDescription,
                                we.InnerException.Message);
                    }
                }
                else
                {
                    webAccessorResponse.StatusDescription = string.Format(CultureInfo.InvariantCulture,
                            "{0}WebException: {1}",
                            webAccessorResponse.StatusDescription,
                            response.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                webAccessorResponse.StatusDescription = ex.Message;
                if (ex.InnerException != null)
                {
                    webAccessorResponse.StatusDescription = string.Format(CultureInfo.InvariantCulture,
                            "{0}\n{1}",
                            webAccessorResponse.StatusDescription,
                            ex.InnerException.Message);
                }
            }

            webAccessorResponse.IsSuccessful = false;
            return webAccessorResponse;
        }

        /// <summary>
        /// Close the internal HttpWebResponse, after reading from the stream returned by
        /// SubmitHttpRequest with getResponse = false.
        /// </summary>
        public void Close()
        {
            if (_webResponse != null)
            {
                _webResponse.Close();
                _webResponse = null;
            }
        }

        /// <summary>
        /// This method instantiates and invokes asynchronous web call.
        /// Create the web request object and do a Async call
        /// 1.	If post data is required register EndAsyncRequest as callback.
        /// 2.	Otherwise register EndAsyncResponse as callback method.
        /// </summary>
        /// <param name="input">Input parameters</param>
        public void BeginAsyncRequest(AsyncWebMethodRequest input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            AsyncWebMethodResponse asyncResponse = new AsyncWebMethodResponse(input.State);
            try
            {
                WebRequest request = null;
                if (string.IsNullOrEmpty(input.PostData))
                {
                    string queryString = BuildQueryString(input.Parameter);
                    request = CreateGetRequest(input.Url, queryString, input.Credential);

                    Close();    // get rid of any old response
                    input.Request = request;
                    request.BeginGetResponse(EndAsyncResponse, input);
                }
                else
                {
                    request = CreatePostRequest(input.Url, input.Credential, input.PostData.Length);
                    input.Request = request;
                    request.BeginGetRequestStream(EndAsyncRequest, input);
                }
            }
            catch (WebException we)
            {
                HttpWebResponse response = (HttpWebResponse)we.Response;
                if (response == null)
                {
                    asyncResponse.StatusDescription = string.Format(CultureInfo.InvariantCulture,
                            "{0}{1}",
                            asyncResponse.StatusDescription,
                            we.Message);
                    if (we.InnerException != null)
                    {
                        asyncResponse.StatusDescription = string.Format(CultureInfo.InvariantCulture,
                                "{0}\n{1}",
                                asyncResponse.StatusDescription,
                                we.InnerException.Message);
                    }
                }
                else
                {
                    asyncResponse.StatusDescription = string.Format(CultureInfo.InvariantCulture,
                            "{0}WebException: {1}",
                            asyncResponse.StatusDescription,
                            response.StatusDescription);
                }

                asyncResponse.Status = AsyncMethodState.Failed;
                asyncResponse.Error = we;

                input.Callback(asyncResponse);
            }
            catch (Exception ex)
            {
                asyncResponse.StatusDescription = ex.Message;
                if (ex.InnerException != null)
                {
                    asyncResponse.StatusDescription = string.Format(CultureInfo.InvariantCulture,
                            "{0}\n{1}",
                            asyncResponse.StatusDescription,
                            ex.InnerException.Message);
                }

                asyncResponse.Status = AsyncMethodState.Failed;
                asyncResponse.Error = ex;

                input.Callback(asyncResponse);
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Do asynchronous web post.
        /// Register EndAsyncResponse and callback method.
        /// </summary>
        /// <param name="state">Async Web method state</param>
        private void EndAsyncRequest(IAsyncResult state)
        {
            AsyncWebMethodRequest input = state.AsyncState as AsyncWebMethodRequest;
            try
            {
                WebRequest request = input.Request;
                Stream requestStream = request.EndGetRequestStream(state);

                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] postBytes = encoding.GetBytes(input.PostData);
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();

                request.BeginGetResponse(EndAsyncResponse, input);
            }
            catch (Exception ex)
            {
                AsyncWebMethodResponse asyncResponse = new AsyncWebMethodResponse(input.State);
                asyncResponse.StatusDescription = ex.Message;
                if (ex.InnerException != null)
                {
                    asyncResponse.StatusDescription = string.Format(CultureInfo.InvariantCulture,
                            "{0}\n{1}",
                            asyncResponse.StatusDescription,
                            ex.InnerException.Message);
                }

                asyncResponse.Status = AsyncMethodState.Failed;
                asyncResponse.Error = ex;

                input.Callback(asyncResponse);
            }
        }

        /// <summary>
        /// Read asynchronous web response.
        /// Invoke the callback method to report completion.
        /// </summary>
        /// <param name="state">Async Web method state</param>
        private void EndAsyncResponse(IAsyncResult state)
        {
            AsyncWebMethodRequest input = state.AsyncState as AsyncWebMethodRequest;
            AsyncWebMethodResponse asyncResponse = new AsyncWebMethodResponse(input.State);
            try
            {
                WebRequest request = input.Request;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(state);
                asyncResponse.StatusDescription = response.StatusDescription;
                if (asyncResponse.StatusDescription == "OK")
                {
                    asyncResponse.Result = response.GetResponseStream();
                    asyncResponse.Status = AsyncMethodState.Passed;
                }
                else
                {
                    asyncResponse.Status = AsyncMethodState.Passed;
                }
            }
            catch (WebException we)
            {
                HttpWebResponse response = (HttpWebResponse)we.Response;
                if (response == null)
                {
                    asyncResponse.StatusDescription = string.Format(CultureInfo.InvariantCulture,
                            "{0}{1}",
                            asyncResponse.StatusDescription,
                            we.Message);
                    if (we.InnerException != null)
                    {
                        asyncResponse.StatusDescription = string.Format(CultureInfo.InvariantCulture,
                                "{0}\n{1}",
                                asyncResponse.StatusDescription,
                                we.InnerException.Message);
                    }
                }
                else
                {
                    asyncResponse.StatusDescription = string.Format(CultureInfo.InvariantCulture,
                            "{0}WebException: {1}",
                            asyncResponse.StatusDescription,
                            response.StatusDescription);
                }

                asyncResponse.Status = AsyncMethodState.Failed;
                asyncResponse.Error = we;
            }
            catch (Exception ex)
            {
                asyncResponse.StatusDescription = ex.Message;
                if (ex.InnerException != null)
                {
                    asyncResponse.StatusDescription = string.Format(CultureInfo.InvariantCulture,
                            "{0}\n{1}",
                            asyncResponse.StatusDescription,
                            ex.InnerException.Message);
                }

                asyncResponse.Status = AsyncMethodState.Failed;
                asyncResponse.Error = ex;
            }

            input.Callback(asyncResponse);
        }

        /// <summary>
        /// Build the query string using the request parameters
        /// </summary>
        /// <param name="requestParameters">Request parameters</param>
        /// <returns>Query string.</returns>
        private static string BuildQueryString(Dictionary<string, string> requestParameters)
        {
            StringBuilder paramBlock = new StringBuilder();
            string separator = string.Empty;
            foreach (KeyValuePair<string, string> kvp in requestParameters)
            {
                paramBlock.Append(separator);
                separator = "&";
                paramBlock.Append(HttpUtility.UrlEncode(kvp.Key));
                if (!string.IsNullOrEmpty(kvp.Value))
                {
                    paramBlock.Append("=");
                    paramBlock.Append(HttpUtility.UrlEncode(kvp.Value));
                }
            }

            return paramBlock.ToString();
        }

        /// <summary>
        /// Create the Post request object
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="credentials">Authentication credentials</param>
        /// <param name="postDataLength">Post data length</param>
        /// <returns>WebRequest object</returns>
        private WebRequest CreatePostRequest(Uri url, ICredentials credentials, int postDataLength)
        {
            WebRequest request = WebRequest.Create(url);
            request.Proxy = Proxy;
            request.Credentials = credentials;
            request.Method = PostMethod;
            request.ContentType = ContentType;
            request.ContentLength = postDataLength;
            request.Timeout = 30*1000; // 30 second timeout
            return request;
        }

        /// <summary>
        /// Create the Get request object
        /// </summary>
        /// <param name="url">Request Url</param>
        /// <param name="queryString">Query string</param>
        /// <param name="credentials">Authentication credentials</param>
        /// <returns>WebRequest object</returns>
        private WebRequest CreateGetRequest(Uri url, string queryString, ICredentials credentials)
        {
            url = new Uri(string.Format(CultureInfo.InvariantCulture,
                    "{0}?{1}",
                    url.ToString(),
                    queryString));

            WebRequest request = WebRequest.Create(url);
            request.Proxy = Proxy;
            request.Credentials = credentials;
            request.Method = GetMethod;
            request.Timeout = 30 * 1000; // 30 second timeout
            return request;
        }
        #endregion
    }
}
