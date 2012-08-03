using System;

namespace Bio.Web.ClustalW
{
    /// <summary>
    /// Event arguments used to notify the user when the job is completed.
    /// </summary>
    public class ClustalWCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Is the search successful
        /// </summary>
        private bool isSearchSuccessful;

        /// <summary>
        /// Result of blast search
        /// </summary>
        private ClustalWResult searchResult;

        /// <summary>
        /// Error message on failure
        /// </summary>
        private string errorMessage;

        /// <summary>
        /// Exception occured
        /// </summary>
        private Exception error;

        /// <summary>
        /// ClustalW Service parameters object
        /// </summary>
        private ServiceParameters parameters;

        /// <summary>
        /// Is this request cancelled.
        /// </summary>
        private bool canceled;

        /// <summary>
        /// Initializes a new instance of the ClustalWCompletedEventArgs class
        /// </summary>
        /// <param name="parameters">Service parameter</param>
        /// <param name="isSearchSuccessful">Is search successful</param>
        /// <param name="searchResult">Search result records</param>
        /// <param name="error">Exception if any</param>
        /// <param name="errorMessage">Error message if any</param>
        /// <param name="canceled">Was request cancelled</param>
        public ClustalWCompletedEventArgs(
                ServiceParameters parameters,
                bool isSearchSuccessful,
                ClustalWResult searchResult,
                Exception error,
                string errorMessage,
                bool canceled)
        {
            this.parameters = parameters;
            this.isSearchSuccessful = isSearchSuccessful;
            this.searchResult = searchResult;
            this.error = error;
            this.errorMessage = errorMessage;
            this.canceled = canceled;
        }

        /// <summary>
        /// Gets a value indicating whether the search  is successful
        /// </summary>
        public bool IsSearchSuccessful
        {
            get { return isSearchSuccessful; }
        }

        /// <summary>
        /// Gets result of blast search
        /// </summary>
        public ClustalWResult SearchResult
        {
            get { return searchResult; }
        }

        /// <summary>
        /// Gets the error message on failure
        /// </summary>
        public string ErrorMessage
        {
            get { return errorMessage; }
        }

        /// <summary>
        /// Gets the Exception occured
        /// </summary>
        public Exception Error
        {
            get { return error; }
        }

        /// <summary>
        /// Gets a value indicating whether the search  is cancelled.
        /// </summary>
        public bool Canceled
        {
            get { return canceled; }
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
