using System;
using System.Collections.Generic;

namespace Bio.IO.GenBank
{
    /// <summary>
    ///  Region of genome containing repeating units.
    /// </summary>
    public class RepeatRegion : FeatureItem
    {
        #region Constructors
        /// <summary>
        /// Creates new RepeatRegion feature item from the specified location.
        /// </summary>
        /// <param name="location">Location of the RepeatRegion.</param>
        public RepeatRegion(ILocation location)
            : base(StandardFeatureKeys.RepeatRegion, location) { }

        /// <summary>
        /// Creates new RepeatRegion feature item with the specified location.
        /// Note that this constructor uses LocationBuilder to construct location object from the specified 
        /// location string.
        /// </summary>
        /// <param name="location">Location of the RepeatRegion.</param>
        public RepeatRegion(string location)
            : base(StandardFeatureKeys.RepeatRegion, location) { }

        /// <summary>
        /// Private constructor for clone method.
        /// </summary>
        /// <param name="other">Other RepeatRegion instance.</param>
        private RepeatRegion(RepeatRegion other)
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
        /// Type and name or identifier of the mobile element which is described by the parent feature.
        /// </summary>
        public List<string> MobileElement
        {
            get
            {
                return GetQualifier(StandardQualifierNames.MobileElement);
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
        /// rpt_unit_range; Identity of a repeat range.
        /// </summary>
        public List<string> RepeatedRange
        {
            get
            {
                return GetQualifier(StandardQualifierNames.RepeatedRange);
            }
        }

        /// <summary>
        /// rpt_unit_seq; Identity of a repeat sequence.
        /// </summary>
        public List<string> RepeatedSequence
        {
            get
            {
                return GetQualifier(StandardQualifierNames.RepeatedSequence);
            }
        }

        /// <summary>
        /// rpt_family; Type of repeated sequence; ""Alu"" or ""Kpn"", for example.
        /// </summary>
        public List<string> RepeatedSequenceFamily
        {
            get
            {
                return GetQualifier(StandardQualifierNames.RepeatedSequenceFamily);
            }
        }

        /// <summary>
        /// rpt_type; Organization of repeated sequence.
        /// </summary>
        public List<string> RepeatedSequenceType
        {
            get
            {
                return GetQualifier(StandardQualifierNames.RepeatedSequenceType);
            }
        }

        /// <summary>
        /// Identifier for a satellite DNA marker, compose of many tandem repeats 
        /// (identical or related) of a short basic repeated unit.
        /// </summary>
        public string Satellite
        {
            get
            {
                return GetSingleTextQualifier(StandardQualifierNames.Satellite);
            }

            set
            {
                SetSingleTextQualifier(StandardQualifierNames.Satellite, value);
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
        /// Creates a new RepeatRegion that is a copy of the current RepeatRegion.
        /// </summary>
        /// <returns>A new RepeatRegion that is a copy of this RepeatRegion.</returns>
        public new RepeatRegion Clone()
        {
            return new RepeatRegion(this);
        }
        #endregion Methods
    }
}
