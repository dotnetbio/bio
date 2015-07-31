using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Bio.Util;
using System.Collections;
using System.Globalization;

namespace Bio.Matrix
{
    /// <summary>
    /// A 2-D collection of values, accessable via a pair of indexes or a pair of keys.
    /// </summary>
    /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
    /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
    /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
    [Serializable]
    abstract public class Matrix<TRowKey, TColKey, TValue> : IList<IList<TValue>>
    {
        /// <summary>
        /// Gets or sets the value associated with the specified indexes.
        /// </summary>
        /// <param name="rowIndex">The index for the row of interest</param>
        /// <param name="colIndex">The index for the col of interest</param>
        /// <returns>The value associated with the specified indexes.
        /// A get operation will throw an exception if the value is missing from the matrix. (See TryGet and GetValueOrMissing.)
        /// A set operation will overwrite a missing value but will not set to the special Missing value. (See Remove and SetValueOrMissing).</returns>
        virtual public TValue this[int rowIndex, int colIndex]
        {
            get
            {
                TValue value;
                if (!TryGetValue(rowIndex, colIndex, out value))
                {
                    throw new KeyNotFoundException(string.Format("Value missing at rowIndex={0},colIndex={1}", rowIndex, colIndex));
                }
                return value;
            }
            set
            {
                if (IsMissing(value))
                {
                    throw new ArgumentException("Don't assign the 'MissingValue'. Instead, use Remove");
                }
                SetValueOrMissing(rowIndex, colIndex, value);
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified keys.
        /// </summary>
        /// <param name="rowKey">The key for the row of interest. The key must exist in RowKeys (and, thus, IndexOfRowKey)</param>
        /// <param name="colKey">The key for the col of interest. The key must exist in ColKeys (and, thus, IndexOfColKey)</param>
        /// <returns>The value associated with the specified keys.
        /// A get operation will throw an exception if the value is missing from the matrix. (See TryGet and GetValueOrMissing.)
        /// A set operation will overwrite a missing value but will not set to the special Missing value. (See Remove and SetValueOrMissing).</returns>
        virtual public TValue this[TRowKey rowKey, TColKey colKey]
        {
            get
            {
                TValue value;
                if (!TryGetValue(rowKey, colKey, out value))
                {
                    throw new KeyNotFoundException(string.Format("Value missing at rowKey={0},colKey={1}", rowKey, colKey));
                }
                return value;
            }
            set
            {
                if (IsMissing(value))
                {
                    throw new ArgumentException("Don't assign the 'MissingValue'. Instead, use Remove");
                }
                SetValueOrMissing(rowKey, colKey, value);
            }
        }

        /// <summary>
        /// Gets a row in the form of an IDictionary.
        /// </summary>
        /// <param name="rowKey">The key for the row of interest. The key must exist in RowKeys (and, thus, IndexOfRowKey)</param>
        /// <returns>An IDictionary that maps col keys to (nonmissing) values. Values in the dictionary may be read, set, or added.
        /// Values changes are reflected in the Matrix. Any values added to the dictionary must have a key that already
        /// exists in the ColKeys (and, thus, IndexOfColKeys) of the Matrix.
        /// The RowView method is at least O(log n) and typically O(1).
        /// Access in the dictionary is at least O(log n) and typically O(1).
        /// </returns>
        public IDictionary<TColKey, TValue> RowView(TRowKey rowKey)
        {
            return new RowView<TRowKey, TColKey, TValue>(this, rowKey);
        }

        /// <summary>
        /// Gets the value associated with the specified keys, if a value is not missing.
        /// </summary>
        /// <param name="rowKey">The key for the row of interest. The key must exist in RowKeys (and, thus, IndexOfRowKey)</param>
        /// <param name="colKey">The key for the col of interest. The key must exist in ColKeys (and, thus, IndexOfColKey)</param>
        /// <param name="value">When this method returns, contains the value associated with the specified keys, if the value is not missing;
        /// otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the matrix contains an element with the specified keys; otherwise, false.</returns>
        abstract public bool TryGetValue(TRowKey rowKey, TColKey colKey, out TValue value);

        /// <summary>
        /// Gets the value associated with the specified integer indexes, if a value is not missing.
        /// </summary>
        /// <param name="rowIndex">The index for the row of interest</param>
        /// <param name="colIndex">The index for the col of interest</param>
        /// <param name="value">When this method returns, contains the value associated with the specified indexes, if the value is not missing;
        /// otherwise, the missing value is returned. This parameter is passed uninitialized.</param>
        /// <returns>true if the matrix contains an element with the specified indexes; otherwise, false.</returns>
        abstract public bool TryGetValue(int rowIndex, int colIndex, out TValue value);

        /// <summary>
        /// Gets the value associated with the specified keys or the special Missing value.
        /// </summary>
        /// <param name="rowKey">The key for the row of interest. The key must exist in RowKeys (and, thus, IndexOfRowKey)</param>
        /// <param name="colKey">The key for the col of interest. The key must exist in ColKeys (and, thus, IndexOfColKey)</param>
        /// <returns>a value if the matrix contains an element with the specified keys; otherwise, the special Missing value.</returns>
        public TValue GetValueOrMissing(TRowKey rowKey, TColKey colKey)
        {
            try
            {
                TValue value;
                if (TryGetValue(rowKey, colKey, out value))
                {
                    return value;
                }
                else
                {
                    return MissingValue;
                }
            }
            catch (KeyNotFoundException)
            {
                if (!ContainsRowKey(rowKey))
                    throw new KeyNotFoundException(string.Format(CultureInfo.InvariantCulture, "Row key {0} is not in the matrix.", rowKey));
                else if (!ContainsColKey(colKey))
                    throw new KeyNotFoundException(string.Format(CultureInfo.InvariantCulture, "Column key {0} is not in the matrix.", colKey));
                else
                    throw;
            }
        }

        /// <summary>
        /// Gets the value associated with the specified indexes or the special Missing value.
        /// </summary>
        /// <param name="rowIndex">The index for the row of interest</param>
        /// <param name="colIndex">The index for the col of interest</param>
        /// <returns>a value if the matrix contains an element with the specified indexes; otherwise, the special Missing value.</returns>
        public TValue GetValueOrMissing(int rowIndex, int colIndex)
        {
            TValue value;
            if (TryGetValue(rowIndex, colIndex, out value))
            {
                return value;
            }
            else
            {
                return MissingValue;
            }
        }

        /// <summary>
        /// Set a value in the matrix or, by using the special Missing value, mark an element as missing.
        /// </summary>
        /// <param name="rowKey">The key for the row of interest. The key must exist in RowKeys (and, thus, IndexOfRowKey)</param>
        /// <param name="colKey">The key for the col of interest. The key must exist in ColKeys (and, thus, IndexOfColKey)</param>
        /// <param name="value">The value to set or the special Missing value.</param>
        abstract public void SetValueOrMissing(TRowKey rowKey, TColKey colKey, TValue value);
        //{
        //    if (IsMissing(value))
        //    {
        //        Remove(rowKey, colKey); //we don't care if it was missing before or not
        //    }
        //    else
        //    {
        //        this[rowKey, colKey] = value;
        //    }
        //}

        /// <summary>
        /// Set a value in the matrix or, by using the special Missing value, mark an element as missing.
        /// </summary>
        /// <param name="rowIndex">The index for the row of interest</param>
        /// <param name="colIndex">The index for the col of interest</param>
        /// <param name="value">The value to set or the special Missing value.</param>
        abstract public void SetValueOrMissing(int rowIndex, int colIndex, TValue value);
        //{
        //    if (MissingValue.Equals(value))
        //    {
        //        Remove(rowIndex, colIndex);//we don't care if it was missing before or not
        //    }
        //    else
        //    {
        //        this[rowIndex, colIndex] = value;
        //    }
        //}

        /// <summary>
        /// The collection of row keys. As with any collection this is a mapping from
        /// an index to a value. The collection is read-only and never changes. To get the
        /// effect of changing this list, a new matrix can be created on top of the current matrix.
        /// Access is at least O(log n) and typically O(1).
        /// </summary>
        abstract public ReadOnlyCollection<TRowKey> RowKeys { get; }

        /// <summary>
        /// The collection of col keys. As with any collection this is a mapping from
        /// an index to a value. The collection is read-only and never changes. To get the
        /// effect of changing this list, a new matrix can be created on top of the current matrix.
        /// Access is at least O(log n) and typically O(1).
        /// </summary>
        abstract public ReadOnlyCollection<TColKey> ColKeys { get; }

        /// <summary>
        /// A dictionary that maps a row index (an integer) to a row key. The
        /// dictionary is read-only and never changes. To get the
        /// effect of changing this list, a new matrix can be created on top of the current matrix.
        /// Access is at least O(log n) and typically O(1).
        /// </summary>

        abstract public IDictionary<TRowKey, int> IndexOfRowKey { get; }
        /// <summary>
        /// A dictionary that maps a col index (an integer) to a col key. The
        /// dictionary is read-only and never changes. To get the
        /// effect of changing this list, a new matrix can be created on top of the current matrix.
        /// Access is at least O(log n) and typically O(1).
        /// </summary>
        abstract public IDictionary<TColKey, int> IndexOfColKey { get; }

        /// <summary>
        /// Gets the number of keys in RowKeys. Because RowKeys never changes, this value never changes.
        /// The property is O(1).
        /// </summary>
        virtual public int RowCount
        {
            get { return RowKeys.Count; }
        }

        /// <summary>
        /// Gets the number of keys in ColKeys. Because ColKeys never changes, this value never changes.
        /// The property is O(1).
        /// </summary>
        virtual public int ColCount
        {
            get { return ColKeys.Count; }
        }

        /// <summary>
        /// Removes the value with the specified keys from the Matrix.
        /// </summary>
        /// <param name="rowKey">The key for the row of interest. The key must exist in RowKeys (and, thus, IndexOfRowKey)</param>
        /// <param name="colKey">The key for the col of interest. The key must exist in ColKeys (and, thus, IndexOfColKey)</param>
        /// <returns>true if the element is successfully found and removed; otherwise, false.
        /// This method returns false if the value is already missing from the Matrix.</returns>
        virtual public bool Remove(TRowKey rowKey, TColKey colKey)
        {
            TValue oldValue;
            if (TryGetValue(rowKey, colKey, out oldValue)) //Is there a (non-missing) value?
            {
                SetValueOrMissing(rowKey, colKey, MissingValue);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the value with the specified indexes from the Matrix.
        /// </summary>
        /// <param name="rowIndex">The index for the row of interest</param>
        /// <param name="colIndex">The index for the col of interest</param>
        /// <returns>true if the element is successfully found and removed; otherwise, false.
        /// This method returns false if the value is already missing from the Matrix.</returns>
        virtual public bool Remove(int rowIndex, int colIndex)
        {
            TValue oldValue;
            if (TryGetValue(rowIndex, colIndex, out oldValue)) //Is there a (non-missing) value?
            {
                SetValueOrMissing(rowIndex, colIndex, MissingValue);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Tells if the value is the special missing value. (Unlike 'Equals', this works even if the special missing value is null.)
        /// </summary>
        /// <param name="value">The value to test</param>
        /// <returns>true if the value is the special missing value</returns>
        virtual public bool IsMissing(TValue value)
        {
            if (null == MissingValue)
            {
                return null == value;
            }
            else
            {
                return MissingValue.Equals(value);
            }
        }

        /// <summary>
        /// Determines if the Matrix is missing a value.
        /// </summary>
        /// <param name="rowKey">The key for the row of interest. The key must exist in RowKeys (and, thus, IndexOfRowKey)</param>
        /// <param name="colKey">The key for the col of interest. The key must exist in ColKeys (and, thus, IndexOfColKey)</param>
        /// <returns>true if the matrix contains an element with the specified keys; otherwise, false.</returns>
        virtual public bool IsMissing(TRowKey rowKey, TColKey colKey)
        {
            return IsMissing(GetValueOrMissing(rowKey, colKey));
        }

        /// <summary>
        /// Determines if the Matrix is missing a value.
        /// </summary>
        /// <param name="rowIndex">The index for the row of interest</param>
        /// <param name="colIndex">The index for the col of interest</param>
        /// <returns>true if the matrix contains an element with the specified indexes; otherwise, false.</returns>
        virtual public bool IsMissing(int rowIndex, int colIndex)
        {
            return IsMissing(GetValueOrMissing(rowIndex, colIndex));
        }

        /// <summary>
        /// Determines whether the Matrix contains the specified keys.
        /// The method is at least O(log n) and typically O(1).
        /// </summary>
        /// <param name="rowKey">A row key of interest. It need not exist in RowKeys (and thus, IndexOfRowKeys)</param>
        /// <param name="colKey">A col key of interest. It need not exist in ColKeys (and thus, IndexOfColKeys)</param>
        /// <returns>true if RowKeys contrains rowKey and ColKeys contains colKey; otherwise, false.</returns>
        virtual public bool ContainsRowAndColKeys(TRowKey rowKey, TColKey colKey)
        {
            return this.ContainsRowKey(rowKey) && this.ContainsColKey(colKey);
        }

        /// <summary>
        /// Determines whether the Matrix contains the specified row key.
        /// The method is at least O(log n) and typically O(1).
        /// </summary>
        /// <param name="rowKey">A row key of interest. It need not exist in RowKeys (and thus, IndexOfRowKeys)</param>
        /// <returns>true if RowKeys contrains rowKey; otherwise, false.</returns>
        virtual public bool ContainsRowKey(TRowKey rowKey)
        {
            bool containsRowKey = IndexOfRowKey.ContainsKey(rowKey);
            return containsRowKey;
        }


        /// <summary>
        /// Determines whether the Matrix contains the specified col key.
        /// The method is at least O(log n) and typically O(1).
        /// </summary>
        /// <param name="colKey">A row key of interest. It need not exist in ColKeys (and thus, IndexOfColKeys)</param>
        /// <returns>true if ColKeys contrains colKey; otherwise, false.</returns>
        virtual public bool ContainsColKey(TColKey colKey)
        {
            return IndexOfColKey.ContainsKey(colKey);
        }

        /// <summary>
        /// The special Missing value.
        /// This property is O(1).
        /// </summary>
        abstract public TValue MissingValue { get; }

        /// <summary>
        /// Gets a row in the form of an IDictionary.
        /// </summary>
        /// <param name="rowIndex">The index for the row of interest</param>
        /// <returns>An IDictionary that maps col keys to (nonmissing) values. Values in the dictionary may be read, set, or added.
        /// Values changes are reflected in the Matrix. Any values added to the dictionary must have a key that already
        /// exists in the ColKeys (and, thus, IndexOfColKeys) of the Matrix.
        /// The RowView method is at least O(log n) and typically O(1).
        /// Access in the dictionary is at least O(log n) and typically O(1).
        /// </returns>
        public IDictionary<TColKey, TValue> RowView(int rowIndex)
        {
            return new RowView<TRowKey, TColKey, TValue>(this, RowKeys[rowIndex]);
        }

        /// <summary>
        /// Gets a col in the form of an IDictionary.
        /// </summary>
        /// <param name="colKey">The key for the col of interest. The key must exist in ColKeys (and, thus, IndexOfColKey)</param>
        /// <returns>An IDictionary that maps row keys to (nonmissing) values. Values in the dictionary may be read, set, or added.
        /// Values changes are reflected in the Matrix. Any values added to the dictionary must have a key that already
        /// exists in the RowKeys (and, thus, IndexOfRowKeys) of the Matrix.
        /// The ColView method is at least O(log n) and typically O(1).
        /// Access in the dictionary is at least O(log n) and typically O(1).
        /// </returns>
        public IDictionary<TRowKey, TValue> ColView(TColKey colKey)
        {
            return new ColView<TRowKey, TColKey, TValue>(this, colKey);
        }

        /// <summary>
        /// Gets a col in the form of an IDictionary.
        /// </summary>
        /// <param name="colIndex">The index for the col of interest</param>
        /// <returns>An IDictionary that maps row keys to (nonmissing) values. Values in the dictionary may be read, set, or added.
        /// Values changes are reflected in the Matrix. Any values added to the dictionary must have a key that already
        /// exists in the RowKeys (and, thus, IndexOfRowKeys) of the Matrix.
        /// The ColView method is at least O(log n) and typically O(1).
        /// Access in the dictionary is at least O(log n) and typically O(1).
        /// </returns>
        public IDictionary<TRowKey, TValue> ColView(int colIndex)
        {
            return new ColView<TRowKey, TColKey, TValue>(this, ColKeys[colIndex]);
        }

        /// <summary>
        /// Gets a sequence containing the nonmissing values in the Matrix.
        /// </summary>
        /// <returns>A sequence containing the nonmissing values of the Matrix.</returns>
        virtual public IEnumerable<TValue> Values
        {
            get
            {
                for (int rowIndex = 0; rowIndex < RowCount; ++rowIndex)
                {
                    TRowKey rowKey = RowKeys[rowIndex];
                    for (int colIndex = 0; colIndex < ColCount; ++colIndex)
                    {
                        TValue value;
                        if (TryGetValue(rowIndex, colIndex, out value))
                        {
                            TColKey colKey = ColKeys[colIndex];
                            yield return value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the Matrix, skipping any missing values.
        /// </summary>
        /// <returns>An enumerator for the Matrix. For purposes of enumeration, each item is a RowKeyColKeyValue structure representing nonmissing value and its keys.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        virtual public IEnumerable<RowKeyColKeyValue<TRowKey, TColKey, TValue>> RowKeyColKeyValues
        {
            get
            {
                for (int rowIndex = 0; rowIndex < RowCount; ++rowIndex)
                {
                    TRowKey rowKey = RowKeys[rowIndex];
                    for (int colIndex = 0; colIndex < ColCount; ++colIndex)
                    {
                        TValue value;
                        if (TryGetValue(rowIndex, colIndex, out value))
                        {
                            TColKey colKey = ColKeys[colIndex];
                            yield return new RowKeyColKeyValue<TRowKey, TColKey, TValue>(rowKey, colKey, value);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Returns a String that represents the Matrix as a series of lines.
        /// </summary>
        /// <returns>A string, containing newlines characters, representing the matrix.</returns>
        public string ToString2D()
        {
            StringBuilder sb = new StringBuilder();
            TextWriter textWriter = new StringWriter(sb);
            WriteDense(textWriter);
            return sb.ToString();
        }

        /// <summary>
        /// Writes the matrix to a TextWriter in dense format. The first line is "var" TAB and then the tab-delimited col keys.
        /// Next is one line per row key. Each line is the row key TAB and then the tab-limited values.
        /// Values may include the special Missing value.
        /// </summary>
        /// <param name="textWriter">The TextWriter to write to.</param>
        virtual public void WriteDense(TextWriter textWriter)
        {
            textWriter.WriteLine("var\t{0}", ColKeys.StringJoin("\t"));
            foreach (TRowKey rowKey in RowKeys)
            {
                textWriter.Write(rowKey);
                foreach (TColKey colKey in ColKeys)
                {
                    textWriter.Write("\t");
                    TValue value = GetValueOrMissing(rowKey, colKey);
                    IEnumerable asEnum = value as IEnumerable;
                    if (!(value is string) && asEnum != null)
                    {
                        textWriter.Write(asEnum.StringJoin(","));
                    }
                    else
                    {
                        textWriter.Write(value);
                    }
                }
                textWriter.WriteLine();
            }
        }

        /// <summary>
        /// Determines of the Matrix contains any missing values.
        /// May be as slow as O(rowCount * colCount)
        /// </summary>
        /// <returns>true if the matrix contains any missing values; otherwise, false.</returns>
        virtual public bool IsMissingSome()
        {
            for (int rowIndex = 0; rowIndex < RowCount; ++rowIndex)
            {
                for (int colIndex = 0; colIndex < ColCount; ++colIndex)
                {
                    TValue ignore;
                    if (!TryGetValue(rowIndex, colIndex, out ignore))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines of the Matrix is completely empty.
        /// May be as slow as O(rowCount * colCount)
        /// </summary>
        /// <returns>true if the matrix contains all missing values; otherwise, false.</returns>
        virtual public bool IsMissingAll()
        {
            return !Values.Any();
        }

        /// <summary>
        /// Determines if the Matrix contains a row missing all values.
        /// May be as slow as O(rowCount * colCount)
        /// </summary>
        /// <returns>true if the matrix contains a row missing all values; otherwise, false.</returns>
        virtual public bool IsMissingAllInSomeRow()
        {
            for (int rowIndex = 0; rowIndex < RowCount; ++rowIndex)
            {
                if (IsMissingAllInRow(rowIndex))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if the Matrix contains a column missing all values.
        /// May be as slow as O(rowCount * colCount)
        /// </summary>
        /// <returns>true if the matrix contains a column missing all values; otherwise, false.</returns>
        virtual public bool IsMissingAllInSomeCol()
        {
            for (int colIndex = 0; colIndex < ColCount; ++colIndex)
            {
                if (IsMissingAllInCol(colIndex))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if a row is missing all its values.
        /// May be as slow as O(log(rowCount) * colCount)
        /// </summary>
        /// <param name="rowKey">The key for the row of interest. The key must exist in RowKeys (and, thus, IndexOfRowKey)</param>
        /// <returns>true is the row specified is missing all its values; otherwise false.</returns>
        virtual public bool IsMissingAllInRow(TRowKey rowKey)
        {
            return !RowView(rowKey).Any();
        }

        /// <summary>
        /// Determines if a row is missing all its values.
        /// May be as slow as O(log(rowCount) * colCount)
        /// </summary>
        /// <param name="rowIndex">The index for the row of interest</param>
        /// <returns>true is the row specified is missing all its values; otherwise false.</returns>
        virtual public bool IsMissingAllInRow(int rowIndex)
        {
            return !RowView(rowIndex).Any();
        }

        /// <summary>
        /// Determines if a column is missing all its values.
        /// May be as slow as O(rowCount * log(colCount))
        /// </summary>
        /// <param name="colKey">The key for the col of interest. The key must exist in ColKeys (and, thus, IndexOfColKey)</param>
        /// <returns>true is the col specified is missing all its values; otherwise false.</returns>
        virtual public bool IsMissingAllInCol(TColKey colKey)
        {
            return !ColView(colKey).Any();
        }

        /// <summary>
        /// Determines if a column is missing all its values.
        /// May be as slow as O(rowCount * log(colCount))
        /// </summary>
        /// <param name="colIndex">The index for the col of interest</param>
        /// <returns>true is the col specified is missing all its values; otherwise false.</returns>
        virtual public bool IsMissingAllInCol(int colIndex)
        {
            return !ColView(colIndex).Any();
        }

        /// <summary>
        /// Determines whether two Matrix&lt;TRowKey,TColKey,TValue&gt; are equal. They are equal if they
        ///   0. The 2nd one is not null
        ///   3. Have the same missing values
        ///   1. have the same rowKeys, in the same order
        ///   2. have the same colKeys, in the same order
        ///   4. Have the same nonmissing values
        /// May be as slow as O(rowCount * colCount)
        /// </summary>
        /// <param name="otherMatrix">The matrix to compare to</param>
        /// <returns>true, if they are equal in terms of rowKeys, colKeys, missing and nonMissing values. Otherwise, false.</returns>
        public bool MatrixEquals(Matrix<TRowKey, TColKey, TValue> otherMatrix)
        {
            if (otherMatrix == null)
            {
                return false;
            }

            if (!IsMissing(otherMatrix.MissingValue))
            {
                return false;
            }

            if (!RowKeys.SequenceEqual(otherMatrix.RowKeys))
            {
                return false;
            }

            if (!ColKeys.SequenceEqual(otherMatrix.ColKeys))
            {
                return false;
            }


            for (int rIdx = 0; rIdx < RowCount; rIdx++)
            {
                for (int cIdx = 0; cIdx < ColCount; cIdx++)
                {
                    TValue value1 = GetValueOrMissing(rIdx, cIdx);
                    TValue value2 = otherMatrix.GetValueOrMissing(rIdx, cIdx);
                    if (value1 == null)
                    {
                        if (value2 != null)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!value1.Equals(value2))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }


        #region IList<IList<TValue>> Members

        int IList<IList<TValue>>.IndexOf(IList<TValue> item)
        {
            throw new NotImplementedException();
        }

        void IList<IList<TValue>>.Insert(int index, IList<TValue> item)
        {
            throw new NotImplementedException(); //!!!don't allow this
        }

        void IList<IList<TValue>>.RemoveAt(int index)
        {
            throw new NotImplementedException(); //!!!don't allow this
        }

        IList<TValue> IList<IList<TValue>>.this[int rowIndex]
        {
            get
            {
                return new MatrixRowAsIList<TRowKey, TColKey, TValue>(this, rowIndex);
            }
            set
            {
                throw new NotImplementedException(); //!!!don't allow this
            }
        }

        #endregion

        #region ICollection<IList<TValue>> Members

        void ICollection<IList<TValue>>.Add(IList<TValue> item)
        {
            throw new NotImplementedException(); //!!!don't allow this
        }

        void ICollection<IList<TValue>>.Clear()
        {
            throw new NotImplementedException(); //!!!don't allow this
        }

        bool ICollection<IList<TValue>>.Contains(IList<TValue> item)
        {
            throw new NotImplementedException();
        }

        void ICollection<IList<TValue>>.CopyTo(IList<TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        int ICollection<IList<TValue>>.Count
        {
            get
            {
                return RowCount;
            }
        }

        bool ICollection<IList<TValue>>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<IList<TValue>>.Remove(IList<TValue> item)
        {
            throw new NotImplementedException(); //!!!don't allow this
        }

        #endregion

        #region IEnumerable<IList<TValue>> Members

        IEnumerator<IList<TValue>> IEnumerable<IList<TValue>>.GetEnumerator()
        {
            for (int rowIndex = 0; rowIndex < RowCount; ++rowIndex)
            {
                yield return new MatrixRowAsIList<TRowKey, TColKey, TValue>(this, rowIndex);
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<IList<TValue>>)this).GetEnumerator();
            //throw new NotImplementedExceptkion();//!!Use Values() or RowKeyValueKeyValues() or IListOfILists()
        }

        #endregion


    }


    /// <summary>
    /// A structure for representing the triple of rowKey, colKey, and value.
    /// </summary>
    /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
    /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
    /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
    [Serializable]
    public struct RowKeyColKeyValue<TRowKey, TColKey, TValue>
    {
        /// <summary>
        /// Initializes a new instance of the RowKeyColKeyValue structure with the specified keys and value.
        /// </summary>
        /// <param name="rowKey">The key for the row of interest.</param>
        /// <param name="colKey">The key for the col of interest.</param>
        /// <param name="value">The value associated with these keys.</param>
        public RowKeyColKeyValue(TRowKey rowKey, TColKey colKey, TValue value)
        {
            RowKey = rowKey;
            ColKey = colKey;
            Value = value;
        }

        /// <summary>
        /// The rowKey in the triple.
        /// </summary>
        public TRowKey RowKey;
        /// <summary>
        /// The colKey in the triple.
        /// </summary>
        public TColKey ColKey;
        /// <summary>
        /// The value in the triple.
        /// </summary>
        public TValue Value;

        /// <summary>
        /// Returns a string represenation of the RowKeyColKeyValue, using the string represenations of the keys and of the value.
        /// </summary>
        /// <returns>A string representation of the RowKeyColKeyValue, which includes the string representations of the keys and value.</returns>
        public override string ToString()
        {
            return string.Format("<{0},{1},{2}>", RowKey, ColKey, Value);
        }
    }


    /// <summary>
    /// Defines a static Create method.
    /// </summary>
    static public class RowKeyColKeyValue
    {
        /// <summary>
        /// Usage:  RowKeyColKeyValue.Create(rowKey, colKey, value)
        /// </summary>
        public static RowKeyColKeyValue<TRowKey, TColKey, TValue> Create<TRowKey, TColKey, TValue>(TRowKey rowKey, TColKey colKey, TValue value)
        {
            return new RowKeyColKeyValue<TRowKey, TColKey, TValue>(rowKey, colKey, value);
        }
    }

    /// <summary>
    /// The type of functions that can create new Matrix objects. Because values are not specified, the resulting matrix
    /// is typically empty or full of default values.
    /// </summary>
    /// <typeparam name="TMatrix">The type of matrix created. Must be a subclass Matrix{TRowKey,TColKey,TValue}.</typeparam>
    /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
    /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
    /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
    /// <param name="rowKeySequence">A sequence of row keys. The items will become the RowKeys of the Matrix.</param>
    /// <param name="colKeySequence">A sequence of colKeys. The items will come the ColKeys of the Matrix.</param>
    /// <param name="missingValue">The special Missing value.</param>
    /// <returns>A new matrix.</returns>
    public delegate TMatrix MatrixFactoryDelegate<TMatrix, TRowKey, TColKey, TValue>(IEnumerable<TRowKey> rowKeySequence, IEnumerable<TColKey> colKeySequence, TValue missingValue) where TMatrix : Matrix<TRowKey, TColKey, TValue>;

    /// <summary>
    /// The exception that is thrown when the parsing of data into a Matrix fails.
    /// </summary>
    [Serializable]
    public class MatrixFormatException : FormatException
    {
        // Extending System.Exception requires a full set of four constructors
        //   public MatrixFormatException();
        //   public MatrixFormatException(string);
        //   public MatrixFormatException(string, Exception);
        //   protected MatrixFormatException(SerializationInfo, StreamingContext);

        /// <summary>
        ///     Initializes a new instance of the MatrixFormatException class.
        /// </summary>
        public MatrixFormatException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the System.FormatException class with a specified
        ///     error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MatrixFormatException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Set the exception string with the innerException.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public MatrixFormatException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if !SILVERLIGHT
        /// <summary>
        ///     Initializes a new instance of the System.SystemException class with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected MatrixFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif

        internal static void CheckCondition(bool condition, string message)
        {
            if (!condition)
            {
                throw new MatrixFormatException(message);
            }
        }
    }
}
