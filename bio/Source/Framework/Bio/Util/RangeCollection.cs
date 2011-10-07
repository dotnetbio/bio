using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

// This is getting rather unwieldy. Maybe replace the ints with Element objects that can do some work themselves?
namespace Bio.Util
{

    /// <summary>
    /// A set of integers. Internally, represents this set as a sequence of ranges, for example, 1-10,333-1200,1300, so if
    /// the integers are clumpy then RangeCollection is very fast and uses very little memory.
    /// </summary>
    public class RangeCollection : IEnumerable<long>
    {

        private static readonly Regex _rangeExpression = new Regex(@"^(?<begin>-?\d+)(-(?<last>-?\d+))?$", RegexOptions.Compiled);
        private List<long> _startItems;
        private SortedDictionary<long, long> _itemToLength;

        /// <summary>
        /// Create an new, empty, RangeCollection.
        /// </summary>
        public RangeCollection()
        {
            _startItems = new List<long>();
            _itemToLength = new SortedDictionary<long, long>();
        }

        /// <summary>
        /// Create an new RangeCollection containing a single integer.
        /// </summary>
        /// <returns>a new RangeCollection</returns>
        public RangeCollection(long singleItem)
            : this()
        {
            Add(singleItem);
        }

        /// <summary>
        /// Create a new RangeCollection containing the integers from a sequence.
        /// </summary>
        /// <param name="itemSequence">A sequences of integers</param>
        /// <returns>a new RangeCollection</returns>
        public RangeCollection(IEnumerable<long> itemSequence)
            : this()
        {
            AddRange(itemSequence);
        }

        /// <summary>
        /// Create a new RangeCollection containing the integers from a sequence.
        /// </summary>
        /// <param name="itemSequence">A sequences of integers</param>
        /// <returns>a new RangeCollection</returns>
        public RangeCollection(IEnumerable<int> itemSequence)
            : this(itemSequence.Select(i => (long)i)) { }

