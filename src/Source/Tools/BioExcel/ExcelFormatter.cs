namespace BiodexExcel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Bio;
    using Bio.IO.GenBank;
    using Bio.Util;
    using System.Globalization;

    /// <summary>
    /// Utilities to format sequence / metadata in excel required format
    /// </summary>
    public class ExcelImportFormatter
    {
        /// <summary>
        /// Key representing Features metadata
        /// </summary>
        private const string FEATURES = "features";

        /// <summary>
        /// GFF - key for metadata dictionary - source
        /// </summary>
        private const string GFF_SOURCE = "source";

        /// <summary>
        /// GFF - key for metadata dictionary - start
        /// </summary>
        private const string GFF_START = "start";

        /// <summary>
        /// GFF - key for metadata dictionary - end
        /// </summary>
        private const string GFF_END = "end";

        /// <summary>
        /// GFF - key for metadata dictionary - score
        /// </summary>
        private const string GFF_SCORE = "score";

        /// <summary>
        /// GFF - key for metadata dictionary - strand
        /// </summary>
        private const string GFF_STRAND = "strand";

        /// <summary>
        /// GFF - key for metadata dictionary - frame
        /// </summary>
        private const string GFF_FRAME = "frame";

        /// <summary>
        /// Convert fasta sequence id to a string array
        /// </summary>
        public static string[,] SequenceIDHeaderToRange(ISequence sequence)
        {
            var formattedData = new string[1,2];
            formattedData[0, 0] = Properties.Resources.Sequence_ID;
            formattedData[0, 1] = (sequence != null) ? sequence.ID : "";
            return formattedData;
        }

        /// <summary>
        /// Convert quality values to string array
        /// </summary>
        /// <param name="sequence">Sequence which has the quality values</param>
        /// <param name="maxColumns">Max number of columns to write to</param>
        /// <returns>string array with quality values</returns>
        public static string[,] FastQQualityValuesToRange(QualitativeSequence sequence, int maxColumns)
        {
            var qualityScoreArray = sequence.GetEncodedQualityScores();
            long rowCount = (int)Math.Ceiling((decimal)qualityScoreArray.Length / (decimal)maxColumns);
            long columnCount = sequence.Count > maxColumns ? maxColumns : sequence.Count;
            string[,] rangeData = new string[rowCount, columnCount];

            int count = 0;

            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < columnCount && count < qualityScoreArray.Length; col++, count++)
                {
                    rangeData[row, col] = (qualityScoreArray[count]).ToString(CultureInfo.InvariantCulture);
                }
            }

            return rangeData;
        }

        /// <summary>
        /// Gives out string array of metadata and features just below metadata.
        /// </summary>
        /// <param name="metadata">GenBank Metadata</param>
        /// <returns>string array of metadata</returns>
        public static string[,] GenBankMetadataToRange(GenBankMetadata metadata)
        {
            List<string[]> excelData = new List<string[]>();
            List<string> excelRow = new List<string>();

            // Add the metadata headers
            excelRow.Add(Properties.Resources.GenbankMetadataHeader);
            excelData.Add(excelRow.ToArray());
            excelRow.Clear();

            if (metadata.Locus != null)
            {
                excelData.Add(new[] { Properties.Resources.GenbankMetadataLocus });
                AddNameValuePair(excelData, 1, Properties.Resources.GenbankMetadataName, metadata.Locus.Name);
                AddNameValuePair(excelData, 1, Properties.Resources.GenbankMetadataSeqLength, metadata.Locus.SequenceLength.ToString());
                AddNameValuePair(excelData, 1, Properties.Resources.GenbankMetadataSeqType, metadata.Locus.SequenceType);
                AddNameValuePair(excelData, 1, Properties.Resources.GenbankMetadataStrandType, Helper.GetStrandType(metadata.Locus.Strand));
                AddNameValuePair(excelData, 1, Properties.Resources.GenbankMetadataMoleculeType, metadata.Locus.MoleculeType.ToString());
                AddNameValuePair(excelData, 1, Properties.Resources.GenbankMetadataStrandTopology, Helper.GetStrandTopology(metadata.Locus.StrandTopology));
                AddNameValuePair(excelData, 1, Properties.Resources.GenbankMetadataDivisionCode, metadata.Locus.DivisionCode.ToString());
                AddNameValuePair(excelData, 1, Properties.Resources.GenbankMetadataDate, metadata.Locus.Date.ToString("dd-MMM-yyyy").ToUpper());
            }

            if (!string.IsNullOrWhiteSpace(metadata.Definition))
            {
                AddNameValuePair(excelData, 0, Properties.Resources.GenbankMetadataDefinition, "", metadata.Definition);
            }

            if (metadata.Accession != null)
            {
                string secondaryAccession = string.Empty;
                foreach (string accession2 in metadata.Accession.Secondary)
                {
                    secondaryAccession += accession2 == null ? " " : " " + accession2;
                }
                AddNameValuePair(excelData, 0, Properties.Resources.GenbankMetadataAccession, "", metadata.Accession.Primary + secondaryAccession);
            }

            if (metadata.DbLinks != null)
            {
                foreach (var link in metadata.DbLinks)
                {
                    string linkNumbers = string.Empty;
                    foreach (string linkNumber in link.Numbers)
                    {
                        linkNumbers += linkNumber + ",";
                    }
                    AddNameValuePair(excelData, 0, Properties.Resources.GenbankMetadataDBLink, "", link.Type.ToString() + ":" + linkNumbers);
                }
            }

            if (!string.IsNullOrWhiteSpace(metadata.DbSource))
            {
                AddNameValuePair(excelData, 0, Properties.Resources.GenbankMetadataDBSource, "", metadata.DbSource);
            }

            if (metadata.Version != null)
            {
                AddNameValuePair(excelData, 0, Properties.Resources.GenbankMetadataVersion, "", (metadata.Version.Accession ?? string.Empty) + "." + 
                                    (metadata.Version.Version ?? string.Empty) + " " + Properties.Resources.GenbankMetadataGI + (metadata.Version.GiNumber ?? string.Empty));
            }

            if (metadata.Segment != null)
            {
                AddNameValuePair(excelData, 0, Properties.Resources.GenbankMetadataSegment, "", metadata.Segment.Current + " of " + metadata.Segment.Count);
            }

            AddNameValuePair(excelData, 0, Properties.Resources.GenbankMetadataKeywords, "", metadata.Keywords);

            if (metadata.Source != null)
            {
                AddNameValuePair(excelData, 0, Properties.Resources.GenbankMetadataSource, "", metadata.Source.CommonName ?? string.Empty);
                AddNameValuePair(excelData, 1, Properties.Resources.GenbankMetadataOrganism, (metadata.Source.Organism.Genus ?? string.Empty) + " " + (metadata.Source.Organism.Species ?? string.Empty));
                AddNameValuePair(excelData, 1, Properties.Resources.GenbankMetadataClassLevels, metadata.Source.Organism.ClassLevels ?? string.Empty);
            }

            foreach (CitationReference reference in metadata.References)
            {
                AddNameValuePair(excelData, 0, Properties.Resources.GenbankMetadataReference, "", reference.Number.ToString() + " (" + reference.Location + ")");
                AddNameValuePair(excelData, 1, Properties.Resources.GenbankMetadataAuthors, reference.Authors);
                AddNameValuePair(excelData, 1, Properties.Resources.GenbankMetadataTitle, reference.Title);
                AddNameValuePair(excelData, 1, Properties.Resources.GenbankMetadataJournal, reference.Journal);
                AddNameValuePair(excelData, 1, Properties.Resources.GenbankMetadataConsortiums, reference.Consortiums);
                AddNameValuePair(excelData, 1, Properties.Resources.GenbankMetadataMedLine, reference.Medline);
                AddNameValuePair(excelData, 1, Properties.Resources.GenbankMetadataPubMed, reference.PubMed);
                AddNameValuePair(excelData, 1, Properties.Resources.GenbankMetadataRemarks, reference.Remarks);
            }

            if (!string.IsNullOrWhiteSpace(metadata.Primary))
            {
                AddNameValuePair(excelData, 0, Properties.Resources.GenbankMetadataPrimary, "", metadata.Primary);
            }

            if (metadata.Comments != null && metadata.Comments.Count > 0)
            {
                StringBuilder strbuilder = null;

                foreach (string str in metadata.Comments)
                {
                    if (strbuilder == null)
                    {
                        strbuilder = new StringBuilder();
                    }
                    else
                    {
                        strbuilder.Append(Environment.NewLine);
                    }

                    strbuilder.Append(str);
                }

                if (strbuilder != null)
                {
                    AddNameValuePair(excelData, 0, Properties.Resources.GenbankMetadataComment, "", strbuilder.ToString());
                }
            }

            if (metadata.Features != null)
            {
                // Add the metadata headers
                excelRow.Add(Properties.Resources.GenbankFeaturesHeader);
                excelData.Add(excelRow.ToArray());
                excelRow.Clear();

                IList<FeatureItem> featureList = metadata.Features.All;
                foreach (FeatureItem featureItem in featureList)
                {
                    LocationBuilder locBuilder = new LocationBuilder();
                    // Add the feature headers
                    excelRow.Add(featureItem.Key);
                    //excelRow.Add(""); // skip one column
                    excelRow.Add(locBuilder.GetLocationString(featureItem.Location));
                    excelData.Add(excelRow.ToArray());
                    excelRow.Clear();

                    foreach (string key in featureItem.Qualifiers.Keys)
                    {
                        foreach (string value in featureItem.Qualifiers[key])
                        {
                            AddNameValuePair(excelData, 1, key, value);
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(metadata.BaseCount))
            {
                AddNameValuePair(excelData, 0, Properties.Resources.GenbankMetadataBaseCount, "", metadata.BaseCount);
            }

            return ConvertToArray(excelData);
        }

        /// <summary>
        /// Gives out string array of metadata and features just below metadata.
        /// </summary>
        /// <param name="sequence">Sequence object</param>
        /// <returns>string array of metadata</returns>
        public static string[,] GffMetaDataToRange(ISequence sequence)
        {
            if (sequence.Metadata.Keys.Contains(FEATURES))
            {
                var listItems = sequence.Metadata[FEATURES] as List<MetadataListItem<List<string>>>;
                if (listItems != null)
                {
                    var excelData = new List<string[]>();
                    var excelRow = new List<string>();

                    // Add metadata
                    foreach (MetadataListItem<List<string>> listItem in listItems)
                    {
                        excelRow.Add(GetGFFColumnValue(listItem, GFF_SOURCE));
                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat("{0} = {1}\n", Properties.Resources.GffColumnType, listItem.Key ?? string.Empty);
                        sb.AppendFormat("{0} = {1}\n", Properties.Resources.GffColumnStart, GetGFFColumnValue(listItem, GFF_START));
                        sb.AppendFormat("{0} = {1}\n", Properties.Resources.GffColumnEnd, GetGFFColumnValue(listItem, GFF_END));
                        sb.AppendFormat("{0} = {1}\n", Properties.Resources.GffColumnScore, GetGFFColumnValue(listItem, GFF_SCORE));
                        sb.AppendFormat("{0} = {1}\n", Properties.Resources.GffColumnStrand, GetGFFColumnValue(listItem, GFF_STRAND));
                        sb.AppendFormat("{0} = {1}\n", Properties.Resources.GffColumnFrame, GetGFFColumnValue(listItem, GFF_FRAME));
                        sb.AppendFormat("{0} = {1}", Properties.Resources.GffColumnGroup, listItem.FreeText ?? string.Empty);
                        excelRow.Add(sb.ToString());

                        excelData.Add(excelRow.ToArray());
                        excelRow.Clear();
                    }

                    return ConvertToArray(excelData);
                }
            }

            return null;
        }

        /// <summary>
        /// Convert a List of string array to a 2-D array.
        /// </summary>
        /// <param name="excelData">List to be converted</param>
        /// <returns>2-D array representation of the given list.</returns>
        private static string[,] ConvertToArray(List<string[]> excelData)
        {
            // Get the length of the longest string in the list
            int colCount = excelData.Max(r => r.Length);
            string[,] finalMetadata = new string[excelData.Count, colCount];

            for (int curRow = 0; curRow < excelData.Count; curRow++)
            {
                string[] row = excelData[curRow];
                for (int curCol = 0; curCol < row.Length; curCol++)
                {
                    finalMetadata[curRow, curCol] = row[curCol];
                }
            }

            return finalMetadata;
        }

        /// <summary>
        /// Get the value of a particular key from GFF metadata structure
        /// </summary>
        /// <param name="listItem">GFF Metadata</param>
        /// <param name="itemKey">Header of the column in GFF metadata</param>
        /// <returns>Value of the given column</returns>
        private static string GetGFFColumnValue(MetadataListItem<List<string>> listItem, string itemKey)
        {
            List<string> values = new List<string>(1);
            if (listItem.SubItems.TryGetValue(itemKey, out values))
            {
                if (values.Count > 0)
                {
                    return values[0];
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Add a row of data to a List of string[]
        /// </summary>
        /// <param name="excelData">list of string representing excel data</param>
        /// <param name="indent">Number of columns to indent on the left</param>
        /// <param name="name">name of metadata</param>
        /// <param name="value">value of metadata</param>
        /// <param name="values">Additional values</param>
        private static void AddNameValuePair(
                List<string[]> excelData,
                int indent,
                string name,
                string value,
                params string[] values)
        {
            // Never perform an indent of the values - so the Key is always
            // in column 1. This breaks the tree relationship visually but was requested
            // in order to make the layout correct.
            indent = 0;

            if (!string.IsNullOrEmpty(value) || values.Length > 0)
            {
                if (string.IsNullOrEmpty(value) && values.Length == 1 && string.IsNullOrWhiteSpace(values[0]))
                {
                    // basic value is blank and values list is having one item which is null or blank then return
                    return;
                }

                List<string> excelRow = new List<string>();

                // Add blank cells till requested indent
                while (indent-- > 0)
                {
                    excelRow.Add(string.Empty);
                }

                excelRow.Add(name);
                if (!string.IsNullOrEmpty(value))
                    excelRow.Add(value);
                excelRow.AddRange(values);

                excelData.Add(excelRow.ToArray());
            }
        }
    }
}
