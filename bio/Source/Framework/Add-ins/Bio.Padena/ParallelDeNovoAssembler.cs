using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Timers;
using Bio.Algorithms.Assembly.Graph;
using Bio.Algorithms.Assembly.Padena.Scaffold;
using Bio.Registration;
using Bio.Util;

namespace Bio.Algorithms.Assembly.Padena
{
    /// <summary>
    /// Implements a de bruijn based approach for
    /// assembly of DNA sequences.
    /// </summary>
    [RegistrableAttribute(true)]
    public class ParallelDeNovoAssembler : IDeBruijnDeNovoAssembler, IDisposable
    {
        #region Fields
        /// <summary>
        /// Holds time interval between two progress events.
        /// </summary>
        private const int ProgressTimerInterval = 5 * 60 * 1000;

        /// <summary>
        /// User Input Parameter
        /// Length of k-mer.
        /// </summary>
        private int kmerLength;

        /// <summary>
        /// User Input Parameter
        /// Threshold for removing dangling ends in graph.
        /// </summary>
        private int dangleThreshold = 0;

        /// <summary>
        /// Bool to do erosion or not.
        /// </summary>
        private bool isErosionEnabled = false;

        /// <summary>
        /// User Input Parameter
        /// Threshold for eroding low coverage ends.
        /// </summary>
        private int erosionThreshold = -1;

        /// <summary>
        /// User Input Parameter
        /// Length Threshold for removing redundant paths in graph.
        /// </summary>
        private int redundantPathLengthThreshold = 0;

        /// <summary>
        /// Threshold used for removing low-coverage contigs.
        /// </summary>
        private double contigCoverageThreshold = -1;

        /// <summary>
        /// Class implementing Low coverage contig removal.
        /// </summary>
        private ILowCoverageContigPurger lowCoverageContigPurger;

        ///// <summary>
        ///// List of input sequence reads. Different steps in the assembly 
        ///// may access this. Should be set before starting the assembly process.
        ///// </summary>
        private IEnumerable<ISequence> sequenceReads;

        /// <summary>
        /// Holds the de bruijn graph used for assembly process.
        /// Graph creation modules sets this, so that further steps 
        /// can access this for modifications.
        /// </summary>
        private DeBruijnGraph graph;

        /// <summary>
        /// Class implementing dangling links purging.
        /// </summary>
        private IGraphErrorPurger danglingLinksPurger;

        /// <summary>
        /// Class implementing redundant paths purger.
        /// </summary>
        private IGraphErrorPurger redundantPathsPurger;

        /// <summary>
        /// Class implementing contig building.
        /// </summary>
        private IContigBuilder contigBuilder;

        /// <summary>
        /// Class implementing scaffold building.
        /// </summary>
        private IGraphScaffoldBuilder scaffoldBuilder;

        /// <summary>
        /// Holds the status message which will be sent throught the Status event.
        /// </summary>
        private string statusMessage;

        /// <summary>
        /// Timer to report progress.
        /// </summary>
        private Timer progressTimer;

        /// <summary>
        /// Holds the current step number being executed.
        /// </summary>
        private int currentStep;

        /// <summary>
        /// Flag to indicate whether graph building completed progress message is written to cconsole or not.
        /// </summary>
        private bool graphBuildCompleted;

        /// <summary>
        /// Flag to indicate whether generate link completed progress message is written to cconsole or not.
        /// </summary>
        private bool linkGenerationCompleted;

        #endregion

        /// <summary>
        /// Initializes a new instance of the ParallelDeNovoAssembler class.
        /// Sets thresholds to default values.
        /// Also initializes instances implementing different steps.
        /// </summary>
        public ParallelDeNovoAssembler()
        {
            // Initialize to default here.
            // Values set to -1 here will be reset based on input sequences.
            this.kmerLength = -1;
            this.dangleThreshold = -1;
            this.redundantPathLengthThreshold = -1;
            this.sequenceReads = new List<ISequence>();

            // Contig and scaffold Builder are required modules. Set this to default.
            this.contigBuilder = new SimplePathContigBuilder();

            // Default values for parameters used in building scaffolds.
            this.ScaffoldRedundancy = 2;
            this.Depth = 10;
            this.AllowKmerLengthEstimation = true;
        }

