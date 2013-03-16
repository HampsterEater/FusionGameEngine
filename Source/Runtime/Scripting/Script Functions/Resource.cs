/* 
 * File: Resource.cs
 *
 * This source file contains the declaration of the ResourceFunctionSet class which 
 * contains numerous native functions that are commonly used in scripts.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Resources;
using BinaryPhoenix.Fusion.Runtime.Scripting;

namespace BinaryPhoenix.Fusion.Runtime.Scripting.ScriptingFunctions
{

	/// <summary>
	///		Contains numerous native functions that are commonly used in scripts.
	///		None are commented as they are all fairly self-explanatory.
	/// </summary>
	public class ResourceFunctionSet : NativeFunctionSet
	{
		[NativeFunctionInfo("ResourceExists", "bool", "string")]
		public void ResourceExists(ScriptThread thread)
		{
			thread.SetReturnValue(ResourceManager.ResourceExists(thread.GetStringParameter(0)));
		}
	}

}