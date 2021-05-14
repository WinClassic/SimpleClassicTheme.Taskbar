using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar
{
	public abstract class UserControlEx : UserControl
	{
		public bool Erroring = false;
		public abstract string GetErrorString();

		protected void DrawError(Graphics g)
		{
			g.Clear(SystemColors.ControlLightLight);
			g.DrawLine(Pens.Red, 0, 0, Width - 1, Height - 1);
			g.DrawLine(Pens.Red, Width - 1, 0, 0, Height - 1);
		}

		protected string controlState;
		protected string GetBaseErrorString() =>
            $"An unexpected error occured while {controlState}\r\n" +
            $"\r\n" +
            $"Source: {GetType()}\r\n";
	}
}
