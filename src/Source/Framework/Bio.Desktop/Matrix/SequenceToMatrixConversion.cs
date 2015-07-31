using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bio.Algorithms.Alignment;
using Bio.Algorithms.Translation;
using Bio.Distributions;
using Bio.Distributions.Converters;
using Bio.IO;
using Bio.IO.FastA;
using Bio.IO.Phylip;
using Bio.Properties;
using Bio.Util;
using Bio.IO.Text;

namespace Bio.Matrix
{
    /// <summary>
    /// Static class converting sequence to Matrix. 
    /// </summary>
    public static class SequenceToMatrixConversion
    {

        #region Fields

        /// <summary>
        /// Depicts whether output values will be binary or multistate(Discrete states).
        /// </summary>
        public static WriteType ColumnType { get; set; }

        /// <summary>
        /// Depicts output values in case of ambiguous characters.
        /// </summary>
        public static MixtureSemantics MixtureSemanticsValue { get; set; }

        #endregion

        #region Public Members

        #region Registered Converters in Matrix.

        /// <summary>
        /// Constructs Protein matrix from DNA sequence by keeping one value variables in matrix. 
        /// </summary>
        /// <typeparam name="TRow">Row type of matrix.</typeparam>
        /// <typeparam name="TCol">Column type of matrix</typeparam>
        /// <typeparam name="TVal">Value of matrix.</typeparam>
        /// <param name="filename">Sequence file path.</param>
        /// <param name="missing">The special value that represents missing.</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="matrix">The matrix created</param>
        /// <returns>True if the function was able to create a matrix from the information in the file; otherwise, false</returns>
        public static bool TryGetMatrixFromSequenceFileAndPossiblyConvertDna2AaKeepOneValueVariables<TRow, TCol, TVal>(
            string filename,
            TVal missing,
            ParallelOptions parallelOptions,
            out Matrix<TRow, TCol, TVal> matrix)
        {
            return TryGetMatrixFromSequenceFileAndPossiblyConvertDNAToProtein<TRow, TCol, TVal>(filename, true, out matrix);
        }

        /// <summary>
        /// Constructs DNA matrix from DNA sequence by keeping one value variables in matrix. 
        /// </summary>
        /// <typeparam name="TRow">Row type of matrix.</typeparam>
        /// <typeparam name="TCol">Column type of matrix</typeparam>
        /// <typeparam name="TVal">Value of matrix.</typeparam>
        /// <param name="filename">Sequence file path.</param>
        /// <param name="missing">The special value that represents missing.</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="matrix">The matrix created</param>
        /// <returns>True if the function was able to create a matrix from the information in the file; otherwise, false</returns>
        public static bool TryGetMatrixFromSequenceFileKeepOneValueVariables<TRow, TCol, TVal>(
            string filename,
            TVal missing,
            ParallelOptions parallelOptions,
            out Matrix<TRow, TCol, TVal> matrix)
        {
            return TryGetMatrixFromSequenceFileForDNASequence<TRow, TCol, TVal>(filename, true, out matrix);
        }

        /// <summary>
        /// Constructs Protein matrix from DNA sequence by not keeping one value variables in matrix. 
        /// </summary>
        /// <typeparam name="TRow">Row type of matrix.</typeparam>
        /// <typeparam name="TCol">Column type of matrix</typeparam>
        /// <typeparam name="TVal">Value of matrix.</typeparam>
        /// <param name="filename">Sequence file path.</param>
        /// <param name="missing">The special value that represents missing.</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="matrix">The matrix created</param>
        /// <returns>True if the function was able to create a matrix from the information in the file; otherwise, false</returns>
        public static bool TryGetMatrixFromSequenceFileAndPossiblyConvertDna2AaIgnoreOneValueVariables<TRow, TCol, TVal>(
            string filename,
            TVal missing,
            ParallelOptions parallelOptions,
            out Matrix<TRow, TCol, TVal> matrix)
        {
            return TryGetMatrixFromSequenceFileAndPossiblyConvertDNAToProtein<TRow, TCol, TVal>(filename, false, out matrix);
        }

