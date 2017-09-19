using System;
using System.Collections.Generic;

namespace Bio.Web.Blast
{
    /// <summary>
    /// BLAST request parameters
    /// </summary>
    public class BlastRequestParameters
    {
        /// <summary>
        /// Database to query
        /// </summary>
        /// <value>The database.</value>
        public string Database { get; set; }

        /// <summary>
        /// Program to use.
        /// </summary>
        /// <value>The program.</value>
        public string Program { get; set; }

        /// <summary>
        /// Additional parameters passed to the service.
        /// </summary>
        /// <value>The extra parameters.</value>
        public IDictionary<string,string> ExtraParameters { get; private set; }

        /// <summary>
        /// Set of sequences to search for
        /// </summary>
        /// <value>The sequences.</value>
        public IList<ISequence> Sequences { get; private set; }

        /// <summary>
        /// Constructor (default)
        /// </summary>
        public BlastRequestParameters(IEnumerable<KeyValuePair<string,string>> extraParameters = null)
        {
            this.Initialize(extraParameters);
            this.Sequences = new List<ISequence>();
        }

        /// <summary>
        /// Constructor which takes a set of sequences to initialize.
        /// </summary>
        /// <param name="sequences">Sequences.</param>
        /// <param name = "extraParameters">Extra parameters</param>
        public BlastRequestParameters(IEnumerable<ISequence> sequences, 
            IEnumerable<KeyValuePair<string, string>> extraParameters = null)
        {
            this.Initialize(extraParameters);
            this.Sequences = new List<ISequence>(sequences);
        }

        /// <summary>
        /// Initialize with defaults
        /// </summary>
        private void Initialize(IEnumerable<KeyValuePair<string,string>> extraParameters)
        {
            this.ExtraParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            this.Program = BlastProgram.Blastn;
            if (extraParameters != null) {
                foreach (var item in extraParameters)
                    this.ExtraParameters.Add(item);
            }
        }
    }
}