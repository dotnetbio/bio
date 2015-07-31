namespace BiodexExcel.Visualizations.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Forms;
    using Bio;

    /// <summary>
    /// Interaction logic for InputSelectionDialog.xaml
    /// </summary>
    public partial class ExportSelectionDialog : Window, ISelectionDialog
    {
        /// <summary>
        /// Resource for SelectInputSequenceRanges
        /// </summary>
        public const string RESOURCESELECTINPUTSEQUENCERANGES = "ExportSelectionDialog_SelectInputSequenceRanges";

        /// <summary>
        /// Resource for SelectInputSequences
        /// </summary>
        public const string RESOURCESELECTINPUTSEQUENCES = "ExportSelectionDialog_SelectInputSequences";

        /// <summary>
        /// Minimum sequence items to be selected
        /// </summary>
        private int maxSequenceItems;

        /// <summary>
        /// Selected InputSequenceItem object
        /// </summary>
        private InputSequenceItem selectedSequenceItem;

        /// <summary>
        /// Callback to show selection helper window.
        /// </summary>
        private SelectionHelperCallback GetSelectionFromSheet;

        /// <summary>
        /// Initializes a new instance of the ExportSelectionDialog class.
        /// Shows the sequence selection dialog
        /// </summary>
        /// <param name="callbackForSelectionHelper">
        /// Callback to method which will show the selection helper and return selection address as string
        /// </param>
        /// <param name="maxSequenceCount">Maximum number of sequences user can select</param>
        public ExportSelectionDialog(
                SelectionHelperCallback callbackForSelectionHelper,
                int maxSequenceCount)
        {
            this.IsSequenceNameVisible = true;
            this.GetSelectionFromSheet = callbackForSelectionHelper;
            this.maxSequenceItems = maxSequenceCount > 1 ? maxSequenceCount : 1;

            InitializeComponent();
        }

        /// <summary>
        /// Event raised when Selection dialog box is submitted.
        /// </summary>
        public event SequenceSelectionDialogSubmit InputSelectionDialogSubmitting;

        /// <summary>
        /// Gets or sets a value indicating whether sequencer range is selected.
        /// </summary>
        public bool IsSequenceRangeSelection { get; set; }

        /// <summary>
        /// Gets or sets currently selected sequence in the list. 
        /// Used by selection helper and other classes to set the sequence address from outside.
        /// </summary>
        public InputSequenceItem SelectedItem
        {
            get { return selectedSequenceItem; }
            set { selectedSequenceItem = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show/hide the box for entering sequence ID/Name
        /// </summary>
        public bool IsSequenceNameVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show/hide metadata selection box.
        /// </summary>
        public bool IsMetadataVisible { get; set; }

        /// <summary>
        /// Gets a value indicating whether to treat blank cells as gaps. Else ignore the blank cells.
        /// </summary>
        public bool TreatBlankCellsAsGaps
        {
            get { return (bool)blankCellsAsGaps.IsChecked; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to change metadata to qualityvalues if its a fastq file export
        /// </summary>
        public bool IsQualityScoresVisible { get; set; }

        /// <summary>
        /// Gets selected molecule type.
        /// [Auto,DNA,RNA,Protien]
        /// </summary>
        public string MoleculeType
        {
            get { return moleculeTypeCombo.Text; }
        }

        /// <summary>
        /// Gets selection model input parameters for cache key
        /// </summary>
        public string InputParamsAsKey
        {
            get { return "_" + MoleculeType + (TreatBlankCellsAsGaps ? "_B" : "_NB"); }
        }

        /// <summary>
        /// Returns the list of sequence items(UserControl) in the dialog
        /// </summary>
        /// <returns>List of InputSequenceItem objects</returns>
        public List<InputSequenceItem> GetSequences()
        {
            List<InputSequenceItem> sequenceList = new List<InputSequenceItem>();
            foreach (UIElement sequenceItem in this.sequenceList.Children)
            {
                sequenceList.Add(sequenceItem as InputSequenceItem);
            }

            return sequenceList;
        }

        /// <summary>
        /// Loads default values and minimum number of sequence items and so on
        /// </summary>
        public void Initialize()
        {
            // Set window title
            if (IsSequenceRangeSelection)
            {
                windowHeader.Text = Resources[RESOURCESELECTINPUTSEQUENCERANGES].ToString();
                sequenceConfigurationControls.Visibility = Visibility.Collapsed;
            }
            else
            {
                windowHeader.Text = Resources[RESOURCESELECTINPUTSEQUENCES].ToString();
                sequenceConfigurationControls.Visibility = Visibility.Visible;
            }

            // populate molecule type combo box
            this.moleculeTypeCombo.Items.Add(Properties.Resources.AutoDetectString);
            IEnumerable<IAlphabet> moleculeTypes = Alphabets.All.OrderBy(alpha => alpha.Name);
            foreach (IAlphabet alpha in moleculeTypes)
            {
                this.moleculeTypeCombo.Items.Add(alpha.Name);
            }

            this.moleculeTypeCombo.SelectedIndex = 0;

            // Populate minimum number of sequence selection controls to the list.
            InputSequenceItem tmpSequenceItem;
            sequenceList.Children.Clear();

            tmpSequenceItem = new InputSequenceItem();
            tmpSequenceItem.IsRemoveItemVisible = false;
            tmpSequenceItem.IsSequenceNameVisible = this.IsSequenceNameVisible;
            tmpSequenceItem.IsMetadataVisible = this.IsMetadataVisible;
            tmpSequenceItem.IsQualityScoresVisible = this.IsQualityScoresVisible;
            tmpSequenceItem.RequestSelectionHelper += new RoutedEventHandler(OnRequestSelectionHelper);
            sequenceList.Children.Add(tmpSequenceItem);

            this.selectedSequenceItem = sequenceList.Children[0] as InputSequenceItem;
            UpdateAddSequenceButton();
        }

        /// <summary>
        /// Method called when user clicks the selection helper button.
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">Event arguments</param>
        private void OnRequestSelectionHelper(object sender, RoutedEventArgs e)
        {
            this.SelectedItem = sender as InputSequenceItem;
            this.Hide();

            // This call will show selection helper window and get the selected address as string
            GetSelectionFromSheet(this);
        }

        /// <summary>
        /// Determines if the add sequence button should be displayed or not depending on the maxNumber possible and current count.
        /// </summary>
        private void UpdateAddSequenceButton()
        {
            if (sequenceList.Children.Count >= this.maxSequenceItems)
            {
                btnAddSequence.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                btnAddSequence.Visibility = System.Windows.Visibility.Visible;
            }
        }

        /// <summary>
        /// Adds a new sequence item to the UI
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">Event arguments</param>
        private void OnAddSequenceClick(object sender, RoutedEventArgs e)
        {
            InputSequenceItem tmpSequenceItem = new InputSequenceItem();
            tmpSequenceItem.IsSequenceNameVisible = this.IsSequenceNameVisible;
            tmpSequenceItem.IsMetadataVisible = this.IsMetadataVisible;
            tmpSequenceItem.IsQualityScoresVisible = this.IsQualityScoresVisible;
            tmpSequenceItem.RemoveItemClick += new RoutedEventHandler(OnRemoveItemClick);
            tmpSequenceItem.RequestSelectionHelper += new RoutedEventHandler(OnRequestSelectionHelper);
            sequenceList.Children.Add(tmpSequenceItem);
            UpdateAddSequenceButton();
        }

        /// <summary>
        /// Removes the selected sequence item
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">Event arguments</param>
        private void OnRemoveItemClick(object sender, RoutedEventArgs e)
        {
            sequenceList.Children.Remove(sender as UIElement);
            UpdateAddSequenceButton();
        }

        /// <summary>
        /// Calls the callback method provided when showing this dialog, in case of error this dialog will be shown again
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">Event arguments</param>
        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            this.Hide();
            if (InputSelectionDialogSubmitting != null)
            {
                InputSelectionDialogSubmitting(this);
            }
        }

        /// <summary>
        /// Closes this dialog without invoking the callback delegate.
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">Event arguments</param>
        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
