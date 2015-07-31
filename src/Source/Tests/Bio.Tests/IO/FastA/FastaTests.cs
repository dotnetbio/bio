using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Bio;
using Bio.IO.FastA;
using NUnit.Framework;

namespace Bio.Tests.IO.FastA
{
    /// <summary>
    /// FASTA format parser and formatter.
    /// </summary>
    [TestFixture]
    public class FastaTests
    {
        /// <summary>
        /// Verifies that the parser doesn't throw an exception when calling ParseOne on a file
        /// containing more than one sequence.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestFastaWhenParsingOneOfMany()
        {
            // parse

            string filepath = System.IO.Path.Combine("TestUtils","Fasta","5_sequences.fasta");
            FastAParser parser = new FastAParser { Alphabet = Alphabets.Protein };
            using (parser.Open(filepath))
            {
                int[] sequenceCountArray = { 27, 29, 30, 35, 32 };

                int i = 0;
                foreach (ISequence seq in parser.Parse())
                {
                    Assert.IsNotNull(seq);
                    Assert.AreEqual(seq.Count, sequenceCountArray[i]);
                    i++;
                }
            }
        }

        /// <summary>
        /// Parse sample FASTA file 186972391.fasta and verify that it is read correctly.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestFastaFor186972391()
        {
            string expectedSequence =

                "IFYEPVEILGYDNKSSLVLVKRLITRMYQQKSLISSLNDSNQNEFWGHKNSFSSHFSSQMVSEGFGVILE" +
                "IPFSSRLVSSLEEKRIPKSQNLRSIHSIFPFLEDKLSHLNYVSDLLIPHPIHLEILVQILQCWIKDVPSL" +
                "HLLRLFFHEYHNLNSLITLNKSIYVFSKRKKRFFGFLHNSYVYECEYLFLFIRKKSSYLRSISSGVFLER" +
                "THFYGKIKYLLVVCCNSFQRILWFLKDTFIHYVRYQGKAIMASKGTLILMKKWKFHLVNFWQSYFHFWFQ" +
                "PYRINIKQLPNYSFSFLGYFSSVRKNPLVVRNQMLENSFLINTLTQKLDTIVPAISLIGSLSKAQFCTVL" +
                "GHPISKPIWTDLSDSDILDRFCRICRNLCRYHSGSSKKQVLYRIKYIFRLSCARTLARKHKSTVRTFMRR" +
                "LGSGFLEEFFLEEE";

            // parse
            string filepath = System.IO.Path.Combine("TestUtils","Fasta", "186972391.fasta");


            Assert.IsTrue(File.Exists(filepath));

            FastAParser parser = new FastAParser { Alphabet = Alphabets.Protein };
            foreach (ISequence seq in parser.Parse(filepath))
            {
                Assert.IsNotNull(seq);
                Assert.AreEqual(434, seq.Count);

                string actual = seq.Aggregate("", (current, b) => current + (char)b);

                Assert.AreEqual(expectedSequence, actual);
                Assert.AreEqual(seq.Alphabet.Name, "Protein");

                Assert.AreEqual("gi|186972391|gb|ACC99454.1| maturase K [Scaphosepalum rapax]", seq.ID);
            }
        }

        /// <summary>
        /// Parse sample FASTA file 186972391.fasta and verify that it is read correctly.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestLargeFasta()
        {
            if (Environment.Is64BitProcess)
            {
                int sequenceCount = 300 * 1024 * 1024; // 300 MB of data
                string filePath = CreateData(sequenceCount);

                Assert.IsTrue(File.Exists(filePath));

                try
                {
                    var parser = new FastAParser { Alphabet = Alphabets.Protein };
                    int count = 0;
                    foreach (ISequence seq in parser.Parse(filePath))
                    {
                        Assert.IsNotNull(seq);
                        Assert.AreEqual(sequenceCount, seq.Count);
                        Assert.AreEqual(seq.Alphabet.Name, "Protein");
                        count++;
                    }
                    Assert.AreEqual(1, count);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }
        }

        /// <summary>
        /// Verifies that the parser throws an exception when Parsing a sequence which contains valid id but no sequence data
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestFastaWhenParsingSequenceWithEmptyData()
        {
            // parse
            const string relativepath = @"\TestUtils\Fasta\EmptySequenceWithID.fasta";
            string assemblypath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);
            string filepath = assemblypath + relativepath;
            FastAParser parser = new FastAParser();

            try
            {
                parser.Parse(filepath).First();
                Assert.Fail();
            }
            catch (Exception)
            {
                // OK
            }
        }


        /// <summary>
        /// Create a seq of ACGT
        /// </summary>
        /// <param name="filename">name of temporary file</param>
        /// <param name="seqCount">Count of sequences</param>
        /// <param name="seqLength">Length of sequence</param>
        private static void CreateSeq(string filename, long seqCount, long seqLength)
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            char[] alphs = new char[] { 'A', 'C', 'G', 'T' };
            using (StreamWriter writer = new StreamWriter(filename))
            {
                for (long i = 0; i < seqCount; i++)
                {
                    if (i != 0)
                    {
                        writer.Write(Environment.NewLine);
                    }
                    writer.WriteLine(">" + i.ToString(CultureInfo.InvariantCulture));

                    for (long j = 0; j < seqLength; j++)
                    {
                        if (j > 0 && j % 80 == 0)
                        {
                            writer.WriteLine();
                        }

                        writer.Write(alphs[rnd.Next(0, alphs.Length)]);
                    }
                }

                writer.Flush();
            }

        }

        /// <summary>
        /// Create a fasta file 
        /// </summary>
        /// <param name="count">Sequence length</param>
        /// <returns>return the name of file</returns>
        private static string CreateData(int count)
        {
            string FileName = Path.GetTempFileName() + ".fasta";

            CreateSeq(FileName, 1, count);

            return FileName;
        }
    }
}

