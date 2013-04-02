/****************************************************************************
 * AlignmentBvtTestCases.cs
 * 
 *   This file contains the Alignment Bvt test cases i.e., NeedlemanWunschAligner, 
 *   SmithWatermanAlignmer and SequenceAlignment
 * 
***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

using Bio.Algorithms.Alignment;
using Bio.Algorithms.Alignment.Legacy;
using Bio.IO;
using Bio.SimilarityMatrices;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.IO.FastA;
using Bio;

namespace Bio.TestAutomation.Algorithms.Alignment
{
    /// <summary>
    /// Alignment BVT test cases implementation.
    /// </summary>
    [TestClass]
    public class AlignmentBvtTestCases
    {
        #region Enums

        /// <summary>
        /// Alignment Type Parameters which are used for different test cases 
        /// based on which the test cases are executed.
        /// </summary>
        enum AlignmentType
        {
            SimpleAlign,
            Align
        };

        /// <summary>
        /// Alignment Parameters which are used for different test cases 
        /// based on which the test cases are executed.
        /// </summary>
        enum AlignmentParamType
        {
            AlignTwo,
            AlignList,
            AllParam
        };

        #endregion Enums

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static AlignmentBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region NeedlemanWunschAligner BVT Test cases

        /// <summary>
        /// Pass a Valid Sequence with valid GapPenalty, Similarity Matrix 
        /// which is in a text file using the method Align(sequence1, sequence2) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : FastA File
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NeedlemanWunschSimpleAlignTwoSequencesFromTextFile()
        {
            ValidateNeedlemanWunschAlignment(true, AlignmentParamType.AlignTwo);
        }

        /// <summary>
        /// Pass a Valid Sequence with valid GapPenalty, Similarity Matrix 
        /// which is passed in code using the method Align(sequence1, sequence2) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : sequence in xml
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NeedlemanWunschSimpleAlignTwoSequencesFromXml()
        {
            ValidateNeedlemanWunschAlignment(false, AlignmentParamType.AlignTwo);
        }

        /// <summary>
        /// Pass a Valid Sequence with valid GapPenalty, Similarity Matrix 
        /// which is in a text file using the method Align(List) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : FastA File
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NeedlemanWunschSimpleAlignListSequencesFromTextFile()
        {
            ValidateNeedlemanWunschAlignment(true, AlignmentParamType.AlignList);
        }

        /// <summary>
        /// Pass a Valid Sequence with valid GapPenalty, Similarity Matrix 
        /// which is passed in code using the method Align(List) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : sequence in xml
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NeedlemanWunschSimpleAlignListSequencesFromXml()
        {
            ValidateNeedlemanWunschAlignment(false, AlignmentParamType.AlignList);
        }

        /// <summary>
        /// Pass a Valid Sequence with valid GapPenalty, Similarity Matrix 
        /// which is in a text file using the method Align(all parameters) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : Text File i.e., Fasta
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NeedlemanWunschSimpleAlignAllParamsFromTextFile()
        {
            ValidateNeedlemanWunschAlignment(true, AlignmentParamType.AllParam);
        }

        /// <summary>
        /// Pass a Valid Sequence with valid GapPenalty, Similarity Matrix 
        /// which is passed in code using the method Align(all parameters) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : sequence in xml
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NeedlemanWunschSimpleAlignAllParamsFromXml()
        {
            ValidateNeedlemanWunschAlignment(false, AlignmentParamType.AllParam);
        }
        #region Gap Extension Cost inclusion Test cases

        /// <summary>
        /// Pass a Valid Sequence with valid Gap Open Cost, Gap Extension Cost, Similarity Matrix 
        /// which is in a text file using the method Align(sequence1, sequence2) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : FastA File
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NeedlemanWunschAlignTwoSequencesFromTextFile()
        {
            ValidateNeedlemanWunschAlignment(true, AlignmentParamType.AlignTwo,
                AlignmentType.Align);
        }

        /// <summary>
        /// Pass a Valid Sequence with valid Gap Open Cost, Gap Extension Cost, Similarity Matrix 
        /// which is in a text file using the method Align(List) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : FastA File
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NeedlemanWunschAlignListSequencesFromTextFile()
        {
            ValidateNeedlemanWunschAlignment(true, AlignmentParamType.AlignList,
                AlignmentType.Align);
        }

        /// <summary>
        /// Pass a Valid Sequence with valid Gap Open Cost, Gap Extension Cost, Similarity Matrix 
        /// which is in a text file using the method Align(all parameters) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : Text File i.e., Fasta
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NeedlemanWunschAlignAllParamsFromTextFile()
        {
            ValidateNeedlemanWunschAlignment(true, AlignmentParamType.AllParam,
                AlignmentType.Align);
        }
        #endregion Gap Extension Cost inclusion Test cases

        #endregion NeedlemanWunschAligner BVT Test cases

        #region SmithWatermanAligner BVT Test cases

        /// <summary>
        /// Pass a Valid Sequence with valid GapPenalty, Similarity Matrix 
        /// which is in a text file using the method Align(sequence1, sequence2) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : FastA File
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void SmithWatermanSimpleAlignTwoSequencesFromTextFile()
        {
            ValidateSmithWatermanAlignment(true, AlignmentParamType.AlignTwo);
        }

        /// <summary>
        /// Pass a Valid Sequence with valid GapPenalty, Similarity Matrix 
        /// which is passed in code using the method Align(sequence1, sequence2) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : sequence in xml
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void SmithWatermanSimpleAlignTwoSequencesFromXml()
        {
            ValidateSmithWatermanAlignment(false, AlignmentParamType.AlignTwo);
        }

        /// <summary>
        /// Pass a Valid Sequence with valid GapPenalty, Similarity Matrix 
        /// which is in a text file using the method Align(List) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : FastA File
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void SmithWatermanSimpleAlignListSequencesFromTextFile()
        {
            ValidateSmithWatermanAlignment(true, AlignmentParamType.AlignList);
        }


        /// <summary>
        /// Pass a Valid Sequence with valid GapPenalty, Similarity Matrix 
        /// which is passed in code using the method Align(List) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : sequence in xml
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void SmithWatermanSimpleAlignListSequencesFromXml()
        {
            ValidateSmithWatermanAlignment(false, AlignmentParamType.AlignList);
        }

        /// <summary>
        /// Pass a Valid Sequence with valid GapPenalty, Similarity Matrix 
        /// which is in a text file using the method Align(all parameters) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : Text File i.e., Fasta
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void SmithWatermanSimpleAlignAllParamsFromTextFile()
        {
            ValidateSmithWatermanAlignment(true, AlignmentParamType.AllParam);
        }

        /// <summary>
        /// Pass a Valid Sequence with valid GapPenalty, Similarity Matrix 
        /// which is passed in code using the method Align(all parameters) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : sequence in xml
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void SmithWatermanSimpleAlignAllParamsFromXml()
        {
            ValidateSmithWatermanAlignment(false, AlignmentParamType.AllParam);
        }

        #region Gap Extension Cost inclusion Test cases

        /// <summary>
        /// Pass a Valid Sequence with valid Gap Open Cost, Gap Extension Cost, Similarity Matrix 
        /// which is in a text file using the method Align(sequence1, sequence2) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : FastA File
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void SmithWatermanAlignTwoSequencesFromTextFile()
        {
            ValidateSmithWatermanAlignment(true, AlignmentParamType.AlignTwo, AlignmentType.Align);
        }

        /// <summary>
        /// Pass a Valid Sequence with valid Gap Open Cost, Gap Extension Cost, Similarity Matrix 
        /// which is in a text file using the method Align(List) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : FastA File
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void SmithWatermanAlignListSequencesFromTextFile()
        {
            ValidateSmithWatermanAlignment(true, AlignmentParamType.AlignList, AlignmentType.Align);
        }

        /// <summary>
        /// Pass a Valid Sequence with valid Gap Open Cost, Gap Extension Cost, Similarity Matrix 
        /// which is in a text file using the method Align(all parameters) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : Text File i.e., Fasta
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void SmithWatermanAlignAllParamsFromTextFile()
        {
            ValidateSmithWatermanAlignment(true, AlignmentParamType.AllParam, AlignmentType.Align);
        }

        #endregion Gap Extension Cost inclusion Test cases

        #endregion SmithWatermanAligner BVT Test cases

        #region Sequence Alignment BVT Test cases

        /// <summary>
        /// Pass a valid sequence to AddSequence() method and validate the same.
        /// Input : Sequence read from xml file.
        /// Validation : Added sequences are got back and validated.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void SequenceAlignmentAddSequence()
        {
            // Read the xml file for getting both the files for aligning.
            string origSequence1 = utilityObj.xmlUtil.GetTextValue(
                Constants.AlignAlgorithmNodeName,
                Constants.SequenceNode1);
            string origSequence2 = utilityObj.xmlUtil.GetTextValue(Constants.AlignAlgorithmNodeName,
                Constants.SequenceNode2);

            IAlphabet alphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(
                Constants.AlignAlgorithmNodeName,
                Constants.AlphabetNameNode));

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "SequenceAlignment BVT : First sequence used is '{0}'.",
                origSequence1));
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "SequenceAlignment BVT : Second sequence used is '{0}'.",
                origSequence2));

            Console.WriteLine(string.Format((IFormatProvider)null,
                "SequenceAlignment BVT : First sequence used is '{0}'.",
                origSequence1));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "SequenceAlignment BVT : Second sequence used is '{0}'.",
                origSequence2));

            // Create two sequences
            ISequence aInput = new Sequence(alphabet, origSequence1);
            ISequence bInput = new Sequence(alphabet, origSequence2);

            // Add the sequences to the Sequence alignment object using AddSequence() method.
            IList<IPairwiseSequenceAlignment> sequenceAlignmentObj =
                new List<IPairwiseSequenceAlignment>();

            PairwiseAlignedSequence alignSeq = new PairwiseAlignedSequence();
            alignSeq.FirstSequence = aInput;
            alignSeq.SecondSequence = bInput;
            IPairwiseSequenceAlignment seqAlignObj = new PairwiseSequenceAlignment();
            seqAlignObj.Add(alignSeq);
            sequenceAlignmentObj.Add(seqAlignObj);

            // Read the output back and validate the same.
            IList<PairwiseAlignedSequence> newAlignedSequences =
                sequenceAlignmentObj[0].PairwiseAlignedSequences;

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "SequenceAlignment BVT : First sequence read is '{0}'.",
                origSequence1));
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "SequenceAlignment BVT : Second sequence read is '{0}'.",
                origSequence2));

            Console.WriteLine(string.Format((IFormatProvider)null,
                "SequenceAlignment BVT : First sequence read is '{0}'.",
                origSequence1));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "SequenceAlignment BVT : Second sequence read is '{0}'.",
                origSequence2));

            Assert.AreEqual(new String(newAlignedSequences[0].FirstSequence.Select(a => (char)a).ToArray()), origSequence1);
            Assert.AreEqual(new String(newAlignedSequences[0].SecondSequence.Select(a => (char)a).ToArray()), origSequence2);
        }

        #endregion Sequence Alignment BVT Test cases

        #region Aligned Sequence BVT Test Cases

        /// <summary>
        /// Validate Aligned Sequence ctor by adding aligned sequnece and 
        /// metadata using smithwatermanaligner
        /// Input : dna aligned sequence
        /// Output : dna aligned sequence instance
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateAlignedSequenceCtor()
        {
            ValidateAlignedSequenceCtor(Constants.SmithWatermanAlignAlgorithmNodeName,
                SequenceAligners.SmithWaterman);
        }

        //private void ValidateAlignedSequence(string nodeName, ISequenceAligner aligner);

        /// <summary>
        /// Validate Aligned Sequence by passing IAligned sequence of dna sequence 
        /// using smithwatermanaligner
        /// Input : dna sequence
        /// Output : dna aligned sequence instance
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateAlignedSequenceWithSmithWatermanAligner()
        {
            ValidateAlignedSequence(Constants.SmithWatermanAlignAlgorithmNodeName,
                SequenceAligners.SmithWaterman);
        }

        /// <summary>
        /// Validate Aligned Sequence by passing IAligned sequence of dna sequence 
        /// using needlemanwunschaligner
        /// Input : dna sequence
        /// Output : dna aligned sequence instance
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateAlignedSequenceWithNeedlemanWunschAligner()
        {
            ValidateAlignedSequence(Constants.NeedlemanWunschAlignAlgorithmNodeName,
                SequenceAligners.NeedlemanWunsch);
        }


        #endregion

        #region Sequence Alignment BVT Test Cases

        /// <summary>
        /// Validate Sequence Alignment ctor by passing ISequenceAlignment of dna sequence 
        /// using smithwatermanaligner
        /// Input : dna sequence
        /// Output : dna sequence alignment instance
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSequenceAlignmentCtorWithSmithWatermanAligner()
        {
            ValidateSequenceAlignmentCtor(Constants.SmithWatermanAlignAlgorithmNodeName,
                SequenceAligners.SmithWaterman);
        }

        /// <summary>
        /// Validate Sequence Alignment ctor by passing ISequenceAlignment of dna sequence 
        /// using needlemanwunschaligner
        /// Input : dna sequence
        /// Output : dna sequence alignment instance
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSequenceAlignmentCtorWithNeedlemanWunschAligner()
        {
            ValidateSequenceAlignmentCtor(Constants.NeedlemanWunschAlignAlgorithmNodeName,
                SequenceAligners.NeedlemanWunsch);
        }


        /// <summary>
        /// Validate Sequence Alignment by passing ISequenceAlignment of dna sequence 
        /// using smithwatermanaligner
        /// Input : dna sequence
        /// Output : dna sequence alignment instance
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSequenceAlignmentWithSmithWatermanAligner()
        {
            ValidateSequenceAlignment(Constants.SmithWatermanAlignAlgorithmNodeName,
                SequenceAligners.SmithWaterman);
        }

        /// <summary>
        /// Validate Sequence Alignment by passing ISequenceAlignment of dna sequence 
        /// using needlemanwunschaligner
        /// Input : dna sequence
        /// Output : dna sequence alignment instance
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSequenceAlignmentWithNeedlemanWunschAligner()
        {
            ValidateSequenceAlignment(Constants.NeedlemanWunschAlignAlgorithmNodeName,
                SequenceAligners.NeedlemanWunsch);
        }



        #endregion

        #region Supporting Methods

        /// <summary>
        /// Validates NeedlemanWunschAlignment algorithm for the parameters passed.
        /// </summary>
        /// <param name="isTextFile">Is text file an input.</param>
        /// <param name="alignParam">parameter based on which certain validations are done.</param>
        void ValidateNeedlemanWunschAlignment(bool isTextFile, AlignmentParamType alignParam)
        {
            ValidateNeedlemanWunschAlignment(isTextFile, alignParam, AlignmentType.SimpleAlign);
        }

        /// <summary>
        /// Validates NeedlemanWunschAlignment algorithm for the parameters passed.
        /// </summary>
        /// <param name="isTextFile">Is text file an input.</param>
        /// <param name="alignParam">parameter based on which certain validations are done.</param>
        /// <param name="alignType">Is the Align type Simple or Align with Gap Extension cost?</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        void ValidateNeedlemanWunschAlignment(bool isTextFile, AlignmentParamType alignParam, AlignmentType alignType)
        {
            ISequence aInput = null;
            ISequence bInput = null;

            IAlphabet alphabet = Utility.GetAlphabet(
                utilityObj.xmlUtil.GetTextValue(Constants.NeedlemanWunschAlignAlgorithmNodeName,
                Constants.AlphabetNameNode));

            if (isTextFile)
            {
                // Read the xml file for getting both the files for aligning.
                string filePath1 = utilityObj.xmlUtil.GetTextValue(
                    Constants.NeedlemanWunschAlignAlgorithmNodeName,
                    Constants.FilePathNode1);
                string filePath2 = utilityObj.xmlUtil.GetTextValue(
                    Constants.NeedlemanWunschAlignAlgorithmNodeName,
                    Constants.FilePathNode2);

                // Parse the files and get the sequence.
                using (FastAParser parseObjectForFile1 = new FastAParser(filePath1))
                {
                    parseObjectForFile1.Alphabet = alphabet;
                    aInput = parseObjectForFile1.Parse().ElementAt(0);
                }

                using (FastAParser parseObjectForFile2 = new FastAParser(filePath2))
                {
                    parseObjectForFile2.Alphabet = alphabet;
                    bInput = parseObjectForFile2.Parse().ElementAt(0);
                }
            }
            else
            {
                // Read the xml file for getting both the files for aligning.
                string origSequence1 = utilityObj.xmlUtil.GetTextValue(
                    Constants.NeedlemanWunschAlignAlgorithmNodeName,
                    Constants.SequenceNode1);
                string origSequence2 = utilityObj.xmlUtil.GetTextValue(
                    Constants.NeedlemanWunschAlignAlgorithmNodeName,
                    Constants.SequenceNode2);

                aInput = new Sequence(alphabet, origSequence1);
                bInput = new Sequence(alphabet, origSequence2);
            }

            string blosumFilePath = utilityObj.xmlUtil.GetTextValue(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                Constants.BlosumFilePathNode);

            SimilarityMatrix sm = new SimilarityMatrix(blosumFilePath);
            int gapOpenCost = int.Parse(utilityObj.xmlUtil.GetTextValue(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                Constants.GapOpenCostNode), (IFormatProvider)null);

            int gapExtensionCost = int.Parse(utilityObj.xmlUtil.GetTextValue(
                Constants.NeedlemanWunschAlignAlgorithmNodeName,
                Constants.GapExtensionCostNode), (IFormatProvider)null);

            NeedlemanWunschAligner needlemanWunschObj = new NeedlemanWunschAligner();
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
                            result = needlemanWunschObj.Align(new List<ISequence>() { aInput, bInput });
                            break;
                        default:
                            result = needlemanWunschObj.AlignSimple(new List<ISequence>() { aInput, bInput });
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
            string expectedSequence1 = string.Empty;
            string expectedSequence2 = string.Empty;

            string expectedScore = string.Empty;

            switch (alignType)
            {
                case AlignmentType.Align:
                    expectedScore = utilityObj.xmlUtil.GetTextValue(
                        Constants.NeedlemanWunschAlignAlgorithmNodeName,
                        Constants.ExpectedGapExtensionScoreNode);
                    expectedSequence1 = utilityObj.xmlUtil.GetTextValue(
                        Constants.NeedlemanWunschAlignAlgorithmNodeName,
                        Constants.ExpectedGapExtensionSequence1Node);
                    expectedSequence2 = utilityObj.xmlUtil.GetTextValue(
                        Constants.NeedlemanWunschAlignAlgorithmNodeName,
                        Constants.ExpectedGapExtensionSequence2Node);
                    break;
                default:
                    expectedScore = utilityObj.xmlUtil.GetTextValue(
                        Constants.NeedlemanWunschAlignAlgorithmNodeName,
                        Constants.ExpectedScoreNode);
                    expectedSequence1 = utilityObj.xmlUtil.GetTextValue(
                        Constants.NeedlemanWunschAlignAlgorithmNodeName,
                        Constants.ExpectedSequenceNode1);
                    expectedSequence2 = utilityObj.xmlUtil.GetTextValue(
                        Constants.NeedlemanWunschAlignAlgorithmNodeName,
                        Constants.ExpectedSequenceNode2);
                    break;
            }

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();

            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            PairwiseAlignedSequence alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(alphabet, expectedSequence1);
            alignedSeq.SecondSequence = new Sequence(alphabet, expectedSequence2);
            alignedSeq.Score = Convert.ToInt32(expectedScore, (IFormatProvider)null);
            align.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(align);
            Assert.IsTrue(CompareAlignment(result, expectedOutput));

            align = null;
            alignedSeq = null;
            aInput = null;
            bInput = null;
            needlemanWunschObj = null;
            alphabet = null;

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "NeedlemanWunschAligner BVT : Final Score '{0}'.", expectedScore));
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "NeedlemanWunschAligner BVT : Aligned First Sequence is '{0}'.",
               expectedSequence1));
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "NeedlemanWunschAligner BVT : Aligned Second Sequence is '{0}'.",
                expectedSequence2));

            Console.WriteLine(string.Format((IFormatProvider)null,
                "NeedlemanWunschAligner BVT : Final Score '{0}'.", expectedScore));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "NeedlemanWunschAligner BVT : Aligned First Sequence is '{0}'.",
               expectedSequence1));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "NeedlemanWunschAligner BVT : Aligned Second Sequence is '{0}'.",
                expectedSequence2));
        }

        /// <summary>
        /// Validates SmithWatermanAlignment algorithm for the parameters passed.
        /// </summary>
        /// <param name="isTextFile">Is text file an input.</param>
        /// <param name="alignParam">parameter based on which certain validations are done.</param>
        void ValidateSmithWatermanAlignment(bool isTextFile, AlignmentParamType alignParam)
        {
            ValidateSmithWatermanAlignment(isTextFile, alignParam, AlignmentType.SimpleAlign);
        }

        /// <summary>
        /// Validates SmithWatermanAlignment algorithm for the parameters passed.
        /// </summary>
        /// <param name="isTextFile">Is text file an input.</param>
        /// <param name="alignParam">parameter based on which certain validations are done.</param>
        /// <param name="alignType">Is the Align type Simple or Align with Gap Extension cost?</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        void ValidateSmithWatermanAlignment(bool isTextFile, AlignmentParamType alignParam,
            AlignmentType alignType)
        {
            ISequence aInput = null;
            ISequence bInput = null;

            IAlphabet alphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(
                Constants.SmithWatermanAlignAlgorithmNodeName,
                Constants.AlphabetNameNode));

            if (isTextFile)
            {
                // Read the xml file for getting both the files for aligning.
                string filePath1 = utilityObj.xmlUtil.GetTextValue(
                    Constants.SmithWatermanAlignAlgorithmNodeName,
                    Constants.FilePathNode1);
                string filePath2 = utilityObj.xmlUtil.GetTextValue(
                    Constants.SmithWatermanAlignAlgorithmNodeName,
                    Constants.FilePathNode2);

                // Parse the files and get the sequence.
                using (FastAParser parseObjectForFile1 = new FastAParser(filePath1))
                {
                    parseObjectForFile1.Alphabet = alphabet;
                    aInput = parseObjectForFile1.Parse().ElementAt(0);
                }

                using (FastAParser parseObjectForFile2 = new FastAParser(filePath2))
                {
                    parseObjectForFile2.Alphabet = alphabet;
                    bInput = parseObjectForFile2.Parse().ElementAt(0);
                }
            }
            else
            {
                // Read the xml file for getting both the files for aligning.
                string origSequence1 = utilityObj.xmlUtil.GetTextValue(
                    Constants.SmithWatermanAlignAlgorithmNodeName,
                    Constants.SequenceNode1);
                string origSequence2 = utilityObj.xmlUtil.GetTextValue(
                    Constants.SmithWatermanAlignAlgorithmNodeName,
                    Constants.SequenceNode2);
                aInput = new Sequence(alphabet, origSequence1);
                bInput = new Sequence(alphabet, origSequence2);
            }

            string blosumFilePath = utilityObj.xmlUtil.GetTextValue(
                Constants.SmithWatermanAlignAlgorithmNodeName,
                Constants.BlosumFilePathNode);

            SimilarityMatrix sm = new SimilarityMatrix(blosumFilePath);
            int gapOpenCost = int.Parse(utilityObj.xmlUtil.GetTextValue(
                Constants.SmithWatermanAlignAlgorithmNodeName,
                Constants.GapOpenCostNode), (IFormatProvider)null);

            int gapExtensionCost = int.Parse(utilityObj.xmlUtil.GetTextValue(
                Constants.SmithWatermanAlignAlgorithmNodeName,
                Constants.GapExtensionCostNode), (IFormatProvider)null);

            SmithWatermanAligner smithWatermanObj = new SmithWatermanAligner();

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
                            result = smithWatermanObj.Align(new List<ISequence>() { aInput, bInput });
                            break;
                        default:
                            result = smithWatermanObj.AlignSimple(new List<ISequence>() { aInput, bInput });
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

            // Validating the Equals method in Aligner.
            Assert.IsTrue(smithWatermanObj.Equals(smithWatermanObj));

            // Read the xml file for getting both the files for aligning.
            string expectedSequence1 = string.Empty;
            string expectedSequence2 = string.Empty;

            string expectedScore = string.Empty;

            switch (alignType)
            {
                case AlignmentType.Align:
                    expectedScore = utilityObj.xmlUtil.GetTextValue(
                        Constants.SmithWatermanAlignAlgorithmNodeName,
                        Constants.ExpectedGapExtensionScoreNode);
                    expectedSequence1 = utilityObj.xmlUtil.GetTextValue(
                        Constants.SmithWatermanAlignAlgorithmNodeName,
                        Constants.ExpectedGapExtensionSequence1Node);
                    expectedSequence2 = utilityObj.xmlUtil.GetTextValue(
                        Constants.SmithWatermanAlignAlgorithmNodeName,
                        Constants.ExpectedGapExtensionSequence2Node);
                    break;
                default:
                    expectedScore = utilityObj.xmlUtil.GetTextValue(
                        Constants.SmithWatermanAlignAlgorithmNodeName,
                        Constants.ExpectedScoreNode);
                    expectedSequence1 = utilityObj.xmlUtil.GetTextValue(
                        Constants.SmithWatermanAlignAlgorithmNodeName,
                        Constants.ExpectedSequenceNode1);
                    expectedSequence2 = utilityObj.xmlUtil.GetTextValue(
                        Constants.SmithWatermanAlignAlgorithmNodeName,
                        Constants.ExpectedSequenceNode2);
                    break;
            }

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();

            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            PairwiseAlignedSequence alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(alphabet, expectedSequence1);
            alignedSeq.SecondSequence = new Sequence(alphabet, expectedSequence2);
            alignedSeq.Score = Convert.ToInt32(expectedScore, (IFormatProvider)null);
            align.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(align);
            Assert.IsTrue(CompareAlignment(result, expectedOutput));

            align = null;
            alignedSeq = null;
            aInput = null;
            bInput = null;
            smithWatermanObj = null;
            alphabet = null;

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "SmithWatermanAligner BVT : Final Score '{0}'.", expectedScore));
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "SmithWatermanAligner BVT : Aligned First Sequence is '{0}'.",
                expectedSequence1));
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "SmithWatermanAligner BVT : Aligned Second Sequence is '{0}'.",
                expectedSequence2));

            Console.WriteLine(string.Format((IFormatProvider)null,
                "SmithWatermanAligner BVT : Final Score '{0}'.", expectedScore));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "SmithWatermanAligner BVT : Aligned First Sequence is '{0}'.",
                expectedSequence1));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "SmithWatermanAligner BVT : Aligned Second Sequence is '{0}'.",
                expectedSequence2));
        }

        /// <summary>
        /// Compare the alignment of mummer and defined alignment
        /// </summary>
        /// <param name="result">output of Aligners</param>
        /// <param name="expectedAlignment">expected output</param>
        /// <returns>Compare result of alignments</returns>
        private static bool CompareAlignment(IList<IPairwiseSequenceAlignment> actualAlignment,
             IList<IPairwiseSequenceAlignment> expectedAlignment)
        {
            bool output = true;

            if (actualAlignment.Count == expectedAlignment.Count)
            {
                for (int resultCount = 0; resultCount < actualAlignment.Count; resultCount++)
                {
                    if (actualAlignment[resultCount].PairwiseAlignedSequences.Count == expectedAlignment[resultCount].PairwiseAlignedSequences.Count)
                    {
                        for (int alignSeqCount = 0; alignSeqCount < actualAlignment[resultCount].PairwiseAlignedSequences.Count; alignSeqCount++)
                        {
                            // Validates the First Sequence, Second Sequence and Score                            
                            if (new string(actualAlignment[resultCount].PairwiseAlignedSequences[alignSeqCount].FirstSequence.Select(a => (char)a).ToArray()).Equals(
                                new string(expectedAlignment[resultCount].PairwiseAlignedSequences[alignSeqCount].FirstSequence.Select(a => (char)a).ToArray()))
                            && new string(actualAlignment[resultCount].PairwiseAlignedSequences[alignSeqCount].SecondSequence.Select(a => (char)a).ToArray()).Equals(
                               new string(expectedAlignment[resultCount].PairwiseAlignedSequences[alignSeqCount].SecondSequence.Select(a => (char)a).ToArray()))
                            && actualAlignment[resultCount].PairwiseAlignedSequences[alignSeqCount].Score ==
                                expectedAlignment[resultCount].PairwiseAlignedSequences[alignSeqCount].Score)
                            {
                                output = true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            actualAlignment.Clear();
            expectedAlignment.Clear();
            actualAlignment = null;
            expectedAlignment = null;
            return output;
        }

        /// <summary>
        /// Validate aligned sequence instance using different aligners
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="aligner">sw/nw/pw aligners</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        private void ValidateAlignedSequence(string nodeName, ISequenceAligner aligner)
        {
            IAlphabet alphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.AlphabetNameNode));
            string origSequence1 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode1);
            string origSequence2 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode2);

            // Create input sequences
            List<ISequence> inputSequences = new List<ISequence>();
            inputSequences.Add(new Sequence(alphabet, origSequence1));
            inputSequences.Add(new Sequence(alphabet, origSequence2));

            // Get aligned sequences
            IList<ISequenceAlignment> alignment = aligner.Align(inputSequences);

            // Create AlignedSequence instance
            IAlignedSequence sequence = alignment[0].AlignedSequences[0];

            // Validate the alignedsequence properties
            Assert.AreEqual(alignment[0].AlignedSequences[0].Sequences, sequence.Sequences);
            Assert.AreEqual(alignment[0].AlignedSequences[0].Metadata, sequence.Metadata);

            Console.WriteLine(@"Alignment BVT : Validation of 
                               aligned sequence completed successfully");
            ApplicationLog.WriteLine(@"Alignment BVT : Validation of 
                                     aligned sequence completed successfully");
        }

        /// <summary>
        /// Validate sequence alignment instance using different aligners
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="aligner">sw/nw/pw aligners</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        private void ValidateSequenceAlignment(string nodeName, ISequenceAligner aligner)
        {
            IAlphabet alphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.AlphabetNameNode));
            string origSequence1 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode1);
            string origSequence2 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode2);

            // Create input sequences
            List<ISequence> inputSequences = new List<ISequence>();
            inputSequences.Add(new Sequence(alphabet, origSequence1));
            inputSequences.Add(new Sequence(alphabet, origSequence2));

            // Get aligned sequences
            IList<ISequenceAlignment> alignments = aligner.Align(inputSequences);
            ISequenceAlignment alignment = alignments[0];

            Assert.AreEqual(alignments[0].AlignedSequences.Count, alignment.AlignedSequences.Count);
            Assert.AreEqual(alignments[0].Metadata, alignment.Metadata);
            Assert.AreEqual(inputSequences[0].ToString(), alignment.Sequences[0].ToString());

            Console.WriteLine(@"Alignment BVT : Validation of 
                               sequence alignment completed successfully");
            ApplicationLog.WriteLine(@"Alignment BVT : Validation of 
                                     sequence alignment completed successfully");
        }

        /// <summary>
        /// Validate aligned sequence instance using different aligners
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="aligner">sw/nw/pw aligners</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        private void ValidateAlignedSequenceCtor(string nodeName, ISequenceAligner aligner)
        {
            IAlphabet alphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.AlphabetNameNode));
            string origSequence1 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode1);
            string origSequence2 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode2);

            // Create input sequences
            List<ISequence> inputSequences = new List<ISequence>();
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

            Console.WriteLine(@"Alignment BVT : Validation of 
                               aligned sequence ctor completed successfully");
            ApplicationLog.WriteLine(@"Alignment BVT : Validation of 
                                     aligned sequence ctor completed successfully");
        }

        /// <summary>
        /// Validate sequence alignment instance using different aligners
        /// </summary>
        /// <param name="nodeName">xml node name</param>
        /// <param name="aligner">sw/nw/pw aligners</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        private void ValidateSequenceAlignmentCtor(string nodeName, ISequenceAligner aligner)
        {
            IAlphabet alphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.AlphabetNameNode));
            string origSequence1 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode1);
            string origSequence2 = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode2);

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

            Console.WriteLine(@"Alignment BVT : Validation of 
                               sequence alignment ctor completed successfully");
            ApplicationLog.WriteLine(@"Alignment BVT : Validation of 
                                     sequence alignment  ctor completed successfully");
        }

        #endregion Supporting Methods
    }
}
