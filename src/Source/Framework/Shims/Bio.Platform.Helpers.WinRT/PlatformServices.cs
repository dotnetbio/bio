using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Windows.Storage;

namespace Bio.Platform.Helpers
{
    /// <summary>
    /// WinRT version of platform services.
    /// </summary>
    public class PlatformServices
    {
        /// <summary>
        /// Retrieves the assemblies in the application/package/bundle
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Assembly> GetAssemblies()
        {
            var assemblies = new List<Assembly>();
            var files = Windows.ApplicationModel.Package.Current.InstalledLocation.GetFilesAsync().AsTask().Result;
            foreach (var file in files)
            {
                if ((file.FileType == ".dll") || (file.FileType == ".exe"))
                {
                    try
                    {
                        AssemblyName name = new AssemblyName() { Name = Path.GetFileNameWithoutExtension(file.Name) };
                        Assembly asm = Assembly.Load(name);
                        assemblies.Add(asm);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                }
            }
            return assemblies;
        }

        /// <summary>
        /// True if this is a 64-bit process.
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
            return new Regex(pattern, options == null ? RegexOptions.None : options.Value);
        }

        /// <summary>
        /// Creates a temporary stream that is deleted when disposed.
        /// </summary>
        /// <returns>Stream</returns>
        public Stream CreateTempStream()
        {
            string fn = Guid.NewGuid().ToString() + ".tmp";
            var sfile = ApplicationData.Current.TemporaryFolder.CreateFileAsync(fn, CreationCollisionOption.GenerateUniqueName).AsTask().Result;
            var stream = sfile.OpenStreamForWriteAsync().Result;
            return new TemporaryStream(sfile, stream);
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
        /// Constructor
        /// </summary>
        public PlatformServices()
        {
            var architecture = GetProcessorArchitecture();
            Is64BitProcessType = architecture == ProcessorArchitecture.AMD64 || architecture == ProcessorArchitecture.IA64;
            DefaultBufferSize = 4 * 1024 * 1024; // 4m
            MaxSequenceSize = Int32.MaxValue - 1;
        }

        static ProcessorArchitecture GetProcessorArchitecture()
        {
            try
            {
                var sysInfo = new _SYSTEM_INFO();
                GetNativeSystemInfo(ref sysInfo);

                return Enum.IsDefined(typeof(ProcessorArchitecture), sysInfo.wProcessorArchitecture)
                    ? (ProcessorArchitecture)sysInfo.wProcessorArchitecture
                    : ProcessorArchitecture.UNKNOWN;
            }
            catch
            {
            }
            return ProcessorArchitecture.UNKNOWN;
        }


        [DllImport("kernel32.dll")]
        static extern void GetNativeSystemInfo(ref _SYSTEM_INFO lpSystemInfo);

        [StructLayout(LayoutKind.Sequential)]
        struct _SYSTEM_INFO
        {
            public ushort wProcessorArchitecture;
            public ushort wReserved;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public UIntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        };

        enum ProcessorArchitecture : ushort
        {
            INTEL = 0,
            MIPS = 1,
            ALPHA = 2,
            PPC = 3,
            SHX = 4,
            ARM = 5,
            IA64 = 6,
            ALPHA64 = 7,
            MSIL = 8,
            AMD64 = 9,
            IA32_ON_WIN64 = 10,
            UNKNOWN = 0xFFFF
        }
    
    }
}

