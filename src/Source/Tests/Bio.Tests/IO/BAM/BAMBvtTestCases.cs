using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Bio.IO.BAM;
using Bio.IO.FastA;
using Bio.IO.SAM;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests.Framework.IO.BAM
{
    /// <summary>
    ///     BAM Bvt parser and formatter Test case implementation.
    /// </summary>
    [TestFixture]
    public class BAMBvtTestCases
    {
        #region Enums

        /// <summary>
        ///     BAM Parser ctor parameters used for different test cases.
        /// </summary>
        private enum BAMParserParameters
        {
            StreamReader,
            FileName,
            ParseRangeFileName,
            ParseRangeWithIndex,
            ParserWithEncoding,
            IndexFile,
            IndexStreamWriter,
            StreamWriter,
            ParseRangeUsingRefSeq,
            ParseRangeUsingRefSeqAndFlag,
            ParseRangeUsingRefSeqUsingIndex,
            ParseRangeUsingIndexesAndFlag,
            Default
        }

        /// <summary>
        ///     Get Paired parameters
        /// </summary>
        private enum GetPairedReadParameters
        {
            GetPairedReadWithParameters,
            GetPairedReadWithLibraryName,
            GetPairedReadWithCloneLibraryInfo,
            Default
        }

        /// <summary>
        ///     PairedReadType method parameters
        /// </summary>
        private enum GetPairedReadTypeParameters
        {
            PaireReadTypeUsingLibraryName,
            PaireReadTypeUsingCloneLibraryInfo,
            PaireReadTypeUsingMeanAndDeviation,
            PaireReadTypeUsingReadsAndLibraryInfo,
            PaireReadTypeUsingReadsAndLibrary,
            GetInsertLength,
            Default
        }

        #endregion Enums

        #region Global Variables

        private readonly Utility utilityObj = new Utility(@"TestUtils\SAMBAMTestData\SAMBAMTestsConfig.xml");

        #endregion Global Variables

        #region BAM Parser Test Cases

        /// <summary>
        ///     Validate BAM Parse(Stream) by passing valid BAM file
        ///     Input : Small size BAM file.
        ///     Output : Validation of aligned sequence.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidateBAMParserWithStreamReader()
        {
            this.ValidateBAMParser(Constants.SmallSizeBAMFileNode,
                              BAMParserParameters.StreamReader, false);
        }

        /// <summary>
        ///     Validate BAM Parse(filename) by passing valid BAM file
        ///     Input : Small size BAM file.
        ///     Output : Validation of aligned sequence.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidateBAMParserWithFilePath()
        {
            this.ValidateBAMParser(Constants.SmallSizeBAMFileNode,
                              BAMParserParameters.FileName, false);
        }


        /// <summary>
        ///     Validate BAM Parse(filename) by passing valid BAM file
        ///     Input : Small size BAM file.
        ///     Output : Validation of aligned sequence.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidateBAMParserWithDummyReads()
        {
            
            string bamFilePath = utilityObj.xmlUtil.GetTextValue(Constants.BAMFileWithDummyReads,
                                                                Constants.FilePathNode);
            SequenceAlignmentMap seqAlignment = null;
            BAMParser bamParser = null;
            bamParser = new BAMParser();
            seqAlignment = bamParser.ParseOne<SequenceAlignmentMap>(bamFilePath);
            var seq = seqAlignment.QuerySequences.First();
            Assert.AreEqual("fakeref", seq.RName);
            Assert.AreEqual("1M", seq.CIGAR);
            Assert.AreEqual(10, seq.Pos);
            Assert.IsNull(seq.QuerySequence);
            var optField = seq.OptionalFields.First();
            Assert.AreEqual("CT", optField.Tag);
            Assert.AreEqual("Z", optField.VType);
            Assert.AreEqual(".;ESDN;", optField.Value);
            Assert.IsTrue(seq.IsDummyRead);
        }



        /// <summary>
        ///     Validate BAM ParseRange(filename,RefIndex) by passing valid BAM file
        ///     Input : Small size BAM file.
        ///     Output : Validation of aligned sequence.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidateBAMParserRangeWithFilePath()
        {
            this.ValidateBAMParser(Constants.SmallSizeBAMFileNode,
                              BAMParserParameters.ParseRangeFileName, false);
        }

        /// <summary>
        ///     Validate BAM ParseRange(filename,refSeqName)
        ///     by passing valid BAM file
        ///     Input : Small size BAM file.
        ///     Output : Validation of aligned sequence.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidateBAMParseRangeUsingRefSeqName()
        {
            this.ValidateBAMParser(Constants.BAMFileWithSequenceRangeRefSeqsNode,
                              BAMParserParameters.ParseRangeUsingRefSeq, false);
        }

        /// <summary>
        ///     Validate BAM ParseRange(filename,refSeqName,flag)
        ///     by passing valid BAM file
        ///     Input : Small size BAM file.
        ///     Output : Validation of aligned sequence.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidateBAMParseRangeUsingRefSeqNameAndFlag()
        {
            this.ValidateBAMParser(Constants.BAMFileWithSequenceRangeRefSeqsNode,
                              BAMParserParameters.ParseRangeUsingRefSeqAndFlag, false);
        }

        /// <summary>
        ///     Validate BAM ParseRange(filename,refSeqName,Start,end)
        ///     by passing valid BAM file
        ///     Input : Small size BAM file.
        ///     Output : Validation of aligned sequence.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidateBAMParseRangeUsingStartAndEndIndex()
        {
            this.ValidateBAMParser(Constants.SeqRangeBAMFileNode,
                              BAMParserParameters.ParseRangeUsingRefSeqUsingIndex, false);
        }

        /// <summary>
        ///     Validate BAM ParseRange(filename,refSeqName,Start,end,bool)
        ///     by passing valid BAM file
        ///     Input : Small size BAM file.
        ///     Output : Validation of aligned sequence.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidateBAMParseRangeUsingStartAndEndIndexWithFlag()
        {
            this.ValidateBAMParser(Constants.SeqRangeBAMFileNode,
                              BAMParserParameters.ParseRangeUsingIndexesAndFlag, false);
        }

        /// <summary>
        ///     Validate BAM GetPairedRead(Mean,Deviation)
        ///     by passing valid BAM file
        ///     Input : Small size BAM file.
        ///     Output : Validation of aligned sequence.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidatePairedReadUsingMeanAndDeviation()
        {
            this.ValidatePairedReads(Constants.PairedReadForSmallFileNodeName,
                                GetPairedReadParameters.GetPairedReadWithParameters);
        }

        /// <summary>
        ///     Validate BAM GetPairedRead()
        ///     by passing valid BAM file
        ///     Input : Small size BAM file.
        ///     Output : Validation of aligned sequence.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidatePairedReads()
        {
            this.ValidatePairedReads(Constants.PairedReadForSmallFileNodeName,
                                GetPairedReadParameters.Default);
        }

        /// <summary>
        ///     Validate BAM GetPairedRead(libraryName)
        ///     by passing valid BAM file
        ///     Input : Small size BAM file.
        ///     Output : Validation of aligned sequence.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidatePairedReadUsingLibraryName()
        {
            this.ValidatePairedReads(Constants.PairedReadForSmallFileNodeName,
                                GetPairedReadParameters.GetPairedReadWithLibraryName);
        }

        /// <summary>
        ///     Validate BAM GetPairedRead(CloneLibraryInfo)
        ///     by passing valid BAM file
        ///     Input : Small size BAM file.
        ///     Output : Validation of aligned sequence.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidatePairedReadUsingCloneLibraryInfo()
        {
            this.ValidatePairedReads(Constants.PairedReadForSmallFileNodeName,
                                GetPairedReadParameters.GetPairedReadWithCloneLibraryInfo);
        }

        /// <summary>
        ///     Validate BAM GetPairedReadType(PairedRead,CloneLibraryInfo)
        ///     by passing valid BAM file
        ///     Input : Small size BAM file.
        ///     Output : Validation of aligned sequence.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidatePairedReadTypeUsingCloneLibraryInfo()
        {
            this.ValidatePairedReadTypes(Constants.PairedReadTypesNode,
                                    GetPairedReadTypeParameters.PaireReadTypeUsingCloneLibraryInfo);
        }


        /// <summary>
        ///     Validate BAM GetPairedReadType(PairedRead,Library)
        ///     by passing valid BAM file
        ///     Input : Small size BAM file.
        ///     Output : Validation of aligned sequence.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidatePairedReadTypeUsingLibrary()
        {
            this.ValidatePairedReadTypes(Constants.PairedReadTypesNode,
                                    GetPairedReadTypeParameters.PaireReadTypeUsingLibraryName);
        }

        /// <summary>
        ///     Validate BAM GetPairedReadType(PairedRead,Mean,Devn)
        ///     by passing valid BAM file
        ///     Input : Small size BAM file.
        ///     Output : Validation of aligned sequence.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidatePairedReadTypeUsingMeanAndDeviation()
        {
            this.ValidatePairedReadTypes(Constants.PairedReadTypesForMeanAndDeviationNode,
                                    GetPairedReadTypeParameters.PaireReadTypeUsingMeanAndDeviation);
        }

        /// <summary>
        ///     Validate BAM GetPairedReadType(Read1,Read2,Library)
        ///     by passing valid BAM file
        ///     Input : Small size BAM file.
        ///     Output : Validation of aligned sequence.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidatePairedReadTypeUsingReadsAndLibrary()
        {
            this.ValidatePairedReadTypes(Constants.PairedReadTypesForLibraryInfoNode,
                                    GetPairedReadTypeParameters.PaireReadTypeUsingReadsAndLibrary);
        }

        /// <summary>
        ///     Validate BAM GetPairedReadType(Read1,Read2,CloneLibraryInfo)
        ///     by passing valid BAM file
        ///     Input : Small size BAM file.
        ///     Output : Validation of aligned sequence.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidatePairedReadTypeUsingReadsAndCloneLibraryInfo()
        {
            this.ValidatePairedReadTypes(Constants.PairedReadTypesForLibraryInfoNode,
                                    GetPairedReadTypeParameters.PaireReadTypeUsingReadsAndLibraryInfo);
        }

        /// <summary>
        ///     Validate BAM GetInsertLength(Read1,Read2)
        ///     by passing valid BAM file
        ///     Input : Small size BAM file.
        ///     Output : Validation of aligned sequence.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidateInsertLengthForPairedReads()
        {
            this.ValidatePairedReadTypes(Constants.PairedReadTypesForLibraryInfoNode,
                                    GetPairedReadTypeParameters.GetInsertLength);
        }

        /// <summary>
        ///     Validates the GetInsertLength method with a bool value passed for validate
        ///     Input : Small size BAM file.
        ///     Output : Validation of aligned sequence.
        /// </summary>
        /// </summary>
        [Category("Priority0"), Category("BAM")]
        public void ValidateGetInsertLengthWithValidate()
        {
            // Get input and output values from xml node.
            string bamFilePath = this.utilityObj.xmlUtil.GetTextValue(Constants.PairedReadTypesForLibraryInfoNode,
                                                                 Constants.FilePathNode);
            string mean = this.utilityObj.xmlUtil.GetTextValue(
                Constants.PairedReadTypesForLibraryInfoNode, Constants.MeanNode);
            string deviation = this.utilityObj.xmlUtil.GetTextValue(
                Constants.PairedReadTypesForLibraryInfoNode, Constants.DeviationValueNode);
            string[] insertLength = this.utilityObj.xmlUtil.GetTextValue(
                Constants.PairedReadTypesForLibraryInfoNode, Constants.InsertLengthNode).Split(',');

            using (var bamParser = new BAMParser())
            {
                SequenceAlignmentMap seqAlignmentMapObj = bamParser.ParseOne<SequenceAlignmentMap>(bamFilePath);
                int i = 0;
                try
                {
                    IList<PairedRead> pairedReads = seqAlignmentMapObj.GetPairedReads(float.Parse(mean, null),
                        float.Parse(deviation, null));
                    foreach (PairedRead read in pairedReads)
                    {
                        //pass true for validate parameter
                        int length = PairedRead.GetInsertLength(read.Read1, read.Read2, true);
                        Assert.AreEqual(length.ToString((IFormatProvider) null), insertLength[i]);
                        i++;
                    }
                    ApplicationLog.WriteLine(string.Format(null,
                                                           "BAM Parser BVT : Validated GetInsertLength Successfully"));
                }
                finally
                {
                    bamParser.Dispose();
                }
            }
        }

        #endregion BAM Parser Test Cases

        #region BAM Formatter Test Cases

        /// <summary>
        ///     Validate format(seqAlignment,bamFile,indexFile) by
        ///     parsing formatting valid BAM file.
        ///     Input : Small size BAM file and index file.
        ///     Output : Validation of aligned sequence.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidateBAMFormatterWithIndexFile()
        {
            this.ValidateBAMFormatter(Constants.SmallSizeBAMFileNode,
                                 BAMParserParameters.IndexFile);
        }

        /// <summary>
        ///     Validate format(seqAlignment, Stream) by
        ///     parsing formatting valid BAM file.
        ///     Input : Small size BAM file.
        ///     Output : Validation of aligned sequence.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidateBAMFormatterWithStreamWriter()
        {
            this.ValidateBAMFormatter(Constants.SmallSizeBAMFileNode,
                                 BAMParserParameters.StreamWriter);
        }

        /// <summary>
        ///     Validate format(seqAlignment, filename) by
        ///     parsing formatting valid BAM file.
        ///     Input : Small size BAM file.
        ///     Output : Validation of aligned sequence.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidateBAMFormatterWithFilename()
        {
            this.ValidateBAMFormatter(Constants.SmallSizeBAMFileNode,
                                 BAMParserParameters.FileName);
        }

        #endregion BAM Formatter Test Cases.

        #region BAM Sort Test Cases

        /// <summary>
        ///     Validates BAM Sort on ReadNames
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidateSortByReadNames()
        {
            using (var parser = new BAMParser())
            {
                string bamFilePath = this.utilityObj.xmlUtil.GetTextValue(Constants.MediumSizeBAMFileNode,
                                                                     Constants.FilePathNode);
                SequenceAlignmentMap seqAlignment = parser.ParseOne<SequenceAlignmentMap>(bamFilePath);
                this.ValidateSort(seqAlignment, BAMSortByFields.ReadNames);
            }
        }

        /// <summary>
        ///     Validates BAM Sort by ChromosomeCoordinates
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidateSortByChromosomeCoordinates()
        {
            using (var parser = new BAMParser())
            {
                string bamFilePath = this.utilityObj.xmlUtil.GetTextValue(Constants.MediumSizeBAMFileNode,
                                                                     Constants.FilePathNode);
                Assert.IsNotNull(bamFilePath);

                SequenceAlignmentMap seqAlignment = parser.ParseOne<SequenceAlignmentMap>(bamFilePath);
                this.ValidateSort(seqAlignment, BAMSortByFields.ChromosomeCoordinates);
            }
        }

        /// <summary>
        ///     Validates BAM Sort by ChromosomeNameAndCoordinates
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidateSortByChromosomeNameAndCoordinates()
        {
            using (var parser = new BAMParser())
            {
                string bamFilePath = this.utilityObj.xmlUtil.GetTextValue(Constants.MediumSizeBAMFileNode,
                                                                     Constants.FilePathNode);
                Assert.IsNotNull(bamFilePath);

                SequenceAlignmentMap seqAlignment = parser.ParseOne<SequenceAlignmentMap>(bamFilePath);
                this.ValidateSort(seqAlignment, BAMSortByFields.ChromosomeNameAndCoordinates);
            }
        }

        #endregion BAM Sort Test Cases

        #region SAM BAM InterConversion Test Cases

        /// <summary>
        ///     Validate BAM to SAM file conversion.
        ///     Input : BAM file.
        ///     Output : SAM file.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidateBAMToSAMConversion()
        {
            // Get values from xml config file.
            string expectedSamFilePath = this.utilityObj.xmlUtil.GetTextValue(Constants.BAMToSAMConversionNode,
                                                                         Constants.FilePathNode1);
            string bamFilePath = this.utilityObj.xmlUtil.GetTextValue(Constants.BAMToSAMConversionNode,
                                                                 Constants.FilePathNode);

            var samParserObj = new SAMParser();
            SequenceAlignmentMap expextedSamAlignmentObj = samParserObj.ParseOne<SequenceAlignmentMap>(expectedSamFilePath);

            var bamParserObj = new BAMParser();
            SequenceAlignmentMap bamSeqAlignment = bamParserObj.ParseOne<SequenceAlignmentMap>(bamFilePath);

            try
            {
                // Format BAM sequenceAlignment object to SAM file.
                var samFormatterObj = new SAMFormatter();
                samFormatterObj.Format(bamSeqAlignment, Constants.SAMTempFileName);
                SequenceAlignmentMap samSeqAlignment = samParserObj.ParseOne<SequenceAlignmentMap>(Constants.SAMTempFileName);

                Assert.IsTrue(CompareSequencedAlignmentHeader(samSeqAlignment, expextedSamAlignmentObj));
                Assert.IsTrue(CompareAlignedSequences(samSeqAlignment, expextedSamAlignmentObj));
            }
            finally
            {
                // Delete temporary file.
                File.Delete(Constants.SAMTempFileName);
            }
        }

        /// <summary>
        ///     Validate SAM to BAM file conversion.
        ///     Input : SAM file.
        ///     Output : BAM file.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidateSAMToBAMConversion()
        {
            // Get values from xml config file.
            string expectedBamFilePath = this.utilityObj.xmlUtil.GetTextValue(
                Constants.BAMToSAMConversionNode, Constants.FilePathNode);
            string samFilePath = this.utilityObj.xmlUtil.GetTextValue(
                Constants.BAMToSAMConversionNode, Constants.FilePathNode1);

            // Parse expected BAM file.
            var bamParserObj = new BAMParser();
            SequenceAlignmentMap expextedBamAlignmentObj = bamParserObj.ParseOne<SequenceAlignmentMap>(expectedBamFilePath);

            // Parse a SAM file.
            var samParserObj = new SAMParser();
            SequenceAlignmentMap samSeqAlignment = samParserObj.ParseOne<SequenceAlignmentMap>(samFilePath);

            try
            {
                // Format SAM sequenceAlignment object to BAM file.
                var bamFormatterObj = new BAMFormatter();
                bamFormatterObj.Format(samSeqAlignment, Constants.BAMTempFileName);

                // Parse a formatted BAM file.
                SequenceAlignmentMap bamSeqAlignment = bamParserObj.ParseOne<SequenceAlignmentMap>(Constants.BAMTempFileName);

                // Validate converted BAM file with expected BAM file.
                Assert.IsTrue(CompareSequencedAlignmentHeader(bamSeqAlignment, expextedBamAlignmentObj));

                // Validate BAM file aligned sequences.
                Assert.IsTrue(CompareAlignedSequences(bamSeqAlignment, expextedBamAlignmentObj));
            }
            finally
            {
                // Delete temporary file.
                File.Delete(Constants.BAMTempFileName);
            }
        }

        /// <summary>
        ///     Validate that it will not try to index an unsorted BAM
        ///     Input : SAM file sorted by name instead of alignment location.
        ///     Output: Should throw an error.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void ValidateErrorIndexingUnsortedBAM()
        {
            //samtools throws the following error, we should do the same
            //[bam_index_core] the alignment is not sorted (H0KTMADXX130517:2:1111:17648:28366): 12370 > 12324 in 25-th chr
            
            // Get filepath from xml config file.
            string bamFilePath = this.utilityObj.xmlUtil.GetTextValue(
                Constants.BAMUnsortedFilePath, Constants.FilePathNode);
            using (Stream bamStream = new FileStream(bamFilePath, FileMode.Open, FileAccess.Read))
            {
                BAMParser parser = new BAMParser();
                BAMIndex bamIndex;
                try
                {
                    bamIndex = parser.GetIndexFromBAMStorage(bamStream);
                    Assert.Fail();
                }    
                catch(InvalidDataException)
                {
                    
                }
                catch(Exception) {
                Assert.Fail();
                }
                finally
                {
                    parser.Dispose();
                }
            }
            // Log message to VSTest GUI.
            ApplicationLog.WriteLine(string.Format(null,
                                                   "BAM Parser BVT : Validated error is thrown on indexing unsorted file"));
        }


        #endregion SAM BAM InterConversion Test Cases

        #region Helper Methods

        /// <summary>
        ///     calls the Sort() method in BAMSort and checks if a BAMSortedIndex is returned.
        /// </summary>
        /// <param name="seqAlignment">SequenceAlignmentMap to be sorted</param>
        /// <param name="sortType">The sort type to be used when sorting using BAMSort</param>
        private void ValidateSort(SequenceAlignmentMap seqAlignment, BAMSortByFields sortType)
        {
            var sorter = new BAMSort(seqAlignment, sortType);
            IList<BAMSortedIndex> sortedIndex = sorter.Sort();
            Assert.IsNotNull(sortedIndex);
            Assert.IsTrue(this.IsSortedIndex(sortedIndex, sortType));
        }

        /// <summary>
        ///     Checks if the BAMSorted Index is sorted
        /// </summary>
        /// <param name="sortedIndex">The BAMSortedIndex that needs to be checked for proper sorting</param>
        /// <param name="sortType">The sort type to be used when sorting using BAMSort</param>
        /// <returns>true if properly sorted</returns>
        private bool IsSortedIndex(IList<BAMSortedIndex> sortedIndex, BAMSortByFields sortType)
        {
            bool isSorted = true;
            string matchFile1 = this.utilityObj.xmlUtil.GetTextValue(Constants.MediumSizeBAMFileNode,
                                                                Constants.MediumSizeBAMSortOutputMatchReadNames);
            string matchFile2 = this.utilityObj.xmlUtil.GetTextValue(Constants.MediumSizeBAMFileNode,
                                                                Constants
                                                                    .MediumSizeBAMSortOutputMatchChromosomeCoordinates);
            string matchFile3 = this.utilityObj.xmlUtil.GetTextValue(Constants.MediumSizeBAMFileNode,
                                                                Constants
                                                                    .MediumSizeBAMSortOutputMatchChromosomeNameAndCoordinates);
            string temp = string.Empty;

            switch (sortType)
            {
                case BAMSortByFields.ReadNames:
                    temp = File.ReadAllText(matchFile1);
                    Assert.AreEqual(temp, this.getSortedOutput(sortedIndex));
                    break;
                case BAMSortByFields.ChromosomeCoordinates:
                    temp = File.ReadAllText(matchFile2);
                    Assert.AreEqual(temp, this.getSortedOutput(sortedIndex));
                    break;
                case BAMSortByFields.ChromosomeNameAndCoordinates:
                    temp = File.ReadAllText(matchFile3);
                    Assert.AreEqual(temp, this.getSortedOutput(sortedIndex));
                    break;
                default:
                    break;
            }
            return isSorted;
        }

        /// <summary>
        ///     Gets the sorted outpur from the BAMSortedIndex
        /// </summary>
        /// <param name="sortedIndex">The BAMSortedIndex for which sortedindex values are required.</param>
        /// <returns>Sorted index values as a string</returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private string getSortedOutput(IList<BAMSortedIndex> sortedIndex)
        {
            using (var parser = new BAMParser())
            {
                BAMSortedIndex index = sortedIndex.ElementAt(0);
                IEnumerator<int> sortedIndexList = index.GetEnumerator();
                var temp = new StringBuilder();
                temp.Append(sortedIndexList.Current.ToString((IFormatProvider) null));
                while (sortedIndexList.MoveNext())
                {
                    temp.Append("|" + sortedIndexList.Current.ToString((IFormatProvider) null));
                }
                Assert.IsNotNull(temp);
                Assert.AreNotEqual(0, temp.Length);
                sortedIndexList.Dispose();
                return temp.ToString();
            }
        }

        /// <summary>
        ///     Parse BAM and validate parsed aligned sequences and its properties.
        /// </summary>
        /// <param name="nodeName">Different xml nodes used for different test cases</param>
        /// <param name="BAMParserPam">BAM Parse method parameters</param>
        /// <param name="IsEncoding">
        ///     True for BAMParser ctor with encoding.
        ///     False otherwise
        /// </param>
        private void ValidateBAMParser(string nodeName,
                                       BAMParserParameters BAMParserPam,
                                       bool IsReferenceIndex)
        {
            // Get input and output values from xml node.
            string bamFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                 Constants.FilePathNode);
            string expectedAlignedSeqFilePath = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequence);
            string refIndexValue = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.RefIndexNode);
            string startIndexValue = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.StartIndexNode);
            string endIndexValue = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.EndIndexNode);
            string alignedSeqCount = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlignedSeqCountNode);
            string refSeqName = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ChromosomeNameNode);
            SequenceAlignmentMap seqAlignment = null;
            BAMParser bamParser = null;
            try
            {
                bamParser = new BAMParser();

                // Parse a BAM file with different parameters.
                switch (BAMParserPam)
                {
                    case BAMParserParameters.StreamReader:
                        using (Stream stream = new FileStream(bamFilePath, FileMode.Open,
                                                              FileAccess.Read))
                        {
                            seqAlignment = bamParser.ParseOne(stream);
                        }
                        break;
                    case BAMParserParameters.FileName:
                        seqAlignment = bamParser.ParseOne<SequenceAlignmentMap>(bamFilePath);
                        break;
                    case BAMParserParameters.ParseRangeFileName:
                        seqAlignment = bamParser.ParseRange(bamFilePath,
                                                            Convert.ToInt32(refIndexValue, null));
                        break;
                    case BAMParserParameters.ParseRangeWithIndex:
                        seqAlignment = bamParser.ParseRange(bamFilePath,
                                                            Convert.ToInt32(refIndexValue, null),
                                                            Convert.ToInt32(startIndexValue, null),
                                                            Convert.ToInt32(endIndexValue, null));
                        break;
                    case BAMParserParameters.ParseRangeUsingRefSeq:
                        seqAlignment = bamParser.ParseRange(bamFilePath, refSeqName);
                        break;
                    case BAMParserParameters.ParseRangeUsingRefSeqAndFlag:
                        seqAlignment = bamParser.ParseRange(bamFilePath, refSeqName);
                        break;
                    case BAMParserParameters.ParseRangeUsingRefSeqUsingIndex:
                        seqAlignment = bamParser.ParseRange(bamFilePath, refSeqName,
                                                            Convert.ToInt32(startIndexValue, null),
                                                            Convert.ToInt32(endIndexValue, null));
                        break;
                    case BAMParserParameters.ParseRangeUsingIndexesAndFlag:
                        seqAlignment = bamParser.ParseRange(bamFilePath, refSeqName,
                                                            Convert.ToInt32(startIndexValue, null),
                                                            Convert.ToInt32(endIndexValue, null));
                        break;
                }

                // Validate BAM Header record fileds.
                if (!IsReferenceIndex)
                {
                    this.ValidateBAMHeaderRecords(nodeName, seqAlignment);
                }

                IList<SAMAlignedSequence> alignedSeqs = seqAlignment.QuerySequences;
                Assert.AreEqual(alignedSeqCount, alignedSeqs.Count.ToString((IFormatProvider) null));
                // Get expected sequences
                var parserObj = new FastAParser();
                {
                    IEnumerable<ISequence> expectedSequences = parserObj.Parse(expectedAlignedSeqFilePath);
                    IList<ISequence> expectedSequencesList = expectedSequences.ToList();
                    // Validate aligned sequences from BAM file.
                    for (int index = 0; index < alignedSeqs.Count; index++)
                    {
                        Assert.IsFalse(alignedSeqs[index].IsDummyRead);
                        Assert.AreEqual(
                            new string(expectedSequencesList[index].Select(a => (char) a).ToArray()),
                            new string(alignedSeqs[index].QuerySequence.Select(a => (char) a).ToArray()));
                        // Log to VSTest GUI.
                        ApplicationLog.WriteLine(string.Format(null,
                                                               "BAM Parser BVT : Validated Aligned sequence :{0} successfully",
                                                               alignedSeqs[index].QuerySequence));
                    }
                }
            }
            finally
            {
                bamParser.Dispose();
            }
        }

        /// <summary>
        ///     Validate BAM file Header fields.
        /// </summary>
        /// <param name="nodeName">XML nodename used for different test cases</param>
        /// <param name="seqAlignment">seqAlignment object</param>
        private void ValidateBAMHeaderRecords(string nodeName,
                                              SequenceAlignmentMap seqAlignment)
        {
            string expectedHeaderTagValues = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.RecordTagValuesNode);
            string expectedHeaderTagKeys = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.RecordTagKeysNode);
            string expectedHeaderTypes = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.HeaderTyepsNodes);
            string[] expectedHeaderTagsValues = expectedHeaderTagValues.Split(',');
            string[] expectedHeaderKeys = expectedHeaderTagKeys.Split(',');
            string[] expectedHeaders = expectedHeaderTypes.Split(',');
            SAMAlignmentHeader header = seqAlignment.Header;
            IList<SAMRecordField> recordFields = header.RecordFields;
            int tagKeysCount = 0;
            int tagValuesCount = 0;

            for (int index = 0; index < recordFields.Count; index++)
            {
                Assert.AreEqual(expectedHeaders[index].Replace("/", ""),
                                recordFields[index].Typecode.ToString(null).Replace("/", ""));
                for (int tags = 0; tags < recordFields[index].Tags.Count; tags++)
                {
                    Assert.AreEqual(expectedHeaderKeys[tagKeysCount].Replace("/", ""),
                                    recordFields[index].Tags[tags].Tag.ToString(null).Replace("/", ""));
                    Assert.AreEqual(expectedHeaderTagsValues[tagValuesCount].Replace("/", ""),
                                    recordFields[index].Tags[tags].Value.ToString(null)
                                                                  .Replace("/", "")
                                                                  .Replace("\r", "")
                                                                  .Replace("\n", ""));
                    tagKeysCount++;
                    tagValuesCount++;
                }
            }
        }

        /// <summary>
        ///     Validate formatted BAM file.
        /// </summary>
        /// <param name="nodeName">Different xml nodes used for different test cases</param>
        /// <param name="BAMParserPam">BAM Format method parameters</param>
        private void ValidateBAMFormatter(string nodeName,
                                          BAMParserParameters BAMParserPam)
        {
            // Get input and output values from xml node.
            string bamFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                 Constants.FilePathNode);
            string expectedAlignedSeqFilePath = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequence);
            string alignedSeqCount = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlignedSeqCountNode);

            using (var bamParserObj = new BAMParser())
            {
                // Parse a BAM file.
                var seqAlignment = bamParserObj.ParseOne<SequenceAlignmentMap>(bamFilePath);
                // Create a BAM formatter object.
                var formatterObj = new BAMFormatter();
                // Write/Format aligned sequences to BAM file.
                switch (BAMParserPam)
                {
                    case BAMParserParameters.StreamWriter:
                        Stream stream;
                        using (stream = new FileStream(Constants.BAMTempFileName, FileMode.Create, FileAccess.Write))
                        {
                            formatterObj.Format(stream, seqAlignment);
                        }
                        break;
                    case BAMParserParameters.FileName:
                        formatterObj.Format(seqAlignment, Constants.BAMTempFileName);
                        break;
                    case BAMParserParameters.IndexFile:
                        formatterObj.Format(seqAlignment, Constants.BAMTempFileName, Constants.BAMTempIndexFile);
                        File.Exists(Constants.BAMTempIndexFile);
                        break;
                    default:
                        break;
                }

                // Parse formatted BAM file and validate aligned sequences.
                SequenceAlignmentMap expectedSeqAlignmentMap = bamParserObj.ParseOne<SequenceAlignmentMap>(Constants.BAMTempFileName);

                // Validate Parsed BAM file Header record fileds.
                this.ValidateBAMHeaderRecords(nodeName, expectedSeqAlignmentMap);
                IList<SAMAlignedSequence> alignedSeqs = expectedSeqAlignmentMap.QuerySequences;
                Assert.AreEqual(alignedSeqCount, alignedSeqs.Count.ToString((IFormatProvider) null));

                // Get expected sequences
                var parserObj = new FastAParser();
                {
                    IEnumerable<ISequence> expectedSequences = parserObj.Parse(expectedAlignedSeqFilePath);
                    IList<ISequence> expectedSequencesList = expectedSequences.ToList();
                    // Validate aligned sequences from BAM file.
                    for (int index = 0; index < alignedSeqs.Count; index++)
                    {
                        Assert.AreEqual(
                            new string(expectedSequencesList[index].Select(a => (char) a).ToArray()),
                            new string(alignedSeqs[index].QuerySequence.Select(a => (char) a).ToArray()));
                        // Log to VSTest GUI.
                        ApplicationLog.WriteLine(string.Format("BAM Formatter BVT : Validated Aligned sequence :{0} successfully", alignedSeqs[index].QuerySequence));
                    }
                }
            }
            File.Delete(Constants.BAMTempFileName);
            File.Delete(Constants.BAMTempIndexFile);
        }

        /// <summary>
        ///     Comapare Sequence Alignment Header fields
        /// </summary>
        /// <param name="actualAlignment">Actual sequence alignment object</param>
        /// <param name="expectedAlignment">Expected sequence alignment object</param>
        /// <returns></returns>
        private static bool CompareSequencedAlignmentHeader(SequenceAlignmentMap actualAlignment,
                                                            SequenceAlignmentMap expectedAlignment)
        {
            SAMAlignmentHeader aheader = actualAlignment.Header;
            IList<SAMRecordField> arecordFields = aheader.RecordFields;
            SAMAlignmentHeader expectedheader = expectedAlignment.Header;
            IList<SAMRecordField> expectedrecordFields = expectedheader.RecordFields;
            int tagKeysCount = 0;
            int tagValuesCount = 0;

            for (int index = 0; index < expectedrecordFields.Count; index++)
            {
                if (0 != string.Compare(expectedrecordFields[index].Typecode.ToString(null),
                                        arecordFields[index].Typecode.ToString(null), StringComparison.CurrentCulture))
                {
                    return false;
                }
                for (int tags = 0; tags < expectedrecordFields[index].Tags.Count; tags++)
                {
                    if ((0 != string.Compare(expectedrecordFields[index].Tags[tags].Tag.ToString(null),
                                             arecordFields[index].Tags[tags].Tag.ToString(null),
                                             StringComparison.CurrentCulture))
                        || (0 != string.Compare(expectedrecordFields[index].Tags[tags].Value.ToString(null),
                                                arecordFields[index].Tags[tags].Value.ToString(null),
                                                StringComparison.CurrentCulture)))
                    {
                        ApplicationLog.WriteLine(string.Format(null,
                                                               "BAM Parser BVT : Sequence alignment header does not match"));
                        return false;
                    }
                    tagKeysCount++;
                    tagValuesCount++;
                }
            }

            return true;
        }

        /// <summary>
        ///     Compare BAM file aligned sequences.
        /// </summary>
        /// <param name="expectedAlignment">Expected sequence alignment object</param>
        /// <param name="actualAlignment">Actual sequence alignment object</param>
        /// <returns></returns>
        private static bool CompareAlignedSequences(SequenceAlignmentMap expectedAlignment,
                                                    SequenceAlignmentMap actualAlignment)
        {
            IList<SAMAlignedSequence> actualAlignedSeqs = actualAlignment.QuerySequences;
            IList<SAMAlignedSequence> expectedAlignedSeqs = expectedAlignment.QuerySequences;

            if (
                expectedAlignedSeqs.Where(
                    (t, i) =>
                    0 !=
                    string.Compare(
                        new string(expectedAlignedSeqs.ElementAt(i).QuerySequence.Select(a => (char) a).ToArray()),
                        new string(actualAlignedSeqs[i].QuerySequence.Select(a => (char) a).ToArray()),
                        true, CultureInfo.CurrentCulture)).Any())
            {
                ApplicationLog.WriteLine(string.Format(null,
                                                       "BAM Parser BVT : Sequence alignment aligned seq does match"));
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Validate GetPaired method
        /// </summary>
        /// <param name="nodeName">XML node name</param>
        /// <param name="pams">GetPairedReads method parameters</param>
        private void ValidatePairedReads(string nodeName, GetPairedReadParameters pams)
        {
            // Get input and output values from xml node.
            string bamFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                 Constants.FilePathNode);
            string expectedAlignedSeqFilePath = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequence);
            string mean = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.MeanNode);
            string deviation = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DeviationValueNode);
            string library = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LibraryNameNode);
            string pairedReadsCount = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.PairedReadsNode);
            string[] insertLength = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.InsertLengthNode).Split(',');
            string[] pairedReadType = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.PairedReadTypeNode).Split(',');

            SequenceAlignmentMap seqAlignment = null;
            IList<PairedRead> pairedReads = null;
            var bamParser = new BAMParser();
            var parserObj = new FastAParser();

            try
            {
                seqAlignment = bamParser.ParseOne<SequenceAlignmentMap>(bamFilePath);
                IEnumerable<ISequence> expectedSequences = parserObj.Parse(expectedAlignedSeqFilePath);

                switch (pams)
                {
                    case GetPairedReadParameters.GetPairedReadWithParameters:
                        pairedReads = seqAlignment.GetPairedReads(float.Parse(mean, null),
                                                                  float.Parse(deviation, null));
                        break;
                    case GetPairedReadParameters.GetPairedReadWithLibraryName:
                        pairedReads = seqAlignment.GetPairedReads(library);
                        break;
                    case GetPairedReadParameters.GetPairedReadWithCloneLibraryInfo:
                        CloneLibraryInformation libraryInfo =
                            CloneLibrary.Instance.GetLibraryInformation(library);
                        pairedReads = seqAlignment.GetPairedReads(libraryInfo);
                        break;
                    case GetPairedReadParameters.Default:
                        pairedReads = seqAlignment.GetPairedReads();
                        break;
                }

                Assert.AreEqual(pairedReadsCount, pairedReads.Count.ToString((IFormatProvider) null));

                int i = 0;
                foreach (PairedRead read in pairedReads)
                {
                    Assert.AreEqual(insertLength[i], read.InsertLength.ToString((IFormatProvider) null));
                    Assert.AreEqual(pairedReadType[i], read.PairedType.ToString());

                    foreach (SAMAlignedSequence seq in read.Reads)
                    {
                        Assert.AreEqual(new string(expectedSequences.ElementAt(i).Select(a => (char) a).ToArray()),
                                        new string(seq.QuerySequence.Select(a => (char) a).ToArray()));

                        // Log to VSTest GUI.
                        ApplicationLog.WriteLine(string.Format(null,
                                                               "BAM Parser BVT : Validated Paired read :{0} successfully",
                                                               seq.QuerySequence));
                    }
                    i++;
                }
            }

            finally
            {
                bamParser.Dispose();
            }
        }

        /// <summary>
        ///     Validate different paired read types
        /// </summary>
        /// <param name="nodeName">XML node name</param>
        /// <param name="pams">GetPairedReadTypes method parameters</param>
        private void ValidatePairedReadTypes(string nodeName, GetPairedReadTypeParameters pams)
        {
            // Get input and output values from xml node.
            string bamFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                 Constants.FilePathNode);
            string mean = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.MeanNode);
            string deviation = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DeviationValueNode);
            string library = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LibraryNameNode);
            string[] pairedReadType = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.PairedReadTypeNode).Split(',');
            string[] insertLength = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.InsertLengthNode).Split(',');

            var bamParser = new BAMParser();
            SequenceAlignmentMap seqAlignmentMapObj = bamParser.ParseOne<SequenceAlignmentMap>(bamFilePath);
            CloneLibraryInformation libraryInfo;
            int i = 0;
            try
            {
                IList<PairedRead> pairedReads;
                switch (pams)
                {
                    case GetPairedReadTypeParameters.PaireReadTypeUsingLibraryName:
                        pairedReads = seqAlignmentMapObj.GetPairedReads(float.Parse(mean, null),
                                                                        float.Parse(deviation, null));
                        foreach (PairedRead read in pairedReads)
                        {
                            PairedReadType type = PairedRead.GetPairedReadType(read, library);
                            Assert.AreEqual(type.ToString(), pairedReadType[i]);
                            i++;
                        }
                        break;
                    case GetPairedReadTypeParameters.PaireReadTypeUsingCloneLibraryInfo:
                        pairedReads = seqAlignmentMapObj.GetPairedReads(float.Parse(mean, null),
                                                                        float.Parse(deviation, null));
                        libraryInfo = CloneLibrary.Instance.GetLibraryInformation(library);
                        foreach (PairedRead read in pairedReads)
                        {
                            PairedReadType type = PairedRead.GetPairedReadType(read, libraryInfo);
                            Assert.AreEqual(type.ToString(), pairedReadType[i]);
                            i++;
                        }
                        break;
                    case GetPairedReadTypeParameters.PaireReadTypeUsingMeanAndDeviation:
                        pairedReads = seqAlignmentMapObj.GetPairedReads(float.Parse(mean, null),
                                                                        float.Parse(deviation, null));
                        foreach (PairedRead read in pairedReads)
                        {
                            PairedReadType type = PairedRead.GetPairedReadType(read, float.Parse(mean, null),
                                                                               float.Parse(deviation, null));
                            Assert.AreEqual(type.ToString(), pairedReadType[i]);
                            i++;
                        }
                        break;
                    case GetPairedReadTypeParameters.PaireReadTypeUsingReadsAndLibrary:
                        pairedReads = seqAlignmentMapObj.GetPairedReads(float.Parse(mean, null),
                                                                        float.Parse(deviation, null));
                        foreach (PairedRead read in pairedReads)
                        {
                            PairedReadType type = PairedRead.GetPairedReadType(read.Read1,
                                                                               read.Read2, library);
                            Assert.AreEqual(type.ToString(), pairedReadType[i]);
                            i++;
                        }
                        break;
                    case GetPairedReadTypeParameters.PaireReadTypeUsingReadsAndLibraryInfo:
                        pairedReads = seqAlignmentMapObj.GetPairedReads(float.Parse(mean, null),
                                                                        float.Parse(deviation, null));
                        libraryInfo = CloneLibrary.Instance.GetLibraryInformation(library);
                        foreach (PairedRead read in pairedReads)
                        {
                            PairedReadType type = PairedRead.GetPairedReadType(read.Read1,
                                                                               read.Read2, libraryInfo);
                            Assert.AreEqual(type.ToString(), pairedReadType[i]);
                            i++;
                        }
                        break;
                    case GetPairedReadTypeParameters.GetInsertLength:
                        pairedReads = seqAlignmentMapObj.GetPairedReads(float.Parse(mean, null),
                                                                        float.Parse(deviation, null));
                        libraryInfo = CloneLibrary.Instance.GetLibraryInformation(library);
                        foreach (PairedRead read in pairedReads)
                        {
                            int length = PairedRead.GetInsertLength(read.Read1, read.Read2);
                            Assert.AreEqual(length.ToString((IFormatProvider) null), insertLength[i]);
                            i++;
                        }
                        break;
                }
                ApplicationLog.WriteLine(string.Format(null,
                                                       "BAM Parser BVT : Validated Paired read Type Successfully"));
            }

            finally
            {
                bamParser.Dispose();
            }
        }

        #endregion Helper Methods
    }
}