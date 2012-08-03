/****************************************************************************
 * NUCmerP2TestCases.cs
 * 
 *   This file contains NUCmer P2 test cases
 * 
***************************************************************************/

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using Bio;
using Bio.Algorithms.Alignment;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.TestAutomation.Algorithms.Alignment
{
    /// <summary>
    /// NUCmer P2 Test case implementation.
    /// </summary>
    [TestClass]
    public class NUCmerP2TestCases
    {

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\NUCmerTestsConfig.xml");
        ASCIIEncoding encodingObj = new ASCIIEncoding();

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static NUCmerP2TestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region InValidate Align NUCmer Test Cases

        /// <summary>
        /// Validate Align() method with empty QueryList
        /// and validate the aligned sequences
        /// Input : Empty QueryList sequences
        /// Validation : Validate the Exception
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void NUCmerAlignEmptyQueryList()
        {
            ValidateNUCmerEmptyAlignGeneralTestCases(null);
        }

        /// <summary>
        /// Validate Align() method with empty ReferenceSequence
        /// and validate the aligned sequences
        /// Input : Empty Reference Sequence
        /// Validation : Validate the Exception
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void NUCmerAlignEmptyReferenceList()
        {
            ValidateNUCmerEmptyAlignGeneralTestCases(null);
        }

        /// <summary>
        /// Validate Align() method with Zero MUMLength
        /// and validate the aligned sequences
        /// Input : Zero MUMLength
        /// Validation : Validate the Exception
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void NUCmerAlignInvalidMumLength()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.InvalidMumLengthSequence);
        }

        /// <summary>
        /// Validate Align() method with MUMLength
        /// Greater than ReferenceSequence
        /// and validate the aligned sequences
        /// Input : MUMLength Greater than Sequences
        /// Validation : Validate the Exception
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void NUCmerAlignGreaterMumLength()
        {
            string mumLength = String.Empty;
            string[] referenceSequences = null;
            string[] searchSequences = null;
            List<ISequence> refSeqList = new List<ISequence>();
            List<ISequence> searchSeqList = new List<ISequence>();

            // Gets the reference & search sequences from the configurtion file
            referenceSequences = utilityObj.xmlUtil.GetTextValues(Constants.GreaterMumLengthSequence,
                Constants.ReferenceSequencesNode);
            searchSequences = utilityObj.xmlUtil.GetTextValues(Constants.GreaterMumLengthSequence,
                Constants.SearchSequencesNode);

            IAlphabet seqAlphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(Constants.GreaterMumLengthSequence,
                Constants.AlphabetNameNode));

            for (int i = 0; i < referenceSequences.Length; i++)
            {
                ISequence referSeq = new Sequence(seqAlphabet, encodingObj.GetBytes(referenceSequences[i]));
                refSeqList.Add(referSeq);
            }

            for (int i = 0; i < searchSequences.Length; i++)
            {
                ISequence searchSeq = new Sequence(seqAlphabet, encodingObj.GetBytes(searchSequences[i]));
                searchSeqList.Add(searchSeq);
            }

            NucmerPairwiseAligner nucmerObj = new NucmerPairwiseAligner();
            mumLength = utilityObj.xmlUtil.GetTextValue(Constants.GreaterMumLengthSequence, Constants.MUMLengthNode);

            // Update other values for NUCmer object
            nucmerObj.MaximumSeparation = 0;
            nucmerObj.MinimumScore = 2;
            nucmerObj.SeparationFactor = 0.12f;
            nucmerObj.BreakLength = 2;
            nucmerObj.LengthOfMUM = long.Parse(mumLength, null);
            nucmerObj.MinimumScore = int.Parse(utilityObj.xmlUtil.GetTextValue(Constants.GreaterMumLengthSequence,
                Constants.MinimumScoreNode), (IFormatProvider)null);


            IList<ISequenceAlignment> seqAlign = nucmerObj.Align(refSeqList, searchSeqList);

            if (seqAlign.Count == 0)
            {
                Console.WriteLine("NUCmer P2 : Successfully validated the invalid mumlength");
                ApplicationLog.WriteLine("NUCmer P2 : Successfully validated the invalid mumlength");
            }
            else
                Assert.Fail();
        }

        #endregion InValidate Align NUCmer Test Cases

        #region NUCmer Simple Align Test Cases

        /// <summary>
        /// Validate AlignSimple() method with 1000 BP Dna sequence 
        /// and validate the aligned sequences
        /// Input : One line Dna sequence
        /// Validation : Validate the aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void NUCmerAlignSimple1000BPDnaSequence()
        {
            ValidateNUCmerAlignSimpleGeneralTestCases(Constants.SingleDna1000BPSequence);
        }

        #endregion NUCmer Simple Align Test Cases

        #region Supported Methods

        /// <summary>
        /// Validates the NUCmer align method for several test cases for the parameters passed.
        /// </summary>
        /// <param name="sequenceList">List of Input Sequences</param>
        private static void ValidateNUCmerEmptyAlignGeneralTestCases(IEnumerable<ISequence> sequenceList)
        {
            NucmerPairwiseAligner nucmerObj = new NucmerPairwiseAligner();

            try
            {
                IList<ISequenceAlignment> result = nucmerObj.Align(sequenceList);
                Assert.Fail();
                ApplicationLog.WriteLine("NUCmer P2 : Exception not thrown");
            }
            catch (System.ArgumentNullException)
            {
                ApplicationLog.WriteLine("NUCmer P2: Successfully validated empty SequenceList");
            }
        }

        /// <summary>
        /// Validates the NUCmer align method for several test cases for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Node name to be read from xml</param>
        void ValidateNUCmerAlignGeneralTestCases(string nodeName)
        {
            string mumLength = String.Empty;
            bool exThrown = false;
            string[] referenceSequences = null;
            string[] searchSequences = null;
            List<ISequence> refSeqList = new List<ISequence>();
            List<ISequence> searchSeqList = new List<ISequence>();

            // Gets the reference & search sequences from the configurtion file
            referenceSequences = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.ReferenceSequencesNode);
            searchSequences = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.SearchSequencesNode);

            IAlphabet seqAlphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.AlphabetNameNode));

            for (int i = 0; i < referenceSequences.Length; i++)
            {
                ISequence referSeq = new Sequence(seqAlphabet, encodingObj.GetBytes(referenceSequences[i]));
                refSeqList.Add(referSeq);
            }

            for (int i = 0; i < searchSequences.Length; i++)
            {
                ISequence searchSeq = new Sequence(seqAlphabet, encodingObj.GetBytes(searchSequences[i]));
                searchSeqList.Add(searchSeq);
            }

            NucmerPairwiseAligner nucmerObj = new NucmerPairwiseAligner();
            mumLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMLengthNode);

            // Update other values for NUCmer object
            nucmerObj.MaximumSeparation = 0;
            nucmerObj.MinimumScore = 2;
            nucmerObj.SeparationFactor = 0.12f;
            nucmerObj.BreakLength = 2;
            nucmerObj.LengthOfMUM = long.Parse(mumLength, null);
            nucmerObj.MinimumScore = int.Parse(utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.MinimumScoreNode), (IFormatProvider)null);

            try
            {
                nucmerObj.Align(refSeqList, searchSeqList);
                Assert.Fail();
            }
            catch (ArgumentException ex)
            {
                exThrown = true;
                Assert.IsTrue(exThrown, String.Format((IFormatProvider)null,
                    "NUCmer P2: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Validates the NUCmer align method for several test cases for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Node name to be read from xml</param>
        /// <param name="isAlignList">Is align method to take list?</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        void ValidateNUCmerAlignSimpleGeneralTestCases(string nodeName)
        {
            string[] referenceSequences = null;
            string[] searchSequences = null;
            List<ISequence> refSeqList = new List<ISequence>();
            List<ISequence> searchSeqList = new List<ISequence>();

            // Gets the reference & search sequences from the configurtion file
            referenceSequences = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.ReferenceSequencesNode);
            searchSequences = utilityObj.xmlUtil.GetTextValues(nodeName,
              Constants.SearchSequencesNode);

            IAlphabet seqAlphabet = Utility.GetAlphabet(
                utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.AlphabetNameNode));

            for (int i = 0; i < referenceSequences.Length; i++)
            {
                ISequence referSeq = new Sequence(seqAlphabet,
                   encodingObj.GetBytes(referenceSequences[i]));
                refSeqList.Add(referSeq);
            }

            for (int i = 0; i < searchSequences.Length; i++)
            {
                ISequence searchSeq = new Sequence(seqAlphabet,
                    encodingObj.GetBytes(searchSequences[i]));
                searchSeqList.Add(searchSeq);
            }

            // Gets the mum length from the xml
            string mumLength = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.MUMAlignLengthNode);

            NucmerPairwiseAligner nucmerObj = new NucmerPairwiseAligner();

            // Update other values for NUCmer object
            nucmerObj.MaximumSeparation = int.Parse
                (utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode), (IFormatProvider)null);
            nucmerObj.MinimumScore = int.Parse(
                utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode), (IFormatProvider)null);
            nucmerObj.SeparationFactor = int.Parse(
                utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode), (IFormatProvider)null);
            nucmerObj.BreakLength = int.Parse(
                utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode), (IFormatProvider)null);
            nucmerObj.LengthOfMUM = long.Parse(mumLength, null);

            IList<ISequenceAlignment> alignSimple = null;
            IList<ISequence> seqList = new List<ISequence>();

            foreach (ISequence seq in refSeqList)
            {
                seqList.Add(seq);
            }

            foreach (ISequence seq in searchSeqList)
            {
                seqList.Add(seq);
            }

            alignSimple = nucmerObj.AlignSimple(seqList);

            string expectedSequences = string.Empty;
            expectedSequences = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.ExpectedSequencesNode);

            string[] expSeqArray = expectedSequences.Split(',');

            int j = 0;

            // Gets all the aligned sequences in comma seperated format
            foreach (IPairwiseSequenceAlignment seqAlignment in alignSimple)
            {
                foreach (PairwiseAlignedSequence alignedSeq in seqAlignment)
                {
                    Assert.AreEqual(expSeqArray[j], new string(alignedSeq.FirstSequence.Select(a => (char)a).ToArray()));
                    ++j;
                    Assert.AreEqual(expSeqArray[j], new string(alignedSeq.SecondSequence.Select(a => (char)a).ToArray()));
                    j++;

                }
            }

            Console.WriteLine(
                "NUCmer P2 : Successfully validated all the aligned sequences.");
            ApplicationLog.WriteLine(
                "NUCmer P2 : Successfully validated all the aligned sequences.");
        }

        #endregion Supported Methods
    }
}
