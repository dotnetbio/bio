using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QUT.Bio.SilverMap.ObjectModel;
using QUT.Bio.Util;

namespace SequenceAssembler.SilverMapIntegration
{
    /// <summary>
    /// Defines a SilverMapCollection which presents a uniform interface to a collection
	/// of BLAST results.
    /// </summary>
    
	internal class FeatureCollection : IFeatureCollection
    {
        internal readonly Dictionary<string, ILinearDomain> sequences = new Dictionary<string, ILinearDomain>();
        internal readonly List<IFeature> features = new List<IFeature>();
        internal readonly List<Hit> hits = new List<Hit>();
        internal ILinearDomain initialQuerySequence;
        internal readonly HashList<ILinearDomain> querySequences = new HashList<ILinearDomain>();

        /// <summary>
        /// Gets an iterable list of the linear feature domains currently loaded.
        /// </summary>
        
		public IEnumerable<ILinearDomain> Domains
        {
            get
            {
                return sequences.Values;
            }
        }

        /// <summary>
        /// Gets a list of the features (which are objects that refer to extents on domains) that are currently loaded.
        /// </summary>
        
		public IEnumerable<IFeature> Features
        {
            get
            {
                return features;
            }
        }

        /// <summary>
        /// Gets a list of the hits that are currently loaded.
        /// </summary>
        
		public IEnumerable<Hit> Hits
        {
            get
            {
                return hits;
            }
        }

        /// <summary>
        /// Gets the intially focussed object. That will be the first query sequence in the BLAST result set.
        /// </summary>
        
		public ILinearDomain InitialFocus
        {
            get
            {
                return initialQuerySequence;
            }
        }

        /// <summary>
        /// Gets a lookup table containing a list of presently known query sequences.
        /// </summary>
        
		public HashList<ILinearDomain> Focii
        {
            get
            {
                return querySequences;
            }
        }
    }
}
