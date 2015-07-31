namespace BiodexExcel.Visualizations.Common
{
    #region -- Using Directive --

    using System.Globalization;
    using System.Windows;

    #endregion -- Using Directive --

    /// <summary>
    /// MaxColumnsDialog lets the user configure the maximum number of columns
    /// he wishes to see in a particular workbook.
    /// </summary>
    public partial class MaxColumnsDialog
    {
        /// <summary>
        /// Indicates whether the dialog was cancelled by the user or not.
        /// </summary>
        private bool submitSelected;

        /// <summary>
        /// Gets the value of maximum column number.
        /// </summary>
        private int maxColumnNumber = 0;

        #region -- Constructor --

        /// <summary>
        /// Initializes a new instance of the MaxColumnsDialog class.
        /// </summary>
        /// <param name="currentMaxColumn">
        /// The current value for maximum number of columns.
        /// </param>  
        /// <param name="alignAllSequenceSheet">
        /// Indicates whether the current sheet has to be aligned 
        /// or the entire set of sequence sheets in the current workbook.
        /// </param>
        public MaxColumnsDialog(int currentMaxColumn, bool alignAllSequenceSheet)
        {
            this.InitializeComponent();
            this.maxColumnNumber = currentMaxColumn;
            this.btnSave.Click += new RoutedEventHandler(this.OnSaveMaxColumns);
            this.btnCancel.Click += new RoutedEventHandler(this.OnCancelMaxColumns);
            this.txtMaxColumns.Text = currentMaxColumn.ToString(CultureInfo.CurrentCulture);
            this.chkAlignAll.IsChecked = alignAllSequenceSheet;

            this.btnSave.Focus();
        }

        #endregion -- Constructor --

        /// <summary>
        /// Gets the maximum number of columns the user wishes to see.
        /// </summary>
        public int MaxNumber
        {
            get
            {
                return this.maxColumnNumber;
            }
        }

        /// <summary>
        /// Gets a value indicating whether all sequence sheet should be aligned or not.
        /// </summary>
        public bool AlignAllSequenceSheet
        {
            get
            {
                return this.chkAlignAll.IsChecked.Value;
            }
        }

        /// <summary>
        /// This method displays the dialog and waits for the user
        /// to enter the max number of columns.
        /// </summary>
        /// <returns>The max number of columns.</returns>
        public new bool Show()
        {
            this.ShowDialog();
            return this.submitSelected;
        }

        #region -- Private Methods --

        /// <summary>
        /// This method is called when the user has chosen the maz.
        /// </summary>
        /// <param name="sender">btnSave instance.</param>
        /// <param name="e">Event data.</param>
        private void OnSaveMaxColumns(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(this.txtMaxColumns.Text, out this.maxColumnNumber) && this.maxColumnNumber > 0)
            {
                this.submitSelected = true;
                this.Close();
            }
            else
            {
                MessageBox.Show(Properties.Resources.MAX_COLUMN_ERROR, Properties.Resources.CAPTION, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// This method is called when the user has cancelled the dialog
        /// and does not wish to save his changes.
        /// </summary>
        /// <param name="sender">btnCancel instance.</param>
        /// <param name="e">Event data.</param>
        private void OnCancelMaxColumns(object sender, RoutedEventArgs e)
        {
            this.submitSelected = false;
            this.Close();
        }

        #endregion -- Private Methods --
    }
}
