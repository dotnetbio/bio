namespace Bio.Web
{
    /// <summary>
    /// A simple summary of the status of a remote service request.
    /// </summary>
    public enum ServiceRequestStatus
    {
        /// <summary>
        /// The request is queued at the server.
        /// </summary>
        Queued,
        /// <summary>
        /// The request is being processed by the server.
        /// </summary>
        Waiting,
        /// <summary>
        /// The request has been processed and results are available.
        /// </summary>
        Ready,
        /// <summary>
        /// An error has occurred while processing the request.
        /// </summary>
        Error,
        /// <summary>
        /// The request has been cancelled at server.
        /// </summary>
        Canceled,
    }
}
