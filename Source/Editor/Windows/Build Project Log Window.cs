/*
 * File: Build Project Log Window.cs
 *
 * Contains all the functional partial code declaration for the BuildProjectLogWindow form.
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
    ///		the project building log window.
    /// </summary>
    public partial class BuildProjectLogWindow : Form
    {
        /// <summary>
        ///     Updates the sub task label and progress bar.
        /// </summary>
        /// <param name="progress">Percetage complete.</param>
        /// <param name="msg">Sub task name.</param>
        public void UpdateSubTaskProgress(int progress, string msg)
        {
            subTaskLabel.Text = msg;
            subTaskProgressBar.Value = progress;
            Refresh();
        }

        /// <summary>
        ///     Updates the task label and progress bar.
        /// </summary>
        /// <param name="progress">Percetage complete.</param>
        /// <param name="msg">Task name.</param>
        public void UpdateTaskProgress(int progress, string msg)
        {
            taskLabel.Text = msg;
            taskProgressBar.Value = progress;
            Refresh();
        }

        /// <summary>
        ///     Adds a new log to the log list view.
        /// </summary>  
        /// <param name="msg">Log to add.</param>
        public void AddLog(string msg)
        {
            logListView.Items.Add(msg);
            errorCounterLabel.Text = "Build Logs ( " + logListView.Items.Count + " logs )";
        }

        /// <summary>
        ///     Creates a new instance of this class.
        /// </summary>
        /// <param name="task">Starting task.</param>
        /// <param name="subTask">Starting sub task.</param>
        public BuildProjectLogWindow(string task, string subTask)
        {
            InitializeComponent();
            taskLabel.Text = task;
            subTaskLabel.Text = subTask;
        }

    }

}