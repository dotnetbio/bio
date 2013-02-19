using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Bio;
using Bio.IO;
using Bio.IO.FastA;

namespace ReadSimulator
{
    /// <summary>
    /// Provides the glue logic between the UI and the data model
    /// </summary>
    public class SimulatorController
    {
        private readonly SimulatorSettings simulatorSettings = new SimulatorSettings();

        /// <summary>
        /// The loaded in sequence to be split
        /// </summary>
        public ISequence SequenceToSplit { get; set; }

        /// <summary>
        /// Currently held data settings for simulation runs.
        /// </summary>
        public SimulatorSettings Settings
        {
            get { return simulatorSettings; }
        }

        /// <summary>
        /// Parses a sequence given a file name. Uses built in mechanisms to detect the
        /// appropriate parser based on the file name.
        /// </summary>
        /// <param name="fileName">The path of the file to be parsed for a sequence</param>
        public void ParseSequence(string fileName)
        {
            ISequenceParser parser = SequenceParsers.FindParserByFileName(fileName);
            if (parser == null)
                throw new ArgumentException("Could not find an appropriate parser for " + fileName);

            // Get the first sequence from the file
            SequenceToSplit = parser.Parse().FirstOrDefault();
            parser.Close();

            if (SequenceToSplit == null)
                throw new ArgumentException("Unable to parse a sequence from file " + fileName);
        }

        /// <summary>
        /// Does the logic behind the sequence simulation
        /// </summary>
        public void DoSimulation(string outputFileName, Action<long,long> updateSimulationStats, Action<string> simulationComplete)
        {
            const string filePostfix = "_{0}.fa";

            FileInfo file = new FileInfo(outputFileName);
            if (!file.Directory.Exists)
                throw new ArgumentException("Could not write to the output directory for " + outputFileName);

            if (simulatorSettings.OutputSequenceCount <= 0)
                throw new ArgumentException("'Max Output Sequences Per File' should be greater than zero.");

            if (simulatorSettings.SequenceLength <= 0)
                throw new ArgumentException("'Mean Output Length' should be greater than zero.");

            string filePrefix;
            if (String.IsNullOrEmpty(file.Extension))
                filePrefix = file.FullName;
            else
                filePrefix = file.FullName.Substring(0, file.FullName.IndexOf(file.Extension));

            long seqCount = (simulatorSettings.DepthOfCoverage * SequenceToSplit.Count) / simulatorSettings.SequenceLength;
            long fileCount = seqCount / simulatorSettings.OutputSequenceCount;
            if (seqCount % simulatorSettings.OutputSequenceCount != 0)
                fileCount++;

            // Update the UI
            updateSimulationStats(seqCount, fileCount);

            int fileIndex = 1;
            FastAFormatter formatter = null;
            List<ISequence> generatedSequenceList = new List<ISequence>();

            for (long i = 0; i < seqCount; i++)
            {
                generatedSequenceList.Add(CreateSubsequence(i, SequenceToSplit, Settings));
                if (generatedSequenceList.Count >= simulatorSettings.OutputSequenceCount)
                {
                    FileInfo outFile = new FileInfo(filePrefix + string.Format(filePostfix, fileIndex++));
                    formatter = new FastAFormatter(outFile.FullName);
                    foreach (ISequence seq in generatedSequenceList)
                    {
                        formatter.Write(seq);
                    }
                    formatter.Close();
                    generatedSequenceList.Clear();
                }
            }

            // Pick off any remaining sequences into the final file.
            if (generatedSequenceList.Count > 0)
            {
                FileInfo outFile = new FileInfo(filePrefix + string.Format(filePostfix, fileIndex++));
                formatter = new FastAFormatter(outFile.FullName);
                foreach (ISequence seq in generatedSequenceList)
                {
                    formatter.Write(seq);
                }
                formatter.Close();
                simulationComplete(formatter.Name);
            }

            // Either we ended exactly on the boundary with no additional sequences
            // generated, OR we never generated any files.
            else
            {
                simulationComplete(formatter != null ? formatter.Name : string.Empty);
            }
        }

