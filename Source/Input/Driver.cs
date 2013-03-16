/* 
 * File: Driver.cs
 *
 * This source file contains the declaration of the IInputDriver interface which is
 * used as an intermediate stage between the game engine and input API's, this keeps
 * the input API's isolated from the game.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Windows.Forms;

namespace BinaryPhoenix.Fusion.Input
{
	/// <summary>
	///		Contains the key codes of all keys on the keyboard.
	/// </summary>
	public enum KeyCodes
	{
		Sleep,
		Next,
		Stop,
		Convert,
		Decimal,
		X,
		Y,
		Escape,
		Circumflex,
		PageDown,
		DownArrow, 
		RightArrow,
		LeftArrow,
		PageUp,
		UpArrow,
		RightAlt,
		NumPadSlash,
		NumPadPeriod,
		NumPadPlus,
		NumPadMinus,
		CapsLock,
		LeftAlt,
		NumPadStar,
		BackSpace,
		MediaSelect,
		Mail,
		MyComputer,
		WebBack,
		WebForward,
		WebStop,
		WebRefresh,
		WebFavorites,
		WebSearch,
		Wake,
		Power,
		Apps,
		RightWindows,
		LeftWindows,
		Down,
		End,
		Prior,
		Up,
		Home,
		RightMenu,
		SysRq,
		Divide,
		NumPadComma,
		WebHome,
		VolumeUp,
		VolumeDown,
		MediaStop,
		PlayPause,
		Calculator,
		Mute,
		RightControl,
		NumPadEnter,
		NextTrack,
		Unlabeled,
		AX,
		Kanji,
		Underline,
		Colon,
		At,
		PrevTrack,
		NumPadEquals,
		AbntC2,
		Yen,
		NoConvert,
		AbntC1,
		Kana,
		F15,
		F14,
		F13,
		F12,
		F11,
		OEM102,
		NumPad0,
		NumPad3,
		NumPad2,
		NumPad1,
		NumPad6,
		NumPad5,
		NumPad4,
		Subtract,
		NumPad9,
		NumPad8,
		NumPad7,
		Scroll,
		Numlock,
		F10,
		F9,
		F8,
		F7,
		F6,
		F5,
		F4,
		F3,
		F2,
		F1,
		Capital,
		Space,
		LeftMenu,
		Multiply,
		RightShift,
		Slash,
		Period,
		Comma,
		M,
		N,
		B,
		V,
		C,
		Z,
		BackSlash,
		LeftShift,
		Grave,
		Apostrophe,
		SemiColon,
		L,
		K,
		J,
		H,
		G,
		F,
		D,
		S,
		A,
		LeftControl,
		Return,
		RightBracket,
		LeftBracket,
		P,
		O,
		I,
		U,
		T,
		R,
		E,
		W,
		Tab,
		Back,
		Equals,
		Minus,
		D0,
		D9,
		D8,
		D7,
		D6,
		D5,
		D4,
		D3,
		D2,
		D1,
		Insert,
		Right,
		Left,
		Pause,
		Add,
		Delete,
		Q,

		LeftMouse,
		RightMouse,
		MiddleMouse
	}

	/// <summary>
	///     The IInputDriver interface is used to provide an abstraction layer above all input
	///     API calls, which allows the input API to be completly isolated from the 
	///     game, allowing easier extendability and porting. 
	/// </summary>
	public interface IInputDriver
	{
		#region Properties

		int MouseX { get; }
		int MouseY { get; }
		int MouseScroll { get; }
		int MouseDeltaX { get; }
		int MouseDeltaY { get; }
		int MouseDeltaScroll { get; }
		int MouseScreenX { get; }
		int MouseScreenY { get; }

		#endregion
		#region Methods

        void Poll();

		bool KeyPressed(KeyCodes key);
		bool KeyDown(KeyCodes key);
		bool KeyReleased(KeyCodes key);

		void AttachToControl(Control control);

		#endregion
	}

}