        /// <summary>
        /// Create a new Range collection containing all the integers in a range (inclusive)
        /// </summary>
        /// <param name="begin">The first integer to include in the RangeCollection</param>
        /// <param name="last">The last integer to include in the RangeCollection</param>
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
        /// GetInstance(RangeCollection) initialize.
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
                foreach (long item in _startItems)
                {
                    long last = item + _itemToLength[item] - 1;
                    yield return new KeyValuePair<long, long>(item, last);
                }
            }
        }


        /// <summary>
        /// The smallest integer in the RangeCollection.
        /// </summary>
        public long FirstElement
        {
            get
            {
                return _startItems[0];
            }
        }

        /// <summary>
        /// The largest integer in the RangeCollection
        /// </summary>
        public long LastElement
        {
            get
            {
                long startItem = _startItems[_startItems.Count - 1];
                return startItem + _itemToLength[startItem] - 1;
            }
        }

        /// <summary>
        /// Make the set empty
        /// </summary>
        public void Clear()
        {
            _startItems.Clear();
            _itemToLength.Clear();
        }

        /// <summary>
        /// The number of integer elements in the RangeCollection
        /// </summary>
        /// <returns>A size of the set</returns>
        public long Count()
        {
            return Count(long.MinValue, long.MaxValue);
        }

        /// <summary>
        /// The number of integer elements in the RangeCollection between min and max (inclusive)
        /// </summary>
        /// <param name="min">The smallest integer element to consider</param>
        /// <param name="max">The largest integer element to consider</param>
        /// <returns>The number of element in between min and max (inclusive)</returns>
        public long Count(long min, long max)
        {
            long count = 0;
            foreach (long start in _startItems)
            {
                long stop = _itemToLength[start] + start - 1;

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
            get { return _startItems.Count; }
        }

        /// <summary>
        /// Add the integers of one RangeCollection to this RangeCollection.
        /// </summary>
        /// <param name="rangeCollection">The RangeCollection to add</param>
        public void AddRangeCollection(RangeCollection rangeCollection)
        {
            TryAddRangeCollection(rangeCollection);
        }

        /// <summary>
        /// Add the integers of one RangeCollection to this RangeCollection.
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
        /// Given a sequence of strings, each of which represents a contiguous range, add all the integers in all the ranges to this RangeCollection.
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
        /// Given a contiguous range represented as a string, for example, "1-5", add all the integers in that range to this RangeCollection.
        /// </summary>
        /// <param name="rangeAsString">the range to add represented as a string</param>
        public void AddRange(string rangeAsString)
        {
            Helper.CheckCondition(_rangeExpression.IsMatch(rangeAsString), Properties.Resource.ExpectedValidRangeString, rangeAsString);
            Match match = _rangeExpression.Match(rangeAsString);
            long begin = long.Parse(match.Groups["begin"].Value);
            long last = string.IsNullOrEmpty(match.Groups["last"].Value) ? begin : long.Parse(match.Groups["last"].Value);
            AddRange(begin, last);
        }

        /// <summary>
        /// Add all the integers starting at 'begin' to 'last' (inclusive) to the RangeCollection. They may or may not already be in the RangeCollection.
        /// The number of integers added must not be more than long.MaxValue.
        /// </summary>
        /// <param name="begin">The first integer to add</param>
        /// <param name="last">The last integer to add</param>
        public void AddRange(long begin, long last)
        {
            Helper.CheckCondition(begin <= last, Properties.Resource.InvalidRangeBeginMustBeLessThanLast, begin, last);
            long length = (long)last - (long)begin + 1;
            Helper.CheckCondition(length <= long.MaxValue, Properties.Resource.InvalidRangeSizeOfRangeMustBeLessThanMaxValue, length);
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
        /// <param name="itemList">The sequence of integers to add</param>
        public void AddRange(IEnumerable<long> itemList)
        {
            foreach (long item in itemList)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Add an integer to the RangeCollection. The integer may or may not already be in the RangeCollection.
        /// </summary>
        /// <param name="item">The integer to add.</param>
        public void Add(long item)
        {
            bool isOK = TryAdd(item);
        }

        /// <summary>
        /// Try to add a new element to the set.
        /// </summary>
        /// <param name="item">An integer to add</param>
        /// <returns>True if item was added. False if it already existed in the range.</returns>
        public bool TryAdd(long item)
        {
            return TryAdd(item, 1);
        }


        private bool TryAdd(long item, long length)
        {
            Helper.CheckCondition(length > 0, Properties.Resource.InvalidRangeSizeOfRangeMustBeGreaterThanZero, length);
            Debug.Assert(_startItems.Count == _itemToLength.Count); // real assert
            int indexOfMiss = ~_startItems.BinarySearch(item);

            long previous, last;

            if (indexOfMiss < 0) //Hit a start
            {
                indexOfMiss = ~indexOfMiss;
                if (length <= _itemToLength[item])
                {
                    return false;
                }
                else
                {
                    _itemToLength[item] = length;
                    indexOfMiss++;	  // indexOfMiss should point to the following range for the remainder of this method
                    previous = item;
                    last = item + length - 1;
                }
            }
            else if (indexOfMiss == 0)
            {
                _startItems.Insert(indexOfMiss, item);
                _itemToLength.Add(item, length);
                previous = item;
                last = item + length - 1;
                indexOfMiss++;		  // indexOfMiss should point to the following range for the remainder of this method
                //return true;
            }
            else
            {
                previous = _startItems[indexOfMiss - 1];
                last = previous + _itemToLength[previous] - 1;

                if (item <= last + 1)
                {
                    long newLength = item - previous + length;
                    Debug.Assert(newLength > 0); // real assert
                    if (newLength < _itemToLength[previous])
                    {
                        return false;
                    }
                    else
                    {
                        _itemToLength[previous] = newLength;
                        last = previous + newLength - 1;
                    }
                }
                else // after previous range, not contiguous with previous range
                {
                    _startItems.Insert(indexOfMiss, item);
                    _itemToLength.Add(item, length);
                    previous = item;
                    last = item + length - 1;
                    indexOfMiss++;
                }
            }

            if (indexOfMiss == _startItems.Count)
            {
                return true;
            }

            // collapse next range into this one
            long next = _startItems[indexOfMiss];
            while (last >= next - 1)
            {
                long newEnd = Math.Max(last, next + _itemToLength[next] - 1);
                _itemToLength[previous] = newEnd - previous + 1;
                _itemToLength.Remove(next);
                _startItems.RemoveAt(indexOfMiss);

                last = newEnd;
                next = indexOfMiss < _startItems.Count ? _startItems[indexOfMiss] : long.MaxValue;
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
        /// Returns true iff item is within the ranges of this RangeCollection.
        /// </summary>
        public bool Contains(long item)
        {
            int indexOfMiss = _startItems.BinarySearch(item);
            if (indexOfMiss >= 0) // item is the beginning of a range
                return true;

            indexOfMiss = ~indexOfMiss;

            if (indexOfMiss == 0)   // item is before any of the ranges
                return false;

            long previous = _startItems[indexOfMiss - 1];
            long last = previous + _itemToLength[previous];

            return item < last; // we already know it's greater than previous...
        }


        /// <summary>
        /// Tells if all integers from start to last (inclusive) are members of this RangeCollection.
        /// </summary>
        /// <param name="start">The first integer</param>
        /// <param name="last">The last integer</param>
        /// <returns>true if all integers from start to last (inclusive) are members of this RangeCollection; otherwise, false</returns>
        public bool Contains(long start, long last)
        {
            if (last < start) // can't contain an empty range.
                return false;

            int indexOfMiss = _startItems.BinarySearch(start);
            if (indexOfMiss >= 0) // start is the beginning of a range that is at least as long as last-start+1
                return start + _itemToLength[start] - 1 >= last;

            indexOfMiss = ~indexOfMiss;

            if (indexOfMiss == 0)   // start is before any of the ranges
                return false;

            long previous = _startItems[indexOfMiss - 1];
            long lastOfPrevious = previous + _itemToLength[previous] - 1;

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
                return _startItems.Count == 0;
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
        /// Tells if the range collection includes all integers from 0 (inclusive) to itemCount-1 (inclusive)
        /// </summary>
        /// <param name="itemCount">The number of integers, starting at 0, expected</param>
        /// <returns>true if the range collection includes the itemCount integers starting at 0; otherwise, false</returns>
        public bool IsComplete(long itemCount)
        {
            return IsComplete(0, itemCount - 1);
        }


        /// <summary>
        /// Tells if the range collection contains a continuous set of integers.
        /// </summary>
        /// <returns>true if a continuous set; false if empty or if gaps.</returns>
        public bool IsContiguous()
        {
            bool b = (_startItems.Count == 1);
            return b;
        }



        /// <summary>
        /// Tells if the range collection includes all integers from firstItem to lastItem (inclusive)
        /// </summary>
        /// <param name="firstItem">The first integer of interest</param>
        /// <param name="lastItem">The last integer of interest</param>
        /// <returns>true if the range collection includes all the integers between firstItem and lastItem (inclusive); otherwise, false</returns>
        public bool IsComplete(long firstItem, long lastItem)
        {
            Helper.CheckCondition(IsEmpty || LastElement <= lastItem, Properties.Resource.ShouldNotBeSmaller);
            bool b = (_startItems.Count == 1) && (_startItems[0] == firstItem) && (_itemToLength[_startItems[0]] == lastItem - firstItem + 1);
            return b;
        }

        /// <summary>
        /// Unit tests
        /// </summary>
        static public void Test()
        {
            RangeCollection aRangeCollection = new RangeCollection();
            aRangeCollection.Add(0);
            Helper.CheckCondition("0" == aRangeCollection.ToString());
            aRangeCollection.Add(1);
            Helper.CheckCondition("0-1" == aRangeCollection.ToString());
            aRangeCollection.Add(4);
            Helper.CheckCondition("0-1,4" == aRangeCollection.ToString());
            aRangeCollection.Add(5);
            Helper.CheckCondition("0-1,4-5" == aRangeCollection.ToString());
            aRangeCollection.Add(7);
            Helper.CheckCondition("0-1,4-5,7" == aRangeCollection.ToString());
            aRangeCollection.Add(2);
            Helper.CheckCondition("0-2,4-5,7" == aRangeCollection.ToString());
            aRangeCollection.Add(3);
            Helper.CheckCondition("0-5,7" == aRangeCollection.ToString());
            aRangeCollection.Add(6);
            Helper.CheckCondition("0-7" == aRangeCollection.ToString());
            aRangeCollection.Add(-10);
            Helper.CheckCondition("-10,0-7" == aRangeCollection.ToString());
            aRangeCollection.Add(-5);
            Helper.CheckCondition("-10,-5,0-7" == aRangeCollection.ToString());

            string range = "-10--5,-3,-2-1,1-5,7-12,13-15,14-16,20-25,22-23";
            aRangeCollection = RangeCollection.Parse(range);
            Console.WriteLine(range);
            Console.WriteLine(aRangeCollection);


            range = "1-5,0,4-10,-10--5,-12--3,15-20,12-21,-13";
            aRangeCollection = RangeCollection.Parse(range);

            Console.WriteLine(range);
            Console.WriteLine(aRangeCollection);

            Console.WriteLine("Count: " + aRangeCollection.Count());
            Console.WriteLine("Count -5 to 2: " + aRangeCollection.Count(-5, 2));
        }


        /// <summary>
        /// Create a RangeCollection by doing a deep copy of a RangeCollection
        /// </summary>
        /// <param name="rangeCollection">A new RangeCollection</param>
        public RangeCollection(RangeCollection rangeCollection)
            : this()
        {
            _startItems = new List<long>(rangeCollection._startItems);
            _itemToLength = new SortedDictionary<long, long>(rangeCollection._itemToLength);
        }

        /// <summary>
        /// Returns a collection of elements at what would be the i'th element for i \in rangeCollectionOfIndeces. 
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
        /// Returns what would be the i'th element if each element were enumerated.
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
        /// <returns>Range Collection.</returns>
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

        /// <summary>
        /// Neighbors.
        /// </summary>
        /// <typeparam name="T">Generic type.</typeparam>
        /// <param name="collection">Generic collection.</param>
        /// <returns>Returns collection of KeyValuePair of generic types. </returns>
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

            if (!_startItems.SequenceEqual(rangeCollection._startItems))
            {
                return false;
            }

            if (!_itemToLength.Keys.SequenceEqual(rangeCollection._itemToLength.Keys))
            {
                return false;
            }

            if (!_itemToLength.Values.SequenceEqual(rangeCollection._itemToLength.Values))
            {
                return false;
            }

            return true;
        }

        #region IEnumerable<long> Members

        /// <summary>
        /// Returns an enumeration of the integer elements in this RangeCollection.
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
        /// <summary>
        /// Enumeration of the elements
        /// </summary>
        /// <returns>Returns an enumeration of the integer elements.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Just enumerate the Elements.
        /// </summary>
        [Obsolete("Just enumerate the object (which calls .GetEnumerator<long>())")]
        public IEnumerable<long> Elements
        {
            get
            {
                foreach (KeyValuePair<long, long> range in Ranges)
                {
                    for (long i = range.Key; i <= range.Value; i++)
                    {
                        yield return i;
                    }
                }
            }
        }
    }

}
