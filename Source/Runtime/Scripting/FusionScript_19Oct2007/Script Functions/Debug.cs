/* 
 * File: Debug.cs
 *
 * This source file contains the declaration of the DebugFunctionSet class which 
 * contains numerous native functions that are commonly used in scripts.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Runtime.Processes;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Scripting;
using BinaryPhoenix.Fusion.Runtime.Debug;

namespace BinaryPhoenix.Fusion.Runtime.Scripting.ScriptingFunctions
{

	/// <summary>
	///		Contains numerous native functions that are commonly used in scripts.
	///		None are commented as they are all fairly self-explanatory.
	/// </summary>
	public class DebugFunctionSet : NativeFunctionSet
	{
		[NativeFunctionInfo("DebugLog", "void", "string,int")]
		public void DebugLogA(ScriptThread thread)
		{
			DebugLogger.WriteLog(thread.GetStringParameter(0), (LogAlertLevel)thread.GetIntegerParameter(1));
		}

		[NativeFunctionInfo("DebugLog", "void", "string")]
		public void DebugLogB(ScriptThread thread)
		{
			DebugLogger.WriteLog(thread.GetStringParameter(0));
		}
	}

}