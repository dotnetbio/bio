using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Bio.Tests
{
    /// <summary>
    /// Summary description for TestSequencesCopyToForBio
    /// </summary>
    [TestFixture]
    public class TestSequencesCopyToForBio
    {
        /// <summary>
        /// Test for Sequence CopyTo method.
        /// </summary>
        [Test]
        public void TestSequenceCopyTo()
        {
            Sequence seq = new Sequence(Alphabets.DNA, "ATCG");
            byte[] array = new byte[2];
            seq.CopyTo(array, 1, 2);
            string expectedValue = "TC";
            StringBuilder b = new StringBuilder();
            b.Append((char)array[0]);
            b.Append((char)array[1]);
            string actualValue = b.ToString();
            Assert.AreEqual(expectedValue, actualValue);
        }

        /// <summary>
        /// Test for DeriveSequence CopyTo method.
        /// </summary>
        [Test]
        public void TestDeriveSequenceCopyTo()
        {
            ISequence seq = new Sequence(Alphabets.DNA, "ATCG");
            DerivedSequence derSeq = new DerivedSequence(seq, false, true);
            byte[] array = new byte[2];
            derSeq.CopyTo(array, 1, 2);
            string expectedValue = "AG";
            StringBuilder b = new StringBuilder();
            b.Append((char)array[0]);
            b.Append((char)array[1]);
            string actualValue = b.ToString();
            Assert.AreEqual(expectedValue, actualValue);
        }

        /// <summary>
        /// Test for DeriveSequence CopyTo method.
        /// </summary>
        [Test]
        public void TestSparseSequenceCopyTo()
        {
            IEnumerable<byte> seqItems = new List<Byte>() { 65, 65, 67, 67 };
            SparseSequence sparseSeq = new SparseSequence(Alphabets.DNA, 0, seqItems);
            byte[] array = new byte[2];
            sparseSeq.CopyTo(array, 1, 2);
            string expectedValue = "AC";
            StringBuilder b = new StringBuilder();
            b.Append((char)array[0]);
            b.Append((char)array[1]);
            string actualValue = b.ToString();
            Assert.AreEqual(expectedValue, actualValue);
        }

        /// <summary>
        /// Test for Sequence ConstructorWithISequenceArgument method.
        /// </summary>
        [Test]
        public void TestSequenceConstructorWithISequenceArgument()
        {
            ISequence seq = new Sequence(Alphabets.DNA, "ATCG");
            ISequence newSeq = new Sequence(seq);
            string expectedSequence = "ATCG";
            string actualSequence = new string(newSeq.Select(x => (char)x).ToArray());
            Assert.AreEqual(expectedSequence, actualSequence);
        }

        /// <summary>
        /// Test for DeriveSequence ConstructorWithSequenceArgument method.
        /// </summary>
        [Test]
        public void TestSparseSequenceConstructorWithSequenceArgument()
        {
            IEnumerable<byte> seqItems = new List<Byte>() { 65, 65, 67, 67 };
            SparseSequence sparseSeq = new SparseSequence(Alphabets.DNA, 0, seqItems);
            ISequence seq = sparseSeq.GetSubSequence(0, sparseSeq.Count);
            SparseSequence newSparseSequence = new SparseSequence(seq);
            ISequence newSeq = newSparseSequence.GetSubSequence(0, newSparseSequence.Count);

            string expectedValue = "AACC";
            string actualValue = new string(newSeq.Select(x => (char)x).ToArray());
            Assert.AreEqual(expectedValue, actualValue);
        }
    }
}
