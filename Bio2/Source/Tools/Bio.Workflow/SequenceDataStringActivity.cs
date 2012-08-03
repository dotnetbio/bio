using System.ComponentModel;
using System.Linq;
using System.Workflow.ComponentModel;
using Microsoft.Research.ScientificWorkflow;

namespace Bio.Workflow
{
    /// <summary>
    /// This activity produces a string version of the data of the sequence as well as access to the metadata of the sequence.
    /// </summary>
    [Name("Sequence Data Breakdown")]
    [Description("Given an input sequence, this activity produces a string version of the data of the sequence as well as access to the metadata of the sequence.")]
    [WorkflowCategory("Bioinformatics")]
    public class SequenceDataStringActivity : Activity
    {
        #region Dependency Properties
        /// <summary>
        /// The input sequence from which to extract the data and metadata.
        /// </summary>
        public static DependencyProperty SequenceProperty = 
            DependencyProperty.Register("Sequence", typeof(ISequence), 
            typeof(SequenceDataStringActivity));

        /// <summary>
        /// The input sequence from which to extract the data and metadata.
        /// </summary>
        [RequiredInputParam]
        [Name("Sequence")]
        [Description(@"The input sequence from which to extract the data and metadata.")]
        public ISequence Sequence
        {
            get { return ((ISequence)(base.GetValue(SequenceDataStringActivity.SequenceProperty)));}
            set { base.SetValue(SequenceDataStringActivity.SequenceProperty, value);}
        }

        /// <summary>
        /// An unformatted string representation of the data of the sequence.
        /// </summary>
        public static DependencyProperty SequenceDataProperty = 
            DependencyProperty.Register("SequenceData", typeof(string), 
            typeof(SequenceDataStringActivity));

        /// <summary>
        /// An unformatted string representation of the data of the sequence.
        /// </summary>
        [OutputParam]
        [Name("Sequence Data")]
        [Description(@"An unformatted string representation of the data of the sequence.")]
        public string SequenceData
        {
            get { return ((string)(base.GetValue(SequenceDataStringActivity.SequenceDataProperty))); }
            set { base.SetValue(SequenceDataStringActivity.SequenceDataProperty, value); }
        }

        /// <summary>
        /// The ID of the sequence.
        /// </summary>
        public static DependencyProperty IDProperty = 
            DependencyProperty.Register("ID", typeof(string), 
            typeof(SequenceDataStringActivity));

        /// <summary>
        /// The ID of the sequence.
        /// </summary>
        [OutputParam]
        [Name("ID")]
        [Description(@"The ID of the sequence.")]
        public string ID
        {
            get { return ((string)(base.GetValue(SequenceDataStringActivity.IDProperty))); }
            set { base.SetValue(SequenceDataStringActivity.IDProperty, value); }
        }

        /// <summary>
        /// The display ID of the sequence.
        /// </summary>
        public static DependencyProperty DisplayIDProperty = 
            DependencyProperty.Register("DisplayID", typeof(string), 
            typeof(SequenceDataStringActivity));

        /// <summary>
        /// The display ID of the sequence.
        /// </summary>
        [OutputParam]
        [Name("DisplayID")]
        [Description(@"The display ID of the sequence.")]
        public string DisplayID
        {
            get { return ((string)(base.GetValue(SequenceDataStringActivity.DisplayIDProperty))); }
            set { base.SetValue(SequenceDataStringActivity.DisplayIDProperty, value); }
        }

        /// <summary>
        /// The count statistics of the sequence data.
        /// </summary>
        public static DependencyProperty StatisticsProperty =
            DependencyProperty.Register("Statistics", typeof(SequenceStatistics),
            typeof(SequenceDataStringActivity));

        /// <summary>
        /// The count statistics of the sequence data.
        /// </summary>
        [OutputParam]
        [Name("Statistics")]
        [Description(@"The count statistics of the sequence data.")]
        public SequenceStatistics Statistics
        {
            get { return ((SequenceStatistics)(base.GetValue(SequenceDataStringActivity.StatisticsProperty))); }
            set { base.SetValue(SequenceDataStringActivity.StatisticsProperty, value); }
        }

        #endregion
        /// <summary>
        /// The execution method for the activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <returns>The execution status.</returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            SequenceData = new string(Sequence.ToArray().Select(a => (char)a).ToArray());
            ID = Sequence.ID;
            DisplayID = Sequence.ID;
            Statistics = new SequenceStatistics(Sequence);

            return ActivityExecutionStatus.Closed;
        }
    }
}
