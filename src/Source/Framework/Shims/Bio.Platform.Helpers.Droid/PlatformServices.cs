using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Bio.Platform.Helpers
{
    /// <summary>
    /// Xamarin version of the platform services. This file is shared
    /// between the iOS and Android versions.
    /// </summary>
    public class PlatformServices
    {
        /// <summary>
        /// Retrieves the assemblies in the application/package/bundle
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Assembly> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
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

        /// <summary>
        /// Creates a Regular Expression; pushed here because some platforms do not support compiling
        /// </summary>
        /// <param name="pattern">Regex pattern</param>
        /// <param name="options">Optional options</param>
        /// <returns></returns>
        public Regex CreateCompiledRegex(string pattern, RegexOptions? options = null)
        {
            return new Regex(pattern, (options != null) ? (options.Value | RegexOptions.Compiled) : RegexOptions.Compiled);
        }

        /// <summary>
        /// Creates a temporary stream that is deleted when disposed.
        /// </summary>
        /// <returns>Stream</returns>
        public Stream CreateTempStream()
        {
            string tempFilename = Path.GetTempFileName();
            FileStream stream = new FileStream(tempFilename, FileMode.Create, FileAccess.ReadWrite);
            return new TemporaryStream(stream, s =>
            {
                s.Dispose();
                File.Delete	(tempFilename);
            });
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PlatformServices()
        {
            Is64BitProcessType = Environment.Is64BitProcess;
            DefaultBufferSize = 4 * 1024; // 4k
            MaxSequenceSize = 512 * 1024 * 1024; // 512m
        }
    }
}
