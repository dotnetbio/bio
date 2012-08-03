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
using System.Xml;

using Bio.Algorithms.Alignment;
using Bio.IO.Nexus;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.TestAutomation.IO.Nexus
{
    /// <summary>
    /// Nexus P2 parser and formatter Test case implementation.
    /// </summary>
    [TestClass]
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

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static NexusP2TestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region Nexus Parser P2 Test cases

        /// <summary>
        /// Parse a empty Nexus file and invalidate Parse(reader, isReadOnly)
        /// Input : Invalid File
        /// Output: Validation of exception
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
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
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
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
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
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
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
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
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
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
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
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
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
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
                        parser.Parse(filePath);
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
                Console.WriteLine(
                    "Nexus Parser P2 : All the features validated successfully.");
            }
            catch (FormatException)
            {
                ApplicationLog.WriteLine(
                   "Nexus Parser P2 : All the features validated successfully.");
                Console.WriteLine(
                    "Nexus Parser P2 : All the features validated successfully.");
            }
        }

        #endregion
    }
}
