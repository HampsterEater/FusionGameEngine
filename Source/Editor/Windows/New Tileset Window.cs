/*
 * File: New Tileset Window.cs
 *
 * Contains all the functional partial code declaration for the NewTilesetWindow form.
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

namespace BinaryPhoenix.Fusion.Editor.Windows
{

	/// <summary>
	///		This class contains the code used to display and operate  
	///		the new tileset dialog window.
	/// </summary>
	public partial class NewTilesetWindow : Form
	{
		#region Members
		#region Variables

		private string _file, _imageFile;
		private int _horizontalSize = 16, _verticalSize = 16;
		private int _horizontalSpace, _verticalSpace;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the path this tileset should be saved to.
		/// </summary>
		public string File
		{
			get { return _file; }
			set { _file = value; }
		}

		/// <summary>
		///		Gets or sets the path to the image file this tileset should use.
		/// </summary>
		public string ImageFile
		{
			get { return _imageFile; }
			set { _imageFile = value; }
		}

		/// <summary>
		///		Gets or sets the horizontal cell-size of the tileset image.
		/// </summary>
		public int HorizontalSize
		{
			get { return _horizontalSize; }
			set { _horizontalSize = value; }
		}

		/// <summary>
		///		Gets or sets the vertical cell-size of the tileset image.
		/// </summary>
		public int VerticalSize
		{
			get { return _verticalSize; }
			set { _verticalSize = value; }
		}

		/// <summary>
		///		Gets or sets the horizontal cell-spacing of the tileset image.
		/// </summary>
		public int HorizontalSpace
		{
			get { return _horizontalSpace; }
			set { _horizontalSpace = value; }
		}

		/// <summary>
		///		Gets or sets the vertical cell-spacing of the tileset image.
		/// </summary>
		public int VerticalSpace
		{
			get { return _verticalSpace; }
			set { _verticalSpace = value; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Initializes a new instance of this class and sets up all the 
		///		controls on this form.
		/// </summary>
		public NewTilesetWindow()
		{
			InitializeComponent();
		}


		/// <summary>
		///		Overrides the Form classes ShowDialog method so that 
		///		all of the control's values are reset before the dialog is shown.
		/// </summary>
		/// <returns>Result of showing this form.</returns>
		public new DialogResult ShowDialog()
		{
			tilesetFileTextBox.Text = _file;
			imageFileTextBox.Text = _imageFile;
			imageHorizontalSizeBox.Value = _horizontalSize;
			imageVerticalSizeBox.Value = _verticalSize;
			imageHorizontalSpaceBox.Value = _horizontalSpace;
			imageVerticalSpaceBox.Value = _verticalSpace;
			return base.ShowDialog();
		}

		/// <summary>
		///		Shows a file selection dialog to allow the user to select the file 
		///		this tileset should be saved to, when the file browse button is clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void tilesetFileBrowseButton_Click(object sender, EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.RestoreDirectory = true;
			dialog.Filter = "Tileset Declaration File|*.xml";
			dialog.InitialDirectory = Editor.GlobalInstance.TilesetPath;
			dialog.Title = "Selection Tileset File ...";
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				string file = dialog.FileName;

				if (file.ToLower().StartsWith(Editor.GlobalInstance.GamePath.ToLower() + "\\") == true)
					file = file.Substring(Editor.GlobalInstance.GamePath.Length + 1);

				if (file.ToLower().StartsWith(Editor.GlobalInstance.TilesetPath.ToLower()) == false)
				{
					MessageBox.Show("Tileset file must be within the tileset directory or a sub directory of it.", "Relative Path Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				tilesetFileTextBox.Text = file;
				_file = file;
			}
		}

		/// <summary>
		///		Shows a file selection dialog to allow the user to select the image file 
		///		this tileset should use, when the image file browse button is clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void imageFileBrowseButton_Click(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.RestoreDirectory = true;
			dialog.Filter = "Image Files|*.tga;*.bmp;*.png";
			dialog.InitialDirectory = Editor.GlobalInstance.GraphicPath;
			dialog.Title = "Selection Image File ...";
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				string file = dialog.FileName;

				if (file.ToLower().StartsWith(Editor.GlobalInstance.GamePath.ToLower() + "\\") == true)
					file = file.Substring(Editor.GlobalInstance.GamePath.Length + 1);

				if (file.ToLower().StartsWith(Editor.GlobalInstance.GraphicPath.ToLower()) == false)
				{
					MessageBox.Show("Image file must be within the graphics directory or a sub directory of it.", "Relative Path Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				imageFileTextBox.Text = file;
				_imageFile = file;
			}
		}

		/// <summary>
		///		Closes this window with a successfull dialog result, when the user clicks the ok button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void okButton_Click(object sender, EventArgs e)
		{
			if (_file == null || _imageFile == null || _file == "" || _imageFile == "" || _horizontalSize == 0 || VerticalSize == 0)
			{
				MessageBox.Show("Please fill in all required fields.", "Input Missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			DialogResult = DialogResult.OK;
			Close();
		}

		/// <summary>
		///		Closes this window with a cancel dialog result, when the user clicks the cancel button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void cancelButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		/// <summary>
		///		Retrieves data from the form when the user modifys the horizontal size box's value.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void imageHorizontalSizeBox_ValueChanged(object sender, EventArgs e)
		{
			_horizontalSize = (int)imageHorizontalSizeBox.Value;
		}

		/// <summary>
		///		Retrieves data from the form when the user modifys the vertical size box's value.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void imageVerticalSizeBox_ValueChanged(object sender, EventArgs e)
		{
			_verticalSize = (int)imageVerticalSizeBox.Value;
		}

		/// <summary>
		///		Retrieves data from the form when the user modifys the horizontal spacing box's value.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void imageHorizontalSpaceBox_ValueChanged(object sender, EventArgs e)
		{
			_horizontalSpace = (int)imageHorizontalSpaceBox.Value;
		}

		/// <summary>
		///		Retrieves data from the form when the user modifys the vertical spacing box's value.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void imageVerticalSpaceBox_ValueChanged(object sender, EventArgs e)
		{
			_verticalSpace = (int)imageVerticalSpaceBox.Value;
		}

		#endregion
	}
}