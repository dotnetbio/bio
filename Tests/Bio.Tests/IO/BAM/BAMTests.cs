using System.IO;
using System.Linq;

using Bio;
using Bio.IO.BAM;
using Bio.IO.SAM;
using NUnit.Framework;

namespace Bio.Tests.IO.BAM
{
    /// <summary>
    /// Test BAM format parser and formatter.
    /// </summary>
    [TestFixture]
    public class BAMTests
    {

        /// <summary>
        /// Test the BAM Parser.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void TestParser()
        {
            string FilePath = @"TestUtils\BAM\SeqAlignment.bam".TestDir();
            BAMParser parser = null;
            try
            {
                parser = new BAMParser();
                SequenceAlignmentMap alignmentMap = parser.ParseOne<SequenceAlignmentMap>(FilePath);
                Assert.IsTrue(alignmentMap != null);
                Assert.AreEqual(alignmentMap.Header.GetReferenceSequencesInfoFromSQHeader().Count, 1);
                Assert.AreEqual(alignmentMap.QuerySequences.Count, 2);
            }
            finally
            {
                if (parser != null)
                    parser.Dispose();
            }
        }

        /// <summary>
        /// Test the BAM Formatter.
        /// </summary>
        [Test]
        [Category("Priority0"),Category("BAM")]
        public void TestFormatter()
        {
            string filePath = @"TestUtils\BAM\SeqAlignment.bam".TestDir();
            const string outputfilePath = "BAMTests123.bam";
            string outputIndexFile = outputfilePath + ".bai";
            BAMParser parser = new BAMParser();
            SequenceAlignmentMap alignmentMap = parser.ParseOne<SequenceAlignmentMap>(filePath);

            Assert.IsNotNull(alignmentMap);
            Assert.IsNotNull(alignmentMap.Header);
            Assert.AreEqual(alignmentMap.Header.GetReferenceSequencesInfoFromSQHeader().Count, 1);
            Assert.AreEqual(alignmentMap.Header.ReferenceSequences.Count, 1);
            Assert.IsNotNull(alignmentMap.QuerySequences);
            Assert.AreEqual(alignmentMap.QuerySequences.Count, 2);

            BAMFormatter formatter = new BAMFormatter();
            formatter.Format(alignmentMap, outputfilePath);

            formatter.CreateSortedBAMFile = true;
            formatter.CreateIndexFile = true;
            formatter.Format(alignmentMap, outputfilePath);
            Assert.IsTrue(File.Exists("BAMTests123.bam.bai"));

            alignmentMap = parser.ParseOne<SequenceAlignmentMap>(outputfilePath);
            Assert.IsNotNull(alignmentMap);
            Assert.IsNotNull(alignmentMap.Header);
            Assert.AreEqual(alignmentMap.Header.GetReferenceSequencesInfoFromSQHeader().Count, 1);
            Assert.AreEqual(alignmentMap.Header.ReferenceSequences.Count, 1);
            Assert.AreEqual(alignmentMap.QuerySequences.Count, 2);

            alignmentMap = parser.ParseRange("BAMTests123.bam", new SequenceRange("chr20", 0, int.MaxValue));
            Assert.IsNotNull(alignmentMap);
            Assert.IsNotNull(alignmentMap.Header);
            Assert.AreEqual(alignmentMap.Header.GetReferenceSequencesInfoFromSQHeader().Count, 1);
            Assert.AreEqual(alignmentMap.Header.ReferenceSequences.Count, 1);
            Assert.AreEqual(alignmentMap.QuerySequences.Count, 2);

            alignmentMap = parser.ParseRange("BAMTests123.bam", new SequenceRange("chr20", 0, 28833));
            Assert.IsNotNull(alignmentMap);
            Assert.IsNotNull(alignmentMap.Header);
            Assert.AreEqual(alignmentMap.Header.GetReferenceSequencesInfoFromSQHeader().Count, 1);
            Assert.AreEqual(alignmentMap.Header.ReferenceSequences.Count, 1);
            Assert.AreEqual(alignmentMap.QuerySequences.Count, 1);

            File.Delete(outputfilePath);
            File.Delete(outputIndexFile);
        }

        /// <summary>
        /// Tests the name,description and file extension property of 
        /// BAM formatter and parser.
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void BAMProperties()
        {
            using (BAMParser parser = new BAMParser())
            {
                Assert.AreEqual(parser.Name, Properties.Resource.BAM_NAME);
                Assert.AreEqual(parser.Description, Properties.Resource.BAMPARSER_DESCRIPTION);
                Assert.AreEqual(parser.SupportedFileTypes, Properties.Resource.BAM_FILEEXTENSION);
            }

            BAMFormatter formatter = new BAMFormatter();
            Assert.AreEqual(formatter.Name, Properties.Resource.BAM_NAME);
            Assert.AreEqual(formatter.Description, Properties.Resource.BAMFORMATTER_DESCRIPTION);
            Assert.AreEqual(formatter.SupportedFileTypes, Properties.Resource.BAM_FILEEXTENSION);

        }

        /// <summary>
        /// Test the BAM Formatter with sort by coordinate option
        /// </summary>
        [Test]
        [Category("Priority0"), Category("BAM")]
        public void TestFormatterWithSort()
        {
            string inputFilePath = @"TestUtils\BAM\SeqAlignment.bam".TestDir();
            string outputFilePath1 = "output1.bam";
            string outputFilePath2 = "output2.bam";
            BAMParser parser = null;
            try
            {
                parser = new BAMParser();
                BAMFormatter formatter = new BAMFormatter();
                SequenceAlignmentMap alignmentMap = parser.ParseOne<SequenceAlignmentMap>(inputFilePath);

                Assert.IsTrue(alignmentMap != null);
                Assert.AreEqual(alignmentMap.Header.GetReferenceSequencesInfoFromSQHeader().Count, 1);
                Assert.AreEqual(alignmentMap.Header.ReferenceSequences.Count, 1);
                Assert.AreEqual(alignmentMap.QuerySequences.Count, 2);

                formatter.CreateSortedBAMFile = true;
                formatter.SortType = BAMSortByFields.ChromosomeCoordinates;
                formatter.Format(alignmentMap, outputFilePath1);

                alignmentMap = parser.ParseOne<SequenceAlignmentMap>(inputFilePath);
                formatter.Format(alignmentMap, outputFilePath2);

                Assert.IsTrue(File.Exists(outputFilePath1));
                Assert.IsTrue(File.Exists(outputFilePath2));

                Assert.AreEqual(true, FileCompare(outputFilePath1, outputFilePath2));
            }
            finally
            {
                if (parser != null)
                    parser.Dispose();
                File.Delete(outputFilePath1);
                File.Delete(outputFilePath2);
            }
        }

        // This method accepts two strings the represent two files to 
        // compare. A return value of 0 indicates that the contents of the files
        // are the same. A return value of any other value indicates that the 
        // files are not the same.
        private static bool FileCompare(string file1, string file2)
        {
            int file1byte;
            int file2byte;

            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            FileStream fs1 = new FileStream(file1, FileMode.Open);
            FileStream fs2 = new FileStream(file2, FileMode.Open);

            // Check the file sizes. If they are not the same, the files 
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                // Close the file
                fs1.Close();
                fs2.Close();

                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            // Close the files.
            fs1.Dispose();
            fs2.Dispose();
            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return ((file1byte - file2byte) == 0);
        }

    }
}
