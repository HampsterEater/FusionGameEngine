namespace BinaryPhoenix.Fusion.Engine.Windows
{
    partial class BanWindow
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
            this.label1 = new System.Windows.Forms.Label();
            this.banIPCheckBox = new System.Windows.Forms.CheckBox();
            this.banAccountCheckBox = new System.Windows.Forms.CheckBox();
            this.banButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.expirationDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(384, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please set the properties of the ban you wish to implement on the specified user." +
                "";
            // 
            // banIPCheckBox
            // 
            this.banIPCheckBox.AutoSize = true;
            this.banIPCheckBox.Location = new System.Drawing.Point(22, 41);
            this.banIPCheckBox.Name = "banIPCheckBox";
            this.banIPCheckBox.Size = new System.Drawing.Size(104, 17);
            this.banIPCheckBox.TabIndex = 1;
            this.banIPCheckBox.Text = "Ban IP address?";
            this.banIPCheckBox.UseVisualStyleBackColor = true;
            // 
            // banAccountCheckBox
            // 
            this.banAccountCheckBox.AutoSize = true;
            this.banAccountCheckBox.Location = new System.Drawing.Point(22, 64);
            this.banAccountCheckBox.Name = "banAccountCheckBox";
            this.banAccountCheckBox.Size = new System.Drawing.Size(94, 17);
            this.banAccountCheckBox.TabIndex = 2;
            this.banAccountCheckBox.Text = "Ban Account?";
            this.banAccountCheckBox.UseVisualStyleBackColor = true;
            // 
            // banButton
            // 
            this.banButton.Location = new System.Drawing.Point(12, 111);
            this.banButton.Name = "banButton";
            this.banButton.Size = new System.Drawing.Size(75, 23);
            this.banButton.TabIndex = 3;
            this.banButton.Text = "Ban User";
            this.banButton.UseVisualStyleBackColor = true;
            this.banButton.Click += new System.EventHandler(this.banButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(104, 111);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // expirationDateTimePicker
            // 
            this.expirationDateTimePicker.Location = new System.Drawing.Point(180, 59);
            this.expirationDateTimePicker.Name = "expirationDateTimePicker";
            this.expirationDateTimePicker.Size = new System.Drawing.Size(200, 20);
            this.expirationDateTimePicker.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(177, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Ban Expiration";
            // 
            // BanWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 145);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.expirationDateTimePicker);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.banButton);
            this.Controls.Add(this.banAccountCheckBox);
            this.Controls.Add(this.banIPCheckBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BanWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Ban User";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox banIPCheckBox;
        private System.Windows.Forms.CheckBox banAccountCheckBox;
        private System.Windows.Forms.Button banButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.DateTimePicker expirationDateTimePicker;
        private System.Windows.Forms.Label label2;
    }
}