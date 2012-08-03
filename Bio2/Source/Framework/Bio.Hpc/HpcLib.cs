using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bio.Hpc.Properties;
using Bio.Util;
using Microsoft.Hpc.Scheduler;
using Microsoft.Hpc.Scheduler.Properties;

namespace Bio.Hpc
{
    /// <summary>
    /// This class takes care of the HPC connection and file transfer operations.
    /// </summary>
    public static class HpcLib
    {
        #region private members
        internal static string GetCurrentUsernameWithDomain()
        {
            string domain = Environment.GetEnvironmentVariable("USERDOMAIN");
            string username = Environment.GetEnvironmentVariable("USERNAME");
            return domain + "\\" + username;
        }

        internal static string GetCurrentUsername()
        {
            string username = Environment.GetEnvironmentVariable("USERNAME");
            return username;
        }
        #endregion

        #region public members
        /// <summary>
        /// Holds folder names and EXE folder name maps
        /// </summary>
        public static Dictionary<string, string> _dirNameToExeDirName = new Dictionary<string, string>();
        #endregion

        #region public methods
        /// <summary>
        /// For each pattern in filePatternsToCopy, will create and call xcopy /d /c /i baseSrcDir\pattern baseDestnDir\patternPath, 
        /// where patternPath is the path part of the pattern. Set baseDestnDir or baseSrcDir to the empty string to make it use
        /// the current working directory. 
        /// </summary>
        /// <param name="filePatternsToCopy">patten to be copied</param>
        /// <param name="baseSrcDir">source dir</param>
        /// <param name="baseDestnDir">target dir</param>
        public static void CopyFiles(IEnumerable<string> filePatternsToCopy, string baseSrcDir, string baseDestnDir)
        {
            foreach (string file in filePatternsToCopy)
            {
                string xcopyArgsFormatString = GetXCopyArgsFormatString(file, baseSrcDir, baseDestnDir);
                string srcFilePath = Path.Combine(baseSrcDir, file);
                if (!srcFilePath.Contains('*') && !File.Exists(srcFilePath) && !Directory.Exists(srcFilePath))
                {
                    Console.Error.WriteLine("File or directory {0} does not exist. Skipping...", srcFilePath);
                    continue;
                }

                Process proc = new Process();
                proc.StartInfo.FileName = "xcopy";
                proc.StartInfo.Arguments = string.Format(xcopyArgsFormatString, srcFilePath);

                proc.StartInfo.UseShellExecute = false;
                Console.WriteLine(Resource.xcopy + proc.StartInfo.Arguments);
                proc.Start();
                proc.WaitForExit();
                if (proc.ExitCode > 0)
                    throw new Exception("Unable to copy file using command xcopy " + proc.StartInfo.Arguments);
            }
        }

        /// <summary>
        /// Gets the arguments for XCopy
        /// </summary>
        /// <param name="exampleFileToCopy">file to be copied</param>
        /// <param name="baseSrcDir">source dir</param>
        /// <param name="baseDestnDir">target dir</param>
        /// <returns></returns>
        public static string GetXCopyArgsFormatString(string exampleFileToCopy, string baseSrcDir, string baseDestnDir)
        {
            string relativeDirName = FileUtils.GetDirectoryName(exampleFileToCopy, baseSrcDir);
            string outputDir = Path.Combine(baseDestnDir, relativeDirName);

            if (string.IsNullOrWhiteSpace(outputDir))
                outputDir = ".";

            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            string args = "/d /c /i /y \"{0}\" \"" + outputDir + "\"";
            return args;
        }

        /// <summary>
        /// Deletes the files in source dir
        /// </summary>
        /// <param name="filePatternsToDelete">file pattern to be deleted</param>
        /// <param name="baseDir">source dir</param>
        public static void DeleteFiles(IEnumerable<string> filePatternsToDelete, string baseDir)
        {
            foreach (string filePattern in filePatternsToDelete)
            {
                string directory = Path.Combine(baseDir, Path.GetDirectoryName(filePattern));
                string filePatternNoDir = Path.GetFileName(filePattern);
                foreach (string file in Directory.GetFiles(directory, filePatternNoDir))
                {
                    File.Delete(file);
                }
            }
        }

