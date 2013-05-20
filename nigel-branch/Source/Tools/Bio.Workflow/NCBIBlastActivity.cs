using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Workflow.ComponentModel;
using Bio.IO.FastA;
using Bio.Web;
using Bio.Web.Blast;
using Microsoft.Research.ScientificWorkflow;

namespace Bio.Workflow
{
    /// <summary>
    /// Blasts a FastA file using the NCBI QBlast service and returns the BLAST hit-table
    /// </summary>
    [Name("NCBI Blast")]
    [Description("Blasts a FastA file using the NCBI QBlast service and returns the BLAST hit-table")]
    [WorkflowCategory("Bioinformatics")]
    public class NCBIBlastActivity : Activity
    {
        #region Dependency Properties

        /// <summary>
        /// The path to the FASTA formatted file to be read in and BLAST.
        /// </summary>
        public static DependencyProperty InputFileProperty =
             DependencyProperty.Register("InputFile", typeof(string), typeof(NCBIBlastActivity));

        /// <summary>
        /// The path to the FASTA formatted file to be read in and BLAST.
        /// </summary>
        [RequiredInputParam]
        [Name("Input File")]
        [Description(@"The path to the FASTA formatted file to be read in and BLAST.")]
        public string InputFile
        {
            get { return ((string)(base.GetValue(NCBIBlastActivity.InputFileProperty))); }
            set { base.SetValue(NCBIBlastActivity.InputFileProperty, value); }
        }

        /// <summary>
        /// Serialized NCBI Blast Result Output.
        /// </summary>
        public static DependencyProperty BlastResultProperty =
    DependencyProperty.Register("BlastResult", typeof(string), typeof(NCBIBlastActivity));

        /// <summary>
        /// Serialized NCBI Blast Result Output.
        /// </summary>
        [OutputParam]
        [Name("Blast Result")]
        [Description(@"Serialized NCBI Blast Result Output.")]
        public string BlastResult
        {
            get { return ((string)(base.GetValue(NCBIBlastActivity.BlastResultProperty))); }
            set { base.SetValue(NCBIBlastActivity.BlastResultProperty, value); }
        }

        #endregion
        /// <summary>
        /// The execution method for the activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <returns>The execution status.</returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            FastAParser fastaParser = new FastAParser();
            fastaParser.Open(InputFile);
            ISequence searchSequence = fastaParser.Parse().FirstOrDefault();

            NCBIBlastHandler service = new NCBIBlastHandler();

            ConfigParameters configParams = new ConfigParameters();
            configParams.UseBrowserProxy = true;
            service.Configuration = configParams;

            BlastParameters searchParams = new BlastParameters();
            // fill in the BLAST settings:
            searchParams.Add("Program", "blastn");
            searchParams.Add("Database", "nr");
            // higher Expect will return more results
            searchParams.Add("Expect", "1e-10");
            searchParams.Add("CompositionBasedStatistics", "0");

            // create the request
            string jobID = service.SubmitRequest(searchSequence, searchParams);

            // query the status
            ServiceRequestInformation info = service.GetRequestStatus(jobID);
            if (info.Status != ServiceRequestStatus.Waiting
                && info.Status != ServiceRequestStatus.Ready)
            {
                // TODO: Add error handling here
            }

            // get async results, poll until ready
            int maxAttempts = 10;
            int attempt = 1;

            while (attempt <= maxAttempts
                    && info.Status != ServiceRequestStatus.Error
                    && info.Status != ServiceRequestStatus.Ready)
            {
                ++attempt;
                info = service.GetRequestStatus(jobID);
                Thread.Sleep(
                    info.Status == ServiceRequestStatus.Waiting || info.Status == ServiceRequestStatus.Queued
                    ? 20000 * attempt : 0);
            }

            // Get blast result.
            BlastXmlParser blastParser = new BlastXmlParser();
            IList<BlastResult> results = blastParser.Parse(new StringReader(service.GetResult(jobID, searchParams)));

            // Convert blast result to BlastCollator.
            List<BlastResultCollator> blastResultCollator = new List<BlastResultCollator>();
            foreach (BlastResult result in results)
            {
                foreach (BlastSearchRecord record in result.Records)
                {
                    if (null != record.Hits
                            && 0 < record.Hits.Count)
                    {
                        foreach (Hit hit in record.Hits)
                        {
                            if (null != hit.Hsps
                                    && 0 < hit.Hsps.Count)
                            {
                                foreach (Hsp hsp in hit.Hsps)
                                {
                                    BlastResultCollator blast = new BlastResultCollator();
                                    blast.Alignment = hsp.AlignmentLength;
                                    blast.Bit = hsp.BitScore;
                                    blast.EValue = hsp.EValue;
                                    blast.Identity = hsp.IdentitiesCount;
                                    blast.Length = hit.Length;
                                    blast.QEnd = hsp.QueryEnd;
                                    blast.QStart = hsp.QueryStart;
                                    blast.QueryId = record.IterationQueryId;
                                    blast.SEnd = hsp.HitEnd;
                                    blast.SStart = hsp.HitStart;
                                    blast.SubjectId = hit.Id;
                                    blast.Positives = hsp.PositivesCount;
                                    blast.QueryString = hsp.QuerySequence;
                                    blast.SubjectString = hsp.HitSequence;
                                    blast.Accession = hit.Accession;
                                    blast.Description = hit.Def;
                                    blastResultCollator.Add(blast);
                                }
                            }
                        }
                    }
                }
            }

            BlastXmlSerializer serializer = new BlastXmlSerializer();
            Stream stream = serializer.SerializeBlastOutput(blastResultCollator);

            // set result to the output property.
            BlastResult = GetSerializedData(stream);

            return ActivityExecutionStatus.Closed;
        }

        /// <summary>
        /// Gets the Xml Serialized data from the given stream.
        /// </summary>
        /// <param name="stream">memory stream</param>
        /// <returns>serialized blast string</returns>
        private static string GetSerializedData(Stream stream)
        {
            string xml = string.Empty;
            MemoryStream memStream = stream as MemoryStream;
            if (memStream != null)
            {
                xml = System.Text.Encoding.UTF8.GetString(memStream.GetBuffer());
                xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
                xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
                memStream.Close();
            }

            return xml;
        }
    }
}
