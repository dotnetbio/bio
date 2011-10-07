namespace SequenceAssembler
{
    #region -- Using Directives --
    using System;
    using System.Collections.ObjectModel;
    using Bio;
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
        #endregion

        #region -- Constructor --
        /// <summary>
        /// Initiliazes the ImportFileArgs with list of filenames
        /// and the associated molecule type
        /// </summary>
        /// <param name="names">List of filename</param>
        /// <param name="molecule">Molecule type</param>
        public ImportFileEventArgs(Collection<string> names, MoleculeType molecule)
        {
            this.fileNames = names;
            this.Molecule = molecule;
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
        /// Gets or sets the Molecule associated with the Files
        /// </summary>
        public MoleculeType Molecule
        {
            get;
            set;
        }
        #endregion
    }
}
