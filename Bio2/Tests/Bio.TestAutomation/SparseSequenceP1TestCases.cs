/****************************************************************************
 * SparseSequenceP1TestCases.cs
 * 
 * This file contains the Sparse Sequence P1 test case validation.
 * 
******************************************************************************/

using System;
using System.Collections.Generic;

using Bio;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Bio.TestAutomation
{
    /// <summary>
    /// Test Automation code for Bio Sparse Sequence P1 level validations
    /// </summary>
    [TestClass]
    public class SparseSequenceP1TestCases
    {

        #region Global Variables

        ASCIIEncoding encodingObj = new ASCIIEncoding();

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static SparseSequenceP1TestCases()
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
        /// Creates sparse sequence object and validates the constructor.
        /// Validates if all items are present in sparse sequence instance.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateRnaSparseSequenceConstAlp()
        {
            SparseSequence sparseSeq = new SparseSequence(Alphabets.RNA);
            Assert.IsNotNull(sparseSeq);
            Assert.AreEqual(0, sparseSeq.Count);
            Assert.IsNotNull(sparseSeq.Statistics);

            Console.WriteLine("SparseSequence P1: Validation of SparseSequence(alp) constructor is completed");
            ApplicationLog.WriteLine("SparseSequence P1: Validation of SparseSequence(alp) constructor is completed");
        }

        /// <summary>
        /// Creates sparse sequence object and validates the constructor with Index.
        /// Validates if all items are present in sparse sequence instance.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateRnaSparseSequenceConstAlpIndex()
        {
            SparseSequence sparseSeq = new SparseSequence(Alphabets.RNA, 0);
            Assert.IsNotNull(sparseSeq);
            Assert.AreEqual(0, sparseSeq.Count);
            Assert.IsNotNull(sparseSeq.Statistics);

            Console.WriteLine("SparseSequence P1: Validation of SparseSequence(alp, index) constructor is completed");
            ApplicationLog.WriteLine("SparseSequence P1: Validation of SparseSequence(alp, index) constructor is completed");
        }

        /// <summary>
        /// Creates sparse sequence object and validates the constructor with Index, byte.
        /// Validates if all items are present in sparse sequence instance.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateRnaSparseSequenceConstAlpIndexByte()
        {
            byte[] byteArrayObj = encodingObj.GetBytes("AGCU");
            SparseSequence sparseSeq = new SparseSequence(Alphabets.DNA, 1, byteArrayObj[0]);
            Assert.IsNotNull(sparseSeq);
            Assert.IsNotNull(sparseSeq.Statistics);
            SequenceStatistics seqStatObj = sparseSeq.Statistics;
            Assert.AreEqual(1, seqStatObj.GetCount('A'));

            Console.WriteLine("SparseSequence P1: Validation of SparseSequence(alp, index, byte) constructor is completed");
            ApplicationLog.WriteLine("SparseSequence P1: Validation of SparseSequence(alp, index, byte) constructor is completed");
        }

        /// <summary>
        /// Creates sparse sequence object and validates the constructor with Index, byte.
        /// Validates if all items are present in sparse sequence instance.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateRnaSparseSequenceConstAlpIndexByteList()
        {
            byte[] byteArrayObj = encodingObj.GetBytes("AGCU");

            IEnumerable<byte> seqItems =
                new List<Byte>() { byteArrayObj[0], byteArrayObj[1], byteArrayObj[2], byteArrayObj[3] };

            SparseSequence sparseSeq = new SparseSequence(Alphabets.RNA, 4, seqItems);
            Assert.IsNotNull(sparseSeq);
            Assert.IsNotNull(sparseSeq.Statistics);
            Assert.AreEqual(8, sparseSeq.Count);
            SequenceStatistics seqStatObj = sparseSeq.Statistics;
            Assert.AreEqual(1, seqStatObj.GetCount('A'));
            Assert.AreEqual(1, seqStatObj.GetCount('G'));
            Assert.AreEqual(1, seqStatObj.GetCount('C'));
            Assert.AreEqual(1, seqStatObj.GetCount('U'));

            Console.WriteLine("SparseSequence P1: Validation of SparseSequence(alp, index, seq items) constructor is completed");
            ApplicationLog.WriteLine("SparseSequence P1: Validation of SparseSequence(alp, index, seq items) constructor is completed");
        }

        /// <summary>
        /// Creates sparse sequence object and validates the constructor.
        /// Validates if all items are present in sparse sequence instance.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateProteinSparseSequenceConstAlp()
        {
            SparseSequence sparseSeq = new SparseSequence(Alphabets.Protein);
            Assert.IsNotNull(sparseSeq);
            Assert.AreEqual(0, sparseSeq.Count);
            Assert.IsNotNull(sparseSeq.Statistics);

            Console.WriteLine("SparseSequence P1: Validation of SparseSequence(alp) constructor is completed");
            ApplicationLog.WriteLine("SparseSequence P1: Validation of SparseSequence(alp) constructor is completed");
        }

        /// <summary>
        /// Creates sparse sequence object and validates the constructor with Index.
        /// Validates if all items are present in sparse sequence instance.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateProteinSparseSequenceConstAlpIndex()
        {
            SparseSequence sparseSeq = new SparseSequence(Alphabets.Protein, 0);
            Assert.IsNotNull(sparseSeq);
            Assert.AreEqual(0, sparseSeq.Count);
            Assert.IsNotNull(sparseSeq.Statistics);

            Console.WriteLine("SparseSequence P1: Validation of SparseSequence(alp, index) constructor is completed");
            ApplicationLog.WriteLine("SparseSequence P1: Validation of SparseSequence(alp, index) constructor is completed");
        }

        /// <summary>
        /// Creates sparse sequence object and validates the constructor with Index, byte.
        /// Validates if all items are present in sparse sequence instance.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateProteinSparseSequenceConstAlpIndexByte()
        {
            byte[] byteArrayObj = encodingObj.GetBytes("KIEG");
            SparseSequence sparseSeq = new SparseSequence(Alphabets.Protein, 1, byteArrayObj[0]);
            Assert.IsNotNull(sparseSeq);
            Assert.IsNotNull(sparseSeq.Statistics);
            SequenceStatistics seqStatObj = sparseSeq.Statistics;
            Assert.AreEqual(1, seqStatObj.GetCount('K'));

            Console.WriteLine("SparseSequence P1: Validation of SparseSequence(alp, index, byte) constructor is completed");
            ApplicationLog.WriteLine("SparseSequence P1: Validation of SparseSequence(alp, index, byte) constructor is completed");
        }

        /// <summary>
        /// Creates sparse sequence object and validates the constructor with Index, byte.
        /// Validates if all items are present in sparse sequence instance.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateProteinSparseSequenceConstAlpIndexByteList()
        {
            byte[] byteArrayObj = encodingObj.GetBytes("KIEG");

            IEnumerable<byte> seqItems =
                new List<Byte>() { byteArrayObj[0], byteArrayObj[1], byteArrayObj[2], byteArrayObj[3] };

            SparseSequence sparseSeq = new SparseSequence(Alphabets.Protein, 4, seqItems);
            Assert.IsNotNull(sparseSeq);
            Assert.IsNotNull(sparseSeq.Statistics);
            Assert.AreEqual(8, sparseSeq.Count);
            SequenceStatistics seqStatObj = sparseSeq.Statistics;
            Assert.AreEqual(1, seqStatObj.GetCount('K'));
            Assert.AreEqual(1, seqStatObj.GetCount('I'));
            Assert.AreEqual(1, seqStatObj.GetCount('E'));
            Assert.AreEqual(1, seqStatObj.GetCount('G'));

            Console.WriteLine("SparseSequence P1: Validation of SparseSequence(alp, index, seq items) constructor is completed");
            ApplicationLog.WriteLine("SparseSequence P1: Validation of SparseSequence(alp, index, seq items) constructor is completed");
        }

        #endregion
    }
}
