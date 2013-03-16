/* 
 * File: Map.cs
 *
 * This source file contains the declaration of the MapFunctionSet class which 
 * contains numerous native functions that are commonly used in scripts.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Collision;
using BinaryPhoenix.Fusion.Runtime.Scripting;
using BinaryPhoenix.Fusion.Runtime.Debug;

namespace BinaryPhoenix.Fusion.Engine.ScriptingFunctions
{

    /// <summary>
    ///		Contains numerous native functions that are commonly used in scripts.
    ///		None are commented as they are all fairly self-explanatory.
    /// </summary>
    public class MapFunctionSet : NativeFunctionSet
    {

        [NativeFunctionInfo("LoadMap", "void", "string,string")]
        public void LoadMapA(ScriptThread thread)
        {
            Engine.GlobalInstance.LoadMap(thread.GetStringParameter(0), thread.GetStringParameter(1));
            thread.Pause(0); // We just want to stop running until the map is loaded - This is a hackish way of doing that.
        }

        [NativeFunctionInfo("LoadMap", "void", "string")]
        public void LoadMapB(ScriptThread thread)
        {
            Engine.GlobalInstance.LoadMap(thread.GetStringParameter(0), "");
            thread.Pause(0); // We just want to stop running until the map is loaded - This is a hackish way of doing that.
        }

        [NativeFunctionInfo("CheckForCollision", "bool", "float,float")]
        public void CheckForCollisionA(ScriptThread thread)
        {
            thread.SetReturnValue(CollisionManager.HitTest(new CollisionRectangle(new Transformation(thread.GetFloatParameter(0), thread.GetFloatParameter(1), 0, 0, 0, 0, 1.0f, 1.0f, 1.0f), 1, 1)));
        }

        [NativeFunctionInfo("CheckForCollision", "bool", "float,float,float,float")]
        public void CheckForCollisionB(ScriptThread thread)
        {
            thread.SetReturnValue(CollisionManager.HitTest(new CollisionRectangle(new Transformation(thread.GetFloatParameter(0), thread.GetFloatParameter(1), 0, 0, 0, 0, 1.0f, 1.0f, 1.0f), thread.GetFloatParameter(2), thread.GetFloatParameter(3))));
        }

        [NativeFunctionInfo("NodeAtPoint", "object", "float,float")]
        public void NodeAtPoint(ScriptThread thread)
        {
            CollisionPolygon polygon = CollisionManager.PolygonAtPoint((int)thread.GetFloatParameter(0), (int)thread.GetFloatParameter(1));
            if (polygon == null || polygon.MetaData == null) return;
            thread.SetReturnValue(new SceneNodeScriptObject(polygon.MetaData as SceneNode));
        }

        [NativeFunctionInfo("DisposeOfMap", "void", "")]
        public void DisposeOfMap(ScriptThread thread)
        {
            ArrayList list = Engine.GlobalInstance.Map.SceneGraph.EnumerateNodes();
            for (int i = 0; i < list.Count; i++)
            {
                ((SceneNode)list[i]).ClearChildren();
                ((SceneNode)list[i]).Dispose();
            }

            Engine.GlobalInstance.Map.SceneGraph.RootNode = new SceneNode("Root Node");
            BinaryPhoenix.Fusion.Engine.Entitys.Tileset.TilesetPool.Clear();
        }

    }

}