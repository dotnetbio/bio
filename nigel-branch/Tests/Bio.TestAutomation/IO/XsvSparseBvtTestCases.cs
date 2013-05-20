/****************************************************************************
 * XsvSparseBvtTestCases.cs
 * 
 *   This file contains the XsvSparse - Parsers & Formatter Bvt test cases.
 * 
***************************************************************************/

using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Algorithms.Assembly;
using Bio.IO.Xsv;

namespace Bio.TestAutomation.IO
{
    /// <summary>
    /// XsvSparse Bvt parser Test case implementation.
    /// </summary>
    [TestClass]
    public class XsvSparseBvtTestCases
    {
        #region Enums

        /// <summary>
        /// Additional Parameters which are used for different test cases 
        /// based on which the test cases are executed.
        /// </summary>
        enum AdditionalParameters
        {
            FormatFilePath,
            ForamtListWithFilePath
        };

        #endregion Enums

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static XsvSparseBvtTestCases()
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
        /// Parse a valid XsvSparse file (Small size sequence less than 35 kb) and convert the 
        /// same to one sequence using Parse(file-name) method and validate with the 
        /// expected sequence.
        /// Input : XsvSparse File
        /// Validation : Expected sequence, Sequence Alphabet, Sequence ID.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void XsvSparseBvtParserValidateParseFilePath()
        {
            XsvSparseParserGeneralTestCases(Constants.SimpleXsvSparseNodeName);
        }

        /// <summary>
        /// Validate All properties in XsvSparse parser class
        /// Input : One line sequence and update all properties
        /// Validation : Validate the properties
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void XsvSparseBvtParserProperties()
        {
            // Gets the expected file from the Xml
            string filePathObj = utilityObj.xmlUtil.GetTextValue(Constants.SimpleXsvSparseNodeName,
                Constants.FilePathNode);

            XsvContigParser xsvParser = new XsvContigParser(filePathObj, Alphabets.DNA, ',', '#');
            Assert.AreEqual(Constants.XsvSparseDescription, xsvParser.Description);
            Assert.AreEqual(Constants.XsvSparseFileTypes, xsvParser.SupportedFileTypes);
            Assert.AreEqual(Constants.XsvSparseName, xsvParser.Name);

            ApplicationLog.WriteLine
                ("Successfully validated all the properties of XsvSparse Parser class.");
        }

        /// <summary>
        /// Validate SparseContigFormatter Format()
        /// Input : Xsv file.
        /// Validation : Validation of Format() method.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void XsvSparseParseContig()
        {
            // Gets the expected file from the Xml
            string filePathObj = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleXsvSparseNodeName,
                Constants.FilePathNode);

