/* 
 * File: Ban Window.cs
 *
 * This source file contains the declaration of the ban window class which is used on network servers
 * to select options when banning a user.
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
    ///     This window is used to select options when banning a user.
    /// </summary>
    public partial class BanWindow : Form
    {
        #region Members
        #region Variables

        #endregion
        #region Properties

        /// <summary>
        ///     Gets if the account should be banned.
        /// </summary>
        public bool BanAccount
        {
            get { return banAccountCheckBox.Checked; }
        }

        /// <summary>
        ///     Gets if the IP should be banned.
        /// </summary>
        public bool BanIP
        {
            get { return banIPCheckBox.Checked; }
        }

        /// <summary>
        ///     Gets the time that this ban should expire.
        /// </summary>
        public DateTime BanExpiration
        {
            get { return expirationDateTimePicker.Value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        public BanWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Bans the user when the ban button is clicked.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Reason why this event was invoked.</param>
        private void banButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        ///     Cancels ban when the cancel button is clicked.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Reason why this event was invoked.</param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion
    }
}
