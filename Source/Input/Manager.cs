/* 
 * File: Manager.cs
 *
 * This source file contains the declaration of the InputManager class which 
 * contains static versions of every function in the IInputDriver class to make
 * for easier access and tracking.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Threading;
using System.Collections;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Events;
//using BinaryPhoenix.Fusion.Graphics.Direct3D9Driver;

namespace BinaryPhoenix.Fusion.Input
{
	#region Input Manager Classes

	/// <summary>
	///		The input manager is used to keep track of the current driver
	///		being used to render graphics to the screen, it also provides an abstration
	///		layer between the driver and any input calls. This
	///		is usfull as it means the input driver does not have to be passed between
	///		different classes so that each can directly call the drivers methods. 
	/// </summary>
	public static class InputManager
	{
		#region Members
		#region Variables

		private static IInputDriver _driver;
		private static Hashtable _keyBindings = new Hashtable();

        private static KeyCodes _lastKey;
        private static HighPreformanceTimer _fastKeyTimer = new HighPreformanceTimer();
        private static int _fastKeyDelay = 500;

        private static int _grabIndex = 0;

        private static int[] _virtualKeys = new int[]
            {
            #region Virtual Key Map
		        0x00,
		        0x00,
		        0xB2,
		        0x1C,
		        0x6E,
		        0x58,
		        0x59,
		        0x1B,
		        0x0202,
		        0x22,
		        0x28, 
		        0x27,
		        0x25,
		        0x21,
		        0x26,
		        0xA4,
		        0xE2,
		        0xBE,
		        0xBB,
		        0xBD,
		        0x14,
		        0xA5,
		        0x6A,
		        0x08,
		        0xB5,
		        0xB4,
		        0xB6,
		        0xA6,
		        0xA7,
		        0xA9,
		        0xA8,
		        0xAB,
		        0xAA,
		        0x5F,
		        0x5E,
		        0x5D,
		        0x5C,
		        0x5B,
		        0x28,
		        0x23,
		        0x21,
		        0x26,
		        0x24,
		        0xA5,
		        0x2C,
		        0x6F,
		        0xBC,
		        0xAC,
		        0xAF,
		        0xAE,
		        0xB2,
		        0xFA,
		        0xB7,
		        0xAD,
		        0xA3,
		        0x0D,
		        0xB0,
		        0x97,
		        0x96,
		        0x19,
		        0x93,
		        0xBA,
		        0x0200,
		        0xB1,
		        0x92,
		        0x7E,
		        0x7D,
		        0x1D,
		        0x73,
		        0x15,
		        0x7E,
		        0x7D,
		        0x7C,
		        0x7B,
		        0x7A,
		        0xDC,
		        0x60,
		        0x63,
		        0x62,
		        0x61,
		        0x66,
		        0x65,
		        0x64,
		        0x6D,
		        0x69,
		        0x68,
		        0x67,
		        0x91,
		        0x90,
		        0x79,
		        0x78,
		        0x77,
		        0x76,
		        0x75,
		        0x74,
		        0x73,
		        0x72,
		        0x71,
		        0x70,
		        0x14,
		        0x20,
		        0xA4,
		        0x6A,
		        0xA1,
		        0xBF,
		        0xBE,
		        0xBC,
		        0x4D,
		        0x4E,
		        0x42,
		        0x56,
		        0x43,
		        0x5A,
		        0xDE,
		        0xA0,
		        0xDF,
		        0xC0,
		        0xBA,
		        0x4C,
		        0x4B,
		        0x4A,
		        0x48,
		        0x47,
		        0x46,
		        0x44,
		        0x53,
		        0x41,
		        0xA2,
		        0x0D,
		        0xDD,
		        0xDB,
		        0x50,
		        0x4F,
		        0x49,
		        0x55,
		        0x54,
		        0x52,
		        0x45,
		        0x57,
		        0x09,
		        0x08,
		        0xBB,
		        0xBD,
		        0x30,
		        0x39,
		        0x38,
		        0x37,
		        0x36,
		        0x35,
		        0x34,
		        0x33,
		        0x32,
		        0x31,
		        0x2D,
		        0x27,
		        0x25,
		        0x13,
		        0x6B,
		        0x2E,
		        0x51,

		        0x01,
		        0x02,
		        0x04
            #endregion
            };

        private static int[] _hitKeyTimers = new int[_virtualKeys.Length];
        private static bool[] _hitKeys = new bool[_virtualKeys.Length];

		#endregion
		#region Properties
        
		/// <summary>
		///		Gets or sets the current input driver being used.
		/// </summary>
		public static IInputDriver Driver
		{
			get { return _driver; }
			set { _driver = value; }
		}

		/// <summary>
		///		Gets or sets the table of current key bindings.
		/// </summary>
		public static Hashtable KeyBindings
		{
			get { return _keyBindings; }
			set { _keyBindings = value; }
		}

		/// <summary>
		///		Gets the current position on the x axis of the mouse.
		/// </summary>
		public static int MouseX
		{
			get { return _driver.MouseX; }
		}

		/// <summary>
		///		Gets the current position on the y axis of the mouse.
		/// </summary>
		public static int MouseY
		{
			get { return _driver.MouseY; }
		}

		/// <summary>
		///		Gets the current position of the scroller on the mouse.
		/// </summary>
		public static int MouseScroll
		{
			get { return _driver.MouseScroll; }
		}

		/// <summary>
		///		Gets the delta position on the x axis of the mouse.
		/// </summary>
		public static int MouseDeltaX
		{
			get { return _driver.MouseDeltaX; }
		}

		/// <summary>
		///		Gets the delta position on the y axis of the mouse.
		/// </summary>
		public static int MouseDeltaY
		{
			get { return _driver.MouseDeltaY; }
		}

		/// <summary>
		///		Gets the delta position of the scroller on the mouse.
		/// </summary>
		public static int MouseDeltaScroll
		{
			get { return _driver.MouseDeltaScroll; }
		}

		/// <summary>
		///		Gets the current position on the x axis of the mouse. Relative to the screen dimensions.
		/// </summary>
		public static int MouseScreenX
		{
			get { return _driver.MouseScreenX; }
		}

		/// <summary>
		///		Gets the current position on the y axis of the mouse. Relative to the screen dimensions.
		/// </summary>
		public static int MouseScreenY
		{
			get { return _driver.MouseScreenY; }
		}

		#endregion
		#endregion
		#region Static Functions

        /// <summary>
        ///     Polls the manager to recieve new input.
        /// </summary>
        public static void Poll()
        {
            _driver.Poll();
            _grabIndex = 0;

            // Sort out the hits.
            for (int i = 0; i < _virtualKeys.Length; i++)
            {
                if (KeyDown((KeyCodes)i) == true)
                {
                    _hitKeys[i] = (_hitKeyTimers[i] >= 3);
                    _hitKeyTimers[i] = 0;
                }
                else
                {
                    _hitKeys[i] = false;
                    _hitKeyTimers[i]++;
                }
            }
        }

        /// <summary>
        ///     Grabs the first key that is currently pressed.
        /// </summary>
        /// <returns>Keycode of the first key current pressed.</returns>
        public static KeyCodes GetKey()
        {
            for (int i = _grabIndex; i < 160; i++)
                if (KeyDown((KeyCodes)i) == true)
                {
                    _grabIndex = i + 1;
                    return (KeyCodes)i;
                }
            _grabIndex = 160;
            return (KeyCodes)0;
        }

        /// <summary>
        ///     Grabs the first key that is currently pressed and returns it as a character.
        /// </summary>
        /// <returns>ASCII character of the first key current pressed.</returns>
        public static char GetChar()
        {
            while (_grabIndex < 160)
            {
                KeyCodes keyCode = GetKey();
                if ((int)keyCode != 0)
                {
                    char chr = NativeMethods.VirtualKeyToChar((uint)_virtualKeys[(int)keyCode]);
                    if (chr != '\0')
                    {
                        if (_lastKey == keyCode && _fastKeyTimer.DurationMillisecond > _fastKeyDelay)
                            return chr;
                        else
                        {
                            if (_lastKey != keyCode)
                            {
                                _fastKeyTimer.Restart();
                                _lastKey = keyCode;
                                return chr;
                            }
                            return '\0';
                        }
                    }
                }
            }

            _fastKeyTimer.Restart();
            _lastKey = (KeyCodes)0;
            return '\0';
        }


		/// <summary>
		///		Returns true if the given key has just been pressed.
		/// </summary>
		/// <param name="key">Key code of key to check.</param>
		/// <returns>True if key is pressed.</returns>
		public static bool KeyPressed(KeyCodes key)
		{
			return _driver.KeyPressed(key);
		}

        /// <summary>
        ///		Returns true if the given key has just been hit.
        /// </summary>
        /// <param name="key">Key code of key to check.</param>
        /// <returns>True if key is pressed.</returns>
        public static bool KeyHit(KeyCodes key)
        {
            return _hitKeys[(int)key];
        }

		/// <summary>
		///		Returns true if the given binded key has just been pressed.
		/// </summary>
		/// <param name="name">Name of binded key to check.</param>
		public static bool BindedKeyPressed(string name)
		{
			name = name.ToLower();
			if (_keyBindings.Contains(name) == false) return false;
			return _driver.KeyPressed((KeyCodes)_keyBindings[name]);
		}

        /// <summary>
        ///		Returns true if the given binded key has just been hit.
        /// </summary>
        /// <param name="name">Name of binded key to check.</param>
        public static bool BindedKeyHit(string name)
        {
            name = name.ToLower();
            if (_keyBindings.Contains(name) == false) return false;
            return _hitKeys[(int)_keyBindings[name]];
        }

		/// <summary>
		///		Retruns true if the given key is currently down.
		/// </summary>
		/// <param name="key">Key code of key to check.</param>
		/// <returns>True if key is currently down.</returns>
		public static bool KeyDown(KeyCodes key)
		{
			return _driver.KeyDown(key);
		}

		/// <summary>
		///		Returns true if the given binded key is currently down.
		/// </summary>
		/// <param name="name">Name of binded key to check.</param>
		public static bool BindedKeyDown(string name)
		{
			name = name.ToLower();
			if (_keyBindings.Contains(name) == false) return false;
			return _driver.KeyDown((KeyCodes)_keyBindings[name]);
		}

		/// <summary>
		///		Returns true if the given key has just been released.
		/// </summary>
		/// <param name="key">Key code of key to check.</param>
		/// <returns>True if key has just been released.</returns>
		public static bool KeyReleased(KeyCodes key)
		{
			return _driver.KeyReleased(key);
		}

		/// <summary>
		///		Returns true if the given binded key has just been released.
		/// </summary>
		/// <param name="name">Name of binded key to check.</param>
		public static bool BindedKeyReleased(string name)
		{
			name = name.ToLower();
			if (_keyBindings.Contains(name) == false) return false;
			return _driver.KeyReleased((KeyCodes)_keyBindings[name]);
		}

		/// <summary>
		///		Binds a keyboard key to a given name.
		/// </summary>
		/// <param name="key">Key to bind.</param>
		/// <param name="name">Name to bind key to.</param>
		public static void BindKey(string name, KeyCodes key)
		{
			name = name.ToLower();
			if (_keyBindings.Contains(name) == true) _keyBindings.Remove(name);
			_keyBindings.Add(name, key);
		}

		/// <summary>
		///		Unbinds the event attached to the given key.
		/// </summary>
		/// <param name="name">Name of key to unbind.</param>
		public static void UnbindKey(string name)
		{
			name = name.ToLower();
			if (_keyBindings.Contains(name) == true) _keyBindings.Remove(name);
		}

		#endregion
	}

	#endregion
}