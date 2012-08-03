using System;
using Microsoft.Hpc.Scheduler;
using Microsoft.Hpc.Scheduler.Properties;

namespace Bio.Hpc
{
    /// <summary>
    /// !!! This is untested.
    /// This class attempts to unify sever disparate attempts at listening to job state. It contains some new methods available in SP1 and should be much simpler.
    /// I haven't tried moving the old code to this, because the old code (mostly) works. However, I suspect that any new code would work best by building on this class.
    /// The two known places that could move this this are LogEntry.cs and ClusterLogViewer.WaitInternal()
    /// </summary>
    public class JobListener : IDisposable
    {
        #region public members
        /// <summary>
        /// Event On job state changed
        /// </summary>
        public event EventHandler OnJobStateChanged;
        /// <summary>
        /// Event on task state changed
        /// </summary>
        public event EventHandler OnTaskStateChanged;

        /// <summary>
        /// Gets job state
        /// </summary>
        public JobState JobState { get { return Job.State; } }

        /// <summary>
        /// Gets job counters
        /// </summary>
        public ISchedulerJobCounters JobCounters { get; private set; }

        /// <summary>
        /// Gets the status of connection
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Gets scheduler
        /// </summary>
        public IScheduler Scheduler { get; private set; }

        /// <summary>
        /// Gets job
        /// </summary>
        public ISchedulerJob Job { get; private set; }
        #endregion

        #region public methods
        /// <summary>
        /// Returns either job is completed or not
        /// </summary>
        public bool JobIsDone
        {
            get
            {
                return JobState == JobState.Finished || JobState == JobState.Canceled || JobState == JobState.Failed;
            }
        }

        /// <summary>
        /// Connect to HPC
        /// </summary>
        /// <param name="schedulerName">scheduler name</param>
        /// <param name="jobID">job id</param>
        /// <param name="usernameOrNull">user name</param>
        /// <param name="jobListener">job listener</param>
        /// <returns></returns>
        public static bool TryConnect(string schedulerName, int jobID,  string usernameOrNull, out JobListener jobListener)
        {
            IScheduler scheduler;
            ISchedulerJob job;
            if (HpcLib.TryConnect(schedulerName, out scheduler) && HpcLib.TryGetJob(scheduler, usernameOrNull, jobID, out job))
            {
                jobListener = new JobListener(scheduler, job);
                return true;
            }
            else
            {
                jobListener = null;
                return false;
            }
        }

        /// <summary>
        /// Update the state of Job
        /// </summary>
        public void UpdateJobState()
        {
            if (IsConnected)
            {
                var oldJobState = Job.State;
                var oldCounters = JobCounters;

                Job.Refresh();
                JobCounters = Job.GetCounters();

                if (oldJobState != JobState)
                    RaiseJobStateChangedEvent();
                if (oldCounters == null || !oldCounters.Equals(JobCounters))
                    RaiseTaskStateChangedEvent();
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Scheduler.Dispose();
        }
        #endregion

        #region private methods
        private JobListener(IScheduler scheduler, ISchedulerJob job)
        {
            IsConnected = true;
            Scheduler = scheduler;
            Scheduler.OnSchedulerReconnect += (sender, eventArgs) =>
            {
                if (eventArgs.Code == ConnectionEventCode.StoreReconnect)
                {
                    UpdateJobState();
                    IsConnected = true;
                }
                else if (eventArgs.Code == ConnectionEventCode.StoreDisconnect)
                {
                    IsConnected = false;
                }
            };

            Job = job;
            Job.OnJobState += (sender, e) => UpdateJobState();
            Job.OnTaskState += (sender, e) => UpdateJobState();
            UpdateJobState();
        }
        
        private void RaiseJobStateChangedEvent()
        {
            //_jobStateChangedSinceLastEvent = false;
            if (OnJobStateChanged != null)
            {
                OnJobStateChanged(this, new EventArgs());
            }
        }

        private void RaiseTaskStateChangedEvent()
        {
            //_taskStateChangedSinceLastEvent = false;
            if (OnTaskStateChanged != null)
            {
                OnTaskStateChanged(this, new EventArgs());
            }
        }
        #endregion
    }
}
