using System;
using System.Collections.ObjectModel;
using Bio.Util;
using System.Collections.Generic;

namespace Bio.Matrix
{
    /// <summary>
    /// Creates a view on a matrix in which values converted one-to-one. For example, Suppose you have a matrix with
    /// char values such as '0', '1', '2' (with '?' for missing) and you need doubles (with double.NaN as missing).
    /// With this class you can wrap your original matrix, making it act like the matrix you need.
    /// </summary>
    /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
    /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
    /// <typeparam name="TValueView">The type of desired values.</typeparam>
    /// <typeparam name="TValueParent">The type of the current values.</typeparam>
    public class ConvertValueView<TRowKey, TColKey, TValueView, TValueParent> : Matrix<TRowKey, TColKey, TValueView>
    {

        /// <summary>
        /// Get the original matrix that this view wraps.
        /// </summary>
        public Matrix<TRowKey, TColKey, TValueParent> ParentMatrix { get; internal set; }

        /// <summary>
        /// Parameterless constructor. Don't use without setting the member variables with, for example, the SetUp method
        /// </summary>
        protected internal ConvertValueView()
        {
        }

        private TValueView missingValue;

        /// <summary>
        /// A function that converts a value of the wrapped matrix into a value of the wrapping matrix.
        /// </summary>
        public Func<TValueParent, TValueView> ParentValueToViewValue { get; private set; }
        /// <summary>
        /// A function that converts value of the wrapped matrix into a value of the wrapped matrix.
        /// </summary>
        public Func<TValueView, TValueParent> ViewValueToParentValue { get; private set; }

        /// <summary>
        /// Set all important member variables after using the parameterless constructor.
        /// </summary>
        /// <param name="parentMatrix">parent Matrix.</param>
        /// <param name="converter">The converter.</param>
        /// <param name="missingValue">The missingValue.</param>
        internal protected void SetUp(Matrix<TRowKey, TColKey, TValueParent> parentMatrix, ValueConverter<TValueParent, TValueView> converter, TValueView missingValue)
        {
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            ParentMatrix = parentMatrix;
            this.missingValue = missingValue;
            ParentValueToViewValue = converter.ConvertForward;
            ViewValueToParentValue = converter.ConvertBackward;
        }

#pragma warning disable 1591
        public override ReadOnlyCollection<TRowKey> RowKeys
#pragma warning restore 1591
        {
            get { return ParentMatrix.RowKeys; }
        }

#pragma warning disable 1591
        public override ReadOnlyCollection<TColKey> ColKeys
#pragma warning restore 1591
        {
            get { return ParentMatrix.ColKeys; }
        }

#pragma warning disable 1591
        public override IDictionary<TRowKey, int> IndexOfRowKey
#pragma warning restore 1591
        {
            get { return ParentMatrix.IndexOfRowKey; }
        }

#pragma warning disable 1591
        public override IDictionary<TColKey, int> IndexOfColKey
#pragma warning restore 1591
        {
            get { return ParentMatrix.IndexOfColKey; }
        }

#pragma warning disable 1591
        public override bool TryGetValue(TRowKey rowKey, TColKey colKey, out TValueView value)
#pragma warning restore 1591
        {
            TValueParent valueParent;
            if (ParentMatrix.TryGetValue(rowKey, colKey, out valueParent))
            {
                value = ParentValueToViewValue(valueParent);
                Helper.CheckCondition(!IsMissing(value), Properties.Resource.MayNotConvert);
                return true;
            }
            else
            {
                value = MissingValue;
                return false;
            }
        }

#pragma warning disable 1591
        public override bool TryGetValue(int rowIndex, int colIndex, out TValueView value)
#pragma warning restore 1591
        {
            TValueParent valueParent;
            if (ParentMatrix.TryGetValue(rowIndex, colIndex, out valueParent))
            {
                value = ParentValueToViewValue(valueParent);
                Helper.CheckCondition(!IsMissing(value), Properties.Resource.MayNotConvert);
                return true;
            }
            else
            {
                value = MissingValue;
                return false;
            }
        }

#pragma warning disable 1591
        public override TValueView MissingValue
#pragma warning restore 1591
        {
            get { return this.missingValue; }
        }

#pragma warning disable 1591
        public override void SetValueOrMissing(TRowKey rowKey, TColKey colKey, TValueView value)
#pragma warning restore 1591
        {
            ParentMatrix.SetValueOrMissing(rowKey, colKey, ViewValueOrMissingToParentValueOrMissing(value));
        }

#pragma warning disable 1591
        public override void SetValueOrMissing(int rowIndex, int colIndex, TValueView value)
#pragma warning restore 1591
        {
            ParentMatrix.SetValueOrMissing(rowIndex, colIndex, ViewValueOrMissingToParentValueOrMissing(value));
        }

        private TValueParent ViewValueOrMissingToParentValueOrMissing(TValueView value)
        {
            if (IsMissing(value))
            {
                return ParentMatrix.MissingValue;
            }
            else
            {
                TValueParent valueParent = ViewValueToParentValue(value);
                Helper.CheckCondition(!ParentMatrix.IsMissing(valueParent), Properties.Resource.MayNotConvert);
                return valueParent;
            }
        }

    }
}
