using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Bio;
using Bio.Algorithms.Assembly.Comparative;
using Bio.IO;
using Bio.IO.FastA;
using Bio.Util;
using ComparativeUtil.Properties;

namespace ComparativeUtil
{
    /// <summary>
    /// Class for Assemble options.
    /// </summary>
    internal class AssembleArguments
    {
        /// <summary>
        /// Paths of input and output file.
        /// </summary>
        public string[] FilePath = null;

        /// <summary>
        /// Use anchor matches that are unique in both the reference and query.
        /// </summary>
        public int KmerLength = 10;

        /// <summary>
        /// Run scaffolding step after generating contigs
        /// </summary>
        public bool Scaffold = false;

        /// <summary>
        /// Minimum length of MUM.
        /// </summary>
        public int MumLength = 20;

        /// <summary>
        /// Print the help information.
        /// </summary>
        public bool Help = false;

        /// <summary>
        /// Output file.
        /// </summary>
        public string OutputFile = null;

        /// <summary>
        /// Clone Library Name.
        /// </summary>
        public string CloneLibraryName = string.Empty;

        /// <summary>
        /// Mean Length of clone library.
        /// </summary>
        public double MeanLengthOfInsert = 0;

        /// <summary>
        /// Standard Deviation of Clone Library.
        /// </summary>
        public double StandardDeviationOfInsert = 0;

        /// <summary>
        /// Display verbose logging during processing.
        /// </summary>
        public bool Verbose = false;

        /// <summary>
        /// It assembles the sequences.
        /// </summary>
        public virtual void AssembleSequences()
        {
            if (this.FilePath.Length != 2)
            {
                Output.WriteLine(OutputLevel.Error, "Error: A reference file and 1 query file are required.");
                return;
            }

            TimeSpan timeSpan = new TimeSpan();
            Stopwatch runAlgorithm = new Stopwatch();
            FileInfo inputFileinfo = new FileInfo(this.FilePath[0]);
            long inputFileLength = inputFileinfo.Length;
            inputFileinfo = null;

            if (!string.IsNullOrEmpty(this.CloneLibraryName))
            {
                CloneLibrary.Instance.AddLibrary(this.CloneLibraryName, (float)this.MeanLengthOfInsert, (float)this.StandardDeviationOfInsert);
            }

            runAlgorithm.Restart();

            // Parse input files
            IEnumerable<ISequence> referenceSequences = ParseFile(this.FilePath[0]);
            runAlgorithm.Stop();

            if (this.Verbose)
            {
                Output.WriteLine(OutputLevel.Verbose);
                Output.WriteLine(OutputLevel.Verbose, "Processed reference file: {0}", Path.GetFullPath(this.FilePath[0]));
                Output.WriteLine(OutputLevel.Verbose, "   Read/Processing time : {0}", runAlgorithm.Elapsed);
                Output.WriteLine(OutputLevel.Verbose, "   File Size            : {0}", inputFileLength);
            }

            inputFileinfo = new FileInfo(this.FilePath[1]);
            inputFileLength = inputFileinfo.Length;
            runAlgorithm.Restart();
            FastASequencePositionParser queryParser;
            using (var stream = File.OpenRead(this.FilePath[1]))
            {
                // Parse and cache the sequences.
                queryParser = new FastASequencePositionParser(stream, true);
                queryParser.CacheSequencesForRandomAccess();
            }

            // Check the input
            var reads = queryParser.Parse();
            if (reads.Any(s => s.Alphabet.HasAmbiguity))
                throw new ArgumentException(Resources.AmbiguousReadsNotSupported);

            runAlgorithm.Stop();

            if (this.Verbose)
            {
                Output.WriteLine(OutputLevel.Verbose);
                Output.WriteLine(OutputLevel.Verbose, "Processed reads file   : {0}", Path.GetFullPath(this.FilePath[1]));
                Output.WriteLine(OutputLevel.Verbose, "   Read/Processing time: {0}", runAlgorithm.Elapsed);
                Output.WriteLine(OutputLevel.Verbose, "   File Size           : {0}", inputFileLength);
            }

            runAlgorithm.Restart();


            runAlgorithm.Stop();

            if (this.Verbose)
            {
                Output.WriteLine(OutputLevel.Verbose);
                Output.WriteLine(OutputLevel.Verbose, "Time taken for Validating reads: {0}", runAlgorithm.Elapsed);
                Output.WriteLine(OutputLevel.Verbose);
            }

            runAlgorithm.Restart();
            ComparativeGenomeAssembler assembler = new ComparativeGenomeAssembler();
            assembler.StatusChanged += this.AssemblerStatusChanged;
            assembler.ScaffoldingEnabled = this.Scaffold;
            assembler.KmerLength = this.KmerLength;
            assembler.LengthOfMum = this.MumLength;
            IEnumerable<ISequence> assemblerResult = assembler.Assemble(referenceSequences, queryParser);
            runAlgorithm.Stop();
            timeSpan = timeSpan.Add(runAlgorithm.Elapsed);

            runAlgorithm.Restart();

            this.WriteContigs(assemblerResult);
            runAlgorithm.Stop();

            if (this.Verbose)
            {
                Output.WriteLine(OutputLevel.Verbose, "Assemble time: {0}", timeSpan);
                Output.WriteLine(OutputLevel.Verbose, "Write time: {0}", runAlgorithm.Elapsed);
            }
        }