        #region Properties
        /// <summary>
        /// Provides the status to the subscribers.
        /// </summary>
        public event EventHandler<StatusChangedEventArgs> StatusChanged;

        /// <summary>
        /// Gets the name of the current assembly algorithm used.
        /// This property returns the Name of our assembly algorithm i.e 
        /// Parallel De Novo algorithm.
        /// </summary>
        public string Name
        {
            get { return Properties.Resource.Padena; }
        }

        /// <summary>
        /// Gets the description of the current assembly algorithm used.
        /// This property returns a simple description of what 
        ///  Parallel De Novo class implements.
        /// </summary>
        public string Description
        {
            get { return Properties.Resource.PadenaDescription; }
        }

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
        /// Gets the assembler de-bruijn graph.
        /// </summary>
        public DeBruijnGraph Graph
        {
            get { return this.graph; }
        }

        /// <summary>
        /// Gets or sets the instance that implements
        /// dangling links purging step.
        /// </summary>
        public IGraphErrorPurger DanglingLinksPurger
        {
            get { return this.danglingLinksPurger; }
            set { this.danglingLinksPurger = value; }
        }

        /// <summary>
        /// Gets or sets the threshold length 
        /// for dangling link purger.
        /// </summary>
        public int DanglingLinksThreshold
        {
            get { return this.dangleThreshold; }
            set { this.dangleThreshold = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow erosion of the graph.
        /// </summary>
        public bool AllowErosion
        {
            get { return this.isErosionEnabled; }
            set { this.isErosionEnabled = value; }
        }

        /// <summary>
        /// Gets or sets the threshold length for eroding low coverage graph 
        /// ends. In case erosion step is not to be done, set this to 0.
        /// As an performance optimization in assembler process, erosion and 
        /// dangling link purging step are done together in a single step. 
        /// Note that because of this optimization, unless the danglingLinkPurger 
        /// implements IGraphErodePurger, erosion will not be done irrespective 
        /// of the threshold value provided. 
        /// </summary>
        public int ErosionThreshold
        {
            get { return this.erosionThreshold; }
            set { this.erosionThreshold = value; }
        }

        /// <summary>
        /// Gets or sets the instance that implements
        /// redundant paths purging step.
        /// </summary>
        public IGraphErrorPurger RedundantPathsPurger
        {
            get { return this.redundantPathsPurger; }
            set { this.redundantPathsPurger = value; }
        }

        /// <summary>
        /// Gets or sets the length threshold 
        /// for redundant paths purger.
        /// </summary>
        public int RedundantPathLengthThreshold
        {
            get { return this.redundantPathLengthThreshold; }
            set { this.redundantPathLengthThreshold = value; }
        }

        /// <summary>
        /// Gets or sets instance of class implementing Low coverage contig removal.
        /// </summary>
        public ILowCoverageContigPurger LowCoverageContigPurger
        {
            get { return this.lowCoverageContigPurger; }
            set { this.lowCoverageContigPurger = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable removal of low coverage contigs.
        /// </summary>
        public bool AllowLowCoverageContigRemoval { get; set; }

        /// <summary>
        /// Gets or sets Threshold used for removing low-coverage contigs.
        /// </summary>
        public double ContigCoverageThreshold
        {
            get { return this.contigCoverageThreshold; }
            set { this.contigCoverageThreshold = value; }
        }

        /// <summary>
        /// Gets or sets the instance that implements
        /// contig building step.
        /// </summary>
        public IContigBuilder ContigBuilder
        {
            get { return this.contigBuilder; }
            set { this.contigBuilder = value; }
        }

        /// <summary>
        /// Gets or sets the instance that implements
        /// scaffold building step.
        /// </summary>
        public IGraphScaffoldBuilder ScaffoldBuilder
        {
            get { return this.scaffoldBuilder; }
            set { this.scaffoldBuilder = value; }
        }

        /// <summary>
        /// Gets or sets value of redundancy for building scaffolds.
        /// </summary>
        public int ScaffoldRedundancy { get; set; }

        /// <summary>
        /// Gets or sets the Depth for graph traversal in scaffold builder step.
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Gets or sets the list of sequence reads.
        /// </summary>
        protected IList<ISequence> SequenceReads
        {
            get { return this.sequenceReads.ToList(); }
            set { this.sequenceReads = value; }
        }

        #endregion

        /// <summary>
        /// For optimal graph formation, k-mer length should not be less 
        /// than half the length of the longest input sequence and 
        /// cannot be more than the length of the shortest input sequence. 
        /// Reference for estimating kmerlength from reads: Supplement material from 
        /// publication "ABySS: A parallel assembler for short read sequence data".
        /// </summary>
        /// <param name="sequences">List of input sequences.</param>
        /// <returns>Estimated optimal kmer length.</returns>
        public static int EstimateKmerLength(IEnumerable<ISequence> sequences)
        {
            // kmer length should be less than input sequence lengths
            long minSeqLength = long.MaxValue;
            long maxSeqLength = 0;

            float maxLengthOfKmer = int.MaxValue;
            float minLengthOfKmer = 0;
            int kmerLength = 0;

            if (sequences == null)
            {
                throw new ArgumentNullException("sequences");
            }

            if (!Alphabets.CheckIsFromSameBase(sequences.First().Alphabet, Alphabets.DNA))
            {
                throw new InvalidOperationException(Properties.Resource.CannotAssembleSequenceType);
            }

            foreach (ISequence seq in sequences)
            {
                long seqCount = seq.Count;
                if (minSeqLength > seqCount)
                {
                    // Get the min seq count to maxLength.
                    minSeqLength = seqCount;
                }

                if (maxSeqLength < seqCount)
                {
                    // Get the max sequence count to minLength.
                    maxSeqLength = seqCount;
                }
            }

            maxLengthOfKmer = minSeqLength;

            // for optimal purpose, kmer length should be more than half of longest sequence
            minLengthOfKmer = maxSeqLength / 2;

            if (minLengthOfKmer < maxLengthOfKmer)
            {
                // Choose median value between the end-points
                kmerLength = (int)Math.Ceiling((minLengthOfKmer + maxLengthOfKmer) / 2);
            }
            else
            {
                // In this case pick maxLength, since this is a hard limit
                kmerLength = (int)Math.Floor(maxLengthOfKmer);
            }

            if (maxLengthOfKmer < kmerLength)
            {
                throw new InvalidOperationException(Properties.Resource.InappropriateKmerLength);
            }

            if (kmerLength <= 0)
            {
                throw new InvalidOperationException(Properties.Resource.KmerLength);
            }

            // This needs to be modified when we have a structure which can hold kmerData of more than 32 symbols.
            return kmerLength > 32 ? 32 : kmerLength;
        }

        /// <summary>
        /// Assemble the list of sequence reads.
        /// </summary>
        /// <param name="inputSequences">List of input sequences.</param>
        /// <returns>Assembled output.</returns>
        public IDeNovoAssembly Assemble(IEnumerable<ISequence> inputSequences)
        {
            if (inputSequences == null)
            {
                throw new ArgumentNullException("inputSequences");
            }

            this.sequenceReads = inputSequences;

            // Remove ambiguous reads and set up fields for assembler process
            this.Initialize();

            // Step 1, 2: Create k-mers from reads and build de bruijn graph
            this.CreateGraphStarted();
            this.CreateGraph();
            this.CreateGraphEnded();

            // Estimate and set default value for erosion and coverage thresholds
            this.EstimateDefaultValuesStarted();
            this.EstimateDefaultThresholds();
            this.EstimateDefaultValuesEnded();

            // Step 3: Remove dangling links from graph
            this.UndangleGraphStarted();
            this.UnDangleGraph();
            this.UndangleGraphEnded();

            // Step 4: Remove redundant paths from graph
            this.RemoveRedundancyStarted();
            this.RemoveRedundancy();
            this.RemoveRedundancyEnded();

            // Perform dangling link purger step once more.
            // This is done to remove any links created by redundant paths purger.
            this.statusMessage = string.Format(CultureInfo.CurrentCulture, "\n UndangleGraph - Start time: {0}", DateTime.Now);
            this.RaiseStatusEvent();
            this.UnDangleGraph();
            this.statusMessage = string.Format(CultureInfo.CurrentCulture, "\n UndangleGraph - End time: {0}", DateTime.Now);
            this.RaiseStatusEvent();

            // Step 5: Build Contigs
            this.BuildContigsStarted();
            IEnumerable<ISequence> contigSequences = this.BuildContigs();
            this.BuildContigsEnded();

            PadenaAssembly result = new PadenaAssembly();
            result.AddContigs(contigSequences);

            return result;
        }

        /// <summary>
        /// Assemble the list of sequence reads. Also performs the 
        /// scaffold building step as part of assembly process.
        /// </summary>
        /// <param name="inputSequences">List of input sequences.</param>
        /// <param name="includeScaffolds">Boolean indicating whether scaffold building step has to be run.</param>
        /// <returns>Assembled output.</returns>
        public IDeNovoAssembly Assemble(IEnumerable<ISequence> inputSequences, bool includeScaffolds)
        {
            PadenaAssembly assemblyResult = (PadenaAssembly)this.Assemble(inputSequences);

            if (includeScaffolds)
            {
                // Step 6: Build _scaffolds
                this.BuildScaffoldsStarted();
                IList<ISequence> scaffolds = this.BuildScaffolds(assemblyResult.ContigSequences);
                this.BuildScaffoldsEnded();

                if (scaffolds != null)
                {
                    assemblyResult.AddScaffolds(scaffolds);
                }
            }

            return assemblyResult;
        }

        /// <summary>
        /// Implements dispose to suppress GC finalize
        /// This is done as one of the methods uses ReadWriterLockSlim
        /// which extends IDisposable.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Estimates and sets erosion and coverage threshold for contigs.
        /// Median value of kmer coverage is set as default value.
        /// Reference: ABySS Release Notes 1.1.1 - "The default threshold 
        /// is the square root of the median k-mer coverage".
        /// </summary>
        protected void EstimateDefaultThresholds()
        {
            if (this.isErosionEnabled || this.AllowLowCoverageContigRemoval)
            {
                // In case of low coverage data, set default as 2.
                // Reference: ABySS Release Notes 1.0.15
                // Before calculating median, discard thresholds less than 2.
                List<long> kmerCoverage = this.graph.GetNodes().AsParallel().Aggregate(
                    new List<long>(),
                    (kmerList, n) =>
                    {
                        if (n.KmerCount > 2)
                        {
                            kmerList.Add(n.KmerCount);
                        }

                        return kmerList;
                    });

                double threshold;
                if (kmerCoverage.Count == 0)
                {
                    threshold = 2; // For low coverage data, set default as 2
                }
                else
                {
                    kmerCoverage.Sort();
                    int midPoint = kmerCoverage.Count / 2;
                    double median = (kmerCoverage.Count % 2 == 1 || midPoint == 0) ?
                        kmerCoverage[midPoint] :
                        ((float)(kmerCoverage[midPoint] + kmerCoverage[midPoint - 1])) / 2;
                    threshold = Math.Sqrt(median);
                }

                // Set coverage threshold
                if (this.AllowLowCoverageContigRemoval && this.contigCoverageThreshold == -1)
                {
                    this.contigCoverageThreshold = threshold;
                }

                if (this.isErosionEnabled && this.erosionThreshold == -1)
                {
                    // Erosion threshold is an int, so round it off
                    this.erosionThreshold = (int)Math.Round(threshold);
                }
            }
        }

        /// <summary>
        /// Step 1: Building k-mers from sequence reads
        /// Step 2: Build de bruijn graph for input set of k-mers.
        /// Sets the _assemblerGraph field.
        /// </summary>
        protected void CreateGraph()
        {
            this.graph = new DeBruijnGraph(this.kmerLength);
            this.graph.Build(this.sequenceReads);
        }

        /// <summary>
        /// Step 3: Remove dangling links from graph.
        /// </summary>
        protected void UnDangleGraph()
        {
            if (this.danglingLinksPurger != null && this.dangleThreshold > 0)
            {
                DeBruijnPathList danglingNodes = null;

                // Observe lenghts of dangling links in the graph
                // This is an optimization - instead of incrementing threshold by 1 and 
                // running the purger iteratively, we first determine the lengths of the 
                // danglings links found in the graph and run purger only for those lengths.
                this.danglingLinksPurger.LengthThreshold = this.dangleThreshold - 1;

                IEnumerable<int> danglingLengths;
                IGraphEndsEroder graphEndsEroder = this.danglingLinksPurger as IGraphEndsEroder;
                if (graphEndsEroder != null && this.isErosionEnabled)
                {
                    // If eroder is implemented, while getting lengths of dangling links, 
                    // it also erodes the low coverage ends.
                    danglingLengths = graphEndsEroder.ErodeGraphEnds(this.graph, this.erosionThreshold);
                }
                else
                {
                    // Perform dangling purger at all incremental values till dangleThreshold.
                    danglingLengths = Enumerable.Range(1, this.dangleThreshold - 1);
                }

                // Erosion is to be only once. Reset erode threshold to -1.
                this.erosionThreshold = -1;

                // Start removing dangling links
                foreach (int threshold in danglingLengths)
                {
                    if (this.graph.NodeCount >= threshold)
                    {
                        this.danglingLinksPurger.LengthThreshold = threshold;
                        danglingNodes = this.danglingLinksPurger.DetectErroneousNodes(this.graph);
                        this.danglingLinksPurger.RemoveErroneousNodes(this.graph, danglingNodes);
                    }
                }

                // Removing dangling links can in turn create more dangling links
                // In order to remove all links within threshold, we therefore run
                // purger at threshold length until there is no more change in graph.
                do
                {
                    danglingNodes = null;
                    if (this.graph.NodeCount >= this.dangleThreshold)
                    {
                        this.danglingLinksPurger.LengthThreshold = this.dangleThreshold;
                        danglingNodes = this.danglingLinksPurger.DetectErroneousNodes(this.graph);
                        this.danglingLinksPurger.RemoveErroneousNodes(this.graph, danglingNodes);
                    }
                }
                while (danglingNodes != null && danglingNodes.Paths.Count > 0);
            }
        }

        /// <summary>
        /// Step 4: Remove redundant paths from graph.
        /// </summary>
        protected void RemoveRedundancy()
        {
            if (this.redundantPathsPurger != null)
            {
                DeBruijnPathList redundantNodes;
                do
                {
                    redundantNodes = this.redundantPathsPurger.DetectErroneousNodes(this.graph);
                    this.redundantPathsPurger.RemoveErroneousNodes(this.graph, redundantNodes);
                }
                while (redundantNodes.Paths.Count > 0);
            }
        }

        /// <summary>
        /// Step 5: Build contigs from de bruijn graph.
        /// If coverage threshold is set, remove low coverage contigs.
        /// </summary>
        /// <returns>List of contig sequences.</returns>
        protected IEnumerable<ISequence> BuildContigs()
        {
            if (this.contigBuilder == null)
            {
                throw new InvalidOperationException(Properties.Resource.NullContigBuilder);
            }

            // Step 5.1: Remove low coverage contigs
            if (this.AllowLowCoverageContigRemoval && this.contigCoverageThreshold > 0)
            {
                this.lowCoverageContigPurger.RemoveLowCoverageContigs(this.graph, this.contigCoverageThreshold);
            }

            // Step 5.2: Build Contigs
            return this.contigBuilder.Build(this.graph);
        }

        /// <summary>
        /// Step 6: Build scaffolds from contig list and paired reads.
        /// </summary>
        /// <param name="contigs">List of contigs.</param>
        /// <returns>List of scaffold sequences.</returns>
        protected IList<ISequence> BuildScaffolds(IList<ISequence> contigs)
        {
            if (this.scaffoldBuilder == null)
            {
                // Scaffold Builder is a required module for this method. Set this to default.
                this.scaffoldBuilder = new GraphScaffoldBuilder();
            }

            return this.scaffoldBuilder.BuildScaffold(this.SequenceReads, contigs, this.KmerLength, depth: this.Depth, redundancy: this.ScaffoldRedundancy);
        }

        /// <summary>
        /// Dispose field instances.
        /// </summary>
        /// <param name="disposeManaged">If disposeManaged equals true, clean all resources.</param>
        protected virtual void Dispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                if (this.scaffoldBuilder != null)
                {
                    this.scaffoldBuilder.Dispose();
                }

                this.graph = null;
                this.sequenceReads = null;
                this.danglingLinksPurger = null;
                this.redundantPathsPurger = null;
                this.contigBuilder = null;
                this.scaffoldBuilder = null;

                if (this.progressTimer != null)
                {
                    this.progressTimer.Dispose();
                    this.progressTimer = null;
                }
            }
        }

        /// <summary>
        /// Sets the sequences from which the graph will be created.
        /// </summary>
        /// <param name="sequences">Sequences to set.</param>
        protected void SetSequenceReads(IList<ISequence> sequences)
        {
            this.SequenceReads = sequences;
        }

        /// <summary>
        /// Sets up fields for the assembly process.
        /// </summary>
        private void Initialize()
        {
            this.currentStep = 0;
            this.progressTimer = new Timer(ProgressTimerInterval);
            this.progressTimer.Elapsed += new ElapsedEventHandler(this.ProgressTimerElapsed);

            this.statusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InitializingStarted, DateTime.Now);
            this.RaiseStatusEvent();

            // Reset parameters not set by user, based on sequenceReads
            if (this.AllowKmerLengthEstimation)
            {
                this.kmerLength = EstimateKmerLength(this.sequenceReads);
            }
            else
            {
                if (this.kmerLength <= 0)
                {
                    throw new InvalidOperationException(Properties.Resource.KmerLength);
                }

                try
                {
                    if (!Alphabets.CheckIsFromSameBase(this.sequenceReads.First().Alphabet, Alphabets.DNA))
                    {
                        throw new InvalidOperationException(Properties.Resource.CannotAssembleSequenceType);
                    }
                }
                catch (Exception e)
                {
                    if (e.InnerException != null && !string.IsNullOrEmpty(e.InnerException.Message))
                    {
                        throw e.InnerException;
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            if (this.dangleThreshold == -1)
            {
                this.dangleThreshold = this.kmerLength + 1;
            }

            if (this.redundantPathLengthThreshold == -1)
            {
                // Reference for default threshold for redundant path purger:
                // ABySS Release Notes 1.1.2 - "Pop bubbles shorter than N bp. The default is b=3*(k + 1)."
                this.redundantPathLengthThreshold = 3 * (this.kmerLength + 1);
            }

            this.InitializeDefaultGraphModifiers();

            this.statusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InitializingEnded, DateTime.Now);
            this.RaiseStatusEvent();
        }

        /// <summary>
        /// Initializes the above defined fields. For each step in assembly
        /// we use a separate class for implementation. This method assigns 
        /// these variables to classes with desired implementation.
        /// </summary>
        private void InitializeDefaultGraphModifiers()
        {
            // Assign uninitialized fields to default values
            if (this.danglingLinksPurger == null)
            {
                this.danglingLinksPurger = new DanglingLinksPurger();
            }

            if (this.redundantPathsPurger == null)
            {
                this.redundantPathsPurger = new RedundantPathsPurger(this.redundantPathLengthThreshold);
            }

            if (this.lowCoverageContigPurger == null)
            {
                this.lowCoverageContigPurger = new SimplePathContigBuilder();
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
            switch (this.currentStep)
            {
                case 2:
                    if (!this.graph.GraphBuildCompleted)
                    {
                        if (this.graph.SkippedSequencesCount > 0)
                        {
                            this.statusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.CreateGraphSubStatus, this.graph.SkippedSequencesCount, this.graph.ProcessedSequencesCount);
                        }
                        else
                        {
                            this.statusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.CreateGraphSubStatusWithoutSkipped, this.graph.ProcessedSequencesCount);
                        }
                    }
                    else
                    {
                        if (!this.graphBuildCompleted)
                        {
                            this.graphBuildCompleted = this.graph.GraphBuildCompleted;
                            this.statusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.GenerateLinkStarted, this.graph.ProcessedSequencesCount);
                        }
                        else
                            if (!this.linkGenerationCompleted && this.graph.LinkGenerationCompleted)
                            {
                                this.linkGenerationCompleted = this.graph.LinkGenerationCompleted;
                                this.statusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.GenerateLinkEnded);
                            }
                            else
                            {
                                this.statusMessage = Properties.Resource.DefaultSubStatus;
                            }
                    }

                    this.RaiseStatusEvent();
                    break;
                default:
                    this.statusMessage = Properties.Resource.DefaultSubStatus;
                    this.RaiseStatusEvent();
                    break;
            }
        }

