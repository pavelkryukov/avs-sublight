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
        public Form1()
        {
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
            var data = new byte[3];

            Socket mysocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ipep = new IPEndPoint(IPAddress.Loopback, Convert.ToInt32(textBox3.Text));
            mysocket.Bind(ipep);

            IPEndPoint _sender = new IPEndPoint(IPAddress.Loopback, Convert.ToInt32(textBox3.Text));
            EndPoint Remote = (EndPoint)(_sender);
            data = Encoding.ASCII.GetBytes("L?");

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
