using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bio;
using Bio.Algorithms.Assembly;
using Bio.IO.Xsv;

using Microsoft.SqlServer.Server;
using NUnit.Framework;

namespace Bio.Tests.IO.Xsv
{
    /// <summary>
    /// Tests for the XsvSparse classes.
    /// </summary>
    [TestFixture]
    public class XsvSparseTests
    {
        private const string XsvFilename = @"\TestUtils\SampleSparseSeq.csv";

        /// <summary>
        /// Validate xsv parser for filepath
        /// Input : XsvSparse File
        /// Validation : Expected sequence, Sequence Alphabet, Sequence ID.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void XsvSparserParserValidateParseFilePath()
        {
            XsvSparseParserGeneralTestCases();
        }

        /// <summary>
        /// Validate xsv formatter for filepath
        /// Input : XsvSparse File
        /// Validation : Format is successful.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void XsvSparseFormatterValidateFilePath()
        {
            XsvSparseFormatterGeneralTestCases("FormatFilePath");
        }

        /// <summary>
        /// Validate xsv formatter for one filepath
        /// Input : XsvSparse File
        /// Validation : Format is successful.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void XsvSparseFormatterValidateFilePathWithSeqList()
        {
            XsvSparseFormatterGeneralTestCases("ForamtListWithFilePath");
        }

        /// <summary>
        /// Validate All properties in XsvSparse formatter class
        /// Input : One line sequence and update all properties
        /// Validation : Validate the properties
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void XsvSparseFormatterProperties()
        {
            string XsvTempFileName = Path.GetTempFileName();
            XsvSparseFormatter formatterObj = new XsvSparseFormatter(',', '#');
            using (formatterObj.Open(XsvTempFileName))
            {
                Assert.AreEqual("Sparse Sequence formatter to character separated value file", formatterObj.Description);
                Assert.AreEqual("csv,tsv", formatterObj.SupportedFileTypes);
                Assert.AreEqual("XsvSparseFormatter", formatterObj.Name);
                Assert.AreEqual(',', formatterObj.Separator);
                Assert.AreEqual('#', formatterObj.SequenceIDPrefix);
            }

            if (File.Exists(XsvTempFileName))
                File.Delete(XsvTempFileName);
        }

        /// <summary>
        /// Validate SparseContigFormatter
        /// Input : Xsv file.
        /// Validation : Validation of Format() method.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void XsvContigFormatter()
        {
            // Gets the expected sequence from the Xml
            string filePathObj = Directory.GetCurrentDirectory() + XsvFilename;
            string xsvTempFileName = Path.GetTempFileName();
            Assert.IsTrue(File.Exists(filePathObj));

            // Read the contigs
            Contig contig = new XsvContigParser(Alphabets.DNA, ',', '#')
                .ParseContig(filePathObj);

            string seqId = contig.Sequences.Aggregate(string.Empty, (current, seq) => current + (seq.Sequence.ID + ","));

            // Format Xsv file.
            new XsvContigFormatter(',', '#')
                .Format(contig, xsvTempFileName);

            Contig expectedContig = new XsvContigParser(Alphabets.DNA, ',', '#')
                .ParseContig(xsvTempFileName);

            string expectedseqId = expectedContig.Sequences.Aggregate(string.Empty, (current, seq) => current + (seq.Sequence.ID + ","));

            // Validate parsed temp file with original Xsv file.
            Assert.AreEqual(contig.Length, expectedContig.Length);
            Assert.AreEqual(contig.Consensus.Count, expectedContig.Consensus.Count);
            Assert.AreEqual(contig.Consensus.ID, expectedContig.Consensus.ID);
            Assert.AreEqual(contig.Sequences.Count, expectedContig.Sequences.Count);
            Assert.AreEqual(seqId.Length, expectedseqId.Length);
            Assert.AreEqual(seqId, expectedseqId);
        }

