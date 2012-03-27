using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Timers;
using Bio.Algorithms.Alignment;
using Bio.Algorithms.Assembly.Padena.Scaffold;
using Bio.Util;

namespace Bio.Algorithms.Assembly.Comparative
{
    /// <summary>
    /// Implements a comparative genome assembly for
    /// assembly of DNA sequences.
    /// </summary>
    public class ComparativeGenomeAssembler
    {
        /// <summary>
        /// Holds the delta alingment output filename for ReadAlignment step.
        /// </summary>
        private const string ReadAlignmentOutputFilename = "DeltaAlignments_ReadAlginmentOutput.txt";

        /// <summary>
        /// Holds the unsorted delta alingment output filename for RepeateResolution step.
        /// </summary>
        private const string UnsortedRepeatResolutionOutputFilename = "UnsortedDeltaAlignments_RepeatResolutionOutput.txt";

        /// <summary>
        /// Holds the sorted delta alingment output filename for RepeateResolution step.
        /// </summary>
        private const string RepeateResolutionOutputFilename = "SortedDeltaAlignments_RepeatResolverOutput.txt";

        /// <summary>
        /// Holds the unsorted delta alingment output filename for LayoutRefinment step.
        /// </summary>
        private const string UnsortedLayoutRefinmentOutputFilename = "UnsortedDeltaAlignments_LayoutRefinmentOutput.txt";

        /// <summary>
        /// Holds the sorted delta alingment output filename for LayoutRefinment step.
        /// </summary>
        private const string LayoutRefinmentOutputFileName = "SortedDeltaAlignments_LayoutRefinmentOutput.txt";

        /// <summary>
        /// Holds time interval between two progress events.
        /// </summary>
        private const int ProgressTimerInterval = 5 * 60 * 1000;

        /// <summary>
        /// K-mer Length.
        /// </summary>
        private int kmerLength = 10;

        /// <summary>
        /// Length of MUM. 
        /// </summary>
        private int lengthOfMum = 20;

        /// <summary>
        /// Default depth for graph traversal in scaffold builder step.
        /// </summary>
        private int depth = 10;

        /// <summary>
        /// Holds the status message which will be sent through the Status event.
        /// </summary>
        private string statusMessage;

        /// <summary>
        /// Timer to report progress.
        /// </summary>
        private Timer progressTimer;

