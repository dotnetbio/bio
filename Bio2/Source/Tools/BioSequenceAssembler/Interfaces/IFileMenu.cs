namespace SequenceAssembler
{
    #region -- Using Directives --
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using Bio;
    #endregion

    /// <summary>
    /// IFileMenu would be implemented by the Filemenu Usercontrol.
    /// This will be used by the Sequence assembler to display 
    /// the supported file types in the file dialogue. 
    /// </summary>
    public interface IFileMenu
    {
        /// <summary>
        /// Event to indicate that IFileMenu is launching a pop-up.
        /// This gives the hosting control a chance to implement custom
        /// actions when a pop-up is opened.
        /// </summary>
        event EventHandler PopupOpened;

        /// <summary>
        /// Event to indicate that IFileMenu is closing a pop-up.
        /// This gives the hosting control a chance to implement custom
        /// actions when a pop-up is closed.
        /// </summary>
        event EventHandler PopupClosed;

        /// <summary>
        /// Event to indicate that user has cancelled
        /// importing the sequence file which wouldve been parsed.
        /// </summary>
        event EventHandler CancelImport;

        /// <summary>
        /// Event to indicate that user has selected
        /// the sequence file which has to be parsed.
        /// </summary>
        event EventHandler<ImportFileEventArgs> ImportFile;

        /// <summary>
        /// Event to indicate that the Save the contig or sequence
        /// have been cancelled.
        /// </summary>
        event EventHandler SaveWorkspace;

        /// <summary>
        /// Event to indicate that the user wants to exit the application.
        /// </summary>
        event EventHandler CloseWindow;

        /// <summary>
        /// Gets the files loaded into SequenceViewer as Collection of FileInfo objects.
        /// </summary>
        Collection<FileSystemInfo> FileInfo { get; }

        /// <summary>
        /// Lists all the file types available for selection to the End-user.
        /// </summary>
        /// <param name="supportedFileTypes">
        /// Collection of supported file extensions.
        /// </param>
        void DisplayFileTypes(Collection<string> supportedFileTypes);

        /// <summary>
        /// Open the open file dialog window with the file browser open by default
        /// </summary>
        void ShowOpenFileDialog();

        /// <summary>
        /// CloseDialogue will be called by hosting control
        /// to when parsing the sequence file has been completed.
        /// </summary>
        void CloseDialogue();

        /// <summary>
        /// This method would remove the file info from the current 
        /// fileinfo colection
        /// </summary>
        /// <param name="info">File Info of the file to be removed</param>
        void RemoveFileInfo(FileInfo info);

        /// <summary>
        /// This method updates Enable status of the Save menu item. 
        /// </summary>
        /// <param name="status">if true Enabled else Disabled</param>
        void UpdateSaveStatus(bool status);

        /// <summary>
        /// Hides the animation and shows the 
        /// import and cancel button
        /// </summary>
        void OnCancelImport();
    }
}
