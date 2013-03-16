namespace BinaryPhoenix.Fusion.Runtime.Controls
{
	partial class FindAndReplaceWindow
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
			this.findWhatTextBox = new System.Windows.Forms.TextBox();
			this.matchCaseCheckBox = new System.Windows.Forms.CheckBox();
			this.closeButton = new System.Windows.Forms.Button();
			this.findNextButton = new System.Windows.Forms.Button();
			this.replaceButton = new System.Windows.Forms.Button();
			this.replaceAllButton = new System.Windows.Forms.Button();
			this.replaceWithTextBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(59, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Find What:";
			// 
			// findWhatTextBox
			// 
			this.findWhatTextBox.Location = new System.Drawing.Point(16, 36);
			this.findWhatTextBox.Name = "findWhatTextBox";
			this.findWhatTextBox.Size = new System.Drawing.Size(352, 20);
			this.findWhatTextBox.TabIndex = 1;
			this.findWhatTextBox.TextChanged += new System.EventHandler(this.findWhatTextBox_TextChanged);
			// 
			// matchCaseCheckBox
			// 
			this.matchCaseCheckBox.AutoSize = true;
			this.matchCaseCheckBox.Location = new System.Drawing.Point(16, 120);
			this.matchCaseCheckBox.Name = "matchCaseCheckBox";
			this.matchCaseCheckBox.Size = new System.Drawing.Size(83, 17);
			this.matchCaseCheckBox.TabIndex = 2;
			this.matchCaseCheckBox.Text = "Match Case";
			this.matchCaseCheckBox.UseVisualStyleBackColor = true;
			this.matchCaseCheckBox.CheckedChanged += new System.EventHandler(this.matchCaseCheckBox_CheckedChanged);
			// 
			// closeButton
			// 
			this.closeButton.Location = new System.Drawing.Point(292, 148);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(80, 24);
			this.closeButton.TabIndex = 3;
			this.closeButton.Text = "Cancel";
			this.closeButton.UseVisualStyleBackColor = true;
			this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
			// 
			// findNextButton
			// 
			this.findNextButton.Location = new System.Drawing.Point(200, 148);
			this.findNextButton.Name = "findNextButton";
			this.findNextButton.Size = new System.Drawing.Size(80, 24);
			this.findNextButton.TabIndex = 4;
			this.findNextButton.Text = "Find Next";
			this.findNextButton.UseVisualStyleBackColor = true;
			this.findNextButton.Click += new System.EventHandler(this.findNextButton_Click);
			// 
			// replaceButton
			// 
			this.replaceButton.Location = new System.Drawing.Point(108, 148);
			this.replaceButton.Name = "replaceButton";
			this.replaceButton.Size = new System.Drawing.Size(80, 24);
			this.replaceButton.TabIndex = 5;
			this.replaceButton.Text = "Replace";
			this.replaceButton.UseVisualStyleBackColor = true;
			this.replaceButton.Click += new System.EventHandler(this.replaceButton_Click);
			// 
			// replaceAllButton
			// 
			this.replaceAllButton.Location = new System.Drawing.Point(16, 148);
			this.replaceAllButton.Name = "replaceAllButton";
			this.replaceAllButton.Size = new System.Drawing.Size(80, 24);
			this.replaceAllButton.TabIndex = 6;
			this.replaceAllButton.Text = "Replace All";
			this.replaceAllButton.UseVisualStyleBackColor = true;
			this.replaceAllButton.Click += new System.EventHandler(this.replaceAllButton_Click);
			// 
			// replaceWithTextBox
			// 
			this.replaceWithTextBox.Location = new System.Drawing.Point(16, 88);
			this.replaceWithTextBox.Name = "replaceWithTextBox";
			this.replaceWithTextBox.Size = new System.Drawing.Size(352, 20);
			this.replaceWithTextBox.TabIndex = 8;
			this.replaceWithTextBox.TextChanged += new System.EventHandler(this.replaceWithTextBox_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 68);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(75, 13);
			this.label2.TabIndex = 7;
			this.label2.Text = "Replace With:";
			// 
			// FindAndReplaceWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(387, 185);
			this.Controls.Add(this.replaceWithTextBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.replaceAllButton);
			this.Controls.Add(this.replaceButton);
			this.Controls.Add(this.findNextButton);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.matchCaseCheckBox);
			this.Controls.Add(this.findWhatTextBox);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FindAndReplaceWindow";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Find And Replace Text";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox findWhatTextBox;
		private System.Windows.Forms.CheckBox matchCaseCheckBox;
		private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.Button findNextButton;
		private System.Windows.Forms.Button replaceButton;
		private System.Windows.Forms.Button replaceAllButton;
		private System.Windows.Forms.TextBox replaceWithTextBox;
		private System.Windows.Forms.Label label2;
	}
}