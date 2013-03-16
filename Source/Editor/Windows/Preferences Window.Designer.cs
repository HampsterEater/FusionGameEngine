namespace BinaryPhoenix.Fusion.Editor.Windows
{
	partial class PreferencesWindow
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
			this.optionTreeView = new System.Windows.Forms.TreeView();
			this.panel1 = new System.Windows.Forms.Panel();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.generalTabPage = new System.Windows.Forms.TabPage();
			this.panel5 = new System.Windows.Forms.Panel();
			this.stepTabControl = new System.Windows.Forms.TabControl();
			this.generalTabPage.SuspendLayout();
			this.stepTabControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// optionTreeView
			// 
			this.optionTreeView.FullRowSelect = true;
			this.optionTreeView.HotTracking = true;
			this.optionTreeView.ItemHeight = 20;
			this.optionTreeView.Location = new System.Drawing.Point(12, 12);
			this.optionTreeView.Name = "optionTreeView";
			this.optionTreeView.ShowNodeToolTips = true;
			this.optionTreeView.Size = new System.Drawing.Size(121, 248);
			this.optionTreeView.TabIndex = 6;
			this.optionTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.optionTreeView_AfterSelect);
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panel1.Location = new System.Drawing.Point(12, 272);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(484, 1);
			this.panel1.TabIndex = 10;
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(320, 284);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(76, 24);
			this.okButton.TabIndex = 9;
			this.okButton.Text = "Ok";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.Location = new System.Drawing.Point(412, 284);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(76, 24);
			this.cancelButton.TabIndex = 8;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// generalTabPage
			// 
			this.generalTabPage.Controls.Add(this.panel5);
			this.generalTabPage.Location = new System.Drawing.Point(4, 25);
			this.generalTabPage.Name = "generalTabPage";
			this.generalTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.generalTabPage.Size = new System.Drawing.Size(352, 259);
			this.generalTabPage.TabIndex = 0;
			this.generalTabPage.Text = "General";
			this.generalTabPage.UseVisualStyleBackColor = true;
			// 
			// panel5
			// 
			this.panel5.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panel5.Location = new System.Drawing.Point(0, 12);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(352, 1);
			this.panel5.TabIndex = 18;
			// 
			// stepTabControl
			// 
			this.stepTabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
			this.stepTabControl.Controls.Add(this.generalTabPage);
			this.stepTabControl.Location = new System.Drawing.Point(140, -25);
			this.stepTabControl.Name = "stepTabControl";
			this.stepTabControl.SelectedIndex = 0;
			this.stepTabControl.Size = new System.Drawing.Size(360, 288);
			this.stepTabControl.TabIndex = 7;
			// 
			// PreferencesWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(506, 322);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.stepTabControl);
			this.Controls.Add(this.optionTreeView);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PreferencesWindow";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Preferences";
			this.generalTabPage.ResumeLayout(false);
			this.stepTabControl.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView optionTreeView;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.TabPage generalTabPage;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.TabControl stepTabControl;

	}
}