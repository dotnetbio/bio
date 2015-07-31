/****************************************************************************
 * BAMP2TestCases.cs
 * 
 *   This file contains the BAM - Parsers and Formatters P2 test cases.
 * 
***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Bio.Algorithms.Alignment;
using Bio.IO;
using Bio.IO.BAM;
using Bio.IO.SAM;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using NUnit.Framework;

namespace Bio.TestAutomation.IO.BAM
{
    /// <summary>
    ///     BAM parser and formatter P2 Test case implementation.
    /// </summary>
    [TestFixture]
    public class BAMP2TestCases
    {
        #region Enums

        /// <summary>
        ///     BAM Parser ctor parameters used for different test cases.
        /// </summary>
        private enum BAMParserParameters
        {
            StreamReader,
            FileName,
            ParseRangeFileName,
            ParseRangeWithIndex,
            Stream,
            ParseOneStream,
        }

        #endregion Enums

        #region Global Variables

        private readonly Utility _utilityObj = new Utility(@"TestUtils\SAMBAMTestData\SAMBAMTestsConfig.xml");

        #endregion Global Variables

        #region BAM Parser P2 Testcases

        /// <summary>
        ///     Invalidate BAM Parser Parse(TextReader)
        ///     Input : BAM file.
        ///     Output : NotSupportedException.
        /// </summary>
        [Test]
        [Ignore("Not sure why ignored")]
        [Category("Priority1"), Category("BAM")]
        public void InValidateSequenceAlignmentParseTextReader()
        {
            InValidateISequenceAlignmentBAMParser(Constants.SmallSizeBAMFileNode,
                                                  BAMParserParameters.Stream);
        }

        /// <summary>
        ///     Invalidate BAM Parser Parse(stream)
        ///     Input : Invlaid Stream.
        ///     Output : Argument NullException.
        /// </summary>
        [Test]
        [Category("Priority1"), Category("BAM")]
        public void InvalidateBAMParserForInvalidStreamInput()
        {
            InValidateSeqAlignmentMapBAMParser(Constants.InvalidBAMFileNode,
                                               BAMParserParameters.StreamReader);
        }

        /// <summary>
        ///     Invalidate BAM Parser Parse(filename)
        ///     Input : Invlaid filename.
        ///     Output : Argument NullException.
        /// </summary>
        [Test]
        [Category("Priority1"), Category("BAM")]
        public void InvalidateBAMParserForInvalidBAMFile()
        {
            InValidateSeqAlignmentMapBAMParser(Constants.SmallSizeBAMFileNode,
                                               BAMParserParameters.FileName);
        }

        /// <summary>
        ///     Invalidate GetIndexFromBAMFile(fileName) by passing
        ///     null as BAM file.
        ///     Input : Null BAM file.
        ///     Output : Exception.
        /// </summary>
        [Test]
        [Category("Priority1"), Category("BAM")]
        public void InvalidateGetIndexFromBAMFile()
        {
            // Create BAM Parser object
            using (var bamParserObj = new BAMParser())
            {
                try
                {
                    bamParserObj.GetIndexFromBAMStorage(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException ex)
                {
                    string exceptionMessage = ex.Message;
                    ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully",
                                                           exceptionMessage));
                }
            }
        }

        /// <summary>
        ///     Invalidate GetIndexFromBAMFile(stream) by passing
        ///     null as BAM file.
        ///     Input : Null BAM file.
        ///     Output : Exception.
        /// </summary>
        [Test]
        [Category("Priority1"), Category("BAM")]
        public void InvalidateGetIndexFromBAMFileUsingStream()
        {
            // Create BAM Parser object
            using (var bamParserObj = new BAMParser())
            {
                try
                {
                    bamParserObj.GetIndexFromBAMStorage(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException ex)
                {
                    string exceptionMessage = ex.Message;
                    ApplicationLog.WriteLine(string.Format(null,
                                                           "BAM Parser P2 : Validated Exception {0} successfully",
                                                           exceptionMessage));
                }
            }
        }

        /// <summary>
        ///     Invalidate BAM Parser ParseOne(TextReader)
        ///     Input : BAM file.
        ///     Output : NotSupportedException.
        /// </summary>
        [Test]
        [Ignore("Not sure why")]
        [Category("Priority1"), Category("BAM")]
        public void InValidateSequenceAlignmentParseOneTextReader()
        {
            InValidateISequenceAlignmentBAMParser(Constants.SmallSizeBAMFileNode,
                                                  BAMParserParameters.ParseOneStream);
        }

        /// <summary>
        ///     Invalidate BAM Parser ParseRange(filename, RefIndex)
        ///     Input : Invalid BAM file and RefIndex values.
        ///     Output : NotSupportedException.
        /// </summary>
        [Test]
        [Category("Priority1"), Category("BAM")]
        public void InvalidateParseRangeForInvalidInputs()
        {
            InValidateSeqAlignmentMapBAMParser(Constants.SmallSizeBAMFileNode,
                                               BAMParserParameters.ParseRangeWithIndex);
        }

        /// <summary>
        ///     Invalidate BAM Parser ParseRange(filename,range)
        ///     Input : Invalid BAM file and SequenceRange values.
        ///     Output : NotSupportedException.
        /// </summary>
        [Test]
        [Category("Priority1"), Category("BAM")]
        public void InvalidateParseRangeForInvalidSequenceRange()
        {
            InValidateSeqAlignmentMapBAMParser(Constants.SmallSizeBAMFileNode,
                                               BAMParserParameters.ParseRangeFileName);
        }

        /// <summary>
        ///     Invalidate Set Alphabet.
        ///     Input : Null BAM file.
        ///     Output : Exception.
        /// </summary>
        [Test]
        [Category("Priority1"), Category("BAM")]
        public void InvalidateSetAlphabet()
        {
            // Create BAM Parser object
            using (var bamParserObj = new BAMParser())
            {
                // TO cover code coverage.
                try
                {
                    bamParserObj.Alphabet = Alphabets.DNA;
                    Assert.Fail();
                }
                catch (NotSupportedException ex)
                {
                    string exceptionMessage = ex.Message;
                    ApplicationLog.WriteLine(string.Format(null,
                                                           "BAM Parser P2 : Validated Exception {0} successfully",
                                                           exceptionMessage));
                }
            }
        }

        #endregion BAM Parser P2 Testcases

        # region BAM Formatter P2 Testcases

        /// <summary>
        ///     InValidate BAM Formatter Format() methods with invalid inputs.
        ///     Input : Invalid inputs
        ///     Output : Exception validation.
        /// </summary>
        [Test]
        [Category("Priority1"), Category("BAM")]
        public void InvalidateBAMFormatMethods()
        {
            InValidateBAMFormatter(Constants.SmallSizeBAMFileNode);
        }

        /// <summary>
        ///     InValidate BAM Formatter Format() methods with invalid inputs
        ///     For ISequenceAlignment.
        ///     Input : Invalid inputs
        ///     Output : Exception validation.
        /// </summary>
        [Test]
        [Category("Priority1"), Category("BAM")]
        public void InvalidateBAMFormatMethodsWithISequenceAlignment()
        {
            InValidateBAMFormatterWithSequenceAlignment(Constants.SmallSizeBAMFileNode);
        }

        /// <summary>
        ///     Invalidate the WriteHeader method in BAMFormatter
        /// </summary>
        [Test]
        [Category("Priority1"), Category("BAM")]
        public void InvalidateBAMFormatterWriteHeader()
        {
            //pass null value for stream
            try
            {
                var formatter = new BAMFormatter();
                formatter.WriteHeader(new SAMAlignmentHeader(), null);
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.Write("BAM P2 : ArgumentNullException successfully thrown : " + ex.Message);
            }

            //pass null for SAMAlignmentHeader
            try
            {
                var formatter = new BAMFormatter();
                string tmpFileName = Path.GetTempFileName();
                using (var stream = new FileStream(tmpFileName, FileMode.Create))
                {
                    formatter.WriteHeader(null, stream);
                    Assert.Fail();
                    File.Delete(tmpFileName);
                }
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.Write("BAM P2 : ArgumentNullException successfully thrown : " + ex.Message);
            }
        }

        /// <summary>
        ///     Invalidate WriteAlignedSequence method
        /// </summary>
        [Test]
        [Category("Priority1"), Category("BAM")]
        public void InvalidateWriteAlignedSequence()
        {
            //pass nul for SAMAlignmentHeader
            try
            {
                string tmpFileName = Path.GetTempFileName();
                var formatter = new BAMFormatter();
                using (var stream = new FileStream(tmpFileName, FileMode.Create))
                {
                    formatter.WriteAlignedSequence(null, new SAMAlignedSequence(), stream);
                    Assert.Fail();
                    File.Delete(tmpFileName);
                }
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.Write("BAM P2 : ArgumentNullException successfully thrown : " + ex.Message);
            }

            //pass null for SAMAlignedSequence
            try
            {
                string tmpFileName = Path.GetTempFileName();
                var formatter = new BAMFormatter();
                using (var stream = new FileStream(tmpFileName, FileMode.Create))
                {
                    formatter.WriteAlignedSequence(new SAMAlignmentHeader(), null, stream);
                    Assert.Fail();
                    File.Delete(tmpFileName);
                }
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.Write("BAM P2 : ArgumentNullException successfully thrown : " + ex.Message);
            }

            //pass null for stream
            try
            {
                var formatter = new BAMFormatter();
                formatter.WriteAlignedSequence(new SAMAlignmentHeader(), new SAMAlignedSequence(), null);
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.Write("BAM P2 : ArgumentNullException successfully thrown : " + ex.Message);
            }
        }

        /// <summary>
        ///     Invalidate compressBAMFile method with null stream
        /// </summary>
        [Test]
        [Category("Priority1"), Category("BAM")]
        public void InvalidateCompressBAMFile()
        {
            //pass null for stream
            try
            {
                string tmpFileName = Path.GetTempFileName();
                var formatter = new BAMFormatter();
                using (var stream = new FileStream(tmpFileName, FileMode.Create))
                {
                    formatter.CompressBAMFile(null, stream);
                    Assert.Fail();
                    File.Delete(tmpFileName);
                }
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.Write("BAM P2 : ArgumentNullException successfully thrown : " + ex.Message);
            }

            //pass null for stream
            try
            {
                string tmpFileName = Path.GetTempFileName();
                var formatter = new BAMFormatter();
                using (var stream = new FileStream(tmpFileName, FileMode.Create))
                {
                    formatter.CompressBAMFile(stream, null);
                    Assert.Fail();
                    File.Delete(tmpFileName);
                }
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.Write("BAM P2 : ArgumentNullException successfully thrown : " + ex.Message);
            }
        }

        # endregion BAM Formatter P2 Testcases

        #region BAMIndexStorage TestCases

        /// <summary>
        ///     invalidate BAMIndexStorage constructure with null source stream
        /// </summary>
        [Test]
        [Category("Priority1"), Category("BAM")]
        public void InvalidateBAMIndexStorage()
        {
            //set source stream as null            
            try
            {
                var biFile = new BAMIndexStorage(null);
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.Write("BAM P2 : Successfully caught ArgumentNullException : " + ex.Message);
            }

            try
            {
                string temp = null;
                using (var biFile = new BAMIndexStorage(File.Create(temp)))
                {
                    Assert.Fail();
                }
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.Write("BAM P2 : Successfully caught ArgumentNullException : " + ex.Message);
            }
        }

        #endregion BAMIndexStorage TestCases

        # region Supporting Methods

        /// <summary>
        ///     Parse BAM File and Invalidate parsed aligned sequences by creating
        ///     ISequenceAlignment interface object and its properties.
        /// </summary>
        /// <param name="nodeName">Different xml nodes used for different test cases</param>
        /// <param name="BAMParserPam">BAM Parse method parameters</param>
        private void InValidateISequenceAlignmentBAMParser(string nodeName,
                                                           BAMParserParameters BAMParserPam)
        {
            // Get input and output values from xml node.
            string bamFilePath = _utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                  Constants.FilePathNode);
            string exception = string.Empty;

            ISequenceAlignmentParser bamParser = null;
            bamParser = new BAMParser();

            try
            {
                // Parse a BAM file with different invalid parameters.
                switch (BAMParserPam)
                {
                    case BAMParserParameters.Stream:
                        try
                        {
                            using (var reader = File.OpenRead(bamFilePath))
                            {
                                bamParser.Parse(reader);
                                Assert.Fail();
                            }
                        }
                        catch (NotSupportedException ex)
                        {
                            exception = ex.Message;
                        }
                        break;
                    case BAMParserParameters.ParseOneStream:
                        try
                        {
                            using (var reader = File.OpenRead(bamFilePath))
                            {
                                bamParser.ParseOne(reader);
                                Assert.Fail();
                            }
                        }
                        catch (NotSupportedException ex)
                        {
                            exception = ex.Message;
                        }
                        break;
                    default:
                        break;
                }

                // Log to VSTest GUI.
                ApplicationLog.WriteLine(string.Format(null,
                                                       "BAM Parser P2 : Validated Exception {0} successfully",
                                                       exception));
            }
            finally
            {
                (bamParser as BAMParser).Dispose();
            }
        }

        /// <summary>
        ///     Parse BAM and validate parsed aligned sequences by creating
        ///     ISequenceAlignment interface object and its properties.
        /// </summary>
        /// <param name="nodeName">Different xml nodes used for different test cases</param>
        /// <param name="BAMParserPam">BAM Parse method parameters</param>
        private void InValidateSeqAlignmentMapBAMParser(string nodeName,
                                                        BAMParserParameters BAMParserPam)
        {
            // Get input and output values from xml node.
            string bamFilePath = _utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string exception = string.Empty;

            using (var bamParser = new BAMParser())
            {
                // Parse a BAM file with different parameters.
                switch (BAMParserPam)
                {
                    case BAMParserParameters.StreamReader:
                        try
                        {
                            bamParser.Parse(null as Stream);
                            Assert.Fail();
                        }
                        catch (ArgumentNullException ex)
                        {
                            exception = ex.Message;
                        }

                        try
                        {
                            using (Stream stream = new FileStream(bamFilePath, FileMode.Open, FileAccess.Read))
                            {
                                bamParser.Parse(stream).ToList();
                                Assert.Fail();
                            }
                        }
                        catch (Exception ex)
                        {
                            exception = ex.Message;
                        }
                        break;
                    case BAMParserParameters.FileName:
                        try
                        {
                            bamParser.Parse(null as string).First();
                            Assert.Fail();
                        }
                        catch (ArgumentNullException ex)
                        {
                            exception = ex.Message;
                        }
                        break;
                    case BAMParserParameters.ParseRangeWithIndex:
                        try
                        {
                            bamParser.ParseRange(null, 0);
                            Assert.Fail();
                        }
                        catch (ArgumentNullException ex)
                        {
                            exception = ex.Message;
                        }

                        try
                        {
                            bamParser.ParseRange(bamFilePath, -2);
                            Assert.Fail();
                        }
                        catch (ArgumentOutOfRangeException ex)
                        {
                            exception = ex.Message;
                        }
                        break;
                    case BAMParserParameters.ParseRangeFileName:
                        try
                        {
                            bamParser.ParseRange(null, new SequenceRange("chr20", 0, 10));
                            Assert.Fail();
                        }
                        catch (ArgumentNullException ex)
                        {
                            exception = ex.Message;
                        }

                        try
                        {
                            bamParser.ParseRange(bamFilePath, null as SequenceRange);
                            Assert.Fail();
                        }
                        catch (ArgumentNullException ex)
                        {
                            exception = ex.Message;
                        }
                        break;
                    default:
                        break;
                }

                // Log to VSTest GUI.
                ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully", exception));
            }
        }

        /// <summary>
        ///     Format BAM file and validate.
        /// </summary>
        /// <param name="nodeName">Different xml nodes used for different test cases</param>
        private void InValidateBAMFormatter(string nodeName)
        {
            // Get input and output values from xml node.
            string bamFilePath = _utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);

            using (var bamParserObj = new BAMParser())
            {
                using (var storage = new BAMIndexStorage(File.Create(Constants.BAMTempIndexFileForIndexData)))
                {
                    // Parse a BAM file.
                    var seqAlignment = bamParserObj.ParseOne <SequenceAlignmentMap>(bamFilePath);

                    // Create a BAM formatter object.
                    var formatterObj = new BAMFormatter();

                    // Null filename
                    try
                    {
                        formatterObj.Format(seqAlignment, null, Constants.BAMTempIndexFileForIndexData);
                    }
                    catch (ArgumentNullException ex)
                    {
                        ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully", ex.Message));
                    }

                    // Same filename
                    try
                    {
                        formatterObj.Format(seqAlignment, Constants.BAMTempFileName, Constants.BAMTempFileName);
                    }
                    catch (ArgumentException ex)
                    {
                        ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully", ex.Message));
                    }

                    // Null sequence
                    try
                    {
                        formatterObj.Format(null, bamFilePath, Constants.BAMTempIndexFileForIndexData);
                    }
                    catch (ArgumentNullException ex)
                    {
                        ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully", ex.Message));
                    }

                    // Null index
                    try
                    {
                        formatterObj.Format(seqAlignment, bamFilePath, null);
                    }
                    catch (ArgumentNullException ex)
                    {
                        ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully", ex.Message));
                    }

                    // Invalidate BAM Parser Format(SeqAlignmentMap, BamFileName)
                    try
                    {
                        formatterObj.Format(null as SequenceAlignmentMap,
                                            Constants.BAMTempFileName);
                    }
                    catch (ArgumentNullException ex)
                    {
                        ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully", ex.Message));
                    }

                    try
                    {
                        formatterObj.Format(seqAlignment, null as string);
                    }
                    catch (ArgumentNullException ex)
                    {
                        ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully", ex.Message));
                    }

                    // Invalidate Format(SequenceAlignmentMap, StreamWriter)
                    try
                    {
                        formatterObj.Format(null, seqAlignment);
                    }
                    catch (ArgumentNullException ex)
                    {
                        ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully", ex.Message));
                    }

                    Stream stream;
                    try
                    {
                        using (stream = new FileStream(Constants.BAMTempFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            formatterObj.Format(stream, null as ISequenceAlignment);
                        }
                    }
                    catch (ArgumentNullException ex)
                    {
                        ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully", ex.Message));
                    }

                    // Invalidate Format(SequenceAlignmentMap, StreamWriter, IndexFile)
                    try
                    {
                        formatterObj.Format(null, storage, seqAlignment);
                    }
                    catch (ArgumentNullException ex)
                    {
                        ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully", ex.Message));
                    }

                    try
                    {
                        using (stream = new FileStream(Constants.BAMTempFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            formatterObj.Format(stream, storage, null);
                        }
                    }
                    catch (ArgumentNullException ex)
                    {
                        ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully", ex.Message));
                    }

                    try
                    {
                        using (stream = new FileStream(Constants.BAMTempFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            formatterObj.Format(stream, null, seqAlignment);
                        }
                    }

                    catch (ArgumentNullException ex)
                    {
                        ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully", ex.Message));
                    }

                    formatterObj = null;
                }
            }
        }

        /// <summary>
        ///     Format BAM file using IsequenceAlignment object.
        /// </summary>
        /// <param name="nodeName">Different xml nodes used for different test cases</param>
        private void InValidateBAMFormatterWithSequenceAlignment(string nodeName)
        {
            // Get input and output values from xml node.
            string bamFilePath = _utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);

            ISequenceAlignmentParser bamParserObj = new BAMParser();
            IList<ISequenceAlignment> seqList = bamParserObj.Parse(bamFilePath).ToList();
            try
            {
                using (var BAMIndexStorageObj = new BAMIndexStorage(File.Create(Constants.BAMTempIndexFileForInvalidData)))
                {
                    // Create a BAM formatter object.
                    var formatterObj = new BAMFormatter();
                    InvalidateBAmFormatter(formatterObj, seqList);
                    InvalidateBAmFormatterWithWithInvalidValues(formatterObj,seqList, bamFilePath, BAMIndexStorageObj);
                    InvalidateBAmFormatterWithWithNullValues(formatterObj,seqList);
                }
            }
            finally
            {
                (bamParserObj as BAMParser).Dispose();
                File.Delete(Constants.BAMTempIndexFileForInvalidData);
            }
        }


        /// <summary>
        ///     Invalidate BAMFormatter.
        /// </summary>
        /// <param name="formatterObj">Bam formatter obj</param>
        /// <param name="seqList">List of sequences</param>
        private static void InvalidateBAmFormatter(BAMFormatter formatterObj,
                                                   IList<ISequenceAlignment> seqList)
        {
            // Invalidate BAM Parser Format(SeqAlignment, BamFileName)
            try
            {
                formatterObj.Format(null as ISequenceAlignment,
                                    Constants.BAMTempFileName);
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully", ex.Message));
            }

            try
            {
                foreach (ISequenceAlignment seq in seqList)
                {
                    formatterObj.Format(seq, null as string);
                }
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully", ex.Message));
            }

            // Invalidate Format(IseqAlignment, BAMFile, IndexFile)
            try
            {
                foreach (ISequenceAlignment seq in seqList)
                {
                    formatterObj.Format(seq, null,
                                        Constants.BAMTempIndexFileForIndexData);
                }
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully", ex.Message));
            }

            try
            {
                foreach (ISequenceAlignment seq in seqList)
                {
                    formatterObj.Format(seq, Constants.BAMTempFileName,
                                        Constants.BAMTempFileName);
                }
            }
            catch (ArgumentException ex)
            {
                ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully", ex.Message));
            }
        }

        /// <summary>
        ///     Invalidate BAMFormatter with invalid values.
        /// </summary>
        /// <param name="formatterObj">Bam formatter obj</param>
        /// <param name="seqList">List of sequences</param>
        private static void InvalidateBAmFormatterWithWithInvalidValues(BAMFormatter formatterObj,
                                                                        IList<ISequenceAlignment> seqList,
                                                                        string bamFilePath, BAMIndexStorage BAMIndexStorageObj)
        {
            try
            {
                formatterObj.Format(null as ISequenceAlignment, bamFilePath,
                                    Constants.BAMTempIndexFileForIndexData);
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully",
                                                       ex.Message));
            }

            try
            {
                foreach (ISequenceAlignment seq in seqList)
                {
                    formatterObj.Format(seq, bamFilePath, null);
                }
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully",
                                                       ex.Message));
            }

            // Invalidate Format(IseqAlignment, StreamWriter, IndexFile)
            try
            {
                foreach (ISequenceAlignment seq in seqList)
                {
                    formatterObj.Format(null, BAMIndexStorageObj, seq);
                }
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully",
                                                       ex.Message));
            }

            try
            {
                using (Stream stream = new
                    FileStream(Constants.BAMTempFileName,
                               FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    formatterObj.Format(stream, BAMIndexStorageObj, null as ISequenceAlignment);
                }
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully",
                                                       ex.Message));
            }
        }

        /// <summary>
        ///     Invalidate BAMFormatter with null values.
        /// </summary>
        /// <param name="formatterObj">Bam formatter obj</param>
        /// <param name="seqList">List of sequences</param>
        private static void InvalidateBAmFormatterWithWithNullValues(BAMFormatter formatterObj,
                                                                     IList<ISequenceAlignment> seqList)
        {
            try
            {
                using (Stream stream = new
                    FileStream(Constants.BAMTempFileName,
                               FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    foreach (ISequenceAlignment seq in seqList)
                    {
                        formatterObj.Format(stream, null, seq);
                    }
                }
            }

            catch (ArgumentNullException ex)
            {
                ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully",
                                                       ex.Message));
            }

            // Invalidate Format(IseqAlignment, StreamWriter)
            try
            {
                foreach (ISequenceAlignment seq in seqList)
                {
                    formatterObj.Format(null as Stream, seq);
                }
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully",
                                                       ex.Message));
            }

            try
            {
                using (Stream stream = new
                    FileStream(Constants.BAMTempFileName,
                               FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    formatterObj.Format(stream, null as ISequenceAlignment);
                }
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.WriteLine(string.Format("BAM Parser P2 : Validated Exception {0} successfully",
                                                       ex.Message));
            }
        }

        #endregion Supporting Methods
    }
}