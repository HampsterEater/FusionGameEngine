/* 
 * File: Engine.cs
 *
 * This source file contains the declaration of the EntityFunctionSet class which 
 * contains numerous native functions that are commonly used in scripts.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Graphics;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Engine.Processes;
using BinaryPhoenix.Fusion.Runtime.Collision;
using BinaryPhoenix.Fusion.Runtime.Scripting;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Engine.Entitys;

namespace BinaryPhoenix.Fusion.Engine.ScriptingFunctions
{

    /// <summary>
    ///     Wraps a scene node object used in a script.
    /// </summary>
    public class SceneNodeScriptObject : NativeObject
    {

        /// <summary>
        ///     Converts this object to a textural form.
        /// </summary>
        /// <returns>Textural form of this object.</returns>
        public override string ToString()
        {
            return _nativeObject != null ? _nativeObject.ToString() : base.ToString();
        }

        /// <summary>
        ///     Invoked when a script wants to call a method of this object.
        /// </summary>
        /// <param name="thread">Script thread that invoked this function.</param>
        /// <param name="method">Symbol describing the method that it wants called.</param>
        /// <returns>True if successfull or false if not.</returns>
        public override bool InvokeMethod(ScriptThread thread, FunctionSymbol method)
        {
            if (_nativeObject is ScriptedEntityNode)
            {
                ScriptProcess process = ((ScriptedEntityNode)_nativeObject).ScriptProcess;
                if (process == null) 
                    return false;

                FunctionSymbol func = (FunctionSymbol)process.MemberFunctionHashTable[method.ToString().ToLower()];
                if (func != null)
                {
                    // Call the method.
                    process[0].InvokeExportFunction(func, process[0], thread);
                    return true;
                }

                /*
                foreach (Symbol symbol in process.GlobalScope.Symbols)
                {
                    if (symbol != null && symbol.Identifier.ToLower() == method.Identifier.ToLower() && symbol.Type == SymbolType.Function && ((FunctionSymbol)symbol).ParameterCount == method.ParameterCount && ((FunctionSymbol)symbol).AccessModifier == SymbolAccessModifier.Public)
                    {
                        FunctionSymbol functionSymbol = symbol as FunctionSymbol;
                        bool parametersValid = true;
                        for (int i = 0; i < functionSymbol.ParameterCount; i++)
                            if (((VariableSymbol)symbol.Symbols[i]).DataType != ((VariableSymbol)method.Symbols[i]).DataType)
                            {
                                parametersValid = false;
                                break;
                            }

                        if (parametersValid == true)
                        {
                            // Call the method.
                            process[0].InvokeExportFunction(functionSymbol, process[0], thread);
                            return true;
                        }
                    }
                }
                */
                DebugLogger.WriteLog("Unable to find method '"+method.Identifier+"'.", LogAlertLevel.Warning);
            }
            else if (_nativeObject is EntityNode)
            {
                //EntityNode entityNode = _nativeObject as EntityNode;
                
            }
            else if (_nativeObject is EmitterNode)
            {

            }
            else if (_nativeObject is TilemapSegmentNode)
            {

            }
            else if ((_nativeObject as SceneNode) != null)
            {

            }

            return false;
        }

        /// <summary>
        ///     Invoked when a script wants to set the value of a member of this object.
        /// </summary>
        /// <param name="thread">Script thread that invoked this function.</param>
        /// <param name="variable">Symbol describing the member that it wants set.</param>
        /// <param name="value">Value it wants the member to be set to.</param>
        /// <returns>True if successfull or false if not.</returns>
        public override bool GetMember(ScriptThread thread, VariableSymbol variable)
        {
            if (_nativeObject is ScriptedEntityNode)
            {
                // Check the script process is valid.
                ScriptProcess process = ((ScriptedEntityNode)_nativeObject).ScriptProcess;
                if (process == null) return false;

                // See if we can get the symbol they are after.
                VariableSymbol processSymbol = process.GetSymbol(variable.Identifier, SymbolType.Variable) as VariableSymbol;
                if (processSymbol == null)
                    return false;

                // Grab the variable runtime value. (This can probably be optimized).
                RuntimeValue runtimeValue= process[0].GetRuntimeValueGlobal(processSymbol.Identifier);

                // Check its public.
                if (processSymbol.AccessModifier != SymbolAccessModifier.Public)
                    return false;

                // Check data types are correct.
                if (variable.DataType != processSymbol.DataType)
                    return false;

                // Copy value into calling thread.
                if (variable.DataType.IsArray == true)
                {
                    thread.Registers[(int)Register.Return].MemoryIndex = (short)thread.CopyValueArrayFromThread(runtimeValue, thread, process[0]);
                }
                else
                    thread.Registers[(int)Register.Return] = thread.CopyValueFromThread(runtimeValue, thread, process[0]);

                // Set the return register to our newly copied value.

                // Return.
                return true;
            }
            else if (_nativeObject is EntityNode)
            {

            }
            else if (_nativeObject is EmitterNode)
            {

            }
            else if (_nativeObject is TilemapSegmentNode)
            {

            }
            else if ((_nativeObject as SceneNode) != null)
            {

            }

            return false;
        }

        /// <summary>
        ///     Invoked when a script wants to set the value of an indexed member of this object.
        /// </summary>
        /// <param name="thread">Script thread that invoked this function.</param>
        /// <param name="variable">Symbol describing the member that it wants set.</param>
        /// <param name="value">Value it wants the member to be set to.</param>
        /// <param name="index">Index of member to it wants set.</param>
        /// <returns>True if successfull or false if not.</returns>
        public override bool GetMember(ScriptThread thread, VariableSymbol variable, RuntimeValue indexer)
        {
            if (_nativeObject is ScriptedEntityNode)
            {
                // Check the script process is valid.
                ScriptProcess process = ((ScriptedEntityNode)_nativeObject).ScriptProcess;
                if (process == null) return false;

                // See if we can get the symbol they are after.
                VariableSymbol processSymbol = process.GetSymbol(variable.Identifier, SymbolType.Variable) as VariableSymbol;
                if (processSymbol == null)
                    return false;

                // Grab the variable runtime value. (This can probably be optimized).
                RuntimeValue runtimeValue = process[0].GetRuntimeValueGlobal(processSymbol.Identifier);

                // Check its public.
                if (processSymbol.AccessModifier != SymbolAccessModifier.Public)
                    return false;

                // Check data types are correct.
                if (variable.DataType.DataType != processSymbol.DataType.DataType || processSymbol.DataType.IsArray != true)
                    return false;

                // Grab the arrays variable.
                RuntimeValue indexValue = process[0].GetRuntimeValueArrayElement(runtimeValue.MemoryIndex, indexer.IntegerLiteral);

                // Copy value into calling thread.
                RuntimeValue copiedValue = thread.CopyValueFromThread(indexValue, thread, process[0]);

                // Set the return register to our newly copied value.
                thread.Registers[(int)Register.Return] = copiedValue;

                // Return.
                return true;
            }
            else if (_nativeObject is EntityNode)
            {

            }
            else if (_nativeObject is EmitterNode)
            {

            }
            else if (_nativeObject is TilemapSegmentNode)
            {

            }
            else if ((_nativeObject as SceneNode) != null)
            {

            }

            return false;
        }

        /// <summary>
        ///     Invoked when a script wants to get the value of a member of this object.
        /// </summary>
        /// <param name="thread">Script thread that invoked this function.</param>
        /// <param name="variable">Symbol describing the member that it wants get.</param>
        /// <returns>True if successfull or false if not.</returns>
        public override bool SetMember(ScriptThread thread, VariableSymbol variable, RuntimeValue value)
        {
            if (_nativeObject is ScriptedEntityNode)
            {
                // Check the script process is valid.
                ScriptProcess process = ((ScriptedEntityNode)_nativeObject).ScriptProcess;
                if (process == null) return false;

                // See if we can get the symbol they are after.
                VariableSymbol processSymbol = process.GetSymbol(variable.Identifier, SymbolType.Variable) as VariableSymbol;
                if (processSymbol == null)
                    return false;

                // Grab the variable runtime value. (This can probably be optimized).
                RuntimeValue runtimeValue = process[0].GetRuntimeValueGlobal(processSymbol.Identifier);

                // Check its public.
                if (processSymbol.AccessModifier != SymbolAccessModifier.Public)
                    return false;

                // Check data types are correct.
                if (variable.DataType != processSymbol.DataType)
                    return false;

                // Copy value.
                if (processSymbol.DataType.IsArray == true)
                {
                    int arrayIndex = process[0].CopyValueArrayFromThread(value, process[0], thread);
                    process[0].SetGlobalArray(processSymbol.Identifier, arrayIndex);
                }
                else
                {
                    RuntimeValue copiedValue = process[0].CopyValueFromThread(value, process[0], thread);
                    process[0].SetGlobalVariable(processSymbol.Identifier, copiedValue);
                }
            }
            else if (_nativeObject is EntityNode)
            {

            }
            else if (_nativeObject is EmitterNode)
            {

            }
            else if (_nativeObject is TilemapSegmentNode)
            {

            }
            else if ((_nativeObject as SceneNode) != null)
            {

            }

            return false;
        }

        /// <summary>
        ///     Invoked when a script wants to get the value of an indexed member of this object.
        /// </summary>
        /// <param name="thread">Script thread that invoked this function.</param>
        /// <param name="variable">Symbol describing the member that it wants get.</param>
        /// <param name="index">Index of member it want to get.</param>
        /// <returns>True if successfull or false if not.</returns>
        public override bool SetMember(ScriptThread thread, VariableSymbol variable, RuntimeValue value, RuntimeValue indexer)
        {
            if (_nativeObject is ScriptedEntityNode)
            {
                // Check the script process is valid.
                ScriptProcess process = ((ScriptedEntityNode)_nativeObject).ScriptProcess;
                if (process == null) return false;

                // See if we can get the symbol they are after.
                VariableSymbol processSymbol = process.GetSymbol(variable.Identifier, SymbolType.Variable) as VariableSymbol;
                if (processSymbol == null)
                    return false;

                // Grab the variable runtime value. (This can probably be optimized).
                RuntimeValue runtimeValue = process[0].GetRuntimeValueGlobal(processSymbol.Identifier);

                // Check its public.
                if (processSymbol.AccessModifier != SymbolAccessModifier.Public)
                    return false;

                // Check data types are correct.
                if (variable.DataType.DataType != processSymbol.DataType.DataType || processSymbol.DataType.IsArray != true)
                    return false;

                // Grab the arrays variable.
                RuntimeValue indexValue = process[0].GetRuntimeValueArrayElement(runtimeValue.MemoryIndex, indexer.IntegerLiteral);

                // Copy value.
                RuntimeValue copiedValue = process[0].CopyValueFromThread(value, process[0], thread);

                // Set value (this can be optimized).
                process[0].SetGlobalVariable(processSymbol.Identifier, copiedValue);
            }
            else if (_nativeObject is EntityNode)
            {

            }
            else if (_nativeObject is EmitterNode)
            {

            }
            else if (_nativeObject is TilemapSegmentNode)
            {

            }
            else if ((_nativeObject as SceneNode) != null)
            {

            }

            return false;
        }

        /// <summary>
        ///     Creates a new instance of this class.
        /// </summary>
        /// <param name="node">Scene node to wrap.</param>
        public SceneNodeScriptObject(SceneNode node)
        {
            _nativeObject = node;
        }

        public SceneNodeScriptObject() { }

    }

	/// <summary>
	///		Contains numerous native functions that are commonly used in scripts.
	///		None are commented as they are all fairly self-explanatory.
	/// </summary>
	public class EntityFunctionSet : NativeFunctionSet
	{
		[NativeFunctionInfo("CreateEntity", "object", "")]
		public void CreateEntity(ScriptThread thread)
		{
            EntityNode node = new EntityNode();
            thread.SetReturnValue(new SceneNodeScriptObject(node));
		}

		[NativeFunctionInfo("CreateCamera", "object", "")]
		public void CreateCamera(ScriptThread thread)
		{
            EntityNode node = new CameraNode();
            thread.SetReturnValue(new SceneNodeScriptObject(node));
		}

		[NativeFunctionInfo("CreateEmitter", "object", "")]
		public void CreateEmitter(ScriptThread thread)
		{
            EntityNode node = new EmitterNode();
            thread.SetReturnValue(new SceneNodeScriptObject(node));
		}

		[NativeFunctionInfo("CreateGroup", "object", "")]
		public void CreateGroup(ScriptThread thread)
		{
            EntityNode node = new GroupNode();
            thread.SetReturnValue(new SceneNodeScriptObject(node));
		}

		[NativeFunctionInfo("CreateScriptedEntity", "object", "")]
		public void CreateScriptedEntityA(ScriptThread thread)
		{
            EntityNode node = new ScriptedEntityNode();
            thread.SetReturnValue(new SceneNodeScriptObject(node));
		}

		[NativeFunctionInfo("CreateScriptedEntity", "object", "string")]
		public void CreateScriptedEntityB(ScriptThread thread)
		{
			ScriptedEntityNode node = new ScriptedEntityNode();
            node.ScriptProcess = VirtualMachine.GlobalInstance.LoadScript(thread.GetStringParameter(0));
            Engine.GlobalInstance.ForcedDeltaTimeThisFrame = 1.0f;
            if (node.ScriptProcess != null) node.ScriptProcess[0].InvokeFunction("OnCreate", true, false);
            else return;
            thread.SetReturnValue(new SceneNodeScriptObject(node));
		}

		[NativeFunctionInfo("CreateTilemap", "object", "")]
		public void CreateTilemap(ScriptThread thread)
		{
            EntityNode node = new TilemapSegmentNode();
            thread.SetReturnValue(new SceneNodeScriptObject(node));
		}

		[NativeFunctionInfo("SetEntityParent", "void", "object,object")]
		public void SetEntityParent(ScriptThread thread)
		{
			EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
			SceneNode parent = ((NativeObject)thread.GetObjectParameter(1)).Object as SceneNode;
            if (parent == null) parent = Engine.GlobalInstance.Map.SceneGraph.RootNode;
            if (entity == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetEntityParent with an invalid object.", LogAlertLevel.Error);
				return;
			}
			if (entity.Parent != null) entity.Parent.RemoveChild(entity);
			if (parent != null) parent.AddChild(entity);
            entity.Parent = parent;
        }

		[NativeFunctionInfo("EntityParent", "object", "object")]
		public void EntityParent(ScriptThread thread)
		{
			EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
			if (entity == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityParent with an invalid object.", LogAlertLevel.Error);
				return;
			}
            thread.SetReturnValue(new SceneNodeScriptObject(entity.Parent));
       }

		[NativeFunctionInfo("SetEntityImage", "void", "object,object")]
		public void SetEntityImage(ScriptThread thread)
		{
			EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
			Image image = ((NativeObject)thread.GetObjectParameter(1)).Object as Image;
			if (entity == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetEntityImage with an invalid object.", LogAlertLevel.Error);
				return;
			}
			entity.Image = image;
		}

		[NativeFunctionInfo("EntityImage", "object", "object")]
		public void EntityImage(ScriptThread thread)
		{
			EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
			if (entity == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityImage with an invalid object.", LogAlertLevel.Error);
				return;
			}
            thread.SetReturnValue(new ImageScriptObject(entity.Image));
		}

        [NativeFunctionInfo("SetEntityFont", "void", "object,object")]
        public void SetEntityFont(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            BitmapFont font = ((NativeObject)thread.GetObjectParameter(1)).Object as BitmapFont;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetEntityFont with an invalid object.", LogAlertLevel.Error);
                return;
            }
            entity.Font = font;
        }

        [NativeFunctionInfo("EntityFont", "object", "object")]
        public void EntityFont(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityFont with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(new FontScriptObject(entity.Font));
        }

		[NativeFunctionInfo("SetEntityRenderMode", "void", "object,int")]
		public void SetEntityRenderMode(ScriptThread thread)
		{
			EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
			if (entity == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetEntityRenderMode with an invalid object.", LogAlertLevel.Error);
				return;
			}
			entity.RenderMode = (EntityRenderMode)thread.GetIntegerParameter(1);
		}

		[NativeFunctionInfo("EntityRenderMode", "int", "object")]
		public void EntityRenderMode(ScriptThread thread)
		{
			EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
			if (entity == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityRenderMode with an invalid object.", LogAlertLevel.Error);
				return;
			}
			thread.SetReturnValue((int)entity.RenderMode);
		}

		[NativeFunctionInfo("SetEntityFrame", "void", "object,int")]
		public void SetEntityFrame(ScriptThread thread)
		{
			EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
			if (entity == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetEntityFrame with an invalid object.", LogAlertLevel.Error);
				return;
			}
			entity.Frame = thread.GetIntegerParameter(1);
		}

		[NativeFunctionInfo("EntityFrame", "int", "object")]
		public void EntityFrame(ScriptThread thread)
		{
			EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
			if (entity == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityFrame with an invalid object.", LogAlertLevel.Error);
				return;
			}
			thread.SetReturnValue(entity.Frame);
		}

        [NativeFunctionInfo("SetEntitySolid", "void", "object,bool")]
        public void SetEntitySolid(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetEntitySolid with an invalid object.", LogAlertLevel.Error);
                return;
            }
            entity.IsSolid = thread.GetBooleanParameter(1);
        }

        [NativeFunctionInfo("EntitySolid", "bool", "object")]
        public void EntitySolid(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntitySolid with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.IsSolid);
        }

        [NativeFunctionInfo("SetEntityEnabled", "void", "object,bool")]
        public void SetEntityEnabled(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetEntityEnabled with an invalid object.", LogAlertLevel.Error);
                return;
            }
            entity.IsEnabled = thread.GetBooleanParameter(1);
        }

        [NativeFunctionInfo("EntityEnabled", "bool", "object")]
        public void EntityEnabled(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityEnabled with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.IsEnabled);
        }

        [NativeFunctionInfo("SetEntityVisible", "void", "object,bool")]
        public void SetEntityVisible(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetEntityVisible with an invalid object.", LogAlertLevel.Error);
                return;
            }
            entity.IsVisible = thread.GetBooleanParameter(1);
        }

        [NativeFunctionInfo("EntityVisible", "bool", "object")]
        public void EntityVisible(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityVisible with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.IsVisible);
        }

        [NativeFunctionInfo("SetEntityPersistent", "void", "object,bool")]
        public void SetEntityPersistent(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetEntityPersistent with an invalid object.", LogAlertLevel.Error);
                return;
            }
            entity.IsPersistent = thread.GetBooleanParameter(1);
        }

        [NativeFunctionInfo("EntityPersistent", "bool", "object")]
        public void EntityPersistent(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityPersistent with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.IsPersistent);
        }

        [NativeFunctionInfo("SetEntityBlendMode", "void", "object,int")]
        public void SetEntityBlendMode(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetEntityBlendMode with an invalid object.", LogAlertLevel.Error);
                return;
            }
            entity.BlendMode = (BlendMode)thread.GetIntegerParameter(1);
        }

        [NativeFunctionInfo("EntityBlendMode", "int", "object")]
        public void EntityBlendMode(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityBlendMode with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue((int)entity.BlendMode);
        }

        [NativeFunctionInfo("SetEntityName", "void", "object,string")]
        public void SetEntityName(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetEntityName with an invalid object.", LogAlertLevel.Error);
                return;
            }
            entity.Name = thread.GetStringParameter(1);
        }

        [NativeFunctionInfo("EntityName", "string", "object")]
        public void EntityName(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityName with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.Name);
        }

        [NativeFunctionInfo("SetEntityType", "void", "object,string")]
        public void SetEntityType(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetEntityType with an invalid object.", LogAlertLevel.Error);
                return;
            }
            entity.Type = thread.GetStringParameter(1);
        }

        [NativeFunctionInfo("SetEntityEvent", "void", "object,string")]
        public void SetEntityEvent(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetEntityEvent with an invalid object.", LogAlertLevel.Error);
                return;
            }
            entity.Event = thread.GetStringParameter(1);
        }

        [NativeFunctionInfo("EntityEvent", "string", "object")]
        public void EntityEvent(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityEvent with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.Event);
        }

        [NativeFunctionInfo("SetEntityText", "void", "object,string")]
        public void SetEntityText(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetEntityText with an invalid object.", LogAlertLevel.Error);
                return;
            }
            entity.Text = thread.GetStringParameter(1);
        }

        [NativeFunctionInfo("EntityText", "string", "object")]
        public void EntityText(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityText with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.Text);
        }

        [NativeFunctionInfo("EntityType", "string", "object")]
        public void EntityType(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityType with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.Type);
        }

        [NativeFunctionInfo("SetCameraClearColor", "void", "object,int")]
        public void SetCameraClearColor(ScriptThread thread)
        {
            CameraNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as CameraNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetCameraClearColor with an invalid object.", LogAlertLevel.Error);
                return;
            }
            entity.ClearColor = thread.GetIntegerParameter(1);
        }

        [NativeFunctionInfo("CameraClearColor", "int", "object")]
        public void CameraClearColor(ScriptThread thread)
        {
            CameraNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as CameraNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called CameraClearColor with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.ClearColor);
        }

        [NativeFunctionInfo("SetEntityColor", "void", "object,int")]
        public void SetEntityColor(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetEntityColor with an invalid object.", LogAlertLevel.Error);
                return;
            }
            entity.Color = thread.GetIntegerParameter(1);
        }

        [NativeFunctionInfo("EntityColor", "int", "object")]
        public void EntityColor(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityColor with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.Color);
        }

        [NativeFunctionInfo("EntityBoundingBoxX", "int", "object")]
        public void EntityBoundingBoxX(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityBoundingBoxX with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.BoundingRectangle.X);
        }

        [NativeFunctionInfo("EntityBoundingBoxY", "int", "object")]
        public void EntityBoundingBoxY(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityBoundingBoxY with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.BoundingRectangle.Y);
        }

        [NativeFunctionInfo("EntityBoundingBoxWidth", "int", "object")]
        public void EntityBoundingBoxWidth(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityBoundingBoxWidth with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.BoundingRectangle.Width);
        }

        [NativeFunctionInfo("EntityBoundingBoxHeight", "int", "object")]
        public void EntityBoundingBoxHeight(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityBoundingBoxHeight with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.BoundingRectangle.Height);
        }

        [NativeFunctionInfo("SetEntityBoundingBox", "void", "object,int,int,int,int")]
        public void SetEntityBoundingBox(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetEntityBoundingBox with an invalid object.", LogAlertLevel.Error);
                return;
            }
            entity.BoundingRectangle = new System.Drawing.Rectangle(thread.GetIntegerParameter(1), thread.GetIntegerParameter(2), thread.GetIntegerParameter(3), thread.GetIntegerParameter(4));
        }

        [NativeFunctionInfo("EntityX", "float", "object")]
        public void EntityX(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityX with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.Transformation.X);
        }

        [NativeFunctionInfo("EntityY", "float", "object")]
        public void EntityY(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityY with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.Transformation.Y);
        }

        [NativeFunctionInfo("EntityZ", "float", "object")]
        public void EntityZ(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityZ with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.Transformation.Z);
        }

        [NativeFunctionInfo("EntityDepthLayer", "int", "object")]
        public void EntityDepthLayer(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityDepthLayer with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.DepthLayer);
        }

        [NativeFunctionInfo("SetEntityDepthLayer", "void", "object,int")]
        public void SetEntityDepthLayer(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetEntityDepthLayer with an invalid object.", LogAlertLevel.Error);
                return;
            }
            entity.DepthLayer = thread.GetIntegerParameter(1);
        }

        [NativeFunctionInfo("EntityScaleX", "float", "object")]
        public void EntityScaleX(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityScaleX with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.Transformation.ScaleX);
        }

        [NativeFunctionInfo("EntityScaleY", "float", "object")]
        public void EntityScaleY(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityScaleY with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.Transformation.ScaleY);
        }

        [NativeFunctionInfo("EntityScaleZ", "float", "object")]
        public void EntityScaleZ(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityScaleZ with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.Transformation.ScaleZ);
        }

        [NativeFunctionInfo("EntityAngleX", "float", "object")]
        public void EntityAngleX(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityAngleX with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.Transformation.AngleX);
        }

        [NativeFunctionInfo("EntityAngleY", "float", "object")]
        public void EntityAngleY(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityAngleY with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.Transformation.AngleY);
        }

        [NativeFunctionInfo("EntityAngleZ", "float", "object")]
        public void EntityAngleZ(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityAngleZ with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.Transformation.AngleZ);
        }


		[NativeFunctionInfo("SceneGraphRootNode", "object", "")]
		public void SceneGraphRootNode(ScriptThread thread)
		{
            thread.SetReturnValue(new SceneNodeScriptObject(Engine.GlobalInstance.Map.SceneGraph.RootNode));
		}

		[NativeFunctionInfo("ActivateCamera", "void", "object")]
		public void ActivateCamera(ScriptThread thread)
		{
            CameraNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as CameraNode;
			if (entity == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called AttachCameraToSceneGraph with an invalid object.", LogAlertLevel.Error);
				return;
			}
			Engine.GlobalInstance.Map.SceneGraph.AttachCamera(entity);
		}

		[NativeFunctionInfo("DeactivateCamera", "void", "object")]
		public void DeactiveCamera(ScriptThread thread)
		{
			CameraNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as CameraNode;
			if (entity == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called DetachCameraToSceneGraph with an invalid object.", LogAlertLevel.Error);
				return;
			}
			Engine.GlobalInstance.Map.SceneGraph.DetachCamera(entity);
		}

        [NativeFunctionInfo("TurnEntity", "void", "object,float,float,float")]
        public void TurnEntity(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called TurnEntity with an invalid object.", LogAlertLevel.Error);
                return;
            }
            entity.Turn(thread.GetFloatParameter(1), thread.GetFloatParameter(2), thread.GetFloatParameter(3));
        }

        [NativeFunctionInfo("RotateEntity", "void", "object,float,float,float")]
        public void RotateEntity(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called RotateEntity with an invalid object.", LogAlertLevel.Error);
                return;
            }
            entity.Rotate(thread.GetFloatParameter(1), thread.GetFloatParameter(2), thread.GetFloatParameter(3));
        }

		[NativeFunctionInfo("MoveEntity", "void", "object,float,float,float")]
		public void MoveEntity(ScriptThread thread)
		{
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
			if (entity == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called MoveEntity with an invalid object.", LogAlertLevel.Error);
				return;
			}
			entity.Move(thread.GetFloatParameter(1), thread.GetFloatParameter(2), thread.GetFloatParameter(3));
		}

        [NativeFunctionInfo("TranslateEntity", "void", "object,float,float,float")]
        public void TranslateEntity(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called TranslateEntity with an invalid object.", LogAlertLevel.Error);
                return;
            }
            entity.Translate(thread.GetFloatParameter(1), thread.GetFloatParameter(2), thread.GetFloatParameter(3));
        }

		[NativeFunctionInfo("PositionEntity", "void", "object,float,float,float")]
		public void PositionEntity(ScriptThread thread)
		{
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
			if (entity == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called MoveEntity with an invalid object.", LogAlertLevel.Error);
				return;
			}
			entity.Position(thread.GetFloatParameter(1), thread.GetFloatParameter(2), thread.GetFloatParameter(3));
		}

		[NativeFunctionInfo("SetEntityCollisionLayers", "void", "object,int[]")]
		public void SetEntityCollisionLayers(ScriptThread thread)
		{
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
			if (entity == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetEntityCollisionLayers with an invalid object.", LogAlertLevel.Error);
				return;
			}
			if (entity.CollisionPolygon == null) return;
            int arrayIndex = thread.GetArrayParameter(1);
            entity.CollisionPolygon.Layers = new int[thread.GetArrayLength(arrayIndex)];

            for (int i = 0; i < thread.GetArrayLength(arrayIndex); i++)
                entity.CollisionPolygon.Layers[i] = thread.GetIntArrayElement(arrayIndex, i);
        }

        [NativeFunctionInfo("EntityCollisionLayers", "int[]", "object")]
        public void EntityCollisionLayer(ScriptThread thread)
		{
			EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
			if (entity == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityCollisionLayers with an invalid object.", LogAlertLevel.Error);
				return;
			}
			if (entity.CollisionPolygon == null) return;
            int arrayIndex = thread.AllocateArray(DataType.Int, entity.CollisionPolygon.Layers.Length);
            for (int i = 0; i < entity.CollisionPolygon.Layers.Length; i++)
                thread.SetArrayElement(arrayIndex, i, entity.CollisionPolygon.Layers[i]);
			thread.SetReturnValueArray(arrayIndex);
		}

        [NativeFunctionInfo("EntityCollisionBoxX", "int", "object")]
        public void EntityCollisionBoxX(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityCollisionBoxX with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.CollisionRectangle.X);
        }

        [NativeFunctionInfo("EntityCollisionBoxY", "int", "object")]
        public void EntityCollisionBoxY(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityCollisionBoxY with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.CollisionRectangle.Y);
        }

        [NativeFunctionInfo("EntityCollisionBoxWidth", "int", "object")]
        public void EntityCollisionBoxWidth(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityCollisionBoxWidth with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.CollisionRectangle.Width);
        }

        [NativeFunctionInfo("EntityCollisionBoxHeight", "int", "object")]
        public void EntityCollisionBoxHeight(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityCollisionBoxHeight with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entity.CollisionRectangle.Height);
        }

        [NativeFunctionInfo("SetEntityCollisionBox", "void", "object,int,int,int,int")]
        public void SetEntityCollisionBox(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetEntityCollisionBox with an invalid object.", LogAlertLevel.Error);
                return;
            }
            entity.CollisionRectangle = new System.Drawing.Rectangle(thread.GetIntegerParameter(1), thread.GetIntegerParameter(2), thread.GetIntegerParameter(3), thread.GetIntegerParameter(4));
        }

        [NativeFunctionInfo("EntityChildren", "object[]", "object")]
        public void EntityChildren(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityChildren with an invalid object.", LogAlertLevel.Error);
                return;
            }
            if (entity.Children.Count > 0)
            {
                int memoryIndex = thread.AllocateArray(DataType.Object, entity.Children.Count);
                for (int i = 0; i < entity.Children.Count; i++)
                {
                    thread.SetArrayElement(memoryIndex, i, new SceneNodeScriptObject((SceneNode)entity.Children[i]));
                }
                thread.SetReturnValueArray(memoryIndex);
            }
        }

        [NativeFunctionInfo("DestroyEntity", "void", "object")]
        public void DestroyEntity(ScriptThread thread)
        {
            RuntimeObject obj = thread.GetObjectParameter(0);
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called DestroyEntity with an invalid object.", LogAlertLevel.Error);
                return;
            }
            entity.Dispose();
        }

        [NativeFunctionInfo("TriggerEntityEvents", "void", "object")]
        public void TriggerEntityEvents(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called TriggerEntityEvents with an invalid object.", LogAlertLevel.Error);
                return;
            }
            entity.TriggerEvents();
        }

        [NativeFunctionInfo("ClosestSideOfEntity", "int", "object,object")]
        public void ClosestSideOfEntity(ScriptThread thread)
        {
            EntityNode aEntity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            EntityNode bEntity = ((NativeObject)thread.GetObjectParameter(1)).Object as EntityNode;
            if (aEntity == null || bEntity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ClosestSideOfEntity with an invalid object.", LogAlertLevel.Error);
                return;
            }

            float bWidth = bEntity.Width * bEntity.Transformation.ScaleX, bHeight = bEntity.Height * bEntity.Transformation.ScaleY;
            float bCenterX = bEntity.Transformation.X + (bWidth / 2.0f), bCenterY = bEntity.Transformation.Y + (bHeight / 2.0f);
            float aWidth = aEntity.Width * aEntity.Transformation.ScaleX, aHeight = aEntity.Height * aEntity.Transformation.ScaleY;
            float aCenterX = aEntity.Transformation.X + (aWidth / 2.0f), aCenterY = aEntity.Transformation.Y + (aHeight / 2.0f);
            float bVertexX = 0, bVertexY = 0;
            float aVertexX = 0, aVertexY = 0;

            if (aCenterX > bCenterX)
                if (aCenterY > bCenterY)
                {
                    // Were in the bottom-right corner of the rectangle.
                    bVertexX = bEntity.Transformation.X + bWidth;
                    bVertexY = bEntity.Transformation.Y + bHeight;
                    aVertexX = aEntity.Transformation.X;
                    aVertexY = aEntity.Transformation.Y;

                    float xDifference = aVertexX - bVertexX;
                    float yDifference = aVertexY - bVertexY;
                    if (Math.Abs(xDifference) > Math.Abs(yDifference))
                        thread.SetReturnValue(4);
                    else
                        thread.SetReturnValue(2);
                }
                else
                {
                    // Were in the top-right corner of the rectangle.
                    bVertexX = bEntity.Transformation.X + bWidth;
                    bVertexY = bEntity.Transformation.Y;
                    aVertexX = aEntity.Transformation.X;
                    aVertexY = aEntity.Transformation.Y + aHeight;

                    float xDifference = aVertexX - bVertexX;
                    float yDifference = aVertexY - bVertexY;
                    if (Math.Abs(xDifference) > Math.Abs(yDifference))
                        thread.SetReturnValue(3);
                    else
                        thread.SetReturnValue(2);
                }
            else
                if (aCenterY > bCenterY)
                {
                    // Were in the bottom-left corner of the rectangle.
                    bVertexX = bEntity.Transformation.X;
                    bVertexY = bEntity.Transformation.Y + bHeight;
                    aVertexX = aEntity.Transformation.X + aWidth;
                    aVertexY = aEntity.Transformation.Y;

                    float xDifference = aVertexX - bVertexX;
                    float yDifference = aVertexY - bVertexY;
                    if (Math.Abs(xDifference) > Math.Abs(yDifference))
                        thread.SetReturnValue(4);
                    else
                        thread.SetReturnValue(1);
                }
                else
                {
                    // Were in the top-left corner of the rectangle.
                    bVertexX = bEntity.Transformation.X;
                    bVertexY = bEntity.Transformation.Y;
                    aVertexX = aEntity.Transformation.X + aWidth;
                    aVertexY = aEntity.Transformation.Y + aHeight;

                    float xDifference = aVertexX - bVertexX;
                    float yDifference = aVertexY - bVertexY;
                    if (Math.Abs(xDifference) > Math.Abs(yDifference))
                        thread.SetReturnValue(3);
                    else
                        thread.SetReturnValue(1);
                }
        }

        [NativeFunctionInfo("MapBoundryX", "float", "")]
        public void MapBoundryX(ScriptThread thread)
        {
            thread.SetReturnValue(Engine.GlobalInstance.Map.SceneGraph.GlobalBoundingBox.X);
        }

        [NativeFunctionInfo("MapBoundryY", "float", "")]
        public void MapBoundryY(ScriptThread thread)
        {
            thread.SetReturnValue(Engine.GlobalInstance.Map.SceneGraph.GlobalBoundingBox.Y);
        }

        [NativeFunctionInfo("MapBoundryWidth", "float", "")]
        public void MapBoundryWidth(ScriptThread thread)
        {
            thread.SetReturnValue(Engine.GlobalInstance.Map.SceneGraph.GlobalBoundingBox.Width);
        }

        [NativeFunctionInfo("MapBoundryHeight", "float", "")]
        public void MapBoundryHeight(ScriptThread thread)
        {
            thread.SetReturnValue(Engine.GlobalInstance.Map.SceneGraph.GlobalBoundingBox.Height);
        }

        [NativeFunctionInfo("EntitiesOfType", "object[]", "string")]
        public void EntitiesOfType(ScriptThread thread)
        {
            string typeName = thread.GetStringParameter(0);
            ArrayList list = new ArrayList();
            foreach (SceneNode node in Fusion.Engine.Engine.GlobalInstance.Map.SceneGraph.EnumerateNodes())
            {
                if (node is ScriptedEntityNode && ((ScriptedEntityNode)node).Type.ToLower() == typeName)
                    list.Add(node);
            }

            if (list.Count > 0)
            {
                int memoryIndex = thread.AllocateArray(DataType.Object, list.Count);
                for (int i = 0; i < list.Count; i++)
                {
                    thread.SetArrayElement(memoryIndex, i, new SceneNodeScriptObject((SceneNode)list[i]));
                }
                thread.SetReturnValueArray(memoryIndex);
            }
        }

        [NativeFunctionInfo("CenterEntityOn", "void", "object,object")]
        public void CenterEntityOn(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            EntityNode target = ((NativeObject)thread.GetObjectParameter(1)).Object as EntityNode;
            if (entity == null || target == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called CenterEntityOn with an invalid object.", LogAlertLevel.Error);
                return;
            }

            // Work out the central points.
            Transformation entityTransform = entity.CalculateTransformation();
            Transformation targetTransform = target.CalculateTransformation();

            // If its a camera then we need to invert the coordinates as it uses
            // slightly different ones from normal entities.
            if (entity as CameraNode != null)
            {
                entityTransform.X = -entityTransform.X;
                entityTransform.Y = -entityTransform.Y;
            }
            if (target as CameraNode != null)
            {
                targetTransform.X = -targetTransform.X;
                targetTransform.Y = -targetTransform.Y;
            }

            float targetEntityTransformCenterX = targetTransform.X + ((target.BoundingRectangle.Width / 2) * targetTransform.ScaleX);
            float targetEntityTransformCenterY = targetTransform.Y + ((target.BoundingRectangle.Height / 2) * targetTransform.ScaleY);
            entity.Position(targetEntityTransformCenterX - ((entity.BoundingRectangle.Width / 2) * entityTransform.ScaleX), targetEntityTransformCenterY - ((entity.BoundingRectangle.Height / 2) * entityTransform.ScaleY), entity.Transformation.Z);
        }

        [NativeFunctionInfo("EntityRenderX", "float", "object,object")]
        public void EntityRenderX(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            CameraNode camera = ((NativeObject)thread.GetObjectParameter(1)).Object as CameraNode;
            if (entity == null || camera == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityRenderX with an invalid object.", LogAlertLevel.Error);
                return;
            }
            Transformation transformation = entity.CalculateTransformation(camera);
            thread.SetReturnValue(transformation.X);
        }

        [NativeFunctionInfo("EntityRenderY", "float", "object,object")]
        public void EntityRenderY(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            CameraNode camera = ((NativeObject)thread.GetObjectParameter(1)).Object as CameraNode;
            if (entity == null || camera == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityRenderY with an invalid object.", LogAlertLevel.Error);
                return;
            }
            Transformation transformation = entity.CalculateTransformation(camera);
            thread.SetReturnValue(transformation.Y);
        }

        [NativeFunctionInfo("EntityRenderZ", "float", "object,object")]
        public void EntityRenderZ(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            CameraNode camera = ((NativeObject)thread.GetObjectParameter(1)).Object as CameraNode;
            if (entity == null || camera == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityRenderZ with an invalid object.", LogAlertLevel.Error);
                return;
            }
            Transformation transformation = entity.CalculateTransformation(camera);
            thread.SetReturnValue(transformation.Z);
        }

        [NativeFunctionInfo("EntityRenderScaleX", "float", "object,object")]
        public void EntityRenderScaleX(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            CameraNode camera = ((NativeObject)thread.GetObjectParameter(1)).Object as CameraNode;
            if (entity == null || camera == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityRenderScaleX with an invalid object.", LogAlertLevel.Error);
                return;
            }
            Transformation transformation = entity.CalculateTransformation(camera);
            thread.SetReturnValue(transformation.ScaleX);
        }

        [NativeFunctionInfo("EntityRenderScaleY", "float", "object,object")]
        public void EntityRenderScaleY(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            CameraNode camera = ((NativeObject)thread.GetObjectParameter(1)).Object as CameraNode;
            if (entity == null || camera == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityRenderScaleY with an invalid object.", LogAlertLevel.Error);
                return;
            }
            Transformation transformation = entity.CalculateTransformation(camera);
            thread.SetReturnValue(transformation.ScaleX);
        }

        [NativeFunctionInfo("EntityRenderScaleZ", "float", "object,object")]
        public void EntityRenderScaleZ(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            CameraNode camera = ((NativeObject)thread.GetObjectParameter(1)).Object as CameraNode;
            if (entity == null || camera == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityRenderScaleZ with an invalid object.", LogAlertLevel.Error);
                return;
            }
            Transformation transformation = entity.CalculateTransformation(camera);
            thread.SetReturnValue(transformation.ScaleZ);
        }

        [NativeFunctionInfo("EntityRenderAngleX", "float", "object,object")]
        public void EntityRenderAngleX(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            CameraNode camera = ((NativeObject)thread.GetObjectParameter(1)).Object as CameraNode;
            if (entity == null || camera == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityRenderAngleZ with an invalid object.", LogAlertLevel.Error);
                return;
            }
            Transformation transformation = entity.CalculateTransformation(camera);
            thread.SetReturnValue(transformation.AngleX);
        }

        [NativeFunctionInfo("EntityRenderAngleY", "float", "object,object")]
        public void EntityRenderAngleY(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            CameraNode camera = ((NativeObject)thread.GetObjectParameter(1)).Object as CameraNode;
            if (entity == null || camera == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityRenderAngleY with an invalid object.", LogAlertLevel.Error);
                return;
            }
            Transformation transformation = entity.CalculateTransformation(camera);
            thread.SetReturnValue(transformation.AngleY);
        }

        [NativeFunctionInfo("EntityRenderAngleZ", "float", "object,object")]
        public void EntityRenderAngleZ(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            CameraNode camera = ((NativeObject)thread.GetObjectParameter(1)).Object as CameraNode;
            if (entity == null || camera == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityRenderAngleZ with an invalid object.", LogAlertLevel.Error);
                return;
            }
            Transformation transformation = entity.CalculateTransformation(camera);
            thread.SetReturnValue(transformation.AngleZ);
        }

        [NativeFunctionInfo("FindEntityByName", "void", "string")]
        public void FindEntityByName(ScriptThread thread)
        {
            string name = thread.GetStringParameter(0);
            foreach (SceneNode node in Engine.GlobalInstance.Map.SceneGraph.EnumerateNodes())
                if (node.Name.ToLower() == name.ToLower())
                {
                    thread.SetReturnValue(new SceneNodeScriptObject(node));
                    return;
                }
        }

        [NativeFunctionInfo("DestroyEntitiesWithName", "object", "string")]
        public void DestroyEntitiesWithName(ScriptThread thread)
        {
            string name = thread.GetStringParameter(0);
            foreach (SceneNode node in Engine.GlobalInstance.Map.SceneGraph.EnumerateNodes())
                if (node.Name.ToLower() == name.ToLower())
                    node.Dispose();
        }

        [NativeFunctionInfo("StopEntityScript", "void", "object")]
        public void StopEntityScript(ScriptThread thread)
        {
            ScriptedEntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptedEntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StopObjectScript with an invalid object.", LogAlertLevel.Error);
                return;
            }
            //entity.Dispose();
            if (entity.ScriptExecutionProcess != null)
            {
                entity.ScriptExecutionProcess.Finish(BinaryPhoenix.Fusion.Runtime.Processes.ProcessResult.Failed);
                entity.ScriptExecutionProcess.Dispose();
                entity.ScriptExecutionProcess = null;
                //GC.Collect();
            }
        }

        [NativeFunctionInfo("EntityHitTest", "bool", "object,object")]
        public void EntityHitTestA(ScriptThread thread)
        {
            EntityNode entitya = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            EntityNode entityb = ((NativeObject)thread.GetObjectParameter(1)).Object as EntityNode;
            if (entitya == null || entityb == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityHitTest with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(entitya.CollisionPolygon.HitTest(entityb.CollisionPolygon));
        }

        [NativeFunctionInfo("EntityHitTest", "bool", "object,int,int,int,int")]
        public void EntityHitTestB(ScriptThread thread)
        {
            EntityNode entitya = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entitya == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called EntityHitTest with an invalid object.", LogAlertLevel.Error);
                return;
            }
            CollisionRectangle rect = new CollisionRectangle(new Transformation(thread.GetIntegerParameter(1), thread.GetIntegerParameter(2), 0, 0, 0, 0, 1, 1, 1), thread.GetIntegerParameter(3), thread.GetIntegerParameter(4));
            rect.Layers = entitya.CollisionPolygon.Layers;
            thread.SetReturnValue(entitya.CollisionPolygon.HitTest(rect));
        }

        [NativeFunctionInfo("DisableAllEntities", "void", "")]
        public void DisableAllEntities(ScriptThread thread)
        {
            foreach (SceneNode node in Engine.GlobalInstance.Map.SceneGraph.EnumerateNodes())
                if (node as EntityNode != null)
                {
                    ((EntityNode)node).PreviousIsEnabled = ((EntityNode)node).IsEnabled; 
                    ((EntityNode)node).IsEnabled = false;
                }
        }

        [NativeFunctionInfo("EnableAllEntities", "void", "")]
        public void EnableAllEntities(ScriptThread thread)
        {
            foreach (SceneNode node in Engine.GlobalInstance.Map.SceneGraph.EnumerateNodes())
                if (node as EntityNode != null)
                {
                    ((EntityNode)node).IsEnabled = ((EntityNode)node).PreviousIsEnabled;
                }
        }

        [NativeFunctionInfo("MoveEntityTowardsPoint", "void", "object,float,float,float")]
        public void MoveEntityTowardsPoint(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            float x = thread.GetFloatParameter(1);
            float y = thread.GetFloatParameter(2);
            float speed = thread.GetFloatParameter(3);
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called MoveEntityTowardsPoint with an invalid object.", LogAlertLevel.Error);
                return;
            }

            // Work out the central points.
            Transformation entityTransform = entity.CalculateTransformation();

            // If its a camera then we need to invert the coordinates as it uses
            // slightly different ones from normal entities.
            if (entity is CameraNode)
            {
                entityTransform.X = -entityTransform.X;
                entityTransform.Y = -entityTransform.Y;
            }

            // Are we at the correct place? If so why bother with the below code :P.
            if (entityTransform.X == x && entityTransform.Y == y)
                return;

            float entityTransformCenterX = entityTransform.X + ((entity.BoundingRectangle.Width / 2) * entityTransform.ScaleX);
            float entityTransformCenterY = entityTransform.Y + ((entity.BoundingRectangle.Height / 2) * entityTransform.ScaleY);
            float vectorX = entityTransformCenterX - x;
            float vectorY = entityTransformCenterY - y;
            float distance = (float)Math.Sqrt(vectorX * vectorX + vectorY * vectorY);
            vectorX /= distance;
            vectorY /= distance;

            if (float.IsNaN(vectorX * speed) || float.IsNaN(vectorY * speed)) return;

            // Work out vector towards entity.
            entity.Move(-(vectorX * speed),-(vectorY * speed), 0.0f);
        }
    }

}