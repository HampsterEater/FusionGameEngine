/*
 * File: Entity Properties Window.cs
 *
 * Contains all the functional partial code declaration for the EntityPropertiesWindow form.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */ 

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using BinaryPhoenix.Fusion.Graphics;
using BinaryPhoenix.Fusion.Engine.Entitys;
using BinaryPhoenix.Fusion.Engine.Windows;
using BinaryPhoenix.Fusion.Runtime.Controls;
using BinaryPhoenix.Fusion.Runtime.Scripting;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Runtime.Resources;

namespace BinaryPhoenix.Fusion.Editor.Windows
{
    /// <summary>
    ///     Used to register and invoke events by the EntityPropertiesWindow class when a property is changed.
    /// </summary>
    /// <param name="source">Instance of EntityPropertyWindow that event originated from.</param>
    /// <param name="propertyName">Name of property that was changed.</param>
    /// <param name="value">Value that property was changed to.</param>
    public delegate void PropertyChangedEventHandler(object source, string propertyName, object value);

	/// <summary>
	///		Contains the code required to initialize and run an window to show entity properties.
	/// </summary>
	public partial class EntityPropertiesWindow : Form
	{
		#region Members
		#region Variables

		private EntityNode _entity = null;

        private object[] _categories = null;

        private PropertyChangedEventHandler _propertyChangedDelegate = null;

		#endregion
		#region Events

		#endregion
		#region Properties

        /// <summary>
        ///     Gets or sets the delegate that is invoked when a property is changed.
        /// </summary>
        public PropertyChangedEventHandler PropertyChangedDelegate
        {
            get { return _propertyChangedDelegate; }
            set { _propertyChangedDelegate = value; }
        }

		/// <summary>
		///		Gets or sets the entity this window is bound to.
		/// </summary>
		public EntityNode Entity
		{
			get { return _entity; }
			set 
			{ 
				_entity = value; 
				SyncronizeData();
			}
		}

		#endregion
		#endregion
		#region Methods

        /// <summary>
        ///     Called when the property list view wishs to set the value of an item.
        /// </summary>
        /// <param name="item">Item to set value of.</param>
        /// <param name="value">Value to set item as.</param>
        private void SetValue(PropertyListViewItem item, object value)
        {
            // If the value is a file editor we need to grab its file URL.
            if (value is FileEditorValue)
                value = ((FileEditorValue)value).FileUrl;

            // If its an enumeration get its enumeration value instead.
            if (item.EnumerationValues != null)
                value = item.EnumerationValue;

            // Go through non-dynamic items.
            switch (item.Name.ToLower())
            {
                // General
                case "name":        
                    _entity.Name = value as string;
                    Engine.Engine.GlobalInstance.Map.SceneGraph.SyncronizeNodes();
                    return;
                case "event":       
                    _entity.Event = value as string;
                    _entity.EventNodes = Engine.Engine.GlobalInstance.Map.SceneGraph.GetNodesByName(_entity.Event);
                    return;

                case "enabled":     _entity.IsEnabled = (bool)value; return;

                // Rendering
                case "visible":     _entity.IsVisible = (bool)value; return;
                case "color":
                    {
                        Color color = (Color)value;
                        int a, r, g, b;
                        ColorMethods.SplitColor(ColorFormats.A8R8G8B8, _entity.Color, out r, out g, out b, out a);
                        _entity.Color = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, color.R, color.G, color.B, a);
                        return;
                    }
                case "alpha":
                    {
                        int a, r, g, b;
                        ColorMethods.SplitColor(ColorFormats.A8R8G8B8, _entity.Color, out r, out g, out b, out a);
                        _entity.Color = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, r, g, b, (byte)value);
                        return;
                    }
                case "blend mode":  _entity.BlendMode = (BlendMode)value; return;
                case "render mode": _entity.RenderMode = (EntityRenderMode)value; return;
                case "image":
                    {
                        _entity.Image = value as Graphics.Image;

                        if (_entity.Image != null)
                        {
                            _entity.BoundingRectangle = new Rectangle(0, 0, _entity.Image.Width, _entity.Image.Height);
                            _entity.CollisionRectangle = new Rectangle(0, 0, _entity.Image.Width, _entity.Image.Height);
                        }
                        
                        return;
                    }
                case "mesh": _entity.Mesh = (string)value == "" ? null : new Mesh(Engine.Engine.GlobalInstance.MediaPath+ "\\" + value, 0); return;
                case "frame":      _entity.Frame = (int)value; return;

                // Collision
                case "solid":               _entity.IsSolid = (bool)value; return;
                case "collision rectangle": _entity.CollisionRectangle = (Rectangle)value; return;
                case "collision layers":    _entity.CollisionPolygon.Layers = value as int[]; return;

