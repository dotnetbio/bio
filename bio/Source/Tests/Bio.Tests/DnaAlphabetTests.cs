using System.Collections.Generic;
using System.Linq;
using Bio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests
{
    /// <summary>
    /// Tests DnaAlphabet class.
    /// </summary>
    [TestClass]
    public class DnaAlphabetTests
    {
        #region DnaAlphabet Test Cases

        /// <summary>
        /// Test TryGetComplementSymbol method
        /// </summary>
        [TestMethod]
        public void TestDnaAlphabetTryGetComplementSymbol()
        {
            byte basicSymbols;
            DnaAlphabet dnaAlphabet = DnaAlphabet.Instance;
            
            Assert.AreEqual(true, dnaAlphabet.TryGetComplementSymbol((byte)'A', out basicSymbols));            
            Assert.AreEqual('T', (char)basicSymbols);

            Assert.AreEqual(true, dnaAlphabet.TryGetComplementSymbol((byte)'T', out basicSymbols));
            Assert.AreEqual('A', (char)basicSymbols);

            Assert.AreEqual(true, dnaAlphabet.TryGetComplementSymbol((byte)'G', out basicSymbols));
            Assert.AreEqual('C', (char)basicSymbols);

            Assert.AreEqual(true, dnaAlphabet.TryGetComplementSymbol((byte)'C', out basicSymbols));
            Assert.AreEqual('G', (char)basicSymbols);
            Assert.AreEqual('G', (char)dnaAlphabet.GetSymbolValueMap()[(byte)'g']);
            Assert.IsTrue(dnaAlphabet.CompareSymbols((byte)'T', (byte)'t'));
            Assert.IsTrue(dnaAlphabet.CompareSymbols((byte)'t', (byte)'T'));
            Assert.AreEqual(dnaAlphabet.GetAmbiguousSymbols().Count, 0);
        }

        /// <summary>
        /// Test TryGetBasicSymbols method
        /// </summary>
        [TestMethod]
        public void TestDnaAlphabetTryGetBasicSymbols()
        {
            HashSet<byte> basicSymbols;
            AmbiguousDnaAlphabet dnaAlphabet = AmbiguousDnaAlphabet.Instance;
            
            Assert.AreEqual(true, dnaAlphabet.TryGetBasicSymbols((byte)'M', out basicSymbols));
            Assert.IsTrue(basicSymbols.All(sy => (sy == (byte)'A' || sy == (byte) 'C')));            
        }

        #endregion DnaAlphabet Test Cases
    }
}
