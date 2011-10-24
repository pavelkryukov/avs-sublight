using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Linq;

namespace sublight_cl
{
    internal sealed class Lamp : Form
    {
        private readonly Label _sideLabel = new Label();
        private readonly PictureBox _closeButton = new PictureBox();

        private readonly byte[] _chk;
        private readonly byte[] _chkAns;
        private readonly byte _mask;

        private readonly PictureBox[] _fields = new PictureBox[4];

        private readonly Side _side;

        private readonly Socket _mysocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private EndPoint _remote;

        public bool IsOn;
        
        private const int Timeout = 100;

        internal Lamp(UInt16 port, Side side)
        {
            _side = side;

            switch (_side)
            {
                case Side.Left:
                    _chk =    new byte[] { 0x00, 0xFF, 0xFF, 0xFF };
                    _chkAns = new byte[] { 0x04, 0xAA, 0xAA, 0xAA };
                    _mask = 0x0C;
                    
                    break;
                case Side.Right:
                    _chk    = new byte[] { 0xC0, 0xFF, 0xFF, 0xFF };
                    _chkAns = new byte[] { 0xC4, 0xAA, 0xAA, 0xAA };
                    _mask = 0xCC;
                    break;
            }

            _mysocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            _mysocket.Bind(new IPEndPoint(IPAddress.Any, port));
            _mysocket.ReceiveTimeout = Timeout;

            IsOn = true;

            _remote = new IPEndPoint(IPAddress.Any,0);

            SuspendLayout();

            Top = 0;
            Left = 0;
            Width = Screen.PrimaryScreen.Bounds.Width;
            Height = Screen.PrimaryScreen.Bounds.Height;

            for (var i = 0; i < 4; i++ )
            {
                _fields[i] = new PictureBox
                                 {
                                     Top = i*Screen.PrimaryScreen.Bounds.Height/4,
                                     Left = 0,
                                     Width = Screen.PrimaryScreen.Bounds.Width,
                                     Height = Screen.PrimaryScreen.Bounds.Height/4,
                                     TabIndex = i
                                 };
                Controls.Add(_fields[i]);
            }

            Icon = Properties.Resources.Monitor;

            _sideLabel.AutoSize = true;
            _sideLabel.Location = new Point(Screen.PrimaryScreen.Bounds.Height/8, 100);
            _sideLabel.Name = "portLabel";
            _sideLabel.Size = new Size(26, 13);
            _sideLabel.Font = new Font("Microsoft Sans Serif", 36F, FontStyle.Regular);
            _sideLabel.TabIndex = 5;
            _sideLabel.Text = side == Side.Left ? @"Left" : @"Right";
            Controls.Add(_sideLabel);

            _closeButton.Size = new Size(64, 64);
            _closeButton.Location = new Point(Width - _closeButton.Width, 0);
            _closeButton.Image = Properties.Resources.cross;
            _closeButton.TabIndex = 6;
            _closeButton.Click += ((sender, e) =>
                                       {
                                           IsOn = false;
                                           Application.Exit();
                                       });
            Controls.Add(_closeButton);

            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            Name = "Lamp";
            StartPosition = FormStartPosition.CenterScreen;
            Text = @"Lamp";
            
            ResumeLayout(false);
        
            _closeButton.BringToFront();
            _sideLabel.BringToFront();
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
            while(true)
            {
                var data = new byte[4];
                try
                {
                    _mysocket.ReceiveFrom(data, 4, SocketFlags.None, ref _remote);
                }
                catch (SocketException)
                {
                    if (!IsOn)
                    {
                        break;
                    }
                    Application.DoEvents();
                    continue;
                }

                if (data.SequenceEqual(_chk))
                {
                    _mysocket.SendTo(_chkAns, 4, SocketFlags.None, _remote);
                }

                else if ((data[0] & 0xCC) == _mask && Crc(data))
                {
                    _fields[(data[0] >> 4) & 0x03].BackColor = Color.FromArgb(data[1], data[2], data[3]);
                }
                
                _closeButton.BackColor = _fields[0].BackColor;
                _sideLabel.BackColor = _fields[0].BackColor;
                Application.DoEvents();
            }
        }

        static byte ControlSumByte(byte source)
        {
            byte sum = 0;
            for (; source > 0; source >>= 1)
            {
                sum += (byte)(source & 1);
            }
            return sum;
        }

        static bool Crc(byte[] check)
        {
            return (check[0] & 0x3) == ((ControlSumByte((byte)(check[0] & ~0x3)) +
                                         ControlSumByte(check[1]) + 
                                         ControlSumByte(check[2]) +
                                         ControlSumByte(check[3])
                                        ) & 3);
        }
    }
}
