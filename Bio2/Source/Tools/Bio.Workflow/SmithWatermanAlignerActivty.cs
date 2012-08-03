using System.Collections.Generic;
using System.ComponentModel;
using System.Workflow.ComponentModel;
using Bio.Algorithms.Alignment;
using Bio.SimilarityMatrices;
using Microsoft.Research.ScientificWorkflow;

namespace Bio.Workflow
{
    /// <summary>
    /// Given two input sequences, this activity creates an alignment of the two sequences.
    /// </summary>
    [Name("Smith-Waterman Aligner")]
    [Description("Given two input sequences, this activity creates an alignment of the two sequences.")]
    [WorkflowCategory("Bioinformatics")]
    public class SmithWatermanAlignerActivty : Activity
    {
        #region Dependency Properties

        /// <summary>
        /// The reference sequence to align against.
        /// </summary>
        public static DependencyProperty Sequence1Property =
            DependencyProperty.Register("Sequence1", typeof(ISequence),
            typeof(SmithWatermanAlignerActivty));

        /// <summary>
        /// The reference sequence to align against.
        /// </summary>
        [RequiredInputParam]
        [Name("First Sequence")]
        [Description(@"The reference sequence to align against.")]
        public ISequence Sequence1
        {
            get { return ((ISequence)(base.GetValue(SmithWatermanAlignerActivty.Sequence1Property))); }
            set { base.SetValue(SmithWatermanAlignerActivty.Sequence1Property, value); }
        }

        /// <summary>
        /// The sequence to align.
        /// </summary>
        public static DependencyProperty Sequence2Property =
            DependencyProperty.Register("Sequence2", typeof(ISequence),
            typeof(SmithWatermanAlignerActivty));

        /// <summary>
        /// The sequence to align.
        /// </summary>
        [RequiredInputParam]
        [Name("Second Sequence")]
        [Description(@"The sequence to align.")]
        public ISequence Sequence2
        {
            get { return ((ISequence)(base.GetValue(SmithWatermanAlignerActivty.Sequence2Property))); }
            set { base.SetValue(SmithWatermanAlignerActivty.Sequence2Property, value); }
        }

        /// <summary>
        /// The similarity matrix used to manage comparison costs.
        /// </summary>
        public static DependencyProperty SimilarityMatrixProperty =
            DependencyProperty.Register("SimilarityMatrix", typeof(SimilarityMatrix),
            typeof(SmithWatermanAlignerActivty));

        /// <summary>
        /// The similarity matrix used to manage comparison costs.
        /// </summary>
        [InputParam]
        [Name("Similarity Matrix")]
        [Description(@"The similarity matrix used to manage comparison costs.")]
        public SimilarityMatrix SimilarityMatrix
        {
            get { return ((SimilarityMatrix)(base.GetValue(SmithWatermanAlignerActivty.SimilarityMatrixProperty))); }
            set { base.SetValue(SmithWatermanAlignerActivty.SimilarityMatrixProperty, value); }
        }

        /// <summary>
        /// The penalty for adding gaps during alignment.
        /// </summary>
        public static DependencyProperty GapPenaltyProperty =
            DependencyProperty.Register("GapPenalty", typeof(int),
            typeof(SmithWatermanAlignerActivty),
            new PropertyMetadata(-2));

        /// <summary>
        /// The penalty for adding gaps during alignment.
        /// </summary>
        [InputParam]
        [Name("Gap Penalty")]
        [Description(@"The penalty for adding gaps during alignment.")]
        public int GapPenalty
        {
            get { return ((int)(base.GetValue(SmithWatermanAlignerActivty.GapPenaltyProperty))); }
            set { base.SetValue(SmithWatermanAlignerActivty.GapPenaltyProperty, value); }
        }

        /// <summary>
        /// The sequence alignment produced by running the algorithm.
        /// </summary>
        public static DependencyProperty ResultProperty =
            DependencyProperty.Register("Result", typeof(IList<IPairwiseSequenceAlignment>),
            typeof(SmithWatermanAlignerActivty));

        /// <summary>
        /// The sequence alignment produced by running the algorithm.
        /// </summary>
        [OutputParam]
        [Name("Alignment")]
        [Description(@"The sequence alignment produced by running the algorithm.")]
        public IList<IPairwiseSequenceAlignment> Result
        {
            get
            {
                return ((IList<IPairwiseSequenceAlignment>)(base.GetValue(SmithWatermanAlignerActivty.ResultProperty)));
            }
            set
            {
                base.SetValue(SmithWatermanAlignerActivty.ResultProperty, value);
            }
        }

        /// <summary>
        /// The consensus sequence from the alignment result.
        /// </summary>
        public static DependencyProperty ConsensusProperty =
            DependencyProperty.Register("Consensus", typeof(ISequence),
            typeof(SmithWatermanAlignerActivty));

        /// <summary>
        /// The consensus sequence from the alignment result.
        /// </summary>
        [OutputParam]
        [Name("Consensus")]
        [Description(@"The consensus sequence from the alignment result.")]
        public ISequence Consensus
        {
            get { return ((ISequence)(base.GetValue(SmithWatermanAlignerActivty.ConsensusProperty))); }
            set { base.SetValue(SmithWatermanAlignerActivty.ConsensusProperty, value); }
        }

        /// <summary>
        /// The first modified result sequence.
        /// </summary>
        public static DependencyProperty Result1Property =
            DependencyProperty.Register("Result1", typeof(ISequence),
            typeof(SmithWatermanAlignerActivty));

        /// <summary>
        /// The first modified result sequence.
        /// </summary>
        [OutputParam]
        [Name("First Result")]
        [Description(@"The first modified result sequence.")]
        public ISequence Result1
        {
            get { return ((ISequence)(base.GetValue(SmithWatermanAlignerActivty.Result1Property))); }
            set { base.SetValue(SmithWatermanAlignerActivty.Result1Property, value); }
        }

        /// <summary>
        /// The second modified result sequence.
        /// </summary>
        public static DependencyProperty Result2Property =
            DependencyProperty.Register("Result2", typeof(ISequence),
            typeof(SmithWatermanAlignerActivty));

        /// <summary>
        /// The second modified result sequence.
        /// </summary>
        [OutputParam]
        [Name("Second Result")]
        [Description(@"The second modified result sequence.")]
        public ISequence Result2
        {
            get { return ((ISequence)(base.GetValue(SmithWatermanAlignerActivty.Result2Property))); }
            set { base.SetValue(SmithWatermanAlignerActivty.Result2Property, value); }
        }

        #endregion
        /// <summary>
        /// The execution method for the activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <returns>The execution status.</returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            SmithWatermanAligner SWA = new SmithWatermanAligner();
            Result = SWA.AlignSimple(SimilarityMatrix, GapPenalty, Sequence1, Sequence2);
            if (Result.Count >= 1 && Result[0].PairwiseAlignedSequences.Count >= 1)
            {
                Result1 = Result[0].PairwiseAlignedSequences[0].FirstSequence;
                Result2 = Result[0].PairwiseAlignedSequences[0].SecondSequence;
                Consensus = Result[0].PairwiseAlignedSequences[0].Consensus;
            }

            return ActivityExecutionStatus.Closed;
        }
    }
}
