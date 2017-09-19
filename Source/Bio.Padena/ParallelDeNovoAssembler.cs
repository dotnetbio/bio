using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Bio.Algorithms.Assembly.Graph;
using Bio.Algorithms.Assembly.Padena;
using Bio.Algorithms.Assembly.Padena.Scaffold;
using Bio.Algorithms.Kmer;
using Bio.Registration;

[assembly: BioRegister(typeof(ParallelDeNovoAssembler))]

namespace Bio.Algorithms.Assembly.Padena
{
    /// <summary>
    /// Implements a De-Bruijn based approach for
    /// assembly of DNA sequences.
    /// </summary>
    public class ParallelDeNovoAssembler : IDeBruijnDeNovoAssembler, IDisposable
    {
        /// <summary>
        /// User Input Parameter
        /// Length of k-mer.
        /// </summary>
        private int kmerLength;

        /// <summary>
        /// List of input sequence reads. Different steps in the assembly 
        /// may access this. Should be set before starting the assembly process.
        /// </summary>
        private IEnumerable<ISequence> sequenceReads;

        /// <summary>
        /// Holds the status message which will be sent through the Status event.
        /// </summary>
        private string StatusMessage
        {
            set { RaiseStatusEvent(value); }
        }

        /// <summary>
        /// Holds the current step number being executed.
        /// </summary>
        private int currentStep;

        /// <summary>
        /// Flag to indicate whether graph building completed progress message has been reported.
        /// </summary>
        private bool graphBuildCompleted;

        /// <summary>
        /// Flag to indicate whether generate link completed progress message has been reported.
        /// </summary>
        private bool linkGenerationCompleted;

        /// <summary>
        /// Initializes a new instance of the ParallelDeNovoAssembler class.
        /// Sets thresholds to default values.
        /// Also initializes instances implementing different steps.
        /// </summary>
        public ParallelDeNovoAssembler()
        {
            ContigCoverageThreshold = -1;
            ErosionThreshold = -1;
            AllowErosion = false;
            
            // Initialize to default here.
            // Values set to -1 here will be reset based on input sequences.
            this.kmerLength = -1;
            DanglingLinksThreshold = -1;
            RedundantPathLengthThreshold = -1;
            this.sequenceReads = new List<ISequence>();

            // Contig and scaffold Builder are required modules. Set this to default.
            ContigBuilder = new SimplePathContigBuilder();

            // Default values for parameters used in building scaffolds.
            ScaffoldRedundancy = 2;
            Depth = 10;
            AllowKmerLengthEstimation = true;
        }

        /// <summary>
        /// Provides the status to the subscribers.
        /// </summary>
        public event Action<string> StatusChanged = delegate { };

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
                AllowKmerLengthEstimation = false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to estimate kmer length.
        /// </summary>
        public bool AllowKmerLengthEstimation { get; set; }

        /// <summary>
        /// Gets the assembler de-bruijn graph.
        /// </summary>
        public DeBruijnGraph Graph { get; protected set; }

        /// <summary>
        /// Gets or sets the instance that implements
        /// dangling links purging step.
        /// </summary>
        public IGraphErrorPurger DanglingLinksPurger { get; set; }

