using System;
using System.Collections.Generic;
using Bio.Extensions;
using NUnit.Framework;
using Bio.Util;
using System.Globalization;
using Bio.Algorithms.Alignment;
using Bio.Algorithms.SuffixTree;
using Bio.Algorithms.Assembly;
using Bio.SimilarityMatrices;

namespace Bio.Tests
{
    /// <summary>
    /// Summary description for TestToStringForBio
    /// </summary>
    [TestFixture]
    public class TestToStringForBio
    {
        /// <summary>
        /// Test for Sequence ConvertToString method.
        /// </summary>
        [Test]
        public void TestSequenceConvertToString()
        {
            ISequence seq = new Sequence(Alphabets.DNA, "ATCGATCGGATCGATCGGCTACTAATATCGATCGGCTACGATCGGCTAATCGATCGATCGGCTAATCGATCGATCGGCTAGCTA");
            const string expectedValue = "TCGATCGGCT";
            string actualValue = seq.ConvertToString(10, 10);
            Assert.AreEqual(expectedValue, actualValue);
        }

        /// <summary>
        /// Test for Sequence ConvertToString method.
        /// </summary>
        [Test]
        public void TestDerivedSequenceConvertToString()
        {
            Sequence baseSeq = new Sequence(Alphabets.DNA, "ATCGATCGGATCGATCGGCTACTAATATCGATCGGCTACGATCGGCTAATCGATCGATCGGCTAATCGATCGATCGGCTAGCTA");
            DerivedSequence seq = new DerivedSequence(baseSeq, false, false);
            const string expectedValue = "TCGATCGGCT";
            string actualValue = seq.ConvertToString(10, 10);
            Assert.AreEqual(expectedValue, actualValue);
        }

        /// <summary>
        /// Test for Sequence ConvertToString method.
        /// </summary>
        [Test]
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
        [Test]
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
        [Test]
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
        [Test]
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
            string actualString = seq.ToString().Replace("\r\n", Environment.NewLine);
            string expectedString = "ATCG\r\nAAAB".Replace("\r\n", Environment.NewLine);
            Assert.AreEqual(actualString, expectedString);
        }

        /// <summary>
        /// Test for All Alphabets ToString() Function.
        /// </summary>
        [Test]
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
        [Test]
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
        [Test]
        public void TestSequenceRangeGroupingToString()
        {
            ISequenceRange range1 = new SequenceRange("chr20", 0, 3);
            ISequenceRange range2 = new SequenceRange("chr21", 0, 4);
            IList<ISequenceRange> ranges = new List<ISequenceRange>();
            ranges.Add(range1);
            ranges.Add(range2);
            
            SequenceRangeGrouping rangegrouping = new SequenceRangeGrouping(ranges);
            string actualString = rangegrouping.ToString();

            string expectedString = "ID=chr20 Start=0 End=3\r\nID=chr21 Start=0 End=4\r\n".Replace("\r\n", Environment.NewLine);
            Assert.AreEqual(actualString, expectedString);
        }

         /// <summary>
        /// Test for SequenceStatistics ToString() Function.
        /// </summary>
        [Test]
        public void TestSequenceStatisticsToString()
        {
            ISequence seq = new Sequence(Alphabets.DNA, "ATCGATCG");
            SequenceStatistics seqStats = new SequenceStatistics(seq);
            string actualString = seqStats.ToString();
            string expectedString = "A - 2\r\nC - 2\r\nG - 2\r\nT - 2\r\n".Replace("\r\n", Environment.NewLine);
            Assert.AreEqual(actualString, expectedString);
        }

