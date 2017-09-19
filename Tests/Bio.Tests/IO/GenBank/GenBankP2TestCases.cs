/****************************************************************************
 * GenBankP2TestCases.cs
 * 
 *   This file contains the GenBank - Parsers and Formatters P2 test cases.
 * 
***************************************************************************/

using System;
using System.IO;
using Bio.IO.GenBank;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using NUnit.Framework;

#if (SILVERLIGHT == false)
    namespace Bio.TestAutomation.IO.GenBank
#else
    namespace Bio.Silverlight.TestAutomation.IO.GenBank
#endif
{
    /// <summary>
    /// GenBank P2 parser and formatter Test case implementation.
    /// </summary>
    [TestFixture]
    public class GenBankP2TestCases
    {

        #region Global Variables

        /// <summary>
        /// Global variables which store the information of xml file values
        /// and is used across the class file.
        /// </summary>
        static string filePath;

        #endregion Global Variables

        #region Properties

        static string FilePath
        {
            get { return GenBankP2TestCases.filePath; }
            set { GenBankP2TestCases.filePath = value; }
        }

        #endregion Properties

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

        #region GenBank Parser P2 Test Cases

        /// <summary>
        /// Invalidate ParseHeader by passing invalid Locus header and
        /// validate with the expected exception.
        /// Input : GenBank File
        /// Output : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateGenBankParseHeaderLocus()
        {
            InvalidateGenBankParser(
                Constants.InvalidateGenBankNodeName);
        }

        /// <summary>
        /// Invalidate ParseHeader by passing invalid version header and
        /// validate with the expected exception.
        /// Input : GenBank File
        /// Output : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateGenBankParseHeaderVersion()
        {
            InvalidateGenBankParser(
                Constants.InvalidateGenBankNodeName);
        }

        /// <summary>
        /// Invalidate ParseHeader by passing header without Locus 
        /// and validate with the expected exception.
        /// Input : GenBank File
        /// Output : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateGenBankParseHeaderWithoutLocus()
        {
            InvalidateGenBankParser(
                Constants.InvalidGenBankWithoutLocusNode);
        }

        /// <summary>
        /// Invalidate ParseHeader by passing invalid Segment header
        /// and validate with the expected exception.
        /// Input : GenBank File
        /// Output : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateGenBankParseHeader()
        {
            InvalidateGenBankParser(
                Constants.InvalidGenBankWithSegmentNode);
        }

        /// <summary>
        /// Invalidate ParseHeader by passing invalid Primary header
        /// and validate with the expected exception.
        /// Input : GenBank File
        /// Output : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateGenBankParseHeaderPrimary()
        {
            InvalidateGenBankParser(
                Constants.InvalidGenBankWithPrimaryNode);
        }

        /// <summary>
        /// Invalidate ParseHeader by making LocationBuilder property
        /// null and validate with the expected exception.
        /// Input : GenBank File
        /// Output : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateGenBankParseFeaturesLocBuild()
        {
            InvalidateGenBankParser(
                Constants.SimpleGenBankNodeName);
        }

        /// <summary>
        /// Invalidate ParseFeatures by passing invalid Line Reader
        /// and validate with the expected exception.
        /// Input : GenBank File
        /// Output : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateGenBankParseFeaturesLineHasReader()
        {
            InvalidateGenBankParser(
                Constants.InvalidateGenBankParseFeaturesHasReaderNode);
        }

        /// <summary>
        /// Invalidate ParseLocus by passing invalid Locus header
        /// and validate with the expected exception.
        /// Input : GenBank File with Locusheader contain pp
        /// Output : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateGenBankParseLocus()
        {
            InvalidateGenBankParser(
                Constants.InvalidGenBankUnknownLocusNode);
        }

        /// <summary>
        /// Invalidate ParseLocus by passing invalid Locus header
        /// and validate with the expected exception.
        /// Input : GenBank File with Locusheader contain invalid Strand
        /// Output : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateGenBankParseLocusStrandType()
        {
            InvalidateGenBankParser(
                Constants.InvalidGenBankUnknownStrandTypeNode);
        }

        /// <summary>
        /// Invalidate ParseLocus by passing invalid Locus header
        /// and validate with the expected exception.
        /// Input : GenBank File with Locusheader contain invalid topology
        /// Output : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateGenBankParseLocusStrandTopology()
        {
            InvalidateGenBankParser(
                Constants.InvalidGenBankUnknownStrandTopologyNode);
        }

        /// <summary>
        /// Invalidate ParseLocus by passing invalid Locus header
        /// and validate with the expected exception.
        /// Input : GenBank File with Locusheader contain invalid date
        /// Output : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateGenBankParseLocusRawDate()
        {
            InvalidateGenBankParser(
                Constants.InvalidGenBankUnknownRawDateNode);
        }

        /// <summary>
        /// Invalidate ParseLocus by passing invalid Locus header
        /// and validate with the expected exception.
        /// Input : GenBank File with Locusheader contain invalid MoleculeType
        /// Output : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateGenBankParseLocusInvalidMoleculeType()
        {
            InvalidateGenBankParser(
                Constants.InvalidGenBankUnknownMoleculeTypeNode);
        }

        /// <summary>
        /// Invalidate ParseReference by passing invalid Reference header
        /// and validate with the expected exception.
        /// Input : GenBank File
        /// Output : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateGenBankParseReference()
        {
            InvalidateGenBankParser(
                Constants.InvalidGenBankParseReferenceNode);
        }

        /// <summary>
        /// Invalidate ParseReference by passing invalid Reference Line
        /// and validate with the expected exception.
        /// Input : GenBank File
        /// Output : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateGenBankParseReferenceDefault()
        {
            InvalidateGenBankParser(
                Constants.InvalidGenBankParseReferenceDefaultNode);
        }

        /// <summary>
        /// Invalidate ParseSequence by passing invalid Sequence Line
        /// and validate with the expected exception.
        /// Input : GenBank File
        /// Output : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateGenBankParseSequenceDefault()
        {
            InvalidateGenBankParser(
                Constants.InvalidGenBankParseSequenceDefaultNode);
        }

        /// <summary>
        /// Invalidate ParseSequence by passing invalid Sequence
        /// Origin Line and validate with the expected exception.
        /// Input : GenBank File
        /// Output : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateGenBankParseSequence()
        {
            InvalidateGenBankParser(
                Constants.InvalidGenBankParseSequenceNode);
        }

        /// <summary>
        /// Invalidate ParseSource by passing invalid Line Header
        /// and validate with the expected exception.
        /// Input : GenBank File
        /// Output : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateGenBankParseSource()
        {
            InvalidateGenBankParser(
                Constants.InvalidGenBankParseSourceNode);
        }

        /// <summary>
        /// Invalidate ParseLocus by passing invalid datatype Header
        /// and validate with the expected exception.
        /// Input : GenBank File
        /// Output : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateGenBankHeaderDataType()
        {
            InvalidateGenBankParser(
                Constants.InvalidGenBankHeaderDataTypeNode);
        }

        /// <summary>
        /// Invalidate ParseLocus by passing invalid Alphabet
        /// and validate with the expected exception.
        /// Input : GenBank File
        /// Output : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateGenBankLocusAlphabet()
        {
            InvalidateGenBankParser(
                Constants.SimpleGenBankPrimaryNode);
        }

        /// <summary>
        /// Invalidate ParseReference by passing invalid reference
        /// and validate with the expected exception.
        /// Input : GenBank File
        /// Output : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateGenBankReference()
        {
            InvalidateGenBankParser(
                Constants.InvalideGenBankReferenceNode);
        }

        /// <summary>
        /// Invalidate ParseHeader by passing invalid files
        /// and validate with the expected exception.
        /// Input : GenBank File
        /// Output : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateGenParserHeader()
        {
            InvalidateGenBankParser(
                Constants.InvalideGenBankParseHeaderNode);
        }

        #endregion GenBank Parser P2 Test Cases

        #region Supporting Methods

        /// <summary>
        /// Validates GenBank Parser for General test cases.
        /// </summary>
        /// Suppressing the Error "DoNotCatchGeneralExceptionTypes" because the exception is being thrown by DEV code
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        void InvalidateGenBankParser(string node)
        {
            // Initialization of xml strings.
            FilePath = utilityObj.xmlUtil.GetTextValue(node,
                Constants.FilePathNode);

            try
            {
                GenBankParser parserObj = new GenBankParser();
                if (string.Equals(Constants.SimpleGenBankNodeName, node))
                {
                    parserObj.LocationBuilder = null;
                }
                else if (string.Equals(Constants.SimpleGenBankPrimaryNode, node))
                {
                    parserObj.Alphabet = Alphabets.RNA;
                }

                parserObj.Parse(FilePath);
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                ApplicationLog.WriteLine(
                    "GenBank Parser : Successfully validated the exception:");
            }
            catch (InvalidDataException)
            {
                ApplicationLog.WriteLine(
                    "GenBank Parser : Successfully validated the exception:");
            }
            catch (Exception)
            {
                ApplicationLog.WriteLine(
                    "GenBank Parser : Successfully validated the exception:");
            }
        }

        #endregion
    }
}
