using System.ComponentModel;
using System.Workflow.ComponentModel;
using Microsoft.Research.ScientificWorkflow;

namespace Bio.Workflow
{
    /// <summary>
    /// Given a DNA or RNA, this activity produces the complement of that sequence.
    /// </summary>
    [Name("Nucleotide Complementation")]
    [Description("Given a DNA or RNA, this activity produces the complement of that sequence.")]
    [WorkflowCategory("Bioinformatics")]
    public class NucleotideComplementationActivity : Activity
    {
        #region Dependency Properties

        /// <summary>
        /// The DNA or RNA sequence as an input.
        /// </summary>
        public static DependencyProperty DnaOrRnaInputProperty = 
            DependencyProperty.Register("DnaOrRnaInput", typeof(ISequence), 
            typeof(NucleotideComplementationActivity));

        /// <summary>
        /// The DNA or RNA sequence as an input.
        /// </summary>
        [RequiredInputParam]
        [Name("Input DNA or RNA")]
        [Description(@"The DNA or RNA sequence as an input.")]
        public ISequence DnaOrRnaInput
        {
            get { return ((ISequence)(base.GetValue(NucleotideComplementationActivity.DnaOrRnaInputProperty))); }
            set { base.SetValue(NucleotideComplementationActivity.DnaOrRnaInputProperty, value); }
        }

        /// <summary>
        /// The DNA or RNA sequence as an output.
        /// </summary>
        public static DependencyProperty DnaOrRnaOutputProperty = 
            DependencyProperty.Register("DnaOrRnaOutput", typeof(ISequence), 
            typeof(NucleotideComplementationActivity));

        /// <summary>
        /// The DNA or RNA sequence as an output.
        /// </summary>
        [OutputParam]
        [Name("Ouput DNA or RNA")]
        [Description(@"The DNA or RNA sequence as an output.")]
        public ISequence DnaOrRnaOutput
        {
            get { return ((ISequence)(base.GetValue(NucleotideComplementationActivity.DnaOrRnaOutputProperty))); }
            set { base.SetValue(NucleotideComplementationActivity.DnaOrRnaOutputProperty, value); }
        }

        #endregion
        /// <summary>
        /// The execution method for the activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <returns>The execution status.</returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            DnaOrRnaOutput = DnaOrRnaInput.GetComplementedSequence();
            return ActivityExecutionStatus.Closed;
        }
    }
}
