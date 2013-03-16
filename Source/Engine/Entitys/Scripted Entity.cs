/*
 * File: Scripted Entity.cs
 *
 * Contains the main declaration of the scripted entity class
 * which is a derivitive of the SceneNode class and is
 * used to control an entitys logic via a FusionScript script.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using BinaryPhoenix.Fusion.Graphics;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Runtime.Scripting;
using BinaryPhoenix.Fusion.Engine.ScriptingFunctions;
using BinaryPhoenix.Fusion.Runtime.Processes;
using BinaryPhoenix.Fusion.Runtime.Resources;
using BinaryPhoenix.Fusion.Runtime.Collision;
using BinaryPhoenix.Fusion.Engine.Processes;

namespace BinaryPhoenix.Fusion.Engine.Entitys
{

	/// <summary>
	///		The scripted entity class is a derivitive of the SceneNode class and is
	///		used to control an entitys logic via a FusionScript script.
	/// </summary>
	public class ScriptedEntityNode : EntityNode
	{
		#region Members
		#region Variables

		protected ScriptExecutionProcess _process = null;
        protected FunctionSymbol _renderFunction = null;

		#endregion
		#region Properties

        /// <summary>
        ///		Returns a string which can be used to identify what kind of entity this is.
        /// </summary>
        public override string Type
        {
            get 
            {
                if (_type != "")
                    return _type;
                else
                    if (_process != null && _process.Process != null && _process.Process.Url != "")
                        return Path.GetFileNameWithoutExtension(_process.Process.Url.ToLower());
                    else
                        return "scriptedentity";
            }
        }

		/// <summary>
		///		Gets or sets the script process that contains this entitys logic.
		/// </summary>
		public ScriptProcess ScriptProcess
		{
			get { return _process.Process; }
			set 
			{
                if (_process.Process != null && _process.Process.State != null) _process.Process[0].InvokeFunction("OnDispose");
				_process.Process = value;
                if (_process.Process != null) _process.Process.OnStateChange += new StateChangeDelegate(OnStateChange);
                if (_process.Process != null && _process.Process.State != null) OnStateChange(_process.Process, _process.Process.State);
                SyncCollisionEvents();
            }
		}

		/// <summary>
		///		Gets or sets the process used to execute this entitys script.
		/// </summary>
		public ScriptExecutionProcess ScriptExecutionProcess
		{
			get { return _process; }
			set { _process = value; }
		}

		/// <summary>
		///		Gets or sets the parent node thats above this node in the scene graph.
		/// </summary>
		public override SceneNode Parent
		{
			get { return _parent; }
			set 
			{
                if (_process != null && _parent != null && _process.Process != null)
				{
                    _process.Process[0].PassParameter(new SceneNodeScriptObject(_parent));
					_process.Process[0].InvokeFunction("OnDetached", true, false);
				}
				_parent = value;
                if (_process != null && _process.Process != null)
				{
                    _process.Process[0].PassParameter(new SceneNodeScriptObject(_parent));
                    _process.Process[0].InvokeFunction("OnAttached", true, false);
				}
			}
		}

		#endregion
		#endregion
		#region Methods

        /// <summary>
        ///     Converts this object to a textural form.
        /// </summary>
        /// <returns>Textural form of this object.</returns>
        public override string ToString()
        {
            return _process != null && _process.Process != null ? _process.Process.Url : base.ToString();
        }

        /// <summary>
        ///     Called when the state of this entities script is changed.
        /// </summary>
        /// <param name="process">Process that had its state changed.</param>
        /// <param name="sate">New state.</param>
        public void OnStateChange(ScriptProcess process, StateSymbol state)
        {
            _renderFunction = state == null ? null : _process.Process.State.FindSymbol("OnRender", SymbolType.Function) as FunctionSymbol;
           // SyncCollisionEvents();
        }

        /// <summary>
        ///     Responsable for removing references of this object and deallocated
        ///     resources that have been allocated.
        /// </summary>
        public override void Dispose()
        {
            // Call the abstracted base method to clean up general things.
            base.Dispose();

            // Dispose of the execution process.
            if (_process != null)
            {
                ProcessManager.DettachProcess(_process);
                _process.Finish(ProcessResult.Failed);
                _process.Dispose();
                _process = null;
                //_process.Dispose();
            }
        }

		/// <summary>
		///		Adds the given scene node to this nodes child list.
		/// </summary>	
		/// <param name="child">Scene node to add as child.</param>
		public override void AddChild(SceneNode child)
        {
            if (_childList.Contains(child)) return;
            if (_process != null && _process.Process != null)
			{
                _process.Process[0].PassParameter(new SceneNodeScriptObject(child));
                _process.Process[0].InvokeFunction("OnGainedChild", true, false);
			}
			base.AddChild(child);
		}

		/// <summary>
		///		Removes the given scene node from this nodes child list.
		/// </summary>
		/// <param name="child">Scene node to remove.</param>
		public override void RemoveChild(SceneNode child)
		{
            if (_process != null && _process.Process != null)
			{
                _process.Process[0].PassParameter(new SceneNodeScriptObject(child));
                _process.Process[0].InvokeFunction("OnLostChild", true, false);
			}
			base.RemoveChild(child);
		}

		/// <summary>
		///		Renders this entity at a position releative to its parent.
		/// </summary>
		/// <param name="position">Position where this entity's parent node was rendered.</param>
        public override void Render(Transformation transformation, CameraNode camera, int layer)
		{
			if (layer == _depthLayer && _process != null && _process.Process != null && _renderFunction != null)
                _process.Process[0].InvokeFunction(_renderFunction, true, true, true);
			base.Render(transformation, camera, layer);
		}

		/// <summary>
		///		Called when a property attached to this entitys script is modified from native code. 
		/// </summary>
		/// <param name="propertyName">Name of property that was modified</param>
		public virtual void OnPropertyChange(string propertyName)
		{
            if (_process != null && _process.Process != null)
			{
				_process.Process[0].PassParameter(propertyName);
                _process.Process[0].InvokeFunction("OnPropertyChange", true, false);
			}
		}

		/// <summary>
		///		Copys all the data contained in this scene node to another scene node.
		/// </summary>
		/// <param name="node">Scene node to copy data into.</param>
		public override void CopyTo(SceneNode node)
		{
			ScriptedEntityNode entityNode = node as ScriptedEntityNode;
			if (entityNode == null) return;
			base.CopyTo(node);
			
			if (_process.Process.Url != "")
			{
                // TODO: Possibly implement copy system rather than reloading?
                entityNode.ScriptProcess = new ScriptProcess(VirtualMachine.GlobalInstance,_process.Process);
                entityNode.ScriptProcess.Url = _process.Process.Url;

                if (_process.Process.State != null)
                    entityNode.ScriptProcess.ChangeState(_process.Process.State.Identifier);
			
                // Copy memory and object heap.
                /*
                _process.Process.MemoryHeap = new RuntimeValue[entityNode.ScriptProcess.MemoryHeap.Length];
                for (int i = 0; i < entityNode.ScriptProcess.MemoryHeap.Length; i++)
                {
                    if (entityNode.ScriptProcess.MemoryHeap[i] != null)
                        _process.Process.MemoryHeap[i] = entityNode.ScriptProcess.MemoryHeap[i].Clone();
                }

                _process.Process.ObjectHeap = new RuntimeObject[entityNode.ScriptProcess.ObjectHeap.Length];
                for (int i = 0; i < entityNode.ScriptProcess.ObjectHeap.Length; i++)
                {
                    if (entityNode.ScriptProcess.ObjectHeap[i] != null)
                        _process.Process.ObjectHeap[i] = entityNode.ScriptProcess.ObjectHeap[i].Clone();
                }
                 * */

                // Copy properties
                foreach (Symbol symbol in entityNode.ScriptProcess.Symbols)
                {
                    if (symbol == null || symbol.Type != SymbolType.Variable) continue;
                    
                    VariableSymbol variable = symbol as VariableSymbol;
                    if (variable.IsProperty == true)
                    {
                        if (variable.DataType.IsArray == true)
                        {
                            int originalArrayIndex = _process.Process[0].GetArrayGlobal(variable.Identifier);
                            if (originalArrayIndex == -1) continue;
                            int originalArraySize = _process.Process[0].GetArrayLength(originalArrayIndex);
                            int newArrayIndex = entityNode.ScriptProcess[0].AllocateArray(variable.DataType.DataType, originalArraySize);
                            if (variable.DataType.DataType == DataType.Object)
                            {
                                for (int i = 0; i < originalArraySize; i++)
                                {
                                    entityNode.ScriptProcess[0].SetArrayElement(newArrayIndex, i, _process.Process[0].GetObjectArrayElement(originalArrayIndex, i));
                                }
                            }
                            else
                            {
                                for (int i = 0; i < originalArraySize; i++)
                                    entityNode.ScriptProcess[0].SetArrayElement(newArrayIndex, i, _process.Process[0].GetRuntimeValueArrayElement(originalArrayIndex, i));
                            }
                        }
                        else
                        {
                            if (variable.DataType.DataType == DataType.Object)
                            {
                                entityNode.ScriptProcess[0].SetGlobalVariable(variable.Identifier, _process.Process[0].GetObjectGlobal(variable.Identifier));
                            }
                            else
                                entityNode.ScriptProcess[0].SetGlobalVariable(variable.Identifier, _process.Process[0].GetRuntimeValueGlobal(variable.Identifier));
                        }
                    }
                }

                entityNode.SyncCollisionEvents();
            }
		}

		/// <summary>
		///		Saves this entity into a given binary writer.
		/// </summary>
		/// <param name="writer">Binary writer to save this entity into.</param>
		public override void Save(BinaryWriter writer)
		{
			// Save all the basic entity details.
			base.Save(writer);

			// Save all the scripted entity specific details.
            writer.Write((_process.Process != null && _process.Process.Url != null));
            if ((_process.Process != null && _process.Process.Url != null))
			{
                writer.Write(_process.Process.Url);

				int propertyCount = 0;
				foreach (Symbol symbol in _process.Process.GlobalScope.Symbols)
				{
					if (symbol is VariableSymbol == false || ((VariableSymbol)symbol).IsProperty == false) continue;
					propertyCount++;
				}
				writer.Write((short)propertyCount);

				// Write in the value of all the properties stored in the process.
				foreach (Symbol symbol in _process.Process.GlobalScope.Symbols)
				{
					if (symbol is VariableSymbol == false || ((VariableSymbol)symbol).IsProperty == false) continue;
					VariableSymbol variableSymbol = symbol as VariableSymbol;

                    // Quickly find out the meta data of this symbol.
                    string editMethod = "";
                    foreach (Symbol subSymbol in symbol.Symbols)
                        if (subSymbol is MetaDataSymbol)
                            if (((MetaDataSymbol)subSymbol).Identifier.ToLower() == "editmethod")
                            {
                                editMethod = ((MetaDataSymbol)subSymbol).Value;
                                break;
                            }

					writer.Write(variableSymbol.Identifier);
					writer.Write((byte)variableSymbol.DataType.DataType);
                    writer.Write(variableSymbol.IsArray);
                    if (variableSymbol.IsArray == true)
                    {
                        int arrayIndex = _process.Process[0].GetArrayGlobal(variableSymbol.Identifier);
                        writer.Write(arrayIndex != 0);
                        if (arrayIndex != 0)
                        {
                            int arrayLength = _process.Process[0].GetArrayLength(arrayIndex);
                            writer.Write(arrayLength);
                            for (int i = 0; i < arrayLength; i++)
                            {
                                switch (variableSymbol.DataType.DataType)
                                {
                                    case DataType.Bool: writer.Write(_process.Process[0].GetBooleanArrayElement(arrayIndex, i)); break;
                                    case DataType.Byte: writer.Write(_process.Process[0].GetByteArrayElement(arrayIndex, i)); break;
                                    case DataType.Double: writer.Write(_process.Process[0].GetDoubleArrayElement(arrayIndex, i)); break;
                                    case DataType.Float: writer.Write(_process.Process[0].GetFloatArrayElement(arrayIndex, i)); break;
                                    case DataType.Int: writer.Write(_process.Process[0].GetIntArrayElement(arrayIndex, i)); break;
                                    case DataType.Long: writer.Write(_process.Process[0].GetLongArrayElement(arrayIndex, i)); break;
                                    case DataType.Short: writer.Write(_process.Process[0].GetShortArrayElement(arrayIndex, i)); break;
                                    case DataType.String: writer.Write(_process.Process[0].GetStringArrayElement(arrayIndex, i)); break;
                                    case DataType.Object:
                                        RuntimeObject obj = _process.Process[0].GetObjectArrayElement(arrayIndex, i); 
                                        if (obj != null)
                                        {
                                            if (editMethod.ToLower() == "image")
                                            {
                                                if (obj is NativeObject && ((Graphics.Image)((NativeObject)obj).Object) != null)
                                                {
                                                    Graphics.Image img = ((Graphics.Image)((NativeObject)obj).Object);
                                                    writer.Write(true);
                                                    writer.Write((byte)0);
                                                    writer.Write(img.URL);
                                                    writer.Write(img.Width);
                                                    writer.Write(img.Height);
                                                    writer.Write((short)img.VerticalSpacing);
                                                    writer.Write((short)img.HorizontalSpacing);
                                                }
                                                else
                                                    writer.Write(false);
                                            }
                                            else if (editMethod.ToLower() == "sound")
                                            {
                                                if (obj is NativeObject && ((Audio.Sound)((NativeObject)obj).Object) != null)
                                                {
                                                    Audio.Sound sound = ((Audio.Sound)((NativeObject)obj).Object);
                                                    writer.Write(true);
                                                    writer.Write((byte)1);
                                                    writer.Write(sound.URL);
                                                    writer.Write(sound.Frequency);
                                                    writer.Write(sound.InnerRadius);
                                                    writer.Write(sound.Looping);
                                                    writer.Write(sound.OuterRadius);
                                                    writer.Write(sound.Pan);
                                                    writer.Write(sound.Volume);
                                                    writer.Write(sound.Streaming);
                                                    writer.Write(sound.Positional);
                                                }
                                                else
                                                    writer.Write(false);
                                            }
                                            else
                                                writer.Write(false);
                                        }
                                        else
                                            writer.Write(false); 
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        switch (variableSymbol.DataType.DataType)
                        {
                            case DataType.Bool: writer.Write(_process.Process[0].GetBooleanGlobal(variableSymbol.Identifier)); break;
                            case DataType.Byte: writer.Write(_process.Process[0].GetByteGlobal(variableSymbol.Identifier)); break;
                            case DataType.Double: writer.Write(_process.Process[0].GetDoubleGlobal(variableSymbol.Identifier)); break;
                            case DataType.Float: writer.Write(_process.Process[0].GetFloatGlobal(variableSymbol.Identifier)); break;
                            case DataType.Int: writer.Write(_process.Process[0].GetIntegerGlobal(variableSymbol.Identifier)); break;
                            case DataType.Long: writer.Write(_process.Process[0].GetLongGlobal(variableSymbol.Identifier)); break;
                            case DataType.Short: writer.Write(_process.Process[0].GetShortGlobal(variableSymbol.Identifier)); break;
                            case DataType.String:
                                string value = _process.Process[0].GetStringGlobal(variableSymbol.Identifier);
                                writer.Write(value == null || value == "" ? "" : value); 
                                break;
                            case DataType.Object:
                                RuntimeObject obj = _process.Process[0].GetObjectGlobal(variableSymbol.Identifier);
                                if (obj != null)
                                {
                                    if (editMethod.ToLower() == "image")
                                    {
                                        if (obj is NativeObject && ((Graphics.Image)((NativeObject)obj).Object) != null)
                                        {
                                            Graphics.Image img = ((Graphics.Image)((NativeObject)obj).Object);
                                            writer.Write(true);
                                            writer.Write((byte)0);
                                            writer.Write(img.URL);
                                            writer.Write(img.Width);
                                            writer.Write(img.Height);
                                            writer.Write((short)img.VerticalSpacing);
                                            writer.Write((short)img.HorizontalSpacing);
                                        }
                                        else
                                            writer.Write(false);
                                    }
                                    else if (editMethod.ToLower() == "sound")
                                    {
                                        if (obj is NativeObject && ((Audio.Sound)((NativeObject)obj).Object) != null)
                                        {
                                            Audio.Sound sound = ((Audio.Sound)((NativeObject)obj).Object);
                                            writer.Write(true);
                                            writer.Write((byte)1);
                                            writer.Write(sound.URL);
                                            writer.Write(sound.Frequency);
                                            writer.Write(sound.InnerRadius);
                                            writer.Write(sound.Looping);
                                            writer.Write(sound.OuterRadius);
                                            writer.Write(sound.Pan);
                                            writer.Write(sound.Volume);
                                            writer.Write(sound.Streaming);
                                            writer.Write(sound.Positional);
                                        }
                                        else
                                            writer.Write(false);
                                    }
                                    else
                                        writer.Write(false);
                                }
                                else
                                    writer.Write(false);
                                break;
                        }
                    }
				}
			}

		}

		/// <summary>
		///		Loads this scene node from a given binary reader.
		/// </summary>
		/// <param name="reader">Binary reader to load this scene node from.</param>
		public override void Load(BinaryReader reader)
		{
			// Load all the basic entity details.
			base.Load(reader);

			// Load all the scripted specific details.
			if (reader.ReadBoolean() == true)
			{
				string url = reader.ReadString();

				// Load the objects script from the memory stream we dumped it into.
				ScriptProcess process = VirtualMachine.GlobalInstance.LoadScript(url);

                #region Property Loading
                int propertyCount = reader.ReadInt16();
				for (int i = 0; i < propertyCount; i++)
				{
					string identifier = reader.ReadString();
					DataTypeValue dataTypeValue = new DataTypeValue((DataType)reader.ReadByte(), false, false);
                    bool isArray = reader.ReadBoolean();

					// Look through the script processes global variables to see if this
					// variable exists.
					VariableSymbol variableSymbol = null;
                    if (process != null)
                    {
                        foreach (Symbol symbol in process.GlobalScope.Symbols)
                        {
                            if (symbol is VariableSymbol == false || ((VariableSymbol)symbol).Identifier != identifier || ((VariableSymbol)symbol).DataType != dataTypeValue) continue;
                            variableSymbol = symbol as VariableSymbol;
                            break;
                        }
                    }

                    // Quickly find out the meta data of this symbol.
                    string editMethod = "";
                    if (variableSymbol != null)
                    {
                        foreach (Symbol subSymbol in variableSymbol.Symbols)
                            if (subSymbol is MetaDataSymbol)
                                if (((MetaDataSymbol)subSymbol).Identifier.ToLower() == "editmethod")
                                {
                                    editMethod = ((MetaDataSymbol)subSymbol).Value;
                                    break;
                                }
                    }

					// Read in value based on data type.
                    if (isArray == true && reader.ReadBoolean() == true)
                    {
                        int arrayLength = reader.ReadInt32();
                        int arrayIndex = process == null ? 0 : process[0].AllocateArray(dataTypeValue.DataType, arrayLength);
                        for (int k = 0; k < arrayLength; k++)
                        {
                            switch (dataTypeValue.DataType)
                            {
                                case DataType.Bool:
                                    {
                                        bool value = reader.ReadBoolean();
                                        if (process != null && variableSymbol != null) process[0].SetArrayElement(arrayIndex, k, value);
                                    }
                                    break;
                                case DataType.Byte:
                                    {
                                        byte value = reader.ReadByte();
                                        if (process != null && variableSymbol != null) process[0].SetArrayElement(arrayIndex, k, value);
                                    }
                                    break;
                                case DataType.Double:
                                    {
                                        double value = reader.ReadDouble();
                                        if (process != null && variableSymbol != null) process[0].SetArrayElement(arrayIndex, k, value);
                                    }
                                    break;
                                case DataType.Float:
                                    {
                                        float value = reader.ReadSingle();
                                        if (process != null && variableSymbol != null) process[0].SetArrayElement(arrayIndex, k, value);
                                    }
                                    break;
                                case DataType.Int:
                                    {
                                        int value = reader.ReadInt32();
                                        if (process != null && variableSymbol != null) process[0].SetArrayElement(arrayIndex, k, value);
                                    }
                                    break;
                                case DataType.Long:
                                    {
                                        long value = reader.ReadInt64();
                                        if (process != null && variableSymbol != null) process[0].SetArrayElement(arrayIndex, k, value);
                                    }
                                    break;
                                case DataType.Short:
                                    {
                                        short value = reader.ReadInt16();
                                        if (process != null && variableSymbol != null) process[0].SetArrayElement(arrayIndex, k, value);
                                    }
                                    break;
                                case DataType.String:
                                    {
                                        string value = reader.ReadString();
                                        if (process != null && variableSymbol != null) process[0].SetArrayElement(arrayIndex, i, value);
                                    }
                                    break;
                                case DataType.Object:
                                    {
                                        if (reader.ReadBoolean() == true)
                                        {
                                            int type = reader.ReadByte();
                                            if (type == 0)
                                            {
                                                string imageUrl = reader.ReadString();
                                                int cellWidth = reader.ReadInt32();
                                                int cellHeight = reader.ReadInt32();
                                                int hSpacing = reader.ReadInt16();
                                                int vSpacing = reader.ReadInt16();
                                                if (process != null && variableSymbol != null) process[0].SetArrayElement(arrayIndex, i, new Fusion.Engine.ScriptingFunctions.ImageScriptObject(GraphicsManager.LoadImage(imageUrl, cellWidth, cellHeight, hSpacing, vSpacing, 0)));
                                            }
                                            else if (type == 1)
                                            {
                                                string soundUrl = reader.ReadString();
                                                int freq = reader.ReadInt32();
                                                float innerRadius = reader.ReadSingle();
                                                float outerRadius = reader.ReadSingle();
                                                bool loop = reader.ReadBoolean();
                                                float pan = reader.ReadSingle();
                                                float volume = reader.ReadSingle();
                                                bool streaming = reader.ReadBoolean();
                                                bool positional = reader.ReadBoolean();
                                                Audio.SoundFlags flags = 0;
                                                if (streaming == true) flags |= Audio.SoundFlags.Streamed;
                                                if (positional == true) flags |= Audio.SoundFlags.Positional;

                                                Audio.Sound sound = Audio.AudioManager.LoadSound(soundUrl, flags);
                                                sound.Frequency = freq;
                                                sound.InnerRadius = innerRadius;
                                                sound.OuterRadius = outerRadius;
                                                sound.Looping = loop;
                                                sound.Pan = pan;
                                                sound.Volume = volume;
                                                if (process != null && variableSymbol != null) process[0].SetArrayElement(arrayIndex, i, new Fusion.Engine.ScriptingFunctions.SoundScriptObject(sound));
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        switch (dataTypeValue.DataType)
                        {
                            case DataType.Bool:
                                {
                                    bool value = reader.ReadBoolean();
                                    if (process != null && variableSymbol != null) process[0].SetGlobalVariable(identifier, value);
                                }
                                break;
                            case DataType.Byte:
                                {
                                    byte value = reader.ReadByte();
                                    if (process != null && variableSymbol != null) process[0].SetGlobalVariable(identifier, value);
                                }
                                break;
                            case DataType.Double:
                                {
                                    double value = reader.ReadDouble();
                                    if (process != null && variableSymbol != null) process[0].SetGlobalVariable(identifier, value);
                                }
                                break;
                            case DataType.Float:
                                {
                                    float value = reader.ReadSingle();
                                    if (process != null && variableSymbol != null) process[0].SetGlobalVariable(identifier, value);
                                }
                                break;
                            case DataType.Int:
                                {
                                    int value = reader.ReadInt32();
                                    if (process != null && variableSymbol != null) process[0].SetGlobalVariable(identifier, value);
                                }
                                break;
                            case DataType.Long:
                                {
                                    long value = reader.ReadInt64();
                                    if (process != null && variableSymbol != null) process[0].SetGlobalVariable(identifier, value);
                                }
                                break;
                            case DataType.Short:
                                {
                                    short value = reader.ReadInt16();
                                    if (process != null && variableSymbol != null) process[0].SetGlobalVariable(identifier, value);
                                }
                                break;
                            case DataType.String:
                                {
                                    string value = reader.ReadString();
                                    if (process != null && variableSymbol != null) process[0].SetGlobalVariable(identifier, value);
                                }
                                break;
                            case DataType.Object:
                                {
                                    if (reader.ReadBoolean() == true)
                                    {
                                        int type = reader.ReadByte();
                                        if (type == 0)
                                        {
                                            string imageUrl = reader.ReadString();
                                            int cellWidth = reader.ReadInt32();
                                            int cellHeight = reader.ReadInt32();
                                            int hSpacing = reader.ReadInt16();
                                            int vSpacing = reader.ReadInt16();
                                            if (ResourceManager.ResourceExists(imageUrl) == true && process != null && variableSymbol != null)
                                                process[0].SetGlobalVariable(identifier, new Fusion.Engine.ScriptingFunctions.ImageScriptObject(GraphicsManager.LoadImage(imageUrl, cellWidth, cellHeight, hSpacing, vSpacing, 0)));
                                        }
                                        else if (type == 1)
                                        {
                                            string soundUrl = reader.ReadString();
                                            int freq = reader.ReadInt32();
                                            float innerRadius = reader.ReadSingle();
                                            float outerRadius = reader.ReadSingle();
                                            bool loop = reader.ReadBoolean();
                                            float pan = reader.ReadSingle();
                                            float volume = reader.ReadSingle();
                                            bool streaming = reader.ReadBoolean();
                                            bool positional = reader.ReadBoolean();
                                            Audio.SoundFlags flags = 0;
                                            if (streaming == true) flags |= Audio.SoundFlags.Streamed;
                                            if (positional == true) flags |= Audio.SoundFlags.Positional;

                                            if (ResourceManager.ResourceExists(soundUrl) == true && process != null && variableSymbol != null)
                                            {
                                                Audio.Sound sound = Audio.AudioManager.LoadSound(soundUrl, flags);
                                                sound.Frequency = freq;
                                                sound.InnerRadius = innerRadius;
                                                sound.OuterRadius = outerRadius;
                                                sound.Looping = loop;
                                                sound.Pan = pan;
                                                sound.Volume = volume;
                                                process[0].SetGlobalVariable(identifier, new Fusion.Engine.ScriptingFunctions.SoundScriptObject(sound));
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
                #endregion

                // Now set the process!
                _process.Process = process;
                if (_process.Process != null) _process.Process.OnStateChange += OnStateChange;
                if (_process.Process != null && _process.Process.State != null) OnStateChange(_process.Process, _process.Process.State);

                SyncCollisionEvents();
            }
		}

        /// <summary>
        ///     Called when this entity is triggered by another entities event.
        /// </summary>
        /// <param name="node">The node that triggered this one.</param>
        protected override void OnTrigger(EntityNode node)
        {
            if (node == null) return;
            if (_process != null && _process.Process != null && _enabled == true)
            {
                _process.Process[0].PassParameter(new SceneNodeScriptObject(node));
                _process.Process[0].InvokeFunction("OnTrigger", true, false);
            }
            TriggerEvents();
        }

        /// <summary>
        ///     Called by this entitys collision polygon when this entity touches another.
        /// </summary>
        /// <param name="aPolygon">Entity that touched.</param>
        /// <param name="bPolygon">Entity that was touched.</param>
        protected void OnTouch(CollisionPolygon aPolygon, CollisionPolygon bPolygon)
        {
            if (_process == null || _process.Process == null || aPolygon.MetaData == null || bPolygon.MetaData == null || _process.Process == null || _enabled == false) return;
            _process.Process[0].PassParameter(new SceneNodeScriptObject((SceneNode)aPolygon.MetaData));
            _process.Process[0].PassParameter(new SceneNodeScriptObject((SceneNode)bPolygon.MetaData));
            _process.Process[0].InvokeFunction("OnTouch", true, false);
            //base.OnTouch(aPolygon, bPolygon);
        }

        /// <summary>
        ///     Called by this entitys collision polygon when this entity enters another.
        /// </summary>
        /// <param name="aPolygon">Entity that entered.</param>
        /// <param name="bPolygon">Entity that was entered.</param>
        protected void OnEnter(CollisionPolygon aPolygon, CollisionPolygon bPolygon)
        {
            if (_process == null || _process.Process == null || aPolygon.MetaData == null || bPolygon.MetaData == null || _process.Process == null || _enabled == false) return;
            _process.Process[0].PassParameter(new SceneNodeScriptObject((SceneNode)aPolygon.MetaData));
            _process.Process[0].PassParameter(new SceneNodeScriptObject((SceneNode)bPolygon.MetaData));
            _process.Process[0].InvokeFunction("OnEnter", true, false);
            //base.OnEnter(aPolygon, bPolygon);
        }

        /// <summary>
        ///     Called by this entitys collision polygon when this entity leaves another.
        /// </summary>
        /// <param name="aPolygon">Entity that left.</param>
        /// <param name="bPolygon">Entity that was left.</param>
        protected void OnLeave(CollisionPolygon aPolygon, CollisionPolygon bPolygon)
        {
            if (_process == null || _process.Process == null || aPolygon.MetaData == null || bPolygon.MetaData == null || _process.Process == null || _enabled == false) return;
            _process.Process[0].PassParameter(new SceneNodeScriptObject((SceneNode)aPolygon.MetaData));
            _process.Process[0].PassParameter(new SceneNodeScriptObject((SceneNode)bPolygon.MetaData));
            _process.Process[0].InvokeFunction("OnLeave", true, false);
           // base.OnLeave(aPolygon, bPolygon);
        }

        /// <summary>
        ///     Called by this entitys collision polygon is colliding with another.
        /// </summary>
        /// <param name="aPolygon">Entity that has touched.</param>
        /// <param name="bPolygon">Entity that is being touched.</param>
        protected void OnColliding(CollisionPolygon aPolygon, CollisionPolygon bPolygon)
        {
            if (_process == null || _process.Process == null || aPolygon.MetaData == null || bPolygon.MetaData == null || _process.Process == null || _enabled == false) return;
            _process.Process[0].PassParameter(new SceneNodeScriptObject((SceneNode)aPolygon.MetaData));
            _process.Process[0].PassParameter(new SceneNodeScriptObject((SceneNode)bPolygon.MetaData));
            _process.Process[0].InvokeFunction("OnColliding", true, false);
            // base.OnLeave(aPolygon, bPolygon);
        }

        /// <summary>
        ///     Creates callback events for any collision events we need.
        /// </summary>
        private void SyncCollisionEvents()
        {
            bool hasEnter = false, hasLeave = false, hasTouch = false, hasColliding = false;

            if (_process != null && _process.Process != null)
            {
                // Check global scope for events.
                foreach (Symbol symbol in _process.Process.Symbols)
                    if (symbol.Type == SymbolType.Function && ((FunctionSymbol)symbol).IsEvent == true)
                    {
                        switch (symbol.Identifier.ToLower())
                        {
                            case "ontouch": hasTouch = true; break;
                            case "onenter": hasEnter = true; break;
                            case "onleave": hasLeave = true; break;
                            case "oncolliding": hasColliding = true; break;
                        }
                    }

                // Check state scope for events.
                /*
                if (_process.Process.State != null)
                {
                    foreach (Symbol symbol in _process.Process.State.Symbols)
                        if (symbol.Type == SymbolType.Function && ((FunctionSymbol)symbol).IsEvent == true)
                        {
                            switch (symbol.Identifier.ToLower())
                            {
                                case "ontouch": hasTouch = true; break;
                                case "onenter": hasEnter = true; break;
                                case "onleave": hasLeave = true; break;
                                case "oncolliding": hasColliding = true; break;
                            }
                        }
                }      */
            }

            if (hasEnter == true)
                _collisionPolygon.OnEnterDelegate += new CollisionNotificationDelegate(OnEnter);
            else
                _collisionPolygon.OnEnterDelegate = null;

            if (hasLeave == true)
                _collisionPolygon.OnLeaveDelegate += new CollisionNotificationDelegate(OnLeave);
            else
                _collisionPolygon.OnLeaveDelegate = null;

            if (hasTouch == true)
                _collisionPolygon.OnTouchDelegate += new CollisionNotificationDelegate(OnTouch);
            else
                _collisionPolygon.OnTouchDelegate = null;

            if (hasColliding == true)
                _collisionPolygon.OnCollidingDelegate += new CollisionNotificationDelegate(OnColliding);
            else
                _collisionPolygon.OnCollidingDelegate = null;
        }

		/// <summary>
		///		Initializes a new instance and gives it the specified  
		///		name.
		/// </summary>
		/// <param name="name">Name to name as node to.</param>
		public ScriptedEntityNode(string name)
		{
			_name = name;
			_process = new ScriptExecutionProcess(this);
			ProcessManager.AttachProcess(_process);
            SyncCollisionEvents();
        }
		public ScriptedEntityNode()
		{
			_name = "Scripted Entity";
			_process = new ScriptExecutionProcess(this);
			ProcessManager.AttachProcess(_process);
            SyncCollisionEvents();
        }

		#endregion
	}

}