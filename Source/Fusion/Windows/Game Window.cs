/* 
 * File: Game Window.cs
 *
 * This source file contains the declaration of the game window class and all code regarding
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
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Graphics;
using BinaryPhoenix.Fusion.Engine.Debug;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Scripting;

namespace BinaryPhoenix.Fusion.Engine.Windows
{

	/// <summary>
	///		The game window class is a simple form that allows the user to view the scene.
	/// </summary>
	public partial class GameWindow : Form
	{
		#region Members
		#region Variables

        protected Graphics.Image _renderTarget = null;

		protected GraphicsCanvas _canvas = null;
        protected GraphicsFlags _flags = 0;

        protected bool _renderSceneGraph = true, _renderLoadingScreen = false;
        protected FunctionSymbol _gameScriptRenderFunction = null, _mapScriptRenderFunction = null;

        protected Shader _postProcessingShader = null;

        protected bool _renderThisFrame = true;

		#endregion
        #region Properties

        /// <summary>
        ///		Gets or sets if the scene graph should be rendered.
        /// </summary>
        public bool RenderSceneGraph
        {
            get { return _renderSceneGraph; }
            set { _renderSceneGraph = value; }
        }

        /// <summary>
        ///		Gets or sets if the loading screen should be rendered.
        /// </summary>
        public bool RenderLoadingScreen
        {
            get { return _renderLoadingScreen; }
            set { _renderLoadingScreen = value; }
        }

        /// <summary>
        ///		Gets or sets if this frame should be rendered.
        /// </summary>
        public bool RenderThisFrame
        {
            get { return _renderThisFrame; }
            set { _renderThisFrame = value; }
        }

        /// <summary>
        ///     Gets or sets the current render function of the game script.
        /// </summary>
        public FunctionSymbol GameScriptRenderFunction
        {
            get { return _gameScriptRenderFunction; }
            set { _gameScriptRenderFunction = value; }
        }

        /// <summary>
        ///     Gets or sets the current render function of the map script.
        /// </summary>
        public FunctionSymbol MapScriptRenderFunction
        {
            get { return _mapScriptRenderFunction; }
            set { _mapScriptRenderFunction = value; }
        }

        /// <summary>
        ///     Gets or sets the shader applied during post processing.
        /// </summary>
        public Shader PostProcessingShader
        {
            get { return _postProcessingShader; }
            set { _postProcessingShader = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Renders the game in all its glory!
        /// </summary>
        public void Render()
        {
           /* GraphicsManager.BeginScene();
            GraphicsManager.ClearRenderState();
            GraphicsManager.Viewport = new Rectangle(0, 0, GraphicsManager.Resolution[0], GraphicsManager.Resolution[1]);
            GraphicsManager.ClearColor = unchecked((int)0xFFFF0000);
            GraphicsManager.ClearScene();

            GraphicsManager.VertexColors[0] = unchecked((int)0xFFFFFFFF);
            GraphicsManager.VertexColors[1] = unchecked((int)0xFF00FF00);
            GraphicsManager.VertexColors[2] = unchecked((int)0xFFFF00FF);
            GraphicsManager.VertexColors[3] = unchecked((int)0xFF0000FF);
            GraphicsManager.RenderRectangle(5, 5, 0, 16, 16, true);
            GraphicsManager.RenderRectangle(5, 37, 0, 16, 16, false);

            GraphicsManager.RenderOval(37, 5, 0, 16, 16, true);
            GraphicsManager.RenderOval(37, 37, 0, 16, 16, false);

            GraphicsManager.RenderLine(74, 5, 0, 74 + 16, 5 + 16, 0);
            GraphicsManager.RenderLine(74, 37, 0, 74 + 16, 37 + 16, 0);

            GraphicsManager.FinishScene();
            GraphicsManager.PresentScene();

            return;*/
            //HighPreformanceTimer timer = new HighPreformanceTimer();

            // Don't try and render if we don't have a loading function.
            if (_renderLoadingScreen == true && (Fusion.GlobalInstance.GameScriptProcess == null || Fusion.GlobalInstance.GameScriptProcess.Process == null || Fusion.GlobalInstance.GameScriptProcess.Process.SymbolExists("OnLoadingRender") == false))
                return;

            // Begin rendering.
            GraphicsManager.PushRenderState();
            GraphicsManager.BeginScene();
            GraphicsManager.ClearRenderState();
           // GraphicsManager.ClearColor = unchecked((int)0xFFFF0000);
            GraphicsManager.ClearScene();
            GraphicsManager.Viewport = new Rectangle(0, 0, GraphicsManager.Resolution[0], GraphicsManager.Resolution[1]);

            if (_renderThisFrame == true)
            {
                // Render the current scene graph.
                if (_renderSceneGraph == true)
                {
                    GraphicsManager.PushRenderState();
                    Engine.GlobalInstance.Map.SceneGraph.Render();
                    GraphicsManager.PopRenderState();
                }

                // Do we have a game script render function? 
                if (_gameScriptRenderFunction != null)
                {
                    GraphicsManager.PushRenderState();
                    Fusion.GlobalInstance.GameScriptProcess.Process[0].InvokeFunction(_gameScriptRenderFunction, true, true, true);
                    GraphicsManager.PopRenderState();
                }

                // Do we have a map script render function? 
                if (_mapScriptRenderFunction != null)
                {
                    GraphicsManager.PushRenderState();
                    Fusion.GlobalInstance.MapScriptProcess.Process[0].InvokeFunction(_mapScriptRenderFunction, true, true, true);
                    GraphicsManager.PopRenderState();
                }
            }
            else
                _renderThisFrame = true;

            // Render the loading screen if we have been asked to.
            if (_renderLoadingScreen == true && Fusion.GlobalInstance.GameScriptProcess != null && Fusion.GlobalInstance.GameScriptProcess.Process != null && Fusion.GlobalInstance.GameScriptProcess.Process.SymbolExists("OnLoadingRender"))
            {
                GraphicsManager.PushRenderState();
                Fusion.GlobalInstance.GameScriptProcess.Process[0].InvokeFunction("OnLoadingRender", true, true, true);
                GraphicsManager.PopRenderState();
            }

            // Tell plugins to render.
            Plugin.CallPluginMethod("Render");

            // Render the console.
            GraphicsManager.Resolution = new int[] { ClientSize.Width, ClientSize.Height };
            GraphicsManager.ResolutionOffset = new float[] { 0, 0 };
            GraphicsManager.ResolutionScale = new float[] { 1.0f, 1.0f };
            GraphicalConsole.Render();

            GraphicsManager.FinishScene();
            GraphicsManager.PresentScene();
            GraphicsManager.PopRenderState();
            //System.Console.WriteLine("R: " + (float)timer.DurationMillisecond);
            //System.Console.WriteLine("Spent " + Graphics.Image._tileTime);
            //Graphics.Image._tileTime = 0;
        }

        /// <summary>
        ///     We've been resized! Oh noooz!
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Reason why this event was invoked.</param>
        private void GameWindow_SizeChanged(object sender, EventArgs e)
        {
            if (GraphicsManager.Resolution[0] == 0 || GraphicsManager.Resolution[1] == 0) return;
            GraphicsManager.SetResolution(GraphicsManager.Resolution[0], GraphicsManager.Resolution[1], Engine.GlobalInstance.KeepAspectRatio); 
        }

        /// <summary>
        ///     Resets this window to take the give effect to the given settings.
        /// </summary>
        /// <param name="width">Width of the resolution of this window.</param>
        /// <param name="height">Height of the resolution of this window.</param>
        /// <param name="flags">Decides how the window is created.</param>
        public void Reset(int width, int height, GraphicsFlags flags)
        {
            _flags = flags;
            bool fullScreen = (flags & GraphicsFlags.FullScreen) != 0;
            FormBorderStyle = fullScreen ? FormBorderStyle.None : FormBorderStyle.FixedDialog;
            ClientSize = new Size(fullScreen ? NativeMethods.GetSystemMetrics(0) : width, fullScreen ? NativeMethods.GetSystemMetrics(1) : height);
            Location = new Point(fullScreen ? 0 : (NativeMethods.GetSystemMetrics(0) - ClientSize.Width) / 2, fullScreen ? 0 : (NativeMethods.GetSystemMetrics(1) - ClientSize.Height) / 2);
            GraphicsManager.SetResolution(width, height, Engine.GlobalInstance.KeepAspectRatio);
            //_renderTarget = new Graphics.Image((int)(GraphicsManager.Resolution[0] * GraphicsManager.ResolutionScale[0]), (int)(GraphicsManager.Resolution[1] * GraphicsManager.ResolutionScale[1]), ImageFlags.Dynamic);
        }

		/// <summary>
		///		Initializes a new instance of this class.
		/// </summary>
        /// <param name="width">Width of the resolution of this window.</param>
        /// <param name="height">Height of the resolution of this window.</param>
        /// <param name="flags">Decides how the window is created.</param>
		public GameWindow(int width, int height, GraphicsFlags flags)
		{
			InitializeComponent();
            _canvas = new GraphicsCanvas(this, 0, new CanvasRenderHandler(Render));
            GraphicsManager.RenderTarget = _canvas;
            Reset(width, height, flags);
		}

		#endregion
	}
}