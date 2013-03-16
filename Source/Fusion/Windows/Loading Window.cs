/*
 * File: Loading Window.cs
 *
 * The contains the code used to show and display a loading window while resources are being unpacked.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace BinaryPhoenix.Fusion
{

    /// <summary>
    ///     Our loading window class. Guess what this does?
    /// </summary>
    public partial class LoadingWindow : Form
    {
        #region Members
        #region Variables

        private ArrayList _logs = new ArrayList();

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
        ///     Sets the status of this loading window.
        /// </summary>
        /// <param name="text">Status to set.</param>
        public void SetStatus(string text)
        {
            loadingLabel.Text = text;
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

        #endregion
    }
}