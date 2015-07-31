/****************************************************************************
 * PadenaBvtTestCases.cs
 * 
 *  This file contains the Padena Bvt test cases.
 * 
***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bio.Extensions;
using Bio.TestAutomation.Util;
using Bio.Algorithms.Assembly;
using Bio.Algorithms.Assembly.Graph;
using Bio.Algorithms.Assembly.Padena;
using Bio.Algorithms.Assembly.Padena.Scaffold;
using Bio.Algorithms.Assembly.Padena.Scaffold.ContigOverlapGraph;
using Bio.Algorithms.Kmer;
using Bio.IO.FastA;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Tests.Framework;

namespace Bio.TestAutomation.Algorithms.Assembly.Padena
{
    /// <summary>
    /// The class contains Bvt test cases to confirm Padena assembler.
    /// </summary>
    [TestClass]
    public class PadenaBvtTestCases
    {
        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\PadenaTestData\PadenaTestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static PadenaBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region PadenaStep1TestCases

        /// <summary>
        /// Validate ParallelDeNovoAssembler is building valid kmers 
        /// using 4 one line reads in a fasta file and kmerLength 4
        /// Input : 4 one line input reads from dna base sequence and kmerLength 4
        /// Output : kmers sequence
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep1BuildKmers()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateDe2AssemblerBuildKmers(Constants.OneLineReadsNode);
            }
        }

        /// <summary>
        /// Validate SequenceRangeToKmerBuilder Build(lstsequences,kmerLength) 
        /// is building valid kmers using one line 4 reads in a fasta file and kmerLength 4
        /// Input : 4 one line input reads from dna base sequence and kmerLength 4
        /// Output : kmers sequence
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep1KmerBuilderBuild()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateKmerBuilderBuild(Constants.OneLineReadsNode);
            }
        }

        /// <summary>
        /// Validate SequenceRangeToKmerBuilder Build(sequence,kmerLength) is 
        /// building valid kmers using one line sequence in a fasta file and kmerLength 4
        /// Input : 4 one line input reads from dna base sequence and kmerLength 4
        /// Output : kmers sequence
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep1KmerBuilderBuildWithSequence()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateKmerBuilderBuildWithSequence(Constants.OneLineReadsNode);
            }
        }

        /// <summary>
        /// Validate KmersOfSequence ctor (sequence, length, set of kmers) 
        /// by passing small size chromsome sequence and kmer length 28
        /// after building kmers
        /// Input : Build kmeres from 4000 input reads of small size 
        /// chromosome sequence and kmerLength 28
        /// Output : kmers of sequence object with build kmers
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep1KmersOfSequenceCtorWithBuildKmers()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateKmersOfSequenceCtorWithBuildKmers(Constants.SmallChromosomeReadsNode);
            }
        }

        /// <summary>
        /// Validate KmersOfSequence ctor (sequence, length) by passing 
        /// one line sequences, kmerLength 4 
        /// and populate kmers after building it. 
        /// Input : Build kmeres from 4 input reads of one line sequence
        /// and kmerLength 28
        /// Output : kmers of sequence object with build kmers
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep1KmersOfSequenceCtor()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateKmersOfSequenceCtor(Constants.OneLineReadsNode);
            }
        }

        /// <summary>
        /// Validate KmersOfSequence ToSequences() method using one line reads
        /// Input: Build kmers using 4 reads of one line sequence and kmerLength 4
        /// Ouput: kmers sequences
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep1KmersOfSequenceToSequences()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateKmersOfSequenceToSequences(Constants.OneLineReadsNode);
            }
        }

        /// <summary>
        /// Validate KmersOfSequence GetKmerSequence() method after populating kmers 
        /// using 4 reads from one line sequences and kmerLength 4
        /// Input: Build kmers using 4 reads of one line sequence and kmerLength 4
        /// Ouput: kmers sequences
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep1KmersOfSequenceGetKmers()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateKmersOfSequenceCtor(Constants.OneLineReadsNode);
            }
        }

        #endregion

        #region PadenaStep2TestCases

        /// <summary>
        /// Validate Graph after building it using build kmers 
        /// with one line reads and kmerLength 4
        /// Input: kmers
        /// Output: Graph
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep2BuildGraph()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateDe2AssemblerBuildGraph(Constants.OneLineStep2GraphNode);
            }
        }

        /// <summary>
        /// Validate Graph after building graph with DeBruijnGraph.Build()
        /// with kmers
        /// Input : Kmers
        /// Output: Graph
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep2DeBruijnGraph()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateDeBruijnGraphBuild(Constants.OneLineStep2GraphNode);
            }
        }

        /// <summary>
        /// Validate the DeBruijnNode ctor by passing kmer, kmerLength and graph object
        /// Input: kmer
        /// Output: DeBruijn Node
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep2DeBruijnNode()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateDeBruijnNodeCtor(Constants.OneLineStep2GraphNode);
            }
        }

        /// <summary>
        /// Create dbruijn node by passing kmer and create another node.
        /// Add new node as leftendextension of first node. Validate the 
        /// AddLeftEndExtension() method.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep2DeBruijnNodeAddLeftExtension()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateDeBruijnNodeAddLeftExtension(Constants.OneLineStep2GraphNode);
            }
        }

        /// <summary>
        /// Create dbruijn node by passing kmer and create another node.
        /// Add new node as leftendextension of first node. Validate the 
        /// AddRightEndExtension() method.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep2DeBruijnNodeAddRightExtension()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateDeBruijnNodeAddRightExtension(Constants.OneLineStep2GraphNode);
            }
        }

        #endregion

        #region PadenaStep3TestCases

        /// <summary>
        /// Validate the Padena step3 
        /// which removes dangling links from the graph
        /// Input: Graph with dangling links
        /// Output: Graph without any dangling links
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep3UndangleGraph()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateDe2AssemblerUnDangleGraph(Constants.OneLineStep3GraphNode);
            }
        }

        /// <summary>
        /// Validate the DanglingLinksPurger DetectErrorNodes() method 
        /// is identying the dangling nodes as expected
        /// Input: Graph with dangling links
        /// Output: dangling nodes
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep3DetectErrorNodes()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidatePadenaDetectErrorNodes(Constants.OneLineStep3GraphNode);
            }
        }

        /// <summary>
        /// Validate the DanglingLinksPurger is removing the dangling link nodes
        /// from the graph
        /// Input: Graph and dangling node
        /// Output: Graph without any dangling nodes
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep3RemoveErrorNodes()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidatePadenaRemoveErrorNodes(Constants.OneLineStep3GraphNode);
            }
        }
        #endregion

        #region PadenaStep4TestCases

        /// <summary>
        /// Validate Padena step4 ParallelDeNovoAssembler.RemoveRedundancy() by passing graph 
        /// using one line reads such that it will create bubbles in the graph
        /// Input: Graph with bubbles
        /// Output: Graph without bubbles
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep4RemoveRedundancy()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateDe2AssemblerRemoveRedundancy(
                    Constants.OneLineStep4ReadsAfterRemoveRedundancy);
            }
        }

        /// <summary>
        /// Validate Padena step4 Simp.RemoveRedundancy() by passing graph 
        /// using one line reads such that it will create bubbles in the graph
        /// Input: Graph with bubbles
        /// Output: Graph without bubbles
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep4RedundantPathPurgerCtor()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateRedundantPathPurgerCtor(Constants.OneLineStep4ReadsAfterErrorRemove);
            }
        }

        /// <summary>
        /// Validate Padena DetectErrorNodes() by passing graph with bubbles
        /// Input : Graph with bubbles
        /// Output: Nodes list
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep4DetectErrorNodes()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateRedundantPathPurgerCtor(Constants.OneLineStep4ReadsAfterErrorRemove);
            }
        }

        /// <summary>
        /// Validate Padena RemoveErrorNodes() by passing redundant nodes list and graph
        /// Input : graph and redundant nodes list
        /// Output: Graph without bubbles
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep4RemoveErrorNodes()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateRedundantPathPurgerCtor(Constants.OneLineStep4ReadsAfterErrorRemove);
            }
        }

        #endregion

        #region PadenaStep5TestCases

        /// <summary>
        /// Validate Padena step5 by passing graph and validating the contigs
        /// Input : graph
        /// Output: Contigs
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep5BuildContigs()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateDe2AssemblerBuildContigs(Constants.OneLineStep5ReadsNode);
            }
        }

        /// <summary>
        /// Validate Padena step5 SimpleContigBuilder.BuildContigs() by passing graph 
        /// Input : graph
        /// Output: Contigs
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep5SimpleContigBuilderBuildContigs()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateSimpleContigBuilderBuild(Constants.OneLineStep5ReadsNode);
            }
        }

        #endregion

        #region PadenaStep6:Step1:TestCases

        /// <summary>
        /// Validate paired reads for X1, Y1 format map reads.
        /// Input : X1,Y1 format map reads.
        /// Output : Validate forward and backward reads.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep6X1Y1FormatPairedReads()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidatePairedReads(Constants.X1AndY1PairedReadsNode);
            }
        }

        /// <summary>
        /// Validate paired reads for 1 and 2 format map reads.
        /// Input : 1,2 format map reads.
        /// Output : Validate forward and backward reads.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep6OneAndTwoFormatPairedReads()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidatePairedReads(Constants.OneAndTwoPairedReadsNode);
            }
        }

        /// <summary>
        /// Validate paired reads for F and R format map reads.
        /// Input : F,R format map reads.
        /// Output : Validate forward and backward reads.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep6FAndRFormatPairedReads()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidatePairedReads(Constants.FAndRPairedReadsNode);
            }
        }

        /// <summary>
        /// Validate Adding new library information to library list.
        /// Input : Library name,Standard deviation and mean length.
        /// Output : Validate forward and reverse reads.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep6Libraryinformation()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.AddLibraryInformation(Constants.X1AndY1FormatPairedReadAddLibrary);
            }
        }

        #endregion PadenaStep6:Step1:TestCases

        #region PadenaStep6:Step2:TestCases

        /// <summary>
        /// Validate MapReads to contigs for One line reads.
        /// Input : Reads,KmerLength,dangling threshold.
        /// Output : Validate MapReads to contigs.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep6MapReadsToContigs()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateMapReadsToContig(Constants.MapReadsToContigFullOverlapNode, true);
            }
        }

        /// <summary>
        /// Validate MapReads to contigs for One line reads for Partial overlap.
        /// Input : Reads,KmerLength,dangling threshold.
        /// Output : Validate MapReads to contigs.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep6MapReadsToContigsForPartialOverlap()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateMapReadsToContig(Constants.MapReadsToContigPartialOverlapNode, false);
            }
        }

        #endregion PadenaStep6:Step2:TestCases

        #region PadenaStep6:Step4:TestCases

        /// <summary>
        /// Validate filter Contig Pairs formed in Forward direction 
        /// with all paired reads supports orientation.
        /// Input : 3-4 Line sequence reads.
        /// Output : Filtered contigs.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep6FilterPairedReadsForForwardOrientation()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateFilterPaired(Constants.FilterPairedReadContigsNode);
            }
        }

        /// <summary>
        /// Validate filter Contig Pairs formed in Forward direction 
        /// using FilterPairedReads(redundancy)
        /// Input : 3-4 Line sequence reads.
        /// Output : Filtered contigs.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep6FilterPairedReadsUsingRedundancy()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateFilterPaired(Constants.FilterPairedReadContigsUsingRedundancy);
            }
        }

        /// <summary>
        /// Validate filter Contig Pairs formed in Reverse direction with 
        /// all paired reads support orientation.
        /// with all paired reads support orientation.
        /// Input : 3-4 Line sequence reads.
        /// Output : Filtered contigs.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep6FilterPairedReadsForReverseOrientation()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateFilterPaired(Constants.FilterPairedReadReverseOrietnationContigsNode);
            }
        }

        #endregion PadenaStep6:Step4:TestCases

        #region PadenaStep6:Step5:TestCases

        /// <summary>
        /// Calculate distance between Contig Pairs formed in Forward
        /// direction with all paired reads support orientation.
        /// Input : 3-4 Line sequence reads.
        /// Output : Filtered contigs.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep6CalculateDistanceForForwardPairedContigs()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateContigDistance(Constants.FilterPairedReadContigsNode);
            }
        }

        /// <summary>
        /// Calculate distance between Contig Pairs formed in Reverse
        /// direction with all paired reads support orientation.
        /// Input : 3-4 Line sequence reads.
        /// Output : Filtered contigs.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        [Ignore]
        public void ValidatePadenaStep6CalculateDistanceForReversePairedContigs()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateContigDistance(Constants.FilterPairedReadReverseOrietnationContigsNode);
            }
        }

        #endregion PadenaStep6:Step5:TestCases

        #region PadenaStep6:Step6:TestCases

        /// <summary>
        /// Validate scaffold path for Contig Pairs formed in Forward 
        /// direction with all paired reads support orientation
        /// Input : 3-4 Line sequence reads.
        /// Output : Scaffold path 
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        [Ignore]
        public void ValidatePadenaStep6ScaffoldPathForForwardOrientation()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateScaffoldPath(Constants.ScaffoldPathWithOverlapNode);
            }
        }

        /// <summary>
        /// Validate scaffold path for Contig Pairs formed in Reverse 
        /// direction with all paired reads support orientation
        /// Input : 3-4 Line sequence reads.
        /// Output : Scaffold path 
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        [Ignore]
        public void ValidatePadenaStep6ScaffoldPathForReverseOrientation()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateScaffoldPath(Constants.ScaffoldPathWithOverlapReverseNode);
            }
        }

        #endregion PadenaStep6:Step5:TestCases

        #region PadenaStep6:Step7:TestCases

        /// <summary>
        /// Validate merging assembled path for scaffold paths 
        /// which have overalap.
        /// Input : 3-4 Line sequence reads.
        /// Output : Assembled path 
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        [Ignore]
        public void ValidatePadenaStep6AssembledPathWithOverlapContigs()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateAssembledPath(Constants.AssembledPathWithOverlapNode);
            }
        }

        /// <summary>
        /// Validate merging assembled path for scaffold paths which 
        /// have partial overlaps.
        /// Input : 3-4 Line sequence reads.
        /// Output : Assembled path 
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        [Ignore]
        public void ValidatePadenaStep6AssembledPathWithPartialOverlapContigs()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidateAssembledPath(Constants.AssembledPathWithoutOverlap);
            }
        }

        /// <summary>
        /// Validate Assembled sequences for one line reads.
        /// Input : 3-4 Line sequence reads.
        /// Output : Assembled sequences.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaStep6AssembledSequencesWithSmallReads()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidatePadenaAssembledSeqs(
                    Constants.AssembledSequencesForSequenceReadsNode, true, false,
                    false);
            }
        }

        /// <summary>
        /// Validate ParallelDenovoAssembler class properties.
        /// Input : Sequence reads.
        /// Output : Validate ParallerlDenovoAssembler properties.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateParallelDenovoAssemblerProperties()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ParallelDenovoAssemblyProperties(
                    Constants.AssembledSequencesForSequenceReadsNode);
            }
        }

        /// <summary>
        /// Validate sequence contigs for one line reads.
        /// Input : 3-4 Line sequence reads.
        /// Output : Sequence contigs.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaSequenceContigsWithSmallReads()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidatePadenaAssembledSeqs(
                    Constants.AssembledContigsForSequenceReadsNode,
                    false, false, false);
            }
        }

        /// <summary>
        /// Validate sequence contigs for one line reads with Erosion enabled.
        /// Input : 3-4 Line sequence reads.
        /// Output : Sequence contigs.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaAssembledSeqsForSmallReadsWithErosion()
        {
            using (PadenaBvtTest testObj = new PadenaBvtTest())
            {
                testObj.ValidatePadenaAssembledSeqs(
                    Constants.AssembledSequencesForSequenceReadsWithErosionNode,
                    true, false, true);
            }
        }

        /// <summary>
        /// Validate sequence contigs for one line reads with 
        /// Low coverage contig enabled.
        /// Input : 3-4 Line sequence reads.
        /// Output : Sequence contigs.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaAssembledSeqsForSmallReadsWithLowCoverageContig()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidatePadenaAssembledSeqs(
                    Constants.AssembledSequencesForSequenceReadsWithLCCNode,
                    true, true, false);
            }
        }

        /// <summary>
        /// Validate sequence contigs for one line reads with 
        /// Erosion and Low coverage contig enabled.
        /// Input : 3-4 Line sequence reads.
        /// Output : Sequence contigs.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePadenaAssembledSeqsWithErosionAndLowCoverageContig()
        {
            using (PadenaBvtTest testObj = new Padena.PadenaBvtTest())
            {
                testObj.ValidatePadenaAssembledSeqs(
                    Constants.AssembledSequencesForSequenceReadsWithErosionAndLCCNode,
                    true, true, true);
            }
        }

        #endregion PadenaStep6:Step5:TestCases


        #region General Test Cases
        /// <summary>
        /// Validates the ReadContigMap constructor
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateReadContigMap()
        {
            // Read all the input sequences from xml config file
            string filePath = utilityObj.xmlUtil.GetTextValue(Constants.SmallChromosomeReadsNode,
              Constants.FilePathNode);
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();
                ReadContigMap map = new ReadContigMap(sequenceReads);
                Assert.IsNotNull(map);
                for (int i = 0; i < 10; i++)
                {
                    Assert.IsTrue(map.ContainsKey(sequenceReads.ElementAt(0).ID));
                }
            }
        }

        /// <summary>
        /// Validates BuildSequenceFromPath method
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Collections.Generic.List`1<Bio.Algorithms.Assembly.Padena.Scaffold.ScaffoldPath>"), TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateBuildSequenceFromPath()
        {
            const int KmerLength = 7;
            ISequence sequence = new Sequence(Alphabets.DNA, "GATTCAAGGGCTGGGGG");
            ISequence sequenceNew;
            IList<ISequence> contigsSequence = SequenceToKmerBuilder.GetKmerSequences(sequence, KmerLength).ToList();
            using (ContigGraph graph = new ContigGraph())
            {

                graph.BuildContigGraph(contigsSequence, KmerLength);
                List<Node> contigs = graph.Nodes.ToList();
                ScaffoldPath path = new ScaffoldPath();

                foreach (Node node in contigs.GetRange(0, 11))
                {
                    path.Add(new KeyValuePair<Node, Edge>(node, new Edge(true)));
                }
                sequenceNew = path.BuildSequenceFromPath(graph, KmerLength);
            }
            Assert.IsNotNull(sequenceNew);
            Assert.AreEqual((new string(sequenceNew.Select(a => (char)a).ToArray())), "GATTCAAGGGCTGGGGG");
        }

        #endregion General Test Cases
    }

    /// <summary>
    /// The class contains helper methods for Padena assembler.
    /// </summary>
    internal class PadenaBvtTest : ParallelDeNovoAssembler
    {
        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\PadenaTestData\PadenaTestsConfig.xml");

        #endregion Global Variables

        #region HelperMethods

        /// <summary>
        /// Validate ParallelDeNovoAssembler step1 Build kmers 
        /// </summary>
        /// <param name="nodeName">xml node for test data</param>
        internal void ValidateDe2AssemblerBuildKmers(string nodeName)
        {
            // Read all the input sequences from xml config file
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.KmerLengthNode);

            // set kmerLength
            this.KmerLength = int.Parse(kmerLength, (IFormatProvider)null);
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();
                this.SequenceReads.Clear();

                // set all the input reads and execute build kmers
                this.SetSequenceReads(sequenceReads.ToList());
                IEnumerable<KmersOfSequence> lstKmers =
                    (new SequenceToKmerBuilder()).Build(this.SequenceReads,
                    this.KmerLength);

                ValidateKmersList(new List<KmersOfSequence>(lstKmers),
                   new List<ISequence>(sequenceReads), nodeName);
            }

            ApplicationLog.WriteLine(
                @"Padena BVT : Validation of Build with all input reads using 
                    ParallelDeNovoAssembler sequence completed successfully");
        }

        /// <summary>
        /// Validate SequenceRangeToKmerBuilder Build() method which build kmers
        /// </summary>
        /// <param name="nodeName">xml node name for test data</param>
        internal void ValidateKmerBuilderBuild(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(
              nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(
              nodeName, Constants.KmerLengthNode);

            // Get the input reads
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                // Pass all the input reads and kmerLength to generate kmers
                SequenceToKmerBuilder builder = new SequenceToKmerBuilder();
                IEnumerable<KmersOfSequence> lstKmers = builder.Build(sequenceReads,
                  int.Parse(kmerLength, (IFormatProvider)null));

                // Validate kmers list
                ValidateKmersList(new List<KmersOfSequence>(lstKmers),
                    new List<ISequence>(sequenceReads), nodeName);
            }

            ApplicationLog.WriteLine(
                @"Padena BVT : Validation of Build with all input reads 
                    sequence completed successfully");
        }

        /// <summary>
        /// Validate SequenceRangeToKmerBuilder Build() which build kmers 
        /// using one base sequence 
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateKmerBuilderBuildWithSequence(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(
              nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(
              nodeName, Constants.KmerLengthNode);

            // Get the input reads
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                // Pass each input read and kmerLength
                // Add all the generated kmers to kmer list
                SequenceToKmerBuilder builder = new SequenceToKmerBuilder();
                IList<KmersOfSequence> lstKmers = new List<KmersOfSequence>();
                foreach (ISequence sequence in sequenceReads)
                {
                    lstKmers.Add(builder.Build(sequence, int.Parse(kmerLength, (IFormatProvider)null)));
                }

                // Validate all the kmers
                ValidateKmersList(lstKmers, sequenceReads.ToList(), nodeName);
            }

            ApplicationLog.WriteLine(
                @"Padena BVT : Validation of Build with each input read sequence 
                    completed successfully");
        }

        /// <summary>
        /// Validate the generated kmers using expected output kmer file
        /// </summary>
        /// <param name="lstKmers">generated kmers</param>
        /// <param name="inputReads">input base sequence reads</param>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateKmersList(IList<KmersOfSequence> lstKmers,
            IList<ISequence> inputReads, string nodeName)
        {
            string kmerOutputFile = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.KmersOutputFileNode);

            Assert.AreEqual(inputReads.Count(), lstKmers.Count);

            // Get the array of kmer sequence using kmer positions
            for (int kmerIndex = 0; kmerIndex < lstKmers.Count; kmerIndex++)
            {
                Assert.AreEqual(
                    new string(inputReads[kmerIndex].Select(a => (char)a).ToArray()),
                    new string(lstKmers[kmerIndex].BaseSequence.Select(a => (char)a).ToArray()));
            }

            // Validate all the generated kmer sequence with the expected kmer sequence
            using (StreamReader kmerFile = new StreamReader(kmerOutputFile))
            {

                string line = string.Empty;
                List<string[]> fileContent = new List<string[]>();
                while (null != (line = kmerFile.ReadLine()))
                {
                    fileContent.Add(line.Split(','));
                }

                for (int kmerIndex = 0; kmerIndex < lstKmers.Count; kmerIndex++)
                {
                    int count = 0;
                    HashSet<KmersOfSequence.KmerPositions> kmers =
                        lstKmers[kmerIndex].Kmers;

                    foreach (KmersOfSequence.KmerPositions kmer in kmers)
                    {
                        ISequence sequence =
                            lstKmers[kmerIndex].KmerToSequence(kmer);
                        string aab = new string(sequence.Select(a => (char)a).ToArray());
                        Assert.AreEqual(fileContent[kmerIndex][count], aab);
                        count++;
                    }

                }
            }
        }

        /// <summary>
        /// Validate kmersofsequence ctor() by passing kmers, kmer length and input reads
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateKmersOfSequenceCtor(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);

            // Get the input reads and build kmers
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                // Validate the KmersOfSequence ctor by passing build kmers
                // Validate the kmersof sequence instance using GetKmerSequence()
                this.KmerLength = int.Parse(kmerLength, (IFormatProvider)null);
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());
                IEnumerable<KmersOfSequence> lstKmers =
                    (new SequenceToKmerBuilder()).Build(this.SequenceReads, this.KmerLength);
                IList<KmersOfSequence> newKmersList = new List<KmersOfSequence>();
                int index = 0;
                foreach (KmersOfSequence kmer in lstKmers)
                {
                    KmersOfSequence newkmer = new KmersOfSequence(sequenceReads.ElementAt(index),
                      int.Parse(kmerLength, (IFormatProvider)null), kmer.Kmers);
                    newKmersList.Add(newkmer);
                    index++;
                }

                ValidateKmersList(newKmersList, new List<ISequence>(sequenceReads), nodeName);
            }

            ApplicationLog.WriteLine(
                @"Padena BVT : KmersOfSequence ctor validation for 
                    Padena step1 completed successfully");
        }

        /// <summary>
        /// Validate KmersOfSequence ctor by passing base sequence reads, kmer length and
        /// built kmers
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        internal void ValidateKmersOfSequenceCtorWithBuildKmers(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.KmerLengthNode);

            // Get the input reads
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                SequenceToKmerBuilder builder = new SequenceToKmerBuilder();
                IList<KmersOfSequence> lstKmers = new List<KmersOfSequence>();

                // Validate KmersOfSequence ctor using build kmers
                foreach (ISequence sequence in sequenceReads)
                {
                    KmersOfSequence kmer =
                        builder.Build(sequence, int.Parse(kmerLength, (IFormatProvider)null));
                    KmersOfSequence kmerSequence = new KmersOfSequence(sequence,
                      int.Parse(kmerLength, (IFormatProvider)null), kmer.Kmers);
                    lstKmers.Add(kmerSequence);
                }

                ValidateKmersList(lstKmers, new List<ISequence>(sequenceReads), nodeName);
            }

            ApplicationLog.WriteLine(
                @"Padena BVT : KmersOfSequence ctor with build 
                    kmers method validation completed successfully");
        }

        /// <summary>
        /// Validate KmersOfSequence ToSequences() method which returns kmers sequence
        /// using its positions
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateKmersOfSequenceToSequences(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);
            string kmerOutputFile = utilityObj.xmlUtil.GetTextValue(nodeName,
                  Constants.KmersOutputFileNode);

            // Get the input reads and build kmers
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                this.KmerLength = int.Parse(kmerLength, (IFormatProvider)null);
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());
                IList<KmersOfSequence> lstKmers = new List<KmersOfSequence>(
                    (new SequenceToKmerBuilder()).Build(this.SequenceReads, this.KmerLength));

                // Get the array of kmer sequence using ToSequence()
                int index = 0;

                // Validate the generated kmer sequence with the expected output
                using (StreamReader kmerFile = new StreamReader(kmerOutputFile))
                {
                    string line = string.Empty;
                    List<string[]> fileContent = new List<string[]>();
                    while (null != (line = kmerFile.ReadLine()))
                    {
                        fileContent.Add(line.Split(','));
                    }

                    foreach (ISequence sequenceRead in sequenceReads)
                    {
                        int count = 0;
                        KmersOfSequence kmerSequence = new KmersOfSequence(sequenceRead,
                          int.Parse(kmerLength, (IFormatProvider)null), lstKmers[index].Kmers);
                        IEnumerable<ISequence> sequences = kmerSequence.KmersToSequences();
                        foreach (ISequence sequence in sequences)
                        {
                            string aab = new string(sequence.Select(a => (char)a).ToArray());
                            Assert.AreEqual(fileContent[index][count], aab);
                            count++;
                        }
                        index++;
                    }
                }
            }

            ApplicationLog.WriteLine(
                @"Padena BVT : KmersOfSequence ToSequences() method 
                    validation completed successfully");
        }

        /// <summary>
        /// Validate graph generated using ParallelDeNovoAssembler.CreateGraph() with kmers
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateDe2AssemblerBuildGraph(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);

            // Get the input reads and build kmers
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                this.KmerLength = int.Parse(kmerLength, (IFormatProvider)null);
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());

                this.CreateGraph();
                DeBruijnGraph graph = this.Graph;
                ValidateGraph(graph, nodeName);
            }

            ApplicationLog.WriteLine(
                @"Padena BVT : ParallelDeNovoAssembler CreateGraph() validation 
                    for Padena step2 completed successfully");
        }

        /// <summary>
        /// Validate graph generated using DeBruijnGraph.CreateGraph() with kmers
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateDeBruijnGraphBuild(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);

            // Get the input reads and build kmers
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                this.KmerLength = int.Parse(kmerLength, (IFormatProvider)null);
                this.SequenceReads.Clear();
                this.SetSequenceReads(sequenceReads.ToList());
                DeBruijnGraph graph = new DeBruijnGraph(this.KmerLength);
                graph.Build(this.SequenceReads);
                ValidateGraph(graph, nodeName);
            }
            ApplicationLog.WriteLine(@"Padena BVT : DeBruijnGraph Build() validation for Padena step2 completed successfully");

        }

        /// <summary>
        /// Validate the graph nodes sequence, left edges and right edges
        /// </summary>
        /// <param name="graph">graph object</param>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateGraph(DeBruijnGraph graph, string nodeName)
        {
            string nodesSequence = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.NodesSequenceNode);
            string nodesLeftEdges = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.NodesLeftEdgesCountNode);
            string nodesRightEdges = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.NodeRightEdgesCountNode);

            string[] leftEdgesCount = nodesLeftEdges.Split(',');
            string[] rightEdgesCount = nodesRightEdges.Split(',');
            string[] nodesSequences = nodesSequence.Split(',');

            // Validate the nodes 
            for (int iseq = 0; iseq < nodesSequences.Length; iseq++)
            {
                DeBruijnNode dbnodes = graph.GetNodes().FirstOrDefault(
                    n => graph.GetNodeSequence(n).ConvertToString() == nodesSequences[iseq]
                      || graph.GetNodeSequence(n).GetReverseComplementedSequence().ConvertToString() == nodesSequences[iseq]);

                Assert.IsNotNull(dbnodes);

                // Due to parallelization the left edges and right edges count
                // can be swapped while processing. if actual left edges count 
                // is either equal to expected left edges count or right edges count and vice versa.
                Assert.IsTrue(dbnodes.LeftExtensionNodesCount.ToString((IFormatProvider)null) == leftEdgesCount[iseq] ||
                  dbnodes.LeftExtensionNodesCount.ToString((IFormatProvider)null) == rightEdgesCount[iseq]);
                Assert.IsTrue(dbnodes.RightExtensionNodesCount.ToString((IFormatProvider)null) == leftEdgesCount[iseq] ||
                  dbnodes.RightExtensionNodesCount.ToString((IFormatProvider)null) == rightEdgesCount[iseq]);
            }
        }

        /// <summary>
        /// Validate the DeBruijnNode ctor by passing the kmer and validating 
        /// the node object.
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateDeBruijnNodeCtor(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);

            // Get the input reads and build kmers
            using (FastAParser parser = new FastAParser(filePath))
            {
                IEnumerable<ISequence> sequenceReads = parser.Parse();

                // Build the kmers using assembler
                this.KmerLength = int.Parse(kmerLength, null);
                this.SequenceReads.Clear();
                this.SetSequenceReads(sequenceReads.ToList());
                IList<KmersOfSequence> lstKmers = new List<KmersOfSequence>((new SequenceToKmerBuilder()).Build(this.SequenceReads, this.KmerLength));

                // Validate the node creation
                // Create node and add left node.
                ISequence seq = this.SequenceReads.First();
                KmerData32 kmerData = new KmerData32();
                kmerData.SetKmerData(seq, lstKmers[0].Kmers.First().Positions[0], this.KmerLength);

                DeBruijnNode node = new DeBruijnNode(kmerData, 1);
                kmerData = new KmerData32();
                kmerData.SetKmerData(seq, lstKmers[1].Kmers.First().Positions[0], this.KmerLength);

                DeBruijnNode leftnode = new DeBruijnNode(kmerData, 1);
                node.SetExtensionNode(false, true, leftnode);

                Assert.AreEqual(lstKmers[1].Kmers.First().Count, node.LeftExtensionNodesCount);
            }

            ApplicationLog.WriteLine(
                "Padena BVT : DeBruijnNode ctor() validation for Padena step2 completed successfully");
        }

        /// <summary>
        /// Validate AddLeftEndExtension() method of DeBruijnNode 
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateDeBruijnNodeAddLeftExtension(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);

            // Get the input reads and build kmers
            using (FastAParser parser = new FastAParser(filePath))
            {
                IEnumerable<ISequence> sequenceReads = parser.Parse();

                // Build kmers from step1
                this.KmerLength = int.Parse(kmerLength, null);
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());
                IList<KmersOfSequence> lstKmers = new List<KmersOfSequence>((new SequenceToKmerBuilder()).Build(this.SequenceReads, this.KmerLength));

                // Validate the node creation
                // Create node and add left node.
                ISequence seq = this.SequenceReads.First();
                KmerData32 kmerData = new KmerData32();
                kmerData.SetKmerData(seq, lstKmers[0].Kmers.First().Positions[0], this.KmerLength);

                DeBruijnNode node = new DeBruijnNode(kmerData, 1);
                kmerData = new KmerData32();
                kmerData.SetKmerData(seq, lstKmers[1].Kmers.First().Positions[0], this.KmerLength);

                DeBruijnNode leftnode = new DeBruijnNode(kmerData, 1);
                node.SetExtensionNode(false, true, leftnode);
                Assert.AreEqual(lstKmers[1].Kmers.First().Count, node.LeftExtensionNodesCount);
            }

            ApplicationLog.WriteLine(@"Padena BVT :DeBruijnNode AddLeftExtension() validation for Padena step2 completed successfully");
        }

        /// <summary>
        /// Validate AddRightEndExtension() method of DeBruijnNode 
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateDeBruijnNodeAddRightExtension(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);

            // Get the input reads and build kmers
            using (FastAParser parser = new FastAParser(filePath))
            {
                IEnumerable<ISequence> sequenceReads = parser.Parse();

                // Build kmers from step1
                this.KmerLength = int.Parse(kmerLength, null);
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());
                IList<KmersOfSequence> lstKmers = new List<KmersOfSequence>((new SequenceToKmerBuilder()).Build(this.SequenceReads, this.KmerLength));

                // Validate the node creation
                // Create node and add left node.
                ISequence seq = this.SequenceReads.First();
                KmerData32 kmerData = new KmerData32();
                kmerData.SetKmerData(seq, lstKmers[0].Kmers.First().Positions[0], this.KmerLength);

                DeBruijnNode node = new DeBruijnNode(kmerData, 1);
                kmerData = new KmerData32();
                kmerData.SetKmerData(seq, lstKmers[1].Kmers.First().Positions[0], this.KmerLength);

                DeBruijnNode rightNode = new DeBruijnNode(kmerData, 1);
                node.SetExtensionNode(true, true, rightNode);
                Assert.AreEqual(lstKmers[1].Kmers.First().Count, node.RightExtensionNodesCount);
            }

            ApplicationLog.WriteLine(@"Padena BVT :DeBruijnNode AddRightExtension() validation for Padena step2 completed successfully");
        }

        /// <summary>
        /// Validate the ParallelDeNovoAssembler unDangleGraph() method which removes the dangling link
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateDe2AssemblerUnDangleGraph(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);

            // Get the input reads and build kmers
            using (FastAParser parser = new FastAParser(filePath))
            {
                IEnumerable<ISequence> sequenceReads = parser.Parse();

                // Build kmers from step1,graph in step2 
                // and remove the dangling links from graph in step3
                // Validate the graph
                this.KmerLength = int.Parse(kmerLength, null);
                this.SequenceReads.Clear();
                this.SetSequenceReads(sequenceReads.ToList());
                this.CreateGraph();
                this.UnDangleGraph();

                ValidateGraph(this.Graph, nodeName);
            }

            ApplicationLog.WriteLine(@"Padena BVT :ParallelDeNovoAssembler.UndangleGraph() validation for Padena step3 completed successfully");
        }

        /// <summary>
        /// Validate the Padena DetectErrorNodes() method is 
        /// returning dangling nodes as expected 
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidatePadenaDetectErrorNodes(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.KmerLengthNode);
            string danglingSequence = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.DangleNodeSequenceNode);
            string[] expectedDanglings = danglingSequence.Split(',');

            // Get the input reads and build kmers
            using (FastAParser parser = new FastAParser(filePath))
            {
                IEnumerable<ISequence> sequenceReads = parser.Parse();

                // Build kmers from step1,graph in step2 
                // and remove the dangling links from graph in step3
                // Validate the graph
                this.KmerLength = int.Parse(kmerLength, null);
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());
                this.CreateGraph();

                // Find the dangling node
                DanglingLinksPurger danglingLinksPurger = new DanglingLinksPurger(int.Parse(kmerLength, null) + 1);
                DeBruijnPathList danglingnodes = danglingLinksPurger.DetectErroneousNodes(this.Graph);
                foreach (DeBruijnPath dbnodes in danglingnodes.Paths)
                {
                    foreach (DeBruijnNode node in dbnodes.PathNodes)
                    {
                        Assert.IsTrue(expectedDanglings.Contains(Graph.GetNodeSequence(node).ToString()));
                    }
                }
            }

            ApplicationLog.WriteLine(
                @"Padena BVT :DeBruijnGraph.DetectErrorNodes() 
                    validation for Padena step3 completed successfully");
        }

        /// <summary>
        /// Validate RemoveErrorNodes() method is removing dangling nodes from the graph
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidatePadenaRemoveErrorNodes(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);

            // Get the input reads and build kmers
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                // Build kmers from step1,graph in step2 
                // and remove the dangling links from graph in step3
                // Validate the graph
                this.KmerLength = int.Parse(kmerLength, (IFormatProvider)null);
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());
                this.CreateGraph();
                DeBruijnGraph graph = this.Graph;

                // Find the dangling nodes and remove the dangling node
                DanglingLinksPurger danglingLinksPurger =
                    new DanglingLinksPurger(int.Parse(kmerLength, (IFormatProvider)null) + 1);
                DeBruijnPathList danglingnodes =
                    danglingLinksPurger.DetectErroneousNodes(graph);
                danglingLinksPurger.RemoveErroneousNodes(graph, danglingnodes);
                Assert.IsFalse(graph.GetNodes().Contains(danglingnodes.Paths[0].PathNodes[0]));
            }

            ApplicationLog.WriteLine(
                @"Padena BVT :DeBruijnGraph.RemoveErrorNodes() validation 
                    for Padena step3 completed successfully");
        }

        /// <summary>
        /// Validate ParallelDeNovoAssembler.RemoveRedundancy() which removes bubbles formed in the graph
        /// and validate the graph
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateDe2AssemblerRemoveRedundancy(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);

            // Get the input reads and build kmers
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                // Build kmers from step1,graph in step2 
                // Remove the dangling links from graph in step3
                // Remove bubbles from graph in step4
                // Validate the graph
                this.KmerLength = int.Parse(kmerLength, (IFormatProvider)null);
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());
                this.CreateGraph();
                this.UnDangleGraph();
                this.RedundantPathsPurger =
                    new RedundantPathsPurger(int.Parse(kmerLength, (IFormatProvider)null) + 1);
                this.RemoveRedundancy();

                ValidateGraph(this.Graph, nodeName);
            }
            ApplicationLog.WriteLine(
                @"Padena BVT :ParallelDeNovoAssembler.RemoveRedundancy() 
                    validation for Padena step4 completed successfully");
        }

        /// <summary>
        /// Creates RedundantPathPurger instance by passing pathlength and count. Detect 
        /// redundant error nodes and remove these nodes from the graph. Validate the graph.
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateRedundantPathPurgerCtor(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);

            // Get the input reads and build kmers
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                // Build kmers from step1,graph in step2 
                // Remove the dangling links from graph in step3
                // Validate the graph
                this.KmerLength = int.Parse(kmerLength, (IFormatProvider)null);
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());
                this.CreateGraph();
                this.UnDangleGraph();

                // Create RedundantPathPurger instance, detect redundant nodes and remove error nodes
                RedundantPathsPurger redundantPathPurger =
                    new RedundantPathsPurger(int.Parse(kmerLength, (IFormatProvider)null) + 1);
                DeBruijnPathList redundantnodelist =
                    redundantPathPurger.DetectErroneousNodes(this.Graph);
                redundantPathPurger.RemoveErroneousNodes(this.Graph, redundantnodelist);

                ValidateGraph(this.Graph, nodeName);
            }

            ApplicationLog.WriteLine(
                @"Padena BVT :RedundantPathsPurger ctor and methods validation for 
                    Padena step4 completed successfully");
        }

        /// <summary>
        /// Validate ParallelDeNovoAssembler.BuildContigs() by passing graph object
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateDe2AssemblerBuildContigs(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.KmerLengthNode);
            string expectedContigsString = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.ContigsNode);
            string[] expectedContigs = expectedContigsString.Split(',');

            // Get the input reads and build kmers
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                // Build kmers from step1,graph in step2 
                // Remove the dangling links from graph in step3
                // Remove bubbles form the graph in step4 
                // Pass the graph and build contigs
                // Validate the contigs
                this.KmerLength = int.Parse(kmerLength, (IFormatProvider)null);
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());
                this.CreateGraph();
                this.UnDangleGraph();
                this.RedundantPathsPurger =
                    new RedundantPathsPurger(int.Parse(kmerLength, (IFormatProvider)null) + 1);
                this.RemoveRedundancy();
                this.ContigBuilder = new SimplePathContigBuilder();
                IEnumerable<ISequence> contigs = this.BuildContigs();

                for (int index = 0; index < contigs.Count(); index++)
                {
                    Assert.IsTrue(expectedContigs.Contains(
                        new string(contigs.ToList()[index].Select(a => (char)a).ToArray())));
                }
            }
            ApplicationLog.WriteLine(
                @"Padena BVT :ParallelDeNovoAssembler.BuildContigs() 
                    validation for Padena step5 completed successfully");
        }

        /// <summary>
        /// Validate the SimpleContigBuilder Build() method using step 4 graph
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateSimpleContigBuilderBuild(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);
            string expectedContigsString =
                utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ContigsNode);
            string[] expectedContigs = expectedContigsString.Split(',');

            // Get the input reads and build kmers
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                // Build kmers from step1,graph in step2 
                // Remove the dangling links from graph in step3
                // Remove bubbles from graph in step4
                this.KmerLength = int.Parse(kmerLength, (IFormatProvider)null);
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());
                this.CreateGraph();
                this.UnDangleGraph();
                this.RedundantPathsPurger =
                    new RedundantPathsPurger(int.Parse(kmerLength, (IFormatProvider)null) + 1);
                this.RemoveRedundancy();

                // Validate the SimpleContigBuilder.Build() by passing graph
                SimplePathContigBuilder builder = new SimplePathContigBuilder();
                IEnumerable<ISequence> contigs = builder.Build(this.Graph);

                // Validate the contigs
                for (int index = 0; index < contigs.Count(); index++)
                {
                    Assert.IsTrue(expectedContigs.Contains(
                         new string(contigs.ToList()[index].Select(a => (char)a).ToArray())));
                }
            }

            ApplicationLog.WriteLine(
                @"Padena BVT :SimpleContigBuilder.BuildContigs() validation for 
                    Padena step5 completed successfully");
        }

        /// <summary>
        /// Validate generating Map paired reads.
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidatePairedReads(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.FilePathNode);
            string expectedPairedReadsCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.PairedReadsCountNode);
            string[] backwardReadsNode = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.BackwardReadsNode);
            string[] forwardReadsNode = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.ForwardReadsNode);
            string[] expectedLibrary = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.LibraryNode);
            string[] expectedMean = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.MeanLengthNode);
            string[] deviationNode = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.DeviationNode);

            IList<ISequence> sequenceReads = new List<ISequence>();
            IList<MatePair> pairedreads = new List<MatePair>();

            // Get the input reads 
            IEnumerable<ISequence> sequences = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequences = parser.Parse();

                // Convert reads to map paired reads.
                MatePairMapper pair = new MatePairMapper();
                foreach (ISequence seq in sequences)
                {
                    sequenceReads.Add(seq);
                }
                pairedreads = pair.Map(sequenceReads);

                // Validate Map paired reads.
                Assert.AreEqual(expectedPairedReadsCount,
                    pairedreads.Count.ToString((IFormatProvider)null));

                for (int index = 0; index < pairedreads.Count; index++)
                {
                    Assert.IsTrue(
                        forwardReadsNode.Contains(
                        new string(pairedreads[index].GetForwardRead(sequenceReads).Select(a => (char)a).ToArray())));
                    Assert.IsTrue(backwardReadsNode.Contains(
                        new string(pairedreads[index].GetReverseRead(sequenceReads).Select(a => (char)a).ToArray())));
                    Assert.IsTrue(
                        deviationNode.Contains(pairedreads[index].StandardDeviationOfLibrary.ToString((IFormatProvider)null)));
                    Assert.IsTrue(
                        expectedMean.Contains(pairedreads[index].MeanLengthOfLibrary.ToString((IFormatProvider)null)));
                    Assert.IsTrue(
                        expectedLibrary.Contains(pairedreads[index].Library.ToString((IFormatProvider)null)));
                }
            }

            ApplicationLog.WriteLine(@"Padena BVT : Map paired reads has been verified successfully");
        }

        /// <summary>
        /// Validate Add library information to existing libraries.
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void AddLibraryInformation(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.FilePathNode);
            string expectedPairedReadsCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.PairedReadsCountNode);
            string[] backwardReadsNode = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.BackwardReadsNode);
            string[] forwardReadsNode = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.ForwardReadsNode);
            string[] expectedLibrary = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.LibraryNode);
            string[] expectedMean = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.MeanLengthNode);
            string[] deviationNode = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.DeviationNode);
            string libraray = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.LibraryName);
            string StdDeviation = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.StdDeviation);
            string mean = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Mean);


            IList<MatePair> pairedreads = new List<MatePair>();

            // Get the input reads 
            IEnumerable<ISequence> sequences = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequences = parser.Parse();
                IList<ISequence> sequenceReads =
                    new List<ISequence>(sequences);

                // Add a new library infomration.
                CloneLibrary.Instance.AddLibrary(libraray,
                    float.Parse(mean, (IFormatProvider)null), float.Parse(StdDeviation, (IFormatProvider)null));

                // Convert reads to map paired reads. 
                MatePairMapper pair = new MatePairMapper();
                pairedreads = pair.Map(sequenceReads);

                // Validate Map paired reads.
                Assert.AreEqual(expectedPairedReadsCount, pairedreads.Count.ToString((IFormatProvider)null));

                for (int index = 0; index < pairedreads.Count; index++)
                {
                    Assert.IsTrue(forwardReadsNode.Contains(
                        new string(pairedreads[index].GetForwardRead(sequenceReads).Select(a => (char)a).ToArray())));
                    Assert.IsTrue(backwardReadsNode.Contains(
                        new string(pairedreads[index].GetReverseRead(sequenceReads).Select(a => (char)a).ToArray())));
                    Assert.IsTrue(deviationNode.Contains(pairedreads[index].StandardDeviationOfLibrary.ToString((IFormatProvider)null)));
                    Assert.IsTrue(expectedLibrary.Contains(pairedreads[index].Library.ToString((IFormatProvider)null)));
                    Assert.IsTrue(expectedMean.Contains(pairedreads[index].MeanLengthOfLibrary.ToString((IFormatProvider)null)));
                }
            }

            ApplicationLog.WriteLine(@"Padena BVT : Map paired reads has been verified successfully");
        }

        /// <summary>
        /// Validate building map reads to contigs.
        /// </summary>
        /// <param name="nodeName">xml node name used for a different testcases</param>
        /// <param name="IsFullOverlap">True if full overlap else false</param>
        /// //TODO: This test was originally written with hard coded assumptions about the direction of the 
        /// returned reads, currently this test has a hack to "flip" some reads to match these hard coded 
        /// assumptions.  This should be cleaned up.
        internal void ValidateMapReadsToContig(string nodeName, bool IsFullOverlap)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.KmerLengthNode);
            string daglingThreshold = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.DanglingLinkThresholdNode);
            string redundantThreshold = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.RedundantThreshold);
            string readMapLengthString = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.ReadMapLength);
            string readStartPosString = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.ReadStartPos);
            string contigStartPosString = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.ContigStartPos);
            string[] expectedReadmapLength = readMapLengthString.Split(',');
            string[] expectedReadStartPos = readStartPosString.Split(',');
            string[] expectedContigStartPos = contigStartPosString.Split(',');

            // Get the input reads and build kmerssequences
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                // Build kmers from step1,graph in step2 
                // Remove the dangling links from graph in step3
                // Remove bubbles form the graph in step4 
                // Pass the graph and build contigs
                // Validate contig reads
                this.KmerLength = Int32.Parse(kmerLength, (IFormatProvider)null);
                this.DanglingLinksThreshold = Int32.Parse(daglingThreshold, (IFormatProvider)null); ;
                this.DanglingLinksPurger =
                    new DanglingLinksPurger(Int32.Parse(daglingThreshold, (IFormatProvider)null));
                this.RedundantPathsPurger =
                    new RedundantPathsPurger(Int32.Parse(redundantThreshold, (IFormatProvider)null));
                this.RedundantPathLengthThreshold = Int32.Parse(redundantThreshold, (IFormatProvider)null);
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());
                this.CreateGraph();
                this.UnDangleGraph();
                this.ContigBuilder = new SimplePathContigBuilder();
                this.RemoveRedundancy();

                //IList<ISequence> contigs = this.BuildContigs().ToList();

                IList<ISequence> listContigs = this.BuildContigs().ToList();
                //Hack to satisfy the assumptions of one test by flipping the read to its reverse complement
                if (nodeName == Constants.MapReadsToContigFullOverlapNode)
                {
                    listContigs[0] = (listContigs[0] as Sequence).GetReverseComplementedSequence();
                }

                IList<ISequence> sortedContigs = SortContigsData(listContigs);
                ReadContigMapper mapper = new ReadContigMapper();
                ReadContigMap maps = mapper.Map(sortedContigs, sequenceReads, this.KmerLength);
                Assert.AreEqual(maps.Count, sequenceReads.Count());

                Dictionary<ISequence, IList<ReadMap>> readMaps = maps[sequenceReads.ElementAt(0).ID];
                IList<ReadMap> readMap = null;

                for (int i = 0; i < sortedContigs.Count; i++)
                {
                    readMap = readMaps[sortedContigs[i]];
                    if (IsFullOverlap)
                    {
                        Assert.AreEqual(expectedReadmapLength[i], readMap[0].Length.ToString((IFormatProvider)null), "readMap failed for pos " + i);
                        Assert.AreEqual(expectedContigStartPos[i], readMap[0].StartPositionOfContig.ToString((IFormatProvider)null), "contigStart failed for pos " + i);
                        Assert.AreEqual(expectedReadStartPos[i], readMap[0].StartPositionOfRead.ToString((IFormatProvider)null), "readStart failed for pos " + i);
                        Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.FullOverlap);
                    }
                    else
                    {
                        Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.PartialOverlap);
                        break;
                    }
                }
            }

            ApplicationLog.WriteLine(
                "Padena BVT :ReadContigMapper.Map() validation for Padena step6:step2 completed successfully");
        }

        /// <summary>
        /// Validate Filter contig nodes.
        /// </summary>
        /// <param name="nodeName">xml node name used for a differnt testcase.</param>
        internal void ValidateFilterPaired(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.KmerLengthNode);
            string daglingThreshold = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DanglingLinkThresholdNode);
            string redundantThreshold = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.RedundantThreshold);
            string expectedContigPairedReadsCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ContigPairedReadsCount);
            string forwardReadStartPos = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ForwardReadStartPos);
            string reverseReadStartPos = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ReverseReadStartPos);
            string reverseComplementStartPos = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.RerverseReadReverseCompPos);
            string[] expectedForwardReadStartPos = forwardReadStartPos.Split(',');
            string[] expectedReverseReadStartPos = reverseReadStartPos.Split(',');
            string[] expectedReverseComplementStartPos = reverseComplementStartPos.Split(',');

            // Get the input reads and build kmers
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                // Build kmers from step1,graph in step2 
                // Remove the dangling links from graph in step3
                // Remove bubbles form the graph in step4 
                // Pass the graph and build contigs
                // Validate contig reads
                this.KmerLength = Int32.Parse(kmerLength, (IFormatProvider)null);
                this.DanglingLinksThreshold = Int32.Parse(daglingThreshold, (IFormatProvider)null); ;
                this.DanglingLinksPurger =
                    new DanglingLinksPurger(Int32.Parse(daglingThreshold, (IFormatProvider)null));
                this.RedundantPathsPurger =
                    new RedundantPathsPurger(Int32.Parse(redundantThreshold, (IFormatProvider)null));
                this.RedundantPathLengthThreshold = Int32.Parse(redundantThreshold, (IFormatProvider)null);
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());
                this.CreateGraph();
                this.UnDangleGraph();

                // Build contig.
                this.ContigBuilder = new SimplePathContigBuilder();
                this.RemoveRedundancy();
                IEnumerable<ISequence> contigs = this.BuildContigs();
                IList<ISequence> listContigs = contigs.ToList();

                IList<ISequence> sortedContigs = SortContigsData(listContigs);
                ReadContigMapper mapper = new ReadContigMapper();

                ReadContigMap maps = mapper.Map(sortedContigs, sequenceReads, this.KmerLength);

                // Find map paired reads.
                MatePairMapper mapPairedReads = new MatePairMapper();
                ContigMatePairs pairedReads = mapPairedReads.MapContigToMatePairs(sequenceReads,
                    maps);

                // Filter paired reads based on the contig orientation.
                OrientationBasedMatePairFilter filter =
                    new OrientationBasedMatePairFilter();
                ContigMatePairs contigpairedReads = null;

                contigpairedReads = filter.FilterPairedReads(pairedReads, 0);

                Assert.AreEqual(expectedContigPairedReadsCount,
                    contigpairedReads.Values.Count.ToString((IFormatProvider)null));
                ISequence sortedContig = sortedContigs[0];
                ISequence secondSortedContig = sortedContigs[1];

                // Validate Contig paired reads after filtering contig sequences.
                Dictionary<ISequence, IList<ValidMatePair>> map = null;
                IList<ValidMatePair> valid = null;

                if (contigpairedReads.ContainsKey(sortedContig))
                {
                    map = contigpairedReads[sortedContig];
                    valid = SortPairedReads(map[secondSortedContig], sequenceReads.ToList());
                }
                else
                {
                    map = contigpairedReads[secondSortedContig];
                    valid = SortPairedReads(map[sortedContig], sequenceReads.ToList());
                }
                for (int index = 0; index < valid.Count; index++)
                {
                    if (valid[index].ForwardReadStartPosition.Count > 1)
                    {
                        Assert.IsTrue((expectedForwardReadStartPos[index] ==
                          valid[index].ForwardReadStartPosition[0].ToString((IFormatProvider)null)
                          || (expectedForwardReadStartPos[index] ==
                          valid[index].ForwardReadStartPosition[1].ToString((IFormatProvider)null))));
                    }
                    
                    if (valid[index].ReverseReadReverseComplementStartPosition.Count > 1)
                    {
                        Assert.IsTrue((expectedReverseReadStartPos[index] ==
                          valid[index].ReverseReadReverseComplementStartPosition[0].ToString((IFormatProvider)null)
                          || (expectedReverseReadStartPos[index] ==
                          valid[index].ReverseReadReverseComplementStartPosition[1].ToString((IFormatProvider)null))));
                    }

                    if (valid[index].ReverseReadStartPosition.Count > 1)
                    {
                        Assert.IsTrue((expectedReverseComplementStartPos[index] ==
                          valid[index].ReverseReadStartPosition[0].ToString((IFormatProvider)null)
                          || (expectedReverseComplementStartPos[index] ==
                          valid[index].ReverseReadStartPosition[1].ToString((IFormatProvider)null))));
                    }
                }
            }

            ApplicationLog.WriteLine(
                "Padena BVT : FilterPairedReads() validation for Padena step6:step4 completed successfully");
        }

        /// <summary>
        /// Validate FilterPairedRead.FilterPairedRead() by passing graph object
        /// </summary>
        /// <param name="nodeName">xml node name used for a differnt testcase.</param>
        internal void ValidateContigDistance(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.KmerLengthNode);
            string daglingThreshold = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DanglingLinkThresholdNode);
            string redundantThreshold = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.RedundantThreshold);
            string expectedContigPairedReadsCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ContigPairedReadsCount);
            string distanceBetweenFirstContigs = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DistanceBetweenFirstContig);
            string distanceBetweenSecondContigs = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DistanceBetweenSecondContig);
            string firstStandardDeviation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FirstContigStandardDeviation);
            string secondStandardDeviation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.SecondContigStandardDeviation);

            // Get the input reads and build kmers
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                // Build kmers from step1,graph in step2 
                // Remove the dangling links from graph in step3
                // Remove bubbles form the graph in step4 
                // Pass the graph and build contigs
                // Validate contig reads
                this.KmerLength = Int32.Parse(kmerLength, (IFormatProvider)null);
                this.DanglingLinksThreshold = Int32.Parse(daglingThreshold, (IFormatProvider)null); ;
                this.DanglingLinksPurger =
                    new DanglingLinksPurger(Int32.Parse(daglingThreshold, (IFormatProvider)null));
                this.RedundantPathsPurger =
                    new RedundantPathsPurger(Int32.Parse(redundantThreshold, (IFormatProvider)null));
                this.RedundantPathLengthThreshold = Int32.Parse(redundantThreshold, (IFormatProvider)null);
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());
                this.CreateGraph();
                this.UnDangleGraph();

                // Build contig.
                this.ContigBuilder = new SimplePathContigBuilder();
                this.RemoveRedundancy();
                IEnumerable<ISequence> contigs = this.BuildContigs();

                IList<ISequence> listContigs = contigs.ToList();

                IList<ISequence> sortedContigs = SortContigsData(listContigs);
                ReadContigMapper mapper = new ReadContigMapper();

                ReadContigMap maps = mapper.Map(sortedContigs, sequenceReads, this.KmerLength);

                // Find map paired reads.
                MatePairMapper mapPairedReads = new MatePairMapper();
                ContigMatePairs pairedReads = mapPairedReads.MapContigToMatePairs(sequenceReads, maps);

                // Filter contigs based on the orientation.
                OrientationBasedMatePairFilter filter = new OrientationBasedMatePairFilter();
                ContigMatePairs contigpairedReads = filter.FilterPairedReads(pairedReads, 0);

                // Calculate the distance between contigs.
                DistanceCalculator calc = new DistanceCalculator(contigpairedReads);
                calc.CalculateDistance();
                Assert.AreEqual(expectedContigPairedReadsCount,
                    contigpairedReads.Values.Count.ToString((IFormatProvider)null));

                Dictionary<ISequence, IList<ValidMatePair>> map;
                IList<ValidMatePair> valid;
                ISequence sortedContig = sortedContigs[0];
                ISequence secondSortedContig = sortedContigs[1];
                if (contigpairedReads.ContainsKey(sortedContig))
                {
                    map = contigpairedReads[sortedContig];
                    valid = map[secondSortedContig];
                }
                else
                {
                    map = contigpairedReads[secondSortedContig];
                    valid = map[sortedContig];
                }

                // Validate distance and standard deviation between contigs.
                Assert.AreEqual(float.Parse(distanceBetweenFirstContigs, (IFormatProvider)null),
                    valid.First().DistanceBetweenContigs[0]);
                Assert.AreEqual(float.Parse(distanceBetweenSecondContigs, (IFormatProvider)null),
                    valid.First().DistanceBetweenContigs[1]);
                Assert.AreEqual(float.Parse(firstStandardDeviation, (IFormatProvider)null),
                    valid.First().StandardDeviation[0]);
                Assert.AreEqual(float.Parse(secondStandardDeviation, (IFormatProvider)null),
                    valid.First().StandardDeviation[1]);
            }

            ApplicationLog.WriteLine(
                "Padena BVT : DistanceCalculator() validation for Padena step6:step5 completed successfully");
        }

        /// <summary>
        /// Validate scaffold paths for a given input reads.
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateScaffoldPath(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);
            string daglingThreshold = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.DanglingLinkThresholdNode);
            string redundantThreshold = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.RedundantThreshold);
            string[] expectedScaffoldNodes = utilityObj.xmlUtil.GetTextValues(nodeName, Constants.ScaffoldNodes);
            string libraray = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.LibraryName);
            string stdDeviation = utilityObj.xmlUtil.GetTextValue(nodeName,Constants.StdDeviation);
            string mean = utilityObj.xmlUtil.GetTextValue(nodeName,Constants.Mean);
            string expectedDepth = utilityObj.xmlUtil.GetTextValue(nodeName,Constants.DepthNode);
            string expectedScaffoldPathCount = utilityObj.xmlUtil.GetTextValue(nodeName,Constants.ScaffoldPathCount);

            // Get the input reads and build kmers
            using (FastAParser parser = new FastAParser(filePath))
            {
                IEnumerable<ISequence> sequenceReads = parser.Parse();

                // Build kmers from step1,graph in step2 
                // Remove the dangling links from graph in step3
                // Remove bubbles form the graph in step4 
                // Pass the graph and build contigs
                // Validate contig reads
                this.KmerLength = Int32.Parse(kmerLength, null);
                this.DanglingLinksThreshold = Int32.Parse(daglingThreshold, null); 
                this.DanglingLinksPurger = new DanglingLinksPurger(Int32.Parse(daglingThreshold, null));
                this.RedundantPathsPurger = new RedundantPathsPurger(Int32.Parse(redundantThreshold, null));
                this.RedundantPathLengthThreshold = Int32.Parse(redundantThreshold, null);
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());
                this.CreateGraph();
                ContigGraph graph = new ContigGraph();

                this.UnDangleGraph();

                // Build contig.
                this.ContigBuilder = new SimplePathContigBuilder();
                this.RemoveRedundancy();
                IEnumerable<ISequence> contigs = this.BuildContigs();
                IList<ISequence> listContigs = contigs.ToList();
                IList<ISequence> sortedContigs = SortContigsData(listContigs);
                ReadContigMapper mapper = new ReadContigMapper();

                ReadContigMap maps = mapper.Map(sortedContigs, sequenceReads, this.KmerLength);

                // Find map paired reads.
                CloneLibrary.Instance.AddLibrary(libraray, float.Parse(mean, null), float.Parse(stdDeviation, null));
                MatePairMapper mapPairedReads = new MatePairMapper();
                ContigMatePairs pairedReads = mapPairedReads.MapContigToMatePairs(sequenceReads, maps);

                // Filter contigs based on the orientation.
                OrientationBasedMatePairFilter filter = new OrientationBasedMatePairFilter();
                ContigMatePairs contigpairedReads = filter.FilterPairedReads(pairedReads, 0);

                DistanceCalculator dist = new DistanceCalculator(contigpairedReads);
                dist.CalculateDistance();

                graph.BuildContigGraph(contigs.ToList(), this.KmerLength);

                // Validate ScaffoldPath using BFS.
                TracePath trace = new TracePath();
                IList<ScaffoldPath> paths = trace.FindPaths(graph, contigpairedReads, Int32.Parse(kmerLength, null),
                                                            Int32.Parse(expectedDepth, null));

                ScaffoldPath scaffold = paths.First();
                Assert.AreEqual(expectedScaffoldPathCount, paths.Count.ToString((IFormatProvider)null));

                foreach (KeyValuePair<Node, Edge> kvp in scaffold)
                {
                    string sequence = graph.GetNodeSequence(kvp.Key).ConvertToString();
                    Assert.IsTrue(expectedScaffoldNodes.Contains(sequence), "Missing " + sequence);
                }
            }

            ApplicationLog.WriteLine("Padena BVT : FindPaths() validation for Padena step6:step6 completed successfully");
        }

        /// <summary>
        /// Validate Assembled paths for a given input reads.
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateAssembledPath(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);
            string daglingThreshold = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.DanglingLinkThresholdNode);
            string redundantThreshold = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.RedundantThreshold);
            string libraray = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.LibraryName);
            string stdDeviation = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.StdDeviation);
            string mean = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Mean);
            string expectedDepth = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.DepthNode);
            string expectedScaffoldPathCount = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ScaffoldPathCount);
            string[] assembledPath = utilityObj.xmlUtil.GetTextValues(nodeName, Constants.SequencePathNode);

            // Get the input reads and build kmers
            using (FastAParser parser = new FastAParser(filePath))
            {
                IEnumerable<ISequence> sequenceReads = parser.Parse();

                // Build kmers from step1,graph in step2 
                // Remove the dangling links from graph in step3
                // Remove bubbles form the graph in step4 
                // Pass the graph and build contigs
                // Validate contig reads
                this.KmerLength = Int32.Parse(kmerLength, null);
                this.DanglingLinksThreshold = Int32.Parse(daglingThreshold, null); ;
                this.DanglingLinksPurger = new DanglingLinksPurger(Int32.Parse(daglingThreshold, null));
                this.RedundantPathsPurger = new RedundantPathsPurger(Int32.Parse(redundantThreshold, null));
                this.RedundantPathLengthThreshold = Int32.Parse(redundantThreshold, (IFormatProvider)null);
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());
                this.CreateGraph();
                ContigGraph graph = new ContigGraph();
                this.UnDangleGraph();

                // Build contig.
                this.ContigBuilder = new SimplePathContigBuilder();
                this.RemoveRedundancy();
                IEnumerable<ISequence> contigs = this.BuildContigs();
                IList<ISequence> listContigs = contigs.ToList();

                IList<ISequence> sortedContigs = SortContigsData(listContigs);
                ReadContigMapper mapper = new ReadContigMapper();

                ReadContigMap maps = mapper.Map(sortedContigs, sequenceReads, this.KmerLength);

                // Find map paired reads.
                CloneLibrary.Instance.AddLibrary(libraray, float.Parse(mean, null), float.Parse(stdDeviation, null));
                MatePairMapper mapPairedReads = new MatePairMapper();
                ContigMatePairs pairedReads = mapPairedReads.MapContigToMatePairs(sequenceReads, maps);

                // Filter contigs based on the orientation.
                OrientationBasedMatePairFilter filter = new OrientationBasedMatePairFilter();
                ContigMatePairs contigpairedReads = filter.FilterPairedReads(pairedReads, 0);

                DistanceCalculator dist = new DistanceCalculator(contigpairedReads);
                dist.CalculateDistance();

                graph.BuildContigGraph(contigs.ToList(), this.KmerLength);

                // Validate ScaffoldPath using BFS.
                TracePath trace = new TracePath();
                IList<ScaffoldPath> paths = trace.FindPaths(graph, contigpairedReads, Int32.Parse(kmerLength, null), Int32.Parse(expectedDepth, null));

                Assert.AreEqual(expectedScaffoldPathCount, paths.Count.ToString((IFormatProvider)null));

                // Assemble paths.
                PathPurger pathsAssembler = new PathPurger();
                pathsAssembler.PurgePath(paths);

                // Get sequences from assembled path.
                IList<ISequence> seqList = paths.Select(temp => temp.BuildSequenceFromPath(graph, Int32.Parse(kmerLength, null))).ToList();

                // Validate assembled sequence paths.
                foreach (string sequence in seqList.Select(t => t.ConvertToString()))
                {
                    Assert.IsTrue(assembledPath.Contains(sequence), "Could not locate " + sequence);
                }
            }

            ApplicationLog.WriteLine(
                "Padena BVT : AssemblePath() validation for Padena step6:step7 completed successfully");
        }

        /// <summary>
        /// Validate Parallel Denovo Assembly Assembled sequences.
        /// </summary>
        /// <param name="nodeName">XML node used to validate different test scenarios</param>
        /// <param name="isScaffold"></param>
        /// <param name="enableLowerContigRemoval"></param>
        /// <param name="allowErosion"></param>
        internal void ValidatePadenaAssembledSeqs(string nodeName,
            bool isScaffold, bool enableLowerContigRemoval, bool allowErosion)
        {
            // Get values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);
            string daglingThreshold = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.DanglingLinkThresholdNode);
            string redundantThreshold = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.RedundantThreshold);
            string library = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.LibraryName);
            string stdDeviation = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.StdDeviation);
            string mean = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Mean);
            string erosionThreshold = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ErosionNode);
            string lowCCThreshold = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.LowCoverageContigNode);
            string expectedSequences = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequencePathNode);

            // Get the input reads and build kmers
            using (FastAParser parser = new FastAParser(filePath))
            {
                IEnumerable<ISequence> sequenceReads = parser.Parse();

                // Create a ParallelDeNovoAssembler instance.
                ParallelDeNovoAssembler assembler = null;
                try
                {
                    assembler = new ParallelDeNovoAssembler
                    {
                        KmerLength = Int32.Parse(kmerLength, null),
                        DanglingLinksThreshold = Int32.Parse(daglingThreshold, null),
                        RedundantPathLengthThreshold = Int32.Parse(redundantThreshold, null)
                    };

                    if (enableLowerContigRemoval)
                    {
                        assembler.AllowLowCoverageContigRemoval = enableLowerContigRemoval;
                        assembler.ContigCoverageThreshold = double.Parse(lowCCThreshold, null);
                    }

                    if (allowErosion)
                    {
                        assembler.AllowErosion = true;
                        assembler.ErosionThreshold = Int32.Parse(erosionThreshold, null);
                    }

                    CloneLibrary.Instance.AddLibrary(library, float.Parse(mean, null),
                        float.Parse(stdDeviation, null));

                    IDeNovoAssembly assembly = assembler.Assemble(sequenceReads.ToList(), isScaffold);
                    IList<ISequence> assembledSequenceList = assembly.AssembledSequences.ToList();

                    HashSet<string> expected = new HashSet<string>(expectedSequences.Split(',').Select(s => s.Trim()));
                    AlignmentHelpers.CompareSequenceLists(expected, assembledSequenceList);

                    ApplicationLog.WriteLine("Padena BVT : Assemble() validation for Padena step6:step7 completed successfully");
                }
                finally
                {
                    if (assembler != null)
                        assembler.Dispose();
                }
            }
        }

        /// <summary>
        /// Validate ParallelDenovoAssembler class properties.
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ParallelDenovoAssemblyProperties(string nodeName)
        {
            // Get values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);
            string library = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.LibraryName);
            string StdDeviation = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.StdDeviation);
            string mean = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Mean);

            // Get the input reads and build kmers
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                // Build kmers from step1,graph in step2 
                // Remove the dangling links from graph in step3
                // Remove bubbles form the graph in step4 
                // Pass the graph and build contigs
                // Validate the contigs
                this.KmerLength = int.Parse(kmerLength, (IFormatProvider)null);
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());
                this.CreateGraph();
                this.UnDangleGraph();
                this.RedundantPathsPurger =
                    new RedundantPathsPurger(int.Parse(kmerLength, (IFormatProvider)null) + 1);
                this.RemoveRedundancy();
                this.ContigBuilder = new SimplePathContigBuilder();

                // Build contigs
                IEnumerable<ISequence> contigs = this.BuildContigs();

                CloneLibraryInformation cloneLibInfoObj = new CloneLibraryInformation();
                cloneLibInfoObj.LibraryName = library;
                cloneLibInfoObj.MeanLengthOfInsert = float.Parse(mean, (IFormatProvider)null);
                cloneLibInfoObj.StandardDeviationOfInsert = float.Parse(StdDeviation, (IFormatProvider)null);

                // Build scaffolds.
                CloneLibrary.Instance.AddLibrary(library, float.Parse(mean, (IFormatProvider)null),
                float.Parse(StdDeviation, (IFormatProvider)null));

                IEnumerable<ISequence> scaffolds = BuildScaffolds(contigs.ToList());
                PadenaAssembly denovoAssembly = new PadenaAssembly();

                denovoAssembly.AddContigs(contigs);
                denovoAssembly.AddScaffolds(scaffolds);

                Assert.AreEqual(denovoAssembly.ContigSequences.Count(),
                    contigs.Count());
                Assert.AreEqual(denovoAssembly.Scaffolds.Count(), scaffolds.Count());
                Assert.IsNull(denovoAssembly.Documentation);

                // Validates the Clone Library for the existing clone
                CloneLibraryInformation actualObj = CloneLibrary.Instance.GetLibraryInformation(library);
                Assert.IsTrue(actualObj.Equals(cloneLibInfoObj));

                ApplicationLog.WriteLine("CloneLibraryInformation Equals() is successfully validated");
            }

            // Validate ParallelDenovoAssembler properties.
            ApplicationLog.WriteLine(
                @"Padena BVT : Validated ParallelDenovo Assembly properties");
        }

        /// <summary>
        /// Sort Contig List based on the contig sequence
        /// </summary>
        /// <param name="contigsList">xml node name used for different testcases</param>
        static IList<ISequence> SortContigsData(IEnumerable<ISequence> contigsList)
        {
            return (contigsList.OrderBy(contigData => contigData.ToString())).ToList();
        }

        ///<summary>
        /// Sort Valid Paired reads based on forward reads.
        /// For consistent output due to parallel implementation.
        /// </summary>
        /// <param name="list">List of Paired Reads</param>
        /// <param name="reads">Input list of reads.</param>
        /// <returns>Sorted List of Paired reads</returns>
        static IList<ValidMatePair> SortPairedReads(IEnumerable<ValidMatePair> list, IEnumerable<ISequence> reads)
        {
            return (list.OrderBy(valid => valid.PairedRead.GetForwardRead(reads).ToString())).ToList();
        }

        #endregion
    }
}
