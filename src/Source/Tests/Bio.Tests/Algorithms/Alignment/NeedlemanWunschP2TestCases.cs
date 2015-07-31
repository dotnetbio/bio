using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;

using Bio.Algorithms.Alignment;
using Bio.Extensions;
using Bio.IO.FastA;
using Bio.SimilarityMatrices;
using Bio.TestAutomation.Util;
using Bio.Tests.Framework;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests.Algorithms.Alignment
{
    /// <summary>
    ///     NeedlemanWunschAlignment algorithm P2 test cases
    /// </summary>
    [TestFixture]
    public class NeedlemanWunschP2TestCases
    {
        #region Enums

        /// <summary>
        ///     Alignment Parameters which are used for different test cases
        ///     based on which the test cases are executed.
        /// </summary>
        private enum AlignParameters
        {
            AlignList,
            AllParam,
            AlignTwo,
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
        ///     Types of invalid sequence
        /// </summary>
        private enum InvalidSequenceType
        {
            SequenceWithSpecialChars,
            AlphabetMap,
            EmptySequence,
            SequenceWithInvalidChars,
            InvalidSequence,
            SequenceWithSpaces,
            SequenceWithGap,
            SequenceWithUnicodeChars,
            Default
        }

        /// <summary>
        ///     Input sequences to get aligned in different cases.
        /// </summary>
        private enum SequenceCaseType
        {
            LowerCase,
            UpperCase,
            LowerUpperCase,
            Default
        }

        /// <summary>
        ///     Types of invalid similarity matrix
        /// </summary>
        private enum SimilarityMatrixInvalidTypes
        {
            NonMatchingSimilarityMatrix,
            EmptySimilaityMatrix,
            OnlyAlphabetSimilarityMatrix,
            FewAlphabetsSimilarityMatrix,
            ModifiedSimilarityMatrix,
            NullSimilarityMatrix,
            EmptySequence,
            ExpectedErrorMessage,
        }

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

        private readonly Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #region Test Cases

        /// <summary>
        ///     Pass a Valid Sequence(Lower case) with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(sequence1, sequence2)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void NeedlemanWunschSimpleAlignTwoLowerCaseSequencesFromTextFile()
        {
            this.ValidateNeedlemanWunschAlignment(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                SequenceCaseType.LowerCase,
                AlignParameters.AlignTwo);
        }

        /// <summary>
        ///     Pass a Valid Sequence(Upper case) with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(sequence1, sequence2)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void NeedlemanWunschSimpleAlignTwoUpperCaseSequencesFromTextFile()
        {
            this.ValidateNeedlemanWunschAlignment(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                SequenceCaseType.UpperCase,
                AlignParameters.AlignTwo);
        }

        /// <summary>
        ///     Pass a Valid 1000 BP Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(sequence1, sequence2)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void NeedlemanWunschSimpleAlignWith1000BP()
        {
            this.ValidateNeedlemanWunschAlignment(
                Constants.NeedlemanWunschAlignAlgorithmNodeNameFor1000BP,
                true,
                SequenceCaseType.UpperCase,
                AlignParameters.AlignTwo);
        }

        /// <summary>
        ///     Pass a Valid Sequence(Lower and Upper case) with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(sequence1, sequence2)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void NeedlemanWunschSimpleAlignTwoLowerUpperCaseSequencesFromTextFile()
        {
            this.ValidateNeedlemanWunschAlignment(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                SequenceCaseType.LowerUpperCase,
                AlignParameters.AlignTwo);
        }

        /// <summary>
        ///     Pass a Valid Sequence(Lower case) with valid GapPenalty, Similarity Matrix
        ///     from code using the method Align(sequence1, sequence2)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void NeedlemanWunschSimpleAlignTwoLowerCaseSequencesFromCode()
        {
            this.ValidateNeedlemanWunschAlignment(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                false,
                SequenceCaseType.LowerCase,
                AlignParameters.AlignTwo);
        }

        /// <summary>
        ///     Pass a Valid Sequence(Upper case) with valid GapPenalty, Similarity Matrix
        ///     from code using the method Align(sequence1, sequence2)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void NeedlemanWunschSimpleAlignTwoUpperCaseSequencesFromCode()
        {
            this.ValidateNeedlemanWunschAlignment(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                false,
                SequenceCaseType.UpperCase,
                AlignParameters.AlignTwo);
        }

        /// <summary>
        ///     Pass a Valid Sequence(Lower and Upper case) with valid GapPenalty, Similarity Matrix
        ///     from code using the method Align(sequence1, sequence2)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void NeedlemanWunschSimpleAlignTwoLowerUpperCaseSequencesFromCode()
        {
            this.ValidateNeedlemanWunschAlignment(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                false,
                SequenceCaseType.LowerUpperCase,
                AlignParameters.AlignTwo);
        }

        /// <summary>
        ///     Pass a Valid Sequence(Lower case) with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method AlignList
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void NeedlemanWunschSimpleAlignListLowerCaseSequencesFromTextFile()
        {
            this.ValidateNeedlemanWunschAlignment(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                SequenceCaseType.LowerCase,
                AlignParameters.AlignList);
        }

        /// <summary>
        ///     Pass a Valid Sequence(Upper case) with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method AlignList
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void NeedlemanWunschSimpleAlignListUpperCaseSequencesFromTextFile()
        {
            this.ValidateNeedlemanWunschAlignment(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                SequenceCaseType.UpperCase,
                AlignParameters.AlignList);
        }

        /// <summary>
        ///     Pass a Valid Sequence(Lower and Upper case) with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method AlignList
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void NeedlemanWunschSimpleAlignListLowerUpperCaseSequencesFromTextFile()
        {
            this.ValidateNeedlemanWunschAlignment(Constants.NeedlemanWunschAlignAlgorithmNodeName, true,
                                             SequenceCaseType.LowerUpperCase,
                                             AlignParameters.AlignList);
        }

        /// <summary>
        ///     Pass a Valid Sequence(Lower case)  with valid GapPenalty, Similarity Matrix
        ///     from text file using the method AlignList
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void NeedlemanWunschSimpleAlignAllParamsLowerCaseSequencesFromTextFile()
        {
            this.ValidateNeedlemanWunschAlignment(Constants.NeedlemanWunschAlignAlgorithmNodeName, true,
                                             SequenceCaseType.LowerCase,
                                             AlignParameters.AllParam);
        }

        /// <summary>
        ///     Pass a Valid Sequence(Upper case) with valid GapPenalty, Similarity Matrix
        ///     from text file using the method AlignList
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void NeedlemanWunschSimpleAlignAllParamsUpperCaseSequencesFromTextFile()
        {
            this.ValidateNeedlemanWunschAlignment(Constants.NeedlemanWunschAlignAlgorithmNodeName, true,
                                             SequenceCaseType.UpperCase,
                                             AlignParameters.AllParam);
        }

        /// <summary>
        ///     Pass a Valid Sequence(Lower and Upper case) with valid GapPenalty, Similarity Matrix
        ///     from text file using the method AlignList
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void NeedlemanWunschSimpleAlignAllParamsLowerUpperCaseSequencesFromTextFile()
        {
            this.ValidateNeedlemanWunschAlignment(Constants.NeedlemanWunschAlignAlgorithmNodeName, true,
                                             SequenceCaseType.LowerUpperCase, AlignParameters.AllParam);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Non Matching)
        ///     from text file and validate if Align(se1,seq2) throws expected exception
        ///     Input : Two Input sequence and Non Matching similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignTwoSequencesWithNonMatchingSimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName, true,
                SimilarityMatrixInvalidTypes.NonMatchingSimilarityMatrix, AlignParameters.AlignTwo,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Non Matching)
        ///     from text file and validate if Align using List throws expected exception
        ///     Input : Input sequence List and Non Matching similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignListSequencesWithNonMatchingSimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName, true,
                SimilarityMatrixInvalidTypes.NonMatchingSimilarityMatrix, AlignParameters.AlignList,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Non Matching)
        ///     from text file and validate if Align using all params throws expected exception
        ///     Input : Input sequence and Non Matching similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignAllParamsSequencesWithNonMatchingSimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName, true,
                SimilarityMatrixInvalidTypes.NonMatchingSimilarityMatrix, AlignParameters.AllParam,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Empty)
        ///     from text file and validate if Align(se1,seq2) throws expected exception
        ///     Input : Two Input sequence and Empty similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignTwoSequencesWithEmptySimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName, true,
                SimilarityMatrixInvalidTypes.EmptySimilaityMatrix, AlignParameters.AlignTwo,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Empty)
        ///     from text file and validate if Align using List throws expected exception
        ///     Input : Input sequence List and Empty similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignListSequencesWithEmptySimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName, true,
                SimilarityMatrixInvalidTypes.EmptySimilaityMatrix,
                AlignParameters.AlignList,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Empty)
        ///     from text file and validate if Align using all params throws expected exception
        ///     Input : Input sequence and Empty similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignAllParamsSequencesWithEmptySimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                SimilarityMatrixInvalidTypes.EmptySimilaityMatrix,
                AlignParameters.AllParam,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Only Alphabet)
        ///     from text file and validate if Align(se1,seq2) throws expected exception
        ///     Input : Two Input sequence and Only Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignTwoSequencesWithOnlyAlphabetSimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName, true,
                SimilarityMatrixInvalidTypes.OnlyAlphabetSimilarityMatrix,
                AlignParameters.AlignTwo,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Only Alphabet)
        ///     from text file and validate if Align using List throws expected exception
        ///     Input : Input sequence List and Only Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignListSequencesWithOnlyAlphabetSimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                SimilarityMatrixInvalidTypes.OnlyAlphabetSimilarityMatrix,
                AlignParameters.AlignList,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Only Alphabet)
        ///     from text file and validate if Align using all params throws expected exception
        ///     Input : Input sequence and Only Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignAllParamsSequencesWithOnlyAlphabetSimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                SimilarityMatrixInvalidTypes.OnlyAlphabetSimilarityMatrix,
                AlignParameters.AllParam,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Modified)
        ///     from text file and validate if Align(se1,seq2) throws expected exception
        ///     Input : Two Input sequence and Modified similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignTwoSequencesWithModifiedSimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                SimilarityMatrixInvalidTypes.ModifiedSimilarityMatrix,
                AlignParameters.AlignTwo,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Modified)
        ///     from text file and validate if Align using list throws expected exception
        ///     Input : Input sequence list and Modified similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignListSequencesWithModifiedSimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                SimilarityMatrixInvalidTypes.ModifiedSimilarityMatrix,
                AlignParameters.AlignList,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Modified)
        ///     from text file and validate if Align using all params throws expected exception
        ///     Input : Input sequence and Modified similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignAllParamsSequencesWithModifiedSimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                SimilarityMatrixInvalidTypes.ModifiedSimilarityMatrix,
                AlignParameters.AllParam,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Few Alphabet)
        ///     from text file and validate if Align(se1,seq2) throws expected exception
        ///     Input : Two Input sequence and Few Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignTwoSequencesWithFewAlphabetSimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                SimilarityMatrixInvalidTypes.FewAlphabetsSimilarityMatrix,
                AlignParameters.AlignTwo,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Few Alphabet)
        ///     from text file and validate if Align using list throws expected exception
        ///     Input : Input sequence list and Few Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignListSequencesWithFewAlphabetSimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                SimilarityMatrixInvalidTypes.FewAlphabetsSimilarityMatrix,
                AlignParameters.AlignList,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Few Alphabet)
        ///     from text file and validate if Align using all params throws expected exception
        ///     Input : Input sequence and Few Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignAllParamsSequencesWithFewAlphabetSimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                SimilarityMatrixInvalidTypes.FewAlphabetsSimilarityMatrix,
                AlignParameters.AllParam,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Empty)
        ///     from code and validate if Align(se1,seq2) throws expected exception
        ///     Input : Two Input sequence and Empty similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignTwoSequencesFromCodeWithEmptySimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                false,
                SimilarityMatrixInvalidTypes.EmptySimilaityMatrix,
                AlignParameters.AlignTwo,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Empty)
        ///     from text file and validate if Align using List throws expected exception
        ///     Input : Input sequence List and Empty similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignListSequencesFromCodeWithEmptySimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                false,
                SimilarityMatrixInvalidTypes.EmptySimilaityMatrix,
                AlignParameters.AlignList,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Empty)
        ///     from code and validate if Align using all params throws expected exception
        ///     Input : Input sequence List and Empty similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignAllParamsSequencesFromCodeWithEmptySimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                false,
                SimilarityMatrixInvalidTypes.EmptySimilaityMatrix,
                AlignParameters.AllParam,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Only Alphabet)
        ///     from code and validate if Align(seq1,seq2) throws expected exception
        ///     Input : Input sequence List and Only Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignTwoSequencesFromCodeWithOnlyAlphabetSimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                false,
                SimilarityMatrixInvalidTypes.OnlyAlphabetSimilarityMatrix,
                AlignParameters.AlignTwo,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Only Alphabet)
        ///     from code and validate if Align using list throws expected exception
        ///     Input : Input sequence List and Only Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignListSequencesFromCodeWithOnlyAlphabetSimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                false,
                SimilarityMatrixInvalidTypes.OnlyAlphabetSimilarityMatrix,
                AlignParameters.AlignList,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Only Alphabet)
        ///     from code and validate if Align using all params throws expected exception
        ///     Input : Input sequence List and Only Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignAllParamsSequencesFromCodeWithOnlyAlphabetSimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                false,
                SimilarityMatrixInvalidTypes.OnlyAlphabetSimilarityMatrix,
                AlignParameters.AllParam,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Null)
        ///     from code and validate if Align using all params throws expected exception
        ///     Input : Input sequence and Few Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignTwoSequencesFromCodeWithNullSimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                false,
                SimilarityMatrixInvalidTypes.NullSimilarityMatrix,
                AlignParameters.AlignTwo,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Null)
        ///     from code and validate if Align using all params throws expected exception
        ///     Input : Input sequence and Few Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignListSequencesFromCodeWithNullSimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                false,
                SimilarityMatrixInvalidTypes.NullSimilarityMatrix,
                AlignParameters.AlignList,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix (Null)
        ///     from code and validate if Align using all params throws expected exception
        ///     Input : Input sequence and Few Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignAllParamsSequencesFromCodeWithNullSimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                false,
                SimilarityMatrixInvalidTypes.NullSimilarityMatrix,
                AlignParameters.AllParam,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Invalid DiagonalSimilarityMatrix
        ///     from text file and validate if Align using all params throws expected exception
        ///     Input : Input sequence and Few Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignTwoSequencesWithInvalidDiagonalSimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschDiagonalSimMatAlignAlgorithmNodeName,
                true,
                SimilarityMatrixInvalidTypes.NonMatchingSimilarityMatrix,
                AlignParameters.AlignTwo,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Invalid DiagonalSimilarityMatrix
        ///     from text file and validate if Align using all params throws expected exception
        ///     Input : Input sequence and Invalid DiagonalSimilarityMatrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignListSequencesWithInvalidDiagonalSimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschDiagonalSimMatAlignAlgorithmNodeName,
                true,
                SimilarityMatrixInvalidTypes.NonMatchingSimilarityMatrix,
                AlignParameters.AlignList,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Invalid DiagonalSimilarityMatrix
        ///     from text file and validate if Align using all params throws expected exception
        ///     Input : Input sequence and Invalid DiagonalSimilarityMatrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignAllParamsSequencesWithInvalidDiagonalSimilarityMatrix()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
                Constants.NeedlemanWunschDiagonalSimMatAlignAlgorithmNodeName,
                true,
                SimilarityMatrixInvalidTypes.NonMatchingSimilarityMatrix,
                AlignParameters.AllParam,
                AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Pass a In Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     from text file and validate if Parser throws expected exception
        ///     Input : Input sequence List and Only Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignTwoWithInvalidSequencesFromTextFile()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSequence(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                InvalidSequenceType.SequenceWithSpecialChars,
                AlignParameters.AlignTwo,
                AlignmentType.SimpleAlign,
                InvalidSequenceType.InvalidSequence);
        }

        /// <summary>
        ///     Pass a In Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     from text file and validate if Parser throws expected exception
        ///     Input : Input sequence List and Only Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignListWithInvalidSequencesFromTextFile()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSequence(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                InvalidSequenceType.SequenceWithSpecialChars,
                AlignParameters.AlignList,
                AlignmentType.SimpleAlign,
                InvalidSequenceType.InvalidSequence);
        }

        /// <summary>
        ///     Pass a In Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     from text file and validate if Parser throws expected exception
        ///     Input : Input sequence List and Only Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignAllParamsWithInvalidSequencesFromTextFile()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSequence(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                InvalidSequenceType.SequenceWithSpecialChars,
                AlignParameters.AllParam,
                AlignmentType.SimpleAlign,
                InvalidSequenceType.InvalidSequence);
        }

        /// <summary>
        ///     Pass Empty Sequence with valid GapPenalty, Similarity Matrix
        ///     from text file and validate if Parser throws expected exception
        ///     Input : Input sequence List and Only Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignTwoWithEmptySequencesFromTextFile()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSequence(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                InvalidSequenceType.EmptySequence,
                AlignParameters.AlignTwo,
                AlignmentType.SimpleAlign,
                InvalidSequenceType.SequenceWithInvalidChars);
        }

        /// <summary>
        ///     Pass Empty Sequence with valid GapPenalty, Similarity Matrix
        ///     from text file and validate if Parser throws expected exception
        ///     Input : Input sequence List and Only Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignListWithEmptySequencesFromTextFile()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSequence(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                InvalidSequenceType.EmptySequence,
                AlignParameters.AlignList,
                AlignmentType.SimpleAlign,
                InvalidSequenceType.SequenceWithInvalidChars);
        }

        /// <summary>
        ///     Pass Empty Sequence with valid GapPenalty, Similarity Matrix
        ///     from text file and validate if Parser throws expected exception
        ///     Input : Input sequence List and Only Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignAllParamsWithEmptySequencesFromTextFile()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSequence(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                InvalidSequenceType.EmptySequence,
                AlignParameters.AllParam,
                AlignmentType.SimpleAlign,
                InvalidSequenceType.SequenceWithInvalidChars);
        }

        /// <summary>
        ///     Pass invalid Sequence(Contains Gap) with valid GapPenalty, Similarity Matrix
        ///     from text file and validate if Parser throws expected exception
        ///     Input : Input sequence List and Only Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignTwoWithGapSequencesFromTextFile()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSequence(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                InvalidSequenceType.SequenceWithGap,
                AlignParameters.AlignTwo,
                AlignmentType.SimpleAlign,
                InvalidSequenceType.Default);
        }

        /// <summary>
        ///     Pass invalid Sequence(Contains Gap) with valid GapPenalty, Similarity Matrix
        ///     from text file and validate if Align using all params throws expected exception
        ///     Input : Input sequence List and Only Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignListWithGapSequencesFromTextFile()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSequence(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                InvalidSequenceType.SequenceWithGap,
                AlignParameters.AlignList,
                AlignmentType.SimpleAlign,
                InvalidSequenceType.Default);
        }

        /// <summary>
        ///     Pass invalid Sequence(Contains Gap) with valid GapPenalty, Similarity Matrix
        ///     from text file and validate if Align using all params throws expected exception
        ///     Input : Input sequence List and Only Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignAllParamsWithGapSequencesFromTextFile()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSequence(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                InvalidSequenceType.SequenceWithGap,
                AlignParameters.AllParam,
                AlignmentType.SimpleAlign,
                InvalidSequenceType.Default);
        }

        /// <summary>
        ///     Pass invalid Sequence(Unicode) with valid GapPenalty, Similarity Matrix
        ///     from text file and validate if Align using all params throws expected exception
        ///     Input : Input sequence List and Only Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignTwoWithUnicodeSequencesFromTextFile()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSequence(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                InvalidSequenceType.SequenceWithUnicodeChars,
                AlignParameters.AlignTwo,
                AlignmentType.SimpleAlign,
                InvalidSequenceType.SequenceWithUnicodeChars);
        }

        /// <summary>
        ///     Pass invalid Sequence(Unicode) with valid GapPenalty, Similarity Matrix
        ///     from text file and validate if Align using all params throws expected exception
        ///     Input : Input sequence List and Only Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignListWithUnicodeSequencesFromTextFile()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSequence(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                InvalidSequenceType.SequenceWithUnicodeChars,
                AlignParameters.AlignList,
                AlignmentType.SimpleAlign,
                InvalidSequenceType.SequenceWithUnicodeChars);
        }

        /// <summary>
        ///     Pass invalid Sequence(Unicode) with valid GapPenalty, Similarity Matrix
        ///     from text file and validate if Align using all params throws expected exception
        ///     Input : Input sequence List and Only Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignAllParamsWithUnicodeSequencesFromTextFile()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSequence(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                InvalidSequenceType.SequenceWithUnicodeChars,
                AlignParameters.AllParam,
                AlignmentType.SimpleAlign,
                InvalidSequenceType.SequenceWithUnicodeChars);
        }

        /// <summary>
        ///     Pass invalid Sequence(Spaces) with valid GapPenalty, Similarity Matrix
        ///     from text file and validate if Align using all params throws expected exception
        ///     Input : Input sequence List and Only Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignTwoSequencesWithSpacesFromTextFile()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSequence(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                InvalidSequenceType.SequenceWithSpaces,
                AlignParameters.AlignTwo,
                AlignmentType.SimpleAlign,
                InvalidSequenceType.SequenceWithSpaces);
        }

        /// <summary>
        ///     Pass invalid Sequence(Spaces) with valid GapPenalty, Similarity Matrix
        ///     from text file and validate if Align using all params throws expected exception
        ///     Input : Input sequence List and Only Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignListSequencesWithSpacesFromTextFile()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSequence(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                InvalidSequenceType.SequenceWithSpaces,
                AlignParameters.AlignList,
                AlignmentType.SimpleAlign,
                InvalidSequenceType.SequenceWithSpaces);
        }

        /// <summary>
        ///     Pass invalid Sequence(Spaces) with valid GapPenalty, Similarity Matrix
        ///     from text file and validate if Align using all params throws expected exception
        ///     Input : Input sequence List and Only Alphabet similarity matrix
        ///     Validation : Exception should be thrown
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateNWSimpleAlignParamsSequencesWithSpacesFromTextFile()
        {
            this.InValidateNeedlemanWunschAlignmentWithInvalidSequence(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                InvalidSequenceType.SequenceWithSpaces,
                AlignParameters.AllParam,
                AlignmentType.SimpleAlign,
                InvalidSequenceType.SequenceWithSpaces);
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
        [Category("Priority2")]
        public void ValidateNeedlemanWunschAlignTwoDnaSequences()
        {
            this.ValidateNeedlemanWunschAlignment(
                Constants.NeedlemanWunschDnaAlignAlgorithmNodeName,
                true,
                SequenceCaseType.LowerCase,
                AlignParameters.AlignTwo,
                AlignmentType.Align);
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
        [Category("Priority2")]
        public void ValidateNeedlemanWunschAlignTwoRnaSequences()
        {
            this.ValidateNeedlemanWunschAlignment(
                Constants.NeedlemanWunschRnaAlignAlgorithmNodeName,
                true,
                SequenceCaseType.LowerCase,
                AlignParameters.AlignTwo,
                AlignmentType.Align);
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
        [Category("Priority2")]
        public void ValidateNeedlemanWunschAlignTwoProteinSequences()
        {
            this.ValidateNeedlemanWunschAlignment(
                Constants.NeedlemanWunschProAlignAlgorithmNodeName,
                true,
                SequenceCaseType.LowerCase,
                AlignParameters.AlignTwo,
                AlignmentType.Align);
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
        [Category("Priority2")]
        public void ValidateNeedlemanWunschAlignTwoSequencesGapCostMin()
        {
            this.ValidateNeedlemanWunschAlignment(
                Constants.NeedlemanWunschGapCostMinAlignAlgorithmNodeName,
                true,
                SequenceCaseType.LowerCase,
                AlignParameters.AlignTwo,
                AlignmentType.Align);
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
        [Category("Priority2")]
        public void ValidateNeedlemanWunschAlignTwoSequencesWithBlosomSimilarityMatrix()
        {
            this.ValidateNeedlemanWunschAlignment(
                Constants.NeedlemanWunschBlosumAlignAlgorithmNodeName,
                true,
                SequenceCaseType.LowerCase,
                AlignParameters.AlignTwo,
                AlignmentType.Align);
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
        [Category("Priority2")]
        public void ValidateNeedlemanWunschAlignTwoSequencesWithPamSimilarityMatrix()
        {
            this.ValidateNeedlemanWunschAlignment(
                Constants.NeedlemanWunschPamAlignAlgorithmNodeName,
                true,
                SequenceCaseType.LowerCase,
                AlignParameters.AlignTwo,
                AlignmentType.Align);
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
        [Category("Priority2")]
        public void ValidateNeedlemanWunschAlignTwoSequencesWithTextReaderSimilarityMatrix()
        {
            this.ValidateNeedlemanWunschAlignment(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                true,
                SequenceCaseType.LowerCase,
                AlignParameters.AlignTwo,
                AlignmentType.Align,
                SimilarityMatrixParameters.TextReader);
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
        [Category("Priority2")]
        public void ValidateNeedlemanWunschAlignTwoSequencesWithDiagonalSimilarityMatrix()
        {
            this.ValidateNeedlemanWunschAlignment(
                Constants.NeedlemanWunschDiagonalSimMatAlignAlgorithmNodeName,
                true, SequenceCaseType.LowerCase,
                AlignParameters.AlignTwo,
                AlignmentType.Align,
                SimilarityMatrixParameters.DiagonalMatrix);
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
        [Category("Priority2")]
        public void ValidateNeedlemanWunschAlignTwoSequencesWithEqualGapOpenAndExtensionCost()
        {
            this.ValidateNeedlemanWunschAlignment(
                Constants.NeedlemanWunschEqualAlignAlgorithmNodeName,
                true, SequenceCaseType.LowerCase,
                AlignParameters.AlignTwo,
                AlignmentType.Align);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        ///     Validates NeedlemanWunschAlignment algorithm for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Xml node name</param>
        /// <param name="isTextFile">Is text file an input.</param>
        /// <param name="caseType">Case Type</param>
        /// <param name="additionalParameter">parameter based on which certain validations are done.</param>
        private void ValidateNeedlemanWunschAlignment(string nodeName,
                                                      bool isTextFile, SequenceCaseType caseType,
                                                      AlignParameters additionalParameter)
        {
            this.ValidateNeedlemanWunschAlignment(nodeName, isTextFile,
                                             caseType, additionalParameter, AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Validates NeedlemanWunschAlignment algorithm for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Xml node name</param>
        /// <param name="isTextFile">Is text file an input.</param>
        /// <param name="caseType">Case Type</param>
        /// <param name="additionalParameter">parameter based on which certain validations are done.</param>
        /// <param name="alignType">Is the Align type Simple or Align with Gap Extension cost?</param>
        private void ValidateNeedlemanWunschAlignment(string nodeName,
                                                      bool isTextFile, SequenceCaseType caseType,
                                                      AlignParameters additionalParameter, AlignmentType alignType)
        {
            this.ValidateNeedlemanWunschAlignment(nodeName, isTextFile,
                                             caseType, additionalParameter, alignType,
                                             SimilarityMatrixParameters.Default);
        }

        /// <summary>
        ///     Validates NeedlemanWunschAlignment algorithm for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Xml node name</param>
        /// <param name="isTextFile">Is text file an input.</param>
        /// <param name="caseType">Case Type</param>
        /// <param name="additionalParameter">parameter based on which certain validations are done.</param>
        /// <param name="alignType">Is the Align type Simple or Align with Gap Extension cost?</param>
        /// <param name="similarityMatrixParam">Similarity Matrix</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling"),
         SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void ValidateNeedlemanWunschAlignment(string nodeName, bool isTextFile, SequenceCaseType caseType,
                                                      AlignParameters additionalParameter, AlignmentType alignType,
                                                      SimilarityMatrixParameters similarityMatrixParam)
        {
            Sequence aInput, bInput;
            IAlphabet alphabet =
                Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.AlphabetNameNode));

            if (isTextFile)
            {
                // Read the xml file for getting both the files for aligning.
                string filePath1 = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode1);
                string filePath2 = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode2);

                // Parse the files and get the sequence.

                var parseObjectForFile1 = new FastAParser { Alphabet = alphabet };
                var parseObjectForFile2 = new FastAParser { Alphabet = alphabet };
                ISequence originalSequence1 = parseObjectForFile1.Parse(filePath1).First();
                ISequence originalSequence2 = parseObjectForFile2.Parse(filePath2).First();

                // Create input sequence for sequence string in different cases.
                GetSequenceWithCaseType(originalSequence1.ConvertToString(),
                                        originalSequence2.ConvertToString(), alphabet, caseType, out aInput, out bInput);
            }
            else
            {
                string originalSequence1 = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode1);
                string originalSequence2 = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode2);

                // Create input sequence for sequence string in different cases.
                GetSequenceWithCaseType(
                    originalSequence1,
                    originalSequence2,
                    alphabet,
                    caseType,
                    out aInput,
                    out bInput);
            }

            // Create similarity matrix object for a given file.
            string blosumFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.BlosumFilePathNode);

            SimilarityMatrix sm;
            switch (similarityMatrixParam)
            {
                case SimilarityMatrixParameters.TextReader:
                    using (TextReader reader = new StreamReader(blosumFilePath))
                        sm = new SimilarityMatrix(reader);
                    break;
                case SimilarityMatrixParameters.DiagonalMatrix:
                    string matchValue = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                        Constants.MatchScoreNode);
                    string misMatchValue = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                           Constants.MisMatchScoreNode);
                    sm = new DiagonalSimilarityMatrix(int.Parse(matchValue, null),
                                                      int.Parse(misMatchValue, null));
                    break;
                default:
                    sm = new SimilarityMatrix(new StreamReader(blosumFilePath));
                    break;
            }

            int gapOpenCost = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.GapOpenCostNode), null);
            int gapExtensionCost = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.GapExtensionCostNode),
                                             null);

            // Create NeedlemanWunschAligner instance and set its values.
            var needlemanWunschObj = new NeedlemanWunschAligner();
            if (additionalParameter != AlignParameters.AllParam)
            {
                needlemanWunschObj.SimilarityMatrix = sm;
                needlemanWunschObj.GapOpenCost = gapOpenCost;
                needlemanWunschObj.GapExtensionCost = gapExtensionCost;
            }
            IList<IPairwiseSequenceAlignment> result = null;

            // Align the input sequences.
            switch (additionalParameter)
            {
                case AlignParameters.AlignList:
                    switch (alignType)
                    {
                        case AlignmentType.Align:
                            result = needlemanWunschObj.Align(new List<ISequence> {aInput, bInput});
                            break;
                        default:
                            result = needlemanWunschObj.AlignSimple(new List<ISequence> {aInput, bInput});
                            break;
                    }
                    break;
                case AlignParameters.AlignTwo:
                    switch (alignType)
                    {
                        case AlignmentType.Align:
                            result = needlemanWunschObj.Align(aInput, bInput);
                            break;
                        default:
                            result = needlemanWunschObj.AlignSimple(aInput, bInput);
                            break;
                    }
                    break;
                case AlignParameters.AllParam:
                    switch (alignType)
                    {
                        case AlignmentType.Align:
                            result = needlemanWunschObj.Align(sm, gapOpenCost,
                                                              gapExtensionCost, aInput, bInput);
                            break;
                        default:
                            result = needlemanWunschObj.AlignSimple(sm, gapOpenCost, aInput, bInput);
                            break;
                    }
                    break;
                default:
                    break;
            }

            // Get the expected sequence and scorde from xml config.
            string expectedSequence1, expectedSequence2, expectedScore;

            switch (alignType)
            {
                case AlignmentType.Align:
                    expectedScore = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedGapExtensionScoreNode);
                    switch (caseType)
                    {
                        case SequenceCaseType.LowerCase:
                            expectedSequence1 = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                Constants
                                                                                    .ExpectedGapExtensionSequence1InLower);
                            expectedSequence2 = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                Constants
                                                                                    .ExpectedGapExtensionSequence2InLower);
                            break;
                        default:
                            expectedSequence1 = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                Constants
                                                                                    .ExpectedGapExtensionSequence1Node);
                            expectedSequence2 = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                Constants
                                                                                    .ExpectedGapExtensionSequence2Node);
                            break;
                    }
                    break;
                default:
                    expectedScore = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                    Constants.ExpectedScoreNode);
                    switch (caseType)
                    {
                        case SequenceCaseType.LowerCase:
                            expectedSequence1 = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                Constants.ExpectedSequence1inLowerNode);
                            expectedSequence2 = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                Constants.ExpectedSequence2inLowerNode);
                            break;
                        case SequenceCaseType.LowerUpperCase:
                            expectedSequence1 = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                Constants.ExpectedSequence1inLowerNode);
                            expectedSequence2 = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                Constants.ExpectedSequenceNode2);
                            break;
                        default:
                            expectedSequence1 = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                Constants.ExpectedSequenceNode1);
                            expectedSequence2 = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                Constants.ExpectedSequenceNode2);
                            break;
                    }
                    break;
            }

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();

            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            var alignedSeq = new PairwiseAlignedSequence
                                 {
                                     FirstSequence = new Sequence(alphabet, expectedSequence1),
                                     SecondSequence = new Sequence(alphabet, expectedSequence2),
                                     Score = Convert.ToInt32(expectedScore, null),
                                     FirstOffset = Int32.MinValue,
                                     SecondOffset = Int32.MinValue
                                 };
            align.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(align);

            ApplicationLog.WriteLine(string.Format(null, "NeedlemanWunschAligner P2 : Final Score '{0}'.", expectedScore));
            ApplicationLog.WriteLine(string.Format(null, "NeedlemanWunschAligner P2 : Aligned First Sequence is '{0}'.",
                                                   expectedSequence1));
            ApplicationLog.WriteLine(string.Format(null, "NeedlemanWunschAligner P2 : Aligned Second Sequence is '{0}'.",
                                                   expectedSequence2));

            Assert.IsTrue(CompareAlignment(result, expectedOutput));
        }

        /// <summary>
        ///     InValidates NeedlemanWunschAlignment with invalid sequence.
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="isTextFile">Is text file an input.</param>
        /// <param name="invalidSequenceType"></param>
        /// <param name="additionalParameter">parameter based on which certain validations are done.</param>
        /// <param name="alignType">Is the Align type Simple or Align with Gap Extension cost?</param>
        /// <param name="sequenceType"></param>
       [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void InValidateNeedlemanWunschAlignmentWithInvalidSequence(
            string nodeName, bool isTextFile, InvalidSequenceType invalidSequenceType,
            AlignParameters additionalParameter, AlignmentType alignType,
            InvalidSequenceType sequenceType)
        {
            IAlphabet alphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(nodeName,Constants.AlphabetNameNode));
            Exception actualException = null;
            Sequence aInput = null;

            if (isTextFile)
            {
                // Read the xml file for getting both the files for aligning.
                string filepath = this.GetInputFileNameWithInvalidType(nodeName, invalidSequenceType);

                // Create input sequence for sequence string in different cases.
                try
                {
                    // Parse the files and get the sequence.
                    var parser = new FastAParser { Alphabet = alphabet };
                    var sequence = parser.Parse(filepath).First();
                    aInput = new Sequence(alphabet, sequence.ConvertToString());
                }
                catch (Exception ex)
                {
                    actualException = ex;
                }
            }
            else
            {
                string originalSequence = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.InvalidSequence1);

                // Create input sequence for sequence string in different cases.
                try
                {
                    aInput = new Sequence(alphabet, originalSequence);
                }
                catch (ArgumentException ex)
                {
                    actualException = ex;
                }
            }

            if (null == actualException)
            {
                Sequence bInput = aInput;

                // Create similarity matrix object for a given file.
                string blosumFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.BlosumFilePathNode);
                var sm = new SimilarityMatrix(new StreamReader(blosumFilePath));

                int gapOpenCost = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.GapOpenCostNode), null);
                int gapExtensionCost = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.GapExtensionCostNode), null);

                // Create NeedlemanWunschAligner instance and set its values.
                var needlemanWunschObj = new NeedlemanWunschAligner();
                if (additionalParameter != AlignParameters.AllParam)
                {
                    needlemanWunschObj.SimilarityMatrix = sm;
                    needlemanWunschObj.GapOpenCost = gapOpenCost;
                    needlemanWunschObj.GapExtensionCost = gapExtensionCost;
                }

                // Align the input sequences and catch the exception.
                switch (additionalParameter)
                {
                    case AlignParameters.AlignList:
                        switch (alignType)
                        {
                            case AlignmentType.Align:
                                try
                                {
                                    needlemanWunschObj.Align(new List<ISequence> {aInput, bInput});
                                }
                                catch (ArgumentException ex)
                                {
                                    actualException = ex;
                                }
                                break;
                            default:
                                try
                                {
                                    needlemanWunschObj.AlignSimple(new List<ISequence> {aInput, bInput});
                                }
                                catch (ArgumentException ex)
                                {
                                    actualException = ex;
                                }
                                break;
                        }
                        break;
                    case AlignParameters.AlignTwo:
                        switch (alignType)
                        {
                            case AlignmentType.Align:
                                try
                                {
                                    needlemanWunschObj.Align(aInput, bInput);
                                }
                                catch (ArgumentException ex)
                                {
                                    actualException = ex;
                                }
                                break;
                            default:
                                try
                                {
                                    needlemanWunschObj.AlignSimple(aInput, bInput);
                                }
                                catch (ArgumentException ex)
                                {
                                    actualException = ex;
                                }
                                break;
                        }
                        break;
                    case AlignParameters.AllParam:
                        switch (alignType)
                        {
                            case AlignmentType.Align:
                                try
                                {
                                    needlemanWunschObj.Align(sm, gapOpenCost,
                                                             gapExtensionCost, aInput, bInput);
                                }
                                catch (ArgumentException ex)
                                {
                                    actualException = ex;
                                }
                                break;
                            default:
                                try
                                {
                                    needlemanWunschObj.AlignSimple(sm, gapOpenCost, aInput, bInput);
                                }
                                catch (ArgumentException ex)
                                {
                                    actualException = ex;
                                }
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }

            // Validate Error messages for Invalid Sequence types.
            string expectedErrorMessage = this.GetExpectedErrorMeesageWithInvalidSequenceType(
                nodeName, sequenceType);

            Assert.AreEqual(expectedErrorMessage, actualException.Message);

            ApplicationLog.WriteLine(string.Concat(
                "NeedlemanWunschAligner P2 : Expected Error message is thrown ",
                expectedErrorMessage));
        }

        /// <summary>
        ///     Validates NeedlemanWunschAlignment algorithm for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Xml node name</param>
        /// <param name="isTextFile">Is text file an input.</param>
        /// <param name="invalidType">Invalid type</param>
        /// <param name="additionalParameter">parameter based on which certain validations are done.</param>
        /// <param name="alignType">Is the Align type Simple or Align with Gap Extension cost?</param>
       [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void InValidateNeedlemanWunschAlignmentWithInvalidSimilarityMatrix(
            string nodeName, bool isTextFile, SimilarityMatrixInvalidTypes invalidType,
            AlignParameters additionalParameter, AlignmentType alignType)
        {
            Sequence aInput = null;
            Sequence bInput = null;

            IAlphabet alphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                     Constants.AlphabetNameNode));
            if (isTextFile)
            {
                // Read the xml file for getting both the files for aligning.
                string firstInputFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                            Constants.FilePathNode1);
                string secondInputFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                             Constants.FilePathNode2);

                // Parse the files and get the sequence.
                var parseObjectForFile1 = new FastAParser();
                var parseObjectForFile2 = new FastAParser();
                ISequence inputSequence1 = parseObjectForFile1.Parse(firstInputFilePath).ElementAt(0);
                ISequence inputSequence2 = parseObjectForFile2.Parse(secondInputFilePath).ElementAt(0);

                // Create input sequence for sequence string in different cases.
                GetSequenceWithCaseType(new string(inputSequence1.Select(a => (char) a).ToArray()),
                                        new string(inputSequence2.Select(a => (char) a).ToArray()), alphabet,
                                        SequenceCaseType.LowerCase, out aInput, out bInput);
            }
            else
            {
                string firstInputSequence = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode1);
                string secondInputSequence = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode2);

                // Create input sequence for sequence string in different cases.
                GetSequenceWithCaseType(firstInputSequence, secondInputSequence, alphabet,
                                        SequenceCaseType.LowerCase, out aInput, out bInput);
            }

            // Create similarity matrix object for a invalid file.
            string blosumFilePath = this.GetSimilarityMatrixFileWithInvalidType(nodeName, invalidType);
            Exception actualExpection = null;

            // For invalid similarity matrix data format; exception will be thrown while instantiating
            SimilarityMatrix sm = null;
            try
            {
                if (invalidType != SimilarityMatrixInvalidTypes.NullSimilarityMatrix)
                {
                    sm = new SimilarityMatrix(new StreamReader(blosumFilePath));
                }
            }
            catch (InvalidDataException ex)
            {
                actualExpection = ex;
            }

            // For non matching similarity matrix exception will be thrown while alignment
            if (actualExpection == null)
            {
                int gapOpenCost = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.GapOpenCostNode),
                                            null);

                int gapExtensionCost = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                 Constants.GapExtensionCostNode), null);

                // Create NeedlemanWunschAligner instance and set its values.
                var needlemanWunschObj = new NeedlemanWunschAligner();
                if (additionalParameter != AlignParameters.AllParam)
                {
                    needlemanWunschObj.SimilarityMatrix = sm;
                    needlemanWunschObj.GapOpenCost = gapOpenCost;
                    needlemanWunschObj.GapExtensionCost = gapExtensionCost;
                }

                // Align the input sequences and catch the exception.
                switch (additionalParameter)
                {
                    case AlignParameters.AlignList:
                        switch (alignType)
                        {
                            case AlignmentType.Align:
                                try
                                {
                                    needlemanWunschObj.Align(new List<ISequence> {aInput, bInput});
                                }
                                catch (ArgumentException ex)
                                {
                                    actualExpection = ex;
                                }
                                break;
                            default:
                                try
                                {
                                    needlemanWunschObj.AlignSimple(new List<ISequence> {aInput, bInput});
                                }
                                catch (ArgumentException ex)
                                {
                                    actualExpection = ex;
                                }
                                break;
                        }
                        break;
                    case AlignParameters.AlignTwo:
                        switch (alignType)
                        {
                            case AlignmentType.Align:
                                try
                                {
                                    needlemanWunschObj.Align(aInput, bInput);
                                }
                                catch (ArgumentException ex)
                                {
                                    actualExpection = ex;
                                }
                                break;
                            default:
                                try
                                {
                                    needlemanWunschObj.AlignSimple(aInput, bInput);
                                }
                                catch (ArgumentException ex)
                                {
                                    actualExpection = ex;
                                }
                                break;
                        }
                        break;
                    case AlignParameters.AllParam:
                        switch (alignType)
                        {
                            case AlignmentType.Align:
                                try
                                {
                                    needlemanWunschObj.Align(sm, gapOpenCost,
                                                             gapExtensionCost, aInput, bInput);
                                }
                                catch (ArgumentException ex)
                                {
                                    actualExpection = ex;
                                }
                                break;
                            default:
                                try
                                {
                                    needlemanWunschObj.AlignSimple(sm, gapOpenCost, aInput, bInput);
                                }
                                catch (ArgumentException ex)
                                {
                                    actualExpection = ex;
                                }
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }

            // Validate that expected exception is thrown using error message.
            string expectedErrorMessage =
                this.GetExpectedErrorMeesageWithInvalidSimilarityMatrixType(nodeName, invalidType);
            Assert.AreEqual(expectedErrorMessage, actualExpection.Message);

            ApplicationLog.WriteLine(string.Concat(
                "NeedlemanWunschAligner P2 : Expected Error message is thrown ",
                expectedErrorMessage));
        }

        /// <summary>
        ///     Gets the expected error message for invalid similarity matrix type.
        /// </summary>
        /// <param name="nodeName">xml node</param>
        /// <param name="invalidType">similarity matrix invalid type.</param>
        /// <returns>Returns expected error message</returns>
        private string GetExpectedErrorMeesageWithInvalidSimilarityMatrixType(string nodeName,
                                                                              SimilarityMatrixInvalidTypes invalidType)
        {
            string expectedErrorMessage = string.Empty;
            switch (invalidType)
            {
                case SimilarityMatrixInvalidTypes.FewAlphabetsSimilarityMatrix:
                case SimilarityMatrixInvalidTypes.NonMatchingSimilarityMatrix:
                    expectedErrorMessage = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                           Constants.ExpectedErrorMessage);
                    break;
                case SimilarityMatrixInvalidTypes.EmptySimilaityMatrix:
                    expectedErrorMessage = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                           Constants.EmptyMatrixErrorMessage);
                    break;
                case SimilarityMatrixInvalidTypes.OnlyAlphabetSimilarityMatrix:
                    expectedErrorMessage = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                           Constants.SimilarityMatrixFewerLinesException);
                    break;
                case SimilarityMatrixInvalidTypes.ModifiedSimilarityMatrix:
                    expectedErrorMessage = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                           Constants.ModifiedMatrixErrorMessage);
                    break;
                case SimilarityMatrixInvalidTypes.NullSimilarityMatrix:
                    expectedErrorMessage = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                           Constants.NullErrorMessage);
                    break;
                case SimilarityMatrixInvalidTypes.EmptySequence:
                    expectedErrorMessage = this.utilityObj.xmlUtil.GetFileTextValue(nodeName,
                                                                               Constants.EmptySequenceErrorMessage);
                    break;
                case SimilarityMatrixInvalidTypes.ExpectedErrorMessage:
                    expectedErrorMessage = this.utilityObj.xmlUtil.GetFileTextValue(nodeName,
                                                                               Constants.ExpectedErrorMessage);
                    break;
                default:
                    break;
            }

            return expectedErrorMessage;
        }

        /// <summary>
        ///     Gets the expected error message for invalid sequence type.
        /// </summary>
        /// <param name="nodeName">xml node</param>
        /// <param name="sequenceType"></param>
        /// <returns>Returns expected error message</returns>
        private string GetExpectedErrorMeesageWithInvalidSequenceType(string nodeName,
                                                                      InvalidSequenceType sequenceType)
        {
            string expectedErrorMessage = string.Empty;
            switch (sequenceType)
            {
                case InvalidSequenceType.SequenceWithInvalidChars:
                    expectedErrorMessage = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                           Constants.EmptySequenceErrorMessage);
                    break;
                case InvalidSequenceType.InvalidSequence:
                    expectedErrorMessage = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                           Constants.InvalidSequenceErrorMessage);
                    break;
                case InvalidSequenceType.SequenceWithUnicodeChars:
                    expectedErrorMessage = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                           Constants.UnicodeSequenceErrorMessage);
                    break;
                case InvalidSequenceType.SequenceWithSpaces:
                    expectedErrorMessage = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                           Constants.SequenceWithSpaceErrorMessage);
                    break;
                case InvalidSequenceType.AlphabetMap:
                    expectedErrorMessage = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                           Constants.InvalidAlphabetErrorMessage);
                    break;
                default:
                    expectedErrorMessage = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                           Constants.ExpectedErrorMessage);
                    break;
            }

            return expectedErrorMessage;
        }

        /// <summary>
        ///     Gets the similarity matrix file name for a given invalid similarity matrix type.
        /// </summary>
        /// <param name="nodeName">xml node.</param>
        /// <param name="invalidType">similarity matrix invalid type.</param>
        /// <returns>Returns similarity matrix file name.</returns>
        private string GetSimilarityMatrixFileWithInvalidType(string nodeName,
                                                              SimilarityMatrixInvalidTypes invalidType)
        {
            string invalidFileNode = string.Empty;
            string invalidFilePath = string.Empty;
            switch (invalidType)
            {
                case SimilarityMatrixInvalidTypes.NonMatchingSimilarityMatrix:
                    invalidFileNode = Constants.BlosumInvalidFilePathNode;
                    break;
                case SimilarityMatrixInvalidTypes.EmptySimilaityMatrix:
                    invalidFileNode = Constants.BlosumEmptyFilePathNode;
                    break;
                case SimilarityMatrixInvalidTypes.OnlyAlphabetSimilarityMatrix:
                    invalidFileNode = Constants.BlosumOnlyAlphabetFilePathNode;
                    break;
                case SimilarityMatrixInvalidTypes.FewAlphabetsSimilarityMatrix:
                    invalidFileNode = Constants.BlosumFewAlphabetsFilePathNode;
                    break;
                case SimilarityMatrixInvalidTypes.ModifiedSimilarityMatrix:
                    invalidFileNode = Constants.BlosumModifiedFilePathNode;
                    break;
                default:
                    break;
            }
            if (1 == string.Compare(invalidFileNode, string.Empty, StringComparison.CurrentCulture))
            {
                invalidFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName, invalidFileNode);
            }
            return invalidFilePath;
        }

        /// <summary>
        ///     Gets the input file name for a given invalid sequence type.
        /// </summary>
        /// <param name="nodeName">xml node.</param>
        /// <param name="invalidType">sequence invalid type.</param>
        /// <returns>Returns input file name.</returns>
        private string GetInputFileNameWithInvalidType(string nodeName,
                                                       InvalidSequenceType invalidSequenceType)
        {
            string invalidFilePath = string.Empty;
            switch (invalidSequenceType)
            {
                case InvalidSequenceType.SequenceWithSpecialChars:
                    invalidFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.InvalidFilePathNode1);
                    break;
                case InvalidSequenceType.EmptySequence:
                    invalidFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.EmptyFilePath1);
                    break;
                case InvalidSequenceType.SequenceWithSpaces:
                    invalidFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SpacesFilePath1);
                    break;
                case InvalidSequenceType.SequenceWithGap:
                    invalidFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.GapFilePath1);
                    break;
                case InvalidSequenceType.SequenceWithUnicodeChars:
                    invalidFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.UnicodeFilePath1);
                    break;
                case InvalidSequenceType.SequenceWithInvalidChars:
                    invalidFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                      Constants.EmptySequenceErrorMessage);
                    break;
                default:
                    break;
            }

            return invalidFilePath;
        }

        /// <summary>
        ///     Creates the sequence object with sequences in different cases
        /// </summary>
        /// <param name="firstSequenceString">First sequence string.</param>
        /// <param name="secondSequenceString">Second sequence string.</param>
        /// <param name="alphabet">alphabet type.</param>
        /// <param name="caseType">Sequence case type</param>
        /// <param name="firstInputSequence">First input sequence object.</param>
        /// <param name="secondInputSequence">Second input sequence object.</param>
        private static void GetSequenceWithCaseType(string firstSequenceString,
                                                    string secondSequenceString, IAlphabet alphabet,
                                                    SequenceCaseType caseType,
                                                    out Sequence firstInputSequence, out Sequence secondInputSequence)
        {
            switch (caseType)
            {
                case SequenceCaseType.LowerCase:
                    firstInputSequence = new Sequence(alphabet,
                                                      firstSequenceString.ToString(null)
                                                                         .ToLower(CultureInfo.CurrentCulture));
                    secondInputSequence = new Sequence(alphabet,
                                                       secondSequenceString.ToString(null)
                                                                           .ToLower(CultureInfo.CurrentCulture));
                    break;
                case SequenceCaseType.UpperCase:
                    firstInputSequence = new Sequence(alphabet,
                                                      firstSequenceString.ToString(null)
                                                                         .ToUpper(CultureInfo.CurrentCulture));
                    secondInputSequence = new Sequence(alphabet,
                                                       secondSequenceString.ToString(null)
                                                                           .ToUpper(CultureInfo.CurrentCulture));
                    break;
                case SequenceCaseType.LowerUpperCase:
                    firstInputSequence = new Sequence(alphabet,
                                                      firstSequenceString.ToString(null)
                                                                         .ToLower(CultureInfo.CurrentCulture));
                    secondInputSequence = new Sequence(alphabet,
                                                       secondSequenceString.ToString(null)
                                                                           .ToUpper(CultureInfo.CurrentCulture));
                    break;
                case SequenceCaseType.Default:
                default:
                    firstInputSequence = new Sequence(alphabet, firstSequenceString.ToString(null));
                    secondInputSequence = new Sequence(alphabet, secondSequenceString.ToString(null));
                    break;
            }
        }

        /// <summary>
        ///     Compare the alignment of mummer and defined alignment
        /// </summary>
        /// <param name="actualAlignment"></param>
        /// <param name="expectedAlignment">expected output</param>
        /// <returns>Compare result of alignments</returns>
        private static bool CompareAlignment(IList<IPairwiseSequenceAlignment> actualAlignment,
                                             IList<IPairwiseSequenceAlignment> expectedAlignment)
        {
            return AlignmentHelpers.CompareAlignment(actualAlignment, expectedAlignment);
        }

        #endregion
    }
}