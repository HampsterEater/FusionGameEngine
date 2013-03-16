/*
 * File: Updater.cs
 *
 * Contains all the code used to update a specific piece of software.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Threading;
using System.Diagnostics;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Resources;
using BinaryPhoenix.Fusion.Updater.Windows;

namespace BinaryPhoenix.Fusion.Updater
{

	/// <summary>
	///		Used to update a given piece of software to the latest build.
	/// </summary>
	public class Updater
	{
		#region Members
		#region Variables

        private static Updater _globalInstance = null;
		private string _configFilePath = "Fusion.xml";
		private string _outputFilePath = "Fusion.log";

		private string _relativeBasePath = "FusionApps", _basePath = "";

		private XmlConfigFile _engineConfigFile = null;

		private UpdaterWindow _window;

#if DEBUG || PROFILE
		private bool _logDebugOutput = true;
#else
		private bool _logDebugOutput = false;
#endif

		private string[] _commandLineArguments;
		private string _updateFile  = "";
        private string _finishFile = "";

        private bool _createUpdate = false;
        private bool _buildSDK = false;

		#endregion
		#region Properties

        /// <summary>
        ///		Gets or sets the currently running instance of the Updater class.
        /// </summary>
        public static Updater GlobalInstance
        {
            get { return _globalInstance; }
            set { _globalInstance = value; }
        }

		/// <summary>
		///		Get or sets the update file to use for updating.
		/// </summary>
		public string UpdateFile
		{
			get { return _updateFile; }
			set { _updateFile = value; }
		}

        /// <summary>
        ///     Gets or sets if we should create an update as opposed to installing one.
        /// </summary>
        public bool CreateUpdate
        {
            get { return _createUpdate; }
            set { _createUpdate = value; }
        }

        /// <summary>
        ///     Gets or sets if the SDK should be included in the created update.
        /// </summary>
        public bool BuildSDK
        {
            get { return _buildSDK; }
            set { _buildSDK = value; }
        }

        /// <summary>
        ///		Gets or sets the xml configuration file that contains the engine settings.
        /// </summary>
        public XmlConfigFile EngineConfigFile
        {
            get { return _engineConfigFile; }
            set { _engineConfigFile = value; }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		When called the updater will start running.
		/// </summary>
		public void Run(string[] cmdArgs)
		{
			bool restartEngine = false; // This is used to flag if the engine should
			// restart when the game finishs, this is 
			// mainly used to handle errors gracefully.

			_commandLineArguments = cmdArgs;

			// Initialize the game engine.
			Application.EnableVisualStyles();
			if (Initialize() == false)
			{
                DeInitialize();
				return;
			}

			// Start the application message loop.
			Application.Run(_window);

			// DeInitialize the game engine.
            DeInitialize();

			if (restartEngine == true) Run(cmdArgs);
		}

		/// <summary>
		///		Initializes the updater and sets up all the sub systems.
		/// </summary>
		/// <returns>True if initialization was sucessfull.</returns>
		private bool Initialize()
		{
			#region Command Line & Configuration Parsing

			// Parse the command lines for configuration options.
			ParseCommandLines(_commandLineArguments, true);

			// First things first lets read in the engine configuration file.
			_engineConfigFile = new XmlConfigFile(_configFilePath);

			// Get any configuration values relative to initializing the engine.
			_relativeBasePath = _engineConfigFile["basepath", _basePath];
			_basePath = _relativeBasePath;

			// Parse the command lines for anything besides configuration options.
			ParseCommandLines(_commandLineArguments, false);

            // Check an update file has been specified.
            if (_updateFile == "")
            {
                MessageBox.Show("Unable to update, no update file was specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

			#endregion
			#region Sub System Initialization

			// Create the updater's window.
			_window = new UpdaterWindow();
			_window.Show();

			#endregion
			return true;
		}

		/// <summary>
		///		DeInitializes the game engine and closes down all sub systems.
		/// </summary>
		private void DeInitialize()
		{
			// Save any changes made to the engine xml configuration file.
			//_engineConfigFile.Save(_configFilePath);

            // Boot up the finish file!
            if (_finishFile != "" && File.Exists(_finishFile))
            {
                Process process = new Process();
                process.StartInfo.FileName = _finishFile;
                process.StartInfo.Verb = "Open";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                process.Start();
            }
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
					// Sets the update file to use.
					// Syntax: -file:Name
					case "-file":
						if (onlyConfig == true) continue;
						if (value.Length == 1)
							_updateFile = value[0].ToLower().Trim();
						else
							throw new Exception("Invalid argument length. Usage: -file:<file>");
						break;

                    // Sets the file to boot up when we finish use.
                    // Syntax: -finishfile:Name
                    case "-finishfile":
                        if (onlyConfig == true) continue;
                        if (value.Length == 1)
                            _finishFile = value[0].ToLower().Trim();
                        else
                            throw new Exception("Invalid argument length. Usage: -finishfile:<file>");
                        break;

                    // Determines if we should create an update or install one.
                    // Syntax: -create
                    case "-create":
                        if (onlyConfig == true) continue;
                        if (value.Length == 0)
                            _createUpdate = true;
                        else
                            throw new Exception("Invalid argument length. Usage: -create");
                        break;

                    // Determines if we build the sdk into the created update.
                    // Syntax: -buildsdk
                    case "-buildsdk":
                        if (onlyConfig == true) continue;
                        if (value.Length == 0)
                            _buildSDK = true;
                        else
                            throw new Exception("Invalid argument length. Usage: -buildsdk");
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
				}
			}
		}

		/// <summary>
		///		Entry point of this application.
		/// </summary>
		/// <param name="args">Command lines passed to this application.</param>
		[STAThread]
		public static void Main(string[] args)
		{
            _globalInstance = new Updater();
			_globalInstance.Run(args);
		}

		#endregion
	}

}