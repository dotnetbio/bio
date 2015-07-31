/****************************************************************************
 * PadenaP1TestCases.cs
 * 
 *  This file contains the Padena P1 test cases.
 * 
***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bio.Algorithms.Assembly.Padena.Scaffold.ContigOverlapGraph;
using Bio.Algorithms.Assembly;
using Bio.Algorithms.Assembly.Graph;
using Bio.Algorithms.Assembly.Padena;
using Bio.Algorithms.Assembly.Padena.Scaffold;
using Bio.Algorithms.Kmer;
using Bio.Extensions;
using Bio.IO.FastA;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using Bio.Tests.Framework;

namespace Bio.TestAutomation.Algorithms.Assembly.Padena
{
    /// <summary>
    /// The class contains P1 test cases to confirm Padena.
    /// 
    /// Note: Due to all the optimizations in PADENA, step 6 no longer works. 
    /// Skipping those tests pending a fix.
    /// </summary>
    [TestClass]
    public class PadenaP1TestCases
    {

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static PadenaP1TestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion

        #region PadenaStep1TestCases

        /// <summary>
        /// Validate ParallelDeNovothis is building valid kmers 
        /// using virul genome input reads in a fasta file and kmerLength 28
        /// Input : virul genome input reads and kmerLength 28
        /// Output : kmers sequence
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep1BuildKmersForViralGenomeReads()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaBuildKmers(Constants.ViralGenomeReadsNode, true);
            }
        }

        /// <summary>
        /// Validate ParallelDeNovothis is building valid kmers 
        /// using input reads which contains sequence and reverse complement
        /// Input : input reads with reverse complement and kmerLength 20
        /// Output : kmers sequence
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep1BuildKmersWithRCReads()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaBuildKmers(Constants.OneLineReadsWithRCNode, false);
            }
        }

        /// <summary>
        /// Validate ParallelDeNovothis is building valid kmers 
        /// using input reads which will generate clusters in step2 graph
        /// Input : input reads which will generate clusters and kmerLength 7
        /// Output : kmers sequence
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep1BuildKmersWithClusters()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaBuildKmers(Constants.OneLineReadsWithClustersNode, false);
            }
        }

        /// <summary>
        /// Validate KmersOfSequence ctor (sequence, length) by passing
        /// one line sequence and kmer length 4
        /// Input : Build kmeres from one line input reads of small size 
        /// chromosome sequence and kmerLength 4
        /// Output : kmers of sequence object with build kmers
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep1KmersOfSequenceCtorWithBuildKmers()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateKmersOfSequenceCtorWithBuildKmers(Constants.OneLineReadsNode);
            }
        }

        /// <summary>
        /// Validate KmersOfSequence ctor (sequence, length, set of kmers) 
        /// by passing small size chromsome sequence and kmer length 28
        /// after building kmers
        /// Input : Build kmeres from one line input reads of small size 
        /// chromosome sequence and kmerLength 28
        /// Output : kmers of sequence object with build kmers
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep1KmersOfSequenceCtorWithBuildKmersForSmallSizeSequences()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateKmersOfSequenceCtorWithBuildKmers(Constants.OneLineReadsNode);
            }
        }

        /// <summary>
        /// Validate KmersOfSequence properties
        /// Input : Build kmeres from 4000 input reads of small size 
        /// chromosome sequence and kmerLength 4 
        /// Output : kmers sequence
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateKmersOfSequenceCtrproperties()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateKmersOfSequenceCtorProperties(Constants.OneLineReadsNode);
            }
        }

        /// <summary>
        /// Validate KmersOfSequence ToSequences() method using small size reads
        /// Input : Build kmeres from 4000 input reads of small size 
        /// chromosome sequence and kmerLength 28 
        /// Output : kmers sequence
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep1KmersOfSequenceToSequencesUsingSmallSizeReads()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateKmersOfSequenceToSequences(Constants.OneLineReadsNode);
            }
        }



        #endregion

        #region PadenaStep2TestCases

        /// <summary>
        /// Validate Graph after building it using build kmers 
        /// with virul genome reads and kmerLength 28
        /// Input: kmers
        /// Output: Graph
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep2BuildGraphForVirulGenome()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaBuildGraph(Constants.ViralGenomeReadsNode, true);
            }
        }


        /// <summary>
        /// Validate Graph after building it using build kmers 
        /// with input reads contains reverse complement and kmerLength 20
        /// Input: kmers
        /// Output: Graph
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep2BuildGraphWithRCReads()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaBuildGraph(Constants.OneLineWithRCStep2Node, false);
            }
        }

        /// <summary>
        /// Validate Graph after building it using build kmers 
        /// with input reads which will generate clusters in step2 graph
        /// Input: kmers
        /// Output: Graph
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep2BuildGraphWithClusters()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaBuildGraph(Constants.OneLineReadsWithClustersNode, false);
            }
        }

        /// <summary>
        /// Validate Graph after building it using build kmers 
        /// with input reads which will generate clusters in step2 graph
        /// Input: kmers
        /// Output: Graph
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep2BuildGraphWithSmallSizeReads()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaBuildGraph(Constants.SmallChromosomeReadsNode, true);
            }
        }

        ///<summary>
        /// Validate Validate DeBruijinGraph properties
        /// Input: kmers
        /// Output: Graph
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep2DeBruijinGraphProperties()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateDeBruijinGraphproperties(Constants.OneLineStep2GraphNode);
            }
        }

        ///<summary>
        /// Validate Validate DeBruijinGraph properties for small size sequence reads.
        /// Input: kmers
        /// Output: Graph
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep2DeBruijinGraphPropertiesForSmallSizeRC()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateDeBruijinGraphproperties(Constants.OneLineReadsWithRCNode);
            }
        }

        /// <summary>
        /// Validate DeBruijinNode ctor by passing dna 
        /// kmersof sequence and graph object of chromosome
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep2DeBruijinCtrByPassingOneLineRC()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateDeBruijnNodeCtor(Constants.OneLineStep2GraphNode);
            }
        }

        /// <summary>
        /// Validate AddLeftExtension() method by 
        /// passing node object and orinetation 
        /// with chromosome read
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep2DeBruijnNodeAddLeftExtensionWithReads()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateDeBruijnNodeAddLeftExtension(Constants.OneLineWithRCStep2Node);
            }
        }

        /// <summary>
        /// Create dbruijn node by passing kmer and create another node.
        /// Add new node as leftendextension of first node. Validate the 
        /// AddRightEndExtension() method.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep2DeBruijnNodeAddRightExtensionWithRCReads()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateDeBruijnNodeAddRightExtension(Constants.OneLineWithRCStep2Node);
            }
        }

        /// <summary>
        /// Validate RemoveExtension() method by passing node 
        /// object and orientation with one line read
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep2DeBruijnNodeRemoveExtensionWithOneLineReads()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateDeBruijnNodeRemoveExtension(Constants.OneLineWithRCStep2Node);
            }
        }

        #endregion

        #region PadenaStep3TestCases

        /// <summary>
        /// Validate the Padena step3 
        /// which removes dangling links from the graph using reads with rc kmers
        /// Input: Graph with dangling links
        /// Output: Graph without any dangling links
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep3UndangleGraphWithRCReads()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaUnDangleGraph(Constants.OneLineWithRCStep2Node, true, false);
            }
        }

        /// <summary>
        /// Validate the Padena step3 
        /// which removes dangling links from the graph using virul genome kmers
        /// Input: Graph with dangling links
        /// Output: Graph without any dangling links
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep3UndangleGraphForViralGenomeReads()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaUnDangleGraph(Constants.ViralGenomeReadsNode, true, true);
            }
        }

        /// <summary>
        /// Validate the Padena step3 using input reads which will generate clusters in step2 graph
        /// Input: Graph with dangling links
        /// Output: Graph without any dangling links
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep3UndangleGraphWithClusters()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaUnDangleGraph(Constants.OneLineReadsWithClustersAfterDangling, false, false);
            }
        }

        /// <summary>
        /// Validate removal of dangling links by passing input reads with 3 dangling links
        /// Input: Graph with 3 dangling links
        /// Output: Graph without any dangling links
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep3UndangleGraphWithDanglingLinks()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaUnDangleGraph(Constants.ReadsWithDanglingLinksNode, false, false);
            }
        }

        /// <summary>
        /// Validate removal of dangling links by passing input reads with 3 dangling links
        /// Input: Graph with 3 dangling links
        /// Output: Graph without any dangling links
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep3UndangleGraphWithMultipleDanglingLinks()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaUnDangleGraph(Constants.ReadsWithMultipleDanglingLinksNode, false, false);
            }
        }


        /// <summary>
        /// Validate the DanglingLinksPurger is removing the dangling link nodes
        /// from the graph
        /// Input: Graph and dangling node
        /// Output: Graph without any dangling nodes
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep3RemoveErrorNodesForSmallSizeReads()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaRemoveErrorNodes(Constants.ViralGenomeReadsNode);
            }
        }

        #endregion

        #region PadenaStep4TestCases

        /// <summary>
        /// Validate Padena step4 ParallelDeNovothis.RemoveRedundancy() by passing graph 
        /// using virul genome reads such that it will create bubbles in the graph
        /// Input: Graph with bubbles
        /// Output: Graph without bubbles
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep4RemoveRedundancyForViralGenomeReads()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaRemoveRedundancy(Constants.ViralGenomeReadsNode, true, true);
            }
        }

        /// <summary>
        /// Validate Padena step4 ParallelDeNovothis.RemoveRedundancy() by passing graph 
        /// using input reads with rc such that it will create bubbles in the graph
        /// Input: Graph with bubbles
        /// Output: Graph without bubbles
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep4RemoveRedundancyWithRCReads()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaRemoveRedundancy(Constants.OneLineWithRCStep2Node, true, false);
            }
        }

        /// <summary>
        /// Validate Padena step4 ParallelDeNovothis.RemoveRedundancy() by passing graph 
        /// using input reads which will generate clusters in step2 graph
        /// Input: Graph with bubbles
        /// Output: Graph without bubbles
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep4RemoveRedundancyWithClusters()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaRemoveRedundancy(Constants.OneLineReadsWithClustersNode, true, false);
            }
        }

        /// <summary>
        /// Validate Padena step4 ParallelDeNovothis.RemoveRedundancy() by passing graph 
        /// using input reads which will generate clusters in step2 graph
        /// Input: Graph with bubbles
        /// Output: Graph without bubbles
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep4RemoveRedundancyWithBubbles()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaRemoveRedundancy(Constants.ReadsWithBubblesNode, false, false);
            }
        }

        /// <summary>
        /// Validate Padena step4 ParallelDeNovothis.RemoveRedundancy() by passing graph 
        /// using input reads which will generate clusters in step2 graph
        /// Input: Graph with bubbles
        /// Output: Graph without bubbles
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep4RemoveRedundancyWithMultipleBubbles()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaRemoveRedundancy(Constants.ReadsWithMultipleBubblesNode, false, false);
            }
        }

        /// <summary>
        /// Validate Padena step4 ParallelDeNovothis.RemoveRedundancy() by passing graph 
        /// using input reads which will generate clusters in step2 graph using Small size reads
        /// Input: Graph with bubbles
        /// Output: Graph without bubbles
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep4RemoveRedundancyWithSmallSizeReads()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaRemoveRedundancy(Constants.Step4ReadsWithSmallSize, false, true);
            }
        }

        /// <summary>
        /// Validate Padena step4 RedundantPathPurgerCtor() by passing graph 
        /// using one line reads 
        /// Input: One line graph
        /// Output: Graph without bubbles
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep4RedundantPathPurgerCtorWithOneLineReads()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateRedundantPathPurgerCtor(Constants.Step4RedundantPathReadsNode, false);
            }
        }

        /// <summary>
        /// Validate DetectErrorNodes() by passing graph object with
        /// one line reads such that it has bubbles
        /// Input: One line graph
        /// Output: Graph without bubbles
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep4DetectErrorNodesForOneLineReads()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateRedundantPathPurgerCtor(Constants.Step4RedundantPathReadsNode, false);
            }
        }

        /// <summary>
        /// Validate Padena RemoveErrorNodes() by passing redundant nodes list and graph
        /// Input : graph and redundant nodes list
        /// Output: Graph without bubbles
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep4RemoveErrorNodes()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateRedundantPathPurgerCtor(Constants.OneLineStep4ReadsNode, false);
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
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep5BuildContigsForViralGenomeReads()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateDe2thisBuildContigs(Constants.ViralGenomeReadsNode, true);
            }
        }

        /// <summary>
        /// Validate Padena step5 by passing graph and validating the contigs
        /// Input : graph
        /// Output: Contigs
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep5BuildContigsWithRCReads()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateDe2thisBuildContigs(Constants.OneLineReadsWithRCNode, false);
            }
        }

        /// <summary>
        /// Validate Padena step5 by passing graph and validating the contigs
        /// Input : graph
        /// Output: Contigs
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep5BuildContigsWithClusters()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateDe2thisBuildContigs(Constants.OneLineReadsWithClustersNode, false);
            }
        }

        /// <summary>
        /// Validate Padena step5 by passing graph and validating the
        /// contigs for small size chromosomes
        /// Input : graph
        /// Output: Contigs
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep5BuildContigsForSmallSizeChromosomes()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateDe2thisBuildContigs(Constants.SmallChromosomeReadsNode, true);
            }
        }

        /// <summary>
        /// Validate Padena step5 SimpleContigBuilder.BuildContigs() 
        /// by passing graph for small size chromosome.
        /// Input : graph
        /// Output: Contigs
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep5SimpleContigBuilderBuildContigsForSmallSizeRC()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateSimpleContigBuilderBuild(Constants.ChromosomeReads, true);
            }
        }

        #endregion

        #region PadenaStep6:Step1:TestCases

        /// <summary>
        /// Validate paired reads for Seq Id Starts with ".."
        /// Input : X1,Y1 format map reads with sequence ID contains "..".
        /// Output : Validate forward and backward reads.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6PairedReadsForSeqIDWithDots()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePairedReads(Constants.ReadsWithDotsNode);
            }
        }

        /// <summary>
        /// Validate paired reads for Seq Id Starts with ".." between
        /// Input : X1,Y1 format map reads with sequence ID contains ".." between.
        /// Output : Validate forward and backward reads.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6PairedReadsForSeqIDWithDotsBetween()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePairedReads(Constants.ReadsWithDotsBetweenSeqIdNode);
            }
        }

        /// <summary>
        /// Validate paired reads for Seq Id Chr1.X1:abc.X1:50K
        /// Input : X1,Y1 format map reads with sequence ID contains "X1 and Y1 letters" between.
        /// Output : Validate forward and backward reads.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6PairedReadsForSeqIDContainsX1Y1()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePairedReads(Constants.OneLineReadsForPairedReadsNode);
            }
        }

        /// <summary>
        /// Validate paired reads for Seq Id with special characters
        /// Input : X1,Y1 format map reads with sequence ID contains "X1 and Y1 letters" between.
        /// Output : Validate forward and backward reads.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6PairedReadsForSpecialCharsSeqId()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePairedReads(Constants.ReadsWithSpecialCharsNode);
            }
        }

        /// <summary>
        /// Validate paired reads for Mixed reads.
        /// Input : X1,Y1,F,R,1,2 format map reads
        /// Output : Validate forward and backward reads.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6PairedReadsForMixedFormatReads()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePairedReads(Constants.ReadsWithDotsNode);
            }
        }

        /// <summary>
        /// Validate paired reads for 2K and 0.5K library.
        /// Input : X1,Y1,F,R,1,2 format map reads
        /// Output : Validate forward and backward reads.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6PairedReadsForDifferentLibrary()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePairedReads(Constants.ReadsWith2KlibraryNode);
            }
        }

        /// <summary>
        /// Validate paired reads for 10K,50K and 100K library.
        /// Input : X1,Y1,F,R,1,2 format map reads
        /// Output : Validate forward and backward reads.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6PairedReadsFor100kLibrary()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePairedReads(Constants.ReadsWith10KAnd50KAnd100KlibraryNode);
            }
        }

        /// <summary>
        /// Validate paired reads for Reads without any Seq Name
        /// Input : X1,Y1,F,R,1,2 format map reads
        /// Output : Validate forward and backward reads.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6PairedReadsForSeqsWithoutAnyID()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePairedReads(Constants.ReadsWithoutAnySeqIdNode);
            }
        }

        /// <summary>
        /// Validate paired reads for Reads with numeric library name.
        /// Input : X1,Y1,F,R,1,2 format map reads
        /// Output : Validate forward and backward reads.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6PairedReadsForNumericLibraryName()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePairedReads(Constants.ReadsWith10KAnd50KAnd100KlibraryNode);
            }
        }

        /// <summary>
        /// Validate Adding new library information to library list.
        /// Input : Library name,Standard deviation and mean length.
        /// Output : Validate forward and backward reads.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6Libraryinformation()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.AddLibraryInformation(Constants.AddX1AndY1FormatPairedReadsNode, true);
            }
        }

        /// <summary>
        /// Validate library information for 1 and 2 format paired reads.
        /// Input : 1 and 2 format paired reads.
        /// Output : Validate library information.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6GetLibraryinformation()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.GetLibraryInformation(Constants.GetLibraryInformationNode);
            }
        }

        #endregion PadenaStep6:Step1:TestCases

        #region PadenaStep6:Step2:TestCases

        /// <summary>
        /// Validate ReadContigMapper.Map() using multiple clustalW 
        /// contigs.
        /// Input : Reads,KmerLength,dangling threshold.
        /// Output : Validate MapReads to contigs.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        [Ignore]
        public void ValidatePadenaStep6MapReadsToContigForClustalW()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateMapReadsToContig(Constants.MapPairedReadsToContigForClustalWContigsNode, true);
            }
        }

        /// <summary>
        /// Validate ReadContigMapper.Map() using Reverse complement contig.
        /// contigs.
        /// Input : Reads,KmerLength,dangling threshold.
        /// Output : Validate MapReads to contigs.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        [Ignore]
        public void ValidatePadenaStep6MapReadsToContigForUsingReverseComplementContig()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateMapReadsToContig(Constants.MapPairedReadsToContigForReverseComplementContigsNode,
                    true);
            }
        }

        /// <summary>
        /// Validate ReadContigMapper.Map() using left side contig generator.
        /// Input : Reads,KmerLength,dangling threshold.
        /// Output : Validate MapReads to contigs.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6MapReadsToContigForLeftSideContigGenerator()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateMapReadsToContig(Constants.MapPairedReadsToContigForLeftSideContigGeneratorNode,
                    true);
            }
        }

        /// <summary>
        /// Validate ReadContigMapper.Map() using Contigs generated by passing input 
        /// reads from sequence such that one read is sequence and another 
        /// read is reverse complement
        /// Input : Reads,KmerLength,dangling threshold.
        /// Output : Validate MapReads to contigs.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6MapReadsToContigForOneSeqReadAndOtherRevComp()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateMapReadsToContig(Constants.MapPairedReadsToContigForSeqAndRevCompNode,
                 false);
            }
        }

        /// <summary>
        /// Validate ReadContigMapper.Map() using Right side contig generator.
        /// Input : Reads,KmerLength,dangling threshold.
        /// Output : Validate MapReads to contigs.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6MapReadsToContigForRightSideGenerator()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateMapReadsToContig(Constants.MapPairedReadsToContigForRightSideContigGeneratorNode,
                    false);
            }
        }

        #endregion PadenaStep6:Step2:TestCases

        #region PadenaStep6:Step4:TestCases

        /// <summary>
        /// Validate filter Contig Pairs formed in Forward direction with one 
        /// paired read does not support orientation.
        /// Input : Reads,KmerLength,dangling threshold.
        /// Output : Validate filter contig pairs.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6FilterPairedReadsWithFWReadsNotSupportOrientation()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateFilterPaired(Constants.FilterPairedReadContigsForFWOrnNode, true);
            }
        }

        /// <summary>
        /// Validate filter Contig Pairs formed in Reverse direction with one paired
        /// read does not support orientation.
        /// Input : Reads,KmerLength,dangling threshold.
        /// Output : Validate filter contig pairs.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6FilterPairedReadsWithRevReadsNotSupportOrientation()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateFilterPaired(Constants.FilterPairedReadContigsForRevOrientationNode, true);
            }
        }

        /// <summary>
        /// Validate filter Contig Pairs formed in Forward direction and reverse 
        /// complement of Contig
        /// Input : Reads,KmerLength,dangling threshold.
        /// Output : Validate filter contig pairs.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6FilterPairedsForContigRevComplement()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateFilterPaired(
                  Constants.FilterPairedReadContigsForFWDirectionWithRevCompContigNode, false);
            }
        }

        /// <summary>
        /// Validate filter Contig Pairs formed in Backward direction
        /// and reverse complement of Contig
        /// Input : Reads,KmerLength,dangling threshold.
        /// Output : Validate filter contig pairs.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6FilterPairedsForReverseReadAndRevComplement()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateFilterPaired(
                  Constants.FilterPairedReadContigsForBackwardDirectionWithRevCompContig, true);
            }
        }


        #endregion PaDeNaStep6:Step4:TestCases

        #region PadenaStep6:Step5:TestCases

        /// <summary>
        /// Calculate distance for Contig Pairs formed in Forward 
        /// direction with one paired read does not support orientation.
        /// Input : 3-4 Line sequence reads.
        /// Output : Filtered contigs.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6CalculateDistanceForForwardPairedReads()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateContigDistance(Constants.FilterPairedReadContigsForFWOrnNode);
            }
        }

        /// <summary>
        /// Calculate distance for Contig Pairs formed in Forward 
        /// direction with one paired read does not support orientation.
        /// Input : 3-4 Line sequence reads.
        /// Output : Filtered contigs.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        [Ignore]
        public void ValidatePadenaStep6CalculateDistanceForReversePairedReads()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateContigDistance(Constants.FilterPairedReadContigsForRevOrientationNode);
            }
        }

        /// <summary>
        /// Calculate distance for Contig Pairs formed in Forward direction
        /// and reverse complement of Contig
        /// Input : 3-4 Line sequence reads.
        /// Output : Filtered contigs.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        [Ignore]
        public void ValidatePadenaStep6CalculateDistanceForForwardPairedReadsWithRevCompl()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateContigDistance(
                  Constants.FilterPairedReadContigsForFWDirectionWithRevCompContigNode);
            }
        }

        /// <summary>
        /// Calculate distance for Contig Pairs formed in Reverse direction
        /// and reverse complement of Contig
        /// Input : 3-4 Line sequence reads.
        /// Output : Standard deviation and distance between contigs.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6CalculateDistanceForReversePairedReadsWithRevCompl()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateContigDistance(
                  Constants.FilterPairedReadContigsForBackwardDirectionWithRevCompContig);
            }
        }


        #endregion PadenaStep6:Step5:TestCases

        #region PadenaStep6:Step6:TestCases

        /// <summary>
        /// Validate scaffold path for Contig Pairs formed in Forward
        /// direction with all paired reads support orientation using
        /// FindPath(grpah;ContigPairedReads;KmerLength;Depth)
        /// Input : 3-4 Line sequence reads.
        /// Output : Validation of scaffold paths.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        [Ignore]
        public void ValidatePadenaStep6ScaffoldPathsForForwardOrientation()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateScaffoldPath(
                  Constants.ScaffoldPathWithForwardOrientationNode);
            }
        }

        /// <summary>
        /// Validate scaffold path for Contig Pairs formed in Reverse
        /// direction with all paired reads support orientation using
        /// FindPath(grpah;ContigPairedReads;KmerLength;Depth).
        /// Input : 3-4 Line sequence reads.
        /// Output : Validation of scaffold paths.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6ScaffoldPathsForReverseOrientation()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateScaffoldPath(
                  Constants.ScaffoldPathWithReverseOrientationNode);
            }
        }

        /// <summary>
        /// Validate trace path for Contig Pairs formed in
        /// Forward direction and reverse complement of
        /// Contig
        /// Input : Forward read orientation and rev complement.
        /// Output : Validation of scaffold paths.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6ScaffoldPathsForForwardDirectionAndRevComp()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateScaffoldPath(
                  Constants.ScaffoldPathWithForwardDirectionAndRevComp);
            }
        }

        /// <summary>
        /// Validate trace path for Contig Pairs formed in
        /// Reverse direction and reverse complement of
        /// Contig
        /// Input : Reverse read orientation and rev complement.
        /// Output : Validation of scaffold paths.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6ScaffoldPathsForReverseDirectionAndRevComp()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateScaffoldPath(
                  Constants.ScaffoldPathWithReverseDirectionAndRevComp);
            }
        }

        /// <summary>
        /// Validate trace path for Contig Pairs formed in
        /// Forward direction and palindrome of
        /// Contig
        /// Input : Forward read orientation and palindrome of contig.
        /// Output : Validation of scaffold paths.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6ScaffoldPathsForForwardDirectionAndPalContig()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateScaffoldPath(
                  Constants.ScaffoldPathWithForwardDirectionAndPalContig);
            }
        }

        /// <summary>
        /// Validate trace path for Contig Pairs formed in
        /// Reverse direction and palindrome of
        /// Contig
        /// Input : Reverse read orientation and palindrome of contig.
        /// Output : Validation of scaffold paths.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6ScaffoldPathsForReverseDirectionAndPalContig()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateScaffoldPath(
                  Constants.ScaffoldPathWithReverseDirectionAndPalContig);
            }
        }


        #endregion PadenaStep6:Step6:TestCases

        #region PadenaStep6:Step7/8:TestCases

        /// <summary>
        /// Validate assembled path by passing scaffold paths for
        /// Contig Pairs formed in Forward direction and reverse 
        /// complement of Contig
        /// Input : 3-4 Line sequence reads.
        /// Output : Assembled paths 
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6AssembledPathForForwardAndRevComplContig()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateAssembledPath(
                  Constants.AssembledPathForForwardWithReverseCompl);
            }
        }

        /// <summary>
        /// Validate assembled path by passing scaffold paths for
        /// Contig Pairs formed in Reverse direction and reverse 
        /// complement of Contig
        /// Input : 3-4 Line sequence reads.
        /// Output : Assembled paths 
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        [Ignore]
        public void ValidatePadenaStep6AssembledPathForReverseAndRevComplContig()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateAssembledPath(
                  Constants.AssembledPathForReverseWithReverseCompl);
            }
        }

        /// <summary>
        /// Validate assembled path by passing scaffold for Contig Pairs 
        /// formed in Forward direction and palindrome of Contig
        /// Input : Sequence reads with Palindrome contigs.
        /// Output : Assembled paths 
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6AssembledPathForForwardAndPalContig()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateAssembledPath(
                  Constants.AssembledPathForForwardAndPalContig);
            }
        }

        /// <summary>
        /// Validate assembled path by passing scaffold for Contig Pairs 
        /// formed in Reverse direction and palindrome of Contig
        /// Input : Sequence reads with Palindrome contigs.
        /// Output : Assembled paths 
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6AssembledPathForReverseAndPalContig()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateAssembledPath(
                  Constants.AssembledPathForReverseAndPalContig);
            }
        }

        /// <summary>
        /// Validate Scaffold sequence for small size sequence reads.
        /// Input : small size sequence reads.
        /// Output : Validation of Scaffold sequence.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6ScaffoldSequenceForSmallReads()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidateScaffoldSequence(Constants.ScaffoldSequenceNode);
            }
        }

        /// <summary>
        ///  Validate Assembled sequences with Euler Test data reads,
        ///  Input : Euler testObj data seq reads.
        ///  output : Aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6AssembledSequenceWithEulerData()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaAssembledSeqs(
                    Constants.AssembledSequencesForEulerDataNode);
            }
        }

        /// <summary>
        ///  Validate Assembled sequences for reads formed 
        ///  scaffold paths containing overlapping paths.
        ///  Input : Viral Genome reads.
        ///  output : Aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6AssembledSequenceForOverlappingScaffoldPaths()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaAssembledSeqs(
                    Constants.AssembledSequencesForViralGenomeReadsNode);
            }
        }

        /// <summary>
        ///  Validate Assembled sequences for reads formed 
        ///  contigs in forward and reverse complement contig.
        ///  Input : Sequence reads.
        ///  output : Aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6AssembledSequenceForForwardAndRevCompl()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaAssembledSeqs(
                    Constants.AssembledSequencesForForwardAndRevComplContigNode);
            }
        }

        /// <summary>
        ///  Validate Assembled sequences for reads formed 
        ///  contigs in forward and palindrome contig.
        ///  Input : Sequence reads.
        ///  output : Aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePadenaStep6AssembledSequenceForForwardAndPalContig()
        {
            using (PadenaP1Test testObj = new PadenaP1Test())
            {
                testObj.ValidatePadenaAssembledSeqs(
                    Constants.AssembledSequencesForForwardAndPalContigNode);
            }
        }

        #endregion PadenaStep6:Step7/8:TestCases
    }

    /// <summary>
    /// This class contains helper methods for Padena.
    /// </summary>
    internal class PadenaP1Test : ParallelDeNovoAssembler
    {
        #region Global Variables

        readonly Utility utilityObj = new Utility(@"TestUtils\PadenaTestData\PadenaTestsConfig.xml");

        #endregion Global Variables

        #region Helper Methods

        /// <summary>
        /// Validate ParallelDeNovothis step1 Build kmers 
        /// </summary>
        /// <param name="nodeName">xml node for test data</param>
        /// <param name="isSmallSize">Is file small size?</param>
        internal void ValidatePadenaBuildKmers(string nodeName, bool isSmallSize)
        {
            // Read all the input sequences from xml config file
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);
            string expectedKmersCount = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedKmersCount);

            // Set kmerLength
            this.KmerLength = int.Parse(kmerLength, (IFormatProvider)null);
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                // Set all the input reads and execute build kmers
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());
                IEnumerable<KmersOfSequence> lstKmers =
                    (new SequenceToKmerBuilder()).Build(this.SequenceReads, this.KmerLength);

                if (isSmallSize)
                {
                    Assert.AreEqual(expectedKmersCount, lstKmers.Count().ToString((IFormatProvider)null));
                }
                else
                {
                    ValidateKmersList(new List<KmersOfSequence>(lstKmers), sequenceReads.ToList(), nodeName);
                }
            }

            ApplicationLog.WriteLine(@"Padena P1 : Validation of Build with all input reads using ParallelDeNovothis sequence completed successfully");
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
        /// Validate graph generated using ParallelDeNovothis.CreateGraph() with kmers
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        /// <param name="isLargeSizeReads">Is large size reads?</param>
        internal void ValidatePadenaBuildGraph(string nodeName, bool isLargeSizeReads)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);
            string expectedGraphsNodeCount = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.GraphNodesCountNode);

            // Get the input reads and build kmers
            using (FastAParser parser = new FastAParser(filePath))
            {
                IEnumerable<ISequence> sequenceReads = parser.Parse();

                this.KmerLength = int.Parse(kmerLength, null);
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());
                this.CreateGraph();
                DeBruijnGraph graph = this.Graph;

                ApplicationLog.WriteLine("Padena P1 : Step1,2 Completed Successfully");

                if (isLargeSizeReads)
                    Assert.AreEqual(Int32.Parse(expectedGraphsNodeCount, null), graph.GetNodes().Count());
                else
                    ValidateGraph(graph, nodeName);
            }
            ApplicationLog.WriteLine(@"Padena P1 : ParallelDeNovothis CreateGraph() validation for Padena step2 completed successfully");
        }

        /// <summary>
        /// Validate the graph nodes sequence, left edges and right edges
        /// </summary>
        /// <param name="graph">graph object</param>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateGraph(DeBruijnGraph graph, string nodeName)
        {
            string nodesSequence = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.NodesSequenceNode);
            string nodesLeftEdges = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.NodesLeftEdgesCountNode);
            string nodesRightEdges = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.NodeRightEdgesCountNode);

            string[] leftEdgesCount = ReadStringFromFile(nodesLeftEdges).Replace("\r\n", "").Split(',');
            string[] rightEdgesCount = ReadStringFromFile(nodesRightEdges).Replace("\r\n", "").Split(',');
            string[] nodesSequences = ReadStringFromFile(nodesSequence).Replace("\r\n", "").Split(',');

            // Validate the nodes 
            for (int iseq = 0; iseq < nodesSequences.Length; iseq++)
            {
                DeBruijnNode dbnodes = graph.GetNodes().First(n => graph.GetNodeSequence(n).ConvertToString() == nodesSequences[iseq]
                                                                || graph.GetNodeSequence(n).GetReverseComplementedSequence().ConvertToString() == nodesSequences[iseq]);

                //Due to parallelization the left edges and right edges count
                //can be swapped while processing. if actual left edges count 
                //is either equal to expected left edges count or right edges count and vice versa.
                Assert.IsTrue(
                  dbnodes.LeftExtensionNodesCount.ToString((IFormatProvider)null) == leftEdgesCount[iseq] ||
                  dbnodes.LeftExtensionNodesCount.ToString((IFormatProvider)null) == rightEdgesCount[iseq]);
                Assert.IsTrue(
                  dbnodes.RightExtensionNodesCount.ToString((IFormatProvider)null) == leftEdgesCount[iseq] ||
                  dbnodes.RightExtensionNodesCount.ToString((IFormatProvider)null) == rightEdgesCount[iseq]);
            }
        }

        /// <summary>
        /// Get the input string from the file.
        /// </summary>
        /// <param name="filename">input filename</param>
        /// <returns>Reads the file and returns input string</returns>
        static string ReadStringFromFile(string filename)
        {
            string readString = null;
            using (StreamReader reader = new StreamReader(filename))
            {
                readString = reader.ReadToEnd();
            }
            return readString;
        }

        /// <summary>
        /// Validate ParallelDeNovothis.RemoveRedundancy() which removes bubbles formed in the graph
        /// and validate the graph
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        /// <param name="defaultThreshold">Is Default Threshold?</param>
        /// <param name="isMicroorganism">Is micro organsm?</param>
        internal void ValidatePadenaRemoveRedundancy(string nodeName, bool defaultThreshold, bool isMicroorganism)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.KmerLengthNode);
            string expectedNodesCount = utilityObj.xmlUtil.GetTextValue(
              nodeName, Constants.ExpectedNodesCountRemoveRedundancy);

            string danglingThreshold = null;
            string pathlengthThreshold = null;
            if (!defaultThreshold)
            {
                danglingThreshold = utilityObj.xmlUtil.GetTextValue(nodeName,
                  Constants.DanglingLinkThresholdNode);
                pathlengthThreshold = utilityObj.xmlUtil.GetTextValue(nodeName,
                  Constants.PathLengthThresholdNode);
            }

            // Get the input reads and build kmers
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                // Build kmers from step1,graph in step2 
                // Remove the dangling links from graph in step3
                // Remove bubbles from graph in step4
                // Validate the graph
                if (!defaultThreshold)
                {
                    this.DanglingLinksThreshold = int.Parse(danglingThreshold, (IFormatProvider)null);
                    this.DanglingLinksPurger =
                      new DanglingLinksPurger(this.DanglingLinksThreshold);
                    this.RedundantPathLengthThreshold = int.Parse(pathlengthThreshold, (IFormatProvider)null);
                    this.RedundantPathsPurger =
                      new RedundantPathsPurger(this.RedundantPathLengthThreshold);
                }
                else
                {
                    this.DanglingLinksPurger =
                      new DanglingLinksPurger(int.Parse(kmerLength, (IFormatProvider)null));
                    this.RedundantPathsPurger =
                      new RedundantPathsPurger(int.Parse(kmerLength, (IFormatProvider)null) + 1);
                }
                this.KmerLength = int.Parse(kmerLength, (IFormatProvider)null);
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());
                this.CreateGraph();
                DeBruijnGraph graph = this.Graph;

                ApplicationLog.WriteLine("Padena P1 : Step1,2 Completed Successfully");
                this.UnDangleGraph();

                ApplicationLog.WriteLine("Padena P1 : Step3 Completed Successfully");
                this.RemoveRedundancy();

                ApplicationLog.WriteLine("Padena P1 : Step4 Completed Successfully");
                if (isMicroorganism)
                {
                    Assert.AreEqual(expectedNodesCount, graph.GetNodes().Count().ToString((IFormatProvider)null));
                }
                else
                {
                    ValidateGraph(graph, nodeName);
                }
            }
            ApplicationLog.WriteLine(@"Padena P1 :ParallelDeNovothis.RemoveRedundancy() validation for Padena step4 completed successfully");
        }

        /// <summary>
        /// Validate the ParallelDeNovothis unDangleGraph() method which removes the dangling link
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        /// <param name="defaultThreshold">Default Threshold</param>
        /// <param name="smallSizeChromosome">Small size chromosome</param>
        internal void ValidatePadenaUnDangleGraph(string nodeName, bool defaultThreshold, bool smallSizeChromosome)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);
            string expectedNodesCount = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.NodesCountAfterDanglingGraphNode);
            string danglingThreshold = null;
            
            if (!defaultThreshold)
                danglingThreshold = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.DanglingLinkThresholdNode);

            // Get the input reads and build kmers
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                // Build kmers from step1,graph in step2 
                this.KmerLength = int.Parse(kmerLength, null);
                if (!defaultThreshold)
                {
                    this.DanglingLinksThreshold = int.Parse(danglingThreshold, null);
                }
                else
                {
                    this.DanglingLinksThreshold = int.Parse(kmerLength, null) + 1;
                }
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());
                this.CreateGraph();
                DeBruijnGraph graph = this.Graph;

                ApplicationLog.WriteLine("Padena P1 : Step1,2 Completed Successfully");
                this.DanglingLinksPurger = new DanglingLinksPurger(this.DanglingLinksThreshold);
                this.UnDangleGraph();

                ApplicationLog.WriteLine("Padena P1 : Step3 Completed Successfully");
                if (smallSizeChromosome)
                {
                    Assert.AreEqual(expectedNodesCount, graph.GetNodes().Count().ToString((IFormatProvider)null));
                }
                else
                {
                    ValidateGraph(graph, nodeName);
                }
            }
            ApplicationLog.WriteLine(@"Padena P1 :ParallelDeNovothis.UndangleGraph() validation for Padena step3 completed successfully");
        }

        /// <summary>
        /// Validate KmersOfSequence ctor by passing base sequence reads, kmer length and
        /// built kmers
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        internal void ValidateKmersOfSequenceCtorWithBuildKmers(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);

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
                    KmersOfSequence kmer = builder.Build(sequence, int.Parse(kmerLength, (IFormatProvider)null));
                    KmersOfSequence kmerSequence = new KmersOfSequence(sequence,
                      int.Parse(kmerLength, (IFormatProvider)null), kmer.Kmers);
                    lstKmers.Add(kmerSequence);
                }

                ValidateKmersList(lstKmers, sequenceReads.ToList(), nodeName);
            }
            ApplicationLog.WriteLine(@"Padena P1 : KmersOfSequence ctor with build kmers method validation completed successfully");
        }

        /// <summary>
        /// Validate KmersOfSequence ctor by passing base sequence reads, kmer length and
        /// built kmers and validate its properties.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        internal void ValidateKmersOfSequenceCtorProperties(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.KmerLengthNode);
            string expectedSeq = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.BaseSequenceNode);
            string expectedKmers = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.KmersCountNode);

            // Get the input reads
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                SequenceToKmerBuilder builder = new SequenceToKmerBuilder();

                KmersOfSequence kmer = builder.Build(sequenceReads.ToList()[0],
                  int.Parse(kmerLength, (IFormatProvider)null));
                KmersOfSequence kmerSequence = new KmersOfSequence(sequenceReads.ToList()[0],
                  int.Parse(kmerLength, (IFormatProvider)null), kmer.Kmers);

                // Validate KmerOfSequence properties.
                Assert.AreEqual(expectedSeq, new string(kmerSequence.BaseSequence.Select(a => (char)a).ToArray()));
                Assert.AreEqual(expectedKmers, kmerSequence.Kmers.Count.ToString((IFormatProvider)null));
            }

            ApplicationLog.WriteLine(@"Padena P1 : KmersOfSequence ctor with build kmers method validation completed successfully");
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

            ApplicationLog.WriteLine(@"Padena P1 : KmersOfSequence ToSequences() method validation completed successfully");
        }

        /// <summary>
        /// Validate Validate DeBruijinGraph properties
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateDeBruijinGraphproperties(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.KmerLengthNode);
            string ExpectedNodesCount = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.GraphNodesCountNode);

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

                ApplicationLog.WriteLine("Padena P1 : Step1,2 Completed Successfully");

                // Validate DeBruijnGraph Properties.
                Assert.AreEqual(ExpectedNodesCount, graph.GetNodes().Count().ToString((IFormatProvider)null));
            }

            ApplicationLog.WriteLine(@"Padena P1 : ParallelDeNovothis CreateGraph() validation for Padena step2 completed successfully");
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
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

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
            ApplicationLog.WriteLine(@"Padena P1 :DeBruijnNode AddLeftExtension() validation for Padena step2 completed successfully");
        }

        /// <summary>
        /// Validate AddRightEndExtension() method of DeBruijnNode 
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateDeBruijnNodeAddRightExtension(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.KmerLengthNode);

            // Get the input reads and build kmers
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                // Build kmers from step1
                this.KmerLength = int.Parse(kmerLength, (IFormatProvider)null);
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());
                IList<KmersOfSequence> lstKmers = new List<KmersOfSequence>(
                    (new SequenceToKmerBuilder()).Build(this.SequenceReads, this.KmerLength));

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

                Assert.AreEqual(lstKmers[1].Kmers.First().Count,
                  node.RightExtensionNodesCount);
            }
            ApplicationLog.WriteLine(@"Padena P1 :DeBruijnNode AddRightExtension() validation for Padena step2 completed successfully");
        }

        /// <summary>
        /// Validate RemoveExtension() method of DeBruijnNode 
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateDeBruijnNodeRemoveExtension(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.KmerLengthNode);

            // Get the input reads and build kmers
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                // Build kmers from step1
                this.KmerLength = int.Parse(kmerLength, (IFormatProvider)null);
                this.SequenceReads.Clear();

                this.SetSequenceReads(sequenceReads.ToList());
                IList<KmersOfSequence> lstKmers = new List<KmersOfSequence>(
                    (new SequenceToKmerBuilder()).Build(this.SequenceReads, this.KmerLength));

                // Validate the node creation
                // Create node and add left node.
                ISequence seq = this.SequenceReads.First();
                KmerData32 kmerData = new KmerData32();
                kmerData.SetKmerData(seq, lstKmers[0].Kmers.First().Positions[0], this.KmerLength);

                DeBruijnNode node = new DeBruijnNode(kmerData, 1);
                kmerData = new KmerData32();
                kmerData.SetKmerData(seq, lstKmers[1].Kmers.First().Positions[0], this.KmerLength);

                DeBruijnNode leftnode = new DeBruijnNode(kmerData, 1);
                DeBruijnNode rightnode = new DeBruijnNode(kmerData, 1);

                node.SetExtensionNode(false, true, leftnode);
                node.SetExtensionNode(true, true, rightnode);

                // Validates count before removing right and left extension nodes.
                Assert.AreEqual(lstKmers[1].Kmers.First().Count,
                  node.RightExtensionNodesCount);
                Assert.AreEqual(1, node.RightExtensionNodesCount);
                Assert.AreEqual(1, node.LeftExtensionNodesCount);

                // Remove right and left extension nodes.
                node.RemoveExtensionThreadSafe(rightnode);
                node.RemoveExtensionThreadSafe(leftnode);

                // Validate node after removing right and left extensions.
                Assert.AreEqual(0, node.RightExtensionNodesCount);
                Assert.AreEqual(0, node.LeftExtensionNodesCount);
            }
            ApplicationLog.WriteLine(@"Padena P1 :DeBruijnNode AddRightExtension() validation for Padena step2 completed successfully");
        }

        /// <summary>
        /// Validate the DeBruijnNode ctor by passing the kmer and validating 
        /// the node object.
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateDeBruijnNodeCtor(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.KmerLengthNode);
            string nodeExtensionsCount = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.NodeExtensionsCountNode);
            string kmersCount = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.KmersCountNode);
            string leftNodeExtensionCount = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.LeftNodeExtensionsCountNode);
            string rightNodeExtensionCount = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.RightNodeExtensionsCountNode);

            // Get the input reads and build kmers
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                // Build the kmers using this
                this.KmerLength = int.Parse(kmerLength, (IFormatProvider)null);
                this.SequenceReads.Clear();
                this.SetSequenceReads(sequenceReads.ToList());
                IList<KmersOfSequence> lstKmers = new List<KmersOfSequence>(
                    (new SequenceToKmerBuilder()).Build(this.SequenceReads, this.KmerLength));

                // Validate the node creation
                // Create node and add left node.
                ISequence seq = this.SequenceReads.First();
                KmerData32 kmerData = new KmerData32();
                kmerData.SetKmerData(seq, lstKmers[0].Kmers.First().Positions[0], this.KmerLength);

                DeBruijnNode node = new DeBruijnNode(kmerData, 1);
                kmerData = new KmerData32();
                kmerData.SetKmerData(seq, lstKmers[1].Kmers.First().Positions[0], this.KmerLength);

                DeBruijnNode leftnode = new DeBruijnNode(kmerData, 1);
                DeBruijnNode rightnode = new DeBruijnNode(kmerData, 1);

                node.SetExtensionNode(false, true, leftnode);
                node.SetExtensionNode(true, true, rightnode);

                // Validate DeBruijnNode class properties.
                Assert.AreEqual(nodeExtensionsCount, node.ExtensionsCount.ToString((IFormatProvider)null));
                Assert.AreEqual(kmersCount, node.KmerCount.ToString((IFormatProvider)null));
                Assert.AreEqual(leftNodeExtensionCount, node.LeftExtensionNodesCount.ToString((IFormatProvider)null));
                Assert.AreEqual(rightNodeExtensionCount, node.RightExtensionNodesCount.ToString((IFormatProvider)null));
                Assert.AreEqual(leftNodeExtensionCount, node.LeftExtensionNodesCount.ToString((IFormatProvider)null));
                Assert.AreEqual(rightNodeExtensionCount, node.RightExtensionNodesCount.ToString((IFormatProvider)null));
            }

            ApplicationLog.WriteLine("Padena P1 : DeBruijnNode ctor() validation for Padena step2 completed successfully");
        }

        /// <summary>
        /// Validate RemoveErrorNodes() method is removing dangling nodes from the graph
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidatePadenaRemoveErrorNodes(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.KmerLengthNode);

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
                DeBruijnPathList danglingnodes = danglingLinksPurger.DetectErroneousNodes(graph);
                danglingLinksPurger.RemoveErroneousNodes(graph, danglingnodes);
                Assert.IsFalse(graph.GetNodes().Contains(danglingnodes.Paths[0].PathNodes[0]));
            }
            ApplicationLog.WriteLine(@"Padena P1 :DeBruijnGraph.RemoveErrorNodes() validation for Padena step3 completed successfully");
        }

        /// <summary>
        /// Creates RedundantPathPurger instance by passing pathlength and count. Detect 
        /// redundant error nodes and remove these nodes from the graph. Validate the graph.
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        /// <param name="isMicroOrganism">Is micro organism</param>    
        internal void ValidateRedundantPathPurgerCtor(string nodeName, bool isMicroOrganism)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);
            string expectedNodesCount = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.ExpectedNodesCountAfterDangling);

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
                DeBruijnGraph graph = this.Graph;
                this.DanglingLinksPurger = new DanglingLinksPurger(this.KmerLength);
                this.UnDangleGraph();

                // Create RedundantPathPurger instance, detect redundant nodes and remove error nodes
                RedundantPathsPurger redundantPathPurger =
                  new RedundantPathsPurger(int.Parse(kmerLength, (IFormatProvider)null) + 1);
                DeBruijnPathList redundantnodelist = redundantPathPurger.DetectErroneousNodes(graph);
                redundantPathPurger.RemoveErroneousNodes(graph, redundantnodelist);

                if (isMicroOrganism)
                    Assert.AreEqual(expectedNodesCount, graph.GetNodes().Count());
                else
                    ValidateGraph(graph, nodeName);
            }
            ApplicationLog.WriteLine(@"Padena P1 :RedundantPathsPurger ctor and methods validation for Padena step4 completed successfully");
        }

        /// <summary>
        /// Validate ParallelDeNovothis.BuildContigs() by passing graph object
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        /// <param name="isChromosomeRC">Is chromosome RC?</param>
        internal void ValidateDe2thisBuildContigs(string nodeName, bool isChromosomeRC)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.KmerLengthNode);
            string expectedContigsString = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.ContigsNode);
            string[] expectedContigs;
            if (!expectedContigsString.ToUpper(CultureInfo.InstalledUICulture).Contains("PADENATESTDATA"))
                expectedContigs = expectedContigsString.Split(',');
            else
                expectedContigs =
                  ReadStringFromFile(expectedContigsString).Replace("\r\n", "").Split(',');

            string expectedContigsCount = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ContigsCount);

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
                this.DanglingLinksPurger = new DanglingLinksPurger(this.KmerLength);
                this.UnDangleGraph();
                this.RedundantPathsPurger = new RedundantPathsPurger(this.KmerLength + 1);
                this.RemoveRedundancy();
                this.ContigBuilder = new SimplePathContigBuilder();
                IList<ISequence> contigs = this.BuildContigs().ToList();

                // Validate contigs count only for Chromosome files. 
                if (isChromosomeRC)
                {
                    Assert.AreEqual(expectedContigsCount, contigs.Count.ToString((IFormatProvider)null));
                }
                // validate all contigs of a sequence.
                else
                {
                    for (int index = 0; index < contigs.Count(); index++)
                    {
                        Assert.IsTrue(expectedContigs.Contains(new string(contigs[index].Select(a => (char)a).ToArray())) ||
                          expectedContigs.Contains(new string(contigs[index].GetReverseComplementedSequence().Select(a => (char)a).ToArray())));
                    }
                }
            }

            ApplicationLog.WriteLine(@"Padena P1 :ParallelDeNovothis.BuildContigs() validation for Padena step5 completed successfully");
        }

        /// <summary>
        /// Validate the SimpleContigBuilder Build() method using step 4 graph
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        /// <param name="isChromosomeRC">Is Chromosome RC?</param>
        internal void ValidateSimpleContigBuilderBuild(string nodeName, bool isChromosomeRC)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.KmerLengthNode);
            string expectedContigsString = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.ContigsNode);
            string[] expectedContigs = expectedContigsString.Split(',');
            string expectedContigsCount = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.ContigsCount);

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
                DeBruijnGraph graph = this.Graph;
                this.UnDangleGraph();
                this.RemoveRedundancy();

                // Validate the SimpleContigBuilder.Build() by passing graph
                SimplePathContigBuilder builder = new SimplePathContigBuilder();
                IList<ISequence> contigs = builder.Build(graph).ToList();

                if (isChromosomeRC)
                {
                    Assert.AreEqual(expectedContigsCount,
                        contigs.Count.ToString((IFormatProvider)null));
                }
                else
                {
                    // Validate the contigs
                    for (int index = 0; index < contigs.Count; index++)
                    {
                        Assert.IsTrue(expectedContigs.Contains(new string(contigs[index].Select(a => (char)a).ToArray())));
                    }
                }
            }
            ApplicationLog.WriteLine(@"Padena P1 :SimpleContigBuilder.BuildContigs() validation for Padena step5 completed successfully");
        }

        /// <summary>
        /// Validate Map paired reads for a sequence reads.
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

                foreach (ISequence seq in sequences)
                {
                    sequenceReads.Add(seq);
                }

                // Convert reads to map paired reads.
                MatePairMapper pair = new MatePairMapper();
                pairedreads = pair.Map(sequenceReads);

                // Validate Map paired reads.
                Assert.AreEqual(expectedPairedReadsCount,
                    pairedreads.Count.ToString((IFormatProvider)null));

                for (int index = 0; index < pairedreads.Count; index++)
                {
                    Assert.IsTrue(forwardReadsNode.Contains(new string(pairedreads[index].GetForwardRead(sequenceReads).Select(a => (char)a).ToArray())));
                    Assert.IsTrue(backwardReadsNode.Contains(new string(pairedreads[index].GetReverseRead(sequenceReads).Select(a => (char)a).ToArray())));
                    Assert.IsTrue(deviationNode.Contains(pairedreads[index].StandardDeviationOfLibrary.ToString((IFormatProvider)null)));
                    Assert.IsTrue(expectedMean.Contains(pairedreads[index].MeanLengthOfLibrary.ToString((IFormatProvider)null)));
                    Assert.IsTrue(expectedLibrary.Contains(pairedreads[index].Library.ToString((IFormatProvider)null)));
                }
            }
            ApplicationLog.WriteLine(@"Padena P1 : Map paired reads has been verified successfully");
        }

        /// <summary>
        /// Validate library information
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void GetLibraryInformation(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.FilePathNode);
            string expectedPairedReadsCount = utilityObj.xmlUtil.GetTextValue(
              nodeName, Constants.PairedReadsCountNode);
            string expectedLibraray = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.LibraryName);
            string expectedStdDeviation = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.StdDeviation);
            string mean = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.Mean);

            IList<MatePair> pairedreads = new List<MatePair>();

            // Get the input reads 
            IEnumerable<ISequence> sequences = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequences = parser.Parse();

                // Convert reads to map paired reads.
                MatePairMapper pair = new MatePairMapper();
                pairedreads = pair.Map(new List<ISequence>(sequences));

                // Validate Map paired reads.
                Assert.AreEqual(expectedPairedReadsCount,
                  pairedreads.Count.ToString((IFormatProvider)null));

                // Get library infomration and validate
                CloneLibraryInformation libraryInfo =
                  CloneLibrary.Instance.GetLibraryInformation
                  (pairedreads[0].Library);

                Assert.AreEqual(expectedStdDeviation, libraryInfo.StandardDeviationOfInsert.ToString((IFormatProvider)null));
                Assert.AreEqual(expectedLibraray, libraryInfo.LibraryName.ToString((IFormatProvider)null));
                Assert.AreEqual(mean, libraryInfo.MeanLengthOfInsert.ToString((IFormatProvider)null));
            }
            ApplicationLog.WriteLine(@"Padena P1 : Map paired reads has been verified successfully");
        }

        /// <summary>
        /// Validate Add library information in existing libraries.
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        /// <param name="IsLibraryInfo">Is library info?</param>
        internal void AddLibraryInformation(string nodeName, bool IsLibraryInfo)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.FilePathNode);
            string expectedPairedReadsCount = utilityObj.xmlUtil.GetTextValue(
              nodeName, Constants.PairedReadsCountNode);
            string[] backwardReadsNode = utilityObj.xmlUtil.GetTextValues(nodeName,
              Constants.BackwardReadsNode);
            string[] forwardReadsNode = utilityObj.xmlUtil.GetTextValues(nodeName,
              Constants.ForwardReadsNode);
            string expectedLibraray = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.LibraryName);
            string expectedStdDeviation = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.StdDeviation);
            string mean = utilityObj.xmlUtil.GetTextValue(nodeName,
              Constants.Mean);

            IList<ISequence> sequenceReads = new List<ISequence>();
            IList<MatePair> pairedreads = new List<MatePair>();

            // Get the input reads 
            IEnumerable<ISequence> sequences = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequences = parser.Parse();

                foreach (ISequence seq in sequences)
                {
                    sequenceReads.Add(seq);
                }

                // Add a new library infomration.
                if (IsLibraryInfo)
                {
                    CloneLibraryInformation libraryInfo =
                      new CloneLibraryInformation();
                    libraryInfo.LibraryName = expectedLibraray;
                    libraryInfo.MeanLengthOfInsert = float.Parse(mean, (IFormatProvider)null);
                    libraryInfo.StandardDeviationOfInsert = float.Parse(expectedStdDeviation, (IFormatProvider)null);
                    CloneLibrary.Instance.AddLibrary(libraryInfo);
                }
                else
                {
                    CloneLibrary.Instance.AddLibrary(expectedLibraray,
                        float.Parse(mean, (IFormatProvider)null), float.Parse(expectedStdDeviation, (IFormatProvider)null));
                }

                // Convert reads to map paired reads.
                MatePairMapper pair = new MatePairMapper();
                pairedreads = pair.Map(sequenceReads);

                // Validate Map paired reads.
                Assert.AreEqual(expectedPairedReadsCount, pairedreads.Count.ToString((IFormatProvider)null));

                for (int index = 0; index < pairedreads.Count; index++)
                {
                    Assert.IsTrue(forwardReadsNode.Contains(new string(pairedreads[index].GetForwardRead(sequenceReads).Select(a => (char)a).ToArray())));
                    Assert.IsTrue(backwardReadsNode.Contains(new string(pairedreads[index].GetReverseRead(sequenceReads).Select(a => (char)a).ToArray())));
                    Assert.AreEqual(expectedStdDeviation,
                      pairedreads[index].StandardDeviationOfLibrary.ToString((IFormatProvider)null));
                    Assert.AreEqual(expectedLibraray, pairedreads[index].Library.ToString((IFormatProvider)null));
                    Assert.AreEqual(mean, pairedreads[index].MeanLengthOfLibrary.ToString((IFormatProvider)null));
                }
            }

            ApplicationLog.WriteLine(@"Padena P1 : Map paired reads has been verified successfully");
        }

        /// <summary>
        /// Validate building map reads to contigs.
        /// </summary>
        /// <param name="nodeName">xml node name used for a different testcases</param>
        /// <param name="isFullOverlap">True if full overlap else false</param>
        internal void ValidateMapReadsToContig(string nodeName, bool isFullOverlap)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);
            string daglingThreshold = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.DanglingLinkThresholdNode);
            string redundantThreshold = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.RedundantThreshold);
            string readMapLengthString = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ReadMapLength);
            string readStartPosString = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ReadStartPos);
            string contigStartPosString = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ContigStartPos);

            string[] expectedReadmapLength = readMapLengthString.Split(',');
            string[] expectedReadStartPos = readStartPosString.Split(',');
            string[] expectedContigStartPos = contigStartPosString.Split(',');

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

                this.SetSequenceReads(sequenceReads.ToList());
                this.CreateGraph();
                this.UnDangleGraph();
                this.ContigBuilder = new SimplePathContigBuilder();
                this.RemoveRedundancy();

                IEnumerable<ISequence> contigs = this.BuildContigs();

                IList<ISequence> sortedContigs = SortContigsData(contigs.ToList());
                ReadContigMapper mapper = new ReadContigMapper();
                ReadContigMap maps = mapper.Map(sortedContigs, sequenceReads, this.KmerLength);

                Assert.AreEqual(maps.Count, sequenceReads.Count());

                Dictionary<ISequence, IList<ReadMap>> readMaps = maps[sequenceReads.ToList()[0].ID];

                for (int i = 0; i < SortContigsData(readMaps.Keys.ToList()).Count; i++)
                {
                    IList<ReadMap> readMap = readMaps[SortContigsData(readMaps.Keys.ToList())[i]];

                    if (isFullOverlap)
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

            ApplicationLog.WriteLine("PADENA P1 :ReadContigMapper.Map() validation for Padena step6:step2 completed successfully");
        }

        /// <summary>
        /// Validate Filter contig nodes.
        /// </summary>
        /// <param name="nodeName">xml node name used for a differnt testcase.</param>
        /// <param name="isFirstContig">Is First Contig?</param>
        internal void ValidateFilterPaired(string nodeName, bool isFirstContig)
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
                this.DanglingLinksThreshold = Int32.Parse(daglingThreshold, (IFormatProvider)null);
                this.DanglingLinksPurger = new DanglingLinksPurger(Int32.Parse(daglingThreshold, (IFormatProvider)null));
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

                IList<ISequence> sortedContigs = SortContigsData(contigs.ToList());
                ReadContigMapper mapper = new ReadContigMapper();

                ReadContigMap maps = mapper.Map(
                    sortedContigs, sequenceReads, this.KmerLength);

                // Find map paired reads.
                MatePairMapper mapPairedReads = new MatePairMapper();
                ContigMatePairs pairedReads = mapPairedReads.MapContigToMatePairs(sequenceReads, maps);

                // Filter contigs based on the orientation.
                OrientationBasedMatePairFilter filter = new OrientationBasedMatePairFilter();
                ContigMatePairs contigpairedReads = filter.FilterPairedReads(pairedReads, 0);


                Assert.AreEqual(expectedContigPairedReadsCount,
                  contigpairedReads.Values.Count.ToString((IFormatProvider)null));

                Dictionary<ISequence, IList<ValidMatePair>> map = null;
                IList<ValidMatePair> valid = null;
                ISequence firstSeq = sortedContigs[0];
                ISequence secondSeq = sortedContigs[1];
                // Validate Contig paired reads after filtering contig sequences.
                if (isFirstContig)
                {

                    map = contigpairedReads[firstSeq];
                    valid = SortPairedReads(map[secondSeq], sequenceReads);
                }
                else
                {
                    map = contigpairedReads[secondSeq];
                    valid = SortPairedReads(map[firstSeq], sequenceReads);
                }

                for (int index = 0; index < valid.Count; index++)
                {
                    Assert.IsTrue((expectedForwardReadStartPos[index] ==
                          valid[index].ForwardReadStartPosition[0].ToString((IFormatProvider)null)
                          || (expectedForwardReadStartPos[index] ==
                          valid[index].ForwardReadStartPosition[1].ToString((IFormatProvider)null))));

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
            ApplicationLog.WriteLine("PADENA P1 : FilterPairedReads() validation for Padena step6:step4 completed successfully");
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
                this.DanglingLinksThreshold = Int32.Parse(daglingThreshold, (IFormatProvider)null);
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

                IList<ISequence> sortedContigs = SortContigsData(contigs.ToList());
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
                ISequence firstSeq = sortedContigs[0];
                ISequence secondSeq = sortedContigs[1];
                if (contigpairedReads.ContainsKey(firstSeq))
                {
                    map = contigpairedReads[firstSeq];
                }
                else
                {
                    map = contigpairedReads[secondSeq];
                }

                if (map.ContainsKey(firstSeq))
                {
                    valid = map[firstSeq];
                }
                else
                {
                    valid = map[secondSeq];
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

            ApplicationLog.WriteLine("PADENA P1 : DistanceCalculator() validation for Padena step6:step5 completed successfully");
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
            string library = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.LibraryName);
            string StdDeviation = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.StdDeviation);
            string mean = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Mean);
            string expectedDepth = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.DepthNode);
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

                IList<ISequence> sortedContigs = SortContigsData(contigs.ToList());
                ReadContigMapper mapper = new ReadContigMapper();

                ReadContigMap maps = mapper.Map(sortedContigs, sequenceReads, this.KmerLength);

                // Find map paired reads.
                CloneLibrary.Instance.AddLibrary(library, float.Parse(mean, null), float.Parse(StdDeviation, null));
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

                // Assemble paths.
                PathPurger pathsAssembler = new PathPurger();
                pathsAssembler.PurgePath(paths);

                // Get sequences from assembled path.
                IList<ISequence> seqList = paths.Select(temp => temp.BuildSequenceFromPath(graph, Int32.Parse(kmerLength, null))).ToList();

                //Validate assembled sequence paths.
                foreach (string sequence in seqList.Select(t => t.ConvertToString()))
                {
                    Assert.IsTrue(assembledPath.Contains(sequence), "Failed to locate " + sequence);
                }
            }

            ApplicationLog.WriteLine("PADENA P1 : AssemblePath() validation for Padena step6:step7 completed successfully");
        }

        /// <summary>
        /// Sort Contig List based on the contig sequence
        /// </summary>
        /// <param name="contigsList">xml node name used for different testcases</param>
        private static IList<ISequence> SortContigsData(IList<ISequence> contigsList)
        {
            return (from ContigData in contigsList
                    orderby new string(ContigData.Select(a => (char)a).ToArray())
                    select ContigData).ToList();
        }

        ///<summary>
        /// Sort Valid Paired reads based on forward reads.
        /// For consistent output due to parallel implementation.
        /// </summary>
        /// <param name="list">List of Paired Reads</param>
        /// <param name="reads">Input list of reads.</param>
        /// <returns>Sorted List of Paired reads</returns>
        private static IList<ValidMatePair> SortPairedReads(IList<ValidMatePair> list,
            IEnumerable<ISequence> reads)
        {
            return (from valid in list
                    orderby new string(valid.PairedRead.GetForwardRead(reads).Select(a => (char)a).ToArray())
                    select valid).ToList();
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
            string[] expectedScaffoldNodes = utilityObj.xmlUtil.GetTextValues(nodeName,Constants.ScaffoldNodes);
            string libraray = utilityObj.xmlUtil.GetTextValue(nodeName,Constants.LibraryName);
            string stdDeviation = utilityObj.xmlUtil.GetTextValue(nodeName,Constants.StdDeviation);
            string mean = utilityObj.xmlUtil.GetTextValue(nodeName,Constants.Mean);
            string expectedDepth = utilityObj.xmlUtil.GetTextValue(nodeName,Constants.DepthNode);

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

                IList<ISequence> sortedContigs = SortContigsData(contigs.ToList());
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

                foreach (KeyValuePair<Node, Edge> kvp in scaffold)
                {
                    ISequence seq = graph.GetNodeSequence(kvp.Key);
                    string sequence = seq.ConvertToString();
                    string reversedSequence = seq.GetReverseComplementedSequence().ConvertToString();

                    Assert.IsTrue(expectedScaffoldNodes.Contains(sequence)
                               || expectedScaffoldNodes.Contains(reversedSequence),
                               "Failed to find " + sequence + ", or " + reversedSequence);
                }
            }

            ApplicationLog.WriteLine("PADENA P1 : FindPaths() validation for Padena step6:step6 completed successfully");
        }

        /// <summary>
        /// Validate scaffold sequence for a given input reads.
        /// </summary>
        /// <param name="nodeName">xml node name used for different testcases</param>
        internal void ValidateScaffoldSequence(string nodeName)
        {
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);
            string daglingThreshold = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.DanglingLinkThresholdNode);
            string redundantThreshold = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.RedundantThreshold);
            string libraray = utilityObj.xmlUtil.GetTextValue(nodeName,Constants.LibraryName);
            string stdDeviation = utilityObj.xmlUtil.GetTextValue(nodeName,Constants.StdDeviation);
            string mean = utilityObj.xmlUtil.GetTextValue(nodeName,Constants.Mean);
            string inputRedundancy = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.InputRedundancy);
            string expectedSeq = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ScaffoldSeq);
            string[] scaffoldSeqNodes = expectedSeq.Split(',');

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
                this.UnDangleGraph();

                // Build contig.
                this.RemoveRedundancy();
                IEnumerable<ISequence> contigs = this.BuildContigs();

                // Find map paired reads.
                CloneLibrary.Instance.AddLibrary(libraray, float.Parse(mean, null),
                 float.Parse(stdDeviation, null));
                IEnumerable<ISequence> scaffoldSeq;

                using (GraphScaffoldBuilder scaffold = new GraphScaffoldBuilder())
                {
                    scaffoldSeq = scaffold.BuildScaffold(
                       sequenceReads, contigs.ToList(), this.KmerLength, redundancy: Int32.Parse(inputRedundancy, null));
                }

                AlignmentHelpers.CompareSequenceLists(new HashSet<string>(scaffoldSeqNodes), scaffoldSeq.ToList());
            }

            ApplicationLog.WriteLine("PADENA P1 : Scaffold sequence : validation for Padena step6:step8 completed successfully");
        }

        /// <summary>
        /// Validate Parallel Denovo Assembly Assembled sequences.
        /// </summary>
        /// <param name="nodeName">XML node used to validate different test scenarios</param>
        internal void ValidatePadenaAssembledSeqs(string nodeName)
        {
            // Get values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
               nodeName, Constants.FilePathNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.KmerLengthNode);
            string daglingThreshold = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DanglingLinkThresholdNode);
            string redundantThreshold = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.RedundantThreshold);
            string libraray = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.LibraryName);
            string stdDeviation = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.StdDeviation);
            string mean = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.Mean);
            string assembledSequences = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.SequencePathNode);
            string assembledSeqCount = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.AssembledSeqCountNode);
            string[] updatedAssembledSeqs = assembledSequences.Split(',');

            // Get the input reads and build kmers
            IEnumerable<ISequence> sequenceReads = null;
            using (FastAParser parser = new FastAParser(filePath))
            {
                sequenceReads = parser.Parse();

                // Create a ParallelDeNovoAssembler instance.
                ParallelDeNovoAssembler denovoObj = null;
                try
                {
                    denovoObj = new ParallelDeNovoAssembler();

                    denovoObj.KmerLength = Int32.Parse(kmerLength, (IFormatProvider)null);
                    denovoObj.DanglingLinksThreshold = Int32.Parse(daglingThreshold, (IFormatProvider)null);
                    denovoObj.RedundantPathLengthThreshold = Int32.Parse(redundantThreshold, (IFormatProvider)null);

                    CloneLibrary.Instance.AddLibrary(libraray, float.Parse(mean, (IFormatProvider)null),
                    float.Parse(stdDeviation, (IFormatProvider)null));

                    byte[] symbols = sequenceReads.ElementAt(0).Alphabet.GetSymbolValueMap();

                    IDeNovoAssembly assembly =
                        denovoObj.Assemble(sequenceReads.Select(a => new Sequence(Alphabets.DNA, a.Select(b => symbols[b]).ToArray()) { ID = a.ID }), true);

                    IList<ISequence> assembledSequenceList = assembly.AssembledSequences.ToList();

                    // Validate assembled sequences.
                    Assert.AreEqual(assembledSeqCount, assembledSequenceList.Count.ToString((IFormatProvider)null));

                    for (int i = 0; i < assembledSequenceList.Count; i++)
                    {
                        Assert.IsTrue(assembledSequences.Contains(
                       new string(assembledSequenceList[i].Select(a => (char)a).ToArray()))
                        || updatedAssembledSeqs.Contains(
                        new string(assembledSequenceList[i].GetReverseComplementedSequence().Select(a => (char)a).ToArray())));
                    }
                }
                finally
                {
                    if (denovoObj != null)
                        denovoObj.Dispose();
                }
            }

            ApplicationLog.WriteLine("Padena P1 : Assemble() validation for Padena step6:step7 completed successfully");
        }

        #endregion
    }
}
