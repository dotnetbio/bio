using System;
using System.Collections.Generic;
using System.Globalization;
using Bio.Util;

namespace Bio.Distributions
{
    /// <summary>
    /// Gaussian Statistics. It assumes that the observations are closely clustered 
    /// around the mean, μ, and this amount is decaying quickly as we go farther away from the mean.
    /// </summary>
    public class GaussianStatistics : SufficientStatistics
    {
        /// <summary>
        /// The mean Value.
        /// </summary>
        private readonly double _mean;

        /// <summary>
        /// The Variance.
        /// </summary>
        private readonly double _variance;

        /// <summary>
        /// Sample size.
        /// </summary>
        private readonly int _sampleSize;

        /// <summary>
        /// IsMissing flag.
        /// </summary>
        private readonly bool _isMissing;    // this way, the default constructor will set _isMissing to true.

        /// <summary>
        /// Constructor Gaussian Statistics.
        /// </summary>
        private GaussianStatistics()
        {
            _isMissing = true;
        }

        /// <summary>
        /// Constructor Gaussian Statistics.
        /// </summary>
        /// <param name="mean">The Mean.</param>
        /// <param name="var">The Variance.</param>
        /// <param name="sampleSize">The Sample size.</param>
        private GaussianStatistics(double mean, double var, int sampleSize)
        {
            _mean = mean;
            _variance = var;
            _sampleSize = sampleSize;
            _isMissing = false;
        }

        /// <summary>
        /// Get Missing Instance.
        /// </summary>
        /// <returns>Gaussian Statistics.</returns>
        static public GaussianStatistics GetMissingInstance
        {
            get
            {
                return new GaussianStatistics();
            }
        }

        /// <summary>
        /// Gets new Instance.
        /// </summary>
        /// <param name="mean">The mean.</param>
        /// <param name="variance">The Variance.</param>
        /// <param name="sampleSize">Sample size.</param>
        /// <returns>Gaussian Statistics.</returns>
        static public GaussianStatistics GetInstance(double mean, double variance, int sampleSize)
        {
            return new GaussianStatistics(mean, variance, sampleSize);
        }

        /// <summary>
        /// Get's the sufficient statistics of the population using <b>population</b> variance (as opposed to the unbiased sample variance).
        /// </summary>
        /// <param name="observations">Collection of observations.</param>
        /// <returns>Gaussian Statistics.</returns>
        public static GaussianStatistics GetInstance(IEnumerable<double> observations)
        {
            if (observations == null)
            {
                throw new ArgumentNullException("observations");
            }

            int n = 0;
            double sum = 0;
            foreach (double d in observations)
            {
                sum += d;
                n++;
            }
            double mean = sum / n;
            double variance = 0;
            foreach (double d in observations)
            {
                variance += (d - mean) * (d - mean);
            }
            variance /= n;
            return GetInstance(mean, variance, n);
        }

        /// <summary>
        /// The Mean value.
        /// </summary>
        public double Mean
        {
            get
            {
                Helper.CheckCondition(!IsMissing());
                return _mean;
            }
        }

        /// <summary>
        /// The Variance.
        /// </summary>
        public double Variance
        {
            get
            {
                Helper.CheckCondition(!IsMissing());
                return _variance;
            }
        }

        /// <summary>
        /// The sample Size.
        /// </summary>
        public int SampleSize
        {
            get
            {
                Helper.CheckCondition(!IsMissing());
                return _sampleSize;
            }
        }

        /// <summary>
        /// Sum Of Squares.
        /// </summary>
        public double SumOfSquares
        {
            get
            {
                Helper.CheckCondition(!IsMissing());
                return (_variance + _mean * _mean) * _sampleSize;
            }
        }