        /// <summary>
        /// Constructs DNA matrix from DNA sequence by ignoring one value variables in matrix. 
        /// </summary>
        /// <typeparam name="TRow">Row type of matrix.</typeparam>
        /// <typeparam name="TCol">Column type of matrix</typeparam>
        /// <typeparam name="TVal">Value of matrix.</typeparam>
        /// <param name="filename">Sequence file path.</param>
        /// <param name="missing">The special value that represents missing.</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="matrix">The matrix created</param>
        /// <returns>True if the function was able to create a matrix from the information in the file; otherwise, false</returns>
        public static bool TryGetMatrixFromSequenceFileIgnoreOneValueVariables<TRow, TCol, TVal>(
            string filename,
            TVal missing,
            ParallelOptions parallelOptions,
            out Matrix<TRow, TCol, TVal> matrix)
        {
            return TryGetMatrixFromSequenceFileForDNASequence<TRow, TCol, TVal>(filename, false, out matrix);
        }

        #endregion

        /// <summary>
        /// Constructs DNA matrix from DNA sequence.
        /// </summary>
        /// <typeparam name="TRow">Row type of matrix.</typeparam>
        /// <typeparam name="TCol">Column type of matrix</typeparam>
        /// <typeparam name="TVal">Value of matrix.</typeparam>
        /// <param name="filename">Sequence file path.</param>
        /// <param name="keepOneValueVariables">Keeps variables with single value only.</param>
        /// <param name="matrix">The matrix created</param>
        /// <returns>True if the function was able to create a matrix from the information in the file; otherwise, false</returns>
        public static bool TryGetMatrixFromSequenceFileForDNASequence<TRow, TCol, TVal>(
            string filename,
            bool keepOneValueVariables,
            out Matrix<TRow, TCol, TVal> matrix)
        {
            matrix = null;
            IList<ISequence> sequences;
            if (!TryParseSequenceFile(filename, Alphabets.DNA, out sequences))
            {
                return false;
            }

            ValidateSequences(sequences);

            return TryCreateMatrix<TRow, TCol, TVal>(
                sequences.Select(seq => RemoveAmbigiousDNACharacters(seq)).ToList(),
                sequences.Select(seq => seq.ID),
                keepOneValueVariables,
                out matrix);
        }

        /// <summary>
        /// Constructs RNA matrix from DNA sequence. 
        /// </summary>
        /// <typeparam name="TRow">Row type of matrix.</typeparam>
        /// <typeparam name="TCol">Column type of matrix</typeparam>
        /// <typeparam name="TVal">Value of matrix.</typeparam>
        /// <param name="filename">Sequence file path.</param>
        /// <param name="keepOneValueVariables">Keeps variables with single value only.</param>
        /// <param name="matrix">The matrix created</param>
        /// <returns>True if the function was able to create a matrix from the information in the file; otherwise, false</returns>
        public static bool TryGetMatrixFromSequenceFileAndPossiblyConvertDNAToRNA<TRow, TCol, TVal>(
            string filename,
            bool keepOneValueVariables,
            out Matrix<TRow, TCol, TVal> matrix)
        {
            matrix = null;
            IList<ISequence> sequences;
            if (!TryParseSequenceFile(filename, Alphabets.DNA, out sequences))
            {
                return false;
            }

            ValidateSequences(sequences);
            return TryCreateMatrix<TRow, TCol, TVal>(
                sequences.Select(seq => ConvertDNAToRNA(seq)).ToList(),
                sequences.Select(seq => seq.ID),
                keepOneValueVariables,
                out matrix);
        }

