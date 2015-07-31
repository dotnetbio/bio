using System;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// A DivisionCode specifies which family a sequence belongs to.
    /// </summary>
    public enum SequenceDivisionCode
    {
        /// <summary>
        /// None - Division code is unspecified.
        /// </summary>
        None,

        /// <summary>
        /// Primate sequences.
        /// </summary>
        PRI,

        /// <summary>
        /// Rodent sequences.
        /// </summary>
        ROD,

        /// <summary>
        /// Other mammalian sequences.
        /// </summary>
        MAM,

        /// <summary>
        /// Other vertebrate sequences.
        /// </summary>
        VRT,

        /// <summary>
        /// Invertebrate sequences.
        /// </summary>
        INV,

        /// <summary>
        /// Plant, fungal, and algal sequences.
        /// </summary>
        PLN,

        /// <summary>
        /// Bacterial sequences.
        /// </summary>
        BCT,

        /// <summary>
        /// Viral sequences.
        /// </summary>
        VRL,

        /// <summary>
        /// Bacteriophage sequences.
        /// </summary>
        PHG,

        /// <summary>
        /// Synthetic sequences.
        /// </summary>
        SYN,

        /// <summary>
        /// Unannotated sequences.
        /// </summary>
        UNA,

        /// <summary>
        /// EST sequences (expressed sequence tags).
        /// </summary>
        EST,

        /// <summary>
        /// Patent sequences.
        /// </summary>
        PAT,

        /// <summary>
        /// STS sequences (sequence tagged sites).
        /// </summary>
        STS,

        /// <summary>
        /// GSS sequences (genome survey sequences).
        /// </summary>
        GSS,

        /// <summary>
        /// HTGS sequences (high throughput genomic sequences).
        /// </summary>
        HTG,

        /// <summary>
        /// HTC sequences (high throughput cDNA sequences).
        /// </summary>
        HTC,

        /// <summary>
        /// Environmental sampling sequences.
        /// </summary>
        ENV,

        /// <summary>
        /// Constructed sequences.
        /// </summary>
        CON
    }
}
