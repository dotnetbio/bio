using System.IO;
using Bio.IO;
using Bio.IO.FastA;
using Bio.IO.FastQ;
using Bio.IO.GenBank;
using Bio.IO.Gff;
using Bio.IO.SFF;
using Bio.IO.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests.IO
{
    /// <summary>
    /// Unit tests for the SequenceParser class
    /// </summary>
    [TestClass]
    public class SequenceParserTests
    {
        /// <summary>
        /// Tests a normal .FASTA file extension.
        /// </summary>
        [TestMethod]
        public void TestFastAFileExtension()
        {
            string[] extensions = {".fa", ".fas", ".fasta", ".fna", ".fsa", ".mpfa"};
            string filepath = @"TestUtils\Simple_Fasta_DNA";

            foreach (var ext in extensions)
            {
                ISequenceParser foundParser = SequenceParsers.FindParserByFileName(filepath + ext);
                Assert.IsNotNull(foundParser);
                Assert.IsInstanceOfType(foundParser, typeof(FastAParser));
            }
        }

        /// <summary>
        /// Tests a normal .fastq file extension.
        /// </summary>
        [TestMethod]
        public void TestFastQFileExtension()
        {
            string[] extensions = { ".fq", ".fastq" };
            string filepath = @"TestUtils\SimpleDnaIllumina";

            foreach (var ext in extensions)
            {
                ISequenceParser foundParser = SequenceParsers.FindParserByFileName(filepath + ext);
                Assert.IsNotNull(foundParser);
                Assert.IsInstanceOfType(foundParser, typeof(FastQParser));
            }
        }

        /// <summary>
        /// Tests a normal .genbank file extension.
        /// </summary>
        [TestMethod]
        public void TestGenBankFileExtension()
        {
            string[] extensions = { ".gbk", ".genbank" };
            string filepath = @"TestUtils\Simple_GenBank_DNA";

            foreach (var ext in extensions)
            {
                ISequenceParser foundParser = SequenceParsers.FindParserByFileName(filepath + ext);
                Assert.IsNotNull(foundParser);
                Assert.IsInstanceOfType(foundParser, typeof(GenBankParser));
            }
        }

        /// <summary>
        /// Tests a normal .gff file extension.
        /// </summary>
        [TestMethod]
        public void TestGffFileExtension()
        {
            string filepath = @"TestUtils\Simple_Gff_Dna.gff";

            ISequenceParser foundParser = SequenceParsers.FindParserByFileName(filepath);
            Assert.IsNotNull(foundParser);
            Assert.IsInstanceOfType(foundParser, typeof(GffParser));
        }

        /// <summary>
        /// Test an unknown parser type
        /// </summary>
        [TestMethod]
        public void TestUnknownFileExtension()
        {
            string filepath = @"Test.ukn";

            ISequenceParser foundParser = SequenceParsers.FindParserByFileName(filepath);
            Assert.IsNull(foundParser);
        }

        /// <summary>
        /// Ensure parser throws exception when file is missing.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestMissingFile()
        {
            string filepath = @"TestUtils\NoFileHere.fa";
            ISequenceParser foundParser = SequenceParsers.FindParserByFileName(filepath);
            Assert.Fail("Should not get here!");
        }

        /// <summary>
        /// Ensure parser throws exception when part of path is missing
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void TestMissingDirectory()
        {
            string filepath = @"NoDirectoryHere\NoFileHere.fa";
            ISequenceParser foundParser = SequenceParsers.FindParserByFileName(filepath);
            Assert.Fail("Should not get here!");
        }

        /// <summary>
        /// Tests the .txt extension
        /// </summary>
        [TestMethod]
        public void TestTxtFileExtension()
        {
            string filepath = @"TestUtils\BLOSUM50.txt";

            ISequenceParser foundParser = SequenceParsers.FindParserByFileName(filepath);
            Assert.IsNull(foundParser);
            // Should not auto-locate FieldTextParser.
        }

        /// <summary>
        /// Tests the .sff extension from Bio.IO.dll
        /// </summary>
        [TestMethod]
        public void TestSffFileExtension()
        {
            string filepath = @"TestUtils\dummy.sff";

            ISequenceParser foundParser = SequenceParsers.FindParserByFileName(filepath);
            Assert.IsNotNull(foundParser);
            Assert.IsInstanceOfType(foundParser, typeof(SFFParser));
        }
    }
}
