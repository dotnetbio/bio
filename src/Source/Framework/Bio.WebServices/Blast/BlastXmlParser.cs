using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using Bio.Extensions;
using Bio.Util.Logging;

namespace Bio.Web.Blast
{
    /// <summary>
    /// Parse text containing XML BLAST results into a list
    /// of SequenceSearchRecord objects.
    /// </summary>
    public class BlastXmlParser : IBlastParser
    {
        private static void DoBlastOutput(string element, string value, BlastXmlMetadata metadata)
        {
            switch (element)
            {
                case "BlastOutput_program":
                    metadata.Program = value;
                    break;
                case "BlastOutput_version":
                    metadata.Version = value;
                    break;
                case "BlastOutput_reference":
                    metadata.Reference = value;
                    break;
                case "BlastOutput_db":
                    metadata.Database = value;
                    break;
                case "BlastOutput_query-ID":
                    metadata.QueryId = value;
                    break;
                case "BlastOutput_query-def":
                    metadata.QueryDefinition = value;
                    break;
                case "BlastOutput_query-len":
                    metadata.QueryLength = int.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "BlastOutput_query-seq":
                    metadata.QuerySequence = value;
                    break;
                default:
                    string message = String.Format(
                            CultureInfo.CurrentCulture,
                            Properties.Resources.UnknownElement,
                            "DoBlast",
                            element);
                    throw new FormatException(message);
            }
        }

        private static void DoParameters(string element, string value, BlastXmlMetadata metadata)
        {
            switch (element)
            {
                case "Parameters_matrix":
                    metadata.ParameterMatrix = value;
                    break;
                case "Parameters_expect":
                    metadata.ParameterExpect = double.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Parameters_include":
                    metadata.ParameterInclude = double.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Parameters_sc-match":
                    metadata.ParameterMatchScore = int.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Parameters_sc-mismatch":
                    metadata.ParameterMismatchScore = int.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Parameters_gap-open":
                    metadata.ParameterGapOpen = int.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Parameters_gap-extend":
                    metadata.ParameterGapExtend = int.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Parameters_filter":
                    metadata.ParameterFilter = value;
                    break;
                case "Parameters_pattern":
                    metadata.ParameterPattern = value;
                    break;
                case "Parameters_entrez-query":
                    metadata.ParameterEntrezQuery = value;
                    break;
                default:
                    string message = String.Format(
                            CultureInfo.CurrentCulture,
                            Properties.Resources.UnknownElement,
                            "DoParameters",
                            element);
                    throw new FormatException(message);
            }
        }

        private static void DoIteration(string element, string value, BlastSearchRecord curRecord)
        {
            switch (element)
            {
                case "Iteration_iter-num":
                    curRecord.IterationNumber = int.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Iteration_query-ID":
                    curRecord.IterationQueryId = value;
                    break;
                case "Iteration_query-def":
                    curRecord.IterationQueryDefinition = value;
                    break;
                case "Iteration_query-len":
                    curRecord.IterationQueryLength = int.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Iteration_hits":
                    // ignore
                    break;
                case "Iteration_stat":
                    // ignore
                    break;
                case "Iteration_message":
                    curRecord.IterationMessage = value;
                    break;
                default:
                    string message = String.Format(
                            CultureInfo.CurrentCulture,
                            Properties.Resources.UnknownElement,
                            "DoIteration",
                            element);
                    throw new FormatException(message);
            }
        }

        private static void DoHit(string element, string value, Hit curHit)
        {
            switch (element)
            {
                case "Hit_num":
                    // ignore
                    break;
                case "Hit_id":
                    curHit.Id = value;
                    break;
                case "Hit_def":
                    curHit.Def = value;
                    break;
                case "Hit_accession":
                    curHit.Accession = value;
                    break;
                case "Hit_len":
                    curHit.Length = long.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Hit_hsps":
                    // ignore
                    break;
                default:
                    string message = String.Format(
                            CultureInfo.CurrentCulture,
                            Properties.Resources.UnknownElement,
                            "DoHit",
                            element);
                    throw new FormatException(message);
            }
        }

