namespace BinaryPhoenix.Fusion.Editor.Windows
{
	partial class NewTilesetWindow
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
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.tilesetFileLabel = new System.Windows.Forms.Label();
			this.tilesetFileTextBox = new System.Windows.Forms.TextBox();
			this.infoGroupBox = new System.Windows.Forms.GroupBox();
			this.tilesetFileBrowseButton = new System.Windows.Forms.Button();
			this.imageGroupBox = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.imageVerticalSpaceBox = new System.Windows.Forms.NumericUpDown();
			this.imageHorizontalSpaceBox = new System.Windows.Forms.NumericUpDown();
			this.imageSpaceLabel = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.imageVerticalSizeBox = new System.Windows.Forms.NumericUpDown();
			this.imageHorizontalSizeBox = new System.Windows.Forms.NumericUpDown();
			this.imageSizeLabel = new System.Windows.Forms.Label();
			this.imageFileBrowseButton = new System.Windows.Forms.Button();
			this.imageFileTextBox = new System.Windows.Forms.TextBox();
			this.imageFileLabel = new System.Windows.Forms.Label();
			this.infoGroupBox.SuspendLayout();
			this.imageGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.imageVerticalSpaceBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imageHorizontalSpaceBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imageVerticalSizeBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imageHorizontalSizeBox)).BeginInit();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(324, 16);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 0;
			this.okButton.Text = "Ok";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.Location = new System.Drawing.Point(324, 48);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// tilesetFileLabel
			// 
			this.tilesetFileLabel.AutoSize = true;
			this.tilesetFileLabel.Location = new System.Drawing.Point(16, 24);
			this.tilesetFileLabel.Name = "tilesetFileLabel";
			this.tilesetFileLabel.Size = new System.Drawing.Size(23, 13);
			this.tilesetFileLabel.TabIndex = 2;
			this.tilesetFileLabel.Text = "File";
			// 
			// tilesetFileTextBox
			// 
			this.tilesetFileTextBox.Location = new System.Drawing.Point(48, 20);
			this.tilesetFileTextBox.Name = "tilesetFileTextBox";
			this.tilesetFileTextBox.ReadOnly = true;
			this.tilesetFileTextBox.Size = new System.Drawing.Size(200, 20);
			this.tilesetFileTextBox.TabIndex = 3;
			// 
			// infoGroupBox
			// 
			this.infoGroupBox.Controls.Add(this.tilesetFileBrowseButton);
			this.infoGroupBox.Controls.Add(this.tilesetFileTextBox);
			this.infoGroupBox.Controls.Add(this.tilesetFileLabel);
			this.infoGroupBox.Location = new System.Drawing.Point(12, 12);
			this.infoGroupBox.Name = "infoGroupBox";
			this.infoGroupBox.Size = new System.Drawing.Size(296, 56);
			this.infoGroupBox.TabIndex = 4;
			this.infoGroupBox.TabStop = false;
			this.infoGroupBox.Text = "Info";
			// 
			// tilesetFileBrowseButton
			// 
			this.tilesetFileBrowseButton.Location = new System.Drawing.Point(256, 20);
			this.tilesetFileBrowseButton.Name = "tilesetFileBrowseButton";
			this.tilesetFileBrowseButton.Size = new System.Drawing.Size(24, 19);
			this.tilesetFileBrowseButton.TabIndex = 7;
			this.tilesetFileBrowseButton.Text = "...";
			this.tilesetFileBrowseButton.UseVisualStyleBackColor = true;
			this.tilesetFileBrowseButton.Click += new System.EventHandler(this.tilesetFileBrowseButton_Click);
			// 
			// imageGroupBox
			// 
			this.imageGroupBox.Controls.Add(this.label5);
			this.imageGroupBox.Controls.Add(this.imageVerticalSpaceBox);
			this.imageGroupBox.Controls.Add(this.imageHorizontalSpaceBox);
			this.imageGroupBox.Controls.Add(this.imageSpaceLabel);
			this.imageGroupBox.Controls.Add(this.label4);
			this.imageGroupBox.Controls.Add(this.imageVerticalSizeBox);
			this.imageGroupBox.Controls.Add(this.imageHorizontalSizeBox);
			this.imageGroupBox.Controls.Add(this.imageSizeLabel);
			this.imageGroupBox.Controls.Add(this.imageFileBrowseButton);
			this.imageGroupBox.Controls.Add(this.imageFileTextBox);
			this.imageGroupBox.Controls.Add(this.imageFileLabel);
			this.imageGroupBox.Location = new System.Drawing.Point(12, 80);
			this.imageGroupBox.Name = "imageGroupBox";
			this.imageGroupBox.Size = new System.Drawing.Size(296, 120);
			this.imageGroupBox.TabIndex = 5;
			this.imageGroupBox.TabStop = false;
			this.imageGroupBox.Text = "Image";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(160, 88);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(12, 13);
			this.label5.TabIndex = 14;
			this.label5.Text = "x";
			// 
			// imageVerticalSpaceBox
			// 
			this.imageVerticalSpaceBox.Location = new System.Drawing.Point(184, 84);
			this.imageVerticalSpaceBox.Name = "imageVerticalSpaceBox";
			this.imageVerticalSpaceBox.Size = new System.Drawing.Size(64, 20);
			this.imageVerticalSpaceBox.TabIndex = 13;
			this.imageVerticalSpaceBox.ValueChanged += new System.EventHandler(this.imageVerticalSpaceBox_ValueChanged);
			// 
			// imageHorizontalSpaceBox
			// 
			this.imageHorizontalSpaceBox.Location = new System.Drawing.Point(84, 84);
			this.imageHorizontalSpaceBox.Name = "imageHorizontalSpaceBox";
			this.imageHorizontalSpaceBox.Size = new System.Drawing.Size(64, 20);
			this.imageHorizontalSpaceBox.TabIndex = 12;
			this.imageHorizontalSpaceBox.ValueChanged += new System.EventHandler(this.imageHorizontalSpaceBox_ValueChanged);
			// 
			// imageSpaceLabel
			// 
			this.imageSpaceLabel.AutoSize = true;
			this.imageSpaceLabel.Location = new System.Drawing.Point(8, 88);
			this.imageSpaceLabel.Name = "imageSpaceLabel";
			this.imageSpaceLabel.Size = new System.Drawing.Size(66, 13);
			this.imageSpaceLabel.TabIndex = 11;
			this.imageSpaceLabel.Text = "Tile Spacing";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(160, 56);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(12, 13);
			this.label4.TabIndex = 10;
			this.label4.Text = "x";
			// 
			// imageVerticalSizeBox
			// 
			this.imageVerticalSizeBox.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
			this.imageVerticalSizeBox.Location = new System.Drawing.Point(184, 52);
			this.imageVerticalSizeBox.Name = "imageVerticalSizeBox";
			this.imageVerticalSizeBox.Size = new System.Drawing.Size(64, 20);
			this.imageVerticalSizeBox.TabIndex = 9;
			this.imageVerticalSizeBox.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
			this.imageVerticalSizeBox.ValueChanged += new System.EventHandler(this.imageVerticalSizeBox_ValueChanged);
			// 
			// imageHorizontalSizeBox
			// 
			this.imageHorizontalSizeBox.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
			this.imageHorizontalSizeBox.Location = new System.Drawing.Point(84, 52);
			this.imageHorizontalSizeBox.Name = "imageHorizontalSizeBox";
			this.imageHorizontalSizeBox.Size = new System.Drawing.Size(64, 20);
			this.imageHorizontalSizeBox.TabIndex = 8;
			this.imageHorizontalSizeBox.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
			this.imageHorizontalSizeBox.ValueChanged += new System.EventHandler(this.imageHorizontalSizeBox_ValueChanged);
			// 
			// imageSizeLabel
			// 
			this.imageSizeLabel.AutoSize = true;
			this.imageSizeLabel.Location = new System.Drawing.Point(28, 56);
			this.imageSizeLabel.Name = "imageSizeLabel";
			this.imageSizeLabel.Size = new System.Drawing.Size(47, 13);
			this.imageSizeLabel.TabIndex = 7;
			this.imageSizeLabel.Text = "Tile Size";
			// 
			// imageFileBrowseButton
			// 
			this.imageFileBrowseButton.Location = new System.Drawing.Point(256, 20);
			this.imageFileBrowseButton.Name = "imageFileBrowseButton";
			this.imageFileBrowseButton.Size = new System.Drawing.Size(24, 19);
			this.imageFileBrowseButton.TabIndex = 6;
			this.imageFileBrowseButton.Text = "...";
			this.imageFileBrowseButton.UseVisualStyleBackColor = true;
			this.imageFileBrowseButton.Click += new System.EventHandler(this.imageFileBrowseButton_Click);
			// 
			// imageFileTextBox
			// 
			this.imageFileTextBox.Location = new System.Drawing.Point(84, 20);
			this.imageFileTextBox.Name = "imageFileTextBox";
			this.imageFileTextBox.ReadOnly = true;
			this.imageFileTextBox.Size = new System.Drawing.Size(164, 20);
			this.imageFileTextBox.TabIndex = 5;
			// 
			// imageFileLabel
			// 
			this.imageFileLabel.AutoSize = true;
			this.imageFileLabel.Location = new System.Drawing.Point(52, 24);
			this.imageFileLabel.Name = "imageFileLabel";
			this.imageFileLabel.Size = new System.Drawing.Size(23, 13);
			this.imageFileLabel.TabIndex = 4;
			this.imageFileLabel.Text = "File";
			// 
			// NewTilesetWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(414, 217);
			this.Controls.Add(this.imageGroupBox);
			this.Controls.Add(this.infoGroupBox);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NewTilesetWindow";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "New Tileset";
			this.infoGroupBox.ResumeLayout(false);
			this.infoGroupBox.PerformLayout();
			this.imageGroupBox.ResumeLayout(false);
			this.imageGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.imageVerticalSpaceBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imageHorizontalSpaceBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imageVerticalSizeBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imageHorizontalSizeBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Label tilesetFileLabel;
		private System.Windows.Forms.TextBox tilesetFileTextBox;
		private System.Windows.Forms.GroupBox infoGroupBox;
		private System.Windows.Forms.GroupBox imageGroupBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown imageVerticalSpaceBox;
		private System.Windows.Forms.NumericUpDown imageHorizontalSpaceBox;
		private System.Windows.Forms.Label imageSpaceLabel;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown imageVerticalSizeBox;
		private System.Windows.Forms.NumericUpDown imageHorizontalSizeBox;
		private System.Windows.Forms.Label imageSizeLabel;
		private System.Windows.Forms.Button imageFileBrowseButton;
		private System.Windows.Forms.TextBox imageFileTextBox;
		private System.Windows.Forms.Label imageFileLabel;
		private System.Windows.Forms.Button tilesetFileBrowseButton;
	}
}