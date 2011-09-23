using System;
using System.Drawing;
using System.Windows.Forms;

namespace sublight_cl_net
{
    public sealed class Lamp : Form
    {   
        public Lamp()
        {
            SuspendLayout();
            //
            // Lamp
            // 
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
            MouseClick += LampMouseClick;
            ResumeLayout(false);

        }

        private void LampMouseClick(object sender, MouseEventArgs e)
        {

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
