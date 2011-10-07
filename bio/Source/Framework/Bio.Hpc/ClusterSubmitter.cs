using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bio.Hpc.Properties;
using Bio.Util;
using Bio.Util.ArgumentParser;
using Bio.Util.Distribute;
using Microsoft.Hpc.Scheduler;
using Microsoft.Hpc.Scheduler.Properties;

namespace Bio.Hpc
{
    /// <summary>
    /// Allows other EXEs to easily submit themselves to the cluster.
    /// </summary>
    /// <remarks>
    /// Typical usage: in main class, test for the optional argument -cluster. If found, call this method. Example from PhyloD:
    /// <code>
    ///if (ArgumentCollection.ExtractOptionalFlag("cluster"))
    ///{
    ///    ClusterSubmitter.Submit(args, CheckForSkipFileAndTabulateFile, PhyloDRun.Validate);
    ///    return;
    ///}
    ///</code>
    /// </remarks>
    public static class ClusterSubmitter
    {
        #region public methods
        
        /// <summary>
        /// Single submitter object
        /// </summary>
        public static object _submitterLockObj = new object();

        /// <summary>
        /// Calls the corresponding Submit function, but waits for the cluster to Finish, Fail, or be Canceled. If the final state is
        /// Finished, returns silently. Otherwise, it throws and Exception. For a description of the other parameters, see Submit().
        /// *** NOTE: ONLY WORKS WITH V2 CLUSTERS. ****
        /// </summary>
        public static ClusterSubmitterArgs SubmitAndWait(ArgumentCollection argumentCollection, int maxSubmitAfterTasksFail = 0)
        {
            if (argumentCollection.PeekOptional<string>("cluster", "help").Equals("help", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("");
                Console.WriteLine(ArgumentCollection.CreateHelpMessage(typeof(ClusterSubmitterArgs), includeDateStamp: false));
                return null;
            }

            ClusterSubmitterArgs clusterArgs = new ClusterSubmitterArgs(argumentCollection);
            SubmitAndWait(clusterArgs, argumentCollection, maxSubmitAfterTasksFail);
            return clusterArgs;
        }

        /// <summary>
        /// Calls the corresponding Submit function, but waits for the cluster to Finish, Fail, or be Canceled. If the final state is
        /// Finished, returns silently. Otherwise, it throws and Exception. For a description of the other parameters, see Submit().
        /// </summary>
        public static void SubmitAndWait(ClusterSubmitterArgs clusterArgs, ArgumentCollection applicationArgs, int maxSubmitAfterTasksFail = 0)
        {
            CommandArguments cmd = applicationArgs as CommandArguments;
            Helper.CheckCondition<ArgumentException>(cmd != null, "Can only provide command arguments to the cluster submitter");

            SubmitAndWait(clusterArgs, new DistributableWrapper(cmd), maxSubmitAfterTasksFail: maxSubmitAfterTasksFail);
        }

        /// <summary>
        /// Submits the jobs and waits for it to complete. 
        /// When it submits, it create a log entry file in the cluster working directory, named according to the run name. This file is deleted
        /// when the job finishes successfully, so long as we're still waiting for it to finish. If SubmitAndWait is called and this file already 
        /// exists, then it is assumed that the job we want to submit was already submitted, so we wait for it to finish rather than submit again.
        /// </summary>
        /// <param name="clusterArgs"></param>
        /// <param name="distributableObj"></param>
        /// <param name="maxSubmitAfterTasksFail"></param>
        /// <param name="OnSubmittedCallbackOrNull"></param>
        public static void SubmitAndWait(ClusterSubmitterArgs clusterArgs, IDistributable distributableObj, int maxSubmitAfterTasksFail = 0, Action OnSubmittedCallbackOrNull = null)
        {
            using (ParallelOptionsScope.Suspend())
            {
                FileInfo logEntryFile = HpcLibSettings.GetLogEntryFile(clusterArgs);
                if (logEntryFile.Exists)
                {
                    Console.WriteLine(Resource.Job_already_exists, logEntryFile.FullName);
                    clusterArgs = HpcLibSettings.LoadLogEntryFile(logEntryFile).ClusterArgs;
                }
                else
                {
                    Submit(clusterArgs, distributableObj);
                    Console.WriteLine(Resource.Wait_Writing_log);
                    HpcLibSettings.WriteLogEntryToClusterDirectory(clusterArgs);
                }

                if (OnSubmittedCallbackOrNull != null)
                    OnSubmittedCallbackOrNull();

                JobState jobState = WaitForJobInternal(clusterArgs, maxSubmitAfterTasksFail);

                logEntryFile.Delete();  // job finished successfully, so we can delete this. Even if failed or canceled, we assume that we'll want to overwrite in the future.

                if (jobState != JobState.Finished)
                {
                    throw new Exception("Job " + jobState);
                }
            }
        }

        /// <summary>
        /// Waits for the job specified by clusterArgs to finish. If the state is Canceled or Failed,
        /// will throw an exception. Attempts to connect to the cluster if not already connected.
        /// </summary>
        /// <param name="clusterArgs">cluster args</param>
        /// <param name="maxNumTimesToResubmitFailedTasks">max number of times to resubmit</param>
        public static void WaitForJob(ClusterSubmitterArgs clusterArgs, int maxNumTimesToResubmitFailedTasks = 0)
        {
            JobState jobState = WaitForJobInternal(clusterArgs, maxNumTimesToResubmitFailedTasks);

            if (jobState == Microsoft.Hpc.Scheduler.Properties.JobState.Canceled)
            {
                throw new Exception("Job canceled.");
            }
            else if (jobState == Microsoft.Hpc.Scheduler.Properties.JobState.Failed)
            {
                throw new Exception("Job failed.");
            }
        }

        /// <summary>
        /// Requeues failed and canceled tasks for the given job. First checks that there are any, otherwise won't do anything.
        /// This is thread safe, locking on the job.
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="job"></param>
        public static void RequeueFailedAndCanceledTasks(IScheduler scheduler, ISchedulerJob job)
        {
            lock (job)
            {
                job.Refresh();
                var counters = job.GetCounters();
                if (counters.FailedTaskCount > 0 || counters.CanceledTaskCount > 0)
                {
                    var failedTasks = GetFailedAndCanceledTasks(scheduler, job);
                    foreach (ISchedulerTask task in failedTasks)
                    {
                        job.RequeueTask(task.TaskId);
                    }

                    if (job.State != Microsoft.Hpc.Scheduler.Properties.JobState.Running)
                    {
                        scheduler.ConfigureJob(job.Id);
                        scheduler.SubmitJob(job, null, null);
                    }
                }
            }
        }

        /// <summary>
        /// GetFailedAndCanceledTasks
        /// </summary>
        /// <param name="scheduler">scheduler</param>
        /// <param name="job">job</param>
        /// <returns>ISchedulerTask</returns>
        public static List<ISchedulerTask> GetFailedAndCanceledTasks(IScheduler scheduler, ISchedulerJob job)
        {
            IFilterCollection filters = scheduler.CreateFilterCollection();
            filters.Add(FilterOperator.Equal, PropId.Task_State, TaskState.Failed);
            var failedTasks = job.GetTaskList(filters, null, true).Cast<ISchedulerTask>().ToList();

            filters = scheduler.CreateFilterCollection();
            filters.Add(FilterOperator.Equal, PropId.Task_State, TaskState.Canceled);
            var canceledTasks = job.GetTaskList(filters, null, true).Cast<ISchedulerTask>().ToList();

            return failedTasks.Append(canceledTasks).ToList();
        }

        /// <summary>
        /// Submits the ArgumentCollection to the cluster, telling the cluster to run whichever exe is currently running using a new set of args that divids the work up in to tasks. 
        /// IMPORTANT: For this to work, the calling exe needs to except two OPTIONAL args:
        ///     1) -TaskCount n  which tells the exe how many pieces to divide the work in to
        ///     2) -Tasks i which tells the exe which piece that instance is in change of. Submit() will create TaskCount instances of the exe and do a parameter sweek on Tasks
        /// For the complete list of optional and required args, see the HelpString.
        /// </summary>
        /// <param name="argumentCollection">The set of args, including the args for the exe and the args for the cluster submission, all of which are encoded as optional args (though some are required)
        /// check to see if this job should actually be submitted (if so, return true, otherwise, false). This is useful to see if the results are already computed and if a completedRows
        /// file should be made into a skipFile automatically. See PhyloDMain.cs for an example.  Note that the out parameter is the ArgumentCollection that will actually be used to run the jobs.
        /// This is so the function can add in a new SkipFile parameter if needed. Also, be careful: often times checking to see if you need a skipFile requires reading arguments from
        /// the input args, which will modify that object. Your first step should be to Clone args and asign the result to argsToUseWithPossiblyUpdatedSkipFile.</param>
        public static ClusterSubmitterArgs Submit(ArgumentCollection argumentCollection)
        {
            if (argumentCollection.PeekOptional<string>("cluster", "help").Equals("help", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("");
                Console.WriteLine(ArgumentCollection.CreateHelpMessage(typeof(ClusterSubmitterArgs), includeDateStamp: false));
                return null;
            }

            ClusterSubmitterArgs clusterArgs = new ClusterSubmitterArgs(argumentCollection);
            Submit(clusterArgs, argumentCollection);
            return clusterArgs;
        }

        /// <summary>
        /// Submits the ArgumentCollection and ClusterArgs to the cluster, telling the cluster to run whichever exe is currently running using a new set of args that divids the work up in to tasks.
        /// </summary>
        /// <param name="clusterArgs">cluster args</param>
        /// <param name="applicationArgs">application args</param>
        public static void Submit(ClusterSubmitterArgs clusterArgs, ArgumentCollection applicationArgs)
        {
            CommandArguments cmd = applicationArgs as CommandArguments;
            Helper.CheckCondition<ArgumentException>(cmd != null, "Can only provide command arguments to the cluster submitter");
            Submit(clusterArgs, new DistributableWrapper(cmd));
        }

        /// <summary>
        /// Submits the ArgumentCollection to the cluster, telling the cluster to run whichever exe is currently running using a new set of args that divids the work up in to tasks.
        /// </summary>
        /// <param name="clusterArgs">cluster args</param>
        /// <param name="distributableObj">distributable objects</param>
        public static void Submit(ClusterSubmitterArgs clusterArgs, IDistributable distributableObj)
        {
            for (int numTries = 0; numTries < clusterArgs.MaxSubmitTries; numTries++)
            {
                try
                {
                    SubmitInternal(clusterArgs, distributableObj);
                    return;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(Resource.Error_Submitting + clusterArgs.Cluster + ": " + exception.Message);
                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "numTry={0} of {1}", numTries, clusterArgs.MaxSubmitTries));
                    Console.WriteLine(exception.StackTrace);
                    Console.WriteLine(Resource.User_CluserHelp);
                    Thread.Sleep(new TimeSpan(0, 10, 0));
                }
            }
            throw new Exception("max number of cluster submitter tries (" + clusterArgs.MaxSubmitTries + ") exceeded");
        }

