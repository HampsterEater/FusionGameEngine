/*
 * File: Script Text Box.cs
 *
 * Contains all the functional partial code declaration for the ScriptTextBox user control.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Runtime.Controls
{

	/// <summary>
	///		The ScriptTextBox class is a specialized user control used to allow easy
	///		access to features like syntax highlighting and line numbering.
	/// </summary>
	public partial class ScriptTextBox : UserControl
	{
		#region Members
		#region Variables

		private Font _normalFont = new Font("Courier New", 8.25f);
		private Color _normalColor = Color.FromArgb(0, 0, 0);

		private Font _commentFont = new Font("Courier New", 8.25f);
		private Color _commentColor = Color.FromArgb(0, 128, 0);

		private Font _directiveFont = new Font("Courier New", 8.25f);
		private Color _directiveColor = Color.FromArgb(128, 128, 128);

		private Font _numberFont = new Font("Courier New", 8.25f);
		private Color _numberColor = Color.FromArgb(128, 0, 128);

		private Font _stringFont = new Font("Courier New", 8.25f);
		private Color _stringColor = Color.FromArgb(128, 0, 0);

		private Font _keywordFont = new Font("Courier New", 8.25f, FontStyle.Bold);
		private Color _keywordColor = Color.FromArgb(0, 0, 128);

		private string[] _keywords = new string[]
			{
				"state", "event", "default", "switch", "case",
				"if", "else", "enum", "const", "ref", "for",
				"return", "while", "do", "new", "delete", "lock",
				"atom", "object", "string", "bool", "byte", "int",
				"short", "float", "double", "long", "void", "thread",
				"native", "goto", "break", "continue", "true", "false",
				"null", "indexer", "gotostate", "static", "breakpoint",
				"property", "public", "private", "protected", "class", "struct",
				"engine", "editor", "this"
			};

		private bool _highlighting = false;

		private string _previousText = "";
		private bool _fullHighlightNext = false;

		private FindAndReplaceWindow _findAndReplaceWindow = null;

        private bool _requireHighlighting = true;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the text displayed in this control.
		/// </summary>
		public override string Text
		{
			get { return scriptRichTextBox.Text; }
			set { scriptRichTextBox.Text = value;  }
		}

		/// <summary>
		///		Gets or sets the context menu strip associated with this control.
		/// </summary>
		public override ContextMenuStrip ContextMenuStrip
		{
			get { return scriptRichTextBox.ContextMenuStrip; }
			set { scriptRichTextBox.ContextMenuStrip = value; }
		}

		/// <summary>
		///		Gets a boolean value describing if this control can preform a cut operation.
		/// </summary>
        [Browsable(false)]
		public bool CanCut
		{
			get { return scriptRichTextBox.SelectionLength > 0; }
		}

		/// <summary>
		///		Gets a boolean value describing if this control can preform a copy operation.
		/// </summary>
        [Browsable(false)]
		public bool CanCopy
		{
			get { return scriptRichTextBox.SelectionLength > 0; }
		}

		/// <summary>
		///		Gets a boolean value describing if this control can preform a paste operation.
		/// </summary>
        [Browsable(false)]
		public bool CanPaste
		{
			get { return scriptRichTextBox.CanPaste(DataFormats.GetFormat(DataFormats.Rtf)); }
		}

		/// <summary>
		///		Gets a boolean value describing if this control can preform an undo operation.
		/// </summary>
        [Browsable(false)]
		public bool CanUndo
		{
			get { return scriptRichTextBox.CanUndo; }
		}

		/// <summary>
		///		Gets a boolean value describing if this control can preform a redo operation.
		/// </summary>
        [Browsable(false)]
		public bool CanRedo
		{
			get { return scriptRichTextBox.CanRedo; }
		}

        /// <summary>
        ///     Gets or sets if this script is read only.
        /// </summary>
        public bool ReadOnly
        {
            get { return scriptRichTextBox.ReadOnly; }
            set { scriptRichTextBox.ReadOnly = value; }
        }

        /// <summary>
        ///     Gets the rich text box associated with this control.
        /// </summary>
        public RichTextBox RichTextBox
        {
            get { return scriptRichTextBox; }
        }

        /// <summary>
        ///     Gets or sets if the script shown in this control should be highlighted.
        /// </summary>
        public bool RequireHighlighting
        {
            get { return _requireHighlighting; }
            set { _requireHighlighting = value; }
        }

		#endregion
		#region events

		/// <summary>
		///		Invoked when text is changed in this control.
		/// </summary>
		public new event EventHandler TextChanged;

		/// <summary>
		///		Invoked when the selection is changed in this control.
		/// </summary>
		public event EventHandler SelectionChanged;

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Disposes of this control and any controls owned by it.
		/// </summary>
		public new void Dispose()
		{
			if (_findAndReplaceWindow != null) 
			{
				_findAndReplaceWindow.Dispose();
				_findAndReplaceWindow = null;	
			}		
		}

		/// <summary>
		///		Initializes this control.
		/// </summary>
		public ScriptTextBox()
		{
			InitializeComponent();
			UpdateLineLabel();
		}

		/// <summary>
		///		Shows a find and replace text form.
		/// </summary>
		public void ShowFindAndReplaceForm()
		{
			_findAndReplaceWindow = new FindAndReplaceWindow(scriptRichTextBox);
			_findAndReplaceWindow.Show();
		}

		/// <summary>
		///		Preforms a cut operation on this control
		/// </summary>
		public void Cut()
		{
			scriptRichTextBox.Cut();
		}

		/// <summary>
		///		Preforms a copy operation on this control
		/// </summary>
		public void Copy()
		{
			scriptRichTextBox.Copy();
		}

		/// <summary>
		///		Preforms a paste operation on this control
		/// </summary>	
		public void Paste()
		{
			scriptRichTextBox.Paste();
			Highlight(true);
		}

		/// <summary>
		///		Preforms a undo operation on this control
		/// </summary>
		public void Undo()
		{
			scriptRichTextBox.Undo();
			Highlight(false);
		}

		/// <summary>
		///		Preforms a redo operation on this control
		/// </summary>
		public void Redo()
		{
			scriptRichTextBox.Redo();
			Highlight(false);
		}

		/// <summary>
		///		Preforms basic syntax highlighting on the current script.
		/// </summary>
		/// <param name="full">If true the whole script will be highlighted, if false only the locally modified line will be.</param>
		public void Highlight(bool full)
		{
            if (_requireHighlighting == false) return;

			// Check we are not already highlighting
			if (_highlighting == true) return;

			int startIndex = 0;
			string text = scriptRichTextBox.Text;
			int finishIndex = text.Length;

			// If we are highlighting local changes then work out where to do it from.
			if (full == false)
			{
				startIndex = scriptRichTextBox.GetFirstCharIndexOfCurrentLine();
				if (startIndex < 0) return;
				int line = scriptRichTextBox.GetLineFromCharIndex(startIndex);
				if (line >= scriptRichTextBox.Lines.Length) return;
				finishIndex = startIndex + scriptRichTextBox.Lines[line].Length;
			}

			// Store the scroll position for later.
			NativeMethods.POINT scrollPosition = new NativeMethods.POINT();
			NativeMethods.SendMessage(new HandleRef(scriptRichTextBox, scriptRichTextBox.Handle), (int)NativeMethods.Win32Messages.EM_GETSCROLLPOS, 0, ref scrollPosition);

			// Store the original selection position and length.
			int selectionIndex = scriptRichTextBox.SelectionStart;
			int selectionLength = scriptRichTextBox.SelectionLength;
			int processStartIndex = 0;

			// Lock the script rich text box.
			NativeMethods.LockWindowUpdate((int)scriptRichTextBox.Handle);

			// Blank out the current selection.
			scriptRichTextBox.Select(startIndex, finishIndex - startIndex);
			scriptRichTextBox.SelectionColor = _normalColor;
			scriptRichTextBox.SelectionFont = _normalFont;

			// Check if the current text is inside a block comment, if
			// it is recolor the block.
			if (full == false)
			{
				string previousSub = (selectionIndex <= _previousText.Length - 2 && selectionIndex >= 2) ? _previousText.Substring(selectionIndex - 1, 2) : "";
				int blockStartIndex = text.LastIndexOf("/*", selectionIndex);
				if (blockStartIndex > -1)
				{
					int blockStartEndIndex = text.IndexOf("*/", blockStartIndex);
					int blockEndIndex = text.IndexOf("*/", selectionIndex);
					if (blockStartEndIndex > 0 && blockEndIndex > 0 && blockStartEndIndex >= blockEndIndex)
					{
						scriptRichTextBox.Select(blockStartIndex, (blockEndIndex - blockStartIndex) + 2);
						scriptRichTextBox.SelectionColor = _commentColor;
						scriptRichTextBox.SelectionFont = _commentFont;

						// As we are highlighting a block comment we don't want
						// to highlight this selection normally.
						startIndex = finishIndex = -1;
					}
				}
			}

			// Go through the characters we are highlighting and highlight them.
			_highlighting = true;
			for (int charIndex = startIndex; charIndex < finishIndex; charIndex++)
			{
				// See if there is a single line comment at this position.
				if (charIndex < text.Length - 1 && text[charIndex] == '/' && text[charIndex + 1] == '/')
				{
					processStartIndex = charIndex;
					while (charIndex < finishIndex)
					{
						if (text[charIndex] == '\n')
							break;
						charIndex++;
					}
					scriptRichTextBox.Select(processStartIndex, charIndex - processStartIndex);
					scriptRichTextBox.SelectionColor = _commentColor;
					scriptRichTextBox.SelectionFont = _commentFont;
				}

				// See if there is a block comment start at this position.
				else if (full == true && charIndex < finishIndex - 1 && text[charIndex] == '/' && scriptRichTextBox.Text[charIndex + 1] == '*')
				{
					processStartIndex = charIndex;
					charIndex += 2;
					int depth = 1;
					while (charIndex < text.Length - 1)
					{
						if (text[charIndex] == '*' && text[charIndex + 1] == '/')
						{
							depth--;
							if (depth == 0)
							{
								charIndex += 2;
								break;
							}
						}
						else if (text[charIndex] == '/' && text[charIndex + 1] == '*')
						{
							depth++;
						}
						charIndex++;
					}
					scriptRichTextBox.Select(processStartIndex, charIndex - processStartIndex);
					scriptRichTextBox.SelectionColor = _commentColor;
					scriptRichTextBox.SelectionFont = _commentFont;
				}

				// See if there is a preprocessor line at this position.
				else if (text[charIndex] == '#')
				{
					processStartIndex = charIndex;
					while (charIndex < finishIndex)
					{
						if (text[charIndex] == '\n')
							break;
						charIndex++;
					}
					scriptRichTextBox.Select(processStartIndex, charIndex - processStartIndex);
					scriptRichTextBox.SelectionColor = _directiveColor;
					scriptRichTextBox.SelectionFont = _directiveFont;
				}

				// See if there is a string value next.
				else if (text[charIndex] == '\"')
				{
					processStartIndex = charIndex++;
					while (charIndex < finishIndex)
					{
						if (text[charIndex] == '\"' || text[charIndex] == '\n')
						{
							charIndex++;
							break;
						}
						charIndex++;
					}
					scriptRichTextBox.Select(processStartIndex, charIndex - processStartIndex);
					scriptRichTextBox.SelectionColor = _stringColor;
					scriptRichTextBox.SelectionFont = _stringFont;
				}

				// See if there is a numeric value next.
				else if (text[charIndex] >= '0' && text[charIndex] <= '9')
				{
					processStartIndex = charIndex;
					while (charIndex < finishIndex)
					{
						if ((text[charIndex] < '0' || text[charIndex] > '9') && text[charIndex] != '.')
						{
							scriptRichTextBox.Select(processStartIndex, charIndex - processStartIndex);
							scriptRichTextBox.SelectionColor = _numberColor;
							scriptRichTextBox.SelectionFont = _numberFont;
							charIndex--;
							break;
						}
						charIndex++;
					}
				}

				// See if there is a identifier next.
				else if ((text[charIndex] >= 'a' && text[charIndex] <= 'z') || (text[charIndex] >= 'A' && text[charIndex] <= 'Z') || text[charIndex] == '_')
				{
					processStartIndex = charIndex;
					while (charIndex < finishIndex)
					{
						if (!((text[charIndex] >= 'a' && text[charIndex] <= 'z') || (text[charIndex] >= 'A' && text[charIndex] <= 'Z') || (text[charIndex] >= '0' && text[charIndex] <= '9') || text[charIndex] == '_'))
						{
							charIndex--;
							break;
						}
						charIndex++;
					}

					if (processStartIndex + (charIndex - processStartIndex + 1) >= text.Length) continue;
					string keywordText = text.Substring(processStartIndex, charIndex - processStartIndex + 1).ToLower();

					bool isKeyword = false;
					foreach (string keyword in _keywords)
						if (keywordText == keyword)
						{
							isKeyword = true;
							break;
						}

					if (isKeyword == true)
					{
						scriptRichTextBox.Select(processStartIndex, charIndex - processStartIndex + 1);
						scriptRichTextBox.SelectionColor = _keywordColor;
						scriptRichTextBox.SelectionFont = _keywordFont;
					}
				}

			}
			_highlighting = false;

			// Restore the original selection.
			scriptRichTextBox.SelectionStart = selectionIndex;
			scriptRichTextBox.SelectionLength = selectionLength;

			// Restore the scrool bar position
			NativeMethods.SendMessage(new HandleRef(scriptRichTextBox, scriptRichTextBox.Handle), (int)NativeMethods.Win32Messages.EM_SETSCROLLPOS, 0, ref scrollPosition);

			// Unlock the script rich text box.
			NativeMethods.LockWindowUpdate(0);

			// Save the previous text.
			_previousText = text;
		}

		/// <summary>
		///		Invoked when the text is changed in this control, forces the 
		///		re-highlighting of this controls text.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void scriptRichTextBox_TextChanged(object sender, EventArgs e)
		{
			if (_highlighting == true) return;
			Highlight(_fullHighlightNext);
			_fullHighlightNext = false;
			if (TextChanged != null) TextChanged(this, new EventArgs());
		}

		/// <summary>
		///		Invoked when the text selection is changed in this control, forces the 
		///		re-highlighting of this controls text.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void scriptRichTextBox_SelectionChanged(object sender, EventArgs e)
		{
			if (SelectionChanged != null) SelectionChanged(this, new EventArgs());
		}

		/// <summary>
		///		Invoked when the text is scrolled in this control, forces a refresh of the
		///		line strip.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void scriptRichTextBox_VScroll(object sender, EventArgs e)
		{
			UpdateLineLabel();
		}

		/// <summary>
		///		Invoked when this control is resized, forces a refresh of the
		///		line strip.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void scriptRichTextBox_SizeChanged(object sender, EventArgs e)
		{
			UpdateLineLabel();
		}

		/// <summary>
		///		Invoked when a key is pressed on this control, preforms operations on
		///		this controls text depending on the key.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void scriptRichTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				UpdateLineLabel();

			else if (e.KeyCode == Keys.Back)
			{
				char deletedChar = scriptRichTextBox.SelectionStart <= 0 ? (char)0 : _previousText[scriptRichTextBox.SelectionStart - 1];
				_fullHighlightNext = (deletedChar == '/' || deletedChar == '*');
			}
			else if (e.KeyCode == Keys.OemQuestion || (e.KeyCode == Keys.D8 && e.Shift == true))
			{
				_fullHighlightNext = true;
			}
		}

		/// <summary>
		///		Updates the line strip on the left hand side of the script rich text box.
		/// </summary>
		private void UpdateLineLabel()
		{
			int offset = scriptRichTextBox.GetPositionFromCharIndex(0).Y % (scriptRichTextBox.Font.Height + 1);
			lineRichTextBox.Location = new Point(0, offset);

			lineRichTextBox.Text = "";
			int startLine = scriptRichTextBox.GetLineFromCharIndex(scriptRichTextBox.GetCharIndexFromPosition(new Point(0, 0)));
			int endLine = scriptRichTextBox.GetLineFromCharIndex(scriptRichTextBox.GetCharIndexFromPosition(new Point(scriptRichTextBox.ClientSize.Width, scriptRichTextBox.ClientSize.Height)));
			string text = "";
			for (int line = startLine; line < endLine + 2; line++)
				text += (line + 1) + "\n";
			lineRichTextBox.Text = text;
		}
		 
		#endregion
	}
}
