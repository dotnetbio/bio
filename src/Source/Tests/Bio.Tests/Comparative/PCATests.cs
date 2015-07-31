using System.Collections.Generic;
using System.Linq;
using Bio;
using Bio.Algorithms.Assembly.Comparative;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests
{
    /// <summary>
    /// Pca unit test cases.
    /// </summary>
    [TestClass]
    public class PcaTests
    {
        #region Step - 2 : Repeat Resolution
        /// <summary>
        /// With Two Reads.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestPcaStep2WithTwoReads()
        {
            ComparativeGenomeAssembler asm = new ComparativeGenomeAssembler() { LengthOfMum = 9, KmerLength = 9 };

            Sequence q = new Sequence(DnaAlphabet.Instance, "AACCTTGGCC");
            q.ID = ">read.F:TestPcaStep2WithTwoReads";

            Sequence p = new Sequence(DnaAlphabet.Instance, "GGGGGGGGGG");
            p.ID = ">read.R:TestPcaStep2WithTwoReads";

            CloneLibrary.Instance.AddLibrary("TestPcaStep2WithTwoReads", (float)61, (float)1);

            TestPcaAssemble(asm,
                new List<Sequence>{
                    new Sequence(DnaAlphabet.Instance, "AACCTTGGCCCCCACGATCGCGCTAGATCGCATCGATCCCCAACCTTGGCCGGGGGGGGGG", false)
                },
                new List<ISequence> { q,p },
                new List<string> { 
                    "AACCTTGGCC",
                    "GGGGGGGGGG"
                });
        }
        #endregion

        #region Step - 3 Layout Refinement
        /// <summary>
        /// Deletion In Reference Two.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestPcaStep3DeletionInReferenceTwo()
        {
            Sequence r = new Sequence(DnaAlphabet.Instance, "CTACGATCGGGG");
            //                                               CTACGTGC         //   TGCGCA is deleted from reference
            //                                                  GCATCG
            //                                                 AGCATC
            //                                                       GGGG
            //                                                   CATCG
            Sequence q = new Sequence(DnaAlphabet.Instance, "CTACGTGC");
            Sequence q2 = new Sequence(DnaAlphabet.Instance, "GCATCG");
            Sequence q3 = new Sequence(DnaAlphabet.Instance, "GGGG");
            Sequence q4 = new Sequence(DnaAlphabet.Instance, "CATCG");

            ComparativeGenomeAssembler asm = new ComparativeGenomeAssembler() { LengthOfMum = 3 };
            var output = asm.Assemble(new List<ISequence> { r }, new List<ISequence> { q, q2, q3, q4 });
            string res = new string(output.ElementAt(0).Select(a => (char)a).ToArray());

            Assert.AreEqual("CTACGTGCATCGGGG", res);
        }

        /// <summary>
        /// Deletion In Reference.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestPcaStep3DeletionInReference()
        {
            Sequence r = new Sequence(DnaAlphabet.Instance, "CTACGATCGGGG");
            //                                               CTACGTGC         //   TGCGCA is deleted from reference
            //                                                    GCATCG
            //                                                       GGGG
            Sequence q = new Sequence(DnaAlphabet.Instance, "CTACGTGC");
            Sequence q2 = new Sequence(DnaAlphabet.Instance, "GCATCG");
            Sequence q3 = new Sequence(DnaAlphabet.Instance, "GGGG");

            ComparativeGenomeAssembler asm = new ComparativeGenomeAssembler() { LengthOfMum = 3 };
            var output = asm.Assemble(new List<ISequence> { r }, new List<ISequence> { q, q2, q3 });
            string res = new string(output.ElementAt(0).Select(a => (char)a).ToArray());

            Assert.AreEqual("CTACGTGCATCGGGG", res);
        }

        /// <summary>
        /// Insertion In Reference Three.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestPcaStep3InsertionInReferenceThree()
        {
            Sequence refSeq = new Sequence(DnaAlphabet.Instance, "AAAACCCGGGGTTTTTTACGTGACTGCA");
            Sequence q = new Sequence(DnaAlphabet.Instance, "AAAAGGGG");
            Sequence r = new Sequence(DnaAlphabet.Instance, "ACGTTGCA");

            ComparativeGenomeAssembler asm = new ComparativeGenomeAssembler() { LengthOfMum = 4 };
            var output = asm.Assemble(new List<ISequence> { refSeq }, new List<ISequence> { q, r });

            string res = new string(output.ElementAt(0).Select(a => (char)a).ToArray());
            Assert.AreEqual("AAAAGGGG", res);

            res = new string(output.ElementAt(1).Select(a => (char)a).ToArray());
            Assert.AreEqual("ACGTTGCA", res);
        }

        /// <summary>
        /// Insertion In Reference Two.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestPcaStep3InsertionInReferenceTwo()
        {
            Sequence refSeq = new Sequence(DnaAlphabet.Instance, "AACCTTGGCCTAGTACGGATATTGCCCACGATCG");
            //                                                       CTTGGCCTAGTA       TGCCCACGATC
            //                                                    AACCTTGGCCTA            CCCACGATCG
            Sequence q = new Sequence(DnaAlphabet.Instance, "AACCTTGGCCTACCCACGATCG");
            Sequence r = new Sequence(DnaAlphabet.Instance, "CTTGGCCTAGTATGCCCACGATCG");

            ComparativeGenomeAssembler asm = new ComparativeGenomeAssembler() { LengthOfMum = 9 };
            var output = asm.Assemble(new List<ISequence> { refSeq }, new List<ISequence> { q, r });
            string res = new string(output.ElementAt(0).Select(a => (char)a).ToArray());

            Assert.AreEqual("AACCTTGGCCTAGTATGCCCACGATCG", res);
        }

        /// <summary>
        /// Insertion In Reference.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestPcaStep3InsertionInReference()
        {
            Sequence r = new Sequence(DnaAlphabet.Instance, "AACCTTGGCCTAGTACGGATATTGCCCACGATCG");

            //                                               AACCTTGGCCTA            CCCACGATCG
            Sequence q = new Sequence(DnaAlphabet.Instance, "AACCTTGGCCTACCCACGATCG");


            ComparativeGenomeAssembler asm = new ComparativeGenomeAssembler() { LengthOfMum = 9 };
            var output = asm.Assemble(new List<ISequence> { r }, new List<ISequence> { q });
            string res = new string(output.ElementAt(0).Select(a => (char)a).ToArray());

            Assert.AreEqual("AACCTTGGCCTACCCACGATCG", res);
        }

        /// <summary>
        /// Repeat Test.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestPcaStep3RepeatTest()
        {
            Sequence r = new Sequence(DnaAlphabet.Instance, "AACCTTGGCCCCCACGATCGCGCTAGATCGCATCGATCCCCAACCTTGGCCGGGGGGGGGG");

            Sequence q = new Sequence(DnaAlphabet.Instance, "AACCTTGGCC");
            q.ID = ">read.F:abc";
            Sequence p = new Sequence(DnaAlphabet.Instance, "GGGGGGGGGG");
            p.ID = ">read.R:abc";
            CloneLibrary.Instance.AddLibrary("abc", (float)61, (float)1);


            ComparativeGenomeAssembler asm = new ComparativeGenomeAssembler() { LengthOfMum = 9 };
            var res = asm.Assemble(new List<ISequence> { r }, new List<ISequence> { q, p });
            string[] expectedResult = new string[2];
            expectedResult[0] = "AACCTTGGCC";
            expectedResult[1] = "GGGGGGGGGG";
            int i = 0;
            foreach (var s in res)
            {
                string actual = new string(s.Select(a => (char)a).ToArray());
                Assert.AreEqual(expectedResult[i], actual);
                i++;
            }
        }

        #endregion Step - 3 Layout Refinement
        #region Step - 4 : Consensus Generation
        /// <summary>
        /// With Adjascent Reads.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestPcaStep4WithAdjascentReads()
        {
            ComparativeGenomeAssembler asm = new ComparativeGenomeAssembler() { LengthOfMum = 3, KmerLength = 3 };

            TestPcaAssemble(asm,
                new List<Sequence>{
                    new Sequence(DnaAlphabet.Instance, "AGAAAAGTTTTCA", false)
                },
                new List<ISequence> { 
                    new Sequence(DnaAlphabet.Instance, "TTTT", false),
                    new Sequence(DnaAlphabet.Instance, "AAAAG", false)
                },
                new List<string>
                {
                    "AAAAGTTTT"
                });
        }

        /// <summary>
        /// With Overlapping Reads.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestPcaStep4WithOverlappingReads()
        {
            ComparativeGenomeAssembler asm = new ComparativeGenomeAssembler() { LengthOfMum = 3 };

            TestPcaAssemble(asm,
                new List<Sequence>{
                    new Sequence(DnaAlphabet.Instance, "AGAAAAGTTTTCA", false)
                },
                new List<ISequence> { 
                    new Sequence(DnaAlphabet.Instance, "AGAAAA", false) ,
                    new Sequence(DnaAlphabet.Instance, "AAAAGTTTT", false)
                },
                new List<string>
                {
                    "AGAAAAGTTTT"
                });
        }
        #endregion

        /// <summary>
        /// Test Pca assemble.
        /// </summary>
        /// <param name="asm">Comparative Genome Assembler.</param>
        /// <param name="reference">Reference sequence.</param>
        /// <param name="query">Query sequence.</param>
        /// <param name="expected">Expected strings.</param>
        private static void TestPcaAssemble(ComparativeGenomeAssembler asm, IEnumerable<ISequence> reference, IEnumerable<ISequence> query, IList<string> expected)
        {
            IEnumerable<ISequence> result = asm.Assemble(reference, query);

            Assert.IsTrue(result.Count() == expected.Count);

            foreach (var act in result)
            {
                string actualStr = new string(act.Select(a => (char)a).ToArray());
                Assert.IsTrue(expected.Contains(actualStr));
            }
        }
    }
}

