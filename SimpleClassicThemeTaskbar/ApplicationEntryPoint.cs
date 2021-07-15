using Microsoft.Win32;

using SimpleClassicThemeTaskbar.Helpers;
using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace SimpleClassicThemeTaskbar
{
    internal static class ApplicationEntryPoint
    {
        public static int ErrorCount = 0;
        public static UserControlEx ErrorSource;

        public static bool SCTCompatMode = false;

        static UnmanagedCodeMigration.VirtualDesktopNotification VirtualDesktopNotification;
        static IntPtr virtualDesktopNotificationCookie;

        internal static void ExitSCTT()
        {
            Logger.Log(LoggerVerbosity.Detailed, "TaskbarManager", $"Exit requested");
            Config.Instance.SaveToRegistry();

            DestroyAllTaskbars();

            // Taskbar randomBar = activeBars.FirstOrDefault();
            Logger.Log(LoggerVerbosity.Detailed, "TaskbarManager", $"Killed all taskbars, exiting");
            Logger.Uninitialize();

            if (Environment.OSVersion.Version.Major == 10)
                UnmanagedCodeMigration.UnregisterVdmNotification(virtualDesktopNotificationCookie);

            Environment.Exit(0);
        }

        #region Taskbar Management
        internal static void GenerateNewTaskbars()
        {
            Logger.Log(LoggerVerbosity.Detailed, "TaskbarManager", "Generating new taskbars");

            DestroyAllTaskbars();

            if (Config.Instance.ShowTaskbarOnAllDesktops)
            {
                int nTaskbars = 0;
                foreach (Screen screen in Screen.AllScreens)
                {
                    nTaskbars++;
                    CreateTaskbar(screen);
                }
                Logger.Log(LoggerVerbosity.Detailed, "TaskbarManager", $"Created {nTaskbars} taskbars in total");
            }
            else
            {
                CreateTaskbar(Screen.PrimaryScreen);
            }
        }

        static void DestroyAllTaskbars()
        {
            var activeBars = Helpers.Helpers.GetOpenTaskbars();

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

        static void CreateTaskbar(Screen screen)
        {
            Taskbar taskbar = new(screen.Primary);
            taskbar.ShowOnScreen(screen);
            Logger.Log(LoggerVerbosity.Detailed, "TaskbarManager", $"Created taskbar in working area: {screen.Bounds}");
        }

        #endregion

        private static void ShowCrashMessageBox(Exception ex)
        {
            Logger.Log(LoggerVerbosity.Basic, "ExceptionHandler", $"An exception of type {ex.GetType().FullName} has occured.");
            Logger.Log(LoggerVerbosity.Detailed, "ExceptionHandler", $"Exception details:\nMessage: {ex.Message}\nException location: {ex.TargetSite}\nStack trace: {ex.StackTrace}");
            
            if (Logger.GetVerbosity() == LoggerVerbosity.None)
            {
                MessageBox.Show("Unhandled exception ocurred. For more information logging must be enabled.", "Something has gone wrong");
                return;
            }

            var dialogResult = MessageBox.Show(
                    "Unhandled exception ocurred. More information is available in the log file. Would you like to open the log file now?",
                    "Something has gone wrong",
                    MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                Logger.OpenLog();
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Kernel32.AttachConsole(Kernel32.ATTACH_PARENT_PROCESS);

            Logger.Initialize(LoggerVerbosity.Verbose);

            var rootCommand = new RootCommand
            {
                new Option<LoggerVerbosity>(
                    new[] { "--verbose", "-v" },
                    getDefaultValue: () => LoggerVerbosity.Basic,
                    description: "Sets the verbosity")
                {
                    Name = "verbosity",
                },

                new Option<bool>("--sct", "Runs in SCT-managed mode")
                {
                    Name = "isSctManaged",
                    Arity = ArgumentArity.ZeroOrOne,
                },
            };
            rootCommand.Handler = CommandHandler.Create<LoggerVerbosity, bool>(RunSctt);
            rootCommand.Add(new Command("exit", "Closes all running SCTT instances")
            {
                Handler = CommandHandler.Create(EndScttInstances),
            });
            rootCommand.Add(new Command("network-ui", "Runs experimental network manager UI")
            {
                Handler = CommandHandler.Create(RunNetworkManager),
            });
            rootCommand.Add(new Command("gui-test", "Runs graphics test")
            {
                Handler = CommandHandler.Create(RunGraphicsTest),
            });

            // Parse the incoming args and invoke the handler
            _ = rootCommand.InvokeAsync(args).Result;
        }

        [STAThread]
        private static void RunSctt(LoggerVerbosity verbosity, bool isSctManaged)
        {
            //Logger.SetVerbosity(verbosity);

            Logger.Log(LoggerVerbosity.Basic, "EntryPoint", verbosity.ToString());
            Logger.Log(LoggerVerbosity.Basic, "EntryPoint", isSctManaged.ToString());


            if (isSctManaged)
            {
                Logger.Log(LoggerVerbosity.Detailed, "EntryPoint", "Current instance is an SCT managed instance");
                SCTCompatMode = true;
            }
#if !DEBUG
            else
            {
                MessageBox.Show("SCT Taskbar does not currently work without SCT. Please install SCTT via the Options menu in SCT 1.2.0 or higher", "SCT required");
                ExitSCTT();
            }
#endif

            //Setup crash reports
#if !DEBUG
            Logger.Log(LoggerVerbosity.Detailed, "EntryPoint", "Release instance, enabling error handler");
            Application.ThreadException += (sender, arg) => ShowCrashMessageBox(arg.Exception);
#endif
            //Check if system/program architecture matches
            if (Environment.Is64BitOperatingSystem != Environment.Is64BitProcess)
            {
                string sysArch = Environment.Is64BitOperatingSystem ? "x64" : "x86";
                string processArch = Environment.Is64BitProcess ? "x64" : "x86";
                MessageBox.Show($"You are trying to run a {processArch} version of SCT Taskbar on a {sysArch} version of Windows. This is not supported. Please download a {sysArch} version of SCT Taskbar", "Incorrect architecture");
#if !DEBUG
                return;
#endif
                Logger.Log(LoggerVerbosity.Detailed, "EntryPoint", "Debug instance, ignoring incorrect architecture");
            }

            if (false && Environment.OSVersion.Version.Major == 10)
            {
                Logger.Log(LoggerVerbosity.Detailed, "EntryPoint", "OSVersion.Major is above 10. Initializing IVirtualDesktopManager");
                UnmanagedCodeMigration.InitializeVdmInterface();
                VirtualDesktopNotification = new();
                VirtualDesktopNotification.CurrentDesktopChanged += VirtualDesktopNotifcation_CurrentDesktopChanged;
                virtualDesktopNotificationCookie = UnmanagedCodeMigration.RegisterVdmNotification(VirtualDesktopNotification);
            }

            Logger.Log(LoggerVerbosity.Detailed, "EntryPoint", "Main initialization done, passing execution to TaskbarManager");

            Directory.CreateDirectory(Constants.VisualStyleDirectory);

            Application.VisualStyleState = VisualStyleState.NoneEnabled;
            Application.SetCompatibleTextRenderingDefault(false);
            SystemEvents.DisplaySettingsChanged += (s, e) => GenerateNewTaskbars();
            GenerateNewTaskbars();
            Application.Run();
            ExitSCTT();
        }

        private static void RunGraphicsTest()
        {
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Forms.GraphicsTest());
        }

        public static void RunNetworkManager()
        {
            Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.NoneEnabled;
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Forms.NetworkUI());
        }

        public static void EndScttInstances()
        {
            Logger.Log(LoggerVerbosity.Detailed, "EntryPoint", "Killing all SCTT instances");
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

        public static void VirtualDesktopNotifcation_CurrentDesktopChanged(object sender, EventArgs e)
		{
            Logger.Log(LoggerVerbosity.Detailed, "TaskbarManager", "Current virtual desktop changed, sending notification to all Taskbars.");
            var activeBars = Helpers.Helpers.GetOpenTaskbars();

            foreach (Taskbar bar in activeBars)
            {
                // Redo the enumeration because the open windows are completely different
                bar.Invoke(new Action(() => { bar.EnumerateWindows(); }));
            }
        }
    }
}