using Bio;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests
{
    /// <summary>
    /// PatternConverter Test cases
    /// </summary>
    [TestClass]
    public class SequenceTests
    {
        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static void TestSequenceFindMatch()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("Bio.Tests.log");
            }
        }

        /// <summary>
        /// Find pattern test.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestMultipleFindOneOutputPattern()
        {
            byte[] b = new byte[6];
            b[0] = (byte)'C';
            b[1] = (byte)'A';
            b[2] = (byte)'A';
            b[3] = (byte)'G';
            b[4] = (byte)'C';
            b[5] = (byte)'T';

            string expectedSequence = "CAAGCT";
            ISequence sequence = new Sequence(Alphabets.DNA, b);

            string actual = "";
            foreach (byte bt in sequence)
            {
                actual += (char)bt;
            }
            Assert.AreEqual(expectedSequence, actual);

            Assert.AreEqual(sequence.Alphabet, Alphabets.DNA);
            Assert.AreEqual(sequence.Count, 6);
            // 
            // Test for indexer
            Assert.AreEqual(sequence[0], (byte)'C');
            Assert.AreEqual(sequence[1], (byte)'A');
            Assert.AreEqual(sequence[2], (byte)'A');
            Assert.AreEqual(sequence[3], (byte)'G');
            Assert.AreEqual(sequence[4], (byte)'C');
            Assert.AreEqual(sequence[5], (byte)'T');

        }

        /// <summary>
        /// Find pattern test.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestSequenceCTor()
        {
            byte[] b = new byte[6];
            b[0] = (byte)'C';
            b[1] = (byte)'A';
            b[2] = (byte)'A';
            b[3] = (byte)'G';
            b[4] = (byte)'C';
            b[5] = (byte)'T';

            string expectedSequence = "CAAGCT";
           ISequence sequence = new Sequence(Alphabets.DNA, b);

            string actual = "";
            foreach (byte bt in sequence)
            {
                actual += (char)bt;
            }
            Assert.AreEqual(expectedSequence, actual);

            Assert.AreEqual(sequence.Alphabet,Alphabets.DNA);
            Assert.AreEqual(sequence.Count, 6);
            // 
            // Test for indexer
            Assert.AreEqual(sequence[0], (byte)'C');
            Assert.AreEqual(sequence[1], (byte)'A');
            Assert.AreEqual(sequence[2], (byte)'A');
            Assert.AreEqual(sequence[3], (byte)'G');
            Assert.AreEqual(sequence[4], (byte)'C');
            Assert.AreEqual(sequence[5], (byte)'T');

        }
   }
}
