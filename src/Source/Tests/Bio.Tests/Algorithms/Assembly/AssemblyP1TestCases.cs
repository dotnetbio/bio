using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

using Bio.Algorithms.Alignment;
using Bio.Algorithms.Assembly;
using Bio.Algorithms.Kmer;
using Bio.SimilarityMatrices;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests.Algorithms.Assembly
{
    /// <summary>
    ///     Sequence Assembly and Consensus P1 Test case implementation.
    /// </summary>
    [TestFixture]
    public class AssemblyP1TestCases
    {
        #region Enums

        /// <summary>
        ///     Assembly Parameters which are used for different test cases
        ///     based on which the test cases are executed.
        /// </summary>
        private enum AssemblyParameters
        {
            Assemble,
            DiagonalSM,
            SimilarityMatrix,
            Consensus
        };

        #endregion Enums

        private readonly Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #region Sequence Assembly P1 Test cases

        /// <summary>
        ///     Validates if the Assemble() method assembles the Dna
        ///     sequences on passing valid sequences as parameter..
        ///     Input: Sequences with Alphabets, matchscore, mismatch score,
        ///     gap cost, merge threshold, consensus threshold.
        ///     Validation: validates unmerged sequences count, contigs count,
        ///     contig sequences count and concensus.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SequenceAssemblerWithAssembleMethodForDna()
        {
            this.ValidateSequenceAssemblerGeneral(Constants.AssemblySequenceAlgorithmNodeName,
                                             AssemblyParameters.Assemble, false);
        }

        /// <summary>
        ///     Validates if the Assemble() method assembles Maximum number of
        ///     sequences on passing valid sequences as parameter..
        ///     Input: Sequences with Alphabets, matchscore, mismatch score,
        ///     gap cost, merge threshold, consensus threshold.
        ///     Validation: validates unmerged sequences count, contigs count,
        ///     contig sequences count and concensus.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SequenceAssemblerWithAssembleMethodForMaxSequences()
        {
            this.ValidateSequenceAssemblerGeneral(Constants.AssemblyMaxSequenceAlgorithmNodeName,
                                             AssemblyParameters.Assemble, false);
        }

        /// <summary>
        ///     Validates if the Assemble() method assembles Minimum number of
        ///     sequences on passing valid sequences as parameter.
        ///     Input: Sequences with Alphabets, matchscore, mismatch score,
        ///     gap cost, merge threshold, consensus threshold.
        ///     Validation: validates unmerged sequences count, contigs count,
        ///     contig sequences count and concensus.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SequenceAssemblerWithAssembleMethodForMinSequences()
        {
            this.ValidateSequenceAssemblerGeneral(Constants.AssemblyMinSequenceAlgorithmNodeName,
                                             AssemblyParameters.Assemble, false);
        }

        /// <summary>
        ///     Validates if the Assemble() method assembles the Dna
        ///     sequences on passing valid sequences as parameter with valid threshold.
        ///     Input: Sequences with Alphabets, matchscore, mismatch score,
        ///     gap cost, merge threshold, consensus threshold.
        ///     Validation: validates unmerged sequences count, contigs count,
        ///     contig sequences count and concensus.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SequenceAssemblerWithAssembleMethodForValidThreshold()
        {
            this.ValidateSequenceAssemblerGeneral(Constants.AssemblySequenceAlgorithmNodeName,
                                             AssemblyParameters.Assemble, false);
        }

        /// <summary>
        ///     Validates if the Assemble() method assembles the Dna
        ///     sequences on passing valid sequences as parameter and maximum threshold.
        ///     Input: Sequences with Alphabets, matchscore, mismatch score,
        ///     gap cost, Maximum merge threshold, consensus threshold.
        ///     Validation: validates unmerged sequences count, contigs count,
        ///     contig sequences count and concensus.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SequenceAssemblerWithAssembleMethodForMaxThreshold()
        {
            this.ValidateSequenceAssemblerGeneral(Constants.AssemblyMaxThresholdSequenceAlgorithmNodeName,
                                             AssemblyParameters.Assemble, false);
        }

        /// <summary>
        ///     Validates if the Assemble() method assembles the Dna
        ///     sequences on passing valid sequences as parameter and minimum threshold.
        ///     Input: Sequences with Alphabets, matchscore, mismatch score,
        ///     gap cost, Minimum merge threshold, consensus threshold.
        ///     Validation: validates unmerged sequences count, contigs count,
        ///     contig sequences count and concensus.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SequenceAssemblerWithAssembleMethodForMinThreshold()
        {
            this.ValidateSequenceAssemblerGeneral(Constants.AssemblyMinThresholdSequenceAlgorithmNodeName,
                                             AssemblyParameters.Assemble, false);
        }

        /// <summary>
        ///     Validates if the Assemble() method assembles the
        ///     sequences on passing valid sequences as parameter using Diagonal Similarity Matrix.
        ///     Input: Sequences with Alphabets, matchscore, mismatch score,
        ///     gap cost, Minimum merge threshold, consensus threshold, Diagonal Similarity Matrix.
        ///     Validation: validates unmerged sequences count, contigs count,
        ///     contig sequences count and concensus.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SequenceAssemblerWithAssembleMethodForDiagonalSM()
        {
            this.ValidateSequenceAssemblerGeneral(Constants.AssemblySequenceAlgorithmNodeName,
                                             AssemblyParameters.DiagonalSM, false);
        }

        /// <summary>
        ///     Validates if the Assemble() method assembles the
        ///     sequences on passing valid sequences as parameter using Diagonal Similarity Matrix.
        ///     Input: Sequences with Alphabets, matchscore, mismatch score,
        ///     gap cost, Minimum merge threshold, consensus threshold, Diagonal Similarity Matrix.
        ///     Validation: validates unmerged sequences count, contigs count,
        ///     contig sequences count and concensus.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SequenceAssemblerWithAssembleMethodForSimilarityMatrix()
        {
            this.ValidateSequenceAssemblerGeneral(Constants.AssemblySequenceAlgorithmNodeName,
                                             AssemblyParameters.SimilarityMatrix, false);
        }

        #endregion Sequence Assembly P1 Test cases

        #region Consensus P1 Test cases

        /// <summary>
        ///     Validates if the MakeConsensus() method provides the Consensus for a given
        ///     DNA Contig.
        ///     Input: Sequences with Alphabets, matchscore, mismatch score,
        ///     gap cost, merge threshold, consensus threshold.
        ///     Validation: validates unmerged sequences count, contigs count,
        ///     contig sequences count and concensus.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SimpleConsensusWithMakeConsensusMethodForDna()
        {
            this.ValidateSequenceAssemblerGeneral(Constants.AssemblySequenceAlgorithmNodeName,
                                             AssemblyParameters.Consensus, false);
        }

        /// <summary>
        ///     Validates if the MakeConsensus() method provides the Consensus for a given
        ///     Contig with Maximum threshold value.
        ///     Input: Sequences with Alphabets, matchscore, mismatch score,
        ///     gap cost, Maximum merge threshold, consensus threshold.
        ///     Validation: validates unmerged sequences count, contigs count,
        ///     contig sequences count and concensus.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SimpleConsensusWithMakeConsensusMethodForMaxThreshold()
        {
            this.ValidateSequenceAssemblerGeneral(Constants.AssemblyMaxThresholdSequenceAlgorithmNodeName,
                                             AssemblyParameters.Consensus, false);
        }

        /// <summary>
        ///     Validates if the MakeConsensus() method provides the Consensus for a given
        ///     Contig with Minimum threshold value.
        ///     Input: Sequences with Alphabets, matchscore, mismatch score,
        ///     gap cost, Minimum merge threshold, consensus threshold.
        ///     Validation: validates unmerged sequences count, contigs count,
        ///     contig sequences count and concensus.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SimpleConsensusWithMakeConsensusMethodForMinThreshold()
        {
            this.ValidateSequenceAssemblerGeneral(Constants.AssemblyMinThresholdSequenceAlgorithmNodeName,
                                             AssemblyParameters.Consensus, false);
        }

        /// <summary>
        ///     Validates if the MakeConsensus() method provides the Consensus for a given
        ///     Contig with  valid Threshold and valid useAmbiguityCodes value.
        ///     Input: Sequences with Alphabets, matchscore, mismatch score,
        ///     gap cost, Maximum merge threshold, consensus threshold.
        ///     Validation: validates unmerged sequences count, contigs count,
        ///     contig sequences count and concensus.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SimpleConsensusWithMakeConsensusForValidThreshold()
        {
            this.ValidateSequenceAssemblerGeneral(Constants.AssemblySequenceAlgorithmNodeName,
                                             AssemblyParameters.Consensus, false);
        }

        /// <summary>
        ///     Validate Sequence Assmebly default constructor.
        ///     Input: Sequences with Alphabets, matchscore, mismatch score,
        ///     gap cost, merge threshold, consensus threshold.
        ///     Validation: Validate Sequence Assmebly default constructor,unmerged sequences count,
        ///     contigs count,contig sequences count and concensus.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SequenceAssemblerWithAssembleMethodForDnaUsingSeqAssemblyCtr()
        {
            this.ValidateSequenceAssemblerGeneral(Constants.AssemblySequenceAlgorithmNodeName,
                                             AssemblyParameters.Assemble, true);
        }

        /// <summary>
        ///     Validate Kmer constructor and its properties.
        ///     Input:  Valid Sequence
        ///     Validation: Validation of Kmer Sequence.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateKmerSequenceProperties()
        {
            this.ValidateKmer(Constants.AssemblyAlgorithmNodeName, false);
        }

        /// <summary>
        ///     Validate building kmer by passing Kmer length.
        ///     Input:  Valid Sequence and Kmer length.
        ///     Validation: Validation of Kmer builder.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateBuilderKmer()
        {
            this.ValidateKmer(Constants.KmerbuilderNode, true);
        }

        /// <summary>
        ///     Validate Compare two sequences
        ///     Input:  Valid Sequences and Kmer length.
        ///     Validation: Validation of  Comparing two sequences
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateCompareTwoSequences()
        {
            this.ValidateComputeFeature(
                Constants.CompareTwoSequencesNode,
                AssemblyParameters.Consensus);
        }

        /// <summary>
        ///     Validate Features of two sequences
        ///     Input:  Valid Sequences and Kmer length.
        ///     Validation: Validation of Features two sequences
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateComputeFeatures()
        {
            this.ValidateComputeFeature(
                Constants.CompareTwoProtienSequencesNode,
                AssemblyParameters.Assemble);
        }

        /// <summary>
        ///     Validate Features of two sequences
        ///     Input:  Valid Sequences and Kmer length.
        ///     Validation: Validation of Features two sequences
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateComputeMoleculeFeatures()
        {
            this.ValidateComputeFeature(
                Constants.CompareTwoSequencesNode,
                AssemblyParameters.Assemble);
        }

        /// <summary>
        ///     Validate Properties of Difference Node
        ///     Input:  Valid DifferenceNode object.
        ///     Validation: Validation of Properties
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateDifferenceNodeProperty()
        {
            var diffObj = new DifferenceNode(1, 1, 4, 4);

            Assert.AreEqual(1, diffObj.Sequence1Start);
            Assert.AreEqual(1, diffObj.Sequence2Start);
            Assert.AreEqual(4, diffObj.Sequence1End);
            Assert.AreEqual(4, diffObj.Sequence2End);
            Assert.AreEqual("1 : 1 : 4 : 4", diffObj.ToString());
        }

        /// <summary>
        ///     Compares the difference between 2 sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateSequenceCompare()
        {
            string firstSequence = this.utilityObj.xmlUtil.GetTextValue(Constants.SequenceCompareNode,
                                                                   Constants.SequenceNode1);
            string secondSequence = this.utilityObj.xmlUtil.GetTextValue(Constants.SequenceCompareNode,
                                                                    Constants.SequenceNode2);
            string replace = this.utilityObj.xmlUtil.GetTextValue(Constants.SequenceCompareNode,
                                                             Constants.ReplaceNode);
            ISequence seq1 = new Sequence(Alphabets.DNA, firstSequence);
            ISequence seq2 = new Sequence(Alphabets.DNA, secondSequence);
            var kmerBuilder = new SequenceToKmerBuilder();
            KmersOfSequence kmers = kmerBuilder.Build(seq1, 2);
            List<WordMatch> nodes = WordMatch.BuildMatchTable(kmers, seq2, 2);
            List<WordMatch> matchList = WordMatch.GetMinimalList(nodes, 2);
            List<DifferenceNode> diffNode = DifferenceNode.BuildDiffList(matchList, seq1, seq2);
            List<DifferenceNode.CompareFeature> features = DifferenceNode.OutputDiffList(diffNode, seq1, seq2);

            //Validating the behavior. 
            Assert.AreEqual(features.Count, 4);
            Assert.AreEqual(features[0].Feature, Constants.InsertionOfOneBaseIn2);
            Assert.AreEqual(features[1].FeatureType, replace);
            Assert.AreEqual(features[2].Feature, Constants.InsertionOfOneBaseIn1);
            Assert.AreEqual(features[3].FeatureType, replace);
        }

        #endregion Consensus BVT Test cases

        #region Supported methods

        /// <summary>
        ///     Validates the Sequence Assembler for all the general test cases.
        /// </summary>
        /// <param name="nodeName">Xml Node Name</param>
        /// <param name="additionalParameter">
        ///     Additional Parameter based
        ///     on which the validations are done.
        /// </param>
        /// <param name="isSeqAssemblyctr">True if Default contructor is validated or else false.</param>
        private void ValidateSequenceAssemblerGeneral(string nodeName,
                                                      AssemblyParameters additionalParameter, bool isSeqAssemblyctr)
        {
            // Get the parameters from Xml
            int matchScore = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                       Constants.MatchScoreNode), null);
            int mismatchScore = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                          Constants.MisMatchScoreNode), null);
            int gapCost = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                    Constants.GapCostNode), null);
            double mergeThreshold = double.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                 Constants.MergeThresholdNode), null);
            double consensusThreshold = double.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                     Constants.ConsensusThresholdNode),
                                                     null);
            string[] sequences = this.utilityObj.xmlUtil.GetTextValues(nodeName,
                                                                  Constants.SequencesNode);
            IAlphabet alphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                     Constants.AlphabetNameNode));
            string documentation = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                   Constants.DocumentaionNode);
            var info = new SerializationInfo(typeof (OverlapDeNovoAssembly),
                                             new FormatterConverter());
            var context = new StreamingContext(StreamingContextStates.All);

            var inputs = new List<ISequence>();

            switch (additionalParameter)
            {
                case AssemblyParameters.Consensus:
                    for (int i = 0; i < sequences.Length; i++)
                    {
                        // Logs the sequences
                        ApplicationLog.WriteLine(string.Format(null, "SimpleConsensusMethod P1 : Sequence '{0}' used is '{1}'.", i, sequences[i]));

                        var seq = new Sequence(alphabet, sequences[i]);
                        inputs.Add(seq);
                    }
                    break;
                default:
                    for (int i = 0; i < sequences.Length; i++)
                    {
                        // Logs the sequences
                        ApplicationLog.WriteLine(string.Format(null, "SequenceAssembly P1 : Sequence '{0}' used is '{1}'.", i, sequences[i]));
                        var seq = new Sequence(alphabet, sequences[i]);
                        inputs.Add(seq);
                    }
                    break;
            }

            // here is how the above sequences should align:
            // TATAAAGCGCCAA
            //         GCCAAAATTTAGGC
            //                   AGGCACCCGCGGTATT   <= reversed
            // 
            // TATAAAGCGCCAAAATTTAGGCACCCGCGGTATT

            var assembler = new OverlapDeNovoAssembler
            {
                MergeThreshold = mergeThreshold,
                OverlapAlgorithm = new PairwiseOverlapAligner()
            };

            switch (additionalParameter)
            {
                case AssemblyParameters.DiagonalSM:
                    (assembler.OverlapAlgorithm).SimilarityMatrix =
                        new DiagonalSimilarityMatrix(matchScore, mismatchScore);
                    break;
                case AssemblyParameters.SimilarityMatrix:
                    string blosumFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.BlosumFilePathNode);
                    (assembler.OverlapAlgorithm).SimilarityMatrix = new SimilarityMatrix(new StreamReader(blosumFilePath));
                    break;
                default:
                    (assembler.OverlapAlgorithm).SimilarityMatrix = new DiagonalSimilarityMatrix(matchScore, mismatchScore);
                    break;
            }

            (assembler.OverlapAlgorithm).GapOpenCost = gapCost;
            assembler.ConsensusResolver = new SimpleConsensusResolver(consensusThreshold);
            assembler.AssumeStandardOrientation = false;

            // Assembles all the sequences.
            IOverlapDeNovoAssembly assembly = (IOverlapDeNovoAssembly) assembler.Assemble(inputs);

            // Set Documentation property.
            assembly.Documentation = documentation;

            // Get the parameters from Xml in general
            int contigSequencesCount = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ContigSequencesCountNode), null);
            string contigConsensus = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ContigConsensusNode);

            switch (additionalParameter)
            {
                case AssemblyParameters.Consensus:
                    // Read the contig from Contig method.
                    Contig contigReadForConsensus = assembly.Contigs[0];
                    contigReadForConsensus.Consensus = null;
                    var simpleSeqAssembler = new OverlapDeNovoAssembler
                    {
                        ConsensusResolver = new SimpleConsensusResolver(consensusThreshold)
                    };
                    simpleSeqAssembler.MakeConsensus(alphabet, contigReadForConsensus);

                    // Log the required info.
                    ApplicationLog.WriteLine(string.Format(null, "SimpleConsensusMethod BVT : Consensus read is '{0}'.", contigReadForConsensus.Consensus));
                    Assert.AreEqual(contigConsensus, new String(contigReadForConsensus.Consensus.Select(a => (char) a).ToArray()));
                    break;
                default:
                    // Get the parameters from Xml for Assemble() method test cases.
                    int unMergedCount = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                  Constants.UnMergedSequencesCountNode),
                                                  null);
                    int contigsCount = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                 Constants.ContigsCountNode), null);

                    Assert.AreEqual(unMergedCount, assembly.UnmergedSequences.Count);
                    Assert.AreEqual(contigsCount, assembly.Contigs.Count);
                    Assert.AreEqual(documentation, assembly.Documentation);
                    Contig contigRead = assembly.Contigs[0];

                    // Logs the consensus
                    ApplicationLog.WriteLine(string.Format(null, "SequenceAssembly BVT : Un Merged Sequences Count is '{0}'.", assembly.UnmergedSequences.Count));
                    ApplicationLog.WriteLine(string.Format(null, "SequenceAssembly BVT : Contigs Count is '{0}'.", assembly.Contigs.Count));
                    ApplicationLog.WriteLine(string.Format(null, "SequenceAssembly BVT : Contig Sequences Count is '{0}'.", contigRead.Sequences.Count));
                    ApplicationLog.WriteLine(string.Format(null, "SequenceAssembly BVT : Consensus read is '{0}'.", contigRead.Consensus));

                    Assert.AreEqual(contigConsensus, new String(contigRead.Consensus.Select(a => (char) a).ToArray()));
                    Assert.AreEqual(contigSequencesCount, contigRead.Sequences.Count);
                    break;
            }
        }

        /// <summary>
        ///     Validate building Kmer using sequence and kmer length.
        /// </summary>
        /// <param name="nodeName">Name of the xml node for different test cases</param>
        /// <param name="IsKmerBuilder">True if validating kmerbuilder or else false.</param>
        private void ValidateKmer(string nodeName, bool IsKmerBuilder)
        {
            // Get the parameters from Xml
            string Sequence = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode1);
            string expectedKmerCount = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmrSeqCountNode);
            string expectedKmerSeq = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerSequenceNode);
            string expectedKmerPos = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.PositionsNode);

            // Create a Kmer Sequence.
            ISequence seq = new Sequence(Alphabets.DNA, Sequence);
            KmersOfSequence kmerSeq;

            if (IsKmerBuilder)
            {
                // Build Kmer.
                var kmerBuilder = new SequenceToKmerBuilder();
                KmersOfSequence kmerList = kmerBuilder.Build(seq, 2);

                // Validate builder kmer.
                Assert.AreEqual(expectedKmerCount, kmerList.Kmers.Count.ToString((IFormatProvider) null));
                Assert.AreEqual(expectedKmerSeq, new String(kmerList.BaseSequence.Select(a => (char) a).ToArray()));
                Assert.AreEqual(expectedKmerPos, kmerList.Length.ToString((IFormatProvider) null));
            }
            else
            {
                kmerSeq = new KmersOfSequence(seq, 2);

                // Validate Kmer Seq.
                Assert.AreEqual(expectedKmerSeq, new String(kmerSeq.BaseSequence.Select(a => (char) a).ToArray()));
                Assert.AreEqual(expectedKmerPos, kmerSeq.Length.ToString((IFormatProvider) null));
            }
        }

        /// <summary>
        ///     Validates the Sequences for all the general test cases.
        /// </summary>
        /// <param name="node">Xml Node Name</param>
        /// <param name="additionalParameter">
        ///     Additional Parameter based
        ///     on which the validations are done.
        /// </param>
        private void ValidateComputeFeature(string node, AssemblyParameters additionalParameter)
        {
            // Get the parameters from Xml
            string firstSequence = this.utilityObj.xmlUtil.GetTextValue(node, Constants.SequenceNode1);
            string secondSequence = this.utilityObj.xmlUtil.GetTextValue(node, Constants.SequenceNode2);
            string kmerLength = this.utilityObj.xmlUtil.GetTextValue(node, Constants.KmerLengthNode);
            string expectedFeatureCount = this.utilityObj.xmlUtil.GetTextValue(node, Constants.FeatureCount);
            string expectedFeature = this.utilityObj.xmlUtil.GetTextValue(node, Constants.FeatureName);
            string expectedFeatureType = this.utilityObj.xmlUtil.GetTextValue(node, Constants.FeatureType);
            string expectedStartIndex = this.utilityObj.xmlUtil.GetTextValue(node, Constants.StartIndexNode);
            string expectedEndIndex = this.utilityObj.xmlUtil.GetTextValue(node, Constants.EndIndexNode);

            ISequence seq1 = null;
            ISequence seq2 = null;

            // Create Sequences.
            switch (additionalParameter)
            {
                case AssemblyParameters.Assemble:
                    var seqObj1 =
                        new Sequence(Alphabets.Protein, firstSequence);
                    var seqObj2 =
                        new Sequence(Alphabets.Protein, secondSequence);
                    seq1 = seqObj1;
                    seq2 = seqObj2;
                    break;
                case AssemblyParameters.Consensus:
                    seq1 = new Sequence(Alphabets.DNA, firstSequence);
                    seq2 = new Sequence(Alphabets.DNA, secondSequence);
                    break;
            }

            var kmerBuilder = new SequenceToKmerBuilder();
            KmersOfSequence kmerList =
                kmerBuilder.Build(seq1, int.Parse(kmerLength, null));
            List<WordMatch> nodes =
                WordMatch.BuildMatchTable(
                    kmerList,
                    seq2,
                    int.Parse(kmerLength, null));
            List<WordMatch> matchList =
                WordMatch.GetMinimalList(nodes, int.Parse(kmerLength, null));
            List<DifferenceNode> diffNode =
                DifferenceNode.BuildDiffList(matchList, seq1, seq2);
            List<DifferenceNode.CompareFeature> features =
                DifferenceNode.OutputDiffList(diffNode, seq1, seq2);

            // Validate difference.

            Assert.AreEqual(expectedFeatureCount, features.Count.ToString((IFormatProvider) null));
            Assert.AreEqual(expectedFeature, features[0].Feature);
            Assert.AreEqual(expectedFeatureType, features[0].FeatureType);
            Assert.AreEqual(expectedStartIndex, features[0].Start.ToString((IFormatProvider) null));
            Assert.AreEqual(expectedEndIndex, features[0].End.ToString((IFormatProvider) null));
            ApplicationLog.WriteLine(string.Format(null, "Kmer P1 : Validated DifferenceNodes successfully."));
        }

        #endregion Supported methods
    }
}