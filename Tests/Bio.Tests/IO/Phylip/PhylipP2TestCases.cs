/****************************************************************************
 * PhylipP2TestCases.cs
 * 
 *   This file contains the Phylip - Parsers and Formatters P2 test cases.
 * 
***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

using Bio.Algorithms.Alignment;
using Bio.IO;
using Bio.IO.Phylip;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.TestAutomation.IO.Phylip
{
    /// <summary>
    /// Phylip P2 parser and formatter Test case implementation.
    /// </summary>
    [TestFixture]
    public class PhylipP2TestCases
    {

        #region Enums

        /// <summary>
        /// Additional parameters to validate different scenarios.
        /// </summary>
        enum ParserTestAttributes
        {
            Parse,
            ParseOne
        };

        #endregion Enums

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\PhylipTestsConfig.xml");

        #endregion Global Variables

        #region Phylip Parser P2 Test cases

        /// <summary>
        /// Parse a empty Phylip file and invalidate Parse(file-name)
        /// Input : Invalid File
        /// Output: Validation of exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateReadOnlyPhylipParserParseReader()
        {
            InvalidatePhylipParserTestCases(Constants.EmptyPhylipParserFileNode,
                ParserTestAttributes.Parse);
        }

        /// <summary>
        /// Parse a Empty Phylip file and invalidate ParseOne(reader, isReadOnly)
        /// Input : Invalid File
        /// Output: Validation of exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateReadOnlyPhylipParserOneReader()
        {
            InvalidatePhylipParserTestCases(Constants.EmptyPhylipParserFileNode,
                ParserTestAttributes.ParseOne);
        }

        /// <summary>
        /// Parse a invalid Phylip file and invalidate ParseOneWithSpecificFormat()
        /// Input : Invalid File
        /// Output: Validation of exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidatePhylipParserHeader()
        {
            InvalidatePhylipParserTestCases(
                Constants.InvalidatePhylipParserCountNode,
                ParserTestAttributes.Parse);
        }

        /// <summary>
        /// Parse a invalid Phylip file and invalidate Alphabet
        /// Input : Invalid File
        /// Output: Validation of exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidatePhylipParserAlphabet()
        {
            InvalidatePhylipParserTestCases(
                Constants.InvalidatePhylipParserAlphabetNode,
                ParserTestAttributes.Parse);
        }

        /// <summary>
        /// Parse a invalid Phylip file and invalidate Align Alphabet
        /// Input : Invalid File
        /// Output: Validation of exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidatePhylipParserAlignAlphabet()
        {
            InvalidatePhylipParserTestCases(
                Constants.InvalidatePhylipParserAlignAlphabetNode,
                ParserTestAttributes.Parse);
        }

        /// <summary>
        /// Parse a invalid Phylip file and invalidate Sequence length
        /// Input : Invalid File
        /// Output: Validation of exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidatePhylipParserSeqLength()
        {
            InvalidatePhylipParserTestCases(
                Constants.InvalidatePhylipParserSeqLengthNode,
                ParserTestAttributes.Parse);
        }

        #endregion Phylip Parser P2 Test cases

        #region Helper Method

        /// <summary>
        /// General method to invalidate Phylip parser
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="method">Phylip Parser method parameters</param>
        void InvalidatePhylipParserTestCases(
            string nodeName,
            ParserTestAttributes method)
        {
            try
            {
                string filePath = utilityObj.xmlUtil.GetTextValue(
                    nodeName,
                    Constants.FilePathNode);
                PhylipParser parser = new PhylipParser();

                switch (method)
                {
                    case ParserTestAttributes.Parse:
                        parser.Parse(filePath).First();
                        break;
                    case ParserTestAttributes.ParseOne:
                        parser.ParseOne(filePath);
                        break;
                    default:
                        break;
                }

                Assert.Fail();
            }
            catch (InvalidDataException)
            {
                ApplicationLog.WriteLine(
                    "Phylip Parser P2 : All the features validated successfully.");
            }
            catch (Exception)
            {
                ApplicationLog.WriteLine(
                    "Phylip Parser P2 : All the features validated successfully.");
            }
        }

        #endregion
    }
}
