namespace BinaryPhoenix.Fusion.Engine.Windows
{
    partial class MusicPlayerWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MusicPlayerWindow));
            this.fileLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.loopingCheckBox = new System.Windows.Forms.CheckBox();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.volumeTrackBar = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panTrackBar = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.frequencyTrackBar = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.pauseButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.volumeTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // fileLabel
            // 
            this.fileLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileLabel.Location = new System.Drawing.Point(80, 36);
            this.fileLabel.Name = "fileLabel";
            this.fileLabel.Size = new System.Drawing.Size(209, 20);
            this.fileLabel.TabIndex = 1;
            this.fileLabel.Text = "test.mpg";
            this.fileLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(10, 12);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(360, 15);
            this.progressBar.TabIndex = 3;
            // 
            // loopingCheckBox
            // 
            this.loopingCheckBox.AutoSize = true;
            this.loopingCheckBox.Location = new System.Drawing.Point(10, 39);
            this.loopingCheckBox.Name = "loopingCheckBox";
            this.loopingCheckBox.Size = new System.Drawing.Size(64, 17);
            this.loopingCheckBox.TabIndex = 4;
            this.loopingCheckBox.Text = "Looping";
            this.loopingCheckBox.UseVisualStyleBackColor = true;
            this.loopingCheckBox.CheckedChanged += new System.EventHandler(this.loopingCheckBox_CheckedChanged);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "pause.png");
            this.imageList.Images.SetKeyName(1, "play.png");
            // 
            // updateTimer
            // 
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // volumeTrackBar
            // 
            this.volumeTrackBar.Location = new System.Drawing.Point(69, 82);
            this.volumeTrackBar.Maximum = 100;
            this.volumeTrackBar.Name = "volumeTrackBar";
            this.volumeTrackBar.Size = new System.Drawing.Size(302, 45);
            this.volumeTrackBar.TabIndex = 5;
            this.volumeTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.volumeTrackBar.Value = 100;
            this.volumeTrackBar.Scroll += new System.EventHandler(this.volumeTrackBar_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Volume";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 118);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Pan";
            // 
            // panTrackBar
            // 
            this.panTrackBar.Location = new System.Drawing.Point(69, 115);
            this.panTrackBar.Maximum = 50;
            this.panTrackBar.Minimum = -50;
            this.panTrackBar.Name = "panTrackBar";
            this.panTrackBar.Size = new System.Drawing.Size(302, 45);
            this.panTrackBar.TabIndex = 7;
            this.panTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.panTrackBar.Scroll += new System.EventHandler(this.panTrackBar_Scroll);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 153);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Frequency";
            // 
            // frequencyTrackBar
            // 
            this.frequencyTrackBar.Location = new System.Drawing.Point(69, 148);
            this.frequencyTrackBar.Maximum = 199999;
            this.frequencyTrackBar.Name = "frequencyTrackBar";
            this.frequencyTrackBar.Size = new System.Drawing.Size(302, 45);
            this.frequencyTrackBar.TabIndex = 9;
            this.frequencyTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.frequencyTrackBar.Value = 44600;
            this.frequencyTrackBar.Scroll += new System.EventHandler(this.frequencyTrackBar_Scroll);
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Location = new System.Drawing.Point(10, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(361, 2);
            this.label4.TabIndex = 11;
            this.label4.Text = "label4";
            // 
            // pauseButton
            // 
            this.pauseButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.pauseButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.pauseButton.ImageIndex = 0;
            this.pauseButton.ImageList = this.imageList;
            this.pauseButton.Location = new System.Drawing.Point(304, 33);
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.Size = new System.Drawing.Size(67, 26);
            this.pauseButton.TabIndex = 0;
            this.pauseButton.Text = "Pause";
            this.pauseButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.pauseButton.UseVisualStyleBackColor = true;
            this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
            // 
            // MusicPlayerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 188);
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MusicPlayerWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Music Player";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MusicPlayerWindow_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.volumeTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button pauseButton;
        private System.Windows.Forms.Label fileLabel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.CheckBox loopingCheckBox;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.TrackBar volumeTrackBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar panTrackBar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar frequencyTrackBar;
        private System.Windows.Forms.Label label4;
    }
}