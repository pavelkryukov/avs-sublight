using System;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace sublight_sv
{
    public partial class MainForm : Form
    {
        public readonly Socket _mysocket;
        public string ChkL = "islok";
        public string ChkR = "isrok";
        public readonly IPEndPoint _sender = new IPEndPoint(IPAddress.Broadcast, 12050);//Слать пакеты будем на 9050

        public MainForm()
        {
            _mysocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _mysocket.Bind(new IPEndPoint(IPAddress.Any, 12051));//Cлушаем приходящие пакеты на 12051
            _mysocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast,true);
            _mysocket.Blocking = false;

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
            _sender.Port = Convert.ToInt32(portText.Text);

            Thread t = new Thread(() => CheckDialog = new ChkDialog(this));
            t.Start();
            t.Join();

            LcheckBox.Checked = lchkd;
            RcheckBox.Checked = rchkd;
            Application.DoEvents();

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
