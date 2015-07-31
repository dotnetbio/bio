/****************************************************************************
 * ComparativeP2TestCases.cs
 * 
 *  This file contains the Comparative P2 test cases.
 * 
***************************************************************************/

using Bio.Algorithms.Assembly.Comparative;
using Bio.Algorithms.Alignment;
using Bio.Util.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.TestAutomation.Util;
using Bio.Util;

namespace Bio.TestAutomation.Algorithms.Assembly.Comparative
{
    /// <summary>
    /// Comparative P2 test cases
    /// </summary>
    [TestClass]
    public class ComparativeP2TestCases
    {
        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\ComparativeTestData\ComparativeTestsConfig.xml");

        #endregion Global Variables

        # region enum

        public enum ExceptionTypes
        {
            RefineLayoutNullDeltas,
            ResolveAmbiguityNullDeltas,
            ConsensusWithNullDelta,
        };


        # endregion enum

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static ComparativeP2TestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("Bio.automation.log");
            }
        }

        #endregion Constructor

        # region Comparative P2 test cases

        /// <summary>
        /// Validate Generate Consensus() with null value as Delta Alignment.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void ValidateGenerateConsensusMethodWithNullDeltas()
        {
            try
            {
                ConsensusGeneration.GenerateConsensus(null);
                ApplicationLog.WriteLine("GenerateConsensus P2 : Exception not thrown");
            }
            catch (ArgumentNullException)
            {
                ApplicationLog.WriteLine("GenerateConsensus P2 : Successfully validated the exception");
            }
        }


        /// <summary>
        /// Validate RefineLayout() with null as Delta Alignment.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void ValidateRefineLayoutMethodWithNullDeltas()
        {
            ValidateRefineLayoutMethod(Constants.RefineLayout, ExceptionTypes.RefineLayoutNullDeltas);
        }

        /// <summary>
        /// Validate ResolveAmbiguity() with null value as Delta Alignment.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void ValidateResolveAmbiguityMethodWithNullDeltas()
        {
            string actualException = null;

            try
            {
                foreach (var delta in RepeatResolver.ResolveAmbiguity(null))
                {
                    ApplicationLog.WriteLine("{0}", delta.Errors);
                }

                ApplicationLog.WriteLine("Resolve Ambiguity P2 : Exception not thrown");
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                actualException = ex.Message;
                ApplicationLog.WriteLine("Resolve Ambiguity P2 : Successfully validated the exception");
            }

            string expectedMessage = GetExpectedErrorMessageWithInvalidSequenceType(Constants.RepeatResolution, ExceptionTypes.ResolveAmbiguityNullDeltas);
            Assert.AreEqual(expectedMessage, actualException.Replace("\n", "").Replace("\r", ""));
        }

        #endregion Comparative P2 test cases

        #region Supporting methods

        /// <summary>
        /// Validate exceptions from RefineLayout() with null reads/null delta's.
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="exceptionType">Layout refinement different parameters</param>
        public void ValidateRefineLayoutMethod(string nodeName, ExceptionTypes exceptionType)
        {
            string actualException = null;

            try
            {
                DeltaAlignmentCollection coll = null;
                IEnumerable<DeltaAlignment> alignments = LayoutRefiner.RefineLayout(coll);
                if (alignments.Count() >= 0)
                {
                    ApplicationLog.WriteLine("RefineLayout P2 : Exception not thrown.");
                    Assert.Fail();
                }
            }
            catch (ArgumentNullException ex)
            {
                actualException = ex.Message;
                ApplicationLog.WriteLine("Refine Layout P2 : Successfully validated the exception");
            }

            string expectedMessage = GetExpectedErrorMessageWithInvalidSequenceType(nodeName, exceptionType);
            Assert.AreEqual(expectedMessage, actualException.Replace("\n", "").Replace("\r", ""));
        }


        /// <summary>
        /// Gets the expected error message for invalid sequence type.
        /// </summary>
        /// <param name="nodeName">xml node</param>
        /// <param name="sequenceType"></param>
        /// <returns>Returns expected error message</returns>
        string GetExpectedErrorMessageWithInvalidSequenceType(string nodeName,
            ExceptionTypes sequenceType)
        {
            string expectedErrorMessage = string.Empty;
            switch (sequenceType)
            {
                case ExceptionTypes.RefineLayoutNullDeltas:
                    expectedErrorMessage = utilityObj.xmlUtil.GetTextValue(nodeName,
                        Constants.RefineLayoutWithNullDeltasNode);
                    break;
                case ExceptionTypes.ResolveAmbiguityNullDeltas:
                    expectedErrorMessage = utilityObj.xmlUtil.GetTextValue(nodeName,
                        Constants.ResolveAmbiguityWithNullDeltasNode);
                    break;
                case ExceptionTypes.ConsensusWithNullDelta:
                    expectedErrorMessage = utilityObj.xmlUtil.GetTextValue(nodeName,
                        Constants.ConsensusWithNullDeltaNode);
                    break;
                default:
                    expectedErrorMessage = utilityObj.xmlUtil.GetTextValue(nodeName,
                        Constants.ExpectedErrorMessage);
                    break;
            }

            return expectedErrorMessage;
        }

        #endregion Supporting methods
    }
}
