/* 
 * File: OpenGL Driver.cs
 *
 * This source file contains the declaration of the OpenGLDriver class, which adds
 * the capability of using OpenGL to render graphics.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Drawing;
using System.Threading;
using System.Collections;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Debug;

namespace BinaryPhoenix.Fusion.Graphics.OpenGLDriver
{

    /// <summary>
    ///     The OpenGLDriver allows the use of OpenGL when rendering the game.
    /// </summary>
    public sealed class OpenGLDriver : IGraphicsDriver
    {
        #region Members
        #region Variables

        private IRenderTarget _renderTarget;

        private QuadVertexColors _vertexColors = new QuadVertexColors();
        private int _colorKey = unchecked((int)0xFFFF00FF);
        private BlendMode _blend = BlendMode.Alpha;

        private float _rotationAngle;
        private float[] _scaleFactor = new float[2] { 1.0f, 1.0f };
        private Rectangle _viewport = new Rectangle(0, 0, 0, 0);
        private int _clearColor = unchecked((int)0x00000000);
        private float[] _offset = new float[3] { 0.0f, 0.0f, 0.0f };

        private int[] _resolution = new int[2];
        private float[] _resolutionScale = new float[2] { 1.0f, 1.0f };
        private float[] _resolutionOffset = new float[2] { 0.0f, 0.0f };

        private float _angleSin, _angleCos;

        private bool _zBufferEnabled = true;

        #endregion
        #region Properties

        /// <summary>
        ///		Gets a QuadVertexColors structure which contains the vertex 
        ///		colors used when rendering an image quad.
        /// </summary>
        QuadVertexColors IGraphicsDriver.VertexColors
        {
            get { return _vertexColors; }
        }

        /// <summary>
        ///		Gets or sets the default color-key used to mask images as they are loaded.
        /// </summary>
        int IGraphicsDriver.ColorKey
        {
            get { return _colorKey; }
            set { _colorKey = value; }
        }

        /// <summary>
        ///		Gets or sets the scaling factor to apply to any images drawn.
        /// </summary>
        float[] IGraphicsDriver.ScaleFactor
        {
            get { return _scaleFactor; }
            set
            {
                if (value.Length != 2) throw new Exception("An array of an invalid length was passed to the ScaleFactory property.");
                _scaleFactor = value;
                float sfx = _resolutionScale[0] + (Math.Abs(value[0]) - 1.0f);
                float sfy = _resolutionScale[1] + (Math.Abs(value[1]) - 1.0f);
            }
        }

        /// <summary>
        ///		Gets or sets the rotation angle to apply to any images drawn.
        /// </summary>
        float IGraphicsDriver.RotationAngle
        {
            get { return _rotationAngle; }
            set
            {
                _rotationAngle = value;
                _angleCos = (float)Math.Cos(MathMethods.DegreesToRadians(value));
                _angleSin = (float)Math.Sin(MathMethods.DegreesToRadians(value));
            }
        }

        /// <summary>
        ///		Gets or sets the offset applyed when rendering images.
        /// </summary>
        float[] IGraphicsDriver.Offset
        {
            get { return _offset; }
            set
            {
                if (value.Length != 3) throw new Exception("An array of an invalid length was passed to the Offset property.");
                _offset = value;
            }
        }

        /// <summary>
        ///		Gets or sets the blend mode used to blend colors together.
        /// </summary>
        BlendMode IGraphicsDriver.BlendMode
        {
            get { return _blend; }
            set
            {
                // Set here
                _blend = value;
            }
        }

        /// <summary>
        ///		Gets or sets the viewport (clipping rectangle) used to clip vertexs outside its bounds. 
        /// </summary>
        Rectangle IGraphicsDriver.Viewport
        {
            get { return _viewport; }
            set
            {
                // Thats a bad user, bad user! *Beats with stick*
                if (value.X < 0)
                {
                    value.Width += value.X;
                    value.X = 0;
                }
                if (value.Y < 0)
                {
                    value.Height += value.Y;
                    value.Y = 0;
                }
                if (value.Bottom > _resolution[1])
                    value.Height -= (value.Bottom - _resolution[1]);
                if (value.Right > _resolution[0])
                    value.Width -= (value.Right - _resolution[0]);

                value.Width = Math.Max(0, value.Width);
                value.Height = Math.Max(0, value.Height);

                _viewport = value;
                OpenGL.glViewport((int)Math.Ceiling((_viewport.X * _resolutionScale[0]) + _resolutionOffset[0]),
                                         (int)Math.Ceiling((_viewport.Y * _resolutionScale[1]) + _resolutionOffset[1]),
                                         (int)Math.Round(_viewport.Width * _resolutionScale[0], 0, MidpointRounding.ToEven),
                                         (int)Math.Round(_viewport.Height * _resolutionScale[1], 0, MidpointRounding.ToEven));
            }
        }

        /// <summary>
        ///		Gets or sets the color used to blank the screen when ClearBuffer is called.
        /// </summary>
        int IGraphicsDriver.ClearColor
        {
            get { return _clearColor; }
            set { _clearColor = value; }
        }

        /// <summary>
        ///		Sets the target this graphics driver should render to, this is normally an
        ///		image or window.
        /// </summary>
        IRenderTarget IGraphicsDriver.RenderTarget
        {
            get { return _renderTarget; }
            set { SetRenderTarget(value); }
        }

        /// <summary>
        ///     Returns the sign of the rotation angle.
        /// </summary>
        public float RotationAngleSin
        {
            get { return _angleSin; }
        }

        /// <summary>
        ///     Returns the cosine of the rotation angle.
        /// </summary>
        public float RotationAngleCos
        {
            get { return _angleCos; }
        }

        /// <summary>
        ///		Sets or gets the current emulated resolution.
        /// </summary>
        int[] IGraphicsDriver.Resolution
        {
            get { return _resolution; }
            set { _resolution = value; }
        }

        /// <summary>
        ///		Sets or gets the current emulated resolution scale.
        /// </summary>
        float[] IGraphicsDriver.ResolutionScale
        {
            get { return _resolutionScale; }
            set { _resolutionScale = value; }
        }

        /// <summary>
        ///		Sets or gets the current emulated resolution offset.
        /// </summary>
        float[] IGraphicsDriver.ResolutionOffset
        {
            get { return _resolutionOffset; }
            set { _resolutionOffset = value; }
        }

        /// <summary>
        ///     Gets or sets if the ZBuffer is enabled or not.
        /// </summary>
        bool IGraphicsDriver.DepthBufferEnabled
        {
            get { return _zBufferEnabled; }
            set
            {
                // Enable / Disable here
                _zBufferEnabled = value;
            }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///		Attachs the given render target to this driver and detachs the old one.
        /// </summary>
        /// <param name="target">Render target to attach.</param>
        public void SetRenderTarget(IRenderTarget target)
        {
            if (target == _renderTarget || target == null) return;
            if (_renderTarget != null) _renderTarget.Detach();

            ResetState(target);
            _renderTarget = target;
            if (target.Attach() != true)
                throw new Exception("An error occured while attempting to attach the graphics driver to a render target.");
        }

        /// <summary>
        ///		Creates a new canvas capable of being rendered to by this driver..
        /// </summary>
        /// <param name="control">Control this canvas should render to.</param>
        /// <param name="flags">Flags describing how canvas should be created.</param>
        /// <returns>New canvas being capable of being rendered to.</returns>
        IRenderTarget IGraphicsDriver.CreateCanvas(Control control, GraphicsFlags flags)
        {
            return new OpenGLCanvas();
        }

        /// <summary>
        ///     Sets up the DirectX9 devices used for rendering.
        /// </summary>
        /// <param name="renderControl">Control that default swap chain should render to.</param>
        /// <param name="width">Width of control to render on, this is used to create the correct backbuffer size.</param>
        /// <param name="height">Height of control to render on, this is used to create the correct backbuffer size.</param>
        /// <param name="flags">A bitmask of GraphicsFlags, specifying how the device should be setup.</param>
        /// <returns>True if device was setup successfull, else false.</returns>
        public bool InitializeDevice(Control renderControl, int width, int height, GraphicsFlags flags)
        {
            return true;
        }

        /// <summary>
        ///		Resets the rendering state of the DX9Device to default 2D.
        /// </summary>
        private void ResetState(IRenderTarget target)
        {

        }

        /// <summary>
        ///     ClearBuffer will clear the current rendering target to the color
        ///     previously specificed in ClearColor.
        /// </summary>
        void IGraphicsDriver.ClearScene()
        {

        }

        /// <summary>
        ///     Clears the depth buffer.
        /// </summary>
        void IGraphicsDriver.ClearDepthBuffer()
        {

        }

        /// <summary>
        ///		This will setup the drawing buffers so that they are ready for you to render on 
        ///		them this must be called before you can render anything/
        /// </summary>
        void IGraphicsDriver.BeginScene()
        {

        }

        /// <summary>
        ///		Partner of BeginScene, must be called after rendering scene.
        /// </summary>
        void IGraphicsDriver.FinishScene()
        {

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
        void IGraphicsDriver.RenderImage(Image image, float x, float y, float z, int frame)
        {
            image.Render(x, y, z, frame);
        }

        /// <summary>
        ///     Tiles a given image at the given position.
        ///		Color, scale, rotation and offset all effect the renderin	of an image.
        /// </summary>
        /// <param name="image">Image to render.</param>
        /// <param name="x">Position on the x-axis to render image.</param>
        /// <param name="y">Position on the y-axis to render image.</param>
        /// <param name="z">Position on the z-axis to render image.</param>
        /// <param name="frame">Frame of image to render.</param>
        void IGraphicsDriver.TileImage(Image image, float x, float y, float z, int frame)
        {
            image.Tile(x, y, z, frame);
        }

        /// <summary>
        ///		Renders a rectangle. 
        ///		Color, scale, rotation and offset all effect the rendering	of a rectangle.
        /// </summary>
        /// <param name="x">Position on the x-axis to render rectangle.</param>
        /// <param name="y">Position on the y-axis to render rectangle.</param>
        /// <param name="z">Position on the z-axis to render rectangle.</param>
        /// <param name="width">Width of rectangle to render.</param>
        /// <param name="height">Height of rectangle to render.</param>
        /// <param name="Filled">If true the rectangle will be filled in, else it will be hollow.</param>
        void IGraphicsDriver.RenderRectangle(float x, float y, float z, float width, float height, bool filled)
        {
 
        }

        /// <summary>
        ///		Renders an oval. 
        ///		Color, scale, rotation and offset all effect the rendering	of an oval.
        /// </summary>
        /// <param name="x">Position on the x-axis to render oval.</param>
        /// <param name="y">Position on the y-axis to render oval.</param>
        /// <param name="z">Position on the z-axis to render oval.</param>
        /// <param name="width">Width of oval to render.</param>
        /// <param name="height">Height of oval to render.</param>
        /// <param name="Filled">If true the oval will be filled in, else it will be hollow.</param>
        void IGraphicsDriver.RenderOval(float x, float y, float z, float width, float height, bool filled)
        {
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
        void IGraphicsDriver.RenderLine(float x, float y, float z, float x2, float y2, float z2)
        {
        }

        /// <summary>
        ///		Renders a series of vertexs as a polygon.
        ///		Color, scale, rotation and offset all effect the rendering	of a polygon.
        /// </summary>
        /// <param name="verts">Array of vertexs to render.</param>
        /// <param name="Filled">If true the polygon will be filled in, else it will be hollow.</param>
        void IGraphicsDriver.RenderPolygon(Vertex[] polyVerts, bool filled)
        {
        }

        /// <summary>
        ///		Renders a single pixel to the screen.
        ///		Color and offset effect the rendering of a pixel.
        /// </summary>
        /// <param name="x">Position on the x-axis to render pixel.</param>
        /// <param name="y">Position on the y-axis to render pixel.</param>
        /// <param name="z">Position on the z-axis to render pixel.</param>
        void IGraphicsDriver.RenderPixel(float x, float y, float z)
        {
        }

        /// <summary>
        ///		Creates a new valid image frame from a pixelmap.
        /// 
        ///		This is mainly used by the Image class to recreate images whenever the driver changes 
        /// </summary>
        /// <param name="pixelMap">Pixelmap to create new image frame from.</param>
        IImageFrame IGraphicsDriver.CreateImageFrame(PixelMap pixelMap)
        {
            return pixelMap == null ? new OpenGLImageFrame(this) : new OpenGLImageFrame(this, pixelMap);
        }

        /// <summary>
        ///		Sets the emulated resolution of the current rendering context.
        /// </summary>
        /// <param name="width">Width to set resolution to.</param>
        /// <param name="height">Height to set resolution to.</param>
        /// <param name="keepAspectRatio">If set to true the aspect ratio of the resolution will be kept.</param>
        void IGraphicsDriver.SetResolution(int width, int height, bool keepAspectRatio)
        {
            _resolution[0] = width;
            _resolution[1] = height;
            if (keepAspectRatio == true)
            {
                float aspectWidth = width, aspectHeight = height;
                if (aspectWidth > _renderTarget.Width || aspectHeight > _renderTarget.Height)
                    while (aspectWidth > _renderTarget.Width && aspectHeight > _renderTarget.Height)
                    {
                        aspectWidth /= 1.5f;
                        aspectHeight /= 1.5f;
                    }
                else
                    while (aspectWidth + (width / 2) < _renderTarget.Width && aspectHeight + (height / 2) < _renderTarget.Height)
                    {
                        aspectWidth *= 1.5f;
                        aspectHeight *= 1.5f;
                    }

                _resolutionOffset[0] = (int)((_renderTarget.Width - aspectWidth) / 2);
                _resolutionOffset[1] = (int)((_renderTarget.Height - aspectHeight) / 2);
                _resolutionScale[0] = (aspectWidth / width);
                _resolutionScale[1] = (aspectHeight / height);
            }
            else
            {
                _resolutionOffset[0] = 0;
                _resolutionOffset[1] = 0;
                _resolutionScale[0] = (float)_renderTarget.Width / width;
                _resolutionScale[1] = (float)_renderTarget.Height / height;
            }
            _viewport = new Rectangle(0, 0, width, height);
            ((IGraphicsDriver)this).ScaleFactor = new float[2] { 1.0f, 1.0f };
        }

        #endregion
    }

}