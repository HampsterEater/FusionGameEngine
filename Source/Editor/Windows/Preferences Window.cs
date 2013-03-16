/*
 * File: Preferences Window.cs
 *
 * Contains all the functional partial code declaration for the PreferencesWindow form.
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
using System.IO;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Editor.Windows
{

	/// <summary>
	///		This class contains the code used to display and operate  
	///		the preferences window.
	/// </summary>
	public partial class PreferencesWindow : Form
	{
		#region Members
		#region Variables

		private TreeNode[] _nodes = new TreeNode[1];

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Initializes a new instance of this class and sets up all the 
		///		controls on this form.
		/// </summary>
		public PreferencesWindow()
		{
			// Initializes all the controls on this window.
			InitializeComponent();

			// Create the navigation nodes and add them to the navigation tree.
			_nodes[0] = new System.Windows.Forms.TreeNode("General");
			optionTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] { _nodes[0] });
		}

		/// <summary>
		///		Closes this window without saving the changed settings.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void cancelButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		/// <summary>
		///		Closes this window and saves and reloads the changed settings.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void okButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
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

		#endregion
	}
}