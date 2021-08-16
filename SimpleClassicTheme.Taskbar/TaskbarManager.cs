using SimpleClassicTheme.Common.Logging;
using SimpleClassicTheme.Taskbar.Helpers;

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SimpleClassicTheme.Taskbar
{
    internal static class TaskbarManager
    {
        public static SystemTrayNotificationService TrayNotificationService { get; private set; }

        public static IEnumerable<Taskbar> OpenTaskbars
        {
            get
            {
                // We're using a for loop because apparently Application.OpenForm can be modified while iterating
                for (int i = 0; i < Application.OpenForms.Count; i++)
                {
                    var form = Application.OpenForms[i];
                    if (form is Taskbar taskbar && taskbar.ParentForm is not Settings)
                    {
                        yield return taskbar;
                    }
                }
            }
        }

        public static void GenerateNewTaskbars()
        {
            Logger.Instance.Log(LoggerVerbosity.Detailed, "TaskbarManager", "Generating new taskbars");

            DestroyAllTaskbars();

            if (Config.Default.EnablePassiveTray)
            {
                TrayNotificationService ??= new();
            }
            else if (!Config.Default.EnablePassiveTray && TrayNotificationService is not null)
            {
                TrayNotificationService.Dispose();
                TrayNotificationService = null;
            }

            int taskbars = 0;
            foreach (Screen screen in Screen.AllScreens)
            {
                // Skip over non-primary screens if the user only wants one taskbar.
                if (!Config.Default.ShowTaskbarOnAllDesktops && !screen.Primary)
                {
                    continue;
                }

                Taskbar taskbar = new(screen);
                taskbar.Show();

                Logger.Instance.Log(LoggerVerbosity.Detailed, "TaskbarManager", $"Created taskbar in working area: {screen.Bounds}");

                taskbars++;
            }
            Logger.Instance.Log(LoggerVerbosity.Detailed, "TaskbarManager", $"Created {taskbars} taskbars in total");
        }

        public static void DestroyAllTaskbars()
        {
            var activeBars = OpenTaskbars;

            foreach (Taskbar taskbar in activeBars)
            {
                foreach (Control control in taskbar.Controls)
                {
                    control.Dispose();
                }

                taskbar.selfClose = true;
                taskbar.Close();
                taskbar.Dispose();
            }
        }

        private static void VirtualDesktopNotifcation_CurrentDesktopChanged(object sender, EventArgs e)
        {
            // Logger.Instance.Log(LoggerVerbosity.Detailed, "TaskbarManager", "Current virtual desktop changed, sending notification to all Taskbars.");
            //
            // foreach (Taskbar bar in OpenTaskbars)
            // {
            //     // Redo the enumeration because the open windows are completely different
            // bar.Invoke(new Action(() => { bar.EnumerateWindows(); }));
            // }
        }

        public static void Initialize()
        {
            if (ApplicationEntryPoint.VirtualDesktopNotification is not null)
            {
                ApplicationEntryPoint.VirtualDesktopNotification.CurrentDesktopChanged += VirtualDesktopNotifcation_CurrentDesktopChanged;
            }

            if (Config.Default.EnablePassiveTray)
            {
                Logger.Instance.Log(LoggerVerbosity.Detailed, "TaskbarManager", "Initializing new SystemTrayNotificationService");
                TrayNotificationService = new();
            }
        }
    }
}
