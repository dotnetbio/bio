/****************************************************************************
 * SAMBvtTestCases.cs
 * 
 *   This file contains the Sam - Parsers and Formatters Bvt test cases.
 * 
***************************************************************************/

using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Bio.Algorithms.Alignment;
using Bio.Extensions;
using Bio.IO;
using Bio.IO.FastA;
using Bio.IO.SAM;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using NUnit.Framework;

namespace Bio.TestAutomation.IO.SAM
{
    /// <summary>
    /// SAM Bvt parser and formatter Test case implementation.
    /// </summary>
    [TestFixture]
    public class SAMBvtTestCases
    {
        #region Enums

        /// <summary>
        /// Additional parameters to validate different scenarios.
        /// </summary>
        enum ParseOrFormatTypes
        {
            ParseOrFormatText,
            ParseOrFormatFileName,
        }

        #endregion Enums

        readonly Utility utilityObj = new Utility(@"TestUtils\SAMBAMTestData\SAMBAMTestsConfig.xml");

        #region Test Cases

        #region SAM Parser TestCases

        /// <summary>
        /// Validate SAM Parse(textreader) by parsing dna sam file.
        /// Input : sam file
        /// Output: alignments
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSAMParserWithTextReader()
        {
            ValidateSAMParser(Constants.SmallSAMFileNode, ParseOrFormatTypes.ParseOrFormatText);
        }

        /// <summary>
        /// Validate SAM Parse(filename) by parsing dna sam file.
        /// Input : sam file
        /// Output: alignments
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSAMParserWithFileName()
        {
            ValidateSAMParser(Constants.SmallSAMFileNode,
                ParseOrFormatTypes.ParseOrFormatFileName);
        }

        /// <summary>
        /// Validate SAM ParseOne(textreader) by parsing dna sam file.
        /// Input : sam file
        /// Output: alignments
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSAMParserParseOneWithTextReader()
        {
            ValidateSAMParserWithParseOne(Constants.SmallSAMFileNode,
                ParseOrFormatTypes.ParseOrFormatText);
        }

        /// <summary>
        /// Validate SAM ParseOne(filename) by parsing dna sam file.
        /// Input : sam file
        /// Output: alignments
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSAMParserParseOneWithFileName()
        {
            ValidateSAMParserWithParseOne(Constants.SmallSAMFileNode,
                ParseOrFormatTypes.ParseOrFormatFileName);
        }

        /// <summary>
        /// Validate properties in SAM Parser class
        /// Input : Create a SAM Parser object.
        /// Validation : Validate the properties
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSAMProperties()
        {
            SAMParser parser = new SAMParser();
            {
                Assert.AreEqual(Constants.SAMParserDescription, parser.Description);
                Assert.AreEqual(Constants.SAMFileType, parser.SupportedFileTypes);
                Assert.AreEqual(Constants.SAMName, parser.Name);
            }
            ApplicationLog.WriteLine("Successfully validated all the properties of SAM Parser class.");
        }

        /// <summary>
        /// Validate Parse(reader) by parsing dna sam file.
        /// Input : sam file
        /// Output: Validation of Sequence Alignment Map 
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSAMParserWithReader()
        {
            ValidateSAMParserSeqAlign(Constants.SmallSAMFileNode,
                ParseOrFormatTypes.ParseOrFormatText);
        }

        /// <summary>
        /// Validate ParserSAMHeader by parsing SAM file
        /// Input : SAM file
        /// Output: Validation of Sequence Alignment Header
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSAMParserHeader()
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.SmallSAMFileNode, Constants.FilePathNode);
            string[] expectedHeaderTagValues = utilityObj.xmlUtil.GetTextValue(
               Constants.SmallSAMFileNode, Constants.RecordTagValuesNode).Split(',');
            string[] expectedHeaderTagKeys = utilityObj.xmlUtil.GetTextValue(
                Constants.SmallSAMFileNode, Constants.RecordTagKeysNode).Split(',');
            string[] expectedHeaderTypes = utilityObj.xmlUtil.GetTextValue(
                Constants.SmallSAMFileNode, Constants.HeaderTyepsNodes).Split(',');
            SAMAlignmentHeader aligntHeader = SAMParser.ParseSAMHeader(File.OpenRead(filePath));

