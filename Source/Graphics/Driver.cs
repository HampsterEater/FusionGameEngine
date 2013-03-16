/* 
 * File: Driver.cs
 *
 * This source file contains the declaration of the IGraphicsDriver interface which is
 * used as an intermediate stage between the game engine and graphics APIs, this keeps
 * the graphics APIs isolated from the game.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace BinaryPhoenix.Fusion.Graphics
{
	#region Canvas

    /// <summary>
    ///     Used to notify classes that a canvas needs rendering.
    /// </summary>
    public delegate void CanvasRenderHandler();

	/// <summary>
	///     The graphics canvas class is used to create and keep track of a 
	///		rendering target that is independent of the graphics driver.
	/// </summary>
	public class GraphicsCanvas : IRenderTarget
	{
		#region Members
		#region Variables

		private Control _renderControl = null;
		private IRenderTarget _graphicsCanvas = null;
		private IGraphicsDriver _driver = null;

        private static event CanvasRenderHandler _renderDelegate = null;

		#endregion
		#region Properties

		/// <summary>
		///		Gets the graphics flags used to setup this rendering target.
		/// </summary>
		GraphicsFlags IRenderTarget.Flags
		{
			get { return _graphicsCanvas.Flags; }
		}

		/// <summary>
		///		Gets the width in pixels of this rendering target.
		/// </summary>
		int IRenderTarget.Width
		{
			get { return _graphicsCanvas.Width; }
		}

		/// <summary>
		///		Gets the height in pixels of this rendering target.
		/// </summary>
		int IRenderTarget.Height
		{
			get { return _graphicsCanvas.Height; }
		}

		/// <summary>
		///		Gets the control this canvas is rendering to.
		/// </summary>
		public Control RenderControl
		{
			get { return _renderControl; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Attachs this rendering target to the graphics driver, so that all rendering
		///		is done to it.
		/// </summary>
		bool IRenderTarget.Attach()
		{
			if (GraphicsManager.Driver != _driver)
			{
				((Control)_graphicsCanvas).Dispose();
				_graphicsCanvas = GraphicsManager.Driver.CreateCanvas(_renderControl, _graphicsCanvas.Flags);
				_driver = GraphicsManager.Driver;
			}
			return _graphicsCanvas.Attach();
		}

		/// <summary>
		///		Detachs this rendering target from the graphics driver. 
		/// </summary>
		void IRenderTarget.Detach()
		{
			_graphicsCanvas.Detach();
		}

        /// <summary>
        ///     Renders all canvases at once.
        /// </summary>
        public static void RenderAll()
        {
            if (_renderDelegate != null) _renderDelegate();
        }

        /// <summary>
        ///     Invoked when the scene needs to be presented.
        /// </summary>
        void IRenderTarget.PresentScene()
        {
            _graphicsCanvas.PresentScene();
        }

        /// <summary>
        ///     Invoked when the scene needs to begin rendering.
        /// </summary>
        void IRenderTarget.BeginScene()
        {
            _graphicsCanvas.BeginScene();
        }

        /// <summary>
        ///     Invoked when the scene needs to finish renderinh.
        /// </summary>
        void IRenderTarget.FinishScene()
        {
            _graphicsCanvas.FinishScene();
        }

		/// <summary>
		///		Sets up a new instance of this class with the given values.
		/// </summary>	
		/// <param name="renderControl">Control to render to.</param>
		/// <param name="flags">Flags to setup this control with.</param>
		public GraphicsCanvas(Control renderControl, GraphicsFlags flags, CanvasRenderHandler handler)
		{
			_renderControl = renderControl;
			_graphicsCanvas = GraphicsManager.Driver.CreateCanvas(renderControl, flags);
            if (handler != null) _renderDelegate += handler; // Fix this so its removed when canvas is destroyed!.
            _driver = GraphicsManager.Driver;
			GraphicsManager.RenderTarget = this;
		}

		#endregion
	}

	#endregion
	#region Graphics Driver Classes

	/// <summary>
    ///     The IGraphicsDriver interface is used to provide an abstraction layer above all graphics 
    ///     API calls, which allows the graphics API to be completly isolated from the 
    ///     game, allowing easier extendability and porting. 
    /// </summary>
    public interface IGraphicsDriver 
    {
		#region Properties

		QuadVertexColors VertexColors { get; }
		int	ColorKey { get; set; }
		BlendMode BlendMode { get; set; }
		float[] ScaleFactor { get; set; }
		float[] RotationAngle { get; set; }
		Rectangle Viewport { get; set; }
		int ClearColor { get; set; }
		float[] Offset { get; set; }
		IRenderTarget RenderTarget { get; set; }
        Shader Shader { get; set; }
        int[] Resolution { get; set; }
		float[] ResolutionOffset { get; set; }
		float[] ResolutionScale { get; set; }
        bool DepthBufferEnabled { get; set; }
        RenderMode RenderMode { get; set; }
        bool ThreadSafe { get; }

		#endregion
		#region Methods

		IRenderTarget CreateCanvas(Control control, GraphicsFlags flags);
		void BeginScene();
		void FinishScene();
        void PresentScene();
		void ClearScene();
        void ClearDepthBuffer();
        void RenderMesh(Mesh mesh, float x, float y, float z);
        void RenderImage(Image image, float x, float y, float z, int frame);
        void TileImage(Image image, float x, float y, float z, int frame);
        void RenderRectangle(float x, float y, float z, float width, float height, bool filled);
		void RenderOval(float x, float y, float z, float width, float height, bool filled);
		void RenderLine(float x, float y, float z, float x2, float y2, float z2);
		void RenderPolygon(Vertex[] polyVerts, bool filled);
		IImageFrame CreateImageFrame(PixelMap pixelMap);
        IShader CreateShader(string code);
		void SetResolution(int width, int height, bool keepAspectRatio);
        void SetShaderVariable(string variable, object value);
        PixelMap GrabPixelmap(int x, int y, int width, int height);
        IDriverMesh CreateDriverMesh(VertexMap map);

		#endregion
	}

	#endregion
	#region Interfaces

    /// <summary>
    ///     The IRenderTarget interface is used as a base for all the surfaces capable of being
    ///     rendered on by a graphics API's.
    /// </summary>
    public interface IRenderTarget 
	{
		bool Attach();
		void Detach();
		int Width  { get; }
		int Height { get; }
		GraphicsFlags Flags { get; }
        void PresentScene();
        void BeginScene();
        void FinishScene();
	}

	#endregion
	#region Enumerations

    /// <summary>
    ///     Describes render mode.
    /// </summary>
    public enum RenderMode : int
    {
        Dimensions2,
        Dimensions3
    }

	/// <summary>
	///		Describes a alpha/color blending mode which can be used when rendering.
	/// </summary>
	public enum BlendMode : int
	{
		Solid,
		Mask,
		Alpha,
		Lighten,
		Darken,
		Invert,
		Add,
		Subtract
	}

	/// <summary>
	///     The GraphicsWindowFlags enumerations are used to describe how the 
	///     graphics window is set up when IGraphicsDriver.CreateWindow() is called.
	/// </summary>
	/// <remarks>
	///     All flags are capable of being combined into a bitmask to allow multiple flags
	///     to be used, when adding new flags please make sure they area capable of being 
	///     combined into a bitmask.
	/// </remarks>
	public enum GraphicsFlags : int
	{
		FullScreen = 0x000001
	}

	#endregion
	#region Storage Classes

	/// <summary>
	///		Simple class used to store the color of each vertex in an image quad.
	/// </summary>
	public sealed class QuadVertexColors
	{
		#region Members
		#region Variables

		private int[] _vertexColors = new int[4];

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the color of the given vertex.
		/// </summary>
		/// <param name="index">Index of vertex to be modified.</param>
		public int this[int index]
		{
			get { return _vertexColors[index]; }
			set { _vertexColors[index] = value; }
		}

		/// <summary>
		///		Sets the color of all vertexs in this quad.
		/// </summary>
		public int AllVertexs
		{
			set
			{
				for (int i = 0; i < 4; i++)
					_vertexColors[i] = value;
			}
			get { return _vertexColors[0]; }
		}

		#endregion
		#endregion
	}

    /// <summary>
    ///		Simple class that describes a vertex position in x/y/z coordinates.
    /// </summary>
    public sealed class Vertex
    {
        private float _x, _y, _z;
        private float _tx, _ty;
        private int _color;

        /// <summary>
        ///		Sets or gets the x position of the vertex.
        /// </summary>
        public float X
        {
            get { return _x; }
            set { _x = value; }
        }

        /// <summary>
        ///		Sets or gets the y position of the vertex.
        /// </summary>
        public float Y
        {
            get { return _y; }
            set { _y = value; }
        }

        /// <summary>
        ///		Sets or gets the z position of the vertex.
        /// </summary>
        public float Z
        {
            get { return _z; }
            set { _z = value; }
        }

        /// <summary>
        ///     X position along the texture if loading for use in a mesh.
        /// </summary>
        public float TX
        {
            get { return _tx; }
            set { _tx = value; }
        }

        /// <summary>
        ///     Y position along the texture if loading for use in a mesh.
        /// </summary>
        public float TY
        {
            get { return _ty; }
            set { _ty = value; }
        }

        /// <summary>
        ///     Color of this vertex if loading for use in a mesh.
        /// </summary>
        public int Color
        {
            get { return _color; }
            set { _color = value; }
        }

        /// <summary>
        ///		Creates a new vertex at coordinate x/y/z.
        /// </summary>
        /// <param name="x">Position on the x-axis to create vertex at.</param>
        /// <param name="y">Position on the y-axis to create vertex at.</param>
        /// <param name="z">Position on the z-axis to create vertex at.</param>
        /// <param name="tx">Position on the x axis of the texture.</param>
        /// <param name="ty">Position on the y axis of the texture.</param>
        /// <param name="color">Color of this vertex.</param>
        public Vertex(float x, float y, float z, float tx, float ty, int color)
        {
            _x = x;
            _y = y;
            _z = z;
            _tx = tx;
            _ty = ty;
            _color = color;
        }
        public Vertex(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

    }

	#endregion
}