        /// <summary>
        /// Copies input files
        /// </summary>
        /// <param name="filesToCopy">list of input files</param>
        /// <param name="baseDestnDir">target dir</param>
        public static void CopyInputFiles(List<string> filesToCopy, string baseDestnDir)
        {
            HpcLib.CopyFiles(filesToCopy, Environment.CurrentDirectory, baseDestnDir);
        }
        #endregion

        #region private methods

        private static JobState WaitForJobInternal(ClusterSubmitterArgs clusterArgs, int maxNumTimesToResubmitFailedTasks)
        {
            JobListener jobListener;
            JobListener.TryConnect(clusterArgs.Cluster, clusterArgs.JobID, clusterArgs.Username, out jobListener).Enforce("Could not connect to scheduler {0} or find jobID {1} for user {2}.",
                clusterArgs.Cluster, clusterArgs.JobID, clusterArgs.Username);

            ManualResetEvent mre = new ManualResetEvent(false);
            // setup a notification for when the job is done.
            jobListener.OnJobStateChanged += (o, e) =>
            {
                if (jobListener.JobIsDone)
                    mre.Set();
            };

            // setup the notification to requeue failed tasks.
            if (maxNumTimesToResubmitFailedTasks > 0)
            {
                jobListener.OnTaskStateChanged += (o, e) =>
                {
                    if (jobListener.JobCounters.FailedTaskCount > 0 && maxNumTimesToResubmitFailedTasks-- > 0)
                    {
                        Console.WriteLine(Resource.Tasks_failed);
                        RequeueFailedAndCanceledTasks(jobListener.Scheduler, jobListener.Job);
                    }
                };
            }

            if (!jobListener.JobIsDone)
            {
                mre.WaitOne();
            }

            return jobListener.JobState;
        }

