/*
 * File: TipOfTheDay Window.cs
 *
 * Contains all the functional partial code declaration for the TipOfTheDayWindow form.
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
	///		the tip of the day window.
	/// </summary>
	public partial class TipOfTheDayWindow : Form
	{
		#region Members
		#region Variables

		private static string[] _tips = 
		{ 
		    "You can select multiple entities at once by holding the control button down and dragging the cursor over the entities you wish to select..",
		    "Shaders can be attached to entities in the properties window for per-pixel real time effects.",
		    "Did you know that Fusion is built on an engine which is designed to be completely extendable. No game layer code is written in native code, and plugins can be bound on a per-game basis."
		};

		private int _currentTip = 0;

		#endregion
		#region Properties

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Initializes a new instance of this class and sets up all the 
		///		controls on this form.
		/// </summary>
		public TipOfTheDayWindow()
		{
			// Initialize this form's components.
			InitializeComponent();

			// Setup the state of this window.
			Random rand = new Random(Environment.TickCount);
			_currentTip = rand.Next() % _tips.Length;
			if (_currentTip == 0) previousButton.Enabled = false;
			if (_currentTip == _tips.Length - 1) nextButton.Enabled = false;

			tipLabel.Text = _tips[_currentTip];
			showOnStartupCheckBox.Checked = Editor.GlobalInstance.EngineConfigFile["editor:showtipsonstartup", "1"] == "1" ? true : false;
		}

		/// <summary>
		///		Changes the text of the tip label to the next tip in the sequence.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void previousButton_Click(object sender, EventArgs e)
		{
			_currentTip--;
			previousButton.Enabled = !(_currentTip == 0);
			nextButton.Enabled = (_currentTip < _tips.Length - 1) ;
			tipLabel.Text = _tips[_currentTip];
		}

		/// <summary>
		///		Changes the text of the tip label to the previous tip in the sequence.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void nextButton_Click(object sender, EventArgs e)
		{
			_currentTip++;
			nextButton.Enabled = !(_currentTip == _tips.Length - 1);
			previousButton.Enabled = (_currentTip > 0);
			tipLabel.Text = _tips[_currentTip];
		}

		/// <summary>
		///		Closes this window if the close button is clicked. It also saves
		///		the view tips on startup checkbox's value to the xml engine configuration file. 
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void closeButton_Click(object sender, EventArgs e)
		{
			Editor.GlobalInstance.EngineConfigFile.SetSetting("editor:showtipsonstartup", showOnStartupCheckBox.Checked == true ? "1" : "0");
			Close();
		}

		#endregion
	}
}