/*
 * File: Engine.cs
 *
 * Contains several classes used as a framework to build software off of.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Threading;
using BinaryPhoenix.Fusion.Engine;
using BinaryPhoenix.Fusion.Engine.Windows;
using BinaryPhoenix.Fusion.Engine.ScriptingFunctions;
using BinaryPhoenix.Fusion.Engine.Entitys;
using BinaryPhoenix.Fusion.Engine.ConsoleFunctions;
using BinaryPhoenix.Fusion.Graphics;
//using BinaryPhoenix.Fusion.Graphics.Direct3D9Driver;
//using BinaryPhoenix.Fusion.Graphics.PixelMapFactorys;
using BinaryPhoenix.Fusion.Audio;
//using BinaryPhoenix.Fusion.Audio.DirectSound9Driver;
//using BinaryPhoenix.Fusion.Audio.SampleFactorys;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Scripting;
using BinaryPhoenix.Fusion.Runtime.Scripting.ScriptingFunctions;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Runtime.StreamFactorys;
using BinaryPhoenix.Fusion.Runtime.Events;
using BinaryPhoenix.Fusion.Runtime.Resources;
using BinaryPhoenix.Fusion.Runtime.Processes;
using BinaryPhoenix.Fusion.Runtime.Languages;
using BinaryPhoenix.Fusion.Runtime.Collision;
using BinaryPhoenix.Fusion.Runtime.Console.ConsoleFunctions;
using BinaryPhoenix.Fusion.Input;
//using BinaryPhoenix.Fusion.Input.DirectInput9Driver;
using System.Diagnostics;
using System.Reflection;

namespace BinaryPhoenix.Fusion.Engine
{

	/// <summary>
	///		Contains general initialization and deinitialization code for the base of an game/editor,
	///		basically its a framework for any software built off the Fusion engine.
	/// </summary>
	public abstract class Engine
	{
		#region Members
		#region Variables

		private static Engine _globalInstance = null;

		protected string _configFilePath = "Fusion.xml";
		protected string _outputFilePath = "Fusion.log";

		protected string _gameName = "";
		protected string _relativeBasePath = "FusionApps", _basePath = "";
		protected string _relativePluginPath = "Plugins", _pluginPath = "";
        protected string _relativeDownloadPath = "Downloads", _downloadPath = "";
        protected string _relativeLogPath = "Logs", _logPath = "";
        protected string _relativeGlobalScriptLibraryPath = "Plugins\\Script Libraries", _globalScriptLibraryPath = "";
		protected string _gamePath = "";

		protected XmlConfigFile _gameConfigFile = null;
		protected XmlConfigFile _engineConfigFile = null;

		protected string _graphicsDriverID;
		protected string _audioDriverID;
		protected string _inputDriverID;

		protected float _engineVersion = 1.0f;

		protected bool _loadPlugins = true;
		protected bool _loadOnlyRequiredPlugins = false;

		protected bool _showInFullscreen = false;
		protected int _fpsLimit = 60;
		protected int[] _realResolution = new int[2] { 800, 600 };

		protected bool _showConsole = false;

	#if DEBUG || PROFILE
		protected bool _logDebugOutput = true;
	#else
			protected bool _logDebugOutput = false;
	#endif

		protected string[] _commandLineArguments;

		protected string _enginePath = "";
		protected string _configPath = "config";
		protected string _languagePath = "languages";
		protected string _savePath = "saves";
		protected string _buildPath = "builds";
		protected string _mediaPath = "media";
		protected string _scriptLibraryPath = "media\\scripts\\Libraries";

		protected string _tilesetPath = "media\\tilesets";
		protected string _fontPath = "media\\fonts";
        protected string _shaderPath = "media\\shaders";
		protected string _scriptPath = "media\\scripts";
		protected string _objectPath = "media\\objects";
		protected string _graphicPath = "media\\graphics";
		protected string _audioPath = "media\\audio";
		protected string _mapPath = "media\\maps";
        protected string _effectPath = "media\\effects";
        protected string _gamePluginPath = "media\\plugins";
        protected string _mapScriptPath = "media\\scripts\\maps";

		protected float _gameVersion = 1.0f;
		protected string _gameTitle = "";
		protected string _startScript = "";
		protected string _language = "English";

		protected bool _usePakFiles = false;
		protected string _pakFilePrefix = "pak#";
		protected int _maximumPakFileSize = 50;

		protected bool _compileScripts = true;
		protected bool _keepScriptSource = false;
		protected bool _treatWarningsAsErrors = false;
		protected bool _treatMessagesAsErrors = false;
		protected bool _compileInDebugMode = false;
		protected string _scriptDefines = "";

		protected int[] _resolution = new int[2] { 800, 600 };
		protected bool _keepAspectRatio = true;
		protected int _targetFps = 60;

        protected HighPreformanceTimer _fpsTimer = new HighPreformanceTimer();
        protected int _currentFPS = 0;
        protected int _fpsTicks = 0;

        protected bool _closingDown = false;

		protected ArrayList _requiredPlugins = new ArrayList();

		protected Map _map = new Map();

		protected HighPreformanceTimer _deltaTimer = new HighPreformanceTimer();
		protected float _deltaTimerCurrent = 0, _deltaTimerOld = 0, _deltaTime = 0;
        protected float _deltaScale = 1.0f;
        protected float _forcedDeltaTimeThisFrame = 0.0f;
        protected bool _trackDeltaTime = true;

        protected int _frameCount = 0;
        protected HighPreformanceTimer _frameTimer = new HighPreformanceTimer();
        protected float _frameDuration = 0;

        protected HighPreformanceTimer _frameStartTimer = new HighPreformanceTimer();
        protected int _frameStartTime = 0;

        protected float _eventProcessingDuration = 0, _processProcessingDuration = 0, _applicationProcessingDuration = 0, _collisionProcessingDuration = 0;
        protected float _renderingDuration = 0;

        protected float _timer1 = 0, _bounceTimer1;
        protected bool _bounceTimerDir = true;

        protected string _workDir = "";
        protected bool _standAlone = false;

        protected int _frameSkip = 0;
        protected int _frameSkipTracker = 0;

        protected bool _isServer = false;

        protected string _networkConfigFile = "network.xml";

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the currently running instance of the Editor class.
		/// </summary>
		public static Engine GlobalInstance
		{
			get { return _globalInstance; }
			set { _globalInstance = value; }
		}

        /// <summary>
        ///     If this returns true the game will not be updated this frame and time will be handed back to the OS.
        /// </summary>
        public virtual bool HandBackTimeToOS
        {
            get { return false; }
        }

        /// <summary>
        ///     Gets the duration in milliseconds that it took to process
        ///     game level events.
        /// </summary>
        public float EventProcessingDuration
        {
            get { return _eventProcessingDuration; }
        }

        /// <summary>
        ///     Gets if the aspect ratio should be kept in the graphics window.
        /// </summary>
        public bool KeepAspectRatio
        {
            get { return _keepAspectRatio; }
        }

        /// <summary>
        ///     Gets the duration in milliseconds that it took to process
        ///     game processs.
        /// </summary>
        public float ProcessProcessingDuration
        {
            get { return _processProcessingDuration; }
        }

        /// <summary>
        ///     Gets the duration in milliseconds that it took to process
        ///     application level events.
        /// </summary>
        public float ApplicationProcessingDuration
        {
            get { return _applicationProcessingDuration; }
        }

        /// <summary>
        ///     Gets the duration in milliseconds that it took to process
        ///     entity collisions.
        /// </summary>
        public float CollisionProcessingDuration
        {
            get { return _collisionProcessingDuration; }
        }

        /// <summary>
        ///     Gets the duration in milliseconds that it took to render the scene.
        /// </summary>
        public float RenderingDuration
        {
            get { return _renderingDuration; }
        }

        /// <summary>
        ///		Get or sets the the duration in milliseconds of the last frame.
        /// </summary>
        public float FrameDuration
        {
            get { return _frameDuration; }
        }

        /// <summary>
        ///		Get or sets the time at which this frame started.
        /// </summary>
        public int FrameStartTime
        {
            get { return _frameStartTime; }
        }

        /// <summary>
        ///		Get or sets if this engine should close down.
        /// </summary>
        public bool ClosingDown
        {
            get { return _closingDown; }
            set { _closingDown = value; }
        }

		/// <summary>
		///		Get or sets the current delta time.
		/// </summary>
		public float DeltaTime
		{
			get { return _deltaTime; }
			set { _deltaTime = value; }
		}

        /// <summary>
        ///		Get or sets the forced delta time for this frame.
        /// </summary>
        public float ForcedDeltaTimeThisFrame
        {
            get { return _forcedDeltaTimeThisFrame; }
            set { _forcedDeltaTimeThisFrame = value; }
        }

        /// <summary>
        ///		Get or sets the current delta scale.
        /// </summary>
        public float DeltaScale
        {
            get { return _deltaScale; }
            set { _deltaScale = value; }
        }

		/// <summary>
		///		Get or sets the fps limit.
		/// </summary>
		public int FPSLimit
		{
			get { return _fpsLimit; }
			set { _fpsLimit = value; }
		}

		/// <summary>
		///		Get or sets the fps target.
		/// </summary>
		public int FPSTarget
		{
			get { return _targetFps; }
			set { _targetFps = value; }
		}

        /// <summary>
        ///		Get or sets the current fps.
        /// </summary>
        public int CurrentFPS
        {
            get { return _currentFPS; }
            set { _currentFPS = value; }
        }

		/// <summary>
		///		Get or sets the path to the engine directory.
		/// </summary>
		public string EnginePath
		{
			get { return _enginePath; }
			set { _enginePath = value; }
		}

		/// <summary>
		///		Get or sets the path to the base game directory.
		/// </summary>
		public string BasePath
		{
			get { return _basePath; }
			set { _basePath = value; }
		}

        /// <summary>
        ///		Get or sets the path to the shader directory.
        /// </summary>
        public string ShaderPath
        {
            get { return _shaderPath; }
            set { _shaderPath = value; }
        }

		/// <summary>
		///		Get or sets the path to the plugin directory.
		/// </summary>
		public string PluginPath
		{
			get { return _pluginPath; }
			set { _pluginPath = value; }
		}

        /// <summary>
        ///		Get or sets the path to the games plugin directory.
        /// </summary>
        public string GamePluginPath
        {
            get { return _gamePluginPath; }
            set { _gamePluginPath = value; }
        }

        /// <summary>
        ///		Get or sets the path to the downloads directory.
        /// </summary>
        public string DownloadPath
        {
            get { return _downloadPath; }
            set { _downloadPath = value; }
        }

        /// <summary>
        ///		Get or sets the path to the logs directory.
        /// </summary>
        public string LogPath
        {
            get { return _logPath; }
            set { _logPath = value; }
        }

        /// <summary>
        ///		Get or sets the path to the global script library directory.
        /// </summary>
        public string GlobalScriptLibraryPath
        {
            get { return _globalScriptLibraryPath; }
            set { _globalScriptLibraryPath = value; }
        }

		/// <summary>
		///		Get or sets the path to the game directory.
		/// </summary>
		public string GamePath
		{
			get { return _gamePath; }
			set { _gamePath = value; }
		}

		/// <summary>
		///		Get or sets the path to the configuration directory.
		/// </summary>
		public string ConfigPath
		{
			get { return _configPath; }
			set { _configPath = value; }
		}

		/// <summary>
		///		Get or sets the path to the language directory.
		/// </summary>
		public string LanguagePath
		{
			get { return _languagePath; }
			set { _languagePath = value; }
		}

		/// <summary>
		///		Get or sets the path to the map directory.
		/// </summary>
		public string MapPath
		{
			get { return _mapPath; }
			set { _mapPath = value; }
		}

        /// <summary>
        ///		Get or sets the path to the map script directory.
        /// </summary>
        public string MapScriptPath
        {
            get { return _mapScriptPath; }
            set { _mapScriptPath = value; }
        }

        /// <summary>
        ///		Get or sets the path to the effect directory.
        /// </summary>
        public string EffectPath
        {
            get { return _effectPath; }
            set { _effectPath = value; }
        }

		/// <summary>
		///		Get or sets the path to the saves directory.
		/// </summary>
		public string SavePath
		{
			get { return _savePath; }
			set { _savePath = value; }
		}

		/// <summary>
		///		Get or sets the path to the build directory.
		/// </summary>
		public string BuildPath
		{
			get { return _buildPath; }
			set { _buildPath = value; }
		}

		/// <summary>
		///		Get or sets the path to the media directory.
		/// </summary>
		public string MediaPath
		{
			get { return _mediaPath; }
			set { _mediaPath = value; }
		}

		/// <summary>
		///		Gets or sets the path to the directory containing all tileset data used by the current game.
		/// </summary>
		public string TilesetPath
		{
			get { return _tilesetPath; }
			set { _tilesetPath = value; }
		}

		/// <summary>
		///		Gets or sets the path to the directory containing all font data used by the current game.
		/// </summary>
		public string FontPath
		{
			get { return _fontPath; }
			set { _fontPath = value; }
		}

		/// <summary>
		///		Gets or sets the path to the directory containing all script data used by the current game.
		/// </summary>
		public string ScriptPath
		{
			get { return _scriptPath; }
			set { _scriptPath = value; }
		}

		/// <summary>
		///		Gets or sets the path to the directory containing all object data used by the current game.
		/// </summary>
		public string ObjectPath
		{
			get { return _objectPath; }
			set { _objectPath = value; }
		}

		/// <summary>
		///		Gets or sets the path to the directory containing all graphical data used by the current game.
		/// </summary>
		public string GraphicPath
		{
			get { return _graphicPath; }
			set { _graphicPath = value; }
		}

		/// <summary>
		///		Gets or sets the path to the directory containing all audio data used by the current game.
		/// </summary>
		public string AudioPath
		{
			get { return _audioPath; }
			set { _audioPath = value; }
		}

		/// <summary>
		///		Get or sets the path to the scripts library directory.
		/// </summary>
		public string ScriptLibraryPath
		{
			get { return _scriptLibraryPath; }
			set { _scriptLibraryPath = value; }
		}

		/// <summary>
		///		Gets or sets the name of the game currently being accessed.
		/// </summary>
		public string GameName
		{
			get { return _gameName; }
			set { _gameName = value; }
		}

		/// <summary>
		///		Gets or sets the xml configuration file that contains the engine settings.
		/// </summary>
		public XmlConfigFile EngineConfigFile
		{
			get { return _engineConfigFile; }
			set { _engineConfigFile = value; }
		}

		/// <summary>
		///		Get or sets the xml configuration file that contains the game settings.
		/// </summary>
		public XmlConfigFile GameConfigFile
		{
			get { return _gameConfigFile; }
			set { _gameConfigFile = value; }
		}

		/// <summary>
		///		Gets or sets the command line arguments recieved by this application.
		/// </summary>
		public string[] CommandLineArguments
		{
			get { return _commandLineArguments; }
			set { _commandLineArguments = value; }
		}

		/// <summary>
		///		Gets or sets the map currently being edited.
		/// </summary>
		public Map Map
		{
			get { return _map; }
			set { _map = value; }
		}

        /// <summary>
        ///     Gets or sets if the delta time should be tracked.
        /// </summary>
        public bool TrackDeltaTime
        {
            get { return _trackDeltaTime; }
            set { _trackDeltaTime = value; }
        }

        /// <summary>
        ///     Gets or sets if this engine is working as a server.
        /// </summary>
        public bool IsServer
        {
            get { return _isServer; }
            set { _isServer = value; }
        }

		#endregion
		#endregion
		#region Methods

        /// <summary>
        ///     Called when a new instance of this class is created.
        /// </summary>
        public Engine()
        {
            // Creates an event handler that will be executed if an error occurs.
            Application.ThreadException += new ThreadExceptionEventHandler(ThreadException);
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);
        }

		/// <summary>
		///		When called the game engine will start running.
		/// </summary>
		public void Run(string[] cmdArgs)
		{
			bool restartEngine = false; // This is used to flag if the engine should
			// restart when the game finishs, this is 
			// mainly used to handle errors gracefully.

            _closingDown = false;
			_commandLineArguments = cmdArgs;
#if RELEASE
            try
            {
#endif
                // Initialize the game engine.
                Application.EnableVisualStyles();
                if (Initialize() == false)
                {
                    DeInitialize();
                    return;
                }

                // Start the application message loop.
                DebugLogger.WriteLog("Running application...");
                MainLoop();
#if RELEASE
            }
            catch (Exception e)
            {
                ErrorWindow window = new ErrorWindow();
                window.ErrorMessage = e.ToString();
                if (window.ShowDialog() == DialogResult.Abort)
                    return;
            }
#endif

            // DeInitialize the game engine.
            DeInitialize();

			if (restartEngine == true) Run(cmdArgs);
		}
        /*
        /// <summary>
        ///     Called when an exception occurs in the application.
        /// </summary>
        /// <param name="sender">Object that invoked this function.</param>
        /// <param name="e">Paramaters describing why this function was called.</param>
        void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            DebugLogger.WriteLog("Exception occured: " + e.ExceptionObject.ToString(), LogAlertLevel.Error);

            ErrorWindow window = new ErrorWindow();
            window.ErrorMessage = e.ToString();
            if (window.ShowDialog() == DialogResult.Abort)
                return;
        }
        */
        /// <summary>
        ///     Called when an exception occurs in the application.
        /// </summary>
        /// <param name="sender">Object that invoked this function.</param>
        /// <param name="e">Paramaters describing why this function was called.</param>
        void ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            DebugLogger.WriteLog("Exception occured: "+e.Exception.ToString(), LogAlertLevel.Error);
        }

		/// <summary>
		///		Initializes the game engine and sets up all the sub systems.
		/// </summary>
		/// <returns>True if initialization was sucessfull.</returns>
		private bool Initialize()
		{
            Environment.CurrentDirectory = Path.GetDirectoryName(Application.ExecutablePath);

			#region Factory Installation

			// Install stream factorys.
			new PakStreamFactory();
			new MemoryStreamFactory();
			new DefaultStreamFactory();

			// Install standard scripting language librarys.
			new MathmaticsFunctionSet();
			new IOFunctionSet();
			new StringFunctionSet();
			new RuntimeFunctionSet();
			new DebugFunctionSet();
			new ThreadFunctionSet();
            
			new EngineFunctionSet();
			new EntityFunctionSet();
			new InputFunctionSet();
			new ResourceFunctionSet();
			new LanguageFunctionSet();
			new AudioFunctionSet();
			new EventFunctionSet();
			new GraphicsFunctionSet();
			new ProcessFunctionSet();
			new AIFunctionSet();
            new MapFunctionSet();

            // Console commands
            new EntityConsoleCommands();
            new EngineConsoleCommands();
            new SystemConsoleCommands();

			#endregion

            // Trim working set to stop this app hogging memory.
            NativeMethods.SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);

			// Initialize debug log first of all so we can start logging messages.
			DebugLogger.Initialize(_outputFilePath);

			// Parse the command lines for configuration options.
			ParseCommandLines(_commandLineArguments, true);

			// Load the engines configuration.
			LoadEngineConfiguration();

			// Parse the command lines for anything besides configuration options.
			ParseCommandLines(_commandLineArguments, false);

            if (_gameName != "")
            {
                // Load the games configuration.
                LoadGameConfiguration();

                // Set the debug loggers output file.
                DebugLogger.OutputFile = _logPath + "\\" + _gameTitle + " " + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + ".log";
            }
            else
                DebugLogger.OutputFile = _outputFilePath;

			// Bind all available plugins.
			CreatePlugins();

			// Install the drivers.
			InstallDrivers();

			// Initialize the script compiler.
			InitializeScriptCompiler();

            // Set the wroking directory to the new one.
            if (_workDir != "")
            {
                Environment.CurrentDirectory = _workDir;
                DebugLogger.WriteLog("Setting working directory to \""+_workDir+"\".");
            }

			// Tell the overidding class that they can begin setting everything up.
			if (Begin() == true)
                return false;

            // Tell Plugins that we have begun.
            Plugin.CallPluginMethod("Begin");

			return true;
		}

		/// <summary>
		///		Parses the command lines passed to this application
		///		and runs the appropriate operations.
		/// </summary>
		/// <param name="cmdArgs">Array of command line arguments to parse.</param>
		/// <param name="onlyConfig">If true this method will only parse the -config command line.</param>
		private void ParseCommandLines(string[] cmdArgs, bool onlyConfig)
		{
			foreach (string arg in cmdArgs)
			{
				string[] value = new string[0];
				string command = arg;
				int colonIndex = arg.IndexOf(':');

				// Seperate values and command if a colon exists.
				if (colonIndex >= 0)
				{
					value = new string[1];
					value[0] = arg.Substring(colonIndex + 1, arg.Length - colonIndex - 1);
					if (value[0].IndexOf(",") >= 0) value = value[0].Split(new char[1] { ',' });
					command = arg.Substring(0, colonIndex);
				}

				// Check the command and preform its action.
				switch (command.ToLower())
				{
                    // Its a server! Awesomeness!
                    case "-server":
                        if (onlyConfig == true) continue;
                        if (value.Length == 0)
                            _isServer = true;
                        else
                            throw new Exception("Invalid argument length. Usage: -server");
                        break;

                    // Its a port number, er... cool?
                    case "-port":
                        if (onlyConfig == true) continue;
                        if (value.Length == 1)
                           Networking.NetworkManager.Port = Int32.Parse(value[0].ToLower().Trim());
                        else
                            throw new Exception("Invalid argument length. Usage: -port:<port_number>");
                        break;
                    
                    // Overrides the default game name.
					case "-game":
						if (onlyConfig == true) continue;
						if (value.Length == 1)
							_gameName = value[0].ToLower().Trim();
						else
							throw new Exception("Invalid argument length. Usage: -game:<game_name>");
						break;

                    // Overrides the default workign dir.
                    case "-workdir":
                        if (onlyConfig == true) continue;
                        if (value.Length == 1)
                            _workDir = value[0].ToLower().Trim();
                        else
                            throw new Exception("Invalid argument length. Usage: -workdir:<dir>");
                        break;

					// Overrides the default game base path.
					case "-basepath":
						if (onlyConfig == true) continue;
						if (value.Length == 1)
							_basePath = value[0].ToLower().Trim();
						else
							throw new Exception("Invalid argument length. Usage: -basepath:<base_path>");
						break;

					// Overrides the default plugin base path.
					case "-pluginpath":
						if (onlyConfig == true) continue;
						if (value.Length == 1)
							_pluginPath = value[0].ToLower().Trim();
						else
							throw new Exception("Invalid argument length. Usage: -pluginpath:<plugin_path>");
						break;

					// Overrides the default "Crystalline River.cfg" file used
					// to load configuration options. This is usfull when working
					// with multiple developers all wanting there own config files.
					case "-config":
						if (onlyConfig == false) continue;
						if (value.Length == 1)
						{
							if (File.Exists(value[0]) == true)
								_configFilePath = value[0];
							else
								throw new Exception("Invalid configuration file, file does not exist.");
						}
						else
							throw new Exception("Invalid argument length. Usage: -config:<file>");
						break;

					// Selects which graphics driver to use.
					// Syntax: -gdriver:driver_mnemonic
					case "-gdriver":
						if (onlyConfig == true) continue;
						if (value.Length == 1)
							_graphicsDriverID = value[0].ToLower().Trim();
						else
							throw new Exception("Invalid argument length. Usage: -gdriver:<driver_mnemonic>");
						break;

					// Selects which audio driver to use.
					// Syntax: -adriver:driver_mnemonic
					case "-adriver":
						if (onlyConfig == true) continue;
						if (value.Length == 1)
							_audioDriverID = value[0].ToLower().Trim();
						else
							throw new Exception("Invalid argument length. Usage: -adriver:<driver_mnemonic>");
						break;

					// Selects which input driver to use.
					// Syntax: -idriver:driver_mnemonic
					case "-idriver":
						if (onlyConfig == true) continue;
						if (value.Length == 1)
							_inputDriverID = value[0].ToLower().Trim();
						else
							throw new Exception("Invalid argument length. Usage: -idriver:<driver_mnemonic>");
						break;

					// If set all engine output will be written to a log file, this is pointless
					// in debug and profile builds as they automatically do it.
					// Syntax: -log
					// Syntax: -log:Path
					case "-log":
						if (onlyConfig == true) continue;
						if (value.Length == 0)
							_logDebugOutput = true;
						else if (value.Length == 1)
						{
							if (File.Exists(value[0]) == true)
								_outputFilePath = value[0];
							else
								throw new Exception("Invalid output file, file does not exist.");
						}
						else
							throw new Exception("Invalid argument length. Usage: -log:<log_file>");
						break;
				}
			}
		}

		/// <summary> 
		///		Loads this engines configuration from the engine configuration file.
		/// </summary>
		private void LoadEngineConfiguration()
		{
			// First things first lets read in the engine configuration file.
			_engineConfigFile = new XmlConfigFile(_configFilePath);

			// Get any configuration values relative to initializing the engine.
            _standAlone = _engineConfigFile["standalone", "0"] == "0" ? false : true;
            if (_standAlone == true) _gameName = "standalone";
			DebugLogger.OutputFile = _engineConfigFile["debug:log:logfile", _outputFilePath];
			DebugLogger.LogFatalErrors = _engineConfigFile["debug:log:logfatalerrors", "1"] == "0" ? false : true;
			DebugLogger.LogErrors = _engineConfigFile["debug:log:logerrors", "1"] == "0" ? false : true;
			DebugLogger.LogWarnings = _engineConfigFile["debug:log:logwarnings", "1"] == "0" ? false : true;
			DebugLogger.LogMessages = _engineConfigFile["debug:log:logmessages", "1"] == "0" ? false : true;
			DebugLogger.EmitLogFile = _engineConfigFile["debug:log:emitlog", _logDebugOutput.ToString().ToLower() == "true" ? "1" : "0"] == "0" ? false : true;

            // Read in driver names.
			_graphicsDriverID = _engineConfigFile["graphics:driver", _graphicsDriverID];
			_audioDriverID = _engineConfigFile["audio:driver", _audioDriverID];
			_inputDriverID = _engineConfigFile["input:driver", _inputDriverID];

            // Work out media paths.
			_relativeBasePath = _engineConfigFile["paths:basepath", _basePath];
			_relativePluginPath = _engineConfigFile["paths:pluginpath", _pluginPath];
            _relativeDownloadPath = _engineConfigFile["paths:downloadpath", _downloadPath];
            _relativeLogPath = _engineConfigFile["paths:logpath", _logPath];
            _relativeGlobalScriptLibraryPath = _engineConfigFile["paths:scriptlibrarypath", _globalScriptLibraryPath];
            _basePath = Environment.CurrentDirectory + "\\" + _relativeBasePath;
			_pluginPath = Environment.CurrentDirectory + "\\" + _relativePluginPath;
            _downloadPath = Environment.CurrentDirectory + "\\" + _relativeDownloadPath;
            _logPath = Environment.CurrentDirectory + "\\" + _relativeLogPath;
            _outputFilePath = _logPath + "\\Fusion "+DateTime.Now.Day+"-"+DateTime.Now.Month+"-"+DateTime.Now.Year+" "+DateTime.Now.Hour+"-"+DateTime.Now.Minute+"-"+DateTime.Now.Second+".log";
            _globalScriptLibraryPath = Environment.CurrentDirectory + "\\" + _relativeGlobalScriptLibraryPath;

            // Work out plugin details.
			_loadPlugins = _engineConfigFile["plugins:loadplugins", _loadPlugins == true ? "1" : "0"] == "1" ? true : false;
			_loadOnlyRequiredPlugins = _engineConfigFile["plugins:loadonlyrequiredplugins", _loadOnlyRequiredPlugins == true ? "1" : "0"] == "1" ? true : false;

            // Work out resolution details.
			_showInFullscreen = _engineConfigFile["graphics:fullscreen", _showInFullscreen == true ? "1" : "0"] == "1" ? true : false;
            int.TryParse(_engineConfigFile["graphics:fpslimit", _fpsLimit.ToString()], out _fpsLimit);
            int.TryParse(_engineConfigFile["graphics:frameskip", _frameSkip.ToString()], out _frameSkip);

			string[] realResolutionSplit =_engineConfigFile["graphics:resolution:dimensions", _realResolution[0]+","+_realResolution[1]].Split(new char[] {','});
            int.TryParse(realResolutionSplit[0], out _realResolution[0]);
            int.TryParse(realResolutionSplit[1], out _realResolution[1]);
            
            // Misc stuff
			_showConsole = _engineConfigFile["debug:showconsole", _showConsole == true ? "1" : "0"] == "1" ? true : false;
            float.TryParse(_engineConfigFile["version", _engineVersion.ToString()], out _engineVersion);
        }

		/// <summary>
		///		Installs the graphics, audio and input drivers.
		/// </summary>
		private void InstallDrivers()
		{
			GraphicsManager.Driver = null;
			AudioManager.Driver = null;
			InputManager.Driver = null;

			// Create a new graphics driver instance based on the graphicsDriverID variable
			// which can be changed by the configuration file or command line.
			DebugLogger.WriteLog("Installing '" + _graphicsDriverID + "' graphics driver...");
            try
            {
                GraphicsManager.Driver = ReflectionMethods.CreateObject(_graphicsDriverID) as IGraphicsDriver;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to install graphics driver, please make sure you have the latest version of the API you are using or change the API to one your computer supports.\n\n" + e.ToString());
            }

			// Create a new audio driver instance based on the audioDriverID variable
			// which can be changed by the configuration file or command line.
			DebugLogger.WriteLog("Installing '" + _audioDriverID + "' audio driver...");
            try
            {
			    AudioManager.Driver = ReflectionMethods.CreateObject(_audioDriverID) as IAudioDriver;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to install audio driver, please make sure you have the latest version of the API you are using or change the API to one your computer supports.\n\n" + e.ToString());
            }

			// Create a new input driver instance based on the inputDriverID variable
			// which can be changed by the configuration file or command line.
			DebugLogger.WriteLog("Installing '" + _inputDriverID + "' input driver...");
            try
            {
			    InputManager.Driver = ReflectionMethods.CreateObject(_inputDriverID) as IInputDriver;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to install input driver, please make sure you have the latest version of the API you are using or change the API to one your computer supports.\n\n" + e.ToString());
            }
		}

		/// <summary>
		///		Loads this games configuration from the games configuration file.
		/// </summary>
		private void LoadGameConfiguration()
		{
            if (_standAlone == false)
            {
                // Reset the working folder in case we are already in a game directory.
                Environment.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

                // Work out the path to the game's directory and check it exists.
                _gamePath = _basePath + "\\" + _gameName;
                if (Directory.Exists(_gamePath) == false) DebugLogger.WriteLog("Game directory '" + _gamePath + "' could not be found.", LogAlertLevel.FatalError);
                DebugLogger.WriteLog("Loading game from " + _gamePath + "...");

                // Change the working directory to the games directory.
                _enginePath = Environment.CurrentDirectory;
                Environment.CurrentDirectory = _gamePath;
            }

            // Read in the Xml configuration file for it.
            DebugLogger.WriteLog("Loading game configuration file...");
            _gameConfigFile = new XmlConfigFile(_gameName + ".xml");

			// Load the game's configuration details.
			_configPath = _gameConfigFile["paths:configpath", _configPath];
			_languagePath = _gameConfigFile["paths:languagepath", _languagePath];
			_savePath = _gameConfigFile["paths:savepath", _savePath];
            if (_savePath != "" && Directory.Exists(_savePath) == false) Directory.CreateDirectory(_savePath);

			_buildPath = _gameConfigFile["paths:buildpath", _buildPath];
			_mediaPath = _gameConfigFile["paths:mediapath", _mediaPath];
			_scriptLibraryPath = _gameConfigFile["paths:scriptlibrarypath", _scriptLibraryPath];
			_tilesetPath = _gameConfigFile["paths:tilesetpath", _tilesetPath];
			_fontPath = _gameConfigFile["paths:fontpath", _fontPath];
			_scriptPath = _gameConfigFile["paths:scriptpath", _scriptPath];
			_objectPath = _gameConfigFile["paths:objectpath", _objectPath];
			_graphicPath = _gameConfigFile["paths:graphicpath", _graphicPath];
			_audioPath = _gameConfigFile["paths:audiopath", _audioPath];
			_mapPath = _gameConfigFile["paths:mappath", _mapPath];
            _mapScriptPath = _gameConfigFile["paths:mapscriptpath", _mapScriptPath]; 
            _effectPath = _gameConfigFile["paths:effectpath", _effectPath];
            _shaderPath = _gameConfigFile["paths:shaderpath", _shaderPath];
            _gamePluginPath = _gameConfigFile["paths:pluginpath", _shaderPath];

			_gameVersion = Convert.ToSingle(_gameConfigFile["version", _gameVersion.ToString()]);
			_gameTitle = _gameConfigFile["title", _gameTitle];
			_startScript = _gameConfigFile["startscript", _startScript];
			_language = _gameConfigFile["language", _language];

			_usePakFiles = _gameConfigFile["resources:usepakfiles", _usePakFiles == true ? "1" : "0"] == "1" ? true : false;
			_pakFilePrefix = _gameConfigFile["resources:pakfileprefix", _pakFilePrefix];
			_maximumPakFileSize = Convert.ToInt32(_gameConfigFile["resources:maximumpakfilesize", _maximumPakFileSize.ToString()]);

			_compileScripts = _gameConfigFile["resources:scripts:compilescripts", _compileScripts == true ? "1" : "0"] == "1" ? true : false;
			_keepScriptSource = _gameConfigFile["resources:scripts:keepscriptsource", _keepScriptSource == true ? "1" : "0"] == "1" ? true : false;
			_treatWarningsAsErrors = _gameConfigFile["resources:scripts:treatwarningsaserrors", _treatWarningsAsErrors == true ? "1" : "0"] == "1" ? true : false;
			_treatMessagesAsErrors = _gameConfigFile["resources:scripts:treatmessagesaserrors", _treatMessagesAsErrors == true ? "1" : "0"] == "1" ? true : false;
			_compileInDebugMode = _gameConfigFile["resources:scripts:compileindebugmode", _compileInDebugMode == true ? "1" : "0"] == "1" ? true : false;
			_scriptDefines = _gameConfigFile["resources:scripts:scriptdefines", _scriptDefines];

			string[] resolutionSplit = _gameConfigFile["graphics:resolution:dimensions", _resolution[0] + "," + _resolution[1]].Split(new char[] { ',' });
			_resolution[0] = Convert.ToInt32(resolutionSplit[0]);
			_resolution[1] = Convert.ToInt32(resolutionSplit[1]);
			_keepAspectRatio = _gameConfigFile["graphics:resolution:keepaspectratio", _keepAspectRatio == true ? "1" : "0"] == "1" ? true : false;
			_targetFps = Convert.ToInt32(_gameConfigFile["graphics:targetfps", _targetFps.ToString()]);

            Networking.NetworkManager.NetworkConfigFile = _gameConfigFile["network:configfile", Networking.NetworkManager.NetworkConfigFile];

			// Go through requirements and filter them.
			string[] requiredPlugins = _gameConfigFile.GetSettings("requirements");
			if (requiredPlugins != null)
			{
				foreach (string requirement in requiredPlugins)
				{
					string[] pathSplit = requirement.Split(new char[] { ':' });
					string name = pathSplit[pathSplit.Length - 1];
					string value = _gameConfigFile[requirement, ""];
					switch (name.ToLower())
					{
						case "plugin":
							_requiredPlugins.Add(value);
							break;
					}
				}
			}

			// Load key bindings.
			string[] keyBindings = _gameConfigFile.GetSettings("input:bindings");
			if (keyBindings != null)
			{
				foreach (string binding in keyBindings)
				{
					string[] pathSplit = binding.Split(new char[] { ':' });
					string name = pathSplit[pathSplit.Length - 1];
					string value = _gameConfigFile[binding, ""];
					try
					{
						InputManager.BindKey(name, (KeyCodes)Enum.Parse(typeof(KeyCodes), value, true));
						DebugLogger.WriteLog("Bound key " + value + " to action " + name + ".");
					}
					catch (Exception)
					{
						DebugLogger.WriteLog("Unable to bind key " + value + " to action " + name + ".", LogAlertLevel.Error);
					}
				}
			}

            // Load entity settings
            string defaultDepthMode = _gameConfigFile["entities:defaultdepthmode", EntityDepthMode.SubtractCollisionBoxBottom.ToString()];
            EntityNode.DefaultDepthMode = (EntityDepthMode)Enum.Parse(typeof(EntityDepthMode), defaultDepthMode, true);
		}

		/// <summary>
		///		Initializes all the default settings for the script compiler.
		/// </summary>
		private void InitializeScriptCompiler()
		{
			// Set up the script language compiler.
			string[] defines = _scriptDefines.Split(new char[1] { ',' });
            ScriptCompiler.DefaultIncludes = new object[] { _scriptLibraryPath, _globalScriptLibraryPath };
			ScriptCompiler.DefaultDefines = new Define[defines.Length + 1]; 
			ScriptCompiler.DefaultFlags = _compileInDebugMode ? CompileFlags.Debug : 0;

			// Split up the script defines contained in the game configuration files.
			for (int i = 0; i < defines.Length; i++)
				if (defines[i].IndexOf('=') >= 0)
				{
					string[] splitList = defines[i].Split(new char[1] { '=' });
					ScriptCompiler.DefaultDefines[i] = new Define(splitList[0], splitList[1], TokenID.TypeIdentifier);
				}
				else
					ScriptCompiler.DefaultDefines[i] = new Define(defines[i], "", TokenID.TypeIdentifier);

			ScriptCompiler.DefaultDefines[ScriptCompiler.DefaultDefines.Length - 1] = new Define(_compileInDebugMode ? "__DEBUG__" : "__RELEASE__", "", TokenID.TypeBoolean);
		}

		/// <summary>
		///		Decides which plugins need to be bound to this application.
		/// </summary>
		private void CreatePlugins()
		{
			if (_loadPlugins == false) return;
			if (_loadOnlyRequiredPlugins == true)
			{
				foreach (string plugin in _requiredPlugins)
				{
                    if (File.Exists(_pluginPath + "\\" + plugin) == false)
                    {
                        if (File.Exists(_gamePluginPath + "\\" + plugin) == false)
                            DebugLogger.WriteLog("Unable to bind required plugin '" + plugin + "', could not find plugin.", LogAlertLevel.FatalError);
                        else
                        {
                            if (CreateAndBindPlugin(_gamePluginPath + "\\" + plugin) == false)
                                DebugLogger.WriteLog("Unable to bind required plugin '" + plugin + "', error occured during binding process.", LogAlertLevel.FatalError);
                        }
                    }
                    else
                    {
                        if (CreateAndBindPlugin(_pluginPath + "\\" + plugin) == false)
                            DebugLogger.WriteLog("Unable to bind required plugin '" + plugin + "', error occured during binding process.", LogAlertLevel.FatalError);
                    }
                }
			}
			else
			{
                if (Directory.Exists(_pluginPath))
                {
                    foreach (string file in Directory.GetFiles(_pluginPath))
                    {
                        if (Path.GetExtension(file).ToLower() != ".dll") continue;
                        if (CreateAndBindPlugin(file) == false)
                        {
                            foreach (string plugin in _requiredPlugins)
                                if (plugin.ToLower() == Path.GetFileName(file).ToLower())
                                    DebugLogger.WriteLog("Unable to bind required plugin '" + plugin + "', error occured during binding process.", LogAlertLevel.FatalError);
                        }
                    }
                }
                if (Directory.Exists(_gamePluginPath))
                {
                    foreach (string file in Directory.GetFiles(_gamePluginPath))
                    {
                        if (Path.GetExtension(file).ToLower() != ".dll") continue;
                        if (CreateAndBindPlugin(file) == false)
                        {
                            foreach (string plugin in _requiredPlugins)
                                if (plugin.ToLower() == Path.GetFileName(file).ToLower())
                                    DebugLogger.WriteLog("Unable to bind required plugin '" + plugin + "', error occured during binding process.", LogAlertLevel.FatalError);
                        }
                    }
                }
			}
		}

		/// <summary>
		///		Binds a plugin .dll file to this application.
		/// </summary>
		/// <param name="file">Plugin to bind to this application.</param>
		/// <returns>True on success, else false.</returns>
		private bool CreateAndBindPlugin(string file)
		{
			// Notify the user that we are loading this plugin.
			DebugLogger.WriteLog("Creating plugin '" + file + "'.");

            // Create a plugin class.
            Plugin plugin = new Plugin(file);
            
            // Bind the plugin.
            return plugin.Bind();
		}

		/// <summary>
		///		Updates and renders the game until the application becomes active 
		///		(when it gets sent a message).
		/// </summary>
		/// <param name="sender">Object that called this method.</param>
		/// <param name="eventArgs">Arguments describing why this method was called.</param>
		private void MainLoop()
		{
            while (_closingDown == false)
            {
                // Reset statistics.
               // Statistics.StoreInt("Nodes Rendered", 0);

                // Hand back time to OS as we don't need it when the window is 
                // minimized or not it focus.
                if (HandBackTimeToOS == true)
                {
                    // Update essential stuff.
                    Application.DoEvents();
                    Networking.NetworkManager.Poll();

                    // Sleep.
                    Thread.Sleep(50);
                    _deltaTimerOld = (float)_deltaTimer.DurationMillisecond;
                    continue;
                }

                // Track frame stats.
                TrackFrameStatsBegin();

                // Poll for new input.
                InputManager.Poll();

                // Process new events.
                HighPreformanceTimer timer = new HighPreformanceTimer();
                EventManager.ProcessEvents();
                _eventProcessingDuration = (float)timer.DurationMillisecond;

                // Process processes.
                timer.Restart();
                ProcessManager.RunProcesses(_deltaTime);
                _processProcessingDuration = (float)timer.DurationMillisecond;

                // Update the collision manager.
                timer.Restart();
                CollisionManager.ProcessCollisions();
                _collisionProcessingDuration = (float)timer.DurationMillisecond;

                // Update plugins.
                Plugin.CallPluginMethod("Update");

                // Tell the abstraction layer that it can update.
                Update();

                // Update the shader variables
                _bounceTimer1 += _bounceTimerDir == true ? (1.0f * _deltaTime) : -(1.0f * _deltaTime);
                if (_bounceTimer1 >= 100)
                    _bounceTimerDir = false;
                else if (_bounceTimer1 <= 0)
                    _bounceTimerDir = true;

                GraphicsManager.SetShaderVariable("deltaTime", _deltaTime);
                GraphicsManager.SetShaderVariable("frameCount", _frameCount);
                GraphicsManager.SetShaderVariable("timer1", _timer1 += (0.001f * _deltaTime));
                GraphicsManager.SetShaderVariable("random1", MathMethods.Random(0.0f, 1.0f));
                GraphicsManager.SetShaderVariable("random2", MathMethods.Random(0.0f, 1.0f));
                GraphicsManager.SetShaderVariable("bounceTimer1", _bounceTimer1);

                // Render all canvases.
                timer.Restart();
                if (_isServer == false)
                {
                    if (_frameSkip == 0 || (++_frameSkipTracker % _frameSkip == 0))
                        GraphicsCanvas.RenderAll();
                }
                _renderingDuration = (float)timer.DurationMillisecond;
                
                // Process application level events.
                timer.Restart();
                Application.DoEvents();
                _applicationProcessingDuration = (float)timer.DurationMillisecond;

                // Update the network.
                Networking.NetworkManager.Poll();

                // Track frame stats.
                TrackFrameStatsFinish();
            }
        }

        /// <summary>
        ///     Tracks statistics about this frame. (Called at the start of the frame)
        /// </summary>
        protected void TrackFrameStatsBegin()
        {
            // Time the frame.
            _frameTimer.Restart();
            _frameStartTime = (int)_frameStartTimer.DurationMillisecond;

            // Work out the current delta time.
            _deltaTimerCurrent = (float)_deltaTimer.DurationMillisecond;
            _deltaTime = ((_deltaTimerCurrent - _deltaTimerOld) / (1000.0f / _targetFps)) * _deltaScale;
            if (_deltaTime < 0.0f || _deltaTime > 10.0f) _deltaTime = 1.0f; // Oh shit, something wierd has happened. Lets try and fix it.

            // If we're not using the delta time - just set it to 1.
            if (_trackDeltaTime == false)
                _deltaTime = 1.0f;

            // Do we need to force the delta time to be something this frame?
            if (_forcedDeltaTimeThisFrame != 0.0f)
            {
                _deltaTime = _forcedDeltaTimeThisFrame;
                _forcedDeltaTimeThisFrame = 0.0f;
            }

            //_deltaTimer.Restart();
            _deltaTimerOld = _deltaTimerCurrent;
        }

        /// <summary>
        ///     Tracks statistics about this frame. (Called at the end of the frame)
        /// </summary>
        protected void TrackFrameStatsFinish()
        {
            // If the update was less than than the frame limit threshold pass some 
            // cpu time back to the OS.
            _frameDuration = (float)_frameTimer.DurationMillisecond;
            if (_fpsLimit != 0 && _frameDuration < (1000.0f / _fpsLimit))
            {
                float freeTime = (freeTime = Math.Max(((int)(1000.0f / _fpsLimit) - (int)_frameDuration), 0));
               // while (freeTime > 0 && freeTime <= (1000.0f / _fpsLimit))
                //{
                ///    Thread.Sleep(1);
                //    Application.DoEvents();
                //    freeTime = (freeTime = Math.Max(((int)(1000.0f / _fpsLimit) - (int)_frameTimer.DurationMillisecond), 0));
                //}


                // Make sure its not going over the max amount of allowed for each frame, 
                // if it is an error has probably occured - so for god sake, don't call sleep.
                if (freeTime > 0 && freeTime <= (1000.0f / _fpsLimit))
                {
                    Thread.Sleep((int)freeTime);
                }
            }

            // Count FPS.
            if (_fpsTimer.DurationMillisecond > 1000)
            {
                _fpsTimer.Restart();
                _currentFPS = _fpsTicks;
                _fpsTicks = 0;
            }
            else
                _fpsTicks++;

            // Increase frame count.
            _frameCount++;
        }

		/// <summary>
		///		DeInitializes the game engine and closes down all sub systems.
		/// </summary>
		private void DeInitialize()
		{
			// Tell the overriding class that they can close down.
			Finish();

            // Tell Plugins that we have finished.
            Plugin.CallPluginMethod("Finish");

            // Unbind plugins.
            Plugin.UnBindPlugins();

			// Destroy the sub-system drivers.
			DebugLogger.WriteLog("Destroying drivers...");
			AudioManager.Driver = null;
			InputManager.Driver = null;
			GraphicsManager.Driver = null;
			GC.Collect();

			// Switch the current directory back to the exe's director.
			Environment.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

			// Save any changes made to the engine xml configuration file.
			_engineConfigFile.Save(_configFilePath);

			// Close debug log.
			DebugLogger.WriteLog("Closing log...\r\n");
			DebugLogger.DeInitialize();

            // Ech, horrible way to terminate the process.
            System.Diagnostics.Process.GetCurrentProcess().Kill();
		}

		/// <summary>
		///		Used to notify the overridding class that it can begin its initialization.
		/// </summary>
        protected virtual bool Begin() { return false; }

		/// <summary>
		///		Used to notify the overridding class that it can begin its deinitialization.
		/// </summary>
		protected virtual void Finish() { }

		/// <summary>
		///		Used to notify the overridding class that it can update its game logic and
		///		render its scene.
		/// </summary>
		protected virtual void Update() { }

		/// <summary>
		///		Used to notify the overridding class that it should load another map.
		/// </summary>
		public virtual void LoadMap(string file, string password) { }

		#endregion
	}

}