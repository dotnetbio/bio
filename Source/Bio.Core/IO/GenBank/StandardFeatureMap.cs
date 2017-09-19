using System;
using System.Collections.Generic;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// Class to map each standard feature key to the class which can hold that feature.
    /// Note that the classes which can hold feature has to be derived from FeatureItem class.
    /// </summary>
    public static class StandardFeatureMap
    {
        // Dictionary hold feature key to class map.
        private static Dictionary<string, Type> featureMap;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static StandardFeatureMap()
        {
            featureMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            featureMap.Add(StandardFeatureKeys.Minus10Signal, typeof(Minus10Signal));
            featureMap.Add(StandardFeatureKeys.Minus35Signal, typeof(Minus35Signal));
            featureMap.Add(StandardFeatureKeys.ThreePrimeUtr, typeof(ThreePrimeUtr));
            featureMap.Add(StandardFeatureKeys.FivePrimeUtr, typeof(FivePrimeUtr));
            featureMap.Add(StandardFeatureKeys.Attenuator, typeof(Attenuator));
            featureMap.Add(StandardFeatureKeys.CaaTSignal, typeof(CaatSignal));
            featureMap.Add(StandardFeatureKeys.CodingSequence, typeof(CodingSequence));
            featureMap.Add(StandardFeatureKeys.DisplacementLoop, typeof(DisplacementLoop));
            featureMap.Add(StandardFeatureKeys.Enhancer, typeof(Enhancer));
            featureMap.Add(StandardFeatureKeys.Exon, typeof(Exon));
            featureMap.Add(StandardFeatureKeys.GcSingal, typeof(GcSingal));
            featureMap.Add(StandardFeatureKeys.Gene, typeof(Gene));
            featureMap.Add(StandardFeatureKeys.InterveningDna, typeof(InterveningDna));
            featureMap.Add(StandardFeatureKeys.Intron, typeof(Intron));
            featureMap.Add(StandardFeatureKeys.LongTerminalRepeat, typeof(LongTerminalRepeat));
            featureMap.Add(StandardFeatureKeys.MaturePeptide, typeof(MaturePeptide));
            featureMap.Add(StandardFeatureKeys.MiscBinding, typeof(MiscBinding));
            featureMap.Add(StandardFeatureKeys.MiscDifference, typeof(MiscDifference));
            featureMap.Add(StandardFeatureKeys.MiscFeature, typeof(MiscFeature));
            featureMap.Add(StandardFeatureKeys.MiscRecombination, typeof(MiscRecombination));
            featureMap.Add(StandardFeatureKeys.MiscRna, typeof(MiscRna));
            featureMap.Add(StandardFeatureKeys.MiscSignal, typeof(MiscSignal));
            featureMap.Add(StandardFeatureKeys.MiscStructure, typeof(MiscStructure));
            featureMap.Add(StandardFeatureKeys.ModifiedBase, typeof(ModifiedBase));
            featureMap.Add(StandardFeatureKeys.MessengerRna, typeof(MessengerRna));
            featureMap.Add(StandardFeatureKeys.NonCodingRna, typeof(NonCodingRna));
            featureMap.Add(StandardFeatureKeys.OperonRegion, typeof(OperonRegion));
            featureMap.Add(StandardFeatureKeys.PolyASignal, typeof(PolyASignal));
            featureMap.Add(StandardFeatureKeys.PolyASite, typeof(PolyASite));
            featureMap.Add(StandardFeatureKeys.PrecursorRna, typeof(PrecursorRna));
            featureMap.Add(StandardFeatureKeys.Promoter, typeof(Promoter));
            featureMap.Add(StandardFeatureKeys.ProteinBindingSite, typeof(ProteinBindingSite));
            featureMap.Add(StandardFeatureKeys.RibosomeBindingSite, typeof(RibosomeBindingSite));
            featureMap.Add(StandardFeatureKeys.ReplicationOrigin, typeof(ReplicationOrigin));
            featureMap.Add(StandardFeatureKeys.RepeatRegion, typeof(RepeatRegion));
            featureMap.Add(StandardFeatureKeys.RibosomalRna, typeof(RibosomalRna));
            featureMap.Add(StandardFeatureKeys.SignalPeptide, typeof(SignalPeptide));
            featureMap.Add(StandardFeatureKeys.StemLoop, typeof(StemLoop));
            featureMap.Add(StandardFeatureKeys.TataSignal, typeof(TataSignal));
            featureMap.Add(StandardFeatureKeys.Terminator, typeof(Terminator));
            featureMap.Add(StandardFeatureKeys.TransferMessengerRna, typeof(TransferMessengerRna));
            featureMap.Add(StandardFeatureKeys.TransitPeptide, typeof(TransitPeptide));
            featureMap.Add(StandardFeatureKeys.TransferRna, typeof(TransferRna));
            featureMap.Add(StandardFeatureKeys.UnsureSequenceRegion, typeof(UnsureSequenceRegion));
            featureMap.Add(StandardFeatureKeys.Variation, typeof(Variation));
        }

        /// <summary>
        /// Returns standard feature class instance, if the key in the specified feature item is found 
        /// in the map; otherwise returns the specified feature item itself.
        /// For example:
        /// If the specified feature item has the key "Gene" then this method returns instance of the Gene class
        /// with data copied from the specified item.
        /// </summary>
        /// <param name="item">Feature item instance to which the standard feature item instance is needed.</param>
        /// <returns>If found returns appropriate class instance for the specified feature item, otherwise returns 
        /// the specified item itself.</returns>
        public static FeatureItem GetStandardFeatureItem(FeatureItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            Type type = null;
            if (featureMap.ContainsKey(item.Key))
            {
                type = featureMap[item.Key];
            }

            if (type != null)
            {
                FeatureItem newItem = (FeatureItem)Activator.CreateInstance(type, item.Location);

                foreach (KeyValuePair<string, List<string>> kvp in item.Qualifiers)
                {
                    newItem.Qualifiers.Add(kvp.Key, kvp.Value);
                }

                item = newItem;
            }

            return item;
        }
    }
}
