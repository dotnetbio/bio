using System;
using System.Collections.Generic;
using System.Globalization;

namespace Bio.Distributions
{
    /// <summary>
    /// Implementation of SufficientStatistics class for Boolean values and
    /// Missing values which cannot be classified in either of two states. 
    /// </summary>
    public class BooleanStatistics : SufficientStatistics
    {
        /// <summary>
        /// Use integer mapping for consistent array indexing conventions.
        /// Missing value which cannot be classified in either of two states. 
        /// </summary>
        public const int Missing = -1;

        /// <summary>
        /// False: Boolean state.
        /// </summary>
        public const int False = 0;

        /// <summary>
        /// True: Boolean state.
        /// </summary>
        public const int True = 1;

        /// <summary>
        /// Boolean value at current state.
        /// </summary>
        private readonly Classification _value;

        /// <summary>
        /// Whether boolean value is missing.
        /// </summary>
        private readonly bool _isMissing;
        
        /// <summary>
        /// Instantiate a new instance of BooleanStatistics class as missing value.
        /// </summary>
        private BooleanStatistics()
        {
            _isMissing = true;
        }

        /// <summary>
        /// Instantiate a new instance of BooleanStatistics class.
        /// </summary>
        /// <param name="classification">State of object. (True or False)</param>
        private BooleanStatistics(bool classification) : this(classification ? Classification.True : Classification.False) { }

        /// <summary>
        /// Instantiate a new instance of BooleanStatistics class.
        /// </summary>
        /// <param name="classification">State of object. (True or False or Missing)</param>
        private BooleanStatistics(Classification classification)
        {
            _value = classification;
            _isMissing = classification == Classification.Missing;
        }

        /// <summary>
        /// Gets a new instance of BooleanStatistics as missing value.
        /// </summary>
        /// <returns>Instance of boolean class and value as missing.</returns>
        public static BooleanStatistics GetMissingInstance
        {
            get
            {
                return new BooleanStatistics(Classification.Missing);
            }
        }

        /// <summary>
        /// Instantiate a new instance of BooleanStatistics class.
        /// </summary>
        /// <param name="classification">The classification flag.</param>
        /// <returns></returns>
        public static BooleanStatistics GetInstance(bool classification)
        {
            return new BooleanStatistics(classification);
        }

        /// <summary>
        /// Convert Sufficient statistics to boolean statistics.
        /// </summary>
        /// <param name="dictionary">Dictionary containing key and state statistics.</param>
        /// <returns>Dictionary containing key and BooleanStatistics.</returns>
        public static Dictionary<string, BooleanStatistics> ConvertToBooleanStatistics(Dictionary<string, SufficientStatistics> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            Dictionary<string, BooleanStatistics> result = new Dictionary<string, BooleanStatistics>();

            foreach (KeyValuePair<string, SufficientStatistics> stringAndSuff in dictionary)
            {
                result.Add(stringAndSuff.Key, stringAndSuff.Value.AsBooleanStatistics());
            }

            return result;
        }

