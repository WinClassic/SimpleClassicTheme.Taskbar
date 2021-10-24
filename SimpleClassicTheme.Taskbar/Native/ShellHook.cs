using SimpleClassicTheme.Common.Logging;

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using static SimpleClassicTheme.Taskbar.Native.Headers.WinUser;

namespace SimpleClassicTheme.Taskbar.Native
{
    internal class ShellHook
    {
        public int WindowMessage { get; private set; }

        /// <summary>
        /// A top-level, unowned window has been created. The window exists when the system calls this hook.
        /// </summary>
        public event EventHandler<IntPtr> WindowCreated;

        /// <summary>
        /// The activation has changed to a different top-level, unowned window.
        /// </summary>
        public event EventHandler<IntPtr> WindowActivated;

        /// <summary>
        /// A top-level, unowned window is about to be destroyed. The window still exists when the system calls this hook.
        /// </summary>
        public event EventHandler<IntPtr> WindowDestroyed;

        /// <summary>
        /// A top-level window is being replaced. The window exists when the system calls this hook.
        /// </summary>
        public event EventHandler<IntPtr> WindowReplaced;

        /// <summary>
        /// The title of a window in the task bar has been redrawn.
        /// </summary>
        public event EventHandler<IntPtr> WindowRedrawn;

        public ShellHook(IntPtr hWnd)
        {
            //hookProcedure = new WindowsHookProcedure(HookProcedure);
            //if (SetWindowsHookEx(ShellHookId.WH_SHELL, hookProcedure, Marshal.GetHINSTANCE(typeof(Taskbar).Module), /*GetCurrentThreadId()*/0) == IntPtr.Zero)
            
            if (!RegisterShellHookWindow(hWnd))
            {
                int errorCode = Marshal.GetLastWin32Error();
                Logger.Instance.Log(LoggerVerbosity.Basic, "ShellHook", $"Failed to create Shell Hook. ({errorCode:X8})");
                throw new Win32Exception(errorCode);
            }

            WindowMessage = RegisterWindowMessage("SHELLHOOK");

            if (WindowMessage == 0)
            {
                int errorCode = Marshal.GetLastWin32Error();
                Logger.Instance.Log(LoggerVerbosity.Basic, "ShellHook", $"Failed to register Shell Hook message. ({errorCode:X8})");
                throw new Win32Exception(errorCode);
            }
        }

        public IntPtr HandleWindowMessage(IntPtr wParam, IntPtr lParam)
        {
            var shellEvent = (ShellEvents)wParam;
            wParam = lParam;
            lParam = IntPtr.Zero;

            Logger.Instance.Log(LoggerVerbosity.Verbose, "ShellHook", $"Call parameters\t\t\t{shellEvent}\t\t{wParam:X8}\t{lParam:X8}");

            if (shellEvent >= 0)
            {
                HandleShellEvent(shellEvent, wParam, lParam);
            }

            return CallNextHookEx(IntPtr.Zero, shellEvent, wParam, lParam);
        }

        private void HandleShellEvent(ShellEvents shellEvent, IntPtr wParam, IntPtr lParam)
        {
            switch (shellEvent)
            {
                case ShellEvents.HSHELL_WINDOWCREATED:
                    WindowCreated?.Invoke(this, wParam);
                    break;

                case ShellEvents.HSHELL_WINDOWACTIVATED:
                case ShellEvents.HSHELL_RUDEAPPACTIVATED:
                    WindowActivated?.Invoke(this, wParam);
                    break;

                case ShellEvents.HSHELL_WINDOWDESTROYED:
                    WindowDestroyed?.Invoke(this, wParam);
                    break;

                case ShellEvents.HSHELL_REDRAW:
                    WindowRedrawn?.Invoke(this, wParam);
                    break;

                case ShellEvents.HSHELL_WINDOWREPLACED:
                    WindowReplaced?.Invoke(this, wParam);
                    break;

                default:
                    Logger.Instance.Log(LoggerVerbosity.Verbose,
                                        "ShellHook",
                                        $"Cannot handle\t\t\t{(int)shellEvent}\t\t{wParam:X8}\t{lParam:X8}");
                    break;
            }
        }
    }
}
