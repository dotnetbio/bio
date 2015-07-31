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
using NUnit.Framework;

#if (SILVERLIGHT == false)
namespace Bio.TestAutomation.IO.FastQ
#else
namespace Bio.Silverlight.TestAutomation.IO.FastQ
#endif
{
    /// <summary>
    ///     FASTQ parser and formatter P2 Test cases implementation.
    /// </summary>
    [TestFixture]
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

        #region FastQ Parser P2 Test cases

        /// <summary>
        ///     Invalidate FastQ Parser with invalid FastQ file.
        ///     Input : Qualitative sequence without @ at first line
        ///     Output : Validate Exception.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateFastQParserWithInvalidSeqId()
        {
           Assert.Throws<Exception> ( () =>  InValidateFastQParser(Constants.FastQSequenceWithInvalidSeqIdNode));
        }

        /// <summary>
        ///     Invalidate FastQ Parser with empty sequence.
        ///     Input : FastQ empty sequence.
        ///     Output : Validate Exception.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateFastQParserWithEmptySequence()
        {
            Assert.Throws<Exception>(() => InValidateFastQParser(Constants.FastQParserEmptySequenceNode));
        }

        /// <summary>
        ///     Invalidate FastQ Parser with invalid Qual scores.
        ///     Input : FastQ file with invalid qual score.
        ///     Output : Validate Exception.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateFastQParserWithInvalidQualScore()
        {
            Assert.Throws<Exception>(() => InValidateFastQParser(Constants.FastQParserWithInvalidQualScoreNode));
        }

        /// <summary>
        ///     Invalidate FastQ Parser with empty Qual scores.
        ///     Input : FastQ file with empty qual score.
        ///     Output : Validate Exception.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateFastQParserWithEmptyQualScore()
        {
            Assert.Throws<Exception> ( ()=> InValidateFastQParser(Constants.FastQParserWithEmptyQualScoreNode));
        }

        /// <summary>
        ///     Invalidate FastQ Parser with empty Qual scores and Empty Qual Id.
        ///     Input : FastQ file with empty qual score and Id.
        ///     Output : Validate Exception.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateFastQParserWithEmptyQualScoreAndQualId()
        {
            Assert.Throws<Exception>(() => InValidateFastQParser(Constants.FastQParserWithEmptyQualScoreAndQualID));
        }

        /// <summary>
        ///     Invalidate FastQ Parser with invalid alphabet.
        ///     Input : Invalid alphabet.
        ///     Output : Validate Exception.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateFastQParserWithInvalidAlphabet()
        {
            Assert.Throws<Exception>(() => InValidateFastQParser(Constants.FastQParserWithInvalidAlphabet));
        }

      

        /// <summary>
        ///     Invalidate fastq formatter with Sequence as null value.
        ///     Input : Invalid sequence.
        ///     Output : Validate Exception.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateFastQFormatterWithInvalidSequence()
        {
            InValidateFastQFormatter(FastQFormatParameters.Sequence);
        }


        /// <summary>
        ///     Invalidate fastq formatter with Qual Sequence and TextWriter null value.
        ///     Input : Invalid Qualitative sequence.
        ///     Output : Validate Exception.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateFastQFormatterWithInvalidQualSequenceAndTextWriter()
        {
            InValidateFastQFormatter(FastQFormatParameters.Default);
        }

        /// <summary>
        ///     Invalidate Parse(file-name, isReadOnly) with null as file-name
        ///     Input : Invalid file-name.
        ///     Output : Validate Exception.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateFastQParseNoFileName()
        {
            var fqParserObj = new FastQParser();
            Assert.Throws<ArgumentNullException>( () => fqParserObj.Parse(null).ToList());
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
            var fastQParserObj = new FastQParser();
            {
                fastQParserObj.Parse(filePath).ToList();
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
            var fastQParser = new FastQParser();
            using (fastQParser.Open(filepath))
            {
                FastQFormatter fastQFormatter = null;

                switch (param)
                {
                    case FastQFormatParameters.Sequence:
                        try
                        {
                            fastQFormatter = new FastQFormatter();
                            fastQFormatter.Format(null as ISequence, filepath);
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
                            IEnumerable<IQualitativeSequence> sequence = fastQParser.Parse();
                            fastQFormatter = new FastQFormatter();
                            fastQFormatter.Format(sequence, null);
                            Assert.Fail();
                        }
                        catch (ArgumentNullException)
                        {
                            ApplicationLog.WriteLine("FastQ Parser P2 : Successfully validated the exception");
                        }
                        break;
                }
            }
        }

        #endregion Helper Methods
    }
}