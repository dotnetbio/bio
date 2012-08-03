/****************************************************************************
 * TranslationBvtTestCases.cs
 * 
 * This file contains the Translation BVT Test Cases which includes Codons, 
 * Complementation, ProteinTranslation and Transcription.
 * 
******************************************************************************/

using System;
using Bio.Algorithms.Translation;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio;

namespace Bio.TestAutomation.Algorithms.Translation
{

    /// <summary>
    /// Test Automation code for Bio Translation and BVT level validations.
    /// </summary>
    [TestClass]
    public class TranslationBvtTestCases
    {

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static TranslationBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region Translation Bvt TestCases

        /// <summary>
        /// Validate an aminoacod for a given valid Sequence.
        /// Input Data : Valid Sequence - 'GAUUCAAGGGCU'
        /// Output Data : Corresponding amino acid 'Serine'.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateAminoAcidForSequence()
        {
            // Get Node values from XML.
            string alphabetName = utilityObj.xmlUtil.GetTextValue(Constants.SimpleRnaAlphabetNode,
                Constants.AlphabetNameNode);
            string expectedSeq = utilityObj.xmlUtil.GetTextValue(Constants.CodonsNode,
                Constants.ExpectedNormalString);
            string expectedAminoAcid = utilityObj.xmlUtil.GetTextValue(Constants.CodonsNode,
                Constants.SeqAminoAcidV2);
            string expectedOffset = utilityObj.xmlUtil.GetTextValue(Constants.CodonsNode,
                Constants.OffsetVaule1);
            string aminoAcid = null;

            Sequence seq = new Sequence(Utility.GetAlphabet(alphabetName), expectedSeq);
            // Validate Codons lookup method.
            aminoAcid = Codons.Lookup(seq, Convert.ToInt32(expectedOffset, null)).ToString();

            // Validate amino acids for each triplet.
            Assert.AreEqual(expectedAminoAcid, aminoAcid);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Translation BVT: Amino Acid {0} is expected.", aminoAcid));
            ApplicationLog.WriteLine(
                "Translation BVT: Amino Acid validation for a given sequence was completed successfully.");
        }

        /// <summary>
        /// Validate an Protein translation for a given sequence.
        /// Input Data : Valid Sequence - 'AUUG'
        /// Output Data : Corresponding amino acid 'I'.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateProteinTranslation()
        {
            // Get Node values from XML.
            string expectedSeq = utilityObj.xmlUtil.GetTextValue(
                Constants.TranslationNode, Constants.ExpectedSequence);
            string expectedAminoAcid = utilityObj.xmlUtil.GetTextValue(
                Constants.TranslationNode, Constants.AminoAcid);
            ISequence protein = null;

            Sequence proteinTranslation = new Sequence(Alphabets.RNA, expectedSeq);
            protein = ProteinTranslation.Translate(proteinTranslation);

            // Validate Protein Translation.
            Assert.AreEqual(protein.Alphabet, Alphabets.Protein);
            Assert.AreEqual(new string(protein.Select(a => (char)a).ToArray()), expectedAminoAcid);

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Translation BVT: Amino Acid {0} is expected.", protein));
            ApplicationLog.WriteLine(
                "Translation BVT: Amino Acid validation for a given sequence was completed successfully.");
        }

        /// <summary>
        /// Validate an Protein translation for a given sequence by passing offset value.
        /// Input Data : Valid Sequence - 'AUUG'
        /// Output Data : Corresponding amino acid 'I'.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateProteinTranslationWithOffset()
        {
            // Get Node values from XML.
            string expectedSeq = utilityObj.xmlUtil.GetTextValue(
                Constants.TranslationNode, Constants.ExpectedSequence);
            string expectedAminoAcid = utilityObj.xmlUtil.GetTextValue(
                Constants.TranslationNode, Constants.AminoAcid);
            ISequence protein = null;

            Sequence proteinTranslation = new Sequence(Alphabets.RNA, expectedSeq);
            protein = ProteinTranslation.Translate(proteinTranslation, 0);

            // Validate Protein Translation.
            Assert.AreEqual(protein.Alphabet, Alphabets.Protein);
            Assert.AreEqual(new string(protein.Select(a => (char)a).ToArray()), expectedAminoAcid);

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Translation BVT: Amino Acid {0} is expected.", protein));
            ApplicationLog.WriteLine(
                "Translation BVT: Amino Acid validation for a given sequence was completed successfully.");
        }

