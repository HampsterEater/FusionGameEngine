/* 
 * File: DirectX9 Image.cs
 *
 * This source file contains the declaration of the DirectX9Image class, which is
 * used to store details on a 
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Graphics.Direct3D9Driver
{

    /// <summary>
    ///     The DirectX9 Image is used to store details on a single image loaded
    ///     by the DirectX9 driver.
    /// </summary>
    public sealed class Direct3D9ImageFrame : IImageFrame
	{
		#region Members
		#region Variables

		private CustomVertex.PositionColoredTextured[] _vertexArray = new CustomVertex.PositionColoredTextured[4] 
			{
				new CustomVertex.PositionColoredTextured(0, 0, 0.0f, unchecked((int)0xFFFFFFFF), 0, 0),
				new CustomVertex.PositionColoredTextured(0, 0, 0.0f, unchecked((int)0xFFFFFFFF), 1, 0),
				new CustomVertex.PositionColoredTextured(0, 0, 0.0f, unchecked((int)0xFFFFFFFF), 1, 1),
				new CustomVertex.PositionColoredTextured(0, 0, 0.0f, unchecked((int)0xFFFFFFFF), 0, 1)
			};
        
        private PixelMap _pixelMap;
        private Texture _texture;
		private Texture _renderTarget;
        private bool _isRenderTarget = false;
        private int _textureWidth, _textureHeight;

		private Direct3D9Driver _dx9Driver;

		private RenderState _previousRenderState = null;

        private int _memoryPressure = 0;

        private Point _origin = new Point(0, 0);

		#endregion
		#region Properties

        /// <summary>
        ///     Sets the origin of all this images frames. The origin is 
        ///     the offset from the top-left corner used when rendering this image.
        /// </summary>
        Point IImageFrame.Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }

		/// <summary>
		///		Gets or sets the DirectX9 texture used by this frame.
		/// </summary>
		public Texture Texture
		{
			get { return _texture;  }
			set { _texture = value; }
		}

		/// <summary>
		///		Gets or sets the pixelmap used to create this image frame.
		///		Setting it will cause any old data to be lost, and the image recreated.
		/// </summary>
		PixelMap IImageFrame.PixelMap
		{
			get { return _pixelMap; }
			set { ConstructFromPixelMap(value); }
		}

		/// <summary>
		///		Gets the graphics driver this image frame is associated with.
		/// </summary>
		IGraphicsDriver IImageFrame.Driver
		{
			get { return _dx9Driver; }
		}

		/// <summary>
		///		Returns the width of this render target.
		/// </summary>
		int IRenderTarget.Width
		{
			get { return _pixelMap.Width; }
		}

		/// <summary>
		///		Returns the height of this render target.
		/// </summary>
		int IRenderTarget.Height
		{
			get { return _pixelMap.Height; }
		}

		/// <summary>
		///		Returns the flags used to create of this render target.
		/// </summary>
		GraphicsFlags IRenderTarget.Flags
		{
			get { return 0; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Creates a new image frame out of a PixelMap and associates it with a specific image.
		/// </summary>
		/// <param name="driver">Direct3D9Driver that this image frame should be associated with.</param>
		/// <param name="pixelMap">PixelMap to create frame from.</param>
        public Direct3D9ImageFrame(Direct3D9Driver driver, PixelMap pixelMap)
        {
			_dx9Driver = driver;
            ConstructFromPixelMap(pixelMap);
        }
		public Direct3D9ImageFrame(Direct3D9Driver driver)
		{
			_dx9Driver = driver;
		}

		/// <summary>
		///		Disposes of this image frame, and frees all the resources it has 
		///		allocated.
		/// </summary>
		~Direct3D9ImageFrame()
		{
            GC.RemoveMemoryPressure(_memoryPressure);
			if (_texture != null)      _texture.Dispose();
			if (_renderTarget != null) _renderTarget.Dispose();
		}

        /// <summary>
        ///     Simple helper function, takes a integer and finds the closest power-of-2 integer
        ///     from it.
        /// </summary>
        /// <param name="n">Value to find closest power-of-2 from.</param>
        /// <returns>Closest power-of-2.</returns>
        private int Pow(int n,int p)
        {
            int i = 1;
            while (i < n)
            {
                i *= p;
            }
            return i;
        }

        /// <summary>
        ///     Constructs a DirectX9 surface for this image from a PixelMap.
        /// </summary>
        /// <param name="pixelMap">PixelMap to construct surface from.</param>
        void ConstructFromPixelMap(PixelMap pixelMap)
        {
			 _pixelMap = pixelMap;
            
            // Work out size of texture to create so that we have a nice square texture.
             if (_dx9Driver.DX9Device.DeviceCaps.TextureCaps.SupportsSquareOnly == true || _dx9Driver.DX9Device.DeviceCaps.TextureCaps.SupportsPower2 == true)
            {
                 if (pixelMap.Width > pixelMap.Height)
                 {
                     _textureWidth = MathMethods.Pow(pixelMap.Width, 2);
                     _textureHeight = _textureWidth;
                 }
                 else
                 {
                     _textureHeight = MathMethods.Pow(pixelMap.Height, 2);
                     _textureWidth = _textureHeight;
                 }
            }
            else
            {
                _textureWidth = pixelMap.Width;
                _textureHeight = pixelMap.Height;
            }

			// Create texture.
			_texture = new Texture(_dx9Driver.DX9Device, _textureWidth, _textureHeight, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
		
			// Copy the pixelMap and resize it.
			PixelMap copy = new PixelMap(_textureWidth, _textureHeight);
			copy.Fill(0x00000000);
			copy.Paste(pixelMap, 0, 0);

            // Add memory pressure so the GC collects faster.
            _memoryPressure = copy.Data.Length;
            GC.AddMemoryPressure(_memoryPressure);

			// Create a buffer and then write the image data into it.
			int pitch;
			GraphicsStream stream = _texture.LockRectangle(0, 0, out pitch);
			stream.Write(copy.Data); 
			_texture.UnlockRectangle(0);

			// Sets up the vertex-buffer so all our data is correct
			_vertexArray[0].X = 0;
			_vertexArray[0].Y = 0;
			_vertexArray[1].X = _textureWidth;
			_vertexArray[1].Y = 0;
			_vertexArray[2].X = _textureWidth;
			_vertexArray[2].Y = _textureHeight;
			_vertexArray[3].X = 0;
			_vertexArray[3].Y = _textureHeight;

            // Hook into the dx9 disposal event so we can clean up.
            _dx9Driver.DX9Device.Disposing += new EventHandler(DX9Device_Disposing);
		}

        /// <summary>
        ///     Invoked when the DX9 device that created this image is destroyed.
        /// </summary>
        /// <param name="sender">Object that invoked this function.</param>
        /// <param name="e">Arguments explaining why this was invoked.</param>
        private void DX9Device_Disposing(object sender, EventArgs e)
        {
            if (_texture != null && _texture.Disposed == false)
                _texture.Dispose();
        }

        /// <summary>
        ///     Renders this frame at the given position.
        /// </summary>
        /// <param name="x">Position on the x-axis to render this frame.</param>
        /// <param name="y">Position on the y-axis to render this frame.</param>
        /// <param name="z">Position on the z-axis to render this frame.</param>
        void IImageFrame.Render(float x, float y, float z)
        {
            // Set the world matrix to transform our image.
            IGraphicsDriver graphicsDriver = (IGraphicsDriver)_dx9Driver;

            // Add origin to position.
            x += _origin.X;
            y += _origin.Y;

            // Work out where we should draw.
            float sx = graphicsDriver.ResolutionScale[0] + (Math.Abs(graphicsDriver.ScaleFactor[0]) - 1.0f);
            float sy = graphicsDriver.ResolutionScale[1] + (Math.Abs(graphicsDriver.ScaleFactor[1]) - 1.0f);
            float x0 = -_origin.X;
            float y0 = -_origin.Y; 
            float x1 = x0 + _pixelMap.Width;
            float y1 = y0 + _pixelMap.Height;
            float tx = ((x + graphicsDriver.Offset[0]) * graphicsDriver.ResolutionScale[0]) + graphicsDriver.ResolutionOffset[0];
            float ty = ((y + graphicsDriver.Offset[1]) * graphicsDriver.ResolutionScale[0]) + graphicsDriver.ResolutionOffset[1];
            float ix = (float)_dx9Driver.RotationAngleCos * sx;
            float iy = -(float)_dx9Driver.RotationAngleSin * sy;
            float jx = (float)_dx9Driver.RotationAngleSin * sx;
            float jy = (float)_dx9Driver.RotationAngleCos * sy;
            float z0 = z + graphicsDriver.Offset[2];
            float tu = (float)_pixelMap.Width / (float)_textureWidth;
            float tv = (float)_pixelMap.Height / (float)_textureHeight;

            _vertexArray[0].Color = graphicsDriver.VertexColors[0];
            _vertexArray[0].X = x0 * ix + y0 * iy + tx;
            _vertexArray[0].Y = x0 * jx + y0 * jy + ty;
            _vertexArray[0].Z = z0;
            _vertexArray[0].Tu = graphicsDriver.ScaleFactor[0] >= 0.0f ? 0 : tu;
            _vertexArray[0].Tv = graphicsDriver.ScaleFactor[1] >= 0.0f ? 0 : tv;

            _vertexArray[1].Color = graphicsDriver.VertexColors[1];
            _vertexArray[1].X = x1 * ix + y0 * iy + tx;
            _vertexArray[1].Y = x1 * jx + y0 * jy + ty;
            _vertexArray[1].Z = z0;
            _vertexArray[1].Tu = graphicsDriver.ScaleFactor[0] >= 0.0f ? tu : 0;
            _vertexArray[1].Tv = graphicsDriver.ScaleFactor[1] >= 0.0f ? 0 : tv;

            _vertexArray[2].Color = graphicsDriver.VertexColors[2];
            _vertexArray[2].X = x1 * ix + y1 * iy + tx;
            _vertexArray[2].Y = x1 * jx + y1 * jy + ty;
            _vertexArray[2].Z = z0;
            _vertexArray[2].Tu = graphicsDriver.ScaleFactor[0] >= 0.0f ? tu : 0;
            _vertexArray[2].Tv = graphicsDriver.ScaleFactor[1] >= 0.0f ? tv : 0;

            _vertexArray[3].Color = graphicsDriver.VertexColors[3];
            _vertexArray[3].X = x0 * ix + y1 * iy + tx;
            _vertexArray[3].Y = x0 * jx + y1 * jy + ty;
            _vertexArray[3].Z = z0;
            _vertexArray[3].Tu = graphicsDriver.ScaleFactor[0] >= 0.0f ? 0 : tu;
            _vertexArray[3].Tv = graphicsDriver.ScaleFactor[1] >= 0.0f ? tv : 0;

            // Write out vertexs into the buffer.
            _dx9Driver.UploadTextureAndBuffer(_texture, null);

            if (graphicsDriver.Shader != null)
            {
                int passes = graphicsDriver.Shader.Begin();
                for (int i = 0; i < passes; i++)
                {
                    graphicsDriver.Shader.BeginPass(i);
                    _dx9Driver.DX9Device.DrawUserPrimitives(PrimitiveType.TriangleFan, 2, _vertexArray);
                    graphicsDriver.Shader.FinishPass();
                }
                graphicsDriver.Shader.Finish();
            }
            else
                _dx9Driver.DX9Device.DrawUserPrimitives(PrimitiveType.TriangleFan, 2, _vertexArray);
        }

        /// <summary>
        ///     Tiles this frame at the given position.
        /// </summary>
        /// <param name="x">Position on the x-axis to tile this frame.</param>
        /// <param name="y">Position on the y-axis to tile this frame.</param>
        /// <param name="z">Position on the z-axis to tile this frame.</param>
        void IImageFrame.Tile(float x, float y, float z)
        {
            IGraphicsDriver graphicsDriver = (IGraphicsDriver)_dx9Driver;

            // Can we do it as a single quad?
            bool singleQuad = false;
            if (_dx9Driver.DX9Device.DeviceCaps.TextureCaps.SupportsSquareOnly == true || _dx9Driver.DX9Device.DeviceCaps.TextureCaps.SupportsPower2 == true)
                singleQuad = (_textureWidth == _pixelMap.Width && _textureHeight == _pixelMap.Height);    
            else
                singleQuad = true;

            // Yippe!
            if (singleQuad == true)
            {
                // Work out the scale.
                float rsx = GraphicsManager.Driver.ResolutionScale[0];
                float rsy = GraphicsManager.Driver.ResolutionScale[1];
                float sx = Math.Abs(GraphicsManager.Driver.ResolutionScale[0] + (Math.Abs(GraphicsManager.Driver.ScaleFactor[0]) - 1.0f));
                float sy = Math.Abs(GraphicsManager.Driver.ResolutionScale[1] + (Math.Abs(GraphicsManager.Driver.ScaleFactor[1]) - 1.0f));

                float ox = (((graphicsDriver.Viewport.X - x) % (_pixelMap.Width * sx)) * rsx) + (_pixelMap.Width * sx);
                float oy = (((graphicsDriver.Viewport.Y - y) % (_pixelMap.Height * sy)) * rsy) + (_pixelMap.Height * sy);

                float qx = ((graphicsDriver.Viewport.X * rsx) + graphicsDriver.ResolutionOffset[0]) - ox;
                float qy = ((graphicsDriver.Viewport.Y * rsx) + graphicsDriver.ResolutionOffset[1]) - oy;
                float qw = (graphicsDriver.Viewport.Width * rsx) + ox;
                float qh = (graphicsDriver.Viewport.Height * rsy) + oy;

                // If we are flipping the image we need to compensate.
                if (graphicsDriver.ScaleFactor[0] < 0.0f && qx + qw > graphicsDriver.Resolution[0])
                {
                    int hCount = Math.Max(1, (int)Math.Round(((float)graphicsDriver.Viewport.Width * rsx) / ((float)_pixelMap.Width * sx))) + 1;
                    qw = (hCount * (_pixelMap.Width * sx)) + ox;
                }
                if (graphicsDriver.ScaleFactor[1] < 0.0f && qy + qh > graphicsDriver.Resolution[1])
                {
                    int vCount = Math.Max(1, (int)Math.Round(((float)graphicsDriver.Viewport.Height * rsy) / ((float)_pixelMap.Height * sy))) + 1;
                    qh = (vCount * (_pixelMap.Height * sy)) + oy;
                }

                float tw = qw / (_pixelMap.Width * sx);
                float th = qh / (_pixelMap.Height * sy);

                // Work out vertex positions.
                _vertexArray[0].X = qx;
                _vertexArray[0].Y = qy;
                _vertexArray[0].Z = z;
                _vertexArray[0].Tu = GraphicsManager.Driver.ScaleFactor[0] >= 0 ? 0 : tw;
                _vertexArray[0].Tv = GraphicsManager.Driver.ScaleFactor[1] >= 0 ? 0 : th;
                _vertexArray[0].Color = graphicsDriver.VertexColors[0];

                _vertexArray[1].X = qx + qw;
                _vertexArray[1].Y = qy;
                _vertexArray[1].Z = z;
                _vertexArray[1].Tu = GraphicsManager.Driver.ScaleFactor[0] >= 0 ? tw : 0;
                _vertexArray[1].Tv = GraphicsManager.Driver.ScaleFactor[1] >= 0 ? 0 : th;
                _vertexArray[1].Color = graphicsDriver.VertexColors[1];

                _vertexArray[2].X = qx + qw;
                _vertexArray[2].Y = qy + qh;
                _vertexArray[2].Z = z;
                _vertexArray[2].Tu = GraphicsManager.Driver.ScaleFactor[0] >= 0 ? tw : 0;
                _vertexArray[2].Tv = GraphicsManager.Driver.ScaleFactor[1] >= 0 ? th : 0;
                _vertexArray[2].Color = graphicsDriver.VertexColors[2];

                _vertexArray[3].X = qx;
                _vertexArray[3].Y = qy + qh;
                _vertexArray[3].Z = z;
                _vertexArray[3].Tu = GraphicsManager.Driver.ScaleFactor[0] >= 0 ? 0 : tw;
                _vertexArray[3].Tv = GraphicsManager.Driver.ScaleFactor[1] >= 0 ? th : 0;
                _vertexArray[3].Color = graphicsDriver.VertexColors[3];

                // Render the image using a 2 part triangle fan.
                _dx9Driver.UploadTextureAndBuffer(_texture, null);
                if (graphicsDriver.Shader != null)
                {
                    int passes = graphicsDriver.Shader.Begin();
                    for (int i = 0; i < passes; i++)
                    {
                        graphicsDriver.Shader.BeginPass(i);
                        _dx9Driver.DX9Device.DrawUserPrimitives(PrimitiveType.TriangleFan, 2, _vertexArray);
                        graphicsDriver.Shader.FinishPass();
                    }
                    graphicsDriver.Shader.Finish();
                }
                else
                    _dx9Driver.DX9Device.DrawUserPrimitives(PrimitiveType.TriangleFan, 2, _vertexArray);

                // Reset vertex texture positions.
                _vertexArray[0].Tu = 0;
                _vertexArray[0].Tv = 0;

                _vertexArray[1].Tu = 1;
                _vertexArray[1].Tv = 0;

                _vertexArray[2].Tu = 1;
                _vertexArray[2].Tv = 1;

                _vertexArray[3].Tu = 0;
                _vertexArray[3].Tv = 1;
            }

            // Nuts, we will have to do them seperately.
            else
            {
                float sx = Math.Abs(graphicsDriver.ScaleFactor[0]);
                float sy = Math.Abs(graphicsDriver.ScaleFactor[1]);

                float ox = graphicsDriver.Viewport.X - ((graphicsDriver.Viewport.X - x) % (_pixelMap.Width * sx));
                float oy = graphicsDriver.Viewport.Y - ((graphicsDriver.Viewport.Y - y) % (_pixelMap.Height * sy));

                int hCount = (int)Math.Ceiling(graphicsDriver.Viewport.Width / (_pixelMap.Width * sx)) + 1;
                int vCount = (int)Math.Ceiling(graphicsDriver.Viewport.Height / (_pixelMap.Height * sy)) + 1;

                for (int xr = 0; xr < hCount; xr++)
                    for (int yr = 0; yr < vCount; yr++)
                        ((IImageFrame)this).Render(ox + (xr * (_pixelMap.Width * sx)), oy + (yr * (_pixelMap.Height * sy)), z);
            }
        }
		/// <summary>
		///		This is called when the graphics driver wants you to render to this
		///		target, it sets the rendering target to this image frame.
		/// </summary>
		/// <returns>True if the attachment was successfull, if not false, the graphics driver will throw an exception in this case.</returns>
		bool IRenderTarget.Attach()
		{
			// Create our render target.t.
            if (_isRenderTarget == false || _texture == null || _texture.Disposed == true)
            {
                // Dispose of original texture.
                if (_texture != null)
                {
                    _texture.Dispose();
                    _texture = null;
                    GC.Collect();
                }

                // Create a new one.
                _renderTarget = new Texture(_dx9Driver.DX9Device, _textureWidth, _textureHeight, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
                if (_renderTarget == null) return false;

                // Set it as the new texture.
                _texture = _renderTarget;
                _isRenderTarget = true;
            }

			// Set the current target to our newly created one, and return success!
			_dx9Driver.DX9Device.SetRenderTarget(0, _texture.GetSurfaceLevel(0));

			// If we have a previous render state then set the current state to it.
			if (_previousRenderState != null) 
				_previousRenderState.SetAsCurrent();
			else
				GraphicsManager.SetResolution(((IRenderTarget)this).Width, ((IRenderTarget)this).Height, false);

			return true;
		}

		/// <summary>
		///		This is called when the graphics driver wants to finish rendering
		///		to this target, and use another. This is used primarily for cleaning
		///		up resources.
		/// </summary>
		void IRenderTarget.Detach()
		{
			// Save the current render state for later, incase we switch back to this render target.
			_previousRenderState = new RenderState();
		}
		#endregion
    }

}