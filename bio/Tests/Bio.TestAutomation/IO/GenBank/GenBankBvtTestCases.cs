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
using Bio.IO;
using Bio.IO.GenBank;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio;

#if (SILVERLIGHT == false)
    namespace Bio.TestAutomation.IO.GenBank
#else
    namespace Bio.Silverlight.TestAutomation.IO.GenBank
#endif
{
    /// <summary>
    /// GenBank Bvt parser and formatter Test case implementation.
    /// </summary>
    [TestClass]
    public class GenBankBvtTestCases
    {

        #region Global Variables

        // Global variables which store the information of xml file values and is used across the class file.
        static string _filepath;
        static string _alpName;
        static string _seqId;
        static string _strTopo;
        static string _strType;
        static string _div;
        static string _version;
        static string _date;
        static string _primId;
        static string _expSeq;

        Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

        #region Properties

        static string AlphabetName
        {
            get { return GenBankBvtTestCases._alpName; }
            set { GenBankBvtTestCases._alpName = value; }
        }

        static string FilePath
        {
            get { return GenBankBvtTestCases._filepath; }
            set { GenBankBvtTestCases._filepath = value; }
        }

        static string SeqId
        {
            get { return GenBankBvtTestCases._seqId; }
            set { GenBankBvtTestCases._seqId = value; }
        }

        static string StrandTopology
        {
            get { return GenBankBvtTestCases._strTopo; }
            set { GenBankBvtTestCases._strTopo = value; }
        }

        static string StrandType
        {
            get { return GenBankBvtTestCases._strType; }
            set { GenBankBvtTestCases._strType = value; }
        }

        static string Div
        {
            get { return GenBankBvtTestCases._div; }
            set { GenBankBvtTestCases._div = value; }
        }

        static string Version
        {
            get { return GenBankBvtTestCases._version; }
            set { GenBankBvtTestCases._version = value; }
        }

        static string SequenceDate
        {
            get { return GenBankBvtTestCases._date; }
            set { GenBankBvtTestCases._date = value; }
        }

        static string PrimaryId
        {
            get { return GenBankBvtTestCases._primId; }
            set { GenBankBvtTestCases._primId = value; }
        }

        static string ExpectedSequence
        {
            get { return GenBankBvtTestCases._expSeq; }
            set { GenBankBvtTestCases._expSeq = value; }
        }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static GenBankBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region GenBank Parser BVT Test cases

        /// <summary>
        /// Parse a valid GenBank file (Small size sequence less than 35 kb) and 
        /// convert the same to one sequence using Parse(file-name) method and 
        /// validate with the expected sequence.
        /// Input : GenBank File
        /// Validation: Properties like StrandType, StrandTopology, Division, Date, 
        /// Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void GenBankParserValidateParseFileName()
        {
            InitializeXmlVariables();

            // parse            
            using (ISequenceParser parserObj = new GenBankParser(FilePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();
                ISequence seq = seqList.ElementAt(0);
                Assert.AreEqual(Utility.GetAlphabet(AlphabetName), seq.Alphabet);
                Assert.AreEqual(SeqId, seq.ID);
                ApplicationLog.WriteLine(
                    "GenBank Parser BVT: Successfully validated the Alphabet, Molecular type, Sequence ID and Display ID");

                // test the metadata that is tricky to parse, and will not be tested implicitly by
                // testing the formatting
                GenBankMetadata metadata = (GenBankMetadata)seq.Metadata["GenBank"];
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

                Assert.AreEqual(Version, metadata.Version.Version.ToString((IFormatProvider)null));
                Assert.AreEqual(PrimaryId, metadata.Version.GiNumber);
                ApplicationLog.WriteLine(
                    "GenBank Parser BVT: Successfully validated the StrandType, StrandTopology, Division, Date, Version, PrimaryID Properties");

                // test the sequence string            
                Assert.AreEqual(ExpectedSequence, new string(seq.Select(a => (char)a).ToArray()));

                ApplicationLog.WriteLine(
                    "GenBank Parser BVT: Successfully validated the Sequence");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Parser BVT: Successfully validated the Sequence '{0}'",
                    ExpectedSequence));
            }
        }

        /// <summary>
        /// Parse a valid GenBank file (Small size sequence less than 35 kb) and 
        /// convert the same to one sequence using Parse(Stream) method and 
        /// validate with the expected sequence.
        /// Input : GenBank File
        /// Validation: Properties like StrandType, StrandTopology, Division, Date, 
        /// Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void GenBankParserValidateParseFileNameWithStream()
        {
            InitializeXmlVariables();            
            List<ISequence> seq=null;   
            IEnumerable<ISequence> seqList=null;

            // Parse the Stream.           
            using (ISequenceParser parserObj = new GenBankParser())
            {
                using (StreamReader reader = new StreamReader(FilePath))
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
                GenBankMetadata metadata = (GenBankMetadata)seq[0].Metadata["GenBank"];
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

                Assert.AreEqual(Version, metadata.Version.Version.ToString((IFormatProvider)null));
                Assert.AreEqual(PrimaryId, metadata.Version.GiNumber);
                ApplicationLog.WriteLine(
                    "GenBank Parser BVT: Successfully validated the StrandType, StrandTopology, Division, Date, Version, PrimaryID Properties");

                // test the sequence string            
                Assert.AreEqual(ExpectedSequence, new string(seq[0].Select(a => (char)a).ToArray()));

                ApplicationLog.WriteLine(
                    "GenBank Parser BVT: Successfully validated the Sequence");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Parser BVT: Successfully validated the Sequence '{0}'",
                    ExpectedSequence));
            }
        }

        /// <summary>
        /// Parse a valid GenBank file (Small size sequence less than 35 kb) and 
        /// convert the same to one sequence using ParseOne(file-name) method and 
        /// set Alphabet and Encoding value and validate with the expected sequence.
        /// Input : GenBank File
        /// Output : Properties like StrandType, StrandTopology, Division, Date, 
        /// Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
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
            using (ISequenceParser parserObj = new GenBankParser(FilePath))
            {
                parserObj.Alphabet = Alphabets.Protein;
                IEnumerable<ISequence> seq = parserObj.Parse();

                Assert.AreEqual(Utility.GetAlphabet(AlphabetName),
                    seq.ElementAt(0).Alphabet);
                Assert.AreEqual(SeqId, seq.ElementAt(0).ID);
                ApplicationLog.WriteLine(
                    "GenBank Parser BVT: Successfully validated the Alphabet, Molecular type, Sequence ID and Display ID");

                // test the metadata that is tricky to parse, and will not be tested implicitly by
                // testing the formatting
                GenBankMetadata metadata = (GenBankMetadata)seq.ElementAt(0).Metadata["GenBank"];
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
                Assert.AreEqual(Version, metadata.Version.Version.ToString((IFormatProvider)null));
                Assert.AreEqual(PrimaryId, metadata.Version.GiNumber);
                ApplicationLog.WriteLine(
                    "GenBank Parser BVT: Successfully validated the StrandType, StrandTopology, Division, Date, Version, PrimaryID Properties");

                // test the sequence string            
                Assert.AreEqual(ExpectedSequence, new string(seq.ElementAt(0).Select(a => (char)a).ToArray()));
                ApplicationLog.WriteLine(
                    "GenBank Parser BVT: Successfully validated the Sequence");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Parser BVT: Successfully validated the Sequence '{0}'",
                    ExpectedSequence));
            }
        }

        #endregion GenBank Parser BVT Test cases

        #region GenBank Formatter BVT Test cases

        /// <summary>
        /// Write a valid Sequence (Small size sequence  less than 35 kb) to a 
        /// GenBank file using GenBankFormatter(File-Info) constructor and 
        /// validate the same.
        /// Input : GenBank Sequence
        /// Validation :  Read the GenBank file to which the sequence was formatted 
        /// using File-Info and Validate Properties like StrandType, StrandTopology,
        /// Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void GenBankFormatterValidateWrite()
        {
            InitializeXmlVariables();

            // Create a Sequence with all attributes.
            // parse and update the properties instead of parsing entire file.            
            using (ISequenceParser parser1 = new GenBankParser(FilePath))
            {
                IEnumerable<ISequence> seqList1 = parser1.Parse();
                string tempFileName = System.IO.Path.GetTempFileName();

                string expectedUpdatedSequence =
                    ExpectedSequence.Replace("\r", "").Replace("\n", "").Replace(" ", "");
                Sequence orgSeq =
                     new Sequence(Utility.GetAlphabet(AlphabetName), expectedUpdatedSequence);
                orgSeq.Metadata.Add("GenBank",
          (GenBankMetadata)seqList1.ElementAt(0).Metadata["GenBank"]);
                orgSeq.ID = seqList1.ElementAt(0).ID;

                using (ISequenceFormatter formatter = new GenBankFormatter(tempFileName))
                {
                    formatter.Write(orgSeq);
                    formatter.Close();

                    // parse            
                    GenBankParser parserObj = new GenBankParser(tempFileName);

                    IEnumerable<ISequence> seqList = parserObj.Parse();
                    ISequence seq = seqList.ElementAt(0);
                    Assert.AreEqual(Utility.GetAlphabet(AlphabetName), seq.Alphabet);
                    Assert.AreEqual(SeqId, seq.ID);
                    ApplicationLog.WriteLine(
                    "GenBank Formatter BVT: Successfully validated the Alphabet, Molecular type, Sequence ID and Display ID");

                    // test the metadata that is tricky to parse, and will not be tested implicitly by
                    // testing the formatting 
                    GenBankMetadata metadata = (GenBankMetadata)seq.Metadata["GenBank"];
                    if (metadata.Locus.Strand != SequenceStrandType.None)
                    {
                        Assert.AreEqual(StrandType, metadata.Locus.Strand.ToString());
                    }
                    Assert.AreEqual(StrandTopology.ToUpper(CultureInfo.CurrentCulture), metadata.Locus.StrandTopology.ToString().ToUpper(CultureInfo.CurrentCulture));
                    Assert.AreEqual(Div, metadata.Locus.DivisionCode.ToString());
                    Assert.AreEqual(DateTime.Parse(SequenceDate, null), metadata.Locus.Date);
                    Assert.AreEqual(Version, metadata.Version.Version.ToString((IFormatProvider)null));
                    Assert.AreEqual(PrimaryId, metadata.Version.GiNumber);
                    ApplicationLog.WriteLine(
                    "GenBank Formatter BVT: Successfully validated the StrandType, StrandTopology, Division, Date, Version, PrimaryID Properties");

                    // test the sequence string            
                    Assert.AreEqual(ExpectedSequence, new string(seq.Select(a => (char)a).ToArray()));
                    ApplicationLog.WriteLine("GenBank Formatter BVT: Successfully validated the Sequence");
                    Console.WriteLine(string.Format((IFormatProvider)null, "GenBank Formatter BVT: Successfully validated the Sequence '{0}'", ExpectedSequence));
                    parserObj.Close();
                    parserObj.Dispose();
                    File.Delete(tempFileName);
                }
            }
        }

        /// <summary>
        /// Write a valid Sequence (Small size sequence  less than 35 kb) to a 
        /// GenBank file using GenBankFormatter(File-Path) constructor and 
        /// validate the same.
        /// Input : GenBank Sequence
        /// Validation :  Read the GenBank file to which the sequence was formatted 
        /// using File-Path and Validate Properties like StrandType, StrandTopology,
        /// Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void GenBankFormatterValidateWriteWithFilePath()
        {
            InitializeXmlVariables();
            using (ISequenceParser parserObj = new GenBankParser(FilePath))
            {
                IEnumerable<ISequence> seqList1 = parserObj.Parse();
                string tempFileName = System.IO.Path.GetTempFileName();
                string expectedUpdatedSequence =
                    ExpectedSequence.Replace("\r", "").Replace("\n", "").Replace(" ", "");
                Sequence orgSeq = new Sequence(Utility.GetAlphabet(AlphabetName), expectedUpdatedSequence);
                orgSeq.ID = seqList1.ElementAt(0).ID;
                orgSeq.Metadata.Add("GenBank", (GenBankMetadata)seqList1.ElementAt(0).Metadata["GenBank"]);
                using (ISequenceFormatter formatter = new GenBankFormatter(tempFileName))
                {
                    formatter.Write(orgSeq);
                    formatter.Close();

                    // parse
                    ISequenceParser parserObjFromFile = new GenBankParser(tempFileName);
                    IEnumerable<ISequence> seqList =
                        parserObjFromFile.Parse();
                    ISequence seq = seqList.ElementAt(0);
                    Assert.AreEqual(Utility.GetAlphabet(AlphabetName), seq.Alphabet);
                    Assert.AreEqual(SeqId, seq.ID);
                    ApplicationLog.WriteLine(
                        "GenBank Formatter BVT: Successfully validated the Alphabet, Molecular type, Sequence ID and Display ID");

                    // test the metadata that is tricky to parse, and will not be tested implicitly by
                    // testing the formatting
                    GenBankMetadata metadata =
                        (GenBankMetadata)orgSeq.Metadata["GenBank"];
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
                    Assert.AreEqual(Version, metadata.Version.Version.ToString((IFormatProvider)null));
                    Assert.AreEqual(PrimaryId, metadata.Version.GiNumber);
                    ApplicationLog.WriteLine(
                        "GenBank Formatter BVT: Successfully validated the StrandType, StrandTopology, Division, Date, Version, PrimaryID Properties");

                    // test the sequence string            
                    Assert.AreEqual(ExpectedSequence, new string(seq.Select(a => (char)a).ToArray()));
                    ApplicationLog.WriteLine(
                        "GenBank Formatter BVT: Successfully validated the Sequence");
                    Console.WriteLine(string.Format((IFormatProvider)null,
                        "GenBank Formatter BVT: Successfully validated the Sequence '{0}'",
                        ExpectedSequence));
                    parserObjFromFile.Close();
                    parserObjFromFile.Dispose();
                    File.Delete(tempFileName);
                }
            }
        }

        /// <summary>
        /// Parse a GenBank File (Small size sequence less than 35 kb) using Parse() 
        /// method and Format the same to a GenBank file using GenBankFormatter(File-Info) 
        /// constructor and validate the same.
        /// Input : GenBank File
        /// Validation :  Read the New GenBank file to which the sequence was formatted 
        /// using File-Info and Validate Properties like StrandType, StrandTopology,
        /// Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void GenBankFormatterWithParseValidateWrite()
        {
            InitializeXmlVariables();
            // parse
            ISequenceParser parserObj = new GenBankParser(FilePath);

            IEnumerable<ISequence> seqList = parserObj.Parse();
            string tempFileName = System.IO.Path.GetTempFileName();
            ISequence seq = seqList.ElementAt(0);

            using (ISequenceFormatter formatter = new GenBankFormatter(tempFileName))
            {
                formatter.Write(seq);
                formatter.Close();

                // parse
                parserObj = new GenBankParser(tempFileName);
                seqList = parserObj.Parse();
                seq = seqList.ElementAt(0);
                Assert.AreEqual(Utility.GetAlphabet(AlphabetName), seq.Alphabet);
                Assert.AreEqual(SeqId, seq.ID);
                ApplicationLog.WriteLine(
                    "GenBank Formatter BVT: Successfully validated the Alphabet, Molecular type, Sequence ID and Display ID");

                // test the metadata that is tricky to parse, and will not be tested implicitly by
                // testing the formatting
                GenBankMetadata metadata = (GenBankMetadata)seq.Metadata["GenBank"];
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
                Assert.AreEqual(Version, metadata.Version.Version.ToString((IFormatProvider)null));
                Assert.AreEqual(PrimaryId, metadata.Version.GiNumber);
                ApplicationLog.WriteLine(
                    "GenBank Formatter BVT: Successfully validated the StrandType, StrandTopology, Division, Date, Version, PrimaryID Properties");

                // test the sequence string            
                Assert.AreEqual(ExpectedSequence, new string(seq.Select(a => (char)a).ToArray()));
                ApplicationLog.WriteLine(
                    "GenBank Formatter BVT: Successfully validated the Sequence");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Formatter BVT: Successfully validated the Sequence '{0}'",
                    ExpectedSequence));
                parserObj.Close();
                parserObj.Dispose();
                File.Delete(tempFileName);
            }
        }

        /// <summary>
        /// Parse a GenBank File (Small size sequence less than 35 kb) using Parse() 
        /// method and Write the same to a GenBank file using 
        /// GenBankFormatter(File-Path) constructor and validate the same.
        /// Input : GenBank File
        /// Validation :  Read the New GenBank file to which the sequence was formatted 
        /// using File-Path and Validate Properties like StrandType, StrandTopology,
        /// Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void GenBankFormatterWithParseValidateWriteFilePath()
        {
            InitializeXmlVariables();
            // parse
            using (ISequenceParser parserObj = new GenBankParser(FilePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();
                ISequence seq = seqList.ElementAt(0);
                string tempFileName = System.IO.Path.GetTempFileName();
                using (ISequenceFormatter formatter = new GenBankFormatter(tempFileName))
                {
                    formatter.Write(seq);
                    formatter.Close();

                    // parse
                    ISequenceParser parserObjFromFile = new GenBankParser(tempFileName);
                    seqList = parserObjFromFile.Parse();
                    seq = seqList.ElementAt(0);
                    Assert.AreEqual(Utility.GetAlphabet(AlphabetName), seq.Alphabet);
                    Assert.AreEqual(SeqId, seq.ID);
                    ApplicationLog.WriteLine(
                        "GenBank Formatter BVT: Successfully validated the Alphabet, Molecular type, Sequence ID and Display ID");

                    // test the metadata that is tricky to parse, and will not be tested implicitly by
                    // testing the formatting
                    GenBankMetadata metadata =
                        (GenBankMetadata)seq.Metadata["GenBank"];
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
                    Assert.AreEqual(Version, metadata.Version.Version.ToString((IFormatProvider)null));
                    Assert.AreEqual(PrimaryId, metadata.Version.GiNumber);
                    ApplicationLog.WriteLine(
                        "GenBank Formatter BVT: Successfully validated the StrandType, StrandTopology, Division, Date, Version, PrimaryID Properties");

                    // test the sequence string
                    Assert.AreEqual(ExpectedSequence, new string(seq.Select(a => (char)a).ToArray()));

                    ApplicationLog.WriteLine(
                        "GenBank Formatter BVT: Successfully validated the Sequence");
                    Console.WriteLine(string.Format((IFormatProvider)null,
                        "GenBank Formatter BVT: Successfully validated the Sequence '{0}'",
                        ExpectedSequence));
                    parserObjFromFile.Close();
                    parserObjFromFile.Dispose();
                    File.Delete(tempFileName);

                }
            }
        }

        /// <summary>
        /// Write a valid Sequence (Small size sequence  less than 35 kb) to a 
        /// GenBank file using GenBankFormatter() through a Stream  and 
        /// validate the same.
        /// Input : GenBank Sequence
        /// Validation :  Read the GenBank file to which the sequence was formatted 
        /// using File-Info and Validate Properties like StrandType, StrandTopology,
        /// Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void GenBankFormatterValidateWriteUsingStream()
        {
            InitializeXmlVariables();

            // Create a Sequence with all attributes.
            // Parse and update the properties instead of parsing entire file.            
            using (ISequenceParser parser1 = new GenBankParser(FilePath))
            {               
                IEnumerable<ISequence> seqList1 = parser1.Parse();
                string tempFileName = System.IO.Path.GetTempFileName();
                GenBankMetadata metadata = null;
                ISequence seq =null;
                string expectedUpdatedSequence =
                    ExpectedSequence.Replace("\r", "").Replace("\n", "").Replace(" ", "");
                Sequence orgSeq =
                     new Sequence(Utility.GetAlphabet(AlphabetName), expectedUpdatedSequence);
                orgSeq.Metadata.Add("GenBank",
                (GenBankMetadata)seqList1.ElementAt(0).Metadata["GenBank"]);
                orgSeq.ID = seqList1.ElementAt(0).ID;

                using (ISequenceFormatter formatter = new GenBankFormatter())
                {
                    using (StreamWriter writer = new StreamWriter(tempFileName))
                    {
                        formatter.Open(writer);
                        formatter.Write(orgSeq);                        
                    }
                }
                    using (GenBankParser parserObj = new GenBankParser(tempFileName))
                    {
                        IEnumerable<ISequence> seqList = parserObj.Parse();
                        seq= seqList.ElementAt(0);
                        Assert.AreEqual(Utility.GetAlphabet(AlphabetName), seq.Alphabet);
                        Assert.AreEqual(SeqId, seq.ID);
                        ApplicationLog.WriteLine(
                        "GenBank Formatter BVT: Successfully validated the Alphabet, Molecular type, Sequence ID and Display ID");

                        // test the metadata that is tricky to parse, and will not be tested implicitly by
                        // testing the formatting 
                        metadata = (GenBankMetadata)seq.Metadata["GenBank"];
                        if (metadata.Locus.Strand != SequenceStrandType.None)
                        {
                            Assert.AreEqual(StrandType, metadata.Locus.Strand.ToString());
                        }
                    }
                    Assert.AreEqual(StrandTopology.ToUpper(CultureInfo.CurrentCulture), metadata.Locus.StrandTopology.ToString().ToUpper(CultureInfo.CurrentCulture));
                    Assert.AreEqual(Div, metadata.Locus.DivisionCode.ToString());
                    Assert.AreEqual(DateTime.Parse(SequenceDate, null), metadata.Locus.Date);
                    Assert.AreEqual(Version, metadata.Version.Version.ToString((IFormatProvider)null));
                    Assert.AreEqual(PrimaryId, metadata.Version.GiNumber);
                    ApplicationLog.WriteLine(
                    "GenBank Formatter BVT: Successfully validated the StrandType, StrandTopology, Division, Date, Version, PrimaryID Properties");

                    // test the sequence string            
                    Assert.AreEqual(ExpectedSequence, new string(seq.Select(a => (char)a).ToArray()));
                    ApplicationLog.WriteLine("GenBank Formatter BVT: Successfully validated the Sequence");
                    Console.WriteLine(string.Format((IFormatProvider)null, "GenBank Formatter BVT: Successfully validated the Sequence '{0}'", ExpectedSequence));                    
                    File.Delete(tempFileName);                
            }
        }

        #endregion GenBank Formatter BVT Test cases

        #region Helper Methods

        /// <summary>
        /// Initializes Xml Variables
        /// </summary>
        void InitializeXmlVariables()
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
