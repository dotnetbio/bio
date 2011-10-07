using System.ComponentModel;
using System.Workflow.ComponentModel;
using Bio.Algorithms.Translation;
using Microsoft.Research.ScientificWorkflow;

namespace Bio.Workflow
{
    /// <summary>
    /// Translates a RNA sequence into a corresponding amino acid sequence for encoding proteins.
    /// </summary>
    [Name("Protein Translation")]
    [Description("Translates a RNA sequence into a corresponding amino acid sequence for encoding proteins.")]
    [WorkflowCategory("Bioinformatics")]
    public class ProteinTranslationActivity : Activity
    {
        #region Dependency Properties
        /// <summary>
        /// The RNA sequence to be translated.
        /// </summary>
        public static DependencyProperty RnaSequenceProperty =
            DependencyProperty.Register("RnaSequence", typeof(ISequence),
            typeof(ProteinTranslationActivity));

        /// <summary>
        /// The RNA sequence to be translated.
        /// </summary>
        [RequiredInputParam]
        [Name("RNA Sequence")]
        [Description(@"The RNA sequence to be translated.")]
        public ISequence RnaSequence
        {
            get { return ((ISequence)(base.GetValue(ProteinTranslationActivity.RnaSequenceProperty))); }
            set { base.SetValue(ProteinTranslationActivity.RnaSequenceProperty, value); }
        }

        /// <summary>
        /// Translation will start by skipping the number of base pairs specifed by this parameter in the input sequence.
        /// </summary>
        public static DependencyProperty initialSequenceOffsetProperty =
            DependencyProperty.Register("initialSequenceOffset", typeof(int),
            typeof(ProteinTranslationActivity), new PropertyMetadata((int)0));

        /// <summary>
        /// Translation will start by skipping the number of base pairs specifed by this parameter in the input sequence.
        /// </summary>
        [OptionalInputParam]
        [Name("Sequence Offset")]
        [Description(@"Translation will start by skipping the number of base pairs specifed by this parameter in the input sequence.")]
        public int initialSequenceOffset
        {
            get { return ((int)(base.GetValue(ProteinTranslationActivity.initialSequenceOffsetProperty))); }
            set { base.SetValue(ProteinTranslationActivity.initialSequenceOffsetProperty, value); }
        }

        /// <summary>
        /// The tranlsated protein sequence.
        /// </summary>
        public static DependencyProperty ProteinProperty =
            DependencyProperty.Register("Protein", typeof(ISequence),
            typeof(ProteinTranslationActivity));

        /// <summary>
        /// The tranlsated protein sequence.
        /// </summary>
        [OutputParam]
        [Name("Protein")]
        [Description(@"The tranlsated protein sequence.")]
        public ISequence Protein
        {
            get { return ((ISequence)(base.GetValue(ProteinTranslationActivity.ProteinProperty))); }
            set { base.SetValue(ProteinTranslationActivity.ProteinProperty, value); }
        }
        #endregion
        /// <summary>
        /// The execution method for the activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <returns>The execution status.</returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            Protein = ProteinTranslation.Translate(RnaSequence, initialSequenceOffset);
            return ActivityExecutionStatus.Closed;
        }
    }
}
