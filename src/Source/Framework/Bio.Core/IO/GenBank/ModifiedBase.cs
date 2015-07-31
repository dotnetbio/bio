using System;
using System.Collections.Generic;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// The indicated nucleotide is a modified nucleotide and should be substituted for by the 
    /// indicated molecule (given in the ModifiedNucleotideBase qualifier value).
    /// </summary>
    public class ModifiedBase : FeatureItem
    {
        #region Constructors
        /// <summary>
        /// Creates new ModifiedBase feature item from the specified location.
        /// </summary>
        /// <param name="location">Location of the ModifiedBase.</param>
        public ModifiedBase(ILocation location)
            : base(StandardFeatureKeys.ModifiedBase, location) { }

        /// <summary>
        /// Creates new ModifiedBase feature item with the specified location.
        /// Note that this constructor uses LocationBuilder to construct location object from the specified 
        /// location string.
        /// </summary>
        /// <param name="location">Location of the ModifiedBase.</param>
        public ModifiedBase(string location)
            : base(StandardFeatureKeys.ModifiedBase, location) { }

        /// <summary>
        /// Private constructor for clone method.
        /// </summary>
        /// <param name="other">Other ModifiedBase instance.</param>
        private ModifiedBase(ModifiedBase other)
            : base(other) { }
        #endregion Constructors

        #region Properties

        /// <summary>
        /// Name of the allele for the given gene.
        /// All gene-related features (exon, CDS etc) for a given gene should share the same Allele qualifier value; 
        /// the Allele qualifier value must, by definition, be different from the Gene qualifier value; when used with 
        /// the variation feature key, the Allele qualifier value should be that of the variant.
        /// </summary>
        public string Allele
        {
            get
            {
                return GetSingleTextQualifier(StandardQualifierNames.Allele);
            }

            set
            {
                SetSingleTextQualifier(StandardQualifierNames.Allele, value);
            }
        }

        /// <summary>
        /// Reference to a citation listed in the entry reference field.
        /// </summary>
        public List<string> Citation
        {
            get
            {
                return GetQualifier(StandardQualifierNames.Citation);
            }
        }

        /// <summary>
        /// Database cross-reference: pointer to related information in another database.
        /// </summary>
        public List<string> DatabaseCrossReference
        {
            get
            {
                return GetQualifier(StandardQualifierNames.DatabaseCrossReference);
            }
        }

        /// <summary>
        /// A brief description of the nature of the experimental evidence that supports the feature 
        /// identification or assignment.
        /// </summary>
        public List<string> Experiment
        {
            get
            {
                return GetQualifier(StandardQualifierNames.Experiment);
            }
        }

        /// <summary>
        /// Frequency of the occurrence of a feature.
        /// </summary>
        public string Frequency
        {
            get
            {
                return GetSingleTextQualifier(StandardQualifierNames.Frequency);
            }

            set
            {
                SetSingleTextQualifier(StandardQualifierNames.Frequency, value);
            }
        }

        /// <summary>
        /// Symbol of the gene corresponding to a sequence region.
        /// </summary>
        public string GeneSymbol
        {
            get
            {
                return GetSingleTextQualifier(StandardQualifierNames.GeneSymbol);
            }

            set
            {
                SetSingleTextQualifier(StandardQualifierNames.GeneSymbol, value);
            }
        }

        /// <summary>
        /// Synonymous, replaced, obsolete or former gene symbol.
        /// </summary>
        public List<string> GeneSynonym
        {
            get
            {
                return GetQualifier(StandardQualifierNames.GeneSynonym);
            }
        }

        /// <summary>
        /// Genomic map position of feature.
        /// </summary>
        public string GenomicMapPosition
        {
            get
            {
                return GetSingleTextQualifier(StandardQualifierNames.GenomicMapPosition);
            }

            set
            {
                SetSingleTextQualifier(StandardQualifierNames.GenomicMapPosition, value);
            }
        }

        /// <summary>
        /// A structured description of non-experimental evidence that supports the feature 
        /// identification or assignment.
        /// </summary>
        public List<string> Inference
        {
            get
            {
                return GetQualifier(StandardQualifierNames.Inference);
            }
        }

        /// <summary>
        /// A submitter-supplied, systematic, stable identifier for a gene and its associated 
        /// features, used for tracking purposes.
        /// </summary>
        public List<string> LocusTag
        {
            get
            {
                return GetQualifier(StandardQualifierNames.LocusTag);
            }
        }

        /// <summary>
        /// mod_base; Abbreviation for a modified nucleotide base.
        /// </summary>
        public List<string> ModifiedNucleotideBase
        {
            get
            {
                return GetQualifier(StandardQualifierNames.ModifiedNucleotideBase);
            }
        }

        /// <summary>
        /// Any comment or additional information.
        /// </summary>
        public List<string> Note
        {
            get
            {
                return GetQualifier(StandardQualifierNames.Note);
            }
        }

        /// <summary>
        /// Feature tag assigned for tracking purposes.
        /// </summary>
        public List<string> OldLocusTag
        {
            get
            {
                return GetQualifier(StandardQualifierNames.OldLocusTag);
            }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Creates a new ModifiedBase that is a copy of the current ModifiedBase.
        /// </summary>
        /// <returns>A new ModifiedBase that is a copy of this ModifiedBase.</returns>
        public new ModifiedBase Clone()
        {
            return new ModifiedBase(this);
        }
        #endregion Methods
    }
}