        private static void DoHsp(string element, string value, Hsp hsp)
        {
            switch (element)
            {
                case "Hsp_num":
                    // ignore
                    break;
                case "Hsp_bit-score":
                    hsp.BitScore = double.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Hsp_score":
                    hsp.Score = double.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Hsp_evalue":
                    hsp.EValue = double.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Hsp_query-from":
                    hsp.QueryStart = long.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Hsp_query-to":
                    hsp.QueryEnd = long.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Hsp_hit-from":
                    hsp.HitStart = long.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Hsp_hit-to":
                    hsp.HitEnd = long.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Hsp_query-frame":
                    hsp.QueryFrame = long.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Hsp_hit-frame":
                    hsp.HitFrame = long.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Hsp_identity":
                    hsp.IdentitiesCount = long.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Hsp_positive":
                    hsp.PositivesCount = long.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Hsp_align-len":
                    hsp.AlignmentLength = long.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Hsp_density":
                    hsp.Density = int.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Hsp_qseq":
                    hsp.QuerySequence = value;
                    break;
                case "Hsp_hseq":
                    hsp.HitSequence = value;
                    break;
                case "Hsp_midline":
                    hsp.Midline = value;
                    break;
                case "Hsp_pattern-from":
                    hsp.PatternFrom = int.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Hsp_pattern-to":
                    hsp.PatternTo = int.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Hsp_gaps":
                    hsp.Gaps = int.Parse(value, CultureInfo.InvariantCulture);
                    break;
                default:
                    string message = String.Format(
                            CultureInfo.CurrentCulture,
                            Properties.Resources.UnknownElement,
                            "DoHsp",
                            element);
                    throw new FormatException(message);
            }
        }

        private static void DoStatistics(string element, string value, BlastStatistics curStats)
        {
            switch (element)
            {
                case "Statistics_db-num":
                    // ignore
                    break;
                case "Statistics_db-len":
                    curStats.DatabaseLength = long.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Statistics_hsp-len":
                    curStats.HspLength = long.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Statistics_eff-space":
                    curStats.EffectiveSearchSpace = double.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Statistics_kappa":
                    curStats.Kappa = double.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Statistics_lambda":
                    curStats.Lambda = double.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "Statistics_entropy":
                    curStats.Entropy = double.Parse(value, CultureInfo.InvariantCulture);
                    break;
                default:
                    string message = String.Format(
                            CultureInfo.CurrentCulture,
                            Properties.Resources.UnknownElement,
                            "DoStatistics",
                            element);
                    throw new FormatException(message);
            }
        }

