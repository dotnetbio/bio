/****************************************************************************
 * PamSamBvtTestCases.cs
 * 
 *  This file contains the MuscleMultipleSequenceAlignment Bvt test cases.
 * 
***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Bio.Algorithms.Alignment.MultipleSequenceAlignment;
using Bio.IO;
using Bio.IO.FastA;
using Bio.SimilarityMatrices;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.TestAutomation.Pamsam
{
    /// <summary>
    ///     The class contains Bvt test cases to confirm Muscle MSA alignment.
    /// </summary>
    [TestClass]
    public class PamSamBvtTestCases
    {
        #region Enums

        /// <summary>
        ///     Different profile aligner method types
        /// </summary>
        private enum AlignType
        {
            AlignSimpleAllParams,
            AlignSimpleOnlyProfiles,
            AlignAllParams
        }

        /// <summary>
        ///     Different mathematical functions present in MsaUtils
        /// </summary>
        private enum FunctionType
        {
            Correlation,
            FindMaxIndex,
            JensenShanonDivergence,
            KullbackLeiblerDistance
        }

        /// <summary>
        ///     Collection of different score functions
        /// </summary>
        private enum ScoreType
        {
            QScore,
            TCScore,
            Offset,
            MultipleAlignmentScoreFunction,
            PairWiseScoreFunction
        }

        #endregion Enums

        #region Global Variables

        private readonly Utility utilityObj = new Utility(@"TestUtils\MSAConfig.xml");

        /// <summary>
        ///     Initialize the expected score.
        /// </summary>
        private string expectedScore = string.Empty;

        /// <summary>
        ///     Initialize expected aligned sequence list.
        /// </summary>
        private List<ISequence> expectedSequences;

        /// <summary>
        ///     Initialize gap extend penalty.
        /// </summary>
        private int gapExtendPenalty = -3;

        /// <summary>
        ///     Initialize the gap open penalty
        /// </summary>
        private int gapOpenPenalty = -8;

        /// <summary>
        ///     kmer length to generate kmer distance matrix
        /// </summary>
        private int kmerLength = 2;

        /// <summary>
        ///     Initialize input sequence list.
        /// </summary>
        private IList<ISequence> lstSequences;

        /// <summary>
        ///     Set it with NW/ SW profiler
        /// </summary>
        private IProfileAligner profileAligner;

        /// <summary>
        ///     Similarity matrix object
        /// </summary>
        private SimilarityMatrix similarityMatrix;

        /// <summary>
        ///     Initialize the expected score of Stage1.
        /// </summary>
        private string stage1ExpectedScore = string.Empty;

        /// <summary>
        ///     Initialize expected aligned sequence list for stage1.
        /// </summary>
        private List<ISequence> stage1ExpectedSequences;

        /// <summary>
        ///     Initialize the expected score of Stage2.
        /// </summary>
        private string stage2ExpectedScore = string.Empty;

        /// <summary>
        ///     Initialize expected aligned sequence list for stage2.
        /// </summary>
        private List<ISequence> stage2ExpectedSequences;

        #endregion

        #region Constructors

        /// <summary>
        ///     Static constructor to open log and make other settings needed for test
        /// </summary>
        static PamSamBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion

        #region Test Cases

        #region PamSam TestCases

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences and score
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequences()
        {
            ValidatePamsamAlign(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode,
                                ProfileAlignerNames.NeedlemanWunschProfileAligner, true);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences
        ///     and score with distance matrix method name as ModifiedMuscle
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesAndModifiedMuscle()
        {
            ValidatePamsamAlignWithDistanceFunctionaTypes(
                Constants.MuscleDnaSequenceWithModifiedMuscleDistanceMethodNodeName,
                Constants.ExpectedScoreNode,
                DistanceFunctionTypes.ModifiedMUSCLE,
                ProfileAlignerNames.NeedlemanWunschProfileAligner, false);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences
        ///     and score with distance matrix method name as EuclieanDistance
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesAndEuclieanDistance()
        {
            ValidatePamsamAlignWithDistanceFunctionaTypes(
                Constants.MuscleDnaSequenceNode,
                Constants.ExpectedScoreNode,
                DistanceFunctionTypes.EuclideanDistance,
                ProfileAlignerNames.NeedlemanWunschProfileAligner, true);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences
        ///     and score with distance matrix method name as CoVariance
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesAndCOVariance()
        {
            ValidatePamsamAlignWithDistanceFunctionaTypes(
                Constants.MuscleDnaSequenceWithCoVarianceNodeName,
                Constants.ExpectedScoreNode,
                DistanceFunctionTypes.CoVariance,
                ProfileAlignerNames.NeedlemanWunschProfileAligner, false);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences
        ///     and score with distance matrix method name as PearsonCorrelation
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesAndPearsonCorrelation()
        {
            ValidatePamsamAlignWithDistanceFunctionaTypes(
                Constants.MuscleDnaSequenceWithPearsonCorrelationDistanceMethodNodeName,
                Constants.ExpectedScoreNode,
                DistanceFunctionTypes.PearsonCorrelation,
                ProfileAlignerNames.NeedlemanWunschProfileAligner, false);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences
        ///     and score with Hierarchical Clustering method name as Average
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesAndAverageMethod()
        {
            ValidatePamsamAlignWithUpdateDistanceMethodTypes(
                Constants.MuscleDnaSequenceNode,
                Constants.ExpectedScoreNode,
                UpdateDistanceMethodsTypes.Average,
                ProfileAlignerNames.NeedlemanWunschProfileAligner, true);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences and score with
        ///     Hierarchical Clustering method name as Complete
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesAndCompleteMethod()
        {
            ValidatePamsamAlignWithUpdateDistanceMethodTypes(
                Constants.MuscleDnaSequenceNode,
                Constants.ExpectedScoreNode,
                UpdateDistanceMethodsTypes.Complete,
                ProfileAlignerNames.NeedlemanWunschProfileAligner, true);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences and score with
        ///     Hierarchical Clustering method name as Single
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesAndSingleMethod()
        {
            ValidatePamsamAlignWithUpdateDistanceMethodTypes(
                Constants.MuscleDnaSequenceWithSingleDistanceMethodNodeName,
                Constants.ExpectedScoreNode,
                UpdateDistanceMethodsTypes.Single,
                ProfileAlignerNames.NeedlemanWunschProfileAligner, true);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences and score with
        ///     Hierarchical Clustering method name as WeightedMAFFT
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesAndWeightedMafftMethod()
        {
            ValidatePamsamAlignWithUpdateDistanceMethodTypes(
                Constants.MuscleDnaSequenceWithWeightedMafftNode,
                Constants.ExpectedScoreNode,
                UpdateDistanceMethodsTypes.WeightedMAFFT,
                ProfileAlignerNames.NeedlemanWunschProfileAligner, true);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences and score.
        ///     Profile score method name as InnerProduct
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesAndInnerProduct()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleDnaSequenceNode,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.InnerProduct, true);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences and score.
        ///     Profile score method name as JensenShannonDivergence
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesAndJensenShannonDivergence()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleDnaWithJensenShannonDivergence,
                Constants.ExpectedScoreNode, ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.JensenShannonDivergence, true);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences and score.
        ///     Profile score method name as LogExponentialInnerProduct
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesAndLogExponentialInnerProduct()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleDnaWithLogExponentialInnerProduct,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.LogExponentialInnerProduct, false);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences and score.
        ///     Profile score method name as LogExponentialInnerProductShifted
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesAndLogExponentialInnerProductShifted()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleDnaSequenceWithLogExponentialInnerProductShiftedNodeName,
                Constants.ExpectedScoreNode, ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.LogExponentialInnerProductShifted, true);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences and score.
        ///     Profile score method name as PearsonCorrelation
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesAndPearsonCorrelationProfileScore()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleDnaWithJensenShannonDivergence,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.PearsonCorrelation, true);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences and score.
        ///     Profile score method name as SymmetrizedEntropy
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesAndSymmetrizedEntropy()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleDnaWithSymmetrizedEntropy,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.SymmetrizedEntropy, true);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences and score.
        ///     Profile score method name as WeightedEuclideanDistance
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesAndWeightedEuclideanDistance()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleDnaSequenceWithWeightedEuclideanDistanceNodeName,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.WeightedEuclideanDistance, true);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences and score.
        ///     Profile score method name as WeightedInnerProduct
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesAndWeightedInnerProduct()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleDnaSequenceWithWeightedInnerProduct,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.WeightedInnerProduct, true);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences and score.
        ///     Profile score method name as WeightedInnerProductShifted
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesAndWeightedInnerProductShifted()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MusclerDnaSequenceWithWeightedInnerProductShiftedNode,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.WeightedInnerProductShifted, true);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment Stage1 using its aligned sequences and score
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamStage1WithDnaSequences()
        {
            ValidatePamsamAlignStage1(Constants.MuscleDnaSequenceNode,
                                      Constants.ExpectedScoreNode,
                                      UpdateDistanceMethodsTypes.Average, DistanceFunctionTypes.EuclideanDistance,
                                      ProfileAlignerNames.NeedlemanWunschProfileAligner,
                                      ProfileScoreFunctionNames.InnerProduct);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment Stage2 using its aligned sequences and score
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamStage2WithDnaSequences()
        {
            ValidatePamsamAlignStage2(Constants.MuscleDnaSequenceNode,
                                      Constants.ExpectedScoreNode,
                                      UpdateDistanceMethodsTypes.Average, DistanceFunctionTypes.EuclideanDistance,
                                      ProfileAlignerNames.NeedlemanWunschProfileAligner,
                                      ProfileScoreFunctionNames.InnerProduct);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment Stage3 using its aligned sequences and score
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamStage3WithDnaSequences()
        {
            ValidatePamsamAlignStage3(Constants.MuscleDnaSequenceNode,
                                      Constants.ExpectedScoreNode,
                                      UpdateDistanceMethodsTypes.Average, DistanceFunctionTypes.EuclideanDistance,
                                      ProfileAlignerNames.NeedlemanWunschProfileAligner,
                                      ProfileScoreFunctionNames.InnerProduct);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences and score.
        ///     Profile score method name as InnerProductFast
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesAndInnerProductFast()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MultipleNWProfilerDnaSequenceWithInnerProductFastNode,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.InnerProductFast, true);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences and score.
        ///     Profile score method name as LogExponentialInnerProductFast
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesAndLogExponentialInnerProductFast()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleDnaWithLogExponentialInnerProductFastNode,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.LogExponentialInnerProductFast, true);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences and score.
        ///     Profile score method name as LogExponentialInnerProductShiftedFast
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesAndLogExponentialInnerProductShiftedFast()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleDnaWithLogExponentialInnerProductFastNode,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.LogExponentialInnerProductShiftedFast, true);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences and score.
        ///     Profile score method name as WeightedEuclideanDistanceFast
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesAndWeightedEuclideanDistanceFast()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleDnaSequenceWithWeightedEuclideanDistanceNodeName,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.WeightedEuclideanDistanceFast, true);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment using its aligned sequences and score.
        ///     Profile score method name as WeightedInnerProductShiftedFast
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesAndWeightedInnerProductShiftedFast()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleDnaSequenceWithWeightedInnerProductShiftedFastNode,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.WeightedInnerProductShiftedFast, true);
        }

        /// <summary>
        ///     Validate Muscle sequence alignment with sequence weights
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateUseWeightsPamsamWithDnaSequences()
        {
            ValidatePamsamAlign(Constants.MuscleDnaSequenceWithWeightsNode,
                                Constants.ExpectedScoreNode, UpdateDistanceMethodsTypes.Average,
                                DistanceFunctionTypes.EuclideanDistance,
                                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                                ProfileScoreFunctionNames.InnerProductFast, true, false, false);
        }

        /// <summary>
        ///     Validate the faster version of muscle multiple sequence alignment
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamWithDnaSequencesWithFasterVersion()
        {
            ValidatePamsamAlign(
                Constants.MuscleDnaWithJensenShannonDivergence,
                Constants.ExpectedScoreNode, UpdateDistanceMethodsTypes.Average,
                DistanceFunctionTypes.EuclideanDistance, ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.InnerProductFast, false, true, false);
        }

        #endregion

        #region KmerDistanceMatrix

        /// <summary>
        ///     Validate kmerdistancematrix for stage1 with default distance function name
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamKmerDistanceMatrixStage1()
        {
            ValidateKmerDistanceMatrixStage1(Constants.KmerDistanceMatrixNode, 3);
        }

        /// <summary>
        ///     Validate kmerdistancematrix for stage1 with EuclieanDistance distance function name
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamKmerDistanceMatrixWithEuclieanDistance()
        {
            ValidateKmerDistanceMatrixStage1(Constants.KmerDistanceMatrixNode, 3,
                                             DistanceFunctionTypes.EuclideanDistance);
        }

        /// <summary>
        ///     Validate kmerdistancematrix for stage1 with PearsonCorrelation distance function name
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamKmerDistanceMatrixWithPearsonCorrelation()
        {
            ValidateKmerDistanceMatrixStage1(Constants.KmerDistanceMatrixWithPearsonCorrelation,
                                             kmerLength, DistanceFunctionTypes.PearsonCorrelation);
        }

        /// <summary>
        ///     Validate kmerdistancematrix for stage1 with CoVariance distance function name
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamKmerDistanceMatrixWithCOVariance()
        {
            ValidateKmerDistanceMatrixStage1(Constants.KmerDistanceMatrixWithCoVariance, kmerLength,
                                             DistanceFunctionTypes.CoVariance);
        }

        /// <summary>
        ///     Validate kmerdistancematrix for stage1 with ModifiedMUSCLE distance function name
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamKmerDistanceMatrixWithModifiedMuscle()
        {
            ValidateKmerDistanceMatrixStage1(Constants.KmerDistanceMatrixWithModifiedMuscle, kmerLength,
                                             DistanceFunctionTypes.ModifiedMUSCLE);
        }

        #endregion

        #region HierarchicalClusteringStage1 & Stage2

        /// <summary>
        ///     Validate HierarchicalClustering for stage1 using kmer distance matrix
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamHierarchicalClusteringStage1()
        {
            ValidateHierarchicalClusteringStage1(Constants.HierarchicalClusteringNode,
                                                 kmerLength);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1 using kmer distance matrix
        ///     and hierarchical clustering method name as Average
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamHierarchicalClusteringWithAverage()
        {
            ValidateHierarchicalClusteringStage1(Constants.HierarchicalClusteringNode,
                                                 kmerLength, UpdateDistanceMethodsTypes.Average);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1 using kmer distance matrix
        ///     and hierarchical clustering method name as Single
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamHierarchicalClusteringWithSingle()
        {
            ValidateHierarchicalClusteringStage1(Constants.HierarchicalClusteringWeightedMAFFT,
                                                 kmerLength, UpdateDistanceMethodsTypes.Single);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1 using kmer distance matrix
        ///     and hierarchical clustering method name as Complete
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamHierarchicalClusteringWithComplete()
        {
            ValidateHierarchicalClusteringStage1(Constants.HierarchicalClusteringNode,
                                                 kmerLength, UpdateDistanceMethodsTypes.Complete);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1 using kmer distance matrix
        ///     and hierarchical clustering method name as WeightedMAFFT
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamHierarchicalClusteringWithWeightedMafft()
        {
            ValidateHierarchicalClusteringStage1(Constants.HierarchicalClusteringWeightedMAFFT, kmerLength,
                                                 UpdateDistanceMethodsTypes.WeightedMAFFT);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1 using kimura distance matrix
        ///     and stage 1 aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamHierarchicalClusteringStage2()
        {
            ValidateHierarchicalClusteringStage2(Constants.HierarchicalClusteringStage2Node);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1
        ///     using kimura distance matrix with hierarchical method name as Average
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamHierarchicalClusteringStage2WithAverage()
        {
            ValidateHierarchicalClusteringStage2(Constants.HierarchicalClusteringStage2Node,
                                                 UpdateDistanceMethodsTypes.Average);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1
        ///     using kimura distance matrix with hierarchical method name as Complete
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamHierarchicalClusteringStage2WithComplete()
        {
            ValidateHierarchicalClusteringStage2(Constants.HierarchicalClusteringStage2WithCompleteNode,
                                                 UpdateDistanceMethodsTypes.Complete);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1
        ///     using kimura distance matrix with hierarchical method name as Single
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamHierarchicalClusteringStage2WithSingle()
        {
            ValidateHierarchicalClusteringStage2(Constants.HierarchicalClusteringStage2WithSingleNode,
                                                 UpdateDistanceMethodsTypes.Single);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1
        ///     using kimura distance matrix with hierarchical method name as WeightedMAFFT
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamHierarchicalClusteringStage2WithWeightedMafft()
        {
            ValidateHierarchicalClusteringStage2(Constants.HierarchicalClusteringStage2WithWeightedMAFFT,
                                                 UpdateDistanceMethodsTypes.WeightedMAFFT);
        }

        #endregion

        #region BinaryTreeStage1 & Stage2

        /// <summary>
        ///     Validate HierarchicalClustering for stage1 using kmer distance matrix
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamBinaryTreeStage1()
        {
            ValidateBinaryTreeStage1(Constants.BinaryTreeNode, kmerLength);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1 using kmer distance matrix
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamBinaryTreeStage2()
        {
            ValidateBinaryTreeStage2(Constants.BinaryTreeStage2Node);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1 using kmer distance matrix
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamBinaryTreeSeparateSequencesByCutTree()
        {
            ValidateBinaryTreeSeparateSequencesByCutTree(3, Constants.BinaryTreeNode, 4);
        }

        #endregion

        #region ProfileAligner & ProgressiveAlignment

        /// <summary>
        ///     Validate ProgressiveAligner using stage1 sequences.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamPrgressiveAlignerStage1()
        {
            ValidateProgressiveAlignmentStage1(Constants.MuscleDnaSequenceNode);
        }

        /// <summary>
        ///     Validate Profile Aligner GenerateEString() method using two profiles of sub trees.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamProfileAlignerGenerateEString()
        {
            ValidateProfileAlignerGenerateEString(Constants.ProfileAligner, 4);
        }

        /// <summary>
        ///     Validate Profile Aligner GenerateSequencesEString() method using two profiles of sub trees.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamProfileAlignerGenerateSequencesEString()
        {
            ValidateProfileAlignerGenerateSequenceString(Constants.ProfileAlignerWithAlignmentNode,
                                                         4);
        }

        #endregion

        #region KimuraDistanceMatrix

        /// <summary>
        ///     Validate kimura distance matrix using stage1 aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePamsamKimuraDistanceMatrix()
        {
            ValidateKimuraDistanceMatrix(Constants.KimuraDistanceMatrix);
        }

        #endregion

        #region NeedlemanProfileAlignerSerial

        /// <summary>
        ///     Creates binarytree using stage2 sequences and
        ///     cut the binary tree at an random edge to get two profiles.
        ///     Create NeedlemanWunschProfileAlignerSerial instance
        ///     and execute AlignSimple(Profile A,Profile B).
        ///     Validate the IProfileAlignment properties.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerSerialAlignSimple()
        {
            ValidateProfileAlignerAlign(Constants.ProfileAligner,
                                        Constants.SerialProcess, 3, AlignType.AlignSimpleOnlyProfiles);
        }

        /// <summary>
        ///     Creates binarytree using stage2 sequences
        ///     and cut the binary tree at an random edge to get two profiles.
        ///     Create NeedlemanWunschProfileAlignerSerial instance
        ///     and execute AlignSimple(sm, gapOpenPenalty, Profile A,Profile B).
        ///     Validate the IProfileAlignment properties.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerSerialAlignSimpleWithAllParams()
        {
            ValidateProfileAlignerAlign(Constants.ProfileAligner,
                                        Constants.SerialProcess, 3, AlignType.AlignSimpleAllParams);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "WeightedInnerProductCached".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerSerialWithWeightedInnerProductCached()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithWeightedInnerProductCachedNode,
                Constants.SerialProcess, ProfileScoreFunctionNames.WeightedInnerProductCached,
                5);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "WeightedInnerProductFast".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerSerialWithWeightedInnerProductFast()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithWeightedInnerProductCachedNode, Constants.SerialProcess,
                ProfileScoreFunctionNames.WeightedInnerProductFast, 5);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "WeightedInnerProduct".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerSerialWithWeightedInnerProduct()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithWeightedInnerProductCachedNode, Constants.SerialProcess,
                ProfileScoreFunctionNames.WeightedInnerProduct, 5);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "WeightedInnerProductShiftedFast".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerSerialWithWeightedInnerProductShiftedFast()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithWeightedInnerProductCachedNode, Constants.SerialProcess,
                ProfileScoreFunctionNames.WeightedInnerProductShiftedFast, 5);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "WeightedEuclideanDistanceFast".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerSerialWithWeightedEuclideanDistanceFast()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithWeightedEuclideanDistanceFastNode, Constants.SerialProcess,
                ProfileScoreFunctionNames.WeightedEuclideanDistanceFast, 5);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "WeightedEuclideanDistance".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerSerialWithWeightedEuclideanDistance()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithWeightedEuclideanDistanceFastNode, Constants.SerialProcess,
                ProfileScoreFunctionNames.WeightedEuclideanDistance, 5);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "PearsonCorrelation".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerSerialWithPearsonCorrelation()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithWeightedInnerProductCachedNode, Constants.SerialProcess,
                ProfileScoreFunctionNames.PearsonCorrelation, 5);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "LogExponentialInnerProductShiftedFast".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerSerialWithLogExponentialInnerProductShiftedFast()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithLogExponentialInnerProductShiftedFastNode,
                Constants.SerialProcess,
                ProfileScoreFunctionNames.LogExponentialInnerProductShiftedFast, 3);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "LogExponentialInnerProductShifted".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerSerialWithLogExponentialInnerProductShifted()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithWeightedEuclideanDistanceFastNode, Constants.SerialProcess,
                ProfileScoreFunctionNames.LogExponentialInnerProductShifted, 5);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "LogExponentialInnerProduct".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerSerialWithLogExponentialInnerProduct()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithWeightedInnerProductNode, Constants.SerialProcess,
                ProfileScoreFunctionNames.LogExponentialInnerProduct, 7);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "LogExponentialInnerProductFast".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerSerialWithLogExponentialInnerProductFast()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithLogExponentialInnerProductFastNode, Constants.SerialProcess,
                ProfileScoreFunctionNames.LogExponentialInnerProductFast, 5);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "JensenShannonDivergence".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerSerialWithJensenShannonDivergence()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAligner, Constants.SerialProcess,
                ProfileScoreFunctionNames.JensenShannonDivergence, 8);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with profilescorefunction
        ///     name as "InnerProductFast".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerSerialWithInnerProductFast()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAligner, Constants.SerialProcess,
                ProfileScoreFunctionNames.InnerProductFast, 8);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with profilescorefunction
        ///     name as "InnerProduct".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerSerialWithInnerProduct()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAligner, Constants.SerialProcess,
                ProfileScoreFunctionNames.InnerProduct, 8);
        }

        /// <summary>
        ///     Creates binarytree using stage2 sequences and cut the binary tree at an
        ///     random edge to get two profiles.
        ///     Create NeedlemanWunschProfileAlignerParallel instance and execute Align(Profile A,Profile B).
        ///     Validate the IProfileAlignment properties.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerSerialAlign()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithWeightedInnerProductNode, Constants.SerialProcess,
                ProfileScoreFunctionNames.WeightedInnerProduct, 3);
        }

        /// <summary>
        ///     Creates binarytree using stage2 sequences and cut the binary tree at an
        ///     random edge to get two profiles.
        ///     Create NeedlemanWunschProfileAlignerParallel instance and execute
        ///     AlignSimple(Profile A,Profile B).
        ///     Validate the IProfileAlignment properties.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerSerialAlignWithAllParams()
        {
            ValidateProfileAlignerAlign(Constants.ProfileAligner,
                                        Constants.SerialProcess, 2, AlignType.AlignAllParams);
        }

        #endregion

        #region NeedlemanProfileAlignerParallel

        /// <summary>
        ///     Creates binarytree using stage1 sequences and cut the binary tree
        ///     at an random edge to get two profiles.
        ///     Create NeedlemanWunschProfileAlignerParallel instance and execute
        ///     AlignSimple(Profile A,Profile B).
        ///     Validate the IProfileAlignment properties.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerParallelAlignSimple()
        {
            ValidateProfileAlignerAlign(Constants.ProfileAligner,
                                        Constants.ParallelProcess, 3, AlignType.AlignSimpleOnlyProfiles);
        }

        /// <summary>
        ///     Creates binarytree using stage1 sequences and cut the binary tree
        ///     at an random edge to get two profiles.
        ///     Create NeedlemanWunschProfileAlignerParallel instance and execute
        ///     AlignSimple(sm, gappenalty,Profile A,Profile B).
        ///     Validate the IProfileAlignment properties.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerParallelAlignSimpleAllParams()
        {
            ValidateProfileAlignerAlign(Constants.ProfileAligner,
                                        Constants.ParallelProcess, 3, AlignType.AlignSimpleAllParams);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "WeightedInnerProductCached".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerParallelWithWeightedInnerProductCached()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithWeightedInnerProductCachedNode,
                Constants.ParallelProcess,
                ProfileScoreFunctionNames.WeightedInnerProductCached,
                5);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "WeightedInnerProductFast".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerParallelWithWeightedInnerProductFast()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithWeightedInnerProductCachedNode,
                Constants.ParallelProcess,
                ProfileScoreFunctionNames.WeightedInnerProductFast,
                5);
        }

        /// <summary>
        ///     Creates binarytree using stage1 sequences and cut the binary tree
        ///     at an random edge to get two profiles.
        ///     Create NeedlemanWunschProfileAlignerParallel instance and execute
        ///     Align(sm, gappenalty,Profile A,Profile B).
        ///     Validate the IProfileAlignment properties.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNWProfileAlignerParallelAlignAllParams()
        {
            ValidateProfileAlignerAlign(Constants.ProfileAligner,
                                        Constants.ParallelProcess, 3, AlignType.AlignAllParams);
        }

        #endregion

        #region MsaUtils

        /// <summary>
        ///     Validate the QScore of 12 Pamsam aligned dna sequences against benmark sequences
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateAlignmentQualityScore()
        {
            ValidateAlignmentScore(Constants.DnaWith12SequencesNode, ScoreType.QScore);
        }

        /// <summary>
        ///     Validate the TCScore of 12 Pamsam aligned dna sequences against benmark sequences
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateAlignmentTCScore()
        {
            ValidateAlignmentScore(Constants.DnaWith12SequencesNode, ScoreType.TCScore);
        }

        /// <summary>
        ///     Execute the CalculateOffset().
        ///     Validate the number of residues whose position index will be negative
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateAlignmentOffset()
        {
            ValidateAlignmentScore(Constants.DnaWith12SequencesNode, ScoreType.Offset);
        }

        /// <summary>
        ///     Validate the Multiple alignment score of Pamsam aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMultipleAlignmentScore()
        {
            ValidateAlignmentScore(Constants.DnaWith12SequencesNode,
                                   ScoreType.MultipleAlignmentScoreFunction);
        }

        /// <summary>
        ///     Validate the pairwise score function of a pair of aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePairWiseScoreFunction()
        {
            ValidateAlignmentScore(Constants.DnaWith12SequencesNode,
                                   ScoreType.PairWiseScoreFunction);
        }

        /// <summary>
        ///     Get two profiles after cutting the edge of binary tree.
        ///     Validate the correlation value of two profiles.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateCorrelation()
        {
            ValidateFunctionCalculations(Constants.DnaFunctionsNode,
                                         4, FunctionType.Correlation);
        }

        /// <summary>
        ///     Get profiles after cutting the edge of binary tree.
        ///     Validate the max index value of a profile.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFindMaxIndex()
        {
            ValidateFunctionCalculations(Constants.DnaFunctionsNode,
                                         4, FunctionType.FindMaxIndex);
        }

        /// <summary>
        ///     Get profiles after cutting the edge of binary tree.
        ///     Validate the JensenShanonDivergence value of two profiles.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateJensenShanonDivergence()
        {
            ValidateFunctionCalculations(Constants.DnaFunctionsNode,
                                         4, FunctionType.JensenShanonDivergence);
        }

        /// <summary>
        ///     Get profiles after cutting the edge of binary tree.
        ///     Validate the KullbackLeiblerDistance of two profiles.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateKullbackLeiblerDistance()
        {
            ValidateFunctionCalculations(Constants.DnaFunctionsNode,
                                         4, FunctionType.KullbackLeiblerDistance);
        }

        /// <summary>
        ///     Get pam sam aligned sequences. Execute UnAlign() method
        ///     and verify that it does not contains gap
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateUNAlign()
        {
            ValidateUNAlign(Constants.DnaWith12SequencesNode);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        ///     Read from xml config and initialize all member variables
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="expectedScoreNode">Expected score node</param>
        private void Initialize(string nodeName, string expectedScoreNode)
        {
            // Read all the input sequences from xml config file
            IAlphabet alphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                     Constants.AlphabetNameNode));
            string sequenceString1 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Sequence1);
            string sequenceString2 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Sequence2);
            string sequenceString3 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Sequence3);
            string sequenceString4 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Sequence4);
            string sequenceString5 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Sequence5);
            string sequenceString6 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Sequence6);
            string sequenceString7 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Sequence7);
            string sequenceString8 = null;
            string sequenceString9 = null;

            // Get all the input sequence object
            lstSequences = new List<ISequence>();
            ISequence seq1 = new Sequence(alphabet, sequenceString1);
            ISequence seq2 = new Sequence(alphabet, sequenceString2);
            ISequence seq3 = new Sequence(alphabet, sequenceString3);
            ISequence seq4 = new Sequence(alphabet, sequenceString4);
            ISequence seq5 = new Sequence(alphabet, sequenceString5);
            ISequence seq6 = new Sequence(alphabet, sequenceString6);
            ISequence seq7 = new Sequence(alphabet, sequenceString7);
            ISequence seq8 = null;
            ISequence seq9 = null;

            // Add all sequences to list.
            lstSequences.Add(seq1);
            lstSequences.Add(seq2);
            lstSequences.Add(seq3);
            lstSequences.Add(seq4);
            lstSequences.Add(seq5);
            lstSequences.Add(seq6);
            lstSequences.Add(seq7);

            similarityMatrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.AmbiguousDna);
            profileAligner = new NeedlemanWunschProfileAlignerParallel(similarityMatrix,
                                                                       ProfileScoreFunctionNames.InnerProduct,
                                                                       gapOpenPenalty, gapExtendPenalty,
                                                                       Environment.ProcessorCount);

            // Read all expected Sequences
            sequenceString1 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode1);
            sequenceString2 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode2);
            sequenceString3 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode3);
            sequenceString4 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode4);
            sequenceString5 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode5);
            sequenceString6 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode6);
            sequenceString7 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode7);
            sequenceString8 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode8);
            sequenceString9 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode9);

            seq1 = new Sequence(alphabet, sequenceString1);
            seq2 = new Sequence(alphabet, sequenceString2);
            seq3 = new Sequence(alphabet, sequenceString3);
            seq4 = new Sequence(alphabet, sequenceString4);
            seq5 = new Sequence(alphabet, sequenceString5);
            seq6 = new Sequence(alphabet, sequenceString6);
            seq7 = new Sequence(alphabet, sequenceString7);
            seq8 = new Sequence(alphabet, sequenceString8);
            seq9 = new Sequence(alphabet, sequenceString9);

            // Add all sequences to list.
            expectedSequences = new List<ISequence>();
            expectedSequences.Add(seq1);
            expectedSequences.Add(seq2);
            expectedSequences.Add(seq3);
            expectedSequences.Add(seq4);
            expectedSequences.Add(seq5);
            expectedSequences.Add(seq6);
            expectedSequences.Add(seq7);
            expectedSequences.Add(seq8);
            expectedSequences.Add(seq9);

            expectedScore = utilityObj.xmlUtil.GetTextValue(nodeName, expectedScoreNode);

            // Parallel Option will only get set if the PAMSAMMultipleSequenceAligner is getting called
            // To test separately distance matrix, binary tree etc.. 
            // Set the parallel option using below ctor.
            var msa = new PAMSAMMultipleSequenceAligner(lstSequences,
                                                        kmerLength, DistanceFunctionTypes.EuclideanDistance,
                                                        UpdateDistanceMethodsTypes.Average,
                                                        ProfileAlignerNames.NeedlemanWunschProfileAligner,
                                                        ProfileScoreFunctionNames.InnerProduct, similarityMatrix,
                                                        gapOpenPenalty, gapExtendPenalty, 2, 2);

            ApplicationLog.WriteLine(String.Format(null,
                                                   "Initialization of all variables successfully completed for xml node {0}",
                                                   nodeName));
        }

        private void InitializeStage1Variables(string nodeName)
        {
            // Read all the input sequences from xml config file
            IAlphabet alphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                     Constants.AlphabetNameNode));
            // Read all expected Sequences for stage1 
            string sequenceString1 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Stage1ExpectedSequenceNode1);
            string sequenceString2 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Stage1ExpectedSequenceNode2);
            string sequenceString3 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Stage1ExpectedSequenceNode3);
            string sequenceString4 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Stage1ExpectedSequenceNode4);
            string sequenceString5 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Stage1ExpectedSequenceNode5);
            string sequenceString6 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Stage1ExpectedSequenceNode6);
            string sequenceString7 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Stage1ExpectedSequenceNode7);

            // Get all expected stage1 sequences object
            ISequence seq1 = new Sequence(alphabet, sequenceString1);
            ISequence seq2 = new Sequence(alphabet, sequenceString2);
            ISequence seq3 = new Sequence(alphabet, sequenceString3);
            ISequence seq4 = new Sequence(alphabet, sequenceString4);
            ISequence seq5 = new Sequence(alphabet, sequenceString5);
            ISequence seq6 = new Sequence(alphabet, sequenceString6);
            ISequence seq7 = new Sequence(alphabet, sequenceString7);

            stage1ExpectedSequences = new List<ISequence>();
            stage1ExpectedSequences.Add(seq1);
            stage1ExpectedSequences.Add(seq2);
            stage1ExpectedSequences.Add(seq3);
            stage1ExpectedSequences.Add(seq4);
            stage1ExpectedSequences.Add(seq5);
            stage1ExpectedSequences.Add(seq6);
            stage1ExpectedSequences.Add(seq7);

            stage1ExpectedScore = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Stage1ExpectedScoreNode);

            ApplicationLog.WriteLine(String.Format(null,
                                                   "Initialization of stage1 variables successfully completed for xml node {0}",
                                                   nodeName));
        }

        private void InitializeStage2Variables(string nodeName)
        {
            // Read all the input sequences from xml config file
            IAlphabet alphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                     Constants.AlphabetNameNode));
            // Read all expected Sequences for stage1 
            string sequenceString1 = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                     Constants.Stage2ExpectedSequenceNode1);
            string sequenceString2 = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                     Constants.Stage2ExpectedSequenceNode2);
            string sequenceString3 = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                     Constants.Stage2ExpectedSequenceNode3);
            string sequenceString4 = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                     Constants.Stage2ExpectedSequenceNode4);
            string sequenceString5 = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                     Constants.Stage2ExpectedSequenceNode5);
            string sequenceString6 = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                     Constants.Stage2ExpectedSequenceNode6);
            string sequenceString7 = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                     Constants.Stage2ExpectedSequenceNode7);

            // Get all expected stage1 sequences object
            ISequence seq1 = new Sequence(alphabet, sequenceString1);
            ISequence seq2 = new Sequence(alphabet, sequenceString2);
            ISequence seq3 = new Sequence(alphabet, sequenceString3);
            ISequence seq4 = new Sequence(alphabet, sequenceString4);
            ISequence seq5 = new Sequence(alphabet, sequenceString5);
            ISequence seq6 = new Sequence(alphabet, sequenceString6);
            ISequence seq7 = new Sequence(alphabet, sequenceString7);

            stage2ExpectedSequences = new List<ISequence>();
            stage2ExpectedSequences.Add(seq1);
            stage2ExpectedSequences.Add(seq2);
            stage2ExpectedSequences.Add(seq3);
            stage2ExpectedSequences.Add(seq4);
            stage2ExpectedSequences.Add(seq5);
            stage2ExpectedSequences.Add(seq6);
            stage2ExpectedSequences.Add(seq7);

            stage2ExpectedScore = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                  Constants.Stage2ExpectedScoreNode);

            ApplicationLog.WriteLine(String.Format(null,
                                                   "PamsamBvtTest:: Initialization of stage2 variables successfully completed for xml node {0}",
                                                   nodeName));
        }

        /// <summary>
        ///     Validate Muscle multiple sequence alignment with different profiler and hierarchical clustering method name.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="expectedScoreNode">Expected score node</param>
        /// <param name="hierarchicalClusteringMethodName">hierarchical clustering method name</param>
        /// <param name="profileName">SW/NW profiler</param>
        /// <param name="isWeightedProduct">True if it of the WeightedProduct type else false.</param>
        private void ValidatePamsamAlignWithUpdateDistanceMethodTypes(string nodeName,
                                                                      string expectedScoreNode,
                                                                      UpdateDistanceMethodsTypes
                                                                          hierarchicalClusteringMethodName,
                                                                      ProfileAlignerNames profileName,
                                                                      bool isWeightedProduct)
        {
            ValidatePamsamAlign(nodeName, expectedScoreNode, hierarchicalClusteringMethodName,
                                DistanceFunctionTypes.EuclideanDistance, profileName,
                                ProfileScoreFunctionNames.InnerProduct,
                                isWeightedProduct);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamBvtTest:: Pamsam alignment validation completed successfully with different hierarchical clustering method name {0}",
                                                   hierarchicalClusteringMethodName.ToString()));
        }

        /// <summary>
        ///     Validate Muscle multiple sequence alignment with different
        ///     profiler and distance matrix method name.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="expectedScoreNode">Expected score node</param>
        /// <param name="distanceFunctionName">distance method name.</param>
        /// <param name="profileName">SW/NW profiler</param>
        /// <param name="isWeightedProduct">True if it of the WeightedProduct type else false.</param>
        private void ValidatePamsamAlignWithDistanceFunctionaTypes(string nodeName,
                                                                   string expectedScoreNode,
                                                                   DistanceFunctionTypes distanceFunctionName,
                                                                   ProfileAlignerNames profileName,
                                                                   bool isWeightedProduct)
        {
            ValidatePamsamAlign(nodeName, expectedScoreNode,
                                UpdateDistanceMethodsTypes.Average, distanceFunctionName,
                                profileName, ProfileScoreFunctionNames.InnerProduct, isWeightedProduct);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamBvtTest:: Pamsam alignment validation completed successfully with different kmer distance method name {0}",
                                                   distanceFunctionName.ToString()));
        }

        /// <summary>
        ///     Validate Muscle multiple sequence alignment with different profiler and
        ///     profile score function name.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="expectedScoreNode">Expected score node</param>
        /// <param name="profileName">SW/NW profiler</param>
        /// <param name="profileScoreFunctionName">Profile score function name</param>
        /// <param name="isWeightedProduct">True if it of the WeightedProduct type else false.</param>
        private void ValidatePamsamAlignWithProfileScoreFunctionName(string nodeName,
                                                                     string expectedScoreNode,
                                                                     ProfileAlignerNames profileName,
                                                                     ProfileScoreFunctionNames profileScoreFunctionName,
                                                                     bool isWeightedProduct)
        {
            ValidatePamsamAlign(nodeName, expectedScoreNode,
                                UpdateDistanceMethodsTypes.Average,
                                DistanceFunctionTypes.EuclideanDistance, profileName,
                                profileScoreFunctionName, isWeightedProduct);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamBvtTest:: Pamsam alignment validation completed successfully with different profile score function name {0}",
                                                   profileScoreFunctionName.ToString()));
        }

        /// <summary>
        ///     Validate Muscle multiple sequence alignment with default values.
        /// </summary>
        /// <param name="nodeName">Node Name</param>
        /// <param name="expectedScoreNode">Expected score node</param>
        /// <param name="profileName">Profile Name</param>
        /// <param name="isWeightedProduct">True if it of the WeightedProduct type else false.</param>
        private void ValidatePamsamAlign(string nodeName,
                                         string expectedScoreNode, ProfileAlignerNames profileName,
                                         bool isWeightedProduct)
        {
            ValidatePamsamAlign(nodeName, expectedScoreNode,
                                UpdateDistanceMethodsTypes.Average, DistanceFunctionTypes.EuclideanDistance,
                                profileName, ProfileScoreFunctionNames.InnerProduct, isWeightedProduct);

            ApplicationLog.WriteLine(
                "PamsamBvtTest:: Pamsam alignment validation completed successfully with all default params");
        }

        /// <summary>
        ///     Validate Muscle multiple sequence alignment.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="expectedScoreNode">Expected score node</param>
        /// <param name="hierarchicalClusteringMethodName">hierarchical clustering method name</param>
        /// <param name="distanceFunctionName">kmerdistancematrix method name.</param>
        /// <param name="profileAlignerName">SW/NW profiler</param>
        /// <param name="profileScoreName">Profile score function name.</param>
        /// <param name="isWeightedProduct">True if it of the WeightedProduct type else false.</param>
        private void ValidatePamsamAlign(string nodeName,
                                         string expectedScoreNode,
                                         UpdateDistanceMethodsTypes hierarchicalClusteringMethodName,
                                         DistanceFunctionTypes distanceFunctionName,
                                         ProfileAlignerNames profileAlignerName,
                                         ProfileScoreFunctionNames profileScoreName,
                                         bool isWeightedProduct)
        {
            Initialize(nodeName, expectedScoreNode);

            // MSA aligned sequences.
            var msa = new PAMSAMMultipleSequenceAligner(lstSequences,
                                                        kmerLength, distanceFunctionName,
                                                        hierarchicalClusteringMethodName,
                                                        profileAlignerName, profileScoreName,
                                                        similarityMatrix, gapOpenPenalty, gapExtendPenalty, 2, 2);

            int index = 0;
            foreach (ISequence seq in msa.AlignedSequences)
            {
                if (isWeightedProduct)
                {
                    Assert.AreEqual(new string(seq.Select(a => (char) a).ToArray()),
                                    new string(expectedSequences[index].Select(a => (char) a).ToArray()));
                    index++;
                }
            }

            Assert.IsTrue(expectedScore.Contains(msa.AlignmentScore.ToString((IFormatProvider) null)));
        }

        /// <summary>
        ///     Validate Stage 1 aligned sequences and score of Muscle multiple sequence alignment.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="expectedScoreNode">Expected score node</param>
        /// <param name="hierarchicalClusteringMethodName">hierarchical clustering method name</param>
        /// <param name="distanceFunctionName">kmerdistancematrix method name.</param>
        /// <param name="profileAlignerName">SW/NW profiler</param>
        /// <param name="profileScoreName">Profile score function name.</param>
        private void ValidatePamsamAlignStage1(string nodeName,
                                               string expectedScoreNode,
                                               UpdateDistanceMethodsTypes hierarchicalClusteringMethodName,
                                               DistanceFunctionTypes distanceFunctionName,
                                               ProfileAlignerNames profileAlignerName,
                                               ProfileScoreFunctionNames profileScoreName)
        {
            Initialize(nodeName, expectedScoreNode);
            InitializeStage1Variables(nodeName);

            // MSA aligned sequences.
            var msa =
                new PAMSAMMultipleSequenceAligner(lstSequences, kmerLength,
                                                  distanceFunctionName, hierarchicalClusteringMethodName,
                                                  profileAlignerName, profileScoreName, similarityMatrix, gapOpenPenalty,
                                                  gapExtendPenalty, 2, 2);

            // Validate the aligned Sequence and score of stage1
            Assert.AreEqual(stage1ExpectedSequences.Count, msa.AlignedSequences.Count);
            int index = 0;
            foreach (ISequence seq in msa.AlignedSequencesA)
            {
                Assert.AreEqual(new string(stage1ExpectedSequences[index].Select(a => (char) a).ToArray()),
                                new string(seq.Select(a => (char) a).ToArray()));
                index++;
            }
            Assert.AreEqual(stage1ExpectedScore, msa.AlignmentScoreA.ToString((IFormatProvider) null));

            ApplicationLog.WriteLine(String.Format(null,
                                                   "PamsamBvtTest:: Pamsam stage1 alignment completed successfully with all default params"));
        }

        /// <summary>
        ///     Validate Stage 2 aligned sequences and score of Muscle multiple sequence alignment.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="expectedScoreNode">Expected score node</param>
        /// <param name="hierarchicalClusteringMethodName">hierarchical clustering method name</param>
        /// <param name="distanceFunctionName">kmerdistancematrix method name.</param>
        /// <param name="profileAlignerName">SW/NW profiler</param>
        /// <param name="profileScoreName">Profile score function name.</param>
        private void ValidatePamsamAlignStage2(string nodeName,
                                               string expectedScoreNode,
                                               UpdateDistanceMethodsTypes hierarchicalClusteringMethodName,
                                               DistanceFunctionTypes distanceFunctionName,
                                               ProfileAlignerNames profileAlignerName,
                                               ProfileScoreFunctionNames profileScoreName)
        {
            Initialize(nodeName, expectedScoreNode);
            InitializeStage2Variables(nodeName);

            // MSA aligned sequences.
            var msa =
                new PAMSAMMultipleSequenceAligner(lstSequences,
                                                  kmerLength, distanceFunctionName, hierarchicalClusteringMethodName,
                                                  profileAlignerName, profileScoreName, similarityMatrix, gapOpenPenalty,
                                                  gapExtendPenalty, 2, 2);

            // Validate the aligned Sequence and score of stage2
            if (null != msa.AlignedSequencesB)
            {
                Assert.AreEqual(stage2ExpectedSequences.Count, msa.AlignedSequencesB.Count);
                int index = 0;
                foreach (ISequence seq in msa.AlignedSequencesB)
                {
                    Assert.AreEqual(new string(stage2ExpectedSequences[index].Select(a => (char) a).ToArray()),
                                    new string(seq.Select(a => (char) a).ToArray()));
                    index++;
                }
                Assert.AreEqual(stage2ExpectedScore, msa.AlignmentScoreB.ToString((IFormatProvider) null));
            }

            ApplicationLog.WriteLine(String.Format(null,
                                                   "PamsamBvtTest:: Pamsam stage2 alignment completed successfully with all default params"));
        }

        /// <summary>
        ///     Validate Stage 3 aligned sequences and score of Muscle multiple sequence alignment.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="expectedScoreNode">Expected score node</param>
        /// <param name="hierarchicalClusteringMethodName">hierarchical clustering method name</param>
        /// <param name="distanceFunctionName">kmerdistancematrix method name.</param>
        /// <param name="profileAlignerName">SW/NW profiler</param>
        /// <param name="profileScoreName">Profile score function name.</param>
        private void ValidatePamsamAlignStage3(string nodeName,
                                               string expectedScoreNode,
                                               UpdateDistanceMethodsTypes hierarchicalClusteringMethodName,
                                               DistanceFunctionTypes distanceFunctionName,
                                               ProfileAlignerNames profileAlignerName,
                                               ProfileScoreFunctionNames profileScoreName)
        {
            Initialize(nodeName, expectedScoreNode);

            // MSA aligned sequences.
            var msa =
                new PAMSAMMultipleSequenceAligner(lstSequences,
                                                  kmerLength, distanceFunctionName, hierarchicalClusteringMethodName,
                                                  profileAlignerName, profileScoreName, similarityMatrix, gapOpenPenalty,
                                                  gapExtendPenalty, 2, 2);

            string expectedSeqString = expectedSequences.Aggregate(string.Empty,
                                                                   (current, seq) =>
                                                                   current +
                                                                   (new string(seq.Select(a => (char) a).ToArray()) +
                                                                    ","));

            foreach (ISequence seq in msa.AlignedSequencesC)
            {
                Assert.IsTrue(expectedSeqString.Contains(new string(seq.Select(a => (char) a).ToArray())));
            }

            Assert.IsTrue(expectedScore.Contains(msa.AlignmentScoreC.ToString((IFormatProvider) null)));
            ApplicationLog.WriteLine(String.Format(null,
                                                   "PamsamBvtTest:: Pamsam stage3 alignment completed successfully with all default params"));
        }

        /// <summary>
        ///     Validate DistanceMatrix at stage1 using different DistanceFunction names.
        /// </summary>
        /// <param name="nodeName">Xml node name</param>
        /// <param name="kmrlength">Kmer length</param>
        /// <param name="distanceFunctionName">Distance method name</param>
        private void ValidateKmerDistanceMatrixStage1(string nodeName,
                                                      int kmrlength,
                                                      DistanceFunctionTypes distanceFunctionName)
        {
            Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
            IDistanceMatrix matrix = GetKmerDistanceMatrix(kmrlength, distanceFunctionName);
            ValidateDistanceMatrix(nodeName, matrix);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamBvtTest:: kmer distance matrix generation and validation completed success with different distance method name {0}",
                                                   distanceFunctionName.ToString()));
        }

        /// <summary>
        ///     Validate Distance Matrix with default distancefunctionname
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="kmrlength">kmr length</param>
        private void ValidateKmerDistanceMatrixStage1(string nodeName,
                                                      int kmrlength)
        {
            Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
            IDistanceMatrix matrix = GetKmerDistanceMatrix(kmrlength);
            ValidateDistanceMatrix(nodeName, matrix);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamBvtTest:: kmer distance matrix generation and validation completed success with default params"));
        }

        /// <summary>
        ///     Validate Distance Matrix
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="matrix">distance matrix</param>
        private void ValidateDistanceMatrix(string nodeName, IDistanceMatrix matrix)
        {
            // Read expected values from config
            string expectedDimension = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Dimension);
            string expectedMinimumValue = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MinimumValue);
            string expectedNearestDistances = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                              Constants.NearestDistances);

            // Validate values in distance matrix
            Assert.AreEqual(expectedDimension, matrix.Dimension.ToString((IFormatProvider) null));
            Assert.IsTrue(expectedMinimumValue.Contains(matrix.MinimumValue.ToString((IFormatProvider) null)));

            for (int idist = 0; idist < matrix.NearestDistances.Length; idist++)
            {
                Assert.IsTrue(expectedNearestDistances.Contains(
                    matrix.NearestDistances[idist].ToString((IFormatProvider) null)));
            }
        }

        /// <summary>
        ///     Validate Hierarchical Clustering for stage1
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="kmrlength">kmer length for distance matrix</param>
        /// <param name="hierarchicalMethoName">hierarchical method name.</param>
        private void ValidateHierarchicalClusteringStage1(string nodeName,
                                                          int kmrlength,
                                                          UpdateDistanceMethodsTypes hierarchicalMethoName)
        {
            Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
            IDistanceMatrix matrix = GetKmerDistanceMatrix(kmrlength);
            IHierarchicalClustering hierarcicalClustering =
                GetHierarchicalClustering(matrix, hierarchicalMethoName);

            ValidateHierarchicalClustering(nodeName, hierarcicalClustering.Nodes,
                                           hierarcicalClustering.Edges);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamBvtTest:: hierarchical clustering stage1 nodes and edges generation and validation completed success with different hierarchical clustering method name {0}",
                                                   hierarchicalMethoName.ToString()));
        }

        /// <summary>
        ///     Validate Hierarchical Clustering for stage2 using kimura distance matrix and other default params.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        private void ValidateHierarchicalClusteringStage2(string nodeName)
        {
            Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
            List<ISequence> stage1AlignedSequences = GetStage1AlignedSequence();
            IDistanceMatrix matrix = GetKimuraDistanceMatrix(stage1AlignedSequences);
            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix);

            ValidateHierarchicalClustering(nodeName, hierarcicalClustering.Nodes,
                                           hierarcicalClustering.Edges);

            ApplicationLog.WriteLine(
                @"PamsamBvtTest:: hierarchical clustering stage2 nodes and edges generation and validation completed success with default params");
        }

        /// <summary>
        ///     Validate Hierarchical Clustering for stage1 using kmer distance matrix
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="kmrlength">kmer length to generate distance matrix</param>
        private void ValidateHierarchicalClusteringStage1(string nodeName, int kmrlength)
        {
            Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
            IDistanceMatrix matrix = GetKmerDistanceMatrix(kmrlength);
            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix);
            ValidateHierarchicalClustering(nodeName, hierarcicalClustering.Nodes,
                                           hierarcicalClustering.Edges);

            ApplicationLog.WriteLine(
                @"PamsamBvtTest:: hierarchical clustering stage1 nodes and edges generation and validation completed success with default params");
        }

        /// <summary>
        ///     Validate Hierarchical Clustering for stage2 using kimura distance matrix and hierarchical method name
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="hierarchicalMethodName"></param>
        private void ValidateHierarchicalClusteringStage2(string nodeName,
                                                          UpdateDistanceMethodsTypes hierarchicalMethodName)
        {
            Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
            List<ISequence> stage1AlignedSequences = GetStage1AlignedSequence();
            IDistanceMatrix matrix = GetKimuraDistanceMatrix(stage1AlignedSequences);

            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix,
                                                                                      hierarchicalMethodName);

            ValidateHierarchicalClustering(nodeName, hierarcicalClustering.Nodes,
                                           hierarcicalClustering.Edges);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamBvtTest:: herarchical clustering stage2 nodes and edges generation and 
          validation completed success with different 
          hierarchical clustering method name {0}",
                                                   hierarchicalMethodName.ToString()));
        }

        /// <summary>
        ///     Validate the nodes and edges of hierarchical clustering object.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="nodes">binary tree nodes</param>
        /// <param name="edges">binary tree edges</param>
        private void ValidateHierarchicalClustering(string nodeName,
                                                    List<BinaryGuideTreeNode> nodes, List<BinaryGuideTreeEdge> edges)
        {
            // Validate the nodes and edges.
            string expectedEdgeCount = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                       Constants.EdgesCount);
            string expectedNodesLeftChild = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                            Constants.NodesLeftChild);
            string expectedNodesRightChild = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                             Constants.NodesRightChild);
            string expectednode = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                  Constants.Nodes);

            foreach (BinaryGuideTreeNode node in nodes)
            {
                Assert.IsTrue(expectednode.Contains(node.ID.ToString((IFormatProvider) null)));
                if (node.LeftChildren != null)
                {
                    Assert.IsTrue(expectedNodesLeftChild.Contains(node.LeftChildren.ID.ToString((IFormatProvider) null)));
                }
                if (node.RightChildren != null)
                {
                    Assert.IsTrue(
                        expectedNodesRightChild.Contains(node.RightChildren.ID.ToString((IFormatProvider) null)));
                }
            }
            Assert.AreEqual(expectedEdgeCount, edges.Count.ToString((IFormatProvider) null));
        }

        /// <summary>
        ///     Validate the binary tree leaves, root using unaligned sequences.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="kmrLength">kmer length to generate distance matrix</param>
        private void ValidateBinaryTreeStage1(string nodeName, int kmrLength)
        {
            Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
            IDistanceMatrix matrix = GetKmerDistanceMatrix(kmrLength);

            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix);

            BinaryGuideTree binaryTree = GetBinaryTree(hierarcicalClustering);

            ValidateBinaryTree(binaryTree, nodeName);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamBvtTest:: Binary Tree stage1 root and leaves generation and 
          validation completed success with default params"));
        }

        /// <summary>
        ///     Validate the binary tree leaves, root using stage1 aligned sequences.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        private void ValidateBinaryTreeStage2(string nodeName)
        {
            Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
            List<ISequence> stage1AlignedSequences = GetStage1AlignedSequence();

            IDistanceMatrix matrix = GetKimuraDistanceMatrix(stage1AlignedSequences);

            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix);

            BinaryGuideTree binaryTree = GetBinaryTree(hierarcicalClustering);

            ValidateBinaryTree(binaryTree, nodeName);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamBvtTest:: Binary tree stage2 root and leaves generation and 
          validation completed success with default params"));
        }

        /// <summary>
        ///     Validate SeparateSequencesByCutTree() method of Binary tree by cutting the tree at an edge.
        /// </summary>
        /// <param name="edgeIndex">edge index to cut the tree</param>
        /// <param name="nodeName">xml node name</param>
        /// <param name="kmrLength">kmerlength to get distance matrix.</param>
        private void ValidateBinaryTreeSeparateSequencesByCutTree(int edgeIndex, string nodeName,
                                                                  int kmrLength)
        {
            Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
            IDistanceMatrix matrix = GetKmerDistanceMatrix(kmrLength);
            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix);

            BinaryGuideTree binaryTree = GetBinaryTree(hierarcicalClustering);

            List<int>[] sequences = binaryTree.SeparateSequencesByCuttingTree(edgeIndex);
            string seqIndicesString = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                      Constants.SequenceIndicesWithCutTree);

            string[] seqIndices = seqIndicesString.Split(',');
            int counter = 0;
            for (int index = 0; index < sequences.Length; index++)
            {
                for (int seqIndex = 0; seqIndex < sequences[index].Count; seqIndex++)
                {
                    Assert.AreEqual(sequences[index][seqIndex].ToString((IFormatProvider) null), seqIndices[counter]);
                    counter++;
                }
            }

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamBvtTest::Validate binary tree by cutting tree at an edge index {0}
          and validation of nodes and edges completed successfully",
                                                   edgeIndex));
        }

        /// <summary>
        ///     Validate the leaves and root of binary tree
        /// </summary>
        /// <param name="binaryTree">binary tree object.</param>
        /// <param name="nodeName">xml node name.</param>
        private void ValidateBinaryTree(BinaryGuideTree binaryTree, string nodeName)
        {
            string rootId = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.RootId);
            string leavesCount = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.LeavesCount);
            string expectedNodesLeftChild = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                            Constants.NodesLeftChild);
            string expectedNodesRightChild = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                             Constants.NodesRightChild);
            string expectenode = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Nodes);
            string[] expectedNodes = expectenode.Split(',');

            Assert.IsTrue(rootId.Contains(binaryTree.Root.ID.ToString((IFormatProvider) null)));
            Assert.IsTrue(leavesCount.Contains(binaryTree.NumberOfLeaves.ToString((IFormatProvider) null)));
            int index = 0;
            foreach (BinaryGuideTreeNode node in binaryTree.Nodes)
            {
                Assert.AreEqual(expectedNodes[index], node.ID.ToString((IFormatProvider) null));
                if (node.LeftChildren != null)
                {
                    Assert.IsTrue(expectedNodesLeftChild.Contains(node.LeftChildren.ID.ToString((IFormatProvider) null)));
                }
                if (node.RightChildren != null)
                {
                    Assert.IsTrue(
                        expectedNodesRightChild.Contains(node.RightChildren.ID.ToString((IFormatProvider) null)));
                }
                index++;
            }
        }

        /// <summary>
        ///     Validate Progressive Alignment of Stage 1
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        private void ValidateProgressiveAlignmentStage1(string nodeName)
        {
            Initialize(nodeName, Constants.ExpectedScoreNode);
            InitializeStage1Variables(nodeName);

            IDistanceMatrix matrix = GetKmerDistanceMatrix(kmerLength);
            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix);
            BinaryGuideTree binaryTree = GetBinaryTree(hierarcicalClustering);

            List<ISequence> alignedSequences = GetProgressiveAlignerAlignedSequences(lstSequences,
                                                                                     binaryTree);

            // Validate the aligned Sequence of stage1
            Assert.AreEqual(stage1ExpectedSequences.Count, alignedSequences.Count);
            int index = 0;
            foreach (ISequence seq in alignedSequences)
            {
                Assert.AreEqual(new string(stage1ExpectedSequences[index].Select(a => (char) a).ToArray()),
                                new string(seq.Select(a => (char) a).ToArray()));
                index++;
            }

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamBvtTest:: Validation and generation of stage1 aligned sequences
          using progressivealignment completed successfully"));
        }

        /// <summary>
        ///     Validate the Profile Aligner GenerateEString() method using profiles of sub trees.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="edgeIndex">Edge index to cut tree.</param>
        private void ValidateProfileAlignerGenerateEString(string nodeName, int edgeIndex)
        {
            Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
            InitializeStage2Variables(Constants.MuscleDnaSequenceNode);

            // Get Stage2 Binary Tree
            List<ISequence> stage1AlignedSequences = GetStage1AlignedSequence();
            IDistanceMatrix matrix = GetKimuraDistanceMatrix(stage1AlignedSequences);
            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix);
            BinaryGuideTree binaryTree = GetBinaryTree(hierarcicalClustering);

            // Get profiles
            GetProfiles(edgeIndex, binaryTree);

            // Get id's of edges and root using two profiles
            List<int> eStringSubtreeEdge = profileAligner.GenerateEString(profileAligner.AlignedA);
            List<int> eStringSubtreeRoot = profileAligner.GenerateEString(profileAligner.AlignedB);

            string expectedTreeEdges = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SubTreeEdges);
            string expectedTreeRoot = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SubTreeRoots);

            string[] expectededges = expectedTreeEdges.Split(',');
            for (int index = 0; index < eStringSubtreeEdge.Count; index++)
            {
                Assert.AreEqual(eStringSubtreeEdge[index].ToString((IFormatProvider) null), expectededges[index]);
            }

            Assert.AreEqual(eStringSubtreeRoot[0].ToString((IFormatProvider) null), expectedTreeRoot);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamBvtTest:: Validation and generation of subtrees roots and edges
          using profile aligner GenerateEString() completed successfully"));
        }

        /// <summary>
        ///     Validate the Profile Aligner GenerateSequenceString() method using profiles of sub trees.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="edgeIndex">Edge index to cut tree.</param>
        private void ValidateProfileAlignerGenerateSequenceString(string nodeName, int edgeIndex)
        {
            Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
            InitializeStage2Variables(Constants.MuscleDnaSequenceNode);

            // Get Stage2 Binary Tree
            List<ISequence> stage1AlignedSequences = GetStage1AlignedSequence();
            IDistanceMatrix matrix = GetKimuraDistanceMatrix(stage1AlignedSequences);
            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix);
            BinaryGuideTree binaryTree = GetBinaryTree(hierarcicalClustering);

            GetProfiles(edgeIndex, binaryTree);

            // Get id's of edges and root using two profiles
            List<int> eStringSubtreeEdge = profileAligner.GenerateEString(profileAligner.AlignedA);

            string expectedSequence = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                      Constants.GenerateSequenceString);

            ISequence sequence = profileAligner.GenerateSequenceFromEString(eStringSubtreeEdge,
                                                                            lstSequences[0]);
            Assert.AreEqual(new string(sequence.Select(a => (char) a).ToArray()), expectedSequence);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamBvtTest:: Validation and generation of subtrees sequences
          using profile aligner GenerateSequenceFromEString() completed successfully"));
        }

        /// <summary>
        ///     Validate the kimura distance matrix using stage 1 aligned sequences.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        private void ValidateKimuraDistanceMatrix(string nodeName)
        {
            Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
            List<ISequence> stage1AlignedSequences = GetStage1AlignedSequence();

            IDistanceMatrix matrix = GetKimuraDistanceMatrix(stage1AlignedSequences);
            ValidateDistanceMatrix(nodeName, matrix);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamBvtTest:: kimura distance matrix generation and validation completed success with default params"));
        }

        /// <summary>
        ///     Creates binarytree using stage1 sequences and
        ///     cut the binary tree at an random edge to get two profiles.
        ///     Create NeedlemanWunschProfileAlignerSerial\Parallel instance
        ///     according to degree of parallelism
        ///     and execute AlignSimple\Align() method using two profiles.
        ///     Validate the IProfileAlignment properties.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="degreeOfParallelism">if 1 it is serial Profiler else parallel profiler</param>
        /// <param name="edgeIndex">edge index to cut the tree</param>
        /// <param name="overloadType">Execute Align()\AlignSimple()</param>
        private void ValidateProfileAlignerAlign(string nodeName,
                                                 int degreeOfParallelism, int edgeIndex, AlignType overloadType)
        {
            Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
            InitializeStage2Variables(Constants.MuscleDnaSequenceNode);

            // Get Stage2 Binary Tree
            List<ISequence> stage1AlignedSequences = GetStage1AlignedSequence();
            IDistanceMatrix matrix = GetKimuraDistanceMatrix(stage1AlignedSequences);
            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix);
            BinaryGuideTree binaryTree = GetBinaryTree(hierarcicalClustering);

            // Cut Tree at an edge and get sequences.
            List<int>[] leafNodeIndices = binaryTree.SeparateSequencesByCuttingTree(edgeIndex);

            // Extract profiles 
            List<int>[] removedPositions = null;
            IProfileAlignment[] separatedProfileAlignments = ProfileAlignment.ProfileExtraction(
                stage2ExpectedSequences, leafNodeIndices[0], leafNodeIndices[1], out removedPositions);

            // Align it
            IProfileAlignment alignedProfile = null;
            if (1 == degreeOfParallelism)
            {
                var nwprofileAligner =
                    new NeedlemanWunschProfileAlignerSerial(similarityMatrix,
                                                            ProfileScoreFunctionNames.InnerProductFast,
                                                            gapOpenPenalty, gapExtendPenalty, 2);
                switch (overloadType)
                {
                    case AlignType.AlignSimpleAllParams:
                        alignedProfile = nwprofileAligner.AlignSimple(similarityMatrix,
                                                                      gapOpenPenalty, separatedProfileAlignments[0],
                                                                      separatedProfileAlignments[1]);
                        break;
                    case AlignType.AlignSimpleOnlyProfiles:
                        alignedProfile = nwprofileAligner.AlignSimple(separatedProfileAlignments[0],
                                                                      separatedProfileAlignments[1]);
                        break;
                    case AlignType.AlignAllParams:
                        alignedProfile = nwprofileAligner.Align(similarityMatrix, gapOpenPenalty,
                                                                gapExtendPenalty, separatedProfileAlignments[0],
                                                                separatedProfileAlignments[1]);
                        break;
                }
            }
            else
            {
                if (Environment.ProcessorCount >= degreeOfParallelism)
                {
                    var nwprofileAlignerParallel =
                        new NeedlemanWunschProfileAlignerParallel(
                            similarityMatrix,
                            ProfileScoreFunctionNames.InnerProductFast,
                            gapOpenPenalty,
                            gapExtendPenalty, 2);

                    alignedProfile = nwprofileAlignerParallel.AlignSimple(
                        separatedProfileAlignments[0],
                        separatedProfileAlignments[1]);

                    switch (overloadType)
                    {
                        case AlignType.AlignSimpleAllParams:
                            alignedProfile = nwprofileAlignerParallel.AlignSimple(similarityMatrix,
                                                                                  gapOpenPenalty,
                                                                                  separatedProfileAlignments[0],
                                                                                  separatedProfileAlignments[1]);
                            break;
                        case AlignType.AlignSimpleOnlyProfiles:
                            alignedProfile = nwprofileAlignerParallel.AlignSimple(
                                separatedProfileAlignments[0],
                                separatedProfileAlignments[1]);
                            break;
                        case AlignType.AlignAllParams:
                            alignedProfile = nwprofileAlignerParallel.Align(
                                similarityMatrix, gapOpenPenalty,
                                gapExtendPenalty, separatedProfileAlignments[0],
                                separatedProfileAlignments[1]);
                            break;
                    }
                }
                else
                {
                    ApplicationLog.WriteLine(String.Format(null,
                                                           @"PamsamBvtTest: NeedlemanWunschProfileAlignerParallel could not be instantiated
                          as number of processor is {0} and degree of parallelism {1}",
                                                           Environment.ProcessorCount.ToString((IFormatProvider) null),
                                                           degreeOfParallelism));
                }
            }

            if (null != alignedProfile)
            {
                // Validate profile alignement 
                string expectedRowSize = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.RowSize);
                string expectedColSize = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ColumnSize);
                Assert.AreEqual(expectedColSize,
                                alignedProfile.ProfilesMatrix.ColumnSize.ToString((IFormatProvider) null));
                Assert.AreEqual(expectedRowSize, alignedProfile.ProfilesMatrix.RowSize.ToString((IFormatProvider) null));

                ApplicationLog.WriteLine(String.Format(null,
                                                       @"PamsamBvtTest: {0} {1} method validation completed successfully with
                        number of processor is {2} and degree of parallelism {3}",
                                                       profileAligner,
                                                       overloadType,
                                                       Environment.ProcessorCount.ToString((IFormatProvider) null),
                                                       degreeOfParallelism));
            }
            else
            {
                Assert.Fail("Profile Aligner is not instantiated");
            }
        }

        /// <summary>
        ///     Creates binarytree using stage1 sequences and
        ///     cut the binary tree at an random edge to get two profiles.
        ///     Create NeedlemanWunschProfileAlignerSerial\Parallel instance
        ///     according to degree of parallelism
        ///     and using profile function score . Execute Align() method.
        ///     Validates the IProfileAlignment properties.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="degreeOfParallelism">if 1 it is serial Profiler else parallel profiler</param>
        /// <param name="edgeIndex">edge index to cut the tree</param>
        /// <param name="profileFunction">profile function score name</param>
        private void ValidateProfileAlignerAlignWithProfileFunctionScore(
            string nodeName, int degreeOfParallelism, ProfileScoreFunctionNames profileFunction,
            int edgeIndex)
        {
            Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
            InitializeStage2Variables(Constants.MuscleDnaSequenceNode);

            // Get Stage2 Binary Tree
            List<ISequence> stage1AlignedSequences = GetStage1AlignedSequence();
            IDistanceMatrix matrix = GetKimuraDistanceMatrix(stage1AlignedSequences);
            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix);
            BinaryGuideTree binaryTree = GetBinaryTree(hierarcicalClustering);

            // Cut Tree at an edge and get sequences.
            List<int>[] leafNodeIndices = binaryTree.SeparateSequencesByCuttingTree(edgeIndex);

            // Extract profiles 
            List<int>[] removedPositions = null;
            IProfileAlignment[] separatedProfileAlignments = ProfileAlignment.ProfileExtraction(
                stage2ExpectedSequences, leafNodeIndices[0], leafNodeIndices[1], out removedPositions);

            IProfileAligner aligner = null;
            if (1 == degreeOfParallelism)
            {
                aligner = new NeedlemanWunschProfileAlignerSerial(similarityMatrix,
                                                                  profileFunction, gapOpenPenalty, gapExtendPenalty, 2);
            }
            else
            {
                if (Environment.ProcessorCount >= degreeOfParallelism)
                {
                    aligner = new NeedlemanWunschProfileAlignerParallel(similarityMatrix,
                                                                        profileFunction, gapOpenPenalty,
                                                                        gapExtendPenalty, 2);
                }
                else
                {
                    ApplicationLog.WriteLine(String.Format(null,
                                                           @"PamsamBvtTest: NeedlemanWunschProfileAlignerParallel could not be instantiated
                          as number of processor is {0} and degree of parallelism {1}",
                                                           Environment.ProcessorCount.ToString((IFormatProvider) null),
                                                           degreeOfParallelism));
                }
            }

            if (null != aligner)
            {
                IProfileAlignment profileAlignment = aligner.Align(separatedProfileAlignments[0],
                                                                   separatedProfileAlignments[0]);

                // Validate profile alignement 
                string expectedRowSize = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.RowSize);
                string expectedColSize = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ColumnSize);
                Assert.IsTrue(
                    expectedColSize.Contains(profileAlignment.ProfilesMatrix.ColumnSize.ToString((IFormatProvider) null)));
                Assert.IsTrue(
                    expectedRowSize.Contains(profileAlignment.ProfilesMatrix.RowSize.ToString((IFormatProvider) null)));
                ApplicationLog.WriteLine(String.Format(null,
                                                       @"PamsamBvtTest: {0} Align() method validation completed successfully with
                        number of processor is {1} and degree of parallelism {2}",
                                                       profileAligner,
                                                       Environment.ProcessorCount.ToString((IFormatProvider) null),
                                                       degreeOfParallelism));
            }
            else
            {
                Assert.Fail("Profile Aligner is not instantiated");
            }
        }

        /// <summary>
        ///     Get Hierarchical Clustering using kmerdistancematrix\kimura distance matrix and hierarchical method name.
        /// </summary>
        /// <param name="distanceMatrix">distance matrix.</param>
        /// <param name="hierarchicalClusteringMethodName">Hierarchical clustering method name.</param>
        /// <returns>Hierarchical clustering</returns>
        private static IHierarchicalClustering GetHierarchicalClustering(IDistanceMatrix distanceMatrix,
                                                                         UpdateDistanceMethodsTypes
                                                                             hierarchicalClusteringMethodName)
        {
            // Hierarchical clustering
            IHierarchicalClustering hierarcicalClustering =
                new HierarchicalClusteringParallel(distanceMatrix, hierarchicalClusteringMethodName);

            return hierarcicalClustering;
        }

        /// <summary>
        ///     Get Hierarchical Clustering using kmerdistancematrix\kimura distance matrix.
        /// </summary>
        /// <param name="distanceMatrix"></param>
        /// <param name="hierarchicalClusteringMethodName"></param>
        /// <returns>Hierarchical clustering</returns>
        private static IHierarchicalClustering GetHierarchicalClustering(IDistanceMatrix distanceMatrix)
        {
            // Hierarchical clustering with default distance method name
            IHierarchicalClustering hierarcicalClustering =
                new HierarchicalClusteringParallel(distanceMatrix);

            return hierarcicalClustering;
        }

        /// <summary>
        ///     Get distance matrix with distance function name
        /// </summary>
        /// <param name="kmrlength">kmr length</param>
        /// <param name="distanceFunctionName">distance matrix function name.</param>
        /// <returns>Distance matrix</returns>
        private IDistanceMatrix GetKmerDistanceMatrix(int kmrlength,
                                                      DistanceFunctionTypes distanceFunctionName)
        {
            // Generate DistanceMatrix
            var kmerDistanceMatrixGenerator =
                new KmerDistanceMatrixGenerator(lstSequences, kmrlength,
                                                lstSequences[0].Alphabet, distanceFunctionName);

            return kmerDistanceMatrixGenerator.DistanceMatrix;
        }

        /// <summary>
        ///     Get distance matrix with default distance function name
        /// </summary>
        /// <param name="kmrlength">kmr length</param>
        /// <returns>Distance matrix</returns>
        private IDistanceMatrix GetKmerDistanceMatrix(int kmrlength)
        {
            // Generate DistanceMatrix
            var kmerDistanceMatrixGenerator =
                new KmerDistanceMatrixGenerator((List<ISequence>) lstSequences, kmrlength, lstSequences[0].Alphabet);


            return kmerDistanceMatrixGenerator.DistanceMatrix;
        }

        /// <summary>
        ///     Get kimura distanc matrix using stage1 aligned sequences
        /// </summary>
        /// <param name="alignedSequences">aligned Sequences of stage 1</param>
        /// <returns>Distance matrix</returns>
        private static IDistanceMatrix GetKimuraDistanceMatrix(List<ISequence> alignedSequences)
        {
            // Generate DistanceMatrix from Multiple Sequence Alignment
            var kimuraDistanceMatrixGenerator =
                new KimuraDistanceMatrixGenerator();
            kimuraDistanceMatrixGenerator.GenerateDistanceMatrix(alignedSequences);

            return kimuraDistanceMatrixGenerator.DistanceMatrix;
        }

        /// <summary>
        ///     Get the binary tree object using hierarchical clustering object
        /// </summary>
        /// <param name="hierarchicalClustering">hierarchical Clustering</param>
        /// <returns>Binary guide tree</returns>
        private static BinaryGuideTree GetBinaryTree(IHierarchicalClustering hierarchicalClustering)
        {
            // Generate Guide Tree
            var binaryGuideTree =
                new BinaryGuideTree(hierarchicalClustering);

            return binaryGuideTree;
        }

        /// <summary>
        ///     Get the aligned sequence for stage1
        /// </summary>
        /// <returns>Sequence list</returns>
        private List<ISequence> GetStage1AlignedSequence()
        {
            // MSA aligned sequences.
            var msa =
                new PAMSAMMultipleSequenceAligner(lstSequences,
                                                  kmerLength, DistanceFunctionTypes.EuclideanDistance,
                                                  UpdateDistanceMethodsTypes.Average,
                                                  ProfileAlignerNames.NeedlemanWunschProfileAligner,
                                                  ProfileScoreFunctionNames.InnerProduct, similarityMatrix,
                                                  gapOpenPenalty,
                                                  gapExtendPenalty, 2, 2);
            return msa.AlignedSequencesA;
        }

        /// <summary>
        ///     Gets progressive aligner aligned sequences
        /// </summary>
        /// <param name="sequences">list of sequences</param>
        /// <param name="binaryGuidTree">binary guide tree</param>
        /// <returns>list of aligned sequences</returns>
        private List<ISequence> GetProgressiveAlignerAlignedSequences(IList<ISequence> sequences,
                                                                      BinaryGuideTree binaryGuidTree)
        {
            // Progressive Alignment
            IProgressiveAligner progressiveAligner = new ProgressiveAligner(profileAligner);
            progressiveAligner.Align(sequences, binaryGuidTree);

            return progressiveAligner.AlignedSequences;
        }

        /// <summary>
        ///     Gets profiles for the give edge index and binary tree
        /// </summary>
        /// <param name="edgeIndex">Edge index</param>
        /// <param name="binaryTree">Binary Guide tree</param>
        private void GetProfiles(int edgeIndex, BinaryGuideTree binaryTree)
        {
            // Cut Tree at an edge and get sequences.
            List<int>[] leafNodeIndices = binaryTree.SeparateSequencesByCuttingTree(edgeIndex);

            // Extract profiles and align it.
            List<int>[] removedPositions = null;
            IProfileAlignment[] separatedProfileAlignments =
                ProfileAlignment.ProfileExtraction(stage2ExpectedSequences,
                                                   leafNodeIndices[0], leafNodeIndices[1], out removedPositions);

            profileAligner.Align(separatedProfileAlignments[0], separatedProfileAlignments[1]);
        }

        /// <summary>
        ///     Validate different alignment score functions
        ///     using input sequences and reference sequences
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="type">Molecule Type</param>
        /// <param name="scoretype">Score Function Type.</param>
        private void ValidateAlignmentScore(string nodeName, ScoreType scoretype)
        {
            string inputFilePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string refFilePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.RefFilePathNode);

            ISequenceParser parser = null;
            ISequenceParser refParser = null;
            try
            {
                parser = new FastAParser(inputFilePath);
                IEnumerable<ISequence> sequences = parser.Parse();

                refParser = new FastAParser(refFilePath);
                IEnumerable<ISequence> refSequences = refParser.Parse();

                IList<ISequence> alignedSequences = GetPAMSAMAlignedSequences(sequences.ToList());

                // Validate the score
                switch (scoretype)
                {
                    case ScoreType.QScore:
                        string expectedQScore = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.QScoreNode);
                        float qScore = MsaUtils.CalculateAlignmentScoreQ(alignedSequences, refSequences.ToList());
                        Assert.AreEqual(expectedQScore, qScore.ToString((IFormatProvider) null));
                        break;
                    case ScoreType.TCScore:
                        string expectedTCScore = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.TCScoreNode);
                        float tcScore = MsaUtils.CalculateAlignmentScoreQ(alignedSequences, refSequences.ToList());
                        Assert.AreEqual(expectedTCScore, tcScore.ToString((IFormatProvider) null));
                        break;
                    case ScoreType.Offset:
                        string expectedResiduesCount = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                       Constants.ResiduesCountNode);
                        List<int> positions = MsaUtils.CalculateOffset(alignedSequences[0], refSequences.ElementAt(0));
                        int residuesCount = 0;
                        for (int i = 0; i < positions.Count; i++)
                        {
                            if (positions[i] < 0)
                            {
                                residuesCount++;
                            }
                        }
                        Assert.AreEqual(expectedResiduesCount, residuesCount.ToString((IFormatProvider) null));
                        break;
                    case ScoreType.MultipleAlignmentScoreFunction:
                        string expectedAlignScore = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                    Constants.ExpectedScoreNode);
                        float score = MsaUtils.MultipleAlignmentScoreFunction(
                            alignedSequences, similarityMatrix, gapOpenPenalty, gapExtendPenalty);

                        Assert.IsTrue(expectedAlignScore.Contains(score.ToString((IFormatProvider) null)));
                        break;
                    case ScoreType.PairWiseScoreFunction:
                        string expectedPairwiseScore = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                       Constants.PairWiseScoreNode);
                        float pairwiseScore = MsaUtils.PairWiseScoreFunction(
                            alignedSequences[0], alignedSequences[1], similarityMatrix,
                            gapOpenPenalty, gapExtendPenalty);
                        Assert.AreEqual(expectedPairwiseScore, pairwiseScore.ToString((IFormatProvider) null));
                        break;
                }

                ApplicationLog.WriteLine(
                    String.Format(null, @"PamsamBvtTest:{0} validation completed successfully",
                                  scoretype.ToString()));
            }
            finally
            {
                if (parser != null)
                    (parser).Dispose();
                if (refParser != null)
                    (refParser).Dispose();
            }
        }

        /// <summary>
        ///     Validate the UnAlign method is removing gaps from the sequence
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="type">Molecule Type</param>
        private void ValidateUNAlign(string nodeName)
        {
            string inputFilePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);

            ISequenceParser parser = null;
            try
            {
                parser = new FastAParser(inputFilePath);
                IEnumerable<ISequence> sequences = parser.Parse();
                List<ISequence> seqList = sequences.ToList();
                IList<ISequence> alignedSequences = GetPAMSAMAlignedSequences(seqList);
                var gapItem = (byte) '-';
                Assert.IsTrue(alignedSequences[0].Contains(gapItem));
                ISequence unalignedseq = MsaUtils.UnAlign(alignedSequences[0]);
                Assert.IsFalse(unalignedseq.Contains(gapItem));

                ApplicationLog.WriteLine(
                    String.Format(null, @"PamsamBvtTest:Validation of UnAlign() method of MsaUtils completed 
                    successfully"));
            }
            finally
            {
                if (parser != null)
                    (parser).Dispose();
            }
        }

        /// <summary>
        ///     Get Pamsam aligned sequences
        /// </summary>
        /// <param name="sequences">sequences.</param>
        /// <returns>returns aligned sequences</returns>
        private IList<ISequence> GetPAMSAMAlignedSequences(IList<ISequence> sequences)
        {
            similarityMatrix = new SimilarityMatrix(
                SimilarityMatrix.StandardSimilarityMatrix.AmbiguousDna);

            // MSA aligned sequences.
            var msa = new PAMSAMMultipleSequenceAligner(sequences,
                                                        kmerLength, DistanceFunctionTypes.EuclideanDistance,
                                                        UpdateDistanceMethodsTypes.Average,
                                                        ProfileAlignerNames.NeedlemanWunschProfileAligner,
                                                        ProfileScoreFunctionNames.InnerProductFast, similarityMatrix,
                                                        gapOpenPenalty, gapExtendPenalty, 2, 2);

            return msa.AlignedSequences;
        }

        /// <summary>
        ///     Validate function calculations of MsaUtils class.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="edgeIndex">Edge Index</param>
        /// <param name="functionType">Function Type.</param>
        private void ValidateFunctionCalculations(string nodeName,
                                                  int edgeIndex, FunctionType functionType)
        {
            // Get Two profiles
            IProfileAlignment[] separatedProfileAlignments = GetProfiles(edgeIndex);

            switch (functionType)
            {
                case FunctionType.Correlation:
                    float correlation = MsaUtils.Correlation(
                        separatedProfileAlignments[0].ProfilesMatrix.ProfilesMatrix[0],
                        separatedProfileAlignments[1].ProfilesMatrix.ProfilesMatrix[0]);
                    string expectedCorrelation = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                 Constants.CorrelationNode);
                    Assert.AreEqual(expectedCorrelation, correlation.ToString((IFormatProvider) null));
                    break;
                case FunctionType.FindMaxIndex:
                    string expectedMaxIndex = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                              Constants.MaxIndexNode);
                    int index = MsaUtils.FindMaxIndex(
                        separatedProfileAlignments[0].ProfilesMatrix.ProfilesMatrix[0]);
                    Assert.AreEqual(expectedMaxIndex, index.ToString((IFormatProvider) null));
                    break;
                case FunctionType.JensenShanonDivergence:
                    string expectedJsDivergence = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                  Constants.JensenShanonDivergenceNode);
                    float jsdivergence = MsaUtils.JensenShannonDivergence(
                        separatedProfileAlignments[0].ProfilesMatrix.ProfilesMatrix[0],
                        separatedProfileAlignments[1].ProfilesMatrix.ProfilesMatrix[0]);
                    Assert.AreEqual(expectedJsDivergence, jsdivergence.ToString((IFormatProvider) null));
                    break;
                case FunctionType.KullbackLeiblerDistance:
                    string expectedKlDistance = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                Constants.KullbackLeiblerDistanceNode);
                    float kldistance = MsaUtils.KullbackLeiblerDistance(
                        separatedProfileAlignments[0].ProfilesMatrix.ProfilesMatrix[0],
                        separatedProfileAlignments[1].ProfilesMatrix.ProfilesMatrix[0]);
                    Assert.AreEqual(expectedKlDistance, kldistance.ToString((IFormatProvider) null));
                    break;
            }

            ApplicationLog.WriteLine(
                String.Format(null, @"Validation of {0} function calculation of MsaUtils completed 
                    successfully", functionType));
        }

        /// <summary>
        ///     Validate Muscle multiple sequence alignment with static properties
        ///     of PamsamMultipleSequenceAligner.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="hierarchicalClusteringMethodName"></param>
        /// <param name="distanceFunctionName">kmerdistancematrix method name.</param>
        /// <param name="profileAlignerName"></param>
        /// <param name="profileScoreName">Profile score function name.</param>
        /// <param name="useweights">use sequence weights true\false</param>
        /// <param name="fasterVersion">fasterversion true\false</param>
        /// <param name="useStageB">stage2 computation true\false</param>
        /// <param name="expectedScoreNode"></param>
        private void ValidatePamsamAlign(string nodeName,
                                         string expectedScoreNode,
                                         UpdateDistanceMethodsTypes hierarchicalClusteringMethodName,
                                         DistanceFunctionTypes distanceFunctionName,
                                         ProfileAlignerNames profileAlignerName,
                                         ProfileScoreFunctionNames profileScoreName,
                                         bool useweights,
                                         bool fasterVersion,
                                         bool useStageB)
        {
            Initialize(nodeName, expectedScoreNode);

            // get old properties
            bool prevVersion = PAMSAMMultipleSequenceAligner.FasterVersion;
            bool prevUseWeights = PAMSAMMultipleSequenceAligner.UseWeights;
            bool prevUseStageB = PAMSAMMultipleSequenceAligner.UseStageB;

            try
            {
                // Set static properties
                PAMSAMMultipleSequenceAligner.FasterVersion = fasterVersion;
                PAMSAMMultipleSequenceAligner.UseWeights = useweights;
                PAMSAMMultipleSequenceAligner.UseStageB = useStageB;

                // MSA aligned sequences.
                int numberOfDegrees = 2;
                int numberOfPartitions = 2;
                var msa =
                    new PAMSAMMultipleSequenceAligner(lstSequences,
                                                      kmerLength, distanceFunctionName, hierarchicalClusteringMethodName,
                                                      profileAlignerName, profileScoreName, similarityMatrix,
                                                      gapOpenPenalty,
                                                      gapExtendPenalty, numberOfDegrees, numberOfPartitions);

                // Validate the aligned Sequence and score
                if (fasterVersion)
                {
                    InitializeStage1Variables(nodeName);
                    Assert.AreEqual(stage1ExpectedSequences.Count, msa.AlignedSequences.Count);
                    int index = 0;
                    foreach (ISequence seq in msa.AlignedSequences)
                    {
                        Assert.AreEqual(new string(seq.Select(a => (char) a).ToArray()),
                                        new string(stage1ExpectedSequences[index].Select(a => (char) a).ToArray()));
                        index++;
                    }
                    Assert.IsTrue(stage1ExpectedScore.Contains(msa.AlignmentScore.ToString((IFormatProvider) null)));
                }
                else
                {
                    int index = 0;
                    foreach (ISequence seq in msa.AlignedSequences)
                    {
                        Assert.AreEqual(new string(seq.Select(a => (char) a).ToArray()),
                                        new string(expectedSequences[index].Select(a => (char) a).ToArray()));
                        index++;
                    }
                    Assert.AreEqual(expectedScore, msa.AlignmentScore.ToString((IFormatProvider) null));
                }
            }
            finally
            {
                // Reset it back
                PAMSAMMultipleSequenceAligner.FasterVersion = prevVersion;
                PAMSAMMultipleSequenceAligner.UseWeights = prevUseWeights;
                PAMSAMMultipleSequenceAligner.UseStageB = prevUseStageB;
            }

            ApplicationLog.WriteLine(
                String.Format(null, @"Validation of pamsam alignment completed 
                      successfully for molecule type {0} with 
                      static property fasterversion {0}, usestageb {1} and useweights {2}",
                              fasterVersion, useStageB, useweights));
        }


        /// <summary>
        ///     Creates binarytree using stage1 sequences.
        ///     Cut the binary tree at an random edge to get two profiles.
        /// </summary>
        /// <param name="edgeIndex">Random edge index.</param>
        /// <returns>Returns profiles</returns>
        private IProfileAlignment[] GetProfiles(int edgeIndex)
        {
            Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
            InitializeStage2Variables(Constants.MuscleDnaSequenceNode);

            // Get Stage2 Binary Tree
            List<ISequence> stage1AlignedSequences = GetStage1AlignedSequence();
            IDistanceMatrix matrix = GetKimuraDistanceMatrix(stage1AlignedSequences);
            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix);
            BinaryGuideTree binaryTree = GetBinaryTree(hierarcicalClustering);

            // Cut Tree at an edge and get sequences.
            List<int>[] leafNodeIndices = binaryTree.SeparateSequencesByCuttingTree(edgeIndex);

            // Extract profiles 
            List<int>[] removedPositions = null;
            IProfileAlignment[] separatedProfileAlignments = ProfileAlignment.ProfileExtraction(
                stage2ExpectedSequences, leafNodeIndices[0], leafNodeIndices[1], out removedPositions);

            return separatedProfileAlignments;
        }

        #endregion

        #endregion
    }
}