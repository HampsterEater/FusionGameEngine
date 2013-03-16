/* 
 * File: Thread.cs
 *
 * This source file contains the declaration of the ThreadFunctionSet class which 
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
    ///     Wraps a script thread object used in a script.
    /// </summary>
    public class ThreadScriptObject : NativeObject
    {
        public ThreadScriptObject(ScriptThread thread)
        {
            _nativeObject = thread;
        }

        public ThreadScriptObject() { }
    }

	/// <summary>
	///		Contains numerous native functions that are commonly used in scripts.
	///		None are commented as they are all fairly self-explanatory.
	/// </summary>
	public class ThreadFunctionSet : NativeFunctionSet
	{
		[NativeFunctionInfo("ExecutingThread", "object", "")]
		public void ExecutingThread(ScriptThread thread)
		{
            thread.SetReturnValue(new ThreadScriptObject(thread));
		}

		[NativeFunctionInfo("Sleep", "void", "int")]
		public void Sleep(ScriptThread thread)
		{
			thread.Pause(thread.GetIntegerParameter(0));
		}

		[NativeFunctionInfo("PauseThread", "void", "object,int")]
		public void PauseThread(ScriptThread thread)
		{
			ScriptThread actionThread = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptThread;
			if (actionThread == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called PauseThread with an invalid object.", LogAlertLevel.Error);
				return;
			}
			actionThread.Pause(thread.GetIntegerParameter(0));
		}

		[NativeFunctionInfo("StopThread", "void", "object")]
		public void StopThread(ScriptThread thread)
		{
            ScriptThread actionThread = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptThread;
			if (actionThread == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StopThread with an invalid object.", LogAlertLevel.Error);
				return;
			}
			actionThread.Stop();
		}

		[NativeFunctionInfo("ResumeThread", "void", "object")]
		public void ResumeThread(ScriptThread thread)
		{
            ScriptThread actionThread = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptThread;
			if (actionThread == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ResumeThread with an invalid object.", LogAlertLevel.Error);
				return;
			}
			actionThread.Resume();
		}

		[NativeFunctionInfo("InvokeThreadFunction", "void", "object,string,bool,bool")]
		public void InvokeThreadFunction(ScriptThread thread)
		{
            ScriptThread actionThread = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptThread;
			if (actionThread == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called InvokeThreadFunction with an invalid object.", LogAlertLevel.Error);
				return;
			}
			actionThread.InvokeFunction(thread.GetStringParameter(1), thread.GetBooleanParameter(2), thread.GetBooleanParameter(3), false);
		}
	}

}