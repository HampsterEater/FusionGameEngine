/*
 * File: Emitter.cs
 *
 * Contains the main declaration of the emitter class
 * which is a derivitive of the SceneNode class and is
 * used to create complex particle effects.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Drawing.Design;
using BinaryPhoenix.Fusion.Graphics;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Resources;
using BinaryPhoenix.Fusion.Engine.Windows;
using BinaryPhoenix.Fusion.Runtime.Processes;
using BinaryPhoenix.Fusion.Engine.Processes;
using System.ComponentModel;

namespace BinaryPhoenix.Fusion.Engine.Entitys
{
    /// <summary>
    ///		The path marker class is a derivitive of the SceneNode class and is
    ///		used to mark out a path an object can take.
    /// </summary>
    public class PathMarkerNode : EntityNode
    {
        #region Members
        #region Variables

        protected int _pathLineColor = unchecked((int)0xFF00FF00);

        private StartFinishF _speed = new StartFinishF(1,1);
        private int _delay = 0;

        private PathMarkerNode _nextNode = null;
        private string _nextNodeName = "";

        #endregion
        #region Properties

        /// <summary>
        ///		Returns a string which can be used to identify what kind of entity this is.
        /// </summary>
        public override string Type
        {
            get { return _type != "" ? _type : "path marker"; }
        }

        /// <summary>
        ///		Gets or sets the starting and finishing speed of this marker.
        /// </summary>
        public StartFinishF Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        /// <summary>
        ///     Gets or sets the delay before this part of the path starts.
        /// </summary>
        public int Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

        /// <summary>
        ///     Gets the next node of this path.
        /// </summary>
        public string NextNodeName
        {
            get { return _nextNodeName; }
            set { _nextNodeName = value; }
        }

        /// <summary>
        ///     Gets the next node of this path.
        /// </summary>
        public PathMarkerNode NextNode
        {
            get { return _nextNode; }
            set { _nextNode = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///		Copys all the data contained in this scene node to another scene node.
        /// </summary>
        /// <param name="node">Scene node to copy data into.</param>
        public override void CopyTo(SceneNode node)
        {
            PathMarkerNode markerNode = node as PathMarkerNode;
            if (markerNode == null) return;

            base.CopyTo(node);

            // Copy emitter stuff
            markerNode._speed = new StartFinishF(_speed.Start, _speed.Finish);
            markerNode._delay = _delay;
            markerNode._nextNodeName = _nextNodeName;
        }

        /// <summary>
        ///		Saves this entity into a given binary writer.
        /// </summary>
        /// <param name="writer">Binary writer to save this entity into.</param>
        public override void Save(BinaryWriter writer)
        {
            // Save all the basic entity details.
            base.Save(writer);

            // Save all the emitter specific details.
            writer.Write(_speed.Start);
            writer.Write(_speed.Finish);
            writer.Write(_delay);
            writer.Write(_nextNodeName);
        }

        /// <summary>
        ///		Loads this scene node from a given binary reader.
        /// </summary>
        /// <param name="reader">Binary reader to load this scene node from.</param>
        public override void Load(BinaryReader reader)
        {
            // Load all the basic entity details.
            base.Load(reader);

            // Load all the emitter specific details.
            _speed = new StartFinishF(reader.ReadSingle(), reader.ReadSingle());
            _delay = reader.ReadInt32();
            _nextNodeName = reader.ReadString();
        }

        /// <summary>
        ///		Renders the event lines of this entity.
        /// </summary>
        /// <param name="transformation">Transformation to render event lines at.</param>
        protected override void RenderEventLines(Transformation transformation, CameraNode camera)
        {
            GraphicsManager.PushRenderState();
            GraphicsManager.ClearRenderState();
            GraphicsManager.DepthBufferEnabled = false;
            GraphicsManager.VertexColors.AllVertexs = _eventLineColor;

            // Normal.
            foreach (SceneNode node in _eventNodes)
            {
                EntityNode entityNode = node as EntityNode;
                if (entityNode == null) continue;

                Transformation relativeTransformation = entityNode.CalculateTransformation(camera);
                int x1 = (int)(transformation.X + ((_boundingRectangle.Width * transformation.ScaleX) / 2));
                int y1 = (int)(transformation.Y + ((_boundingRectangle.Height * transformation.ScaleY) / 2));
                int x2 = (int)(relativeTransformation.X + ((entityNode.BoundingRectangle.Width * relativeTransformation.ScaleX) / 2));
                int y2 = (int)(relativeTransformation.Y + ((entityNode.BoundingRectangle.Height * relativeTransformation.ScaleY) / 2));
                GraphicsManager.RenderLine(x1, y1, transformation.Z, x2, y2, relativeTransformation.Z);
            }

            // Our special ones!
            GraphicsManager.VertexColors.AllVertexs = _pathLineColor;
            if (_nextNode != null)
            {
                Transformation relativeTransformation = _nextNode.CalculateTransformation(camera);
                int x1 = (int)(transformation.X + ((_boundingRectangle.Width * transformation.ScaleX) / 2));
                int y1 = (int)(transformation.Y + ((_boundingRectangle.Height * transformation.ScaleY) / 2));
                int x2 = (int)(relativeTransformation.X + ((_nextNode._boundingRectangle.Width * relativeTransformation.ScaleX) / 2));
                int y2 = (int)(relativeTransformation.Y + ((_nextNode._boundingRectangle.Height * relativeTransformation.ScaleY) / 2));
                GraphicsManager.RenderLine(x1, y1, transformation.Z, x2, y2, relativeTransformation.Z);
            }

            GraphicsManager.PopRenderState();
        }

        /// <summary>
        ///		Initializes a new instance and gives it the specified  
        ///		name.
        /// </summary>
        /// <param name="name">Name to name as node to.</param>
        /// <param name="speed">Starting and finishing speed of this path marker.</param>
        public PathMarkerNode(string name, StartFinishF speed)
        {
            _name = name;
            _speed = Speed;
            _visible = false;
        }
        public PathMarkerNode()
        {
            _name = "Path Marker";
        }

        #endregion
    }
}