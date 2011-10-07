using Bio.Algorithms.Alignment;
using Bio.Phylogenetics;

namespace Bio.Web.ClustalW
{
    /// <summary>
    /// Represent a result of ClustalW Web service.
    /// ClustalW will return either of the following
    ///  SequenceAlignment: If the operation performed is Alignment.
    ///  Tree: If the operation performed is tree for multiple sequences.
    /// </summary>
    public class ClustalWResult
    {
        /// <summary>
        /// Sequence Alignment result of ClustalW
        /// </summary>
        private ISequenceAlignment sequenceAlignment;

        /// <summary>
        /// Tree result of ClustalW
        /// </summary>
        private Tree tree;

        /// <summary>
        /// Constructor to set the SequenceAlignment result
        /// </summary>
        /// <param name="sequenceAlignment">Sequence alignment object</param>
        public ClustalWResult(ISequenceAlignment sequenceAlignment)
        {
            this.sequenceAlignment = sequenceAlignment;
        }

        /// <summary>
        /// Constructor to set the Tree result
        /// </summary>
        /// <param name="tree">Phylogentic Tree object</param>
        public ClustalWResult(Tree tree)
        {
            this.tree = tree;
        }

        /// <summary>
        /// Gets the SequenceAlignment result of ClustalW
        /// </summary>
        public ISequenceAlignment SequenceAlignment
        {
            get { return sequenceAlignment; }
        }

        /// <summary>
        /// Gets the Tree result of ClustalW
        /// </summary>
        public Tree Tree
        {
            get { return tree; }
        }
    }
}
