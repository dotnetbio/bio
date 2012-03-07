using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Util;
using System.Globalization;
using Bio.Algorithms.Alignment;
using Bio.Algorithms.SuffixTree;
using Bio.Algorithms.Assembly;
using Bio.SimilarityMatrices;
using Bio.Algorithms.Assembly.Padena;

namespace Bio.Tests
{
    /// <summary>
    /// Summary description for TestToStringForBio
    /// </summary>
    [TestClass]
    public class TestToStringForBio
    {
        /// <summary>
        /// Test for Sequence ConvertToString method.
        /// </summary>
        [TestMethod]
        public void TestSequenceConvertToString()
        {
            Sequence seq = new Sequence(Alphabets.DNA, "ATCGATCGGATCGATCGGCTACTAATATCGATCGGCTACGATCGGCTAATCGATCGATCGGCTAATCGATCGATCGGCTAGCTA");
            string expectedValue = "TCGATCGGCT";
            string actualValue = seq.ConvertToString(10, 10);

            Assert.AreEqual(expectedValue, actualValue);
        }

        /// <summary>
        /// Test for Sequence ConvertToString method.
        /// </summary>
        [TestMethod]
        public void TestDerivedSequenceConvertToString()
        {
            Sequence baseSeq = new Sequence(Alphabets.DNA, "ATCGATCGGATCGATCGGCTACTAATATCGATCGGCTACGATCGGCTAATCGATCGATCGGCTAATCGATCGATCGGCTAGCTA");
            DerivedSequence seq = new DerivedSequence(baseSeq, false, false);
            string expectedValue = "TCGATCGGCT";
            string actualValue = seq.ConvertToString(10, 10);

            Assert.AreEqual(expectedValue, actualValue);
        }

        /// <summary>
        /// Test for Sequence ConvertToString method.
        /// </summary>
        [TestMethod]
        public void TestSequenceConvertToStringWithException()
        {
            Sequence seq = new Sequence(Alphabets.DNA, "ATCGATCGGATCGATCGGCTACTAATATCGATCGGCTACGATCGGCTAATCGATCGATCGGCTAATCGATCGATCGGCTAGCTA");

            try
            {
                seq.ConvertToString(80, 10);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.AreEqual("length", ex.ParamName);
            }
            
        }

        /// <summary>
        /// Test for Sequence ToString() Function.
        /// </summary>
        [TestMethod]
        public void TestSequenceToString()
        {
            ISequence seqSmall = new Sequence(Alphabets.DNA, "ATCG");
            ISequence seqLarge = new Sequence(Alphabets.DNA, "ATCGGGGGGGGGGGGGGGGGGGGGGGGCCCCCCCCCCCCCCCCCCCCCCGGGGGGGGGGTTTTTTTTTTTTTTT");
            string ActualSmallString = seqSmall.ToString();
            string ActualLargeString = seqLarge.ToString();
            string ExpectedSmallString = "ATCG";
            string expectedLargeString = string.Format(CultureInfo.CurrentCulture, "ATCGGGGGGGGGGGGGGGGGGGGGGGGCCCCCCCCCCCCCCCCCCCCCCGGGGGGGGGGTTTTT... +[{0}]", (seqLarge.Count - Helper.AlphabetsToShowInToString));

            Assert.AreEqual(ExpectedSmallString, ActualSmallString);
            Assert.AreEqual(expectedLargeString, ActualLargeString);
        }

        /// <summary>
        /// Test for DeriveSequence ToString() Function.
        /// </summary>
        [TestMethod]
        public void TestDerivedSequenceToString()
        {
            ISequence seqSmall = new Sequence(Alphabets.DNA, "ATCG");
            ISequence seqLarge = new Sequence(Alphabets.DNA, "ATCGGGGGGGGGGGGGGGGGGGGGGGGCCCCCCCCCCCCCCCCCCCCCCGGGGGGGGGGTTTTTTTTTTTTTTT");
            ISequence DeriveSeqSmall = new DerivedSequence(seqSmall, false, true);
            ISequence DeriveSeqLarge = new DerivedSequence(seqLarge, false, true);

            string ActualSmallString = DeriveSeqSmall.ToString();
            string ActualLargeString = DeriveSeqLarge.ToString();
            string ExpectedSmallString = "TAGC";
            string expectedLargeString = string.Format(CultureInfo.CurrentCulture, "TAGCCCCCCCCCCCCCCCCCCCCCCCCGGGGGGGGGGGGGGGGGGGGGGCCCCCCCCCCAAAAA... +[{0}]", (seqLarge.Count - Helper.AlphabetsToShowInToString));

            Assert.AreEqual(ExpectedSmallString, ActualSmallString);
            Assert.AreEqual(expectedLargeString, ActualLargeString);
        }

