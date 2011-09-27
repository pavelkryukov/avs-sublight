using System;
using System.Windows.Forms;

namespace sublight_cl_net
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
            catch (Exception)
            {
                MessageBox.Show(this, @"Invalid port number");
                return;
            }
            _lamp = new Lamp(port, leftButton.Checked ? Side.Left : Side.Right);
            _lamp.Start();
            _lamp.Show();
        }

        private void StopButtonClick(object sender, EventArgs e)
        {
            if (_lamp == null) return;
            _lamp.Close();
            _lamp = null;
        }
    }
}
