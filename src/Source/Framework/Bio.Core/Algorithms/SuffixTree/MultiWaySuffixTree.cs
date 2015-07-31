using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Bio.Properties;
using Bio.Util;

namespace Bio.Algorithms.SuffixTree
{
    /// <summary>
    ///     Represents an in-memory suffix tree.
    ///     This implementation of ISuffix tree only works with Sequence class.
    /// </summary>
    public class MultiWaySuffixTree : ISuffixTree
    {
        /// <summary>
        ///     Character ($) used as terminating symbol for Suffix Tree.
        /// </summary>
        private const byte TerminatingSymbol = 36;

        /// <summary>
        ///     Holds the reference sequence.
        ///     This will be converted using SymbolValueMap of the alphabet for the performance.
        /// </summary>
        private readonly Sequence referenceSequence;

        /// <summary>
        ///     Base alphabet supported by this instance of suffix tree.
        ///     This property depends on the reference sequence.
        ///     For example: if the reference sequence's alphabet is
        ///     AmbiguousDna then Dna and its all derivatives classes are supported.
        /// </summary>
        private readonly IAlphabet supportedBaseAlphabet;

        /// <summary>
        ///     Holds the unique symbols present in the reference sequence with their start index.
        /// </summary>
        private readonly HashSet<byte> uniqueSymbolsInReference;

        /// <summary>
        ///     Holds the start index of the symbols in uniqueSymbolsInReference set.
        /// </summary>
        private readonly long[] uniqueSymbolsStartIndexes;

        /// <summary>
        ///     Holds number of symbols in reference sequence.
        /// </summary>
        private readonly long symbolsCount;

        /// <summary>
        ///     Holds number of edges present in the suffix tree.
        /// </summary>
        private long edgesCount;

        /// <summary>
        ///     Gets or sets the root node (edge) in suffix tree.
        /// </summary>
        private MultiWaySuffixEdge rootEdge;

        /// <summary>
        ///     Initializes a new instance of the MultiWaySuffixTree class with the specified sequence.
        /// </summary>
        /// <param name="sequence">Sequence to build the suffix tree.</param>
        public MultiWaySuffixTree(ISequence sequence)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            if (sequence.Count == 0)
            {
                throw new ArgumentOutOfRangeException("sequence", Resource.EmptySequence);
            }

            byte[] aliasMap = sequence.Alphabet.GetSymbolValueMap();
            this.uniqueSymbolsInReference = new HashSet<byte>();
            this.uniqueSymbolsStartIndexes = new long[byte.MaxValue + 1];
            var convertedValeus = new byte[sequence.Count];
            for (int index = 0; index < sequence.Count; index++)
            {
                byte symbol = aliasMap[sequence[index]];
                if (!this.uniqueSymbolsInReference.Contains(symbol))
                {
                    this.uniqueSymbolsStartIndexes[symbol] = index;
                    this.uniqueSymbolsInReference.Add(symbol);
                }

                convertedValeus[index] = symbol;
            }

            this.Sequence = sequence;
            this.referenceSequence = new Sequence(sequence.Alphabet, convertedValeus, false);
            this.symbolsCount = sequence.Count;
            this.Name = Resource.MultiWaySuffixTreeName;
            this.MinLengthOfMatch = 20;
            this.NoAmbiguity = false;

            // Create root edge.
            this.rootEdge = new MultiWaySuffixEdge();
            this.edgesCount++;

            this.supportedBaseAlphabet = sequence.Alphabet;

            IAlphabet alphabet;

            while (Alphabets.AlphabetToBaseAlphabetMap.TryGetValue(this.supportedBaseAlphabet, out alphabet))
            {
                this.supportedBaseAlphabet = alphabet;
            }

            // Build the suffix tree.
            this.BuildSuffixTree();

            // Update tree with suffixLinks.
            this.UpdateSuffixLinks();
        }

        /// <summary>
        ///     Gets total number of edges in this suffix tree.
        /// </summary>
        public long EdgesCount
        {
            get
            {
                return this.edgesCount;
            }
        }

        /// <summary>
        ///     Gets Name of the suffix tree.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets Sequence used to build the suffix tree.
        /// </summary>
        public ISequence Sequence { get; private set; }

