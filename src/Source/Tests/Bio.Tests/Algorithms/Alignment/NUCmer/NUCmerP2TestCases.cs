using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bio.Algorithms.Alignment;
using Bio.Extensions;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests.Algorithms.Alignment.NUCmer
{
    /// <summary>
    ///     NUCmer P2 Test case implementation.
    /// </summary>
    [TestFixture]
    public class NUCmerP2TestCases
    {
        private readonly Utility utilityObj = new Utility(@"TestUtils\NUCmerTestsConfig.xml");

        #region Invalidate Align NUCmer Test Cases

        /// <summary>
        ///     Validate Align() method with empty QueryList
        ///     and validate the aligned sequences
        ///     Input : Empty QueryList sequences
        ///     Validation : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void NUCmerAlignEmptyQueryList()
        {
            ValidateNUCmerEmptyAlignGeneralTestCases(null);
        }

        /// <summary>
        ///     Validate Align() method with empty ReferenceSequence
        ///     and validate the aligned sequences
        ///     Input : Empty Reference Sequence
        ///     Validation : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void NUCmerAlignEmptyReferenceList()
        {
            ValidateNUCmerEmptyAlignGeneralTestCases(null);
        }

        /// <summary>
        ///     Validate Align() method with Zero MUMLength
        ///     and validate the aligned sequences
        ///     Input : Zero MUMLength
        ///     Validation : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void NUCmerAlignInvalidMumLength()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.InvalidMumLengthSequence);
        }

        /// <summary>
        ///     Validate Align() method with MUMLength
        ///     Greater than ReferenceSequence
        ///     and validate the aligned sequences
        ///     Input : MUMLength Greater than Sequences
        ///     Validation : Validate the Exception
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void NUCmerAlignGreaterMumLength()
        {
            // Gets the reference & search sequences from the configuration file
            string[] referenceSequences = this.utilityObj.xmlUtil.GetTextValues(Constants.GreaterMumLengthSequence, Constants.ReferenceSequencesNode);
            string[] searchSequences = this.utilityObj.xmlUtil.GetTextValues(Constants.GreaterMumLengthSequence, Constants.SearchSequencesNode);
            IAlphabet seqAlphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(Constants.GreaterMumLengthSequence, Constants.AlphabetNameNode));

            var refSeqList = referenceSequences.Select(t => new Sequence(seqAlphabet, Encoding.ASCII.GetBytes(t))).Cast<ISequence>().ToList();
            var searchSeqList = searchSequences.Select(t => new Sequence(seqAlphabet, Encoding.ASCII.GetBytes(t))).Cast<ISequence>().ToList();
            string mumLength = this.utilityObj.xmlUtil.GetTextValue(Constants.GreaterMumLengthSequence, Constants.MUMLengthNode);

            var nucmerObj = new NucmerPairwiseAligner
            {
                MaximumSeparation = 0,
                SeparationFactor = 0.12f,
                BreakLength = 2,
                LengthOfMUM = long.Parse(mumLength, null),
                MinimumScore = int.Parse(this.utilityObj.xmlUtil.GetTextValue(Constants.GreaterMumLengthSequence, Constants.MinimumScoreNode), null)
            };

            IList<ISequenceAlignment> seqAlign = nucmerObj.Align(refSeqList, searchSeqList);
            Assert.AreEqual(0, seqAlign.Count);
        }

        #endregion InValidate Align NUCmer Test Cases

        #region NUCmer Simple Align Test Cases

        /// <summary>
        ///     Validate AlignSimple() method with 1000 BP Dna sequence
        ///     and validate the aligned sequences
        ///     Input : One line Dna sequence
        ///     Validation : Validate the aligned sequences
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void NUCmerAlignSimple1000BPDnaSequence()
        {
            this.ValidateNUCmerAlignSimpleGeneralTestCases(Constants.SingleDna1000BPSequence);
        }

        #endregion NUCmer Simple Align Test Cases

        #region Supported Methods

        /// <summary>
        ///     Validates the NUCmer align method for several test cases for the parameters passed.
        /// </summary>
        /// <param name="sequenceList">List of Input Sequences</param>
        private static void ValidateNUCmerEmptyAlignGeneralTestCases(IEnumerable<ISequence> sequenceList)
        {
            var nucmerObj = new NucmerPairwiseAligner();

            try
            {
                IList<ISequenceAlignment> result = nucmerObj.Align(sequenceList);
                ApplicationLog.WriteLine("NUCmer P2 : Exception not thrown");
                Assert.Fail("ArgumentNullException not thrown.");
            }
            catch (ArgumentNullException)
            {
                ApplicationLog.WriteLine("NUCmer P2: Successfully validated empty SequenceList");
            }
        }

        /// <summary>
        ///     Validates the NUCmer align method for several test cases for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Node name to be read from xml</param>
        private void ValidateNUCmerAlignGeneralTestCases(string nodeName)
        {
            // Gets the reference & search sequences from the configuration file
            string[] referenceSequences = this.utilityObj.xmlUtil.GetTextValues(nodeName, Constants.ReferenceSequencesNode);
            string[] searchSequences = this.utilityObj.xmlUtil.GetTextValues(nodeName, Constants.SearchSequencesNode);
            IAlphabet seqAlphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.AlphabetNameNode));

            var refSeqList = referenceSequences.Select(t => new Sequence(seqAlphabet, Encoding.ASCII.GetBytes(t))).Cast<ISequence>().ToList();
            var searchSeqList = searchSequences.Select(t => new Sequence(seqAlphabet, Encoding.ASCII.GetBytes(t))).Cast<ISequence>().ToList();

            var nucmerObj = new NucmerPairwiseAligner();
            string mumLength = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMLengthNode);

            // Update other values for NUCmer object
            nucmerObj.MaximumSeparation = 0;
            nucmerObj.MinimumScore = 2;
            nucmerObj.SeparationFactor = 0.12f;
            nucmerObj.BreakLength = 2;
            nucmerObj.LengthOfMUM = long.Parse(mumLength, null);
            nucmerObj.MinimumScore = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName,
                                                                               Constants.MinimumScoreNode), null);

            try
            {
                nucmerObj.Align(refSeqList, searchSeqList);
                Assert.Fail("Expected Exception: ArgumentException");
            }
            catch (ArgumentException)
            {
            }
        }

        /// <summary>
        ///     Validates the NUCmer align method for several test cases for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Node name to be read from xml</param>
        private void ValidateNUCmerAlignSimpleGeneralTestCases(string nodeName)
        {
            // Gets the reference & search sequences from the configuration file
            string[] referenceSequences = this.utilityObj.xmlUtil.GetTextValues(nodeName, Constants.ReferenceSequencesNode);
            string[] searchSequences = this.utilityObj.xmlUtil.GetTextValues(nodeName, Constants.SearchSequencesNode);

            IAlphabet seqAlphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.AlphabetNameNode));
            var refSeqList = referenceSequences.Select(t => new Sequence(seqAlphabet, Encoding.ASCII.GetBytes(t))).Cast<ISequence>().ToList();
            var searchSeqList = searchSequences.Select(t => new Sequence(seqAlphabet, Encoding.ASCII.GetBytes(t))).Cast<ISequence>().ToList();

            // Gets the mum length from the xml
            string mumLength = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode);

            var nucmerObj = new NucmerPairwiseAligner
            {
                MaximumSeparation = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode), null),
                MinimumScore = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode), null),
                SeparationFactor = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode), null),
                BreakLength = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode), null),
                LengthOfMUM = long.Parse(mumLength, null)
            };

            IList<ISequence> seqList = refSeqList.ToList();

            foreach (ISequence seq in searchSeqList)
            {
                seqList.Add(seq);
            }

            IList<ISequenceAlignment> alignSimple = nucmerObj.AlignSimple(seqList);
            string expectedSequences = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequencesNode);
            string[] expSeqArray = expectedSequences.Split(',');

            int j = 0;

            // Gets all the aligned sequences in comma separated format
            foreach (PairwiseAlignedSequence alignedSeq in alignSimple.Cast<IPairwiseSequenceAlignment>().SelectMany(seqAlignment => seqAlignment))
            {
                Assert.AreEqual(expSeqArray[j], alignedSeq.FirstSequence.ConvertToString());
                ++j;
                Assert.AreEqual(expSeqArray[j], alignedSeq.SecondSequence.ConvertToString());
                j++;
            }

            ApplicationLog.WriteLine("NUCmer P2 : Successfully validated all the aligned sequences.");
        }

        #endregion Supported Methods
    }
}