using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Collections.Concurrent;

namespace Bio.Selectome
{
    /// <summary>
    /// A class the implements a web cache to locally store url requests.
    /// </summary>
    public class WebCache
    {
        private static string hashString(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var hash = new SHA1Managed())
            {
                var hashBytes = hash.ComputeHash(plainTextBytes);
                var s = Convert.ToBase64String(hashBytes);
                return s.Replace("ab", "abab").Replace("\\", "ab");
            }
        }
        private string cacheFile(string key)
        {
            var sha1 = hashString(key);
            var encoded = Uri.EscapeDataString(sha1);
            return Path.Combine(downloadCache,encoded);
        }
        private static bool isWellFormedResult(string result)
        {
            return !(String.IsNullOrEmpty(result));
        }
        private string downloadCache;
        private double expiration;

        /// <summary>
        /// Attempt to create a web cache for the files downloaded from Selectome.  
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="expiration">How many days until the cache expires?</param>
        /// <returns></returns>
        public WebCache(string prefix, double expiration)
        {
            this.expiration = expiration;
            //TODO: Handle failures better?
            // e.g. C:\Users\<user>\AppData\Local\Microsoft\Windows\Temporary Internet Files
            var cacheFolder = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
            downloadCache = Path.Combine(cacheFolder, prefix);
            // Get file name for a given string (using hash)
            // Try to create directory, if it does not exist
            if (!Directory.Exists(downloadCache))
            {
                Directory.CreateDirectory(downloadCache);
            }
        }
        /// <summary>
        /// Try to get a stored result
        /// </summary>
        /// <param name="key"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryRetrieve(string key, out string result)
        {
            result = String.Empty;
            var cacheFile = this.cacheFile(key);
            if (File.Exists(cacheFile) && (File.GetLastWriteTimeUtc(cacheFile) - DateTime.UtcNow).TotalDays < expiration)
            {
                result = File.ReadAllText(cacheFile);
                if (isWellFormedResult(result))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Save a value in the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, string value)
        {
            try
            {
                var cFile = cacheFile(key);
                File.WriteAllText(cFile, value);
            }
            catch (Exception e)
            {
                throw new IOException("Could not cache the web file with key: " + key, e);
            }
        }
    }
}