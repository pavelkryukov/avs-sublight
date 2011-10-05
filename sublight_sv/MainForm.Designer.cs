using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
namespace sublight_sv
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.selectPlayerButton = new System.Windows.Forms.Button();
            this.playerText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.videoText = new System.Windows.Forms.TextBox();
            this.portText = new System.Windows.Forms.TextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.openVideoButton = new System.Windows.Forms.Button();
            this.checkButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.startButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.RcheckBox = new System.Windows.Forms.CheckBox();
            this.LcheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // selectPlayerButton
            // 
            this.selectPlayerButton.Location = new System.Drawing.Point(375, 4);
            this.selectPlayerButton.Name = "selectPlayerButton";
            this.selectPlayerButton.Size = new System.Drawing.Size(64, 23);
            this.selectPlayerButton.TabIndex = 0;
            this.selectPlayerButton.Text = "Open...";
            this.selectPlayerButton.UseVisualStyleBackColor = true;
            this.selectPlayerButton.Click += new System.EventHandler(this.ChoosePlayer);
            // 
            // playerText
            // 
            this.playerText.Location = new System.Drawing.Point(73, 6);
            this.playerText.Name = "playerText";
            this.playerText.Size = new System.Drawing.Size(296, 20);
            this.playerText.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Player:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Video File:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Send port:";
            // 
            // videoText
            // 
            this.videoText.Location = new System.Drawing.Point(73, 34);
            this.videoText.Name = "videoText";
            this.videoText.Size = new System.Drawing.Size(296, 20);
            this.videoText.TabIndex = 5;
            // 
            // portText
            // 
            this.portText.Location = new System.Drawing.Point(73, 60);
            this.portText.Name = "portText";
            this.portText.Size = new System.Drawing.Size(100, 20);
            this.portText.TabIndex = 6;
            this.portText.Text = "12050";
            this.portText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "*.exe";
            // 
            // openVideoButton
            // 
            this.openVideoButton.Location = new System.Drawing.Point(375, 32);
            this.openVideoButton.Name = "openVideoButton";
            this.openVideoButton.Size = new System.Drawing.Size(64, 23);
            this.openVideoButton.TabIndex = 7;
            this.openVideoButton.Text = "Open...";
            this.openVideoButton.UseVisualStyleBackColor = true;
            this.openVideoButton.Click += new System.EventHandler(this.ChooseVideo);
            // 
            // checkButton
            // 
            this.checkButton.Location = new System.Drawing.Point(179, 58);
            this.checkButton.Name = "checkButton";
            this.checkButton.Size = new System.Drawing.Size(75, 23);
            this.checkButton.TabIndex = 8;
            this.checkButton.Text = "Check";
            this.checkButton.UseVisualStyleBackColor = true;
            this.checkButton.Click += new System.EventHandler(this.CheckButtonClick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::sublight_sv.Properties.Resources.monitor;
            this.pictureBox1.Location = new System.Drawing.Point(73, 87);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 100);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(375, 163);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(64, 23);
            this.startButton.TabIndex = 10;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.StartButtonClick);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(260, 63);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(46, 13);
            this.statusLabel.TabIndex = 11;
            this.statusLabel.Text = "Status...";
            // 
            // RcheckBox
            // 
            this.RcheckBox.AutoSize = true;
            this.RcheckBox.Enabled = false;
            this.RcheckBox.Location = new System.Drawing.Point(179, 88);
            this.RcheckBox.Name = "RcheckBox";
            this.RcheckBox.Size = new System.Drawing.Size(34, 17);
            this.RcheckBox.TabIndex = 14;
            this.RcheckBox.Text = "R";
            this.RcheckBox.UseVisualStyleBackColor = true;
            // 
            // LcheckBox
            // 
            this.LcheckBox.AutoSize = true;
            this.LcheckBox.Enabled = false;
            this.LcheckBox.Location = new System.Drawing.Point(35, 87);
            this.LcheckBox.Name = "LcheckBox";
            this.LcheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.LcheckBox.Size = new System.Drawing.Size(32, 17);
            this.LcheckBox.TabIndex = 15;
            this.LcheckBox.Text = "L";
            this.LcheckBox.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 199);
            this.Controls.Add(this.LcheckBox);
            this.Controls.Add(this.RcheckBox);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.checkButton);
            this.Controls.Add(this.openVideoButton);
            this.Controls.Add(this.portText);
            this.Controls.Add(this.videoText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.playerText);
            this.Controls.Add(this.selectPlayerButton);
            this.Name = "MainForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button selectPlayerButton;
        private System.Windows.Forms.TextBox playerText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox videoText;
        private System.Windows.Forms.TextBox portText;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button openVideoButton;
        private System.Windows.Forms.Button checkButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Label statusLabel;
        private ChkDialog CheckDialog;
        public System.Windows.Forms.CheckBox RcheckBox;
        public System.Windows.Forms.CheckBox LcheckBox;
        public bool lchkd;
        public bool rchkd;
    }

    public class ChkDialog : System.Windows.Forms.Form
    {
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Timer timer2;
        private System.ComponentModel.IContainer components;
        private MainForm _parent;
        private int i;

        public ChkDialog(MainForm parent)
        {
            _parent = parent;
            InitializeComponent();
            this.Visible = true;
            this.progressBar1.Maximum = 100;
            //this.progressBar1.Value = 100;
            StartSending();
        }

        private void StartTimer()
        {
            this.timer2.Start();
            this.i = 0;
        }

        public void StartSending()
        {
            var Rdata = new byte[5];
            int recv=0, steps = 2;

            _parent.rchkd = false;
            _parent.rchkd = false;

            for (int a = 1; a <= steps; a++)
            {
                StartTimer();
                System.Windows.Forms.Application.DoEvents();

                byte[] SLdata = Encoding.ASCII.GetBytes(_parent.ChkL);

                _parent._mysocket.SendTo(SLdata, SLdata.Length, SocketFlags.None, _parent._sender);
                var remote = (EndPoint)_parent._sender;
                while (this.i <= 3)//Wait for ansver 3 timer ticks
                {
                    System.Windows.Forms.Application.DoEvents();
                    recv = _parent._mysocket.Available;
                    if (recv > 0)
                    {
                        recv = _parent._mysocket.ReceiveFrom(Rdata, ref remote);
                        break;
                    }
                }
                var message = Encoding.ASCII.GetString(Rdata, 0, recv);

                if (message.Equals("lisok"))
                {
                    _parent.lchkd = true;
                    this.progressBar1.Value = this.progressBar1.Maximum / 2;
                    System.Windows.Forms.Application.DoEvents();
                    break;
                }

                this.progressBar1.Value = a * this.progressBar1.Maximum / (2 * steps);
                System.Windows.Forms.Application.DoEvents();
                timer2.Stop();
            }

            for (int a = 1; a <= steps; a++)//Wait for ansver 3 timer ticks
            {
                StartTimer();
                System.Windows.Forms.Application.DoEvents();

                byte[] SRdata = Encoding.ASCII.GetBytes(_parent.ChkR);

                _parent._mysocket.SendTo(SRdata, SRdata.Length, SocketFlags.None, _parent._sender);
                var remote = (EndPoint)_parent._sender;
                while (this.i <= 3)//Wait for ansver 2 timer ticks
                {
                    System.Windows.Forms.Application.DoEvents();
                    recv = _parent._mysocket.Available;
                    if (recv > 0)
                    {
                        recv = _parent._mysocket.ReceiveFrom(Rdata, Rdata.Length, SocketFlags.None, ref remote);
                        break;
                    }
                }
                var message = Encoding.ASCII.GetString(Rdata, 0, recv);

                if (message.Equals("risok"))
                {
                    _parent.rchkd = true;
                    this.progressBar1.Value = this.progressBar1.Maximum;
                    System.Windows.Forms.Application.DoEvents();
                    break;
                }     
           
                this.progressBar1.Value = a * this.progressBar1.Maximum/steps;
                System.Windows.Forms.Application.DoEvents();
                timer2.Stop();
            }
            this.progressBar1.Value = this.progressBar1.Maximum;
            System.Windows.Forms.Application.DoEvents();
            this.Close();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.label10 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 39);
            this.progressBar1.Maximum = 10;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(218, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 100;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // label1
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(25, 13);
            this.label10.Name = "label1";
            this.label10.Size = new System.Drawing.Size(196, 23);
            this.label10.TabIndex = 1;
            this.label10.Text = "Checking connection . . .";
            // 
            // ChkDialog
            // 
            this.ClientSize = new System.Drawing.Size(242, 74);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.progressBar1);
            this.Name = "ChkDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            i++;
        }

        
    }
}

