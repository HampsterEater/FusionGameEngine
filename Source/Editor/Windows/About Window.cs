/*
 * File: About Window.cs
 *
 * Contains all the functional partial code declaration for the AboutWindow form.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */ 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

namespace BinaryPhoenix.Fusion.Editor.Windows
{

	/// <summary>
	///		This class contains the code used to display and operate  
	///		the about window.
	/// </summary>
	partial class AboutWindow : Form
	{
		#region Methods

		/// <summary>
		///		Initializes a new instance of this class and sets up all the 
		///		controls on this form.
		/// </summary>
		public AboutWindow()
		{
			InitializeComponent();

			// Set the text of the atttribute labels as that of the 
			// manifest stored in this assembly.
			object[] titleAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
			object[] productAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
			object[] copyrightAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
			object[] companyAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);

			this.Text = "About " + this.ProductName;
			this.productLabel.Text = this.ProductName;
			this.versionLabel.Text = "Version " + Editor.GlobalInstance.EngineConfigFile["editor:version", "1.0"];
			this.copyrightLabel.Text = ((AssemblyCopyrightAttribute)copyrightAttributes[0]).Copyright;
			this.companyLabel.Text = this.CompanyName;
		}

		/// <summary>
		///		Causes the system infomation program to start up if the user clicks the syste
		///		infomation button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void systemInfoButton_Click(object sender, EventArgs e)
		{
			// Boot up the msinfo32.exe program, which shows
			// the system infomation window.
			Process process = new Process();
			process.StartInfo.FileName = @"msinfo32";
            process.StartInfo.Verb = "Open";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            process.Start();
		}

		/// <summary>
		///		Closes this window if the users clicks the ok button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void okButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		#endregion
	}
}