        /// <summary>
        /// Test for QualitativeSequence ToString() Function.
        /// </summary>
        [TestMethod]
        public void TestQualitativeSequenceToString()
        {
            byte[] seqData = new byte[4];
            seqData[0] = (byte)'A';
            seqData[1] = (byte)'T';
            seqData[2] = (byte)'C';
            seqData[3] = (byte)'G';
            byte[] qualityScores = new byte[4];
            qualityScores[0] = (byte)'A';
            qualityScores[1] = (byte)'A';
            qualityScores[2] = (byte)'A';
            qualityScores[3] = (byte)'B';
            QualitativeSequence seq = new QualitativeSequence(Alphabets.DNA, FastQFormatType.Illumina_v1_3, seqData, qualityScores);
            string actualString = seq.ToString();
            string expectedString = "ATCG\r\nAAAB";
            Assert.AreEqual(actualString, expectedString);
        }

        /// <summary>
        /// Test for All Alphabets ToString() Function.
        /// </summary>
        [TestMethod]
        public void TestAllAlphabetsToString()
        {
            DnaAlphabet dna = DnaAlphabet.Instance;
            RnaAlphabet rna = RnaAlphabet.Instance;
            ProteinAlphabet protein = ProteinAlphabet.Instance;
            AmbiguousDnaAlphabet dnaAmbiguous = AmbiguousDnaAlphabet.Instance;
            AmbiguousRnaAlphabet rnaAmbiguous = AmbiguousRnaAlphabet.Instance;
            AmbiguousProteinAlphabet proteinAmbiguous = AmbiguousProteinAlphabet.Instance;

            string dnaStringActual = dna.ToString();
            string rnaStringActual = rna.ToString();
            string proteinStringActual = protein.ToString();
            string dnaAmbiguousStringActual = dnaAmbiguous.ToString();
            string rnaAmbiguousStringActual = rnaAmbiguous.ToString();
            string proteinAmbiguousStringActual = proteinAmbiguous.ToString();

            string dnaStringExpected = "ACGT-";
            string rnaStringExpected = "ACGU-";
            string proteinStringExpected = "ACDEFGHIKLMNOPQRSTUVWY-*";
            string dnaAmbiguousStringExpected = "ACGT-MRSWYKVHDBN";
            string rnaAmbiguousStringExpected = "ACGU-NMRSWYKVHDB";
            string proteinAmbiguousStringExpected = "ACDEFGHIKLMNOPQRSTUVWY-*XZBJ";

            Assert.AreEqual(dnaStringExpected, dnaStringActual);
            Assert.AreEqual(rnaStringExpected, rnaStringActual);
            Assert.AreEqual(proteinStringExpected, proteinStringActual);
            Assert.AreEqual(dnaAmbiguousStringExpected, dnaAmbiguousStringActual);
            Assert.AreEqual(rnaAmbiguousStringExpected, rnaAmbiguousStringActual);
            Assert.AreEqual(proteinAmbiguousStringExpected, proteinAmbiguousStringActual);
        }

        /// <summary>
        /// Test for SequenceRange ToString() Function.
        /// </summary>
        [TestMethod]
        public void TestSequenceRangeToString()
        {
            SequenceRange range = new SequenceRange("chr20", 0, 3);
            string actualString = range.ToString();
            string expectedString = "ID=chr20 Start=0 End=3";
            Assert.AreEqual(actualString, expectedString);
        }

        /// <summary>
        /// Test for SequenceRangeGrouping ToString() Function.
        /// </summary>
        [TestMethod]
        public void TestSequenceRangeGroupingToString()
        {
            ISequenceRange range1 = new SequenceRange("chr20", 0, 3);
            ISequenceRange range2 = new SequenceRange("chr21", 0, 4);
            IList<ISequenceRange> ranges = new List<ISequenceRange>();
            ranges.Add(range1);
            ranges.Add(range2);
            
            SequenceRangeGrouping rangegrouping = new SequenceRangeGrouping(ranges);
            string actualString = rangegrouping.ToString();

            string expectedString = "ID=chr20 Start=0 End=3\r\nID=chr21 Start=0 End=4\r\n";
            Assert.AreEqual(actualString, expectedString);
        }

         /// <summary>
        /// Test for SequenceStatistics ToString() Function.
        /// </summary>
        [TestMethod]
        public void TestSequenceStatisticsToString()
        {
            ISequence seq = new Sequence(Alphabets.DNA, "ATCGATCG");
            SequenceStatistics seqStats = new SequenceStatistics(seq);
            string actualString = seqStats.ToString();
            string expectedString = "A - 2\r\nC - 2\r\nG - 2\r\nT - 2\r\n";
            Assert.AreEqual(actualString, expectedString);
        }

