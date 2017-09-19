using System;
using System.Collections.Generic;

namespace Bio.Util
{
    /// <summary>
    /// Defines a pair in which the order of the two items are always keep sorted. This struct is hashable and IComparable based
    /// on its elements.
    /// </summary>
    /// <typeparam name="T">The type of the pair's elements</typeparam>
    public struct UOPair<T> : IEnumerable<T>, IComparable<UOPair<T>> where T : IComparable<T>
    {
        /// <summary>
        /// The first of the two sorted items.
        /// </summary>
        public T First { get; private set; }
        /// <summary>
        /// The second of the two sorted items.
        /// </summary>
        public T Second { get; private set; }


        /// <summary>
        /// Creates a new UOPair from new elements. The items may be the same and do not need to be in order. If T allows null, then null is allowed.
        /// If exactly one of e1 and e2 is null, then First will be null and Second will be non-null.
        /// The items must be IComparable{T}.
        /// </summary>
        /// <param name="e1">an element for the UOPair</param>
        /// <param name="e2">another element for the UOPair. The two elements may be the same and do not need to be in order.</param>
        /// <returns>A struct with the two items in sorted order.</returns>
        public UOPair(T e1, T e2)
            : this() //The two elements can be the same
        {

            if (null == e1)
            {
                First = e1;
                Second = e2;
            }
            else if (null == e2)
            {
                First = e2;
                Second = e1;
            }
            else
            {
                Helper.CheckCondition(e1 is IComparable<T>, () => Properties.Resource.ExpectedUoPairElementsToBeIComparable);
                if (((IComparable<T>)e1).CompareTo(e2) < 1)
                {
                    First = e1;
                    Second = e2;
                }
                else
                {
                    First = e2;
                    Second = e1;
                }
            }

        }


        /// <summary>
        /// Enumerates the pair in sorted order
        /// </summary>
        /// <returns>the elements of the pair in sorted order.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            yield return First;
            yield return Second;
        }

        /// <summary>
        /// Enumerates the pair in sorted order
        /// </summary>
        /// <returns>the elements of the pair in sorted order.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Two UOPairs are equal if their (sorted) elements are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is UOPair<T>))
            {
                return false;
            }

            UOPair<T> other = (UOPair<T>)obj;
            return First.Equals(other.First) && Second.Equals(other.Second);
        }

        // Omitting any of the following operator overloads 
        // violates rule: OverrideMethodsOnComparableTypes.
        /// <summary> </summary>
        public static bool operator ==(UOPair<T> uo1, UOPair<T> uo2)
        {
            return (uo1.Equals(uo2));
        }
        /// <summary> </summary>
        public static bool operator !=(UOPair<T> uo1, UOPair<T> uo2)
        {
            return !(uo1 == uo2);
        }
        /// <summary> </summary>
        public static bool operator <(UOPair<T> uo1, UOPair<T> uo2)
        {
            return (uo1.CompareTo(uo2) < 0);
        }
        /// <summary> </summary>
        public static bool operator >(UOPair<T> uo1, UOPair<T> uo2)
        {
            return (uo1.CompareTo(uo2) > 0);
        }

        /// <summary>
        /// True, if the elements are Equals; false, otherwise. If both elements are null, also true.
        /// </summary>
        public bool ElementsAreSame
        {
            get
            {
                if (null == First)
                {
                    return null == (object)Second;
                }
                else
                {
                    return First.Equals(Second);
                }
            }
        }

        static int uopairStringHashCode = -49838282;

        /// <summary>
        /// A hashcode such that two UOPairs{T} with the same elements will have the same hashcode.
        /// Depending on the subtypes, the hash code may be different on 32-bit and 64-bit machines
        /// </summary>
        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode() ^ uopairStringHashCode ^ typeof(T).GetHashCode();
        }

        /// <summary>
        /// Either (UO e1 e2) -- if elements differ -- or (UO 2x e)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (ElementsAreSame)
            {
                return string.Format("(UO 2x {0})", First.ToString());
            }
            else
            {
                return string.Format("(UO {0}, {1})", First.ToString(), Second.ToString());
            }
        }



        /// <summary>
        /// Compares the UOPair to the other UOPair and returns an indication of their relative values (based on the ICompare of their elements).
        /// </summary>
        /// <param name="other">the UOPair to compare to</param>
        /// <returns></returns>
        public int CompareTo(UOPair<T> other)
        {
            //If we are the same object in memory, we are equal
            if ((object)this == (object)other)
            {
                return 0;
            }

            //If our hash codes are different, sort on it
            int hashCodeComp = GetHashCode().CompareTo(other.GetHashCode());
            if (0 != hashCodeComp)
            {
                return hashCodeComp;
            }


            int compFirst = ((IComparable)First).CompareTo((IComparable)other.First);
            if (compFirst != 0)
            {
                return compFirst;
            }

            return ((IComparable)Second).CompareTo((IComparable)other.Second);
        }


    }

    /// <summary>
    /// Defines a static Create method.
    /// </summary>
    public static class UOPair
    {
        /// <summary>
        /// Usage:  UOPair.Create(val1, val2)
        /// </summary>
        public static UOPair<T> Create<T>(T t1, T t2) where T : IComparable<T>
        {
            return new UOPair<T>(t1, t2);
        }
    }
}
