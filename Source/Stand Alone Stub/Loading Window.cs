/*
 * File: Loading Window.cs
 *
 * The contains the code used to show and display a loading window while resources are being unpacked.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace BinaryPhoenix.Fusion.StandAloneStub
{

    /// <summary>
    ///     Our loading window class. Guess what this does?
    /// </summary>
    public partial class LoadingWindow : Form
    {
        #region Members
        #region Variables

        private Thread _loadingThread;
        private string _tempFolder;
        private bool _progressError = false;
        private string _progressText = "";
        private int _progress = 0;
        private long _gameIDCode = 0;

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        public LoadingWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     This is used as the entry point for our loading thread.
        /// </summary>
        public void LoadThread()
        {
            try
            {
                // Open up the exe.
                Stream stream = new FileStream(Application.ExecutablePath, FileMode.Open, FileAccess.Read);
                BinaryReader reader = new BinaryReader(stream);
                stream.Seek(stream.Length - 16, SeekOrigin.Begin);

                // Read in the files.
                _gameIDCode = reader.ReadInt64();
                long offset = reader.ReadInt64();
                stream.Seek(offset, SeekOrigin.Begin);
                int fileCount = reader.ReadInt32();
                for (int i = 0; i < fileCount; i++)
                {
                    string url = reader.ReadString();
                    _progressText = "Unpacking " + Path.GetFileName(url) + "...";

                    int length = reader.ReadInt32();

                    byte[] fileBuffer = new byte[length];
                    stream.Read(fileBuffer, 0, length);

                    string fileDir = Path.GetDirectoryName(url);
                    if (fileDir != "" && Directory.Exists(_tempFolder + "\\" + fileDir) == false)
                        Directory.CreateDirectory(_tempFolder + "\\" + fileDir);

                    Stream fileStream = new FileStream(_tempFolder + "\\" + url, FileMode.OpenOrCreate);
                    fileStream.Write(fileBuffer, 0, length);
                    fileStream.Close();

                    _progress = (int)((100.0f / fileCount) * i);
                }

                // Close the exe.
                stream.Close();
            }
            catch (Exception e)
            {
                _progressError = true;
                MessageBox.Show("The following error occured while trying to unpack files;\n\n" + e.Message + "\n\n" + e.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _progressText = "An error occured while unpacking the files.";
            }
        }

        /// <summary>
        ///     Invoked when the link label is clciked.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Reasons why this event was invoked.</param>
        private void websiteLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process process = new Process();
            process.StartInfo.FileName = @"iexplore";
            process.StartInfo.Arguments = "www.binaryphoenix.com";
            process.StartInfo.Verb = "Open";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            process.Start();
        }

        /// <summary>
        ///     Starts up the loading thread when this window is shown.
        /// </summary>
        public void Show()
        {
            this.Visible = false;
            // Grab a couple of pieces of info from the end of the exe.
            Stream stream = new FileStream(Application.ExecutablePath, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);
            stream.Seek(stream.Length - 16, SeekOrigin.Begin);
            _gameIDCode = reader.ReadInt64();
            stream.Close();

            // Find a temporary folder to run from.
            _tempFolder = Path.GetTempPath() + "\\fusionSA("+_gameIDCode+")";
            if (Directory.Exists(_tempFolder) == false)
            {
                this.Visible = true;
                Directory.CreateDirectory(_tempFolder);

                _loadingThread = new Thread(LoadThread);
                _loadingThread.IsBackground = true;
                _loadingThread.Start();

                while (_loadingThread != null && _loadingThread.IsAlive == true && Disposing == false)
                {
                    Application.DoEvents();
                    if (_progressText != "")
                    {
                        loadingLabel.Text = _progressText;
                        _progressText = "";
                        progressBar.Value = _progress;
                        if (_progressError == true)
                            loadingLabel.ForeColor = Color.Red;
                    }
                }

                if (_progressError == true)
                {
                    Thread.Sleep(2000);
                    return;
                }
            }

            loadingLabel.Text = "Loaded, starting game ...";
            Thread.Sleep(2000);

            // Run the exe.
            Process process = new Process();
            process.StartInfo.Arguments = "-workdir:\"" + Environment.CurrentDirectory + "\"";
            process.StartInfo.FileName = _tempFolder + "\\fusion.exe";
            process.StartInfo.Verb = "Open";
            process.StartInfo.WorkingDirectory = _tempFolder;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            process.Start();
            Close();
        }

        #endregion
    }
}