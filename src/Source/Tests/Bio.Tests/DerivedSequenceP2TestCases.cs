using System;

using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests
{
    /// <summary>
    /// Test Automation code for Bio Derived Sequence P2 level validations..
    /// </summary>
    [TestFixture]
    public class DerivedSequenceP2TestCases
    {
        readonly Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #region P2 Test Cases

        /// <summary>
        /// Invalidates CopyTo
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateCopyTo()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.AlphabetNameNode);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            // Create derived Sequence
            DerivedSequence seqObj = CreateDerivedSequence(
                alphabet, expectedSequence);

            //check with null array
            byte[] array = null;
            try
            {
                seqObj.CopyTo(array, 10, 20);
                Assert.Fail();
            }
            catch (ArgumentNullException anex)
            {
                ApplicationLog.WriteLine("Successfully caught ArgumentNullException : " + anex.Message);
            }

            //check with more than available length
            array = new byte[expectedSequence.Length];
            try
            {
                seqObj.CopyTo(array, 0, expectedSequence.Length + 100);
                Assert.Fail();
            }
            catch (ArgumentException aex)
            {
                ApplicationLog.WriteLine("Successfully caught ArgumentException : " + aex.Message);
            }

            //check with negative start
            array = new byte[expectedSequence.Length];
            try
            {
                seqObj.CopyTo(array, -5, expectedSequence.Length);
                Assert.Fail();
            }
            catch (ArgumentException aex)
            {
                ApplicationLog.WriteLine("Successfully caught ArgumentException : " + aex.Message);
            }

            //check with negative count
            array = new byte[expectedSequence.Length];
            try
            {
                seqObj.CopyTo(array, 0, -5);
                Assert.Fail();
            }
            catch (ArgumentException aex)
            {
                ApplicationLog.WriteLine("Successfully caught ArgumentException : " + aex.Message);
            }

        }
        #endregion P2 Test Cases

        #region Supporting methods

        /// <summary>
        /// Creates a dna derived sequence after adding and removing few items from original sequence.
        /// </summary>
        /// <param name="alphabet">Alphabet</param>
        /// <param name="source">source sequence</param>
        private static DerivedSequence CreateDerivedSequence(
            IAlphabet alphabet, string source)
        {
            ISequence seq = new Sequence(alphabet, source);
            DerivedSequence derSequence = new DerivedSequence(seq, false, false);

            return derSequence;
        }
        #endregion Supporting methods
    }
}


