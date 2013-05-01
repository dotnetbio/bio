/****************************************************************************
 * SequenceParserFormatterBvtTestCases.cs
 * 
 *   This file contains the SequenceParserFormatter - Parsers & Formatter Bvt test cases.
 * 
***************************************************************************/

using Bio.IO;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.TestAutomation.Util;

namespace Bio.TestAutomation.IO
{
    /// <summary>
    /// SequenceParserFormatter Bvt parser Test case implementation.
    /// </summary>
    [TestClass]
    public class SequenceParserFormatterBvtTestCases
    {
        #region enum

        private enum SequenceType
        {
            FastA,
            FastQ,
            GenBank,
            GFF,
        };

        #endregion enum

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static SequenceParserFormatterBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region Parser Test cases

        /// <summary>
        /// Validate FindParserByFileName() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSequenceParserFindParserByFileName()
        {
            ISequenceParser fastAObj = SequenceParsers.FindParserByFileName(@"TestUtils\Small_Size.fasta");
            Assert.AreEqual(SequenceType.FastA.ToString(), fastAObj.Name);

            ISequenceParser gbkObj = SequenceParsers.FindParserByFileName(@"TestUtils\Small_Size.gbk");
            Assert.AreEqual(SequenceType.GenBank.ToString(), gbkObj.Name);

            ISequenceParser fastQObj = SequenceParsers.FindParserByFileName(@"TestUtils\Simple_Fastq_Sequence.fastq");
            Assert.AreEqual(SequenceType.FastQ.ToString(), fastQObj.Name);

            ISequenceParser gffObj = SequenceParsers.FindParserByFileName(@"TestUtils\Simple_Gff_Dna.gff");
            Assert.AreEqual(SequenceType.GFF.ToString(), gffObj.Name);

            ApplicationLog.WriteLine("Sequence Formatter BVT : Successfully validated the FindParserByFileName() method");
        }
        
        /// <summary>
        /// Validate FindParserByName() method with Fasta format
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSequenceParserFindParserByNameForFastA()
        {
            ValidateFindParserByName(SequenceType.FastA);
        }

        /// <summary>
        /// Validate FindParserByName() method with FastQ format
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSequenceParserFindParserByNameForFastQ()
        {
            ValidateFindParserByName(SequenceType.FastQ);
        }

        /// <summary>
        /// Validate FindParserByName() method with GFF format
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSequenceParserFindParserByNameForGFF()
        {
            ValidateFindParserByName(SequenceType.GFF);
        }

        /// <summary>
        /// Validate FindParserByName() method with Genbank format
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSequenceParserFindParserByNameForGenbank()
        {
            ValidateFindParserByName(SequenceType.GenBank);
        }

        #endregion Test cases

        #region Formatter Test cases

        /// <summary>
        /// Validate FindFormatterByFileName() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSequenceFormatterFindFormatterByFileName()
        {
            IFormatter fastAObj = SequenceFormatters.FindFormatterByFileName("test.fasta");
            Assert.AreEqual(SequenceType.FastA.ToString(), fastAObj.Name);

            IFormatter gbkObj = SequenceFormatters.FindFormatterByFileName("test.gbk");
            Assert.AreEqual(SequenceType.GenBank.ToString(), gbkObj.Name);

            IFormatter fastQObj = SequenceFormatters.FindFormatterByFileName("test.fastq");
            Assert.AreEqual(SequenceType.FastQ.ToString(), fastQObj.Name);

            IFormatter gffObj = SequenceFormatters.FindFormatterByFileName("test.gff");
            Assert.AreEqual(SequenceType.GFF.ToString(), gffObj.Name);

            ApplicationLog.WriteLine("Sequence Formatter BVT : Successfully validated the FindFormatterByFileName() method");
        }

        
        /// <summary>
        /// Validate FindFormatterByName() method with Fasta format
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSequenceFormatterFindFormatterByNameForFastA()
        {
            ValidateFindFormatterByName(SequenceType.FastA);
        }

        /// <summary>
        /// Validate FindFormatterByName() method with FastQ format
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSequenceFormatterFindFormatterByNameForFastQ()
        {
            ValidateFindFormatterByName(SequenceType.FastQ);
        }

        /// <summary>
        /// Validate FindFormatterByName() method with GFF format
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSequenceFormatterFindFormatterByNameForGFF()
        {
            ValidateFindFormatterByName(SequenceType.GFF);
        }

        /// <summary>
        /// Validate FindFormatterByName() method with Genbank format
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSequenceFormatterFindFormatterByNameForGenbank()
        {
            ValidateFindFormatterByName(SequenceType.GenBank);
        }               

      #endregion

      # region Supporting methods.

        void ValidateFindFormatterByName(SequenceType type)
        {
            string filename = null;
            string formatterName = null;
            string expectedFomatter = null;

            switch (type)
            {
                case SequenceType.FastA:                    
                    filename = Constants.FastaTempFileName;
                    formatterName = SequenceType.FastA.ToString();
                    expectedFomatter = Constants.FastAFormatter;
                    break;
                case SequenceType.FastQ:
                    filename = Constants.FastQTempFileName;
                    formatterName = SequenceType.FastQ.ToString();
                    expectedFomatter = Constants.FastQFormatter;
                    break;
                case SequenceType.GenBank:
                    filename = Constants.GenBankTempFileName;
                    formatterName = SequenceType.GenBank.ToString();
                    expectedFomatter = Constants.GenBankFormatter;
                    break;
                case SequenceType.GFF:                    
                    filename = Constants.GffTempFileName;
                    formatterName = SequenceType.GFF.ToString();
                    expectedFomatter = Constants.GffFormatter;
                    break;
            }

            ISequenceFormatter formatter = SequenceFormatters.FindFormatterByName(filename, formatterName);
            System.Type formatterTypes = formatter.GetType();
            Assert.AreEqual(formatterTypes.ToString(), expectedFomatter);
            ApplicationLog.WriteLine("Sequence Formatter BVT : Successfully validated the FindFormatterByName() method");
        }

        void ValidateFindParserByName(SequenceType type)
        {
            string filename = null;
            string parserName = null;
            string expectedParser = null;

            switch (type)
            {
                case SequenceType.FastA:
                    filename = @"TestUtils\Small_Size.fasta";
                    parserName = SequenceType.FastA.ToString();
                    expectedParser = Constants.FastAParser;
                    break;
                case SequenceType.FastQ:
                    filename = @"TestUtils\Simple_Fastq_Sequence.fastq";
                    parserName = SequenceType.FastQ.ToString();
                    expectedParser = Constants.FastQParser;
                    break;
                case SequenceType.GenBank:
                    filename = @"TestUtils\Small_Size.gbk";
                    parserName = SequenceType.GenBank.ToString();
                    expectedParser = Constants.GenBankParser;
                    break;
                case SequenceType.GFF:
                    filename = @"TestUtils\Simple_Gff_Dna.gff";
                    parserName = SequenceType.GFF.ToString();
                    expectedParser = Constants.GffParser;
                    break;
            }

            ISequenceParser parser = SequenceParsers.FindParserByName(filename, parserName);
            System.Type parserTypes = parser.GetType();
            Assert.AreEqual(parserTypes.ToString(), expectedParser);
            ApplicationLog.WriteLine("Sequence parser BVT : Successfully validated the FindParserByName() method");
        }

      #endregion Supporting methods.
    }
}
