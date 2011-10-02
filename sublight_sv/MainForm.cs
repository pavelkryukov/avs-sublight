using System;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;

namespace sublight_sv
{
    public partial class MainForm : Form
    {
        private readonly Socket _mysocket;
        private const string ChkL = "islok";
/*
        private const string ChkR = "isrok";
*/
        private readonly IPEndPoint _sender = new IPEndPoint(IPAddress.Broadcast, 9050);

        public MainForm()
        {
            _mysocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _mysocket.Bind(new IPEndPoint(IPAddress.Any, 9051));


            InitializeComponent();
        }

        private void ChoosePlayer(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            playerText.Text = openFileDialog.FileName;
        }

        private void ChooseVideo(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            videoText.Text = openFileDialog.FileName;
        }

        private void CheckButtonClick(object sender, EventArgs e)
        {
            _sender.Port = Convert.ToInt32(portText.Text);
            
            _mysocket.SendTo(Encoding.ASCII.GetBytes(ChkL), ChkL.Length, SocketFlags.None, _sender);
            while (!leftRadio.Checked)
            {
                var data = new byte[1024];
                var remote = (EndPoint) _sender;
                var recv = _mysocket.ReceiveFrom(data, ref remote);
                var message = Encoding.ASCII.GetString(data, 0, recv);
                if (!message.Equals("lisok")) continue;
                leftRadio.Checked = true;
                statusLabel.Text += @" Left is OK!";
            }
        }

        private void StartButtonClick(object sender, EventArgs e)
        {
            var file = new System.IO.StreamWriter(@"test.avs");
            file.WriteLine(@"LoadPlugin(""as_sublight.dll"")
return Sublight(DirectShowSource(""" + videoText.Text + @"""), PORT=" + this.portText.Text + @")");
            file.Close();

            // Устанавливаем параметры запуска процесса
            var prc = new Process();
            if(playerText.Text.Length!=0)
                prc.StartInfo.FileName = playerText.Text;
            prc.StartInfo.Arguments = "test.avs";
            prc.Start();
            prc.WaitForExit();

            prc.Close();
        }
    }
}
