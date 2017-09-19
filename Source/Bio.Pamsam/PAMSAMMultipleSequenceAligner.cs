using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Bio.Algorithms.Alignment.MultipleSequenceAlignment;
using Bio.Registration;
using Bio.SimilarityMatrices;
using System.Linq;
using System.Diagnostics;

[assembly: BioRegister(typeof(PAMSAMMultipleSequenceAligner))]


namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Implementation of modified MUSCLE multiple sequence alignment algorithm.
    /// Detailed can be found from paper: MUSCLE Edgar 2004.
    /// 
    /// There are three stages of MUSCLE (in the figure, green, cyan, and purple rectangles). 
    /// In Stage 1 the unaligned sequences are aligned in a fast and rough manner. Stage 2 and 
    /// stage 3 refine the alignment and can be iterated. The whole procedure can terminate at 
    /// the end of any stage and generate multiple sequence alignment (MSA). More iterations 
    /// yield more accurate MSA.
    ///
    /// Stage 1: draft progressive. In this stage the algorithm roughly aligns the sequences to 
    ///             generate the first multiple sequence alignment (MSA1). Stage 1 can be broken 
    ///             down into a few steps: a distance matrix (DM1) is first constructed via 
    ///             'k-mer counting', which will then be clustered into a binary guide tree (T1) 
    ///             by hierarchical clustering methods, e.g. UPGMA, neighbor-joining, or an even 
    ///             faster algorithm PartTree. In the guide tree, the leaves are the sequences.  
    ///             Next step is a general 'progressive alignment', where the sequences at the 
    ///             leaves of the tree will be aligned first, and then preceded to internal 
    ///             nodes in a bottom up manner.  A MSA is generated once the root is aligned. 
    ///             Progressive alignment employs profile-profile pair wise alignment algorithm 
    ///             (profile is a subset of aligned sequences), such as profile-profile 
    ///             Needleman-Wunsch algorithm in which a profile-profile function needs to be 
    ///             defined. 
    /// State 2: Improved progressive. With MSA1, a more accurate distance matrix can be constructed 
    ///             with Kimura distance matrix algorithm and the rest steps are the same as in 
    ///             step 1. Notice that in stage 2, accurate hierarchical clustering algorithms 
    ///             should be used, so PartTree is not an option here.
    /// Stage 3: Refinement. In this stage, the edges of guide tree will be  
    ///             selected in top-down order and broken, resulting in two sub-trees.which will be 
    ///             aligned separately. The two new MSAs are combined by 
    ///             profile-profile aligner. This step can be iterated until convergence or maximum 
    ///             iteration step is reached.
    /// 
    /// </summary>
    public class PAMSAMMultipleSequenceAligner : IMultipleSequenceAligner
    {
        // The alphabet of this class - assigned from the first input sequence
        private IAlphabet alphabet;

        // The number of partitions for parallelization
        // It is set to 1 for non-parallelization, and set to the number of processor cores for parallelization.
        private readonly int numberOfPartitions = 1;

        // The number of cores used by parallel extension
        private readonly int degreeOfParallelism = 1;

        private Stopwatch currentReportLog;

        /// <summary>
        /// Distance function name (enum)
        /// </summary>
        public DistanceFunctionTypes DistanceFunctionName { get; set; }

        /// <summary>
        /// Cluster update method (enum)
        /// </summary>
        public UpdateDistanceMethodsTypes HierarchicalClusteringMethodName { get; set; }

        /// <summary>
        /// Profile Aligner name (enum)
        /// </summary>
        public ProfileAlignerNames ProfileAlignerName { get; set; }

        /// <summary>
        /// Profile distance function (enum)
        /// </summary>
        public ProfileScoreFunctionNames ProfileProfileFunctionName { get; set; }

        /// <summary>
        /// Kmer Length
        /// </summary>
        public int KmerLength { get; set; }

        /// <summary>
        /// Gets or sets the object that will be used to compute the alignment's consensus.
        /// </summary>
        public IConsensusResolver ConsensusResolver { set; get; }

        /// <summary>
        /// Aligned sequences of the stage 1 in this class.
        /// </summary>
        public IList<ISequence> AlignedSequencesA { get; private set; }

        /// <summary>
        /// Aligned sequences of the stage 2 in this class.
        /// </summary>
        public IList<ISequence> AlignedSequencesB { get; private set; }

        /// <summary>
        /// Aligned sequences of the stage 3 in this class.
        /// </summary>
        public IList<ISequence> AlignedSequencesC { get; private set; }

        /// <summary>
        /// The final aligned sequences in this class
        /// </summary>
        public IList<ISequence> AlignedSequences { get; private set; }

        /// <summary>
        /// The alignment score of this class
        /// </summary>
        public float AlignmentScore { get; private set; }

        /// <summary>
        /// The alignment score of this class in stage 1
        /// </summary>
        public float AlignmentScoreA { get; private set; }

        /// <summary>
        /// The alignment score of this class in stage 2
        /// </summary>
        public float AlignmentScoreB { get; private set; }

        /// <summary>
        /// The alignment score of this class in stage 3
        /// </summary>
        public float AlignmentScoreC { get; private set; }

        /// <summary>
        /// The alignment method name of this class
        /// </summary>
        public string Name
        {
            get { return Resource.MuscleMultipleAlignmentMethodName; }
        }

        /// <summary>
        /// Gets the description of the sequence alignment algorithm
        /// </summary>
        public string Description
        {
            get { return Resource.MuscleMultipleAlignmentMethodDescription; }
        }

        /// <summary>
        /// Gets or sets similarity matrix for use in alignment algorithms.
        /// </summary>
        public SimilarityMatrix SimilarityMatrix { get; set; }

        /// <summary> 
        /// Gets or sets gap open penalty for use in alignment algorithms. 
        /// For alignments using a single gap penalty, this is the gap penalty.
        /// For alignments using an affine gap, this is the penalty to open a new gap.
        /// This is a negative number, for example GapOpenCost = -8, not +8.
        /// </summary>
        public int GapOpenCost { get; set; }

        /// <summary> 
        /// Gets or sets gap extension penalty for use in alignment algorithms. 
        /// Not used for alignments using a single gap penalty.
        /// For alignments using an affine gap, this is the penalty to
        /// extend an existing gap.
        /// This is a negative number, for example GapExtensionCost = -2, not +2.
        /// </summary>
        public int GapExtensionCost { get; set; }

        /// <summary>
        /// Status event for logging
        /// </summary>
        public event Action<string> StatusChanged = delegate { };

        /// <summary>
        /// Switch controls whether sequence weights are used
        /// </summary>
        static public bool UseWeights { get; set; }

        /// <summary>
        /// Control the number of cores used in parallel extensions
        /// </summary>
        static public int NumberOfCores { get; set; }

        /// <summary>
        /// Switch controls whether faster version PAMSAM is used.
        /// </summary>
        static public bool FasterVersion { get; set; }

        /// <summary>
        ///  Switch controls whether stage 2 is used
        /// </summary>
        static public bool UseStageB { get; set; }

        /// <summary>
        /// Set parallel options
        /// </summary>
        static public ParallelOptions ParallelOption { get; set; }

        /// <summary>
        /// Constructor used to add PAMSAM algorithm to SequenceAligners.All
        /// </summary>
        public PAMSAMMultipleSequenceAligner()
        {
            AlignmentScoreC = float.MinValue;
            AlignmentScoreB = float.MinValue;
            AlignmentScoreA = float.MinValue;
            AlignmentScore = float.MinValue;
            
            // Set parallel extension option
            this.degreeOfParallelism = (PlatformManager.Services.Is64BitProcessType) ? Environment.ProcessorCount : 1;
            ParallelOption = new ParallelOptions { MaxDegreeOfParallelism = degreeOfParallelism };
            this.numberOfPartitions = degreeOfParallelism * 2;

            // Initialize parameters to general defaults - must override by setting property values prior 
            // to calling Align.
            KmerLength = 3;
            DistanceFunctionName = DistanceFunctionTypes.EuclideanDistance;
            HierarchicalClusteringMethodName = UpdateDistanceMethodsTypes.Average;
            ProfileAlignerName = ProfileAlignerNames.NeedlemanWunschProfileAligner;
            ProfileProfileFunctionName = ProfileScoreFunctionNames.WeightedInnerProduct;

            // SimularityMatrix will be assigned in Align if it's not set prior through property.
        }

        /// <summary>
        /// Construct an aligner and run the alignment.
        /// </summary>
        /// <param name="sequences">input sequences</param>
        /// <param name="kmerLength">positive integer of kmer length</param>
        /// <param name="distanceFunctionName">enum: distance function name</param>
        /// <param name="hierarchicalClusteringMethodName">enum: cluster update method</param>
        /// <param name="profileAlignerMethodName">enum: profile-profile aligner name</param>
        /// <param name="profileFunctionName">enum: profile-profile distance function</param>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="gapOpenPenalty">negative gapOpenPenalty</param>
        /// <param name="gapExtendPenalty">negative gapExtendPenalty</param>
        /// <param name="numberOfPartitions">the number of partitions in dynamic programming</param>
        /// <param name="degreeOfParallelism">degree of parallelism option for parallel extension</param>
        public PAMSAMMultipleSequenceAligner(
                    IList<ISequence> sequences,
                    int kmerLength,
                    DistanceFunctionTypes distanceFunctionName,
                    UpdateDistanceMethodsTypes hierarchicalClusteringMethodName,
                    ProfileAlignerNames profileAlignerMethodName,
                    ProfileScoreFunctionNames profileFunctionName,
                    SimilarityMatrix similarityMatrix,
                    int gapOpenPenalty,
                    int gapExtendPenalty,
                    int numberOfPartitions,
                    int degreeOfParallelism)
        {
            AlignmentScoreC = float.MinValue;
            AlignmentScoreB = float.MinValue;
            AlignmentScoreA = float.MinValue;
            AlignmentScore = float.MinValue;
            StartLog();

            if (null == sequences)
            {
                throw new ArgumentNullException("sequences");
            }

            if (sequences.Count == 0)
            {
                throw new ArgumentException("Empty input sequences");
            }

            // Set parallel extension option
            if (degreeOfParallelism <= 0)
            {
                throw new ArgumentException("Invalid parallel degree parameter");
            }

            this.degreeOfParallelism = degreeOfParallelism;
            ParallelOption = new ParallelOptions { MaxDegreeOfParallelism = degreeOfParallelism };

            if (numberOfPartitions <= 0)
            {
                throw new ArgumentException("Invalid number of partition parameter");
            }
            this.numberOfPartitions = numberOfPartitions;

            // Assign the alphabet
            SetAlphabet(sequences, similarityMatrix, false);

            // Initialize parameters
            KmerLength = kmerLength;
            DistanceFunctionName = distanceFunctionName;
            HierarchicalClusteringMethodName = hierarchicalClusteringMethodName;
            ProfileAlignerName = profileAlignerMethodName;
            ProfileProfileFunctionName = profileFunctionName;
            SimilarityMatrix = similarityMatrix;
            GapOpenCost = gapOpenPenalty;
            GapExtensionCost = gapExtendPenalty;

            MsaUtils.SetProfileItemSets(this.alphabet);

            ReportLog("Start Aligning");

            // Work...
            DoAlignment(sequences);
        }

        /// <summary>
        /// Stop the current timer and report status changed.
        /// </summary>
        /// <param name="text"></param>
        private void ReportLog(string text)
        {
            StatusChanged(string.Format("{0}:\t{1}", currentReportLog.Elapsed, text));
        }

        /// <summary>
        /// Start the log timer.
        /// </summary>
        private void StartLog()
        {
            currentReportLog = Stopwatch.StartNew();
        }

        /// <summary>
        /// currently not implemented
        /// </summary>
        /// <param name="inputSequences"></param>
        /// <returns></returns>
        public IList<Alignment.ISequenceAlignment> AlignSimple(IEnumerable<ISequence> inputSequences)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs Stage 1, 2, and 3 as described in class description.
        /// </summary>
        /// <param name="inputSequences">Input sequences</param>
        /// <returns>Alignment results</returns>
        public IList<Alignment.ISequenceAlignment> Align(IEnumerable<ISequence> inputSequences)
        {
            // Reset all our data in case this same instance is used multiple times.
            this.AlignedSequences = this.AlignedSequencesA = this.AlignedSequencesB = this.AlignedSequencesC = null;
            this.AlignmentScore = this.AlignmentScoreA = this.AlignmentScoreB = this.AlignmentScoreC = float.MinValue;

            // Get our list of sequences.
            List<ISequence> sequences = inputSequences.ToList();
            if (sequences.Count == 0)
            {
                throw new ArgumentException("Empty input sequences");
            }

            // Assign the gap open/extension cost if it hasn't been assigned.
            if (GapOpenCost == 0)
                GapOpenCost = -4;
            if (GapExtensionCost == 0)
                GapExtensionCost = -1;

            StartLog();

            // Assign the alphabet
            SetAlphabet(sequences, SimilarityMatrix, true);
            MsaUtils.SetProfileItemSets(this.alphabet);

            ReportLog("Start Aligning");

            // Work...
            DoAlignment(sequences);

            // just for the purpose of integrating PW and MSA with the same output
            var alignment = new Alignment.SequenceAlignment();
            IAlignedSequence aSequence = new AlignedSequence();
            foreach (var alignedSequence in AlignedSequences)
                aSequence.Sequences.Add(alignedSequence);
            foreach (var inputSequence in sequences)
                alignment.Sequences.Add(inputSequence);
            alignment.AlignedSequences.Add(aSequence);
            return new List<Alignment.ISequenceAlignment>() {alignment};
        }


        /// <summary>
        /// This method assigns the alphabet from the input sequences
        /// </summary>
        /// <param name="sequences">Input sequences</param>
        /// <param name="similarityMatrix">Matrix to use for similarity comparisons</param>
        /// <param name="fixSimilarityMatrixErrors">True to fix any similarity matrix issue related to the alphabet.</param>
        private void SetAlphabet(IList<ISequence> sequences, SimilarityMatrix similarityMatrix, bool fixSimilarityMatrixErrors)
        {
            if (sequences.Count == 0)
            {
                throw new ArgumentException("Empty input sequences");
            }

            // Validate data type
            this.alphabet = Alphabets.GetAmbiguousAlphabet(sequences[0].Alphabet);
            Parallel.For(1, sequences.Count, ParallelOption, i =>
            {
                if (!Alphabets.CheckIsFromSameBase(sequences[i].Alphabet, this.alphabet))
                {
                    throw new ArgumentException("Inconsistent sequence alphabet");
                }
            });

            SimilarityMatrix bestSimilarityMatrix = null;

            if (this.alphabet is DnaAlphabet)
            {
                bestSimilarityMatrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.AmbiguousDna);
            }
            else if (this.alphabet is RnaAlphabet)
            {
                bestSimilarityMatrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.AmbiguousRna);
            }
            else if (this.alphabet is ProteinAlphabet)
            {
                bestSimilarityMatrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.Blosum50);
            }

            // Check or assign the similarity matrix.
            if (similarityMatrix == null)
            {
                SimilarityMatrix = bestSimilarityMatrix;
                if (SimilarityMatrix == null)
                    throw new ArgumentException("Unknown alphabet - could not choose SimilarityMatrix.");
            }
            else
            {
                var similarityMatrixDNA = new List<String> { "AmbiguousDNA" };
                var similarityMatrixRNA = new List<String> { "AmbiguousRNA" };
                var similarityMatrixProtein = new List<String> { "BLOSUM45", "BLOSUM50", "BLOSUM62", "BLOSUM80", "BLOSUM90", "PAM250", "PAM30", "PAM70" };

                if (this.alphabet is DnaAlphabet)
                {
                    if (!similarityMatrixDNA.Contains(similarityMatrix.Name))
                    {
                        if (fixSimilarityMatrixErrors)
                            SimilarityMatrix = bestSimilarityMatrix;
                        else
                            throw new ArgumentException("Inappropriate Similarity Matrix for DNA.");
                    }
                }
                else if (this.alphabet is ProteinAlphabet)
                {
                    if (!similarityMatrixProtein.Contains(similarityMatrix.Name))
                    {
                        if (fixSimilarityMatrixErrors)
                            SimilarityMatrix = bestSimilarityMatrix;
                        else
                            throw new ArgumentException("Inappropriate Similarity Matrix for Protein.");
                    }
                }
                else if (this.alphabet is RnaAlphabet)
                {
                    if (!similarityMatrixRNA.Contains(similarityMatrix.Name))
                    {
                        if (fixSimilarityMatrixErrors)
                            SimilarityMatrix = bestSimilarityMatrix;
                        else
                            throw new ArgumentException("Inappropriate Similarity Matrix for RNA.");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid alphabet");
                }
            }
        }

        /// <summary>
        /// Performs Stage 1, 2, and 3 as described in class description.
        /// </summary>
        /// <param name="sequences">Input sequences</param>
        /// <returns>Alignment results</returns>
        private void DoAlignment(IList<ISequence> sequences)
        {
            Debug.Assert(this.alphabet != null);
            Debug.Assert(sequences.Count > 0);

            // Initializations
            if (ConsensusResolver == null)
                ConsensusResolver = new SimpleConsensusResolver(this.alphabet);
            else
                ConsensusResolver.SequenceAlphabet = this.alphabet;

            // Get ProfileAligner ready
            IProfileAligner profileAligner = null;
            switch (ProfileAlignerName)
            {
                case (ProfileAlignerNames.NeedlemanWunschProfileAligner):
                    if (this.degreeOfParallelism == 1)
                    {
                        profileAligner = new NeedlemanWunschProfileAlignerSerial(
                            SimilarityMatrix, ProfileProfileFunctionName, GapOpenCost, GapExtensionCost, this.numberOfPartitions);
                    }
                    else
                    {
                        profileAligner = new NeedlemanWunschProfileAlignerParallel(
                            SimilarityMatrix, ProfileProfileFunctionName, GapOpenCost, GapExtensionCost, this.numberOfPartitions);
                    }
                    break;
                case (ProfileAlignerNames.SmithWatermanProfileAligner):
                    if (this.degreeOfParallelism == 1)
                    {
                        profileAligner = new SmithWatermanProfileAlignerSerial(
                        SimilarityMatrix, ProfileProfileFunctionName, GapOpenCost, GapExtensionCost, this.numberOfPartitions);

                    }
                    else
                    {
                        profileAligner = new SmithWatermanProfileAlignerParallel(
                    SimilarityMatrix, ProfileProfileFunctionName, GapOpenCost, GapExtensionCost, this.numberOfPartitions);

                    }
                    break;
                default:
                    throw new ArgumentException("Invalid profile aligner name");
            }

            this.AlignedSequences = new List<ISequence>(sequences.Count);
            float currentScore = 0;

            // STAGE 1

            ReportLog("Stage 1");
            // Generate DistanceMatrix
            var kmerDistanceMatrixGenerator = new KmerDistanceMatrixGenerator(sequences, KmerLength, this.alphabet, DistanceFunctionName);

            // Hierarchical clustering
            IHierarchicalClustering hierarcicalClustering =
                new HierarchicalClusteringParallel
                    (kmerDistanceMatrixGenerator.DistanceMatrix, HierarchicalClusteringMethodName);

            // Generate Guide Tree
            var binaryGuideTree = new BinaryGuideTree(hierarcicalClustering);

            // Progressive Alignment
            IProgressiveAligner progressiveAlignerA = new ProgressiveAligner(profileAligner);
            progressiveAlignerA.Align(sequences, binaryGuideTree);

            currentScore = MsaUtils.MultipleAlignmentScoreFunction(progressiveAlignerA.AlignedSequences, SimilarityMatrix, GapOpenCost, GapExtensionCost);
            if (currentScore > this.AlignmentScoreA)
            {
                this.AlignmentScoreA = currentScore;
                this.AlignedSequencesA = progressiveAlignerA.AlignedSequences;
            }
            if (this.AlignmentScoreA > this.AlignmentScore)
            {
                this.AlignmentScore = this.AlignmentScoreA;
                this.AlignedSequences = this.AlignedSequencesA;
            }

            if (PAMSAMMultipleSequenceAligner.FasterVersion)
            {
                this.AlignedSequencesB = this.AlignedSequencesA;
                this.AlignedSequencesC = this.AlignedSequencesA;
                this.AlignmentScoreB = this.AlignmentScoreA;
                this.AlignmentScoreC = this.AlignmentScoreA;
            }
            else
            {
                BinaryGuideTree binaryGuideTreeB = null;
                IHierarchicalClustering hierarcicalClusteringB = null;
                KimuraDistanceMatrixGenerator kimuraDistanceMatrixGenerator = new KimuraDistanceMatrixGenerator();

                if (UseStageB)
                {
                    // STAGE 2
                    ReportLog("Stage 2");
                    // Generate DistanceMatrix from Multiple Sequence Alignment

                    while (true)
                    {
                        kimuraDistanceMatrixGenerator.GenerateDistanceMatrix(this.AlignedSequences);

                        // Hierarchical clustering
                        hierarcicalClusteringB = new HierarchicalClusteringParallel
                                (kimuraDistanceMatrixGenerator.DistanceMatrix, HierarchicalClusteringMethodName);

                        // Generate Guide Tree
                        binaryGuideTreeB = new BinaryGuideTree(hierarcicalClusteringB);

                        BinaryGuideTree.CompareTwoTrees(binaryGuideTreeB, binaryGuideTree);
                        binaryGuideTree = binaryGuideTreeB;

                        // Progressive Alignment
                        IProgressiveAligner progressiveAlignerB = new ProgressiveAligner(profileAligner);
                        progressiveAlignerB.Align(sequences, binaryGuideTreeB);

                        currentScore = MsaUtils.MultipleAlignmentScoreFunction(progressiveAlignerB.AlignedSequences, SimilarityMatrix, GapOpenCost, GapExtensionCost);

                        if (currentScore > this.AlignmentScoreB)
                        {
                            this.AlignmentScoreB = currentScore;
                            this.AlignedSequencesB = progressiveAlignerB.AlignedSequences;
                        }
                        break;
                    }
                    if (this.AlignmentScoreB > this.AlignmentScore)
                    {
                        this.AlignmentScore = this.AlignmentScoreB;
                        this.AlignedSequences = this.AlignedSequencesB;
                    }
                }
                else
                {
                    binaryGuideTreeB = binaryGuideTree;
                }


                // STAGE 3
                ReportLog("Stage 3");
                // refinement
                int maxRefineMentTime = 1;
                if (sequences.Count == 2)
                {
                    maxRefineMentTime = 0;
                }

                int refinementTime = 0;
                this.AlignedSequencesC = new List<ISequence>(this.AlignedSequences.Count);
                foreach (ISequence t in this.AlignedSequences)
                {
                    this.AlignedSequencesC.Add(new Sequence(Alphabets.GetAmbiguousAlphabet(this.alphabet), t.ToArray())
                        {
                            ID = t.ID,
                            // Do not shallow copy dictionary
                            //Metadata = t.Metadata
                        });
                }

                while (refinementTime < maxRefineMentTime)
                {
                    ++refinementTime;
                    ReportLog("Refinement iter " + refinementTime);
                    bool needRefinement = false;
                    for (int edgeIndex = 0; edgeIndex < binaryGuideTreeB.NumberOfEdges; ++edgeIndex)
                    {
                        List<int>[] leafNodeIndices = binaryGuideTreeB.SeparateSequencesByCuttingTree(edgeIndex);

                        List<int>[] allIndelPositions = new List<int>[2];

                        IProfileAlignment[] separatedProfileAlignments = ProfileAlignment.ProfileExtraction(this.AlignedSequencesC, leafNodeIndices[0], leafNodeIndices[1], out allIndelPositions);
                        List<int>[] eStrings = new List<int>[2];

                        if (separatedProfileAlignments[0].NumberOfSequences < separatedProfileAlignments[1].NumberOfSequences)
                        {
                            profileAligner.Align(separatedProfileAlignments[0], separatedProfileAlignments[1]);
                            eStrings[0] = profileAligner.GenerateEString(profileAligner.AlignedA);
                            eStrings[1] = profileAligner.GenerateEString(profileAligner.AlignedB);
                        }
                        else
                        {
                            profileAligner.Align(separatedProfileAlignments[1], separatedProfileAlignments[0]);
                            eStrings[0] = profileAligner.GenerateEString(profileAligner.AlignedB);
                            eStrings[1] = profileAligner.GenerateEString(profileAligner.AlignedA);
                        }

                        for (int set = 0; set < 2; ++set)
                        {
                            Parallel.ForEach(leafNodeIndices[set], ParallelOption, i =>
                            {
                                //Sequence seq = new Sequence(_alphabet, "");
                                List<byte> seqBytes = new List<byte>();

                                int indexAllIndel = 0;
                                for (int j = 0; j < this.AlignedSequencesC[i].Count; ++j)
                                {
                                    if (indexAllIndel < allIndelPositions[set].Count && j == allIndelPositions[set][indexAllIndel])
                                    {
                                        ++indexAllIndel;
                                    }
                                    else
                                    {
                                        seqBytes.Add(this.AlignedSequencesC[i][j]);
                                    }
                                }

                                this.AlignedSequencesC[i] = profileAligner.GenerateSequenceFromEString(eStrings[set], new Sequence(Alphabets.GetAmbiguousAlphabet(this.alphabet), seqBytes.ToArray()));
                                this.AlignedSequencesC[i].ID = this.AlignedSequencesC[i].ID;
                                // Do not shallow copy dictionary
                                //(_alignedSequencesC[i] as Sequence).Metadata = _alignedSequencesC[i].Metadata;
                            });
                        }

                        currentScore = MsaUtils.MultipleAlignmentScoreFunction(this.AlignedSequencesC, SimilarityMatrix, GapOpenCost, GapExtensionCost);

                        if (currentScore > this.AlignmentScoreC)
                        {
                            this.AlignmentScoreC = currentScore;
                            needRefinement = true;

                            // recreate the tree
                            kimuraDistanceMatrixGenerator.GenerateDistanceMatrix(this.AlignedSequencesC);
                            hierarcicalClusteringB = new HierarchicalClusteringParallel
                                    (kimuraDistanceMatrixGenerator.DistanceMatrix, HierarchicalClusteringMethodName);

                            binaryGuideTreeB = new BinaryGuideTree(hierarcicalClusteringB);
                            break;
                        }
                    }
                    if (!needRefinement)
                    {
                        refinementTime = maxRefineMentTime;
                        break;
                    }

                }
                if (this.AlignmentScoreC > this.AlignmentScore)
                {
                    this.AlignmentScore = this.AlignmentScoreC;
                    this.AlignedSequences = this.AlignedSequencesC;
                }
                ReportLog("Stop Stage 3");
            }
        }
    }
}
