/****************************************************************************
 * GffBvtTestCases.cs
 * 
 *   This file contains the Gff - Parsers and Formatters Bvt test cases.
 * 
***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Bio.IO.Gff;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if (SILVERLIGHT == false)
   namespace Bio.TestAutomation.IO.GFF
#else
   namespace Bio.Silverlight.TestAutomation.IO.GFF
#endif
{
    /// <summary>
    /// Gff Bvt parser and formatter Test case implementation.
    /// </summary>
    [TestClass]
    public class GffBvtTestCases
    {

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\GffTestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static GffBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region Gff Parser Bvt Test cases

        /// <summary>
        /// Parse a valid Gff file (Small size sequence less than 35 kb) and convert the 
        /// same to one sequence using Parse(file-name) method and validate with the 
        /// expected sequence.
        /// Input : Gff File
        /// Validation : Features, Expected sequence, Sequence Length, 
        /// Sequence Alphabet, Sequence ID.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void GffParserValidateParseFileName()
        {
            ValidateParseGeneralTestCases(Constants.SimpleGffNodeName, true);
        }

        /// <summary>
        /// Parse a valid Gff file (Small size sequence less than 35 kb) and convert the 
        /// same to one sequence using Parse(Streams) method and validate with the 
        /// expected sequence.
        /// Input : Gff File
        /// Validation : Features, Expected sequence, Sequence Length, 
        /// Sequence Alphabet, Sequence ID.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void GffParserValidateParseFileNameWithStreams()
        {
            ValidateParseWithStreams(Constants.SimpleGffNodeName);
        }

        /// <summary>
        /// Parse a valid Gff file with one line sequence and using Parse(Streams) method and 
        /// validate the expected sequence
        /// Input : Gff File
        /// Validation : Read the Gff file to which the sequence was formatted and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void GffParserValidateParseWithOneLineSequenceWithStreams()
        {
            ValidateParseWithStreams(Constants.OneLineSeqGffNodeName);
        }
        
        /// <summary>
        /// Parse a valid Gff file (Small size sequence less than 35 kb) and convert the 
        /// same to one sequence using Parse(text-reader) method and validate with the 
        /// expected sequence.
        /// Input : Gff File
        /// Validation : Features, Expected sequence, Sequence Length, 
        /// Sequence Alphabet, Sequence ID.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void GffParserValidateParseTextReader()
        {
            ValidateParseGeneralTestCases(Constants.SimpleGffNodeName, false);
        }

        /// <summary>
        /// Parse a valid Gff file (Small size sequence less than 35 kb) and convert the 
        /// same to one sequence using ParseOne(file-name) method and validate with the 
        /// expected sequence.
        /// Input : Gff File
        /// Validation : Features, Expected sequence, Sequence Length, 
        /// Sequence Alphabet, Sequence ID.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void GffParserValidateParseOneFileName()
        {
            ValidateParseOneGeneralTestCases(Constants.SimpleGffNodeName, true);
        }

        /// <summary>
        /// Parse a valid Gff file (Small size sequence less than 35 kb) and convert the 
        /// same to one sequence using ParseOne(text-reader) method and validate with the 
        /// expected sequence.
        /// Input : Gff File
        /// Validation : Features, Expected sequence, Sequence Length, 
        /// Sequence Alphabet, Sequence ID.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void GffParserValidateParseOneTextReader()
        {
            ValidateParseOneGeneralTestCases(Constants.SimpleGffNodeName, false);
        }

        /// <summary>
        /// Parse a valid Gff file with one line sequence and using Parse(file-name) method and 
        /// validate the expected sequence
        /// Input : Gff File
        /// Validation : Read the Gff file to which the sequence was formatted and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void GffParserValidateParseWithOneLineSequence()
        {
            ValidateParseGeneralTestCases(Constants.OneLineSeqGffNodeName, true);
        }

        /// <summary>
        /// Parse a valid Gff file with one line sequence and
        /// using Parse(file-name) method and validate the expected sequence
        /// Input : Gff File
        /// Validation : Read the Gff file to which the sequence was formatted
        /// and validate Features, Sequence, Sequence Count
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void GffParserValidateParseWithOneLineFeatures()
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(
            Constants.SimpleGffFeaturesNode,
            Constants.FilePathNode);
            Assert.IsTrue(File.Exists(filePath));
            IList<ISequence> seqs = null;
            GffParser parserObj = new GffParser(filePath);
            seqs = parserObj.Parse().ToList();
            Sequence originalSequence = (Sequence)seqs[0];
            bool val = ValidateFeatures(originalSequence,
            Constants.OneLineSeqGffNodeName);
            Assert.IsTrue(val);
            filePath = utilityObj.xmlUtil.GetTextValue(
            Constants.SimpleGffFeaturesReaderNode,
            Constants.FilePathNode);
            GffParser parserObj1 = new GffParser(filePath);
            seqs.Add(parserObj1.Parse().FirstOrDefault());
            originalSequence = (Sequence)seqs[0];
            val = ValidateFeatures(originalSequence,
            Constants.OneLineSeqGffNodeName);
            Assert.IsTrue(val);
            ApplicationLog.WriteLine("GFF Parser BVT : All the features validated successfully.");
        }

        /// <summary>
        /// Opens a file.
        /// Input : File containing Gff Sequence.
        /// Validation : Validated successfull opening of Gff file .
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void GffParserValidateOpen()
        {
            string filename = utilityObj.xmlUtil.GetTextValue(Constants.OneLineSeqGffNodeName,
                Constants.FilePathNode);

            using (GffParser parser = new GffParser())
            {
                try
                {
                    parser.Open(filename);
                }
                catch (System.IO.IOException exception)
                {
                    Assert.Fail("Exception thrown on opening a file " + exception.Message);
                }
            }

            ApplicationLog.WriteLine("Opened the file successfully");

        }

        #endregion Gff Parser BVT Test cases

        #region Gff Formatter BVT Test cases

        /// <summary>
        /// Format a valid Single Sequence (Small size sequence less than 35 kb) to a 
        /// Gff file Format(sequence, filename) method with Sequence and Writer as parameter
        /// and validate the same.
        /// Input : Gff Sequence
        /// Validation : Read the Gff file to which the sequence was formatted and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void GffFormatterValidateFormatFileName()
        {
            ValidateFormatGeneralTestCases(Constants.SimpleGffNodeName, true, false);
        }

        /// <summary>
        /// Format a valid Single Sequence (Small size sequence less than 35 kb) to a 
        /// Gff file Format(sequence list, filename) method with Sequence and Writer as parameter
        /// and validate the same.
        /// Input : Gff Sequence list
        /// Validation : Read the Gff file to which the sequence was formatted and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void GffFormatterValidateFormatSequenceListFileName()
        {
            ValidateFormatGeneralTestCases(Constants.SimpleGffNodeName, true, true);
        }

        /// <summary>
        /// Format a valid Single Sequence (Small size sequence less than 35 kb) to a 
        /// Gff file FormatString() method with Sequence and Writer as parameter
        /// and validate the same.
        /// Input : Gff Sequence
        /// Validation : Read the Gff file to which the sequence was formatted and 
        /// validate Features, Sequence, Sequence Count
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void GffFormatterValidateFormatString()
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(Constants.SimpleGffNodeName,
                Constants.FilePathNode);

            Assert.IsTrue(File.Exists(filePath));
            IList<ISequence> seqs = null;
            GffParser parserObj = new GffParser(filePath);
            seqs = parserObj.Parse().ToList();
            ISequence originalSequence = (Sequence)seqs[0];

            // Use the formatter to write the original sequences to a temp file            
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Formatter BVT: Creating the Temp file '{0}'.", Constants.GffTempFileName));

            GffFormatter formatter = new GffFormatter();
            formatter.ShouldWriteSequenceData = true;
            string formatString = formatter.FormatString(originalSequence);

            string expectedString = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGffNodeName, Constants.FormatStringNode);

            expectedString = expectedString.Replace("current-date",
                DateTime.Today.ToString("yyyy-MM-dd", null));
            expectedString =
                expectedString.Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace("\t", "").ToUpper(CultureInfo.CurrentCulture);
            string modifedformatString =
                formatString.Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace("\t", "").ToUpper(CultureInfo.CurrentCulture);

            Assert.AreEqual(modifedformatString, expectedString);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Formatter BVT: The Gff Format String '{0}' are matching with FormatString() method and is as expected.",
                formatString));

            // Passed all the tests, delete the tmp file. If we failed an Assert,
            // the tmp file will still be there in case we need it for debugging.
            File.Delete(Constants.GffTempFileName);
            ApplicationLog.WriteLine("Deleted the temp file created.");
        }
        
        /// <summary>
        /// Opens a file.
        /// Input : File containing Gff Sequence.
        /// Validation : Validated successfull opening of Gff file .
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void GffFormatterValidateOpen()
        {
            using (GffFormatter formatter = new GffFormatter())
            {
                try
                {
                    formatter.Open(Constants.GffTempFileName);
                }
                catch (System.IO.IOException exception)
                {
                    Assert.Fail("Exception thrown on opening a file " + exception.Message);
                }
            }           
            
            ApplicationLog.WriteLine("Opened the file successfully");            
        }

        /// <summary>
        /// Validates the Write method in Gff Formatter based on the parameters and using an Input stream.
        /// </summary>                
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void GffFormatterValidateStreams()            
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(Constants.SimpleGffNodeName,
                Constants.FilePathNode);
            Assert.IsTrue(File.Exists(filePath));
            IList<ISequence> seqs = null;
            Sequence originalSequence = null;
            
            using (GffParser parserObj = new GffParser(filePath))
            {
                seqs = parserObj.Parse().ToList();
                originalSequence = (Sequence)seqs[0];
            }                        

            // Use the formatter to write the original sequences to a temp file            
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Formatter BVT: Creating the Temp file '{0}'.",
                Constants.GffTempFileName));

            using (StreamWriter writer = new StreamWriter(Constants.GffTempFileName))
            {
                using (GffFormatter formatter = new GffFormatter())
                {
                    formatter.ShouldWriteSequenceData = true;
                    formatter.Open(writer);
                    formatter.Write(originalSequence);
                }
            }            

            // Read the new file, then compare the sequences
            IList<ISequence> seqsNew = null;

            using (GffParser newParser = new GffParser(Constants.GffTempFileName))
            {
                seqsNew = newParser.Parse().ToList();
            }

            Assert.IsNotNull(seqsNew);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Formatter BVT: New Sequence is '{0}'.",
                seqsNew[0].ToString()));

            bool val = ValidateFeatures(seqsNew[0], Constants.SimpleGffNodeName);
            Assert.IsTrue(val);
            ApplicationLog.WriteLine(
                "GFF Formatter BVT : All the features validated successfully.");

            // Now compare the sequences.
            int countNew = seqsNew.Count();
            int expectedCount = 1;
            Assert.AreEqual(expectedCount, countNew);
            ApplicationLog.WriteLine("The Number of sequences are matching.");

            Assert.AreEqual(originalSequence.ID, seqsNew[0].ID);            
            ISequence newSeq = seqsNew.FirstOrDefault();           
            Assert.AreEqual(new string(originalSequence.Select(x => (char)x).ToArray()),
                            new string(newSeq.Select(x => (char)x).ToArray()));
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Formatter BVT: The Gff sequences '{0}' are matching with Write() method.",
                seqsNew[0].ToString()));

            // Passed all the tests, delete the tmp file. If we failed an Assert,
            // the tmp file will still be there in case we need it for debugging.
            if (File.Exists(Constants.GffTempFileName))
            {
                File.Delete(Constants.GffTempFileName);
            }

            ApplicationLog.WriteLine("Deleted the temp file created.");
        }

        #endregion Gff Formatter BVT Test cases

        #region Supported Methods

        /// <summary>
        /// Parses all test cases related to Parse() method based on the
        /// parameters passed and validates the same.
        /// </summary>
        /// <param name="nodeName">Xml Node name to be read.</param>
        /// <param name="isFilePath">Is file path passed as parameter?</param>
        void ValidateParseGeneralTestCases(string nodeName, bool isFilePath)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.FilePathNode);

            Assert.IsTrue(File.Exists(filePath));

            // Logs information to the log file
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser BVT : File Exists in the Path '{0}'.", filePath));

            IList<ISequence> seqs = null;
            GffParser parserObj = new GffParser(filePath);

            if (isFilePath)
            {
                seqs = parserObj.Parse().ToList();
            }
            else
            {
                using (StreamReader reader = File.OpenText(filePath))
                {
                    seqs = parserObj.Parse().ToList();
                }
            }
            int expectedSequenceCount = 1;
            Assert.IsNotNull(seqs);
            Assert.AreEqual(expectedSequenceCount, seqs.Count);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser BVT : Number of Sequences found are '{0}'.",
                seqs.Count.ToString((IFormatProvider)null)));

            Assert.IsTrue(ValidateFeatures(seqs[0], nodeName));
            ApplicationLog.WriteLine(
                "Gff Parser BVT : Successfully validated all the Features for a give Sequence in GFF File.");

            string expectedSequence = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.ExpectedSequenceNode);

            Sequence seq = (Sequence)seqs[0];
            Assert.IsNotNull(seq);
            byte[] TempSeqData = new byte[seq.Count];

            for (int i = 0; i < seq.Count; i++)
            {
                TempSeqData[i] = seq[i];
            }
            string sequenceInString = new string(TempSeqData.Select(x => (char)x).ToArray());

            Assert.AreEqual(expectedSequence, sequenceInString);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser BVT: The Gff sequence '{0}' validation after Parse() is found to be as expected.",
                seq.ToString()));

            byte[] tmpEncodedSeq = new byte[seq.Count];
            for (int i = 0; i < seq.Count; i++)
            {
                tmpEncodedSeq[i] = seq[i];
            }
            Assert.AreEqual(expectedSequence.Length, tmpEncodedSeq.Length);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser BVT: The Gff Length sequence '{0}' is as expected.",
                expectedSequence.Length));

            string expectedAlphabet = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.AlphabetNameNode).ToLower(CultureInfo.CurrentCulture);
            Assert.IsNotNull(seq.Alphabet);
            Assert.AreEqual(seq.Alphabet.Name.ToLower(CultureInfo.CurrentCulture),
                expectedAlphabet);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser BVT: The Sequence Alphabet is '{0}' and is as expected.",
                seq.Alphabet.Name));

            string expectedSequenceId = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.SequenceIdNode);
            Assert.AreEqual(expectedSequenceId, seq.ID);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser BVT: The Sequence ID is '{0}' and is as expected.",
                seq.ID));
        }

        /// <summary>
        /// Parses all test cases related to ParseOne() method based on the 
        /// parameters passed and validates the same.
        /// </summary>
        /// <param name="nodeName">Xml Node name to be read.</param>
        /// <param name="isFilePath">Is file path passed as parameter?</param>
        void ValidateParseOneGeneralTestCases(string nodeName,
            bool isFilePath)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.FilePathNode);

            Assert.IsTrue(File.Exists(filePath));

            // Logs information to the log file
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser BVT : File Exists in the Path '{0}'.", filePath));

            ISequence originalSeq = null;
            GffParser parserObj = new GffParser(filePath);

            if (isFilePath)
            {
                originalSeq = parserObj.Parse().FirstOrDefault();
            }
            else
            {
                using (StreamReader reader = File.OpenText(filePath))
                {
                    originalSeq = parserObj.Parse().FirstOrDefault();
                }
            }

            Assert.IsNotNull(originalSeq);
            Assert.IsTrue(ValidateFeatures(originalSeq, nodeName));
            ApplicationLog.WriteLine(
                "Gff Parser BVT : Successfully validated all the Features for a give Sequence in GFF File.");

            string expectedSequence = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.ExpectedSequenceNode);

            Sequence seq = (Sequence)originalSeq;
            Assert.IsNotNull(seq);                       
            string sequenceInString = new string(seq.Select(x => (char)x).ToArray());

            Assert.AreEqual(expectedSequence, sequenceInString);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser BVT: The Gff sequence '{0}' validation after ParseOne() is found to be as expected.",
                seq.ToString()));

            byte[] tmpEncodedSeq = new byte[seq.Count];
            for (int i = 0; i < seq.Count; i++)
            {
                tmpEncodedSeq[i] = seq[i];
            }
            Assert.AreEqual(expectedSequence.Length, tmpEncodedSeq.Length);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser BVT: The Gff Length sequence '{0}' is as expected.",
                expectedSequence.Length));

            string expectedAlphabet = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.AlphabetNameNode).ToLower(CultureInfo.CurrentCulture);

            Assert.IsNotNull(seq.Alphabet);
            Assert.AreEqual(seq.Alphabet.Name.ToLower(CultureInfo.CurrentCulture),
                expectedAlphabet);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser BVT: The Sequence Alphabet is '{0}' and is as expected.",
                seq.Alphabet.Name));

            string expectedSequenceId = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.SequenceIdNode);

            Assert.AreEqual(expectedSequenceId, seq.ID);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser BVT: The Sequence ID is '{0}' and is as expected.",
                seq.ID));
        }

        /// <summary>
        /// Validates the Format() method in Gff Formatter based on the parameters.
        /// </summary>
        /// <param name="nodeName">Xml Node name to be read.</param>
        /// <param name="isFilePath">Is file path passed as parameter?</param>
        /// <param name="isSequenceList">Is sequence list passed as parameter?</param>
        void ValidateFormatGeneralTestCases(string nodeName,
            bool isFilePath, bool isSequenceList)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.FilePathNode);

            Assert.IsTrue(File.Exists(filePath));
            IList<ISequence> seqs = null;
            GffParser parserObj = new GffParser(filePath);
            seqs = parserObj.Parse().ToList();
            Sequence originalSequence = (Sequence)seqs[0];

            // Use the formatter to write the original sequences to a temp file            
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Formatter BVT: Creating the Temp file '{0}'.",
                Constants.GffTempFileName));

            GffFormatter formatter = new GffFormatter(Constants.GffTempFileName);
            formatter.ShouldWriteSequenceData = true;
            if (isFilePath)
            {
                if (isSequenceList)
                    formatter.Write(seqs);
                else
                    formatter.Write(originalSequence);
            }
            else
            {
                if (isSequenceList)
                {
                    formatter.Write(seqs);
                }
                else
                {
                    formatter.Write(originalSequence);
                }
            }

            // Read the new file, then compare the sequences
            IList<ISequence> seqsNew = null;
            using (GffParser newParser = new GffParser(Constants.GffTempFileName))
            {
                seqsNew = newParser.Parse().ToList();
            }
            Assert.IsNotNull(seqsNew);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Formatter BVT: New Sequence is '{0}'.",
                seqsNew[0].ToString()));

            bool val = ValidateFeatures(seqsNew[0], nodeName);
            Assert.IsTrue(val);
            ApplicationLog.WriteLine(
                "GFF Formatter BVT : All the features validated successfully.");

            // Now compare the sequences.
            int countNew = seqsNew.Count();
            int expectedCount = 1;
            Assert.AreEqual(expectedCount, countNew);
            ApplicationLog.WriteLine("The Number of sequences are matching.");

            Assert.AreEqual(originalSequence.ID, seqsNew[0].ID);          

            string orgSeq = new string(originalSequence.Select(x => (char)x).ToArray()); ; 
            ISequence newSeq = seqsNew.FirstOrDefault();           
            string newSeqString = new string(newSeq.Select(x => (char)x).ToArray());

            Assert.AreEqual(orgSeq, newSeqString);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Formatter BVT: The Gff sequences '{0}' are matching with Format() method.",
                seqsNew[0].ToString()));

            // Passed all the tests, delete the tmp file. If we failed an Assert,
            // the tmp file will still be there in case we need it for debugging.
            if (File.Exists(Constants.GffTempFileName))
                File.Delete(Constants.GffTempFileName);
            ApplicationLog.WriteLine("Deleted the temp file created.");
        }

        /// <summary>
        /// Validates the Metadata Features of a Gff Sequence for the sequence and node name specified.
        /// </summary>
        /// <param name="seq">Sequence that needs to be validated.</param>
        /// <param name="nodeName">Xml Node name to be read.</param>
        /// <returns>True/false</returns>
        bool ValidateFeatures(ISequence seq, string nodeName)
        {
            // Gets all the Features from the Sequence for Validation
            List<MetadataListItem<List<string>>> featureList =
                (List<MetadataListItem<List<string>>>)seq.Metadata[Constants.Features];

            // Gets all the xml values for validation
            string[] sequenceNames = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.SequenceNameNodeName);
            string[] sources = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.SourceNodeName);
            string[] featureNames = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.FeatureNameNodeName);
            string[] startValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.StartNodeName);
            string[] endValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.EndNodeName);
            string[] scoreValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.ScoreNodeName);
            string[] strandValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.StrandNodeName);
            string[] frameValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.FrameNodeName);
            string[] attributeValues = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.AttributesNodeName);
            int i = 0;

            // Loop through each and every feature and validate the same.
            foreach (MetadataListItem<List<string>> feature in featureList)
            {
                Dictionary<string, List<string>> itemList = feature.SubItems;

                // Read specific feature Item and validate
                // Validate Start
                try
                {
                    List<string> st = itemList[Constants.FeatureStart];
                    foreach (string sin in st)
                    {
                        if (0 != string.Compare(startValues[i], sin,
                             CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                            return false;
                    }
                }
                catch (KeyNotFoundException) { }

                // Validate Score
                try
                {
                    List<string> st = itemList[Constants.FeatureScore];
                    foreach (string sin in st)
                    {
                        if (0 != string.Compare(scoreValues[i],
                            sin, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                            return false;
                    }
                }
                catch (KeyNotFoundException) { }


                // Validate Strand
                try
                {
                    List<string> st = itemList[Constants.FeatureStrand];
                    foreach (string sin in st)
                    {
                        if (0 != string.Compare(strandValues[i],
                            sin, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                            return false;
                    }
                }
                catch (KeyNotFoundException) { }

                // Validate Source
                try
                {
                    List<string> st = itemList[Constants.FeatureSource];
                    foreach (string sin in st)
                    {
                        if (0 != string.Compare(sources[i],
                            sin, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                            return false;
                    }
                }
                catch (KeyNotFoundException) { }

                // Validate End
                try
                {
                    List<string> st = itemList[Constants.FeatureEnd];
                    foreach (string sin in st)
                    {
                        if (0 != string.Compare(endValues[i],
                            sin, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                            return false;
                    }
                }
                catch (KeyNotFoundException) { }


                // Validate Frame
                try
                {
                    List<string> st = itemList[Constants.FeatureFrame];
                    foreach (string sin in st)
                    {
                        if (0 != string.Compare(frameValues[i],
                            sin, CultureInfo.CurrentCulture,CompareOptions.IgnoreCase))
                            return false;
                    }
                }
                catch (KeyNotFoundException) { }

                if (0 != string.Compare(feature.FreeText, attributeValues[i],
                    CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                    return false;

                if (0 != string.Compare(feature.Key, featureNames[i],
                     CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                    return false;

                if (0 != string.Compare(seq.ID, sequenceNames[i],
                    CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                    return false;

                i++;
            }

            return true;
        }

        /// <summary>
        /// Parses all test cases related to Parse() method based on the
        /// parameters passed and validates the same.
        /// </summary>
        /// <param name="nodeName">Xml Node name to be read.</param>        
        void ValidateParseWithStreams(string nodeName)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.FilePathNode);

            Assert.IsTrue(File.Exists(filePath));

            // Logs information to the log file
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser BVT : File Exists in the Path '{0}'.", filePath));

            IList<ISequence> seqs = null;

            using (StreamReader reader = new StreamReader(filePath))
            {
                using (GffParser parserObj = new GffParser())
                {
                    seqs = parserObj.Parse(reader).ToList();
                }
            }                                       
            
            int expectedSequenceCount = 1;
            Assert.IsNotNull(seqs);
            Assert.AreEqual(expectedSequenceCount, seqs.Count);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser BVT : Number of Sequences found are '{0}'.",
                seqs.Count.ToString((IFormatProvider)null)));

            Assert.IsTrue(ValidateFeatures(seqs[0], nodeName));
            ApplicationLog.WriteLine(
                "Gff Parser BVT : Successfully validated all the Features for a give Sequence in GFF File.");
            string expectedSequence = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.ExpectedSequenceNode);

            Sequence seq = (Sequence)seqs[0];
            Assert.IsNotNull(seq);
            byte[] TempSeqData = new byte[seq.Count];

            for (int i = 0; i < seq.Count; i++)
            {
                TempSeqData[i] = seq[i];
            }
            string sequenceInString = new string(TempSeqData.Select(x => (char)x).ToArray());

            Assert.AreEqual(expectedSequence, sequenceInString);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser BVT: The Gff sequence '{0}' validation after Parse() is found to be as expected.",
                seq.ToString()));

            byte[] tmpEncodedSeq = new byte[seq.Count];

            for (int i = 0; i < seq.Count; i++)
            {
                tmpEncodedSeq[i] = seq[i];
            }

            Assert.AreEqual(expectedSequence.Length, tmpEncodedSeq.Length);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser BVT: The Gff Length sequence '{0}' is as expected.",
                expectedSequence.Length));

            string expectedAlphabet = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.AlphabetNameNode).ToLower(CultureInfo.CurrentCulture);
            Assert.IsNotNull(seq.Alphabet);
            Assert.AreEqual(seq.Alphabet.Name.ToLower(CultureInfo.CurrentCulture),
                expectedAlphabet);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser BVT: The Sequence Alphabet is '{0}' and is as expected.",
                seq.Alphabet.Name));

            string expectedSequenceId = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.SequenceIdNode);
            Assert.AreEqual(expectedSequenceId, seq.ID);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Gff Parser BVT: The Sequence ID is '{0}' and is as expected.",
                seq.ID));
        }

      #endregion Supported Methods
    }
}