        /// <summary>
        /// XsvSparse parser generic method called by all the test cases 
        /// to validate the test case based on the parameters passed.
        /// </summary>
        private void XsvSparseParserGeneralTestCases()
        {
            // Gets the expected sequence from the Xml
            string filePathObj = Directory.GetCurrentDirectory() + XsvFilename;

            Assert.IsTrue(File.Exists(filePathObj));
            XsvContigParser parserObj = new XsvContigParser(Alphabets.DNA, ',', '#');
            
            string expectedSeqIds = "Chr22+Chr22+Chr22+Chr22,m;Chr22;16,m;Chr22;17,m;Chr22;29,m;Chr22;32,m;Chr22;39,m;Chr22;54,m;Chr22;72,m;Chr22;82,m;Chr22;85,m;Chr22;96,m;Chr22;99,m;Chr22;118,m;Chr22;119,m;Chr22;129,m;Chr22;136,m;Chr22;146,m;Chr22;153,m;Chr22;161,m;Chr22;162,m;Chr22;174,m;Chr22;183,m;Chr22;209,m;Chr22;210,m;Chr22;224,m;Chr22;241,m;Chr22;243,m;Chr22;253,m;Chr22;267,m;Chr22;309,m;Chr22;310,m;Chr22;313,m;Chr22;331,m;Chr22;333,m;Chr22;338,m;Chr22;348,m;Chr22;352,m;Chr22;355,m;Chr22;357,m;Chr22;368,m;Chr22;370,m;Chr22;380,m;Chr22;382,m;Chr22;402,m;Chr22;418,m;Chr22;419,m;Chr22;429,m;Chr22;432,m;Chr22;450,m;Chr22;462,m;Chr22;482,m;Chr22;484,m;Chr22;485,m;Chr22;494,m;Chr22;508,m;Chr22;509,m;Chr22;512,";

            IEnumerable<ISequence> seqList = parserObj.Parse(filePathObj);
            SparseSequence sparseSeq = (SparseSequence) seqList.FirstOrDefault();
                 
            if (null == sparseSeq)
            {
                string expCount = "57";
                Assert.IsNotNull(seqList);
                Assert.AreEqual(expCount, seqList.ToList().Count);

                StringBuilder actualId = new StringBuilder();
                foreach (ISequence seq in seqList)
                {
                    SparseSequence sps = (SparseSequence)seq;
                    actualId.Append(sps.ID);
                    actualId.Append(",");
                }

                Assert.AreEqual(expectedSeqIds, actualId.ToString());
            }
            else
            {
                string[] idArray = expectedSeqIds.Split(',');
                Assert.AreEqual(sparseSeq.ID, idArray[0]);
            }

            string XsvTempFileName = Path.GetTempFileName();
            XsvSparseFormatter formatter = new XsvSparseFormatter(',', '#');
            using (formatter.Open(XsvTempFileName))
            {
                formatter.Format(seqList.ToList());
            }

            string expectedOutput = string.Empty;
            using (StreamReader readerSource = new StreamReader(filePathObj))
            {
                expectedOutput = readerSource.ReadToEnd();
            }

            string actualOutput = string.Empty;
            using (StreamReader readerDest = new StreamReader(XsvTempFileName))
            {
                actualOutput = readerDest.ReadToEnd();
            }
           
            Assert.AreEqual(expectedOutput.Replace("\r\n", System.Environment.NewLine), actualOutput);


            Assert.IsNotNull(sparseSeq.Alphabet);
            // Delete the temporary file.
            if (File.Exists(XsvTempFileName))
                File.Delete(XsvTempFileName);
        }

        /// <summary>
        /// XsvSparse formatter generic method called by all the test cases 
        /// to validate the test case based on the parameters passed.
        /// </summary>
        /// <param name="switchParam">Additional parameter 
        /// based on which the validation of  test case is done.</param>
        private void XsvSparseFormatterGeneralTestCases(string switchParam)
        {
            // Gets the expected sequence from the Xml
            string filePathObj = Directory.GetCurrentDirectory() + XsvFilename;

            Assert.IsTrue(File.Exists(filePathObj));

            XsvContigParser parserObj = new XsvContigParser(Alphabets.DNA, ',', '#');
            IEnumerable<ISequence> seqList = parserObj.Parse(filePathObj);
            SparseSequence sparseSeq = (SparseSequence) seqList.FirstOrDefault();

            var sparseSeqItems = sparseSeq.GetKnownSequenceItems();

            string xsvTempFileName = Path.GetTempFileName();
            XsvSparseFormatter formatterObj = new XsvSparseFormatter(',', '#');
            using (formatterObj.Open(xsvTempFileName))
            {
                switch (switchParam)
                {
                    case "FormatFilePath":
                        formatterObj.Format(sparseSeq);
                        break;
                    case "ForamtListWithFilePath":
                        formatterObj.Format(sparseSeq);
                        break;
                }
            }

            // Parse a formatted Xsv file and validate.
            var parserObj1 = new XsvContigParser(Alphabets.DNA, ',', '#');
            using (parserObj1.Open(xsvTempFileName))
            {
                seqList = parserObj1.Parse();
                SparseSequence expectedSeq = (SparseSequence)seqList.FirstOrDefault();

                var expectedSparseSeqItems = expectedSeq.GetKnownSequenceItems();

                Assert.AreEqual(sparseSeqItems.Count, expectedSparseSeqItems.Count);
                for (int i = 0; i < sparseSeqItems.Count; i++)
                {
                    Assert.AreEqual(sparseSeqItems.ElementAt(i).Index, expectedSparseSeqItems.ElementAt(i).Index);
                    Assert.AreEqual(sparseSeqItems.ElementAt(i).Item, expectedSparseSeqItems.ElementAt(i).Item);
                }

            }

            // Delete the temporary file.
            if (File.Exists(xsvTempFileName))
                File.Delete(xsvTempFileName);
        }
    }
}