        /// <summary>
        /// Test for AlignedSequence ToString() Function.
        /// </summary>
        [TestMethod]
        public void TestAlignedSequenceToString()
        {
            IList<ISequence> seqList = new List<ISequence>();
            seqList.Add(new Sequence(Alphabets.DNA, "CAAAAGGGATTGC---TGTTGGAGTGAATGCCATTACCTACCGGCTAGGAGGAGTAGTACAAAGGAGC"));
            seqList.Add(new Sequence(Alphabets.DNA, "CAAAAGGGATTGC---"));
            seqList.Add(new Sequence(Alphabets.DNA, "TAGTAGTTCTGCTATATACATTTG"));
            seqList.Add(new Sequence(Alphabets.DNA, "GTTATCATGCGAACAATTCAACAGACACTGTAGA"));
            NucmerPairwiseAligner num = new NucmerPairwiseAligner();
            num.BreakLength = 8;
            num.FixedSeparation = 0;
            num.MinimumScore = 0;
            num.MaximumSeparation = 0;
            num.SeparationFactor = 0;
            num.LengthOfMUM = 8;
            IList<ISequence> sequenceList = seqList;
            IList<ISequenceAlignment> alignmentObj = num.Align(sequenceList);
            AlignedSequence alignedSeqs = (AlignedSequence)alignmentObj[0].AlignedSequences[0];

            string actualString = alignedSeqs.ToString();
            string expectedString = "CAAAAGGGATTGC---\r\nCAAAAGGGATTGC---\r\nCAAAAGGGATTGC---\r\n";
            Assert.AreEqual(actualString, expectedString);
        }

        /// <summary>
        /// Test for Cluster ToString() Function.
        /// </summary>
        [TestMethod]
        public void TestClusterToString()
        {
            Match match = new Match();

            MatchExtension matchExtn1 = new MatchExtension(match);
            matchExtn1.ID = 1;
            matchExtn1.Length = 20;
            MatchExtension matchExtn2 = new MatchExtension(match);
            matchExtn2.ID = 2;
            matchExtn2.Length = 30;
            IList<MatchExtension> extnList = new List<MatchExtension>();
            extnList.Add(matchExtn1);
            extnList.Add(matchExtn2);

            Cluster clust = new Cluster(extnList);
            string actualString = clust.ToString();
            string expectedString = "RefStart=0 QueryStart=0 Length=20 Score=0 WrapScore=0 IsGood=False\r\nRefStart=0 QueryStart=0 Length=30 Score=0 WrapScore=0 IsGood=False\r\n";
            Assert.AreEqual(actualString, expectedString);
        }

        /// <summary>
        /// Test for DeltaAlignment ToString() Function.
        /// </summary>
        [TestMethod]
        public void TestDeltaAlignmentToString()
        {
            ISequence refSeq = new Sequence(Alphabets.DNA, "ATCGGGGGGGGAAAAAAATTTTCCCCGGGGG");
            ISequence qrySeq = new Sequence(Alphabets.DNA, "GGGGG");
            DeltaAlignment delta = new DeltaAlignment(refSeq, qrySeq);
            delta.FirstSequenceEnd = 21;
            delta.SecondSequenceEnd = 20;
            string actualString = delta.ToString();
            string expectedString = "Ref ID= Query Id= Ref start=0 Ref End=21 Query start=0 Query End=20";
            Assert.AreEqual(actualString, expectedString);
        }

         /// <summary>
        /// Test for PairwiseAlignedSequence ToString() Function.
        /// </summary>
        [TestMethod]
        public void TestPairwiseAlignedSequenceToString()
        {
            PairwiseAlignedSequence alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(Alphabets.Protein, "AWGHE");
            alignedSeq.SecondSequence = new Sequence(Alphabets.Protein, "AW-HE");
            alignedSeq.Consensus = new Sequence(Alphabets.Protein, "AWGHE");
            alignedSeq.Score = 28;
            alignedSeq.FirstOffset = 0;
            alignedSeq.SecondOffset = 3;

            string actualString = alignedSeq.ToString();
            string expectedString = "AWGHE\r\nAWGHE\r\nAW-HE\r\n";
            Assert.AreEqual(actualString, expectedString);
        }

