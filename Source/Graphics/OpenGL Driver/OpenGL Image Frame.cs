/* 
 * File: OpenGL Image.cs
 *
 * This source file contains the declaration of the OpenGLImage class, which is
 * used to store details on a renderable image.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Graphics.OpenGLDriver
{

    /// <summary>
    ///     The OpenGL Image is used to store details on a single image loaded
    ///     by the OpenGL driver.
    /// </summary>
    public sealed class OpenGLImageFrame : IImageFrame
    {
        #region Members
        #region Variables

        private OpenGLDriver _glDriver = null;

        private PixelMap _pixelMap;
        private int _textureWidth, _textureHeight;

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
            get { return _glDriver; }
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
        public OpenGLImageFrame(OpenGLDriver driver, PixelMap pixelMap)
        {
            _glDriver = driver;
            ConstructFromPixelMap(pixelMap);
        }
        public OpenGLImageFrame(OpenGLDriver driver)
        {
            _glDriver = driver;
        }

        /// <summary>
        ///		Disposes of this image frame, and frees all the resources it has 
        ///		allocated.
        /// </summary>
        ~OpenGLImageFrame()
        {
            GC.RemoveMemoryPressure(_memoryPressure);
            // Free image.
        }

        /// <summary>
        ///     Constructs a DirectX9 surface for this image from a PixelMap.
        /// </summary>
        /// <param name="pixelMap">PixelMap to construct surface from.</param>
        void ConstructFromPixelMap(PixelMap pixelMap)
        {
            _pixelMap = pixelMap;
        }

        /// <summary>
        ///     Renders this frame at the given position.
        /// </summary>
        /// <param name="x">Position on the x-axis to render this frame.</param>
        /// <param name="y">Position on the y-axis to render this frame.</param>
        /// <param name="z">Position on the z-axis to render this frame.</param>
        void IImageFrame.Render(float x, float y, float z)
        {

        }

        /// <summary>
        ///     Tiles this frame at the given position.
        /// </summary>
        /// <param name="x">Position on the x-axis to tile this frame.</param>
        /// <param name="y">Position on the y-axis to tile this frame.</param>
        /// <param name="z">Position on the z-axis to tile this frame.</param>
        void IImageFrame.Tile(float x, float y, float z)
        {
        }
        /// <summary>
        ///		This is called when the graphics driver wants you to render to this
        ///		target, it sets the rendering target to this image frame.
        /// </summary>
        /// <returns>True if the attachment was successfull, if not false, the graphics driver will throw an exception in this case.</returns>
        bool IRenderTarget.Attach()
        {
            return true;
        }

        /// <summary>
        ///		This is called when the graphics driver wants to finish rendering
        ///		to this target, and use another. This is used primarily for cleaning
        ///		up resources.
        /// </summary>
        void IRenderTarget.Detach()
        {
        }
        #endregion
    }

}