        /// <summary>
        /// Validate Complement of DNA Sequence.
        /// Input Data : Valid Sequence - 'AGGTCCGATA'
        /// Output Data : Complement of DNA - 'TCCATGGGCTAT'
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateDnaComplementation()
        {
            // Get Node values from XML.
            string alphabetName = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleDnaAlphabetNode, Constants.AlphabetNameNode);
            string expectedSeq = utilityObj.xmlUtil.GetTextValue(
                Constants.ComplementNode, Constants.DnaSequence);
            string expectedComplement = utilityObj.xmlUtil.GetTextValue(
                Constants.ComplementNode, Constants.DnaComplement);
            ISequence complement = null;

            Sequence seq = new Sequence(Utility.GetAlphabet(alphabetName), expectedSeq);
            // Complement DNA Sequence.
            complement = seq.GetComplementedSequence();

            // Validate Complement.
            Assert.AreEqual(new string(complement.Select(a => (char)a).ToArray()), expectedComplement);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Translation BVT: Complement {0} is expected.", seq));
            ApplicationLog.WriteLine(
                "Translation BVT: Complement of DNA sequence was validate successfully.");
        }

        /// <summary>
        /// Validate Reverse Complement of DNA Sequence.
        /// Input Data : Valid Sequence - 'AGGTCCGATA'
        /// Output Data : Reverse Complement of DNA - 'TATCGGGTACCT'
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateDnaRevComplementation()
        {
            // Get Node values from XML.
            string alphabetName = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleDnaAlphabetNode, Constants.AlphabetNameNode);
            string expectedSeq = utilityObj.xmlUtil.GetTextValue(
                Constants.ComplementNode, Constants.DnaSequence);
            string expectedRevComplement = utilityObj.xmlUtil.GetTextValue(
                Constants.ComplementNode, Constants.DnaRevComplement);

            Sequence seq = new Sequence(Utility.GetAlphabet(alphabetName), expectedSeq);
            // Reverse Complement of DNA Sequence.
            ISequence revComplement = seq.GetReverseComplementedSequence();

            // Validate Reverse Complement.
            Assert.AreEqual(new string(revComplement.Select(a => (char)a).ToArray()), expectedRevComplement);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Translation BVT: Reverse Complement {0} is expected.", seq));

            ApplicationLog.WriteLine(
                "Translation BVT: Reverse Complement of DNA sequence was validate successfully.");
        }

        /// <summary>
        /// Validate Transcribe of DNA Sequence.
        /// Input Data : Valid Sequence - 'ATGGCG'
        /// Output Data : Transcription - 'AUGGCG'
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateTranscribe()
        {
            // Get Node values from XML.
            string alphabetName = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleDnaAlphabetNode, Constants.AlphabetNameNode);
            string expectedSeq = utilityObj.xmlUtil.GetTextValue(
                Constants.TranscribeNode, Constants.DnaSequence);
            string expectedTranscribe = utilityObj.xmlUtil.GetTextValue(
                Constants.TranscribeNode, Constants.TranscribeV2);

            Sequence seq = new Sequence(Utility.GetAlphabet(alphabetName), expectedSeq);
            // Transcription of DNA Sequence.
            ISequence transcribe = Transcription.Transcribe(seq);

            // Validate Transcription.
            Assert.AreEqual(expectedTranscribe, new string(transcribe.Select(a => (char)a).ToArray()));
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Translation BVT: Transcription {0} is expected.", seq));
            ApplicationLog.WriteLine(
                "Translation BVT: Transcription of DNA sequence was validate successfully.");
        }

        /// <summary>
        /// Validate Reverse Transcribe of RNA Sequence.
        /// Input Data : Valid Sequence - 'UACCGC'
        /// Output Data : Reverse Transcription - 'TACCGC'
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateRevTranscribe()
        {
            // Get Node values from XML.
            string alphabetName = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleRnaAlphabetNode, Constants.AlphabetNameNode);
            string expectedSeq = utilityObj.xmlUtil.GetTextValue(
                Constants.TranscribeNode, Constants.RnaSequence);
            string expectedRevTranscribe = utilityObj.xmlUtil.GetTextValue(
                Constants.TranscribeNode, Constants.RevTranscribeV2);

            Sequence seq = new Sequence(Utility.GetAlphabet(alphabetName), expectedSeq);
            // Reverse Transcription of RNA Sequence.
            ISequence revTranscribe = Transcription.ReverseTranscribe(seq);

            // Validate Reverse Transcription.
            Assert.AreEqual(expectedRevTranscribe, new string(revTranscribe.Select(a => (char)a).ToArray()));
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Translation BVT: Reverse Transcription {0} is expected.", seq));
            ApplicationLog.WriteLine(
                "Translation BVT: Reverse Transcription of DNA sequence was validate successfully.");
        }

        #endregion Translation Bvt TestCases
    }
}
