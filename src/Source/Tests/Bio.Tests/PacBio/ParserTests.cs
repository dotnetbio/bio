using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Bio.IO.PacBio;

namespace Bio.Tests.PacBio
{
    [TestFixture]
    public static class ParserTests
    {
        [Test]
        [Category("PacBio")]
        public static void SimpleCCSTest ()
        {
            string fname = System.IO.Path.Combine ("TestUtils", "PacBio", "ccs.bam");
            var csp = new PacBioCCSBamReader ();
            var seqs = csp.Parse (fname).Select(z => z as PacBioCCSRead).ToList();

            var seq4 = seqs [4];
            Assert.AreEqual (146331, seq4.HoleNumber);
            Assert.AreEqual (124, seq4.NumPasses);
            Assert.AreEqual (2, seq4.ReadCountBadZscore);
            Assert.AreEqual (136, seq4.Sequence.Count);
            Assert.AreEqual (128, seq4.ZScores.Length);
            Assert.AreEqual("m141008_060349_42194_c100704972550000001823137703241586_s1_p0/146331/ccs",
                             seq4.Sequence.ID);
            var seq = new Sequence (DnaAlphabet.Instance, seq4.Sequence.ToArray (), true);
            Assert.AreEqual("CCCGGGGATCCTCTAGAATGCTCATACACTGGGGGATACATATACGGGGGGGGGCACATCATCTAGACAGACGACTTTTTTTTTTCGAGCGCAGCTTTTTGAGCGACGCACAAGCTTGCTGAGGACTAGTAGCTTC",
                seq.ConvertToString());

            Assert.AreEqual (seqs.Count, 7);
        }
    }
}

