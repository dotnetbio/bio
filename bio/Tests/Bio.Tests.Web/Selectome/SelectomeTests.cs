using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Selectome;
using System.Linq;
using Bio.Algorithms.Alignment.MultipleSequenceAlignment;
using Bio.SimilarityMatrices;

namespace Bio.Tests.Web.Selectome
{
    /// <summary>
    /// These tests are from 10/13/2013 and verify values returned by the database, if they fail
    /// they can either be due to changes in the parser changing the data, or the underlying data having changed.
    /// The second scenario can be quickly checked by going to the website and querying through the interface.
    /// </summary>
    [TestClass]
    public class SelectomeTests
    {
        /// <summary>
        /// Verify that Selectome can correctly calculate the alignment score
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0"), TestCategory("Selectome")]
        public void TestSelectomeAlignmentCalculation()
        {
            var d = SelectomeDataFetcher.FetchGeneByEnsemblID("ENSG00000125246").Gene;
            Assert.AreEqual(1288.265869140625, d.GetUnmaskedBlosum90AlignmentScore());
        }

        /// <summary>
        /// Verify that selectome calculates the expected MSA score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0"), TestCategory("Selectome")]
        public void TestSelectomeQuery()
        {
            var d = SelectomeDataFetcher.FetchGeneByEnsemblID("ENSG00000125246").Gene;
            Assert.AreEqual(41, d.VertebrateTree.TaxaPresent.Count); 
        }
        /// <summary>
        /// Verifies that a gene which caused problems previously can work, the formatting on 
        /// this one may be weird.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0"), TestCategory("Selectome")]
        public void TestSelectomeQueryCarnitinePalmitoTransferase()
        {
            var d = SelectomeDataFetcher.FetchGeneByEnsemblID("ENSG00000110090").Gene;
            //var d = SelectomeDataFetcher.FetchGeneByEnsemblID("CPT1A").Gene;
            //TODO: Should be 41 for unique taxa.
            Assert.AreEqual(45, d.VertebrateTree.TaxaPresent.Count);
        }
        /// <summary>
        /// Test the number of selected nodes are correctly identified
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0"), TestCategory("Selectome")]
        public void TestSelectomeSelectedNodesCountParsing()
        {

            var d = SelectomeDataFetcher.FetchGeneByEnsemblID("ENSG00000125246").Gene;
            var tree = d.VertebrateTree;
            var count = d.VertebrateTree.AllNodes().Where(x => x.Selected.HasValue && x.Selected.Value).Count();
            Assert.AreEqual(2, count);
        }
        /// <summary>
        /// Test MSA parsing
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0"), TestCategory("Selectome")]
        public void TestSelectomeMSAParsing()
        {
            var d = SelectomeDataFetcher.FetchGeneByEnsemblID("ENSG00000125246").Gene;
            var msa = d.MaskedAminoAcidAlignment;
            Assert.AreEqual(594, msa.NumberOfColumns);
        }

        /// <summary>
        /// Make sure queries start with correct value
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0"), TestCategory("Selectome")]
        public void TestSelectomeQueryPrefixCheck()
        {
            try
            {
                var d = SelectomeDataFetcher.FetchGeneByEnsemblID("00000125246");
                Assert.Fail("Expected exception for bad query");
            }
            catch (ArgumentException)
            {

            }
        }

        /// <summary>
        /// Make sure tree root is correct
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0"), TestCategory("Selectome")]
        public void TestSelectomeQueryTreeRootName()
        {           
            var d = SelectomeDataFetcher.FetchGeneByEnsemblID("ENSG00000125246").Gene;
            Assert.AreEqual(SelectomeConstantsAndEnums.GroupToNameMapping[SelectomeTaxaGroup.Euteleostomi], d.VertebrateTree.Root.Name);
        }


    }
}
