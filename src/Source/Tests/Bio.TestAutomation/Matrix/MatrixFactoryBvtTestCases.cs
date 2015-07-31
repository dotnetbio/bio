using System;
using System.Linq;
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
    public class MatrixFactoryBvtTestCases
    {
        #region Global Variables

        DenseMatrix<string, string, double> denseMatObj;
        Utility utilityObj = new Utility(@"TestUtils\MatrixTestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static MatrixFactoryBvtTestCases()
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
        /// Creates a Register Matrix Parser
        /// Input : Valid values for MatrixFactory
        /// Validation : Register Matrix Parser
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMatrixFactoryRegisterMatrixParser()
        {
            MatrixFactory<String, String, Double> mfactObj =
                MatrixFactory<String, String, Double>.GetInstance();

            TryParseMatrixDelegate<string, string, double> tryParseDelObj =
                new TryParseMatrixDelegate<string, string, double>(TryParseMatrix);

            mfactObj.RegisterMatrixParser(tryParseDelObj);
            Assert.IsTrue(true,
                "No exceptions were thrown on running RegisterMatrixParser() method");
        }

        /// <summary>
        /// Creates a Dense Matrix and validate parse
        /// method
        /// Input : Valid values for MatrixFactory
        /// Validation : parse method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMatrixFactoryParse()
        {
            denseMatObj = GetDenseMatrix();

            MatrixFactory<String, String, Double> mfObj =
                MatrixFactory<String, String, Double>.GetInstance();

            ParallelOptions poObj = new ParallelOptions();

            TryParseMatrixDelegate<string, string, double> a =
                new TryParseMatrixDelegate<string, string, double>(this.TryParseMatrix);
            mfObj.RegisterMatrixParser(a);
            // Writes the text file
            denseMatObj.WritePaddedDouble(Constants.FastQTempTxtFileName, poObj);

            Matrix<string, string, double> newMatObj =
                mfObj.Parse(Constants.FastQTempTxtFileName, double.NaN, poObj);

            Assert.AreEqual(denseMatObj.RowCount, newMatObj.RowCount);
            Assert.AreEqual(denseMatObj.RowKeys.Count, newMatObj.RowKeys.Count);
            Assert.AreEqual(denseMatObj.ColCount, newMatObj.ColCount);
            Assert.AreEqual(denseMatObj.ColKeys.Count, newMatObj.ColKeys.Count);
            Assert.AreEqual(denseMatObj.Values.Count(), newMatObj.Values.Count());

            ApplicationLog.WriteLine(
                "MatrixFactory BVT : Successfully validated Parse() method");
        }

        /// <summary>
        /// Creates a Dense Matrix and validate try parse
        /// method
        /// Input : Valid values for MatrixFactory
        /// Validation : try parse method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMatrixFactoryTryParse()
        {
            denseMatObj = GetDenseMatrix();

            MatrixFactory<String, String, Double> mfObj =
                MatrixFactory<String, String, Double>.GetInstance();

            ParallelOptions poObj = new ParallelOptions();

            TryParseMatrixDelegate<string, string, double> a =
                new TryParseMatrixDelegate<string, string, double>(this.TryParseMatrix);
            mfObj.RegisterMatrixParser(a);
            // Writes the text file
            denseMatObj.WritePaddedDouble(Constants.FastQTempTxtFileName, poObj);

            Matrix<string, string, double> newMatObj = null;
            Assert.IsTrue(mfObj.TryParse(
                Constants.FastQTempTxtFileName, double.NaN, poObj, out newMatObj));

            Assert.AreEqual(denseMatObj.RowCount, newMatObj.RowCount);
            Assert.AreEqual(denseMatObj.RowKeys.Count, newMatObj.RowKeys.Count);
            Assert.AreEqual(denseMatObj.ColCount, newMatObj.ColCount);
            Assert.AreEqual(denseMatObj.ColKeys.Count, newMatObj.ColKeys.Count);
            Assert.AreEqual(denseMatObj.Values.Count(), newMatObj.Values.Count());

            ApplicationLog.WriteLine(
                "MatrixFactory BVT : Successfully validated TryParse() method");
        }

        /// <summary>
        /// Creates a Dense Matrix and validate try parse
        /// method
        /// Input : Valid values for MatrixFactory
        /// Validation : try parse method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMatrixFactoryAllMethods()
        {
            denseMatObj = GetDenseMatrix();

            MatrixFactory<String, String, Double> mfObj =
                MatrixFactory<String, String, Double>.GetInstance();

            Assert.IsTrue(string.IsNullOrEmpty(mfObj.ErrorMessages));

            ApplicationLog.WriteLine(
                "MatrixFactory BVT : Successfully validated All methods");
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

        /// <summary>
        /// Delegate implementation
        /// </summary>
        /// <param name="filename">File name</param>
        /// <param name="missingValue">Missing value</param>
        /// <param name="parallelOptions">parallel options</param>
        /// <param name="matrix">Matrix</param>
        /// <returns>bool</returns>
        bool TryParseMatrix(string filename, double missingValue, ParallelOptions parallelOptions,
            out Matrix<string, string, double> matrix)
        {
            matrix = denseMatObj;
            return false;
        }

        #endregion;
    }
}
