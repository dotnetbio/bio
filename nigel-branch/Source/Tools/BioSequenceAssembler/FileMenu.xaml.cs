namespace SequenceAssembler
{
    #region -- Using Directives --
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    #endregion

    /// <summary>
    /// Interaction logic for FileMenu.xaml, It would also implement 
    /// the IFileMenu interface.It would provide User with Options:
    ///    1. Open - Would open the File Dialog, from which User 
    ///              will select the FASTA , GENBANK etc files to Import 
    ///              it to the Sequence Assembler.
    ///    3. Save As - Saving the sequence in a given format.
    /// </summary>
    public partial class FileMenu : MenuItem, IFileMenu
    {
        #region -- Private members --

        /// <summary>
        /// Describes the File Types supported for importing.
        /// </summary>
        private Collection<string> fileTypes;

        /// <summary>
        /// Keeps track of the last file dialog opened.
        /// </summary>
        private OpenFileDialog dialog;

        /// <summary>
        /// Describes the FileInfo of the uploaded sequences
        /// </summary>
        private Collection<FileSystemInfo> fileInfo;
        #endregion

        #region -- Properties --
        /// <summary>
        /// Gets the files loaded into SequenceViewer as Collection of FileInfo objects.
        /// </summary>
        public Collection<FileSystemInfo> FileInfo
        {
            get
            {
                return this.fileInfo;
            }
        }
        #endregion -- Properties --

        #region -- Constructor --

        /// <summary>
        /// Initializes a new instance of the FileMenu class
        /// </summary>
        public FileMenu()
        {
            InitializeComponent();
            this.fileTypes = new Collection<string>();
            this.fileInfo = new Collection<FileSystemInfo>();
            this.openItem.Click += new RoutedEventHandler(this.OnOpenItemClick);
            this.saveItem.Click += new RoutedEventHandler(this.OnSaveItemClick);
            this.exitMenu.Click += new RoutedEventHandler(this.OnExitMenuClick);
        }

        #endregion

        #region -- Public Events --

        /// <summary>
        /// Event to Show Dialog which would bring 
        /// the Graying effect to the Dialog
        /// </summary>
        public event EventHandler PopupOpened;

        /// <summary>
        /// Event to close the Dialog which would close 
        /// the Gray effect for the dialog.
        /// </summary>
        public event EventHandler PopupClosed;

        /// <summary>
        /// Event to indicate that the import of sequence files
        /// have been cancelled.
        /// </summary>
        public event EventHandler CancelImport;

        /// <summary>
        /// Event to indicate that the Save the contig or sequence
        /// have been cancelled.
        /// </summary>
        public event EventHandler SaveWorkspace;

        /// <summary>
        /// Event to indicate that the import of sequence files
        /// has to start.
        /// </summary>
        public event EventHandler<ImportFileEventArgs> ImportFile;

        /// <summary>
        /// Event to indicate that the user wants to exit application.
        /// </summary>
        public event EventHandler CloseWindow;

        #endregion

        #region -- Public methods --

        /// <summary>
        /// Initializes the fileTypes with supported file types by 
        /// the Sequence Assembler. 
        /// </summary>
        /// <param name="supportedFileTypes">List of supported file types</param>    
        public void DisplayFileTypes(Collection<string> supportedFileTypes)
        {
            this.fileTypes = supportedFileTypes;
        }

        /// <summary>
        /// This method closes the OpenFileDialog.
        /// </summary>
        public void CloseDialogue()
        {
            if (this.dialog != null)
            {
                this.dialog.Close();
            }

            if (this.PopupClosed != null)
            {
                this.PopupClosed(this, new EventArgs());
            }
        }

        /// <summary>
        /// This method updates Enable status of the Save menu item. 
        /// </summary>
        /// <param name="status">if true Enabled else Disabled</param>
        public void UpdateSaveStatus(bool status)
        {
            this.saveItem.IsEnabled = status;
        }

        /// <summary>
        /// This method would remove the file info from the current 
        /// fileinfo colection
        /// </summary>
        /// <param name="info">File Info of the file to be removed</param>
        public void RemoveFileInfo(FileInfo info)
        {
            FileInfo removeFile = null;
            foreach (FileInfo file in this.fileInfo)
            {
                if (file.LastWriteTime == info.LastWriteTime)
                {
                    removeFile = file;
                    break;
                }
            }

            this.fileInfo.Remove(removeFile);
        }

        /// <summary>
        /// Hides the animation and shows the 
        /// import and cancel button
        /// </summary>
        public void OnCancelImport()
        {
            if (this.dialog != null)
            {
                this.dialog.OnCancelImport();
            }
        }

        #endregion

        #region -- Private methods --

        /// <summary>
        /// Handles the Event raised from the open file dialog
        /// would raise the event to close the Gray back ground.
        /// </summary>
        /// <param name="sender">Framework element</param>
        /// <param name="e"> Event Args</param>
        private void OnPopupClose(object sender, EventArgs e)
        {
            if (this.PopupClosed != null)
            {
                this.PopupClosed(sender, e);
            }
        }

        /// <summary>
        /// Handles the click action on the Save Item of the File menu.
        /// This would Save the Sequence or contig and save it in the 
        /// supported file format.
        /// </summary>
        /// <param name="sender">Save menu Item</param>
        /// <param name="e">Event Data</param>
        private void OnSaveItemClick(object sender, RoutedEventArgs e)
        {
            if (this.SaveWorkspace != null)
            {
                this.SaveWorkspace(sender, e);
            }
        }

        /// <summary>
        /// Open the file open dialog with the file browser open by default
        /// </summary>
        public void ShowOpenFileDialog()
        {
            this.dialog = new OpenFileDialog(this.fileTypes, this.fileInfo, true);
            this.dialog.ClosePopup += new EventHandler(this.OnPopupClose);
            this.dialog.ImportFile += new EventHandler<ImportFileEventArgs>(this.OnFileImport);
            this.dialog.CancelImport += new EventHandler(this.OnFileImportCancelled);

            if (this.PopupOpened != null)
            {
                this.PopupOpened(this ,null);
            }

            this.dialog.ShowDialog();
        }

        /// <summary>
        /// Handles the click action on the Open Item of the File menu.
        /// This would open the custom File Dialog and initialize the 
        /// supported files types for the dialog.
        /// </summary>
        /// <param name="sender">Framework Element</param>
        /// <param name="e">Routed event args</param>
        private void OnOpenItemClick(object sender, RoutedEventArgs e)
        {
            this.dialog = new OpenFileDialog(this.fileTypes, this.fileInfo);
            this.dialog.ClosePopup += new EventHandler(this.OnPopupClose);
            this.dialog.ImportFile += new EventHandler<ImportFileEventArgs>(this.OnFileImport);
            this.dialog.CancelImport += new EventHandler(this.OnFileImportCancelled);

            if (this.PopupOpened != null)
            {
                this.PopupOpened(sender, e);
            }

            this.dialog.ShowDialog();
        }

        /// <summary>
        /// Indicates that the sequence files have to be
        /// parsed.
        /// </summary>
        /// <param name="sender">OpenFileDialog instance</param>
        /// <param name="e">Event data</param>
        private void OnFileImport(object sender, ImportFileEventArgs e)
        {
            this.fileInfo = e.FileInfo;
            if (this.ImportFile != null)
            {
                this.ImportFile(sender, e);
            }
        }

        /// <summary>
        /// Indicates that the file import has to be
        /// cancelled.
        /// </summary>
        /// <param name="sender">OpenFileDialog instance</param>
        /// <param name="e">Event data</param>
        private void OnFileImportCancelled(object sender, EventArgs e)
        {
            if (this.CancelImport != null)
            {
                this.CancelImport(sender, e);
            }
        }

        /// <summary>
        /// Indicates that the user has clicked on Exit option in the file menu.
        /// This informs the controller to close the window.
        /// </summary>
        /// <param name="sender">Exit menu.</param>
        /// <param name="e">Event data.</param>
        private void OnExitMenuClick(object sender, RoutedEventArgs e)
        {
            if (this.CloseWindow != null)
            {
                this.CloseWindow(sender, e);
            }
        }

        #endregion
    }
}
