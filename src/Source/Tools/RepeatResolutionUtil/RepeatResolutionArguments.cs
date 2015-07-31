using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Bio.Algorithms.Alignment;
using Bio.Algorithms.Assembly.Comparative;
using Bio.Util;

namespace RepeatResolutionUtil
{
    /// <summary>
    /// Command line arguments for RepeatResolution.
    /// </summary>
    internal class RepeatResolutionArguments
    {
        #region Public Fields
        /// <summary>
        /// Unsorted delta file.
        /// </summary>
        private const string UnsortedDeltaFile = "DeltaAlignments_RepeatResolutionStepOutput_UnSorted.txt";

        /// <summary>
        /// Paths of deltaalignment and query file.
        /// </summary>
        public string[] FilePath = null;

        /// <summary>
        /// Print the help information.
        /// </summary>
        public bool Help = false;

        /// <summary>
        /// Output file.
        /// </summary>
        public string OutputFile = null;

        /// <summary>
        /// Display verbose logging during processing.
        /// </summary>
        public bool Verbose = false;

        #endregion

        #region Public Method

        /// <summary>
        /// Resolve ambiguity in the delta alignments.
        /// </summary>
        public void ResolveAmbiguity()
        {
            TimeSpan repeatResolutionSpan = new TimeSpan();
            Stopwatch runRepeatResolution = new Stopwatch();

            runRepeatResolution.Restart();
            FileInfo inputFileinfo = new FileInfo(this.FilePath[1]);
            long inputFileLength = inputFileinfo.Length;
            runRepeatResolution.Stop();

            if (this.Verbose)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("  Processed Query FastA file: {0}", Path.GetFullPath(this.FilePath[1]));
                Console.Error.WriteLine("            Read/Processing time: {0}", runRepeatResolution.Elapsed);
                Console.Error.WriteLine("            File Size           : {0}", inputFileLength);
            }

            inputFileinfo = new FileInfo(this.FilePath[0]);
            inputFileLength = inputFileinfo.Length;

            runRepeatResolution.Restart();
            DeltaAlignmentSorter sorter = null;
            using (var alignmentStream = File.OpenRead(this.FilePath[0]))
            using (var readStream = File.OpenRead(this.FilePath[1]))
            using (DeltaAlignmentCollection deltaCollection = new DeltaAlignmentCollection(alignmentStream, readStream))
            {
                runRepeatResolution.Stop();

                if (this.Verbose)
                {
                    Console.Error.WriteLine();
                    Console.Error.WriteLine("  Processed DeltaAlignment file: {0}", Path.GetFullPath(this.FilePath[0]));
                    Console.Error.WriteLine("            Read/Processing time: {0}", runRepeatResolution.Elapsed);
                    Console.Error.WriteLine("            File Size           : {0}", inputFileLength);
                }

                runRepeatResolution.Restart();
                IEnumerable<DeltaAlignment> outputDeltas = RepeatResolver.ResolveAmbiguity(deltaCollection);
                sorter = new DeltaAlignmentSorter();
                WriteUnsortedDelta(outputDeltas, sorter);
            }

            runRepeatResolution.Stop();
            repeatResolutionSpan = repeatResolutionSpan.Add(runRepeatResolution.Elapsed);
            runRepeatResolution.Restart();
            this.WriteDelta(sorter);
            runRepeatResolution.Stop();

            if (this.Verbose)
            {
                Console.Error.WriteLine("  Compute time: {0}", repeatResolutionSpan);
                Console.Error.WriteLine("  Write() time: {0}", runRepeatResolution.Elapsed);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Writes delta for query sequences.
        /// </summary>
        /// <param name="sorter">The Deltas.</param>
        private void WriteDelta(DeltaAlignmentSorter sorter)
        {
            TextWriter textWriterConsoleOutSave = Console.Out;
            StreamWriter streamWriterConsoleOut = null;

            try
            {
                using (var reads = File.OpenRead(this.FilePath[1]))
                using (var unsortedDeltas = File.OpenRead(UnsortedDeltaFile))
                using (var sequenceParser = new FastASequencePositionParser(reads, true))
                using (var unsortedDeltaParser = new DeltaAlignmentParser(unsortedDeltas, sequenceParser))
                {
                    if (!string.IsNullOrEmpty(this.OutputFile))
                    {
                        streamWriterConsoleOut = new StreamWriter(this.OutputFile);
                        Console.SetOut(streamWriterConsoleOut);
                    }

                    long deltaPositionInFile = 0;

                    foreach (long id in sorter.GetSortedIds())
                    {
                        DeltaAlignment deltaAlignment = unsortedDeltaParser.GetDeltaAlignmentAt(id);

                        deltaAlignment.Id = deltaPositionInFile;
                        string deltaString = Helper.GetString(deltaAlignment);
                        deltaPositionInFile += deltaString.Length;
                        Console.Write(deltaString);
                    }

                    Console.Out.Flush();
                }
            }
            finally
            {
                if (streamWriterConsoleOut != null)
                    streamWriterConsoleOut.Dispose();
                Console.SetOut(textWriterConsoleOutSave);
            }
        }

        /// <summary>
        /// Writes delta for query sequences.
        /// </summary>
        /// <param name="delta">The Deltas.</param>
        /// <param name="sorter">Sorter instance.</param>
        private static void WriteUnsortedDelta(IEnumerable<DeltaAlignment> delta, DeltaAlignmentSorter sorter)
        {
            using (var writer = new StreamWriter(UnsortedDeltaFile))
            {
                long deltaPositionInFile = 0;
                foreach (DeltaAlignment deltaAlignment in delta)
                {
                    deltaAlignment.Id = deltaPositionInFile;
                    string deltaString = Helper.GetString(deltaAlignment);
                    deltaPositionInFile += deltaString.Length;
                    writer.Write(deltaString);
                    sorter.Add(deltaAlignment.Id, deltaAlignment.FirstSequenceStart);
                }
            }
        }

        #endregion
    }
}