        /// <summary>
        /// Gets the machine name from the dir
        /// </summary>
        /// <param name="directoryName">source dir</param>
        /// <returns>machine name</returns>
        public static string GetMachineNameFromUNCName(string directoryName)
        {
            //!!!be nicer to use Regex
            Helper.CheckCondition(directoryName.StartsWith(@"\\"), "The directory name should start with '\\'");
            string[] partCollecton = directoryName.Substring(2).Split(new char[] { '\\' }, 2);
            Helper.CheckCondition(partCollecton.Length == 2, "Expected the directory name to have a machine part and a file part");
            return partCollecton[0];
        }

        /// <summary>
        /// Creates the new dir on specificed source dir
        /// </summary>
        /// <param name="niceName">prefix of dir</param>
        /// <param name="directoryName">dir name</param>
        /// <param name="newExeDirectoryName">EXE path</param>
        /// <param name="exeRelativeDirectoryName">EXE relative dir</param>
        public static void CreateNewExeDirectory(string niceName, string directoryName, out string newExeDirectoryName, out string exeRelativeDirectoryName)
        {
            string exeRelativeDirectoryNameMostly = string.Format(@"exes\{0}{1}", niceName, DateTime.Now.ToShortDateString().Replace("/", ""));
            for (int suffixIndex = 0; ; ++suffixIndex)
            {
                string suffix = suffixIndex == 0 ? "" : suffixIndex.ToString();
                newExeDirectoryName = string.Format(@"{0}\{1}{2}", directoryName, exeRelativeDirectoryNameMostly, suffix);
                //!!!Two instances of this program could (it is possible) create the same directory.
                if (!Directory.Exists(newExeDirectoryName))
                {
                    try
                    {
                        Directory.CreateDirectory(newExeDirectoryName);
                    }
                    catch (IOException e)
                    {
                        throw new IOException("Could not create directory " + newExeDirectoryName, e);
                    }
                    exeRelativeDirectoryName = exeRelativeDirectoryNameMostly + suffix;
                    break;
                }
            }
        }

        /// <summary>
        /// Copies the EXEs to cluster
        /// </summary>
        /// <param name="directoryName">target dir</param>
        /// <param name="niceName">dir name</param>
        /// <returns></returns>
        public static string CopyExesToCluster(string directoryName, string niceName)
        {
            string finalResult;
            //!! note that there is a small chance of a copy conflict with another process. Should come up with a safe way around that.
            if (!_dirNameToExeDirName.TryGetValue(directoryName, out finalResult))
            {
                Console.WriteLine(Resource.ExeAlreadyThere);
                string newExeDirectoryName;
                string exeNewRelativeDirectoryName;
                string executingExeDirectoryName = Path.GetDirectoryName(FileUtils.GetEntryOrCallingAssembly().Location);

                bool needToCopyExesToCluster = !ExesAreAlreadyOnCluster(niceName, directoryName, executingExeDirectoryName, out newExeDirectoryName, out exeNewRelativeDirectoryName);
                if (needToCopyExesToCluster)
                {
                    Console.WriteLine(Resource.Copying_Exes);
                    Directory.CreateDirectory(directoryName);
                    CreateNewExeDirectory(niceName, directoryName, out newExeDirectoryName, out exeNewRelativeDirectoryName);
                    SpecialFunctions.CopyDirectory(executingExeDirectoryName, newExeDirectoryName, /*recursive*/ true);
                    Console.WriteLine(Resource.Done_copying);
                }
                finalResult = "\"" + exeNewRelativeDirectoryName + "\"";
                _dirNameToExeDirName.Add(directoryName, finalResult);
            }
            return finalResult;
        }

