namespace SequenceAssembler
{
    #region -- Using Directives --

    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using Bio;
    using System.Collections.Generic;
    #endregion

    /// <summary>
    /// Parsed file info would describe the file name and the associated 
    /// sequences parsed from the file.
    /// </summary>
    public class ParsedFileInfo
    {
        #region -- Private members --
        /// <summary>
        /// Describes the sequences associated with the file.
        /// </summary>
        private IList<ISequence> sequence;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the Parsed File info
        /// </summary>
        /// <param name="filename">File name of the parsed file</param>
        /// <param name="seq">sequences from the file</param>
        public ParsedFileInfo(string fileName, IList<ISequence> seq)
        {
            this.sequence = seq;
            this.FileName = fileName;
        }
        #endregion

        #region -- Public Properties --

        /// <summary>
        /// Gets or sets thename of the parsed file.
        /// </summary>
        public string FileName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the sequences from the file.
        /// </summary>
        public IList<ISequence> Sequence
        {
            get
            {
                return this.sequence;
            }
        }
        #endregion
    }
}
