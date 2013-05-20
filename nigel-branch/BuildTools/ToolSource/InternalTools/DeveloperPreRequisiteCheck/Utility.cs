using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace DeveloperPreRequisiteCheck
{
    /// <summary>
    /// Contains the utility methods required by "DeveloperPreRequisiteCheck"
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Key for file path parameter
        /// </summary>
        public const string PARAM_FILEPATH = "FILEPATH";

        /// <summary>
        /// Key for the string containing Reset environment variable batch file path
        /// </summary>
        public const string PARAM_RESETENVVARFILEPATH = "RESETENVVARFILEPATH";
                
        /// <summary>
        /// Reads the value from registry and returns.
        /// </summary>
        /// <param name="path">Path which has to be read.</param>
        /// <param name="key">Key in the path which has to be read.</param>
        /// <param name="value">value in registry.</param>
        /// <returns>Value in the registry</returns>
        public static bool ReadRegistry(string path, string key, out string value)
        {
            value = string.Empty;

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            object registryKey = Registry.GetValue(path, key, null);

            if (null == registryKey)
            {
                return false;
            }

            value = registryKey.ToString();
            return true;
        }

        /// <summary>
        /// Compares the version and check if currentVersion is greaterthan
        /// minimumVersion
        /// </summary>
        /// <param name="minimumVersion">Minimum version</param>
        /// <param name="currentVersion">Current version</param>
        /// <returns>Is currentVersion greater than minimumVersion</returns>
        public static bool CompareVersion(string minimumVersion, string currentVersion)
        {
            Version minimumValue, currentValue;

            if (Version.TryParse(minimumVersion, out minimumValue))
            {
                if (Version.TryParse(currentVersion, out currentValue))
                {
                    return (minimumValue <= currentValue);
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the version of file and returns
        /// </summary>
        /// <param name="path">Path of the file.</param>
        /// <returns>Version of the file.</returns>
        public static bool GetVersion(string path, out string version)
        {
            version = string.Empty;
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            if (File.Exists(path))
            {
                version = FileVersionInfo.GetVersionInfo(path).FileVersion;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reads the environment variable and returns the value
        /// </summary>
        /// <param name="key">Key of environment variable</param>
        /// <returns>Does the key exists.</returns>
        public static bool ReadEnvironmentVariable(string key, out string value)
        {
            value = string.Empty;

            value = System.Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write a value to given environment variable
        /// </summary>
        /// <param name="key">Key of environment variable</param>
        /// <param name="value">value to be written</param>
        /// <returns>Does the key exists.</returns>
        public static void WriteEnvironmentVariable(string key, string value)
        {
            Environment.SetEnvironmentVariable(key, value, EnvironmentVariableTarget.User);
        }

        /// <summary>
        /// If the file exist, delete the file.
        /// </summary>
        /// <param name="path">File path</param>
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Create the file if does not exist or append the file.
        /// Write the content to file.
        /// </summary>
        /// <param name="path">file path</param>
        /// <param name="content">content to be written</param>
        public static void WriteToFile(string path, string content)
        {
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(content);
                writer.Flush();
                writer.Close();
            }
        }
     }
}
