using System;
using System.Windows.Forms;
using System.IO;

namespace sublight_sv
{
    internal partial class MainForm : Form
    {
        public bool Lchkd
        {
            set { LcheckBox.Checked = value; }
            private get { return LcheckBox.Checked; }
        }

        public bool Rchkd
        {
            set { RcheckBox.Checked = value; }
            private get { return RcheckBox.Checked; }
        }

        public MainForm()
        {
            InitializeComponent();
            playerText.Text = RegistryReader.GetName();
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
            var t = new System.Threading.Thread(() => new CheckUdp(this, Convert.ToUInt16(portText.Text)).StartSending());
            t.Start();
            t.Join();

            if (Lchkd)
                statusLabel.Text = @"Left is OK ";
            if (Rchkd)
                statusLabel.Text = @"Right is OK ";
            if (!Rchkd && !Lchkd)
                statusLabel.Text = @"No clients connected ";
            if (Rchkd && Lchkd)
                statusLabel.Text = @"Both clients connected";
            Application.DoEvents();
        }

        private void StartButtonClick(object sender, EventArgs e)
        {
            if (!Rchkd || !Lchkd)
            {
               var result = MessageBox.Show(@"Connection hasn't been checked yet. Continue?", @"Sublight server", 
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
               switch (result)
               {
                   case DialogResult.OK:
                       break;
                   case DialogResult.Cancel:
                       return;
               }
            }
            try
            {
                AvsLauncher.Launch(Path.GetDirectoryName(Application.ExecutablePath), 
                                   playerText.Text, 
                                   videoText.Text,
                                   portText.Text);
            }
            catch (AvsLauncherException exc)
            {
                MessageBox.Show(exc.ToString(), @"Sublight server", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }
    }
}
