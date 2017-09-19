using System;
using System.Globalization;

namespace Bio.IO.BAM
{
    /// <summary>
    /// Class to hold offset of a BAM file.
    /// </summary>
    public struct FileOffset : IComparable<FileOffset>
    {
        /// <summary>
        /// The two types are stored  in one 64 bit field here.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        public ulong BothDataElements;
        /// <summary>
        /// Gets or sets BGZF block start offset.
        /// </summary>
        public UInt64 CompressedBlockOffset { 
            get 
            {
                return (BothDataElements >> 16);
            } 
        }

        /// <summary>
        /// Gets or sets an offset of uncompressed block inside a BGZF block 
        /// from which aligned sequences starts or ends.
        /// </summary>
        public UInt16 UncompressedBlockOffset
        {
            get 
            {
                return (ushort)BothDataElements;
            }
        }
        /// <summary>
        /// Create a new file offset.
        /// </summary>
        /// <param name="compressedStreamPosition"></param>
        /// <param name="decompressedStreamPosition"></param>
        public FileOffset(ulong compressedStreamPosition, ushort decompressedStreamPosition)
        {
            BothDataElements = (compressedStreamPosition << 16) | decompressedStreamPosition;
 
        }

        /// <summary>
        /// Compares two FileOffsets.
        /// </summary>
        /// <param name="other">Other file offset to compare.</param>
        public int CompareTo(FileOffset other)
        {
            return this.BothDataElements.CompareTo(other.BothDataElements);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified FileOffset instance.
        /// </summary>
        /// <param name="obj">An FileOffset instance to compare to this instance.</param>
        /// <returns> true if obj has the same value as this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is FileOffset)) { return false; }
            FileOffset other = (FileOffset) obj;
            return this.BothDataElements == other.BothDataElements ;
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified FileOffset instance.
        /// </summary>
        /// <param name="other">An FileOffset instance to compare to this instance.</param>
        /// <returns> true if other has the same value as this instance; otherwise, false.</returns>
        public bool Equals(FileOffset other)
        {
            return this.BothDataElements == other.BothDataElements;
        }

        /// <summary>
        /// Returns a value indicating whether x is equal to y.
        /// </summary>
        /// <param name="x">First operand.</param>
        /// <param name="y">second operand.</param>
        public static bool operator ==(FileOffset x, FileOffset y)
        {
            return x.BothDataElements == y.BothDataElements;
        }

        /// <summary>
        /// Returns a value indicating whether x is greater than y.
        /// </summary>
        /// <param name="x">First operand.</param>
        /// <param name="y">second operand.</param>
        public static bool operator >(FileOffset x, FileOffset y)
        {
            return x.BothDataElements>y.BothDataElements;
        }
        /// <summary>
        /// Returns a value indicating whether x is greater than or equal to y.
        /// </summary>
        /// <param name="x">First operand.</param>
        /// <param name="y">second operand.</param>
        public static bool operator >=(FileOffset x, FileOffset y)
        {
            return x.BothDataElements >= y.BothDataElements;
        }

        /// <summary>
        /// Returns a value indicating whether x is less than or equal to y.
        /// </summary>
        /// <param name="x">First operand.</param>
        /// <param name="y">second operand.</param>
        public static bool operator <=(FileOffset x, FileOffset y)
        {
            return x.BothDataElements <= y.BothDataElements;
        }

        /// <summary>
        /// Returns a value indicating whether x is less than y.
        /// </summary>
        /// <param name="x">First operand.</param>
        /// <param name="y">second operand.</param>
        public static bool operator <(FileOffset x, FileOffset y)
        {
            return x.BothDataElements < y.BothDataElements;
        }

        /// <summary>
        /// Returns a value indicating whether x is not equal to y.
        /// </summary>
        /// <param name="x">First operand.</param>
        /// <param name="y">second operand.</param>
        public static bool operator !=(FileOffset x, FileOffset y)
        {
            return !(x == y);
        }

        /// <summary>
        /// Gets the Hashcode for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            return this.CompressedBlockOffset.GetHashCode() ^ this.UncompressedBlockOffset.GetHashCode();
        }
    }
}
