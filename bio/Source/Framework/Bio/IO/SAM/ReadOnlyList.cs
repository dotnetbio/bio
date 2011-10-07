using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using Bio.Algorithms.Alignment;

namespace Bio.IO.SAM
{
    /// <summary>
    /// Provides the base class for a read-only aligned sequence collection.
    /// </summary>
    public class ReadOnlyAlignedSequenceCollection : IList<IAlignedSequence>, IEnumerable<IAlignedSequence>
    {
        IList<SAMAlignedSequence> items;
        
        /// <summary>
        /// Initializes a new instance of the ReadOnlyAlignedSequenceCollection
        /// class that is a read-only wrapper around the specified list.
        /// </summary>
        /// <param name="list">The list to wrap.</param>
        public ReadOnlyAlignedSequenceCollection(IList<SAMAlignedSequence> list)
        {
            items = list;
        }

        /// <summary>
        /// Gets a value indicating whether the ReadOnlyAlignedSequenceCollection
        /// is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the ReadOnlyAlignedSequenceCollection
        ///  instance.
        /// </summary>
        public int Count
        {
            get
            {
                return items.Count;
            }
        }

        /// <summary>
        ///  Determines whether an element is in the ReadOnlyAlignedSequenceCollection.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the ReadOnlyAlignedSequenceCollection.
        /// </param>
        /// <returns>
        /// true if value is found in the ReadOnlyAlignedSequenceCollection;
        /// otherwise, false.
        /// </returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public bool Contains(IAlignedSequence item)
        {
            return IndexOf(item) >= 0;
        }

        /// <summary>
        /// Copies the entire ReadOnlyAlignedSequenceCollection to
        /// a compatible one-dimensional array, starting at the specified index
        /// of the target array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array that is the destination of the elements
        /// copied from ReadOnlyAlignedSequenceCollection. The array
        /// must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(IAlignedSequence[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(Properties.Resource.ParameterNameArray);
            }

            int index = arrayIndex;
            foreach (IAlignedSequence seq in this)
            {
                array[index++] = seq;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the ReadOnlyAlignedSequenceCollection.
        /// </summary>
        /// <returns>An enumerator for the ReadOnlyAlignedSequenceCollection.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public IEnumerator<IAlignedSequence> GetEnumerator()
        {
            return new ReadOnlyAlignedSequenceCollectionEnumerator(this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the ReadOnlyAlignedSequenceCollection.
        /// </summary>
        /// <returns>An enumerator for the ReadOnlyAlignedSequenceCollection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ReadOnlyAlignedSequenceCollectionEnumerator(this);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the
        /// first occurrence within the entire ReadOnlyAlignedSequenceCollection.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the ReadOnlyAlignedSequenceCollection. The value
        /// can be null for reference types.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of item within the entire ReadOnlyAlignedSequenceCollection,
        /// if found; otherwise, -1.
        /// </returns>
        public int IndexOf(IAlignedSequence item)
        {
            return items.IndexOf((SAMAlignedSequence)item);
        }

        #region IList<IAlignedSequence> Members

        /// <summary>
        /// This method is not supported since ReadOnlyAlignedSequenceCollection is read-only.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the ReadOnlyAlignedSequenceCollection.</param>
        public void Insert(int index, IAlignedSequence item)
        {
            throw new NotSupportedException(Properties.Resource.NotSupportedReadOnlyCollection);
        }

        /// <summary>
        /// This method is not supported since ReadOnlyAlignedSequenceCollection is read-only.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            throw new NotSupportedException(Properties.Resource.NotSupportedReadOnlyCollection);
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// Throws a NotSupportedException when attempting to set the position
        /// since VirtualAlignedSequenceList is read-only.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public IAlignedSequence this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                throw new NotSupportedException(Properties.Resource.NotSupportedReadOnlyCollection);
            }
        }

        #endregion

        #region ICollection<IAlignedSequence> Members
        /// <summary>
        /// This method is not supported since ReadOnlyAlignedSequenceCollection is read-only.
        /// </summary>
        /// <param name="item">The object to add to the ReadOnlyAlignedSequenceCollection.</param>
        public void Add(IAlignedSequence item)
        {
            throw new NotSupportedException(Properties.Resource.NotSupportedReadOnlyCollection);
        }

        /// <summary>
        /// This method is not supported since ReadOnlyAlignedSequenceCollection is read-only.
        /// </summary>
        public void Clear()
        {
            throw new NotSupportedException(Properties.Resource.NotSupportedReadOnlyCollection);
        }

        /// <summary>
        /// This method is not supported since ReadOnlyAlignedSequenceCollection is read-only.
        /// </summary>
        /// <param name="item">The object to remove from the ReadOnlyAlignedSequenceCollection.</param>
        /// <returns>true if item was successfully removed from the ReadOnlyAlignedSequenceCollection;
        /// otherwise, false. This method also returns false if item is not found in
        /// the original ReadOnlyAlignedSequenceCollection.</returns>
        public bool Remove(IAlignedSequence item)
        {
            throw new NotSupportedException(Properties.Resource.NotSupportedReadOnlyCollection);
        }

        #endregion
    }

    internal class ReadOnlyAlignedSequenceCollectionEnumerator : IEnumerator<IAlignedSequence>
    {
        #region Fields
        /// <summary>
        /// A list of sequences.
        /// </summary>
        private readonly IList<IAlignedSequence> alignedSequences;

        /// <summary>
        /// The zero-based index of the sequence in the list.
        /// </summary>
        private int index;

        /// <summary>
        /// Track whether disposed has been called.
        /// </summary>
        private bool disposed;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes an enumerator for the VirtualAlignedSequenceEnumerator.
        /// </summary>
        /// <param name="virtualAlignedSequenceList"></param>
        public ReadOnlyAlignedSequenceCollectionEnumerator(IList<IAlignedSequence> virtualAlignedSequenceList)
        {
            alignedSequences = virtualAlignedSequenceList as IList<IAlignedSequence>;
            Reset();
        }
        #endregion


        #region IEnumerator<IAlignedSequence> Members

        /// <summary>
        /// The current item reference for the enumerator.
        /// </summary>
        public IAlignedSequence Current
        {
            get
            {
                if (index < 0)
                {
                    return null;
                }

                return alignedSequences[index];
            }
        }

        #endregion

        #region IDisposable Members
        /// <summary>
        /// Disposes of any allocated memory.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of any allocated memory.
        /// </summary>
        /// <param name="disposing">Indicates whether to dispose of all resources or only unmanaged ones.</param>
        public void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // No op
                }
                disposed = true;
            }
        }

        #endregion

        #region IEnumerator Members
        /// <summary>
        /// The current item reference for the enumerator.
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                return alignedSequences[index];
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; 
        /// false if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            if (index < (alignedSequences.Count - 1))
            {
                index++;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element
        /// in the collection.
        /// </summary>
        public void Reset()
        {
            index = -1;
        }

        #endregion
    }

}
