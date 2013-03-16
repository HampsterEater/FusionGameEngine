/* 
 * File: Language.cs
 *
 * This source file contains the declaration of the LanguageFunctionSet class which 
 * contains numerous native functions that are commonly used in scripts.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Languages;
using BinaryPhoenix.Fusion.Runtime.Scripting;

namespace BinaryPhoenix.Fusion.Runtime.Scripting.ScriptingFunctions
{

	/// <summary>
	///		Contains numerous native functions that are commonly used in scripts.
	///		None are commented as they are all fairly self-explanatory.
	/// </summary>
	public class LanguageFunctionSet : NativeFunctionSet
	{
		[NativeFunctionInfo("LoadLanguage", "void", "string,string")]
		public void LoadLanguagePack(ScriptThread thread)
		{
			LanguageManager.LoadLanguagePack(thread.GetStringParameter(0), thread.GetStringParameter(1));
		}

		[NativeFunctionInfo("LanguageName", "string", "")]
		public void LanguagePackName(ScriptThread thread)
		{
			thread.SetReturnValue(LanguageManager.Name);
		}

		[NativeFunctionInfo("LanguageCaption", "string", "string")]
		public void LanguageCaption(ScriptThread thread)
		{
			thread.SetReturnValue(LanguageManager.GetCaption(thread.GetStringParameter(0)));
		}

		[NativeFunctionInfo("LanguageCaptionExists", "bool", "string")]
		public void LanguageCaptionExists(ScriptThread thread)
		{
			thread.SetReturnValue(LanguageManager.CaptionExists(thread.GetStringParameter(0)));
		}
	}

}