using System;

namespace Bio.IO.GenBank
{
    /// <summary>
    ///  Citations for all articles containing data reported in this sequence.
    ///  
    /// Citations in PubMed that do not fall within Medline's scope will have only
    /// a PUBMED identifier. Similarly, citations that *are* in Medline's scope but
    /// which have not yet been assigned Medline UIs will have only a PUBMED identifier.
    /// If a citation is present in both the PubMed and Medline databases, both a
    /// MEDLINE and a PUBMED line will be present.
    /// </summary>
    public class CitationReference
    {
        #region Properties
        /// <summary>
        /// Reference number.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// The range of bases in the sequence entry reported in this citation.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Lists the authors in the order in which they appear in the cited article
        /// Last names are separated from initials by a comma (no space); there is no comma 
        /// before the final `and'. The list of authors ends with a period. 
        /// </summary>
        public string Authors { get; set; }

        /// <summary>
        /// Lists the collective names of consortiums associated with the citation 
        /// (eg, International Human Genome Sequencing Consortium), rather than individual author names. 
        /// </summary>
        public string Consortiums { get; set; }

        /// <summary>
        /// Full title of citation. 
        /// Present in all but unpublished citations.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Lists the journal name, volume, year, and page numbers of the citation
        /// </summary>
        public string Journal { get; set; }

        /// <summary>
        /// The National Library of Medicine's Medline unique identifier for a citation (if known).
        /// Medline UIs are 8 digit numbers.
        /// </summary>
        public string Medline { get; set; }

        /// <summary>
        /// The PubMed unique identifier for a citation (if known). 
        /// PUBMED ids are numeric, and are record identifiers for article abstracts in the PubMed database.
        /// 
        /// http://www.ncbi.nlm.nih.gov/entrez/query.fcgi?db=PubMed
        /// </summary>
        public string PubMed { get; set; }

        /// <summary>
        /// The REMARK line is a textual comment that specifies the relevance
        /// of the citation to the entry.
        /// </summary>
        public string Remarks { get; set; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Creates a new CitationReference that is a copy of the current CitationReference.
        /// </summary>
        /// <returns>A new CitationReference that is a copy of this CitationReference.</returns>
        public CitationReference Clone()
        {
            return (CitationReference)MemberwiseClone();
        }
        #endregion Methods

    }
}
