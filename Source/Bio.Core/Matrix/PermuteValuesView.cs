using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Bio.Util;

namespace Bio.Matrix
{

    /// <summary>
    /// A wrapper around a parent matrix that permutes the values by column.
    /// e.g. Before m1=
    ///          cid0 cid1 cid2
    ///     v0    a     b   c
    ///     v1    d    null f
    ///     v2    null null i
    ///     
    ///     m1.PermuteValuesView(1, 2, 0) =
    ///          cid0 cid1 cid2
    ///     v0    b    c    a
    ///     v1    null f   d
    ///     v2    null i   null
    /// 
    /// 
    /// Because it is a view, any changes made to the values of this matrix or the parent matrix are reflected in both.
    /// </summary>
    /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
    /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
    /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>

    // Currently permutes by column, but could be expected to also permute by row or both.
    // Currently requires a full permutation, generalize to selection, too????
    public class PermuteValuesView<TRowKey, TColKey, TValue> : Matrix<TRowKey, TColKey, TValue>
    {
        internal PermuteValuesView()
        {
        }

        /// <summary>
        /// The matrix that this view wraps.
        /// </summary>
        public Matrix<TRowKey, TColKey, TValue> ParentMatrix { get; internal set; }



#pragma warning disable 1591
        override public ReadOnlyCollection<TRowKey> RowKeys { get { return ParentMatrix.RowKeys; } }
#pragma warning restore 1591
#pragma warning disable 1591
        override public ReadOnlyCollection<TColKey> ColKeys { get { return ParentMatrix.ColKeys; } }
#pragma warning restore 1591
#pragma warning disable 1591
        override public IDictionary<TRowKey, int> IndexOfRowKey { get { return ParentMatrix.IndexOfRowKey; } }
#pragma warning restore 1591
#pragma warning disable 1591
        override public IDictionary<TColKey, int> IndexOfColKey { get { return ParentMatrix.IndexOfColKey; } }
#pragma warning restore 1591

        /// <summary>
        /// Given a colIndex value tells the cooresponding colIndex in the parent matrix.
        /// </summary>
        public IList<int> IndexOfParentCol
        {
            get { return _indexOfParentCol; }
        }
        private IList<int> _indexOfParentCol;


        internal void SetUp(Matrix<TRowKey, TColKey, TValue> parentMatrix, IEnumerable<int> colIndexSequence)
        {
            ParentMatrix = parentMatrix;

            //Check that there are not any missing or extra column indexes
            _indexOfParentCol = colIndexSequence.ToList();

            Helper.CheckCondition(IndexOfParentCol.Count == ParentMatrix.ColCount, () => Properties.Resource.ExpectedEveryColumnToBeAMemberOfThePermutation);
            RangeCollection rangeCollection = new RangeCollection(colIndexSequence);
            Helper.CheckCondition(rangeCollection.IsComplete(0, ParentMatrix.ColCount - 1), () => Properties.Resource.ExpectedEveryColumnToBeUsedOnceInThePermuation);
        }

#pragma warning disable 1591
        public override bool IsMissing(TRowKey rowKey, TColKey colKey)
#pragma warning restore 1591
        {
            return ParentMatrix.IsMissing(rowKey, colKey);
        }

#pragma warning disable 1591
        public override bool TryGetValue(TRowKey rowKey, TColKey colKey, out TValue value)
#pragma warning restore 1591
        {
            return TryGetValue(IndexOfRowKey[rowKey], IndexOfColKey[colKey], out value);
        }

#pragma warning disable 1591
        public override bool TryGetValue(int rowIndex, int colIndex, out TValue value)
#pragma warning restore 1591
        {
            return ParentMatrix.TryGetValue(rowIndex, IndexOfParentCol[colIndex], out value);
        }

#pragma warning disable 1591
        public override bool Remove(TRowKey rowKey, TColKey colKey)
#pragma warning restore 1591
        {
            return Remove(IndexOfRowKey[rowKey], IndexOfColKey[colKey]);
        }

#pragma warning disable 1591
        public override void SetValueOrMissing(int rowIndex, int colIndex, TValue value)
#pragma warning restore 1591
        {
            ParentMatrix.SetValueOrMissing(rowIndex, IndexOfParentCol[colIndex], value);
        }

#pragma warning disable 1591
        public override void SetValueOrMissing(TRowKey rowKey, TColKey colKey, TValue value)
#pragma warning restore 1591
        {
            SetValueOrMissing(IndexOfRowKey[rowKey], IndexOfColKey[colKey], value);
        }

#pragma warning disable 1591
        public override TValue MissingValue
#pragma warning restore 1591
        {
            get { return ParentMatrix.MissingValue; }
        }
    }
}
