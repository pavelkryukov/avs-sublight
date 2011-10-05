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
        private readonly IPEndPoint _sender = new IPEndPoint(IPAddress.Loopback, 12050);//Слать пакеты будем на 9050

        public MainForm()
        {
            _mysocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _mysocket.Bind(new IPEndPoint(IPAddress.Loopback, 9051));//Cлушаем приходящие пакеты на 9051
            _mysocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast,true);


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

            var data = new byte[1024];
            for (uint i = 0; i < 1024; i++)
            {
                data[i] = 0xA;
            }

            _mysocket.SendTo(data, data.Length, SocketFlags.None, _sender);

            /*while (!leftRadio.Checked)
            {
                var data = new byte[1024];
                for (uint i = 0; i < 1024; i++)
                {
                    data[i] = 0xA;
                }
                var remote = (EndPoint) _sender;
                var recv = _mysocket.ReceiveFrom(data, ref remote);
                var message = Encoding.ASCII.GetString(data, 0, recv);
                if (!message.Equals("lisok")) continue;
                leftRadio.Checked = true;
                statusLabel.Text += @" Left is OK!";
            }
             */
        }

        private void StartButtonClick(object sender, EventArgs e)
        {
            var file = new System.IO.StreamWriter(@"test.avs");
            file.WriteLine(@"LoadPlugin(""c:\Users\Ado1ff\Documents\Visual Studio 2010\Projects\sublight\bin\Debug\as_sublight.dll"")
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
