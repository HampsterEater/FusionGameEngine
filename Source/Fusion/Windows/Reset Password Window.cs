/*
 * File: Reset Password Window.cs
 *
 * Contains all the functional code required to show a reset password window.
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
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Engine.Windows
{

    /// <summary>
    ///     This class contains the code to display and process a reset password window.
    /// </summary>
    public partial class ResetPasswordWindow : Form
    {
        #region Methods

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        public ResetPasswordWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Invoked when the reset button is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void resetButton_Click(object sender, EventArgs e)
        {
            string email = emailTextBox.Text;
            if (email == null || !email.Contains("@"))
            {
                MessageBox.Show("A valid email must be entered.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                ProgressWindow progressWindow = new ProgressWindow("Reseting Password", "Contacting server");
                progressWindow.Marquee = true;
                progressWindow.Show();
                this.Enabled = false;

                FileDownloader downloader = new FileDownloader("http://www.binaryphoenix.com/index.php?action=fusionresetpassword&email=" + System.Web.HttpUtility.UrlEncode(email));
                downloader.Start();
                while (downloader.Complete == false)
                {
                    if (downloader.Error != "")
                    {
                        MessageBox.Show("An error occured while connecting to the server. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Application.DoEvents();
                }

                this.Enabled = true;
                progressWindow.Dispose();

                ASCIIEncoding encoding = new ASCIIEncoding();
                string response = encoding.GetString(downloader.FileData);
                if (response.ToLower() == "noaccount")
                    MessageBox.Show("No account exists with the email that your provided.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (response.ToLower() == "invalid")
                    MessageBox.Show("The email you provided was invalid.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (response.ToLower() == "valid")
                {
                    MessageBox.Show("An email has been sent to the email account we have in our database. It contains a link to a webpage you can reset your password at.", "Password Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }
                else
                    MessageBox.Show("Unexpected value returned from server. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        ///     Invoked when the cancel button is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion
    }

}