namespace BinaryPhoenix.Fusion.Editor.Windows
{
	partial class SceneGraphWindow
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SceneGraphWindow));
			this.sceneGraphTreeView = new System.Windows.Forms.TreeView();
			this.sceneNodeImageList = new System.Windows.Forms.ImageList(this.components);
			this.downButton = new System.Windows.Forms.Button();
			this.rightButton = new System.Windows.Forms.Button();
			this.leftButton = new System.Windows.Forms.Button();
			this.upButton = new System.Windows.Forms.Button();
			this.deleteButton = new System.Windows.Forms.Button();
			this.duplicateButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// sceneGraphTreeView
			// 
			this.sceneGraphTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.sceneGraphTreeView.ImageIndex = 0;
			this.sceneGraphTreeView.ImageList = this.sceneNodeImageList;
			this.sceneGraphTreeView.Location = new System.Drawing.Point(8, 40);
			this.sceneGraphTreeView.Name = "sceneGraphTreeView";
			this.sceneGraphTreeView.SelectedImageIndex = 0;
			this.sceneGraphTreeView.Size = new System.Drawing.Size(474, 244);
			this.sceneGraphTreeView.TabIndex = 0;
			this.sceneGraphTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.sceneGraphTreeView_AfterSelect);
			// 
			// sceneNodeImageList
			// 
			this.sceneNodeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("sceneNodeImageList.ImageStream")));
			this.sceneNodeImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.sceneNodeImageList.Images.SetKeyName(0, "entity.png");
			// 
			// downButton
			// 
			this.downButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.downButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.down;
			this.downButton.Location = new System.Drawing.Point(84, 8);
			this.downButton.Name = "downButton";
			this.downButton.Size = new System.Drawing.Size(26, 26);
			this.downButton.TabIndex = 3;
			this.downButton.UseVisualStyleBackColor = true;
			this.downButton.Click += new System.EventHandler(this.downButton_Click);
			// 
			// rightButton
			// 
			this.rightButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.rightButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.right1;
			this.rightButton.Location = new System.Drawing.Point(40, 8);
			this.rightButton.Name = "rightButton";
			this.rightButton.Size = new System.Drawing.Size(26, 26);
			this.rightButton.TabIndex = 5;
			this.rightButton.UseVisualStyleBackColor = true;
			this.rightButton.Click += new System.EventHandler(this.rightButton_Click);
			// 
			// leftButton
			// 
			this.leftButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.leftButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.left1;
			this.leftButton.Location = new System.Drawing.Point(8, 8);
			this.leftButton.Name = "leftButton";
			this.leftButton.Size = new System.Drawing.Size(26, 26);
			this.leftButton.TabIndex = 4;
			this.leftButton.UseVisualStyleBackColor = true;
			this.leftButton.Click += new System.EventHandler(this.leftButton_Click);
			// 
			// upButton
			// 
			this.upButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.upButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.up;
			this.upButton.Location = new System.Drawing.Point(116, 8);
			this.upButton.Name = "upButton";
			this.upButton.Size = new System.Drawing.Size(26, 26);
			this.upButton.TabIndex = 2;
			this.upButton.UseVisualStyleBackColor = true;
			this.upButton.Click += new System.EventHandler(this.upButton_Click);
			// 
			// deleteButton
			// 
			this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.deleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.deleteButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.delete;
			this.deleteButton.Location = new System.Drawing.Point(456, 8);
			this.deleteButton.Name = "deleteButton";
			this.deleteButton.Size = new System.Drawing.Size(26, 26);
			this.deleteButton.TabIndex = 1;
			this.deleteButton.UseVisualStyleBackColor = true;
			this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
			// 
			// duplicateButton
			// 
			this.duplicateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.duplicateButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.duplicateButton.Image = global::BinaryPhoenix.Fusion.Editor.Properties.Resources.copy;
			this.duplicateButton.Location = new System.Drawing.Point(424, 8);
			this.duplicateButton.Name = "duplicateButton";
			this.duplicateButton.Size = new System.Drawing.Size(26, 26);
			this.duplicateButton.TabIndex = 6;
			this.duplicateButton.UseVisualStyleBackColor = true;
			this.duplicateButton.Click += new System.EventHandler(this.duplicateButton_Click);
			// 
			// SceneGraphWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(491, 290);
			this.Controls.Add(this.duplicateButton);
			this.Controls.Add(this.rightButton);
			this.Controls.Add(this.leftButton);
			this.Controls.Add(this.downButton);
			this.Controls.Add(this.upButton);
			this.Controls.Add(this.deleteButton);
			this.Controls.Add(this.sceneGraphTreeView);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "SceneGraphWindow";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Scene Graph";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SceneGraphWindow_FormClosing);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView sceneGraphTreeView;
		private System.Windows.Forms.Button deleteButton;
		private System.Windows.Forms.Button upButton;
		private System.Windows.Forms.Button downButton;
		private System.Windows.Forms.Button rightButton;
		private System.Windows.Forms.Button leftButton;
		private System.Windows.Forms.ImageList sceneNodeImageList;
		private System.Windows.Forms.Button duplicateButton;
	}
}