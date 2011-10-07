using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Hpc;
using Bio.Util.ArgumentParser;
using System.IO;

namespace Bio.Tests
{
    /// <summary>
    /// Summary description for HPCTests
    /// </summary>
    [TestClass]
    public class HpcTests
    {
        /// <summary>
        /// Tests cluster status refresh 
        /// </summary>
        [TestMethod]
        public void TestClusterStatusRefresh()
        {
            ClusterStatus clusterStatus = new ClusterStatus("er-clust1-c07");
            clusterStatus.Cluster = "er-clust1-c07";
            bool ActualResult = clusterStatus.Refresh();
            string ActualString = clusterStatus.ToString();
            string ExpectedString = "er-clust1-c07: -1/-2";
            bool expectedResult = true;
            Assert.AreEqual(expectedResult, ActualResult);
            Assert.AreEqual(ExpectedString, ActualString);
        }

        /// <summary>
        /// Tests cluster submitter args 
        /// </summary>
        [TestMethod]
        public void TestClusterSubmitterArgsFinalizeParse()
        {
            ClusterSubmitterArgs clusterSubmitterArgs = new ClusterSubmitterArgs();
            clusterSubmitterArgs.JobID = 2;
            clusterSubmitterArgs.StdOutDirName = @"\TestUtils\Fasta";
            clusterSubmitterArgs.TaskCount = 1;
            clusterSubmitterArgs.Cluster = ":auto:";
            clusterSubmitterArgs.Version = 3;
            clusterSubmitterArgs.Dir = @"\TestUtils\Fasta\5_sequences.fasta";
            HpcLibSettings.CreateExampleSettingsFiles("a.txt", "b.txt");
            clusterSubmitterArgs.FinalizeParse();

            ClusterSubmitterArgs expected = (ClusterSubmitterArgs)clusterSubmitterArgs.Clone();
            Assert.AreEqual(clusterSubmitterArgs, expected);
        }

        /// <summary>
        /// Tests GetMostAppropriateCluster 
        /// </summary>
        [TestMethod]
        public void TestHpcLibGetMostAppropriateCluster()
        {
            HpcLibSettings.CreateExampleSettingsFiles("c.txt", "d.txt");
            string clusterName = HpcLib.GetMostAppropriateCluster(10).Cluster;
            Assert.IsFalse(string.IsNullOrEmpty(clusterName));
        }

        /// <summary>
        /// Tests HpcLibSettingsTryWriteToLog 
        /// </summary>
        [TestMethod]
        public void TestHpcLibSettingsTryWriteToLog()
        {
            ClusterSubmitterArgs clusterSubmitterArgs = new ClusterSubmitterArgs();
            clusterSubmitterArgs.TaskRange = new Util.RangeCollection(2);
            string path = Path.GetTempPath() + @"\clusterLog.xml";
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Exists)
                fileInfo.Delete();
            HpcLibSettings.CreateExampleSettingsFiles("a.txt", "b.txt");
            bool ActualResult = HpcLibSettings.TryWriteToLog(clusterSubmitterArgs);
            bool ExpectedResult = true;
            Assert.AreEqual(ExpectedResult, ActualResult);
        }

        /// <summary>
        /// Tests HpcLibCopyFilesAndDeleteFiles 
        /// </summary>
        [TestMethod]
        public void TestHpcLibCopyFilesAndDeleteFiles()
        {
            string assemblypath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);

            string sourcedir = assemblypath + @"\TestUtils\Fasta";
            string destDir = assemblypath + @"\TestUtils\Fasta1";
            IList<string> sampleFileList = new List<string>();
            sampleFileList.Add("5_sequences.fasta");
            try
            {
                HpcLib.CopyFiles(sampleFileList, sourcedir, destDir);
            }
            catch
            {
                Assert.Fail();
            }
            HpcLib.DeleteFiles(sampleFileList, destDir);
            DirectoryInfo info = new DirectoryInfo(destDir);
            Assert.AreEqual(0, info.GetFiles().Count());
        }


        /// <summary>
        /// Tests parallel query extensions with parallel options scope 
        /// </summary>
        [TestMethod]
        public void TestParallelQueryExtensionsWithParallelOptionsScope()
        {
            List<ClusterStatus> clusterStatusList = HpcLibSettings.ActiveClusters.Where(name => !name.Equals("msr-gcb-n10", StringComparison.CurrentCultureIgnoreCase))
                                                                .Select(clusterName => new ClusterStatus(clusterName)).ToList();

            List<ClusterStatus> expected = clusterStatusList;
            using (ParallelOptionsScope.CreateFullyParallel())
            {
                clusterStatusList.AsParallel().WithParallelOptionsScope();
            }

            Assert.AreSame(expected, clusterStatusList);
        }

        /// <summary>
        /// Tests cluster submitter wait for job
        /// </summary>
        [TestMethod]
        public void TestConstructAndRun()
        {
            string inputFile = @"\TestUtils\HpcInput.txt";
            string outputFile = @"\TestUtils\results.txt";
            string assemblypath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);
            string inputFilePath = assemblypath + inputFile;

            FileInfo fileInfo = new FileInfo(assemblypath + outputFile);
            if (fileInfo.Exists)
                fileInfo.Delete();

            string[] args = { "-InputFile", "InputFile((\"" + inputFilePath + "\"))", "-Operator", "Sum" };
            CommandArguments.ConstructAndRun<AggregateNumbers>(args);

            FileStream fs = fileInfo.OpenRead();
            byte[] buffer = new byte[2];
            fs.Read(buffer, 0, 2);

            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            Assert.AreEqual(enc.GetString(buffer), "15");
        }

    }
}
