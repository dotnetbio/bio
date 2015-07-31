/****************************************************************************
 * ClustalWBvtTestCases.cs
 * 
 *   This file contains the ClustalW - Parsers Bvt test cases.
 * 
***************************************************************************/

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Bio.Algorithms.Alignment;
using Bio.IO.ClustalW;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using NUnit.Framework;

namespace Bio.TestAutomation.IO.ClustalW
{
    /// <summary>
    ///     ClustalW Bvt parser Test case implementation.
    /// </summary>
    [TestFixture]
    public class ClustalWBvtTestCases
    {
        #region Enums

        /// <summary>
        ///     Additional parameters to validate different scenarios.
        /// </summary>
        private enum AdditionalParameters
        {
            Parse,
            ParseOne,
            ParseTextReader,
            ParseOneTextReader,
        };

        #endregion Enums

        #region Global Variables

        private readonly Utility utilityObj = new Utility(@"TestUtils\ClustalWTestsConfig.xml");

        #endregion Global Variables

      
        #region ClustalW Parser BVT Test cases

        /// <summary>
        ///     Parse a valid ClustalW file (Small size sequence less than 35 kb) and
        ///     convert the same to one sequence using Parse(file-name) method and
        ///     validate with the expected sequence.
        ///     Input : ClustalW File
        ///     Validation: Sequence Alignment list
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ClustalWParserValidateParseFileName()
        {
            ParserGeneralTestCases(Constants.SmallSizeClustalWNodeName,
                                   AdditionalParameters.Parse);
        }

        /// <summary>
        ///     Parse a valid ClustalW file (Small size sequence less than 35 kb) and
        ///     convert the same to one sequence using ParseOne(file-name) method and
        ///     validate with the expected sequence.
        ///     Input : ClustalW File
        ///     Validation: Sequence Alignment list
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ClustalWParserValidateParseOneFileName()
        {
            ParserGeneralTestCases(Constants.SmallSizeClustalWNodeName,
                                   AdditionalParameters.ParseOne);
        }


        /// <summary>
        ///     Parse a valid ClustalW file (Small size sequence less than 35 kb) and
        ///     convert the same to one sequence using ParseOne(text-reader) method and
        ///     validate with the expected sequence.
        ///     Input : ClustalW File
        ///     Validation: Sequence Alignment list
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ClustalWParserValidateParseOneTextReader()
        {
            ParserGeneralTestCases(Constants.SmallSizeClustalWNodeName,
                                   AdditionalParameters.ParseOneTextReader);
        }

        /// <summary>
        ///     Parse a valid ClustalW file (Small size sequence less than 35 kb) and
        ///     convert the same to one sequence using Parse(text-reader) method and
        ///     validate with the expected sequence.
        ///     Input : ClustalW File
        ///     Validation: Sequence Alignment list
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ClustalWParserValidateParseTextReader()
        {
            ParserGeneralTestCases(Constants.SmallSizeClustalWNodeName,
                                   AdditionalParameters.ParseTextReader);
        }

        /// <summary>
        ///     Validate Description,Name and Supported file types for ClustalW Parsers.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ClustalWParserValidatePublicProperties()
        {
            string description = utilityObj.xmlUtil.GetTextValue(
                Constants.ClustalWDescriptionsNode, Constants.DescriptionNode);

            string name = utilityObj.xmlUtil.GetTextValue(
                Constants.ClustalWDescriptionsNode, Constants.NameNode);

            string supportedFileTypes = utilityObj.xmlUtil.GetTextValue(
                Constants.ClustalWDescriptionsNode, Constants.SupportedFileTypesNode);

            // Get the rangelist after parsing.
            var parserObj = new ClustalWParser();

            //Validate Description for ClustalW Parser. 
            Assert.AreEqual(description, parserObj.Description);
            ApplicationLog.WriteLine(string.Format(null,
                                                   "ClustalW Parser BVT: Successfully validated description"));

            //Validate Name for Clustal W parser.
            Assert.AreEqual(name, parserObj.Name);
            ApplicationLog.WriteLine(string.Format(null,
                                                   "ClustalW Parser BVT: Successfully validated Name"));

            //Validate supported file types for Clustal W parser.
            Assert.AreEqual(supportedFileTypes, parserObj.SupportedFileTypes);
            ApplicationLog.WriteLine(string.Format(null,
                                                   "ClustalW Parser BVT: Successfully validated Supported File types."));
        }

