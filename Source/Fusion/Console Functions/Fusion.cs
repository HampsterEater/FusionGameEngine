/* 
 * File: Fusion.cs
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
    public sealed class FusionConsoleCommands : ConsoleCommandSet
    {
        [ConsoleCommandInfo("set_game_flag", "string,string")]
        public void set_game_flag(object[] arguments)
        {
            string key = arguments[0].ToString();
            string value = arguments[1].ToString();
            if (Fusion.GlobalInstance.GameFlags.Contains(key))
                Fusion.GlobalInstance.GameFlags.Remove(key);
            DebugLogger.WriteLog("Game flag '" + key + "' was set to '" + value + "'.");
            Fusion.GlobalInstance.GameFlags.Add(key, value);
        }

        [ConsoleCommandInfo("delete_game_flag", "string")]
        public void delete_game_flag(object[] arguments)
        {
            string key = arguments[0].ToString();
            if (Fusion.GlobalInstance.GameFlags.Contains(key))
                Fusion.GlobalInstance.GameFlags.Remove(key);
        }

        [ConsoleCommandInfo("get_game_flag", "string")]
        public void get_game_flag(object[] arguments)
        {
            string key = arguments[0].ToString();
            if (!Fusion.GlobalInstance.GameFlags.Contains(key))
                DebugLogger.WriteLog("Game flag '" + key + "' dosen't exist.");
            else
                DebugLogger.WriteLog("Game flag '" + key + "' is set to '" + ((string)Fusion.GlobalInstance.GameFlags[key]) + "'.");
        }

        [ConsoleCommandInfo("list_game_flags", "")]
        public void list_game_flags(object[] arguments)
        {
            if (Fusion.GlobalInstance.GameFlags.Count == 0)
            {
                DebugLogger.WriteLog("No flags are currently set.");
                return;
            }
            foreach (object key in Fusion.GlobalInstance.GameFlags.Keys)
                DebugLogger.WriteLog("Game flag '" + key.ToString() + "' is set to '" + ((string)Fusion.GlobalInstance.GameFlags[key]) + "'.");
        }
    }
}
