using System;
using System.Diagnostics;
using System.IO;
using Bio.Algorithms.Alignment;
using Bio.Algorithms.Assembly.Comparative;
using Bio.Util;
using System.Collections.Generic;

namespace LayoutRefinementUtil
{
    /// <summary>
    /// Command line arguments for LayoutRefinement.
    /// </summary>
    internal class LayoutRefinementArguments
    {
        #region Fields
        /// <summary>
        /// Holds the unsorted delta alingment output filename for LayoutRefinment step.
        /// </summary>
        private const string UnsortedLayoutRefinmentOutputFilename = "UnsortedDeltaAlignments_LayoutRefinmentOutput.txt";

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
        /// Refine layout in the delta alignments.
        /// </summary>
        public void RefineLayout()
        {
            TimeSpan timeSpan = new TimeSpan();
            Stopwatch runAlgorithm = new Stopwatch();

            runAlgorithm.Restart();
            FileInfo inputFileinfo = new FileInfo(this.FilePath[1]);
            long inputFileLength = inputFileinfo.Length;
            FastASequencePositionParser queryParser;
            using(var input = File.OpenRead(FilePath[1]))
            {
                queryParser = new FastASequencePositionParser(input, true);
                queryParser.CacheSequencesForRandomAccess();
            }
            runAlgorithm.Stop();

            if (this.Verbose)
            {
                Output.WriteLine(OutputLevel.Verbose);
                Output.WriteLine(OutputLevel.Verbose, "Processed Query FastA file: {0}", Path.GetFullPath(this.FilePath[1]));
                Output.WriteLine(OutputLevel.Verbose, "   Read/Processing time   : {0}", runAlgorithm.Elapsed);
                Output.WriteLine(OutputLevel.Verbose, "   File Size              : {0}", inputFileLength);
            }

            inputFileinfo = new FileInfo(this.FilePath[0]);
            inputFileLength = inputFileinfo.Length;
            runAlgorithm.Restart();
            using (var input = File.OpenRead(FilePath[0]))
            using (DeltaAlignmentCollection deltaCollection = new DeltaAlignmentCollection(input, queryParser))
            {
                runAlgorithm.Stop();

                if (this.Verbose)
                {
                    Output.WriteLine(OutputLevel.Verbose);
                    Output.WriteLine(OutputLevel.Verbose, "Processed DeltaAlignment file: {0}", Path.GetFullPath(this.FilePath[0]));
                    Output.WriteLine(OutputLevel.Verbose, "   Read/Processing time      : {0}", runAlgorithm.Elapsed);
                    Output.WriteLine(OutputLevel.Verbose, "   File Size                 : {0}", inputFileLength);
                }

                runAlgorithm.Restart();
                IEnumerable<DeltaAlignment> result = LayoutRefiner.RefineLayout(deltaCollection);
                DeltaAlignmentSorter sorter = new DeltaAlignmentSorter();
                WriteDelta(result, sorter, UnsortedLayoutRefinmentOutputFilename);
                runAlgorithm.Stop();
                timeSpan = timeSpan.Add(runAlgorithm.Elapsed);

                runAlgorithm.Restart();
                WriteSortedDelta(sorter, UnsortedLayoutRefinmentOutputFilename, queryParser, this.OutputFile);
                runAlgorithm.Stop();
            }

            if (this.Verbose)
            {
                Output.WriteLine(OutputLevel.Verbose);
                Output.WriteLine(OutputLevel.Verbose, "Compute time: {0}", timeSpan);
                Output.WriteLine(OutputLevel.Verbose, "Write time: {0}", runAlgorithm.Elapsed);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Writes delta for query sequences.
        /// </summary>
        /// <param name="sorter">Sorter instance.</param>
        /// <param name="unsortedDeltaFilename">Unsorted Delta Filename.</param>
        /// <param name="queryParser">Query/Read parser</param>
        /// <param name="outputfilename">Output file name.</param>
        private static void WriteSortedDelta(DeltaAlignmentSorter sorter, string unsortedDeltaFilename, FastASequencePositionParser queryParser, string outputfilename)
        {
            using (var unsortedReads = File.OpenRead(unsortedDeltaFilename))
            using (DeltaAlignmentParser unsortedDeltaParser = new DeltaAlignmentParser(unsortedReads, queryParser))
            {
                TextWriter textWriterConsoleOutSave = Console.Out;
                StreamWriter streamWriterConsoleOut = null;
                try
                {
                    if (!string.IsNullOrEmpty(outputfilename))
                    {
                        Output.WriteLine(OutputLevel.Required);
                        streamWriterConsoleOut = new StreamWriter(outputfilename);
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
                        streamWriterConsoleOut.Dispose();
                    Console.SetOut(textWriterConsoleOutSave);
                }
            }
        }

        /// <summary>
        /// Writes delta for query sequences.
        /// </summary>
        /// <param name="deltaAlignments">Delta alignments to write.</param>
        /// <param name="sorter"> </param>
        /// <param name="filename">File name to write.</param>
        private static void WriteDelta(IEnumerable<DeltaAlignment> deltaAlignments, DeltaAlignmentSorter sorter, string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                long deltaPositionInFile = 0;

                foreach (DeltaAlignment deltaAlignment in deltaAlignments)
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
