/****************************************************************************
 * BedBvtTestCases.cs
 * 
 *   This file contains the Bed - Parsers and Formatters Bvt test cases.
 * 
***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Bio.IO.Bed;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using NUnit.Framework;

#if (SILVERLIGHT == false)
namespace Bio.TestAutomation.IO.Bed
#else
namespace Bio.Silverlight.TestAutomation.IO.Bed
#endif

{
    /// <summary>
    ///     Bed Bvt parser and formatter Test case implementation.
    /// </summary>
    [TestFixture]
    public class BedBvtTestCases
    {
        #region Enums

        /// <summary>
        ///     Additional parameters to validate different scenarios.
        /// </summary>
        private enum AdditionalParameters
        {
            RangeFileName,
            RangeTextReader,
            RangeGroupFileName,
            RangeTextWriter,
            RangeGroupTextWriter,
            RangeGroupTextReader,
            ParseRange,
            ParseRangeGroup
        };

        #endregion Enums

        #region Global Variables

        private readonly Utility utilityObj = new Utility(@"TestUtils\BedTestsConfig.xml");

        #endregion Global Variables

        #region Bed Parser BVT Test cases

        /// <summary>
        ///     Parse a valid Bed file (Small size sequence less than 35 kb) and
        ///     convert the same Range using ParseRange(file-name) method and
        ///     validate the same
        ///     Input : Bed File
        ///     Validation: Range properties like ID, Start and End.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void BedParserValidateSmallSizeParseRangeFileName()
        {
            ParserGeneralTestCases(Constants.SmallSizeBedNodeName,
                                   AdditionalParameters.RangeFileName);
        }

        /// <summary>
        ///     Parse a valid Bed file (Small size sequence less than 35 kb) and
        ///     convert the same Range using ParseRange(text-reader) method and
        ///     validate the same
        ///     Input : Bed File
        ///     Validation: Range properties like ID, Start and End.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void BedParserValidateSmallSizeParseRangeTextReader()
        {
            ParserGeneralTestCases(Constants.SmallSizeBedNodeName,
                                   AdditionalParameters.RangeTextReader);
        }

        /// <summary>
        ///     Parse a valid Bed file (Small size sequence less than 35 kb) and
        ///     convert the same Range using ParseRangeGrouping(file-name) method and
        ///     validate the same
        ///     Input : Bed File
        ///     Validation: Range properties like ID, Start and End.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void BedParserValidateSmallSizeParseRangeGroupFileName()
        {
            ParserGeneralTestCases(Constants.SmallSizeBedNodeName,
                                   AdditionalParameters.RangeGroupFileName);
        }

        /// <summary>
        ///     Parse a valid Bed file (Small size sequence less than 35 kb) and
        ///     convert the same Range using ParseRangeGrouping(text-reader) method and
        ///     validate the same
        ///     Input : Bed File
        ///     Validation: Range properties like ID, Start and End.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void BedParserValidateSmallSizeParseRangeGroupTextReader()
        {
            ParserGeneralTestCases(Constants.SmallSizeBedNodeName,
                                   AdditionalParameters.RangeGroupTextReader);
        }

        /// <summary>
        ///     Parse a valid Bed file (one line) and
        ///     convert the same Range using ParseRange(file-name) method and
        ///     validate the same
        ///     Input : Bed File
        ///     Validation: Range properties like ID, Start and End.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void BedParserValidateOneLineParseRangeFileName()
        {
            ParserGeneralTestCases(Constants.OneLineBedNodeName,
                                   AdditionalParameters.RangeFileName);
        }

        /// <summary>
        ///     Parse a valid Bed file (one line) and
        ///     convert the same Range using ParseRangeGrouping(file-name) method and
        ///     validate the same
        ///     Input : Bed File
        ///     Validation: Range properties like ID, Start and End.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void BedParserValidateOneLineParseRangeGroupFileName()
        {
            ParserGeneralTestCases(Constants.OneLineBedNodeName,
                                   AdditionalParameters.RangeGroupTextReader);
        }

        /// <summary>
        ///     Validate all the properties of a parser object
        ///     Validation: All properties.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void BedParserValidateAllProperties()
        {
            var parserObj = new BedParser();
            Assert.AreEqual(Constants.BedDescription, parserObj.Description);
            Assert.AreEqual(Constants.BedFileTypes, parserObj.SupportedFileTypes);
            Assert.AreEqual(Constants.BedName, parserObj.Name);
        }

        #endregion Bed Parser BVT Test cases

        #region Bed Formatter BVT Test cases

        /// <summary>
        ///     Format a valid Range (Small size sequence  less than 35 kb) to a
        ///     Bed file using Format(Range, file-path) method and
        ///     validate the same.
        ///     Input : Bed Range
        ///     Validation :  Read the Bed file to which the range was formatted
        ///     using File-Info and Validate Properties like ID, Start and End
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void BedFormatterValidateFormatRangeFileName()
        {
            FormatterGeneralTestCases(Constants.SmallSizeBedNodeName,
                                      AdditionalParameters.RangeFileName);
        }

        /// <summary>
        ///     Format a valid Range (Small size sequence  less than 35 kb) to a
        ///     Bed file using Format(Range, text-writer) method and
        ///     validate the same.
        ///     Input : Bed Range
        ///     Validation :  Read the Bed file to which the range was formatted
        ///     using File-Info and Validate Properties like ID, Start and End
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void BedFormatterValidateFormatRangeTextWriter()
        {
            FormatterGeneralTestCases(Constants.SmallSizeBedNodeName,
                                      AdditionalParameters.RangeTextWriter);
        }

        /// <summary>
        ///     Format a valid Range (Small size sequence  less than 35 kb) to a
        ///     Bed file using Format(RangeGroup, file-path) method and
        ///     validate the same.
        ///     Input : Bed Range
        ///     Validation :  Read the Bed file to which the range was formatted
        ///     using File-Info and Validate Properties like ID, Start and End
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void BedFormatterValidateFormatRangeGroupFileName()
        {
            FormatterGeneralTestCases(Constants.SmallSizeBedNodeName,
                                      AdditionalParameters.RangeGroupFileName);
        }

        /// <summary>
        ///     Format a valid Range (Small size sequence  less than 35 kb) to a
        ///     Bed file using Format(RangeGroup, text-writer) method and
        ///     validate the same.
        ///     Input : Bed Range
        ///     Validation :  Read the Bed file to which the range was formatted
        ///     using File-Info and Validate Properties like ID, Start and End
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void BedFormatterValidateFormatRangeGroupTextWriter()
        {
            FormatterGeneralTestCases(Constants.SmallSizeBedNodeName,
                                      AdditionalParameters.RangeGroupTextWriter);
        }

        /// <summary>
        ///     Format a valid Range (one line file) to a
        ///     Bed file using Format(Range, file-path) method and
        ///     validate the same.
        ///     Input : Bed Range
        ///     Validation :  Read the Bed file to which the range was formatted
        ///     using File-Info and Validate Properties like ID, Start and End
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void BedFormatterValidateOneLineFormatRangeFileName()
        {
            FormatterGeneralTestCases(Constants.OneLineBedNodeName,
                                      AdditionalParameters.RangeFileName);
        }

        /// <summary>
        ///     Format a valid Range (one line file) to a
        ///     Bed file using Format(Range, text-writer) method and
        ///     validate the same.
        ///     Input : Bed Range
        ///     Validation :  Read the Bed file to which the range was formatted
        ///     using File-Info and Validate Properties like ID, Start and End
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void BedFormatterValidateOneLineFormatRangeGroupFileName()
        {
            FormatterGeneralTestCases(Constants.OneLineBedNodeName,
                                      AdditionalParameters.RangeGroupFileName);
        }

        /// <summary>
        ///     Format a valid Range (one line file) to a
        ///     Bed file using Format(Range, file-path) method and
        ///     validate the same.
        ///     Input : Bed Range
        ///     Validation :  Read the Bed file to which the range was formatted
        ///     using File-Info and Validate Properties like ID, Start and End
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void BedFormatterParseValidateOneLineFormatRangeFileName()
        {
            FormatterGeneralTestCases(Constants.OneLineBedNodeName,
                                      AdditionalParameters.ParseRange);
        }

        /// <summary>
        ///     Format a valid Range (one line file) to a
        ///     Bed file using Format(Range, text-writer) method and
        ///     validate the same.
        ///     Input : Bed Range
        ///     Validation :  Read the Bed file to which the range was formatted
        ///     using File-Info and Validate Properties like ID, Start and End
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void BedFormatterValidateParseOneLineFormatRangeGroupFileName()
        {
            FormatterGeneralTestCases(Constants.OneLineBedNodeName,
                                      AdditionalParameters.ParseRangeGroup);
        }

        /// <summary>
        ///     Validate all the properties of a Format object
        ///     Validation: All properties.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void BedFormatterValidateAllProperties()
        {
            var formatterObj = new BedFormatter();
            Assert.AreEqual(Constants.BedDescription, formatterObj.Description);
            Assert.AreEqual(Constants.BedFileTypes, formatterObj.SupportedFileTypes);
            Assert.AreEqual(Constants.BedName, formatterObj.Name);
        }

        /// <summary>
        ///     Format a valid Range list(chromosomes with all metadata info close to 10 MB in size) to a
        ///     Bed file using Format(Range, filepath) method and
        ///     validate all the properties and metadata information.
        ///     Input : Human Reference bed file with all metadata
        ///     Validation :  Read the Bed file to which the range was formatted
        ///     using File-Info and Validate Properties like ID, Start and End and metadata information.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void BedFormatterValidateFormatUsingAllMetadataWithTextWriter()
        {
            FormatterGeneralTestCases(Constants.LargeSizeBedNodeName,
                                      AdditionalParameters.ParseRange);
        }

        /// <summary>
        ///     Format a valid Range list(chromosomes with all metadata info close to 10 MB in size) to a
        ///     Bed file using Format(Range, filepath) method and
        ///     validate all the properties and metadata information.
        ///     Input : Human Reference bed file with all metadata
        ///     Validation :  Read the Bed file to which the range was formatted
        ///     using File-Info and Validate Properties like ID, Start and End and metadata information.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void BedFormatterValidateFormatWithAllMetadata()
        {
            FormatterGeneralTestCases(Constants.LargeSizeBedNodeName,
                                      AdditionalParameters.ParseRange);
        }

        #endregion Bed Formatter BVT Test cases

        #region Supported Methods

        /// <summary>
        ///     Parsers the Bed file for different test cases based
        ///     on Additional parameter
        /// </summary>
        /// <param name="nodeName">Xml Node name</param>
        /// <param name="addParam">Additional parameter</param>
        private void ParserGeneralTestCases(string nodeName,
                                            AdditionalParameters addParam)
        {
            // Gets the Filename
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);

            Assert.IsFalse(string.IsNullOrEmpty(filePath));
            ApplicationLog.WriteLine(string.Format(null,
                                                   "Bed Parser BVT: Reading the File from location '{0}'", filePath));

            // Get the rangelist after parsing.
            var parserObj = new BedParser();

            IList<ISequenceRange> rangeList = null;
            SequenceRangeGrouping rangeGroup = null;

            // Gets the Range list/Range Group based on the parameters.
            switch (addParam)
            {
                case AdditionalParameters.RangeFileName:
                    rangeList = parserObj.ParseRange(filePath);
                    break;
                case AdditionalParameters.RangeTextReader:
                    using (var strObj = File.OpenRead(filePath))
                    {
                        rangeList = parserObj.ParseRange(strObj);
                    }
                    break;
                case AdditionalParameters.RangeGroupFileName:
                    rangeGroup = parserObj.ParseRangeGrouping(filePath);
                    break;
                case AdditionalParameters.RangeGroupTextReader:
                    using (var strObj = File.OpenRead(filePath))
                    {
                        rangeGroup = parserObj.ParseRangeGrouping(strObj);
                    }
                    break;
                default:
                    break;
            }

            // Gets the Range list from Group
            switch (addParam)
            {
                case AdditionalParameters.RangeGroupTextReader:
                case AdditionalParameters.RangeGroupFileName:
                    IEnumerable<string> grpIDsObj = rangeGroup.GroupIDs;
                    string rangeID = string.Empty;

                    foreach (string grpID in grpIDsObj)
                    {
                        rangeID = grpID;
                    }

                    rangeList = rangeGroup.GetGroup(rangeID);
                    break;
                default:
                    break;
            }

            // Gets all the expected values from xml.
            string[] expectedIDs = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.IDNode).Split(',');
            string[] expectedStarts = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.StartNode).Split(',');
            string[] expectedEnds = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.EndNode).Split(',');

            int i = 0;
            // Reads all the ranges with comma seperated for validation
            foreach (ISequenceRange range in rangeList)
            {
                Assert.AreEqual(expectedStarts[i], range.Start.ToString((IFormatProvider) null));
                Assert.AreEqual(expectedEnds[i], range.End.ToString((IFormatProvider) null));
                Assert.AreEqual(expectedIDs[i], range.ID.ToString(null));
                i++;
            }

            ApplicationLog.WriteLine(
                "Bed Parser BVT: Successfully validated the ID, Start and End Ranges");
        }

        /// <summary>
        ///     Formats the Range/RangeGroup for different test cases based
        ///     on Additional parameter
        /// </summary>
        /// <param name="nodeName">Xml Node name</param>
        /// <param name="addParam">Additional parameter</param>
        private void FormatterGeneralTestCases(string nodeName,
                                               AdditionalParameters addParam)
        {
            IList<ISequenceRange> rangeList = new List<ISequenceRange>();
            var rangeGroup = new SequenceRangeGrouping();

            // Gets the file name.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);

            // Condition to check if Parse() happens before Format()
            switch (addParam)
            {
                case AdditionalParameters.ParseRangeGroup:
                    var initialParserGroupObj = new BedParser();
                    rangeGroup =
                        initialParserGroupObj.ParseRangeGrouping(filePath);
                    break;
                case AdditionalParameters.ParseRange:
                    var initialParserObj = new BedParser();
                    rangeList = initialParserObj.ParseRange(filePath);
                    break;
                default:
                    // Gets all the expected values from xml.
                    string expectedID = utilityObj.xmlUtil.GetTextValue(
                        nodeName, Constants.IDNode);
                    string expectedStart = utilityObj.xmlUtil.GetTextValue(
                        nodeName, Constants.StartNode);
                    string expectedEnd = utilityObj.xmlUtil.GetTextValue(
                        nodeName, Constants.EndNode);

                    string[] expectedIDs = expectedID.Split(',');
                    string[] expectedStarts = expectedStart.Split(',');
                    string[] expectedEnds = expectedEnd.Split(',');

                    // Gets the Range Group or Range based on the additional parameter
                    switch (addParam)
                    {
                        case AdditionalParameters.RangeGroupTextWriter:
                        case AdditionalParameters.RangeGroupFileName:
                            for (int i = 0; i < expectedIDs.Length; i++)
                            {
                                var rangeObj1 = new SequenceRange(expectedIDs[i],
                                                                  long.Parse(expectedStarts[i], null),
                                                                  long.Parse(expectedEnds[i], null));
                                rangeGroup.Add(rangeObj1);
                            }
                            break;
                        default:
                            for (int i = 0; i < expectedIDs.Length; i++)
                            {
                                var rangeObj2 = new SequenceRange(expectedIDs[i],
                                                                  long.Parse(expectedStarts[i], null),
                                                                  long.Parse(expectedEnds[i], null));
                                rangeList.Add(rangeObj2);
                            }
                            break;
                    }
                    break;
            }

            var formatterObj = new BedFormatter();

            // Gets the Range list/Range Group based on the parameters.
            switch (addParam)
            {
                case AdditionalParameters.RangeFileName:
                case AdditionalParameters.ParseRange:
                    formatterObj.Format(rangeList, Constants.BedTempFileName);
                    break;
                case AdditionalParameters.RangeTextWriter:
                    using (var txtWriter =
                        File.Create(Constants.BedTempFileName))
                    {
                        formatterObj.Format(txtWriter, rangeList);
                    }
                    break;
                case AdditionalParameters.RangeGroupFileName:
                case AdditionalParameters.ParseRangeGroup:
                    formatterObj.Format(rangeGroup, Constants.BedTempFileName);
                    break;
                case AdditionalParameters.RangeGroupTextWriter:
                    using (var txtWriter =
                        File.Create(Constants.BedTempFileName))
                    {
                        formatterObj.Format(txtWriter, rangeGroup);
                    }
                    break;
                default:
                    break;
            }

            // Reparse to validate the results
            var parserObj = new BedParser();
            IList<ISequenceRange> newRangeList =
                parserObj.ParseRange(Constants.BedTempFileName);

            // Validation of all the properties.
            for (int i = 0; i < rangeList.Count; i++)
            {
                Assert.AreEqual(rangeList[i].ID, newRangeList[i].ID);
                Assert.AreEqual(rangeList[i].Start, newRangeList[i].Start);
                Assert.AreEqual(rangeList[i].End, newRangeList[i].End);

                // Validation of all metadata information.
                if (rangeList[i].Metadata.Count > 0)
                {
                    if (rangeList[i].Metadata.ContainsKey("Name"))
                    {
                        Assert.AreEqual(rangeList[i].Metadata["Name"],
                                        newRangeList[i].Metadata["Name"]);
                    }

                    if (rangeList[i].Metadata.ContainsKey("Score"))
                    {
                        Assert.AreEqual(rangeList[i].Metadata["Score"],
                                        newRangeList[i].Metadata["Score"]);
                    }

                    if (rangeList[i].Metadata.ContainsKey("Strand"))
                    {
                        Assert.AreEqual(rangeList[i].Metadata["Strand"],
                                        newRangeList[i].Metadata["Strand"]);
                    }

                    if (rangeList[i].Metadata.ContainsKey("ThickStart"))
                    {
                        Assert.AreEqual(rangeList[i].Metadata["ThickStart"],
                                        newRangeList[i].Metadata["ThickStart"]);
                    }

                    if (rangeList[i].Metadata.ContainsKey("ThickEnd"))
                    {
                        Assert.AreEqual(rangeList[i].Metadata["ThickEnd"],
                                        newRangeList[i].Metadata["ThickEnd"]);
                    }

                    if (rangeList[i].Metadata.ContainsKey("ItemRGB"))
                    {
                        Assert.AreEqual(rangeList[i].Metadata["ItemRGB"],
                                        newRangeList[i].Metadata["ItemRGB"]);
                    }

                    if (rangeList[i].Metadata.ContainsKey("BlockCount"))
                    {
                        Assert.AreEqual(rangeList[i].Metadata["BlockCount"],
                                        newRangeList[i].Metadata["BlockCount"]);
                    }

                    if (rangeList[i].Metadata.ContainsKey("BlockSizes"))
                    {
                        Assert.AreEqual(rangeList[i].Metadata["BlockSizes"],
                                        newRangeList[i].Metadata["BlockSizes"]);
                    }

                    if (rangeList[i].Metadata.ContainsKey("BlockStarts"))
                    {
                        Assert.AreEqual(rangeList[i].Metadata["BlockStarts"],
                                        newRangeList[i].Metadata["BlockStarts"]);
                    }

                    ApplicationLog.WriteLine(
                        "Bed Formatter BVT: Successfully validated all the metadata information");
                }
            }

            ApplicationLog.WriteLine(
                "Bed Formatter BVT: Successfully validated the ID, Start and End Ranges");

            // Cleanup the file.
            if (File.Exists(Constants.BedTempFileName))
                File.Delete(Constants.BedTempFileName);
        }

        #endregion Supported Methods
    }
}