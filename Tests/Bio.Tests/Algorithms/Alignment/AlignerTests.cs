using System.Collections.Generic;
using System.Linq;
using Bio.Algorithms.Alignment;
using Bio.SimilarityMatrices;
using Bio.Tests.Framework;
using NUnit.Framework;

namespace Bio.Tests.Algorithms.Alignment
{
    /// <summary>
    /// Tests for the Aligner classes.
    /// </summary>
    [TestFixture]
    public class AlignerTests
    {
        #region Smith-Waterman Aligner
        /// <summary>
        /// Test SmithWatermanAligner using Protein Sequence and Simple Gap Penalty Function.
        /// </summary>
        [Test]
        [Category("Priority0")]
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
        [Test]
        [Category("Priority0")]
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
        [Test]
        [Category("Priority0")]
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
        [Test]
        [Category("Priority0")]
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
        [Test]
        [Category("Priority0")]
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
        [Test]
        [Category("Priority0")]
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
        [Test]
        [Category("Priority0")]
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
        [Test]
        [Category("Priority0")]
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
        [Test]
        [Category("Priority0")]
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
        [Test]
        [Category("Priority0")]
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
        [Test]
        [Category("Priority0")]
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
        [Test]
        [Category("Priority0")]
        public void TestNUCmer3CustomBreakLength()
        {
            var referenceSeqs = new List<ISequence>
            {
                new Sequence(Alphabets.DNA, "CAAAAGGGATTGCAAATGTTGGAGTGAATGCCATTACCTACCGGCTAGGAGGAGT") { ID = "R1" },
                new Sequence(Alphabets.DNA, "CCCCCCCCC") { ID = "R2" },
                new Sequence(Alphabets.DNA, "TTTTT") { ID = "R3" },
            };

            var searchSeqs = new List<ISequence>
            {
                new Sequence(Alphabets.DNA, "CATTAATGATAAAGGGAAAGAAGTCCTCGTGCTATGGGGCATTCACCATCCATCTACTAGTGCTGACCAA") { ID = "Q1" },
                new Sequence(Alphabets.DNA, "CAAAGTCTCTATCAGAATGCAGATGCAGATGTTTTTGTGGGGTCATCAAGATATAGCAAGAAGTTCAAGC") { ID = "Q2" },
                new Sequence(Alphabets.DNA, "AAGCAAAATTAAACAGAGAAGAAATAGATGGGGTAAAGCTGGAATCAACAAGGATTTACCAGATTTTGGC") { ID = "Q3" },
            };

            NucmerPairwiseAligner nucmer = new NucmerPairwiseAligner
            {
                MaximumSeparation = 0,
                MinimumScore = 2,
                SeparationFactor = 0.12F,
                LengthOfMUM = 5,
                BreakLength = 2,
                ForwardOnly = true
            };

            var result = nucmer.Align(referenceSeqs, searchSeqs)
                .Select(a => a as IPairwiseSequenceAlignment)
                .ToList();

            // Check if output is not null
            Assert.IsNotNull(result);

            var expectedOutput = new List<IPairwiseSequenceAlignment>();

            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            align.PairwiseAlignedSequences.Add(new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(Alphabets.DNA, "AAAGGGA"),
                SecondSequence = new Sequence(Alphabets.DNA, "AAAGGGA"),
                Consensus = new Sequence(Alphabets.DNA, "AAAGGGA"),
                Score = 21,
                FirstOffset = 8,
                SecondOffset = 0
            });
            align.PairwiseAlignedSequences.Add(new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(Alphabets.DNA, "CATTA"),
                SecondSequence = new Sequence(Alphabets.DNA, "CATTA"),
                Consensus = new Sequence(Alphabets.DNA, "CATTA"),
                Score = 15,
                FirstOffset = 0,
                SecondOffset = 31
            });
            expectedOutput.Add(align);

            align = new PairwiseSequenceAlignment();
            align.PairwiseAlignedSequences.Add(new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(Alphabets.DNA, "ATGTT"),
                SecondSequence = new Sequence(Alphabets.DNA, "ATGTT"),
                Consensus = new Sequence(Alphabets.DNA, "ATGTT"),
                Score = 15,
                FirstOffset = 13,
                SecondOffset = 0
            });
            align.PairwiseAlignedSequences.Add(new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(Alphabets.DNA, "GAATGC"),
                SecondSequence = new Sequence(Alphabets.DNA, "GAATGC"),
                Consensus = new Sequence(Alphabets.DNA, "GAATGC"),
                Score = 18,
                FirstOffset = 0,
                SecondOffset = 11
            });
            align.PairwiseAlignedSequences.Add(new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(Alphabets.DNA, "TTTTT"),
                SecondSequence = new Sequence(Alphabets.DNA, "TTTTT"),
                Consensus = new Sequence(Alphabets.DNA, "TTTTT"),
                Score = 15,
                FirstOffset = 31,
                SecondOffset = 0
            });
            expectedOutput.Add(align);

            align = new PairwiseSequenceAlignment();
            align.PairwiseAlignedSequences.Add(new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(Alphabets.DNA, "CAAAA"),
                SecondSequence = new Sequence(Alphabets.DNA, "CAAAA"),
                Consensus = new Sequence(Alphabets.DNA, "CAAAA"),
                Score = 15,
                FirstOffset = 3,
                SecondOffset = 0
            });
            align.PairwiseAlignedSequences.Add(new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(Alphabets.DNA, "GGATT"),
                SecondSequence = new Sequence(Alphabets.DNA, "GGATT"),
                Consensus = new Sequence(Alphabets.DNA, "GGATT"),
                Score = 15,
                FirstOffset = 45,
                SecondOffset = 0
            });
            align.PairwiseAlignedSequences.Add(new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(Alphabets.DNA, "GCAAA"),
                SecondSequence = new Sequence(Alphabets.DNA, "GCAAA"),
                Consensus = new Sequence(Alphabets.DNA, "GCAAA"),
                Score = 15,
                FirstOffset = 0,
                SecondOffset = 9
            });
            align.PairwiseAlignedSequences.Add(new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(Alphabets.DNA, "TTACC"),
                SecondSequence = new Sequence(Alphabets.DNA, "TTACC"),
                Consensus = new Sequence(Alphabets.DNA, "TTACC"),
                Score = 15,
                FirstOffset = 22,
                SecondOffset = 0
            });
            expectedOutput.Add(align);

            Assert.IsTrue(AlignmentHelpers.CompareAlignment(result, expectedOutput));

        }

        #endregion NUCmer Test Cases
    }
}
