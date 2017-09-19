using System;
using System.Globalization;

namespace Bio
{
    /// <summary>
    /// IndexedItem holds an item and its index.
    /// Index is a zero based position of item.
    /// This class is used in Sparse Sequence to get the known sequence items with their positions.
    /// 
    /// This class implements IComparable interface and all comparisons are based on index 
    /// and not on item.
    /// </summary>
    /// <typeparam name="T">The type of item in IndexedItem.</typeparam>
    public class IndexedItem<T> : IComparable<IndexedItem<T>>, IComparable
    {
        #region Constructors
        /// <summary>
        /// Creates a new IndexedItem from the specified index and item.
        /// </summary>
        /// <param name="index">Index of the item specified.</param>
        /// <param name="item">Item.</param>
        public IndexedItem(long index, T item)
        {
            Index = index;
            Item = item;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the index.
        /// Specifies the zero based position of the item.
        /// </summary>
        public long Index
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        public T Item
        {
            get;
            set;
        }

        #endregion Properties

        #region IComparable<IndexedItem<T>> Members

        /// <summary>
        /// Compares the index of leftHandSideObject and rightHandSideObject, if index of leftHandSideObject is less than 
        /// the index of rightHandSideObject then returns true, else returns false.
        /// 
        /// Note that this method will compare only index and will not compare Item property.
        /// </summary>
        /// <param name="leftHandSideObject">An instance of IndexedItem as first operand.</param>
        /// <param name="rightHandSideObject">An instance of IndexedItem as second operand.</param>
        /// <returns>Returns true if index of leftHandSideObject is less than the index of rightHandSideObject,
        /// else returns false.</returns>
        public static bool operator <(IndexedItem<T> leftHandSideObject, IndexedItem<T> rightHandSideObject)
        {
            if (ReferenceEquals(leftHandSideObject, null) && ReferenceEquals(rightHandSideObject, null))
            {
                return false;
            }

            if (ReferenceEquals(leftHandSideObject, null))
            {
                return true;
            }

            return leftHandSideObject.CompareTo(rightHandSideObject) < 0;
        }

        /// <summary>
        /// Compares the index of leftHandSideObject and rightHandSideObject, if index of leftHandSideObject is 
        /// less than or equal to the index of rightHandSideObject then returns true else returns false.
        /// 
        /// Note that this method will compare only index and will not compare Item property.
        /// </summary>
        /// <param name="leftHandSideObject">An instance of IndexedItem as first operand.</param>
        /// <param name="rightHandSideObject">An instance of IndexedItem as second operand.</param>
        /// <returns>Returns true if index of leftHandSideObject is less than or equal to the index of rightHandSideObject,
        /// else returns false.</returns>
        public static bool operator <=(IndexedItem<T> leftHandSideObject, IndexedItem<T> rightHandSideObject)
        {
            if (ReferenceEquals(leftHandSideObject, null) && ReferenceEquals(rightHandSideObject, null))
            {
                return true;
            }

            if (ReferenceEquals(leftHandSideObject, null))
            {
                return true;
            }

            return leftHandSideObject.CompareTo(rightHandSideObject) <= 0;
        }

        /// <summary>
        /// Compares the index of leftHandSideObject and rightHandSideObject, if index of leftHandSideObject is greater than 
        /// the index of rightHandSideObject then returns true else returns false.
        /// 
        /// Note that this method will compare only index and will not compare Item property.
        /// </summary>
        /// <param name="leftHandSideObject">An instance of IndexedItem as first operand.</param>
        /// <param name="rightHandSideObject">An instance of IndexedItem as second operand.</param>
        /// <returns>Returns true if index of leftHandSideObject is greater than the index of rightHandSideObject,
        /// else returns false.</returns>
        public static bool operator >(IndexedItem<T> leftHandSideObject, IndexedItem<T> rightHandSideObject)
        {
            return !(leftHandSideObject <= rightHandSideObject);
        }

        /// <summary>
        /// Compares the index of leftHandSideObject and rightHandSideObject, if index of leftHandSideObject is greater than
        /// or equal to the index of rightHandSideObject then returns true else returns false.
        /// 
        /// Note that this method will compare only index and will not compare Item property.
        /// </summary>
        /// <param name="leftHandSideObject">An instance of IndexedItem as first operand.</param>
        /// <param name="rightHandSideObject">An instance of IndexedItem as second operand.</param>
        /// <returns>Returns true if index of leftHandSideObject is greater than or equal to the index of rightHandSideObject,
        /// else returns false.</returns>
        public static bool operator >=(IndexedItem<T> leftHandSideObject, IndexedItem<T> rightHandSideObject)
        {
            return !(leftHandSideObject < rightHandSideObject);
        }

        /// <summary>
        /// Compares the index of leftHandSideObject and rightHandSideObject, if index of leftHandSideObject is
        /// equal to the index of rightHandSideObject then returns true else returns false.
        /// 
        /// Note that this method will compare only index and will not compare Item property.
        /// </summary>
        /// <param name="leftHandSideObject">An instance of IndexedItem as first operand.</param>
        /// <param name="rightHandSideObject">An instance of IndexedItem as second operand.</param>
        /// <returns>Returns true if index of leftHandSideObject is equal to the index of rightHandSideObject,
        /// else returns false.</returns>
        public static bool operator ==(IndexedItem<T> leftHandSideObject, IndexedItem<T> rightHandSideObject)
        {
            if (ReferenceEquals(leftHandSideObject, null) || ReferenceEquals(rightHandSideObject, null))
            {
                return false;
            }

            return leftHandSideObject.Equals(rightHandSideObject);
        }

        /// <summary>
        /// Compares the index of leftHandSideObject and rightHandSideObject, if index of leftHandSideObject is
        /// not equal to the index of rightHandSideObject then returns true else returns false.
        /// 
        /// Note that this method will compare only index and will not compare Item property.
        /// </summary>
        /// <param name="leftHandSideObject">An instance of IndexedItem as first operand.</param>
        /// <param name="rightHandSideObject">An instance of IndexedItem as second operand.</param>
        /// <returns>Returns true if index of leftHandSideObject is not equal to the index of rightHandSideObject,
        /// else returns false.</returns>
        public static bool operator !=(IndexedItem<T> leftHandSideObject, IndexedItem<T> rightHandSideObject)
        {
            if (ReferenceEquals(leftHandSideObject, null) || ReferenceEquals(rightHandSideObject, null))
            {
                return true;
            }

            return !leftHandSideObject.Equals(rightHandSideObject);
        }

        /// <summary>
        /// Compares Index property of this instance with the Index property of specified IndexedItem 
        /// and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">IndexedItem to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of index property of this instance and the 
        /// index property of other.
        /// Return value Description:
        ///       Less than zero index of this instance is less than the index of other.
        ///       Zero index of this instance is equal to the index of other.
        ///       Greater than zero index of this instance is greater than the index of other 
        ///         or other is null.
        /// </returns>
        public int CompareTo(IndexedItem<T> other)
        {
            if (ReferenceEquals(other, null))
            {
                return 1;
            }

            return Index.CompareTo(other.Index);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified IndexedItem.
        /// 
        /// Note that this method compares both Index and Item. If both Index and Item of this instance and 
        /// other are equal then it returns true, else returns false.
        /// </summary>
        /// <param name="other">IndexedItem instance to compare.</param>
        /// <returns>Returns true if other has the same index and item values as of this instance;
        /// otherwise, false.</returns>
        public bool Equals(IndexedItem<T> other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return Index.Equals(other.Index) && Item.Equals(other.Item);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified IndexedItem.
        /// 
        /// Note that this method compares both index and item. If both Index and Item of this instance and 
        /// obj are equal then it returns true else returns false.
        /// </summary>
        /// <param name="obj">IndexedItem instance to compare.</param>
        /// <returns>Returns true if obj has the same index and item values as of this instance;
        /// otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            IndexedItem<T> other = obj as IndexedItem<T>;

            if (other == null)
            {
                return false;
            }

            return Equals(other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            if (Item == null)
            {
                return Index.ToString(CultureInfo.InvariantCulture).GetHashCode();
            }

            return Index.ToString(CultureInfo.InvariantCulture).GetHashCode() +
                Item.GetHashCode();
        }
        #endregion IComparable<IndexedItem<T>> Members

        #region IComparable Members

        /// <summary>
        /// Compares Index property of this instance with the Index property of specified IndexedItem 
        /// and returns an indication of their relative values.
        /// 
        /// Parameter obj must be of IndexedItem, else an ArgumentException will occur.
        /// </summary>
        /// <param name="obj">IndexedItem instance to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of index property of this instance and the 
        /// index property of specified IndexedItem.
        /// Return Value Description:
        ///       Less than zero index of this instance is less than the index of specified IndexedItem.
        ///       Zero index of this instance is equal to the index of specified IndexedItem.
        ///       Greater than zero index of this instance is greater than the index of specified IndexedItem 
        ///         or specified IndexedItem is null.
        /// </returns>
        public int CompareTo(object obj)
        {
            IndexedItem<T> other = obj as IndexedItem<T>;

            if (ReferenceEquals(other, null) && !ReferenceEquals(obj, null))
            {
                throw new ArgumentException("Resource.InvalidTypeIndexedItem");
            }

            return CompareTo(other);
        }

        #endregion
    }
}
