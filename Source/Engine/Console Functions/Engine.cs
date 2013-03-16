/* 
 * File: Engine.cs
 *
 * This source file contains the declaration of several console commands.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using BinaryPhoenix.Fusion.Runtime.Resources;
using BinaryPhoenix.Fusion.Engine;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Engine.Entitys;
using BinaryPhoenix.Fusion.Runtime.Console;

namespace BinaryPhoenix.Fusion.Engine.ConsoleFunctions
{
    /// <summary>
    ///     Contains the code for several commonly used engine console commands.
    /// </summary>
    public sealed class EngineConsoleCommands : ConsoleCommandSet
    {
        [ConsoleCommandInfo("execute", "string")]
        public void execute(object[] arguments)
        {
            string file = arguments[0].ToString();

            if (!File.Exists(file))
            {
                file = Engine.GlobalInstance.EnginePath + "\\" + file;
                if (!File.Exists(file))
                {
                    DebugLogger.WriteLog("File could not be found.", LogAlertLevel.Warning);
                    return;
                }
            }

            string[] commands = File.ReadAllLines(file);
            for (int i = 0; i < commands.Length; i++)
                Runtime.Console.Console.ProcessCommand(commands[i]);
        }

        [ConsoleCommandInfo("toggle_delta_timing", "")]
        public void toggle_delta_timing(object[] arguments)
        {
            Engine.GlobalInstance.TrackDeltaTime = !Engine.GlobalInstance.TrackDeltaTime;
            DebugLogger.WriteLog("Delta time is now " + (Engine.GlobalInstance.TrackDeltaTime ? "on." : "off."));
        }

        [ConsoleCommandInfo("set_delta_scale", "float")]
        public void set_delta_scale(object[] arguments)
        {
            Engine.GlobalInstance.DeltaScale = (float)arguments[0];
            DebugLogger.WriteLog("Delta scale is now " + Engine.GlobalInstance.DeltaScale + ".");
        }

        [ConsoleCommandInfo("load_map", "string")]
        public void load_Map(object[] arguments)
        {
            if (ResourceManager.ResourceExists((string)arguments[0]))
                Engine.GlobalInstance.LoadMap((string)arguments[0], "");
            else if (ResourceManager.ResourceExists(Engine.GlobalInstance.MapPath + "/" + (string)arguments[0]))
                Engine.GlobalInstance.LoadMap(Engine.GlobalInstance.MapPath + "/" + (string)arguments[0], "");
            else
                DebugLogger.WriteLog("Unable to find map file.", LogAlertLevel.Warning);
        }

        [ConsoleCommandInfo("load_passworded_map", "string,string")]
        public void load_passworded_map(object[] arguments)
        {
            if (File.Exists((string)arguments[0]))
                Engine.GlobalInstance.LoadMap((string)arguments[0], (string)arguments[1]);
            else if (File.Exists(Engine.GlobalInstance.MapPath + "/" + (string)arguments[0]))
                Engine.GlobalInstance.LoadMap(Engine.GlobalInstance.MapPath + "/" + (string)arguments[0], (string)arguments[1]);
            else
                DebugLogger.WriteLog("Unable to find map file.", LogAlertLevel.Warning);
        }


    }
}
