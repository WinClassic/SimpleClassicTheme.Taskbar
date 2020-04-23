using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar
{
    public partial class SystemTrayView : Form
    {
        public SystemTrayView()
        {
            InitializeComponent();
        }

        SystemTrayIcon tray;

        public void ParseData(List<SystemTrayIcon> icons)
        {
            dataGridView1.Rows.Clear();
            if (icons.Count <= 0)
                return;

            List<(Image, string, string, string)> temp = new List<(Image, string, string, string)>();
            icons[0].Invoke(new Action(() =>
            {
                tray = icons[0];
                foreach (SystemTrayIcon d in icons)
                {
                    temp.Add((d.Image, d.BaseHandle.ToString(), d.Handle.ToString(), d.Text));
                }
            }));
            foreach ((Image, string, string, string) row in temp)
            {
                dataGridView1.Rows.Add(row.Item1, row.Item2, row.Item3, row.Item4);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            ((SystemTray)tray.Parent.Parent).firstTime = true;
            Close();
        }
    }
}
