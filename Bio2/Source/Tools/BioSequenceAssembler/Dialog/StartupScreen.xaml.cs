namespace SequenceAssembler.Dialog
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    /// <summary>
    /// Interaction logic for StartupScreen.xaml
    /// </summary>
    public partial class StartupScreen : Window
    {
        /// <summary>
        /// Path of the selected file
        /// </summary>
        public string SelectedFilePath { get; set; }

        /// <summary>
        /// Flag to see if user requested to open a file from disk
        /// </summary>
        public bool ShowOpenFileDialog { get; set; }

        /// <summary>
        /// Initializes a new instance of the StartupScreen class
        /// </summary>
        public StartupScreen()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(OnStartupScreenLoaded);
        }

        /// <summary>
        /// Load recent files list
        /// </summary>
        private void OnStartupScreenLoaded(object sender, RoutedEventArgs e)
        {
            List<string> recentFiles = RecentFilesManager.RecentFiles;
            foreach (string filePath in recentFiles)
            {
                string filename = System.IO.Path.GetFileName(filePath);
                Button recentFileButton = new Button();
                recentFileButton.Content = filename;
                recentFileButton.Tag = filePath;
                recentFileButton.ToolTip = filePath;
                recentFileButton.Click += new RoutedEventHandler(OnRecentFileButtonClick);

                recentFilespanel.Children.Add(recentFileButton);
            }
        }

        /// <summary>
        /// Fired when user clicks on any recent file button
        /// </summary>
        /// <param name="sender">Recent file button being clicked</param>
        /// <param name="e">Event args</param>
        private void OnRecentFileButtonClick(object sender, RoutedEventArgs e)
        {
            Button recentFileButton = sender as Button;

            this.SelectedFilePath = recentFileButton.Tag as string;
            this.Close();
        }

        /// <summary>
        /// Close this window and take the user to the empty workspace
        /// </summary>
        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            ShowOpenFileDialog = false;
            SelectedFilePath = null;
            this.Close();
        }

        /// <summary>
        /// Raised when user clicks the open file button
        /// </summary>
        private void OnOpenClick(object sender, RoutedEventArgs e)
        {
            ShowOpenFileDialog = true;
            this.Close();
        }
    }
}
