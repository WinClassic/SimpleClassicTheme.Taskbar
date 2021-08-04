using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleClassicTheme.Taskbar
{
	public partial class SettingsAddProgramFilter : Form
	{
		public string result = "";

		public SettingsAddProgramFilter()
		{
			InitializeComponent();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			result = textBox1.Text + "|" + comboBox1.SelectedItem;
			Close();
		}

		private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if ((Path.GetInvalidFileNameChars().Contains(e.KeyChar) ||
				Path.GetInvalidPathChars().Contains(e.KeyChar)) && !char.IsControl(e.KeyChar))
			{
				e.Handled = true;
			}
		}
	}
}
