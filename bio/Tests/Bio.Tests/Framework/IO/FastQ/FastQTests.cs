using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bio;
using Bio.IO;
using Bio.IO.FastQ;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Tests.Properties;

namespace Bio.Tests.IO.FastQ
{
    /// <summary>
    /// FASTQ format parser and formatter.
    /// </summary>
    [TestClass]
    public class FastQTests
    {

        /// <summary>
        /// Verifies that the parser doesn't throw an exception when Parsing first sequene on a file
        /// containing more than one sequence.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestFastQWhenParsingOneOfMany()
        {
            string filepath = @"TestUtils\FASTQ\SRR002012_5.fastq";
            // parse
            ISequenceParser parser = new FastQParser(filepath);
            try
            {
                ISequence seq = parser.Parse().First();
                Assert.IsNotNull(seq);
            }
            finally
            {
                parser.Dispose();
            }

            using (FastQParser fqParser = new FastQParser(filepath))
            {
                fqParser.AutoDetectFastQFormat = false;
                fqParser.FormatType = FastQFormatType.Sanger;
                fqParser.Alphabet = Alphabets.DNA;
                QualitativeSequence qualSeq = fqParser.Parse().First();
                Assert.IsNotNull(qualSeq);
            }
        }

        /// <summary>
        /// Test formatter - by reading the multisequence FASTQ file SRR002012_5.fastq,
        /// writing it back to disk using the formatter, then reading the new file
        /// and confirming that the data has been written correctly.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void FastQFormatter()
        {
            string filepathOriginal = @"TestUtils\FASTQ\SRR002012_5.fastq";
            Assert.IsTrue(File.Exists(filepathOriginal));
            
            IList<QualitativeSequence> seqsOriginal = null;
            string filepathTmp = Path.GetTempFileName();

            using (FastQParser parser = new FastQParser())
            {
                parser.Open(filepathOriginal);


                // Read the original file
                seqsOriginal = parser.Parse().ToList();
                Assert.IsNotNull(seqsOriginal);

                // Use the formatter to write the original sequences to a temp file
                using (FastQFormatter formatter = new FastQFormatter(filepathTmp))
                {
                    foreach (QualitativeSequence s in seqsOriginal)
                    {
                        formatter.Write(s);
                    }
                }
            }

            // Read the new file, then compare the sequences
            IList<QualitativeSequence> seqsNew = null;
            using (FastQParser parser = new FastQParser(filepathTmp))
            {
                seqsNew = parser.Parse().ToList();
                Assert.IsNotNull(seqsNew);

                // Now compare the sequences.
                int countOriginal = seqsOriginal.Count();
                int countNew = seqsNew.Count();
                Assert.AreEqual(countOriginal, countNew);

                int i;
                for (i = 0; i < countOriginal; i++)
                {
                    Assert.AreEqual(seqsOriginal[i].ID, seqsNew[i].ID);
                    string orgSeq = ASCIIEncoding.ASCII.GetString(seqsOriginal[i].ToArray());
                    string newSeq = ASCIIEncoding.ASCII.GetString(seqsNew[i].ToArray());
                    string orgscores = ASCIIEncoding.ASCII.GetString(seqsOriginal[i].QualityScores.ToArray());
                    string newscores = ASCIIEncoding.ASCII.GetString(seqsNew[i].QualityScores.ToArray());
                    Assert.AreEqual(orgSeq, newSeq);
                    Assert.AreEqual(orgscores, newscores);
                }

                // Passed all the tests, delete the tmp file. If we failed an Assert,
                // the tmp file will still be there in case we need it for debugging.
                File.Delete(filepathTmp);
            }
        }

