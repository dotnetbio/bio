using System.Globalization;
using Bio.Algorithms.MUMmer;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// This class extends MUMmerAttributes and adds NUCmer specific attributes
    /// required to run the NUCmer algorithm.
    /// </summary>
    public class NUCmerAttributes : MUMmerAttributes
    {
        /// <summary>
        /// Describes maximum fixed diagonal difference
        /// </summary>
        public const string FixedSeparation = "FIXEDSEPARATION";

        /// <summary>
        /// Describes maximum separation between the adjacent matches in clusters
        /// </summary>
        public const string MaximumSeparation = "MAXIMUMSEPARATION";

        /// <summary>
        /// Describes Minimum Output Score
        /// </summary>
        public const string MinimumScore = "MINIMUMSCORE";

        /// <summary>
        /// Describes Separation Factor
        /// </summary>
        public const string SeparationFactor = "SEPARATIONFACTOR";

        /// <summary>
        /// Describes number of bases to be extended before stopping alignment
        /// </summary>
        public const string BreakLength = "BREAKLENGTH";

        /// <summary>
        /// Initializes a new instance of the NUCmerAttributes class.
        /// </summary>
        public NUCmerAttributes()
        {
            AlignmentInfo alignmentAttribute;

            alignmentAttribute = new AlignmentInfo(
                    Properties.Resource.FIXED_SEPARATION_NAME,
                    Properties.Resource.FIXED_SEPARATION_DESCRIPTION,
                    true,
                    ClusterBuilder.DefaultFixedSeparation.ToString(CultureInfo.InvariantCulture),
                    AlignmentInfo.IntType,
                    null);
            Attributes.Add(FixedSeparation, alignmentAttribute);

            alignmentAttribute = new AlignmentInfo(
                    Properties.Resource.MAXIMUM_SEPARATION_NAME,
                    Properties.Resource.MAXIMUM_SEPARATION_DESCRIPTION,
                    true,
                    ClusterBuilder.DefaultMaximumSeparation.ToString(CultureInfo.InvariantCulture),
                    AlignmentInfo.IntType,
                    null);
            Attributes.Add(MaximumSeparation, alignmentAttribute);

            alignmentAttribute = new AlignmentInfo(
                    Properties.Resource.MINIMUM_SCORE_NAME,
                    Properties.Resource.MINIMUM_SCORE_DESCRIPTION,
                    true,
                    ClusterBuilder.DefaultMinimumScore.ToString(CultureInfo.InvariantCulture),
                    AlignmentInfo.IntType,
                    null);
            Attributes.Add(MinimumScore, alignmentAttribute);

            alignmentAttribute = new AlignmentInfo(
                    Properties.Resource.SEPARATION_FACTOR_NAME,
                    Properties.Resource.SEPARATION_FACTOR_DESCRIPTION,
                    true,
                    ClusterBuilder.DefaultSeparationFactor.ToString(CultureInfo.InvariantCulture),
                    AlignmentInfo.FloatType,
                    null);
            Attributes.Add(SeparationFactor, alignmentAttribute);

            alignmentAttribute = new AlignmentInfo(
                    Properties.Resource.BREAK_LENGTH_NAME,
                    Properties.Resource.BREAK_LENGTH_DESCRIPTION,
                    true,
                    ModifiedSmithWaterman.DefaultBreakLength.ToString(CultureInfo.InvariantCulture),
                    AlignmentInfo.IntType,
                    null);
            Attributes.Add(BreakLength, alignmentAttribute);
        }
    }
}
