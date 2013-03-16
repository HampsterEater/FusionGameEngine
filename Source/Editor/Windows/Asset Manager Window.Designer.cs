namespace BinaryPhoenix.Fusion.Editor.Windows
{
	partial class AssetManagerWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssetManagerWindow));
            this.projectTreeView = new System.Windows.Forms.TreeView();
            this.projectImageList = new System.Windows.Forms.ImageList(this.components);
            this.projectToolStrip = new System.Windows.Forms.ToolStrip();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.newFileToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.newFolderToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.exportToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.importToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.renameToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.duplicateToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.fileContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.newFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToolStrip.SuspendLayout();
            this.fileContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // projectTreeView
            // 
            this.projectTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectTreeView.FullRowSelect = true;
            this.projectTreeView.ImageIndex = 0;
            this.projectTreeView.ImageList = this.projectImageList;
            this.projectTreeView.LabelEdit = true;
            this.projectTreeView.Location = new System.Drawing.Point(0, 25);
            this.projectTreeView.Name = "projectTreeView";
            this.projectTreeView.SelectedImageIndex = 0;
            this.projectTreeView.Size = new System.Drawing.Size(279, 360);
            this.projectTreeView.StateImageList = this.projectImageList;
            this.projectTreeView.TabIndex = 0;
            this.projectTreeView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.projectTreeView_MouseClick);
            this.projectTreeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.projectTreeView_AfterLabelEdit);
            this.projectTreeView.DoubleClick += new System.EventHandler(this.openToolStripMenuItem_Click);
            this.projectTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.projectTreeView_AfterSelect);
            // 
            // projectImageList
            // 
            this.projectImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("projectImageList.ImageStream")));
            this.projectImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.projectImageList.Images.SetKeyName(0, "folder.png");
            this.projectImageList.Images.SetKeyName(1, "sound.png");
            this.projectImageList.Images.SetKeyName(2, "emitter.png");
            this.projectImageList.Images.SetKeyName(3, "entity.png");
            this.projectImageList.Images.SetKeyName(4, "tilemap.png");
            this.projectImageList.Images.SetKeyName(5, "compress.png");
            this.projectImageList.Images.SetKeyName(6, "page_white.png");
            this.projectImageList.Images.SetKeyName(7, "page_white_code_red.png");
            this.projectImageList.Images.SetKeyName(8, "page_white_gear.png");
            this.projectImageList.Images.SetKeyName(9, "image.png");
            // 
            // projectToolStrip
            // 
            this.projectToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripButton,
            this.toolStripSeparator2,
            this.newFileToolStripButton,
            this.newFolderToolStripButton,
            this.toolStripSeparator5,
            this.exportToolStripButton,
            this.importToolStripButton,
            this.toolStripSeparator6,
            this.deleteToolStripButton,
            this.renameToolStripButton,
            this.duplicateToolStripButton});
            this.projectToolStrip.Location = new System.Drawing.Point(0, 0);
            this.projectToolStrip.Name = "projectToolStrip";
            this.projectToolStrip.Size = new System.Drawing.Size(279, 25);
            this.projectToolStrip.TabIndex = 1;
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.open;
            this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openToolStripButton.Text = "Open";
            this.openToolStripButton.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // newFileToolStripButton
            // 
            this.newFileToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newFileToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.page_white_add;
            this.newFileToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newFileToolStripButton.Name = "newFileToolStripButton";
            this.newFileToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.newFileToolStripButton.Text = "Create File";
            this.newFileToolStripButton.Click += new System.EventHandler(this.newFileToolStripMenuItem_Click);
            // 
            // newFolderToolStripButton
            // 
            this.newFolderToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newFolderToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.folder_add;
            this.newFolderToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newFolderToolStripButton.Name = "newFolderToolStripButton";
            this.newFolderToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.newFolderToolStripButton.Text = "Create Folder";
            this.newFolderToolStripButton.Click += new System.EventHandler(this.newFolderToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // exportToolStripButton
            // 
            this.exportToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.exportToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.page_white_go;
            this.exportToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.exportToolStripButton.Name = "exportToolStripButton";
            this.exportToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.exportToolStripButton.Text = "Export";
            this.exportToolStripButton.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // importToolStripButton
            // 
            this.importToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.importToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.page_white_get;
            this.importToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.importToolStripButton.Name = "importToolStripButton";
            this.importToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.importToolStripButton.Text = "Import";
            this.importToolStripButton.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // deleteToolStripButton
            // 
            this.deleteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.delete;
            this.deleteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteToolStripButton.Name = "deleteToolStripButton";
            this.deleteToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.deleteToolStripButton.Text = "Delete";
            this.deleteToolStripButton.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // renameToolStripButton
            // 
            this.renameToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.renameToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.page_white_edit;
            this.renameToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.renameToolStripButton.Name = "renameToolStripButton";
            this.renameToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.renameToolStripButton.Text = "Rename";
            this.renameToolStripButton.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // duplicateToolStripButton
            // 
            this.duplicateToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.duplicateToolStripButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.page_white_copy;
            this.duplicateToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.duplicateToolStripButton.Name = "duplicateToolStripButton";
            this.duplicateToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.duplicateToolStripButton.Text = "Duplicate";
            this.duplicateToolStripButton.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
            // 
            // fileContextMenuStrip
            // 
            this.fileContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripSeparator4,
            this.newFileToolStripMenuItem,
            this.newFolderToolStripMenuItem,
            this.toolStripSeparator1,
            this.exportToolStripMenuItem,
            this.importToolStripMenuItem,
            this.toolStripSeparator3,
            this.deleteToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.duplicateToolStripMenuItem});
            this.fileContextMenuStrip.Name = "fileContextMenuStrip";
            this.fileContextMenuStrip.Size = new System.Drawing.Size(153, 220);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.open;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "Open ...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(149, 6);
            // 
            // newFileToolStripMenuItem
            // 
            this.newFileToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.page_white_add;
            this.newFileToolStripMenuItem.Name = "newFileToolStripMenuItem";
            this.newFileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newFileToolStripMenuItem.Text = "New File";
            this.newFileToolStripMenuItem.Click += new System.EventHandler(this.newFileToolStripMenuItem_Click);
            // 
            // newFolderToolStripMenuItem
            // 
            this.newFolderToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.folder_add;
            this.newFolderToolStripMenuItem.Name = "newFolderToolStripMenuItem";
            this.newFolderToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newFolderToolStripMenuItem.Text = "New Folder";
            this.newFolderToolStripMenuItem.Click += new System.EventHandler(this.newFolderToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.page_white_go;
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exportToolStripMenuItem.Text = "Export ...";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.page_white_get;
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.importToolStripMenuItem.Text = "Import ...";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(149, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.delete;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.page_white_edit;
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // duplicateToolStripMenuItem
            // 
            this.duplicateToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.page_white_copy;
            this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
            this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.duplicateToolStripMenuItem.Text = "Duplicate";
            this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
            // 
            // AssetManagerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(279, 385);
            this.Controls.Add(this.projectTreeView);
            this.Controls.Add(this.projectToolStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "AssetManagerWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Asset Manager";
            this.VisibleChanged += new System.EventHandler(this.AssetManagerWindow_VisibleChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AssetManagerWindow_FormClosing);
            this.projectToolStrip.ResumeLayout(false);
            this.projectToolStrip.PerformLayout();
            this.fileContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.TreeView projectTreeView;
        private System.Windows.Forms.ImageList projectImageList;
        private System.Windows.Forms.ToolStrip projectToolStrip;
        private System.Windows.Forms.ContextMenuStrip fileContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem newFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton openToolStripButton;
        private System.Windows.Forms.ToolStripButton newFileToolStripButton;
        private System.Windows.Forms.ToolStripButton newFolderToolStripButton;
        private System.Windows.Forms.ToolStripButton exportToolStripButton;
        private System.Windows.Forms.ToolStripButton importToolStripButton;
        private System.Windows.Forms.ToolStripButton deleteToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem duplicateToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripButton renameToolStripButton;
        private System.Windows.Forms.ToolStripButton duplicateToolStripButton;
	}
}