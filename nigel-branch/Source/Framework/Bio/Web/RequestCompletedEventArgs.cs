using System;

namespace Bio.Web
{
    /// <summary>
    /// Event arguments used to notify the user when the job is completed.
    /// </summary>
    public class RequestCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Is the search successful
        /// </summary>
        private bool isSearchSuccessful;

        /// <summary>
        /// Error message on failure
        /// </summary>
        private string errorMessage;

        /// <summary>
        /// Exception occured
        /// </summary>
        private Exception error;

        /// <summary>
        /// Is this request cancelled.
        /// </summary>
        private bool isCanceled;

        /// <summary>
        /// Initializes a new instance of the RequestCompletedEventArgs class
        /// </summary>
        /// <param name="isSearchSuccessful">Is search successful</param>
        /// <param name="error">Exception if any</param>
        /// <param name="errorMessage">Error message if any</param>
        /// <param name="isCanceled">Was request cancelled</param>
        public RequestCompletedEventArgs(
                bool isSearchSuccessful,
                Exception error,
                string errorMessage,
                bool isCanceled)
        {
            this.isSearchSuccessful = isSearchSuccessful;
            this.error = error;
            this.errorMessage = errorMessage;
            this.isCanceled = isCanceled;
        }

        /// <summary>
        /// Gets a value indicating whether the search  is successful
        /// </summary>
        public bool IsSearchSuccessful
        {
            get { return isSearchSuccessful; }
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
        public bool IsCanceled
        {
            get { return isCanceled; }
        }
    }
}
