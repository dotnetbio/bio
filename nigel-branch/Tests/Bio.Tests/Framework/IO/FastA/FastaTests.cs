using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Bio;
using Bio.IO.FastA;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests.IO.FastA
{
    /// <summary>
    /// FASTA format parser and formatter.
    /// </summary>
    [TestClass]
    public class FastaTests
    {
        /// <summary>
        /// Initialize static member of the class. Static constructor to open log and make other settings needed for test
        /// </summary>
        static FastaTests()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("Bio.Tests.log");
            }
        }

        /// <summary>
        /// Verifies that the parser doesn't throw an exception when calling ParseOne on a file
        /// containing more than one sequence.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestFastaWhenParsingOneOfMany()
        {
            // parse
            string relativepath = @"\TestUtils\Fasta\5_sequences.fasta";
            string assemblypath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);
            string filepath = assemblypath + relativepath;
            using (FastAParser parser = new FastAParser(filepath))
            {
                parser.Alphabet = (IAlphabet)Alphabets.Protein;

                int[] sequenceCountArray = new int[5];
                sequenceCountArray[0] = 27;
                sequenceCountArray[1] = 29;
                sequenceCountArray[2] = 30;
                sequenceCountArray[3] = 35;
                sequenceCountArray[4] = 32;

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
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
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
            string relativepath = @"\TestUtils\Fasta\186972391.fasta";
            string assemblypath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);
            string filepath = assemblypath + relativepath;


            Assert.IsTrue(File.Exists(filepath));

            FastAParser parser = null;
            try
            {
                parser = new FastAParser(filepath);
                parser.Alphabet = (IAlphabet)Alphabets.Protein;

                foreach (ISequence seq in parser.Parse())
                {
                    Assert.IsNotNull(seq);
                    Assert.AreEqual(434, seq.Count);

                    string actual = "";
                    foreach (byte b in seq)
                    {
                        actual += (char)b;
                    }

                    Assert.AreEqual(expectedSequence, actual);
                    Assert.AreEqual(seq.Alphabet.Name, "Protein");

                    Assert.AreEqual("gi|186972391|gb|ACC99454.1| maturase K [Scaphosepalum rapax]", seq.ID);
                }
            }
            finally
            {
                if (parser != null)
                {
                    parser.Dispose();
                }
            }
        }

        /// <summary>
        /// Parse sample FASTA file 186972391.fasta and verify that it is read correctly.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        [Ignore()]
        public void TestFastaForMemoryMapFiles()
        {
            int sequenceCount = 300 * 1024 * 1024; // 300 MB of data
            string filePath = CreateData(sequenceCount);

            Assert.IsTrue(File.Exists(filePath));

            FastAParser parser = null;
            try
            {
                parser = new FastAParser(filePath);
                parser.Alphabet = (IAlphabet)Alphabets.Protein;

                foreach (ISequence seq in parser.Parse())
                {
                    Assert.IsNotNull(seq);
                    Assert.AreEqual(sequenceCount, seq.Count);
                    Assert.AreEqual(seq.Alphabet.Name, "Protein");
                }
            }
            finally
            {
                File.Delete(filePath);
                parser.Dispose();
            }
        }

        /// <summary>
        /// Verifies that the parser throws an exception when Parsing a sequence which contains valid id but no sequence data
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestFastaWhenParsingSequenceWithEmptyData()
        {
            // parse
            string relativepath = @"\TestUtils\Fasta\EmptySequenceWithID.fasta";
            string assemblypath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);
            string filepath = assemblypath + relativepath;
            FastAParser parser = new FastAParser(filepath);

            try
            {
                parser.Parse().First();
                Assert.Fail();
            }
            catch
            {
            }
            finally
            {
                parser.Dispose();
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

