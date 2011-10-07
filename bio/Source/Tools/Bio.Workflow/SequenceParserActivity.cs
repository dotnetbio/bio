using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Workflow.ComponentModel;
using Bio.IO;
using Microsoft.Research.ScientificWorkflow;

namespace Bio.Workflow
{
    /// <summary>
    /// Parses any sequence file of a known format and file extension and returns all sequences found in that file.
    /// </summary>
    [Name("Sequence Parser")]
    [Description("Parses any sequence file of a known format and file extension and returns all sequences found in that file.")]
    [WorkflowCategory("Bioinformatics")]
    public class SequenceParserActivity : Activity
    {
        #region Dependency Properties

        /// <summary>
        /// The path of the input sequence file.
        /// </summary>
        public static DependencyProperty InputFileProperty = 
            DependencyProperty.Register("InputFile", typeof(string), 
            typeof(SequenceParserActivity));

        /// <summary>
        /// The path of the input sequence file.
        /// </summary>
        [RequiredInputParam]
        [Name("Input File")]
        [Description(@"The path of the input sequence file.")]
        public string InputFile
        {
            get { return ((string)(base.GetValue(SequenceParserActivity.InputFileProperty))); }
            set { base.SetValue(SequenceParserActivity.InputFileProperty, value); }
        }

        /// <summary>
        /// List of sequence found in the input file.
        /// </summary>
        public static DependencyProperty ListSequenceResultProperty =
            DependencyProperty.Register("ListSequenceResult", typeof(IList<ISequence>), 
            typeof(SequenceParserActivity));

        /// <summary>
        /// List of sequence found in the input file.
        /// </summary>
        [OutputParam]
        [Name("Sequence List")]
        [Description("List of sequence found in the input file.")]
        public IList<ISequence> ListSequenceResult
        {
            get { return ((IList<ISequence>)(base.GetValue(SequenceParserActivity.ListSequenceResultProperty))); }
            set { base.SetValue(SequenceParserActivity.ListSequenceResultProperty, value); }
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
            ISequenceParser parser = SequenceParsers.FindParserByFileName(inputFileName);
            ListSequenceResult = parser.Parse().ToList();
            parser.Close();
            return ActivityExecutionStatus.Closed;
        }
    }
}
