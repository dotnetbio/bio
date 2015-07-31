/****************************************************************************
 * FastQBvtTestCases.cs
 * 
 *This file contains FastQ Parsers and Formatters Bvt test cases.
 * 
***************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using Bio.Extensions;
using Bio.IO.FastQ;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using NUnit.Framework;

#if (SILVERLIGHT == false)
namespace Bio.TestAutomation.IO.FastQ
#else
namespace Bio.Silverlight.TestAutomation.IO.FastQ
#endif
{
    /// <summary>
    ///     FASTQ Bvt parser and formatter Test cases implementation.
    /// </summary>
    [TestFixture]
    public class FastQBvtTestCases
    {
        #region Enums

        /// <summary>
        ///     FastQ Formatter Parameters which are used for different test cases
        ///     based on which the test cases are executed.
        /// </summary>
        private enum FastQFileParameters
        {
            FastQ,
            Fq,
        };

        #endregion Enums

        #region Global Variables

        private readonly Utility utilityObj = new Utility(@"TestUtils\FastQTestsConfig.xml");

        #endregion Global Variables

        #region FastQ Parser & Formatter Bvt Test cases

        /// <summary>
        ///     Parse a valid small size FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : FastQ file with Illumina format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateFastQParserWithIlluminaUsingFastQFile()
        {
            ValidateFastQParser(Constants.SimpleIlluminaFastQNode);
        }

        /// <summary>
        ///     Parse a valid small size FastQ file and convert the same to
        ///     sequence using Parse(Stream) method and validate with the
        ///     expected sequence.
        ///     Input : FastQ file with Illumina format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateFastQParserWithIlluminaUsingFastQFileOnAStream()
        {
            ValidateFastQParserOnAStream(Constants.SimpleIlluminaFastQNode);
        }

        /// <summary>
        ///     Parse a valid small size FastQ file and convert the same to
        ///     sequence using Parse(Stream) method and validate with the
        ///     expected sequence.
        ///     Input : FastQ fq file extension with Solexa format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateFastQParserWithSolexaUsingFastQFqFileOnAStream()
        {
            ValidateFastQParserOnAStream(Constants.SimpleSolexaFqFastQNode);
        }

        /// <summary>
        ///     Parse a valid small size FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : FastQ fq file extension with Illumina format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateFastQParserWithIlluminaUsingFastQFqFile()
        {
            ValidateFastQParser(Constants.SimpleIlluminaFqFastQNode);
        }

        /// <summary>
        ///     Parse a valid small size FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : FastQ fq file extension with Solexa format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateFastQParserWithSolexaUsingFastQFqFile()
        {
            ValidateFastQParser(Constants.SimpleSolexaFqFastQNode);
        }

        /// <summary>
        ///     Parse a valid small size FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : FastQ file with Sanger format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateFastQParserWithSangerUsingFastQFile()
        {
            ValidateFastQParser(Constants.SimpleSangerFastQNode);
        }

        /// <summary>
        ///     Parse a valid small size FastQ file and convert the same to
        ///     sequence using Parse(file-name, isReadOnly) method and validate with the
        ///     expected sequence.
        ///     Input : FastQ fq file extension with Illumina format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateFastQParserReadOnlyWithIlluminaUsingFastQFqFile()
        {
            ValidateFastQParser(Constants.SimpleIlluminaFqFastQNode);
        }

        /// <summary>
        ///     Parse a valid small size FastQ file and convert the same to
        ///     sequence using Parse(file-name, isReadOnly) method and validate with the
        ///     expected sequence.
        ///     Input : FastQ fq file extension with Solexa format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateFastQParserReadOnlyWithSolexaUsingFastQFqFile()
        {
            ValidateFastQParser(Constants.SimpleSolexaFqFastQNode);
        }

        /// <summary>
        ///     Parse a valid small size FastQ file and convert the same to
        ///     sequence using Parse(file-name, isReadOnly) method and validate with the
        ///     expected sequence.
        ///     Input : FastQ file with Sanger format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateFastQParserReadOnlyWithSangerUsingFastQFile()
        {
            ValidateFastQParser(Constants.SimpleSangerFastQNode);
        }

        /// <summary>
        ///     Format a valid Single Sequence (Small size sequence less than 35 kb) to a
        ///     FastQ file Parse(reader, isReadOnly) method and validate the same.
        ///     Input : Solexa format FastQ Sequence
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateFastQParseForSolexa()
        {
            ValidateFastQParser(Constants.SimpleSolexaFqFastQNode);
        }

        /// <summary>
        ///     Format a valid Single Sequence (Small size sequence less than 35 kb) to a
        ///     FastQ file Parse(reader) method and validate the same.
        ///     Input : Solexa format FastQ Sequence
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateFastQParseReadOnlyForSolexa()
        {
            ValidateFastQParser(Constants.SimpleSolexaFqFastQNode);
        }

        /// <summary>
        ///     Format a valid Single Sequence (Small size sequence less than 35 kb) to a
        ///     FastQ file Parse(reader, isReadOnly) method and validate the same.
        ///     Input : Sanger format FastQ Sequence
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateFastQParseForSanger()
        {
            ValidateFastQParser(Constants.SimpleSangerFastQNode);
        }

        /// <summary>
        ///     Format a valid Single Sequence (Small size sequence less than 35 kb) to a
        ///     FastQ file Parse(reader) method and validate the same.
        ///     Input : Sanger format FastQ Sequence
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateFastQParseReadOnlyForSanger()
        {
            ValidateFastQParser(Constants.SimpleSangerFastQNode);
        }

        /// <summary>
        ///     Format a valid Single Sequence (Small size sequence less than 35 kb) to
        ///     FastQ file Parse(reader, isReadOnly) method and validate the same.
        ///     Input : Illumina format FastQ Sequence
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateFastQParseForIllumina()
        {
            ValidateFastQParser(Constants.SimpleIlluminaFqFastQNode);
        }

        /// <summary>
        ///     Format a valid Single Sequence (Small size sequence less than 35 kb) to a
        ///     FastQ file Parse(reader) method and validate the same.
        ///     Input : Illumina format FastQ Sequence
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateFastQParseReadOnlyForIllumina()
        {
            ValidateFastQParser(Constants.SimpleIlluminaFqFastQNode);
        }

        /// <summary>
        ///     Format a valid small size Sequence to FastQ file, Parse a temporary file and
        ///     convert the same to sequence using Parse(file-name) method and
        ///     validate with the expected sequence.
        ///     Input : FastQ file with Sanger format.
        ///     Output : Validation of formatting sequence to temporary FastQ file.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void FastQFormatterValidateFastQFileFormat()
        {
            ValidateFastQFormatter(Constants.SimpleSangerFastQNode,
                                   FastQFileParameters.FastQ);
        }

        /// <summary>
        ///     Format a valid small size Sequence to FastQ file, Parse a temporary file and
        ///     convert the same to sequence using Parse() method and
        ///     validate with the expected sequence.
        ///     Input : FastQ file with Sanger format.
        ///     Output : Validation of formatting sequence to temporary FastQ file.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void FastQFormatterValidateFastQFileFormatOnStream()
        {
            ValidateFastQFormatterOnAStream(Constants.SimpleSangerFastQNode);
        }


        /// <summary>
        ///     Format a valid small size to FastQ file with Fq extension, Parse a temporary
        ///     file and convert the same to sequence using ParseOne(file-name) method and
        ///     validate with the expected sequence.
        ///     Input : FastQ file with Sanger format.
        ///     Output : Validation of formatting sequence to temporary FastQ Fq file.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void FastQFormatterValidateFastQFqFileFormat()
        {
            ValidateFastQFormatter(Constants.SimpleSangerFastQNode,
                                   FastQFileParameters.Fq);
        }

        #endregion FastQ Parser & Formatter Bvt Test cases

        #region Supporting Methods

        /// <summary>
        ///     General method to validate FastQ Parser.
        ///     <param name="nodeName">xml node name.</param>
        /// </summary>
        private void ValidateFastQParser(string nodeName)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedQualitativeSequence = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequenceNode);
            string expectedSequenceId = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.SequenceIdNode);
            int expectedSeqCount = int.Parse(utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SeqsCount));

            // Parse a FastQ file.
            var fastQParserObj = new FastQParser();
            using (fastQParserObj.Open(filePath))
            {
                IList<IQualitativeSequence> qualSequenceList = fastQParserObj.Parse().ToList();

                // Validate qualitative Sequence upon parsing FastQ file.                                                
                Assert.AreEqual(expectedSeqCount, qualSequenceList.Count);
                Assert.AreEqual(expectedQualitativeSequence, qualSequenceList[0].ConvertToString());
                Assert.AreEqual(expectedSequenceId, qualSequenceList[0].ID);

                ApplicationLog.WriteLine(string.Format("FastQ Parser BVT: The FASTQ sequence '{0}' validation after Parse() is found to be as expected.",
                                                       qualSequenceList[0].ConvertToString()));
                ApplicationLog.WriteLine(string.Format("FastQ Parser BVT: The FASTQ sequence ID '{0}' validation after Parse() is found to be as expected.",
                                                       qualSequenceList[0].ID));
            }
        }

        /// <summary>
        ///     General method to validate FastQ Parser On Streams.
        ///     <param name="nodeName">xml node name.</param>
        /// </summary>
        private void ValidateFastQParserOnAStream(string nodeName)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedQualitativeSequence = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequenceNode);
            string expectedSequenceId = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.SequenceIdNode);
            IList<IQualitativeSequence> qualSequenceList;

            using (var reader = File.OpenRead(filePath))
            {
                // Parse a FastQ file.
                var fastQParserObj = new FastQParser();
                qualSequenceList = fastQParserObj.Parse(reader).ToList();
            }

            string actualQualitativeSequence = qualSequenceList[0].ConvertToString();
            string actualId = qualSequenceList[0].ID;

            // Validate qualitative Sequence upon parsing FastQ file.                                                                
            Assert.AreEqual(expectedQualitativeSequence, actualQualitativeSequence);
            Assert.AreEqual(actualId, expectedSequenceId);

            ApplicationLog.WriteLine(string.Format("FastQ Parser BVT: The FASTQ sequence '{0}' validation after Parse(Stream) is found to be as expected.",
                                                   actualQualitativeSequence));
            ApplicationLog.WriteLine(string.Format("FastQ Parser BVT: The FASTQ sequence ID '{0}' validation after Parse(Stream) is found to be as expected.",
                                                   actualId));
        }

        /// <summary>
        ///     General method to validate FastQ Formatter.
        ///     <param name="nodeName">xml node name.</param>
        ///     <param name="fileExtension">Different temporary file extensions</param>
        /// </summary>
        private void ValidateFastQFormatter(string nodeName, FastQFileParameters fileExtension)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string expectedQualitativeSequence = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode);
            string expectedSequenceId = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceIdNode);
            string tempFileName = Path.GetTempFileName();

            // Parse a FastQ file using parseOne method.
            var fastQParserObj = new FastQParser();
            using (fastQParserObj.Open(filePath))
            {
                IQualitativeSequence oneSequence = fastQParserObj.ParseOne();

                // Format Parsed Sequence to temp file with different extension.
                var fastQFormatter = new FastQFormatter();
                using (fastQFormatter.Open(tempFileName))
                {
                    fastQFormatter.Format(oneSequence);
                }

                string parsedValue;
                string parsedId;

                var fastQParserObjTemp = new FastQParser();
                using (fastQParserObjTemp.Open(tempFileName))
                {
                    oneSequence = fastQParserObjTemp.Parse().First();
                    parsedValue = oneSequence.ConvertToString();
                    parsedId = oneSequence.ID;
                }

                // Validate qualitative parsing temporary file.                
                Assert.AreEqual(expectedQualitativeSequence, parsedValue);
                Assert.AreEqual(expectedSequenceId, parsedId);
                ApplicationLog.WriteLine(string.Format("FastQ Formatter BVT: The FASTQ sequence '{0}' validation after Write() and Parse() is found to be as expected.",
                                                       parsedValue));
                ApplicationLog.WriteLine(string.Format("FastQ Formatter BVT: The FASTQ sequence '{0}' validation after Write() and Parse() is found to be as expected.",
                                                       parsedId));

                File.Delete(tempFileName);
            }
        }

        /// <summary>
        ///     General method to validate FastQ Formatter on a Stream.
        ///     <param name="nodeName">xml node name.</param>
        /// </summary>
        private void ValidateFastQFormatterOnAStream(string nodeName)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string expectedQualitativeSequence = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode);
            string expectedSequenceId = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceIdNode);
            string tempFileName1 = Path.GetTempFileName();

            // Parse a FastQ file using parseOne method.
            var fastQParserObj = new FastQParser();
            using (fastQParserObj.Open(filePath))
            {
                var oneSequence = fastQParserObj.ParseOne();

                // New Sequence after formatting file.                
                var fastQFormatter = new FastQFormatter();
                using (fastQFormatter.Open(tempFileName1))
                    fastQFormatter.Format(oneSequence);

                var fastQParserObjTemp = new FastQParser();
                string parsedValue, parsedId;
                using (fastQParserObjTemp.Open(tempFileName1))
                {
                    oneSequence = fastQParserObjTemp.Parse().First();
                    parsedValue = oneSequence.ConvertToString();
                    parsedId = oneSequence.ID;
                }

                // Validate qualitative parsing temporary file.                
                Assert.AreEqual(expectedQualitativeSequence, parsedValue);
                Assert.AreEqual(expectedSequenceId, parsedId);
                ApplicationLog.WriteLine(string.Format("FastQ Formatter BVT: The FASTQ sequence '{0}' validation after Write() and Parse() is found to be as expected.", parsedValue));
                ApplicationLog.WriteLine(string.Format("FastQ Formatter BVT: The FASTQ sequence '{0}' validation after Write() and Parse() is found to be as expected.", parsedId));

                File.Delete(tempFileName1);
            }
        }

        #endregion Supporting Methods
    }
}