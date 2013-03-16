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
using BinaryPhoenix.Fusion.Engine;
using BinaryPhoenix.Fusion.Engine.Debug;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Console;
using BinaryPhoenix.Fusion.Runtime.StreamFactorys;

namespace BinaryPhoenix.Fusion.Engine.ConsoleFunctions
{
    /// <summary>
    ///     Contains the code for several commonly console commands.
    /// </summary>
    public sealed class SystemConsoleCommands : ConsoleCommandSet
    {
        [ConsoleCommandInfo("toggle_fullscreen", "")]
        public void toggle_fullscreen(object[] arguments)
        {
            Fusion.GlobalInstance.Fullscreen = !Fusion.GlobalInstance.Fullscreen;
        }

        [ConsoleCommandInfo("resize_window", "int,int")]
        public void resize_window(object[] arguments)
        {
            Fusion.GlobalInstance.Window.ClientSize = new System.Drawing.Size((int)arguments[0], (int)arguments[1]);
        }

        [ConsoleCommandInfo("resize_resolution", "int,int,bool")]
        public void resize_resolution(object[] arguments)
        {
            Fusion.GlobalInstance.Window.Reset((int)arguments[0], (int)arguments[1], ((bool)arguments[0]) == false ? 0 : Graphics.GraphicsFlags.FullScreen);
        }

        [ConsoleCommandInfo("end", "")]
        public void End(object[] arguments)
        {
            Fusion.GlobalInstance.ClosingDown = true;
        }

        [ConsoleCommandInfo("clear", "")]
        public void clear(object[] arguments)
        {
            GraphicalConsole.Clear();
        }
        
        [ConsoleCommandInfo("collect_garbage", "")]
        public void collect_garbage(object[] arguments)
        {
            GC.Collect();
        }
    }
}
