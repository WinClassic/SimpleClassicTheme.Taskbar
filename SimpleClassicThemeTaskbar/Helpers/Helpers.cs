using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar.Helpers
{
    internal class Helpers
    {
        public static IEnumerable<Taskbar> GetOpenTaskbars()
        {
            // We're using a for loop because apparently Application.OpenForm can be modified while iterating
            for (int i = 0; i < Application.OpenForms.Count; i++)
            {
                var form = Application.OpenForms[i];
                if (form is Taskbar taskbar)
                {
                    yield return taskbar;
                }
            }
        }

        public static void OpenQuickLaunchFolder()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var quickLaunchPath = Path.Combine(appDataPath, @"Microsoft\Internet Explorer\Quick Launch");

            _ = Process.Start(new ProcessStartInfo
            {
                FileName = quickLaunchPath,
                UseShellExecute = true,
            });
        }
    }
}