        /// <summary>
        /// Gets or sets the threshold length 
        /// for dangling link purger.
        /// </summary>
        public int DanglingLinksThreshold { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow erosion of the graph.
        /// </summary>
        public bool AllowErosion { get; set; }

        /// <summary>
        /// Gets or sets the threshold length for eroding low coverage graph 
        /// ends. In case erosion step is not to be done, set this to 0.
        /// As an performance optimization in assembler process, erosion and 
        /// dangling link purging step are done together in a single step. 
        /// Note that because of this optimization, unless the danglingLinkPurger 
        /// implements IGraphErodePurger, erosion will not be done irrespective 
        /// of the threshold value provided. 
        /// </summary>
        public int ErosionThreshold { get; set; }

        /// <summary>
        /// Gets or sets the instance that implements
        /// redundant paths purging step.
        /// </summary>
        public IGraphErrorPurger RedundantPathsPurger { get; set; }

        /// <summary>
        /// Gets or sets the length threshold 
        /// for redundant paths purger.
        /// </summary>
        public int RedundantPathLengthThreshold { get; set; }

        /// <summary>
        /// Gets or sets instance of class implementing Low coverage contig removal.
        /// </summary>
        public ILowCoverageContigPurger LowCoverageContigPurger { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable removal of low coverage contigs.
        /// </summary>
        public bool AllowLowCoverageContigRemoval { get; set; }

        /// <summary>
        /// Gets or sets Threshold used for removing low-coverage contigs.
        /// </summary>
        public double ContigCoverageThreshold { get; set; }

        /// <summary>
        /// Gets or sets the instance that implements
        /// contig building step.
        /// </summary>
        public IContigBuilder ContigBuilder { get; set; }

        /// <summary>
        /// Gets or sets the instance that implements
        /// scaffold building step.
        /// </summary>
        public IGraphScaffoldBuilder ScaffoldBuilder { get; set; }

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
            if (sequences == null)
                throw new ArgumentNullException("sequences");

            if (!Alphabets.CheckIsFromSameBase(sequences.First().Alphabet, Alphabets.DNA))
                throw new InvalidOperationException(Properties.Resource.CannotAssembleSequenceType);

            long minSeqLength = long.MaxValue, maxSeqLength = 0;

            // Get the min/max ranges for the sequences
            foreach (ISequence seq in sequences)
            {
                long seqCount = seq.Count;
                if (minSeqLength > seqCount)
                    minSeqLength = seqCount;
                if (maxSeqLength < seqCount)
                    maxSeqLength = seqCount;
            }

            // for optimal purpose, kmer length should be more than half of longest sequence
            float minLengthOfKmer = Math.Max(1, maxSeqLength / 2);
            float maxLengthOfKmer = minSeqLength;

            int kmerLength = minLengthOfKmer < maxLengthOfKmer
                                 ? (int)Math.Ceiling((minLengthOfKmer + maxLengthOfKmer) / 2)
                                 : (int)Math.Floor(maxLengthOfKmer);

            // Make the kmer odd to avoid palindromes.
            if (kmerLength % 2 == 0)
            {
                kmerLength++;
                if (kmerLength > maxLengthOfKmer)
                    kmerLength -= 2;
                if (kmerLength <= 0)
                    kmerLength = 1;
            }

            // Final sanity checks.
            if (maxLengthOfKmer < kmerLength)
                throw new InvalidOperationException(Properties.Resource.InappropriateKmerLength);
            if (kmerLength <= 0)
                throw new InvalidOperationException(Properties.Resource.KmerLength);

            // Bound to our max size based on data handling.
            return kmerLength > KmerData32.MAX_KMER_LENGTH ? KmerData32.MAX_KMER_LENGTH : kmerLength;
        }

        /// <summary>
        /// Assemble the list of sequence reads.
        /// </summary>
        /// <param name="inputSequences">List of input sequences.</param>
        /// <returns>Assembled output.</returns>
        public virtual IDeNovoAssembly Assemble(IEnumerable<ISequence> inputSequences)
        {
            if (inputSequences == null)
            {
                throw new ArgumentNullException("inputSequences");
            }

            this.sequenceReads = inputSequences;

            CancellationTokenSource cts = new CancellationTokenSource();
            ReportIntermediateProgress(cts.Token);

            try
            {
                // Remove ambiguous reads and set up fields for assembler process
                Initialize();

                // Step 1, 2: Create k-mers from reads and build de bruijn graph
                Stopwatch sw = Stopwatch.StartNew();
                CreateGraphStarted();
                CreateGraph();
                sw.Stop();

                CreateGraphEnded();
                TaskTimeSpanReport(sw.Elapsed);
                NodeCountReport();

                // Estimate and set default value for erosion and coverage thresholds
                sw = Stopwatch.StartNew();
                EstimateDefaultValuesStarted();
                EstimateDefaultThresholds();
                sw.Stop();

                EstimateDefaultValuesEnded();
                TaskTimeSpanReport(sw.Elapsed);

                // Step 3: Remove dangling links from graph
                sw = Stopwatch.StartNew();
                UndangleGraphStarted();
                UnDangleGraph();
                sw.Stop();

                UndangleGraphEnded();
                TaskTimeSpanReport(sw.Elapsed);
                NodeCountReport();

                // Step 4: Remove redundant paths from graph
                sw = Stopwatch.StartNew();
                RemoveRedundancyStarted();
                RemoveRedundancy();
                NodeCountReport();

                // Perform dangling link purger step once more.
                // This is done to remove any links created by redundant paths purger.
                StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.SecondaryUndangleGraphStarted, DateTime.Now);
                UnDangleGraph();
                StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.SecondaryUndangleGraphEnded, DateTime.Now);

                // Report end after undangle
                sw.Stop();
                RemoveRedundancyEnded();
                TaskTimeSpanReport(sw.Elapsed);
                NodeCountReport();

                // Step 5: Build Contigs
                sw = Stopwatch.StartNew();
                BuildContigsStarted();
                IEnumerable<ISequence> contigSequences = BuildContigs();
                sw.Stop();

                BuildContigsEnded();
                TaskTimeSpanReport(sw.Elapsed);

                PadenaAssembly result = new PadenaAssembly();
                result.AddContigs(contigSequences);

                return result;
            }
            finally
            {
                cts.Cancel();
            }
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
            PadenaAssembly assemblyResult = (PadenaAssembly)Assemble(inputSequences);

