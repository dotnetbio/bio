using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bio.Algorithms.Kmer;
using Bio.Util;
using System.Diagnostics;
using System.Globalization;
using Bio.Algorithms.Assembly.Graph;

namespace Bio.Algorithms.Assembly
{
    /// <summary>
    /// Implements a thread safe K-mer dictionary (12 &lt; k &lt; 31) for storing values associated with ulong k-mers.
    /// Backend is an array of binary search trees
    /// </summary>
    public class KmerDictionary
    {
        /// <summary>
        /// Each bucket stores a Tree that can be searched.
        /// </summary>
        BinaryTreeOfDebrujinNodes[] buckets;
        
        /// <summary>
        /// How many bits of the ulong to use to decide which bucket is inserted where? 
        /// Also determines the number of buckets as 2^hashLength,
        /// </summary>
        const int hashLength=12;

        /// <summary>
        /// Mask to hash off higher bits which can be used to get the bucket assignment of a k-mer
        /// </summary>
        ulong hashingMask;
        
        /// <summary>
        /// Number of kmers in the dictionary 
        /// </summary>
        public long NodeCount
        {
            get
            {
                return buckets.Select(x => x.Count).Sum();
            }
        }

        /// <summary>
        /// Creates a new dictionary to store and search for deBruijin Nodes
        /// </summary>
        public KmerDictionary()
        {
            ulong maxSize = (ulong)Math.Pow(2, hashLength);
            //now create a list of buckets up to that size
            buckets = new BinaryTreeOfDebrujinNodes[maxSize];
            //Create a new tree in each bucket position 
            Enumerable.Range(0,(int)maxSize).ForEach(x=> buckets[x]=new BinaryTreeOfDebrujinNodes());
            //now make a mask for incoming bits
            hashingMask = maxSize - 1; //This should be all bits up to the length of the buckets array.
        }
        /// <summary>
        /// Either returns the DeBrujin node associated with the ulong, or
        /// sets it if an old one does not exist
        /// 
        /// Parallel Note: Is thread safe
        /// </summary>
        /// <returns>The node representing this value</returns>
        public DeBruijnNode SetNewOrGetOld(KmerData32 value)
        {
            int bucket = assignBucket(value);
            BinaryTreeOfDebrujinNodes curBucket = buckets[bucket];
            
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
        /// <returns></returns>
        private IEnumerable<DeBruijnNode> GetNodes()
        {           
            foreach (var tree in buckets)
            {
                foreach (var node in tree.GetNodes())
                {                    
                    yield return node;
                }
            }
        }

        /// <summary>
        /// Returns a node for a given k-mer
        /// </summary>
        /// <param name="kmer">The kmer</param>
        /// <returns>true if the item has previously been assigned a serial number; otherwise, false.</returns>
        public DeBruijnNode TryGetOld(KmerData32 kmer)
        {
            int bucketIndex = assignBucket(kmer);
            var tree = buckets[bucketIndex];
            return tree.SearchTree(kmer);
        }

        /// <summary>
        /// Assign a k-mer encoded as a ulong to a bucket
        /// </summary>
        /// <param name="value">kmer value</param>
        /// <returns>bucket index</returns>
        private int assignBucket(KmerData32 value)
        {
            //This should be inlined by the JIT, only writing this way for clarity
            return (int)(value.KmerData & hashingMask);
        }

        /// <summary>
        /// Converts the nodes in the kmer manager into an array 
        /// in order to improve performance and simultaneously destroys tree data structure.
        /// </summary>
        public IEnumerable<DeBruijnNode> GenerateNodeArray()
        {
            IEnumerable<DeBruijnNode> nodes;
            long maxSize = int.MaxValue - 64;//assuming objects in .NET 4.5 can be >2 GB, we are only limited by size of array.
            //arrays are indexed by int values (thanks to Mark for pointing that out) so can be up to the max of an int type. 
            if (!Environment.Is64BitProcess)
            {
                maxSize = maxSize / 4; //I believe most that can be held by array is 2GB so dropping this down
            }
            if (NodeCount > maxSize)
            {
                //too many nodes for a simple array
                nodes = new BigList<DeBruijnNode>(GetNodes(), NodeCount);
            }
            else
            {
                var tnodes = new List<DeBruijnNode>((int)NodeCount);
                tnodes.AddRange(GetNodes());
                nodes = tnodes;
            }
            return nodes;
        }

        }
}
