using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Bio.Hpc.Properties;
using Bio.Util;

namespace Bio.Hpc
{
    /// <summary>
    /// This class provides the necessary settings for HPC lib.
    /// </summary>
    public class HpcLibSettings
    {
        #region private members
        private SharedSettings _sharedSettings;
        private LocalSettings _localSettings;
        private static HpcLibSettings Singleton = new HpcLibSettings();

        #endregion

        #region pubilc members
        /// <summary>
        /// Settings file name
        /// </summary>
        public const string SettingsFileName = "HpcLibSettings.xml";

        /// <summary>
        /// Local setting file name
        /// </summary>
        public const string LocalSettingsFileName = "HpcLibSettings.local.xml";

        /// <summary>
        /// Gets all well known clusters
        /// </summary>
        public static Dictionary<string, ClusterInfo> KnownClusters { get { return Singleton._sharedSettings.KnownClusters; } }

        /// <summary>
        /// Gets log file
        /// </summary>
        public static string LogFile { get { return Singleton._localSettings.LogFile; } }

        /// <summary>
        /// Gets default results copy pattern
        /// </summary>
        public static string DefaultResultsCopyPattern { get { return Singleton._localSettings.DefaultResultsCopyPattern; } }

        /// <summary>
        /// Gets all active clusters
        /// </summary>
        public static List<string> ActiveClusters { get { return Singleton._sharedSettings.ActiveClusters; } }

        #endregion

        #region pubilc methods
        /// <summary>
        /// Creates the necessary settings files
        /// </summary>
        /// <param name="sharedFileName">shared file name</param>
        /// <param name="localFileName">local file name</param>
        public static void CreateExampleSettingsFiles(string sharedFileName, string localFileName)
        {
            KnownClusters.Add("cluster1", new ClusterInfo("cluster1", "path1"));
            KnownClusters.Add("cluster2", new ClusterInfo("cluster2", "path2"));
            ActiveClusters.Add("cluster1");
            ActiveClusters.Add("cluster2");
            Singleton._localSettings.LogFile = Path.GetTempPath() + @"\clusterLog.xml";
            Singleton._localSettings.DefaultResultsCopyPattern = @"raw\*raw*,raw\*tab*";

            Singleton._localSettings.Username = Environment.GetEnvironmentVariable("username");

            Singleton._localSettings.Save(localFileName);
            Singleton._sharedSettings.Save(sharedFileName);
        }

        /// <summary>
        /// Tries to write to the log file. If no log file is defined, returns false.
        /// </summary>
        /// <param name="clusterArgs">Cluser submitter arugemtns</param>
        /// <returns>true if write sucessed</returns>
        public static bool TryWriteToLog(ClusterSubmitterArgs clusterArgs)
        {
            if (!string.IsNullOrEmpty(LogFile))
            {
                if (!Directory.Exists(Path.GetDirectoryName(LogFile)))
                {
                    Console.WriteLine(Resource.Log_file_directory, Path.GetDirectoryName(LogFile));
                    return false;
                }
                FileStream filestream = null;
                try
                {
                    bool exists = File.Exists(LogFile);
                    if (FileUtils.TryToOpenFile(LogFile, new TimeSpan(0, 3, 0), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, out filestream))
                    {
                        List<LogEntry> logEntries = exists && filestream.Length > 0 ? LogEntry.LoadEntries(new StreamReader(filestream)) : new List<LogEntry>();
                        LogEntry newSubmission = new LogEntry(clusterArgs);
                        logEntries.Add(newSubmission);
                        filestream.Position = 0;
                        LogEntry.SaveEntries(logEntries, new StreamWriter(filestream));
                        filestream.Dispose();

                        return true;
                    }
                }
                catch (System.Xml.XmlException exception)
                {
                    Console.WriteLine(exception);
                    return false;
                }
                finally
                {
                    if (filestream != null)
                        filestream.Dispose();
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the log entry file 
        /// </summary>
        /// <param name="clusterArgs">Cluster submitter arguments</param>
        /// <returns>log file info</returns>
        public static FileInfo GetLogEntryFile(ClusterSubmitterArgs clusterArgs)
        {
            string directory = clusterArgs.ExternalRemoteDirectoryName;
            string filename = LogEntryFileName(clusterArgs.Name);
            return new FileInfo(Path.Combine(directory, filename));
        }

        /// <summary>
        /// Writes log entries into cluster dir
        /// </summary>
        /// <param name="clusterArgs">Cluster submitter arguments</param>
        public static void WriteLogEntryToClusterDirectory(ClusterSubmitterArgs clusterArgs)
        {
            FileInfo logEntryFile = GetLogEntryFile(clusterArgs);
            LogEntry logEntry = new LogEntry(clusterArgs);
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(LogEntry));
            using (var writer = logEntryFile.CreateText())
            {
                serializer.Serialize(writer, logEntry);
            }
        }

        /// <summary>
        /// Loads log entry file
        /// </summary>
        /// <param name="file">log entry file</param>
        /// <returns>Log entry</returns>
        public static LogEntry LoadLogEntryFile(FileInfo file)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(LogEntry));
            using (var reader = file.OpenRead())
            {
                return (LogEntry)serializer.Deserialize(reader);
            }
        }

