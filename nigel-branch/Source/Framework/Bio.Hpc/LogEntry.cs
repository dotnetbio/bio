using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Bio.Util;
using Microsoft.Hpc.Scheduler;
using Microsoft.Hpc.Scheduler.Properties;

namespace Bio.Hpc
{
    /// <summary>
    /// Logs the result of any jobs
    /// </summary>
    [Serializable]
    public class LogEntry : IComparable<LogEntry>
    {
        #region public members
        /// <summary>
        /// Event on job state changed
        /// </summary>
        public event EventHandler OnJobStateChanged;

        /// <summary>
        /// Even on task state changed
        /// </summary>
        public event EventHandler OnTaskStateChanged;

        /// <summary>
        /// Enum of result status
        /// </summary>
        [Flags]
        public enum UpdateResult
        {
            /// <summary>
            /// Same state
            /// </summary>
            NoChange = 0, 
            /// <summary>
            /// Job state changed
            /// </summary>
            JobStateChanged = 1, 
            /// <summary>
            /// Task status changed
            /// </summary>
            TaskStatusChanged = 2
        };

        /// <summary>
        /// Gets or Sets log index
        /// </summary>
        [XmlIgnore]
        public int LogIndex { get; set; }

        /// <summary>
        /// Gets or Sets log date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or Sets local dir
        /// </summary>
        public string LocalDir { get; set; }

        /// <summary>
        /// Gets or Sets cluster args
        /// </summary>
        public ClusterSubmitterArgs ClusterArgs { get; set; }

        /// <summary>
        /// Gets or Sets copied to local dir
        /// </summary>
        public bool CopiedToLocalDir { get; set; }

        /// <summary>
        /// Gets or Sets task status
        /// </summary>
        public string TaskStatus { get; set; }

        /// <summary>
        /// Gets or Sets job state
        /// </summary>
        public JobState JobState { get; set; }

        /// <summary>
        /// Gets or Sets failed task count
        /// </summary>
        public int FailedTaskCount { get; set; }

        /// <summary>
        /// Gets or Sets failed tasks
        /// </summary>
        public string FailedTasks { get; set; }

        /// <summary>
        /// Gets or Sets wall time
        /// </summary>
        [XmlIgnore]
        public TimeSpan WallTime { get; set; }

        /// <summary>
        /// Gets or Sets wall time for saving
        /// </summary>
        public string WallTimeForSaving
        {
            get { return WallTime.ToString(); }
            set { WallTime = TimeSpan.Parse(value); }
        }

        /// <summary>
        /// Gets or Sets CPU time
        /// </summary>
        [XmlIgnore]
        public TimeSpan CpuTime { get; set; }

        /// <summary>
        /// Gets or Sets CPU time for saving
        /// </summary>
        public string CpuTimeForSaving
        {
            get { return CpuTime.ToString(); }
            set { CpuTime = TimeSpan.Parse(value); }
        }

        /// <summary>
        /// Gets job Id
        /// </summary>
        public int JobID { get { return ClusterArgs.JobID; } }

        /// <summary>
        /// Gets job name
        /// </summary>
        public string JobName { get { return ClusterArgs.Name; } }

        /// <summary>
        /// Gets cluster
        /// </summary>
        public string Cluster { get { return ClusterArgs.Cluster; } }

        /// <summary>
        /// Gets job template
        /// </summary>
        public string JobTemplate { get { return ClusterArgs.JobTemplate; } }

        /// <summary>
        /// Gets cluster dir
        /// </summary>
        public string ClusterDir { get { try { return ClusterArgs.ExternalRemoteDirectoryName; } catch (UnknownClusterException) { return ""; } } }

        /// <summary>
        /// Gets std out dir
        /// </summary>
        public string StdOutDir { get { return ClusterArgs.StdOutDirName; } }

        /// <summary>
        /// Gets std error dir
        /// </summary>
        public string StdErrDir { get { return ClusterArgs.StdErrDirName; } }

        /// <summary>
        /// Gets either it's failed or canceled
        /// </summary>
        public bool FailedOrCanceled
        {
            get
            {
                return this.JobState == JobState.Failed || JobState == JobState.Canceled || FailedTaskCount > 0 || !string.IsNullOrEmpty(FailedTasks);
            }
        }

