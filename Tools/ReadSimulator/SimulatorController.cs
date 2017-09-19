using System;
using System.Collections.Generic;
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
        /// <summary>
        /// A random number generator used to create random
        /// starting locations in a sequence.
        /// </summary>
        private readonly Random _seqRandom;

        /// <summary>
        /// The loaded in sequence to be split
        /// </summary>
        public ISequence SequenceToSplit { get; set; }

        /// <summary>
        /// Currently held data settings for simulation runs.
        /// </summary>
        public SimulatorSettings Settings { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public SimulatorController()
        {
            _seqRandom = new Random();
            Settings = new SimulatorSettings();
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

            using (parser.Open(fileName))
            {
                // Get the first sequence from the file
                SequenceToSplit = parser.ParseOne();
                if (SequenceToSplit == null)
                    throw new ArgumentException("Unable to parse a sequence from file " + fileName);
            }
        }

        /// <summary>
        /// Does the logic behind the sequence simulation
        /// </summary>
        public void DoSimulation(string outputFileName, Action<long,long> updateSimulationStats, Action<string> simulationComplete)
        {
            const string filePostfix = "_{0}.fa";

            FileInfo file = new FileInfo(outputFileName);
            if (file.Directory == null || !file.Directory.Exists)
                throw new ArgumentException("Could not write to the output directory for " + outputFileName);

            if (Settings.OutputSequenceCount <= 0)
                throw new ArgumentException("'Max Output Sequences Per File' should be greater than zero.");

            if (Settings.SequenceLength <= 0)
                throw new ArgumentException("'Mean Output Length' should be greater than zero.");

            string filePrefix = String.IsNullOrEmpty(file.Extension) ? file.FullName : file.FullName.Substring(0, file.FullName.IndexOf(file.Extension));

            long seqCount = (Settings.DepthOfCoverage * SequenceToSplit.Count) / Settings.SequenceLength;
            long fileCount = seqCount / Settings.OutputSequenceCount;
            if (seqCount % Settings.OutputSequenceCount != 0)
                fileCount++;

            // Update the UI
            updateSimulationStats(seqCount, fileCount);

            int fileIndex = 1;
            FastAFormatter formatter = null;
            List<ISequence> generatedSequenceList = new List<ISequence>();

            for (long i = 0; i < seqCount; i++)
            {
                generatedSequenceList.Add(CreateSubsequence(i, SequenceToSplit, Settings));
                if (generatedSequenceList.Count >= Settings.OutputSequenceCount)
                {
                    FileInfo outFile = new FileInfo(filePrefix + string.Format(filePostfix, fileIndex++));
                    formatter = new FastAFormatter();
                    using (formatter.Open(outFile.FullName))
                    {
                        formatter.Format(generatedSequenceList);
                    }
                    generatedSequenceList.Clear();
                }
            }

            // Pick off any remaining sequences into the final file.
            if (generatedSequenceList.Count > 0)
            {
                FileInfo outFile = new FileInfo(filePrefix + string.Format(filePostfix, fileIndex++));
                formatter = new FastAFormatter();
                using (formatter.Open(outFile.FullName))
                {
                    formatter.Format(generatedSequenceList);
                }
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
        /// Creates a subsequence from a source sequence given the settings provided
        /// </summary>
        /// <param name="index"></param>
        /// <param name="sequenceToSplit"></param>
        /// <param name="simulatorSettings"></param>
        /// <returns></returns>
        private ISequence CreateSubsequence(long index, ISequence sequenceToSplit, SimulatorSettings simulatorSettings)
        {
            double err = simulatorSettings.ErrorFrequency;

            // Set the length using the appropriate random number distribution type
            long subLength = simulatorSettings.SequenceLength;
            switch (simulatorSettings.DistributionType)
            {
                case (int)Distribution.Uniform:
                    subLength += _seqRandom.Next(simulatorSettings.LengthVariation * 2) - simulatorSettings.LengthVariation;
                    break;
                case (int)Distribution.Normal:
                    subLength = (long)Math.Floor(Bio.Util.Helper.GetNormalRandom(simulatorSettings.SequenceLength, simulatorSettings.LengthVariation));
                    break;
            }

            // Quick sanity checks on the length of the subsequence
            if (subLength <= 0)
                subLength = 1;

            if (subLength > sequenceToSplit.Count)
                subLength = sequenceToSplit.Count;

            // Set the start position
            long startPosition = (long)Math.Floor(_seqRandom.NextDouble() * (sequenceToSplit.Count - subLength));
            byte[] sequenceBytes = new byte[subLength];
            IAlphabet resultSequenceAlphabet = sequenceToSplit.Alphabet;

            // Get ambiguity symbols
            if (simulatorSettings.AllowAmbiguities &&
                (sequenceToSplit.Alphabet == DnaAlphabet.Instance || sequenceToSplit.Alphabet == RnaAlphabet.Instance 
                    || sequenceToSplit.Alphabet == ProteinAlphabet.Instance))
            {
                resultSequenceAlphabet = Alphabets.AmbiguousAlphabetMap[sequenceToSplit.Alphabet];
            }

            List<byte> errorSource = resultSequenceAlphabet.GetValidSymbols().ToList();
            
            // remove gap and termination symbol
            HashSet<byte> gaps, terminations;
            sequenceToSplit.Alphabet.TryGetGapSymbols(out gaps);
            sequenceToSplit.Alphabet.TryGetTerminationSymbols(out terminations);

            if (gaps != null)
                errorSource.RemoveAll(a => gaps.Contains(a));
            if (terminations != null)
                errorSource.RemoveAll(a => terminations.Contains(a));

            for (long i = 0; i < subLength; i++)
            {
                // Apply Errors if applicable
                sequenceBytes[i] = _seqRandom.NextDouble() < err
                                       ? errorSource[_seqRandom.Next(errorSource.Count - 1)]
                                       : sequenceToSplit[startPosition + i];
            }

            ISequence generatedSequence = new Sequence(resultSequenceAlphabet, sequenceBytes.ToArray());
            generatedSequence.ID = sequenceToSplit.ID + " (Split " + (index + 1) + ", " + generatedSequence.Count + "bp)";

            // Reverse Sequence if applicable
            return simulatorSettings.ReverseHalf && _seqRandom.NextDouble() < 0.5f
                       ? new DerivedSequence(generatedSequence, true, true)
                       : generatedSequence;
        }

        /// <summary>
        /// This method will query the Framework abstraction
        /// to figure out the parsers supported by the framework.
        /// </summary>
        /// <returns>List of all parsers and the file extensions the parsers support.</returns>
        public IEnumerable<string> QuerySupportedFileType()
        {
            return SequenceParsers.All
                .Select(parser => parser.Name + "|" + parser.SupportedFileTypes.Replace(".", "*.").Replace(',', ';'));
        }
    }
}