        private static void SubmitInternal(ClusterSubmitterArgs clusterArgs, IDistributable distributableObj)
        {
            lock (_submitterLockObj)  // for now, just let one thread submit at a time.
            {
                if (string.IsNullOrEmpty(clusterArgs.Name))
                    clusterArgs.Name = distributableObj.JobName;

                CopyExes(clusterArgs);

                clusterArgs.StdErrDirName = CreateUniqueDirectory(clusterArgs.ExternalRemoteDirectoryName, "Stderr", distributableObj.JobName);
                clusterArgs.StdOutDirName = CreateUniqueDirectory(clusterArgs.ExternalRemoteDirectoryName, "Stdout", distributableObj.JobName);

                if (clusterArgs.CopyInputFiles != null)
                {
                    if (!(distributableObj is DistributableWrapper))
                        clusterArgs.CopyInputFiles.AddRange(ArgumentCollection.EnumerateValuesOfTypeFromParsable<InputFile>(distributableObj).Select(file => file.ToString()));

                    if (clusterArgs.CopyInputFiles.Count > 0)
                    {
                        CopyInputFiles(clusterArgs.CopyInputFiles, clusterArgs.ExternalRemoteDirectoryName);
                    }
                }

                using (ParallelOptionsScope.Suspend())
                {
                    switch (clusterArgs.Version)
                    {
                        case 3:
                            SubmitViaAPI3(clusterArgs, distributableObj);
                            break;
                        default:
                            throw new NotSupportedException(string.Format("Cluster version {0} is not supported.", clusterArgs.Version));
                    }
                }
                Console.WriteLine(Resource.Processed_job, clusterArgs.Cluster, clusterArgs.ExternalRemoteDirectoryName);


                Console.WriteLine(Resource.Writing_log_file);
                HpcLibSettings.TryWriteToLog(clusterArgs);

                Console.WriteLine(Resource.Done);
            }
            return;
        }

