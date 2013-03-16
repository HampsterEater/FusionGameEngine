namespace BinaryPhoenix.Fusion.Editor.Windows
{
	partial class EntityPropertiesWindow
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
			this.propertyListView = new BinaryPhoenix.Fusion.Runtime.Controls.PropertyListView();
			this.SuspendLayout();
			// 
			// propertyListView
			// 
			this.propertyListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyListView.Location = new System.Drawing.Point(0, 0);
			this.propertyListView.Name = "propertyListView";
			this.propertyListView.Size = new System.Drawing.Size(316, 382);
			this.propertyListView.TabIndex = 0;
			// 
			// EntityPropertiesWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(316, 382);
			this.Controls.Add(this.propertyListView);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "EntityPropertiesWindow";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Entity Properties";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Closing);
			this.ResumeLayout(false);

		}

		#endregion

		private BinaryPhoenix.Fusion.Runtime.Controls.PropertyListView propertyListView;


	}
}