namespace BiodexExcel
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Bio;
    using Bio.IO;
    using Bio.IO.GenBank;
    using BiodexExcel.Properties;
    using Microsoft.Office.Interop.Excel;

    /// <summary>
    /// Utilities to parse sequence / metadata from excel range
    /// </summary>
    public class ExcelSelectionParser
    {
        #region constants
        /// <summary>
        /// Gap nucleotide
        /// </summary>
        private static readonly char Gap = '-';

        #region Genbank Related Fields

        /// <summary>
        /// Index of key column for parsing genbank metadata.
        /// </summary>
        private const int KeyColumnIndex = 1;

        /// <summary>
        /// Index of sub key column for parsing genbank metadata.
        /// </summary>
        private const int SubKeyColumnIndex = 2;

        /// <summary>
        /// Index of value column for parsing genbank metadata.
        /// </summary>
        private const int ValueColumnIndex = 3;

        /// <summary>
        /// Key representing Features metadata
        /// </summary>
        private const string FEATURES = "FEATURES";

        /// <summary>
        /// Key representing Metadata
        /// </summary>
        private const string METADATA = "METADATA";

        /// <summary>
        /// Key representing Locus data
        /// </summary>
        private const string LOCUS = "LOCUS";

        /// <summary>
        /// Sub key representing LOCUS name.
        /// </summary>
        private const string LOCUS_NAME = "NAME";

        /// <summary>
        /// Sub key representing Seq length in LOCUS.
        /// </summary>
        private const string LOCUS_SEQLEN = "SEQ LENGTH";

        /// <summary>
        /// Sub key representing Seq type in LOCUS.
        /// </summary>
        private const string LOCUS_SEQTYPE = "SEQ TYPE";

        /// <summary>
        /// Sub key representing molecule type in LOCUS.
        /// </summary>
        private const string LOCUS_MOLTYPE = "MOLECULE TYPE";

        /// <summary>
        /// Sub key representing strand type in LOCUS.
        /// </summary>
        private const string LOCUS_STRANDTYPE = "STRAND TYPE";

        /// <summary>
        /// Sub key representing strand topology in LOCUS.
        /// </summary>
        private const string LOCUS_STRANTTOPOLOGY = "STRAND TOPOLOGY";

        /// <summary>
        /// Sub key representing division code in LOCUS.
        /// </summary>
        private const string LOCUS_DIVISIONCODE = "DIVISION CODE";

        /// <summary>
        /// Sub key representing date in LOCUS.
        /// </summary>
        private const string LOCUS_DATE = "DATE";

        /// <summary>
        /// Key representing DEFINITION.
        /// </summary>
        private const string DEFINITION = "DEFINITION";

        /// <summary>
        /// Sub key representing DBLINK.
        /// </summary>
        private const string DBLINK = "DBLINK";
        /// <summary>
        /// Key representing ACCESSION.
        /// </summary>
        private const string ACCESSION = "ACCESSION";

        /// <summary>
        /// Sub key representing DBSource.
        /// </summary>
        private const string DBSOURCE = "DBSOURCE";

        /// <summary>
        /// Key representing Version
        /// </summary>
        private const string VERSION = "VERSION";

        /// <summary>
        /// Key representing SEGMENT
        /// </summary>
        private const string SEGMENT = "SEGMENT";

        /// <summary>
        /// Key representing Keywords.
        /// </summary>
        private const string KEYWORDS = "KEYWORDS";

        /// <summary>
        /// Key representing SOURCE.
        /// </summary>
        private const string SOURCE = "SOURCE";

        /// <summary>
        /// Sub key representing ORGANISM.
        /// </summary>
        private const string SOURCE_ORGANISM = "ORGANISM";

        /// <summary>
        /// Sub key representing CLASSLEVELS.
        /// </summary>
        private const string SOURCE_CLASSLEVELS = "CLASS LEVELS";

        /// <summary>
        /// Key representing CLASSLEVELS.
        /// </summary>
        private const string REFERENCE = "REFERENCE";

        /// <summary>
        /// Sub key representing Authors.
        /// </summary>
        private const string REFERENCE_AUTHORS = "AUTHORS";

        /// <summary>
        /// Sub key representing TITLE.
        /// </summary>
        private const string REFERENCE_TITLE = "TITLE";

        /// <summary>
        /// Sub key representing JOURNAL.
        /// </summary>
        private const string REFERENCE_JOURNAL = "JOURNAL";

        /// <summary>
        /// Sub key representing CONSORTIUMS.
        /// </summary>
        private const string REFERENCE_CONSORTIUMS = "CONSORTIUMS";

        /// <summary>
        /// Sub key representing MEDLINE.
        /// </summary>
        private const string REFERENCE_MEDLINE = "MEDLINE";

        /// <summary>
        /// Sub key representing PUBMED.
        /// </summary>
        private const string REFERENCE_PUBMED = "PUBMED";

        /// <summary>
        /// Sub key representing REMARK.
        /// </summary>
        private const string REFERENCE_REMARK = "REMARK";

        /// <summary>
        /// Key representing PRIMARY.
        /// </summary>
        private const string PRIMARY = "PRIMARY";

        /// <summary>
        /// Key representing BASECOUNT.
        /// </summary>
        private const string BASECOUNT = "BASE COUNT";

        /// <summary>
        /// Key representing COMMENT.
        /// </summary>
        private const string COMMENT = "COMMENT";
        #endregion

        /// <summary>
        /// Chromosome name or ID
        /// </summary>
        private const string CHROM_ID = "Chromosome";

        /// <summary>
        /// Starting range
        /// </summary>
        private const string CHROM_START = "Start";

        /// <summary>
        /// Ending range
        /// </summary>
        private const string CHROM_END = "Stop";

        #endregion

        /// <summary>
        /// Convert given range containing any sequence (other than BED) to Bio.Sequence object
        /// </summary>
        /// <param name="userSelectedRanges">Range of cells</param>
        /// <param name="treatBlankCellsAsGaps">Flag to treat blank cells as gaps</param>
        /// <param name="alphabetType">Molecule Type</param>
        /// <param name="sequenceId">Sequence Identifier</param>
        /// <returns>Sequence object</returns>
        public static ISequence RangeToSequence(IList<Range> userSelectedRanges, bool treatBlankCellsAsGaps, string alphabetType, string sequenceId)
        {
            StringBuilder builder = new StringBuilder();
            string cellValue = string.Empty;

            foreach (Range range in userSelectedRanges)
            {
                foreach (Range currentCell in range.Cells)
                {
                    // if cell has value, and its row and column is not hidden
                    if (currentCell != null && currentCell.Value2 != null && !(currentCell.EntireColumn.Hidden || currentCell.EntireRow.Hidden))
                    {
                        cellValue = currentCell.Value2.ToString();
                        if (string.IsNullOrWhiteSpace(cellValue))
                        {
                            if (treatBlankCellsAsGaps)
                            {
                                builder.Append(Gap);
                            }
                        }
                        else
                        {
                            builder.Append(cellValue);
                        }
                    }
                    else if (treatBlankCellsAsGaps && !(currentCell.EntireColumn.Hidden || currentCell.EntireRow.Hidden))
                    {
                        builder.Append(Gap);
                    }
                }
            }

            string sequenceString = builder.ToString();

            IAlphabet alphabet = null;
            foreach (IAlphabet alphabetSet in Alphabets.All)
            {
                if (0 == string.Compare(alphabetSet.Name, alphabetType, true))
                {
                    alphabet = alphabetSet;
                    break;
                }
            }

            if (null == alphabet)
            {
                byte[] sequenceBytes = sequenceString.Select(a => (byte)a).ToArray();
                alphabet = Alphabets.AutoDetectAlphabet(sequenceBytes, 0, sequenceBytes.Length, null);
            }

            if (alphabet == null)
            {
                throw new Exception(Properties.Resources.CannotFindMatchingAlphabet);
            }

            Sequence sequence = new Sequence(alphabet, sequenceString);
            sequence.ID = sequenceId;
            return sequence;
        }

        /// <summary>
        /// Convert given range containing any bed sequence to SequenceRange object
        /// </summary>
        /// <param name="userSelectedRange">Range of cells</param>
        /// <param name="columnMapping">A dictionary which has a column number to BED header mapping.</param>
        /// <returns>SequenceRange, Address object</returns>
        public static Dictionary<ISequenceRange, string> RangeToSequenceRange(Range userSelectedRange, Dictionary<int, string> columnMapping)
        {
            Dictionary<ISequenceRange, string> sequenceRanges = new Dictionary<ISequenceRange, string>();
            SequenceRange sequenceRange;
            string rangeAddress, cellValue, columnName;
            object[,] cellRange = userSelectedRange.Value2 as object[,]; // cellrange will be a base 1 array
            int rangeWidth = columnMapping.Keys.Max() - columnMapping.Keys.Min() + 1;
            SortedSet<int> sortedColumn = new SortedSet<int>(columnMapping.Keys);

            bool idFound, startFound, endFound;

            for (int row = 1; row <= cellRange.GetLength(0); row++)
            {
                // If hidden row, then skip
                if ((userSelectedRange.Rows[row] as Range).EntireRow.Hidden)
                {
                    continue;
                }

                // Reset the flags to false
                idFound = false;
                startFound = false;
                endFound = false;

                sequenceRange = new SequenceRange();
                rangeAddress = string.Empty;
                for (int col = 1; col <= cellRange.GetLength(1); col++)
                {
                    if (cellRange[row, col] != null)
                    {
                        cellValue = cellRange[row, col].ToString();
                        if (columnMapping.TryGetValue((userSelectedRange[1, col] as Range).Column, out columnName))
                        {
                            switch (columnName) // mapped header for current column
                            {
                                case CHROM_ID:
                                    sequenceRange.ID = cellValue;
                                    idFound = true;
                                    break;

                                case CHROM_START:
                                    sequenceRange.Start = long.Parse(cellValue, CultureInfo.InvariantCulture);
                                    startFound = true;
                                    break;

                                case CHROM_END:
                                    sequenceRange.End = long.Parse(cellValue, CultureInfo.InvariantCulture);
                                    endFound = true;
                                    break;

                                default:
                                    // any other item goes into the metadata dictionary
                                    sequenceRange.Metadata.Add(columnMapping[(userSelectedRange[1, col] as Range).Column], cellValue);
                                    break;
                            }
                        }
                    }
                }

                if (idFound & startFound & endFound)
                {
                    rangeAddress = GetRangeAddress(userSelectedRange, sortedColumn, rangeWidth, row);
                    sequenceRanges.Add(sequenceRange, rangeAddress);
                }
            }

            return sequenceRanges;
        }

        /// <summary>
        /// Convert given range containing any quality values to QualitativeSequence object
        /// </summary>
        /// <param name="range">Range of cells</param>
        /// <param name="sequence">Sequece object</param>
        /// <returns>QualitativeSequence Object</returns>
        public static QualitativeSequence RangeToQualitativeSequence(List<Range> range, ISequence sequence)
        {
            string[] rangeData = FlattenToArray(range);
            // see if we have enough quality scores to map with the sequence
            if (rangeData.Length < sequence.Count)
                throw new FormatException(Properties.Resources.ExportFastQ_SequenceAndScoresNotMapping);

            System.Collections.IEnumerator qualityScores = rangeData.GetEnumerator();
            byte[] sequenceSymbols = new byte[sequence.Count];
            byte[] sequencequalityValues = new byte[sequence.Count];
            byte currentQualityScore;

            long curIndex = 0;
            foreach (byte sequenceSymbol in sequence)
            {
                qualityScores.MoveNext();

                if (byte.TryParse(qualityScores.Current.ToString(), out currentQualityScore))
                {
                    sequenceSymbols[curIndex] = sequenceSymbol;
                    sequencequalityValues[curIndex] = currentQualityScore;
                    curIndex++;
                }
                else
                {
                    throw new FormatException(Properties.Resources.ExportFasQ_InvalidQualityScore);
                }
            }

            QualitativeSequence qualitativeSequence = new QualitativeSequence(sequence.Alphabet, FastQFormatType.Sanger, sequenceSymbols, sequencequalityValues)
            {
                ID = sequence.ID
            };
            
            return qualitativeSequence;
        }
       
        /// <summary>
        /// Parses the gff metadata from the specified ranges.
        /// </summary>
        /// <param name="sequence">Sequence object</param>
        /// <param name="ranges">ranges.</param>
        /// <returns></returns>
        public static void RangeToGffMetadata(ISequence sequence, IList<Range> ranges)
        {
            int height = 0, width = 0;
            object[,] cellRange;

            foreach (Range r in ranges)
            {
                height += r.Rows.Count;
                width = width < r.Columns.Count ? r.Columns.Count : width;
            }

            cellRange = new object[height + 1, width + 1];
            int k = 1;
            foreach (Range r in ranges)
            {
                for (int i = 1; i <= r.Rows.Count; i++, k++)
                {
                    for (int j = 1; j <= r.Columns.Count; j++)
                    {
                        cellRange[k, j] = r[i, j].Value2 as object;
                    }
                }
            }

            int rowIndex = 1;
            ParseGffFeatures(sequence, cellRange, rowIndex);
        }

        /// <summary>
        /// Given range may contain normal metadata and features, if you can find 
        /// the heading 'Features' in any row of the range, anything below it is 
        /// part of features otherwise, try to parse everything as metadata.
        /// </summary>
        /// <param name="ranges">ranges</param>
        /// <returns></returns>
        public static GenBankMetadata RangeToGenBankMetadata(IList<Range> ranges)
        {
            GenBankMetadata metadata = new GenBankMetadata();
            int height = 0, width = 0;
            object[,] cellRange;

            foreach (Range r in ranges)
            {
                height += r.Rows.Count;
                width = width < r.Columns.Count ? r.Columns.Count : width;
            }

            cellRange = new object[height + 1, width + 1];
            int k = 1;
            foreach (Range r in ranges)
            {
                for (int i = 1; i <= r.Rows.Count; i++, k++)
                {
                    for (int j = 1; j <= r.Columns.Count; j++)
                    {
                        cellRange[k, j] = r[i, j].Value2 as object;
                    }
                }
            }

            int rowIndex = 1;

            while (rowIndex < cellRange.GetLength(0))
            {
                if (null != cellRange[rowIndex, 1])
                {
                    string cellValue = cellRange[rowIndex, 1].ToString().ToUpperInvariant();
                    switch (cellValue)
                    {
                        case METADATA:
                            rowIndex++;
                            rowIndex = ParseGenBankMetadata(metadata,
                                    cellRange,
                                    rowIndex);
                            break;
                        case FEATURES:
                            rowIndex++;
                            rowIndex = ParseGenBankFeatures(metadata,
                                    cellRange,
                                    rowIndex);
                            break;
                        default:
                            rowIndex++;
                            break;
                    }
                }
                else
                {
                    rowIndex++;
                }
            }

            return metadata;
        }

        /// <summary>
        /// Parses locus info.
        /// </summary>
        /// <param name="metadata">Metadata object</param>
        /// <param name="cellRange">Range of cells</param>
        /// <param name="rowIndex">Current index of row</param>
        /// <returns>Index of row</returns>
        private static int ParseLocus(GenBankMetadata metadata, object[,] cellRange, int rowIndex)
        {
            string Key;
            string subKey;
            string value;
            string message;
            rowIndex++;

            while (rowIndex < cellRange.GetLength(0))
            {
                if (3 > cellRange.GetLength(1))
                {
                    message = String.Format(
                                CultureInfo.InvariantCulture,
                                Resources.UnrecognizedGenBankMetadataFormat,
                                LOCUS);
                    throw new FormatException(message);
                }

                if (null != cellRange[rowIndex, KeyColumnIndex])
                {
                    Key = cellRange[rowIndex, KeyColumnIndex].ToString();
                    if (!string.IsNullOrWhiteSpace(Key))
                    {
                        break;
                    }
                }

                if (null == cellRange[rowIndex, SubKeyColumnIndex] || string.IsNullOrWhiteSpace(cellRange[rowIndex, SubKeyColumnIndex].ToString()))
                {
                    message = String.Format(
                              CultureInfo.InvariantCulture,
                              Resources.UnrecognizedGenBankMetadataFormat,
                              LOCUS);
                    throw new FormatException(message);
                }

                if (metadata.Locus == null)
                {
                    metadata.Locus = new GenBankLocusInfo();
                }

                subKey = cellRange[rowIndex, SubKeyColumnIndex].ToString().ToUpperInvariant();
                value = cellRange[rowIndex, ValueColumnIndex] != null ? cellRange[rowIndex, ValueColumnIndex].ToString() : string.Empty;
                switch (subKey)
                {
                    case LOCUS_NAME:
                        metadata.Locus.Name = value;
                        break;
                    case LOCUS_SEQLEN:
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            metadata.Locus.SequenceLength = int.Parse(value);
                        }

                        break;
                    case LOCUS_SEQTYPE:
                        metadata.Locus.SequenceType = value;
                        break;
                    case LOCUS_MOLTYPE:
                        MoleculeType moleculetype = MoleculeType.NA;

                        if (!string.IsNullOrWhiteSpace(value) && Enum.TryParse<MoleculeType>(value, true, out moleculetype))
                        {
                            metadata.Locus.MoleculeType = moleculetype;
                        }
                        else
                        {
                            message = String.Format(
                               CultureInfo.InvariantCulture,
                               Resources.UnrecognizedGenBankMetadataFormat,
                               LOCUS_MOLTYPE);
                            throw new FormatException(message);
                        }
                        break;
                    case LOCUS_STRANTTOPOLOGY:
                        SequenceStrandTopology strandTopology = SequenceStrandTopology.None;
                        if (!string.IsNullOrWhiteSpace(value) && Enum.TryParse<SequenceStrandTopology>(value, true, out strandTopology))
                        {
                            metadata.Locus.StrandTopology = strandTopology;
                        }
                        else
                        {
                            message = String.Format(
                             CultureInfo.InvariantCulture,
                             Resources.UnrecognizedGenBankMetadataFormat,
                             LOCUS_STRANTTOPOLOGY);
                            throw new FormatException(message);
                        }

                        break;

                    case LOCUS_STRANDTYPE:

                        SequenceStrandType strandtype = SequenceStrandType.None;
                        if (!string.IsNullOrWhiteSpace(value) && Enum.TryParse<SequenceStrandType>(value, true, out strandtype))
                        {
                            metadata.Locus.Strand = strandtype;
                        }
                        else
                        {
                            message = String.Format(
                            CultureInfo.InvariantCulture,
                            Resources.UnrecognizedGenBankMetadataFormat,
                            LOCUS_STRANDTYPE);
                            throw new FormatException(message);
                        }

                        break;
                    case LOCUS_DIVISIONCODE:
                        SequenceDivisionCode divisionCode = SequenceDivisionCode.None;
                        if (!string.IsNullOrWhiteSpace(value) && Enum.TryParse<SequenceDivisionCode>(value, true, out divisionCode))
                        {
                            metadata.Locus.DivisionCode = divisionCode;
                        }
                        else
                        {
                            message = String.Format(
                           CultureInfo.InvariantCulture,
                           Resources.UnrecognizedGenBankMetadataFormat,
                           LOCUS_DIVISIONCODE);
                            throw new FormatException(message);
                        }

                        break;
                    case LOCUS_DATE:
                        DateTime date;
                        if (!string.IsNullOrWhiteSpace(value) && DateTime.TryParse(value, out date))
                        {
                            metadata.Locus.Date = date;
                        }
                        else
                        {
                            message = String.Format(
                            CultureInfo.InvariantCulture,
                            Resources.UnrecognizedGenBankMetadataFormat,
                            LOCUS_DATE);
                            throw new FormatException(message);
                        }

                        break;
                    default:
                        message = String.Format(
                             CultureInfo.InvariantCulture,
                             Resources.UnrecognizedGenBankMetadataFormat,
                             LOCUS);
                        throw new FormatException(message);
                }

                rowIndex++;
            }

            return rowIndex;
        }

        /// <summary>
        /// Parses source info.
        /// </summary>
        /// <param name="metadata">Metadata object</param>
        /// <param name="cellRange">Range of cells</param>
        /// <param name="rowIndex">Current index of row</param>
        /// <returns>Index of row</returns>
        private static int ParseSource(GenBankMetadata metadata, object[,] cellRange, int rowIndex)
        {
            string Key;
            string subKey;
            string value;
            string message;

            value = cellRange[rowIndex, ValueColumnIndex] != null ? cellRange[rowIndex, ValueColumnIndex].ToString() : string.Empty;
            rowIndex++;

            while (rowIndex < cellRange.GetLength(0))
            {
                if (3 > cellRange.GetLength(1))
                {
                    message = String.Format(
                                CultureInfo.InvariantCulture,
                                Resources.UnrecognizedGenBankMetadataFormat,
                                SOURCE);
                    throw new FormatException(message);
                }

                if (null != cellRange[rowIndex, KeyColumnIndex])
                {
                    Key = cellRange[rowIndex, KeyColumnIndex].ToString();
                    if (!string.IsNullOrWhiteSpace(Key))
                    {
                        break;
                    }
                }

                if (null == cellRange[rowIndex, SubKeyColumnIndex] || string.IsNullOrWhiteSpace(cellRange[rowIndex, SubKeyColumnIndex].ToString()))
                {
                    message = String.Format(
                              CultureInfo.InvariantCulture,
                              Resources.UnrecognizedGenBankMetadataFormat,
                              SOURCE);
                    throw new FormatException(message);
                }

                if (metadata.Source == null)
                {
                    metadata.Source = new SequenceSource();
                    metadata.Source.CommonName = value;
                }

                subKey = cellRange[rowIndex, SubKeyColumnIndex].ToString().ToUpperInvariant();
                value = cellRange[rowIndex, ValueColumnIndex] != null ? cellRange[rowIndex, ValueColumnIndex].ToString() : string.Empty;

                if (metadata.Source.Organism == null)
                {
                    metadata.Source.Organism = new OrganismInfo();
                }

                switch (subKey)
                {
                    case SOURCE_ORGANISM:
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            break;
                        }

                        string[] tokens = value.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        metadata.Source.Organism.Genus = tokens[0];

                        if (tokens.Length > 1)
                        {
                            metadata.Source.Organism.Species = tokens[1];

                            for (int i = 2; i < tokens.Length; i++)
                            {
                                metadata.Source.Organism.Species = metadata.Source.Organism.Species + " " + tokens[i];
                            }
                        }
                        break;
                    case SOURCE_CLASSLEVELS:
                        metadata.Source.Organism.ClassLevels = value;
                        break;
                }

                rowIndex++;
            }

            return rowIndex;
        }

        /// <summary>
        /// Parses reference info.
        /// </summary>
        /// <param name="metadata">Metadata object</param>
        /// <param name="cellRange">Range of cells</param>
        /// <param name="rowIndex">Current index of row</param>
        /// <returns>Index of row</returns>
        private static int ParseReference(GenBankMetadata metadata, object[,] cellRange, int rowIndex)
        {
            string Key;
            string subKey;
            string value;
            string message;

            value = cellRange[rowIndex, ValueColumnIndex] != null ? cellRange[rowIndex, ValueColumnIndex].ToString() : string.Empty;
            rowIndex++;
            CitationReference reference = new CitationReference();
            if (!string.IsNullOrWhiteSpace(value))
            {
                // check for start/end e.g. (bases 1 to 118), or prose notes
                Match m = Regex.Match(value,
                    @"^(?<number>\d+)(\s+\((?<location>.*)\))?");
                if (m.Success)
                {
                    // create new reference
                    string number = m.Groups["number"].Value;
                    string location = m.Groups["location"].Value;
                    int outValue;
                    if (!int.TryParse(number, out outValue))
                    {
                        message = String.Format(
                         CultureInfo.InvariantCulture,
                         Resources.UnrecognizedGenBankMetadataFormat,
                         REFERENCE);
                        throw new FormatException(message);
                    }

                    reference.Number = outValue;
                    reference.Location = location;
                }
            }

            while (rowIndex < cellRange.GetLength(0))
            {
                if (3 > cellRange.GetLength(1))
                {
                    message = String.Format(
                                CultureInfo.InvariantCulture,
                                Resources.UnrecognizedGenBankMetadataFormat,
                                REFERENCE);
                    throw new FormatException(message);
                }

                if (null != cellRange[rowIndex, KeyColumnIndex])
                {
                    Key = cellRange[rowIndex, KeyColumnIndex].ToString();
                    if (!string.IsNullOrWhiteSpace(Key))
                    {
                        break;
                    }
                }

                if (null == cellRange[rowIndex, SubKeyColumnIndex] || string.IsNullOrWhiteSpace(cellRange[rowIndex, SubKeyColumnIndex].ToString()))
                {
                    message = String.Format(
                              CultureInfo.InvariantCulture,
                              Resources.UnrecognizedGenBankMetadataFormat,
                              REFERENCE);
                    throw new FormatException(message);
                }

                subKey = cellRange[rowIndex, SubKeyColumnIndex].ToString().ToUpperInvariant();
                value = cellRange[rowIndex, ValueColumnIndex] != null ? cellRange[rowIndex, ValueColumnIndex].ToString() : string.Empty;
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                switch (subKey)
                {
                    case REFERENCE_AUTHORS:
                        reference.Authors = value;
                        break;
                    case REFERENCE_CONSORTIUMS:
                        reference.Consortiums = value;
                        break;
                    case REFERENCE_JOURNAL:
                        reference.Journal = value;
                        break;
                    case REFERENCE_MEDLINE:
                        reference.Medline = value;
                        break;
                    case REFERENCE_PUBMED:
                        reference.PubMed = value;
                        break;
                    case REFERENCE_REMARK:
                        reference.Remarks = value;
                        break;
                    case REFERENCE_TITLE:
                        reference.Title = value;
                        break;
                }

                rowIndex++;
            }

            metadata.References.Add(reference);

            return rowIndex;
        }

        /// <summary>
        /// Helper method to parse the metadata of gen bank data
        /// </summary>
        /// <param name="metadata">Metadata object</param>
        /// <param name="cellRange">Range of cells</param>
        /// <param name="rowIndex">Current index of row</param>
        /// <returns>Index of row</returns>
        private static int ParseGenBankMetadata(GenBankMetadata metadata, object[,] cellRange, int rowIndex)
        {
            string message = string.Empty;
            string key;
            string subKey;
            string value = string.Empty;
            while (rowIndex < cellRange.GetLength(0))
            {
                if (null != cellRange[rowIndex, KeyColumnIndex] && !string.IsNullOrWhiteSpace(cellRange[rowIndex, KeyColumnIndex].ToString()))
                {
                    key = cellRange[rowIndex, KeyColumnIndex].ToString().ToUpperInvariant();
                    if (key.Equals(FEATURES))
                    {
                        break;
                    }
                }
                else
                {
                    rowIndex++;
                    continue;
                }

                subKey = cellRange[rowIndex, SubKeyColumnIndex] != null ? cellRange[rowIndex, SubKeyColumnIndex].ToString() : string.Empty;
                value = cellRange[rowIndex, ValueColumnIndex] != null ? cellRange[rowIndex, ValueColumnIndex].ToString() : string.Empty;
                string[] tokens;
                switch (key)
                {
                    case LOCUS:
                        rowIndex = ParseLocus(metadata, cellRange, rowIndex);
                        rowIndex--;
                        break;
                    case DEFINITION:
                        metadata.Definition = value;
                        break;
                    case ACCESSION:

                        metadata.Accession = new GenBankAccession();
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            message = String.Format(
                             CultureInfo.InvariantCulture,
                             Resources.UnrecognizedGenBankMetadataFormat,
                             ACCESSION);
                            throw new FormatException(message);
                        }

                        string[] accessions = value.Split(' ');
                        metadata.Accession.Primary = accessions[0];

                        for (int i = 1; i < accessions.Length; i++)
                        {
                            metadata.Accession.Secondary.Add(accessions[i]);
                        }
                        break;

                    case DBLINK:
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            break;
                        }

                        tokens = value.Split(':');
                        if (tokens.Length == 2)
                        {
                            if (metadata.DbLinks == null) 
                            { metadata.DbLinks = new List<CrossReferenceLink>(2); }
                            var curLink = new CrossReferenceLink();
                            
                            if (string.Compare(tokens[0],
                                CrossReferenceType.Project.ToString(),
                                StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                curLink.Type = CrossReferenceType.Project;
                            }
                            else if (string.Compare(tokens[0], 
                                CrossReferenceType.BioProject.ToString(), 
                                StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                curLink.Type = CrossReferenceType.BioProject;
                            }
                            else
                            {
                                curLink.Type = CrossReferenceType.TraceAssemblyArchive;
                            }

                            tokens = tokens[1].Split(',');
                            for (int i = 0; i < tokens.Length; i++)
                            {
                                curLink.Numbers.Add(tokens[i]);
                            }
                            metadata.DbLinks.Add(curLink);
                        }

                        break;
                    case DBSOURCE:
                        metadata.DbSource = value;
                        break;

                    case VERSION:
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            break;
                        }

                        tokens = value.Split(new char[] { ' ' },
                           StringSplitOptions.RemoveEmptyEntries);
                        // first token contains accession and version
                        Match m = Regex.Match(tokens[0], @"^(?<accession>\w+)\.(?<version>\d+)$");
                        metadata.Version = new GenBankVersion();

                        if (m.Success)
                        {
                            metadata.Version.Version = m.Groups["version"].Value;
                            // The first token in the data from the accession line is referred to as
                            // the primary accession number, and should be the one used here in the
                            // version line.
                            metadata.Version.Accession = m.Groups["accession"].Value;
                        }

                        if (tokens.Length > 1)
                        {
                            // second token contains primary ID
                            m = Regex.Match(tokens[1], @"^GI:(?<primaryID>.*)");
                            if (m.Success)
                            {
                                metadata.Version.GiNumber = m.Groups["primaryID"].Value;
                            }
                        }
                        break;
                    case SEGMENT:
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            break;
                        }

                        tokens = value.Split(" of ".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (tokens.Length == 2)
                        {
                            int current;
                            int count;
                            if (int.TryParse(tokens[0], out current))
                            {
                                if (int.TryParse(tokens[1], out count))
                                {
                                    metadata.Segment = new SequenceSegment();
                                    metadata.Segment.Current = current;
                                    metadata.Segment.Count = count;
                                }
                            }
                        }

                        if (metadata.Segment == null)
                        {
                            message = String.Format(
                            CultureInfo.InvariantCulture,
                            Resources.UnrecognizedGenBankMetadataFormat,
                            ACCESSION);
                            throw new FormatException(message);
                        }

                        break;
                    case KEYWORDS:
                        metadata.Keywords = value;
                        break;
                    case SOURCE:
                        rowIndex = ParseSource(metadata, cellRange, rowIndex);
                        rowIndex--;
                        break;
                    case REFERENCE:
                        rowIndex = ParseReference(metadata, cellRange, rowIndex);
                        rowIndex--;
                        break;
                    case PRIMARY:
                        metadata.Primary = value;
                        break;
                    case COMMENT:
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            tokens = value.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            foreach (string str in tokens)
                            {
                                metadata.Comments.Add(str);
                            }
                        }
                        break;
                }

                rowIndex++;
            }

            return rowIndex;
        }

        /// <summary>
        /// Helper method to parse the feature's qualifiers of gen bank data.
        /// </summary>
        /// <param name="metadata">feature object</param>
        /// <param name="cellRange">Range of cells</param>
        /// <param name="rowIndex">Current index of row</param>
        /// <returns>Index of row</returns>
        private static int ParseQualifiers(FeatureItem featureItem, object[,] cellRange, int rowIndex)
        {
            string message = string.Empty;
            string key;
            string subKey;
            string value = string.Empty;

            while (rowIndex < cellRange.GetLength(0))
            {
                if (3 > cellRange.GetLength(1))
                {
                    message = String.Format(
                                CultureInfo.InvariantCulture,
                                Resources.UnrecognizedGenBankMetadataFormat,
                                REFERENCE);
                    throw new FormatException(message);
                }

                if (null != cellRange[rowIndex, KeyColumnIndex])
                {
                    key = cellRange[rowIndex, KeyColumnIndex].ToString().ToUpperInvariant();
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        break;
                    }
                }

                subKey = cellRange[rowIndex, SubKeyColumnIndex] != null ? cellRange[rowIndex, SubKeyColumnIndex].ToString() : string.Empty;
                if (string.IsNullOrWhiteSpace(subKey))
                {
                    rowIndex++;
                    continue;
                }

                value = cellRange[rowIndex, ValueColumnIndex] != null ? cellRange[rowIndex, ValueColumnIndex].ToString() : string.Empty;

                if (!featureItem.Qualifiers.ContainsKey(subKey))
                {
                    featureItem.Qualifiers[subKey] = new List<string>();
                }

                featureItem.Qualifiers[subKey].Add(value);

                rowIndex++;
            }

            return rowIndex;
        }

        /// <summary>
        /// Helper method to parse the feature of gen bank data
        /// </summary>
        /// <param name="metadata">Metadata object</param>
        /// <param name="cellRange">Range of cells</param>
        /// <param name="rowIndex">Current index of row</param>
        /// <returns>Index of row</returns>
        private static int ParseGenBankFeatures(GenBankMetadata metadata, object[,] cellRange, int rowIndex)
        {

            string message = string.Empty;
            string key;
            string subKey;
            string value = string.Empty;

            while (rowIndex < cellRange.GetLength(0))
            {
                if (null != cellRange[rowIndex, KeyColumnIndex])
                {
                    key = cellRange[rowIndex, KeyColumnIndex].ToString().ToUpperInvariant();
                    if (key.Equals(METADATA))
                    {
                        break;
                    }
                }
                else
                {
                    rowIndex++;
                    continue;
                }

                if (3 > cellRange.GetLength(1))
                {
                    message = String.Format(
                                CultureInfo.InvariantCulture,
                                Resources.UnrecognizedGenBankMetadataFormat,
                                REFERENCE);
                    throw new FormatException(message);
                }

                subKey = cellRange[rowIndex, SubKeyColumnIndex] != null ? cellRange[rowIndex, SubKeyColumnIndex].ToString() : string.Empty;
                value = cellRange[rowIndex, ValueColumnIndex] != null ? cellRange[rowIndex, ValueColumnIndex].ToString() : string.Empty;

                if (key.Equals(BASECOUNT))
                {
                    metadata.BaseCount = value;
                    rowIndex++;
                }
                else if (!string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(key))
                {
                    FeatureItem featureItem = StandardFeatureMap.GetStandardFeatureItem(new FeatureItem(key, value));
                    if (metadata.Features == null)
                    {
                        metadata.Features = new SequenceFeatures();
                    }

                    metadata.Features.All.Add(featureItem);
                    rowIndex++;
                    rowIndex = ParseQualifiers(featureItem, cellRange, rowIndex);
                }
                else
                {
                    rowIndex++;
                }
            }

            return rowIndex;
        }

        /// <summary>
        /// Helper method to parse the feature of Gff data
        /// </summary>
        /// <param name="sequence">sequence object</param>
        /// <param name="cellRange">Range of cells</param>
        /// <param name="rowIndex">Current index of row</param>
        /// <returns>Index of row</returns>
        private static int ParseGffFeatures(ISequence sequence, object[,] cellRange, int rowIndex)
        {
            Dictionary<string, object> metadata = sequence.Metadata;

            Sequence seq = sequence as Sequence;

            if (cellRange.GetLength(1) < 9 && cellRange.GetLength(1) > 10)
            {
                throw new FormatException(Resources.UnrecognizedGffMetadataFormat);
            }

            int nameColIndex = -1;
            int sourceColIndex = -1;
            int typeColIndex = -1;
            int startColIndex = -1;
            int endColIndex = -1;
            int scoreColIndex = -1;
            int strandColIndex = -1;
            int frameColIndex = -1;
            int groupColIndex = -1;

            for (int i = 1; i < cellRange.GetLength(1); i++)
            {
                if (cellRange[rowIndex, i] == null)
                {
                    continue;
                }

                if (cellRange[rowIndex, i].ToString().ToUpperInvariant().Equals(Properties.Resources.GffColumnName.ToUpperInvariant()))
                {
                    nameColIndex = i;
                }

                if (cellRange[rowIndex, i].ToString().ToUpperInvariant().Equals(Properties.Resources.GffColumnSource.ToUpperInvariant()))
                {
                    sourceColIndex = i;
                }

                if (cellRange[rowIndex, i].ToString().ToUpperInvariant().Equals(Properties.Resources.GffColumnType.ToUpperInvariant()))
                {
                    typeColIndex = i;
                }

                if (cellRange[rowIndex, i].ToString().ToUpperInvariant().Equals(Properties.Resources.GffColumnStart.ToUpperInvariant()))
                {
                    startColIndex = i;
                }

                if (cellRange[rowIndex, i].ToString().ToUpperInvariant().Equals(Properties.Resources.GffColumnEnd.ToUpperInvariant()))
                {
                    endColIndex = i;
                }

                if (cellRange[rowIndex, i].ToString().ToUpperInvariant().Equals(Properties.Resources.GffColumnScore.ToUpperInvariant()))
                {
                    scoreColIndex = i;
                }

                if (cellRange[rowIndex, i].ToString().ToUpperInvariant().Equals(Properties.Resources.GffColumnStrand.ToUpperInvariant()))
                {
                    strandColIndex = i;
                }

                if (cellRange[rowIndex, i].ToString().ToUpperInvariant().Equals(Properties.Resources.GffColumnFrame.ToUpperInvariant()))
                {
                    frameColIndex = i;
                }

                if (cellRange[rowIndex, i].ToString().ToUpperInvariant().Equals(Properties.Resources.GffColumnGroup.ToUpperInvariant()))
                {
                    groupColIndex = i;
                }
            }

            if (nameColIndex == -1 ||
                sourceColIndex == -1 ||
                typeColIndex == -1 ||
                startColIndex == -1 ||
                endColIndex == -1 ||
             scoreColIndex == -1 ||
            strandColIndex == -1 ||
             frameColIndex == -1)
            {
                throw new FormatException(Resources.UnrecognizedGffMetadataFormat);
            }

            List<MetadataListItem<List<string>>> featureList = new List<MetadataListItem<List<string>>>();

            metadata["features"] = featureList;
            rowIndex++;

            while (rowIndex < cellRange.GetLength(0))
            {
                string name = cellRange[rowIndex, nameColIndex] != null ? cellRange[rowIndex, nameColIndex].ToString() : string.Empty;
                string value = cellRange[rowIndex, typeColIndex] != null ? cellRange[rowIndex, typeColIndex].ToString() : string.Empty;

                string attributes = string.Empty;
                if (groupColIndex != -1)
                {
                    attributes = cellRange[rowIndex, groupColIndex] != null ? cellRange[rowIndex, groupColIndex].ToString() : string.Empty;
                }

                MetadataListItem<List<string>> feature = new MetadataListItem<List<string>>(value, attributes);

                value = cellRange[rowIndex, sourceColIndex] != null ? cellRange[rowIndex, sourceColIndex].ToString() : string.Empty;
                feature.SubItems.Add("source", new List<string> { value });

                // start is an int
                int ignoreMe;

                value = cellRange[rowIndex, startColIndex] != null ? cellRange[rowIndex, startColIndex].ToString() : string.Empty;
                if (!int.TryParse(value, out ignoreMe))
                {
                    string message = String.Format(
                            CultureInfo.CurrentCulture,
                            Resources.GffInvalidField,
                            "start",
                            value);
                    throw new InvalidDataException(message);
                }

                feature.SubItems.Add("start", new List<string> { value });

                // end is an int
                value = cellRange[rowIndex, endColIndex] != null ? cellRange[rowIndex, endColIndex].ToString() : string.Empty;
                if (!int.TryParse(value, out ignoreMe))
                {
                    string message = String.Format(
                            CultureInfo.CurrentCulture,
                            Resources.GffInvalidField,
                            "end",
                            value);

                    throw new InvalidDataException(message);
                }

                feature.SubItems.Add("end", new List<string> { value });

                // source is a double, or a dot as a space holder
                value = cellRange[rowIndex, scoreColIndex] != null ? cellRange[rowIndex, scoreColIndex].ToString() : string.Empty;
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = ".";
                }

                if (value != ".")
                {
                    double ignoreMeToo;
                    if (!double.TryParse(value, out ignoreMeToo))
                    {
                        string message = String.Format(
                            CultureInfo.CurrentCulture,
                            Resources.GffInvalidField,
                            "score",
                            value);

                        throw new InvalidDataException(message);
                    }
                    feature.SubItems.Add("score", new List<string> { value });
                }

                // strand is + or -, or a dot as a space holder
                value = cellRange[rowIndex, strandColIndex] != null ? cellRange[rowIndex, strandColIndex].ToString() : string.Empty;
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = ".";
                }

                if (value != ".")
                {
                    if (value != "+" && value != "-")
                    {
                        string message = String.Format(
                            CultureInfo.CurrentCulture,
                            Resources.GffInvalidField,
                            "strand",
                           value);

                        throw new InvalidDataException(message);
                    }
                    feature.SubItems.Add("strand", new List<string> { value });
                }

                // frame is an int, or a dot as a space holder
                value = cellRange[rowIndex, frameColIndex] != null ? cellRange[rowIndex, frameColIndex].ToString() : string.Empty;
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = ".";
                }

                if (value != ".")
                {
                    if (!int.TryParse(value, out ignoreMe))
                    {
                        string message = String.Format(
                        CultureInfo.CurrentCulture,
                            Resources.GffInvalidField,
                            "frame",
                            value);

                        throw new InvalidDataException(message);
                    }

                    feature.SubItems.Add("frame", new List<string> { value });
                }

                // done with that one
                featureList.Add(feature);
                rowIndex++;
            }

            return rowIndex;
        }

        /// <summary>
        /// Convert a given list of range to a string array.
        /// </summary>
        /// <param name="sourceRanges">List of rnages to convert</param>
        /// <returns>String array with data from the ranges given</returns>
        private static string[] FlattenToArray(List<Range> sourceRanges)
        {
            // Find the size to be allocated to accomodate the whole range data
            int totalCount = 0;
            foreach (Range currentRange in sourceRanges)
            {
                totalCount += currentRange.Count;
            }

            // Put all to a list
            List<string> resultArray = new List<string>(totalCount);
            foreach (Range currentRange in sourceRanges)
            {
                foreach (Range currentCell in currentRange.Cells)
                {
                    // if cell is not null and its row and column is not hidden
                    if (currentCell != null && currentCell.Value2 != null && !(currentCell.EntireColumn.Hidden || currentCell.EntireRow.Hidden))
                    {
                        resultArray.Add(currentCell.Value2.ToString());
                    }
                }
            }

            resultArray.RemoveAll(x => string.IsNullOrWhiteSpace(x)); // remove all whitespace
            return resultArray.ToArray();
        }

        /// <summary>
        /// Check if an object contains valid string data
        /// </summary>
        /// <param name="targetObject">Any object to check. (Will use ToString() method to get any valid data)</param>
        /// <returns>True is the object is not null and not white space</returns>
        private static bool IsValidString(object targetObject)
        {
            if (targetObject == null)
                return false;
            return !string.IsNullOrWhiteSpace(targetObject.ToString());
        }

        /// <summary>
        /// Gets the address of the cell
        /// </summary>
        /// <param name="userSelectedRange">Range of selected cell</param>
        /// <param name="rangeAddress">addresses of range</param>
        /// <param name="row">row index</param>
        /// <param name="column">column index</param>
        /// <returns>Address of Cell</returns>
        private static string GetRangeAddress(Range userSelectedRange, SortedSet<int> columns, int rangeWidth, int row)
        {
            Range startRange = userSelectedRange[row, 1];
            string sheetName = startRange.Worksheet.Name;
            if (rangeWidth == columns.Count)
            {
                // Range are in continuous fashion, read the address from range
                startRange = startRange.get_Resize(1, columns.Count);
                return string.Concat(sheetName, "!", startRange.Address);
            }

            // Range are spread across (unwanted column exist), so build the address
            // of only required ranges.
            int adjacentColumnCount = 0, startIndex = -1, firstIndex = 0;
            string rangeAddress = string.Empty;
            foreach (int columnIndex in columns)
            {
                if (-1 == startIndex)
                {
                    startIndex = columnIndex;
                    firstIndex = columnIndex - 1;
                    continue;
                }

                if (startIndex + adjacentColumnCount + 1 == columnIndex)
                {
                    adjacentColumnCount++;
                    continue;
                }
                else
                {
                    if (0 < adjacentColumnCount)
                    {
                        startRange = startRange.get_Resize(1, adjacentColumnCount + 1);
                    }

                    if (string.IsNullOrEmpty(rangeAddress))
                    {
                        rangeAddress = string.Concat(sheetName, "!", startRange.Address);
                    }
                    else
                    {
                        rangeAddress = string.Concat(rangeAddress, ",", sheetName, "!", startRange.Address);
                    }

                    startIndex = columnIndex;
                    startRange = userSelectedRange[row, columnIndex - firstIndex];
                    adjacentColumnCount = 0;
                }
            }

            // Add the last set of range address
            if (0 < adjacentColumnCount)
            {
                startRange = startRange.get_Resize(1, adjacentColumnCount + 1);
            }

            rangeAddress = string.Concat(rangeAddress, ",", sheetName, "!", startRange.Address);
            return rangeAddress;
        }
    }
}
