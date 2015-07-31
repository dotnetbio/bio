using System;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// Locus provides a short mnemonic name for the sequence entry in gen bank 
    /// database, chosen to suggest the sequence's definition.
    /// 
    /// It also contains information like Sequence type, Strand type division code etc.
    /// </summary>
    public class GenBankLocusInfo
    {
        #region Properties
        /// <summary>
        /// Mnemonic name for the sequence entry.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Specifies the length of the sequence present.
        /// </summary>
        public int SequenceLength { get; set; }

        /// <summary>
        /// Sequence type specifies whether the sequence is a base pair (bp) or an amino acid (aa).
        /// </summary>
        public string SequenceType { get; set; }

        /// <summary>
        /// A StrandType specifies whether sequence occurs as a single stranded,
        /// double stranded or mixed stranded. 
        /// </summary>
        public SequenceStrandType Strand { get; set; }

        /// <summary>
        /// Specifies type of the biological sequence.
        /// </summary>
       // public MoleculeType MoleculeType { get; set; }
        public MoleculeType MoleculeType { get; set; }

        /// <summary>
        /// A StrandTopology specifies whether the strand is linear or circular.
        /// </summary>
        public SequenceStrandTopology StrandTopology { get; set; }

        /// <summary>
        /// A DivisionCode specifies which family a sequence belongs to.
        /// </summary>
        public SequenceDivisionCode DivisionCode { get; set; }

        /// <summary>
        /// Contains the date the entry was entered or underwent any substantial revisions,
        /// such as the addition of newly published data.
        /// </summary>
        public DateTime Date { get; set; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Creates a new GenBankLocusInfo that is a copy of the current GenBankLocusInfo.
        /// </summary>
        /// <returns>A new GenBankLocusInfo that is a copy of this GenBankLocusInfo.</returns>
        public GenBankLocusInfo Clone()
        {
            return (GenBankLocusInfo)MemberwiseClone();
        }
        #endregion Methods
    }
}
