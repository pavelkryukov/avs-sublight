using System;
using System.Drawing;
using System.Windows.Forms;

namespace sublight_cl_net
{
    internal sealed class Lamp : Form
    {
        private readonly Label _sideLabel = new Label();

        private readonly Side _side;
        private readonly UInt16 _port;

        internal Lamp(UInt16 port, Side side)
        {
            _side = side;
            _port = port;

            SuspendLayout();

            _sideLabel.AutoSize = true;
            _sideLabel.Location = new Point(100, 100);
            _sideLabel.Name = "portLabel";
            _sideLabel.Size = new Size(26, 13);
            _sideLabel.Font = new Font("Microsoft Sans Serif", 36F, FontStyle.Regular);
            _sideLabel.TabIndex = 1;
            _sideLabel.Text = side == Side.Left ? @"Left" : @"Right";
            Controls.Add(_sideLabel);

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1292, 812);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            Name = "Lamp";
            StartPosition = FormStartPosition.CenterScreen;
            Text = @"Lamp";

            Top = 0;
            Left = 0;
            Width = Screen.PrimaryScreen.Bounds.Width;
            Height = Screen.PrimaryScreen.Bounds.Height;

            DoubleClick += ((sender, e) => Application.Exit());
            
            ResumeLayout(false);
        }
        
        public void Start()
        {
            // ToDo [adikue] implement socket reader
            BackColor = Color.FromArgb(255, 255, 255);
        }
    }
}
