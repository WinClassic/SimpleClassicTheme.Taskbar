using NativeWifi;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Linq;

using SimpleClassicThemeTaskbar.UIElements.NetworkUI;
using System.Threading;
using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

namespace SimpleClassicThemeTaskbar.Forms
{
    public partial class NetworkUI : Form
    {
        private readonly List<(WlanClient.WlanInterface, List<Wlan.WlanAvailableNetwork>)> allNetworks = new();
        private readonly WlanClient client;
        private readonly List<Wlan.WlanConnectionAttributes> currentConnections = new();
        private readonly int selectedInterface = 0;

        public NetworkUI()
        {
#if DEBUG
#else
            MessageBox.Show("NOTE: This part of the SCTT UI is unfinished.", "'Dummy' UI");
#endif
            InitializeComponent();

            //Create a Wlan Client
            client = new WlanClient();
        }

        private enum ScrollBarDirection
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3
        }

        public void ConnectOrDisconnect(object sender, EventArgs e)
        {
            NetworkControl origin = sender as NetworkControl;
            WlanClient.WlanInterface interFace = allNetworks[selectedInterface].Item1;
            //if (origin.IsConnected)
            //    interFace.Disconnect();

            string ssid = System.Text.Encoding.UTF8.GetString(origin.network.dot11Ssid.SSID).Replace("\0", "");
            bool connectionInitiated = false;

            foreach (Wlan.WlanProfileInfo profileInfo in interFace.GetProfiles())
            {
                if (profileInfo.profileName == "Ziggo1330640")
                    Clipboard.SetText(interFace.GetProfileXml(profileInfo.profileName));
                if (profileInfo.profileName == ssid)
                {
                    interFace.Connect(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, profileInfo.profileName);
                    origin.statusLabel.Text = "Connecting";
                    Application.DoEvents(); Application.DoEvents(); Application.DoEvents();
                    connectionInitiated = true;
                }
            }
            if (!connectionInitiated)
            {
                if (!origin.network.securityEnabled)
                    interFace.Connect(Wlan.WlanConnectionMode.Auto, Wlan.Dot11BssType.Any, origin.network.dot11Ssid, Wlan.WlanConnectionFlags.IgnorePrivacyBit);
                else
                {
                    NetworkPasswordPrompt prompt = new();
                    prompt.ShowDialog(ssid);

                    origin.statusLabel.Text = "Connecting";

                    Application.DoEvents(); Application.DoEvents(); Application.DoEvents();
                    string profileName = ssid;
                    byte[] ssidBytes = System.Text.Encoding.Default.GetBytes(ssid);
                    string ssidHex = BitConverter.ToString(ssidBytes);
                    string mac = ssidHex.Replace("-", "");
                    string key = prompt.Password;
                    string profileXml = string.Format("<?xml version=\"1.0\"?><WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>{0}</name><SSIDConfig><SSID><hex>{1}</hex><name>{0}</name></SSID></SSIDConfig><connectionType>ESS</connectionType><connectionMode>auto</connectionMode><MSM><security><authEncryption><authentication>WPA2PSK</authentication><encryption>AES</encryption><useOneX>false</useOneX></authEncryption><sharedKey><keyType>passPhrase</keyType><protected>false</protected><keyMaterial>{2}</keyMaterial></sharedKey></security></MSM></WLANProfile>", profileName, mac, key);
                    _ = interFace.SetProfile(Wlan.WlanProfileFlags.AllUser, profileXml, true);
                    interFace.Connect(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, profileName);
                    bool returned = false;
                    interFace.WlanConnectionNotification += delegate (Wlan.WlanNotificationData data, Wlan.WlanConnectionNotificationData connData)
                    {
                        if (!returned && connData.profileName == profileName)
                        {
                            if ((Wlan.WlanNotificationCodeAcm)data.NotificationCode == Wlan.WlanNotificationCodeAcm.ConnectionAttemptFail)
                            {
                                _ = MessageBox.Show("Connection failed");
                                returned = true;
                            }
                        }
                    };
                    while (!returned) ;
                }
            }
        }

