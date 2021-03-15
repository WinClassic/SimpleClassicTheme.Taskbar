using System;
using System.IO;
using System.Text;

namespace SimpleClassicThemeTaskbar
{
	public enum LoggerVerbosity : int
	{
		None = 0,
		Basic = 1,
		Detailed = 2,
		Verbose = 4
	}

	public static class Logger
	{
		private static LoggerVerbosity verb;
		private static FileStream fs;
		private static bool loggerOff = false;
		public static void Initialize(LoggerVerbosity verbosity)
		{
			SetVerbosity(verbosity);
			fs = File.OpenWrite("./logs/" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"));
			Log(LoggerVerbosity.Basic, "Logger", "Succesfully initialized logger");
		}

		public static void SetVerbosity(LoggerVerbosity verbosity)
		{
			verb = verbosity;
			loggerOff = verb == LoggerVerbosity.None;
		}

		public static void Log(LoggerVerbosity verbosity, string source, string text)
		{
			if (loggerOff) return;
			if (verbosity <= verb)
			{
				string toWrite = $"[{verbosity,-8}][{source,-16}]:[{text}]";
				byte[] bytes = Encoding.UTF8.GetBytes(toWrite);
				fs.Write(bytes, 0, bytes.Length);
			}
		}

		public static void Uninitialize()
		{
			Log(LoggerVerbosity.Basic, "Logger", "Shutting down logger");
			fs.Close();
		}
	}
}
