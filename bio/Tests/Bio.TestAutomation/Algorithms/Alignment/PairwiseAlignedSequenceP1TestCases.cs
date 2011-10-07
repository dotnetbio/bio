/****************************************************************************
 * PairwiseAlignedSequenceP1TestCases.cs
 * 
 * This file contains the PairwiseAlignedSequence P1 test cases.
 * 
******************************************************************************/

using System.Collections.Generic;
using Bio.Algorithms.Alignment;
using Bio.Util.Logging;
using System.Linq;
using Bio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM = Bio.SimilarityMatrices.SimilarityMatrix;

namespace Bio.TestAutomation.Algorithms.Alignment
{
    /// <summary>
    /// Test Automation code for Bio Sequences and P1 level validations.
    /// </summary>
    [TestClass]
    public class PairwiseAlignedSequenceP1TestCases
    {

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static PairwiseAlignedSequenceP1TestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region PairwiseAlignedSequence P1 TestCases

        /// <summary>
        /// Validate the attributes in PairwiseAlignedSequence.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePairwiseAlignedSequenceCustomBreakLength()
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
            Assert.IsTrue(CompareAlignment(result, expectedOutput));

            ApplicationLog.WriteLine(
                "PairwiseAlignedSequence P1: Successfully validated Sequence with Custom Break Length.");
        }

        /// <summary>
        /// Validate PairwiseAlignedSequence with Multiple reference.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePairwiseAlignedSequenceMultipleRef()
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
            Assert.IsTrue(CompareAlignment(result, expectedOutput));
            ApplicationLog.WriteLine(
                 "PairwiseAlignedSequence P1: Successfully validated Sequence with Multiple Reference.");
        }

        /// <summary>
        /// Validate PairwiseAlignedSequence with multiple reference & query sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePairwiseAlignedSequenceMultipleRefQuery()
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
            Assert.IsTrue(CompareAlignment(result, expectedOutput));
            ApplicationLog.WriteLine(
                "PairwiseAlignedSequence P1: Successfully validated Sequence with Multiple Reference.");
        }

        #endregion PairwiseAlignedSequence P1 TestCases

        #region Supporting Methods

        /// <summary>
        /// Compare the alignment of mummer and defined alignment
        /// </summary>
        /// <param name="result">output of Aligners</param>
        /// <param name="expectedAlignment">expected output</param>
        /// <returns>Compare result of alignments</returns>
        private static bool CompareAlignment(
                IList<IPairwiseSequenceAlignment> result,
                IList<IPairwiseSequenceAlignment> expectedAlignment)
        {
            bool output = true;
            if (result.Count == expectedAlignment.Count)
            {
                for (int count = 0; count < result.Count; count++)
                {
                    if (result[count].PairwiseAlignedSequences.Count == expectedAlignment[count].PairwiseAlignedSequences.Count)
                    {
                        for (int count1 = 0; count1 < result[count].PairwiseAlignedSequences.Count; count1++)
                        {
                            if (result[count].PairwiseAlignedSequences[count1].FirstSequence.ToString().Equals(
                                    expectedAlignment[count].PairwiseAlignedSequences[count1].FirstSequence.ToString())
                                && result[count].PairwiseAlignedSequences[count1].SecondSequence.ToString().Equals(
                                    expectedAlignment[count].PairwiseAlignedSequences[count1].SecondSequence.ToString())
                                && result[count].PairwiseAlignedSequences[count1].Consensus.ToString().Equals(
                                    expectedAlignment[count].PairwiseAlignedSequences[count1].Consensus.ToString())
                                && result[count].PairwiseAlignedSequences[count1].FirstOffset ==
                                    expectedAlignment[count].PairwiseAlignedSequences[count1].FirstOffset
                                && result[count].PairwiseAlignedSequences[count1].SecondOffset ==
                                    expectedAlignment[count].PairwiseAlignedSequences[count1].SecondOffset
                                && result[count].PairwiseAlignedSequences[count1].Score ==
                                    expectedAlignment[count].PairwiseAlignedSequences[count1].Score)
                            {
                                output = true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            return output;
        }

        #endregion Supporting Methods
    }
}

