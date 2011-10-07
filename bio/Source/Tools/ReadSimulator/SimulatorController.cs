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
    internal class SimulatorController
    {
        private ISequence sequence;
        private List<ISequence> generatedSequenceList;
        private Random random = new Random();

        /// <summary>
        /// The loaded in sequence to be split
        /// </summary>
        internal ISequence SequenceToSplit
        {
            get { return sequence; }
            set { sequence = value; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        internal SimulatorController()
        {

        }

        /// <summary>
        /// Parses a sequence given a file name. Uses built in mechanisms to detect the
        /// appropriate parser based on the file name.
        /// </summary>
        /// <param name="fileName">The path of the file to be parsed for a sequence</param>
        internal void ParseSequence(string fileName)
        {
            ISequenceParser parser = SequenceParsers.FindParserByFileName(fileName);
            if (parser == null)
                throw new ArgumentException("Could not find an appropriate parser for " + fileName);
            
            IEnumerable<ISequence> sequences = parser.Parse();
            if (sequences == null)
                throw new ArgumentException("Unable to parse a sequence from file " + fileName);
            
            SequenceToSplit = sequences.ElementAt(0);
            parser.Close();
        }

        /// <summary>
        /// Does the logic behind the sequence simulation
        /// </summary>
        internal void DoSimulation(SimulatorWindow window, string outputFileName, SimulatorSettings settings)
        {
            FileInfo file = new FileInfo(outputFileName);
            if (!file.Directory.Exists)
                throw new ArgumentException("Could not write to the output directory for " + outputFileName);

            if(settings.OutputSequenceCount <=0)
                throw new ArgumentException("'Max Output Sequences Per File' should be greater than zero.");

            if (settings.SequenceLength <= 0)
                throw new ArgumentException("'Mean Output Length' should be greater than zero.");

            string filePrefix;
            if (String.IsNullOrEmpty(file.Extension))
                filePrefix = file.FullName;
            else
                filePrefix = file.FullName.Substring(0, file.FullName.IndexOf(file.Extension));

            string filePostfix = "_{0}.fa";

            long seqCount = (settings.DepthOfCoverage * SequenceToSplit.Count) / settings.SequenceLength;
            long fileCount = seqCount / settings.OutputSequenceCount;
            if (seqCount % settings.OutputSequenceCount!= 0)
                fileCount++;

            window.UpdateSimulationStats(seqCount, fileCount);

            if (generatedSequenceList == null)
                generatedSequenceList = new List<ISequence>();
            else
                generatedSequenceList.Clear();

            int fileIndex = 1;
            FastAFormatter formatter = null;

            for (long i = 0; i < seqCount; i++)
            {
                generatedSequenceList.Add(CreateSubsequence(settings, i));

                if (generatedSequenceList.Count >= settings.OutputSequenceCount)
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

            if (generatedSequenceList.Count > 0)
            {
                FileInfo outFile = new FileInfo(filePrefix + string.Format(filePostfix, fileIndex++));
                formatter = new FastAFormatter(outFile.FullName);
                foreach (ISequence seq in generatedSequenceList)
                {
                    formatter.Write(seq);
                }
                formatter.Close();
                window.NotifySimulationComplete(formatter.Name);
            }
            else
            {
                window.NotifySimulationComplete(string.Empty);
            }
            
        }

        // Creates a subsequence from a source sequence given the settings provided
        private ISequence CreateSubsequence(SimulatorSettings settings, long index)
        {
            double err = (double)settings.ErrorFrequency;

            // Set the length using the appropriate random number distribution type
            long subLength = settings.SequenceLength;
            if (settings.DistributionType == (int)Distribution.Uniform)
            {
                subLength += random.Next(settings.LengthVariation * 2) - settings.LengthVariation;
            }
            else if (settings.DistributionType == (int)Distribution.Normal)
            {
                subLength = (long)Math.Floor(Bio.Util.Helper.GetNormalRandom((double)settings.SequenceLength,
                    (double)settings.LengthVariation));
            }

            // Quick sanity checks on the length of the subsequence
            if (subLength <= 0)
                subLength = 1;

            if (subLength > SequenceToSplit.Count)
                subLength = SequenceToSplit.Count;

            // Set the start position
            long startPosition = (long)Math.Floor(random.NextDouble() * (SequenceToSplit.Count - subLength));

            byte[] sequenceBytes = new byte[subLength];
            IAlphabet resultSequenceAlphabet = SequenceToSplit.Alphabet;

            // Get ambiguity symbols
            List<byte> errorSource = null;
            //= Sequence.Alphabet.LookupAll(true, false, settings.AllowAmbiguities, false);
            if (settings.AllowAmbiguities &&
                (SequenceToSplit.Alphabet == DnaAlphabet.Instance || SequenceToSplit.Alphabet == RnaAlphabet.Instance || SequenceToSplit.Alphabet == ProteinAlphabet.Instance)
               )
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
                if (random.NextDouble() < err)
                {
                    sequenceBytes[i] = errorSource[random.Next(errorSource.Count - 1)];
                }
                else
                {
                    sequenceBytes[i] = SequenceToSplit[startPosition + i];
                }
            }

            Sequence generatedSequence = new Sequence(resultSequenceAlphabet, sequenceBytes.ToArray());
            generatedSequence.ID = SequenceToSplit.ID + " (Split " + (index + 1) + ", " + generatedSequence.Count + "bp)";

            // Reverse Sequence if applicable
            if (settings.ReverseHalf && random.NextDouble() < 0.5f)
            {
                return new DerivedSequence(generatedSequence, true, false);
            }

            return generatedSequence;
        }

        /// <summary>
        /// This method will query the Framework abstraction
        /// to figure out the parsers supported by the framwork.
        /// </summary>
        /// <returns>List of all parsers and the file extensions the parsers support.</returns>
        internal Collection<string> QuerySupportedFileType()
        {
            Collection<string> fileExtensions = new Collection<string>();
            foreach (ISequenceParser parser in SequenceParsers.All)
            {
                // Add to filters collection after formatting it properly to user as a filter for FileDialogs
                fileExtensions.Add(parser.Name + "|" + parser.SupportedFileTypes.Replace(".", "*.").Replace(',', ';'));
            }

            return fileExtensions;
        }
    }
}
