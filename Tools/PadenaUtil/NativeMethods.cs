using System.Runtime.InteropServices;

namespace PadenaUtil
{
    public static class NativeMethods
    {
        private const uint ATTACH_PARENT_PROCESS = 0x0ffffffff;
        private const int ERROR_ACCESS_DENIED = 5;
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AttachConsole(uint dwProcessId);
        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)] 
        private static extern bool FreeConsole();

        /// <summary>
        /// Gets if the current process has a console window.
        /// </summary>
        public static bool IsConsoleAttached()
        {
            if (AttachConsole(ATTACH_PARENT_PROCESS))
            {
                FreeConsole();
                return false;
            }

            //If the calling process is already attached to a console, 
            // the error code returned is ERROR_ACCESS_DENIED
            return Marshal.GetLastWin32Error() == ERROR_ACCESS_DENIED;
        }
    }
}
