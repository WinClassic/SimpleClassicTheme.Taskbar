using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SimpleClassicThemeTaskbar.Helpers
{
    public class TimingDebugger
    {
        public List<(string Label, TimeSpan Elapsed)> Times = new();
        private readonly Stopwatch stopwatch = new();

        public void FinishRegion(string label)
        {
            Times.Add((label, stopwatch.Elapsed));
            stopwatch.Reset();
            stopwatch.Start();
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
        }

        public void Stop()
        {
            stopwatch.Stop();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Watch Logic:");

            foreach (var (Label, Elapsed) in Times)
            {
                sb.AppendLine(Elapsed.ToString() + '\t' + Label);
            }

            sb.AppendLine(stopwatch.Elapsed.ToString() + '\t' + "Final bit");

            return sb.ToString();
        }
    }
}