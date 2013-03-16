/*
 * File: Fusion.cs
 *
 * Contains all the code declarations of the main game engine classes.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Threading;
using BinaryPhoenix.Fusion.Engine;
using BinaryPhoenix.Fusion.Engine.Entitys;
using BinaryPhoenix.Fusion.Engine.Processes;
using BinaryPhoenix.Fusion.Graphics;
//using BinaryPhoenix.Fusion.Graphics.Direct3D9Driver;
//using BinaryPhoenix.Fusion.Graphics.PixelMapFactorys;
using BinaryPhoenix.Fusion.Audio;
//using BinaryPhoenix.Fusion.Audio.DirectSound9Driver;
//using BinaryPhoenix.Fusion.Audio.SampleFactorys;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Languages;
using BinaryPhoenix.Fusion.Runtime.Scripting;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Runtime.StreamFactorys;
using BinaryPhoenix.Fusion.Runtime.Events;
using BinaryPhoenix.Fusion.Runtime.Collision;
using BinaryPhoenix.Fusion.Runtime.Resources;
using BinaryPhoenix.Fusion.Runtime.Processes;
using BinaryPhoenix.Fusion.Input;
//using BinaryPhoenix.Fusion.Input.DirectInput9Driver;
using BinaryPhoenix.Fusion.Editor.Windows;
using System.Diagnostics;
using System.Reflection;

namespace BinaryPhoenix.Fusion.Editor
{

	/// <summary>
	///		Used as base for the actual game, contains all the 
	///		nitty gritty of running the engine.
	/// </summary>
	public class Editor : Engine.Engine
	{
		#region Members
		#region Variables

		private static Editor _globalInstance = null;

		private EditorWindow _window;
		private CameraNode _camera;

		private bool _windowClosing = false;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the currently running instance of the Editor class.
		/// </summary>
		public new static Editor GlobalInstance
		{
			get { return _globalInstance; }
			set { _globalInstance = value; }
		}

		/// <summary>
		///		Gets the camera node attached to the scene graph.
		/// </summary>
		public CameraNode CameraNode
		{
			get { return _camera; }
		}

        /// <summary>
        ///		Gets the window currently being shown.
        /// </summary>
        public EditorWindow Window
        {
            get { return _window; }
        }


        /// <summary>
        ///     If this returns true the game will not be updated this frame and time will be handed back to the OS.
        /// </summary>
        public override bool HandBackTimeToOS
        {
            get
            {
                return false; //(_window != null && (_window.WindowState == FormWindowState.Minimized || _window.Focused == false));
            }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Called by the base engine class when its safe to begin initialization.
		/// </summary>
		protected override bool Begin()
		{
            // We alwasy want to run with the editor state!
            ScriptExecutionProcess.DefaultToEditorState = true;

            // Bind all function sets to the global virtual machine.
            NativeFunctionSet.RegisterCommandSetsToVirtualMachine(VirtualMachine.GlobalInstance);

            // Set the output file.
            DebugLogger.OutputFile = _logPath + "\\Editor " + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + ".log";
            
            // Ahhhh, no game given!!
            if (_gameName == "")
            {
                MessageBox.Show("No game was specified. Please pass a command line of -game:<ident> when running this application.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return true;
            }

			// Make sure this game has not already been compiled.
			if (_gameConfigFile["resources:usepakfiles", "0"] == "1")
				DebugLogger.WriteLog("Unable to edit game, game's media has already been compiled into pak files.", LogAlertLevel.FatalError);

			_fpsLimit = int.Parse(_engineConfigFile["graphics:fpslimit", _fpsLimit.ToString()]);

            // Disable all resource caching.
            //ResourceManager.ResourceCacheEnabled = false;

			// Create the editor's window.
			_window = new EditorWindow();
			_window.FormClosing += new FormClosingEventHandler(OnClosing);
			_window.Show();
			AudioManager.Driver.AttachToControl(_window);
			InputManager.Driver.AttachToControl(_window);

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
                DebugLogger.WriteLog("Unable to find language pack for language " + _language + ".", LogAlertLevel.FatalError);

			// Setup a camera that we can view the scene from.
			_camera = new CameraNode("Root Camera");
			_map.SceneGraph.AttachCamera(_camera);
			_camera.BackgroundImage = new Image(ReflectionMethods.GetEmbeddedResourceStream("grid.png"), 0);
			_camera.ClearColor = unchecked((int)0xFFACACAC);

			// Show the tip-of-the-day window.
			if (_engineConfigFile["editor:showtipsonstartup", "1"] == "1")
				(new TipOfTheDayWindow()).ShowDialog();

            // Disable collision processing.
            CollisionManager.Enabled = false;

            return false;
		}

		/// <summary>
		///		Called by the base engine class when its safe to update and render the scene.
		/// </summary>
		protected override void Update()
		{
        }

		/// <summary>
		///		Called by the base engine class when its safe to begin deinitialization.
		/// </summary>
		protected override void Finish()
		{

		}

		/// <summary>
		///		Called when the editor window is closed.
		/// </summary>
		/// <param name="sender">Object that called this method.</param>
		/// <param name="eventArgs">Arguments describing why this method was called.</param>
		private void OnClosing(object sender, FormClosingEventArgs eventArgs)
		{
			if (_windowClosing == false)
			{
				DebugLogger.WriteLog("Window closed.");
				_windowClosing = true;
                Engine.Engine.GlobalInstance.ClosingDown = true;
			}
		}

		/// <summary>
		///		Entry point of this application.
		/// </summary>
		/// <param name="args">Command lines passed to this application.</param>
		[STAThread]
		public static void Main(string[] args)
		{
			Editor.GlobalInstance = new Editor();
			Engine.Engine.GlobalInstance = (Engine.Engine)Editor.GlobalInstance;
			Editor.GlobalInstance.Run(args);
		}

		#endregion
	}

}