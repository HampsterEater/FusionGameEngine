namespace BinaryPhoenix.Fusion.Editor.Windows
{
    partial class EmitterEditorWindow
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
            this.typeListBox = new System.Windows.Forms.ListBox();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.addModButton = new System.Windows.Forms.Button();
            this.modifierTypeComboBox = new System.Windows.Forms.ComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newEffectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.importEffectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportEffectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetEmitterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.viewBoundingBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewOriginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.previewPanel = new System.Windows.Forms.Panel();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.resetCameraToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.resetEmitterToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.viewBoundingBoxToolStripContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewOriginToolStripContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.deleteButton = new System.Windows.Forms.Button();
            this.newTypeButton = new System.Windows.Forms.Button();
            this.fpsStatusStrip = new System.Windows.Forms.StatusStrip();
            this.fpsToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.particleCountToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.fpsStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // typeListBox
            // 
            this.typeListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.typeListBox.FormattingEnabled = true;
            this.typeListBox.Location = new System.Drawing.Point(3, 3);
            this.typeListBox.Name = "typeListBox";
            this.typeListBox.ScrollAlwaysVisible = true;
            this.typeListBox.Size = new System.Drawing.Size(182, 82);
            this.typeListBox.TabIndex = 2;
            this.typeListBox.SelectedIndexChanged += new System.EventHandler(this.typeListBox_SelectedIndexChanged);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(187, 308);
            this.propertyGrid.TabIndex = 5;
            // 
            // addModButton
            // 
            this.addModButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addModButton.Enabled = false;
            this.addModButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.tick;
            this.addModButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addModButton.Location = new System.Drawing.Point(92, 125);
            this.addModButton.Name = "addModButton";
            this.addModButton.Size = new System.Drawing.Size(92, 23);
            this.addModButton.TabIndex = 8;
            this.addModButton.Text = "Add Modifier";
            this.addModButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.addModButton.UseVisualStyleBackColor = true;
            this.addModButton.Click += new System.EventHandler(this.addModButton_Click);
            // 
            // modifierTypeComboBox
            // 
            this.modifierTypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.modifierTypeComboBox.FormattingEnabled = true;
            this.modifierTypeComboBox.Items.AddRange(new object[] {
            "Acceleration",
            "Color",
            "Scale",
            "Rotation",
            "Render State",
            "Animation"});
            this.modifierTypeComboBox.Location = new System.Drawing.Point(4, 127);
            this.modifierTypeComboBox.Name = "modifierTypeComboBox";
            this.modifierTypeComboBox.Size = new System.Drawing.Size(82, 21);
            this.modifierTypeComboBox.TabIndex = 9;
            this.modifierTypeComboBox.Text = "Acceleration";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(661, 24);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newEffectToolStripMenuItem,
            this.toolStripSeparator3,
            this.importEffectToolStripMenuItem,
            this.exportEffectToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newEffectToolStripMenuItem
            // 
            this.newEffectToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.emitter_designer;
            this.newEffectToolStripMenuItem.Name = "newEffectToolStripMenuItem";
            this.newEffectToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.newEffectToolStripMenuItem.Text = "New Effect";
            this.newEffectToolStripMenuItem.Click += new System.EventHandler(this.newEffectToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(135, 6);
            // 
            // importEffectToolStripMenuItem
            // 
            this.importEffectToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.open;
            this.importEffectToolStripMenuItem.Name = "importEffectToolStripMenuItem";
            this.importEffectToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.importEffectToolStripMenuItem.Text = "Import Effect";
            this.importEffectToolStripMenuItem.Click += new System.EventHandler(this.importEffectToolStripMenuItem_Click);
            // 
            // exportEffectToolStripMenuItem
            // 
            this.exportEffectToolStripMenuItem.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.save;
            this.exportEffectToolStripMenuItem.Name = "exportEffectToolStripMenuItem";
            this.exportEffectToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.exportEffectToolStripMenuItem.Text = "Export Effect";
            this.exportEffectToolStripMenuItem.Click += new System.EventHandler(this.exportEffectToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(135, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.exitToolStripMenuItem.Text = "Close";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetEmitterToolStripMenuItem,
            this.resetCameraToolStripMenuItem,
            this.toolStripSeparator2,
            this.viewBoundingBoxToolStripMenuItem,
            this.viewOriginToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // resetEmitterToolStripMenuItem
            // 
            this.resetEmitterToolStripMenuItem.Name = "resetEmitterToolStripMenuItem";
            this.resetEmitterToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.resetEmitterToolStripMenuItem.Text = "Reset Emitter";
            this.resetEmitterToolStripMenuItem.Click += new System.EventHandler(this.resetEmitterToolStripMenuItem_Click);
            // 
            // resetCameraToolStripMenuItem
            // 
            this.resetCameraToolStripMenuItem.Name = "resetCameraToolStripMenuItem";
            this.resetCameraToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.resetCameraToolStripMenuItem.Text = "Reset Camera";
            this.resetCameraToolStripMenuItem.Click += new System.EventHandler(this.resetCameraToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(161, 6);
            // 
            // viewBoundingBoxToolStripMenuItem
            // 
            this.viewBoundingBoxToolStripMenuItem.Checked = true;
            this.viewBoundingBoxToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewBoundingBoxToolStripMenuItem.Name = "viewBoundingBoxToolStripMenuItem";
            this.viewBoundingBoxToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.viewBoundingBoxToolStripMenuItem.Text = "View Bounding Box";
            this.viewBoundingBoxToolStripMenuItem.Click += new System.EventHandler(this.viewBoundingBoxToolStripMenuItem_Click);
            // 
            // viewOriginToolStripMenuItem
            // 
            this.viewOriginToolStripMenuItem.Checked = true;
            this.viewOriginToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewOriginToolStripMenuItem.Name = "viewOriginToolStripMenuItem";
            this.viewOriginToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.viewOriginToolStripMenuItem.Text = "View Origin";
            this.viewOriginToolStripMenuItem.Click += new System.EventHandler(this.viewOriginToolStripMenuItem_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 24);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.previewPanel);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainer2.Size = new System.Drawing.Size(661, 464);
            this.splitContainer2.SplitterDistance = 472;
            this.splitContainer2.SplitterWidth = 2;
            this.splitContainer2.TabIndex = 14;
            // 
            // previewPanel
            // 
            this.previewPanel.BackColor = System.Drawing.Color.Black;
            this.previewPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.previewPanel.ContextMenuStrip = this.contextMenuStrip;
            this.previewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewPanel.Location = new System.Drawing.Point(0, 0);
            this.previewPanel.Name = "previewPanel";
            this.previewPanel.Size = new System.Drawing.Size(472, 464);
            this.previewPanel.TabIndex = 15;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetCameraToolStripMenuItem1,
            this.resetEmitterToolStripMenuItem1,
            this.toolStripSeparator4,
            this.viewBoundingBoxToolStripContextMenuItem,
            this.viewOriginToolStripContextMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(165, 98);
            // 
            // resetCameraToolStripMenuItem1
            // 
            this.resetCameraToolStripMenuItem1.Name = "resetCameraToolStripMenuItem1";
            this.resetCameraToolStripMenuItem1.Size = new System.Drawing.Size(164, 22);
            this.resetCameraToolStripMenuItem1.Text = "Reset Camera";
            this.resetCameraToolStripMenuItem1.Click += new System.EventHandler(this.resetCameraToolStripMenuItem_Click);
            // 
            // resetEmitterToolStripMenuItem1
            // 
            this.resetEmitterToolStripMenuItem1.Name = "resetEmitterToolStripMenuItem1";
            this.resetEmitterToolStripMenuItem1.Size = new System.Drawing.Size(164, 22);
            this.resetEmitterToolStripMenuItem1.Text = "Reset Emitter";
            this.resetEmitterToolStripMenuItem1.Click += new System.EventHandler(this.resetEmitterToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(161, 6);
            // 
            // viewBoundingBoxToolStripContextMenuItem
            // 
            this.viewBoundingBoxToolStripContextMenuItem.Checked = true;
            this.viewBoundingBoxToolStripContextMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewBoundingBoxToolStripContextMenuItem.Name = "viewBoundingBoxToolStripContextMenuItem";
            this.viewBoundingBoxToolStripContextMenuItem.Size = new System.Drawing.Size(164, 22);
            this.viewBoundingBoxToolStripContextMenuItem.Text = "View Bounding Box";
            this.viewBoundingBoxToolStripContextMenuItem.Click += new System.EventHandler(this.viewBoundingBoxToolStripMenuItem_Click);
            // 
            // viewOriginToolStripContextMenuItem
            // 
            this.viewOriginToolStripContextMenuItem.Checked = true;
            this.viewOriginToolStripContextMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewOriginToolStripContextMenuItem.Name = "viewOriginToolStripContextMenuItem";
            this.viewOriginToolStripContextMenuItem.Size = new System.Drawing.Size(164, 22);
            this.viewOriginToolStripContextMenuItem.Text = "View Origin";
            this.viewOriginToolStripContextMenuItem.Click += new System.EventHandler(this.viewOriginToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.deleteButton);
            this.splitContainer1.Panel1.Controls.Add(this.typeListBox);
            this.splitContainer1.Panel1.Controls.Add(this.modifierTypeComboBox);
            this.splitContainer1.Panel1.Controls.Add(this.addModButton);
            this.splitContainer1.Panel1.Controls.Add(this.newTypeButton);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid);
            this.splitContainer1.Size = new System.Drawing.Size(187, 464);
            this.splitContainer1.SplitterDistance = 152;
            this.splitContainer1.TabIndex = 6;
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.delete;
            this.deleteButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.deleteButton.Location = new System.Drawing.Point(4, 96);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(82, 23);
            this.deleteButton.TabIndex = 10;
            this.deleteButton.Text = "Delete";
            this.deleteButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // newTypeButton
            // 
            this.newTypeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.newTypeButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.tick;
            this.newTypeButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.newTypeButton.Location = new System.Drawing.Point(92, 96);
            this.newTypeButton.Name = "newTypeButton";
            this.newTypeButton.Size = new System.Drawing.Size(92, 23);
            this.newTypeButton.TabIndex = 3;
            this.newTypeButton.Text = "New Type";
            this.newTypeButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.newTypeButton.UseVisualStyleBackColor = true;
            this.newTypeButton.Click += new System.EventHandler(this.newTypeButton_Click);
            // 
            // fpsStatusStrip
            // 
            this.fpsStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fpsToolStripStatusLabel,
            this.particleCountToolStripStatusLabel});
            this.fpsStatusStrip.Location = new System.Drawing.Point(0, 466);
            this.fpsStatusStrip.Name = "fpsStatusStrip";
            this.fpsStatusStrip.Size = new System.Drawing.Size(661, 22);
            this.fpsStatusStrip.TabIndex = 15;
            this.fpsStatusStrip.Text = "FPS: 60";
            // 
            // fpsToolStripStatusLabel
            // 
            this.fpsToolStripStatusLabel.Name = "fpsToolStripStatusLabel";
            this.fpsToolStripStatusLabel.Size = new System.Drawing.Size(44, 17);
            this.fpsToolStripStatusLabel.Text = "FPS: 60";
            // 
            // particleCountToolStripStatusLabel
            // 
            this.particleCountToolStripStatusLabel.Name = "particleCountToolStripStatusLabel";
            this.particleCountToolStripStatusLabel.Size = new System.Drawing.Size(94, 17);
            this.particleCountToolStripStatusLabel.Text = "Particles: 500/500";
            // 
            // EmitterEditorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(661, 488);
            this.Controls.Add(this.fpsStatusStrip);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.menuStrip1);
            this.MinimizeBox = false;
            this.Name = "EmitterEditorWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Effect Designer";
            this.VisibleChanged += new System.EventHandler(this.EmitterEditorWindow_VisibleChanged);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EmitterEditorWindow_FormClosed);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.contextMenuStrip.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.fpsStatusStrip.ResumeLayout(false);
            this.fpsStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox typeListBox;
        private System.Windows.Forms.Button newTypeButton;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Button addModButton;
        private System.Windows.Forms.ComboBox modifierTypeComboBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importEffectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportEffectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ToolStripMenuItem resetEmitterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetCameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem viewBoundingBoxToolStripMenuItem;
        private System.Windows.Forms.Panel previewPanel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.ToolStripMenuItem newEffectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.StatusStrip fpsStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel fpsToolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel particleCountToolStripStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem viewOriginToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem resetCameraToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem resetEmitterToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem viewBoundingBoxToolStripContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewOriginToolStripContextMenuItem;
    }
}