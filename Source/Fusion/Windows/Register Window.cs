/*
 * File: Register Window.cs
 *
 * Contains all the functional code required to show a register window.
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
    ///     This class contains the code to display and process a register window.
    /// </summary>
    public partial class RegisterWindow : Form
    {
        #region Methods

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        public RegisterWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Invoked when the checked state of the agree check box is changed.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void agreeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            registerButton.Enabled = agreeCheckBox.Checked;
        }

        /// <summary>
        ///     Invoked when the register button is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void registerButton_Click(object sender, EventArgs e)
        {
            string email = emailTextBox.Text;
            string displayName = displaynameTextBox.Text;
            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;
            string confirmPassword = confirmPasswordTextBox.Text;     

            if (email == null || !email.Contains("@"))
            {
                MessageBox.Show("A valid email must be entered.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (username == null || username.Length < 6)
            {
                MessageBox.Show("A valid username must be entered.", "Invalid Username", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (password.ToLower() != confirmPassword.ToLower())
            {
                MessageBox.Show("Your passwords do not match.", "Invalid Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (password.Length < 6)
            {
                MessageBox.Show("A valid password must be entered.", "Invalid Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                ProgressWindow progressWindow = new ProgressWindow("Registering", "Contacting server");
                progressWindow.Marquee = true;
                progressWindow.Show();
                this.Enabled = false;

                FileDownloader downloader = new FileDownloader("http://www.binaryphoenix.com/index.php?action=fusionregister&email=" + System.Web.HttpUtility.UrlEncode(email) + "&username=" + System.Web.HttpUtility.UrlEncode(username) + "&password=" + System.Web.HttpUtility.UrlEncode(password) + (displayName != "" ? ("&displayname=" + System.Web.HttpUtility.UrlEncode(displayName)) : ""));
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
                if (response.ToLower() == "usernameinuse")
                    MessageBox.Show("An account already exists with the username that your provided.", "Invalid Register", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (response.ToLower() == "emailinuse")
                    MessageBox.Show("An account already exists with the email that your provided.", "Invalid Register", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (response.ToLower() == "valid")
                {
                    MessageBox.Show("Your account has successfully been registered with the server and you have been logged in. We hope you like being a member of Binary Phoenix's community.", "Registration Successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                    Fusion.GlobalInstance.CurrentUsername = username;
                    Fusion.GlobalInstance.CurrentPassword = password;
                    return;
                }
                else
                    MessageBox.Show("Unexpected value returned from server. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///     Invoked when cancel button is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        #endregion
    }
}