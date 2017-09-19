using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Bio.Algorithms.Assembly.Padena.Scaffold
{
    /// <summary>
    /// Filter mate pairs based on support for contig orientation.
    /// The mate pairs support specific orientation of contigs, 
    /// based on mapping of reverse read or forward read to specify orientation.
    /// Orientation 1
    /// ----------) (------------- 
    /// contig 1      contig 2
    /// Orientation 2
    /// ----------) (-------------
    /// contig 2      contig 1.
    /// </summary>
    public class OrientationBasedMatePairFilter : IOrientationBasedMatePairFilter
    {
        /// <summary>
        /// Filter Paired Read Based on Orientation.
        /// </summary>
        /// <param name="matePairMap">Map between contigs using mate pair information.</param>
        /// <param name="redundancy">Number of mate pairs required to create a link between two contigs.
        ///  Hierarchical Scaffolding With Bambus
        ///  by: Mihai Pop, Daniel S. Kosack, Steven L. Salzberg 
        /// Genome Research, Vol. 14, No. 1. (January 2004), pp. 149-159.</param>  
        public ContigMatePairs FilterPairedReads(ContigMatePairs matePairMap, int redundancy = 2)
        {
            if (null == matePairMap)
            {
                throw new ArgumentNullException("matePairMap");
            }

            if (redundancy < 0)
            {
                throw new ArgumentException(Properties.Resource.NegativeRedundancy);
            }

            foreach (KeyValuePair<ISequence, Dictionary<ISequence, IList<ValidMatePair>>> matePair in matePairMap)
            {
                foreach (KeyValuePair<ISequence, IList<ValidMatePair>> validMatePair in matePair.Value)
                {
                    if (matePair.Key != validMatePair.Key)
                    {
                        Dictionary<ISequence, IList<ValidMatePair>> validMatePairs;
                        if (matePairMap.TryGetValue(validMatePair.Key, out validMatePairs))
                        {
                            IList<ValidMatePair> pair;
                            if (validMatePairs.TryGetValue(matePair.Key, out pair))
                            {
                                OrientationFilter(pair, validMatePair.Value, redundancy);
                            }
                            else
                            {
                                if (validMatePair.Value.Count < redundancy)
                                {
                                    validMatePair.Value.Clear();
                                }
                            }
                        }
                        else
                        {
                            if (validMatePair.Value.Count < redundancy)
                            {
                                validMatePair.Value.Clear();
                            }
                        }
                    }
                    else
                    {
                        validMatePair.Value.Clear();
                    }
                }
            }

            ContigMatePairs newMatePairMap = new ContigMatePairs();
            Parallel.ForEach(matePairMap, matePair =>
            {
                var map = matePair.Value
                    .Where(validMatePair => validMatePair.Value.Count > 0)
                    .ToDictionary(validMatePair => validMatePair.Key, validMatePair => validMatePair.Value);

                if (map.Count > 0)
                {
                    lock (newMatePairMap)
                    {
                        newMatePairMap.Add(matePair.Key, map);
                    }
                }
            });

            return newMatePairMap;
        }

        private static void OrientationFilter(IList<ValidMatePair> validPair1, IList<ValidMatePair> validPair2, int redundancy)
        {
            if (validPair1.Count > validPair2.Count)
            {
                if (validPair1.Count < redundancy)
                {
                    validPair1.Clear();
                }

                validPair2.Clear();
            }
            else
            {
                if (validPair1.Count != validPair2.Count)
                {
                    if (validPair2.Count < redundancy)
                    {
                        validPair2.Clear();
                    }

                    validPair1.Clear();
                }
                else
                {
                    foreach (ValidMatePair validPair in validPair2)
                    {
                        validPair1.Add(validPair);
                    }

                    if (validPair1.Count <= redundancy)
                    {
                        validPair1.Clear();
                    }

                    validPair2.Clear();
                }
            }
        }
    }
}
