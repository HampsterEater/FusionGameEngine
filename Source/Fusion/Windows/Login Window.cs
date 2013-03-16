/*
 * File: Login Window.cs
 *
 * Contains all the functional code required to show a login window.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */ 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Engine.Windows
{

    /// <summary>
    ///     This class contains the code to display and process a login window.
    /// </summary>
    public partial class LoginWindow : Form
    {
        #region Methods

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        public LoginWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Invoked when the register link is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void registerLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            if (registerWindow.ShowDialog(this) == DialogResult.OK)
                Close();
        }

        /// <summary>
        ///     Invoked when the lost password link is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void lostPasswordLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ResetPasswordWindow resetWindow = new ResetPasswordWindow();
            if (resetWindow.ShowDialog(this) == DialogResult.OK)
                Close();
        }

        /// <summary>
        ///     Invoked when the cancel button is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void cancelbutton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     Invoked when the login button is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void loginButton_Click(object sender, EventArgs e)
        {
            string password = passwordTextBox.Text;
            string username = usernameTextBox.Text;
            if (password == null || password.Length < 6 ||
                username == null || username.Length < 6)
            {
                MessageBox.Show("A password and username must be entered. Both must be equal to or longer than 6 characters", "Invalid Login", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                ProgressWindow progressWindow = new ProgressWindow("Logging in", "Verifying login");
                progressWindow.Marquee = true;
                progressWindow.Show();
                this.Enabled = false;

                FileDownloader downloader = new FileDownloader("http://www.binaryphoenix.com/index.php?action=fusionverifylogin&username=" + System.Web.HttpUtility.UrlEncode(username) + "&password=" + System.Web.HttpUtility.UrlEncode(password));
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
                if (response.ToLower() == "invalid")
                    MessageBox.Show("The login given was invalid. If you have lost your password please click the password reset link.", "Invalid Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (response.ToLower() == "valid")
                {
                    MessageBox.Show("Your login was successfull.", "Valid Login", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Place the password / username in the config file if we are to remember them.
                    Fusion.GlobalInstance.CurrentUsername = username;
                    Fusion.GlobalInstance.CurrentPassword = password;
                    Fusion.GlobalInstance.RememberUser = rememberMeCheckBox.Checked;
                  
                    Close();
                }
                else
                    MessageBox.Show("Unexpected value returned from server. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }

}