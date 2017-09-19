using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Globalization;

namespace Bio.Util
{
    /// <summary>
    /// A set of longs. Internally, represents this set as a sequence of ranges, for example, 1-10,333-1200,1300, so if
    /// the longs are clumpy then RangeCollection is very fast and uses very little memory.
    /// </summary>
    public class RangeCollection : ISet<long>, IXmlSerializable
    {
        private static readonly Regex RangeExpression = PlatformManager.Services.CreateCompiledRegex(@"^(?<begin>-?\d+)(-(?<last>-?\d+))?$");
        private readonly List<long> startItems;
        private readonly SortedDictionary<long, long> itemToLength;

        /// <summary>
        /// Create an new, empty, RangeCollection.
        /// </summary>
        public RangeCollection()
        {
            this.startItems = new List<long>();
            this.itemToLength = new SortedDictionary<long, long>();
        }


        /// <summary>
        /// Create an new RangeCollection containing a single long.
        /// </summary>
        /// <returns>a new RangeCollection</returns>
        public RangeCollection(long singleItem)
            : this()
        {
            Add(singleItem);
        }

        /// <summary>
        /// Create a new RangeCollection containing the longs from a sequence.
        /// </summary>
        /// <param name="itemSequence">A sequences of longs</param>
        /// <returns>a new RangeCollection</returns>
        public RangeCollection(IEnumerable<long> itemSequence)
            : this()
        {
            AddRange(itemSequence);
        }

        /// <summary>
        /// Create a new RangeCollection containing the longs from a sequence.
        /// </summary>
        /// <param name="itemSequence">A sequences of longs</param>
        /// <returns>a new RangeCollection</returns>
        public RangeCollection(IEnumerable<int> itemSequence)
            : this(itemSequence.Select(i => (long)i)) { }


        /// <summary>
        /// Create a new Range collection containing all the longs in a range (inclusive)
        /// </summary>
        /// <param name="begin">The first long to include in the RangeCollection</param>
        /// <param name="last">The last long to include in the RangeCollection</param>
        /// <returns>a new RangeCollection</returns>
        public RangeCollection(long begin, long last)
            : this()
        {
            AddRange(begin, last);
        }

        /// <summary>
        /// Parses strings of the form -10--5,-2-10,12-12 . Spaces are allowed, no other characters are.
        /// If mergeOverlappingRanges, then, for example, 2-3,4-5 is represented
        /// as 2-5. Otherwise, they're maintained as separate ranges. The only difference is in the behavior of the ToString() call.
        /// By extension, this will change how a RangeCollection is parsed into a RangeCollectionCollection using the latter's
        /// GetInstance(RangeCollection) initializer.
        /// </summary>
        /// <param name="ranges">A range or the string empty. \"empty\" will return an empty range.</param>
        /// <returns>a new RangeCollection</returns>
        public static RangeCollection Parse(string ranges)
        {
            ranges = ranges.Trim();
            RangeCollection aRangeCollection = new RangeCollection();

            aRangeCollection.InternalParse(ranges);

            return aRangeCollection;
        }

        private void InternalParse(string ranges)
        {
            if (ranges.Equals("null", StringComparison.CurrentCultureIgnoreCase) || ranges.Equals("empty", StringComparison.CurrentCultureIgnoreCase))
                return;

            AddRanges(ranges.Split(','));
        }

        /// <summary>
        /// The ranges as a sequence, for example,  1-10 then 333-1200 then 1300
        /// </summary>
        public IEnumerable<KeyValuePair<long, long>> Ranges
        {
            get
            {
                return from item in this.startItems 
                       let last = item + this.itemToLength[item] - 1 
                       select new KeyValuePair<long, long>(item, last);
            }
        }


        /// <summary>
        /// The smallest long in the RangeCollection.
        /// </summary>
        public long FirstElement
        {
            get
            {
                Helper.CheckCondition<ArgumentOutOfRangeException>(this.startItems.Count > 0, Properties.Resource.RangeCollectionIsEmpty);
                return this.startItems[0];
            }
        }

