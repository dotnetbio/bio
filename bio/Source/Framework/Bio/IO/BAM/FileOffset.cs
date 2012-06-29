using System;
using System.Globalization;

namespace Bio.IO.BAM
{
    /// <summary>
    /// Class to hold offset of a BAM file.
    /// </summary>
    public class FileOffset : IComparable<FileOffset>
    {
        /// <summary>
        /// Gets or sets BGZF block start offset.
        /// </summary>
        public UInt64 CompressedBlockOffset { get; set; }

        /// <summary>
        /// Gets or sets an offset of uncompressed block inside a BGZF block 
        /// from which aligned sequences starts or ends.
        /// </summary>
        public UInt16 UncompressedBlockOffset { get; set; }

        /// <summary>
        /// Compares two FileOffsets.
        /// </summary>
        /// <param name="other">Other file offset to compare.</param>
        public int CompareTo(FileOffset other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return 1;
            }

            int result = CompressedBlockOffset.CompareTo(other.CompressedBlockOffset);
            if (result == 0)
            {
                result = UncompressedBlockOffset.CompareTo(other.UncompressedBlockOffset);
            }

            return result;
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified FileOffset instance.
        /// </summary>
        /// <param name="obj">An FileOffset instance to compare to this instance.</param>
        /// <returns> true if obj has the same value as this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            FileOffset other = obj as FileOffset;

            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            return this.CompressedBlockOffset == other.CompressedBlockOffset && this.UncompressedBlockOffset == other.UncompressedBlockOffset;
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified FileOffset instance.
        /// </summary>
        /// <param name="other">An FileOffset instance to compare to this instance.</param>
        /// <returns> true if other has the same value as this instance; otherwise, false.</returns>
        public bool Equals(FileOffset other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            return this.CompressedBlockOffset == other.CompressedBlockOffset && this.UncompressedBlockOffset == other.UncompressedBlockOffset;
        }

        /// <summary>
        /// Returns a value indicating whether x is equal to y.
        /// </summary>
        /// <param name="x">First operand.</param>
        /// <param name="y">second operand.</param>
        public static bool operator ==(FileOffset x, FileOffset y)
        {
            if (Object.ReferenceEquals(x, y))
            {
                return true;
            }

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(null, y))
            {
                return false;
            }

            return x.Equals(y);
        }

        /// <summary>
        /// Returns a value indicating whether x is greater than y.
        /// </summary>
        /// <param name="x">First operand.</param>
        /// <param name="y">second operand.</param>
        public static bool operator >(FileOffset x, FileOffset y)
        {
            if (Object.ReferenceEquals(x, y))
            {
                return false;
            }

            if (Object.ReferenceEquals(x, null))
            {
                return false;
            }

            return x.CompareTo(y) > 0;
        }

        /// <summary>
        /// Returns a value indicating whether x is less than y.
        /// </summary>
        /// <param name="x">First operand.</param>
        /// <param name="y">second operand.</param>
        public static bool operator <(FileOffset x, FileOffset y)
        {
            if (Object.ReferenceEquals(x, y))
            {
                return false;
            }

            if (Object.ReferenceEquals(y, null))
            {
                return false;
            }

            return y.CompareTo(x) > 0;
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
