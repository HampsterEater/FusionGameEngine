/* 
 * File: DirectX9 Driver.cs
 *
 * This source file contains the declaration of the DirectX9Driver class, which adds
 * the capability of using DirectX9 to render graphics.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Reflection;
using System.Drawing;
using System.Threading;
using System.Collections;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Debug;

namespace BinaryPhoenix.Fusion.Graphics.Direct3D9Driver 
{

    /// <summary>
    ///     The DirectX9Driver allows the use of DirectX9 when rendering the game.
    /// </summary>
	public sealed class Direct3D9Driver : IGraphicsDriver
	{
		#region Members
		#region Variables

        private RenderMode _renderMode = RenderMode.Dimensions2;

        private Shader _currentShader = null;
        private IRenderTarget  _renderTarget;

        private Device _dx9Device;
        private PresentParameters _presentParameters;

		private QuadVertexColors _vertexColors = new QuadVertexColors();
		private int _colorKey = unchecked((int)0xFFFF00FF);
		private BlendMode _blend = BlendMode.Alpha;

		private float _rotationAngle;
		private float[] _scaleFactor = new float[2] { 1.0f, 1.0f };
		private Rectangle _viewport = new Rectangle(0,0,0,0);
		private int _clearColor = unchecked((int)0x00000000);
		private float[] _offset = new float[3] { 0.0f, 0.0f, 0.0f }; 

		private SwapChain _swapChain;

		private Texture _uploadedTexture;
		private VertexBuffer _uploadedVertexBuffer;

		private int[] _resolution = new int[2];
		private float[] _resolutionScale = new float[2] { 1.0f, 1.0f };
		private float[] _resolutionOffset = new float[2] { 0.0f, 0.0f };

        private float _angleSin, _angleCos;

        private bool _zBufferEnabled = true;

        private Hashtable _shaderValues = new Hashtable();
        private bool _shaderValuesChanged = false;

		#endregion
		#region Properties

		/// <summary>
		///		Gets the current DirectX9 rendering device.
		/// </summary>
		public Device DX9Device
        {
            get { return _dx9Device; }
        }

		/// <summary>
		///		Gets the currently uploaded texture.
		/// </summary>
		public Texture UploadedTexture
		{
			get { return _uploadedTexture;  }
		}

		/// <summary>
		///		Gets the currently uploaded vertex buffer
		/// </summary>
		public VertexBuffer UploadedVertexBuffer
		{
			get { return _uploadedVertexBuffer; }
		}

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
			get { return _colorKey;  }
			set { _colorKey = value; }
		}

        /// <summary>
        ///		Gets or sets the shader currently being applied to rendered objects.
        /// </summary>
        Shader IGraphicsDriver.Shader
        {
            get { return _currentShader; }
            set { _currentShader = value; }
        }

        /// <summary>
        ///		Gets or sets the list of values used by shaders.
        /// </summary>
        public Hashtable ShaderValues
        {
            get { return _shaderValues; }
            set { _shaderValues = value; }
        }

        /// <summary>
        ///		Gets or sets the if the variables have been changed or not.
        /// </summary>
        public bool ShaderValuesChanged
        {
            get { return _shaderValuesChanged; }
            set { _shaderValuesChanged = value; }
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
			get { return _blend;  }
			set 
			{ 
				switch (value) {
					case BlendMode.Solid:
						_dx9Device.RenderState.AlphaTestEnable  = false;
						_dx9Device.RenderState.AlphaBlendEnable = false;
						break;
					case BlendMode.Mask:
						_dx9Device.RenderState.AlphaTestEnable	= true;
						_dx9Device.RenderState.AlphaBlendEnable = false;
						break;
					case BlendMode.Alpha:
						_dx9Device.RenderState.AlphaTestEnable	= true;
						_dx9Device.RenderState.AlphaBlendEnable = true;
						_dx9Device.RenderState.SourceBlend		= Blend.SourceAlpha;
						_dx9Device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
						break;
					case BlendMode.Lighten:
						_dx9Device.RenderState.AlphaTestEnable	= true;
						_dx9Device.RenderState.AlphaBlendEnable = true;
						_dx9Device.RenderState.SourceBlend		= Blend.SourceAlpha;
						_dx9Device.RenderState.DestinationBlend = Blend.One;
						break;
					case BlendMode.Darken:
                        _dx9Device.RenderState.AlphaTestEnable = true;
						_dx9Device.RenderState.AlphaBlendEnable = true;
						_dx9Device.RenderState.SourceBlend		= Blend.Zero;
						_dx9Device.RenderState.DestinationBlend = Blend.SourceColor;
						break;
					case BlendMode.Invert:
                        _dx9Device.RenderState.AlphaTestEnable = true;
						_dx9Device.RenderState.AlphaBlendEnable = true;
						_dx9Device.RenderState.SourceBlend		= Blend.InvDestinationColor;
						_dx9Device.RenderState.DestinationBlend = Blend.InvSourceColor;
						break;
					case BlendMode.Add:
                        _dx9Device.RenderState.AlphaTestEnable = true;
						_dx9Device.RenderState.AlphaBlendEnable = true;
						_dx9Device.RenderState.SourceBlend		= Blend.One;
						_dx9Device.RenderState.DestinationBlend = Blend.One;
						break;
					case BlendMode.Subtract:
                        _dx9Device.RenderState.AlphaTestEnable = true;
						_dx9Device.RenderState.AlphaBlendEnable = true;
						_dx9Device.RenderState.SourceBlend		= Blend.InvSourceColor;
						_dx9Device.RenderState.DestinationBlend = Blend.InvDestinationColor;
						break;
				}
				_blend = value;
			}
		}

        /// <summary>
        ///     Gets or sets the currentrender mode.
        /// </summary>
        RenderMode IGraphicsDriver.RenderMode
        {
            get { return _renderMode; }
            set { _renderMode = value; ResetState(_renderTarget); }
        }

		/// <summary>
		///		Gets or sets the viewport (clipping rectangle) used to clip vertexs outside its bounds. 
		/// </summary>
		Rectangle IGraphicsDriver.Viewport 
		{
			get { return _viewport;  }
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

                _dx9Device.ScissorRectangle = new Rectangle((int)Math.Ceiling((_viewport.X * _resolutionScale[0]) + _resolutionOffset[0]),
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
			get { return _clearColor;  }
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
		///		Sets or Gets the swap chain to render to, if this is set to null 
		///		the default swap chain will be rendered to.
		/// </summary>
		public SwapChain SwapChain
		{
			get { return _swapChain;  }
			set { _swapChain = value; }
		}

		/// <summary>
		///		Sets or Gets the presentation parameters used by this drivers device.
		/// </summary>
		public PresentParameters PresentParameters
		{
			get { return _presentParameters; }
			set { _presentParameters = value; }
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
                _dx9Device.RenderState.ZBufferFunction = (value == false ? Compare.Always : Compare.LessEqual);
                _zBufferEnabled = value;
            }
        }

		#endregion
		#endregion
		#region Methods

        /// <summary>
        ///     Returns the name of this drivers API.
        /// </summary>
        /// <returns>Name of this drivers API.</returns>
        public override string ToString()
        {
            return "Direct3D9";
        }

        /// <summary>
        ///     Sets a variable contained in all shaders used by this graphics driver.
        /// </summary>
        /// <param name="variable">Name of variable to set.</param>
        /// <param name="value">Value to set variable to.</param>
        void IGraphicsDriver.SetShaderVariable(string variable, object value)
        {
            if (_shaderValues.ContainsKey(variable))
                _shaderValues[variable] = value;
            else
            {
                _shaderValues.Add(variable, value);
                _shaderValuesChanged = true;
            }
        }

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
        ///     Creates a new driver dependent shader.
        /// </summary>
        /// <param name="code">Shader code to compile.</param>
        IShader IGraphicsDriver.CreateShader(string code)
        {
            return new Direct3D9Shader(this, code);
        }

		/// <summary>
		///		Creates a new canvas capable of being rendered to by this driver..
		/// </summary>
		/// <param name="control">Control this canvas should render to.</param>
		/// <param name="flags">Flags describing how canvas should be created.</param>
		/// <returns>New canvas being capable of being rendered to.</returns>
		IRenderTarget IGraphicsDriver.CreateCanvas(Control control, GraphicsFlags flags)
		{
			// Create a new canvas capable o being rendered to.
			Direct3D9Canvas canvas = new Direct3D9Canvas(this, control, flags);

			// Gets the rendering device and attach to the current window
			if ((_dx9Device == null || _dx9Device.Disposed == true) && InitializeDevice(control, ((IRenderTarget)canvas).Width, ((IRenderTarget)canvas).Height, flags) == false) 
                throw new Exception("An error occured while attempting to initialize the Direct3D9 device.");

			// Set the render target to this canvas.
			SetRenderTarget(canvas);

			return canvas;
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
			if (width == 0 || height == 0) return false;

            CreateFlags createFlags;
            Caps caps;

            // Catch any odd errors DirectX may give off.
            try
            {
                // Get device capabilitys
                caps = Manager.GetDeviceCaps(0, DeviceType.Hardware);

                // Try and get the fastest game experiance possible
                createFlags = (caps.DeviceCaps.SupportsHardwareTransformAndLight == true) ? CreateFlags.HardwareVertexProcessing : CreateFlags.SoftwareVertexProcessing;
                if (caps.DeviceCaps.SupportsPureDevice == true) createFlags |= CreateFlags.PureDevice;

                // Decide on window setup parameters
                _presentParameters = CreatePresentParameters(renderControl, width, height, flags);

				// Create device if it dosent exist, else reset it
				if (_dx9Device == null)
				{
					// Last but not least create out new DirectX9 device with all the settings
					// we just worked out.
					_dx9Device = new Device(0, DeviceType.Hardware, renderControl, createFlags, _presentParameters);

					// Add lost and resizing handlers
					_dx9Device.DeviceLost     += new System.EventHandler(DeviceLost);
					_dx9Device.DeviceReset    += new System.EventHandler(DeviceReset);
					_dx9Device.DeviceResizing += new System.ComponentModel.CancelEventHandler(DeviceResizing);
				}
				else
				{
					int result;
					_dx9Device.CheckCooperativeLevel(out result);
                    int count = 0;
					while (true)
					{
						if (result == (int)ResultCode.DeviceLost)
							Thread.Sleep(500);
						else if (result == (int)ResultCode.DeviceNotReset)
							break;
						else
							break;
                        if (count++ >= 10) break;
					}

					// Reset the device.
                    DebugLogger.WriteLog("Attempting to reset Direct3D9 device...");
					_dx9Device.Reset(_presentParameters);
				}
			}
            catch (DirectXException)
            {
                _dx9Device = null;
                return false;
            }

            return true; 
        }

        /// <summary>
        ///     Handy macro function that works out the appropriate present parameters for the given values.
        /// </summary>
        /// <param name="renderControl">Control to render to.</param>
        /// <param name="width">Width of rendering target.</param>
        /// <param name="height">Height of rendering target.</param>
        /// <param name="flags">Flags specifying how to create parameters.</param>
        /// <returns>New present parameters created using the values given.</returns>
        public static PresentParameters CreatePresentParameters(Control renderControl, int width, int height, GraphicsFlags flags)
        {
            PresentParameters presentParameters = new PresentParameters();
			presentParameters.DeviceWindow = renderControl;
            presentParameters.Windowed = (flags & GraphicsFlags.FullScreen) == 0;
            presentParameters.AutoDepthStencilFormat = DepthFormat.D16;
			presentParameters.EnableAutoDepthStencil = true;

            // Speeds it up a bit in debug mode at the expense of a few graphical effects.
//#if DEBUG
            presentParameters.SwapEffect		    = SwapEffect.Discard;
			presentParameters.PresentationInterval = PresentInterval.Immediate;
//#else
//                _presentParameters.SwapEffect		    = SwapEffect.Flip;
//				_presentParameters.PresentationInterval	= PresentInterval.One;
//#endif
			
			// If we are in fullscreen the setup the screen to the current resolution
            if ((flags & GraphicsFlags.FullScreen) != 0)
            {
                presentParameters.BackBufferWidth				= width;
                presentParameters.BackBufferHeight				= height;
				presentParameters.BackBufferFormat				= Manager.Adapters[0].CurrentDisplayMode.Format;
                presentParameters.BackBufferCount				= 1;
                presentParameters.FullScreenRefreshRateInHz	    = Manager.Adapters[0].CurrentDisplayMode.RefreshRate;
			}

            return presentParameters;
        }

        /// <summary>
        ///     Used as an event handler for the DirectX9 device DeviceLost event, called
        ///     when the DirectX9 devies changes to a lost state.
        /// </summary>
        /// <param name="sender">Object which sent this event.</param>
        /// <param name="e">Event arguments.</param>
        private void DeviceLost(object sender, System.EventArgs e)
        {
            // Clean up rendering stuff a bit.
			_dx9Device.SetTexture(0, null);
			_dx9Device.SetStreamSource(0, null, 0);
			_uploadedTexture = null;
			_uploadedVertexBuffer = null;

            DebugLogger.WriteLog("DirectX9 device lost.");
        }

        /// <summary>
        ///     Used as an event handler for the DirectX9 device DeviceReset event, called
        ///     after the device is reset to allow time for resources to be recreated.
        /// </summary>
        /// <param name="sender">Object which sent this event.</param>
        /// <param name="e">Event arguments.</param>
        private void DeviceReset(object sender, System.EventArgs e)
        {
			ResetState(_renderTarget);
            DebugLogger.WriteLog("DirectX9 device reset.");
        }

		/// <summary>
		///		Resets the rendering state of the DX9Device to default 2D.
		/// </summary>
		private void ResetState(IRenderTarget target)
		{
            if (_renderMode == RenderMode.Dimensions3)
            {
                // Setup default rendering state.
                _dx9Device.RenderState.AlphaTestEnable = true;
                _dx9Device.RenderState.ReferenceAlpha = 0;
                _dx9Device.RenderState.AlphaFunction = Compare.Greater;
                _dx9Device.RenderState.CullMode = Cull.Clockwise;
                _dx9Device.RenderState.ShadeMode = ShadeMode.Gouraud;
                _dx9Device.RenderState.Lighting = false;
                _dx9Device.RenderState.ScissorTestEnable = true;
                _dx9Device.RenderState.ZBufferEnable = true;
                _dx9Device.RenderState.ZBufferFunction = Compare.LessEqual;
                _zBufferEnabled = true;

                // Setup default tecture stages
                _dx9Device.SetTextureStageState(0, TextureStageStates.AlphaOperation, (int)TextureOperation.Modulate);
                _dx9Device.SetTextureStageState(0, TextureStageStates.AlphaArgument1, (int)TextureArgument.TextureColor);
                _dx9Device.SetTextureStageState(0, TextureStageStates.ColorOperation, (int)TextureOperation.Modulate);
                _dx9Device.SetTextureStageState(0, TextureStageStates.ColorArgument1, (int)TextureArgument.TextureColor);
                _dx9Device.SetTextureStageState(0, TextureStageStates.ColorArgument2, (int)TextureArgument.Diffuse);
                _dx9Device.SamplerState[0].MinFilter = TextureFilter.Point;
                _dx9Device.SamplerState[0].MagFilter = TextureFilter.Point;
                _dx9Device.SamplerState[0].MipFilter = TextureFilter.Point;

                // Setup vertexs and key colors.
                _dx9Device.VertexFormat = CustomVertex.PositionColoredTextured.Format;

                // Setup projection and view matrix's
                _dx9Device.Transform.Projection = Matrix.PerspectiveLH(target.Width, target.Height, -1000.0f, 1000.0f);
                _dx9Device.Transform.World = Matrix.Identity;
                _dx9Device.Transform.View = Matrix.Identity;

                ((IGraphicsDriver)this).BlendMode = BlendMode.Alpha;
            }
            else
            {
                // Setup default rendering state.
                _dx9Device.RenderState.AlphaTestEnable = true;
                _dx9Device.RenderState.ReferenceAlpha = 0;
                _dx9Device.RenderState.AlphaFunction = Compare.Greater;
                _dx9Device.RenderState.CullMode = Cull.None;
                _dx9Device.RenderState.ShadeMode = ShadeMode.Flat;
                _dx9Device.RenderState.Lighting = false;
                _dx9Device.RenderState.ScissorTestEnable = true;
                _dx9Device.RenderState.ZBufferEnable = true;
                _dx9Device.RenderState.ZBufferFunction = Compare.LessEqual;
                _zBufferEnabled = true;

                // Setup default tecture stages
                _dx9Device.SetTextureStageState(0, TextureStageStates.AlphaOperation, (int)TextureOperation.Modulate);
                _dx9Device.SetTextureStageState(0, TextureStageStates.AlphaArgument1, (int)TextureArgument.TextureColor);
                _dx9Device.SetTextureStageState(0, TextureStageStates.ColorOperation, (int)TextureOperation.Modulate);
                _dx9Device.SetTextureStageState(0, TextureStageStates.ColorArgument1, (int)TextureArgument.TextureColor);
                _dx9Device.SetTextureStageState(0, TextureStageStates.ColorArgument2, (int)TextureArgument.Diffuse);
                _dx9Device.SamplerState[0].MinFilter = TextureFilter.Point;
                _dx9Device.SamplerState[0].MagFilter = TextureFilter.Point;
                _dx9Device.SamplerState[0].MipFilter = TextureFilter.Point;

                // Setup vertexs and key colors.
                _dx9Device.VertexFormat = CustomVertex.PositionColoredTextured.Format;

                // Setup projection and view matrix's
                float width = target.Width;
                float height = target.Height;
                float zFar = 10000.0f;
                float zNear = -10000.0f;

                Matrix projectionMatrix = new Matrix();
                projectionMatrix.M11 = 2.0f / width;
                projectionMatrix.M12 = 0.0f;
                projectionMatrix.M13 = 0.0f;
                projectionMatrix.M14 = 0.0f;

                projectionMatrix.M21 = 0.0f;
                projectionMatrix.M22 = -2.0f / height;
                projectionMatrix.M23 = 0.0f;
                projectionMatrix.M24 = 0.0f;

                projectionMatrix.M31 = 0.0f;
                projectionMatrix.M32 = 0.0f;
                projectionMatrix.M33 = 1.0f / (zFar - zNear);
                projectionMatrix.M34 = 0.0f;

                projectionMatrix.M41 = -1 - (1.0f / width);
                projectionMatrix.M42 = 1 + (1.0f / height);
                projectionMatrix.M43 = -zNear / (zFar - zNear);
                projectionMatrix.M44 = 1.0f;

                _dx9Device.Transform.Projection = projectionMatrix;
                _dx9Device.Transform.World = Matrix.Identity;
                _dx9Device.Transform.View = Matrix.Identity;

                ((IGraphicsDriver)this).BlendMode = BlendMode.Alpha;
            }
		}

        /// <summary>
        ///     Used as an event handler for the DirectX9 device DeviceResizing event, normally
        ///     called due to minimizing or maximizing the window.
        /// </summary>
        /// <param name="sender">Object which sent this event.</param>
        /// <param name="e">Event arguments.</param>
        private void DeviceResizing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           // e.Cancel = true;
        }

        /// <summary>
        ///     Recovers the DirectX9 device when it is in the DeviceLost state.
        /// </summary>
        private void RecoverDevice()
        {
            int result;
            _dx9Device.CheckCooperativeLevel(out result);
            DebugLogger.WriteLog("Attempting to recover DirectX9 device.");
			while (true)
			{
				if (result == (int)ResultCode.DeviceLost)
					Thread.Sleep(500);
				else if (result == (int)ResultCode.DeviceNotReset)
					try
					{
                        DebugLogger.WriteLog("Attempting to reset DirectX9 device.");
						_dx9Device.Reset(_presentParameters);
						break;
					}
					catch (DeviceLostException) { }
			}
            DebugLogger.WriteLog("Recovered DirectX9 device.");
		}

        /// <summary>
        ///     ClearBuffer will clear the current rendering target to the color
        ///     previously specificed in ClearColor.
        /// </summary>
		void IGraphicsDriver.ClearScene()
		{
            _dx9Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, _clearColor, 1.0f, 0);
		}

        /// <summary>
        ///     Clears the depth buffer.
        /// </summary>
        void IGraphicsDriver.ClearDepthBuffer()
        {
           _dx9Device.Clear(ClearFlags.ZBuffer, _clearColor, 1.0f, 0);
        }

        /// <summary>
        ///		This will setup the drawing buffers so that they are ready for you to render on 
		///		them this must be called before you can render anything/
        /// </summary>
		void IGraphicsDriver.BeginScene()
        {
            // Begin rendering a new scene
            _dx9Device.BeginScene();

			// Clear the WHOLE (including any border caused by emulating a resolution) window.
			_dx9Device.ScissorRectangle = new System.Drawing.Rectangle(0, 0, _renderTarget.Width, _renderTarget.Height);
            _dx9Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, _clearColor, 1.0f, 0);

			// Clear out texture and vertex buffer.
			_dx9Device.SetTexture(0, null);
			_dx9Device.SetStreamSource(0, null, 0);
		}

		/// <summary>
		///		Partner of BeginScene, must be called after rendering scene.
		/// </summary>
		void IGraphicsDriver.FinishScene()
		{
			// End rendering scene
			_uploadedTexture      = null;
			_uploadedVertexBuffer = null;
			_dx9Device.EndScene();
			
			// Check for device loss.
			try
			{
                if (_swapChain != null && _swapChain.Disposed == false && _swapChain.PresentParameters.DeviceWindow.Disposing == false)
                {
                    _swapChain.Present();
                }
                else if (_dx9Device.Disposed == false)
                {
                    _dx9Device.Present();
                    _shaderValuesChanged = false;
                }
			}
			catch (DeviceLostException)
			{
				RecoverDevice();
			}
		}

		/// <summary>
		///		Uploads a texture and buffer to the driver, this function mainly
		///		just checks that it's not uploading an already uploaded texture/buffer.
		/// </summary>
		/// <param name="texture">Texture to upload.</param>
		/// <param name="vertexBuffer">VertexBuffer to upload.</param>
		public void UploadTextureAndBuffer(Texture texture, VertexBuffer vertexBuffer)
		{
			if (_uploadedTexture != texture || _uploadedVertexBuffer != vertexBuffer)
			{
				_dx9Device.SetTexture(0, null);
				if (texture != null) _dx9Device.SetTexture(0, texture);

				_dx9Device.SetStreamSource(0, null, 0);
				if (vertexBuffer != null) _dx9Device.SetStreamSource(0, vertexBuffer, 0);
				
                _uploadedTexture = texture;
				_uploadedVertexBuffer = vertexBuffer;
			}
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
            // Work out where we should draw.
            float sx = _resolutionScale[0] + (Math.Abs(_scaleFactor[0]) - 1.0f);
            float sy = _resolutionScale[1] + (Math.Abs(_scaleFactor[1]) - 1.0f);
            float x0 = 0;
            float y0 = 0; 
            float x1 = x0 + width;
            float y1 = y0 + height;
            float tx = ((x + _offset[0]) * _resolutionScale[0]) + _resolutionOffset[0];
            float ty = ((y + _offset[1]) * _resolutionScale[1]) + _resolutionOffset[1];
            float ix = (float)_angleCos * sx;
            float iy = -(float)_angleSin * sy;
            float jx = (float)_angleSin * sx;
            float jy = (float)_angleCos * sy;
            float z0 = z + _offset[2];

			// Create an array of vertexs.
            CustomVertex.PositionColoredTextured[] verts = new CustomVertex.PositionColoredTextured[5]
                {
                    new CustomVertex.PositionColoredTextured
                    (
                        x0 * ix + y0 * iy + tx,
                        x0 * jx + y0 * jy + ty,
                        z0, _vertexColors[0], 0,0
                    ),
                    new CustomVertex.PositionColoredTextured
                    (
                        x1 * ix + y0 * iy + tx,
                        x1 * jx + y0 * jy + ty,
                        z0, _vertexColors[1], 0,0
                    ),
                    new CustomVertex.PositionColoredTextured
                    (
                        x1 * ix + y1 * iy + tx,
                        x1 * jx + y1 * jy + ty,
                        z0, _vertexColors[2], 0,0
                    ),
                    new CustomVertex.PositionColoredTextured
                    (
                        x0 * ix + y1 * iy + tx,
                        x0 * jx + y1 * jy + ty,
                        z0, _vertexColors[3], 0,0
                    ),
                    new CustomVertex.PositionColoredTextured
                    (
                        x0 * ix + y0 * iy + tx,
                        x0 * jx + y0 * jy + ty,
                        z0, _vertexColors[0], 0,0
                    ),
                };

			// Render
            UploadTextureAndBuffer(null, null);

            if (_currentShader != null)
            {
                int passes = _currentShader.Begin();
                for (int i = 0; i < passes; i++)
                {
                    _currentShader.BeginPass(i);
			        _dx9Device.DrawUserPrimitives((filled ? PrimitiveType.TriangleFan : PrimitiveType.LineStrip), (filled ? 2 : 4), verts);
                    _currentShader.FinishPass();
                }
                _currentShader.Finish();
            }
            else
                _dx9Device.DrawUserPrimitives((filled ? PrimitiveType.TriangleFan : PrimitiveType.LineStrip), (filled ? 2 : 4), verts);
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
            // Work out where we should draw.
            float sx = _resolutionScale[0] + (Math.Abs(_scaleFactor[0]) - 1.0f);
            float sy = _resolutionScale[1] + (Math.Abs(_scaleFactor[1]) - 1.0f);
            float x0 = 0;
            float y0 = 0;
            float x1 = x0 + width;
            float y1 = y0 + height;
            float tx = ((x + _offset[0]) * sx) + _resolutionOffset[0];
            float ty = ((y + _offset[1]) * sy) + _resolutionOffset[1];
            float ix = (float)_angleCos * sx;
            float iy = -(float)_angleSin * sy;
            float jx = (float)_angleSin * sx;
            float jy = (float)_angleCos * sy;
            float z0 = z + _offset[2];

            float xr = (x1 - x0) * 0.5f;
            float yr = (y1 - y0) * 0.5f;
            int segs = (int)(Math.Abs(xr) + Math.Abs(yr));
            segs = (int)Math.Max(segs, 12) &~ 3;
            x0 += xr;
            y0 += yr;

            CustomVertex.PositionColoredTextured[] verts = new CustomVertex.PositionColoredTextured[segs + (filled == true ? 0 : 1)];
            for (int i = 0; i < segs; i++)
            {
                float th = -i * 360.0f / segs;
                float vx = x0 + (float)Math.Cos(MathMethods.DegreesToRadians(th)) * xr;
                float vy = y0 - (float)Math.Sin(MathMethods.DegreesToRadians(th)) * yr;
                verts[i].X = vx * ix + vy * iy + tx;
                verts[i].Y = vx * jx + vy * jy + ty; 
                verts[i].Color = _vertexColors[0];
            }
            if (filled == false)
            {
                verts[verts.Length - 1].X = verts[0].X;
                verts[verts.Length - 1].Y = verts[0].Y;
                verts[verts.Length - 1].Color = verts[0].Color;;
            }

            // Render!
            UploadTextureAndBuffer(null, null);
            if (_currentShader != null)
            {
                int passes = _currentShader.Begin();
                for (int i = 0; i < passes; i++)
                {
                    _currentShader.BeginPass(i);
			        _dx9Device.DrawUserPrimitives((filled ? PrimitiveType.TriangleFan : PrimitiveType.LineStrip), filled ? segs - 2 : segs, verts);
                    _currentShader.FinishPass();
                }
                _currentShader.Finish();
            }
            else
                _dx9Device.DrawUserPrimitives((filled ? PrimitiveType.TriangleFan : PrimitiveType.LineStrip), filled ? segs - 2 : segs, verts);            
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
			if (_scaleFactor[0] < 0)
			{
				float diffX = x2 - x;
				x += diffX;
				x2 -= diffX;
			}
			if (_scaleFactor[1] < 0)
			{
				float diffY = y2 - y;
				y += diffY;
				y2 -= diffY;
			}

            // Upload empty texture and buffer.
            UploadTextureAndBuffer(null, null);

            // Work out where to render.
            float renderX = _resolutionOffset[0] + ((x + _offset[0]) * _resolutionScale[0]);
            float renderY = _resolutionOffset[1] + ((y + _offset[1]) * _resolutionScale[1]);
            float renderZ = z + _offset[2];
            float renderX2 = _resolutionOffset[0] + ((x2 + _offset[0]) * _resolutionScale[0]);
            float renderY2 = _resolutionOffset[1] + ((y2 + _offset[1]) * _resolutionScale[1]);
            float renderZ2 = z2 + _offset[2];
           
			// Create an array of vertexs.
			CustomVertex.PositionColoredTextured[] verts = new CustomVertex.PositionColoredTextured[2] 
			{
				new CustomVertex.PositionColoredTextured(renderX, renderY, renderZ, _vertexColors[0], 0, 0),
				new CustomVertex.PositionColoredTextured(renderX2, renderY2, renderZ2, _vertexColors[1], 1, 1),
            };

            // Render
            if (_currentShader != null)
            {
                int passes = _currentShader.Begin();
                for (int i = 0; i < passes; i++)
                {
                    _currentShader.BeginPass(i);
			        _dx9Device.DrawUserPrimitives(PrimitiveType.LineStrip, 1, verts);
                    _currentShader.FinishPass();
                }
                _currentShader.Finish();
            }
            else
                _dx9Device.DrawUserPrimitives(PrimitiveType.LineStrip, 1, verts);
		}

		/// <summary>
		///		Renders a series of vertexs as a polygon.
		///		Color, scale, rotation and offset all effect the rendering	of a polygon.
		/// </summary>
		/// <param name="verts">Array of vertexs to render.</param>
		/// <param name="Filled">If true the polygon will be filled in, else it will be hollow.</param>
		void IGraphicsDriver.RenderPolygon(Vertex[] polyVerts, bool filled)
		{
			// Create an array of vertexs.
			CustomVertex.PositionColoredTextured[] verts = new CustomVertex.PositionColoredTextured[filled ? polyVerts.Length : polyVerts.Length + 1];
			float x, y;
			for (int i = 0; i < polyVerts.Length; i++)
			{
				x = _resolutionOffset[0] + ((polyVerts[i].X + _offset[0]) * _resolutionScale[0]);
				y = _resolutionOffset[1] + ((polyVerts[i].Y + _offset[1]) * _resolutionScale[1]);
				verts[i] = new CustomVertex.PositionColoredTextured(x, y, polyVerts[i].Z, _vertexColors[0], 0, 0);
			}
			if (filled == false)
			{
				x = _resolutionOffset[0] + ((polyVerts[0].X + _offset[0]) * _resolutionScale[0]);
				y = _resolutionOffset[1] + ((polyVerts[0].Y + _offset[1]) * _resolutionScale[1]);
				verts[polyVerts.Length] = new CustomVertex.PositionColoredTextured(x, y, polyVerts[0].Z, _vertexColors[0], 0, 0);
			}

			// Alert: SLOW SLOW SLOW!, This can be speed up if the vertexs are precompiled to a buffer.
			// Set transform and render
			UploadTextureAndBuffer(null, null);

			// This could probably be done more elegently, but it works.
            if (_currentShader != null)
            {
                int passes = _currentShader.Begin();
                for (int i = 0; i < passes; i++)
                {
                    _currentShader.BeginPass(i);
			        _dx9Device.DrawUserPrimitives(filled ? PrimitiveType.TriangleFan : PrimitiveType.LineStrip, filled ? ((polyVerts.Length / 3) + 1) : polyVerts.Length, verts);
                    _currentShader.FinishPass();
                }
                _currentShader.Finish();
            }
            else
                _dx9Device.DrawUserPrimitives(filled ? PrimitiveType.TriangleFan : PrimitiveType.LineStrip, filled ? ((polyVerts.Length / 3) + 1) : polyVerts.Length, verts);                
        }

		/// <summary>
		///		Renders a single pixel to the screen.
		///		Color and offset effect the rendering of a pixel.
		/// </summary>
		/// <param name="x">Position on the x-axis to render pixel.</param>
		/// <param name="y">Position on the y-axis to render pixel.</param>
		/// <param name="z">Position on the z-axis to render pixel.</param>
		void IGraphicsDriver.RenderPixel(float x, float y,float z)
		{
			// Upload null buffer.
			UploadTextureAndBuffer(null, null);

            // Work out where to render.
			float renderX = _resolutionOffset[0] + ((x + _offset[0]) * _resolutionScale[0]);
			float renderY = _resolutionOffset[1] + ((y + _offset[1]) * _resolutionScale[1]);
            float renderZ = z + _offset[2];

            // Create vertexs
            CustomVertex.PositionColoredTextured[] verts = new CustomVertex.PositionColoredTextured[1] 
			{
				new CustomVertex.PositionColoredTextured(renderX, renderY, renderZ, _vertexColors[0], 0, 0),
			};

            // Render.
            if (_currentShader != null)
            {
                int passes = _currentShader.Begin();
                for (int i = 0; i < passes; i++)
                {
                    _currentShader.BeginPass(i);
			        _dx9Device.DrawUserPrimitives(PrimitiveType.PointList, 1, verts);
                    _currentShader.FinishPass();
                }
                _currentShader.Finish();
            }
            else
                _dx9Device.DrawUserPrimitives(PrimitiveType.PointList, 1, verts);
		}

		/// <summary>
		///		Creates a new valid image frame from a pixelmap.
		/// 
		///		This is mainly used by the Image class to recreate images whenever the driver changes 
		/// </summary>
		/// <param name="pixelMap">Pixelmap to create new image frame from.</param>
		IImageFrame IGraphicsDriver.CreateImageFrame(PixelMap pixelMap)
		{
			return pixelMap == null ? new Direct3D9ImageFrame(this) : new Direct3D9ImageFrame(this, pixelMap);
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
                int aspectWidth = 0;
                int aspectHeight = 0;

                // This code was taken from a code guru article. Thanks guys :P.
                double dWidth = _renderTarget.Width;
                double dHeight = _renderTarget.Height;
                double dAspectRatio = dWidth / dHeight;

                double dPictureWidth = width;
                double dPictureHeight = height;
                double dPictureAspectRatio = dPictureWidth / dPictureHeight;

                if (dPictureAspectRatio >= dAspectRatio)
                {
                    int nNewHeight = (int)(dWidth/dPictureWidth*dPictureHeight);
                    int nCenteringFactor = (int)((dHeight - nNewHeight) / 2);
                    aspectWidth = (int)dWidth;
                    aspectHeight = nNewHeight;// +nCenteringFactor;
                    //_resolutionOffset[0] = 0;
                    //_resolutionOffset[1] = nCenteringFactor;
                }
                else if (dPictureAspectRatio < dAspectRatio)
                {
                    int nNewWidth =  (int)(dHeight/dPictureHeight*dPictureWidth);
                    int nCenteringFactor = (int)((dWidth - nNewWidth) / 2);
                    aspectWidth = nNewWidth;// +nCenteringFactor;
                    aspectHeight = (int)dHeight;
                    //_resolutionOffset[0] = nCenteringFactor;
                    //_resolutionOffset[1] = 0;
                }

                aspectWidth = (aspectWidth / width) * width;
                aspectHeight = (aspectHeight / height) * height;

                _resolutionOffset[0] = ((int)dWidth - aspectWidth) / 2;
                _resolutionOffset[1] = ((int)dHeight - aspectHeight) / 2;
				_resolutionScale[0] = (aspectWidth / (float)width);
                _resolutionScale[1] = (aspectHeight / (float)height);
            }
			else
			{
				_resolutionOffset[0] = 0;
				_resolutionOffset[1] = 0;
				_resolutionScale[0] = (float)_renderTarget.Width / width;
				_resolutionScale[1] = (float)_renderTarget.Height / height;
			}
			_viewport = new Rectangle(0,0,width,height);
			((IGraphicsDriver)this).ScaleFactor = new float[2] { 1.0f, 1.0f };
		}

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        public Direct3D9Driver()
        {
            _vertexColors.AllVertexs = unchecked((int)0xFFFFFFFF);
        }

		#endregion
	}

}