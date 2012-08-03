namespace SequenceAssembler
{
    #region -- Using Directives --

    using System;
    using System.Collections.ObjectModel;
    using Bio;
    using System.Collections.Generic;

    #endregion -- Using Directives --

    /// <summary>
    /// SaveSequenceInputEventArgs holds the list of ISequences
    /// and the path of the file where the ISequences will have to
    /// be written to.
    /// </summary>
    public class SaveSequenceInputEventArgs : EventArgs
    {
        #region -- Private Members --

        /// <summary>
        /// Collection of sequences that have to be stored on
        /// the disk.
        /// </summary>
        private ICollection<ISequence> sequences;

        /// <summary>
        /// Path of the file where the ISequences will have to
        /// be written to.
        /// </summary>
        private string fileName;

        #endregion -- Private Members --

        #region -- Constructor --

        /// <summary>
        /// Custom constructor.
        /// </summary>
        /// <param name="sequences">
        /// Collection of sequences that have to be stored on
        /// the disk.
        /// </param>
        /// <param name="fileName">
        /// Path of the file where the ISequences will have to
        /// be written to.
        /// </param>
        public SaveSequenceInputEventArgs(ICollection<ISequence> sequences, string fileName)
        {
            this.sequences = sequences;
            this.fileName = fileName;
        }

        #endregion -- Constructor --

        #region -- Public Properties --

        /// <summary>
        /// Gets the sequences a collection of sequences that have to be stored on
        /// the disk.
        /// </summary>
        public ICollection<ISequence> Sequences
        {
            get 
            {
                return this.sequences; 
            }           
        }
        
        /// <summary>
        /// Gets the path of the file where the ISequences will have to
        /// be written to.
        /// </summary>
        public string FileName
        {
            get 
            { 
                return this.fileName; 
            }
        }

        #endregion -- Public Properties --
    }
}
