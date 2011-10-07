/****************************************************************************
 * NexusBvtTestCases.cs
 * 
 *   This file contains the Nexus - Parsers and Formatters Bvt test cases.
 * 
***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Linq;
using System.Xml.Linq;

using Bio.Algorithms.Alignment;
using Bio.IO.Nexus;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio;

namespace Bio.TestAutomation.IO.Nexus
{
    /// <summary>
    /// Nexus Bvt parser and formatter Test case implementation.
    /// </summary>
    [TestClass]
    public class NexusBvtTestCases
    {

        #region Enums

        /// <summary>
        /// Additional parameters to validate different scenarios.
        /// </summary>
        enum AdditionalParameters
        {
            Parse,
            ParseOne,
            ParseTextReader,
            ParseOneTextReader,
        };

        #endregion Enums

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\NexusTestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static NexusBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region Nexus Parser BVT Test cases

        /// <summary>
        /// Parse a valid Nexus file (Small size sequence less than 35 kb) and 
        /// convert the same to one sequence using Parse(file-name) method and 
        /// validate with the expected sequence.
        /// Input : Nexus File
        /// Validation: Sequence Alignment list
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NexusParserValidateParseFileName()
        {
            ParserGeneralTestCases(Constants.SmallSizeNexusNodeName,
                AdditionalParameters.Parse);
        }

        /// <summary>
        /// Parse a valid Nexus file (Small size sequence less than 35 kb) and 
        /// convert the same to one sequence using ParseOne(file-name) method and 
        /// validate with the expected sequence.
        /// Input : Nexus File
        /// Validation: Sequence Alignment list
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NexusParserValidateParseOneFileName()
        {
            ParserGeneralTestCases(Constants.SmallSizeNexusNodeName,
                AdditionalParameters.ParseOne);
        }

        /// <summary>
        /// Parse a valid Nexus file (Small size sequence less than 35 kb) and 
        /// convert the same to one sequence using ParseOne(text-reader) method and 
        /// validate with the expected sequence.
        /// Input : Nexus File
        /// Validation: Sequence Alignment list
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NexusParserValidateParseOneTextReader()
        {
            ParserGeneralTestCases(Constants.SmallSizeNexusNodeName,
                AdditionalParameters.ParseOneTextReader);
        }

        /// <summary>
        /// Parse a valid Nexus file (Small size sequence less than 35 kb) and 
        /// convert the same to one sequence using Parse(text-reader) method and 
        /// validate with the expected sequence.
        /// Input : Nexus File
        /// Validation: Sequence Alignment list
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NexusParserValidateParseTextReader()
        {
            ParserGeneralTestCases(Constants.SmallSizeNexusNodeName,
                AdditionalParameters.ParseTextReader);
        }

        /// <summary>
        /// Parse a valida Nexus Parser object and validate its properties
        /// Input : Valide Object
        /// Output : Validatation of properties
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNexusParserProperties()
        {
            NexusParser parser = new NexusParser();
            Assert.AreEqual(
                utilityObj.xmlUtil.GetTextValue(Constants.NexusPropertyNode,
                Constants.NexusDescriptionNode),
                parser.Description);
            Assert.AreEqual(
                utilityObj.xmlUtil.GetTextValue(Constants.NexusPropertyNode,
                Constants.NexusNameNode),
                parser.Name);
            Assert.AreEqual(null, parser.Alphabet);
        }

        /// <summary>
        /// Parse a valid Nexus file and validate char block
        /// Input : Valid File
        /// Output: Expected sequence alignment count
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NexusParserValidateCharBlock()
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(
                    Constants.SimpleNexusCharBlockNode,
                    Constants.FilePathNode);
            NexusParser parser = new NexusParser();

            IList<ISequenceAlignment> alignment =
                parser.Parse(filePath);

            Assert.AreEqual(1, alignment.Count);
        }

        #endregion Nexus Parser BVT Test cases

        #region Supported Methods

        /// <summary>
        /// Parsers the Nexus file for different test cases based
        /// on Additional parameter
        /// </summary>
        /// <param name="nodeName">Xml Node name</param>
        /// <param name="addParam">Additional parameter</param>
        void ParserGeneralTestCases(string nodeName,
            AdditionalParameters addParam)
        {
            // Gets the Filename
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);

            Assert.IsFalse(string.IsNullOrEmpty(filePath));
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Nexus Parser BVT: Reading the File from location '{0}'", filePath));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "Nexus Parser BVT: Reading the File from location '{0}'", filePath));

            // Get the rangelist after parsing.
            NexusParser parserObj = new NexusParser();

            IList<ISequenceAlignment> sequenceAlignmentList = null;
            ISequenceAlignment sequenceAlignment = null;

            // Gets the SequenceAlignment list based on the parameters.
            switch (addParam)
            {
                case AdditionalParameters.Parse:
                    sequenceAlignmentList = parserObj.Parse(filePath);
                    break;
                case AdditionalParameters.ParseOne:
                    sequenceAlignment = parserObj.ParseOne(filePath);
                    break;
                case AdditionalParameters.ParseTextReader:
                    using (StreamReader strRdrObj = new StreamReader(filePath))
                    {
                        sequenceAlignmentList = parserObj.Parse(strRdrObj);
                    }
                    break;
                case AdditionalParameters.ParseOneTextReader:
                    using (StreamReader strRdrObj = new StreamReader(filePath))
                    {
                        sequenceAlignment = parserObj.ParseOne(strRdrObj);
                    }
                    break;
                default:
                    break;
            }

            // Gets all the expected values from xml.
            IList<Dictionary<string, string>> expectedAlignmentList =
                new List<Dictionary<string, string>>();
            Dictionary<string, string> expectedAlignmentObj =
                new Dictionary<string, string>();

            XElement expectedAlignmentNodes = utilityObj.xmlUtil.GetNode(
                      nodeName, Constants.ExpectedAlignmentNode);
            IEnumerable<XNode> nodes = expectedAlignmentNodes.Nodes();

            //Get all the values from the elements in the node.
            foreach (XElement node in nodes)
            {
                expectedAlignmentObj[node.Name.ToString()] =
                     node.Value.ToString();
            }

            // Create a ISequenceAlignment List
            switch (addParam)
            {
                case AdditionalParameters.ParseOne:
                case AdditionalParameters.ParseOneTextReader:
                    sequenceAlignmentList = new List<ISequenceAlignment>();
                    sequenceAlignmentList.Add(sequenceAlignment);
                    break;
                default:
                    break;
            }

            expectedAlignmentList.Add(expectedAlignmentObj);

            Assert.IsTrue(CompareOutput(sequenceAlignmentList, expectedAlignmentList));
            ApplicationLog.WriteLine(
                "Nexus Parser BVT: Successfully validated all the Alignment Sequences");
            Console.WriteLine(
                "Nexus Parser BVT: Successfully validated all the Alignment Sequences");
        }

        /// <summary>
        /// Compare the actual output with expected output
        /// </summary>
        /// <param name="actualOutput">Actual output</param>
        /// <param name="expectedOutput">Expected output</param>
        /// <returns>True, if comparison is successful</returns>
        private static bool CompareOutput(
                 IList<ISequenceAlignment> actualOutput,
                 IList<Dictionary<string, string>> expectedOutput)
        {
            if (expectedOutput.Count != actualOutput.Count)
            {
                return false;
            }

            int alignmentIndex = 0;

            // Validate each output alignment
            foreach (ISequenceAlignment alignment in actualOutput)
            {
                Dictionary<string, string> expectedAlignment =
                    expectedOutput[alignmentIndex];

                foreach (Sequence actualSequence in alignment.AlignedSequences[0].Sequences)
                {
                    if (0 != string.Compare(new String(actualSequence.Select(a => (char)a).ToArray()),
                            expectedAlignment[actualSequence.ID], true,
                            CultureInfo.CurrentCulture))
                    {
                        return false;
                    }
                }

                alignmentIndex++;
            }

            return true;
        }

        #endregion Supported Methods
    }
}