        /// <summary>
        /// Constructs Protein matrix from DNA sequence.
        /// </summary>
        /// <typeparam name="TRow">Row type of matrix.</typeparam>
        /// <typeparam name="TCol">Column type of matrix</typeparam>
        /// <typeparam name="TVal">Value of matrix.</typeparam>
        /// <param name="filename">Sequence file path.</param>
        /// <param name="keepOneValueVariables">Keeps variables with single value only.</param>
        /// <param name="matrix">The matrix created</param>
        /// <param name="readingFrame">Reading frame used for translation.</param>
        /// <param name="isMissing">Treat gap as missing values.</param>
        /// <returns>True if the function was able to create a matrix from the information in the file; otherwise, false</returns>
        public static bool TryGetMatrixFromSequenceFileAndPossiblyConvertDNAToProtein<TRow, TCol, TVal>(
            string filename,
            bool keepOneValueVariables,
            out Matrix<TRow, TCol, TVal> matrix,
            int readingFrame = 0,
            bool isMissing = false)
        {
            matrix = null;
            IList<ISequence> sequences;
            if (!TryParseSequenceFile(filename, Alphabets.DNA, out sequences))
            {
                return false;
            }

            ValidateSequences(sequences);
            return TryCreateMatrix<TRow, TCol, TVal>(
                sequences.Select(seq => Translation(seq, readingFrame, isMissing, Alphabets.DNA)).ToList(),
                sequences.Select(seq => seq.ID),
                keepOneValueVariables,
                out matrix);
        }

        /// <summary>
        /// Constructs RNA matrix from RNA sequence.
        /// </summary>
        /// <typeparam name="TRow">Row type of matrix.</typeparam>
        /// <typeparam name="TCol">Column type of matrix</typeparam>
        /// <typeparam name="TVal">Value of matrix.</typeparam>
        /// <param name="filename">Sequence file path.</param>
        /// <param name="keepOneValueVariables">Keeps variables with single value only.</param>
        /// <param name="matrix">The matrix created</param>
        /// <returns>True if the function was able to create a matrix from the information in the file; otherwise, false</returns>
        public static bool TryGetMatrixFromSequenceFileForRNASequence<TRow, TCol, TVal>(
            string filename,
            bool keepOneValueVariables,
            out Matrix<TRow, TCol, TVal> matrix)
        {
            matrix = null;
            IList<ISequence> sequences;
            if (!TryParseSequenceFile(filename, Alphabets.RNA, out sequences))
            {
                return false;
            }

            ValidateSequences(sequences);
            return TryCreateMatrix<TRow, TCol, TVal>(
                sequences.Select(seq => RemoveAmbigiousRNACharacters(seq)).ToList(),
                sequences.Select(seq => seq.ID),
                keepOneValueVariables,
                out matrix);
        }

        /// <summary>
        /// Constructs Protein matrix from RNA sequence.
        /// </summary>
        /// <typeparam name="TRow">Row type of matrix.</typeparam>
        /// <typeparam name="TCol">Column type of matrix</typeparam>
        /// <typeparam name="TVal">Value of matrix.</typeparam>
        /// <param name="filename">Sequence file path.</param>
        /// <param name="keepOneValueVariables">Keeps variables with single value only.</param>
        /// <param name="matrix">The matrix created</param>
        /// <param name="readingFrame">Reading frame used for translation.</param>
        /// <param name="isMissing">Treat gap as missing values.</param>
        /// <returns>True if the function was able to create a matrix from the information in the file; otherwise, false</returns>
        public static bool TryGetMatrixFromSequenceFileAndPossiblyConvertRNAToProtein<TRow, TCol, TVal>(
            string filename,
            bool keepOneValueVariables,
            out Matrix<TRow, TCol, TVal> matrix,
             int readingFrame = 0,
            bool isMissing = false)
        {
            matrix = null;
            IList<ISequence> sequences;
            if (!TryParseSequenceFile(filename, Alphabets.RNA, out sequences))
            {
                return false;
            }

            ValidateSequences(sequences);
            return TryCreateMatrix<TRow, TCol, TVal>(
                sequences.Select(seq => Translation(seq, readingFrame, isMissing, Alphabets.RNA)).ToList(),
                sequences.Select(seq => seq.ID),
                keepOneValueVariables,
                out matrix);
        }

