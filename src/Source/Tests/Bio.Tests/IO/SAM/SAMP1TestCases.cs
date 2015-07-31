/****************************************************************************
 * SAMP1TestCases.cs
 * 
 *   This file contains the Sam - Parsers and Formatters P1 test cases.
 * 
***************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Linq;

using Bio.Algorithms.Alignment;
using Bio.IO.FastA;
using Bio.IO.SAM;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using NUnit.Framework;
using Bio.IO;
using System;
using System.Runtime.Serialization;
using Bio;

namespace Bio.TestAutomation.IO.SAM
{
    /// <summary>
    /// SAM P1 parser and formatter Test case implementation.
    /// </summary>
    [TestFixture]
    public class SAMP1TestCases
    {

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\SAMBAMTestData\SAMBAMTestsConfig.xml");

        #endregion Global Variables

        
        #region Test Cases

        #region SAM Parser TestCases

        /// <summary>
        /// Validate Parse(reader) by parsing medium size
        /// dna sam file.
        /// Input : medium size sam file
        /// Output: Validation of Sequence Alignment Map 
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateSAMParserWithReader()
        {
            ValidateSAMParser(Constants.SAMFileWithRefNode);
        }

        /// <summary>
        /// Validate Parse(BioReader, isReadOnly) by parsing empty
        /// SAM file.
        /// Input : Empty file
        /// Output: Validation of null Sequence Alignment Map 
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateSAMParserWithEmptyAlignmentMap()
        {
            SAMParser parser = new SAMParser();
            {
                SequenceAlignmentMap alignment = parser.ParseOne<SequenceAlignmentMap>(utilityObj.xmlUtil.GetTextValue(Constants.EmptySamFileNode, Constants.FilePathNode));
                Assert.IsNotNull(alignment);
            }
        }

        #endregion

        #region Formatter TestCases

        /// <summary>
        /// Validate SAM Formatter Format(sequenceAlignmentMap, writer) by
        /// parsing and formatting the medium size dna sam file
        /// Input : alignment
        /// Output: sam file
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateSAMFormatterSeqAlignReader()
        {
            ValidateSAMFormatter(Constants.SAMFileWithAllFieldsNode);
        }

        #endregion Formatter TestCases

        #endregion Test Cases

        #region Helper Methods

        /// <summary>
        /// General method to validate SAM parser method.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="parseTypes">enum type to execute different overload</param>
        void ValidateSAMParser(string nodeName)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedSequenceFile = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequence);
            var parser = new SAMParser();
            {
                SequenceAlignmentMap alignments = null;

                // Parse SAM File
                using (var reader = File.OpenRead(filePath))
                {
                    alignments = parser.Parse(reader);
                }

                // Get expected sequences
                FastAParser parserObj = new FastAParser();
                {
                    IEnumerable<ISequence> expectedSequences = parserObj.Parse(expectedSequenceFile);
                    IList<ISequence> expectedSequencesList = expectedSequences.ToList();

                    // Validate parsed output with expected output
                    for (int index = 0;
                        index < alignments.QuerySequences.Count;
                        index++)
                    {
                        for (int count = 0;
                            count < alignments.QuerySequences[index].Sequences.Count;
                            count++)
                        {
                            Assert.AreEqual(new string(expectedSequencesList[index].Select(a => (char)a).ToArray()),
                                new string(alignments.QuerySequences[index].Sequences[count].Select(a => (char)a).ToArray()));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// General method to validate SAM Formatter method.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="parseTypes">enum type to execute different overload</param>
        void ValidateSAMFormatter(string nodeName)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedSequenceFile = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequence);
            SAMParser parser = new SAMParser();
            {
                SequenceAlignmentMap alignments = (SequenceAlignmentMap) parser.ParseOne(filePath);
                SAMFormatter formatter = new SAMFormatter();

                using (var writer =
                            File.Create(Constants.SAMTempFileName))
                {
                    formatter.Format(writer, alignments);
                }

                alignments = parser.ParseOne<SequenceAlignmentMap>(Constants.SAMTempFileName);

                // Get expected sequences
                FastAParser parserObj = new FastAParser();
                {
                    IEnumerable<ISequence> expectedSequences =
                        parserObj.Parse(expectedSequenceFile);

                    IList<ISequence> expectedSequencesList = expectedSequences.ToList();

                    // Validate parsed output with expected output
                    for (int index = 0;
                        index < alignments.QuerySequences.Count;
                        index++)
                    {
                        for (int count = 0;
                            count < alignments.QuerySequences[index].Sequences.Count;
                            count++)
                        {
                            Assert.AreEqual(
                                new string(expectedSequencesList[index].Select(a => (char)a).ToArray()),
                                new string(alignments.QuerySequences[index].Sequences[count].Select(a => (char)a).ToArray()));
                        }
                    }
                }
            }
        }

        #endregion
    }
}
