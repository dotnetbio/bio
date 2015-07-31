using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Bio.Web.Blast
{
    /// <summary>
    /// BLAST web service handler.
    /// </summary>
    public interface IBlastWebHandler
    {
        /// <summary>
        /// Endpoint for the BLAST service - should be initialized to
        /// normal value, but can be replaced to hit custom services.
        /// </summary>
        string EndPoint { get; set; }

        /// <summary>
        /// Timeout to wait in seconds for response.
        /// </summary>
        int TimeoutInSeconds { get; set; }

        /// <summary>
        /// Delegate which can receive output as the BLAST request is processed.
        /// </summary>
        Action<string> LogOutput { get; set; }

        /// <summary>
        /// Executes the BLAST search request.
        /// </summary>
        /// <param name="bp">Parameters</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>XML data as a string</returns>
        Task<Stream> ExecuteAsync(BlastRequestParameters bp, CancellationToken token);
    }
}