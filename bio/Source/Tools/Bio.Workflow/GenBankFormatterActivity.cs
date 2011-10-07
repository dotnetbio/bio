using System.Collections.Generic;
using System.ComponentModel;
using System.Workflow.ComponentModel;
using Bio.IO.GenBank;
using Microsoft.Research.ScientificWorkflow;

namespace Bio.Workflow
{
    /// <summary>
    /// Writes a sequence and/or list of sequences out to a specified file using the GenBank file format.
    /// </summary>
    [Name("GenBank File Formatter")]
    [Description("Writes a sequence and/or list of sequences out to a specified file using the GenBank file format.")]
    [WorkflowCategory("Bioinformatics")]
    public class GenBankFormatterActivity : Activity
    {
        #region Dependency Properties
        /// <summary>
        /// A file path specifying the location of the output file.
        /// </summary>
        public static DependencyProperty OutputFileProperty =
            DependencyProperty.Register("OutputFile", typeof(string),
            typeof(GenBankFormatterActivity));
        
        /// <summary>
        /// A file path specifying the location of the output file.
        /// </summary>
        [RequiredInputParam]
        [Name("Ouput file")]
        [Description(@"A file path specifying the location of the output file.")]
        public string OutputFile
        {
            get { return ((string)(base.GetValue(GenBankFormatterActivity.OutputFileProperty))); }
            set { base.SetValue(GenBankFormatterActivity.OutputFileProperty, value); }
        }
        
        /// <summary>
        /// An individual sequence to write.
        /// </summary>
        public static DependencyProperty SequenceProperty =
            DependencyProperty.Register("Sequence", typeof(ISequence),
            typeof(GenBankFormatterActivity));
        
        /// <summary>
        /// An individual sequence to write.
        /// </summary>
        [InputParam]
        [Name("Sequence")]
        [Description(@"An individual sequence to write.")]
        public ISequence Sequence
        {
            get { return ((ISequence)(base.GetValue(GenBankFormatterActivity.SequenceProperty))); }
            set { base.SetValue(GenBankFormatterActivity.SequenceProperty, value); }
        }

        /// <summary>
        /// A list of sequences to write.
        /// </summary>
        public static DependencyProperty SequenceListProperty =
            DependencyProperty.Register("SequenceList", typeof(IList<ISequence>), 
            typeof(GenBankFormatterActivity));

        /// <summary>
        /// A list of sequences to write.
        /// </summary>
        [InputParam]
        [Name("Sequence List")]
        [Description("A list of sequences to write.")]
        public IList<ISequence> SequenceList
        {
            get { return ((IList<ISequence>)(base.GetValue(GenBankFormatterActivity.SequenceListProperty))); }
            set { base.SetValue(GenBankFormatterActivity.SequenceListProperty, value); }
        }

        #endregion

        /// <summary>
        /// The execution method for the activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <returns>The execution status.</returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            GenBankFormatter formatter = new GenBankFormatter();
            formatter.Open(OutputFile);
            if ((Sequence == null) && (SequenceList != null))
            {
                foreach(ISequence sequence in SequenceList)
                {
                    formatter.Write(sequence);
                }
            }
            else if ((Sequence != null) && (SequenceList == null))
            {
                formatter.Write(Sequence);
            }
            else if ((Sequence != null) && (SequenceList != null))
            {
                foreach (ISequence sequence in SequenceList)
                {
                    formatter.Write(sequence);
                }

                formatter.Write(Sequence);
            }

            formatter.Close();
            return ActivityExecutionStatus.Closed;
        }
    }
}
