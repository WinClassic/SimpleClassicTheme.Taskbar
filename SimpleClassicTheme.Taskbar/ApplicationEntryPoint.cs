using Microsoft.Win32;

using SimpleClassicTheme.Common.ErrorHandling;
using SimpleClassicTheme.Common.Logging;
using SimpleClassicTheme.Taskbar.Forms;
using SimpleClassicTheme.Taskbar.Helpers;
using SimpleClassicTheme.Taskbar.Helpers.NativeMethods;

using SimpleClassicThemeTaskbar;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleClassicTheme.Taskbar
{
    internal static class ApplicationEntryPoint
    {
        private static readonly ErrorHandler errorHandler = new() { SentryDsn = "https://eadebff4c83e42e4955d85403ff0cc7c@o925637.ingest.sentry.io/5874762" };
        private static uint virtualDesktopNotificationCookie;
        public static UnmanagedCodeMigration.VirtualDesktopNotification VirtualDesktopNotification { get; private set; }
        public static bool SCTCompatMode = false;
        public static int ErrorCount = 0;
        public static UserControlEx ErrorSource;

        internal static void ExitSCTT()
        {
            Logger.Instance.Log(LoggerVerbosity.Detailed, "EntryPoint", $"Exit requested");
            Config.Default.WriteToRegistry();

            TaskbarManager.DestroyAllTaskbars();

            Logger.Instance.Log(LoggerVerbosity.Detailed, "EntryPoint", $"Killed all taskbars, exiting");
            Logger.Instance.Dispose();

            if (Environment.OSVersion.Version.Major >= 10 && virtualDesktopNotificationCookie != 0)
                UnmanagedCodeMigration.UnregisterVdmNotification(virtualDesktopNotificationCookie);

            Environment.Exit(0);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            LoggerVerbosity verbosity = LoggerVerbosity.Verbose;

            foreach (var arg in args)
            {
                if (arg.StartsWith("-v="))
                    verbosity = (LoggerVerbosity)int.Parse(arg[3..]);
            }

            Logger.Instance.Initialize(verbosity);

            Logger.Instance.Log(LoggerVerbosity.Detailed, "EntryPoint", "Parsing arguments");
            if (args.Contains("--exit"))
            {
                Logger.Instance.Log(LoggerVerbosity.Detailed, "EntryPoint", "Killing all SCTT instances");
                static List<IntPtr> EnumerateProcessWindowHandles(int processId, string name)
                {
                    List<IntPtr> handles = new();

                    foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
                    {
                        User32.EnumThreadWindows(thread.Id, (hWnd, lParam) =>
                        {
                            handles.Add(hWnd);
                            return true;
                        }, IntPtr.Zero);
                    }
                    return handles;
                }

                Process[] scttInstances = Process.GetProcessesByName("SimpleClassicThemeTaskbar");
                Array.ForEach(scttInstances, a =>
                {
                    List<IntPtr> handles = EnumerateProcessWindowHandles(a.Id, "SCTT_Shell_TrayWnd");
                    string s = "";
                    foreach (IntPtr handle in handles)
                    {
                        StringBuilder builder = new(1000);
                        User32.GetClassName(handle, builder, 1000);

                        if (builder.Length > 0)
                            s = s + builder.ToString() + "\n";

                        IntPtr returnValue = User32.SendMessage(handle, Constants.WM_SCT, new IntPtr(Constants.SCTWP_ISSCT), IntPtr.Zero);
                        if (returnValue != IntPtr.Zero)
                        {
                            User32.SendMessage(handle, Constants.WM_SCT, new IntPtr(Constants.SCTWP_EXIT), IntPtr.Zero);
                        }
                    }
                });
            }
            if (args.Contains("--gui-test"))
            {
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new GraphicsTest());
                return;
            }
            if (args.Contains("--network-ui"))
            {
                Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.NoneEnabled;
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new NetworkUI());
                return;
            }
            if (args.Contains("--unmanaged"))
            {
                Logger.Instance.Log(LoggerVerbosity.Detailed, "EntryPoint", "Attempted to start unmanaged SCTT. This is not supported anymore.");
                Environment.Exit(/*d.UnmanagedSCTT()*/0);
            }
            if (args.Contains("--traydump"))
            {
                Logger.Instance.Log(LoggerVerbosity.Detailed, "EntryPoint", "Dumping system tray information to traydump.txt");
                FileStream fs = new("traydump.txt", FileMode.Create, FileAccess.ReadWrite);

                IntPtr hWndTray = User32.FindWindow("Shell_TrayWnd", null);
                if (hWndTray != IntPtr.Zero)
                {
                    hWndTray = User32.FindWindowEx(hWndTray, IntPtr.Zero, "TrayNotifyWnd", null);
                    if (hWndTray != IntPtr.Zero)
                    {
                        hWndTray = User32.FindWindowEx(hWndTray, IntPtr.Zero, "SysPager", null);
                        if (hWndTray != IntPtr.Zero)
                        {
                            hWndTray = User32.FindWindowEx(hWndTray, IntPtr.Zero, "ToolbarWindow32", null);
                            if (hWndTray != IntPtr.Zero)
                            {
                                UnmanagedCodeMigration.TBBUTTONINFO[] buttons = UnmanagedCodeMigration.GetTrayButtons(hWndTray);
                                foreach (var button in buttons)
                                {
                                    string str = "START OF TBBUTTONINFO\n";
                                    foreach (var d in button.GetType().GetProperties())
                                        str += d.Name + ": " + d.GetValue(button) + "\n";
                                    foreach (var d in button.GetType().GetFields())
                                        str += d.Name + ": " + d.GetValue(button) + "\n";
                                    str += "END OF TBBUTTONINFO\n";
                                    byte[] bytes = Encoding.UTF8.GetBytes(str);
                                    fs.Write(bytes, 0, bytes.Length);
                                    fs.Flush();
                                }
                            }
                        }
                    }
                }
                fs.Close();
                return;
            }
            if (args.Contains("--sct"))
            {
                Logger.Instance.Log(LoggerVerbosity.Detailed, "EntryPoint", "Current instance is an SCT managed instance");
                SCTCompatMode = true;
            }
#if DEBUG
#else
            else
            {
                MessageBox.Show("SCT Taskbar does not currently work without SCT. Please install SCTT via the Options menu in SCT 1.2.0 or higher", "SCT required");
                ExitSCTT();
            }
#endif

            //Setup crash reports
            Logger.Instance.Log(LoggerVerbosity.Detailed, "EntryPoint", "Release instance, enabling error handler");
            errorHandler.SubscribeToErrorEvents();


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
                Logger.Instance.Log(LoggerVerbosity.Detailed, "EntryPoint", "Debug instance, ignoring incorrect architecture");
            }

            if (Environment.OSVersion.Version.Major >= 10 && Process.GetProcessesByName("explorer").Length > 0)
            {
                Logger.Instance.Log(LoggerVerbosity.Detailed, "EntryPoint", "OSVersion.Major is equal or above 10. Initializing IVirtualDesktopManager");

                try
                {
                    UnmanagedCodeMigration.InitializeVdmInterface();
                    VirtualDesktopNotification = new();
                    virtualDesktopNotificationCookie = UnmanagedCodeMigration.RegisterVdmNotification(VirtualDesktopNotification);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(LoggerVerbosity.Detailed, "EntryPoint", "Failed to initialize VDM!\n" + ex.ToString());
                }
            }

            Directory.CreateDirectory(Constants.VisualStyleDirectory);

            //Application.EnableVisualStyles();
            Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.NoneEnabled;
            Application.SetCompatibleTextRenderingDefault(false);

            Logger.Instance.Log(LoggerVerbosity.Detailed, "EntryPoint", "Main initialization done, passing execution to TaskbarManager");

            SystemEvents.DisplaySettingsChanged += delegate
            {
                TaskbarManager.GenerateNewTaskbars();
            };

            TaskbarManager.Initialize();
            TaskbarManager.GenerateNewTaskbars();

            Application.Run();

            ExitSCTT();
        }
    }
}