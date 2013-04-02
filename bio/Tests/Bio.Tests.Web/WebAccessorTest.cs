using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Bio.Util;
using Bio.Web;
using Bio.Web.Blast;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests.Web
{
    /// <summary>
    ///This is a test class for WebAccessorTest and is intended
    ///to contain all WebAccessorTest Unit Tests
    ///</summary>
    /// Disabled the test case as the web service is down at the time of release
     [TestClass]
    public class WebAccessorTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        private static string requestIdentifier = string.Empty;
        private static ServiceRequestInformation status = null;

        /// <summary>
        ///A test for BeginAsyncRequest
        ///</summary>
        /// Disabled the test case as the web service is down at the time of release
        /// Suppressed the message as this test case will be enabled in future
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [TestMethod]
        [Ignore]
        public void BeginAsyncRequestTest()
        {
            status = new ServiceRequestInformation();
            BlastParameters searchParams = new BlastParameters();
            // fill in the BLAST settings:
            searchParams.Add("Command", "Put");
            searchParams.Add("Program", "blastn");
            searchParams.Add("Expect", "1e-10");
            searchParams.Add("CompositionBasedStatistics", "0");

            string badDbName = "ThisDatabaseDoesNotExist";
            searchParams.Add("Database", badDbName);

            // test parameters
            string sequence = @"GACGCCGCCGCCACCACCGCCACCGCCGCAGCAGAAGCAGCGCACCGCAGGAGGGAAG" +
                "ATGCCGGCGGGGCACGGGCTGCGGGCGCGGACGGCGACCTCTTCGCGCGGCCGTTCCGCAAGAAGGGTTA" +
                "CATCCCGCTCACCACCTACCTGAGGACGTACAAGATCGGCGATTACGTNGACGTCAAGGTGAACGGTG";
            searchParams.Add("Query", sequence);

            AutoResetEvent requestSubmitWait = new AutoResetEvent(false);
            AsyncWebMethodRequest input = new AsyncWebMethodRequest(
                new Uri("http://www.ncbi.nlm.nih.gov/blast/Blast.cgi"),
                CredentialCache.DefaultCredentials,
                searchParams.Settings,
                BuildQueryString(searchParams.Settings),
                SubmitRequestCompleted,
                requestSubmitWait);

            WebAccessor target = new WebAccessor();
            target.GetBrowserProxy();
            target.BeginAsyncRequest(input);
            WaitHandle.WaitAny(new WaitHandle[] {requestSubmitWait});

            Assert.IsFalse(string.IsNullOrEmpty(requestIdentifier));

            // query the status

            Dictionary<string, string> settings = new Dictionary<string, string>();
            settings.Add("CMD", "GET");
            settings.Add("RID", HttpUtility.UrlEncode(requestIdentifier));

            AutoResetEvent requestStatusWait = new AutoResetEvent(false);
            input = new AsyncWebMethodRequest(
                new Uri("http://www.ncbi.nlm.nih.gov/blast/Blast.cgi"),
                CredentialCache.DefaultCredentials,
                settings,
                BuildQueryString(settings),
                RequestStatusCompleted,
                requestStatusWait);

            target.BeginAsyncRequest(input);
            WaitHandle.WaitAny(new WaitHandle[] { requestStatusWait });

            bool ok = false;
            if (status.Status != ServiceRequestStatus.Waiting && status.Status != ServiceRequestStatus.Ready)
            {
                if (status.StatusInformation.Contains(badDbName) &&
                    status.StatusInformation.Contains("No alias or index file found for nucleotide database"))
                {
                    ok = true;
                }
            }
            if (!ok)
            {
                Assert.Fail("Failed to find server error message for bad request. Info: " + status.StatusInformation);
            }
        }

         /// <summary>
         /// Test the Blast parser.
         /// </summary>
        [TestMethod]
        public void ParseBlastValidation()
        {
            var blastFilePath = @"TestUtils\BlastRecords.xml";
            var expectedGapCost = "11";
            var expectedParameterMatrix = "BLOSUM62";
            var expectedResultCount = "9";
            var expectedDatabselength = "2974038";
            var expectedHitsCount = "1";
            var expectedAccession = "NP_010143";
            var expectedAlignmentLength = "142";
            var expectedBitScore = "33.113";
            var expectedHitSequence = "KKIRKRCLAFPISQQIEMKTNIKNAGKLDNNNSPIVQDDS-----HNIIDVDGFLENQIQSNDHMIAADKGSGSLVNIIDTAPLTSTEFKPVEHPPVNY---------------VDLGNGLGLNTYNRGPPSIISDSTDEFY";

            // Parse a Blast xml file.
            BlastXmlParser parser = new BlastXmlParser();
            IList<BlastResult> blastResults = parser.Parse(blastFilePath);

            // Validate Meta data 
            BlastXmlMetadata meta = blastResults[0].Metadata;

            Assert.AreEqual(meta.ParameterGapOpen.ToString((IFormatProvider)null),
                expectedGapCost);
            Assert.AreEqual(meta.ParameterMatrix.ToString((IFormatProvider)null),
                expectedParameterMatrix);

            // Validate blast records.
            BlastSearchRecord record = blastResults[4].Records[0];

            Assert.AreEqual(expectedResultCount, blastResults.Count.ToString((IFormatProvider)null));
            Assert.AreEqual(expectedDatabselength,
                record.Statistics.DatabaseLength.ToString((IFormatProvider)null));
            Assert.AreEqual(expectedHitsCount, record.Hits.Count.ToString((IFormatProvider)null));
            Assert.AreEqual(expectedAccession, record.Hits[0].Accession.ToString((IFormatProvider)null));
            Assert.AreEqual(expectedHitsCount, record.Hits[0].Hsps.Count.ToString((IFormatProvider)null));

            // Validate bit score.
            Hsp highScoreSgment = record.Hits[0].Hsps[0];
            Assert.AreEqual(expectedAlignmentLength, highScoreSgment.AlignmentLength.ToString(
                (IFormatProvider)null));
            Assert.AreEqual(expectedBitScore, highScoreSgment.BitScore.ToString((
                IFormatProvider)null));
            Assert.AreEqual(expectedHitSequence, highScoreSgment.HitSequence.ToString(
                (IFormatProvider)null));
        }

        /// <summary>
        /// Handle the Submit request completed response.
        /// </summary>
        /// <param name="response">Response output</param>
        private static void SubmitRequestCompleted(AsyncWebMethodResponse response)
        {
            if (response.Status == AsyncMethodState.Failed)
            {
                // failed
                response.Error = new WebException(String.Format(CultureInfo.InvariantCulture,
                        "SubmitHttpRequest failed. Status: {0}.",
                        response.StatusDescription));
            }

            string responseString = string.Empty;
            using (StreamReader reader = new StreamReader(response.Result))
            {
                responseString = reader.ReadToEnd();
            }
            response.Result.Close();

            string info = ExtractInfoSection(responseString);
            if (!String.IsNullOrEmpty(info))
            {
                int ridStart = info.IndexOf("RID = ", StringComparison.OrdinalIgnoreCase);
                if (ridStart >= 0)
                {
                    ridStart += "RID = ".Length;
                    int ridEnd = info.IndexOf('\n', ridStart);
                    if (ridEnd >= 0)
                    {
                        requestIdentifier = info.Substring(ridStart, ridEnd - ridStart);
                    }
                }
            }

            if (string.IsNullOrEmpty(requestIdentifier))
            {
                response.Error = new ArgumentNullException(String.Format(CultureInfo.InvariantCulture,
                        "Failed to extract a requestIdentifier. Error: {0}.",
                        ExtractError(responseString)));
            }

            AutoResetEvent requestSubmitWait = response.State as AutoResetEvent;
            requestSubmitWait.Set();
        }

        /// <summary>
        /// Handle the Submit request completed response.
        /// </summary>
        /// <param name="response">Response output</param>
        private static void RequestStatusCompleted(AsyncWebMethodResponse response)
        {
            AutoResetEvent requestStatusWait = response.State as AutoResetEvent;
            if (response.Status == AsyncMethodState.Failed)
            {
                // failure
                response.Result.Close();
                status.Status = ServiceRequestStatus.Error;
                status.StatusInformation = response.StatusDescription;
                requestStatusWait.Set();
                return;
            }

            response.StatusDescription = string.Empty;

            string responseString = string.Empty;
            using (StreamReader reader = new StreamReader(response.Result))
            {
                responseString = reader.ReadToEnd();
            }
            response.Result.Close();

            string information = ExtractInfoSection(responseString);
            if (String.IsNullOrEmpty(information))
            {
                status.Status = ServiceRequestStatus.Error;
                // see if we got an error message
                string errorInformation = ExtractBlastErrorSection(responseString);
                if (string.IsNullOrEmpty(errorInformation))
                {
                    status.StatusInformation = "An unknown server error has occurred.";
                }
                else
                {
                    status.StatusInformation = errorInformation;
                }

                requestStatusWait.Set();
                return;
            }
            else
            {
                int statusStart = information.IndexOf("Status=", StringComparison.OrdinalIgnoreCase);
                if (statusStart >= 0)
                {
                    statusStart += "Status=".Length;
                    int statusEnd = information.IndexOf('\n', statusStart);
                    if (statusEnd >= 0)
                    {
                        response.StatusDescription = information.Substring(statusStart, statusEnd - statusStart);
                    }
                }
            }

            if (response.StatusDescription == "WAITING")
            {
                status.Status = ServiceRequestStatus.Waiting;
                requestStatusWait.Set();
                return;
            }
            else if (response.StatusDescription == "READY")
            {
                status.Status = ServiceRequestStatus.Ready;
                requestStatusWait.Set();
                return;
            }

            status.Status = ServiceRequestStatus.Error;
            status.StatusInformation = response.StatusDescription;
            requestStatusWait.Set();
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
        /// Find the QBlastInfoBegin section where the request ID is stored
        /// </summary>
        /// <param name="response">Web response string</param>
        /// <returns>Information section string</returns>
        private static string ExtractInfoSection(string response)
        {
            const string startTag = "QBlastInfoBegin";
            const string endTag = "QBlastInfoEnd";

            int startInfo = response.IndexOf(startTag, StringComparison.OrdinalIgnoreCase);
            if (startInfo >= 0)
            {
                startInfo += startTag.Length;
                int endInfo = response.IndexOf(endTag, startInfo, StringComparison.OrdinalIgnoreCase);
                if (endInfo >= 0)
                {
                    return response.Substring(startInfo, endInfo - startInfo);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Error message is contained in list element with id msgR
        /// Here is the html structure
        /// <ul id="msgR" class="msg">
        ///     <li class="error">
        ///         <div class="error msInf">
        ///             {Error message}
        ///         </div>
        ///     </li>
        /// </ul>
        /// </summary>
        /// <param name="response">Response string</param>
        /// <returns>Error message</returns>
        private static string ExtractError(string response)
        {
            const string errorSectionStartTag = "<ul id=\"msgR\"";
            const string errorSectionEndTag = "</ul>";
            const string paraStartTag = "<p";
            const string endTag = ">";
            const string startTag = "<";
            string errorMessage = string.Empty;

            int startIndex = response.IndexOf(errorSectionStartTag, StringComparison.OrdinalIgnoreCase);
            if (0 <= startIndex)
            {
                int endIndex = 0;
                endIndex = response.IndexOf(errorSectionEndTag, startIndex, StringComparison.OrdinalIgnoreCase);
                string errorSection = response.Substring(
                        startIndex,
                        endIndex - startIndex);
                // find the index of div tag
                startIndex = errorSection.IndexOf(paraStartTag, StringComparison.OrdinalIgnoreCase);
                if (0 <= startIndex)
                {
                    // move to the end of div starttag
                    startIndex = errorSection.IndexOf(endTag, startIndex, StringComparison.OrdinalIgnoreCase);
                    if (0 <= startIndex)
                    {
                        startIndex++;
                        // End at the start of next index.
                        endIndex = errorSection.IndexOf(startTag, startIndex, StringComparison.OrdinalIgnoreCase);
                        if (0 <= endIndex)
                        {
                            // Error message has irregular spacing. Reform the 
                            // error message with regular spacing.
                            errorMessage = errorSection.Substring(
                                    startIndex,
                                    endIndex - startIndex).Trim();
                            errorMessage = string.Join(" ", errorMessage.Split(
                                    new char[] { ' ' },
                                    StringSplitOptions.RemoveEmptyEntries));
                        }
                    }
                }
            }

            return errorMessage;
        }

        /// <summary>
        /// Look for a blast error message in the response. Try to be robust
        /// with respect to possible changes in formatting, etc.
        /// </summary>
        /// <param name="response">Web response string</param>
        /// <returns>Error section string</returns>
        private static string ExtractBlastErrorSection(string response)
        {
            const string preTag = "p id=\"blastErr\"";
            const string startTag = "Informational Message: ";
            const string endTag = ") persists";
            const string altStartTag = "Error:";
            const string altEndTag = "<";

            int startInfo = response.IndexOf(preTag, StringComparison.OrdinalIgnoreCase);
            if (startInfo >= 0)
            {
                startInfo += preTag.Length;
                int startMessage = response.IndexOf(startTag, startInfo, StringComparison.OrdinalIgnoreCase);
                if (startMessage >= 0)
                {
                    startMessage += startTag.Length;
                    int endMessage = response.IndexOf(endTag, startMessage, StringComparison.OrdinalIgnoreCase);
                    if (endMessage >= 0)
                    {
                        return response.Substring(startMessage, endMessage - startMessage);
                    }
                }
            }
            else
            {
                // look for other variant
                startInfo = response.IndexOf(altStartTag, StringComparison.OrdinalIgnoreCase);
                {
                    if (startInfo >= 0)
                    {
                        int endInfo = response.IndexOf(altEndTag, StringComparison.OrdinalIgnoreCase);
                        if (endInfo >= 0)
                        {
                            return response.Substring(startInfo, endInfo - startInfo);
                        }
                    }
                }
            }

            return string.Empty;
        }
    }
}
