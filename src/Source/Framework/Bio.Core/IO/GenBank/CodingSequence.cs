using System;
using System.Collections.Generic;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// Coding sequence (CDS); sequence of nucleotides that corresponds with the sequence of amino acids 
    /// in a protein (location includes stop codon); feature includes amino acid conceptual translation.
    /// </summary>
    public class CodingSequence : FeatureItem
    {
        #region Constructors
        /// <summary>
        /// Creates new CodingSequence feature item from the specified location.
        /// </summary>
        /// <param name="location">Location of the CodingSequence.</param>
        public CodingSequence(ILocation location)
            : base(StandardFeatureKeys.CodingSequence, location) { }

        /// <summary>
        /// Creates new CodingSequence feature item with the specified location.
        /// Note that this constructor uses LocationBuilder to construct location object from the specified 
        /// location string.
        /// </summary>
        /// <param name="location">Location of the CodingSequence.</param>
        public CodingSequence(string location)
            : base(StandardFeatureKeys.CodingSequence, location) { }

        /// <summary>
        /// Private constructor for clone method.
        /// </summary>
        /// <param name="other">Other CodingSequence instance.</param>
        private CodingSequence(CodingSequence other)
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
        /// Specifies a codon which is different from any found in the reference genetic code.
        /// </summary>
        public List<string> Codon
        {
            get
            {
                return GetQualifier(StandardQualifierNames.Codon);
            }
        }

        /// <summary>
        /// Indicates the offset at which the first complete codon of a coding feature can be found, 
        /// relative to the first base of that feature.
        /// </summary>
        public List<string> CodonStart
        {
            get
            {
                return GetQualifier(StandardQualifierNames.CodonStart);
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
        /// Indicates that the coding region cannot be translated using standard biological rules.
        /// </summary>
        public string Exception
        {
            get
            {
                return GetSingleTextQualifier(StandardQualifierNames.Exception);
            }

            set
            {
                SetSingleTextQualifier(StandardQualifierNames.Exception, value);
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
        /// A number to indicate the order of genetic elements (e.g., exons or introns) in the 5' to 3' direction.
        /// </summary>
        public string Number
        {
            get
            {
                return GetSingleTextQualifier(StandardQualifierNames.Number);
            }

            set
            {
                SetSingleTextQualifier(StandardQualifierNames.Number, value);
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
        /// Protein identifier, issued by International collaborators. this qualifier consists of a stable ID 
        /// portion (3+5 format with 3 position letters and 5 numbers) plus a version number after the decimal point.
        /// </summary>
        public List<string> ProteinId
        {
            get
            {
                return GetQualifier(StandardQualifierNames.ProteinId);
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
        /// During protein translation, certain sequences can program ribosomes to change to an alternative 
        /// reading frame by a mechanism known as ribosomal slippage.
        /// </summary>
        public bool RibosomalSlippage
        {
            get
            {
                return GetSingleBooleanQualifier(StandardQualifierNames.RibosomalSlippage);
            }

            set
            {
                SetSingleBooleanQualifier(StandardQualifierNames.RibosomalSlippage, value);
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

        /// <summary>
        /// Automatically generated one-letter abbreviated amino acid sequence derived from either 
        /// the universal genetic code or the table as specified in Transl_table qualifier and as 
        /// determined by exceptions in the Transl_except and Codon qualifiers.
        /// </summary>
        public string Translation
        {
            get
            {
                return GetSingleTextQualifier(StandardQualifierNames.Translation);
            }

            set
            {
                SetSingleTextQualifier(StandardQualifierNames.Translation, value);
            }
        }

        /// <summary>
        /// Translational exception: single codon the translation of which does not conform 
        /// to genetic code defined by Organism and Codon qualifier.
        /// </summary>
        public List<string> TranslationalExcept
        {
            get
            {
                return GetQualifier(StandardQualifierNames.TranslationalExcept);
            }
        }

        /// <summary>
        /// Definition of genetic code table used if other than universal genetic code table.
        /// </summary>
        public string TranslationTable
        {
            get
            {
                return GetSingleTextQualifier(StandardQualifierNames.TranslationTable);
            }

            set
            {
                SetSingleTextQualifier(StandardQualifierNames.TranslationTable, value);
            }
        }

        /// <summary>
        /// Indicates that exons from two RNA molecules are ligated in intermolecular 
        /// reaction to form mature RNA.
        /// </summary>
        public bool TransSplicing
        {
            get
            {
                return GetSingleBooleanQualifier(StandardQualifierNames.TransSplicing);
            }

            set
            {
                SetSingleBooleanQualifier(StandardQualifierNames.TransSplicing, value);
            }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Returns protein sequence from the translation.
        /// </summary>
        public Sequence GetTranslation()
        {
            return new Sequence(Alphabets.Protein, Translation.Trim('"'));
        }

        /// <summary>
        /// Creates a new CodingSequence that is a copy of the current CodingSequence.
        /// </summary>
        /// <returns>A new CodingSequence that is a copy of this CodingSequence.</returns>
        public new CodingSequence Clone()
        {
            return new CodingSequence(this);
        }
        #endregion Methods
    }
}
