using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleClassicTheme.Taskbar.Forms
{
    public partial class NetworkPasswordPrompt : Form
    {
        private bool canClose = false;

        public string Username { get { return textBox1.Text; } }
        public string Password { get { return textBox2.Text; } }

        public NetworkPasswordPrompt()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            canClose = true;
            Close();   
        }

        public void ShowDialog(string ssid)
        {
            label1.Text = label1.Text.Replace("SSID", ssid);
            textBox1.Text = "WPA2 - Private";
            textBox1.Enabled = false;
            _ = ShowDialog();
        }
        private void NetworkPasswordPrompt_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !canClose;
        }
    }
}
