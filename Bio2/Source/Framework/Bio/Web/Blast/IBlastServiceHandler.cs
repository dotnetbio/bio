using System;
using System.Collections.Generic;

namespace Bio.Web.Blast
{
    /// <summary>
    /// This interface will serve to define an element set which will be common 
    /// to the underlying web-service and transport protocol related information.
    /// </summary>
    public interface IBlastServiceHandler : IServiceHandler
    {
        /// <summary>
        /// This event is raised when Blast search is complete. It could be either a success or failure.
        /// </summary>
        event EventHandler<BlastRequestCompletedEventArgs> RequestCompleted;

        /// <summary>
        /// Return the status of a submitted job.
        /// </summary>
        /// <param name="requestIdentifier">Identifier for the request of interest.</param>
        /// <returns>The status of the request.</returns>
        ServiceRequestInformation GetRequestStatus(string requestIdentifier);

        /// <summary>
        /// Submit the search request with the user supplied configuration parameters and sequence
        /// Implementation should make use of the Bio.IO formatters to convert the sequence into 
        /// the web interface compliant sequence format
        /// </summary>
        /// <remarks>An exception is thrown if the request does not succeed.</remarks>
        /// <param name="sequence">The sequence to search with</param>
        /// <param name="parameters">Blast input parameters</param>
        /// <returns>Request Identifier</returns>
        string SubmitRequest(ISequence sequence, BlastParameters parameters);

        /// <summary>
        /// Submit the search request with the user supplied configuration parameters and sequence
        /// Implementation should make use of the Bio.IO formatters to convert the sequence into 
        /// the web interface compliant sequence format
        /// </summary>
        /// <remarks>An exception is thrown if the request does not succeed.</remarks>
        /// <param name="sequences">The sequence to search with</param>
        /// <param name="parameters">Blast input parameters</param>
        /// <returns>Request Identifier</returns>
        string SubmitRequest(IList<ISequence> sequences, BlastParameters parameters);

        /// <summary>
        /// Gets the search results for the pertinent request identifier.
        /// Implementation should have dedicated parsers to format the received results into Bio
        /// </summary>
        /// <remarks>An exception is thrown if the request does not succeed.</remarks>
        /// <param name="requestIdentifier">Identifier for the request of interest.</param>
        /// <param name="parameters">Blast input parameters</param>
        /// <returns>The search results</returns>
        string GetResult(string requestIdentifier, BlastParameters parameters);

        /// <summary>
        /// Fetch the search results synchronously for the pertinent request identifier.
        /// This is a synchronous method and will not return until the results are 
        /// available.
        /// Implementation should have dedicated parsers to format the received results into
        /// Bio
        /// </summary>
        /// <remarks>
        /// An exception is thrown if the request does not succeed.
        /// </remarks>
        /// <param name="requestIdentifier">Identifier for the request of interest</param>
        /// <param name="parameters">Blast input parameters</param>
        /// <returns>The search results</returns>
        IList<BlastResult> FetchResultsSync(string requestIdentifier, BlastParameters parameters);

        /// <summary>
        /// Cancels the submitted job.
        /// </summary>
        /// <param name="requestIdentifier">Identifier for the request of interest.</param>
        /// <returns>Is the job cancelled.</returns>
        bool CancelRequest(string requestIdentifier);
    }
}