        /// <summary>
        /// Test formatter - by reading the multisequence FASTQ file SRR002012_5.fastq,
        /// writing it back to disk using the ISequenceFormatter interface, then reading the new file
        /// and confirming that the data has been written correctly.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void FastQFormatterUsingInterface()
        {
            string filepathOriginal = @"TestUtils\FASTQ\SRR002012_5.fastq";
            Assert.IsTrue(File.Exists(filepathOriginal));

            IList<QualitativeSequence> seqsOriginal = null;
            string filepathTmp = Path.GetTempFileName();

            using (FastQParser parser = new FastQParser())
            {
                parser.Open(filepathOriginal);

                // Read the original file
                seqsOriginal = parser.Parse().ToList();
                Assert.IsNotNull(seqsOriginal);

                // Use the formatter to write the original sequences to a temp file
                ISequenceFormatter formatter = null;
                try
                {
                    formatter = new FastQFormatter();
                    formatter.Open(filepathTmp);
                    foreach (ISequence s in seqsOriginal)
                    {
                        formatter.Write(s);
                    }

                    formatter.Close();
                }
                finally
                {
                    if (formatter != null)
                    {
                        ((FastQFormatter)formatter).Dispose();
                    }
                }
            }

            // Read the new file, then compare the sequences
            IList<QualitativeSequence> seqsNew = null;
            using (FastQParser parser = new FastQParser(filepathTmp))
            {
                seqsNew = parser.Parse().ToList();
                Assert.IsNotNull(seqsNew);

                // Now compare the sequences.
                int countOriginal = seqsOriginal.Count();
                int countNew = seqsNew.Count();
                Assert.AreEqual(countOriginal, countNew);

                int i;
                for (i = 0; i < countOriginal; i++)
                {
                    Assert.AreEqual(seqsOriginal[i].ID, seqsNew[i].ID);
                    string orgSeq = ASCIIEncoding.ASCII.GetString(seqsOriginal[i].ToArray());
                    string newSeq = ASCIIEncoding.ASCII.GetString(seqsNew[i].ToArray());
                    string orgscores = ASCIIEncoding.ASCII.GetString(seqsOriginal[i].QualityScores.ToArray());
                    string newscores = ASCIIEncoding.ASCII.GetString(seqsNew[i].QualityScores.ToArray());
                    Assert.AreEqual(orgSeq, newSeq);
                    Assert.AreEqual(orgscores, newscores);
                }

                // Passed all the tests, delete the tmp file. If we failed an Assert,
                // the tmp file will still be there in case we need it for debugging.
                File.Delete(filepathTmp);
            }
        }

        /// <summary>
        /// Verify that the parser can read many files without exceptions.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void FastQParserForManyFiles()
        {
            string path = @"TestUtils\FASTQ";
            Assert.IsTrue(Directory.Exists(path));
            int count = 0;
           
            DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo fi in di.GetFiles("*.fastq"))
            {
                using ( FastQParser parser = new FastQParser(fi.FullName))
                {
                    foreach (QualitativeSequence seq in parser.Parse())
                    {
                        Assert.IsNotNull(seq);
                        count++;
                    }
                }
            }

            Assert.IsTrue(count >= 3);
        }

        /// <summary>
        /// Tests the name,description and file extension property of 
        /// Fasta formatter and parser.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void FastQProperties()
        {
            using (FastQParser parser = new FastQParser())
            {
                Assert.AreEqual(parser.Name, Resource.FastQName);
                Assert.AreEqual(parser.Description, Resource.FASTQPARSER_DESCRIPTION);
                Assert.AreEqual(parser.SupportedFileTypes, Resource.FASTQ_FILEEXTENSION);
            }

            using(FastQFormatter formatter = new FastQFormatter())
            {
                Assert.AreEqual(formatter.Name, Resource.FastQName);
                Assert.AreEqual(formatter.Description, Resource.FASTQFORMATTER_DESCRIPTION);
                Assert.AreEqual(formatter.SupportedFileTypes, Resource.FASTQ_FILEEXTENSION);
            }
        }

        /// <summary>
        /// Tests the default FastQ format type.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestDefaultFastQFormatType()
        {
            using (FastQParser parser = new FastQParser())
            {
                Assert.AreEqual(parser.FormatType, FastQFormatType.Illumina);
            }
        }
    }
}
