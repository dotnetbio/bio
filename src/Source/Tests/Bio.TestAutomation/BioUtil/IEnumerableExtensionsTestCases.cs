/****************************************************************************
 * IEnumerableExtensionsBvtTestCases.cs
 * 
 * This file contains the IEnumerableExtensions BVT test cases.
 * 
******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bio.TestAutomation.Util;
using Bio.Util;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

#if (SILVERLIGHT == false)
namespace Bio.TestAutomation.Util
#else
   namespace Bio.SilverLight.TestAutomation
#endif
{
    /// <summary>
    /// Test Automation code for Bio IEnumerableExtensions and BVT level validations.
    /// </summary>
    [TestClass]
    public class IEnumerableExtensionsBvtTestCases
    {
        #region Private Variables

        /// <summary>
        /// Contains collection of sequences.
        /// </summary>
        private string[] sequences;

        #endregion

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static IEnumerableExtensionsBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region IEnumerableExtensions Bvt TestCases

        /// <summary>
        /// Validate Shuffle method of IEnumerableExtensions by passing valid random object and sequences.
        /// Input Data : Valid random object and sequences.
        /// Output Data : Validate list of seqeunces.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateShuffleForValidIEnumItems()
        {
            sequences = utilityObj.xmlUtil.GetTextValue(
                Constants.IEnumerableExtensionsNode,
                Constants.Sequence1).Split(';');
            IEnumerable<string> enumerable = new string[] { sequences[0], sequences[1], sequences[2] };
            List<string> lists = enumerable.Shuffle(new Random());
            Assert.IsNotNull(lists);
            Assert.AreEqual(sequences.Length, lists.Count);

            foreach (string list in lists)
            {
                Assert.IsTrue(enumerable.Contains<string>(list));
            }

            ApplicationLog.WriteLine(string.Concat(
                  "IEnumerableExtensions BVT: Validation of Shuffle method for Random object and sequences completed successfully."));
        }

        /// <summary>
        /// Validate StringJoin method of IEnumerableExtensions by passing valid collection.
        /// Input Data : Valid collection.
        /// Output Data : Validate seqeunce.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateStringJoinForCollection()
        {
            string expectedSequence = utilityObj.xmlUtil.GetTextValue(
                Constants.IEnumerableExtensionsNode,
                Constants.ExpectedOutputNode);
            sequences = utilityObj.xmlUtil.GetTextValue(
                Constants.IEnumerableExtensionsNode,
                Constants.Sequence2).Split(';');

            IEnumerable<string> enumerable = new string[] { sequences[0], sequences[1], sequences[2], null };
            string join = enumerable.StringJoin();
            Assert.IsNotNull(join);
            Assert.AreEqual(expectedSequence, join);

            ApplicationLog.WriteLine(string.Concat(
                  "IEnumerableExtensions BVT: Validation of StringJoin method for sequences completed successfully."));
        }

        /// <summary>
        /// Validate StringJoin method of IEnumerableExtensions by passing valid collection with separator.
        /// Input Data : Valid collection with separator.
        /// Output Data : Validate seqeunce.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateStringJoinForSeparator()
        {
            string expectedSequence = utilityObj.xmlUtil.GetTextValue(
                Constants.IEnumerableExtensionsNode,
                Constants.ExpectedNormalString);
            sequences = utilityObj.xmlUtil.GetTextValue(
                Constants.IEnumerableExtensionsNode,
                Constants.Sequence2).Split(';');

            IEnumerable<string> enumerable = new string[] { sequences[0], sequences[1], sequences[2], null };
            string join = enumerable.StringJoin(":");
            Assert.IsNotNull(join);
            Assert.AreEqual(expectedSequence, join);

            ApplicationLog.WriteLine(string.Concat(
                  "IEnumerableExtensions BVT: Validation of StringJoin method for sequences and seperator completed successfully."));
        }

        /// <summary>
        /// Validate StringJoin method of IEnumerableExtensions by passing valid collection with separator and etcString.
        /// Input Data : Valid collection with separator and etcString.
        /// Output Data : Validate sequence.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateStringJoinForEtcString()
        {
            string expectedSequence = utilityObj.xmlUtil.GetTextValue(
                Constants.IEnumerableExtensionsNode, Constants.ExpectedSequence);
            string etcString = utilityObj.xmlUtil.GetTextValue(
                Constants.IEnumerableExtensionsNode, Constants.EtcStringNode);
            int maxLength = Convert.ToInt32(utilityObj.xmlUtil.GetTextValue(
                Constants.IEnumerableExtensionsNode, Constants.MaxAttemptString), CultureInfo.InvariantCulture);
            sequences = utilityObj.xmlUtil.GetTextValue(Constants.IEnumerableExtensionsNode, Constants.Sequence2).Split(';');

            IEnumerable<string> enumerable = new[] { sequences[0], sequences[1], sequences[2], null };
            string join = enumerable.StringJoin(":", maxLength, etcString);
            Assert.IsNotNull(join);
            Assert.AreEqual(expectedSequence, join);

            ApplicationLog.WriteLine("IEnumerableExtensions BVT: Validation of StringJoin method for sequences, separator, length and etc-string completed successfully.");
        }

        /// <summary>
        /// Validate ToHashSet method of IEnumerableExtensions by passing valid collection.
        /// Input Data : Valid collection.
        /// Output Data : Validate HashSet sequence.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateToHashSetForCollection()
        {
            sequences = utilityObj.xmlUtil.GetTextValue(
                Constants.IEnumerableExtensionsNode,
                Constants.Sequence2).Split(';');

            IEnumerable<string> enumerable = new string[] { sequences[0], sequences[1], sequences[2] };
            HashSet<string> joins = enumerable.ToHashSet();
            Assert.IsNotNull(joins);
            Assert.IsTrue(joins.Contains(sequences[0]));
            Assert.IsTrue(joins.Contains(sequences[1]));
            Assert.IsTrue(joins.Contains(sequences[2]));

            ApplicationLog.WriteLine(string.Concat(
                  "IEnumerableExtensions BVT: Validation of ToHashSet method for sequences completed successfully."));
        }

        /// <summary>
        /// Validate ToQueue method of IEnumerableExtensions by passing valid collection.
        /// Input Data : Valid collection.
        /// Output Data : Validate HashSet sequence.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateToQueueForCollection()
        {
            sequences = utilityObj.xmlUtil.GetTextValue(
                Constants.IEnumerableExtensionsNode,
                Constants.Sequence2).Split(';');

            IEnumerable<string> enumerable = new string[] { sequences[0], sequences[1], sequences[2] };
            Queue<string> joins = enumerable.ToQueue();
            Assert.IsNotNull(joins);
            Assert.IsTrue(joins.Contains(sequences[0]));
            Assert.IsTrue(joins.Contains(sequences[1]));
            Assert.IsTrue(joins.Contains(sequences[2]));

            ApplicationLog.WriteLine(string.Concat(
                  "IEnumerableExtensions BVT: Validation of ToQueue method for sequences completed successfully."));
        }

        #endregion IEnumerableExtensions Bvt TestCases
    }
}
