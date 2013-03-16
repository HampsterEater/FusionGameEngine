/* 
 * File: Server Window.cs
 *
 * This source file contains the declaration of the server window class and all code regarding
 * running it.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Engine.Networking;
using BinaryPhoenix.Fusion.Runtime.Debug;

namespace BinaryPhoenix.Fusion.Engine.Windows
{

    /// <summary>
    ///		The server window class is a simple form that allows the user to view statistics of the server connection.
    /// </summary>
    public partial class ServerWindow : Form
    {
        #region Members
        #region Variables

        private Thread _startThread = null;

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        public ServerWindow()
        {
            // Initialize components.
            InitializeComponent();
            _startThread = Thread.CurrentThread;

            // Add all the log that have already been logged.
            foreach (DebugLog log in DebugLogger.Logs)
                LogRecieved(log);

            // Attach a listener to the log recieved event of the debug logger.
            DebugLogger.LogRecieved += new LogRecievedEventHandler(LogRecieved);
        }

        /// <summary>
        ///     Invoked by the update timer object. Simply updates the windows to refelect the most 
        ///     recent state of the server.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Reason why this event was invoked.</param>
        private void updateTimer_Tick(object sender, EventArgs e)
        {
            // Clear everything.
            string selectedPeer = "";
            if (peerListView.SelectedItems.Count != 0)
                selectedPeer = peerListView.SelectedItems[0].Text;
            peerListView.Items.Clear();

            // Fill it with all the current peers.
            foreach (NetworkClient client in NetworkManager.ClientList)
            {
                ListViewItem item = new ListViewItem();
                item.Text = client.ID.ToString();
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, (client.LoggedIn == true) ? client.AccountID.ToString() : "N/A"));
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, client.ConnectTime.DayOfWeek.ToString() + " at " + client.ConnectTime.Hour + ":" + client.ConnectTime.Minute + ":" + client.ConnectTime.Second));
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, client.DownRate.ToString()));
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, client.UpRate.ToString()));
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, client.Connection.Ping.ToString()));
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, client.Connection.BytesSent.ToString()));
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, client.Connection.BytesRecieved.ToString()));
                peerListView.Items.Add(item);

                // Is this the item we were previously selecting? Then select it.
                if (client.ID.ToString() == selectedPeer)
                    peerListView.SelectedIndices.Add(peerListView.Items.Count - 1);
            }
        }

        /// <summary>
        ///		Used as a callback for the LogRecieved event of the DebugLogger.
        ///		Adds the given log to the log list view.
        /// </summary>
        /// <param name="log">Log to add to log list view</param>
        private void LogRecieved(DebugLog log)
        {
            if (Thread.CurrentThread != _startThread) return;
            ListViewItem item = new ListViewItem();
            item.StateImageIndex = log.AlertLevel == LogAlertLevel.FatalError ? 2 : (int)log.AlertLevel;
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, log.Date.ToString()));
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, log.Message));
            consoleListView.Items.Add(item);
        }

        /// <summary>
        ///     Kicks a client from the network when the kick button is pressed.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Reason why this event was invoked.</param>
        private void kickPeerButton_Click(object sender, EventArgs e)
        {
            string selectedPeer = "";
            int clientID = 0;
            if (peerListView.SelectedItems.Count != 0)
                selectedPeer = peerListView.SelectedItems[0].Text;
            if (int.TryParse(selectedPeer, out clientID) == false)
                return;

            NetworkPacket packet = new NetworkPacket();
            packet.ID = "KICK".GetHashCode();

            foreach (NetworkClient client in NetworkManager.Connection.ClientList)
                if (client.ID == clientID)
                {
                    client.Connection.SendPacket(packet);
                    client.Connection.Close();
                }

            DebugLogger.WriteLog("Kicking client " + clientID + ".");
        }

        /// <summary>
        ///    Bans a client from the network when the kick button is pressed.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Reason why this event was invoked.</param>
        private void banPeerButton_Click(object sender, EventArgs e)
        {
            string selectedPeer = "";
            int clientID = 0;
            if (peerListView.SelectedItems.Count != 0)
                selectedPeer = peerListView.SelectedItems[0].Text;
            if (int.TryParse(selectedPeer, out clientID) == false)
                return;

            // Open ban window.
            BanWindow banWindow = new BanWindow();
            if (banWindow.ShowDialog(this) == DialogResult.OK)
            {
                NetworkPacket packet = new NetworkPacket();
                packet.ID = "BAN".GetHashCode();

                foreach (NetworkClient client in NetworkManager.Connection.ClientList)
                    if (client.ID == clientID)
                    {
                        // Send it the explanation packet.
                        client.Connection.SendPacket(packet);

                        // Create a ban.
                        NetworkBan ban = new NetworkBan("000.000.000.000", -1, banWindow.BanExpiration);
                        NetworkManager.BansList.Add(ban);

                        // Add its IP to the black list.
                        if (banWindow.BanIP == true)
                        {
                            string currentIP = client.Connection.Socket.RemoteEndPoint.ToString();
                            if (currentIP.IndexOf(":") >= 0) currentIP = currentIP.Substring(0, currentIP.IndexOf(":"));
                            ban.IP = currentIP;
                        }

                        // Ban its account (if its logged in).
                        if (banWindow.BanAccount == true && client.LoggedIn == true)
                            ban.AccountID = client.AccountID;

                        // Close its connection.
                        client.Connection.Close();
                    }

                DebugLogger.WriteLog("Banning client " + clientID + ".");
            }
        }

        /// <summary>
        ///     Executes the current console command on the local machine.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Reason why this event was invoked.</param>
        private void consoleExecuteLocalButton_Click(object sender, EventArgs e)
        {
            if (consoleTextBox.Text != "")
                Runtime.Console.Console.ProcessCommand(consoleTextBox.Text);
            consoleTextBox.Text = "";
        }

        /// <summary>
        ///     Executes the current console command on all remote machines.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Reason why this event was invoked.</param>
        private void consoleExecuteRemoteButton_Click(object sender, EventArgs e)
        {
            if (consoleClientIDCheckbox.Checked == true)
            {
                int clientID = 0;
                if (consoleClientIDTextBox.Text == "" || int.TryParse(consoleClientIDTextBox.Text, out clientID) == false) return;

                NetworkPacket packet = new NetworkPacket();
                packet.ID = "CONSOLE_COMMAND".GetHashCode();
                packet[0] = consoleTextBox.Text;

                foreach (NetworkClient client in NetworkManager.Connection.ClientList)
                    if (client.ID == clientID) client.Connection.SendPacket(packet);

                DebugLogger.WriteLog("Executing remote console command '" + consoleTextBox.Text + "' on client "+clientID+".");
                consoleTextBox.Text = "";
            }
            else
            {
                NetworkPacket packet = new NetworkPacket();
                packet.ID = "CONSOLE_COMMAND".GetHashCode();
                packet[0] = consoleTextBox.Text;
                NetworkManager.Connection.BroadcastPacket(packet);

                DebugLogger.WriteLog("Executing remote console command '" + consoleTextBox.Text + "'.");
                consoleTextBox.Text = "";
            }
        }

        #endregion
    }
}