                // Text
                case "text":                _entity.Text = (string)value; return;
                case "bitmap font":         _entity.Font = value as BitmapFont; return;
                case "shader":              _entity.Shader = (string)value == "" ? null : new Shader(Engine.Engine.GlobalInstance.ShaderPath + "\\" + value); return;

                // Transformation
                case "location":            _entity.Position(((Point)value).X, ((Point)value).Y, _entity.Transformation.Z); return;
                case "scale":               _entity.Scale(((Vector)value).X, ((Vector)value).Y, ((Vector)value).Z); return;
                case "angle":               _entity.Rotate(((Vector)value).X, ((Vector)value).Y, ((Vector)value).Z); return;
                case "z-offset":            _entity.Position(_entity.Transformation.X, _entity.Transformation.Y, (float)value); return;
                case "z-layer":             _entity.DepthLayer = (int)value; return;
                case "depth mode":          _entity.DepthMode = (EntityDepthMode)value; return;      
                case "bounding rectangle":  _entity.BoundingRectangle = (Rectangle)value; return;
            }   

            // Go through dynamic items.
            if (_entity is ScriptedEntityNode)
            {
                ScriptedEntityNode scriptedEntity = (ScriptedEntityNode)_entity;
                foreach (Symbol symbol in scriptedEntity.ScriptProcess.GlobalScope.Symbols)
                {
                    if (!(symbol is VariableSymbol)) continue;

                    string name = ((VariableSymbol)symbol).Identifier;
                    string editMethod = "";
                    foreach (Symbol subSymbol in symbol.Symbols)
                        if (subSymbol is MetaDataSymbol)
                            if (((MetaDataSymbol)subSymbol).Identifier.ToLower() == "name")
                                name = ((MetaDataSymbol)subSymbol).Value;
                            else if (((MetaDataSymbol)subSymbol).Identifier.ToLower() == "editmethod")
                                editMethod = ((MetaDataSymbol)subSymbol).Value;
                        

                    if (((VariableSymbol)symbol).IsProperty == true && name.ToLower() == item.Name.ToLower())
                    {
                        VariableSymbol variableSymbol = ((VariableSymbol)symbol);
                        if (variableSymbol.DataType.IsArray == false)
                        {
                            switch (variableSymbol.DataType.DataType)
                            {
                                case DataType.Bool: scriptedEntity.ScriptProcess[0].SetGlobalVariable(variableSymbol.Identifier, (bool)value); break;
                                case DataType.Byte: scriptedEntity.ScriptProcess[0].SetGlobalVariable(variableSymbol.Identifier, (byte)value); break;
                                case DataType.Double: scriptedEntity.ScriptProcess[0].SetGlobalVariable(variableSymbol.Identifier, (double)value); break;
                                case DataType.Float: scriptedEntity.ScriptProcess[0].SetGlobalVariable(variableSymbol.Identifier, (float)value); break;
                                case DataType.Int: scriptedEntity.ScriptProcess[0].SetGlobalVariable(variableSymbol.Identifier, (int)value); break;
                                case DataType.Long: scriptedEntity.ScriptProcess[0].SetGlobalVariable(variableSymbol.Identifier, (long)value); break;
                                case DataType.Short: scriptedEntity.ScriptProcess[0].SetGlobalVariable(variableSymbol.Identifier, (short)value); break;
                                case DataType.String: scriptedEntity.ScriptProcess[0].SetGlobalVariable(variableSymbol.Identifier, (string)value); break;
                                case DataType.Object:
                                    if (editMethod.ToLower() == "image")
                                    {
                                        scriptedEntity.ScriptProcess[0].SetGlobalVariable(variableSymbol.Identifier, value == null ? (RuntimeObject)null : new Engine.ScriptingFunctions.ImageScriptObject(value as Graphics.Image)); break;
                                    }
                                    else if (editMethod.ToLower() == "sound")
                                    {
                                        scriptedEntity.ScriptProcess[0].SetGlobalVariable(variableSymbol.Identifier, value == null ? (RuntimeObject)null : new Engine.ScriptingFunctions.SoundScriptObject(value as Audio.Sound)); break;
                                    }
                                    break;

                                default: continue;
                            }
                        }
                        else
                        {
                            object[] array = (object[])value;
                            int arrayIndex = scriptedEntity.ScriptProcess[0].AllocateArray(variableSymbol.DataType.DataType, array.Length);
                            for (int i = 0; i < array.Length; i++)
                            {
                                switch (variableSymbol.DataType.DataType)
                                {
                                    case DataType.Bool: scriptedEntity.ScriptProcess[0].SetArrayElement(arrayIndex, i, (bool)array[i]); break;
                                    case DataType.Byte: scriptedEntity.ScriptProcess[0].SetArrayElement(arrayIndex, i, (byte)array[i]); break;
                                    case DataType.Double: scriptedEntity.ScriptProcess[0].SetArrayElement(arrayIndex, i, (double)array[i]); break;
                                    case DataType.Float: scriptedEntity.ScriptProcess[0].SetArrayElement(arrayIndex, i, (float)array[i]); break;
                                    case DataType.Int: scriptedEntity.ScriptProcess[0].SetArrayElement(arrayIndex, i, (int)array[i]); break;
                                    case DataType.Long: scriptedEntity.ScriptProcess[0].SetArrayElement(arrayIndex, i, (long)array[i]); break;
                                    case DataType.Short: scriptedEntity.ScriptProcess[0].SetArrayElement(arrayIndex, i, (short)array[i]); break;
                                    case DataType.String: scriptedEntity.ScriptProcess[0].SetArrayElement(arrayIndex, i, (string)array[i]); break;

                                    // Needs fixing - What if we have other items apart from images stored in this variable?
                                    case DataType.Object: 
                                        if (editMethod.ToLower() == "image")
                                        {
                                            scriptedEntity.ScriptProcess[0].SetArrayElement(arrayIndex, i, array[i] == null ? (RuntimeObject)null : new Engine.ScriptingFunctions.ImageScriptObject(array[i] as Graphics.Image)); break;
                                        }
                                        else if (editMethod.ToLower() == "image")
                                        {
                                            scriptedEntity.ScriptProcess[0].SetArrayElement(arrayIndex, i, array[i] == null ? (RuntimeObject)null : new Engine.ScriptingFunctions.SoundScriptObject(array[i] as Audio.Sound)); break;
                                        }
                                        break;

                                    default: continue;
                                }
                            }
                        }
                        scriptedEntity.OnPropertyChange(symbol.Identifier);
                    }
                }
            }

