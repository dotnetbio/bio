using System.Collections.Generic;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// Static class to hold standard feature keys.
    /// </summary>
    public static class StandardFeatureKeys
    {
        #region Feature Keys
        /// <summary>
        /// Holds key for Minus10Signal (-10_signal) feature.
        /// </summary>
        public const string Minus10Signal = "-10_signal";

        /// <summary>
        /// Holds key for Minus35Signal (-35_signal) feature.
        /// </summary>
        public const string Minus35Signal = "-35_signal";

        /// <summary>
        /// Holds key for ThreePrimeUtr (3'UTR) feature.
        /// </summary>
        public const string ThreePrimeUtr = "3'UTR";

        /// <summary>
        /// Holds key for FivePrimeUtr (5'UTR) feature.
        /// </summary>
        public const string FivePrimeUtr = "5'UTR";

        /// <summary>
        /// Holds key for Attenuator feature.
        /// </summary>
        public const string Attenuator = "attenuator";

        /// <summary>
        /// Holds key for CAATSignal (CAAT_signal) feature.
        /// </summary>
        public const string CaaTSignal = "CAAT_signal";

        /// <summary>
        /// Holds key for CodingSequence (CDS) feature.
        /// </summary>
        public const string CodingSequence = "CDS";

        /// <summary>
        /// Holds key for DisplacementLoop (D-loop) feature.
        /// </summary>
        public const string DisplacementLoop = "D-loop";

        /// <summary>
        /// Holds key for Enhancer feature.
        /// </summary>
        public const string Enhancer = "enhancer";

        /// <summary>
        /// Holds key for Exon feature.
        /// </summary>
        public const string Exon = "exon";

        /// <summary>
        /// Holds key for GCSingal (GC_signal) feature.
        /// </summary>
        public const string GcSingal = "GC_signal";

        /// <summary>
        /// Holds key for Gene feature.
        /// </summary>
        public const string Gene = "gene";

        /// <summary>
        /// Holds key for InterveningDNA (iDNA) feature.
        /// </summary>
        public const string InterveningDna = "iDNA";

        /// <summary>
        /// Holds key for Intron feature.
        /// </summary>
        public const string Intron = "intron";

        /// <summary>
        /// Holds key for LongTerminalRepeat (LTR) feature.
        /// </summary>
        public const string LongTerminalRepeat = "LTR";

        /// <summary>
        /// Holds key for MaturePeptide (mat_peptide) feature.
        /// </summary>
        public const string MaturePeptide = "mat_peptide";

        /// <summary>
        /// Holds key for MiscBinding (misc_binding) feature.
        /// </summary>
        public const string MiscBinding = "misc_binding";

        /// <summary>
        /// Holds key for MiscDifference (misc_difference) feature.
        /// </summary>
        public const string MiscDifference = "misc_difference";

        /// <summary>
        /// Holds key for MiscFeature (misc_feature) feature.
        /// </summary>
        public const string MiscFeature = "misc_feature";

        /// <summary>
        /// Holds key for MiscRecombination (misc_recomb) feature.
        /// </summary>
        public const string MiscRecombination = "misc_recomb";

        /// <summary>
        /// Holds key for MiscRNA (misc_RNA) feature.
        /// </summary>
        public const string MiscRna = "misc_RNA";

        /// <summary>
        /// Holds key for MiscSignal (misc_signal) feature.
        /// </summary>
        public const string MiscSignal = "misc_signal";

        /// <summary>
        /// Holds key for MiscStructure (misc_structure) feature.
        /// </summary>
        public const string MiscStructure = "misc_structure";

        /// <summary>
        /// Holds key for ModifiedBase (modified_base) feature.
        /// </summary>
        public const string ModifiedBase = "modified_base";

        /// <summary>
        /// Holds key for MessengerRNA (mRNA) feature.
        /// </summary>
        public const string MessengerRna = "mRNA";

        /// <summary>
        /// Holds key for NonCodingRNA (ncRNA) feature.
        /// </summary>
        public const string NonCodingRna = "ncRNA";

        /// <summary>
        /// Holds key for Operon feature.
        /// </summary>
        public const string OperonRegion = "operon";

        /// <summary>
        /// Holds key for PolyASignal (polyA_signal) feature.
        /// </summary>
        public const string PolyASignal = "polyA_signal";

        /// <summary>
        /// Holds key for PolyASite (polyA_site) feature.
        /// </summary>
        public const string PolyASite = "polyA_site";

        /// <summary>
        /// Holds key for PrecursorRNA (precursor_RNA) feature.
        /// </summary>
        public const string PrecursorRna = "precursor_RNA";

        /// <summary>
        /// Holds key for Promoter feature.
        /// </summary>
        public const string Promoter = "promoter";

        /// <summary>
        /// Holds key for ProteinBindingSite (protein_bind) feature.
        /// </summary>
        public const string ProteinBindingSite = "protein_bind";

        /// <summary>
        /// Holds key for RibosomeBindingSite (RBS) feature.
        /// </summary>
        public const string RibosomeBindingSite = "RBS";

        /// <summary>
        /// Holds key for ReplicationOrigin (rep_origin) feature.
        /// </summary>
        public const string ReplicationOrigin = "rep_origin";

        /// <summary>
        /// Holds key for RepeatRegion (repeat_region) feature.
        /// </summary>
        public const string RepeatRegion = "repeat_region";

        /// <summary>
        /// Holds key for RibosomalRNA (rRNA) feature.
        /// </summary>
        public const string RibosomalRna = "rRNA";

        /// <summary>
        /// Holds key for SignalPeptide (sig_peptide) feature.
        /// </summary>
        public const string SignalPeptide = "sig_peptide";

        /// <summary>
        /// Holds key for StemLoop (stem_loop) feature.
        /// </summary>
        public const string StemLoop = "stem_loop";

        /// <summary>
        /// Holds key for TATASignal (TATA_signal) feature.
        /// </summary>
        public const string TataSignal = "TATA_signal";

        /// <summary>
        /// Holds key for Terminator feature.
        /// </summary>
        public const string Terminator = "terminator";

        /// <summary>
        /// Holds key for TransferMessengerRNA (tmRNA) feature.
        /// </summary>
        public const string TransferMessengerRna = "tmRNA";

        /// <summary>
        /// Holds key for TransitPeptide (transit_peptide) feature.
        /// </summary>
        public const string TransitPeptide = "transit_peptide";

        /// <summary>
        /// Holds key for TransferRNA (tRNA) feature.
        /// </summary>
        public const string TransferRna = "tRNA";

        /// <summary>
        /// Holds key for UnsureSequenceRegion (unsure) feature.
        /// </summary>
        public const string UnsureSequenceRegion = "unsure";

        /// <summary>
        /// Holds key for Variation feature.
        /// </summary>
        public const string Variation = "variation";

        #endregion Feature Keys

        private static List<string> all;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static StandardFeatureKeys()
        {
            all = new List<string>();

            #region Add feature keys to list
            all.Add(Minus10Signal);
            all.Add(Minus35Signal);
            all.Add(ThreePrimeUtr);
            all.Add(FivePrimeUtr);
            all.Add(Attenuator);
            all.Add(CaaTSignal);
            all.Add(CodingSequence);
            all.Add(DisplacementLoop);
            all.Add(Enhancer);
            all.Add(Exon);
            all.Add(GcSingal);
            all.Add(Gene);
            all.Add(InterveningDna);
            all.Add(Intron);
            all.Add(LongTerminalRepeat);
            all.Add(MaturePeptide);
            all.Add(MiscBinding);
            all.Add(MiscDifference);
            all.Add(MiscFeature);
            all.Add(MiscRecombination);
            all.Add(MiscRna);
            all.Add(MiscSignal);
            all.Add(MiscStructure);
            all.Add(ModifiedBase);
            all.Add(MessengerRna);
            all.Add(NonCodingRna);
            all.Add(OperonRegion);
            all.Add(PolyASignal);
            all.Add(PolyASite);
            all.Add(PrecursorRna);
            all.Add(Promoter);
            all.Add(ProteinBindingSite);
            all.Add(RibosomeBindingSite);
            all.Add(ReplicationOrigin);
            all.Add(RepeatRegion);
            all.Add(RibosomalRna);
            all.Add(SignalPeptide);
            all.Add(StemLoop);
            all.Add(TataSignal);
            all.Add(Terminator);
            all.Add(TransferMessengerRna);
            all.Add(TransitPeptide);
            all.Add(TransferRna);
            all.Add(UnsureSequenceRegion);
            all.Add(Variation);
            #endregion Add feature keys to list
        }

        /// <summary>
        /// Returns a list which contains standard feature keys.
        /// </summary>
        public static IList<string> All
        {
            get
            {
                return all.AsReadOnly();
            }
        }
    }
}
