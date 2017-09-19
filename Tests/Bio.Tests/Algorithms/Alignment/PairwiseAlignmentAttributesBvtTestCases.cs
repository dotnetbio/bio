using System.Collections.Generic;

using Bio.Algorithms.Alignment;
using Bio.SimilarityMatrices;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests.Algorithms.Alignment
{
    /// <summary>
    ///     Test Automation code for Bio Sequences and BVT level validations.
    /// </summary>
    [TestFixture]
    public class PairwiseAlignmentAttributesBvtTestCases
    {
        #region PairwiseAlignmentAttributes Bvt TestCases

        /// <summary>
        ///     Validate the attributes in PairwiseAlignmentAttributes.
        /// </summary>
        [Test]
        [Category("Priority0")]
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
                SimilarityMatrix.StandardSimilarityMatrix.AmbiguousDna.ToString(),
                SimilarityMatrix.StandardSimilarityMatrix.AmbiguousRna.ToString(),
                SimilarityMatrix.StandardSimilarityMatrix.Blosum45.ToString(),
                SimilarityMatrix.StandardSimilarityMatrix.Blosum50.ToString(),
                SimilarityMatrix.StandardSimilarityMatrix.Blosum62.ToString(),
                SimilarityMatrix.StandardSimilarityMatrix.Blosum80.ToString(),
                SimilarityMatrix.StandardSimilarityMatrix.Blosum90.ToString(),
                SimilarityMatrix.StandardSimilarityMatrix.DiagonalScoreMatrix.ToString(),
                SimilarityMatrix.StandardSimilarityMatrix.Pam250.ToString(),
                SimilarityMatrix.StandardSimilarityMatrix.Pam30.ToString());

            Assert.IsTrue(validator.IsValid(SimilarityMatrix.StandardSimilarityMatrix.AmbiguousDna.ToString()));
            validator.AddValidValues(SimilarityMatrix.StandardSimilarityMatrix.Pam70.ToString());
            Assert.AreEqual("Diagonal (Match x Mismatch)", validator.ValidValues[0]);
            Assert.AreEqual("Gap Cost", gapOpenCostObj.Name);
            Assert.AreEqual("Gap Extension Cost", gapExtensionCostObj.Name);

            ApplicationLog.WriteLine(
                "PairwiseAlignmentAttributes BVT: Successfully validated all the attributes.");
        }

        #endregion PairwiseAlignmentAttributes Bvt TestCases
    }
}