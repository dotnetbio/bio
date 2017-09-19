/****************************************************************************
 * SnpBvtTestCases.cs
 * 
 *   This file contains the Snp - Parsers Bvt test cases.
 * 
***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Bio.IO.Snp;
using Bio.TestAutomation.Util;
using Bio.Tests;
using Bio.Util.Logging;
using NUnit.Framework;

namespace Bio.TestAutomation.IO.Snp
{
    /// <summary>
    /// Snp Bvt parser Test case implementation.
    /// </summary>
    [TestFixture]
    public class SnpBvtTestCases
    {
        #region Enums

        /// <summary>
        /// Additional Parameters which are used for different test cases 
        /// based on which the test cases are executed.
        /// </summary>
        enum AdditionalParameters
        {
            ParseFilePath,
            ParseAlleleTwo
        };

        #endregion Enums

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

       
        #region Test cases

        /// <summary>
        /// Parse a valid Snp file (Small size sequence less than 35 kb) and convert the 
        /// same to one sequence using Parse(file-name) method and validate with the 
        /// expected sequence.
        /// Input : Snp File
        /// Validation : Expected sequence, Chromosome position, Sequence Alphabet, Sequence ID.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SnpBvtParserValidateParseFilePath()
        {
            SnpParserGeneralTestCases(Constants.SimpleSnpNodeName,
                AdditionalParameters.ParseFilePath);
        }

        /// <summary>
        /// Validate All properties in Snp parser class
        /// Input : One line sequence and update all properties
        /// Validation : Validate the properties
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SnpBvtParserProperties()
        {
            string filepath = utilityObj.xmlUtil.GetTextValue(Constants.SimpleSnpNodeName,
               Constants.FilePathNode);
            SimpleSnpParser snpParser = new SimpleSnpParser();
            Assert.AreEqual(Constants.SnpDescription, snpParser.Description);
            Assert.AreEqual(Constants.SnpFileTypes, snpParser.SupportedFileTypes);
            Assert.AreEqual(Constants.SnpName, snpParser.Name);
            Assert.IsTrue(snpParser.ParseAlleleOne);
            Assert.AreEqual(Constants.SnpFileTypes, snpParser.SupportedFileTypes);

            ApplicationLog.WriteLine
                ("Successfully validated all the properties of Snp Parser class.");
        }

        /// <summary>
        /// Parse a valid Snp file and convert the same to one sequence 
        /// using Parse(file-name) method, with ParseAlleleOne property set to false
        /// and validate with the expected sequence.
        /// Input : Snp File
        /// Validation : Expected sequence, Chromosome position, Sequence Alphabet, Sequence ID.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SnpBvtParserValidateParseAlleleTwo()
        {
            SnpParserGeneralTestCases(Constants.SimpleSnpNodeName,
                AdditionalParameters.ParseAlleleTwo);
        }

        /// <summary>
        /// Parse a valid Snp file with one line and convert the 
        /// same to sequence using Parse(file-name) method and validate with the 
        /// expected sequence.
        /// Input : Snp File
        /// Validation : Expected sequence, Chromosome position, Sequence Alphabet, Sequence ID.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SnpBvtParserValidateParseFilePathOneLine()
        {
            SnpParserGeneralTestCases(Constants.OneLineSnpNode,
                AdditionalParameters.ParseFilePath);
        }

        /// <summary>
        /// Validate Equals(snpItem) method with valid values.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SnpItemBvtValidateEqualsSnpItem()
        {
            SnpItem snpItem1 = new SnpItem();
            snpItem1.AlleleOne = 'A';
            snpItem1.AlleleTwo = 'G';

            SnpItem snpItem2 = new SnpItem();
            snpItem2.AlleleOne = 'A';
            snpItem2.AlleleTwo = 'G';

            Assert.IsTrue(snpItem2.Equals(snpItem1));
            ApplicationLog.WriteLine("Snp Bvt : Successfully validated Equals(snpitem).");
        }

        /// <summary>
        /// Validate Equals(object) method with valid values.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SnpItemBvtValidateEqualsObject()
        {
            SnpItem snpItem1 = new SnpItem();
            snpItem1.AlleleOne = 'A';
            snpItem1.AlleleTwo = 'G';

            SnpItem snpItem2 = new SnpItem();
            snpItem2.AlleleOne = 'A';
            snpItem2.AlleleTwo = 'G';

            Assert.IsTrue(snpItem2.Equals((object)snpItem1));
            ApplicationLog.WriteLine("Snp Bvt : Successfully validated Equals(object).");
        }

        /// <summary>
        /// Validate GetHashCode() method with valid values.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SnpItemBvtValidateGetHashCode()
        {
            SnpItem snpItem1 = new SnpItem();
            snpItem1.AlleleOne = 'A';
            snpItem1.AlleleTwo = 'G';

            Assert.AreEqual(0, snpItem1.GetHashCode());
            ApplicationLog.WriteLine("Snp Bvt : Successfully validated GetHashCode().");
        }

        #endregion Test cases

        #region Supporting Methods

        /// <summary>
        /// Snp parser generic method called by all the test cases 
        /// to validate the test case based on the parameters passed.
        /// </summary>
        /// <param name="nodename">Xml node Name.</param>
        /// <param name="additionalParam">Additional parameter 
        /// based on which the validation of  test case is done.</param>
        void SnpParserGeneralTestCases(string nodename, AdditionalParameters additionalParam)
        {
            // Gets the expected sequence from the Xml
            string filepath = utilityObj.xmlUtil.GetTextValue(nodename,
                Constants.FilePathNode).TestDir();

            Assert.IsTrue(File.Exists(filepath));
            // Logs information to the log file
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Snp Parser BVT: File Exists in the Path '{0}'.",
                filepath));

            IList<ISequence> seqList = null;
            SparseSequence sparseSeq = null;
            SimpleSnpParser parser = new SimpleSnpParser();

            string expectedPosition = utilityObj.xmlUtil.GetTextValue(nodename,
                Constants.ExpectedPositionNode);

            string[] expectedPositions = expectedPosition.Split(',');
            string[] expectedCharacters = null;

            switch (additionalParam)
            {
                case AdditionalParameters.ParseAlleleTwo:
                    parser.ParseAlleleOne = false;
                    string expectedAlleleTwoSequence =
                        utilityObj.xmlUtil.GetTextValue(nodename,
                        Constants.ExpectedSequenceAllele2Node);
                    expectedCharacters = expectedAlleleTwoSequence.Split(',');
                    break;
                default:
                    string expectedSequence = utilityObj.xmlUtil.GetTextValue(nodename,
              Constants.ExpectedSequenceNode);
                    expectedCharacters = expectedSequence.Split(',');
                    break;
            }

            seqList = parser.Parse(filepath).ToList();
            sparseSeq = (SparseSequence)seqList[0];


            if (null == sparseSeq)
            {
                Assert.IsNotNull(seqList);
                Assert.AreEqual(1, seqList.Count);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "Snp Parser BVT: Number of Sequences found are '{0}'.",
                    seqList.Count.ToString((IFormatProvider)null)));
            }

            for (int i = 0; i < expectedPositions.Length; i++)
            {
                byte item = sparseSeq[int.Parse(expectedPositions[i], (IFormatProvider)null)];

                ASCIIEncoding enc = new ASCIIEncoding();

                Assert.AreEqual(enc.GetBytes(expectedCharacters[i])[0].ToString((IFormatProvider)null),
                    item.ToString((IFormatProvider)null));
            }

            ApplicationLog.WriteLine(
                "Snp Parser BVT: The Snp sequence with position is validated successfully with Parse() method.");

            Assert.IsNotNull(sparseSeq.Alphabet);
            Assert.IsTrue(sparseSeq.Alphabet.Name.ToLower(CultureInfo.CurrentCulture).Contains(utilityObj.xmlUtil.GetTextValue(nodename,
                Constants.AlphabetNameNode).ToLower(CultureInfo.CurrentCulture)));

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Snp Parser BVT: The Sequence Alphabet is '{0}' and is as expected.",
                sparseSeq.Alphabet.Name));

            string expSequenceID = utilityObj.xmlUtil.GetTextValue(nodename,
                Constants.SequenceIdNode);

            Assert.AreEqual(expSequenceID, sparseSeq.ID);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Snp Parser BVT: The Sequence ID is '{0}' and is as expected.", sparseSeq.ID));
        }

        #endregion Supporting Methods
    }
}
