namespace BinaryPhoenix.Fusion.Editor.Windows
{
	partial class ScriptEditorWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptEditorWindow));
            this.errorImageList = new System.Windows.Forms.ImageList(this.components);
            this.scriptBoxContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.undoContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.cutContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptToolStrip = new System.Windows.Forms.ToolStrip();
            this.cutToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.copyToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.pasteToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.undoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.redoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.findToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.checkSyntaxToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.scriptTextBox = new BinaryPhoenix.Fusion.Runtime.Controls.ScriptTextBox();
            this.scriptBoxContextMenuStrip.SuspendLayout();
            this.scriptToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // errorImageList
            // 
            this.errorImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("errorImageList.ImageStream")));
            this.errorImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.errorImageList.Images.SetKeyName(0, "message.png");
            this.errorImageList.Images.SetKeyName(1, "warning.png");
            this.errorImageList.Images.SetKeyName(2, "error.png");
            // 
            // scriptBoxContextMenuStrip
            // 
            this.scriptBoxContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoContextMenuItem,
            this.redoContextMenuItem,
            this.toolStripSeparator4,
            this.cutContextMenuItem,
            this.copyContextMenuItem,
            this.pasteContextMenuItem});
            this.scriptBoxContextMenuStrip.Name = "scriptBoxContextMenuStrip";
            this.scriptBoxContextMenuStrip.Size = new System.Drawing.Size(104, 120);
            // 
            // undoContextMenuItem
            // 
            this.undoContextMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.undo;
            this.undoContextMenuItem.Name = "undoContextMenuItem";
            this.undoContextMenuItem.Size = new System.Drawing.Size(103, 22);
            this.undoContextMenuItem.Text = "Undo";
            this.undoContextMenuItem.Click += new System.EventHandler(this.undoToolStripButton_Click);
            // 
            // redoContextMenuItem
            // 
            this.redoContextMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.redo;
            this.redoContextMenuItem.Name = "redoContextMenuItem";
            this.redoContextMenuItem.Size = new System.Drawing.Size(103, 22);
            this.redoContextMenuItem.Text = "Redo";
            this.redoContextMenuItem.Click += new System.EventHandler(this.redoToolStripButton_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(100, 6);
            // 
            // cutContextMenuItem
            // 
            this.cutContextMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.cut;
            this.cutContextMenuItem.Name = "cutContextMenuItem";
            this.cutContextMenuItem.Size = new System.Drawing.Size(103, 22);
            this.cutContextMenuItem.Text = "Cut";
            this.cutContextMenuItem.Click += new System.EventHandler(this.cutToolStripButton_Click);
            // 
            // copyContextMenuItem
            // 
            this.copyContextMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.copy;
            this.copyContextMenuItem.Name = "copyContextMenuItem";
            this.copyContextMenuItem.Size = new System.Drawing.Size(103, 22);
            this.copyContextMenuItem.Text = "Copy";
            this.copyContextMenuItem.Click += new System.EventHandler(this.copyToolStripButton_Click);
            // 
            // pasteContextMenuItem
            // 
            this.pasteContextMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.paste;
            this.pasteContextMenuItem.Name = "pasteContextMenuItem";
            this.pasteContextMenuItem.Size = new System.Drawing.Size(103, 22);
            this.pasteContextMenuItem.Text = "Paste";
            this.pasteContextMenuItem.Click += new System.EventHandler(this.pasteToolStripButton_Click);
            // 
            // scriptToolStrip
            // 
            this.scriptToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripButton,
            this.copyToolStripButton,
            this.pasteToolStripButton,
            this.toolStripSeparator1,
            this.undoToolStripButton,
            this.redoToolStripButton,
            this.toolStripSeparator2,
            this.findToolStripButton,
            this.toolStripSeparator3,
            this.checkSyntaxToolStripButton});
            this.scriptToolStrip.Location = new System.Drawing.Point(0, 0);
            this.scriptToolStrip.Name = "scriptToolStrip";
            this.scriptToolStrip.Size = new System.Drawing.Size(486, 25);
            this.scriptToolStrip.TabIndex = 6;
            this.scriptToolStrip.Text = "toolStrip1";
            // 
            // cutToolStripButton
            // 
            this.cutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cutToolStripButton.Enabled = false;
            this.cutToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.cut;
            this.cutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cutToolStripButton.Name = "cutToolStripButton";
            this.cutToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.cutToolStripButton.Text = "Cut";
            this.cutToolStripButton.Click += new System.EventHandler(this.cutToolStripButton_Click);
            // 
            // copyToolStripButton
            // 
            this.copyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copyToolStripButton.Enabled = false;
            this.copyToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.copy;
            this.copyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyToolStripButton.Name = "copyToolStripButton";
            this.copyToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.copyToolStripButton.Text = "Copy";
            this.copyToolStripButton.Click += new System.EventHandler(this.copyToolStripButton_Click);
            // 
            // pasteToolStripButton
            // 
            this.pasteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pasteToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.paste;
            this.pasteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pasteToolStripButton.Name = "pasteToolStripButton";
            this.pasteToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.pasteToolStripButton.Text = "Paste";
            this.pasteToolStripButton.Click += new System.EventHandler(this.pasteToolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // undoToolStripButton
            // 
            this.undoToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.undoToolStripButton.Enabled = false;
            this.undoToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.undo;
            this.undoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.undoToolStripButton.Name = "undoToolStripButton";
            this.undoToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.undoToolStripButton.Text = "Undo";
            this.undoToolStripButton.Click += new System.EventHandler(this.undoToolStripButton_Click);
            // 
            // redoToolStripButton
            // 
            this.redoToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.redoToolStripButton.Enabled = false;
            this.redoToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.redo;
            this.redoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.redoToolStripButton.Name = "redoToolStripButton";
            this.redoToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.redoToolStripButton.Text = "Redo";
            this.redoToolStripButton.Click += new System.EventHandler(this.redoToolStripButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // findToolStripButton
            // 
            this.findToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.findToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.find;
            this.findToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.findToolStripButton.Name = "findToolStripButton";
            this.findToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.findToolStripButton.Text = "Find And Replace";
            this.findToolStripButton.Click += new System.EventHandler(this.findToolStripButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // checkSyntaxToolStripButton
            // 
            this.checkSyntaxToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.checkSyntaxToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.spellcheck;
            this.checkSyntaxToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.checkSyntaxToolStripButton.Name = "checkSyntaxToolStripButton";
            this.checkSyntaxToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.checkSyntaxToolStripButton.Text = "Check For Errors";
            this.checkSyntaxToolStripButton.Click += new System.EventHandler(this.checkSyntaxToolStripButton_Click);
            // 
            // scriptTextBox
            // 
            this.scriptTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.scriptTextBox.ContextMenuStrip = this.scriptBoxContextMenuStrip;
            this.scriptTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scriptTextBox.Location = new System.Drawing.Point(0, 25);
            this.scriptTextBox.Name = "scriptTextBox";
            this.scriptTextBox.ReadOnly = false;
            this.scriptTextBox.RequireHighlighting = true;
            this.scriptTextBox.Size = new System.Drawing.Size(486, 402);
            this.scriptTextBox.TabIndex = 5;
            // 
            // ScriptEditorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 427);
            this.Controls.Add(this.scriptTextBox);
            this.Controls.Add(this.scriptToolStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ScriptEditorWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Script Editor";
            this.Shown += new System.EventHandler(this.ScriptEditorWindow_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScriptEditorWindow_FormClosing);
            this.scriptBoxContextMenuStrip.ResumeLayout(false);
            this.scriptToolStrip.ResumeLayout(false);
            this.scriptToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.ImageList errorImageList;
		private System.Windows.Forms.ContextMenuStrip scriptBoxContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem undoContextMenuItem;
		private System.Windows.Forms.ToolStripMenuItem redoContextMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem cutContextMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteContextMenuItem;
        private BinaryPhoenix.Fusion.Runtime.Controls.ScriptTextBox scriptTextBox;
        private System.Windows.Forms.ToolStrip scriptToolStrip;
        private System.Windows.Forms.ToolStripButton cutToolStripButton;
        private System.Windows.Forms.ToolStripButton copyToolStripButton;
        private System.Windows.Forms.ToolStripButton pasteToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton undoToolStripButton;
        private System.Windows.Forms.ToolStripButton redoToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton findToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton checkSyntaxToolStripButton;
	}
}