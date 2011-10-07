using System.ComponentModel;
using System.Workflow.ComponentModel;
using Bio.Algorithms.Translation;
using Microsoft.Research.ScientificWorkflow;

namespace Bio.Workflow
{
    /// <summary>
    /// Given a DNA sequence, this activity produces the transcripted RNA sequence.
    /// </summary>
    [Name("DNA Transcription")]
    [Description("Given a DNA sequence, this activity produces the transcripted RNA sequence.")]
    [WorkflowCategory("Bioinformatics")]
    public class DnaTranscriptionActivity : Activity
    {
        #region Dependency Properties

        /// <summary>
        /// The DNA sequence to transcribe.
        /// </summary>
        public static DependencyProperty DnaInputProperty = 
            DependencyProperty.Register("DnaInput", typeof(ISequence), 
            typeof(DnaTranscriptionActivity));

        /// <summary>
        /// The DNA sequence to transcribe.
        /// </summary>
        [RequiredInputParam]
        [Name("DNA Sequence")]
        [Description(@"The DNA sequence to transcribe.")]
        public ISequence DnaInput
        {
            get { return ((ISequence)(base.GetValue(DnaTranscriptionActivity.DnaInputProperty))); }
            set { base.SetValue(DnaTranscriptionActivity.DnaInputProperty, value); }
        }

        /// <summary>
        /// The RNA sequence as an output.
        /// </summary>
        public static DependencyProperty RnaOutputProperty = 
            DependencyProperty.Register("RnaOutput", typeof(ISequence), 
            typeof(DnaTranscriptionActivity));

        /// <summary>
        /// The RNA sequence as an output.
        /// </summary>
        [OutputParam]
        [Name("RNA Sequence")]
        [Description(@"The RNA sequence as an output.")]
        public ISequence RnaOutput
        {
            get { return ((ISequence)(base.GetValue(DnaTranscriptionActivity.RnaOutputProperty))); }
            set { base.SetValue(DnaTranscriptionActivity.RnaOutputProperty, value); }
        }

        #endregion

        /// <summary>
        /// The execution method for the activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <returns>The execution status.</returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            RnaOutput = Transcription.Transcribe(DnaInput);
            return ActivityExecutionStatus.Closed;
        }
	}
}
