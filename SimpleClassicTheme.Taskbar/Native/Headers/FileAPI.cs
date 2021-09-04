using System.Runtime.InteropServices;
using System.Text;

namespace SimpleClassicTheme.Taskbar.Native.Headers
{
    /// <summary>
    /// Represents Win32's <c>fileapi.h</c>.
    /// </summary>
    internal class FileAPI
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetShortPathName([MarshalAs(UnmanagedType.LPWStr)] string path, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder shortPath, int shortPathLength);
    }
}