        /// <summary>
        /// Helper method to parse the given filename into a set
        /// of ISequence elements. This routine will load sequences from
        /// any support sequence parser in .NET Bio.
        /// </summary>
        /// <param name="fileName">Filename to load data from</param>
        /// <returns>Enumerable set of ISequence elements</returns>
        protected static IEnumerable<ISequence> ParseFile(string fileName)
        {
            ISequenceParser parser = SequenceParsers.FindParserByFileName(fileName);
            if (parser == null)
                throw new Exception("Could not locate an appropriate sequence parser for " + fileName);

            return parser.Parse();
        }

        /// <summary>
        /// Generates a sequence Id using the output filename, or first input file.
        /// </summary>
        /// <param name="counter">Sequence counter</param>
        /// <returns>Auto-generated sequence id</returns>
        private string GenerateSequenceId(int counter)
        {
            string filename = Path.GetFileNameWithoutExtension(this.OutputFile);
            if (string.IsNullOrEmpty(filename))
                filename = Path.GetFileNameWithoutExtension(this.FilePath[0]);
            filename = filename.Replace(" ", "");
            Debug.Assert(!string.IsNullOrEmpty(filename));
            return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", filename, counter);
        }

        /// <summary>
        /// It Writes the contigs to the file.
        /// </summary>
        /// <param name="assembly">IDeNovoAssembly parameter is the result of running De Novo Assembly on a set of two or more sequences. </param>
        protected void WriteContigs(IEnumerable<ISequence> assembly)
        {
            int counter = 1;
            if (!string.IsNullOrEmpty(this.OutputFile))
            {
                FastAFormatter formatter = new FastAFormatter { AutoFlush = true };
                using (formatter.Open(this.OutputFile))
                {
                    foreach (ISequence seq in assembly)
                    {
                        if (string.IsNullOrEmpty(seq.ID))
                            seq.ID = GenerateSequenceId(counter);
                        formatter.Format(seq);
                        counter++;
                    }
                }
                Output.WriteLine(OutputLevel.Information, Resources.OutPutWrittenToFileSpecified);
            }
            else
            {
                Output.WriteLine(OutputLevel.Information, "Assembled Sequence Results:");
                foreach (ISequence seq in assembly)
                {
                    if (string.IsNullOrEmpty(seq.ID))
                        seq.ID = GenerateSequenceId(counter);
                    Output.WriteLine(OutputLevel.Results, seq.ID);
                    Output.WriteLine(OutputLevel.Results, new string(seq.Select(a => (char)a).ToArray()));
                    counter++;
                }
            }
        }

        /// <summary>
        /// Method to handle status changed event.
        /// </summary>
        protected void AssemblerStatusChanged(string statusMessage)
        {
            if (Verbose)
                Output.WriteLine(OutputLevel.Verbose, statusMessage);
            else
            {
                if (statusMessage.StartsWith("Step", StringComparison.OrdinalIgnoreCase)
                    && statusMessage.Contains("Start"))
                {
                    int pos = statusMessage.IndexOf(" - ", StringComparison.OrdinalIgnoreCase);
                    Output.WriteLine(OutputLevel.Information, statusMessage.Substring(0, pos));
                }
            }
        }
    }
}
