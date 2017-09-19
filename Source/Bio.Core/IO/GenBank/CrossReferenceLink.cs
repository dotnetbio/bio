using System;
using System.Collections.Generic;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// CrossReferenceLink provides cross-references to resources that support the existence 
    /// a sequence record, such as the Project Database and the NCBI 
    /// Trace Assembly Archive.
    /// </summary>
    public class CrossReferenceLink
    {
        #region Constructors
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public CrossReferenceLink()
        {
            Numbers = new List<string>();
        }

        /// <summary>
        /// Private Constructor for clone method.
        /// </summary>
        /// <param name="other">CrossReferenceLink instance to clone.</param>
        private CrossReferenceLink(CrossReferenceLink other)
        {
            Type = other.Type;
            Numbers = new List<string>(other.Numbers);
        }

        #endregion Constructors

        #region Properties
        /// <summary>
        /// A CrossReferenceType specifies whether the DBLink is 
        /// referring to project or a Trace Assembly Archive.
        /// </summary>
        public CrossReferenceType Type { get; set; }

        /// <summary>
        /// Project numbers.
        /// </summary>
        public IList<string> Numbers { get; private set; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Creates a new CrossReferenceLink that is a copy of the current CrossReferenceLink.
        /// </summary>
        /// <returns>A new CrossReferenceLink that is a copy of this CrossReferenceLink.</returns>
        public CrossReferenceLink Clone()
        {
            return new CrossReferenceLink(this);
        }
        #endregion Methods

    }
}
