/****************************************************************************
 * GffP1TestCases.cs
 * 
 *   This file contains the Gff - Parsers and Formatters Priority One test cases.
 * 
***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Bio.Extensions;
using Bio.TestAutomation.Util;
using Bio.IO.Gff;
using Bio.Util.Logging;
using NUnit.Framework;

#if (SILVERLIGHT == false)
   namespace Bio.TestAutomation.IO.GFF
#else
    namespace Bio.Silverlight.TestAutomation.IO.GFF
#endif
{
    /// <summary>
    /// Gff Priority One parserObj and formatter test cases implementation.
    /// </summary>
    [TestFixture]
    public class GffP1TestCases
    {

        #region Enums

        /// <summary>
        /// GFF Parser Parameters which are used for different test cases 
        /// based on which the test cases are executed.
        /// </summary>
        enum ParserParameters
        {
            EncodeConstructor,
            EncodeProperty,
            AlphabetProperty,
            Default
        };

        #endregion Enums

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\GffTestsConfig.xml");

        #endregion Global Variables

        #region Gff Parser P1 Test cases

        /// <summary>
        /// Parse a valid Gff file (Dna) and using Parse(file-name) method and 
        /// validate the expected sequence
        /// Input : Dna Gff File
        /// Validation : Read the Gff file to which the sequence was parsed and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffParserValidateParseWithDnaSequence()
        {
            ValidateParseGeneralTestCases(Constants.SimpleGffDnaNodeName, true);
        }

        /// <summary>
        /// Parse a valid Gff file (Rna) and using Parse(file-name) method and 
        /// validate the expected sequence
        /// Input : Rna Gff File
        /// Validation : Read the Gff file to which the sequence was parsed and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffParserValidateParseWithRnaSequence()
        {
            ValidateParseGeneralTestCases(Constants.SimpleGffRnaNodeName, true);
        }

        /// <summary>
        /// Parse a valid Gff file (Protein) and using Parse(file-name) method and 
        /// validate the expected sequence
        /// Input : Protein Gff File
        /// Validation : Read the Gff file to which the sequence was parsed and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffParserValidateParseWithProteinSequence()
        {
            ValidateParseGeneralTestCases(Constants.SimpleGffProteinNodeName, true);
        }

        /// <summary>
        /// Parse a valid Gff file and using Parse(file-name) method by passing 
        /// Encoding in constructor and validate the expected sequence
        /// Input : Gff File
        /// Validation : Read the Gff file to which the sequence was formatted and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffParserValidateParseWithEncodingConstructor()
        {
            ValidateParseGeneralTestCases(Constants.SimpleGffDnaNodeName,
                true, ParserParameters.EncodeConstructor);
        }

        /// <summary>
        /// Parse a valid Gff file and using Parse(file-name) method by passing 
        /// Encoding property and validate the expected sequence
        /// Input : Gff File
        /// Validation : Read the Gff file to which the sequence was parsed and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffParserValidateParseWithEncodingProperty()
        {
            ValidateParseGeneralTestCases(Constants.SimpleGffDnaNodeName,
                true, ParserParameters.EncodeProperty);
        }

        /// <summary>
        /// Parse a valid Gff file and using Parse(file-name) method by passing 
        /// Alphabet property and validate the expected sequence
        /// Input : Gff File
        /// Validation : Read the Gff file to which the sequence was parsed and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffParserValidateParseWithAlphabetProperty()
        {
            ValidateParseGeneralTestCases(Constants.SimpleGffDnaNodeName,
                true, ParserParameters.AlphabetProperty);
        }

        /// <summary>
        /// Parse a valid Gff file (DNA & RNA) and using Parse(file-name) method by passing 
        /// and validate the expected sequence
        /// Input : Gff File (DNA & RNA)
        /// Validation : Read the Gff file with multi sequence (DNA & RNA) to which the sequence 
        /// parse and validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffParserValidateParseWithDnaRna()
        {
            ValidateParseMultiSequenceGeneralTestCases(Constants.MultiSeqDnaRnaGffNodeName,
                true);
        }

        /// <summary>
        /// Parse a valid Gff file (RNA & Protein) and using Parse(file-name) method by passing 
        /// and validate the expected sequence
        /// Input : Gff File (RNA & Protein)
        /// Validation : Read the Gff file with multi sequence (RNA & Protein) to which the sequence 
        /// parse and validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffParserValidateParseWithRnaProtein()
        {
            ValidateParseMultiSequenceGeneralTestCases(Constants.MultiSeqRnaProGffNodeName,
                true);
        }

        /// <summary>
        /// Parse a valid Gff file (DNA & Protein) and using Parse(file-name) method by passing 
        /// and validate the expected sequence
        /// Input : Gff File (DNA & Protein)
        /// Validation : Read the Gff file with multi sequence (DNA & Protein) to which the sequence 
        /// parse and validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffParserValidateParseWithDnaProtein()
        {
            ValidateParseMultiSequenceGeneralTestCases(Constants.MultiSeqDnaProGffNodeName,
                true);
        }

        /// <summary>
        /// Parse a valid Gff file (DNA, RNA & Protein) and using Parse(file-name) method by passing 
        /// and validate the expected sequence
        /// Input : Gff File (DNA, RNA & Protein)
        /// Validation : Read the Gff file with multi sequence (DNA, RNA & Protein) to which the sequence 
        /// parse and validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffParserValidateParseWithDnaRnaProtein()
        {
            ValidateParseMultiSequenceGeneralTestCases(Constants.MultiSeqDnaRnaProGffNodeName,
                true);
        }

        /// <summary>
        /// Parse a valid Large size Gff file and using Parse(file-name) method by passing 
        /// and validate the expected sequence
        /// Input : Large size Gff File 
        /// Validation : Read the Gff file with large size to which the sequence 
        /// parse and validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffParserValidateParseWithLargeSize()
        {
            ValidateParseGeneralTestCases(Constants.LargeSizeGffNodeName, true);
        }

        /// <summary>
        /// Parse a valid Medium size Gff file and using Parse(file-name) method by passing 
        /// and validate the expected sequence
        /// Input : Medium size Gff File 
        /// Validation : Read the Gff file with medium size to which the sequence 
        /// parse and validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffParserValidateParseWithMediumSize()
        {
            ValidateParseGeneralTestCases(Constants.MediumSizeGffNodeName, true);
        }

        /// <summary>
        /// Parse a valid Gff file (DNA, DNA, RNA & Protein) and using Parse(file-name) 
        /// method by passing 
        /// and validate the expected sequence
        /// Input : Gff File (DNA, DNA, RNA & Protein)
        /// Validation : Read the Gff file with more than 3 sequences 
        /// parse and validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffParserValidateParseWithMaximumSequences()
        {
            ValidateParseMultiSequenceGeneralTestCases(Constants.MaxSequenceGffNodeName,
                true);
        }

        /// <summary>
        /// Parse a valid Gff file with comments and features and using Parse(file-name) method and 
        /// validate the expected sequence
        /// Input : Gff File with comments and features
        /// Validation : Read the Gff file to which the sequence was parsed and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffParserValidateParseWithCommentsFeatures()
        {
            ValidateParseGeneralTestCases(Constants.SimpleGffDnaNodeName, true);
        }

        /// <summary>
        /// Parse a valid with Only Features in Gff file and using Parse(file-name) 
        /// method by passing 
        /// and validate the expected sequence
        /// Input : Gff File with only features
        /// Validation : Read the Gff file with only features to which the sequence 
        /// parse and validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffParserValidateParseWithOnlyFeatures()
        {
            ValidateParseGeneralTestCases(Constants.OnlyFeaturesGffNodeName, true);
        }

        #endregion Gff Parser P1 Test cases

        #region Gff Formatter P1 Test cases

        /// <summary>
        /// Format a valid DNA Sequence to a 
        /// Gff file Format(sequence, filename) method with Sequence and Writer as parameter
        /// and validate the same.
        /// Input : DNA Gff Sequence
        /// Validation : Read the Gff file to which the sequence was formatted and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffFormatterValidateFormatDnaSequence()
        {
            ValidateFormatGeneralTestCases(Constants.SimpleGffDnaNodeName, true, false);
        }

        /// <summary>
        /// Format a valid Rna Sequence to a 
        /// Gff file Format(sequence, filename) method with Sequence and Writer as parameter
        /// and validate the same.
        /// Input : Rna Gff Sequence
        /// Validation : Read the Gff file to which the sequence was formatted and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffFormatterValidateFormatRnaSequence()
        {
            ValidateFormatGeneralTestCases(Constants.SimpleGffRnaNodeName, true, false);
        }

        /// <summary>
        /// Format a valid Protein Sequence to a 
        /// Gff file Format(sequence, filename) method with Sequence and Writer as parameter
        /// and validate the same.
        /// Input : Protein Gff Sequence
        /// Validation : Read the Gff file to which the sequence was formatted and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffFormatterValidateFormatProteinSequence()
        {
            ValidateFormatGeneralTestCases(Constants.SimpleGffProteinNodeName, true, false);
        }

        /// <summary>
        /// Format a valid Single Dna Sequence to a 
        /// Gff file FormatString() method with Sequence and Writer as parameter
        /// and validate the same.
        /// Input : Dna Gff Sequence
        /// Validation : Read the Gff file to which the sequence was formatted and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffFormatterValidateFormatStringDna()
        {
            ValidateFormatStringTestCases(Constants.SimpleGffDnaNodeName);
        }

        /// <summary>
        /// Format a valid Single Rna Sequence to a 
        /// Gff file FormatString() method with Sequence and Writer as parameter
        /// and validate the same.
        /// Input : Rna Gff Sequence
        /// Validation : Read the Gff file to which the sequence was formatted and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffFormatterValidateFormatStringRna()
        {
            ValidateFormatStringTestCases(Constants.SimpleGffRnaNodeName);
        }

        /// <summary>
        /// Format a valid Single Protein Sequence to a 
        /// Gff file FormatString() method with Sequence and Writer as parameter
        /// and validate the same.
        /// Input : Protein Gff Sequence
        /// Validation : Read the Gff file to which the sequence was formatted and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffFormatterValidateFormatStringProtein()
        {
            ValidateFormatStringTestCases(Constants.SimpleGffProteinNodeName);
        }

        /// <summary>
        /// Format a valid Medium size Sequence to a 
        /// Gff file Format(sequence, filename) method with Sequence and Writer as parameter
        /// and validate the same.
        /// Input : Medium size Gff file
        /// Validation : Read the medium size Gff file to which the sequence was formatted and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffFormatterValidateFormatMediumSizeFileName()
        {
            ValidateFormatGeneralTestCases(Constants.MediumSizeGffNodeName, true, false);
        }

        /// <summary>
        /// Format a valid Medium size Gff file FormatString() method 
        /// with Sequence and Writer as parameter
        /// and validate the same.
        /// Input : medium size Gff file
        /// Validation : Read the medium size Gff file to which the sequence 
        /// was formatted and validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffFormatterValidateFormatStringMediumSize()
        {
            ValidateFormatStringTestCases(Constants.MediumSizeGffNodeName);
        }

        /// <summary>
        /// Format a valid Single Sequence to a 
        /// Gff file Format() method with Sequence and Writer as parameter
        /// and validate the same by reparsing.
        /// Input : Gff Sequence
        /// Validation : Read the Gff file to which the sequence was formatted and 
        /// validate Features, Sequence, Sequence Count by reparsing
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffFormatterValidateFormatByReparsing()
        {
            ValidateFormatGeneralTestCases(Constants.SimpleGffDnaNodeName, false, false);
        }

        /// <summary>
        /// Format a valid Dna, Rna Sequence to a 
        /// Gff file Format(sequence, filename) method with Sequence and Writer as parameter
        /// and validate the same.
        /// Input : Dna, Rna Gff file
        /// Validation : Read the Dna, Rna Gff file to which the sequence was formatted and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffFormatterValidateFormatDnaRna()
        {
            ValidateFormatMultiSequencesTestCases(Constants.MultiSeqDnaRnaGffNodeName);
        }

        /// <summary>
        /// Format a valid Rna, Protein Sequence to a 
        /// Gff file Format(sequence, filename) method with Sequence and Writer as parameter
        /// and validate the same.
        /// Input : Protein, Rna Gff file
        /// Validation : Read the Protein, Rna Gff file to which the sequence was formatted and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffFormatterValidateFormatRnaProtein()
        {
            ValidateFormatMultiSequencesTestCases(Constants.MultiSeqRnaProGffNodeName);
        }

        /// <summary>
        /// Format a valid Dna, Protein Sequence to a 
        /// Gff file Format(sequence, filename) method with Sequence and Writer as parameter
        /// and validate the same.
        /// Input : Protein, Dna Gff file
        /// Validation : Read the Protein, Dna Gff file to which the sequence was formatted and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffFormatterValidateFormatDnaProtein()
        {
            ValidateFormatMultiSequencesTestCases(Constants.MultiSeqDnaProGffNodeName);
        }

        /// <summary>
        /// Format a valid Dna, Rna, Protein Sequence to a 
        /// Gff file Format(sequence, filename) method with Sequence and Writer as parameter
        /// and validate the same.
        /// Input : Protein, Rna, Dna Gff file
        /// Validation : Read the Protein, Rna, Dna Gff file to which the sequence was formatted and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffFormatterValidateFormatDnaRnaProtein()
        {
            ValidateFormatMultiSequencesTestCases(Constants.MultiSeqDnaRnaProGffNodeName);
        }

        /// <summary>
        /// Format a valid Dna, Rna Gff file FormatString() method 
        /// with Sequence and Writer as parameter
        /// and validate the same.
        /// Input : Dna, Rna Gff file
        /// Validation : Read the Dna, Rna Gff file to which the sequence 
        /// was formatted and validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffFormatterValidateFormatStringDnaRna()
        {
            ValidateFormatStringTestCases(Constants.MultiSeqDnaRnaGffNodeName);
        }

        /// <summary>
        /// Format a valid Protein, Rna Gff file FormatString() method 
        /// with Sequence and Writer as parameter
        /// and validate the same.
        /// Input : Protein, Rna Gff file
        /// Validation : Read the Protein, Rna Gff file to which the sequence 
        /// was formatted and validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffFormatterValidateFormatStringRnaProtein()
        {
            ValidateFormatStringTestCases(Constants.MultiSeqRnaProGffNodeName);
        }

        /// <summary>
        /// Format a valid Protein, Dna Gff file FormatString() method 
        /// with Sequence and Writer as parameter
        /// and validate the same.
        /// Input : Protein, Dna Gff file
        /// Validation : Read the Protein, Dna Gff file to which the sequence 
        /// was formatted and validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffFormatterValidateFormatStringDnaProtein()
        {
            ValidateFormatStringTestCases(Constants.MultiSeqDnaProGffNodeName);
        }

        /// <summary>
        /// Format a valid Protein, Rna, Dna Gff file FormatString() method 
        /// with Sequence and Writer as parameter
        /// and validate the same.
        /// Input : Protein, Rna, Dna Gff file
        /// Validation : Read the Protein, Rna, Dna Gff file to which the sequence 
        /// was formatted and validate Features, Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GffFormatterValidateFormatStringDnaRnaProtein()
        {
            ValidateFormatStringTestCases(Constants.MultiSeqDnaRnaProGffNodeName);
        }

        #endregion Gff Formatter P1 Test cases

        #region Supported Methods

        /// <summary>
        /// Parses all test cases related to Parse() method based on the 
        /// parameters passed and validates the same.
        /// </summary>
        /// <param name="nodeName">Xml Node name to be read.</param>
        /// <param name="isFilePath">Is file path passed as parameter?</param>
        void ValidateParseGeneralTestCases(string nodeName,
            bool isFilePath)
        {
            ValidateParseGeneralTestCases(nodeName, isFilePath,
                ParserParameters.Default);
        }

        /// <summary>
        /// Parses all test cases related to Parse() method based on 
        /// the parameters passed and validates the same.
        /// </summary>
        /// <param name="nodeName">Xml Node name to be read.</param>
        /// <param name="isFilePath">Is file path passed as parameter?</param>
        /// <param name="parseParam">Parse method parameters</param>
        void ValidateParseGeneralTestCases(string nodeName,
            bool isFilePath, ParserParameters parseParam)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.FilePathNode);

            Assert.IsTrue(File.Exists(filePath));

            // Logs information to the log file
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser P1 : File Exists in the Path '{0}'.",
                filePath));

            IList<ISequence> seqs = null;
            GffParser parserObj = new GffParser();

            switch (parseParam)
            {
                case ParserParameters.AlphabetProperty:
                    parserObj.Alphabet = Alphabets.DNA;
                    break;
                default:
                    break;
            }

            if (isFilePath)
            {
                seqs = parserObj.Parse(filePath).ToList();
            }
            else
            {
                using (var reader = File.OpenRead(filePath))
                    seqs = parserObj.Parse(reader).ToList();
            }

            Assert.IsNotNull(seqs);
            int expectedCount = 1;
            Assert.AreEqual(expectedCount, seqs.Count);
            ApplicationLog.WriteLine(string.Format("Gff Parser P1 : Number of Sequences found are '{0}'.",
                seqs.Count.ToString((IFormatProvider)null)));

            bool valFeat = ValidateFeatures(seqs[0], nodeName);

            Assert.IsTrue(valFeat);
            ApplicationLog.WriteLine(
                "Gff Parser P1 : Successfully validated all the Features for a give Sequence in GFF File.");

            string expectedSequence = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.ExpectedSequenceNode);

            Sequence seq = (Sequence)seqs[0];
            Assert.IsNotNull(seq);                   
            string sequenceInString = new string(seq.Select(x => (char)x).ToArray());

            Assert.AreEqual(expectedSequence, sequenceInString);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,   
                "Gff Parser P1: The Gff sequence '{0}' validation after Parse() is found to be as expected.",
                sequenceInString));

            byte[] TempSeqData1 = new byte[seq.Count];

            for (int i = 0; i < seq.Count; i++)
            {
                TempSeqData1[i] = seq[i];
            }

            Assert.AreEqual(expectedSequence.Length, TempSeqData1.Length);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser P1: The Gff Length sequence '{0}' is as expected.",
                expectedSequence.Length));

            string expectedAlphabet = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.AlphabetNameNode).ToLower(CultureInfo.CurrentCulture);

            Assert.IsNotNull(seq.Alphabet);
            Assert.AreEqual(seq.Alphabet.Name.ToLower(CultureInfo.CurrentCulture),
                expectedAlphabet);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser P1: The Sequence Alphabet is '{0}' and is as expected.",
                seq.Alphabet.Name));

            string expectedSequenceId = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.SequenceIdNode);
            Assert.AreEqual(expectedSequenceId, seq.ID);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser P1: The Sequence ID is '{0}' and is as expected.", seq.ID));
        }

        /// <summary>
        /// Parses all test cases related to Parse() method with multi sequence 
        /// based on the parameters passed 
        /// and validates the same.
        /// </summary>
        /// <param name="nodeName">Xml Node name to be read.</param>
        /// <param name="isFilePath">Is file path passed as parameter?</param>
        void ValidateParseMultiSequenceGeneralTestCases(string nodeName,
            bool isFilePath)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.FilePathNode);

            Assert.IsTrue(File.Exists(filePath));

            // Logs information to the log file
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser P1 : File Exists in the Path '{0}'.", filePath));

            IList<ISequence> seqs = null;
            GffParser parserObj = new GffParser();

            if (isFilePath)
            {
                seqs = parserObj.Parse(filePath).ToList();
            }
            else
            {
                using (var reader = File.OpenRead(filePath))
                {
                    seqs = parserObj.Parse(reader).ToList();
                }
            }

            int expectedNoOfSeqs = int.Parse(utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.NumberOfSequencesNode), null);
            Assert.IsNotNull(seqs);
            Assert.AreEqual(expectedNoOfSeqs, seqs.Count);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser P1 : Number of Sequences found are '{0}'.",
                seqs.Count.ToString((IFormatProvider)null)));

            string[] expectedSequences = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.ExpectedSequenesNode);
            string[] alphabets = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.AlphabetsNode);
            string[] seqIds = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.SequenceIdsNode);

            for (int i = 0; i < expectedNoOfSeqs; i++)
            {
                bool valFeat = ValidateMultiSequenceFeatures(seqs[i], i + 1, nodeName);

                Assert.IsTrue(valFeat);
                ApplicationLog.WriteLine(
                    "Gff Parser P1 : Successfully validated all the Features for a give Sequence in GFF File.");

                Sequence seq = (Sequence)seqs[i];
                Assert.IsNotNull(seq);                
                string sequenceInString1 = new string(seq.Select(x => (char)x).ToArray());

                if (string.Compare(sequenceInString1, sequenceInString1.ToUpper(CultureInfo.CurrentCulture)) == 0)
                {
                    expectedSequences[i] = expectedSequences[i].ToUpper(CultureInfo.CurrentCulture);
                }
                else
                {
                    expectedSequences[i] = expectedSequences[i].ToLower(CultureInfo.CurrentCulture);
                }
                Assert.AreEqual(expectedSequences[i], sequenceInString1);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "Gff Parser P1: The Gff sequence '{0}' validation after Parse() is found to be as expected.",
                    sequenceInString1));

                Assert.AreEqual(expectedSequences[i].Length, sequenceInString1.Length);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "Gff Parser P1: The Gff Length sequence '{0}' is as expected.",
                    expectedSequences[i].Length));

                Assert.IsNotNull(seq.Alphabet);
                Assert.AreEqual(seq.Alphabet.Name.ToLower(CultureInfo.CurrentCulture),
                    alphabets[i]);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "Gff Parser P1: The Sequence Alphabet is '{0}' and is as expected.",
                    seq.Alphabet.Name));

                Assert.AreEqual(seqIds[i], seq.ID);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "Gff Parser P1: The Sequence ID is '{0}' and is as expected.", seq.ID));
            }
        }

        /// <summary>
        /// Validates the Metadata Features of a Gff Sequence for the 
        /// sequence and node name specified.
        /// </summary>
        /// <param name="seq">Sequence that needs to be validated.</param>
        /// <param name="nodeName">Xml Node name to be read.</param>
        /// <returns>True/False</returns>
        bool ValidateFeatures(ISequence seq, string nodeName)
        {
            // Gets all the Features from the Sequence for Validation
            List<MetadataListItem<List<string>>> featureList =
                (List<MetadataListItem<List<string>>>)seq.Metadata[Constants.Features];

            // Gets all the xml values for validation
            string[] sequenceNames = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.SequenceNameNodeName);
            string[] sources = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.SourceNodeName);
            string[] featureNames = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.FeatureNameNodeName);
            string[] startValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.StartNodeName);
            string[] endValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.EndNodeName);
            string[] scoreValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.ScoreNodeName);
            string[] strandValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.StrandNodeName);
            string[] frameValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.FrameNodeName);
            string[] attributeValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.AttributesNodeName);
            int i = 0;

            // Loop through each and every feature and validate the same.
            foreach (MetadataListItem<List<string>> feature in featureList)
            {
                Dictionary<string, List<string>> itemList = feature.SubItems;

                // Read specific feature Item and validate
                // Validate Start
                try
                {
                    List<string> st = itemList[Constants.FeatureStart];
                    foreach (string sin in st)
                    {
                        if (0 != string.Compare(startValues[i], sin,
                            CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                            return false;
                    }
                }
                catch (KeyNotFoundException) { }

                // Validate Score
                try
                {
                    List<string> st = itemList[Constants.FeatureScore];
                    foreach (string sin in st)
                    {
                        if (0 != string.Compare(scoreValues[i], sin,
                            CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                            return false;
                    }
                }
                catch (KeyNotFoundException) { }

                // Validate Strand
                try
                {
                    List<string> st = itemList[Constants.FeatureStrand];
                    foreach (string sin in st)
                    {
                        if (0 != string.Compare(strandValues[i], sin,
                            CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                            return false;
                    }
                }
                catch (KeyNotFoundException) { }

                // Validate Source
                try
                {
                    List<string> st = itemList[Constants.FeatureSource];
                    foreach (string sin in st)
                    {
                        if (0 != string.Compare(sources[i], sin,
                            CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                            return false;
                    }
                }
                catch (KeyNotFoundException) { }

                // Validate End
                try
                {
                    List<string> st = itemList[Constants.FeatureEnd];
                    foreach (string sin in st)
                    {
                        if (0 != string.Compare(endValues[i], sin,
                             CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                            return false;
                    }
                }
                catch (KeyNotFoundException) { }

                // Validate Frame
                try
                {
                    List<string> st = itemList[Constants.FeatureFrame];
                    foreach (string sin in st)
                    {
                        if (0 != string.Compare(frameValues[i], sin,
                             CultureInfo.CurrentCulture,CompareOptions.IgnoreCase))
                            return false;
                    }
                }
                catch (KeyNotFoundException) { }

                if (0 != string.Compare(feature.FreeText,
                    attributeValues[i], CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                    return false;

                if (0 != string.Compare(feature.Key,
                    featureNames[i], CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                    return false;

                if (0 != string.Compare(seq.ID,
                    sequenceNames[i], CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                    return false;

                i++;
            }

            return true;
        }

        /// <summary>
        /// Validates the Metadata Features of a Gff Multi Sequence for the sequence 
        /// and node name specified.
        /// </summary>
        /// <param name="seq">Sequence that needs to be validated.</param>
        /// <param name="seqNumber">Sequence Number</param>
        /// <param name="nodeName">Xml Node name to be read.</param>
        /// <returns>True/False</returns>
        bool ValidateMultiSequenceFeatures(ISequence seq,
            int seqNumber, string nodeName)
        {
            // Gets all the Features from the Sequence for Validation
            List<MetadataListItem<List<string>>> featureList =
                (List<MetadataListItem<List<string>>>)seq.Metadata[Constants.Features];

            // Gets all the xml values for validation
            string[] sequenceNames = null;
            string[] sources = null;
            string[] featureNames = null;
            string[] startValues = null;
            string[] endValues = null;
            string[] scoreValues = null;
            string[] strandValues = null;
            string[] frameValues = null;
            string[] attributeValues = null;

            switch (seqNumber)
            {
                case 1:
                    sequenceNames = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.SequenceNameNode1Name);
                    sources = utilityObj.xmlUtil.GetTextValues(nodeName
                        , Constants.SourceNode1Name);
                    featureNames = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.FeatureNameNode1Name);
                    startValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.StartNode1Name);
                    endValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.EndNode1Name);
                    scoreValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.ScoreNode1Name);
                    strandValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.StrandNode1Name);
                    frameValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.FrameNode1Name);
                    attributeValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.AttributesNode1Name);
                    break;
                case 2:
                    sequenceNames = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.SequenceNameNode2Name);
                    sources = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.SourceNode2Name);
                    featureNames = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.FeatureNameNode2Name);
                    startValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.StartNode2Name);
                    endValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.EndNode2Name);
                    scoreValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.ScoreNode2Name);
                    strandValues = utilityObj.xmlUtil.GetTextValues(nodeName
                        , Constants.StrandNode2Name);
                    frameValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.FrameNode2Name);
                    attributeValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.AttributesNode2Name);
                    break;
                case 3:
                    sequenceNames = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.SequenceNameNode3Name);
                    sources = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.SourceNode3Name);
                    featureNames = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.FeatureNameNode3Name);
                    startValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.StartNode3Name);
                    endValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.EndNode3Name);
                    scoreValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.ScoreNode3Name);
                    strandValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.StrandNode3Name);
                    frameValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.FrameNode3Name);
                    attributeValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.AttributesNode3Name);
                    break;
                case 4:
                    sequenceNames = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.SequenceNameNode4Name);
                    sources = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.SourceNode4Name);
                    featureNames = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.FeatureNameNode4Name);
                    startValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.StartNode4Name);
                    endValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.EndNode4Name);
                    scoreValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.ScoreNode4Name);
                    strandValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.StrandNode4Name);
                    frameValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.FrameNode4Name);
                    attributeValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                        Constants.AttributesNode4Name);
                    break;
                default:
                    break;
            }

            int i = 0;

            // Loop through each and every feature and validate the same.
            foreach (MetadataListItem<List<string>> feature in featureList)
            {
                Dictionary<string, List<string>> itemList = feature.SubItems;

                // Read specific feature Item and validate
                // Validate Start
                try
                {
                    List<string> st = itemList[Constants.FeatureStart];
                    foreach (string sin in st)
                    {
                        if (0 != string.Compare(startValues[i],
                            sin, CultureInfo.CurrentCulture,CompareOptions.IgnoreCase))
                            return false;
                    }
                }
                catch (KeyNotFoundException) { }

                // Validate Score
                try
                {
                    List<string> st = itemList[Constants.FeatureScore];
                    foreach (string sin in st)
                    {
                        if (0 != string.Compare(scoreValues[i],
                            sin, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                            return false;
                    }
                }
                catch (KeyNotFoundException) { }

                // Validate Strand
                try
                {
                    List<string> st = itemList[Constants.FeatureStrand];
                    foreach (string sin in st)
                    {
                        if (0 != string.Compare(strandValues[i], sin,
                            CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                            return false;
                    }
                }
                catch (KeyNotFoundException) { }

                // Validate Source
                try
                {
                    List<string> st = itemList[Constants.FeatureSource];
                    foreach (string sin in st)
                    {
                        if (0 != string.Compare(sources[i], sin,
                            CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                            return false;
                    }
                }
                catch (KeyNotFoundException) { }

                // Validate End
                try
                {
                    List<string> st = itemList[Constants.FeatureEnd];
                    foreach (string sin in st)
                    {
                        if (0 != string.Compare(endValues[i], sin,
                            CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                            return false;
                    }
                }
                catch (KeyNotFoundException) { }

                // Validate Frame
                try
                {
                    List<string> st = itemList[Constants.FeatureFrame];
                    foreach (string sin in st)
                    {
                        if (0 != string.Compare(frameValues[i], sin,
                            CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                            return false;
                    }
                }
                catch (KeyNotFoundException) { }

                if (0 != string.Compare(feature.FreeText, attributeValues[i],
                    CultureInfo.CurrentCulture,CompareOptions.IgnoreCase))
                    return false;

                if (0 != string.Compare(feature.Key, featureNames[i],
                    CultureInfo.CurrentCulture,CompareOptions.IgnoreCase))
                    return false;

                if (0 != string.Compare(seq.ID, sequenceNames[i],
                    CultureInfo.CurrentCulture,CompareOptions.IgnoreCase))
                    return false;

                i++;
            }

            return true;
        }

        /// <summary>
        /// Validates the Format() method in Gff Formatter based on the parameters.
        /// </summary>
        /// <param name="nodeName">Xml Node name to be read.</param>
        /// <param name="isFilePath">Is file path passed as parameter?</param>
        /// <param name="isSequenceList">Is sequence list passed as parameter?</param>
        void ValidateFormatGeneralTestCases(string nodeName, bool isFilePath, bool isSequenceList)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);

            Assert.IsTrue(File.Exists(filePath));
            GffParser parserObj = new GffParser();
            IList<ISequence> seqs = parserObj.Parse(filePath).ToList();
            var originalSequence = seqs[0];

            // Use the formatter to write the original sequences to a temp file            
            ApplicationLog.WriteLine(string.Format("Gff Formatter P1: Creating the Temp file '{0}'.", Constants.GffTempFileName));

            GffFormatter formatter = new GffFormatter { ShouldWriteSequenceData = true };
            if (isFilePath)
            {
                if (isSequenceList)
                    formatter.Format(seqs, Constants.GffTempFileName);
                else
                    formatter.Format(originalSequence, Constants.GffTempFileName);
            }
            else
            {
                using (var stream = File.Create(Constants.GffTempFileName))
                {
                    if (isSequenceList)
                    {
                        formatter.Format(stream, seqs);
                    }
                    else
                    {
                        formatter.Format(stream, originalSequence);
                    }
                }
            }

            // Read the new file, then compare the sequences
            GffParser newParser = new GffParser(); 
            IList<ISequence> seqsNew = newParser.Parse(Constants.GffTempFileName).ToList();

            ISequence seqTemp = seqsNew.First();
            string sequenceInString = seqTemp.ConvertToString();

            Assert.IsNotNull(seqTemp);
            ApplicationLog.WriteLine(string.Format("Gff Formatter P1: New Sequence is '{0}'.", sequenceInString));

            bool val = ValidateFeatures(seqTemp, nodeName);
            Assert.IsTrue(val);
            ApplicationLog.WriteLine("GFF Formatter P1 : All the features validated successfully.");

            // Now compare the sequences.
            int countNew = seqsNew.Count();
            Assert.AreEqual(1, countNew);
            ApplicationLog.WriteLine("The Number of sequences are matching.");
            Assert.AreEqual(originalSequence.ID, seqTemp.ID);           
            string orgSeq = new string(originalSequence.Select(x => (char)x).ToArray());           
            string newSeq = new string(seqTemp.Select(x => (char)x).ToArray());

            Assert.AreEqual(orgSeq, newSeq);
            ApplicationLog.WriteLine(string.Format("Gff Formatter P1: The Gff sequences '{0}' are matching with Format() method.", newSeq));

            // Passed all the tests, delete the tmp file. If we failed an Assert,
            // the tmp file will still be there in case we need it for debugging.
            if (File.Exists(Constants.GffTempFileName))
                File.Delete(Constants.GffTempFileName);
            ApplicationLog.WriteLine("Deleted the temp file created.");
        }

        /// <summary>
        /// Validate Format string test cases for a give node name.
        /// </summary>
        /// <param name="nodeName">Node name</param>
        void ValidateFormatStringTestCases(string nodeName)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.FilePathNode);

            Assert.IsTrue(File.Exists(filePath));
            IList<ISequence> seqs = null;
            GffParser parserObj = new GffParser();
            seqs = parserObj.Parse(filePath).ToList();
            Sequence originalSequence = (Sequence)seqs[0];

            // Use the formatter to write the original sequences to a temp file            
            ApplicationLog.WriteLine(string.Format("Gff Formatter P1: Creating the Temp file '{0}'.",
                Constants.GffTempFileName));

            GffFormatter formatter = new GffFormatter { ShouldWriteSequenceData = true };
            string formatString = formatter.FormatString(originalSequence);

            string expectedString = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.FormatStringNode);

            expectedString =
                expectedString.Replace("current-date",
                DateTime.Today.ToString("yyyy-MM-dd", null));
            expectedString =
                expectedString.Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace("\t", "");
            string modifedformatString =
                formatString.Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace("\t", "");

            Assert.AreEqual(expectedString.ToLower(CultureInfo.CurrentCulture),
                modifedformatString.ToLower(CultureInfo.CurrentCulture));
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Formatter P1: The Gff Format String '{0}' are matching with FormatString() method and is as expected.",
                formatString));

            // Passed all the tests, delete the tmp file. If we failed an Assert,
            // the tmp file will still be there in case we need it for debugging.
            if (File.Exists(Constants.GffTempFileName))
                File.Delete(Constants.GffTempFileName);
            ApplicationLog.WriteLine("Deleted the temp file created.");
        }

        /// <summary>
        /// Validates the Format() method in Gff Formatter for Multi sequences based on the parameters.
        /// </summary>
        /// <param name="nodeName">Xml Node name to be read.</param>
        void ValidateFormatMultiSequencesTestCases(string nodeName)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.FilePathNode);

            Assert.IsTrue(File.Exists(filePath));
            IList<ISequence> seqs = null;
            GffParser parserObj = new GffParser();
            seqs = parserObj.Parse(filePath).ToList();

            // Use the formatter to write the original sequences to a temp file            
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Formatter P1: Creating the Temp file '{0}'.",
                Constants.GffTempFileName));

            GffFormatter formatter = new GffFormatter { ShouldWriteSequenceData = true };
            formatter.Format(seqs, Constants.GffTempFileName);

            int noOfSeqs = int.Parse(utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.NumberOfSequencesNode), null);
            Assert.IsNotNull(seqs);
            Assert.AreEqual(noOfSeqs, seqs.Count);
            ApplicationLog.WriteLine(string.Format("Gff Formatter P1 : Number of Sequences found are '{0}'.",
                seqs.Count.ToString((IFormatProvider)null)));

            string[] expectedSequences = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.ExpectedSequenesNode);
            string[] alphabets = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.AlphabetsNode);
            string[] seqIds = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.SequenceIdsNode);

            for (int i = 0; i < noOfSeqs; i++)
            {
                bool valFeat = ValidateMultiSequenceFeatures(seqs[i], i + 1, nodeName);

                Assert.IsTrue(valFeat);
                ApplicationLog.WriteLine(
                    "Gff Formatter P1 : Successfully validated all the Features for a give Sequence in GFF File.");

                Sequence seq = (Sequence)seqs[i];
                Assert.IsNotNull(seq);

                string newSeq = new string(seq.Select(x => (char)x).ToArray());

                if (string.Compare(newSeq, newSeq.ToUpper(CultureInfo.CurrentCulture)) == 0)
                {
                    expectedSequences[i] = expectedSequences[i].ToUpper(CultureInfo.CurrentCulture);
                }
                else
                {
                    expectedSequences[i] = expectedSequences[i].ToLower(CultureInfo.CurrentCulture);
                }
                Assert.AreEqual(expectedSequences[i], newSeq);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "Gff Formatter P1: The Gff sequence '{0}' validation after Parse() is found to be as expected.",
                    newSeq));

                byte[] TempSeqData = new byte[seq.Count];
                for (int j = 0; j < seq.Count; j++)
                {
                    TempSeqData[j] = seq[j];
                }

                Assert.AreEqual(expectedSequences[i].Length, TempSeqData.Length);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "Gff Formatter P1: The Gff Length sequence '{0}' is as expected.",
                    expectedSequences[i].Length));

                Assert.IsNotNull(seq.Alphabet);
                Assert.AreEqual(seq.Alphabet.Name.ToLower(CultureInfo.CurrentCulture),
                    alphabets[i]);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "Gff Formatter P1: The Sequence Alphabet is '{0}' and is as expected.",
                    seq.Alphabet.Name));

                Assert.AreEqual(seqIds[i], seq.ID);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "Gff Formatter P1: The Sequence ID is '{0}' and is as expected.",
                    seq.ID));
            }

            // Passed all the tests, delete the tmp file. If we failed an Assert,
            // the tmp file will still be there in case we need it for debugging.
            if (File.Exists(Constants.GffTempFileName))
                File.Delete(Constants.GffTempFileName);
            ApplicationLog.WriteLine("Deleted the temp file created.");
        }

        #endregion Supported Methods
    }
}
