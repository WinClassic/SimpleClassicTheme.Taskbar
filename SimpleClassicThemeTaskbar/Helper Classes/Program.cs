using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CrashReporterDotNET;
using SimpleClassicThemeTaskbar.Cpp.CLI;

namespace SimpleClassicThemeTaskbar
{
    static class Program
    {
        public static bool SCTCompatMode = false;
        static Interop d = new Interop();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Contains("--gui-test")) 
            {
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Forms.GraphicsTest());
                return;
            }

            if (args.Contains("--network-ui"))
            {
                Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.NoneEnabled;
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Forms.NetworkUI());
                return;
            }

            if (args.Contains("--sct"))
			{
                SCTCompatMode = true;
            }

            List<Taskbar> t = new List<Taskbar>();
            //Setup crash reports
            Application.ThreadException += (sender, arg) => 
            {
#if DEBUG
                throw arg.Exception;
#else
                foreach (Taskbar bar in t)
                {
                    bar.selfClose = true;
                    bar.Close();
                    bar.Dispose();
                }
                var reportCrash = new ReportCrash("")
                {
                    DeveloperMessage = "Please save the file and attach it to a github issue"
                };
                reportCrash.Send(arg.Exception);
                Environment.Exit(arg.Exception.HResult);
#endif
            };
            //Check if system/program architecture matches
            if (Environment.Is64BitOperatingSystem != Environment.Is64BitProcess)
            {
                string sysArch = Environment.Is64BitOperatingSystem ? "x64" : "x86";
                string processArch = Environment.Is64BitProcess ? "x64" : "x86";
                MessageBox.Show($"You are trying to run a {processArch} version of SCT Taskbar on a {sysArch} version of Windows. This is not supported. Please download a {sysArch} version of SCT Taskbar", "Incorrect architecture");
#if DEBUG
#else
                return;
#endif
            }   
            Application.SetCompatibleTextRenderingDefault(false);
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += delegate
            {
                NewTaskbars();
            };
            NewTaskbars();
            Application.Run();
        }

        internal static void NewTaskbars()
        {
            List<Taskbar> activeBars = new List<Taskbar>();
            foreach (Form form in Application.OpenForms)
                if (form is Taskbar bar)
                    activeBars.Add(bar);
            foreach (Taskbar bar in activeBars)
            {
                foreach (Control d in bar.Controls)
                    d.Dispose();
                bar.selfClose = true;
                bar.Close();
                bar.Dispose();
            }
            foreach (Screen screen in Screen.AllScreens)
            {
                Rectangle rect = screen.Bounds;
                d.SetWorkingArea(rect.Left, rect.Right, rect.Top, rect.Bottom - 28);
                Taskbar taskbar = new Taskbar(screen.Primary);
                taskbar.ShowOnScreen(screen);
                if (!Config.ShowTaskbarOnAllDesktops && !screen.Primary)
                    taskbar.NeverShow = true;
            }
        }
    }
}
