using System.Text;
using System.Net;
using System.Windows.Forms;
using System.Net.Sockets;

namespace sublight_sv
{
    public class ChkDialog : Form
    {
        private readonly Socket _mysocket;
        private const string ChkL = "islok";
        private const string ChkR = "isrok";
        private readonly IPEndPoint _sender;

        private readonly ProgressBar _progressBar;
        private readonly Label _label;
        private readonly Timer _timer;
        private readonly MainForm _parent;

        private int _i;

        public ChkDialog(MainForm parent, System.Int16 port)
        {
            _parent = parent;

            _mysocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _mysocket.Bind(new IPEndPoint(IPAddress.Any, 12051));//Cлушаем приходящие пакеты на 12051
            _mysocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            _mysocket.Blocking = false;

            _sender = new IPEndPoint(IPAddress.Broadcast, port);

    //        components = new System.ComponentModel.Container();
            _progressBar = new ProgressBar();
            _timer = new Timer();
            _label = new Label();
            SuspendLayout();
            // 
            // progressBar
            // 
            _progressBar.Location = new System.Drawing.Point(12, 39);
            _progressBar.Maximum = 10;
            _progressBar.Name = "_progressBar1";
            _progressBar.Size = new System.Drawing.Size(218, 23);
            _progressBar.TabIndex = 0;
            // 
            // timer
            // 
            _timer.Enabled = true;
            _timer.Interval = 100;
            _timer.Tick += ((sender, e) => _i++);
            // 
            // label
            // 
            _label.AutoSize = true;
            _label.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            _label.Location = new System.Drawing.Point(25, 13);
            _label.Name = "label";
            _label.Size = new System.Drawing.Size(196, 23);
            _label.TabIndex = 1;
            _label.Text = @"Checking connection . . .";
            // 
            // ChkDialog
            // 
            ClientSize = new System.Drawing.Size(242, 74);
            Controls.Add(_label);
            Controls.Add(_progressBar);
            Name = "ChkDialog";
            ResumeLayout(false);
            PerformLayout();

            Visible = true;
            _progressBar.Maximum = 100;
            StartSending();
        }

        private void StartTimer()
        {
           _timer.Start();
           _i = 0;
        }

        public void StartSending()
        {
            var rdata = new byte[5];
            var recv = 0;
            const int steps = 2;

            _parent.Lchkd = false;
            _parent.Rchkd = false;

            for (var a = 1; a <= steps; a++)
            {
                StartTimer();
                Application.DoEvents();

                var sLdata = Encoding.ASCII.GetBytes(ChkL);

                _mysocket.SendTo(sLdata, sLdata.Length, SocketFlags.None, _sender);
                var remote = (EndPoint)_sender;
                while (_i <= 3)//Wait for ansver 3 timer ticks
                {
                    Application.DoEvents();
                    recv = _mysocket.Available;
                    if (recv > 0)
                    {
                        recv = _mysocket.ReceiveFrom(rdata, ref remote);
                        break;
                    }
                }

                if (Encoding.ASCII.GetString(rdata, 0, recv).Equals("lisok"))
                {
                    _parent.Lchkd = true;
                    _progressBar.Value = _progressBar.Maximum / 2;
                    Application.DoEvents();
                    break;
                }

                _progressBar.Value = a * _progressBar.Maximum / (2 * steps);
                Application.DoEvents();
                _timer.Stop();
            }

            for (var a = 1; a <= steps; a++)//Wait for ansver 3 timer ticks
            {
                StartTimer();
                Application.DoEvents();

                var sRdata = Encoding.ASCII.GetBytes(ChkR);

                _mysocket.SendTo(sRdata, sRdata.Length, SocketFlags.None, _sender);
                var remote = (EndPoint)_sender;
                while (_i <= 3)//Wait for ansver 2 timer ticks
                {
                    Application.DoEvents();
                    recv = _mysocket.Available;
                    if (recv > 0)
                    {
                        recv = _mysocket.ReceiveFrom(rdata, rdata.Length, SocketFlags.None, ref remote);
                        break;
                    }
                }

                if (Encoding.ASCII.GetString(rdata, 0, recv).Equals("risok"))
                {
                    _parent.Rchkd = true;
                    _progressBar.Value = _progressBar.Maximum;
                    Application.DoEvents();
                    break;
                }     
           
                _progressBar.Value = a * _progressBar.Maximum/steps;
                Application.DoEvents();
                _timer.Stop();
            }
            _progressBar.Value = _progressBar.Maximum;

            _mysocket.Close();
            Application.DoEvents();
            Close();
        }
    }
}
