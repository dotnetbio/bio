using System;
using System.Collections.Generic;
using System.Globalization;

namespace Bio
{
    /// <summary>
    /// A SequenceRange holds the data necessary to represent a region within
    /// a sequence defined by its start and end index without necessarily holding
    /// any of the sequence item data. At a minimum and ID, start index, and end
    /// index are required. Additional metadata can be stored as well using a
    /// generic key value pair.
    /// </summary>
    public class SequenceRange : ISequenceRange
    {
        private long start = 0;
        private long end = long.MaxValue;
        private string id = String.Empty;
        private Dictionary<string, object> metadata = null;

        private List<ISequenceRange> parentSeqRanges = null;
        /// <summary>
        /// Default constructor that does not set any fields.
        /// </summary>
        public SequenceRange() { }

        /// <summary>
        /// Data constructor that sets the most commonly used fields.
        /// Note that if the end value is less than start value then the end values is assigned to the start value.
        /// </summary>
        /// <param name="id">An ID for the range. This does not need to be unique, and often represents the chromosome of the range.</param>
        /// <param name="start">A starting index for the range. In the BED format this index starts counting from 0.</param>
        /// <param name="end">An ending index for the range. In the BED format this index is exclusive.</param>
        public SequenceRange(string id, long start, long end)
        {
            this.id = id;
            Start = start;
            End = end;
        }

        /// <summary>
        /// The beginning index of the range. This index must be non-negative and
        /// it will be enforced to always be less than or equal to the End index.
        /// </summary>
        public long Start
        {
            get { return start; }
            set
            {
                if (value < 0)
                    throw new IndexOutOfRangeException(Properties.Resource.SequenceRangeNonNegative);

                if (value > end)
                    throw new IndexOutOfRangeException(Properties.Resource.SequenceRangeStartError);
                start = value;
            }
        }

        /// <summary>
        /// The end index of the range. This index must be non-negative and
        /// it will be enforced to always be greater than or equal to the Start index.
        /// </summary>
        public long End
        {
            get { return end; }
            set
            {
                if (value < 0)
                    throw new IndexOutOfRangeException(Properties.Resource.SequenceRangeNonNegative);

                if (value < start)
                    throw new IndexOutOfRangeException(Properties.Resource.SequenceRangeEndError);
                end = value;
            }
        }

        /// <summary>
        /// A string identifier of the sequence range.
        /// </summary>
        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// The length of the range, which can be zero. This result is the
        /// difference of the End and Start index.
        /// </summary>
        public long Length
        {
            get { return End - Start; }
        }

        /// <summary>
        /// Optional additional data to store along with the ID and indices of
        /// the range. Metadata must be stored with a string key name.
        /// </summary>
        public Dictionary<string, object> Metadata
        {
            get
            {
                if (metadata == null)
                    metadata = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                return metadata;
            }
        }

        /// <summary>
        /// Gets the sequence ranges from which this sequence range is obtained.
        /// This property will be filled by the operations like Merge, Intersect etc.
        /// </summary>
        public List<ISequenceRange> ParentSeqRanges 
        {
            get
            {
                if (parentSeqRanges == null)
                    parentSeqRanges = new List<ISequenceRange>();

                return parentSeqRanges;
            }
        } 

        #region IComparable Members
        /// <summary>
        /// Compares two sequence ranges.
        /// </summary>
        /// <param name="obj">SequenceRange instance to compare.</param>
        /// <returns>
        /// If the Start values of the two ranges are identical then the
        /// result of this comparison is the result from calling CompareTo() on
        /// the two End values. If the Start values are not equal then the result
        /// of this comparison is the result of calling CompareTo() on the two
        /// Start values.
        /// </returns>
        public int CompareTo(object obj)
        {
            if (obj == null)
                return 0;

            SequenceRange sequenceRange = obj as SequenceRange;
            if (obj == null)
                return 0;

            return CompareTo(sequenceRange);
        }

