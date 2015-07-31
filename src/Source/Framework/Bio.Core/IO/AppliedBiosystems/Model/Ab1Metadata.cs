using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Bio.Util;

namespace Bio.IO.AppliedBiosystems.Model
{
    /// <summary>
    /// Stores abi metadata associated with a sequence.
    /// </summary>
    public sealed class Ab1Metadata
    {
        /// <summary>
        /// Metadata Key
        /// </summary>
        public const string MetadataKey = "Ab1Metadata";

        #region Constructors

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public Ab1Metadata()
        {
        }

#if FALSE
        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private Ab1Metadata(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException("info");
            ConfidenceData = Ab1ColorData.FromByteArray((byte[])info.GetValue(Constants.ConfidenceDataKey, typeof(byte[])));
            PeakLocations =
                Ab1ColorData.FromByteArray((byte[])info.GetValue(Constants.PeakLocationDataKey, typeof(byte[])));

            AdenineColorData = new Ab1ColorData(PeakLocations,
                                                Ab1ColorData.FromByteArray(
                                                    (byte[])info.GetValue(Constants.AdenineColorDataKey, typeof(byte[]))));
            CytosineColorData = new Ab1ColorData(PeakLocations,
                                                 Ab1ColorData.FromByteArray(
                                                     (byte[])info.GetValue(Constants.CytosineColorDataKey, typeof(byte[]))));
            ThymineColorData = new Ab1ColorData(PeakLocations,
                                                Ab1ColorData.FromByteArray(
                                                    (byte[])info.GetValue(Constants.ThymineColorDataKey, typeof(byte[]))));
            GuanineColorData = new Ab1ColorData(PeakLocations,
                                                Ab1ColorData.FromByteArray(
                                                    (byte[])info.GetValue(Constants.GuanineColorDataKey, typeof(byte[]))));
        }
#endif
        #endregion

        #region Properties

        /// <summary>
        /// Contains color data associated with Adenine.
        /// </summary>
        public Ab1ColorData AdenineColorData { get; set; }

        /// <summary>
        /// Contains color data associated with Thymine.
        /// </summary>
        public Ab1ColorData ThymineColorData { get; set; }

        /// <summary>
        /// Contains color data associated with Guanine.
        /// </summary>
        public Ab1ColorData GuanineColorData { get; set; }

        /// <summary>
        /// Contains color data associated with Cytosine.
        /// </summary>
        public Ab1ColorData CytosineColorData { get; set; }

        /// <summary>
        /// Confidence data for reads, identifying how confident the chosen nucleotide is at any given point.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public short[] ConfidenceData { get; set; }

        /// <summary>
        /// Index of the color data for each chosen peak location.  This identifies the read that was used to pick the
        /// nucleotide at a given point.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public short[] PeakLocations { get; set; }

        #endregion
#if FALSE
        #region ICloneable Members

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Flush();
                stream.Position = 0;
                return formatter.Deserialize(stream);
            }
        }

        #endregion

        #region ISerializable Members

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data. </param><param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param><exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException("info");
            info.AddValue(Constants.ConfidenceDataKey, ToByteArray(ConfidenceData));
            info.AddValue(Constants.PeakLocationDataKey, ToByteArray(PeakLocations));
            info.AddValue(Constants.AdenineColorDataKey, Ab1ColorData.ToByteArray(AdenineColorData));
            info.AddValue(Constants.CytosineColorDataKey, Ab1ColorData.ToByteArray(CytosineColorData));
            info.AddValue(Constants.ThymineColorDataKey, Ab1ColorData.ToByteArray(ThymineColorData));
            info.AddValue(Constants.GuanineColorDataKey, Ab1ColorData.ToByteArray(GuanineColorData));
        }

        #endregion
#endif

        private static byte[] ToByteArray(short[] data)
        {
            if (data == null) throw new ArgumentNullException("data");
            var values = new byte[data.Length * 2];
            int index = 0;
            for (int i = 0; i < data.Length; i++)
            {
                values[index++] = (byte)data[i];
                values[index++] = (byte)(data[i] >> 8);
            }

            return values;
        }

        #region Static Methods

        /// <summary>
        /// Attempts to get ab1 metadata from the sequence.  Returns null if none is found.
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static Ab1Metadata TryGetAb1Data(ISequence sequence)
        {
            if (sequence == null) throw new ArgumentNullException("sequence");
            object metadata;
            sequence.Metadata.TryGetValue(MetadataKey, out metadata);
            return metadata as Ab1Metadata;
        }

        /// <summary>
        /// Sets ab1 metadata on a sequence.
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="metadata"></param>
        public static void SetAb1Data(ISequence sequence, Ab1Metadata metadata)
        {
            if (sequence == null) throw new ArgumentNullException("sequence");
            if (sequence.Metadata.ContainsKey(MetadataKey))
                sequence.Metadata[MetadataKey] = metadata;
            else
                sequence.Metadata.Add(MetadataKey, metadata);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns all color data.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Ab1ColorData> AllColorData()
        {
            yield return AdenineColorData;
            yield return ThymineColorData;
            yield return GuanineColorData;
            yield return CytosineColorData;
        }

        /// <summary>
        /// Trims residue sepecific data for the new range.
        /// </summary>
        /// <param name = "startIndex">Starting residue to include.</param>
        /// <param name = "length">Number of residues to trim to.</param>
        public void Trim(int startIndex, int length)
        {
            AllColorData().Where(data => data != null).ToList()
                .ForEach(data => data.Trim(startIndex, length));
            ConfidenceData = ConfidenceData.ToList().GetRange(startIndex, length).ToArray();

            PeakLocations = new short[length];
            int absolutePeakIndex = 0;
            List<Ab1ResidueColorData> residues = AdenineColorData.DataByResidue;

            for (int i = 0; i < residues.Count; i++)
            {
                int residuePeakIndex = residues[i].PeakIndex;

                //
                // Adjust the absolute peak index basd on the residue peak index.
                //

                absolutePeakIndex += residuePeakIndex;
                PeakLocations[i] = (short)absolutePeakIndex;

                //
                // Advance the absolute peak index to point at the first data point of the next residue.
                //

                absolutePeakIndex += residues[i].Data.Length - residuePeakIndex;
            }
        }

        /// <summary>
        /// Sets color data based on the specific sequence.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="data"></param>
        public void SetColorData(byte item, Ab1ColorData data)
        {
            if (item == DnaAlphabet.Instance.A)
            {
                AdenineColorData = data;
            }
            else if (item == DnaAlphabet.Instance.T)
            {
                ThymineColorData = data;
            }
            else if (item == DnaAlphabet.Instance.G)
            {
                GuanineColorData = data;
            }
            else if (item == DnaAlphabet.Instance.C)
            {
                CytosineColorData = data;
            }
            else
            {
                throw new ArgumentException(item.ToString(CultureInfo.InvariantCulture), "item");
            }
        }

        #endregion

        #region Nested type: Constants

        private static class Constants
        {
            public const string ConfidenceDataKey = "ConfidenceData";
            public const string PeakLocationDataKey = "PeakLocationData";
            public const string AdenineColorDataKey = "AdenineColorData";
            public const string ThymineColorDataKey = "ThymnineColorData";
            public const string CytosineColorDataKey = "CytosineColorData";
            public const string GuanineColorDataKey = "GuanineColorData";
        }

        #endregion
    }
}
