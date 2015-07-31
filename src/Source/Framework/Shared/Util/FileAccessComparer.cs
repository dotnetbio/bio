using System;
using System.Collections.Generic;
using System.IO;

namespace Bio.Util
{
    /// <summary>
    /// Compare FileInfo for equal file access capabilities
    /// </summary>
    public class FileAccessComparer : IEqualityComparer<FileInfo>
    {
        /// <summary>
        /// Compare for file equality using name and timestamps
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns> bool true if FileInfo name and LastWriteTimes are equal, otherwise false </returns>
        public bool Equals(FileInfo x, FileInfo y)
        {
            if (null == x || null == y)
            {
                return (null == x && null == y);
            }

            return x.Name.Equals(y.Name) && x.LastWriteTime.Equals(y.LastWriteTime);
        }

        /// <summary>
        /// Returns HashCode from FileInfo.Name
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(FileInfo obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return obj.Name.GetHashCode();
        }
    }
}
