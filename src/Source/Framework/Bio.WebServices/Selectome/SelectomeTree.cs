using System;
using System.Collections.Generic;
using System.Linq;

using Bio.Phylogenetics;
namespace Bio.Web.Selectome
{
    /// <summary>
    /// A phylogenetic tree with some additional fields made available by the selectome data
    /// </summary>
    public class SelectomeTree
    {
        /// <summary>
        /// Initializes a new selectome tree from a tree parsed by the Newick parser
        /// </summary>
        /// <param name="initialTree"></param>
        public SelectomeTree(Tree initialTree)
        {
            if (initialTree == null)
            {
                throw new ArgumentNullException("initialTree");
            }
            this.Root = new SelectomeTreeNode(initialTree.Root);
            this.Name = initialTree.Name;
            this.IsRoot = true;
        }

        /// <summary>
        /// The name of the tree.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Either node is root node or not
        /// </summary>
        public bool IsRoot { set; get; }

        /// <summary>
        /// The root of the tree.
        /// </summary>
        public SelectomeTreeNode Root { get; set; }

        /// <summary>
        /// All TaxaShortName fields from the leaves
        /// </summary>
        public List<string> TaxaPresent
        {
            get
            {
                return this.GetFeatureFromLeaves(x => x.TaxaShortName);
            }
        }

        /// <summary>
        /// All TaxaShortName fields from the leaves
        /// </summary>
        public List<Tuple<string, string, string>> TaxaInformation
        {
            get
            {
                return
                    this.GetFeatureFromLeaves(x => new Tuple<string, string, string>(x.Name, x.TaxID, x.TaxaShortName));
            }
        }

        /// <summary>
        /// Retrieve any nodes from the tree that show selection
        /// </summary>
        public List<SelectomeTreeNode> SelectedNodes
        {
            get
            {
                return this.AllNodes().Where(x => x.Selected.HasValue && x.Selected.Value).ToList();
            }
        }

        /// <summary>
        /// Apply the selection function to each element of the tree
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        public List<T> GetFeatureFromLeaves<T>(Func<SelectomeTreeNode, T> function)
        {
            var result = new List<T>();
            this.AddFeaturesFromChildLeaves(function, this.Root, result);
            return result;
        }

        private void AddFeaturesFromChildLeaves<T>(
            Func<SelectomeTreeNode, T> function,
            SelectomeTreeNode node,
            List<T> accumulator)
        {
            if (node.IsLeaf)
            {
                accumulator.Add(function(node));
            }
            else
            {
                foreach (var child in node.Children)
                {
                    this.AddFeaturesFromChildLeaves(function, child.Key, accumulator);
                }
            }
        }

        /// <summary>
        /// Returns all leaves on the tree
        /// </summary>
        /// <returns></returns>
        public List<SelectomeTreeNode> AllLeaves()
        {
            return this.GetFeatureFromLeaves(x => x);
        }

        /// <summary>
        /// All nodes in the tree
        /// </summary>
        /// <returns></returns>
        public List<SelectomeTreeNode> AllNodes()
        {
            return this.GetFeatureFromNodes(x => x);
        }

        /// <summary>
        /// Apply the function selector to all nodes in the tree
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        public List<T> GetFeatureFromNodes<T>(Func<SelectomeTreeNode, T> function)
        {
            var result = new List<T>();
            this.AddFeaturesFromChildren(function, this.Root, result);
            return result;
        }

        private void AddFeaturesFromChildren<T>(
            Func<SelectomeTreeNode, T> function,
            SelectomeTreeNode node,
            List<T> accumulator)
        {
            accumulator.Add(function(node));
            foreach (var child in node.Children)
            {
                this.AddFeaturesFromChildren(function, child.Key, accumulator);
            }
        }
    }
}