        private static void CopyExes(ClusterSubmitterArgs clusterArgs)
        {
            if (clusterArgs.ExeRelativeDirectoryName == null)
            {
                clusterArgs.ExeRelativeDirectoryName = HpcLib.CopyExesToCluster(clusterArgs.ExternalRemoteDirectoryName, clusterArgs.ExeName);
            }
            else
            {
                Console.WriteLine(Resource.Using_exe + clusterArgs.ExeRelativeDirectoryName);
                string absoluteExeDir = clusterArgs.ExternalRemoteDirectoryName + "\\" + clusterArgs.ExeRelativeDirectoryName;
                Helper.CheckCondition(Directory.Exists(absoluteExeDir), "Directory {0} does not exist!", absoluteExeDir);
            }
        }

        private static void OpenExplorerWindowAtCluster(string externalRemoteDirectoryName)
        {
            Process explorer = new Process();
            explorer.StartInfo.FileName = "explorer";
            explorer.StartInfo.Arguments = externalRemoteDirectoryName;
            explorer.Start();
        }

        private static void OpenJobConsole(string clusterName)
        {
            Process jobConsole = new Process();
            jobConsole.StartInfo.FileName = "HpcJobManager";
            jobConsole.StartInfo.Arguments = clusterName;
            jobConsole.Start();
        }

        #region API 3

