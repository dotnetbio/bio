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
    public partial class InputSelectionDialog : Window, ISelectionDialog
    {
        #region -- Private Members --

        /// <summary>
        /// Resource for SelectInputSequenceRanges
        /// </summary>
        public const string RESOURCESELECTINPUTSEQUENCERANGES = "InputSelectionDialog_SelectInputSequenceRanges";

        /// <summary>
        /// Resource for SelectInputSequences
        /// </summary>
        public const string RESOURCESELECTINPUTSEQUENCES = "InputSelectionDialog_SelectInputSequences";

        /// <summary>
        /// Stores the minimum overlap.
        /// </summary>
        private long minOverlap;

        /// <summary>
        /// Indicates whether the base pairs overlapping should be intersected or not.
        /// </summary>
        private bool overlappingBasePairs;

        /// <summary>
        /// Minimum and maximum sequence items to be selected
        /// </summary>
        private int minSequenceItems, maxSequenceItems;

        /// <summary>
        /// Labels to be displayed for each sequence textbox
        /// </summary>
        private string[] sequenceLabels;

        /// <summary>
        /// Selected InputSequenceItem object
        /// </summary>
        private InputSequenceItem selectedSequenceItem;

        /// <summary>
        /// Callback to show selection helper window.
        /// </summary>
        private SelectionHelperCallback GetSelectionFromSheet;

        /// <summary>
        /// flag to indicate whether to display Use metadata check box or not.
        /// </summary>
        private bool isUseMetadataCheckBoxVisible;
        #endregion -- Private Members --

        /// <summary>
        /// Initializes a new instance of the InputSelectionDialog class.
        /// Shows the sequence selection dialog
        /// </summary>
        /// <param name="callbackForSelectionHelper">
        /// Callback to method which will show the selection helper and return selection address as string
        /// </param>
        /// <param name="minSequenceCount">Minimum number of sequences to be selected.</param>
        /// <param name="maxSequenceCount">Maximum number of sequences user can select.</param>
        /// <param name="intersectOperation">Is this intersect operation</param>
        /// <param name="isOverlappingIntervalVisible">Is overlapping interval visible</param>
        /// <param name="isMinimumOverlapVisible">Is minimum overlap visible</param>
        /// <param name="sequenceLabels">
        /// Labels to be displayed for each sequence textbox. ex : "Reference Sequence".
        /// Labels will be applied from top to bottom in the order of labels given to a 
        /// maximum number of 'minSequenceCount'
        /// </param>
        public InputSelectionDialog(
                SelectionHelperCallback callbackForSelectionHelper,
                int minSequenceCount,
                int maxSequenceCount,
                bool intersectOperation,
                bool isOverlappingIntervalVisible,
                bool isMinimumOverlapVisible,
                params string[] sequenceLabels)
        {
            this.IsSequenceNameVisible = true;
            this.GetSelectionFromSheet = callbackForSelectionHelper;
            this.sequenceLabels = sequenceLabels;
            this.minSequenceItems = minSequenceCount > 1 ? minSequenceCount : 1;
            this.maxSequenceItems = maxSequenceCount > minSequenceCount ? maxSequenceCount : this.minSequenceItems;

            InitializeComponent();
            if (!isOverlappingIntervalVisible)
            {
                this.stkOverlapping.Visibility = Visibility.Collapsed;

                // This is merge operation, so set the default to "0"
                this.txtOverlap.Text = "0";
                this.isUseMetadataCheckBoxVisible = false;
            }
            else
            {
                this.isUseMetadataCheckBoxVisible = true;
                bool isFirst = true;
                foreach (InputSequenceItem item in this.GetSequences())
                {
                    item.IsUseMetadataCheckBoxVisible = this.isUseMetadataCheckBoxVisible;
                    if (isFirst && this.isUseMetadataCheckBoxVisible)
                    {
                        item.IsUseMetadataSelected = true;
                        isFirst = false;
                    }

                    item.UseMetadataSelected += new RoutedEventHandler(OnUseMetadataSelected);
                }

                this.Initialize(intersectOperation);
            }

            if (!isMinimumOverlapVisible)
            {
                this.stkMinimumOverlap.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Event raised when Selection dialog box is submitted.
        /// </summary>
        public event SequenceSelectionDialogSubmit InputSelectionDialogSubmitting;

        #region -- Properties --

        /// <summary>
        /// Gets the value for minimum overlap.
        /// </summary>
        public long MinOverLap
        {
            get
            {
                return this.minOverlap;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the base pairs which are overlapping should be intersected or not.
        /// </summary>
        public bool OverlappingBasePairs
        {
            get
            {
                return this.overlappingBasePairs;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether query region is selected.
        /// </summary>
        public bool IsSequenceRangeSelection { get; set; }

        /// <summary>
        /// Gets or sets Currently selected sequence in the list. 
        /// Used by selection helper and other classes to set the sequence address from outside.
        /// </summary>
        public InputSequenceItem SelectedItem
        {
            get { return selectedSequenceItem; }
            set { selectedSequenceItem = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the box for entering sequence ID/Name
        /// </summary>
        public bool IsSequenceNameVisible { get; set; }

        /// <summary>
        /// Gets a value indicating whether to treat blank cells as gaps. Else ignore the blank cells.
        /// </summary>
        public bool TreatBlankCellsAsGaps
        {
            get { return (bool)blankCellsAsGaps.IsChecked; }
        }

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

        #endregion -- Properties --

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
            for (int i = 1; i <= minSequenceItems; i++)
            {
                tmpSequenceItem = new InputSequenceItem();
                tmpSequenceItem.IsUseMetadataCheckBoxVisible = this.isUseMetadataCheckBoxVisible;
                if (i == 1 && this.isUseMetadataCheckBoxVisible)
                {
                    tmpSequenceItem.IsUseMetadataSelected = true;
                }

                tmpSequenceItem.UseMetadataSelected += new RoutedEventHandler(OnUseMetadataSelected);
                if (sequenceLabels != null)
                {
                    if (sequenceLabels.Length >= i)
                    {
                        tmpSequenceItem.SequenceLabel = sequenceLabels[i - 1];
                    }
                }

                tmpSequenceItem.IsRemoveItemVisible = false;
                tmpSequenceItem.IsSequenceNameVisible = this.IsSequenceNameVisible;
                tmpSequenceItem.sequenceAddressBox.TextChanged += new TextChangedEventHandler(OnSequenceAddressChanged);
                tmpSequenceItem.RequestSelectionHelper += new RoutedEventHandler(OnRequestSelectionHelper);
                sequenceList.Children.Add(tmpSequenceItem);
            }

            this.selectedSequenceItem = sequenceList.Children[0] as InputSequenceItem;
            UpdateAddSequenceButton();
            ValidateForBlankAddress();
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
        /// This method initializes the UI depending
        /// on whether the UI is for Intersect operation or Subtract operation.
        /// </summary>
        /// <param name="intersectOperation">Indicates whether the operation is Intersect or Subtract.</param>
        private void Initialize(bool intersectOperation)
        {
            if (intersectOperation)
            {
                InitializeIntersectMode();
            }
            else
            {
                InitializeSubtractMode();
            }
        }

        /// <summary>
        /// This method initializes the UI for Subtract operation.
        /// </summary>
        private void InitializeSubtractMode()
        {
            string intervalNoOverlap = Properties.Resources.INTERVAL_NO_OVERLAP;
            string intervalOverlap = Properties.Resources.INTERVAL_OVERLAP;

            this.overlappingOpt2.Content = intervalOverlap;
            overlappingOpt2.ToolTip = Properties.Resources.Subtract_WithNonOverlappingPiecesofInterval;
            this.overlappingOpt2.IsChecked = false;
            this.overlappingOpt1.Content = intervalNoOverlap;
            overlappingOpt1.ToolTip = Properties.Resources.Subtract_WithNoOverlap;
            this.overlappingOpt1.IsChecked = true;

            imgopt3.Visibility = System.Windows.Visibility.Visible;
            imgopt4.Visibility = System.Windows.Visibility.Visible;
            imgopt1.Visibility = System.Windows.Visibility.Collapsed;
            imgopt2.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// This method initializes the UI for Intersect operation.
        /// </summary>
        private void InitializeIntersectMode()
        {
            string overlappingInterval = Properties.Resources.OVERLAPPING_INTERVALS;
            string overlappingPiecesInterval = Properties.Resources.OVERLAPPING_PIECES_OF_INTERVALS;

            this.overlappingOpt2.Content = overlappingInterval;
            overlappingOpt2.ToolTip = Properties.Resources.Intersect_OverlappingIntervals;
            imgopt1.Visibility = System.Windows.Visibility.Visible;
            imgopt2.Visibility = System.Windows.Visibility.Visible;
            imgopt3.Visibility = System.Windows.Visibility.Collapsed;
            imgopt4.Visibility = System.Windows.Visibility.Collapsed;
            this.overlappingOpt2.IsChecked = false;

            this.overlappingOpt1.Content = overlappingPiecesInterval;
            overlappingOpt1.ToolTip = Properties.Resources.Intersect_OverlappingPiecesofIntervals;
            this.overlappingOpt1.IsChecked = true;
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

            // Load sequence label if one exists for current index
            if (null != sequenceLabels)
            {
                if (sequenceLabels.Length > sequenceList.Children.Count)
                {
                    tmpSequenceItem.SequenceLabel = sequenceLabels[sequenceList.Children.Count];
                }
                else
                {
                    tmpSequenceItem.SequenceLabel = sequenceLabels[sequenceLabels.Length - 1];
                }
            }

            tmpSequenceItem.sequenceAddressBox.TextChanged += new TextChangedEventHandler(OnSequenceAddressChanged);
            tmpSequenceItem.RemoveItemClick += new RoutedEventHandler(OnRemoveItemClick);
            tmpSequenceItem.RequestSelectionHelper += new RoutedEventHandler(OnRequestSelectionHelper);
            tmpSequenceItem.IsUseMetadataCheckBoxVisible = this.isUseMetadataCheckBoxVisible;
            tmpSequenceItem.UseMetadataSelected += new RoutedEventHandler(OnUseMetadataSelected);
            sequenceList.Children.Add(tmpSequenceItem);
            UpdateAddSequenceButton();
            ValidateForBlankAddress();
        }

        /// <summary>
        /// Checks if all sequence address boxes has some value or not
        /// Enables 'Ok' button if all sequence address'es has some value irrespective of if the range is proper or not.
        /// </summary>
        private void ValidateForBlankAddress()
        {
            bool valid = true;

            foreach (InputSequenceItem item in sequenceList.Children)
            {
                if (string.IsNullOrWhiteSpace(item.SequenceAddress))
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
            {
                btnOk.IsEnabled = true;
            }
            else
            {
                btnOk.IsEnabled = false;
            }
        }

        /// <summary>
        /// Raised when any sequence address is changed
        /// </summary>
        /// <param name="sender">sequence address textbox</param>
        /// <param name="e">text changed event args</param>
        private void OnSequenceAddressChanged(object sender, TextChangedEventArgs e)
        {
            ValidateForBlankAddress();
        }

        /// <summary>
        /// Removes the selected sequence item
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">Event arguments</param>
        private void OnRemoveItemClick(object sender, RoutedEventArgs e)
        {
            InputSequenceItem tmpSequenceItem = sender as InputSequenceItem;

            if (tmpSequenceItem != null)
            {
                tmpSequenceItem.UseMetadataSelected -= new RoutedEventHandler(OnUseMetadataSelected);
            }

            sequenceList.Children.Remove(sender as UIElement);
            UpdateAddSequenceButton();
            ValidateForBlankAddress();
        }

        /// <summary>
        /// Calls the callback method provided when showing this dialog, in case of error this dialog will be shown again
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">Event arguments</param>
        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            if (long.TryParse(this.txtOverlap.Text, out this.minOverlap))
            {
                this.overlappingBasePairs = this.overlappingOpt1.IsChecked.Value;

                this.Hide();
                if (InputSelectionDialogSubmitting != null)
                {
                    InputSelectionDialogSubmitting(this);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(
                        Properties.Resources.MIN_OVERLAP_ERROR,
                        Properties.Resources.CAPTION,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
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

        /// <summary>
        /// Updates the InputSequenceItems so that at any given time 0 or 1 checkbox should be selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUseMetadataSelected(object sender, RoutedEventArgs e)
        {
            InputSequenceItem tmpSequenceItem = sender as InputSequenceItem;

            if (tmpSequenceItem != null)
            {
                foreach (InputSequenceItem item in GetSequences())
                {
                    if (item != tmpSequenceItem && item.IsUseMetadataSelected)
                    {
                        item.IsUseMetadataSelected = false;
                    }
                }
            }
        }
    }
}
