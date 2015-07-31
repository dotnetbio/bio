/****************************************************************************
 * PamSamP1TestCases.cs
 * 
 *   This file contains the MuscleMultipleSequenceAlignment P1 test cases.
 * 
***************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    ///     The class contains P1 test cases to confirm Muscle MSA alignment.
    /// </summary>
    [TestClass]
    public class PamSamP1TestCases
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
        ///     Dummy MoleculeType so that can be included in case if it is used in the future
        /// </summary>
        private enum MoleculeType
        {
            Protein,
            RNA,
            DNA
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
        private int gapExtendPenalty = -1;

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
        private List<ISequence> lstSequences;

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

        /// <summary>
        ///     Initialize the expected score of Stage3.
        /// </summary>
        private string stage3ExpectedScore = string.Empty;

        /// <summary>
        ///     Initialize expected aligned sequence list for stage3.
        /// </summary>
        private List<ISequence> stage3ExpectedSequences;

        #endregion

        #region Constructor

        /// <summary>
        ///     Static constructor to open log and make other settings needed for test
        /// </summary>
        static PamSamP1TestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion

        #region Test Cases

        #region General Test Cases

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and default params
        ///     using its aligned sequences and score
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequences()
        {
            ValidatePamsamAlign(Constants.MuscleProteinSequenceNode,
                                MoleculeType.Protein, Constants.ExpectedScoreNode,
                                ProfileAlignerNames.NeedlemanWunschProfileAligner);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and default params
        ///     using its aligned sequences
        ///     and score with distance matrix method name as ModifiedMuscle
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequencesAndModifiedMuscle()
        {
            ValidatePamsamAlignWithDistanceFunctionaTypes(
                Constants.MuscleProteinSequenceWithModifiedMuscleNodeName,
                MoleculeType.Protein, Constants.ExpectedScoreNode,
                DistanceFunctionTypes.ModifiedMUSCLE, ProfileAlignerNames.NeedlemanWunschProfileAligner);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and default params
        ///     using its aligned sequences
        ///     and score with distance matrix method name as EuclieanDistance
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequencesAndEuclieanDistance()
        {
            ValidatePamsamAlignWithDistanceFunctionaTypes(
                Constants.MuscleProteinSequenceEuclieanDistanceNode,
                MoleculeType.Protein, Constants.ExpectedScoreNode,
                DistanceFunctionTypes.EuclideanDistance, ProfileAlignerNames.NeedlemanWunschProfileAligner);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and default params
        ///     using its aligned sequences
        ///     and score with distance matrix method name as CoVariance
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequencesAndCovariance()
        {
            ValidatePamsamAlignWithDistanceFunctionaTypes(
                Constants.MuscleProteinSequenceWithCovarianceNodeName,
                MoleculeType.Protein, Constants.ExpectedScoreNode,
                DistanceFunctionTypes.CoVariance, ProfileAlignerNames.NeedlemanWunschProfileAligner);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and default params
        ///     using its aligned sequences
        ///     and score with distance matrix method name as PearsonCorrelation
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequencesAndPearsonCorrelation()
        {
            ValidatePamsamAlignWithDistanceFunctionaTypes(
                Constants.MuscleProteinSequenceWithPearsonCorrelationNodeName,
                MoleculeType.Protein, Constants.ExpectedScoreNode,
                DistanceFunctionTypes.PearsonCorrelation, ProfileAlignerNames.NeedlemanWunschProfileAligner);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and default params
        ///     using its aligned sequences
        ///     and score with Hierarchical Clustering method name as Average
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequencesAndAverageMethod()
        {
            ValidatePamsamAlignWithUpdateDistanceMethodTypes(Constants.MuscleProteinSequenceNode,
                                                             MoleculeType.Protein, Constants.ExpectedScoreNode,
                                                             UpdateDistanceMethodsTypes.Average,
                                                             ProfileAlignerNames.NeedlemanWunschProfileAligner);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and default params
        ///     using its aligned sequences and score with
        ///     Hierarchical Clustering method name as Complete
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequencesAndCompleteMethod()
        {
            ValidatePamsamAlignWithUpdateDistanceMethodTypes(Constants.MuscleProteinSequenceWithComplete,
                                                             MoleculeType.Protein, Constants.ExpectedScoreNode,
                                                             UpdateDistanceMethodsTypes.Complete,
                                                             ProfileAlignerNames.NeedlemanWunschProfileAligner);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and default params
        ///     using its aligned sequences and score with
        ///     Hierarchical Clustering method name as Single
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequencesAndSingleMethod()
        {
            ValidatePamsamAlignWithUpdateDistanceMethodTypes(
                Constants.MuscleProteinSequenceWithSingleMethodNodeName,
                MoleculeType.Protein, Constants.ExpectedScoreNode,
                UpdateDistanceMethodsTypes.Single, ProfileAlignerNames.NeedlemanWunschProfileAligner);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and default params
        ///     using its aligned sequences and score with
        ///     Hierarchical Clustering method name as WeightedMAFFT
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequencesAndWeightedMafftMethod()
        {
            ValidatePamsamAlignWithUpdateDistanceMethodTypes(
                Constants.MuscleProteinSequenceWithWeightedMAFFTNodeName,
                MoleculeType.Protein, Constants.ExpectedScoreNode,
                UpdateDistanceMethodsTypes.WeightedMAFFT,
                ProfileAlignerNames.NeedlemanWunschProfileAligner);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and default params
        ///     using its aligned sequences and score.
        ///     Profile score method name as InnerProduct
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequencesAndInnerProduct()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleProteinSequenceWithInnerProductNodeName,
                MoleculeType.Protein,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.InnerProduct);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and default params
        ///     using its aligned sequences and score.
        ///     Profile score method name as JensenShannonDivergence
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequencesAndJensenShannonDivergence()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleProteinWithJensenShannonDivergence,
                MoleculeType.Protein,
                Constants.ExpectedScoreNode, ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.JensenShannonDivergence);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and default params
        ///     using its aligned sequences and score.
        ///     Profile score method name as LogExponentialInnerProduct
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequencesAndLogExponentialInnerProduct()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleProteinWithLogExponentialInnerProduct,
                MoleculeType.Protein, Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.LogExponentialInnerProduct);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and default params
        ///     using its aligned sequences and score.
        ///     Profile score method name as LogExponentialInnerProductShifted
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequencesAndLogExponentialInnerProductShifted()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleProteinWithLogExponentialInnerProductShifted,
                MoleculeType.Protein,
                Constants.ExpectedScoreNode, ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.LogExponentialInnerProductShifted);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and default params
        ///     using its aligned sequences and score.
        ///     Profile score method name as PearsonCorrelation
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequencesAndPearsonCorrelationProfileScore()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleProteinWithPearsonCorrelationProfileScore,
                MoleculeType.Protein,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.PearsonCorrelation);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and default params
        ///     using its aligned sequences and score.
        ///     Profile score method name as SymmetrizedEntropy
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequencesAndSymmetrizedEntropy()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleProteinWithSymmetrizedEntropy,
                MoleculeType.Protein,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.SymmetrizedEntropy);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and default params
        ///     using its aligned sequences and score.
        ///     Profile score method name as WeightedEuclideanDistance
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequencesAndWeightedEuclideanDistance()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleProteinWithWeightedEuclideanDistanceNodeName,
                MoleculeType.Protein, Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.WeightedEuclideanDistance);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and other default params
        ///     using its aligned sequences and score.
        ///     Profile score method name as WeightedInnerProduct
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequencesAndWeightedInnerProduct()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleProteinSequenceNode,
                MoleculeType.Protein,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.WeightedInnerProduct);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and default params
        ///     using its aligned sequences and score.
        ///     Profile score method name as WeightedInnerProductShifted
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequencesAndWeightedInnerProductShifted()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleProteinSequenceWithWeightedInnerProductShifted,
                MoleculeType.Protein,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.WeightedInnerProductShifted);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences
        ///     Stage1 using its aligned sequences and score
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamStage1WithProteinSequences()
        {
            ValidatePamsamAlignStage1(Constants.MuscleProteinSequenceNode,
                                      MoleculeType.Protein, Constants.ExpectedScoreNode,
                                      UpdateDistanceMethodsTypes.Average, DistanceFunctionTypes.EuclideanDistance,
                                      ProfileAlignerNames.NeedlemanWunschProfileAligner,
                                      ProfileScoreFunctionNames.InnerProduct, false);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences
        ///     alignment Stage2 using its aligned sequences and score
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamStage2WithProteinSequences()
        {
            ValidatePamsamAlignStage2(Constants.MuscleProteinSequenceNode,
                                      MoleculeType.Protein, Constants.ExpectedScoreNode,
                                      UpdateDistanceMethodsTypes.Average, DistanceFunctionTypes.EuclideanDistance,
                                      ProfileAlignerNames.NeedlemanWunschProfileAligner,
                                      ProfileScoreFunctionNames.InnerProduct);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences
        ///     Stage3 using its aligned sequences and score
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamStage3WithProteinSequences()
        {
            ValidatePamsamAlignStage3(Constants.MuscleProteinSequenceNode,
                                      MoleculeType.Protein, Constants.ExpectedScoreNode,
                                      UpdateDistanceMethodsTypes.Average, DistanceFunctionTypes.EuclideanDistance,
                                      ProfileAlignerNames.NeedlemanWunschProfileAligner,
                                      ProfileScoreFunctionNames.InnerProduct, false);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences and score
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequences()
        {
            ValidatePamsamAlign(Constants.MuscleRnaSequenceNode,
                                MoleculeType.RNA, Constants.ExpectedScoreNode,
                                ProfileAlignerNames.NeedlemanWunschProfileAligner);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences
        ///     and score with distance matrix method name as ModifiedMuscle
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequencesAndModifiedMuscle()
        {
            ValidatePamsamAlignWithDistanceFunctionaTypes(
                Constants.MultipleNWProfilerRnaSequenceWithModifiedMuscle,
                MoleculeType.RNA, Constants.ExpectedScoreNode,
                DistanceFunctionTypes.ModifiedMUSCLE, ProfileAlignerNames.NeedlemanWunschProfileAligner);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences
        ///     and score with distance matrix method name as EuclieanDistance
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequencesAndEuclieanDistance()
        {
            ValidatePamsamAlignWithDistanceFunctionaTypes(Constants.MuscleRnaSequenceNode,
                                                          MoleculeType.RNA, Constants.ExpectedScoreNode,
                                                          DistanceFunctionTypes.EuclideanDistance,
                                                          ProfileAlignerNames.NeedlemanWunschProfileAligner);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences
        ///     and score with distance matrix method name as CoVariance
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequencesAndCovariance()
        {
            ValidatePamsamAlignWithDistanceFunctionaTypes(
                Constants.MuscleRnaSequenceWithCoVarianceNodeName,
                MoleculeType.RNA, Constants.ExpectedScoreNode,
                DistanceFunctionTypes.CoVariance, ProfileAlignerNames.NeedlemanWunschProfileAligner);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences
        ///     and score with distance matrix method name as PearsonCorrelation
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequencesAndPearsonCorrelation()
        {
            ValidatePamsamAlignWithDistanceFunctionaTypes(
                Constants.MuscleRnaWithPearsonCorrelation,
                MoleculeType.RNA, Constants.ExpectedScoreNode,
                DistanceFunctionTypes.PearsonCorrelation,
                ProfileAlignerNames.NeedlemanWunschProfileAligner);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences
        ///     and score with Hierarchical Clustering method name as Average
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequencesAndAverageMethod()
        {
            ValidatePamsamAlignWithUpdateDistanceMethodTypes(Constants.MuscleRnaSequenceNode,
                                                             MoleculeType.RNA, Constants.ExpectedScoreNode,
                                                             UpdateDistanceMethodsTypes.Average,
                                                             ProfileAlignerNames.NeedlemanWunschProfileAligner);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences and score with
        ///     Hierarchical Clustering method name as Complete
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequencesAndCompleteMethod()
        {
            ValidatePamsamAlignWithUpdateDistanceMethodTypes(Constants.MuscleRnaSequenceNode,
                                                             MoleculeType.RNA, Constants.ExpectedScoreNode,
                                                             UpdateDistanceMethodsTypes.Complete,
                                                             ProfileAlignerNames.NeedlemanWunschProfileAligner);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences and score with
        ///     Hierarchical Clustering method name as Single
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequencesAndSingleMethod()
        {
            ValidatePamsamAlignWithUpdateDistanceMethodTypes(
                Constants.MultipleNWProfilerRnaSequenceWithSingleMethod,
                MoleculeType.RNA, Constants.ExpectedScoreNode,
                UpdateDistanceMethodsTypes.Single,
                ProfileAlignerNames.NeedlemanWunschProfileAligner);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences and score with
        ///     Hierarchical Clustering method name as WeightedMAFFT
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequencesAndWeightedMafftMethod()
        {
            ValidatePamsamAlignWithUpdateDistanceMethodTypes(
                Constants.MultipleNWProfilerRnaSequenceWithWeightedMAFFTMethod,
                MoleculeType.RNA, Constants.ExpectedScoreNode,
                UpdateDistanceMethodsTypes.WeightedMAFFT,
                ProfileAlignerNames.NeedlemanWunschProfileAligner);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences and score.
        ///     Profile score method name as InnerProduct
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequencesAndInnerProduct()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MultipleNWProfilerRnaSequenceWithInnerProduct,
                MoleculeType.RNA,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.InnerProduct);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences and score.
        ///     Profile score method name as JensenShannonDivergence
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequencesAndJensenShannonDivergence()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(Constants.MuscleRnaWithJensenShannonDivergence,
                                                            MoleculeType.RNA,
                                                            Constants.ExpectedScoreNode,
                                                            ProfileAlignerNames.NeedlemanWunschProfileAligner,
                                                            ProfileScoreFunctionNames.JensenShannonDivergence);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences and score.
        ///     Profile score method name as LogExponentialInnerProduct
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequencesAndLogExponentialInnerProduct()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleRnaWithLogExponentialInnerProduct,
                MoleculeType.RNA, Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.LogExponentialInnerProduct);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences and score.
        ///     Profile score method name as LogExponentialInnerProductShifted
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequencesAndLogExponentialInnerProductShifted()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MultipleNWProfilerRnaSequenceWithLogExponentialInnerProductShiftedNode,
                MoleculeType.RNA,
                Constants.ExpectedScoreNode, ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.LogExponentialInnerProductShifted);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences and score.
        ///     Profile score method name as PearsonCorrelation
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequencesAndPearsonCorrelationProfileScore()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MultipleNWProfilerRnaSequenceWithPearsonCorrelationScore,
                MoleculeType.RNA,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.PearsonCorrelation);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences and score.
        ///     Profile score method name as SymmetrizedEntropy
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequencesAndSymmetrizedEntropy()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleRnaWithSymmetrizedEntropy,
                MoleculeType.RNA,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.SymmetrizedEntropy);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences and score.
        ///     Profile score method name as WeightedEuclideanDistance
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequencesAndWeightedEuclideanDistance()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MultipleNWProfilerRnaSequenceWithWeightedEuclideanDistance,
                MoleculeType.RNA, Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.WeightedEuclideanDistance);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences and score.
        ///     Profile score method name as WeightedInnerProduct
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequencesAndWeightedInnerProduct()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MultipleNWProfilerRnaSequenceWithWeightedInnerProduct,
                MoleculeType.RNA,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.WeightedInnerProduct);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences and score.
        ///     Profile score method name as WeightedInnerProductShifted
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequencesAndWeightedInnerProductShifted()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MultipleNWProfilerRnaSequenceWithWeightedInnerProductShifted,
                MoleculeType.RNA,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.WeightedInnerProductShifted);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences and score.
        ///     Profile score method name as InnerProductFast
        ///     Input :7 Rna unaligned Sequences
        ///     Output : 7 Rna aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequencesAndInnerProductFast()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MultipleNWProfilerRnaSequenceWithInnerProductFastNodeName,
                MoleculeType.RNA,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.InnerProductFast);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences and score.
        ///     Profile score method name as LogExponentialInnerProductFast
        ///     Input :7 Rna unaligned Sequences
        ///     Output : 7 Rna aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequencesAndLogExponentialInnerProductFast()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleRnaSequenceWithLogExponentialInnerProductFastNode,
                MoleculeType.RNA,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.LogExponentialInnerProductFast);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences and score.
        ///     Profile score method name as LogExponentialInnerProductShiftedFast
        ///     Input :7 Rna unaligned Sequences
        ///     Output : 7 Rna aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequencesAndLogExponentialInnerProductShiftedFast()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleRnaSequenceWithLogExponentialInnerProductFastNode,
                MoleculeType.RNA,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.LogExponentialInnerProductShiftedFast);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences and score.
        ///     Profile score method name as WeightedEuclideanDistanceFast
        ///     Input :7 Rna unaligned Sequences
        ///     Output : 7 Rna aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequencesAndWeightedEuclideanDistanceFast()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleRnaSequenceWithWeightedEuclideanDistanceFast,
                MoleculeType.RNA, Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.WeightedEuclideanDistanceFast);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 Rna sequences
        ///     using its aligned sequences and score.
        ///     Profile score method name as WeightedInnerProductShiftedFast
        ///     Input :7 Rna unaligned Sequences
        ///     Output : 7 Rna aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithRnaSequencesAndWeightedInnerProductShiftedFast()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MultipleNWProfilerRnaSequenceWithPearsonCorrelationScore,
                MoleculeType.RNA,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.WeightedInnerProductShiftedFast);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and default params
        ///     using its aligned sequences and score.
        ///     Profile score method name as InnerProductFast
        ///     Input :7 Protein unaligned Sequences
        ///     Output : 7 Protein aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequencesAndInnerProductFast()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleProteinSequenceWithInnerProductFastNode,
                MoleculeType.Protein,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.InnerProductFast);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and default params
        ///     using its aligned sequences and score.
        ///     Profile score method name as LogExponentialInnerProductFast
        ///     Input :7 Protein unaligned Sequences
        ///     Output : 7 Protein aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequencesAndLogExponentialInnerProductFast()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleProteinSequenceWithLogExponentialInnerProductFastNode,
                MoleculeType.Protein, Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.LogExponentialInnerProductFast);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and default params
        ///     using its aligned sequences and score.
        ///     Profile score method name as LogExponentialInnerProductShiftedFast
        ///     Input :7 Protein unaligned Sequences
        ///     Output : 7 Protein aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequencesAndLogExponentialInnerProductShiftedFast()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleProteinSequenceWithLogExponentialInnerProductShiftedFastNode,
                MoleculeType.Protein,
                Constants.ExpectedScoreNode, ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.LogExponentialInnerProductShiftedFast);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and default params
        ///     using its aligned sequences and score.
        ///     Profile score method name as WeightedEuclideanDistanceFast
        ///     Input :7 Protein unaligned Sequences
        ///     Output : 7 Protein aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequencesAndWeightedEuclideanDistanceFast()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleProteinWithWeightedEuclideanDistanceNodeName,
                MoleculeType.Protein, Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.WeightedEuclideanDistanceFast);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 7 protein sequences and default params
        ///     using its aligned sequences and score.
        ///     Profile score method name as WeightedInnerProductShiftedFast
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithProteinSequencesAndWeightedInnerProductShiftedFast()
        {
            ValidatePamsamAlignWithProfileScoreFunctionName(
                Constants.MuscleProteinSequenceWithWeightedInnerProductShiftedFastNode,
                MoleculeType.Protein,
                Constants.ExpectedScoreNode,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.WeightedInnerProductShiftedFast);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment Stage1 with 7 Rna sequences
        ///     using its aligned sequences and score
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamStage1WithRnaSequences()
        {
            ValidatePamsamAlignStage1(Constants.MuscleRnaSequenceNode,
                                      MoleculeType.RNA, Constants.ExpectedScoreNode,
                                      UpdateDistanceMethodsTypes.Average, DistanceFunctionTypes.EuclideanDistance,
                                      ProfileAlignerNames.NeedlemanWunschProfileAligner,
                                      ProfileScoreFunctionNames.InnerProduct, false);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment Stage2 with 7 Rna sequences
        ///     using its aligned sequences and score
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamStage2WithRnaSequences()
        {
            ValidatePamsamAlignStage2(Constants.MuscleRnaSequenceNode,
                                      MoleculeType.RNA, Constants.ExpectedScoreNode,
                                      UpdateDistanceMethodsTypes.Average, DistanceFunctionTypes.EuclideanDistance,
                                      ProfileAlignerNames.NeedlemanWunschProfileAligner,
                                      ProfileScoreFunctionNames.InnerProduct);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment Stage3 with 7 Rna sequences
        ///     using its aligned sequences and score
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamStage3WithRnaSequences()
        {
            ValidatePamsamAlignStage3(Constants.MuscleRnaSequenceNode,
                                      MoleculeType.RNA, Constants.ExpectedScoreNode,
                                      UpdateDistanceMethodsTypes.Average, DistanceFunctionTypes.EuclideanDistance,
                                      ProfileAlignerNames.NeedlemanWunschProfileAligner,
                                      ProfileScoreFunctionNames.InnerProduct, false);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 10 dna one line sequences and default params
        ///     using its aligned sequences and score
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithOneLineDnaSequences()
        {
            ValidatePamsamAlignOneLineSequences(Constants.MuscleDnaOneLineSequence,
                                                MoleculeType.DNA, Constants.ExpectedScoreNode,
                                                ProfileAlignerNames.NeedlemanWunschProfileAligner);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 10 Rna one line sequences and default params
        ///     using its aligned sequences and score
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithOneLineRnaSequences()
        {
            ValidatePamsamAlignOneLineSequences(Constants.MuscleRnaOneLineSequence,
                                                MoleculeType.RNA, Constants.ExpectedScoreNode,
                                                ProfileAlignerNames.NeedlemanWunschProfileAligner);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with 10 Rna one line sequences and default params
        ///     using its aligned sequences and score
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamWithOneLineProteinSequences()
        {
            ValidatePamsamAlignOneLineSequences(Constants.MuscleProteinOneLineSequence,
                                                MoleculeType.Protein, Constants.ExpectedScoreNode,
                                                ProfileAlignerNames.NeedlemanWunschProfileAligner);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with Dna sequences and default params
        ///     with equal gap open cost and penalty(-3,-3) using its aligned sequences and score
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamDnaSequencesWithEqualGapCostandPenalty()
        {
            ValidatePamsamAlignWithGapCost(Constants.MuscleDnaEqualGapCost,
                                           MoleculeType.DNA, Constants.ExpectedScoreNode,
                                           UpdateDistanceMethodsTypes.Average, DistanceFunctionTypes.EuclideanDistance,
                                           ProfileAlignerNames.NeedlemanWunschProfileAligner,
                                           ProfileScoreFunctionNames.WeightedInnerProduct, -3, -3, true);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with Rna sequences and default params
        ///     with equal gap open cost and penalty(-3,-3) using its aligned sequences and score
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamRnaSequencesWithEqualGapCostandPenalty()
        {
            ValidatePamsamAlignWithGapCost(Constants.MuscleRnaEqualGapCost,
                                           MoleculeType.RNA, Constants.ExpectedScoreNode,
                                           UpdateDistanceMethodsTypes.Average, DistanceFunctionTypes.EuclideanDistance,
                                           ProfileAlignerNames.NeedlemanWunschProfileAligner,
                                           ProfileScoreFunctionNames.WeightedInnerProduct, -3, -3, false);
        }

        /// <summary>
        ///     Validates Muscle sequence alignment with Protein sequences and default params
        ///     with equal gap open cost and penalty(-3,-3) using its aligned sequences and score
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamProteinSequencesWithEqualGapCostandPenalty()
        {
            ValidatePamsamAlignWithGapCost(Constants.MuscleProteinEqualGapCost,
                                           MoleculeType.Protein, Constants.ExpectedScoreNode,
                                           UpdateDistanceMethodsTypes.Average, DistanceFunctionTypes.EuclideanDistance,
                                           ProfileAlignerNames.NeedlemanWunschProfileAligner,
                                           ProfileScoreFunctionNames.WeightedInnerProduct, -3, -3, false);
        }

        #endregion

        #region KmerDistanceMatrix

        /// <summary>
        ///     Validate kmerdistancematrix for stage1 with 7 Protein sequences
        ///     and default distance function name
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamKmerDistanceMatrixStage1WithProteinSequences()
        {
            ValidateKmerDistanceMatrixStage1(Constants.KmerDistanceMatrixProtein,
                                             kmerLength, MoleculeType.Protein);
        }

        /// <summary>
        ///     Validate kmerdistancematrix for stage1 with 7 Protein sequences
        ///     and EuclieanDistance distance function name
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamKmerDistanceMatrixWithProteinSequencesAndEuclieanDistance()
        {
            ValidateKmerDistanceMatrixStage1(Constants.KmerDistanceMatrixProtein, kmerLength,
                                             MoleculeType.Protein, DistanceFunctionTypes.EuclideanDistance);
        }

        /// <summary>
        ///     Validate kmerdistancematrix for stage1 with 7 Protein sequences
        ///     and PearsonCorrelation distance function name
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamKmerDistanceMatrixWithProteinSequencesAndPearsonCorrelation()
        {
            ValidateKmerDistanceMatrixStage1(Constants.KmerDistanceMatrixProteinWithPearsonCorrelation,
                                             kmerLength,
                                             MoleculeType.Protein, DistanceFunctionTypes.PearsonCorrelation);
        }

        /// <summary>
        ///     Validate kmerdistancematrix for stage1 with 7 Protein sequences
        ///     and CoVariance distance function name
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamKmerDistanceMatrixWithProteinSequencesAndCOVariance()
        {
            ValidateKmerDistanceMatrixStage1(Constants.KmerDistanceMatrixProteinWithCoVariance, kmerLength,
                                             MoleculeType.Protein, DistanceFunctionTypes.CoVariance);
        }

        /// <summary>
        ///     Validate kmerdistancematrix for stage1 with 7 Protein sequences
        ///     and ModifiedMUSCLE distance function name
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamKmerDistanceMatrixWithProteinSequencesAndModifiedMuscle()
        {
            ValidateKmerDistanceMatrixStage1(Constants.KmerDistanceMatrixProteinWithModifiedMuscle,
                                             kmerLength,
                                             MoleculeType.Protein, DistanceFunctionTypes.ModifiedMUSCLE);
        }

        /// <summary>
        ///     Validate kmerdistancematrix for stage1 with 7 Rna sequences and default distance function name
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamKmerDistanceMatrixStage1WithRnaSequences()
        {
            ValidateKmerDistanceMatrixStage1(Constants.KmerDistanceMatrixRna,
                                             kmerLength, MoleculeType.RNA);
        }

        /// <summary>
        ///     Validate kmerdistancematrix for stage1 with 7 Rna sequences
        ///     and EuclieanDistance distance function name
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamKmerDistanceMatrixWithRnaSequencesAndEuclieanDistance()
        {
            ValidateKmerDistanceMatrixStage1(Constants.KmerDistanceMatrixRna, kmerLength,
                                             MoleculeType.RNA, DistanceFunctionTypes.EuclideanDistance);
        }

        /// <summary>
        ///     Validate kmerdistancematrix for stage1 with 7 Rna sequences
        ///     and PearsonCorrelation distance function name
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamKmerDistanceMatrixWithRnaSequencesAndPearsonCorrelation()
        {
            ValidateKmerDistanceMatrixStage1(
                Constants.KmerDistanceMatrixRnaWithPearsonCorrelation, kmerLength,
                MoleculeType.RNA, DistanceFunctionTypes.PearsonCorrelation);
        }

        /// <summary>
        ///     Validate kmerdistancematrix for stage1 with 7 Rna sequences
        ///     and CoVariance distance function name
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamKmerDistanceMatrixWithRnaSequencesAndCOVariance()
        {
            ValidateKmerDistanceMatrixStage1(Constants.KmerDistanceMatrixRnaWithCoVariance, kmerLength,
                                             MoleculeType.RNA, DistanceFunctionTypes.CoVariance);
        }

        /// <summary>
        ///     Validate kmerdistancematrix for stage1 with 7 Rna sequences
        ///     and Modified MUSCLE distance function name
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamKmerDistanceMatrixWithRnaSequencesAndModifiedMuscle()
        {
            ValidateKmerDistanceMatrixStage1(Constants.KmerDistanceMatrixRnaWithModifiedMuscle, kmerLength,
                                             MoleculeType.RNA, DistanceFunctionTypes.ModifiedMUSCLE);
        }

        #endregion

        #region HierarchicalClusteringStage1&Stage2

        /// <summary>
        ///     Validate HierarchicalClustering for stage1  with 7 Protein sequences and kmer distance matrix
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamHierarchicalClusteringStage1()
        {
            ValidateHierarchicalClusteringStage1(Constants.HierarchicalClusteringProteinNode,
                                                 kmerLength, MoleculeType.Protein);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1 with 7 Protein sequences, kmer distance matrix
        ///     and hierarchical clustering method name as Average
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamHierarchicalClusteringWithAverage()
        {
            ValidateHierarchicalClusteringStage1(Constants.HierarchicalClusteringProteinNode,
                                                 kmerLength, MoleculeType.Protein, UpdateDistanceMethodsTypes.Average);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1 with 7 Protein sequences, kmer distance matrix
        ///     and hierarchical clustering method name as Single
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamHierarchicalClusteringWithSingle()
        {
            ValidateHierarchicalClusteringStage1(Constants.HierarchicalClusteringProteinWithWeightedMAFFT,
                                                 kmerLength, MoleculeType.Protein, UpdateDistanceMethodsTypes.Single);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1 with 7 Protein sequences, kmer distance matrix
        ///     and hierarchical clustering method name as Complete
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamHierarchicalClusteringWithComplete()
        {
            ValidateHierarchicalClusteringStage1(Constants.HierarchicalClusteringProteinWithComplete,
                                                 kmerLength, MoleculeType.Protein, UpdateDistanceMethodsTypes.Complete);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1 with 7 Protein sequences, kmer distance matrix
        ///     and hierarchical clustering method name as WeightedMAFFT
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamHierarchicalClusteringWithWeightedMafft()
        {
            ValidateHierarchicalClusteringStage1(
                Constants.HierarchicalClusteringProteinWithWeightedMAFFT, kmerLength,
                MoleculeType.Protein, UpdateDistanceMethodsTypes.WeightedMAFFT);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1 with 7 Protein sequences, kimura distance matrix
        ///     and stage 1 aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamHierarchicalClusteringStage2()
        {
            ValidateHierarchicalClusteringStage2(Constants.HierarchicalClusteringProteinStage2Node,
                                                 MoleculeType.Protein);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1
        ///     with 7 Protein sequences, kimura distance matrix with hierarchical method name as Average
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamHierarchicalClusteringStage2WithAverage()
        {
            ValidateHierarchicalClusteringStage2(Constants.HierarchicalClusteringProteinStage2Node,
                                                 MoleculeType.Protein,
                                                 UpdateDistanceMethodsTypes.Average);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1
        ///     with 7 Protein sequences, kimura distance matrix with hierarchical method name as Complete
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamHierarchicalClusteringStage2WithComplete()
        {
            ValidateHierarchicalClusteringStage2(Constants.HierarchicalClusteringProteinStage2WithComplete,
                                                 MoleculeType.Protein, UpdateDistanceMethodsTypes.Complete);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1
        ///     with 7 Protein sequences, kimura distance matrix with hierarchical method name as Single
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamHierarchicalClusteringStage2WithSingle()
        {
            ValidateHierarchicalClusteringStage2(Constants.HierarchicalClusteringProteinStage2WithSingle,
                                                 MoleculeType.Protein, UpdateDistanceMethodsTypes.Single);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1
        ///     with 7 Protein sequences, kimura distance matrix with hierarchical method name as WeightedMAFFT
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamHierarchicalClusteringStage2WithWeightedMafft()
        {
            ValidateHierarchicalClusteringStage2(
                Constants.HierarchicalClusteringProteinStage2WithWeightedMAFFT,
                MoleculeType.Protein, UpdateDistanceMethodsTypes.WeightedMAFFT);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1 with 7 Rna sequences and kmer distance matrix
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamHierarchicalClusteringStage1WithRnaSequences()
        {
            ValidateHierarchicalClusteringStage1(Constants.HierarchicalClusteringRnaNode,
                                                 kmerLength, MoleculeType.Protein);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1 with 7 Rna sequences and kmer distance matrix
        ///     and hierarchical clustering method name as Average
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamHierarchicalClusteringWithRnaSequencesAndAverage()
        {
            ValidateHierarchicalClusteringStage1(Constants.HierarchicalClusteringRnaNode,
                                                 kmerLength, MoleculeType.Protein, UpdateDistanceMethodsTypes.Average);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1 with 7 Rna sequences and kmer distance matrix
        ///     and hierarchical clustering method name as Single
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamHierarchicalClusteringWithRnaSequencesAndSingle()
        {
            ValidateHierarchicalClusteringStage1(Constants.HierarchicalClusteringRnaWeightedMAFFT,
                                                 kmerLength, MoleculeType.Protein, UpdateDistanceMethodsTypes.Single);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1 with 7 Rna sequences and kmer distance matrix
        ///     and hierarchical clustering method name as Complete
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamHierarchicalClusteringWithRnaSequencesAndComplete()
        {
            ValidateHierarchicalClusteringStage1(Constants.HierarchicalClusteringRnaNode,
                                                 kmerLength, MoleculeType.Protein, UpdateDistanceMethodsTypes.Complete);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1 with 7 Rna sequences and kmer distance matrix
        ///     and hierarchical clustering method name as WeightedMAFFT
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamHierarchicalClusteringWithRnaSequencesAndWeightedMafft()
        {
            ValidateHierarchicalClusteringStage1(
                Constants.HierarchicalClusteringRnaWeightedMAFFT, kmerLength,
                MoleculeType.Protein, UpdateDistanceMethodsTypes.WeightedMAFFT);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1 with 7 Rna sequences and kimura distance matrix
        ///     and stage 1 aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamHierarchicalClusteringStage2WithRnaSequences()
        {
            ValidateHierarchicalClusteringStage2(
                Constants.HierarchicalClusteringRnaStage2Node, MoleculeType.Protein);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1
        ///     using 7 Rna seqiences, kimura distance matrix with hierarchical method name as Average
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamHierarchicalClusteringStage2WithRnaSequencesAndAverage()
        {
            ValidateHierarchicalClusteringStage2(
                Constants.HierarchicalClusteringRnaStage2Node, MoleculeType.Protein,
                UpdateDistanceMethodsTypes.Average);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1
        ///     using 7 Rna seqiences, kimura distance matrix with hierarchical method name as Complete
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamHierarchicalClusteringStage2WithRnaSequencesAndComplete()
        {
            ValidateHierarchicalClusteringStage2(
                Constants.HierarchicalClusteringRnaStage2WithCompleteNode,
                MoleculeType.Protein, UpdateDistanceMethodsTypes.Complete);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1
        ///     using 7 Rna seqiences, kimura distance matrix with hierarchical method name as Single
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamHierarchicalClusteringStage2WithRnaSequencesAndSingle()
        {
            ValidateHierarchicalClusteringStage2(
                Constants.HierarchicalClusteringRnaStage2WithWeightedMAFFT,
                MoleculeType.Protein, UpdateDistanceMethodsTypes.Single);
        }

        /// <summary>
        ///     Validate HierarchicalClustering for stage1
        ///     using 7 Rna seqiences, kimura distance matrix with hierarchical method name as WeightedMAFFT
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamHierarchicalClusteringStage2WithRnaSequencesAndWeightedMafft()
        {
            ValidateHierarchicalClusteringStage2(
                Constants.HierarchicalClusteringRnaStage2WithWeightedMAFFT,
                MoleculeType.Protein, UpdateDistanceMethodsTypes.WeightedMAFFT);
        }

        #endregion

        #region BinaryTree

        /// <summary>
        ///     Validate Binary Tree nodes and edges using protein sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamBinaryTreeWithProteinSequences()
        {
            ValidateBinaryTreeNodesandEdges(Constants.MuscleProteinSequenceNode,
                                            Constants.BinaryTreeProteinNode, kmerLength, MoleculeType.Protein);
        }

        /// <summary>
        ///     Validate Binary Tree nodes and edges using Rna sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamBinaryTreeWithRnaSequences()
        {
            ValidateBinaryTreeNodesandEdges(Constants.MuscleRnaSequenceNode,
                                            Constants.BinaryTreeRnaNode, kmerLength, MoleculeType.RNA);
        }

        /// <summary>
        ///     Validate the binary tree by cutting tree at an edge
        ///     and validate the nodes of subtree using Extracting sub tree nodes.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamBinaryTreeDnaWithExtractSubTreeNodesAndCutTree()
        {
            ValidateBinaryTreeWithExtractSubTreeNodesAndCutTree(Constants.MuscleDnaSequenceNode,
                                                                Constants.BinarySubTree, 3, MoleculeType.DNA);
        }

        /// <summary>
        ///     Validate the binary tree by cutting tree at an edge
        ///     and validate the nodes of subtree using Extracting sub tree nodes.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamBinaryTreeRnaWithExtractSubTreeNodesAndCutTree()
        {
            ValidateBinaryTreeWithExtractSubTreeNodesAndCutTree(Constants.MuscleRnaSequenceNode,
                                                                Constants.BinarySubTreeRna, 3, MoleculeType.RNA);
        }

        /// <summary>
        ///     Validate the binary tree by cutting tree at an edge
        ///     and validate the nodes of subtree using Extracting sub tree nodes.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamBinaryTreeProteinWithExtractSubTreeNodesAndCutTree()
        {
            ValidateBinaryTreeWithExtractSubTreeNodesAndCutTree(Constants.MuscleProteinSequenceNode,
                                                                Constants.BinarySubTreeProtein, 3, MoleculeType.Protein);
        }

        /// <summary>
        ///     Validate the FindSmallestDifference() of binary tree by validating the node.
        /// </summary>
        public void ValidatePamsamBinaryTreeFindSmallestDifference()
        {
            ValidateBinaryTreeFindSmallestTreeDifference(Constants.BinarySmallestTreeNode,
                                                         2, MoleculeType.DNA);
        }

        /// <summary>
        ///     Compare Two trees of stage1 and stage 2.
        /// </summary>
        public void ValidatePamsamCompareBinaryTreeofStage1WithStage2()
        {
            ValidateBinaryTreeCompareTrees(Constants.BinarySubTreeNeedRealignment,
                                           kmerLength, MoleculeType.DNA);
        }

        #endregion

        #region ProfileAligner&ProgressiveAlignment

        /// <summary>
        ///     Validate ProgressiveAligner using stage1 Protein sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamProgressiveAlignerStage1WithProtein()
        {
            ValidateProgressiveAlignmentStage1(Constants.MuscleProteinSequenceWithProgresiveAlignerNodeName,
                                               MoleculeType.Protein);
        }

        /// <summary>
        ///     Validate ProgressiveAligner using stage1 Rna sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamProgressiveAlignerStage1WithRna()
        {
            ValidateProgressiveAlignmentStage1(Constants.MuscleRnaSequenceNode, MoleculeType.RNA);
        }

        /// <summary>
        ///     Validate Profile Aligner GenerateEString() method using two profiles of sub trees
        ///     by cutting the tree at edge index 4
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamProfileAlignerGenerateEStringForRna()
        {
            ValidateProfileAlignerGenerateEString(Constants.ProfileAlignerRna, MoleculeType.RNA, 4);
        }

        /// <summary>
        ///     Validate Profile Aligner GenerateEString() method using two profiles of sub trees
        ///     by cutting the tree at edge index 4
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamProfileAlignerGenerateEStringForProtein()
        {
            ValidateProfileAlignerGenerateEString(Constants.ProfileAlignerProtein,
                                                  MoleculeType.Protein, 4);
        }

        /// <summary>
        ///     Validate Profile Aligner GenerateSequencesEString() method using two profiles of sub trees
        ///     by cutting the tree at edge index 4
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamProfileAlignerGenerateSequencesEStringForDna()
        {
            ValidateProfileAlignerGenerateSequenceString(Constants.ProfileAligner, MoleculeType.DNA, 4);
        }

        /// <summary>
        ///     Validate Profile Aligner GenerateSequencesEString() method using two profiles of sub trees
        ///     by cutting the tree at edge index 4.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamProfileAlignerGenerateSequencesEStringForRna()
        {
            ValidateProfileAlignerGenerateSequenceString(Constants.ProfileAlignerRna, MoleculeType.RNA, 4);
        }

        /// <summary>
        ///     Validate Profile Aligner GenerateSequencesEString() method using two profiles of sub trees
        ///     by cutting the tree at edge index 4
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamProfileAlignerGenerateSequencesEStringForProtein()
        {
            ValidateProfileAlignerGenerateSequenceString(Constants.ProfileAlignerProtein,
                                                         MoleculeType.Protein, 4);
        }

        /// <summary>
        ///     Validate the extracted profiles of two subtrees by cutting the tree at edge index 4
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamProfileExtractionWithDna()
        {
            ValidateProfileExtraction(Constants.ProfileAligner, MoleculeType.DNA, 4);
        }

        /// <summary>
        ///     Validate the extracted profiles of two subtrees by cutting the tree at edge index 4
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamProfileExtractionWithRna()
        {
            ValidateProfileExtraction(Constants.ProfileAlignerRna, MoleculeType.RNA, 4);
        }

        /// <summary>
        ///     Validate the extracted profiles of two subtrees by cutting the tree at edge index 4
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamProfileExtractionWithProtein()
        {
            ValidateProfileExtraction(Constants.ProfileAlignerProtein, MoleculeType.Protein, 3);
        }

        /// <summary>
        ///     Validate the generated profile using GenerateProfileAlignment() using stage1 aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamGenerateProfileAlignmentWithDna()
        {
            ValidateGenerateProfileAlignmentWithSequences(Constants.ProfileAlignerWithAlignmentNode,
                                                          MoleculeType.DNA);
        }

        /// <summary>
        ///     Validate the generated profile using GenerateProfileAlignment() using stage1 aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamGenerateProfileAlignmentWithRna()
        {
            ValidateGenerateProfileAlignmentWithSequences(Constants.GenerateProfileAlignerRna,
                                                          MoleculeType.RNA);
        }

        /// <summary>
        ///     Validate the generated profile using GenerateProfileAlignment() using stage1 aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamGenerateProfileAlignmentWithProtein()
        {
            ValidateGenerateProfileAlignmentWithSequences(Constants.ProfileAlignerProtein,
                                                          MoleculeType.Protein);
        }

        /// <summary>
        ///     Validate the generated profile using GenerateProfileAlignment()
        ///     using profiles of two sub trees by cutting the tree at edge index 3
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamGenerateProfileAlignmentUsingProfilesWithDna()
        {
            ValidateGenerateProfileAlignmentWithProfiles(Constants.MultipleProfileAligner,
                                                         MoleculeType.DNA, 3);
        }

        /// <summary>
        ///     Validate the generated profile using GenerateProfileAlignment()
        ///     using profiles of two sub trees by cutting the tree at edge index 3
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamGenerateProfileAlignmentUsingProfilesWithRna()
        {
            ValidateGenerateProfileAlignmentWithProfiles(Constants.MultipleProfileAlignerRna,
                                                         MoleculeType.RNA, 3);
        }

        /// <summary>
        ///     Validate the generated profile using GenerateProfileAlignment()
        ///     using profiles of two sub trees by cutting the tree at edge index 3
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamGenerateProfileAlignmentUsingProfilesWithProtein()
        {
            ValidateGenerateProfileAlignmentWithProfiles(Constants.MultipleProfileAlignerProtein,
                                                         MoleculeType.Protein, 3);
        }

        #endregion

        #region KimuraDistanceMatrix

        /// <summary>
        ///     Validate kimura distance matrix using stage1 aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamKimuraDistanceMatrixWithDna()
        {
            ValidateKimuraDistanceMatrix(Constants.KimuraDistanceMatrix, MoleculeType.DNA);
        }

        /// <summary>
        ///     Validate kimura distance matrix using stage1 aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamKimuraDistanceMatrixWithRna()
        {
            ValidateKimuraDistanceMatrix(Constants.KimuraDistanceMatrixRna, MoleculeType.RNA);
        }

        /// <summary>
        ///     Validate kimura distance matrix using stage1 aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePamsamKimuraDistanceMatrixWithProtein()
        {
            ValidateKimuraDistanceMatrix(Constants.KimuraDistanceMatrixProtein, MoleculeType.Protein);
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
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerSerialAlignSimpleRna()
        {
            ValidateProfileAlignerAlign(
                Constants.ProfileAlignerRnaWithSimpleAlignNode,
                MoleculeType.RNA,
                Constants.SerialProcess,
                3,
                AlignType.AlignSimpleOnlyProfiles);
        }

        /// <summary>
        ///     Creates binarytree using stage2 sequences
        ///     and cut the binary tree at an random edge to get two profiles.
        ///     Create NeedlemanWunschProfileAlignerSerial instance
        ///     and execute AlignSimple(sm, gapOpenPenalty, Profile A,Profile B).
        ///     Validate the IProfileAlignment properties.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerSerialAlignSimpleWithAllParamsRna()
        {
            ValidateProfileAlignerAlign(
                Constants.ProfileAlignerRnaWithSimpleAlignNode,
                MoleculeType.RNA,
                Constants.SerialProcess,
                3,
                AlignType.AlignSimpleAllParams);
        }

        /// <summary>
        ///     Creates binarytree using stage2 sequences and
        ///     cut the binary tree at an random edge to get two profiles.
        ///     Create NeedlemanWunschProfileAlignerParallel instance and
        ///     execute Align(Profile A,Profile B).
        ///     Validate the IProfileAlignment properties.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerSerialAlignRna()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerRna,
                Constants.SerialProcess,
                ProfileScoreFunctionNames.WeightedInnerProduct,
                3,
                MoleculeType.RNA);
        }

        /// <summary>
        ///     Creates binarytree using stage2 sequences and
        ///     cut the binary tree at an random edge to get two profiles.
        ///     Create NeedlemanWunschProfileAlignerParallel instance and
        ///     execute AlignSimple(Profile A,Profile B).
        ///     Validate the IProfileAlignment properties.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerSerialAlignRnaWithAllParams()
        {
            ValidateProfileAlignerAlign(
                Constants.ProfileAlignerRnaWithSimpleAlignNode,
                MoleculeType.RNA,
                Constants.SerialProcess,
                2,
                AlignType.AlignAllParams);
        }

        #endregion

        #region NeedlemanProfileAlignerParallel

        /// <summary>
        ///     Creates binarytree using stage2 sequences and
        ///     cut the binary tree at an random edge to get two profiles.
        ///     Create NeedlemanWunschProfileAlignerParallel instance
        ///     and execute AlignSimple(Profile A,Profile B).
        ///     Validate the IProfileAlignment properties.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerParallelAlignSimpleRna()
        {
            ValidateProfileAlignerAlign(
                Constants.ProfileAlignerRnaWithSimpleAlignNode,
                MoleculeType.RNA,
                Constants.ParallelProcess,
                3,
                AlignType.AlignSimpleOnlyProfiles);
        }

        /// <summary>
        ///     Creates binarytree using stage2 sequences
        ///     and cut the binary tree at an random edge to get two profiles.
        ///     Create NeedlemanWunschProfileAlignerParallel instance
        ///     and execute AlignSimple(sm, gapOpenPenalty, Profile A,Profile B).
        ///     Validate the IProfileAlignment properties.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerParallelAlignSimpleWithAllParamsRna()
        {
            ValidateProfileAlignerAlign(
                Constants.ProfileAlignerRnaWithSimpleAlignNode,
                MoleculeType.RNA,
                Constants.ParallelProcess,
                3,
                AlignType.AlignSimpleAllParams);
        }

        /// <summary>
        ///     Creates binarytree using stage2 sequences and
        ///     cut the binary tree at an random edge to get two profiles.
        ///     Create NeedlemanWunschProfileAlignerParallel instance
        ///     and execute AlignSimple(Profile A,Profile B).
        ///     Validate the IProfileAlignment properties.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerParallelAlignSimpleProtein()
        {
            ValidateProfileAlignerAlign(
                Constants.ProfileAlignerProteinWithSimpleAlignNode,
                MoleculeType.Protein,
                Constants.ParallelProcess,
                3,
                AlignType.AlignSimpleOnlyProfiles);
        }

        /// <summary>
        ///     Creates binarytree using stage2 sequences
        ///     and cut the binary tree at an random edge to get two profiles.
        ///     Create NeedlemanWunschProfileAlignerParallel instance
        ///     and execute AlignSimple(sm, gapOpenPenalty, Profile A,Profile B).
        ///     Validate the IProfileAlignment properties.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerParallelAlignSimpleWithAllParamsProtein()
        {
            ValidateProfileAlignerAlign(
                Constants.ProfileAlignerProteinWithSimpleAlignNode,
                MoleculeType.Protein,
                Constants.ParallelProcess,
                3,
                AlignType.AlignSimpleAllParams);
        }


        /// <summary>
        ///     Creates binarytree using stage2 sequences and
        ///     cut the binary tree at an random edge to get two profiles.
        ///     Create NeedlemanWunschProfileAlignerParallel instance and
        ///     execute Align(Profile A,Profile B).
        ///     Validate the IProfileAlignment properties.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerParallelAlignRna()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerRna,
                Constants.ParallelProcess,
                ProfileScoreFunctionNames.WeightedInnerProduct,
                3,
                MoleculeType.RNA);
        }

        /// <summary>
        ///     Creates binarytree using stage2 sequences and
        ///     cut the binary tree at an random edge to get two profiles.
        ///     Create NeedlemanWunschProfileAlignerParallel instance and
        ///     execute AlignSimple(Profile A,Profile B).
        ///     Validate the IProfileAlignment properties.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerParallelAlignRnaWithAllParams()
        {
            ValidateProfileAlignerAlign(
                Constants.ProfileAlignerRnaWithSimpleAlignNode,
                MoleculeType.RNA,
                Constants.ParallelProcess,
                2,
                AlignType.AlignAllParams);
        }

        /// <summary>
        ///     Creates binarytree using stage2 sequences and
        ///     cut the binary tree at an random edge to get two profiles.
        ///     Create NeedlemanWunschProfileAlignerParallel instance and
        ///     execute AlignSimple(Profile A,Profile B).
        ///     Validate the IProfileAlignment properties.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerParallelAlignProteinWithAllParams()
        {
            ValidateProfileAlignerAlign(
                Constants.ProfileAlignerProteinWithSimpleAlignNode,
                MoleculeType.Protein,
                Constants.ParallelProcess,
                2,
                AlignType.AlignAllParams);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "WeightedInnerProduct".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerParallelWithWeightedInnerProduct()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithWeightedInnerProductCachedNode,
                Constants.ParallelProcess,
                ProfileScoreFunctionNames.WeightedInnerProduct,
                5, MoleculeType.DNA);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "WeightedInnerProductShiftedFast".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerParallelWithWeightedInnerProductShiftedFast()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithWeightedInnerProductCachedNode,
                Constants.ParallelProcess,
                ProfileScoreFunctionNames.WeightedInnerProductShiftedFast,
                5, MoleculeType.DNA);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "WeightedEuclideanDistanceFast".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerParallelWithWeightedEuclideanDistanceFast()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithWeightedEuclideanDistanceFastNode,
                Constants.ParallelProcess,
                ProfileScoreFunctionNames.WeightedEuclideanDistanceFast,
                5, MoleculeType.DNA);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "WeightedEuclideanDistance".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerParallelWithWeightedEuclideanDistance()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithWeightedEuclideanDistanceFastNode,
                Constants.ParallelProcess,
                ProfileScoreFunctionNames.WeightedEuclideanDistance,
                5, MoleculeType.DNA);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "PearsonCorrelation".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerParallelWithPearsonCorrelation()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithWeightedInnerProductCachedNode,
                Constants.ParallelProcess,
                ProfileScoreFunctionNames.PearsonCorrelation,
                5, MoleculeType.DNA);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "LogExponentialInnerProductShiftedFast".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerParallelWithLogExponentialInnerProductShiftedFast()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithLogExponentialInnerProductFastNode,
                Constants.ParallelProcess,
                ProfileScoreFunctionNames.LogExponentialInnerProductShiftedFast,
                5, MoleculeType.DNA);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "LogExponentialInnerProductShifted".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerParallelWithLogExponentialInnerProductShifted()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithWeightedEuclideanDistanceFastNode,
                Constants.ParallelProcess,
                ProfileScoreFunctionNames.LogExponentialInnerProductShifted,
                5, MoleculeType.DNA);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "LogExponentialInnerProductFast".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerParallelWithLogExponentialInnerProductFast()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithLogExponentialInnerProductFastNode,
                Constants.ParallelProcess,
                ProfileScoreFunctionNames.LogExponentialInnerProductFast,
                5, MoleculeType.DNA);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "LogExponentialInnerProduct".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerParallelWithLogExponentialInnerProduct()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithWeightedInnerProductCachedNode,
                Constants.ParallelProcess,
                ProfileScoreFunctionNames.LogExponentialInnerProduct,
                5, MoleculeType.DNA);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "JensenShannonDivergence".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerParallelWithJensenShannonDivergence()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithWeightedInnerProductCachedNode,
                Constants.ParallelProcess,
                ProfileScoreFunctionNames.JensenShannonDivergence,
                5, MoleculeType.DNA);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "InnerProductFast".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerParallelWithInnerProductFast()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithWeightedInnerProductCachedNode,
                Constants.ParallelProcess,
                ProfileScoreFunctionNames.InnerProductFast,
                5, MoleculeType.DNA);
        }

        /// <summary>
        ///     Creates NeedlemanWunschProfileAlignerParallel instance with
        ///     profilescorefunction name as "InnerProduct".
        ///     Execute Align() method and Validate IProfileAlignment
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNWProfileAlignerParallelWithInnerProduct()
        {
            ValidateProfileAlignerAlignWithProfileFunctionScore(
                Constants.ProfileAlignerWithWeightedInnerProductCachedNode,
                Constants.ParallelProcess,
                ProfileScoreFunctionNames.InnerProduct,
                5, MoleculeType.DNA);
        }

        #endregion

        #region MsaUtils

        /// <summary>
        ///     Validate the QScore of 12 Pamsam aligned Rna sequences against benchmark sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateAlignmentQualityScoreWithRnaSequences()
        {
            ValidateAlignmentScore(Constants.RnaWith12SequencesNode,
                                   MoleculeType.RNA, ScoreType.QScore);
        }

        /// <summary>
        ///     Validate the TCScore of 12 Pamsam aligned Rna sequences against benchmark sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateAlignmentTCScoreWithRnaSequences()
        {
            ValidateAlignmentScore(Constants.RnaWith12SequencesNode,
                                   MoleculeType.RNA, ScoreType.TCScore);
        }

        /// <summary>
        ///     Execute the CalculateOffset().
        ///     Validate the number of residues whose position index will be negative
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateAlignmentOffsetWithRnaSequences()
        {
            ValidateAlignmentScore(Constants.RnaWith12SequencesNode,
                                   MoleculeType.RNA, ScoreType.Offset);
        }

        /// <summary>
        ///     Validate the Multiple alignment score of Pamsam aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateMultipleAlignmentScoreWithRnaSequences()
        {
            ValidateAlignmentScore(Constants.RnaWith12SequencesNode,
                                   MoleculeType.RNA, ScoreType.MultipleAlignmentScoreFunction);
        }

        /// <summary>
        ///     Validate the pairwise score function of a pair of aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePairWiseScoreFunctionWithRnaSequences()
        {
            ValidateAlignmentScore(Constants.RnaWith12SequencesNode,
                                   MoleculeType.RNA, ScoreType.PairWiseScoreFunction);
        }

        /// <summary>
        ///     Get two profiles after cutting the edge of binary tree.
        ///     Validate the correlation value of two profiles.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateCorrelationWithRnaSequences()
        {
            ValidateFunctionCalculations(Constants.RnaFunctionsNode,
                                         MoleculeType.RNA, 4, FunctionType.Correlation);
        }

        /// <summary>
        ///     Get profiles after cutting the edge of binary tree.
        ///     Validate the max index value of a profile.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateFindMaxIndexWithRnaSequences()
        {
            ValidateFunctionCalculations(Constants.RnaFunctionsNode,
                                         MoleculeType.RNA, 4, FunctionType.FindMaxIndex);
        }

        /// <summary>
        ///     Get profiles after cutting the edge of binary tree.
        ///     Validate the JensenShanonDivergence value of two profiles.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateJensenShanonDivergenceWithRnaSequences()
        {
            ValidateFunctionCalculations(Constants.RnaFunctionsNode,
                                         MoleculeType.RNA, 4, FunctionType.JensenShanonDivergence);
        }

        /// <summary>
        ///     Get profiles after cutting the edge of binary tree.
        ///     Validate the KullbackLeiblerDistance of two profiles.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateKullbackLeiblerDistanceWithRnaSequences()
        {
            ValidateFunctionCalculations(Constants.RnaFunctionsNode,
                                         MoleculeType.RNA, 4, FunctionType.KullbackLeiblerDistance);
        }

        /// <summary>
        ///     Get pam sam aligned sequences. Execute UnAlign() method
        ///     and verify that it does not contains gap
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateUNAlignWithRnaSequences()
        {
            ValidateUNAlign(Constants.RnaWith12SequencesNode, MoleculeType.RNA);
        }

        // <summary>
        /// Validate the QScore of 12 Pamsam aligned Protein sequences against benchmark sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateAlignmentQualityScoreWithProteinSequences()
        {
            ValidateAlignmentScore(Constants.ProteinWith12SequencesNode,
                                   MoleculeType.Protein, ScoreType.QScore);
        }

        /// <summary>
        ///     Validate the TCScore of 12 Pamsam aligned Protein sequences against benchmark sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateAlignmentTCScoreWithProteinSequences()
        {
            ValidateAlignmentScore(Constants.ProteinWith12SequencesNode,
                                   MoleculeType.Protein, ScoreType.TCScore);
        }

        /// <summary>
        ///     Execute the CalculateOffset().
        ///     Validate the number of residues whose position index will be negative
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateAlignmentOffsetWithProteinSequences()
        {
            ValidateAlignmentScore(Constants.ProteinWith12SequencesNode,
                                   MoleculeType.Protein, ScoreType.Offset);
        }

        /// <summary>
        ///     Validate the Multiple alignment score of Pamsam aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateMultipleAlignmentScoreWithProteinSequences()
        {
            ValidateAlignmentScore(Constants.ProteinWith12SequencesNode,
                                   MoleculeType.Protein, ScoreType.MultipleAlignmentScoreFunction);
        }

        /// <summary>
        ///     Validate the pairwise score function of a pair of aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePairWiseScoreFunctionWithProteinSequences()
        {
            ValidateAlignmentScore(Constants.ProteinWith12SequencesNode,
                                   MoleculeType.Protein, ScoreType.PairWiseScoreFunction);
        }

        /// <summary>
        ///     Get two profiles after cutting the edge of binary tree.
        ///     Validate the correlation value of two profiles.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateCorrelationWithProteinSequences()
        {
            ValidateFunctionCalculations(Constants.ProteinFunctionsNode,
                                         MoleculeType.Protein, 4, FunctionType.Correlation);
        }

        /// <summary>
        ///     Get profiles after cutting the edge of binary tree.
        ///     Validate the max index value of a profile.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateFindMaxIndexWithProteinSequences()
        {
            ValidateFunctionCalculations(Constants.ProteinFunctionsNode,
                                         MoleculeType.Protein, 4, FunctionType.FindMaxIndex);
        }

        /// <summary>
        ///     Get profiles after cutting the edge of binary tree.
        ///     Validate the JensenShanonDivergence value of two profiles.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateJensenShanonDivergenceWithProteinSequences()
        {
            ValidateFunctionCalculations(Constants.ProteinFunctionsNode,
                                         MoleculeType.Protein, 4, FunctionType.JensenShanonDivergence);
        }

        /// <summary>
        ///     Get profiles after cutting the edge of binary tree.
        ///     Validate the KullbackLeiblerDistance of two profiles.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateKullbackLeiblerDistanceWithProteinSequences()
        {
            ValidateFunctionCalculations(Constants.ProteinFunctionsNode,
                                         MoleculeType.Protein, 4, FunctionType.KullbackLeiblerDistance);
        }

        #endregion

        #endregion

        #region Helper Methods

        /// <summary>
        ///     Read from xml config and initialize all member variables
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="expectedScoreNode">Expected score xml node</param>
        private void Initialize(string nodeName, string expectedScoreNode)
        {
            // Read all the input sequences from xml config file
            IAlphabet alphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                     Constants.AlphabetNameNode));
            if (alphabet == Alphabets.DNA)
            {
                similarityMatrix = new SimilarityMatrix(
                    SimilarityMatrix.StandardSimilarityMatrix.AmbiguousDna);
            }
            else if (alphabet == Alphabets.RNA)
            {
                similarityMatrix = new SimilarityMatrix(
                    SimilarityMatrix.StandardSimilarityMatrix.AmbiguousRna);
            }
            else if (alphabet == Alphabets.Protein)
            {
                similarityMatrix = new SimilarityMatrix(
                    SimilarityMatrix.StandardSimilarityMatrix.Blosum50);
            }

            string sequenceString1 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Sequence1);
            string sequenceString2 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Sequence2);
            string sequenceString3 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Sequence3);
            string sequenceString4 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Sequence4);
            string sequenceString5 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Sequence5);
            string sequenceString6 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Sequence6);
            string sequenceString7 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Sequence7);
            string sequenceString8 = string.Empty;

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

            // Add all sequences to list.
            lstSequences.Add(seq1);
            lstSequences.Add(seq2);
            lstSequences.Add(seq3);
            lstSequences.Add(seq4);
            lstSequences.Add(seq5);
            lstSequences.Add(seq6);
            lstSequences.Add(seq7);

            // Read all expected Sequences
            sequenceString1 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode1);
            sequenceString2 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode2);
            sequenceString3 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode3);
            sequenceString4 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode4);
            sequenceString5 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode5);
            sequenceString6 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode6);
            sequenceString7 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode7);
            sequenceString8 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode8);

            // Get all expected sequences object
            seq1 = new Sequence(alphabet, sequenceString1);
            seq2 = new Sequence(alphabet, sequenceString2);
            seq3 = new Sequence(alphabet, sequenceString3);
            seq4 = new Sequence(alphabet, sequenceString4);
            seq5 = new Sequence(alphabet, sequenceString5);
            seq6 = new Sequence(alphabet, sequenceString6);
            seq7 = new Sequence(alphabet, sequenceString7);
            seq8 = new Sequence(alphabet, sequenceString8);

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

            profileAligner = new NeedlemanWunschProfileAlignerParallel(
                similarityMatrix, ProfileScoreFunctionNames.WeightedInnerProduct,
                gapOpenPenalty, gapExtendPenalty, Environment.ProcessorCount);

            expectedScore = utilityObj.xmlUtil.GetTextValue(nodeName, expectedScoreNode);

            // Parallel Option will only get set if the PAMSAMMultipleSequenceAligner is getting called
            // To test separately distance matrix, binary tree etc.. 
            // Set the parallel option using below ctor.
            var msa = new PAMSAMMultipleSequenceAligner(
                lstSequences,
                kmerLength, DistanceFunctionTypes.EuclideanDistance,
                UpdateDistanceMethodsTypes.Average,
                ProfileAlignerNames.NeedlemanWunschProfileAligner,
                ProfileScoreFunctionNames.InnerProduct, similarityMatrix,
                gapOpenPenalty, gapExtendPenalty, 2, 2);

            if (null != msa)
            {
                ApplicationLog.WriteLine(String.Format(null,
                                                       "Initialization of all variables successfully completed for xml node {0}",
                                                       nodeName));
            }
        }

        /// <summary>
        ///     Read from xml config and initialize all stage 1 member variables
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        private void InitializeStage1Variables(string nodeName)
        {
            // Read all the input sequences from xml config file
            IAlphabet alphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                     Constants.AlphabetNameNode));
            // Read all expected Sequences for stage1 
            string sequenceString1 = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                     Constants.Stage1ExpectedSequenceNode1);
            string sequenceString2 = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                     Constants.Stage1ExpectedSequenceNode2);
            string sequenceString3 = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                     Constants.Stage1ExpectedSequenceNode3);
            string sequenceString4 = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                     Constants.Stage1ExpectedSequenceNode4);
            string sequenceString5 = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                     Constants.Stage1ExpectedSequenceNode5);
            string sequenceString6 = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                     Constants.Stage1ExpectedSequenceNode6);
            string sequenceString7 = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                     Constants.Stage1ExpectedSequenceNode7);

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

            stage1ExpectedScore = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                  Constants.Stage1ExpectedScoreNode);

            ApplicationLog.WriteLine(String.Format(null,
                                                   "PamsamP1Test:: Initialization of stage1 variables successfully completed for xml node {0}",
                                                   nodeName));
        }

        /// <summary>
        ///     Read from xml config and initialize all stage 2 member variables
        /// </summary>
        /// <param name="nodeName">xml node name</param>
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
                                                   "PamsamP1Test:: Initialization of stage2 variables successfully completed for xml node {0}",
                                                   nodeName));
        }

        /// <summary>
        ///     Read from xml config and initialize all stage 3 member variables
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        private void InitializeStage3Variables(string nodeName)
        {
            // Read all the input sequences from xml config file
            IAlphabet alphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                     Constants.AlphabetNameNode));

            // Read all expected Sequences for stage1 
            string sequenceString1 = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                     Constants.Stage3ExpectedSequenceNode1);
            string sequenceString2 = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                     Constants.Stage3ExpectedSequenceNode2);
            string sequenceString3 = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                     Constants.Stage3ExpectedSequenceNode3);
            string sequenceString4 = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                     Constants.Stage3ExpectedSequenceNode4);
            string sequenceString5 = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                     Constants.Stage3ExpectedSequenceNode5);
            string sequenceString6 = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                     Constants.Stage3ExpectedSequenceNode6);
            string sequenceString7 = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                     Constants.Stage3ExpectedSequenceNode7);

            // Get all expected stage1 sequences object
            ISequence seq1 = new Sequence(alphabet, sequenceString1);
            ISequence seq2 = new Sequence(alphabet, sequenceString2);
            ISequence seq3 = new Sequence(alphabet, sequenceString3);
            ISequence seq4 = new Sequence(alphabet, sequenceString4);
            ISequence seq5 = new Sequence(alphabet, sequenceString5);
            ISequence seq6 = new Sequence(alphabet, sequenceString6);
            ISequence seq7 = new Sequence(alphabet, sequenceString7);

            stage3ExpectedSequences = new List<ISequence>();
            stage3ExpectedSequences.Add(seq1);
            stage3ExpectedSequences.Add(seq2);
            stage3ExpectedSequences.Add(seq3);
            stage3ExpectedSequences.Add(seq4);
            stage3ExpectedSequences.Add(seq5);
            stage3ExpectedSequences.Add(seq6);
            stage3ExpectedSequences.Add(seq7);

            stage3ExpectedScore = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                  Constants.Stage3ExpectedScoreNode);
            ApplicationLog.WriteLine(String.Format(null,
                                                   "PamsamP1Test:: Initialization of stage3 variables successfully completed for xml node {0}",
                                                   nodeName));
        }

        /// <summary>
        ///     Add 3 more one line sequences and sequences count is 10.
        /// </summary>
        /// <param name="nodeName"></param>
        private void AddOneLineSequences(string nodeName)
        {
            // Read all the input sequences from xml config file
            IAlphabet alphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                     Constants.AlphabetNameNode));

            string sequenceString8 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Sequence8);
            string sequenceString9 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Sequence9);
            string sequenceString10 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Sequence10);

            ISequence seq8 = new Sequence(alphabet, sequenceString8);
            ISequence seq9 = new Sequence(alphabet, sequenceString9);
            ISequence seq10 = new Sequence(alphabet, sequenceString10);

            lstSequences.Add(seq8);
            lstSequences.Add(seq9);
            lstSequences.Add(seq10);

            sequenceString8 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode8);
            sequenceString9 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode9);
            sequenceString10 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode10);

            seq8 = new Sequence(alphabet, sequenceString8);
            seq9 = new Sequence(alphabet, sequenceString9);
            seq10 = new Sequence(alphabet, sequenceString10);

            expectedSequences.Add(seq8);
            expectedSequences.Add(seq9);
            expectedSequences.Add(seq10);
        }

        /// <summary>
        ///     Validate Muscle multiple sequence alignment with different profiler and hierarchical clustering method name.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="moleculeType">molecule type of sequences</param>
        /// <param name="expectedScoreNode">Expected score node</param>
        /// <param name="hierarchicalClusteringMethodName">hierarchical clustering method name</param>
        /// <param name="profileName">SW/NW profiler</param>
        private void ValidatePamsamAlignWithUpdateDistanceMethodTypes(string nodeName,
                                                                      MoleculeType moleculeType,
                                                                      string expectedScoreNode,
                                                                      UpdateDistanceMethodsTypes
                                                                          hierarchicalClusteringMethodName,
                                                                      ProfileAlignerNames profileName)
        {
            ValidatePamsamAlign(nodeName, moleculeType, expectedScoreNode,
                                hierarchicalClusteringMethodName, DistanceFunctionTypes.EuclideanDistance,
                                profileName, ProfileScoreFunctionNames.WeightedInnerProduct, kmerLength,
                                false, false);
            ApplicationLog.WriteLine(
                String.Format(null,
                              "PamsamP1Test:: Pamsam alignment validation completed successfully for {0} moleculetype with different hierarchical clustering method name {1}",
                              moleculeType.ToString(),
                              hierarchicalClusteringMethodName.ToString()));
        }

        /// <summary>
        ///     Validate Muscle multiple sequence alignment with different profiler and distance matrix method name.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="moleculeType">molecule type of sequences</param>
        /// <param name="expectedScoreNode">expected score xml node</param>
        /// <param name="distanceFunctionName">Distance function name</param>
        /// <param name="distanceMatrixName">kmerdistancematrix method name.</param>
        /// <param name="profileName">SW/NW profiler</param>
        private void ValidatePamsamAlignWithDistanceFunctionaTypes(string nodeName,
                                                                   MoleculeType moleculeType, string expectedScoreNode,
                                                                   DistanceFunctionTypes distanceFunctionName,
                                                                   ProfileAlignerNames profileName)
        {
            ValidatePamsamAlign(nodeName, moleculeType, expectedScoreNode,
                                UpdateDistanceMethodsTypes.Average, distanceFunctionName,
                                profileName, ProfileScoreFunctionNames.WeightedInnerProduct, kmerLength,
                                false, false);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamP1Test:: Pamsam alignment validation completed successfully for {0} 
                moleculetype with different kmer distance method name {1}",
                                                   moleculeType.ToString(), distanceFunctionName.ToString()));
        }

        /// <summary>
        ///     Validate Muscle multiple sequence alignment with different
        ///     profiler and profile score function name.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="moleculeType">molecule type of sequences</param>
        /// <param name="expectedScoreNode">expected score xml node</param>
        /// <param name="profileName">SW/NW profiler</param>
        /// <param name="profileScoreFunctionName">Profile score function name</param>
        private void ValidatePamsamAlignWithProfileScoreFunctionName(string nodeName,
                                                                     MoleculeType moleculeType, string expectedScoreNode,
                                                                     ProfileAlignerNames profileName,
                                                                     ProfileScoreFunctionNames profileScoreFunctionName)
        {
            ValidatePamsamAlign(nodeName, moleculeType, expectedScoreNode,
                                UpdateDistanceMethodsTypes.Average,
                                DistanceFunctionTypes.EuclideanDistance, profileName,
                                profileScoreFunctionName, kmerLength,
                                false, false);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamP1Test:: Pamsam alignment validation completed successfully for {0} 
                moleculetype with different profile score function name {1}",
                                                   moleculeType.ToString(), profileScoreFunctionName.ToString()));
        }

        /// <summary>
        ///     Validate Muscle multiple sequence alignment with default values.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="moleculeType">Molecule Type.</param>
        /// <param name="expectedScoreNode">expected score xml node</param>
        /// <param name="profileName">Profile name</param>
        private void ValidatePamsamAlign(string nodeName, MoleculeType moleculeType,
                                         string expectedScoreNode, ProfileAlignerNames profileName)
        {
            ValidatePamsamAlign(nodeName, moleculeType, expectedScoreNode,
                                UpdateDistanceMethodsTypes.Average,
                                DistanceFunctionTypes.EuclideanDistance, profileName,
                                ProfileScoreFunctionNames.WeightedInnerProduct, kmerLength,
                                false, false);

            ApplicationLog.WriteLine(String.Format(null,
                                                   "PamsamP1Test:: Pamsam alignment validation completed successfully for {0} moleculetype with all default params",
                                                   moleculeType.ToString()));
        }

        /// <summary>
        ///     Validate Muscle multiple sequence alignment with default values.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="moleculeType">Molecule Type.</param>
        /// <param name="expectedScoreNode">expected score xml node</param>
        /// <param name="profileName">Profile name</param>
        private void ValidatePamsamAlignOneLineSequences(string nodeName,
                                                         MoleculeType moleculeType, string expectedScoreNode,
                                                         ProfileAlignerNames profileName)
        {
            // Use different kmerlength = 3 for one line sequences
            ValidatePamsamAlign(nodeName, moleculeType, expectedScoreNode,
                                UpdateDistanceMethodsTypes.Average,
                                DistanceFunctionTypes.EuclideanDistance, profileName,
                                ProfileScoreFunctionNames.WeightedInnerProduct, 3,
                                true, false);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamP1Test:: Pamsam alignment validation completed successfully with one line sequences
                for {0} moleculetype with all default params",
                                                   moleculeType.ToString()));
        }

        /// <summary>
        ///     Validate Muscle multiple sequence alignment.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="moleculeType">molecule type</param>
        /// <param name="expectedScoreNode">Expected Score Node</param>
        /// <param name="hierarchicalClusteringMethodName">hierarchical clustering method name</param>
        /// <param name="distanceFunctionName">kmerdistancematrix method name.</param>
        /// <param name="profileAlignerName">SW/NW profiler</param>
        /// <param name="profileScoreName">Profile score function name.</param>
        /// <param name="kmrlength">Kmer length</param>
        /// <param name="addOnelineSequences">Add one line sequence?</param>
        /// <param name="IsAlignForMoreSeq">Is Alignment for multiple sequences</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "moleculeType")]
        private void ValidatePamsamAlign(
            string nodeName, MoleculeType moleculeType, string expectedScoreNode,
            UpdateDistanceMethodsTypes hierarchicalClusteringMethodName,
            DistanceFunctionTypes distanceFunctionName,
            ProfileAlignerNames profileAlignerName,
            ProfileScoreFunctionNames profileScoreName, int kmrlength,
            bool addOnelineSequences, bool IsAlignForMoreSeq)
        {
            Initialize(nodeName, expectedScoreNode);
            if (addOnelineSequences)
            {
                AddOneLineSequences(nodeName);
            }

            // MSA aligned sequences.
            var msa = new PAMSAMMultipleSequenceAligner(lstSequences,
                                                        kmrlength, distanceFunctionName,
                                                        hierarchicalClusteringMethodName,
                                                        profileAlignerName, profileScoreName, similarityMatrix,
                                                        gapOpenPenalty,
                                                        gapExtendPenalty, 2, 2);

            // Validate the aligned Sequence and score
            int index = 0;
            foreach (ISequence seq in msa.AlignedSequences)
            {
                if (IsAlignForMoreSeq)
                {
                    Assert.IsTrue(expectedSequences.Contains(seq));
                    index++;
                }
            }

            Assert.IsTrue(expectedScore.Contains(msa.AlignmentScore.ToString((IFormatProvider) null)));
        }

        /// <summary>
        ///     Validate Muscle multiple sequence alignment with gap open cost and penalty.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="moleculeType">molecule type</param>
        /// <param name="expectedScoreNode">Expected score node</param>
        /// <param name="hierarchicalClusteringMethodName">hierarchical clustering method name</param>
        /// <param name="distanceFunctionName">kmerdistancematrix method name.</param>
        /// <param name="profileAlignerName">SW/NW profiler</param>
        /// <param name="profileScoreName">Profile score function name.</param>
        /// <param name="gpOpenPenalty">Gap open penalty</param>
        /// <param name="gpExtendPenalty">Gap extended penalty</param>
        /// <param name="IsAlignedLargeSeq">True for large sequence else false</param>
        private void ValidatePamsamAlignWithGapCost(
            string nodeName, MoleculeType moleculeType, string expectedScoreNode,
            UpdateDistanceMethodsTypes hierarchicalClusteringMethodName,
            DistanceFunctionTypes distanceFunctionName,
            ProfileAlignerNames profileAlignerName,
            ProfileScoreFunctionNames profileScoreName,
            int gpOpenPenalty, int gpExtendPenalty, bool IsAlignedLargeSeq)
        {
            Initialize(nodeName, expectedScoreNode);

            // MSA aligned sequences with sepcified gap costs.
            var msa = new PAMSAMMultipleSequenceAligner(lstSequences,
                                                        kmerLength, distanceFunctionName,
                                                        hierarchicalClusteringMethodName,
                                                        profileAlignerName, profileScoreName, similarityMatrix,
                                                        gpOpenPenalty,
                                                        gpExtendPenalty, 2, 2);

            // Validate the aligned Sequence and score
            int index = 0;
            foreach (ISequence seq in msa.AlignedSequences)
            {
                if (IsAlignedLargeSeq)
                {
                    Assert.AreEqual(new string(seq.Select(a => (char) a).ToArray()),
                                    new string(expectedSequences[index].Select(a => (char) a).ToArray()));
                    index++;
                }
            }

            Assert.IsTrue(expectedScore.Contains(msa.AlignmentScore.ToString((IFormatProvider) null)));
            ApplicationLog.WriteLine(String.Format(null,
                                                   "PamsamP1Test:: Pamsam alignment completed successfully with equal gap cost for {0} moleculetype with all default params",
                                                   moleculeType.ToString()));
        }

        /// <summary>
        ///     Validate Stage 1 aligned sequences and score of Muscle multiple sequence alignment.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="moleculeType">molecule type</param>
        /// <param name="expectedScoreNode">Expected score node</param>
        /// <param name="hierarchicalClusteringMethodName">hierarchical clustering method name</param>
        /// <param name="distanceFunctionName">kmerdistancematrix method name.</param>
        /// <param name="profileAlignerName">SW/NW profiler</param>
        /// <param name="profileScoreName">Profile score function name.</param>
        /// <param name="IsAlignStage1">True for Alignment stage1 validation</param>
        private void ValidatePamsamAlignStage1(string nodeName, MoleculeType moleculeType,
                                               string expectedScoreNode,
                                               UpdateDistanceMethodsTypes hierarchicalClusteringMethodName,
                                               DistanceFunctionTypes distanceFunctionName,
                                               ProfileAlignerNames profileAlignerName,
                                               ProfileScoreFunctionNames profileScoreName,
                                               bool IsAlignStage1)
        {
            Initialize(nodeName, expectedScoreNode);
            InitializeStage1Variables(nodeName);

            // MSA aligned sequences.
            var msa =
                new PAMSAMMultipleSequenceAligner(lstSequences, kmerLength,
                                                  distanceFunctionName, hierarchicalClusteringMethodName,
                                                  profileAlignerName, profileScoreName, similarityMatrix,
                                                  gapOpenPenalty, gapExtendPenalty, 2, 2);

            // Validate the aligned Sequence and score of stage1
            Assert.AreEqual(stage1ExpectedSequences.Count, msa.AlignedSequences.Count);
            int index = 0;
            foreach (ISequence seq in msa.AlignedSequencesA)
            {
                if (IsAlignStage1)
                {
                    Assert.AreEqual(new string(stage1ExpectedSequences[index].Select(a => (char) a).ToArray()),
                                    new string(seq.Select(a => (char) a).ToArray()));
                    index++;
                }
            }

            Assert.IsTrue(stage1ExpectedScore.Contains(
                msa.AlignmentScoreA.ToString((IFormatProvider) null)));

            ApplicationLog.WriteLine(String.Format(null,
                                                   "PamsamP1Test:: Pamsam  stage1 alignment completed successfully for {0} moleculetype with all default params",
                                                   moleculeType.ToString()));
        }

        /// <summary>
        ///     Validate Stage 2 aligned sequences and score of Muscle multiple sequence alignment.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="moleculeType">molecule type</param>
        /// <param name="expectedScoreNode">Expected score node</param>
        /// <param name="hierarchicalClusteringMethodName">hierarchical clustering method name</param>
        /// <param name="distanceFunctionName">kmerdistancematrix method name.</param>
        /// <param name="profileAlignerName">SW/NW profiler</param>
        /// <param name="profileScoreName">Profile score function name.</param>
        private void ValidatePamsamAlignStage2(
            string nodeName,
            MoleculeType moleculeType,
            string expectedScoreNode,
            UpdateDistanceMethodsTypes hierarchicalClusteringMethodName,
            DistanceFunctionTypes distanceFunctionName,
            ProfileAlignerNames profileAlignerName,
            ProfileScoreFunctionNames profileScoreName)
        {
            Initialize(nodeName, expectedScoreNode);
            InitializeStage2Variables(nodeName);

            // MSA aligned sequences.
            var msa = new PAMSAMMultipleSequenceAligner(lstSequences,
                                                        kmerLength, distanceFunctionName,
                                                        hierarchicalClusteringMethodName,
                                                        profileAlignerName, profileScoreName, similarityMatrix,
                                                        gapOpenPenalty, gapExtendPenalty, 2, 2);

            if (null != msa.AlignedSequencesB)
            {
                // Validate the aligned Sequence and score of stage2
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
                                                   "PamsamP1Test:: Pamsam  stage2 alignment completed successfully for {0} moleculetype with all default params",
                                                   moleculeType.ToString()));
        }

        /// <summary>
        ///     Validate Stage 3 aligned sequences and score of Muscle multiple sequence alignment.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="moleculeType">molecule type</param>
        /// <param name="expectedScoreNode">Expected score node</param>
        /// <param name="hierarchicalClusteringMethodName">hierarchical clustering method name</param>
        /// <param name="distanceFunctionName">kmerdistancematrix method name.</param>
        /// <param name="profileAlignerName">SW/NW profiler</param>
        /// <param name="profileScoreName">Profile score function name.</param>
        /// <param name="IsStageAlignment">True for release stage3 validations</param>
        private void ValidatePamsamAlignStage3(string nodeName, MoleculeType moleculeType,
                                               string expectedScoreNode,
                                               UpdateDistanceMethodsTypes hierarchicalClusteringMethodName,
                                               DistanceFunctionTypes distanceFunctionName,
                                               ProfileAlignerNames profileAlignerName,
                                               ProfileScoreFunctionNames profileScoreName,
                                               bool IsStageAlignment)
        {
            Initialize(nodeName, expectedScoreNode);
            InitializeStage3Variables(nodeName);

            // MSA aligned sequences.
            var msa = new PAMSAMMultipleSequenceAligner(lstSequences,
                                                        kmerLength, distanceFunctionName,
                                                        hierarchicalClusteringMethodName,
                                                        profileAlignerName, profileScoreName, similarityMatrix,
                                                        gapOpenPenalty, gapExtendPenalty, 2, 2);

            // Validate the aligned Sequence and score of stage2
            Assert.AreEqual(stage3ExpectedSequences.Count, msa.AlignedSequences.Count);
            int index = 0;
            foreach (ISequence seq in msa.AlignedSequencesC)
            {
                if (IsStageAlignment)
                {
                    Assert.AreEqual(new string(stage3ExpectedSequences[index].Select(a => (char) a).ToArray()),
                                    new string(seq.Select(a => (char) a).ToArray()));
                    index++;
                }
            }
            Assert.IsTrue(stage3ExpectedScore.Contains(msa.AlignmentScoreC.ToString((IFormatProvider) null)));

            ApplicationLog.WriteLine(String.Format(null,
                                                   "PamsamP1Test:: Pamsam  stage3 alignment completed successfully for {0} moleculetype with all default params",
                                                   moleculeType.ToString()));
        }

        /// <summary>
        ///     Validate Distance Matrix with default distancefunctionname
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="kmrlength">kmr length</param>
        /// <param name="moleculeType">molecule type.</param>
        private void ValidateKmerDistanceMatrixStage1(string nodeName,
                                                      int kmrlength, MoleculeType moleculeType)
        {
            switch (moleculeType)
            {
                case MoleculeType.DNA:
                    Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
                    break;
                case MoleculeType.Protein:
                    Initialize(Constants.MuscleProteinSequenceNode, Constants.ExpectedScoreNode);
                    break;
                case MoleculeType.RNA:
                    Initialize(Constants.MuscleRnaSequenceNode, Constants.ExpectedScoreNode);
                    break;
                default:
                    break;
            }


            // Get the kmer distance matrix using default params
            IDistanceMatrix matrix = GetKmerDistanceMatrix(kmrlength);

            // Validate the matrix
            ValidateDistanceMatrix(nodeName, matrix);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamP1Test:: kmer distance matrix generation and validation completed success for {0} 
                    moleculetype with default params",
                                                   moleculeType.ToString()));
        }

        /// <summary>
        ///     Validate DistanceMatrix at stage1 using different DistanceFunction names.
        /// </summary>
        /// <param name="nodeName">Xml Node Name</param>
        /// <param name="kmrlength">Kmer length</param>
        /// <param name="moleculeType">Molecule type</param>
        /// <param name="distanceFunctionName">Distance function name</param>
        private void ValidateKmerDistanceMatrixStage1(string nodeName, int kmrlength, MoleculeType moleculeType,
                                                      DistanceFunctionTypes distanceFunctionName)
        {
            switch (moleculeType)
            {
                case MoleculeType.DNA:
                    Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
                    break;
                case MoleculeType.Protein:
                    Initialize(Constants.MuscleProteinSequenceNode, Constants.ExpectedScoreNode);
                    break;
                case MoleculeType.RNA:
                    Initialize(Constants.MuscleRnaSequenceNode, Constants.ExpectedScoreNode);
                    break;
                default:
                    break;
            }


            // Get the kmer distance matrix using default params
            IDistanceMatrix matrix = GetKmerDistanceMatrix(kmrlength, moleculeType, distanceFunctionName);

            // Validate the matrix
            ValidateDistanceMatrix(nodeName, matrix);
            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamP1Test:: kmer distance matrix generation and validation completed success for {0} 
                    moleculetype with different distance method name {1}",
                                                   moleculeType.ToString(),
                                                   distanceFunctionName.ToString()));
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
            string expectedMinimumValue = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                          Constants.MinimumValue);
            string expectedNearestDistances = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                              Constants.NearestDistances);

            // Validate values in distance matrix
            Assert.AreEqual(expectedDimension, matrix.Dimension.ToString((IFormatProvider) null));
            Assert.AreEqual(expectedMinimumValue, matrix.MinimumValue.ToString((IFormatProvider) null));
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
        /// <param name="moleculeType">molecule type of input sequences</param>
        /// <param name="hierarchicalMethodName">hierarchical method name.</param>
        private void ValidateHierarchicalClusteringStage1(string nodeName,
                                                          int kmrlength, MoleculeType moleculeType,
                                                          UpdateDistanceMethodsTypes hierarchicalMethodName)
        {
            switch (moleculeType)
            {
                case MoleculeType.DNA:
                    Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
                    break;
                case MoleculeType.Protein:
                    Initialize(Constants.MuscleProteinSequenceNode, Constants.ExpectedScoreNode);
                    break;
                case MoleculeType.RNA:
                    Initialize(Constants.MuscleRnaSequenceNode, Constants.ExpectedScoreNode);
                    break;
                default:
                    break;
            }

            // Get the kmer distance matrix using default params
            IDistanceMatrix matrix = GetKmerDistanceMatrix(kmrlength);

            // Get the hierarchical clustering using default params
            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix,
                                                                                      hierarchicalMethodName);

            ValidateHierarchicalClustering(nodeName, hierarcicalClustering.Nodes,
                                           hierarcicalClustering.Edges);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamP1Test:: herarchical clustering stage1 nodes and edges generation and 
                    validation completed success for {0} moleculetype with different 
                    hierarchical clustering method name {1}",
                                                   moleculeType.ToString(),
                                                   hierarchicalMethodName.ToString()));
        }

        /// <summary>
        ///     Validate Hierarchical Clustering for stage2 using
        ///     kimura distance matrix and other default params.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="moleculeType">molecule type of sequences</param>
        private void ValidateHierarchicalClusteringStage2(string nodeName, MoleculeType moleculeType)
        {
            switch (moleculeType)
            {
                case MoleculeType.DNA:
                    Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
                    break;
                case MoleculeType.Protein:
                    Initialize(Constants.MuscleProteinSequenceNode, Constants.ExpectedScoreNode);
                    break;
                case MoleculeType.RNA:
                    Initialize(Constants.MuscleRnaSequenceNode, Constants.ExpectedScoreNode);
                    break;
                default:
                    break;
            }

            // Get stage1 aligned sequences.
            List<ISequence> stage1AlignedSequences = GetStage1AlignedSequence(moleculeType);

            // Get kimura distance matrix using stage 1 aligned sequences.
            IDistanceMatrix matrix = GetKimuraDistanceMatrix(stage1AlignedSequences);

            // Get the hierarchical clustering for stage 2
            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix);

            ValidateHierarchicalClustering(nodeName, hierarcicalClustering.Nodes,
                                           hierarcicalClustering.Edges);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamP1Test:: herarchical clustering stage2 nodes and edges generation and 
                   validation completed success for {0} moleculetype with default params",
                                                   moleculeType.ToString()));
        }

        /// <summary>
        ///     Validate Hierarchical Clustering for stage1 using kmer distance matrix
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="moleculeType">molecule type of sequences</param>
        /// <param name="kmrlength">kmer length to generate distance matrix</param>
        private void ValidateHierarchicalClusteringStage1(string nodeName,
                                                          int kmrlength, MoleculeType moleculeType)
        {
            switch (moleculeType)
            {
                case MoleculeType.DNA:
                    Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
                    break;
                case MoleculeType.Protein:
                    Initialize(Constants.MuscleProteinSequenceNode, Constants.ExpectedScoreNode);
                    break;
                case MoleculeType.RNA:
                    Initialize(Constants.MuscleRnaSequenceNode, Constants.ExpectedScoreNode);
                    break;
                default:
                    break;
            }

            // Get kmer distance matrix
            IDistanceMatrix matrix = GetKmerDistanceMatrix(kmrlength);

            // Get hierarchical clustering
            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix);

            // Validate the hierarchical clustering
            ValidateHierarchicalClustering(nodeName, hierarcicalClustering.Nodes,
                                           hierarcicalClustering.Edges);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamP1Test:: hierarchical clustering stage1 nodes and edges generation and 
                   validation completed successfully for {0} moleculetype with default params",
                                                   moleculeType.ToString()));
        }

        /// <summary>
        ///     Validate Hierarchical Clustering for stage2 using kimura distance matrix
        ///     and hierarchical method name
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="moleculeType">molecule type of sequences</param>
        /// <param name="hierarchicalMethodName">hierarchical method name</param>
        private void ValidateHierarchicalClusteringStage2(string nodeName, MoleculeType moleculeType,
                                                          UpdateDistanceMethodsTypes hierarchicalMethodName)
        {
            switch (moleculeType)
            {
                case MoleculeType.DNA:
                    Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
                    break;
                case MoleculeType.Protein:
                    Initialize(Constants.MuscleProteinSequenceNode, Constants.ExpectedScoreNode);
                    break;
                case MoleculeType.RNA:
                    Initialize(Constants.MuscleRnaSequenceNode, Constants.ExpectedScoreNode);
                    break;
                default:
                    break;
            }

            List<ISequence> stage1AlignedSequences = GetStage1AlignedSequence(moleculeType);

            // Get kimura distance matrix
            IDistanceMatrix matrix = GetKimuraDistanceMatrix(stage1AlignedSequences);

            // Get hierarchical clustering using method name
            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix,
                                                                                      hierarchicalMethodName);

            ValidateHierarchicalClustering(nodeName, hierarcicalClustering.Nodes,
                                           hierarcicalClustering.Edges);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamP1Test:: hierarchical clustering stage2 nodes and edges generation and 
                    validation completed success for {0} moleculetype with different 
                    hierarchical clustering method name {1}",
                                                   moleculeType.ToString(),
                                                   hierarchicalMethodName.ToString()));
        }

        /// <summary>
        ///     Validate the nodes and edges of hierarchical clustering object.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="nodes">binary tree nodes</param>
        /// <param name="edges">binary tree edges</param>
        private void ValidateHierarchicalClustering(string nodeName, List<BinaryGuideTreeNode> nodes,
                                                    List<BinaryGuideTreeEdge> edges)
        {
            // Validate the nodes and edges.
            string expectedEdgeCount = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.EdgesCount);
            string expectedNodesLeftChild = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                            Constants.NodesLeftChild);
            string expectedNodesRightChild = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                             Constants.NodesRightChild);
            string expectednode = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Nodes);
            string[] expectedNodes = expectednode.Split(',');
            int index = 0;
            int leftchildindex = 0;
            int rightchildindex = 0;

            foreach (BinaryGuideTreeNode node in nodes)
            {
                Assert.AreEqual(expectedNodes[index], node.ID.ToString((IFormatProvider) null));
                if (null != node.LeftChildren)
                {
                    Assert.IsTrue(expectedNodesLeftChild.Contains(
                        node.LeftChildren.ID.ToString((IFormatProvider) null)));
                    leftchildindex++;
                }
                if (null != node.RightChildren)
                {
                    Assert.IsTrue(expectedNodesRightChild.Contains(
                        node.RightChildren.ID.ToString((IFormatProvider) null)));
                    rightchildindex++;
                }
                index++;
            }
            Assert.AreEqual(expectedEdgeCount, edges.Count.ToString((IFormatProvider) null));
        }

        /// <summary>
        ///     Get Hierarchical Clustering using kmerdistancematrix\kimura
        ///     distance matrix and hierarchical method name.
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
                new HierarchicalClusteringParallel
                    (distanceMatrix, hierarchicalClusteringMethodName);

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
                new HierarchicalClusteringParallel
                    (distanceMatrix);

            return hierarcicalClustering;
        }

        /// <summary>
        ///     Get distance matrix with default distance function name
        /// </summary>
        /// <param name="kmrlength">kmr length</param>
        /// <param name="moleculeType">Molecule Type</param>
        /// <returns>Distance matrix</returns>
        private IDistanceMatrix GetKmerDistanceMatrix(int kmrlength)
        {
            // Generate DistanceMatrix
            var kmerDistanceMatrixGenerator =
                new KmerDistanceMatrixGenerator(lstSequences, kmrlength, lstSequences[0].Alphabet);

            return kmerDistanceMatrixGenerator.DistanceMatrix;
        }

        /// <summary>
        ///     Get distance matrix with distance function name
        /// </summary>
        /// <param name="kmrlength">kmr length</param>
        /// <param name="moleculeType">Molecule Type</param>
        /// <param name="distanceFunctionName">distance matrix function name.</param>
        /// <returns>Distance matrix</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "moleculeType")]
        private IDistanceMatrix GetKmerDistanceMatrix(int kmrlength, MoleculeType moleculeType,
                                                      DistanceFunctionTypes distanceFunctionName)
        {
            // Generate DistanceMatrix
            var kmerDistanceMatrixGenerator =
                new KmerDistanceMatrixGenerator(lstSequences, kmrlength,
                                                lstSequences[0].Alphabet, distanceFunctionName);

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
        ///     Get the aligned sequence for stage1
        /// </summary>
        /// <param name="moleculeType">Molecule Type</param>
        /// <returns>List of sequences</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "moleculeType")]
        private List<ISequence> GetStage1AlignedSequence(MoleculeType moleculeType)
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
        ///     Validate the binary tree leaves, root using unaligned sequences.
        /// </summary>
        /// <param name="initNodeName">Init Node name</param>
        /// <param name="nodeName">xml node name</param>
        /// <param name="kmrLength">kmer length to generate distance matrix</param>
        /// <param name="moleculeType">molecule type of sequences</param>
        private void ValidateBinaryTreeNodesandEdges(string initNodeName, string nodeName,
                                                     int kmrLength, MoleculeType moleculeType)
        {
            Initialize(initNodeName, Constants.ExpectedScoreNode);
            IDistanceMatrix matrix = GetKmerDistanceMatrix(kmrLength);
            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix);
            BinaryGuideTree binaryTree = GetBinaryTree(hierarcicalClustering);

            ValidateBinaryTree(binaryTree, nodeName);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamP1Test:: Validation of binary tree nodes and edges completed successfully for 
                            {0} moleculetype",
                                                   moleculeType.ToString()));
        }

        /// <summary>
        ///     Validate the binary sub tree by cutting the tree and validating nodes
        ///     of sub tree using ExtractSubTreeNodes()
        /// </summary>
        /// <param name="initNodeName">xml node name.</param>
        /// <param name="nodeName">binary tree node name</param>
        /// <param name="edgeIndex">edge index to cut the tree</param>
        /// <param name="moleculeType">molecule type</param>
        private void ValidateBinaryTreeWithExtractSubTreeNodesAndCutTree(string initNodeName, string nodeName,
                                                                         int edgeIndex, MoleculeType moleculeType)
        {
            Initialize(initNodeName, Constants.ExpectedScoreNode);
            IDistanceMatrix matrix = GetKmerDistanceMatrix(kmerLength);

            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix);
            BinaryGuideTree binaryTree = GetBinaryTree(hierarcicalClustering);

            BinaryGuideTree[] subtrees = binaryTree.CutTree(edgeIndex);
            IList<BinaryGuideTreeNode> nodes = binaryTree.ExtractSubTreeNodes(subtrees[0].Nodes[subtrees[0].Root.ID - 1]);

            // Validate the Binary Tree
            string expectedNodesLeftChild = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.NodesLeftChild);
            string expectedNodesRightChild = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.NodesRightChild);
            string expectednode = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Nodes);

            foreach (BinaryGuideTreeNode node in nodes)
            {
                Assert.IsTrue(expectednode.Contains(node.ID.ToString((IFormatProvider) null)));
                if (null != node.LeftChildren)
                {
                    Assert.IsTrue(expectedNodesLeftChild.Contains(node.LeftChildren.ID.ToString((IFormatProvider) null)));
                }
                if (null != node.RightChildren)
                {
                    Assert.IsTrue(expectedNodesRightChild.Contains(node.RightChildren.ID.ToString((IFormatProvider) null)));
                }
            }

            ApplicationLog.WriteLine("PamsamP1Test: Validate Binary tree by cutting tree at an edge index {0}. " +
                                     "Validation of subtree nodes and edges completed successfully for {1} moleculetype",
                                        edgeIndex, moleculeType);
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
            string expectednode = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Nodes);
            string[] expectedNodes = expectednode.Split(',');


            Assert.AreEqual(rootId, binaryTree.Root.ID.ToString((IFormatProvider) null));
            Assert.AreEqual(leavesCount, binaryTree.NumberOfLeaves.ToString((IFormatProvider) null));
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
        ///     Validate the binary tree leaves, root using unaligned sequences.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="kmrLength">kmer length to generate distance matrix</param>
        /// <param name="moleculeType">molecule type of sequences</param>
        private void ValidateBinaryTreeFindSmallestTreeDifference(string nodeName, int kmrLength,
                                                                  MoleculeType moleculeType)
        {
            Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
            IDistanceMatrix matrix = GetKmerDistanceMatrix(kmrLength);
            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix);
            BinaryGuideTree binaryTree = GetBinaryTree(hierarcicalClustering);
            BinaryGuideTreeNode node = BinaryGuideTree.FindSmallestTreeDifference(
                binaryTree.Nodes[binaryTree.Nodes.Count - 1], binaryTree.Nodes[0]);

            // Validate the node
            string expectedNodesLeftChild = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                            Constants.NodesLeftChild);
            string expectedNodesRightChild = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                             Constants.NodesRightChild);
            string expectednode = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Nodes);

            Assert.AreEqual(node.ID.ToString((IFormatProvider) null), expectednode);
            Assert.AreEqual(node.LeftChildren.ID.ToString((IFormatProvider) null), expectedNodesLeftChild);
            Assert.AreEqual(node.RightChildren.ID.ToString((IFormatProvider) null), expectedNodesRightChild);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamP1Test:: Find smallest nodes between two subtrees 
                   and Validation of smallest node completed successfully for moleculetype {0}",
                                                   moleculeType.ToString()));
        }

        /// <summary>
        ///     Compare the two tree and validate the nodes whcih needs realignment.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="kmrLength">kmr length to generate distance matrix.</param>
        /// <param name="moleculeType">Molecule Type</param>
        private void ValidateBinaryTreeCompareTrees(string nodeName, int kmrLength, MoleculeType moleculeType)
        {
            BinaryGuideTree stage1BinaryTree = GetStage1BinaryTree(kmrLength, moleculeType);
            BinaryGuideTree stage2BinaryTree = GetStage2BinaryTree(moleculeType);
            BinaryGuideTree.CompareTwoTrees(stage1BinaryTree, stage2BinaryTree);

            string expectednode = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.Nodes);
            string[] expectedNodes = expectednode.Split(',');
            int index = 0;
            foreach (BinaryGuideTreeNode node in stage1BinaryTree.Nodes)
            {
                if (node.NeedReAlignment)
                {
                    Assert.AreEqual(node.ID.ToString((IFormatProvider) null), expectedNodes[index]);
                }
                index++;
            }

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamP1Test:: Comparison and Validation of stage1 and stage2 binary tree
                   completed successfully for moleculetype {0}",
                                                   moleculeType.ToString()));
        }

        /// <summary>
        ///     Get stage 1 binary tree using kmerdistance matrix and hierarchical clustering.
        /// </summary>
        /// <param name="kmrLength">kmr length to generate distance matrix.</param>
        /// <param name="moleculeType">Molecule Type.</param>
        /// <returns>returns stage1 binary tree</returns>
        private BinaryGuideTree GetStage1BinaryTree(int kmrLength, MoleculeType moleculeType)
        {
            switch (moleculeType)
            {
                case MoleculeType.DNA:
                    Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
                    break;
                case MoleculeType.Protein:
                    Initialize(Constants.MuscleProteinSequenceNode, Constants.ExpectedScoreNode);
                    break;
                case MoleculeType.RNA:
                    Initialize(Constants.MuscleRnaSequenceNode, Constants.ExpectedScoreNode);
                    break;
            }
            IDistanceMatrix matrix = GetKmerDistanceMatrix(kmrLength);
            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix);
            BinaryGuideTree binaryTree = GetBinaryTree(hierarcicalClustering);

            return binaryTree;
        }

        /// <summary>
        ///     Get Stage2 binary tree using kimura distance matrix and hierarchical clustering.
        /// </summary>
        /// <param name="moleculeType">Molecule Type.</param>
        /// <returns>returns stage2 binary tree</returns>
        private BinaryGuideTree GetStage2BinaryTree(MoleculeType moleculeType)
        {
            switch (moleculeType)
            {
                case MoleculeType.DNA:
                    Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
                    break;
                case MoleculeType.Protein:
                    Initialize(Constants.MuscleProteinSequenceNode, Constants.ExpectedScoreNode);
                    break;
                case MoleculeType.RNA:
                    Initialize(Constants.MuscleRnaSequenceNode, Constants.ExpectedScoreNode);
                    break;
            }
            List<ISequence> stage1AlignedSequences = GetStage1AlignedSequence(moleculeType);
            IDistanceMatrix matrix = GetKimuraDistanceMatrix(stage1AlignedSequences);
            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix);
            BinaryGuideTree binaryTree = GetBinaryTree(hierarcicalClustering);

            return binaryTree;
        }

        /// <summary>
        ///     Get the binary tree object using hierarchical clustering object
        /// </summary>
        /// <param name="hierarchicalClustering">hierarchical Clustering</param>
        /// <returns></returns>
        private static BinaryGuideTree GetBinaryTree(IHierarchicalClustering hierarchicalClustering)
        {
            // Generate Guide Tree
            var binaryGuideTree =
                new BinaryGuideTree(hierarchicalClustering);

            return binaryGuideTree;
        }

        /// <summary>
        ///     Validate Progressive Alignment of Stage 1
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="moleculeType">Molecule Type.</param>
        private void ValidateProgressiveAlignmentStage1(string nodeName, MoleculeType moleculeType)
        {
            Initialize(nodeName, Constants.ExpectedScoreNode);
            InitializeStage1Variables(nodeName);
            IDistanceMatrix matrix = GetKmerDistanceMatrix(kmerLength);
            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix);
            BinaryGuideTree binaryTree = GetBinaryTree(hierarcicalClustering);
            List<ISequence> alignedSequences = GetProgressiveAlignerAlignedSequences(
                lstSequences, binaryTree, moleculeType);

            // Validate the aligned Sequence of stage1
            string expectedSeqString = string.Empty;
            foreach (ISequence seq in expectedSequences)
            {
                expectedSeqString += new string(seq.Select(a => (char) a).ToArray()) + ",";
            }

            // Validate expected sequence
            foreach (ISequence seq in alignedSequences)
            {
                Assert.IsTrue(expectedSeqString.Contains(new string(seq.Select(a => (char) a).ToArray())));
            }

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamP1Test:: Validation and generation of stage1 aligned sequences
                   using progressivealignment completed successfully for moleculetype {0}",
                                                   moleculeType.ToString()));
        }

        /// <summary>
        ///     Get the progressive aligned sequences using input sequences and its binary tree
        /// </summary>
        /// <param name="sequences">input sequences</param>
        /// <param name="binaryGuidTree">binary tree</param>
        /// <param name="moleculeType">Molecule type</param>
        /// <returns>returns the list of aligned sequences</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "moleculeType")]
        private List<ISequence> GetProgressiveAlignerAlignedSequences(List<ISequence> sequences,
                                                                      BinaryGuideTree binaryGuidTree,
                                                                      MoleculeType moleculeType)
        {
            // Progressive Alignment
            IProgressiveAligner progressiveAligner = new ProgressiveAligner(profileAligner);
            progressiveAligner.Align(sequences, binaryGuidTree);

            return progressiveAligner.AlignedSequences;
        }

        /// <summary>
        ///     Validate the Profile Aligner GenerateEString() method using profiles of sub trees.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="edgeIndex">Edge index to cut tree.</param>
        /// <param name="moleculeType">Molecule Type</param>
        private void ValidateProfileAlignerGenerateEString(string nodeName, MoleculeType moleculeType, int edgeIndex)
        {
            switch (moleculeType)
            {
                case MoleculeType.DNA:
                    Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
                    InitializeStage2Variables(Constants.MuscleDnaSequenceNode);
                    break;
                case MoleculeType.Protein:
                    Initialize(Constants.MuscleProteinSequenceNode, Constants.ExpectedScoreNode);
                    InitializeStage2Variables(Constants.MuscleProteinSequenceNode);
                    break;
                case MoleculeType.RNA:
                    Initialize(Constants.MuscleRnaSequenceNode, Constants.ExpectedScoreNode);
                    InitializeStage2Variables(Constants.MuscleRnaSequenceNode);
                    break;
            }
            ;

            // Get Stage2 Binary Tree
            List<ISequence> stage1AlignedSequences = GetStage1AlignedSequence(moleculeType);
            IDistanceMatrix matrix = GetKimuraDistanceMatrix(stage1AlignedSequences);
            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix);
            BinaryGuideTree binaryTree = GetBinaryTree(hierarcicalClustering);

            // Get profiles
            GetAlignedProfiles(edgeIndex, binaryTree, stage1AlignedSequences);

            // Get id's of edges and root using two profiles
            List<int> eStringSubtreeEdge = profileAligner.GenerateEString(profileAligner.AlignedA);
            List<int> eStringSubtreeRoot = profileAligner.GenerateEString(profileAligner.AlignedB);

            string expectedTreeEdges = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SubTreeEdges);
            string expectedTreeRoot = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SubTreeRoots);

            for (int index = 0; index < eStringSubtreeEdge.Count; index++)
            {
                Assert.IsTrue(expectedTreeEdges.Contains(eStringSubtreeEdge[index].ToString((IFormatProvider) null)));
            }

            Assert.IsTrue(expectedTreeRoot.Contains(eStringSubtreeRoot[0].ToString((IFormatProvider) null)));

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamP1Test:: Validation and generation of subtrees roots and edges
                   using profile aligner GenerateEString() completed successfully for moleculetype{0}",
                                                   moleculeType.ToString()));
        }

        /// <summary>
        ///     Validate the Profile Aligner GenerateEString() method using profiles of sub trees.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="moleculeType">Molecule Type</param>
        /// <param name="edgeIndex">Edge index to cut tree.</param>
        private void ValidateProfileExtraction(string nodeName, MoleculeType moleculeType, int edgeIndex)
        {
            switch (moleculeType)
            {
                case MoleculeType.DNA:
                    Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
                    break;
                case MoleculeType.Protein:
                    Initialize(Constants.MuscleProteinSequenceNode, Constants.ExpectedScoreNode);
                    break;
                case MoleculeType.RNA:
                    Initialize(Constants.MuscleRnaSequenceNode, Constants.ExpectedScoreNode);
                    break;
            }

            // Get Stage2 Binary Tree
            List<ISequence> stage1AlignedSequences = GetStage1AlignedSequence(moleculeType);
            IDistanceMatrix matrix = GetKimuraDistanceMatrix(stage1AlignedSequences);
            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix);
            BinaryGuideTree binaryTree = GetBinaryTree(hierarcicalClustering);

            // Cut Tree at an edge and get sequences.
            List<int>[] leafNodeIndices = binaryTree.SeparateSequencesByCuttingTree(edgeIndex);

            // Extract profiles.
            List<int>[] removedPositions = null;
            IProfileAlignment[] separatedProfileAlignments =
                ProfileAlignment.ProfileExtraction(stage1AlignedSequences, leafNodeIndices[0],
                                                   leafNodeIndices[1], out removedPositions);

            // Validate the profiles.
            string expectedColSize = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ColumnSize);
            string expectedProfileCount = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ProfileMatrix);

            Assert.AreEqual(expectedColSize,
                            separatedProfileAlignments[0].ProfilesMatrix.ColumnSize.ToString((IFormatProvider) null));
            Assert.IsTrue(
                expectedProfileCount.Contains(
                    separatedProfileAlignments[0].ProfilesMatrix.ProfilesMatrix.Count.ToString((IFormatProvider) null)));

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamP1Test:: Validation and generation of stage1 aligned sequences subtrees profile
                            using profile aligner ProfileExtraction() completed successfully for moleculetype{0}",
                                                   moleculeType.ToString()));
        }

        /// <summary>
        ///     Validate the Profile Aligner GenerateEString() method using profiles of sub trees.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="moleculeType">Molecule Type</param>
        private void ValidateGenerateProfileAlignmentWithSequences(string nodeName, MoleculeType moleculeType)
        {
            switch (moleculeType)
            {
                case MoleculeType.DNA:
                    Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
                    break;
                case MoleculeType.Protein:
                    Initialize(Constants.MuscleProteinSequenceNode, Constants.ExpectedScoreNode);
                    break;
                case MoleculeType.RNA:
                    Initialize(Constants.MuscleRnaSequenceNode, Constants.ExpectedScoreNode);
                    break;
            }

            List<ISequence> stage1AlignedSequences = GetStage1AlignedSequence(moleculeType);
            IProfileAlignment profile = ProfileAlignment.GenerateProfileAlignment(stage1AlignedSequences);

            // Validate the profile alignment
            string expectedColSize = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ColumnSize);
            string expectedRowSize = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.RowSize);

            Assert.AreEqual(profile.ProfilesMatrix.ColumnSize.ToString((IFormatProvider) null), expectedColSize);
            Assert.AreEqual(profile.ProfilesMatrix.RowSize.ToString((IFormatProvider) null), expectedRowSize);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamP1Test:: Validation and generation of stage1 aligned sequences profile
                            using profile aligner GenerateProfileAlignment() completed successfully for moleculetype{0}",
                                                   moleculeType.ToString()));
        }

        /// <summary>
        ///     Validate the Profile Aligner GenerateEString() method using profiles of sub trees.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="moleculeType">Molecule Type</param>
        /// <param name="edgeIndex">Edge index to cut tree.</param>
        private void ValidateGenerateProfileAlignmentWithProfiles(string nodeName,
                                                                  MoleculeType moleculeType, int edgeIndex)
        {
            switch (moleculeType)
            {
                case MoleculeType.DNA:
                    Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
                    InitializeStage2Variables(Constants.MuscleDnaSequenceNode);
                    break;
                case MoleculeType.Protein:
                    Initialize(Constants.MuscleProteinSequenceNode, Constants.ExpectedScoreNode);
                    InitializeStage2Variables(Constants.MuscleProteinSequenceNode);
                    break;
                case MoleculeType.RNA:
                    Initialize(Constants.MuscleRnaSequenceNode, Constants.ExpectedScoreNode);
                    InitializeStage2Variables(Constants.MuscleRnaSequenceNode);
                    break;
            }
            ;
            BinaryGuideTree binaryTree = GetStage2BinaryTree(moleculeType);

            // Cut the tree
            List<int>[] leafNodeIndices = binaryTree.SeparateSequencesByCuttingTree(edgeIndex);

            // separate the profiles
            List<int>[] removedPositions = null;
            IProfileAlignment[] separatedProfileAlignments = ProfileAlignment.ProfileExtraction(
                stage2ExpectedSequences, leafNodeIndices[0], leafNodeIndices[1], out removedPositions);

            // Now again get combined profile
            IProfileAlignment profile =
                ProfileAlignment.GenerateProfileAlignment(separatedProfileAlignments[0],
                                                          separatedProfileAlignments[0]);

            // Validate the profile alignment
            string expectedColSize = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ColumnSize);
            string expectedRowSize = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.RowSize);

            Assert.AreEqual(profile.ProfilesMatrix.ColumnSize.ToString((IFormatProvider) null), expectedColSize);
            Assert.AreEqual(profile.ProfilesMatrix.RowSize.ToString((IFormatProvider) null), expectedRowSize);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamP1Test:: Validation and generation of subtrees profiles
                            using profile aligner GenerateProfileAlignment() completed successfully for moleculetype{0}",
                                                   moleculeType.ToString()));
        }

        /// <summary>
        ///     Validate the Profile Aligner GenerateSequenceString() method using profiles of sub trees.
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="moleculeType">Molecule Type</param>
        /// <param name="edgeIndex">Edge index to cut tree.</param>
        private void ValidateProfileAlignerGenerateSequenceString(string nodeName,
                                                                  MoleculeType moleculeType, int edgeIndex)
        {
            switch (moleculeType)
            {
                case MoleculeType.DNA:
                    Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
                    InitializeStage2Variables(Constants.MuscleDnaSequenceNode);
                    break;
                case MoleculeType.Protein:
                    Initialize(Constants.MuscleProteinSequenceNode, Constants.ExpectedScoreNode);
                    InitializeStage2Variables(Constants.MuscleProteinSequenceNode);
                    break;
                case MoleculeType.RNA:
                    Initialize(Constants.MuscleRnaSequenceNode, Constants.ExpectedScoreNode);
                    InitializeStage2Variables(Constants.MuscleRnaSequenceNode);
                    break;
            }
            ;

            // Get Stage2 Binary Tree
            List<ISequence> stage1AlignedSequences = GetStage1AlignedSequence(moleculeType);
            IDistanceMatrix matrix = GetKimuraDistanceMatrix(stage1AlignedSequences);
            IHierarchicalClustering hierarcicalClustering = GetHierarchicalClustering(matrix);
            BinaryGuideTree binaryTree = GetBinaryTree(hierarcicalClustering);

            GetAlignedProfiles(edgeIndex, binaryTree, stage1AlignedSequences);

            // Get id's of edges and root using two profiles
            List<int> eStringSubtreeEdge = profileAligner.GenerateEString(profileAligner.AlignedA);

            string expectedSequence = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                      Constants.GenerateSequenceString);

            ISequence sequence = profileAligner.GenerateSequenceFromEString(
                eStringSubtreeEdge, stage1AlignedSequences[0]);

            Assert.IsTrue(expectedSequence.Contains(new string(sequence.Select(a => (char) a).ToArray())));

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamP1Test:: Validation and generation of subtrees sequences
                              using profile aligner GenerateSequenceFromEString() completed successfully for moleculetype{0}",
                                                   moleculeType.ToString()));
        }

        /// <summary>
        ///     Get the profiles of two sub tree by cutting the tree at an edge
        /// </summary>
        /// <param name="edgeIndex">edge index to cut the tree</param>
        /// <param name="binaryTree">binary tree object</param>
        /// <param name="sequences">list of sequences</param>
        private void GetAlignedProfiles(int edgeIndex, BinaryGuideTree binaryTree, List<ISequence> sequences)
        {
            // Cut Tree at an edge and get sequences.
            List<int>[] leafNodeIndices = binaryTree.SeparateSequencesByCuttingTree(edgeIndex);

            // Extract profiles and align it.
            List<int>[] removedPositions = null;
            IProfileAlignment[] separatedProfileAlignments = ProfileAlignment.ProfileExtraction(
                sequences, leafNodeIndices[0], leafNodeIndices[1], out removedPositions);
            profileAligner.Align(separatedProfileAlignments[0], separatedProfileAlignments[1]);
        }

        /// <summary>
        ///     Validate the kimura distance matrix using stage 1 aligned sequences.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="moleculeType">Molecule Type.</param>
        private void ValidateKimuraDistanceMatrix(string nodeName, MoleculeType moleculeType)
        {
            switch (moleculeType)
            {
                case MoleculeType.DNA:
                    Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
                    break;
                case MoleculeType.Protein:
                    Initialize(Constants.MuscleProteinSequenceNode, Constants.ExpectedScoreNode);
                    break;
                case MoleculeType.RNA:
                    Initialize(Constants.MuscleRnaSequenceNode, Constants.ExpectedScoreNode);
                    break;
            }
            List<ISequence> stage1AlignedSequences = GetStage1AlignedSequence(moleculeType);

            IDistanceMatrix matrix = GetKimuraDistanceMatrix(stage1AlignedSequences);
            ValidateDistanceMatrix(nodeName, matrix);

            ApplicationLog.WriteLine(String.Format(null,
                                                   @"PamsamP1Test:: kimura distance matrix generation and validation completed success for {0} 
                    moleculetype with default params",
                                                   moleculeType.ToString()));
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
        /// <param name="moleculeType">Molecule Type</param>
        /// <param name="degreeOfParallelism">if 1 it is serial Profiler else parallel profiler</param>
        /// <param name="edgeIndex">edge index to cut the tree</param>
        /// <param name="overloadType">Execute Align()\AlignSimple()</param>
        private void ValidateProfileAlignerAlign(string nodeName, MoleculeType moleculeType,
                                                 int degreeOfParallelism, int edgeIndex, AlignType overloadType)
        {
            switch (moleculeType)
            {
                case MoleculeType.DNA:
                    Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
                    InitializeStage2Variables(Constants.MuscleDnaSequenceNode);
                    break;
                case MoleculeType.RNA:
                    Initialize(Constants.MuscleRnaSequenceNode, Constants.ExpectedScoreNode);
                    InitializeStage2Variables(Constants.MuscleRnaSequenceNode);
                    break;
                case MoleculeType.Protein:
                    Initialize(Constants.MuscleProteinSequenceNode, Constants.ExpectedScoreNode);
                    InitializeStage2Variables(Constants.MuscleProteinSequenceNode);
                    break;
            }


            // Get Stage2 Binary Tree
            List<ISequence> stage1AlignedSequences = GetStage1AlignedSequence(moleculeType);
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
                var nwprofileAligner = new
                    NeedlemanWunschProfileAlignerSerial(similarityMatrix,
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
                    var nwprofileAlignerParallel = new
                        NeedlemanWunschProfileAlignerParallel(
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
                    ApplicationLog.WriteLine(
                        String.Format(null,
                                      @"PamsamP1Test: NeedlemanWunschProfileAlignerParallel could not be instantiated
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

                ApplicationLog.WriteLine(
                    String.Format(null, @"PamsamP1Test: {0} {1} method validation completed successfully with
                    number of processor is {2} and degree of parallelism {3} for molecule type {4}",
                                  profileAligner,
                                  overloadType,
                                  Environment.ProcessorCount.ToString((IFormatProvider) null),
                                  degreeOfParallelism,
                                  moleculeType));
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
        /// <param name="moleculeType">Molecule Type</param>
        private void ValidateProfileAlignerAlignWithProfileFunctionScore(string nodeName, int degreeOfParallelism,
                                                                         ProfileScoreFunctionNames profileFunction,
                                                                         int edgeIndex, MoleculeType moleculeType)
        {
            switch (moleculeType)
            {
                case MoleculeType.DNA:
                    Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
                    InitializeStage2Variables(Constants.MuscleDnaSequenceNode);
                    break;
                case MoleculeType.RNA:
                    Initialize(Constants.MuscleRnaSequenceNode, Constants.ExpectedScoreNode);
                    InitializeStage2Variables(Constants.MuscleRnaSequenceNode);
                    break;
                case MoleculeType.Protein:
                    Initialize(Constants.MuscleProteinSequenceNode, Constants.ExpectedScoreNode);
                    InitializeStage2Variables(Constants.MuscleProteinSequenceNode);
                    break;
            }


            // Get Stage2 Binary Tree
            List<ISequence> stage1AlignedSequences = GetStage1AlignedSequence(moleculeType);
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
                    ApplicationLog.WriteLine(
                        String.Format(null,
                                      @"PamsamP1Test: NeedlemanWunschProfileAlignerParallel could not be instantiated
                        as number of processor is {0} and degree of parallelism {1}",
                                      Environment.ProcessorCount.ToString((IFormatProvider) null),
                                      degreeOfParallelism));
                }
            }

            IProfileAlignment profileAlignment = aligner.Align(separatedProfileAlignments[0],
                                                               separatedProfileAlignments[0]);

            // Validate profile alignement 
            string expectedRowSize = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.RowSize);
            string expectedColSize = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ColumnSize);
            Assert.IsTrue(
                expectedColSize.Contains(profileAlignment.ProfilesMatrix.ColumnSize.ToString((IFormatProvider) null)));
            Assert.IsTrue(
                expectedRowSize.Contains(profileAlignment.ProfilesMatrix.RowSize.ToString((IFormatProvider) null)));

            ApplicationLog.WriteLine(
                String.Format(null, @"PamsamP1Test: {0} Align() method validation completed successfully with
                number of processor is {1} and degree of parallelism {2} for molecule type {3}",
                              profileAligner,
                              Environment.ProcessorCount.ToString((IFormatProvider) null),
                              degreeOfParallelism,
                              moleculeType));
        }

        /// <summary>
        ///     Validate different alignment score functions
        ///     using input sequences and reference sequences
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="type">Molecule Type</param>
        /// <param name="scoreType">Score Function Type.</param>
        private void ValidateAlignmentScore(string nodeName, MoleculeType type, ScoreType scoreType)
        {
            string inputFilePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string refFilePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.RefFilePathNode);


            ISequenceParser parser = null;
            ISequenceParser refParser = null;
            try
            {
                parser = new FastAParser(inputFilePath);
                refParser = new FastAParser(refFilePath);

                IEnumerable<ISequence> sequences = parser.Parse();
                List<ISequence> seqList = sequences.ToList();
                IEnumerable<ISequence> refSequences = refParser.Parse();
                List<ISequence> refSeqList = refSequences.ToList();

                IList<ISequence> alignedSequences = GetPAMSAMAlignedSequences(type, seqList);

                // Validate the score
                switch (scoreType)
                {
                    case ScoreType.QScore:
                        string expectedQScore = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.QScoreNode);
                        float qScore = MsaUtils.CalculateAlignmentScoreQ(alignedSequences, refSeqList);
                        Assert.IsTrue(expectedQScore.Contains(qScore.ToString((IFormatProvider) null).Substring(0, 4)));
                        break;
                    case ScoreType.TCScore:
                        string expectedTCScore = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                 Constants.TCScoreNode);
                        float tcScore = MsaUtils.CalculateAlignmentScoreQ(alignedSequences, refSeqList);
                        Assert.IsTrue(expectedTCScore.Contains(tcScore.ToString((IFormatProvider) null)));
                        break;
                    case ScoreType.Offset:
                        string expectedResiduesCount = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                       Constants.ResiduesCountNode);
                        List<int> positions = MsaUtils.CalculateOffset(alignedSequences[0], refSeqList[0]);
                        int residuesCount = 0;
                        for (int i = 0; i < positions.Count; i++)
                        {
                            if (positions[i] < 0)
                            {
                                residuesCount++;
                            }
                        }

                        Assert.IsTrue(expectedResiduesCount.Contains(residuesCount.ToString((IFormatProvider) null)));
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
                        Assert.IsTrue(expectedPairwiseScore.Contains(pairwiseScore.ToString((IFormatProvider) null)));
                        break;
                }

                ApplicationLog.WriteLine(
                    String.Format(null, @"PamsamP1Test:{0} validation completed successfully for molecule type {1}",
                                  scoreType.ToString(),
                                  type));
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
        private void ValidateUNAlign(string nodeName, MoleculeType type)
        {
            string inputFilePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);

            ISequenceParser parser = null;
            try
            {
                parser = new FastAParser(inputFilePath);
                IEnumerable<ISequence> sequences = parser.Parse();
                List<ISequence> seqList = sequences.ToList();
                IList<ISequence> alignedSequences = GetPAMSAMAlignedSequences(type, seqList);
                var gapItem = (byte) '-';
                Assert.IsTrue(alignedSequences[0].Contains(gapItem));
                ISequence unalignedseq = MsaUtils.UnAlign(alignedSequences[0]);
                Assert.IsFalse(unalignedseq.Contains(gapItem));

                ApplicationLog.WriteLine(
                    String.Format(null, @"PamsamP1Test:Validation of UnAlign() method of MsaUtils completed 
                                            successfully for molecule type {0}",
                                  type));
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
        /// <param name="moleculeType">Molecule Type.</param>
        /// <param name="sequences">sequences.</param>
        /// <returns>returns aligned sequences</returns>
        private IList<ISequence> GetPAMSAMAlignedSequences(MoleculeType moleculeType,
                                                           IList<ISequence> sequences)
        {
            switch (moleculeType)
            {
                case MoleculeType.DNA:
                    similarityMatrix = new SimilarityMatrix(
                        SimilarityMatrix.StandardSimilarityMatrix.AmbiguousDna);
                    break;
                case MoleculeType.RNA:
                    similarityMatrix = new SimilarityMatrix(
                        SimilarityMatrix.StandardSimilarityMatrix.AmbiguousRna);
                    break;
                case MoleculeType.Protein:
                    similarityMatrix = new SimilarityMatrix(
                        SimilarityMatrix.StandardSimilarityMatrix.Blosum62);
                    break;
            }
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
        /// <param name="moleculeType">Molecule Type</param>
        /// <param name="edgeIndex">Edge Index</param>
        /// <param name="functionType">Function Type.</param>
        private void ValidateFunctionCalculations(string nodeName,
                                                  MoleculeType moleculeType, int edgeIndex, FunctionType functionType)
        {
            // Get Two profiles
            IProfileAlignment[] separatedProfileAlignments = GetProfiles(moleculeType, edgeIndex);

            switch (functionType)
            {
                case FunctionType.Correlation:
                    float correlation = MsaUtils.Correlation(
                        separatedProfileAlignments[0].ProfilesMatrix.ProfilesMatrix[0],
                        separatedProfileAlignments[1].ProfilesMatrix.ProfilesMatrix[0]);
                    string expectedCorrelation = utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                 Constants.CorrelationNode);
                    Assert.IsTrue(expectedCorrelation.Contains(correlation.ToString((IFormatProvider) null)));
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
                    Assert.IsTrue(expectedJsDivergence.Contains(jsdivergence.ToString((IFormatProvider) null)));
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

            ApplicationLog.WriteLine(String.Format(null, @"Validation of {0} function calculation of MsaUtils completed 
                                            successfully for molecule type {1}",
                                                   functionType,
                                                   moleculeType));
        }

        /// <summary>
        ///     Creates binarytree using stage1 sequences.
        ///     Cut the binary tree at an random edge to get two profiles.
        /// </summary>
        /// <param name="moleculeType">Molecule Type.</param>
        /// <param name="edgeIndex">Random edge index.</param>
        /// <returns>Returns profiles</returns>
        private IProfileAlignment[] GetProfiles(MoleculeType moleculeType, int edgeIndex)
        {
            switch (moleculeType)
            {
                case MoleculeType.DNA:
                    Initialize(Constants.MuscleDnaSequenceNode, Constants.ExpectedScoreNode);
                    InitializeStage2Variables(Constants.MuscleDnaSequenceNode);
                    break;
                case MoleculeType.RNA:
                    Initialize(Constants.MuscleRnaSequenceNode, Constants.ExpectedScoreNode);
                    InitializeStage2Variables(Constants.MuscleRnaSequenceNode);
                    break;
                case MoleculeType.Protein:
                    Initialize(Constants.MuscleProteinSequenceNode, Constants.ExpectedScoreNode);
                    InitializeStage2Variables(Constants.MuscleProteinSequenceNode);
                    break;
            }


            // Get Stage2 Binary Tree
            List<ISequence> stage1AlignedSequences = GetStage1AlignedSequence(moleculeType);
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
    }
}