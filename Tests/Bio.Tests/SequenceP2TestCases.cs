using System;

using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests
{
    /// <summary>
    ///     Test Automation code for Bio Sequence P2 level validations..
    /// </summary>
    [TestFixture]
    public class SequenceP2TestCases
    {
        private readonly Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #region P2 Test Cases

        /// <summary>
        ///     Invalidates CopyTo
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateCopyTo()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(Constants.RnaDerivedSequenceNode,
                                                                      Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(Constants.RnaDerivedSequenceNode,
                                                                  Constants.AlphabetNameNode);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            // Create a Sequence object.
            ISequence iseqObj =
                new Sequence(alphabet, expectedSequence);
            var seqObj = new Sequence(iseqObj);
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
    }
}