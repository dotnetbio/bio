using System;

using Bio.Algorithms.Translation;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests.Algorithms.Translation
{
    /// <summary>
    ///     Test Automation code for Bio Translation and P2 level validations.
    /// </summary>
    [TestFixture]
    public class TranslationP2TestCases
    {
        private readonly Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #region  Codons P2 TestCases

        /// <summary>
        ///     Validate an aminoacod for a given RNA Sequence with More than 12 characters..
        ///     Input Data :Sequence with more than 12 characters - 'AAAGGGAUGCCUGUUUGA'.
        ///     Output Data : Corresponding amino acid 'Arginine'.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateLookupWithMoreThanTwelveChars()
        {
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(Constants.SimpleRnaAlphabetNode,
                                                                  Constants.AlphabetNameNode);
            string expectedSeq = this.utilityObj.xmlUtil.GetTextValue(Constants.CodonsNode,
                                                                 Constants.SequenceWithmoreThanTweleveChars);
            string expectedAminoAcid = this.utilityObj.xmlUtil.GetTextValue(Constants.CodonsNode,
                                                                       Constants.OffsetZeroSixCharsAminoAcidV2);
            string expectedOffset = this.utilityObj.xmlUtil.GetTextValue(Constants.CodonsNode,
                                                                    Constants.OffsetVaule2);
            string aminoAcid = null;

            var seq = new Sequence(Utility.GetAlphabet(alphabetName), expectedSeq);
            // Validate Codons lookup method.
            aminoAcid = Codons.Lookup(seq, Convert.ToInt32(expectedOffset, null)).ToString();

            // Validate amino acids for a given sequence.
            Assert.AreEqual(expectedAminoAcid, aminoAcid);

            ApplicationLog.WriteLine(string.Format(null,
                                                   "Translation P2: Amino Acid {0} is expected.", aminoAcid));
            ApplicationLog.WriteLine(
                "Translation P2: Amino Acid validation for a given sequence was completed successfully.");
        }

        /// <summary>
        ///     Validate an aminoacod for a given RNA Sequence with More than 12 characters and offset value "1"..
        ///     Input Data :Sequence with more than 12 characters - 'AAAGGGAUGCCUGUUUGA'.
        ///     Output Data : Corresponding amino acid 'Isoleucine'.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateLookupWithZeroOffset()
        {
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(Constants.SimpleRnaAlphabetNode,
                                                                  Constants.AlphabetNameNode);
            string expectedSeq = this.utilityObj.xmlUtil.GetTextValue(Constants.CodonsNode,
                                                                 Constants.SequenceWithmoreThanTweleveChars);
            string expectedAminoAcid = this.utilityObj.xmlUtil.GetTextValue(Constants.CodonsNode,
                                                                       Constants.OffsetOneMoreThanTwelveCharsAminoAcidV2);
            string expectedOffset = this.utilityObj.xmlUtil.GetTextValue(Constants.CodonsNode,
                                                                    Constants.OffsetVaule4);
            string aminoAcid = null;

            var seq = new Sequence(Utility.GetAlphabet(alphabetName), expectedSeq);
            // Validate Codons lookup method.
            aminoAcid = Codons.Lookup(seq, Convert.ToInt32(expectedOffset, null)).ToString();

            // Validate amino acids for a given sequence.
            Assert.AreEqual(expectedAminoAcid, aminoAcid);

            ApplicationLog.WriteLine(string.Format(null,
                                                   "Translation P2: Amino Acid {0} is expected.", aminoAcid));
            ApplicationLog.WriteLine(
                "Translation P2: Amino Acid validation for a given sequence was completed successfully.");
        }

        /// <summary>
        ///     Validate an aminoacod for a given RNA Sequence with 12 characters and offset value "1"..
        ///     Input Data :Sequence with 12 characters - 'AAAGGGAUGCCU'.
        ///     Output Data : Corresponding amino acid 'Isoleucine'.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateLookupWithOneOffset()
        {
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(Constants.SimpleRnaAlphabetNode,
                                                                  Constants.AlphabetNameNode);
            string expectedSeq = this.utilityObj.xmlUtil.GetTextValue(Constants.CodonsNode,
                                                                 Constants.SequenceWithmoreThanTweleveChars);
            string expectedAminoAcid = this.utilityObj.xmlUtil.GetTextValue(Constants.CodonsNode,
                                                                       Constants.OffsetOneMoreThanTwelveCharsAminoAcidV2);
            string expectedOffset = this.utilityObj.xmlUtil.GetTextValue(Constants.CodonsNode,
                                                                    Constants.OffsetVaule4);
            string aminoAcid = null;

            var seq = new Sequence(Utility.GetAlphabet(alphabetName), expectedSeq);
            // Validate Codons lookup method.
            aminoAcid = Codons.Lookup(seq, Convert.ToInt32(expectedOffset, null)).ToString();

            // Validate amino acids for a given sequence.
            Assert.AreEqual(expectedAminoAcid, aminoAcid);

            ApplicationLog.WriteLine(string.Format(null,
                                                   "Translation P2: Amino Acid {0} is expected.", aminoAcid));
            ApplicationLog.WriteLine(
                "Translation P2: Amino Acid validation for a given sequence was completed successfully.");
        }

        /// <summary>
        ///     Validate an aminoacod for a given DNA Sequence with offset value "1".
        ///     Input Data : Valid Sequence - 'ATGGCG'.
        ///     Output Data : Corresponding amino acid 'Threonine'.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateLookupWithDnaSeqAndOffsetValue()
        {
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(Constants.SimpleDnaAlphabetNode,
                                                                  Constants.AlphabetNameNode);
            string expectedSeq = this.utilityObj.xmlUtil.GetTextValue(Constants.TranscribeNode,
                                                                 Constants.DnaSequence);
            string expectedAminoAcid = this.utilityObj.xmlUtil.GetTextValue(Constants.CodonsNode,
                                                                       Constants.DnaSeqAminoAcidWithOffsetValueOneDna);
            string expectedOffset = this.utilityObj.xmlUtil.GetTextValue(Constants.CodonsNode,
                                                                    Constants.OffsetVaule4);
            ISequence transcribe = null;

            var seq = new Sequence(Utility.GetAlphabet(alphabetName), expectedSeq);
            // Transcribe DNA to RNA.
            transcribe = Transcription.Transcribe(seq);

            // Validate Codons lookup method.
            string aminoAcid = Codons.Lookup(transcribe, Convert.ToInt32(expectedOffset, null)).ToString();

            // Validate amino acids for a given sequence.
            Assert.AreEqual(expectedAminoAcid, aminoAcid);

            ApplicationLog.WriteLine(string.Format(null,
                                                   "Translation P2: Amino Acid {0} is expected.", aminoAcid));
            ApplicationLog.WriteLine(
                "Translation P2: Amino Acid validation for a given sequence was completed successfully.");
        }

        #endregion Codons P2 TestCases

        #region  Transcribe P2 TestCases

        /// <summary>
        ///     Validate Reverse Transcribe by passing null value.
        ///     Input Data : Invalid Sequence - 'null'
        ///     Output Data : Exception.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateRevTranscribeForNull()
        {
            bool Exthrown = false;

            // Reverse Transcribe null Sequence.
            try
            {
                Transcription.ReverseTranscribe(null);
            }
            catch (ArgumentNullException)
            {
                Exthrown = true;
            }
            // Validate if Reverse Transcribe method is throwing an exception.
            Assert.IsTrue(Exthrown);
            ApplicationLog.WriteLine(
                "Translation P2: Reverse Transcribe method was throwing an expection for null value.");
        }

        /// <summary>
        ///     Validate Translation method by passing null value.
        ///     Input Data : Invalid Sequence - 'null'
        ///     Output Data : Exception.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateTranslationForNull()
        {
            bool Exthrown = false;

            // Translation by passing null Sequence.
            try
            {
                ProteinTranslation.Translate(null);
            }
            catch (ArgumentNullException)
            {
                Exthrown = true;
            }

            // Validate if Translate method is throwing an exception.
            Assert.IsTrue(Exthrown);
            ApplicationLog.WriteLine(
                "Translation P2: Translate method was throwing an expection for null value.");
        }

        /// <summary>
        ///     Validate Translation method by passing invalid sequence and valid offset value.
        ///     Input Data : Sequence - 'Null' and offset "10".
        ///     Output Data : Exception.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateTranslationForInvalidOffset()
        {
            // Get Node values from XML.
            bool Exthrown = false;

            // Translation by passing null Sequence.
            try
            {
                ProteinTranslation.Translate(null, 10);
            }
            catch (ArgumentNullException)
            {
                Exthrown = true;
            }
            // Validate if Translate method is throwing an exception.
            Assert.IsTrue(Exthrown);
            ApplicationLog.WriteLine(
                "Translation P2: Translate method was throwing an expection for null value.");
        }


        /// <summary>
        ///     Validate Translation method by passing Negative offset value.
        ///     Input Data : Sequence - 'UACCGC' and offset "-10".
        ///     Output Data : Exception.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateTranslationForNegativeOffset()
        {
            // Get Node values from XML.
            string expectedSeq = this.utilityObj.xmlUtil.GetTextValue(Constants.TranslationNode,
                                                                 Constants.RnaSequence);
            bool Exthrown = false;

            // Translate Six characters RNA to protein.
            ISequence proteinTranslation = new Sequence(Alphabets.RNA, expectedSeq);

            // Call a translate method by passing negative offset value.
            try
            {
                ProteinTranslation.Translate(proteinTranslation, -10);
            }
            catch (ArgumentOutOfRangeException)
            {
                Exthrown = true;
            }
            // Validate if Translate method is throwing an exception.
            Assert.IsTrue(Exthrown);
            ApplicationLog.WriteLine(
                "Translation P2: Translate method was throwing an expection for null value.");
        }

        /// <summary>
        ///     Validate Translation method by passing ambigous RNA but
        ///     pretending to be RNA.
        ///     Input Data : Sequence - 'GUNAACAGAAANUGU' and offset "0".
        ///     Output Data : Exception.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateTranslationOfAmbiguousRnaUsingRnaAlphabet()
        {
            // Get Node values from XML.
            string rnaSequenceStr = this.utilityObj.xmlUtil.GetTextValue(Constants.TranslationNode,
                                                                    Constants.AmbiguousRnaSequence);
            bool Exthrown = false;

            // Build ambiguous RNA sequence using an RNA alphabet.
            ISequence rnaSequence = new Sequence(Alphabets.RNA, rnaSequenceStr, false);

            // Call a translate method by passing negative offset value.
            try
            {
                ProteinTranslation.Translate(rnaSequence);
            }
            catch (InvalidOperationException)
            {
                Exthrown = true;
            }
            // Validate if Translate method is throwing an exception.
            Assert.IsTrue(Exthrown);
            ApplicationLog.WriteLine(
                "Translation P2: Translate method was throwing an expection for ambiguous RNA.");
        }

        #endregion Transcribe P2 TestCases
    }
}