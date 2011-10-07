using System.IO;
using Bio;
using Bio.IO.BAM;
using Bio.IO.SAM;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests.IO.BAM
{
    /// <summary>
    /// Test BAM format parser and formatter.
    /// </summary>
    [TestClass]
    public class BAMTests
    {

        /// <summary>
        /// Test the BAM Parser.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestParser()
        {
            string filePath = @"TestUtils\BAM\SeqAlignment.bam";
            BAMParser parser = null;
            try
            {
                parser = new BAMParser();
                SequenceAlignmentMap alignmentMap = parser.Parse(filePath);
                Assert.IsTrue(alignmentMap != null);
                Assert.AreEqual(alignmentMap.Header.GetReferenceSequences().Count, 1);
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
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestFormatter()
        {
            string filePath = @"TestUtils\BAM\SeqAlignment.bam";
            string outputfilePath = "BAMTests123.bam";
            BAMParser parser = null;
            try
            {
                parser = new BAMParser();
                BAMFormatter formatter = new BAMFormatter();
                SequenceAlignmentMap alignmentMap = parser.Parse(filePath);

                Assert.IsTrue(alignmentMap != null);
                Assert.AreEqual(alignmentMap.Header.GetReferenceSequences().Count, 1);
                Assert.AreEqual(alignmentMap.QuerySequences.Count, 2);

                formatter.Format(alignmentMap, outputfilePath);

                formatter.CreateSortedBAMFile = true;
                formatter.CreateIndexFile = true;
                alignmentMap = parser.Parse(filePath);
                formatter.Format(alignmentMap, outputfilePath);

                Assert.IsTrue(File.Exists("BAMTests123.bam.bai"));

                alignmentMap = parser.Parse(outputfilePath);

                Assert.IsTrue(alignmentMap != null);
                Assert.AreEqual(alignmentMap.Header.GetReferenceSequences().Count, 1);
                Assert.AreEqual(alignmentMap.QuerySequences.Count, 2);

                alignmentMap = parser.ParseRange("BAMTests123.bam", new SequenceRange("chr20", 0, int.MaxValue));

                Assert.IsTrue(alignmentMap != null);
                Assert.AreEqual(alignmentMap.Header.GetReferenceSequences().Count, 1);
                Assert.AreEqual(alignmentMap.QuerySequences.Count, 2);

                alignmentMap = parser.ParseRange("BAMTests123.bam", new SequenceRange("chr20", 0, 28833));

                Assert.IsTrue(alignmentMap != null);
                Assert.AreEqual(alignmentMap.Header.GetReferenceSequences().Count, 1);
                Assert.AreEqual(alignmentMap.QuerySequences.Count, 1);
            }
            finally
            {
                if (parser != null)
                    parser.Dispose();
            }
        }

        /// <summary>
        /// Tests the name,description and file extension property of 
        /// BAM formatter and parser.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
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
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestFormatterWithSort()
        {
            string inputFilePath = @"TestUtils\BAM\SeqAlignment.bam";
            string outputFilePath1 = "output1.bam";
            string outputFilePath2 = "output2.bam";
            BAMParser parser = null;
            try
            {
                parser = new BAMParser();
                BAMFormatter formatter = new BAMFormatter();
                SequenceAlignmentMap alignmentMap = parser.Parse(inputFilePath);

                Assert.IsTrue(alignmentMap != null);
                Assert.AreEqual(alignmentMap.Header.GetReferenceSequences().Count, 1);
                Assert.AreEqual(alignmentMap.QuerySequences.Count, 2);

                formatter.CreateSortedBAMFile = true;
                formatter.SortType = BAMSortByFields.ChromosomeCoordinates;
                formatter.Format(alignmentMap, outputFilePath1);

                alignmentMap = parser.Parse(inputFilePath);
                formatter.Format(alignmentMap, outputFilePath2);

                Assert.IsTrue(File.Exists(outputFilePath1));
                Assert.IsTrue(File.Exists(outputFilePath2));

                Assert.AreEqual(true, FileCompare(outputFilePath1, outputFilePath2));
            }
            finally
            {
                if (parser != null)
                    parser.Dispose();
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
            FileStream fs1;
            FileStream fs2;

            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open);
            fs2 = new FileStream(file2, FileMode.Open);

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
