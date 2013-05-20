/****************************************************************************
 * PairwiseAlignmentAttributesBvtTestCases.cs
 * 
 * This file contains the PairwiseAlignmentAttributes BVT test cases.
 * 
******************************************************************************/

using System.Collections.Generic;
using Bio.Algorithms.Alignment;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM = Bio.SimilarityMatrices.SimilarityMatrix;

namespace Bio.TestAutomation.Algorithms.Alignment
{
    /// <summary>
    ///     Test Automation code for Bio Sequences and BVT level validations.
    /// </summary>
    [TestClass]
    public class PairwiseAlignmentAttributesBvtTestCases
    {
        #region Constructor

        /// <summary>
        ///     Static constructor to open log and make other settings needed for test
        /// </summary>
        static PairwiseAlignmentAttributesBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region PairwiseAlignmentAttributes Bvt TestCases

        /// <summary>
        ///     Validate the attributes in PairwiseAlignmentAttributes.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidatePairwiseAlignmentAttributes()
        {
            var pwAlignAttrib = new PairwiseAlignmentAttributes();
            Dictionary<string, AlignmentInfo> attributes = pwAlignAttrib.Attributes;

            AlignmentInfo similarityMatrixObj = attributes["SIMILARITYMATRIX"];
            AlignmentInfo gapOpenCostObj = attributes["GAPOPENCOST"];
            AlignmentInfo gapExtensionCostObj = attributes["GAPEXTENSIONCOST"];

            Assert.AreEqual("Similarity Matrix", similarityMatrixObj.Name);

            var validator = new StringListValidator(
                "Diagonal (Match x Mismatch)",
                SM.StandardSimilarityMatrix.AmbiguousDna.ToString(),
                SM.StandardSimilarityMatrix.AmbiguousRna.ToString(),
                SM.StandardSimilarityMatrix.Blosum45.ToString(),
                SM.StandardSimilarityMatrix.Blosum50.ToString(),
                SM.StandardSimilarityMatrix.Blosum62.ToString(),
                SM.StandardSimilarityMatrix.Blosum80.ToString(),
                SM.StandardSimilarityMatrix.Blosum90.ToString(),
                SM.StandardSimilarityMatrix.DiagonalScoreMatrix.ToString(),
                SM.StandardSimilarityMatrix.Pam250.ToString(),
                SM.StandardSimilarityMatrix.Pam30.ToString());

            Assert.IsTrue(validator.IsValid(SM.StandardSimilarityMatrix.AmbiguousDna.ToString()));
            validator.AddValidValues(SM.StandardSimilarityMatrix.Pam70.ToString());
            Assert.AreEqual("Diagonal (Match x Mismatch)", validator.ValidValues[0]);
            Assert.AreEqual("Gap Cost", gapOpenCostObj.Name);
            Assert.AreEqual("Gap Extension Cost", gapExtensionCostObj.Name);

            ApplicationLog.WriteLine(
                "PairwiseAlignmentAttributes BVT: Successfully validated all the attributes.");
        }

        #endregion PairwiseAlignmentAttributes Bvt TestCases
    }
}