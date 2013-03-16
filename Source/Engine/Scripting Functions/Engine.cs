/* 
 * File: Engine.cs
 *
 * This source file contains the declaration of the EngineFunctionSet class which 
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

namespace BinaryPhoenix.Fusion.Engine.ScriptingFunctions
{

	/// <summary>
	///		Contains numerous native functions that are commonly used in scripts.
	///		None are commented as they are all fairly self-explanatory.
	/// </summary>
	public class EngineFunctionSet : NativeFunctionSet
	{

		[NativeFunctionInfo("EndGame", "void", "")]
		public void EndGame(ScriptThread thread)
		{
            Engine.GlobalInstance.ClosingDown = true;
		}

		[NativeFunctionInfo("CommandLineExists", "bool", "string")]
		public void CommandLineExists(ScriptThread thread)
		{
			string commandLine = thread.GetStringParameter(0);

			foreach (string arg in Engine.GlobalInstance.CommandLineArguments)
			{
				string[] value = new string[0];
				string command = arg;
				int colonIndex = arg.IndexOf(':');

				// Seperate values and command if a colon exists.
				if (colonIndex >= 0)
				{
					value = new string[1];
					value[0] = arg.Substring(colonIndex + 1, arg.Length - colonIndex - 1);
					if (value[0].IndexOf(",") >= 0) value = value[0].Split(new char[1] { ',' });
					command = arg.Substring(0, colonIndex);
				}

				if (command.ToLower() == commandLine.ToLower())
				{
					thread.SetReturnValue(true);
					return;
				}
			}

			thread.SetReturnValue(false);
		}

		[NativeFunctionInfo("CommandLineValueCount", "int", "string")]
		public void CommandLineValueCount(ScriptThread thread)
		{
			string commandLine = thread.GetStringParameter(0);

			foreach (string arg in Engine.GlobalInstance.CommandLineArguments)
			{
				string[] value = new string[0];
				string command = arg;
				int colonIndex = arg.IndexOf(':');

				// Seperate values and command if a colon exists.
				if (colonIndex >= 0)
				{
					value = new string[1];
					value[0] = arg.Substring(colonIndex + 1, arg.Length - colonIndex - 1);
					if (value[0].IndexOf(",") >= 0) value = value[0].Split(new char[1] { ',' });
					command = arg.Substring(0, colonIndex);
				}

				if (command.ToLower() == commandLine.ToLower())
				{
					thread.SetReturnValue(value.Length);
					return;
				}
			}

			DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called CommandLineValue with a non-existant command line.", LogAlertLevel.Error);
		}

		[NativeFunctionInfo("CommandLineValue", "string", "string,int")]
		public void CommandLineValue(ScriptThread thread)
		{
			string commandLine = thread.GetStringParameter(0);
			int valueIndex = thread.GetIntegerParameter(1);

			foreach (string arg in Engine.GlobalInstance.CommandLineArguments)
			{
				string[] value = new string[0];
				string command = arg;
				int colonIndex = arg.IndexOf(':');

				// Seperate values and command if a colon exists.
				if (colonIndex >= 0)
				{
					value = new string[1];
					value[0] = arg.Substring(colonIndex + 1, arg.Length - colonIndex - 1);
					if (value[0].IndexOf(",") >= 0) value = value[0].Split(new char[1] { ',' });
					command = arg.Substring(0, colonIndex);
				}

				if (command.ToLower() == commandLine.ToLower())
				{
					if (valueIndex < 0 || valueIndex >= value.Length)
					{
						DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called CommandLineValue with an invalid value index.", LogAlertLevel.Error);
						return;
					}
					thread.SetReturnValue(value[valueIndex]);
					return;
				}
			}

			DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called CommandLineValue with a non-existant command line.", LogAlertLevel.Error);
		}

		[NativeFunctionInfo("EngineVariable", "string", "string")]
		public void EngineVariable(ScriptThread thread)
		{
			string value = "";
			switch (thread.GetStringParameter(0).ToLower())
			{
				case "audiopath": value = Engine.GlobalInstance.AudioPath; break;
				case "basepath": value = Engine.GlobalInstance.BasePath; break;
				case "buildpath": value = Engine.GlobalInstance.BuildPath; break;
				case "configpath": value = Engine.GlobalInstance.ConfigPath; break;
				case "enginepath": value = Engine.GlobalInstance.EnginePath; break;
				case "fontpath": value = Engine.GlobalInstance.FontPath; break;
				case "gamename": value = Engine.GlobalInstance.GameName; break;
				case "gamepath": value = Engine.GlobalInstance.GamePath; break;
				case "graphicpath": value = Engine.GlobalInstance.GraphicPath; break;
				case "languagepath": value = Engine.GlobalInstance.LanguagePath; break;
				case "mappath": value = Engine.GlobalInstance.MapPath; break;
				case "mediapath": value = Engine.GlobalInstance.MediaPath; break;
				case "objectpath": value = Engine.GlobalInstance.ObjectPath; break;
				case "pluginpath": value = Engine.GlobalInstance.PluginPath; break;
				case "savepath": value = Engine.GlobalInstance.SavePath; break;
				case "scriptlibrarypath": value = Engine.GlobalInstance.ScriptLibraryPath; break;
				case "scriptpath": value = Engine.GlobalInstance.ScriptPath; break;
				case "tilesetpath": value = Engine.GlobalInstance.TilesetPath; break;
                case "effectpath": value = Engine.GlobalInstance.EffectPath; break;
                case "shaderpath": value = Engine.GlobalInstance.ShaderPath; break;
            }
			thread.SetReturnValue(value);
		}

		[NativeFunctionInfo("DeltaTime", "int", "")]
		public void DeltaTime(ScriptThread thread)
		{
			thread.SetReturnValue(Engine.GlobalInstance.DeltaTime);
		}
	}

}