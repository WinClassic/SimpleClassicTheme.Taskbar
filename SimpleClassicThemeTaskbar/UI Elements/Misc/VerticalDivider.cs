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
        private bool wide = true;

        [Description("Tells the control to use 1 or 2 (wide) dividers when drawing")]
        [Category("Appearance")]
        public bool Wide
        {
            get => wide;
            set
            {
                wide = value;
                Width = wide ? 7 : 2;
            }
        }

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