            else if (_entity is EmitterNode)
            {
                // Unneccessary, the emitter editor deals with this.
            }

            else if (_entity is TilemapSegmentNode)
            {
                TilemapSegmentNode segmentNode = _entity as TilemapSegmentNode;

                switch (item.Name.ToLower())
                {
                    case "grid visible": segmentNode.IsGridVisible = (bool)value; break;
                    case "tile size":    segmentNode.TileWidth = ((Size)value).Width; segmentNode.TileHeight = ((Size)value).Height; break;
                }
            }

            else if (_entity is PathMarkerNode)
            {
                PathMarkerNode markerNode = _entity as PathMarkerNode;

                switch (item.Name.ToLower())
                {
                    case "speed": markerNode.Speed = (StartFinishF)value; break;
                    case "delay": markerNode.Delay = (int)value; break;
                    case "next marker name": 
                        markerNode.NextNodeName = (string)value;
                        ArrayList nodes = Engine.Engine.GlobalInstance.Map.SceneGraph.GetNodesByName(markerNode.NextNodeName);
                        markerNode.NextNode = (nodes.Count != 0) ? nodes[0] as PathMarkerNode : null;
                        break;
                }
            }

            // Call property changed delegate.
            _propertyChangedDelegate(this, item.Name, value);
        }

        /// <summary>
        ///     Updates the value of a given property.
        /// </summary>
        /// <param name="name">Name of property to update.</param>
        /// <param name="value">Value to set property as.</param>
        public void UpdateProperty(string name, object value)
        {
            foreach (object[] category in _categories)
                if (category.Length > 0)
                    for (int i = 1; i < category.Length; i++) 
                    {
                        PropertyListViewItem item = (PropertyListViewItem)category[i];
                        if (item.Name.ToLower() == name.ToLower())
                        {
                            item.Value = value;
                            _propertyChangedDelegate(this, item.Name, value);
                            propertyListView.Refresh();
                            return;
                        }
                    }
        }

		/// <summary>
		///		Syncronizes the data in property list with that contained in the entity.
		/// </summary>
		public void SyncronizeData()
		{
            // Clear the list view so its ready for some new items.
            propertyListView.Clear();
            
            // If we have no entity at the moment then just disable this control and return.
            if (_entity == null)
            {
                propertyListView.Enabled = false;
                _categories = null;
                propertyListView.Refresh();
                return;
            }
            
            // Set the default values of this list view.
            propertyListView.Enabled = true;
            propertyListView.DefaultItem = "Test";

            // Split up the color of our entity to make the rendering category 
            // easier to create.
            int a, r, g, b;
            ColorMethods.SplitColor(ColorFormats.A8R8G8B8, _entity.Color, out r, out g, out b, out a);

            // Create a list of categories and properties to go in them.
            _categories = new object[]
            {
                // This is our general category.
                new object[] 
                {
                    new PropertyListViewCategory("General"),
                    new PropertyListViewItem("Name", _entity.Name, "", "Name of this entity. Used by the event system to identify and trigger this entity if neccessary.", typeof(string)),            
                    new PropertyListViewItem("Event", _entity.Event, "", "Name of entity to trigger when this entity is triggered.", typeof(string)),            
                    new PropertyListViewItem("Enabled", _entity.IsEnabled, true, "Indicates whether this entity is enabled.", typeof(bool)),            
                },

                // This is our transformation category.
                new object[] 
                {
                    new PropertyListViewCategory("Transformation"),
                    new PropertyListViewItem("Location", new Point((int)_entity.Transformation.X, (int)_entity.Transformation.Y), new Point(), "The location of this entity.", typeof(Point)),            
                    new PropertyListViewItem("Z-Offset", _entity.Transformation.Z, 0.0f, "This value is added to this entities resulting depth, you can use it to set a depth origin.", typeof(float)),            
                    new PropertyListViewItem("Z-Layer", _entity.DepthLayer, 0, "Determines which depth layer this entity is render one.", typeof(int)),            
                    new PropertyListViewItem("Depth Mode", _entity.DepthMode, (int)EntityDepthMode.Normal, "Determines how the depth of this entity is worked out.", typeof(EntityDepthMode)),            
                    new PropertyListViewItem("Scale", new Vector(_entity.Transformation.ScaleX, _entity.Transformation.ScaleY, _entity.Transformation.ScaleZ), new Vector(), "The scale of this entity.", typeof(Vector)),                       
                    new PropertyListViewItem("Angle", new Vector(_entity.Transformation.AngleX, _entity.Transformation.AngleY, _entity.Transformation.AngleZ), new Vector(), "The rotational angle of this entity in degrees.", typeof(Vector)),                       
                    new PropertyListViewItem("Bounding Rectangle", _entity.BoundingRectangle, new Rectangle(0,0,0,0), "The bounding box of this entity. The bounding box is used for determining how an entity should be culled to save rendering time.", typeof(Rectangle)),                               
                },
            };

            // See if we can insert the collision group.
            if (_entity.CollisionPolygon != null)
            {
                 object[] newCategories = new object[_categories.Length + 1];
                _categories.CopyTo(newCategories, 0);
                _categories = newCategories;
                _categories[_categories.Length - 1] = new object[] 
                {
                    new PropertyListViewCategory("Collision"),
                    new PropertyListViewItem("Solid", _entity.IsSolid, true, "Indicates whether this entity can respond to collisions.", typeof(bool)),    
                    new PropertyListViewItem("Collision Layers", _entity.CollisionPolygon.Layers, new int[] { 0 }, "Indicates which collision layers this entity can collide with.", typeof(int[])),    
                    new PropertyListViewItem("Collision Rectangle", _entity.CollisionRectangle, new Rectangle(0,0,0,0), "Specifies the dimensions of the collision box of this entity.", typeof(Rectangle)),    
                };
            }

            // Add the text and rendering categories as well.
            if (!(_entity is TilemapSegmentNode) && !(_entity is EmitterNode))
            {
                object[] newCategories = new object[_categories.Length + 2];
                _categories.CopyTo(newCategories, 0);
                _categories = newCategories;
                _categories[_categories.Length - 1] = new object[] 
                {
                    new PropertyListViewCategory("Rendering"),
                    new PropertyListViewItem("Visible", _entity.IsVisible, true, "Indicates whether this entity is visible.", typeof(bool)),    
                    new PropertyListViewItem("Color", Color.FromArgb(ColorMethods.CombineColor(ColorFormats.A8R8G8B8, r, g, b, 255)), Color.FromArgb(unchecked((int)0xFFFFFFFF)), "Indicates the color this entity should be tinted.", typeof(Color)),            
                    new PropertyListViewItem("Alpha", a, (byte)255, "Indicates the transparency this entity should be rendered at.", typeof(byte)),            
                    new PropertyListViewItem("Blend Mode", _entity.BlendMode, BlendMode.Alpha, "Indicates the blending mode this entity should be rendered with.", typeof(BlendMode)),            
                    new PropertyListViewItem("Render Mode", _entity.RenderMode, EntityRenderMode.None, "Indicates how this entity should be rendered.", typeof(EntityRenderMode)),  
                    new PropertyListViewItem("Image", _entity.Image, null, "The image that should be rendered if this entitys rendering mode is set to render an image.", typeof(string), typeof(ImageEditor)),  
                    new PropertyListViewItem("Frame", _entity.Frame, 0, "The frame of this entitys image that should be rendered when render mode is set to an image mode.", typeof(int)),                                                                                                                             
                    new PropertyListViewItem("Shader", new FileEditorValue(_entity.Shader == null ? "" : _entity.Shader.URL.ToString(), "Fusion Shader Files|*.xml", Environment.CurrentDirectory +"\\"+ Engine.Engine.GlobalInstance.ShaderPath), null, "The shader that this entity should apply to itself when rendering.", typeof(string), typeof(FileEditor)),                                                                                                                             
                    new PropertyListViewItem("Mesh", new FileEditorValue(_entity.Mesh == null ? "" : _entity.Mesh.URL.ToString(), "Mesh Files|*.raw", Environment.CurrentDirectory +"\\"+ Engine.Engine.GlobalInstance.MediaPath), null, "The mesh that this entity should use to render if its render mode is set to MESH.", typeof(string), typeof(FileEditor)),                                     
                };

               // ((PropertyListViewItem)(((object[])_categories[_categories.Length - 1])[6])).Attributes = new Attribute[] { new ReadOnlyAttribute(true) };

                // This is our text category.
                _categories[_categories.Length - 2] = new object[] 
                {
                    new PropertyListViewCategory("Text"),
                    new PropertyListViewItem("Text", _entity.Text, "", "The string of text that this entity should render if its render mode is set to Text.", typeof(string)),            
                    new PropertyListViewItem("Bitmap Font", _entity.Font, null, "Bitmap font used to render text when this entitys render mode is set to Text.", typeof(string), typeof(BitmapFontEditor)),            
                };

               // ((PropertyListViewItem)(((object[])_categories[_categories.Length - 2])[2])).Attributes = new Attribute[] { new ReadOnlyAttribute(true) };
            }
            else if (_entity is TilemapSegmentNode)
            {
                object[] newCategories = new object[_categories.Length + 1];
                _categories.CopyTo(newCategories, 0);
                _categories = newCategories;
                _categories[_categories.Length - 1] = new object[] 
                {
                    new PropertyListViewCategory("Rendering"),
                    new PropertyListViewItem("Visible", _entity.IsVisible, true, "Indicates whether this entity is visible.", typeof(bool)),    
                    new PropertyListViewItem("Color", Color.FromArgb(ColorMethods.CombineColor(ColorFormats.A8R8G8B8, r, g, b, 255)), Color.FromArgb(unchecked((int)0xFFFFFFFF)), "Indicates the color this entity should be tinted.", typeof(Color)),            
                    new PropertyListViewItem("Alpha", a, (byte)255, "Indicates the transparency this entity should be rendered at.", typeof(byte)),            
                    new PropertyListViewItem("Blend Mode", _entity.BlendMode, BlendMode.Alpha, "Indicates the blending mode this entity should be rendered with.", typeof(BlendMode)),            
                };
            }
            else if (_entity is EmitterNode)
            {
                object[] newCategories = new object[_categories.Length + 1];
                _categories.CopyTo(newCategories, 0);
                _categories = newCategories;
                _categories[_categories.Length - 1] = new object[] 
                {
                    new PropertyListViewCategory("Rendering"),
                    new PropertyListViewItem("Visible", _entity.IsVisible, true, "Indicates whether this entity is visible.", typeof(bool)),    
                };
            }

            // Specialist properties.
            if (_entity is ScriptedEntityNode)
            {
                #region Script properties

                // Add the new dynamic category to the category list.
                ArrayList categoryList = new ArrayList();
                categoryList.Add(new PropertyListViewCategory("Script"));
                object[] newCategories = new object[_categories.Length + 1];
                _categories.CopyTo(newCategories, 0);
                _categories = newCategories;

                // Insert all the properties contained in the scripts.
                ScriptedEntityNode scriptedEntity = (ScriptedEntityNode)_entity;
				foreach (Symbol symbol in scriptedEntity.ScriptProcess.GlobalScope.Symbols)
				{
                    if (symbol is VariableSymbol && ((VariableSymbol)symbol).IsProperty == true)
                    {
                        VariableSymbol variableSymbol = ((VariableSymbol)symbol);
                        object value = null;

                        if (variableSymbol.DataType.IsArray == false)
                        {
                            switch (variableSymbol.DataType.DataType)
                            {
                                case DataType.Bool: value = scriptedEntity.ScriptProcess[0].GetBooleanGlobal(variableSymbol.Identifier); break;
                                case DataType.Byte: value = scriptedEntity.ScriptProcess[0].GetByteGlobal(variableSymbol.Identifier); break;
                                case DataType.Double: value = scriptedEntity.ScriptProcess[0].GetDoubleGlobal(variableSymbol.Identifier); break;
                                case DataType.Float: value = scriptedEntity.ScriptProcess[0].GetFloatGlobal(variableSymbol.Identifier); break;
                                case DataType.Int: value = scriptedEntity.ScriptProcess[0].GetIntegerGlobal(variableSymbol.Identifier); break;
                                case DataType.Long: value = scriptedEntity.ScriptProcess[0].GetLongGlobal(variableSymbol.Identifier); break;
                                case DataType.Short: value = scriptedEntity.ScriptProcess[0].GetShortGlobal(variableSymbol.Identifier); break;
                                case DataType.String:
                                    value = scriptedEntity.ScriptProcess[0].GetStringGlobal(variableSymbol.Identifier);
                                    if (value == null) value = "";
                                    break;
                                case DataType.Object:
                                    value = scriptedEntity.ScriptProcess[0].GetObjectGlobal(variableSymbol.Identifier);
                                    if (value is NativeObject)
                                        value = ((NativeObject)value).Object;
                                    break;
                                //default: continue;
                            }
                        }
                        else
                        {
                            int arrayIndex = scriptedEntity.ScriptProcess[0].GetArrayGlobal(variableSymbol.Identifier);
                            int arrayLength = 0;
                            if (arrayIndex == 0)
                                arrayLength = 1;
                            else
                                arrayLength = scriptedEntity.ScriptProcess[0].GetArrayLength(arrayIndex);

                            switch (variableSymbol.DataType.DataType)
                            {
                                case DataType.Bool: value = new bool[arrayLength]; break;
                                case DataType.Byte: value = new byte[arrayLength]; break;
                                case DataType.Double: value = new double[arrayLength]; break;
                                case DataType.Float: value = new float[arrayLength]; break;
                                case DataType.Int: value = new int[arrayLength]; break;
                                case DataType.Long: value = new long[arrayLength]; break;
                                case DataType.Short: value = new short[arrayLength]; break;
                                case DataType.String: value = new string[arrayLength]; break;
                                case DataType.Object: value = new object[arrayLength]; break;
                            }

                            if (arrayIndex != 0)
                            {
                                for (int i = 0; i < arrayLength; i++)
                                {
                                    switch (variableSymbol.DataType.DataType)
                                    {
                                        case DataType.Bool: ((object[])value)[i] = scriptedEntity.ScriptProcess[0].GetBooleanArrayElement(arrayIndex, i); break;
                                        case DataType.Byte: ((object[])value)[i] = scriptedEntity.ScriptProcess[0].GetByteArrayElement(arrayIndex, i); break;
                                        case DataType.Double: ((object[])value)[i] = scriptedEntity.ScriptProcess[0].GetDoubleArrayElement(arrayIndex, i); break;
                                        case DataType.Float: ((object[])value)[i] = scriptedEntity.ScriptProcess[0].GetFloatArrayElement(arrayIndex, i); break;
                                        case DataType.Int: ((object[])value)[i] = scriptedEntity.ScriptProcess[0].GetIntArrayElement(arrayIndex, i); break;
                                        case DataType.Long: ((object[])value)[i] = scriptedEntity.ScriptProcess[0].GetLongArrayElement(arrayIndex, i); break;
                                        case DataType.Short: ((object[])value)[i] = scriptedEntity.ScriptProcess[0].GetShortArrayElement(arrayIndex, i); break;
                                        case DataType.String:
                                            ((object[])value)[i] = scriptedEntity.ScriptProcess[0].GetStringArrayElement(arrayIndex, i);
                                            if (((object[])value)[i] == null) ((object[])value)[i] = "";
                                            break;
                                        case DataType.Object:
                                            ((object[])value)[i] = scriptedEntity.ScriptProcess[0].GetObjectArrayElement(arrayIndex, i);
                                            if (((object[])value)[i] is NativeObject)
                                                ((object[])value)[i] = ((NativeObject)value).Object;
                                            break;
                                        //default: continue;
                                    }
                                }
                            }
                        }

                        // Check to see if there is an editmethod meta-data chunk attached to this property.
                        string editMethod = "", basePath = "", filter = "", description = "", name = variableSymbol.Identifier, enumeration = "";
                        bool dontCreate = false;
                        Type type = null, editorType = null;
                        foreach (Symbol subSymbol in symbol.Symbols)
                            if (subSymbol is MetaDataSymbol)
                                switch (((MetaDataSymbol)subSymbol).Identifier.ToLower())
                                {
                                    case "editmethod": editMethod = ((MetaDataSymbol)subSymbol).Value; break;
                                    case "basepath": basePath = Environment.CurrentDirectory +"\\"+ ((MetaDataSymbol)subSymbol).Value; break;
                                    case "filter": filter = ((MetaDataSymbol)subSymbol).Value; break;
                                    case "description": description = ((MetaDataSymbol)subSymbol).Value; break;
                                    case "name": name = ((MetaDataSymbol)subSymbol).Value; break;
                                    case "enumeration": enumeration = ((MetaDataSymbol)subSymbol).Value; break;
                                    case "filtertype":
                                        switch (((MetaDataSymbol)subSymbol).Value.ToLower())
                                        {
                                            case "audio": filter = "Audio Files|*.ogg;*.wav"; break;
                                            case "script": filter = "Script Files|*.fs;*.fso;*.fsl"; break;
                                            case "object": filter = "Object Declaration Files|*.fso"; break;
                                            case "font": filter = "Font Declaration Files|*.xml"; break;
                                            case "map": filter = "Fusion Map Files|*.fmp"; break;
                                            case "graphic": filter = "Graphics Files|*.tga;*.bmp"; break;
                                            case "tileset": filter = "Tileset Declaration Files|*.xml"; break;
                                            case "save": filter = "Fusion Save Files|*.fsv"; break;
                                            case "shader": filter = "Fusion Shader Files|*.xml"; break;
                                        }
                                        break;
                                }

                        if (editMethod.ToLower() == "file" && variableSymbol.DataType.DataType == DataType.String)
                        {
                            value = new FileEditorValue(value.ToString(), filter, basePath);
                            type = typeof(string);
                            editorType = typeof(FileEditor);
                        }
                        else if (editMethod.ToLower() == "color" && value is int)
                        {
                            value = Color.FromArgb((int)value);
                            type = typeof(Color);
                        }
                        else if (editMethod.ToLower() == "image" && variableSymbol.DataType.DataType == DataType.Object)
                        {
                            type = typeof(string);
                            editorType = typeof(ImageEditor);
                        }
                        else if (editMethod.ToLower() == "sound" && variableSymbol.DataType.DataType == DataType.Object)
                        {
                            type = typeof(string);
                            editorType = typeof(SoundEditor);
                        }
                        else if (editMethod.ToLower() == "enumeration" && variableSymbol.DataType.DataType == DataType.Int)
                        {
                            // Split up the enumeration scope.
                            string[] scopeList = enumeration.Split(new char[] { '.' });
                            Symbol currentScope = scriptedEntity.ScriptProcess.GlobalScope;

                            // Lets find the correct enumeration.
                            for (int i = 0; i < scopeList.Length; i++)
                            {
                                if (currentScope == null)
                                    break;

                                string scopeName = scopeList[i];
                                currentScope = currentScope.FindSymbol(scopeName);
                            }

                            // Check it exists and is an enumeration.
                            if (currentScope != null && currentScope.Type == SymbolType.Enumeration)
                            {
                                ArrayList list = new ArrayList();
                                PropertyEnumerationEntry currentEntry = null;
                                foreach (Symbol subSymbol in currentScope.Symbols)
                                {
                                    if (subSymbol == null || subSymbol.Type != SymbolType.Variable) continue;
                                    PropertyEnumerationEntry entry = new PropertyEnumerationEntry(subSymbol.Identifier, Int32.Parse(((VariableSymbol)subSymbol).ConstToken.Ident));
                                    if (entry.Value == (int)value) currentEntry = entry;
                                    list.Add(entry);
                                }

                                int index = (int)value;
                                value = (currentEntry == null) ? "Unknown" : currentEntry.Name;
                                type = typeof(string);

                                PropertyListViewItem item = new PropertyListViewItem(name, value, value, description, type);
                                item.EnumerationValues = list;
                                item.EnumerationValue = (currentEntry == null) ? 0 : currentEntry.Value;
                                item.TypeConverter = typeof(PropertyEnumerationConverter).AssemblyQualifiedName;
                                dontCreate = true;
                                categoryList.Add(item);
                            }
                        }
                        else
                        {
                            if (variableSymbol.DataType.DataType == DataType.Object)
                                continue;
                            type = value.GetType();
                        }

                        // And finally lets create the property item.
                        if (dontCreate == false)
                        {
                            if (editorType == null)
                                categoryList.Add(new PropertyListViewItem(name, value, value, description, type));
                            else
                                categoryList.Add(new PropertyListViewItem(name, value, value, description, type, editorType));
                        }
                    }
				}
                
                // Add everything to the category list.
                _categories[_categories.Length - 1] = categoryList.ToArray();
                #endregion
            }

            else if (_entity is EmitterNode)
            {
                // Add the new dynamic category to the category list.
                object[] newCategories = new object[_categories.Length + 1];
                _categories.CopyTo(newCategories, 0);
                _categories = newCategories;
                _categories[_categories.Length - 1] = new object[]
                    {
                        new PropertyListViewCategory("Emitter"),
                        new PropertyListViewItem("Effect", _entity, null, "Contains the actual definition of this emitters effect. Editing this will caused the effects editor to be opened.", typeof(string), typeof(EmitterEditor)),                                                                   
                    };

                //((PropertyListViewItem)(((object[])_categories[_categories.Length - 1])[1])).Attributes = new Attribute[] { new ReadOnlyAttribute(true) };
            }

            else if (_entity is TilemapSegmentNode)
            {
                TilemapSegmentNode segmentNode = _entity as TilemapSegmentNode;

                // Add the new dynamic category to the category list.
                object[] newCategories = new object[_categories.Length + 1];
                _categories.CopyTo(newCategories, 0);
                _categories = newCategories;
                _categories[_categories.Length - 1] = new object[]
                    {
                        new PropertyListViewCategory("Tilemap"),
                        new PropertyListViewItem("Grid Visible", segmentNode.IsGridVisible, false, "Indicates if a grid should be rendered over this tilemap segment or not.", typeof(bool)),                                                                   
                        new PropertyListViewItem("Tile Size", new Size(segmentNode.TileWidth, segmentNode.TileHeight), new Size(0,0), "Indicates if a grid should be rendered over this tilemap segment or not.", typeof(Size)),                                                                   
                    };
            }

            else if (_entity is PathMarkerNode)
            {
                PathMarkerNode markerNode = _entity as PathMarkerNode;

                // Add the new dynamic category to the category list.
                object[] newCategories = new object[_categories.Length + 1];
                _categories.CopyTo(newCategories, 0);
                _categories = newCategories;
                _categories[_categories.Length - 1] = new object[]
                    {
                        new PropertyListViewCategory("Path Marker"),
                        new PropertyListViewItem("Speed", markerNode.Speed, new StartFinishF(0,0), "Indicates the starting and finishing speeds a entity should move along this path.", typeof(StartFinishF)),                                                                   
                        new PropertyListViewItem("Delay", markerNode.Delay, 0, "If set this indicates the delay before this part of the path starts.", typeof(int)),  
                        new PropertyListViewItem("Next Marker Name", markerNode.NextNodeName, "", "This is the name of the next node in the sequence.", typeof(string)),                                           
                    
                    };
                ((PropertyListViewItem)((object[])_categories[_categories.Length - 1])[1]).TypeConverter = typeof(ExpandableObjectConverter).AssemblyQualifiedName;
            }

            // Populate the list view.
            foreach (object[] category in _categories)
            {
                // Create the category.
                PropertyListViewCategory listViewCategory = category[0] as PropertyListViewCategory;
                propertyListView.AddCategory(listViewCategory);

                // Add any existing items into the category.
                if (category.Length > 0)
                    for (int i = category.Length - 1; i >= 1; i--) // Add items in reverse otherwise it will appear the wrong way round :D.
                        listViewCategory.AddProperty((PropertyListViewItem)category[i]);
            }

            // Refresh the property list view.
            propertyListView.Refresh();
		}

        /// <summary>
        ///     Caleld when the user requests this window closed.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private new void Closing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Visible = false;
        }

		/// <summary>
		///		Initializes a new instance of this window.
		/// </summary>
		/// <param name="entity">The entity this window should be bound to.</param>
		public EntityPropertiesWindow(EntityNode entity)
		{
			_entity = entity;
			InitializeComponent();
			SyncronizeData();
            propertyListView.SetValueDelegate += new PropertyListViewSetValueDelegate(SetValue);
		}

		#endregion
	}

    /// <summary>
    ///     Used to select and edit emitter through the propertygrid control.
    /// </summary>
    public class EmitterEditor : UITypeEditor
    {
        #region Members
        #region Variables

        #endregion
        #region Properties

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Gets the style in which this editor edits the value.
        /// </summary>
        /// <param name="context">Context of editing.</param>
        /// <returns>Editor editing mode.</returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        ///     Called when the value needs to be edited.
        /// </summary>
        /// <param name="context">Context of editing.</param>
        /// <param name="provider">Provider of editing.</param>
        /// <param name="value">Original value to edit.</param>
        /// <returns>Edited version of original value.</returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            EmitterEditorWindow emitterDesignerWindow = new EmitterEditorWindow();
            (value as EmitterNode).CopyTo(emitterDesignerWindow.Emitter);

            // Result of dialog is not really required.
            emitterDesignerWindow.ShowDialog();
            emitterDesignerWindow.Emitter.CopyTo(value as EmitterNode);
            return value as EmitterNode;
        }

        #endregion
    }

}