        /// <summary>
        /// Test for PairwiseSequenceAlignment ToString() Function.
        /// </summary>
        [TestMethod]
        public void TestPairwiseSequenceAlignmentToString()
        {
            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            PairwiseAlignedSequence alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(Alphabets.Protein, "AWGHE");
            alignedSeq.SecondSequence = new Sequence(Alphabets.Protein, "AW-HE");
            alignedSeq.Consensus = new Sequence(Alphabets.Protein, "AWGHE");
            alignedSeq.Score = 28;
            alignedSeq.FirstOffset = 0;
            alignedSeq.SecondOffset = 3;
            align.PairwiseAlignedSequences.Add(alignedSeq);

            string actualString = align.ToString();
            string expectedString = "AWGHE\r\nAWGHE\r\nAW-HE\r\n\r\n";
            Assert.AreEqual(actualString, expectedString); 
        }

         /// <summary>
        /// Test for Match And MatchExtension ToString() Function.
        /// </summary>
        [TestMethod]
        public void TestMatchAndMatchExtensionToString()
        {
            Match match = new Match();
            match.Length = 20;
            match.QuerySequenceOffset = 33;

            MatchExtension matchExtn = new MatchExtension(match);
            matchExtn.ID = 1;
            matchExtn.Length = 20;

            string actualMatchExtnString = matchExtn.ToString();
            string actualMatchstring = match.ToString();
            string ExpectedMatchExtnString = "RefStart=0 QueryStart=33 Length=20 Score=0 WrapScore=0 IsGood=False";
            string ExpectedMatchString = "RefStart=0 QueryStart=33 Length=20";

            Assert.AreEqual(ExpectedMatchExtnString, actualMatchExtnString);
            Assert.AreEqual(actualMatchstring, ExpectedMatchString);
        }

        /// <summary>
        /// Test for SequenceAlignment ToString() Function.
        /// </summary>
        [TestMethod]
        public void TestSequenceAlignmentToString()
        {
            ISequenceAligner aligner = SequenceAligners.NeedlemanWunsch;
            IAlphabet alphabet = Alphabets.Protein;
            string origSequence1 = "KRIPKSQNLRSIHSIFPFLEDKLSHLN";
            string origSequence2 = "LNIPSLITLNKSIYVFSKRKKRLSGFLHN";

            // Create input sequences
            List<ISequence> inputSequences = new List<ISequence>();
            inputSequences.Add(new Sequence(alphabet, origSequence1));
            inputSequences.Add(new Sequence(alphabet, origSequence2));

            // Get aligned sequences
            IList<ISequenceAlignment> alignments = aligner.Align(inputSequences);
            ISequenceAlignment alignment = new SequenceAlignment();
            for (int ialigned = 0; ialigned < alignments[0].AlignedSequences.Count; ialigned++)
            {
                alignment.AlignedSequences.Add(alignments[0].AlignedSequences[ialigned]);
            }

            foreach (string key in alignments[0].Metadata.Keys)
            {
                alignment.Metadata.Add(key, alignments[0].Metadata[key]);
            }

            string actualSequenceAlignmentString = alignment.ToString();
            string ExpectedSequenceAlignmentString = "XXIPXXXXLXXXXXXFXXXXXXLSGFXXN\r\nKRIPKSQNLRSIHSIFPFLEDKLS--HLN\r\nLNIPSLITLNKSIYVFSKRKKRLSGFLHN\r\n\r\n";

            Assert.AreEqual(ExpectedSequenceAlignmentString, actualSequenceAlignmentString);
        }

