using System;
using System.ComponentModel;
using System.Workflow.ComponentModel;
using Bio.SimilarityMatrices;
using Microsoft.Research.ScientificWorkflow;

namespace Bio.Workflow
{
    /// <summary>
    /// This activity looks for a known similarity matrix.
    /// </summary>
    [Name("Lookup Similarity Matrix")]
    [Description("Looks for a known similarity matrix.")]
    [WorkflowCategory("Bioinformatics")]
    public class LookupSimilarityMatrixActivity : Activity
    {
        #region Dependency Properties
        /// <summary>
        /// The name of a known similarity matrix (e.g. 'Blosum50').
        /// </summary>
        public static DependencyProperty MatrixNameProperty = 
            DependencyProperty.Register("MatrixName", typeof(string),
            typeof(LookupSimilarityMatrixActivity),
            new PropertyMetadata("Blosum50"));

        /// <summary>
        /// The name of a known similarity matrix (e.g. 'Blosum50').
        /// </summary>
        [RequiredInputParam]
        [Name("Matrix Name")]
        [Description(@"The name of a known similarity matrix (e.g. 'Blosum50').")]
        public string MatrixName
        {
            get { return ((string)(base.GetValue(LookupSimilarityMatrixActivity.MatrixNameProperty))); }
            set { base.SetValue(LookupSimilarityMatrixActivity.MatrixNameProperty, value); }
        }

        /// <summary>
        /// The similarity matrix result.
        /// </summary>
        public static DependencyProperty MatrixProperty = 
            DependencyProperty.Register("Matrix", typeof(SimilarityMatrix),
            typeof(LookupSimilarityMatrixActivity));

        /// <summary>
        /// The similarity matrix result.
        /// </summary>
        [OutputParam]
        [Name("Matrix")]
        [Description("The similarity matrix result.")]
        public SimilarityMatrix Matrix
        {
            get { return ((SimilarityMatrix)(base.GetValue(LookupSimilarityMatrixActivity.MatrixProperty))); }
            set { base.SetValue(LookupSimilarityMatrixActivity.MatrixProperty, value); }
        }

        #endregion

        /// <summary>
        /// The execution method for the activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <returns>The execution status.</returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            if (MatrixName.Equals("Blosum45", StringComparison.InvariantCultureIgnoreCase))
                Matrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.Blosum45);
            else if (MatrixName.Equals("Blosum50", StringComparison.InvariantCultureIgnoreCase))
                Matrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.Blosum50);
            else if (MatrixName.Equals("Blosum62", StringComparison.InvariantCultureIgnoreCase))
                Matrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.Blosum62);
            else if (MatrixName.Equals("Blosum80", StringComparison.InvariantCultureIgnoreCase))
                Matrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.Blosum80);
            else if (MatrixName.Equals("Blosum90", StringComparison.InvariantCultureIgnoreCase))
                Matrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.Blosum90);
            else if (MatrixName.Equals("Pam250", StringComparison.InvariantCultureIgnoreCase))
                Matrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.Pam250);
            else if (MatrixName.Equals("Pam30", StringComparison.InvariantCultureIgnoreCase))
                Matrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.Pam30);
            else if (MatrixName.Equals("Pam70", StringComparison.InvariantCultureIgnoreCase))
                Matrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.Pam70);
            else if (MatrixName.Equals("AmbiguousDna", StringComparison.InvariantCultureIgnoreCase))
                Matrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.AmbiguousDna);
            else if (MatrixName.Equals("AmbiguousRna", StringComparison.InvariantCultureIgnoreCase))
                Matrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.AmbiguousRna);
            
            return ActivityExecutionStatus.Closed;
        }
    }
}