        /// <summary>
        /// project
        /// </summary>
        string _project;
        /// <summary>
        /// Gets project
        /// </summary>
        public string Project
        {
            get
            {
                if (_project == null && ClusterDir != "")
                    _project = new DirectoryInfo(ClusterDir).Name;
                return _project;
            }
        }


        #endregion

        #region public methods
        /// <summary>
        /// Formatted CPU time
        /// </summary>
        public string FormattedCpuTime
        {
            get { return FormatTime(CpuTime); }
        }

        /// <summary>
        /// Formatted wall time
        /// </summary>
        public string FormattedWallTime
        {
            get { return FormatTime(WallTime); }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public LogEntry() { }
        
        /// <summary>
        /// Constructor with cluster args
        /// </summary>
        /// <param name="clusterArgs">Cluster submitter args</param>
        public LogEntry(ClusterSubmitterArgs clusterArgs)
        {
            Date = DateTime.Now;
            LocalDir = Environment.CurrentDirectory;
            if (clusterArgs.RelativeDir)
            {
                clusterArgs.Dir = clusterArgs.ExternalRemoteDirectoryName;
                clusterArgs.RelativeDir = false;
            }
            ClusterArgs = clusterArgs;
        }

        /// <summary>
        /// Get instance from job id
        /// </summary>
        /// <param name="clustername">cluster name</param>
        /// <param name="jobID">job id</param>
        /// <returns></returns>
        public static LogEntry GetInstanceFromJobID(string clustername, int jobID)
        {
            LogEntry entry = new LogEntry();
            entry.ClusterArgs = new ClusterSubmitterArgs();
            entry.ClusterArgs.Cluster = clustername;
            entry.ClusterArgs.JobID = jobID;

            JobListener.TryConnect(clustername, jobID, null, out entry._jobListener).Enforce("Unable to recover job {0} from cluster {1}.", jobID, clustername);

            //entry.Connect();
            //entry._job = entry.GetJob();
            //entry.ClusterArgs.Name = entry._job.Name;
            entry.Date = entry._jobListener.Job.SubmitTime;


            ISchedulerTask exampleTask = entry._jobListener.Job.GetTaskList(null, null, true).Cast<ISchedulerTask>().First();
            entry.ClusterArgs.StdErrDirName = exampleTask.StdErrFilePath;
            entry.ClusterArgs.StdOutDirName = exampleTask.StdOutFilePath;
            entry.ClusterArgs.Dir = exampleTask.WorkDirectory;

            string clusterPath = entry.ClusterDir.ToLower();
            string rootClusterPath = Path.Combine(HpcLibSettings.KnownClusters[entry.Cluster].StoragePath, "username");
            string relativeDir = clusterPath.Replace(rootClusterPath.ToLower(), "");
            if (!relativeDir.StartsWith("\\"))
                relativeDir = "\\" + relativeDir;
            entry.LocalDir = @"d:\projects" + relativeDir;

            return entry;
        }

        /// <summary>
        /// Start tracking job
        /// </summary>
        /// <returns></returns>
        public bool StartTrackingJob()
        {
            if (HpcLibSettings.ActiveClusters.Contains(Cluster, StringComparer.CurrentCultureIgnoreCase) && JobListener.TryConnect(Cluster, ClusterArgs.JobID, ClusterArgs.Username, out _jobListener))
            {
                _jobListener.OnJobStateChanged += (o, e) => { RefreshJobStatus(); RaiseJobStateChangedEvent(); };
                _jobListener.OnTaskStateChanged += (o, e) => { RefreshJobStatus(); RaiseTaskStateChangedEvent(); };
                if (JobState != _jobListener.JobState)
                {
                    RefreshJobStatus();
                    RaiseJobStateChangedEvent();
                    RaiseTaskStateChangedEvent();
                }
                else if (JobState == Microsoft.Hpc.Scheduler.Properties.JobState.Running)
                {
                    RefreshJobStatus();
                    RaiseTaskStateChangedEvent();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Refreshes job status
        /// </summary>
        public void RefreshJobStatus()
        {
            if (_jobListener != null)
            {
                LogEntry oldVersion = (LogEntry)this.MemberwiseClone();

                JobState = _jobListener.JobState;
                ISchedulerJobCounters counters = _jobListener.JobCounters; //_job.GetCounters();
                string stateStr = string.Format("{0}/{1}/{2}/{3}/{4}", counters.QueuedTaskCount, counters.RunningTaskCount, counters.FailedTaskCount, counters.CanceledTaskCount, counters.FinishedTaskCount);
                FailedTaskCount = counters.FailedTaskCount;
                TaskStatus = stateStr;

                if (FailedTaskCount > 0)
                {
                    IEnumerable<ISchedulerTask> tasklist = ClusterSubmitter.GetFailedAndCanceledTasks(_jobListener.Scheduler, _jobListener.Job);
                    string failedTaskRangeAsString = tasklist.Select(task => task.TaskId.JobTaskId).StringJoin(",");
                    if ("" != failedTaskRangeAsString)
                    {
                        this.FailedTasks = RangeCollection.Parse(failedTaskRangeAsString).ToString();
                    }
                    else
                    {
                        FailedTasks = "";
                    }
                }
                else
                {
                    FailedTasks = "";
                }

                if (_jobListener.JobState == JobState.Finished)
                {
                    if (WallTime.Ticks == 0)
                    {
                        DateTime startTime = _job.SubmitTime;
                        DateTime endTime = _job.EndTime;
                        WallTime = endTime - startTime;
                    }
                    if (CpuTime.Ticks == 0)
                    {
                        var tasklist = _job.GetTaskList(null, null, true).Cast<ISchedulerTask>();
                        var totalTicks = tasklist.Select(task => (task.EndTime - task.StartTime).Ticks).Sum();
                        CpuTime = new TimeSpan(totalTicks);
                    }
                }
            }
        }

        /// <summary>
        /// Requeues all failed or canceled tasks.
        /// </summary>
        /// <returns>True if all tasks were successfully Requeued.</returns>
        public bool RequeueFailedTasks()
        {
            if (_job != null)
            {
                try
                {
                    ClusterSubmitter.RequeueFailedAndCanceledTasks(_jobListener.Scheduler, _jobListener.Job);
                    FailedTaskCount = 0;
                    FailedTasks = "";
                    return true;
                }
                catch
                {

                }
            }
            return false;
        }

        /// <summary>
        /// Returns the formatted info in string
        /// </summary>
        /// <returns>details of cluster, data and job</returns>
        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", Date, Cluster, JobName, JobState);
        }

        /// <summary>
        /// Saves the log entries using file
        /// </summary>
        /// <param name="entries">list of entries</param>
        /// <param name="logFileName">log file name</param>
        public static void SaveEntries(List<LogEntry> entries, string logFileName)
        {
            FileStream filestream;
            if (FileUtils.TryToOpenFile(logFileName, new TimeSpan(0, 3, 0), FileMode.Create, FileAccess.ReadWrite, FileShare.None, out filestream))
            {
                using (TextWriter writer = new StreamWriter(filestream))
                {
                    SaveEntries(entries, writer);
                }
            }
        }

        /// <summary>
        /// Saves the log entries using text writer
        /// </summary>
        /// <param name="entries">list of entries</param>
        /// <param name="writer">text writer</param>
        public static void SaveEntries(List<LogEntry> entries, TextWriter writer)
        {
            entries.Sort();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<LogEntry>));
            serializer.Serialize(writer, entries);
        }

        /// <summary>
        /// Loads the log entries from the specified file name. If the file doesn't exist,
        /// returns an empty list.
        /// </summary>
        /// <param name="logFileName"></param>
        /// <param name="maxWaitTime">If we can't get a lock on the file in this amount of time, a TimeoutException will be thrown.</param>
        /// <exception cref="TimeoutException">If a lock can't be obtained in maxWaitTime</exception>
        /// <exception cref="IOException">If the file doesn't exist.</exception>
        /// <returns></returns>
        public static List<LogEntry> LoadEntries(string logFileName, TimeSpan maxWaitTime)
        {
            FileInfo file = new FileInfo(logFileName);
            if (!file.Exists || file.Length == 0)
            {
                return new List<LogEntry>();
            }

            FileStream filestream;
            if (FileUtils.TryToOpenFile(logFileName, maxWaitTime, FileMode.Open, FileAccess.Read, FileShare.Read, out filestream))
            {
                using (TextReader reader = new StreamReader(filestream))
                {
                    return LoadEntries(reader);
                }
            }
            if (!File.Exists(logFileName))
                throw new IOException("File " + logFileName + " does not exist.");
            else
                throw new TimeoutException("Unable to open log file.");
        }

        /// <summary>
        /// Loads entries from text reader
        /// </summary>
        /// <param name="reader">text reader</param>
        /// <returns>list of log entries</returns>
        public static List<LogEntry> LoadEntries(TextReader reader)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<LogEntry>));
            List<LogEntry> result = (List<LogEntry>)serializer.Deserialize(reader);
            result.Sort();
            return result;
        }

        /// <summary>
        /// Refreshes both the job state and the task status
        /// </summary>
        /// <returns>Flags specifying whether the JobStatus and/or TaskState changed.</returns>
        public UpdateResult RefreshJobStatus_old()
        {
            UpdateResult result = UpdateResult.NoChange;
            if ((string.IsNullOrEmpty(TaskStatus) ||
                    JobState != JobState.Finished && JobState != JobState.Canceled && JobState != JobState.Failed) &&
                _job != null)
            {
                try
                {
                    _job.Refresh();

                    if (JobState != _job.State)
                    {
                        JobState = _job.State;
                        result |= UpdateResult.JobStateChanged;
                    }
                    ISchedulerJobCounters counters = _job.GetCounters();
                    string stateStr = string.Format("{0}/{1}/{2}/{3}/{4}", counters.QueuedTaskCount, counters.RunningTaskCount, counters.FailedTaskCount, counters.CanceledTaskCount, counters.FinishedTaskCount);

                    if (FailedTaskCount != counters.FailedTaskCount || (string.IsNullOrEmpty(FailedTasks) != (FailedTaskCount == 0)))
                    {
                        UpdateFailedTaskString(_job);
                        result |= UpdateResult.TaskStatusChanged;
                    }

                    FailedTaskCount = counters.FailedTaskCount;
                    if (stateStr != TaskStatus)
                    {
                        TaskStatus = stateStr;
                        result |= UpdateResult.TaskStatusChanged;
                    }
                }
                catch
                {
                    // probably a problem with the scheduler.
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        /// Cancel job
        /// </summary>
        /// <returns></returns>
        public string CancelJob()
        {
            if (_job != null)
            {
                try
                {
                    _scheduler.CancelJob(_job.Id, "Cancelled from Cluster Log Viewer.");
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            return "";
        }

        /// <summary>
        /// Compares to other log entry
        /// </summary>
        /// <param name="other">log entry</param>
        /// <returns></returns>
        public int CompareTo(LogEntry other)
        {
            return other.Date.CompareTo(this.Date);
        }

        #endregion

        #region private methods
        private IScheduler _scheduler { get { return _jobListener == null ? null : _jobListener.Scheduler; } }
        
        private ISchedulerJob _job { get { return _jobListener == null ? null : _jobListener.Job; } }
        
        private JobListener _jobListener;
       
        private void RaiseJobStateChangedEvent()
        {
            if (OnJobStateChanged != null)
            {
                OnJobStateChanged(this, new EventArgs());
            }
        }

        private void RaiseTaskStateChangedEvent()
        {
            if (OnTaskStateChanged != null)
            {
                OnTaskStateChanged(this, new EventArgs());
            }
        }
        
        private void UpdateFailedTaskString(ISchedulerJob job)
        {
            IEnumerable<ISchedulerTask> tasklist = ClusterSubmitter.GetFailedAndCanceledTasks(_jobListener.Scheduler, job);
            this.FailedTasks = tasklist.Select(task => task.TaskId.JobTaskId).StringJoin(",");
        }

        private string FormatTime(TimeSpan time)
        {
            return string.Format("{0} {1:00}:{2:00}:{3:00}", time.Days > 0 ? time.Days + "d" : "", time.Hours, time.Minutes, time.Seconds);
        }
        #endregion
    }
}
