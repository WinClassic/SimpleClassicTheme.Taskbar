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
        public bool Do3DBorder = true;

        public BetterBorderPanel()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Paint += OnPaint;
        }

        public Border3DStyle style = Border3DStyle.Raised;

        private void OnPaint(object sender, PaintEventArgs e)
        {
            if (Do3DBorder)
                ControlPaint.DrawBorder3D(e.Graphics, ClientRectangle, style);
            else
            {
                ButtonBorderStyle bStyle =
                    style == Border3DStyle.Raised ? ButtonBorderStyle.Outset : ButtonBorderStyle.Inset;
                ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                    SystemColors.Control, 1, bStyle,
                    SystemColors.Control, 1, bStyle,
                    SystemColors.Control, 2, bStyle,
                    SystemColors.Control, 2, bStyle);
            }
        }


    }
}
