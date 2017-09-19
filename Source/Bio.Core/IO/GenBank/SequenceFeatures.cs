using System;
using System.Collections.Generic;
using System.Linq;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// Contains information about genes and gene products,
    /// as well as regions of biological significance reported 
    /// in the sequence.
    /// </summary>
    public class SequenceFeatures
    {
        #region Constructors
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public SequenceFeatures()
        {
            All = new List<FeatureItem>();
        }

        /// <summary>
        /// Private Constructor for clone method.
        /// </summary>
        /// <param name="other">SequenceFeatures instance to clone.</param>
        private SequenceFeatures(SequenceFeatures other)
        {
            All = new List<FeatureItem>();
            foreach (FeatureItem feature in other.All)
            {
                All.Add(feature.Clone());
            }
        }

        #endregion Constructors

        #region Properties
        /// <summary>
        /// Lists all features.
        /// </summary>
        public List<FeatureItem> All { get; private set; }

        /// <summary>
        /// Returns list of Minus10Signal (-10_signal) features.
        /// </summary>
        public List<Minus10Signal> Minus10Signals
        {
            get
            {
                return All.Where(F => F is Minus10Signal).Select(F => F as Minus10Signal).ToList();
            }
        }

        /// <summary>
        /// Returns list of Minus35Signal (-35_signal) features.
        /// </summary>
        public List<Minus35Signal> Minus35Signals
        {
            get
            {
                return All.Where(F => F is Minus35Signal).Select(F => F as Minus35Signal).ToList();
            }
        }

        /// <summary>
        /// Returns list of ThreePrimeUTR (3'UTR) features.
        /// </summary>
        public List<ThreePrimeUtr> ThreePrimeUTRs
        {
            get
            {
                return All.Where(F => F is ThreePrimeUtr).Select(F => F as ThreePrimeUtr).ToList();
            }
        }

        /// <summary>
        /// Returns list of FivePrimeUTR (5'UTR) features.
        /// </summary>
        public List<FivePrimeUtr> FivePrimeUTRs
        {
            get
            {
                return All.Where(F => F is FivePrimeUtr).Select(F => F as FivePrimeUtr).ToList();
            }
        }

        /// <summary>
        /// Returns list of Attenuator features.
        /// </summary>
        public List<Attenuator> Attenuators
        {
            get
            {
                return All.Where(F => F is Attenuator).Select(F => F as Attenuator).ToList();
            }
        }

        /// <summary>
        /// Returns list of CAATSignal (CAAT_signal) features.
        /// </summary>
        public List<CaatSignal> CAATSignals
        {
            get
            {
                return All.Where(F => F is CaatSignal).Select(F => F as CaatSignal).ToList();
            }
        }

        /// <summary>
        /// Returns list of CodingSequence (CDS) features.
        /// </summary>
        public List<CodingSequence> CodingSequences
        {
            get
            {
                return All.Where(F => F is CodingSequence).Select(F => F as CodingSequence).ToList();
            }
        }

        /// <summary>
        /// Returns list of DisplacementLoop (D-loop) features.
        /// </summary>
        public List<DisplacementLoop> DisplacementLoops
        {
            get
            {
                return All.Where(F => F is DisplacementLoop).Select(F => F as DisplacementLoop).ToList();
            }
        }

        /// <summary>
        /// Returns list of Enhancer features.
        /// </summary>
        public List<Enhancer> Enhancers
        {
            get
            {
                return All.Where(F => F is Enhancer).Select(F => F as Enhancer).ToList();
            }
        }

        /// <summary>
        /// Returns list of Exon features.
        /// </summary>
        public List<Exon> Exons
        {
            get
            {
                return All.Where(F => F is Exon).Select(F => F as Exon).ToList();
            }
        }

        /// <summary>
        /// Returns list of GCSingal (GC_signal) features.
        /// </summary>
        public List<GcSingal> GCSignals
        {
            get
            {
                return All.Where(F => F is GcSingal).Select(F => F as GcSingal).ToList();
            }
        }

        /// <summary>
        /// Returns list of Gene features.
        /// </summary>
        public List<Gene> Genes
        {
            get
            {
                return All.Where(F => F is Gene).Select(F => F as Gene).ToList();
            }
        }

        /// <summary>
        /// Returns list of InterveningDNA (iDNA) features.
        /// </summary>
        public List<InterveningDna> InterveningDNAs
        {
            get
            {
                return All.Where(F => F is InterveningDna).Select(F => F as InterveningDna).ToList();
            }
        }

        /// <summary>
        /// Returns list of Intron features.
        /// </summary>
        public List<Intron> Introns
        {
            get
            {
                return All.Where(F => F is Intron).Select(F => F as Intron).ToList();
            }
        }

        /// <summary>
        /// Returns list of LongTerminalRepeat (LTR) features.
        /// </summary>
        public List<LongTerminalRepeat> LongTerminalRepeats
        {
            get
            {
                return All.Where(F => F is LongTerminalRepeat).Select(F => F as LongTerminalRepeat).ToList();
            }
        }

        /// <summary>
        /// Returns list of MaturePeptide (mat_peptide) features.
        /// </summary>
        public List<MaturePeptide> MaturePeptides
        {
            get
            {
                return All.Where(F => F is MaturePeptide).Select(F => F as MaturePeptide).ToList();
            }
        }

        /// <summary>
        /// Returns list of MiscBinding (misc_binding) features.
        /// </summary>
        public List<MiscBinding> MiscBindings
        {
            get
            {
                return All.Where(F => F is MiscBinding).Select(F => F as MiscBinding).ToList();
            }
        }

        /// <summary>
        /// Returns list of MiscDifference (misc_difference) features.
        /// </summary>
        public List<MiscDifference> MiscDifferences
        {
            get
            {
                return All.Where(F => F is MiscDifference).Select(F => F as MiscDifference).ToList();
            }
        }

        /// <summary>
        /// Returns list of MiscFeature (misc_feature) features.
        /// </summary>
        public List<MiscFeature> MiscFeatures
        {
            get
            {
                return All.Where(F => F is MiscFeature).Select(F => F as MiscFeature).ToList();
            }
        }

        /// <summary>
        /// Returns list of MiscRecombination (misc_recomb) features.
        /// </summary>
        public List<MiscRecombination> MiscRecombinations
        {
            get
            {
                return All.Where(F => F is MiscRecombination).Select(F => F as MiscRecombination).ToList();
            }
        }

        /// <summary>
        /// Returns list of MiscRNA (misc_RNA) features.
        /// </summary>
        public List<MiscRna> MiscRNAs
        {
            get
            {
                return All.Where(F => F is MiscRna).Select(F => F as MiscRna).ToList();
            }
        }

        /// <summary>
        /// Returns list of MiscSignal (misc_signal) features.
        /// </summary>
        public List<MiscSignal> MiscSignals
        {
            get
            {
                return All.Where(F => F is MiscSignal).Select(F => F as MiscSignal).ToList();
            }
        }

        /// <summary>
        /// Returns list of MiscStructure (misc_structure) features.
        /// </summary>
        public List<MiscStructure> MiscStructures
        {
            get
            {
                return All.Where(F => F is MiscStructure).Select(F => F as MiscStructure).ToList();
            }
        }

        /// <summary>
        /// Returns list of ModifiedBase (modified_base) features.
        /// </summary>
        public List<ModifiedBase> ModifiedBases
        {
            get
            {
                return All.Where(F => F is ModifiedBase).Select(F => F as ModifiedBase).ToList();
            }
        }

        /// <summary>
        /// Returns list of MessengerRNA (mRNA) features.
        /// </summary>
        public List<MessengerRna> MessengerRNAs
        {
            get
            {
                return All.Where(F => F is MessengerRna).Select(F => F as MessengerRna).ToList();
            }
        }

        /// <summary>
        /// Returns list of NonCodingRNA (ncRNA) features.
        /// </summary>
        public List<NonCodingRna> NonCodingRNAs
        {
            get
            {
                return All.Where(F => F is NonCodingRna).Select(F => F as NonCodingRna).ToList();
            }
        }

        /// <summary>
        /// Returns list of OperonRegion (Operon) features.
        /// </summary>
        public List<OperonRegion> OperonRegions
        {
            get
            {
                return All.Where(F => F is OperonRegion).Select(F => F as OperonRegion).ToList();
            }
        }

        /// <summary>
        /// Returns list of PolyASignal (polyA_signal) features.
        /// </summary>
        public List<PolyASignal> PolyASignals
        {
            get
            {
                return All.Where(F => F is PolyASignal).Select(F => F as PolyASignal).ToList();
            }
        }

        /// <summary>
        /// Returns list of PolyASite (polyA_site) features.
        /// </summary>
        public List<PolyASite> PolyASites
        {
            get
            {
                return All.Where(F => F is PolyASite).Select(F => F as PolyASite).ToList();
            }
        }

        /// <summary>
        /// Returns list of PrecursorRNA (precursor_RNA) features.
        /// </summary>
        public List<PrecursorRna> PrecursorRNAs
        {
            get
            {
                return All.Where(F => F is PrecursorRna).Select(F => F as PrecursorRna).ToList();
            }
        }

        /// <summary>
        /// Returns list of Promoter features.
        /// </summary>
        public List<Promoter> Promoters
        {
            get
            {
                return All.Where(F => F is Promoter).Select(F => F as Promoter).ToList();
            }
        }

        /// <summary>
        /// Returns list of ProteinBindingSite (protein_bind) features.
        /// </summary>
        public List<ProteinBindingSite> ProteinBindingSites
        {
            get
            {
                return All.Where(F => F is ProteinBindingSite).Select(F => F as ProteinBindingSite).ToList();
            }
        }

        /// <summary>
        /// Returns list of RibosomeBindingSite (RBS) features.
        /// </summary>
        public List<RibosomeBindingSite> RibosomeBindingSites
        {
            get
            {
                return All.Where(F => F is RibosomeBindingSite).Select(F => F as RibosomeBindingSite).ToList();
            }
        }

        /// <summary>
        /// Returns list of ReplicationOrigin (rep_origin) features.
        /// </summary>
        public List<ReplicationOrigin> ReplicationOrigins
        {
            get
            {
                return All.Where(F => F is ReplicationOrigin).Select(F => F as ReplicationOrigin).ToList();
            }
        }

        /// <summary>
        /// Returns list of RepeatRegion (repeat_region) features.
        /// </summary>
        public List<RepeatRegion> RepeatRegions
        {
            get
            {
                return All.Where(F => F is RepeatRegion).Select(F => F as RepeatRegion).ToList();
            }
        }

        /// <summary>
        /// Returns list of RibosomalRNA (rRNA) features.
        /// </summary>
        public List<RibosomalRna> RibosomalRNAs
        {
            get
            {
                return All.Where(F => F is RibosomalRna).Select(F => F as RibosomalRna).ToList();
            }
        }

        /// <summary>
        /// Returns list of SignalPeptide (sig_peptide) features.
        /// </summary>
        public List<SignalPeptide> SignalPeptides
        {
            get
            {
                return All.Where(F => F is SignalPeptide).Select(F => F as SignalPeptide).ToList();
            }
        }

        /// <summary>
        /// Returns list of StemLoop (stem_loop) features.
        /// </summary>
        public List<StemLoop> StemLoops
        {
            get
            {
                return All.Where(F => F is StemLoop).Select(F => F as StemLoop).ToList();
            }
        }

        /// <summary>
        /// Returns list of TATASignal (TATA_signal) features.
        /// </summary>
        public List<TataSignal> TATASignals
        {
            get
            {
                return All.Where(F => F is TataSignal).Select(F => F as TataSignal).ToList();
            }
        }

        /// <summary>
        /// Returns list of Terminator features.
        /// </summary>
        public List<Terminator> Terminators
        {
            get
            {
                return All.Where(F => F is Terminator).Select(F => F as Terminator).ToList();
            }
        }

        /// <summary>
        /// Returns list of TransferMessengerRNA (tmRNA) features.
        /// </summary>
        public List<TransferMessengerRna> TransferMessengerRNAs
        {
            get
            {
                return All.Where(F => F is TransferMessengerRna).Select(F => F as TransferMessengerRna).ToList();
            }
        }

        /// <summary>
        /// Returns list of TransitPeptide (transit_peptide) features.
        /// </summary>
        public List<TransitPeptide> TransitPeptides
        {
            get
            {
                return All.Where(F => F is TransitPeptide).Select(F => F as TransitPeptide).ToList();
            }
        }

        /// <summary>
        /// Returns list of TransferRNA (tRNA) features.
        /// </summary>
        public List<TransferRna> TransferRNAs
        {
            get
            {
                return All.Where(F => F is TransferRna).Select(F => F as TransferRna).ToList();
            }
        }

        /// <summary>
        /// Returns list of UnsureSequenceRegion (unsure) features.
        /// </summary>
        public List<UnsureSequenceRegion> UnsureSequenceRegions
        {
            get
            {
                return All.Where(F => F is UnsureSequenceRegion).Select(F => F as UnsureSequenceRegion).ToList();
            }
        }

        /// <summary>
        /// Returns list of Variation features.
        /// </summary>
        public List<Variation> Variations
        {
            get
            {
                return All.Where(F => F is Variation).Select(F => F as Variation).ToList();
            }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Returns list of feature items for the specified feature key.
        /// </summary>
        /// <param name="featureKey">Feature key.</param>
        /// <returns>Returns List of feature items.</returns>
        public IList<FeatureItem> GetFeatures(string featureKey)
        {
            return All.Where(F => string.Compare(F.Key, featureKey, StringComparison.OrdinalIgnoreCase) == 0).ToList();
        }

        /// <summary>
        /// Creates a new SequenceFeatures that is a copy of the current SequenceFeatures.
        /// </summary>
        /// <returns>A new SequenceFeatures that is a copy of this SequenceFeatures.</returns>
        public SequenceFeatures Clone()
        {
            return new SequenceFeatures(this);
        }
        #endregion Methods
    }
}
