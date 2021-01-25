using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar
{
    static class ApplicationEntryPoint
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        public const int WM_EXITTASKBAR = 0x0420;

        public static bool SCTCompatMode = false;
        internal static CodeBridge d = new CodeBridge();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Contains("--exit"))
            {
                IntPtr window = File.Exists("C:\\SCT\\Taskbar\\MainWindow.txt") ? new IntPtr(Int32.Parse(File.ReadAllText("C:\\SCT\\Taskbar\\MainWindow.txt"))) : new IntPtr(0);
                IntPtr wParam = new IntPtr(0x5354); //ST
                IntPtr lParam = new IntPtr(0x4F50); //OP
                SendMessage(window, WM_EXITTASKBAR, wParam, lParam);
                return;
            }

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
#if DEBUG
#else
            else 
            {
                MessageBox.Show("This version of SCTT is part of the SCT Private Alpha.\nPlease use the latest public build instead.", "SCT Private alpha");
                return;
            }
#endif

            List<Taskbar> t = new List<Taskbar>();
            //Setup crash reports
#if DEBUG
#else
            Application.ThreadException += (sender, arg) => 
            {
                foreach (Taskbar bar in t)
                {
                    bar.selfClose = true;
                    bar.Close();
                    bar.Dispose();
                }
                MessageBox.Show("Unhandled exception ocurred", "Exiting");
                if (SCTCompatMode)
                {
                    File.WriteAllLines($"C:\\SCT\\Taskbar\\outside-taskbar-crash{DateTime.Now:yyyy-dd-M--HH-mm-ss}", new string[] {
                                arg.Exception.GetType().Name,
                                arg.Exception.StackTrace,
                                arg.Exception.Message,
                                arg.Exception.Source
                            }); 
                }
                Application.Exit();
            };
#endif
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
            Config.SaveToRegistry();
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
                d.SetWorkingArea(rect.Left, rect.Right, rect.Top, rect.Bottom - 28, Environment.OSVersion.Version.Major < 10);
                Taskbar taskbar = new Taskbar(screen.Primary);
                taskbar.ShowOnScreen(screen);
                if (!Config.ShowTaskbarOnAllDesktops && !screen.Primary)
                    taskbar.NeverShow = true;
            }
        }
    }
}
