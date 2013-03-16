namespace BinaryPhoenix.Fusion.Updater.Windows
{
	partial class UpdaterWindow
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
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.installLabel = new System.Windows.Forms.Label();
            this.installProgressBar = new System.Windows.Forms.ProgressBar();
            this.finishButton = new System.Windows.Forms.Button();
            this.stepTabControl = new System.Windows.Forms.TabControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.stepDescriptionLabel = new System.Windows.Forms.Label();
            this.stepNameLabel = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tabPage2.SuspendLayout();
            this.stepTabControl.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.installLabel);
            this.tabPage2.Controls.Add(this.installProgressBar);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(488, 242);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Step 2 - Update Software";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // installLabel
            // 
            this.installLabel.AutoEllipsis = true;
            this.installLabel.Location = new System.Drawing.Point(25, 93);
            this.installLabel.Name = "installLabel";
            this.installLabel.Size = new System.Drawing.Size(430, 16);
            this.installLabel.TabIndex = 19;
            this.installLabel.Text = "Updating ...";
            // 
            // installProgressBar
            // 
            this.installProgressBar.Location = new System.Drawing.Point(28, 112);
            this.installProgressBar.Name = "installProgressBar";
            this.installProgressBar.Size = new System.Drawing.Size(433, 16);
            this.installProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.installProgressBar.TabIndex = 18;
            // 
            // finishButton
            // 
            this.finishButton.Enabled = false;
            this.finishButton.Location = new System.Drawing.Point(412, 320);
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(75, 23);
            this.finishButton.TabIndex = 23;
            this.finishButton.Text = "Finish";
            this.finishButton.UseVisualStyleBackColor = true;
            this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
            // 
            // stepTabControl
            // 
            this.stepTabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.stepTabControl.Controls.Add(this.tabPage2);
            this.stepTabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.stepTabControl.Location = new System.Drawing.Point(1, 32);
            this.stepTabControl.Multiline = true;
            this.stepTabControl.Name = "stepTabControl";
            this.stepTabControl.SelectedIndex = 0;
            this.stepTabControl.Size = new System.Drawing.Size(496, 271);
            this.stepTabControl.TabIndex = 9;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BackgroundImage = global::BinaryPhoenix.Fusion.Updater.Properties.Resources.top_strip;
            this.panel1.Controls.Add(this.stepDescriptionLabel);
            this.panel1.Controls.Add(this.stepNameLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(497, 60);
            this.panel1.TabIndex = 25;
            // 
            // stepDescriptionLabel
            // 
            this.stepDescriptionLabel.AutoSize = true;
            this.stepDescriptionLabel.Location = new System.Drawing.Point(44, 32);
            this.stepDescriptionLabel.Name = "stepDescriptionLabel";
            this.stepDescriptionLabel.Size = new System.Drawing.Size(254, 13);
            this.stepDescriptionLabel.TabIndex = 1;
            this.stepDescriptionLabel.Text = "The Fusion Game Engine is currently being updated.";
            // 
            // stepNameLabel
            // 
            this.stepNameLabel.AutoSize = true;
            this.stepNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stepNameLabel.Location = new System.Drawing.Point(20, 12);
            this.stepNameLabel.Name = "stepNameLabel";
            this.stepNameLabel.Size = new System.Drawing.Size(74, 13);
            this.stepNameLabel.TabIndex = 0;
            this.stepNameLabel.Text = "Updating ...";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 60);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(497, 1);
            this.panel2.TabIndex = 26;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel3.Location = new System.Drawing.Point(0, 305);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(496, 1);
            this.panel3.TabIndex = 27;
            // 
            // UpdaterWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 354);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.stepTabControl);
            this.Controls.Add(this.finishButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "UpdaterWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fusion Updater";
            this.Shown += new System.EventHandler(this.UpdaterWindow_Shown);
            this.tabPage2.ResumeLayout(false);
            this.stepTabControl.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button finishButton;
		private System.Windows.Forms.Label installLabel;
        private System.Windows.Forms.ProgressBar installProgressBar;
        private System.Windows.Forms.TabControl stepTabControl;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label stepDescriptionLabel;
		private System.Windows.Forms.Label stepNameLabel;
        private System.Windows.Forms.Panel panel3;
	}
}