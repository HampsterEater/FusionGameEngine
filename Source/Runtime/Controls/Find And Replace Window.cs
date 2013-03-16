/*
 * File: Find Text Window.cs
 *
 * Contains all the functional partial code declaration for the FindTextWindow form.
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

namespace BinaryPhoenix.Fusion.Runtime.Controls
{

	/// <summary>
	///		This class contains the code used to display and operate  
	///		the find & replace window.
	/// </summary>
	public partial class FindAndReplaceWindow : Form
	{
		#region Members
		#region Variables

		private RichTextBox _richTextBox = null;

		private string _findWhatText = "";
		private string _replaceWithText = "";
		private bool _matchCase = false;

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Initializes a new instance of this class and associates
		///		it with the given rich text box.
		/// </summary>
		public FindAndReplaceWindow(RichTextBox richTextBox)
		{
			InitializeComponent();
			_richTextBox = richTextBox;
		}

		/// <summary>
		///		Retrieves data from this window when the user modifys the text in the find what text box.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void findWhatTextBox_TextChanged(object sender, EventArgs e)
		{
			_findWhatText = findWhatTextBox.Text;
		}

		/// <summary>
		///		Retrieves data from this window when the user modifys the text in the replace with text box.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void replaceWithTextBox_TextChanged(object sender, EventArgs e)
		{
			_replaceWithText = replaceWithTextBox.Text;
		}

		/// <summary>
		///		Retrieves data from this window when the user modifys the match case check box.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void matchCaseCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			_matchCase = matchCaseCheckBox.Checked;
		}

		/// <summary>
		///		Finds and replaces all instances of the given text in the rich text box associated with
		///		this window and highlights it when the user clicks the replace all next button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void replaceAllButton_Click(object sender, EventArgs e)
		{
			int replaceIndex = 0;
			int occurances = 0;
			string text = _richTextBox.Text;
			while ((replaceIndex = text.IndexOf(_findWhatText, replaceIndex)) > -1)
			{
				string replaceText = text.Substring(replaceIndex, _findWhatText.Length);
				if ((_matchCase == true && replaceText == _findWhatText) || (_matchCase == false && replaceText.ToLower() == _findWhatText.ToLower()))
				{
					string leftText = text.Substring(0, replaceIndex);
					string rightText = text.Substring(replaceIndex + _findWhatText.Length);
					text = leftText + _replaceWithText + rightText;
					occurances++;
				}
			}
			_richTextBox.Text = text;
			MessageBox.Show(occurances + " occurances of \""+_findWhatText+"\" were found and replaced with \""+_replaceWithText+"\".", "Replace All", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		/// <summary>
		///		Finds and replaces the given text in the rich text box associated with
		///		this window and highlights it when the user clicks the replace button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void replaceButton_Click(object sender, EventArgs e)
		{
			string text = _richTextBox.Text;
			int replaceIndex = text.IndexOf(_findWhatText, _richTextBox.SelectionStart + 1);
			if (replaceIndex > -1)
			{
				string replaceText = text.Substring(replaceIndex, _findWhatText.Length);
				if ((_matchCase == true && replaceText == _findWhatText) || (_matchCase == false && replaceText.ToLower() == _findWhatText.ToLower()))
				{
					string leftText = text.Substring(0, replaceIndex);
					string rightText = text.Substring(replaceIndex + _findWhatText.Length);
					text = leftText + _replaceWithText + rightText;
					_richTextBox.Text = text;
					_richTextBox.SelectionStart = replaceIndex;
					_richTextBox.SelectionLength = _replaceWithText.Length;
					_richTextBox.Focus();
				}
			}
			else
			{
				MessageBox.Show("No occurances of \"" + _findWhatText + "\" could be found.", "Replace", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		/// <summary>
		///		Finds the given text in the rich text box associated with
		///		this window and highlights it when the user clicks the find next button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void findNextButton_Click(object sender, EventArgs e)
		{
			string text = _richTextBox.Text;
			int replaceIndex = text.IndexOf(_findWhatText, _richTextBox.SelectionStart + 1);
			if (replaceIndex > -1)
			{
				string replaceText = text.Substring(replaceIndex, _findWhatText.Length);
				if ((_matchCase == true && replaceText == _findWhatText) || (_matchCase == false && replaceText.ToLower() == _findWhatText.ToLower()))
				{
					_richTextBox.SelectionStart = replaceIndex;
					_richTextBox.SelectionLength = _findWhatText.Length;
					_richTextBox.Focus();
				}
			}
			else
			{
				MessageBox.Show("No occurances of \"" + _findWhatText + "\" could be found.", "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		/// <summary>
		///		Closes the window when the user clicks the cancel button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void closeButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		#endregion
	}
}