        /// <summary>
        /// A random number generator used to create random
        /// starting locations in a sequence.
        /// </summary>
        private static Random seqRandom = new Random();

        /// <summary>
        /// Creates a subsequence from a source sequence given the settings provided
        /// </summary>
        /// <param name="index"></param>
        /// <param name="SequenceToSplit"></param>
        /// <param name="simulatorSettings"></param>
        /// <returns></returns>
        private static ISequence CreateSubsequence(long index, ISequence SequenceToSplit, SimulatorSettings simulatorSettings)
        {
            
            double err = (double)simulatorSettings.ErrorFrequency;

            // Set the length using the appropriate random number distribution type
            long subLength = simulatorSettings.SequenceLength;
            if (simulatorSettings.DistributionType == (int)Distribution.Uniform)
            {
                subLength += seqRandom.Next(simulatorSettings.LengthVariation * 2) - simulatorSettings.LengthVariation;
            }
            else if (simulatorSettings.DistributionType == (int)Distribution.Normal)
            {
                subLength = (long)Math.Floor(Bio.Util.Helper.GetNormalRandom((double)simulatorSettings.SequenceLength,
                    (double)simulatorSettings.LengthVariation));
            }

            // Quick sanity checks on the length of the subsequence
            if (subLength <= 0)
                subLength = 1;

            if (subLength > SequenceToSplit.Count)
                subLength = SequenceToSplit.Count;

            // Set the start position
            long startPosition = (long)Math.Floor(seqRandom.NextDouble() * (SequenceToSplit.Count - subLength));
            byte[] sequenceBytes = new byte[subLength];
            IAlphabet resultSequenceAlphabet = SequenceToSplit.Alphabet;

            // Get ambiguity symbols
            List<byte> errorSource = null;
            //= Sequence.Alphabet.LookupAll(true, false, settings.AllowAmbiguities, false);
            if (simulatorSettings.AllowAmbiguities &&
                (SequenceToSplit.Alphabet == DnaAlphabet.Instance || SequenceToSplit.Alphabet == RnaAlphabet.Instance 
                    || SequenceToSplit.Alphabet == ProteinAlphabet.Instance))
            {
                resultSequenceAlphabet = Alphabets.AmbiguousAlphabetMap[SequenceToSplit.Alphabet];
            }

            errorSource = resultSequenceAlphabet.GetValidSymbols().ToList();
            
            // remove gap and termination symbol
            HashSet<byte> gaps, terminations;
            SequenceToSplit.Alphabet.TryGetGapSymbols(out gaps);
            SequenceToSplit.Alphabet.TryGetTerminationSymbols(out terminations);

            if (gaps != null)
                errorSource.RemoveAll(a => gaps.Contains(a));
            if (terminations != null)
                errorSource.RemoveAll(a => terminations.Contains(a));

            for (long i = 0; i < subLength; i++)
            {
                // Apply Errors if applicable
                if (seqRandom.NextDouble() < err)
                {
                    sequenceBytes[i] = errorSource[seqRandom.Next(errorSource.Count - 1)];
                }
                else
                {
                    sequenceBytes[i] = SequenceToSplit[startPosition + i];
                }
            }

            Sequence generatedSequence = new Sequence(resultSequenceAlphabet, sequenceBytes.ToArray());
            generatedSequence.ID = SequenceToSplit.ID + " (Split " + (index + 1) + ", " + generatedSequence.Count + "bp)";

            // Reverse Sequence if applicable
            if (simulatorSettings.ReverseHalf && seqRandom.NextDouble() < 0.5f)
            {
                 return new DerivedSequence(generatedSequence, true,true);
            }

            return generatedSequence;
        }

        /// <summary>
        /// This method will query the Framework abstraction
        /// to figure out the parsers supported by the framwork.
        /// </summary>
        /// <returns>List of all parsers and the file extensions the parsers support.</returns>
        public IEnumerable<string> QuerySupportedFileType()
        {
            return SequenceParsers.All
                .Select(parser => parser.Name + "|" + parser.SupportedFileTypes.Replace(".", "*.").Replace(',', ';'));
        }
    }
}
