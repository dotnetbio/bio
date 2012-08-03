using System;
using System.Collections.Generic;

namespace Bio.Web.Blast
{
    /// <summary>
    /// Event arguments used to notify the user when the job is completed.
    /// </summary>
    public class BlastRequestCompletedEventArgs : RequestCompletedEventArgs
    {
        /// <summary>
        /// Result of blast search
        /// </summary>
        private IList<BlastResult> searchResult;

        /// <summary>
        /// Job identifier
        /// </summary>
        private string requestIdentifier;

        /// <summary>
        /// Initializes a new instance of the RequestCompletedEventArgs class
        /// </summary>
        /// <param name="requestIdentifier">Job identifier</param>
        /// <param name="isSearchSuccessful">Is search successful</param>
        /// <param name="searchResult">Search result records</param>
        /// <param name="error">Exception if any</param>
        /// <param name="errorMessage">Error message if any</param>
        /// <param name="isCanceled">Was request cancelled</param>
        public BlastRequestCompletedEventArgs(
                string requestIdentifier,
                bool isSearchSuccessful,
                IList<BlastResult> searchResult,
                Exception error,
                string errorMessage,
                bool isCanceled)
            : base(isSearchSuccessful, error, errorMessage, isCanceled)
        {
            this.requestIdentifier = requestIdentifier;
            this.searchResult = searchResult;
        }

        /// <summary>
        /// Gets result of blast search
        /// </summary>
        public IList<BlastResult> SearchResult
        {
            get { return searchResult; }
        }

        /// <summary>
        /// Gets job identifier
        /// </summary>
        public string RequestIdentifier
        {
            get { return requestIdentifier; }
        }
    }
}
