using CommandLine;

using SimpleClassicTheme.Common.Logging;

namespace SimpleClassicTheme.Taskbar
{
    internal partial class ApplicationEntryPoint
    {
        [Verb("gui-text", HelpText = "Starts GUI test")]
        internal record GuiTestOptions : Options { }

        [Verb("network-ui", HelpText = "Starts experimental network UI")]
        internal record NetworkUiOptions : Options { }

        [Verb("tray-dump", HelpText = "Dumps tray information")]
        internal record TrayDumpOptions : Options { }

        [Verb("exit", HelpText = "Exits all running SCTT instances")]
        internal record ExitOptions : Options { }

        [Verb("run", true, Hidden = true)]
        internal record Options
        {
            [Option('v', "verbosity", Required = false, HelpText = "Changes logger verbosity.", Default = LoggerVerbosity.Verbose)]
            public LoggerVerbosity Verbosity { get; set; }


            [Option("sct", Required = false, HelpText = "Runs SCT Taskbar in SCT managed mode.", Default = false)]
            public bool SctManagedMode { get; set; }
        }
    }
}
