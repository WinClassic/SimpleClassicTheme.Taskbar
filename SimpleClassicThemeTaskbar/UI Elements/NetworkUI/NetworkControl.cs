using NativeWifi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar.UIElements.NetworkUI
{
    public partial class NetworkControl : UserControl
    {
        public Label statusLabel { get { return label2; } }
        public Wlan.WlanAvailableNetwork network;
        private bool isSelected = false;

        public EventHandler ConnectionChange;
        public EventHandler SelectedChanged;
        public bool IsSelected 
        { 
            get { return isSelected; } 
            set 
            { 
                isSelected = value;
                BackColor = isSelected ? SystemColors.Control : SystemColors.ControlLightLight;
                button1.Visible = isSelected;
            } 
        }
        public bool IsConnected;

        public NetworkControl()
        {
            InitializeComponent();
        }

        public void SetNetwork(Wlan.WlanAvailableNetwork nw, bool isConnected)
        { 
            label1.Text = Encoding.UTF8.GetString(nw.dot11Ssid.SSID);
            label2.Text = isConnected ? "Connected" : nw.securityEnabled ? "Secured" : "Public";
            button1.Text = isConnected ? "Disconnect" : "Connect";
            
            IsConnected = isConnected;
            network = nw;
        }

        public void SetAdapter(WlanClient.WlanInterface interFace)
        {
            label1.Text = interFace.NetworkInterface.Id;
        }

        public override string ToString()
        {
            return $"{label1.Text} - {label2.Text} - {Handle}";
        }

        private void NetworkControl_Click(object sender, EventArgs e)
        {
            IsSelected = !IsSelected;
            SelectedChanged?.Invoke(this, new EventArgs());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ConnectionChange?.Invoke(this, e);
        }
    }
}
