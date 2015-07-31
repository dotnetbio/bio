using System;
using System.Collections.Generic;

using Bio.Algorithms.StringSearch;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests.Algorithms.StringSearch
{
    /// <summary>
    ///     Boyer Moore BVT Test case implementation.
    /// </summary>
    [TestFixture]
    public class BoyerMooreBvtTestCases
    {
        private readonly Utility utilityObj = new Utility(@"TestUtils\BoyerMooreTestConfig.xml");

        #region BoyerMoore Align Test Cases

        /// <summary>
        ///     Validate FindMatch(sequence, searchpattern) method with 1000 BP Dna sequence
        ///     and validate the matches
        ///     Input : Dna sequence with 1000 BP.
        ///     Validation : Validate the matches.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void BoyerMooreAlignDnaSequenceWith1000BP()
        {
            var boyerMoore = new BoyerMoore();
            string boyerMooreSequence =
                this.utilityObj.xmlUtil.GetTextValue(Constants.BoyerMooreSequence, Constants.SequenceNode);
            string referenceSequence =
                this.utilityObj.xmlUtil.GetTextValue(Constants.BoyerMooreSequence, Constants.SearchSequenceNode);
            string expectedMatch =
                this.utilityObj.xmlUtil.GetTextValue(Constants.BoyerMooreSequence, Constants.ExpectedMatch);
            ISequence boyerMooreSeq = new Sequence(Alphabets.DNA, boyerMooreSequence);

            IList<int> indexList = boyerMoore.FindMatch(boyerMooreSeq,
                                                        referenceSequence);

            Assert.AreEqual(expectedMatch, indexList[0].ToString((IFormatProvider) null));
        }

        /// <summary>
        ///     Validate FindMatch(sequence, searchpatterns) method with 1000 BP Dna sequence
        ///     and validate the matches
        ///     Input : Dna sequence with 1000 BP.
        ///     Validation : Validate the matches.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void BoyerMooreAlignDnaSequenceListWith1000BP()
        {
            var boyerMoore = new BoyerMoore();
            string boyerMooreSequence =
                this.utilityObj.xmlUtil.GetTextValue(Constants.BoyerMooreSequence, Constants.SequenceNode);
            string referenceSequence =
                this.utilityObj.xmlUtil.GetTextValue(Constants.BoyerMooreSequence, Constants.SearchSequenceNode);
            string expectedMatch =
                this.utilityObj.xmlUtil.GetTextValue(Constants.BoyerMooreSequence, Constants.ExpectedMatch);
            ISequence boyerMooreSeq = new Sequence(Alphabets.DNA, boyerMooreSequence);

            IList<string> referenceSeqs = new List<string> {referenceSequence};

            IDictionary<string, IList<int>> indexList = boyerMoore.FindMatch(boyerMooreSeq, referenceSeqs);

            Assert.AreEqual(expectedMatch, indexList[referenceSequence][0].ToString((IFormatProvider) null));
        }

        #endregion BoyerMoore Align Test Cases
    }
}