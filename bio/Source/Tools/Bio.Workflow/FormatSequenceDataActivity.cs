using System.ComponentModel;
using System.Text;
using System.Workflow.ComponentModel;
using Microsoft.Research.ScientificWorkflow;

namespace Bio.Workflow
{
    /// <summary>
    /// Formats a sequence into a string that is human readable. 
    /// </summary>
    [Name("Format Sequence Data")]
    [Description("Formats a sequence into a string that is human readable. The string contains the display ID of the sequence, count statistics, and formatted data symbols.")]
    [WorkflowCategory("Bioinformatics")]
    public class FormatSequenceDataActivity : Activity
    {
        #region Dependency Properties

        /// <summary>
        /// The first sequence found in the input file.
        /// </summary>
        public static DependencyProperty SequenceProperty =
            DependencyProperty.Register("Sequence", typeof(ISequence),
            typeof(FormatSequenceDataActivity));

        /// <summary>
        /// The first sequence found in the input file.
        /// </summary>
        [RequiredInputParam]
        [Name("Sequence")]
        [Description("The first sequence found in the input file.")]
        public ISequence Sequence
        {
            get { return ((ISequence)(base.GetValue(FormatSequenceDataActivity.SequenceProperty))); }
            set { base.SetValue(FormatSequenceDataActivity.SequenceProperty, value); }
        }

        /// <summary>
        /// The formatted represenation of the sequence.
        /// </summary>
        public static DependencyProperty DataProperty = 
            DependencyProperty.Register("Data", typeof(string),
            typeof(FormatSequenceDataActivity));

        /// <summary>
        /// The formatted represenation of the sequence.
        /// </summary>
        [OutputParam]
        [Name("Formatted Data")]
        [Description(@"The formatted represenation of the sequence.")]
        public string Data
        {
            get { return ((string)(base.GetValue(FormatSequenceDataActivity.DataProperty))); }
            set { base.SetValue(FormatSequenceDataActivity.DataProperty, value); }
        }

        #endregion

        /// <summary>
        /// The execution method for the activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <returns>The execution status.</returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            StringBuilder buff = new StringBuilder();

            buff.AppendLine(Sequence.ID);
            buff.Append("Statistics: ");
            buff.Append(Sequence.Count);
            buff.Append(" Total");

            var statistics = new SequenceStatistics(Sequence);
            
            if (Sequence.Alphabet == Alphabets.DNA)
            {
                buff.Append(" - G: ");
                buff.Append(statistics.GetCount(Alphabets.DNA.G));
                buff.Append(" - A: ");
                buff.Append(statistics.GetCount(Alphabets.DNA.A));
                buff.Append(" - T: ");
                buff.Append(statistics.GetCount(Alphabets.DNA.T));
                buff.Append(" - C: ");
                buff.Append(statistics.GetCount(Alphabets.DNA.C));
            }
            else if (Sequence.Alphabet == Alphabets.RNA)
            {
                buff.Append(" - G: ");
                buff.Append(statistics.GetCount(Alphabets.RNA.G));
                buff.Append(" - A: ");
                buff.Append(statistics.GetCount(Alphabets.RNA.A));
                buff.Append(" - U: ");
                buff.Append(statistics.GetCount(Alphabets.RNA.U));
                buff.Append(" - C: ");
                buff.Append(statistics.GetCount(Alphabets.RNA.C));
            }

            buff.AppendLine();
            buff.AppendLine();

            for (int i = 0; i < Sequence.Count; i++)
            {
                if ((i % 50) == 0)
                {
                    string num = (i + 1).ToString();
                    int pad = 5 - num.Length;
                    StringBuilder buff2 = new StringBuilder();
                    for (int j = 0; j < pad; j++)
                        buff2.Append(' ');
                    buff2.Append(num);
                    buff.Append(buff2.ToString());
                }

                if ((i % 10) == 0)
                    buff.Append(' ');

                buff.Append((char)Sequence[i]);

                if ((i % 50) == 49)
                    buff.AppendLine();
            }
            buff.AppendLine();

            Data = buff.ToString();
                        
            return ActivityExecutionStatus.Closed;
        }
    }
}
