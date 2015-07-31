using System;
using System.Collections.Generic;
using System.Globalization;

using Bio.Phylogenetics;

namespace Bio.Web.Selectome
{
    /// <summary>
    /// A Selectome tree node is a normal node with additional abilities to get
    /// metadata relevant to selectome.
    /// </summary>
    public class SelectomeTreeNode
    {
        /// <summary>
        /// A new dictionary of children that hides the old ones
        /// </summary>
        protected Dictionary<SelectomeTreeNode, Edge> children;

        /// <summary>
        /// Create a new node from the initial node, recursively changing things.
        /// </summary>
        /// <param name="initialNode">The node found from the newick parser</param>
        public SelectomeTreeNode(Node initialNode)
        {
            if (initialNode == null)
            {
                throw new NullReferenceException("initialNode");
            }
            this.MetaData = initialNode.MetaData;
            this.children = new Dictionary<SelectomeTreeNode, Edge>();
            this.Name = initialNode.Name;
            //recursively update child nodes
            foreach (var child in initialNode.Children)
            {
                this.children.Add(new SelectomeTreeNode(child.Key), child.Value);
            }
        }

        /// <summary>
        /// The name of the tree node.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The metadata for the string
        /// </summary>
        public IDictionary<string, string> MetaData { get; set; }

        /// <summary>
        /// Get Children nodes
        /// </summary>
        public IDictionary<SelectomeTreeNode, Edge> Children
        {
            get
            {
                return this.children;
            }
        }

        /// <summary>
        /// Either node is root node or not
        /// </summary>
        public bool IsRoot { set; get; }

        /// <summary>
        /// Either node is leaf or not
        /// </summary>
        public bool IsLeaf
        {
            get
            {
                return this.Children.Keys.Count == 0;
            }
        }

        /// <summary>
        /// Is the branch selected?
        /// </summary>
        public bool? Selected
        {
            get
            {
                string val;
                if (this.MetaData.TryGetValue("SEL", out val))
                {
                    return val == "Y";
                }
                return null;
            }
        }

        /// <summary>
        /// Is an ancestral branch selected
        /// </summary>
        public bool? AncestorSelected
        {
            get
            {
                string val;
                if (this.MetaData.TryGetValue("ASEL", out val))
                {
                    return val == "Y";
                }
                return null;
            }
        }

        /// <summary>
        /// The bootstrap value on the tree.
        /// </summary>
        public double? BootStrap
        {
            get
            {
                string val;
                if (this.MetaData.TryGetValue("B", out val))
                {
                    return Convert.ToDouble(val, CultureInfo.CurrentCulture);
                }
                return null;
            }
        }

        /// <summary>
        /// The NCBI taxonomic ID
        /// </summary>
        public string TaxID
        {
            get
            {
                string val;
                return this.MetaData.TryGetValue("T", out val) ? val : null;
            }
        }

        /// <summary>
        /// P-Value
        /// </summary>
        public double? PValue
        {
            get
            {
                string val;
                return !this.MetaData.TryGetValue("PVAL", out val)
                           ? null
                           : (val != "NA" ? (double?) double.Parse(val) : null);
            }
        }

        /// <summary>
        /// The ensembl id of the gene
        /// </summary>
        public string GeneName
        {
            get
            {
                string val;
                return this.MetaData.TryGetValue("G", out val) ? val : null;
            }
        }

        /// <summary>
        /// The ensembl id of the gene
        /// </summary>
        public string GeneID
        {
            get
            {
                string val;
                return this.MetaData.TryGetValue("GN", out val) ? val : null;
            }
        }

        /// <summary>
        /// The short name for the taxa
        /// </summary>
        public string TaxaShortName
        {
            get
            {
                string val;
                return this.MetaData.TryGetValue("S", out val) ? val : null;
            }
        }

        /// <summary>
        /// Is a duplication inferred?
        /// </summary>
        public bool? Duplication
        {
            get
            {
                string val;
                return this.MetaData.TryGetValue("D", out val) ? (bool?)(val == "Y") : null;
            }
        }

        /// <summary>
        /// The transcript ID
        /// </summary>
        public string TranscriptID
        {
            get
            {
                string val;
                return this.MetaData.TryGetValue("TR", out val) ? val : null;
            }
        }

        /// <summary>
        /// The protein ID
        /// </summary>
        public string ProteinID
        {
            get
            {
                string val;
                return this.MetaData.TryGetValue("PR", out val) ? val : null;
            }
        }
    }
}