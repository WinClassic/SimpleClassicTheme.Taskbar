using SimpleClassicTheme.Taskbar.Helpers;
using SimpleClassicTheme.Taskbar.Helpers.NativeMethods;

using System;
using System.Linq;
using System.Runtime.InteropServices;

using static SimpleClassicTheme.Taskbar.Native.Headers.PSAPI;
using static SimpleClassicTheme.Taskbar.Native.Headers.WinUser;

namespace SimpleClassicTheme.Taskbar
{
    public partial class Taskbar
    {
        public static bool IsUwpApp(Window window)
        {
            if (window.ClassName == "ApplicationFrameWindow")
            {
                //Do an API call to see if app isn't cloaked
                _ = DwmApi.DwmGetWindowAttribute(window.Handle, DwmApi.DWMWINDOWATTRIBUTE.Cloaked, out var d, Marshal.SizeOf(0));

                //If returned value is not 0, the window is cloaked
                return d != 0;
            }

            return false;
        }

        // Adapted from: https://devblogs.microsoft.com/oldnewthing/20071008-00/?p=24863
        private static bool IsAltTabWindow(IntPtr hwnd)
        {
            // Start at the root owner
            IntPtr hwndWalk = GetAncestor(hwnd, GA_ROOTOWNER);

            // See if we are the last active visible popup
            IntPtr hwndTry;
            while ((hwndTry = GetLastActivePopup(hwndWalk)) != hwndTry)
            {
                if (IsWindowVisible(hwndTry))
                {
                    break;
                }

                hwndWalk = hwndTry;
            }

            return hwndWalk == hwnd;
        }

        private static bool IsStartButton(Window window)
        {
            return window.ClassName == "Button" && window.Title == "Start";
        }

        private static bool IsWindowOnDesktop(IntPtr handle)
        {
            return !HelperFunctions.ShouldUseVirtualDesktops || VirtualDesktops.IsWindowOnCurrentVirtualDesktop(handle);
        }

        private bool IsBlacklisted(Window window)
        {
            return BlacklistedClassNames.Contains(window.ClassName)
                || BlacklistedWindowNames.Contains(window.Title)
                || BlacklistedProcessNames.Contains(GetProcessModuleName(window.ProcessHandle));
        }

        private bool ShouldIgnoreWindow(Window window)
        {
            var handle = window.Handle;
            var isTooltipWindow = (window.WindowInfo.dwExStyle & 0x00000080L) > 0;

            return !IsWindowVisible(handle)
                || window.Handle == this.Handle
                || !IsAltTabWindow(handle)
                || !IsWindowOnDesktop(handle)
                || IsStartButton(window)
                || IsBlacklisted(window)
                || IsUwpApp(window)
                || isTooltipWindow
                || string.IsNullOrWhiteSpace(window.Title)
                || Constants.HiddenClassNames.Contains(window.ClassName)
                || window.ClassName.StartsWith("WMP9MediaBarFlyout");
        }
    }
}