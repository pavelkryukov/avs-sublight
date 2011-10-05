using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;

namespace sublight_cl_net
{
    internal sealed class Lamp : Form
    {
        private readonly Label _sideLabel = new Label();
        private readonly PictureBox _pictureBox = new PictureBox();

        private readonly Side _side;
        private readonly UInt16 _port;

        private readonly Socket _mysocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private readonly byte[] _data = new byte[1024];
        private EndPoint _remote;

        internal Lamp(UInt16 port, Side side)
        {
            _side = side;
            _port = port;

            _mysocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            _mysocket.Bind(new IPEndPoint(IPAddress.Any, _port)); //Слушать будем 12050
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
        
        public void Start()
        {
            while(true)
            {
                try
                {
                    _mysocket.ReceiveFrom(_data, ref _remote);
                    //_mysocket.Receive(_data);
                }
                catch(Exception)
                {
                    return;
                }

                switch (_side)
                {
                    case Side.Left:
                        BackColor = Color.FromArgb(_data[5], _data[6], _data[7]);                        
                        break;

                    case Side.Right:
                        BackColor = Color.FromArgb(_data[1], _data[2], _data[3]);
                        break;
                }
                Application.DoEvents();
            };
        }
    }
}
