using System.Windows.Forms;

namespace sublight_sv
{
    internal class ChkDialog : Form
    {
        private readonly ProgressBar _progressBar;
        private readonly Label _label;
        private readonly MainForm _parent;

        public bool SetLeft
        {
            set { _parent.Lchkd = value; }
        }
        
        public bool SetRight
        {
            set { _parent.Rchkd = value; }
        }

        public void ClearProgressBar()
        {
            _progressBar.Value = 0;
        }

        public void AddProgressBar()
        {
            _progressBar.Value += _progressBar.Maximum/2;
        }

        public void ReDraw()
        {
            Application.DoEvents();
        }

        public ChkDialog(MainForm parent)
        {
            _parent = parent;

            _progressBar = new ProgressBar();
            _label = new Label();
            SuspendLayout();

            // 
            // progressBar
            // 
            _progressBar.Location = new System.Drawing.Point(12, 39);
            _progressBar.Maximum = 10;
            _progressBar.Name = "_progressBar";
            _progressBar.Size = new System.Drawing.Size(218, 23);
            _progressBar.TabIndex = 0;

            // 
            // label
            // 
            _label.AutoSize = true;
            _label.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            _label.Location = new System.Drawing.Point(25, 13);
            _label.Name = "label";
            _label.Size = new System.Drawing.Size(196, 23);
            _label.TabIndex = 1;
            _label.Text = @"Checking connection . . .";
            
            // 
            // ChkDialog
            // 
            ClientSize = new System.Drawing.Size(242, 74);
            Icon = Properties.Resources.MonitorIco;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            Controls.Add(_label);
            Controls.Add(_progressBar);
            Name = "ChkDialog";
            ResumeLayout(false);
            PerformLayout();

            Visible = true;
            _progressBar.Maximum = 100;
        }
    }
}
