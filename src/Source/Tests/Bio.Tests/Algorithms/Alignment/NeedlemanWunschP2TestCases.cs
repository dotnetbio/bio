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

        private readonly Utility utilityObj = new Utility(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestUtils", "TestsConfig.xml"));

        #region Test Cases


        /// <summary>
        /// Validate that if we try to do an alignment with sequences that are too large
        /// for a global M x N matrix allocation, we throw an exception. 
        /// </summary>
        [Test]
        [Category("NeedlemanWunschAligner")]
        public void NeedlemanWunschThrowsExceptionWhenTooLarge()
        {
            // What size squared is too large?
            int seq_size = (int)Math.Sqrt ((double)Int32.MaxValue) + 5;
            byte[] seq = new byte[seq_size];
            // Now let's generate sequences of those size
            for (int i = 0; i < seq.Length; i++) {
                seq [i] = (byte)'A';
            }
            var seq1 = new Sequence (DnaAlphabet.Instance, seq, false);
            var na = new NeedlemanWunschAligner ();
            Assert.Throws(typeof(ArgumentOutOfRangeException),
                () => na.Align(seq1, seq1));
        }

        /// <summary>
        ///     Pass a Valid Sequence(Lower case) with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(sequence1, sequence2)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
        public void ValidateNeedlemanWunschAlignTwoSequencesWithPamSimilarityMatrix()
        {
            this.ValidateNeedlemanWunschAlignment(
                Constants.NeedlemanWunschPamAlignAlgorithmNodeName,
                true,
                SequenceCaseType.LowerCase,
                AlignParameters.AlignTwo,
                AlignmentType.Align);
        }


        [Test]
        [Category("NeedlemanWunschAligner")]
        public void ValidateNeedlemanWunschAlignTwoSequencesWithAffineGap()
        {
            /* These tests are designed to verify the NeedlemanWunsh aligner behaves as expected, in particular
             * that it matches the EMBOSS Needle program.  
             * 
             * http://www.ebi.ac.uk/Tools/psa/emboss_needle/nucleotide.html
             * 
             * To run these tests, we can insert the sequences there and select options to set the "more options" to match
             * the options shown below.  Note that you need to set teh "EndGapPenalty" box to TRUE or the gaps at the 
             * end will not be scored appropriately
             */
            var seq1 = new Sequence (DnaAlphabet.Instance, "CAAAAGGGATTGCAAATGTTGGAGTGAATGCCATTACCTACCGGCTAGGAGGAGTAGTACAAAGGAGCTATTATCATATATTT");
            var seq2 = new Sequence (DnaAlphabet.Instance, "CATTATGTATAGGTTATCATGCGAACAATTCAACAGACACTGTAGACACAGTACTAGAAAAGAATGTAAC");
            var na = new NeedlemanWunschAligner ();
            na.GapOpenCost = -10;
            na.GapExtensionCost = -1;
            na.SimilarityMatrix = new SimilarityMatrix (SimilarityMatrix.StandardSimilarityMatrix.EDnaFull);

            var aln = na.Align (seq1, seq2).First ().PairwiseAlignedSequences.First ();
            Assert.AreEqual (32, aln.Score);
            Assert.AreEqual ("CAAAAGGGATTGCAAATGTTGGAGTGAATGCCATTACCTACCGGC----TAGGAGGAGTAGTACAAAGGAGCTAT-TATCATATATTT", aln.FirstSequence.ConvertToString ());
            // Note that EMBOSS puts the "GC" neighboring the first gap after but not before, but these are equivalent
            // from a scoring perspective.
            Assert.AreEqual ("CATTATGTATAGGTTATCATGC---GAA--CAATT--CAACAGACACTGTAGACACAGTACTAGAAAAGA---ATGTAAC--------", aln.SecondSequence.ConvertToString ());
            Assert.AreEqual (42, aln.Metadata ["SimilarityCount"]);


            // Now let's verify the simple alignment is different from the affine alignment and is optima.
            na.GapOpenCost = -1;
            aln = na.AlignSimple (seq1, seq2).First().PairwiseAlignedSequences.First();
            Assert.AreEqual (183, aln.Score); // Needle reports a score of 181

            /* Again, we don't have an exact match here.  Part of this is due to
            the placement of equivalent scores .NET Bio tends to not left align,
            but part is also due to the end state.  NEEDLE appears to prevent a
            insertion followed by a deletion at the very end but this is actually
            favorable.  It allows insertions to follow deletions earlier in the
            alignment, so I am not sure what the problem here is, likely a bug in
            NEEDLE.  As a result .NET Bio has a score of 183 to Needles score of
            181 due to the end change.

            This bug in the Needle program was reported to
            emboss-bug@emboss.open-bio.org on 12/6/2015 with the email below.

                    Hi,

                    There appears to be a bug in the needle program where it does not
                    always return the globally best alignment as it should.

                    This can be easily replicated using the web portal:

                    http://www.ebi.ac.uk/Tools/psa/emboss_needle/nucleotide.html

                    And entering the following options

                    Seq1 = CAAAAGGGATTGCAAATGTTGGAGTGAATGCCATTACCTACCGGCTAGGAGGAGTAGTACAA
                    AGGAGCTATTATCATATATTT
                    Seq2 = CATTATGTATAGGTTATCATGCGAACAATTCAACAGACACTGTAGACACAGTACTAGAAAAG
                    AATGTAAC
                    Matrix: DNAfull
                    GapOpen: 1
                    GapExtend: 1
                    EndGapPenalty: True
                    EndGapOpen: 1
                    EndGapExtend: 1

                    Which produces the following alignment with a score of 181

                    CAAAAGGGATTGCAAATGT-T GGAGTG--A--ATGC---C-ATT--ACCT--AC-C-GGCTAGGAGG-
                    |       |||    |||| |   || |  |  ||||   | |||  ||    || | |  || ||
                    C-------ATT----ATGTAT --AG-GTTATCATGCGAACAATTCAAC--AGACACTG--TA-GA--C

                    AGT-AGTAC-A-AAGG-AGCTATTATCA-TATATTT
                    |   ||||| | ||   ||  |  ||   || |  .
                    A--CAGTACTAGAA--AAG--A--AT--GTA-A——C

                    However, the optimal alignment has a score of 183.  The problem here
                    being that with this particular scoring, a mismatch should never
                    occur as the mismatch penalty (-4 in the matrix), is less than 2
                    times the gap open/extend penalty (-2).  This means that you should
                    always have a deletion followed by an insertion rather than a
                    mismatch.  Needle follows this rule for every position except the
                    last one, where for some reason it tacks on a mismatch at the end.
                    The alignment should have been:

                    CAAAAGGGATTGCAAATGT-T GGAGTG--A--ATGC---C-ATT--ACCT--AC-C-GGCTAGGAGG-
                    |       |||    |||| |   || |  |  ||||   | |||  ||    || | |  || ||
                    C-------ATT----ATGTAT --AG-GTTATCATGCGAACAATTCAAC--AGACACTG--TA-GA--C

                    AGT-AGTAC-A-AAGG-AGCTATTATCA-TATATTT-
                    |   ||||| | ||   ||  |  ||   || |
                    A--CAGTACTAGAA--AAG--A--AT--GTA-A——-C

                    Which gives the correct score of 183.  Note that by default needle
                    does not score end gaps at all, but with the —endweight option
                    enabled, it should have returned the alignment with a score of 183.

                    I don’t suppose anyone is able to fix this?

                    Warm wishes, Nigel


            I placed the sequences returned by either program here to show the
            difference explicitly. */
            //var needleExpect1 = "CAAAAGGGATTGCAAATGT-TGGAGTG--A--ATGC---C-ATT--ACCT--AC-C-GGCTAGGAGG-AGT-AGTAC-A-AAGG-AGCTATTATCA-TATATTT";
            //var needleExpect2 = "C-------ATT----ATGTAT--AG-GTTATCATGCGAACAATTCAAC--AGACACTG--TA-GA--CA--CAGTACTAGAA--AAG--A--AT--GTA-A--C";
            var netBioExpect1 = "CAAAAGGGATTGCAAATGT-T-GG--AGTG-AATGC---CA-TT-A-C---CTACC-GGCTAGGAGG-AGT-AGTAC-A-AAGGA-GCTATTATCA-TATATTT-";
            var netBioExpect2 = "CA-------TT--A--TGTATAGGTTA-T-CA-TGCGAACAATTCAACAGAC-AC-TG--TAG-A--CA--CAGTACTAGAA--AAG--A--AT--GTA-A---C";
                                     
            Assert.AreEqual (netBioExpect1, aln.FirstSequence.ConvertToString ());
            Assert.AreEqual (netBioExpect2, aln.SecondSequence.ConvertToString());

            /* Now verify that an affine gap with the penalties set to the same as the simple alignment
             * produces the same result */
            aln = na.Align (seq1, seq2).First().PairwiseAlignedSequences.First();
            Assert.AreEqual (183, aln.Score); // 
            Assert.AreEqual (netBioExpect1, aln.FirstSequence.ConvertToString ());
            Assert.AreEqual (netBioExpect2, aln.SecondSequence.ConvertToString());
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
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
        [Category("NeedlemanWunschAligner")]
        public void ValidateNeedlemanWunschAlignTwoSequencesWithEqualGapOpenAndExtensionCost()
        {
            this.ValidateNeedlemanWunschAlignment(
                Constants.NeedlemanWunschEqualAlignAlgorithmNodeName,
                true, SequenceCaseType.LowerCase,
                AlignParameters.AlignTwo,
                AlignmentType.Align);
        }


        /// <summary>
        /// Validate that sequences with gaps on the ends are handled appropriately 
        /// by both the simple and affine aligners.
        /// 
        /// This test exists because the aligner did not tack on the ends of query sequences before.
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("NeedlemanWunschAligner")]
        public void ValidateNeedlemanWunschAlignTwoSequencesWithEndGaps()
        {
            var exp_ref = "-ATTGTATGGCCAACAA-";
            var refseq= new Sequence (DnaAlphabet.Instance, exp_ref.Replace("-", ""));
            var query = new Sequence (DnaAlphabet.Instance, "CATTGTATGGCCAACAAG");
            NeedlemanWunschAligner alner = new NeedlemanWunschAligner();

            var res_affine = alner.Align (refseq, query).First().PairwiseAlignedSequences.First();
            Assert.AreEqual (query.Count, res_affine.FirstSequence.Count);
            Assert.AreEqual (query.Count, res_affine.SecondSequence.Count);
            Assert.AreEqual (exp_ref, res_affine.FirstSequence.ConvertToString ());

            var res_simple = alner.AlignSimple (refseq, query).First ().PairwiseAlignedSequences.First ();
            Assert.AreEqual (query.Count, res_simple.FirstSequence.Count);
            Assert.AreEqual (query.Count, res_simple.SecondSequence.Count);
            Assert.AreEqual (exp_ref, res_simple.FirstSequence.ConvertToString ());

            // now to flip sequence 1 and 2, try it again.
            res_affine = alner.Align (query, refseq).First().PairwiseAlignedSequences.First();
            Assert.AreEqual (query.Count, res_affine.FirstSequence.Count);
            Assert.AreEqual (query.Count, res_affine.SecondSequence.Count);
            Assert.AreEqual (exp_ref, res_affine.SecondSequence.ConvertToString ());

            res_simple = alner.AlignSimple (query, refseq).First ().PairwiseAlignedSequences.First ();
            Assert.AreEqual (query.Count, res_simple.FirstSequence.Count);
            Assert.AreEqual (query.Count, res_simple.SecondSequence.Count);
            Assert.AreEqual (exp_ref, res_simple.SecondSequence.ConvertToString ());

                
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
                string filePath1 = Path.Combine(TestContext.CurrentContext.TestDirectory, this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode1));
                string filePath2 = Path.Combine(TestContext.CurrentContext.TestDirectory, this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode2));

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
            string blosumFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.BlosumFilePathNode));

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
                string filepath = Path.Combine(TestContext.CurrentContext.TestDirectory, this.GetInputFileNameWithInvalidType(nodeName, invalidSequenceType));

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
                string blosumFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.BlosumFilePathNode));
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
                string firstInputFilePath = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                            Constants.FilePathNode1));
                string secondInputFilePath = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                             Constants.FilePathNode2));

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
            string blosumFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, this.GetSimilarityMatrixFileWithInvalidType(nodeName, invalidType));
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
                                                                               Constants.ExpectedErrorMessage,
                                                                               TestContext.CurrentContext.TestDirectory);
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