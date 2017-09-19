/****************************************************************************
 * SAMP2TestCases.cs
 * 
 *   This file contains the Sam - Parsers and Formatters P2 test cases.
 * 
***************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Linq;

using Bio.Algorithms.Alignment;
using Bio.IO.SAM;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using NUnit.Framework;
using Bio.IO;
using System;
using Bio.Tests;

namespace Bio.TestAutomation.IO.SAM
{
    /// <summary>
    /// SAM P2 parser and formatter Test case implementation.
    /// </summary>
    [TestFixture]
    public class SAMP2TestCases
    {
        #region Enums

        /// <summary>
        /// Additional parameters to validate different scenarios.
        /// </summary>        
        enum ParseOrFormatTypes
        {
            ParseOrFormatText,
            ParseOrFormatFileName,
            ParseOneOrFormatText,
            ParseOneOrFormatFileName,
            ParseOneOrFormatHeader,
            ParseOneOrFormatHeaderFn,
            ParseOneOrFormatSeq,
            ParseOrFormatFormatString,
            ParseOrFormatCollection,
            ParseOrFormatIseq,
            ParseOrFormatIseqFile,
            ParseOrFormatIseqT,
            ParseOrFormatIseqText,
            ParseOrFormatSeqText,
            ParseOrFormatCollString,
            ParseOneOrFormatSeqFile,
            ParseOrFormatSeqTextWithFlag
        }

        /// <summary>
        /// Additional parameters to validate different Quality.
        /// </summary>
        enum ParseOrFormatQualLength
        {
            AlignedSeq,
            Sequencedata,
            Qualitydata,
            QualityLength
        }

        #endregion Enums

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\SAMBAMTestData\SAMBAMTestsConfig.xml");

        #endregion Global Variables

        #region SAM Parser TestCases

        /// <summary>
        /// Invalidate SAM ISequenceAlignmentParser Parse(textreader) by parsing
        /// Invalid value.
        /// Input : Invalid value
        /// Output: Expected Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateSAMISeqAlignParserReader()
        {
            ValidateISeqAlignParser(ParseOrFormatTypes.ParseOrFormatText);
        }

        /// <summary>
        /// Invalidate SAM ISequenceAlignmentParser Parse(file-name) by parsing
        /// null file-name.
        /// Input : Invalid value
        /// Output: Expected Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateSAMISeqAlignParserFileName()
        {
            ValidateISeqAlignParser(ParseOrFormatTypes.ParseOrFormatFileName);
        }

        /// <summary>
        /// Invalidate SAM Parser(textReader) by parsing Invalid value.
        /// Input : Invalid value
        /// Output: Expected Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateSAMParserReader()
        {
            ValidateSAMParser(ParseOrFormatTypes.ParseOrFormatText);
        }

        /// <summary>
        /// Invalidate SAM Parser(file-name) by parsing file-name
        /// as null.
        /// Input : Null file-name
        /// Output: Expected Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateSAMParserfileName()
        {
            ValidateSAMParser(ParseOrFormatTypes.ParseOrFormatFileName);
        }

        /// <summary>
        /// Invalidate SAM ParserOne(textReader) by parsing Invalid value.
        /// Input : Invalid value
        /// Output: Expected Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateSAMParseOneReader()
        {
            ValidateSAMParser(ParseOrFormatTypes.ParseOneOrFormatText);
        }

        /// <summary>
        /// Invalidate SAM Parser(file-name) by parsing file-name
        /// as null.
        /// Input : Null file-name
        /// Output: Expected Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateSAMParseOnefileName()
        {
            ValidateSAMParser(ParseOrFormatTypes.ParseOneOrFormatFileName);
        }

        /// <summary>
        /// Invalidate SAM ParseQualityNSequence() by parsing invalid
        /// align sequence, sequence data and quality data
        /// Input : Null file-name
        /// Output: Expected Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateSAMParseQualiNSeq()
        {
            ValidateQualitySeqLength(ParseOrFormatQualLength.AlignedSeq);
            ValidateQualitySeqLength(ParseOrFormatQualLength.Sequencedata);
            ValidateQualitySeqLength(ParseOrFormatQualLength.Qualitydata);
        }

        /// <summary>
        /// Invalidate SAM ParseQualityNSequence() by parsing invalid
        /// quality score length
        /// Input : Null file-name
        /// Output: Expected Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateSAMParseQualiNSeqLength()
        {
            ValidateQualitySeqLength(ParseOrFormatQualLength.QualityLength);
        }

        /// <summary>
        /// Invalidate ParserSAMHeader(file-name) by parsing invalid
        /// file-name
        /// Input : Null file-name
        /// Output: Expected Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateSAMParseHeaderFileName()
        {
            ValidateISeqAlignParser(ParseOrFormatTypes.ParseOneOrFormatHeaderFn);
        }

        /// <summary>
        /// Invalidate ParserSAMHeader(textReader) by parsing invalid
        /// textReader
        /// Input : Null file-name
        /// Output: Expected Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateSAMParseHeaderTextReader()
        {
            ValidateISeqAlignParser(ParseOrFormatTypes.ParseOneOrFormatHeader);
        }

        /// <summary>
        /// Invalidate ParserSAMHeader(BioReader) by parsing invalid
        /// BioReader
        /// Input : Null file-name
        /// Output: Expected Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateSAMParseHeaderBioReader()
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.InvalidSamBioReaderNode,
                Constants.FilePathNode).TestDir();

            try
            {
                using (TextReader reader = new StreamReader(filePath))
                {
                    SAMParser.ParseSAMHeader(reader);
                }

                Assert.Fail();
            }
            catch (FormatException)
            {
                ApplicationLog.WriteLine(
                    "SAM Parser P2 : Successfully validated the exception");
            }
        }

        /// <summary>
        /// Invalidate Alphabet property
        /// Input : Alphabet
        /// Output: Expected Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateSAMParseAlhabetProp()
        {
            try
            {
                SAMParser sparserObj = new SAMParser();
                {
                    sparserObj.Alphabet = Alphabets.DNA;
                }
                Assert.Fail();
            }
            catch (NotSupportedException)
            {
                ApplicationLog.WriteLine(
                    "SAM Parser P2 : Successfully validated the exception");
            }
        }

        #endregion

        #region SAM Formatter TestCases

        /// <summary>
        /// Invalidate SAM Formatter WriteHeader(header, writer) by parsing
        /// invalid writer.
        /// Input : Invalid value
        /// Output: Expected Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateSAMWriteTextWriter()
        {
            SAMAlignmentHeader header = new SAMAlignmentHeader();

            try
            {
                SAMFormatter.WriteHeader(null, header);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                ApplicationLog.WriteLine(
                    "SAM Formatter P2 : Successfully validated the exception");
            }
        }

        /// <summary>
        /// Invalidate SAM Format(SequenceAlignment, writer) by parsing
        /// null ISequenceAlignment or null writer
        /// Input : Null ISequenceAlignment
        /// Output: Expected Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateSAMFormatWriter()
        {
            ValidateSamFormatter(ParseOrFormatTypes.ParseOrFormatIseqT);
            ValidateSamFormatter(ParseOrFormatTypes.ParseOrFormatIseqText);
        }

        /// <summary>
        /// Invalidate SAM Format(ISequenceAlignment, file-name) by parsing
        /// null sequene alignment and file name.
        /// Input : Invalid value
        /// Output: Expected Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateSAMFormatSeq()
        {
            ValidateSamFormatter(ParseOrFormatTypes.ParseOrFormatIseq);
            ValidateSamFormatter(ParseOrFormatTypes.ParseOrFormatIseqFile);
        }

        /// <summary>
        /// Invalidate SAM Format(SequenceAlignment, file-name) by parsing
        /// null file-name.
        /// Input : Invalid value
        /// Output: Expected Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateSAMFormatSeqFileName()
        {
            ValidateSamFormatter(ParseOrFormatTypes.ParseOneOrFormatSeq);
            ValidateSamFormatter(ParseOrFormatTypes.ParseOneOrFormatSeqFile);
        }

        /// <summary>
        /// Invalidate SAM Format(ISequenceAlignment, writer) by parsing
        /// null ISequenceAlignment and writer.
        /// Input : Invalid value
        /// Output: Expected Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateSAMFormatISeqWriter()
        {
            ValidateSamFormatter(ParseOrFormatTypes.ParseOrFormatSeqText);
            ValidateSamFormatter(ParseOrFormatTypes.ParseOrFormatSeqTextWithFlag);
        }

        /// <summary>
        /// Invalidate SAM Format(Collect_Iseq, file-name) by parsing
        /// null ISequenceAlignment and file-name.
        /// Input : Invalid value
        /// Output: Expected Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateSAMFormatCollSeqFileName()
        {
            ValidateSamFormatter(ParseOrFormatTypes.ParseOrFormatCollString);
        }

        /// <summary>
        /// Invalidate SAM Format(Collect_Iseq, writer) by parsing
        /// null ISequenceAlignment and writer.
        /// Input : Invalid value
        /// Output: Expected Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateSAMFormatCollSeqWriter()
        {
            ValidateSamFormatter(ParseOrFormatTypes.ParseOrFormatCollection);
        }

        /// <summary>
        /// Invalidate SAM FormatString(SequenceAlignment) by parsing
        /// null SequenceAlignment.
        /// Input : Null SequenceAlignment
        /// Output: Expected Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateSAMFormatString()
        {
            ValidateSamFormatter(ParseOrFormatTypes.ParseOrFormatFormatString);
        }

        #endregion SAM Formatter TestCases

        #region Helper Method

        /// <summary>
        /// Genaral method to Invalidate ISequence Alignment
        /// <param name="method">enum type to execute different overload</param>
        /// </summary>
        private static void ValidateISeqAlignParser(ParseOrFormatTypes method)
        {
            ISequenceAlignmentParser parser = new SAMParser();
            try
            {
                switch (method)
                {
                    case ParseOrFormatTypes.ParseOrFormatText:
                        parser.Parse(null).First();
                        break;
                    case ParseOrFormatTypes.ParseOrFormatFileName:
                        parser.Parse(null as string).First();
                        break;
                    case ParseOrFormatTypes.ParseOneOrFormatHeader:
                        SAMParser.ParseSAMHeader(null as TextReader);
                        break;
                    case ParseOrFormatTypes.ParseOneOrFormatHeaderFn:
                        SAMParser.ParseSAMHeader(null as Stream);
                        break;
                    default:
                        break;
                }

                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                ApplicationLog.WriteLine(
                    "SAM Parser P2 : Successfully validated the exception");
            }
        }

        /// <summary>
        /// Genaral method to Invalidate SAM Parser
        /// <param name="method">enum type to execute different overload</param>
        /// </summary>
        private static void ValidateSAMParser(ParseOrFormatTypes method)
        {
            try
            {
                switch (method)
                {
                    case ParseOrFormatTypes.ParseOrFormatText:
                        new SAMParser().Parse(null);
                        break;
                    case ParseOrFormatTypes.ParseOrFormatFileName:
                        new SAMParser().Parse(null as string).First();
                        break;
                    case ParseOrFormatTypes.ParseOneOrFormatText:
                        new SAMParser().ParseOne(null);
                        break;
                    case ParseOrFormatTypes.ParseOneOrFormatFileName:
                        new SAMParser().ParseOne(null as string);
                        break;
                    default:
                        break;
                }

                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                ApplicationLog.WriteLine(
                    "SAM Parser P2 : Successfully validated the exception");
            }
        }

        /// <summary>
        /// General method to Invalidate Quality Sequences
        /// <param name="method">enum type to execute different overload</param>
        /// </summary>
        private static void ValidateQualitySeqLength(ParseOrFormatQualLength method)
        {
            SAMAlignedSequence align = new SAMAlignedSequence();

            try
            {
                switch (method)
                {
                    case ParseOrFormatQualLength.AlignedSeq:
                        SAMParser.ParseQualityNSequence(
                            align,
                            Alphabets.DNA,
                            null,
                            String.Empty);
                        break;
                    case ParseOrFormatQualLength.Sequencedata:
                        align.QName = "Quality Value";
                        SAMParser.ParseQualityNSequence(
                            align,
                            Alphabets.DNA,
                            null,
                            String.Empty);
                        break;
                    case ParseOrFormatQualLength.Qualitydata:
                        align.QName = "Quality Value";
                        SAMParser.ParseQualityNSequence(
                            align,
                            Alphabets.DNA,
                            null,
                            Constants.QualitySequence);
                        break;
                    case ParseOrFormatQualLength.QualityLength:
                        align.QName = "Quality Value";
                        SAMParser.ParseQualityNSequence(
                            align,
                            Alphabets.DNA,
                            null,
                            Constants.QualitySequence);
                        break;
                    default:
                        break;
                }

                Assert.Fail();
            }
            catch (ArgumentException)
            {
                ApplicationLog.WriteLine(
                    "SAM Parser P2 : Successfully validated the exception");
            }
            catch (FormatException)
            {
                ApplicationLog.WriteLine(
                    "SAM Parser P2 : Successfully validated the exception");
            }
        }

        /// <summary>
        /// Genaral method to Invalidate SAM Formatter
        /// <param name="method">enum type to execute different overload</param>
        /// </summary>
        void ValidateSamFormatter(ParseOrFormatTypes method)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.SmallSAMFileNode, Constants.FilePathNode).TestDir();
            ISequenceAlignmentParser parser = new SAMParser();
            ISequenceAlignment alignment = null;

            try
            {
                switch (method)
                {
                    case ParseOrFormatTypes.ParseOrFormatSeqText:
                        new SAMFormatter().Format(null, null as ISequenceAlignment);
                        break;
                    case ParseOrFormatTypes.ParseOrFormatSeqTextWithFlag:
                        alignment = parser.ParseOne(filePath);
                        new SAMFormatter().Format(null, alignment);
                        break;
                    case ParseOrFormatTypes.ParseOrFormatIseq:
                        new SAMFormatter().Format(
                            null as ISequenceAlignment,
                            null as string);
                        break;
                    case ParseOrFormatTypes.ParseOrFormatIseqFile:
                        alignment = parser.ParseOne(filePath);
                        new SAMFormatter().Format(
                            alignment,
                            null as string);
                        break;
                    case ParseOrFormatTypes.ParseOrFormatCollString:
                        new SAMFormatter().Format(
                            null as ICollection<ISequenceAlignment>,
                            null as string);
                        break;
                    case ParseOrFormatTypes.ParseOrFormatCollection:
                        new SAMFormatter().Format(null,
                            null as ICollection<ISequenceAlignment>);
                        break;
                    case ParseOrFormatTypes.ParseOneOrFormatSeq:
                        SequenceAlignmentMap align = new SequenceAlignmentMap();
                        new SAMFormatter().Format(align, null as string);
                        break;
                    case ParseOrFormatTypes.ParseOneOrFormatSeqFile:
                        new SAMFormatter().Format(null as SequenceAlignmentMap, null as string);
                        break;
                    case ParseOrFormatTypes.ParseOrFormatIseqT:
                        SequenceAlignmentMap alignments =
                            new SequenceAlignmentMap();
                        new SAMFormatter().Format(alignments, null as string);
                        break;
                    case ParseOrFormatTypes.ParseOrFormatIseqText:
                        new SAMFormatter().Format(null as SequenceAlignmentMap, null as string);
                        break;
                    case ParseOrFormatTypes.ParseOrFormatFormatString:
                        break;
                    default:
                        break;
                }
            }
            catch (ArgumentNullException)
            {
                ApplicationLog.WriteLine(
                    "SAM Formatter P2 : Successfully validated the exception");
            }
            catch (NotSupportedException)
            {
                ApplicationLog.WriteLine(
                    "SAM Formatter P2 : Successfully validated the exception");
            }
        }

        #endregion Helper Method
    }
}
