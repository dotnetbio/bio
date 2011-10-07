namespace SequenceAssembler
{
    #region -- Using Directive

    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Documents;
    using Bio.Web.Blast;

    #endregion -- Using Directive

    /// <summary>
    /// BlastHeader displays information of the Blast service like
    /// Version, reference, database name etc.
    /// </summary>
    public partial class BlastHeader : Window
    {
        #region -- Constructor --

        /// <summary>
        /// Initializes a new instance of the BlastHeader class.
        /// </summary>
        /// <param name="metadata">BlastXmlMetadata instance.</param>
        public BlastHeader(BlastXmlMetadata metadata)
        {
            this.InitializeComponent();

            // Display Metadata information.
            this.txtReference.Text = metadata.Reference;
            this.txtVersion.Text = metadata.Version;
            this.txtRid.Text = metadata.QueryId;
            this.txtQuery.Text = metadata.QueryLength + " " + Properties.Resource.HEADER_LETTER;
            this.txtDatabase.Text = metadata.Database;

            // Initialize hyperlinks.
            this.linkFaq.Click += new RoutedEventHandler(this.OnHyperLinkClicked);
            this.linkFaq.NavigateUri = new Uri(Properties.Resource.FAQ_LINK);

            this.linkTaxonomy.Click += new RoutedEventHandler(this.OnHyperLinkClicked);
            this.linkTaxonomy.NavigateUri = new Uri(Properties.Resource.TAXONAMY_LINK);

            this.KeyUp += new System.Windows.Input.KeyEventHandler(OnDialogKeyUp);

            this.Owner = Application.Current.MainWindow;
        }

        #endregion -- Constructor --

        #region -- Private Methods --

        /// <summary>
        /// This method processes a hyper link click event and
        /// starts the appropriate process to navigate to that
        /// particular link.
        /// </summary>
        /// <param name="sender">Hyperlink instances.</param>
        /// <param name="e">Event data.</param>
        private void OnHyperLinkClicked(object sender, RoutedEventArgs e)
        {
            Hyperlink blastFaqLink = (Hyperlink)sender;
            string navigateUri = blastFaqLink.NavigateUri.ToString();
            Process.Start(new ProcessStartInfo(navigateUri));
            e.Handled = true;
        }

        /// <summary>
        /// This event close the dialog on escape button pressed, 
        /// it would be a cancel action.
        /// </summary>
        /// <param name="sender">Dialog Instance</param>
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
