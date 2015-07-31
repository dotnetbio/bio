using System;
using System.Collections.Generic;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// Operon is a region containing polycistronic transcript containing genes that encode enzymes 
    /// that are in the same metabolic pathway and regulatory sequences.
    /// </summary>
    public class OperonRegion : FeatureItem
    {
        #region Constructors
        /// <summary>
        /// Creates new Operon feature item from the specified location.
        /// </summary>
        /// <param name="location">Location of the Operon.</param>
        public OperonRegion(ILocation location)
            : base(StandardFeatureKeys.OperonRegion, location) { }

        /// <summary>
        /// Creates new OperonRegion feature item with the specified location.
        /// Note that this constructor uses LocationBuilder to construct location object from the specified 
        /// location string.
        /// </summary>
        /// <param name="location">Location of the OperonRegion.</param>
        public OperonRegion(string location)
            : base(StandardFeatureKeys.OperonRegion, location) { }


        /// <summary>
        /// Private constructor for clone method.
        /// </summary>
        /// <param name="other">Other Operon instance.</param>
        private OperonRegion(OperonRegion other)
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
        /// Creates a new OperonRegion that is a copy of the current OperonRegion.
        /// </summary>
        /// <returns>A new OperonRegion that is a copy of this OperonRegion.</returns>
        public new OperonRegion Clone()
        {
            return new OperonRegion(this);
        }
        #endregion Methods
    }
}
