namespace SequenceAssembler
{
    #region -- Using Directives --

    using System.Windows;

    #endregion  -- Using Directives --

    /// <summary>
    /// MessageDialog class will be used in the application to show custom messages
    /// to the user.
    /// </summary>
    public partial class MessageDialog : Window
    {
        /// <summary>
        /// Default Construtor.
        /// </summary>
        public MessageDialog()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.SetTabIndex();
        }

        /// <summary>
        /// Initializes the MessageDialog window with the message, caption
        /// and the button which have to be displayed.
        /// </summary>
        /// <param name="message">Message to be displayed in the dialog box.</param>
        /// <param name="caption">Caption of the dialog box</param>
        /// <param name="buttons">Buttons which will be displayed to the users.</param>
        public void Initialize(string message, string caption, MessageDialogButton buttons)
        {
            this.txtSequence.Text = message;
            this.txtCaption.Text = caption;

            if (buttons == MessageDialogButton.YesNo)
            {
                this.btnYes.Visibility = Visibility.Visible;
                this.btnNo.Visibility = Visibility.Visible;                
                this.btnYes.Focus();
            }
            else if (buttons == MessageDialogButton.OK)
            {
                this.btnOk.Visibility = Visibility.Visible;
                this.btnOk.Focus();
            }
        }

        #region -- Private Members --

        /// <summary>
        /// This method sets Tab index for the children controls.
        /// </summary>
        private void SetTabIndex()
        {
            this.btnOk.TabIndex = 0;
            this.btnYes.TabIndex = 1;
            this.btnNo.TabIndex = 2;
        }
        #endregion
    }
}
