/*
 * File: Input Window.cs
 *
 * Contains all the functional partial code declaration for the InputWindow form.
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
	///		an instance of the input window.
	/// </summary>
	public partial class InputWindow : Form
	{
		#region Members
		#region Variables

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the input being displayed by this window.
		/// </summary>
		public string Input
		{
			get { return inputTextBox.Text; }
			set { inputTextBox.Text = value; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Initializes a new instance of this class with the given data.
		/// </summary>
		/// <param name="title">Title of this window.</param>
		/// <param name="infomation">Brief description of why input is being asked for.</param>
		/// <param name="defaultInput">What value the input text box should contain when the window is shown.</param>
		/// <param name="password">If set to true the text inputed will be masked like a password.</param>
		public InputWindow(string title, string infomation, string defaultInput, bool password)
		{
			InitializeComponent();
			this.Text = title;
			infomationLabel.Text = infomation;
			inputTextBox.Text = defaultInput;
			if (password == true) inputTextBox.PasswordChar = '*';
		}

		#endregion
	}
}