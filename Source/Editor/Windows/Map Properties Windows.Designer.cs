namespace BinaryPhoenix.Fusion.Editor.Windows
{
	partial class MapPropertiesWindow
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapPropertiesWindow));
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.optionTreeView = new System.Windows.Forms.TreeView();
			this.stepTabControl = new System.Windows.Forms.TabControl();
			this.generalTabPage = new System.Windows.Forms.TabPage();
			this.panel5 = new System.Windows.Forms.Panel();
			this.compressCheckBox = new System.Windows.Forms.CheckBox();
			this.debugTabPage = new System.Windows.Forms.TabPage();
			this.panel3 = new System.Windows.Forms.Panel();
			this.securityTabPage = new System.Windows.Forms.TabPage();
			this.encryptCheckBox = new System.Windows.Forms.CheckBox();
			this.panel4 = new System.Windows.Forms.Panel();
			this.label6 = new System.Windows.Forms.Label();
			this.passwordTextBox = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.infomationTabPage = new System.Windows.Forms.TabPage();
			this.panel2 = new System.Windows.Forms.Panel();
			this.descriptionTextBox = new System.Windows.Forms.TextBox();
			this.authorTextBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.versionNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.nameTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.stepTabControl.SuspendLayout();
			this.generalTabPage.SuspendLayout();
			this.debugTabPage.SuspendLayout();
			this.securityTabPage.SuspendLayout();
			this.infomationTabPage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.versionNumericUpDown)).BeginInit();
			this.SuspendLayout();
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "general.png");
			// 
			// cancelButton
			// 
			this.cancelButton.Location = new System.Drawing.Point(412, 284);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(76, 24);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(320, 284);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(76, 24);
			this.okButton.TabIndex = 2;
			this.okButton.Text = "Ok";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panel1.Location = new System.Drawing.Point(12, 272);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(484, 1);
			this.panel1.TabIndex = 3;
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
			this.optionTreeView.TabIndex = 4;
			this.optionTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.optionTreeView_AfterSelect);
			// 
			// stepTabControl
			// 
			this.stepTabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
			this.stepTabControl.Controls.Add(this.generalTabPage);
			this.stepTabControl.Controls.Add(this.debugTabPage);
			this.stepTabControl.Controls.Add(this.securityTabPage);
			this.stepTabControl.Controls.Add(this.infomationTabPage);
			this.stepTabControl.Location = new System.Drawing.Point(140, -25);
			this.stepTabControl.Name = "stepTabControl";
			this.stepTabControl.SelectedIndex = 0;
			this.stepTabControl.Size = new System.Drawing.Size(360, 288);
			this.stepTabControl.TabIndex = 5;
			// 
			// generalTabPage
			// 
			this.generalTabPage.Controls.Add(this.panel5);
			this.generalTabPage.Controls.Add(this.compressCheckBox);
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
			// compressCheckBox
			// 
			this.compressCheckBox.AutoSize = true;
			this.compressCheckBox.Location = new System.Drawing.Point(12, 24);
			this.compressCheckBox.Name = "compressCheckBox";
			this.compressCheckBox.Size = new System.Drawing.Size(102, 17);
			this.compressCheckBox.TabIndex = 17;
			this.compressCheckBox.Text = "Compress Map?";
			this.compressCheckBox.UseVisualStyleBackColor = true;
			this.compressCheckBox.CheckedChanged += new System.EventHandler(this.compressCheckBox_CheckedChanged);
			// 
			// debugTabPage
			// 
			this.debugTabPage.Controls.Add(this.panel3);
			this.debugTabPage.Location = new System.Drawing.Point(4, 25);
			this.debugTabPage.Name = "debugTabPage";
			this.debugTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.debugTabPage.Size = new System.Drawing.Size(352, 259);
			this.debugTabPage.TabIndex = 1;
			this.debugTabPage.Text = "Debug";
			this.debugTabPage.UseVisualStyleBackColor = true;
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
			this.securityTabPage.Controls.Add(this.encryptCheckBox);
			this.securityTabPage.Controls.Add(this.panel4);
			this.securityTabPage.Controls.Add(this.label6);
			this.securityTabPage.Controls.Add(this.passwordTextBox);
			this.securityTabPage.Controls.Add(this.label5);
			this.securityTabPage.Location = new System.Drawing.Point(4, 25);
			this.securityTabPage.Name = "securityTabPage";
			this.securityTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.securityTabPage.Size = new System.Drawing.Size(352, 259);
			this.securityTabPage.TabIndex = 2;
			this.securityTabPage.Text = "Security";
			this.securityTabPage.UseVisualStyleBackColor = true;
			// 
			// encryptCheckBox
			// 
			this.encryptCheckBox.AutoSize = true;
			this.encryptCheckBox.Location = new System.Drawing.Point(12, 24);
			this.encryptCheckBox.Name = "encryptCheckBox";
			this.encryptCheckBox.Size = new System.Drawing.Size(92, 17);
			this.encryptCheckBox.TabIndex = 15;
			this.encryptCheckBox.Text = "Encrypt Map?";
			this.encryptCheckBox.UseVisualStyleBackColor = true;
			this.encryptCheckBox.CheckedChanged += new System.EventHandler(this.encryptCheckBox_CheckedChanged);
			// 
			// panel4
			// 
			this.panel4.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panel4.Location = new System.Drawing.Point(0, 12);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(352, 1);
			this.panel4.TabIndex = 14;
			// 
			// label6
			// 
			this.label6.Enabled = false;
			this.label6.Location = new System.Drawing.Point(100, 76);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(216, 32);
			this.label6.TabIndex = 13;
			this.label6.Text = "Please be aware that losing this password will make this map totally inaccessable" +
				".";
			// 
			// passwordTextBox
			// 
			this.passwordTextBox.Enabled = false;
			this.passwordTextBox.Location = new System.Drawing.Point(100, 48);
			this.passwordTextBox.Name = "passwordTextBox";
			this.passwordTextBox.PasswordChar = '*';
			this.passwordTextBox.Size = new System.Drawing.Size(148, 20);
			this.passwordTextBox.TabIndex = 12;
			this.passwordTextBox.TextChanged += new System.EventHandler(this.passwordTextBox_TextChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(32, 52);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(56, 13);
			this.label5.TabIndex = 11;
			this.label5.Text = "Password ";
			// 
			// infomationTabPage
			// 
			this.infomationTabPage.Controls.Add(this.panel2);
			this.infomationTabPage.Controls.Add(this.descriptionTextBox);
			this.infomationTabPage.Controls.Add(this.authorTextBox);
			this.infomationTabPage.Controls.Add(this.label4);
			this.infomationTabPage.Controls.Add(this.label3);
			this.infomationTabPage.Controls.Add(this.versionNumericUpDown);
			this.infomationTabPage.Controls.Add(this.label2);
			this.infomationTabPage.Controls.Add(this.nameTextBox);
			this.infomationTabPage.Controls.Add(this.label1);
			this.infomationTabPage.Location = new System.Drawing.Point(4, 25);
			this.infomationTabPage.Name = "infomationTabPage";
			this.infomationTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.infomationTabPage.Size = new System.Drawing.Size(352, 259);
			this.infomationTabPage.TabIndex = 3;
			this.infomationTabPage.Text = "Infomation";
			this.infomationTabPage.UseVisualStyleBackColor = true;
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panel2.Location = new System.Drawing.Point(0, 12);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(352, 1);
			this.panel2.TabIndex = 14;
			// 
			// descriptionTextBox
			// 
			this.descriptionTextBox.Location = new System.Drawing.Point(80, 108);
			this.descriptionTextBox.Multiline = true;
			this.descriptionTextBox.Name = "descriptionTextBox";
			this.descriptionTextBox.Size = new System.Drawing.Size(268, 144);
			this.descriptionTextBox.TabIndex = 16;
			this.descriptionTextBox.TextChanged += new System.EventHandler(this.descriptionTextBox_TextChanged);
			// 
			// authorTextBox
			// 
			this.authorTextBox.Location = new System.Drawing.Point(80, 52);
			this.authorTextBox.Name = "authorTextBox";
			this.authorTextBox.Size = new System.Drawing.Size(148, 20);
			this.authorTextBox.TabIndex = 15;
			this.authorTextBox.TextChanged += new System.EventHandler(this.authorTextBox_TextChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 56);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(41, 13);
			this.label4.TabIndex = 13;
			this.label4.Text = "Author ";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 108);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(60, 13);
			this.label3.TabIndex = 12;
			this.label3.Text = "Description";
			// 
			// versionNumericUpDown
			// 
			this.versionNumericUpDown.DecimalPlaces = 1;
			this.versionNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.versionNumericUpDown.Location = new System.Drawing.Point(80, 80);
			this.versionNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.versionNumericUpDown.Name = "versionNumericUpDown";
			this.versionNumericUpDown.Size = new System.Drawing.Size(68, 20);
			this.versionNumericUpDown.TabIndex = 11;
			this.versionNumericUpDown.ValueChanged += new System.EventHandler(this.versionNumericUpDown_ValueChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 84);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(42, 13);
			this.label2.TabIndex = 10;
			this.label2.Text = "Version";
			// 
			// nameTextBox
			// 
			this.nameTextBox.Location = new System.Drawing.Point(80, 24);
			this.nameTextBox.Name = "nameTextBox";
			this.nameTextBox.Size = new System.Drawing.Size(148, 20);
			this.nameTextBox.TabIndex = 9;
			this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 13);
			this.label1.TabIndex = 8;
			this.label1.Text = "Map name ";
			// 
			// MapPropertiesWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(506, 322);
			this.Controls.Add(this.stepTabControl);
			this.Controls.Add(this.optionTreeView);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MapPropertiesWindow";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Map Properties";
			this.stepTabControl.ResumeLayout(false);
			this.generalTabPage.ResumeLayout(false);
			this.generalTabPage.PerformLayout();
			this.debugTabPage.ResumeLayout(false);
			this.securityTabPage.ResumeLayout(false);
			this.securityTabPage.PerformLayout();
			this.infomationTabPage.ResumeLayout(false);
			this.infomationTabPage.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.versionNumericUpDown)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.TreeView optionTreeView;
		private System.Windows.Forms.TabControl stepTabControl;
		private System.Windows.Forms.TabPage generalTabPage;
		private System.Windows.Forms.TabPage debugTabPage;
		private System.Windows.Forms.TabPage securityTabPage;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox passwordTextBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.CheckBox compressCheckBox;
		private System.Windows.Forms.CheckBox encryptCheckBox;
		private System.Windows.Forms.TabPage infomationTabPage;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.TextBox descriptionTextBox;
		private System.Windows.Forms.TextBox authorTextBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown versionNumericUpDown;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox nameTextBox;
		private System.Windows.Forms.Label label1;
	}
}