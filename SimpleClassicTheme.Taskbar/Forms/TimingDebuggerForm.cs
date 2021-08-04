using SimpleClassicTheme.Taskbar.Helpers;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimpleClassicTheme.Taskbar.Forms
{
    public partial class TimingDebuggerForm : Form
    {
        private readonly TimingDebugger debugger;

        public TimingDebuggerForm()
        {
            InitializeComponent();
        }

        public TimingDebuggerForm(TimingDebugger debugger) : this()
        {
            this.debugger = debugger;
        }

        private int StepSize => (int)(debugger.StopTime.TotalMilliseconds / 10);

        public static Color ColorFromHSL(double h, double s, double l)
        {
            double r = 0, g = 0, b = 0;
            if (l != 0)
            {
                if (s == 0)
                    r = g = b = l;
                else
                {
                    double temp2;
                    if (l < 0.5)
                        temp2 = l * (1.0 + s);
                    else
                        temp2 = l + s - (l * s);

                    double temp1 = 2.0 * l - temp2;

                    r = GetColorComponent(temp1, temp2, h + 1.0 / 3.0);
                    g = GetColorComponent(temp1, temp2, h);
                    b = GetColorComponent(temp1, temp2, h - 1.0 / 3.0);
                }
            }
            return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
        }

        private static Color GetColor(TimingDebugger.Region r, bool intense)
        {
            var random = new Random(r.Label.GetHashCode());
            var hue = random.NextDouble();
            var saturation = intense ? 1d : .35d;

            return ColorFromHSL(hue, saturation, .5d);
        }

        private static double GetColorComponent(double temp1, double temp2, double temp3)
        {
            if (temp3 < 0.0)
                temp3 += 1.0;
            else if (temp3 > 1.0)
                temp3 -= 1.0;

            if (temp3 < 1.0 / 6.0)
                return temp1 + (temp2 - temp1) * 6.0 * temp3;
            else if (temp3 < 0.5)
                return temp2;
            else if (temp3 < 2.0 / 3.0)
                return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
            else
                return temp1;
        }

        private static int GetOverlappingRegions(IEnumerable<TimingDebugger.Region> regions, TimingDebugger.Region targetRegion)
        {
            return regions.Except(new[] { targetRegion }).Count((region) =>
            {
                return region.Timestamp < targetRegion.EndTime && targetRegion.EndTime < region.EndTime;
            });
        }

        private void ListView_Resize(object sender, EventArgs e)
        {
            ResizeColumns();
        }

        private void ListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var region = debugger.Times[e.ItemIndex];
            var items = new List<string>();

            items.Add(region.Timestamp.ToString());
            items.Add(region.Duration.ToString());

            if (percentageCheckBox.Checked)
                items.Add($"{Math.Round(region.Duration / debugger.StopTime * 100d, 2)}%");

            items.Add(region.Label);

            e.Item = new ListViewItem(items.ToArray()) { ForeColor = GetColor(region, false) };
        }

        private void ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            timelinePanel.Invalidate();
        }

        private void PaintRegions(PaintEventArgs e)
        {
            var canvasWidth = e.ClipRectangle.Width;
            var regionIndex = 0;
            foreach (var region in debugger.Times)
            {
                var relativeTimestamp = (float)region.Timestamp.Ticks / (float)debugger.StopTime.Ticks;
                var relativeDuration = (float)region.Duration.Ticks / (float)debugger.StopTime.Ticks;

                var rectangle = new RectangleF
                {
                    Width = canvasWidth * relativeDuration,
                    X = canvasWidth * relativeTimestamp,
                    Y = 16 * GetOverlappingRegions(debugger.Times, region),
                    Height = 16,
                };

                var isRegionSelected = listView.SelectedIndices.Contains(regionIndex);

                var backgroundColor = isRegionSelected ? SystemBrushes.Highlight : new SolidBrush(GetColor(region, true));
                var textColor = isRegionSelected ? SystemBrushes.HighlightText : SystemBrushes.ControlText;

                e.Graphics.FillRectangle(backgroundColor, rectangle);
                e.Graphics.DrawString(region.Label, Font, textColor, rectangle);

                regionIndex++;
            }
        }

        private void PaintTimeScale(PaintEventArgs e)
        {
            var steps = debugger.StopTime.TotalMilliseconds / StepSize;
            for (int i = 0; i < steps; i++)
            {
                var ms = (i * StepSize);
                var relativeTimestamp = ms / (float)debugger.StopTime.TotalMilliseconds;
                var x = e.ClipRectangle.Width * relativeTimestamp;
                var y = e.ClipRectangle.Height - 16;

                e.Graphics.DrawLine(SystemPens.GrayText, x - 1, 0, x - 1, e.ClipRectangle.Height);
                e.Graphics.DrawString(ms + "ms", Font, SystemBrushes.GrayText, x + 6, y);
            }
        }

        private void PercentageCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            listView.BeginUpdate();
            UpdateColumns();
            ResizeColumns();
            listView.EndUpdate();
        }

        private void ResizeColumns()
        {
            foreach (ColumnHeader column in listView.Columns)
            {
                column.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }

        private void TimelinePanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(timelinePanel.BackColor);

            PaintTimeScale(e);
            PaintRegions(e);
        }

        private void TimelinePanel_Resize(object sender, EventArgs e)
        {
            timelinePanel.Invalidate();
        }

        private void TimingDebuggerForm_Load(object sender, EventArgs e)
        {
            listView.VirtualListSize = debugger.Times.Count;
            listView.VirtualMode = true;

            ResizeColumns();
        }

        private void UpdateColumns()
        {
            listView.Columns.Clear();

            listView.Columns.Add(timestampColumnHeader);
            listView.Columns.Add(durationColumnHeader);

            if (percentageCheckBox.Checked)
            {
                listView.Columns.Add(percentageColumnHeader);
            }

            listView.Columns.Add(labelColumnHeader);
        }
    }
}