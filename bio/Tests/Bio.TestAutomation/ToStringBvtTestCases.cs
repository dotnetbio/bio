/****************************************************************************
 * ToStringBvtTestCases.cs
 * 
 * This file contains the ToString BVT test cases for all classes.
 * 
******************************************************************************/
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
using Bio.TestAutomation.Util;
using System.IO;
using Bio.IO.FastA;

namespace Bio.TestAutomation
{
    /// <summary>
    /// Bvt Test cases for ToString and ConvertToString of all classes
    /// </summary>
    [TestClass]
    public class ToStringBvtTestCases
    {

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

        #region BVT Test Cases


        /// <summary>
        /// Validates Sequence ToString()
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSequenceToString()
        {
            ISequence seqSmall = new Sequence(Alphabets.DNA, "ATCG");
            string seqLargeString = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.seqLargeStringNode);
            ISequence seqLarge = new Sequence(Alphabets.DNA, seqLargeString);
            string ActualSmallString = seqSmall.ToString();
            string ActualLargeString = seqLarge.ToString();
            string ExpectedSmallString = "ATCG";
            string seqLargeExpected = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.seqLargeExpected2Node);
            string expectedLargeString = string.Format(CultureInfo.CurrentCulture,
                seqLargeExpected, (seqLarge.Count - Helper.AlphabetsToShowInToString));
            Assert.AreEqual(ExpectedSmallString, ActualSmallString);
            Assert.AreEqual(expectedLargeString, ActualLargeString);

            //check with blank sequence
            Sequence seqBlank = new Sequence(Alphabets.DNA, "");
            string blankString = seqBlank.ToString();
            Assert.AreEqual(string.Empty, blankString);

            //read sequence from file
            List<ISequence> seqsList;
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(Constants.SimpleFastaNodeName,
                Constants.FilePathNode);
            using (StreamReader reader = new StreamReader(filePath))
            {
                IEnumerable<ISequence> seq = null;

                using (FastAParser parser = new FastAParser())
                {
                    parser.Alphabet = Alphabets.Protein;
                    seq = parser.Parse(reader);

                    //Create a list of sequences.
                    seqsList = seq.ToList();
                }
            }