        /// <summary>
        /// Try Parse the value.
        /// </summary>
        /// <param name="val">The Value.</param>
        /// <param name="result">Sufficient Statistics result.</param>
        /// <returns>Returns true if parsed properly.</returns>
        new public static bool TryParse(string val, out SufficientStatistics result)
        {
            result = null;
            if (string.IsNullOrEmpty(val))
            {
                result = GetMissingInstance;
                return false;
            }
            else
            {
                string[] fields = val.Split(',');
                if (!(fields.Length == 3))
                {
                    return false;
                }
                double mean, variance;
                int sampleSize;

                if (double.TryParse(fields[0], out mean) &&
                    double.TryParse(fields[1], out variance) &&
                    int.TryParse(fields[2], out sampleSize))
                {
                    result = GaussianStatistics.GetInstance(mean, variance, sampleSize);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>Returns to string.</returns>
        public override string ToString()
        {
            return IsMissing() ? "Missing" : string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", Mean, Variance, SampleSize);
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
        /// Compares object with Sufficient Statistics.
        /// </summary>
        /// <param name="obj">The Object.</param>
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
        /// Compares Sufficient Statistics.
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

            GaussianStatistics gaussStats = stats.AsGaussianStatistics();

            return _mean == gaussStats._mean && _variance == gaussStats._variance && _sampleSize == gaussStats._sampleSize;
        }

        /// <summary>
        /// Get Hash Code.
        /// </summary>
        /// <returns>Returns the Hash code.</returns>
        public override int GetHashCode()
        {
            return IsMissing() ? MissingStatistics.GetInstance.GetHashCode() : ToString().GetHashCode();
        }

        /// <summary>
        /// Missing Statistics to Gaussian Statistics converter.
        /// </summary>
        /// <param name="missing">Missing Statistics</param>
        /// <returns>Returns the converted type.</returns>
        public static implicit operator GaussianStatistics(MissingStatistics missing)
        {
            if (missing == null)
            {
                return null;
            }

            return GaussianStatistics.GetMissingInstance;
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
            return this;
        }

        /// <summary>
        /// Converts current object As Continuous Statistics.
        /// </summary>
        /// <returns>Continuous Statistics.</returns>
        public override ContinuousStatistics AsContinuousStatistics()
        {
            return IsMissing() ? ContinuousStatistics.GetMissingInstance : ContinuousStatistics.GetInstance(Mean);
        }

        /// <summary>
        /// Converts current object As Discrete Statistics.
        /// </summary>
        /// <returns>Discrete Statistics.</returns>
        public override DiscreteStatistics AsDiscreteStatistics()
        {
            return IsMissing() ? DiscreteStatistics.GetMissingInstance : DiscreteStatistics.GetInstance((int)Mean);
        }

        /// <summary>
        /// Converts current object As Boolean Statistics.
        /// </summary>
        /// <returns>Boolean Statistics.</returns>
        public override BooleanStatistics AsBooleanStatistics()
        {
            int meanAsInt = (int)Mean;
            if (!IsMissing() && (meanAsInt < -1 || meanAsInt > 1))
                throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture, "Cannot cast {0} to Boolean.", Mean));

            return IsMissing() || meanAsInt == -1 ? BooleanStatistics.GetMissingInstance : BooleanStatistics.GetInstance(meanAsInt == 1);
        }

        /// <summary>
        /// Add two Gaussian Statistics.
        /// </summary>
        /// <param name="x">GaussianStatistics x.</param>
        /// <param name="y">GaussianStatistics y.</param>
        /// <returns>Returns added result of two Gaussian Statistics.</returns>
        public static GaussianStatistics Add(GaussianStatistics x, GaussianStatistics y)
        {
            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (y == null)
            {
                throw new ArgumentNullException("y");
            }

            int rN = x.SampleSize + y.SampleSize;
            double rMean = (x.SampleSize * x.Mean + y.SampleSize * y.Mean) / rN;
            double rVar = (x.SumOfSquares + y.SumOfSquares) / rN - rMean * rMean;
            if (rVar < 0)
            {
                Helper.CheckCondition(rVar > -1e-10, "Computed negative variance! " + rVar);
                rVar = 0;
            }

            GaussianStatistics result = GaussianStatistics.GetInstance(rMean, rVar, rN);
            return result;
        }

    }
}
