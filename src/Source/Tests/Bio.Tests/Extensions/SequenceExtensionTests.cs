using System;
using Bio.Extensions;
using NUnit.Framework;

namespace Bio.Tests.Extensions
{
    /// <summary>
    /// This class is used to test various extensions on the
    /// Sequence and ISequence types.
    /// </summary>
    [TestFixture]
    public class SequenceExtensionTests
    {
        private const string SmallSequence = "AAAA----CCCC----GGGG----TTTT";

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
        /// This method retrieves the string for a valid sequence
        /// with no options.
        /// </summary>
        [Test]
        public void TryToGetDefaultValidSequence()
        {
            ISequence sequence = new Sequence(DnaAlphabet.Instance, SmallSequence);
            string theString = sequence.ConvertToString();
            Assert.AreEqual(SmallSequence, theString);
        }

        /// <summary>
        /// This method retrieves the string for a valid sequence
        /// with no options.
        /// </summary>
        [Test]
        public void TryToGetSubstringValidSequence()
        {
            ISequence sequence = new Sequence(DnaAlphabet.Instance, SmallSequence);
            string theString = sequence.ConvertToString(2,4);
            Assert.AreEqual(SmallSequence.Substring(2,4), theString);
        }

        /// <summary>
        /// This method retrieves the string for a valid sequence
        /// with no options.
        /// </summary>
        [Test]
        public void TryToGetEndValidSequence()
        {
            ISequence sequence = new Sequence(DnaAlphabet.Instance, SmallSequence);
            string theString = sequence.ConvertToString(5);
            Assert.AreEqual(SmallSequence.Substring(5), theString);
        }

        /// <summary>
        /// This method retrieves the string for a valid sequence
        /// with no options.
        /// </summary>
        [Test]
        public void CheckForNullException()
        {
            ISequence sequence = null;
            Assert.Throws<ArgumentNullException>( () => sequence.ConvertToString());
        }

        /// <summary>
        /// This method retrieves the string for a valid sequence
        /// with no options.
        /// </summary>
        [Test]
        public void CheckForOutOfRangeExceptionOnInvalidStartIndex()
        {
            ISequence sequence = new Sequence(DnaAlphabet.Instance, "-");
            Assert.Throws<ArgumentOutOfRangeException> ( () =>  sequence.ConvertToString(2,10));
        }

        /// <summary>
        /// This method retrieves the string for a valid sequence
        /// with no options.
        /// </summary>
        [Test]
        public void CheckForOutOfRangeExceptionOnInvalidLength()
        {
            ISequence sequence = new Sequence(DnaAlphabet.Instance, "-");
            Assert.Throws<ArgumentOutOfRangeException> ( () =>  sequence.ConvertToString(0, 10));
        }

    }
}
