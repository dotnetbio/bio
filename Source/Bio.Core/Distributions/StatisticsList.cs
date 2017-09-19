using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bio.Distributions
{
    /// <summary>
    /// Statistics List.
    /// </summary>
    public class StatisticsList : SufficientStatistics, IEnumerable<SufficientStatistics>
    {
        /// <summary>
        /// The separator.
        /// </summary>
        public const char Separator = ';';

        /// <summary>
        /// List of SufficientStatistics.
        /// </summary>
        private readonly List<SufficientStatistics> stats;

        /// <summary>
        /// IsMissing Flag.
        /// </summary>
        private bool isMissing;

        /// <summary>
        /// Hash Code.
        /// </summary>
        private int? hashCode;

        /// <summary>
        /// Sufficient Statistics indexer.
        /// </summary>
        /// <param name="idx">Index id.</param>
        /// <returns>Gets or sets the index based on index id.</returns>
        public SufficientStatistics this[int idx]
        {
            get
            {
                return this.stats[idx];
            }
            set
            {
                if (value != null)
                {
                    this.stats[idx] = value;
                    this.isMissing |= value.IsMissing();
                    this.hashCode = null;
                }
                else
                {
                    this.stats[idx] = null;
                    this.hashCode = null;
                }
            }
        }

        /// <summary>
        /// Stats count.
        /// </summary>
        public int Count
        {
            get { return this.stats.Count; }
        }

        /// <summary>
        /// Sufficient Statistics end.
        /// </summary>
        private SufficientStatistics Last
        {
            get { return this.stats[this.stats.Count - 1]; }
        }

        /// <summary>
        /// Statistics List constructor.
        /// </summary>
        public StatisticsList()
        {
            this.stats = new List<SufficientStatistics>();
            this.isMissing = true;
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
                return GetInstance(MissingStatistics.GetInstance);
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
                        
            string[] fields = val.Split(Separator);
            if (fields.Length < 2)
                return false;

            result = GetInstance(fields.Select(Parse).ToList());
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
            this.isMissing = (Count == 0 ? statsToAdd.IsMissing() : this.isMissing || statsToAdd.IsMissing());
            this.hashCode = null;

            StatisticsList asList = statsToAdd as StatisticsList;
            if (asList != null)
            {
                this.stats.AddRange(asList.stats);
            }
            else
            {
                this.stats.Add(statsToAdd);
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
            return this.isMissing;
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

            return GetMissingInstance;
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
            if (other.stats.Count != this.stats.Count || GetHashCode() != other.GetHashCode())
            {
                return false;
            }

            return !this.stats.Where((t, i) => !t.Equals(other.stats[i])).Any();
        }

        /// <summary>
        /// Gets the Hash code.
        /// </summary>
        /// <returns>Returns the Hash code.</returns>
        public override int GetHashCode()
        {
            if (this.hashCode == null)
            {
                if (IsMissing())
                {
                    this.hashCode = MissingStatistics.GetInstance.GetHashCode();
                }
                this.hashCode = 0;
                foreach (SufficientStatistics stat in this.stats)
                {
                    this.hashCode ^= stat.GetHashCode();
                }
            }
            return (int)this.hashCode;
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
            foreach (SufficientStatistics stat in this.stats)
            {
                if (!isFirst)
                {
                    sb.Append(Separator);
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
            return this.stats.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Gets Enumerator.
        /// </summary>
        /// <returns>Returns Enumerator.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.stats.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Gives the clone of Statistics List. 
        /// </summary>
        /// <returns>Returns clone of Statistics List.</returns>
        public object Clone()
        {
            StatisticsList result = new StatisticsList(this.stats);
            return result;
        }

        /// <summary>
        /// Remove value from Sufficient Statistics.
        /// </summary>
        /// <param name="i">The index from SufficientStatistics to be removed.</param>
        /// <returns>Returns Sufficient Statistics.</returns>
        public SufficientStatistics Remove(int i)
        {
            SufficientStatistics stat = this.stats[i];
            this.stats.RemoveAt(i);
            if (stat.IsMissing())  // check to see if this was the reason we're missing
            {
                ResetIsMissing();
            }
            this.hashCode = null;
            return stat;
        }

        /// <summary>
        /// Reset Is Missing.
        /// </summary>
        private void ResetIsMissing()
        {
            this.isMissing = false;
            foreach (SufficientStatistics ss in this.stats)
            {
                this.isMissing |= ss.IsMissing();
            }
        }

        /// <summary>
        /// Remove Range.
        /// </summary>
        /// <param name="startPos">Starting position.</param>
        /// <param name="count">The no of Count to be removed.</param>
        public void RemoveRange(int startPos, int count)
        {
            this.stats.RemoveRange(startPos, count);
            this.hashCode = null;
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
            return count == 1 ? this.stats[start] : new StatisticsList(this.stats.GetRange(start, count));
        }
    }
}
