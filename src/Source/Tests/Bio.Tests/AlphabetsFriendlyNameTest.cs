using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Bio.Tests.Framework
{
    /// <summary>
    /// Tests the AmbiguousRnaAlphabet class.
    /// </summary>
    [TestFixture]
    public class AlphabetsFriendlyNameTest
    {
        /// <summary>
        /// Tests the AmbiguousRNAAlphabet class.
        /// </summary>
        [Test]
        public void TestFriendlyNames()
        {
            // DNA
            Assert.AreEqual(DnaAlphabet.Instance.GetFriendlyName(DnaAlphabet.Instance.A), "Adenine");
            Assert.AreEqual(DnaAlphabet.Instance.GetFriendlyName(DnaAlphabet.Instance.C), "Cytosine");
            Assert.AreEqual(DnaAlphabet.Instance.GetFriendlyName(DnaAlphabet.Instance.G), "Guanine");
            Assert.AreEqual(DnaAlphabet.Instance.GetFriendlyName(DnaAlphabet.Instance.T), "Thymine");
            Assert.AreEqual(DnaAlphabet.Instance.GetFriendlyName(DnaAlphabet.Instance.Gap), "Gap");

            Assert.AreEqual(AmbiguousDnaAlphabet.Instance.GetFriendlyName(AmbiguousDnaAlphabet.Instance.AC), "Adenine or Cytosine");
            Assert.AreEqual(AmbiguousDnaAlphabet.Instance.GetFriendlyName(AmbiguousDnaAlphabet.Instance.GA), "Guanine or Adenine");
            Assert.AreEqual(AmbiguousDnaAlphabet.Instance.GetFriendlyName(AmbiguousDnaAlphabet.Instance.GC), "Guanine or Cytosine");
            Assert.AreEqual(AmbiguousDnaAlphabet.Instance.GetFriendlyName(AmbiguousDnaAlphabet.Instance.AT), "Adenine or Thymine");
            Assert.AreEqual(AmbiguousDnaAlphabet.Instance.GetFriendlyName(AmbiguousDnaAlphabet.Instance.TC), "Thymine or Cytosine");
            Assert.AreEqual(AmbiguousDnaAlphabet.Instance.GetFriendlyName(AmbiguousDnaAlphabet.Instance.GT), "Guanine or Thymine");
            Assert.AreEqual(AmbiguousDnaAlphabet.Instance.GetFriendlyName(AmbiguousDnaAlphabet.Instance.GCA), "Guanine or Cytosine or Adenine");
            Assert.AreEqual(AmbiguousDnaAlphabet.Instance.GetFriendlyName(AmbiguousDnaAlphabet.Instance.ACT), "Adenine or Cytosine or Thymine");
            Assert.AreEqual(AmbiguousDnaAlphabet.Instance.GetFriendlyName(AmbiguousDnaAlphabet.Instance.GAT), "Guanine or Adenine or Thymine");
            Assert.AreEqual(AmbiguousDnaAlphabet.Instance.GetFriendlyName(AmbiguousDnaAlphabet.Instance.GTC), "Guanine or Thymine or Cytosine");
            Assert.AreEqual(AmbiguousDnaAlphabet.Instance.GetFriendlyName(AmbiguousDnaAlphabet.Instance.Any), "Any");

            // RNA
            Assert.AreEqual(RnaAlphabet.Instance.GetFriendlyName(RnaAlphabet.Instance.A), "Adenine");
            Assert.AreEqual(RnaAlphabet.Instance.GetFriendlyName(RnaAlphabet.Instance.C), "Cytosine");
            Assert.AreEqual(RnaAlphabet.Instance.GetFriendlyName(RnaAlphabet.Instance.G), "Guanine");
            Assert.AreEqual(RnaAlphabet.Instance.GetFriendlyName(RnaAlphabet.Instance.U), "Uracil");
            Assert.AreEqual(RnaAlphabet.Instance.GetFriendlyName(RnaAlphabet.Instance.Gap), "Gap");

            Assert.AreEqual(AmbiguousRnaAlphabet.Instance.GetFriendlyName(AmbiguousRnaAlphabet.Instance.Any), "Any");
            Assert.AreEqual(AmbiguousRnaAlphabet.Instance.GetFriendlyName(AmbiguousRnaAlphabet.Instance.AC), "Adenine or Cytosine");
            Assert.AreEqual(AmbiguousRnaAlphabet.Instance.GetFriendlyName(AmbiguousRnaAlphabet.Instance.GA), "Guanine or Adenine");
            Assert.AreEqual(AmbiguousRnaAlphabet.Instance.GetFriendlyName(AmbiguousRnaAlphabet.Instance.GC), "Guanine or Cytosine");
            Assert.AreEqual(AmbiguousRnaAlphabet.Instance.GetFriendlyName(AmbiguousRnaAlphabet.Instance.AU), "Adenine or Uracil");
            Assert.AreEqual(AmbiguousRnaAlphabet.Instance.GetFriendlyName(AmbiguousRnaAlphabet.Instance.UC), "Uracil or Cytosine");
            Assert.AreEqual(AmbiguousRnaAlphabet.Instance.GetFriendlyName(AmbiguousRnaAlphabet.Instance.GU), "Guanine or Uracil");
            Assert.AreEqual(AmbiguousRnaAlphabet.Instance.GetFriendlyName(AmbiguousRnaAlphabet.Instance.GCA), "Guanine or Cytosine or Adenine");
            Assert.AreEqual(AmbiguousRnaAlphabet.Instance.GetFriendlyName(AmbiguousRnaAlphabet.Instance.ACU), "Adenine or Cytosine or Uracil");
            Assert.AreEqual(AmbiguousRnaAlphabet.Instance.GetFriendlyName(AmbiguousRnaAlphabet.Instance.GAU), "Guanine or Adenine or Uracil");
            Assert.AreEqual(AmbiguousRnaAlphabet.Instance.GetFriendlyName(AmbiguousRnaAlphabet.Instance.GUC), "Guanine or Uracil or Cytosine");

            // Protein
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.A), "Alanine");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.C), "Cysteine");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.D), "Aspartic");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.E), "Glutamic");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.F), "Phenylalanine");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.G), "Glycine");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.H), "Histidine");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.I), "Isoleucine");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.K), "Lysine");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.L), "Leucine");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.M), "Methionine");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.N), "Asparagine");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.O), "Pyrrolysine");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.P), "Proline");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.Q), "Glutamine");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.R), "Arginine");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.S), "Serine");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.T), "Threoine");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.U), "Selenocysteine");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.V), "Valine");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.W), "Tryptophan");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.Y), "Tyrosine");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.Gap), "Gap");
            Assert.AreEqual(ProteinAlphabet.Instance.GetFriendlyName(ProteinAlphabet.Instance.Ter), "Termination");

            Assert.AreEqual(AmbiguousProteinAlphabet.Instance.GetFriendlyName(AmbiguousProteinAlphabet.Instance.X), "Undetermined or atypical");
            Assert.AreEqual(AmbiguousProteinAlphabet.Instance.GetFriendlyName(AmbiguousProteinAlphabet.Instance.Z), "Glutamic or Glutamine");
            Assert.AreEqual(AmbiguousProteinAlphabet.Instance.GetFriendlyName(AmbiguousProteinAlphabet.Instance.B), "Aspartic or Asparagine");
            Assert.AreEqual(AmbiguousProteinAlphabet.Instance.GetFriendlyName(AmbiguousProteinAlphabet.Instance.J), "Leucine or Isoleucine");
        }
    }
}
