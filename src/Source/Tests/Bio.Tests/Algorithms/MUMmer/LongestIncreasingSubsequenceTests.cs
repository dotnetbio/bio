using System.Collections.Generic;
using Bio.Algorithms.MUMmer.LIS;
using Bio.Algorithms.SuffixTree;
using NUnit.Framework;

namespace Bio.Tests.MUMmer.LIS
{
    /// <summary>
    /// Tests for the LongestIncreasingSubsequence class.
    /// </summary>
    [TestFixture]
    public class LongestIncreasingSubsequenceTests
    {
        /// <summary>
        /// Test LongestIncreasingSubsequence with MUM set which has neither
        /// crosses nor overlaps
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestLISWithoutCrossAndOverlap()
        {
            // Create a list of Mum classes.
            List<Match> MUM = new List<Match>();
            Match mum;

            mum = new Match();
            mum.ReferenceSequenceOffset = 0;
            mum.Length = 3;
            mum.QuerySequenceOffset = 0;
            MUM.Add(mum);

            mum = new Match();
            mum.ReferenceSequenceOffset = 4;
            mum.Length = 3;
            mum.QuerySequenceOffset = 3;
            MUM.Add(mum);

           // ILongestIncreasingSubsequence lis = new LongestIncreasingSubsequence();
            LongestIncreasingSubsequence lis = new LongestIncreasingSubsequence();
            IList<Match> lisList = lis.SortMum(MUM);
            IList<Match> lisList1 = lis.GetLongestSequence(lisList);

            List<Match> expectedOutput = new List<Match>();
            mum = new Match();
            mum.ReferenceSequenceOffset = 0;
            mum.Length = 3;
            mum.QuerySequenceOffset = 0;
            expectedOutput.Add(mum);

            mum = new Match();
            mum.ReferenceSequenceOffset = 4;
            mum.Length = 3;
            mum.QuerySequenceOffset = 3;
            expectedOutput.Add(mum);

            Assert.IsTrue(CompareMumList(lisList1, expectedOutput));
        }

        /// <summary>
        /// Test LongestIncreasingSubsequence with MUM set which has crosses.
        /// First MUM is bigger
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestLISWithCross1()
        {
            // Create a list of Mum classes.
            List<Match> MUM = new List<Match>();
            Match mum;

            mum = new Match();
            mum.ReferenceSequenceOffset = 0;
            mum.Length = 4;
            mum.QuerySequenceOffset = 4;
            MUM.Add(mum);

            mum = new Match();
            mum.ReferenceSequenceOffset = 4;
            mum.Length = 3;
            mum.QuerySequenceOffset = 0;
            MUM.Add(mum);

            mum = new Match();
            mum.ReferenceSequenceOffset = 10;
            mum.Length = 3;
            mum.QuerySequenceOffset = 10;
            MUM.Add(mum);

            //ILongestIncreasingSubsequence lis = new LongestIncreasingSubsequence();
            LongestIncreasingSubsequence lis = new LongestIncreasingSubsequence();
            IList<Match> lisList = lis.SortMum(MUM);
            IList<Match> lisList1 = lis.GetLongestSequence(lisList);

            List<Match> expectedOutput = new List<Match>();
            mum = new Match();
            mum.ReferenceSequenceOffset = 0;
            mum.Length = 4;
            mum.QuerySequenceOffset = 4;
            expectedOutput.Add(mum);

            mum = new Match();
            mum.ReferenceSequenceOffset = 10;
            mum.Length = 3;
            mum.QuerySequenceOffset = 10;
            expectedOutput.Add(mum);

            Assert.IsTrue(CompareMumList(lisList1, expectedOutput));
        }

        /// <summary>
        /// Test LongestIncreasingSubsequence with MUM set which has crosses.
        /// Second MUM is bigger
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestLISWithCross2()
        {
            // Create a list of Mum classes.
            List<Match> MUM = new List<Match>();
            Match mum;

            mum = new Match();
            mum.ReferenceSequenceOffset = 0;
            mum.Length = 3;
            mum.QuerySequenceOffset = 4;
            MUM.Add(mum);

            mum = new Match();
            mum.ReferenceSequenceOffset = 4;
            mum.Length = 4;
            mum.QuerySequenceOffset = 0;
            MUM.Add(mum);

            //ILongestIncreasingSubsequence lis = new LongestIncreasingSubsequence();
            LongestIncreasingSubsequence lis = new LongestIncreasingSubsequence();
            IList<Match> lisList = lis.SortMum(MUM);
            IList<Match> lisList1 = lis.GetLongestSequence(lisList);

            List<Match> expectedOutput = new List<Match>();
            mum = new Match();
            mum.ReferenceSequenceOffset = 4;
            mum.Length = 4;
            mum.QuerySequenceOffset = 0;
            expectedOutput.Add(mum);

            Assert.IsTrue(CompareMumList(lisList1, expectedOutput));
        }

