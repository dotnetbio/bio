using System.ComponentModel;
using System.Workflow.ComponentModel;
using Bio.Algorithms.Translation;
using Microsoft.Research.ScientificWorkflow;

namespace Bio.Workflow
{
    /// <summary>
    /// This activity produces the reverse transcription DNA sequence.
    /// </summary>
    [Name("RNA Reverse Transcription")]
    [Description("Given an RNA sequence, this activity produces the reverse transcription DNA sequence.")]
    [WorkflowCategory("Bioinformatics")]
    public class RnaReverseTranscriptionActivty : Activity
    {
        #region Dependency Properties
        /// <summary>
        /// The RNA sequence to which reverse transcription will be applied.
        /// </summary>
        public static DependencyProperty RnaProperty =
            DependencyProperty.Register("Rna", typeof(ISequence),
            typeof(RnaReverseTranscriptionActivty));

        /// <summary>
        /// The RNA sequence to which reverse transcription will be applied.
        /// </summary>
        [RequiredInputParam]
        [Name("RNA")]
        [Description(@"The RNA sequence to which reverse transcription will be applied.")]
        public ISequence Rna
        {
            get { return ((ISequence)(base.GetValue(RnaReverseTranscriptionActivty.RnaProperty))); }
            set { base.SetValue(RnaReverseTranscriptionActivty.RnaProperty, value); }
        }

        /// <summary>
        /// The DNA sequence as an output.
        /// </summary>
        public static DependencyProperty DnaProperty =
            DependencyProperty.Register("Dna", typeof(ISequence),
            typeof(RnaReverseTranscriptionActivty));

        /// <summary>
        /// The DNA sequence as an output.
        /// </summary>
        [OutputParam]
        [Name("DnaOutput")]
        [Description(@"The DNA sequence as an output.")]
        public ISequence Dna
        {
            get { return ((ISequence)(base.GetValue(RnaReverseTranscriptionActivty.DnaProperty))); }
            set { base.SetValue(RnaReverseTranscriptionActivty.DnaProperty, value); }
        }
        #endregion
        /// <summary>
        /// The execution method for the activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <returns>The execution status.</returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            Dna = Transcription.ReverseTranscribe(Rna);
            return ActivityExecutionStatus.Closed;
        }
    }
}
