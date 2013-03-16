namespace BinaryPhoenix.Fusion.Editor.Windows
{
	partial class ProjectBuildWindow
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
            this.stepTabControl = new System.Windows.Forms.TabControl();
            this.generalTabPage = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.fgeDistributableCheckBox = new System.Windows.Forms.CheckBox();
            this.standAloneCheckBox = new System.Windows.Forms.CheckBox();
            this.copySaveDirCheckBox = new System.Windows.Forms.CheckBox();
            this.buildDirButton = new System.Windows.Forms.Button();
            this.buildDirTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.debugTabPage = new System.Windows.Forms.TabPage();
            this.pakFilePanel = new System.Windows.Forms.Panel();
            this.compressPakFileCheckBox = new System.Windows.Forms.CheckBox();
            this.prefixTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.maxFileSizeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.compilePakFilesCheckBox = new System.Windows.Forms.CheckBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.securityTabPage = new System.Windows.Forms.TabPage();
            this.compileScriptsPanel = new System.Windows.Forms.Panel();
            this.treatMessagesAsErrorsCheckBox = new System.Windows.Forms.CheckBox();
            this.treatWarningsAsErrorsCheckBox = new System.Windows.Forms.CheckBox();
            this.compileInDebugCheckBox = new System.Windows.Forms.CheckBox();
            this.keepSourceCheckBox = new System.Windows.Forms.CheckBox();
            this.defineTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.compileScriptsCheckBox = new System.Windows.Forms.CheckBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.optionTreeView = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.stepTabControl.SuspendLayout();
            this.generalTabPage.SuspendLayout();
            this.debugTabPage.SuspendLayout();
            this.pakFilePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxFileSizeNumericUpDown)).BeginInit();
            this.securityTabPage.SuspendLayout();
            this.compileScriptsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // stepTabControl
            // 
            this.stepTabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.stepTabControl.Controls.Add(this.generalTabPage);
            this.stepTabControl.Controls.Add(this.debugTabPage);
            this.stepTabControl.Controls.Add(this.securityTabPage);
            this.stepTabControl.Location = new System.Drawing.Point(132, -23);
            this.stepTabControl.Name = "stepTabControl";
            this.stepTabControl.SelectedIndex = 0;
            this.stepTabControl.Size = new System.Drawing.Size(360, 289);
            this.stepTabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.stepTabControl.TabIndex = 10;
            // 
            // generalTabPage
            // 
            this.generalTabPage.Controls.Add(this.label2);
            this.generalTabPage.Controls.Add(this.label1);
            this.generalTabPage.Controls.Add(this.fgeDistributableCheckBox);
            this.generalTabPage.Controls.Add(this.standAloneCheckBox);
            this.generalTabPage.Controls.Add(this.copySaveDirCheckBox);
            this.generalTabPage.Controls.Add(this.buildDirButton);
            this.generalTabPage.Controls.Add(this.buildDirTextBox);
            this.generalTabPage.Controls.Add(this.label7);
            this.generalTabPage.Controls.Add(this.panel5);
            this.generalTabPage.Location = new System.Drawing.Point(4, 25);
            this.generalTabPage.Name = "generalTabPage";
            this.generalTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.generalTabPage.Size = new System.Drawing.Size(352, 260);
            this.generalTabPage.TabIndex = 0;
            this.generalTabPage.Text = "General";
            this.generalTabPage.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(120, 190);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(226, 55);
            this.label2.TabIndex = 26;
            this.label2.Text = "This will cause the project to be built to a pak file and icon file that can be d" +
                "istributed using the Fusion Games Explorer.";
            // 
            // label1
            // 
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(120, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(226, 67);
            this.label1.TabIndex = 25;
            this.label1.Text = "This will cause the project to be built to a single executable file. This is not " +
                "recomended as it encurs a speed penalty at start up. It is best to build it as a" +
                "n FSE Distributable file.";
            // 
            // fgeDistributableCheckBox
            // 
            this.fgeDistributableCheckBox.AutoSize = true;
            this.fgeDistributableCheckBox.Location = new System.Drawing.Point(103, 170);
            this.fgeDistributableCheckBox.Name = "fgeDistributableCheckBox";
            this.fgeDistributableCheckBox.Size = new System.Drawing.Size(155, 17);
            this.fgeDistributableCheckBox.TabIndex = 24;
            this.fgeDistributableCheckBox.Text = "Build As FGE Distributable?";
            this.fgeDistributableCheckBox.UseVisualStyleBackColor = true;
            this.fgeDistributableCheckBox.CheckedChanged += new System.EventHandler(this.fgeDistributableCheckBox_CheckedChanged);
            // 
            // standAloneCheckBox
            // 
            this.standAloneCheckBox.AutoSize = true;
            this.standAloneCheckBox.Location = new System.Drawing.Point(103, 80);
            this.standAloneCheckBox.Name = "standAloneCheckBox";
            this.standAloneCheckBox.Size = new System.Drawing.Size(131, 17);
            this.standAloneCheckBox.TabIndex = 23;
            this.standAloneCheckBox.Text = "Build As Stand Alone?";
            this.standAloneCheckBox.UseVisualStyleBackColor = true;
            this.standAloneCheckBox.CheckedChanged += new System.EventHandler(this.standAloneCheckBox_CheckedChanged);
            // 
            // copySaveDirCheckBox
            // 
            this.copySaveDirCheckBox.AutoSize = true;
            this.copySaveDirCheckBox.Location = new System.Drawing.Point(103, 57);
            this.copySaveDirCheckBox.Name = "copySaveDirCheckBox";
            this.copySaveDirCheckBox.Size = new System.Drawing.Size(129, 17);
            this.copySaveDirCheckBox.TabIndex = 22;
            this.copySaveDirCheckBox.Text = "Copy Save Directory?";
            this.copySaveDirCheckBox.UseVisualStyleBackColor = true;
            this.copySaveDirCheckBox.CheckedChanged += new System.EventHandler(this.copySaveDirCheckBox_CheckedChanged);
            // 
            // buildDirButton
            // 
            this.buildDirButton.Location = new System.Drawing.Point(243, 31);
            this.buildDirButton.Name = "buildDirButton";
            this.buildDirButton.Size = new System.Drawing.Size(24, 20);
            this.buildDirButton.TabIndex = 21;
            this.buildDirButton.Text = "...";
            this.buildDirButton.UseVisualStyleBackColor = true;
            this.buildDirButton.Click += new System.EventHandler(this.buildDirButton_Click);
            // 
            // buildDirTextBox
            // 
            this.buildDirTextBox.Location = new System.Drawing.Point(103, 31);
            this.buildDirTextBox.Name = "buildDirTextBox";
            this.buildDirTextBox.Size = new System.Drawing.Size(132, 20);
            this.buildDirTextBox.TabIndex = 20;
            this.buildDirTextBox.TextChanged += new System.EventHandler(this.buildDirTextBox_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 35);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Build Directory";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel5.Location = new System.Drawing.Point(0, 12);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(352, 1);
            this.panel5.TabIndex = 18;
            // 
            // debugTabPage
            // 
            this.debugTabPage.Controls.Add(this.pakFilePanel);
            this.debugTabPage.Controls.Add(this.compilePakFilesCheckBox);
            this.debugTabPage.Controls.Add(this.panel3);
            this.debugTabPage.Location = new System.Drawing.Point(4, 25);
            this.debugTabPage.Name = "debugTabPage";
            this.debugTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.debugTabPage.Size = new System.Drawing.Size(352, 260);
            this.debugTabPage.TabIndex = 1;
            this.debugTabPage.Text = "Pak Files";
            this.debugTabPage.UseVisualStyleBackColor = true;
            // 
            // pakFilePanel
            // 
            this.pakFilePanel.Controls.Add(this.compressPakFileCheckBox);
            this.pakFilePanel.Controls.Add(this.prefixTextBox);
            this.pakFilePanel.Controls.Add(this.label9);
            this.pakFilePanel.Controls.Add(this.maxFileSizeNumericUpDown);
            this.pakFilePanel.Controls.Add(this.label8);
            this.pakFilePanel.Location = new System.Drawing.Point(28, 48);
            this.pakFilePanel.Name = "pakFilePanel";
            this.pakFilePanel.Size = new System.Drawing.Size(299, 132);
            this.pakFilePanel.TabIndex = 13;
            // 
            // compressPakFileCheckBox
            // 
            this.compressPakFileCheckBox.AutoSize = true;
            this.compressPakFileCheckBox.Location = new System.Drawing.Point(112, 62);
            this.compressPakFileCheckBox.Name = "compressPakFileCheckBox";
            this.compressPakFileCheckBox.Size = new System.Drawing.Size(124, 17);
            this.compressPakFileCheckBox.TabIndex = 15;
            this.compressPakFileCheckBox.Text = "Compress Pak Files?";
            this.compressPakFileCheckBox.UseVisualStyleBackColor = true;
            this.compressPakFileCheckBox.CheckedChanged += new System.EventHandler(this.compressPakFileCheckBox_CheckedChanged);
            // 
            // prefixTextBox
            // 
            this.prefixTextBox.Location = new System.Drawing.Point(112, 36);
            this.prefixTextBox.Name = "prefixTextBox";
            this.prefixTextBox.Size = new System.Drawing.Size(88, 20);
            this.prefixTextBox.TabIndex = 14;
            this.prefixTextBox.TextChanged += new System.EventHandler(this.prefixTextBox_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(64, 40);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(33, 13);
            this.label9.TabIndex = 13;
            this.label9.Text = "Prefix";
            // 
            // maxFileSizeNumericUpDown
            // 
            this.maxFileSizeNumericUpDown.Increment = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.maxFileSizeNumericUpDown.Location = new System.Drawing.Point(112, 8);
            this.maxFileSizeNumericUpDown.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.maxFileSizeNumericUpDown.Name = "maxFileSizeNumericUpDown";
            this.maxFileSizeNumericUpDown.Size = new System.Drawing.Size(88, 20);
            this.maxFileSizeNumericUpDown.TabIndex = 12;
            this.maxFileSizeNumericUpDown.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.maxFileSizeNumericUpDown.ValueChanged += new System.EventHandler(this.maxFileSizeNumericUpDown_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 12);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(93, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Maximum File Size";
            // 
            // compilePakFilesCheckBox
            // 
            this.compilePakFilesCheckBox.AutoSize = true;
            this.compilePakFilesCheckBox.Location = new System.Drawing.Point(12, 24);
            this.compilePakFilesCheckBox.Name = "compilePakFilesCheckBox";
            this.compilePakFilesCheckBox.Size = new System.Drawing.Size(163, 17);
            this.compilePakFilesCheckBox.TabIndex = 10;
            this.compilePakFilesCheckBox.Text = "Compile Media To Pak Files?";
            this.compilePakFilesCheckBox.UseVisualStyleBackColor = true;
            this.compilePakFilesCheckBox.CheckedChanged += new System.EventHandler(this.compilePakFilesCheckBox_CheckedChanged);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel3.Location = new System.Drawing.Point(0, 12);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(352, 1);
            this.panel3.TabIndex = 9;
            // 
            // securityTabPage
            // 
            this.securityTabPage.Controls.Add(this.compileScriptsPanel);
            this.securityTabPage.Controls.Add(this.compileScriptsCheckBox);
            this.securityTabPage.Controls.Add(this.panel4);
            this.securityTabPage.Location = new System.Drawing.Point(4, 25);
            this.securityTabPage.Name = "securityTabPage";
            this.securityTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.securityTabPage.Size = new System.Drawing.Size(352, 260);
            this.securityTabPage.TabIndex = 2;
            this.securityTabPage.Text = "Scripts";
            this.securityTabPage.UseVisualStyleBackColor = true;
            // 
            // compileScriptsPanel
            // 
            this.compileScriptsPanel.Controls.Add(this.treatMessagesAsErrorsCheckBox);
            this.compileScriptsPanel.Controls.Add(this.treatWarningsAsErrorsCheckBox);
            this.compileScriptsPanel.Controls.Add(this.compileInDebugCheckBox);
            this.compileScriptsPanel.Controls.Add(this.keepSourceCheckBox);
            this.compileScriptsPanel.Controls.Add(this.defineTextBox);
            this.compileScriptsPanel.Controls.Add(this.label5);
            this.compileScriptsPanel.Location = new System.Drawing.Point(28, 48);
            this.compileScriptsPanel.Name = "compileScriptsPanel";
            this.compileScriptsPanel.Size = new System.Drawing.Size(296, 132);
            this.compileScriptsPanel.TabIndex = 16;
            // 
            // treatMessagesAsErrorsCheckBox
            // 
            this.treatMessagesAsErrorsCheckBox.AutoSize = true;
            this.treatMessagesAsErrorsCheckBox.Location = new System.Drawing.Point(12, 108);
            this.treatMessagesAsErrorsCheckBox.Name = "treatMessagesAsErrorsCheckBox";
            this.treatMessagesAsErrorsCheckBox.Size = new System.Drawing.Size(153, 17);
            this.treatMessagesAsErrorsCheckBox.TabIndex = 5;
            this.treatMessagesAsErrorsCheckBox.Text = "Treat Messages As Errors?";
            this.treatMessagesAsErrorsCheckBox.UseVisualStyleBackColor = true;
            this.treatMessagesAsErrorsCheckBox.CheckedChanged += new System.EventHandler(this.treatMessagesAsErrorsCheckBox_CheckedChanged);
            // 
            // treatWarningsAsErrorsCheckBox
            // 
            this.treatWarningsAsErrorsCheckBox.AutoSize = true;
            this.treatWarningsAsErrorsCheckBox.Location = new System.Drawing.Point(12, 84);
            this.treatWarningsAsErrorsCheckBox.Name = "treatWarningsAsErrorsCheckBox";
            this.treatWarningsAsErrorsCheckBox.Size = new System.Drawing.Size(150, 17);
            this.treatWarningsAsErrorsCheckBox.TabIndex = 4;
            this.treatWarningsAsErrorsCheckBox.Text = "Treat Warnings As Errors?";
            this.treatWarningsAsErrorsCheckBox.UseVisualStyleBackColor = true;
            this.treatWarningsAsErrorsCheckBox.CheckedChanged += new System.EventHandler(this.treatWarningsAsErrorsCheckBox_CheckedChanged);
            // 
            // compileInDebugCheckBox
            // 
            this.compileInDebugCheckBox.AutoSize = true;
            this.compileInDebugCheckBox.Location = new System.Drawing.Point(12, 60);
            this.compileInDebugCheckBox.Name = "compileInDebugCheckBox";
            this.compileInDebugCheckBox.Size = new System.Drawing.Size(146, 17);
            this.compileInDebugCheckBox.TabIndex = 3;
            this.compileInDebugCheckBox.Text = "Compile In Debug Mode?";
            this.compileInDebugCheckBox.UseVisualStyleBackColor = true;
            this.compileInDebugCheckBox.CheckedChanged += new System.EventHandler(this.compileInDebugCheckBox_CheckedChanged);
            // 
            // keepSourceCheckBox
            // 
            this.keepSourceCheckBox.AutoSize = true;
            this.keepSourceCheckBox.Location = new System.Drawing.Point(12, 36);
            this.keepSourceCheckBox.Name = "keepSourceCheckBox";
            this.keepSourceCheckBox.Size = new System.Drawing.Size(124, 17);
            this.keepSourceCheckBox.TabIndex = 2;
            this.keepSourceCheckBox.Text = "Keep Script Source?";
            this.keepSourceCheckBox.UseVisualStyleBackColor = true;
            this.keepSourceCheckBox.CheckedChanged += new System.EventHandler(this.keepSourceCheckBox_CheckedChanged);
            // 
            // defineTextBox
            // 
            this.defineTextBox.Location = new System.Drawing.Point(116, 8);
            this.defineTextBox.Name = "defineTextBox";
            this.defineTextBox.Size = new System.Drawing.Size(168, 20);
            this.defineTextBox.TabIndex = 1;
            this.defineTextBox.TextChanged += new System.EventHandler(this.defineTextBox_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Preprocess Defines";
            // 
            // compileScriptsCheckBox
            // 
            this.compileScriptsCheckBox.AutoSize = true;
            this.compileScriptsCheckBox.Location = new System.Drawing.Point(12, 24);
            this.compileScriptsCheckBox.Name = "compileScriptsCheckBox";
            this.compileScriptsCheckBox.Size = new System.Drawing.Size(104, 17);
            this.compileScriptsCheckBox.TabIndex = 15;
            this.compileScriptsCheckBox.Text = "Compile Scripts?";
            this.compileScriptsCheckBox.UseVisualStyleBackColor = true;
            this.compileScriptsCheckBox.CheckedChanged += new System.EventHandler(this.compileScriptsCheckBox_CheckedChanged);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel4.Location = new System.Drawing.Point(0, 12);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(352, 1);
            this.panel4.TabIndex = 14;
            // 
            // optionTreeView
            // 
            this.optionTreeView.FullRowSelect = true;
            this.optionTreeView.HotTracking = true;
            this.optionTreeView.ItemHeight = 20;
            this.optionTreeView.Location = new System.Drawing.Point(8, 12);
            this.optionTreeView.Name = "optionTreeView";
            this.optionTreeView.ShowNodeToolTips = true;
            this.optionTreeView.Size = new System.Drawing.Size(121, 248);
            this.optionTreeView.TabIndex = 9;
            this.optionTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.optionTreeView_AfterSelect);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Location = new System.Drawing.Point(8, 272);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(484, 1);
            this.panel1.TabIndex = 8;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(316, 284);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(76, 24);
            this.okButton.TabIndex = 7;
            this.okButton.Text = "Build";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(408, 284);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(76, 24);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // ProjectBuildWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 319);
            this.Controls.Add(this.stepTabControl);
            this.Controls.Add(this.optionTreeView);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProjectBuildWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Build Project";
            this.stepTabControl.ResumeLayout(false);
            this.generalTabPage.ResumeLayout(false);
            this.generalTabPage.PerformLayout();
            this.debugTabPage.ResumeLayout(false);
            this.debugTabPage.PerformLayout();
            this.pakFilePanel.ResumeLayout(false);
            this.pakFilePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxFileSizeNumericUpDown)).EndInit();
            this.securityTabPage.ResumeLayout(false);
            this.securityTabPage.PerformLayout();
            this.compileScriptsPanel.ResumeLayout(false);
            this.compileScriptsPanel.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl stepTabControl;
		private System.Windows.Forms.TabPage generalTabPage;
		private System.Windows.Forms.Button buildDirButton;
		private System.Windows.Forms.TextBox buildDirTextBox;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.TabPage debugTabPage;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.TabPage securityTabPage;
		private System.Windows.Forms.CheckBox compileScriptsCheckBox;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.TreeView optionTreeView;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.CheckBox copySaveDirCheckBox;
		private System.Windows.Forms.CheckBox compilePakFilesCheckBox;
		private System.Windows.Forms.NumericUpDown maxFileSizeNumericUpDown;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Panel pakFilePanel;
		private System.Windows.Forms.TextBox prefixTextBox;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Panel compileScriptsPanel;
		private System.Windows.Forms.TextBox defineTextBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.CheckBox keepSourceCheckBox;
		private System.Windows.Forms.CheckBox compileInDebugCheckBox;
		private System.Windows.Forms.CheckBox treatMessagesAsErrorsCheckBox;
		private System.Windows.Forms.CheckBox treatWarningsAsErrorsCheckBox;
        private System.Windows.Forms.CheckBox standAloneCheckBox;
        private System.Windows.Forms.CheckBox compressPakFileCheckBox;
        private System.Windows.Forms.CheckBox fgeDistributableCheckBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;

	}
}