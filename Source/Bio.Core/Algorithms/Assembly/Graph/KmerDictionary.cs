using System;
using System.Collections.Generic;
using System.Linq;
using Bio.Algorithms.Kmer;
using Bio.Util;

namespace Bio.Algorithms.Assembly.Graph
{
    /// <summary>
    /// Implements a thread safe K-mer dictionary (12 &lt; k &lt; 31) for storing values associated with ulong k-mers.
    /// Backend is an array of binary search trees
    /// </summary>
    public class KmerDictionary
    {
        /// <summary>
        /// How many bits of the ulong to use to decide which bucket is inserted where?
        /// Also determines the number of buckets as 2^hashLength,
        /// </summary>
        private const int HashLength = 12;

        /// <summary>
        /// Each bucket stores a Tree that can be searched.
        /// </summary>
        private readonly BinaryTreeOfDebrujinNodes[] _buckets;

        /// <summary>
        /// Mask to hash off higher bits which can be used to get the bucket assignment of a k-mer
        /// </summary>
        private readonly ulong _hashingMask;

        /// <summary>
        /// Creates a new dictionary to store and search for deBruijin Nodes
        /// </summary>
        public KmerDictionary()
        {
            var maxSize = (ulong) Math.Pow(2, HashLength);
            
            // Create a list of buckets up to that size
            _buckets = new BinaryTreeOfDebrujinNodes[maxSize];
            
            // Create a new tree in each bucket position 
            Enumerable.Range(0, (int) maxSize).ForEach(x => _buckets[x] = new BinaryTreeOfDebrujinNodes());
            
            // Make a mask for incoming bits
            _hashingMask = maxSize - 1; // This should be all bits up to the length of the buckets array.
        }

        /// <summary>
        /// Number of kmers in the dictionary
        /// </summary>
        public long NodeCount
        {
            get { return _buckets.Select(x => x.Count).Sum(); }
        }

        /// <summary>
        /// Either returns the DeBrujin node associated with the ulong, or
        /// sets it if an old one does not exist
        /// Parallel Note: Is thread safe
        /// </summary>
        /// <returns>The node representing this value</returns>
        public DeBruijnNode SetNewOrGetOld(KmerData32 value)
        {
            int bucket = AssignBucket(value);
            BinaryTreeOfDebrujinNodes curBucket = _buckets[bucket];

            //keep it thread safe for additions
            DeBruijnNode toReturn;
            lock (curBucket)
            {
                toReturn = curBucket.AddOrReturnCurrent(value);
            }
            return toReturn;
        }

        /// <summary>
        /// Enumerate through tree returning an array while removing references to left/right children
        /// so they can become available to GC if not otherwise referenced.
        /// </summary>
        /// <returns>Set of nodes</returns>
        private IEnumerable<DeBruijnNode> GetNodes()
        {
            return _buckets.SelectMany(tree => tree.GetNodes());
        }

        /// <summary>
        /// Returns a node for a given k-mer
        /// </summary>
        /// <param name="kmer">The kmer</param>
        /// <returns>true if the item has previously been assigned a serial number; otherwise, false.</returns>
        public DeBruijnNode TryGetOld(KmerData32 kmer)
        {
            int bucketIndex = AssignBucket(kmer);
            BinaryTreeOfDebrujinNodes tree = _buckets[bucketIndex];
            return tree.SearchTree(kmer);
        }

        /// <summary>
        /// Assign a k-mer encoded as a ulong to a bucket
        /// </summary>
        /// <param name="value">kmer value</param>
        /// <returns>bucket index</returns>
        private int AssignBucket(KmerData32 value)
        {
            return (int) (value.KmerData & _hashingMask);
        }

        /// <summary>
        /// Converts the nodes in the kmer manager into an array
        /// in order to improve performance and simultaneously destroys tree data structure.
        /// </summary>
        public IEnumerable<DeBruijnNode> GenerateNodeArray()
        {
            IEnumerable<DeBruijnNode> nodes;
            long maxSize = PlatformManager.Services.Is64BitProcessType ? int.MaxValue - 64 : int.MaxValue/4;

            if (NodeCount > maxSize)
            {
                nodes = new BigList<DeBruijnNode>(GetNodes(), NodeCount);
            }
            else
            {
                var tnodes = new List<DeBruijnNode>((int) NodeCount);
                tnodes.AddRange(GetNodes());
                nodes = tnodes;
            }
            return nodes;
        }
    }
}