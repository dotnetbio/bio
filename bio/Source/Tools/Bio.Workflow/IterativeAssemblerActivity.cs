using System.Collections.Generic;
using System.ComponentModel;
using System.Workflow.ComponentModel;
using Bio.Algorithms.Assembly;
using Microsoft.Research.ScientificWorkflow;

namespace Bio.Workflow
{
    /// <summary>
    /// This activity creates an assembly object of the contigs and a consensus from running an assembly algorithm.
    /// </summary>
    [Name("Iterative Assembler")]
    [Description("Given a list of input sequences, this activity creates an assembly object of the contigs and a consensus from running an assembly algorithm.")]
    [WorkflowCategory("Bioinformatics")]
    public class IterativeAssemblerActivity : Activity
    {
        #region Dependency Properties

        /// <summary>
        /// A list of sequences to assemble.
        /// </summary>
        public static DependencyProperty SequencesProperty =
            DependencyProperty.Register("Sequences", typeof(IList<ISequence>), 
            typeof(IterativeAssemblerActivity));

        /// <summary>
        /// A list of sequences to assemble.
        /// </summary>
        [RequiredInputParam]
        [Name("Sequences")]
        [Description("A list of sequences to assemble.")]
        public IList<ISequence> Sequences
        {
            get { return ((IList<ISequence>)(base.GetValue(IterativeAssemblerActivity.SequencesProperty))); }
            set { base.SetValue(IterativeAssemblerActivity.SequencesProperty, value); }
        }

        /// <summary>
        /// A list of contigs produced by the assembly algorithm.
        /// </summary>
        public static DependencyProperty ContigsProperty = 
            DependencyProperty.Register("Contigs", typeof(List<Contig>), 
            typeof(IterativeAssemblerActivity));

        /// <summary>
        /// A list of contigs produced by the assembly algorithm.
        /// </summary>
        [OutputParam]
        [Name("Contigs")]
        [Description(@"A list of contigs produced by the assembly algorithm.")]
        public IList<Contig> Contigs
        {
            get { return ((IList<Contig>)(base.GetValue(IterativeAssemblerActivity.ContigsProperty))); }
            set { base.SetValue(IterativeAssemblerActivity.ContigsProperty, value); }
        }

        /// <summary>
        /// The result of ISequence or Consensus.
        /// </summary>
        public static DependencyProperty ConsensusProperty = 
            DependencyProperty.Register("Consensus", typeof(IDeNovoAssembly), 
            typeof(IterativeAssemblerActivity));

        /// <summary>
        /// The result of ISequence or Consensus.
        /// </summary>
        [OutputParam]
        [Name("Consensus")]
        [Description(@"The result of ISequence or Consensus.")]
        public IDeNovoAssembly Consensus
        {
            get { return ((IDeNovoAssembly)(base.GetValue(IterativeAssemblerActivity.ConsensusProperty))); }
            set { base.SetValue(IterativeAssemblerActivity.ConsensusProperty, value); }
        }

        #endregion

        /// <summary>
        /// The execution method for the activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <returns>The execution status.</returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            OverlapDeNovoAssembler SSA = new OverlapDeNovoAssembler();
            Consensus= SSA.Assemble(Sequences);
            Contigs = ((IOverlapDeNovoAssembly)Consensus).Contigs;

            return ActivityExecutionStatus.Closed;
        }
    }
}
