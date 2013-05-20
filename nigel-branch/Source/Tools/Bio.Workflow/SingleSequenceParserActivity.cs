using System;
using System.ComponentModel;
using System.Linq;
using System.Workflow.ComponentModel;
using Bio.IO;
using Microsoft.Research.ScientificWorkflow;

namespace Bio.Workflow
{
    /// <summary>
    /// Parses any sequence file of a known format and file extension and returns the first sequence found in that file.
    /// </summary>
    [Name("Single Sequence Parser")]
    [Description("Parses any sequence file of a known format and file extension and returns the first sequence found in that file.")]
    [WorkflowCategory("Bioinformatics")]
    public class SingleSequenceParserActivity : Activity
    {
        #region Dependency Properties
        /// <summary>
        /// The path of the input sequence file.
        /// </summary>
        public static DependencyProperty InputFileProperty = 
            DependencyProperty.Register("InputFile", typeof(string),
            typeof(SingleSequenceParserActivity));

        /// <summary>
        /// The path of the input sequence file.
        /// </summary>
        [RequiredInputParam]
        [Name("Input File")]
        [Description(@"The path of the input sequence file.")]
        public string InputFile
        {
            get { return ((string)(base.GetValue(SingleSequenceParserActivity.InputFileProperty))); }
            set { base.SetValue(SingleSequenceParserActivity.InputFileProperty, value); }
        }

        /// <summary>
        /// The first sequence found in the input file.
        /// </summary>
        public static DependencyProperty SequenceResultProperty = 
            DependencyProperty.Register("SequenceResult", typeof(ISequence),
            typeof(SingleSequenceParserActivity));

        /// <summary>
        /// The first sequence found in the input file.
        /// </summary>
        [OutputParam]
        [Name("Sequence")]
        [Description("The first sequence found in the input file.")]
        public ISequence SequenceResult
        {
            get { return ((ISequence)(base.GetValue(SingleSequenceParserActivity.SequenceResultProperty))); }
            set { base.SetValue(SingleSequenceParserActivity.SequenceResultProperty, value); }
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
            SequenceResult = parser.Parse().FirstOrDefault();
            parser.Close();
            return ActivityExecutionStatus.Closed;
        }
    }
}
