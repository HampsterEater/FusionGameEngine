/*
 * File: Game Explorer Window.cs
 *
 * Contains all the functional partial code declaration for the GameExplorer form.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */ 

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Resources;

namespace BinaryPhoenix.Fusion.Engine.Windows
{

    /// <summary>
    ///     This is the main games explorer class. It deals with created and processing the games explorer window.
    /// </summary>
    public partial class GamesExplorerWindow : Form
    {
        #region Members
        #region Variables

        private ArrayList _myGames = new ArrayList();
        private ImageList _myGamesImageList = new ImageList();

        private ArrayList _downloads = new ArrayList();
        private ArrayList _gamesLibrary = new ArrayList();
        private ImageList _gamesLibraryImageList = new ImageList();

        private ArrayList _newsItems = new ArrayList();
        private bool _connectedToNet = true;

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        public GamesExplorerWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Syncronizes the data displayed on this window.
        /// </summary>
        private void SyncronizeWindow()
        {
            if (Fusion.GlobalInstance.CurrentUsername != "" && _connectedToNet == true)
            {
                registerAccountToolStripMenuItem.Enabled = false;
                lostPasswordToolStripMenuItem.Enabled = false;
                loginToolStripMenuItem.Text = "Logout Of Account";
                loginToolStripMenuItem.Enabled = true;
                gameLibraryStatusLabel.Visible = !(_gamesLibrary.Count != 0);
                gameLibraryContainer.Visible = (_gamesLibrary.Count != 0);
                if (_gamesLibrary.Count == 0) gameLibraryStatusLabel.Text = "No games available.";
                Text = "Fusion Game Explorer - (Logged in as " + Fusion.GlobalInstance.CurrentUsername+")";
                refreshGamesLibraryToolStripMenuItem.Enabled = true;
            }
            else
            {
                loginToolStripMenuItem.Text = "Login To Account";
                loginToolStripMenuItem.Enabled = true;
                registerAccountToolStripMenuItem.Enabled = true;
                lostPasswordToolStripMenuItem.Enabled = true;
                gameLibraryStatusLabel.Visible = true;
                gameLibraryContainer.Visible = false;
                gameLibraryStatusLabel.Text = "Please login to view Binary Phoenix's game library.";
                refreshGamesLibraryToolStripMenuItem.Enabled = false;
            }

            myGamesStatusLabel.Visible = (_myGames.Count == 0);
            myGamesContainer.Visible = (_myGames.Count != 0);

            myGamesContainer.Panel2Collapsed = (myGamesListView.SelectedItems.Count == 0);
            gameLibraryContainer.Panel2Collapsed = (gameLibraryListView.SelectedItems.Count == 0);

            if (myGamesListView.SelectedItems.Count != 0)
            {
                GameDescription selectedGame = _myGames[myGamesListView.SelectedIndices[0]] as GameDescription;
                myGamesDescriptionTextBox.Text = selectedGame.Description;
                myGamesLanguageLabel.Text = "Language: " + selectedGame.Language;
                myGamesPlayButton.Visible = true;
                myGamesEditorButton.Visible = File.Exists("editor.exe");
                myGamesPlayersLabel.Text = "Players: " + selectedGame.Players;
                myGamesRatingLabel.Text = selectedGame.Rating;
                myGamesRequirementsLabel.Text = selectedGame.Requirements;
                myGamesTitleLabel.Text = selectedGame.Name;
                myGamesPublisherLabel.Text = selectedGame.Publisher;
                myGamesUpdateButton.Visible = false;

                // Is there an update available?
                foreach (GameDescription gamesLibraryDescription in _gamesLibrary)
                {
                    if (gamesLibraryDescription.Name == selectedGame.Name && float.Parse(gamesLibraryDescription.Version) > float.Parse(selectedGame.Version))
                    {
                        // Check we are not already being downloaded.
                        bool found = false;
                        foreach (DownloadItem download in _downloads)
                        {
                            if (download.GameDescription == selectedGame)
                            {
                                found = true;
                                break;
                            }
                        }
                        myGamesUpdateButton.Visible = (found == false);
                        break;
                    }
                }
            }

            if (gameLibraryListView.SelectedItems.Count != 0)
            {
                GameDescription selectedGame = _gamesLibrary[gameLibraryListView.SelectedIndices[0]] as GameDescription;
                gameLibraryDescriptionTextBox.Text = selectedGame.Description;
                gameLibraryLanguageLabel.Text = "Language: " + selectedGame.Language;
                gameLibraryPlayersLabel.Text = "Players: " + selectedGame.Players;
                gameLibraryRatingLabel.Text = selectedGame.Rating;
                gameLibraryRequirementsLabel.Text = selectedGame.Requirements;
                gameLibraryNameLabel.Text = selectedGame.Name;
                gameLibraryPublisherLabel.Text = selectedGame.Publisher;
                gameLibraryDownloadButton.Enabled = (selectedGame.Downloading != true);
                gameLibraryDownloadButton.Text = (selectedGame.Downloading ? "Downloading..." : "Download");

                // Do we already own the current game?
                foreach (GameDescription myGameDescription in _myGames)
                    if (myGameDescription.Name == selectedGame.Name && float.Parse(myGameDescription.Version) >= float.Parse(selectedGame.Version))
                    {
                        gameLibraryDownloadButton.Text = "Downloaded";
                        gameLibraryDownloadButton.Enabled = false;
                        break;
                    }
            }

            if (_downloads.Count != 0)
            {
                downloadsStatusLabel.Visible = false;
                downloadsContainer.Visible = true;
            }
            else
            {
                downloadsStatusLabel.Visible = true;
                downloadsStatusLabel.Text = "You are not currently downloading anything.";
                downloadsContainer.Visible = false;
            }

            if (_connectedToNet == false)
            {
                statusToolStripProgressBar.Style = ProgressBarStyle.Continuous;
                Text = "Fusion Games Explorer (Offline)";
                statusToolStripStatusLabel.BackColor = Color.Red;
                statusToolStripStatusLabel.Text = "Fusion was unable to connect to the internet.";
                newDownloadLabel.Text = "Unable to connect to the internet.";
                gameLibraryStatusLabel.Text = "Unable to connect to the internet.";
                registerAccountToolStripMenuItem.Enabled = false;
                loginToolStripMenuItem.Enabled = false;
            }
        }

