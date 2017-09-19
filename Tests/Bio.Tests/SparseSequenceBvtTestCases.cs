using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests
{
    /// <summary>
    ///     Test Automation code for Bio Sparse Sequence BVT level validations
    /// </summary>
    [TestFixture]
    public class SparseSequenceBvtTestCases
    {
        #region Sparse Sequence BVT Test Cases

        /// <summary>
        ///     Creates sparse sequence object and validates the constructor.
        ///     Validates if all items are present in sparse sequence instance.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateDnaSparseSequenceConstAlp()
        {
            var sparseSeq = new SparseSequence(Alphabets.DNA);
            Assert.IsNotNull(sparseSeq);
            Assert.AreEqual(0, sparseSeq.Count);
            Assert.IsNotNull(sparseSeq.Statistics);
            ApplicationLog.WriteLine("SparseSequence BVT: Validation of SparseSequence(alp) constructor is completed");
        }

        /// <summary>
        ///     Creates sparse sequence object and validates the constructor with Index.
        ///     Validates if all items are present in sparse sequence instance.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateDnaSparseSequenceConstAlpIndex()
        {
            var sparseSeq = new SparseSequence(Alphabets.DNA, 0);
            Assert.IsNotNull(sparseSeq);
            Assert.AreEqual(0, sparseSeq.Count);
            Assert.IsNotNull(sparseSeq.Statistics);
            ApplicationLog.WriteLine("SparseSequence BVT: Validation of SparseSequence(alp, index) constructor is completed");
        }

        /// <summary>
        ///     Creates sparse sequence object and validates the constructor with Index, byte.
        ///     Validates if all items are present in sparse sequence instance.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateDnaSparseSequenceConstAlpIndexByte()
        {
            byte[] byteArrayObj = Encoding.ASCII.GetBytes("AGCT");
            var sparseSeq = new SparseSequence(Alphabets.DNA, 1, byteArrayObj[0]);
            Assert.IsNotNull(sparseSeq);
            Assert.IsNotNull(sparseSeq.Statistics);
            SequenceStatistics seqStatObj = sparseSeq.Statistics;
            Assert.AreEqual(1, seqStatObj.GetCount('A'));
            ApplicationLog.WriteLine("SparseSequence BVT: Validation of SparseSequence(alp, index, byte) constructor is completed");
        }

        /// <summary>
        ///     Creates sparse sequence object and validates the constructor with Index, byte.
        ///     Validates if all items are present in sparse sequence instance.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateDnaSparseSequenceConstAlpIndexByteList()
        {
            byte[] byteArrayObj = Encoding.ASCII.GetBytes("AGCT");

            IEnumerable<byte> seqItems =
                new List<Byte> {byteArrayObj[0], byteArrayObj[1], byteArrayObj[2], byteArrayObj[3]};

            var sparseSeq = new SparseSequence(Alphabets.DNA, 4, seqItems);
            Assert.IsNotNull(sparseSeq);
            Assert.IsNotNull(sparseSeq.Statistics);
            Assert.AreEqual(8, sparseSeq.Count);
            SequenceStatistics seqStatObj = sparseSeq.Statistics;
            Assert.AreEqual(1, seqStatObj.GetCount('A'));
            Assert.AreEqual(1, seqStatObj.GetCount('G'));
            Assert.AreEqual(1, seqStatObj.GetCount('C'));
            Assert.AreEqual(1, seqStatObj.GetCount('T'));

            ApplicationLog.WriteLine("SparseSequence BVT: Validation of SparseSequence(alp, index, seq items) constructor is completed");
        }

        /// <summary>
        ///     Creates a sparse a sequence and inserts all sequence items of alphabet.
        ///     Validates various properties present in the sparse class.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateDnaSparseSequenceProperties()
        {
            IAlphabet alphabet = Alphabets.DNA;

            // Create sparse sequence object
            const int insertPosition = 0;
            // Create sequence item list
            IList<byte> sequenceList = alphabet.ToList();

            // Store sequence item in sparse sequence object using list of sequence items
            var sparseSequence = new SparseSequence(alphabet, insertPosition, sequenceList);

            //Validate all properties
            Assert.AreEqual(alphabet.Count + insertPosition, sparseSequence.Count);
            Assert.AreEqual(alphabet, sparseSequence.Alphabet);
            Assert.IsTrue(string.IsNullOrEmpty(sparseSequence.ID));
            Assert.IsNotNull(sparseSequence.Metadata);
            Assert.IsNotNull(sparseSequence.Statistics);
            Assert.IsNotNull(sparseSequence.GetKnownSequenceItems());

            ApplicationLog.WriteLine("SparseSequence BVT: Validation of all properties of sparse sequence instance is completed");
        }

        /// <summary>
        ///     Creates a sparse sequence and validates IndexOfNonGap method
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSparseSequenceIndexOfNonGap()
        {
            var sparseSeqObj = new SparseSequence(Alphabets.DNA, 10);
            sparseSeqObj[8] = Alphabets.DNA.Gap;
            sparseSeqObj[9] = Alphabets.DNA.A;

            Assert.AreEqual(9, sparseSeqObj.IndexOfNonGap(8));
            Assert.AreEqual(9, sparseSeqObj.IndexOfNonGap(9));

            ApplicationLog.WriteLine("SparseSequenceBVT: Validation of IndexOfNonGap(startPos) method successfully completed");
        }

        /// <summary>
        ///     Creates a sparse sequence and validates LastIndexOfNonGap method
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSparseSequenceLastIndexOfNonGap()
        {
            var sparseSeqObj = new SparseSequence(Alphabets.DNA, 10);
            sparseSeqObj[2] = Alphabets.DNA.Gap;
            sparseSeqObj[1] = Alphabets.DNA.A;

            Assert.AreEqual(1, sparseSeqObj.LastIndexOfNonGap(2));
            Assert.AreEqual(1, sparseSeqObj.LastIndexOfNonGap(1));

            ApplicationLog.WriteLine("SparseSequenceBVT: Validation of LastIndexOfNonGap() method successfully completed");
        }

        /// <summary>
        ///     Creates a sparse sequence and validates IndexOfNonGap method
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSparseSequenceIndexOfNonGapNull()
        {
            var sparseSeqObj = new SparseSequence(Alphabets.DNA, 2);
            sparseSeqObj[0] = Alphabets.DNA.Gap;
            sparseSeqObj[1] = Alphabets.DNA.A;

            Assert.AreEqual(1, sparseSeqObj.IndexOfNonGap());

            ApplicationLog.WriteLine("SparseSequenceBVT: Validation of IndexOfNonGap() method successfully completed");
        }

        /// <summary>
        ///     Creates a sparse sequence and validates LastIndexOfNonGap method
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSparseSequenceLastIndexOfNonGapNull()
        {
            var sparseSeqObj = new SparseSequence(Alphabets.DNA, 2);
            sparseSeqObj[1] = Alphabets.DNA.Gap;
            sparseSeqObj[0] = Alphabets.DNA.A;

            Assert.AreEqual(0, sparseSeqObj.LastIndexOfNonGap());
            ApplicationLog.WriteLine("SparseSequenceBVT: Validation of LastIndexOfNonGap() method successfully completed");
        }

        /// <summary>
        ///     Creates a sparse sequence and validates LastIndexOfNonGap method
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSparseSequenceGetReversedSequence()
        {
            byte[] byteArrayObj = Encoding.ASCII.GetBytes("ACGT");

            IEnumerable<byte> seqItems =
                new List<Byte> {byteArrayObj[0], byteArrayObj[1], byteArrayObj[2], byteArrayObj[3]};

            var sparseSeqObj = new SparseSequence(Alphabets.DNA, 0, seqItems);
            ISequence revSeqObj = sparseSeqObj.GetReversedSequence();

            byteArrayObj = Encoding.ASCII.GetBytes("TGCA");

            for (int i = 0; i < byteArrayObj.Length; i++)
            {
                Assert.AreEqual(byteArrayObj[i], revSeqObj[i]);
            }

            ApplicationLog.WriteLine("SparseSequenceBVT: Validation of GetReversedSequence() method successfully completed");
        }

        /// <summary>
        ///     Creates a sparse sequence and validates GetReverseComplementedSequence method
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSparseSequenceGetReversedComplementedSequence()
        {
            byte[] byteArrayObj = Encoding.ASCII.GetBytes("ACGT");

            IEnumerable<byte> seqItems =
                new List<Byte> {byteArrayObj[0], byteArrayObj[1], byteArrayObj[2], byteArrayObj[3]};

            var sparseSeqObj = new SparseSequence(Alphabets.DNA, 0, seqItems);
            ISequence revSeqObj = sparseSeqObj.GetReverseComplementedSequence();

            for (int i = 0; i < byteArrayObj.Length; i++)
            {
                Assert.AreEqual(byteArrayObj[i], revSeqObj[i]);
            }

            ApplicationLog.WriteLine("SparseSequenceBVT: Validation of GetReverseComplementedSequence() method successfully completed");
        }

        /// <summary>
        ///     Creates a sparse sequence and validates GetKnownSequenceItems method
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSparseSequenceGetKnownSequenceItems()
        {
            byte[] byteArrayObj = Encoding.ASCII.GetBytes("ACGT");

            IEnumerable<byte> seqItems =
                new List<Byte> {byteArrayObj[0], byteArrayObj[1], byteArrayObj[2], byteArrayObj[3]};

            var sparseSeqObj = new SparseSequence(Alphabets.DNA, 0, seqItems);
            IReadOnlyList<IndexedItem<byte>> revSeqObj = sparseSeqObj.GetKnownSequenceItems();
            long i = 0;
            foreach (var by in revSeqObj)
            {
                Assert.AreEqual(i, by.Index);
                Assert.AreEqual(byteArrayObj[i], by.Item);
                i++;
            }

            ApplicationLog.WriteLine("SparseSequenceBVT: Validation of GetKnownSequenceItems() method successfully completed");
        }

        /// <summary>
        ///     Creates a sparse sequence and validates GetComplementedSequence method
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSparseSequenceGetComplementedSequence()
        {
            byte[] byteArrayObj = Encoding.ASCII.GetBytes("ACGT");

            IEnumerable<byte> seqItems =
                new List<Byte> {byteArrayObj[0], byteArrayObj[1], byteArrayObj[2], byteArrayObj[3]};

            var sparseSeqObj = new SparseSequence(Alphabets.DNA, 0, seqItems);
            ISequence revSeqObj = sparseSeqObj.GetComplementedSequence();

            byteArrayObj = Encoding.ASCII.GetBytes("TGCA");

            for (int i = 0; i < byteArrayObj.Length; i++)
            {
                Assert.AreEqual(byteArrayObj[i], revSeqObj[i]);
            }

            ApplicationLog.WriteLine("SparseSequenceBVT: Validation of GetComplementedSequence() method successfully completed");
        }

        /// <summary>
        ///     Creates a sparse sequence and validates GetSubSequence method
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSparseSequenceGetSubSequence()
        {
            var byteList = new List<byte>
            {
                Alphabets.DNA.Gap,
                Alphabets.DNA.G,
                Alphabets.DNA.A,
                Alphabets.DNA.Gap,
                Alphabets.DNA.T,
                Alphabets.DNA.C,
                Alphabets.DNA.Gap,
                Alphabets.DNA.Gap
            };

            var sparseSeq = new SparseSequence(Alphabets.DNA, 0, byteList);

            ISequence result = sparseSeq.GetSubSequence(0, 3);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(Alphabets.DNA.Gap, result[0]);
            Assert.AreEqual(Alphabets.DNA.G, result[1]);
            Assert.AreEqual(Alphabets.DNA.A, result[2]);

            result = sparseSeq.GetSubSequence(0, 0);
            Assert.AreEqual(0, result.Count);

            result = sparseSeq.GetSubSequence(3, 2);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(Alphabets.DNA.Gap, result[0]);
            Assert.AreEqual(Alphabets.DNA.T, result[1]);

            ApplicationLog.WriteLine("SparseSequenceBVT: Validation of GetSubSequence() method successfully completed");
        }

        /// <summary>
        ///     Creates a sparse sequence and validates GetEnumerator method
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSparseSequenceGetEnumerator()
        {
            byte[] byteArrayObj = Encoding.ASCII.GetBytes("ACGT");

            IEnumerable<byte> seqItems =
                new List<Byte> {byteArrayObj[0], byteArrayObj[1], byteArrayObj[2], byteArrayObj[3]};

            var sparseSeqObj = new SparseSequence(Alphabets.DNA, 0, seqItems);
            IEnumerator<byte> seqObj = sparseSeqObj.GetEnumerator();
            int i = 0;
            while (seqObj.MoveNext())
            {
                Assert.AreEqual(byteArrayObj[i], seqObj.Current);
                i++;
            }
            i = 0;
            foreach (byte alp in sparseSeqObj)
            {
                Assert.AreEqual(byteArrayObj[i], alp);
                i++;
            }

            ApplicationLog.WriteLine("SparseSequenceBVT: Validation of GetEnumerator() method successfully completed");
        }

        /// <summary>
        ///     Validate by passing indexer value for read only indexer.
        ///     Input Data : Valid Alphabet
        ///     Output Data : Successfully set
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSparseSequenceSetIndexer()
        {
            IAlphabet alphabet = Alphabets.DNA;

            // Create sequence item list
            var sequenceList = new List<byte>();
            foreach (byte item in alphabet)
            {
                sequenceList.Add(item);
            }

            // Store sequence item in sparse sequence object using list of sequence items
            var sparseSeq = new SparseSequence(alphabet, 0, sequenceList);
            byte seqItem = new Sequence(Alphabets.DNA, "AGCT")[0];

            sparseSeq[0] = seqItem;
            Assert.AreEqual(65, sparseSeq[0]);

            ApplicationLog.WriteLine("SparseSequence BVT: Validation of Indexer successfully completed");
        }

        /// <summary>
        ///     Validates CopyTo
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateCopyTo()
        {
            var byteList = new List<byte>
            {
                Alphabets.DNA.Gap,
                Alphabets.DNA.G,
                Alphabets.DNA.A,
                Alphabets.DNA.Gap,
                Alphabets.DNA.T,
                Alphabets.DNA.C,
                Alphabets.DNA.Gap,
                Alphabets.DNA.Gap
            };

            ISequence iSeq = new SparseSequence(Alphabets.DNA, 0, byteList);
            var sparseSeq = new SparseSequence(iSeq);

            var array = new byte[byteList.Count];
            sparseSeq.CopyTo(array, 0, byteList.Count);
            for (int i = 0; i < byteList.Count; i++)
            {
                Assert.AreEqual(byteList.ElementAt(i), array[i]);
            }

            //check for a part of the sequence
            array = new byte[5];
            sparseSeq.CopyTo(array, 0, 5);
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(byteList.ElementAt(i), array[i]);
            }
        }

        #endregion
    }
}