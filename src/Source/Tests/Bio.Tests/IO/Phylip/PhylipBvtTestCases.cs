/****************************************************************************
 * PhylipBvtTestCases.cs
 * 
 *   This file contains the Phylip - Parsers and Formatters Bvt test cases.
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
using Bio.IO;
using Bio.IO.Phylip;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using NUnit.Framework;
using Bio;

namespace Bio.TestAutomation.IO.Phylip
{
    /// <summary>
    /// Phylip Bvt parser and formatter Test case implementation.
    /// </summary>
    [TestFixture]
    public class PhylipBvtTestCases
    {

        #region Enums

        /// <summary>
        /// Additional parameters to validate different scenarios.
        /// </summary>
        enum ParserTestAttributes
        {
            Parse,
            ParseOne,
            ParseTextReader,
            ParseOneTextReader
        };

        /// <summary>
        /// Additional parameters to validate Common Sequence Parser scenarios.
        /// </summary>
        enum CommonSequenceParserAttributes
        {
            ParseRNA,
            ParseProtein
        };

        #endregion Enums

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\PhylipTestsConfig.xml");

        #endregion Global Variables

        #region Phylip Parser BVT Test cases

        /// <summary>
        /// Parse a valid Phylip file (Small size sequence less than 35 kb) and 
        /// convert the same to one sequence using Parse(file-name) method and 
        /// validate with the expected sequence.
        /// Input : Phylip File
        /// Validation: Sequence Alignment list
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PhylipParserValidateParseFileName()
        {
            ParserGeneralTestCases(Constants.SmallSizePhylipNodeName,
                ParserTestAttributes.Parse);
        }

        /// <summary>
        /// Parse a valid Phylip file (Small size sequence less than 35 kb) and 
        /// convert the same to one sequence using ParseOne(file-name) method and 
        /// validate with the expected sequence.
        /// Input : Phylip File
        /// Validation: Sequence Alignment list
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PhylipParserValidateParseOneFileName()
        {
            ParserGeneralTestCases(Constants.SmallSizePhylipNodeName,
                ParserTestAttributes.ParseOne);
        }

        /// <summary>
        /// Parse a valid Phylip file (Small size sequence less than 35 kb) and 
        /// convert the same to one sequence using ParseOne(text-reader) method and 
        /// validate with the expected sequence.
        /// Input : Phylip File
        /// Validation: Sequence Alignment list
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PhylipParserValidateParseOneTextReader()
        {
            ParserGeneralTestCases(Constants.SmallSizePhylipNodeName,
                ParserTestAttributes.ParseOneTextReader);
        }

        /// <summary>
        /// Parse a valid Phylip file (Small size sequence less than 35 kb) and 
        /// convert the same to one sequence using Parse(text-reader) method and 
        /// validate with the expected sequence.
        /// Input : Phylip File
        /// Validation: Sequence Alignment list
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PhylipParserValidateParseTextReader()
        {
            ParserGeneralTestCases(Constants.SmallSizePhylipNodeName,
                ParserTestAttributes.ParseTextReader);
        }

        /// <summary>
        /// Parse a valida Phylip Parser object and validate its properties
        /// Input : Valide Object
        /// Output : Validatation of properties
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidatePhylipParserProperties()
        {
            PhylipParser parser = new PhylipParser();
            Assert.AreEqual(
                utilityObj.xmlUtil.GetTextValue(Constants.PhylipPropertyNode,
                Constants.PhylipDescriptionNode),
                parser.Description);
            Assert.AreEqual(
                utilityObj.xmlUtil.GetTextValue(Constants.PhylipPropertyNode,
                Constants.PhylipNameNode),
                parser.Name);
            Assert.AreEqual(null, parser.Alphabet);
        }

        #endregion Phylip Parser BVT Test cases

        #region Supported Methods

        /// <summary>
        /// Parsers the Phylip file for different test cases based
        /// on Additional parameter
        /// </summary>
        /// <param name="nodeName">Xml Node name</param>
        /// <param name="addParam">Additional parameter</param>
        void ParserGeneralTestCases(string nodeName, ParserTestAttributes addParam)
        {
            // Gets the Filename
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);

            Assert.IsFalse(string.IsNullOrEmpty(filePath));
            ApplicationLog.WriteLine(string.Format("Phylip Parser BVT: Reading the File from location '{0}'", filePath));

            // Get the range list after parsing.
            PhylipParser parserObj = new PhylipParser();

            IEnumerable<ISequenceAlignment> sequenceAlignmentList = null;
            ISequenceAlignment sequenceAlignment = null;

            // Gets the SequenceAlignment list based on the parameters.
            switch (addParam)
            {
                case ParserTestAttributes.Parse:
                    sequenceAlignmentList = parserObj.Parse(filePath);
                    break;
                case ParserTestAttributes.ParseOne:
                    sequenceAlignment = parserObj.ParseOne(filePath);
                    break;
                case ParserTestAttributes.ParseTextReader:
                    using (var rdrObj = File.OpenRead(filePath))
                    {
                        sequenceAlignmentList = parserObj.Parse(rdrObj).ToList();
                    }
                    break;
                case ParserTestAttributes.ParseOneTextReader:
                    using (var rdrObj = File.OpenRead(filePath))
                    {
                        sequenceAlignment = parserObj.ParseOne(rdrObj);
                    }
                    break;
                default:
                    break;
            }

            // Gets all the expected values from xml.
            var expectedAlignmentList = new List<Dictionary<string, string>>();
            var expectedAlignmentObj = new Dictionary<string, string>();

            XElement expectedAlignmentNodes = utilityObj.xmlUtil.GetNode(nodeName, Constants.ExpectedAlignmentNode);
            IList<XNode> nodes = expectedAlignmentNodes.Nodes().ToList();

            //Get all the values from the elements in the node.
            foreach (XElement node in nodes)
                expectedAlignmentObj[node.Name.ToString()] = node.Value;

            // Create a ISequenceAlignment List
            switch (addParam)
            {
                case ParserTestAttributes.ParseOne:
                case ParserTestAttributes.ParseOneTextReader:
                    sequenceAlignmentList = new List<ISequenceAlignment> { sequenceAlignment };
                    break;
                default:
                    break;
            }            

            expectedAlignmentList.Add(expectedAlignmentObj);

            Assert.IsTrue(CompareOutput(sequenceAlignmentList.ToList(), expectedAlignmentList));
            ApplicationLog.WriteLine("Phylip Parser BVT: Successfully validated all the Alignment Sequences");
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
                        ApplicationLog.WriteLine("Failed for " + actualSequence.ID);
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
