using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

using Windows.ApplicationModel;
using Windows.Storage;

namespace Bio.Platform.Helpers
{
    /// <summary>
    /// PCL version of platform services.
    /// </summary>
    public class PlatformServices
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PlatformServices()
        {
            this.Is64BitProcessType = false;
            this.DefaultBufferSize = 4 * 1024; // 4k
            this.MaxSequenceSize = 512 * 1024; // 512m
        }

        /// <summary>
        /// True if this is a 64-bit process
        /// </summary>
        public bool Is64BitProcessType { get; set; }

        /// <summary>
        /// Default buffer size for parsers
        /// </summary>
        public int DefaultBufferSize { get; set; }

        /// <summary>
        /// Maximum sequence size for the platform.
        /// </summary>
        public long MaxSequenceSize { get; set; }

        /// <summary>
        /// Retrieves the assemblies in the application/package/bundle
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Assembly> GetAssemblies()
        {
            var assemblies = new List<Assembly>();
            IReadOnlyList<StorageFile> files = Package.Current.InstalledLocation.GetFilesAsync().AsTask().Result;
            foreach (StorageFile file in files)
            {
                if ((file.FileType == ".dll") || (file.FileType == ".exe"))
                {
                    try
                    {
                        var name = new AssemblyName { Name = Path.GetFileNameWithoutExtension(file.Name) };
                        Assembly asm = Assembly.Load(name);
                        assemblies.Add(asm);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }
            }
            return assemblies;
        }

        /// <summary>
        /// Creates a Regular Expression; pushed here because some platforms do not support compiling
        /// </summary>
        /// <param name="pattern">Regex pattern</param>
        /// <param name="options">Optional options</param>
        /// <returns></returns>
        public Regex CreateCompiledRegex(string pattern, RegexOptions? options = null)
        {
            return new Regex(pattern, options == null ? RegexOptions.None : options.Value);
        }

        /// <summary>
        /// Creates a temporary stream that is deleted when disposed.
        /// </summary>
        /// <returns>Stream</returns>
        public Stream CreateTempStream()
        {
            string fn = Guid.NewGuid() + ".tmp";
            StorageFile sfile =
                ApplicationData.Current.TemporaryFolder.CreateFileAsync(fn, CreationCollisionOption.GenerateUniqueName)
                    .AsTask()
                    .Result;
            Stream stream = sfile.OpenStreamForWriteAsync().Result;
            return new TemporaryStream(sfile, stream);
        }
    }
}