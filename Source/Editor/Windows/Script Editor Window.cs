/*
 * File: Script Editor Window.cs
 *
 * Contains all the functional partial code declaration for the ScriptEditorWindow form.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Scripting;

namespace BinaryPhoenix.Fusion.Editor.Windows
{

	/// <summary>
	///		This class contains the code used to display and operate  
	///		the script editor window.
	/// </summary>
	public partial class ScriptEditorWindow : Form
	{
		#region Members
		#region Variables

        private bool _isPlainFile = false;
        private object _url = null;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the url of the file this window is editing.
		/// </summary>
		public object Url
		{
			get { return _url; }
			set { _url = value; }
		}

        /// <summary>
        ///     Gets or sets if the file we are editing is just plain (eg. no need to highlight).
        /// </summary>
        public bool IsPlainFile
        {
            get { return _isPlainFile; }
            set { _isPlainFile = value; checkSyntaxToolStripButton.Visible = toolStripSeparator3.Visible = scriptTextBox.RequireHighlighting = !_isPlainFile; }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Reloads the current scripts data and updates the window
		///		to reflect any changes.
		/// </summary>
		private void SyncronizeData()
		{
			// Load in this scripts data.
			Stream stream = StreamFactory.RequestStream(_url, StreamMode.Open);
			if (stream == null) return;
			StreamReader reader = new StreamReader(stream);
			scriptTextBox.Text = reader.ReadToEnd();
			stream.Close();

            Text = "Script Editor - " + _url.ToString();

            scriptTextBox.RichTextBox.ClearUndo();
		}

		/// <summary>
		///		Disables and enables buttons on the tool strip 
		///		depending on if they can be used or not.
		/// </summary>
		private void SyncronizeToolStrip()
		{
			undoToolStripButton.Enabled = scriptTextBox.CanUndo;
			redoToolStripButton.Enabled = scriptTextBox.CanRedo;
			findToolStripButton.Enabled = (scriptTextBox.Text.Length > 0);
			checkSyntaxToolStripButton.Enabled = (scriptTextBox.Text.Length > 0);
			cutToolStripButton.Enabled = scriptTextBox.CanCut;
			copyToolStripButton.Enabled = scriptTextBox.CanCopy;
			pasteToolStripButton.Enabled = scriptTextBox.CanPaste;

			undoContextMenuItem.Enabled = scriptTextBox.CanUndo;
			redoContextMenuItem.Enabled = scriptTextBox.CanRedo;
			cutContextMenuItem.Enabled = scriptTextBox.CanCut;
			copyContextMenuItem.Enabled = scriptTextBox.CanCopy;
			pasteContextMenuItem.Enabled = scriptTextBox.CanPaste;
		}

		/// <summary>
		///		Highlights the script and updates the line labels 
		///		when this window is shown from the last time.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void ScriptEditorWindow_Shown(object sender, EventArgs e)
		{
			Text = "Script Editor - Highlighting...";
			if (_isPlainFile == false) scriptTextBox.Highlight(true);
			Refresh();
			SyncronizeToolStrip();
			Text = "Script Editor - " + _url.ToString();
		}

		/// <summary>
		///		Invoked when this form is closed, saves any changes made to this script.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void ScriptEditorWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			Save();
			if (scriptTextBox != null)
			{
				scriptTextBox.Dispose();
				scriptTextBox = null;
			}
		}

		/// <summary>
		///		Invoked when the user clicks the save button on the toolbar, saves this script.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void saveToolStripButton_Click(object sender, EventArgs e)
		{
			Save();
		}

		/// <summary>
		///		Saves any changes to this script to its file.
		/// </summary>
		private void Save()
		{
            File.Copy((string)_url,(string)_url+".backup", true);

			// Saves the scripts data back into its file.
			Stream stream = StreamFactory.RequestStream(_url, StreamMode.Truncate);
			if (stream == null) return;
			StreamWriter writer = new StreamWriter(stream);
			writer.Write(scriptTextBox.Text);
			writer.Flush();
			stream.Close();
		}

		/// <summary>
		///		Preforms a cut operation if the user clicks the toolbar strip cut button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void cutToolStripButton_Click(object sender, EventArgs e)
		{
			scriptTextBox.Cut();
			SyncronizeToolStrip();
		}

		/// <summary>
		///		Preforms a cut operation if the user clicks the toolbar strip copy button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void copyToolStripButton_Click(object sender, EventArgs e)
		{
			scriptTextBox.Copy();
			SyncronizeToolStrip();
		}

		/// <summary>
		///		Preforms a cut operation if the user clicks the toolbar strip paste button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void pasteToolStripButton_Click(object sender, EventArgs e)
		{
			scriptTextBox.Paste();
			SyncronizeToolStrip();
		}

		/// <summary>
		///		Preforms a cut operation if the user clicks the toolbar strip undo button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void undoToolStripButton_Click(object sender, EventArgs e)
		{
			scriptTextBox.Undo();
			SyncronizeToolStrip();
		}

		/// <summary>
		///		Preforms a cut operation if the user clicks the toolbar strip redo button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void redoToolStripButton_Click(object sender, EventArgs e)
		{
			scriptTextBox.Redo();
			SyncronizeToolStrip();
		}

		/// <summary>
		///		Shows the find and replace form when the users clicks the toolbar strip find button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void findToolStripButton_Click(object sender, EventArgs e)
		{
			scriptTextBox.ShowFindAndReplaceForm();
		}

		/// <summary>
		///		Preforms an error check on this script if the use clicks the check errors toolbar strip button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void checkSyntaxToolStripButton_Click(object sender, EventArgs e)
		{
			// Compile the script.
			ScriptCompiler compiler = new ScriptCompiler();
			bool errorOccured = false;
			//errorListView.Items.Clear();
			CompileFlags flags = 0;

            if (Path.GetExtension(_url.ToString()).ToLower() == ".fsl") flags |= CompileFlags.Library;

            string errors = "";
			if (compiler.CompileString(scriptTextBox.Text, flags, _url.ToString()) > 0)
			{
				foreach (CompileError error in compiler.ErrorList)
				{
					if (error.AlertLevel == ErrorAlertLevel.Error || error.AlertLevel == ErrorAlertLevel.FatalError)
						errorOccured = true;

					//ListViewItem item = new ListViewItem();
					//item.StateImageIndex = error.AlertLevel == ErrorAlertLevel.FatalError ? 2 : (int)error.AlertLevel;
					//item.SubItems.Add(new ListViewItem.ListViewSubItem(item, ((int)error.ErrorCode).ToString()));
					//item.SubItems.Add(new ListViewItem.ListViewSubItem(item, error.Line.ToString()));
					//item.SubItems.Add(new ListViewItem.ListViewSubItem(item, error.Offset.ToString()));
					//item.SubItems.Add(new ListViewItem.ListViewSubItem(item, error.Message));
					//errorListView.Items.Add(item);
                    errors += error.ToString() + "\n\n";
				}
			}
			
			if (errorOccured == false)
                if (errors == "")
                    MessageBox.Show("Error check successfull, script contains no syntax or syntantic errors.", "Error Check Successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Error check successfull, script contains no syntax or syntantic errors. However if contains the following warnings; \n\n" + errors, "Error Check Successfull", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("Script contains " + compiler.ErrorList.Count + " syntax or syntantic errors;\n\n" + errors, "Error Check Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		    
        }

		/// <summary>
		///		Called when the text in the script text box is changed, resyncronizes the 
		///		toolbar strip.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		public void scriptTextBox_TextChanged(object sender, EventArgs e)
		{
			SyncronizeToolStrip();
		}

		/// <summary>
		///		Called when the text selection in the script text box is changed, resyncronizes the 
		///		toolbar strip.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		public void scriptTextBox_SelectionChanged(object sender, EventArgs e)
		{
			SyncronizeToolStrip();
		}

		/// <summary>
		///		Creates a new instance of this class, and loads the script
		///		it should be editing from the given url.
		/// </summary>
		/// <param name="url">Url of script this window should edit.</param>
		public ScriptEditorWindow(object url)
		{
			// Initializes the controls on this form.
			InitializeComponent();
			scriptTextBox.TextChanged += new EventHandler(scriptTextBox_TextChanged);
			scriptTextBox.SelectionChanged += new EventHandler(scriptTextBox_SelectionChanged);

			// Syncronize this windows data.
			_url = url;
			SyncronizeData();
		}

		#endregion
	}
}