        private static void SubmitViaAPI3(ClusterSubmitterArgs clusterArgs, IDistributable distributableObj)
        {
            Console.WriteLine(string.Format("Connecting to cluster {0} using API version 3 .", clusterArgs.Cluster));

            using (IScheduler scheduler = new Scheduler())
            {
                scheduler.Connect(clusterArgs.Cluster);
                ISchedulerJob job = scheduler.CreateJob();

                job.Name = distributableObj.JobName;
                job.Priority = clusterArgs.Priority;

                if (clusterArgs.JobTemplate != null)
                {
                    Microsoft.Hpc.Scheduler.IStringCollection jobTemplates = scheduler.GetJobTemplateList();
                    string decodedJobTemplate = System.Web.HttpUtility.UrlDecode(clusterArgs.JobTemplate);
                    if (jobTemplates.Contains(decodedJobTemplate))
                    {
                        job.SetJobTemplate(decodedJobTemplate);
                    }
                    else
                    {
                        Console.WriteLine(string.Format(Resource.Job_template, decodedJobTemplate));
                        foreach (var template in jobTemplates)
                        {
                            Console.Write("'" + template + "' ");
                        }
                        Console.WriteLine(Resource.SubmitViaAPI3);
                    }
                }


                if (clusterArgs.NumCoresPerTask != null)
                {
                    clusterArgs.IsExclusive = false;
                }

                IStringCollection nodesToUse = null;

                if (clusterArgs.NodeExclusionList != null && clusterArgs.NodeExclusionList.Count > 0)
                {
                    nodesToUse = GetNodesToUse(clusterArgs, scheduler, job);
                }
                else if (clusterArgs.NodesToUseList != null && clusterArgs.NodesToUseList.Count > 0)
                {
                    nodesToUse = scheduler.CreateStringCollection();
                    foreach (string nodeName in clusterArgs.NodesToUseList)
                    {
                        nodesToUse.Add(nodeName);
                    }
                }
                else if (clusterArgs.NumCoresPerTask != null)
                {
                    job.AutoCalculateMax = true;
                    job.AutoCalculateMin = true;
                }
                else if (clusterArgs.IsExclusive)
                {
                    job.UnitType = Microsoft.Hpc.Scheduler.Properties.JobUnitType.Node;
                    if (clusterArgs.MinimumNumberOfNodes != null)
                    {
                        job.MaximumNumberOfNodes = clusterArgs.MaximumNumberOfNodes.Value;
                        job.MinimumNumberOfNodes = clusterArgs.MinimumNumberOfNodes.Value;
                    }
                }
                else if (clusterArgs.MinimumNumberOfCores != null)
                {
                    if (clusterArgs.MaximumNumberOfCores == null)
                        job.AutoCalculateMax = true;
                    else
                    {
                        job.AutoCalculateMax = false;
                        job.MaximumNumberOfCores = clusterArgs.MaximumNumberOfCores.Value;
                    }
                    job.MaximumNumberOfCores = clusterArgs.MaximumNumberOfCores ?? Math.Max(clusterArgs.TaskCount, scheduler.GetCounters().TotalCores);
                    job.MinimumNumberOfCores = clusterArgs.MinimumNumberOfCores.Value;
                    job.AutoCalculateMin = false;
                }
                else
                {
                    job.AutoCalculateMax = true;
                    job.AutoCalculateMin = true;
                }

                if (!clusterArgs.OnlyDoCleanup)
                {

                    if (clusterArgs.TaskRange.IsContiguous())
                    {
                        if (clusterArgs.TaskRange.LastElement > clusterArgs.TaskCount - 1)
                            clusterArgs.TaskRange = new RangeCollection(clusterArgs.TaskRange.FirstElement, clusterArgs.TaskCount - 1);
                        ISchedulerTask task = CreateTask(null, clusterArgs, job, distributableObj, nodesToUse);

                        task.Type = TaskType.ParametricSweep;

                        task.StartValue = 0;
                        task.EndValue = clusterArgs.TaskCount - 1;

                        job.AddTask(task);
                    }
                    else
                    {
                        job.AddTasks(clusterArgs.TaskRange.Select(taskNum => CreateTask((int)taskNum, clusterArgs, job, distributableObj, nodesToUse)).ToArray());
                    }
                }
                else
                {
                    clusterArgs.Cleanup = true;
                }

                ISchedulerTask cleanupTask = null;
                if (clusterArgs.Cleanup)
                {
                    cleanupTask = AddCleanupTaskToJob(clusterArgs, scheduler, job, distributableObj);
                }

                Console.WriteLine(Resource.Submitting_job);
                scheduler.SubmitJob(job, null, null);
                clusterArgs.JobID = job.Id;
                Console.WriteLine(job.Name + Resource.submitted);
            }
        }

