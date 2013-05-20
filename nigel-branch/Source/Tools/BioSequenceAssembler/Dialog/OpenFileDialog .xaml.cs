namespace SequenceAssembler
{
    #region -- Using Directives --

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Bio;
    using Bio.IO;
    using SequenceAssembler.Properties;

    #endregion

    /// <summary>
    /// Interaction logic for OpenFileDialog.xaml. Open file dialogue will
    /// allow the user to select files containing DNA\Protein sequences.
    /// The file types that are supported by the Bio can
    /// be retrieved by querying Framework Abstraction classes.
    /// </summary>
    public partial class OpenFileDialog : Window
    {
        #region -- Private members --

        /// <summary>
        /// Describes the List of the extensions without the '.' character
        /// </summary>
        private Collection<string[]> extensionsList;

        /// <summary>
        /// Describes the Title of the format type.
        /// </summary>
        private Collection<string> fileTypeHeaders;

        /// <summary>
        /// Describes the Supported File Types by the Sequence Assembler
        /// </summary>
        private Collection<string> fileType;

        /// <summary>
        /// Describes Molecule Type
        /// </summary>
        private IAlphabet molecule;

        /// <summary>
        /// Describes the selected filenames. 
        /// </summary>
        private Collection<string> fileNames;

        /// <summary>
        /// Describes the files info of the uploaded file.
        /// </summary>
        private Collection<FileSystemInfo> fileInfo;

        /// <summary>
        /// Describes the open file dialog being escaped
        /// </summary>
        private bool dialogEscaped;

        /// <summary>
        /// Describes the selected parser.
        /// </summary>
        private string selectedParserName;
        #endregion

        #region -- Constructor --

        /// <summary>
        /// Initializes the Opendialog.
        /// </summary>
        /// <param name="types">Supported file Types</param>
        /// <param name="info">Collection of the files and the sequences parsed from them</param>
        /// <param name="showFileBrowserAtStartup">Indicates whether to show file browse dialog by default</param>
        public OpenFileDialog(Collection<string> types, Collection<FileSystemInfo> info, bool showFileBrowserAtStartup = false)
        {
            this.InitializeComponent();
            this.SetTabIndex();
            this.fileType = types;
            this.fileInfo = info;
            this.fileNames = new Collection<string>();
            this.extensionsList = new Collection<string[]>();
            this.fileTypeHeaders = new Collection<string>();
            this.ExtractExtensions();
            this.btnImport.Click += new RoutedEventHandler(this.OnImportButtonClick);
            this.btnCancel.Click += new RoutedEventHandler(this.OnCancelButtonClick);
            this.btnImportCancel.Click += new RoutedEventHandler(this.OnCancelAnimationButtonClick);
            this.btnBrowse.Click += new RoutedEventHandler(this.OnBrowseButtonClick);
            this.comboMoleculeType.SelectionChanged += new SelectionChangedEventHandler(this.ComboMoleculeTypeSelectionChanged);
            this.cmbParserType.SelectionChanged += new SelectionChangedEventHandler(this.OnParserSelectionChanged);
            this.PreviewKeyUp += new System.Windows.Input.KeyEventHandler(this.OnOpenFileDialogKeyUp);
            this.Closed += new EventHandler(this.OnOpenFileDialogClosed);
            this.btnBrowse.Focus();
            this.Owner = Application.Current.MainWindow;

            // Populate the Moleculetype combo box.
            this.comboMoleculeType.Items.Add(Resource.AUTO_DETECT_TEXT);
            IEnumerable<IAlphabet> moleculeTypes = Alphabets.All.OrderBy(alpha => alpha.Name);
            foreach (IAlphabet alpha in moleculeTypes)
            {
                this.comboMoleculeType.Items.Add(alpha.Name);
            }

            this.comboMoleculeType.SelectedIndex = 0;


            ComboBoxItem defaultItem = new ComboBoxItem();
            defaultItem.Content = Resource.MANUAL_CHOICE;
            this.cmbParserType.Items.Add(defaultItem);

            foreach (string parserName in SequenceParsers.All.Select(P => P.Name))
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = parserName;
                item.Tag = parserName;
                this.cmbParserType.Items.Add(item);
            }

            this.cmbParserType.SelectedIndex = 0;

            if (showFileBrowserAtStartup)
            {
                this.OnBrowseButtonClick(this, null);
            }
        }

        #endregion

        #region -- Public Events --

        /// <summary>
        /// Event to close the Pop up, It informs the 
        /// Controller that the pop is closed and to 
        /// close the Gray background.
        /// </summary>
        public event EventHandler ClosePopup;

        /// <summary>
        /// Event to cancel the import of files, It informs the 
        /// Controller to cancel the import of files.
        /// </summary>
        public event EventHandler CancelImport;

        /// <summary>
        /// Event to start import files, it informs the Controller 
        /// to start import the given files and molecule type.
        /// </summary>
        public event EventHandler<ImportFileEventArgs> ImportFile;
        #endregion

        #region -- Public Properties --

        /// <summary>
        /// Gets the different File Types
        /// </summary>
        public Collection<string> FileTypes
        {
            get
            {
                return this.fileType;
            }
        }
        #endregion

        #region -- Public Methods --

        /// <summary>
        /// Hides the animation and shows the 
        /// import and cancel button
        /// </summary>
        public void OnCancelImport()
        {
            buttonPanel.Visibility = Visibility.Visible;
            animationPanel.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region -- Private Methods --

        /// <summary>
        /// This method sets Tab index for the children controls.
        /// </summary>
        private void SetTabIndex()
        {
            this.btnBrowse.TabIndex = 0;
            this.comboMoleculeType.TabIndex = 1;
            this.cmbParserType.TabIndex = 2;
            this.btnImport.TabIndex = 3;
            this.btnCancel.TabIndex = 4;
        }

        /// <summary>
        /// This event is fired when the dialog is closed.
        /// </summary>
        /// <param name="sender">Open File Dialog</param>
        /// <param name="e">Event Data</param>
        private void OnOpenFileDialogClosed(object sender, EventArgs e)
        {
            //// Raise the event to controller, inform closing of the pop up
            if (this.ClosePopup != null)
            {
                this.ClosePopup(sender, e);
            }
        }

        /// <summary>
        /// Extracts the extensions from the supported file types
        /// It creates the array of the extensions without the '.' 
        /// character from the comma separated string
        /// </summary>
        private void ExtractExtensions()
        {
            const int SourceIndex = 1;
            const int DestinationIndex = 0;
            foreach (string type in this.FileTypes)
            {
                //// Split the comma separated values into the array.
                string[] filterTypes = type.Split(',');

                //// Add the Format type header in the headers list.
                this.fileTypeHeaders.Add(filterTypes[0]);
                string[] filters = new string[filterTypes.Length - 1];

                //// remove the Format type header from the array of the extensions 
                Array.Copy(filterTypes, SourceIndex, filters, DestinationIndex, filterTypes.Length - 1);
                int i = 0;

                //// replace the '.' character
                foreach (string filter in filters)
                {
                    filters[i] = filter.Replace(".", string.Empty);
                    i++;
                }

                //// create the extension list.
                this.extensionsList.Add(filters);
            }
        }

        /// <summary>
        /// This event close the dialog on escape button pressed, 
        /// it would be a cancel action.
        /// </summary>
        /// <param name="sender">OpenFileDialog Instance</param>
        /// <param name="e">Event data</param>
        private void OnOpenFileDialogKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape && !this.dialogEscaped)
            {
                //// Raise the event to controller, inform closing of the pop up
                if (this.ClosePopup != null)
                {
                    this.ClosePopup(sender, e);
                }

                //// Close the pop up.
                this.Close();
            }
            else
            {
                this.dialogEscaped = false;
            }
        }

        /// <summary>
        /// On import button click would inform the controller to import files,
        /// would also pass the list of filenames and the molecule type as event args.
        /// </summary>
        /// <param name="sender">Framework Element</param>
        /// <param name="e">Routed event args</param>
        private void OnImportButtonClick(object sender, RoutedEventArgs e)
        {
            //// Creates the collection of the File names.
            foreach (string file in this.fileNames)
            {
                FileInfo info = new FileInfo(file);
                if (this.fileInfo == null)
                {
                    this.fileInfo = new Collection<FileSystemInfo>();
                }

                this.fileInfo.Add(info);
            }

            ImportFileEventArgs importArgs = new ImportFileEventArgs(this.fileNames, this.molecule, this.fileInfo, this.selectedParserName);
            buttonPanel.Visibility = Visibility.Collapsed;
            animationPanel.Visibility = Visibility.Visible;
            if (this.ImportFile != null)
            {
                this.ImportFile(sender, importArgs);
            }
        }

        /// <summary>
        /// On cancel button click would close the Importing dialog and would 
        /// inform the controller
        /// </summary>
        /// <param name="sender">Framework Element</param>
        /// <param name="e">Routed Event args</param>
        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            //// Raise the event to controller, inform closing of the pop up
            if (this.ClosePopup != null)
            {
                this.ClosePopup(sender, e);
            }

            //// Close the pop up.
            this.Close();
        }

        /// <summary>
        /// On cancel button click of Importing of files would inform 
        /// the controller to cancel the import of files through events  
        /// </summary>
        /// <param name="sender">Framework Element</param>
        /// <param name="e">Routed events args</param>
        private void OnCancelAnimationButtonClick(object sender, RoutedEventArgs e)
        {
            //// Raise the event 
            if (this.CancelImport != null)
            {
                this.CancelImport(sender, e);
            }
        }

        /// <summary>
        /// Handles the click on the Browse button,Launches the Windows Open File dialog
        /// with custom File formats filters being set to the dialog.
        /// On selection of files shows the paths of the selected files on the screen.
        /// Gives option to import the files.
        /// </summary>
        /// <param name="sender">Framework Element</param>
        /// <param name="e">Routed event args</param>
        private void OnBrowseButtonClick(object sender, RoutedEventArgs e)
        {
            //// Launch the FileDialog
            this.LaunchWindowFileDialog();
        }

        /// <summary>
        /// Launches the File Dialog, creates the selected filenames list, 
        /// also validates the selected file name for import.
        /// </summary>
        /// <param name="fileDialog">OpenFiledialog instance to be launched</param>
        private void LaunchWindowFileDialog()
        {
            //// Create and launch the Windows File Dialog, Set various validations
            using (System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                fileDialog.Multiselect = true;
                fileDialog.CheckFileExists = true;
                fileDialog.CheckPathExists = true;
                fileDialog.Filter = Resource.AllFilesFilter;

                // On SuccessFull selection of the files. 
                if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Reset the file name collection
                    this.fileNames = new Collection<string>();
                    this.fileNameList.Items.Clear();

                    string autoParser = null;
                    string prevParser = null;

                    bool chooseParserManually = false;

                    //// Validate the file type and create a list of file names to be displayed on the screen.
                    foreach (string file in fileDialog.FileNames)
                    {
                        if (!this.CheckDuplicateFile(file))
                        {
                            MessageDialogBox.Show(Resource.DUPLICATE_FILE, Resource.CAPTION, MessageDialogButton.OK);
                            return;
                        }

                        ISequenceParser parser = SequenceParsers.FindParserByFileName(file);

                        autoParser = parser == null ? null : parser.Name;

                        if (fileDialog.FileNames[0].Equals(file))
                        {
                            prevParser = autoParser;
                        }

                        if (autoParser == null || prevParser == null || !autoParser.Equals(prevParser))
                        {
                            chooseParserManually = true;
                        }

                        prevParser = autoParser;
                    }

                    //// Creates the collection of the File names.
                    foreach (string file in fileDialog.FileNames)
                    {
                        if (!this.fileNames.Contains(file))
                        {
                            this.fileNames.Add(file);
                            this.fileNameList.Items.Add(new ListViewItem { Content = new FileInfo(file).Name, ToolTip = file });
                        }
                    }

                    //// Enables the Molecule type.                 
                    this.comboMoleculeType.IsEnabled = true;
                    this.comboMoleculeType.SelectedIndex = 0;

                    //// Enables the Parser selection.                 
                    this.cmbParserType.IsEnabled = true;

                    if (chooseParserManually == true)
                    {
                        this.cmbParserType.SelectedIndex = 0;
                        this.btnImport.IsEnabled = false;
                    }
                    else
                    {
                        foreach (ComboBoxItem item in this.cmbParserType.Items)
                        {
                            string parser = item.Content as string;

                            if (parser != null && autoParser != null)
                            {
                                if (parser.Equals(autoParser))
                                {
                                    this.cmbParserType.SelectedItem = item;
                                    break;
                                }
                            }
                        }

                        this.btnImport.IsEnabled = true;
                    }

                    this.btnImport.Focus();
                }
                else
                {
                    this.btnBrowse.Focus();
                    this.dialogEscaped = true;
                }
            }
        }

        /// <summary>
        /// This method checks the file selected for import is 
        /// duplicate or a new file
        /// </summary>
        /// <param name="file">File Name to be imported</param>
        /// <returns>Status whether the file is duplicate or not</returns>
        private bool CheckDuplicateFile(string file)
        {
            FileInfo loadFileInfo = new FileInfo(file);
            if (this.fileInfo != null && this.fileInfo.Count > 0)
            {
                foreach (FileInfo info in this.fileInfo)
                {
                    if (info.FullName == loadFileInfo.FullName || info.LastWriteTime == loadFileInfo.LastWriteTime)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Handles the selection changed on the Combo box to select Molecule type.
        /// This would set the molecule type to be used while importingthe given files.
        /// </summary>
        /// <param name="sender">Framework element</param>
        /// <param name="e">selection changed event args</param>
        private void ComboMoleculeTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (comboMoleculeType.SelectedIndex)
            {
                case 0: this.molecule = null;
                    break;
                default:
                    foreach (IAlphabet alpha in Alphabets.All)
                    {
                        if (comboMoleculeType.SelectedValue != null && alpha.Name.Equals(comboMoleculeType.SelectedValue.ToString()))
                        {
                            this.molecule = alpha;
                            break;
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// Handles the selection changed on the Combo box to select Parser type.
        /// This would set the Parser type to be used while parsing the given files.
        /// </summary>
        /// <param name="sender">Framework element</param>
        /// <param name="e">selection changed event args</param>
        private void OnParserSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cmbParserType.SelectedIndex == 0)
            {
                this.btnImport.IsEnabled = false;
                this.selectedParserName = null;
            }
            else
            {
                this.btnImport.IsEnabled = true;
                ComboBoxItem item = this.cmbParserType.SelectedItem as ComboBoxItem;
                this.selectedParserName = item.Content as string;
            }
        }

        #endregion
    }
}
