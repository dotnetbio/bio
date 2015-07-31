namespace BiodexExcel.Visualizations.Common
{
    #region --Using Directive --

    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Documents;

    #endregion --Using Directive --

    /// <summary>
    /// BedWizardDialog helps in auto-parsing the sheets which have query region data.
    /// The bed wizard is prompted when the header cannot be properly identified.
    /// It presents the user a dialog where he can map the header in the sheet to actual
    /// Query region headers.
    /// </summary>
    public partial class BedWizardDialog : Window
    {
        #region -- Private Members --
        /// <summary>
        /// List of all BED headers.
        /// </summary>
        private List<string> bedHeader;

        /// <summary>
        /// List of mandatory headers.
        /// </summary>
        private List<string> requiredHeaders;

        /// <summary>
        /// Mapping of header v\s column.
        /// </summary>
        private Dictionary<string, int> mapping = new Dictionary<string, int>();

        /// <summary>
        /// Indicates whether the dialog was cancelled by the user or not.
        /// </summary>
        private bool submitSelected;

        #endregion -- Private Members --     

        #region -- Constructor --

        /// <summary>
        /// Initializes a new instance of the BedWizardDialog class.
        /// </summary>
        /// <param name="sheetName">Name of the sheet which has to be parsed.</param>
        /// <param name="columnNumbers">List of column numbers which have to be mapped to headers.</param>
        /// <param name="bedHeader">List of BED headers.</param>
        /// <param name="requiredHeaders">List of mandatory headers.</param>
        public BedWizardDialog(string sheetName, List<int> columnNumbers, List<string> bedHeader, List<string> requiredHeaders)
        {
            this.InitializeComponent();
            this.btnOK.Click += this.OnBedWizardSave;
            this.btnCancel.Click += this.OnCancelBedWizard;
            this.PreviewKeyUp += this.OnBedWizardDialogKeyUp;
            this.bedHeader = bedHeader;
            this.requiredHeaders = requiredHeaders;    
            
            this.txtSubText.Text = string.Format(Properties.Resources.HEADER_SUBTEXT, sheetName);
            
            foreach (string s in requiredHeaders)
            {
                ColumnMap map = new ColumnMap(columnNumbers, s);
                this.stkfirstPanel.Children.Add(map);
            }

            IEnumerable<string> optionalHeaders = bedHeader.Except(requiredHeaders);

            foreach (string s in optionalHeaders)
            {
                ColumnMap map = new ColumnMap(columnNumbers, s);
                this.stkSecondPanel.Children.Add(map);
            }
        }

        #endregion -- Constructor --

        #region -- Public Properties --

        /// <summary>
        /// Gets a list of header v\s column mapping.
        /// </summary>
        public Dictionary<string, int> Mapping
        {
            get
            {
                return this.mapping;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the first line in the excel should be treated as a header or not.
        /// </summary>
        public bool IsHeaderPresent
        {
            get
            {
                return this.chkFirstLine.IsChecked.HasValue ? this.chkFirstLine.IsChecked.Value : false;
            }
        }

        #endregion -- Public Properties --

        #region -- Public Methods --

        /// <summary>
        /// This method displays the BED wizard dialog and waits for the user to choose headers.
        /// </summary>
        /// <returns>true: if the user selected "OK", false otherwise.</returns>
        public new bool Show()
        {
            this.ShowDialog();
            return this.submitSelected;
        }

        #endregion -- Public Methods --

        #region -- Private Methods --

        /// <summary>
        /// This method is called when the user has cancelled the BED wizard dialog
        /// and does not wish to save his changes.
        /// </summary>
        /// <param name="sender">btnCancel instance.</param>
        /// <param name="e">Event data.</param>
        private void OnCancelBedWizard(object sender, RoutedEventArgs e)
        {
            this.submitSelected = false;
            this.Close();
        }

        /// <summary>
        /// This method is called when the user has chosen to save 
        /// the BED wizard dialog contents. This method also 
        /// validates to see if mandatory headers are selected
        /// and no column is selected more than once.
        /// </summary>
        /// <param name="sender">btnOk instance.</param>
        /// <param name="e">Event data.</param>
        private void OnBedWizardSave(object sender, RoutedEventArgs e)
        {
            this.mapping.Clear();
            List<UIElement> elements = new List<UIElement>();

            foreach (UIElement element in this.stkfirstPanel.Children)
            {
                elements.Add(element);
            }

            foreach (UIElement element in this.stkSecondPanel.Children)
            {
                elements.Add(element);
            }

            foreach (UIElement element in elements)
            {
                ColumnMap map = element as ColumnMap;
                if (map != null && map.ColumnNumber.HasValue)
                {
                    if (this.mapping.ContainsValue(map.ColumnNumber.Value))
                    {
                        MessageBox.Show(Properties.Resources.ONE_COLUMN, Properties.Resources.CAPTION, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else
                    {
                        this.mapping.Add(map.ColumnHeader, map.ColumnNumber.Value);
                    }
                }
            }

            foreach (string s in this.requiredHeaders)
            {
                if (!this.mapping.ContainsKey(s))
                {
                    MessageBox.Show(Properties.Resources.MANDATORY_COLUMNS, Properties.Resources.CAPTION, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            this.submitSelected = true;
            this.Close();
        }

        /// <summary>
        /// This event close the dialog on escape button pressed, 
        /// it would be a cancel action.
        /// </summary>
        /// <param name="sender">BedWizardDialog Instance</param>
        /// <param name="e">Event data</param>
        private void OnBedWizardDialogKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                //// Close the pop up.
                this.Close();
            }
        }

        #endregion -- Private Methods --
    }
}
