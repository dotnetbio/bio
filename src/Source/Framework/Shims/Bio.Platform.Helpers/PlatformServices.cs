using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Bio.Platform.Helpers
{
    /// <summary>
    /// PCL version of platform services.
    /// </summary>
    public class PlatformServices
    {
        /// <summary>
        /// Retrieves the assemblies in the application/package/bundle
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Assembly> GetAssemblies()
        {
            throw new Exception("You must add a reference to a Bio.Platform.Helpers library.");
        }

        /// <summary>
        /// True if this is a 64-bit process
        /// </summary>
        public bool Is64BitProcessType
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a Regular Expression; pushed here because some platforms do not support compiling
        /// </summary>
        /// <param name="pattern">Regex pattern</param>
        /// <param name="options">Optional options</param>
        /// <returns></returns>
        public Regex CreateCompiledRegex(string pattern, RegexOptions? options = null)
        {
            throw new Exception("You must add a reference to a Bio.Platform.Helpers library.");
        }

        /// <summary>
        /// Creates a temporary stream that is deleted when disposed.
        /// </summary>
        /// <returns>Stream</returns>
        public Stream CreateTempStream()
        {
            throw new Exception("You must add a reference to a Bio.Platform.Helpers library.");
        }

        /// <summary>
        /// Default buffer size for parsers
        /// </summary>
        public int DefaultBufferSize
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum sequence size for the platform.
        /// </summary>
        public long MaxSequenceSize
        {
            get;
            set;
        }
    }
}