        /// <summary>
        /// Test for Contig ToString() Function.
        /// </summary>
        [TestMethod]
        public void TestContigToString()
        {
            // test parameters
            int matchScore = 5;
            int mismatchScore = -4;
            int gapCost = -10;
            double mergeThreshold = 4;
            double consensusThreshold = 66;

            Sequence seq2 = new Sequence(Alphabets.DNA, "ACAAAAGCAACAAAAATGAAGGCAATACTAGTAGTTCTGCTATATACATTTGCAACCGCAAATGCAGACACATTATGTATAGGTTATCATGCGAACAATTCAACAGACACTGTAGACACAGTACTAGAAAAGAATGTAACAGTAACACACTCTGTTAACCTTCTAGAAGACAAGCATAACGGGAAACTATGCAAACTAAGAGGAGTAGCCCCATTGCATTTGGGTAAATGTAACATTGCTGGCTGGATCCTGGGAAATCCAGAGTGTGAATCACTCTCCACAGCAAGCTCATGGTCCTACATTGTGGAAACATCTAGTTCAGACAATGGAACGTGTTACCCAGGAGATTTCATCGATTATGAGGAGCTAAGAGAGCAATTGAGCTCAGTGTCATCATTTGAAAGGTTTGAGATATTCCCCAAGACAAGTTCATGGCCCAATCATGACTCGAACAAAGGTGTAACGGCAGCATGTCCTCATGCTGGAGCAAAAAGCTTCTACAAAAATTTAATATGGCTAGTTAAAAAAGGAAATTCATACCCAAAGCTCAGCAAATCCTACATTAATGATAAAGGGAAAGAAGTCCTCGTGCTATGGGGCATTCACCATCCATCTACTAGTGCTGACCAACAAAGTCTCTATCAGAATGCAGATGCATATGTTTTTGTGGGGTCATCAAGATATAGCAAGAAGTTCAAGCCGGAAATAGCAATAAGACCCAAAGTGAGGGATCAAGAAGGGAGAATGAACTATTACTGGACACTAGTAGAGCCGGGAGACAAAATAACATTCGAAGCAACTGGAAATCTAGTGGTACCGAGATATGCATTCGCAATGGAAAGAAATGCTGGATCTGGTATTATCATTTCAGATACACCAGTCCACGATTGCAATACAACTTGTCAGACACCCAAGGGTGCTATAAACACCAGCCTCCCATTTCAGAATATACATCCGATCACAATTGGAAAATGTCCAAAATATGTAAAAAGCACAAAATTGAGACTGGCCACAGGATTGAGGAATGTCCCGTCTATTCAATCTAGAGGCCTATTTGGGGCCATTGCCGGTTTCATTGAAGGGGGGTGGACAGGGATGGTAGATGGATGGTACGGTTATCACCATCAAAATGAGCAGGGGTCAGGATATGCAGCCGACCTGAAGAGCACACAGAATGCCATTGACGAGATTACTAACAAAGTAAATTCTGTTATTGAAAAGATGAATATACAGTTCACAGCAGTAGGTAAAGAGTTCAACCACCTGGAAAAAAGAATAGAGAATTTAAATAAAAAAGTTGATGATGGTTTCCTGGACATTTGGACTTACAATGCCGAACTGTTGGTTCTATTGGAAAATGAAAGAACTTTGGACTACCACGATTCGAATGTGAAGAACTTATATGAAAAGGTAAGAAGCCAGCTAAAAAACAATGCCAAGGAAATTGGAAACGGCTGCTTTGAATTTTACCACAAATGCGATAACACGTGCATGGAAAGTGTCAAAAATGGGACTTATGACTACCCAAAATACTCAGAGGAAGCAAAATTAAACAGAGAAGAAATAGATGGGGTAAAGCTGGAATCAACAAGGATTTACCAGATTTTGGCGATCTATTCAACTGTCGCCAGTTCATTGGTACTGGTAGTCTCCCTGGGGGCAATCAGTTTCTGGATGTGCTCTAATGGGTCTCTACAGTGTAGAATATGTATTTAACATTAGGATTTCAGAAGCATGAGAAA");
            Sequence seq1 = new Sequence(Alphabets.DNA, "ATGAAGGCAATACTAGTAGTTCTGCTATATACATTTGCAACCGCAAATGCAGACACATTATGTATAGGTTATCATGCGAACAATTCAACAGACACTGTAGACACAGTACTAGAAAAGAATGTAACAGTAACACACTCTGTTAACCTTCTAGAAGACAAGCATAACGGGAAACTATGCAAACTAAGAGGGGTAGCCCCATTGCATTTGGGTAAATGTAACATTGCTGGCTGGATCCTGGGAAATCCAGAGTGTGAATCACTCTCCACAGCAAGCTCATGGTCCTACATTGTGGAAACATCTAGTTCAGACAATGGAACGTGTTACCCAGGAGATTTCATCGATTATGAGGAGCTAAGAGAGCAATTGAGCTCAGTGTCATCATTTGAAAGGTTTGAGATATTCCCCAAGACAAGTTCATGGCCCAATCATGACTCGAACAAAGGTGTAACGGCAGCATGTCCTCATGCTGGAGCAAAAAGCTTCTACAAAAATTTAATATGGCTAGTTAAAAAAGGAAATTCATACCCAAAGCTCAGCAAATCCTACATTAATGATAAAGGGAAAGAAGTCCTCGTGCTATGGGGCATTCACCATCCATCTACTAGTGCTGACCAACAAAGTCTCTATCAGAATGCAGATGCATATGTTTTTGTGGGGACATCAAGATACAGCAAGAAGTTCAAGCCGGAAATAGCAATAAGACCCAAAGTGAGGGATCAAGAAGGGAGAATGAACTATTACTGGACACTAGTAGAGCCGGGAGACAAAATAACATTCGAAGCAACTGGAAATCTAGTGGTACCGAGATATGCATTCGCAATGGAAAGAAATGCTGGATCTGGTATTATCATTTCAGATACACCAGTCCACGATTGCAATACAACTTGTCAGACACCCAAGGGTGCTATAAACACCAGCCTCCCATTTCAGAATATACATCCGATCACAATTGGAAAATGTCCAAAATATGTAAAAAGCACAAAATTGAGACTGGCCACAGGATTGAGGAATGTCCCGTCTATTCAATCTAGAGGCCTATTTGGGGCCATTGCCGGTTTCATTGAAGGGGGGTGGACAGGGATGGTAGATGGATGGTACGGTTATCACCATCAAAATGAGCAGGGGTCAGGATATGCAGCCGACCTGAAGAGCACACAGAATGCCATTGACGAGATTACTAACAAAGTAAATTCTGTTATTGAAAAGATGAATACACAGTTCACAGCAGTAGGTAAAGAGTTCAACCACCTGGAAAAAAGAATAGAGAATTTAAATAAAAAAATTGATGATGGTTTCCTGGACATTTGGACTTACAATGCCGAACTGTTGGTTCTATTGGAAAATGAAAGAACTTTGGACTACCACGATTCAAATGTGAAGAACTTATATGAAAAGGTAAGAAGCCAGTTAAAAAACAATGCCAAGGAAATTGGAAACGGCTGCTTTGAATTTTACCACAAATGCGATAACACGTGCATGGAAAGTGTCAAAAATGGGACTTATGACTACCCAAAATACTCAGAGGAAGCAAAATTAAACAGAGAAGAAATAGATGGGGTAAAGCTGGAATCAACAAGGATTTACCAGATTTTGGCGATCTATTCAACTGTCGCCAGTTCATTGGTACTGGTAGTCTCCCTGGGGGCAATCAGTTTCTGGATGTGCTCTAATGGGTCTCTACAGTGTAGAATATGTATTTAA");

            OverlapDeNovoAssembler assembler = new OverlapDeNovoAssembler();
            assembler.MergeThreshold = mergeThreshold;
            assembler.OverlapAlgorithm = new NeedlemanWunschAligner();
            ((IPairwiseSequenceAligner)assembler.OverlapAlgorithm).SimilarityMatrix = new DiagonalSimilarityMatrix(matchScore, mismatchScore);
            ((IPairwiseSequenceAligner)assembler.OverlapAlgorithm).GapOpenCost = gapCost;
            assembler.ConsensusResolver = new SimpleConsensusResolver(consensusThreshold);
            assembler.AssumeStandardOrientation = false;

            List<ISequence> inputs = new List<ISequence>();
            inputs.Add(seq1);
            inputs.Add(seq2);

            IOverlapDeNovoAssembly seqAssembly = (IOverlapDeNovoAssembly)assembler.Assemble(inputs);
            Contig contig0 = seqAssembly.Contigs[0];
            string actualString = contig0.ToString();
            string expectedString = "ACAAAAGCAACAAAAATGAAGGCAATACTAGTAGTTCTGCTATATACATTTGCAACCGCAAATG... +[1678]";
            Assert.AreEqual(actualString, expectedString); 
        }

