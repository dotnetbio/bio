using System.Collections.Generic;
using Bio.Algorithms.Alignment;
using NUnit.Framework;

namespace Bio.Tests.Algorithms.Alignment.NUCmer
{
    /// <summary>
    /// Test Automation code for Bio Sequences and BVT level validations.
    /// </summary>
    [TestFixture]
    public class NUCmerAttributesBvtTestCases
    {
        /// <summary>
        /// Validate the attributes in NUCmerAttributes.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateNUCmerAttributes()
        {
            NUCmerAttributes nucAttrib = new NUCmerAttributes();
            Dictionary<string, AlignmentInfo> attributes = nucAttrib.Attributes;

            Assert.AreEqual(9, attributes.Count);

            // Validating all the NUCmer attributes
            AlignmentInfo similarityMatrixObj = attributes["SIMILARITYMATRIX"];
            AlignmentInfo gapOpenCostObj = attributes["GAPOPENCOST"];
            AlignmentInfo gapExtensionCostObj = attributes["GAPEXTENSIONCOST"];

            AlignmentInfo lenOfMumObj = attributes["LENGTHOFMUM"];
            AlignmentInfo fixedSepObj = attributes["FIXEDSEPARATION"];
            AlignmentInfo maxSepObj = attributes["MAXIMUMSEPARATION"];

            AlignmentInfo minScoreObj = attributes["MINIMUMSCORE"];
            AlignmentInfo sepFactorObj = attributes["SEPARATIONFACTOR"];
            AlignmentInfo breakLengthObj = attributes["BREAKLENGTH"];

            Assert.AreEqual("Similarity Matrix", similarityMatrixObj.Name);
            Assert.AreEqual("Gap Cost", gapOpenCostObj.Name);
            Assert.AreEqual("Gap Extension Cost", gapExtensionCostObj.Name);

            Assert.AreEqual("Length of MUM", lenOfMumObj.Name);
            Assert.AreEqual("Fixed Separation", fixedSepObj.Name);
            Assert.AreEqual("Maximum Separation", maxSepObj.Name);

            Assert.AreEqual("Minimum Score", minScoreObj.Name);
            Assert.AreEqual("Separation Factor", sepFactorObj.Name);
            Assert.AreEqual("Break Length", breakLengthObj.Name);
        }
    }
}

