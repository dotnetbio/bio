using System;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// A compound identifier consisting of the primary accession number and 
    /// a numeric version number associated with the current version of the 
    /// sequence data in the record. This is followed by an integer key 
    /// (a "GI") assigned to the sequence by NCBI.
    /// </summary>
    public class GenBankVersion
    {
        #region Properties
        /// <summary>
        /// Primary accession number.
        /// </summary>
        public string Accession { get; set; }

        /// <summary>
        /// Version number.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets the CompoundAccession that is Accession.Version.
        /// </summary>
        public string CompoundAccession
        {
            get
            {
                return Accession + "." + Version;
            }
        }

        /// <summary>
        /// GI number.
        /// </summary>
        public string GiNumber { get; set; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Creates a new GenBankVersion that is a copy of the current GenBankVersion.
        /// </summary>
        /// <returns>A new GenBankVersion that is a copy of this GenBankVersion.</returns>
        public GenBankVersion Clone()
        {
            return (GenBankVersion)MemberwiseClone();
        }
        #endregion Methods
    }
}
