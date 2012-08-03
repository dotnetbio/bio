namespace SequenceAssembler
{
    #region -- Using Directive --

    using System.Globalization;
    using System.Windows.Controls;

    #endregion -- Using Directive

    /// <summary>
    /// AlignmentReport displays the query sequence and the subject sequence in a view which
    /// is easy to compare these two sequences.
    /// </summary>
    public partial class AlignmentReport : UserControl
    {
        #region -- Constructor --

        /// <summary>
        /// Initializes a new instance of the AlignmentReport class.
        /// </summary>
        /// <param name="queryString">Query string.</param>
        /// <param name="subjectString">Subject string.</param>
        /// <param name="startIndex">Index from which the sequence had to be displaying.</param>
        public AlignmentReport(string queryValue, string subjectValue, int startIndex)
        {
            InitializeComponent();
            this.DisplayAlignmentReport(queryValue, subjectValue, startIndex);
        }

        #endregion -- Constructor --

        /// <summary>
        /// Displays the the query sequence and the subject sequence in a view which
        /// is easy to compare these two sequences.
        /// </summary>
        /// <param name="queryString">Query string.</param>
        /// <param name="subjectString">Subject string.</param>
        /// <param name="startIndex">Index from which the sequence had to be displaying.</param>
        private void DisplayAlignmentReport(string queryString, string subjectString, int startIndex)
        {
            this.txtQueryStartIndex.Text = startIndex.ToString(CultureInfo.CurrentCulture);
            this.txtQueryString.Text = queryString;
            this.txttQueryEndIndex.Text = (queryString.Length + startIndex - 1).ToString(CultureInfo.CurrentCulture);

            this.txtSubjectStartIndex.Text = startIndex.ToString(CultureInfo.CurrentCulture); 
            this.txtSubjectString.Text = subjectString;
            this.txttSubjectEndIndex.Text = (subjectString.Length + startIndex - 1).ToString(CultureInfo.CurrentCulture); 
        }
    }
}
