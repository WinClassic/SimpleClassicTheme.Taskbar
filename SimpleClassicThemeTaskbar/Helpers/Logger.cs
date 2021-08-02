using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace SimpleClassicThemeTaskbar.Helpers
{
    public enum LoggerVerbosity : int
    {
        None = 0,

        /// <summary>
        /// Log messages about basic information
        /// </summary>
        Basic = 1,

        /// <summary>
        /// Log messages about program flow
        /// </summary>
        Detailed = 2,

        /// <summary>
        /// Log messages for debugging or about received values
        /// </summary>
        Verbose = 3
    }

    public static class Logger
    {
        private static FileStream fs;
        private static object lockObject = new();
        private static bool loggerOff = false;
        private static StreamWriter sw;
        private static LoggerVerbosity verb;
        public static string FilePath { get; private set; }

        public static FileStream FileStream => fs;

        public static LoggerVerbosity GetVerbosity() => verb;

        public static void Initialize(LoggerVerbosity verbosity)
        {
            SetVerbosity(verbosity);
            if (loggerOff)
                return;

            fs = new FileStream(FilePath = "latest.log", FileMode.Create, FileAccess.Write, FileShare.Read);
            sw = new StreamWriter(fs) { AutoFlush = true };
            Log(LoggerVerbosity.Basic, "Logger", "Succesfully initialized logger");

            Log(LoggerVerbosity.Detailed, "SystemDump", "Performing quick system dump");
            Log(LoggerVerbosity.Detailed, "SystemDump", $"OS: {RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture}");

            if (ApplicationEntryPoint.SCTCompatMode)
                Log(LoggerVerbosity.Detailed, "SystemDump", $"SCT version: {Assembly.LoadFrom("C:\\SCT\\SCT.exe").GetName().Version}");

            Log(LoggerVerbosity.Detailed, "SystemDump", $"SCT Taskbar version: {Assembly.GetExecutingAssembly().GetName().Version}");
        }

        public static void Log(LoggerVerbosity verbosity, string source, string text)
        {
            if (Config.Default.Tweaks.EnableDebugging)
                Debug.WriteLine(text, source);

            // text.Replace("\n", "".PadLeft(38));

            if (loggerOff) return;
            if (verbosity <= verb)
            {
                lock (lockObject)
                {
                    sw.WriteLine($"[{verbosity,-8}][{source,-24}]: {text}");
                }
            }
        }

        public static void SetVerbosity(LoggerVerbosity verbosity)
        {
            verb = verbosity;
            loggerOff = verb == LoggerVerbosity.None;
        }

        public static void Uninitialize()
        {
            Log(LoggerVerbosity.Basic, "Logger", "Shutting down logger");
            fs.Close();
            loggerOff = true;
        }
    }
}