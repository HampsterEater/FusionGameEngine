/*
 * File: Map Properties Window.cs
 *
 * Contains all the functional partial code declaration for the MapPropertiesWindow form.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Engine;

namespace BinaryPhoenix.Fusion.Editor.Windows
{

	/// <summary>
	///		Contains all the partial code used to display and operate a map properties window.
	/// </summary>
	public partial class MapPropertiesWindow : Form
	{
		#region Members
		#region Variables

		private TreeNode[] _nodes = new TreeNode[4];
		private MapProperties _mapProperties = null;

		#endregion	
		#region Properties

		/// <summary>
		///		Gets or sets the map properties shown by this window.
		/// </summary>
		public MapProperties MapProperties
		{
			get { return _mapProperties; }
			set { _mapProperties = value; SyncProperties(); }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Initializes this instance and sets up the window and all its controls.
		/// </summary>
		public MapPropertiesWindow(MapProperties properties)
		{
			// Initializes all the controls on this window.
			InitializeComponent();

			// Retrieve the map properties from the editor window.
			_mapProperties = new MapProperties(properties);
			SyncProperties();

			// Create the navigation nodes and add them to the navigation tree.
			_nodes[0] = new System.Windows.Forms.TreeNode("General");
			_nodes[1] = new System.Windows.Forms.TreeNode("Debug");
			_nodes[2] = new System.Windows.Forms.TreeNode("Security");
			_nodes[3] = new System.Windows.Forms.TreeNode("Infomation");
			optionTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] { _nodes[0], _nodes[1], _nodes[2], _nodes[3] });
		}

		/// <summary>
		///		Syncronizes the properties displayed on this window to those
		///		stored in this maps properties.
		/// </summary>
		public void SyncProperties()
		{
			encryptCheckBox.Checked = _mapProperties.Encrypt;
			compressCheckBox.Checked = _mapProperties.Compress;
			passwordTextBox.Text = _mapProperties.Password;
			authorTextBox.Text = _mapProperties.Author;
			descriptionTextBox.Text = _mapProperties.Description;
			nameTextBox.Text = _mapProperties.Name;
			versionNumericUpDown.Value = (decimal)_mapProperties.Version;
		}

		/// <summary>
		///		Called when the users clicks a navigation node. Simply switchs to
		///		the correct option page.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void optionTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			for (int i = 0; i < _nodes.Length; i++)
				if (optionTreeView.SelectedNode == _nodes[i])
					stepTabControl.SelectTab(i);
		}

		/// <summary>
		///		Applys the map settings and closes this window when the use clicks
		///		the Ok button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void okButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			Close();
		}

		/// <summary>
		///		Closes this window without applying map settings when the user clicks 
		///		the Cancel button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void cancelButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			Close();
		}

		/// <summary>
		///		Gathers map properties when the user checks the Compress check box.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void compressCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			_mapProperties.Compress = compressCheckBox.Checked;
		}

		/// <summary>
		///		Gathers map properties when the user checks the Encrypt check box. If
		///		the encrypt button is uncheck this function will also disable the password text box.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void encryptCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			_mapProperties.Encrypt = encryptCheckBox.Checked;
			passwordTextBox.Enabled = encryptCheckBox.Checked;
		}

		/// <summary>
		///		Gathers map properties when the user changes the text in the password text box.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void passwordTextBox_TextChanged(object sender, EventArgs e)
		{
			_mapProperties.Password = passwordTextBox.Text;
		}

		/// <summary>
		///		Gathers map properties when the user changes the text in the name text box.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void nameTextBox_TextChanged(object sender, EventArgs e)
		{
			_mapProperties.Name = nameTextBox.Text;
		}

		/// <summary>
		///		Gathers map properties when the user changes the text in the author text box.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void authorTextBox_TextChanged(object sender, EventArgs e)
		{
			_mapProperties.Author = authorTextBox.Text;
		}

		/// <summary>
		///		Gathers map properties when the user changes the text in the version box.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void versionNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			_mapProperties.Version = (float)versionNumericUpDown.Value;
		}

		/// <summary>
		///		Gathers map properties when the user changes the text in the description text box.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void descriptionTextBox_TextChanged(object sender, EventArgs e)
		{
			_mapProperties.Description = descriptionTextBox.Text;
		}

		#endregion
	}
}