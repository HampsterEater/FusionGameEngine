/* 
 * File: DirectX9 Canvas.cs
 *
 * This source file contains the declaration of the DirectX9Canvas class, which
 * is used by the DirectX9Driver class as a control to render on.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace BinaryPhoenix.Fusion.Graphics.OpenGLDriver
{

    /// <summary>
    ///     The OpenGLCanvas class is used by OpenGLDriver Class to create a 
    ///     control capable of rendering on.
    /// </summary>
    public sealed class OpenGLCanvas : IRenderTarget
    {
        #region Members
        #region Variables

        private OpenGLDriver _glDriver;

        private GraphicsFlags _flags = 0;

        private Control _control = null;

        #endregion
        #region Properties

        /// <summary>
        ///		Returns the width of this render target.
        /// </summary>
        int IRenderTarget.Width
        {
            get { return _control.ClientSize.Width; }
        }

        /// <summary>
        ///		Returns the height of this render target.
        /// </summary>
        int IRenderTarget.Height
        {
            get { return _control.ClientSize.Height; }
        }

        /// <summary>
        ///		Returns the flags used to create of this render target.
        /// </summary>
        GraphicsFlags IRenderTarget.Flags
        {
            get { return _flags; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Setups up a new DirectX9Canvas instance with the given specifications.
        /// </summary>
        /// <param name="driver">DirectX9 driver to associated with this canvas.</param>
        /// <param name="control">Control to render this canvases graphics to.</param>
        /// <param name="flags">BitMask of GraphicsFlags flags, specifies how the canvas is setup.</param>
        public OpenGLCanvas(OpenGLDriver driver, Control control, GraphicsFlags flags)
        {
            // Store the driver for future use
            _control = control;
            _glDriver = driver;
            _flags = flags;

            // Setup windows components and show it.
            _control.Resize += new System.EventHandler(ResizeEvent);
            _control.Disposed += new EventHandler(Disposed);
        }
        public OpenGLCanvas() { }

        /// <summary>
        ///		Called when the user disposes this control.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        void Disposed(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///		Called when the user resizes this control. This forces the control
        ///		to recreate its swap chain.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        public void ResizeEvent(object sender, System.EventArgs e)
        {
        }

        /// <summary>
        ///		This is called when the graphics driver wants you to render to this
        ///		target, it sets the rendering target to this control.
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