        /// <summary>
        /// Tries to connect the head-node
        /// </summary>
        /// <param name="headnode">head node name</param>
        /// <param name="scheduler">scheduler</param>
        /// <returns></returns>
        public static bool TryConnect(string headnode, out IScheduler scheduler)
        {
            IScheduler s = new Scheduler();

            bool success = false;
            try
            {
                s.Connect(headnode);
                success = true;
            }
            catch
            {
                success = false;
            }
            scheduler = success ? s : null;
            return success;
        }

        /// <summary>
        /// Tries to get the Jobs from scheduler
        /// </summary>
        /// <param name="scheduler">scheduler</param>
        /// <param name="username">user of the job</param>
        /// <param name="jobID">job Id</param>
        /// <param name="job">scheduler job</param>
        /// <returns></returns>
        public static bool TryGetJob(IScheduler scheduler, string username, int jobID, out ISchedulerJob job)
        {

            IFilterCollection filter = scheduler.CreateFilterCollection();
            if (username != null)
                filter.Add(FilterOperator.Equal, PropId.Job_UserName, username);

            if (scheduler.GetJobIdList(filter, null).Contains(jobID))
            {
                job = scheduler.OpenJob(jobID);
            }
            else
            {
                job = null;
            }

            return job != null;
        }

        /// <summary>
        /// Returns the cluster with the fewest idle cores that will still run on minIdleCoreCount cores. If none exists, 
        /// returns the cluster with the most idle cores. N10 is excluded, as this is a special cluster for long running jobs.
        /// </summary>
        /// <param name="minIdleCoreCount">The minimum number of cores you want. Will return a cluster with at least this many free, if one exists.</param>
        /// <returns></returns>
        public static ClusterStatus GetMostAppropriateCluster(int minIdleCoreCount)
        {
            string longRunningJobsCluster = "msr-gcb-n10";

            List<ClusterStatus> clusterStatusList = HpcLibSettings.ActiveClusters.Where(name => !name.Equals(longRunningJobsCluster, StringComparison.CurrentCultureIgnoreCase))
                                                    .Select(clusterName => new ClusterStatus(clusterName)).ToList();
            if (ParallelOptionsScope.Exists)
                clusterStatusList.AsParallel().WithParallelOptionsScope().ForAll(cs => cs.Refresh());
            else
                clusterStatusList.ForEach(cs => cs.Refresh());

            clusterStatusList.Sort((cs1, cs2) => cs1.IdleCores.CompareTo(cs2.IdleCores));

            foreach (var cluster in clusterStatusList)
            {
                if (cluster.IdleCores >= minIdleCoreCount + 8) // the count is always off by 8, because it includes the head node's cores, which are unavailable.
                    return cluster;
            }
            return clusterStatusList[clusterStatusList.Count - 1];
        }
        #endregion

        #region private methods
        private static bool ExesAreAlreadyOnCluster(string niceName, string rootDirectory, string executingExeDirectoryName, out string newExeDirectoryName, out string exeRelativeDirectoryName)
        {
            newExeDirectoryName = exeRelativeDirectoryName = null;

            DirectoryInfo rootExeDir = new DirectoryInfo(rootDirectory + "\\exes");
            if (!rootExeDir.Exists)
                return false;

            var orderedDirs = rootExeDir.GetDirectories().OrderByDescending(dir => dir.LastWriteTime);
            if (orderedDirs.Count() == 0)
                return false;

            DirectoryInfo lastExeDir = orderedDirs.First();
            DirectoryInfo executingDir = new DirectoryInfo(executingExeDirectoryName);

            bool sameExes = lastExeDir.GetFiles("*", SearchOption.AllDirectories).SequenceEqual(executingDir.GetFiles("*", SearchOption.AllDirectories), new FileAccessComparer());
            if (sameExes)
            {
                newExeDirectoryName = lastExeDir.FullName;
                exeRelativeDirectoryName = "exes\\" + lastExeDir.Name;
            }

            return sameExes;
        }
        #endregion
    }
}
