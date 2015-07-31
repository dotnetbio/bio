using System.Collections.Generic;

namespace Bio.Web.Blast
{
    /// <summary>
    /// A database sequence with high similarity to the query sequence.
    /// </summary>
    public class Hit
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Hit()
        {
            this.Hsps = new List<Hsp>();
        }

        /// <summary>
        /// The string identifying the hit sequence
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The "defline" or definition line for the hit
        /// </summary>
        public string Def { get; set; }

        /// <summary>
        /// The accession number of the hit, as string
        /// </summary>
        public string Accession { get; set; }

        /// <summary>
        /// The length of the hit sequence
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// The list of HSPs returned for this Hit.
        /// </summary>
        public IList<Hsp> Hsps { get; private set; }
    }
}