        #endregion ClustalW Parser BVT Test cases

        #region Supported Methods

        /// <summary>
        ///     Parsers the ClustalW file for different test cases based
        ///     on Additional parameter
        /// </summary>
        /// <param name="nodeName">Xml Node name</param>
        /// <param name="addParam">Additional parameter</param>
        private void ParserGeneralTestCases(string nodeName, AdditionalParameters addParam)
        {
            // Gets the Filename
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);

            Assert.IsFalse(string.IsNullOrEmpty(filePath));
            ApplicationLog.WriteLine(string.Format("ClustalW Parser BVT: Reading the File from location '{0}'", filePath));

            // Get the range list after parsing.
            var parserObj = new ClustalWParser();

            IEnumerable<ISequenceAlignment> sequenceAlignmentList = null;
            ISequenceAlignment sequenceAlignment = null;

            // Gets the SequenceAlignment list based on the parameters.
            switch (addParam)
            {
                case AdditionalParameters.Parse:
                    sequenceAlignmentList = parserObj.Parse(filePath).ToList();
                    break;
                case AdditionalParameters.ParseOne:
                    sequenceAlignment = parserObj.ParseOne(filePath);
                    break;
                case AdditionalParameters.ParseTextReader:
                    using (var strRdrObj = File.OpenRead(filePath))
                    {
                        sequenceAlignmentList = parserObj.Parse(strRdrObj).ToList();
                    }
                    break;
                case AdditionalParameters.ParseOneTextReader:
                    using (var strRdrObj = File.OpenRead(filePath))
                    {
                        sequenceAlignment = parserObj.ParseOne(strRdrObj);
                    }
                    break;
                default:
                    break;
            }

            // Gets all the expected values from xml.
            XElement expectedAlignmentNodes = utilityObj.xmlUtil.GetNode(nodeName, Constants.ExpectedAlignmentNode);
            IList<XNode> nodes = expectedAlignmentNodes.Nodes().ToList();

            //Get all the values from the elements in the node.
            var expectedAlignmentObj = new Dictionary<string, string>();
            foreach (XElement node in nodes)
            {
                expectedAlignmentObj[node.Name.ToString()] = node.Value;
            }

            //Create a ISequenceAlignment List
            switch (addParam)
            {
                case AdditionalParameters.ParseOne:
                case AdditionalParameters.ParseOneTextReader:
                    sequenceAlignmentList = new List<ISequenceAlignment>() { sequenceAlignment };
                    break;
                default:
                    break;
            }

            var expectedAlignmentList = new List<Dictionary<string, string>> { expectedAlignmentObj };

            Assert.IsTrue(CompareOutput(sequenceAlignmentList.ToList(), expectedAlignmentList));
            ApplicationLog.WriteLine("ClustalW Parser BVT: Successfully validated all the Alignment Sequences");
        }

        /// <summary>
        ///     Compare the actual output with expected output
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

                if (alignment.AlignedSequences[0].Sequences.Cast<Sequence>().Any(actualSequence => 0 != string.Compare(new string(actualSequence.Select(a => (char) a).ToArray()),
                                                                                                             expectedAlignment[actualSequence.ID], true,
                                                                                                             CultureInfo.CurrentCulture)))
                {
                    return false;
                }

                alignmentIndex++;
            }

            return true;
        }

        #endregion Supported Methods
    }
}