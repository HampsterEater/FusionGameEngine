/* 
 * File: Manager.cs
 *
 * This source file contains the declaration of the GraphicsManager class which 
 * contains static versions of every function in the IGraphicsDriver class to make
 * for easier access and tracking.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Runtime.Resources;
//using BinaryPhoenix.Fusion.Graphics.Direct3D9Driver;

namespace BinaryPhoenix.Fusion.Graphics
{
	#region Graphics Manager Classes

	/// <summary>
	///		The graphics manager is used to keep track of the current driver
	///		being used to render graphics to the screen, it also provides an abstration
	///		layer between the driver and any graphics calls. This
	///		is usfull as it means the graphics driver does not have to be passed between
	///		different classes so that each can directly call the drivers methods. 
	/// </summary>
	public static class GraphicsManager
	{
		#region Members
		#region Variables

		private static IGraphicsDriver _driver;

		private static int _fps, _fpsTicks, _renderTime;
		private static HighPreformanceTimer _renderTimer = new HighPreformanceTimer(), _fpsTimer = new HighPreformanceTimer();

		private static RenderState[] _renderStateStack = new RenderState[] 
                { 
                    new RenderState(), 
                    new RenderState(), 
                    new RenderState(), 
                    new RenderState(), 
                    new RenderState(), 
                    new RenderState(), 
                    new RenderState(), 
                    new RenderState(), 
                    new RenderState(), 
                    new RenderState(), 
                };
        private static int _renderStateStackDepth = 0;

		private static BitmapFont _currentFont = null;
		private static BitmapFont _defaultFont = null;
			
		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the current graphics driver being used.
		/// </summary>
		public static IGraphicsDriver Driver
		{
			get { return _driver;   }
			set { _driver = value;  }
		}

		/// <summary>
		///		Gets the current frames per second this application is running at.
		/// </summary>
		public static int FramesPerSecond
		{
			get { return _fps; }
		}

		/// <summary>
		///		Gets how long it took to render the last frame.
		/// </summary>
		public static int RenderTime
		{
			get { return _renderTime; }
		}

		/// <summary>
		///		Gets a QuadVertexColors structure which contains the vertex 
		///		colors used when rendering an image quad.
		/// </summary>
		public static QuadVertexColors VertexColors
		{
			get { return _driver.VertexColors; }
		}

		/// <summary>
		///		Gets or sets the default color-key used to mask images as they are loaded.
		/// </summary>
		public static int ColorKey
		{
			get { return _driver.ColorKey; }
			set { _driver.ColorKey = value;}
		}

		/// <summary>
		///		Gets or sets the scaling factor to apply to any images drawn.
		/// </summary>
		public static float[] ScaleFactor
		{
			get { return _driver.ScaleFactor; }
			set { _driver.ScaleFactor = value;  }
		}

		/// <summary>
		///		Gets or sets the rotation angle to apply to any images drawn.
		/// </summary>
		public static float[] RotationAngle
		{
			get { return _driver.RotationAngle;  }
			set { _driver.RotationAngle = value; }
		}

		/// <summary>
		///		Gets or sets the offset applyed when rendering images.
		/// </summary>
		public static float[] Offset
		{
			get { return _driver.Offset;  }
			set { _driver.Offset = value; }
		}

		/// <summary>
		///		Gets or sets the blend mode used to blend colors together.
		/// </summary>
		public static BlendMode BlendMode
		{
			get { return _driver.BlendMode; }
			set { _driver.BlendMode = value;  }
		}

		/// <summary>
		///		Gets or sets the viewport (clipping rectangle) used to clip vertexs outside its bounds. 
		/// </summary>
		public static Rectangle Viewport
		{
			get { return _driver.Viewport;  }
			set { _driver.Viewport = value; }
		}

		/// <summary>
		///		Gets or sets the color used to blank the screen when ClearBuffer is called.
		/// </summary>
		public static int ClearColor
		{
			get { return _driver.ClearColor; }
			set { _driver.ClearColor = value; }
		}

		/// <summary>
		///		Sets the target this graphics driver should render to, this is normally an
		///		image or window.
		/// </summary>
		public static IRenderTarget RenderTarget
		{
			get { return _driver.RenderTarget;  }
			set { _driver.RenderTarget = value; }
		}

        /// <summary>
        ///		Sets the current shader this graphics driver should apply.
        /// </summary>
        public static Shader Shader
        {
            get { return _driver.Shader; }
            set { _driver.Shader = value; }
        }

		/// <summary>
		///		Gets the dimensions of the resolution of the current render target.
		/// </summary>
		public static int[] Resolution
		{
			get { return _driver.Resolution; }
			set { _driver.Resolution = value; }
		}

		/// <summary>
		///		Gets the offset of the resolution of the current render target.
		/// </summary>
		public static float[] ResolutionOffset
		{
			get { return _driver.ResolutionOffset; }
			set { _driver.ResolutionOffset = value; }
		}

		/// <summary>
		///		Gets the scale of the resolution of the current render target.
		/// </summary>
		public static float[] ResolutionScale
		{
			get { return _driver.ResolutionScale; }
			set { _driver.ResolutionScale = value; }
		}

		/// <summary>
		///		Gets or sets the current bitmap font used to render text.
		/// </summary>
		public static BitmapFont BitmapFont
		{
			get { return _currentFont; }
			set { _currentFont = value; }
		}

		/// <summary>
		///		Gets or sets the default bitmap font used to render text if no other font is available.
		/// </summary>
		public static BitmapFont DefaultBitmapFont
		{
			get { return _defaultFont; }
			set { _defaultFont = value; }
		}

		/// <summary>
		///		Gets or sets the render state stack.
		/// </summary>
		public static RenderState[] RenderStateStack
		{
			get { return _renderStateStack; }
			set { _renderStateStack = value; }
		}

        /// <summary>
        ///		Gets or sets if the depth buffer is enabled or not.
        /// </summary>
        public static bool DepthBufferEnabled
        {
            get { return _driver.DepthBufferEnabled; }
            set { Driver.DepthBufferEnabled = value; }
        }

        /// <summary>
        ///     Gets or sets the current rendering mode.
        /// </summary>
        public static RenderMode RenderMode
        {
            get { return _driver.RenderMode; }
            set { _driver.RenderMode = value; }
        }

        /// <summary>
        ///     Gets if the current graphics driver is thread safe.
        /// </summary>
        public static bool ThreadSafe
        {
            get { return _driver.ThreadSafe; }
        }

		#endregion
		#endregion
		#region Static Functions

		/// <summary>
		///		Creates a canvas that can be rendered to by the current driver.
		/// </summary>
		/// <param name="control">Control to render this canvases graphics to.</param>
		/// <returns>New renderable canvas.</returns>
		public static GraphicsCanvas CreateCanvas(Control control, GraphicsFlags flags, CanvasRenderHandler handler)
		{
			GraphicsCanvas canvas = new GraphicsCanvas(control, flags, handler);
			ClearRenderState();
			return canvas;
		}

        /// <summary>
        ///     Sets a variable contained in all shaders used by the current graphics driver.
        /// </summary>
        /// <param name="variable">Name of variable to set.</param>
        /// <param name="value">Value to set variable to.</param>
        public static void SetShaderVariable(string variable, object value)
        {
            _driver.SetShaderVariable(variable, value);
        }

		/// <summary>
		///		Sets the emulated resolution of the current rendering context.
		/// </summary>
		/// <param name="width">Width to set resolution to.</param>
		/// <param name="height">Height to set resolution to.</param>
		/// <param name="keepAspectRatio">If set to true the aspect ratio of the resolution will be kept.</param>
		public static void SetResolution(int width, int height, bool keepAspectRatio)
		{
			_driver.SetResolution(width, height, keepAspectRatio);
		}

		/// <summary>
		///     ClearBuffer will clear the current rendering target to the color
		///     previously specificed in ClearColor.
		/// </summary>
		public static void ClearScene()
		{
			_driver.ClearScene();
		}

        /// <summary>
        ///     Clears the depth buffer of infromation.
        /// </summary>
        public static void ClearDepthBuffer()
        {
            _driver.ClearDepthBuffer();
        }

		/// <summary>
		///		This will setup the drawing buffers so that they are ready for you to render on 
		///		them this must be called before you can render anything/
		/// </summary>
		public static void BeginScene()
		{
            // Track render timer.
            _renderTimer.Restart();

			// Notify the driver of the begining scene.
			_driver.BeginScene();
		}

		/// <summary>
		///		Partner of BeginScene, must be called after rendering scene.
		/// </summary>
		public static void FinishScene()
		{
			// Track render timer.
			_renderTime = (int)_renderTimer.DurationMillisecond;

			// Track render timer.
			_driver.FinishScene();

			// Track fps.
			if (_fpsTimer.DurationMillisecond > 1000)
			{
				_fps = _fpsTicks;
				_fpsTicks = 0;
				_fpsTimer.Restart();
			}
			else
				_fpsTicks++;
		}

        /// <summary>
        ///		When called the backbuffer will be switched the the frontbuffer.
        /// </summary>
        public static void PresentScene()
        {
            _driver.PresentScene();
        }
        
		/// <summary>
		///		Loads a bitmap font from an xml declaration file.
		/// </summary>
		/// <param name="path">Memory location (or url) of font file to load.</param>
        /// <param name="cache">If set to true this font is added to the resource cache to save loading
        ///                     time when this font is requested again.</param>
		/// <returns>A new instance of the BitmapFont class if the font can be loaded.</returns>
        public static BitmapFont LoadFont(object path, bool cache)
		{
            if (ResourceManager.ResourceIsCached(path.ToString()) == true)
            {
                DebugLogger.WriteLog("Loading font " + path.ToString() + " from cache.");
                return (BitmapFont)ResourceManager.RetrieveResource(path.ToString());
            }

            DebugLogger.WriteLog("Loading font from " + path.ToString() + ".");
            BitmapFont font = new BitmapFont(path);
            if (cache == true) ResourceManager.CacheResource(path.ToString(), font);
            return font;
        }
        public static BitmapFont LoadFont(object path)
        {
            return LoadFont(path, true);
        }

        /// <summary>
        ///		Loads an mesh as renderable data.
        /// </summary>
        /// <param name="path">Memory location (or url) of mesh file to load.</param>
        /// <param name="cache">If set to true this image is added to the resource cache to save loading
        ///                     time when this image is requested again.</param>
        /// <param name="flags">Describes how this image should be loaded, rendered and stored.</param>
        /// <returns>A new instance of the Image class if the image file can be loaded.</returns>
        public static Mesh LoadMesh(object path, MeshFlags flags, bool cache)
        {
            if (ResourceManager.ResourceIsCached(path.ToString()) == true)
            {
                DebugLogger.WriteLog("Loading mesh " + path.ToString() + " from cache.");
                return (Mesh)ResourceManager.RetrieveResource(path.ToString());
            }

            DebugLogger.WriteLog("Loading mesh from " + path.ToString() + ".");
            Mesh mesh = new Mesh(path, flags);
            if (cache == true)
            {
                if ((flags & MeshFlags.Dynamic) != 0)
                    throw new Exception("Dynamic meshs cannot be cached.");
                ResourceManager.CacheResource(path.ToString(), mesh);
            }
            return mesh;
        }
        public static Mesh LoadMesh(object path, MeshFlags flags)
        {
            return LoadMesh(path, flags, true);
        }

		/// <summary>
		///		Loads an image as a single frame of renderable data.
		/// </summary>
		/// <param name="path">Memory location (or url) of image file to load.</param>
        /// <param name="cache">If set to true this image is added to the resource cache to save loading
        ///                     time when this image is requested again.</param>
        /// <param name="flags">Describes how this image should be loaded, rendered and stored.</param>
		/// <returns>A new instance of the Image class if the image file can be loaded.</returns>
		public static Image LoadImage(object path, ImageFlags flags, bool cache)
		{
            if (ResourceManager.ResourceIsCached(path.ToString()) == true)
            {
                DebugLogger.WriteLog("Loading image " + path.ToString() + " from cache.");
                Image cacheImage = (Image)ResourceManager.RetrieveResource(path.ToString());
                if (cacheImage.Width == cacheImage.FullWidth &&
                    cacheImage.Height == cacheImage.FullHeight &&
                    cacheImage.HorizontalSpacing == 0 &&
                    cacheImage.VerticalSpacing == 0)
                    return cacheImage;
            }

            DebugLogger.WriteLog("Loading image from " + path.ToString() + ".");
			Image image = new Image(path, flags);
            if (cache == true)
            {
                if ((flags & ImageFlags.Dynamic) != 0)
                    throw new Exception("Dynamic images cannot be cached.");
                ResourceManager.CacheResource(path.ToString(), image);
            }
            return image;
        }
        public static Image LoadImage(object path, ImageFlags flags)
        {
            return LoadImage(path, flags, true);
        }

        /// <summary>
        ///     Loads a new shader from a  file.
        /// </summary>
        /// <param name="path">URL of shader to load.</param>
        /// <returns>Newly loaded shader.</returns>
        public static Shader LoadShader(object path)
        {
            if (ResourceManager.ResourceIsCached(path.ToString()) == true)
            {
                DebugLogger.WriteLog("Loading shader " + path.ToString() + " from cache.");
                return (Shader)ResourceManager.RetrieveResource(path.ToString());
            }

            DebugLogger.WriteLog("Loading shader from " + path.ToString() + ".");
            return new Shader(path);
        }

		/// <summary>
		///		Loads an image with multiple frames of renderable data, each
		///		frame is of size CellW x CellH, with no spacing.
		/// </summary>
		/// <param name="path">Memory location (or url) of image file to load.</param>
		/// <param name="cellW">Width of each cell in image.</param>
		/// <param name="cellH">Height of each cell in image.</param>
        /// <param name="cache">If set to true this image is added to the resource cache to save loading
        ///                     time when this image is requested again.</param>
        /// <param name="flags">Describes how this image should be loaded, rendered and stored.</param>
		/// <returns>A new image instance containing the image data found at the given url.</returns>
		public static Image LoadImage(object path, int cellW, int cellH, ImageFlags flags, bool cache)
		{
            if (ResourceManager.ResourceIsCached(path.ToString()) == true)
            {
                DebugLogger.WriteLog("Loading image " + path.ToString() + " from cache.");
                Image cacheImage = (Image)ResourceManager.RetrieveResource(path.ToString());
                if (cacheImage.Width == cellW &&
                    cacheImage.Height == cellH &&
                    cacheImage.HorizontalSpacing == 0 &&
                    cacheImage.VerticalSpacing == 0)
                    return cacheImage;
            }

            DebugLogger.WriteLog("Loading image from " + path.ToString() + ".");
            Image image = new Image(path, cellW, cellH, flags);
            if (cache == true)
            {
                if ((flags & ImageFlags.Dynamic) != 0)
                    throw new Exception("Dynamic images cannot be cached.");
                ResourceManager.CacheResource(path.ToString(), image);
            }
            return image;
		}
        public static Image LoadImage(object path, int cellW, int cellH, ImageFlags flags)
        {
            return LoadImage(path, cellW, cellH, flags, true);
        }

		/// <summary>
		///		Loads an image with multiple frames of renderable data, each
		///		frame is of size CellW x CellH, with a horizontal seperation of cellHSeperation
		///		and a vertical seperation of cellVSeperation.
		/// </summary>
		/// <param name="path">Memory location (or url) of image file to load.</param>
		/// <param name="cellW">Width of each cell in image.</param>
		/// <param name="cellH">Height of each cell in image.</param>
		/// <param name="cellHSeperation">Vertical seperation of each cell in image.</param>
		/// <param name="cellVSeperation">Horizontal seperation of each cell in image.</param>
        /// <param name="cache">If set to true this image is added to the resource cache to save loading
        ///                     time when this image is requested again.</param>
        /// <param name="flags">Describes how this image should be loaded, rendered and stored.</param>
        /// <returns>A new image instance containing the image data found at the given url.</returns>
		public static Image LoadImage(object path, int cellW, int cellH, int cellHSeperation, int cellVSeperation, ImageFlags flags, bool cache)
		{
            if (ResourceManager.ResourceIsCached(path.ToString()) == true)
            {
                DebugLogger.WriteLog("Loading image " + path.ToString() + " from cache.");
                Image cacheImage = (Image)ResourceManager.RetrieveResource(path.ToString());
                if (cacheImage.Width == cellW && 
                    cacheImage.Height == cellH && 
                    cacheImage.HorizontalSpacing == cellHSeperation &&
                    cacheImage.VerticalSpacing == cellVSeperation)
                    return cacheImage;
            }

            DebugLogger.WriteLog("Loading image from " + path.ToString() + ".");
            Image image = new Image(path, cellW, cellH, cellHSeperation, cellVSeperation, flags);
            if (cache == true)
            {
                if ((flags & ImageFlags.Dynamic) != 0)
                    throw new Exception("Dynamic images cannot be cached.");
                ResourceManager.CacheResource(path.ToString(), image);
            }
            return image;
		}
        public static Image LoadImage(object path, int cellW, int cellH, int cellHSeperation, int cellVSeperation, ImageFlags flags)
        {
            return LoadImage(path, cellW, cellH, cellHSeperation, cellVSeperation, flags, true);
        }

		/// <summary>
		///		Creates a blank image of size Width x Height with Frames amount of frames.
		/// </summary>
		/// <param name="width">Width of image to create.</param>
		/// <param name="height">Height of image to create.</param>
		/// <param name="frames">Amount of frames to create in image.</param>
        /// <param name="flags">Describes how this image should be loaded, rendered and stored.</param>
		/// <returns>A new Image instance.</returns>
		public static Image CreateImage(int width, int height, int frames, ImageFlags flags)
		{
			return new Image(width, height, frames, flags);
		}

		/// <summary>
		///		Creates a blank image of size Width x Height with 1 frame.
		/// </summary>
		/// <param name="width">Width of image to create.</param>
		/// <param name="height">Height of image to create.</param>
        /// <param name="flags">Describes how this image should be loaded, rendered and stored.</param>
		/// <returns>A new Image instance.</returns>
        public static Image CreateImage(int width, int height, ImageFlags flags)
		{
			return new Image(width, height, flags);
		}

		/// <summary>
		///		Saves the given image to an image file.
		/// </summary>
		/// <param name="url">Location to save the image to.</param>
		/// <param name="image">Image to save to file.</param>
		/// <param name="flags">Bitmask of flags describing how to save the image/</param>
		/// <param name="frame">Index of frame to save.</param>
		/// <returns></returns>
		public static bool SaveImage(object url,Image image,PixelMapSaveFlags flags,int frame)
		{
			return PixelMapFactory.SavePixelMap(url,image[frame].PixelMap,flags);
		}
		public static bool SaveImage(object url, Image image, PixelMapSaveFlags flags)
		{
			return SaveImage(url, image, flags, 0);
		}
		public static bool SaveImage(object url, Image image)
		{
			return SaveImage(url, image, 0, 0);
		}

		/// <summary>
		///     Renders a given image at the given position.
		///		Color, scale, rotation and offset all effect the rendering	of an image.
		/// </summary>
		/// <param name="image">Image to render.</param>
		/// <param name="x">Position on the x-axis to render image.</param>
		/// <param name="y">Position on the y-axis to render image.</param>
		/// <param name="z">Position on the z-axis to render image.</param>
		/// <param name="frame">Frame of image to render.</param>
		public static void RenderImage(Image image, float x, float y, float z, int frame)
		{
			_driver.RenderImage(image, x, y, z, frame);
        }
		public static void RenderImage(Image image, float x, float y, float z)
		{
			RenderImage(image, x, y, z, 0);
		}

        /// <summary>
        ///		Creates a blank mesh.
        /// </summary>
        /// <param name="flags">Describes how this image mesh be loaded, rendered and stored.</param>
        /// <returns>A new mesh instance.</returns>
        public static Mesh CreateMesh(MeshFlags flags)
        {
            return new Mesh(flags);
        }

        /// <summary>
        ///     Renders a given mesh at the given position.
        ///		Color, scale, rotation and offset all effect the rendering of an mesh.
        /// </summary>
        /// <param name="mesh">Mesh to render.</param>
        /// <param name="x">Position on the x-axis to render mesh.</param>
        /// <param name="y">Position on the y-axis to render mesh.</param>
        /// <param name="z">Position on the z-axis to render mesh.</param>
        public static void RenderMesh(Mesh mesh, float x, float y, float z)
        {
            _driver.RenderMesh(mesh, x, y, z);
        }

        /// <summary>
        ///		Saves the given mesh to an mesh file.
        /// </summary>
        /// <param name="url">Location to save the mesh to.</param>
        /// <param name="mesh">Mesh to save to file.</param>
        /// <param name="flags">Bitmask of flags describing how to save the mesh.</param>
        public static bool SaveMesh(object url, Mesh mesh, VertexMapSaveFlags flags)
        {
            return VertexMapFactory.SaveVertexMap(url, mesh.VertexMap, flags);
        }

		/// <summary>
		///		Renders a given string of text to the screen with the current font/ 
		/// </summary>
		/// <param name="text">String of text to render to screen.</param>
		/// <param name="x">Position on the x-axis to start rendering text.</param>
		/// <param name="y">Position on the y-axis to start rendering text.</param>
		/// <param name="z">Position on the z-axis to start rendering text.</param>
		/// <param name="bfCode">If set to true BFCode in the text will be parsed.</param>
		public static void RenderText(string text, float x, float y, float z, bool bfCode)
		{
            if (_currentFont == null)
				_defaultFont.Render(text, x, y, z, bfCode);
			else
				_currentFont.Render(text, x, y, z, bfCode);
		}
		public static void RenderText(string text, float x, float y, float z)
		{
			RenderText(text, x, y, z, false);
		}

        /// <summary>
        ///		Returns the position in pixels of the character in a given piece of text.
        /// </summary>
        /// <param name="text">Text that character resides in.</param>
        /// <param name="characterIndex">Index of character to check position of.</param>
        /// <param name="bfCode">If set to true BFCode will be taken into account.</param>
        /// <returns>Position in pixels of the character.</returns>
        public static System.Drawing.PointF TextCharacterPosition(string text, int characterIndex, bool bfCode)
        {
            if (_currentFont == null)
                return _defaultFont.TextCharacterPosition(text, characterIndex, bfCode);
            else
                return _currentFont.TextCharacterPosition(text, characterIndex, bfCode);
        }
        public static System.Drawing.PointF TextCharacterPosition(string text, int characterIndex)
        {
            return TextCharacterPosition(text, characterIndex, false);
        }

		/// <summary>
		///		Returns the width in pixels of the given piece of text, if it was rendered
		///		by the current bitmap font.
		/// </summary>
		/// <param name="text">Text to measure width of.</param>
		/// <param name="bfCode">If set to true BFCode will be taken into account.</param>
		/// <returns>Width in pixels of the given text.</returns>
		public static int TextWidth(string text, bool bfCode)
		{
			if (_currentFont == null)
				return _defaultFont.TextWidth(text, bfCode);
			else
				return _currentFont.TextWidth(text, bfCode);
		}
		public static int TextWidth(string text)
		{
			return TextWidth(text, false);
		}

		/// <summary>
		///		Returns the height in pixels of the given piece of text, if it was rendered
		///		by the current bitmap font.
		/// </summary>
		/// <param name="text">Text to measure height of.</param>
		/// <param name="bfCode">If set to true BFCode will be taken into account.</param>
		/// <returns>Height in pixels of the given text.</returns>
		public static int TextHeight(string text, bool bfCode)
		{
			if (_currentFont == null)
				return _defaultFont.TextHeight(text, bfCode);
			else
				return _currentFont.TextHeight(text, bfCode);
		}
		public static int TextHeight(string text)
		{
			return TextHeight(text, false);
		}


		/// <summary>
		///     Tiles a given image at the given position.
		///		Color, scale, rotation and offset all effect the rendering of an image.
		/// </summary>
		/// <param name="image">Image to tile.</param>
		/// <param name="x">Position on the x-axis to start tiling image.</param>
		/// <param name="y">Position on the y-axis to start tiling image.</param>
		/// <param name="z">Position on the z-axis to start tiling image.</param>
		/// <param name="frame">Frame of image to tile.</param>
		public static void TileImage(Image image, float x, float y, float z, int frame)
		{
            image.Tile(x, y, z, frame);
		}
		public static void TileImage(Image image, float x, float y, float z)
		{
			TileImage(image, x, y, z, 0);
		}

		/// <summary>
		///		Renders a rectangle. 
		///		Color, scale, rotation and offset all effect the rendering of a rectangle.
		/// </summary>
		/// <param name="x">Position on the x-axis to render rectangle.</param>
		/// <param name="y">Position on the y-axis to render rectangle.</param>
		/// <param name="z">Position on the z-axis to render rectangle.</param>
		/// <param name="width">Width of rectangle to render.</param>
		/// <param name="height">Height of rectangle to render.</param>
		/// <param name="Filled">If true the rectangle will be filled in, else it will be hollow.</param>
		public static void RenderRectangle(float x, float y, float z, float width, float height, bool filled)
		{
			_driver.RenderRectangle(x, y, z, width, height, filled);
		}
		public static void RenderRectangle(float x, float y, float z, float width, float height)
		{
			RenderRectangle(x, y, z, width, height, true);
		}

		/// <summary>
		///		Renders a rectangle in a dashed style.
		///		Color, scale, rotation and offset all effect the rendering of a dashed rectangle.
		/// </summary>
		/// <param name="x">Position on the x-axis to render rectangle.</param>
		/// <param name="y">Position on the y-axis to render rectangle.</param>
		/// <param name="z">Position on the z-axis to render rectangle.</param>
		/// <param name="width">Width of rectangle to render.</param>
		/// <param name="height">Height of rectangle to render.</param>
		/// <param name="dashGap">Size of the gap between each dash.</param>
		public static void RenderDashedRectangle(float x, float y, float z, float width, float height, int dashGap)
		{
			if (width < 0)
			{
				x += width;
				width = Math.Abs(width);
			}
			if (height < 0)
			{
				y += height;
				height = Math.Abs(height);
			}

			for (int i = 0; i < (int)(width / dashGap); i++)
			{
				if ((i % 2) == 1) continue;
				GraphicsManager.RenderLine(x + (i * dashGap), y, 0, x + (i * dashGap) + (i == (int)((width / dashGap) - 1) ? width % dashGap : dashGap), y, 0);
				GraphicsManager.RenderLine(x + (i * dashGap), y + height, 0, x + (i * dashGap) + (i == (int)((width / dashGap) - 1) ? width % dashGap : dashGap), y + height, 0);			
			}
			for (int i = 0; i < (int)(height / dashGap); i++)
			{
				if ((i % 2) == 1) continue;
				GraphicsManager.RenderLine(x, y + (i * dashGap), 0, x, y + (i * dashGap) + (i == (int)((height / dashGap) - 1) ? height % dashGap : dashGap), 0);
				GraphicsManager.RenderLine(x + width, y + (i * dashGap), 0, x + width, y + (i * dashGap) + (i == (int)((height / dashGap) - 1) ? height % dashGap : dashGap), 0);
			}
		}
		public static void RenderDashedRectangle(float x, float y, float z, float width, float height)
		{
			RenderDashedRectangle(x, y, z, width, height, 8);
		}

		/// <summary>
		///		Renders a oval. 
		///		Color, scale, rotation and offset all effect the rendering	of an oval.
		/// </summary>
		/// <param name="x">Position on the x-axis to render oval.</param>
		/// <param name="y">Position on the y-axis to render oval.</param>
		/// <param name="z">Position on the z-axis to render oval.</param>
		/// <param name="width">Width of oval to render.</param>
		/// <param name="height">Height of oval to render.</param>
		/// <param name="Filled">If true the oval will be filled in, else it will be hollow.</param>
		public static void RenderOval(float x, float y, float z, float width, float height, bool filled)
		{
			_driver.RenderOval(x, y, z, width, height, filled);
		}
		public static void RenderOval(float x, float y, float z, float width, float height)
		{
			RenderOval(x, y, z, width, height, true);
		}

		/// <summary>
		///		Renders a line.
		///		Color, scale, rotation and offset all effect the rendering	of a line.
		/// </summary>
		/// <param name="x">Position on the x-axis to start rendering line.</param>
		/// <param name="y">Position on the y-axis to start rendering line.</param>
		/// <param name="z">Position on the z-axis to start rendering line.</param>
		/// <param name="x2">Position on the x-axis to end rendering line.</param>
		/// <param name="y2">Position on the y-axis to end rendering line.</param>
		/// <param name="z2">Position on the z-axis to end rendering line.</param>
		public static void RenderLine(float x, float y, float z, float x2, float y2, float z2)
		{
			_driver.RenderLine(x, y, z, x2, y2, z2);
		}

		/// <summary>
		///		Renders a series of vertexs as a polygon.
		///		Color, scale, rotation and offset all effect the rendering	of a polygon.
		/// </summary>
		/// <param name="verts">Array of vertexs to render.</param>
		/// <param name="Filled">If true the polygon will be filled in, else it will be hollow.</param>
		public static void RenderPolygon(Vertex[] polyVerts, bool filled)
		{
			_driver.RenderPolygon(polyVerts,filled);
		}
		public static void RenderPolygon(Vertex[] polyVerts)
		{
			RenderPolygon(polyVerts, true);
		}

		/// <summary>
		///		Renders a single pixel to the screen.
		///		Color and offset effect the rendering of a pixel.
		/// </summary>
		/// <param name="x">Position on the x-axis to render pixel.</param>
		/// <param name="y">Position on the y-axis to render pixel.</param>
		/// <param name="z">Position on the z-axis to render pixel.</param>
		public static void RenderPixel(float x, float y, float z)
		{
            RenderRectangle(x, y, z, 1, 1, true);
		}

        /// <summary>
        ///     Grabs a backbuffer rectangle and places it on a pixelmap.
        /// </summary>
        /// <param name="x">X position of rectangle to grab.</param>
        /// <param name="y">Y position of rectangle to grab.</param>
        /// <param name="width">Width of rectangle to grab.</param>
        /// <param name="height">Height of rectangle to grab.</param>
        /// <returns>Pixelmap containing the given backbuffer rectangle.</returns>
        public static PixelMap GrabPixelmap(int x, int y, int width, int height)
        {
            return _driver.GrabPixelmap(x, y, width, height);
        }

		/// <summary>
		///		Creates a new valid image frame from a pixelmap.
		/// 
		///		This is mainly used by the Image class to recreate images whenever the driver changes 
		/// </summary>
		/// <param name="pixelMap">Pixelmap to create new image frame from.</param>
		public static IImageFrame CreateImageFrame(PixelMap pixelMap)
		{
			return _driver.CreateImageFrame(pixelMap);
		}
		public static IImageFrame CreateImageFrame()
		{
			return _driver.CreateImageFrame(null);
		}

        /// <summary>
        ///		Creates a new valid driver mesh from a vertex map.
        /// </summary>
        /// <param name="vertexMap">Vertexmap to create new mesh from.</param>
        public static IDriverMesh CreateDriverMesh(VertexMap vertexMap)
        {
            return _driver.CreateDriverMesh(vertexMap);
        }
        public static IDriverMesh CreateDriverMesh()
        {
            return CreateDriverMesh(null);
        }

		/// <summary>
		///		Pushs the current render state on to the stack.
		/// </summary>
        public static void PushRenderState()
		{
            //if (_renderStateStackDepth >= _renderStateStack.Length) throw new Exception("Render state stack overflow.");
            _renderStateStack[_renderStateStackDepth++].RetrieveFromCurrent();
        }
		
		/// <summary>
		///		Pops of the last render state and sets its as the current
		///		state.
		/// </summary>
		public static void PopRenderState()
		{
            _renderStateStack[--_renderStateStackDepth].SetAsCurrent();
        }

		/// <summary>
		///		Clears the current render state and nullifys all 
		///		render values.
		/// </summary>
		public static void ClearRenderState()
		{
			VertexColors.AllVertexs = unchecked((int)0xFFFFFFFF);
			ColorKey		= unchecked((int)0xFFFF00FF);
			ScaleFactor		= new float[3] { 1.0f, 1.0f, 1.0f };
			RotationAngle	= new float[3] { 0.0f, 0.0f, 0.0f };
			Offset			= new float[3] { 0.0f, 0.0f, 0.0f };
			BlendMode		= BlendMode.Alpha;
			Viewport		= new Rectangle(0, 0, Resolution[0], Resolution[1]);
			ClearColor		= unchecked((int)0x00000000);
            BitmapFont         = _defaultFont;
            DepthBufferEnabled = true;
            Shader = null;
		}

		#endregion
	}

	/// <summary>
	///		Stores details on the start of the rendering driver.
	/// </summary>
	public sealed class RenderState
	{
		#region Members
		#region Variables

		private QuadVertexColors _vertexColors = new QuadVertexColors();
		private int _colorKey;
		private float[] _scaleFactor = new float[3];
        private float[] _rotationAngle = new float[3];
		private float[] _offset = new float[3];
		private BlendMode _blendMode = BlendMode.Alpha;
		private Rectangle _viewport = new Rectangle(0,0,0,0);
		private int _clearColor;

		private int[] _resolution = new int[2];
		private float[] _resolutionOffset = new float[2];
		private float[] _resolutionScale = new float[2];

		private BitmapFont _bitmapFont;

        private bool _depthBufferEnabled = false;

        private Shader _shader = null;

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Sets this render state as the current render state.
		/// </summary>
		public void SetAsCurrent()
		{
            // Make sure these guys come first as they effect the others.
            GraphicsManager.Resolution = new int[2] { _resolution[0], _resolution[1] };
            GraphicsManager.ResolutionOffset = new float[2] { _resolutionOffset[0], _resolutionOffset[1] };
            GraphicsManager.ResolutionScale = new float[2] { _resolutionScale[0], _resolutionScale[1] };
            if (GraphicsManager.Viewport != _viewport) GraphicsManager.Viewport = _viewport;

            if (GraphicsManager.BlendMode != _blendMode) GraphicsManager.BlendMode = _blendMode;
            if (GraphicsManager.Shader != _shader) GraphicsManager.Shader = _shader;
          
            for (int i = 0; i < 4; i++)
                GraphicsManager.VertexColors[i] = _vertexColors[i];

            GraphicsManager.ColorKey = _colorKey;
           GraphicsManager.ScaleFactor = new float[3] { _scaleFactor[0], _scaleFactor[1], _scaleFactor[2] };
           GraphicsManager.RotationAngle = new float[3] { _rotationAngle[0], _rotationAngle[1], _rotationAngle[2] };
            GraphicsManager.Offset = new float[3] { _offset[0], _offset[1], _offset[2] };
            GraphicsManager.ClearColor = _clearColor;
            if (GraphicsManager.BitmapFont != _bitmapFont) GraphicsManager.BitmapFont = _bitmapFont;
            if (GraphicsManager.DepthBufferEnabled != _depthBufferEnabled) GraphicsManager.DepthBufferEnabled = _depthBufferEnabled;
        }

		/// <summary>
		///		Gets this render state's data from the current render state.
		/// </summary>
		public void RetrieveFromCurrent()
		{
            // Make sure these guys come first as they effect the others.
            _viewport = GraphicsManager.Viewport;
            _resolution[0] = GraphicsManager.Resolution[0];
            _resolution[1] = GraphicsManager.Resolution[1];
            _resolutionOffset[0] = GraphicsManager.ResolutionOffset[0];
            _resolutionOffset[1] = GraphicsManager.ResolutionOffset[1];
            _resolutionScale[0] = GraphicsManager.ResolutionScale[0];
            _resolutionScale[1] = GraphicsManager.ResolutionScale[1];

			for (int i = 0; i < 4; i++)
				_vertexColors[i] = GraphicsManager.VertexColors[i];
			_colorKey		= GraphicsManager.ColorKey;
			_scaleFactor[0]	= GraphicsManager.ScaleFactor[0];
			_scaleFactor[1] = GraphicsManager.ScaleFactor[1];
            _scaleFactor[2] = GraphicsManager.ScaleFactor[2];
            _rotationAngle[0] = GraphicsManager.RotationAngle[0];
            _rotationAngle[1] = GraphicsManager.RotationAngle[1];
            _rotationAngle[2] = GraphicsManager.RotationAngle[2];
			_offset[0]		= GraphicsManager.Offset[0];
			_offset[1]		= GraphicsManager.Offset[1];
			_offset[2]		= GraphicsManager.Offset[2];
			_blendMode		= GraphicsManager.BlendMode;
			_clearColor		= GraphicsManager.ClearColor;
			_bitmapFont = GraphicsManager.BitmapFont;
            _depthBufferEnabled = GraphicsManager.DepthBufferEnabled;

            _shader = GraphicsManager.Shader;
		}

		#endregion
	}

	#endregion
}