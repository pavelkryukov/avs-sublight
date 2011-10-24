using System;
using System.Windows.Forms;

namespace sublight_cl
{
    internal partial class AppForm : Form
    {
        private static UdpReceiver _udpReceiver;
        
        public AppForm()
        {
            InitializeComponent();
        }

        private void StartButtonClick(object sender, EventArgs e)
        {
            if (_udpReceiver != null) return;
            UInt16 port;
            try
            {
                port = UInt16.Parse(portBox.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show(this, @"Please enter number.");
                return;
            }
            catch (OverflowException)
            {
                MessageBox.Show(this,
                                @"Invalid port number. It should be from" + UInt16.MinValue + @" to " + UInt16.MaxValue +
                                @".");
                return;
            }

            _udpReceiver = new UdpReceiver(port, leftButton.Checked ? Side.Left : Side.Right);
            _udpReceiver.Start();

            _udpReceiver.Lamp.Close();
            _udpReceiver.KillSocket();
            _udpReceiver = null;
        }

        private void OnKill(object sender, FormClosedEventArgs e)
        {
            if (_udpReceiver == null) return;
            _udpReceiver.Lamp.IsOn = false;
            _udpReceiver.KillSocket();
            _udpReceiver.Lamp.Close();
            Application.Exit();
        }
    }
}
