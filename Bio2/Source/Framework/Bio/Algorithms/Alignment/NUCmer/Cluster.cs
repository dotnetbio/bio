using System.Collections.Generic;
using System.Globalization;
using Bio.Algorithms.SuffixTree;
using System.Linq;
using System.Text;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// An ordered list of matches between two a pair of sequences
    /// </summary>
    public class Cluster
    {
        /// <summary>
        /// Represents reverse query sequence direction
        /// </summary>
        public const string ReverseDirection = "REVERSE";

        /// <summary>
        /// Represents forward query sequence direction
        /// </summary>
        public const string ForwardDirection = "FORWARD";

        /// <summary>
        /// List of maximum unique matches inside the cluster
        /// </summary>
        private IList<MatchExtension> internalMatches;

        /// <summary>
        /// Initializes a new instance of the Cluster class
        /// </summary>
        /// <param name="matches">List of matches</param>
        public Cluster(IList<MatchExtension> matches)
        {
            internalMatches = matches;
            IsFused = false;
            QueryDirection = ForwardDirection;
        }

        /// <summary>
        /// Gets list of maximum unique matches inside the cluster
        /// </summary>
        public IList<MatchExtension> Matches
        {
            get { return internalMatches; }
        }

        /// <summary>
        /// Gets or sets the query sequence direction
        ///     FORWARD or REVERSE
        /// </summary>
        public string QueryDirection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cluster is already fused
        /// </summary>
        public bool IsFused { get; set; }

        /// <summary>
        /// Converts RefStart, QueryStart, Length, Score, WrapScore, IsGood for each cluster to string.
        /// </summary>
        /// <returns>RefStart, QueryStart, Length, Score, WrapScore, IsGood for each cluster.</returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (MatchExtension matchExtension in Matches)
            {
                stringBuilder.AppendLine(
                    string.Format(CultureInfo.CurrentCulture, Properties.Resource.ClusterToStringFormat,
                                  matchExtension.ReferenceSequenceOffset, matchExtension.QuerySequenceOffset,
                                  matchExtension.Length,
                                  matchExtension.Score, matchExtension.WrapScore, matchExtension.IsGood));
            }
            return stringBuilder.ToString();
        }
    }
}
