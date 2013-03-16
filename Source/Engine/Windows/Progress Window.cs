/*
 * File: Progress Window.cs
 *
 * Contains all the functional partial code declaration for the ProgressWindow form.
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

namespace BinaryPhoenix.Fusion.Engine.Windows
{

	/// <summary>
	///		This class contains the code used to display and operate  
	///		a progress window.
	/// </summary>
	public partial class ProgressWindow : Form
    {
        #region Members
        #region Variables

        private string _title = "";
        private string _message = "";
        private int _progress = 0;
        private bool _marquee = false;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the title of this window.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; Title = value + "..."; }
        }

        /// <summary>
        ///     Gets or sets the progress message.
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { _message = value; subProcessLabel.Text = value + "..."; }
        }

        /// <summary>
        ///     Gets or sets the progress in percentage of this task.
        /// </summary>
        public int Progress
        {
            get { return _progress; }
            set { _progress = value; progressBar.Value = value; }
        }

        /// <summary>
        ///     Get or sets if this progress window should be displayed with a marquee bar.
        /// </summary>
        public bool Marquee
        {
            get { return _marquee; }
            set 
            { 
                _marquee = value; 
                progressBar.Style = _marquee == true ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous; 
            }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
		///		Initializes a new instance of this class and sets up all the 
		///		controls on this form.
		/// </summary>
		/// <param name="process">The name of the process that will be displayed on the titlebar.</param>
		/// <param name="subProcess">THe sub process currently being processed that will be displayed above the progress bar.</param>
		public ProgressWindow(string process, string subProcess)
		{
			// Initialize this form's components.
			InitializeComponent();

			// Set the process and sub process text.
			this.Text = process + "...";
			subProcessLabel.Text = subProcess + "...";
            this.TopMost = true;
		}
        public ProgressWindow()
        {
            // Initialize this form's components.
            InitializeComponent();
        }

		/// <summary>
		///		Updates the progress and process bar of this window.
		/// </summary>
		/// <param name="progress">How much progress has been had on this process.</param>
		/// <param name="subProcess">What process the progress window is currently working on.</param>
		public void UpdateProgress(int progress, string subProcess)
		{
			progressBar.Value = progress;
			subProcessLabel.Text = subProcess + "...";
			Refresh();
        }

        /// <summary>
        ///     Called when the hide button is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void hideButton_Click(object sender, EventArgs e)
        {
            Visible = false;
        }

        #endregion
    }
}