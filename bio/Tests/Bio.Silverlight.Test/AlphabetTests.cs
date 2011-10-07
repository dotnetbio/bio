using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Silverlight.Test
{
    [TestClass]
    public class AlphabetTests
    {
        /// <summary>
        /// Test TryGetBasicSymbols method
        /// </summary>
        [TestMethod]
        public void TestDnaAlphabetTryGetAmbiguousSymbols()
        {
            byte basicSymbol;
            IAlphabet alphabet = AmbiguousDnaAlphabet.Instance;
            Assert.AreEqual(true, alphabet.TryGetAmbiguousSymbol(new HashSet<byte>() {(byte)'A', (byte)'C'}, out basicSymbol));
            Assert.IsTrue(basicSymbol == (byte)'M');

            alphabet = AmbiguousRnaAlphabet.Instance;
            Assert.AreEqual(true, alphabet.TryGetAmbiguousSymbol(new HashSet<byte>() { (byte)'U', (byte)'C' }, out basicSymbol));
            Assert.IsTrue(basicSymbol == (byte)'Y');
        }

        /// <summary>
        /// Test TryGetBasicSymbols method
        /// </summary>
        [TestMethod]
        public void TestRnaAlphabetTryGetAmbiguousSymbols()
        {
            byte basicSymbol;
            IAlphabet alphabet = AmbiguousRnaAlphabet.Instance;
            Assert.AreEqual(true, alphabet.TryGetAmbiguousSymbol(new HashSet<byte>() { (byte)'U', (byte)'C' }, out basicSymbol));
            Assert.IsTrue(basicSymbol == (byte)'Y');
        }

        /// <summary>
        /// Test TryGetBasicSymbols method
        /// </summary>
        [TestMethod]
        public void TestProteinAlphabetTryGetAmbiguousSymbols()
        {
            byte basicSymbol;
            IAlphabet alphabet = AmbiguousProteinAlphabet.Instance;
            Assert.AreEqual(true, alphabet.TryGetAmbiguousSymbol(new HashSet<byte>() { (byte)'D', (byte)'N' }, out basicSymbol));
            Assert.IsTrue(basicSymbol == (byte)'B');
        }
    }
}