        /// <summary>
        /// Test LongestIncreasingSubsequence with MUM set which has overlap
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestLISWithCrossAndOverlap()
        {
            // Create a list of Mum classes.
            List<Match> MUM = new List<Match>();
            Match mum;

            mum = new Match();
            mum.ReferenceSequenceOffset = 0;
            mum.Length = 5;
            mum.QuerySequenceOffset = 5;
            MUM.Add(mum);

            mum = new Match();
            mum.ReferenceSequenceOffset = 3;
            mum.Length = 5;
            mum.QuerySequenceOffset = 0;
            MUM.Add(mum);

           // ILongestIncreasingSubsequence lis = new LongestIncreasingSubsequence();
            LongestIncreasingSubsequence lis = new LongestIncreasingSubsequence();
            IList<Match> lisList = lis.SortMum(MUM);
            IList<Match> lisList1 = lis.GetLongestSequence(lisList);

            List<Match> expectedOutput = new List<Match>();
            mum = new Match();
            mum.ReferenceSequenceOffset = 0;
            mum.Length = 5;
            mum.QuerySequenceOffset = 5;
            expectedOutput.Add(mum);

            Assert.IsTrue(CompareMumList(lisList1, expectedOutput));
        }

        /// <summary>
        /// Test LongestIncreasingSubsequence with MUM set which has overlap
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestLISWithOverlap()
        {
            // Create a list of Mum classes.
            List<Match> MUM = new List<Match>();
            Match mum;

            mum = new Match();
            mum.ReferenceSequenceOffset = 0;
            mum.Length = 4;
            mum.QuerySequenceOffset = 0;
            MUM.Add(mum);

            mum = new Match();
            mum.ReferenceSequenceOffset = 2;
            mum.Length = 5;
            mum.QuerySequenceOffset = 4;
            MUM.Add(mum);

            //ILongestIncreasingSubsequence lis = new LongestIncreasingSubsequence();
            LongestIncreasingSubsequence lis = new LongestIncreasingSubsequence();
            IList<Match> lisList = lis.SortMum(MUM);
            IList<Match> lisList1 = lis.GetLongestSequence(lisList);

            List<Match> expectedOutput = new List<Match>();
            mum = new Match();
            mum.ReferenceSequenceOffset = 0;
            mum.Length = 4;
            mum.QuerySequenceOffset = 0;
            expectedOutput.Add(mum);

            mum = new Match();
            mum.ReferenceSequenceOffset = 4;
            mum.Length = 3;
            mum.QuerySequenceOffset = 6;
            expectedOutput.Add(mum);

            Assert.IsTrue(CompareMumList(lisList1, expectedOutput));
        }

        /// <summary>
        /// Tests the sorted mums.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestSortMum()
        {
            // Create a list of Mum classes.
            List<Match> MUM = new List<Match>();
            Match mum;

            mum = new Match();
            mum.ReferenceSequenceOffset = 4;
            mum.Length = 3;
            mum.QuerySequenceOffset = 0;
            MUM.Add(mum);

            mum = new Match();
            mum.ReferenceSequenceOffset = 0;
            mum.Length = 3;
            mum.QuerySequenceOffset = 3;
            MUM.Add(mum);

            LongestIncreasingSubsequence lis = new LongestIncreasingSubsequence();
            IList<Match> lisList = lis.SortMum(MUM);

            List<Match> expectedOutput = new List<Match>();
            mum = new Match();
            mum.ReferenceSequenceOffset = 0;
            mum.Length = 3;
            mum.QuerySequenceOffset = 3;
            expectedOutput.Add(mum);

            mum = new Match();
            mum.ReferenceSequenceOffset = 4;
            mum.Length = 3;
            mum.QuerySequenceOffset = 0;
            expectedOutput.Add(mum);

            Assert.IsTrue(CompareMumList(lisList, expectedOutput));
        }

        /// <summary>
        /// Compares two list of Mum against their SecondSequeceMumOrder value.
        /// </summary>
        /// <param name="lisList">First list to be compared.</param>
        /// <param name="expectedOutput">Second list to be compared.</param>
        /// <returns>true if the order of their SecondSequeceMumOrder are same.</returns>
        private static bool CompareMumList(
                IList<Match> lisList,
                IList<Match> expectedOutput)
        {
            if (lisList.Count == expectedOutput.Count)
            {
                bool correctOutput = true;
                for (int index = 0; index < expectedOutput.Count; index++)
                {
                    if (lisList[index].ReferenceSequenceOffset != expectedOutput[index].ReferenceSequenceOffset)
                    {
                        correctOutput = false;
                        break;
                    }

                    if (lisList[index].Length != expectedOutput[index].Length)
                    {
                        correctOutput = false;
                        break;
                    }

                    if (lisList[index].QuerySequenceOffset != expectedOutput[index].QuerySequenceOffset)
                    {
                        correctOutput = false;
                        break;
                    }
                }

                return correctOutput;
            }
            return false;
        }

    }
}
