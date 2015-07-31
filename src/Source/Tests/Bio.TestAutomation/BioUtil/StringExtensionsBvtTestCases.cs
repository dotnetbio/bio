/****************************************************************************
 * StringExtensionsBvtTestCases.cs
 * 
 * This file contains the StringExtensions BVT test cases.
 * 
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using Bio.TestAutomation.Util;
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
    /// Test Automation code for Bio StringExtensions and BVT level validations.
    /// </summary>
    [TestClass]
    public class StringExtensionsBvtTestCases
    {
        #region Private Variables

        /// <summary>
        /// Contains collection of sequences.
        /// </summary>
        private string sequences;

        #endregion

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static StringExtensionsBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region StringExtensions Bvt TestCases

        /// <summary>
        /// Validate ProtectedSplit method of StringExtensions by passing valid text and characters.
        /// Input Data : Valid Text, OpenParenCharacter, CloseParenCharacter, RemoveEmptyItems and 
        /// SplitCharacters
        /// Output Data : Validate of String.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateProtectedSplitForValidText()
        {
            sequences = utilityObj.xmlUtil.GetTextValue(
                Constants.StringExtensionsNode,
                Constants.Sequence1);
            IEnumerable<string> splitedTexts = sequences.ProtectedSplit(
                'S',
                'E',
                true,
                new char[] { '.', ',' });
            Assert.IsNotNull(splitedTexts);
            foreach (string splitedText in splitedTexts)
            {
                continue;
            }

            PrivateObject privateObject = new PrivateObject(splitedTexts);
            Assert.IsTrue((bool)privateObject.GetField("removeEmptyItems"));
            Assert.AreEqual(sequences, privateObject.GetField("text") as string);
            ApplicationLog.WriteLine(string.Concat(
                  "StringExtension BVT: Validation of ProtectedSplit method for Text completed successfully."));
        }

        /// <summary>
        /// Validate Reverse method of StringExtensions by passing valid text.
        /// Input Data : Valid Text.
        /// Output Data : Validate reverse order of a string.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateTryParseAllForValidText()
        {
            sequences = utilityObj.xmlUtil.GetTextValue(
                Constants.StringExtensionsNode,
                Constants.Sequence1);
            string reverseString = StringExtensions.Reverse(sequences);
            Assert.IsNotNull(reverseString);
            for (int count = 0; count < sequences.Length; count++)
            {
                Assert.AreEqual(sequences[(sequences.Length - 1) - count], reverseString[count]);
            }

            ApplicationLog.WriteLine(string.Concat(
                  "StringExtensions BVT: Validation of Reverse method for Text completed successfully."));
        }

        /// <summary>
        /// Validate ToMixedInvariant method of StringExtensions by passing valid text.
        /// Input Data : Valid Text.
        /// Output Data : Validate Mixed Text.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateToMixedInvariantForValidText()
        {
            sequences = utilityObj.xmlUtil.GetTextValue(
                Constants.StringExtensionsNode,
                Constants.Sequence1);
            string expectedSequence = utilityObj.xmlUtil.GetTextValue(
                Constants.StringExtensionsNode,
                Constants.ExpectedOutputNode);
            string mixedString = StringExtensions.ToMixedInvariant(sequences);
            Assert.IsNotNull(mixedString);
            Assert.AreEqual(sequences.Length, mixedString.Length);
            Assert.AreEqual(expectedSequence, mixedString);

            ApplicationLog.WriteLine(string.Concat(
                  "StringExtensions BVT: Validation of Reverse method for Sequences completed successfully."));
        }

        #endregion StringExtensions Bvt TestCases
    }
}
