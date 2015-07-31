using System;
using System.Collections.Generic;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// UnsureSequenceRegion (Unsure) is a region in which author is unsure of exact sequence.
    /// </summary>
    public class UnsureSequenceRegion : FeatureItem
    {
        #region Constructors
        /// <summary>
        /// Creates new UnsureSequenceRegion feature item from the specified location.
        /// </summary>
        /// <param name="location">Location of the UnsureSequenceRegion.</param>
        public UnsureSequenceRegion(ILocation location)
            : base(StandardFeatureKeys.UnsureSequenceRegion, location) { }

        /// <summary>
        /// Creates new UnsureSequenceRegion feature item with the specified location.
        /// Note that this constructor uses LocationBuilder to construct location object from the specified 
        /// location string.
        /// </summary>
        /// <param name="location">Location of the UnsureSequenceRegion.</param>
        public UnsureSequenceRegion(string location)
            : base(StandardFeatureKeys.UnsureSequenceRegion, location) { }

        /// <summary>
        /// Private constructor for clone method.
        /// </summary>
        /// <param name="other">Other UnsureSequenceRegion instance.</param>
        private UnsureSequenceRegion(UnsureSequenceRegion other)
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
        /// Reference details of an existing public INSD entry to which a comparison is made.
        /// </summary>
        public List<string> Compare
        {
            get
            {
                return GetQualifier(StandardQualifierNames.Compare);
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

        /// <summary>
        /// Indicates that the sequence identified a feature's intervals is replaced by the sequence shown in ""text"";
        /// if no sequence is contained within the qualifier, this indicates a deletion.
        /// </summary>
        public string Replace
        {
            get
            {
                return GetSingleTextQualifier(StandardQualifierNames.Replace);
            }

            set
            {
                SetSingleTextQualifier(StandardQualifierNames.Replace, value);
            }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Creates a new UnsureSequenceRegion that is a copy of the current UnsureSequenceRegion.
        /// </summary>
        /// <returns>A new UnsureSequenceRegion that is a copy of this UnsureSequenceRegion.</returns>
        public new UnsureSequenceRegion Clone()
        {
            return new UnsureSequenceRegion(this);
        }
        #endregion Methods
    }
}
