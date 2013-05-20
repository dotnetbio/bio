using System;
using System.Collections.Generic;
using System.Text;

namespace Bio.Distributions
{
    /// <summary>
    /// Statistics List.
    /// </summary>
    [Serializable]
    public class StatisticsList : SufficientStatistics, IEnumerable<SufficientStatistics>, ICloneable
    {
        /// <summary>
        /// The separator.
        /// </summary>
        public const char SEPARATOR = ';';

        /// <summary>
        /// List of SufficientStatistics.
        /// </summary>
        private readonly List<SufficientStatistics> _stats;

        /// <summary>
        /// IsMissing Flag.
        /// </summary>
        private bool _isMissing;

        /// <summary>
        /// Hash Code.
        /// </summary>
        private int? _hashCode;

        /// <summary>
        /// Sufficient Statistics indexer.
        /// </summary>
        /// <param name="idx">Index id.</param>
        /// <returns>Gets or sets the index based on index id.</returns>
        public SufficientStatistics this[int idx]
        {
            get
            {
                return _stats[idx];
            }
            set
            {
                if (value != null)
                {
                    _stats[idx] = value;
                    _isMissing |= value.IsMissing();
                    _hashCode = null;
                }
                else
                {
                    _stats[idx] = null;
                    _hashCode = null;
                }
            }
        }

        /// <summary>
        /// Stats count.
        /// </summary>
        public int Count
        {
            get { return _stats.Count; }
        }

        /// <summary>
        /// Sufficient Statistics end.
        /// </summary>
        private SufficientStatistics Last
        {
            get { return _stats[_stats.Count - 1]; }
        }

        /// <summary>
        /// Statistics List constructor.
        /// </summary>
        public StatisticsList()
        {
            _stats = new List<SufficientStatistics>();
            _isMissing = true;
        }

        /// <summary>
        /// Statistics List constructor.
        /// </summary>
        /// <param name="stats">Collection of Sufficient Statistics.</param>
        private StatisticsList(IEnumerable<SufficientStatistics> stats)
            : this()
        {
            foreach (SufficientStatistics stat in stats)
            {
                Add(stat);
            }
        }

        /// <summary>
        /// Get Missing Instance.
        /// </summary>
        /// <returns>Statistics List.</returns>
        public static StatisticsList GetMissingInstance
        {
            get
            {
                return StatisticsList.GetInstance(MissingStatistics.GetInstance);
            }
        }

        /// <summary>
        /// Get Instance.
        /// </summary>
        /// <param name="stats">Variable number of Sufficient Statistics input.</param>
        /// <returns></returns>
        public static StatisticsList GetInstance(params SufficientStatistics[] stats)
        {
            return new StatisticsList(stats);
        }

        /// <summary>
        /// Get Instance.
        /// </summary>
        /// <param name="stats">Collection of Sufficient Statistics.</param>
        /// <returns>Statistics List.</returns>
        public static StatisticsList GetInstance(IEnumerable<SufficientStatistics> stats)
        {
            return new StatisticsList(stats);
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
                return false;
            }
                        
            string[] fields = val.Split(SEPARATOR);
            if (fields.Length < 2)
                return false;

            List<SufficientStatistics> stats = new List<SufficientStatistics>();
            foreach (string stat in fields)
            {
                stats.Add(SufficientStatistics.Parse(stat));
            }
            result = GetInstance(stats);
            return true;
        }

        /// <summary>
        /// Add Sufficient Statistics.
        /// </summary>
        /// <param name="statsToAdd">Stats To Add.</param>
        public void Add(SufficientStatistics statsToAdd)
        {
            if (statsToAdd == null)
            {
                throw new ArgumentNullException("statsToAdd");
            }
            _isMissing = (Count == 0 ? statsToAdd.IsMissing() : _isMissing || statsToAdd.IsMissing());
            _hashCode = null;

            StatisticsList asList = statsToAdd as StatisticsList;
            if (asList != null)
            {
                _stats.AddRange(asList._stats);
            }
            else
            {
                _stats.Add(statsToAdd);
            }
        }

