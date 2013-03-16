namespace BinaryPhoenix.Fusion.Engine.Windows
{
    partial class SoundSelectorWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SoundSelectorWindow));
            this.fileTreeView = new System.Windows.Forms.TreeView();
            this.fileImageList = new System.Windows.Forms.ImageList(this.components);
            this.loadButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.label3 = new System.Windows.Forms.Label();
            this.frequencyTrackBar = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.panTrackBar = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.volumeTrackBar = new System.Windows.Forms.TrackBar();
            this.loopingCheckBox = new System.Windows.Forms.CheckBox();
            this.fileLabel = new System.Windows.Forms.Label();
            this.pauseButton = new System.Windows.Forms.Button();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.streamedCheckBox = new System.Windows.Forms.CheckBox();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.positionalCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.volumeTrackBar)).BeginInit();
            this.SuspendLayout();
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
            this.fileTreeView.Size = new System.Drawing.Size(231, 234);
            this.fileTreeView.TabIndex = 1;
            this.fileTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.fileTreeView_AfterSelect);
            // 
            // fileImageList
            // 
            this.fileImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("fileImageList.ImageStream")));
            this.fileImageList.TransparentColor = System.Drawing.Color.Fuchsia;
            this.fileImageList.Images.SetKeyName(0, "folder.png");
            this.fileImageList.Images.SetKeyName(1, "sound.png");
            // 
            // loadButton
            // 
            this.loadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.loadButton.Location = new System.Drawing.Point(533, 270);
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
            this.cancelButton.Location = new System.Drawing.Point(452, 270);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.panel3.Location = new System.Drawing.Point(12, 260);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(596, 1);
            this.panel3.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Location = new System.Drawing.Point(260, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(341, 2);
            this.label4.TabIndex = 23;
            this.label4.Text = "label4";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(260, 12);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(340, 15);
            this.progressBar.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(261, 199);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Frequency";
            // 
            // frequencyTrackBar
            // 
            this.frequencyTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.frequencyTrackBar.Location = new System.Drawing.Point(319, 194);
            this.frequencyTrackBar.Maximum = 199999;
            this.frequencyTrackBar.Name = "frequencyTrackBar";
            this.frequencyTrackBar.Size = new System.Drawing.Size(282, 45);
            this.frequencyTrackBar.TabIndex = 21;
            this.frequencyTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.frequencyTrackBar.Value = 44600;
            this.frequencyTrackBar.Scroll += new System.EventHandler(this.frequencyTrackBar_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(262, 164);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Pan";
            // 
            // panTrackBar
            // 
            this.panTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panTrackBar.Location = new System.Drawing.Point(319, 161);
            this.panTrackBar.Maximum = 50;
            this.panTrackBar.Minimum = -50;
            this.panTrackBar.Name = "panTrackBar";
            this.panTrackBar.Size = new System.Drawing.Size(282, 45);
            this.panTrackBar.TabIndex = 19;
            this.panTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.panTrackBar.Scroll += new System.EventHandler(this.panTrackBar_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(261, 133);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Volume";
            // 
            // volumeTrackBar
            // 
            this.volumeTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.volumeTrackBar.Location = new System.Drawing.Point(319, 128);
            this.volumeTrackBar.Maximum = 100;
            this.volumeTrackBar.Name = "volumeTrackBar";
            this.volumeTrackBar.Size = new System.Drawing.Size(282, 45);
            this.volumeTrackBar.TabIndex = 17;
            this.volumeTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.volumeTrackBar.Value = 100;
            this.volumeTrackBar.Scroll += new System.EventHandler(this.volumeTrackBar_Scroll);
            // 
            // loopingCheckBox
            // 
            this.loopingCheckBox.AutoSize = true;
            this.loopingCheckBox.Location = new System.Drawing.Point(260, 39);
            this.loopingCheckBox.Name = "loopingCheckBox";
            this.loopingCheckBox.Size = new System.Drawing.Size(64, 17);
            this.loopingCheckBox.TabIndex = 16;
            this.loopingCheckBox.Text = "Looping";
            this.loopingCheckBox.UseVisualStyleBackColor = true;
            this.loopingCheckBox.CheckedChanged += new System.EventHandler(this.loopingCheckBox_CheckedChanged);
            // 
            // fileLabel
            // 
            this.fileLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileLabel.Location = new System.Drawing.Point(339, 36);
            this.fileLabel.Name = "fileLabel";
            this.fileLabel.Size = new System.Drawing.Size(180, 66);
            this.fileLabel.TabIndex = 14;
            this.fileLabel.Text = "test.mpg";
            this.fileLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pauseButton
            // 
            this.pauseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pauseButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.pauseButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.pauseButton.ImageIndex = 0;
            this.pauseButton.ImageList = this.imageList;
            this.pauseButton.Location = new System.Drawing.Point(534, 33);
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.Size = new System.Drawing.Size(67, 69);
            this.pauseButton.TabIndex = 13;
            this.pauseButton.Text = "Pause";
            this.pauseButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.pauseButton.UseVisualStyleBackColor = true;
            this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "pause.png");
            this.imageList.Images.SetKeyName(1, "play.png");
            // 
            // streamedCheckBox
            // 
            this.streamedCheckBox.AutoSize = true;
            this.streamedCheckBox.Location = new System.Drawing.Point(260, 62);
            this.streamedCheckBox.Name = "streamedCheckBox";
            this.streamedCheckBox.Size = new System.Drawing.Size(73, 17);
            this.streamedCheckBox.TabIndex = 24;
            this.streamedCheckBox.Text = "Streaming";
            this.streamedCheckBox.UseVisualStyleBackColor = true;
            this.streamedCheckBox.CheckedChanged += new System.EventHandler(this.streamedCheckBox_CheckedChanged);
            // 
            // updateTimer
            // 
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // positionalCheckBox
            // 
            this.positionalCheckBox.AutoSize = true;
            this.positionalCheckBox.Location = new System.Drawing.Point(260, 85);
            this.positionalCheckBox.Name = "positionalCheckBox";
            this.positionalCheckBox.Size = new System.Drawing.Size(71, 17);
            this.positionalCheckBox.TabIndex = 25;
            this.positionalCheckBox.Text = "Positional";
            this.positionalCheckBox.UseVisualStyleBackColor = true;
            this.positionalCheckBox.Click += new System.EventHandler(this.positionalCheckBox_Click);
            // 
            // SoundSelectorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 305);
            this.Controls.Add(this.positionalCheckBox);
            this.Controls.Add(this.streamedCheckBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.frequencyTrackBar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panTrackBar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.volumeTrackBar);
            this.Controls.Add(this.loopingCheckBox);
            this.Controls.Add(this.fileLabel);
            this.Controls.Add(this.pauseButton);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.fileTreeView);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SoundSelectorWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Load Sound";
            this.Shown += new System.EventHandler(this.SoundSelectorWindow_VisibleChanged);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SoundSelectorWindow_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.frequencyTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.volumeTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView fileTreeView;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ImageList fileImageList;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar frequencyTrackBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar panTrackBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar volumeTrackBar;
        private System.Windows.Forms.CheckBox loopingCheckBox;
        private System.Windows.Forms.Label fileLabel;
        private System.Windows.Forms.Button pauseButton;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.CheckBox streamedCheckBox;
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.CheckBox positionalCheckBox;
    }
}