        #region Constructor
        /// <summary>
        ///  Initializes a new instance of the ComparativeGenomeAssembler class.
        /// </summary>
        public ComparativeGenomeAssembler()
        {
            // Set default values.
            this.BreakLength = 200;
            this.FixedSeparation = 5;
            this.SeparationFactor = 0.12F;
            this.MinimumScore = 65;
            this.MaximumSeparation = 90;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Provides the status to the subscribers.
        /// </summary>
        public event EventHandler<StatusChangedEventArgs> StatusChanged;

        /// <summary>
        /// Gets or sets the kmer length.
        /// </summary>
        public int KmerLength
        {
            get
            {
                return this.kmerLength;
            }

            set
            {
                this.kmerLength = value;
                this.AllowKmerLengthEstimation = false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to estimate kmer length.
        /// </summary>
        public bool AllowKmerLengthEstimation { get; set; }

        /// <summary>
        /// Gets or sets value of redundancy for building scaffolds.
        /// </summary>
        public int ScaffoldRedundancy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to run scaffolding step or not.
        /// </summary>
        public bool ScaffoldingEnabled { get; set; }

        /// <summary>
        /// Gets or sets the Depth for graph traversal in scaffold builder step.
        /// </summary>
        public int Depth
        {
            get
            {
                return this.depth;
            }

            set
            {
                this.depth = value;
            }
        }

        /// <summary>
        /// Gets the name of the sequence assembly algorithm being
        /// implemented. This is intended to give the
        /// developer some information of the current sequence assembly algorithm.
        /// </summary>
        public string Name
        {
            get { return Properties.Resources.PcaName; }
        }

        /// <summary>
        /// Gets the description of the sequence assembly algorithm being
        /// implemented. This is intended to give the
        /// developer some information of the current sequence assembly algorithm.
        /// </summary>
        public string Description
        {
            get { return Properties.Resources.PcaDescription; }
        }

        /// <summary>
        /// Gets or sets the length of MUM for using with NUCmer.
        /// </summary>
        public int LengthOfMum
        {
            get
            {
                return this.lengthOfMum;
            }

            set
            {
                this.lengthOfMum = value;
            }
        }

        /// <summary>
        /// Gets or sets number of bases to be extended before stopping alignment.
        /// </summary>
        public int BreakLength { get; set; }

        /// <summary>
        /// Gets or sets maximum fixed diagonal difference.
        /// </summary>
        public int FixedSeparation { get; set; }

        /// <summary>
        /// Gets or sets maximum separation between the adjacent matches in clusters.
        /// </summary>
        public int MaximumSeparation { get; set; }

        /// <summary>
        /// Gets or sets minimum output score.
        /// </summary>
        public int MinimumScore { get; set; }

        /// <summary>
        /// Gets or sets separation factor. Fraction equal to 
        /// (diagonal difference / match separation) where higher values
        /// increase the insertion or deletion (indel) tolerance
        /// </summary>
        public float SeparationFactor { get; set; }
        #endregion

        /// <summary>
        /// Assemble the input sequences into the largest possible contigs. 
        /// </summary>
        /// <param name="referenceSequence">The sequence used as backbone for assembly.</param>
        /// <param name="queryParser">The parser to load the sequences to assemble.</param>
        /// <returns>IComparativeAssembly instance which contains list of assembled sequences.</returns>
        public IEnumerable<ISequence> Assemble(IEnumerable<ISequence> referenceSequence, FastASequencePositionParser queryParser)
        {
            this.progressTimer = new Timer(ProgressTimerInterval);
            this.progressTimer.Elapsed += new ElapsedEventHandler(this.ProgressTimerElapsed);
            DeltaAlignmentSorter sorter = null;
            if (queryParser == null)
            {
                throw new ArgumentNullException("queryParser");
            }

            try
            {
                // Converting to list to avoid multiple parse of the reference file if its a yield return
                List<ISequence> refSequences = referenceSequence.ToList();

                // CacheSequencesForRandomAccess will ignore the call if called more than once.
                queryParser.CacheSequencesForRandomAccess();
                IEnumerable<ISequence> reads = queryParser.Parse();

                // Comparative Assembly Steps
                // 1) Read Alignment (Calling NUCmer for aligning reads to reference sequence)
                this.StatusEventStart(Properties.Resources.ReadAlignmentStarted);
                IEnumerable<DeltaAlignment> alignmentBetweenReferenceAndReads = this.ReadAlignment(refSequences, reads.Where(a => a.Count >= this.LengthOfMum));
                WriteDelta(alignmentBetweenReferenceAndReads, ReadAlignmentOutputFilename);
                this.StatusEventEnd(Properties.Resources.ReadAlignmentEnded);

                // 2) Repeat Resolution
                this.StatusEventStart(Properties.Resources.RepeatResolutionStarted);
                using (DeltaAlignmentCollection deltaAlignmentFromReadAlignment = new DeltaAlignmentCollection(ReadAlignmentOutputFilename, queryParser))
                {
                    IEnumerable<DeltaAlignment> repeatResolvedDeltas = RepeatResolution(deltaAlignmentFromReadAlignment);
                    sorter = new DeltaAlignmentSorter(refSequences[0].Count);
                    WriteUnsortedDelta(repeatResolvedDeltas, sorter, UnsortedRepeatResolutionOutputFilename);
                }

                this.StatusEventEnd(Properties.Resources.RepeatResolutionEnded);

                this.StatusEventStart(Properties.Resources.SortingResolvedDeltasStarted);
                WriteSortedDelta(sorter, UnsortedRepeatResolutionOutputFilename, queryParser, RepeateResolutionOutputFilename);
                sorter = null;
                this.StatusEventEnd(Properties.Resources.SortingResolvedDeltasEnded);

                // 3) Layout Refinement
                this.StatusEventStart(Properties.Resources.LayoutRefinementStarted);

                using (DeltaAlignmentCollection unsortedDeltaCollectionForLayoutRefinment = new DeltaAlignmentCollection(RepeateResolutionOutputFilename, queryParser))
                {
                    IEnumerable<DeltaAlignment> layoutRefinedDeltas = LayoutRefinment(unsortedDeltaCollectionForLayoutRefinment);
                    sorter = new DeltaAlignmentSorter(refSequences[0].Count);
                    WriteUnsortedDelta(layoutRefinedDeltas, sorter, UnsortedLayoutRefinmentOutputFilename);
                    WriteSortedDelta(sorter, UnsortedLayoutRefinmentOutputFilename, queryParser, LayoutRefinmentOutputFileName);
                }

                sorter = null;

                this.StatusEventEnd(Properties.Resources.LayoutRefinementEnded);

                // 4) Consensus Generation
                this.StatusEventStart(Properties.Resources.ConsensusGenerationStarted);
                IEnumerable<ISequence> contigs = this.ConsensusGenerator(new DeltaAlignmentCollection(LayoutRefinmentOutputFileName, queryParser));
                this.StatusEventEnd(Properties.Resources.ConsensusGenerationEnded);

                if (this.ScaffoldingEnabled)
                {
                    // 5) Scaffold Generation
                    this.StatusEventStart(Properties.Resources.ScaffoldGenerationStarted);
                    IEnumerable<ISequence> scaffolds = this.ScaffoldsGenerator(contigs, reads);
                    this.StatusEventEnd(Properties.Resources.ScaffoldGenerationEnded);

                    return scaffolds;
                }
                else
                {
                    return contigs;
                }
            }
            finally
            {
                this.progressTimer.Stop();
            }
        }

        /// <summary>
        /// Writes delta for query sequences.
        /// </summary>
        /// <param name="deltaAlignments">Delta alignments to write.</param>
        /// <param name="filename">File name to write.</param>
        public static void WriteDelta(IEnumerable<DeltaAlignment> deltaAlignments, string filename)
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
                }

                writer.Flush();
            }
        }

