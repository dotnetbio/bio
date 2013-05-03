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
        /// Holds time interval between two progress events.
        /// </summary>
        private const int ProgressTimerInterval = 5 * 60 * 1000;

        /// <summary>
        /// K-mer Length.
        /// </summary>
        private int _kmerLength = 10;

        /// <summary>
        /// Holds the status message which will be sent through the Status event.
        /// </summary>
        private string _statusMessage;

        /// <summary>
        /// Timer to report progress.
        /// </summary>
        private Timer _progressTimer;

        #region Constructor
        /// <summary>
        ///  Initializes a new instance of the ComparativeGenomeAssembler class.
        /// </summary>
        public ComparativeGenomeAssembler()
        {
            LengthOfMum = 20;
            Depth = 10;
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
                return this._kmerLength;
            }

            set
            {
                this._kmerLength = value;
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
        public int Depth { get; set; }

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
        public int LengthOfMum { get; set; }

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
            this._progressTimer = new Timer(ProgressTimerInterval);
            this._progressTimer.Elapsed += this.ProgressTimerElapsed;
            if (queryParser == null)
            {
                throw new ArgumentNullException("queryParser");
            }

            string readAlignmentOutputFilename = null;
            string unsortedRepeatResolutionOutputFilename = null;
            string repeateResolutionOutputFilename = null;
            string unsortedLayoutRefinmentOutputFilename = null;
            string layoutRefinmentOutputFileName = null;

            try
            {
                // Converting to list to avoid multiple parse of the reference file if its a yield return
                var refSequences = referenceSequence.ToList();

                // CacheSequencesForRandomAccess will ignore the call if called more than once.
                queryParser.CacheSequencesForRandomAccess();
                IEnumerable<ISequence> reads = queryParser.Parse();

                // Comparative Assembly Steps
                // 1) Read Alignment (Calling NUCmer for aligning reads to reference sequence)
                this.StatusEventStart(Properties.Resources.ReadAlignmentStarted);
                IEnumerable<DeltaAlignment> alignmentBetweenReferenceAndReads = this.ReadAlignment(refSequences, 
                            reads.Where(a => a.Count >= this.LengthOfMum));

                readAlignmentOutputFilename = Path.GetTempFileName();
                WriteDelta(alignmentBetweenReferenceAndReads, readAlignmentOutputFilename);
                this.StatusEventEnd(Properties.Resources.ReadAlignmentEnded);

                // 2) Repeat Resolution
                this.StatusEventStart(Properties.Resources.RepeatResolutionStarted);
                DeltaAlignmentSorter sorter;

                unsortedRepeatResolutionOutputFilename = Path.GetTempFileName();
                using (DeltaAlignmentCollection deltaAlignmentFromReadAlignment = new DeltaAlignmentCollection(readAlignmentOutputFilename, queryParser))
                {
                    IEnumerable<DeltaAlignment> repeatResolvedDeltas = RepeatResolution(deltaAlignmentFromReadAlignment);
                    sorter = new DeltaAlignmentSorter(refSequences[0].Count);
                    WriteUnsortedDelta(repeatResolvedDeltas, sorter, unsortedRepeatResolutionOutputFilename);
                }

                this.StatusEventEnd(Properties.Resources.RepeatResolutionEnded);
                this.StatusEventStart(Properties.Resources.SortingResolvedDeltasStarted);

                repeateResolutionOutputFilename = Path.GetTempFileName();
                WriteSortedDelta(sorter, unsortedRepeatResolutionOutputFilename, queryParser, repeateResolutionOutputFilename);
                this.StatusEventEnd(Properties.Resources.SortingResolvedDeltasEnded);

                // 3) Layout Refinement
                this.StatusEventStart(Properties.Resources.LayoutRefinementStarted);

                layoutRefinmentOutputFileName = Path.GetTempFileName();
                using (DeltaAlignmentCollection unsortedDeltaCollectionForLayoutRefinment = new DeltaAlignmentCollection(repeateResolutionOutputFilename, queryParser))
                {
                    unsortedLayoutRefinmentOutputFilename = Path.GetTempFileName();
                    IEnumerable<DeltaAlignment> layoutRefinedDeltas = LayoutRefinment(unsortedDeltaCollectionForLayoutRefinment);
                    sorter = new DeltaAlignmentSorter(refSequences[0].Count);
                    WriteUnsortedDelta(layoutRefinedDeltas, sorter, unsortedLayoutRefinmentOutputFilename);
                    WriteSortedDelta(sorter, unsortedLayoutRefinmentOutputFilename, queryParser, layoutRefinmentOutputFileName);
                }

                this.StatusEventEnd(Properties.Resources.LayoutRefinementEnded);

                // 4) Consensus Generation
                this.StatusEventStart(Properties.Resources.ConsensusGenerationStarted);
                IList<ISequence> contigs;
                using (var delta = new DeltaAlignmentCollection(layoutRefinmentOutputFileName, queryParser))
                {
                    contigs = this.ConsensusGenerator(delta).ToList();
                }
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
                this._progressTimer.Stop();

                // Cleanup temp files.
                if (!string.IsNullOrEmpty(readAlignmentOutputFilename))
                    File.Delete(readAlignmentOutputFilename);
                if (!string.IsNullOrEmpty(unsortedRepeatResolutionOutputFilename))
                    File.Delete(unsortedRepeatResolutionOutputFilename);
                if (!string.IsNullOrEmpty(repeateResolutionOutputFilename))
                    File.Delete(repeateResolutionOutputFilename);
                if (!string.IsNullOrEmpty(unsortedLayoutRefinmentOutputFilename))
                    File.Delete(unsortedLayoutRefinmentOutputFilename);
                if (!string.IsNullOrEmpty(layoutRefinmentOutputFileName))
                    File.Delete(layoutRefinmentOutputFileName);
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
            return from sequence in referenceSequence 
                   select new NUCmer(sequence)
                   {
                        FixedSeparation = this.FixedSeparation,
                        MinimumScore = this.MinimumScore,
                        SeparationFactor = this.SeparationFactor,
                        MaximumSeparation = this.MaximumSeparation,
                        BreakLength = this.BreakLength,
                        LengthOfMUM = this.LengthOfMum
                   } 
                   into nucmer 
                   from qrySequence in reads from delta in nucmer.GetDeltaAlignments(qrySequence, false) 
                   select delta;
        }

        /// <summary>
        /// Raises status event.
        /// </summary>
        private void RaiseStatusEvent()
        {
            if (this.StatusChanged != null)
                this.StatusChanged.Invoke(this, new StatusChangedEventArgs(this._statusMessage));
        }

        /// <summary>
        /// Method to handle ProgressTimer elapsed event.
        /// </summary>
        /// <param name="sender">Progress timer.</param>
        /// <param name="e">Event arguments.</param>
        private void ProgressTimerElapsed(object sender, ElapsedEventArgs e)
        {
            this._statusMessage = Properties.Resources.DefaultSubStatus;
            this.RaiseStatusEvent();
        }

        /// <summary>
        /// Raises status changed event with Step started status message.
        /// </summary>
        private void StatusEventStart(string message)
        {
            this._statusMessage = string.Format(CultureInfo.CurrentCulture, message, DateTime.Now);
            this.RaiseStatusEvent();
            this._progressTimer.Start();
        }

        /// <summary>
        /// Raises status changed event with Step ended status message.
        /// </summary>
        private void StatusEventEnd(string message)
        {
            this._progressTimer.Stop();
            this._statusMessage = string.Format(CultureInfo.CurrentCulture, message, DateTime.Now);
            this.RaiseStatusEvent();
        }
    }
}
