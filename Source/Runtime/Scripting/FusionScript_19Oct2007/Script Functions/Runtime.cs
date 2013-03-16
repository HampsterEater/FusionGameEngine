/* 
 * File: Runtime.cs
 *
 * This source file contains the declaration of the RuntimeFunctionSet class which 
 * contains numerous native functions that are commonly used in scripts.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Runtime.Scripting;

namespace BinaryPhoenix.Fusion.Runtime.Scripting.ScriptingFunctions
{

	/// <summary>
	///		Contains numerous native functions that are commonly used in scripts.
	///		None are commented as they are all fairly self-explanatory.
	/// </summary>
	public class RuntimeFunctionSet : NativeFunctionSet
	{
		#region Members
		#region Variables

		private HighPreformanceTimer _timer = new HighPreformanceTimer();

		#endregion
		#endregion

		[NativeFunctionInfo("ExecuteFile", "void", "string,string,bool")]
		public void ExecuteFile(ScriptThread thread)
		{
			Process process = new Process();
			process.StartInfo.FileName = thread.GetStringParameter(0);
			process.StartInfo.Arguments = thread.GetStringParameter(1);
			process.StartInfo.Verb = "Open";
			process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			process.Start();
			if (thread.GetBooleanParameter(2) == true) process.WaitForExit();
		}

		[NativeFunctionInfo("Milliseconds", "int", "")]
		public void Milliseconds(ScriptThread thread)
		{
			thread.SetReturnValue((int)_timer.DurationMillisecond);
		}

        [NativeFunctionInfo("Length", "int", "byte[]")]
        public void LengthA(ScriptThread thread)
        {
            thread.SetReturnValue(thread.GetArrayLength(thread.GetArrayParameter(0)));
        }

        [NativeFunctionInfo("Length", "int", "bool[]")]
        public void LengthB(ScriptThread thread)
        {
            thread.SetReturnValue(thread.GetArrayLength(thread.GetArrayParameter(0)));
        }

        [NativeFunctionInfo("Length", "int", "short[]")]
        public void LengthC(ScriptThread thread)
        {
            thread.SetReturnValue(thread.GetArrayLength(thread.GetArrayParameter(0)));
        }

        [NativeFunctionInfo("Length", "int", "int[]")]
        public void LengthD(ScriptThread thread)
        {
            thread.SetReturnValue(thread.GetArrayLength(thread.GetArrayParameter(0)));
        }

        [NativeFunctionInfo("Length", "int", "float[]")]
        public void LengthE(ScriptThread thread)
        {
            thread.SetReturnValue(thread.GetArrayLength(thread.GetArrayParameter(0)));
        }

        [NativeFunctionInfo("Length", "int", "double[]")]
        public void LengthF(ScriptThread thread)
        {
            thread.SetReturnValue(thread.GetArrayLength(thread.GetArrayParameter(0)));
        }

        [NativeFunctionInfo("Length", "int", "long[]")]
        public void LengthG(ScriptThread thread)
        {
            thread.SetReturnValue(thread.GetArrayLength(thread.GetArrayParameter(0)));
        }

        [NativeFunctionInfo("Length", "int", "string[]")]
        public void LengthH(ScriptThread thread)
        {
            thread.SetReturnValue(thread.GetArrayLength(thread.GetArrayParameter(0)));
        }

        [NativeFunctionInfo("Length", "int", "object[]")]
        public void LengthI(ScriptThread thread)
        {
            thread.SetReturnValue(thread.GetArrayLength(thread.GetArrayParameter(0)));
        }
	}

}