            Assert.IsTrue(File.Exists(filePathObj));
            // Logs information to the log file
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "XsvSparse Formatter BVT: File Exists in the Path '{0}'.",
                filePathObj));

            using (XsvContigParser parserObj = new XsvContigParser(filePathObj,
                Alphabets.DNA, ',', '#'))
            {
                parserObj.Parse();

                Contig contig = parserObj.ParseContig();

                // Validate parsed temp file with original Xsv file.
                Assert.AreEqual(26048682, contig.Length);
                Assert.AreEqual(26048682, contig.Consensus.Count);
                Assert.AreEqual("Chr22+Chr22+Chr22+Chr22", contig.Consensus.ID);
                Assert.AreEqual(56, contig.Sequences.Count);
            }
            // Log to GUI.
            ApplicationLog.WriteLine("Successfully validated the ParseConting() method with Xsv file");
        }

        #endregion Test cases

        #region Formatter Test cases

        /// <summary>
        /// Parse a valid XsvSparse file (Small size sequence less than 35 kb) and convert the 
        /// same to one sequence using Parse(file-name) method and format back using 
        /// Format(isequence, filename)
        /// Input : XsvSparse File
        /// Validation : Format is successful.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void XsvSparseBvtFormatterValidateFilePath()
        {
            XsvSparseFormatterGeneralTestCases(Constants.SimpleXsvSparseNodeName,
                AdditionalParameters.FormatFilePath);
        }

        /// <summary>
        /// Parse a valid XsvSparse file (Small size sequence less than 35 kb) and convert the 
        /// same to one sequence using Parse(file-name) method and format back using 
        /// Format(ListSequence, filename)
        /// Input : XsvSparse File
        /// Validation : Format is successful.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void XsvSparseBvtFormatterValidateFilePathWithSeqList()
        {
            XsvSparseFormatterGeneralTestCases(Constants.SimpleXsvSparseNodeName,
                AdditionalParameters.ForamtListWithFilePath);
        }

        /// <summary>
        /// Validate All properties in XsvSparse formatter class
        /// Input : One line sequence and update all properties
        /// Validation : Validate the properties
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void XsvSparseBvtFormatterProperties()
        {
            XsvSparseFormatter formatterObj = new XsvSparseFormatter(Constants.CharSeperator,
                Constants.SequenceIDPrefix);
            Assert.AreEqual(Constants.XsvSparseFormatterDescription, formatterObj.Description);
            Assert.AreEqual(Constants.XsvSparseFileTypes, formatterObj.SupportedFileTypes);
            Assert.AreEqual(Constants.XsvSparseFormatterNode, formatterObj.Name);
            Assert.AreEqual(Constants.XsvSeperator, formatterObj.Separator);
            Assert.AreEqual(Constants.XsvSeqIdPrefix, formatterObj.SequenceIDPrefix);

            ApplicationLog.WriteLine
                ("Successfully validated all the properties of XsvSparse Formatter class.");
        }

        /// <summary>
        /// Validate XsvContigFormatter Write()
        /// Input : Xsv file.
        /// Validation : Validation of Write() method.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void XsvSparseContigFormatterWrite()
        {
            // Gets the expected sequence from the Xml
            string filePathObj = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleXsvSparseNodeName,
                Constants.FilePathNode);

            Assert.IsTrue(File.Exists(filePathObj));

            // Logs information to the log file
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Xsv Contig Formatter BVT: File Exists in the Path '{0}'.",
                filePathObj));
            Contig contig, expectedContig;

            using (XsvContigParser parserObj = new XsvContigParser(filePathObj, Alphabets.DNA,
                ',', '#'))
            {                          
                contig = parserObj.ParseContig();                
            }           
            
            string seqId = string.Empty;
            foreach (Contig.AssembledSequence seq in contig.Sequences)
            {
                seqId += seq.Sequence.ID + ",";
            }

            // Write Xsv file.            
            using(XsvContigFormatter formatObj = new XsvContigFormatter(Constants.XsvTempFileName, ',', '#'))
            {                           
               formatObj.Write(contig);                                
            }
                                    
            using (XsvContigParser parserObjNew = new XsvContigParser(Constants.XsvTempFileName, Alphabets.DNA,
                  ',', '#'))
            {                            
                expectedContig = parserObjNew.ParseContig();
            }
             
            string expectedseqId = string.Empty;
            foreach (Contig.AssembledSequence seq in expectedContig.Sequences)
            {
                expectedseqId += seq.Sequence.ID + ",";
            }

            // Validate parsed temp file with original Xsv file.
            Assert.AreEqual(contig.Length, expectedContig.Length);
            Assert.AreEqual(contig.Consensus.Count, expectedContig.Consensus.Count);            
            Assert.AreEqual(contig.Consensus.ID, expectedContig.Consensus.ID);
            Assert.AreEqual(contig.Sequences.Count, expectedContig.Sequences.Count);
            Assert.AreEqual(seqId.Length, expectedseqId.Length);
            Assert.AreEqual(seqId, expectedseqId);

            ApplicationLog.WriteLine("Successfully validated the Write Xsv file");
        }

        #endregion

        #region Supporting Methods

        /// <summary>
        /// XsvSparse parser generic method called by all the test cases 
        /// to validate the test case based on the parameters passed.
        /// </summary>
        /// <param name="nodename">Xml node Name.</param>
        /// <param name="additionalParam">Additional parameter 
        /// based on which the validation of  test case is done.</param>
        void XsvSparseParserGeneralTestCases(string nodename)
        {
            // Gets the expected file from the Xml
            string filePathObj = utilityObj.xmlUtil.GetTextValue(nodename,
                Constants.FilePathNode);

            Assert.IsTrue(File.Exists(filePathObj));
            // Logs information to the log file
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "XsvSparse Parser BVT: File Exists in the Path '{0}'.",
                filePathObj));

            IEnumerable<ISequence> seqList = null;
            SparseSequence sparseSeq = null;
            XsvContigParser parserObj = new XsvContigParser(filePathObj, Alphabets.DNA,
                Constants.CharSeperator, Constants.SequenceIDPrefix);

            string expectedSeqIds = utilityObj.xmlUtil.GetTextValue(nodename,
                Constants.SequenceIdNode);

            seqList = parserObj.Parse();
            sparseSeq = (SparseSequence)seqList.ElementAt(0);

            if (null == sparseSeq)
            {
                string expCount = utilityObj.xmlUtil.GetTextValue(nodename,
                    Constants.SequenceCountNode);

                Assert.IsNotNull(seqList);
                Assert.AreEqual(expCount, seqList.Count());
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "XsvSparse Parser BVT: Number of Sequences found are '{0}'.",
                    expCount));

                string[] expectedSeqIdArray = expectedSeqIds.Split(',');

                List<ISequence> sparseSeqList = seqList.ToList();
                for (int i = 0; i < expectedSeqIdArray.Length; i++)
                {
                    Assert.AreEqual(expectedSeqIdArray[i], sparseSeqList[i].ID);
                }
            }
            else
            {
                string[] idArray = expectedSeqIds.Split(',');
                Assert.AreEqual(sparseSeq.ID, idArray[0]);
            }

            ApplicationLog.WriteLine(
                "XsvSparse Parser BVT: The XsvSparse sequence is validated successfully with Parse() method.");

            Assert.IsNotNull(sparseSeq.Alphabet);
            Assert.AreEqual(sparseSeq.Alphabet.Name.ToLower(CultureInfo.InvariantCulture),
                utilityObj.xmlUtil.GetTextValue(nodename,
                Constants.AlphabetNameNode).ToLower(CultureInfo.InvariantCulture));

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "XsvSparse Parser BVT: The Sequence Alphabet is '{0}' and is as expected.",
                sparseSeq.Alphabet.Name));
        }

        /// <summary>
        /// XsvSparse formatter generic method called by all the test cases 
        /// to validate the test case based on the parameters passed.
        /// </summary>
        /// <param name="nodename">Xml node Name.</param>
        /// <param name="additionalParam">Additional parameter 
        /// based on which the validation of  test case is done.</param>
        void XsvSparseFormatterGeneralTestCases(string nodename,
            AdditionalParameters additionalParam)
        {
            // Gets the expected sequence from the Xml
            string filePathObj = utilityObj.xmlUtil.GetTextValue(nodename,
                Constants.FilePathNode);

            Assert.IsTrue(File.Exists(filePathObj));
            // Logs information to the log file
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "XsvSparse Formatter BVT: File Exists in the Path '{0}'.",
                filePathObj));

            IEnumerable<ISequence> seqList = null;
            SparseSequence sparseSeq = null;
            XsvContigParser parserObj = new XsvContigParser(filePathObj, Alphabets.DNA,
                Constants.CharSeperator, Constants.SequenceIDPrefix);

            seqList = parserObj.Parse();
            sparseSeq = (SparseSequence)seqList.ElementAt(0);

            IList<IndexedItem<byte>> sparseSeqItems =
               sparseSeq.GetKnownSequenceItems();

            string tempFile = Path.GetTempFileName();

            XsvSparseFormatter formatterObj = new XsvSparseFormatter(tempFile, Constants.CharSeperator,
                Constants.SequenceIDPrefix);

            switch (additionalParam)
            {
                case AdditionalParameters.FormatFilePath:
                    formatterObj.Write(sparseSeq);
                    break;
                default:
                    break;
                case AdditionalParameters.ForamtListWithFilePath:
                    formatterObj.Write(seqList.ToList());
                    break;
            }
            XsvContigParser newParserObj = new XsvContigParser(filePathObj, Alphabets.DNA,
                Constants.CharSeperator, Constants.SequenceIDPrefix);
            // Parse a formatted Xsv file and validate.
            SparseSequence expectedSeq;
            seqList = newParserObj.Parse();
            expectedSeq = (SparseSequence)seqList.ElementAt(0);

            IList<IndexedItem<byte>> expectedSparseSeqItems =
                expectedSeq.GetKnownSequenceItems();

            for (int i = 0; i < sparseSeqItems.Count; i++)
            {
                IndexedItem<byte> seqItem = sparseSeqItems[i];
                IndexedItem<byte> expectedSeqItem = expectedSparseSeqItems[i];
                Assert.AreEqual(seqItem.Index, expectedSeqItem.Index);
            }

            ApplicationLog.WriteLine("Successfully validated the format Xsv file");
        }

        #endregion Supporting Methods
    }
}
