using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bio.Matrix;
using Bio.Util;
using NUnit.Framework;

namespace Bio.Tests.Matrix
{
    /// <summary>
    /// Unit tests on many kinds of matrices.
    /// </summary>
    [TestFixture]
    public class MatrixUnitTests
    {
        /// <summary>
        /// Unit tests on matrices.
        /// </summary>
        [Test]
        public void MatrixTest()
        {
            MainTest(true, new ParallelOptions());
        }

        /// <summary>
        /// Preforms unit tests related to setting and reading value on all the built-in matrix types.
        /// </summary>
        /// <param name="doOutOfRangeTest">If true performs a test should throw a caught exception.</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        public static void MainTest(bool doOutOfRangeTest, ParallelOptions parallelOptions)
        {
            //densematrix
            TestByKeysAndIndexes(() => CreateModelMatrix().ToDenseMatrix(), doOutOfRangeTest);

            //sparsematrix
            TestByKeysAndIndexes(() => CreateModelMatrix().ToSparseMatrix(), doOutOfRangeTest);

            //TransposeView
            TestByKeysAndIndexes(() => CreateModelMatrix().TransposeView().ToDenseMatrix().TransposeView(), doOutOfRangeTest);

            //ConvertValueView
            TestByKeysAndIndexes(() => CreateModelMatrix().ConvertValueView(ValueConverter.DoubleToInt, int.MaxValue).ToDenseMatrix().ConvertValueView(ValueConverter.IntToDouble, double.NaN), doOutOfRangeTest);

            //SelectRowsAndColsView
            TestByKeysAndIndexes(() => CreateModelMatrix().SelectRowsAndColsView(new int[] { 2, 1, 0 }, new int[] { 2, 1, 0 }).ToDenseMatrix().SelectRowsAndColsView(new int[] { 2, 1, 0 }, new int[] { 2, 1, 0 }), doOutOfRangeTest);

            //RenameColsView
            TestByKeysAndIndexes(() => CreateModelMatrix().RenameColsView(new Dictionary<string, string> { { "x", "X" }, { "y", "Y" }, { "z", "Z" } }).ToDenseMatrix().RenameColsView(new Dictionary<string, string> { { "X", "x" }, { "Y", "y" }, { "Z", "z" } }), doOutOfRangeTest);

            //PermuteColValuesForEachRowView
            TestByKeysAndIndexes(() => CreateModelMatrix().PermuteColValuesForEachRowView(new int[] { 2, 1, 0 }).ToDenseMatrix().PermuteColValuesForEachRowView(new int[] { 2, 1, 0 }), doOutOfRangeTest);

            //MergeColsView
            TestByKeysAndIndexes(() => CreateModelMatrix().SelectColsView(0).ToDenseMatrix().MergeColsView(/*rowsMustMatch*/true, CreateModelMatrix().SelectColsView(1, 2).ToDenseMatrix()), doOutOfRangeTest);

            //MergeRowsView
            TestByKeysAndIndexes(() => CreateModelMatrix().SelectRowsView(0).ToDenseMatrix().MergeRowsView(/*colsMustMatch*/true, CreateModelMatrix().SelectRowsView(1, 2).ToDenseMatrix()), doOutOfRangeTest);

            //rowkeyspaddeddouble
            TestByKeysAndIndexes(() => CreateModelMatrix().ToPaddedDouble(parallelOptions), doOutOfRangeTest);

            //rowkeysansi
            TestByKeysAndIndexes(() => CreateModelMatrix().ConvertValueView(ValueConverter.DoubleToChar, '?').ToDenseAnsi(parallelOptions).ConvertValueView(ValueConverter.CharToDouble, double.NaN), doOutOfRangeTest);

            //densepairansi
            ValueConverter<double, UOPair<char>> doubleToUOPairConvert = new ValueConverter<double, UOPair<char>>(
                    r => new UOPair<char>(((int)r).ToString((IFormatProvider)null)[0], ((int)r).ToString((IFormatProvider)null)[0]),
                    pair => double.Parse(pair.First.ToString((IFormatProvider)null), (IFormatProvider)null));
            TestByKeysAndIndexes(() => CreateModelMatrix().ConvertValueView(doubleToUOPairConvert, DensePairAnsi.StaticMissingValue).ToDensePairAnsi(parallelOptions).ConvertValueView(doubleToUOPairConvert.Inverted, double.NaN), doOutOfRangeTest);



            //RowKeysPaddedDouble
            string paddedDoubleFile = Path.GetTempFileName();
            CreateModelMatrix().WritePaddedDouble(paddedDoubleFile, parallelOptions);
            using (RowKeysPaddedDouble rowKeysPaddedDouble = RowKeysPaddedDouble.GetInstanceFromPaddedDouble(paddedDoubleFile, parallelOptions, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                TestByKeys(rowKeysPaddedDouble, doOutOfRangeTest);
            }
            CreateModelMatrix().WritePaddedDouble(paddedDoubleFile, parallelOptions);
            using (RowKeysPaddedDouble rowKeysPaddedDouble = RowKeysPaddedDouble.GetInstanceFromPaddedDouble(paddedDoubleFile, parallelOptions, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                TestByIndexes(rowKeysPaddedDouble, doOutOfRangeTest);
            }
            File.Delete(paddedDoubleFile);

            //RowKeysRowKeysAnsi
            string rowKeysAnsiFile = Path.GetTempFileName();
            CreateModelMatrix().WriteDenseAnsi(rowKeysAnsiFile, parallelOptions);
            using (RowKeysAnsi rowKeysRowKeysAnsi = RowKeysAnsi.GetInstanceFromDenseAnsi(rowKeysAnsiFile, parallelOptions, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                TestByKeys(rowKeysRowKeysAnsi.ConvertValueView(ValueConverter.CharToDouble, double.NaN), doOutOfRangeTest);
            }
            CreateModelMatrix().WriteDenseAnsi(rowKeysAnsiFile, parallelOptions);
            using (RowKeysAnsi rowKeysRowKeysAnsi = RowKeysAnsi.GetInstanceFromDenseAnsi(rowKeysAnsiFile, parallelOptions, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                TestByIndexes(rowKeysRowKeysAnsi.ConvertValueView(ValueConverter.CharToDouble, double.NaN), doOutOfRangeTest);
            }
            File.Delete(rowKeysAnsiFile);

            //RowKeysRowKeysPairAnsi
            string rowKeysPairAnsiFile = Path.GetTempFileName();
            CreateModelMatrix().ConvertValueView(doubleToUOPairConvert, DensePairAnsi.StaticMissingValue).WriteDensePairAnsi(rowKeysPairAnsiFile, parallelOptions);
            using (RowKeysPairAnsi rowKeysAnsiPair = RowKeysPairAnsi.GetInstanceFromPairAnsi(rowKeysPairAnsiFile, parallelOptions, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                TestByKeys(rowKeysAnsiPair.ConvertValueView(doubleToUOPairConvert.Inverted, double.NaN), doOutOfRangeTest);
            }
            CreateModelMatrix().ConvertValueView(doubleToUOPairConvert, DensePairAnsi.StaticMissingValue).WriteDensePairAnsi(rowKeysPairAnsiFile, parallelOptions);
            using (RowKeysPairAnsi rowKeysRowKeysAnsi = RowKeysPairAnsi.GetInstanceFromPairAnsi(rowKeysPairAnsiFile, parallelOptions, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                TestByIndexes(rowKeysRowKeysAnsi.ConvertValueView(doubleToUOPairConvert.Inverted, double.NaN), doOutOfRangeTest);
            }
            File.Delete(rowKeysPairAnsiFile);

        }

        /// <summary>
        /// Create a new instance of the matrix that the tests expect for input.
        /// </summary>
        /// <returns>A new instance of the test matrix.</returns>
        static public Matrix<string, string, double> CreateModelMatrix()
        {
            return new DenseMatrix<string, string, double>(new double[,] { { 1, 2, 3 }, { 4, double.NaN, 5 }, { double.NaN, double.NaN, double.NaN } }, new string[] { "A", "B", "C" }, new string[] { "X", "Y", "Z" }, double.NaN);
        }

        /// <summary>
        /// Test the matrix created by the matrixCreator.
        /// </summary>
        /// <param name="matrixCreator">A function that creates a new test matrix.</param>
        /// <param name="doOutOfRangeTest">If true, does a test that throws and catches an exception related to being out of bounds.</param>
        public static void TestByKeysAndIndexes(Func<Matrix<string, string, double>> matrixCreator, bool doOutOfRangeTest)
        {
            if (null == matrixCreator)
            {
                throw new ArgumentNullException("matrixCreator");
            }
            TestByKeys(matrixCreator(), doOutOfRangeTest);
            TestByIndexes(matrixCreator(), doOutOfRangeTest);
        }

        /// <summary>
        /// Do a series of unit tests in which the matrix is access by rowKey and colKey
        /// </summary>
        /// <param name="matrix">The matrix to test. It should have the values of the matrix from CreateModelMatrix()</param>
        /// <param name="doOutOfRangeTest">If true, does a test that throws and catches an exception related to being out of bounds.</param>
        public static void TestByKeys(Matrix<string, string, double> matrix, bool doOutOfRangeTest)
        {
            //Loop for index and keys
            //Get the missing value

            //TryGetValue - true
            double valueAX;
            if (null == matrix)
            {
                throw new ArgumentNullException("matrix");
            }

            Helper.CheckCondition(matrix.TryGetValue("A", "X", out valueAX) && valueAX == 1);
            //TryGetValue - false
            double valueCZ;
            Helper.CheckCondition(!matrix.TryGetValue("C", "Z", out valueCZ) && matrix.IsMissing(valueCZ));


            //TryGetValue that is not in range should throw some exception
            if (doOutOfRangeTest)
            {
                try
                {
                    double valueDW;
                    matrix.TryGetValue("D", "W", out valueDW);
                    Helper.CheckCondition(false);
                }
                catch (Exception)
                {
                }
            }



            //SetValueOrMissing - value -> value
            matrix.SetValueOrMissing("A", "X", 6);
            Helper.CheckCondition(matrix.TryGetValue("A", "X", out valueAX) && valueAX == 6);
            //SetValueOrMissing - value -> missing
            matrix.SetValueOrMissing("A", "X", double.NaN);
            Helper.CheckCondition(!matrix.TryGetValue("A", "X", out valueAX));
            //SetValueOrMissing - missing -> value
            matrix.SetValueOrMissing("A", "X", 7);
            Helper.CheckCondition(matrix.TryGetValue("A", "X", out valueAX) && valueAX == 7);
            //SetValueOrMissing - missing -> missing
            matrix.SetValueOrMissing("C", "Z", double.NaN);
            Helper.CheckCondition(!matrix.TryGetValue("C", "Z", out valueCZ));
            //Remove - true
            Helper.CheckCondition(matrix.Remove("A", "X"));
            //Remove - false
            Helper.CheckCondition(!matrix.Remove("A", "X"));
            //this get value
            Helper.CheckCondition(2 == matrix["A", "Y"]);
            //this set to value
            matrix["A", "Y"] = 8;
            Helper.CheckCondition(8 == matrix["A", "Y"]);
            //this get missing - expect error
            try
            {
                double valueCX = matrix["C", "X"];
                Helper.CheckCondition(false);
            }
            catch (Exception)
            {
            }

            //this set to missing - expect error
            try
            {
                matrix["A", "X"] = double.NaN;
                Helper.CheckCondition(false);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Do a series of unit tests in which the matrix is access by rowIndex and colIndex.
        /// </summary>
        /// <param name="matrix">The matrix to test. It should have the values of the matrix from CreateModelMatrix()</param>
        /// <param name="doOutOfRangeTest">If true, does a test that throws and catches an exception related to being out of bounds.</param>
        public static void TestByIndexes(Matrix<string, string, double> matrix, bool doOutOfRangeTest)
        {
            //Loop for index and keys
            //Get the missing value

            //TryGetValue - true
            double valueAX;
            if (null == matrix)
            {
                throw new ArgumentNullException("matrix");
            }
            Helper.CheckCondition(matrix.TryGetValue(0, 0, out valueAX) && valueAX == 1);
            //TryGetValue - false
            double valueCZ;
            Helper.CheckCondition(!matrix.TryGetValue(2, 2, out valueCZ) && matrix.IsMissing(valueCZ));


            //TryGetValue that is not in range should throw some exception
            if (doOutOfRangeTest)
            {
                try
                {
                    double valueDW;
                    matrix.TryGetValue(3, 3, out valueDW);
                    Helper.CheckCondition(false);
                }
                catch (Exception)
                {
                }
            }

            //SetValueOrMissing - value -> value
            matrix.SetValueOrMissing(0, 0, 6);
            Helper.CheckCondition(matrix.TryGetValue(0, 0, out valueAX) && valueAX == 6);
            //SetValueOrMissing - value -> missing
            matrix.SetValueOrMissing(0, 0, double.NaN);
            Helper.CheckCondition(!matrix.TryGetValue(0, 0, out valueAX));
            //SetValueOrMissing - missing -> value
            matrix.SetValueOrMissing(0, 0, 7);
            Helper.CheckCondition(matrix.TryGetValue(0, 0, out valueAX) && valueAX == 7);
            //SetValueOrMissing - missing -> missing
            matrix.SetValueOrMissing(2, 2, double.NaN);
            Helper.CheckCondition(!matrix.TryGetValue(2, 2, out valueCZ));
            //Remove - true
            Helper.CheckCondition(matrix.Remove(0, 0));
            //Remove - false
            Helper.CheckCondition(!matrix.Remove(0, 0));
            //this get value
            Helper.CheckCondition(2 == matrix[0, 1]);
            //this set to value
            matrix[0, 1] = 8;
            Helper.CheckCondition(8 == matrix[0, 1]);
            //this get missing - expect error
            try
            {
                double valueCX = matrix[2, 0];
                Helper.CheckCondition(false);
            }
            catch (Exception)
            {
            }

            //this set to missing - expect error
            try
            {
                matrix[0, 0] = double.NaN;
                Helper.CheckCondition(false);
            }
            catch (Exception)
            {
            }
        }
    }
}
