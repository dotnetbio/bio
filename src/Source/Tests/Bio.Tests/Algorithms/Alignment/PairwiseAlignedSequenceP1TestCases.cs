using System.Collections.Generic;
using System.Linq;

using Bio.Algorithms.Alignment;
using Bio.Tests.Framework;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests.Algorithms.Alignment
{
    /// <summary>
    ///     Test Automation code for Bio Sequences and P1 level validations.
    /// </summary>
    [TestFixture]
    public class PairwiseAlignedSequenceP1TestCases
    {
        #region PairwiseAlignedSequence P1 TestCases

        /// <summary>
        ///     Validate the attributes in PairwiseAlignedSequence.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidatePairwiseAlignedSequenceCustomBreakLength()
        {
            var referenceSeqs = new List<ISequence>()
            {
                new Sequence(Alphabets.DNA, "CAAAAGGGATTGCAAATGTTGGAGTGAATGCCATTACCTACCGGCTAGGAGGAGT") {ID = "R1"},
                new Sequence(Alphabets.DNA, "CCCCCCCCC") {ID = "R2"},
                new Sequence(Alphabets.DNA, "TTTTT") {ID = "R3"}
            };

            var searchSeqs = new List<ISequence>()
            {
                new Sequence(Alphabets.DNA, "CATTAATGATAAAGGGAAAGAAGTCCTCGTGCTATGGGGCATTCACCATCCATCTACTAGTGCTGACCAA") { ID = "Q1" },
                new Sequence(Alphabets.DNA, "CAAAGTCTCTATCAGAATGCAGATGCAGATGTTTTTGTGGGGTCATCAAGATATAGCAAGAAGTTCAAGC") { ID = "Q2" },
                new Sequence(Alphabets.DNA, "AAGCAAAATTAAACAGAGAAGAAATAGATGGGGTAAAGCTGGAATCAACAAGGATTTACCAGATTTTGGC") { ID = "Q3" },
            };

            var nucmer = new NucmerPairwiseAligner
            {
                MaximumSeparation = 0,
                MinimumScore = 2,
                SeparationFactor = 0.12F,
                LengthOfMUM = 5,
                BreakLength = 2,
                ForwardOnly = true,
            };

            var result = nucmer.Align(referenceSeqs, searchSeqs).Select(a => a as IPairwiseSequenceAlignment).ToList();

            // Check if output is not null
            Assert.AreNotEqual(null, result);

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

            ApplicationLog.WriteLine("PairwiseAlignedSequence P1: Successfully validated Sequence with Custom Break Length.");
        }

        /// <summary>
        ///     Validate PairwiseAlignedSequence with Multiple reference.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidatePairwiseAlignedSequenceMultipleRef()
        {
            var referenceSeqs = new List<ISequence>()
            {
                new Sequence(Alphabets.DNA, "ATGCGCATCCCC") {ID = "R1"},
                new Sequence(Alphabets.DNA, "TAGCT") {ID = "R11"},
            };

            var searchSeqs = new List<ISequence>()
            {
                new Sequence(Alphabets.DNA, "CCGCGCCCCCTCAGCT") {ID = "Q1"}
            };

            var nucmer = new NucmerPairwiseAligner
            {
                FixedSeparation = 0,
                MinimumScore = 2,
                SeparationFactor = -1,
                LengthOfMUM = 3,
                ForwardOnly = true,
            };

            IList<IPairwiseSequenceAlignment> result = nucmer.Align(referenceSeqs, searchSeqs).Select(a => a as IPairwiseSequenceAlignment).ToList();

            // Check if output is not null
            Assert.AreNotEqual(null, result);

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            
            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            var alignedSeq = new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(Alphabets.DNA, "GCGCATCCCC"),
                SecondSequence = new Sequence(Alphabets.DNA, "GCGC--CCCC"),
                Consensus = new Sequence(Alphabets.DNA, "GCGCATCCCC"),
                Score = -5,
                FirstOffset = 0,
                SecondOffset = 0
            };
            align.PairwiseAlignedSequences.Add(alignedSeq);
            alignedSeq = new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(Alphabets.DNA, "AGCT"),
                SecondSequence = new Sequence(Alphabets.DNA, "AGCT"),
                Consensus = new Sequence(Alphabets.DNA, "AGCT"),
                Score = 12,
                FirstOffset = 11,
                SecondOffset = 0
            };
            align.PairwiseAlignedSequences.Add(alignedSeq);

            expectedOutput.Add(align);
            Assert.IsTrue(AlignmentHelpers.CompareAlignment(result, expectedOutput));
            ApplicationLog.WriteLine("PairwiseAlignedSequence P1: Successfully validated Sequence with Multiple Reference.");
        }

        /// <summary>
        ///     Validate PairwiseAlignedSequence with multiple reference & query sequences
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidatePairwiseAlignedSequenceMultipleRefQuery()
        {
            var referenceSeqs = new List<ISequence>()
            {
                new Sequence(Alphabets.DNA, "ATGCGCATCCCC") {ID = "R1"},
                new Sequence(Alphabets.DNA, "TAGCT") {ID = "R2"}
            };

            var searchSeqs = new List<ISequence>()
            {
                new Sequence(Alphabets.DNA, "CCGCGCCCCCTC") {ID = "Q1"},
                new Sequence(Alphabets.DNA, "AGCT") {ID = "Q2"}
            };

            var nucmer = new NucmerPairwiseAligner
            {
                FixedSeparation = 0,
                MinimumScore = 2,
                SeparationFactor = -1,
                LengthOfMUM = 3,
                ForwardOnly = true,
            };

            IList<IPairwiseSequenceAlignment> result = nucmer.Align(referenceSeqs, searchSeqs).Select(a => a as IPairwiseSequenceAlignment).ToList();

            // Check if output is not null
            Assert.AreNotEqual(null, result);

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            var alignedSeq = new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(Alphabets.DNA, "GCGCATCCCC"),
                SecondSequence = new Sequence(Alphabets.DNA, "GCGC--CCCC"),
                Consensus = new Sequence(Alphabets.DNA, "GCGCATCCCC"),
                Score = -5,
                FirstOffset = 0,
                SecondOffset = 0
            };
            align.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(align);

            align = new PairwiseSequenceAlignment();
            alignedSeq = new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(Alphabets.DNA, "AGCT"),
                SecondSequence = new Sequence(Alphabets.DNA, "AGCT"),
                Consensus = new Sequence(Alphabets.DNA, "AGCT"),
                Score = 12,
                FirstOffset = 0,
                SecondOffset = 1
            };
            align.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(align);

            Assert.IsTrue(AlignmentHelpers.CompareAlignment(result, expectedOutput));
            ApplicationLog.WriteLine("PairwiseAlignedSequence P1: Successfully validated Sequence with Multiple Reference.");
        }

        #endregion PairwiseAlignedSequence P1 TestCases
    }
}