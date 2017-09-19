using System;
using System.Linq;
using System.Text;

using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests
{
    /// <summary>
    /// Bvt test cases to confirm the features of Derived Sequence
    /// </summary>
    [TestFixture]
    public class DerivedSequenceBvtTestCases
    {
        readonly Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #region Test Cases

        /// <summary>
        /// Creates a dna derived sequence after adding and removing few items from original sequence.
        /// Validates it against expected sequence. 
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateDnaDerivedSequence()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.AlphabetNameNode);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            // Create derived Sequence
            DerivedSequence derSequence = CreateDerivedSequence(
                alphabet, expectedSequence);

            // Validate derived Sequence.
            Assert.AreEqual(expectedSequence, new string(derSequence.Select(a => (char)a).ToArray()));

            ApplicationLog.WriteLine(
                "DerivedSequenceBvtTestCases:Validation of derived sequence completed successfully");
        }

        /// <summary>
        /// Creates a dna derived sequence using IndexOfNonGap() method.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateDnaDerivedSequenceIndexOfNonGap()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.AlphabetNameNode);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            // Create derived Sequence
            DerivedSequence derSequence = CreateDerivedSequence(
                alphabet, expectedSequence);

            try
            {
                derSequence.IndexOfNonGap();
                Assert.Fail("The method is now implemented");
            }
            catch (NotImplementedException)
            {
                ApplicationLog.WriteLine(
                    "DerivedSequenceBvtTestCases:Validation of IndexOfNonGap() method of derived sequence completed successfully");
            }
        }

        /// <summary>
        /// Creates a dna derived sequence using IndexOfNonGap(int) method.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateDnaDerivedSequenceIndexOfNonGapInt()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.AlphabetNameNode);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            // Create derived Sequence
            DerivedSequence derSequence = CreateDerivedSequence(
                alphabet, expectedSequence);

            try
            {
                derSequence.IndexOfNonGap(0);
                Assert.Fail("The method is now implemented");
            }
            catch (NotImplementedException)
            {
                ApplicationLog.WriteLine(
                    "DerivedSequenceBvtTestCases:Validation of IndexOfNonGap(int) method of derived sequence completed successfully");
            }
        }

        /// <summary>
        /// Creates a dna derived sequence using LastIndexOfNonGap() method.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateDnaDerivedSequenceLastIndexOfNonGap()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.AlphabetNameNode);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            // Create derived Sequence
            DerivedSequence derSequence = CreateDerivedSequence(
                alphabet, expectedSequence);

            try
            {
                derSequence.LastIndexOfNonGap();
                Assert.Fail("The method is now implemented");
            }
            catch (NotImplementedException)
            {
                ApplicationLog.WriteLine(
                    "DerivedSequenceBvtTestCases:Validation of LastIndexOfNonGap() method of derived sequence completed successfully");
            }
        }

        /// <summary>
        /// Creates a dna derived sequence using LastIndexOfNonGap(int) method.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateDnaDerivedSequenceLastIndexOfNonGapInt()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.AlphabetNameNode);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            // Create derived Sequence
            DerivedSequence derSequence = CreateDerivedSequence(
                alphabet, expectedSequence);

            try
            {
                derSequence.LastIndexOfNonGap(derSequence.Count - 1);
                Assert.Fail("The method is now implemented");
            }
            catch (NotImplementedException)
            {
                ApplicationLog.WriteLine(
                    "DerivedSequenceBvtTestCases:Validation of LastIndexOfNonGap(int) method of derived sequence completed successfully");
            }
        }

        /// <summary>
        /// Creates a dna derived sequence and validates GetReverseComplementedSequence() method.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateDnaDerivedSequenceGetReverseComplemented()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.AlphabetNameNode);
            string reverseCompObj = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.ReverseComplement);
            string derivedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.DerivedSequence);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            // Create derived Sequence
            DerivedSequence derSequence = CreateDerivedSequence(
                alphabet, expectedSequence);

            // Validate IndexOf() derived Sequence.
            Assert.AreEqual(derivedSequence, new string(derSequence.Select(a=>(char)a).ToArray()));
            Assert.AreEqual(reverseCompObj, new string(derSequence.GetReverseComplementedSequence().Select(a => (char)a).ToArray()));

            ApplicationLog.WriteLine(
                "DerivedSequenceBvtTestCases:Validation of GetReverseComplementedSequence() method of derived sequence completed successfully");
        }

        /// <summary>
        /// Creates a dna derived sequence and validates GetReversedSequence() method.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateDnaDerivedSequenceGetReversed()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.AlphabetNameNode);
            string reverseObj = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.Reverse);
            string derivedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.DerivedSequence);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            // Create derived Sequence
            DerivedSequence derSequence = CreateDerivedSequence(
                alphabet, expectedSequence);

            // Validate IndexOf() derived Sequence.
            Assert.AreEqual(derivedSequence, new string(derSequence.Select(a=>(char)a).ToArray()));
            Assert.AreEqual(reverseObj, new string(derSequence.GetReversedSequence().Select(a => (char)a).ToArray()));

            ApplicationLog.WriteLine(
                "DerivedSequenceBvtTestCases:Validation of GetReversedSequence() method of derived sequence completed successfully");
        }

        /// <summary>
        /// Creates a dna derived sequence and validates GetComplementedSequence() method.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateDnaDerivedSequenceGetComplemented()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.AlphabetNameNode);
            string complementObj = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.Complement);
            string derivedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.DerivedSequence);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            // Create derived Sequence
            DerivedSequence derSequence = CreateDerivedSequence(
                alphabet, expectedSequence);

            // Validate IndexOf() derived Sequence.
            Assert.AreEqual(derivedSequence, new string(derSequence.Select(a=>(char)a).ToArray()));
            Assert.AreEqual(complementObj, new string(derSequence.GetComplementedSequence().Select(a => (char)a).ToArray()));

            ApplicationLog.WriteLine(
                "DerivedSequenceBvtTestCases:Validation of GetComplementedSequence() method of derived sequence completed successfully");
        }

        /// <summary>
        /// Creates a dna derived sequence and validates GetSubSequence() method.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateDnaDerivedSequenceGetSubSequence()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.AlphabetNameNode);
            string rangeObj = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.Range);
            string expSubSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.RangeSequence);
            string derivedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.DerivedSequence);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            // Create derived Sequence
            DerivedSequence derSequence = CreateDerivedSequence(
                alphabet, expectedSequence);
            string[] ranges = rangeObj.Split(',');

            // Validate IndexOf() derived Sequence.
            Assert.AreEqual(derivedSequence, new string(derSequence.Select(a=>(char)a).ToArray()));
            Assert.AreEqual(expSubSequence, new string(derSequence.GetSubSequence(long.Parse(ranges[0],
                (IFormatProvider)null), long.Parse(ranges[1], (IFormatProvider)null)).Select(a => (char)a).ToArray()));

            ApplicationLog.WriteLine(
                "DerivedSequenceBvtTestCases:Validation of GetSubSequence() method of derived sequence completed successfully");
        }

        /// <summary>
        /// Creates a dna derived sequence after adding and removing few items from original sequence.
        /// Validates properties of derived sequence.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateDnaDerivedSequenceProperties()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.AlphabetNameNode);
            string derivedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.DerivedSequence);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            // Create derived Sequence
            DerivedSequence derSequence = CreateDerivedSequence(
                alphabet, expectedSequence);

            // Validate properties of derived Sequence.
            Assert.AreEqual(derivedSequence, new string(derSequence.Select(a=>(char)a).ToArray()));
            Assert.AreEqual(alphabet, derSequence.Alphabet);
            Assert.IsNotNull(derSequence.Metadata);

            ApplicationLog.WriteLine(
                "DerivedSequenceBvtTestCases:Validation of properties of derived sequence completed successfully");
        }

        /// <summary>
        /// Validates CopyTo
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateCopyTo()
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
            byte[] array = new byte[expectedSequence.Length];
            seqObj.CopyTo(array, 0, expectedSequence.Length);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < expectedSequence.Length; i++)
            {
                builder.Append((char)array[i]);
            }
            string actualValue = builder.ToString();
            Assert.AreEqual(expectedSequence, actualValue);

            //check with a part of the expected seq only
            seqObj.CopyTo(array, 0, 5);
            builder = new StringBuilder();
            for (int i = 0; i < 5; i++)
            {
                builder.Append((char)array[i]);
            }
            actualValue = builder.ToString();
            Assert.AreEqual(expectedSequence.Substring(0, 5), actualValue);
        }

        #endregion

        #region Helper Methods

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

        #endregion
    }
}
