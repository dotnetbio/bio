using System;
using System.IO;
using System.Threading.Tasks;
using Bio.Matrix;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.TestAutomation.Matrix
{
    /// <summary>
    /// Bvt test cases to confirm the features of Dense Matrix
    /// </summary>
    [TestClass]
    public class RowKeysAnsiBvtTestCases
    {
        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\MatrixTestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static RowKeysAnsiBvtTestCases()
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
        /// Validates GetInstanceFromDenseAnsi method
        /// Input : Valid values for RowKeysAnsi
        /// Validation : GetInstance From DenseAnsi
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateRowKeysAnsiGetInstanceFromDenseAnsi()
        {
            DenseMatrix<string, string, double> denseMatObj =
                GetDenseMatrix();
            ParallelOptions parOptObj = new ParallelOptions();

            denseMatObj.WriteDenseAnsi(Constants.FastQTempTxtFileName,
                parOptObj);

            using (RowKeysAnsi rkaObj =
                RowKeysAnsi.GetInstanceFromDenseAnsi(Constants.FastQTempTxtFileName,
                parOptObj))
            {

                Assert.AreEqual(denseMatObj.ColCount, rkaObj.ColCount);
                Assert.AreEqual(denseMatObj.RowCount, rkaObj.RowCount);
                Assert.AreEqual(denseMatObj.RowKeys.Count, rkaObj.RowKeys.Count);
                Assert.AreEqual(denseMatObj.ColKeys.Count, rkaObj.ColKeys.Count);
            }

            if (File.Exists(Constants.FastQTempTxtFileName))
                File.Delete(Constants.FastQTempTxtFileName);

            ApplicationLog.WriteLine(
                "RowKeysAnsi BVT : Validation of GetInstanceFromDenseAnsi() method successful");
        }

        /// <summary>
        /// Validates GetInstanceFromDenseAnsi method with file-access
        /// Input : Valid values for RowKeysAnsi
        /// Validation : GetInstance From DenseAnsi with file-access
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateRowKeysAnsiGetInstanceFromDenseAnsiFileAccess()
        {
            DenseMatrix<string, string, double> denseMatObj =
                GetDenseMatrix();
            ParallelOptions parOptObj = new ParallelOptions();

            denseMatObj.WriteDenseAnsi(Constants.FastQTempTxtFileName,
                parOptObj);

            using (RowKeysAnsi rkaObj =
                RowKeysAnsi.GetInstanceFromDenseAnsi(
                Constants.FastQTempTxtFileName, parOptObj, FileAccess.ReadWrite,
                FileShare.ReadWrite))
            {
                Assert.AreEqual(denseMatObj.ColCount, rkaObj.ColCount);
                Assert.AreEqual(denseMatObj.RowCount, rkaObj.RowCount);
                Assert.AreEqual(denseMatObj.RowKeys.Count, rkaObj.RowKeys.Count);
                Assert.AreEqual(denseMatObj.ColKeys.Count, rkaObj.ColKeys.Count);
            }

            if (File.Exists(Constants.FastQTempTxtFileName))
                File.Delete(Constants.FastQTempTxtFileName);

            ApplicationLog.WriteLine(
                "RowKeysAnsi BVT : Validation of GetInstanceFromDenseAnsi(file-access) method successful");
        }

        /// <summary>
        /// Validates GetInstanceFromDenseAnsi method
        /// Input : Valid values for RowKeysAnsi
        /// Validation : GetInstance From DenseAnsi
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateRowKeysAnsiGetInstanceFromRowKeysAnsi()
        {
            DenseMatrix<string, string, double> denseMatObj =
                GetDenseMatrix();
            ParallelOptions parOptObj = new ParallelOptions();

            denseMatObj.WriteDenseAnsi(Constants.FastQTempTxtFileName,
                parOptObj);

            using (RowKeysAnsi tempRkaObj =
                RowKeysAnsi.GetInstanceFromDenseAnsi(Constants.FastQTempTxtFileName, parOptObj))
            {
                tempRkaObj.WriteRowKeys(Constants.KeysTempFile);
            }

            using (RowKeysAnsi rkaObj =
                RowKeysAnsi.GetInstanceFromRowKeysAnsi(Constants.KeysTempFile,
                parOptObj))
            {
                Assert.AreEqual(denseMatObj.ColCount, rkaObj.ColCount);
                Assert.AreEqual(denseMatObj.RowCount, rkaObj.RowCount);
                Assert.AreEqual(denseMatObj.RowKeys.Count, rkaObj.RowKeys.Count);
                Assert.AreEqual(denseMatObj.ColKeys.Count, rkaObj.ColKeys.Count);
            }

            if (File.Exists(Constants.FastQTempTxtFileName))
                File.Delete(Constants.FastQTempTxtFileName);

            ApplicationLog.WriteLine(
                "RowKeysAnsi BVT : Validation of GetInstanceFromRowKeysAnsi() method successful");
        }

        /// <summary>
        /// Validates GetInstanceFromDenseAnsi method with file-access
        /// Input : Valid values for RowKeysAnsi
        /// Validation : GetInstance From DenseAnsi with file-access
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateRowKeysAnsiGetInstanceFromRowKeysAnsiFileAccess()
        {
            DenseMatrix<string, string, double> denseMatObj =
                 GetDenseMatrix();
            ParallelOptions parOptObj = new ParallelOptions();

            denseMatObj.WriteDenseAnsi(Constants.FastQTempTxtFileName,
                parOptObj);

            using (RowKeysAnsi rkaObj =
                 RowKeysAnsi.GetInstanceFromDenseAnsi(Constants.FastQTempTxtFileName, parOptObj))
            {
                rkaObj.WriteRowKeys(Constants.KeysTempFile);
            }

            using (RowKeysAnsi newRkaObj =
                RowKeysAnsi.GetInstanceFromRowKeysAnsi(Constants.KeysTempFile, parOptObj,
                FileAccess.Read, FileShare.Read))
            {
                Assert.AreEqual(denseMatObj.ColCount, newRkaObj.ColCount);
                Assert.AreEqual(denseMatObj.RowCount, newRkaObj.RowCount);
                Assert.AreEqual(denseMatObj.RowKeys.Count, newRkaObj.RowKeys.Count);
                Assert.AreEqual(denseMatObj.ColKeys.Count, newRkaObj.ColKeys.Count);
            }

            if (File.Exists(Constants.FastQTempTxtFileName))
                File.Delete(Constants.FastQTempTxtFileName);

            if (File.Exists(Constants.KeysTempFile))
                File.Delete(Constants.KeysTempFile);

            ApplicationLog.WriteLine(
                "RowKeysAnsi BVT : Validation of GetInstanceFromRowKeysAnsi(file-access) method successful");
        }

        #endregion

        #region Helper Methods

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
