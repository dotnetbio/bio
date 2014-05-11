using System.Collections.Generic;
using System.Linq;
using Bio.Algorithms.Alignment;
using Bio.SimilarityMatrices;
using Bio.Tests.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests.Algorithms.Alignment
{
    /// <summary>
    /// Tests for the Aligner classes.
    /// </summary>
    [TestClass]
    public class AlignerTests
    {
        #region Smith-Waterman Aligner
        /// <summary>
        /// Test SmithWatermanAligner using Protein Sequence and Simple Gap Penalty Function.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void SmithWatermanProteinSeqSimpleGap()
        {
            IPairwiseSequenceAligner sw = new SmithWatermanAligner
            {
                SimilarityMatrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.Blosum62), 
                GapOpenCost = -8
            };

            ISequence sequence1 = new Sequence(Alphabets.Protein, "HEAGAWGHEE");
            ISequence sequence2 = new Sequence(Alphabets.Protein, "PAWHEAE");
            var result = sw.AlignSimple(sequence1, sequence2);
            AlignmentHelpers.LogResult(sw, result);

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            PairwiseAlignedSequence alignedSeq = new PairwiseAlignedSequence
                {
                    FirstSequence = new Sequence(Alphabets.Protein, "AWGHE"),
                    SecondSequence = new Sequence(Alphabets.Protein, "AW-HE"),
                    Consensus = new Sequence(Alphabets.AmbiguousProtein, "AWGHE"),
                    Score = 20,
                    FirstOffset = 0,
                    SecondOffset = 3
                };
            align.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(align);
            Assert.IsTrue(AlignmentHelpers.CompareAlignment(result, expectedOutput));
        }

        /// <summary>
        /// Test SmithWatermanAligner using Protein sequence and Affine Gap Penalty Function.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void SmithWatermanProteinSeqAffineGap()
        {
            IPairwiseSequenceAligner sw = new SmithWatermanAligner
                {
                    SimilarityMatrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.Blosum62),
                    GapOpenCost = -8,
                    GapExtensionCost = -1,
                };

            ISequence sequence1 = new Sequence(Alphabets.Protein, "HEAGAWGHEE");
            ISequence sequence2 = new Sequence(Alphabets.Protein, "PAWHEAE");
            IList<IPairwiseSequenceAlignment> result = sw.Align(sequence1, sequence2);
            AlignmentHelpers.LogResult(sw, result);

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            PairwiseAlignedSequence alignedSeq = new PairwiseAlignedSequence
                {
                    FirstSequence = new Sequence(Alphabets.Protein, "AWGHE"),
                    SecondSequence = new Sequence(Alphabets.Protein, "AW-HE"),
                    Consensus = new Sequence(Alphabets.AmbiguousProtein, "AWGHE"),
                    Score = 20,
                    FirstOffset = 0,
                    SecondOffset = 3
                };
            align.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(align);

            Assert.IsTrue(AlignmentHelpers.CompareAlignment(result, expectedOutput));
        }

        /// <summary>
        /// Test SmithWatermanAligner for cases where it returns multiple alignments
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void SmithWatermanAlignerMultipleAlignments1()
        {
            IPairwiseSequenceAligner sw = new SmithWatermanAligner
            {
                SimilarityMatrix = new DiagonalSimilarityMatrix(5, -20), 
                GapOpenCost = -5
            };

            ISequence sequence1 = new Sequence(Alphabets.DNA, "AAATTCCCAG");
            ISequence sequence2 = new Sequence(Alphabets.DNA, "AAAGCCC");
            IList<IPairwiseSequenceAlignment> result = sw.AlignSimple(sequence1, sequence2);
            AlignmentHelpers.LogResult(sw, result);

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment(sequence1, sequence2);

            // First alignment
            align.PairwiseAlignedSequences.Add(new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(Alphabets.DNA, "AAA"),
                SecondSequence = new Sequence(Alphabets.DNA, "AAA"),
                Consensus = new Sequence(Alphabets.DNA, "AAA"),
                Score = 15,
                FirstOffset = 0,
                SecondOffset = 0
            });

            // Second alignment
            align.PairwiseAlignedSequences.Add(new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(Alphabets.DNA, "CCC"),
                SecondSequence = new Sequence(Alphabets.DNA, "CCC"),
                Consensus = new Sequence(Alphabets.DNA, "CCC"),
                Score = 15,
                FirstOffset = 0,
                SecondOffset = 1
            });

            expectedOutput.Add(align);
            Assert.IsTrue(AlignmentHelpers.CompareAlignment(result, expectedOutput));
        }

        /// <summary>
        /// Test SmithWatermanAligner for cases where it returns multiple alignments
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void SmithWatermanAlignerMultipleAlignments2()
        {
            IPairwiseSequenceAligner sw = new SmithWatermanAligner
            {
                SimilarityMatrix = new DiagonalSimilarityMatrix(5, -4),
                GapOpenCost = -5
            };

            ISequence sequence1 = new Sequence(Alphabets.DNA, "AAAAGGGGGGCCCC");
            ISequence sequence2 = new Sequence(Alphabets.DNA, "AAAATTTTTTTCCCC");
            IList<IPairwiseSequenceAlignment> result = sw.AlignSimple(sequence1, sequence2);
            AlignmentHelpers.LogResult(sw, result);

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();

            // First alignment
            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment(sequence1, sequence2);

            align.PairwiseAlignedSequences.Add(new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(Alphabets.DNA, "AAAA"),
                SecondSequence = new Sequence(Alphabets.DNA, "AAAA"),
                Consensus = new Sequence(Alphabets.DNA, "AAAA"),
                Score = 20,
                FirstOffset = 0,
                SecondOffset = 0
            });

            // Second alignment
            align.PairwiseAlignedSequences.Add(new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(Alphabets.DNA, "CCCC"),
                SecondSequence = new Sequence(Alphabets.DNA, "CCCC"),
                Consensus = new Sequence(Alphabets.DNA, "CCCC"),
                Score = 20,
                FirstOffset = 1,
                SecondOffset = 0
            });

            expectedOutput.Add(align);
            Assert.IsTrue(AlignmentHelpers.CompareAlignment(result, expectedOutput));
        }
        #endregion Smith-Waterman Aligner

        #region Needleman-Wunsch Aligner

        /// <summary>
        /// Test NeedlemanWunschAligner using Protein sequence and Simple Gap Penalty Function.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NeedlemanWunschProteinSeqSimpleGap()
        {
            IPairwiseSequenceAligner nw = new NeedlemanWunschAligner
            {
                SimilarityMatrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.Blosum62),
                GapOpenCost = -8
            };

            ISequence sequence1 = new Sequence(Alphabets.Protein, "HEAGAWGHEE");
            ISequence sequence2 = new Sequence(Alphabets.Protein, "PAWHEAE");
            IList<IPairwiseSequenceAlignment> result = nw.AlignSimple(sequence1, sequence2);
            AlignmentHelpers.LogResult(nw, result);

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            align.PairwiseAlignedSequences.Add(new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(Alphabets.Protein, "HEAGAWGHE-E"),
                SecondSequence = new Sequence(Alphabets.Protein, "-PA--W-HEAE"),
                Consensus = new Sequence(AmbiguousProteinAlphabet.Instance, "HXAGAWGHEAE"),
                Score = -8,
                FirstOffset = 0,
                SecondOffset = 0
            });
            expectedOutput.Add(align);
            
            Assert.IsTrue(AlignmentHelpers.CompareAlignment(result, expectedOutput));
        }

        /// <summary>
        /// Test NeedlemanWunschAligner using Protein sequence and Affine Gap Penalty Function.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NeedlemanWunschProteinSeqAffineGap()
        {
            IPairwiseSequenceAligner nw = new NeedlemanWunschAligner
            {
                SimilarityMatrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.Blosum62),
                GapOpenCost = -8,
                GapExtensionCost = -1
            };

            ISequence sequence1 = new Sequence(Alphabets.Protein, "HEAGAWGHEE");
            ISequence sequence2 = new Sequence(Alphabets.Protein, "PAWHEAE");
            IList<IPairwiseSequenceAlignment> result = nw.Align(sequence1, sequence2);
            AlignmentHelpers.LogResult(nw, result);

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            align.PairwiseAlignedSequences.Add(new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(Alphabets.Protein,  "HEAGAWGHE-E"),
                SecondSequence = new Sequence(Alphabets.Protein, "P---AW-HEAE"),
                Consensus = new Sequence(AmbiguousProteinAlphabet.Instance, "XEAGAWGHEAE"),
                Score = 5,
                FirstOffset = 0,
                SecondOffset = 0
            });
            expectedOutput.Add(align);

            Assert.IsTrue(AlignmentHelpers.CompareAlignment(result, expectedOutput));
        }

        /// <summary>
        /// Test NeedlemanWunschAligner using DNA sequence and Simple Gap Penalty Function.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NeedlemanWunschDnaSeqSimpleGap()
        {
            IPairwiseSequenceAligner nw = new NeedlemanWunschAligner
            {
                SimilarityMatrix = new DiagonalSimilarityMatrix(2, -1),
                GapOpenCost = -2
            };

            ISequence sequence1 = new Sequence(Alphabets.DNA, "GAATTCAGTTA");
            ISequence sequence2 = new Sequence(Alphabets.DNA, "GGATCGA");
            IList<IPairwiseSequenceAlignment> result = nw.AlignSimple(sequence1, sequence2);
            AlignmentHelpers.LogResult(nw, result);

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            align.PairwiseAlignedSequences.Add(new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(Alphabets.DNA, "GAATTCAGTTA"),
                SecondSequence = new Sequence(Alphabets.DNA, "GGAT-C-G--A"),
                Consensus = new Sequence(AmbiguousDnaAlphabet.Instance, "GRATTCAGTTA"),
                Score = 3,
                FirstOffset = 0,
                SecondOffset = 0
            });
            expectedOutput.Add(align);
            
            Assert.IsTrue(AlignmentHelpers.CompareAlignment(result, expectedOutput));
        }

        #endregion Needleman-Wunsch Aligner

        #region NUCmer Test Cases

        /// <summary>
        /// Test NUCmer3 with single valid extendable cluster among multiple
        /// clusters and single reference sequence.
        /// Also covers extend backward start 
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestNUCmer3SingleCluster()
        {
            string reference = "AGAAAAGTTTTCA";
            string search = "TTTTGAGATAAAATC";

            Sequence referenceSeq = null;
            Sequence searchSeq = null;
            List<ISequence> referenceSeqs = null;
            List<ISequence> searchSeqs = null;

            referenceSeq = new Sequence(Alphabets.DNA, reference);
            referenceSeq.ID = "R1";

            searchSeq = new Sequence(Alphabets.DNA, search);
            searchSeq.ID = "Q1";

            referenceSeqs = new List<ISequence>();
            referenceSeqs.Add(referenceSeq);

            searchSeqs = new List<ISequence>();
            searchSeqs.Add(searchSeq);

            NucmerPairwiseAligner nucmer = new NucmerPairwiseAligner();
            nucmer.FixedSeparation = 0;
            nucmer.MinimumScore = 2;
            nucmer.SeparationFactor = -1;
            nucmer.LengthOfMUM = 3;
            nucmer.ForwardOnly = true;
            IList<IPairwiseSequenceAlignment> result = nucmer.Align(referenceSeqs, searchSeqs).Select(a => a as IPairwiseSequenceAlignment).ToList();

            // Check if output is not null
            Assert.AreNotEqual(null, result);

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            PairwiseAlignedSequence alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(Alphabets.DNA, "AG--AAAA");
            alignedSeq.SecondSequence = new Sequence(Alphabets.DNA, "AGATAAAA");
            alignedSeq.Consensus = new Sequence(Alphabets.DNA, "AGATAAAA");
            alignedSeq.Score = -11;
            alignedSeq.FirstOffset = 5;
            alignedSeq.SecondOffset = 0;
            align.PairwiseAlignedSequences.Add(alignedSeq);
            alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(Alphabets.DNA, "TTTT");
            alignedSeq.SecondSequence = new Sequence(Alphabets.DNA, "TTTT");
            alignedSeq.Consensus = new Sequence(Alphabets.DNA, "TTTT");
            alignedSeq.Score = 12;
            alignedSeq.FirstOffset = 0;
            alignedSeq.SecondOffset = 7;
            align.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(align);
            Assert.IsTrue(AlignmentHelpers.CompareAlignment(result, expectedOutput));
        }

        /// <summary>
        /// Test NUCmer3 with multiple valid extendable clusters and single
        /// reference sequences
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestNUCmer3MultipleClusters()
        {
            string reference = "ATGCGCATCCCCTAGCT";
            string search = "CCGCGCCCCCTCAGCT";

            Sequence referenceSeq = null;
            Sequence searchSeq = null;
            List<ISequence> referenceSeqs = null;
            List<ISequence> searchSeqs = null;

            referenceSeq = new Sequence(Alphabets.DNA, reference);
            referenceSeq.ID = "R1";

            searchSeq = new Sequence(Alphabets.DNA, search);
            searchSeq.ID = "Q1";

            referenceSeqs = new List<ISequence>();
            referenceSeqs.Add(referenceSeq);

            searchSeqs = new List<ISequence>();
            searchSeqs.Add(searchSeq);

            NucmerPairwiseAligner nucmer = new NucmerPairwiseAligner();
            nucmer.FixedSeparation = 0;
            nucmer.MinimumScore = 2;
            nucmer.SeparationFactor = -1;
            nucmer.LengthOfMUM = 3;
            nucmer.ForwardOnly = true;
            IList<IPairwiseSequenceAlignment> result = nucmer.Align(referenceSeqs, searchSeqs).Select(a => a as IPairwiseSequenceAlignment).ToList();

            // Check if output is not null
            Assert.AreNotEqual(0, result);

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            PairwiseAlignedSequence alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(Alphabets.DNA, "GCGCATCCCCT-AGCT");
            alignedSeq.SecondSequence = new Sequence(Alphabets.DNA, "GCGC--CCCCTCAGCT");
            alignedSeq.Consensus = new Sequence(Alphabets.DNA, "GCGCATCCCCTCAGCT");
            alignedSeq.Score = -11;
            alignedSeq.FirstOffset = 0;
            alignedSeq.SecondOffset = 0;
            align.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(align);
            Assert.IsTrue(AlignmentHelpers.CompareAlignment(result, expectedOutput));
        }

        /// <summary>
        /// Test NUCmer3 with multiple reference sequences
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestNUCmer3MultipleReferences()
        {
            Sequence referenceSeq = null;
            Sequence searchSeq = null;
            List<ISequence> referenceSeqs = null;
            List<ISequence> searchSeqs = null;

            referenceSeqs = new List<ISequence>();

            string reference = "ATGCGCATCCCC";
            referenceSeq = new Sequence(Alphabets.DNA, reference);
            referenceSeq.ID = "R1";
            referenceSeqs.Add(referenceSeq);

            reference = "TAGCT";
            referenceSeq = new Sequence(Alphabets.DNA, reference);
            referenceSeq.ID = "R11";
            referenceSeqs.Add(referenceSeq);

            searchSeqs = new List<ISequence>();

            string search = "CCGCGCCCCCTCAGCT";
            searchSeq = new Sequence(Alphabets.DNA, search);
            searchSeq.ID = "Q1";
            searchSeqs.Add(searchSeq);

            NucmerPairwiseAligner nucmer = new NucmerPairwiseAligner();
            nucmer.FixedSeparation = 0;
            nucmer.MinimumScore = 2;
            nucmer.SeparationFactor = -1;
            nucmer.LengthOfMUM = 3;
            nucmer.ForwardOnly = true;
            IList<IPairwiseSequenceAlignment> result = nucmer.Align(referenceSeqs, searchSeqs).Select(a => a as IPairwiseSequenceAlignment).ToList();

            // Check if output is not null
            Assert.AreNotEqual(null, result);
            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            PairwiseAlignedSequence alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(Alphabets.DNA, "GCGCATCCCC");
            alignedSeq.SecondSequence = new Sequence(Alphabets.DNA, "GCGC--CCCC");
            alignedSeq.Consensus = new Sequence(Alphabets.DNA, "GCGCATCCCC");
            alignedSeq.Score = -5;
            alignedSeq.FirstOffset = 0;
            alignedSeq.SecondOffset = 0;
            align.PairwiseAlignedSequences.Add(alignedSeq);
            alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(Alphabets.DNA, "AGCT");
            alignedSeq.SecondSequence = new Sequence(Alphabets.DNA, "AGCT");
            alignedSeq.Consensus = new Sequence(Alphabets.DNA, "AGCT");
            alignedSeq.Score = 12;
            alignedSeq.FirstOffset = 11;
            alignedSeq.SecondOffset = 0;
            align.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(align);
            Assert.IsTrue(AlignmentHelpers.CompareAlignment(result, expectedOutput));
        }

        /// <summary>
        /// Test NUCmer3 with multiple reference sequences
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestNUCmer3MultipleReferencesAndQueries()
        {
            Sequence referenceSeq = null;
            Sequence searchSeq = null;
            List<ISequence> referenceSeqs = null;
            List<ISequence> searchSeqs = null;

            referenceSeqs = new List<ISequence>();

            string reference = "ATGCGCATCCCC";
            referenceSeq = new Sequence(Alphabets.DNA, reference);
            referenceSeq.ID = "R1";
            referenceSeqs.Add(referenceSeq);

            reference = "TAGCT";
            referenceSeq = new Sequence(Alphabets.DNA, reference);
            referenceSeq.ID = "R11";
            referenceSeqs.Add(referenceSeq);

            searchSeqs = new List<ISequence>();

            string search = "CCGCGCCCCCTC";
            searchSeq = new Sequence(Alphabets.DNA, search);
            searchSeq.ID = "Q1";
            searchSeqs.Add(searchSeq);

            search = "AGCT";
            searchSeq = new Sequence(Alphabets.DNA, search);
            searchSeq.ID = "Q11";
            searchSeqs.Add(searchSeq);

            NucmerPairwiseAligner nucmer = new NucmerPairwiseAligner();
            nucmer.FixedSeparation = 0;
            nucmer.MinimumScore = 2;
            nucmer.SeparationFactor = -1;
            nucmer.LengthOfMUM = 3;
            nucmer.ForwardOnly = true;
            IList<IPairwiseSequenceAlignment> result = nucmer.Align(referenceSeqs, searchSeqs).Select(a => a as IPairwiseSequenceAlignment).ToList();

            // Check if output is not null
            Assert.AreNotEqual(null, result);

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            PairwiseAlignedSequence alignedSeq = new PairwiseAlignedSequence();

            alignedSeq.FirstSequence = new Sequence(Alphabets.DNA, "GCGCATCCCC");
            alignedSeq.SecondSequence = new Sequence(Alphabets.DNA, "GCGC--CCCC");
            alignedSeq.Consensus = new Sequence(Alphabets.DNA, "GCGCATCCCC");
            alignedSeq.Score = -5;
            alignedSeq.FirstOffset = 0;
            alignedSeq.SecondOffset = 0;
            align.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(align);
            align = new PairwiseSequenceAlignment();
            alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(Alphabets.DNA, "AGCT");
            alignedSeq.SecondSequence = new Sequence(Alphabets.DNA, "AGCT");
            alignedSeq.Consensus = new Sequence(Alphabets.DNA, "AGCT");
            alignedSeq.Score = 12;
            alignedSeq.FirstOffset = 0;
            alignedSeq.SecondOffset = 1;
            align.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(align);
            Assert.IsTrue(AlignmentHelpers.CompareAlignment(result, expectedOutput));
        }

        /// <summary>
        /// Test NUCmer3 with multiple reference sequences
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestNUCmer3CustomBreakLength()
        {
            Sequence referenceSeq = null;
            Sequence searchSeq = null;
            List<ISequence> referenceSeqs = null;
            List<ISequence> searchSeqs = null;

            referenceSeqs = new List<ISequence>();

            string reference = "CAAAAGGGATTGCAAATGTTGGAGTGAATGCCATTACCTACCGGCTAGGAGGAGT";
            referenceSeq = new Sequence(Alphabets.DNA, reference);
            referenceSeq.ID = "R1";
            referenceSeqs.Add(referenceSeq);

            reference = "CCCCCCCCC";
            referenceSeq = new Sequence(Alphabets.DNA, reference);
            referenceSeq.ID = "R2";
            referenceSeqs.Add(referenceSeq);

            reference = "TTTTT";
            referenceSeq = new Sequence(Alphabets.DNA, reference);
            referenceSeq.ID = "R3";
            referenceSeqs.Add(referenceSeq);

            searchSeqs = new List<ISequence>();

            string search = "CATTAATGATAAAGGGAAAGAAGTCCTCGTGCTATGGGGCATTCACCATCCATCTACTAGTGCTGACCAA";
            searchSeq = new Sequence(Alphabets.DNA, search);
            searchSeq.ID = "Q1";
            searchSeqs.Add(searchSeq);

            search = "CAAAGTCTCTATCAGAATGCAGATGCAGATGTTTTTGTGGGGTCATCAAGATATAGCAAGAAGTTCAAGC";
            searchSeq = new Sequence(Alphabets.DNA, search);
            searchSeq.ID = "Q2";
            searchSeqs.Add(searchSeq);

            search = "AAGCAAAATTAAACAGAGAAGAAATAGATGGGGTAAAGCTGGAATCAACAAGGATTTACCAGATTTTGGC";
            searchSeq = new Sequence(Alphabets.DNA, search);
            searchSeq.ID = "Q3";
            searchSeqs.Add(searchSeq);

            NucmerPairwiseAligner nucmer = new NucmerPairwiseAligner();
            nucmer.MaximumSeparation = 0;
            nucmer.MinimumScore = 2;
            nucmer.SeparationFactor = 0.12F;
            nucmer.LengthOfMUM = 5;
            nucmer.BreakLength = 2;
            nucmer.ForwardOnly = true;
            IList<IPairwiseSequenceAlignment> result = nucmer.Align(referenceSeqs, searchSeqs).Select(a => a as IPairwiseSequenceAlignment).ToList();

            // Check if output is not null
            Assert.AreNotEqual(null, result);

            List<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            PairwiseAlignedSequence alignedSeq = new PairwiseAlignedSequence();

            alignedSeq.FirstSequence = new Sequence(Alphabets.DNA, "AAAGGGA");
            alignedSeq.SecondSequence = new Sequence(Alphabets.DNA, "AAAGGGA");
            alignedSeq.Consensus = new Sequence(Alphabets.DNA, "AAAGGGA");
            alignedSeq.Score = 21;
            alignedSeq.FirstOffset = 8;
            alignedSeq.SecondOffset = 0;
            align.PairwiseAlignedSequences.Add(alignedSeq);
            alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(Alphabets.DNA, "CATTA");
            alignedSeq.SecondSequence = new Sequence(Alphabets.DNA, "CATTA");
            alignedSeq.Consensus = new Sequence(Alphabets.DNA, "CATTA");
            alignedSeq.Score = 15;
            alignedSeq.FirstOffset = 0;
            alignedSeq.SecondOffset = 31;
            align.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(align);

            align = new PairwiseSequenceAlignment();
            alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(Alphabets.DNA, "ATGTT");
            alignedSeq.SecondSequence = new Sequence(Alphabets.DNA, "ATGTT");
            alignedSeq.Consensus = new Sequence(Alphabets.DNA, "ATGTT");
            alignedSeq.Score = 15;
            alignedSeq.FirstOffset = 13;
            alignedSeq.SecondOffset = 0;
            align.PairwiseAlignedSequences.Add(alignedSeq);
            alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(Alphabets.DNA, "GAATGC");
            alignedSeq.SecondSequence = new Sequence(Alphabets.DNA, "GAATGC");
            alignedSeq.Consensus = new Sequence(Alphabets.DNA, "GAATGC");
            alignedSeq.Score = 18;
            alignedSeq.FirstOffset = 0;
            alignedSeq.SecondOffset = 11;
            align.PairwiseAlignedSequences.Add(alignedSeq);
            alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(Alphabets.DNA, "TTTTT");
            alignedSeq.SecondSequence = new Sequence(Alphabets.DNA, "TTTTT");
            alignedSeq.Consensus = new Sequence(Alphabets.DNA, "TTTTT");
            alignedSeq.Score = 15;
            alignedSeq.FirstOffset = 31;
            alignedSeq.SecondOffset = 0;
            align.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(align);

            align = new PairwiseSequenceAlignment();
            alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(Alphabets.DNA, "CAAAA");
            alignedSeq.SecondSequence = new Sequence(Alphabets.DNA, "CAAAA");
            alignedSeq.Consensus = new Sequence(Alphabets.DNA, "CAAAA");
            alignedSeq.Score = 15;
            alignedSeq.FirstOffset = 3;
            alignedSeq.SecondOffset = 0;
            align.PairwiseAlignedSequences.Add(alignedSeq);
            alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(Alphabets.DNA, "GGATT");
            alignedSeq.SecondSequence = new Sequence(Alphabets.DNA, "GGATT");
            alignedSeq.Consensus = new Sequence(Alphabets.DNA, "GGATT");
            alignedSeq.Score = 15;
            alignedSeq.FirstOffset = 45;
            alignedSeq.SecondOffset = 0;
            align.PairwiseAlignedSequences.Add(alignedSeq);
            alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(Alphabets.DNA, "GCAAA");
            alignedSeq.SecondSequence = new Sequence(Alphabets.DNA, "GCAAA");
            alignedSeq.Consensus = new Sequence(Alphabets.DNA, "GCAAA");
            alignedSeq.Score = 15;
            alignedSeq.FirstOffset = 0;
            alignedSeq.SecondOffset = 9;
            align.PairwiseAlignedSequences.Add(alignedSeq);
            alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(Alphabets.DNA, "TTACC");
            alignedSeq.SecondSequence = new Sequence(Alphabets.DNA, "TTACC");
            alignedSeq.Consensus = new Sequence(Alphabets.DNA, "TTACC");
            alignedSeq.Score = 15;
            alignedSeq.FirstOffset = 22;
            alignedSeq.SecondOffset = 0;
            align.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(align);
            Assert.IsTrue(AlignmentHelpers.CompareAlignment(result, expectedOutput));
        }

        #endregion NUCmer Test Cases
    }
}
