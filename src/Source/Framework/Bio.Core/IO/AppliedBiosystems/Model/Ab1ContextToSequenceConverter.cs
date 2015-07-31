using System.Collections.Generic;
using System.Linq;
using Bio.IO.AppliedBiosystems.DataParsers;
using Bio.IO.AppliedBiosystems.DataTypes;
using Bio.Util;

namespace Bio.IO.AppliedBiosystems.Model
{
    /// <summary>
    /// Converts a parser context into a sequence object type.
    /// </summary>
    public static class Ab1ContextToSequenceConverter
    {
        /// <summary>
        /// Converts the parsed ab1 raw data file into a sequence, with the associated ab1 metadata setup.  I use
        /// this method because only a subset of the data is needed so there is no point in persisting a large amount
        /// of unused metadata.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ISequence Convert(IParserContext context)
        {
            ISequence sequence = GetSequence(context);
            Ab1Metadata metadata = GetAb1Metadata(context);
            Ab1Metadata.SetAb1Data(sequence, metadata);
            return sequence;
        }

        private static Ab1Metadata GetAb1Metadata(IParserContext context)
        {
            var metadata = new Ab1Metadata();
            LoadPeakLocations(metadata, context);
            LoadSequenceConfidence(metadata, context);
            LoadColorWheelData(metadata, context);
            return metadata;
        }

        private static void LoadColorWheelData(Ab1Metadata metadata, IParserContext context)
        {
            List<KeyValuePair<byte, int>> nucleotideIndices = GetNucleotideDataIndex(context);
            IEnumerable<ShortDataItem> dataItems = context.DataItems.OfType<ShortDataItem>();
            nucleotideIndices.ForEach(
                pair =>
                    {
                        ShortDataItem item =
                            dataItems.First(i => i.Entry.TagNumber == pair.Value && i.Entry.TagName == Constants.DataTagName);
                        metadata.SetColorData(
                            pair.Key,
                            new Ab1ColorData(metadata.PeakLocations, item.Value));
                    });
        }

        private static void LoadSequenceConfidence(Ab1Metadata metadata, IParserContext context)
        {
            metadata.ConfidenceData =
                ((CharDataItem)context.DataItems.First(item => item.Entry.TagName == Constants.SequenceConfidanceTagName)).Value.Select(c => (short)c).ToArray();
        }

        private static void LoadPeakLocations(Ab1Metadata metadata, IParserContext context)
        {
            metadata.PeakLocations =
                ((ShortDataItem)context
                                    .DataItems
                                    .First(item =>
                                           item.Entry.TagName == Constants.PeakLocationTagName)).Value;
        }

        /// <summary>
        /// Retrieves the user defined sequence.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static ISequence GetSequence(IParserContext context)
        {
            char[] value = context.DataItems
                .OfType<CharDataItem>()
                .First(item => item.Entry.TagName == Constants.SequenceTagName)
                .Value;

            return new Sequence(context.Alphabet ?? Alphabets.DNA, new string(value));
        }

        /// <summary>
        /// Returns the order of nucleotides based on the ab1 color wheel definition.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static List<KeyValuePair<byte, int>> GetNucleotideDataIndex(IParserContext context)
        {
            char[] value = context.DataItems
                .OfType<CharDataItem>()
                .First(item => item.Entry.TagName == Constants.SequencingOrderTagName)
                .Value;

            var items = new List<KeyValuePair<byte, int>>();

            // 
            // Analyzed color wheel data is contained in data tags 9-12
            //

            byte[] alphabetMap = DnaAlphabet.Instance.GetSymbolValueMap();

            var index = new[] {9};
            value.ToList().ForEach(
                c =>
                    {
                        items.Add(new KeyValuePair<byte, int>(alphabetMap[c], index[0]));
                        index[0]++;
                    });

            return items;
        }

        #region Nested type: Constants

        private static class Constants
        {
            public const string SequenceTagName = "PBAS";
            public const string SequenceConfidanceTagName = "PCON";
            public const string SequencingOrderTagName = "FWO_";
            public const string DataTagName = "DATA";
            public const string PeakLocationTagName = "PLOC";
        }

        #endregion
    }
}
