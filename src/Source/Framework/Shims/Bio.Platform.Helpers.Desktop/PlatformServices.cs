using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Bio.Platform.Helpers
{
    /// <summary>
    /// .NET 4.5 desktop version of the platform services.
    /// </summary>
    public class PlatformServices
    {
        /// <summary>
        /// Retrieves the assemblies in the application/package/bundle
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Assembly> GetAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            // Add additional ones from the path.
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!string.IsNullOrEmpty(path))
            {
                foreach (string dll in Directory.GetFiles(path, "*.dll"))
                {
                    Assembly asm = null;
                    try
                    {
                        asm = Assembly.LoadFrom(dll);
                    }
                    catch (FileLoadException)
                    {
                    }
                    catch (BadImageFormatException)
                    {
                    }

                    if (asm != null)
                        assemblies.Add(asm);
                }
            }

            return assemblies.Distinct();
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
            DefaultBufferSize = 4 * 1024*1024; // 4m
            MaxSequenceSize = Int32.MaxValue - 1;
        }
    }
}
