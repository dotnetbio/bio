using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio;

namespace Bio.Tests
{
    /// <summary>
    /// Tests the AmbiguousRnaAlphabet class.
    /// </summary>
    [TestClass]
    public class AmbiguousAmbiguousRnaAlphabetTests
    {
        /// <summary>
        /// Tests the AmbiguousRNAAlphabet class.
        /// </summary>
        [TestMethod]
        public void TestAmbiguousRnaAlphabetCompareSymbols()
        {
            AmbiguousRnaAlphabet ambiguousRnaAlphabet = AmbiguousRnaAlphabet.Instance;
            Assert.AreEqual(false, ambiguousRnaAlphabet.CompareSymbols((byte)'A', (byte)'M'));
            Assert.AreEqual(true, ambiguousRnaAlphabet.CompareSymbols((byte)'A', (byte)'A'));
        }
    }
}
