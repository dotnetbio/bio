using System.Collections.Generic;
using Bio;
using Bio.Algorithms.Alignment.MultipleSequenceAlignment;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests
{
    /// <summary>
    /// Test for Profile class
    /// </summary>
    [TestClass]
    public class ProfileTests
    {
        /// <summary>
        /// Test Profile class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestProfile()
        {
            ISequence templateSequence = new Sequence(Alphabets.AmbiguousDNA, "ATGCSWRYKMBVHDN-");
            Dictionary<byte, int> itemSet = new Dictionary<byte, int>();
            for (int i = 0; i < templateSequence.Count; ++i)
            {
                itemSet.Add(templateSequence[i], i);

                if (char.IsLetter((char)templateSequence[i]))
                    itemSet.Add((byte)char.ToLower((char)templateSequence[i]), i);
            }
            Profiles.ItemSet = itemSet;

            ISequence seqA = new Sequence(Alphabets.DNA, "GGGAAAAATCAGATT");
            ISequence seqB = new Sequence(Alphabets.DNA, "GGGAATCAAAATCAG");

            List<ISequence> sequences = new List<ISequence>();
            sequences.Add(seqA);
            sequences.Add(seqB);

            // Test GenerateProfiles
            IProfiles profileA = Profiles.GenerateProfiles(sequences[0]);
            Assert.AreEqual(16, profileA.ColumnSize);
            Assert.AreEqual(sequences[0].Count, profileA.RowSize);

            // Test ProfileMatrix
            Assert.AreEqual(1, profileA.ProfilesMatrix[0][2]);
            Assert.AreEqual(0, profileA.ProfilesMatrix[0][3]);

            // Test ProfileAlignment
            IProfileAlignment profileAlignmentA = ProfileAlignment.GenerateProfileAlignment(sequences[0]);
            Assert.AreEqual(1, profileAlignmentA.ProfilesMatrix[0][2]);
            Assert.AreEqual(0, profileAlignmentA.ProfilesMatrix[0][3]);
            Assert.AreEqual(1, profileAlignmentA.NumberOfSequences);

            IProfileAlignment profileAlignmentB = ProfileAlignment.GenerateProfileAlignment(sequences);
            Assert.AreEqual(1, profileAlignmentB.ProfilesMatrix[0][2]);
            Assert.AreEqual(0, profileAlignmentB.ProfilesMatrix[0][3]);
            Assert.AreEqual(2, profileAlignmentB.NumberOfSequences);

            Assert.AreEqual(0.5, profileAlignmentB.ProfilesMatrix[5][0]);
            Assert.AreEqual(0.5, profileAlignmentB.ProfilesMatrix[5][1]);
            Assert.AreEqual(0, profileAlignmentB.ProfilesMatrix[5][2]);
        }
    }
}
