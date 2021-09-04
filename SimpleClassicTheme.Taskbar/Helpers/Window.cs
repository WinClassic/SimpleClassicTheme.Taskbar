using SimpleClassicTheme.Taskbar.Helpers.NativeMethods;

using System;
using System.Text;

using static SimpleClassicTheme.Taskbar.Native.Headers.WinUser;
using static SimpleClassicTheme.Taskbar.Native.Headers.PSAPI;
using static SimpleClassicTheme.Taskbar.Native.Headers.ProcessThreadsAPI;

namespace SimpleClassicTheme.Taskbar
{
    public struct Window : IEquatable<Window>
    {
        public string GroupingKey;
        public IntPtr Handle;
        internal WINDOWINFO WindowInfo;
        private IntPtr _processHandle;
        private int? _processId;

        public Window(IntPtr handle) : this()
        {
            Handle = handle;
            WindowInfo = WINDOWINFO.FromHWND(handle);
        }

        public string ClassName
        {
            get
            {
                var sb = new StringBuilder(100);
                _ = GetClassName(Handle, sb, sb.Capacity - 1);
                return sb.ToString();
            }
        }

        public IntPtr ProcessHandle
        {
            get
            {
                if (_processHandle == IntPtr.Zero)
                {
                    var dwDesiredAccess = PROCESS_QUERY_LIMITED_INFORMATION | PROCESS_VM_READ;
                    _processHandle = OpenProcess(dwDesiredAccess, false, ProcessId);
                }

                return _processHandle;
            }
        }

        public int ProcessId
        {
            get
            {
                if (!_processId.HasValue)
                {
                    _ = GetWindowThreadProcessId(Handle, out uint pid);
                    _processId = (int)pid;
                }

                return _processId.Value;
            }
        }

        public string Title
        {
            get
            {
                var sb = new StringBuilder(100);
                if (GetWindowText(Handle, sb, sb.Capacity) != 0)
                    return sb.ToString();
                else
                    return string.Empty;
            }
            set
            {
                SetWindowText(Handle, value);
            }
        }

        public static bool operator !=(Window left, Window right)
        {
            return !(left == right);
        }

        public static bool operator ==(Window left, Window right)
        {
            return left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            return obj is Window window && Equals(window);
        }

        public bool Equals(Window other)
        {
            return Handle.Equals(other.Handle);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Handle);
        }
    }
}