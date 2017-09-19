using System;

namespace Bio.Web.Selectome
{
    /// <summary>
    /// An enumeration of results returned by selectome
    /// </summary>
    public enum QueryResult
    {
        /// <summary>
        /// Successfully returned data for the vertebrate tree.
        /// </summary>
        Success,

        /// <summary>
        /// Nothing returned
        /// </summary>
        NoResultsFound,

        /// <summary>
        /// Data was returned, but not for the verterbrate/eulestomi tree
        /// </summary>
        NoVeterbrateTreeDataFound
    };

    /// <summary>
    /// A class to be returned from a query, indicates success or failure, and the gene if success
    /// </summary>
    public class SelectomeQueryResult
    {
        private readonly SelectomeGene pGene;

        /// <summary>
        /// Create a new Query Result from Selectome
        /// </summary>
        /// <param name="status"></param>
        /// <param name="gene"></param>
        public SelectomeQueryResult(QueryResult status, SelectomeGene gene = null)
        {
            this.Result = status;
            this.pGene = gene;
        }

        /// <summary>
        /// An enum describing the result
        /// </summary>
        public QueryResult Result { get; private set; }

        /// <summary>
        /// A Gene present when the result is successful;
        /// </summary>
        public SelectomeGene Gene
        {
            get
            {
                if (this.Result != QueryResult.Success)
                {
                    throw new Exception("Cannot get gene from unsuccessful query result");
                }
                return this.pGene;
            }
        }
    }

    /// <summary>
    /// A parsed result from the selectome database.
    /// </summary>
    public class SelectomeQuerySubResult
    {
        /// <summary>
        /// Was selection inferred?
        /// </summary>
        public readonly bool SelectionInferred;

        /// <summary>
        /// Create a new result.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="selectionInferred"></param>
        public SelectomeQuerySubResult(SelectomeTaxaGroup group, bool selectionInferred) // double pval, int aminoAcids)
        {
            this.Group = group;
            this.SelectionInferred = selectionInferred; //pValue = pval; AminoAcidsUnderSelection = aminoAcids;
        }

        /// <summary>
        /// What tree/taxonomic group is this result from?
        /// </summary>
        public SelectomeTaxaGroup Group { get; private set; }

        /// <summary>
        /// The link related to this result
        /// </summary>
        public SelectomeQueryLink RelatedLink { get; set; }
    }

    /// <summary>
    /// The data representing the link used to
    /// </summary>
    public class SelectomeQueryLink
    {
        /// <summary>
        /// The group
        /// </summary>
        public readonly SelectomeTaxaGroup Group;

        /// <summary>
        /// The genes sub-tree (see ENSEMBL, this typically is relevant for multi-gene families thathave subtrees for each type.)
        /// </summary>
        public readonly string SubTree;

        /// <summary>
        /// The tree
        /// </summary>
        public readonly string Tree;

        /// <summary>
        /// Create a new link result
        /// </summary>
        /// <param name="group"></param>
        /// <param name="tree"></param>
        /// <param name="subtree"></param>
        public SelectomeQueryLink(SelectomeTaxaGroup group, string tree, string subtree)
        {
            this.Group = group;
            this.Tree = tree;
            this.SubTree = subtree;
        }
    }
}