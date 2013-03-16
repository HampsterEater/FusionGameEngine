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
using Microsoft.DirectX.Direct3D;

namespace BinaryPhoenix.Fusion.Graphics.Direct3D9Driver
{

	/// <summary>
	///     The DirectX9Window class is used by DirectX9Class to create a 
	///     window capable of rendering on.
	/// </summary>
	public sealed class Direct3D9Canvas : IRenderTarget
	{
		#region Members
		#region Variables

		private Direct3D9Driver	  _dx9Driver;
		private SwapChain		  _swapChain;
		private PresentParameters _presentParameters = new PresentParameters();

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
		public Direct3D9Canvas(Direct3D9Driver driver, Control control, GraphicsFlags flags)
		{
			// Store the driver for future use
			_control = control;
			_dx9Driver = driver;
			_flags = flags;

			// Setup windows components and show it.
			_control.Resize += new System.EventHandler(ResizeEvent);
            _control.Disposed += new EventHandler(Disposed);
		}
		public Direct3D9Canvas() { }

        /// <summary>
        ///		Called when the user disposes this control.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        void Disposed(object sender, EventArgs e)
        {
            if (_swapChain == null) return;
            if (_dx9Driver.PresentParameters.DeviceWindow == _control)
                _dx9Driver.PresentParameters.DeviceWindow = null;

            if (_dx9Driver.SwapChain == _swapChain)
            {
                _dx9Driver.SwapChain = null;
                _dx9Driver.DX9Device.SetRenderTarget(0, _dx9Driver.DX9Device.GetBackBuffer(0, 0, BackBufferType.Mono));
            }  
            //_swapChain.Dispose();
        }

		/// <summary>
		///		Called when the user resizes this control. This forces the control
		///		to recreate its swap chain.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		public void ResizeEvent(object sender, System.EventArgs e)
		{
			// If this is the device window we need to reset the device.
            if (_dx9Driver != null && _dx9Driver.PresentParameters.DeviceWindow == _control)
                _dx9Driver.InitializeDevice(_control, _control.ClientSize.Width, _control.ClientSize.Height, _flags);
            else
                CreateSwapChain();
        }

		/// <summary>
		///		This is called when the graphics driver wants you to render to this
		///		target, it sets the rendering target to this control.
		/// </summary>
		/// <returns>True if the attachment was successfull, if not false, the graphics driver will throw an exception in this case.</returns>
		bool IRenderTarget.Attach()
		{
			// Create swap chain if it hasen't already been setup
			if ((_swapChain == null || _swapChain.Disposed == true) && _dx9Driver.DX9Device.PresentationParameters.DeviceWindow != _control)
				CreateSwapChain();

			// Set the render target to this window's swap chain.
			Surface surface = _swapChain == null ? _dx9Driver.DX9Device.GetBackBuffer(0, 0, BackBufferType.Mono) : _swapChain.GetBackBuffer(0, BackBufferType.Mono);
			try
			{
				_dx9Driver.DX9Device.SetRenderTarget(0, surface);
			}
			catch (Exception)
			{
				CreateSwapChain();
			}

			// Set the drivers swap chain so it knows which to present.
			_dx9Driver.SwapChain = _swapChain;

            // If we have a previous render state then set the current state to it.
            GraphicsManager.PushRenderState();
            GraphicsManager.Viewport = new Rectangle(0, 0, _control.ClientSize.Width, _control.ClientSize.Height);
            GraphicsManager.SetResolution(_control.ClientSize.Width, _control.ClientSize.Height, false);

			return true;
		}

		/// <summary>
		///		Creates a new swap chain to use when rendering to this control.
		/// </summary>
		private void CreateSwapChain()
		{
            if (_control.ClientSize.Width <= 0 || _control.ClientSize.Height <= 0)
                return;

			// If this is the default swap chain of the DX9Device then 
			// reset the DX9Device instead of creating one.
			if (_dx9Driver.DX9Device.PresentationParameters.DeviceWindow == _control)
			{
				_swapChain = null; 
				return;
			}

            bool isPreviousSwapChain = (_dx9Driver.SwapChain == _swapChain);

            if (_swapChain != null)
                _swapChain.Dispose();

            _presentParameters = Direct3D9Driver.CreatePresentParameters(_control, _control.ClientSize.Width, _control.ClientSize.Height, _flags);
			_swapChain = new SwapChain(_dx9Driver.DX9Device, _presentParameters);

            if (isPreviousSwapChain == true)
            {
                _dx9Driver.SwapChain = _swapChain;
                _dx9Driver.DX9Device.SetRenderTarget(0, _swapChain.GetBackBuffer(0, BackBufferType.Mono));
            }   
        }

		/// <summary>
		///		This is called when the graphics driver wants to finish rendering
		///		to this target, and use another. This is used primarily for cleaning
		///		up resources.
		/// </summary>
		void IRenderTarget.Detach()
		{
			// Pop of the render state of this canvas.
			GraphicsManager.PopRenderState();

			// Nullify the current swap chain.
			_dx9Driver.SwapChain = null;
		}

		#endregion
	}

}