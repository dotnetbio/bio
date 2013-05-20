using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Bio.Util;
using Bio.Util.ArgumentParser;
using Microsoft.Hpc.Scheduler;
using Microsoft.Hpc.Scheduler.Properties;

namespace Bio.Hpc
{
    /// <summary>
    /// Holds the cluster submitter arguments
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(OnHpc)), XmlInclude(typeof(OnHpcAndWait))]
    public class ClusterSubmitterArgs : IParsable, ICloneable
    {
        #region private members
        private string _externalRemoteDirectoryName = null;
        #endregion

        #region public members
        /// <summary>
        /// The cluster name, or :auto: or $auto$ to take the cluster with the most idle cores.
        /// </summary>
        [Parse(ParseAction.Required)]
        public string Cluster { get; set; }

        /// <summary>
        /// The number of tasks that the work will be divided into.
        /// </summary>
        [Parse(ParseAction.Required)]
        public int TaskCount { get; set; }

        /// <summary>
        /// dir
        /// </summary>
        public string Dir = null;

        /// <summary>
        /// Relative dir
        /// </summary>
        public bool RelativeDir = false;

        /// <summary>
        /// List of nodes exclusion
        /// </summary>
        public List<string> NodeExclusionList = new List<string>();

        /// <summary>
        /// List of nodes to use
        /// </summary>
        public List<string> NodesToUseList = new List<string>();

        /// <summary>
        /// Exclusive or not
        /// </summary>
        public bool IsExclusive = false;

        /// <summary>
        /// Suppresseds default options when constructing the petask commandline.
        /// </summary>
        public bool MinimalCommandLine = true;

        /// <summary>
        /// Name
        /// </summary>
        public string Name;

        /// <summary>
        /// Maximum number of cores
        /// </summary>
        public int? MaximumNumberOfCores = null;

        /// <summary>
        /// Minimum number of cores
        /// </summary>
        public int? MinimumNumberOfCores = null;

        /// <summary>
        /// Maximum number of nodes
        /// </summary>
        public int? MaximumNumberOfNodes = null;

        /// <summary>
        /// Minimum number of nodes
        /// </summary>
        public int? MinimumNumberOfNodes = null;

        /// <summary>
        /// If true, will add a cleanup task that runs after all other tasks are finished (or failed or canceled)
        /// </summary>
        public bool Cleanup = true;

        /// <summary>
        /// Will only do cleanup and will not submit the other tasks.
        /// </summary>
        public bool OnlyDoCleanup = false;

        /// <summary>
        /// List of input files to be copied
        /// </summary>
        public List<string> CopyInputFiles = new List<string>();

        /// <summary>
        /// Cluster version
        /// </summary>
        public int Version = 3;

        /// <summary>
        /// Priority of the job
        /// </summary>
        public JobPriority Priority = JobPriority.Normal;

        /// <summary>
        /// Current user name with domain
        /// </summary>
        public string Username = HpcLib.GetCurrentUsernameWithDomain();

        /// <summary>
        /// Max tries to submit
        /// </summary>
        public int MaxSubmitTries = 1;

        /// <summary>
        /// Use MPI or not
        /// </summary>
        public bool UseMPI = false;

        /// <summary>
        /// Job template
        /// </summary>
        public string JobTemplate = null;

        /// <summary>
        /// EXE relative dir
        /// </summary>
        public string ExeRelativeDirectoryName = null;//specify just the last part of the the path, so that there should be no slashes

        /// <summary>
        /// EXE name
        /// </summary>
        [Parse(ParseAction.Optional)]
        public string ExeName = null;

        /// <summary>
        /// Number of cores per task
        /// </summary>
        [Parse(ParseAction.Optional)]
        public int? NumCoresPerTask = null;//each task will use this many processors/cores on a given node (range is typically 1 to 8)

        /// <summary>
        /// Task range collection
        /// </summary>
        [Parse(ParseAction.Optional)]
        public RangeCollection TaskRange = new RangeCollection(0, int.MaxValue - 1); //!!!!why???

        /// <summary>
        /// Program parameters
        /// </summary>
        [Parse(ParseAction.Ignore)]
        public string ProgramParams { get; set; }
       
        /// <summary>
        /// Job Id
        /// </summary>
        [Parse(ParseAction.Ignore)]
        public int JobID { get; set; }

        /// <summary>
        /// Gets external remote directory name
        /// </summary>
        [Parse(ParseAction.Ignore)]
        public string ExternalRemoteDirectoryName
        {
            get
            {
                if (_externalRemoteDirectoryName == null)
                {
                    if (RelativeDir)
                    {
                        Helper.CheckCondition<UnknownClusterException>(HpcLibSettings.KnownClusters.ContainsKey(Cluster),
                            "{0} is not a known cluster in {1}. Known clusters: {2}", Cluster, HpcLibSettings.SettingsFileName, HpcLibSettings.KnownClusters.Keys.StringJoin(","));
                        string basePath = HpcLibSettings.KnownClusters[Cluster].StoragePath;
                        _externalRemoteDirectoryName = Path.Combine(basePath, Dir);
                    }
                    else
                    {
                        _externalRemoteDirectoryName = Dir;
                    }
                }
                return _externalRemoteDirectoryName;
            }
        }

        /// <summary>
        /// Gets or Sets out dir name
        /// </summary>
        public string StdOutDirName { get; set; }

        /// <summary>
        /// Gets or Sets error  dir name
        /// </summary>
        public string StdErrDirName { get; set; }

        /// <summary>
        /// Gets out relative dir name
        /// </summary>
        public string StdOutRelativeDirName { get { return StdOutDirName.StartsWith(ExternalRemoteDirectoryName, StringComparison.CurrentCultureIgnoreCase) ? StdOutDirName.Substring(ExternalRemoteDirectoryName.Length + 1) : StdOutDirName; } }

        /// <summary>
        /// Gets error relative dir name
        /// </summary>
        public string StdErrRelativeDirName { get { return StdErrDirName.StartsWith(ExternalRemoteDirectoryName, StringComparison.CurrentCultureIgnoreCase) ? StdErrDirName.Substring(ExternalRemoteDirectoryName.Length + 1) : StdErrDirName; } }
        #endregion

        #region public methods
        /// <summary>
        /// Finalize parse
        /// </summary>
        public void FinalizeParse()
        {
            Helper.CheckCondition<ParseException>(Version == 3, "Only API version 3 is supported.");    // technically, I think version 2 api is the same, so we could probably relax this.
            Helper.CheckCondition<ParseException>(TaskCount > 0, "You must specify a positive task count using -TaskCount");
            Helper.CheckCondition<ParseException>(Cluster != null, "You must specify a cluster name using -cluster name");

            if (Dir == null)
            {
                Dir = Environment.CurrentDirectory;
                string username = HpcLib.GetCurrentUsername();
                Dir = username + Dir.Substring(2); // first 2 characters are the drive. e.g. D:
                RelativeDir = true;
            }
            if (ExeName == null)
                ExeName = SpecialFunctions.GetEntryOrCallingAssembly().GetName().Name;

            if (Cluster.Equals(":auto:", StringComparison.CurrentCultureIgnoreCase) || Cluster.Equals("$auto$", StringComparison.CurrentCultureIgnoreCase))
                Cluster = HpcLib.GetMostAppropriateCluster(TaskCount).Cluster;
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        public ClusterSubmitterArgs() { }

        /// <summary>
        /// Constructor with string arguments
        /// </summary>
        /// <param name="args">string of args</param>
        public ClusterSubmitterArgs(string args) : this(new CommandArguments(args)) { }

        /// <summary>
        /// Constructor with argument collection
        /// </summary>
        /// <param name="args">Argument Collection</param>
        public ClusterSubmitterArgs(ArgumentCollection args)
        {
            args.ParseInto(this, checkComplete: false);
            ProgramParams = args.ToString();    // put what's left into here.
        }

        /// <summary>
        /// Returns the string of program params and args
        /// </summary>
        /// <returns>params and args</returns>
        public override string ToString()
        {
            return CommandArguments.ToString(this) + " " + ProgramParams;
        }

        /// <summary>
        /// Compare obj to check either equal or not
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>equal or not</returns>
        public override bool Equals(object obj)
        {
            return obj is ClusterSubmitterArgs && this.ToString().Equals(obj.ToString());
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns>hash code</returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Attempts to connect the the scheduler specified by Cluster and retrieve the Job specified by JobID. Will throw a SchedulerException if either of those fails.
        /// </summary>
        /// <returns>scheduler job</returns>
        public ISchedulerJob GetV2Job()
        {
            ISchedulerJob job;

            IScheduler scheduler;
            HpcLib.TryConnect(Cluster, out scheduler).Enforce<SchedulerException>("Unable to connect to head node {0}", Cluster);
            HpcLib.TryGetJob(scheduler, Username, JobID, out job).Enforce<SchedulerException>("Unable to load jobID {0} for user {1}", JobID, Username);

            return job;
        }

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>object</returns>
        public virtual object Clone()
        {
            var result = this.MemberwiseClone() as ClusterSubmitterArgs;
            result.CopyInputFiles = new List<string>(this.CopyInputFiles);
            result.TaskRange = new RangeCollection(this.TaskRange);
            return result;
        }
        #endregion
    }
}
