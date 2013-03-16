/* 
 * File: Input Event.cs
 *
 * This source file contains the declaration of the InputEventData class
 * which is used to describe an input releated event.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Events;
using BinaryPhoenix.Fusion.Graphics;
//using BinaryPhoenix.Fusion.Graphics.Direct3D9Driver;

namespace BinaryPhoenix.Fusion.Input
{

	/// <summary>
	///		Describes the data contained in an input event.
	/// </summary>
	public class InputEventData 
	{
		#region Members
		#region Variables

		private KeyCodes _keyCode;
		private bool _pressed;

		private int _mouseX;
		private int _mouseY;
		private int _mouseZ;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the keycode associated with this event.
		/// </summary>
		public KeyCodes KeyCode
		{
			get { return _keyCode;  }
			set { _keyCode = value; }
		}

		/// <summary>
		///		Gets or sets if this event was fired when a key was released or pressed.
		/// </summary>
		public bool Pressed
		{
			get { return _pressed;  }
			set { _pressed = value; }
		}

		/// <summary>
		///		Gets or sets the mouses position on the x-axis.
		/// </summary>
		public int MouseX
		{
			get { return _mouseX; }
			set { _mouseX = value; }
		}

		/// <summary>
		///		Gets or sets the mouses position on the y-axis.
		/// </summary>
		public int MouseY
		{
			get { return _mouseY; }
			set { _mouseY = value; }
		}

		/// <summary>
		///		Gets or sets the mouses scroll position.
		/// </summary>
		public int MouseScroll
		{
			get { return _mouseZ; }
			set { _mouseZ = value; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Initializes a new instance of this class.
		/// </summary>
		/// <param name="keyCode">KeyCode of key that fired this event.</param>
		/// <param name="pressed">If this event was fired by pressing or releasing the event.</param>
		/// <param name="mx">Position of the mouse on the x-axis.</param>
		/// <param name="my">Position of the mouse on the y-axis.</param>
		/// <param name="mz">Position of the mouse on the z-axis.</param>
		public InputEventData(KeyCodes keyCode, bool pressed, int mx, int my, int mz)
		{
			_keyCode = keyCode;
			_pressed = pressed;
			_mouseX = mx;
			_mouseY = my;
			_mouseZ = mz;
		}

		#endregion
	}

}