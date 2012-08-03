using System.ComponentModel;
using System.Workflow.ComponentModel;
using Microsoft.Research.ScientificWorkflow;

namespace Bio.Workflow
{
    /// <summary>
    /// This activity performs a quick comparison to see if two formatted sequences are the same or not.
    /// </summary>
    [Name("Sequence Comparison")]
    [Description("Performs a quick comparison to see if two formatted sequences are the same or not")]
    [WorkflowCategory("Bioinformatics")]
    public class SequenceComparisonActivity : Activity
    {
        #region Dependency Properties

        /// <summary>
        /// The first sequence to compare.
        /// </summary>
        public static DependencyProperty Sequence1Property = 
            DependencyProperty.Register("Sequence1", typeof(ISequence), 
            typeof(SequenceComparisonActivity));

        /// <summary>
        /// The first sequence to compare.
        /// </summary>
        [RequiredInputParam]
        [Name("First Sequence")]
        [Description(@"The first sequence to compare.")]
        public ISequence Sequence1
        {
            get { return ((ISequence)(base.GetValue(SequenceComparisonActivity.Sequence1Property))); }
            set { base.SetValue(SequenceComparisonActivity.Sequence1Property, value); }
        }

        /// <summary>
        /// The second sequence to compare.
        /// </summary>
        public static DependencyProperty Sequence2Property = 
            DependencyProperty.Register("Sequence2", typeof(ISequence), 
            typeof(SequenceComparisonActivity));

        /// <summary>
        /// The second sequence to compare.
        /// </summary>
        [RequiredInputParam]
        [Name("Second Sequence")]
        [Description(@"The second sequence to compare.")]
        public ISequence Sequence2
        {
            get { return ((ISequence)(base.GetValue(SequenceComparisonActivity.Sequence2Property))); }
            set { base.SetValue(SequenceComparisonActivity.Sequence2Property, value); }
        }

        /// <summary>
        /// An indication as to whether or not the sequences are the same.
        /// </summary>
        public static DependencyProperty AreEqualProperty = 
            DependencyProperty.Register("AreEqual", typeof(bool),
            typeof(SequenceComparisonActivity));

        /// <summary>
        /// An indication as to whether or not the sequences are the same.
        /// </summary>
        [OutputParam]
        [Name("Are Equal")]
        [Description(@"An indication as to whether or not the sequences are the same.")]
        public bool AreEqual
        {
            get { return ((bool)(base.GetValue(SequenceComparisonActivity.AreEqualProperty))); }
            set { base.SetValue(SequenceComparisonActivity.AreEqualProperty, value); }
        }

        #endregion

        /// <summary>
        /// The execution method for the activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <returns>The execution status.</returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            AreEqual = true;
            if (Sequence1.Alphabet != Sequence2.Alphabet)
            {
                AreEqual = false;
                return ActivityExecutionStatus.Closed;
            }

            if (Sequence1.Count != Sequence2.Count)
            {
                AreEqual = false;
                return ActivityExecutionStatus.Closed;
            }

            for (int i = 0; i < Sequence1.Count; i++ )
            {
                if (Sequence1[i] != Sequence2[i])
                {
                    AreEqual = false;
                    return ActivityExecutionStatus.Closed;
                }
            }

            AreEqual = true;
            return ActivityExecutionStatus.Closed;
        }
    }
}
