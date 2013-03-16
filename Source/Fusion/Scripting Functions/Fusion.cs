/* 
 * File: Fusion.cs
 *
 * This source file contains the declaration of the FusionFunctionSet class which 
 * contains the few native functions that are implemented directly in the Fusion exe
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using BinaryPhoenix.Fusion.Graphics;
using BinaryPhoenix.Fusion.Engine.Entitys;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Runtime.Scripting;

namespace BinaryPhoenix.Fusion.Engine.ScriptingFunctions
{
    /// <summary>
    ///		Contains numerous native functions that are commonly used in scripts.
    ///		None are commented as they are all fairly self-explanatory.
    /// </summary>
    public class FusionFunctionSet : NativeFunctionSet
    {
        [NativeFunctionInfo("SetPostProcessingShader", "void", "object")]
        public void SetPostProcessingShader(ScriptThread thread)
        {
            Shader shader = ((NativeObject)thread.GetObjectParameter(0)).Object as Shader;
            if (shader == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetPostProcessingShader with an invalid object.", LogAlertLevel.Error);
                return;
            }
            Fusion.GlobalInstance.Window.PostProcessingShader = shader;
        }

        [NativeFunctionInfo("PostProcessingShader", "object", "")]
        public void PostProcessingShader(ScriptThread thread)
        {
            thread.SetReturnValue(new ShaderScriptObject(Fusion.GlobalInstance.Window.PostProcessingShader));
        }

        [NativeFunctionInfo("TriggerMapEvent", "void", "string")]
        public void TriggerMapEvent(ScriptThread thread)
        {
            if (Fusion.GlobalInstance.MapScriptProcess != null)
            {
                DebugLogger.WriteLog("Map event '" + thread.GetStringParameter(0) + "' was triggered.");
                Fusion.GlobalInstance.MapScriptProcess.Process[0].InvokeFunction(thread.GetStringParameter(0), false, false);
            }
        }

        [NativeFunctionInfo("SetGameFlag", "void", "string,string")]
        public void SetGameFlag(ScriptThread thread)
        {
            string key = thread.GetStringParameter(0).ToLower();
            string value = thread.GetStringParameter(1);
            if (Fusion.GlobalInstance.GameFlags.Contains(key))
                Fusion.GlobalInstance.GameFlags.Remove(key);
            Fusion.GlobalInstance.GameFlags.Add(key, value);
        }

        [NativeFunctionInfo("GameFlag", "string", "string")]
        public void GameFlag(ScriptThread thread)
        {
            string key = thread.GetStringParameter(0).ToLower();
            if (!Fusion.GlobalInstance.GameFlags.Contains(key))
                return;
            thread.SetReturnValue((string)Fusion.GlobalInstance.GameFlags[key]);
        }

        [NativeFunctionInfo("IsGameFlagSet", "bool", "string")]
        public void IsGameFlagSet(ScriptThread thread)
        {
            string key = thread.GetStringParameter(0).ToLower();
            thread.SetReturnValue(Fusion.GlobalInstance.GameFlags.Contains(key));
        }

        [NativeFunctionInfo("GameFlagCount", "int", "")]
        public void GameFlagCount(ScriptThread thread)
        {
            thread.SetReturnValue(Fusion.GlobalInstance.GameFlags.Count);
        }

        [NativeFunctionInfo("GameFlagValueAtIndex", "string", "int")]
        public void GameFlagValueAtIndex(ScriptThread thread)
        {
            int index = thread.GetIntegerParameter(0);
            int currentIndex = 0;
            foreach (string value in Fusion.GlobalInstance.GameFlags.Values)
            {
                if (index == currentIndex)
                {
                    thread.SetReturnValue(value);
                    return;
                }
                currentIndex++;
            }
        }

        [NativeFunctionInfo("GameFlagNameAtIndex", "string", "int")]
        public void GameFlagNameAtIndex(ScriptThread thread)
        {
            int index = thread.GetIntegerParameter(0);
            int currentIndex = 0;
            foreach (string value in Fusion.GlobalInstance.GameFlags.Keys)
            {
                if (index == currentIndex)
                {
                    thread.SetReturnValue(value);
                    return;
                }
                currentIndex++;
            }
        }

        [NativeFunctionInfo("ClearGameFlags", "void", "")]
        public void ClearGameFlags(ScriptThread thread)
        {
            Fusion.GlobalInstance.GameFlags.Clear();
        }

        [NativeFunctionInfo("UniqueEntityFlag", "string", "object")]
        public void UniqueEntityFlag(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called UniqueEntityFlag with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(("quest:maps:["+Fusion.GlobalInstance.Map.URL + "]:Entity" + entity.UniqueID).ToLower());
        }

        [NativeFunctionInfo("SetSceneGraphRender", "void", "bool")]
        public void SetSceneGraphRender(ScriptThread thread)
        {
            Fusion.GlobalInstance.Window.RenderSceneGraph = thread.GetBooleanParameter(0);
        }

        [NativeFunctionInfo("SceneGraphRender", "bool", "")]
        public void SceneGraphRender(ScriptThread thread)
        {
            thread.SetReturnValue(Fusion.GlobalInstance.Window.RenderSceneGraph);
        }
    }
}