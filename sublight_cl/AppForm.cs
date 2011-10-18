using System;
using System.Windows.Forms;

namespace sublight_cl
{
    internal partial class AppForm : Form
    {
        private static Lamp _lamp;
        
        public AppForm()
        {
            InitializeComponent();
        }

        private void StartButtonClick(object sender, EventArgs e)
        {
            if (_lamp != null) return;
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

            _lamp = new Lamp(port, leftButton.Checked ? Side.Left : Side.Right);

            _lamp.Show();
            _lamp.Start();

            _lamp.Close();
            _lamp.KillSocket();
            _lamp = null;
        }

        private void OnKill(object sender, FormClosedEventArgs e)
        {
            _lamp.IsOn = false;
            _lamp.KillSocket();
            _lamp.Close();
            Application.Exit();
        }
    }
}
