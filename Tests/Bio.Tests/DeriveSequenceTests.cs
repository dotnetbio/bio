using System.Linq;
using NUnit.Framework;

namespace Bio.Tests
{
    /// <summary>
    /// Tests for derived sequences.
    /// </summary>
    [TestFixture]
    public class DeriveSequenceTests
    {
        /// <summary>
        /// Get Reversed Sequence.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestGetReversedSequence()
        {
            const string sequence = "ATGCC";
            const string expectedSequence = "CCGTA";
            ISequence orignalSequence = new Sequence(Alphabets.DNA, sequence);
            DerivedSequence deriveSequence = new DerivedSequence(orignalSequence, false, false);
            string actualSequence = new string(deriveSequence.GetReversedSequence().Select(a => (char)a).ToArray());
            Assert.AreEqual(expectedSequence, actualSequence);
        }

        /// <summary>
        /// Get Complemented Sequence.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestGetComplementedSequence()
        {
            const string sequence = "ATGCC";
            const string expectedSequence = "TACGG";
            ISequence orignalSequence = new Sequence(Alphabets.DNA, sequence);
            DerivedSequence deriveSequence = new DerivedSequence(orignalSequence, false, false);
            string actualSequence = new string(deriveSequence.GetComplementedSequence().Select(a => (char)a).ToArray());
            Assert.AreEqual(expectedSequence, actualSequence);
        }

        /// <summary>
        /// Get Reverse Complemented Sequence.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestGetReverseComplementedSequence()
        {
            const string sequence = "ATGCC";
            const string expectedSequence = "GGCAT";
            ISequence orignalSequence = new Sequence(Alphabets.DNA, sequence);
            DerivedSequence deriveSequence = new DerivedSequence(orignalSequence, false, false);
            string actualSequence = new string(deriveSequence.GetReverseComplementedSequence().Select(a => (char)a).ToArray());
            Assert.AreEqual(expectedSequence, actualSequence);
        }
    }
}
