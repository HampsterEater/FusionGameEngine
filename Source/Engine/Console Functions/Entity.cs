/* 
 * File: Entity.cs
 *
 * This source file contains the declaration of several console commands.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Text;
using BinaryPhoenix.Fusion.Engine;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Engine.Entitys;
using BinaryPhoenix.Fusion.Runtime.Console;

namespace BinaryPhoenix.Fusion.Engine.ConsoleFunctions
{
    /// <summary>
    ///     Contains the code for several commonly used stastical console commands.
    /// </summary>
    public sealed class EntityConsoleCommands : ConsoleCommandSet
    {
        [ConsoleCommandInfo("toggle_bounding_boxs", "")]
        public void toggle_bounding_boxs(object[] arguments)
        {
            EntityNode.ForceGlobalBoundingBoxVisibility = !EntityNode.ForceGlobalBoundingBoxVisibility;
            DebugLogger.WriteLog(EntityNode.ForceGlobalBoundingBoxVisibility ? "Bounding boxs are now visible" : "Bounding boxs are now invisible");
        }

        [ConsoleCommandInfo("toggle_collision_boxs", "")]
        public void toggle_collision_boxs(object[] arguments)
        {
            EntityNode.ForceGlobalCollisionBoxVisibility = !EntityNode.ForceGlobalCollisionBoxVisibility;
            DebugLogger.WriteLog(EntityNode.ForceGlobalCollisionBoxVisibility ? "Collision boxs are now visible" : "Collision boxs are now invisible");
        }

        [ConsoleCommandInfo("toggle_debug_information", "")]
        public void toggle_debug_information(object[] arguments)
        {
            EntityNode.ForceGlobalDebugVisibility = !EntityNode.ForceGlobalDebugVisibility;
            DebugLogger.WriteLog(EntityNode.ForceGlobalDebugVisibility ? "Entity debugging information is now visible" : "Entity debugging information is now invisible");
        }

        [ConsoleCommandInfo("toggle_event_lines", "")]
        public void toggle_event_lines(object[] arguments)
        {
            EntityNode.ForceGlobalEventLineVisibility = !EntityNode.ForceGlobalEventLineVisibility;
            DebugLogger.WriteLog(EntityNode.ForceGlobalEventLineVisibility ? "Entity event lines are now visible" : "Entityevent lines area now invisible");
        }

        [ConsoleCommandInfo("move_entity", "")]
        public void move_entity(object[] arguments)
        {
        }
    }
}
