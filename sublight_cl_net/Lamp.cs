using System;
using System.Drawing;
using System.Windows.Forms;

namespace sublight_cl_net
{
    internal sealed class Lamp : Form
    {
        private readonly Side _side;
        private readonly UInt16 _port;
        internal Lamp(UInt16 port, Side side)
        {
            SuspendLayout();
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1292, 812);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            Name = "Lamp";
            StartPosition = FormStartPosition.CenterScreen;
            Text = @"Lamp";
            Load += LampLoad;
            ResumeLayout(false);

            _side = side;
            _port = port;
        }
        
        public void Start()
        {
            // ToDo [adikue] implement socket reader
            BackColor = Color.FromArgb(255, 255, 255);
        }

        private void LampLoad(object sender, EventArgs e)
        {
            Top = 0;
            Left = 0;
            Width = 5000;
            Height = 5000;
        }
    }
}