        /// <summary>
        ///     Invoked when the window is first shown.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void GamesExplorerWindow_Shown(object sender, EventArgs e)
        {
            //this.Enabled = false;

            // Create a splash screen.
           // SplashScreenWindow splashScreen = new SplashScreenWindow();
           // splashScreen.Show();

            // Refresh the my game's panel.
            //splashScreen.LoadingMessage = "Loading games ... ";
            LoadMyGames();

            // Refresh the news
            //splashScreen.LoadingMessage = "Loading news ... ";
            if (Fusion.GlobalInstance.CurrentUsername != "")
                ValidateLogin();

            if (_connectedToNet == true)
            {
                // Check password is correct.
                //splashScreen.LoadingMessage = "Validating login ... ";
                RefreshNews();

                // Check for any updates and notify the user.
                //splashScreen.LoadingMessage = "Checking for updates ... ";
                CheckForUpdates();
            }

            // Refresh the games library panel.
            if (_connectedToNet)
            {
                //splashScreen.LoadingMessage = "Loading game library ... ";
                LoadGamesLibrary();
            }

            // Syncronize window.
            SyncronizeWindow();

            //splashScreen.Close();
            //this.Enabled = true;
            //this.Focus();
        }

        /// <summary>
        ///     Validates the current username and password.
        /// </summary>
        /// <returns>True if login is valid, else false.</returns>
        private bool ValidateLogin()
        {
            // See if we can connect to the internet.
            FileDownloader downloader = new FileDownloader("http://www.binaryphoenix.com/index.php?action=fusionverifylogin&username=" + System.Web.HttpUtility.UrlEncode(Fusion.GlobalInstance.CurrentUsername) + "&password=" + System.Web.HttpUtility.UrlEncode(Fusion.GlobalInstance.CurrentPassword));
            try
            {
                downloader.Start();
                statusToolStripStatusLabel.Text = "Validating login.";
                statusToolStripProgressBar.Style = ProgressBarStyle.Marquee;

                while (downloader.Complete == false)
                {
                    if (downloader.Error != "")
                        throw new Exception("An error occured while connecting to the server.");
                    Application.DoEvents();
                }

                statusToolStripProgressBar.Style = ProgressBarStyle.Continuous;
                statusToolStripStatusLabel.Text = "Validating login.";

                // Get the data.
                ASCIIEncoding encoding = new ASCIIEncoding();
                string response = encoding.GetString(downloader.FileData);

                if (response.ToLower() == "invalid")
                {
                    Fusion.GlobalInstance.CurrentPassword = "";
                    Fusion.GlobalInstance.CurrentUsername = "";
                    return false;
                }
                else if (response.ToLower() == "valid")
                    return true;
                else
                    MessageBox.Show("Unexpected value returned from server. Please login manually.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
               
                statusToolStripStatusLabel.Text = "Idle.";
            }
            catch (Exception)
            {
                _connectedToNet = false;
            }

            return false;
        }

        /// <summary>
        ///     Refreshs the new tab by downloading news ite from the
        ///     central server.
        /// </summary>
        private void RefreshNews()
        {
            // See if we can connect to the internet.
            FileDownloader downloader = new FileDownloader("http://www.binaryphoenix.com/index.php?action=fusionnews");
            try
            {
                downloader.Start();
                statusToolStripStatusLabel.Text = "Downloading news.";
                statusToolStripProgressBar.Style = ProgressBarStyle.Marquee;

                while (downloader.Complete == false)
                {
                    if (downloader.Error != "")
                        throw new Exception("An error occured while connecting to the server.");
                    Application.DoEvents();
                }

                statusToolStripProgressBar.Style = ProgressBarStyle.Continuous;
                statusToolStripStatusLabel.Text = "Downloaded news.";

                // Get the data.
                ASCIIEncoding encoding = new ASCIIEncoding();
                string data = encoding.GetString(downloader.FileData);

                // Ok now parse the data.
                if (data.Length != 4 && data.ToLower() != "none")
                {
                    XmlDocument document = new XmlDocument();
                    document.LoadXml(data);
                    if (document["news"] != null)
                    {
                        int y = 15;
                        for (int i = 0; i < document["news"].ChildNodes.Count; i++)
                        {
                            string title = document["news"].ChildNodes[i]["title"].InnerText;
                            string author = document["news"].ChildNodes[i]["author"].InnerText;
                            string date = document["news"].ChildNodes[i]["date"].InnerText;
                            string body = document["news"].ChildNodes[i]["body"].InnerText;

                            NewsItem item = new NewsItem(title, "written by " + author + " on " + date, body, recentNewsTabPage);
                            item.NewsPanel.Location = new Point(15, y);
                            _newsItems.Add(item);

                            y += item.NewsPanel.Height + 15;
                        }

                        statusToolStripStatusLabel.Text = "Idle.";
                        newDownloadLabel.Visible = false;
                    }
                    else
                    {
                        BinaryPhoenix.Fusion.Runtime.Debug.DebugLogger.WriteLog("Unable to parse news XML, the root node dosen't exist.");
                        statusToolStripStatusLabel.Text = "An error occured while parsing news XML.";
                        newDownloadLabel.Text = "Unable to download.";
                    }
                }
            }
            catch (Exception)
            {
                _connectedToNet = false;
            }
        }

        /// <summary>
        ///     Checks for new updates with the central server.
        /// </summary>
        private void CheckForUpdates()
        {
            // Wrok out the major / minor versions.
            string version = Engine.GlobalInstance.EngineConfigFile["version", "1.0"];
            int radixIndex = version.IndexOf('.');
            string majorVersion = radixIndex == -1 ? "1" : version.Substring(0, radixIndex);
            string minorVersion = radixIndex == -1 ? "0" : version.Substring(radixIndex + 1);

            // See if we can connect to the internet.
            FileDownloader downloader = new FileDownloader("http://www.binaryphoenix.com/index.php?action=fusionupdates&mjrver="+majorVersion+"&minver="+minorVersion);
            try
            {
                downloader.Start();
                statusToolStripStatusLabel.Text = "Checking for updates.";
                statusToolStripProgressBar.Style = ProgressBarStyle.Marquee;

                while (downloader.Complete == false)
                {
                    if (downloader.Error != "")
                        throw new Exception("An error occured while connecting to the server.");
                    Application.DoEvents();
                }


                statusToolStripProgressBar.Style = ProgressBarStyle.Continuous;
                statusToolStripStatusLabel.Text = "Downloaded updates.";

                // Get the data.
                ASCIIEncoding encoding = new ASCIIEncoding();
                string data = encoding.GetString(downloader.FileData);

                // Ok now parse the data.
                if (data.Length != 4 && data.ToLower() != "none")
                {
                    try
                    {
                        XmlDocument document = new XmlDocument();
                        document.LoadXml(data);

                        if (document["updates"] != null)
                        {
                            if (MessageBox.Show("There are updates available for the Fusion Game Engine, would you like to download the updates now?", "Updates Available", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                            {
                                for (int i = 0; i < document["updates"].ChildNodes.Count; i++)
                                {
                                    string name = document["updates"].ChildNodes[i]["name"].InnerText;
                                    string file = document["updates"].ChildNodes[i]["file"].InnerText;
                                    AddDownload(name, file, DownloadType.FusionUpdate, null);
                                }
                                SyncronizeWindow();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        BinaryPhoenix.Fusion.Runtime.Debug.DebugLogger.WriteLog("Unable to parse updates XML, the root node dosen't exist.");
                        statusToolStripStatusLabel.Text = "An error occured while parsing updates XML.";
                    }
                }

                statusToolStripStatusLabel.Text = "Idle.";
            }
            catch (Exception)
            {
                _connectedToNet = false;
            }
        }

        /// <summary>
        ///     Downloads the games library from the central server.
        /// </summary>
        private void LoadGamesLibrary()
        {
            // Make sure we are logged in.
            if (Fusion.GlobalInstance.CurrentUsername == "" || Fusion.GlobalInstance.CurrentPassword == "")
                return;

            // See if we can connect to the internet.
            FileDownloader downloader = new FileDownloader("http://www.binaryphoenix.com/index.php?action=fusiongameslist&username=" + System.Web.HttpUtility.UrlEncode(Fusion.GlobalInstance.CurrentUsername) + "&password=" + System.Web.HttpUtility.UrlEncode(Fusion.GlobalInstance.CurrentPassword));
#if RELEASE || PROFILE
            try
            {
#endif
                downloader.Start();
                gameLibraryStatusLabel.Text = "Downloading Games Library ...";
                statusToolStripStatusLabel.Text = "Downloading Games Library ...";
                statusToolStripProgressBar.Style = ProgressBarStyle.Marquee;

                while (downloader.Complete == false)
                {
                    if (downloader.Error != "")
                        throw new Exception("An error occured while connecting to the server.");
                    Application.DoEvents();
                }

                statusToolStripProgressBar.Style = ProgressBarStyle.Continuous;
                statusToolStripStatusLabel.Text = "Downloaded Games Library.";

                // Get the data.
                ASCIIEncoding encoding = new ASCIIEncoding();
                string data = encoding.GetString(downloader.FileData);

                // Clear up.
                _gamesLibrary.Clear();
                _gamesLibraryImageList.Images.Clear();
                _gamesLibraryImageList.ColorDepth = ColorDepth.Depth32Bit;
                _gamesLibraryImageList.ImageSize = new Size(48, 48);

                // Ok now parse the data.
                if (data.Length != 4 && data.ToLower() != "none")
                {
#if RELEASE || PROFILE
                    try
                    {
#endif
                        XmlDocument document = new XmlDocument();
                        document.LoadXml(data);

                        if (document["games"] != null)
                        {
                            for (int i = 0; i < document["games"].ChildNodes.Count; i++)
                            {
                                string title = document["games"].ChildNodes[i]["title"].InnerText;
                                string version = document["games"].ChildNodes[i]["version"].InnerText;
                                string language = document["games"].ChildNodes[i]["language"].InnerText;
                                string shortdescription = document["games"].ChildNodes[i]["shortdescription"].InnerText;
                                string description = document["games"].ChildNodes[i]["description"].InnerText;
                                string rating = document["games"].ChildNodes[i]["rating"].InnerText;
                                string players = document["games"].ChildNodes[i]["players"].InnerText;
                                string requirements = document["games"].ChildNodes[i]["requirements"].InnerText;
                                string publisher = document["games"].ChildNodes[i]["publisher"].InnerText;

                                // Download the icon.
                                FileDownloader iconDownloader = new FileDownloader("http://www.binaryphoenix.com/index.php?action=fusiongamedownload&username=" + System.Web.HttpUtility.UrlEncode(Fusion.GlobalInstance.CurrentUsername) + "&password=" + System.Web.HttpUtility.UrlEncode(Fusion.GlobalInstance.CurrentPassword) + "&game=" + System.Web.HttpUtility.UrlEncode(title) + "&file=0");
                                try
                                {
                                    iconDownloader.Start();
                                    while (iconDownloader.Complete == false)
                                    {
                                        if (iconDownloader.Error != "")
                                            break; // Not important enough for an error message.
                                        Application.DoEvents();
                                    }
                                }
                                catch {}

                                Stream iconDataStream = new MemoryStream(iconDownloader.FileData);
                                Image iconImage = Image.FromStream(iconDataStream);
                                //iconDataStream.Close();

                                GameDescription gameDescription = new GameDescription("", title, description, shortdescription, rating, language, requirements, players, version, publisher);
                                gameDescription.Icon = iconImage;
                                _gamesLibrary.Add(gameDescription);

                                _gamesLibraryImageList.Images.Add(iconImage);
                            }
                            SyncronizeWindow();
                            statusToolStripStatusLabel.Text = "Idle.";
                        }
#if RELEASE || PROFILE
                    }
                    catch (Exception)
                    {
                        BinaryPhoenix.Fusion.Runtime.Debug.DebugLogger.WriteLog("Unable to parse game library XML, the root node dosen't exist.");
                        statusToolStripStatusLabel.Text = "An error occured while parsing game library XML.";
                    }
#endif
                }
#if RELEASE || PROFILE
            }
            catch (Exception)
            {
                _connectedToNet = false;
            }
#endif

            // Is there an update available for any of our games?
            foreach (GameDescription myGameDescription in _myGames)
                foreach (GameDescription gamesLibraryDescription in _gamesLibrary)
                    if (gamesLibraryDescription.Name == myGameDescription.Name && float.Parse(gamesLibraryDescription.Version) > float.Parse(myGameDescription.Version))
                        if (MessageBox.Show("There are updates available for "+myGameDescription.Name+", would you like to download the updates now?", "Updates Available", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            gamesLibraryDescription.Downloading = true;
                            AddDownload(gamesLibraryDescription.Name + " (Version " + gamesLibraryDescription.Version + ")", "http://www.binaryphoenix.com/index.php?action=fusiongamedownload&username=" + System.Web.HttpUtility.UrlEncode(Fusion.GlobalInstance.CurrentUsername) + "&password=" + System.Web.HttpUtility.UrlEncode(Fusion.GlobalInstance.CurrentPassword) + "&game=" + System.Web.HttpUtility.UrlEncode(gamesLibraryDescription.Name) + "&file=1", DownloadType.Game, gamesLibraryDescription);
                        }
        }

        /// <summary>
        ///     Refreshs the games library tab.
        /// </summary>
        private void RefreshGamesLibrary()
        {
           // gameLibraryListView.Clear();
            gameLibraryListView.SmallImageList = _gamesLibraryImageList;
            gameLibraryListView.Items.Clear();
            int iconIndex = 0;
            foreach (GameDescription description in _gamesLibrary)
            {
                gameLibraryListView.Items.Add(new ListViewItem(new string[] { "", description.Name, description.Publisher, description.Version, description.Rating, description.ShortDescription }, iconIndex));
                if (description.Icon != null)
                    iconIndex++;
            }
            SyncronizeWindow();
        }

        /// <summary>
        ///     Loads in details on all games that are on the local drive.
        /// </summary>
        private void LoadMyGames()
        {
            _myGames.Clear();
            _myGamesImageList.Images.Clear();
            _myGamesImageList.ColorDepth = ColorDepth.Depth32Bit;
            _myGamesImageList.ImageSize = new Size(48, 48);

            string[] directories = Directory.GetDirectories(Fusion.GlobalInstance.BasePath);
            for (int i = 0; i < directories.Length; i++)
            {
                string gameDir = directories[i];
                string gameName = gameDir.Substring(gameDir.LastIndexOf('\\') + 1);
                if (File.Exists(gameDir + "\\" + gameName + ".xml") == false)
                    continue;

                // Grab data out of the config file.
                XmlConfigFile configFile = new XmlConfigFile(gameDir + "\\" + gameName + ".xml");
                GameDescription description = new GameDescription(gameName, configFile["title", "Unknown"], configFile["description", "Unknown"], configFile["shortDescription", "Unknown"], configFile["rating", "Unknown"], configFile["language", "Unknown"], configFile["requirements", "Unknown"], configFile["players", "Unknown"], configFile["version", "Unknown"], configFile["publisher", "Unknown"]);

                // Load an icon?
                string icon = configFile["icon", ""];
                if (icon != "" && File.Exists(gameDir + "\\" + icon))
                {
                    byte[] data = File.ReadAllBytes(gameDir + "\\" + icon);
                    Stream iconStream = new MemoryStream(data);
                    description.Icon = Image.FromStream(iconStream);
                    //iconStream.Close();

                    _myGamesImageList.Images.Add(description.Icon);
                }

                _myGames.Add(description);
            }

            myGamesStatusLabel.Text = "You currently don't own any games.";

            RefreshMyGames();
        }

        /// <summary>
        ///     Refreshs the my games tab.
        /// </summary>
        private void RefreshMyGames()
        {
            myGamesListView.Items.Clear();
            myGamesListView.SmallImageList = _myGamesImageList;
            int iconIndex = 0;
            foreach (GameDescription description in _myGames)
            {
                myGamesListView.Items.Add(new ListViewItem(new string[] {"", description.Name, description.Publisher, description.Version, description.Rating, description.ShortDescription }, iconIndex));
                if (description.Icon != null)
                    iconIndex++;
            }
            SyncronizeWindow();
        }

        /// <summary>
        ///     Called when the exit menu item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        ///     Called when the contents menu item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\help.chm") == false)
            {
                MessageBox.Show("Unable to locate help file, please make sure that the file \"Help.chm\" is inside the same directory as this executable.", "Unable to locate file");
                return;
            }

            // Boot up the help page.
            Process process = new Process();
            process.StartInfo.WorkingDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            process.StartInfo.FileName = "help.chm";
            process.StartInfo.Verb = "Open";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            process.Start();
        }

        /// <summary>
        ///     Called when the visit website menu item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void visitWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Boot up internet explorer to the binary phoenix page.
            Process process = new Process();
            process.StartInfo.FileName = @"iexplore";
            process.StartInfo.Arguments = "www.binaryphoenix.com";
            process.StartInfo.Verb = "Open";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            process.Start();
        }

        /// <summary>
        ///     Called when the about menu item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog(this);
        }

        /// <summary>
        ///     Called when the register account menu item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void registerAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.ShowDialog(this);
            SyncronizeWindow();
        }

        /// <summary>
        ///     Called when the lost password menu item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void lostPasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetPasswordWindow registerWindow = new ResetPasswordWindow();
            registerWindow.ShowDialog(this);
        }

        /// <summary>
        ///     Called when the login account menu item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Fusion.GlobalInstance.CurrentUsername == "")
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.ShowDialog(this);

                LoadGamesLibrary();
                RefreshGamesLibrary();
            }
            else
            {
                Fusion.GlobalInstance.CurrentPassword = "";
                Fusion.GlobalInstance.CurrentUsername = "";
            }
            SyncronizeWindow();
        }

