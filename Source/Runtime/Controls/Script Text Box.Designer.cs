namespace BinaryPhoenix.Fusion.Runtime.Controls
{
	partial class ScriptTextBox
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.scriptRichTextBox = new System.Windows.Forms.RichTextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.lineRichTextBox = new System.Windows.Forms.RichTextBox();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// scriptRichTextBox
			// 
			this.scriptRichTextBox.AcceptsTab = true;
			this.scriptRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.scriptRichTextBox.DetectUrls = false;
			this.scriptRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scriptRichTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.scriptRichTextBox.Location = new System.Drawing.Point(40, 0);
			this.scriptRichTextBox.Name = "scriptRichTextBox";
			this.scriptRichTextBox.Size = new System.Drawing.Size(374, 322);
			this.scriptRichTextBox.TabIndex = 0;
			this.scriptRichTextBox.Text = "";
			this.scriptRichTextBox.WordWrap = false;
			this.scriptRichTextBox.SizeChanged += new System.EventHandler(this.scriptRichTextBox_SizeChanged);
			this.scriptRichTextBox.SelectionChanged += new System.EventHandler(this.scriptRichTextBox_SelectionChanged);
			this.scriptRichTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.scriptRichTextBox_KeyDown);
			this.scriptRichTextBox.VScroll += new System.EventHandler(this.scriptRichTextBox_VScroll);
			this.scriptRichTextBox.TextChanged += new System.EventHandler(this.scriptRichTextBox_TextChanged);
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panel1.Controls.Add(this.lineRichTextBox);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(40, 322);
			this.panel1.TabIndex = 1;
			// 
			// lineRichTextBox
			// 
			this.lineRichTextBox.AcceptsTab = true;
			this.lineRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.lineRichTextBox.BackColor = System.Drawing.SystemColors.Control;
			this.lineRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lineRichTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lineRichTextBox.Location = new System.Drawing.Point(0, 0);
			this.lineRichTextBox.Name = "lineRichTextBox";
			this.lineRichTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lineRichTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.lineRichTextBox.Size = new System.Drawing.Size(39, 368);
			this.lineRichTextBox.TabIndex = 2;
			this.lineRichTextBox.Text = "";
			this.lineRichTextBox.WordWrap = false;
			// 
			// ScriptTextBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.scriptRichTextBox);
			this.Controls.Add(this.panel1);
			this.Name = "ScriptTextBox";
			this.Size = new System.Drawing.Size(414, 322);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RichTextBox scriptRichTextBox;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RichTextBox lineRichTextBox;
	}
}
