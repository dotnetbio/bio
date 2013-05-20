namespace SequenceAssembler
{
    #region -- Using Directive --

    using System.Collections.Generic;
    using System.Windows;

    #endregion -- Using Directive --

    /// <summary>
    /// ColorSchemeDialog dialog will allow the user choose between DNA color scheme
    /// and Protein color scheme for Consensus custom view.
    /// </summary>
    public partial class ColorSchemeDialog
    {
        #region -- Private Members --

        /// <summary>
        /// Holds reference to the current color scheme chosen.
        /// </summary>
        private ColorSchemeInfo chosenColorScheme;

        /// <summary>
        /// Holds a reference to list of color schemes mentioned in App.config.
        /// </summary>
        private IList<ColorSchemeInfo> colorSchemes;

        #endregion -- Private Members --

        #region -- Constructor --

        /// <summary>
        /// Initializes a new instance of the ColorSchemeDialog class.
        /// </summary>
        /// <param name="colorSchemes">
        /// List of of color schemes mentioned in App.config.
        /// </param>
        /// <param name="currentlyChosenColorScheme">
        /// The current color scheme chosen.
        /// </param>
        public ColorSchemeDialog(IList<ColorSchemeInfo> colorSchemes, ColorSchemeInfo currentlyChosenColorScheme)
        {
            this.InitializeComponent();
            this.colorSchemes = colorSchemes;
            this.chosenColorScheme = currentlyChosenColorScheme;
            this.btnSave.Click += new RoutedEventHandler(this.OnSaveColorScheme);
            this.btnCancel.Click += new RoutedEventHandler(this.OnCancelColorScheme);
            this.Owner = Application.Current.MainWindow;
            this.SetColorSchemes();

            this.btnSave.Focus();
        }
        #endregion -- Constructor --
        
        #region -- Public Methods --

        /// <summary>
        /// This method displays the color dialog and waits for the user to choose the color
        /// scheme and returns the chosen color scheme back to the listener.
        /// </summary>
        /// <returns>The color scheme chosen by the user.</returns>
        public new ColorSchemeInfo Show()
        {
            this.ShowDialog();
            return this.chosenColorScheme;
        }

        #endregion -- Public Methods --

        #region -- Private Methods --

        /// <summary>
        /// This method is called when the user has made his selection
        /// for the color scheme and has decided to save it.
        /// </summary>
        /// <param name="sender">btnSave instance.</param>
        /// <param name="e">Event data.</param>
        private void OnSaveColorScheme(object sender, RoutedEventArgs e)
        {
            string selectedItem = this.cmbColorScheme.SelectedItem as string;
            foreach (ColorSchemeInfo info in this.colorSchemes)
            {
                if (info.Name.Equals(selectedItem))
                {
                    this.chosenColorScheme = info;
                    break;
                }
            }

            this.Close();
        }

        /// <summary>
        /// This method is called when the user has cancelled the color scheme dialog
        /// and does not wish to save his changes.
        /// </summary>
        /// <param name="sender">btnCancel instance.</param>
        /// <param name="e">Event data.</param>
        private void OnCancelColorScheme(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// This method initializes the combo box with the color schemes,
        /// and also sets the currently chosen color scheme as the selected
        /// item in teh combo box.
        /// </summary>
        private void SetColorSchemes()
        {
            if (this.colorSchemes.Count > 0)
            {
                foreach (ColorSchemeInfo info in this.colorSchemes)
                {
                    this.cmbColorScheme.Items.Add(info.Name);

                    if (info.Equals(this.chosenColorScheme))
                    {
                        this.cmbColorScheme.SelectedItem = info.Name;
                    }
                }
            }
        }

        #endregion -- Private Methods --
    }    
}