        /// <summary>
        /// The largest long in the RangeCollection
        /// </summary>
        public long LastElement
        {
            get
            {
                Helper.CheckCondition<ArgumentOutOfRangeException>(this.startItems.Count > 0, Properties.Resource.RangeCollectionIsEmpty);
                long startItem = this.startItems[this.startItems.Count - 1];
                return startItem + this.itemToLength[startItem] - 1;
            }
        }

        /// <summary>
        /// Make the set empty
        /// </summary>
        public void Clear()
        {
            this.startItems.Clear();
            this.itemToLength.Clear();
        }

        /// <summary>
        /// The number of long elements in the RangeCollection
        /// </summary>
        /// <returns>A size of the set</returns>
        public long Count()
        {
            return Count(long.MinValue, long.MaxValue);
        }

        /// <summary>
        /// The number of long elements in the RangeCollection between min and max (inclusive)
        /// </summary>
        /// <param name="min">The smallest long element to consider</param>
        /// <param name="max">The largest long element to consider</param>
        /// <returns>The number of element in between min and max (inclusive)</returns>
        public long Count(long min, long max)
        {
            long count = 0;
            foreach (long start in this.startItems)
            {
                long stop = this.itemToLength[start] + start - 1;

                // truncate start and stop around max.
                long begin = Math.Max(start, min);
                long last = Math.Min(stop, max);
                long diff = Math.Max(0, last - begin + 1);

                count += diff;
            }

            return count;

        }

        /// <summary>
        /// True if and only if two RangeCollections have exactly the same elements.
        /// </summary>
        /// <param name="obj">The object to compare to</param>
        /// <returns>true, if there the RangeCollections have the same elements; false, otherwise</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as RangeCollection);
        }

        /// <summary>
        /// Two RangeCollections with exactly the same elements will have the same hash code.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Returns the number of contiguous ranges in this collection. Useful for memory
        /// consumption debugging.
        /// </summary>
        public long RangeCount
        {
            get { return this.startItems.Count; }
        }

        /// <summary>
        /// Add the longs of one RangeCollection to this RangeCollection.
        /// </summary>
        /// <param name="rangeCollection">The RangeCollection to add</param>
        public void AddRangeCollection(RangeCollection rangeCollection)
        {
            TryAddRangeCollection(rangeCollection);
        }

        /// <summary>
        /// Add the longs of one RangeCollection to this RangeCollection.
        /// </summary>
        /// <param name="rangeCollection">The RangeCollection to add</param>
        /// <returns>true of all the elements added are new; otherwise, false</returns>
        public bool TryAddRangeCollection(RangeCollection rangeCollection)
        {
            bool allNew = true;
            foreach (KeyValuePair<long, long> startAndLast in rangeCollection.Ranges)
            {
                allNew &= TryAdd(startAndLast.Key, startAndLast.Value - startAndLast.Key + 1);
            }
            return allNew;
        }

        /// <summary>
        /// Given a sequence of strings, each of which represents a contiguous range, add all the longs in all the ranges to this RangeCollection.
        /// </summary>
        /// <param name="rangeAsStringSequence">A sequence of strings</param>
        public void AddRanges(IEnumerable<string> rangeAsStringSequence)
        {
            foreach (string rangeAsString in rangeAsStringSequence)
            {
                AddRange(rangeAsString);
            }
        }

