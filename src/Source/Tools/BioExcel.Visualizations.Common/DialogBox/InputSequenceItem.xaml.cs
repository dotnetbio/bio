namespace BiodexExcel.Visualizations.Common
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for InputSequenceItem.xaml
    /// </summary>
    public partial class InputSequenceItem : UserControl
    {
        /// <summary>
        /// Resource for QualityValues
        /// </summary>
        public const string RESOURCEQUALITYVALUES = "InputSequenceItem_QualityValues";

        /// <summary>
        /// Resource for ErrorItemBackground
        /// </summary>
        public const string RESOURCEERRORITEMBACKGROUND = "ErrorItemBackground";

        /// <summary>
        /// Resource for NormalItemBackground
        /// </summary>
        public const string RESOURCENORMALITEMBACKGROUND = "NormalItemBackground";

        /// <summary>
        /// Initializes a new instance of the InputSequenceItem class.
        /// </summary>
        public InputSequenceItem()
        {
            InitializeComponent();
            this.chkUseMetadata.Checked += new RoutedEventHandler(OnchkUseMetadataChecked);
        }

        /// <summary>
        /// Event for publishing remove item request to container form.
        /// </summary>
        public event RoutedEventHandler RemoveItemClick;

        /// <summary>
        /// Event for publishing request to show the selection helper
        /// </summary>
        public event RoutedEventHandler RequestSelectionHelper;

        public event RoutedEventHandler UseMetadataSelected;

        /// <summary>
        /// Gets or sets label given for the sequence textbox.
        /// </summary>
        public string SequenceLabel
        {
            get { return sequenceBoxLabel.Text; }
            set { sequenceBoxLabel.Text = value; }
        }

        /// <summary>
        /// Gets or sets text in sequence address box
        /// </summary>
        public string SequenceAddress
        {
            get { return sequenceAddressBox.Text; }
            set { sequenceAddressBox.Text = value; }
        }

        /// <summary>
        /// Gets or sets range address in the metadata textbox
        /// </summary>
        public string MetadataAddress
        {
            get { return sequenceMetadata.Text; }
            set { sequenceMetadata.Text = value; }
        }

        /// <summary>
        /// Gets textbox in focus, Text value of this textbox will be set by selection helper.
        /// </summary>
        public TextBox FocusedTextBox
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Gets or sets Name/ID of the sequence
        /// </summary>
        public string SequenceName
        {
            get { return sequenceID.Text; }
            set { sequenceID.Text = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Name/ID option should be visible or not.
        /// </summary>
        public bool IsSequenceNameVisible 
        {
            get { return sequenceNameGrid.Visibility == System.Windows.Visibility.Visible; }
            set { sequenceNameGrid.Visibility = value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Metadata box is visible or not.
        /// Set as visible for making selections for exporting to file.
        /// </summary>
        public bool IsMetadataVisible
        {
            get { return sequenceMetadataGrid.Visibility == System.Windows.Visibility.Visible; }
            set { sequenceMetadataGrid.Visibility = value || IsQualityScoresVisible ? Visibility.Visible : Visibility.Collapsed; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Quality Scores is visible
        /// </summary>
        public bool IsQualityScoresVisible
        {
            get 
            { 
                return sequenceMetadataGrid.Visibility == System.Windows.Visibility.Visible; 
            }

            set 
            {
                if (value)
                {
                    sequenceMetadataGrid.Visibility = Visibility.Visible;
                    metadataBoxLabel.Text = Resources[RESOURCEQUALITYVALUES].ToString();
                }
                else
                {
                    if (!IsMetadataVisible)
                    {
                        sequenceMetadataGrid.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether remove item button (close button on each item) is visible or not
        /// </summary>
        public bool IsRemoveItemVisible
        {
            get
            {
                return deleteItemButton.Visibility == System.Windows.Visibility.Visible;
            }

            set
            {
                if (value)
                {
                    deleteItemButton.Visibility = Visibility.Visible;
                }
                else
                {
                    deleteItemButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Use as Metadata checkbox is visible or not.
        /// </summary>
        public bool IsUseMetadataCheckBoxVisible
        {
            get
            {
                return chkUseMetadata.Visibility == Visibility.Visible;
            }
            set
            {
                if (value)
                {
                    chkUseMetadata.Visibility = Visibility.Visible;
                }
                else
                {
                    chkUseMetadata.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Use as Metadata checkbox is selected or not.
        /// </summary>
        public bool IsUseMetadataSelected
        {
            get
            {
                return this.chkUseMetadata.IsChecked.Value;
            }
            set
            {
                this.chkUseMetadata.IsChecked = value;
            }
        }

        /// <summary>
        /// Sets a particular item to be highlighted.
        /// Will be called if there were any parsing issues which this item.
        /// </summary>
        /// <param name="isError">If true, highlight the given item, else remove the highlight.</param>
        public void SetErrorStatus(bool isError)
        {
            if (isError)
            {
                backgroundBorder.Background = Resources[RESOURCEERRORITEMBACKGROUND] as System.Windows.Media.LinearGradientBrush;
            }
            else
            {
                backgroundBorder.Background = Resources[RESOURCENORMALITEMBACKGROUND] as System.Windows.Media.LinearGradientBrush;
            }
        }

        /// <summary>
        /// Event raised when user clicks the remove item / close button.
        /// The event is published back to the container window after changing the sender as this object.
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">Event arguments</param>
        private void OnRemoveItemClick(object sender, RoutedEventArgs e)
        {
            RemoveItemClick(this, e);
        }

        /// <summary>
        /// Event raised when user clicks selection helper button next to the sequence address textbox.
        /// The event is published back to the container window after changing the sender as this object.
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">Event arguments</param>
        private void OnSelectFromSheetClick(object sender, RoutedEventArgs e)
        {
            if (object.ReferenceEquals(sender, sequenceSelectionHelper))
            {
                FocusedTextBox = sequenceAddressBox;
            }
            else if (object.ReferenceEquals(sender, metadataSelectionHelper))
            {
                FocusedTextBox = sequenceMetadata;
            }
            else
            {
                return;
            }

            RequestSelectionHelper(this, e);
        }

        /// <summary>
        /// This method will be called when UseMetadata check box is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnchkUseMetadataChecked(object sender, RoutedEventArgs e)
        {
            if (UseMetadataSelected != null)
            {
                UseMetadataSelected.Invoke(this, e);
            }
        }
    }
}
