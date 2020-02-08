using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar
{
    public partial class BetterBorderPanel : Panel
    {
        public BetterBorderPanel()
        {
            InitializeComponent();
            Paint += OnPaint;
        }

        public Border3DStyle style = Border3DStyle.Raised;

        private void OnPaint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder3D(e.Graphics, ClientRectangle, style);
        }
    }
}
