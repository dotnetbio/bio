using System;
using System.Collections.Generic;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// Region of DNA at which regulation of termination of transcription occurs, 
    /// which controls the expression of some bacterial operons.
    /// Sequence segment located between the promoter and the first structural gene that 
    /// causes partial termination of transcription.
    /// </summary>
    public class Attenuator : FeatureItem
    {
        #region Constructors
        /// <summary>
        /// Creates new Attenuator feature item from the specified location.
        /// </summary>
        /// <param name="location">Location of the Attenuator.</param>
        public Attenuator(string location)
            : base(StandardFeatureKeys.Attenuator, location) { }

        /// <summary>
        /// Creates new Attenuator feature item from the specified location.
        /// </summary>
        /// <param name="location">Location of the Attenuator.</param>
        public Attenuator(ILocation location)
            : base(StandardFeatureKeys.Attenuator, location) { }

        /// <summary>
        /// Private constructor for clone method.
        /// </summary>
        /// <param name="other">Other Attenuator instance.</param>
        private Attenuator(Attenuator other)
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
        /// Name of the group of contiguous genes transcribed into a single transcript to which that feature belongs.
        /// </summary>
        public string Operon
        {
            get
            {
                return GetSingleTextQualifier(StandardQualifierNames.Operon);
            }

            set
            {
                SetSingleTextQualifier(StandardQualifierNames.Operon, value);
            }
        }

        /// <summary>
        /// Phenotype conferred by the feature, where phenotype is defined as a physical, biochemical or behavioral 
        /// characteristic or set of characteristics.
        /// </summary>
        public List<string> Phenotype
        {
            get
            {
                return GetQualifier(StandardQualifierNames.Phenotype);
            }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Creates a new Attenuator that is a copy of the current Attenuator.
        /// </summary>
        /// <returns>A new Attenuator that is a copy of this Attenuator.</returns>
        public new Attenuator Clone()
        {
            return new Attenuator(this);
        }
        #endregion Methods
    }
}
