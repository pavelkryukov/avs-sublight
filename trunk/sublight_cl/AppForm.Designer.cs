namespace sublight_cl
{
    partial class AppForm
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
            this.portBox = new System.Windows.Forms.TextBox();
            this.portLabel = new System.Windows.Forms.Label();
            this.leftButton = new System.Windows.Forms.RadioButton();
            this.rightButton = new System.Windows.Forms.RadioButton();
            this.sideBox = new System.Windows.Forms.GroupBox();
            this.startButton = new System.Windows.Forms.Button();
            this.topButton = new System.Windows.Forms.RadioButton();
            this.sideBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // portBox
            // 
            this.portBox.Location = new System.Drawing.Point(44, 12);
            this.portBox.Name = "portBox";
            this.portBox.Size = new System.Drawing.Size(105, 20);
            this.portBox.TabIndex = 0;
            this.portBox.Text = "12050";
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(12, 15);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(26, 13);
            this.portLabel.TabIndex = 1;
            this.portLabel.Text = "Port";
            // 
            // leftButton
            // 
            this.leftButton.AutoSize = true;
            this.leftButton.Checked = true;
            this.leftButton.Location = new System.Drawing.Point(6, 19);
            this.leftButton.Name = "leftButton";
            this.leftButton.Size = new System.Drawing.Size(43, 17);
            this.leftButton.TabIndex = 2;
            this.leftButton.TabStop = true;
            this.leftButton.Text = "Left";
            this.leftButton.UseVisualStyleBackColor = true;
            // 
            // rightButton
            // 
            this.rightButton.AutoSize = true;
            this.rightButton.Location = new System.Drawing.Point(6, 42);
            this.rightButton.Name = "rightButton";
            this.rightButton.Size = new System.Drawing.Size(50, 17);
            this.rightButton.TabIndex = 3;
            this.rightButton.TabStop = true;
            this.rightButton.Text = "Right";
            this.rightButton.UseVisualStyleBackColor = true;
            // 
            // sideBox
            // 
            this.sideBox.Controls.Add(this.topButton);
            this.sideBox.Controls.Add(this.leftButton);
            this.sideBox.Controls.Add(this.rightButton);
            this.sideBox.Location = new System.Drawing.Point(12, 38);
            this.sideBox.Name = "sideBox";
            this.sideBox.Size = new System.Drawing.Size(137, 91);
            this.sideBox.TabIndex = 4;
            this.sideBox.TabStop = false;
            this.sideBox.Text = "Side";
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(213, 46);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 28);
            this.startButton.TabIndex = 5;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.StartButtonClick);
            // 
            // topButton
            // 
            this.topButton.AutoSize = true;
            this.topButton.Location = new System.Drawing.Point(6, 65);
            this.topButton.Name = "topButton";
            this.topButton.Size = new System.Drawing.Size(44, 17);
            this.topButton.TabIndex = 4;
            this.topButton.TabStop = true;
            this.topButton.Text = "Top";
            this.topButton.UseVisualStyleBackColor = true;
            // 
            // AppForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 141);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.sideBox);
            this.Controls.Add(this.portLabel);
            this.Controls.Add(this.portBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::sublight_cl.Properties.Resources.Monitor;
            this.MaximizeBox = false;
            this.Name = "AppForm";
            this.Text = "Sublight UDP Receiver";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnKill);
            this.sideBox.ResumeLayout(false);
            this.sideBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox portBox;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.RadioButton leftButton;
        private System.Windows.Forms.RadioButton rightButton;
        private System.Windows.Forms.GroupBox sideBox;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.RadioButton topButton;
 //       private System.Windows.Forms.Button stopButton;
    }
}