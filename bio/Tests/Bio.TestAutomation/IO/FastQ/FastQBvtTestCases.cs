/****************************************************************************
 * FastQBvtTestCases.cs
 * 
 *This file contains FastQ Parsers and Formatters Bvt test cases.
 * 
***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Bio.TestAutomation.Util;
using Bio;
using Bio.IO;
using Bio.IO.FastQ;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if (SILVERLIGHT == false)
    namespace Bio.TestAutomation.IO.FastQ
#else
    namespace Bio.Silverlight.TestAutomation.IO.FastQ
#endif
{
    /// <summary>
    /// FASTQ Bvt parser and formatter Test cases implementation.
    /// </summary>
    [TestClass]
    public class FastQBvtTestCases
    {
        #region Enums

        /// <summary>
        /// FastQ Formatter Parameters which are used for different test cases 
        /// based on which the test cases are executed.
        /// </summary>
        enum FastQFileParameters
        {
            FastQ,
            Fq,
            TextReader,
            TextReaderReadOnly,
            FileName,
            FileNameReadOnly
        };

        #endregion Enums

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\FastQTestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static FastQBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region FastQ Parser & Formatter Bvt Test cases

        /// <summary>
        /// Parse a valid small size FastQ file and convert the same to 
        /// sequence using Parse(file-name) method and validate with the 
        /// expected sequence.
        /// Input : FastQ file with Illumina format.
        /// Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFastQParserWithIlluminaUsingFastQFile()
        {
            ValidateFastQParser(Constants.SimpleIlluminaFastQNode);
        }        

        /// <summary>
        /// Parse a valid small size FastQ file and convert the same to 
        /// sequence using Parse(Stream) method and validate with the 
        /// expected sequence.
        /// Input : FastQ file with Illumina format.
        /// Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFastQParserWithIlluminaUsingFastQFileOnAStream()
        {
            ValidateFastQParserOnAStream(Constants.SimpleIlluminaFastQNode);
        }

        /// <summary>
        /// Parse a valid small size FastQ file and convert the same to 
        /// sequence using Parse(Stream) method and validate with the 
        /// expected sequence.
        /// Input : FastQ fq file extension with Solexa format.
        /// Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFastQParserWithSolexaUsingFastQFqFileOnAStream()
        {
            ValidateFastQParserOnAStream(Constants.SimpleSolexaFqFastQNode);
        }
        
        /// <summary>
        /// Parse a valid small size FastQ file and convert the same to 
        /// sequence using Parse(file-name) method and validate with the 
        /// expected sequence.
        /// Input : FastQ fq file extension with Illumina format.
        /// Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFastQParserWithIlluminaUsingFastQFqFile()
        {
            ValidateFastQParser(Constants.SimpleIlluminaFqFastQNode);
        }

        /// <summary>
        /// Parse a valid small size FastQ file and convert the same to 
        /// sequence using Parse(file-name) method and validate with the 
        /// expected sequence.
        /// Input : FastQ fq file extension with Solexa format.
        /// Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFastQParserWithSolexaUsingFastQFqFile()
        {
            ValidateFastQParser(Constants.SimpleSolexaFqFastQNode);
        }

        /// <summary>
        /// Parse a valid small size FastQ file and convert the same to 
        /// sequence using Parse(file-name) method and validate with the 
        /// expected sequence.
        /// Input : FastQ file with Sanger format.
        /// Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFastQParserWithSangerUsingFastQFile()
        {
            ValidateFastQParser(Constants.SimpleSangerFastQNode);
        }

        /// <summary>
        /// Parse a valid small size FastQ file and convert the same to 
        /// sequence using Parse(file-name, isReadOnly) method and validate with the 
        /// expected sequence.
        /// Input : FastQ fq file extension with Illumina format.
        /// Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFastQParserReadOnlyWithIlluminaUsingFastQFqFile()
        {
            ValidateFastQParser(Constants.SimpleIlluminaFqFastQNode);

        }

        /// <summary>
        /// Parse a valid small size FastQ file and convert the same to 
        /// sequence using Parse(file-name, isReadOnly) method and validate with the 
        /// expected sequence.
        /// Input : FastQ fq file extension with Solexa format.
        /// Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFastQParserReadOnlyWithSolexaUsingFastQFqFile()
        {
            ValidateFastQParser(Constants.SimpleSolexaFqFastQNode);
        }

        /// <summary>
        /// Parse a valid small size FastQ file and convert the same to 
        /// sequence using Parse(file-name, isReadOnly) method and validate with the 
        /// expected sequence.
        /// Input : FastQ file with Sanger format.
        /// Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFastQParserReadOnlyWithSangerUsingFastQFile()
        {
            ValidateFastQParser(Constants.SimpleSangerFastQNode);
        }

        /// <summary>
        /// Format a valid Single Sequence (Small size sequence less than 35 kb) to a 
        /// FastQ file Parse(reader, isReadOnly) method and validate the same.
        /// Input : Solexa format FastQ Sequence
        /// Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFastQParseForSolexa()
        {
            ValidateFastQParser(Constants.SimpleSolexaFqFastQNode);
        }

        /// <summary>
        /// Format a valid Single Sequence (Small size sequence less than 35 kb) to a 
        /// FastQ file Parse(reader) method and validate the same.
        /// Input : Solexa format FastQ Sequence
        /// Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFastQParseReadOnlyForSolexa()
        {
            ValidateFastQParser(Constants.SimpleSolexaFqFastQNode);
        }

        /// <summary>
        /// Format a valid Single Sequence (Small size sequence less than 35 kb) to a 
        /// FastQ file Parse(reader, isReadOnly) method and validate the same.
        /// Input : Sanger format FastQ Sequence
        /// Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFastQParseForSanger()
        {
            ValidateFastQParser(Constants.SimpleSangerFastQNode);
        }

        /// <summary>
        /// Format a valid Single Sequence (Small size sequence less than 35 kb) to a 
        /// FastQ file Parse(reader) method and validate the same.
        /// Input : Sanger format FastQ Sequence
        /// Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFastQParseReadOnlyForSanger()
        {
            ValidateFastQParser(Constants.SimpleSangerFastQNode);
        }

        /// <summary>
        /// Format a valid Single Sequence (Small size sequence less than 35 kb) to
        /// FastQ file Parse(reader, isReadOnly) method and validate the same.
        /// Input : Illumina format FastQ Sequence
        /// Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFastQParseForIllumina()
        {
            ValidateFastQParser(Constants.SimpleIlluminaFqFastQNode);
        }

        /// <summary>
        /// Format a valid Single Sequence (Small size sequence less than 35 kb) to a 
        /// FastQ file Parse(reader) method and validate the same.
        /// Input : Illumina format FastQ Sequence
        /// Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFastQParseReadOnlyForIllumina()
        {
            ValidateFastQParser(Constants.SimpleIlluminaFqFastQNode);
        }

        /// <summary>
        /// Format a valid small size Sequence to FastQ file, Parse a temporary file and 
        /// convert the same to sequence using Parse(file-name) method and 
        /// validate with the expected sequence.
        /// Input : FastQ file with Sanger format.
        /// Output : Validation of formatting sequence to temporary FastQ file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void FastQFormatterValidateFastQFileFormat()
        {
            ValidateFastQFormatter(Constants.SimpleSangerFastQNode,
                FastQFileParameters.FastQ);
        }

        /// <summary>
        /// Format a valid small size Sequence to FastQ file, Parse a temporary file and 
        /// convert the same to sequence using Parse() method and 
        /// validate with the expected sequence.
        /// Input : FastQ file with Sanger format.
        /// Output : Validation of formatting sequence to temporary FastQ file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void FastQFormatterValidateFastQFileFormatOnStream()
        {
            ValidateFastQFormatterOnAStream(Constants.SimpleSangerFastQNode);                
        }
        
        
        /// <summary>
        /// Format a valid small size to FastQ file with Fq extension, Parse a temporary 
        /// file and convert the same to sequence using ParseOne(file-name) method and 
        /// validate with the expected sequence.
        /// Input : FastQ file with Sanger format.
        /// Output : Validation of formatting sequence to temporary FastQ Fq file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void FastQFormatterValidateFastQFqFileFormat()
        {
            ValidateFastQFormatter(Constants.SimpleSangerFastQNode,
                FastQFileParameters.Fq);
        }

        #endregion FastQ Parser & Formatter Bvt Test cases

        #region Supporting Methods

        /// <summary>
        /// General method to validate FastQ Parser.
        /// <param name="nodeName">xml node name.</param>
        /// </summary>
        void ValidateFastQParser(string nodeName)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedQualitativeSequence = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequenceNode);
            string expectedSequenceId = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.SequenceIdNode);
            string expectedSeqCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.SeqsCount);

            // Parse a FastQ file.
            using (FastQParser fastQParserObj = new FastQParser(filePath))
            {
                fastQParserObj.AutoDetectFastQFormat = false;
                IEnumerable<QualitativeSequence> qualSequenceList = null;
                qualSequenceList = fastQParserObj.Parse();

                // Validate qualitative Sequence upon parsing FastQ file.                                                
                Assert.AreEqual(qualSequenceList.Count().ToString((IFormatProvider)null), expectedSeqCount);
                Assert.AreEqual(new String(qualSequenceList.ElementAt(0).Select(a => (char)a).ToArray()), expectedQualitativeSequence);
                Assert.AreEqual(qualSequenceList.ElementAt(0).ID.ToString((IFormatProvider)null), expectedSequenceId);

                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "FastQ Parser BVT: The FASTQ sequence '{0}' validation after Parse() is found to be as expected.",
                    qualSequenceList.ElementAt(0).Select(a => (char)a).ToArray()));

                // Logs to the VSTest GUI (Console.Out) window
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "FastQ Parser BVT: The FASTQ sequence ID '{0}' validation after Parse() is found to be as expected.",
                    qualSequenceList.ElementAt(0).Select(a => (char)a).ToArray()));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "FastQ Parser BVT: The FASTQ sequence ID '{0}' validation after Parse() is found to be as expected.",
                    qualSequenceList.ElementAt(0).ID.ToString((IFormatProvider)null)));
            }
        }

        /// <summary>
        /// General method to validate FastQ Parser On Streams.
        /// <param name="nodeName">xml node name.</param>
        /// </summary>
        void ValidateFastQParserOnAStream(string nodeName)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedQualitativeSequence = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequenceNode);
            string expectedSequenceId = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.SequenceIdNode);
            string actualQualitativeSequence = String.Empty;
            string actualId=String.Empty;
            IEnumerable<QualitativeSequence> qualSequence=null;
            IList<QualitativeSequence> qualSequenceList=null;

            using(StreamReader reader = new StreamReader(filePath))
            {
                // Parse a FastQ file.
                using (FastQParser fastQParserObj = new FastQParser())
                {
                    fastQParserObj.AutoDetectFastQFormat = false;
                    qualSequence = fastQParserObj.Parse(reader);                
                    qualSequenceList = qualSequence.ToList();                
                }
            }

            actualQualitativeSequence=new string(qualSequenceList[0].Select(a => (char)a).ToArray());
            actualId=qualSequenceList[0].ID.ToString((IFormatProvider)null);

            // Validate qualitative Sequence upon parsing FastQ file.                                                                
            Assert.AreEqual(expectedQualitativeSequence,actualQualitativeSequence);
            Assert.AreEqual(actualId,expectedSequenceId);

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "FastQ Parser BVT: The FASTQ sequence '{0}' validation after Parse(Stream) is found to be as expected.",
                actualQualitativeSequence));

            // Logs to the VSTest GUI (Console.Out) window
            Console.WriteLine(string.Format((IFormatProvider)null,
                "FastQ Parser BVT: The FASTQ sequence ID '{0}' validation after Parse(Stream) is found to be as expected.",
                actualQualitativeSequence));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "FastQ Parser BVT: The FASTQ sequence ID '{0}' validation after Parse(Stream) is found to be as expected.",
                actualId));
            }          

        /// <summary>
        /// General method to validate FastQ Formatter.
        /// <param name="nodeName">xml node name.</param>
        /// <param name="fileExtension">Different temporary file extensions</param>
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        void ValidateFastQFormatter(string nodeName,
            FastQFileParameters fileExtension)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedQualitativeSequence = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequenceNode);
            string expectedSequenceId = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.SequenceIdNode);
            string tempFileName = System.IO.Path.GetTempFileName();            

            // Parse a FastQ file using parseOne method.
            using (FastQParser fastQParserObj = new FastQParser(filePath))
            {
                fastQParserObj.AutoDetectFastQFormat = false;
                IEnumerable<QualitativeSequence> qualSequence = null;
                qualSequence = fastQParserObj.Parse();

                // New Sequence after formatting file.                
                IEnumerable<QualitativeSequence> newQualSeq = null;                
                string parsedValue = null;
                string parsedID = null;

                // Format Parsed Sequence to temp file with different extension.
                switch (fileExtension)
                {
                    case FastQFileParameters.FastQ:
                        using (FastQFormatter fastQFormatter = new FastQFormatter(tempFileName))
                        {
                            fastQFormatter.Write(qualSequence.ElementAt(0));                         
                        }
                        using (FastQParser fastQParserObjTemp = new FastQParser(tempFileName))
                        {
                            newQualSeq = fastQParserObjTemp.Parse();
                            parsedValue = new string(newQualSeq.ElementAt(0).Select(a => (char)a).ToArray());
                            parsedID = newQualSeq.ElementAt(0).ID.ToString((IFormatProvider)null);
                        }
                        
                        break;
                    case FastQFileParameters.Fq:
                        using (FastQFormatter fastQFormatterFq = new FastQFormatter(tempFileName))
                        {
                            fastQFormatterFq.Write(qualSequence.ElementAt(0));                            
                        }
                        using (FastQParser fastQParserObjTemp1 = new FastQParser(tempFileName))
                        {
                            newQualSeq = fastQParserObjTemp1.Parse();
                            parsedValue = new string(newQualSeq.ElementAt(0).Select(a => (char)a).ToArray());
                            parsedID = newQualSeq.ElementAt(0).ID.ToString((IFormatProvider)null);
                        }
                        break;
                    default:
                        break;
                }

                // Validate qualitative parsing temporary file.                
                Assert.AreEqual(parsedValue, expectedQualitativeSequence);
                Assert.AreEqual(parsedID, expectedSequenceId);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "FastQ Formatter BVT: The FASTQ sequence '{0}' validation after Write() and Parse() is found to be as expected.",
                    parsedValue));

                // Logs to the VSTest GUI (Console.Out) window
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "FastQ Formatter BVT: The FASTQ sequence '{0}' validation after Write() and Parse() is found to be as expected.",
                    parsedValue));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "FastQ Formatter BVT: The FASTQ sequence '{0}' validation after Write() and Parse() is found to be as expected.",
                    parsedID));

                qualSequence = null;                                
                File.Delete(tempFileName);                
            }
        }

        /// <summary>
        /// General method to validate FastQ Formatter on a Stream.
        /// <param name="nodeName">xml node name.</param>        
        /// </summary>
        void ValidateFastQFormatterOnAStream(string nodeName)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedQualitativeSequence = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequenceNode);
            string expectedSequenceId = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.SequenceIdNode);
            string tempFileName1 = System.IO.Path.GetTempFileName();
            string parsedValue = string.Empty;
            string parsedID = string.Empty;            
            IEnumerable<QualitativeSequence> qualSequence = null;

            // Parse a FastQ file using parseOne method.
            using (FastQParser fastQParserObj = new FastQParser(filePath))
            {
                fastQParserObj.AutoDetectFastQFormat = false;
                qualSequence = fastQParserObj.Parse();

                // New Sequence after formatting file.                
                IEnumerable<QualitativeSequence> newQualSeq = null;
                using (StreamWriter writer = new StreamWriter(tempFileName1))
                {
                    using (FastQFormatter fastQFormatter = new FastQFormatter())
                    {
                        fastQFormatter.Open(writer);
                        fastQFormatter.Write(qualSequence.ElementAt(0));
                    }
                }

                using (FastQParser fastQParserObjTemp = new FastQParser(tempFileName1))
                {
                    newQualSeq = fastQParserObjTemp.Parse();
                    parsedValue = new string(newQualSeq.ElementAt(0).Select(a => (char)a).ToArray());
                    parsedID = newQualSeq.ElementAt(0).ID.ToString((IFormatProvider)null);
                }                                                                           

                // Validate qualitative parsing temporary file.                
                Assert.AreEqual(parsedValue, expectedQualitativeSequence);
                Assert.AreEqual(parsedID, expectedSequenceId);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "FastQ Formatter BVT: The FASTQ sequence '{0}' validation after Write() and Parse() is found to be as expected.",
                    parsedValue));

                // Logs to the VSTest GUI (Console.Out) window
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "FastQ Formatter BVT: The FASTQ sequence '{0}' validation after Write() and Parse() is found to be as expected.",
                    parsedValue));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "FastQ Formatter BVT: The FASTQ sequence '{0}' validation after Write() and Parse() is found to be as expected.",
                    parsedID));

                qualSequence = null;                
                File.Delete(tempFileName1);                
            }
        }

        #endregion Supporting Methods
    }
}
