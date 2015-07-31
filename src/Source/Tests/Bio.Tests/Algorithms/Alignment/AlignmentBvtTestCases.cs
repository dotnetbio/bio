using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    ///     Alignment BVT test cases implementation.
    /// </summary>
    [TestFixture]
    public class AlignmentBvtTestCases
    {
        #region Enums

        /// <summary>
        ///     Alignment Parameters which are used for different test cases
        ///     based on which the test cases are executed.
        /// </summary>
        private enum AlignmentParamType
        {
            AlignTwo,
            AlignList,
            AllParam
        };

        /// <summary>
        ///     Alignment Type Parameters which are used for different test cases
        ///     based on which the test cases are executed.
        /// </summary>
        private enum AlignmentType
        {
            SimpleAlign,
            Align
        };

        #endregion Enums

        #region Global Variables

        private readonly Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

        #region NeedlemanWunschAligner BVT Test cases

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(sequence1, sequence2)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void NeedlemanWunschSimpleAlignTwoSequencesFromTextFile()
        {
            this.ValidateNeedlemanWunschAlignment(true, AlignmentParamType.AlignTwo);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is passed in code using the method Align(sequence1, sequence2)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : sequence in xml
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void NeedlemanWunschSimpleAlignTwoSequencesFromXml()
        {
            this.ValidateNeedlemanWunschAlignment(false, AlignmentParamType.AlignTwo);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(List)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void NeedlemanWunschSimpleAlignListSequencesFromTextFile()
        {
            this.ValidateNeedlemanWunschAlignment(true, AlignmentParamType.AlignList);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is passed in code using the method Align(List)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : sequence in xml
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void NeedlemanWunschSimpleAlignListSequencesFromXml()
        {
            this.ValidateNeedlemanWunschAlignment(false, AlignmentParamType.AlignList);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : Text File i.e., Fasta
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void NeedlemanWunschSimpleAlignAllParamsFromTextFile()
        {
            this.ValidateNeedlemanWunschAlignment(true, AlignmentParamType.AllParam);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is passed in code using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : sequence in xml
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void NeedlemanWunschSimpleAlignAllParamsFromXml()
        {
            this.ValidateNeedlemanWunschAlignment(false, AlignmentParamType.AllParam);
        }

        #region Gap Extension Cost inclusion Test cases

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Open Cost, Gap Extension Cost, Similarity Matrix
        ///     which is in a text file using the method Align(sequence1, sequence2)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void NeedlemanWunschAlignTwoSequencesFromTextFile()
        {
            this.ValidateNeedlemanWunschAlignment(true, AlignmentParamType.AlignTwo,
                                             AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Open Cost, Gap Extension Cost, Similarity Matrix
        ///     which is in a text file using the method Align(List)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void NeedlemanWunschAlignListSequencesFromTextFile()
        {
            this.ValidateNeedlemanWunschAlignment(true, AlignmentParamType.AlignList, AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Open Cost, Gap Extension Cost, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : Text File i.e., Fasta
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void NeedlemanWunschAlignAllParamsFromTextFile()
        {
            this.ValidateNeedlemanWunschAlignment(true, AlignmentParamType.AllParam, AlignmentType.Align);
        }

        #endregion Gap Extension Cost inclusion Test cases

        #endregion NeedlemanWunschAligner BVT Test cases

        #region SmithWatermanAligner BVT Test cases

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(sequence1, sequence2)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SmithWatermanSimpleAlignTwoSequencesFromTextFile()
        {
            this.ValidateSmithWatermanAlignment(true, AlignmentParamType.AlignTwo);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is passed in code using the method Align(sequence1, sequence2)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : sequence in xml
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SmithWatermanSimpleAlignTwoSequencesFromXml()
        {
            this.ValidateSmithWatermanAlignment(false, AlignmentParamType.AlignTwo);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(List)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SmithWatermanSimpleAlignListSequencesFromTextFile()
        {
            this.ValidateSmithWatermanAlignment(true, AlignmentParamType.AlignList);
        }


        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is passed in code using the method Align(List)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : sequence in xml
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SmithWatermanSimpleAlignListSequencesFromXml()
        {
            this.ValidateSmithWatermanAlignment(false, AlignmentParamType.AlignList);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : Text File i.e., Fasta
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SmithWatermanSimpleAlignAllParamsFromTextFile()
        {
            this.ValidateSmithWatermanAlignment(true, AlignmentParamType.AllParam);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is passed in code using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : sequence in xml
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SmithWatermanSimpleAlignAllParamsFromXml()
        {
            this.ValidateSmithWatermanAlignment(false, AlignmentParamType.AllParam);
        }

        #region Gap Extension Cost inclusion Test cases

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Open Cost, Gap Extension Cost, Similarity Matrix
        ///     which is in a text file using the method Align(sequence1, sequence2)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SmithWatermanAlignTwoSequencesFromTextFile()
        {
            this.ValidateSmithWatermanAlignment(true, AlignmentParamType.AlignTwo, AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Open Cost, Gap Extension Cost, Similarity Matrix
        ///     which is in a text file using the method Align(List)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SmithWatermanAlignListSequencesFromTextFile()
        {
            this.ValidateSmithWatermanAlignment(true, AlignmentParamType.AlignList, AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Open Cost, Gap Extension Cost, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : Text File i.e., Fasta
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SmithWatermanAlignAllParamsFromTextFile()
        {
            this.ValidateSmithWatermanAlignment(true, AlignmentParamType.AllParam, AlignmentType.Align);
        }

        #endregion Gap Extension Cost inclusion Test cases

        #endregion SmithWatermanAligner BVT Test cases

        #region Sequence Alignment BVT Test cases

        /// <summary>
        ///     Pass a valid sequence to AddSequence() method and validate the same.
        ///     Input : Sequence read from xml file.
        ///     Validation : Added sequences are got back and validated.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")] 
        [Test]
        [Category("Priority0")]
        public void SequenceAlignmentAddSequence()
        {
            // Read the xml file for getting both the files for aligning.
            string origSequence1 = this.utilityObj.xmlUtil.GetTextValue(Constants.AlignAlgorithmNodeName, Constants.SequenceNode1);
            string origSequence2 = this.utilityObj.xmlUtil.GetTextValue(Constants.AlignAlgorithmNodeName, Constants.SequenceNode2);

            IAlphabet alphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(Constants.AlignAlgorithmNodeName, Constants.AlphabetNameNode));

            ApplicationLog.WriteLine(string.Format(null, "SequenceAlignment BVT : First sequence used is '{0}'.", origSequence1));
            ApplicationLog.WriteLine(string.Format(null,"SequenceAlignment BVT : Second sequence used is '{0}'.", origSequence2));

            // Create two sequences
            ISequence aInput = new Sequence(alphabet, origSequence1);
            ISequence bInput = new Sequence(alphabet, origSequence2);

            // Add the sequences to the Sequence alignment object using AddSequence() method.
            IList<IPairwiseSequenceAlignment> sequenceAlignmentObj =
                new List<IPairwiseSequenceAlignment>();

            var alignSeq = new PairwiseAlignedSequence {FirstSequence = aInput, SecondSequence = bInput};
            IPairwiseSequenceAlignment seqAlignObj = new PairwiseSequenceAlignment();
            seqAlignObj.Add(alignSeq);
            sequenceAlignmentObj.Add(seqAlignObj);

            // Read the output back and validate the same.
            IList<PairwiseAlignedSequence> newAlignedSequences = sequenceAlignmentObj[0].PairwiseAlignedSequences;

            ApplicationLog.WriteLine(string.Format(null, "SequenceAlignment BVT : First sequence read is '{0}'.", origSequence1));
            ApplicationLog.WriteLine(string.Format(null, "SequenceAlignment BVT : Second sequence read is '{0}'.", origSequence2));

            Assert.AreEqual(newAlignedSequences[0].FirstSequence.ConvertToString(), origSequence1);
            Assert.AreEqual(newAlignedSequences[0].SecondSequence.ConvertToString(), origSequence2);
        }

        #endregion Sequence Alignment BVT Test cases

        #region Aligned Sequence BVT Test Cases

        /// <summary>
        ///     Validate Aligned Sequence ctor by adding aligned sequnece and
        ///     metadata using smithwatermanaligner
        ///     Input : dna aligned sequence
        ///     Output : dna aligned sequence instance
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateAlignedSequenceCtor()
        {
            this.ValidateAlignedSequenceCtor(Constants.SmithWatermanAlignAlgorithmNodeName,
                                        SequenceAligners.SmithWaterman);
        }

        //private void ValidateAlignedSequence(string nodeName, ISequenceAligner aligner);

        /// <summary>
        ///     Validate Aligned Sequence by passing IAligned sequence of dna sequence
        ///     using smithwatermanaligner
        ///     Input : dna sequence
        ///     Output : dna aligned sequence instance
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateAlignedSequenceWithSmithWatermanAligner()
        {
            this.ValidateAlignedSequence(Constants.SmithWatermanAlignAlgorithmNodeName,
                                    SequenceAligners.SmithWaterman);
        }

        /// <summary>
        ///     Validate Aligned Sequence by passing IAligned sequence of dna sequence
        ///     using needlemanwunschaligner
        ///     Input : dna sequence
        ///     Output : dna aligned sequence instance
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateAlignedSequenceWithNeedlemanWunschAligner()
        {
            this.ValidateAlignedSequence(Constants.NeedlemanWunschAlignAlgorithmNodeName,
                                    SequenceAligners.NeedlemanWunsch);
        }

        #endregion

        #region Sequence Alignment BVT Test Cases

        /// <summary>
        ///     Validate Sequence Alignment ctor by passing ISequenceAlignment of dna sequence
        ///     using smithwatermanaligner
        ///     Input : dna sequence
        ///     Output : dna sequence alignment instance
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSequenceAlignmentCtorWithSmithWatermanAligner()
        {
            this.ValidateSequenceAlignmentCtor(Constants.SmithWatermanAlignAlgorithmNodeName,
                                          SequenceAligners.SmithWaterman);
        }

        /// <summary>
        ///     Validate Sequence Alignment ctor by passing ISequenceAlignment of dna sequence
        ///     using needlemanwunschaligner
        ///     Input : dna sequence
        ///     Output : dna sequence alignment instance
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSequenceAlignmentCtorWithNeedlemanWunschAligner()
        {
            this.ValidateSequenceAlignmentCtor(Constants.NeedlemanWunschAlignAlgorithmNodeName,
                                          SequenceAligners.NeedlemanWunsch);
        }


        /// <summary>
        ///     Validate Sequence Alignment by passing ISequenceAlignment of dna sequence
        ///     using smithwatermanaligner
        ///     Input : dna sequence
        ///     Output : dna sequence alignment instance
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSequenceAlignmentWithSmithWatermanAligner()
        {
            this.ValidateSequenceAlignment(Constants.SmithWatermanAlignAlgorithmNodeName,
                                      SequenceAligners.SmithWaterman);
        }

        /// <summary>
        ///     Validate Sequence Alignment by passing ISequenceAlignment of dna sequence
        ///     using needlemanwunschaligner
        ///     Input : dna sequence
        ///     Output : dna sequence alignment instance
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSequenceAlignmentWithNeedlemanWunschAligner()
        {
            this.ValidateSequenceAlignment(Constants.NeedlemanWunschAlignAlgorithmNodeName,
                                      SequenceAligners.NeedlemanWunsch);
        }

        #endregion

        #region Supporting Methods

        /// <summary>
        ///     Validates NeedlemanWunschAlignment algorithm for the parameters passed.
        /// </summary>
        /// <param name="isTextFile">Is text file an input.</param>
        /// <param name="alignParam">parameter based on which certain validations are done.</param>
        private void ValidateNeedlemanWunschAlignment(bool isTextFile, AlignmentParamType alignParam)
        {
            this.ValidateNeedlemanWunschAlignment(isTextFile, alignParam, AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Validates NeedlemanWunschAlignment algorithm for the parameters passed.
        /// </summary>
        /// <param name="isTextFile">Is text file an input.</param>
        /// <param name="alignParam">parameter based on which certain validations are done.</param>
        /// <param name="alignType">Is the Align type Simple or Align with Gap Extension cost?</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void ValidateNeedlemanWunschAlignment(bool isTextFile, AlignmentParamType alignParam,
                                                      AlignmentType alignType)
        {
            ISequence aInput, bInput;
            IAlphabet alphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(Constants.NeedlemanWunschAlignAlgorithmNodeName, Constants.AlphabetNameNode));

            if (isTextFile)
            {
                // Read the xml file for getting both the files for aligning.
                string filePath1 = this.utilityObj.xmlUtil.GetTextValue(Constants.NeedlemanWunschAlignAlgorithmNodeName, Constants.FilePathNode1);
                string filePath2 = this.utilityObj.xmlUtil.GetTextValue(Constants.NeedlemanWunschAlignAlgorithmNodeName, Constants.FilePathNode2);

                // Parse the files and get the sequence.
                var parseObjectForFile1 = new FastAParser { Alphabet = alphabet };
                aInput = parseObjectForFile1.Parse(filePath1).First();

                var parseObjectForFile2 = new FastAParser { Alphabet = alphabet };
                bInput = parseObjectForFile2.Parse(filePath2).First();
            }
            else
            {
                // Read the xml file for getting both the files for aligning.
                string origSequence1 = this.utilityObj.xmlUtil.GetTextValue(Constants.NeedlemanWunschAlignAlgorithmNodeName, Constants.SequenceNode1);
                string origSequence2 = this.utilityObj.xmlUtil.GetTextValue(Constants.NeedlemanWunschAlignAlgorithmNodeName, Constants.SequenceNode2);

                aInput = new Sequence(alphabet, origSequence1);
                bInput = new Sequence(alphabet, origSequence2);
            }

            string blosumFilePath = this.utilityObj.xmlUtil.GetTextValue(Constants.NeedlemanWunschAlignAlgorithmNodeName, Constants.BlosumFilePathNode);

            var sm = new SimilarityMatrix(new StreamReader(blosumFilePath));
            int gapOpenCost = int.Parse(this.utilityObj.xmlUtil.GetTextValue(Constants.NeedlemanWunschAlignAlgorithmNodeName, Constants.GapOpenCostNode), null);
            int gapExtensionCost = int.Parse(this.utilityObj.xmlUtil.GetTextValue(Constants.NeedlemanWunschAlignAlgorithmNodeName, Constants.GapExtensionCostNode), null);

            var needlemanWunschObj = new NeedlemanWunschAligner();
            if (AlignmentParamType.AllParam != alignParam)
            {
                needlemanWunschObj.SimilarityMatrix = sm;
                needlemanWunschObj.GapOpenCost = gapOpenCost;
                needlemanWunschObj.GapExtensionCost = gapExtensionCost;
            }

            IList<IPairwiseSequenceAlignment> result = null;

            switch (alignParam)
            {
                case AlignmentParamType.AlignList:
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
                case AlignmentParamType.AlignTwo:
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
                case AlignmentParamType.AllParam:
                    switch (alignType)
                    {
                        case AlignmentType.Align:
                            result = needlemanWunschObj.Align(
                                sm, gapOpenCost, gapExtensionCost, aInput, bInput);
                            break;
                        default:
                            result = needlemanWunschObj.AlignSimple(sm, gapOpenCost, aInput, bInput);
                            break;
                    }
                    break;
                default:
                    break;
            }

            // Read the xml file for getting both the files for aligning.
            string expectedSequence1, expectedSequence2, expectedScore;

            switch (alignType)
            {
                case AlignmentType.Align:
                    expectedScore = this.utilityObj.xmlUtil.GetTextValue(
                        Constants.NeedlemanWunschAlignAlgorithmNodeName,
                        Constants.ExpectedGapExtensionScoreNode);
                    expectedSequence1 = this.utilityObj.xmlUtil.GetTextValue(
                        Constants.NeedlemanWunschAlignAlgorithmNodeName,
                        Constants.ExpectedGapExtensionSequence1Node);
                    expectedSequence2 = this.utilityObj.xmlUtil.GetTextValue(
                        Constants.NeedlemanWunschAlignAlgorithmNodeName,
                        Constants.ExpectedGapExtensionSequence2Node);
                    break;
                default:
                    expectedScore = this.utilityObj.xmlUtil.GetTextValue(
                        Constants.NeedlemanWunschAlignAlgorithmNodeName,
                        Constants.ExpectedScoreNode);
                    expectedSequence1 = this.utilityObj.xmlUtil.GetTextValue(
                        Constants.NeedlemanWunschAlignAlgorithmNodeName,
                        Constants.ExpectedSequenceNode1);
                    expectedSequence2 = this.utilityObj.xmlUtil.GetTextValue(
                        Constants.NeedlemanWunschAlignAlgorithmNodeName,
                        Constants.ExpectedSequenceNode2);
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
                                     SecondOffset = Int32.MinValue,
                                 };
            align.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(align);

            ApplicationLog.WriteLine(string.Format(null, "NeedlemanWunschAligner BVT : Final Score '{0}'.", expectedScore));
            ApplicationLog.WriteLine(string.Format(null, "NeedlemanWunschAligner BVT : Aligned First Sequence is '{0}'.", expectedSequence1));
            ApplicationLog.WriteLine(string.Format(null, "NeedlemanWunschAligner BVT : Aligned Second Sequence is '{0}'.", expectedSequence2));

            Assert.IsTrue(CompareAlignment(result, expectedOutput));
        }

        /// <summary>
        ///     Validates SmithWatermanAlignment algorithm for the parameters passed.
        /// </summary>
        /// <param name="isTextFile">Is text file an input.</param>
        /// <param name="alignParam">parameter based on which certain validations are done.</param>
        private void ValidateSmithWatermanAlignment(bool isTextFile, AlignmentParamType alignParam)
        {
            this.ValidateSmithWatermanAlignment(isTextFile, alignParam, AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Validates SmithWatermanAlignment algorithm for the parameters passed.
        /// </summary>
        /// <param name="isTextFile">Is text file an input.</param>
        /// <param name="alignParam">parameter based on which certain validations are done.</param>
        /// <param name="alignType">Is the Align type Simple or Align with Gap Extension cost?</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void ValidateSmithWatermanAlignment(bool isTextFile, AlignmentParamType alignParam,
                                                    AlignmentType alignType)
        {
            ISequence aInput, bInput;
            IAlphabet alphabet =
                Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(Constants.SmithWatermanAlignAlgorithmNodeName,
                                                                    Constants.AlphabetNameNode));

            if (isTextFile)
            {
                // Read the xml file for getting both the files for aligning.
                string filePath1 = this.utilityObj.xmlUtil.GetTextValue(Constants.SmithWatermanAlignAlgorithmNodeName,
                                                                   Constants.FilePathNode1);
                string filePath2 = this.utilityObj.xmlUtil.GetTextValue(Constants.SmithWatermanAlignAlgorithmNodeName,
                                                                   Constants.FilePathNode2);

                // Parse the files and get the sequence.
                var parseObjectForFile1 = new FastAParser();
                {
                    parseObjectForFile1.Alphabet = alphabet;
                    aInput = parseObjectForFile1.Parse(filePath1).First();
                }

                var parseObjectForFile2 = new FastAParser();
                {
                    parseObjectForFile2.Alphabet = alphabet;
                    bInput = parseObjectForFile2.Parse(filePath2).First();
                }
            }
            else
            {
                // Read the xml file for getting both the files for aligning.
                string origSequence1 = this.utilityObj.xmlUtil.GetTextValue(Constants.SmithWatermanAlignAlgorithmNodeName,
                                                                       Constants.SequenceNode1);
                string origSequence2 = this.utilityObj.xmlUtil.GetTextValue(Constants.SmithWatermanAlignAlgorithmNodeName,
                                                                       Constants.SequenceNode2);
                aInput = new Sequence(alphabet, origSequence1);
                bInput = new Sequence(alphabet, origSequence2);
            }

            string blosumFilePath = this.utilityObj.xmlUtil.GetTextValue(Constants.SmithWatermanAlignAlgorithmNodeName,
                                                                    Constants.BlosumFilePathNode);

            var sm = new SimilarityMatrix(new StreamReader(blosumFilePath));
            int gapOpenCost =
                int.Parse(
                    this.utilityObj.xmlUtil.GetTextValue(Constants.SmithWatermanAlignAlgorithmNodeName,
                                                    Constants.GapOpenCostNode), null);
            int gapExtensionCost =
                int.Parse(
                    this.utilityObj.xmlUtil.GetTextValue(Constants.SmithWatermanAlignAlgorithmNodeName,
                                                    Constants.GapExtensionCostNode), null);

            var smithWatermanObj = new SmithWatermanAligner();
            if (AlignmentParamType.AllParam != alignParam)
            {
                smithWatermanObj.SimilarityMatrix = sm;
                smithWatermanObj.GapOpenCost = gapOpenCost;
            }

            IList<IPairwiseSequenceAlignment> result = null;

            switch (alignParam)
            {
                case AlignmentParamType.AlignList:
                    switch (alignType)
                    {
                        case AlignmentType.Align:
                            result = smithWatermanObj.Align(new List<ISequence> {aInput, bInput});
                            break;
                        default:
                            result = smithWatermanObj.AlignSimple(new List<ISequence> {aInput, bInput});
                            break;
                    }
                    break;
                case AlignmentParamType.AlignTwo:
                    switch (alignType)
                    {
                        case AlignmentType.Align:
                            result = smithWatermanObj.Align(aInput, bInput);
                            break;
                        default:
                            result = smithWatermanObj.AlignSimple(aInput, bInput);
                            break;
                    }
                    break;
                case AlignmentParamType.AllParam:
                    switch (alignType)
                    {
                        case AlignmentType.Align:
                            result = smithWatermanObj.Align(sm, gapOpenCost,
                                                            gapExtensionCost, aInput, bInput);
                            break;
                        default:
                            result = smithWatermanObj.AlignSimple(sm, gapOpenCost, aInput, bInput);
                            break;
                    }
                    break;
                default:
                    break;
            }

            // Read the xml file for getting both the files for aligning.
            string expectedSequence1, expectedSequence2, expectedScore;

            switch (alignType)
            {
                case AlignmentType.Align:
                    expectedScore = this.utilityObj.xmlUtil.GetTextValue(
                        Constants.SmithWatermanAlignAlgorithmNodeName,
                        Constants.ExpectedGapExtensionScoreNode);
                    expectedSequence1 = this.utilityObj.xmlUtil.GetTextValue(
                        Constants.SmithWatermanAlignAlgorithmNodeName,
                        Constants.ExpectedGapExtensionSequence1Node);
                    expectedSequence2 = this.utilityObj.xmlUtil.GetTextValue(
                        Constants.SmithWatermanAlignAlgorithmNodeName,
                        Constants.ExpectedGapExtensionSequence2Node);
                    break;
                default:
                    expectedScore = this.utilityObj.xmlUtil.GetTextValue(
                        Constants.SmithWatermanAlignAlgorithmNodeName,
                        Constants.ExpectedScoreNode);
                    expectedSequence1 = this.utilityObj.xmlUtil.GetTextValue(
                        Constants.SmithWatermanAlignAlgorithmNodeName,
                        Constants.ExpectedSequenceNode1);
                    expectedSequence2 = this.utilityObj.xmlUtil.GetTextValue(
                        Constants.SmithWatermanAlignAlgorithmNodeName,
                        Constants.ExpectedSequenceNode2);
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
                                     SecondOffset = Int32.MinValue,
                                 };
            align.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(align);

            ApplicationLog.WriteLine(string.Format(null, "SmithWatermanAligner BVT : Final Score '{0}'.", expectedScore));
            ApplicationLog.WriteLine(string.Format(null, "SmithWatermanAligner BVT : Aligned First Sequence is '{0}'.",
                                                   expectedSequence1));
            ApplicationLog.WriteLine(string.Format(null, "SmithWatermanAligner BVT : Aligned Second Sequence is '{0}'.",
                                                   expectedSequence2));

            Assert.IsTrue(CompareAlignment(result, expectedOutput));
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

        /// <summary>
        ///     Validate aligned sequence instance using different aligners
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="aligner">sw/nw/pw aligners</param>
        private void ValidateAlignedSequence(string nodeName, ISequenceAligner aligner)
        {
            IAlphabet alphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                     Constants.AlphabetNameNode));
            string origSequence1 = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode1);
            string origSequence2 = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode2);

            // Create input sequences
            var inputSequences = new List<ISequence>();
            inputSequences.Add(new Sequence(alphabet, origSequence1));
            inputSequences.Add(new Sequence(alphabet, origSequence2));

            // Get aligned sequences
            IList<ISequenceAlignment> alignment = aligner.Align(inputSequences);

            // Create AlignedSequence instance
            IAlignedSequence sequence = alignment[0].AlignedSequences[0];

            // Validate the alignedsequence properties
            Assert.AreEqual(alignment[0].AlignedSequences[0].Sequences, sequence.Sequences);
            Assert.AreEqual(alignment[0].AlignedSequences[0].Metadata, sequence.Metadata);

            ApplicationLog.WriteLine(@"Alignment BVT : Validation of aligned sequence completed successfully");
        }

        /// <summary>
        ///     Validate sequence alignment instance using different aligners
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="aligner">sw/nw/pw aligners</param>
        private void ValidateSequenceAlignment(string nodeName, ISequenceAligner aligner)
        {
            IAlphabet alphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                     Constants.AlphabetNameNode));
            string origSequence1 = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode1);
            string origSequence2 = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode2);

            // Create input sequences
            var inputSequences = new List<ISequence>();
            inputSequences.Add(new Sequence(alphabet, origSequence1));
            inputSequences.Add(new Sequence(alphabet, origSequence2));

            // Get aligned sequences
            IList<ISequenceAlignment> alignments = aligner.Align(inputSequences);
            ISequenceAlignment alignment = alignments[0];

            Assert.AreEqual(alignments[0].AlignedSequences.Count, alignment.AlignedSequences.Count);
            Assert.AreEqual(alignments[0].Metadata, alignment.Metadata);
            Assert.AreEqual(inputSequences[0].ToString(), alignment.Sequences[0].ToString());

            ApplicationLog.WriteLine(@"Alignment BVT : Validation of sequence alignment completed successfully");
        }

        /// <summary>
        ///     Validate aligned sequence instance using different aligners
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="aligner">sw/nw/pw aligners</param>
        private void ValidateAlignedSequenceCtor(string nodeName, ISequenceAligner aligner)
        {
            IAlphabet alphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                     Constants.AlphabetNameNode));
            string origSequence1 = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode1);
            string origSequence2 = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode2);

            // Create input sequences
            var inputSequences = new List<ISequence>();
            inputSequences.Add(new Sequence(alphabet, origSequence1));
            inputSequences.Add(new Sequence(alphabet, origSequence2));

            // Get aligned sequences
            IAlignedSequence alignedSequence = new AlignedSequence();
            IList<ISequenceAlignment> alignment = aligner.Align(inputSequences);

            // add aligned sequence and metadata information
            for (int iseq = 0; iseq < alignment[0].AlignedSequences[0].Sequences.Count; iseq++)
            {
                alignedSequence.Sequences.Add(alignment[0].AlignedSequences[0].Sequences[iseq]);
            }

            foreach (string key in alignment[0].AlignedSequences[0].Metadata.Keys)
            {
                alignedSequence.Metadata.Add(key, alignment[0].AlignedSequences[0].Metadata[key]);
            }

            // Validate the alignedsequence properties
            for (int index = 0; index < alignment[0].AlignedSequences[0].Sequences.Count; index++)
            {
                Assert.AreEqual(alignment[0].AlignedSequences[0].Sequences[index].ToString(),
                                alignedSequence.Sequences[index].ToString());
            }

            foreach (string key in alignment[0].AlignedSequences[0].Metadata.Keys)
            {
                Assert.AreEqual(alignment[0].AlignedSequences[0].Metadata[key],
                                alignedSequence.Metadata[key]);
            }

            ApplicationLog.WriteLine(@"Alignment BVT : Validation of aligned sequence ctor completed successfully");
        }

        /// <summary>
        ///     Validate sequence alignment instance using different aligners
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="aligner">sw/nw/pw aligners</param>
        private void ValidateSequenceAlignmentCtor(string nodeName, ISequenceAligner aligner)
        {
            IAlphabet alphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                                     Constants.AlphabetNameNode));
            string origSequence1 = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode1);
            string origSequence2 = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode2);

            // Create input sequences
            var inputSequences = new List<ISequence>();
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

            // Validate the properties
            for (int ialigned = 0; ialigned < alignments[0].AlignedSequences.Count; ialigned++)
            {
                Assert.AreEqual(alignments[0].AlignedSequences[ialigned].Sequences[0].ToString(),
                                alignment.AlignedSequences[ialigned].Sequences[0].ToString());
            }

            foreach (string key in alignments[0].Metadata.Keys)
            {
                Assert.AreEqual(alignments[0].Metadata[key], alignment.Metadata[key]);
            }

            ApplicationLog.WriteLine(@"Alignment BVT : Validation of sequence alignment  ctor completed successfully");
        }

        #endregion Supporting Methods
    }
}