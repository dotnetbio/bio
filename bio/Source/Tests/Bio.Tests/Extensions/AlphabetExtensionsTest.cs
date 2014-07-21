using Bio.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Bio;

namespace Bio.Tests.Extensions
{
    /// <summary>
    ///This is a test class for AlphabetExtensionsTest and is intended
    ///to contain all AlphabetExtensionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AlphabetExtensionsTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        /// Test the CheckIsAmbiguous extension with a valid ambiguous value.
        ///</summary>
        [TestMethod()]
        public void CheckIsAmbiguousTrueTest()
        {
            IAlphabet alphabet = Alphabets.AmbiguousDNA;
            bool actual = alphabet.CheckIsAmbiguous('M');
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// Test the CheckIsAmbiguous extension with lower-case ambiguous value.
        ///</summary>
        [TestMethod()]
        public void CheckIsAmbiguousLowercaseTrueTest()
        {
            IAlphabet alphabet = Alphabets.AmbiguousDNA;
            bool actual = alphabet.CheckIsAmbiguous('m');
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// Test the CheckIsAmbiguous extension with non-ambiguous value.
        ///</summary>
        [TestMethod()]
        public void CheckIsAmbiguousFalseTest()
        {
            IAlphabet alphabet = Alphabets.AmbiguousDNA;
            bool actual = alphabet.CheckIsAmbiguous('A');
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        /// Test the CheckIsAmbiguous extension method with an alphabet that does not support ambiguous values.
        /// </summary>
        [TestMethod()]
        public void CheckIsAmbiguousUnsupportedTest()
        {
            IAlphabet alphabet = Alphabets.DNA;
            bool actual = alphabet.CheckIsAmbiguous('A');
            Assert.AreEqual(false, actual);
        }

        ///<summary>
        /// Test the CheckIsAmbiguous extension method with a null sequence.
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckIsAmbiguousNullSequence()
        {
            IAlphabet alphabet = null;
            alphabet.CheckIsAmbiguous('A');
        }

        /// <summary>
        /// Test the CheckIsGap extension method with a valid gap.
        /// </summary>
        [TestMethod()]
        public void CheckIsGapTrueTest()
        {
            IAlphabet alphabet = Alphabets.DNA;
            bool actual = alphabet.CheckIsGap('-');
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// Test the CheckIsGap extension method with a non-gap.
        ///</summary>
        [TestMethod()]
        public void CheckIsGapFalseTest()
        {
            IAlphabet alphabet = Alphabets.DNA;
            bool actual = alphabet.CheckIsGap('A');
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        /// Test the CheckIsGap extension method with a null input sequence.
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckIsGapNullSequence()
        {
            IAlphabet alphabet = null;
            alphabet.CheckIsGap('A');
        }

        /// <summary>
        /// Test for CheckIsTermination extension method with a valid termination.
        ///</summary>
        [TestMethod()]
        public void CheckIsTerminationTrueTest()
        {
            IAlphabet alphabet = Alphabets.Protein;
            bool actual = alphabet.CheckIsTermination('*');
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// Test for CheckIsTermination extension method with a normal character (non-terminator).
        ///</summary>
        [TestMethod()]
        public void CheckIsTerminationFalseTest()
        {
            IAlphabet alphabet = Alphabets.Protein;
            bool actual = alphabet.CheckIsTermination('A');
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        /// Test for CheckIsTermination extension method with an unsupported alphabet.
        ///</summary>
        [TestMethod()]
        public void CheckIsTerminationUnsupportedTest()
        {
            IAlphabet alphabet = Alphabets.DNA;
            bool actual = alphabet.CheckIsTermination('A');
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        /// Test for CheckIsTermination extension method with a null alphabet.
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckIsTerminationMissingAlphabetTest()
        {
            IAlphabet alphabet = null;
            alphabet.CheckIsTermination('A');
        }

        /// <summary>
        /// Test for GetFriendlyName
        ///</summary>
        [TestMethod()]
        public void GetFriendlyNameTest()
        {
            IAlphabet alphabet = Alphabets.DNA;
            string actual = alphabet.GetFriendlyName('A');
            Assert.AreEqual("Adenine", actual);
        }

        /// <summary>
        /// Test for GetFriendlyName with a lower case input
        ///</summary>
        [TestMethod()]
        public void GetFriendlyNameLowecaseTest()
        {
            IAlphabet alphabet = Alphabets.DNA;
            string actual = alphabet.GetFriendlyName('a');
            Assert.AreEqual("Adenine", actual);
        }


        /// <summary>
        /// Test for GetFriendlyName with a null input alphabet.
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFriendlyNameTestNullAlphabet()
        {
            IAlphabet alphabet = null;
            alphabet.GetFriendlyName('A');
        }
    }
}
