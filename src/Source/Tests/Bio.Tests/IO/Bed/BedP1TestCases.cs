/****************************************************************************
 * BedP1TestCases.cs
 * 
 *  This file contains the Bed - Parsers and Formatters P1 test cases.
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
    ///     Bed P1 parser and formatter Test case implementation.
    /// </summary>
    [TestFixture]
    public class BedP1TestCases
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
            ParseRangeGroup,
            ParseRangeTextWriter,
            ParseRangeGroupTextWriter
        };

        #endregion Enums

        #region Global Variables

        private readonly Utility utilityObj = new Utility(@"TestUtils\BedTestsConfig.xml");

        #endregion Global Variables

        #region Bed Parser P1 Test cases

        /// <summary>
        ///     Parse a valid Bed file (one line) and
        ///     convert the same Range using ParseRange(text-reader) method and
        ///     validate the same
        ///     Input : Bed File
        ///     Validation: Range properties like ID, Start and End.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void BedParserValidateOneLineParseRangeTextReader()
        {
            ParserGeneralTestCases(Constants.OneLineBedNodeName,
                                   AdditionalParameters.RangeTextReader);
        }

        /// <summary>
        ///     Parse a valid Bed file (one line) and
        ///     convert the same Range using ParseRangeGrouping(file-name) method and
        ///     validate the same
        ///     Input : Bed File
        ///     Validation: Range properties like ID, Start and End.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void BedParserValidateOneLineParseRangeGroupFileName()
        {
            ParserGeneralTestCases(Constants.OneLineBedNodeName,
                                   AdditionalParameters.RangeGroupFileName);
        }

        /// <summary>
        ///     Parse a valid Bed file (three chromosomes) and
        ///     convert the same Range using ParseRange(file-name) method and
        ///     validate the same
        ///     Input : Bed File
        ///     Validation: Range properties like ID, Start and End.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void BedParserValidateThreeChromosomeParseRangeFileName()
        {
            ParserGeneralTestCases(Constants.ThreeChromoBedNodeName,
                                   AdditionalParameters.RangeFileName);
        }

        #endregion Bed Parser P1 Test cases

        #region Bed Formatter P1 Test cases

        /// <summary>
        ///     Format a valid Range (small size file) to a
        ///     Bed file using Format(Range, file-path) method and
        ///     validate the same.
        ///     Input : Bed Range
        ///     Validation : Read the Bed file to which the range was formatted
        ///     using File-Info and Validate Properties like ID, Start and End
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void BedFormatterValidateParseSmallSizeFormatRangeFileName()
        {
            FormatterGeneralTestCases(Constants.SmallSizeBedNodeName,
                                      AdditionalParameters.ParseRange);
        }

        /// <summary>
        ///     Format a valid Range (small size file) to a
        ///     Bed file using Format(Range, text-writer) method and
        ///     validate the same.
        ///     Input : Bed Range
        ///     Validation : Read the Bed file to which the range was formatted
        ///     using File-Info and Validate Properties like ID, Start and End
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void BedFormatterValidateParseSmallSizeFormatRangeTextWriter()
        {
            FormatterGeneralTestCases(Constants.SmallSizeBedNodeName,
                                      AdditionalParameters.ParseRangeTextWriter);
        }

        /// <summary>
        ///     Format a valid Range (one line file) to a
        ///     Bed file using Format(Range, file-name) method and
        ///     validate the same.
        ///     Input : Bed Range
        ///     Validation : Read the Bed file to which the range was formatted
        ///     using File-Info and Validate Properties like ID, Start and End
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void BedFormatterValidateParseOneLineFormatRangeFileName()
        {
            FormatterGeneralTestCases(Constants.OneLineBedNodeName,
                                      AdditionalParameters.ParseRange);
        }

        /// <summary>
        ///     Format a valid RangeGroup (small size file) to a
        ///     Bed file using Format(RangeGroup, file-path) method and
        ///     validate the same.
        ///     Input : Bed Range
        ///     Validation : Read the Bed file to which the range was formatted
        ///     using File-Info and Validate Properties like ID, Start and End
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void BedFormatterValidateParseSmallSizeFormatRangeGroupFileName()
        {
            FormatterGeneralTestCases(Constants.SmallSizeBedNodeName,
                                      AdditionalParameters.ParseRangeGroup);
        }

        /// <summary>
        ///     Format a valid RangeGroup (small size file) to a
        ///     Bed file using Format(RangeGroup, text-writer) method and
        ///     validate the same.
        ///     Input : Bed Range
        ///     Validation : Read the Bed file to which the range was formatted
        ///     using File-Info and Validate Properties like ID, Start and End
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void BedFormatterValidateParseSmallSizeFormatRangeGroupTextWriter()
        {
            FormatterGeneralTestCases(Constants.SmallSizeBedNodeName,
                                      AdditionalParameters.ParseRangeGroupTextWriter);
        }

        /// <summary>
        ///     Format a valid RangeGroup (one line file) to a
        ///     Bed file using Format(RangeGroup, file-path) method and
        ///     validate the same.
        ///     Input : Bed Range
        ///     Validation : Read the Bed file to which the range was formatted
        ///     using File-Info and Validate Properties like ID, Start and End
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void BedFormatterValidateParseOneLineFormatRangeGroupFileName()
        {
            FormatterGeneralTestCases(Constants.OneLineBedNodeName,
                                      AdditionalParameters.ParseRangeGroup);
        }

        /// <summary>
        ///     Format a valid Range (one line file) to a
        ///     Bed file using Format(Range, Text-Writer) method and
        ///     validate the same.
        ///     Input : Bed Range
        ///     Validation : Read the Bed file to which the range was formatted
        ///     using File-Info and Validate Properties like ID, Start and End
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void BedFormatterValidateParseOneLineFormatRangeTextWriter()
        {
            FormatterGeneralTestCases(Constants.OneLineBedNodeName,
                                      AdditionalParameters.RangeTextWriter);
        }

        /// <summary>
        ///     Format a valid RangeGroup (one line file) to a
        ///     Bed file using Format(RangeGroup, Text-Writer) method and
        ///     validate the same.
        ///     Input : Bed Range
        ///     Validation : Read the Bed file to which the range was formatted
        ///     using File-Info and Validate Properties like ID, Start and End
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void BedFormatterValidateParseOneLineFormatRangeGroupTextWriter()
        {
            FormatterGeneralTestCases(Constants.OneLineBedNodeName,
                                      AdditionalParameters.RangeGroupTextWriter);
        }

        /// <summary>
        ///     Format a valid Range (three chromosome file) to a
        ///     Bed file using Format(Range, file-path) method and
        ///     validate the same.
        ///     Input : Bed Range
        ///     Validation : Read the Bed file to which the range was formatted
        ///     using File-Info and Validate Properties like ID, Start and End
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void BedFormatterValidateParseThreeChromosomeFormatRangeFileName()
        {
            FormatterGeneralTestCases(Constants.ThreeChromoBedNodeName,
                                      AdditionalParameters.ParseRange);
        }

        /// <summary>
        ///     Format a valid RangeGroup (three chromosome file) to a
        ///     Bed file using Format(RangeGroup, file-path) method and
        ///     validate the same.
        ///     Input : Bed Range
        ///     Validation : Read the Bed file to which the range was formatted
        ///     using File-Info and Validate Properties like ID, Start and End
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void BedFormatterValidateParseThreeChromosomeFormatRangeGroupFileName()
        {
            FormatterGeneralTestCases(Constants.ThreeChromoBedNodeName,
                                      AdditionalParameters.ParseRangeGroup);
        }

        /// <summary>
        ///     Format a valid Range (Long Start End file) to a
        ///     Bed file using Format(Range, file-path) method and
        ///     validate the same.
        ///     Input : Bed Range
        ///     Validation : Read the Bed file to which the range was formatted
        ///     using File-Info and Validate Properties like ID, Start and End
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void BedFormatterValidateParseLongStartEndFormatRangeFileName()
        {
            FormatterGeneralTestCases(Constants.LongStartEndBedNodeName,
                                      AdditionalParameters.ParseRange);
        }

        /// <summary>
        ///     Format a valid RangeGroup (Long Start End file) to a
        ///     Bed file using Format(RangeGroup, file-path) method and
        ///     validate the same.
        ///     Input : Bed Range
        ///     Validation : Read the Bed file to which the range was formatted
        ///     using File-Info and Validate Properties like ID, Start and End
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void BedFormatterValidateParseLongStartEndFormatRangeGroupFileName()
        {
            FormatterGeneralTestCases(Constants.LongStartEndBedNodeName,
                                      AdditionalParameters.ParseRangeGroup);
        }

        #endregion Bed Formatter P1 Test cases

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
                                                   "Bed Parser P1: Reading the File from location '{0}'", filePath));

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
                "Bed Parser P1: Successfully validated the ID, Start and End Ranges");
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
                case AdditionalParameters.ParseRangeGroupTextWriter:
                    var initialParserGroupObj = new BedParser();
                    rangeGroup =
                        initialParserGroupObj.ParseRangeGrouping(filePath);
                    break;
                case AdditionalParameters.ParseRange:
                case AdditionalParameters.ParseRangeTextWriter:
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
                                var rangeObj1 =
                                    new SequenceRange(expectedIDs[i],
                                                      long.Parse(expectedStarts[i], null),
                                                      long.Parse(expectedEnds[i], null));
                                rangeGroup.Add(rangeObj1);
                            }
                            break;
                        default:
                            for (int i = 0; i < expectedIDs.Length; i++)
                            {
                                var rangeObj2 =
                                    new SequenceRange(expectedIDs[i],
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
                case AdditionalParameters.ParseRangeTextWriter:
                    using (var txtWriter = File.Create(Constants.BedTempFileName))
                    {
                        formatterObj.Format(txtWriter, rangeList);
                    }
                    break;
                case AdditionalParameters.RangeGroupFileName:
                case AdditionalParameters.ParseRangeGroup:
                    formatterObj.Format(rangeGroup, Constants.BedTempFileName);
                    break;
                case AdditionalParameters.RangeGroupTextWriter:
                case AdditionalParameters.ParseRangeGroupTextWriter:
                    using (var txtWriter = File.Create(Constants.BedTempFileName))
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
                Assert.AreEqual(rangeList[0].ID, newRangeList[0].ID);
                Assert.AreEqual(rangeList[0].Start, newRangeList[0].Start);
                Assert.AreEqual(rangeList[0].End, newRangeList[0].End);
            }

            ApplicationLog.WriteLine(
                "Bed Formatter P1: Successfully validated the ID, Start and End Ranges");

            // Cleanup the file.
            if (File.Exists(Constants.BedTempFileName))
                File.Delete(Constants.BedTempFileName);
        }

        #endregion Supported Methods
    }
}