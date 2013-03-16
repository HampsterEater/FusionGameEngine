/*
 * File: Error Window.cs
 *
 * Contains all the functional partial code declaration for the ErrorWindow form.
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
    ///     Contains all the code required to create a form on which errors can be 
    ///     reported in a user friendly way.
    /// </summary>
    public partial class ErrorWindow : Form
    {
        #region Members
        #region Variables

        private string _errorMessage = "";

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the error message shown in this form.
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; messageTextBox.Text = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Called when a new instance of this class is created.
        /// </summary>
        public ErrorWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Called when the abort button is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void stopButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }

        /// <summary>
        ///     Called when the continue button is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void continueButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        #endregion
    }
}