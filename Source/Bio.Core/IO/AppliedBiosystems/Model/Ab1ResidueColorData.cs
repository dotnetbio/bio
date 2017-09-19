namespace Bio.IO.AppliedBiosystems.Model
{
    /// <summary>
    /// Color data associated with a residue.
    /// </summary>
    public class Ab1ResidueColorData
    {
        /// <summary>
        /// The color value at the designated peak location.
        /// </summary>
        public short PeakValue
        {
            get { return Data[PeakIndex]; }
        }

        /// <summary>
        /// Peak index relative to the residue color data.
        /// </summary>
        public int PeakIndex { get; set; }

        /// <summary>
        /// Individual color data reading relative to this residue.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public short[] Data { get; set; }
    }
}