        /// <summary>
        /// Given a contiguous range represented as a string, for example, "1-5", add all the longs in that range to this RangeCollection.
        /// </summary>
        /// <param name="rangeAsString">the range to add represented as a string</param>
        public void AddRange(string rangeAsString)
        {
            Helper.CheckCondition(RangeExpression.IsMatch(rangeAsString), () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedValidRangeString, rangeAsString));
            Match match = RangeExpression.Match(rangeAsString);
            long begin = long.Parse(match.Groups["begin"].Value);
            long last = string.IsNullOrEmpty(match.Groups["last"].Value) ? begin : long.Parse(match.Groups["last"].Value);
            AddRange(begin, last);
        }

        /// <summary>
        /// Add all the longs starting at 'begin' to 'last' (inclusive) to the RangeCollection. They may or may not already be in the RangeCollection.
        /// The number of longs added must not be more than long.MaxValue.
        /// </summary>
        /// <param name="begin">The first long to add</param>
        /// <param name="last">The last long to add</param>
        public void AddRange(long begin, long last)
        {
            Helper.CheckCondition(begin <= last, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.InvalidRangeBeginMustBeLessThanLast, begin, last));
            long length = (long)last - (long)begin + 1;
            Helper.CheckCondition(length <= long.MaxValue, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.InvalidRangeSizeOfRangeMustBeLessThanMaxValue, length));
            TryAdd(begin, (long)length);
        }

        /// <summary>
        /// The range starting at 0 (inclusive) and going to long.MaxValue (exclusive).
        /// </summary>
        static public RangeCollection MaxValue
        {
            get
            {
                return new RangeCollection(0, long.MaxValue - 1);
            }
        }

        /// <summary>
        /// Add a sequence of integers to the RangeCollection. Each may or may not already be in the RangeCollection.
        /// </summary>
        /// <param name="itemList">The sequence of longs to add</param>
        public void AddRange(IEnumerable<long> itemList)
        {
            foreach (long item in itemList)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Add a sequence of integers to the RangeCollection. Each may or may not already be in the RangeCollection.
        /// </summary>
        /// <param name="itemList">The sequence of longs to add</param>
        public void AddRange(IEnumerable<int> itemList)
        {
            if (itemList == null)
            {
                throw new ArgumentNullException("itemList");
            }
            foreach (int item in itemList)
            {
                Add(item);
            }
        }



        /// <summary>
        /// Add an long to the RangeCollection. The long may or may not already be in the RangeCollection.
        /// </summary>
        /// <param name="item">The long to add.</param>
        public void Add(long item)
        {
            TryAdd(item);
        }

        /// <summary>
        /// Add an long to the RangeCollection. An exception is thrown if the long is already in the RangeCollection.
        /// </summary>
        /// <param name="item">The long to add.</param>
        public void AddNew(long item)
        {
            bool isOK = TryAdd(item);
            Helper.CheckCondition(isOK, () => string.Format(CultureInfo.InvariantCulture, "The element {0} was already in the RangeCollection.", item));
        }


        /// <summary>
        /// Trys to add a new element to the set.
        /// </summary>
        /// <param name="item">An long to add</param>
        /// <returns>True if item was added. False if it already existed in the range.</returns>
        public bool TryAdd(long item)
        {
            return TryAdd(item, 1);
        }

        private bool TryAdd(long item, long length)
        {
            Helper.CheckCondition(length > 0, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.InvalidRangeSizeOfRangeMustBeGreaterThanZero, length));
            Debug.Assert(this.startItems.Count == this.itemToLength.Count); // real assert
            int indexOfMiss = ~this.startItems.BinarySearch(item);

            long previous, last;

            if (indexOfMiss < 0) //Hit a start
            {
                indexOfMiss = ~indexOfMiss;
                if (length <= this.itemToLength[item])
                {
                    return false;
                }
                else
                {
                    this.itemToLength[item] = length;
                    indexOfMiss++;	  // indexOfMiss should point to the following range for the remainder of this method
                    previous = item;
                    last = item + length - 1;
                }
            }
            else if (indexOfMiss == 0)
            {
                this.startItems.Insert(indexOfMiss, item);
                this.itemToLength.Add(item, length);
                previous = item;
                last = item + length - 1;
                indexOfMiss++;		  // indexOfMiss should point to the following range for the remainder of this method
                //return true;
            }
            else
            {
                previous = this.startItems[indexOfMiss - 1];
                last = previous + this.itemToLength[previous] - 1;

                if (item <= last + 1)
                {
                    long newLength = item - previous + length;
                    Debug.Assert(newLength > 0); // real assert
                    if (newLength < this.itemToLength[previous])
                    {
                        return false;
                    }
                    else
                    {
                        this.itemToLength[previous] = newLength;
                        last = previous + newLength - 1;
                    }
                }
                else // after previous range, not contiguous with previous range
                {
                    this.startItems.Insert(indexOfMiss, item);
                    this.itemToLength.Add(item, length);
                    previous = item;
                    last = item + length - 1;
                    indexOfMiss++;
                }
            }

            if (indexOfMiss == this.startItems.Count)
            {
                return true;
            }

            // collapse next range into this one
            long next = this.startItems[indexOfMiss];
            while (last >= next - 1)
            {
                long newEnd = Math.Max(last, next + this.itemToLength[next] - 1);
                this.itemToLength[previous] = newEnd - previous + 1; //ItemToLength[previous] + ItemToLength[next];
                this.itemToLength.Remove(next);
                this.startItems.RemoveAt(indexOfMiss);

                last = newEnd;
                next = indexOfMiss < this.startItems.Count ? this.startItems[indexOfMiss] : long.MaxValue;
            }

#if DEBUG
            foreach (KeyValuePair<KeyValuePair<long, long>, KeyValuePair<long, long>> previousStartAndLastAndNextStartAndLast in Neighbors(Ranges))
            {
                long previousStart = previousStartAndLastAndNextStartAndLast.Key.Key;
                long previousLast = previousStartAndLastAndNextStartAndLast.Key.Value;
                long nextStart = previousStartAndLastAndNextStartAndLast.Value.Key;
                long nextLast = previousStartAndLastAndNextStartAndLast.Value.Value;

                Debug.Assert(previousLast < nextStart);
            }
#endif

            return true;
        }

        /// <summary>
        /// Returns true if item is within the ranges of this RangeCollection.
        /// </summary>
        public bool Contains(long item)
        {
            int indexOfMiss = this.startItems.BinarySearch(item);
            if (indexOfMiss >= 0) // item is the beginning of a range
                return true;

            indexOfMiss = ~indexOfMiss;

            if (indexOfMiss == 0)   // item is before any of the ranges
                return false;

            long previous = this.startItems[indexOfMiss - 1];
            long last = previous + this.itemToLength[previous];

            return item < last; // we already know it's greater than previous...
        }


        /// <summary>
        /// Tells if all longs from start to last (inclusive) are members of this RangeCollection.
        /// </summary>
        /// <param name="start">The first long</param>
        /// <param name="last">The last long</param>
        /// <returns>true if all longs from start to last (inclusive) are members of this RangeCollection; otherwise, false</returns>
        public bool Contains(long start, long last)
        {
            if (last < start) // can't contain an empty range.
                return false;

            int indexOfMiss = this.startItems.BinarySearch(start);
            if (indexOfMiss >= 0) // start is the beginning of a range that is at least as long as last-start+1
                return start + this.itemToLength[start] - 1 >= last;

            indexOfMiss = ~indexOfMiss;

            if (indexOfMiss == 0)   // start is before any of the ranges
                return false;

            long previous = this.startItems[indexOfMiss - 1];
            long lastOfPrevious = previous + this.itemToLength[previous] - 1;

            return start <= lastOfPrevious && last <= lastOfPrevious; // we already know both start and last are greater than previous...
        }

        /// <summary>
        /// Tells if this RangeCollection is a superset of another. An an equal RangeCollection is a superset.
        /// </summary>
        /// <param name="rangeCollection">The RangeCollection that may be the subset</param>
        /// <returns>true, if this RangeCollection is a superset; otherwise, false</returns>
        public bool Contains(RangeCollection rangeCollection)
        {
            foreach (KeyValuePair<long, long> range in this.Ranges)
            {
                if (!Contains(range.Key, range.Value))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Tells if every element of the RangeCollection is between low (inclusive) and high (inclusive)
        /// </summary>
        /// <param name="low">That value that every element must be at least as large as</param>
        /// <param name="high">The value that every element must be no learger than</param>
        /// <returns>true if every element is between these two values (inclusive); otherwise, false</returns>
        public bool IsBetween(long low, long high)
        {
            bool isBetween = low <= FirstElement && LastElement <= high;
            return isBetween;
        }

        /// <summary>
        /// True if the RangeCollection contains no elements; otherwise, false.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.startItems.Count == 0;
            }
        }

        /// <summary>
        /// Returns RangeCollection as a string in the form. For example, the range collection containing 1, 2, 10, 11, and 12 returns "1-2,10-12".
        /// If the set is empty, returns "Empty"
        /// </summary>
        /// <param name="seperator1">The string that indicates a contiguous range. Usually "-"</param>
        /// <param name="separator2">The string the separates contiguous range. Usually ","</param>
        /// <returns>A string version of the range collection.</returns>
        public string ToString(string seperator1, string separator2)
        {
            if (this.IsEmpty)
            {
                return "empty";
            }
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<long, long> startAndLast in Ranges)
            {
                if (sb.Length != 0)
                {
                    sb.Append(separator2);
                }
                if (startAndLast.Key == startAndLast.Value)
                {
                    sb.Append(startAndLast.Key);
                }
                else
                {
                    sb.AppendFormat("{0}{1}{2}", startAndLast.Key, seperator1, startAndLast.Value);
                }
            }
            return sb.ToString();
        }


        /// <summary>
        /// Returns RangeCollection as a string in the form. For example, the range collection containing 1, 2, 10, 11, and 12 returns "1-2,10-12".
        /// If the set is empty, returns "Empty"
        /// </summary>
        /// <returns>A string version of the range collection.</returns>
        public override string ToString()
        {
            return ToString("-", ",");
        }


        /// <summary>
        /// Tells if the range collection includes all longs from 0 (inclusive) to itemCount-1 (inclusive)
        /// </summary>
        /// <param name="itemCount">The number of longs, starting at 0, expected</param>
        /// <returns>true if the range collection includes the itemCount longs starting at 0; otherwise, false</returns>
        public bool IsComplete(long itemCount)
        {
            return IsComplete(0, itemCount - 1);
        }


        /// <summary>
        /// Tells if the range collection contains a continuous set of longs.
        /// </summary>
        /// <returns>true if a continuous set; false if empty or if gaps.</returns>
        public bool IsContiguous()
        {
            bool b = (this.startItems.Count == 1);
            return b;
        }

        /// <summary>
        /// Tells if the range collection includes all longs from firstItem to lastItem (inclusive)
        /// </summary>
        /// <param name="firstItem">The first long of interest</param>
        /// <param name="lastItem">The last long of interest</param>
        /// <returns>true if the range collection includes all the longs between firstItem and lastItem (inclusive); otherwise, false</returns>
        public bool IsComplete(long firstItem, long lastItem)
        {
            bool b = (this.startItems.Count == 1) && (this.startItems[0] == firstItem) && (this.itemToLength[this.startItems[0]] == lastItem - firstItem + 1);
            return b;
        }

        /// <summary>
        /// Create a RangeCollection by doing a deep copy of a RangeCollection
        /// </summary>
        /// <param name="rangeCollection">A new RangeCollection</param>
        public RangeCollection(RangeCollection rangeCollection)
            : this()
        {
            this.startItems = new List<long>(rangeCollection.startItems);
            this.itemToLength = new SortedDictionary<long, long>(rangeCollection.itemToLength);
        }

        /// <summary>
        /// Returns a collection of elements at what would be the nth element for i \in rangeCollectionOfIndeces. 
        /// </summary>
        /// <param name="rangeCollectionOfIndeces">0-based indices.</param>
        public RangeCollection ElementsAt(RangeCollection rangeCollectionOfIndeces)
        {
            RangeCollection result = new RangeCollection();

            foreach (KeyValuePair<long, long> range in rangeCollectionOfIndeces.Ranges)
            {
                result.AddRangeCollection(ElementsAt(range.Key, range.Value));
            }
            return result;
        }

        /// <summary>
        /// Returns a collection of elements at what would be the i'th element for i \in [startIdx,lastIdx]. startIdx and lastIdx are 0-based.
        /// </summary>
        private RangeCollection ElementsAt(long startIndex, long lastIndex)
        {
            if (lastIndex < startIndex || startIndex < 0 || lastIndex >= Count())
            {
                throw new ArgumentOutOfRangeException(string.Format("{0}-{1} must be a non-empty range that falls between 0 and {2}", startIndex, lastIndex, Count() - 1));
            }

            RangeCollection result = new RangeCollection();

            long countSoFar = 0;
            foreach (KeyValuePair<long, long> range in Ranges)
            {
                long rangeLength = range.Value - range.Key + 1;
                if (startIndex - countSoFar < rangeLength)
                {
                    long start = range.Key + startIndex - countSoFar;
                    long lastIfContinuousRange = range.Key + lastIndex - countSoFar;
                    long last = Math.Min(range.Value, lastIfContinuousRange);   // if startIdx-lastIdx falls completely in range, then take 2nd entry.
                    result.AddRange(start, last);

                    if (lastIfContinuousRange <= range.Value) // if this range covers the remaining indeces, we're done.
                        return result;
                    else
                        startIndex = countSoFar + rangeLength;
                }

                countSoFar += rangeLength;
            }
            throw new NotImplementedException("If we get here, then there's a bug in the implementation.");
        }

        /// <summary>
        /// Returns what would be the nth element if each element were enumerated.
        /// </summary>
        /// <param name="i">0-based index.</param>
        public long ElementAt(long i)
        {
            if (i < 0 || i >= Count())
            {
                throw new ArgumentOutOfRangeException(string.Format("{0} must be between 0 and {1}", i, Count()));
            }

            long countSoFar = 0;    // to make the 0-based indexing work out, we will need to effectively include the first elt of each range among things seen thus far.
            foreach (KeyValuePair<long, long> range in Ranges)
            {
                long rangeLength = range.Value - range.Key + 1;
                //long offset = countSoFar == 0 ? 0 : countSoFar + 1;
                if (i - countSoFar < rangeLength)
                {
                    long relativeIdx = i - countSoFar;
                    return range.Key + relativeIdx;
                }
                countSoFar += rangeLength;
            }

            throw new NotImplementedException("If we get here, then the implementation is buggy.");
        }

        /// <summary>
        /// Returns the competeCollection - thisCollection
        /// </summary>
        /// <returns></returns>
        public RangeCollection Complement(long fullRangeBegin, long fullRangeLast)
        {
            RangeCollection result = new RangeCollection();

            long rangeLeftToCoverBegin = fullRangeBegin;
            foreach (KeyValuePair<long, long> range in Ranges)
            {
                long start = range.Key;
                long last = range.Value;
                if (start > rangeLeftToCoverBegin)
                {
                    result.AddRange(rangeLeftToCoverBegin, Math.Min(start - 1, fullRangeLast));
                }
                rangeLeftToCoverBegin = Math.Max(rangeLeftToCoverBegin, last + 1);
                if (rangeLeftToCoverBegin > fullRangeLast)
                {
                    break;
                }
            }
            if (rangeLeftToCoverBegin <= fullRangeLast)
            {
                result.AddRange(rangeLeftToCoverBegin, fullRangeLast);
            }
            return result;
        }

        private static IEnumerable<KeyValuePair<T, T>> Neighbors<T>(IEnumerable<T> collection)
        {
            T previous = default(T);
            bool first = true;
            foreach (T item in collection)
            {
                if (!first)
                {
                    yield return new KeyValuePair<T, T>(previous, item);
                }
                else
                {
                    first = false;
                }
                previous = item;
            }
        }

        /// <summary>
        /// Tests equality between two range collections
        /// </summary>
        /// <param name="rangeCollection">The range collection we're testing against</param>
        /// <returns>True if and only if the ranges are identical</returns>
        public bool Equals(RangeCollection rangeCollection)
        {
            if (null == rangeCollection)
            {
                return false;
            }

            if (!this.startItems.SequenceEqual(rangeCollection.startItems))
            {
                return false;
            }

            if (!this.itemToLength.Keys.SequenceEqual(rangeCollection.itemToLength.Keys))
            {
                return false;
            }

            if (!this.itemToLength.Values.SequenceEqual(rangeCollection.itemToLength.Values))
            {
                return false;
            }

            return true;
        }

        #region ISet<long> Members

        bool ISet<long>.Add(long item)
        {
            if (Contains(item))
            {
                return false;
            }
            else
            {
                Add(item);
                return true;
            }
        }

