/****************************************************************************
 * ParserBvtTestCases.cs
 * 
 * This file contains the Parser BVT test cases.
 * 
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Bio.Util;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if (SILVERLIGHT == false)
namespace Bio.TestAutomation.Util
#else
   namespace Bio.SilverLight.TestAutomation
#endif
{
    /// <summary>
    /// Test Automation code for Bio Parser and BVT level validations.
    /// </summary>
    [TestClass]
    public class ParserP2TestCases
    {
        #region Enum

        /// <summary>
        /// Sequence type for validating different test cases.
        /// </summary>
        private enum SequenceType
        {
            DNA,
            RNA,
            Proteins
        }

        #endregion

        #region Private Variables

        /// <summary>
        /// Contains collection of sequences.
        /// </summary>
        private string[] sequences;

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static ParserP2TestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region Parser P2 TestCases

        /// <summary>
        /// Validate TryParseAll method of Parser by passing null sequences.
        /// Input Data : Null Sequences.
        /// Output Data : Validate Exception.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void ValidateTryParseAllForNullSequences()
        {
            sequences = new string[] { null };
            IList<string> outseq = new List<string>();

            try
            {
                Parser.TryParseAll(sequences.ToList(), out outseq);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Parser P2: Validation of ParseAll method for NULL Sequences completed successfully : {0}.",
                ex.Message));
            }
        }

        /// <summary>
        /// Validate ParseAll method of Parser by passing null sequences.
        /// Input Data : Null Sequences.
        /// Output Data : Validate Exception.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void ValidateParseAllForNullSequences()
        {
            sequences = new string[] { null };

            try
            {
                Parser.ParseAll<SequenceType>(sequences.ToList());
                Assert.Fail();
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Parser P2: Validation of ParseAll method for NULL Sequences completed successfully : {0}.",
                ex.Message));
            }
        }

        /// <summary>
        /// Validate TryParse method of Parser by passing null sequences.
        /// Input Data : Null Sequences.
        /// Output Data : Validate Exception.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void ValidateTryParseForNullSequences()
        {
            sequences = new string[] { null };

            try
            {
                int expectedResult = 0;
                Parser.TryParse(sequences[0], out expectedResult);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Parser P2: Validation of TryParse method for NULL Sequences completed successfully: {0}.",
                ex.Message));
            }
        }

        /// <summary>
        /// Validate Parse method of Parser by passing null sequences.
        /// Input Data : Null Sequences.
        /// Output Data : Validate Exception.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void ValidateParseForSequenceValues()
        {
            sequences = new string[] { null };
            const int expectedResult = 0;

            try
            {
                Parser.Parse(sequences[0], expectedResult.GetType());
                Assert.Fail();
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Parser P2: Validation of Parse method for NULL Sequences completed successfully: {0}.",
                ex.Message));
            }
        }

        #endregion Parser P2 TestCases
    }

}
