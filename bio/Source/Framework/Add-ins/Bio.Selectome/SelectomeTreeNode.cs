using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bio.Phylogenetics;
using System.Globalization;

namespace Bio.Selectome
{
    /// <summary>
    /// A Selectome tree node is a normal node with additional abilities to get 
    /// metadata relevant to selectome.
    /// </summary>
    public class SelectomeTreeNode 
    {

        /// <summary>
        /// The name of the tree node.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The metadata for the string
        /// </summary>
        public Dictionary<string, string> MetaData {get;set;}

        /// <summary>
        /// A new dictionary of children that hides the old ones
        /// </summary>
        protected Dictionary<SelectomeTreeNode, Edge> _Children;

        // <summary>
        /// Get Childern nodes
        /// </summary>
        public Dictionary<SelectomeTreeNode, Edge> Children 
        { 
            get 
            { 
                return _Children; 
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
                return Children.Keys.Count == 0;
            }
        }

        /// <summary>
        /// Create a new node from the initial node, recursively changing things.
        /// </summary>
        /// <param name="initialNode">The node found from the newick parser</param>
        public SelectomeTreeNode(Node initialNode)
        {
            if(initialNode==null)
            {
                throw new NullReferenceException("initialNode");
            }
            this.MetaData = initialNode.MetaData;
            this._Children = new Dictionary<SelectomeTreeNode, Edge>();
            this.Name = initialNode.Name;
            //recursively update child nodes
            foreach (var child in initialNode.Children)
            {
                this._Children.Add(new SelectomeTreeNode(child.Key), child.Value);
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
                if (MetaData.TryGetValue("SEL", out val))
                {
                    return val == "Y" ? true : false;
                }
                else { return null; }
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
                if (MetaData.TryGetValue("ASEL", out val))
                {
                    return val == "Y" ? true : false;
                }
                else { return null; }
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
                if (MetaData.TryGetValue("B", out val))
                {
                    return Convert.ToDouble(val,CultureInfo.CurrentCulture);
                }
                else { return null; }
            }
        }
        /// <summary>
        /// The NCBI taxonomic ID
        /// </summary>
        public string TaxaID
        {
            get
            {
                string val;
                if (MetaData.TryGetValue("T", out val))
                {
                    return val;
                }
                else { return null; }
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
                if (MetaData.TryGetValue("PVAL", out val))
                {
                    if (val != "NA")
                    {

                        return ConvertString(val);
                    }
                }
                return null; 
            }
        }
        private static double ConvertString(string str)
        {
            try
            {
                return Convert.ToDouble(str,CultureInfo.CurrentCulture);
            }
            catch (FormatException fe)
            {
                throw new FormatException("Could not convert: " + str + " to a double", fe);
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
                if (MetaData.TryGetValue("G", out val))
                {
                    return val;
                }
                else { return null; }
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
                if (MetaData.TryGetValue("GN", out val))
                {
                    return val;
                }
                else { return null; }
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
                if (MetaData.TryGetValue("S", out val))
                {
                    return val;
                }
                else { return null; }
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
                if (MetaData.TryGetValue("D", out val))
                {
                    return val == "Y" ? true : false;
                }
                else { return null; }
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
                if (MetaData.TryGetValue("TR", out val))
                {
                    return val;
                }
                else { return null; }
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
                if (MetaData.TryGetValue("PR", out val))
                {
                    return val;
                }
                else { return null; }
            }
        }
    }
}