#pragma warning disable 1591
        public void ExceptWith(IEnumerable<long> other)
#pragma warning restore 1591
        {
            throw new NotImplementedException();
        }

#pragma warning disable 1591
        public void IntersectWith(IEnumerable<long> other)
#pragma warning restore 1591
        {
            throw new NotImplementedException();
        }

#pragma warning disable 1591
        public bool IsProperSubsetOf(IEnumerable<long> other)
#pragma warning restore 1591
        {
            throw new NotImplementedException();
        }

#pragma warning disable 1591
        public bool IsProperSupersetOf(IEnumerable<long> other)
#pragma warning restore 1591
        {
            throw new NotImplementedException();
        }

#pragma warning disable 1591
        public bool IsSubsetOf(IEnumerable<long> other)
#pragma warning restore 1591
        {
            throw new NotImplementedException();
        }

#pragma warning disable 1591
        public bool IsSupersetOf(IEnumerable<long> other)
#pragma warning restore 1591
        {
            throw new NotImplementedException();
        }

#pragma warning disable 1591
        public bool Overlaps(IEnumerable<long> other)
#pragma warning restore 1591
        {
            throw new NotImplementedException();
        }

#pragma warning disable 1591
        public bool SetEquals(IEnumerable<long> other)
#pragma warning restore 1591
        {
            throw new NotImplementedException();
        }

