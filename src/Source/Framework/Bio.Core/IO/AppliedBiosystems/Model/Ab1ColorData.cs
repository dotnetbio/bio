using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bio.Core.Extensions;
using Bio.Properties;
using Bio.Util;

namespace Bio.IO.AppliedBiosystems.Model
{
    /// <summary>
    /// Model for storing chromatogram data associated with calculated peak locations.  This is not the raw readings but the calculated readings.  
    /// There may be a desire for raw readings in order to perform custom peak calculations. 
    /// </summary>
    public class Ab1ColorData
    {
        /// <summary>
        /// Create a new color data with the specified data and peak locations.
        /// </summary>
        /// <param name="peakLocations">Peak locations</param>
        /// <param name="data">Calculated reading</param>
        public Ab1ColorData(short[] peakLocations, short[] data)
        {
            LoadResidueColorData(peakLocations, data);
            LoadMaxValue();
        }

        /// <summary>
        /// Each residue contains a sec of data points and one peak, contains a list of all data for each residue.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public List<Ab1ResidueColorData> DataByResidue { get; private set; }

        /// <summary>
        /// The maximum peak value found for all residues.
        /// </summary>
        public short Max { get; private set; }

        /// <summary>
        /// Converts the color data to a byte array.  Does not include redundant information only the color data.
        /// </summary>
        /// <param name="data">Color Data</param>
        /// <returns>Readings</returns>
        public static byte[] ToByteArray(Ab1ColorData data)
        {
            if (data == null) throw new ArgumentNullException("data");
            int dataPointCount = data.DataByResidue.Sum(residue => residue.Data.Length);

            var values = new byte[dataPointCount * 2];
            int index = 0;
            foreach (Ab1ResidueColorData residue in data.DataByResidue)
            {
                short[] residueData = residue.Data;
                for (int i = 0; i < residueData.Length; i++)
                {
                    short value = residueData[i];
                    values[index++] = (byte)value;
                    values[index++] = (byte)(value >> 8);
                }
            }

            return values;
        }

        /// <summary>
        /// Converts the byte array to an array of shorts
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static short[] FromByteArray(byte[] value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (value.Length % 2 != 0)
                throw new ArgumentException(Resource.Ab1ColorDataFromByteArrayEvenNumberRequired, "value");

            var data = new short[value.Length / 2];

            for (int i = 0; i < value.Length; i += 2)
                data[i / 2] = (short)((value[i + 1] << 8) | value[i]);

            return data;
        }

        /// <summary>
        /// Helper method for converting a list of shorts to the same format found in the ab1 xml parser.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public static string ToString(List<short> data)
        {
            if (data == null) throw new ArgumentNullException("data");
            var builder = new StringBuilder();
            data.ForEach(value => builder.AppendFormat(builder.Length == 0 ? "{0}" : " {0}", value));

            return builder.ToString();
        }

        /// <summary>
        /// Trims the color data, adjust peak locations and data based on the new residue range.
        /// </summary>
        /// <param name = "startIndex">Starting residue to include.</param>
        /// <param name = "length">Number of residues to trim to.</param>
        public void Trim(int startIndex, int length)
        {
            if (startIndex < 0 || DataByResidue.Count <= startIndex)
                throw new ArgumentException(Resource.ColorDataTrimStartIndexOutOfRange, "startIndex");
            if (startIndex + length > DataByResidue.Count)
                throw new ArgumentException(Resource.ColorDataTrimLengthArgumentException, "length");

            DataByResidue = new List<Ab1ResidueColorData>(
                DataByResidue.GetRange(startIndex, length));
            LoadMaxValue();
        }

        /// <summary>
        /// Loads all residue color data.
        /// </summary>
        /// <param name="peakLocations"></param>
        /// <param name="data"></param>
        private void LoadResidueColorData(short[] peakLocations, short[] data)
        {
            DataByResidue = new List<Ab1ResidueColorData>();

            int residueDataIndex = 0;

            for (int i = 0; i < peakLocations.Length - 1; i++)
            {
                int peakIndex = peakLocations[i];
                int nextPeakIndex = peakLocations[i + 1];
                int residueEndIndex = (peakIndex + (nextPeakIndex - peakIndex) / 2);

                AddResidueColorData(peakIndex, residueDataIndex, residueEndIndex, data);

                residueDataIndex = residueEndIndex + 1;
            }

            //
            // Add the last residue data
            // 

            AddResidueColorData(peakLocations.Last(), residueDataIndex, data.Length - 1, data);
        }

        /// <summary>
        /// Adds a new color data definition for a residue to <see cref="DataByResidue"/>.
        /// </summary>
        /// <param name="peakIndex"></param>
        /// <param name="residueDataIndex"></param>
        /// <param name="residueEndIndex"></param>
        /// <param name="data"></param>
        private void AddResidueColorData(int peakIndex, int residueDataIndex, int residueEndIndex, short[] data)
        {
            var residueColorData =
                new Ab1ResidueColorData
                    {
                        //
                        // There appears to be situations in AB1 files where 2 residues share the same peak index, this breaks the logic below by assigning
                        // the peak index a negative value, so we can avoid that by maxing to 0.
                        //

                        PeakIndex = Math.Max(0, peakIndex - residueDataIndex),
                        Data = data.GetRange(residueDataIndex, residueEndIndex - residueDataIndex + 1)
                    };

            DataByResidue.Add(residueColorData);
        }

        /// <summary>
        /// Calculates and stores the max peak value.
        /// </summary>
        private void LoadMaxValue()
        {
            Max = 0;
            DataByResidue.ForEach(item => Max = Math.Max(Max, item.PeakValue));
        }
    }
}
