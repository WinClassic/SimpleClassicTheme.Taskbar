/*
 *
 * This file contains the current stable code for
 * haandling with taskbar windows (pulling data).
 * The idea is that we add new code to Taskbar.cs
 * and keep this in place as a backup for when
 * things go wrong with the new system (getting
 * pushed data).
 *
 */

using SimpleClassicTheme.Common.Providers;
using SimpleClassicTheme.Taskbar.Helpers;
using SimpleClassicTheme.Taskbar.Helpers.NativeMethods;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using static SimpleClassicTheme.Taskbar.Native.Headers.WinDef;
using static SimpleClassicTheme.Taskbar.Native.Headers.WinUser;

namespace SimpleClassicTheme.Taskbar
{
    public partial class Taskbar : Form
    {
        public static IEnumerable<Window> GetTaskbarWindows()
        {
            var trayWindows = UnmanagedHelpers.FilterWindows(_ => true);
            return trayWindows.Select(hWnd => new Window(hWnd));
        }

        private IEnumerable<Window> GetTrayWindows()
        {
            if (_screen == null)
            {
                return Array.Empty<Window>();
            }

            var trayWindows = UnmanagedHelpers.FilterWindows((hWnd) =>
            {
                var window = new Window(hWnd);
                var isTrayWindow = window.ClassName == "Shell_TrayWnd" || window.ClassName == "Shell_SecondaryTrayWnd";
                return isTrayWindow && Screen.FromHandle(hWnd).Bounds == _screen.Bounds;
            });

            return trayWindows.Select(hWnd => new Window(hWnd));
        }

        private void TimerUpdateInformation_Tick(object sender, EventArgs e)
        {
            if (!watchLogic)
                logicTiming.Start();

            using (var _ = logicTiming.StartRegion("Hide Shell_TrayWnd and Shell_SecondaryTrayWnd"))
            {
                HideExplorerTaskbars();
            }

            using (var _ = logicTiming.StartRegion("Hide if ForegroundWindow is fullscreen"))
            {
                Window fgWindow = new(GetForegroundWindow());
                //The window should only be visible if the active window is not fullscreen (with the exception of the desktop window)
                Screen scr = Screen.FromHandle(fgWindow.Handle);
                GetWindowRect(fgWindow.Handle, out RECT rect);
                int width = rect.Right - rect.Left, height = rect.Bottom - rect.Top;
                bool full = width >= scr.Bounds.Width && height >= scr.Bounds.Height;
                if (NeverShow)
                    Visible = false;
                else
                    Visible = !(full && fgWindow.ClassName != "Progman" && fgWindow.ClassName != "WorkerW");

                //If we're not visible, why do anything?
                if (!Visible)
                    return;
            }

            if (Provider is FetchProvider<Window> fetchProvider)
            {
                fetchProvider.Update();
            }

            if (!watchLogic)
            {
                watchLogic = true;
                logicTiming.Stop();
                logicTiming.Show();
            }

            //Update UI
            TimerUpdateUI_Tick(true, null);
        }

        private void TimerUpdateUI_Tick(object sender, EventArgs e)
        {
            if (!watchUI)
            {
                uiTiming.Reset();
                uiTiming.Start();
            }

            UpdateUI();

            if (!watchUI)
            {
                watchUI = true;
                uiTiming.Stop();
                uiTiming.Show();
            }
        }
    }
}