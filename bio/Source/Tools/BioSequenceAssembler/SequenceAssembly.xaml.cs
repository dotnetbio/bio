namespace SequenceAssembler
{
    #region -- Using directives --

    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using SequenceAssembler.Dialog;
    using System.Diagnostics;
    using System.IO;

    #endregion -- Using directives --

    /// <summary>
    /// Interaction logic for Window1.xaml.SequenceAssembly is the parent window
    /// which will host Assembler pane and Blast pane.
    /// Assembler shows sequence view and consesus view. Blast pane shows the 
    /// output of BLAST webservice.
    /// </summary>
    public partial class SequenceAssembly : Window, IDisposable
    {
        #region -- Private Members --

        /// <summary>
        /// Instance of the controller class which will
        /// control the interaction between various user-controls.
        /// </summary>
        private Controller controller;

        /// <summary>
        /// Prevents IDisposable.Dispose() getting executed multiple times.
        /// </summary>
        private bool disposed;

        #endregion -- Private Members --

        #region -- Public Members --
        /// <summary>
        /// Holds a list of all the color schemes mentioned in the app.config file.
        /// </summary>
        public static List<ColorSchemeInfo> colorSchemeInfo;

        /// <summary>
        /// Holds a reference to the current chosen color scheme.
        /// </summary>
        public static ColorSchemeInfo chosenColorScheme;

        /// <summary>
        /// Holds a mapping of string name and its System.Windows.Media.Color object.
        /// </summary>
        public static Dictionary<string, Color> colorLookUpTable = new Dictionary<string, Color>();

        #endregion -- Public Members --

        #region -- Public Constructor --

        /// <summary>
        /// Initializes a new instance of the SequenceAssembly class.
        /// </summary>
        public SequenceAssembly()
        {
            try
            {
                this.InitializeComponent();

                // Initialize Controller class.
                this.SetTabIndex();
                this.controller = new Controller();
                this.controller.Assembler = this.assembler;
                this.controller.FileMenu = this.fileMenu;
                this.controller.WebServicePresenter = this.assembler.webServicePresenter;
                this.controller.PopupOpened += this.OnPopUpOpened;
                this.controller.PopupClosed += this.OnPopUpClosed;
                this.controller.SearchCompleted += this.OnSearchCompleted;
                this.controller.WindowClosed += this.OnWindowExited;
                this.menuAbout.Click += new RoutedEventHandler(this.OnAboutClicked);
                this.menuUserGuide.Click += new RoutedEventHandler(this.OnMenuUserGuideClick);
                this.optionsMenuChangeColors.IsEnabled = SequenceAssembly.colorSchemeInfo.Count > 1;
                this.optionsMenuChangeColors.Click += new RoutedEventHandler(OnChangeColorsClicked);
                this.optionsMenuAssociateFileTypes.Click += new RoutedEventHandler(OnAssociateFileTypesClicked);
                this.Closing += new System.ComponentModel.CancelEventHandler(this.OnWindowClosed);
                this.Loaded += new RoutedEventHandler(this.OnSequenceAssemblyLoaded);
            }
            catch (TypeInitializationException ex)
            {
                MessageBox.Show(ex.InnerException.Message, "Add-In Failure");
            }
        }

        #endregion -- Public Constructor --

        #region -- Public Methods --

        /// <summary>
        /// Method to dispose on all objects 
        /// implementing IDisposable interface.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion -- Public Methods --

        #region -- Private Methods --

        /// <summary>
        /// Fired when user clicks the User Guide option in the Help menu
        /// </summary>
        private void OnMenuUserGuideClick(object sender, RoutedEventArgs e)
        {
            string userGuidePath = AppDomain.CurrentDomain.BaseDirectory + Properties.Resource.UserGuideRelativePath;
            if (File.Exists(userGuidePath))
            {
                Process.Start(userGuidePath);
            }
            else
            {
                MessageDialogBox.Show(Properties.Resource.NoUserGuidePresent, Properties.Resource.CAPTION, MessageDialogButton.OK);
            }
        }

        /// <summary>
        /// This event loads any files specified in command line to the sequence viewer.
        /// </summary>
        /// <param name="sender">SequeceAssembly instance.</param>
        /// <param name="e">Routed event args.</param>
        private void OnSequenceAssemblyLoaded(object sender, RoutedEventArgs e)
        {
            string[] filenames = Application.Current.Properties["FilesToLoad"] as string[];
            if (filenames != null && this.controller != null)
            {
                this.controller.LoadFiles(new List<string>(filenames));
            }
            else
            {
                StartupScreen startupScreen = new StartupScreen();
                startupScreen.Owner = this;
                startupScreen.ShowDialog();

                if (startupScreen.SelectedFilePath != null)
                {
                    this.controller.LoadFiles(new List<string> { startupScreen.SelectedFilePath });
                }
                else if (startupScreen.ShowOpenFileDialog)
                {
                    this.applicationHelpText.Visibility = System.Windows.Visibility.Collapsed;
                    this.controller.ShowOpenFileDialog();
                }
                else
                {
                    assembler.sequenceViewExpander.IsExpanded = true;
                    assembler.consensusViewExpander.IsExpanded = false;
                    assembler.blastResultsExpander.IsExpanded = false;
                    this.overlayrect.Visibility = System.Windows.Visibility.Hidden;
                    this.applicationHelpText.Visibility = System.Windows.Visibility.Collapsed;

                    assembler.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// This method sets Tab index for the children controls.
        /// </summary>
        private void SetTabIndex()
        {
            this.fileMenu.TabIndex = 0;
            this.optionsMenu.TabIndex = 1;
            this.helpMenu.TabIndex = 2;
        }

        /// <summary>
        /// This event will display a black background, indicating that
        /// the user cannot interact with the background window, when there
        /// is a pop-up open.
        /// </summary>
        /// <param name="sender">Controller object.</param>
        /// <param name="e">Event data.</param>
        private void OnPopUpOpened(object sender, EventArgs e)
        {
            panelDialog.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// This event will remove the black background, indicating that
        /// the user can now interact with the background window, when a 
        /// pop-up is closed, but this close represents no files were imported.
        /// </summary>
        /// <param name="sender">Controller object.</param>
        /// <param name="e">Event data.</param>
        private void OnPopUpClosed(object sender, PopupEventArgs e)
        {
            if (e.Status)
            {
                panelDialog.Visibility = Visibility.Collapsed;
                this.overlayrect.Visibility = Visibility.Collapsed;
                this.applicationHelpText.Visibility = Visibility.Collapsed;
            }
            else
            {
                panelDialog.Visibility = Visibility.Collapsed;
                this.overlayrect.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// This method is invoked when the user wants
        /// to close the window using Load > Exit click.
        /// </summary>
        /// <param name="sender">Controller instance.</param>
        /// <param name="e">Event data.</param>
        private void OnWindowExited(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// This method calls dispose on all objects 
        /// implementing IDisposable interface. This method
        /// has logic to prevent disposing objects multiple times.
        /// </summary>
        /// <param name="disposable">
        /// If true then it indicates non-framework
        /// code is calling dispose.
        /// </param>
        private void Dispose(bool disposable)
        {
            if (!this.disposed)
            {
                if (disposable)
                {
                    if (this.controller != null)
                    {
                        this.controller.Dispose();
                    }

                    this.disposed = true;
                }
            }
        }

        /// <summary>
        /// This method gets fired when online search operation has been completed.
        /// </summary>
        /// <param name="sender">Controller instance.</param>
        /// <param name="e">Event data.</param>
        private void OnSearchCompleted(object sender, EventArgs e)
        {
            this.assembler.blastResultsExpander.IsExpanded = true;
        }

        /// <summary>
        /// This method is called when the user wishes to see about dialog.
        /// </summary>
        /// <param name="sender">About menu item.</param>
        /// <param name="e">Event data.</param>
        private void OnAboutClicked(object sender, RoutedEventArgs e)
        {
            AboutScreen about = new AboutScreen();
            about.Owner = this;
            about.ShowDialog();
        }

        /// <summary>
        /// This method is called when the user wishes the change the sequence color.
        /// </summary>
        /// <param name="sender">Change Colors menu item.</param>
        /// <param name="e">Event data.</param>
        private void OnChangeColorsClicked(object sender, RoutedEventArgs e)
        {
            ColorSchemeDialog dialog = new ColorSchemeDialog(colorSchemeInfo, chosenColorScheme);
            dialog.Owner = Application.Current.MainWindow;
            ColorSchemeInfo info = dialog.Show();
            SequenceAssembly.chosenColorScheme = info;

            assembler.customSequenceView.ApplyCurrentColor();
            assembler.consensusCustomView.ApplyCurrentColor();
        }

        /// <summary>
        /// This method is called when the user selects Associate file type menu item.
        /// </summary>
        /// <param name="sender">Menu item.</param>
        /// <param name="e">Event args.</param>
        private void OnAssociateFileTypesClicked(object sender, RoutedEventArgs e)
        {
            FileTypeAssociationDialog dialog = new FileTypeAssociationDialog();
            dialog.Owner = Application.Current.MainWindow;
            dialog.ShowDialog();
        }

        /// <summary>
        /// This event is called when the user closes the window.Before closing
        /// the window the user is asked to confirm if he really wants to close the
        /// window.
        /// </summary>
        /// <param name="sender">Window instance.</param>
        /// <param name="e">Event data.</param>
        private void OnWindowClosed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageDialogResult.No == MessageDialogBox.Show(Properties.Resource.EXIT_APPLICATION, Properties.Resource.CAPTION, MessageDialogButton.YesNo))
            {
                e.Cancel = true;
            }
        }

        #endregion -- Private Methods --
    }
}
