using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;

namespace sublight_cl
{
    internal sealed class Lamp : Form
    {
        private readonly Label _sideLabel = new Label();
        private readonly PictureBox _pictureBox = new PictureBox();

        private readonly Side _side;
        private readonly UInt16 _port;

        private readonly Socket _mysocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private EndPoint _remote;
        
        private const int Timeout = 10000;

        internal Lamp(UInt16 port, Side side)
        {
            _side = side;
            _port = port;

            _mysocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            _mysocket.Bind(new IPEndPoint(IPAddress.Any, _port)); //Слушать будем 12050
            _mysocket.ReceiveTimeout = Timeout;

            _remote = new IPEndPoint(IPAddress.Any,0);

            SuspendLayout();

            Top = 0;
            Left = 0;
            Width = Screen.PrimaryScreen.Bounds.Width;
            Height = Screen.PrimaryScreen.Bounds.Height;

            _sideLabel.AutoSize = true;
            _sideLabel.Location = new Point(100, 100);
            _sideLabel.Name = "portLabel";
            _sideLabel.Size = new Size(26, 13);
            _sideLabel.Font = new Font("Microsoft Sans Serif", 36F, FontStyle.Regular);
            _sideLabel.TabIndex = 1;
            _sideLabel.Text = side == Side.Left ? @"Left" : @"Right";
            Controls.Add(_sideLabel);

            _pictureBox.Size = new Size(64, 64);
            _pictureBox.Location = new Point(Width - _pictureBox.Width, 0);
            _pictureBox.Image = Properties.Resources.cross;
            _pictureBox.Click += ((sender, e) => Application.Exit());
            Controls.Add(_pictureBox);

            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            Name = "Lamp";
            StartPosition = FormStartPosition.CenterScreen;
            Text = @"Lamp";
            
            ResumeLayout(false);
        }

        public void KillSocket()
        {
            _mysocket.Close();
        }

        ~Lamp()
        {
            KillSocket();
        }

        public void Start()
        {
            uint r, g, b;
            switch (_side)
            {
                case Side.Left:
                    r = 5;
                    g = 6;
                    b = 7;
                    break;
                case Side.Right:
                    r = 1;
                    g = 2;
                    b = 3;
                    break;
                default:
                    throw new Exception("Invalid side has been selected.");
            }
            while(true)
            {
                var data = new byte[8];
                try
                {
                    _mysocket.ReceiveFrom(data, 8, SocketFlags.None, ref _remote);
                }
                catch (SocketException)
                {
                    var result = MessageBox.Show(@"Error while reading data from socket", @"Sublight receiver", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    switch (result)
                    {
                        case DialogResult.Retry:
                            continue;
                        case DialogResult.Cancel:
                            return;
                    }
                }

                if ((data[0] == 0xA7) && (data[4] == 0xEB))
                {
                    BackColor = Color.FromArgb(data[r], data[g], data[b]);
                }
                else if (System.Text.Encoding.ASCII.GetString(data).Equals("islok\0\0\0") && (_side == Side.Left))
                {
                    _mysocket.SendTo(System.Text.Encoding.ASCII.GetBytes("lisok"), 5, SocketFlags.None, _remote);
                }
                else if (System.Text.Encoding.ASCII.GetString(data).Equals("isrok\0\0\0") && (_side == Side.Right))
                {
                    _mysocket.SendTo(System.Text.Encoding.ASCII.GetBytes("risok"), 5, SocketFlags.None, _remote);
                }
                
                Application.DoEvents();
            }
        }
    }
}
