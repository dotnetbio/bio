/****************************************************************************
 * ParserBvtTestCases.cs
 * 
 * This file contains the Parser BVT test cases.
 * 
******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bio.TestAutomation.Util;
using Bio.Util;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if (SILVERLIGHT == false)
namespace Bio.TestAutomation.Util
#else
   namespace Bio.SilverLight.TestAutomation
#endif
{
    /// <summary>
    /// Test Automation code for Bio Parser and BVT level validations.
    /// </summary>
    [TestClass]
    public class ParserBvtTestCases
    {
        #region Enum

        /// <summary>
        /// Sequence type for validating different test cases.
        /// </summary>
        private enum SequenceType
        {
            DNA,
            RNA,
            Proteins
        }

        #endregion

        #region Private Variables

        /// <summary>
        /// Contains collection of sequences.
        /// </summary>
        private string[] sequences;

        #endregion

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static ParserBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region Parser Bvt TestCases

        /// <summary>
        /// Validate TryParseAll method of Parser by passing valid sequences.
        /// Input Data : Valid Sequences.
        /// Output Data : Validate out variables and results.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateTryParseAllForValidSequences()
        {
            sequences = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilParserNode,
                Constants.Sequence1).Split(';');
            IList<string> outseq = new List<string>();

            bool result = Parser.TryParseAll(sequences.ToList(), out outseq);
            Assert.IsTrue(result);
            for (int count = 0; count < sequences.Length; count++)
            {
                Assert.AreEqual(sequences[count], outseq[count]);
            }

            ApplicationLog.WriteLine(string.Concat(
                  "Parser BVT: Validation of TryParseAll method for Sequences completed successfully."));
        }

        /// <summary>
        /// Validate TryParseAll method of Parser by passing  Enum name.
        /// Input Data : Valid Enum values.
        /// Output Data : Validate out variables and results.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateTryParseAllForEnumName()
        {
            sequences = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilParserNode,
                Constants.SequenceTypeNode).Split(';');
            IList<SequenceType> outseq = new List<SequenceType>();

            bool result = Parser.TryParseAll(sequences.ToList(), out outseq);
            Assert.IsTrue(result);
            for (int count = 0; count < sequences.Length; count++)
            {
                Assert.AreEqual((SequenceType)count, outseq[count]);
            }

            ApplicationLog.WriteLine(string.Concat(
                  "Parser BVT: Validation of TryParseAll method for Enum completed successfully."));
        }

        /// <summary>
        /// Validate TryParseAll method of Parser by passing  Enum value.
        /// Input Data : Valid Enum values.
        /// Output Data : Validate out variables and results.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateTryParseAllForEnumValue()
        {
            sequences = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilParserNode,
                Constants.SequenceValuesNode).Split(';');
            IList<SequenceType> outseq = new List<SequenceType>();

            bool result = Parser.TryParseAll(sequences.ToList(), out outseq);
            Assert.IsTrue(result);
            for (int count = 0; count < sequences.Length; count++)
            {
                Assert.AreEqual((SequenceType)count, outseq[count]);
            }

            ApplicationLog.WriteLine(string.Concat(
                  "Parser BVT: Validation of TryParseAll method for Enum completed successfully."));
        }

        /// <summary>
        /// Validate ParseAll method of Parser by passing  Enum Name.
        /// Input Data : Valid Enum values.
        /// Output Data : Validate List of Results.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateParseAllForEnumName()
        {
            sequences = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilParserNode,
                Constants.SequenceValuesNode).Split(';');

            IEnumerable<SequenceType> results = Parser.ParseAll<SequenceType>(sequences.ToList());
            int count = 0;
            foreach (SequenceType result in results)
            {
                Assert.AreEqual(((SequenceType)count++).ToString(), result.ToString());
            }

            ApplicationLog.WriteLine(string.Concat(
                  "Parser BVT: Validation of ParseAll method for Enum completed successfully."));
        }

        /// <summary>
        /// Validate ParseAll method of Parser by passing  Enum value.
        /// Input Data : Valid Enum values.
        /// Output Data : Validate List of Results.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateParseAllForEnumValue()
        {
            sequences = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilParserNode,
                Constants.SequenceValuesNode).Split(';');

            IEnumerable<SequenceType> results = Parser.ParseAll<SequenceType>(sequences.ToList());

            int count = 0;
            foreach (SequenceType result in results)
            {
                Assert.AreEqual(((SequenceType)count++).ToString(), result.ToString());
            }

            ApplicationLog.WriteLine(string.Concat(
                  "Parser BVT: Validation of ParseAll method for Enum completed successfully."));
        }

        /// <summary>
        /// Validate TryParse method of Parser by passing sequences.
        /// Input Data : Valid sequences.
        /// Output Data : Validate out variables and result.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateTryParseForSequences()
        {
            sequences = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilParserNode,
                Constants.SequenceTypeNode).Split(';');
            string expectedResult = String.Empty;
            for (int count = 0; count < sequences.Length; count++)
            {
                bool result = Parser.TryParse(sequences[count], out expectedResult);
                Assert.IsTrue(result);
                Assert.AreEqual(sequences[count], expectedResult);
            }

            ApplicationLog.WriteLine(string.Concat(
                  "Parser BVT: Validation of TryParse method for Sequences completed successfully."));
        }

        /// <summary>
        /// Validate TryParse method of Parser by passing sequences values.
        /// Input Data : Valid sequences.
        /// Output Data : Validate out variable and result.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateTryParseForSequencesValues()
        {
            sequences = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilParserNode,
                Constants.SequenceValuesNode).Split(';');
            int expectedResult = 0;
            for (int count = 0; count < sequences.Length; count++)
            {
                bool results = Parser.TryParse(sequences[count], out expectedResult);
                Assert.IsTrue(results);
                Assert.AreEqual(sequences[count], expectedResult.ToString());
            }

            ApplicationLog.WriteLine(string.Concat(
                  "Parser BVT: Validation of TryParse method for Sequences completed successfully."));
        }

        /// <summary>
        /// Validate Parse method of Parser by passing Sequence value.
        /// Input Data : Valid Sequence values.
        /// Output Data : Validate parsed result.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateParseForSequenceValues()
        {
            sequences = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilParserNode,
                Constants.SequenceValuesNode).Split(';');
            int expectedResult = 0;

            for (int count = 0; count < sequences.Length; count++)
            {
                object result = Parser.Parse(sequences[count], expectedResult.GetType());
                Assert.IsNotNull(result);
                Assert.AreEqual(sequences[count], result.ToString());
            }

            ApplicationLog.WriteLine(string.Concat(
                  "Parser BVT: Validation of Parse method for Sequences completed successfully."));
        }

        /// <summary>
        /// Validate Parse method of Parser by passing Sequence value.
        /// Input Data : Valid Sequence values.
        /// Output Data : Validate parsed result.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateParseForCollections()
        {
            sequences = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilParserNode,
                Constants.Sequence1).Split(';');
            IList<List<string>> outseq = new List<List<string>>();

            bool result = Parser.TryParseAll(sequences.ToList(), out outseq);
            Assert.IsTrue(result);
            int count = 0;
            foreach (List<string> list in outseq)
            {
                Assert.AreEqual(sequences[count++], list[0].ToString());
            }

            ApplicationLog.WriteLine(string.Concat(
                  "Parser BVT: Validation of TryParseAll method for Sequences completed successfully."));
        }

        /// <summary>
        /// Validate Parse method of Parser by passing Sequence value.
        /// Input Data : Valid Sequence values.
        /// Output Data : Validate parsed result.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateParseForNullable()
        {
            sequences = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilParserNode,
                Constants.SequenceValuesNode).Split(';');
            IList<int?> outseq = new List<int?>();

            bool result = Parser.TryParseAll(sequences.ToList(), out outseq);
            Assert.IsTrue(result);
            int count = 0;
            foreach (int list in outseq)
            {
                Assert.AreEqual(sequences[count++], list.ToString());
            }

            ApplicationLog.WriteLine(string.Concat(
                  "Parser BVT: Validation of TryParseAll method for Sequences completed successfully."));
        }

        /// <summary>
        /// Validate Parse method of Parser by passing Sequence value.
        /// Input Data : Valid Sequence values.
        /// Output Data : Validate parsed result.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateParseForGeneric()
        {
            sequences = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilParserNode,
                Constants.SequenceValuesNode).Split(';');
            IList<int> outseq = new List<int>();

            bool result = Parser.TryParseAll(sequences.ToList(), out outseq);
            Assert.IsTrue(result);
            int count = 0;
            foreach (int list in outseq)
            {
                Assert.AreEqual(sequences[count++], list.ToString());
            }

            ApplicationLog.WriteLine(string.Concat(
                  "Parser BVT: Validation of TryParseAll method for Sequences completed successfully."));
        }

        #endregion Parser Bvt TestCases
    }
}
