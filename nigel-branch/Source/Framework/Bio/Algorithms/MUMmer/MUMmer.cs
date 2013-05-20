using System;
using System.Collections.Generic;
using Bio.Algorithms.SuffixTree;

namespace Bio.Algorithms.MUMmer
{
    /// <summary>
    /// MUMmer is a system for rapidly aligning entire genomes or very large protein
    /// sequences. It is a pair wise sequence algorithm. The algorithm assumes the 
    /// sequences are closely related, and using this assumption can quickly compare
    /// sequences that are millions of nucleotides in length. The algorithm is 
    /// designed to perform high resolution comparison of genome-length sequences. 
    /// </summary>
    public class MUMmer
    {
        /// <summary>
        /// Holds the suffix tree.
        /// </summary>
        private readonly ISuffixTree _suffixTree;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MUMmer class with specified reference sequence.
        /// </summary>
        /// <param name="referenceSequence">Reference sequence.</param>
        public MUMmer(ISequence referenceSequence)
        {
            if (referenceSequence == null)
            {
                throw new ArgumentNullException("referenceSequence");
            }

            this.ReferenceSequence = referenceSequence;

            // build the suffix tree for the reference sequence.
            this._suffixTree = new MultiWaySuffixTree(referenceSequence);

            // Default Min length of Match - set to 20.
            this.LengthOfMUM = 20;
            this.NoAmbiguity = false;

            this.Name = Properties.Resource.MUMmerName;
            this.Description = Properties.Resource.MUMmerDescription;
        }

        /// <summary>
        /// Initializes a new instance of the MUMmer class with the specified suffix tree.
        /// This enables to use custom suffix tree.
        /// </summary>
        /// <param name="suffixTree">Suffix tree.</param>
        public MUMmer(ISuffixTree suffixTree)
        {
            if (suffixTree == null)
            {
                throw new ArgumentNullException("suffixTree");
            }

            this._suffixTree = suffixTree;
            this.ReferenceSequence = this._suffixTree.Sequence;

            // Default Min length of Match - set to 20.
            this.LengthOfMUM = 20;
            this.NoAmbiguity = false;
            this.Name = Properties.Resource.MUMmerName;
            this.Description = Properties.Resource.MUMmerDescription;
        }

        #endregion

        #region -- Properties --

        /// <summary>
        /// Gets the name of the MUMmer.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the description of the MUMmer.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the length of MUM.
        /// </summary>
        public long LengthOfMUM { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only basic symbols should be matched.
        /// Thus the matches will not contains any ambiguous symbols.
        /// </summary>
        public bool NoAmbiguity { get; set; }

        /// <summary>
        /// Gets the referenceSequence.
        /// </summary>
        public ISequence ReferenceSequence { get; private set; }

        #endregion -- Properties --

        /// <summary>
        /// Gets the maximum matches - MaxMatch.
        /// This method does not considers uniqueness.
        /// </summary>
        /// <param name="querySequence">Query sequence.</param>
        /// <returns>Returns IEnumerable of MUMs.</returns>
        public IEnumerable<Match> GetMatches(ISequence querySequence)
        {
            // Set the required properties of suffix tree.
            this._suffixTree.MinLengthOfMatch = this.LengthOfMUM;
            this._suffixTree.NoAmbiguity = this.NoAmbiguity;

            // Get matches from the suffix tree with out considering the uniqueness.
            return this._suffixTree.SearchMatches(querySequence);
        }

        /// <summary>
        /// Gets the maximum unique matches in reference sequence.
        /// </summary>
        /// <param name="querySequence">Query sequence.</param>
        /// <returns>Returns IEnumerable of MUMs.</returns>
        public IEnumerable<Match> GetMatchesUniqueInReference(ISequence querySequence)
        {
            // Set the required properties of suffix tree.
            this._suffixTree.MinLengthOfMatch = this.LengthOfMUM;
            this._suffixTree.NoAmbiguity = this.NoAmbiguity;

            // Get matches that are unique in reference sequence from the suffix tree. 
            return this._suffixTree.SearchMatchesUniqueInReference(querySequence);
        }
    }
}