        /// <summary>
        /// Add two Sufficient Statistics.
        /// </summary>
        /// <param name="stats1">Sufficient Statistics 1.</param>
        /// <param name="stats2">Sufficient Statistics 2.</param>
        /// <returns>Returns the Addition of all.</returns>
        public static StatisticsList Add(SufficientStatistics stats1, SufficientStatistics stats2)
        {
            StatisticsList newList = StatisticsList.GetInstance(stats1);
            newList.Add(stats2);
            return newList;
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
        /// Missing Statistics to Statistics List converter.
        /// </summary>
        /// <param name="missing">Missing Statistics.</param>
        /// <returns>Returns the converted type.</returns>
        public static implicit operator StatisticsList(MissingStatistics missing)
        {
            if (missing == null)
            {
                return null;
            }

            return StatisticsList.GetMissingInstance;
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

            StatisticsList other = stats.AsStatisticsList();
            if (other._stats.Count != _stats.Count || GetHashCode() != other.GetHashCode())
            {
                return false;
            }

            for (int i = 0; i < _stats.Count; i++)
            {
                if (!_stats[i].Equals(other._stats[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the Hash code.
        /// </summary>
        /// <returns>Returns the Hash code.</returns>
        public override int GetHashCode()
        {
            if (_hashCode == null)
            {
                if (IsMissing())
                {
                    _hashCode = MissingStatistics.GetInstance.GetHashCode();
                }
                _hashCode = 0;
                foreach (SufficientStatistics stat in _stats)
                {
                    _hashCode ^= stat.GetHashCode();
                }
            }
            return (int)_hashCode;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>Returns string.</returns>
        public override string ToString()
        {
            if (Count == 0)
            {
                return "Missing";
            }

            // may still be missing, but enumerate all anyways. one will be missing, but we'll be able to recover the full set.
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            foreach (SufficientStatistics stat in _stats)
            {
                if (!isFirst)
                {
                    sb.Append(SEPARATOR);
                }
                sb.Append(stat.ToString());
                isFirst = false;
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override StatisticsList AsStatisticsList()
        {
            return this;
        }

        /// <summary>
        /// Converts current object As Statistics List.
        /// </summary>
        /// <returns>Statistics List.</returns>
        public override GaussianStatistics AsGaussianStatistics()
        {
            return IsMissing() ? GaussianStatistics.GetMissingInstance : Last.AsGaussianStatistics();
        }

        /// <summary>
        /// Converts current object As Continuous Statistics.
        /// </summary>
        /// <returns>Continuous Statistics.</returns>
        public override ContinuousStatistics AsContinuousStatistics()
        {
            return IsMissing() ? ContinuousStatistics.GetMissingInstance : Last.AsContinuousStatistics();
        }

        /// <summary>
        /// Converts current object As Discrete Statistics.
        /// </summary>
        /// <returns>Discrete Statistics.</returns>
        public override DiscreteStatistics AsDiscreteStatistics()
        {
            return IsMissing() ? DiscreteStatistics.GetMissingInstance : Last.AsDiscreteStatistics();
        }

        /// <summary>
        /// Converts current object As Boolean Statistics.
        /// </summary>
        /// <returns>Boolean Statistics.</returns>
        public override BooleanStatistics AsBooleanStatistics()
        {
            return IsMissing() ? BooleanStatistics.GetMissingInstance : Last.AsBooleanStatistics();
        }

        #region IEnumerable<SufficientStatistics> Members

        /// <summary>
        /// Gets Enumerator for Sufficient Statistics.
        /// </summary>
        /// <returns>Returns Enumerator of Sufficient Statistics.</returns>
        public IEnumerator<SufficientStatistics> GetEnumerator()
        {
            return _stats.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Gets Enumerator.
        /// </summary>
        /// <returns>Returns Enumerator.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _stats.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Gives the clone of Statistics List. 
        /// </summary>
        /// <returns>Returns clone of Statistics List.</returns>
        public object Clone()
        {
            StatisticsList result = new StatisticsList(_stats);
            return result;
        }

        /// <summary>
        /// Remove value from Sufficient Statistics.
        /// </summary>
        /// <param name="i">The index from SufficientStatistics to be removed.</param>
        /// <returns>Returns Sufficient Statistics.</returns>
        public SufficientStatistics Remove(int i)
        {
            SufficientStatistics stat = _stats[i];
            _stats.RemoveAt(i);
            if (stat.IsMissing())  // check to see if this was the reason we're missing
            {
                ResetIsMissing();
            }
            _hashCode = null;
            return stat;
        }

        /// <summary>
        /// Reset Is Missing.
        /// </summary>
        private void ResetIsMissing()
        {
            _isMissing = false;
            foreach (SufficientStatistics ss in _stats)
            {
                _isMissing |= ss.IsMissing();
            }
        }

        /// <summary>
        /// Remove Range.
        /// </summary>
        /// <param name="startPos">Starting position.</param>
        /// <param name="count">The no of Count to be removed.</param>
        public void RemoveRange(int startPos, int count)
        {
            _stats.RemoveRange(startPos, count);
            _hashCode = null;
            ResetIsMissing();
        }

        /// <summary>
        /// The Sub Sequence.
        /// </summary>
        /// <param name="start">Starting position.</param>
        /// <param name="count">The Count.</param>
        /// <returns>Sufficient Statistics.</returns>
        public SufficientStatistics SubSequence(int start, int count)
        {
            return count == 1 ? _stats[start] : new StatisticsList(_stats.GetRange(start, count));
        }
    }
}