        /// <summary>
        /// Constructs Protein matrix from Protein sequence.
        /// </summary>
        /// <typeparam name="TRow">Row type of matrix.</typeparam>
        /// <typeparam name="TCol">Column type of matrix</typeparam>
        /// <typeparam name="TVal">Value of matrix.</typeparam>
        /// <param name="filename">Sequence file path.</param>
        /// <param name="keepOneValueVariables">Keeps variables with single value only.</param>
        /// <param name="matrix">The matrix created</param>
        /// <returns>True if the function was able to create a matrix from the information in the file; otherwise, false</returns>
        public static bool TryGetMatrixFromSequenceFileForProteinSequence<TRow, TCol, TVal>(
            string filename,
            bool keepOneValueVariables,
            out Matrix<TRow, TCol, TVal> matrix)
        {
            matrix = null;
            IList<ISequence> sequences;
            if (!TryParseSequenceFile(filename, Alphabets.Protein, out sequences))
            {
                return false;
            }

            ValidateSequences(sequences);
            return TryCreateMatrix<TRow, TCol, TVal>(
                sequences.Select(seq => RemoveAmbigiousProteinCharacters(seq)).ToList(),
                sequences.Select(seq => seq.ID),
                keepOneValueVariables,
                out matrix);
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Validate sequence length are equal or not.
        /// In Multiple Sequence Alignment, all sequences have equal lengths. 
        /// </summary>
        /// <param name="sequences">Input sequences.</param>
        private static void ValidateSequences(IList<ISequence> sequences)
        {
            int seqLength = sequences.First().Count();
            if (!sequences.All(sequence => sequence.Count == seqLength))
            {
                throw new ArgumentException("Sequence lengths are not equal");
            }
        }

        /// <summary>
        /// Remove ambiguous protein characters by converting them into unambiguous characters.
        /// </summary>
        /// <param name="seq">Protein sequence.</param>
        /// <returns>List of List of protein alphabets. </returns>
        private static IList<IList<byte>> RemoveAmbigiousProteinCharacters(ISequence seq)
        {
            return seq.Select(seqItem =>
            {
                HashSet<byte> basicSymbols;
                ProteinAlphabet.Instance.TryGetBasicSymbols(seqItem, out basicSymbols);
                return (IList<byte>)new List<byte>(basicSymbols);
            }).ToList();
        }

        /// <summary>
        /// Remove ambiguous RNA characters by converting them into unambiguous characters.
        /// </summary>
        /// <param name="seq">RNA sequence.</param>
        /// <returns>List of List of RNA alphabets. </returns>
        private static IList<IList<byte>> RemoveAmbigiousRNACharacters(ISequence seq)
        {
            return seq.Select(seqItem =>
            {
                HashSet<byte> basicSymbols;
                RnaAlphabet.Instance.TryGetBasicSymbols(seqItem, out basicSymbols);
                return (IList<byte>)new List<byte>(basicSymbols);
            }).ToList();
        }

        /// <summary>
        /// Remove ambiguous DNA characters by converting them into unambiguous characters.
        /// </summary>
        /// <param name="seqs">DNA sequence.</param>
        /// <returns>List of List of DNA alphabets. </returns>
        private static IList<IList<byte>> RemoveAmbigiousDNACharacters(ISequence seqs)
        {
            return seqs.Select(seqItem =>
                {
                    HashSet<byte> basicSymbols;
                    DnaAlphabet.Instance.TryGetBasicSymbols(seqItem, out basicSymbols);
                    return (IList<byte>)new List<byte>(basicSymbols);
                }).ToList();
        }

        /// <summary>
        /// Transcription (Converts DNA to RNA alphabets.)
        /// </summary>
        /// <param name="seq">DNA sequence.</param>
        /// <returns>List of List unambiguous RNA characters.</returns>
        private static IList<IList<byte>> ConvertDNAToRNA(ISequence seq)
        {
            return seq.Select(seqItem =>
                {
                    HashSet<byte> basicSymbols;
                    DnaAlphabet.Instance.TryGetBasicSymbols(seqItem, out basicSymbols);
                    return basicSymbols.Select(item =>
                        {
                            return Transcription.GetRnaComplement(item);
                        }).ToList();
                }).ToList<IList<byte>>();
        }

        /// <summary>
        /// Create Matrix from positional sequence variation. 
        /// </summary>
        /// <typeparam name="TRow">Row type of matrix.</typeparam>
        /// <typeparam name="TCol">Column type of matrix</typeparam>
        /// <typeparam name="TVal">Value of matrix.</typeparam>
        /// <param name="sequences">List of List of positional sequence variation.</param>
        /// <param name="headerValues">Values of column headers of matrix</param>
        /// <param name="keepOneValueVariables">Keeps variables with single value only.</param>
        /// <param name="matrix">The matrix created</param>
        /// <returns>True if the function was able to create a matrix from the information in the file; otherwise, false</returns>
        private static bool TryCreateMatrix<TRow, TCol, TVal>(
            IList<IList<IList<byte>>> sequences,
            IEnumerable<string> headerValues,
            bool keepOneValueVariables,
            out Matrix<TRow, TCol, TVal> matrix)
        {
            IList<string> columnValues;
            int maxLength = sequences.Max(seq => seq.Count);
            HashSet<byte>[] positionalAminoAcidDistribution = new HashSet<byte>[maxLength];
            foreach (IList<IList<byte>> sequence in sequences)
            {
                for (int pos = 0; pos < sequence.Count; pos++)
                {
                    if (positionalAminoAcidDistribution[pos] == null)
                    {
                        positionalAminoAcidDistribution[pos] = new HashSet<byte>(sequence[pos]);
                    }
                    else
                    {
                        positionalAminoAcidDistribution[pos].AddNewOrOldRange(sequence[pos]);
                    }
                }
            }

            IList<List<SufficientStatistics>> statistics = CreateSequenceStatistics(
                positionalAminoAcidDistribution,
                sequences,
                keepOneValueVariables,
                out columnValues);

            if (ColumnType == WriteType.MultiState)
            {
                ConvertBinaryToMultistate(statistics, columnValues);
            }

            SufficientStatistics[,] matrixValues = new SufficientStatistics[statistics.Count, sequences.Count];
            for (int i = 0; i < statistics.Count; i++)
            {
                for (int j = 0; j < sequences.Count; j++)
                {
                    matrixValues[i, j] = statistics[i][j];
                }
            }
            matrix = new DenseMatrix<string, string, SufficientStatistics>(
                matrixValues, columnValues, headerValues, MissingStatistics.GetInstance) as DenseMatrix<TRow, TCol, TVal>;
            if (matrix == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Converts positional variations in sequences to statistics
        /// </summary>
        /// <param name="positionalAminoAcidDistribution">Positional distribution of alphabets.</param>
        /// <param name="sequences">List of List of positional sequence variation.</param>
        /// <param name="keepOneValueVariables">Keeps variables with single value only.</param>
        /// <param name="rowValues">Values in rows</param>
        /// <returns>Positional sequence statistics</returns>
        private static IList<List<SufficientStatistics>> CreateSequenceStatistics(
            HashSet<byte>[] positionalAminoAcidDistribution,
            IList<IList<IList<byte>>> sequences,
            bool keepOneValueVariables,
            out IList<string> rowValues) 
        {
            IList<List<SufficientStatistics>> statistics = new List<List<SufficientStatistics>>();
            rowValues = new List<string>();
            for (int pos = 0; pos < positionalAminoAcidDistribution.Length; pos++)
            {
                foreach (byte aa in positionalAminoAcidDistribution[pos])
                {
                    string merAndPos = (pos + 1) + "@" + (char)aa;
                    int?[] values = new int?[sequences.Count];
                    HashSet<int> nonMissingValues = new HashSet<int>();
                    for (int pidIdx = 0; pidIdx < sequences.Count; pidIdx++)
                    {
                        int? value;
                        IList<byte> observedAAs = sequences[pidIdx][pos];
                        if (observedAAs.Contains(Alphabets.Protein.Gap) || observedAAs.Contains(Alphabets.AmbiguousProtein.X) || observedAAs.Count == 0 ||
                            (observedAAs.Count > 1 && MixtureSemanticsValue == MixtureSemantics.none && observedAAs.Contains(aa)))
                        {
                            value = null;
                        }
                        else if (observedAAs.Contains(aa) && (MixtureSemanticsValue != MixtureSemantics.pure || observedAAs.Count == 1))
                            value = 1;
                        else
                            value = 0;

                        values[pidIdx] = value;
                        if (value != null)
                            nonMissingValues.Add((int)value);
                    }
                    if (nonMissingValues.Count > 1 || (keepOneValueVariables && nonMissingValues.Count == 1 && nonMissingValues.First() == 1))
                    {
                        rowValues.Add(merAndPos);
                        statistics.Add(
                            values.Select(value => value.HasValue ?
                            (value.Value > 0 ? BooleanStatistics.GetInstance(true) : BooleanStatistics.GetInstance(false))
                            : MissingStatistics.GetInstance).ToList<SufficientStatistics>());
                    }
                }
            }

            return statistics;
        }

        /// <summary>
        /// Convert Binary To Multistate.
        /// </summary>
        /// <param name="statistics">statistics.</param>
        /// <param name="columnValues">column Values.</param>
        private static void ConvertBinaryToMultistate(IList<List<SufficientStatistics>> statistics, IList<string> columnValues)
        {
            var multToBinaryPositions = from binaryKey in columnValues
                                        where binaryKey.Contains('@')
                                        let merAndPos = GetMerAndPos(binaryKey)
                                        let pos = (int)merAndPos.Value
                                        group binaryKey by pos into g
                                        select new KeyValuePair<string, IList<string>>
                                        (
                                             g.Key + "@" + g.StringJoin("#").Replace(g.Key + "@", ""),
                                             g.ToList()
                                        );

            var nonAaKeys = from key in columnValues
                            where !key.Contains('@')
                            select new KeyValuePair<string, IList<string>>(key, new List<string>() { key });

            var allKeys = nonAaKeys.Concat(multToBinaryPositions).ToList();

            Dictionary<string, List<SufficientStatistics>> values = statistics.Select((key, idx) =>
                new KeyValuePair<string, List<SufficientStatistics>>(columnValues[idx], key)).ToDictionary();

            columnValues.Clear();
            ((List<string>)columnValues).AddRange(allKeys.Select(keys => keys.Key));
            statistics.Clear();

            for (int index = 0; index < allKeys.Count; index++)
            {
                List<SufficientStatistics> posStatistics = new List<SufficientStatistics>();
                for (int index1 = 0; index1 < values.First().Value.Count; index1++)
                {
                    if (allKeys[index].Value.Count == 1)
                    {
                        posStatistics.Add(values[allKeys[index].Value.First()][index1]);
                    }
                    else
                    {
                        int state = 0;
                        bool valueAdded = false;
                        foreach (string key in allKeys[index].Value)
                        {
                            SufficientStatistics tempValue = values[key][index1];
                            if (ValueConverters.SufficientStatisticsToInt.ConvertForward(tempValue) == 1)
                            {
                                posStatistics.Add(ValueConverters.SufficientStatisticsToInt.ConvertBackward(state));
                                valueAdded = true;
                                break;
                            }

                            state++;
                        }

                        if (!valueAdded)
                        {
                            posStatistics.Add(MissingStatistics.GetInstance);
                        }
                    }
                }

                statistics.Add(posStatistics);
            }
        }

        /// <summary>
        /// Get Mer And Pos.
        /// </summary>
        /// <param name="variableName">Variable name.</param>
        /// <returns>Returns KeyValuePair of string and double.</returns>
        private static KeyValuePair<string, double> GetMerAndPos(string variableName)
        {
            KeyValuePair<string, double> merAndPos;
            if (!TryGetMerAndPos(variableName, out merAndPos))
            {
                throw new ArgumentException("Cannot parse " + variableName + " into merAndPos and pos.");
            }

            return merAndPos;
        }

        /// <summary>
        /// Try Get Mer And Pos.
        /// </summary>
        /// <param name="variableName">Variable name.</param>
        /// <param name="merAndPos">KeyValuePair of string and double. </param>
        /// <returns></returns>
        private static bool TryGetMerAndPos(string variableName, out KeyValuePair<string, double> merAndPos)
        {
            string[] fields = variableName.Split('@');
            double pos = -1;
            string mer;

            int posField = -1;

            // find pos and the field that describes it.
            while (++posField < fields.Length && !double.TryParse(fields[posField], out pos)) ;

            if (posField == fields.Length)  // got to the end without finding the pos.
            {
                merAndPos = new KeyValuePair<string, double>();
                return false;
            }

            // arbitrarily choose one of the non-posFields to be the mer. This is really only designed to work with mer@pos or pos@mer.
            // if we have str1@str2@pos@str3, then we make no guarantees as to what will be called mer, except that it won't be pos.

            mer = fields[fields.Length - posField - 1];

            merAndPos = new KeyValuePair<string, double>(mer, pos);
            return true;
        }

        /// <summary>
        /// Translation.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        /// <param name="readingFrame">The readingFrame.</param>
        /// <param name="isMissing">IsMissing flag.</param>
        /// <param name="alphabet">The Alphabet.</param>
        /// <returns>Returns list of list of bytes.</returns>
        private static IList<IList<byte>> Translation(ISequence sequence, int readingFrame, bool isMissing, IAlphabet alphabet)
        {
            if (readingFrame > 2)
            {
                readingFrame -= 2;
                sequence = sequence.GetReverseComplementedSequence();
            }

            HashSet<byte> gapSymbols;
            sequence.Alphabet.TryGetGapSymbols(out gapSymbols);
            if (gapSymbols == null) gapSymbols = new HashSet<byte>();

            IList<IList<byte>> sequences = new List<IList<byte>>();
            for (int pos = readingFrame; pos < sequence.Count - 2; pos += 3)
            {
                if (gapSymbols.Contains(sequence[pos]) || gapSymbols.Contains(sequence[pos + 1]) || gapSymbols.Contains(sequence[pos + 2]))
                {
                    if (isMissing)
                    {
                        sequences.Add(new List<byte>() { AmbiguousProteinAlphabet.Instance.X });
                    }
                    else
                    {
                        sequences.Add(new List<byte>() { ProteinAlphabet.Instance.Gap });
                    }
                }
                else
                {
                    if (alphabet == Alphabets.DNA)
                    {
                        sequences.Add(GetAminoAcidsUsingAmbigiousDNACodons(sequence[pos], sequence[pos + 1], sequence[pos + 2]));
                    }
                    else
                    {
                        sequences.Add(GetAminoAcidsUsingAmbigiousRNACodons(sequence[pos], sequence[pos + 1], sequence[pos + 2]));
                    }
                }
            }

            return sequences;
        }

        /// <summary>
        /// Get Amino Acids Using Ambiguous RNA Codons.
        /// </summary>
        /// <param name="firstPos">First position.</param>
        /// <param name="secondPos">Second position.</param>
        /// <param name="thirdPos">Third position.</param>
        /// <returns>Returns list of bytes.</returns>
        private static IList<byte> GetAminoAcidsUsingAmbigiousRNACodons(byte firstPos, byte secondPos, byte thirdPos)
        {
            IList<byte> aminoAcids = new List<byte>();
            HashSet<byte> firstBasicSymbols;
            RnaAlphabet.Instance.TryGetBasicSymbols(firstPos, out firstBasicSymbols);
            HashSet<byte> secondBasicSymbols;
            RnaAlphabet.Instance.TryGetBasicSymbols(secondPos, out secondBasicSymbols);
            HashSet<byte> thirdBasicSymbols;
            RnaAlphabet.Instance.TryGetBasicSymbols(thirdPos, out thirdBasicSymbols);
            
            foreach (byte firstPosAlphabet in firstBasicSymbols)
            {
                foreach (byte secondPosAlphabet in secondBasicSymbols)
                {
                    foreach (byte thirdPosAlphabet in thirdBasicSymbols)
                    {
                        aminoAcids.Add(Codons.Lookup(firstPosAlphabet, secondPosAlphabet, thirdPosAlphabet));
                    }
                }
            }

            return aminoAcids;
        }

        /// <summary>
        /// Get Amino Acids Using Ambiguous DNA Codons.
        /// </summary>
        /// <param name="firstPos">First position.</param>
        /// <param name="secondPos">Second position.</param>
        /// <param name="thirdPos">Third position.</param>
        /// <returns>Returns list of bytes.</returns>
        private static IList<byte> GetAminoAcidsUsingAmbigiousDNACodons(byte firstPos, byte secondPos, byte thirdPos)
        {
            IList<byte> aminoAcids = new List<byte>();
            HashSet<byte> firstBasicSymbols;
            DnaAlphabet.Instance.TryGetBasicSymbols(firstPos, out firstBasicSymbols);
            HashSet<byte> secondBasicSymbols;
            DnaAlphabet.Instance.TryGetBasicSymbols(secondPos, out secondBasicSymbols);
            HashSet<byte> thirdBasicSymbols;
            DnaAlphabet.Instance.TryGetBasicSymbols(thirdPos, out thirdBasicSymbols);

            foreach (byte firstPosAlphabet in firstBasicSymbols)
            {
                byte firstPosNucleotide = Transcription.GetRnaComplement(firstPosAlphabet);
                foreach (byte secondPosAlphabet in secondBasicSymbols)
                {
                    byte secondPosNucleotide = Transcription.GetRnaComplement(secondPosAlphabet);
                    foreach (byte thirdPosAlphabet in thirdBasicSymbols)
                    {
                        aminoAcids.Add(Codons.Lookup(firstPosNucleotide, secondPosNucleotide, Transcription.GetRnaComplement(thirdPosAlphabet)));
                    }
                }
            }

            return aminoAcids;
        }

        /// <summary>
        /// Try Parse Sequence File.
        /// </summary>
        /// <param name="fileName">The fileName.</param>
        /// <param name="alphabet">The Alphabet.</param>
        /// <param name="sequences">List of sequences.</param>
        /// <returns>True if Parsed sequence properly.</returns>
        private static bool TryParseSequenceFile(string fileName, IAlphabet alphabet, out IList<ISequence> sequences)
        {
            // Read the first line.
            string firstLine;
            using (var reader = new StreamReader(fileName))
            {
                firstLine = reader.ReadLine();
            }

            // Attempt to discover the format.
            if (ValidateFastaFormat(firstLine))
            {
                var parse = new FastAParser { Alphabet = alphabet };
                using (parse.Open(fileName))
                {
                    sequences = parse.Parse().ToList();
                }

                return true;
            }

            if (ValidatePhylipFormat(firstLine))
            {
                var parse = new PhylipParser { Alphabet = alphabet };
                using (parse.Open(fileName))
                {
                    sequences = ConvertAlignedSequenceToSequence(parse.Parse());
                }

                return true;
            }

            if (ValidateTabFormat(firstLine))
            {
                var parse = new FieldTextFileParser { Alphabet = alphabet };
                using (parse.Open(fileName))
                {
                    sequences = parse.Parse().ToList();
                    return true;
                }
            }

            sequences = null;
            return false;

        }

        /// <summary>
        /// Conforms the file format using information in first line of file.
        /// </summary>
        /// <param name="firstLine">First line of file.</param>
        /// <returns>Whether file is in phylip format or not.</returns>
        private static bool ValidatePhylipFormat(string firstLine)
        {
            return Regex.IsMatch(firstLine, @"^\s*\d+\s+\d+\s*$");
        }

        /// <summary>
        /// Conforms the file format using information in first line of file.
        /// </summary>
        /// <param name="firstLine">First line of file.</param>
        /// <returns>Whether file is in tab format or not.</returns>
        private static bool ValidateTabFormat(string firstLine)
        {
            return firstLine.Split('\t').Length == 2;
        }

        /// <summary>
        /// Conforms the file format using information in first line of file.
        /// </summary>
        /// <param name="firstLine">First line of file.</param>
        /// <returns>Whether file is in fasta format or not.</returns>
        private static bool ValidateFastaFormat(string firstLine)
        {
            return firstLine.StartsWith(">", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Converts Aligned Sequences to List of sequences.
        /// </summary>
        /// <param name="alignment">List of aligned sequences.</param>
        /// <returns>List of sequences.</returns>
        private static IList<ISequence> ConvertAlignedSequenceToSequence(IEnumerable<ISequenceAlignment> alignment)
        {
            if (alignment == null)
            {
                throw new ArgumentNullException("alignment");
            }

            var firstAlignment = alignment.FirstOrDefault();

            if (firstAlignment == null
                || firstAlignment.AlignedSequences.Count == 0)
            {
                throw new ArgumentException(Resource.AlignedSequenceCount);
            }

            return firstAlignment.AlignedSequences[0].Sequences;
        }

        #endregion
    }
}