        /// <summary>
        /// Test for AlignedSequence ToString() Function.
        /// </summary>
        [Test]
        public void TestAlignedSequenceToString()
        {
            var seqList = new List<ISequence>
            {
                new Sequence(Alphabets.DNA,"CAAAAGGGATTGC---TGTTGGAGTGAATGCCATTACCTACCGGCTAGGAGGAGTAGTACAAAGGAGC"),
                new Sequence(Alphabets.DNA, "CAAAAGGGATTGC---"),
                new Sequence(Alphabets.DNA, "TAGTAGTTCTGCTATATACATTTG"),
                new Sequence(Alphabets.DNA, "GTTATCATGCGAACAATTCAACAGACACTGTAGA")
            };

            NucmerPairwiseAligner num = new NucmerPairwiseAligner
            {
                BreakLength = 8,
                FixedSeparation = 0,
                MinimumScore = 0,
                MaximumSeparation = 0,
                SeparationFactor = 0,
                LengthOfMUM = 8
            };

            IList<ISequence> sequenceList = seqList;
            IList<ISequenceAlignment> alignmentObj = num.Align(sequenceList);
            Assert.AreNotEqual(0, alignmentObj.Count);

            var alignedSeqs = (AlignedSequence) alignmentObj[0].AlignedSequences[0];

            string actualString = alignedSeqs.ToString();
            string ExpectedString = "CAAAAGGGATTGC---\r\nCAAAAGGGATTGC---\r\nCAAAAGGGATTGC---\r\n".Replace("\r\n", Environment.NewLine);
            Assert.AreEqual(ExpectedString, actualString);
        }

