using System;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// A non-protein-coding gene (ncRNA), other than ribosomal RNA and transfer RNA, the functional 
    /// molecule of which is the RNA transcript.
    /// </summary>
    public class NonCodingRna : FeatureItem
    {
        #region Constructors
        /// <summary>
        /// Creates new NonCodingRNA feature item from the specified location.
        /// </summary>
        /// <param name="location">Location of the NonCodingRNA.</param>
        public NonCodingRna(ILocation location)
            : base(StandardFeatureKeys.NonCodingRna, location) { }

        /// <summary>
        /// Creates new NonCodingRNA feature item with the specified location.
        /// Note that this constructor uses LocationBuilder to construct location object from the specified 
        /// location string.
        /// </summary>
        /// <param name="location">Location of the NonCodingRNA.</param>
        public NonCodingRna(string location)
            : base(StandardFeatureKeys.NonCodingRna, location) { }

        /// <summary>
        /// Private constructor for clone method.
        /// </summary>
        /// <param name="other">Other NonCodingRNA instance.</param>
        private NonCodingRna(NonCodingRna other)
            : base(other) { }
        #endregion Constructors

        #region Properties

        /// <summary>
        /// ncRNA_class; A structured description of the classification of the non-coding RNA described by the ncRNA parent key.
        /// </summary>
        public string NonCodingRnaClass
        {
            get
            {
                return GetSingleTextQualifier(StandardQualifierNames.NonCodingRnaClass);
            }

            set
            {
                SetSingleTextQualifier(StandardQualifierNames.NonCodingRnaClass, value);
            }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Creates a new NonCodingRNA that is a copy of the current NonCodingRNA.
        /// </summary>
        /// <returns>A new NonCodingRNA that is a copy of this NonCodingRNA.</returns>
        public new NonCodingRna Clone()
        {
            return new NonCodingRna(this);
        }
        #endregion Methods
    }
}
