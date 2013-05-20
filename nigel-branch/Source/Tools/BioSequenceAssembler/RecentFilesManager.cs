namespace SequenceAssembler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    /// Utility class which will manage recent file list
    /// </summary>
    public static class RecentFilesManager
    {
        /// <summary>
        /// File path to store and retrieve recent files info
        /// </summary>
        private static string recentFilesStorePath;

        /// <summary>
        /// Maximum number of rencent files to be stored
        /// </summary>
        private static int maxRecentFilesCount = 6;

        /// <summary>
        /// Fielname for storing recent files list
        /// </summary>
        private static string recentFileFilename = "SequenceAssembler.Recent";

        /// <summary>
        /// Initialize static varialbes
        /// </summary>
        static RecentFilesManager()
        {
            recentFilesStorePath = Path.GetTempPath() + recentFileFilename;

            if (!File.Exists(recentFilesStorePath))
            {
                recentFilesStorePath = AppDomain.CurrentDomain.BaseDirectory + recentFileFilename;   
            }
        }

        /// <summary>
        /// Get the list of recent files
        /// </summary>
        public static List<string> RecentFiles
        {
            get
            {
                List<string> recentFileList = new List<string>();
                BinaryFormatter formatter = new BinaryFormatter();
                Stream readStream = null;

                try
                {
                    if (File.Exists(recentFilesStorePath))
                    {
                        readStream = File.OpenRead(recentFilesStorePath);

                        recentFileList = formatter.Deserialize(readStream) as List<string>;
                    }
                }
                catch
                {
                    // ignore any error as this is not a critical feature
                }
                finally
                {
                    if (readStream != null)
                    {
                        readStream.Close();
                    }
                }

                return recentFileList;
            }
        }

        /// <summary>
        /// Add a file to top of the recent files list
        /// </summary>
        /// <param name="filePath">Path of file to add</param>
        public static void AddFile(string filePath)
        {
            List<string> recentFiles = RecentFiles;

            recentFiles.Remove(filePath);
            recentFiles.Insert(0, filePath);

            WriteRecentFiles(recentFiles);
        }

        /// <summary>
        /// Write the modified recent file list to file
        /// </summary>
        /// <param name="recentFiles">List of recent files</param>
        private static void WriteRecentFiles(List<string> recentFiles)
        {
            Stream writeStream = null;
            BinaryFormatter formatter = new BinaryFormatter();

            if (recentFiles.Count > maxRecentFilesCount)
            {
                recentFiles.RemoveRange(maxRecentFilesCount - 1, recentFiles.Count - maxRecentFilesCount);
            }

            try
            {
                try
                {
                    if (File.Exists(recentFilesStorePath))
                        File.Delete(recentFilesStorePath);

                    writeStream = File.OpenWrite(recentFilesStorePath);
                }
                catch
                {
                    recentFilesStorePath = Path.GetTempPath() + recentFileFilename;
                    writeStream = File.OpenWrite(recentFilesStorePath);
                }                

                formatter.Serialize(writeStream, recentFiles);
            }
            catch
            {
                // ignore any error as this is not a critical feature
            }
            finally
            {
                if (writeStream != null)
                {
                    writeStream.Close();
                }
            }
        }
    }
}