        private static IStringCollection GetNodesToUse(ClusterSubmitterArgs clusterArgs, IScheduler scheduler, ISchedulerJob job)
        {
            job.AutoCalculateMax = false;
            job.AutoCalculateMin = false;
            var availableNodes = scheduler.GetNodeList(null, null);
            IStringCollection nodesToUse = scheduler.CreateStringCollection();
            List<string> nodesFound = new List<string>();
            foreach (var node in availableNodes)
            {
                string nodeName = ((Microsoft.Hpc.Scheduler.SchedulerNode)node).Name;
                if (!clusterArgs.NodeExclusionList.Contains(nodeName))
                {
                    nodesToUse.Add(nodeName);
                }
                else
                {
                    nodesFound.Add(nodeName);
                }
            }
            Helper.CheckCondition(nodesFound.Count != clusterArgs.NodeExclusionList.Count, "not all nodes in exclusion list found: check for typo " + clusterArgs.NodeExclusionList);

            return nodesToUse;
        }

        private static ISchedulerTask AddCleanupTaskToJob(ClusterSubmitterArgs clusterArgs, IScheduler scheduler, ISchedulerJob job, IDistributable distributableJob)
        {
            ISchedulerCollection taskList = job.GetTaskList(scheduler.CreateFilterCollection(), scheduler.CreateSortCollection(), true);
            IStringCollection dependencyTasks = scheduler.CreateStringCollection();

            if (!clusterArgs.OnlyDoCleanup)
            {
                dependencyTasks.Add(((ISchedulerTask)taskList[0]).Name);
            }
            ISchedulerTask cleanupTask = CreateCleanupTask(job, clusterArgs.ExternalRemoteDirectoryName, clusterArgs.StdErrRelativeDirName, clusterArgs.StdOutRelativeDirName, "cleanup", isFinalCleanup: true);

            Locally local = new Locally()
            {
                Cleanup = true,
                TaskCount = clusterArgs.TaskCount,
                Tasks = new RangeCollection(),
                ParallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 1 }
            };

            DistributeApp.Distribute distributeExe = new DistributeApp.Distribute()
            {
                Distributor = local,
                Distributable = distributableJob
            };

            string exeName = distributableJob is DistributableWrapper ? clusterArgs.ExeName : distributeExe.GetType().Assembly.GetName().Name;

            string taskCommandLine = string.Format("{0}\\{1} {2}", clusterArgs.ExeRelativeDirectoryName, exeName, CreateTaskString(distributeExe, clusterArgs.MinimalCommandLine));
            cleanupTask.CommandLine = taskCommandLine;

            if (!clusterArgs.OnlyDoCleanup)
            {
                cleanupTask.DependsOn = dependencyTasks;
            }
            job.AddTask(cleanupTask);
            return cleanupTask;
        }


        private static ISchedulerTask CreateCleanupTask(ISchedulerJob job, string internalRemoteDirectoryName, string stdErrDirName, string stdOutDirName, string name, bool isFinalCleanup)
        {
            ISchedulerTask cleanupTask = job.CreateTask();

            cleanupTask.WorkDirectory = internalRemoteDirectoryName;
            cleanupTask.Name = name;
            cleanupTask.IsExclusive = false;
            cleanupTask.StdErrFilePath = string.Format(@"{0}\{1}.txt", stdErrDirName, name);
            cleanupTask.StdOutFilePath = string.Format(@"{0}\{1}.txt", stdOutDirName, name);

            if (isFinalCleanup)
            {

            }
            else
            {
                cleanupTask.CommandLine = "ECHO The cleanup task is running.";
            }

            return cleanupTask;
        }

        private static ISchedulerTask CreateTask(int? taskNumber, ClusterSubmitterArgs clusterArgs, ISchedulerJob job, IDistributable distributableObj, IStringCollection nodesToUse)
        {
            Locally local = new Locally()
            {
                Cleanup = false,
                TaskCount = clusterArgs.TaskCount,
                Tasks = taskNumber.HasValue ? new RangeCollection(taskNumber.Value) : null,
            };

            ISchedulerTask task = job.CreateTask();
            if (nodesToUse != null)
            {
                task.RequiredNodes = nodesToUse;
            }
            if (clusterArgs.NumCoresPerTask != null)
            {
                task.MinimumNumberOfCores = clusterArgs.NumCoresPerTask.Value;
                task.MaximumNumberOfCores = clusterArgs.NumCoresPerTask.Value;
                task.MaximumNumberOfNodes = 1;
                local.ParallelOptions.MaxDegreeOfParallelism = clusterArgs.NumCoresPerTask.Value;
            }
            if (!clusterArgs.IsExclusive)
            {
                local.ParallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 1 };
            }

