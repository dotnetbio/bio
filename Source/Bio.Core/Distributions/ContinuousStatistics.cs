using System;
using System.Globalization;

namespace Bio.Distributions
{
    /// <summary>
    /// Continuous Statistics class.
    /// </summary>
    public class ContinuousStatistics : SufficientStatistics
    {
        /// <summary>
        /// Value.
        /// </summary>
        private readonly double _value;

        /// <summary>
        /// IsMissing.
        /// </summary>
        private readonly bool _isMissing;
        
        /// <summary>
        /// Value.
        /// </summary>
        public double Value
        {
            get
            {
                if (IsMissing()) throw new ArgumentException("Attempting to retrieve value from missing statistics");
                return _value;
            }
        }

        /// <summary>
        /// Continuous Statistics.
        /// </summary>
        private ContinuousStatistics()
        {
            _isMissing = true;
        }

        /// <summary>
        /// Continuous Statistics.
        /// </summary>
        /// <param name="value">The value.</param>
        private ContinuousStatistics(double value)
        {
            _value = value;
            _isMissing = false;
        }
        
        /// <summary>
        /// GetMissing Instance.
        /// </summary>
        /// <returns>Continuous Statistics.</returns>
        public static ContinuousStatistics GetMissingInstance
        {
            get
            {
                return new ContinuousStatistics();
            }
        }

        /// <summary>
        /// Get Instance.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Continuous Statistics.</returns>
        public static ContinuousStatistics GetInstance(double value)
        {
            return new ContinuousStatistics(value);
        }

        /// <summary>
        /// Try Parse.
        /// </summary>
        /// <param name="val">Value String.</param>
        /// <param name="result">Sufficient Statistics.</param>
        /// <returns>Return true in parsed properly.</returns>
        new public static bool TryParse(string val, out SufficientStatistics result)
        {
            double valAsDouble;
            if (double.TryParse(val, out valAsDouble))
            {
                result = ContinuousStatistics.GetInstance(valAsDouble);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Is Missing.
        /// </summary>
        /// <returns>True if missing.</returns>
        public override bool IsMissing()
        {
            return _isMissing;
        }

        /// <summary>
        /// Converts double to Continuous Statistics.
        /// </summary>
        /// <param name="value">The Value</param>
        /// <returns>Continuous Statistics.</returns>
        public static implicit operator ContinuousStatistics(double value)
        {
            return new ContinuousStatistics(value);
        }

        /// <summary>
        /// Converts ContinuousStatistics to double.
        /// </summary>
        /// <param name="stats">Continuous Statistics.</param>
        /// <returns>Double.</returns>
        public static implicit operator double(ContinuousStatistics stats)
        {
            if (stats == null)
            {
                throw new ArgumentNullException("stats");
            }
            return stats.Value;
        }

        /// <summary>
        /// Converts MissingStatistics to ContinuousStatistics.
        /// </summary>
        /// <param name="missing">Missing Statistics.</param>
        /// <returns>ContinuousStatistics.</returns>
        public static implicit operator ContinuousStatistics(MissingStatistics missing)
        {
            if (missing == null)
            {
                return null;
            }

            return ContinuousStatistics.GetMissingInstance;
        }

        /// <summary>
        /// Converts DiscreteStatistics to ContinuousStatistics.
        /// </summary>
        /// <param name="discreteStats">Discrete Statistics.</param>
        /// <returns>ContinuousStatistics.</returns>
        public static implicit operator ContinuousStatistics(DiscreteStatistics discreteStats)
        {
            if (discreteStats == null)
            {
                return null;
            }

            return discreteStats.IsMissing() ? ContinuousStatistics.GetMissingInstance : ContinuousStatistics.GetInstance(discreteStats.Value);
        }

        /// <summary>
        /// Converts ContinuousStatistics to DiscreteStatistics.
        /// </summary>
        /// <param name="continuousStats">Continuous Statistics.</param>
        /// <returns>DiscreteStatistics.</returns>
        public static explicit operator DiscreteStatistics(ContinuousStatistics continuousStats)
        {
            if (continuousStats == null)
            {
                return null;
            }

            return continuousStats.IsMissing() ? DiscreteStatistics.GetMissingInstance : DiscreteStatistics.GetInstance((int)continuousStats.Value);
        }

        /// <summary>
        /// Compares a given object with the current object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>True if both the objects are equal.</returns>
        public override bool Equals(object obj)
        {
            SufficientStatistics stats = obj as SufficientStatistics;
            if (stats != null)
            {
                return Equals(stats);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Compares a given SufficientStatistics object with the current object.
        /// </summary>
        /// <param name="stats">SufficientStatistics object to be compared.</param>
        /// <returns>True if both the objects are equal.</returns>
        public override bool Equals(SufficientStatistics stats)
        {
            if (stats == null)
            {
                throw new ArgumentNullException("stats");
            }

            if (IsMissing() && stats.IsMissing())
            {
                return true;
            }

            return this.AsGaussianStatistics().Equals(stats);  // let the most general class decide
        }

        /// <summary>
        /// Get Hash Code.
        /// </summary>
        /// <returns>Returns the Hash code.</returns>
        public override int GetHashCode()
        {
            return IsMissing() ? MissingStatistics.GetInstance.GetHashCode() : Value.GetHashCode();
        }

        /// <summary>
        /// converts to string.
        /// </summary>
        /// <returns>Returns value in string format.</returns>
        public override string ToString()
        {
            return IsMissing() ? "Missing" : Value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts current object As Statistics List.
        /// </summary>
        /// <returns>Statistics List.</returns>
        public override StatisticsList AsStatisticsList()
        {
            return IsMissing() ? StatisticsList.GetMissingInstance : StatisticsList.GetInstance(this);
        }

        /// <summary>
        /// Converts current object As Gaussian Statistics.
        /// </summary>
        /// <returns>Gaussian Statistics.</returns>
        public override GaussianStatistics AsGaussianStatistics()
        {
            return IsMissing() ? GaussianStatistics.GetMissingInstance : GaussianStatistics.GetInstance(Value, 0, 1);
        }

        /// <summary>
        /// Converts current object As Continuous Statistics.
        /// </summary>
        /// <returns>Continuous Statistics.</returns>
        public override ContinuousStatistics AsContinuousStatistics()
        {
            return this;
        }

        /// <summary>
        /// Converts current object As Discrete Statistics.
        /// </summary>
        /// <returns>Discrete Statistics.</returns>
        public override DiscreteStatistics AsDiscreteStatistics()
        {
            return IsMissing() ? DiscreteStatistics.GetMissingInstance : DiscreteStatistics.GetInstance((int)Value);
        }

        /// <summary>
        /// Converts current object As Boolean Statistics.
        /// </summary>
        /// <returns>Boolean Statistics.</returns>
        public override BooleanStatistics AsBooleanStatistics()
        {
            int valAsInt = (int)Value;
            if (!IsMissing() && !(valAsInt < -1 || valAsInt > 1))
            {
                throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture,"Cannot convert {0} to Boolean.", Value));
            }

            return IsMissing() || valAsInt == -1 ? BooleanStatistics.GetMissingInstance : BooleanStatistics.GetInstance(valAsInt == 1);
        }
    }
}
