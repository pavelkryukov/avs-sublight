using System.Drawing;
using System.Windows.Forms;

namespace sublight_cl
{
    internal sealed class Lamp4 : Form
    {
        private readonly Label _sideLabel = new Label();
        private readonly PictureBox _closeButton = new PictureBox();

        private readonly PictureBox[] _fields = new PictureBox[4];

        private delegate int GetNum(byte val);
        private readonly GetNum _getNum;

        private static readonly string[] SideNames = {"Left", "Right", "Top", "Bottom"};

        public bool IsOn;

        internal Lamp4(Side side)
        {
            SuspendLayout();

            Top = 0;
            Left = 0;
            Width = Screen.PrimaryScreen.Bounds.Width;
            Height = Screen.PrimaryScreen.Bounds.Height;

            switch (side)
            {
                case Side.Left:
                case Side.Right:
                    _getNum = data => (data >> 4) & 3;
                    break;
                case Side.Top:
                case Side.Bottom:
                    _getNum = data => (data >> 6) & 3;
                    break;
            }

            for (var i = 0; i < 4; i++ )
            {
                switch (side) {
                    case Side.Left:
                    case Side.Right:
                        _fields[i] = new PictureBox
                                 {
                                     Top = i*Screen.PrimaryScreen.Bounds.Height/4,
                                     Left = 0,
                                     Width = Screen.PrimaryScreen.Bounds.Width,
                                     Height = Screen.PrimaryScreen.Bounds.Height/4
                                 };
                        break;
                    case Side.Top:
                    case Side.Bottom:
                        _fields[i] = new PictureBox
                                 {
                                     Top = 0,
                                     Left = i*Screen.PrimaryScreen.Bounds.Width/4,
                                     Width = Screen.PrimaryScreen.Bounds.Width/4,
                                     Height = Screen.PrimaryScreen.Bounds.Height
                                 };
                        break;
                }
                TabIndex = i;
                Controls.Add(_fields[i]);
            }

            Icon = Properties.Resources.Monitor;

            _sideLabel.AutoSize = true;
            _sideLabel.Location = new Point(Screen.PrimaryScreen.Bounds.Height/8, 100);
            _sideLabel.Name = "portLabel";
            _sideLabel.Size = new Size(26, 13);
            _sideLabel.Font = new Font("Microsoft Sans Serif", 36F, FontStyle.Regular);
            _sideLabel.TabIndex = 5;
            _sideLabel.Text = SideNames[(int)side];
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
            _fields[_getNum(data[0])].BackColor = Color.FromArgb(data[1], data[2], data[3]);

            _closeButton.BackColor = _fields[0].BackColor;
            _sideLabel.BackColor = _fields[0].BackColor;
        }

        public void DoEvents()
        {
            Application.DoEvents();
        }
    }
}
