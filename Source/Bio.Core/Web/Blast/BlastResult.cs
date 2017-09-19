using System.Collections.Generic;

namespace Bio.Web.Blast
{
    /// <summary>
    /// A single BLAST search result. This is represented by a single XML
    /// document in BLAST XML format. It consist of some introductory information
    /// such as BLAST version, a structure listing the search parameters, and
    /// a list of Iterations (represented in the BlastSearchRecord class).
    /// </summary>
    public class BlastResult
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public BlastResult()
        {
            this.Records = new List<BlastSearchRecord>();
        }

        /// <summary>
        /// The summary data for the search
        /// </summary>
        public BlastXmlMetadata Metadata { get; set; }

        /// <summary>
        /// The list of BlastSearchRecords in the document.
        /// </summary>
        public IList<BlastSearchRecord> Records { get; private set; }
    }
}
