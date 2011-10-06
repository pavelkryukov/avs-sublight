﻿using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace sublight_sv
{
    public partial class MainForm : Form
    {
        public bool Lchkd;
        public bool Rchkd;

        private const string ScriptName = "script.avs";

        private ChkDialog _chkDialog;

        public MainForm()
        {
            InitializeComponent();
            playerText.Text = @".\..\..\utils\mpc-hc.exe";
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
            var t = new Thread(() =>
                                   {
                                       _chkDialog = new ChkDialog(this, Convert.ToInt16(portText.Text));
                                       _chkDialog.StartSending();
                                   });
            t.Start();
            t.Join();

            LcheckBox.Checked = Lchkd;
            RcheckBox.Checked = Rchkd;
            if (Lchkd)
                statusLabel.Text = @"Left is OK ";
            if (Rchkd)
                statusLabel.Text = @"Right is OK ";
            if (!Rchkd || !Lchkd)
            {
                statusLabel.Text = @"No clients connected ";
            }
            Application.DoEvents();

        }

        private void StartButtonClick(object sender, EventArgs e)
        {
            if (!Rchkd || !Lchkd)
            {
               var result = MessageBox.Show(@"Connection hasn't been ckecked yet. Continue?", @"Sublight server", 
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
               switch (result)
               {
                   case DialogResult.OK:
                       break;
                   case DialogResult.Cancel:
                       return;
               }
            }
            var file = new System.IO.StreamWriter(ScriptName);
            file.WriteLine(@"LoadPlugin("".\..\bin\Release\as_sublight.dll"")");
            file.WriteLine(String.Format(@"return Sublight(DirectShowSource(""{0}""), PORT={1}, IP=""{2}"")",
                                         videoText.Text,
                                         portText.Text,
                                         @"255.255.255.255"));
            file.Close();

            // Устанавливаем параметры запуска процесса
            var prc = new Process();
            if(playerText.Text.Length!=0)
                prc.StartInfo.FileName = playerText.Text;
            prc.StartInfo.Arguments = ScriptName;
            prc.Start();
            prc.WaitForExit();

            prc.Close();
        }
    }
}
