namespace BinaryPhoenix.Fusion.Runtime.Scripting
{
    partial class Debugger
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Debugger));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.sourceCodeScriptTextBox = new BinaryPhoenix.Fusion.Runtime.Controls.ScriptTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.disassemblyScriptTextBox = new BinaryPhoenix.Fusion.Runtime.Controls.ScriptTextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.variablesPropertyListView = new BinaryPhoenix.Fusion.Runtime.Controls.PropertyListView();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.callStackListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.stackTreeView = new System.Windows.Forms.TreeView();
            this.variableIconsImageList = new System.Windows.Forms.ImageList(this.components);
            this.stackPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.frameIndexLabel = new System.Windows.Forms.Label();
            this.topIndexLabel = new System.Windows.Forms.Label();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.objectsTreeView = new System.Windows.Forms.TreeView();
            this.objectsPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.heapTreeView = new System.Windows.Forms.TreeView();
            this.heapPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.startToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.stopToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.stepToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabPage8.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Controls.Add(this.tabPage5);
            this.tabControl.Controls.Add(this.tabPage6);
            this.tabControl.Controls.Add(this.tabPage7);
            this.tabControl.Controls.Add(this.tabPage8);
            this.tabControl.Location = new System.Drawing.Point(1, 40);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(509, 399);
            this.tabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.sourceCodeScriptTextBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(501, 370);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Source Code";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // sourceCodeScriptTextBox
            // 
            this.sourceCodeScriptTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sourceCodeScriptTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceCodeScriptTextBox.Location = new System.Drawing.Point(3, 3);
            this.sourceCodeScriptTextBox.Name = "sourceCodeScriptTextBox";
            this.sourceCodeScriptTextBox.ReadOnly = true;
            this.sourceCodeScriptTextBox.Size = new System.Drawing.Size(495, 364);
            this.sourceCodeScriptTextBox.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.disassemblyScriptTextBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(501, 370);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Disassembly";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // disassemblyScriptTextBox
            // 
            this.disassemblyScriptTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.disassemblyScriptTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.disassemblyScriptTextBox.Location = new System.Drawing.Point(3, 3);
            this.disassemblyScriptTextBox.Name = "disassemblyScriptTextBox";
            this.disassemblyScriptTextBox.ReadOnly = true;
            this.disassemblyScriptTextBox.Size = new System.Drawing.Size(495, 364);
            this.disassemblyScriptTextBox.TabIndex = 2;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.variablesPropertyListView);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(501, 370);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Variables";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // variablesPropertyListView
            // 
            this.variablesPropertyListView.BackColor = System.Drawing.SystemColors.Control;
            this.variablesPropertyListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.variablesPropertyListView.Location = new System.Drawing.Point(3, 3);
            this.variablesPropertyListView.Name = "variablesPropertyListView";
            this.variablesPropertyListView.Size = new System.Drawing.Size(495, 364);
            this.variablesPropertyListView.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.callStackListView);
            this.tabPage5.Location = new System.Drawing.Point(4, 25);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(501, 370);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Call Stack";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // callStackListView
            // 
            this.callStackListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.callStackListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.callStackListView.FullRowSelect = true;
            this.callStackListView.Location = new System.Drawing.Point(3, 3);
            this.callStackListView.MultiSelect = false;
            this.callStackListView.Name = "callStackListView";
            this.callStackListView.Size = new System.Drawing.Size(495, 364);
            this.callStackListView.TabIndex = 0;
            this.callStackListView.UseCompatibleStateImageBehavior = false;
            this.callStackListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Identifier";
            this.columnHeader1.Width = 460;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.splitContainer3);
            this.tabPage6.Controls.Add(this.frameIndexLabel);
            this.tabPage6.Controls.Add(this.topIndexLabel);
            this.tabPage6.Location = new System.Drawing.Point(4, 25);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(501, 370);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "Stack";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer3.Location = new System.Drawing.Point(3, 30);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.stackTreeView);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.stackPropertyGrid);
            this.splitContainer3.Size = new System.Drawing.Size(495, 337);
            this.splitContainer3.SplitterDistance = 165;
            this.splitContainer3.TabIndex = 4;
            // 
            // stackTreeView
            // 
            this.stackTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stackTreeView.ImageIndex = 0;
            this.stackTreeView.ImageList = this.variableIconsImageList;
            this.stackTreeView.Location = new System.Drawing.Point(0, 0);
            this.stackTreeView.Name = "stackTreeView";
            this.stackTreeView.SelectedImageIndex = 0;
            this.stackTreeView.Size = new System.Drawing.Size(165, 337);
            this.stackTreeView.TabIndex = 0;
            this.stackTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.stackTreeView_AfterSelect);
            // 
            // variableIconsImageList
            // 
            this.variableIconsImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("variableIconsImageList.ImageStream")));
            this.variableIconsImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.variableIconsImageList.Images.SetKeyName(0, "variable_icon.PNG");
            // 
            // stackPropertyGrid
            // 
            this.stackPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stackPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.stackPropertyGrid.Name = "stackPropertyGrid";
            this.stackPropertyGrid.Size = new System.Drawing.Size(326, 337);
            this.stackPropertyGrid.TabIndex = 0;
            // 
            // frameIndexLabel
            // 
            this.frameIndexLabel.AutoSize = true;
            this.frameIndexLabel.Location = new System.Drawing.Point(98, 8);
            this.frameIndexLabel.Name = "frameIndexLabel";
            this.frameIndexLabel.Size = new System.Drawing.Size(77, 13);
            this.frameIndexLabel.TabIndex = 3;
            this.frameIndexLabel.Text = "Frame Index: 0";
            // 
            // topIndexLabel
            // 
            this.topIndexLabel.AutoSize = true;
            this.topIndexLabel.Location = new System.Drawing.Point(7, 8);
            this.topIndexLabel.Name = "topIndexLabel";
            this.topIndexLabel.Size = new System.Drawing.Size(67, 13);
            this.topIndexLabel.TabIndex = 2;
            this.topIndexLabel.Text = "Top Index: 0";
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.splitContainer1);
            this.tabPage7.Location = new System.Drawing.Point(4, 25);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(501, 370);
            this.tabPage7.TabIndex = 6;
            this.tabPage7.Text = "Objects";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.objectsTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.objectsPropertyGrid);
            this.splitContainer1.Size = new System.Drawing.Size(495, 364);
            this.splitContainer1.SplitterDistance = 165;
            this.splitContainer1.TabIndex = 0;
            // 
            // objectsTreeView
            // 
            this.objectsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectsTreeView.ImageIndex = 0;
            this.objectsTreeView.ImageList = this.variableIconsImageList;
            this.objectsTreeView.Location = new System.Drawing.Point(0, 0);
            this.objectsTreeView.Name = "objectsTreeView";
            this.objectsTreeView.SelectedImageIndex = 0;
            this.objectsTreeView.Size = new System.Drawing.Size(165, 364);
            this.objectsTreeView.TabIndex = 0;
            this.objectsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.objectsTreeView_AfterSelect);
            // 
            // objectsPropertyGrid
            // 
            this.objectsPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectsPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.objectsPropertyGrid.Name = "objectsPropertyGrid";
            this.objectsPropertyGrid.Size = new System.Drawing.Size(326, 364);
            this.objectsPropertyGrid.TabIndex = 0;
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.splitContainer2);
            this.tabPage8.Location = new System.Drawing.Point(4, 25);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage8.Size = new System.Drawing.Size(501, 370);
            this.tabPage8.TabIndex = 7;
            this.tabPage8.Text = "Heap";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.heapTreeView);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.heapPropertyGrid);
            this.splitContainer2.Size = new System.Drawing.Size(495, 364);
            this.splitContainer2.SplitterDistance = 165;
            this.splitContainer2.TabIndex = 1;
            // 
            // heapTreeView
            // 
            this.heapTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.heapTreeView.ImageIndex = 0;
            this.heapTreeView.ImageList = this.variableIconsImageList;
            this.heapTreeView.Location = new System.Drawing.Point(0, 0);
            this.heapTreeView.Name = "heapTreeView";
            this.heapTreeView.SelectedImageIndex = 0;
            this.heapTreeView.Size = new System.Drawing.Size(165, 364);
            this.heapTreeView.TabIndex = 0;
            this.heapTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.heapTreeView_AfterSelect);
            // 
            // heapPropertyGrid
            // 
            this.heapPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.heapPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.heapPropertyGrid.Name = "heapPropertyGrid";
            this.heapPropertyGrid.Size = new System.Drawing.Size(326, 364);
            this.heapPropertyGrid.TabIndex = 0;
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripButton,
            this.stopToolStripButton,
            this.toolStripSeparator1,
            this.stepToolStripButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(510, 36);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // startToolStripButton
            // 
            this.startToolStripButton.Image = global::BinaryPhoenix.Fusion.Runtime.Properties.Resources.start;
            this.startToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.startToolStripButton.Name = "startToolStripButton";
            this.startToolStripButton.Size = new System.Drawing.Size(35, 33);
            this.startToolStripButton.Text = "Start";
            this.startToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.startToolStripButton.Click += new System.EventHandler(this.startToolStripButton_Click);
            // 
            // stopToolStripButton
            // 
            this.stopToolStripButton.Enabled = false;
            this.stopToolStripButton.Image = global::BinaryPhoenix.Fusion.Runtime.Properties.Resources.stop;
            this.stopToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopToolStripButton.Name = "stopToolStripButton";
            this.stopToolStripButton.Size = new System.Drawing.Size(33, 33);
            this.stopToolStripButton.Text = "Stop";
            this.stopToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.stopToolStripButton.Click += new System.EventHandler(this.stopToolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 36);
            // 
            // stepToolStripButton
            // 
            this.stepToolStripButton.Image = global::BinaryPhoenix.Fusion.Runtime.Properties.Resources.step;
            this.stepToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stepToolStripButton.Name = "stepToolStripButton";
            this.stepToolStripButton.Size = new System.Drawing.Size(33, 33);
            this.stepToolStripButton.Text = "Step";
            this.stepToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.stepToolStripButton.Click += new System.EventHandler(this.stepToolStripButton_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = global::BinaryPhoenix.Fusion.Runtime.Properties.Resources.start;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(35, 33);
            this.toolStripButton1.Text = "Start";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(33, 33);
            this.toolStripButton2.Text = "Stop";
            this.toolStripButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(33, 33);
            this.toolStripButton3.Text = "Step";
            this.toolStripButton3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // Debugger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(510, 439);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "Debugger";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Script Debugger";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Debugger_FormClosed);
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            this.tabPage7.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabPage8.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton startToolStripButton;
        private System.Windows.Forms.ToolStripButton stopToolStripButton;
        private System.Windows.Forms.ToolStripButton stepToolStripButton;
        private BinaryPhoenix.Fusion.Runtime.Controls.ScriptTextBox sourceCodeScriptTextBox;
        private BinaryPhoenix.Fusion.Runtime.Controls.ScriptTextBox disassemblyScriptTextBox;
        private System.Windows.Forms.ListView callStackListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.ImageList variableIconsImageList;
        private System.Windows.Forms.Label frameIndexLabel;
        private System.Windows.Forms.Label topIndexLabel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView objectsTreeView;
        private System.Windows.Forms.PropertyGrid objectsPropertyGrid;
        private BinaryPhoenix.Fusion.Runtime.Controls.PropertyListView variablesPropertyListView;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TreeView heapTreeView;
        private System.Windows.Forms.PropertyGrid heapPropertyGrid;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.TreeView stackTreeView;
        private System.Windows.Forms.PropertyGrid stackPropertyGrid;
    }
}