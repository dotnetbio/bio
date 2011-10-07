using System.IO;
using System.Linq;
using Bio.IO.BAM;
using Bio.IO.SAM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamUtil;

namespace Bio.Tests
{
    /// <summary>
    /// Test for SAMUtils(sort, merge, view, import, index)
    /// </summary>
    [TestClass]
    public class SAMUtilTests
    {

        #region Import Option
        /// <summary>
        /// Test Import option of SAMUtility using small size SAM file. (SAM => BAM conversion)
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ImportTestWithBAM()
        {
            Import options = new Import();
            options.FilePath = new string[2];
            string tempFilename = Path.GetTempFileName() + ".bam";
            options.FilePath[0] = tempFilename;
            options.FilePath[1] = @"TestUtils\SAM\SeqAlignment.sam";
            options.DoImport();

            using (SAMParser parser = new SAMParser())
            {
                SequenceAlignmentMap map = parser.Parse(@"TestUtils\SAM\SeqAlignment.sam");
                using (BAMParser parse = new BAMParser())
                {
                    SequenceAlignmentMap map1 = parse.Parse(tempFilename);
                    Assert.IsTrue(CompareSAM(map, map1));
                }
            }
            File.Delete(tempFilename);
        }

        /// <summary>
        /// Test Import option of SAMUtility using small size SAM file. (BAM => SAM conversion)
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ImportTestWithSAM()
        {
            Import options = new Import();
            options.FilePath = new string[2];
            string tempFilename = Path.GetTempFileName();
            options.FilePath[0] = tempFilename;
            options.FilePath[1] = @"TestUtils\SAM\SeqAlignment.bam";
            options.DoImport();

            using (BAMParser parser = new BAMParser())
            {
                SequenceAlignmentMap map = parser.Parse(@"TestUtils\SAM\SeqAlignment.bam");
                using (SAMParser parse = new SAMParser())
                {
                    SequenceAlignmentMap map1 = parse.Parse(tempFilename);
                    Assert.IsTrue(CompareSAM(map, map1));
                }
            }

            File.Delete(tempFilename);

        }

        #endregion

        #region Index Option

        /// <summary>
        /// Test Index option of SAMUtility.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void IndexTest()
        {
            Index option = new Index();
            option.FilePath = new string[2];
            option.FilePath[0] = @"TestUtils\SAM\SeqAlignment.bam";
            string tempFile = Path.GetTempFileName();
            option.FilePath[1] = tempFile;
            option.GenerateIndexFile();
            Assert.IsTrue(File.Exists(tempFile));
            File.Delete(tempFile);
        }

        #endregion

        #region Sort Option

        /// <summary>
        /// Test sort option of SAMUtility.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void SortTest()
        {
            Sort option = new Sort();
            option.FilePaths = new string[2];
            option.FilePaths[0] = @"TestUtils\SAM\SeqAlignment.bam";
            string tempFile = Path.GetTempFileName();
            option.FilePaths[1] = tempFile;
            option.SortByReadName = true;
            option.DoSort();
            using (BAMParser parser = new BAMParser())
            {
                SequenceAlignmentMap map = parser.Parse(@"TestUtils\SAM\SeqAlignment.bam");
                SequenceAlignmentMap map1 = parser.Parse(tempFile);

                Assert.AreEqual(map.QuerySequences.Count, map1.QuerySequences.Count);
                Assert.AreEqual(map.QuerySequences[0].QName, map1.QuerySequences[1].QName);
                Assert.AreEqual(map.QuerySequences[1].QName, map1.QuerySequences[0].QName);
            }
            File.Delete(tempFile);
        }

        #endregion

        #region Merge Option

        /// <summary>
        /// Test sort option of SAMUtility.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void MergeTest()
        {
            Merge options = new Merge();
            options.FilePaths = new string[3];
            options.SortByReadName = true;
            string tempFile = Path.GetTempFileName();
            options.FilePaths[0] = tempFile;
            options.FilePaths[1] = @"TestUtils\SAM\SeqAlignment.bam";
            options.FilePaths[2] = @"TestUtils\SAM\SeqAlignment.bam";
            options.DoMerge();
            using (BAMParser parser = new BAMParser())
            {
                SequenceAlignmentMap map = parser.Parse(@"TestUtils\SAM\SeqAlignment.bam");
                SequenceAlignmentMap map1 = parser.Parse(tempFile);
                Assert.AreEqual(map.QuerySequences.Count * 2, map1.QuerySequences.Count);
            }
            File.Delete(tempFile);
        }

        #endregion

        #region View Option

        /// <summary>
        /// Test sort option of SAMUtility.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ViewTest()
        {
            View option = new View();
            option.InputFilePath = @"TestUtils\SAM\SeqAlignment.bam";
            option.Header = true;
            string tempFile = Path.GetTempFileName();
            option.OutputFilePath = tempFile;
            option.ViewResult();
            using (BAMParser parser = new BAMParser())
            {
                SequenceAlignmentMap map = parser.Parse(@"TestUtils\SAM\SeqAlignment.bam");
                using (SAMParser parse = new SAMParser())
                {
                    SequenceAlignmentMap map1 = parse.Parse(tempFile);
                    Assert.IsTrue(CompareSAM(map, map1));
                }
            }
        }

        #endregion

        #region Private Method

        /// <summary>
        /// Compare two SAM objects
        /// </summary>
        /// <param name="map">First SAM object.</param>
        /// <param name="map1">Second DAM object.</param>
        /// <returns>Whether two objects are equal or not.</returns>
        private static bool CompareSAM(SequenceAlignmentMap map, SequenceAlignmentMap map1)
        {
            bool comparison = false;
            if (map.Header.RecordFields.Count == map1.Header.RecordFields.Count &&
                map.Header.Comments.Count == map1.Header.Comments.Count)
            {
                if (map.Header.RecordFields.All(
                    a => map1.Header.RecordFields.Where(
                        b => a.Tags.All(
                            c => b.Tags.Where(
                                d => c.Tag.Equals(d.Tag) && c.Value.Equals(d.Value)).ToList().Count > 0)).ToList().Count > 0))
                {
                    if (map.QuerySequences.Count == map1.QuerySequences.Count)
                    {
                        if (map.QuerySequences.AsParallel().All(e => map1.QuerySequences.Where(f => e.Bin == f.Bin && e.CIGAR == f.CIGAR).ToList().Count > 0))
                        {
                            comparison = true;
                        }
                    }
                }
            }

            return comparison;
        }

        #endregion
    }
}
