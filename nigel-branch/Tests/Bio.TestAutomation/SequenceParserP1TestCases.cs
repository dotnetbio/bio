/****************************************************************************
 * SequenceParserP1TestCases.cs
 * 
 *   This file contains the SequenceParser P1 test cases.
 * 
***************************************************************************/

using System.Collections.Generic;
using Bio.IO;
using Bio.IO.FastA;
using Bio.IO.FastQ;
using Bio.IO.GenBank;
using Bio.IO.Gff;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if (SILVERLIGHT == false)

namespace Bio.TestAutomation.IO
#else
    namespace Bio.Silverlight.TestAutomation.IO
#endif
{
    /// <summary>
    ///     SequenceParser P1 Test case implementation.
    /// </summary>
    [TestClass]
    public class SequenceParserP1TestCases
    {
        #region Global Variables

        private readonly Utility utilityObj = new Utility(@"TestUtils\SequenceParser.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        ///     Static constructor to open log and make other settings needed for test
        /// </summary>
        static SequenceParserP1TestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region SequenceParser TestCases

        /// <summary>
        ///     Find a parser name, description for the Fasta file.
        ///     Input : FastA Files
        ///     Validation : Expected parser, parser type and description.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateFastAFileParser()
        {
            ValidateSequenceFileParser(Constants.FastAFileParserNode, true);
        }

        /// <summary>
        ///     Find a parser name, description for the GenBank file.
        ///     Input : GenBank Files
        ///     Validation : Expected parser, parser type and description.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankFileParser()
        {
            ValidateSequenceFileParser(Constants.GenBankFileParserNode, true);
        }

        /// <summary>
        ///     Find a parser name, description for the FastQ file.
        ///     Input : FastQ Files
        ///     Validation : Expected parser, parser type and description.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateFastQFileParser()
        {
            ValidateSequenceFileParser(Constants.FastQFileParserNode, true);
        }

        /// <summary>
        ///     Find a parser name, description for the Gff file.
        ///     Input : GFF Files
        ///     Validation : Expected parser, parser type and description.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGffFileParser()
        {
            ValidateSequenceFileParser(Constants.GffFileParserNode, true);
        }

        /// <summary>
        ///     Find a formatter name, description for the Fasta file.
        ///     Input : FastA Files
        ///     Validation : Expected formatter, formatter type and description.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateFastAFileFormatter()
        {
            ValidateSequenceFileParser(Constants.FastAFileFormatterNode, false);
        }

        /// <summary>
        ///     Find a formatter name, description for the GenBank file.
        ///     Input : GenBank Files
        ///     Validation : Expected formatter, formatter type and description.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankFileFormatter()
        {
            ValidateSequenceFileParser(Constants.GenBankFileFormatterNode, false);
        }

        /// <summary>
        ///     Find a formatter name, description for the Gff file.
        ///     Input : Gff Files
        ///     Validation : Expected formatter, formatter type and description.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGffFileFormatter()
        {
            ValidateSequenceFileParser(Constants.GffFileFormatterNode, false);
        }

        /// <summary>
        ///     Find a formatter name, description for the FastQ file.
        ///     Input : FastQ Files
        ///     Validation : Expected formatter, formatter type and description.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateFastQFileFormatter()
        {
            ValidateSequenceFileParser(Constants.FastQFileFormatterNode, false);
        }

        /// <summary>
        ///     Valildate SequenceParser class properties.
        ///     Validation : Expected parser, parser type and description.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateSeqParserProperties()
        {
            // Gets the expected sequence from the Xml
            string fastaParserName = utilityObj.xmlUtil.GetTextValue(Constants.FastAFileParserNode,
                                                                     Constants.ParserNameNode);
            string genBankParserName = utilityObj.xmlUtil.GetTextValue(Constants.GenBankFileParserNode,
                                                                       Constants.ParserNameNode);
            string gffParserName = utilityObj.xmlUtil.GetTextValue(Constants.GffFileParserNode,
                                                                   Constants.ParserNameNode);
            string fastQParserName = utilityObj.xmlUtil.GetTextValue(Constants.FastQFileParserNode,
                                                                     Constants.ParserNameNode);

            // Get SequenceParser class properties.
            FastAParser actualFastAParser = SequenceParsers.Fasta;
            IList<ISequenceParser> allParser = SequenceParsers.All;
            GenBankParser actualgenBankParserName = SequenceParsers.GenBank;
            FastQParser actualFastQParserName = SequenceParsers.FastQ;
            GffParser actualGffParserName = SequenceParsers.Gff;

            // Validate Sequence parsers
            Assert.AreEqual(fastaParserName, actualFastAParser.Name);
            Assert.AreEqual(genBankParserName, actualgenBankParserName.Name);
            Assert.AreEqual(gffParserName, actualGffParserName.Name);
            Assert.AreEqual(fastQParserName, actualFastQParserName.Name);
            Assert.IsNotNull(allParser);
            ApplicationLog.WriteLine("Type of the parser is validated successfully");
        }

        /// <summary>
        /// Validate SequenceFormatter class properties.
        /// Validation : Expected parser, parser type and description.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateSeqFormatterProperties()
        {
            // Gets the expected sequence from the Xml
            string fastaFormatterName = utilityObj.xmlUtil.GetTextValue(Constants.FastAFileParserNode,
                                                                        Constants.ParserNameNode);
            string genBankFormatterName = utilityObj.xmlUtil.GetTextValue(Constants.GenBankFileParserNode,
                                                                          Constants.ParserNameNode);
            string gffFormatterName = utilityObj.xmlUtil.GetTextValue(Constants.GffFileParserNode,
                                                                      Constants.ParserNameNode);
            string fastQFormatterName = utilityObj.xmlUtil.GetTextValue(Constants.FastQFileParserNode,
                                                                        Constants.ParserNameNode);

            // Get SequenceFormatter class properties.
            FastAFormatter actualFastAFormatter = SequenceFormatters.Fasta;
            IList<ISequenceFormatter> allFormatters = SequenceFormatters.All;
            GenBankFormatter actualgenBankFormatterName = SequenceFormatters.GenBank;
            FastQFormatter actualFastQFormatterName = SequenceFormatters.FastQ;
            GffFormatter actualGffFormatterName = SequenceFormatters.Gff;

            // Validate Sequence Formatter
            Assert.AreEqual(fastaFormatterName, actualFastAFormatter.Name);
            Assert.AreEqual(genBankFormatterName, actualgenBankFormatterName.Name);
            Assert.AreEqual(gffFormatterName, actualGffFormatterName.Name);
            Assert.AreEqual(fastQFormatterName, actualFastQFormatterName.Name);
            Assert.IsNotNull(allFormatters);
            ApplicationLog.WriteLine("Type of the parser is validated successfully");
        }

        #endregion SequenceParser TestCases

        #region Supporting Methods

        /// <summary>
        ///     Validates general Sequence Parser.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="IsParser">
        ///     IsParser is true if testcases is validating Parsers,
        ///     false if formatter validation
        /// </param>
        private void ValidateSequenceFileParser(string nodeName, bool IsParser)
        {
            // Gets the expected sequence from the Xml
            string[] filePaths = utilityObj.xmlUtil.GetTextValues(nodeName,
                                                                  Constants.FilePathsNode);
            string parserDescription = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                       Constants.DescriptionNode);
            string parserName = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                Constants.ParserNameNode);
            string fileTypes = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                               Constants.FileTypesNode);

            // Get a default parser for the file types.
            for (int i = 0; i < filePaths.Length; i++)
            {
                if (IsParser)
                {
                    using (ISequenceParser parser = SequenceParsers.FindParserByFileName(filePaths[i]))
                    {
                        string description = parser.Description.Replace("\r", "").Replace("\n", "");
                        // Validate parser name, description and the file type supported by parser.
                        Assert.AreEqual(parserName, parser.Name);
                        Assert.AreEqual(parserDescription, description);
                        Assert.AreEqual(fileTypes, parser.SupportedFileTypes);
                    }
                }
                else
                {
                    using (ISequenceFormatter formatter =
                        SequenceFormatters.FindFormatterByFileName(filePaths[i]))
                    {
                        string description =
                            formatter.Description.Replace("\r", "").Replace("\n", "");
                        // Validate parser name, description and the file type supported by parser.
                        Assert.AreEqual(parserName, formatter.Name);
                        Assert.AreEqual(parserDescription, description);
                        Assert.AreEqual(fileTypes, formatter.SupportedFileTypes);
                    }
                }
            }

            ApplicationLog.WriteLine("Type of the parser is validated successfully");
        }

        #endregion Supporting Methods
    }
}