        /// <summary>
        ///     Called when the selected item in the my games list view is changed.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void myGamesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            SyncronizeWindow();
        }

        /// <summary>
        ///     Called when the selected item in the games library list view is changed.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void gameLibraryListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            SyncronizeWindow();
        }

        /// <summary>
        ///     Called when the update button in the my games tab is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void myGamesUpdateButton_Click(object sender, EventArgs e)
        {
            GameDescription selectedGame = _myGames[myGamesListView.SelectedIndices[0]] as GameDescription;
            foreach (GameDescription game in _gamesLibrary)
            {
                if (game.Name.ToLower() == selectedGame.Name.ToLower())
                {
                    AddDownload(game.Name + " (Version " + game.Version + ")", "http://www.binaryphoenix.com/index.php?action=fusiongamedownload&username=" + System.Web.HttpUtility.UrlEncode(Fusion.GlobalInstance.CurrentUsername) + "&password=" + System.Web.HttpUtility.UrlEncode(Fusion.GlobalInstance.CurrentPassword) + "&game=" + System.Web.HttpUtility.UrlEncode(game.Name) + "&file=1", DownloadType.Game, game);
                    return;
                }
            }
            SyncronizeWindow();
        }

        /// <summary>
        ///     Called when the play button in the my games tab is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void myGamesPlayButton_Click(object sender, EventArgs e)
        {
            if (myGamesListView.SelectedItems.Count != 0)
            {
                GameDescription selectedGame = _myGames[myGamesListView.SelectedIndices[0]] as GameDescription;

                Process process = new Process();
                process.StartInfo.FileName = Application.ExecutablePath;
                process.StartInfo.Arguments = "-game:\"" + selectedGame.FolderName + "\"";
                process.StartInfo.Verb = "Open";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                process.Start();

                this.WindowState = FormWindowState.Minimized;
                while (process.HasExited == false)
                {
                    if (this.WindowState != FormWindowState.Minimized)
                        break;
                    Application.DoEvents();
                }
                this.WindowState = FormWindowState.Normal;
            }
        }

        /// <summary>
        ///     Called when the run editor button in the my games tab is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void myGamesEditorButton_Click(object sender, EventArgs e)
        {
            if (myGamesListView.SelectedItems.Count != 0 && File.Exists("editor.exe"))
            {
                GameDescription selectedGame = _myGames[myGamesListView.SelectedIndices[0]] as GameDescription;

                Process process = new Process();
                process.StartInfo.FileName = "editor.exe";
                process.StartInfo.Arguments = "-game:\"" + selectedGame.FolderName + "\"";
                process.StartInfo.Verb = "Open";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                process.Start();

                this.WindowState = FormWindowState.Minimized;
                while (process.HasExited == false)
                {
                    if (this.WindowState != FormWindowState.Minimized)
                        break;
                    Application.DoEvents();
                }
                this.WindowState = FormWindowState.Normal;
            }
        }

        /// <summary>
        ///     Called when the download button in the games library tab is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void gameLibraryDownloadButton_Click(object sender, EventArgs e)
        {
            GameDescription selectedGame = _gamesLibrary[gameLibraryListView.SelectedIndices[0]] as GameDescription;
            selectedGame.Downloading = true;
            AddDownload(selectedGame.Name + " (Version " + selectedGame.Version + ")", "http://www.binaryphoenix.com/index.php?action=fusiongamedownload&username=" + System.Web.HttpUtility.UrlEncode(Fusion.GlobalInstance.CurrentUsername) + "&password=" + System.Web.HttpUtility.UrlEncode(Fusion.GlobalInstance.CurrentPassword) + "&game=" + System.Web.HttpUtility.UrlEncode(selectedGame.Name) + "&file=1", DownloadType.Game, selectedGame);
        }

        /// <summary>
        ///     Called when the download timer ticks.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void downloadTimer_Tick(object sender, EventArgs e)
        {
            downloadTimer.Enabled = false; // Disable it so that we don't recieve notifications while we are trying to proess this one.
            int totalTimeRemaining = 0;
            int totalSpeed = 0;
            int workingDownloads = 0;

            foreach (DownloadItem download in _downloads)
            {
                if (download.Downloader.Error != "")
                {
                    MessageBox.Show("An error occured while downloading " + download.Downloader.URL + "\n\n\t '" + download.Downloader.Error + "'.", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    download.ListViewItem.StateImageIndex = 3;
                    download.ListViewItem.SubItems[2].Text = "Error";
                    download.ListViewItem.SubItems[3].Text = "Error";
                    download.ListViewItem.SubItems[4].Text = "Error";
                    continue;
                }
                else if (download.Downloader.Complete == true)
                {
                    if (download.ListViewItem.StateImageIndex != 1 && download.DownloadType == DownloadType.FusionUpdate && MessageBox.Show("Updates have been downloaded, would you like to install them now?\n\nThis will require this application to restart.", "Updates Downloaded", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        download.Install();
                    else if (download.ListViewItem.StateImageIndex != 1 && download.ListViewItem.StateImageIndex != 3 && download.DownloadType == DownloadType.Game)
                    {
                        download.ListViewItem.StateImageIndex = 3;
                        download.ListViewItem.SubItems[2].Text = "Installing";
                        download.ListViewItem.SubItems[3].Text = "Installing";
                        download.ListViewItem.SubItems[4].Text = "Installing";
                        download.Install();
                        while (download.Installing == true)
                            Application.DoEvents();

                        LoadMyGames();
                        RefreshMyGames();
                    }

                    download.ListViewItem.StateImageIndex = 1;
                    download.ListViewItem.SubItems[2].Text = "Complete";
                    download.ListViewItem.SubItems[3].Text = "Complete";
                    download.ListViewItem.SubItems[4].Text = "Complete";

                    continue;
                }

                totalTimeRemaining += download.Downloader.TimeRemaining;
                totalSpeed += download.Downloader.AverageSpeed;
                workingDownloads++;

                int time = download.Downloader.TimeRemaining;
                download.ListViewItem.SubItems[2].Text = download.Downloader.AverageSpeed+"kb/s";
                download.ListViewItem.SubItems[3].Text = StringMethods.SizeFromBytes(download.Downloader.Size) + " / " + StringMethods.SizeFromBytes(download.Downloader.TotalSize) + " (" + download.Downloader.Progress + "%)";
                if (time > 60 * 60)
                    download.ListViewItem.SubItems[4].Text = (time / (60 * 60)) + " hours";
                else if (time > 60)
                    download.ListViewItem.SubItems[4].Text = (time / 60) + " minutes";
                else
                    download.ListViewItem.SubItems[4].Text = time + " seconds";
            }

            // Set the status text labels.
            if (workingDownloads > 0)
            {
                downloadSpeedLabel.Text = "Downloading " + workingDownloads + " files at " + (totalSpeed / workingDownloads) + " kb/s";
                int approxTime = totalTimeRemaining / workingDownloads;
                if (approxTime > 60 * 60)
                    downloadTimeLabel.Text = "Approximatly " + (approxTime / (60 * 60)) + " hours remaining";
                else if (approxTime > 60)
                    downloadTimeLabel.Text = "Approximatly " + (approxTime / 60) + " minutes remaining";
                else
                    downloadTimeLabel.Text = "Approximatly " + approxTime + " seconds remaining";
            }
            else
            {
                downloadSpeedLabel.Text = "";
                downloadTimeLabel.Text = "";
            }

            downloadTimer.Enabled = true; // Allow notifications again.
        }

        /// <summary>
        ///     Starts a new download item and adds it to the downloads page.
        /// </summary>
        /// <param name="name">Name of the download.</param>
        /// <param name="url">URL of file to download.</param>
        /// <param name="type">Type of download.</param>
        /// <param name="description">Game description, used in the case of game downloads.</param>
        private void AddDownload(string name, string url, DownloadType type, GameDescription description)
        {
            DownloadItem download = new DownloadItem(name, url, type, description);
            download.ListViewItem = new ListViewItem(new string[] { "", download.Name, "Unknown", "Unknown", "Unknown" });
            download.ListViewItem.StateImageIndex = 0;
            downloadsListView.Items.Add(download.ListViewItem);
            _downloads.Add(download);
            SyncronizeWindow();
        }

        /// <summary>
        ///     Called when a sub item of the downloads list view needs drawing.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void downloadsListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            DownloadItem itemDownload = null;
            foreach (DownloadItem download in _downloads)
                if (download.ListViewItem == e.Item)
                {
                    itemDownload = download;
                    break;
                }

            if (e.ColumnIndex == 3)
            {
                int width = (int)(((e.Bounds.Width - 4) / 100.0f) * (float)itemDownload.Downloader.Progress);

                // Draw selection backround if necessary.
                if ((e.ItemState & ListViewItemStates.Selected) != 0) e.Graphics.FillRectangle(SystemBrushes.Highlight, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));
                e.Graphics.DrawRectangle(new Pen(Color.Gray), new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 3, e.Bounds.Height - 3));
                e.Graphics.FillRectangle(Brushes.LightGray, new Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.Width - 4, e.Bounds.Height - 4));
                e.Graphics.FillRectangle(Brushes.LightGreen, new Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, width, e.Bounds.Height - 4));
                StringFormat format = new StringFormat();
                format.LineAlignment = StringAlignment.Center;
                format.Alignment = StringAlignment.Center;
                e.Graphics.DrawString(e.SubItem.Text, SystemFonts.MessageBoxFont, Brushes.Black, new RectangleF(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height), format);
            }
            else
                e.DrawDefault = true;
        }

        /// <summary>
        ///     Called when a header of the downloads list view needs drawing.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void downloadsListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        /// <summary>
        ///     Retrieves the currently selected download item on the downloads tab.
        /// </summary>
        /// <returns>Currently selected download item.</returns>
        private DownloadItem GetSelectedDownload()
        {
            if (downloadsListView.SelectedItems.Count > 0)
                foreach (DownloadItem download in _downloads)
                    if (download.ListViewItem == downloadsListView.SelectedItems[0])
                        return download;
            return null;
        }

        /// <summary>
        ///     Called when the selected item of the downloads list view is changed.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void downloadsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            DownloadItem selectedDownload = GetSelectedDownload();

            stopDownloadsButton.Enabled = (selectedDownload != null && selectedDownload.Downloader.Complete == false);
            if (selectedDownload != null)
            {
                if (selectedDownload.Downloader.Complete == false)
                {
                    if (selectedDownload.Downloader.Paused == true)
                    {
                        pauseDownloadsButton.Enabled = true;
                        pauseDownloadsButton.Text = "Resume";
                        pauseDownloadsButton.ImageIndex = 3;
                    }
                    else
                    {
                        pauseDownloadsButton.Enabled = true;
                        pauseDownloadsButton.Text = "Pause";
                        pauseDownloadsButton.ImageIndex = 2;
                    }
                }
                else
                {
                    pauseDownloadsButton.Enabled = false;
                }
                if (selectedDownload.DownloadType == DownloadType.FusionUpdate && selectedDownload.Downloader.Complete == true)
                    installDownloadsButton.Enabled = true;
            }
            else
            {
                pauseDownloadsButton.Enabled = false;
                installDownloadsButton.Enabled = false;
            }
        }

        /// <summary>
        ///     Called when the stop download button is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void stopDownloadsButton_Click(object sender, EventArgs e)
        {
            DownloadItem downloadItem = GetSelectedDownload();
            if (downloadItem != null && MessageBox.Show("Are you sure you wish to stop this download? This action is irreversible and any downloaded data will be lost.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                downloadItem.Downloader.Abort();
                downloadItem.Downloader = null;
                downloadsListView.Items.Remove(downloadItem.ListViewItem);
                downloadItem.ListViewItem = null;
                _downloads.Remove(downloadItem);
            }
        }

        /// <summary>
        ///     Called when the pause download button is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void pauseDownloadsButton_Click(object sender, EventArgs e)
        {
            DownloadItem downloadItem = GetSelectedDownload();
            if (downloadItem != null)
            {
                downloadItem.Downloader.Paused = !downloadItem.Downloader.Paused;
                if (downloadItem.Downloader.Paused == true)
                {
                    pauseDownloadsButton.Enabled = true;
                    pauseDownloadsButton.Text = "Resume";
                    pauseDownloadsButton.ImageIndex = 3;
                }
                else
                {
                    pauseDownloadsButton.Enabled = true;
                    pauseDownloadsButton.Text = "Pause";
                    pauseDownloadsButton.ImageIndex = 2;
                }
            }
        }

        /// <summary>
        ///     Called when the install download button is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void installDownloadsButton_Click(object sender, EventArgs e)
        {
            DownloadItem download = GetSelectedDownload();
            download.Install();
        }

        /// <summary>
        ///     Called when the check for updates menu ite is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _connectedToNet = true;
            CheckForUpdates();
        }

        /// <summary>
        ///     Called wehn the visit website menu item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void visitToolStripStatusLabel_Click(object sender, EventArgs e)
        {
            // Boot up internet explorer to the binary phoenix page.
            Process process = new Process();
            process.StartInfo.FileName = @"iexplore";
            process.StartInfo.Arguments = "www.binaryphoenix.com";
            process.StartInfo.Verb = "Open";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            process.Start();
        }

        /// <summary>
        ///     Called when the refresh news menu item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void refreshNewsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _connectedToNet = true;
            RefreshNews();
        }

        /// <summary>
        ///     Called when the refresh games library menu item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void refreshGamesLibraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _connectedToNet = true;
            LoadGamesLibrary();
            RefreshGamesLibrary();
        }

        /// <summary>
        ///     Called wehn the refresh my gaes menu item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void refreshMyGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _connectedToNet = true;
            LoadMyGames();
            RefreshMyGames();
        }

        /// <summary>
        ///     Called when the user attempts to close the window.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void GamesExplorerWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (DownloadItem download in _downloads)
            {
                if (download.Downloader.Complete == false && download.Downloader.Error == "")
                {
                    if (MessageBox.Show("You are currently downloading one or more files. If you quit now those downloads will be canceled.\n\nAre you sure you wish to quit?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        return;
                    }
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
        }

        #endregion
    }

    /// <summary>
    ///     This enum describes what a download item is downloading.
    /// </summary>
    public enum DownloadType
    {
        Game,
        FusionUpdate
    }

    /// <summary>
    ///     This class describes a currently downloading file.
    /// </summary>
    public class DownloadItem
    {
        #region Members
        #region Variables

        private string _name;
        private FileDownloader _downloader;
        private ListViewItem _listViewItem;
        private DownloadType _downloadType;
        private Thread _installGameThread = null;
        private GameDescription _gameDescription = null;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the name of this download.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        ///     Gets or sets the file downloader instance associated with this download.
        /// </summary>
        public FileDownloader Downloader
        {
            get { return _downloader; }
            set { _downloader = value; }
        }

        /// <summary>
        ///     Gets or sets the list view item that this download is being displayed with.
        /// </summary>
        public ListViewItem ListViewItem
        {
            get { return _listViewItem; }
            set { _listViewItem = value; }
        }

        /// <summary>
        ///     Gets or sets the type of item that is being downloaded.
        /// </summary>
        public DownloadType DownloadType
        {
            get { return _downloadType; }
            set { _downloadType = value; }
        }

        /// <summary>
        ///     Returns if this download is being installed.
        /// </summary>
        public bool Installing
        {
            get { return _installGameThread != null; }
        }

        /// <summary>
        ///     Gets or sets the description of the game being downloaded.
        /// </summary>
        public GameDescription GameDescription
        {
            get { return _gameDescription; }
            set { _gameDescription = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Installs this download.
        /// </summary>
        public void Install()
        {
            Runtime.Debug.DebugLogger.WriteLog("Installing \"" + _name + "\"...");

            if (_downloadType == DownloadType.FusionUpdate)
            {
                // DO update here! 
                Stream stream = StreamFactory.RequestStream(Fusion.GlobalInstance.DownloadPath + "\\" + Path.GetFileName(_downloader.URL), StreamMode.Truncate);
                stream.Write(_downloader.FileData, 0, _downloader.FileData.Length);
                stream.Close();

                if (File.Exists("Updater.exe") == false)
                {
                    MessageBox.Show("Unable to locate updater file, please make sure that the file \"Updater.exe\" is inside the same directory as this executable.", "Unable to locate file");
                    return;
                }
                else
                {
                    Process process = new Process();
                    process.StartInfo.FileName = "Updater.exe";
                    process.StartInfo.Arguments = "-file:\"" + Fusion.GlobalInstance.DownloadPath + "\\" + Path.GetFileName(_downloader.URL) + "\" -finishfile:\"Fusion.exe\"";
                    process.StartInfo.Verb = "Open";
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    process.Start();

                    Application.Exit();
                }
            }
            else if (_downloadType == DownloadType.Game)
            {
                _installGameThread = new Thread(InstallGameThread);
                _installGameThread.IsBackground = true;
                _installGameThread.Start();
            }
        }

        /// <summary>
        ///     Used as the entry point for the installation thread.
        /// </summary>
        private void InstallGameThread()
        {
            // Dump it to a file.
            Stream stream = StreamFactory.RequestStream(Fusion.GlobalInstance.DownloadPath + "\\" + _gameDescription.Name + ".pk", StreamMode.Truncate);
            stream.Write(_downloader.FileData, 0, _downloader.FileData.Length);
            stream.Close();

            // Load the pak file.
            PakFile pakFile = new PakFile(Fusion.GlobalInstance.DownloadPath + "\\" + _gameDescription.Name + ".pk");

            // Create a folder to dump the files into.
            string gameFolder = Fusion.GlobalInstance.BasePath + "\\" + _gameDescription.Name;
            if (!Directory.Exists(gameFolder))
                Directory.CreateDirectory(Fusion.GlobalInstance.BasePath + "\\" + _gameDescription.Name);

            // Output to the correct folder.
            foreach (PakResource resource in pakFile.Resources)
            {
                if (!Directory.Exists(gameFolder + "\\" + Path.GetDirectoryName(resource.URL as string)))
                    Directory.CreateDirectory(gameFolder + "\\" + Path.GetDirectoryName(resource.URL as string));

                Runtime.Debug.DebugLogger.WriteLog("Unpacking \""+(resource.URL as string)+"\"...");

                Stream resourceStream = resource.RequestResourceStream();
                Stream resourceFileStream = StreamFactory.RequestStream(gameFolder + "\\" + resource.URL, StreamMode.Truncate);

                byte[] data = new byte[resourceStream.Length];
                resourceStream.Read(data, 0, (int)resourceStream.Length);
                resourceFileStream.Write(data, 0, data.Length);

                resourceFileStream.Close();
                resourceStream.Close();
            }

            _installGameThread = null;
        }

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        /// <param name="name">Name of file to download.</param>
        /// <param name="url">URL of file to download.</param>
        /// <param name="type">Type of file being downloaded.</param>
        /// <param name="description">Description of game being downloaded.</param>
        public DownloadItem(string name, string url, DownloadType type, GameDescription description)
        {
            _name = name;
            _gameDescription = description;
            _downloadType = type;
            _downloader = new FileDownloader(url);
            _downloader.Start();
        }

        #endregion
    }

    /// <summary>
    ///     This class describes a game that is shown in either the games library or
    ///     my games tab.
    /// </summary>
    public class GameDescription
    {
        #region Members
        #region Variables

        private string _name;
        private string _description, _shortDescription;
        private string _rating;
        private string _language;
        private string _requirements;
        private string _players;
        private string _version;
        private string _publisher;
        private string _folderName;
        private Image _icon;
        private bool _downloading;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the icon associated with this game.
        /// </summary>
        public Image Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }

        /// <summary>
        ///     Gets or sets if this game is being downloaded.
        /// </summary>
        public bool Downloading
        {
            get { return _downloading; }
            set { _downloading = value; }
        }

        /// <summary>
        ///     Gets or sets the name of this game.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        ///     Gets or sets the name of the folder this game is stored in.
        /// </summary>
        public string FolderName
        {
            get { return _folderName; }
            set { _folderName = value; }
        }

        /// <summary>
        ///     Gets or sets the description of this game.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        ///     Gets or sets the short description of this game.
        /// </summary>
        public string ShortDescription
        {
            get { return _shortDescription; }
            set { _shortDescription = value; }
        }

        /// <summary>
        ///     Gets or sets the publisher of this game.
        /// </summary>
        public string Publisher
        {
            get { return _publisher; }
            set { _publisher = value; }
        }

        /// <summary>
        ///     Gets or sets the age rating of this game.
        /// </summary>
        public string Rating
        {
            get { return _rating; }
            set { _rating = value; }
        }

        /// <summary>
        ///     Gets  or sets the language of this game.
        /// </summary>
        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }

        /// <summary>
        ///     Gets or sets the requirements of this game.
        /// </summary>
        public string Requirements
        {
            get { return _requirements; }
            set { _requirements = value; }
        }

        /// <summary>
        ///     Gets or sets the player count of this game.
        /// </summary>
        public string Players
        {
            get { return _players; }
            set { _players = value; }
        }

        /// <summary>
        ///     Gets or sets the current version of this game.
        /// </summary>
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        /// <param name="folderName">Name of folder this game is stored in.</param>
        /// <param name="name">Nae of this game.</param>
        /// <param name="description">Description of this game.</param>
        /// <param name="shortDescription">Short description of this game.</param>
        /// <param name="rating">Age rating of this game.</param>
        /// <param name="language">Language of this game.</param>
        /// <param name="requirements">Requirements of this game.</param>
        /// <param name="players">Player count for this game.</param>
        /// <param name="version">Version of this game.</param>
        /// <param name="publisher">Publisher of this game.</param>
        public GameDescription(string folderName, string name, string description, string shortDescription, string rating, string language, string requirements, string players, string version, string publisher)
        {
            _folderName = folderName;
            _name = name;
            _description = description;
            _shortDescription = shortDescription;
            _rating = rating;
            _language = language;
            _requirements = requirements;
            _players = players;
            _version = version;
            _publisher = publisher;
        }

        #endregion
    }

    /// <summary>
    ///     This class describes a news item that has been downloaded from the central
    ///     server and is being displayed in the recent news tab.
    /// </summary>
    public class NewsItem
    {
        #region Members
        #region Variables

        private Control _parent = null;
        private Panel _newsPanel;
        private Label _titleLabel, _nameLabel;
        private RichTextBox _bodyBox = null;
        private string _title, _name, _body;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the panel this news item is being displayed on.
        /// </summary>
        public Panel NewsPanel
        {
            get { return _newsPanel; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Sets up the panel this news item is being displayed on.
        /// </summary>
        private void SetupPanel()
        {
            _titleLabel = new Label();
            _titleLabel.Parent = _newsPanel;
            _titleLabel.Text = _title;
            _titleLabel.AutoSize = false;
            _titleLabel.Location = new Point(22, 22);
            _titleLabel.Size = new Size(_parent.ClientRectangle.Width - 30 - 44, 25);
            _titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            _nameLabel = new Label();
            _nameLabel.Parent = _newsPanel;
            _nameLabel.Text = _name;
            _nameLabel.AutoSize = false;
            _nameLabel.Location = new Point(24, 47);
            _nameLabel.Size = new Size(_parent.ClientRectangle.Width - 30 - 44, 25);
            _nameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            _bodyBox = new RichTextBox();
            _bodyBox.ReadOnly = true;
            _bodyBox.Parent = _newsPanel;
            _bodyBox.Text = _body;
            _bodyBox.BackColor = Color.White;
            _bodyBox.BorderStyle = BorderStyle.None;
            _bodyBox.Location = new Point(22, 76);
            _bodyBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _bodyBox.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            _bodyBox.Size = new Size(_parent.ClientRectangle.Width - 30 - 44, 100);
          
            int height = 44 + _titleLabel.Font.Height + _nameLabel.Font.Height + 110;

            _newsPanel = new Panel();
            _newsPanel.Parent = _parent;
            _parent.Controls.Add(_newsPanel);
            _newsPanel.Location = new Point(15, 25);
            _newsPanel.Size = new Size(_parent.ClientRectangle.Width - 30, height);
            _newsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            _newsPanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            _newsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            _newsPanel.Controls.Add(_titleLabel);
            _newsPanel.Controls.Add(_nameLabel);
            _newsPanel.Controls.Add(_bodyBox);                       
        }

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        /// <param name="title">Title of this news item,</param>
        /// <param name="name">Name of this news item.</param>
        /// <param name="body">Body of this news item.</param>
        /// <param name="parent">Parent control of this news item.</param>
        public NewsItem(string title, string name, string body, Control parent)
        {
            _title = title;
            _parent = parent;
            _name = name;
            _body = body;
            SetupPanel();
        }

        #endregion
    }

}