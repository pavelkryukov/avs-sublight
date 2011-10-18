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
        private readonly PictureBox _closeButton = new PictureBox();

        private readonly PictureBox[] _fields = new PictureBox[4];

        private readonly Side _side;

        private readonly Socket _mysocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private EndPoint _remote;

        public bool IsOn;
        
        private const int Timeout = 100;

        internal Lamp(UInt16 port, Side side)
        {
            _side = side;

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
                                     Top = i * Screen.PrimaryScreen.Bounds.Height/4,
                                     Left = 0,
                                     Width = Screen.PrimaryScreen.Bounds.Width,
                                     Height = Screen.PrimaryScreen.Bounds.Height/4,
                                     TabIndex = i
                                 };
                Controls.Add(_fields[i]);
            }

            Icon = Properties.Resources.Monitor;

            _sideLabel.AutoSize = true;
            _sideLabel.Location = new Point(100, 100);
            _sideLabel.Name = "portLabel";
            _sideLabel.Size = new Size(26, 13);
            _sideLabel.Font = new Font("Microsoft Sans Serif", 36F, FontStyle.Regular);
            _sideLabel.TabIndex = 5;
            _sideLabel.Text = side == Side.Left ? @"Left" : @"Right";
            _sideLabel.BackColor = Color.Transparent;
            Controls.Add(_sideLabel);

            _closeButton.Size = new Size(64, 64);
            _closeButton.Location = new Point(Width - _closeButton.Width, 0);
            _closeButton.Image = Properties.Resources.cross;
            _closeButton.TabIndex = 6;
            _closeButton.BackColor = Color.Transparent;
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

                switch (_side)
                {
                    case Side.Left:
                        if (data[0] == 0x3C)
                        {
                            if ((data[1] == 0xFF) && (data[2] == 0xFF) && (data[3] == 0xFF))
                            {
                                _mysocket.SendTo(System.Text.Encoding.ASCII.GetBytes("liok"), 4, SocketFlags.None,
                                                 _remote);
                            }
                        }
                        else if ((data[0] & 0xFC) == 0xFC)
                        {
                            _fields[data[0] & 0x03].BackColor = Color.FromArgb(data[1], data[2], data[3]);
                        }
                        break;

                    case Side.Right:
                        if (data[0] == 0x30)
                        {
                            if ((data[1] == 0xFF) && (data[2] == 0xFF) && (data[3] == 0xFF))
                            {
                                _mysocket.SendTo(System.Text.Encoding.ASCII.GetBytes("riok"), 4, SocketFlags.None,
                                                 _remote);
                            }
                        }
                        else if ((data[0] & 0xFC) == 0xF0)
                        {
                            _fields[data[0] & 0x03].BackColor = Color.FromArgb(data[1], data[2], data[3]);
                        }
                        break;

                    default:
                        break;
                }

                Application.DoEvents();
            }
        }
    }
}
