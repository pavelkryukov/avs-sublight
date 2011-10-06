using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace sublight_sv
{
    public partial class MainForm : Form
    {
        public readonly Socket Mysocket;
        public string ChkL = "islok";
        public string ChkR = "isrok";
        public readonly IPEndPoint Sender = new IPEndPoint(IPAddress.Broadcast, 12050);//Слать пакеты будем на 9050

        public MainForm()
        {
            Mysocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Mysocket.Bind(new IPEndPoint(IPAddress.Any, 12051));//Cлушаем приходящие пакеты на 12051
            Mysocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast,true);
            Mysocket.Blocking = false;

            InitializeComponent();
            //playerText.Text = @".\utils\mpc-hc.exe";
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
            Sender.Port = Convert.ToInt32(portText.Text);

            var t = new Thread(() => CheckDialog = new ChkDialog(this));
            t.Start();
            t.Join();

            LcheckBox.Checked = lchkd;
            RcheckBox.Checked = rchkd;
            Application.DoEvents();

        }

        private void StartButtonClick(object sender, EventArgs e)
        {
            var file = new System.IO.StreamWriter(@"test.avs");
            file.WriteLine(@"LoadPlugin("".\as_sublight.dll"")" + "\n" +
            @"return Sublight(DirectShowSource(""" + videoText.Text +
                                             @"""), PORT=" + this.portText.Text + @"IP=""255.255.255.255"")")
            ;
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