#pragma warning disable 1591
        public void SymmetricExceptWith(IEnumerable<long> other)
#pragma warning restore 1591
        {
            throw new NotImplementedException();
        }

#pragma warning disable 1591
        public void UnionWith(IEnumerable<long> other)
#pragma warning restore 1591
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICollection<long> Members

        /// <summary>
        /// Copies the contents of the range into an array.
        /// </summary>
        /// <param name="array">Destination array</param>
        /// <param name="arrayIndex">Index in destination to copy data into</param>
        public void CopyTo(long[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array", "Array cannot be null.");
            if (array.Length + arrayIndex < Count())
                throw new ArgumentException("Array will not hold all the elements.", "array");

            foreach (long item in this)
            {
                array[arrayIndex++] = item;
            }
        }

        /// <summary>
        /// Count of range items as an integer. If the count exceeds the 
        /// size storage for an integer we return zero.
        /// </summary>
        int ICollection<long>.Count
        {
            get
            {
                int count = (int) this.Count();
                return (count > 0) ? count : 0;
            }
        }

        /// <summary>
        /// Returns whether this collection is considered read-only.
        /// The Range is not changeable through ICollection.
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Remove an item from the collection. This is not supported
        /// with the RangeCollection.
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <returns>Exception</returns>
        public bool Remove(long item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<long> Members

        /// <summary>
        /// Returns an enumeration of the long elements in this RangeCollection.
        /// </summary>
        public IEnumerator<long> GetEnumerator()
        {
            foreach (KeyValuePair<long, long> range in Ranges)
            {
                for (long i = range.Key; i <= range.Value; i++)
                {
                    yield return i;
                }
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IXmlSerialiable members
        /// <summary>
        /// Required to override XmlSerialization. We use the default schema
        /// </summary>
        /// <returns></returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Overrides default XML serialization by using the ToString representation.
        /// </summary>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            string range = reader.ReadInnerXml();
            this.InternalParse(range);
        }


        /// <summary>
        /// Overrides default XML serialization by using the ToString representation.
        /// </summary>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            writer.WriteString(this.ToString());
        }
        #endregion
    }

}
