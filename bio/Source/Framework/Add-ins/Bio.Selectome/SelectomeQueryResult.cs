using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bio.Selectome
{
    /// <summary>
    /// An enumeration of results returned by selectome
    /// </summary>
    public enum QUERY_RESULT { 
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
        NoVeterbrateTreeDataFound };
    /// <summary>
    /// A class to be returned from a query, indicates success or failure, and the gene if success
    /// </summary>
    public class SelectomeQueryResult
    {
        /// <summary>
        /// An enum describing the result
        /// </summary>
        public readonly QUERY_RESULT Result;
        /// <summary>
        /// A Gene present when the result is successful;
        /// </summary>
        public SelectomeGene Gene
        {
            get{
                if (Result != QUERY_RESULT.Success)
                {
                    throw new Exception("Cannot get gene from unsuccessful query result");
                }
                return pGene;
            }
        }
        private SelectomeGene pGene;
        /// <summary>
        /// Create a new Query Result from Selectome
        /// </summary>
        /// <param name="status"></param>
        /// <param name="gene"></param>
        public SelectomeQueryResult(QUERY_RESULT status, SelectomeGene gene=null)
        {
            Result = status;
            pGene = gene;            
        }
    }
    /// <summary>
    /// A parsed result from the selectome database.
    /// </summary>
    public class SelectomeQuerySubResult
    {
        /// <summary>
        /// What tree/taxonomic group is this result from?
        /// </summary>
        public SelectomeTaxaGroup Group {get;private set;}
        /// <summary>
        /// Was selection inferred?
        /// </summary>
        public readonly bool SelectionInferred;
        /// <summary>
        /// Create a new result.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="selectionInferred"></param>
        public SelectomeQuerySubResult(SelectomeTaxaGroup group, bool selectionInferred)// double pval, int aminoAcids)
        {
            Group = group; SelectionInferred = selectionInferred; //pValue = pval; AminoAcidsUnderSelection = aminoAcids;
        }
        /// <summary>
        /// The link related to this result
        /// </summary>
        public SelectomeQueryLink RelatedLink {get;set;}
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
        /// The tree
        /// </summary>
        public readonly string Tree;
        /// <summary>
        /// The genes sub-tree (see ENSEMBL, this typically is relevant for multi-gene families thathave subtrees for each type.) 
        /// </summary>
        public readonly string SubTree;
        /// <summary>
        /// Create a new link result
        /// </summary>
        /// <param name="group"></param>
        /// <param name="tree"></param>
        /// <param name="subtree"></param>
        public SelectomeQueryLink(SelectomeTaxaGroup group, string tree, string subtree)
        { Group = group; Tree = tree; SubTree = subtree; }
    }
}
