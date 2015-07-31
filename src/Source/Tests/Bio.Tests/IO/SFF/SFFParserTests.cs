using System;
using System.IO;
using System.Linq;

using Bio.Extensions;
using Bio.IO.SFF;
using NUnit.Framework;

namespace Bio.Tests.IO.SFF
{
    /// <summary>
    /// This tests the SFF parser (454)
    /// </summary>
    [TestFixture]
    public class SFFParserTests
    {
        private const string filePath = @"TestUtils\SFF\E3MFGYR02_random_10_reads.sff";

        /// <summary>
        /// Verifies that the parser doesn't throw an exception when calling Parse on a file
        /// containing more than one sequence.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestSffWhenParsingOneOfMany()
        {
            SFFParser parser = new SFFParser();
            using (parser.Open(filePath))
            {
                var sequence = parser.Parse().FirstOrDefault();
                Assert.IsNotNull(sequence);
                Assert.AreEqual(265, sequence.Count);
            }
        }

        /// <summary>
        /// Parse sample .SFF file and verify we read all the sequences.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestSffForOneSequence()
        {
            const string ExpectedSequence = "TCAGGGTCTACATGTTGGTTAACCCGTACTGATTTGAATTGGCTCTTTGTCTTTCCAAAGGGAATTCATCTTCTTATGGC" +
                                            "ACACATAAAGGATAAATACAAGAATCTTCCTATTTACATCACTGAAAATGGCATGGCTGAATCAAGGAATGACTCAATAC" +
                                            "CAGTCAATGAAGCCCGCAAGGATAGTATAAGGATTAGATACCATGATGGCCATCTTAAATTCCTTCTTCAAGCGATCAAG" +
                                            "GAAGGTGTTAATTTGAAGGGGCTTA";

            SFFParser parser = new SFFParser();
            using (parser.Open(filePath))
            {
                var sequence = parser.Parse().FirstOrDefault();
                Assert.IsNotNull(sequence);
                Assert.IsInstanceOf<QualitativeSequence>(sequence);
                Assert.AreEqual(265, sequence.Count);

                var actualSequence = sequence.ConvertToString();
                Assert.AreEqual(ExpectedSequence, actualSequence);
                Assert.AreEqual(sequence.Alphabet, Alphabets.DNA);
                Assert.AreEqual("E3MFGYR02JWQ7T", sequence.ID);
            }
        }

        /// <summary>
        /// Verifies that the parser throws an exception when Parsing a sequence which contains valid id but no sequence data
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestMultipleSequencesInFile()
        {
            var expectedData = new[]
            {
                Tuple.Create("E3MFGYR02JWQ7T", 265),
                Tuple.Create("E3MFGYR02JA6IL", 271),
                Tuple.Create("E3MFGYR02JHD4H", 310),
                Tuple.Create("E3MFGYR02GFKUC", 299),
                Tuple.Create("E3MFGYR02FTGED", 281),
                Tuple.Create("E3MFGYR02FR9G7", 261),
                Tuple.Create("E3MFGYR02GAZMS", 278),
                Tuple.Create("E3MFGYR02HHZ8O", 221),
                Tuple.Create("E3MFGYR02GPGB1", 269),
                Tuple.Create("E3MFGYR02F7Z7G", 219),
            };

            SFFParser parser = new SFFParser();
            using (parser.Open(filePath))
            {
                int index = 0;
                foreach (var sequence in parser.Parse())
                {
                    Assert.IsTrue(expectedData.Length > index);
                    Assert.AreEqual(expectedData[index].Item1, sequence.ID);
                    Assert.AreEqual(expectedData[index].Item2, sequence.Count);
                    index++;
                }
            }
        }
    }
}
