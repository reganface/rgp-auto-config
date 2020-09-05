namespace RGP_Auto_Config
{
    partial class Form1
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
            this.pinText = new System.Windows.Forms.Label();
            this.inputPin = new System.Windows.Forms.TextBox();
            this.signInBtn = new System.Windows.Forms.Button();
            this.logo = new System.Windows.Forms.PictureBox();
            this.panelLogin = new System.Windows.Forms.Panel();
            this.versionLabel = new System.Windows.Forms.Label();
            this.locationSelect = new System.Windows.Forms.ComboBox();
            this.statusText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.logo)).BeginInit();
            this.panelLogin.SuspendLayout();
            this.SuspendLayout();
            // 
            // pinText
            // 
            this.pinText.AutoSize = true;
            this.pinText.Location = new System.Drawing.Point(165, 201);
            this.pinText.Name = "pinText";
            this.pinText.Size = new System.Drawing.Size(25, 13);
            this.pinText.TabIndex = 0;
            this.pinText.Text = "PIN";
            // 
            // inputPin
            // 
            this.inputPin.Location = new System.Drawing.Point(196, 198);
            this.inputPin.Name = "inputPin";
            this.inputPin.PasswordChar = '*';
            this.inputPin.Size = new System.Drawing.Size(100, 20);
            this.inputPin.TabIndex = 1;
            // 
            // signInBtn
            // 
            this.signInBtn.Location = new System.Drawing.Point(302, 197);
            this.signInBtn.Name = "signInBtn";
            this.signInBtn.Size = new System.Drawing.Size(75, 23);
            this.signInBtn.TabIndex = 2;
            this.signInBtn.Text = "OK";
            this.signInBtn.UseVisualStyleBackColor = true;
            this.signInBtn.Click += new System.EventHandler(this.signInBtn_Click);
            // 
            // logo
            // 
            this.logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.logo.Location = new System.Drawing.Point(12, 12);
            this.logo.Name = "logo";
            this.logo.Size = new System.Drawing.Size(365, 99);
            this.logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.logo.TabIndex = 4;
            this.logo.TabStop = false;
            // 
            // panelLogin
            // 
            this.panelLogin.Controls.Add(this.versionLabel);
            this.panelLogin.Controls.Add(this.locationSelect);
            this.panelLogin.Controls.Add(this.statusText);
            this.panelLogin.Controls.Add(this.signInBtn);
            this.panelLogin.Controls.Add(this.inputPin);
            this.panelLogin.Controls.Add(this.pinText);
            this.panelLogin.Controls.Add(this.logo);
            this.panelLogin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLogin.Location = new System.Drawing.Point(0, 0);
            this.panelLogin.Name = "panelLogin";
            this.panelLogin.Size = new System.Drawing.Size(389, 231);
            this.panelLogin.TabIndex = 6;
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.Location = new System.Drawing.Point(9, 207);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(72, 13);
            this.versionLabel.TabIndex = 8;
            this.versionLabel.Text = "Version: 1.0.0";
            // 
            // locationSelect
            // 
            this.locationSelect.FormattingEnabled = true;
            this.locationSelect.Location = new System.Drawing.Point(141, 198);
            this.locationSelect.Name = "locationSelect";
            this.locationSelect.Size = new System.Drawing.Size(155, 21);
            this.locationSelect.TabIndex = 7;
            this.locationSelect.Visible = false;
            // 
            // statusText
            // 
            this.statusText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusText.Location = new System.Drawing.Point(12, 114);
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(365, 79);
            this.statusText.TabIndex = 7;
            this.statusText.Text = "Enter your PIN to configure RGP";
            this.statusText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AcceptButton = this.signInBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 231);
            this.Controls.Add(this.panelLogin);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RGP Auto Configuration";
            ((System.ComponentModel.ISupportInitialize)(this.logo)).EndInit();
            this.panelLogin.ResumeLayout(false);
            this.panelLogin.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label pinText;
        private System.Windows.Forms.TextBox inputPin;
        private System.Windows.Forms.Button signInBtn;
        private System.Windows.Forms.PictureBox logo;
        private System.Windows.Forms.Panel panelLogin;
        private System.Windows.Forms.Label statusText;
        private System.Windows.Forms.ComboBox locationSelect;
        private System.Windows.Forms.Label versionLabel;
    }
}