            task.WorkDirectory = clusterArgs.ExternalRemoteDirectoryName;

            DistributeApp.Distribute distributeExe = new DistributeApp.Distribute()
            {
                Distributable = distributableObj,
                Distributor = local
            };

            string taskArgString = CreateTaskString(distributeExe, clusterArgs.MinimalCommandLine);
            string exeName = distributeExe.Distributable is DistributableWrapper ? clusterArgs.ExeName : distributeExe.GetType().Assembly.GetName().Name;

            string taskCommandLine = null;
            if (clusterArgs.UseMPI)
            {
                taskCommandLine = string.Format("mpiexec -n {0} {1}\\{2} {3}", clusterArgs.NumCoresPerTask, clusterArgs.ExeRelativeDirectoryName, exeName, taskArgString);
            }
            else
            {
                taskCommandLine = string.Format("{0}\\{1} {2}", clusterArgs.ExeRelativeDirectoryName, exeName, taskArgString);
            }
            task.CommandLine = taskCommandLine;

            string taskNumberAsString = taskNumber.HasValue ? taskNumber.Value.ToString() : "*";
            task.Name = Helper.CreateDelimitedString(" ", distributableObj.JobName, taskNumberAsString);
            Console.WriteLine(Resource.StdOutRelativeDirName + clusterArgs.StdOutRelativeDirName);
            task.StdErrFilePath = string.Format(@"{0}\{1}.txt", clusterArgs.StdErrRelativeDirName, taskNumberAsString);
            task.StdOutFilePath = string.Format(@"{0}\{1}.txt", clusterArgs.StdOutRelativeDirName, taskNumberAsString);

            Console.WriteLine(Resource.CreateTask, task.CommandLine.Length, task.CommandLine);
            if (task.StdErrFilePath.Length >= 160)
                Console.WriteLine(Resource.Caution, task.StdErrFilePath.Length);

            return task;
        }

        #endregion

        private static string CreateUniqueDirectory(string internalRemoteDirectoryName, string subDirectoryName, string dirNameBase)
        {
            int i = 0;
            string dirName;
            string cleanTimeString = GetCleanTimeString();

            do
            {
                dirName = Path.Combine(internalRemoteDirectoryName, string.Format(@"{0}\{1}_{2}{3}", subDirectoryName, dirNameBase, cleanTimeString, i == 0 ? "" : "_" + i));
                i++;
            } while (Directory.Exists(dirName));

            Directory.CreateDirectory(dirName);
            return dirName;
        }

        private static string GetCleanTimeString()
        {
            int hour = DateTime.Now.Hour;
            string hourString = hour < 12 ? hour + "a" : hour == 12 ? hour + "p" : hour - 12 + "p";
            string timeString = string.Format("{0}_{1}", DateTime.Now.ToShortDateString(), hourString);
            timeString = timeString.Replace("/", "");
            timeString = timeString.Replace(" ", "");
            timeString = timeString.Replace(':', '_');
            return timeString;
        }

        private static string CreateTaskString(DistributeApp.Distribute distributeExe, bool suppressDefaults)
        {
            DistributableWrapper wrapper = distributeExe.Distributable as DistributableWrapper;
            if (wrapper != null)
            {
                Locally local = (Locally)distributeExe.Distributor;

                string result = string.Format("{0} -TaskCount {1} -Tasks {2} -Cleanup {3}",
                    wrapper.ToString(),
                    local.TaskCount,
                    local.Tasks == null ? "*" : local.Tasks.ToString(),
                    local.Cleanup);

                return result;
            }
            else
            {
                string result = CommandArguments.ToString(distributeExe, suppressDefaults: true, protectWithQuotes: true);
                result = result.Replace("Tasks:null", "Tasks:*");
                return result;
            }
        }
        #endregion
    }
}