        /// <summary>
        /// Test for Cluster ToString() Function.
        /// </summary>
        [Test]
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
            string expectedString = "RefStart=0 QueryStart=0 Length=20 Score=0 WrapScore=0 IsGood=False\r\nRefStart=0 QueryStart=0 Length=30 Score=0 WrapScore=0 IsGood=False\r\n".Replace ("\r\n", Environment.NewLine);
            Assert.AreEqual(actualString, expectedString);
        }

        /// <summary>
        /// Test for DeltaAlignment ToString() Function.
        /// </summary>
        [Test]
        public void TestDeltaAlignmentToString()
        {
            ISequence refSeq = new Sequence(Alphabets.DNA, "ATCGGGGGGGGAAAAAAATTTTCCCCGGGGG");
            ISequence qrySeq = new Sequence(Alphabets.DNA, "GGGGG");
            DeltaAlignment delta = new DeltaAlignment(refSeq, qrySeq);
            delta.FirstSequenceEnd = 21;
            delta.SecondSequenceEnd = 20;
            string actualString = delta.ToString();
            string expectedString = "Ref ID= Query Id= Ref start=0 Ref End=21 Query start=0 Query End=20, Direction=FORWARD";
            Assert.AreEqual(actualString, expectedString);
        }

         /// <summary>
        /// Test for PairwiseAlignedSequence ToString() Function.
        /// </summary>
        [Test]
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
            string expectedString = "AWGHE\r\nAWGHE\r\nAW-HE\r\n".Replace("\r\n", Environment.NewLine);
            Assert.AreEqual(actualString, expectedString);
        }

        /// <summary>
        /// Test for PairwiseSequenceAlignment ToString() Function.
        /// </summary>
        [Test]
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
            string expectedString = "AWGHE\r\nAWGHE\r\nAW-HE\r\n\r\n".Replace("\r\n", Environment.NewLine);
            Assert.AreEqual(actualString, expectedString); 
        }

         /// <summary>
        /// Test for Match And MatchExtension ToString() Function.
        /// </summary>
        [Test]
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
        [Test]
        public void TestSequenceAlignmentToString()
        {
            ISequenceAligner aligner = SequenceAligners.NeedlemanWunsch;
            IAlphabet alphabet = Alphabets.Protein;
            const string origSequence1 = "KRIPKSQNLRSIHSIFPFLEDKLSHLN";
            const string origSequence2 = "LNIPSLITLNKSIYVFSKRKKRLSGFLHN";

            // Create input sequences
            var inputSequences = new List<ISequence>
                {
                    new Sequence(alphabet, origSequence1),
                    new Sequence(alphabet, origSequence2)
                };

            // Get aligned sequences
            IList<ISequenceAlignment> alignments = aligner.Align(inputSequences);
            ISequenceAlignment alignment = new SequenceAlignment();
            foreach (var alignedSequence in alignments[0].AlignedSequences)
                alignment.AlignedSequences.Add(alignedSequence);

            const string expected = "XXIPXXXXLXXXXXXFXXXXXXLSXXLHN\r\n" +
                                    "KRIPKSQNLRSIHSIFPFLEDKLSHL--N\r\n" +
                                    "LNIPSLITLNKSIYVFSKRKKRLSGFLHN\r\n\r\n";
            Assert.AreEqual(expected.Replace("\r\n", Environment.NewLine), alignment.ToString());
        }

        /// <summary>
        /// Test for Contig ToString() Function.
        /// </summary>
        [Test]
        public void TestContigToString()
        {
            // test parameters
            const int matchScore = 5;
            const int mismatchScore = -4;
            const int gapCost = -10;
            const double mergeThreshold = 4;
            const double consensusThreshold = 66;

            Sequence seq2 = new Sequence(Alphabets.DNA, "ACAAAAGCAACAAAAATGAAGGCAATACTAGTAGTTCTGCTATATACATTTGCAACCGCAAATGCAGACACATTATGTATAGGTTATCATGCGAACAATTCAACAGACACTGTAGACACAGTACTAGAAAAGAATGTAACAGTAACACACTCTGTTAACCTTCTAGAAGACAAGCATAACGGGAAACTATGCAAACTAAGAGGAGTAGCCCCATTGCATTTGGGTAAATGTAACATTGCTGGCTGGATCCTGGGAAATCCAGAGTGTGAATCACTCTCCACAGCAAGCTCATGGTCCTACATTGTGGAAACATCTAGTTCAGACAATGGAACGTGTTACCCAGGAGATTTCATCGATTATGAGGAGCTAAGAGAGCAATTGAGCTCAGTGTCATCATTTGAAAGGTTTGAGATATTCCCCAAGACAAGTTCATGGCCCAATCATGACTCGAACAAAGGTGTAACGGCAGCATGTCCTCATGCTGGAGCAAAAAGCTTCTACAAAAATTTAATATGGCTAGTTAAAAAAGGAAATTCATACCCAAAGCTCAGCAAATCCTACATTAATGATAAAGGGAAAGAAGTCCTCGTGCTATGGGGCATTCACCATCCATCTACTAGTGCTGACCAACAAAGTCTCTATCAGAATGCAGATGCATATGTTTTTGTGGGGTCATCAAGATATAGCAAGAAGTTCAAGCCGGAAATAGCAATAAGACCCAAAGTGAGGGATCAAGAAGGGAGAATGAACTATTACTGGACACTAGTAGAGCCGGGAGACAAAATAACATTCGAAGCAACTGGAAATCTAGTGGTACCGAGATATGCATTCGCAATGGAAAGAAATGCTGGATCTGGTATTATCATTTCAGATACACCAGTCCACGATTGCAATACAACTTGTCAGACACCCAAGGGTGCTATAAACACCAGCCTCCCATTTCAGAATATACATCCGATCACAATTGGAAAATGTCCAAAATATGTAAAAAGCACAAAATTGAGACTGGCCACAGGATTGAGGAATGTCCCGTCTATTCAATCTAGAGGCCTATTTGGGGCCATTGCCGGTTTCATTGAAGGGGGGTGGACAGGGATGGTAGATGGATGGTACGGTTATCACCATCAAAATGAGCAGGGGTCAGGATATGCAGCCGACCTGAAGAGCACACAGAATGCCATTGACGAGATTACTAACAAAGTAAATTCTGTTATTGAAAAGATGAATATACAGTTCACAGCAGTAGGTAAAGAGTTCAACCACCTGGAAAAAAGAATAGAGAATTTAAATAAAAAAGTTGATGATGGTTTCCTGGACATTTGGACTTACAATGCCGAACTGTTGGTTCTATTGGAAAATGAAAGAACTTTGGACTACCACGATTCGAATGTGAAGAACTTATATGAAAAGGTAAGAAGCCAGCTAAAAAACAATGCCAAGGAAATTGGAAACGGCTGCTTTGAATTTTACCACAAATGCGATAACACGTGCATGGAAAGTGTCAAAAATGGGACTTATGACTACCCAAAATACTCAGAGGAAGCAAAATTAAACAGAGAAGAAATAGATGGGGTAAAGCTGGAATCAACAAGGATTTACCAGATTTTGGCGATCTATTCAACTGTCGCCAGTTCATTGGTACTGGTAGTCTCCCTGGGGGCAATCAGTTTCTGGATGTGCTCTAATGGGTCTCTACAGTGTAGAATATGTATTTAACATTAGGATTTCAGAAGCATGAGAAA");
            Sequence seq1 = new Sequence(Alphabets.DNA, "ATGAAGGCAATACTAGTAGTTCTGCTATATACATTTGCAACCGCAAATGCAGACACATTATGTATAGGTTATCATGCGAACAATTCAACAGACACTGTAGACACAGTACTAGAAAAGAATGTAACAGTAACACACTCTGTTAACCTTCTAGAAGACAAGCATAACGGGAAACTATGCAAACTAAGAGGGGTAGCCCCATTGCATTTGGGTAAATGTAACATTGCTGGCTGGATCCTGGGAAATCCAGAGTGTGAATCACTCTCCACAGCAAGCTCATGGTCCTACATTGTGGAAACATCTAGTTCAGACAATGGAACGTGTTACCCAGGAGATTTCATCGATTATGAGGAGCTAAGAGAGCAATTGAGCTCAGTGTCATCATTTGAAAGGTTTGAGATATTCCCCAAGACAAGTTCATGGCCCAATCATGACTCGAACAAAGGTGTAACGGCAGCATGTCCTCATGCTGGAGCAAAAAGCTTCTACAAAAATTTAATATGGCTAGTTAAAAAAGGAAATTCATACCCAAAGCTCAGCAAATCCTACATTAATGATAAAGGGAAAGAAGTCCTCGTGCTATGGGGCATTCACCATCCATCTACTAGTGCTGACCAACAAAGTCTCTATCAGAATGCAGATGCATATGTTTTTGTGGGGACATCAAGATACAGCAAGAAGTTCAAGCCGGAAATAGCAATAAGACCCAAAGTGAGGGATCAAGAAGGGAGAATGAACTATTACTGGACACTAGTAGAGCCGGGAGACAAAATAACATTCGAAGCAACTGGAAATCTAGTGGTACCGAGATATGCATTCGCAATGGAAAGAAATGCTGGATCTGGTATTATCATTTCAGATACACCAGTCCACGATTGCAATACAACTTGTCAGACACCCAAGGGTGCTATAAACACCAGCCTCCCATTTCAGAATATACATCCGATCACAATTGGAAAATGTCCAAAATATGTAAAAAGCACAAAATTGAGACTGGCCACAGGATTGAGGAATGTCCCGTCTATTCAATCTAGAGGCCTATTTGGGGCCATTGCCGGTTTCATTGAAGGGGGGTGGACAGGGATGGTAGATGGATGGTACGGTTATCACCATCAAAATGAGCAGGGGTCAGGATATGCAGCCGACCTGAAGAGCACACAGAATGCCATTGACGAGATTACTAACAAAGTAAATTCTGTTATTGAAAAGATGAATACACAGTTCACAGCAGTAGGTAAAGAGTTCAACCACCTGGAAAAAAGAATAGAGAATTTAAATAAAAAAATTGATGATGGTTTCCTGGACATTTGGACTTACAATGCCGAACTGTTGGTTCTATTGGAAAATGAAAGAACTTTGGACTACCACGATTCAAATGTGAAGAACTTATATGAAAAGGTAAGAAGCCAGTTAAAAAACAATGCCAAGGAAATTGGAAACGGCTGCTTTGAATTTTACCACAAATGCGATAACACGTGCATGGAAAGTGTCAAAAATGGGACTTATGACTACCCAAAATACTCAGAGGAAGCAAAATTAAACAGAGAAGAAATAGATGGGGTAAAGCTGGAATCAACAAGGATTTACCAGATTTTGGCGATCTATTCAACTGTCGCCAGTTCATTGGTACTGGTAGTCTCCCTGGGGGCAATCAGTTTCTGGATGTGCTCTAATGGGTCTCTACAGTGTAGAATATGTATTTAA");

            OverlapDeNovoAssembler assembler = new OverlapDeNovoAssembler
            {
                MergeThreshold = mergeThreshold,
                OverlapAlgorithm = new NeedlemanWunschAligner 
                {
                    SimilarityMatrix = new DiagonalSimilarityMatrix(matchScore, mismatchScore),
                    GapOpenCost = gapCost
                },
                ConsensusResolver = new SimpleConsensusResolver(consensusThreshold),
                AssumeStandardOrientation = false,
            };

            IOverlapDeNovoAssembly seqAssembly = (IOverlapDeNovoAssembly) assembler.Assemble(new List<ISequence> {seq1, seq2});
            Contig contig0 = seqAssembly.Contigs[0];
            string actualString = contig0.ToString();
            //string expectedString = "ACAAAAGCAACAAAAATGAAGGCAATACTAGTAGTTCTGCTATATACATTTGCAACCGCAAATG... +[1678]";
            string expectedString = "AYRAARGCAAYAMWARTRRWKSYRMTAYWWRYAKTTSYRMYMKMWAMWKYWGMMACMKYAWRTR... +[1678]";
            Assert.AreEqual(actualString, expectedString); 
        }

        /// <summary>
        /// Test for MatePair ToString() Function.
        /// </summary>
        [Test]
        public void TestMatePairToString()
        {
            MatePair p = new MatePair("2K") {ForwardReadID = "F", ReverseReadID = "R"};
            string actualString = p.ToString();
            const string expectedString = "ForwardReadID=F, ReverseReadID=R, MeanLength=2000, Standard Deviation=100";
            Assert.AreEqual(actualString, expectedString); 
        }

        /// <summary>
        /// Test for OverlapDenovoAssembly ToString() Function.
        /// </summary>
        [Test]
        public void TestOverlapDenovoAssemblyToString()
        {
            const int matchScore = 5;
            const int mismatchScore = -4;
            const int gapCost = -10;
            const double mergeThreshold = 4;
            const double consensusThreshold = 66;

            ISequence seq2 = new Sequence(Alphabets.DNA, "ACAAAAGCAACAAAAATGAAGGCAATACTAGTAGTTCTGCTATATACATTTGCAACCGCAAATGCAGACACATTATGTATAGGTTATCATGCGAACAATTCAACAGACACTGTAGACACAGTACTAGAAAAGAATGTAACAGTAACACACTCTGTTAACCTTCTAGAAGACAAGCATAACGGGAAACTATGCAAACTAAGAGGAGTAGCCCCATTGCATTTGGGTAAATGTAACATTGCTGGCTGGATCCTGGGAAATCCAGAGTGTGAATCACTCTCCACAGCAAGCTCATGGTCCTACATTGTGGAAACATCTAGTTCAGACAATGGAACGTGTTACCCAGGAGATTTCATCGATTATGAGGAGCTAAGAGAGCAATTGAGCTCAGTGTCATCATTTGAAAGGTTTGAGATATTCCCCAAGACAAGTTCATGGCCCAATCATGACTCGAACAAAGGTGTAACGGCAGCATGTCCTCATGCTGGAGCAAAAAGCTTCTACAAAAATTTAATATGGCTAGTTAAAAAAGGAAATTCATACCCAAAGCTCAGCAAATCCTACATTAATGATAAAGGGAAAGAAGTCCTCGTGCTATGGGGCATTCACCATCCATCTACTAGTGCTGACCAACAAAGTCTCTATCAGAATGCAGATGCATATGTTTTTGTGGGGTCATCAAGATATAGCAAGAAGTTCAAGCCGGAAATAGCAATAAGACCCAAAGTGAGGGATCAAGAAGGGAGAATGAACTATTACTGGACACTAGTAGAGCCGGGAGACAAAATAACATTCGAAGCAACTGGAAATCTAGTGGTACCGAGATATGCATTCGCAATGGAAAGAAATGCTGGATCTGGTATTATCATTTCAGATACACCAGTCCACGATTGCAATACAACTTGTCAGACACCCAAGGGTGCTATAAACACCAGCCTCCCATTTCAGAATATACATCCGATCACAATTGGAAAATGTCCAAAATATGTAAAAAGCACAAAATTGAGACTGGCCACAGGATTGAGGAATGTCCCGTCTATTCAATCTAGAGGCCTATTTGGGGCCATTGCCGGTTTCATTGAAGGGGGGTGGACAGGGATGGTAGATGGATGGTACGGTTATCACCATCAAAATGAGCAGGGGTCAGGATATGCAGCCGACCTGAAGAGCACACAGAATGCCATTGACGAGATTACTAACAAAGTAAATTCTGTTATTGAAAAGATGAATATACAGTTCACAGCAGTAGGTAAAGAGTTCAACCACCTGGAAAAAAGAATAGAGAATTTAAATAAAAAAGTTGATGATGGTTTCCTGGACATTTGGACTTACAATGCCGAACTGTTGGTTCTATTGGAAAATGAAAGAACTTTGGACTACCACGATTCGAATGTGAAGAACTTATATGAAAAGGTAAGAAGCCAGCTAAAAAACAATGCCAAGGAAATTGGAAACGGCTGCTTTGAATTTTACCACAAATGCGATAACACGTGCATGGAAAGTGTCAAAAATGGGACTTATGACTACCCAAAATACTCAGAGGAAGCAAAATTAAACAGAGAAGAAATAGATGGGGTAAAGCTGGAATCAACAAGGATTTACCAGATTTTGGCGATCTATTCAACTGTCGCCAGTTCATTGGTACTGGTAGTCTCCCTGGGGGCAATCAGTTTCTGGATGTGCTCTAATGGGTCTCTACAGTGTAGAATATGTATTTAACATTAGGATTTCAGAAGCATGAGAAA");
            ISequence seq1 = new Sequence(Alphabets.DNA, "ATGAAGGCAATACTAGTAGTTCTGCTATATACATTTGCAACCGCAAATGCAGACACATTATGTATAGGTTATCATGCGAACAATTCAACAGACACTGTAGACACAGTACTAGAAAAGAATGTAACAGTAACACACTCTGTTAACCTTCTAGAAGACAAGCATAACGGGAAACTATGCAAACTAAGAGGGGTAGCCCCATTGCATTTGGGTAAATGTAACATTGCTGGCTGGATCCTGGGAAATCCAGAGTGTGAATCACTCTCCACAGCAAGCTCATGGTCCTACATTGTGGAAACATCTAGTTCAGACAATGGAACGTGTTACCCAGGAGATTTCATCGATTATGAGGAGCTAAGAGAGCAATTGAGCTCAGTGTCATCATTTGAAAGGTTTGAGATATTCCCCAAGACAAGTTCATGGCCCAATCATGACTCGAACAAAGGTGTAACGGCAGCATGTCCTCATGCTGGAGCAAAAAGCTTCTACAAAAATTTAATATGGCTAGTTAAAAAAGGAAATTCATACCCAAAGCTCAGCAAATCCTACATTAATGATAAAGGGAAAGAAGTCCTCGTGCTATGGGGCATTCACCATCCATCTACTAGTGCTGACCAACAAAGTCTCTATCAGAATGCAGATGCATATGTTTTTGTGGGGACATCAAGATACAGCAAGAAGTTCAAGCCGGAAATAGCAATAAGACCCAAAGTGAGGGATCAAGAAGGGAGAATGAACTATTACTGGACACTAGTAGAGCCGGGAGACAAAATAACATTCGAAGCAACTGGAAATCTAGTGGTACCGAGATATGCATTCGCAATGGAAAGAAATGCTGGATCTGGTATTATCATTTCAGATACACCAGTCCACGATTGCAATACAACTTGTCAGACACCCAAGGGTGCTATAAACACCAGCCTCCCATTTCAGAATATACATCCGATCACAATTGGAAAATGTCCAAAATATGTAAAAAGCACAAAATTGAGACTGGCCACAGGATTGAGGAATGTCCCGTCTATTCAATCTAGAGGCCTATTTGGGGCCATTGCCGGTTTCATTGAAGGGGGGTGGACAGGGATGGTAGATGGATGGTACGGTTATCACCATCAAAATGAGCAGGGGTCAGGATATGCAGCCGACCTGAAGAGCACACAGAATGCCATTGACGAGATTACTAACAAAGTAAATTCTGTTATTGAAAAGATGAATACACAGTTCACAGCAGTAGGTAAAGAGTTCAACCACCTGGAAAAAAGAATAGAGAATTTAAATAAAAAAATTGATGATGGTTTCCTGGACATTTGGACTTACAATGCCGAACTGTTGGTTCTATTGGAAAATGAAAGAACTTTGGACTACCACGATTCAAATGTGAAGAACTTATATGAAAAGGTAAGAAGCCAGTTAAAAAACAATGCCAAGGAAATTGGAAACGGCTGCTTTGAATTTTACCACAAATGCGATAACACGTGCATGGAAAGTGTCAAAAATGGGACTTATGACTACCCAAAATACTCAGAGGAAGCAAAATTAAACAGAGAAGAAATAGATGGGGTAAAGCTGGAATCAACAAGGATTTACCAGATTTTGGCGATCTATTCAACTGTCGCCAGTTCATTGGTACTGGTAGTCTCCCTGGGGGCAATCAGTTTCTGGATGTGCTCTAATGGGTCTCTACAGTGTAGAATATGTATTTAA");

            IOverlapDeNovoAssembler assembler = new OverlapDeNovoAssembler
            {
                MergeThreshold = mergeThreshold,
                OverlapAlgorithm = new NeedlemanWunschAligner 
                { 
                    SimilarityMatrix = new DiagonalSimilarityMatrix(matchScore, mismatchScore),
                    GapOpenCost = gapCost
                },
                ConsensusResolver = new SimpleConsensusResolver(consensusThreshold),
                AssumeStandardOrientation = false
            };

            var inputs = new List<ISequence> {seq1, seq2};
            var seqAssembly = (IOverlapDeNovoAssembly) assembler.Assemble(inputs);

            Assert.AreEqual(0, seqAssembly.UnmergedSequences.Count);
            Assert.AreEqual(1, seqAssembly.Contigs.Count);
            
            assembler.OverlapAlgorithm = new SmithWatermanAligner();
            seqAssembly = (IOverlapDeNovoAssembly) assembler.Assemble(inputs);

            string expectedString = "ACAAAAGCAACAAAAATGAAGGCAATACTAGTAGTTCTGCTATATACATTTGCAACCGCAAATG... +[1678]\r\n".Replace("\r\n",Environment.NewLine);
            string actualString = seqAssembly.ToString();
            Assert.AreEqual(expectedString, actualString);
        }
    }
}
