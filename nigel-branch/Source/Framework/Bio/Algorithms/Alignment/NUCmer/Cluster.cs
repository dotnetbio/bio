using System;
using System.Collections.Generic;
using System.Globalization;
using Bio.Algorithms.SuffixTree;
using System.Text;
using Bio.Properties;

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
        private readonly IList<MatchExtension> _internalMatches;

        /// <summary>
        /// The query direction
        /// </summary>
        private string _queryDirection;

        /// <summary>
        /// Initializes a new instance of the Cluster class
        /// </summary>
        /// <param name="matches">List of matches</param>
        public Cluster(IList<MatchExtension> matches)
        {
            _internalMatches = matches;
            IsFused = false;
            QueryDirection = ForwardDirection;
        }

        /// <summary>
        /// Initialize a reverse direction instance of the Cluster class
        /// </summary>
        /// <param name="matches">List of matches</param>
        /// <param name="isReverse">True/False reverse query direction</param>
        public Cluster(IList<MatchExtension> matches, bool isReverse) : this(matches)
        {
            if (isReverse)
                QueryDirection = ReverseDirection;
        }

        /// <summary>
        /// Gets list of maximum unique matches inside the cluster
        /// </summary>
        public IList<MatchExtension> Matches
        {
            get { return _internalMatches; }
        }

        /// <summary>
        /// Gets or sets the query sequence direction
        ///     FORWARD or REVERSE
        /// </summary>
        public string QueryDirection
        {
            get { return _queryDirection; }
            set
            {
                if (value != ForwardDirection && value != ReverseDirection)
                    throw new ArgumentOutOfRangeException("value", Resource.InvalidQueryDirection);
                _queryDirection = value;
            }
        }

        /// <summary>
        /// Returns TRUE if this is a REVERSE query sequence direction
        /// </summary>
        public bool IsReverseQueryDirection
        {
            get { return QueryDirection == ReverseDirection; }
        }

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
