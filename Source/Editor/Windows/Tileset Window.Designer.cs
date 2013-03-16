namespace BinaryPhoenix.Fusion.Editor.Windows
{
    partial class TilesetWindow
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
            this.tilesetHScrollBar = new System.Windows.Forms.HScrollBar();
            this.tilesetVScrollBar = new System.Windows.Forms.VScrollBar();
            this.canvasPanel = new System.Windows.Forms.PictureBox();
            this.alphaTrackBar = new System.Windows.Forms.TrackBar();
            this.colorPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.canvasPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.alphaTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // tilesetHScrollBar
            // 
            this.tilesetHScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tilesetHScrollBar.Location = new System.Drawing.Point(0, 334);
            this.tilesetHScrollBar.Name = "tilesetHScrollBar";
            this.tilesetHScrollBar.Size = new System.Drawing.Size(325, 18);
            this.tilesetHScrollBar.TabIndex = 2;
            // 
            // tilesetVScrollBar
            // 
            this.tilesetVScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tilesetVScrollBar.Location = new System.Drawing.Point(325, 0);
            this.tilesetVScrollBar.Name = "tilesetVScrollBar";
            this.tilesetVScrollBar.Size = new System.Drawing.Size(18, 334);
            this.tilesetVScrollBar.TabIndex = 3;
            // 
            // canvasPanel
            // 
            this.canvasPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.canvasPanel.Location = new System.Drawing.Point(0, 0);
            this.canvasPanel.Name = "canvasPanel";
            this.canvasPanel.Size = new System.Drawing.Size(325, 334);
            this.canvasPanel.TabIndex = 4;
            this.canvasPanel.TabStop = false;
            // 
            // alphaTrackBar
            // 
            this.alphaTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.alphaTrackBar.Location = new System.Drawing.Point(0, 355);
            this.alphaTrackBar.Maximum = 255;
            this.alphaTrackBar.Name = "alphaTrackBar";
            this.alphaTrackBar.Size = new System.Drawing.Size(343, 45);
            this.alphaTrackBar.TabIndex = 5;
            this.alphaTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.alphaTrackBar.Value = 255;
            this.alphaTrackBar.Scroll += new System.EventHandler(this.alphaTrackBar_Scroll);
            // 
            // colorPanel
            // 
            this.colorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.colorPanel.BackColor = System.Drawing.Color.White;
            this.colorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorPanel.Location = new System.Drawing.Point(325, 334);
            this.colorPanel.Name = "colorPanel";
            this.colorPanel.Size = new System.Drawing.Size(18, 18);
            this.colorPanel.TabIndex = 7;
            this.colorPanel.Click += new System.EventHandler(this.colorPanel_Click);
            // 
            // TilesetWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 382);
            this.Controls.Add(this.tilesetVScrollBar);
            this.Controls.Add(this.colorPanel);
            this.Controls.Add(this.alphaTrackBar);
            this.Controls.Add(this.canvasPanel);
            this.Controls.Add(this.tilesetHScrollBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "TilesetWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tileset Viewer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TilesetWindow_FormClosed);
            this.ResizeEnd += new System.EventHandler(this.TilesetWindow_ResizeEnd);
            ((System.ComponentModel.ISupportInitialize)(this.canvasPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.alphaTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.HScrollBar tilesetHScrollBar;
        private System.Windows.Forms.VScrollBar tilesetVScrollBar;
        private System.Windows.Forms.PictureBox canvasPanel;
        private System.Windows.Forms.TrackBar alphaTrackBar;
        private System.Windows.Forms.Panel colorPanel;
    }
}