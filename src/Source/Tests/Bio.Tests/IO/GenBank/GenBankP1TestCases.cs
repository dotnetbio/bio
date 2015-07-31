/****************************************************************************
 * GenBankP1TestCases.cs
 * 
 *   This file contains the GenBank - Parsers and Formatters Priority One test cases.
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
using NUnit.Framework;
using Bio;

#if (SILVERLIGHT == false)
    namespace Bio.TestAutomation.IO.GenBank
#else
    namespace Bio.Silverlight.TestAutomation.IO.GenBank
#endif
{
    /// <summary>
    /// GenBank Priority One parser and formatter test cases implementation.
    /// </summary>
    [TestFixture]
    public class GenBankP1TestCases
    {

        #region Global Variables

        // Global variables which store the information of xml 
        // file values and is used across the class file.
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

        #endregion Global Variables

        #region Properties

        static string AlphabetName
        {
            get { return GenBankP1TestCases._alpName; }
            set { GenBankP1TestCases._alpName = value; }
        }

        static string FilePath
        {
            get { return GenBankP1TestCases._filepath; }
            set { GenBankP1TestCases._filepath = value; }
        }

        static string SeqId
        {
            get { return GenBankP1TestCases._seqId; }
            set { GenBankP1TestCases._seqId = value; }
        }

        static string StrandTopology
        {
            get { return GenBankP1TestCases._strTopo; }
            set { GenBankP1TestCases._strTopo = value; }
        }

        static string StrandType
        {
            get { return GenBankP1TestCases._strType; }
            set { GenBankP1TestCases._strType = value; }
        }

        static string Div
        {
            get { return GenBankP1TestCases._div; }
            set { GenBankP1TestCases._div = value; }
        }

        static string Version
        {
            get { return GenBankP1TestCases._version; }
            set { GenBankP1TestCases._version = value; }
        }

        static string SequenceDate
        {
            get { return GenBankP1TestCases._date; }
            set { GenBankP1TestCases._date = value; }
        }

        static string PrimaryId
        {
            get { return GenBankP1TestCases._primId; }
            set { GenBankP1TestCases._primId = value; }
        }

        static string ExpectedSequence
        {
            get { return GenBankP1TestCases._expSeq; }
            set { GenBankP1TestCases._expSeq = value; }
        }

        #endregion Properties

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

        #region GenBank Parser P1 Test cases

        /// <summary>
        /// Parse a valid GenBank file (Medium size sequence less than 100 Kb) and 
        /// convert the same to one sequence using Parse(file-name) method and 
        /// validate with the expected sequence.
        /// Input : GenBank File with size less than 100 Kb
        /// Validation: Properties like StrandType, StrandTopology, Division, Date, 
        /// Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankParserValidateParseMediumSize()
        {
            InitializeXmlVariables(Constants.MediumSizeGenBankNodeName);
            ValidateParserGeneralTestCases();
        }

        /// <summary>
        /// Parse a valid GenBank file (One line Sequence) and 
        /// convert the same to one sequence using Parse(file-name) method and 
        /// validate with the expected sequence.
        /// Input : GenBank File with one line sequence
        /// Validation: Properties like StrandType, StrandTopology, Division, Date, 
        /// Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankParserValidateParseOneLineSeq()
        {
            InitializeXmlVariables(Constants.OneLineSequenceGenBankNodeName);
            ValidateParserGeneralTestCases();
        }

        /// <summary>
        /// Parse a valid GenBank file with DNA Sequence and 
        /// convert the same to one sequence using Parse(file-name) method and 
        /// validate with the expected sequence.
        /// Input : GenBank File with DNA sequence
        /// Validation: Properties like StrandType, StrandTopology, Division, Date, 
        /// Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankParserValidateParseDnaSequence()
        {
            InitializeXmlVariables(Constants.SimpleGenBankDnaNodeName);
            ValidateParserGeneralTestCases();
        }

        /// <summary>
        /// Parse a valid GenBank file with RNA Sequence and 
        /// convert the same to one sequence using Parse(file-name) method and 
        /// validate with the expected sequence.
        /// Input : GenBank File with RNA sequence
        /// Validation: Properties like StrandType, StrandTopology, Division, Date, 
        /// Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankParserValidateParseRnaSequence()
        {
            InitializeXmlVariables(Constants.SimpleGenBankRnaNodeName);
            ValidateParserGeneralTestCases();
        }

        /// <summary>
        /// Parse a valid GenBank file with Protein Sequence and 
        /// convert the same to one sequence using Parse(file-name) method and 
        /// validate with the expected sequence.
        /// Input : GenBank File with Protein sequence
        /// Validation: Properties like StrandType, StrandTopology, Division, Date, 
        /// Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankParserValidateParseProteinSequence()
        {
            InitializeXmlVariables(Constants.SimpleGenBankProNodeName);
            ValidateParserGeneralTestCases();
        }

        /// <summary>
        /// Parse a valid GenBank file with mandatory headers and 
        /// convert the same to one sequence using Parse(file-name) method and 
        /// validate with the expected sequence.
        /// Input : GenBank File with mandatory headers
        /// Validation: Properties like StrandType, StrandTopology, Division, Date, 
        /// Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankParserValidateParseMandatoryHeaders()
        {
            InitializeXmlVariables(Constants.MandatoryGenBankHeadersNodeName);
            ValidateParserGeneralTestCases();
        }

        /// <summary>
        /// Parse a valid GenBank large file and 
        /// convert the same to one sequence using Parse(file-name) method and 
        /// validate with the expected sequence.
        /// Input : GenBank large files 
        /// Validation: Properties like StrandType, StrandTopology, Division, Date, 
        /// Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankParserValidateParseLargeSizeSequence()
        {
            InitializeXmlVariables(Constants.LargeSizeGenBank);
            ValidateParserGeneralTestCases();
        }

        /// <summary>
        /// Parse a valid GenBank file with two sequences and 
        /// convert the same to one sequence using Parse(file-name) method and 
        /// validate with the expected sequence.
        /// Input : GenBank large files 
        /// Validation: Properties like StrandType, StrandTopology, Division, Date, 
        /// Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankParserValidateParseMultiSequence()
        {
            ValidateParseMultiSeqTestCases(Constants.MultipleSequenceGenBankNodeName);
        }

        /// <summary>
        /// Parse a valid Simple GenBank file Sequence with Encoding 
        /// Passed as a constructor and convert the same to one 
        /// sequence using Parse(file-name) method and 
        /// validate with the expected sequence.
        /// Input : GenBank File with encoding passed as constructor
        /// Validation: Properties like StrandType, StrandTopology, Division, Date, 
        /// Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankParserValidateParseWithEncodingAsConstructor()
        {
            InitializeXmlVariables(Constants.SimpleGenBankDnaNodeName);
            ValidateParserSpecialTestCases();
        }

        /// <summary>
        /// Parse a valid Simple GenBank file Sequence with Encoding 
        /// Passed as a property and convert the same to one 
        /// sequence using Parse(file-name) method and 
        /// validate with the expected sequence.
        /// Input : GenBank File with encoding passed as property
        /// Validation: Properties like StrandType, StrandTopology, Division, Date, 
        /// Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankParserValidateParseWithEncodingAsProperty()
        {
            InitializeXmlVariables(Constants.SimpleGenBankDnaNodeName);
            ValidateParserSpecialTestCases();
        }

        /// <summary>
        /// Parse a valid Simple GenBank file Sequence with Alphabet 
        /// Passed as a property and convert the same to one 
        /// sequence using Parse(file-name) method and 
        /// validate with the expected sequence.
        /// Input : GenBank File with Alphabet passed as property
        /// Validation: Properties like StrandType, StrandTopology, Division, Date, 
        /// Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankParserValidateParseWithAlphabetAsProperty()
        {
            InitializeXmlVariables(Constants.SimpleGenBankDnaNodeName);
            // parse
            ISequenceParser parserObj = new GenBankParser();
            parserObj.Alphabet = Utility.GetAlphabet(
                utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankDnaNodeName,
                Constants.AlphabetNameNode));
            ValidateParserSpecialTestCases();
        }

        /// <summary>
        /// Parse a valid GenBank file with multiple references and 
        /// convert the same to one sequence using Parse(file-name) method and 
        /// validate with the expected sequence.
        /// Input : GenBank large files 
        /// Validation: Properties like StrandType, StrandTopology, Division, Date, 
        /// Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankParserValidateParseMultipleReferenceSequence()
        {
            InitializeXmlVariables(Constants.MultipleReferenceGenBankNodeName);
            ValidateParserGeneralTestCases();
        }

        /// <summary>
        /// Parse a valid GenBank file with multiple gene and cds. 
        /// Convert the same to one sequence using Parse(file-name) method and 
        /// validate with the expected sequence.
        /// Input : GenBank large files 
        /// Validation: Properties like StrandType, StrandTopology, Division, Date, 
        /// Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankParserValidateParseMultipleGeneCdsSequence()
        {
            InitializeXmlVariables(Constants.MultipleGeneCDSGenBankNodeName);
            ValidateParserGeneralTestCases();
        }

        /// <summary>
        /// Parse a valid GenBank file using text reader with multiple references and 
        /// convert the same to one sequence using Parse(file-name) method and 
        /// validate with the expected sequence.
        /// Input : GenBank large files 
        /// Validation: Properties like StrandType, StrandTopology, Division, Date, 
        /// Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankParserValidateParseWithTextReaderMultipleReferenceSequence()
        {
            InitializeXmlVariables(Constants.MultipleReferenceGenBankNodeName);
            ValidateParseWithTextReaderTestCases();
        }

        /// <summary>
        /// Parse a valid GenBank file (Medium size sequence greater than 35 kb 
        /// and less than 100 kb) and convert the same to one sequence using 
        /// ParseOne(file-name) method and set Alphabet and Encoding value and 
        /// validate with the expected sequence.
        /// Input : GenBank File
        /// Output : Properties like StrandType, StrandTopology, Division, Date, 
        /// Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankParserValidateParseOneFileNameWithSpecificFormats()
        {
            InitializeXmlVariables(Constants.SimpleGenBankPrimaryNode);

            // parse
            ISequenceParser parserObj = new GenBankParser();
            parserObj.Alphabet = Alphabets.Protein;
            //parserObj.Encoding = NcbiEAAEncoding.Instance;
            IEnumerable<ISequence> seq = parserObj.Parse(FilePath);
            ValidateParserGeneralTestCases(seq.ElementAt(0), ExpectedSequence);
        }

        #endregion GenBank Parser P1 Test cases

        #region GenBank Formatter P1 Test cases

        /// <summary>
        /// Format a valid DNA Sequence to a 
        /// GenBank file using GenBankFormatter() constructor and Format() method 
        /// with sequence and writer as parameters and 
        /// validate the same.
        /// Input : DNA GenBank Sequence
        /// Validation :  Read the GenBank file to which the sequence was formatted 
        /// using File-Path and Validate Properties like StrandType, StrandTopology,
        /// Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankFormatterValidateWriteDnaSequence()
        {
            InitializeXmlVariables(Constants.SimpleGenBankDnaNodeName);
            ValidateWriterGeneralTestCases();
        }

        /// <summary>
        /// Format a valid RNA Sequence to a 
        /// GenBank file using GenBankFormatter() constructor and Format() method 
        /// with sequence and writer as parameters and 
        /// validate the same.
        /// Input : RNA GenBank Sequence
        /// Validation :  Read the GenBank file to which the sequence was formatted 
        /// using File-Path and Validate Properties like StrandType, StrandTopology,
        /// Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankFormatterValidateWriteRnaSequence()
        {
            InitializeXmlVariables(Constants.SimpleGenBankRnaNodeName);
            ValidateWriterGeneralTestCases();
        }

        /// <summary>
        /// Format a valid Protein Sequence to a 
        /// GenBank file using GenBankFormatter() constructor and Format() method 
        /// with sequence and writer as parameters and 
        /// validate the same.
        /// Input : Protein GenBank Sequence
        /// Validation :  Read the GenBank file to which the sequence was formatted 
        /// using File-Path and Validate Properties like StrandType, StrandTopology,
        /// Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankFormatterValidateWriteProteinSequence()
        {
            InitializeXmlVariables(Constants.SimpleGenBankProNodeName);
            ValidateWriterGeneralTestCases();
        }

        /// <summary>
        /// Format a valid Sequence with Mandatory Headers to a 
        /// GenBank file using GenBankFormatter() constructor and Format() method 
        /// with sequence and writer as parameters and 
        /// validate the same.
        /// Input : Protein GenBank Sequence
        /// Validation :  Read the GenBank file to which the sequence was formatted 
        /// using File-Path and Validate Properties like StrandType, StrandTopology,
        /// Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankFormatterValidateWriteMandatoryHeaders()
        {
            InitializeXmlVariables(Constants.MandatoryGenBankHeadersNodeName);
            ValidateWriterGeneralTestCases();
        }

        /// <summary>
        /// Format a valid Medium Size i.e., less than 100 Kb Sequence to a 
        /// GenBank file using GenBankFormatter() constructor and Format() method 
        /// with sequence and writer as parameters and 
        /// validate the same.
        /// Input : GenBank Sequence with less than 100 Kb
        /// Validation :  Read the GenBank file to which the sequence was formatted 
        /// using File-Path and Validate Properties like StrandType, StrandTopology,
        /// Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankFormatterValidateWriteMediumSizeSequence()
        {
            InitializeXmlVariables(Constants.MediumSizeGenBankNodeName);
            ValidateWriterGeneralTestCases();
        }

        /// <summary>
        /// Format a valid Large Size i.e., greater than 100 Kb and less than 350 Kb Sequence to a 
        /// GenBank file using GenBankFormatter() constructor and Format() method 
        /// with sequence and writer as parameters and 
        /// validate the same.
        /// Input : GenBank Sequence with greater than 100 Kb and less than 350 Kb
        /// Validation :  Read the GenBank file to which the sequence was formatted 
        /// using File-Path and Validate Properties like StrandType, StrandTopology,
        /// Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankFormatterValidateWriteLargeSizeSequence()
        {
            InitializeXmlVariables(Constants.LargeSizeGenBank);
            ValidateWriterGeneralTestCases();
        }

        /// <summary>
        /// Format a valid Sequence with multiple reference to a 
        /// GenBank file using GenBankFormatter() constructor and Format() method 
        /// with sequence and writer as parameters and 
        /// validate the same.
        /// Input : GenBank Sequence with greater than 100 Kb and less than 350 Kb
        /// Validation :  Read the GenBank file to which the sequence was formatted 
        /// using File-Path and Validate Properties like StrandType, StrandTopology,
        /// Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankFormatterValidateWriteMultipleReferenceSequence()
        {
            InitializeXmlVariables(Constants.MultipleReferenceGenBankNodeName);
            ValidateWriterGeneralTestCases();
        }

        /// <summary>
        /// Format a valid Sequence with multiple gene and cds to a 
        /// GenBank file using GenBankFormatter() constructor and Format() method 
        /// with sequence and writer as parameters and 
        /// validate the same.
        /// Input : GenBank Sequence with greater than 100 Kb and less than 350 Kb
        /// Validation :  Read the GenBank file to which the sequence was formatted 
        /// using File-Path and Validate Properties like StrandType, StrandTopology,
        /// Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankFormatterValidateWriteMultipleGeneCdsSequence()
        {
            InitializeXmlVariables(Constants.MultipleGeneCDSGenBankNodeName);
            ValidateWriterGeneralTestCases();
        }

        /// <summary>
        /// Parse a valid Medium Size i.e., less than 100 Kb File and Format to a 
        /// GenBank file using GenBankFormatter() constructor and Format() method 
        /// with sequence and writer as parameters and 
        /// validate the same.
        /// Input : GenBank File with less than 100 Kb
        /// Validation :  Read the GenBank file to which the sequence was formatted 
        /// using File-Path and Validate Properties like StrandType, StrandTopology,
        /// Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankFormatterValidateParseWriteMediumSizeSequence()
        {
            InitializeXmlVariables(Constants.MediumSizeGenBankNodeName);
            ValidateParseWriterGeneralTestCases();
        }

        /// <summary>
        /// Parse a valid large Size i.e., greater than 100 Kb and less than 350 Kb File 
        /// and Format to a GenBank file using GenBankFormatter() constructor and Format() method 
        /// with sequence and writer as parameters and 
        /// validate the same.
        /// Input : GenBank File with less than 100 Kb
        /// Validation :  Read the GenBank file to which the sequence was formatted 
        /// using File-Path and Validate Properties like StrandType, StrandTopology,
        /// Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankFormatterValidateParseWriteLargeSizeSequence()
        {
            InitializeXmlVariables(Constants.LargeSizeGenBank);
            ValidateParseWriterGeneralTestCases();
        }

        /// <summary>
        /// Format a valid Sequence to a GenBank file using GenBankFormatter(File-Path) 
        /// constructor and validate the same by parsing back the file.
        /// Input : DNA GenBank Sequence
        /// Validation :  Parse the GenBank file to which the sequence was formatted 
        /// using File-Path and Validate Properties like StrandType, StrandTopology,
        /// Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankFormatterValidateWriteAndParse()
        {
            InitializeXmlVariables(Constants.SimpleGenBankNodeName);
            ValidateWriterGeneralTestCases();
        }

        /// <summary>
        /// Format a valid DNA Sequence to a 
        /// GenBank file using GenBankFormatter() constructor and Format() method 
        /// with sequence and writer as parameters and 
        /// validate the same.
        /// Input : DNA GenBank Sequence
        /// Validation :  Read the GenBank file to which the sequence was formatted 
        /// using textreader and Validate Properties like StrandType, StrandTopology,
        /// Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankFormatterParseWithTextReaderValidateWriteDnaSequence()
        {
            InitializeXmlVariables(Constants.SimpleGenBankDnaNodeName);
            ValidateParseWithTextReaderWriteGeneralTestCases();
        }

        /// <summary>
        /// Format a valid RNA Sequence to a 
        /// GenBank file using GenBankFormatter() constructor and Format() method 
        /// with sequence and writer as parameters and 
        /// validate the same.
        /// Input : RNA GenBank Sequence
        /// Validation :  Read the GenBank file to which the sequence was formatted 
        /// using textreader and Validate Properties like StrandType, StrandTopology,
        /// Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankFormatterParseWithTextReaderValidateWriteRnaSequence()
        {
            InitializeXmlVariables(Constants.SimpleGenBankRnaNodeName);
            ValidateParseWithTextReaderWriteGeneralTestCases();
        }

        /// <summary>
        /// Format a valid Protein Sequence to a 
        /// GenBank file using GenBankFormatter() constructor and Format() method 
        /// with sequence and writer as parameters and 
        /// validate the same.
        /// Input : Protein GenBank Sequence
        /// Validation :  Read the GenBank file to which the sequence was formatted 
        /// using textreader and Validate Properties like StrandType, StrandTopology,
        /// Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankFormatterParseWithTextReaderValidateWriteProteinSequence()
        {
            InitializeXmlVariables(Constants.SimpleGenBankProNodeName);
            ValidateParseWithTextReaderWriteGeneralTestCases();
        }

        /// <summary>
        /// Format a valid Sequence with Mandatory Headers to a 
        /// GenBank file using GenBankFormatter() constructor and Format() method 
        /// with sequence and writer as parameters and 
        /// validate the same.
        /// Input : Protein GenBank Sequence
        /// Validation :  Read the GenBank file to which the sequence was formatted 
        /// using textreader and Validate Properties like StrandType, StrandTopology,
        /// Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankFormatterParseWithTextReaderValidateWriteMandatoryHeaders()
        {
            InitializeXmlVariables(Constants.MandatoryGenBankHeadersNodeName);
            ValidateParseWithTextReaderWriteGeneralTestCases();
        }

        /// <summary>
        /// Format a valid Medium Size i.e., less than 100 Kb Sequence to a 
        /// GenBank file using GenBankFormatter() constructor and Format() method 
        /// with sequence and writer as parameters and 
        /// validate the same.
        /// Input : GenBank Sequence with less than 100 Kb
        /// Validation :  Read the GenBank file to which the sequence was formatted 
        /// using text reader and Validate Properties like StrandType, StrandTopology,
        /// Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankFormatterParseWithTextReaderValidateWriteMediumSizeSequence()
        {
            InitializeXmlVariables(Constants.MediumSizeGenBankNodeName);
            ValidateParseWithTextReaderWriteGeneralTestCases();
        }

        /// <summary>
        /// Format a valid large Size i.e., greater than 100 Kb and less than 350 Kb Sequence to a 
        /// GenBank file using GenBankFormatter() constructor and Format() method 
        /// with sequence and writer as parameters and 
        /// validate the same.
        /// Input : GenBank Sequence with greater than 100 Kb and less than 350 Kb
        /// Validation :  Read the GenBank file to which the sequence was formatted 
        /// using text reader and Validate Properties like StrandType, StrandTopology,
        /// Division, Date, Version, PrimaryID, Sequence, Metadata Count and Sequence ID
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankFormatterParseWithTextReaderValidateWriteLargeSizeSequence()
        {
            InitializeXmlVariables(Constants.LargeSizeGenBank);
            ValidateParseWithTextReaderWriteGeneralTestCases();
        }

        /// <summary>
        /// Format a valid Sequence to a GenBank file with multiple references  
        /// using GenBankFormatter(File-Path) constructor and validate by reading the 
        /// GenBank file to which the sequence was formatted and its properties. 
        /// Input : GenBank file with multiple references
        /// Validation :  Read the GenBank file and validate Properties like StrandType; 
        /// StrandTopology; Division; Date; Version; PrimaryID; Sequence; Metadata Count 
        /// and Sequence ID.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankFormatterValidateWriteWithFilePathMultipleReferenceSequence()
        {
            InitializeXmlVariables(Constants.MultipleReferenceGenBankNodeName);
            ValidateWriteWithFilePathGeneralTestCases();
        }

        /// <summary>
        /// Format a valid Sequence to a GenBank file with multiple gene and cds values 
        /// using GenBankFormatter(File-Path) constructor and validate by reading the 
        /// GenBank file to which the sequence was formatted and its properties. 
        /// Input : GenBank file with multiple gene and cds 
        /// Validation :  Read the GenBank file and validate Properties like StrandType; 
        /// StrandTopology; Division; Date; Version; PrimaryID; Sequence; Metadata Count 
        /// and Sequence ID.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankFormatterValidateWriteWithFilePathMultipleGeneCdsSequence()
        {
            InitializeXmlVariables(Constants.MultipleGeneCDSGenBankNodeName);
            ValidateWriteWithFilePathGeneralTestCases();
        }

        /// <summary>
        /// Format a valid Sequence to a GenBank file with large size sequence 
        /// using GenBankFormatter(File-Path) constructor and validate by reading the 
        /// GenBank file to which the sequence was formatted and its properties. 
        /// Input : GenBank Sequence with greater than 100 Kb and less than 350 Kb
        /// Validation :  Read the GenBank file and validate Properties like StrandType; 
        /// StrandTopology; Division; Date; Version; PrimaryID; Sequence; Metadata Count 
        /// and Sequence ID.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankFormatterValidateWriteWithFilePathLargeSizeSequence()
        {
            InitializeXmlVariables(Constants.LargeSizeGenBank);
            ValidateWriteWithFilePathGeneralTestCases();
        }

        /// <summary>
        /// Format a valid Sequence to a GenBank file  using GenBankFormatter(File-Path) constructor 
        /// and validate by reading the GenBank file to which the sequence was formatted and its properties. 
        /// Input : GenBank Sequence with greater than 100 Kb and less than 350 Kb
        /// Validation :  Read the GenBank file and validate Properties like StrandType; 
        /// StrandTopology; Division; Date; Version; PrimaryID; Sequence; Metadata Count 
        /// and Sequence ID.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void GenBankFormatterValidateWriteWithFilePathSimpleSequence()
        {
            InitializeXmlVariables(Constants.SimpleGenBankNodeName);
            ValidateWriteWithFilePathGeneralTestCases();
        }

        #endregion GenBank Formatter P1 Test cases

        #region Supporting Methods

        /// <summary>
        /// Initializes xml variables for the xml node name specified.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        void InitializeXmlVariables(string nodeName)
        {
            FilePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            AlphabetName = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlphabetNameNode);
            SeqId = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.SequenceIdNode);
            StrandTopology = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.StrandTopologyNode);
            StrandType = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.StrandTypeNode);
            Div = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DivisionNode);
            Version = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.VersionNode);
            SequenceDate = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DateNode);
            PrimaryId = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.PrimaryIdNode);
            ExpectedSequence = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequenceNode);
        }

        /// <summary>
        /// Validates GenBank Parser for General test cases.
        /// </summary>
        private static void ValidateParserGeneralTestCases()
        {
            ValidateParserSpecialTestCases();
        }

        /// <summary>
        /// Validates GenBank Parser for specific test cases
        /// which takes ISequenceParser as input.
        /// <param name="parser">ISequenceParser object.</param>
        /// </summary>
        //private static void ValidateParserSpecialTestCases(ISequenceParser parserObj)
        private static void ValidateParserSpecialTestCases()
        {
            ISequenceParser parserObj = new GenBankParser();
            {
                Assert.IsTrue(File.Exists(FilePath));
                // Logs information to the log file
                ApplicationLog.WriteLine(string.Format("GenBank Parser : File Exists in the Path '{0}'.",
                    FilePath));
                IEnumerable<ISequence> seqList = parserObj.Parse(FilePath);
                ISequence seq = seqList.ElementAt(0);
                Assert.AreEqual(Utility.GetAlphabet(AlphabetName), seq.Alphabet);
                Assert.AreEqual(SeqId, seq.ID);

                ApplicationLog.WriteLine(
                    "GenBank Parser : Successfully validated the Alphabet, Molecular type, Sequence ID and Display ID");

                // test the metadata that is tricky to parse, and will not be tested implicitly by
                // testing the formatting
                GenBankMetadata metadata = (GenBankMetadata)seq.Metadata["GenBank"];
                if (metadata.Locus.Strand != SequenceStrandType.None)
                {
                    Assert.AreEqual(StrandType, metadata.Locus.Strand.ToString());
                }
                if (metadata.Locus.Strand != SequenceStrandType.None)
                {
                    Assert.AreEqual(StrandTopology.ToUpper(CultureInfo.CurrentCulture),
                        metadata.Locus.StrandTopology.ToString().ToUpper(
                        CultureInfo.CurrentCulture));
                }
                if (metadata.Locus.DivisionCode != SequenceDivisionCode.None)
                {
                    Assert.AreEqual(Div, metadata.Locus.DivisionCode.ToString());
                }
                Assert.AreEqual(DateTime.Parse(SequenceDate, null), metadata.Locus.Date);

                if (0 != string.Compare(AlphabetName, "rna",
                    CultureInfo.CurrentCulture,CompareOptions.IgnoreCase))
                {
                    Assert.AreEqual(Version, metadata.Version.Version.ToString((IFormatProvider)null));
                    Assert.AreEqual(PrimaryId, metadata.Version.GiNumber);
                    ApplicationLog.WriteLine(
                        "GenBank Parser : Successfully validated the StrandType, StrandTopology, Division, Date, Version, PrimaryID Properties");
                }
                else
                {
                    ApplicationLog.WriteLine(
                        "GenBank Parser : Successfully validated the StrandType, StrandTopology, Division, Date Properties");
                }

                // Replace all the empty spaces, paragraphs and new line for validation
                string updatedExpSequence =
                    ExpectedSequence.Replace("\r", "").Replace("\n", "").Replace(" ", "").ToUpper(
                    CultureInfo.CurrentCulture);
                string updatedActualSequence =
                    new string(seq.Select(a => (char)a).ToArray()).Replace("\r", "").Replace("\n", "").Replace(" ", "").ToUpper(
                    CultureInfo.CurrentCulture);

                Assert.AreEqual(updatedExpSequence, updatedActualSequence);
                ApplicationLog.WriteLine(
                    "GenBank Parser : Successfully validated the Sequence");
            }
        }

        /// <summary>
        /// Validates GenBank Parser for specific test cases
        /// which takes sequence list object
        /// <param name="seqList">Sequence list object.</param>
        /// </summary>
        private static void ValidateParserSpecialTestCases(IEnumerable<ISequence> seqList)
        {
            ISequence seq = seqList.ElementAt(0);
            Assert.AreEqual(Utility.GetAlphabet(AlphabetName),
                seq.Alphabet);
            Assert.AreEqual(SeqId, seq.ID);
            ApplicationLog.WriteLine(
                "GenBank Parser : Successfully validated the Alphabet, Molecular type, Sequence ID and Display ID");

            // Test the metadata that is tricky to parse, and will not be tested implicitly by
            // Testing the formatting
            GenBankMetadata metadata =
                (GenBankMetadata)seq.Metadata["GenBank"];
            if (metadata.Locus.Strand != SequenceStrandType.None)
            {
                Assert.AreEqual(StrandType,
                    metadata.Locus.Strand.ToString());
            }
            if (metadata.Locus.Strand != SequenceStrandType.None)
            {
                Assert.AreEqual(StrandTopology.ToUpper(CultureInfo.CurrentCulture),
                    metadata.Locus.StrandTopology.ToString().ToUpper(CultureInfo.CurrentCulture));
            }

            Assert.AreEqual(Div, metadata.Locus.DivisionCode.ToString());
            Assert.AreEqual(DateTime.Parse(SequenceDate, null),
                metadata.Locus.Date);

            if (0 != string.Compare(AlphabetName, "rna",
                  CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
            {
                Assert.AreEqual(Version, metadata.Version.Version.ToString((IFormatProvider)null));
                Assert.AreEqual(PrimaryId, metadata.Version.GiNumber.ToString((IFormatProvider)null));
                ApplicationLog.WriteLine(
                    "GenBank Parser : Successfully validated the StrandType, StrandTopology, Division, Date, Version, PrimaryID Properties");
            }
            else
            {
                ApplicationLog.WriteLine(
                    "GenBank Parser : Successfully validated the StrandType, StrandTopology, Division, Date Properties");
            }

            // Replace all the empty spaces, paragraphs and new line for validation
            string updatedExpSequence =
                ExpectedSequence.Replace("\r", "").Replace("\n", "").Replace(" ", "").ToUpper(
                CultureInfo.CurrentCulture);
            string updatedActualSequence =
                new string(seq.Select(a => (char)a).ToArray()).Replace("\r", "").Replace("\n", "").Replace(" ", "").ToUpper(
                CultureInfo.CurrentCulture);

            Assert.AreEqual(updatedExpSequence, updatedActualSequence);
            ApplicationLog.WriteLine(
                "GenBank Parser : Successfully validated the Sequence");
        }

        /// <summary>
        /// Validates GenBank Formatter for General test cases.
        /// </summary>
        /// <param name="seqList">sequence list.</param>
        private static void ValidateWriteGeneralTestCases(IEnumerable<ISequence> seqList1)
        {
            // Create a Sequence with all attributes.
            // Parse and update the properties instead of parsing entire file.
            string expectedUpdatedSequence =
                ExpectedSequence.Replace("\r", "").Replace("\n", "").Replace(" ", "");
            Sequence orgSeq =
                new Sequence(Utility.GetAlphabet(AlphabetName), expectedUpdatedSequence);
                orgSeq.Metadata.Add("GenBank",
                    (GenBankMetadata)seqList1.ElementAt(0).Metadata["GenBank"]);
                orgSeq.ID = seqList1.ElementAt(0).ID;
                string tempFileName = System.IO.Path.GetTempFileName();
                ISequenceFormatter formatter = new GenBankFormatter();
                formatter.Format(orgSeq, tempFileName);

                // parse
                GenBankParser parserObj = new GenBankParser();
                IEnumerable<ISequence> seqList = parserObj.Parse(tempFileName);
                ISequence seq = seqList.ElementAt(0);

                Assert.AreEqual(Utility.GetAlphabet(AlphabetName), seq.Alphabet);
                Assert.AreEqual(SeqId, seq.ID);
                ApplicationLog.WriteLine(
                    "GenBank Formatter P1: Successfully validated the Alphabet, Molecular type, Sequence ID and Display ID");

                // test the metadata that is tricky to parse, and will not be tested implicitly by
                // testing the formatting
                GenBankMetadata metadata =
                    (GenBankMetadata)seq.Metadata["GenBank"];
                if (metadata.Locus.Strand != SequenceStrandType.None)
                {
                    Assert.AreEqual(StrandType,
                        metadata.Locus.Strand.ToString());
                }
                if (metadata.Locus.Strand != SequenceStrandType.None)
                {
                    Assert.AreEqual(StrandTopology.ToUpper(CultureInfo.CurrentCulture),
                        metadata.Locus.StrandTopology.ToString().ToUpper(CultureInfo.CurrentCulture));
                }
                if (metadata.Locus.DivisionCode != SequenceDivisionCode.None)
                {
                    Assert.AreEqual(Div,
                        metadata.Locus.DivisionCode.ToString());
                }
                Assert.AreEqual(DateTime.Parse(SequenceDate, null),
                    metadata.Locus.Date);

                if (0 != string.Compare(AlphabetName, "rna",
                     CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                {
                    Assert.AreEqual(Version, metadata.Version.Version.ToString((IFormatProvider)null));
                    Assert.AreEqual(PrimaryId, metadata.Version.GiNumber);
                    ApplicationLog.WriteLine(
                        "GenBank Parser : Successfully validated the StrandType, StrandTopology, Division, Date, Version, PrimaryID Properties");
                }
                else
                {
                    ApplicationLog.WriteLine(
                        "GenBank Parser : Successfully validated the StrandType, StrandTopology, Division, Date Properties");
                }

                string truncatedExpectedSequence =
                    ExpectedSequence.Replace("\r", "").Replace("\n", "").Replace(" ", "").ToUpper(
                    CultureInfo.CurrentCulture);
                string truncatedActualSequence =
                    new string(seq.Select(a => (char)a).ToArray()).Replace("\r", "").Replace("\n", "").Replace(" ", "").ToUpper(
                    CultureInfo.CurrentCulture);

                // test the sequence string
                Assert.AreEqual(truncatedExpectedSequence, truncatedActualSequence);
                ApplicationLog.WriteLine(
                    "GenBank Formatter P1: Successfully validated the Sequence");
                File.Delete(tempFileName);
        }

        /// <summary>
        /// Validates GenBank Formatter with file path for General test cases.
        /// </summary>
        /// <param name="seqList">sequence list.</param>
        private static void ValidateWriterWithFilePathGeneralTestCases(IEnumerable<ISequence> seqList1)
        {
            // Create a Sequence with all attributes.
            // Parse and update the properties instead of parsing entire file.
            string expectedUpdatedSequence =
                ExpectedSequence.Replace("\r", "").Replace("\n", "").Replace(" ", "");
            Sequence orgSeq =
                new Sequence(Utility.GetAlphabet(AlphabetName), expectedUpdatedSequence);
            orgSeq.Metadata.Add("GenBank",
                (GenBankMetadata)seqList1.ElementAt(0).Metadata["GenBank"]);
            orgSeq.ID = seqList1.ElementAt(0).ID;
            string tempFileName = System.IO.Path.GetTempFileName();
            ISequenceFormatter formatter = new GenBankFormatter();
            {
                formatter.Format(orgSeq, tempFileName);

                // parse
                GenBankParser parserObj = new GenBankParser();
                IEnumerable<ISequence> seqList = parserObj.Parse(tempFileName);
                ISequence seq = seqList.ElementAt(0);
                Assert.AreEqual(Utility.GetAlphabet(AlphabetName), seq.Alphabet);
                Assert.AreEqual(SeqId, seq.ID);
                ApplicationLog.WriteLine(
                    "GenBank Formatter P1: Successfully validated the Alphabet, Molecular type, Sequence ID and Display ID");

                // test the metadata that is tricky to parse, and will not be tested implicitly by
                // testing the formatting
                GenBankMetadata metadata =
                    (GenBankMetadata)seq.Metadata["GenBank"];
                if (metadata.Locus.Strand != SequenceStrandType.None)
                {
                    Assert.AreEqual(StrandType, metadata.Locus.Strand.ToString());
                }
                if (metadata.Locus.Strand != SequenceStrandType.None)
                {
                    Assert.AreEqual(StrandTopology.ToUpper(CultureInfo.CurrentCulture),
                        metadata.Locus.StrandTopology.ToString().ToUpper(CultureInfo.CurrentCulture));
                }

                Assert.AreEqual(Div, metadata.Locus.DivisionCode.ToString());
                Assert.AreEqual(DateTime.Parse(SequenceDate, null), metadata.Locus.Date);

                if (0 != string.Compare(AlphabetName, "rna", CultureInfo.CurrentCulture, 
                    CompareOptions.IgnoreCase))
                {
                    Assert.AreEqual(Version, metadata.Version.Version);
                    Assert.AreEqual(PrimaryId, metadata.Version.GiNumber);
                    ApplicationLog.WriteLine(
                        "GenBank Parser : Successfully validated the StrandType, StrandTopology, Division, Date, Version, PrimaryID Properties");
                }
                else
                {
                    ApplicationLog.WriteLine(
                        "GenBank Parser : Successfully validated the StrandType, StrandTopology, Division, Date Properties");
                }


                string truncatedExpectedSequence =
                    ExpectedSequence.Replace("\r", "").Replace("\n", "").Replace(" ", "").ToUpper(
                    CultureInfo.CurrentCulture);
                string truncatedActualSequence =
                    new string(seq.Select(a => (char)a).ToArray()).Replace("\r", "").Replace("\n", "").Replace(" ", "").ToUpper(
                    CultureInfo.CurrentCulture);

                // test the sequence string
                Assert.AreEqual(truncatedExpectedSequence, truncatedActualSequence);
                ApplicationLog.WriteLine(
                    "GenBank Formatter P1: Successfully validated the Sequence");
                if (File.Exists(tempFileName))
                    File.Delete(tempFileName);
            }
        }

        /// <summary>
        /// Parse the file and validate GenBank Formatter for General test cases.
        /// </summary>
        private static void ValidateWriterGeneralTestCases()
        {
            // Parse the file
            ISequenceParser parseObj = new GenBankParser();
            {
                IEnumerable<ISequence> seqList = parseObj.Parse(FilePath);

                // Validate the sequence and few more properties.
                ValidateWriteGeneralTestCases(seqList);
            }
        }

        /// <summary>
        /// Parse the file and validate GenBank Formatter for General test cases.
        /// </summary>
        private static void ValidateWriteWithFilePathGeneralTestCases()
        {
            // Parse the file
            ISequenceParser parseObj = new GenBankParser();
            {
                IEnumerable<ISequence> seqList = parseObj.Parse(FilePath);

                // Validate the sequence and few more properties.
                ValidateWriterWithFilePathGeneralTestCases(seqList);
            }
        }

        /// <summary>
        /// Validates GenBank Formatter for General test cases.
        /// </summary>
        private static void ValidateParseWithTextReaderWriteGeneralTestCases()
        {
            // Parse the file using text reader.
            ISequenceParser parseObj = new GenBankParser();
            {
                IEnumerable<ISequence> seqList = parseObj.Parse(FilePath);

                // Validate the sequence and few more properties.
                ValidateWriteGeneralTestCases(seqList);
            }
        }

        /// <summary>
        /// Validates GenBank parser using text reader.
        /// </summary>
        private static void ValidateParseWithTextReaderTestCases()
        {
            // Parse the file using text reader.
            ISequenceParser parserObj = new GenBankParser();
            IEnumerable<ISequence> seqList = parserObj.Parse(FilePath);

            // Validate the sequence and few more properties.
            ValidateParserSpecialTestCases(seqList);
        }

        /// <summary>
        /// Validates GenBank Formatter for the files which are 
        /// Parsed using GenBankParser for General test cases.
        /// </summary>
        private static void ValidateParseWriterGeneralTestCases()
        {
            Assert.IsTrue(File.Exists(FilePath));
            // Logs information to the log file
            ApplicationLog.WriteLine(string.Format("GenBank Parser : File Exists in the Path '{0}'.", FilePath));
            string tempFileName = Path.GetTempFileName();

            // parse
            ISequenceParser parserObj = new GenBankParser();
            {
                IEnumerable<ISequence> seqList = parserObj.Parse(FilePath);
                ISequence seq = seqList.ElementAt(0);
                ISequenceFormatter formatter = new GenBankFormatter();
                {
                    formatter.Format(seq, tempFileName);
                    FilePath = tempFileName;
                    ValidateParserGeneralTestCases();
                }
            }
        }

        /// <summary>
        /// Validates Parse test cases for files with multiple sequences
        /// with the xml node name, fasta file path specified.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        void ValidateParseMultiSeqTestCases(string nodeName)
        {
            // Initialize only required properties.
            FilePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            AlphabetName = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlphabetNameNode);
            SeqId = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.SequenceIdNode);
            StrandTopology = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.StrandTopologyNode);
            StrandType = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.StrandTypeNode);
            Div = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DivisionNode);
            Version = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.VersionNode);
            SequenceDate = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DateNode);
            PrimaryId = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.PrimaryIdNode);

            Assert.IsTrue(File.Exists(FilePath));
            // Logs information to the log file
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "FastA Parser : File Exists in the Path '{0}'.", FilePath));

            IEnumerable<ISequence> seqs = null;
            GenBankParser parserObj = new GenBankParser();
            seqs = parserObj.Parse(FilePath);

            int seqCount = int.Parse(utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.NumberOfSequencesNode), null);
            Assert.IsNotNull(seqs);
            Assert.AreEqual(seqCount, seqs.Count());
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "FastA Parser: Number of Sequences found are '{0}'.",
                seqs.Count().ToString((IFormatProvider)null)));

            // Gets the expected sequences from the Xml, in the test cases
            // we are just validating with 2 sequences and maximum 3 
            // sequences. So, based on that we are validating.
            string expectedSequence1 = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequenceNode1);
            string expectedSequence2 = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequenceNode2);
            string[] expSeqs = null;
            if (2 == seqCount)
            {
                expSeqs = new string[2] { expectedSequence1, expectedSequence2 };
            }
            else
            {
                string expectedSequence3 = utilityObj.xmlUtil.GetTextValue(
                    nodeName, Constants.ExpectedSequenceNode3);
                expSeqs = new string[3] { expectedSequence1, expectedSequence2, 
                    expectedSequence3 };
            }

            // Validate each sequence.
            for (int i = 0; i < seqCount; i++)
            {
                ValidateParserGeneralTestCases(seqs.ElementAt(i), expSeqs[i]);
            }
        }

        /// <summary>
        /// Validates GenBank Parser for each sequence.
        /// which takes sequence list as input.
        /// <param name="seq">Original Sequence.</param>
        /// <param name="expectedSequence">Expected Sequence.</param>
        /// </summary>
        private static void ValidateParserGeneralTestCases(ISequence seq,
             string expectedSequence)
        {

            Assert.AreEqual(Utility.GetAlphabet(AlphabetName),
                seq.Alphabet);
            Assert.AreEqual(SeqId, seq.ID);
            ApplicationLog.WriteLine(
                "GenBank Parser : Successfully validated the Alphabet, Molecular type, Sequence ID and Display ID");

            // test the metadata that is tricky to parse, and will not be tested implicitly by
            // testing the formatting
            GenBankMetadata metadata =
                (GenBankMetadata)seq.Metadata["GenBank"];
            if (metadata.Locus.Strand != SequenceStrandType.None)
            {
                Assert.AreEqual(StrandType, metadata.Locus.Strand.ToString());
            }
            if (metadata.Locus.Strand != SequenceStrandType.None)
            {
                Assert.AreEqual(StrandTopology.ToUpper(CultureInfo.CurrentCulture),
                    metadata.Locus.StrandTopology.ToString().ToUpper(CultureInfo.CurrentCulture));
            }
            Assert.AreEqual(Div, metadata.Locus.DivisionCode.ToString());
            Assert.AreEqual(DateTime.Parse(SequenceDate, null),
                metadata.Locus.Date);

            if (0 != string.Compare(AlphabetName, "rna",
                  CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
            {
                Assert.AreEqual(Version, metadata.Version.Version.ToString((IFormatProvider)null));
                Assert.AreEqual(PrimaryId, metadata.Version.GiNumber);
                ApplicationLog.WriteLine(
                    "GenBank Parser : Successfully validated the StrandType, StrandTopology, Division, Date, Version, PrimaryID Properties");
            }
            else
            {
                ApplicationLog.WriteLine(
                    "GenBank Parser : Successfully validated the StrandType, StrandTopology, Division, Date Properties");
            }

            // Replace all the empty spaces, paragraphs and new line for validation
            string updatedExpSequence =
                expectedSequence.Replace("\r", "").Replace("\n", "").Replace(" ", "").ToUpper(CultureInfo.CurrentCulture);
            string updatedActualSequence =
                new string(seq.Select(a => (char)a).ToArray()).Replace("\r", "").Replace("\n", "").Replace(" ", "").ToUpper(CultureInfo.CurrentCulture);

            Assert.AreEqual(updatedExpSequence, updatedActualSequence);
            ApplicationLog.WriteLine(
                "GenBank Parser : Successfully validated the Sequence");
        }

        #endregion Supporting Methods
    }
}
