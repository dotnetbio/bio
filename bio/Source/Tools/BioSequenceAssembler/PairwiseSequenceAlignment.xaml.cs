namespace SequenceAssembler
{
    #region -- Using Directives --

    using System.Globalization;
    using System.Windows;

    #endregion -- Using Directives --

    /// <summary>
    /// Interaction logic for BlastItemDetails.xaml
    /// </summary>
    public partial class PairwiseSequenceAlignment : Window
    {
        #region -- Private Members --

        /// <summary>
        /// Indicates the number of alphabets to be displayed per line.
        /// </summary>
        private const int NumberOfAlphabets = 60;

        /// <summary>
        /// Holds reference to the Blast result for which we are
        /// displaying Pairwise sequence alignment report.
        /// </summary>
        private BlastResultCollator blastResult;

        #endregion -- Private Members --

        #region -- Constructor --

        /// <summary>
        /// Initializes a new instance of the PairwiseSequenceAlignment class.
        /// </summary>
        /// <param name="blastResult"> 
        /// Blast result for which we are displaying Pairwise sequence alignment report.
        /// </param>
        public PairwiseSequenceAlignment(BlastResultCollator blastResult)
        {
            this.InitializeComponent();

            // Hnadkes the "Esc" key event and closes the dialog.
            this.PreviewKeyUp += new System.Windows.Input.KeyEventHandler(this.OnDialogKeyUp);
            this.Owner = Application.Current.MainWindow;

            if (blastResult != null)
            {
                this.blastResult = blastResult;
                this.Display();
            }
        }

        #endregion -- Constructor --

        #region -- Private Methods --

        /// <summary>
        /// This method displays header and alignment information.
        /// </summary>
        private void Display()
        {
            this.DisplayHeader();
            this.DisplayAlignment();
        }

        /// <summary>
        /// DisplayAlignment splits Query string and Subject string into
        /// 60 alphabet per line and displays it one below the other for 
        /// better readability.
        /// </summary>
        private void DisplayAlignment()
        {
            string queryString = this.blastResult.QueryString;
            string subjectString = this.blastResult.SubjectString;

            int queryStringLength = this.blastResult.QueryString.Length;
            int subjectStringLength = this.blastResult.SubjectString.Length;

            int currentQueryStartIndex = 0;
            int currentSubjectStartIndex = 0;

            while (currentQueryStartIndex < queryStringLength && currentSubjectStartIndex < subjectStringLength)
            {
                int length = 0;
                if (currentQueryStartIndex + NumberOfAlphabets > queryStringLength)
                {
                    length = queryStringLength - currentQueryStartIndex;
                }
                else
                {
                    length = NumberOfAlphabets;
                }

                string querySubString = queryString.Substring(currentQueryStartIndex, length);
                currentQueryStartIndex += length;

                length = 0;
                if (currentSubjectStartIndex + NumberOfAlphabets > subjectStringLength)
                {
                    length = subjectStringLength - currentSubjectStartIndex;
                }
                else
                {
                    length = NumberOfAlphabets;
                }

                string subjectSubString = subjectString.Substring(currentSubjectStartIndex, length);
                currentSubjectStartIndex += length;

                AlignmentReport report = new AlignmentReport(querySubString, subjectSubString, currentQueryStartIndex - length + 1);
                this.stkAlignment.Children.Add(report);
            }
        }

        /// <summary>
        /// This method displays header information for Pairwise sequence alignment report.
        /// </summary>
        private void DisplayHeader()
        {
            this.txtHeader.Text = this.blastResult.SubjectId;
            this.txtLength.Text = this.blastResult.Length.ToString(CultureInfo.CurrentCulture); 

            this.txtScore.Text = string.Format(CultureInfo.CurrentCulture, Properties.Resource.PAIRWISE_BITS, this.blastResult.Bit, Properties.Resource.BITS_TEXT);

            this.txtIdentities.Text = this.blastResult.Identity.ToString(CultureInfo.CurrentCulture);
            this.txtPositives.Text = this.blastResult.Positives.ToString(CultureInfo.CurrentCulture);
            this.txtGap.Text = this.blastResult.GapOpenings;           
        }

        /// <summary>
        /// This event close the dialog on escape button pressed, 
        /// it would be a cancel action.
        /// </summary>
        /// <param name="sender">OpenFileDialog Instance</param>
        /// <param name="e">Event data</param>
        private void OnDialogKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                this.Close();
            }
        }

        #endregion -- Private Methods --
    }
}
