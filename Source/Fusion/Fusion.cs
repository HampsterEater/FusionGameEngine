/*
 * File: Fusion.cs
 *
 * The entry point of the fusion game engine!
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using BinaryPhoenix.Fusion.Engine;
using BinaryPhoenix.Fusion.Engine.ScriptingFunctions;
using BinaryPhoenix.Fusion.Engine.Entitys;
using BinaryPhoenix.Fusion.Engine.Processes;
using BinaryPhoenix.Fusion.Engine.Networking;
using BinaryPhoenix.Fusion.Graphics;
//using BinaryPhoenix.Fusion.Graphics.Direct3D9Driver;
//using BinaryPhoenix.Fusion.Graphics.PixelMapFactorys;
using BinaryPhoenix.Fusion.Audio;
//using BinaryPhoenix.Fusion.Audio.DirectSound9Driver;
//using BinaryPhoenix.Fusion.Audio.SampleFactorys;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Languages;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Runtime.StreamFactorys;
using BinaryPhoenix.Fusion.Runtime.Events;
using BinaryPhoenix.Fusion.Runtime.Processes;
using BinaryPhoenix.Fusion.Runtime.Resources;
using BinaryPhoenix.Fusion.Runtime.Scripting;
using BinaryPhoenix.Fusion.Runtime.Collision;
using BinaryPhoenix.Fusion.Input;
//using BinaryPhoenix.Fusion.Input.DirectInput9Driver;
using BinaryPhoenix.Fusion.Engine.Windows;
using BinaryPhoenix.Fusion.Engine.Debug;
using BinaryPhoenix.Fusion.Engine.ConsoleFunctions;
using BinaryPhoenix.Fusion.Runtime.Console;

namespace BinaryPhoenix.Fusion.Engine
{

	/// <summary>
	///		Used as base for the actual game, contains all the 
	///		nitty gritty of running the engine.
	/// </summary>
	public class Fusion : Engine
	{
		#region Members
		#region Variables

		private static Fusion _globalInstance = null;
		private GameWindow _window = null;

        private GamesExplorerWindow _explorerWindow = null;
        private ServerWindow _serverWindow = null;

		private ScriptExecutionProcess _gameScriptProcess = null;
        private ScriptExecutionProcess _mapScriptProcess = null;

        private bool _mapLoadPending = false;
        private string _mapLoadFile = "", _mapLoadPassword = "";

        private string _currentUsername = "", _currentPassword = "";
        private bool _rememberUser = false;

        private Hashtable _gameFlags = new Hashtable();

        private bool _loading = false;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the currently running instance of the Editor class.
		/// </summary>
		public new static Fusion GlobalInstance
		{
			get { return _globalInstance; }
			set { _globalInstance = value; }
		}

        /// <summary>
        ///     If this returns true the game will not be updated this frame and time will be handed back to the OS.
        /// </summary>
        public override bool HandBackTimeToOS
        {
            get 
            {
                if (_isServer == false)
                {
                    Form window = _explorerWindow == null ? (Form)_window : (Form)_explorerWindow;
                    return (window != null && (window.WindowState == FormWindowState.Minimized));
                }
                return false;
           }
        }

        /// <summary>
        ///     Gets or sets the process that is executing the main game script.
        /// </summary>
        public ScriptExecutionProcess GameScriptProcess
        {
            get { return _gameScriptProcess; }
            set { _gameScriptProcess = value; }
        }

        /// <summary>
        ///     Gets or sets the process that is executing the current maps script.
        /// </summary>
        public ScriptExecutionProcess MapScriptProcess
        {
            get { return _mapScriptProcess; }
            set { _mapScriptProcess = value; }
        }

        /// <summary>
        ///     Gets or sets the window that everything is being rendered to.
        /// </summary>
        public GameWindow Window
        {
            get { return _window; }
            set { _window = value; }
        }

        /// <summary>
        ///     Gets or sets the users current BP password.
        /// </summary>
        public string CurrentPassword
        {
            get { return _currentPassword; }
            set { _currentPassword = value; }
        }

        /// <summary>
        ///     Gets or sets the users current BP username.
        /// </summary>
        public string CurrentUsername
        {
            get { return _currentUsername; }
            set { _currentUsername = value; }
        }

        /// <summary>
        ///     Gets or sets if the login of this user should be remembered.
        /// </summary>
        public bool RememberUser
        {
            get { return _rememberUser; }
            set { _rememberUser = value; }
        }

        /// <summary>
        ///     Gets or sets if this game is being shown in fullscreen.
        /// </summary>
        public bool Fullscreen
        {
            get { return _showInFullscreen; }
            set 
            {
                bool oldValue = _showInFullscreen;
                _showInFullscreen = value;

                if (_showInFullscreen != oldValue && _window != null && _gameName != "")
                    SetupGameWindow();
            }
        }

        /// <summary>
        ///     Gets or sets the game flags table.
        /// </summary>
        public Hashtable GameFlags
        {
            get { return _gameFlags; }
            set { _gameFlags = value; }
        }

        /// <summary>
        ///     Gets or sets if a map load is pending.
        /// </summary>
        public bool MapLoadPending
        {
            get { return _mapLoadPending; }
            set { _mapLoadPending = value; }
        }

		#endregion
		#endregion
		#region Methods

        /// <summary>
        ///     Sets up the game window, or if the window is already setup it 
        ///     refreshs it to take resolution changes into effect.
        /// </summary>
        protected void SetupGameWindow()
        {
            if (_window == null)
            {
                DebugLogger.WriteLog("Creating graphics window " + (_showInFullscreen == true ? "(Fullscreen)" : "(Windowed)") + " with size " + _realResolution[0] + "," + _realResolution[1] + "...");
                _window = new GameWindow(_realResolution[0], _realResolution[1], _showInFullscreen ? GraphicsFlags.FullScreen : 0);
                _window.FormClosing += new FormClosingEventHandler(OnClosing);
                _window.Text = _gameTitle;
                AudioManager.Driver.AttachToControl(_window);
            }
            else
            {
                _window.Reset(_realResolution[0], _realResolution[1], _showInFullscreen ? GraphicsFlags.FullScreen : 0);
            }

            // Set the resolution to the one in the game configuration file.
            DebugLogger.WriteLog("Emulating resolution at " + _resolution[0] + "x" + _resolution[1] + " ...");
            GraphicsManager.SetResolution(_resolution[0], _resolution[1], _keepAspectRatio);
        }

        /// <summary>
        ///     Keeps the loading window updated while the game is loading.
        /// </summary>
        protected void LoadingWindowThread()
        {
            // Create a loading window.
            LoadingWindow loadingWindow = new LoadingWindow();
            loadingWindow.Text = "Loading " + _gameTitle;
            loadingWindow.SetStatus("Loading "+_gameTitle+"...");
            loadingWindow.Show();

            while (_loading == true)
            {
                Application.DoEvents();
                Thread.Sleep(50);
            }

            loadingWindow.Close();
        }

		/// <summary>
		///		Called by the base engine class when its safe to begin initialization.
		/// </summary>
        protected override bool Begin()
        {
            Runtime.Debug.DebugLogger.WriteLog("Entered begin function", LogAlertLevel.Warning);
            // Create the fusion fuction set.
            new FusionFunctionSet();
            new NetworkFunctionSet();

            // Bind all function sets to the global virtual machine.
            NativeFunctionSet.RegisterCommandSetsToVirtualMachine(VirtualMachine.GlobalInstance);

            // Grab some config out of the games config file.
            ASCIIEncoding encoding = new ASCIIEncoding();
            _currentPassword = _engineConfigFile["account:password", ""];
            _currentUsername = _engineConfigFile["account:username", ""];

            // Make sure we have a game we can run.
            if (_gameName != "")
            {
                // Create the loading window thread.
                _loading = true;
                new Thread(LoadingWindowThread).Start();

                NetworkManager.IsServer = _isServer;

                if (_isServer == false)
                {
                    // Setup graphics window.
                    SetupGameWindow();
                }
                else
                {
                    // Setup graphics window. We are only actually doing this so the graphics and audio drivers have something to bind to.
                    // Notice that we are hiding it as soon as its made.
                    SetupGameWindow();
                    _window.Hide();

                    // Setup server window.
                    _serverWindow = new ServerWindow();
                    _serverWindow.FormClosing += new FormClosingEventHandler(OnClosing);
                    _serverWindow.Show();

                    // Setup server.
                    if (NetworkManager.Start() == false)
                    {
                        throw new Exception("An error occured while attempting to setup network.");
                    }
                }

                // Create some hooks into network events.
                NetworkManager.ClientConnected += new ClientConnectedEventHandler(ClientConnected);
                NetworkManager.ClientDisconnected += new ClientDisconnectedEventHandler(ClientDisconnected);
                NetworkManager.Disconnected += new DisconnectedEventHandler(Disconnected);
                NetworkManager.PacketRecieved += new PacketRecievedEventHandler(PacketRecieved);

                // If pak files are being used then load all of them in and register 
                // them with the resource manager
                if (_usePakFiles == true)
                {
                    ResourceManager.UsePakFiles = true;
                    DebugLogger.WriteLog("Looking for resources in pak files...");
                    foreach (string file in Directory.GetFiles(Environment.CurrentDirectory))
                        if (file.ToLower().EndsWith(".pk") == true)
                        {
                            DebugLogger.WriteLog("Looking for resources in \"" + Path.GetFileName(file) + "\"...");
                            PakFile pakFile = new PakFile(file);
                            ResourceManager.RegisterPakFile(pakFile);
                        }
                }

                // Load in the default tileset if it exists and add it to the tileset list.
                if (ResourceManager.ResourceExists(_tilesetPath + "\\default.xml") == true)
                {
                    DebugLogger.WriteLog("Found default tileset, loading...");
                    Tileset.AddToTilesetPool(new Tileset(_tilesetPath + "\\default.xml"));
                }

                // Load in the default font if it exists.
                if (ResourceManager.ResourceExists(_fontPath + "\\default.xml") == true)
                {
                    DebugLogger.WriteLog("Found default bitmap font, loading...");
                    GraphicsManager.DefaultBitmapFont = new BitmapFont(_fontPath + "\\default.xml");
                }

                // Load in the required language pack.
                string languageFile = _languagePath + "\\" + _language + ".xml";
                if (ResourceManager.ResourceExists(languageFile) == true)
                {
                    DebugLogger.WriteLog("Loading language pack for language " + _language + ".");
                    LanguageManager.LoadLanguagePack(_languagePath + "\\" + _language + ".xml", _language);
                }
                else
                   DebugLogger.WriteLog("Unable to find language pack for language " + _language + ".", LogAlertLevel.Error);

                // Register the console commands, incase we turn the console on later.
                new StatisticalConsoleCommands();
                new FusionConsoleCommands();
                new MapConsoleCommands();
                new SystemConsoleCommands();

                // Register all the command sets with the console.
                ConsoleCommandSet.RegisterCommandSets();

                // Shall we setup the graphical console?
                if (_showConsole == true && _isServer == false)
                {
                    // Enable the graphical console.
                    GraphicalConsole.Enabled = true;
                }
                
                // Check if the start script exists.
                if (ResourceManager.ResourceExists(_startScript) == true)
                {
                    // It does, w00t, lets create an execution process for it then.
                    _gameScriptProcess = new ScriptExecutionProcess(_startScript);
                    if (_gameScriptProcess.Process != null)
                    {
                        _gameScriptProcess.Priority = 1000000; // Always runs first, otherwise all hell breaks loose with the OnCreate events.
                        _gameScriptProcess.IsPersistent = true;
                        _gameScriptProcess.Process.OnStateChange += this.OnStateChange;
                        ProcessManager.AttachProcess(_gameScriptProcess);

                        OnStateChange(_gameScriptProcess.Process, _gameScriptProcess.Process.State);
                    }
                }

                // Pump out a GameBegin event.
                EventManager.FireEvent(new Event("game_begin", this, null));

                // Close the loading window.
                _loading = false;

                // Show the game window.
                if (_isServer == false)
                {
                    _window.Show();
                    _window.BringToFront();
                    _window.Activate();
                }
                else
                {
                    _serverWindow.Show();
                    _serverWindow.BringToFront();
                    _serverWindow.Activate();
                }
            }

            // Nope? Ok show the games explorer.
            else
            {
                _explorerWindow = new GamesExplorerWindow();
                _explorerWindow.FormClosing += new FormClosingEventHandler(OnClosing);
                _explorerWindow.Show();
            }

            return false;
        }

        /// <summary>
        ///     Invoked when a packet is recieved by the network manager.
        /// </summary>
        /// <param name="sender">Connection that packet was connected from.</param>
        /// <param name="packet">Packet that was recieved.</param>
        void PacketRecieved(object sender, NetworkPacket packet)
        {
            foreach (ScriptProcess process in VirtualMachine.GlobalInstance.Processes)
            {
                NetworkPacketScriptObject packetSO = new NetworkPacketScriptObject(packet);
                NetworkConnectionScriptObject connectionSO = new NetworkConnectionScriptObject(sender as NetworkConnection);
                foreach (ScriptThread thread in process.Threads)
                {
                    thread.PassParameter(connectionSO);
                    thread.PassParameter(packetSO);
                    thread.InvokeFunction("OnPacketRecieved", true, false);  
                }
            }   
        }
        
        /// <summary>
        ///     Invoked when the network managers connection is closed.
        /// </summary>
        /// <param name="sender">Connection that was disconnected.</param>
        void Disconnected(object sender)
        {
            foreach (ScriptProcess process in VirtualMachine.GlobalInstance.Processes)
            {
                NetworkConnectionScriptObject connectionSO = new NetworkConnectionScriptObject(sender as NetworkConnection);
                foreach (ScriptThread thread in process.Threads)
                {
                    thread.PassParameter(connectionSO);
                    thread.InvokeFunction("OnDisconnected", true, false);
                }
            }  
        }

        /// <summary>
        ///     Invoked when a client disconnects from the network manager.
        /// </summary>
        /// <param name="sender">Connection that client disconnected from.</param>
        /// <param name="client">Client that disconnected.</param>
        void ClientDisconnected(object sender, NetworkClient client)
        {
            foreach (ScriptProcess process in VirtualMachine.GlobalInstance.Processes)
            {
                NetworkClientScriptObject clientSO = new NetworkClientScriptObject(client);
                NetworkConnectionScriptObject connectionSO = new NetworkConnectionScriptObject(sender as NetworkConnection);
                foreach (ScriptThread thread in process.Threads)
                {
                    thread.PassParameter(connectionSO);
                    thread.PassParameter(clientSO);
                    thread.InvokeFunction("OnClientDisconnected", true, false);
                }
            }             
        }

        /// <summary>
        ///     Invoked when a client connects to the network manager.
        /// </summary>
        /// <param name="sender">Connection that client disconnected from.</param>
        /// <param name="client">Clent that connected.</param>
        void ClientConnected(object sender, NetworkClient client)
        {
            foreach (ScriptProcess process in VirtualMachine.GlobalInstance.Processes)
            {
                NetworkClientScriptObject clientSO = new NetworkClientScriptObject(client);
                NetworkConnectionScriptObject connectionSO = new NetworkConnectionScriptObject(sender as NetworkConnection);
                foreach (ScriptThread thread in process.Threads)
                {
                    thread.PassParameter(connectionSO);
                    thread.PassParameter(clientSO);
                    thread.InvokeFunction("OnClientConnected", true, false);
                }
            }               
        }

        /// <summary>
        ///     Called when the state of this entities script is changed.
        /// </summary>
        /// <param name="process">Process that had its state changed.</param>
        /// <param name="sate">New state.</param>
        public void OnStateChange(ScriptProcess process, StateSymbol state)
        {
            if (_isServer == true) return;

            if (_mapScriptProcess != null && process == _mapScriptProcess.Process)
                _window.MapScriptRenderFunction = state.FindSymbol("OnRender", SymbolType.Function) as FunctionSymbol;
            else if (_gameScriptProcess != null && process == _gameScriptProcess.Process)
                _window.GameScriptRenderFunction = state.FindSymbol("OnRender", SymbolType.Function) as FunctionSymbol;   
        }

		/// <summary>
		///		Called by the base engine class when its safe to update and render the scene.
		/// </summary>
        protected override void Update()
        {
            // Ignore everything else if we are not running a game.
            if (_gameName == "")
                return;

            //System.Console.WriteLine("Collective Time("+EntityNode.CollectiveCalls+" calls): "+EntityNode.CollectiveTime);
            //EntityNode.CollectiveTime = 0.0f;
            //EntityNode.CollectiveCalls = 0;
            //EntityNode.CollectiveTimer.Restart();

            // Time to load a map?
            if (_mapLoadPending == true)
            {
                // Pump out a MapFinish event.
                EventManager.FireEvent(new Event("map_finish", this, null));

                // Allow the processes some time to be notified of closing events.
                //EventManager.ProcessEvents();
                //ProcessManager.RunProcesses(1);

                // Tell the scripts that we are now loading the new map (so they can show a loadings screen).
                _gameScriptProcess.Process.InvokeFunction("OnLoadingBegin", true, true);

                // Don't want to render the scene graph as its currently being loaded >.<.
                bool priorSceneGraphRender = true;
                if (_isServer == false)
                {
                    priorSceneGraphRender = _window.RenderSceneGraph;
                    _window.RenderLoadingScreen = true;
                    _window.RenderSceneGraph = false;
                }

                // Keep track of time :).
                HighPreformanceTimer loadTimer = new HighPreformanceTimer();

                // Get starting memory.
                long startingMemory = GC.GetTotalMemory(true);

                if (GraphicsManager.ThreadSafe == false)
                {
                    // Load the new map.
                    DebugLogger.WriteLog("Loading map from " + _mapLoadFile + " " + (_mapLoadPassword != "" ? " with password " + _mapLoadPassword : "") + ".");
                    LoadMapThread();
                }
                else
                {
                    // Load the new map.
                    DebugLogger.WriteLog("Loading map from " + _mapLoadFile + " " + (_mapLoadPassword != "" ? " with password " + _mapLoadPassword : "") + ".");
                    Thread thread = new Thread(LoadMapThread);
                    thread.Priority = ThreadPriority.Highest;
                    // Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                    thread.IsBackground = true;
                    thread.Start();

                    // Ech, there has to be a better way than this. I hate thread safety >.>.
                    HighPreformanceTimer timer = new HighPreformanceTimer();
                    while (thread != null && thread.IsAlive == true)
                    {
                        // Track frame stats.
                        TrackFrameStatsBegin();

                        // Update the process.
                        //timer = new HighPreformanceTimer();
                        //_gameScriptProcess.Run(_deltaTime);
                        //_processProcessingDuration = (float)timer.DurationMillisecond;

                        // Update the graphical console.
                        if (_isServer == false) GraphicalConsole.Update(1.0f);

                        // Tell the window to render
                        if (_isServer == false)
                        {
                            timer.Restart();
                            GraphicsCanvas.RenderAll();
                            _window.Render();
                            _renderingDuration = (float)timer.DurationMillisecond;
                        }

                        // Update network.
                        NetworkManager.Poll();
                        
                        // Process application level events.
                        timer.Restart();
                        Application.DoEvents();
                        _applicationProcessingDuration = (float)timer.DurationMillisecond;

                        // Track frame stats.
                        TrackFrameStatsFinish();
                    }
                }

                // Invoke OnCreate events of scripted entities.
                foreach (SceneNode node in _map.SceneGraph.EnumerateNodes())
                {
                    if (node != null && node is ScriptedEntityNode && node.IsPersistent == false)
                        ((ScriptedEntityNode)node).ScriptProcess[0].InvokeFunction("OnCreate", true, false);
                }

                // Run the collision manager quickly so that we can sort out any unplesent problems.
                CollisionManager.ProcessCollisions();

                //Thread.CurrentThread.Priority = ThreadPriority.Normal;

                // We can render again! Oh holy days!
                if (_isServer == false)
                {
                    _window.RenderLoadingScreen = false;
                    _window.RenderSceneGraph = priorSceneGraphRender;
                }

                // Remove any old resource from the cache that haven't been used for 5 maps :P.
                ResourceManager.CollectGarbage(3);

                // Free up some space.
                GC.Collect();

                // And we are done!
                DebugLogger.WriteLog("Map loaded successfully in " + loadTimer.DurationMillisecond + "ms, " + (((GC.GetTotalMemory(false) - startingMemory) / 1024.0f) / 1024.0f) + "mb allocated during loading.");

                // Yay, loaded!
                _mapLoadPending = false;

                // Reset the delta time!
                _forcedDeltaTimeThisFrame = 1.0f;
            }
        }

        /// <summary>
        ///     Entry point of the map loading thread.
        /// </summary>
        protected void LoadMapThread()
        {
            // Ignore everything else if we are not running a game.
            if (_gameName == "")
                return;

            // Kill of old processes.
            ArrayList disposeList = new ArrayList();
            foreach (Process process in ProcessManager.Processes)
                if (process.IsPersistent == false) disposeList.Add(process);
            foreach (Process process in disposeList)
            {
                ProcessManager.DettachProcess(process);
                process.Dispose();
            }

            // Remove process / media  references from scripts?
            // Unneccessary?

            // Load the map.
            _map.Load(_mapLoadFile, _mapLoadPassword, null, true, true);

            // Remove the map path part.
            string mapScriptURL = Path.ChangeExtension(_mapLoadFile, ".fs");
            if (mapScriptURL.ToLower().StartsWith(_mapPath.ToLower() + "\\"))
                mapScriptURL = mapScriptURL.Substring(_mapPath.Length + 1);

            // Check if the map script exists.
            if (ResourceManager.ResourceExists(_mapScriptPath + "\\" + mapScriptURL) == true)
            {
                // It does, w00t, lets create an execution process for it then.
                _mapScriptProcess = new ScriptExecutionProcess(_mapScriptPath + "\\" + mapScriptURL);
                if (_mapScriptProcess.Process != null)
                {
                    _mapScriptProcess.Priority = 999999; // Always runs first, otherwise all hell breaks loose with the OnCreate events.
                    _mapScriptProcess.Process.OnStateChange += OnStateChange;
                    ProcessManager.AttachProcess(_mapScriptProcess);

                    OnStateChange(_mapScriptProcess.Process, _mapScriptProcess.Process.State);   
                }
            }

            // Tell the scripts that we are now loading the new map (so they can show a loadings screen).
            _gameScriptProcess.Process.InvokeFunction("OnLoadingFinish", true, true);

            // Pump out a MapBegin event.
            EventManager.FireEvent(new Event("map_begin", this, null));
        }

		/// <summary>
		///		Called by the base engine class when its safe to begin deinitialization.
		/// </summary>
		protected override void Finish()
		{
            // Remember me!!! Remember me!!
            if (_rememberUser == true)
            {
                _engineConfigFile.SetSetting("account:username", _currentUsername);
                _engineConfigFile.SetSetting("account:password", _currentPassword);
            }
            else
            {
                _engineConfigFile.SetSetting("account:username", "");
                _engineConfigFile.SetSetting("account:password", "");
            }

            // Ignore everything else if we are not running a game.
            if (_gameName == "")
            {
                _explorerWindow.Dispose();
                _explorerWindow = null;
                GC.Collect();
                return;
            }

            // Close network.
            if (_serverWindow != null)
                NetworkManager.Finish();

			// Pump out a GameFinish event.
			EventManager.FireEvent(new Event("game_finish", this, null));

			// Allow the processes some time to be notified of closing events.
			EventManager.ProcessEvents();
			ProcessManager.RunProcesses(-1);

            // Close down the window.
            if (_window != null)
            {
                _window.Dispose();
                _window = null;
                GC.Collect();
            }

            // Close down the server window.
            if (_serverWindow != null)
            {
                _serverWindow.Dispose();
                _serverWindow = null;
                GC.Collect();
            }

            // Close the network connection.
            NetworkManager.Close();
        }

		/// <summary>
		///		Called by the base engine class to tell this class to load a map.
		/// </summary>
		public override void LoadMap(string file, string password)
		{
            // Ignore everything else if we are not running a game.
            if (_gameName == "")
                return;

            // Tell it to do it on the next frame, if we do it now we will get cyclic repeats from the scripts getting multiple
            // start / finish events.
            _mapLoadPending = true;
            _mapLoadFile = file;
            _mapLoadPassword = password;
		}

		/// <summary>
		///		Called when the game window or explorer window is closed.
		/// </summary>
		/// <param name="sender">Object that called this method.</param>
		/// <param name="eventArgs">Arguments describing why this method was called.</param>
		private void OnClosing(object sender, FormClosingEventArgs eventArgs)
		{
		    DebugLogger.WriteLog("Window closed.");
            _closingDown = true;
            eventArgs.Cancel = true;
        }

		/// <summary>
		///		Entry point of this application.
		/// </summary>
		/// <param name="args">Command lines passed to this application.</param>
		[STAThread]
		public static void Main(string[] args)
		{
			Fusion.GlobalInstance = new Fusion();
			Engine.GlobalInstance = (Engine)Fusion.GlobalInstance;
			Fusion.GlobalInstance.Run(args);
		}

		#endregion
	}

}