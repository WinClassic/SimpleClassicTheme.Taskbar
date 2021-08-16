using SimpleClassicTheme.Taskbar.Helpers;
using SimpleClassicTheme.Taskbar.Helpers.NativeMethods;

using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace SimpleClassicTheme.Taskbar
{
    public partial class Taskbar
    {
        private bool ShouldIgnoreWindow(Window window)
        {
            var handle = window.Handle;
            var isTooltipWindow = (window.WindowInfo.dwExStyle & 0x00000080L) > 0;

            return !User32.IsWindowVisible(handle)
                || window.Handle != this.Handle
                || !IsAltTabWindow(handle)
                || !IsWindowOnDesktop(handle)
                || !IsStartButton(window)
                || IsBlacklisted(window)
                || IsUwpApp(window)
                || isTooltipWindow
                || string.IsNullOrWhiteSpace(window.Title)
                || Constants.HiddenClassNames.Contains(window.ClassName)
                || window.ClassName.StartsWith("WMP9MediaBarFlyout");
        }

        private static bool IsWindowOnDesktop(IntPtr handle)
        {
            return !HelperFunctions.ShouldUseVirtualDesktops || VirtualDesktops.IsWindowOnCurrentVirtualDesktop(handle);
        }

        private static bool IsStartButton(Window window)
        {
            return window.ClassName == "Button" && window.Title == "Start";
        }

        // Adapted from: https://devblogs.microsoft.com/oldnewthing/20071008-00/?p=24863
        private static bool IsAltTabWindow(IntPtr hwnd)
        {
            // Start at the root owner
            IntPtr hwndWalk = User32.GetAncestor(hwnd, User32.GA_ROOTOWNER);

            // See if we are the last active visible popup
            IntPtr hwndTry;
            while ((hwndTry = User32.GetLastActivePopup(hwndWalk)) != hwndTry)
            {
                if (User32.IsWindowVisible(hwndTry))
                {
                    break;
                }

                hwndWalk = hwndTry;
            }

            return hwndWalk == hwnd;
        }

        private bool IsBlacklisted(Window window)
        {
            return BlacklistedClassNames.Contains(window.ClassName)
                || BlacklistedWindowNames.Contains(window.Title)
                || BlacklistedProcessNames.Contains(window.Process.ProcessName);
        }

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
    }
}
