using System;
using System.Collections.Generic;
using System.Linq;

using Bio.IO;
using Bio.IO.Bed;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests
{
    /// <summary>
    ///     Test Automation code for Bio SequenceRange BVT level validations.
    /// </summary>
    [TestFixture]
    public class SequenceRangeAndSequenceRangeGroupingBvtTestCases
    {
        private readonly Utility utilityObj = new Utility(@"TestUtils\BedTestsConfig.xml");

        #region SequenceRangeAndSequenceRangeGroupingBvtTestCases

        /// <summary>
        ///     Validate creation of SequenceRange.
        ///     Input Data : Valid Range ID,Start and End.
        ///     Output Data : Validation of created SequenceRange.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSequenceRange()
        {
            // Get values from xml.
            string expectedRangeId = this.utilityObj.xmlUtil.GetTextValue(Constants.SequenceRangeNode, Constants.IDNode);
            string expectedStartIndex = this.utilityObj.xmlUtil.GetTextValue(Constants.SequenceRangeNode, Constants.StartNode);
            string expectedEndIndex = this.utilityObj.xmlUtil.GetTextValue(Constants.SequenceRangeNode, Constants.EndNode);

            // Create a SequenceRange.
            var seqRange = new SequenceRange(expectedRangeId,
                                             long.Parse(expectedStartIndex, null), long.Parse(expectedEndIndex, null));

            // Validate created SequenceRange.
            Assert.AreEqual(expectedRangeId, seqRange.ID.ToString(null));
            Assert.AreEqual(expectedStartIndex, seqRange.Start.ToString((IFormatProvider) null));
            Assert.AreEqual(expectedEndIndex, seqRange.End.ToString((IFormatProvider) null));
            ApplicationLog.WriteLine("SequenceRange BVT : Successfully validated the SequenceStart,SequenceID and SequenceEnd.");
        }

        /// <summary>
        ///     Validate comparison of two SequenceRanges.
        ///     Input Data : Valid Range ID,Start and End.
        ///     Output Data : Validation of cmompareTo.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateCompareTwoSequenceRanges()
        {
            // Get values from xml.
            string expectedRangeId = this.utilityObj.xmlUtil.GetTextValue(Constants.SequenceRangeNode, Constants.IDNode);
            string expectedStartIndex = this.utilityObj.xmlUtil.GetTextValue(Constants.SequenceRangeNode, Constants.StartNode);
            string expectedEndIndex = this.utilityObj.xmlUtil.GetTextValue(Constants.SequenceRangeNode, Constants.EndNode);
            string expectedRangeId1 = this.utilityObj.xmlUtil.GetTextValue(Constants.SequenceRangeNode, Constants.IDNode1);
            string expectedStartIndex1 = this.utilityObj.xmlUtil.GetTextValue(Constants.SequenceRangeNode, Constants.StartNode1);
            string expectedEndIndex1 = this.utilityObj.xmlUtil.GetTextValue(Constants.SequenceRangeNode, Constants.EndNode1);

            // Create first SequenceRange.
            var seqRange = new SequenceRange(expectedRangeId, long.Parse(expectedStartIndex, null), long.Parse(expectedEndIndex, null));

            // Create second SequenceRange.
            var secondSeqRange = new SequenceRange(expectedRangeId1, long.Parse(expectedStartIndex1, null), long.Parse(expectedEndIndex1, null));

            // Compare two SequenceRanges which are identical.
            int result = seqRange.CompareTo(secondSeqRange);

            // Validate result of comparison.
            Assert.AreEqual(0, result);
            ApplicationLog.WriteLine("SequenceRange BVT : Successfully validated the SequenceRange comparison");
        }

        /// <summary>
        ///     Validate creation of SequenceRangeGrouping.
        ///     Input Data : Valid Range ID,Start and End.
        ///     Output Data : Validation of created SequenceRangeGrouping.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSequenceRangeGrouping()
        {
            this.CreateSequenceRangeGrouping(Constants.SmallSizeBedNodeName);
        }

        /// <summary>
        ///     Validate addition of SequenceRange to SequenceRangeGrouping.
        ///     Input Data : Valid Range ID,Start and End.
        ///     Output Data : Validation of adding SequenceRange to SequenceRangeGrouping.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateAdditionOfSequenceRange()
        {
            this.CreateSequenceRangeGrouping(Constants.LongStartEndBedNodeName);
        }

        /// <summary>
        ///     Validate getGroup() of SequenceRangeGrouping.
        ///     Input Data : Valid Range ID,Start and End.
        ///     Output Data : Validation of getGroup() method.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSequenceRangeGetGroup()
        {
            this.CreateSequenceRangeGrouping(Constants.SequenceRangeNode);
        }

        /// <summary>
        ///     Validate SequenceRange MergeOveralp.
        ///     Input Data : Valid small size BED file.
        ///     Output Data : Validation of SequenceRange MergeOveralp.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSequenceRangeMergeOverlaps()
        {
            this.MergeSequenceRange(Constants.MergeBedFileNode, false, false);
        }

        /// <summary>
        ///     Validate Merge two bed files.
        ///     Input Data : Valid small size BED file.
        ///     Output Data : Validation of Merge two bed files.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateMergeTwoBedFiles()
        {
            this.MergeSequenceRange(Constants.MergeTwoFiles, true, true);
        }

        /// <summary>
        ///     Validate Intersect sequence range grouping without pieces of intervals
        ///     Input Data : Two bed files..
        ///     Output Data : Validate Intersect sequence range grouping.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateIntersectSequenceRangeGroupingWithoutPiecesOfIntervals()
        {
            this.IntersectSequenceRange(Constants.IntersectResultsWithoutPiecesOfIntervals, false, true);
        }

        /// <summary>
        ///     Validate Intersect sequence range grouping with pieces of intervals
        ///     Input Data : Two bed files..
        ///     Output Data : Validate Intersect sequence range grouping.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateIntersectSequenceRangeGroupingWithPiecesOfIntervals()
        {
            this.IntersectSequenceRange(Constants.IntersectResultsWithPiecesOfIntervals, true, true);
        }

        /// <summary>
        ///     Validate Intersect sequence range grouping without pieces of intervals
        ///     for small size bed files.
        ///     Input Data : Two small size bed files..
        ///     Output Data : Validate Intersect sequence range grouping.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateIntersectSequenceRangeGroupingForSmallSizeBedFiles()
        {
            this.IntersectSequenceRange(Constants.IntersectWithoutPiecesOfIntervalsForSmallSizeFile,
                                   false, true);
        }

        /// <summary>
        ///     Validate Intersect sequence range grouping with pieces of intervals
        ///     for small size bed files.
        ///     Input Data : Two small size bed files..
        ///     Output Data : Validate Intersect sequence range grouping.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateIntersectSequenceRangeGroupingWithPiecesOfIntervalsForSmallSizeBedFiles()
        {
            this.IntersectSequenceRange(Constants.IntersectWithPiecesOfIntervalsForSmallSizeFile,
                                   true, false);
        }

        /// <summary>
        ///     Validate Flatten method
        ///     Input Data : SequenceRangeGroup
        ///     Output Data : SequenceRangeList.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateFlatten()
        {
            // Get values from xml.
            string expectedRangeIDs = this.utilityObj.xmlUtil.GetTextValue(
                Constants.SmallSizeBedNodeName, Constants.IDNode);
            string expectedStartIndex = this.utilityObj.xmlUtil.GetTextValue(
                Constants.SmallSizeBedNodeName, Constants.StartNode);
            string expectedEndIndex = this.utilityObj.xmlUtil.GetTextValue(
                Constants.SmallSizeBedNodeName, Constants.EndNode);
            string expectedSequenceRangeCount = this.utilityObj.xmlUtil.GetTextValue(
                Constants.SmallSizeBedNodeName, Constants.SequenceRangeCountNode);

            string[] rangeIDs = expectedRangeIDs.Split(',');
            string[] rangeStarts = expectedStartIndex.Split(',');
            string[] rangeEnds = expectedEndIndex.Split(',');
            var seqRangeGrouping = new SequenceRangeGrouping();
            List<ISequenceRange> rangeList = null;

            // Create a SequenceRange and add to SequenceRangeList.
            for (int i = 0; i < rangeIDs.Length; i++)
            {
                var seqRange = new SequenceRange(rangeIDs[i],
                                                 long.Parse(rangeStarts[i], null),
                                                 long.Parse(rangeEnds[i], null));

                seqRangeGrouping.Add(seqRange);
            }

            //Convert SequenceRangeGroup to SequenceRangeList.
            rangeList = seqRangeGrouping.Flatten();

            int j = 0;
            // Validate created SequenceRanges.
            foreach (ISequenceRange seqRange in rangeList)
            {
                Assert.AreEqual(rangeStarts[j], seqRange.Start.ToString((IFormatProvider) null));
                Assert.AreEqual(rangeEnds[j], seqRange.End.ToString((IFormatProvider) null));
                Assert.AreEqual(rangeIDs[j], seqRange.ID.ToString(null));
                j++;
            }

            Assert.AreEqual(expectedSequenceRangeCount, rangeList.Count.ToString((IFormatProvider) null));
            ApplicationLog.WriteLine("SequenceRange BVT : Successfully validated the SequenceStart,SequenceID and SequenceEnd.");
        }

        /// <summary>
        ///     Validate subtract two small size Bed files with minimal overlap and
        ///     with non overlapping pieces of intervals
        ///     Input Data : Valid BED file.
        ///     Output Data : Validation of subtract operation.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSubtractTwoBedFilesWithMinimalandNonOverlap()
        {
            this.SubtractSequenceRange(Constants.SubtractSmallBedFilesWithMinimalOverlapNodeName,
                                  false, true);
        }

        /// <summary>
        ///     Validate subtract two small size Bed files and
        ///     with non overlapping pieces of intervals
        ///     Input Data : Valid BED file.
        ///     Output Data : Validation of subtract operation.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSubtractTwoBedFilesWithNonOverlapIntervals()
        {
            this.SubtractSequenceRange(Constants.SubtractSmallBedFilesNodeName,
                                  false, false);
        }

        /// <summary>
        ///     Validate subtract two small size Bed files and
        ///     intervals with no overlap
        ///     Input Data : Valid BED file.
        ///     Output Data : Validation of subtract operation.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSubtractTwoBedFilesUsingIntervalsWithNoOverlap()
        {
            this.SubtractSequenceRange(Constants.SubtractSmallBedFilesWithIntervalsNodeName,
                                  true, true);
        }

        #endregion SequenceRangeAndSequenceRangeGroupingBvtTestCases

        # region Sequence Range Parser test cases.

        /// <summary>
        ///     Validates public properties of SequenceRangeParsers class.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSequenceRangeParsers()
        {
            IReadOnlyList<ISequenceRangeParser> parserRange = null;

            //Validate All property
            parserRange = SequenceRangeParsers.All;
            Assert.AreEqual(Constants.BedParser, parserRange[0].ToString());

            //Validate Bed property
            Type parserType = SequenceRangeParsers.Bed.GetType();
            Assert.AreEqual(parserType.ToString(), Constants.BedParser);
            ApplicationLog.WriteLine("Sequence Range Parser BVT: Validation of Sequence Range Parser Public properties completed successfully.");
        }

        # endregion Sequence Range Parser test cases.

        # region Sequence Range Formatter test cases.

        /// <summary>
        ///     Validates public properties of SequenceRangeFormatters class.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSequenceRangeFormatters()
        {
            IReadOnlyList<ISequenceRangeFormatter> formatterRange = null;

            //Validate All property
            formatterRange = SequenceRangeFormatters.All;
            Assert.AreEqual(Constants.BedFormatter, formatterRange[0].ToString());

            //Validate Bed property
            Type formatterType = SequenceRangeFormatters.Bed.GetType();
            Assert.AreEqual(formatterType.ToString(), Constants.BedFormatter);
            ApplicationLog.WriteLine(
                "Sequence Range Formatter BVT: Validation of Sequence Range Formatter Public properties completed successfully.");
        }

        # endregion Sequence Range Formatter test cases.

        #region Helper Methods

        /// <summary>
        ///     Create a SequenceRangeGrouping by passing RangeID,Start and End Index.
        ///     and validate created SequenceRange.
        /// </summary>
        /// <param name="nodeName">Xml Node name for different inputs.</param>
        private void CreateSequenceRangeGrouping(string nodeName)
        {
            // Get values from xml.
            string expectedRangeIDs = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.IDNode);
            string expectedStartIndex = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.StartNode);
            string expectedEndIndex = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.EndNode);
            string expectedSequenceRangeCount = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.SequenceRangeCountNode);

            string[] rangeIDs = expectedRangeIDs.Split(',');
            string[] rangeStarts = expectedStartIndex.Split(',');
            string[] rangeEnds = expectedEndIndex.Split(',');
            var seqRangeGrouping = new SequenceRangeGrouping();
            List<ISequenceRange> rangeList = null;

            // Create a SequenceRange and add to SequenceRangeList.
            for (int i = 0; i < rangeIDs.Length; i++)
            {
                var seqRange = new SequenceRange(rangeIDs[i],
                                                 long.Parse(rangeStarts[i], null), long.Parse(rangeEnds[i], null));

                seqRangeGrouping.Add(seqRange);
            }

            IEnumerable<string> rangeGroupIds = seqRangeGrouping.GroupIDs;
            string rangeID = string.Empty;
            int j = 0;

            foreach (string groupID in rangeGroupIds)
            {
                rangeID = groupID;

                // Get SequenceRangeIds.
                rangeList = seqRangeGrouping.GetGroup(rangeID);

                // Validate created SequenceRanges.
                foreach (ISequenceRange seqRange in rangeList)
                {
                    Assert.AreEqual(rangeStarts[j], seqRange.Start.ToString((IFormatProvider) null));
                    Assert.AreEqual(rangeEnds[j], seqRange.End.ToString((IFormatProvider) null));
                    Assert.AreEqual(rangeIDs[j], seqRange.ID.ToString(null));
                    j++;
                }
            }
            Assert.AreEqual(expectedSequenceRangeCount,
                            rangeList.Count.ToString((IFormatProvider) null));
            ApplicationLog.WriteLine(
                "SequenceRange BVT : Successfully validated the SequenceStart, SequenceID and SequenceEnd.");
        }

        /// <summary>
        ///     Validate Intersect SequenceRangeGrouping.
        /// </summary>
        /// <param name="nodeName">Xml Node name for different inputs.</param>
        /// <param name="overlappingBasePair">Value of overlappingBasePair</param>
        /// <param name="isParentSeqRangeRequired"></param>
        private void IntersectSequenceRange(string nodeName,
                                            bool overlappingBasePair, bool isParentSeqRangeRequired)
        {
            // Get values from xml.
            string[] expectedRangeIDs = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.IDNode).Split(',');
            string[] expectedStartIndex = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.StartNode).Split(',');
            string[] expectedEndIndex = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.EndNode).Split(',');
            string referenceFilePath = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string queryFilePath = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.QueryFilePath);
            string minimalOverlap = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.OverlapValue);

            // Parse a BED file.
            var parserObj = new BedParser();
            SequenceRangeGrouping referenceGroup = parserObj.ParseRangeGrouping(referenceFilePath);
            SequenceRangeGrouping queryGroup = parserObj.ParseRangeGrouping(queryFilePath);

            var outputType = IntersectOutputType.OverlappingIntervals;
            if (overlappingBasePair)
            {
                outputType = IntersectOutputType.OverlappingPiecesOfIntervals;
            }

            // Intersect a SequenceRangeGroup.
            SequenceRangeGrouping intersectGroup = referenceGroup.Intersect(queryGroup, long.Parse(minimalOverlap, null), outputType);

            // Get a intersect SequenceGroup Id.
            IEnumerable<string> groupIds = intersectGroup.GroupIDs;
            int j = 0;
            foreach (string grpId in groupIds)
            {
                string rangeId = grpId;

                List<ISequenceRange> rangeList = intersectGroup.GetGroup(rangeId);

                // Validate intersect sequence range.
                foreach (ISequenceRange range in rangeList)
                {
                    Assert.AreEqual(expectedStartIndex[j], range.Start.ToString((IFormatProvider) null));
                    Assert.AreEqual(expectedEndIndex[j], range.End.ToString((IFormatProvider) null));
                    Assert.AreEqual(expectedRangeIDs[j], range.ID.ToString(null));
                    j++;
                }
            }

            // Validate ParentSeqRanges.
            bool result = ValidateParentSeqRange(intersectGroup, referenceGroup, queryGroup, isParentSeqRangeRequired);
            Assert.IsTrue(result);

            ApplicationLog.WriteLine(
                "Intersect SequenceRangeGrouping BVT: Successfully validated the intersect SequeID, Start and End Ranges");
        }

        /// <summary>
        ///     Validate Merge SequenceRangeGrouping.
        /// </summary>
        /// <param name="nodeName">Xml Node name for different inputs.</param>
        /// <param name="isMergePam">Merge parameter</param>
        /// <param name="isParentSeqRangesRequired">Is Parent Sequence Range required?</param>
        private void MergeSequenceRange(string nodeName, bool isMergePam, bool isParentSeqRangesRequired)
        {
            // Get values from xml.
            string[] expectedRangeIDs = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.IDNode).Split(',');
            string[] expectedStartIndex = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.StartNode).Split(',');
            string[] expectedEndIndex = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.EndNode).Split(',');
            string filePath = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string queryFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.QueryFilePath);

            // Parse a BED file.
            var parserObj = new BedParser();
            SequenceRangeGrouping referenceGroup = parserObj.ParseRangeGrouping(filePath);
            SequenceRangeGrouping queryGroup = parserObj.ParseRangeGrouping(queryFilePath);

            // Merge a SequenceRangeGroup.
            SequenceRangeGrouping mergedGroup = isMergePam ? referenceGroup.MergeOverlaps(queryGroup, 0, isParentSeqRangesRequired) : referenceGroup.MergeOverlaps();

            // Get a merged SequenceGroup Id.
            IEnumerable<string> groupIds = mergedGroup.GroupIDs;

            int j = 0;
            foreach (string grpId in groupIds)
            {
                string rangeId = grpId;

                List<ISequenceRange> rangeList = mergedGroup.GetGroup(rangeId);

                // Validate merged sequence range.
                foreach (ISequenceRange range in rangeList)
                {
                    Assert.AreEqual(expectedStartIndex[j], range.Start.ToString((IFormatProvider) null));
                    Assert.AreEqual(expectedEndIndex[j], range.End.ToString((IFormatProvider) null));
                    Assert.AreEqual(expectedRangeIDs[j], range.ID.ToString(null));
                    j++;
                }
            }

            // Validate Parent SequenceRanges.
            bool result = ValidateParentSeqRange(mergedGroup,referenceGroup, queryGroup, isParentSeqRangesRequired);
            Assert.IsTrue(result);

            ApplicationLog.WriteLine("Merge SequenceRangeGrouping BVT: Successfully validated the merged SequeID, Start and End Ranges");
        }

        /// <summary>
        ///     Validate Subtract SequenceRangeGrouping.
        /// </summary>
        /// <param name="nodeName">Xml Node name for different inputs.</param>
        /// <param name="overlappingBasePair">Value of overlappingBasePair</param>
        /// <param name="isParentSeqRangeRequired"></param>
        private void SubtractSequenceRange(string nodeName, bool overlappingBasePair, bool isParentSeqRangeRequired)
        {
            // Get values from xml.
            string[] expectedRangeIDs = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.IDNode).Split(',');
            string[] expectedStartIndex = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.StartNode).Split(',');
            string[] expectedEndIndex = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.EndNode).Split(',');
            string referenceFilePath = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string queryFilePath = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.QueryFilePath);
            string minimalOverlap = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.OverlapValue);
            string rangeID = string.Empty;
            bool result = false;

            // Parse a BED file.
            var parserObj = new BedParser();
            SequenceRangeGrouping referenceGroup = parserObj.ParseRangeGrouping(referenceFilePath);
            SequenceRangeGrouping queryGroup = parserObj.ParseRangeGrouping(queryFilePath);

            var subtractOutputType = SubtractOutputType.NonOverlappingPiecesOfIntervals;
            if (overlappingBasePair)
            {
                subtractOutputType = SubtractOutputType.IntervalsWithNoOverlap;
            }

            // Subtract a SequenceRangeGroup.
            SequenceRangeGrouping subtractGroup = referenceGroup.Subtract(queryGroup,
                                                                          long.Parse(minimalOverlap, null),
                                                                          subtractOutputType, isParentSeqRangeRequired);

            // Get a intersect SequenceGroup Id.
            IEnumerable<string> groupIds = subtractGroup.GroupIDs;

            int j = 0;
            foreach (string grpID in groupIds)
            {
                rangeID = grpID;

                List<ISequenceRange> rangeList = subtractGroup.GetGroup(rangeID);

                // Validate intersect sequence range.
                foreach (ISequenceRange range in rangeList)
                {
                    Assert.AreEqual(expectedStartIndex[j], range.Start.ToString((IFormatProvider) null));
                    Assert.AreEqual(expectedEndIndex[j], range.End.ToString((IFormatProvider) null));
                    Assert.AreEqual(expectedRangeIDs[j], range.ID.ToString(null));
                    j++;
                }
            }

            // Validate ParentSeqRanges.
            result = ValidateParentSeqRange(subtractGroup, referenceGroup, queryGroup, isParentSeqRangeRequired);
            Assert.IsTrue(result);

            ApplicationLog.WriteLine("Subtract SequenceRangeGrouping BVT: Successfully validated the subtract SequeID, Start and End Ranges");
        }

        /// <summary>
        /// Validate Parent Sequence ranges in result sequence range.
        /// </summary>
        /// <param name="resultSeq">Result seq range group.</param>
        /// <param name="refSeq">Reference seq range group.</param>
        /// <param name="querySeq">Query seq range group.</param>
        /// <param name="isParentSeqRangeRequired">
        /// Flag to indicate whether result should contain parent seq ranges or not.
        /// </param>
        /// <returns>Returns true if the parent seq ranges are valid; otherwise returns false.</returns>
        private static bool ValidateParentSeqRange(SequenceRangeGrouping resultSeq, SequenceRangeGrouping refSeq,
                                                   SequenceRangeGrouping querySeq, bool isParentSeqRangeRequired)
        {
            IList<ISequenceRange> refSeqRangeList = new List<ISequenceRange>();
            IList<ISequenceRange> querySeqRangeList = new List<ISequenceRange>();

            IEnumerable<string> groupIds = resultSeq.GroupIDs;

            foreach (string groupId in groupIds)
            {
                if (null != refSeq)
                {
                    refSeqRangeList = refSeq.GetGroup(groupId);
                }

                if (null != querySeq)
                {
                    querySeqRangeList = querySeq.GetGroup(groupId);
                }

                foreach (ISequenceRange resultRange in resultSeq.GetGroup(groupId))
                {
                    if (!isParentSeqRangeRequired)
                    {
                        if (resultRange.ParentSeqRanges.Count != 0)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        int refSeqRangeCount = refSeqRangeList.Count(s => resultRange.ParentSeqRanges.Contains(s));
                        int querySeqRangeCount = querySeqRangeList.Count(s => resultRange.ParentSeqRanges.Contains(s));
                        if (resultRange.ParentSeqRanges.Count != refSeqRangeCount + querySeqRangeCount)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        #endregion Helper Methods
    }
}