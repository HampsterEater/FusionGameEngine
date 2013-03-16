namespace BinaryPhoenix.Fusion.Engine.Windows
{
	partial class AboutWindow
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutWindow));
			this.okButton = new System.Windows.Forms.Button();
			this.systemInfoButton = new System.Windows.Forms.Button();
			this.productLabel = new System.Windows.Forms.Label();
			this.versionLabel = new System.Windows.Forms.Label();
			this.copyrightLabel = new System.Windows.Forms.Label();
			this.creditsBox = new System.Windows.Forms.RichTextBox();
			this.binaryphoenixLinkLabel = new System.Windows.Forms.LinkLabel();
			this.companyLabel = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(352, 48);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(80, 24);
			this.okButton.TabIndex = 0;
			this.okButton.Text = "Ok";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// systemInfoButton
			// 
			this.systemInfoButton.Location = new System.Drawing.Point(352, 16);
			this.systemInfoButton.Name = "systemInfoButton";
			this.systemInfoButton.Size = new System.Drawing.Size(80, 24);
			this.systemInfoButton.TabIndex = 1;
			this.systemInfoButton.Text = "System Info";
			this.systemInfoButton.UseVisualStyleBackColor = true;
			this.systemInfoButton.Click += new System.EventHandler(this.systemInfoButton_Click);
			// 
			// productLabel
			// 
			this.productLabel.AutoSize = true;
			this.productLabel.Location = new System.Drawing.Point(16, 16);
			this.productLabel.Name = "productLabel";
			this.productLabel.Size = new System.Drawing.Size(68, 13);
			this.productLabel.TabIndex = 2;
			this.productLabel.Text = "Fusion Editor";
			// 
			// versionLabel
			// 
			this.versionLabel.AutoSize = true;
			this.versionLabel.Location = new System.Drawing.Point(16, 40);
			this.versionLabel.Name = "versionLabel";
			this.versionLabel.Size = new System.Drawing.Size(126, 13);
			this.versionLabel.TabIndex = 3;
			this.versionLabel.Text = "Version 000.000.000.000";
			// 
			// copyrightLabel
			// 
			this.copyrightLabel.AutoSize = true;
			this.copyrightLabel.Location = new System.Drawing.Point(16, 64);
			this.copyrightLabel.Name = "copyrightLabel";
			this.copyrightLabel.Size = new System.Drawing.Size(151, 13);
			this.copyrightLabel.TabIndex = 4;
			this.copyrightLabel.Text = "Copyright 2006 Binary Phoenix";
			// 
			// creditsBox
			// 
			this.creditsBox.BackColor = System.Drawing.Color.White;
			this.creditsBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.creditsBox.Location = new System.Drawing.Point(17, 112);
			this.creditsBox.Name = "creditsBox";
			this.creditsBox.ReadOnly = true;
			this.creditsBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.creditsBox.Size = new System.Drawing.Size(415, 72);
			this.creditsBox.TabIndex = 5;
			this.creditsBox.Text = resources.GetString("creditsBox.Text");
			// 
			// binaryphoenixLinkLabel
			// 
			this.binaryphoenixLinkLabel.AutoSize = true;
			this.binaryphoenixLinkLabel.Location = new System.Drawing.Point(280, 88);
			this.binaryphoenixLinkLabel.Name = "binaryphoenixLinkLabel";
			this.binaryphoenixLinkLabel.Size = new System.Drawing.Size(153, 13);
			this.binaryphoenixLinkLabel.TabIndex = 7;
			this.binaryphoenixLinkLabel.TabStop = true;
			this.binaryphoenixLinkLabel.Text = "http://www.binaryphoenix.com";
			// 
			// companyLabel
			// 
			this.companyLabel.AutoSize = true;
			this.companyLabel.Location = new System.Drawing.Point(16, 88);
			this.companyLabel.Name = "companyLabel";
			this.companyLabel.Size = new System.Drawing.Size(77, 13);
			this.companyLabel.TabIndex = 8;
			this.companyLabel.Text = "Binary Phoenix";
			// 
			// textBox1
			// 
			this.textBox1.BackColor = System.Drawing.SystemColors.Control;
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox1.Enabled = false;
			this.textBox1.Location = new System.Drawing.Point(16, 192);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(416, 56);
			this.textBox1.TabIndex = 9;
			this.textBox1.Text = resources.GetString("textBox1.Text");
			// 
			// AboutWindow
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(446, 258);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.companyLabel);
			this.Controls.Add(this.binaryphoenixLinkLabel);
			this.Controls.Add(this.creditsBox);
			this.Controls.Add(this.copyrightLabel);
			this.Controls.Add(this.versionLabel);
			this.Controls.Add(this.productLabel);
			this.Controls.Add(this.systemInfoButton);
			this.Controls.Add(this.okButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutWindow";
			this.Padding = new System.Windows.Forms.Padding(9);
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About Fusion Editor ...";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button systemInfoButton;
		private System.Windows.Forms.Label productLabel;
		private System.Windows.Forms.Label versionLabel;
		private System.Windows.Forms.Label copyrightLabel;
		private System.Windows.Forms.RichTextBox creditsBox;
		private System.Windows.Forms.LinkLabel binaryphoenixLinkLabel;
		private System.Windows.Forms.Label companyLabel;
		private System.Windows.Forms.TextBox textBox1;

	}
}
