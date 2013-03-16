namespace BinaryPhoenix.Fusion.Editor.Windows
{
	partial class ConsoleWindow
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConsoleWindow));
			this.logListView = new System.Windows.Forms.ListView();
			this.warningColumn = new System.Windows.Forms.ColumnHeader();
			this.timeColumn = new System.Windows.Forms.ColumnHeader();
			this.messageColumn = new System.Windows.Forms.ColumnHeader();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.commandTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// logListView
			// 
			this.logListView.Alignment = System.Windows.Forms.ListViewAlignment.Default;
			this.logListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.logListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.warningColumn,
            this.timeColumn,
            this.messageColumn});
			this.logListView.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.logListView.FullRowSelect = true;
			this.logListView.LargeImageList = this.imageList;
			this.logListView.Location = new System.Drawing.Point(4, 4);
			this.logListView.Name = "logListView";
			this.logListView.Size = new System.Drawing.Size(480, 256);
			this.logListView.SmallImageList = this.imageList;
			this.logListView.StateImageList = this.imageList;
			this.logListView.TabIndex = 1;
			this.logListView.UseCompatibleStateImageBehavior = false;
			this.logListView.View = System.Windows.Forms.View.Details;
			// 
			// warningColumn
			// 
			this.warningColumn.Text = "";
			this.warningColumn.Width = 21;
			// 
			// timeColumn
			// 
			this.timeColumn.Text = "Time";
			this.timeColumn.Width = 109;
			// 
			// messageColumn
			// 
			this.messageColumn.Text = "Message";
			this.messageColumn.Width = 327;
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "message.png");
			this.imageList.Images.SetKeyName(1, "warning.png");
			this.imageList.Images.SetKeyName(2, "error.png");
			// 
			// commandTextBox
			// 
			this.commandTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.commandTextBox.Location = new System.Drawing.Point(4, 265);
			this.commandTextBox.Multiline = true;
			this.commandTextBox.Name = "commandTextBox";
			this.commandTextBox.Size = new System.Drawing.Size(480, 20);
			this.commandTextBox.TabIndex = 3;
			this.commandTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.commandTextBox_KeyPress);
			// 
			// ConsoleWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(489, 291);
			this.Controls.Add(this.logListView);
			this.Controls.Add(this.commandTextBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConsoleWindow";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Console";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConsoleWindow_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView logListView;
		private System.Windows.Forms.ColumnHeader warningColumn;
		private System.Windows.Forms.ColumnHeader timeColumn;
		private System.Windows.Forms.ColumnHeader messageColumn;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.TextBox commandTextBox;

	}
}