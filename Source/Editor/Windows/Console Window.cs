/*
 * File: Console Window.cs
 *
 * Contains all the functional partial code declaration for the ConsoleWindow form.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */ 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Engine;

namespace BinaryPhoenix.Fusion.Editor.Windows
{

	/// <summary>
	///		This class contains the code used to display and operate  
	///		the console window.
	/// </summary>
	public partial class ConsoleWindow : Form
    {
        #region Members
        #region Variables

        private Thread _startThread = null;

        #endregion
        #endregion
        #region Methods

        /// <summary>
		///		Initializes a new instance of this class and sets up all the 
		///		controls on this form.
		/// </summary>
		public ConsoleWindow()
		{
			// Initialize the controls on this window.
			InitializeComponent();
            _startThread = Thread.CurrentThread;

			// Add all the log that have already been logged.
			foreach (DebugLog log in DebugLogger.Logs)
				LogRecieved(log);

			// Attach a listener to the log recieved event of the debug logger.
			DebugLogger.LogRecieved += new LogRecievedEventHandler(LogRecieved);
		}

		/// <summary>
		///		Used as a callback for the LogRecieved event of the DebugLogger.
		///		Adds the given log to the log list view.
		/// </summary>
		/// <param name="log">Log to add to log list view</param>
		private void LogRecieved(DebugLog log)
		{
            if (Thread.CurrentThread != _startThread) return;
			ListViewItem item = new ListViewItem();
			item.StateImageIndex = log.AlertLevel == LogAlertLevel.FatalError ? 2 : (int)log.AlertLevel;
			item.SubItems.Add(new ListViewItem.ListViewSubItem(item, log.Date.ToString()));
			item.SubItems.Add(new ListViewItem.ListViewSubItem(item, log.Message));
			logListView.Items.Add(item);
		}

		/// <summary>
		///		Called when the user presses a key over the command text box. When
		///		it recieves a new-line character it processes the command and clears
		///		the command box's text.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void commandTextBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				logListView.TopItem = logListView.Items[logListView.Items.Count - 1];
				Runtime.Console.Console.ProcessCommand(commandTextBox.Text.Replace("\n", "").Replace("\r", ""));
				commandTextBox.Text = "";
			}
		}

		/// <summary>
		///		Stops the window from closing, but instead make it hide itself.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void ConsoleWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			Hide();
			e.Cancel = true;
		}

		#endregion
	}
}