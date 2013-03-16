/*
 * File: Updater Window.cs
 *
 * Contains all the functional partial code declaration for the UpdaterWindow form.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Resources;

namespace BinaryPhoenix.Fusion.Updater.Windows
{

	/// <summary>
	///		This class contains the code used to display and operate  
	///		the fusion updater window.
	/// </summary>
	public partial class UpdaterWindow : Form
	{
		#region Members
		#region Variables

        private PakFile _pakFile = null;

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Initializes a new instance of this class.
		/// </summary>
		public UpdaterWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		///		Updates all the software owned by this user and out of date.
		/// </summary>
		private void UpdateSoftware()
		{
            if (Updater.GlobalInstance.CreateUpdate == false) // Install an update.
            {
                // Load the pak file.
                PakFile pakFile = new PakFile(Updater.GlobalInstance.UpdateFile);

                // Output to the correct folder.
                int resourceIndex = 0;
                foreach (PakResource resource in pakFile.Resources)
                {
                    if (Path.GetDirectoryName(resource.URL as string) != "" && !Directory.Exists(Path.GetDirectoryName(resource.URL as string)))
                        Directory.CreateDirectory(Path.GetDirectoryName(resource.URL as string));

                    installLabel.Text = "Unpacking \"" + (resource.URL as string) + "\"...";
                    installProgressBar.Value = (int)((100.0f / pakFile.Resources.Count) * resourceIndex);
                    resourceIndex++;
                    this.Refresh();

                    Stream resourceStream = resource.RequestResourceStream();
                    if (File.Exists(resource.URL as string) == false) File.Create(resource.URL as string);
                    Stream resourceFileStream = new FileStream(resource.URL as string, FileMode.Truncate, FileAccess.Write);

                    byte[] data = new byte[resourceStream.Length];
                    resourceStream.Read(data, 0, (int)resourceStream.Length);
                    resourceFileStream.Write(data, 0, data.Length);

                    resourceFileStream.Close();
                    resourceStream.Close();
                }

                // We're done, so allow the user to quit.
                installLabel.Text = "Installation complete";
                stepNameLabel.Text = "Updated";
                stepDescriptionLabel.Text = "The Fusion Game Engine has been updated.";
                finishButton.Enabled = true;
            }
            else // Create an update.
            {
                // Tell the we are creating one!.
                installLabel.Text = "Creating update";
                stepNameLabel.Text = "Creating Update";
                stepDescriptionLabel.Text = "A new update file for the Fusion engine is being built.";

                // Create a pak file to put everything into.
                _pakFile = new PakFile();

                // Add the system files.
                PakFile("Fusion.exe");
                if (Updater.GlobalInstance.BuildSDK == true) PakFile("Editor.exe");
                if (Updater.GlobalInstance.BuildSDK == true) PakFile("Stand Alone Stub.exe");
                PakFile("Graphics.dll");
                PakFile("Audio.dll");
                PakFile("Input.dll");
                PakFile("Engine.dll");
                PakFile("Runtime.dll");
                PakFile("Fusion.xml");

                // Pak the plugin folder.
                string[] files = Directory.GetFiles(Updater.GlobalInstance.EngineConfigFile["paths:pluginpath", ""], "*.*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                    PakFile(files[i]);

                // Update status.
                installLabel.Text = "Saving update";
                installProgressBar.Value = 50;

                // Save update.
                _pakFile.Save(Updater.GlobalInstance.UpdateFile);
            }

            // We be done!
            installProgressBar.Value = 100;
            installLabel.Text = "Completed";
            finishButton.Enabled = true;
		}

        /// <summary>
        ///     Paks a file into the main update pak file.
        /// </summary>
        /// <param name="file">File to pak.</param>
        private void PakFile(string file)
        {
            //System.Console.WriteLine("Packed: " + file);
            PakResource resource = new PakResource(_pakFile, file, 0, 0);
            resource.DataStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            _pakFile.AddResource(resource);
        }

        /// <summary>
        ///     Called when the window is shown. Mainly used to get the ball rolling.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void UpdaterWindow_Shown(object sender, EventArgs e)
        {
            UpdateSoftware();
        }

        /// <summary>
        ///     Called when the finish button is clicked. Closes the window.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void finishButton_Click(object sender, EventArgs e)
        {
            Close();
        }

		#endregion
	}

}