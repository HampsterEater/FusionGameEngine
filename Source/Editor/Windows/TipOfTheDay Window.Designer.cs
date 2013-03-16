namespace BinaryPhoenix.Fusion.Editor.Windows
{
	partial class TipOfTheDayWindow
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.tipLabel = new System.Windows.Forms.Label();
			this.stripPanel = new System.Windows.Forms.Panel();
			this.showOnStartupCheckBox = new System.Windows.Forms.CheckBox();
			this.closeButton = new System.Windows.Forms.Button();
			this.nextButton = new System.Windows.Forms.Button();
			this.previousButton = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.White;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.tipLabel);
			this.panel1.Controls.Add(this.stripPanel);
			this.panel1.Location = new System.Drawing.Point(11, 12);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(336, 220);
			this.panel1.TabIndex = 2;
			// 
			// tipLabel
			// 
			this.tipLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tipLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tipLabel.Location = new System.Drawing.Point(80, 8);
			this.tipLabel.Name = "tipLabel";
			this.tipLabel.Size = new System.Drawing.Size(248, 200);
			this.tipLabel.TabIndex = 1;
			this.tipLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// stripPanel
			// 
			this.stripPanel.BackColor = System.Drawing.Color.DarkGray;
			this.stripPanel.BackgroundImage = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.totd_strip;
			this.stripPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.stripPanel.Dock = System.Windows.Forms.DockStyle.Left;
			this.stripPanel.Location = new System.Drawing.Point(0, 0);
			this.stripPanel.Name = "stripPanel";
			this.stripPanel.Size = new System.Drawing.Size(72, 218);
			this.stripPanel.TabIndex = 0;
			// 
			// showOnStartupCheckBox
			// 
			this.showOnStartupCheckBox.AutoSize = true;
			this.showOnStartupCheckBox.Location = new System.Drawing.Point(12, 244);
			this.showOnStartupCheckBox.Name = "showOnStartupCheckBox";
			this.showOnStartupCheckBox.Size = new System.Drawing.Size(128, 17);
			this.showOnStartupCheckBox.TabIndex = 3;
			this.showOnStartupCheckBox.Text = "Show tips on startup?";
			this.showOnStartupCheckBox.UseVisualStyleBackColor = true;
			// 
			// closeButton
			// 
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.closeButton.Location = new System.Drawing.Point(272, 240);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(75, 23);
			this.closeButton.TabIndex = 4;
			this.closeButton.Text = "Close";
			this.closeButton.UseVisualStyleBackColor = true;
			this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
			// 
			// nextButton
			// 
			this.nextButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.nextButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.next;
			this.nextButton.Location = new System.Drawing.Point(228, 240);
			this.nextButton.Name = "nextButton";
			this.nextButton.Size = new System.Drawing.Size(24, 23);
			this.nextButton.TabIndex = 6;
			this.nextButton.UseVisualStyleBackColor = true;
			this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
			// 
			// previousButton
			// 
			this.previousButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.previousButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.previous;
			this.previousButton.Location = new System.Drawing.Point(200, 240);
			this.previousButton.Name = "previousButton";
			this.previousButton.Size = new System.Drawing.Size(24, 23);
			this.previousButton.TabIndex = 5;
			this.previousButton.UseVisualStyleBackColor = true;
			this.previousButton.Click += new System.EventHandler(this.previousButton_Click);
			// 
			// TipOfTheDayWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(359, 271);
			this.ControlBox = false;
			this.Controls.Add(this.nextButton);
			this.Controls.Add(this.previousButton);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.showOnStartupCheckBox);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TipOfTheDayWindow";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Tip Of The Day";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.CheckBox showOnStartupCheckBox;
		private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.Button previousButton;
		private System.Windows.Forms.Label tipLabel;
		private System.Windows.Forms.Panel stripPanel;
		private System.Windows.Forms.Button nextButton;
	}
}