        /// <summary>
        /// Test for MatePair ToString() Function.
        /// </summary>
        [TestMethod]
        public void TestMatePairToString()
        {
            MatePair p = new MatePair("2K");
            p.ForwardReadID = "F";
            p.ReverseReadID = "R";
            string actualString = p.ToString();
            string expectedString = "ForwardReadID=F, ReverseReadID=R, MeanLength=2000, Standard Deviation=100";
            Assert.AreEqual(actualString, expectedString); 
        }

        /// <summary>
        /// Test for OverlapDenovoAssembly ToString() Function.
        /// </summary>
        [TestMethod]
        public void TestOverlapDenovoAssemblyToString()
        {
            int matchScore = 5;
            int mismatchScore = -4;
            int gapCost = -10;
            double mergeThreshold = 4;
            double consensusThreshold = 66;

            Sequence seq2 = new Sequence(Alphabets.DNA, "ACAAAAGCAACAAAAATGAAGGCAATACTAGTAGTTCTGCTATATACATTTGCAACCGCAAATGCAGACACATTATGTATAGGTTATCATGCGAACAATTCAACAGACACTGTAGACACAGTACTAGAAAAGAATGTAACAGTAACACACTCTGTTAACCTTCTAGAAGACAAGCATAACGGGAAACTATGCAAACTAAGAGGAGTAGCCCCATTGCATTTGGGTAAATGTAACATTGCTGGCTGGATCCTGGGAAATCCAGAGTGTGAATCACTCTCCACAGCAAGCTCATGGTCCTACATTGTGGAAACATCTAGTTCAGACAATGGAACGTGTTACCCAGGAGATTTCATCGATTATGAGGAGCTAAGAGAGCAATTGAGCTCAGTGTCATCATTTGAAAGGTTTGAGATATTCCCCAAGACAAGTTCATGGCCCAATCATGACTCGAACAAAGGTGTAACGGCAGCATGTCCTCATGCTGGAGCAAAAAGCTTCTACAAAAATTTAATATGGCTAGTTAAAAAAGGAAATTCATACCCAAAGCTCAGCAAATCCTACATTAATGATAAAGGGAAAGAAGTCCTCGTGCTATGGGGCATTCACCATCCATCTACTAGTGCTGACCAACAAAGTCTCTATCAGAATGCAGATGCATATGTTTTTGTGGGGTCATCAAGATATAGCAAGAAGTTCAAGCCGGAAATAGCAATAAGACCCAAAGTGAGGGATCAAGAAGGGAGAATGAACTATTACTGGACACTAGTAGAGCCGGGAGACAAAATAACATTCGAAGCAACTGGAAATCTAGTGGTACCGAGATATGCATTCGCAATGGAAAGAAATGCTGGATCTGGTATTATCATTTCAGATACACCAGTCCACGATTGCAATACAACTTGTCAGACACCCAAGGGTGCTATAAACACCAGCCTCCCATTTCAGAATATACATCCGATCACAATTGGAAAATGTCCAAAATATGTAAAAAGCACAAAATTGAGACTGGCCACAGGATTGAGGAATGTCCCGTCTATTCAATCTAGAGGCCTATTTGGGGCCATTGCCGGTTTCATTGAAGGGGGGTGGACAGGGATGGTAGATGGATGGTACGGTTATCACCATCAAAATGAGCAGGGGTCAGGATATGCAGCCGACCTGAAGAGCACACAGAATGCCATTGACGAGATTACTAACAAAGTAAATTCTGTTATTGAAAAGATGAATATACAGTTCACAGCAGTAGGTAAAGAGTTCAACCACCTGGAAAAAAGAATAGAGAATTTAAATAAAAAAGTTGATGATGGTTTCCTGGACATTTGGACTTACAATGCCGAACTGTTGGTTCTATTGGAAAATGAAAGAACTTTGGACTACCACGATTCGAATGTGAAGAACTTATATGAAAAGGTAAGAAGCCAGCTAAAAAACAATGCCAAGGAAATTGGAAACGGCTGCTTTGAATTTTACCACAAATGCGATAACACGTGCATGGAAAGTGTCAAAAATGGGACTTATGACTACCCAAAATACTCAGAGGAAGCAAAATTAAACAGAGAAGAAATAGATGGGGTAAAGCTGGAATCAACAAGGATTTACCAGATTTTGGCGATCTATTCAACTGTCGCCAGTTCATTGGTACTGGTAGTCTCCCTGGGGGCAATCAGTTTCTGGATGTGCTCTAATGGGTCTCTACAGTGTAGAATATGTATTTAACATTAGGATTTCAGAAGCATGAGAAA");
            Sequence seq1 = new Sequence(Alphabets.DNA, "ATGAAGGCAATACTAGTAGTTCTGCTATATACATTTGCAACCGCAAATGCAGACACATTATGTATAGGTTATCATGCGAACAATTCAACAGACACTGTAGACACAGTACTAGAAAAGAATGTAACAGTAACACACTCTGTTAACCTTCTAGAAGACAAGCATAACGGGAAACTATGCAAACTAAGAGGGGTAGCCCCATTGCATTTGGGTAAATGTAACATTGCTGGCTGGATCCTGGGAAATCCAGAGTGTGAATCACTCTCCACAGCAAGCTCATGGTCCTACATTGTGGAAACATCTAGTTCAGACAATGGAACGTGTTACCCAGGAGATTTCATCGATTATGAGGAGCTAAGAGAGCAATTGAGCTCAGTGTCATCATTTGAAAGGTTTGAGATATTCCCCAAGACAAGTTCATGGCCCAATCATGACTCGAACAAAGGTGTAACGGCAGCATGTCCTCATGCTGGAGCAAAAAGCTTCTACAAAAATTTAATATGGCTAGTTAAAAAAGGAAATTCATACCCAAAGCTCAGCAAATCCTACATTAATGATAAAGGGAAAGAAGTCCTCGTGCTATGGGGCATTCACCATCCATCTACTAGTGCTGACCAACAAAGTCTCTATCAGAATGCAGATGCATATGTTTTTGTGGGGACATCAAGATACAGCAAGAAGTTCAAGCCGGAAATAGCAATAAGACCCAAAGTGAGGGATCAAGAAGGGAGAATGAACTATTACTGGACACTAGTAGAGCCGGGAGACAAAATAACATTCGAAGCAACTGGAAATCTAGTGGTACCGAGATATGCATTCGCAATGGAAAGAAATGCTGGATCTGGTATTATCATTTCAGATACACCAGTCCACGATTGCAATACAACTTGTCAGACACCCAAGGGTGCTATAAACACCAGCCTCCCATTTCAGAATATACATCCGATCACAATTGGAAAATGTCCAAAATATGTAAAAAGCACAAAATTGAGACTGGCCACAGGATTGAGGAATGTCCCGTCTATTCAATCTAGAGGCCTATTTGGGGCCATTGCCGGTTTCATTGAAGGGGGGTGGACAGGGATGGTAGATGGATGGTACGGTTATCACCATCAAAATGAGCAGGGGTCAGGATATGCAGCCGACCTGAAGAGCACACAGAATGCCATTGACGAGATTACTAACAAAGTAAATTCTGTTATTGAAAAGATGAATACACAGTTCACAGCAGTAGGTAAAGAGTTCAACCACCTGGAAAAAAGAATAGAGAATTTAAATAAAAAAATTGATGATGGTTTCCTGGACATTTGGACTTACAATGCCGAACTGTTGGTTCTATTGGAAAATGAAAGAACTTTGGACTACCACGATTCAAATGTGAAGAACTTATATGAAAAGGTAAGAAGCCAGTTAAAAAACAATGCCAAGGAAATTGGAAACGGCTGCTTTGAATTTTACCACAAATGCGATAACACGTGCATGGAAAGTGTCAAAAATGGGACTTATGACTACCCAAAATACTCAGAGGAAGCAAAATTAAACAGAGAAGAAATAGATGGGGTAAAGCTGGAATCAACAAGGATTTACCAGATTTTGGCGATCTATTCAACTGTCGCCAGTTCATTGGTACTGGTAGTCTCCCTGGGGGCAATCAGTTTCTGGATGTGCTCTAATGGGTCTCTACAGTGTAGAATATGTATTTAA");

            OverlapDeNovoAssembler assembler = new OverlapDeNovoAssembler();
            assembler.MergeThreshold = mergeThreshold;
            assembler.OverlapAlgorithm = new NeedlemanWunschAligner();
            ((IPairwiseSequenceAligner)assembler.OverlapAlgorithm).SimilarityMatrix = new DiagonalSimilarityMatrix(matchScore, mismatchScore);
            ((IPairwiseSequenceAligner)assembler.OverlapAlgorithm).GapOpenCost = gapCost;
            assembler.ConsensusResolver = new SimpleConsensusResolver(consensusThreshold);
            assembler.AssumeStandardOrientation = false;

            List<ISequence> inputs = new List<ISequence>();
            inputs.Add(seq1);
            inputs.Add(seq2);

            IOverlapDeNovoAssembly seqAssembly = (IOverlapDeNovoAssembly)assembler.Assemble(inputs);

            Assert.AreEqual(0, seqAssembly.UnmergedSequences.Count);
            Assert.AreEqual(1, seqAssembly.Contigs.Count);
            assembler.OverlapAlgorithm = new SmithWatermanAligner();
            seqAssembly = (OverlapDeNovoAssembly)assembler.Assemble(inputs);

            string actualString = seqAssembly.ToString();
            string expectedString = "ACAAAAGCAACAAAAATGAAGGCAATACTAGTAGTTCTGCTATATACATTTGCAACCGCAAATG... +[1678]\r\n";
            Assert.AreEqual(actualString, expectedString);
        }

        /// <summary>
        /// Test for PadenaAssembly ToString() Function.
        /// </summary>
        [TestMethod]
        public void TestPadenaAssemblyToString()
        {
            ISequence seq2 = new Sequence(Alphabets.DNA, "ACAAAAGCAAC");
            ISequence seq1 = new Sequence(Alphabets.DNA, "ATGAAGGCAATACTAGTAGT");
            IList<ISequence> contigList = new List<ISequence>();
            contigList.Add(seq1);
            contigList.Add(seq2);
            PadenaAssembly denovoAssembly = new PadenaAssembly();
            denovoAssembly.AddContigs(contigList);

            string actualString = denovoAssembly.ToString();
            string expectedString = "ATGAAGGCAATACTAGTAGT\r\nACAAAAGCAAC\r\n";
            Assert.AreEqual(actualString, expectedString); 
        }

    }
}
