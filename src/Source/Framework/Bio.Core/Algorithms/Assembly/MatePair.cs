using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Bio.Algorithms.Assembly
{
    /// <summary>
    /// Stores mate pair information.
    /// </summary>
    public class MatePair
    {
        #region Field Variables

        /// <summary>
        /// Stores sequence ID of forward read.
        /// </summary>
        private string forwardRead;

        /// <summary>
        /// Stores sequence ID of reverse read.
        /// </summary>
        private string reverseRead;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the MatePair class with specified library name.
        /// </summary>
        /// <param name="library">Library name.</param>
        public MatePair(string library)
        {
            if (Validate(library))
            {
                this.Library = library;
            }
        }

        /// <summary>
        /// Initializes a new instance of the MatePair class with specified library name,
        /// forward read and reverse read.
        /// </summary>
        /// <param name="forwardRead">Forward Read.</param>
        /// <param name="reverseRead">Reverse Read.</param>
        /// <param name="library">Library used to sequence reads.</param>
        public MatePair(ISequence forwardRead, ISequence reverseRead, string library)
        {
            if (null == forwardRead)
            {
                throw new ArgumentNullException("forwardRead");
            }

            if (null == reverseRead)
            {
                throw new ArgumentNullException("reverseRead");
            }

            if (string.IsNullOrEmpty(library))
            {
                throw new ArgumentNullException("library");
            }

            if (Validate(forwardRead, reverseRead, library))
            {
                this.forwardRead = forwardRead.ID;
                this.reverseRead = reverseRead.ID;
                this.Library = library;
            }
        }

        /// <summary>
        /// Initializes a new instance of the MatePair class with specified library name,
        /// forward read and reverse read.
        /// </summary>
        /// <param name="forwardReadID">ID of forward read.</param>
        /// <param name="reverseReadID">ID of reverse read.</param>
        /// <param name="library">Library used to sequence reads.</param>
        public MatePair(string forwardReadID, string reverseReadID, string library)
        {
            if (Validate(forwardReadID, reverseReadID, library))
            {
                this.forwardRead = forwardReadID;
                this.reverseRead = reverseReadID;
                this.Library = library;
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets sequence of Forward Read.
        /// </summary>
        public string ForwardReadID
        {
            get
            {
                return this.forwardRead;
            }

            set
            {
                if (Validate(value))
                {
                    this.forwardRead = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets sequence for Reverse Read.
        /// </summary>
        public string ReverseReadID
        {
            get
            {
                return this.reverseRead;
            }

            set
            {
                if (Validate(value))
                {
                    this.reverseRead = value;
                }
            }
        }

        /// <summary>
        /// Gets and sets Name of Library.
        /// </summary>
        public string Library
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets mean length of Insert for Library.
        /// </summary>
        public float MeanLengthOfLibrary
        {
            get
            {
                return CloneLibrary.Instance.GetLibraryInformation(this.Library).MeanLengthOfInsert;
            }
        }

        /// <summary>
        /// Gets standard deviation of insert lengths for a library.
        /// </summary>
        public float StandardDeviationOfLibrary
        {
            get
            {
                return CloneLibrary.Instance.GetLibraryInformation(this.Library).StandardDeviationOfInsert;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sequence of forward read.
        /// </summary>
        /// <param name="sequences">List of input reads.</param>
        /// <returns>Sequence of forward read.</returns>
        public ISequence GetForwardRead(IEnumerable<ISequence> sequences)
        {
            return sequences.First(t => t.ID == this.forwardRead);
        }

        /// <summary>
        /// Gets the Sequence of reverse read from given list.
        /// </summary>
        /// <param name="sequences">List of input reads.</param>
        /// <returns>Sequence of reverse read.</returns>
        public ISequence GetReverseRead(IList<ISequence> sequences)
        {
            return sequences.First(t => t.ID == this.reverseRead);
        }

        /// <summary>
        /// Converts ForwardReadID, ReverseReadID, MeanLength, Standard Deviation of MatePair to string.
        /// </summary>
        /// <returns>ForwardReadID, ReverseReadID, MeanLength, Standard Deviation.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, Properties.Resource.MatePairToStringFormat,
                                 this.ForwardReadID, this.ReverseReadID, this.MeanLengthOfLibrary,
                                 this.StandardDeviationOfLibrary);
        }

        #endregion

        #region Private Method

        /// <summary>
        /// Validate library information.
        /// </summary>
        /// <param name="library">Name of library.</param>
        /// <returns>Is Input Valid.</returns>
        private static bool Validate(string library)
        {
            if (string.IsNullOrEmpty(library))
            {
                throw new ArgumentNullException("library");
            }

            return true;
        }

        /// <summary>
        /// Validates the Input.
        /// </summary>
        /// <param name="forwardReadID">ID of forward read.</param>
        /// <param name="reverseReadID">ID of reverse read.</param>
        /// <param name="library">Name of Library.</param>
        /// <returns>Are inputs valid.</returns>
        private static bool Validate(string forwardReadID, string reverseReadID, string library)
        {
            if (string.IsNullOrEmpty(forwardReadID))
            {
                throw new ArgumentNullException("forwardReadID");
            }

            if (string.IsNullOrEmpty(reverseReadID))
            {
                throw new ArgumentNullException("reverseReadID");
            }

            Validate(library);

            return true;
        }

        /// <summary>
        /// Validates the Input.
        /// </summary>
        /// <param name="forwardRead">Sequence of forward read.</param>
        /// <param name="reverseRead">Sequence of reverse read.</param>
        /// <param name="library">Name of libarary.</param>
        /// <returns>Are inputs valid.</returns>
        private static bool Validate(ISequence forwardRead, ISequence reverseRead, string library)
        {
            if (0 == forwardRead.Count)
            {
                throw new ArgumentException(Properties.Resource.ForwardReadCount);
            }

            if (0 == reverseRead.Count)
            {
                throw new ArgumentException(Properties.Resource.ReverseReadCount);
            }

            Validate(library);
            return true;
        }

        #endregion
    }
}
