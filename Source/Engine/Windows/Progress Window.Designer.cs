namespace BinaryPhoenix.Fusion.Engine.Windows
{
	partial class ProgressWindow
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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.subProcessLabel = new System.Windows.Forms.Label();
            this.hideButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 29);
            this.progressBar.MarqueeAnimationSpeed = 50;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(298, 19);
            this.progressBar.Step = 20;
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 0;
            // 
            // subProcessLabel
            // 
            this.subProcessLabel.AutoEllipsis = true;
            this.subProcessLabel.Location = new System.Drawing.Point(12, 9);
            this.subProcessLabel.Name = "subProcessLabel";
            this.subProcessLabel.Size = new System.Drawing.Size(298, 17);
            this.subProcessLabel.TabIndex = 1;
            this.subProcessLabel.Text = "Status";
            // 
            // hideButton
            // 
            this.hideButton.Location = new System.Drawing.Point(119, 56);
            this.hideButton.Name = "hideButton";
            this.hideButton.Size = new System.Drawing.Size(85, 23);
            this.hideButton.TabIndex = 2;
            this.hideButton.Text = "Hide";
            this.hideButton.UseVisualStyleBackColor = true;
            this.hideButton.Click += new System.EventHandler(this.hideButton_Click);
            // 
            // ProgressWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 87);
            this.ControlBox = false;
            this.Controls.Add(this.hideButton);
            this.Controls.Add(this.subProcessLabel);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Building Project...";
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label subProcessLabel;
        private System.Windows.Forms.Button hideButton;
	}
}