            string seqString = new string(seqsList[0].Select(a => (char)a).ToArray());
            if (seqString.Length > Helper.AlphabetsToShowInToString)
            {
                //check if the whole sequence string contains the string retrieved from ToString
                Assert.IsTrue(seqString.Contains(seqsList[0].ToString().Substring(0, Helper.AlphabetsToShowInToString)));
                Assert.IsTrue(seqsList[0].ToString().Contains("... +["));
            }
            else
            {
                Assert.AreEqual(seqString, seqsList[0].ToString());
            }
        }


        /// <summary>
        /// Validates DerivedSequence ToString()
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateDerivedSequenceToString()
        {
            ISequence seqSmall = new Sequence(Alphabets.DNA, "ATCG");
            string seqLargeStr = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.seqLargeStringNode);
            ISequence seqLarge = new Sequence(Alphabets.DNA, seqLargeStr);
            ISequence DeriveSeqSmall = new DerivedSequence(seqSmall, false, true);
            ISequence DeriveSeqLarge = new DerivedSequence(seqLarge, false, true);

            string ActualSmallString = DeriveSeqSmall.ToString();
            string ActualLargeString = DeriveSeqLarge.ToString();
            string ExpectedSmallString = "TAGC";
            string seqLargeExpected = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.seqLargeExpectedNode);
            string expectedLargeString = string.Format(CultureInfo.CurrentCulture,
                seqLargeExpected, (seqLarge.Count - Helper.AlphabetsToShowInToString));

            Assert.AreEqual(ExpectedSmallString, ActualSmallString);
            Assert.AreEqual(expectedLargeString, ActualLargeString);

            //read sequences from file
            // Get input and expected values from xml
            string expectedSequence = utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.AlphabetNameNode);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            // Create derived Sequence
            ISequence seq = new Sequence(alphabet, expectedSequence);
            DerivedSequence derSequence = new DerivedSequence(seq, false, false);

            string actualDerivedSeqStr = derSequence.ToString();
            if (actualDerivedSeqStr.Length > Helper.AlphabetsToShowInToString)
            {
                //check if the whole sequence string contains the string retrieved from ToString
                Assert.IsTrue(expectedSequence.Contains(derSequence.ToString().Substring(0, Helper.AlphabetsToShowInToString)));
                Assert.IsTrue(derSequence.ToString().Contains("... +["));
            }
            else
            {
                Assert.AreEqual(expectedSequence, derSequence.ToString());
            }
        }

        /// <summary>
        /// Validates QualitativeSequence ToString
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateQualitativeSequenceToString()
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
            QualitativeSequence seq = new QualitativeSequence(Alphabets.DNA,
                FastQFormatType.Illumina, seqData, qualityScores);
            string actualString = seq.ToString();
            string expectedString = "ATCG\r\nAAAB";
            Assert.AreEqual(actualString, expectedString);

        }

        /// <summary>
        /// Validates All Alphabets ToString
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateAllAlphabetsToString()
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

            Assert.AreEqual("ACGT-", dnaStringActual);
            Assert.AreEqual("ACGU-", rnaStringActual);
            Assert.AreEqual("ACDEFGHIKLMNOPQRSTUVWY-*", proteinStringActual);
            Assert.AreEqual("ACGT-MRSWYKVHDBN", dnaAmbiguousStringActual);
            Assert.AreEqual("ACGU-NMRSWYKVHDB", rnaAmbiguousStringActual);
            Assert.AreEqual("ACDEFGHIKLMNOPQRSTUVWY-*XZBJ", proteinAmbiguousStringActual);
        }

        /// <summary>
        /// Validates SequenceRange ToString
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSequenceRangeToString()
        {
            SequenceRange range = new SequenceRange("chr20", 0, 3);
            string actualString = range.ToString();
            Assert.AreEqual("ID=chr20 Start=0 End=3", actualString);
        }

        /// <summary>
        /// Validates SequenceRangeGrouping ToString
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSequenceRangeGroupingToString()
        {
            ISequenceRange range1 = new SequenceRange("chr20", 0, 3);
            ISequenceRange range2 = new SequenceRange("chr21", 0, 4);
            IList<ISequenceRange> ranges = new List<ISequenceRange>();
            ranges.Add(range1);
            ranges.Add(range2);

            SequenceRangeGrouping rangegrouping = new SequenceRangeGrouping(ranges);
            string actualString = rangegrouping.ToString();
            string expectedStr = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.SequenceRangeGroupingExpectedNode);
            Assert.AreEqual(expectedStr.Replace("\\r\\n", ""), actualString.Replace("\r\n", ""));
        }

        /// <summary>
        /// Validates SequenceStatistics ToString
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSequenceStatisticsToString()
        {
            ISequence seq = new Sequence(Alphabets.DNA, "ATCGATCG");
            SequenceStatistics seqStats = new SequenceStatistics(seq);
            string actualString = seqStats.ToString();
            string expectedString = "A - 2\r\nC - 2\r\nG - 2\r\nT - 2\r\n";
            Assert.AreEqual(actualString, expectedString);

            // Gets the expected sequence from the Xml
            List<ISequence> seqsList;
            IEnumerable<ISequence> sequences = null;
            string filePath = utilityObj.xmlUtil.GetTextValue(Constants.SimpleFastaNodeName,
                Constants.FilePathNode);
            using (StreamReader reader = new StreamReader(filePath))
            {
                using (FastAParser parser = new FastAParser())
                {
                    parser.Alphabet = Alphabets.Protein;
                    sequences = parser.Parse(reader);

                    //Create a list of sequences.
                    seqsList = sequences.ToList();
                }
            }

            foreach (ISequence Sequence in seqsList)
            {
                seqStats = new SequenceStatistics(Sequence);
                string seqStatStr = seqStats.ToString();
                Assert.IsTrue(seqStatStr.Contains(" - "));
            }

        }

        /// <summary>
        /// Validates AlignedSequence ToString
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateAlignedSequenceToString()
        {
            IList<ISequence> seqList = new List<ISequence>();
            string actualAlignedSeqString = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.AlignedSeqActualNode);
            seqList.Add(new Sequence(Alphabets.DNA,
                actualAlignedSeqString));
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
            string expectedString = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.AlignedSeqExpectedNode);
            Assert.AreEqual(actualString.Replace("\r\n", ""), expectedString.Replace("\\r\\n", ""));
        }

        /// <summary>
        /// Validates Cluster ToString
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateClusterToString()
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
            string expectedString = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.ClusterExpectedNode);
            Assert.AreEqual(actualString.Replace("\r\n", ""), expectedString.Replace("\\r\\n", ""));
        }

        /// <summary>
        /// Validates DeltaAlignment ToString
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateDeltaAlignmentToString()
        {
            ISequence refSeq = new Sequence(Alphabets.DNA, "ATCGGGGGGGGAAAAAAATTTTCCCCGGGGG");
            ISequence qrySeq = new Sequence(Alphabets.DNA, "GGGGG");
            DeltaAlignment delta = new DeltaAlignment(refSeq, qrySeq);
            delta.FirstSequenceEnd = 21;
            delta.SecondSequenceEnd = 20;
            string actualString = delta.ToString();
            string expectedString = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.DeltaAlignmentExpectedNode);
            Assert.AreEqual(actualString, expectedString);

            // Gets the expected sequence from the Xml
            List<ISequence> seqsList;
            IEnumerable<ISequence> sequences = null;
            string filePath = utilityObj.xmlUtil.GetTextValue(Constants.SimpleFastaNodeName,
                Constants.FilePathNode);
            using (StreamReader reader = new StreamReader(filePath))
            {
                using (FastAParser parser = new FastAParser())
                {
                    parser.Alphabet = Alphabets.Protein;
                    sequences = parser.Parse(reader);

                    //Create a list of sequences.
                    seqsList = sequences.ToList();
                }
            }

            DeltaAlignment delta1 = new DeltaAlignment(seqsList[0], qrySeq);
            delta1.FirstSequenceEnd = 21;
            delta1.SecondSequenceEnd = 20;
            string actualString1 = delta1.ToString();
            string expectedString1 = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.DeltaAlignmentExpected2Node);
            Assert.AreEqual(expectedString1, actualString1);
        }

        /// <summary>
        /// Validates PairwiseAlignedSequence ToString
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePairwiseAlignedSequenceToString()
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
        /// Validates PairwiseSequenceAlignment ToString
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePairwiseSequenceAlignmentToString()
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
        /// Validates Match And MatchExtension ToString
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMatchAndMatchExtensionToString()
        {
            Match match = new Match();
            match.Length = 20;
            match.QuerySequenceOffset = 33;

            MatchExtension matchExtn = new MatchExtension(match);
            matchExtn.ID = 1;
            matchExtn.Length = 20;

            string actualMatchExtnString = matchExtn.ToString();
            string actualMatchstring = match.ToString();
            string ExpectedMatchExtnString = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.ExpectedMatchExtnStringNode);
            string ExpectedMatchString = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.ExpectedMatchStringNode);

            Assert.AreEqual(ExpectedMatchExtnString, actualMatchExtnString);
            Assert.AreEqual(actualMatchstring, ExpectedMatchString);
        }

        /// <summary>
        /// Validates SequenceAlignment ToString
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSequenceAlignmentToString()
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
            string ExpectedSequenceAlignmentString = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.SequenceAlignmentExpectedNode);

            Assert.AreEqual(ExpectedSequenceAlignmentString.Replace("\\r\\n", ""), actualSequenceAlignmentString.Replace("\r\n", ""));
        }

        /// <summary>
        /// Validates Contig ToString
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateContigToString()
        {
            int matchScore = 5;
            int mismatchScore = -4;
            int gapCost = -10;
            double mergeThreshold = 4;
            double consensusThreshold = 66;
            string seq2Str = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.Seq2StrNode);
            string seq1Str = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.Seq1StrNode);
            Sequence seq2 = new Sequence(Alphabets.DNA, seq2Str);
            Sequence seq1 = new Sequence(Alphabets.DNA, seq1Str);

            OverlapDeNovoAssembler assembler = new OverlapDeNovoAssembler();
            assembler.MergeThreshold = mergeThreshold;
            assembler.OverlapAlgorithm = new NeedlemanWunschAligner();
            ((IPairwiseSequenceAligner)assembler.OverlapAlgorithm).SimilarityMatrix =
                new DiagonalSimilarityMatrix(matchScore, mismatchScore);
            ((IPairwiseSequenceAligner)assembler.OverlapAlgorithm).GapOpenCost = gapCost;
            assembler.ConsensusResolver = new SimpleConsensusResolver(consensusThreshold);
            assembler.AssumeStandardOrientation = false;

            List<ISequence> inputs = new List<ISequence>();
            inputs.Add(seq1);
            inputs.Add(seq2);

            IOverlapDeNovoAssembly seqAssembly = (IOverlapDeNovoAssembly)assembler.Assemble(inputs);
            Contig contig0 = seqAssembly.Contigs[0];
            string actualString = contig0.ToString();
            string expectedString = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.OverlapDenovoExpectedNode);
            Assert.AreEqual(actualString.Replace("\r\n", ""), expectedString.Replace("\\r\\n", ""));

            // Get the parameters from Xml
            int matchScore1 = int.Parse(utilityObj.xmlUtil.GetTextValue(Constants.AssemblyAlgorithmNodeName,
                Constants.MatchScoreNode), null);
            int mismatchScore1 = int.Parse(utilityObj.xmlUtil.GetTextValue(Constants.AssemblyAlgorithmNodeName,
                Constants.MisMatchScoreNode), null);
            int gapCost1 = int.Parse(utilityObj.xmlUtil.GetTextValue(Constants.AssemblyAlgorithmNodeName,
                Constants.GapCostNode), null);
            double mergeThreshold1 = double.Parse(utilityObj.xmlUtil.GetTextValue(
                Constants.AssemblyAlgorithmNodeName,
                Constants.MergeThresholdNode), null);
            double consensusThreshold1 = double.Parse(utilityObj.xmlUtil.GetTextValue(
                Constants.AssemblyAlgorithmNodeName,
                Constants.ConsensusThresholdNode), null);
            string sequence1 = utilityObj.xmlUtil.GetTextValue(Constants.AssemblyAlgorithmNodeName,
                Constants.SequenceNode1);
            string sequence2 = utilityObj.xmlUtil.GetTextValue(Constants.AssemblyAlgorithmNodeName,
                Constants.SequenceNode2);
            string sequence3 = utilityObj.xmlUtil.GetTextValue(Constants.AssemblyAlgorithmNodeName,
                Constants.SequenceNode3);
            IAlphabet alphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(
                Constants.AssemblyAlgorithmNodeName,
                Constants.AlphabetNameNode));

            Sequence seq4 = new Sequence(alphabet, sequence1);
            Sequence seq5 = new Sequence(alphabet, sequence2);
            Sequence seq6 = new Sequence(alphabet, sequence3);

            OverlapDeNovoAssembler assembler1 = new OverlapDeNovoAssembler();
            assembler1.MergeThreshold = mergeThreshold1;
            assembler1.OverlapAlgorithm = new PairwiseOverlapAligner();
            ((IPairwiseSequenceAligner)assembler1.OverlapAlgorithm).SimilarityMatrix =
                new DiagonalSimilarityMatrix(matchScore1, mismatchScore1);
            ((IPairwiseSequenceAligner)assembler1.OverlapAlgorithm).GapOpenCost = gapCost1;
            assembler1.ConsensusResolver = new SimpleConsensusResolver(consensusThreshold1);
            assembler1.AssumeStandardOrientation = false;

            List<ISequence> inputs1 = new List<ISequence>();
            inputs1.Add(seq4);
            inputs1.Add(seq5);
            inputs1.Add(seq6);

            // Assembles all the sequences.
            IOverlapDeNovoAssembly seqAssembly1 = (IOverlapDeNovoAssembly)assembler1.Assemble(inputs1);
            Contig contig1 = seqAssembly1.Contigs[0];
            string actualString1 = contig1.ToString();
            string expectedString1 = "TATAAAGCGCCAAAATTTAGGCACCCGCGGTATT";
            Assert.AreEqual(expectedString1, actualString1);

        }

        /// <summary>
        /// Validates MatePair ToString
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMatePairToString()
        {
            MatePair p = new MatePair("2K");
            p.ForwardReadID = "F";
            p.ReverseReadID = "R";
            string actualString = p.ToString();
            string expectedString = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.MatePairExpectedNode);
            Assert.AreEqual(actualString, expectedString);
        }

        /// <summary>
        /// Validates OverlapDenovoAssembly ToString
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateOverlapDenovoAssemblyToString()
        {
            int matchScore = 5;
            int mismatchScore = -4;
            int gapCost = -10;
            double mergeThreshold = 4;
            double consensusThreshold = 66;
            string seq2Str = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.Seq2StrNode);
            string seq1Str = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.Seq1StrNode);
            Sequence seq2 = new Sequence(Alphabets.DNA, seq2Str);
            Sequence seq1 = new Sequence(Alphabets.DNA, seq1Str);

            OverlapDeNovoAssembler assembler = new OverlapDeNovoAssembler();
            assembler.MergeThreshold = mergeThreshold;
            assembler.OverlapAlgorithm = new NeedlemanWunschAligner();
            ((IPairwiseSequenceAligner)assembler.OverlapAlgorithm).SimilarityMatrix =
                new DiagonalSimilarityMatrix(matchScore, mismatchScore);
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
            string expectedString = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.OverlapDenovoExpectedNode);
            Assert.AreEqual(actualString.Replace("\r\n", ""), expectedString.Replace("\\r\\n", ""));


            // Get the parameters from Xml
            int matchScore1 = int.Parse(utilityObj.xmlUtil.GetTextValue(Constants.AssemblyAlgorithmNodeName,
                Constants.MatchScoreNode), null);
            int mismatchScore1 = int.Parse(utilityObj.xmlUtil.GetTextValue(Constants.AssemblyAlgorithmNodeName,
                Constants.MisMatchScoreNode), null);
            int gapCost1 = int.Parse(utilityObj.xmlUtil.GetTextValue(Constants.AssemblyAlgorithmNodeName,
                Constants.GapCostNode), null);
            double mergeThreshold1 = double.Parse(utilityObj.xmlUtil.GetTextValue(
                Constants.AssemblyAlgorithmNodeName,
                Constants.MergeThresholdNode), null);
            double consensusThreshold1 = double.Parse(utilityObj.xmlUtil.GetTextValue(
                Constants.AssemblyAlgorithmNodeName,
                Constants.ConsensusThresholdNode), null);
            string sequence1 = utilityObj.xmlUtil.GetTextValue(Constants.AssemblyAlgorithmNodeName,
                Constants.SequenceNode1);
            string sequence2 = utilityObj.xmlUtil.GetTextValue(Constants.AssemblyAlgorithmNodeName,
                Constants.SequenceNode2);
            string sequence3 = utilityObj.xmlUtil.GetTextValue(Constants.AssemblyAlgorithmNodeName,
                Constants.SequenceNode3);
            IAlphabet alphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(
                Constants.AssemblyAlgorithmNodeName,
                Constants.AlphabetNameNode));

            Sequence seq4 = new Sequence(alphabet, sequence1);
            Sequence seq5 = new Sequence(alphabet, sequence2);
            Sequence seq6 = new Sequence(alphabet, sequence3);

            OverlapDeNovoAssembler assembler1 = new OverlapDeNovoAssembler();
            assembler1.MergeThreshold = mergeThreshold1;
            assembler1.OverlapAlgorithm = new PairwiseOverlapAligner();
            ((IPairwiseSequenceAligner)assembler1.OverlapAlgorithm).SimilarityMatrix =
                new DiagonalSimilarityMatrix(matchScore1, mismatchScore1);
            ((IPairwiseSequenceAligner)assembler1.OverlapAlgorithm).GapOpenCost = gapCost1;
            assembler1.ConsensusResolver = new SimpleConsensusResolver(consensusThreshold1);
            assembler1.AssumeStandardOrientation = false;

            List<ISequence> inputs1 = new List<ISequence>();
            inputs1.Add(seq4);
            inputs1.Add(seq5);
            inputs1.Add(seq6);

            // Assembles all the sequences.
            IOverlapDeNovoAssembly seqAssembly1 = (IOverlapDeNovoAssembly)assembler1.Assemble(inputs1);

            Assert.AreEqual(0, seqAssembly1.UnmergedSequences.Count);
            Assert.AreEqual(1, seqAssembly1.Contigs.Count);
            assembler1.OverlapAlgorithm = new SmithWatermanAligner();
            seqAssembly = (OverlapDeNovoAssembly)assembler1.Assemble(inputs1);

            string actualString1 = seqAssembly.ToString();
            string expectedString1 = "TYMKWRRGCGCCAAAATTTAGGC\r\n";
            Assert.AreEqual(actualString1, expectedString1);

        }

        /// <summary>
        /// Validates PadenaAssembly ToString
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaAssemblyToString()
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

            // read sequences from xml
            string sequence1 = utilityObj.xmlUtil.GetTextValue(Constants.AssemblyAlgorithmNodeName,
    Constants.SequenceNode1);
            string sequence2 = utilityObj.xmlUtil.GetTextValue(Constants.AssemblyAlgorithmNodeName,
                Constants.SequenceNode2);
            IAlphabet alphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(
                Constants.AssemblyAlgorithmNodeName,
                Constants.AlphabetNameNode));

            Sequence seq3 = new Sequence(alphabet, sequence1);
            Sequence seq4 = new Sequence(alphabet, sequence2);
            IList<ISequence> contigList1 = new List<ISequence>();
            contigList1.Add(seq3);
            contigList1.Add(seq4);
            PadenaAssembly denovoAssembly1 = new PadenaAssembly();
            denovoAssembly1.AddContigs(contigList1);

            string actualString1 = denovoAssembly1.ToString();
            string expectedString1 = "GCCAAAATTTAGGC\r\nTTATGGCGCCCACGGA\r\n";
            Assert.AreEqual(expectedString1, actualString1);

        }

        /// <summary>
        /// Validates Sequence ConvertToString()
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSequenceConvertToString()
        {
            string seqLargeString = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.seqLargeStringNode);
            ISequence seqLarge = new Sequence(Alphabets.DNA, seqLargeString);
            string ActualLargeString = seqLarge.ToString();
            string seqLargeExpected = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.seqLargeExpected2Node);
            string expectedLargeString = string.Format(CultureInfo.CurrentCulture,
                seqLargeExpected, (seqLarge.Count - Helper.AlphabetsToShowInToString));
            Assert.AreEqual(expectedLargeString, ActualLargeString);

            List<ISequence> seqsList;
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(Constants.SimpleFastaNodeName,
                Constants.FilePathNode);
            using (StreamReader reader = new StreamReader(filePath))
            {
                IEnumerable<ISequence> seq = null;

                using (FastAParser parser = new FastAParser())
                {
                    parser.Alphabet = Alphabets.Protein;
                    seq = parser.Parse(reader);

                    //Create a list of sequences.
                    seqsList = seq.ToList();
                }
            }

            string seqString = new string(seqsList[0].Select(a => (char)a).ToArray());
            Assert.AreEqual(seqString.Substring(2, 5), ((Sequence)seqsList[0]).ConvertToString(2, 5));
        }

        /// <summary>
        /// Validates DerivedSequence ConvertToString()
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateDerivedSequenceConvertToString()
        {
            string seqLargeStr = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.seqLargeStringNode);
            ISequence seqLarge = new Sequence(Alphabets.DNA, seqLargeStr);
            ISequence DeriveSeqLarge = new DerivedSequence(seqLarge, false, true);

            string ActualLargeString = DeriveSeqLarge.ToString();
            string seqLargeExpected = utilityObj.xmlUtil.GetTextValue(Constants.ToStringNodeName,
                Constants.seqLargeExpectedNode);
            string expectedLargeString = string.Format(CultureInfo.CurrentCulture,
                seqLargeExpected, (seqLarge.Count - Helper.AlphabetsToShowInToString));

            Assert.AreEqual(expectedLargeString, ActualLargeString);

            // Get input and expected values from xml
            string expectedSequence = utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.ExpectedSequence);
            string alphabetName = utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinDerivedSequenceNode, Constants.AlphabetNameNode);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            // Create derived Sequence
            ISequence seq = new Sequence(alphabet, expectedSequence);
            DerivedSequence derSequence = new DerivedSequence(seq, false, false);

            Assert.AreEqual(expectedSequence.Substring(2, 5), derSequence.ConvertToString(2, 5));
        }

        #endregion BVT Test Cases
    }
}
