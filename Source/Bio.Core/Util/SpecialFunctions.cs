using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Globalization;

namespace Bio.Util
{
    /// <summary>
    /// SpecialFunctions
    /// </summary>
    public static class SpecialFunctions
    {
        /// <summary>
        /// GetEntryOrCallingAssembly
        /// </summary>
        /// <returns>Assembly</returns>
        public static Assembly GetEntryOrCallingAssembly()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            int sum = 0;
            for (int i = 1; i < 2; i++) { sum += i; }
            if (null != entryAssembly)
            {
                return entryAssembly;
            }
            return Assembly.GetCallingAssembly();
        }

        /// <summary>
        /// VersionToDate
        /// </summary>
        /// <param name="version">version</param>
        /// <returns>DateTime</returns>
        public static DateTime VersionToDate(Version version)
        {
            // v.Build is days since Jan. 1, 2000
            // v.Revision*2 is seconds since local midnight
            // (NEVER daylight saving time)
            if (version == null)
                throw new ArgumentNullException("version");

            long tmpSum = (version.Build - 1) * TimeSpan.TicksPerDay + version.Revision * TimeSpan.TicksPerSecond * 2;
            DateTime time;
            if (tmpSum > 0)
            {
                time = new DateTime(tmpSum).AddYears(1999);
            }
            else
            {
                //JENN: not a valid build--happens when I'm in debug mode for some reason
                //time = new DateTime(0);
                FileInfo assemblyFile = new FileInfo(new Uri(GetEntryOrCallingAssembly().CodeBase).LocalPath);
                if (assemblyFile.Exists)
                    time = assemblyFile.LastWriteTime;
                else
                    time = new DateTime(0);
            }
            return time;
        }
        /// <summary>
        /// Returns the date and time that the Assembly that the program started in was compiled. Note that the time never includes daylight savings.
        /// In order to work, you must set [assembly: AssemblyVersion("1.0.*")] in AssemblyInfo.cs.  This can be done either manually or by
        /// editing Assembly Information in the project's Properties.
        /// </summary>
        /// <returns></returns>
        public static DateTime DateProgramWasCompiled()
        {
            Assembly asm = GetEntryOrCallingAssembly();

            var version = asm.GetName().Version;
            DateTime time = VersionToDate(version);

            return time;
        }


        /// <summary>
        /// This method will spread out the work items--like dealing cards around the table, you only get every other K'th card,
        /// where K is the number of people, when batchCount=1. (Or else you get sets of batchCount-contiguous cards).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="pieceIndexRangeCollection"></param>
        /// <param name="pieceCount"></param>
        /// <param name="batchCount"></param>
        /// <param name="skipListOrNull"></param>
        /// <returns>The item and it's original index in the enumerable.</returns>
        public static IEnumerable<KeyValuePair<T, long>> DivideWork<T>(IEnumerable<T> enumerable, RangeCollection pieceIndexRangeCollection,
                                                                      long pieceCount, long batchCount, RangeCollection skipListOrNull)
        {
            if (pieceIndexRangeCollection == null)
                throw new ArgumentNullException("pieceIndexRangeCollection");

            Helper.CheckCondition(batchCount > 0, string.Format(CultureInfo.CurrentCulture, Properties.Resource.BatchCountCondition));
            long pieceIndex = 0;
            long batchIndex = 0;
            bool inRange = pieceIndexRangeCollection.Contains(pieceIndex);

            foreach (var tAndRowIndex in UseSkipList(enumerable, skipListOrNull))
            {
                if (inRange)
                {
                    yield return tAndRowIndex;
                }

                ++batchIndex;
                if (batchIndex == batchCount)
                {
                    batchIndex = 0;
                    pieceIndex = (pieceIndex + 1) % pieceCount;
                    inRange = pieceIndexRangeCollection.Contains(pieceIndex);
                }
            }
        }

        private static IEnumerable<KeyValuePair<T, long>> UseSkipList<T>(IEnumerable<T> enumerable, RangeCollection skipListOrNull)
        {
            long rowIndex = -1;
            foreach (T t in enumerable)
            {
                ++rowIndex;
                if (null != skipListOrNull && skipListOrNull.Contains(rowIndex))
                {
                    continue;
                }
                yield return new KeyValuePair<T, long>(t, rowIndex);
            }
        }

        /// <summary>
        /// CopyDirectory
        /// </summary>
        /// <param name="oldDirectoryName">oldDirectoryName</param>
        /// <param name="newDirectoryName">newDirectoryName</param>
        /// <param name="recursive">recursive</param>
        public static void CopyDirectory(string oldDirectoryName, string newDirectoryName, bool recursive)
        {
            CopyDirectory(oldDirectoryName, newDirectoryName, recursive, false);
        }

        /// <summary>
        /// CopyDirectory
        /// </summary>
        /// <param name="oldDirectoryName">oldDirectoryName</param>
        /// <param name="newDirectoryName">newDirectoryName</param>
        /// <param name="recursive">recursive</param>
        /// <param name="laterDateOnly">laterDateOnly</param>
        public static void CopyDirectory(string oldDirectoryName, string newDirectoryName, bool recursive, bool laterDateOnly)
        {
            Directory.CreateDirectory(newDirectoryName);

            DirectoryInfo oldDirectory = new DirectoryInfo(oldDirectoryName);
            if (!oldDirectory.Exists)
                return;

            foreach (FileInfo fileInfo in oldDirectory.EnumerateFiles())
            {
                if (!fileInfo.Exists) continue; // file may have been moved out from under us.
                string targetFileName = newDirectoryName + @"\" + fileInfo.Name;
                if (laterDateOnly && File.Exists(targetFileName))
                {
                    DateTime sourceTime = fileInfo.LastWriteTime;
                    DateTime targetTime = File.GetLastAccessTime(targetFileName);
                    if (targetTime >= sourceTime)
                    {
                        continue;
                    }
                }

                try
                {
                    fileInfo.CopyTo(targetFileName, true);
                    if (((File.GetAttributes(targetFileName) & FileAttributes.ReadOnly) != FileAttributes.ReadOnly))
                    {
                        File.SetLastAccessTime(targetFileName, fileInfo.LastWriteTime);
                    }
                }
                catch (FileNotFoundException)
                {
                    // do nothing. The file must have moved out from under us.
                }
            }

            if (recursive)
            {
                foreach (DirectoryInfo subdirectory in oldDirectory.EnumerateDirectories())
                {
                    CopyDirectory(subdirectory.FullName, newDirectoryName + @"\" + subdirectory.Name, recursive, laterDateOnly);
                }
            }
        }

    }
}