            if (includeScaffolds)
            {
                // Step 6: Build _scaffolds
                BuildScaffoldsStarted();
                IList<ISequence> scaffolds = BuildScaffolds(assemblyResult.ContigSequences);
                BuildScaffoldsEnded();

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
            Dispose(true);
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
            if (AllowErosion || AllowLowCoverageContigRemoval)
            {
                // In case of low coverage data, set default as 2.
                // Reference: ABySS Release Notes 1.0.15
                // Before calculating median, discard thresholds less than 2.
                List<long> kmerCoverage = Graph.GetNodes().AsParallel().Aggregate(
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
                if (AllowLowCoverageContigRemoval && ContigCoverageThreshold < 0)
                {
                    ContigCoverageThreshold = threshold;
                }

                if (AllowErosion && ErosionThreshold == -1)
                {
                    // Erosion threshold is an int, so round it off
                    ErosionThreshold = (int) Math.Round(threshold);
                }
            }
        }

        /// <summary>
        /// Step 1: Building k-mers from sequence reads
        /// Step 2: Build de bruijn graph for input set of k-mers.
        /// Sets the _assemblerGraph field.
        /// </summary>
        protected virtual void CreateGraph()
        {
            Graph = new DeBruijnGraph(this.kmerLength);
            Graph.Build(this.sequenceReads);
        }

        /// <summary>
        /// Step 3: Remove dangling links from graph.
        /// </summary>
        protected void UnDangleGraph()
        {
            if (DanglingLinksPurger != null && DanglingLinksThreshold > 0)
            {
                DeBruijnPathList danglingNodes;

                // Observe lengths of dangling links in the graph
                // This is an optimization - instead of incrementing threshold by 1 and 
                // running the purger iteratively, we first determine the lengths of the 
                // danglings links found in the graph and run purger only for those lengths.
                DanglingLinksPurger.LengthThreshold = DanglingLinksThreshold - 1;

                IEnumerable<int> danglingLengths;
                IGraphEndsEroder graphEndsEroder = DanglingLinksPurger as IGraphEndsEroder;
                if (graphEndsEroder != null && AllowErosion)
                {
                    // If eroder is implemented, while getting lengths of dangling links, 
                    // it also erodes the low coverage ends, this marks any node for deletion below a threshold.

                    // TODO: Verify that this does enumerate all dangling ends, the concern is that if a dangling end of length 7 and 2
                    // arrive at a node which itself would be of dangling node of length 2 without these "dangling ends" then a dangling end of 9
                    // (which it would be without either the 7 or 2 end) might not be reported.
                    danglingLengths = graphEndsEroder.ErodeGraphEnds(Graph, ErosionThreshold);
                }
                else
                {
                    // Perform dangling purger at all incremental values till dangleThreshold.
                    danglingLengths = Enumerable.Range(1, DanglingLinksThreshold - 1);
                }

                // Erosion is to be only once. Reset erode threshold to -1.
                ErosionThreshold = -1;
            
                // Start removing dangling links
                foreach (int threshold in danglingLengths)
                {
                    if (Graph.NodeCount >= threshold)
                    {
                        DanglingLinksPurger.LengthThreshold = threshold;
                        danglingNodes = DanglingLinksPurger.DetectErroneousNodes(Graph);
                        DanglingLinksPurger.RemoveErroneousNodes(Graph, danglingNodes);
                    }
                }
          
                // Removing dangling links can in turn create more dangling links
                // In order to remove all links within threshold, we therefore run
                // purger at threshold length until there is no more change in graph.
                do
                {
                    danglingNodes = null;
                    if (Graph.NodeCount >= DanglingLinksThreshold)
                    {
                        DanglingLinksPurger.LengthThreshold = DanglingLinksThreshold;
                        danglingNodes = DanglingLinksPurger.DetectErroneousNodes(Graph);
                        DanglingLinksPurger.RemoveErroneousNodes(Graph, danglingNodes);
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
            if (RedundantPathsPurger != null)
            {
                DeBruijnPathList redundantNodes;
                do
                {
                    redundantNodes = RedundantPathsPurger.DetectErroneousNodes(Graph);
                    RedundantPathsPurger.RemoveErroneousNodes(Graph, redundantNodes);
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
            if (ContigBuilder == null)
            {
                throw new InvalidOperationException(Properties.Resource.NullContigBuilder);
            }

            // Step 5.1: Remove low coverage contigs
            if (AllowLowCoverageContigRemoval && ContigCoverageThreshold > 0)
            {
                LowCoverageContigPurger.RemoveLowCoverageContigs(Graph, ContigCoverageThreshold);
            }

            // Step 5.2: Build Contigs
            return ContigBuilder.Build(Graph);
        }

        /// <summary>
        /// Step 6: Build scaffolds from contig list and paired reads.
        /// </summary>
        /// <param name="contigs">List of contigs.</param>
        /// <returns>List of scaffold sequences.</returns>
        protected IList<ISequence> BuildScaffolds(IList<ISequence> contigs)
        {
            if (ScaffoldBuilder == null)
            {
                // Scaffold Builder is a required module for this method. Set this to default.
                ScaffoldBuilder = new GraphScaffoldBuilder();
            }

            return ScaffoldBuilder.BuildScaffold(SequenceReads, contigs, KmerLength, Depth, ScaffoldRedundancy);
        }

        /// <summary>
        /// Dispose field instances.
        /// </summary>
        /// <param name="disposeManaged">If disposeManaged equals true, clean all resources.</param>
        protected virtual void Dispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                if (ScaffoldBuilder != null)
                {
                    ScaffoldBuilder.Dispose();
                }

                Graph = null;
                this.sequenceReads = null;
                DanglingLinksPurger = null;
                RedundantPathsPurger = null;
                ContigBuilder = null;
                ScaffoldBuilder = null;
            }
        }

        /// <summary>
        /// Sets the sequences from which the graph will be created.
        /// </summary>
        /// <param name="sequences">Sequences to set.</param>
        protected void SetSequenceReads(IList<ISequence> sequences)
        {
            SequenceReads = sequences;
        }

        /// <summary>
        /// Sets up fields for the assembly process.
        /// </summary>
        protected void Initialize()
        {
            this.currentStep = 0;

            StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InitializingStarted, DateTime.Now);

            // Reset parameters not set by user, based on sequenceReads
            if (AllowKmerLengthEstimation)
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
                        throw e.InnerException;
                    throw;
                }
            }

            if (DanglingLinksThreshold == -1)
            {
                DanglingLinksThreshold = this.kmerLength + 1;
            }

            if (RedundantPathLengthThreshold == -1)
            {
                // Reference for default threshold for redundant path purger:
                // ABySS Release Notes 1.1.2 - "Pop bubbles shorter than N bp. The default is b=3*(k + 1)."
                RedundantPathLengthThreshold = 3 * (this.kmerLength + 1);
            }

            InitializeDefaultGraphModifiers();

            StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InitializingEnded, DateTime.Now);
        }

