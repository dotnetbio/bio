using System.Collections.Generic;
using System.ComponentModel;
using System.Workflow.ComponentModel;
using Microsoft.Research.ScientificWorkflow;

namespace Bio.Workflow
{
    /// <summary>
    /// Fetches and returns sequence at the specified index from a list of sequences.
    /// </summary>
    [Name("Sequence List Indexer")]
    [Description("Fetches and returns sequence at the specified index from a list of sequences.")]
    [WorkflowCategory("Bioinformatics")]
    public class SequenceListIndexerActivity : Activity
    {
        #region Dependency Properties

        /// <summary>
        /// A list of sequences.
        /// </summary>
        public static DependencyProperty SequenceListProperty =
            DependencyProperty.Register("SequenceList", typeof(IList<ISequence>),
            typeof(SequenceListIndexerActivity));

        /// <summary>
        /// A list of sequences.
        /// </summary>
        [RequiredInputParam]
        [Name("Sequence List")]
        [Description("A list of sequences.")]
        public IList<ISequence> SequenceList
        {
            get { return ((IList<ISequence>)(base.GetValue(SequenceListIndexerActivity.SequenceListProperty))); }
            set { base.SetValue(SequenceListIndexerActivity.SequenceListProperty, value); }
        }

        /// <summary>
        /// Index of the item in the sequence list.
        /// </summary>
        public static DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int),
            typeof(SequenceListIndexerActivity));

        /// <summary>
        /// Index of the item in the sequence list.
        /// </summary>
        [RequiredInputParam]
        [Name("Index")]
        [Description("Index of the item in the sequence list.")]
        public int Index
        {
            get { return ((int)(base.GetValue(SequenceListIndexerActivity.IndexProperty))); }
            set { base.SetValue(SequenceListIndexerActivity.IndexProperty, value); }
        }

        /// <summary>
        /// Sequence at the specified index.
        /// </summary>
        public static DependencyProperty SequenceProperty =
            DependencyProperty.Register("Sequence", typeof(ISequence),
            typeof(SequenceListIndexerActivity));

        /// <summary>
        /// Sequence at the specified index.
        /// </summary>
        [OutputParam]
        [Name("Sequence")]
        [Description("Sequence at the specified index.")]
        public ISequence Sequence
        {
            get { return ((ISequence)(base.GetValue(SequenceListIndexerActivity.SequenceProperty))); }
            set { base.SetValue(SequenceListIndexerActivity.SequenceProperty, value); }
        }

        #endregion
        /// <summary>
        /// The execution method for the activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <returns>The execution status.</returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            // Fetch nth element and store it in local output variable.
            Sequence = SequenceList[Index];
            return ActivityExecutionStatus.Closed;
        }
    }
}
