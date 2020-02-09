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
    public partial class VerticalDivider : UserControl
    {
        public VerticalDivider()
        {
            InitializeComponent();
        }

        private void VerticalDivider_Load(object sender, EventArgs e)
        {
            betterBorderPanel2.style = Border3DStyle.RaisedInner;
            betterBorderPanel2.Invalidate();
        }
    }
}