        #endregion

        #region private methods
        private HpcLibSettings()
        {
            string username = Environment.GetEnvironmentVariable("username");
            _sharedSettings = SharedSettings.Load(SettingsFileName);
            _localSettings = LocalSettings.Load(LocalSettingsFileName, username);
        }

        private static string LogEntryFileName(string runName)
        {
            return runName + "_logEntry.xml";
        }
        #endregion

        /// <summary>
        /// Holds the shared settings
        /// </summary>
        [Serializable]
        public class SharedSettings
        {
            /// <summary>
            /// DummyField
            /// </summary>
            [XmlArray("KnownClusters")]
            public List<ClusterInfo> DummyField;

            /// <summary>
            /// ActiveClusters
            /// </summary>
            public List<string> ActiveClusters = new List<string>();

            /// <summary>
            /// KnownClusters
            /// </summary>
            [XmlIgnore()]
            public Dictionary<string, ClusterInfo> KnownClusters { get; private set; }

            /// <summary>
            /// Loads the file
            /// </summary>
            /// <param name="filename">file name</param>
            /// <returns></returns>
            public static SharedSettings Load(string filename)
            {
                try
                {
                    string asmPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string settingsPath = Path.Combine(asmPath, filename);
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(SharedSettings));

                    using (TextReader reader = File.OpenText(settingsPath))
                    {
                        SharedSettings result = (SharedSettings)serializer.Deserialize(reader);
                        result.KnownClusters = new Dictionary<string, ClusterInfo>(StringComparer.CurrentCultureIgnoreCase);
                        result.DummyField.ForEach(node => result.KnownClusters.Add(node.Name, node));

                        return result;
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Problem loading " + filename);
                    Console.Error.WriteLine(e.Message);
                    SharedSettings result = new SharedSettings();
                    result.KnownClusters = new Dictionary<string, ClusterInfo>();
                    return result;
                }
            }

            /// <summary>
            /// Save the file
            /// </summary>
            /// <param name="filename">file name</param>
            public void Save(string filename)
            {
                string asmPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string settingsPath = Path.Combine(asmPath, filename);
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(SharedSettings));

                DummyField = KnownClusters.Values.ToList();
                using (TextWriter writer = File.CreateText(settingsPath))
                {
                    serializer.Serialize(writer, this);
                }
            }
        }

        /// <summary>
        /// Holds local settings
        /// </summary>
        [Serializable]
        public class LocalSettings
        {
            /// <summary>
            /// user name
            /// </summary>
            [XmlAttribute]
            public string Username;
            /// <summary>
            /// log file
            /// </summary>
            public string LogFile = "";
            /// <summary>
            /// default results copy pattern
            /// </summary>
            public string DefaultResultsCopyPattern = "";

