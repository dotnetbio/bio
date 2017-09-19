using System;
using System.Windows;

namespace BiodexExcel.Visualizations.Common.DialogBox
{
    /// <summary>
    /// Interaction logic for ImportConfiguration.xaml
    /// </summary>
    public partial class ImportConfiguration
    {
        /// <summary>
        /// The number of sequences to place onto each worksheet.
        /// </summary>
        public int SequencesPerWorksheet { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ImportConfiguration()
        {
            Loaded += OnLoaded;
            InitializeComponent();
        }

        /// <summary>
        /// Visual tree has been loaded - initialize our controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            tbSequenceCount.Text = "100";
            if (SequencesPerWorksheet == 1)
            {
                rbOnePerSheet.IsChecked = true;
            }
            else if (SequencesPerWorksheet > 1)
            {
                rbMultiPerSheet.IsChecked = true;
                tbSequenceCount.Text = SequencesPerWorksheet.ToString();
            }
            else
            {
                rbAllOnOne.IsChecked = true;
            }
        }

        /// <summary>
        /// OK button clicked.  Get new value and dismiss dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            if (rbMultiPerSheet.IsChecked == true)
            {
                int value;
                if (Int32.TryParse(tbSequenceCount.Text, out value) == false)
                {
                    MessageBox.Show(Properties.Resources.INVALID_SEQ_PER_SHEET, Properties.Resources.CAPTION,
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                    tbSequenceCount.Focus();
                    return;
                }
                SequencesPerWorksheet = value;
            }
            else if (rbOnePerSheet.IsChecked == true)
            {
                SequencesPerWorksheet = 1;
            }
            else if (rbAllOnOne.IsChecked == true)
            {
                SequencesPerWorksheet = -1;
            }

            this.DialogResult = true;
        }

        /// <summary>
        /// The multiple sequences per worksheet radio button was selected, 
        /// shift focus to our text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMultipleSequencesChecked(object sender, RoutedEventArgs e)
        {
            if (rbMultiPerSheet.IsChecked == true)
            {
                tbSequenceCount.Focus();
            }
        }
    }
}
