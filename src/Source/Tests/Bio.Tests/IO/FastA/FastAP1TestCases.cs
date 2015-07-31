/****************************************************************************
 * FastaP1TestCases.cs
 * 
 *   This file contains the FastA - Parsers and Formatters Priority One test cases.
 * 
***************************************************************************/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using Bio.Extensions;
using Bio.IO.FastA;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using NUnit.Framework;

#if (SILVERLIGHT == false)
namespace Bio.TestAutomation.IO.FastA
#else
namespace Bio.Silverlight.TestAutomation.IO.FastA
#endif
{
    /// <summary>
    ///     FASTA Priority One parser and formatter test cases implementation.
    /// </summary>
    [TestFixture]
    public class FastAP1TestCases
    {
        #region Global Variables

        private readonly Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

       
        #region FastA Parser P1 Test cases

        /// <summary>
        ///     Parse a valid FastA file (DNA) and using Parse(file-name) method and
        ///     validate the expected sequence
        ///     Input : DNA FastA File
        ///     Validation : Read the FastA file to which the sequence was formatted and
        ///     validate Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastAParserValidateParseWithDnaSequence()
        {
            ValidateParseGeneralTestCases(Constants.SimpleFastaDnaNodeName);
        }

        /// <summary>
        ///     Parse a valid FastA file (Protein) and using Parse(file-name) method and
        ///     validate the expected sequence
        ///     Input : Protein FastA File
        ///     Validation : Read the FastA file to which the sequence was formatted and
        ///     validate Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastAParserValidateParseWithProteinSequence()
        {
            ValidateParseGeneralTestCases(Constants.SimpleFastaProteinNodeName);
        }

        /// <summary>
        ///     Parse a valid FastA file (RNA) and using Parse(file-name) method and
        ///     validate the expected sequence
        ///     Input : RNA FastA File
        ///     Validation : Read the FastA file to which the sequence was formatted and
        ///     validate Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastAParserValidateParseWithRnaSequence()
        {
            ValidateParseGeneralTestCases(Constants.SimpleFastaRnaNodeName);
        }

        /// <summary>
        ///     Parse a valid FastA file (DNA) which is of less than 100KB
        ///     and using Parse(file-name) method and validate the expected
        ///     sequence
        ///     Input : Medium Size FastA File
        ///     Validation : Read the FastA file to which the sequence was formatted and
        ///     validate Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastAParserValidateParseWithMediumSizeSequence()
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.MediumSizeFastaNodeName, Constants.FilePathNode);
            string alphabet = utilityObj.xmlUtil.GetTextValue(Constants.MediumSizeFastaNodeName,
                                                              Constants.AlphabetNameNode);
            Assert.IsTrue(File.Exists(filePath));
            // Logs information to the log file
            ApplicationLog.WriteLine(string.Format(null,
                                                   "FastA Parser : File Exists in the Path '{0}'.", filePath));

            IEnumerable<ISequence> seqs = null;
            var parserObj = new FastAParser();
            {
                parserObj.Alphabet = Utility.GetAlphabet(alphabet);
                seqs = parserObj.Parse(filePath);

                Assert.IsNotNull(seqs);
                Assert.AreEqual(1, seqs.Count());
                ApplicationLog.WriteLine(string.Format(null,
                                                       "FastA Parser: Number of Sequences found are '{0}'.",
                                                       seqs.Count()));

                // Gets the expected sequence from the Xml
                string expectedSequence = utilityObj.xmlUtil.GetTextValue(
                    Constants.MediumSizeFastaNodeName, Constants.ExpectedSequenceNode);

                var seq = (Sequence) seqs.ElementAt(0);
                char[] seqString = seqs.ElementAt(0).Select(a => (char) a).ToArray();
                var newSequence = new string(seqString);
                Assert.IsNotNull(seq);

                // Replace all the empty spaces, paragraphs and new line for validation
                string updatedExpSequence =
                    expectedSequence.Replace("\r", "").Replace("\n", "").Replace(" ", "");
                string updatedActualSequence =
                    newSequence.Replace("\r", "").Replace("\n", "").Replace(" ", "");
                Assert.AreEqual(updatedExpSequence, updatedActualSequence);
                ApplicationLog.WriteLine(
                    string.Format(null, "FastA Parser: Sequence is '{0}' and is as expected.",
                                  updatedActualSequence));

                Assert.AreEqual(updatedExpSequence.Length, updatedActualSequence.Length);
                ApplicationLog.WriteLine(
                    string.Format(null, "FastA Parser: Sequence Length is '{0}' and is as expected.",
                                  updatedActualSequence.Length));

                Assert.IsNotNull(seq.Alphabet);
                Assert.AreEqual(seq.Alphabet.Name.ToLower(CultureInfo.CurrentCulture),
                                utilityObj.xmlUtil.GetTextValue(Constants.MediumSizeFastaNodeName,
                                                                Constants.AlphabetNameNode)
                                          .ToLower(CultureInfo.CurrentCulture));
                ApplicationLog.WriteLine(string.Format(null,
                                                       "FastA Parser: The Sequence Alphabet is '{0}' and is as expected.",
                                                       seq.Alphabet.Name));

                Assert.AreEqual(utilityObj.xmlUtil.GetTextValue(
                    Constants.MediumSizeFastaNodeName, Constants.SequenceIdNode), seq.ID);
                ApplicationLog.WriteLine(string.Format(null,
                                                       "FastA Parser: Sequence ID is '{0}' and is as expected.", seq.ID));
            }
        }

        /// <summary>
        ///     Parse a valid FastA file with one line sequence and using
        ///     Parse(file-name) method and validate the expected sequence
        ///     Input : One line sequence FastA File
        ///     Validation : Read the FastA file to which the sequence was formatted and
        ///     validate Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastAParserValidateParseWithOneLineSequence()
        {
            ValidateParseGeneralTestCases(Constants.OneLineSequenceFastaNodeName);
        }

        /// <summary>
        ///     Parse a valid FastA file with Alphabet passed as property
        ///     and using Parse(file-name) method and
        ///     validate the expected sequence
        ///     Input : DNA FastA File with Encoding specified
        ///     Validation : Read the FastA file to which the sequence was formatted and
        ///     validate Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastAParserValidateParseWithAlphabetAsProperty()
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(Constants.SimpleFastaDnaNodeName, Constants.FilePathNode);
            Assert.IsTrue(File.Exists(filePath));
            ApplicationLog.WriteLine(string.Format("FastA Parser : File Exists in the Path '{0}'.", filePath));

            var parserObj = new FastAParser();
            using (parserObj.Open(filePath))
            {
                parserObj.Alphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(Constants.SimpleFastaDnaNodeName,
                                                    Constants.AlphabetNameNode));

                ValidateParserGeneralTestCases(parserObj);
            }
        }

        #endregion FastA Parser P1 Test cases

        #region FastA Formatter P1 Test cases

        /// <summary>
        ///     Format a valid DNA Sequence to a
        ///     FastA file Format() method and validate the same.
        ///     Input : FastA DNA Sequence
        ///     Validation : Read the FastA file to which the sequence was formatted and
        ///     validate Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastAFormatterValidateFormatWithDnaSequence()
        {
            ValidateFormatterGeneralTestCases(Constants.SimpleFastaDnaNodeName);
        }

        /// <summary>
        ///     Format a valid RNA Sequence to a
        ///     FastA file Format() method and validate the same.
        ///     Input : FastA RNA Sequence
        ///     Validation : Read the FastA file to which the sequence was formatted and
        ///     validate Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastAFormatterValidateFormatWithRnaSequence()
        {
            ValidateFormatterGeneralTestCases(Constants.SimpleFastaRnaNodeName);
        }

        /// <summary>
        ///     Format a valid Protein Sequence to a
        ///     FastA file Format() method and validate the same.
        ///     Input : FastA Protein Sequence
        ///     Validation : Read the FastA file to which the sequence was formatted and
        ///     validate Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastAFormatterValidateFormatWithProteinSequence()
        {
            ValidateFormatterGeneralTestCases(Constants.SimpleFastaProteinNodeName);
        }

        /// <summary>
        ///     Parse a FastA DNA File using Parse() method and Format the
        ///     same to a FastA file using Format() method and validate the same.
        ///     Input : FastA DNA File which would be parsed
        ///     Validation : Read the New FastA file to which the sequence was formatted and
        ///     validate Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastAFormatterValidateWithParseDnaSequence()
        {
            ValidateParseFormatGeneralTestCases(Constants.SimpleFastaDnaNodeName);
        }

        /// <summary>
        ///     Parse a FastA RNA File using Parse() method and Format the
        ///     same to a FastA file using Format() method and validate the same.
        ///     Input : FastA RNA File which would be parsed
        ///     Validation : Read the New FastA file to which the sequence was formatted and
        ///     validate Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastAFormatterValidateWithParseRnaSequence()
        {
            ValidateParseFormatGeneralTestCases(Constants.SimpleFastaRnaNodeName);
        }

        /// <summary>
        ///     Parse a FastA Protein File using Parse() method and Format the
        ///     same to a FastA file using Format() method and validate the same.
        ///     Input : FastA Protein File which would be parsed
        ///     Validation : Read the New FastA file to which the sequence was formatted and
        ///     validate Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastAFormatterValidateWithParseProteinSequence()
        {
            ValidateParseFormatGeneralTestCases(Constants.SimpleFastaProteinNodeName);
        }

        /// <summary>
        ///     Format a valid medium size i.e., less than 100KB Fasta File
        ///     using Format() method and validate the same.
        ///     Input : Medium size FastA file
        ///     Validation : Read the FastA file to which the sequence was formatted and
        ///     validate Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastAFormatterValidateFormatWithMediumSizeSequence()
        {
            ValidateFormatterGeneralTestCases(Constants.MediumSizeFastaNodeName);
        }

        /// <summary>
        ///     Format a valid large size i.e., greater than 100 KB and less tha 350 KB
        ///     using Format() method and validate the same.
        ///     Input : Large size FastA file
        ///     Validation : Read the FastA file to which the sequence was formatted and
        ///     validate Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastAFormatterValidateFormatWithLargeSizeSequence()
        {
            ValidateFormatterGeneralTestCases(Constants.LargeSizeFasta);
        }


        /// <summary>
        ///     Parse a medium size FastA File i.e., less than 100 KB
        ///     using Parse() method and Format the
        ///     same to a FastA file using Format() method and validate the same.
        ///     Input : Medium size FastA File which would be parsed
        ///     Validation : Read the New FastA file to which the sequence was formatted and
        ///     validate Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastAFormatterValidateWithParseMediumSizeSequence()
        {
            ValidateParseFormatGeneralTestCases(Constants.MediumSizeFastaNodeName);
        }

        /// <summary>
        ///     Parse a large size FastA File i.e., greater than 100 KB and less than 350 KB
        ///     using Parse() method and Format the
        ///     same to a FastA file using Format() method and validate the same.
        ///     Input : Large size FastA File which would be parsed
        ///     Validation : Read the New FastA file to which the sequence was formatted and
        ///     validate Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastAFormatterValidateWithParseLargeSizeSequence()
        {
            ValidateParseFormatGeneralTestCases(Constants.LargeSizeFasta);
        }

        /// <summary>
        ///     Format a valid Sequence to a FastA file using Format() method and
        ///     validate the same by Parsing it back.
        ///     Input : FastA Sequence
        ///     Validation : Read the FastA file using Parse() and
        ///     validate Sequence, Sequence Count
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastAFormatterValidateWithFormatAndParse()
        {
            ValidateFormatterGeneralTestCases(Constants.SimpleFastaNodeName);
        }

        #endregion FastA Formatter P1 Test cases

        #region Supporting Methods

        /// <summary>
        ///     Validates general Parse test cases with the xml node name specified.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        private void ValidateParseGeneralTestCases(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string alphabet = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.AlphabetNameNode);

            Assert.IsTrue(File.Exists(filePath));

            var parserObj = new FastAParser();
            {
                parserObj.Alphabet = Utility.GetAlphabet(alphabet);
                IList<ISequence> seqs = parserObj.Parse(filePath).ToList();

                Assert.AreEqual(1, seqs.Count);

                // Gets the expected sequence from the Xml
                string expectedSequence = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode);

                ISequence seq = seqs[0];
                var newSequence = seq.ConvertToString();
                Assert.AreEqual(expectedSequence, newSequence);

                var tmpEncodedSeq = new byte[seq.Count];
                seq.ToArray().CopyTo(tmpEncodedSeq, 0);
                Assert.AreEqual(expectedSequence.Length, tmpEncodedSeq.Length);

                Assert.IsNotNull(seq.Alphabet);
                Assert.AreEqual(seq.Alphabet.Name.ToLower(CultureInfo.CurrentCulture), utilityObj.xmlUtil.GetTextValue(nodeName, Constants.AlphabetNameNode).ToLower(CultureInfo.CurrentCulture));
                Assert.AreEqual(utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceIdNode), seq.ID);
            }
        }

        /// <summary>
        ///     Validates general Parse test cases with Fasta parser object name specified.
        /// </summary>
        /// <param name="parserObj">fasta parser object.</param>
        private void ValidateParserGeneralTestCases(FastAParser parserObj)
        {
            IList<ISequence> seqs = parserObj.Parse().ToList();
            Assert.AreEqual(1, seqs.Count);
            ApplicationLog.WriteLine("FastA Parser with Alphabet: Number of Sequences found are '{0}'.", seqs.Count);

            // Gets the expected sequence from the Xml
            string expectedSequence = utilityObj.xmlUtil.GetTextValue(Constants.SimpleFastaDnaNodeName, Constants.ExpectedSequenceNode);
            string newSequence = seqs[0].ConvertToString();
            Assert.AreEqual(expectedSequence, newSequence);

            ISequence seq = seqs[0];
            var tmpEncodedSeq = new byte[seq.Count];
            seq.ToArray().CopyTo(tmpEncodedSeq, 0);
            Assert.AreEqual(expectedSequence.Length, tmpEncodedSeq.Length);

            Assert.IsNotNull(seq.Alphabet);
            Assert.AreEqual(seq.Alphabet.Name.ToLower(CultureInfo.CurrentCulture), utilityObj.xmlUtil.GetTextValue(Constants.SimpleFastaDnaNodeName, Constants.AlphabetNameNode).ToLower(CultureInfo.CurrentCulture));
            Assert.AreEqual(utilityObj.xmlUtil.GetTextValue(Constants.SimpleFastaDnaNodeName, Constants.SequenceIdNode), seq.ID);
        }

        /// <summary>
        ///     Validates general FastA Formatter test cases with the xml node name specified.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private void ValidateFormatterGeneralTestCases(string nodeName)
        {
            // Gets the actual sequence and the alphabet from the Xml
            string expectedSequence = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode);
            string formattedSequence = expectedSequence.Replace("\r", "").Replace("\n", "").Replace(" ", "");

            string alphabet = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.AlphabetNameNode);

            // Logs information to the log file
            ApplicationLog.WriteLine(string.Format(null,
                                                   "FastA Formatter : Validating with Sequence '{0}' and Alphabet '{1}'.",
                                                   expectedSequence, alphabet));

            // Replacing all the empty characters, Paragraphs and null entries added 
            // while formatting the xml.
            ISequence seqOriginal = new Sequence(Utility.GetAlphabet(alphabet), formattedSequence) {ID = "test"};
            Assert.IsNotNull(seqOriginal);

            // Write it to a file
            var formatter = new FastAFormatter();
            {
                // Use the formatter to write the original sequences to a temp file
                ApplicationLog.WriteLine(string.Format("FastA Formatter : Creating the Temp file '{0}'.",
                                                       Constants.FastaTempFileName));
                formatter.Format(seqOriginal, Constants.FastaTempFileName);
            }

            // Read the new file, then compare the sequences
            var parserObj = new FastAParser();
            {
                parserObj.Alphabet = Utility.GetAlphabet(alphabet);
                IEnumerable<ISequence> seqsNew = parserObj.Parse(Constants.FastaTempFileName);

                // Get a single sequence
                ISequence seqNew = seqsNew.FirstOrDefault();
                Assert.IsNotNull(seqNew);

                string newSequence = seqNew.ConvertToString();
                ApplicationLog.WriteLine(string.Format(null, "FastA Formatter : New Sequence is '{0}'.", newSequence));
                Assert.AreEqual(formattedSequence, newSequence);
                Assert.AreEqual(seqOriginal.ID, seqNew.ID);

                // Verify only one sequence exists.
                Assert.AreEqual(1, seqsNew.Count());
            }

            // Passed all the tests, delete the tmp file. If we failed an Assert,
            // the tmp file will still be there in case we need it for debugging.
            File.Delete(Constants.FastaTempFileName);
            ApplicationLog.WriteLine("Deleted the temp file created.");
        }

        /// <summary>
        ///     Validates general FastA Parser test cases which are further Formatted
        ///     with the xml node name specified.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        private void ValidateParseFormatGeneralTestCases(string nodeName)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string alphabet = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.AlphabetNameNode);
            Assert.IsTrue(File.Exists(filePath));
            string filepathTmp = Path.Combine(Path.GetTempPath(), "temp.fasta");

            // Ensure output is deleted
            if (File.Exists(filepathTmp))
                File.Delete(filepathTmp);

            List<ISequence> seqsOriginal;
            var parserObj = new FastAParser();
            {
                // Read the original file
                parserObj.Alphabet = Utility.GetAlphabet(alphabet);
                seqsOriginal = parserObj.Parse(filePath).ToList();
                Assert.IsFalse(seqsOriginal.Count == 0);
            }

            // Write to a new file
            var formatter = new FastAFormatter();
            formatter.Format(seqsOriginal, filepathTmp);
            
            try
            {
                // Compare original with new file
                var parserObjNew = new FastAParser();
                {
                    // Read the new file, then compare the sequences
                    parserObjNew.Alphabet = Utility.GetAlphabet(alphabet);
                    IEnumerable<ISequence> seqsNew = parserObjNew.Parse(filepathTmp);
                    Assert.IsNotNull(seqsNew);

                    int count = 0;
                    foreach (ISequence newSequence in seqsNew)
                    {
                        string s1 = seqsOriginal[count].ConvertToString();
                        string s2 = newSequence.ConvertToString();
                        Assert.AreEqual(s1, s2);
                        count++;
                    }

                    Assert.AreEqual(count, seqsOriginal.Count, "Number of sequences is different.");
                }
            }
            finally
            {
                // Delete new file
                File.Delete(filepathTmp);
            }
        }

        #endregion Supporting Methods
    }
}