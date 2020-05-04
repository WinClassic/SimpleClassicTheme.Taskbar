using NativeWifi;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Linq;

namespace SimpleClassicThemeTaskbar.Forms
{
    public partial class NetworkUI : Form
    {
        private WlanClient client;
        private List<Wlan.WlanConnectionAttributes> currentConnections = new List<Wlan.WlanConnectionAttributes>();
        private List<(WlanClient.WlanInterface, List<Wlan.WlanAvailableNetwork>)> allNetworks = new List<(WlanClient.WlanInterface, List<Wlan.WlanAvailableNetwork>)>();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        private enum ScrollBarDirection
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3
        }

        public NetworkUI()
        {
            InitializeComponent();

            //Create a Wlan Client
            client = new WlanClient();
        }

        private void NetworkUI_Load(object sender, EventArgs e)
        {
            ShowScrollBar(panel1.Handle, (int)ScrollBarDirection.SB_VERT, true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            allNetworks.Clear();
            currentConnections.Clear();

            if (client.Interfaces.Length < 1)
            {
                MessageBox.Show("This menu currently only supports WiFi adapters. The Windows network menu will be shown instead.", "Cannot find a WLAN capable Network Interface");
                Close();
                Process.Start("ms-availablenetworks:");
                return;   
            }
            foreach (WlanClient.WlanInterface wlanInterface in client.Interfaces)
            {
                List<Wlan.WlanAvailableNetwork> networks = new List<Wlan.WlanAvailableNetwork>();
                foreach(Wlan.WlanAvailableNetwork network in wlanInterface.GetAvailableNetworkList(0))
                    networks.Add(network);
                currentConnections.Add(wlanInterface.CurrentConnection);
                networks.Sort((a, b) => b.wlanSignalQuality.CompareTo(a.wlanSignalQuality));
                allNetworks.Add((wlanInterface, networks));
            }
            ShowNetworks(0);
        }

        private void ShowNetworks(int interfaceNo)
        {
            List<NetworkControl> controls = new List<NetworkControl>();

            Control[] ctrls = new Control[panel1.Controls.Count];
            panel1.Controls.CopyTo(ctrls, 0);
            foreach (Control ctrl in ctrls)
            {
                ctrl.Dispose();
            }
            foreach (Wlan.WlanAvailableNetwork nw in allNetworks[interfaceNo].Item2)
            {
                if (nw.profileName == currentConnections[interfaceNo].profileName)
                {
                    NetworkControl nwCtrl = new NetworkControl();
                    nwCtrl.SetNetwork(nw, true);
                    controls.Add(nwCtrl);
                }
                else
                {
                    bool exists = false;
                    foreach (NetworkControl ctrl in controls)
                    {
                        if (Enumerable.SequenceEqual(nw.dot11Ssid.SSID, ctrl.network.dot11Ssid.SSID))
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        NetworkControl nwCtrl = new NetworkControl();
                        nwCtrl.SetNetwork(nw, false);
                        controls.Add(nwCtrl);
                    }
                }
            }
            DoubleBuffered = true;
            int y = 0;
            foreach (NetworkControl ctrl in controls)
            {
                panel1.Controls.Add(ctrl);
                ctrl.Location = new Point(0, y);
                ctrl.Width = panel1.Width - SystemInformation.VerticalScrollBarWidth - 4;
                y += ctrl.Height;
            }
        }
    }
}
