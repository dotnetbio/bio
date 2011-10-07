using System;
using Microsoft.Hpc.Scheduler;

namespace Bio.Hpc
{
    /// <summary>
    /// Holds the status of cluster includes core info
    /// </summary>
    public class ClusterStatus
    {
        #region private members
        private IScheduler _scheduler;
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets the cluster name
        /// </summary>
        public string Cluster { get; set; }
        /// <summary>
        /// Gets or sets the number of idle cores
        /// </summary>
        public int IdleCores { get; set; }
        /// <summary>
        /// Gets or sets the number of busy cores
        /// </summary>
        public int BusyCores { get; set; }
        /// <summary>
        /// Gets or sets the number of queued tasks
        /// </summary>
        public int QueuedTasks { get; set; }
        #endregion properties

        #region public methods
        /// <summary>
        /// Constructor of ClusterStatus with cluster name
        /// </summary>
        /// <param name="clusterName">cluster name</param>
        public ClusterStatus(string clusterName)
        {
            Cluster = clusterName;
        }
        /// <summary>
        /// Returns the string of cluster, core info
        /// </summary>
        /// <returns>cluster info</returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}/{2}", Cluster, IdleCores, IdleCores + BusyCores);
        }
        /// <summary>
        /// Refreshes the cluster or core
        /// </summary>
        /// <returns>refreshed or not</returns>
        public bool Refresh()
        {
            bool changed = false;
            ISchedulerCounters counters;
            using (ParallelOptionsScope.Suspend())
            {
                if (Connect() && null != (counters = GetCounters()))
                {
                    changed =
                       BusyCores != counters.BusyCores ||
                       IdleCores != counters.IdleCores ||
                       QueuedTasks != counters.QueuedTasks;


                    BusyCores = counters.BusyCores;
                    IdleCores = counters.IdleCores;
                    QueuedTasks = counters.QueuedTasks;

                }
                else
                {
                    changed = BusyCores != -1;

                    BusyCores = -1;
                    IdleCores = -1;
                    QueuedTasks = -1;

                }
            }
            return changed;
        }
        #endregion

        #region private methods
        private ISchedulerCounters GetCounters()
        {
            try
            {
                ISchedulerCounters counters = _scheduler.GetCounters();
                return counters;
            }
            catch (Exception)
            {
                _scheduler = null;
                return null;
            }
        }

        private bool Connect()
        {
            if (_scheduler != null) return true;

            bool connected = false;

            using (ParallelOptionsScope.Suspend())
            {
                connected = HpcLib.TryConnect(Cluster, out _scheduler);
            }

            return connected;
        }
        #endregion
    }
}
