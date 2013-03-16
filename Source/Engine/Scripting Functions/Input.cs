/* 
 * File: Input.cs
 *
 * This source file contains the declaration of the InputFunctionSet class which 
 * contains numerous native functions that are commonly used in scripts.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Scripting;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Engine.Entitys;
using BinaryPhoenix.Fusion.Input;

namespace BinaryPhoenix.Fusion.Engine.ScriptingFunctions
{

	/// <summary>
	///		Contains numerous native functions that are commonly used in scripts.
	///		None are commented as they are all fairly self-explanatory.
	/// </summary>
	public class InputFunctionSet : NativeFunctionSet
	{
		[NativeFunctionInfo("KeyDown", "bool", "int")]
		public void KeyDown(ScriptThread thread)
		{
			thread.SetReturnValue(InputManager.KeyDown((KeyCodes)thread.GetIntegerParameter(0)));
		}

		[NativeFunctionInfo("BindedKeyDown", "bool", "string")]
		public void BindedKeyDown(ScriptThread thread)
		{
			thread.SetReturnValue(InputManager.BindedKeyDown(thread.GetStringParameter(0)));
		}

        [NativeFunctionInfo("KeyHit", "bool", "int")]
        public void KeyHit(ScriptThread thread)
        {
            thread.SetReturnValue(InputManager.KeyHit((KeyCodes)thread.GetIntegerParameter(0)));
        }

        [NativeFunctionInfo("BindedKeyHit", "bool", "string")]
        public void BindedKeyHit(ScriptThread thread)
        {
            thread.SetReturnValue(InputManager.BindedKeyHit(thread.GetStringParameter(0)));
        }

		[NativeFunctionInfo("KeyPressed", "bool", "int")]
		public void KeyPressed(ScriptThread thread)
		{
			thread.SetReturnValue(InputManager.KeyPressed((KeyCodes)thread.GetIntegerParameter(0)));
		}

		[NativeFunctionInfo("BindedKeyPressed", "bool", "string")]
		public void BindedKeyPressed(ScriptThread thread)
		{
			thread.SetReturnValue(InputManager.BindedKeyPressed(thread.GetStringParameter(0)));
		}

		[NativeFunctionInfo("KeyReleased", "bool", "int")]
		public void KeyReleased(ScriptThread thread)
		{
			thread.SetReturnValue(InputManager.KeyReleased((KeyCodes)thread.GetIntegerParameter(0)));
		}

		[NativeFunctionInfo("BindedKeyReleased", "bool", "string")]
		public void BindedKeyReleased(ScriptThread thread)
		{
			thread.SetReturnValue(InputManager.BindedKeyReleased(thread.GetStringParameter(0)));
		}

		[NativeFunctionInfo("BindKey", "void", "string,int")]
		public void BindKey(ScriptThread thread)
		{
			InputManager.BindKey(thread.GetStringParameter(0),(KeyCodes)thread.GetIntegerParameter(1));
		}

		[NativeFunctionInfo("UnbindKey", "void", "string")]
		public void UnbindKey(ScriptThread thread)
		{
			InputManager.UnbindKey(thread.GetStringParameter(0));
		}

		[NativeFunctionInfo("MouseDesktopX", "int", "")]
		public void MouseDesktopX(ScriptThread thread)
		{
			thread.SetReturnValue(InputManager.MouseX);
		}

		[NativeFunctionInfo("MouseDesktopY", "int", "")]
		public void MouseDesktopY(ScriptThread thread)
		{
			thread.SetReturnValue(InputManager.MouseY);
		}

		[NativeFunctionInfo("MouseScroll", "int", "")]
		public void MouseScroll(ScriptThread thread)
		{
			thread.SetReturnValue(InputManager.MouseScroll);
		}

		[NativeFunctionInfo("MouseX", "int", "")]
		public void MouseX(ScriptThread thread)
		{
			thread.SetReturnValue(InputManager.MouseX);
		}

		[NativeFunctionInfo("MouseY", "int", "")]
		public void MouseY(ScriptThread thread)
		{
			thread.SetReturnValue(InputManager.MouseY);
		}

		[NativeFunctionInfo("MouseDeltaX", "int", "")]
		public void MouseDeltaX(ScriptThread thread)
		{
			thread.SetReturnValue(InputManager.MouseDeltaX);
		}

		[NativeFunctionInfo("MouseDeltaY", "int", "")]
		public void MouseDeltaY(ScriptThread thread)
		{
			thread.SetReturnValue(InputManager.MouseDeltaY);
		}

		[NativeFunctionInfo("MouseDeltaScroll", "int", "")]
		public void MouseDeltaScroll(ScriptThread thread)
		{
			thread.SetReturnValue(InputManager.MouseDeltaScroll);
		}
	}

}