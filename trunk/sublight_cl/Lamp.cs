using System.Drawing;
using System.Windows.Forms;

namespace sublight_cl
{
    internal sealed class Lamp : Form
    {
        private readonly Label _sideLabel = new Label();
        private readonly PictureBox _closeButton = new PictureBox();

        private readonly PictureBox[] _fields = new PictureBox[4];

        private readonly Side _side;

        public bool IsOn;

        internal Lamp(Side side)
        {
            _side = side;

            SuspendLayout();

            Top = 0;
            Left = 0;
            Width = Screen.PrimaryScreen.Bounds.Width;
            Height = Screen.PrimaryScreen.Bounds.Height;

            for (var i = 0; i < 4; i++ )
            {
                _fields[i] = new PictureBox
                                 {
                                     Top = i*Screen.PrimaryScreen.Bounds.Height/4,
                                     Left = 0,
                                     Width = Screen.PrimaryScreen.Bounds.Width,
                                     Height = Screen.PrimaryScreen.Bounds.Height/4,
                                     TabIndex = i
                                 };
                Controls.Add(_fields[i]);
            }

            Icon = Properties.Resources.Monitor;

            _sideLabel.AutoSize = true;
            _sideLabel.Location = new Point(Screen.PrimaryScreen.Bounds.Height/8, 100);
            _sideLabel.Name = "portLabel";
            _sideLabel.Size = new Size(26, 13);
            _sideLabel.Font = new Font("Microsoft Sans Serif", 36F, FontStyle.Regular);
            _sideLabel.TabIndex = 5;
            _sideLabel.Text = side == Side.Left ? @"Left" : @"Right";
            Controls.Add(_sideLabel);

            _closeButton.Size = new Size(64, 64);
            _closeButton.Location = new Point(Width - _closeButton.Width, 0);
            _closeButton.Image = Properties.Resources.cross;
            _closeButton.TabIndex = 6;
            _closeButton.Click += ((sender, e) =>
                                       {
                                           IsOn = false;
                                           Application.Exit();
                                       });
            Controls.Add(_closeButton);

            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            Name = "Lamp";
            StartPosition = FormStartPosition.CenterScreen;
            Text = @"Lamp";
            
            ResumeLayout(false);
        
            _closeButton.BringToFront();
            _sideLabel.BringToFront();
        }

        public void SetColor(byte[] data)
        {
            _fields[(data[0] >> 4) & 0x03].BackColor = Color.FromArgb(data[1], data[2], data[3]);

            _closeButton.BackColor = _fields[0].BackColor;
            _sideLabel.BackColor = _fields[0].BackColor;
            Application.DoEvents();
        }

        public void DoEvents()
        {
            Application.DoEvents();
        }
    }
}
