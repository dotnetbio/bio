/****************************************************************************
 * FastQP2TestCases.cs
 * 
 *This file contains FastQ Parsers and Formatters P2 test cases.
 * 
***************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Bio.IO.FastQ;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if (SILVERLIGHT == false)
namespace Bio.TestAutomation.IO.FastQ
#else
namespace Bio.Silverlight.TestAutomation.IO.FastQ
#endif
{
    /// <summary>
    ///     FASTQ parser and formatter P2 Test cases implementation.
    /// </summary>
    [TestClass]
    public class FastQP2TestCases
    {
        #region Enums

        /// <summary>
        ///     FastQ Formatter Parameters which are used for different test cases
        ///     based on which the test cases are executed.
        /// </summary>
        private enum FastQFormatParameters
        {
            Sequence,
            Default
        };

        #endregion Enums

        #region Global Variables

        private readonly Utility utilityObj = new Utility(@"TestUtils\FastQTestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        ///     Static constructor to open log and make other settings needed for test
        /// </summary>
        static FastQP2TestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region FastQ Parser P2 Test cases

        /// <summary>
        ///     Invalidate FastQ Parser with invalid FastQ file.
        ///     Input : Qualitative sequence without @ at first line
        ///     Output : Validate Exception.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        [ExpectedException(typeof(FileFormatException))]
        public void InvalidateFastQParserWithInvalidSeqId()
        {
            InValidateFastQParser(Constants.FastQSequenceWithInvalidSeqIdNode);
        }

        /// <summary>
        ///     Invalidate FastQ Parser with empty sequence.
        ///     Input : FastQ empty sequence.
        ///     Output : Validate Exception.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        [ExpectedException(typeof(FileFormatException))]
        public void InvalidateFastQParserWithEmptySequence()
        {
            InValidateFastQParser(Constants.FastQParserEmptySequenceNode);
        }

        /// <summary>
        ///     Invalidate FastQ Parser with invalid Qual scores.
        ///     Input : FastQ file with invalid qual score.
        ///     Output : Validate Exception.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        [ExpectedException(typeof(FileFormatException))]
        public void InvalidateFastQParserWithInvalidQualScore()
        {
            InValidateFastQParser(Constants.FastQParserWithInvalidQualScoreNode);
        }

        /// <summary>
        ///     Invalidate FastQ Parser with empty Qual scores.
        ///     Input : FastQ file with empty qual score.
        ///     Output : Validate Exception.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        [ExpectedException(typeof(FileFormatException))]
        public void InvalidateFastQParserWithEmptyQualScore()
        {
            InValidateFastQParser(Constants.FastQParserWithEmptyQualScoreNode);
        }

        /// <summary>
        ///     Invalidate FastQ Parser with empty Qual scores and Empty Qual Id.
        ///     Input : FastQ file with empty qual score and Id.
        ///     Output : Validate Exception.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        [ExpectedException(typeof(FileFormatException))]
        public void InvalidateFastQParserWithEmptyQualScoreAndQualId()
        {
            InValidateFastQParser(Constants.FastQParserWithEmptyQualScoreAndQualID);
        }

        /// <summary>
        ///     Invalidate FastQ Parser with invalid alphabet.
        ///     Input : Invalid alphabet.
        ///     Output : Validate Exception.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        [ExpectedException(typeof(FileFormatException))]
        public void InvalidateFastQParserWithInvalidAlphabet()
        {
            InValidateFastQParser(Constants.FastQParserWithInvalidAlphabet);
        }

      

        /// <summary>
        ///     Invalidate fastq formatter with Sequence as null value.
        ///     Input : Invalid sequence.
        ///     Output : Validate Exception.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateFastQFormatterWithInvalidSequence()
        {
            InValidateFastQFormatter(FastQFormatParameters.Sequence);
        }


        /// <summary>
        ///     Invalidate fastq formatter with Qual Sequence and TextWriter null value.
        ///     Input : Invalid Qualitative sequence.
        ///     Output : Validate Exception.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateFastQFormatterWithInvalidQualSequenceAndTextWriter()
        {
            InValidateFastQFormatter(FastQFormatParameters.Default);
        }

        /// <summary>
        ///     Invalidate Parse(file-name, isReadOnly) with null as file-name
        ///     Input : Invalid file-name.
        ///     Output : Validate Exception.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InvalidateFastQParseNoFileName()
        {
            using (var fqParserObj = new FastQParser(null))
            {
                fqParserObj.Parse();
            }
        }

        #endregion FastQ Parser P2 Test cases

        #region Helper Methods

        /// <summary>
        ///     General method to Invalidate FastQ Parser.
        ///     <param name="nodeName">xml node name.</param>
        /// </summary>
        private void InValidateFastQParser(string nodeName)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);

            // Create a FastQ Parser object.
            using (var fastQParserObj = new FastQParser(filePath))
            {
                fastQParserObj.Parse()
                    .ToList();
            }
        }

        /// <summary>
        ///     General method to Invalidate FastQ Parser.
        ///     <param name="param">FastQ Formatter different parameters</param>
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void InValidateFastQFormatter(FastQFormatParameters param)
        {
            // Gets the expected sequence from the Xml
            string filepath = utilityObj.xmlUtil.GetTextValue(
                Constants.MultiSeqSangerRnaProNode, Constants.FilePathNode);

            // Parse a FastQ file.
            using (var fastQParser = new FastQParser(filepath))
            {
                IEnumerable<QualitativeSequence> sequence = null;
                FastQFormatter fastQFormatter = null;

                switch (param)
                {
                    case FastQFormatParameters.Sequence:
                        try
                        {
                            fastQFormatter = new FastQFormatter(filepath);
                            fastQFormatter.Write(null as ISequence);
                            Assert.Fail();
                        }
                        catch (ArgumentNullException)
                        {
                            fastQFormatter.Close();
                            ApplicationLog.WriteLine(
                                "FastQ Parser P2 : Successfully validated the exception");
                        }
                        break;
                    default:
                        try
                        {
                            sequence = fastQParser.Parse();
                            fastQFormatter = new FastQFormatter(Constants.FastQTempFileName);
                            fastQFormatter.Write(sequence as QualitativeSequence);
                            Assert.Fail();
                        }
                        catch (ArgumentNullException)
                        {
                            fastQFormatter.Close();
                            ApplicationLog.WriteLine(
                                "FastQ Parser P2 : Successfully validated the exception");
                        }
                        break;
                }
            }
        }

        #endregion Helper Methods
    }
}