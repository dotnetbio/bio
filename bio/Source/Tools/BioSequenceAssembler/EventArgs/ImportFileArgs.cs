namespace SequenceAssembler
{
    #region -- Using Directives --
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using Bio;
    using Bio.IO;
    #endregion
    /// <summary>
    /// This defines the custom Event Arguments for importing the files.
    /// It contains FileNames to be imported with the Molecule, 
    /// associated with the Files
    /// </summary>
    public class ImportFileEventArgs : EventArgs
    {
        #region -- Private Members --
        /// <summary>
        /// Describes the collection of filenames
        /// </summary>
        private Collection<string> fileNames = new Collection<string>();

        /// <summary>
        /// Describes the collection of fileinfos of the uploaded files
        /// </summary>
        private Collection<FileSystemInfo> fileInfo = new Collection<FileSystemInfo>();
        #endregion

        #region -- Constructor --
        /// <summary>
        /// Initializes a new instance of the ImportFileEventArgs class.
        /// </summary>
        /// <param name="names">List of filename</param>
        /// <param name="molecule">Molecule type</param>
        /// <param name="info">Collection of the File names and sequence parsed from them.</param>
        /// <param name="parser">Parser to be used to parse these files.</param>
        public ImportFileEventArgs(Collection<string> names, IAlphabet molecule, Collection<FileSystemInfo> info, string selectedParserName)
        {
            this.fileNames = names;
            this.Molecule = molecule;
            this.fileInfo = info;
            this.ParserName = selectedParserName;
        }
        #endregion

        #region -- Public Properties --
        
        /// <summary>
        /// Gets the collection of filenames
        /// </summary>
        public Collection<string> FileNames
        {
            get
            {
                return this.fileNames;
            }
        }

        /// <summary>
        /// Gets the collection of File infos of the uploaded files
        /// </summary>
        public Collection<FileSystemInfo> FileInfo
        {
            get
            {
                return this.fileInfo;
            }
        }

        /// <summary>
        /// Gets or sets the Molecule associated with the Files
        /// </summary>
        public IAlphabet Molecule
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Parser to be used with these files.
        /// </summary>
        public string ParserName
        {
            get;
            set;
        }
        #endregion
    }
}
