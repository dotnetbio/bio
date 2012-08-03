namespace SequenceAssembler
{
    #region -- Using Directives --
    
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    #endregion

    /// <summary>
    /// This defines the custom Event Arguments for unloading the files.
    /// It contains FileNames to be removed with the File Info.
    /// </summary>
    public class FileUnloadedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes the File unload event args.
        /// </summary>
        /// <param name="info">info of the file to be removed</param>
        public FileUnloadedEventArgs(FileInfo info)
        {
            this.RemoveFileInfo = info;
        }

        #region -- Public properties -- 

        /// <summary>
        /// Gets or sets the info of the file to be removed.
        /// </summary>
        public FileInfo RemoveFileInfo
        {
            get;
            set;
        }
        #endregion
    }
}
