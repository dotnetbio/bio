using System.Collections.Generic;
using Bio.Algorithms.StringSearch;
using NUnit.Framework;
using Bio;

namespace Bio.Tests.Algorithms.StringSearch
{
    /// <summary>
    /// BoyerMoore Test cases
    /// </summary>
    [TestFixture]
    public class BoyerMooreTests
    {
        /// <summary>
        /// Find pattern test.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SimpleFindOneOutputPatternTest()
        {
            ISequence sequence = new Sequence(DnaAlphabet.Instance, "AGCT");
            IPatternFinder patternFinder = new BoyerMoore();
            IList<int> actual = patternFinder.FindMatch(sequence, "AGCT");

            HashSet<int> expected = new HashSet<int>();
            expected.Add(0);

            Assert.IsTrue(Compare(expected, actual));
        }

        /// <summary>
        /// Find pattern test.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SimpleFindMultipleOutputPatternTest()
        {
            ISequence sequence = new Sequence(DnaAlphabet.Instance, "AGTAGT");
            IPatternFinder patternFinder = new BoyerMoore();
            IList<int> actual = patternFinder.FindMatch(sequence, "AGT");

            HashSet<int> expected = new HashSet<int>();
            expected.Add(0);
            expected.Add(3);

            Assert.IsTrue(Compare(expected, actual));
        }

        /// <summary>
        /// Find pattern test.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void MultipleFindOneOutputPatternTest()
        {
            ISequence sequence = new Sequence(DnaAlphabet.Instance, "AGCTAGGTTGGCC");
            IList<string> patterns = new List<string>();
            patterns.Add("AGCT");
            patterns.Add("AAAAA");
            IPatternFinder patternFinder = new BoyerMoore();
            IDictionary<string, IList<int>> actual = patternFinder.FindMatch(sequence, patterns);

            IDictionary<string, HashSet<int>> expected = new Dictionary<string, HashSet<int>>();
            HashSet<int> indices = new HashSet<int>();
            indices.Add(0);
            expected.Add("AGCT", indices);

            indices = new HashSet<int>();
            expected.Add("AAAAA", indices);

            Assert.IsTrue(Compare(expected, actual));
        }

        /// <summary>
        /// Find pattern test.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void MultipleFindMultipleOutputPatternTest()
        {
            ISequence sequence = new Sequence(DnaAlphabet.Instance, "AGCTAGGTAGCTCAAAAA");
            IList<string> patterns = new List<string>();
            patterns.Add("AGCT");
            patterns.Add("AAAAA");
            IPatternFinder patternFinder = new BoyerMoore();
            IDictionary<string, IList<int>> actual = patternFinder.FindMatch(sequence, patterns);

            IDictionary<string, HashSet<int>> expected = new Dictionary<string, HashSet<int>>();
            HashSet<int> indices = new HashSet<int>();
            indices.Add(0);
            indices.Add(8);
            expected.Add("AGCT", indices);

            indices = new HashSet<int>();
            indices.Add(13);
            expected.Add("AAAAA", indices);

            Assert.IsTrue(Compare(expected, actual));
        }

        /// <summary>
        /// Compares the expected and actual values and return true if they match
        /// otherwise return false.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void MatchWildcardPatternEnd()
        {
            ISequence sequence = new Sequence(DnaAlphabet.Instance, "AGCTAGGTAGCTCAAAAA");
            IPatternFinder patternFinder = new BoyerMoore();
            IList<int> actual = patternFinder.FindMatch(sequence, "AGCTAGGTAGCTCA*");

            HashSet<int> expected = new HashSet<int>();
            expected.Add(0);

            Assert.IsTrue(Compare(expected, actual));
        }

        /// <summary>
        /// Compares the expected and actual values and return true if they match
        /// otherwise return false.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void MatchWildcardPatternMiddle()
        {
            ISequence sequence = new Sequence(DnaAlphabet.Instance, "AGCTAGGTAGCTCAAAAAAGGG");
            IPatternFinder patternFinder = new BoyerMoore();
            IList<int> actual = patternFinder.FindMatch(sequence, "AGCTAGGTAGCTCA*GGG");

            HashSet<int> expected = new HashSet<int>();
            expected.Add(0);

            Assert.IsTrue(Compare(expected, actual));
        }

        private static bool Compare(IDictionary<string, HashSet<int>> expected, IDictionary<string, IList<int>> actual)
        {
            if (expected.Count != actual.Count)
            {
                return false;
            }

            HashSet<int> indices = null;
            foreach (KeyValuePair<string, IList<int>> result in actual)
            {
                if (expected.TryGetValue(result.Key, out indices))
                {
                    if (!Compare(indices, result.Value))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compares the expected and actual values and return true if they match
        /// otherwise return false.
        /// </summary>
        /// <param name="expected">Expected values</param>
        /// <param name="actual">Actual values</param>
        /// <returns>Is match</returns>
        private static bool Compare(HashSet<int> expected, IList<int> actual)
        {
            if (expected.Count != actual.Count)
            {
                return false;
            }

            foreach (int result in actual)
            {
                if (!expected.Contains(result))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
