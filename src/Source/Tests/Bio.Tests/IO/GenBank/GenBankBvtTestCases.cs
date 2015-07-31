/****************************************************************************
 * GenBankBvtTestCases.cs
 * 
 *   This file contains the GenBank - Parsers and Formatters Bvt test cases.
 * 
***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Bio.Extensions;
using Bio.IO;
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
    ///     GenBank Bvt parser and formatter Test case implementation.
    /// </summary>
    [TestFixture]
    public class GenBankBvtTestCases
    {
        #region Global Variables

        // Global variables which store the information of xml file values and is used across the class file.

        private readonly Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

        #region Properties

        static string AlphabetName { get; set; }
        static string FilePath { get; set; }
        static string SeqId { get; set; }
        static string StrandTopology { get; set; }
        static string StrandType { get; set; }
        static string Div { get; set; }
        static string Version { get; set; }
        static string SequenceDate { get; set; }
        static string PrimaryId { get; set; }
        static string ExpectedSequence { get; set; }

        #endregion Properties

        #region GenBank Parser BVT Test cases

        /// <summary>
        ///     Parse a valid GenBank file (Small size sequence less than 35 kb) and
        ///     convert the same to one sequence using Parse(file-name) method and
        ///     validate with the expected sequence.
        ///     Input : GenBank File
        ///     Validation: Properties like StrandType, StrandTopology, Division, Date,
        ///     Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void GenBankParserValidateParseFileName()
        {
            InitializeXmlVariables();

            // parse            
            ISequenceParser parserObj = new GenBankParser();
            {
                IEnumerable<ISequence> seqList = parserObj.Parse(FilePath);
                ISequence seq = seqList.ElementAt(0);
                Assert.AreEqual(Utility.GetAlphabet(AlphabetName), seq.Alphabet);
                Assert.AreEqual(SeqId, seq.ID);
                ApplicationLog.WriteLine(
                    "GenBank Parser BVT: Successfully validated the Alphabet, Molecular type, Sequence ID and Display ID");

                // test the metadata that is tricky to parse, and will not be tested implicitly by
                // testing the formatting
                var metadata = (GenBankMetadata) seq.Metadata["GenBank"];
                if (metadata.Locus.Strand != SequenceStrandType.None)
                {
                    Assert.AreEqual(StrandType,
                                    metadata.Locus.Strand.ToString());
                }
                Assert.AreEqual(StrandTopology.ToUpper(CultureInfo.CurrentCulture),
                                metadata.Locus.StrandTopology.ToString().ToUpper(CultureInfo.CurrentCulture));
                Assert.AreEqual(Div, metadata.Locus.DivisionCode.ToString());
                Assert.AreEqual(DateTime.Parse(SequenceDate, null),
                                metadata.Locus.Date);

                Assert.AreEqual(Version, metadata.Version.Version.ToString(null));
                Assert.AreEqual(PrimaryId, metadata.Version.GiNumber);
                ApplicationLog.WriteLine("GenBank Parser BVT: Successfully validated the StrandType, StrandTopology, Division, Date, Version, PrimaryID Properties");

                // test the sequence string            
                Assert.AreEqual(ExpectedSequence, seq.ConvertToString());

                ApplicationLog.WriteLine("GenBank Parser BVT: Successfully validated the Sequence");
            }
        }

        /// <summary>
        ///     Parse a valid GenBank file (Small size sequence less than 35 kb) and
        ///     convert the same to one sequence using Parse(Stream) method and
        ///     validate with the expected sequence.
        ///     Input : GenBank File
        ///     Validation: Properties like StrandType, StrandTopology, Division, Date,
        ///     Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void GenBankParserValidateParseFileNameWithStream()
        {
            InitializeXmlVariables();
            List<ISequence> seq = null;
            IEnumerable<ISequence> seqList = null;

            // Parse the Stream.           
            ISequenceParser parserObj = new GenBankParser();
            {
                using (var reader = File.OpenRead(FilePath))
                {
                    seqList = parserObj.Parse(reader);
                    seq = seqList.ToList();
                }

                Assert.AreEqual(Utility.GetAlphabet(AlphabetName), seq[0].Alphabet);
                Assert.AreEqual(SeqId, seq[0].ID);
                ApplicationLog.WriteLine(
                    "GenBank Parser BVT: Successfully validated the Alphabet, Molecular type, Sequence ID and Display ID");

                // test the metadata that is tricky to parse, and will not be tested implicitly by
                // testing the formatting
                var metadata = (GenBankMetadata) seq[0].Metadata["GenBank"];
                if (metadata.Locus.Strand != SequenceStrandType.None)
                {
                    Assert.AreEqual(StrandType,
                                    metadata.Locus.Strand.ToString());
                }
                Assert.AreEqual(StrandTopology.ToUpper(CultureInfo.CurrentCulture),
                                metadata.Locus.StrandTopology.ToString().ToUpper(CultureInfo.CurrentCulture));
                Assert.AreEqual(Div, metadata.Locus.DivisionCode.ToString());
                Assert.AreEqual(DateTime.Parse(SequenceDate, null),
                                metadata.Locus.Date);

                Assert.AreEqual(Version, metadata.Version.Version.ToString(null));
                Assert.AreEqual(PrimaryId, metadata.Version.GiNumber);
                ApplicationLog.WriteLine(
                    "GenBank Parser BVT: Successfully validated the StrandType, StrandTopology, Division, Date, Version, PrimaryID Properties");

                // test the sequence string            
                Assert.AreEqual(ExpectedSequence, new string(seq[0].Select(a => (char) a).ToArray()));

                ApplicationLog.WriteLine(
                    "GenBank Parser BVT: Successfully validated the Sequence");
            }
        }

        /// <summary>
        ///     Parse a valid GenBank file (Small size sequence less than 35 kb) and
        ///     convert the same to one sequence using ParseOne(file-name) method and
        ///     set Alphabet and Encoding value and validate with the expected sequence.
        ///     Input : GenBank File
        ///     Output : Properties like StrandType, StrandTopology, Division, Date,
        ///     Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void GenBankParserValidateParseOneWithSpecificFormats()
        {
            InitializeXmlVariables();
            // Initialization of xml strings.
            FilePath = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankPrimaryNode,
                Constants.FilePathNode);
            AlphabetName = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankPrimaryNode,
                Constants.AlphabetNameNode);
            SeqId = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankPrimaryNode,
                Constants.SequenceIdNode);
            StrandTopology = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankPrimaryNode,
                Constants.StrandTopologyNode);
            StrandType = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankPrimaryNode,
                Constants.StrandTypeNode);
            Div = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankPrimaryNode,
                Constants.DivisionNode);
            Version = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankPrimaryNode,
                Constants.VersionNode);
            SequenceDate = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankPrimaryNode,
                Constants.DateNode);
            PrimaryId = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankPrimaryNode,
                Constants.PrimaryIdNode);
            ExpectedSequence = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankPrimaryNode,
                Constants.ExpectedSequenceNode);

            // parse            
            ISequenceParser parserObj = new GenBankParser();
            {
                parserObj.Alphabet = Alphabets.Protein;
                IEnumerable<ISequence> seq = parserObj.Parse(FilePath);

                Assert.AreEqual(Utility.GetAlphabet(AlphabetName),
                                seq.ElementAt(0).Alphabet);
                Assert.AreEqual(SeqId, seq.ElementAt(0).ID);
                ApplicationLog.WriteLine(
                    "GenBank Parser BVT: Successfully validated the Alphabet, Molecular type, Sequence ID and Display ID");

                // test the metadata that is tricky to parse, and will not be tested implicitly by
                // testing the formatting
                var metadata = (GenBankMetadata) seq.ElementAt(0).Metadata["GenBank"];
                if (metadata.Locus.Strand != SequenceStrandType.None)
                {
                    Assert.AreEqual(StrandType,
                                    metadata.Locus.Strand.ToString());
                }
                Assert.AreEqual(StrandTopology.ToUpper(CultureInfo.CurrentCulture),
                                metadata.Locus.StrandTopology.ToString().ToUpper(
                                    CultureInfo.CurrentCulture));
                Assert.AreEqual(Div, metadata.Locus.DivisionCode.ToString());
                Assert.AreEqual(DateTime.Parse(SequenceDate, null),
                                metadata.Locus.Date);
                Assert.AreEqual(Version, metadata.Version.Version.ToString(null));
                Assert.AreEqual(PrimaryId, metadata.Version.GiNumber);
                ApplicationLog.WriteLine(
                    "GenBank Parser BVT: Successfully validated the StrandType, StrandTopology, Division, Date, Version, PrimaryID Properties");

                // test the sequence string            
                Assert.AreEqual(ExpectedSequence, new string(seq.ElementAt(0).Select(a => (char) a).ToArray()));
                ApplicationLog.WriteLine(
                    "GenBank Parser BVT: Successfully validated the Sequence");
            }
        }

        #endregion GenBank Parser BVT Test cases

        #region GenBank Formatter BVT Test cases

        /// <summary>
        ///     Write a valid Sequence (Small size sequence  less than 35 kb) to a
        ///     GenBank file using GenBankFormatter(File-Info) constructor and
        ///     validate the same.
        ///     Input : GenBank Sequence
        ///     Validation :  Read the GenBank file to which the sequence was formatted
        ///     using File-Info and Validate Properties like StrandType, StrandTopology,
        ///     Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void GenBankFormatterValidateWrite()
        {
            InitializeXmlVariables();

            // Create a Sequence with all attributes.
            // parse and update the properties instead of parsing entire file.            
            ISequenceParser parser1 = new GenBankParser();
            {
                IEnumerable<ISequence> seqList1 = parser1.Parse(FilePath);
                string tempFileName = Path.GetTempFileName();

                string expectedUpdatedSequence =
                    ExpectedSequence.Replace("\r", "").Replace("\n", "").Replace(" ", "");
                var orgSeq =
                    new Sequence(Utility.GetAlphabet(AlphabetName), expectedUpdatedSequence);
                orgSeq.Metadata.Add("GenBank",
                                    seqList1.ElementAt(0).Metadata["GenBank"]);
                orgSeq.ID = seqList1.ElementAt(0).ID;

                ISequenceFormatter formatter = new GenBankFormatter();
                {
                    formatter.Format(orgSeq, tempFileName);
                    formatter.Close();

                    // parse            
                    var parserObj = new GenBankParser();

                    IEnumerable<ISequence> seqList = parserObj.Parse(tempFileName);
                    ISequence seq = seqList.ElementAt(0);
                    Assert.AreEqual(Utility.GetAlphabet(AlphabetName), seq.Alphabet);
                    Assert.AreEqual(SeqId, seq.ID);
                    ApplicationLog.WriteLine(
                        "GenBank Formatter BVT: Successfully validated the Alphabet, Molecular type, Sequence ID and Display ID");

                    // test the metadata that is tricky to parse, and will not be tested implicitly by
                    // testing the formatting 
                    var metadata = (GenBankMetadata) seq.Metadata["GenBank"];
                    if (metadata.Locus.Strand != SequenceStrandType.None)
                    {
                        Assert.AreEqual(StrandType, metadata.Locus.Strand.ToString());
                    }
                    Assert.AreEqual(StrandTopology.ToUpper(CultureInfo.CurrentCulture),
                                    metadata.Locus.StrandTopology.ToString().ToUpper(CultureInfo.CurrentCulture));
                    Assert.AreEqual(Div, metadata.Locus.DivisionCode.ToString());
                    Assert.AreEqual(DateTime.Parse(SequenceDate, null), metadata.Locus.Date);
                    Assert.AreEqual(Version, metadata.Version.Version.ToString(null));
                    Assert.AreEqual(PrimaryId, metadata.Version.GiNumber);
                    ApplicationLog.WriteLine(
                        "GenBank Formatter BVT: Successfully validated the StrandType, StrandTopology, Division, Date, Version, PrimaryID Properties");

                    // test the sequence string            
                    Assert.AreEqual(ExpectedSequence, new string(seq.Select(a => (char) a).ToArray()));
                    ApplicationLog.WriteLine("GenBank Formatter BVT: Successfully validated the Sequence");
                    File.Delete(tempFileName);
                }
            }
        }

        /// <summary>
        ///     Write a valid Sequence (Small size sequence  less than 35 kb) to a
        ///     GenBank file using GenBankFormatter(File-Path) constructor and
        ///     validate the same.
        ///     Input : GenBank Sequence
        ///     Validation :  Read the GenBank file to which the sequence was formatted
        ///     using File-Path and Validate Properties like StrandType, StrandTopology,
        ///     Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void GenBankFormatterValidateWriteWithFilePath()
        {
            InitializeXmlVariables();
            ISequenceParser parserObj = new GenBankParser();
            {
                IEnumerable<ISequence> seqList1 = parserObj.Parse(FilePath);
                string tempFileName = Path.GetTempFileName();
                string expectedUpdatedSequence =
                    ExpectedSequence.Replace("\r", "").Replace("\n", "").Replace(" ", "");
                var orgSeq = new Sequence(Utility.GetAlphabet(AlphabetName), expectedUpdatedSequence)
                {
                    ID = seqList1.ElementAt(0).ID
                };
                orgSeq.Metadata.Add("GenBank", seqList1.ElementAt(0).Metadata["GenBank"]);
                ISequenceFormatter formatter = new GenBankFormatter();
                {
                    formatter.Format(orgSeq, tempFileName);

                    // parse
                    ISequenceParser parserObjFromFile = new GenBankParser();
                    IEnumerable<ISequence> seqList = parserObjFromFile.Parse(tempFileName);
                    ISequence seq = seqList.ElementAt(0);
                    Assert.AreEqual(Utility.GetAlphabet(AlphabetName), seq.Alphabet);
                    Assert.AreEqual(SeqId, seq.ID);
                    ApplicationLog.WriteLine("GenBank Formatter BVT: Successfully validated the Alphabet, Molecular type, Sequence ID and Display ID");

                    // test the metadata that is tricky to parse, and will not be tested implicitly by
                    // testing the formatting
                    var metadata =
                        (GenBankMetadata) orgSeq.Metadata["GenBank"];
                    if (metadata.Locus.Strand != SequenceStrandType.None)
                    {
                        Assert.AreEqual(StrandType,
                                        metadata.Locus.Strand.ToString());
                    }
                    Assert.AreEqual(StrandTopology.ToUpper(CultureInfo.CurrentCulture),
                                    metadata.Locus.StrandTopology.ToString().ToUpper(CultureInfo.CurrentCulture));
                    Assert.AreEqual(Div, metadata.Locus.DivisionCode.ToString());
                    Assert.AreEqual(DateTime.Parse(SequenceDate, null),
                                    metadata.Locus.Date);
                    Assert.AreEqual(Version, metadata.Version.Version.ToString(null));
                    Assert.AreEqual(PrimaryId, metadata.Version.GiNumber);
                    ApplicationLog.WriteLine(
                        "GenBank Formatter BVT: Successfully validated the StrandType, StrandTopology, Division, Date, Version, PrimaryID Properties");

                    // test the sequence string            
                    Assert.AreEqual(ExpectedSequence, new string(seq.Select(a => (char) a).ToArray()));
                    ApplicationLog.WriteLine("GenBank Formatter BVT: Successfully validated the Sequence");
                    File.Delete(tempFileName);
                }
            }
        }

        /// <summary>
        ///     Parse a GenBank File (Small size sequence less than 35 kb) using Parse()
        ///     method and Format the same to a GenBank file using GenBankFormatter(File-Info)
        ///     constructor and validate the same.
        ///     Input : GenBank File
        ///     Validation :  Read the New GenBank file to which the sequence was formatted
        ///     using File-Info and Validate Properties like StrandType, StrandTopology,
        ///     Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void GenBankFormatterWithParseValidateWrite()
        {
            InitializeXmlVariables();
            // parse
            ISequenceParser parserObj = new GenBankParser();

            IEnumerable<ISequence> seqList = parserObj.Parse(FilePath);
            string tempFileName = Path.GetTempFileName();
            ISequence seq = seqList.ElementAt(0);

            ISequenceFormatter formatter = new GenBankFormatter();
            {
                formatter.Format(seq, tempFileName);

                // parse
                parserObj = new GenBankParser();
                seqList = parserObj.Parse(tempFileName);
                seq = seqList.ElementAt(0);
                Assert.AreEqual(Utility.GetAlphabet(AlphabetName), seq.Alphabet);
                Assert.AreEqual(SeqId, seq.ID);
                ApplicationLog.WriteLine(
                    "GenBank Formatter BVT: Successfully validated the Alphabet, Molecular type, Sequence ID and Display ID");

                // test the metadata that is tricky to parse, and will not be tested implicitly by
                // testing the formatting
                var metadata = (GenBankMetadata) seq.Metadata["GenBank"];
                if (metadata.Locus.Strand != SequenceStrandType.None)
                {
                    Assert.AreEqual(StrandType,
                                    metadata.Locus.Strand.ToString());
                }
                Assert.AreEqual(StrandTopology.ToUpper(CultureInfo.CurrentCulture),
                                metadata.Locus.StrandTopology.ToString().ToUpper(CultureInfo.CurrentCulture));
                Assert.AreEqual(Div, metadata.Locus.DivisionCode.ToString());
                Assert.AreEqual(DateTime.Parse(SequenceDate, null),
                                metadata.Locus.Date);
                Assert.AreEqual(Version, metadata.Version.Version.ToString(null));
                Assert.AreEqual(PrimaryId, metadata.Version.GiNumber);
                ApplicationLog.WriteLine(
                    "GenBank Formatter BVT: Successfully validated the StrandType, StrandTopology, Division, Date, Version, PrimaryID Properties");

                // test the sequence string            
                Assert.AreEqual(ExpectedSequence, new string(seq.Select(a => (char) a).ToArray()));
                ApplicationLog.WriteLine(
                    "GenBank Formatter BVT: Successfully validated the Sequence");
                File.Delete(tempFileName);
            }
        }

        /// <summary>
        ///     Parse a GenBank File (Small size sequence less than 35 kb) using Parse()
        ///     method and Write the same to a GenBank file using
        ///     GenBankFormatter(File-Path) constructor and validate the same.
        ///     Input : GenBank File
        ///     Validation :  Read the New GenBank file to which the sequence was formatted
        ///     using File-Path and Validate Properties like StrandType, StrandTopology,
        ///     Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void GenBankFormatterWithParseValidateWriteFilePath()
        {
            InitializeXmlVariables();
            // parse
            ISequenceParser parserObj = new GenBankParser();
            {
                IEnumerable<ISequence> seqList = parserObj.Parse(FilePath);
                ISequence seq = seqList.ElementAt(0);
                string tempFileName = Path.GetTempFileName();
                ISequenceFormatter formatter = new GenBankFormatter();
                {
                    formatter.Format(seq, tempFileName);

                    // parse
                    ISequenceParser parserObjFromFile = new GenBankParser();
                    seqList = parserObjFromFile.Parse(tempFileName);
                    seq = seqList.ElementAt(0);
                    Assert.AreEqual(Utility.GetAlphabet(AlphabetName), seq.Alphabet);
                    Assert.AreEqual(SeqId, seq.ID);
                    ApplicationLog.WriteLine(
                        "GenBank Formatter BVT: Successfully validated the Alphabet, Molecular type, Sequence ID and Display ID");

                    // test the metadata that is tricky to parse, and will not be tested implicitly by
                    // testing the formatting
                    var metadata =
                        (GenBankMetadata) seq.Metadata["GenBank"];
                    if (metadata.Locus.Strand != SequenceStrandType.None)
                    {
                        Assert.AreEqual(StrandType,
                                        metadata.Locus.Strand.ToString());
                    }
                    Assert.AreEqual(StrandTopology.ToUpper(CultureInfo.CurrentCulture),
                                    metadata.Locus.StrandTopology.ToString().ToUpper(CultureInfo.CurrentCulture));
                    Assert.AreEqual(Div, metadata.Locus.DivisionCode.ToString());
                    Assert.AreEqual(DateTime.Parse(SequenceDate, null),
                                    metadata.Locus.Date);
                    Assert.AreEqual(Version, metadata.Version.Version.ToString(null));
                    Assert.AreEqual(PrimaryId, metadata.Version.GiNumber);
                    ApplicationLog.WriteLine(
                        "GenBank Formatter BVT: Successfully validated the StrandType, StrandTopology, Division, Date, Version, PrimaryID Properties");

                    // test the sequence string
                    Assert.AreEqual(ExpectedSequence, new string(seq.Select(a => (char) a).ToArray()));

                    ApplicationLog.WriteLine(
                        "GenBank Formatter BVT: Successfully validated the Sequence");
                    File.Delete(tempFileName);
                }
            }
        }

        /// <summary>
        ///     Write a valid Sequence (Small size sequence  less than 35 kb) to a
        ///     GenBank file using GenBankFormatter() through a Stream  and
        ///     validate the same.
        ///     Input : GenBank Sequence
        ///     Validation :  Read the GenBank file to which the sequence was formatted
        ///     using File-Info and Validate Properties like StrandType, StrandTopology,
        ///     Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void GenBankFormatterValidateWriteUsingStream()
        {
            InitializeXmlVariables();

            // Create a Sequence with all attributes.
            // Parse and update the properties instead of parsing entire file.            
            ISequenceParser parser1 = new GenBankParser();
            {
                IEnumerable<ISequence> seqList1 = parser1.Parse(FilePath);
                string tempFileName = Path.GetTempFileName();
                GenBankMetadata metadata = null;
                ISequence seq = null;
                string expectedUpdatedSequence =
                    ExpectedSequence.Replace("\r", "").Replace("\n", "").Replace(" ", "");
                var orgSeq =
                    new Sequence(Utility.GetAlphabet(AlphabetName), expectedUpdatedSequence);
                orgSeq.Metadata.Add("GenBank",
                                    seqList1.ElementAt(0).Metadata["GenBank"]);
                orgSeq.ID = seqList1.ElementAt(0).ID;

                ISequenceFormatter formatter = new GenBankFormatter();
                {
                    using (formatter.Open(tempFileName))
                    {
                        formatter.Format(orgSeq);
                    }
                }

                var parserObj = new GenBankParser();
                {
                    IEnumerable<ISequence> seqList = parserObj.Parse(tempFileName);
                    seq = seqList.ElementAt(0);
                    Assert.AreEqual(Utility.GetAlphabet(AlphabetName), seq.Alphabet);
                    Assert.AreEqual(SeqId, seq.ID);
                    ApplicationLog.WriteLine(
                        "GenBank Formatter BVT: Successfully validated the Alphabet, Molecular type, Sequence ID and Display ID");

                    // test the metadata that is tricky to parse, and will not be tested implicitly by
                    // testing the formatting 
                    metadata = (GenBankMetadata) seq.Metadata["GenBank"];
                    if (metadata.Locus.Strand != SequenceStrandType.None)
                    {
                        Assert.AreEqual(StrandType, metadata.Locus.Strand.ToString());
                    }
                }
                Assert.AreEqual(StrandTopology.ToUpper(CultureInfo.CurrentCulture),
                                metadata.Locus.StrandTopology.ToString().ToUpper(CultureInfo.CurrentCulture));
                Assert.AreEqual(Div, metadata.Locus.DivisionCode.ToString());
                Assert.AreEqual(DateTime.Parse(SequenceDate, null), metadata.Locus.Date);
                Assert.AreEqual(Version, metadata.Version.Version.ToString(null));
                Assert.AreEqual(PrimaryId, metadata.Version.GiNumber);
                ApplicationLog.WriteLine(
                    "GenBank Formatter BVT: Successfully validated the StrandType, StrandTopology, Division, Date, Version, PrimaryID Properties");

                // test the sequence string            
                Assert.AreEqual(ExpectedSequence, new string(seq.Select(a => (char) a).ToArray()));
                ApplicationLog.WriteLine("GenBank Formatter BVT: Successfully validated the Sequence");
                File.Delete(tempFileName);
            }
        }

        #endregion GenBank Formatter BVT Test cases

        #region Helper Methods

        /// <summary>
        ///     Initializes Xml Variables
        /// </summary>
        private void InitializeXmlVariables()
        {
            // Initialization of xml strings.
            FilePath = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankNodeName, Constants.FilePathNode);
            AlphabetName = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankNodeName, Constants.AlphabetNameNode);
            SeqId = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankNodeName, Constants.SequenceIdNode);
            StrandTopology = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankNodeName, Constants.StrandTopologyNode);
            StrandType = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankNodeName, Constants.StrandTypeNode);
            Div = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankNodeName, Constants.DivisionNode);
            Version = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankNodeName, Constants.VersionNode);
            SequenceDate = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankNodeName, Constants.DateNode);
            PrimaryId = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankNodeName, Constants.PrimaryIdNode);
            ExpectedSequence = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankNodeName, Constants.ExpectedSequenceNode);
        }

        #endregion Helper Methods
    }
}