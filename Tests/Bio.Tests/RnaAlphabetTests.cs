using Bio;
using NUnit.Framework;

namespace Bio.Tests
{
    /// <summary>
    /// Tests for the RNA alphabet class.
    /// </summary>
    [TestFixture]
    public class RnaAlphabetTests
    {
        /// <summary>
        /// Test TryGetComplementSymbol of RnaAlphabet
        /// </summary>
        [Test]
        public void TestRnaAlphabetTryGetComplementSymbol()
        {
            byte basicSymbols;
            RnaAlphabet rnaAlphabet = RnaAlphabet.Instance;

            Assert.AreEqual(true, rnaAlphabet.TryGetComplementSymbol((byte)'A', out basicSymbols));
            Assert.AreEqual('U', (char)basicSymbols);

            Assert.AreEqual(true, rnaAlphabet.TryGetComplementSymbol((byte)'U', out basicSymbols));
            Assert.AreEqual('A', (char)basicSymbols);

            Assert.AreEqual(true, rnaAlphabet.TryGetComplementSymbol((byte)'G', out basicSymbols));
            Assert.AreEqual('C', (char)basicSymbols);

            Assert.AreEqual(true, rnaAlphabet.TryGetComplementSymbol((byte)'C', out basicSymbols));
            Assert.AreEqual('G', (char)basicSymbols);
        }

        /// <summary>
        /// Test CompareSymbols method of RnaAlphabet
        /// </summary>
        [Test]
        public void TestRnaAlphabetCompareSymbols()
        {
            RnaAlphabet rnaAlphabet = RnaAlphabet.Instance;

            Assert.AreEqual(true, rnaAlphabet.CompareSymbols((byte)'A', (byte)'A'));

            Assert.AreEqual(true, rnaAlphabet.CompareSymbols((byte)'U', (byte)'U'));

            Assert.AreEqual(true, rnaAlphabet.CompareSymbols((byte)'G', (byte)'G'));

            Assert.AreEqual(true, rnaAlphabet.CompareSymbols((byte)'C', (byte)'C'));

            Assert.AreEqual(false, rnaAlphabet.CompareSymbols((byte)'A', (byte)'U'));

            Assert.AreEqual(false, rnaAlphabet.CompareSymbols((byte)'A', (byte)'G'));

            Assert.AreEqual(false, rnaAlphabet.CompareSymbols((byte)'A', (byte)'C'));

            Assert.AreEqual(false, rnaAlphabet.CompareSymbols((byte)'U', (byte)'A'));

            Assert.AreEqual(false, rnaAlphabet.CompareSymbols((byte)'U', (byte)'G'));

            Assert.AreEqual(false, rnaAlphabet.CompareSymbols((byte)'U', (byte)'C'));

            Assert.AreEqual(false, rnaAlphabet.CompareSymbols((byte)'G', (byte)'A'));

            Assert.AreEqual(false, rnaAlphabet.CompareSymbols((byte)'G', (byte)'U'));

            Assert.AreEqual(false, rnaAlphabet.CompareSymbols((byte)'G', (byte)'C'));

            Assert.AreEqual(false, rnaAlphabet.CompareSymbols((byte)'C', (byte)'A'));

            Assert.AreEqual(false, rnaAlphabet.CompareSymbols((byte)'C', (byte)'U'));

            Assert.AreEqual(false, rnaAlphabet.CompareSymbols((byte)'C', (byte)'G'));
        }
    }
}