        /// <summary>
        /// Initializes the above defined fields. For each step in assembly
        /// we use a separate class for implementation. This method assigns 
        /// these variables to classes with desired implementation.
        /// </summary>
        protected void InitializeDefaultGraphModifiers()
        {
            // Assign uninitialized fields to default values
            if (DanglingLinksPurger == null)
            {
                DanglingLinksPurger = new DanglingLinksPurger();
            }

            if (RedundantPathsPurger == null)
            {
                RedundantPathsPurger = new RedundantPathsPurger(RedundantPathLengthThreshold);
            }

            if (LowCoverageContigPurger == null)
            {
                LowCoverageContigPurger = new SimplePathContigBuilder();
            }
        }

        /// <summary>
        /// Raises status event.
        /// </summary>
        protected void RaiseStatusEvent(string statusMessage)
        {
            StatusChanged(statusMessage);
        }

        /// <summary>
        /// Method to report intermediate progress
        /// </summary>
        protected async void ReportIntermediateProgress(CancellationToken token)
        {
            const int ProgressTimerInterval = 5 * 60 * 1000;

            while (!token.IsCancellationRequested)
            {
                await Task.Delay(ProgressTimerInterval, token);

                switch (this.currentStep)
                {
                    case 0:
                    case 1:
                        break;

                    case 2:
                        if (!Graph.GraphBuildCompleted)
                        {
                            StatusMessage = Graph.SkippedSequencesCount > 0
                                ? string.Format(CultureInfo.CurrentCulture, Properties.Resource.CreateGraphSubStatus, Graph.SkippedSequencesCount, Graph.ProcessedSequencesCount)
                                : string.Format(CultureInfo.CurrentCulture, Properties.Resource.CreateGraphSubStatusWithoutSkipped, Graph.ProcessedSequencesCount);
                        }
                        else
                        {
                            if (!this.graphBuildCompleted)
                            {
                                this.graphBuildCompleted = Graph.GraphBuildCompleted;
                                StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.GraphBuilt, Graph.ProcessedSequencesCount);
                                StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.GenerateLinkStarted);
                            }
                            else
                            {
                                if (!this.linkGenerationCompleted && Graph.LinkGenerationCompleted)
                                {
                                    this.linkGenerationCompleted = Graph.LinkGenerationCompleted;
                                    StatusMessage = string.Format(CultureInfo.CurrentCulture,
                                                                       Properties.Resource.GenerateLinkEnded);
                                }
                                else
                                {
                                    StatusMessage = Properties.Resource.DefaultSubStatus;
                                }
                            }
                        }
                        break;
                    default:
                        StatusMessage = Properties.Resource.DefaultSubStatus;
                        break;
                }
            }
        }

        /// <summary>
        /// Raises status changed event with Graph creating started status message.
        /// </summary>
        protected void CreateGraphStarted()
        {
            StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.CreateGraphStarted, DateTime.Now);
            this.currentStep = 2;
        }

        /// <summary>
        /// Raises status changed event with Graph creating ended status message.
        /// </summary>
        protected void CreateGraphEnded()
        {
            if (!this.graphBuildCompleted)
            {
                this.graphBuildCompleted = Graph.GraphBuildCompleted;
                StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.GraphBuilt, Graph.ProcessedSequencesCount);

                this.graphBuildCompleted = Graph.GraphBuildCompleted;
                StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.GenerateLinkStarted);
            }

            if (!this.linkGenerationCompleted && Graph.LinkGenerationCompleted)
            {
                this.linkGenerationCompleted = Graph.LinkGenerationCompleted;
                StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.GenerateLinkEnded);
            }

            StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.CreateGraphEnded, DateTime.Now);
        }

        /// <summary>
        /// Report the time a task took
        /// </summary>
        /// <param name="ts"></param>
        protected void TaskTimeSpanReport(TimeSpan ts)
        {
            StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.TimeSpanReport, ts);
            
        }
        /// <summary>
        /// Raise event to report the number of nodes currently in graph.
        /// </summary>
        protected void NodeCountReport()
        {
            StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.GraphNodeCountDisplay, Graph.NodeCount);
        }

        /// <summary>
        /// Raises status changed event with EstimateDefaultValues started status message.
        /// </summary>
        protected void EstimateDefaultValuesStarted()
        {
            StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.EstimateDefaultValuesStarted, DateTime.Now);
        }

        /// <summary>
        /// Raises status changed event with EstimateDefaultValues ended status message.
        /// </summary>
        protected void EstimateDefaultValuesEnded()
        {
            StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.EstimateDefaultValuesEnded, DateTime.Now);
        }

        /// <summary>
        /// Raises status changed event with UndangleGraph started status message.
        /// </summary>
        protected void UndangleGraphStarted()
        {
            StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.UndangleGraphStarted, DateTime.Now);
            this.currentStep = 3;
        }

        /// <summary>
        /// Raises status changed event with UndangleGraph ended status message.
        /// </summary>
        protected void UndangleGraphEnded()
        {
            StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.UndangleGraphEnded, DateTime.Now);
        }

        /// <summary>
        /// Raises status changed event with RemoveRedundancy started status message.
        /// </summary>
        protected void RemoveRedundancyStarted()
        {
            StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.RemoveReducndancyStarted, DateTime.Now);
            this.currentStep = 4;
        }

        /// <summary>
        /// Raises status changed event with RemoveRedundancy ended status message.
        /// </summary>
        protected void RemoveRedundancyEnded()
        {
            StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.RemoveReducndancyEnded, DateTime.Now);
        }

        /// <summary>
        /// Raises status changed event with BuildContigs started status message.
        /// </summary>
        protected void BuildContigsStarted()
        {
            StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.BuildContigsStarted, DateTime.Now);
            this.currentStep = 5;
        }

        /// <summary>
        /// Raises status changed event with BuildContigs ended status message.
        /// </summary>
        protected void BuildContigsEnded()
        {
            StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.BuildContigsEnded, DateTime.Now);
            this.currentStep = 0;
        }

        /// <summary>
        /// Raises status changed event with BuildScaffolds started status message.
        /// </summary>
        protected void BuildScaffoldsStarted()
        {
            StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.BuildScaffoldStarted, DateTime.Now);
            this.currentStep = 6;
        }

        /// <summary>
        /// Raises status changed event with BuildScaffolds ended status message.
        /// </summary>
        protected void BuildScaffoldsEnded()
        {
            this.currentStep = 0;
            StatusMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resource.BuildScaffoldEnded, DateTime.Now);
        }
    }
}
