using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Bio.Util.Logging;
using Bio.Matrix;
using Bio.TestAutomation.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.TestAutomation.Matrix
{
    /// <summary>
    /// Bvt test cases to confirm the features of Dense Matrix
    /// </summary>
    [TestClass]
    public class PaddedDoubleBvtTestCases
    {
        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\MatrixTestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static PaddedDoubleBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion

        #region Test Cases

        /// <summary>
        /// Validates ColKeysInFile method
        /// Input : Valid values for PaddedDouble
        /// Validation : Col Keys In File
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePaddedDoubleColKeysInFile()
        {
            DenseMatrix<string, string, double> denseMatObj =
               GetDenseMatrix();
            ParallelOptions parOptObj = new ParallelOptions();

            denseMatObj.WritePaddedDouble(Constants.FastQTempTxtFileName,
                parOptObj);

            string[] colkey =
                PaddedDouble.ColKeysInFile(Constants.FastQTempTxtFileName);
            for (int i = 0; i < colkey.Length; i++)
            {
                Assert.AreEqual(denseMatObj.ColKeys[i], colkey[i]);
            }

            if (File.Exists(Constants.FastQTempTxtFileName))
                File.Delete(Constants.FastQTempTxtFileName);

            ApplicationLog.WriteLine(
                "PaddedDouble BVT : Validation of ColKeysInFile() method successful");
        }

        /// <summary>
        /// Validates CreateEmptyInstance method
        /// Input : Valid values for PaddedDouble
        /// Validation : create empty instance method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePaddedDoubleCreateEmptyInstance()
        {
            PaddedDouble pdObj = PaddedDouble.CreateEmptyInstance(new string[] { "R0", "R1", "R2" }, new string[] { "C0", "C1", "C2", "C3" }, double.NaN);

            Assert.AreEqual("3", pdObj.RowCount.ToString((IFormatProvider)null));
            Assert.AreEqual("4", pdObj.ColCount.ToString((IFormatProvider)null));

            ApplicationLog.WriteLine(
                "PaddedDouble BVT : Validation of CreateEmptyInstance() method successful");
        }

        /// <summary>
        /// Validates EachSparseLine method
        /// Input : Valid values for PaddedDouble
        /// Validation : Each Sparse Line method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePaddedDoubleEachSparseLine()
        {
            DenseMatrix<string, string, double> denseMatObj =
               GetDenseMatrix();
            ParallelOptions parOptObj = new ParallelOptions();

            denseMatObj.WritePaddedDouble(Constants.FastQTempTxtFileName,
                parOptObj);

            IEnumerable<string[]> sparseLineObjs =
                PaddedDouble.EachSparseLine(Constants.FastQTempTxtFileName,
                true, "Null File", new CounterWithMessages("Counter", 10, 20));

            foreach (string[] sparseLineObj in sparseLineObjs)
            {
                Assert.IsTrue(denseMatObj.RowKeys.Contains(sparseLineObj[0]));
                Assert.IsTrue(denseMatObj.ColKeys.Contains(sparseLineObj[1]));
            }

            ApplicationLog.WriteLine(
                "PaddedDouble BVT : Validation of EachSparseLine() method successful");
        }

        /// <summary>
        /// Validates GetInstance method
        /// Input : Valid values for PaddedDouble
        /// Validation : gets instance
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePaddedDoubleGetInstance()
        {
            DenseMatrix<string, string, double> denseMatObj =
               GetDenseMatrix();
            ParallelOptions parOptObj = new ParallelOptions();

            denseMatObj.WritePaddedDouble(Constants.FastQTempTxtFileName, parOptObj);

            PaddedDouble pdObj =
                PaddedDouble.GetInstance(Constants.FastQTempTxtFileName, parOptObj);

            Assert.AreEqual(denseMatObj.ColCount, pdObj.ColCount);
            Assert.AreEqual(denseMatObj.RowCount, pdObj.RowCount);
            Assert.AreEqual(denseMatObj.RowKeys.Count, pdObj.RowKeys.Count);
            Assert.AreEqual(denseMatObj.ColKeys.Count, pdObj.ColKeys.Count);

            if (File.Exists(Constants.FastQTempTxtFileName))
                File.Delete(Constants.FastQTempTxtFileName);

            ApplicationLog.WriteLine(
                "PaddedDouble BVT : Validation of GetInstance() method successful");
        }

        /// <summary>
        /// Validates GetInstanceFromSparse method
        /// Input : Valid values for PaddedDouble
        /// Validation : gets instance from sparse
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePaddedDoubleGetInstanceFromSparse()
        {
            SparseMatrix<string, string, double> sparseMatObj =
               CreateSimpleSparseMatrix();

            sparseMatObj.WriteSparse(Constants.FastQTempTxtFileName);

            PaddedDouble pdObj =
                PaddedDouble.GetInstanceFromSparse(Constants.FastQTempTxtFileName);

            Assert.AreEqual(sparseMatObj.ColCount, pdObj.ColCount);
            Assert.AreEqual(sparseMatObj.RowCount, pdObj.RowCount);
            Assert.AreEqual(sparseMatObj.RowKeys.Count, pdObj.RowKeys.Count);
            Assert.AreEqual(sparseMatObj.ColKeys.Count, pdObj.ColKeys.Count);

            if (File.Exists(Constants.FastQTempTxtFileName))
                File.Delete(Constants.FastQTempTxtFileName);

            ApplicationLog.WriteLine(
                "PaddedDouble BVT : Validation of GetInstanceFromSparse() method successful");
        }

        /// <summary>
        /// Validates GetInstanceFromSparse method with enumerable
        /// Input : Valid values for PaddedDouble
        /// Validation : gets instance from sparse with enumerable
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePaddedDoubleGetInstanceFromSparseEnum()
        {
            RowKeyColKeyValue<string, string, double> rowKeyObj =
                new RowKeyColKeyValue<string, string, double>("R0", "C0", 2);

            List<RowKeyColKeyValue<string, string, double>> enumObj =
                new List<RowKeyColKeyValue<string, string, double>>();

            enumObj.Add(rowKeyObj);

            PaddedDouble pdObj =
                PaddedDouble.GetInstanceFromSparse(enumObj);

            Assert.AreEqual(1, pdObj.ColCount);
            Assert.AreEqual(1, pdObj.RowCount);
            Assert.AreEqual("R0", pdObj.RowKeys[0]);
            Assert.AreEqual("C0", pdObj.ColKeys[0]);

            ApplicationLog.WriteLine(
                "PaddedDouble BVT : Validation of GetInstanceFromSparse(enum) method successful");
        }

        /// <summary>
        /// Validates RowKeysInFile method
        /// Input : Valid values for PaddedDouble
        /// Validation : Row Keys In File
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePaddedDoubleRowKeysInFile()
        {
            DenseMatrix<string, string, double> denseMatObj =
                GetDenseMatrix();
            ParallelOptions parOptObj = new ParallelOptions();

            denseMatObj.WritePaddedDouble(Constants.FastQTempTxtFileName,
                parOptObj);
            IEnumerable<string> rowKeys =
                PaddedDouble.RowKeysInFile(Constants.FastQTempTxtFileName);

            int i = 0;
            foreach (string rowKey in rowKeys)
            {
                Assert.AreEqual(denseMatObj.RowKeys[i], rowKey);
                i++;
            }

            if (File.Exists(Constants.FastQTempTxtFileName))
                File.Delete(Constants.FastQTempTxtFileName);

            ApplicationLog.WriteLine(
                "PaddedDouble BVT : Validation of RowKeysInFile() method successful");
        }

        /// <summary>
        /// Validates StoreToSparseVal method
        /// Input : Valid values for PaddedDouble
        /// Validation : Store To Sparse Val
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePaddedDoubleStoreToSparseVal()
        {
            string sparseString = PaddedDouble.StoreToSparseVal(2);

            Assert.AreEqual("                      2", sparseString);

            ApplicationLog.WriteLine(
                "PaddedDouble BVT : Validation of StoreToSparseVal() method successful");
        }

        /// <summary>
        /// Validates TryGetInstance method
        /// Input : Valid values for PaddedDouble
        /// Validation : Try Get Instance
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePaddedDoubleTryGetInstance()
        {
            DenseMatrix<string, string, double> denseMatObj =
                GetDenseMatrix();
            ParallelOptions parOptObj = new ParallelOptions();
            Matrix<string, string, double> matObj = null;
            denseMatObj.WritePaddedDouble(Constants.FastQTempTxtFileName,
                parOptObj);
            Assert.IsTrue(PaddedDouble.TryGetInstance(Constants.FastQTempTxtFileName,
                double.NaN, parOptObj, out matObj));

            Assert.AreEqual(denseMatObj.ColCount, matObj.ColCount);
            Assert.AreEqual(denseMatObj.RowCount, matObj.RowCount);
            Assert.AreEqual(denseMatObj.RowKeys.Count, matObj.RowKeys.Count);
            Assert.AreEqual(denseMatObj.ColKeys.Count, matObj.ColKeys.Count);

            if (File.Exists(Constants.FastQTempTxtFileName))
                File.Delete(Constants.FastQTempTxtFileName);

            ApplicationLog.WriteLine(
                "PaddedDouble BVT : Validation of TryGetInstance() method successful");
        }

        /// <summary>
        /// Validates TryGetInstanceFromSparse method with PD
        /// Input : Valid values for PaddedDouble
        /// Validation : Try Get Instance from sparse with PD
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePaddedDoubleTryGetInstanceFromSparsePD()
        {
            SparseMatrix<string, string, double> sparseMatObj =
                CreateSimpleSparseMatrix();

            PaddedDouble padDoubObj = null;
            sparseMatObj.WriteSparse(Constants.FastQTempTxtFileName);
            Assert.IsTrue(PaddedDouble.TryGetInstanceFromSparse(
                Constants.FastQTempTxtFileName, out padDoubObj));

            Assert.AreEqual(sparseMatObj.ColCount, padDoubObj.ColCount);
            Assert.AreEqual(sparseMatObj.RowCount, padDoubObj.RowCount);
            Assert.AreEqual(sparseMatObj.RowKeys.Count, padDoubObj.RowKeys.Count);
            Assert.AreEqual(sparseMatObj.ColKeys.Count, padDoubObj.ColKeys.Count);

            if (File.Exists(Constants.FastQTempTxtFileName))
                File.Delete(Constants.FastQTempTxtFileName);

            ApplicationLog.WriteLine(
                "PaddedDouble BVT : Validation of TryGetInstanceFromSparse(padded double) method successful");
        }

        /// <summary>
        /// Validates TryGetInstanceFromSparse method with Matrix
        /// Input : Valid values for PaddedDouble
        /// Validation : Try Get Instance from sparse with Matrix
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePaddedDoubleTryGetInstanceFromSparseMatrix()
        {
            SparseMatrix<string, string, double> sparseMatObj =
                CreateSimpleSparseMatrix();

            Matrix<string, string, double> matObj = null;
            sparseMatObj.WriteSparse(Constants.FastQTempTxtFileName);
            Assert.IsTrue(PaddedDouble.TryGetInstanceFromSparse(
                Constants.FastQTempTxtFileName, out matObj));

            Assert.AreEqual(sparseMatObj.ColCount, matObj.ColCount);
            Assert.AreEqual(sparseMatObj.RowCount, matObj.RowCount);
            Assert.AreEqual(sparseMatObj.RowKeys.Count, matObj.RowKeys.Count);
            Assert.AreEqual(sparseMatObj.ColKeys.Count, matObj.ColKeys.Count);

            if (File.Exists(Constants.FastQTempTxtFileName))
                File.Delete(Constants.FastQTempTxtFileName);

            ApplicationLog.WriteLine(
                "PaddedDouble BVT : Validation of TryGetInstanceFromSparse(matrix) method successful");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a simple matrix for local validation
        /// </summary>
        /// <returns>Dense Matrix</returns>
        static SparseMatrix<string, string, double> CreateSimpleSparseMatrix()
        {
            SparseMatrix<string, string, double> sparseMatObj =
                SparseMatrix<string, string, double>.CreateEmptyInstance(
                new string[] { "R0", "R1", "R2" }, new string[] { "C0", "C1", "C2" }, double.NaN);

            sparseMatObj[0, 0] = 1;
            sparseMatObj[0, 1] = 2;
            sparseMatObj[0, 2] = 3;
            sparseMatObj[1, 0] = 4;
            sparseMatObj[1, 1] = 5;
            sparseMatObj[1, 2] = 6;
            sparseMatObj[2, 0] = 7;
            sparseMatObj[2, 1] = 8;
            sparseMatObj[2, 2] = 9;

            return sparseMatObj;
        }

        /// <summary>
        /// Gets the two D array from the xml
        /// </summary>
        /// <param name="nodeName">Node Name of the xml to be parsed</param>
        /// <param name="maxRows">Maximum rows</param>
        /// <param name="maxColumns">Maximum columns</param>
        /// <returns>2 D Array</returns>
        double[,] GetTwoDArray(string nodeName, out int maxRows,
            out int maxColumns)
        {
            string[] rowArray = utilityObj.xmlUtil.GetTextValues(nodeName, Constants.RowsNode);

            // Gets the max number columns in the array
            maxColumns = 0;
            maxRows = rowArray.Length;
            for (int i = 0; i < maxRows; i++)
            {
                string[] colArray = rowArray[i].Split(',');
                if (maxColumns < colArray.Length)
                    maxColumns = colArray.Length;
            }

            // Creates a 2 D with max row and column length
            double[,] twoDArray = new double[maxRows, maxColumns];
            for (int i = 0; i < maxRows; i++)
            {
                string[] colArray = rowArray[i].Split(',');
                for (int j = 0; j < colArray.Length; j++)
                {
                    twoDArray[i, j] = double.Parse(colArray[j], (IFormatProvider)null);
                }
            }

            return twoDArray;
        }

        /// <summary>
        /// Gets the key sequence with the max length specified
        /// </summary>
        /// <param name="maxKey">Max length of the key sequence</param>
        /// <param name="isRow">If Row, append R else append C</param>
        /// <returns>Key Sequence Array</returns>
        static string[] GetKeySequence(int maxKey, bool isRow)
        {
            string[] keySeq = new string[maxKey];
            string tempSeq = string.Empty;

            if (isRow)
                tempSeq = "R";
            else
                tempSeq = "C";

            for (int i = 0; i < maxKey; i++)
            {
                keySeq[i] = tempSeq + i.ToString((IFormatProvider)null);
            }

            return keySeq;
        }

        /// <summary>
        /// Creates a DenseMatrix instance and returns the same.
        /// </summary>
        /// <returns>DenseMatrix Instance</returns>
        DenseMatrix<string, string, double> GetDenseMatrix()
        {
            int maxRows = 0;
            int maxColumns = 0;
            double[,] twoDArray = GetTwoDArray(Constants.SimpleMatrixNodeName,
                out maxRows, out maxColumns);

            string[] rowKeySeq = GetKeySequence(maxRows, true);
            string[] colKeySeq = GetKeySequence(maxColumns, false);

            DenseMatrix<string, string, double> denseMatrixObj =
                new DenseMatrix<string, string, double>(twoDArray, rowKeySeq,
                    colKeySeq, double.NaN);

            return denseMatrixObj;
        }

        #endregion;
    }
}
