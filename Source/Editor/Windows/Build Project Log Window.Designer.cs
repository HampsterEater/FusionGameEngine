namespace BinaryPhoenix.Fusion.Editor.Windows
{
    partial class BuildProjectLogWindow
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.finishButton = new System.Windows.Forms.Button();
            this.taskLabel = new System.Windows.Forms.Label();
            this.taskProgressBar = new System.Windows.Forms.ProgressBar();
            this.errorCounterLabel = new System.Windows.Forms.Label();
            this.subTaskProgressBar = new System.Windows.Forms.ProgressBar();
            this.subTaskLabel = new System.Windows.Forms.Label();
            this.logListView = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(537, 25);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // finishButton
            // 
            this.finishButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.finishButton.Enabled = false;
            this.finishButton.Location = new System.Drawing.Point(537, 57);
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(75, 23);
            this.finishButton.TabIndex = 2;
            this.finishButton.Text = "Finish";
            this.finishButton.UseVisualStyleBackColor = true;
            // 
            // taskLabel
            // 
            this.taskLabel.AutoSize = true;
            this.taskLabel.Location = new System.Drawing.Point(12, 9);
            this.taskLabel.Name = "taskLabel";
            this.taskLabel.Size = new System.Drawing.Size(92, 13);
            this.taskLabel.TabIndex = 3;
            this.taskLabel.Text = "Building Project ...";
            // 
            // taskProgressBar
            // 
            this.taskProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.taskProgressBar.Location = new System.Drawing.Point(15, 25);
            this.taskProgressBar.Name = "taskProgressBar";
            this.taskProgressBar.Size = new System.Drawing.Size(506, 13);
            this.taskProgressBar.TabIndex = 4;
            // 
            // errorCounterLabel
            // 
            this.errorCounterLabel.AutoSize = true;
            this.errorCounterLabel.Location = new System.Drawing.Point(12, 98);
            this.errorCounterLabel.Name = "errorCounterLabel";
            this.errorCounterLabel.Size = new System.Drawing.Size(102, 13);
            this.errorCounterLabel.TabIndex = 5;
            this.errorCounterLabel.Text = "Build Logs ( 0 logs  )";
            // 
            // subTaskProgressBar
            // 
            this.subTaskProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.subTaskProgressBar.Location = new System.Drawing.Point(15, 67);
            this.subTaskProgressBar.Name = "subTaskProgressBar";
            this.subTaskProgressBar.Size = new System.Drawing.Size(506, 13);
            this.subTaskProgressBar.TabIndex = 7;
            // 
            // subTaskLabel
            // 
            this.subTaskLabel.AutoSize = true;
            this.subTaskLabel.Location = new System.Drawing.Point(12, 51);
            this.subTaskLabel.Name = "subTaskLabel";
            this.subTaskLabel.Size = new System.Drawing.Size(153, 13);
            this.subTaskLabel.TabIndex = 6;
            this.subTaskLabel.Text = "Compiling Crystalline River.fs ...";
            // 
            // logListView
            // 
            this.logListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.logListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.logListView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.logListView.FormattingEnabled = true;
            this.logListView.Location = new System.Drawing.Point(15, 124);
            this.logListView.Name = "logListView";
            this.logListView.Size = new System.Drawing.Size(597, 160);
            this.logListView.TabIndex = 8;
            // 
            // BuildProjectLogWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 294);
            this.Controls.Add(this.logListView);
            this.Controls.Add(this.subTaskProgressBar);
            this.Controls.Add(this.subTaskLabel);
            this.Controls.Add(this.errorCounterLabel);
            this.Controls.Add(this.taskProgressBar);
            this.Controls.Add(this.taskLabel);
            this.Controls.Add(this.finishButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BuildProjectLogWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Build Log";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button finishButton;
        private System.Windows.Forms.Label taskLabel;
        private System.Windows.Forms.ProgressBar taskProgressBar;
        private System.Windows.Forms.Label errorCounterLabel;
        private System.Windows.Forms.ProgressBar subTaskProgressBar;
        private System.Windows.Forms.Label subTaskLabel;
        private System.Windows.Forms.ListBox logListView;
    }
}