            int tagKeysCount = 0;
            int tagValuesCount = 0;

            for (int index = 0; index < aligntHeader.RecordFields.Count; index++)
            {
                Assert.AreEqual(expectedHeaderTypes[index].Replace("/", ""),
                     aligntHeader.RecordFields[index].Typecode.ToString(CultureInfo.InvariantCulture).Replace("/", ""));
                for (int tags = 0; tags < aligntHeader.RecordFields[index].Tags.Count; tags++)
                {
                    Assert.AreEqual(
                        expectedHeaderTagKeys[tagKeysCount].Replace("/", ""),
                        aligntHeader.RecordFields[index].Tags[tags].Tag.ToString(CultureInfo.InvariantCulture).Replace("/", ""));
                    Assert.AreEqual(
                        expectedHeaderTagValues[tagValuesCount].Replace("/", ""),
                        aligntHeader.RecordFields[index].Tags[tags].Value.ToString(CultureInfo.InvariantCulture).Replace("/", "").Replace("\r", "").Replace("\n", ""));
                    tagKeysCount++;
                    tagValuesCount++;
                }
            }

        }

        /// <summary>
        /// Validate ParseQualityNSequence() by parsing dna sam file.
        /// Input : sam file
        /// Output: Validation of Sequence Alignment Map 
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSAMParserQualityNSeq()
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.OneEmptySequenceSamFileNode, Constants.FilePathNode);
            string expectedSequence = utilityObj.xmlUtil.GetTextValue(
                Constants.OneEmptySequenceSamFileNode, Constants.ExpectedSequence);

            SAMParser parser = new SAMParser();
            {
                SequenceAlignmentMap alignments = null;

                using (var reader = File.OpenRead(filePath))
                {
                    alignments = parser.Parse(reader);
                }

                Assert.AreEqual(expectedSequence, alignments.QuerySequences[0].Sequences[0].ConvertToString());
                Assert.AreEqual(0, alignments.QuerySequences[1].Sequences.Count);
            }
        }

        #endregion

        #region SAM Formatter TestCases

        /// <summary>
        /// Validate SAM Formatter Format(alignment, filename) by parsing and 
        /// formatting the dna sam file
        /// Input : alignment
        /// Output: sam file
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSAMFormatterWithFileName()
        {
            ValidateSAMFormatter(Constants.SmallSAMFileNode,
                ParseOrFormatTypes.ParseOrFormatFileName);
        }

        /// <summary>
        /// Validate SAM Formatter Format(list of alignments, filename) by parsing and 
        /// formatting the dna sam file
        /// Input : alignment
        /// Output: sam file
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSAMFormatterWithFileNameAndAlignments()
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.SmallSAMFileNode.Replace("\r\n", System.Environment.NewLine), Constants.FilePathNode);
            ISequenceAlignmentParser parser = new SAMParser();
            IEnumerable<ISequenceAlignment> alignments = parser.Parse(filePath);
            SAMFormatter formatter = new SAMFormatter();
            try
            {
                formatter.Format(alignments, Constants.SAMTempFileName);
                Assert.Fail();
            }
            catch (NotSupportedException)
            {
                ApplicationLog.WriteLine("SAM Parser BVT : Validated the exception successfully");
            }
        }

        /// <summary>
        /// Validate SAM Formatter Format(list of alignments, textwriter) by 
        /// parsing and formatting the dna sam file
        /// Input : alignment
        /// Output: sam file
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSAMFormatterWithTextWriterAndAlignments()
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.SmallSAMFileNode, Constants.FilePathNode);
            ISequenceAlignmentParser parser = new SAMParser();
            IEnumerable<ISequenceAlignment> alignments = parser.Parse(filePath);
            SAMFormatter formatter = new SAMFormatter();
            try
            {
                using (var writer = File.Create(Constants.SAMTempFileName))
                {
                    formatter.Format(writer, alignments);
                }
                Assert.Fail();
            }
            catch (NotSupportedException)
            {
                ApplicationLog.WriteLine("SAM Parser BVT : Validated the exception successfully");
            }
        }

        /// <summary>
        /// Validate SAM Formatter Format(alignment, filename) by 
        /// parsing and formatting the dna sam file
        /// Input : alignment
        /// Output: sam file
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSAMFormatterWithTextWriter()
        {
            ValidateSAMFormatter(Constants.SmallSAMFileNode,
                ParseOrFormatTypes.ParseOrFormatText);
        }

        /// <summary>
        /// Validate parser and formatter by parsing the sam file with quality values
        /// Input : sam file with quality values
        /// Output: alignment contains qualitative sequences
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSAMParseAndFormatWithQualityValues()
        {
            ValidateSAMParseAndFormatWithQualityValues(
                Constants.SAMFileWithAllFieldsNode);
        }

        /// <summary>
        /// Validate parser and formatter by parsing the same file which contains 
        /// extended CIGAR string. Validate the CIGAR property in aligned sequence
        /// metadata information is updated as expected.
        /// Input : sam file with CIGAR format
        /// Output: alignment
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSAMParseAndFormatWithCIGAR()
        {
            ValidateSAMParseAndFormatWithCIGARFormat(
                Constants.SAMFileWithAllFieldsNode);
        }

        /// <summary>
        /// Validate properties in SAM Formatter class
        /// Input : Create a SAM Formatter object.
        /// Validation : Validate the properties
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSAMFormatterProperties()
        {
            SAMFormatter parser = new SAMFormatter();
            Assert.AreEqual(Constants.SAMFormatterDescription, parser.Description);
            Assert.AreEqual(Constants.SAMFileType, parser.SupportedFileTypes);
            Assert.AreEqual(Constants.SAMName, parser.Name);

            ApplicationLog.WriteLine("Successfully validated all the properties of SAM Parser class.");
        }

        /// <summary>
        /// Validate SAM Formatter Format(sequenceAlignmentMap, writer) by parsing and 
        /// formatting the dna sam file
        /// Input : alignment
        /// Output: sam file
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSAMFormatterSeqAlignMap()
        {
            ValidateSAMFormatterSeqAlign(Constants.SmallSAMFileNode,
                ParseOrFormatTypes.ParseOrFormatText);
        }

        /// <summary>
        /// Validate SAM Formatter FormatString(IsequenceAlignment) by parsing and 
        /// formatting the dna sam file
        /// Input : alignment
        /// Output: sam file
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSAMFormatterFormatString()
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.SamFormatterFileNode,
                Constants.FilePathNode);
            ISequenceAlignmentParser parser = new SAMParser();
            IList<ISequenceAlignment> alignment = parser.Parse(filePath).ToList();

            SAMFormatter formatter = new SAMFormatter();
            string writer = formatter.FormatString(alignment[0]);

            Assert.AreEqual(writer, Constants.FormatterString.Replace("\r\n", Environment.NewLine));
        }

        #endregion

        #endregion

        #region Helper Methods

        /// <summary>
        /// Validate parser parse method overloads with filePath\textreader
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="parseTypes">enum type to execute different overload</param>
        void ValidateSAMParser(string nodeName, ParseOrFormatTypes parseTypes)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedSequenceFile = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequence);
            ISequenceAlignmentParser parser = new SAMParser();
            IList<ISequenceAlignment> alignments = null;

            // Parse SAM File
            switch (parseTypes)
            {
                case ParseOrFormatTypes.ParseOrFormatText:
                    using (var reader = File.OpenRead(filePath))
                    {
                        alignments = parser.Parse(reader).ToList();
                    }
                    break;
                case ParseOrFormatTypes.ParseOrFormatFileName:
                    alignments = parser.Parse(filePath).ToList();
                    break;
            }

            // Get expected sequences
            FastAParser parserObj = new FastAParser();
            var expectedSequencesList = parserObj.Parse(expectedSequenceFile).ToList();

            // Validate parsed output with expected output
            int count = 0;
            for (int index = 0; index < alignments.Count; index++)
            {
                for (int ialigned = 0; ialigned <
                    alignments[index].AlignedSequences.Count; ialigned++)
                {
                    for (int iseq = 0; iseq <
                        alignments[index].AlignedSequences[ialigned].Sequences.Count; iseq++)
                    {
                        Assert.AreEqual(new string(expectedSequencesList[count].Select(a => (char)a).ToArray()),
                            new string(alignments[index].AlignedSequences[ialigned].Sequences[iseq].Select(a => (char)a).ToArray()));
                        count++;
                    }
                }
            }
        }

        /// <summary>
        /// Validate parser parse one method overloads with filePath\textreader
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="parseTypes">enum type to execute different overload</param>
        void ValidateSAMParserWithParseOne(string nodeName,
            ParseOrFormatTypes parseTypes)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedSequenceFile = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequence);
            ISequenceAlignmentParser parser = new SAMParser();
            ISequenceAlignment alignment = null;

            // Parse SAM File
            switch (parseTypes)
            {
                case ParseOrFormatTypes.ParseOrFormatText:
                    using (var reader = File.OpenRead(filePath))
                    {
                        alignment = parser.ParseOne(reader);
                    }
                    break;
                case ParseOrFormatTypes.ParseOrFormatFileName:
                    alignment = parser.ParseOne(filePath);
                    break;
            }

            // Get expected sequences
            FastAParser parserObj = new FastAParser();
            {
                IEnumerable<ISequence> expectedSequences = parserObj.Parse(expectedSequenceFile);
                IList<ISequence> expectedSequencesList = expectedSequences.ToList();
                // Validate parsed output with expected output
                int count = 0;
                foreach (IAlignedSequence alignedSequence in alignment.AlignedSequences)
                {
                    foreach (ISequence sequence in alignedSequence.Sequences)
                    {
                        Assert.AreEqual(expectedSequencesList[count].ConvertToString(),
                                            sequence.ConvertToString());
                        count++;
                    }
                }
            }
        }

        /// <summary>
        /// Validate formatter all format method overloads with filePath\textwriter
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="formatTypes">enum type to execute different overload</param>
        void ValidateSAMFormatter(string nodeName,
            ParseOrFormatTypes formatTypes)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedSequenceFile = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequence);
            ISequenceAlignmentParser parser = new SAMParser();
            try
            {
                IList<ISequenceAlignment> alignments = parser.Parse(filePath).ToList();
                SAMFormatter formatter = new SAMFormatter();
                switch (formatTypes)
                {
                    case ParseOrFormatTypes.ParseOrFormatText:
                        using (var writer = File.Create(Constants.SAMTempFileName))
                        {
                            formatter.Format(writer, alignments[0]);
                        }
                        break;
                    case ParseOrFormatTypes.ParseOrFormatFileName:
                        formatter.Format(alignments[0], Constants.SAMTempFileName);
                        break;
                }
                alignments = parser.Parse(Constants.SAMTempFileName).ToList();

                // Get expected sequences
                FastAParser parserObj = new FastAParser();
                {
                    IEnumerable<ISequence> expectedSequences = parserObj.Parse(expectedSequenceFile);
                    IList<ISequence> expectedSequencesList = expectedSequences.ToList();

                    // Validate parsed output with expected output
                    int count = 0;
                    for (int index = 0; index < alignments.Count; index++)
                    {
                        for (int ialigned = 0; ialigned <
                            alignments[index].AlignedSequences.Count; ialigned++)
                        {
                            for (int iseq = 0; iseq <
                                alignments[index].AlignedSequences[ialigned].Sequences.Count; iseq++)
                            {
                                Assert.AreEqual(new string(expectedSequencesList[count].Select(a => (char)a).ToArray()),
                                    new string(alignments[index].AlignedSequences[ialigned].Sequences[iseq].Select(a => (char)a).ToArray()));
                                count++;
                            }
                        }
                    }
                }
            }
            finally
            {
            }
        }

        /// <summary>
        /// Validate parser and formatter by parsing the sam file with quality values
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        void ValidateSAMParseAndFormatWithQualityValues(string nodeName)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedSequenceFile = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequence);
            // Create parser using encoding
            ISequenceAlignmentParser parser = new SAMParser();
            try
            {
                var alignments = parser.Parse(filePath).ToList();

                // Get expected sequences
                FastAParser parserObj = new FastAParser();
                {
                    var expectedSequencesList = parserObj.Parse(expectedSequenceFile).ToList();

                    // Validate parsed output with expected output
                    int count = 0;
                    for (int index = 0; index < alignments.Count; index++)
                    {
                        for (int ialigned = 0; ialigned <
                            alignments[index].AlignedSequences.Count; ialigned++)
                        {
                            for (int iseq = 0; iseq <
                                alignments[index].AlignedSequences[ialigned].Sequences.Count; iseq++)
                            {
                                Assert.IsInstanceOf<QualitativeSequence>(alignments[index].AlignedSequences[ialigned].Sequences[iseq]);
                                QualitativeSequence qualSequence =
                                 (QualitativeSequence)alignments[index].AlignedSequences[ialigned].Sequences[iseq];
                                Assert.AreEqual(
                                    new string(expectedSequencesList[count].Select(a => (char)a).ToArray()),
                                    new string(qualSequence.Select(a => (char)a).ToArray()));
                                count++;
                            }
                        }
                    }
                }
            }
            finally
            {
            }
        }

        /// <summary>
        /// Validate parser and formatter by parsing the same file which contains 
        /// extended CIGAR string. Validate the CIGAR property in aligned sequence
        /// metadata information is updated as expected.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        void ValidateSAMParseAndFormatWithCIGARFormat(string nodeName)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedSequenceFile = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequence);
            string expectedCIGARString = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CIGARNode);
            // Create parser using encoding
            ISequenceAlignmentParser parser = new SAMParser();
            try
            {
                var alignments = parser.Parse(filePath).ToList();

                // Get expected sequences
                FastAParser parserObj = new FastAParser();
                {
                    IEnumerable<ISequence> expectedSequences = parserObj.Parse(expectedSequenceFile);
                    IList<ISequence> expectedSequencesList = expectedSequences.ToList();

                    // Validate parsed output with expected output
                    int count = 0;
                    for (int index = 0; index < alignments.Count; index++)
                    {
                        for (int ialigned = 0; ialigned <
                            alignments[index].AlignedSequences.Count; ialigned++)
                        {
                            for (int iseq = 0; iseq <
                                alignments[index].AlignedSequences[ialigned].Sequences.Count; iseq++)
                            {
                                Assert.AreEqual(new string(expectedSequencesList[count].Select(a => (char)a).ToArray()),
                                    new string(alignments[index].AlignedSequences[ialigned].Sequences[iseq].Select(a => (char)a).ToArray()));
                                foreach (string key in alignments[index].AlignedSequences[ialigned].Metadata.Keys)
                                {
                                    SAMAlignedSequenceHeader header = (SAMAlignedSequenceHeader)
                                        alignments[index].AlignedSequences[ialigned].Metadata[key];
                                    Assert.AreEqual(expectedCIGARString, header.CIGAR);
                                }
                                count++;
                            }
                        }
                    }
                }
            }
            finally
            {
            }
        }

        /// <summary>
        /// General method to validate SAM parser method.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="parseTypes">enum type to execute different overload</param>
        void ValidateSAMParserSeqAlign(
            string nodeName,
            ParseOrFormatTypes method)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedSequenceFile = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequence);
            SAMParser parser = new SAMParser();
            {
                SequenceAlignmentMap alignments = null;

                // Parse SAM File
                switch (method)
                {
                    case ParseOrFormatTypes.ParseOrFormatText:
                        using (var reader = File.OpenRead(filePath))
                        {
                            alignments = parser.Parse(reader);
                        }
                        break;
                    case ParseOrFormatTypes.ParseOrFormatFileName:
                        alignments = (SequenceAlignmentMap) parser.ParseOne(filePath);
                        break;
                }

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
        void ValidateSAMFormatterSeqAlign(
            string nodeName,
            ParseOrFormatTypes parseTypes)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedSequenceFile = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequence);
            SAMParser parser = new SAMParser();
            {
                SequenceAlignmentMap alignments = parser.ParseOne<SequenceAlignmentMap>(filePath);
                SAMFormatter formatter = new SAMFormatter();
                switch (parseTypes)
                {
                    case ParseOrFormatTypes.ParseOrFormatText:
                        using (var writer =
                            File.Create(Constants.SAMTempFileName))
                        {
                            formatter.Format(writer, alignments);
                        }
                        break;
                    case ParseOrFormatTypes.ParseOrFormatFileName:
                        formatter.Format(alignments, Constants.SAMTempFileName);
                        break;
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
                            Assert.AreEqual(new string(expectedSequencesList[index].Select(a => (char)a).ToArray()),
                                new string(alignments.QuerySequences[index].Sequences[count].Select(a => (char)a).ToArray()));
                        }
                    }
                }
            }
        }
        
        #endregion
    }
}
