/*
 * File: Scene Graph.cs
 *
 * Contains the main declaration of the scene graph class
 * which is used as a way of structuring the game's scene for
 * intuitive and compound updating and rendering
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Reflection;
using BinaryPhoenix.Fusion.Graphics;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Engine.Entitys;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Runtime.Scripting;

namespace BinaryPhoenix.Fusion.Engine
{

	/// <summary>
	///		This class is used as a way of structuring the game's scene for
	///		intuitive and compound updating and rendering.
	/// </summary>
	public sealed class SceneGraph
	{
		#region Members
		#region Variables

		private SceneNode _rootNode = new SceneNode("Root");
		private ArrayList _cameraList = new ArrayList();

        private static ArrayList _nodeList = new ArrayList();

        private int _uniqueIDTracker = 0;

        private int _loadingProgress = 0;

		#endregion
		#region Properties

        /// <summary>
        ///     Gets the current loading progress.
        /// </summary>
        public int LoadingProgress
        {
            get { return _loadingProgress; }
        }

		/// <summary>
		///		Returns the node used as the root of this scene graph.
		/// </summary>
		public SceneNode RootNode
		{
			get { return _rootNode;  }
			set { _rootNode = value; } 
		}

		/// <summary>
		///		Gets or sets the list of cameras that should be used for 
		///		rendering this scene graph.
		/// </summary>
		public ArrayList CameraList
		{
			get { return _cameraList; }
			set { _cameraList = value; }
		}

        /// <summary>
        ///     Gets a bounding box that encircles the entire scene graph.
        /// </summary>
        public RectangleF GlobalBoundingBox
        {
            get
            {
                return _rootNode.GetGlobalBoundingBox(new Transformation(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f));
            }
        }

        /// <summary>
        ///     Gets or sets the global node list.
        /// </summary>
        public static ArrayList NodeList
        {
            get { return _nodeList; }
            set { _nodeList = value; }
        }

        /// <summary>
        ///     Gets all the layers in the scene graph.
        /// </summary>
        public static int[] Layers
        {
            get
            {
                ArrayList layers = new ArrayList();
                foreach (SceneNode node in _nodeList)
                {
                    if (node as EntityNode != null)
                    {
                        if (!layers.Contains(((EntityNode)node).DepthLayer))
                            layers.Add(((EntityNode)node).DepthLayer);
                    }
                }
                layers.Sort();
                return (int[])layers.ToArray(typeof(int));
            }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Renders all nodes in this scene graph.
		/// </summary>
		public void Render()
		{
            // Work out what layers we need to render.
            ArrayList layers = new ArrayList();
            foreach (SceneNode node in _nodeList)
            {
                if (node as EntityNode != null)
                {
                    if (!layers.Contains(((EntityNode)node).DepthLayer))
                        layers.Add(((EntityNode)node).DepthLayer);
                }
            }
            layers.Sort();

            // Render all the camera views.
            foreach (CameraNode camera in _cameraList)
		    {   
                GraphicsManager.ClearRenderState();

                camera.Render(new Transformation(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f), camera, 0);
                Transformation cameraTransformation = camera.CalculateTransformation();
                
                // Render each layer.
                for (int i = 0; i < layers.Count; i++)
                {
                    // What layer will we render?
                    int layer = (int)layers[i];

                    // Clear the depth buffer so that this layer renders ontop of all others.
                    GraphicsManager.ClearDepthBuffer();

                    // Clear the depth buffer so this layer is drawn above all others.
                    _rootNode.Render(cameraTransformation, camera, layer);
                }
		    }
		}

        /// <summary>
        ///     Renders a given layer of the scene graph.
        /// </summary>
        /// <param name="layer">Layer of scene graph to render.</param>
        public void Render(int layer)
        {
            // Render all the camera views.
            foreach (CameraNode camera in _cameraList)
            {
                GraphicsManager.ClearRenderState();

               // camera.Render(new Transformation(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f), camera, 0);
                Transformation cameraTransformation = camera.CalculateTransformation();

                // Clear the depth buffer so that this layer renders ontop of all others.
                GraphicsManager.ClearDepthBuffer();

                // Clear the depth buffer so this layer is drawn above all others.
                _rootNode.Render(cameraTransformation, camera, layer);
            }
        }

		/// <summary>
		///		Adds the given camera from the rendering list.
		/// </summary>
		/// <param name="camera">Camera to add to rendering list.</param>
		public void AttachCamera(CameraNode camera)
        {
            if (_cameraList.Contains(camera)) return;
			_cameraList.Add(camera);
		}

		/// <summary>
		///		Removes the given camera from the rendering list.
		/// </summary>
		/// <param name="camera">Camera to remove from rendering list.</param>
		public void DetachCamera(CameraNode camera)
		{
			_cameraList.Remove(camera);
		}

        /// <summary>
        ///		Adds a node from the global node list.
        /// </summary>
        /// <param name="node">Node to add to global node list.</param>
        public static void AttachNode(SceneNode node)
        {
            if (_nodeList.Contains(node)) return;
            _nodeList.Add(node);
        }

        /// <summary>
        ///		Removes a node from the global node list.
        /// </summary>
        /// <param name="node">Node to remove to global node list.</param>
        public static void DetachNode(SceneNode node)
        {
            _nodeList.Remove(node);
        }

		/// <summary>
		///		Clear out every scene node from this scene graph.
		/// </summary>
		public void Clear()
		{
			_rootNode.ClearChildren();
			_rootNode = new SceneNode("Root");
			_cameraList.Clear();
		}

		/// <summary>
		///		Creates an array list containing every node in this graph.
		/// </summary>
		/// <returns>Array list containing every node in this graph.</returns>
		public ArrayList EnumerateNodes()
		{
			ArrayList nodeList = new ArrayList();
			nodeList.Add(_rootNode);
			EnumerateChildrenOfNode(nodeList,_rootNode);
			return nodeList;
		}

		/// <summary>
		///		Used by the EnumerateNodes function to add all the children of
		///		a node to the given array list.
		/// </summary>
		/// <param name="nodeList">Array list to add children to.</param>
		/// <param name="node">Node to add children from.</param>
		private void EnumerateChildrenOfNode(ArrayList nodeList, SceneNode node)
		{
            nodeList.AddRange(node.Children);
            for (int i = 0; i < node.Children.Count; i++)
				EnumerateChildrenOfNode(nodeList, (SceneNode)node.Children[i]);
		}

		/// <summary>
		///		Saves this scene graph into a given binary writer.
		/// </summary>
		/// <param name="stream">Binary writer to save this scene graph into.</param>
		/// <param name="baseNode">The base node that all saving should be started from, this allows you to ignore this node and any nodes higher in the graph like the Root Camera.</param>
		public void Save(BinaryWriter writer, SceneNode baseNode)
		{
			writer.Write(baseNode != null);
			if (baseNode != null)
			{
				writer.Write(baseNode.Children.Count);
				foreach (SceneNode childNode in baseNode.Children)
					SaveNode(writer, childNode);
			}
			else
				SaveNode(writer, _rootNode);
		}
		public void Save(BinaryWriter writer)
		{
			Save(writer, null);
		}

		/// <summary>
		///		Saves the given node and all of its children into a given binary writer.
		/// </summary>
		/// <param name="writer">Binary writer to save the given node into.</param>
		/// <param name="node">Node to save into binary writer.</param>
		private void SaveNode(BinaryWriter writer, SceneNode node)
		{
			// Write in the name of this node.
			writer.Write(node.GetType().FullName);

			// Save the nodes details into the stream.
			node.Save(writer);

			// Save the children.
			writer.Write(node.Children.Count);
			foreach (SceneNode childNode in node.Children)
				SaveNode(writer, childNode);
		}

		/// <summary>
		///		Loads the scene graph from a given binary reader.
		/// </summary>
		/// <param name="stream">Binary reader to load this scene graph from.</param>
		/// <param name="baseNode">The base node that all loading should be started from, this allows you to ignore this node and any nodes higher in the graph like the Root Camera.</param>
        /// <param name="keepPersistent">If set to true persistent ob jects will be kept.</param>
        /// <param name="keepCameras">If set to true all cameras will not be destroyed.</param>
        public void Load(BinaryReader reader, bool keepPersistent, bool keepCameras, SceneNode baseNode)
		{
            // Dispose of all old entities.

                // Lets compile a list of all peristent nodes if we are keeping them.
                ArrayList persistentList = new ArrayList();
                if (keepPersistent == true)
                {
                    ArrayList entityList = new ArrayList(_nodeList);
                    foreach (SceneNode node in entityList)
                        if (node.IsPersistent == true)
                            persistentList.Add(node);
                        else
                            node.Dispose();
                    entityList.Clear();
                    entityList = null;
                }

            // Get starting memory.
            long startingMemory = GC.GetTotalMemory(true);

            // Clear out any non-persistent cameras.
            if (keepCameras == false)
            {
                ArrayList removalList = new ArrayList();

                foreach (CameraNode camera in _cameraList)
                    if (camera.IsPersistent == false)
                        removalList.Add(camera);

                foreach (CameraNode camera in removalList)
                    _cameraList.Remove(camera);
            }

            // Reset unique ID counter.
            _uniqueIDTracker = 0;

            // Clean out base node.
            if (baseNode != null)
                baseNode.ClearChildren();
            else
                _rootNode.ClearChildren();

			bool baseNodeExists = reader.ReadBoolean();
			if (baseNodeExists == true)
			{
				int childCount = reader.ReadInt32();
				for(int i = 0; i < childCount; i++)
				{
					if (baseNode != null)
						baseNode.AddChild(LoadNode(reader));
					else
						_rootNode.AddChild(LoadNode(reader));
				}
			}
			else
				_rootNode = LoadNode(reader, true);

            // Lets see if we can reattached any persistent nodes, if not lets just
            // tack them onto the root node.
            if (keepPersistent == true)
            {
                foreach (SceneNode node in persistentList)
                {
                    if (node.Parent == null || node.Parent.IsPersistent == false)
                        (baseNode == null ? _rootNode : baseNode).AddChild(node);
                }
            }

            // Say how many have been created.
            DebugLogger.WriteLog("Loaded " + _uniqueIDTracker + " nodes into scene graph, with a cumulative memory allocation of " + (((GC.GetTotalMemory(true) - startingMemory) / 1024.0f) / 1024.0f) + "mb.");

            // Go through each node and grab its properties
            SyncronizeNodes();

            // Reset loading percentage.
            _loadingProgress = 0;
		}
		public void Load(BinaryReader reader, bool keepPersistent)
		{
			Load(reader, keepPersistent, true, null);
		}

        /// <summary>
        ///     Gets all the nodes in the scene graph by there name.
        /// </summary>
        /// <param name="name">Name of nodes to get.</param>
        /// <returns>List of retrieved nodes.</returns>
        public ArrayList GetNodesByName(string name)
        {
            ArrayList list = new ArrayList();
            ArrayList nodes = EnumerateNodes();
            foreach (SceneNode subNode in nodes)
            {
                EntityNode subEntityNode = subNode as EntityNode;
                if (subEntityNode == null) continue;
                if (subEntityNode.Name.ToLower() == name.ToLower())
                    list.Add(subEntityNode);
            }
            return list;
        }

        /// <summary>
        ///     Syncronizes all entities so that there event node lists are accurate and so 
        ///     that a few other bits and pieces are correct.
        /// </summary>
        public void SyncronizeNodes()
        {
            ArrayList nodes = new ArrayList(_nodeList);
            foreach (SceneNode node in nodes)
            {
                EntityNode entityNode = node as EntityNode;
                if (entityNode == null)
                    continue;

                // Is it solid? If so make its collision polygon static.
                if (entityNode.CollisionPolygon != null) entityNode.CollisionPolygon.FrameCount = 1;

                // Is it a scripted entity node? Is its script fucked up? Kill it!
                if (node is ScriptedEntityNode && (((ScriptedEntityNode)node).ScriptExecutionProcess == null || ((ScriptedEntityNode)node).ScriptExecutionProcess.Process == null))
                {
                    node.Dispose();
                    continue;
                }

                if (entityNode.Event != null && entityNode.Event != "")
                {
                    foreach (SceneNode eventNode in nodes)
                        if (eventNode.Name.ToLower() == entityNode.Event.ToLower()) entityNode.EventNodes.Add(eventNode);
                }

                if (node is PathMarkerNode)
                {
                    ArrayList pathNodes = new ArrayList();
                    foreach (SceneNode eventNode in nodes)
                        if (eventNode.Name.ToLower() == ((PathMarkerNode)entityNode).NextNodeName.ToLower()) pathNodes.Add(eventNode);

                    ((PathMarkerNode)entityNode).NextNode = (pathNodes.Count != 0) ? pathNodes[0] as PathMarkerNode : null;
                }
            }
        }

		/// <summary>
		///		Loads the next node in the given binary reader and returns it.
		/// </summary>
		/// <param name="reader">Binary reader to read node from.</param>
        /// <param name="getProgress">If set to true this node is used to judge the current progress.</param>
		/// <returns>Scene node that was loaded from the binary reader.</returns>
		private SceneNode LoadNode(BinaryReader reader, bool getProgress)
		{
			// Read in the name of this node's class.
			string name = reader.ReadString();

            //HighPreformanceTimer timer = new HighPreformanceTimer();

			// Create a new instance of this entity and tell it to load itself.
            // (See if its a known object first as its quicker to get it without reflection)
            SceneNode node = null;
            if (name.ToLower().EndsWith("binaryphoenix.fusion.engine.entitys.entitynode"))
                node = new EntityNode();
            else if (name.ToLower().EndsWith("binaryphoenix.fusion.engine.entitys.scriptedentitynode"))
                node = new ScriptedEntityNode();
            else if (name.ToLower().EndsWith("binaryphoenix.fusion.engine.scenenode"))
                node = new SceneNode();
            else if (name.ToLower().EndsWith("binaryphoenix.fusion.engine.entitys.groupnode"))
                node = new GroupNode();
            else if (name.ToLower().EndsWith("binaryphoenix.fusion.engine.entitys.emitternode"))
                node = new EmitterNode();
            else if (name.ToLower().EndsWith("binaryphoenix.fusion.engine.entitys.tilemapsegmentnode"))
                node = new TilemapSegmentNode();
            else if (name.ToLower().EndsWith("binaryphoenix.fusion.engine.entitys.pathmarkernode"))
                node = new PathMarkerNode();
            else if (name.ToLower().EndsWith("binaryphoenix.fusion.engine.entitys.tilenode"))
                node = new TileNode();
            else
                node = (SceneNode)ReflectionMethods.CreateObject(name);

            //DebugLogger.WriteLog("Created scene graph node " + name + " in " + timer.DurationMillisecond + ".\n");
            //timer.Restart();

            // Load in this nodes details.
            node.UniqueID = (_uniqueIDTracker++);
            node.Load(reader);

            //DebugLogger.WriteLog("Loaded scene graph node " + name + " in " + timer.DurationMillisecond + ".\n");

			// Read in all the children of this node, and attach
			// them to this node. 
            DebugLogger.IncreaseIndent();
			int childCount = reader.ReadInt32();
			for (int i = 0; i < childCount; i++)
			{
				SceneNode childNode = LoadNode(reader);
				node.AddChild(childNode);

                if (getProgress == true)
                    _loadingProgress = (int)(((float)(i + 1) / (float)childCount) * 100.0f);
            }
            DebugLogger.DecreaseIndent();

			return node;
		}
        private SceneNode LoadNode(BinaryReader reader)
        {
            return LoadNode(reader, false);
        }

		#endregion
	}

	/// <summary>
	///		Used to describ which way to shift a node in a scene graph.
	/// </summary>
	public enum NodeShiftDirection
	{
		Up,
		Down,
		Left,
		Right
	}

	/// <summary>
	///		Used as a base for any object that is added to a scene
	///		graph, anything you wish to render or update should be
	///		inhereted from this.
	/// </summary>
	public class SceneNode
	{
		#region Members
		#region Variables

        protected bool _persistent;

		protected SceneNode _parent;

		protected string _name;
		protected ArrayList _childList = new ArrayList();

        protected Transformation _transformation = new Transformation(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f);
        protected Transformation _transformationOffset = new Transformation(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f);

        protected EntityDepthMode _depthMode = _defaultDepthMode;
        protected int _depthLayer = 0;

        protected int _uniqueID = 0;

        protected static EntityDepthMode _defaultDepthMode = EntityDepthMode.AddBoundingBoxBottom;

		#endregion
		#region properties

        /// <summary>
        ///		Gets or sets if this node should persist when the scene graph it is attached
        ///     to is reloaded.
        /// </summary>
        public bool IsPersistent
        {
            get { return _persistent; }
            set { _persistent = value; }
        }

		/// <summary>
		///		Gets or sets the name of this node.
		/// </summary>
		public string Name
		{
			get { return _name;  }
			set { _name = value; }
		}

		/// <summary>
		///		Gets or sets the parent node thats above this node in the scene graph.
		/// </summary>
		public virtual SceneNode Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}

		/// <summary>
		///		Returns a list of all children associated with this node.
		/// </summary>
		public ArrayList Children
		{
			get { return (ArrayList)_childList.Clone(); }
		}

        /// <summary>
        ///		Gets or sets the width of this entity, this is mainly used for bounding box calculation.
        ///		rendering.
        /// </summary>
        public virtual int Width
        {
            get { return 0; }
            set {  }
        }

        /// <summary>
        ///		Gets or sets the height of this entity, this is mainly used for bounding box calculation.
        /// </summary>
        public virtual int Height
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        ///		Gets or sets the transformation of this entity.
        /// </summary>
        public virtual Transformation Transformation
        {
            get { return _transformation; }
            set { _transformation = value; }
        }

        /// <summary>
        ///		Gets or sets the transformation offset of this entity.
        /// </summary>
        public virtual Transformation TransformationOffset
        {
            get { return _transformationOffset; }
            set { _transformationOffset = value; }
        }

        /// <summary>
        ///     Gets or sets the depth mode to use when Z sorting this entity.
        /// </summary>
        public EntityDepthMode DepthMode
        {
            get { return _depthMode; }
            set { _depthMode = value; }
        }

        /// <summary>
        ///     Gets or sets the depth layer that this object is one.
        /// </summary>
        public int DepthLayer
        {
            get { return _depthLayer; }
            set { _depthLayer = value; }
        }

        /// <summary>
        ///     Gets or sets the default depth mode that entities should use.
        /// </summary>
        public static EntityDepthMode DefaultDepthMode
        {
            get { return _defaultDepthMode; }
            set { _defaultDepthMode = value; }
        }

        /// <summary>
        ///     Gets or sets the unique ID of this entity, relative to the current map.
        /// </summary>
        public int UniqueID
        {
            get { return _uniqueID; }
            set { _uniqueID = value; }
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
            return _parent != null ? base.ToString() + " : (" + _parent.ToString()+")" : base.ToString() +" : (NULL)";
        }

        /// <summary>
        ///     Resets this node to the state it was in when it was created.
        /// </summary>
        public virtual void Reset()
        {
            if (_parent != null) _parent.RemoveChild(this);

            _transformation = new Transformation(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f);
            _transformationOffset = new Transformation(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f);
        }

        /// <summary>
        ///     Gets a bounding box that encircles both this node and its child nodes.
        /// </summary>
        /// <param name="transformation">Transformation of parent node.</param>
        public virtual RectangleF GetGlobalBoundingBox(Transformation transformation)
        {
            if (_persistent == true)
               return new RectangleF(0,0,0,0);

            Transformation relativeTransformation = CalculateRelativeTransformation(transformation);
            RectangleF box = new RectangleF(relativeTransformation.X, relativeTransformation.Y, Width * relativeTransformation.ScaleX, Height * relativeTransformation.ScaleY);
            foreach (SceneNode node in _childList)
            {
                RectangleF childBox = node.GetGlobalBoundingBox(relativeTransformation);
                if (childBox.X == 0 && childBox.Y == 0 && childBox.Width == 0 && childBox.Height == 0) continue;
                if (childBox.X < box.X)
                {
                    box.Width += (box.X - childBox.X);
                    box.X = childBox.X;
                }
                if (childBox.Y < box.Y)
                {
                    box.Height += (box.Y - childBox.Y);
                    box.Y = childBox.Y;
                }
                if (childBox.Right > box.Right) box.Width += (childBox.Right - box.Right);
                if (childBox.Bottom > box.Bottom) box.Height += (childBox.Bottom - box.Bottom);
            }

            return box;
        }

        /// <summary>
        ///		Calculates this entitys relative transformation to another transformation.
        /// </summary>
        /// <param name="transformation">Transformation to calculate relative transformation from.</param>
        /// <returns>Transfromation relative to the given transfromation.</returns>
        public virtual Transformation CalculateRelativeTransformation(Transformation transformation)
        {
            Transformation newTransform = new Transformation();
            newTransform.AngleX = transformation.AngleX + _transformation.AngleX;
            newTransform.AngleY = transformation.AngleY + _transformation.AngleY;
            newTransform.AngleZ = transformation.AngleZ + _transformation.AngleZ;
            newTransform.ScaleX = transformation.ScaleX + Math.Abs(_transformation.ScaleX) - 1.0f;
            newTransform.ScaleY = transformation.ScaleY + Math.Abs(_transformation.ScaleY) - 1.0f;
            newTransform.ScaleZ = transformation.ScaleY + Math.Abs(_transformation.ScaleZ) - 1.0f;
            if (_transformation.ScaleX < 0) newTransform.ScaleX = -newTransform.ScaleX;
            if (_transformation.ScaleY < 0) newTransform.ScaleY = -newTransform.ScaleY;
            if (_transformation.ScaleZ < 0) newTransform.ScaleY = -newTransform.ScaleZ;

            newTransform.X = (float)Math.Truncate(transformation.X + ((_transformation.X + _transformationOffset.X) * transformation.ScaleX));
            newTransform.Y = (float)Math.Truncate(transformation.Y + ((_transformation.Y + _transformationOffset.Y) * transformation.ScaleY));

            // Work out the depth offset.
            float depthOffset = 0;
            switch (_depthMode)
            {
                case EntityDepthMode.AddXCoordinate: depthOffset = newTransform.X; break;
                case EntityDepthMode.AddYCoordinate: depthOffset = newTransform.Y; break;
                case EntityDepthMode.SubtractXCoordinate: depthOffset = -newTransform.X; break;
                case EntityDepthMode.SubtractYCoordinate: depthOffset = -newTransform.Y; break;
            }

            //newTransform.Z = (transformation.Z + _transformation.Z + _transformationOffset.Z) - ((_depthLayer * 1000) + depthOffset);
            newTransform.Z = (transformation.Z + _transformation.Z + _transformationOffset.Z) - depthOffset;

            return newTransform;
        }

        /// <summary>
        ///		Gets the transformation of this entity relative to all the nodes above it.
        /// </summary>
        public Transformation CalculateTransformation()
        {
            ArrayList parentNodes = new ArrayList();
            SceneNode parent = this.Parent;
            while (parent != null)
            {
                if ((parent as SceneNode) != null) parentNodes.Add(parent);
                parent = parent.Parent;
            }
            parentNodes.Reverse();

            Transformation relativeTransformation = new Transformation(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f);
            foreach (SceneNode node in parentNodes)
                relativeTransformation = node.CalculateRelativeTransformation(relativeTransformation);

            return CalculateRelativeTransformation(relativeTransformation);
        }

		/// <summary>
		///		Saves this scene node into a given binary writer.
		/// </summary>
		/// <param name="writer">Binary writer to save this scene node into.</param>
		public virtual void Save(BinaryWriter writer)
		{
            byte mask = 0;
            if (_name != "") mask |= 1;
            if (_transformation.ScaleX != 1.0f || _transformation.ScaleY != 1.0f || _transformation.ScaleZ != 1.0f || _transformation.AngleX != 0.0f || _transformation.AngleY != 0.0f || _transformation.AngleZ != 0.0f) mask |= 2;
            if (_transformation.Z != 0.0f) mask |= 4;
            writer.Write(mask);

            if (_name != "") writer.Write(_name);

            writer.Write(_transformation.X);
            writer.Write(_transformation.Y);
            if (_transformation.Z != 0.0f) writer.Write(_transformation.Z);
            if (_transformation.ScaleX != 1.0f || _transformation.ScaleY != 1.0f || _transformation.ScaleZ != 1.0f ||
                _transformation.AngleX != 0.0f || _transformation.AngleY != 0.0f || _transformation.AngleZ != 0.0f)
            {
                // SCALING UPDATE NOW!
                writer.Write((short)_transformation.AngleX);
                writer.Write((short)_transformation.AngleY);
                writer.Write((short)_transformation.AngleZ);
                writer.Write(_transformation.ScaleX);
                writer.Write(_transformation.ScaleY);
                writer.Write(_transformation.ScaleZ);
            }
		}

		/// <summary>
		///		Loads this scene node from a given binary reader.
		/// </summary>
		/// <param name="reader">Binary reader to load this scene node from.</param>
		public virtual void Load(BinaryReader reader)
		{
            byte mask = reader.ReadByte();

            if ((mask & 1) != 0)
			    _name = reader.ReadString();

            _transformation.X = reader.ReadSingle();
            _transformation.Y = reader.ReadSingle(); 
            if ((mask & 4) != 0) _transformation.Z = reader.ReadSingle();
            if ((mask & 2) != 0)
            {
                _transformation.AngleX = reader.ReadInt16();
                _transformation.AngleY = reader.ReadInt16();
                _transformation.AngleZ = reader.ReadInt16();
                _transformation.ScaleX = reader.ReadSingle();
                _transformation.ScaleY = reader.ReadSingle();
                _transformation.ScaleZ = reader.ReadSingle();
            }
		}

		/// <summary>
		///		When this method is called it will create an exact copy of this scene node.
		/// </summary>
		/// <returns>Exact copy of this scene node.</returns>
		public SceneNode Clone()
		{
			Type type = this.GetType();
			SceneNode node = (SceneNode)Activator.CreateInstance(type);
			if (_parent != null) _parent.AddChild(node);
			this.CopyTo(node);
			return node;
		}

		/// <summary>
		///		Copys all the data contained in this scene node to another scene node.
		/// </summary>
		/// <param name="node">Scene node to copy data into.</param>
		public virtual void CopyTo(SceneNode node)
		{
		}

        /// <summary>
        ///		Rotates this entity relative to its currently rotation.
        /// </summary>
        /// <param name="x">Angle on the x-axis.</param>
        /// <param name="y">Angle on the y-axis.</param>
        /// <param name="z">Angle on the z-axis.</param>
        public void Turn(float x, float y, float z)
        {
            _transformation.AngleX += x;
            _transformation.AngleY += y;
            _transformation.AngleZ += z;
        }

        /// <summary>
        ///		Rotates this entity absolutely.
        /// </summary>
        /// <param name="x">Angle on the x-axis.</param>
        /// <param name="y">Angle on the y-axis.</param>
        /// <param name="z">Angle on the z-axis.</param>
        public void Rotate(float x, float y, float z)
        {
            _transformation.AngleX = x;
            _transformation.AngleY = y;
            _transformation.AngleZ = z;
        }

        /// <summary>
        ///		Translates this entity relative to its currently rotation and position.
        /// </summary>
        /// <param name="x">Amount on the x-axis to translate this entity by.</param>
        /// <param name="y">Amount on the y-axis to translate this entity by.</param>
        /// <param name="z">Amount on the z-axis to translate this entity by.</param>
        public void Translate(float x, float y, float z)
        {
            float mx = ((float)Math.Cos(MathMethods.DegreesToRadians(_transformation.AngleX)) * x);
            float my = -((float)Math.Sin(MathMethods.DegreesToRadians(_transformation.AngleY)) * y);
            _transformation.X += my;
            _transformation.Y += mx;
            _transformation.Z += z;
            Transformation = _transformation;
        }

        /// <summary>
        ///		Translates this entity relative to its currently position.
        /// </summary>
        /// <param name="x">Amount on the x-axis to translate this entity by.</param>
        /// <param name="y">Amount on the y-axis to translate this entity by.</param>
        /// <param name="z">Amount on the z-axis to translate this entity by.</param>
        public void Move(float x, float y, float z)
        {
            _transformation.X += x;
            _transformation.Y += y;
            _transformation.Z += z;
            Transformation = _transformation;
        }

        /// <summary>
        ///		Translates this entity absolutely to a given position.
        /// </summary>
        /// <param name="x">Position on the x-axis to translate this entity to.</param>
        /// <param name="y">Position on the y-axis to translate this entity to.</param>
        /// <param name="z">Position on the z-axis to translate this entity to.</param>
        public void Position(float x, float y, float z)
        {
            _transformation.X = x;
            _transformation.Y = y;
            _transformation.Z = z;
            Transformation = _transformation;
        }

        /// <summary>
        ///		Scales this entity to the given absolute value.
        /// </summary>
        /// <param name="x">Scale on the x-axis to scale this entity to.</param>
        /// <param name="y">Scale on the y-axis to scale this entity to</param>
        /// <param name="z">Scale on the z-axis to scale this entity to</param>
        public void Scale(float x, float y, float z)
        {
            _transformation.ScaleX = x;
            _transformation.ScaleY = y;
            _transformation.ScaleZ = z;
        }

		/// <summary>
		///		Called when this node needs to be rendered.
		/// </summary>
		public virtual void Render(Transformation transformation, CameraNode camera, int layer)
		{
			RenderChildren(transformation, camera, layer);
		}

		/// <summary>
		///		Renders all child nodes of this node.
		/// </summary>
        public void RenderChildren(Transformation transformation, CameraNode camera, int layer)
		{
			foreach (SceneNode child in _childList)
			{
                // Not on the same layer? Not got any children? Return.
                if (layer != child.DepthLayer && (child.Children.Count == 0)) continue;

                //HighPreformanceTimer timer = new HighPreformanceTimer();
			    GraphicsManager.PushRenderState();
				child.Render(transformation, camera, layer);
				GraphicsManager.PopRenderState();
                //if (timer.DurationMillisecond > 2)
               //     System.Console.WriteLine(child.ToString() + " (" + ((child as EntityNode) != null ? (((EntityNode)child).Image != null ? ((EntityNode)child).Image.URL : "") + "," + ((EntityNode)child).RenderMode : "") + ") - Rendered in " + timer.DurationMillisecond);
			}
		}

		/// <summary>
		///		Moves the given child to the back of the rendering list.
		/// </summary>
		/// <param name="child">Child to move.</param>
		public void SendChildToBack(SceneNode child)
		{
			_childList.Remove(child);
			_childList.Insert(0, child);
		}

		/// <summary>
		///		Moves the given child to the front of the rendering list.
		/// </summary>
		/// <param name="child">Child to move.</param>
		public void BringChildToFront(SceneNode child)
		{
			_childList.Remove(child);
			_childList.Add(child);
		}

		/// <summary>
		///		Moves the given child 1 step backwards in the rendering list.
		/// </summary>
		/// <param name="child">Child to move.</param>
		public void ShiftChildBackwards(SceneNode child)
		{
			int index = _childList.IndexOf(child);
			_childList.Remove(child);
			_childList.Insert(Math.Max(0, index - 1), child);
		}


		/// <summary>
		///		Moves the given child 1 step forewards in the rendering list.
		/// </summary>
		/// <param name="child">Child to move.</param>
		public void ShiftChildForewards(SceneNode child)
		{
			int index = _childList.IndexOf(child);
			_childList.Remove(child);
			_childList.Insert(Math.Min(_childList.Count, index + 1), child);
		}

		/// <summary>
		///		Adds the given scene node to this nodes child list.
		/// </summary>	
		/// <param name="child">Scene node to add as child.</param>
		public virtual void AddChild(SceneNode child)
		{
            if (_childList.Contains(child)) return;
			_childList.Add(child);
			child.Parent = this;
            SceneGraph.AttachNode(child);
		}

		/// <summary>
		///		Removes the given scene node from this nodes child list.
		/// </summary>
		/// <param name="child">Scene node to remove.</param>
		public virtual void RemoveChild(SceneNode child)
		{
			_childList.Remove(child);
			child.Parent = null;
            SceneGraph.DetachNode(child);
		}

		/// <summary>
		///		Returns true if the given scene node is a child
		///		of this node.
		/// </summary>
		/// <param name="child">Scene node to look for.</param>
		public bool ChildExists(SceneNode child)
		{
			return _childList.Contains(child);
		}

		/// <summary>
		///		Removes all children from this node.
		/// </summary>
		public void ClearChildren()
		{
            foreach (SceneNode child in new ArrayList(_childList))
            {
                child.Parent = null;
            }
                //    child.Dispose();
             _childList.Clear();
		}

        /// <summary>
        ///     Responsable for removing references of this object and deallocated
        ///     resources that have been allocated.
        /// </summary>
        public virtual void Dispose()
        {
            // Clear out references
            if (_parent != null)
                _parent.RemoveChild(this);
            _parent = null;
            SceneGraph.DetachNode(this);

            // Remove all the childrens references. (Don't tell them to dispose, that is done elsewhere).
            ClearChildren();

            // Remove all the script references.
            VirtualMachine.GlobalInstance.RemoveReferences(this);

            Statistics.StoreInt("Disposal Counted Scene Node Count", Statistics.ReadInt("Disposal Counted Scene Node Count") - 1);
        }

		/// <summary>
		///		Initializes a new instance and gives it the specified  
		///		name.
		/// </summary>
		/// <param name="name">Name to name as node to.</param>
		public SceneNode(string name)
		{
			_name = name;
            Statistics.StoreInt("Scene Node Count", Statistics.ReadInt("Scene Node Count") + 1);
            Statistics.StoreInt("Disposal Counted Scene Node Count", Statistics.ReadInt("Disposal Counted Scene Node Count") + 1);
		}
        public SceneNode() 
        {
            Statistics.StoreInt("Scene Node Count", Statistics.ReadInt("Scene Node Count") + 1);
            Statistics.StoreInt("Disposal Counted Scene Node Count", Statistics.ReadInt("Disposal Counted Scene Node Count") + 1);
        }

        ~SceneNode()
        {
            Statistics.StoreInt("Scene Node Count", Statistics.ReadInt("Scene Node Count") - 1);
        }

		#endregion
	}

}
