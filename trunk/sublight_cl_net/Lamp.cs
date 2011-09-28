using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace sublight_cl_net
{
    internal sealed class Lamp : Form
    {
        private readonly Label _sideLabel = new Label();
        private readonly PictureBox _pictureBox = new PictureBox();

        private readonly Side _side;
        private readonly UInt16 _port;

        private Socket _mysocket;
        private int _recv;
        private byte[] _data;
        private IPEndPoint _ipep, _sender;
        private EndPoint _Remote;


        internal Lamp(UInt16 port, Side side)
        {
            _side = side;
            _port = port;

            _data = new byte[8]; //данные, которые будут передаваться или приниматься
            _mysocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            _ipep = new IPEndPoint(IPAddress.Any, _port);
            _mysocket.Bind(_ipep); //привязываем точку к нашему сокету
            _sender = new IPEndPoint(IPAddress.Broadcast, 9051);
            _Remote = (EndPoint)(_sender);

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
                _recv = _mysocket.ReceiveFrom(_data, ref _Remote);

                if (_side == Side.Left)
                {
                    BackColor = Color.FromArgb(_data[1], _data[2], _data[3]);
                }

                if (_side == Side.Right)
                {
                    BackColor = Color.FromArgb(_data[5], _data[6], _data[7]);
                }
                Application.DoEvents();
            }
        }
    }
}