        #endregion

        #region IComparable<ISequenceRange> Members
        /// <summary>
        /// Compares two sequence ranges.
        /// </summary>
        /// <param name="other">SequenceRange instance to compare.</param>
        /// <returns>
        /// If the Start values of the two ranges are identical then the
        /// result of this comparison is the result from calling CompareTo() on
        /// the two End values. If the Start values are not equal then the result
        /// of this comparison is the result of calling CompareTo() on the two
        /// Start values.
        /// </returns>
        public int CompareTo(ISequenceRange other)
        {
            if (other == null)
            {
                return -1;
            }

            int compare = Start.CompareTo(other.Start);

            if (compare == 0)
                compare = End.CompareTo(other.End);

            if (compare == 0)
                compare = string.Compare(ID, other.ID, StringComparison.OrdinalIgnoreCase);

            if (compare == 0)
            {
                compare = ParentSeqRanges.Count.CompareTo(other.ParentSeqRanges.Count);

                if (compare == 0)
                {
                    for (int index = 0; index < ParentSeqRanges.Count; index++)
                    {
                        compare = ParentSeqRanges[index].CompareTo(other.ParentSeqRanges[index]);
                        if (compare != 0)
                            break;
                    }
                }
            }

            return compare;
        }

        #endregion

        /// <summary>
        /// Overrides hash function for a particular type.
        /// </summary>
        /// <returns>hash code</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Overrides the equal method
        /// </summary>
        /// <param name="obj">Object to be checked</param>
        /// <returns>Is equals</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return base.Equals(obj);
        }

        /// <summary>
        /// Override equal operator
        /// </summary>
        /// <param name="leftHandSideObject">LHS object</param>
        /// <param name="rightHandSideObject">RHS object</param>
        /// <returns>Is LHS == RHS</returns>
        public static bool operator ==(SequenceRange leftHandSideObject, SequenceRange rightHandSideObject)
        {
            return System.Object.ReferenceEquals(leftHandSideObject, rightHandSideObject);
        }

        /// <summary>
        /// Override not equal operator
        /// </summary>
        /// <param name="leftHandSideObject">LHS object</param>
        /// <param name="rightHandSideObject">RHS object</param>
        /// <returns>Is LHS == RHS</returns>
        public static bool operator !=(SequenceRange leftHandSideObject, SequenceRange rightHandSideObject)
        {
            return !(leftHandSideObject == rightHandSideObject);
        }

        /// <summary>
        /// Override less than operator
        /// </summary>
        /// <param name="leftHandSideObject">LHS object</param>
        /// <param name="rightHandSideObject">RHS object</param>
        /// <returns>Is LHS == RHS</returns>
        public static bool operator <(SequenceRange leftHandSideObject, SequenceRange rightHandSideObject)
        {
            if (object.ReferenceEquals(leftHandSideObject, null) || object.ReferenceEquals(rightHandSideObject, null))
            {
                return false;
            }

            return (leftHandSideObject.CompareTo(rightHandSideObject) < 0);
        }

        /// <summary>
        /// Override greater than operator
        /// </summary>
        /// <param name="leftHandSideObject">LHS object</param>
        /// <param name="rightHandSideObject">RHS object</param>
        /// <returns>Is LHS == RHS</returns>
        public static bool operator >(SequenceRange leftHandSideObject, SequenceRange rightHandSideObject)
        {
            if (object.ReferenceEquals(leftHandSideObject, null) || object.ReferenceEquals(rightHandSideObject, null))
            {
                return false;
            }

            return (leftHandSideObject.CompareTo(rightHandSideObject) > 0);
        }

       /// <summary>
        /// Converts ID, Start, End of the sequence to string.
       /// </summary>
        /// <returns>ID, Start, End of the sequence.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, Properties.Resource.SequenceRangeToStringFormat, this.ID, this.Start, this.End);
        }
    }
}