        /// <summary>
        ///     Gets or sets Minimum length of match required.
        /// </summary>
        public long MinLengthOfMatch { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether it Matches basic symbols only.
        /// </summary>
        public bool NoAmbiguity { get; set; }

        /// <summary>
        ///     Gets the matches unique in reference sequence where length is greater than or equal to the MinLengthOfMatch.
        /// </summary>
        /// <param name="searchSequence">Sequence to search.</param>
        /// <returns>Returns IEnumerable of matches.</returns>
        public IEnumerable<Match> SearchMatchesUniqueInReference(ISequence searchSequence)
        {
            long minLengthOfMatch = this.MinLengthOfMatch;
            bool noambiguity = this.NoAmbiguity;
            long queryIndex = 0;
            long querySequenceLength = searchSequence.Count;
            long lastMatchQueryStart = 0;
            long lastMatchLength = 0;
            long lengthOfMatchFound = 0;

            var match = new Match();

            // Get base alphabet of the searchSequence.
            IAlphabet searchSeqBaseAlphabet = searchSequence.Alphabet;
            IAlphabet alphabet;

            if (minLengthOfMatch <= 0)
            {
                throw new ArgumentOutOfRangeException(Resource.MinLengthMustBeGreaterThanZero);
            }

            if (!(searchSequence is Sequence))
            {
                throw new ArgumentException(Resource.OnlySequenceClassSupported);
            }

            while (Alphabets.AlphabetToBaseAlphabetMap.TryGetValue(searchSeqBaseAlphabet, out alphabet))
            {
                searchSeqBaseAlphabet = alphabet;
            }

            // If base alphabets are not same then throw the exception.
            if (searchSeqBaseAlphabet != this.supportedBaseAlphabet)
            {
                throw new ArgumentException(Resource.AlphabetMisMatch);
            }

            ISequence convertedSearchSeq = ProcessQuerySequence(searchSequence, noambiguity);

            long lengthOfMatchInEdge = 0;
            long edgeStartIndex = 0;

            MultiWaySuffixEdge edge = this.rootEdge;
            MultiWaySuffixEdge previousIntermediateEdge = this.rootEdge;

            for (queryIndex = 0; queryIndex <= querySequenceLength - minLengthOfMatch; queryIndex++)
            {
                if (previousIntermediateEdge.StartIndex == -1 && lengthOfMatchInEdge > 0)
                {
                    lengthOfMatchInEdge--;
                }

                // As suffix link always point to another intermediate edge.
                // Note: suffix link for the root is root itself.
                previousIntermediateEdge = previousIntermediateEdge.SuffixLink[0];
                int childCount = previousIntermediateEdge.Children.Length;
                lengthOfMatchFound--;

                if (lengthOfMatchFound < 0)
                {
                    lengthOfMatchFound = 0;
                }

                long searchIndex = queryIndex + lengthOfMatchFound - lengthOfMatchInEdge;

                // if lengthOfMatchInEdge is greater than zero then instead of searching from the query index
                // try to jump to the edge starting at lengthOfMatchFound - lengthOfMatchInEdge distance from the root.
                // As previousIntermediateEdge is lengthOfMatchFound distance from the root find an edge in the path of 
                // match such that lengthOfMatchInEdge will end inside that edge.
                byte refSymbol, querySymbol;
                if (lengthOfMatchInEdge > 0)
                {
                    querySymbol = convertedSearchSeq[searchIndex];
                    for (int index = 0; index < childCount; index++)
                    {
                        edge = previousIntermediateEdge.Children[index];

                        edgeStartIndex = edge.StartIndex;

                        refSymbol = TerminatingSymbol;
                        if (edgeStartIndex < this.symbolsCount)
                        {
                            refSymbol = this.referenceSequence[edgeStartIndex];
                        }

                        if (refSymbol == querySymbol)
                        {
                            break;
                        }
                    }

                    // When lengthOfMatchInEdge > 0 there will be an edge from the previousIntermediateEdge in the path of match.
                    while (!edge.IsLeaf)
                    {
                        long edgeEndIndex = edge.Children[0].StartIndex - 1;

                        // compare the first symbol of the edge.
                        long edgeSymbolCount = edgeEndIndex - edgeStartIndex + 1;
                        if (lengthOfMatchInEdge == edgeSymbolCount)
                        {
                            previousIntermediateEdge = edge;
                            searchIndex += lengthOfMatchInEdge;
                            lengthOfMatchInEdge = 0;
                            break;
                        }
                        if (lengthOfMatchInEdge > edgeSymbolCount)
                        {
                            lengthOfMatchInEdge -= edgeSymbolCount;
                            searchIndex += edgeSymbolCount;

                            long edgeChildCount = edge.Children.Length;

                            querySymbol = convertedSearchSeq[searchIndex];

                            for (int edgeChildIndex = 0; edgeChildIndex < edgeChildCount; edgeChildIndex++)
                            {
                                if (this.referenceSequence[edge.Children[edgeChildIndex].StartIndex] == querySymbol)
                                {
                                    // get the child of edge and continue searching.
                                    previousIntermediateEdge = edge;
                                    edge = edge.Children[edgeChildIndex];
                                    edgeStartIndex = edge.StartIndex;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (lengthOfMatchInEdge > 0)
                    {
                        // lengthOfMatchInEdge > 0 means search is not ending in an intermediate edge or at the endIndex of an edge, 
                        // so no need to continue with the search as there will be missmatch.
                        continue;
                    }
                }

                bool continueSearch = true;

                // start searching for the match by comparing the symbols.
                while (continueSearch)
                {
                    querySymbol = 0;
                    if (searchIndex < querySequenceLength)
                    {
                        querySymbol = convertedSearchSeq[searchIndex];
                    }

                    int edgeIndex = -1;

                    childCount = previousIntermediateEdge.Children.Length;
                    for (int childIndex = 0; childIndex < childCount; childIndex++)
                    {
                        edge = previousIntermediateEdge.Children[childIndex];
                        edgeStartIndex = edge.StartIndex;

                        refSymbol = TerminatingSymbol;

                        if (edgeStartIndex < this.symbolsCount)
                        {
                            refSymbol = this.referenceSequence[edgeStartIndex];
                        }

                        if (refSymbol == querySymbol)
                        {
                            searchIndex++;
                            edgeIndex = childIndex;
                            lengthOfMatchFound++;
                            lengthOfMatchInEdge = 1;
                            break;
                        }
                    }

                    // if edge not found.
                    if (edgeIndex == -1)
                    {
                        // Since the previous edge is an intermediate edge the match is repeated in the reference sequence.
                        // Thus even though the match length is greater than or equal to the MinLengthOfMatch don't consider the match.

                        // Go to the next query index by following the suffix link of the previous intermediate edge.
                        // This will reduce time required for searching from the root. In this case lengthOfMatchFound will be deducted by 1.
                        break;
                    }

                    // Get the endIndex of the edge found.
                    long edgeEndIndex = this.symbolsCount;

                    if (!edge.IsLeaf)
                    {
                        // return the minimum start index of children -1
                        edgeEndIndex = edge.Children[0].StartIndex - 1;
                    }

                    for (long referenceIndex = edgeStartIndex + 1; referenceIndex <= edgeEndIndex; referenceIndex++)
                    {
                        refSymbol = TerminatingSymbol;
                        if (referenceIndex < this.symbolsCount)
                        {
                            refSymbol = this.referenceSequence[referenceIndex];
                        }

                        querySymbol = 0;
                        if (searchIndex < querySequenceLength)
                        {
                            querySymbol = convertedSearchSeq[searchIndex];
                        }

                        // Stop searching if any one of the following conditions is true.
                        // 1. Reached end of the query sequence
                        // 2. Reached end of the leaf edge.
                        // 3. Symbols are not matching
                        if (refSymbol != querySymbol)
                        {
                            break;
                        }

                        searchIndex++;
                        lengthOfMatchFound++;
                        lengthOfMatchInEdge++;
                    }

                    // if it is a leaf node
                    if (edge.IsLeaf)
                    {
                        // if the match length is greater than or equal to the minLengthOfMatch then yield the match.
                        if (lengthOfMatchFound >= minLengthOfMatch
                            && queryIndex + lengthOfMatchFound > lastMatchQueryStart + lastMatchLength)
                        {
                            match = new Match
                                        {
                                            ReferenceSequenceOffset =
                                                edgeStartIndex + lengthOfMatchInEdge - lengthOfMatchFound,
                                            QuerySequenceOffset = queryIndex,
                                            Length = lengthOfMatchFound
                                        };
                            yield return match;

                            if (searchIndex >= querySequenceLength - 1)
                            {
                                // reached the end of the query sequence, no further search needed.
                                continueSearch = false;
                                queryIndex = querySequenceLength;
                                break;
                            }

                            lastMatchLength = lengthOfMatchFound;
                            lastMatchQueryStart = queryIndex;
                        }

                        // go to the next queryIndex
                        continueSearch = false;
                    }
                    else
                    {
                        // if the search is ended 
                        // if the edge is an intermediate node then ignore the match and go to the next queryIndex.
                        if (lengthOfMatchInEdge < (edgeEndIndex - edgeStartIndex + 1))
                        {
                            continueSearch = false;
                        }
                        else
                        {
                            // if the edge is completely searched, then continue with the search.
                            lengthOfMatchInEdge = 0;
                            previousIntermediateEdge = edge;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the matches where length is greater than or equal to the MinLengthOfMatch.
        /// </summary>
        /// <param name="searchSequence">Query sequence to search.</param>
        /// <returns>Returns IEnumerable of matches.</returns>
        public IEnumerable<Match> SearchMatches(ISequence searchSequence)
        {
            // LastQueryEndIndex ->  (LastQueryStartIndex - LastRefStartIndex )-> LastRefEndIndex -> LastRefStartIndex
            var overlappingMatches = new SortedList<long, Dictionary<long, SortedList<long, SortedSet<long>>>>();
            var edgesFound = new Stack<EdgesFound>();
            long minLengthOfMatch = this.MinLengthOfMatch;
            bool noambiguity = this.NoAmbiguity;
            long queryIndex;
            long querySequenceLength = searchSequence.Count;
            long lengthOfMatchFound = 0;

            var match = new Match();

            if (minLengthOfMatch <= 0)
            {
                throw new ArgumentOutOfRangeException(Resource.MinLengthMustBeGreaterThanZero);
            }

            if (!(searchSequence is Sequence))
            {
                throw new ArgumentException(Resource.OnlySequenceClassSupported);
            }

            // Get base alphabet of the searchSequence.
            IAlphabet searchSeqBaseAlphabet = searchSequence.Alphabet;
            IAlphabet alphabet;
            while (Alphabets.AlphabetToBaseAlphabetMap.TryGetValue(searchSeqBaseAlphabet, out alphabet))
            {
                searchSeqBaseAlphabet = alphabet;
            }

            // If base alphabets are not same then throw the exception.
            if (searchSeqBaseAlphabet != this.supportedBaseAlphabet)
            {
                throw new ArgumentException(Resource.AlphabetMisMatch);
            }

            ISequence convertedSearchSeq = ProcessQuerySequence(searchSequence, noambiguity);

            long lengthOfMatchInEdge = 0;
            long edgeStartIndex = 0;
            long childStartIndexToSkip = -1;

            MultiWaySuffixEdge edge = this.rootEdge;
            MultiWaySuffixEdge previousIntermediateEdge = this.rootEdge;

            for (queryIndex = 0; queryIndex <= querySequenceLength - minLengthOfMatch; queryIndex++)
            {
                // if the previousIntermediateEdge is rootEdge then start from the begining.
                if (previousIntermediateEdge.StartIndex == -1 && lengthOfMatchInEdge > 0)
                {
                    lengthOfMatchInEdge--;
                }

                MultiWaySuffixEdge suffixLink = previousIntermediateEdge.SuffixLink[0];
                MultiWaySuffixEdge childEdgePointToParent = previousIntermediateEdge;
                bool suffixLinkPointsToParentEdge = false;

                // Verify whether SuffixLink points to its parent or not.
                if (suffixLink.StartIndex == previousIntermediateEdge.StartIndex - 1
                    && previousIntermediateEdge.SuffixLink[0].StartIndex != -1)
                {
                    int suffixLinkChildCount = suffixLink.Children.Length;

                    for (int suffixLinkChildIndex = 0;
                         suffixLinkChildIndex < suffixLinkChildCount;
                         suffixLinkChildIndex++)
                    {
                        if (suffixLink.Children[suffixLinkChildIndex].Children == previousIntermediateEdge.Children)
                        {
                            suffixLinkPointsToParentEdge = true;
                            edgesFound.Clear();
                            break;
                        }
                    }
                }

                // Go to the next query index by following the suffix link of the previousintermediate edge.
                // This will reduce the searching from the root. In this case lengthOfMatchFound will be deducted by 1.

                // As suffix link always point to another intermediate edge.
                // Note: suffix link for the root is root ifself.
                previousIntermediateEdge = suffixLink;
                lengthOfMatchFound--;

                if (lengthOfMatchFound < 0)
                {
                    lengthOfMatchFound = 0;
                }

                long searchIndex = queryIndex + lengthOfMatchFound - lengthOfMatchInEdge;
                int childCount = previousIntermediateEdge.Children.Length;
                byte refSymbol, querySymbol;

                if (lengthOfMatchInEdge > 0)
                {
                    querySymbol = convertedSearchSeq[searchIndex];
                    for (int index = 0; index < childCount; index++)
                    {
                        edge = previousIntermediateEdge.Children[index];
                        edgeStartIndex = edge.StartIndex;
                        refSymbol = TerminatingSymbol;

                        if (edgeStartIndex < this.symbolsCount)
                        {
                            refSymbol = this.referenceSequence[edgeStartIndex];
                        }

                        if (refSymbol == querySymbol)
                        {
                            break;
                        }
                    }

                    // When lengthOfMatchInEdge >0 there will be an edge from the previousIntermediateEdge.
                    while (!edge.IsLeaf)
                    {
                        long edgeEndIndex = edge.Children[0].StartIndex - 1;

                        // compare the first symbol of the edge.
                        long edgeSymbolCount = edgeEndIndex - edgeStartIndex + 1;
                        if (lengthOfMatchInEdge == edgeSymbolCount)
                        {
                            searchIndex += lengthOfMatchInEdge;

                            if (searchIndex != querySequenceLength)
                            {
                                lengthOfMatchInEdge = 0;
                                previousIntermediateEdge = edge;
                            }

                            break;
                        }
                        if (lengthOfMatchInEdge > edgeSymbolCount)
                        {
                            lengthOfMatchInEdge -= edgeSymbolCount;
                            searchIndex += edgeSymbolCount;

                            long edgeChildCount = edge.Children.Length;

                            querySymbol = convertedSearchSeq[searchIndex];

                            for (int edgeChildIndex = 0; edgeChildIndex < edgeChildCount; edgeChildIndex++)
                            {
                                if (this.referenceSequence[edge.Children[edgeChildIndex].StartIndex] == querySymbol)
                                {
                                    // get the child of edge and continue searching.
                                    previousIntermediateEdge = edge;
                                    edgeStartIndex = edge.Children[edgeChildIndex].StartIndex;
                                    if (lengthOfMatchFound - lengthOfMatchInEdge >= minLengthOfMatch)
                                    {
                                        edgesFound.Push(
                                            new EdgesFound
                                                {
                                                    Edge = previousIntermediateEdge,
                                                    LengthOfMatch = lengthOfMatchFound - lengthOfMatchInEdge
                                                });
                                        childStartIndexToSkip = edgeStartIndex;
                                    }
                                    edge = edge.Children[edgeChildIndex];
                                    break;
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                bool continueSearch = true;

                if (lengthOfMatchInEdge > 0)
                {
                    // no need to continue with search as search is ended inside the edge.
                    continueSearch = false;

                    if (lengthOfMatchFound >= minLengthOfMatch)
                    {
                        // Set -1 so that it wont match with start index of any child edge.
                        edgesFound.Push(new EdgesFound { Edge = edge, LengthOfMatch = lengthOfMatchFound });
                        childStartIndexToSkip = -1;
                    }
                }

                if (queryIndex + lengthOfMatchFound >= querySequenceLength)
                {
                    // no need continue with the seach as entaire query sequence is 
                    // searched and rest of the result can be found using suffix links.
                    continueSearch = false;
                }

                while (continueSearch)
                {
                    querySymbol = 0;
                    if (searchIndex < querySequenceLength)
                    {
                        querySymbol = convertedSearchSeq[searchIndex];
                    }

                    int edgeIndex = -1;

                    childCount = previousIntermediateEdge.Children.Length;
                    for (int childIndex = 0; childIndex < childCount; childIndex++)
                    {
                        edge = previousIntermediateEdge.Children[childIndex];
                        edgeStartIndex = edge.StartIndex;

                        refSymbol = TerminatingSymbol;

                        if (edgeStartIndex < this.symbolsCount)
                        {
                            refSymbol = this.referenceSequence[edgeStartIndex];
                        }

                        if (refSymbol == querySymbol)
                        {
                            edgeIndex = childIndex;
                            break;
                        }
                    }

                    if (edgeIndex == -1)
                    {
                        lengthOfMatchInEdge = 0;
                        continueSearch = false;

                        if (lengthOfMatchFound >= minLengthOfMatch)
                        {
                            // Set -1 so that it wont match with start index of any child edge.
                            edgesFound.Push(
                                new EdgesFound { Edge = previousIntermediateEdge, LengthOfMatch = lengthOfMatchFound });
                            childStartIndexToSkip = -1;
                        }
                    }
                    else
                    {
                        if (lengthOfMatchFound >= minLengthOfMatch)
                        {
                            edgesFound.Push(
                                new EdgesFound { Edge = previousIntermediateEdge, LengthOfMatch = lengthOfMatchFound });
                            childStartIndexToSkip = edge.StartIndex;
                        }

                        searchIndex++;
                        lengthOfMatchFound++;
                        lengthOfMatchInEdge = 1;

                        // Get the endIndex of the edge found.
                        long edgeEndIndex = this.symbolsCount;

                        if (!edge.IsLeaf)
                        {
                            // return the minimum start index of children -1
                            edgeEndIndex = edge.Children[0].StartIndex - 1;
                        }

                        long edgeLength = edgeEndIndex - edgeStartIndex + 1;

                        for (long referenceIndex = edgeStartIndex + 1; referenceIndex <= edgeEndIndex; referenceIndex++)
                        {
                            refSymbol = TerminatingSymbol;
                            if (referenceIndex < this.symbolsCount)
                            {
                                refSymbol = this.referenceSequence[referenceIndex];
                            }

                            querySymbol = 0;
                            if (searchIndex < querySequenceLength)
                            {
                                querySymbol = convertedSearchSeq[searchIndex];
                            }

                            // Stop searching if any one of the following conditions is true.
                            // 1. Reached end of the query sequence
                            // 2. Reached end of the leaf edge.
                            // 3. Symbols are not matching
                            if (refSymbol != querySymbol)
                            {
                                break;
                            }

                            searchIndex++;
                            lengthOfMatchFound++;
                            lengthOfMatchInEdge++;
                        }

                        // Can't continue with search if the following conditions met thus add the edge to the stack.
                        // 1. Edge is a leaf edge regardless of where the search ended.
                        // 2. Edge is an intermediate edge and search ended inside the edge.
                        // 3. searchIndex is equl to the length of the search sequence. (as we increment the searchIndex in advance).
                        if (edge.IsLeaf || lengthOfMatchInEdge < edgeLength || searchIndex == querySequenceLength)
                        {
                            if (lengthOfMatchFound >= minLengthOfMatch)
                            {
                                // Set -1 so that it wont match with start index of any child edge.
                                edgesFound.Push(new EdgesFound { Edge = edge, LengthOfMatch = lengthOfMatchFound });
                                childStartIndexToSkip = -1;
                            }

                            // go to the next queryIndex 
                            continueSearch = false;
                        }
                        else
                        {
                            // if the edge is completly searched and edge is an intemediate edge then continue with the search.
                            previousIntermediateEdge = edge;
                        }
                    }
                }

                // first edge in the stack will be the search ended edge, so process it seperatly.
                if (edgesFound.Count > 0)
                {
                    EdgesFound itemToDisplay = edgesFound.Pop();
                    edge = itemToDisplay.Edge;
                    long matchLength = itemToDisplay.LengthOfMatch;

                    long refIndex;
                    if (edge.IsLeaf)
                    {
                        refIndex = edge.StartIndex + lengthOfMatchInEdge - matchLength;
                        if (ValidateMatch(queryIndex, refIndex, matchLength, overlappingMatches, out match))
                        {
                            yield return match;
                        }
                    }
                    else
                    {
                        childCount = edge.Children.Length;
                        long edgeLength = edge.Children[0].StartIndex - edge.StartIndex;
                        var startIndexes = new List<long>();

                        // suffixLink.Children == edge.Children - reference check to identify the edge having suffix link pointing to its parent.
                        if (suffixLinkPointsToParentEdge && childEdgePointToParent.Children == edge.Children)
                        {
                            startIndexes.Add(edge.StartIndex);
                        }
                        else
                        {
                            for (int childIndex = 0; childIndex < childCount; childIndex++)
                            {
                                if (edge.Children[childIndex].StartIndex == childStartIndexToSkip)
                                {
                                    continue;
                                }

                                DepthFirstIterativeTraversal(edge.Children[childIndex], edgeLength, startIndexes);
                            }

                            startIndexes.Sort();
                        }

                        int listCount = startIndexes.Count;
                        for (int matchIndex = 0; matchIndex < listCount; matchIndex++)
                        {
                            long startIndex = startIndexes[matchIndex];
                            long edgeLengthToAdd = lengthOfMatchInEdge == 0 ? edgeLength : lengthOfMatchInEdge;
                            refIndex = startIndex + edgeLengthToAdd - matchLength;

                            if (ValidateMatch(queryIndex, refIndex, matchLength, overlappingMatches, out match))
                            {
                                yield return match;
                            }
                        }

                        startIndexes.Clear();
                    }

                    // edgesFoundForNextQueryIndex is used for temporary storage and to maintain the order when it pushed to edgesFound stack.
                    var edgesFoundForNextQueryIndex = new Stack<EdgesFound>();

                    EdgesFound previousItemToDisplay = itemToDisplay;

                    // return the output and add the output the list to ignore the outputs that are not required.
                    while (edgesFound.Count > 0)
                    {
                        itemToDisplay = edgesFound.Pop();
                        edge = itemToDisplay.Edge;
                        matchLength = itemToDisplay.LengthOfMatch;

                        if (!edge.IsLeaf && !previousItemToDisplay.Edge.IsLeaf
                            && previousItemToDisplay.Edge.StartIndex
                            != previousItemToDisplay.Edge.SuffixLink[0].StartIndex)
                        {
                            Stack<EdgesFound> tempStack = this.GetIntermediateEdges(
                                edge,
                                previousItemToDisplay.Edge,
                                matchLength,
                                previousItemToDisplay.LengthOfMatch - matchLength,
                                queryIndex + 1,
                                convertedSearchSeq,
                                minLengthOfMatch);
                            if (tempStack.Count > 0)
                            {
                                while (tempStack.Count > 0)
                                {
                                    edgesFoundForNextQueryIndex.Push(tempStack.Pop());
                                }
                            }
                        }

                        childCount = edge.Children.Length;
                        long edgeLength = edge.Children[0].StartIndex - edge.StartIndex;
                        var startIndexes = new List<long>();
                        HashSet<long> overlappingStartIndexes = itemToDisplay.StartIndexesFromPreviousMatchPathEdge;

                        // suffixLink.Children == edge.Children - reference check to identify the edge having suffix link pointing to its parent.
                        if (suffixLinkPointsToParentEdge && childEdgePointToParent.Children == edge.Children)
                        {
                            startIndexes.Add(edge.StartIndex);
                        }
                        else
                        {
                            for (int childIndex = 0; childIndex < childCount; childIndex++)
                            {
                                // if (edge.Children[childIndex].StartIndex == itemToDisplay.ChildStartIndexToSkip)
                                if (edge.Children[childIndex].StartIndex == previousItemToDisplay.Edge.StartIndex)
                                {
                                    continue;
                                }

                                DepthFirstIterativeTraversal(edge.Children[childIndex], edgeLength, startIndexes);
                            }

                            if (overlappingStartIndexes != null)
                            {
                                for (int index = startIndexes.Count - 1; index >= 0; index--)
                                {
                                    if (overlappingStartIndexes.Contains(startIndexes[index]))
                                    {
                                        startIndexes.RemoveAt(index);
                                    }
                                }
                            }

                            startIndexes.Sort();
                        }

                        if (matchLength - 1 >= minLengthOfMatch)
                        {
                            var newEdgeFound = new EdgesFound
                                                   {
                                                       Edge = edge.SuffixLink[0],
                                                       LengthOfMatch = matchLength - 1
                                                   };
                            HashSet<long> overlappingStartIndexesForNextQueryIndex = null;
                            if (edge.StartIndex == edge.SuffixLink[0].StartIndex)
                            {
                                overlappingStartIndexesForNextQueryIndex = new HashSet<long>();
                                if (overlappingStartIndexes != null)
                                {
                                    foreach (long startIndex in overlappingStartIndexes)
                                    {
                                        overlappingStartIndexesForNextQueryIndex.Add(startIndex);
                                    }
                                }

                                for (int index = startIndexes.Count - 1; index >= 0; index--)
                                {
                                    overlappingStartIndexesForNextQueryIndex.Add(startIndexes[index]);
                                }
                            }

                            newEdgeFound.StartIndexesFromPreviousMatchPathEdge =
                                overlappingStartIndexesForNextQueryIndex;

                            // get the suffix link for the edge and add them to the tempstack.
                            edgesFoundForNextQueryIndex.Push(newEdgeFound);
                        }

                        int listCount = startIndexes.Count;
                        for (int matchIndex = 0; matchIndex < listCount; matchIndex++)
                        {
                            long startIndex = startIndexes[matchIndex];
                            refIndex = startIndex + edgeLength - matchLength;

                            if (ValidateMatch(queryIndex, refIndex, matchLength, overlappingMatches, out match))
                            {
                                yield return match;
                            }
                        }

                        startIndexes.Clear();
                        previousItemToDisplay = itemToDisplay;
                    }

                    if (matchLength > minLengthOfMatch && !suffixLinkPointsToParentEdge)
                    {
                        Stack<EdgesFound> tempStack = this.GetIntermediateEdges(
                            this.rootEdge,
                            previousItemToDisplay.Edge,
                            1,
                            previousItemToDisplay.LengthOfMatch,
                            queryIndex + 1,
                            convertedSearchSeq,
                            minLengthOfMatch);
                        while (tempStack.Count > 0)
                        {
                            edgesFoundForNextQueryIndex.Push(tempStack.Pop());
                        }
                    }

                    // push the items in temp stack to the edgesFound stack
                    while (edgesFoundForNextQueryIndex.Count > 0)
                    {
                        edgesFound.Push(edgesFoundForNextQueryIndex.Pop());
                    }
                }
            }
        }

        /// <summary>
        ///     Converts any alias symbols in specified query sequence to its base representation to improve the searching time.
        ///     For example, 'a' to 'A' in case of DNASequence.
        ///     If noambiguity is set then all ambiguous symbols in the sequence are converted to '0'.
        /// </summary>
        /// <param name="searchSequence">Query sequence to process.</param>
        /// <param name="noambiguity">Flag to specify whether to consider ambiguous symbols or not.</param>
        /// <returns>Returns the processed sequence.</returns>
        private static ISequence ProcessQuerySequence(ISequence searchSequence, bool noambiguity)
        {
            HashSet<byte> searchSeqAmbiguousSymbols = searchSequence.Alphabet.GetAmbiguousSymbols();
            byte[] searchSeqSymbolValueMap = searchSequence.Alphabet.GetSymbolValueMap();
            var convertedSymbols = new byte[searchSequence.Count];

            if (!noambiguity)
            {
                for (long index = 0; index < searchSequence.Count; index++)
                {
                    convertedSymbols[index] = searchSeqSymbolValueMap[searchSequence[index]];
                }
            }
            else
            {
                for (long index = 0; index < searchSequence.Count; index++)
                {
                    byte symbol = searchSeqSymbolValueMap[searchSequence[index]];

                    // Set all ambiguous symbols to 0 so that it will not match with any of the reference sequence symbols.
                    if (searchSeqAmbiguousSymbols.Contains(symbol))
                    {
                        symbol = 0;
                    }

                    convertedSymbols[index] = symbol;
                }
            }

            return new Sequence(searchSequence.Alphabet, convertedSymbols, false);
        }

        /// <summary>
        ///     Traverse the suffix tree from the specified Edge and updates the startIndexes list.
        /// </summary>
        /// <param name="current">Edge to start traversing from.</param>
        /// <param name="length">Length of the edge for which the startIndexes are needed.</param>
        /// <param name="startIndexes">List containing the start indexes.</param>
        private static void DepthFirstIterativeTraversal(
            MultiWaySuffixEdge current,
            long length,
            List<long> startIndexes)
        {
            var stack = new Stack<Tuple<MultiWaySuffixEdge, byte, long>>();

            int childIndex = 0;
            if (current.IsLeaf)
            {
                startIndexes.Add(current.StartIndex - length);
            }
            else
            {
                bool done = false;
                while (!done)
                {
                    bool intermediateEdgeFound = false;
                    int count = current.Children.Length;
                    long currentEdgeLength = current.Children[0].StartIndex - current.StartIndex;
                    for (; childIndex < count; childIndex++)
                    {
                        if (current.Children[childIndex].IsLeaf)
                        {
                            startIndexes.Add(current.Children[childIndex].StartIndex - (length + currentEdgeLength));
                        }
                        else
                        {
                            stack.Push(
                                new Tuple<MultiWaySuffixEdge, byte, long>(current, (byte)(childIndex + 1), length));

                            current = current.Children[childIndex];

                            childIndex = 0;
                            length = currentEdgeLength + length;
                            intermediateEdgeFound = true;
                            break;
                        }
                    }

                    if (!intermediateEdgeFound)
                    {
                        if (stack.Count > 0)
                        {
                            Tuple<MultiWaySuffixEdge, byte, long> item = stack.Pop();
                            current = item.Item1;
                            childIndex = item.Item2;
                            length = item.Item3;
                        }
                        else
                        {
                            done = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Builds the suffix tree.
        /// </summary>
        private void BuildSuffixTree()
        {
            int arraySize = this.uniqueSymbolsInReference.Max() + 1;
            var parentRootForSymbol = new MultiWaySuffixEdge[arraySize];

            Parallel.ForEach(
                this.uniqueSymbolsInReference,
                symbol =>
                    {
                        var edge = new MultiWaySuffixEdge();

                        for (long index = this.uniqueSymbolsStartIndexes[symbol]; index < this.symbolsCount; index++)
                        {
                            byte symbolAtIndex = this.referenceSequence[index];

                            if (symbol != symbolAtIndex)
                            {
                                continue;
                            }

                            MultiWaySuffixEdge parent = parentRootForSymbol[symbol];

                            long startIndex = index;
                            MultiWaySuffixEdge[] arrayConainingParent = null;
                            int indexOfArrayContainingParent = -1;
                            bool continueInsert = true;
                            bool duplicatedConsecutiveSymbolsFound = true;

                            do
                            {
                                byte symbolAtStartIndex = TerminatingSymbol;
                                if (startIndex < this.symbolsCount)
                                {
                                    symbolAtStartIndex = this.referenceSequence[startIndex];
                                }

                                int indexOfEdgeFound = -1;
                                int childCount = 0;
                                if (!parent.IsLeaf)
                                {
                                    // Find edge start
                                    childCount = parent.Children.Length;
                                    for (int i = 0; i < childCount; i++)
                                    {
                                        MultiWaySuffixEdge childEdge = parent.Children[i];
                                        if (childEdge.StartIndex < this.symbolsCount)
                                        {
                                            byte edgeSymbol = this.referenceSequence[childEdge.StartIndex];
                                            if (edgeSymbol == symbolAtStartIndex)
                                            {
                                                edge = childEdge;
                                                indexOfEdgeFound = i;
                                                startIndex++;
                                                if (edgeSymbol != symbolAtIndex)
                                                {
                                                    duplicatedConsecutiveSymbolsFound = false;
                                                }

                                                break;
                                            }
                                        }
                                    }
                                }

                                MultiWaySuffixEdge newEdge;
                                if (indexOfEdgeFound == -1)
                                {
                                    // Insert new child
                                    newEdge = new MultiWaySuffixEdge(startIndex);

                                    Array.Resize(ref parent.Children, childCount + 1);

                                    parent.Children[childCount] = newEdge;
                                    parent.SuffixLink = new MultiWaySuffixEdge[1];
                                    Interlocked.Increment(ref this.edgesCount);

                                    // Assign back modified edge.
                                    if (arrayConainingParent == null)
                                    {
                                        parentRootForSymbol[symbol] = parent;
                                    }
                                    else
                                    {
                                        arrayConainingParent[indexOfArrayContainingParent] = parent;
                                    }

                                    continueInsert = false;
                                    break;
                                }
                                long edgeEndIndex = this.symbolsCount;

                                if (!edge.IsLeaf)
                                {
                                    // return the minimum start index of children -1
                                    edgeEndIndex = edge.Children[0].StartIndex - 1;
                                }

                                // Do not enter if only one symbol is there in the edge.
                                if (edge.StartIndex < edgeEndIndex)
                                {
                                    long duplicatedConsicutiveSymbolsCount = 0;

                                    for (long counter = edge.StartIndex + 1;
                                         counter <= edgeEndIndex;
                                         counter++, startIndex++)
                                    {
                                        symbolAtStartIndex = TerminatingSymbol;
                                        if (startIndex < this.symbolsCount)
                                        {
                                            symbolAtStartIndex = this.referenceSequence[startIndex];
                                        }

                                        byte symbolAtCounter = TerminatingSymbol;
                                        if (counter < this.symbolsCount)
                                        {
                                            symbolAtCounter = this.referenceSequence[counter];
                                        }

                                        if (symbolAtStartIndex != symbolAtCounter)
                                        {
                                            // Split the edge
                                            // Create the new edge
                                            // Copy the children of old edge to new edge
                                            newEdge = new MultiWaySuffixEdge(counter)
                                                          {
                                                              Children = edge.Children,
                                                              SuffixLink = edge.SuffixLink
                                                          };

                                            edge.Children = new MultiWaySuffixEdge[2]; // for split edge and leaf edge.

                                            // As this is an internal node allocate the array here itself to avoid updating 
                                            // the parent array with the new address of the edge (as MultiWaySuffixEdge is a value type).
                                            edge.SuffixLink = new MultiWaySuffixEdge[1];
                                            edge.SuffixLink[0].StartIndex = -1;

                                            // Create leaf edge.
                                            var leafEdge = new MultiWaySuffixEdge(startIndex);

                                            if (duplicatedConsecutiveSymbolsFound
                                                && duplicatedConsicutiveSymbolsCount > 1)
                                            {
                                                for (int duplicatedIndex = 1;
                                                     duplicatedIndex < duplicatedConsicutiveSymbolsCount;
                                                     duplicatedIndex++)
                                                {
                                                    var duplicateSymbolEdge =
                                                        new MultiWaySuffixEdge(newEdge.StartIndex - 1);
                                                    duplicateSymbolEdge.Children = new MultiWaySuffixEdge[2];
                                                    duplicateSymbolEdge.SuffixLink = new MultiWaySuffixEdge[1];
                                                    duplicateSymbolEdge.SuffixLink[0].StartIndex = -1;

                                                    duplicateSymbolEdge.Children[0] = newEdge;
                                                    duplicateSymbolEdge.Children[1] = leafEdge;

                                                    // we are adding two edges here - duplicatesymbol edge and leaf edge.
                                                    // leaf edge will be duplicated.
                                                    Interlocked.Increment(ref this.edgesCount);
                                                    Interlocked.Increment(ref this.edgesCount);

                                                    newEdge = duplicateSymbolEdge;
                                                }

                                                index += duplicatedConsicutiveSymbolsCount - 1;
                                            }

                                            // Update the old edge

                                            // Set new edge as child edge to old edge
                                            edge.Children[0] = newEdge;

                                            // Add the leaf edge.
                                            edge.Children[1] = leafEdge;
                                            Interlocked.Increment(ref this.edgesCount);
                                            Interlocked.Increment(ref this.edgesCount);

                                            // assign back edge that got modified.
                                            parent.Children[indexOfEdgeFound] = edge;

                                            continueInsert = false;
                                            duplicatedConsecutiveSymbolsFound = false;
                                            break;
                                        }

                                        if (duplicatedConsecutiveSymbolsFound)
                                        {
                                            duplicatedConsicutiveSymbolsCount++;

                                            if (symbolAtIndex != symbolAtStartIndex)
                                            {
                                                duplicatedConsecutiveSymbolsFound = false;
                                            }
                                        }
                                    }
                                }

                                if (continueInsert)
                                {
                                    arrayConainingParent = parent.Children;
                                    indexOfArrayContainingParent = indexOfEdgeFound;
                                    parent = edge;
                                }
                            }
                            while ((startIndex <= this.symbolsCount) && continueInsert);
                        }
                    });

            this.rootEdge.StartIndex = -1;
            int rootChildrenCount = this.uniqueSymbolsInReference.Count + 1;

            Array.Resize(ref this.rootEdge.Children, rootChildrenCount);

            int rootChildIndex = 0;

            // Add all symbol root's child to the rootEdge.
            foreach (byte symbol in this.uniqueSymbolsInReference)
            {
                this.rootEdge.Children[rootChildIndex] = parentRootForSymbol[symbol].Children[0];
                rootChildIndex++;
            }

            // Add edge for $.
            this.rootEdge.Children[rootChildrenCount - 1] = new MultiWaySuffixEdge(this.symbolsCount);
            this.edgesCount++;
        }

        /// <summary>
        ///     Gets the intermediate edges present in the path of the match between specified edges for the next query index to
        ///     match.
        /// </summary>
        /// <param name="fromEdge">Edge from the which to search from.</param>
        /// <param name="toedge">Edge where to stop the search.</param>
        /// <param name="matchLengthOfFromEdge">Matching symbols count of the fromEdge.</param>
        /// <param name="lengthToSearch">Length to search.</param>
        /// <param name="nextQueryIndex">Next query index.</param>
        /// <param name="convertedSearchSeq">Converted search sequence.</param>
        /// <param name="minLengthOfMatch">Minimum length of match required.</param>
        /// <returns>Returns the intermediate edges found between the fromEdge to toEdge.</returns>
        private Stack<EdgesFound> GetIntermediateEdges(
            MultiWaySuffixEdge fromEdge,
            MultiWaySuffixEdge toedge,
            long matchLengthOfFromEdge,
            long lengthToSearch,
            long nextQueryIndex,
            ISequence convertedSearchSeq,
            long minLengthOfMatch)
        {
            var edgesFoundForNextQueryIndex = new Stack<EdgesFound>();
            var edge = new MultiWaySuffixEdge();
            long edgeStartIndex = 0;

            if (toedge.IsLeaf || fromEdge.IsLeaf)
            {
                return edgesFoundForNextQueryIndex;
            }

            matchLengthOfFromEdge--;
            MultiWaySuffixEdge previousIntermediateEdge = fromEdge.SuffixLink[0];
            long childIndexToStop = toedge.SuffixLink[0].StartIndex;
            long searchIndex = nextQueryIndex + matchLengthOfFromEdge;
            int childCount = previousIntermediateEdge.Children.Length;

            // if the previousIntermediateEdge is rootEdge.
            if (previousIntermediateEdge.StartIndex == -1 && lengthToSearch > 0)
            {
                lengthToSearch--;
            }

            if (lengthToSearch > 0)
            {
                byte querySymbol = convertedSearchSeq[searchIndex];
                for (int index = 0; index < childCount; index++)
                {
                    edge = previousIntermediateEdge.Children[index];

                    edgeStartIndex = edge.StartIndex;

                    byte refSymbol = edgeStartIndex < this.symbolsCount
                                         ? this.referenceSequence[edgeStartIndex]
                                         : TerminatingSymbol;
                    if (refSymbol == querySymbol)
                    {
                        break;
                    }
                }

                // When lengthOfMatchInEdge >0 there will be an edge from the previousIntermediateEdge.
                while (!edge.IsLeaf && edge.StartIndex != childIndexToStop)
                {
                    long edgeEndIndex = edge.Children[0].StartIndex - 1;

                    // compare the first symbol of the edge.
                    long edgeSymbolCount = edgeEndIndex - edgeStartIndex + 1;
                    if (lengthToSearch == edgeSymbolCount)
                    {
                        searchIndex += lengthToSearch;
                        lengthToSearch = 0;
                        previousIntermediateEdge = edge;
                        break;
                    }
                    if (lengthToSearch > edgeSymbolCount)
                    {
                        lengthToSearch -= edgeSymbolCount;
                        searchIndex += edgeSymbolCount;
                        matchLengthOfFromEdge += edgeSymbolCount;
                        long edgeChildCount = edge.Children.Length;

                        querySymbol = convertedSearchSeq[searchIndex];

                        for (int edgeChildIndex = 0; edgeChildIndex < edgeChildCount; edgeChildIndex++)
                        {
                            if (this.referenceSequence[edge.Children[edgeChildIndex].StartIndex] == querySymbol)
                            {
                                // get the child of edge and continue searching.
                                previousIntermediateEdge = edge;
                                if (matchLengthOfFromEdge >= minLengthOfMatch)
                                {
                                    edgesFoundForNextQueryIndex.Push(
                                        new EdgesFound
                                            {
                                                Edge = previousIntermediateEdge,
                                                LengthOfMatch = matchLengthOfFromEdge
                                            });
                                }

                                edge = edge.Children[edgeChildIndex];
                                edgeStartIndex = edge.StartIndex;
                                break;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return edgesFoundForNextQueryIndex;
        }

        /// <summary>
        ///     Updates the suffix links for the suffix tree.
        /// </summary>
        private void UpdateSuffixLinks()
        {
            this.rootEdge.SuffixLink = new MultiWaySuffixEdge[1];
            this.rootEdge.SuffixLink[0] = this.rootEdge;

            int rootChildCount = 0;
            var edges = new List<EdgesToLink>();
            if (!this.rootEdge.IsLeaf)
            {
                rootChildCount = this.rootEdge.Children.Length;
            }

            for (int childIndex = 0; childIndex < rootChildCount; childIndex++)
            {
                MultiWaySuffixEdge childEdge = this.rootEdge.Children[childIndex];
                if (!childEdge.IsLeaf)
                {
                    this.UpdateSuffixLinkForChildOfRoot(childIndex);

                    int childCount = childEdge.Children.Length;

                    for (int index = 0; index < childCount; index++)
                    {
                        if (!childEdge.Children[index].IsLeaf)
                        {
                            edges.Add(new EdgesToLink { ParentEdge = childEdge, ChildIndex = index });
                        }
                    }
                }
            }

            Parallel.ForEach(
                edges,
                currentEdgeToLink =>
                    {
                        var edgesToLink = new Stack<EdgesToLink>();
                        edgesToLink.Push(currentEdgeToLink);
                        while (edgesToLink.Count > 0)
                        {
                            EdgesToLink edgeToLink = edgesToLink.Pop();

                            MultiWaySuffixEdge parentEdge = edgeToLink.ParentEdge;
                            int index = edgeToLink.ChildIndex;

                            this.UpdateSuffixLinkForEdge(parentEdge, index);

                            MultiWaySuffixEdge edge = parentEdge.Children[index];
                            int childCount = edge.Children.Length;
                            for (int childIndex = 0; childIndex < childCount; childIndex++)
                            {
                                if (!(edge.Children[childIndex].IsLeaf))
                                {
                                    edgesToLink.Push(new EdgesToLink { ParentEdge = edge, ChildIndex = childIndex });
                                }
                            }
                        }
                    });
        }

        /// <summary>
        ///     Updates the suffix links for the children of the root.
        /// </summary>
        /// <param name="childIndex">Child index of the root to update.</param>
        private void UpdateSuffixLinkForChildOfRoot(int childIndex)
        {
            MultiWaySuffixEdge childEdge = this.rootEdge.Children[childIndex];

            // if the child is a leaf then no suffix link needed.
            if (childEdge.IsLeaf)
            {
                return;
            }

            long childStartIndex = childEdge.StartIndex;
            long childEndIndex = childEdge.Children[0].StartIndex - 1;
            long childSymbolCount = childEndIndex - childStartIndex + 1;

            // if only one symbols is present in the immediate child of the root then root is the suffix link of that child.
            if (childStartIndex == childEndIndex)
            {
                childEdge.SuffixLink[0] = this.rootEdge;
                return;
            }

            // if first and second symbol of the childEdge are same then edge itself will be the suffix link.
            // Example, a child edge containing symbols AA.
            if (this.referenceSequence[childStartIndex] == this.referenceSequence[childStartIndex + 1])
            {
                // this scenario will not apprear at the root.
                childEdge.SuffixLink[0] = this.rootEdge;
                return;
            }

            // exclude the first symbol.
            childStartIndex++;
            childSymbolCount--;

            int childCount = this.rootEdge.Children.Length;
            byte symbol = this.referenceSequence[childStartIndex];
            for (int index = 0; index < childCount; index++)
            {
                MultiWaySuffixEdge edge = this.rootEdge.Children[index];

                // SuffixLinks will point to another intermediate edges only.
                if (edge.IsLeaf)
                {
                    continue;
                }

                long edgeStartIndex = edge.StartIndex;

                if (this.referenceSequence[edgeStartIndex] == symbol)
                {
                    while (true)
                    {
                        long edgeEndIndex = edge.Children[0].StartIndex - 1;

                        // compare the first symbol of the edge.
                        long edgeSymbolCount = edgeEndIndex - edgeStartIndex + 1;
                        if (childSymbolCount == edgeSymbolCount)
                        {
                            childEdge.SuffixLink[0] = edge;
                            return;
                        }
                        childSymbolCount = childSymbolCount - edgeSymbolCount;
                        childStartIndex += edgeSymbolCount;

                        long edgeChildCount = edge.Children.Length;
                        symbol = this.referenceSequence[childStartIndex];
                        for (int edgeChildIndex = 0; edgeChildIndex < edgeChildCount; edgeChildIndex++)
                        {
                            if (this.referenceSequence[edge.Children[edgeChildIndex].StartIndex] == symbol)
                            {
                                // get the child of edge and continue searching.
                                edge = edge.Children[edgeChildIndex];
                                edgeStartIndex = edge.StartIndex;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Updates the suffix link of a child edge of specified edge.
        /// </summary>
        /// <param name="parenetEdge">Parent edge.</param>
        /// <param name="childIndex">Index of the child to update.</param>
        private void UpdateSuffixLinkForEdge(MultiWaySuffixEdge parenetEdge, int childIndex)
        {
            MultiWaySuffixEdge childEdge = parenetEdge.Children[childIndex];

            long childStartIndex = childEdge.StartIndex;
            long childEndIndex = childEdge.Children[0].StartIndex - 1;
            long childSymbolCount = childEndIndex - childStartIndex + 1;

            MultiWaySuffixEdge parentSuffixLink = parenetEdge.SuffixLink[0];
            int childCount = parentSuffixLink.Children.Length;
            byte symbol = this.referenceSequence[childStartIndex];
            for (int index = 0; index < childCount; index++)
            {
                MultiWaySuffixEdge edge = parentSuffixLink.Children[index];

                // SuffixLinks will point to another intermediate edges only.
                if (edge.IsLeaf)
                {
                    continue;
                }

                long edgeStartIndex = edge.StartIndex;

                if (this.referenceSequence[edgeStartIndex] == symbol)
                {
                    while (true)
                    {
                        long edgeEndIndex = edge.Children[0].StartIndex - 1;

                        // compare the first symbol of the edge.
                        long edgeSymbolCount = edgeEndIndex - edgeStartIndex + 1;
                        if (childSymbolCount == edgeSymbolCount)
                        {
                            childEdge.SuffixLink[0] = edge;
                            return;
                        }
                        childSymbolCount = childSymbolCount - edgeSymbolCount;
                        childStartIndex += edgeSymbolCount;

                        long edgeChildCount = edge.Children.Length;
                        symbol = this.referenceSequence[childStartIndex];
                        for (int edgeChildIndex = 0; edgeChildIndex < edgeChildCount; edgeChildIndex++)
                        {
                            if (this.referenceSequence[edge.Children[edgeChildIndex].StartIndex] == symbol)
                            {
                                // get the child of edge and continue searching.
                                edge = edge.Children[edgeChildIndex];
                                edgeStartIndex = edge.StartIndex;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Validates whether new match is an exact sub match with any of the previous matches if not then returns the match in
        ///     out param.
        /// </summary>
        /// <param name="queryIndex">Query index</param>
        /// <param name="referenceIndex">Reference index</param>
        /// <param name="matchLength">Match length</param>
        /// <param name="previousMatches">Previous matches</param>
        /// <param name="match">New match</param>
        /// <returns>Returns true if the new match is not an exact sub match with any of the previous matches, else returns false</returns>
        private static bool ValidateMatch(
            long queryIndex,
            long referenceIndex,
            long matchLength,
            SortedList<long, Dictionary<long, SortedList<long, SortedSet<long>>>> previousMatches,
            out Match match)
        {
            bool isoverlapedMatchFound = false;

            long lastQueryEndIndex;
            int overlappingMatchesCount = previousMatches.Keys.Count();
            if (overlappingMatchesCount > 0)
            {
                lastQueryEndIndex = previousMatches.Keys.Last();
                if (lastQueryEndIndex < queryIndex)
                {
                    previousMatches.Clear();
                }
            }

            overlappingMatchesCount = previousMatches.Keys.Count();

            for (int listIndex = overlappingMatchesCount - 1; listIndex >= 0; listIndex--)
            {
                lastQueryEndIndex = previousMatches.Keys[listIndex];
                if (lastQueryEndIndex >= queryIndex + matchLength)
                {
                    Dictionary<long, SortedList<long, SortedSet<long>>> diffMap = previousMatches[lastQueryEndIndex];
                    SortedList<long, SortedSet<long>> refEndIndexMap;
                    if (diffMap.TryGetValue(queryIndex - referenceIndex, out refEndIndexMap))
                    {
                        int refEndIndexCount = refEndIndexMap.Count;
                        for (int refEndMapIndex = refEndIndexCount - 1; refEndMapIndex >= 0; refEndMapIndex--)
                        {
                            long refEndindex = refEndIndexMap.Keys[refEndMapIndex];

                            if (refEndindex >= referenceIndex + matchLength)
                            {
                                SortedSet<long> refStartIndexes = refEndIndexMap[refEndindex];
                                isoverlapedMatchFound =
                                    refStartIndexes.Any(refStartIndex => refStartIndex <= referenceIndex);
                                if (isoverlapedMatchFound)
                                {
                                    break;
                                }
                            }
                        }

                        if (isoverlapedMatchFound)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    if (lastQueryEndIndex < queryIndex)
                    {
                        previousMatches.Remove(lastQueryEndIndex);
                    }

                    break;
                }
            }

            match = new Match();

            if (!isoverlapedMatchFound)
            {
                match.ReferenceSequenceOffset = referenceIndex;
                match.QuerySequenceOffset = queryIndex;
                match.Length = matchLength;
                long queryEndIndex = queryIndex + matchLength;
                long diffValue = queryIndex - referenceIndex;
                long refEndIndex = referenceIndex + matchLength;
                Dictionary<long, SortedList<long, SortedSet<long>>> diffsMap;
                SortedList<long, SortedSet<long>> refEndIndexMap;
                SortedSet<long> refStartIndexes;
                if (previousMatches.TryGetValue(queryEndIndex, out diffsMap))
                {
                    if (diffsMap.TryGetValue(diffValue, out refEndIndexMap))
                    {
                        if (refEndIndexMap.TryGetValue(refEndIndex, out refStartIndexes))
                        {
                            refStartIndexes.Add(referenceIndex);
                        }
                        else
                        {
                            refStartIndexes = new SortedSet<long>();
                            refStartIndexes.Add(referenceIndex);
                            refEndIndexMap.Add(refEndIndex, refStartIndexes);
                        }
                    }
                    else
                    {
                        refEndIndexMap = new SortedList<long, SortedSet<long>>();
                        refStartIndexes = new SortedSet<long>();
                        refStartIndexes.Add(referenceIndex);
                        refEndIndexMap.Add(refEndIndex, refStartIndexes);
                        diffsMap.Add(diffValue, refEndIndexMap);
                    }
                }
                else
                {
                    diffsMap = new Dictionary<long, SortedList<long, SortedSet<long>>>();
                    refEndIndexMap = new SortedList<long, SortedSet<long>>();
                    refStartIndexes = new SortedSet<long>();
                    refStartIndexes.Add(referenceIndex);
                    refEndIndexMap.Add(refEndIndex, refStartIndexes);
                    diffsMap.Add(diffValue, refEndIndexMap);
                    previousMatches.Add(queryEndIndex, diffsMap);
                }
            }

            return !isoverlapedMatchFound;
        }

        /// <summary>
        ///     Structure to hold the edge and length of match.
        ///     Used in the MaxMatch.
        /// </summary>
        [DebuggerDisplay("(StartIndex= {Edge.StartIndex}, IsLeaf= {Edge.IsLeaf}), LengthOfMatch = {LengthOfMatch}")]
        private struct EdgesFound
        {
            /// <summary>
            ///     Edge which contains the symbols which matches the query sequence.
            /// </summary>
            public MultiWaySuffixEdge Edge;

            /// <summary>
            ///     Length of the symbols from the root.
            /// </summary>
            public long LengthOfMatch;

            /// <summary>
            ///     Holds the strat indexes of the edge in the previous match path.
            /// </summary>
            public HashSet<long> StartIndexesFromPreviousMatchPathEdge;
        }

        /// <summary>
        ///     Structure to hold Edges to link.
        /// </summary>
        private struct EdgesToLink
        {
            /// <summary>
            ///     Index of child to link.
            /// </summary>
            public int ChildIndex;

            /// <summary>
            ///     Parent edge.
            /// </summary>
            public MultiWaySuffixEdge ParentEdge;
        }
    }
}