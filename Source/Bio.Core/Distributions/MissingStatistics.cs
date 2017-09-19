using System;

namespace Bio.Distributions
{
    /// <summary>
    /// Missing Statistics class.
    /// </summary>
    public class MissingStatistics : SufficientStatistics
    {
        /// <summary>
        /// Missing Character.
        /// </summary>
        public const char MissingChar = '?';

        /// <summary>
        /// Missing Statistics constructor.
        /// </summary>
        private MissingStatistics()
        {
        }

        /// <summary>
        /// Get Instance.
        /// </summary>
        /// <returns>Missing Statistics.</returns>
        static public MissingStatistics GetInstance
        {
            get
            {
                return _singleton.Value;
            }
        }

        /// <summary>
        /// Lazy Missing Statistics singleton.
        /// </summary>
        static Lazy<MissingStatistics> _singleton = new Lazy<MissingStatistics>(() => new MissingStatistics());

        /// <summary>
        /// Try Parse the value.
        /// </summary>
        /// <param name="val">The Value.</param>
        /// <param name="result">Sufficient Statistics result.</param>
        /// <returns>Returns true if parsed properly.</returns>
        new public static bool TryParse(string val, out SufficientStatistics result)
        {
            result = null;
                       
            if (string.IsNullOrEmpty(val) ||
                val.Equals("missing", StringComparison.CurrentCultureIgnoreCase) ||
                val.Equals("?", StringComparison.CurrentCultureIgnoreCase))
            {
                result = GetInstance;
            }

            return result != null;
        }

        /// <summary>
        /// Checks the IsMissing flag and returns it.
        /// </summary>
        /// <returns>Returns the IsMissing flag.</returns>
        public override bool IsMissing()
        {
            return true;
        }

        /// <summary>
        /// Compares object with Is Missing.
        /// </summary>
        /// <param name="stats">Sufficient Statistics.</param>
        /// <returns>Returns true if fount equals.</returns>
        public override bool Equals(SufficientStatistics stats)
        {
            if (stats == null)
            {
                return false;
            }

            return stats.IsMissing();
        }

        /// <summary>
        /// Compares object with Sufficient Statistics.
        /// </summary>
        /// <param name="obj">The Object.</param>
        /// <returns>Returns true if fount equals.</returns>
        public override bool Equals(object obj)
        {
            SufficientStatistics stats = obj as SufficientStatistics;
            if (stats != null)
            {
                return (stats).IsMissing();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get Hash code.
        /// </summary>
        /// <returns>Returns Hash code.</returns>
        public override int GetHashCode()
        {
            return "Missing".GetHashCode();
        }

        /// <summary>
        /// Get Hash Code.
        /// </summary>
        /// <returns>Returns the Hash code.</returns>
        public override string ToString()
        {
            return MissingChar.ToString();
        }

        /// <summary>
        /// Converts current object As Statistics List.
        /// </summary>
        /// <returns>Statistics List.</returns>
        public override StatisticsList AsStatisticsList()
        {
            return StatisticsList.GetMissingInstance;
        }

        /// <summary>
        /// Converts current object As Gaussian Statistics.
        /// </summary>
        /// <returns>Gaussian Statistics.</returns>
        public override GaussianStatistics AsGaussianStatistics()
        {
            return GaussianStatistics.GetMissingInstance;
        }

        /// <summary>
        /// Converts current object As Continuous Statistics.
        /// </summary>
        /// <returns>Continuous Statistics.</returns>
        public override ContinuousStatistics AsContinuousStatistics()
        {
            return ContinuousStatistics.GetMissingInstance;
        }

        /// <summary>
        /// Converts current object As Discrete Statistics.
        /// </summary>
        /// <returns>Discrete Statistics.</returns>
        public override DiscreteStatistics AsDiscreteStatistics()
        {
            return DiscreteStatistics.GetMissingInstance;
        }

        /// <summary>
        /// Converts current object As Boolean Statistics.
        /// </summary>
        /// <returns>Boolean Statistics.</returns>
        public override BooleanStatistics AsBooleanStatistics()
        {
            return BooleanStatistics.GetMissingInstance;
        }
    }
}
