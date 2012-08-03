using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Workflow.ComponentModel;
using Bio.IO.FastA;
using Microsoft.Research.ScientificWorkflow;

namespace Bio.Workflow
{
    /// <summary>
    /// Parses a specified input file according to the FASTA file format.
    /// </summary>
    [Name("FASTA Parser")]
    [Description("Parses a specified input file according to the FASTA file format.")]
    [WorkflowCategory("Bioinformatics")]
    public class FastaParserActivity : Activity
    {
        #region Dependency Properties
        /// <summary>
        /// The path to the FASTA formatted file to read in.
        /// </summary>
        public static DependencyProperty InputFileProperty = 
            DependencyProperty.Register("InputFile", typeof(string), 
            typeof(FastaParserActivity));

        /// <summary>
        /// The path to the FASTA formatted file to read in.
        /// </summary>
        [RequiredInputParam]
        [Name("Input File")]
        [Description(@"The path to the FASTA formatted file to read in.")]
        public string InputFile
        {
            get { return ((string)(base.GetValue(FastaParserActivity.InputFileProperty))); }
            set { base.SetValue(FastaParserActivity.InputFileProperty, value); }
        }

        /// <summary>
        /// A list of sequences found within the input file.
        /// </summary>
        public static DependencyProperty SequenceListProperty =
            DependencyProperty.Register("SequenceList", typeof(IList<ISequence>), 
            typeof(FastaParserActivity));

        /// <summary>
        /// A list of sequences found within the input file.
        /// </summary>
        [OutputParam]
        [Name("Sequence List")]
        [Description("A list of sequences found within the input file.")]
        public IList<ISequence> SequenceList
        {
            get { return ((IList<ISequence>)(base.GetValue(FastaParserActivity.SequenceListProperty))); }
            set { base.SetValue(FastaParserActivity.SequenceListProperty, value); }
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
            FastAParser parser = new FastAParser();
            parser.Open(inputFileName);
            SequenceList = parser.Parse().ToList();
            parser.Close();
            return ActivityExecutionStatus.Closed;
        }
    }
}
