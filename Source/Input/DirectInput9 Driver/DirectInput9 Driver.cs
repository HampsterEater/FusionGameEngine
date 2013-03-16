/* 
 * File: DirectInput9 Driver.cs
 *
 * This source file contains the declaration of the DirectInput9Driver class, which adds
 * the capability of using DirectInput9 for capturing user input.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Threading;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Graphics;
using BinaryPhoenix.Fusion.Runtime.Events;
using BinaryPhoenix.Fusion.Runtime.Debug;

namespace BinaryPhoenix.Fusion.Input.DirectInput9Driver
{

	/// <summary>
	///     The DirectInput9Driver allows the use of DirectInput9 to capture user input.
	/// </summary>
	public sealed class DirectInput9Driver : IInputDriver
	{
		#region Members
		#region Variables

		private Device _keyboardDevice, _mouseDevice;
		private KeyboardState _keyboardState;
		private MouseState _mouseState, _previousMouseState;

		private int _mouseZ, _mouseDeltaZ, _mouseDeltaX, _mouseDeltaY;

		private bool[] _previousKeyboardState = new bool[256];

		private Key[] _directInputKeyCodeMap = new Key[160]
			{
			#region Key Map
				Key.Sleep, 
				Key.Next, 
				Key.Stop, 
				Key.Convert,  
				Key.Decimal, 
				Key.X,
				Key.Y,
				Key.Escape,
				Key.Circumflex,  
				Key.PageDown,  
				Key.DownArrow, 
				Key.RightArrow,
				Key.LeftArrow, 
				Key.PageUp,  
				Key.UpArrow, 
				Key.RightAlt,  
				Key.NumPadSlash,  
				Key.NumPadPeriod, 
				Key.NumPadPlus,
				Key.NumPadMinus,
				Key.CapsLock, 
				Key.LeftAlt,  
				Key.NumPadStar, 
				Key.BackSpace,   
				Key.MediaSelect,   
				Key.Mail, 
				Key.MyComputer, 
				Key.WebBack, 
				Key.WebForward,
				Key.WebStop, 
				Key.WebRefresh, 
				Key.WebFavorites,
				Key.WebSearch,
				Key.Wake,
				Key.Power,   
				Key.Apps, 
				Key.RightWindows,  
				Key.LeftWindows,
				Key.Down, 
				Key.End, 
				Key.Prior, 
				Key.Up, 
				Key.Home, 
				Key.RightMenu, 
				Key.SysRq, 
				Key.Divide, 
				Key.NumPadComma,
				Key.WebHome,  
				Key.VolumeUp,  
				Key.VolumeDown,  
				Key.MediaStop, 
				Key.PlayPause, 
				Key.Calculator,
				Key.Mute, 
				Key.RightControl,  
				Key.NumPadEnter,
				Key.NextTrack, 
				Key.Unlabeled,   
				Key.AX,   
				Key.Kanji, 
				Key.Underline,
				Key.Colon,  
				Key.At,  
				Key.PrevTrack,  
				Key.NumPadEquals, 
				Key.AbntC2, 
				Key.Yen, 
				Key.NoConvert,  
				Key.AbntC1, 
				Key.Kana, 
				Key.F15,  
				Key.F14,  
				Key.F13,  
				Key.F12, 
				Key.F11, 
				Key.OEM102,  
				Key.NumPad0,  
				Key.NumPad3, 
				Key.NumPad2,
				Key.NumPad1,  
				Key.NumPad6,  
				Key.NumPad5,  
				Key.NumPad4,  
				Key.Subtract,  
				Key.NumPad9, 
				Key.NumPad8,  
				Key.NumPad7, 
				Key.Scroll, 
				Key.Numlock, 
				Key.F10, 
				Key.F9,
				Key.F8,  
				Key.F7,  
				Key.F6, 
				Key.F5,  
				Key.F4,  
				Key.F3, 
				Key.F2, 
				Key.F1,  
				Key.Capital,  
				Key.Space,
				Key.LeftMenu,  
				Key.Multiply,  
				Key.RightShift, 
				Key.Slash,  
				Key.Period,  
				Key.Comma, 
				Key.M,  
				Key.N,  
				Key.B,  
				Key.V,  
				Key.C,  
				Key.Z, 
				Key.BackSlash,  
				Key.LeftShift,  
				Key.Grave,  
				Key.Apostrophe,  
				Key.SemiColon,  
				Key.L,  
				Key.K,  
				Key.J, 
				Key.H,  
				Key.G,  
				Key.F,  
				Key.D,  
				Key.S,  
				Key.A,  
				Key.LeftControl, 
				Key.Return, 
				Key.RightBracket,  
				Key.LeftBracket,  
				Key.P,  
				Key.O, 
				Key.I,  
				Key.U,  
				Key.T,  
				Key.R,
				Key.E,  
				Key.W,   
				Key.Tab, 
				Key.Back,
				Key.Equals, 
				Key.Minus, 
				Key.D0,  
				Key.D9,  
				Key.D8, 
				Key.D7, 
				Key.D6, 
				Key.D5,  
				Key.D4, 
				Key.D3, 
				Key.D2, 
				Key.D1, 
				Key.Insert,
				Key.Right,  
				Key.Left,  
				Key.Pause, 
				Key.Add,
				Key.Delete, 
				Key.Q
			#endregion
			};

		#endregion
		#region Properties 

		/// <summary>
		///		Gets the current DirectInput9 used to gather keyboard input.
		/// </summary>
		public Device KeyboardDevice
		{
			get { return _keyboardDevice; }
		}

		/// <summary>
		///		Gets the current DirectInput9 used to gather mouse input.
		/// </summary>
		public Device MouseDevice
		{
			get { return _mouseDevice; }
		}

		/// <summary>
		///		Gets the current position of the mouse on the X axis.
		/// </summary>
		int IInputDriver.MouseX
		{
			get 
			{
				if (GraphicsManager.RenderTarget is GraphicsCanvas == false) return 0;
				return ((GraphicsCanvas)GraphicsManager.RenderTarget).RenderControl.PointToClient(new Point(Cursor.Position.X, Cursor.Position.Y)).X; 
			}
		}

		/// <summary>
		///		Gets the current position of the mouse on the Y axis.
		/// </summary>
		int IInputDriver.MouseY
		{
			get 
			{
				if (GraphicsManager.RenderTarget is GraphicsCanvas == false) return 0;
				return ((GraphicsCanvas)GraphicsManager.RenderTarget).RenderControl.PointToClient(new Point(Cursor.Position.X, Cursor.Position.Y)).Y;
			}
		}

		/// <summary>
		///		Gets the current position of the mouse on the X axis. Relative to the screen dimensions.
		/// </summary>
		int IInputDriver.MouseScreenX
		{
			get { return Cursor.Position.X; }
		}

		/// <summary>
		///		Gets the current position of the mouse on the Y axis. Relative to the screen dimensions.
		/// </summary>
		int IInputDriver.MouseScreenY
		{
			get { return Cursor.Position.Y; }
		}

		/// <summary>
		///		Gets the current position of the scroller on the mouse.
		/// </summary>
		int IInputDriver.MouseScroll
		{
			get { return _mouseZ; }
		}

		/// <summary>
		///		Gets the delta position of the mouse on the x-axis.
		/// </summary>
		int IInputDriver.MouseDeltaX
		{
			get { return _mouseDeltaX; }
		}

		/// <summary>
		///		Gets the delta position of the mouse on the t-axis.
		/// </summary>
		int IInputDriver.MouseDeltaY
		{
			get { return _mouseDeltaY; }
		}

		/// <summary>
		///		Gets the delta position of the mouse's scroll.
		/// </summary>
		int IInputDriver.MouseDeltaScroll
		{
			get { return _mouseDeltaZ; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Returns true if the given key has just been pressed.
		/// </summary>
		/// <param name="key">Key code of key to check.</param>
		/// <returns>True if key is pressed.</returns>
		bool IInputDriver.KeyPressed(KeyCodes key)
		{
			if ((int)key >= 160) return MousePressed(key);
			if (_keyboardState == null || _previousKeyboardState == null) return false;
			bool previousState = _previousKeyboardState[(int)_directInputKeyCodeMap[(int)key]];
			return (_keyboardState[_directInputKeyCodeMap[(int)key]] == true && previousState == false);
		}

		/// <summary>
		///		Retruns true if the given key is currently down.
		/// </summary>
		/// <param name="key">Key code of key to check.</param>
		/// <returns>True if key is currently down.</returns>
		bool IInputDriver.KeyDown(KeyCodes key)
		{
			if ((int)key >= 160) return MouseDown(key);
			if (_keyboardState == null) return false;
			return _keyboardState[_directInputKeyCodeMap[(int)key]];
		}

		/// <summary>
		///		Returns true if the given key has just been released.
		/// </summary>
		/// <param name="key">Key code of key to check.</param>
		/// <returns>True if key has just been released.</returns>
		bool IInputDriver.KeyReleased(KeyCodes key)
		{
			if ((int)key >= 160) return MouseReleased(key);
			if (_keyboardState == null || _previousKeyboardState == null) return false;
			bool previousState = _previousKeyboardState[(int)_directInputKeyCodeMap[(int)key]];
			return (_keyboardState[_directInputKeyCodeMap[(int)key]] == false && previousState == true);
		}

		/// <summary>
		///		Returns true if the given mouse key has just been pressed.
		/// </summary>
		/// <param name="key">Mouse key code of key to check.</param>
		/// <returns>True if mouse key has just been pressed.</returns>
		private bool MousePressed(KeyCodes key)
		{
			byte[] mouseButtons = _mouseState.GetMouseButtons();
			byte[] previousMouseButtons = _previousMouseState.GetMouseButtons();
			if (mouseButtons == null || previousMouseButtons == null) return false;

			byte prevState = previousMouseButtons[((int)key) - 160];
			
			return ((mouseButtons[((int)key) - 160] != 0) && prevState == 0);
		}

		/// <summary>
		///		Retruns true if the given mouse button is currently down.
		/// </summary>
		/// <param name="key">Mouse key code of key to check.</param>
		/// <returns>True if mouse key is currently down.</returns>
		private bool MouseDown(KeyCodes key)
		{
			if (_mouseState.GetMouseButtons() == null) return false;
			return (_mouseState.GetMouseButtons()[((int)key) - 160] != 0);
		}

		/// <summary>
		///		Returns true if the given mouse key has just been released.
		/// </summary>
		/// <param name="key">Mouse key code of key to check.</param>
		/// <returns>True if mouse key has just been released.</returns>
		private bool MouseReleased(KeyCodes key)
		{
			byte[] mouseButtons = _mouseState.GetMouseButtons();
			byte[] previousMouseButtons = _previousMouseState.GetMouseButtons();
			if (mouseButtons == null || previousMouseButtons == null) return false;

			byte prevState = previousMouseButtons[((int)key) - 160];

			return ((mouseButtons[((int)key) - 160] == 0) && prevState != 0);
		}

		/// <summary>
		///		Attachs this drivers input / output control to the given one.
		/// </summary>
		/// <param name="control">Control to attach this driver to.</param>
		void IInputDriver.AttachToControl(Control control)
		{
			//_keyboardDevice.SetCooperativeLevel(control, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
			//_mouseDevice.SetCooperativeLevel(control, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
		}

		/// <summary>
		///		Initializes the DirectInput9 device so its capable of capturing 
		///		input from the basic devices (keyword and mouse).
		/// </summary>
		public void InitializeDevice()
		{
			// Create the input device's.
			_keyboardDevice = new Device(SystemGuid.Keyboard);
			if (_keyboardDevice == null) return;
			_mouseDevice = new Device(SystemGuid.Mouse);
			if (_mouseDevice == null) return;

			// Set the cooperative mode of this device.
			//if (GraphicsManager.RenderTarget as GraphicsCanvas != null)
			//{
			_keyboardDevice.SetCooperativeLevel(null, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
			_mouseDevice.SetCooperativeLevel(null, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
			//}

			// Decide what data format we want the device to return.
			_keyboardDevice.SetDataFormat(DeviceDataFormat.Keyboard);
			_mouseDevice.SetDataFormat(DeviceDataFormat.Mouse);

			// Aquire a connection to the physical device.
			try
			{
				_keyboardDevice.Acquire();
				_mouseDevice.Acquire();
			}
			catch (DirectXException)
			{
				_keyboardDevice = null;
				_mouseDevice = null;
				return;
			}
		}

        /// <summary>
        ///     Polls the input devices for new input.
        /// </summary>
        void IInputDriver.Poll()
        {
            KeyboardPoll();
            MousePoll();
        }

		/// <summary>
		///		Polls the keyboard each time new input arrives.
		/// </summary>
		private void KeyboardPoll()
		{
			try
			{
				_keyboardDevice.Poll();
				if (_keyboardState != null)
					for (int i = 0; i < 160; i++)
						_previousKeyboardState[i] = _keyboardState[(Key)i];
				_keyboardState = _keyboardDevice.GetCurrentKeyboardState();
			}
			catch (NotAcquiredException)
			{
                DebugLogger.WriteLog("Attempting to acquire DirectInput 9 keyboard device.");    
				// Reaquire a connection to the physical device.
				try
				{
					_keyboardDevice.Acquire();
				}
				catch (DirectXException)
				{
					_keyboardDevice = null;
                    return;
				}
			}
			catch (InputException)
			{
                return;
			}

			// Fire input events if we have gained any.
			for (int i = 0; i < 160; i++)
				if (((IInputDriver)this).KeyPressed((KeyCodes)i) == true)
					EventManager.FireEvent(new Event("key_pressed", this, new InputEventData((KeyCodes)i, true, 0, 0, 0)));
				else if (((IInputDriver)this).KeyReleased((KeyCodes)i) == true)
					EventManager.FireEvent(new Event("key_released", this, new InputEventData((KeyCodes)i, false, 0, 0, 0)));
	}

		/// <summary>
		///		Polls the mouse each time new input arrives
		/// </summary>
		private void MousePoll()
		{
			try
			{
				_mouseDevice.Poll();
				_previousMouseState = _mouseState;
				_mouseState = _mouseDevice.CurrentMouseState;

				if (_mouseState.X != 0 || _mouseState.Y != 0 || _mouseState.Z != 0)
					EventManager.FireEvent(new Event("mouse_move", this, new InputEventData(0, true, _mouseState.X, _mouseState.Y, _mouseState.Z)));

				for (int i = 0; i < 3; i++)
				{
					if (MousePressed((KeyCodes)((int)KeyCodes.LeftMouse + i)) == true)
						EventManager.FireEvent(new Event("key_pressed", this, new InputEventData((KeyCodes)((int)KeyCodes.LeftMouse + i), true, _mouseState.X, _mouseState.Y, _mouseState.Z)));
					else if (MouseReleased((KeyCodes)((int)KeyCodes.LeftMouse + i)) == true)
						EventManager.FireEvent(new Event("key_released", this, new InputEventData((KeyCodes)((int)KeyCodes.LeftMouse + i), false, _mouseState.X, _mouseState.Y, _mouseState.Z)));
				}

				// This needs fixing, currently its reading the GDI
				// input of DirectInput >.>.
				_mouseDeltaX = _mouseState.X;
				_mouseDeltaY = _mouseState.Y;
				_mouseDeltaZ = _mouseState.Z;
				_mouseZ += _mouseState.Z;	
			}
			catch (NotAcquiredException)
			{
                DebugLogger.WriteLog("Attempting to acquire DirectInput mouse device.");  

				// Reaquire a connection to the physical device.
				try
				{
					_mouseDevice.Acquire();
				}
				catch (DirectXException)
				{
					_mouseDevice = null;
                    return;
				}
			}
			catch (InputException)
			{
                return;
			}
		}

		/// <summary>
		///		Initializes a new instance of this class.
		/// </summary>
		public DirectInput9Driver()
		{
			InitializeDevice();
		}

		#endregion
	}

}