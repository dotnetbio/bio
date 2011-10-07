using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bio;
using Bio.Algorithms.Assembly.Comparative;
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
        #region Public Fields

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
        #endregion

        #region Public methods

        /// <summary>
        /// It assembles the sequences.
        /// </summary>
        public virtual void AssembleSequences()
        {
            if (this.FilePath.Length != 2)
            {
                Console.Error.WriteLine("\nError: A reference file and 1 query file are required.");
                Environment.Exit(-1);
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
            IEnumerable<ISequence> referenceSequences = new FastAParser(this.FilePath[0]).Parse();
            runAlgorithm.Stop();

            if (this.Verbose)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("  Processed reference file: {0}", Path.GetFullPath(this.FilePath[0]));
                Console.Error.WriteLine("            Read/Processing time: {0}", runAlgorithm.Elapsed);
                Console.Error.WriteLine("            File Size           : {0}", inputFileLength);
            }

            inputFileinfo = new FileInfo(this.FilePath[1]);
            inputFileLength = inputFileinfo.Length;
            runAlgorithm.Restart();
            FastASequencePositionParser queryParser = new FastASequencePositionParser(this.FilePath[1], true);
            queryParser.CacheSequencesForRandomAccess();
            IEnumerable<ISequence> reads = queryParser.Parse();
            runAlgorithm.Stop();

            if (this.Verbose)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("  Processed reads file: {0}", Path.GetFullPath(this.FilePath[1]));
                Console.Error.WriteLine("            Read/Processing time: {0}", runAlgorithm.Elapsed);
                Console.Error.WriteLine("            File Size           : {0}", inputFileLength);
            }

            runAlgorithm.Restart();
            ValidateAmbiguousReads(reads);
            runAlgorithm.Stop();

            if (this.Verbose)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("  Time taken for Validating reads: {0}", runAlgorithm.Elapsed);
            }

            runAlgorithm.Restart();
            ComparativeGenomeAssembler assembler = new ComparativeGenomeAssembler();
            assembler.StatusChanged += new EventHandler<StatusChangedEventArgs>(this.AssemblerStatusChanged);
            assembler.ScaffoldingEnabled = this.Scaffold;
            assembler.KmerLength = this.KmerLength;
            assembler.LengthOfMum = this.MumLength;
            IEnumerable<ISequence> assemblerResult = assembler.Assemble(referenceSequences, queryParser);
            runAlgorithm.Stop();
            timeSpan = timeSpan.Add(runAlgorithm.Elapsed);

            runAlgorithm.Restart();

            if (this.OutputFile == null)
            {
                // Write output to console.
                this.WriteContigs(assemblerResult, Console.Out);
            }
            else
            {
                // Write output to the specified file.
                this.WriteContigs(assemblerResult, null);
                Console.WriteLine(Resources.OutPutWrittenToFileSpecified);
            }
            runAlgorithm.Stop();

            if (this.Verbose)
            {
                Console.Error.WriteLine("  Assemble time: {0}", timeSpan);
                Console.Error.WriteLine("  Write() time: {0}", runAlgorithm.Elapsed);
            }
        }

        #endregion

        #region Protected Members

        /// <summary>
        /// It Writes the contigs to the file.
        /// </summary>
        /// <param name="assembly">IDeNovoAssembly parameter is the result of running De Novo Assembly on a set of two or more sequences. </param>
        /// <param name="outputWriter">A TextWriter to which the output will be written to.</param>
        protected void WriteContigs(IEnumerable<ISequence> assembly, TextWriter outputWriter)
        {
            if (!string.IsNullOrEmpty(this.OutputFile))
            {
                using (FastAFormatter formatter = new FastAFormatter(this.OutputFile))
                {
                    formatter.AutoFlush = true;

                    foreach (ISequence seq in assembly)
                    {
                        formatter.Write(seq);
                    }
                }
            }
            else
            {
                foreach (ISequence seq in assembly)
                {
                    outputWriter.WriteLine(seq.ID);
                    outputWriter.WriteLine(new string(seq.Select(a => (char)a).ToArray()));
                }
            }
        }

        /// <summary>
        /// Method to handle status changed event.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="statusEventArgs">Status message.</param>
        protected void AssemblerStatusChanged(object sender, StatusChangedEventArgs statusEventArgs)
        {
            Console.Error.Write(statusEventArgs.StatusMessage);
        }

        /// <summary>
        /// Checks for ambiguous reads if any, if found ArgumentException.
        /// </summary>
        /// <param name="reads">Input reads.</param>
        private static void ValidateAmbiguousReads(IEnumerable<ISequence> reads)
        {
            foreach (ISequence seq in reads)
            {
                if (seq.Alphabet.HasAmbiguity)
                {
                    throw new ArgumentException(Properties.Resources.AmbiguousReadsNotSupported);
                }
            }
        }
        #endregion
    }
}
