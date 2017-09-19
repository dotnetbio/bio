using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Bio.Extensions;

namespace Bio.Web.Blast
{
    /// <summary>
    /// NCBI BLAST web service handler. Utilizes the REST-based web service
    /// exposed by http://www.ncbi.nlm.nih.gov/blast. 
    /// </summary>
    public class NcbiBlastWebHandler : IBlastWebHandler
    {
        /// <summary>
        /// Default endpoint
        /// </summary>
        const string DefaultEndPoint = @"http://www.ncbi.nlm.nih.gov/blast/Blast.cgi";

        /// <summary>
        /// Endpoint for the BLAST service - should be initialized to
        /// normal value, but can be replaced to hit custom services.
        /// </summary>
        public string EndPoint { get; set; }

        /// <summary>
        /// Timeout to wait in seconds for response.
        /// </summary>
        public int TimeoutInSeconds { get; set; }

        /// <summary>
        /// Delegate which can receive output as the BLAST request is processed.
        /// </summary>
        public Action<string> LogOutput { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public NcbiBlastWebHandler()
        {
            this.EndPoint = DefaultEndPoint;
            this.TimeoutInSeconds = 5 * 60;
        }

        /// <summary>
        /// Executes the BLAST search request.
        /// </summary>
        /// <param name="bp">Parameters</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>XML data as a string</returns>
        public async Task<Stream> ExecuteAsync(BlastRequestParameters bp, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(this.EndPoint))
                throw new ArgumentException("EndPoint must be set.");
            if (this.TimeoutInSeconds <= 0)
                throw new ArgumentException("Timeout must be >= 1");

            this.Log("Posting request to {0}", this.EndPoint);

            var content = this.BuildRequest(bp);
            this.Log(await content.ReadAsStringAsync());

            var client = new HttpClient();
            var result = await client.PostAsync(this.EndPoint, content, token);
            result.EnsureSuccessStatusCode();

            this.Log("Reading initial response - looking for Request Id.");
            string response = await result.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(response))
            {
                this.Log("Failed to find Request Id in: {0}", response);
                throw new HttpRequestException("No data returned.");
            }

            // Get the request ID and Estimated time to completion
            Regex ridExpr = new Regex(@"QBlastInfoBegin\s+RID = (\w+)\s+RTOE = (\w+)");
            var matches = ridExpr.Matches(response);
            if (matches.Count != 1
                || matches[0].Groups.Count != 3)
                throw new HttpRequestException("Unrecognized format returned, no Request Id located.");

            var match = matches[0];
            string rid = match.Groups[1].Value;
            string ttl = match.Groups[2].Value;

            this.Log("RequestId: {0}, Estimated Time to Completion: {1} secs.", rid, ttl);

            // Calculate our max time.
            DateTime timeoutValue = DateTime.Now.AddSeconds(this.TimeoutInSeconds);

            // Get the time to completion - we'll wait that long before
            // starting our polling.
            int seconds;
            if (Int32.TryParse(ttl, out seconds)) 
            {
                seconds = Math.Min(seconds, this.TimeoutInSeconds);
                this.Log("Waiting for {0} seconds", seconds);
                await Task.Delay(seconds * 1000, token);
            }

            Regex statusExpr = new Regex(@"QBlastInfoBegin\s+Status=(\w+)");

            // Begin our polling operation; this isn't the most efficient, but NCBI doesn't
            // provide any other mechanism.
            while (true) {
                // Check on our request.
                this.Log("Checking on request {0}", rid);
                response = await client.GetStringAsync(
                    string.Format("{0}?CMD=Get&FORMAT_OBJECT=SearchInfo&RID={1}", this.EndPoint, rid));
                var statusMatch = statusExpr.Matches(response);
                if (statusMatch.Count == 1) {
                    string state = statusMatch[0].Groups[1].Value;
                    this.Log("Processing response: {0}", response);
                    if (state == "FAILED")
                        throw new Exception("Search " + rid + " failed; please report to blast-help@ncbi.nlm.nih.gov.");
                    if (state == "UNKNOWN")
                        throw new OperationCanceledException("Search " + rid + " expired.");
                    if (state == "READY") {
                        Regex hasHitsExpr = new Regex(@"QBlastInfoBegin\s+ThereAreHits=yes");
                        if (!hasHitsExpr.IsMatch(response)) 
                        {
                            return null; // no hits
                        }
                        break;
                    }
                }
                else
                {
                    this.Log("Did not find Status in response: {0}", response);
                }

                // Go to sleep and try again.
                this.Log("Waiting 2 seconds.", response);
                await Task.Delay(2000, token);

                // Check at the end so we get at least one attempt.
                token.ThrowIfCancellationRequested();

                // Check the timeout value.
                if (DateTime.Now > timeoutValue)
                    throw new OperationCanceledException("Timeout value exceeded.");
            }

            // Retrieve the response.
            this.Log("Retrieving final response for Request Id {0}", rid);
            return await client.GetStreamAsync(
                string.Format("{0}?CMD=Get&FORMAT_TYPE=XML&RID={1}", this.EndPoint, rid));
        }

        /// <summary>
        /// Build the HTTP content for the request based on the parameters
        /// </summary>
        /// <returns>The request.</returns>
        public HttpContent BuildRequest(BlastRequestParameters blastParams)
        {
            if (string.IsNullOrWhiteSpace(blastParams.Database))
                throw new ArgumentException("Database must be supplied.");

            if (string.IsNullOrWhiteSpace(blastParams.Program))
                throw new ArgumentException("Program must be supplied.");

            if (blastParams.Sequences.Count == 0)
                throw new ArgumentException("Must have at least one sequence.");

            // Check that all sequences are same alphabet
            if (blastParams.Sequences.Count > 1)
            {
                ISequence primary = blastParams.Sequences[0];
                for (int i = 1; i < blastParams.Sequences.Count; i++)
                {
                    if (!Alphabets.CheckIsFromSameBase(primary.Alphabet, blastParams.Sequences[i].Alphabet))
                        throw new ArgumentException("Sequences must all share the same base alphabet.");
                }
            }

            var data = new List<KeyValuePair<string, string>> { this.CreateKVP("CMD", "Put") };
            if (blastParams.Program == BlastProgram.Megablast)
            {
                data.Add(this.CreateKVP("PROGRAM", BlastProgram.Blastn));
                data.Add(this.CreateKVP("MEGABLAST", "ON"));
            }
            else
                data.Add(this.CreateKVP("PROGRAM", blastParams.Program));
            data.Add(this.CreateKVP("DATABASE", blastParams.Database));

            data.AddRange(blastParams.ExtraParameters);

            // Add the sequences.
            StringBuilder sb = new StringBuilder();
            foreach (var seq in blastParams.Sequences)
                sb.Append(seq.ConvertToString());

            data.Add(this.CreateKVP("QUERY", sb.ToString()));

            return new FormUrlEncodedContent(data);
        }

        /// <summary>
        /// Creates a KeyValuePair for the request.
        /// </summary>
        /// <returns>The KV.</returns>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        KeyValuePair<string, string> CreateKVP(string key, string value)
        {
            return new KeyValuePair<string,string>(key, value);
        }

        /// <summary>
        /// Output to the diagnostic log.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        private void Log(string format, params object[] args)
        {
            var logOutput = this.LogOutput;
            if (logOutput != null)
            {
                logOutput(string.Format(format, args));
            }
        }
    }
}