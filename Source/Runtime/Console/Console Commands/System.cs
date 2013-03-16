/* 
 * File: System.cs
 *
 * This source file contains the declaration of several console commands.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Console;
using BinaryPhoenix.Fusion.Runtime.StreamFactorys;

namespace BinaryPhoenix.Fusion.Runtime.Console.ConsoleFunctions
{
    /// <summary>
    ///     Contains the code for several commonly console commands.
    /// </summary>
    public sealed class SystemConsoleCommands : ConsoleCommandSet
    {
        [ConsoleCommandInfo("help", "")]
        public void help(object[] arguments)
        {
            DebugLogger.WriteLog("---------- Help ----------");
            DebugLogger.WriteLog("  Commands can be entered into this console to preform realtime actions within the");
            DebugLogger.WriteLog("  game world. To enter commands simply enter the name of the command followed by a list of");
            DebugLogger.WriteLog("  parameters seperated by a space. eg.");
            DebugLogger.WriteLog("  \tposition_entity 'MyEntity' 0 0 0", LogAlertLevel.Warning);
            DebugLogger.WriteLog("  Parameters that contain spaces can be enclosed in quotation characters so they are parsed as a full argument.");
            DebugLogger.WriteLog("  To view a complete list of usable commands please use the command list_commands.");
        }

        [ConsoleCommandInfo("list_commands", "")]
        public void list_commands(object[] arguments)
        {
            DebugLogger.WriteLog("---------- Command List ----------");
            foreach (ConsoleCommand command in Runtime.Console.Console.Commands)
            {
                string parameters = " ";
                for (int i = 0; i < command.ParameterTypes.Length; i++)
                    parameters += command.ParameterTypes[i].ToString().ToLower() + " ";
                DebugLogger.WriteLog("  " + command.Identifier + parameters);
            }
        }
    }
}