        /// <summary>
        /// Returns a System.String that represents the current Object.
        /// </summary>
        /// <returns>A string that represents the current Object.</returns>
        public override string ToString()
        {
            return ((int)_value).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts boolean statistics to boolean value.
        /// </summary>
        /// <param name="stats">Boolean statistics to be converted.</param>
        /// <returns>Converted value.</returns>
        public static implicit operator bool(BooleanStatistics stats)
        {
            if (stats == null)
            {
                return false;
            }

            return !stats.IsMissing() && stats._value == Classification.True;
        }

        /// <summary>
        /// Converts boolean value to boolean statistics.
        /// </summary>
        /// <param name="classification">Boolean value to be converted.</param>
        /// <returns>Converted value.</returns>
        public static implicit operator BooleanStatistics(bool classification)
        {
            return new BooleanStatistics(classification);
        }

        /// <summary>
        /// Converts boolean statistics to integer.
        /// </summary>
        /// <param name="stats">Boolean statistics to be converted.</param>
        /// <returns>Integer value representing boolean value.</returns>
        public static explicit operator int(BooleanStatistics stats)
        {
            if (stats == null)
            {
                throw new ArgumentNullException("stats");
            }

            if (stats.IsMissing())
            {
                throw new InvalidCastException("Cannot cast missing statistics to a bool.");
            }
            
            return (int)stats._value;
        }

        /// <summary>
        /// Converts Discrete statistics to boolean statistics.
        /// </summary>
        /// <param name="stats">Object of discrete statistics to be converted.</param>
        /// <returns>Boolean statistics representing DiscreteStatistics</returns>
        public static explicit operator BooleanStatistics(DiscreteStatistics stats)
        {
            if (stats == null)
            {
                throw new ArgumentNullException("stats");
            }

            if (stats.IsMissing() || stats.Value < 0)
            {
                return GetMissingInstance;
            }
            try
            {
                Classification value = (Classification)(int)stats;
                return new BooleanStatistics(value);
            }
            catch
            {
                throw new InvalidCastException("Cannot cast " + stats + " to BooleanStatistics.");
            }
        }

        /// <summary>
        /// Converts BooleanStatistics to DiscreteStatistics.
        /// </summary>
        /// <param name="stats">Object of BooleanStatistics to be converted.</param>
        /// <returns>DiscreteStatistics object.</returns>
        public static implicit operator DiscreteStatistics(BooleanStatistics stats)
        {
            if (stats == null)
            {
                return null;
            }

            return DiscreteStatistics.GetInstance((int)stats._value);
        }

        /// <summary>
        /// Converts MissingStatistics to boolean statistics.
        /// </summary>
        /// <param name="missing">Object of MissingStatistics to be converted.</param>
        /// <returns>BooleanStatistics object.</returns>
        public static implicit operator BooleanStatistics(MissingStatistics missing)
        {
            if (missing == null)
            {
                return null;
            }

            return BooleanStatistics.GetMissingInstance;
        }

        /// <summary>
        /// Compares a given object with the current object.
        /// </summary>
        /// <param name="obj">Object to be compared.</param>
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
        /// <param name="stats">SufficientStatistics object to be compared</param>
        /// <returns>True if both the objects are equal.</returns>
        public override bool Equals(SufficientStatistics stats)
        {
            if (stats == null)
            {
                throw new ArgumentNullException("stats");
            }

            if (stats.IsMissing())
            {
                return IsMissing();
            }
            else if (stats is BooleanStatistics)
            {
                return _value == stats.AsBooleanStatistics()._value;
            }
            else
            {
                return stats.Equals(this);
            }
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Determines the value of object which cannot be classified into any statistical distribution bins.
        /// </summary>
        public override bool IsMissing()
        {
            return _isMissing;
        }

        /// <summary>
        /// Try converting the given string into SufficientStatistics object.
        /// </summary>
        /// <param name="val">string to be converted.</param>
        /// <param name="result">SufficentStatistics object which corresponding to the given string.</param>
        /// <returns>Whether string was successfully converted.</returns>
        new public static bool TryParse(string val, out SufficientStatistics result)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new ArgumentNullException("val");
            }

            result = null;
            if (val.Equals("true", StringComparison.CurrentCultureIgnoreCase) || val == "1")
            {
                result = BooleanStatistics.GetInstance(true);
            }
            else if (val.Equals("false", StringComparison.CurrentCultureIgnoreCase) || val == "0")
            {
                result = BooleanStatistics.GetInstance(false);
            }
            else if (val.Equals("null", StringComparison.CurrentCultureIgnoreCase) || val == "-1")
            {
                result = BooleanStatistics.GetMissingInstance;
            }
            return result != null;
        }


        /// <summary>
        /// Converts current object to StatisticsList.
        /// </summary>
        /// <returns>StatisticsList object.</returns>
        public override StatisticsList AsStatisticsList()
        {
            return IsMissing() ? StatisticsList.GetMissingInstance : StatisticsList.GetInstance(this);
        }

        /// <summary>
        /// Converts current object to GaussianStatistics.
        /// </summary>
        /// <returns>GaussianStatistics object.</returns>
        public override GaussianStatistics AsGaussianStatistics()
        {
            return IsMissing() ? GaussianStatistics.GetMissingInstance : GaussianStatistics.GetInstance((int)_value, 0, 1);
        }

        /// <summary>
        /// Converts current object to ContinuousStatistics.
        /// </summary>
        /// <returns>ContinuousStatistics object.</returns>
        public override ContinuousStatistics AsContinuousStatistics()
        {
            return IsMissing() ? ContinuousStatistics.GetMissingInstance : ContinuousStatistics.GetInstance((int)_value);
        }

        /// <summary>
        /// Converts current object to DiscreteStatistics.
        /// </summary>
        /// <returns>DiscreteStatistics object.</returns>
        public override DiscreteStatistics AsDiscreteStatistics()
        {
            return IsMissing() ? DiscreteStatistics.GetMissingInstance : DiscreteStatistics.GetInstance((int)_value);
        }

        /// <summary>
        /// Converts current object to BooleanStatistics.
        /// </summary>
        /// <returns>BooleanStatistics object.</returns>
        public override BooleanStatistics AsBooleanStatistics()
        {
            return this;
        }
    }
}
