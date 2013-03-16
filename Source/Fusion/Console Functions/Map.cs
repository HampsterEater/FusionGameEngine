/* 
 * File: Map.cs
 *
 * This source file contains the declaration of several console commands.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Text;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Engine;
using BinaryPhoenix.Fusion.Engine.Debug;
using BinaryPhoenix.Fusion.Runtime.Console;

namespace BinaryPhoenix.Fusion.Engine.ConsoleFunctions
{
    /// <summary>
    ///     Contains the code for several commonly console commands.
    /// </summary>
    public sealed class MapConsoleCommands : ConsoleCommandSet
    {
        [ConsoleCommandInfo("trigger_map_event", "string")]
        public void trigger_map_event(object[] arguments)
        {
            if (Fusion.GlobalInstance.MapScriptProcess != null)
            {
                DebugLogger.WriteLog("Map event '" + arguments[0].ToString() + "' was triggered.");
                Fusion.GlobalInstance.MapScriptProcess.Process[0].InvokeFunction(arguments[0].ToString(), false, false);
            }
        }
    }
}
