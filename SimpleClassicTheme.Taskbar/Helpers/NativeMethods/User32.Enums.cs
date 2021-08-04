using System;

namespace SimpleClassicTheme.Taskbar.Helpers.NativeMethods
{
    internal static partial class User32
    {
        internal enum ShellEvents : int
        {
            HSHELL_WINDOWCREATED = 1,
            HSHELL_WINDOWDESTROYED = 2,
            HSHELL_ACTIVATESHELLWINDOW = 3,
            HSHELL_WINDOWACTIVATED = 4,
            HSHELL_GETMINRECT = 5,
            HSHELL_REDRAW = 6,
            HSHELL_TASKMAN = 7,
            HSHELL_LANGUAGE = 8,
            HSHELL_ACCESSIBILITYSTATE = 11,
            HSHELL_APPCOMMAND = 12
        }

        internal enum ShellHookId : int
        {
            WH_SHELL = 0xa,
            WH_CBT = 0x5
        }
    }
}