            /// <summary>
            /// Saves the file
            /// </summary>
            /// <param name="filename">file name</param>
            public void Save(string filename)
            {
                List<LocalSettings> userSettings = LoadAll(filename);
                bool alreadyExists = false;
                for (int i = 0; i < userSettings.Count; i++)
                {
                    if (userSettings[i].Username.Equals(Username, StringComparison.CurrentCultureIgnoreCase))
                    {
                        Helper.CheckCondition(!alreadyExists, "Multiple entries for " + Username);
                        userSettings[i] = this;
                        alreadyExists = true;
                    }
                }
                if (!alreadyExists)
                    userSettings.Add(this);

                SaveAll(userSettings, filename);
            }

            /// <summary>
            /// Saves all the settings into file
            /// </summary>
            /// <param name="settings">list of local settings</param>
            /// <param name="filename">file name</param>
            public static void SaveAll(List<LocalSettings> settings, string filename)
            {
                string asmPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string settingsPath = Path.Combine(asmPath, filename);
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<LocalSettings>));

                using (TextWriter writer = File.CreateText(settingsPath))
                {
                    serializer.Serialize(writer, settings);
                }
            }

            /// <summary>
            /// Loads the local setting from file
            /// </summary>
            /// <param name="filename">file name</param>
            /// <param name="username">user name</param>
            /// <returns>local settings</returns>
            public static LocalSettings Load(string filename, string username)
            {
                List<LocalSettings> userSettings = LoadAll(filename);
                foreach (LocalSettings user in userSettings)
                {
                    if (user.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return user;
                    }
                }
                Console.Error.WriteLine("No local settings entry for user " + username);
                return new LocalSettings();
            }

            /// <summary>
            /// Loads all the local settings from file
            /// </summary>
            /// <param name="filename">file name</param>
            /// <returns>list of local settings</returns>
            public static List<LocalSettings> LoadAll(string filename)
            {
                try
                {
                    string asmPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string settingsPath = Path.Combine(asmPath, filename);
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<LocalSettings>));

                    using (TextReader reader = File.OpenText(settingsPath))
                    {
                        List<LocalSettings> userSettings = (List<LocalSettings>)serializer.Deserialize(reader);
                        return userSettings;
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Problem loading " + filename);
                    Console.Error.WriteLine(e.Message);
                    return new List<LocalSettings>();
                }
            }
        }

        /// <summary>
        /// Holds user settings
        /// </summary>
        [Serializable]
        public class UserSettings
        {
            /// <summary>
            /// user name
            /// </summary>
            [XmlAttribute]
            public string Username;
            /// <summary>
            /// local settings
            /// </summary>
            public LocalSettings LocalSettings;

            /// <summary>
            /// Constructor
            /// </summary>
            public UserSettings() { }

            /// <summary>
            /// Constructor with user name and local settings
            /// </summary>
            /// <param name="username">user name</param>
            /// <param name="localSettings">local settings</param>
            public UserSettings(string username, LocalSettings localSettings)
            {
                Username = username;
                LocalSettings = localSettings;
            }
        }
    }

    /// <summary>
    /// Holds cluser info
    /// </summary>
    [Serializable]
    public class ClusterInfo
    {
        /// <summary>
        /// Constructor 
        /// </summary>
        public ClusterInfo() { }
        /// <summary>
        /// Constructor with name and path
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="path">path</param>
        public ClusterInfo(string name, string path)
        {
            Name = name;
            StoragePath = path;
        }

        /// <summary>
        /// Cluser name
        /// </summary>
        [XmlAttribute()]
        public string Name { get; set; }
        /// <summary>
        /// Storage path
        /// </summary>
        [XmlAttribute()]
        public string StoragePath { get; set; }

        /// <summary>
        /// Cluser info into string
        /// </summary>
        /// <returns>cluser info</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
