using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Workflow.ComponentModel;
using Bio.IO.GenBank;
using Microsoft.Research.ScientificWorkflow;

namespace Bio.Workflow
{
    /// <summary>
    /// This activity takes in an input file and produces as a result a set of sequence objects.
    /// </summary>
    [Name("GenBank Parser")]
    [Description("This activity should take in an input file and produce as a result a set of sequence objects. It should be left generic as to the file format, choosing the appropriate format based on the file extension.")]
    [WorkflowCategory("Bioinformatics")]
    public class GenBankParserActivity : Activity
    {
        #region Dependency Properties
        /// <summary>
        /// The path to the GenBank formatted file to read in.
        /// </summary>
        public static DependencyProperty InputFileProperty =
            DependencyProperty.Register("InputFile", typeof(string),
            typeof(GenBankParserActivity));

        /// <summary>
        /// The path to the GenBank formatted file to read in.
        /// </summary>
        [RequiredInputParam]
        [Name("Input File")]
        [Description(@"The path to the GenBank formatted file to read in.")]
        public string InputFile
        {
            get
            {
                return ((string)(base.GetValue(GenBankParserActivity.InputFileProperty)));
            }
            set
            {
                base.SetValue(GenBankParserActivity.InputFileProperty, value);
            }
        }

        /// <summary>
        /// A list of sequences found within the input file.
        /// </summary>
        public static DependencyProperty SequenceListProperty =
            DependencyProperty.Register("SequenceList", typeof(IList<ISequence>),
            typeof(GenBankParserActivity));

        /// <summary>
        /// A list of sequences found within the input file.
        /// </summary>
        [OutputParam]
        [Name("Sequence List")]
        [Description("A list of sequences found within the input file.")]
        public IList<ISequence> SequenceList
        {
            get
            {
                return ((IList<ISequence>)(base.GetValue(GenBankParserActivity.SequenceListProperty)));
            }
            set
            {
                base.SetValue(GenBankParserActivity.SequenceListProperty, value);
            }
        }
        #endregion

        /// <summary>
        /// The execution method for the activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <returns>The execution status.</returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            String inputFileName = InputFile;
            GenBankParser parser = new GenBankParser();
            parser.Open(inputFileName);
            SequenceList = parser.Parse().ToList();
            parser.Close();
            return ActivityExecutionStatus.Closed;
        }
    }
}
