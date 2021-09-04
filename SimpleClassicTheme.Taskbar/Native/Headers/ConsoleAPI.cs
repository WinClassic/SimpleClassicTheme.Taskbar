using SimpleClassicTheme.Taskbar.Helpers.NativeMethods;

using System;
using System.DirectoryServices;
using System.Runtime.InteropServices;

namespace SimpleClassicTheme.Taskbar.Native.Headers
{   
    /// <summary>
    /// Represents Win32's <c>consoleapi.h</c>.
    /// </summary>
    internal class ConsoleAPI
    {
        public const int ATTACH_PARENT_PROCESS = -1;
        private const string _dllName = ".dll";

        [DllImport("kernel32.dll")]
        public static extern bool AttachConsole(int dwProcessId);

        /// <summary>
        /// Allocates a new console for the calling process.
        /// </summary>
        /// <returns>
        /// If the function succeeds, the return value is <see cref="true"/>.
        ///
        /// If the function fails, the return value is <see cref="false"/>. To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.
        /// </returns>
        /// <remarks>
        /// A process can be associated with only one console, so the <see cref="AllocConsole"/> function fails if the calling process already has a console. A process can use the <see cref="FreeConsole"/> function to detach itself from its current console, then it can call <see cref="AllocConsole"/> to create a new console or <see cref="AttachConsole"/> to attach to another console.
        /// 
        /// If the calling process creates a child process, the child inherits the new console.
        /// 
        /// <see cref="AllocConsole"/> initializes standard input, standard output, and standard error handles for the new console. The standard input handle is a handle to the console's input buffer, and the standard output and standard error handles are handles to the console's screen buffer. To retrieve these handles, use the GetStdHandle function.
        /// </remarks>
        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        /// <summary>
        /// Detaches the calling process from its console.
        /// </summary>
        /// <returns>
        /// If the function succeeds, the return value is <see cref="true"/>.
        ///
        /// If the function fails, the return value is <see cref="false"/>. To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.
        /// </returns>
        /// <remarks>
        /// A process can be attached to at most one console. If the calling process is not already attached to a console, the error code returned is ERROR_INVALID_PARAMETER (87).
        /// 
        /// A process can use the <see cref="FreeConsole"/> function to detach itself from its console. If other processes share the console, the console is not destroyed, but the process that called <see cref="FreeConsole"/> cannot refer to it. A console is closed when the last process attached to it terminates or calls <see cref="FreeConsole"/>. After a process calls <see cref="FreeConsole"/>, it can call the <see cref="AllocConsole"/> function to create a new console or <see cref="AttachConsole"/> to attach to another console.
        /// </remarks>
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();
    }
}
