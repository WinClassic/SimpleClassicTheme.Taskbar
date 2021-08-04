using SimpleClassicTheme.Taskbar.Forms;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SimpleClassicTheme.Taskbar.Helpers
{
    public class TimingDebugger
    {
        public List<Region> Times = new();
        private readonly Stopwatch stopwatch = new();
        private TimeSpan regionTimestamp = TimeSpan.Zero;
        public TimeSpan StopTime { get; private set; }
        public TimeSpan Elapsed => stopwatch.Elapsed;

        [Obsolete]
        public void FinishRegion(string label)
        {
            var region = new Region
            {
                Timestamp = regionTimestamp,
                EndTime = stopwatch.Elapsed,
                Label = label,
            };

            Times.Add(region);
            regionTimestamp = stopwatch.Elapsed;
            // stopwatch.Reset();
            // stopwatch.Start();
        }

        public Marker StartRegion(string label)
        {
            return new Marker(this, label);
        }

        public void Reset()
        {
            stopwatch.Reset();
            Times.Clear();
        }

        public void Start()
        {
            stopwatch.Reset();
            stopwatch.Start();
            regionTimestamp = TimeSpan.Zero;
        }

        public void Stop()
        {
            StopTime = stopwatch.Elapsed;
            stopwatch.Stop();
        }
        
        public void ShowDialog()
        {
            using (var dialog = new TimingDebuggerForm(this))
            {
                dialog.ShowDialog();
            }
        }

        public void Show()
        {
            var dialog = new TimingDebuggerForm(this);
            dialog.Show();
        }
        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var region in Times)
            {
                sb.AppendLine(region.Duration.ToString() + '\t' + region.Label);
            }

            sb.AppendLine(stopwatch.Elapsed.ToString() + '\t' + "Final bit");

            return sb.ToString();
        }
        
        public record Region
        {
            public string Label { get; init; }
            public TimeSpan Timestamp { get; init; }
            public TimeSpan EndTime { get; init; }

            public TimeSpan Duration => EndTime - Timestamp;
        }

        public class Marker : IDisposable
        {
            public TimingDebugger Debugger { get; }

            public string Label { get; }

            public TimeSpan StartTime { get; }

            public Marker(TimingDebugger debugger, string label)
            {
                StartTime = debugger.Elapsed;
                Debugger = debugger;
                Label = label;
            }

            public void Dispose()
            {
                if (Debugger.stopwatch.IsRunning)
                {
                    var region = new Region
                    {
                        Timestamp = StartTime,
                        EndTime = Debugger.Elapsed,
                        Label = Label,
                    };

                    Debugger.Times.Add(region);
                }
            }
        }
    }

}