        /// <summary>
        /// Raises status changed event with Graph creating started status message.
        /// </summary>
        private void CreateGraphStarted()
        {
            this.statusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.CreateGraphStarted, DateTime.Now);
            this.RaiseStatusEvent();
            this.currentStep = 2;
            this.progressTimer.Start();
        }

        /// <summary>
        /// Raises status changed event with Graph creating ended status message.
        /// </summary>
        private void CreateGraphEnded()
        {
            this.progressTimer.Stop();

            if (!this.graphBuildCompleted)
            {
                this.graphBuildCompleted = this.graph.GraphBuildCompleted;
                this.statusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.GenerateLinkStarted, this.graph.ProcessedSequencesCount);
                this.RaiseStatusEvent();
            }

            if (!this.linkGenerationCompleted && this.graph.LinkGenerationCompleted)
            {
                this.linkGenerationCompleted = this.graph.LinkGenerationCompleted;
                this.statusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.GenerateLinkEnded);
                this.RaiseStatusEvent();
            }

            this.statusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.CreateGraphEnded, DateTime.Now);
            this.RaiseStatusEvent();
        }

        /// <summary>
        /// Raises status changed event with EstimateDefaultValues started status message.
        /// </summary>
        private void EstimateDefaultValuesStarted()
        {
            this.statusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.EstimateDefaultValuesStarted, DateTime.Now);
            this.RaiseStatusEvent();
        }

        /// <summary>
        /// Raises status changed event with EstimateDefaultValues ended status message.
        /// </summary>
        private void EstimateDefaultValuesEnded()
        {
            this.statusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.EstimateDefaultValuesEnded, DateTime.Now);
            this.RaiseStatusEvent();
        }

        /// <summary>
        /// Raises status changed event with UndangleGraph started status message.
        /// </summary>
        private void UndangleGraphStarted()
        {
            this.statusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.UndangleGraphStarted, DateTime.Now);
            this.RaiseStatusEvent();
            this.currentStep = 3;
            this.progressTimer.Start();
        }

        /// <summary>
        /// Raises status changed event with UndangleGraph ended status message.
        /// </summary>
        private void UndangleGraphEnded()
        {
            this.progressTimer.Stop();
            this.statusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.UndangleGraphEnded, DateTime.Now);
            this.RaiseStatusEvent();
        }

        /// <summary>
        /// Raises status changed event with RemoveRedundancy started status message.
        /// </summary>
        private void RemoveRedundancyStarted()
        {
            this.statusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.RemoveReducndancyStarted, DateTime.Now);
            this.RaiseStatusEvent();
            this.currentStep = 4;
            this.progressTimer.Start();
        }

        /// <summary>
        /// Raises status changed event with RemoveRedundancy ended status message.
        /// </summary>
        private void RemoveRedundancyEnded()
        {
            this.progressTimer.Stop();
            this.statusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.RemoveReducndancyEnded, DateTime.Now);
            this.RaiseStatusEvent();
        }

        /// <summary>
        /// Raises status changed event with BuildContigs started status message.
        /// </summary>
        private void BuildContigsStarted()
        {
            this.statusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.BuildContigsStarted, DateTime.Now);
            this.RaiseStatusEvent();
            this.currentStep = 5;
            this.progressTimer.Start();
        }

        /// <summary>
        /// Raises status changed event with BuildContigs ended status message.
        /// </summary>
        private void BuildContigsEnded()
        {
            this.progressTimer.Stop();
            this.statusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.BuildContigsEnded, DateTime.Now);
            this.RaiseStatusEvent();
            this.currentStep = 0;
        }

        /// <summary>
        /// Raises status changed event with BuildScaffolds started status message.
        /// </summary>
        private void BuildScaffoldsStarted()
        {
            this.statusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.BuildScaffoldStarted, DateTime.Now);
            this.RaiseStatusEvent();
            this.currentStep = 6;
            this.progressTimer.Start();
        }

        /// <summary>
        /// Raises status changed event with BuildScaffolds ended status message.
        /// </summary>
        private void BuildScaffoldsEnded()
        {
            this.progressTimer.Stop();
            this.currentStep = 0;
            this.statusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.BuildScaffoldEnded, DateTime.Now);
            this.RaiseStatusEvent();
        }
    }
}
