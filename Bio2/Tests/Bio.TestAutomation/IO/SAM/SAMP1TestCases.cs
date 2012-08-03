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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.IO;
using System;
using System.Runtime.Serialization;
using Bio;

namespace Bio.TestAutomation.IO.SAM
{
    /// <summary>
    /// SAM P1 parser and formatter Test case implementation.
    /// </summary>
    [TestClass]
    public class SAMP1TestCases
    {

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\SAMBAMTestData\SAMBAMTestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static SAMP1TestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region Test Cases

        #region SAM Parser TestCases

        /// <summary>
        /// Validate Parse(reader) by parsing medium size
        /// dna sam file.
        /// Input : medium size sam file
        /// Output: Validation of Sequence Alignment Map 
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
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
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateSAMParserWithEmptyAlignmentMap()
        {
            using (SAMParser parser = new SAMParser())
            {
                SequenceAlignmentMap alignment =
                    parser.Parse(utilityObj.xmlUtil.GetTextValue(
                    Constants.EmptySamFileNode,
                    Constants.FilePathNode));

                Assert.IsNotNull(alignment);
            }
        }

        /// <summary>
        /// Validate SAMAlignedSequence GetObjectData() method.
        /// Input : Valid values.
        /// Output : No exception are thrown.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateSAMAlignedSequenceGetObjectData()
        {
            SerializationInfo info = null;
            StreamingContext context = new StreamingContext(StreamingContextStates.All);

            SAMAlignedSequence sdObj = new SAMAlignedSequence();
            try
            {
                sdObj.GetObjectData(info, context);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                info = new SerializationInfo(typeof(SAMAlignedSequence),
                             new FormatterConverter());
                sdObj.GetObjectData(info, context);
            }

            Console.WriteLine("SAMAlignedSequence P1 : Successfully validated GetObjectData() method");
            ApplicationLog.WriteLine("SAMAlignedSequence P1 : Successfully validated GetObjectData() method");
        }

        /// <summary>
        /// Validate SequenceAlignmentMap GetObjectData() method.
        /// Input : Valid values.
        /// Output : No exception are thrown.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateSequenceAlignmentMapGetObjectData()
        {
            SerializationInfo info = null;
            StreamingContext context = new StreamingContext(StreamingContextStates.All);

            SequenceAlignmentMap samObj = new SequenceAlignmentMap();
            try
            {
                samObj.GetObjectData(info, context);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                info = new SerializationInfo(typeof(SequenceAlignmentMap),
                             new FormatterConverter());
                samObj.GetObjectData(info, context);
            }

            Console.WriteLine("SequenceAlignmentMap P1 : Successfully validated GetObjectData() method");
            ApplicationLog.WriteLine("SequenceAlignmentMap P1 : Successfully validated GetObjectData() method");
        }

        #endregion

        #region Formatter TestCases

        /// <summary>
        /// Validate SAM Formatter Format(sequenceAlignmentMap, writer) by
        /// parsing and formatting the medium size dna sam file
        /// Input : alignment
        /// Output: sam file
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
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
            using (SAMParser parser = new SAMParser())
            {
                SequenceAlignmentMap alignments = null;

                // Parse SAM File
                using (TextReader reader = new StreamReader(filePath))
                {
                    alignments = parser.Parse(reader);
                }

                // Get expected sequences
                using (FastAParser parserObj = new FastAParser(expectedSequenceFile))
                {
                    IEnumerable<ISequence> expectedSequences =
                        parserObj.Parse();
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
            using (SAMParser parser = new SAMParser())
            {
                SequenceAlignmentMap alignments = parser.Parse(filePath);
                SAMFormatter formatter = new SAMFormatter();

                using (TextWriter writer =
                            new StreamWriter(Constants.SAMTempFileName))
                {
                    formatter.Format(alignments, writer);
                }

                alignments = parser.Parse(Constants.SAMTempFileName);

                // Get expected sequences
                using (FastAParser parserObj = new FastAParser(expectedSequenceFile))
                {
                    IEnumerable<ISequence> expectedSequences =
                        parserObj.Parse();

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
