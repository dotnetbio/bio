using System;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// Provides Genus, Species and taxonomic classification levels of the sequence.
    /// </summary>
    public class OrganismInfo
    {
        #region Properties
        /// <summary>
        /// Genus name of the Organism.
        /// </summary>
        public string Genus { get; set; }

        /// <summary>
        /// Species of the organism.
        /// </summary>
        public string Species { get; set; }

        /// <summary>
        /// Taxonomic classification levels of the Organism.
        /// </summary>
        public string ClassLevels { get; set; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Creates a new OrganismInfo that is a copy of the current OrganismInfo.
        /// </summary>
        /// <returns>A new OrganismInfo that is a copy of this OrganismInfo.</returns>
        public OrganismInfo Clone()
        {
            return (OrganismInfo)MemberwiseClone();
        }
        #endregion Methods

    }
}
