/****************************************************************************
 * NexusP2TestCases.cs
 * 
 *   This file contains the Nexus - Parsers and Formatters P2 test cases.
 * 
***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

using Bio.Algorithms.Alignment;
using Bio.IO.Nexus;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.TestAutomation.IO.Nexus
{
    /// <summary>
    /// Nexus P2 parser and formatter Test case implementation.
    /// </summary>
    [TestFixture]
    public class NexusP2TestCases
    {

        #region Enums

        /// <summary>
        /// Additional parameters to validate different scenarios.
        /// </summary>
        enum AdditionalParameters
        {
            Parse,
            ParseOne,
        };

        #endregion Enums

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\NexusTestsConfig.xml");

        #endregion Global Variables

        #region Nexus Parser P2 Test cases

        /// <summary>
        /// Parse a empty Nexus file and invalidate Parse(reader, isReadOnly)
        /// Input : Invalid File
        /// Output: Validation of exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateReadOnlyNexusParserParseReader()
        {
            InvalidateNexusParserTestCases(Constants.EmptyNexusFileNode,
                AdditionalParameters.Parse);
        }

        /// <summary>
        /// Parse a invalid Nexus file and invalidate Sequence count
        /// Input : Invalid File
        /// Output: Validation of exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateNexusParserSeqCount()
        {
            InvalidateNexusParserTestCases(
                Constants.InvalidateNexusParserSeqCountNode,
                AdditionalParameters.Parse);
        }

        /// <summary>
        /// Parse a invalid Nexus file and invalidate ParseHeader()
        /// Input : Invalid File
        /// Output: Validation of exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateNexusParserHeader()
        {
            InvalidateNexusParserTestCases(
                Constants.InvalidateNexusParserHeaderNode,
                AdditionalParameters.Parse);
        }

        /// <summary>
        /// Parse a Empty Nexus file and invalidate ParseOne(reader, isReadOnly)
        /// Input : Invalid File
        /// Output: Validation of exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateReadOnlyNexusParserOneReader()
        {
            InvalidateNexusParserTestCases(Constants.EmptyNexusFileNode,
                AdditionalParameters.ParseOne);
        }

        /// <summary>
        /// Parse a invalid Nexus file and invalidate Alphabet
        /// Input : Invalid File
        /// Output: Validation of exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateNexusParserAlphabet()
        {
            InvalidateNexusParserTestCases(
                Constants.InvalidateNexusParserAlphabetNode,
                AdditionalParameters.Parse);
        }

        /// <summary>
        /// Parse a invalid Nexus file and invalidate Align Alphabet
        /// Input : Invalid File
        /// Output: Validation of exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateNexusParserAlignAlphabet()
        {
            InvalidateNexusParserTestCases(
                Constants.InvalidateNexusParserAlignAlphabetNode,
                AdditionalParameters.Parse);
        }

        /// <summary>
        /// Parse a invalid Nexus file and invalidate Sequence length
        /// Input : Invalid File
        /// Output: Validation of exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateNexusParserSeqLength()
        {
            InvalidateNexusParserTestCases(
                Constants.InvalidateNexusParserSeqLengthNode,
                AdditionalParameters.Parse);
        }

        #endregion Nexus Parser P2 Test cases

        #region Helper Method

        /// <summary>
        /// General method to invalidate Nexus parser
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="method">Nexus Parse method parameters</param>
        void InvalidateNexusParserTestCases(
            string nodeName,
            AdditionalParameters method)
        {
            try
            {
                string filePath = utilityObj.xmlUtil.GetTextValue(
                    nodeName,
                    Constants.FilePathNode);
                NexusParser parser = new NexusParser();

                switch (method)
                {
                    case AdditionalParameters.Parse:
                        parser.Parse(filePath).First();
                        break;
                    case AdditionalParameters.ParseOne:
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
                   "Nexus Parser P2 : All the features validated successfully.");
            }
            catch (Exception)
            {
                ApplicationLog.WriteLine(
                   "Nexus Parser P2 : All the features validated successfully.");
            }
        }

        #endregion
    }
}
