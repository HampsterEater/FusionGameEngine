/* 
 * File: Native.cs
 *
 * This source file contains the declaration of the NativeMethods class which is
 * used to import any neccessary win32 native functions.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Text;
using System.Runtime.InteropServices;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Runtime
{

	public unsafe static class NativeMethods
	{
		#region Members
		#region Properties

		/// <summary>
		///		Gets the idle (not processing messages) state of this application. 
		/// </summary>
		public static bool IsApplicationIdle
		{
			get
			{
				Win32Message msg;
				return !NativeMethods.PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
			}
		}

        /// <summary>
        ///     Gets the current state of the caps lock key.
        /// </summary>
        public static bool CapsLockDown
        {
            get
            {
                if ((GetKeyState(0x14) & 1) != 0)
                    return true;
                return false;
            }
        }

		#endregion
		#endregion
		#region External Methods

        // These methods deal with memory management.
        [DllImport("Kernel32.dll")]
        public static extern bool SetProcessWorkingSetSize(IntPtr handle, int minSize, int maxSize);

		// These methods are used to peek at and translate win32 messages.
		[System.Security.SuppressUnmanagedCodeSecurity]
		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern bool PeekMessage(out Win32Message msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

		// These methods are used to query the Win32 high resolution timer.
		[DllImport("Kernel32.dll")] 
		public static extern bool QueryPerformanceCounter(out long counter);
		[DllImport("Kernel32.dll")]
		public static extern bool QueryPerformanceFrequency(out long frequency);

		// Used with the RichTextBox control when modifying large chunks of text.
		[DllImport("User32.dll")]
		public static extern int LockWindowUpdate(int hWnd);

		// Used by many numerous pieces of code to send event notifications to controls.
		[DllImport("User32.dll")]
		public static extern int SendMessage(HandleRef hWnd, int msg, int wParam, ref POINT lp);

        // Input stuff.
        [DllImport("user32.dll")]
        internal static extern ushort GetKeyState(uint nVirtKey);

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);

        [DllImport("user32.dll")]
        static extern int ToAsciiEx(uint uVirtKey, uint uScanCode, byte[] lpKeyState, short *lpChar, long uFlags, IntPtr hkl);

        // Graphics
        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        public static extern int GetSystemMetrics(int abc);

        // Medis stuff.
        [DllImport("winmm.dll", EntryPoint = "mciSendStringA")]
        public static extern long mciSendString(string lpstrCommand, StringBuilder lpstrReturnString, long uReturnLength, IntPtr hwndCallback);

		#endregion
		#region Enumerations

		/// <summary>
		///		Stores the hexidecimal code of specific win32 message that can 
		///		be used with the SendMessage function.
		/// </summary>
		public enum Win32Messages
		{
			EM_GETSCROLLPOS = 0x0400 + 221,
			EM_SETSCROLLPOS = 0x0400 + 222
		}

		#endregion
        #region Methods

        public unsafe static char VirtualKeyToChar(uint virtualKey)
        {
           IntPtr layout = GetKeyboardLayout(0);
           byte[] state = new byte[256];

           if (GetKeyboardState(state) == false)
              return '\0';

           short result = 0;
           int val = ToAsciiEx(virtualKey, virtualKey, state, &result, 0, layout);
           return (char)result;
        }

        #endregion
        #region Internal Structures

        // This structure stores a classic win32 point value.
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public long X;
			public long Y;
		}

		// This structure is used to describe a Win32 message.
		[StructLayout(LayoutKind.Sequential)]
		public struct Win32Message
		{
			public IntPtr hWnd;
			public IntPtr msg;
			public IntPtr wParam;
			public IntPtr lParam;
			public uint time;
			public System.Drawing.Point p;
		}

		#endregion
	}

}