        /// <summary>
        /// This method expects a single XML document and returns one BlastResult.
        /// </summary>
        /// <param name="doc">A Stringbuilder containing the XML document.</param>
        /// <returns>The BlastResult.</returns>
        private static BlastResult ParseXML(StringBuilder doc)
        {
            BlastResult result = new BlastResult();
            try
            {
                var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore };

                StringReader sr = null;
                try
                {
                    sr = new StringReader(doc.ToString());
                    using (XmlReader r = XmlReader.Create(sr, settings))
                    {
                        string curElement = string.Empty;
                        BlastSearchRecord curRecord = new BlastSearchRecord();
                        Hit curHit = null;
                        Hsp curHsp = null;
                        BlastStatistics curStatistics = null;
                        BlastXmlMetadata curMetadata = null;
                        while (r.Read())
                        {
                            switch (r.NodeType)
                            {
                                case XmlNodeType.Element:
                                    curElement = r.Name;
                                    switch (curElement)
                                    {
                                        case "Hit":
                                            curHit = new Hit();
                                            break;
                                        case "Hsp":
                                            curHsp = new Hsp();
                                            break;
                                        case "Statistics":
                                            curStatistics = new BlastStatistics();
                                            break;
                                        case "BlastOutput":
                                            curMetadata = new BlastXmlMetadata();
                                            break;
                                    }
                                    break;
                                case XmlNodeType.Text:
                                    if (curElement.StartsWith("BlastOutput_", StringComparison.OrdinalIgnoreCase))
                                    {
                                        DoBlastOutput(curElement, r.Value, curMetadata);
                                    }
                                    else if (curElement.StartsWith("Parameters_", StringComparison.OrdinalIgnoreCase))
                                    {
                                        DoParameters(curElement, r.Value, curMetadata);
                                    }
                                    else if (curElement.StartsWith("Iteration_", StringComparison.OrdinalIgnoreCase))
                                    {
                                        DoIteration(curElement, r.Value, curRecord);
                                    }
                                    else if (curElement.StartsWith("Statistics_", StringComparison.OrdinalIgnoreCase))
                                    {
                                        DoStatistics(curElement, r.Value, curStatistics);
                                    }
                                    else if (curElement.StartsWith("Hit_", StringComparison.OrdinalIgnoreCase))
                                    {
                                        DoHit(curElement, r.Value, curHit);
                                    }
                                    else if (curElement.StartsWith("Hsp_", StringComparison.OrdinalIgnoreCase))
                                    {
                                        DoHsp(curElement, r.Value, curHsp);
                                    }
                                    break;
                                case XmlNodeType.XmlDeclaration:
                                case XmlNodeType.ProcessingInstruction:
                                case XmlNodeType.Comment:
                                    break;
                                case XmlNodeType.EndElement:
                                    if (r.Name == "Iteration") {
                                        result.Records.Add(curRecord);
                                        curRecord = new BlastSearchRecord();
                                    }
                                    else if (r.Name == "Statistics")
                                        curRecord.Statistics = curStatistics;
                                    else if (r.Name == "Hit")
                                        curRecord.Hits.Add(curHit);
                                    else if (r.Name == "Hsp")
                                        curHit.Hsps.Add(curHsp);
                                    else if (r.Name == "BlastOutput")
                                        result.Metadata = curMetadata;
                                    break;
                            }
                        }
                    }
                }
                finally
                {
                    if (sr != null)
                        sr.Dispose();
                }
            }
            catch (Exception e)
            {
                ApplicationLog.Exception(e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// Read XML BLAST data from the reader, and build one or more
        /// BlastRecordGroup objects (each containing one or more
        /// BlastSearchRecord results).
        /// </summary>
        /// <param name="stream">The stream to pull the data from</param>
        /// <returns>A list of BLAST iteration objects</returns>
        public IEnumerable<BlastResult> Parse(Stream stream)
        {
            var records = new List<BlastResult>();
            var sb = new StringBuilder();
            long lineNumber = 1;

            using (var reader = stream.OpenRead())
            {
                string line = ReadNextLine(reader, false);

                while (!string.IsNullOrEmpty(line))
                {
                    if (line.StartsWith("RPS-BLAST", StringComparison.OrdinalIgnoreCase))
                    {
                        line = ReadNextLine(reader, false);
                        lineNumber++;
                        continue;
                    }
                    if (line.StartsWith("<?xml version", StringComparison.OrdinalIgnoreCase) &&
                        lineNumber > 1)
                    {
                        records.Add(ParseXML(sb));
                        sb = new StringBuilder();
                    }
                    sb.AppendLine(line);
                    line = ReadNextLine(reader, false);
                    lineNumber++;
                }

                if (sb.Length > 0)
                    records.Add(ParseXML(sb));

                if (records.Count == 0)
                    throw new FormatException(Properties.Resources.BlastNoRecords);
            }
            return records;
        }

        /// <summary>
        /// Parse a single entity from the given stream.
        /// </summary>
        /// <param name="stream">The stream to pull data from</param>
        /// <returns>Parse entity object</returns>
        public BlastResult ParseOne(Stream stream)
        {
            return Parse(stream).First();
        }

        /// <summary>
        /// Reads next line considering
        /// </summary>
        /// <returns></returns>
        private static string ReadNextLine(TextReader reader, bool skipBlankLines)
        {
            if (reader.Peek() == -1)
            {
                return null;
            }

            string line = reader.ReadLine();
            while (skipBlankLines && string.IsNullOrWhiteSpace(line) && reader.Peek() != -1)
            {
                line = reader.ReadLine();
            }

            return line;
        }

        /// <summary>
        /// Gets the name of the parser being implemented. 
        /// This is intended to give the developer name of the parser.
        /// </summary>
        public string Name { get { return Properties.Resources.BlastXmlParserName; } }

        /// <summary>
        /// Gets the description of the parser being
        /// implemented. This is intended to give the
        /// developer some information of the parser.
        /// </summary>
        public string Description { get { return Properties.Resources.BlastXmlParserDescription; } }

        /// <summary>
        /// Gets the file extensions that the parser supports.
        /// If multiple extensions are supported then this property 
        /// will return a string containing all extensions with a ',' delimited.
        /// </summary>
        public string SupportedFileTypes { get { return string.Empty; } }
    }
}
