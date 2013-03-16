namespace BinaryPhoenix.Fusion.Engine.Windows
{
    partial class ServerWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerWindow));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.banPeerButton = new System.Windows.Forms.Button();
            this.kickPeerButton = new System.Windows.Forms.Button();
            this.peerListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.consoleExecuteRemoteButton = new System.Windows.Forms.Button();
            this.consoleExecuteLocalButton = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.consoleTextBox = new System.Windows.Forms.TextBox();
            this.consoleListView = new System.Windows.Forms.ListView();
            this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader10 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader11 = new System.Windows.Forms.ColumnHeader();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.settingsPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.accountsListView = new System.Windows.Forms.ListView();
            this.deleteAccountButton = new System.Windows.Forms.Button();
            this.accountsPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.consoleImageList = new System.Windows.Forms.ImageList(this.components);
            this.consoleClientIDCheckbox = new System.Windows.Forms.CheckBox();
            this.consoleClientIDTextBox = new System.Windows.Forms.TextBox();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.listView1 = new System.Windows.Forms.ListView();
            this.button1 = new System.Windows.Forms.Button();
            this.columnHeader20 = new System.Windows.Forms.ColumnHeader();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Location = new System.Drawing.Point(8, 48);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(642, 379);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.banPeerButton);
            this.tabPage1.Controls.Add(this.kickPeerButton);
            this.tabPage1.Controls.Add(this.peerListView);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(634, 353);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Clients";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // banPeerButton
            // 
            this.banPeerButton.Location = new System.Drawing.Point(141, 318);
            this.banPeerButton.Name = "banPeerButton";
            this.banPeerButton.Size = new System.Drawing.Size(123, 29);
            this.banPeerButton.TabIndex = 2;
            this.banPeerButton.Text = "Ban";
            this.banPeerButton.UseVisualStyleBackColor = true;
            this.banPeerButton.Click += new System.EventHandler(this.banPeerButton_Click);
            // 
            // kickPeerButton
            // 
            this.kickPeerButton.Location = new System.Drawing.Point(6, 318);
            this.kickPeerButton.Name = "kickPeerButton";
            this.kickPeerButton.Size = new System.Drawing.Size(123, 29);
            this.kickPeerButton.TabIndex = 1;
            this.kickPeerButton.Text = "Kick";
            this.kickPeerButton.UseVisualStyleBackColor = true;
            this.kickPeerButton.Click += new System.EventHandler(this.kickPeerButton_Click);
            // 
            // peerListView
            // 
            this.peerListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader5,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8});
            this.peerListView.Dock = System.Windows.Forms.DockStyle.Top;
            this.peerListView.FullRowSelect = true;
            this.peerListView.HideSelection = false;
            this.peerListView.Location = new System.Drawing.Point(3, 3);
            this.peerListView.MultiSelect = false;
            this.peerListView.Name = "peerListView";
            this.peerListView.Size = new System.Drawing.Size(628, 309);
            this.peerListView.TabIndex = 0;
            this.peerListView.UseCompatibleStateImageBehavior = false;
            this.peerListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "ID";
            this.columnHeader1.Width = 30;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Account ID";
            this.columnHeader5.Width = 72;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Connect Time";
            this.columnHeader2.Width = 119;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Down Rate (b/s)";
            this.columnHeader3.Width = 99;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Up Rate (b/s)";
            this.columnHeader4.Width = 86;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Ping (ms)";
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Bytes Sent";
            this.columnHeader7.Width = 66;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Bytes Recieved";
            this.columnHeader8.Width = 89;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.consoleClientIDTextBox);
            this.tabPage2.Controls.Add(this.consoleClientIDCheckbox);
            this.tabPage2.Controls.Add(this.consoleExecuteRemoteButton);
            this.tabPage2.Controls.Add(this.consoleExecuteLocalButton);
            this.tabPage2.Controls.Add(this.label13);
            this.tabPage2.Controls.Add(this.consoleTextBox);
            this.tabPage2.Controls.Add(this.consoleListView);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(634, 353);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Console";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // consoleExecuteRemoteButton
            // 
            this.consoleExecuteRemoteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.consoleExecuteRemoteButton.Location = new System.Drawing.Point(523, 297);
            this.consoleExecuteRemoteButton.Name = "consoleExecuteRemoteButton";
            this.consoleExecuteRemoteButton.Size = new System.Drawing.Size(105, 23);
            this.consoleExecuteRemoteButton.TabIndex = 4;
            this.consoleExecuteRemoteButton.Text = "Execute Remote";
            this.consoleExecuteRemoteButton.UseVisualStyleBackColor = true;
            this.consoleExecuteRemoteButton.Click += new System.EventHandler(this.consoleExecuteRemoteButton_Click);
            // 
            // consoleExecuteLocalButton
            // 
            this.consoleExecuteLocalButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.consoleExecuteLocalButton.Location = new System.Drawing.Point(418, 297);
            this.consoleExecuteLocalButton.Name = "consoleExecuteLocalButton";
            this.consoleExecuteLocalButton.Size = new System.Drawing.Size(99, 23);
            this.consoleExecuteLocalButton.TabIndex = 3;
            this.consoleExecuteLocalButton.Text = "Execute Local";
            this.consoleExecuteLocalButton.UseVisualStyleBackColor = true;
            this.consoleExecuteLocalButton.Click += new System.EventHandler(this.consoleExecuteLocalButton_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 300);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(57, 13);
            this.label13.TabIndex = 2;
            this.label13.Text = "Command:";
            // 
            // consoleTextBox
            // 
            this.consoleTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.consoleTextBox.Location = new System.Drawing.Point(75, 297);
            this.consoleTextBox.Name = "consoleTextBox";
            this.consoleTextBox.Size = new System.Drawing.Size(337, 20);
            this.consoleTextBox.TabIndex = 1;
            // 
            // consoleListView
            // 
            this.consoleListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11});
            this.consoleListView.Dock = System.Windows.Forms.DockStyle.Top;
            this.consoleListView.LargeImageList = this.consoleImageList;
            this.consoleListView.Location = new System.Drawing.Point(3, 3);
            this.consoleListView.Name = "consoleListView";
            this.consoleListView.Size = new System.Drawing.Size(628, 288);
            this.consoleListView.SmallImageList = this.consoleImageList;
            this.consoleListView.StateImageList = this.consoleImageList;
            this.consoleListView.TabIndex = 0;
            this.consoleListView.UseCompatibleStateImageBehavior = false;
            this.consoleListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "";
            this.columnHeader9.Width = 25;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Time";
            this.columnHeader10.Width = 145;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Message";
            this.columnHeader11.Width = 449;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.settingsPropertyGrid);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(634, 353);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Settings";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // settingsPropertyGrid
            // 
            this.settingsPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsPropertyGrid.Location = new System.Drawing.Point(3, 3);
            this.settingsPropertyGrid.Name = "settingsPropertyGrid";
            this.settingsPropertyGrid.Size = new System.Drawing.Size(628, 347);
            this.settingsPropertyGrid.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.splitContainer1);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(634, 353);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Accounts";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.accountsListView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.deleteAccountButton);
            this.splitContainer1.Panel2.Controls.Add(this.accountsPropertyGrid);
            this.splitContainer1.Size = new System.Drawing.Size(628, 347);
            this.splitContainer1.SplitterDistance = 418;
            this.splitContainer1.TabIndex = 1;
            // 
            // accountsListView
            // 
            this.accountsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.accountsListView.Location = new System.Drawing.Point(0, 0);
            this.accountsListView.Name = "accountsListView";
            this.accountsListView.Size = new System.Drawing.Size(418, 347);
            this.accountsListView.TabIndex = 0;
            this.accountsListView.UseCompatibleStateImageBehavior = false;
            // 
            // deleteAccountButton
            // 
            this.deleteAccountButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteAccountButton.Location = new System.Drawing.Point(3, 316);
            this.deleteAccountButton.Name = "deleteAccountButton";
            this.deleteAccountButton.Size = new System.Drawing.Size(200, 28);
            this.deleteAccountButton.TabIndex = 1;
            this.deleteAccountButton.Text = "Delete Account";
            this.deleteAccountButton.UseVisualStyleBackColor = true;
            // 
            // accountsPropertyGrid
            // 
            this.accountsPropertyGrid.Dock = System.Windows.Forms.DockStyle.Top;
            this.accountsPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.accountsPropertyGrid.Name = "accountsPropertyGrid";
            this.accountsPropertyGrid.Size = new System.Drawing.Size(206, 313);
            this.accountsPropertyGrid.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.label11);
            this.tabPage4.Controls.Add(this.label10);
            this.tabPage4.Controls.Add(this.label9);
            this.tabPage4.Controls.Add(this.label8);
            this.tabPage4.Controls.Add(this.label7);
            this.tabPage4.Controls.Add(this.label6);
            this.tabPage4.Controls.Add(this.label5);
            this.tabPage4.Controls.Add(this.label4);
            this.tabPage4.Controls.Add(this.label3);
            this.tabPage4.Controls.Add(this.label2);
            this.tabPage4.Controls.Add(this.label1);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(634, 353);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Statistics";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(19, 143);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(113, 13);
            this.label11.TabIndex = 12;
            this.label11.Text = "Collisions Per Frame: 0";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(19, 130);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(82, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "Total Objects: 0";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(237, 31);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(121, 13);
            this.label9.TabIndex = 10;
            this.label9.Text = "Total Bytes Recieved: 0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(237, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(97, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "Total Bytes Sent: 0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(19, 100);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(102, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Current Map: NONE";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(237, 71);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(110, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Total Packets Sent: 0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(237, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(134, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Total Packets Recieved: 0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 71);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(145, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Packets Sent Per Second: 0 ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(169, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Packets Recieved Per Second: 0 ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Down Rate: 0 kb/s";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Up Rate: 0 kb/s";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(9, 9);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(267, 26);
            this.label12.TabIndex = 2;
            this.label12.Text = "This window permits the administration of online games \r\ndeveloped in the Fusion " +
                "Game Engine.";
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Interval = 1000;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // consoleImageList
            // 
            this.consoleImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("consoleImageList.ImageStream")));
            this.consoleImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.consoleImageList.Images.SetKeyName(0, "message.png");
            this.consoleImageList.Images.SetKeyName(1, "warning.png");
            this.consoleImageList.Images.SetKeyName(2, "error.png");
            // 
            // consoleClientIDCheckbox
            // 
            this.consoleClientIDCheckbox.AutoSize = true;
            this.consoleClientIDCheckbox.Location = new System.Drawing.Point(78, 324);
            this.consoleClientIDCheckbox.Name = "consoleClientIDCheckbox";
            this.consoleClientIDCheckbox.Size = new System.Drawing.Size(167, 17);
            this.consoleClientIDCheckbox.TabIndex = 5;
            this.consoleClientIDCheckbox.Text = "Only execute on client with ID";
            this.consoleClientIDCheckbox.UseVisualStyleBackColor = true;
            // 
            // consoleClientIDTextBox
            // 
            this.consoleClientIDTextBox.Location = new System.Drawing.Point(257, 321);
            this.consoleClientIDTextBox.Name = "consoleClientIDTextBox";
            this.consoleClientIDTextBox.Size = new System.Drawing.Size(155, 20);
            this.consoleClientIDTextBox.TabIndex = 6;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.button1);
            this.tabPage6.Controls.Add(this.listView1);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(634, 353);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "IP Blacklist";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader20});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(3, 3);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(628, 309);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 318);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(123, 29);
            this.button1.TabIndex = 2;
            this.button1.Text = "Delete";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // columnHeader20
            // 
            this.columnHeader20.Text = "IP Address";
            this.columnHeader20.Width = 101;
            // 
            // ServerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 434);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ServerWindow";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fusion Server Monitor";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage6.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button banPeerButton;
        private System.Windows.Forms.Button kickPeerButton;
        private System.Windows.Forms.ListView peerListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox consoleTextBox;
        private System.Windows.Forms.ListView consoleListView;
        private System.Windows.Forms.Button consoleExecuteLocalButton;
        private System.Windows.Forms.Button consoleExecuteRemoteButton;
        private System.Windows.Forms.PropertyGrid settingsPropertyGrid;
        private System.Windows.Forms.ListView accountsListView;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button deleteAccountButton;
        private System.Windows.Forms.PropertyGrid accountsPropertyGrid;
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ImageList consoleImageList;
        private System.Windows.Forms.TextBox consoleClientIDTextBox;
        private System.Windows.Forms.CheckBox consoleClientIDCheckbox;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader20;
    }
}