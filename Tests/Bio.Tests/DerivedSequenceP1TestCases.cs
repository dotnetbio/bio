using System;
using System.Linq;

using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests
{
    /// <summary>
    /// P1 test cases to confirm the features of Derived Sequence
    /// </summary>
    [TestFixture]
    public class DerivedSequenceP1TestCases
    {
        readonly Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #region Test Cases

        /// <summary>
        /// Creates a Rna derived sequence after adding and removing few items from original sequence.
        /// Validates it against expected sequence. 
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateRnaDerivedSequence()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.AlphabetNameNode);
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
        /// Creates a Rna derived sequence using IndexOfNonGap() method.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateRnaDerivedSequenceIndexOfNonGap()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.AlphabetNameNode);
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
        /// Creates a Rna derived sequence using IndexOfNonGap(int) method.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateRnaDerivedSequenceIndexOfNonGapInt()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.AlphabetNameNode);
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
        /// Creates a Rna derived sequence using LastIndexOfNonGap() method.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateRnaDerivedSequenceLastIndexOfNonGap()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.AlphabetNameNode);
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
        /// Creates a Rna derived sequence using LastIndexOfNonGap(int) method.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateRnaDerivedSequenceLastIndexOfNonGapInt()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.AlphabetNameNode);
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
        /// Creates a Rna derived sequence and validates GetReverseComplementedSequence() method.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateRnaDerivedSequenceGetReverseComplemented()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.AlphabetNameNode);
            string reverseCompObj = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.ReverseComplement);
            string derivedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.DerivedSequence);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            // Create derived Sequence
            DerivedSequence derSequence = CreateDerivedSequence(
                alphabet, expectedSequence);

            // Validate IndexOf() derived Sequence.
            Assert.AreEqual(derivedSequence, new string(derSequence.Select(a => (char)a).ToArray()));
            Assert.AreEqual(reverseCompObj, new string(derSequence.GetReverseComplementedSequence().Select(a => (char)a).ToArray()));

            ApplicationLog.WriteLine(
                "DerivedSequenceBvtTestCases:Validation of GetReverseComplementedSequence() method of derived sequence completed successfully");
        }

        /// <summary>
        /// Creates a Rna derived sequence and validates GetReversedSequence() method.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateRnaDerivedSequenceGetReversed()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.AlphabetNameNode);
            string reverseObj = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.Reverse);
            string derivedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.DerivedSequence);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            // Create derived Sequence
            DerivedSequence derSequence = CreateDerivedSequence(
                alphabet, expectedSequence);

            // Validate IndexOf() derived Sequence.
            Assert.AreEqual(derivedSequence, new string(derSequence.Select(a => (char)a).ToArray()));
            Assert.AreEqual(reverseObj, new string(derSequence.GetReversedSequence().Select(a => (char)a).ToArray()));

            ApplicationLog.WriteLine(
                "DerivedSequenceBvtTestCases:Validation of GetReversedSequence() method of derived sequence completed successfully");
        }

        /// <summary>
        /// Creates a Rna derived sequence and validates GetComplementedSequence() method.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateRnaDerivedSequenceGetComplemented()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.AlphabetNameNode);
            string complementObj = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.Complement);
            string derivedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.DerivedSequence);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            // Create derived Sequence
            DerivedSequence derSequence = CreateDerivedSequence(
                alphabet, expectedSequence);

            // Validate IndexOf() derived Sequence.
            Assert.AreEqual(derivedSequence, new string(derSequence.Select(a => (char)a).ToArray()));
            Assert.AreEqual(complementObj, new string(derSequence.GetComplementedSequence().Select(a => (char)a).ToArray()));

            ApplicationLog.WriteLine(
                "DerivedSequenceBvtTestCases:Validation of GetComplementedSequence() method of derived sequence completed successfully");
        }

        /// <summary>
        /// Creates a Rna derived sequence and validates GetSubSequence() method.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateRnaDerivedSequenceGetSubSequence()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.AlphabetNameNode);
            string rangeObj = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.Range);
            string expSubSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.RangeSequence);
            string derivedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.RnaDerivedSequenceNode, Constants.DerivedSequence);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            // Create derived Sequence
            DerivedSequence derSequence = CreateDerivedSequence(
                alphabet, expectedSequence);
            string[] ranges = rangeObj.Split(',');

            // Validate IndexOf() derived Sequence.
            Assert.AreEqual(derivedSequence, new string(derSequence.Select(a => (char)a).ToArray()));
            Assert.AreEqual(expSubSequence, new string(derSequence.GetSubSequence(long.Parse(ranges[0],
                (IFormatProvider)null), long.Parse(ranges[1], (IFormatProvider)null)).Select(a => (char)a).ToArray()));

            ApplicationLog.WriteLine(
                "DerivedSequenceBvtTestCases:Validation of GetSubSequence() method of derived sequence completed successfully");
        }

        /// <summary>
        /// Creates a Protein derived sequence after adding and removing few items from original sequence.
        /// Validates it against expected sequence. 
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateProteinDerivedSequence()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.AlphabetNameNode);
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
        /// Creates a Protein derived sequence using IndexOfNonGap() method.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateProteinDerivedSequenceIndexOfNonGap()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.AlphabetNameNode);
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
        /// Creates a Protein derived sequence using IndexOfNonGap(int) method.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateProteinDerivedSequenceIndexOfNonGapInt()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.AlphabetNameNode);
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
        /// Creates a Protein derived sequence using LastIndexOfNonGap() method.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateProteinDerivedSequenceLastIndexOfNonGap()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.AlphabetNameNode);
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
        /// Creates a Protein derived sequence using LastIndexOfNonGap(int) method.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateProteinDerivedSequenceLastIndexOfNonGapInt()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.AlphabetNameNode);
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
        /// Creates a Protein derived sequence and validates GetReversedSequence() method.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateProteinDerivedSequenceGetReversed()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.AlphabetNameNode);
            string reverseObj = this.utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.Reverse);
            string derivedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.DerivedSequence);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            // Create derived Sequence
            DerivedSequence derSequence = CreateDerivedSequence(
                alphabet, expectedSequence);

            // Validate IndexOf() derived Sequence.
            Assert.AreEqual(derivedSequence, new string(derSequence.Select(a => (char)a).ToArray()));
            Assert.AreEqual(reverseObj, new string(derSequence.GetReversedSequence().Select(a => (char)a).ToArray()));

            ApplicationLog.WriteLine(
                "DerivedSequenceBvtTestCases:Validation of GetReversedSequence() method of derived sequence completed successfully");
        }

        /// <summary>
        /// Creates a Protein derived sequence and validates GetSubSequence() method.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateProteinDerivedSequenceGetSubSequence()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.AlphabetNameNode);
            string rangeObj = this.utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.Range);
            string expSubSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.RangeSequence);
            string derivedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.DerivedSequence);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            // Create derived Sequence
            DerivedSequence derSequence = CreateDerivedSequence(
                alphabet, expectedSequence);
            string[] ranges = rangeObj.Split(',');

            // Validate IndexOf() derived Sequence.
            Assert.AreEqual(derivedSequence, new string(derSequence.Select(a => (char)a).ToArray()));
            Assert.AreEqual(expSubSequence, new string(derSequence.GetSubSequence(long.Parse(ranges[0],
                (IFormatProvider)null), long.Parse(ranges[1], (IFormatProvider)null)).Select(a => (char)a).ToArray()));

            ApplicationLog.WriteLine(
                "DerivedSequenceBvtTestCases:Validation of GetSubSequence() method of derived sequence completed successfully");
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
