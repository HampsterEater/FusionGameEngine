namespace BinaryPhoenix.Fusion.Engine.Windows
{
    partial class ImageSelectorWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageSelectorWindow));
            this.previewPanel = new System.Windows.Forms.Panel();
            this.fileTreeView = new System.Windows.Forms.TreeView();
            this.fileImageList = new System.Windows.Forms.ImageList(this.components);
            this.loadButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.maskColorPanel = new System.Windows.Forms.Panel();
            this.cellWidthBox = new System.Windows.Forms.NumericUpDown();
            this.cellHeightBox = new System.Windows.Forms.NumericUpDown();
            this.cellSpaceYBox = new System.Windows.Forms.NumericUpDown();
            this.cellSpaceXBox = new System.Windows.Forms.NumericUpDown();
            this.panel3 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.cellWidthBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cellHeightBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cellSpaceYBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cellSpaceXBox)).BeginInit();
            this.SuspendLayout();
            // 
            // previewPanel
            // 
            this.previewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.previewPanel.BackColor = System.Drawing.Color.LightGray;
            this.previewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.previewPanel.Location = new System.Drawing.Point(244, 12);
            this.previewPanel.Name = "previewPanel";
            this.previewPanel.Size = new System.Drawing.Size(319, 298);
            this.previewPanel.TabIndex = 0;
            // 
            // fileTreeView
            // 
            this.fileTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.fileTreeView.ImageIndex = 0;
            this.fileTreeView.ImageList = this.fileImageList;
            this.fileTreeView.Location = new System.Drawing.Point(12, 12);
            this.fileTreeView.Name = "fileTreeView";
            this.fileTreeView.SelectedImageIndex = 0;
            this.fileTreeView.Size = new System.Drawing.Size(217, 210);
            this.fileTreeView.TabIndex = 1;
            this.fileTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.fileTreeView_AfterSelect);
            // 
            // fileImageList
            // 
            this.fileImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("fileImageList.ImageStream")));
            this.fileImageList.TransparentColor = System.Drawing.Color.Fuchsia;
            this.fileImageList.Images.SetKeyName(0, "folder.png");
            this.fileImageList.Images.SetKeyName(1, "image.png");
            // 
            // loadButton
            // 
            this.loadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.loadButton.Location = new System.Drawing.Point(488, 332);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(75, 23);
            this.loadButton.TabIndex = 2;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(397, 332);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 237);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Cell Size";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 264);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Cell Spacing";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 290);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Mask Color";
            // 
            // maskColorPanel
            // 
            this.maskColorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.maskColorPanel.BackColor = System.Drawing.Color.Fuchsia;
            this.maskColorPanel.Location = new System.Drawing.Point(96, 288);
            this.maskColorPanel.Name = "maskColorPanel";
            this.maskColorPanel.Size = new System.Drawing.Size(133, 20);
            this.maskColorPanel.TabIndex = 7;
            this.maskColorPanel.Click += new System.EventHandler(this.maskColorPanel_Click);
            // 
            // cellWidthBox
            // 
            this.cellWidthBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cellWidthBox.Location = new System.Drawing.Point(96, 235);
            this.cellWidthBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.cellWidthBox.Name = "cellWidthBox";
            this.cellWidthBox.Size = new System.Drawing.Size(63, 20);
            this.cellWidthBox.TabIndex = 8;
            // 
            // cellHeightBox
            // 
            this.cellHeightBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cellHeightBox.Location = new System.Drawing.Point(166, 235);
            this.cellHeightBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.cellHeightBox.Name = "cellHeightBox";
            this.cellHeightBox.Size = new System.Drawing.Size(63, 20);
            this.cellHeightBox.TabIndex = 9;
            // 
            // cellSpaceYBox
            // 
            this.cellSpaceYBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cellSpaceYBox.Location = new System.Drawing.Point(166, 262);
            this.cellSpaceYBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.cellSpaceYBox.Name = "cellSpaceYBox";
            this.cellSpaceYBox.Size = new System.Drawing.Size(63, 20);
            this.cellSpaceYBox.TabIndex = 11;
            // 
            // cellSpaceXBox
            // 
            this.cellSpaceXBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cellSpaceXBox.Location = new System.Drawing.Point(96, 262);
            this.cellSpaceXBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.cellSpaceXBox.Name = "cellSpaceXBox";
            this.cellSpaceXBox.Size = new System.Drawing.Size(63, 20);
            this.cellSpaceXBox.TabIndex = 10;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.panel3.Location = new System.Drawing.Point(12, 322);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(551, 1);
            this.panel3.TabIndex = 12;
            // 
            // ImageSelectorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(575, 367);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.cellSpaceYBox);
            this.Controls.Add(this.cellSpaceXBox);
            this.Controls.Add(this.cellHeightBox);
            this.Controls.Add(this.cellWidthBox);
            this.Controls.Add(this.maskColorPanel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.fileTreeView);
            this.Controls.Add(this.previewPanel);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImageSelectorWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Load Image";
            this.VisibleChanged += new System.EventHandler(this.ImageSelectorWindow_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.cellWidthBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cellHeightBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cellSpaceYBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cellSpaceXBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel previewPanel;
        private System.Windows.Forms.TreeView fileTreeView;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel maskColorPanel;
        private System.Windows.Forms.NumericUpDown cellWidthBox;
        private System.Windows.Forms.NumericUpDown cellHeightBox;
        private System.Windows.Forms.NumericUpDown cellSpaceYBox;
        private System.Windows.Forms.NumericUpDown cellSpaceXBox;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ImageList fileImageList;
    }
}