using System;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;

namespace sublight_sv
{
    public partial class Form1 : Form
    {
        private Socket mysocket;
        private byte[] chkL, chkR;
        private EndPoint Remote;
        private IPEndPoint ipep;
        private IPEndPoint _sender;

        public Form1()
        {
            chkL = new byte[6];
            chkR = new byte[6];
            chkL = Encoding.ASCII.GetBytes("islok");
            chkR = Encoding.ASCII.GetBytes("isrok");

            mysocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            ipep = new IPEndPoint(IPAddress.Any, 9051);
            mysocket.Bind(ipep);

            _sender = new IPEndPoint(IPAddress.Broadcast, 9050);
            Remote = (EndPoint)(_sender);

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.ShowDialog();
            this.textBox1.Text = this.openFileDialog1.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.ShowDialog();
            this.textBox2.Text = this.openFileDialog1.FileName;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_sender.Port != Convert.ToInt32(textBox3.Text))
            {
                _sender.Port = Convert.ToInt32(textBox3.Text);
                Remote = (EndPoint)(_sender);
            }

            mysocket.SendTo(chkL, chkL.Length, SocketFlags.None, Remote);
            while (!checkBox1.Checked)
            {
                var data = new byte[1024];
                int recv = mysocket.ReceiveFrom(data, ref Remote);
                string message = Encoding.ASCII.GetString(data, 0, recv);
                if (message.Equals("lisok"))
                {
                    checkBox1.Checked = true;
                    label4.Text += " Left is OK!";
                }

            }

          


                       
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var file = new System.IO.StreamWriter(@"test.avs");
            file.WriteLine(@"LoadPlugin(""as_sublight.dll"")
return Sublight(DirectShowSource(""" + this.textBox2.Text + @"""), PORT=" + this.textBox3.Text + @")");
            file.Close();

            Process prc = null;

            // Устанавливаем параметры запуска процесса
            prc = new Process();
            if(textBox1.Text.Length!=0)
                prc.StartInfo.FileName = textBox1.Text;
            prc.StartInfo.Arguments = "test.avs";
            prc.Start();
            prc.WaitForExit();

            if (prc != null) prc.Close();

        }

    }
}
