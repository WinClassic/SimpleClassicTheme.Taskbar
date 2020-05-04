using NativeWifi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar
{
    public partial class NetworkControl : UserControl
    {
        public Wlan.WlanAvailableNetwork network;

        public NetworkControl()
        {
            InitializeComponent();
        }

        public void SetNetwork(Wlan.WlanAvailableNetwork nw, bool isConnected)
        {
            label1.Text = Encoding.UTF8.GetString(nw.dot11Ssid.SSID);
            label2.Text = isConnected ? "Connected" : nw.securityEnabled ? "Secured" : "Public";

            network = nw;
        }
    }
}
