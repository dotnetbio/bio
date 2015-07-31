using System;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// A MoleculeType specifies which type of biological sequence is stored in an ISequence.
    /// </summary>
    public enum MoleculeType
    {
        /// <summary>
        /// Not a valid molecule type.
        /// </summary>
        Invalid,

        /// <summary>
        /// Nucleic acid of an unspecified type.
        /// </summary>
        NA,

        /// <summary>
        /// Deoxyrobonucleic acid.
        /// </summary>
        DNA,

        /// <summary>
        /// Ribonucleic acid of an unspecified type.
        /// </summary>
        RNA,

        /// <summary>
        /// Transfer RNA.
        /// </summary>
        tRNA,

        /// <summary>
        /// Ribosomal RNA.
        /// </summary>
        rRNA,

        /// <summary>
        /// Messenger RNA.
        /// </summary>
        mRNA,

        /// <summary>
        /// Small nuclear RNA.
        /// </summary>
        uRNA,

        /// <summary>
        /// Small nuclear RNA.
        /// </summary>
        snRNA,

        /// <summary>
        /// Small nucleolar RNA (often referred to as guide RNA).
        /// </summary>
        snoRNA,

        /// <summary>
        /// Amino acid chain.
        /// </summary>
        Protein
    }
}
