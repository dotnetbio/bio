using System;
using System.ComponentModel;
using System.Workflow.ComponentModel;

using Microsoft.Research.ScientificWorkflow;


namespace Bio.Workflow
{
    /// <summary>
    /// Allows the creation of a sequence by providing the sequence data directly.
    /// </summary>
    [Name("Sequence Creator")]
    [Description("Allows the creation of a sequence by providing the sequence data directly.")]
    [WorkflowCategory("Bioinformatics")]
    public class CreateSequenceActivity : Activity
    {
        #region Dependency Properties

        /// <summary>
        /// The alphabet of the sequence.
        /// </summary>
        public static DependencyProperty AlphabetNameProperty = 
            DependencyProperty.Register("AlphabetName", typeof(string), 
            typeof(CreateSequenceActivity));

        /// <summary>
        /// The alphabet of the sequence.
        /// </summary>
        [RequiredInputParam]
        [Name("Alphabet")]
        [Description(@"The alphabet of the sequence. Must be one of: DNA, RNA, or Protein.")]
        public string AlphabetName
        {
            get { return ((string)(base.GetValue(CreateSequenceActivity.AlphabetNameProperty))); }
            set { base.SetValue(CreateSequenceActivity.AlphabetNameProperty, value); }
        }

        /// <summary>
        /// The characters making up the sequence.
        /// </summary>
        public static DependencyProperty SequenceDataProperty =
            DependencyProperty.Register("SequenceData", typeof(string),
            typeof(CreateSequenceActivity));

        /// <summary>
        /// The characters making up the sequence.
        /// </summary>
        [RequiredInputParam]
        [Name("Sequence Data")]
        [Description(@"The characters making up the sequence (e.g. for DNA, 'GATTCCA').")]
        public string SequenceData
        {
            get { return ((string)(base.GetValue(CreateSequenceActivity.SequenceDataProperty))); }
            set { base.SetValue(CreateSequenceActivity.SequenceDataProperty, value); }
        }

        /// <summary>
        /// An internal identification of the sequence.
        /// </summary>
        public static DependencyProperty IDProperty =
            DependencyProperty.Register("ID", typeof(string),
            typeof(CreateSequenceActivity));

        /// <summary>
        /// An internal identification of the sequence.
        /// </summary>
        [OptionalInputParam]
        [Name("ID")]
        [Description(@"An internal identification of the sequence.")]
        public string ID
        {
            get { return ((string)(base.GetValue(CreateSequenceActivity.IDProperty))); }
            set { base.SetValue(CreateSequenceActivity.IDProperty, value); }
        }

        /// <summary>
        /// An human readable identification of the sequence.
        /// </summary>
        public static DependencyProperty DisplayIDProperty =
            DependencyProperty.Register("DisplayID", typeof(string),
            typeof(CreateSequenceActivity));

        /// <summary>
        /// An human readable identification of the sequence.
        /// </summary>
        [OptionalInputParam]
        [Name("Display ID")]
        [Description(@"A human readable identification of the sequence.")]
        public string DisplayID
        {
            get { return ((string)(base.GetValue(CreateSequenceActivity.DisplayIDProperty))); }
            set { base.SetValue(CreateSequenceActivity.DisplayIDProperty, value); }
        }

        /// <summary>
        /// The first sequence found in the input file.
        /// </summary>
        public static DependencyProperty SequenceResultProperty = 
            DependencyProperty.Register("SequenceResult", typeof(Sequence), 
            typeof(CreateSequenceActivity));

        /// <summary>
        /// The first sequence found in the input file.
        /// </summary>
        [OutputParam]
        [Name("Sequence")]
        [Description("The first sequence found in the input file.")]
        public Sequence SequenceResult
        {
            get { return ((Sequence)(base.GetValue(CreateSequenceActivity.SequenceResultProperty))); }
            set { base.SetValue(CreateSequenceActivity.SequenceResultProperty, value); }
        }

        #endregion

        /// <summary>
        /// The execution method for the activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <returns>The execution status.</returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            IAlphabet alphabet = Alphabets.DNA;
            if (AlphabetName.Equals("DNA", StringComparison.InvariantCultureIgnoreCase))
                alphabet = Alphabets.DNA;
            else if (AlphabetName.Equals("RNA", StringComparison.InvariantCultureIgnoreCase))
                alphabet = Alphabets.RNA;
            else if (AlphabetName.Equals("Protein", StringComparison.InvariantCultureIgnoreCase))
                alphabet = Alphabets.Protein;
            else
                throw new ArgumentException("Unknown alphabet name");

            SequenceResult = new Sequence(alphabet, SequenceData);
            SequenceResult.ID = ID;
            SequenceResult.ID = DisplayID;
            
            return ActivityExecutionStatus.Closed;
        }
    }
}
