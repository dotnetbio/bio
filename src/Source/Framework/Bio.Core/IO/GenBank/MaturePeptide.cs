using System;
using System.Collections.Generic;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// Mature peptide or protein coding sequence; coding sequence for the mature or final peptide or protein product following 
    /// post-translational modification; the location does not include the stop codon (unlike the corresponding CDS).
    /// </summary>
    public class MaturePeptide : FeatureItem
    {
        #region Constructors
        /// <summary>
        /// Creates new MaturePeptide feature item from the specified location.
        /// </summary>
        /// <param name="location">Location of the MaturePeptide.</param>
        public MaturePeptide(ILocation location)
            : base(StandardFeatureKeys.MaturePeptide, location) { }

        /// <summary>
        /// Creates new MaturePeptide feature item with the specified location.
        /// Note that this constructor uses LocationBuilder to construct location object from the specified 
        /// location string.
        /// </summary>
        /// <param name="location">Location of the MaturePeptide.</param>
        public MaturePeptide(string location)
            : base(StandardFeatureKeys.MaturePeptide, location) { }

        /// <summary>
        /// Private constructor for clone method.
        /// </summary>
        /// <param name="other">Other MaturePeptide instance.</param>
        private MaturePeptide(MaturePeptide other)
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
        /// Enzyme Commission number for enzyme product of sequence.
        /// </summary>
        public string EnzymeCommissionNumber
        {
            get
            {
                return GetSingleTextQualifier(StandardQualifierNames.EnzymeCommissionNumber);
            }

            set
            {
                SetSingleTextQualifier(StandardQualifierNames.EnzymeCommissionNumber, value);
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
        /// Function attributed to a sequence.
        /// </summary>
        public List<string> Function
        {
            get
            {
                return GetQualifier(StandardQualifierNames.Function);
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
        /// Name of the product associated with the feature, e.g. the mRNA of an mRNA feature, 
        /// the polypeptide of a CDS, the mature peptide of a mat_peptide, etc.
        /// </summary>
        public List<string> Product
        {
            get
            {
                return GetQualifier(StandardQualifierNames.Product);
            }
        }

        /// <summary>
        /// Indicates that this feature is a non-functional version of the element named by the feature key.
        /// </summary>
        public bool Pseudo
        {
            get
            {
                return GetSingleBooleanQualifier(StandardQualifierNames.Pseudo);
            }

            set
            {
                SetSingleBooleanQualifier(StandardQualifierNames.Pseudo, value);
            }
        }

        /// <summary>
        /// Accepted standard name for this feature.
        /// </summary>
        public string StandardName
        {
            get
            {
                return GetSingleTextQualifier(StandardQualifierNames.StandardName);
            }

            set
            {
                SetSingleTextQualifier(StandardQualifierNames.StandardName, value);
            }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Creates a new MaturePeptide that is a copy of the current MaturePeptide.
        /// </summary>
        /// <returns>A new MaturePeptide that is a copy of this MaturePeptide.</returns>
        public new MaturePeptide Clone()
        {
            return new MaturePeptide(this);
        }
        #endregion Methods
    }
}
