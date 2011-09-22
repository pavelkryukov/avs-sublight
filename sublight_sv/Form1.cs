using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
        private byte[] data;
        private EndPoint Remote;
        private IPEndPoint ipep;
        private IPEndPoint _sender;

        public Form1()
        {
            data = new byte[10];

            mysocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //mysocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            ipep = new IPEndPoint(IPAddress.Loopback, 9051);
            mysocket.Bind(ipep);

            _sender = new IPEndPoint(IPAddress.Loopback, 9050);
            Remote = (EndPoint)(_sender);
            data = Encoding.ASCII.GetBytes("123456789");
            mysocket.SendTo(data, data.Length, SocketFlags.None, Remote);

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

            mysocket.SendTo(data, data.Length, SocketFlags.None, Remote);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var file = new System.IO.StreamWriter(@"test.avs");
            file.WriteLine(@"LoadPlugin(""as_sublight.dll"")
clip = DirectShowSource(""" + this.textBox2.Text + @""")
return Sublight(clip)");
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