        /// <summary>
        /// Writes delta for query sequences.
        /// </summary>
        /// <param name="delta">The Deltas.</param>
        /// <param name="sorter">Sorter instance.</param>
        /// <param name="filename">Output file name.</param>
        public static void WriteUnsortedDelta(IEnumerable<DeltaAlignment> delta, DeltaAlignmentSorter sorter, string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
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

        /// <summary>
        /// Writes delta for query sequences.
        /// </summary>
        /// <param name="sorter">Sorter instance.</param>
        /// <param name="unsortedDeltaFilename">Unsorted Delta Filename.</param>
        /// <param name="queryParser">Query/read sequences parser.</param>
        /// <param name="outputfilename">Output file name.</param>
        public static void WriteSortedDelta(DeltaAlignmentSorter sorter, string unsortedDeltaFilename, FastASequencePositionParser queryParser, string outputfilename)
        {
            if (sorter == null)
            {
                throw new ArgumentNullException("sorter");
            }

            using (DeltaAlignmentParser unsortedDeltaParser = new DeltaAlignmentParser(unsortedDeltaFilename, queryParser))
            {
                using (StreamWriter writer = new StreamWriter(outputfilename))
                {
                    long deltaPositionInFile = 0;
                    foreach (long id in sorter.GetSortedIds())
                    {
                        DeltaAlignment deltaAlignment = unsortedDeltaParser.GetDeltaAlignmentAt(id);
                        deltaAlignment.Id = deltaPositionInFile;
                        string deltaString = Helper.GetString(deltaAlignment);
                        deltaPositionInFile += deltaString.Length;
                        writer.Write(deltaString);
                    }

                    writer.Flush();
                }
            }
        }

        /// <summary>
        /// Refines layout of alignment between reads and reference genome by taking care of indels and rearrangements.
        /// </summary>
        /// <param name="deltaAlignmentCollection">Ordered Repeat Resolved Deltas.</param>
        private static IEnumerable<DeltaAlignment> LayoutRefinment(DeltaAlignmentCollection deltaAlignmentCollection)
        {
            return LayoutRefiner.RefineLayout(deltaAlignmentCollection);
        }

        /// <summary>
        /// Reads ambiguously placed due to genomic reads.
        /// This step requires mate pair information to resolve the ambiguity about placements of repeated sequences.
        /// </summary>
        /// <param name="deltaAlignmentCollection">Alignment between reference genome and reads.</param>
        /// <returns>List of DeltaAlignments after resolving repeating reads.</returns>
        private static IEnumerable<DeltaAlignment> RepeatResolution(DeltaAlignmentCollection deltaAlignmentCollection)
        {
            return RepeatResolver.ResolveAmbiguity(deltaAlignmentCollection);
        }

        /// <summary>
        /// Build scaffolds from contigs and paired reads (uses Padena Step 6 for assembly).
        /// </summary>
        /// <param name="contigs">List of contigs.</param>
        /// <param name="reads">List of paired reads.</param>
        /// <returns>List of scaffold sequences.</returns>
        private IEnumerable<ISequence> ScaffoldsGenerator(IEnumerable<ISequence> contigs, IEnumerable<ISequence> reads)
        {
            using (GraphScaffoldBuilder scaffoldBuilder = new GraphScaffoldBuilder())
            {
                return scaffoldBuilder.BuildScaffold(reads, contigs.ToList(), this.KmerLength, this.Depth, this.ScaffoldRedundancy);
            }
        }

        /// <summary>
        /// Generates a consensus sequence for the genomic region covered by reads.
        /// </summary>
        /// <param name="deltaAlignmentCollection">Alignment between reference genome and reads.</param>
        /// <returns>List of contigs.</returns>
        private IEnumerable<ISequence> ConsensusGenerator(DeltaAlignmentCollection deltaAlignmentCollection)
        {
            return ConsensusGeneration.GenerateConsensus(deltaAlignmentCollection);
        }

        /// <summary>
        /// Aligns reads to reference genome using NUCmer.
        /// </summary>
        /// <param name="referenceSequence">Sequence of reference genome.</param>
        /// <param name="reads">List of sequence reads.</param>
        /// <returns>Delta alignments after read alignment.</returns>
        private IEnumerable<DeltaAlignment> ReadAlignment(IEnumerable<ISequence> referenceSequence, IEnumerable<ISequence> reads)
        {
            foreach (ISequence sequence in referenceSequence)
            {
                NUCmer nucmer = new NUCmer((Sequence)sequence);

                nucmer.FixedSeparation = this.FixedSeparation;
                nucmer.MinimumScore = this.MinimumScore;
                nucmer.SeparationFactor = this.SeparationFactor;
                nucmer.MaximumSeparation = this.MaximumSeparation;
                nucmer.BreakLength = this.BreakLength;
                nucmer.LengthOfMUM = this.LengthOfMum;

                foreach (ISequence qrySequence in reads)
                {
                    foreach (DeltaAlignment delta in nucmer.GetDeltaAlignments(qrySequence, false))
                    {
                        yield return delta;
                    }
                }
            }
        }

        /// <summary>
        /// Raises status event.
        /// </summary>
        private void RaiseStatusEvent()
        {
            if (this.StatusChanged != null)
            {
                this.StatusChanged.Invoke(this, new StatusChangedEventArgs(this.statusMessage));
            }
        }

        /// <summary>
        /// Method to handle ProgressTimer elapsed event.
        /// </summary>
        /// <param name="sender">Progress timer.</param>
        /// <param name="e">Event arguments.</param>
        private void ProgressTimerElapsed(object sender, ElapsedEventArgs e)
        {
            this.statusMessage = Properties.Resources.DefaultSubStatus;
            this.RaiseStatusEvent();
        }

        /// <summary>
        /// Raises status changed event with Step started status message.
        /// </summary>
        private void StatusEventStart(string message)
        {
            this.statusMessage = string.Format(CultureInfo.CurrentCulture, message, DateTime.Now);
            this.RaiseStatusEvent();
            this.progressTimer.Start();
        }

        /// <summary>
        /// Raises status changed event with Step ended status message.
        /// </summary>
        private void StatusEventEnd(string message)
        {
            this.progressTimer.Stop();
            this.statusMessage = string.Format(CultureInfo.CurrentCulture, message, DateTime.Now);
            this.RaiseStatusEvent();
        }
    }
}
