/* 
 * File: Statistical.cs
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
    public sealed class StatisticalConsoleCommands : ConsoleCommandSet
    {
        [ConsoleCommandInfo("toggle_fps", "")]
        public void toggle_fps(object[] arguments)
        {
            GraphicalConsole.ShowFPS = !GraphicalConsole.ShowFPS;
            DebugLogger.WriteLog(GraphicalConsole.ShowFPS ? "Fps display is now visible" : "Fps display is now invisible");
        }

        [ConsoleCommandInfo("limit_fps", "int")]
        public void limit_fps(object[] arguments)
        {
            Fusion.GlobalInstance.FPSLimit = (int)arguments[0];
            DebugLogger.WriteLog(Fusion.GlobalInstance.FPSLimit == 0 ? "Fps is now unlimited" : "Fps is now limited to " + Fusion.GlobalInstance.FPSLimit);
        }

        [ConsoleCommandInfo("toggle_preformance_statistics", "")]
        public void toggle_preformance_statistics(object[] arguments)
        {
            GraphicalConsole.ShowPreformanceStats = !GraphicalConsole.ShowPreformanceStats;
            DebugLogger.WriteLog(GraphicalConsole.ShowPreformanceStats ? "Preformance statistics display is now visible" : "Preformance statistics display is now invisible");
        }

        [ConsoleCommandInfo("toggle_delta", "")]
        public void toggle_delta(object[] arguments)
        {
            GraphicalConsole.ShowDeltaTime = !GraphicalConsole.ShowDeltaTime;
            DebugLogger.WriteLog(GraphicalConsole.ShowDeltaTime ? "Delta time display is now visible" : "Delta time display is now invisible");
        }

        [ConsoleCommandInfo("toggle_delta_graph", "")]
        public void toggle_delta_graph(object[] arguments)
        {
            GraphicalConsole.ShowDeltaGraph = !GraphicalConsole.ShowDeltaGraph;
            DebugLogger.WriteLog(GraphicalConsole.ShowDeltaGraph ? "Delta time graph display is now visible" : "Delta time graph display is now invisible");
        }

        [ConsoleCommandInfo("toggle_timing", "")]
        public void toggle_timing(object[] arguments)
        {
            GraphicalConsole.ShowTiming = !GraphicalConsole.ShowTiming;
            DebugLogger.WriteLog(GraphicalConsole.ShowTiming ? "Timing display is now visible" : "Timing display is now invisible");
        }

        [ConsoleCommandInfo("toggle_statistics", "")]
        public void toggle_statistics(object[] arguments)
        {
            GraphicalConsole.ShowStats = !GraphicalConsole.ShowStats;
            DebugLogger.WriteLog(GraphicalConsole.ShowStats ? "Statistics display is now visible" : "Statistics display is now invisible");
        }
    }
}
