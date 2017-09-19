using System;
using System.Globalization;

namespace Bio.Distributions
{
    /// <summary>
    /// Act as a base class for classes which contains  distributional statistics.  
    /// </summary>
    public abstract class SufficientStatistics : IComparable<SufficientStatistics>
    {
        /// <summary>
        /// Determines the value of object which cannot be classified into any statistical distribution bins. 
        /// </summary>
        /// <returns></returns>
        public abstract bool IsMissing();

        /// <summary>
        /// Converts current object to StatisticsList.
        /// </summary>
        /// <returns>StatisticsList object.</returns>
        public abstract StatisticsList AsStatisticsList();

        /// <summary>
        /// Converts current object to GaussianStatistics.
        /// </summary>
        /// <returns>GaussianStatistics object.</returns>
        public abstract GaussianStatistics AsGaussianStatistics();

        /// <summary>
        /// Converts current object to ContinuousStatistics.
        /// </summary>
        /// <returns>ContinuousStatistics object.</returns>
        public abstract ContinuousStatistics AsContinuousStatistics();

        /// <summary>
        /// Converts current object to DiscreteStatistics.
        /// </summary>
        /// <returns>DiscreteStatistics object.</returns>
        public abstract DiscreteStatistics AsDiscreteStatistics();

        /// <summary>
        /// Converts current object to BooleanStatistics.
        /// </summary>
        /// <returns>BooleanStatistics object.</returns>
        public abstract BooleanStatistics AsBooleanStatistics();

        /// <summary>
        /// Determines whether the specified Object is equal to the current Object.
        /// </summary>
        /// <param name="stats">The Object to compare with the current Object</param>
        /// <returns>true if the specified Object is equal to the current Object; otherwise, false.</returns>
        public abstract bool Equals(SufficientStatistics stats);

        /// <summary>
        /// Try converting the given string into SufficientStatistics object.
        /// </summary>
        /// <param name="val">string to be converted.</param>
        /// <param name="result">SufficentStatistics object which corresponding to the given string.</param>
        /// <returns>Whether string was successfully converted.</returns>
        public static bool TryParse(string val, out SufficientStatistics result)
        {
            return
                MissingStatistics.TryParse(val, out result) ||
                GaussianStatistics.TryParse(val, out result) ||
                BooleanStatistics.TryParse(val, out result) ||
                DiscreteStatistics.TryParse(val, out result) ||
                ContinuousStatistics.TryParse(val, out result) ||
                StatisticsList.TryParse(val, out result);
        }

        /// <summary>
        /// Convert given string into SufficientStatistics object.
        /// If string is not in correct format, throws an argument exception.
        /// </summary>
        /// <param name="val">string to be converted.</param>
        /// <returns>SufficentStatistics object which corresponding to the given string.</returns>
        public static SufficientStatistics Parse(string val)
        {
            SufficientStatistics result;
            if (TryParse(val, out result))
            {
                return result;
            }

            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, 
                "Unable to parse \"{0}\" into an instance of ISufficientStatistics", val));
        }

        /// <summary>
        ///  Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other"> An object to compare with this object.</param>
        /// <returns> A value that indicates the relative order of the objects being compared.
        /// The return value has the following meanings: Value Meaning Less than zero
        /// This object is less than the other parameter.Zero This object is equal to
        /// other. Greater than zero This object is greater than other.</returns>
        public virtual int CompareTo(SufficientStatistics other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            return this.AsDiscreteStatistics().Value - other.AsDiscreteStatistics().Value;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            throw new NotSupportedException("Derived class must override GetHashCode()");
        }

        /// <summary>
        /// Determines whether the specified SObject is equal to the current Object.
        /// </summary>
        /// <param name="obj">The Object to compare with the current Object</param>
        /// <returns>true if the specified Object is equal to the current Object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            throw new NotSupportedException("Derived class must override Equals()");
        }
    }
}