        public void SelectionChanged(object sender, EventArgs e)
        {
            NetworkControl origin = sender as NetworkControl;
            foreach (NetworkControl control in panel1.Controls)
                if (control.Handle != origin.Handle)
                    control.IsSelected = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            allNetworks.Clear();
            currentConnections.Clear();

            foreach (WlanClient.WlanInterface wlanInterface in client.Interfaces)
            {
                while (wlanInterface.InterfaceState == Wlan.WlanInterfaceState.NotReady)
                    continue;
                bool ctnu = false;
                foreach (Wlan.WlanPhyRadioState state in wlanInterface.RadioState.PhyRadioState)
                {
                    if (state.dot11HardwareRadioState == Wlan.Dot11RadioState.Off)
                    {
                        ctnu = true;
                        continue;
                    }
                    if (state.dot11SoftwareRadioState == Wlan.Dot11RadioState.Off)
                    {
                        ctnu = true;
                        checkBox1.CheckedChanged -= checkBox1_CheckedChanged;
                        checkBox1.Checked = false;
                        checkBox1.CheckedChanged += checkBox1_CheckedChanged;
                        continue;
                    }
                }

                List<Wlan.WlanAvailableNetwork> networks = new();
                if (!ctnu)
                {
                    Wlan.WlanConnectionAttributes attrib;
                    if (wlanInterface.InterfaceState == Wlan.WlanInterfaceState.Connected)
                        attrib = wlanInterface.CurrentConnection;
                    else
                        attrib = new Wlan.WlanConnectionAttributes() { profileName = "___DUMMYOBJECT___" };
                    currentConnections.Add(attrib);
                    foreach (Wlan.WlanAvailableNetwork network in wlanInterface.GetAvailableNetworkList(0))
                        if (System.Text.Encoding.UTF8.GetString(network.dot11Ssid.SSID).Replace("\0", "") != attrib.profileName)
                            networks.Add(network);
                    networks.Sort((a, b) => b.wlanSignalQuality.CompareTo(a.wlanSignalQuality));
                    if (attrib.profileName != "___DUMMYOBJECT___")
                    {
                        Wlan.WlanAvailableNetwork aval = new();
                        aval.profileName = attrib.profileName;
                        byte[] bytes = new byte[32];
                        for (int i = 0; i < 32; i++)
                            bytes[i] = 0x00;
                        System.Text.Encoding.UTF8.GetBytes(aval.profileName).CopyTo(bytes, 0);
                        aval.dot11Ssid.SSID = bytes;
                        networks.Insert(0, aval);
                    }
                }
                else
                {
                    currentConnections.Add(new Wlan.WlanConnectionAttributes() { });
                }
                allNetworks.Add((wlanInterface, networks));
            }
            ShowNetworks(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            foreach (WlanClient.WlanInterface interFace in client.Interfaces)
            {
                IntPtr radioStatePtr = new(0L);
                try
                {
                    Wlan.WlanPhyRadioState radioState = new();
                    radioState.dwPhyIndex = 0;
                    radioState.dot11HardwareRadioState = Wlan.Dot11RadioState.On;
                    radioState.dot11SoftwareRadioState = checkBox1.Checked ? Wlan.Dot11RadioState.On : Wlan.Dot11RadioState.Off;

                    radioStatePtr = Marshal.AllocHGlobal(Marshal.SizeOf(radioState));
                    Marshal.StructureToPtr(radioState, radioStatePtr, false);

                    _ = Wlan.WlanSetInterface(
                                client.clientHandle,
                                interFace.InterfaceGuid,
                                Wlan.WlanIntfOpcode.RadioState,
                                (uint)Marshal.SizeOf(typeof(Wlan.WlanPhyRadioState)),
                                radioStatePtr,
                                IntPtr.Zero);
                }
                finally
                {
                    if (radioStatePtr.ToInt64() != 0)
                        Marshal.FreeHGlobal(radioStatePtr);
                }
            }
        }

        private void NetworkUI_Load(object sender, EventArgs e)
        {
            _ = User32.ShowScrollBar(panel1.Handle, (int)ScrollBarDirection.SB_VERT, true);
        }

        private void ShowNetworks(int interfaceNo)
        {
            List<NetworkControl> controls = new();
            foreach (Control c in panel1.Controls)
                if (c is NetworkControl nc)
                    controls.Add(nc);

            if (allNetworks.Count <= 0)
            {
                return;
            }

            //Check if the adapter is disabled
            bool disabled = false;
            foreach (Wlan.WlanPhyRadioState state in allNetworks[interfaceNo].Item1.RadioState.PhyRadioState)
            {
                if (state.dot11SoftwareRadioState == Wlan.Dot11RadioState.Off)
                {
                    disabled = true;
                    continue;
                }
            }
            if (disabled)
            {
                //TODO: Add a Network control that says that the adapter is disabled
            }

            List<NetworkControl> newControls = new();
            foreach (Wlan.WlanAvailableNetwork nw in allNetworks[interfaceNo].Item2)
            {
                bool exists = false;
                NetworkControl existingControl = null;
                foreach (NetworkControl ctrl in controls)
                {
                    if (Enumerable.SequenceEqual(nw.dot11Ssid.SSID, ctrl.network.dot11Ssid.SSID))
                    {
                        existingControl = ctrl;
                        exists = true;
                        break;
                    }
                }
                foreach (NetworkControl ctrl in newControls)
                {
                    if (Enumerable.SequenceEqual(nw.dot11Ssid.SSID, ctrl.network.dot11Ssid.SSID))
                    {
                        existingControl = ctrl;
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    bool isCon = nw.profileName == currentConnections[interfaceNo].profileName;
                    NetworkControl nwCtrl = new();
                    nwCtrl.SelectedChanged += SelectionChanged;
                    nwCtrl.ConnectionChange += ConnectOrDisconnect;
                    nwCtrl.SetNetwork(nw, isCon);
                    newControls.Add(nwCtrl);
                    if (isCon)
                    {
                        _ = newControls.Remove(nwCtrl);
                        newControls.Insert(0, nwCtrl);
                    }
                }
                else
                {
                    bool isCon = nw.profileName == currentConnections[interfaceNo].profileName;
                    existingControl.SetNetwork(nw, isCon);
                    if (!newControls.Contains(existingControl))
                        newControls.Add(existingControl);
                    if (isCon)
                    {
                        _ = newControls.Remove(existingControl);
                        newControls.Insert(0, existingControl);
                    }
                }
            }

            Control[] panelControls = new Control[panel1.Controls.Count];
            panel1.Controls.CopyTo(panelControls, 0);
            foreach (Control ctrl in panelControls)
            {
                bool exists = false;
                foreach (NetworkControl c in newControls)
                    if (ctrl.Handle == c.Handle)
                        exists = true;
                if (!exists)
                    ctrl.Dispose();
            }

            int y = 0;
            foreach (NetworkControl ctrl in newControls)
            {
                int newY = y - panel1.VerticalScroll.Value;
                ctrl.Location = new Point(0, newY);
                ctrl.Width = panel1.Width - SystemInformation.VerticalScrollBarWidth - 4;
                y += ctrl.Height;
                if (ctrl.Parent != panel1)
                    panel1.Controls.Add(ctrl);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            button1_Click(this, new EventArgs());
        }
    }
}