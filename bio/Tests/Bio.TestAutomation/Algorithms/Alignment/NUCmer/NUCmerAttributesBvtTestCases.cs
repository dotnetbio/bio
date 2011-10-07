/****************************************************************************
 * NUCmerAttributesBvtTestCases.cs
 * 
 * This file contains the NUCmerAttributes BVT test cases.
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
    /// Test Automation code for Bio Sequences and BVT level validations.
    /// </summary>
    [TestClass]
    public class NUCmerAttributesBvtTestCases
    {

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static NUCmerAttributesBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region NUCmerAttributes Bvt TestCases

        /// <summary>
        /// Validate the attributes in NUCmerAttributes.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
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

            ApplicationLog.WriteLine(
                "NUCmerAttributes BVT: Successfully validated all the attributes.");
        }

        #endregion NUCmerAttributes Bvt TestCases
    }
}

