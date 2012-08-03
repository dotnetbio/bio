namespace SequenceAssembler
{
    #region -- Using Directives --

    using System;
    using System.Windows;
    using System.Windows.Input;
    using Bio;

    #endregion -- Using Directives --

    /// <summary>
    /// Interaction logic for EditSequenceDialog.xaml, which will let the 
    /// user to edit the selected sequence. 
    /// </summary>
    public partial class EditSequenceDialog : Window
    {
        #region -- Private Members --

        /// <summary>
        /// Describes the sequence to be edited.
        /// </summary>
        private string sequence;

        #endregion

        #region -- Constructor --

        /// <summary>
        /// Initializes a new instance of the EditSequenceDialog class.
        /// </summary>
        /// <param name="seq">Associated sequence</param>
        public EditSequenceDialog(ISequence seq)
        {
            InitializeComponent();
            this.Sequence = seq;
            this.btnSave.Click += new RoutedEventHandler(this.OnBtnSaveClick);
            this.btnCancel.Click += new RoutedEventHandler(this.OnBtnCancelClick);
            this.txtSequence.KeyUp += new KeyEventHandler(this.OnTxtSequenceKeyUp);
            this.KeyUp += new KeyEventHandler(this.OnEditSequenceDialogKeyUp);
            this.Closed += new EventHandler(this.OnEditSequenceDialogClosed);
            this.btnSave.Focus();
            this.SetTabIndex();
            this.Owner = Application.Current.MainWindow;
        }
        
        #endregion

        #region -- Public Events --

        /// <summary>
        /// This event would send the edited sequence to 
        /// controller to be saved.
        /// </summary>
        public event EventHandler<EditSequenceEventArgs> SaveEditedSequence;

        /// <summary>
        /// This event would inofrm the controller that 
        /// the dialog has been closed.
        /// </summary>
        public event EventHandler CancelEditDialog;

        #endregion

        #region -- Public Members --

        /// <summary>
        /// Gets or sets the sequence.
        /// </summary>
        public ISequence Sequence
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the sequence string being edited.
        /// </summary>
        public string SequenceString
        {
            get
            {
                return this.sequence;
            }

            set
            {
                this.sequence = value;
                if (!string.IsNullOrEmpty(this.sequence))
                {
                    this.txtSequence.Text = this.sequence;
                }
            }
        }

        #endregion

        #region -- Private Method --

        /// <summary>
        /// This method sets Tab index for the children controls.
        /// </summary>
        private void SetTabIndex()
        {
            this.txtSequence.TabIndex = 0;
            this.btnSave.TabIndex = 1;
            this.btnCancel.TabIndex = 2;
        }

        /// <summary>
        /// This method is fired when the dialog is 
        /// closed using the close button 
        /// </summary>
        /// <param name="sender">Edit sequence Dialog</param>
        /// <param name="e">Event Data</param>
        private void OnEditSequenceDialogClosed(object sender, EventArgs e)
        {
            if (this.CancelEditDialog != null)
            {
                this.CancelEditDialog(sender, e);
            }
        }

        /// <summary>
        /// This event closes the dialog on pressing the escape button.
        /// and would be a cancel action.
        /// </summary>
        /// <param name="sender">The edit dialog instance</param>
        /// <param name="e">Event data</param>
        private void OnEditSequenceDialogKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (this.CancelEditDialog != null)
                {
                    this.CancelEditDialog(sender, e);
                }

                this.Close();
            }
        }

        /// <summary>
        /// This event checks whether an edit has been made on the sequence or not.
        /// Depending upon the edit made, the save button will be enabled or disabled.
        /// </summary>
        /// <param name="sender">Keyboard down</param>
        /// <param name="e">Key event data</param>
        private void OnTxtSequenceKeyUp(object sender, KeyEventArgs e)
        {
            if (string.Equals(this.sequence, this.txtSequence.Text, StringComparison.CurrentCulture) || string.IsNullOrEmpty(this.txtSequence.Text))
            {
                this.btnSave.IsEnabled = false;
            }
            else if (!string.IsNullOrEmpty(this.txtSequence.Text) && !string.Equals(this.sequence, this.txtSequence.Text, StringComparison.CurrentCulture))
            {
                this.btnSave.IsEnabled = true;
            }            
        }

        /// <summary>
        /// This event informs that the dialog has been closed, and 
        /// to discard the changes made.
        /// </summary>
        /// <param name="sender">Cancel button</param>
        /// <param name="e">Event data</param>
        private void OnBtnCancelClick(object sender, RoutedEventArgs e)
        {
            if (this.CancelEditDialog != null)
            {
                this.CancelEditDialog(sender, e);
            }

            this.Close();
        }

        /// <summary>
        /// This event would be fired on save button click and the edited 
        /// sequence will be sent as the event argument.
        /// </summary>
        /// <param name="sender">Save button</param>
        /// <param name="e">Event data</param>
        private void OnBtnSaveClick(object sender, RoutedEventArgs e)
        {
            if (this.SaveEditedSequence != null)
            {
                EditSequenceEventArgs args = new EditSequenceEventArgs(this.txtSequence.Text, this.Sequence);
                this.SaveEditedSequence(sender, args);
            }
        }

        #endregion
    }
}
