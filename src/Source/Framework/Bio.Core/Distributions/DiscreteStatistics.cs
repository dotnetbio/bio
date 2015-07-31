using System;
using System.Globalization;

namespace Bio.Distributions
{
    /// <summary>
    /// Discrete Statistics.This can not take a value between two values unlike Continuous Statistics.
    /// </summary>
    public class DiscreteStatistics : SufficientStatistics
    {
        /// <summary>
        /// The Value.
        /// </summary>
        private readonly int _value;

        /// <summary>
        /// Is missing flag.
        /// </summary>
        private readonly bool _isMissing;
        
        /// <summary>
        /// The Value.
        /// </summary>
        public int Value
        {
            get
            {
                return _value;
            }
        }

        /// <summary>
        /// Discrete Statistics constructor.
        /// </summary>
        private DiscreteStatistics()
        {
            _value = -1;
            _isMissing = true;
        }

        /// <summary>
        /// Discrete Statistics constructor.
        /// </summary>
        /// <param name="discreteClassification">The Discrete Classification.</param>
        private DiscreteStatistics(int discreteClassification)
        {
            _value = discreteClassification;
            _isMissing = false;
        }

        /// <summary>
        /// Get Missing Instance.
        /// </summary>
        /// <returns>Discrete Statistics.</returns>
        public static DiscreteStatistics GetMissingInstance
        {
            get
            {
                return new DiscreteStatistics();
            }
        }

        /// <summary>
        /// Get Instance.
        /// </summary>
        /// <param name="discreteteClassification">Discrete Classification.</param>
        /// <returns>Discrete Statistics.</returns>
        public static DiscreteStatistics GetInstance(int discreteteClassification)
        {
            return new DiscreteStatistics(discreteteClassification);
        }

        /// <summary>
        /// Try to Parse the value.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <param name="result">Sufficient Statistics result.</param>
        /// <returns>Returns true if parsed properly.</returns>
        new public static bool TryParse(string val, out SufficientStatistics result)
        {
            int valAsInt;
            if (int.TryParse(val, out valAsInt))
            {
                result = DiscreteStatistics.GetInstance(valAsInt);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Checks the IsMissing flag and returns it.
        /// </summary>
        /// <returns>Returns the IsMissing flag.</returns>
        public override bool IsMissing()
        {
            return _isMissing;
        }

        /// <summary>
        /// Int to Discrete Statistics converter.
        /// </summary>
        /// <param name="classification">The classification.</param>
        /// <returns>Returns the converted type.</returns>
        public static implicit operator DiscreteStatistics(int classification)
        {
            return new DiscreteStatistics(classification);
        }

        /// <summary>
        /// Discrete Statistics to Int converter.
        /// </summary>
        /// <param name="stats">Discrete Statistics.</param>
        /// <returns>Returns the converted type.</returns>
        public static implicit operator int(DiscreteStatistics stats)
        {
            if (stats == null)
            {
                throw new ArgumentNullException("stats");
            }

            return stats.Value;
        }

        /// <summary>
        /// Missing Statistics to Discrete Statistics converter.
        /// </summary>
        /// <param name="missing">Missing Statistics.</param>
        /// <returns>Returns the converted type.</returns>
        public static implicit operator DiscreteStatistics(MissingStatistics missing)
        {
            if (missing == null)
            {
                return null;
            }

            return DiscreteStatistics.GetMissingInstance;
        }

        /// <summary>
        /// Compares object with Sufficient Statistics.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Returns true if fount equals.</returns>
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
        /// Compares Sufficient Statistics with Discrete Statistics and Boolean Statistics.
        /// </summary>
        /// <param name="stats">Sufficient Statistics.</param>
        /// <returns>Returns true if equal.</returns>
        public override bool Equals(SufficientStatistics stats)
        {
            if (stats == null)
            {
                return false;
            }

            if (IsMissing() && stats.IsMissing())
            {
                return true;
            }
            else if (stats is DiscreteStatistics)
            {
                return this.Value == stats.AsDiscreteStatistics().Value;
            }
            else if (stats is BooleanStatistics)
            {
                return this.Value == stats.AsDiscreteStatistics().Value;
            }
            else
            {
                return stats.Equals(this);
            }
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
        /// Converts to string.
        /// </summary>
        /// <returns>Returns to string.</returns>
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
            return IsMissing() ? ContinuousStatistics.GetMissingInstance : ContinuousStatistics.GetInstance(Value);
        }

        /// <summary>
        /// Converts current object As Discrete Statistics.
        /// </summary>
        /// <returns>Discrete Statistics.</returns>
        public override DiscreteStatistics AsDiscreteStatistics()
        {
            return this;
        }

        /// <summary>
        /// Converts current object As Boolean Statistics.
        /// </summary>
        /// <returns>Boolean Statistics.</returns>
        public override BooleanStatistics AsBooleanStatistics()
        {
            if (!IsMissing() && (Value < -1 || Value > 1))
            {
                throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture, "Cannot convert {0} to Boolean", Value));
            }
            return IsMissing() || Value == -1 ? BooleanStatistics.GetMissingInstance : BooleanStatistics.GetInstance(Value == 1);
        }
    }
}
