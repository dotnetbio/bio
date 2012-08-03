using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bio.Algorithms.Alignment;
using Bio.Algorithms.Assembly.Comparative;
using Bio.IO.FastA;
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
            inputFileinfo = null;
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
            inputFileinfo = null;

            runRepeatResolution.Restart();
            DeltaAlignmentSorter sorter = null;
            using (DeltaAlignmentCollection deltaCollection = new DeltaAlignmentCollection(this.FilePath[0], this.FilePath[1]))
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
        /// <param name="delta">The Deltas.</param>
        private void WriteDelta(
            DeltaAlignmentSorter sorter)
        {
            FastASequencePositionParser sequenceParser = null;
            DeltaAlignmentParser unsortedDeltaParser = null;

            TextWriter textWriterConsoleOutSave = Console.Out;
            StreamWriter streamWriterConsoleOut = null;

            try
            {
                sequenceParser = new FastASequencePositionParser(this.FilePath[1], true);
                unsortedDeltaParser = new DeltaAlignmentParser(UnsortedDeltaFile, sequenceParser);
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
            finally
            {
                if (streamWriterConsoleOut != null)
                {
                    streamWriterConsoleOut.Dispose();
                    streamWriterConsoleOut = null;
                }

                if (sequenceParser != null)
                {
                    sequenceParser.Dispose();
                    sequenceParser = null;
                }

                if (unsortedDeltaParser != null)
                {
                    unsortedDeltaParser.Dispose();
                    unsortedDeltaParser = null;
                }

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
            using (StreamWriter writer = new StreamWriter(UnsortedDeltaFile))
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

                writer.Flush();
            }
        }

        #endregion
    }
}
