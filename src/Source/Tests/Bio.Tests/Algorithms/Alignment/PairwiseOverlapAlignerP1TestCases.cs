using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Bio.Algorithms.Alignment;
using Bio.IO.FastA;
using Bio.SimilarityMatrices;
using Bio.TestAutomation.Util;
using Bio.Tests.Framework;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests.Algorithms.Alignment
{
    /// <summary>
    ///     Pairwise Overlap Aligner algorithm Bvt test cases
    /// </summary>
    [TestFixture]
    public class PairwiseOverlapAlignerP1TestCases
    {
        #region Enums

        /// <summary>
        ///     Alignment Parameters which are used for different test cases
        ///     based on which the test cases are executed.
        /// </summary>
        private enum AlignParameters
        {
            AlignList,
            AlignListCode,
            AllParam,
            AllParamCode,
            AlignTwo,
            AlignTwoCode
        };

        /// <summary>
        ///     Alignment Type Parameters which are used for different test cases
        ///     based on which the test cases are executed.
        /// </summary>
        private enum AlignmentType
        {
            SimpleAlign,
            Align,
        };

        /// <summary>
        ///     Similarity Matrix Parameters which are used for different test cases
        ///     based on which the test cases are executed with different Similarity Matrixes.
        /// </summary>
        private enum SimilarityMatrixParameters
        {
            TextReader,
            DiagonalMatrix,
            Default
        };

        #endregion Enums

        #region Global Variables

        private readonly Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

        #region PairwiseOverlapAligner P1 Test cases

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(ListofSequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA DNA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignListSequencesDna()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapDnaAlignAlgorithmNodeName,
                                             AlignParameters.AlignList);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA DNA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignAllParamDna()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapDnaAlignAlgorithmNodeName,
                                             AlignParameters.AllParam);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(ListofSequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignListSequencesPro()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapProAlignAlgorithmNodeName,
                                             AlignParameters.AlignList);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignAllParamPro()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapProAlignAlgorithmNodeName,
                                             AlignParameters.AllParam);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(ListofSequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Rna File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignListSequencesRna()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapRnaAlignAlgorithmNodeName,
                                             AlignParameters.AlignList);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Rna File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignAllParamRna()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapRnaAlignAlgorithmNodeName,
                                             AlignParameters.AllParam);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(ListofSequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with Max Gap Cost
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignListSequencesGapCostMax()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapGapCostMaxAlignAlgorithmNodeName,
                                             AlignParameters.AlignList);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with Max Gap Cost
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignAllParamGapCostMax()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapGapCostMaxAlignAlgorithmNodeName,
                                             AlignParameters.AllParam);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(ListofSequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with Min Gap Cost
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignListSequencesGapCostMin()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapGapCostMinAlignAlgorithmNodeName,
                                             AlignParameters.AlignList);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with Min Gap Cost
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignAllParamGapCostMin()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapGapCostMinAlignAlgorithmNodeName,
                                             AlignParameters.AllParam);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(ListofSequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with blosum SM
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignListSequencesBlosum()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapBlosumAlignAlgorithmNodeName,
                                             AlignParameters.AlignList);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with blosum SM
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignAllParamBlosum()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapBlosumAlignAlgorithmNodeName,
                                             AlignParameters.AllParam);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(ListofSequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with Pam SM
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignListSequencesPam()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapPamAlignAlgorithmNodeName,
                                             AlignParameters.AlignList);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with Pam SM
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignAllParamPam()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapPamAlignAlgorithmNodeName,
                                             AlignParameters.AllParam);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(ListofSequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with Similarity Matrix passed as Text reader
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignListSequencesSimMatTextRead()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapProAlignAlgorithmNodeName,
                                             AlignParameters.AlignList,
                                             SimilarityMatrixParameters.TextReader);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with Similarity Matrix passed as Text Reader
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignAllParamSimMatTextRead()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapProAlignAlgorithmNodeName,
                                             AlignParameters.AllParam,
                                             SimilarityMatrixParameters.TextReader);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(ListofSequences)
        ///     and validate if the aligned sequence is as expected and 6
        ///     also validate the score for the same
        ///     Input : FastA Dna File Diagonal Matrix
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignListSequencesDiagonalSimMat()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapDiagonalSimMatAlignAlgorithmNodeName,
                                             AlignParameters.AlignList,
                                             SimilarityMatrixParameters.DiagonalMatrix);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Dna File Diagonal Matrix
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignAllParamDiagonalSimMat()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapDiagonalSimMatAlignAlgorithmNodeName,
                                             AlignParameters.AllParam,
                                             SimilarityMatrixParameters.DiagonalMatrix);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(two sequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA DNA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignTwoDnaSequences()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapDnaAlignAlgorithmNodeName,
                                             AlignParameters.AlignTwo);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a xml file using the method Align(two sequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA DNA sequence
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignTwoDnaSequencesFromXml()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapDnaAlignAlgorithmNodeName,
                                             AlignParameters.AlignTwoCode);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a xml file using the method Align(list of sequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA DNA sequence
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignListDnaSequencesFromXml()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapDnaAlignAlgorithmNodeName,
                                             AlignParameters.AlignListCode);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a xml file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA DNA sequence
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignAllParamDnaFromXml()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapDnaAlignAlgorithmNodeName,
                                             AlignParameters.AllParamCode);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(two sequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA RNA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignTwoRnaSequences()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapRnaAlignAlgorithmNodeName,
                                             AlignParameters.AlignTwo);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a xml file using the method Align(two sequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA RNA sequence
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignTwoRnaSequencesFromXml()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapRnaAlignAlgorithmNodeName,
                                             AlignParameters.AlignTwoCode);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a xml file using the method Align(list of sequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA RNA sequence
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignListRnaSequencesFromXml()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapRnaAlignAlgorithmNodeName,
                                             AlignParameters.AlignListCode);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a xml file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA RNA sequence
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignAllParamRnaFromXml()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapRnaAlignAlgorithmNodeName,
                                             AlignParameters.AllParamCode);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(two sequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignTwoProSequences()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapProAlignAlgorithmNodeName,
                                             AlignParameters.AlignTwo);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a xml file using the method Align(two sequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein sequence
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignTwoProSequencesFromXml()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapProAlignAlgorithmNodeName,
                                             AlignParameters.AlignTwoCode);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a xml file using the method Align(list of sequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein sequence
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignListProSequencesFromXml()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapProAlignAlgorithmNodeName,
                                             AlignParameters.AlignListCode);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a xml file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein sequence
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignAllParamProFromXml()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapProAlignAlgorithmNodeName,
                                             AlignParameters.AllParamCode);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(two sequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with Max Gap Cost
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignTwoSequencesGapCostMax()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapGapCostMaxAlignAlgorithmNodeName,
                                             AlignParameters.AlignTwo);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a xml file using the method Align(two sequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein Sequence with Max Gap Cost
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignTwoSequencesGapCostMaxFromXml()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapGapCostMaxAlignAlgorithmNodeName,
                                             AlignParameters.AlignTwoCode);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a xml file using the method Align(list of sequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein Sequence with Max Gap Cost
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignSequenceListGapCostMaxFromXml()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapGapCostMaxAlignAlgorithmNodeName,
                                             AlignParameters.AlignListCode);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a xml file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein Sequence with Max Gap Cost
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignAllParamGapCostMaxFromXml()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapGapCostMaxAlignAlgorithmNodeName,
                                             AlignParameters.AllParamCode);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(two sequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with Min Gap Cost
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignTwoSequencesGapCostMin()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapGapCostMinAlignAlgorithmNodeName,
                                             AlignParameters.AlignTwo);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a xml file using the method Align(two sequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein Sequence with Min Gap Cost
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignTwoSequencesGapCostMinFromXml()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapGapCostMinAlignAlgorithmNodeName,
                                             AlignParameters.AlignTwoCode);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a xml file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein Sequence with Min Gap Cost
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignAllParamGapCostMinFromXml()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapGapCostMinAlignAlgorithmNodeName,
                                             AlignParameters.AllParamCode);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(Two Sequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with blosum SM
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignTwoSequencesBlosum()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapBlosumAlignAlgorithmNodeName,
                                             AlignParameters.AlignTwo);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(two Sequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with Pam SM
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignTwoSequencesPam()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapPamAlignAlgorithmNodeName,
                                             AlignParameters.AlignTwo);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(two sequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with Similarity Matrix passed as Text reader
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignTwoSequencesSimMatTextRead()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapProAlignAlgorithmNodeName,
                                             AlignParameters.AlignTwo,
                                             SimilarityMatrixParameters.TextReader);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(two Sequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Dna File Diagonal Matrix
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapSimpleAlignTwoSequencesDiagonalSimMat()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapDiagonalSimMatAlignAlgorithmNodeName,
                                             AlignParameters.AlignTwo,
                                             SimilarityMatrixParameters.DiagonalMatrix);
        }

        #region Gap Extension Cost inclusion Test cases

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Cost, Gap Extension, Similarity Matrix
        ///     which is in a text file using the method Align(ListofSequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA DNA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapAlignListSequencesDna()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapDnaAlignAlgorithmNodeName,
                                             AlignParameters.AlignList, SimilarityMatrixParameters.Default,
                                             AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Cost, Gap Extension, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA DNA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapAlignAllParamDna()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapDnaAlignAlgorithmNodeName,
                                             AlignParameters.AllParam, SimilarityMatrixParameters.Default,
                                             AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Cost, Gap Extension, Similarity Matrix
        ///     which is in a text file using the method Align(ListofSequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapAlignListSequencesPro()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapProAlignAlgorithmNodeName,
                                             AlignParameters.AlignList, SimilarityMatrixParameters.Default,
                                             AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Cost, Gap Extension, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapAlignAllParamPro()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapProAlignAlgorithmNodeName,
                                             AlignParameters.AllParam, SimilarityMatrixParameters.Default,
                                             AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Cost, Gap Extension, Similarity Matrix
        ///     which is in a text file using the method Align(ListofSequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Rna File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapAlignListSequencesRna()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapRnaAlignAlgorithmNodeName,
                                             AlignParameters.AlignList, SimilarityMatrixParameters.Default,
                                             AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Cost, Gap Extension, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Rna File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapAlignAllParamRna()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapRnaAlignAlgorithmNodeName,
                                             AlignParameters.AllParam, SimilarityMatrixParameters.Default,
                                             AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Cost, Gap Extension, Similarity Matrix
        ///     which is in a text file using the method Align(ListofSequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with Max Gap Cost
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapAlignListSequencesGapCostMax()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapGapCostMaxAlignAlgorithmNodeName,
                                             AlignParameters.AlignList, SimilarityMatrixParameters.Default,
                                             AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Cost, Gap Extension, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with Max Gap Cost
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapAlignAllParamGapCostMax()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapGapCostMaxAlignAlgorithmNodeName,
                                             AlignParameters.AllParam, SimilarityMatrixParameters.Default,
                                             AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Cost, Gap Extension, Similarity Matrix
        ///     which is in a text file using the method Align(ListofSequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with Min Gap Cost
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapAlignListSequencesGapCostMin()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapGapCostMinAlignAlgorithmNodeName,
                                             AlignParameters.AlignList, SimilarityMatrixParameters.Default,
                                             AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Cost, Gap Extension, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with Min Gap Cost
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapAlignAllParamGapCostMin()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapGapCostMinAlignAlgorithmNodeName,
                                             AlignParameters.AllParam, SimilarityMatrixParameters.Default,
                                             AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Cost, Gap Extension, Similarity Matrix
        ///     which is in a text file using the method Align(ListofSequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with blosum SM
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapAlignListSequencesBlosum()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapBlosumAlignAlgorithmNodeName,
                                             AlignParameters.AlignList, SimilarityMatrixParameters.Default,
                                             AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Cost, Gap Extension, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with blosum SM
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapAlignAllParamBlosum()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapBlosumAlignAlgorithmNodeName,
                                             AlignParameters.AllParam, SimilarityMatrixParameters.Default,
                                             AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Cost, Gap Extension, Similarity Matrix
        ///     which is in a text file using the method Align(ListofSequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with Pam SM
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapAlignListSequencesPam()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapPamAlignAlgorithmNodeName,
                                             AlignParameters.AlignList, SimilarityMatrixParameters.Default,
                                             AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Cost, Gap Extension, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with Pam SM
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapAlignAllParamPam()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapPamAlignAlgorithmNodeName,
                                             AlignParameters.AllParam, SimilarityMatrixParameters.Default,
                                             AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Cost, Gap Extension, Similarity Matrix
        ///     which is in a text file using the method Align(ListofSequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with Similarity Matrix passed as Text reader
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapAlignListSequencesSimMatTextRead()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapProAlignAlgorithmNodeName,
                                             AlignParameters.AlignList,
                                             SimilarityMatrixParameters.TextReader, AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Cost, Gap Extension, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Protein File with Similarity Matrix passed as Text Reader
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapAlignAllParamSimMatTextRead()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapProAlignAlgorithmNodeName,
                                             AlignParameters.AllParam,
                                             SimilarityMatrixParameters.TextReader, AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Cost, Gap Extension, Similarity Matrix
        ///     which is in a text file using the method Align(ListofSequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Dna File Diagonal Matrix
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapAlignListSequencesDiagonalSimMat()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapDiagonalSimMatAlignAlgorithmNodeName,
                                             AlignParameters.AlignList,
                                             SimilarityMatrixParameters.DiagonalMatrix, AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Cost, Gap Extension, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Dna File Diagonal Matrix
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapAlignAllParamDiagonalSimMat()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapDiagonalSimMatAlignAlgorithmNodeName,
                                             AlignParameters.AllParam,
                                             SimilarityMatrixParameters.DiagonalMatrix, AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Cost = Gap Extension, Similarity Matrix
        ///     which is in a text file using the method Align(ListofSequences)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Dna File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapAlignListSequencesGapCostGapExtensionEqual()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapEqualAlignAlgorithmNodeName,
                                             AlignParameters.AlignList,
                                             SimilarityMatrixParameters.Default, AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Cost = Gap Extension, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA Dna File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PairwiseOverlapAlignAllParamGapCostGapExtensionEqual()
        {
            this.ValidatePairwiseOverlapAlignment(Constants.PairwiseOverlapEqualAlignAlgorithmNodeName,
                                             AlignParameters.AllParam,
                                             SimilarityMatrixParameters.Default, AlignmentType.Align);
        }

        #endregion Gap Extension Cost inclusion Test cases

        #endregion PairwiseOverlapAligner P1 Test cases

        #region Supporting Methods

        /// <summary>
        ///     Validates PairwiseOverlapAlignment algorithm for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Node Name in the xml.</param>
        /// <param name="alignParam">parameter based on which certain validations are done.</param>
        private void ValidatePairwiseOverlapAlignment(string nodeName, AlignParameters alignParam)
        {
            this.ValidatePairwiseOverlapAlignment(nodeName, alignParam, SimilarityMatrixParameters.Default);
        }

        /// <summary>
        ///     Validates PairwiseOverlapAlignment algorithm for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Node Name in the xml.</param>
        /// <param name="alignParam">parameter based on which certain validations are done.</param>
        /// <param name="similarityMatrixParam">Similarity Matrix Parameter.</param>
        private void ValidatePairwiseOverlapAlignment(string nodeName, AlignParameters alignParam,
                                                      SimilarityMatrixParameters similarityMatrixParam)
        {
            this.ValidatePairwiseOverlapAlignment(nodeName, alignParam,
                                             similarityMatrixParam, AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Validates PairwiseOverlapAlignment algorithm for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Node Name in the xml.</param>
        /// <param name="alignParam">parameter based on which certain validations are done.</param>
        /// <param name="similarityMatrixParam">Similarity Matrix Parameter.</param>
        /// <param name="alignType">Alignment Type</param>
        private void ValidatePairwiseOverlapAlignment(string nodeName, AlignParameters alignParam,
                                                      SimilarityMatrixParameters similarityMatrixParam,
                                                      AlignmentType alignType)
        {
            ISequence aInput;
            ISequence bInput;

            IAlphabet alphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.AlphabetNameNode));

            if (alignParam.ToString().Contains("Code"))
            {
                string sequence1 = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode1);
                string sequence2 = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode2);

                aInput = new Sequence(alphabet, sequence1);
                bInput = new Sequence(alphabet, sequence2);
            }
            else
            {
                // Read the xml file for getting both the files for aligning.
                string filePath1 = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode1);
                string filePath2 = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode2);

                var parser1 = new FastAParser { Alphabet = alphabet };
                aInput = parser1.Parse(filePath1).ElementAt(0);
                bInput = parser1.Parse(filePath2).ElementAt(0);
            }

            string blosumFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.BlosumFilePathNode);
            SimilarityMatrix sm;

            switch (similarityMatrixParam)
            {
                case SimilarityMatrixParameters.TextReader:
                    using (TextReader reader = new StreamReader(blosumFilePath))
                        sm = new SimilarityMatrix(reader);
                    break;
                case SimilarityMatrixParameters.DiagonalMatrix:
                    string matchValue = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MatchScoreNode);
                    string misMatchValue = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MisMatchScoreNode);
                    sm = new DiagonalSimilarityMatrix(int.Parse(matchValue, null), int.Parse(misMatchValue, null));
                    break;
                default:
                    sm = new SimilarityMatrix(new StreamReader(blosumFilePath));
                    break;
            }

            int gapOpenCost = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.GapOpenCostNode), null);
            int gapExtensionCost = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.GapExtensionCostNode), null);

            var pairwiseOverlapObj = new PairwiseOverlapAligner();
            if (AlignParameters.AllParam != alignParam)
            {
                pairwiseOverlapObj.SimilarityMatrix = sm;
                pairwiseOverlapObj.GapOpenCost = gapOpenCost;
            }

            IList<IPairwiseSequenceAlignment> result = null;

            switch (alignParam)
            {
                case AlignParameters.AlignList:
                case AlignParameters.AlignListCode:
                    var sequences = new List<ISequence> {aInput, bInput};
                    switch (alignType)
                    {
                        case AlignmentType.Align:
                            pairwiseOverlapObj.GapExtensionCost = gapExtensionCost;
                            result = pairwiseOverlapObj.Align(sequences);
                            break;
                        default:
                            result = pairwiseOverlapObj.AlignSimple(sequences);
                            break;
                    }
                    break;
                case AlignParameters.AllParam:
                case AlignParameters.AllParamCode:
                    switch (alignType)
                    {
                        case AlignmentType.Align:
                            pairwiseOverlapObj.GapExtensionCost = gapExtensionCost;
                            result = pairwiseOverlapObj.Align(sm, gapOpenCost, gapExtensionCost, aInput, bInput);
                            break;
                        default:
                            result = pairwiseOverlapObj.AlignSimple(sm, gapOpenCost, aInput, bInput);
                            break;
                    }
                    break;
                case AlignParameters.AlignTwo:
                case AlignParameters.AlignTwoCode:
                    switch (alignType)
                    {
                        case AlignmentType.Align:
                            pairwiseOverlapObj.GapExtensionCost = gapExtensionCost;
                            result = pairwiseOverlapObj.Align(aInput, bInput);
                            break;
                        default:
                            result = pairwiseOverlapObj.AlignSimple(aInput, bInput);
                            break;
                    }
                    break;
                default:
                    break;
            }

            // Read the xml file for getting both the files for aligning.
            string expectedSequence1;
            string expectedSequence2;
            string expectedScore;

            switch (alignType)
            {
                case AlignmentType.Align:
                    expectedScore = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedGapExtensionScoreNode);
                    expectedSequence1 = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedGapExtensionSequence1Node);
                    expectedSequence2 = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedGapExtensionSequence2Node);
                    break;
                default:
                    expectedScore = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedScoreNode);
                    expectedSequence1 = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode1);
                    expectedSequence2 = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode2);
                    break;
            }

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            var seperators = new [] {';'};
            string[] expectedSequences1 = expectedSequence1.Split(seperators);
            string[] expectedSequences2 = expectedSequence2.Split(seperators);

            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            for (int i = 0; i < expectedSequences1.Length; i++)
            {
                PairwiseAlignedSequence alignedSeq = new PairwiseAlignedSequence
                {
                    FirstSequence = new Sequence(alphabet, expectedSequences1[i]),
                    SecondSequence = new Sequence(alphabet, expectedSequences2[i]),
                    Score = Convert.ToInt32(expectedScore, null),
                    FirstOffset = Int32.MinValue,
                    SecondOffset = Int32.MinValue,
                };
                align.PairwiseAlignedSequences.Add(alignedSeq);
            }
            expectedOutput.Add(align);

            Assert.IsTrue(AlignmentHelpers.CompareAlignment(result, expectedOutput, true));

            ApplicationLog.WriteLine(string.Format(null, "PairwiseOverlapAligner P1 : Final Score '{0}'.", expectedScore));
            ApplicationLog.WriteLine(string.Format(null, "PairwiseOverlapAligner P1 : Aligned First Sequence is '{0}'.", expectedSequence1));
            ApplicationLog.WriteLine(string.Format(null, "PairwiseOverlapAligner P1 : Aligned Second Sequence is '{0}'.", expectedSequence2));
        }
        #endregion Supporting Methods
    }
}