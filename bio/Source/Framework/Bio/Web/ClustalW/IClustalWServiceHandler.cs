using System;
using System.Collections.Generic;

namespace Bio.Web.ClustalW
{
    /// <summary>
    /// This interface will serve to define an element set which will be common 
    /// to the underlying web-service and transport protocol related information.
    /// </summary>
    public interface IClustalWServiceHandler : IServiceHandler
    {
        /// <summary>
        /// This event is raised when Blast search is complete. It could be either a success or failure.
        /// </summary>
        event EventHandler<ClustalWCompletedEventArgs> RequestCompleted;

        /// <summary>
        /// Submit the search request with the user supplied configuration parameters and sequence.
        /// Implementation should make use of the Bio.IO formatters to convert the sequence into 
        /// the web interface compliant sequence format
        /// </summary>
        /// <remarks>An exception is thrown if the request does not succeed.</remarks>
        /// <param name="sequence">The sequence to search with</param>
        /// <param name="parameters">Blast input parameters</param>
        /// <returns>Service parameters</returns>
        ServiceParameters SubmitRequest(IList<ISequence> sequence, ClustalWParameters parameters);

        /// <summary>
        /// Return the status of a submitted job.
        /// </summary>
        /// <param name="parameters">Service parameters.</param>
        /// <returns>The status of the request.</returns>
        ServiceRequestInformation GetRequestStatus(ServiceParameters parameters);

        /// <summary>
        /// Fetch the search results asynchronously for the pertinent request identifier.
        /// Implementation should have dedicated parsers to format the received results into
        /// Bio
        /// </summary>
        /// <remarks>
        /// An exception is thrown if the request does not succeed.
        /// </remarks>
        /// <param name="serviceParameters">Service Parameters</param>
        /// <returns>The ClustalW results</returns>
        ClustalWResult FetchResultsAsync(ServiceParameters serviceParameters);

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
        /// <param name="serviceParameters">Service Parameters</param>
        /// <returns>The ClustalW results</returns>
        ClustalWResult FetchResultsSync(ServiceParameters serviceParameters);

        /// <summary>
        /// Cancels the submitted job.
        /// </summary>
        /// <param name="serviceParameters">Service Parameters</param>
        /// <returns>Is the job cancelled.</returns>
        bool CancelRequest(ServiceParameters serviceParameters);
    }
}
