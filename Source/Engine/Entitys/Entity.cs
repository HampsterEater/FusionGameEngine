/*
 * File: Entity.cs
 *
 * Contains the main declaration of the entity class which
 * is used as a base for anything render-able.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using BinaryPhoenix.Fusion.Graphics;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Resources;
using BinaryPhoenix.Fusion.Runtime.Collision;
using BinaryPhoenix.Fusion.Runtime.Events;
using BinaryPhoenix.Fusion.Runtime.Scripting;

namespace BinaryPhoenix.Fusion.Engine.Entitys
{

	/// <summary>
	///		Describes the mode in which an entity is rendered.
	/// </summary>
	public enum EntityRenderMode
	{
		None,
		Image,
        [Description("Contained Image")]
		ContainedImage,
        [Description("Tiled Image")]
		TiledImage,
        [Description("Contained And Tiled Image")]
		ContainedTiledImage,
		Rectangle,
        [Description("Hollow Rectangle")]
        HollowRectangle,
		Oval,
        [Description("Hollow Oval")]
		HollowOval,
		Pixel,
		Text,
        Mesh
	}

    /// <summary>
    ///     Specifies how an entity should be depth sorted.
    /// </summary>
    public enum EntityDepthMode
    {
        Normal,
        AddYCoordinate,
        AddXCoordinate,
        SubtractXCoordinate,
        SubtractYCoordinate,
        AddCollisionBoxTop,
        AddCollisionBoxBottom,
        AddCollisionBoxLeft,
        AddCollisionBoxRight,
        SubtractCollisionBoxTop,
        SubtractCollisionBoxBottom,
        SubtractCollisionBoxLeft,
        SubtractCollisionBoxRight,
        AddBoundingBoxTop,
        AddBoundingBoxBottom,
        AddBoundingBoxLeft,
        AddBoundingBoxRight,
        SubtractBoundingBoxTop,
        SubtractBoundingBoxBottom,
        SubtractBoundingBoxLeft,
        SubtractBoundingBoxRight
    }

	/// <summary>
	///		The entity class is used as a base for anything render-able.
	/// </summary>
	public class EntityNode : SceneNode
	{
		#region Members
		#region Variables

        public static double CollectiveTime = 0.0f;
        public static HighPreformanceTimer CollectiveTimer = new HighPreformanceTimer();
        public static int CollectiveCalls = 0;

        protected static bool _forceGlobalDebugVisibility = false;
        protected static bool _forceGlobalVisibility = false;
        protected static bool _forceGlobalEventLineVisibility = false;
        protected static bool _forceGlobalBoundingBoxVisibility = false;
        protected static bool _forceGlobalCollisionBoxVisibility = false;

		protected bool _forceVisibility = false;
		protected bool _forceBoundingBoxVisibility = false;
        protected bool _forceCollisionBoxVisibility = false;

		protected bool _visible  = true;
		protected bool _enabled = true, _previousEnabled = true;
		protected bool _solid = false;

		protected string _event = "";
		protected bool _renderEventLines = false;
		protected ArrayList _eventNodes = new ArrayList();
		protected int _eventLineColor = unchecked((int)0xFFFF0000);

        protected Graphics.Mesh _mesh;
		protected Graphics.Image _image;
		protected int _frame;
		protected EntityRenderMode _renderMode = EntityRenderMode.Rectangle;
		protected Rectangle _boundingRectangle = new Rectangle(0, 0, 16, 16);
        protected Rectangle _collisionRectangle = new Rectangle(0, 0, 16, 16);

		protected int _color = unchecked((int)0xFFFFFFFF);
		protected BlendMode _blendMode = BlendMode.Alpha;

		protected string _text = "";
		protected BitmapFont _bitmapFont = null;

        protected Shader _shader = null;

		protected bool _renderBoundingBox = false;
        protected bool _renderCollisionBox = false;
		protected bool _renderSizingPoints = false;
		protected int _boundingBoxColor = unchecked((int)0xFF0000FF);
        protected int _collisionBoxColor = unchecked((int)0xFFFF00FF);
		protected int _sizingPointsColor = unchecked((int)0xFFFFFFFF);
        protected int _debugInfoColor = unchecked((int)0xFFFF5500);
        protected int _sizingPointsSize = 5;

        protected CollisionPolygon _collisionPolygon = null;

        protected bool _triggered = false;

        protected string _type = "";

		#endregion
		#region Properties

        /// <summary>
        ///		Gets or sets if entitys should be rendered with debug information..
        /// </summary>
        public static bool ForceGlobalDebugVisibility
        {
            get { return _forceGlobalDebugVisibility; }
            set { _forceGlobalDebugVisibility = value; }
        }

        /// <summary>
        ///		Gets or sets if entitys should be rendered regardless to the IsVisible property.
        /// </summary>
        public static bool ForceGlobalVisibility
        {
            get { return _forceGlobalVisibility; }
            set { _forceGlobalVisibility = value; }
        }

        /// <summary>
        ///		Gets or sets if all entitys event lines should be rendered regardless of its settings.
        /// </summary>
        public static bool ForceGlobalEventLineVisibility
        {
            get { return _forceGlobalEventLineVisibility; }
            set { _forceGlobalEventLineVisibility = value; }
        }

        /// <summary>
        ///		Gets or sets if this entitys bounding box should be rendered regardless to the IsBoundingBoxVisible property.
        /// </summary>
        public static bool ForceGlobalBoundingBoxVisibility
        {
            get { return _forceGlobalBoundingBoxVisibility; }
            set { _forceGlobalBoundingBoxVisibility = value; }
        }

        /// <summary>
        ///		Gets or sets if this entitys collision box should be rendered regardless to the IsCollisionBoxVisible property.
        /// </summary>
        public static bool ForceGlobalCollisionBoxVisibility
        {
            get { return _forceGlobalCollisionBoxVisibility; }
            set { _forceGlobalCollisionBoxVisibility = value; }
        }

        /// <summary>
        ///		Returns a string which can be used to identify what kind of entity this is.
        /// </summary>
        public virtual string Type
        {
            get { return _type != "" ? _type : "entity"; }
            set { _type = value; }
        }

		/// <summary>
		///		Gets or sets if event lines should be rendered between this node
		///		and all nodes with the same name as this nodes event.
		/// </summary>
		public bool IsEventLinesVisible
		{
			get { return _renderEventLines; }
			set { _renderEventLines = value; }
		}

		/// <summary>
		///		Gets or sets the color this entity nodes event lines should
		///		be rendered in.
		/// </summary>
		public int EventLineColor
		{
			get { return _eventLineColor; }
			set { _eventLineColor = value; }
		}

        /// <summary>
        ///		Gets or sets the color this entity nodes debug information
        ///     is drawn in.
        /// </summary>
        public int DebugInfoColor
        {
            get { return _debugInfoColor; }
            set { _debugInfoColor = value; }
        }

		/// <summary>
		///		Gets or sets a list of nodes that event lines should be rendered between.
		/// </summary>
		public ArrayList EventNodes
		{
			get { return _eventNodes; }
			set { _eventNodes = value; }
		}

		/// <summary>
		///		Gets or sets if this entity should be rendered regardless to the IsVisible property.
		/// </summary>
		public bool ForceVisibility
		{
			get { return _forceVisibility; }
			set { _forceVisibility = value; }
		}

		/// <summary>
		///		Gets or sets if this entitys bounding box should be rendered regardless to the IsBoundingBoxVisible property.
		/// </summary>
		public bool ForceBoundingBoxVisibility
		{
			get { return _forceBoundingBoxVisibility; }
			set { _forceBoundingBoxVisibility = value; }
		}

        /// <summary>
        ///		Gets or sets if this entitys collision box should be rendered regardless to the IsCollisionBoxVisible property.
        /// </summary>
        public bool ForceCollisionBoxVisibility
        {
            get { return _forceCollisionBoxVisibility; }
            set { _forceCollisionBoxVisibility = value; }
        }

		/// <summary>
		///		Gets or sets if this entity should be rendered or not.
		/// </summary>
		public bool IsVisible
		{
			get { return _visible;  }
			set { _visible = value; }
		}

		/// <summary>
		///		Gets or sets if this entity is enabled or not.
		/// </summary>
		public bool IsEnabled
		{
			get { return _enabled; }
			set { _enabled = value; }
		}

        /// <summary>
        ///		Gets or sets the previous enabled state of this entity.
        /// </summary>
        public bool PreviousIsEnabled
        {
            get { return _previousEnabled; }
            set { _previousEnabled = value; }
        }

		/// <summary>
		///		Gets or sets if this entity is solid or not.
		/// </summary>
		public bool IsSolid
		{
			get { return _solid; }
			set { _solid = value; }
		}

		/// <summary>
		///		Gets or sets if this entity's bounding box should be rendered or not.
		/// </summary>
		public bool IsBoundingBoxVisible
		{
			get { return _renderBoundingBox; }
			set { _renderBoundingBox = value; }
		}

		/// <summary>
		///		Gets or sets the color of this entitys bounding box.
		/// </summary>
		public int BoundingBoxColor
		{
			get { return _boundingBoxColor; }
			set { _boundingBoxColor = value; }
		}

        /// <summary>
        ///		Gets or sets if this entity's collision box should be rendered or not.
        /// </summary>
        public bool IsCollisionBoxVisible
        {
            get { return _renderCollisionBox; }
            set { _renderCollisionBox = value; }
        }

        /// <summary>
        ///		Gets or sets the color of this entitys collision box.
        /// </summary>
        public int CollisionBoxColor
        {
            get { return _collisionBoxColor; }
            set { _collisionBoxColor = value; }
        }

		/// <summary>
		///		Gets or sets if this entity's sizing points should be rendered or not.
		/// </summary>
		public bool IsSizingPointsVisible
		{
			get { return _renderSizingPoints; }
			set { _renderSizingPoints = value; }
		}

		/// <summary>
		///		Gets or sets the size of this entity's sizing points..
		/// </summary>
		public int SizingPointsSize
		{
			get { return _sizingPointsSize; }
			set { _sizingPointsSize = value; }
		}

		/// <summary>
		///		Gets or sets the color of this entitys sizing points.
		/// </summary>
		public int SizingPointsColor
		{
			get { return _sizingPointsColor; }
			set { _sizingPointsColor = value; }
		}

		/// <summary>
		///		Gets or sets the image used to render this entity.
		/// </summary>
		public Graphics.Image Image
		{
			get { return _image; }
			set { _image = value; }
		}

        /// <summary>
        ///		Gets or sets the mesh used to render this entity.
        /// </summary>
        public Graphics.Mesh Mesh
        {
            get { return _mesh; }
            set { _mesh = value; }
        }
		
		/// <summary>
		///		Gets or sets the frame to use when rendering this
		///		entitys image.
		/// </summary>
		public int Frame
		{
			get { return _frame; }
            set { _frame = value; }
		}

		/// <summary>
		///		Gets or sets the mode to use when rendering this entity.
		/// </summary>
		public EntityRenderMode RenderMode
		{
			get { return _renderMode; }
			set { _renderMode = value; }
		}

		/// <summary>
		///		Gets or sets the bounding rectangle which is used for culling.
		/// </summary>
		public virtual Rectangle BoundingRectangle
		{
			get { return _boundingRectangle; }
			set { _boundingRectangle = value; }
		}

        /// <summary>
        ///		Gets or sets the collision rectangle which is used for collision checking.
        /// </summary>
        public virtual Rectangle CollisionRectangle
        {
            get { return _collisionRectangle; }
            set { _collisionRectangle = value; }
        }

		/// <summary>
		///		Gets or sets the width of this entity, this is mainly used for primitive 
		///		rendering.
		/// </summary>
		public override int Width
		{
			get { return _boundingRectangle.Width; }
			set { _boundingRectangle.Width = value; }
		}

		/// <summary>
		///		Gets or sets the height of this entity, this is mainly used for primitive 
		///		rendering.
		/// </summary>
		public override int Height
		{
			get { return _boundingRectangle.Height; }
			set { _boundingRectangle.Height = value; }
		}

		/// <summary>
		///		Gets or sets the transformation of this entity.
		/// </summary>
		public override Transformation Transformation
		{
			get { return _transformation; }
			set { _transformation = value; }
		}

		/// <summary>
		///		Gets or sets the color to tint this entity when rendering.
		/// </summary>
		public int Color 
		{
			get { return _color; }
			set { _color = value; }
		}

		/// <summary>
		///		Gets or sets the blend mode to use when rendering this entity.
		/// </summary>
		public BlendMode BlendMode
		{
			get { return _blendMode; }
			set { _blendMode = value; }
		}

		/// <summary>
		///		Gets or sets the font used to render text in the Text render mode.
		/// </summary>
		public BitmapFont Font
		{
			get { return _bitmapFont; }
			set { _bitmapFont = value; }
		}

        /// <summary>
        ///		Gets or sets the shader applied this to entity.
        /// </summary>
        public Shader Shader
        {
            get { return _shader; }
            set { _shader = value; }
        }

		/// <summary>
		///		Gets or sets the text rendered in the Text render mode.
		/// </summary>
		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}

		/// <summary>
		///		Gets or sets the name of the entity that should be triggered when this entitys event is invoked.
		/// </summary>
		public string Event
		{
			get { return _event; }
			set { _event = value; }
		}

		/// <summary>
		///		Gets or sets the collision polygon attached to this entity.
		/// </summary>
		public CollisionPolygon CollisionPolygon
		{
			get { return _collisionPolygon; }
			set { _collisionPolygon = value; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///     Called just before this polygon is put through collision processing.
		/// </summary>
		public void PreCollisionProcessing()
		{
            Transformation parentTransformation = _parent != null ? _parent.CalculateTransformation() : new Transformation(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f);
            Transformation transformation = new Transformation(parentTransformation.X + _transformation.X, parentTransformation.Y + _transformation.Y, parentTransformation.Z + _transformation.Z, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f);
            
            transformation.X += _collisionRectangle.X;
            transformation.Y += _collisionRectangle.Y;

            _collisionPolygon.Transformation = transformation;
			_collisionPolygon.Solid = _solid;
			if (_collisionPolygon is CollisionRectangle)
			{
				((CollisionRectangle)_collisionPolygon).BoundingWidth = _collisionRectangle.Width;
                ((CollisionRectangle)_collisionPolygon).BoundingHeight = _collisionRectangle.Height;
			}
		}

        /// <summary>
        ///     Called just after this polygon is put through collision processing.
        /// </summary>
		public void PostCollisionProcessing()
		{
            if (_solid == true)
            {
                Transformation parentTransformation = _parent != null ? _parent.CalculateTransformation() : new Transformation(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f);
                
                Transformation newTransformation = _collisionPolygon.Transformation;
                newTransformation.X -= parentTransformation.X;
                newTransformation.Y -= parentTransformation.Y;
                newTransformation.X -= _collisionRectangle.X;
                newTransformation.Y -= _collisionRectangle.Y;

                Position(newTransformation.X, newTransformation.Y, _transformation.Z);

                //_transformation.X = newTransformation.X;
                //_transformation.Y = newTransformation.Y;
			 //   _solid = _collisionPolygon.Solid;
            }
        }

        /// <summary>
        ///     When called it invokes the OnTrigger event of all event nodes.
        /// </summary>
        public virtual void TriggerEvents()
        {
            if (_eventNodes.Count == 0)
                return;

            // Trigger all entities.
            foreach (SceneNode node in _eventNodes)
                if (node != null && node is EntityNode)
                    ((EntityNode)node).OnTrigger(this);

            // Flag that we have already been triggered.
            _triggered = true;
        }

        /// <summary>
        ///     Called when this entity is triggered by another entities event.
        /// </summary>
        /// <param name="node">The node that triggered this one.</param>
        protected virtual void OnTrigger(EntityNode node)
        {
            if (_enabled == false)
                _enabled = true;

            TriggerEvents();
        }

        /// <summary>
        ///		Sets up this entitys event listener and collision polygon when called.
        /// </summary>
        public void InitializeCollision()
        {
            _collisionPolygon = new CollisionRectangle(new Transformation(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f), 0.0f, 0.0f);
            _collisionPolygon.MetaData = this;
            _collisionPolygon.PreProcessingDelegate += new CollisionProcessingNotificationDelegate(PreCollisionProcessing);
            _collisionPolygon.PostProcessingDelegate += new CollisionProcessingNotificationDelegate(PostCollisionProcessing);
            CollisionManager.AttachPolygon(_collisionPolygon);
        }

        /// <summary>
        ///		Removes this entitys event listener and collision polygon when called.
        /// </summary>
        public void DeinitializeCollision()
        {
            if (_collisionPolygon == null) return;
            CollisionManager.DetachPolygon(_collisionPolygon);
            _collisionPolygon.PreProcessingDelegate = null;
            _collisionPolygon.PostProcessingDelegate = null;
            _collisionPolygon.OnEnterDelegate = null;
            _collisionPolygon.OnLeaveDelegate = null;
            _collisionPolygon.OnTouchDelegate = null;
            _collisionPolygon.OnCollidingDelegate = null;
            _collisionPolygon.MetaData = null;
            _collisionPolygon = null;
        }

        /// <summary>
        ///     Responsable for removing references of this object and deallocated
        ///     resources that have been allocated.
        /// </summary>
        public override void Dispose()
        {
            // Call the abstracted base method to clean up general things.
            base.Dispose();

            // Deinitialize this entitys collision.
            DeinitializeCollision();

            // Nullify media.
            _image = null;
            _bitmapFont = null;

            // Remove event nodes.
            _eventNodes.Clear();
        }

		/// <summary>
		///		Copys all the data contained in this scene node to another scene node.
		/// </summary>
		/// <param name="node">Scene node to copy data into.</param>
		public override void CopyTo(SceneNode node)
		{
			EntityNode entityNode = node as EntityNode;
			if (entityNode == null) return;
			base.CopyTo(node);

			entityNode._forceVisibility = _forceVisibility;
			entityNode._forceBoundingBoxVisibility = _forceBoundingBoxVisibility;
			entityNode._parent = _parent;
			entityNode._name = _name;
			entityNode._bitmapFont = _bitmapFont;
			entityNode._blendMode = _blendMode;
			entityNode._boundingBoxColor = _boundingBoxColor;
			entityNode._boundingRectangle = _boundingRectangle;
			entityNode._color = _color;
			entityNode._frame = _frame;
			entityNode._image = _image;
            entityNode._mesh = _mesh;
			entityNode._renderBoundingBox = _renderBoundingBox;
			entityNode._renderMode = _renderMode;
			entityNode._renderSizingPoints = _renderSizingPoints;
			entityNode._sizingPointsSize = _sizingPointsSize;
			entityNode._text = _text;
			entityNode._transformation = _transformation;
			entityNode._visible = _visible;
			entityNode._enabled = _enabled;
			entityNode._event = _event;
			entityNode._solid = _solid;
			entityNode._renderEventLines = _renderEventLines;
            entityNode._renderCollisionBox = _renderCollisionBox;
            entityNode._collisionRectangle = _collisionRectangle;
            entityNode._depthMode = _depthMode;
            entityNode._depthLayer = _depthLayer;
            entityNode._shader = _shader;
            if (entityNode._collisionPolygon == null && _collisionPolygon != null) entityNode._collisionPolygon = new CollisionRectangle();
            if (_collisionPolygon != null) entityNode._collisionPolygon.Layers = (int[])_collisionPolygon.Layers.Clone();

			foreach (SceneNode eventNode in entityNode._eventNodes)
				_eventNodes.Add(eventNode);
			entityNode._eventLineColor = unchecked((int)0xFF000000);
		}

		/// <summary>
		///		Resizes this entity to the given dimensions (in pixels)
		/// </summary>
		/// <param name="width">Width to resize entity to.</param>
		/// <param name="height">Height to resize entity to.</param>
		public virtual void Resize(int width, int height)
		{
			_boundingRectangle.Width = width;
			_boundingRectangle.Height = height;
            _collisionRectangle.Width = width;
            _collisionRectangle.Height = height;
		}

		/// <summary>
		///		Calculates this entitys relative transformation to another transformation.
		/// </summary>
		/// <param name="transformation">Transformation to calculate relative transformation from.</param>
		/// <returns>Transfromation relative to the given transfromation.</returns>
        public override Transformation CalculateRelativeTransformation(Transformation transformation)
		{
            //CollectiveTimer.Restart();

            Transformation newTransform = new Transformation();
            newTransform.AngleX = transformation.AngleX + _transformation.AngleX;
            newTransform.AngleY = transformation.AngleY + _transformation.AngleY;
            newTransform.AngleZ = transformation.AngleZ + _transformation.AngleZ;
            newTransform.ScaleX = transformation.ScaleX + (_transformation.ScaleX < 0.0f ? -_transformation.ScaleX : _transformation.ScaleX) - 1.0f;
            newTransform.ScaleY = transformation.ScaleY + (_transformation.ScaleY < 0.0f ? -_transformation.ScaleY : _transformation.ScaleY) - 1.0f;
            newTransform.ScaleZ = transformation.ScaleZ + (_transformation.ScaleZ < 0.0f ? -_transformation.ScaleZ : _transformation.ScaleZ) - 1.0f;
            
            if (_transformation.ScaleX < 0) newTransform.ScaleX = -newTransform.ScaleX;
            if (_transformation.ScaleY < 0) newTransform.ScaleY = -newTransform.ScaleY;
            if (_transformation.ScaleZ < 0) newTransform.ScaleZ = -newTransform.ScaleZ;

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
                case EntityDepthMode.AddCollisionBoxTop: depthOffset = (newTransform.Y + _collisionRectangle.Top); break;
                case EntityDepthMode.AddCollisionBoxBottom: depthOffset = (newTransform.Y + _collisionRectangle.Bottom); break;
                case EntityDepthMode.AddCollisionBoxLeft: depthOffset = (newTransform.X + _collisionRectangle.Left); break;
                case EntityDepthMode.AddCollisionBoxRight: depthOffset = (newTransform.X + _collisionRectangle.Right); break;
                case EntityDepthMode.SubtractCollisionBoxTop: depthOffset = -(newTransform.Y + _collisionRectangle.Top); break;
                case EntityDepthMode.SubtractCollisionBoxBottom: depthOffset = -(newTransform.Y + _collisionRectangle.Bottom); break;
                case EntityDepthMode.SubtractCollisionBoxLeft: depthOffset = -(newTransform.X + _collisionRectangle.Left); break;
                case EntityDepthMode.SubtractCollisionBoxRight: depthOffset = -(newTransform.X + _collisionRectangle.Right); break;
                case EntityDepthMode.AddBoundingBoxTop: depthOffset = (newTransform.Y + _boundingRectangle.Top); break;
                case EntityDepthMode.AddBoundingBoxBottom: depthOffset = (newTransform.Y + _boundingRectangle.Bottom); break;
                case EntityDepthMode.AddBoundingBoxLeft: depthOffset = (newTransform.X + _boundingRectangle.Left); break;
                case EntityDepthMode.AddBoundingBoxRight: depthOffset = (newTransform.X + _boundingRectangle.Right); break;
                case EntityDepthMode.SubtractBoundingBoxTop: depthOffset = -(newTransform.Y + _boundingRectangle.Top); break;
                case EntityDepthMode.SubtractBoundingBoxBottom: depthOffset = -(newTransform.Y + _boundingRectangle.Bottom); break;
                case EntityDepthMode.SubtractBoundingBoxLeft: depthOffset = -(newTransform.X + _boundingRectangle.Left); break;
                case EntityDepthMode.SubtractBoundingBoxRight: depthOffset = -(newTransform.X + _boundingRectangle.Right); break;
            }

            newTransform.Z = (transformation.Z) - (depthOffset + _transformation.Z + _transformationOffset.Z);

            //CollectiveCalls++;
            //CollectiveTime += CollectiveTimer.DurationMillisecond;

            return newTransform;
		}

		/// <summary>
		///		Gets the transformation of this entity relative to all the nodes above it, including
		///		the given camera.
		/// </summary>
		/// <param name="camera">Camera to get transformation relative to.</param>
		public Transformation CalculateTransformation(CameraNode camera)
		{
			ArrayList parentNodes = new ArrayList();
			SceneNode parent = this.Parent;
			while (parent != null)
			{
				parentNodes.Add(parent);
				parent = parent.Parent;
			}
			parentNodes.Reverse();

            Transformation relativeTransformation = camera.CalculateRelativeTransformation(new Transformation(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f));
            foreach (SceneNode node in parentNodes)
                relativeTransformation = node.CalculateRelativeTransformation(relativeTransformation);
   
			return CalculateRelativeTransformation(relativeTransformation);
		}

		/// <summary>
		///		Checks if this nodes bounding box intersects with the given rectangle.
		/// </summary>
		/// <param name="rectangle">Rectangle to check intersecting against.</param>
		public virtual bool RectangleBoundingBoxIntersect(Rectangle rectangle)
		{
			Transformation relativeTransformation = CalculateTransformation();
			Rectangle relativeRectangle = new Rectangle((int)relativeTransformation.X, (int)relativeTransformation.Y, (int)(_boundingRectangle.Width * Math.Abs(relativeTransformation.ScaleX)), (int)(_boundingRectangle.Height * Math.Abs(relativeTransformation.ScaleY)));
			return rectangle.IntersectsWith(relativeRectangle);
		}
		public virtual bool RectangleBoundingBoxIntersect(Rectangle rectangle, CameraNode camera)
		{
			Transformation relativeTransformation = CalculateTransformation(camera);
			Rectangle relativeRectangle = new Rectangle((int)relativeTransformation.X, (int)relativeTransformation.Y, (int)(_boundingRectangle.Width * Math.Abs(relativeTransformation.ScaleX)), (int)(_boundingRectangle.Height * Math.Abs(relativeTransformation.ScaleY)));
			return rectangle.IntersectsWith(relativeRectangle);
		}

        /// <summary>
        ///     Returns true if the given rectangle collides with any of the sizing points of this entity.
        /// </summary>
        /// <param name="rectangle">Entry to check insection against.</param>
        /// <param name="camera">Camera to check inserection from.</param>
        /// <returns>True if rectangle and a sizing point collide.</returns>
        public bool RectangleSizingPointsIntersect(Rectangle rectangle, CameraNode camera)
        {
            Transformation transformation = CalculateTransformation(camera);
            int x = (int)(transformation.X - (_sizingPointsSize / 2)), y = (int)(transformation.Y - (_sizingPointsSize / 2));
            int w = (int)(_boundingRectangle.Width * camera.Zoom), h = (int)(_boundingRectangle.Height * camera.Zoom);

            if (rectangle.IntersectsWith(new Rectangle(x, y, _sizingPointsSize, _sizingPointsSize)))
                return true;
            else if (rectangle.IntersectsWith(new Rectangle(x + w, y, _sizingPointsSize, _sizingPointsSize)))
                return true;
            else if (rectangle.IntersectsWith(new Rectangle(x, y + h, _sizingPointsSize, _sizingPointsSize)))
                return true;
            else if (rectangle.IntersectsWith(new Rectangle(x + w, y + h, _sizingPointsSize, _sizingPointsSize)))
                return true;
            else if (rectangle.IntersectsWith(new Rectangle(x + (w / 2), y, _sizingPointsSize, _sizingPointsSize)))
                return true;
            else if (rectangle.IntersectsWith(new Rectangle(x + (w / 2), y + h, _sizingPointsSize, _sizingPointsSize)))
                return true;
            else if (rectangle.IntersectsWith(new Rectangle(x, y + (h / 2), _sizingPointsSize, _sizingPointsSize)))
                return true;
            else if (rectangle.IntersectsWith(new Rectangle(x + w, y + (h / 2), _sizingPointsSize, _sizingPointsSize)))
                return true;
            else
                return false;
        }

        /// <summary>
        ///		Checks if this nodes collision box intersects with the given rectangle.
        /// </summary>
        /// <param name="rectangle">Rectangle to check intersecting against.</param>
        public virtual bool RectangleCollisionBoxIntersect(Rectangle rectangle)
        {
            Transformation relativeTransformation = CalculateTransformation();
            Rectangle relativeRectangle = new Rectangle((int)relativeTransformation.X, (int)relativeTransformation.Y, (int)(_collisionRectangle.Width * Math.Abs(relativeTransformation.ScaleX)), (int)(_collisionRectangle.Height * Math.Abs(relativeTransformation.ScaleY)));
            return rectangle.IntersectsWith(relativeRectangle);
        }
        public virtual bool RectangleCollisionBoxIntersect(Rectangle rectangle, CameraNode camera)
        {
            Transformation relativeTransformation = CalculateTransformation(camera);
            Rectangle relativeRectangle = new Rectangle((int)relativeTransformation.X, (int)relativeTransformation.Y, (int)(_collisionRectangle.Width * Math.Abs(relativeTransformation.ScaleX)), (int)(_collisionRectangle.Height * Math.Abs(relativeTransformation.ScaleY)));
            return rectangle.IntersectsWith(relativeRectangle);
        }

		/// <summary>
		///		Returns true if this entity is within the viewport of the current render target.
		/// </summary>
		/// <param name="transformation">Position this entity should be checked from.</param>
		/// <returns>True if this entity can be seen.</returns>
		public bool CanBeSeen(Transformation transformation)
		{
            return MathMethods.RectanglesOverlap((int)transformation.X, (int)transformation.Y, (int)(Width * Math.Abs(transformation.ScaleX)), (int)(Height * Math.Abs(transformation.ScaleY)), 0, 0, GraphicsManager.Resolution[0], GraphicsManager.Resolution[1]);
        }

		/// <summary>
		///		Renders the bounding box of this entity.
		/// </summary>
		/// <param name="transformation">Transformation to render bounding box at.</param>
        protected void RenderBoundingBox(Transformation transformation, CameraNode camera)
		{
			GraphicsManager.PushRenderState();
			GraphicsManager.ClearRenderState();
            GraphicsManager.DepthBufferEnabled = false;
            GraphicsManager.VertexColors.AllVertexs = _boundingBoxColor;
            GraphicsManager.RenderRectangle(transformation.X, transformation.Y, transformation.Z, Math.Abs(_boundingRectangle.Width * transformation.ScaleX), _boundingRectangle.Height * Math.Abs(transformation.ScaleY), false);
			GraphicsManager.PopRenderState();
		}

        /// <summary>
        ///		Renders the collision box of this entity.
        /// </summary>
        /// <param name="transformation">Transformation to render collisionbox at.</param>
        protected void RenderCollisionBox(Transformation transformation, CameraNode camera)
        {
            if (_solid == false)
                return;

            GraphicsManager.PushRenderState();
            GraphicsManager.ClearRenderState();
            GraphicsManager.DepthBufferEnabled = false;
            GraphicsManager.VertexColors.AllVertexs = _collisionBoxColor;
            GraphicsManager.RenderRectangle(transformation.X + (_collisionRectangle.X * transformation.ScaleX), transformation.Y + (_collisionRectangle.Y * transformation.ScaleY), transformation.Z, Math.Abs(_collisionRectangle.Width * transformation.ScaleX), _collisionRectangle.Height * Math.Abs(transformation.ScaleY), false);
            GraphicsManager.PopRenderState();
        }

		/// <summary>
		///		Renders the sizing points of this entity.
		/// </summary>
		/// <param name="transformation">Transformation to render sizing points at.</param>
        protected void RenderSizingPoints(Transformation transformation, CameraNode camera)
		{
			GraphicsManager.PushRenderState();
			GraphicsManager.ClearRenderState();
            GraphicsManager.DepthBufferEnabled = false;
			GraphicsManager.VertexColors.AllVertexs = _sizingPointsColor;

			int halfSize = _sizingPointsSize / 2;
			int x = (int)(transformation.X - halfSize);
			int y = (int)(transformation.Y - halfSize);
			int w = (int)(_boundingRectangle.Width * Math.Abs(transformation.ScaleX));
			int h = (int)(_boundingRectangle.Height * Math.Abs(transformation.ScaleY));
            GraphicsManager.RenderRectangle(x, y, transformation.Z, _sizingPointsSize, _sizingPointsSize, false);
            GraphicsManager.RenderRectangle(x + w, y, transformation.Z, _sizingPointsSize, _sizingPointsSize, false);
            GraphicsManager.RenderRectangle(x + (w / 2), y, transformation.Z, _sizingPointsSize, _sizingPointsSize, false);
            GraphicsManager.RenderRectangle(x + w, y + h, transformation.Z, _sizingPointsSize, _sizingPointsSize, false);
            GraphicsManager.RenderRectangle(x + w, y + (h / 2), transformation.Z, _sizingPointsSize, _sizingPointsSize, false);
            GraphicsManager.RenderRectangle(x, y + h, transformation.Z, _sizingPointsSize, _sizingPointsSize, false);
            GraphicsManager.RenderRectangle(x, y + (h / 2), transformation.Z, _sizingPointsSize, _sizingPointsSize, false);
            GraphicsManager.RenderRectangle(x + (w / 2), y + h, transformation.Z, _sizingPointsSize, _sizingPointsSize, false);
			
			GraphicsManager.PopRenderState();
		}

		/// <summary>
		///		Renders the event lines of this entity.
		/// </summary>
		/// <param name="transformation">Transformation to render event lines at.</param>
        protected virtual void RenderEventLines(Transformation transformation, CameraNode camera)
		{
			GraphicsManager.PushRenderState();
			GraphicsManager.ClearRenderState();
            GraphicsManager.DepthBufferEnabled = false;
			GraphicsManager.VertexColors.AllVertexs = _eventLineColor;
            
			foreach (SceneNode node in _eventNodes)
			{
				EntityNode entityNode = node as EntityNode;
				if (entityNode == null) continue;
				
                Transformation relativeTransformation = entityNode.CalculateTransformation(camera);
				int x1 = (int)(transformation.X + ((_boundingRectangle.Width * transformation.ScaleX) / 2));
				int y1 = (int)(transformation.Y + ((_boundingRectangle.Height * transformation.ScaleY) / 2));
				int x2 = (int)(relativeTransformation.X + ((entityNode._boundingRectangle.Width * relativeTransformation.ScaleX) / 2));
				int y2 = (int)(relativeTransformation.Y + ((entityNode._boundingRectangle.Height * relativeTransformation.ScaleY) / 2));
				GraphicsManager.RenderLine(x1, y1, transformation.Z, x2, y2, relativeTransformation.Z);
			}
			
			GraphicsManager.PopRenderState();
		}

        /// <summary>
        ///		Renders the debug information of this entity.
        /// </summary>
        /// <param name="transformation">Transformation to render debug information at.</param>
        protected void RenderDebug(Transformation transformation, CameraNode camera)
        {
            GraphicsManager.PushRenderState();
            GraphicsManager.ClearRenderState();
            GraphicsManager.DepthBufferEnabled = false;

            GraphicsManager.ScaleFactor = new float[3] { 1.0f, 1.0f, 1.0f };
            float[] resolutionScale = GraphicsManager.ResolutionScale;
            GraphicsManager.ResolutionScale = new float[2] { 1.0f, 1.0f };

            string debugText = "(" + transformation.X + "," + transformation.Y + "," + transformation.Z + ") (" + transformation.ScaleX + "," + transformation.ScaleY + "," + transformation.ScaleZ + ") (" + transformation.AngleX + "," + transformation.AngleY + "," + transformation.AngleZ + ")";
            GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFF000000);
            GraphicsManager.RenderRectangle((transformation.X * resolutionScale[0]), (transformation.Y * resolutionScale[1]) - 30, transformation.Z, GraphicsManager.BitmapFont.TextWidth(debugText, false), GraphicsManager.BitmapFont.TextHeight(debugText, false));
            GraphicsManager.VertexColors.AllVertexs = _debugInfoColor;
            GraphicsManager.RenderText(debugText, (transformation.X * resolutionScale[0]), (transformation.Y * resolutionScale[1]) - 30, transformation.Z);
            
            debugText = "[Solid:\"" + _solid + "\" Visible:\""+_visible+"\" Frame:\""+_frame+"\" Event:\""+_event+"\" Name:\""+_name+"\"]";
            GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFF000000);
            GraphicsManager.RenderRectangle((transformation.X * resolutionScale[0]), (transformation.Y * resolutionScale[1]) - 15, transformation.Z, GraphicsManager.BitmapFont.TextWidth(debugText, false), GraphicsManager.BitmapFont.TextHeight(debugText, false));
            GraphicsManager.VertexColors.AllVertexs = _debugInfoColor;
            GraphicsManager.RenderText(debugText, (transformation.X * resolutionScale[0]), (transformation.Y * resolutionScale[1]) - 15, transformation.Z);

            GraphicsManager.PopRenderState();
        }

        /// <summary>
        ///     Sets up the rendering state so this entity is rendered correctly.
        /// </summary>
        /// <param name="relativeTransformation">Transformation used to set scale and rotation factors.</param>
        protected void SetupRenderingState(Transformation relativeTransformation)
        {
            GraphicsManager.VertexColors.AllVertexs = _color;
            if (GraphicsManager.BlendMode != _blendMode) GraphicsManager.BlendMode = _blendMode;
            if (GraphicsManager.ScaleFactor[0] != relativeTransformation.ScaleX ||
                GraphicsManager.ScaleFactor[1] != relativeTransformation.ScaleY ||
                GraphicsManager.ScaleFactor[2] != relativeTransformation.ScaleZ)
            {
                GraphicsManager.ScaleFactor[0] = relativeTransformation.ScaleX;
                GraphicsManager.ScaleFactor[1] = relativeTransformation.ScaleY;
                GraphicsManager.ScaleFactor[2] = relativeTransformation.ScaleZ;
            }

            if (GraphicsManager.RotationAngle[0] != relativeTransformation.AngleX ||
                GraphicsManager.RotationAngle[1] != relativeTransformation.AngleY ||
                GraphicsManager.RotationAngle[2] != relativeTransformation.AngleZ)
            {
                GraphicsManager.RotationAngle[0] = relativeTransformation.AngleX;
                GraphicsManager.RotationAngle[1] = relativeTransformation.AngleY;
                GraphicsManager.RotationAngle[2] = relativeTransformation.AngleZ;
            }

            if (_bitmapFont != null) GraphicsManager.BitmapFont = _bitmapFont;
        }

		/// <summary>
		///		Renders this entity at a position releative to its parent.
		/// </summary>
		/// <param name="position">Position where this entity's parent node was rendered.</param>
        public override void Render(Transformation transformation, CameraNode camera, int layer)
		{
            // Setup rendering mode.
            //HighPreformanceTimer timer = new HighPreformanceTimer();
            Transformation relativeTransformation = CalculateRelativeTransformation(transformation);
            SetupRenderingState(relativeTransformation);
            //if (timer.DurationMillisecond > 1.0f)
            //    System.Console.WriteLine((_image != null ? _image.URL : this.ToString()) + " worked out transformation in " + timer.DurationMillisecond);
            //timer.Restart();

            // Are we currnetly rendering the layer this entity is on?
            if (layer != _depthLayer)
            {
                if (_visible == true) RenderChildren(relativeTransformation, camera, layer);
                return;
            }
 
            if ((_visible == true || _forceVisibility == true) && (_renderMode == EntityRenderMode.TiledImage || CanBeSeen(relativeTransformation) == true))
			{
                //Statistics.StoreInt("Nodes Rendered", Statistics.ReadInt("Nodes Rendered") + 1);

                Shader previousShader = null;
                if (_shader != null)
                {
                    previousShader = GraphicsManager.Shader;
                    GraphicsManager.Shader = _shader;
                }

                switch (_renderMode)
				{
					case EntityRenderMode.Image:
						if (_image == null) break;
                        GraphicsManager.RenderImage(_image, relativeTransformation.X + (((_boundingRectangle.Width - _image.Width) * transformation.ScaleX) / 2), relativeTransformation.Y + (((_boundingRectangle.Height - _image.Height) * transformation.ScaleY) / 2), relativeTransformation.Z, _frame);
						break;
					case EntityRenderMode.ContainedImage:
						{
							if (_image == null) break;
							Rectangle originalViewport = GraphicsManager.Viewport;
                            GraphicsManager.Viewport = new Rectangle((int)relativeTransformation.X,
                                                                     (int)relativeTransformation.Y,
                                                                     (int)(_boundingRectangle.Width * Math.Abs(relativeTransformation.ScaleX)),
                                                                     (int)(_boundingRectangle.Height * Math.Abs(relativeTransformation.ScaleY))); 
                            GraphicsManager.RenderImage(_image, relativeTransformation.X + (((_boundingRectangle.Width - _image.Width) * transformation.ScaleX) / 2), relativeTransformation.Y + (((_boundingRectangle.Height - _image.Height) * transformation.ScaleY) / 2), relativeTransformation.Z, _frame);
							GraphicsManager.Viewport = originalViewport;
						}
						break;
					case EntityRenderMode.TiledImage:
						if (_image == null) break;
						GraphicsManager.TileImage(_image, relativeTransformation.X, relativeTransformation.Y, relativeTransformation.Z, _frame);
						break;
					case EntityRenderMode.ContainedTiledImage:
						{
							if (_image == null) break;
							Rectangle originalViewport = GraphicsManager.Viewport;
                            GraphicsManager.Viewport = new Rectangle((int)relativeTransformation.X, 
                                                                     (int)relativeTransformation.Y, 
                                                                     (int)(_boundingRectangle.Width * Math.Abs(relativeTransformation.ScaleX)),
                                                                     (int)(_boundingRectangle.Height * Math.Abs(relativeTransformation.ScaleY)));
                            GraphicsManager.TileImage(_image, relativeTransformation.X, relativeTransformation.Y, relativeTransformation.Z, _frame);
							GraphicsManager.Viewport = originalViewport;
						}
						break;
					case EntityRenderMode.Oval:
						GraphicsManager.RenderOval(relativeTransformation.X + (_boundingRectangle.X * transformation.ScaleX), relativeTransformation.Y + (_boundingRectangle.Y * transformation.ScaleY), relativeTransformation.Z, _boundingRectangle.Width, _boundingRectangle.Height);
						break;
					case EntityRenderMode.HollowOval:
                        GraphicsManager.RenderOval(relativeTransformation.X + (_boundingRectangle.X * transformation.ScaleX), relativeTransformation.Y + (_boundingRectangle.Y * transformation.ScaleY), relativeTransformation.Z, _boundingRectangle.Width, _boundingRectangle.Height, false);
						break;
					case EntityRenderMode.Pixel:
						GraphicsManager.RenderPixel(relativeTransformation.X + ((_boundingRectangle.Width * transformation.ScaleX) / 2), relativeTransformation.Y + ((_boundingRectangle.Height * transformation.ScaleX) / 2), relativeTransformation.Z);
						break;
					case EntityRenderMode.Rectangle:
                        GraphicsManager.RenderRectangle(relativeTransformation.X + (_boundingRectangle.X * transformation.ScaleX), relativeTransformation.Y + (_boundingRectangle.Y * transformation.ScaleY), relativeTransformation.Z, _boundingRectangle.Width, _boundingRectangle.Height);
						break;
					case EntityRenderMode.HollowRectangle:
                        GraphicsManager.RenderRectangle(relativeTransformation.X + (_boundingRectangle.X * transformation.ScaleX), relativeTransformation.Y + (_boundingRectangle.Y * transformation.ScaleY), relativeTransformation.Z, _boundingRectangle.Width, _boundingRectangle.Height, false);
						break;
					case EntityRenderMode.Text:
						GraphicsManager.RenderText(_text, relativeTransformation.X + (((_boundingRectangle.Width - GraphicsManager.TextWidth(_text, true)) * transformation.ScaleX) / 2), relativeTransformation.Y + (((_boundingRectangle.Height - GraphicsManager.TextHeight(_text, true)) * transformation.ScaleY) / 2), relativeTransformation.Z, true);
						break;
                    case EntityRenderMode.Mesh:
                        if (_mesh == null) break;
                        GraphicsManager.RenderMesh(_mesh, relativeTransformation.X + (((_boundingRectangle.Width - _mesh.Width) * transformation.ScaleX) / 2), relativeTransformation.Y + (((_boundingRectangle.Height - _mesh.Height) * transformation.ScaleY) / 2), relativeTransformation.Z);
                        break;
				}

                if (_shader != null)
                    GraphicsManager.Shader = previousShader;
			}

			// Render bits and pieces that are required.
            if (_renderCollisionBox == true || _forceCollisionBoxVisibility == true || _forceGlobalCollisionBoxVisibility) RenderCollisionBox(relativeTransformation, camera);
            if (_renderBoundingBox == true || _forceBoundingBoxVisibility == true || _forceGlobalBoundingBoxVisibility) RenderBoundingBox(relativeTransformation, camera);
            if (_renderSizingPoints == true) RenderSizingPoints(relativeTransformation, camera);
			if (_renderEventLines == true || _forceGlobalEventLineVisibility) RenderEventLines(relativeTransformation, camera);
            if (_forceGlobalDebugVisibility == true) RenderDebug(relativeTransformation, camera);

            //if (timer.DurationMillisecond > 1.0f)
            //    System.Console.WriteLine((_image != null ? _image.URL : this.ToString()) + " rendered in " + timer.DurationMillisecond);

			// Render all the children of this entity.
			if (_visible == true) RenderChildren(relativeTransformation, camera, layer);
		}

		/// <summary>
		///		Saves this entity into a given binary writer.
		/// </summary>
		/// <param name="writer">Binary writer to save this entity into.</param>
		public override void Save(BinaryWriter writer)
		{
			// Save the basic properties of this entity,
            base.Save(writer);
			writer.Write(_event);

			// Write in a bitmask representing the basic flags of this entity.
			int flagMask = 0;
			if (_visible == true) flagMask |= 1;
			if (_enabled == true) flagMask |= 2;
			if (_solid == true) flagMask |= 4;
			if (_image != null && _image.URL != null) flagMask |= 8;
			if (_bitmapFont != null && _bitmapFont.URL != null) flagMask |= 16;
			if (_text != null) flagMask |= 32;
			if (_boundingRectangle.X != 0 || _boundingRectangle.Y != 0) flagMask |= 64;
			if (_boundingRectangle.Width != 0 || _boundingRectangle.Height != 0) flagMask |= 128;
			if (_frame != 0) flagMask |= 256;
			if (_renderMode != EntityRenderMode.Image) flagMask |= 512;
			if (_blendMode != BlendMode.Alpha) flagMask |= 1024;
			if (_color != unchecked((int)0xFFFFFFFF)) flagMask |= 2048;	
			if (_collisionPolygon != null) flagMask |= 4096;
            if (_collisionRectangle.X != 0 || _collisionRectangle.Y != 0) flagMask |= 8192;
            if (_collisionRectangle.Width != 0 || _collisionRectangle.Height != 0) flagMask |= 16384;
			if (_depthLayer != 0) flagMask |= 32768;
            if (_depthMode != EntityDepthMode.Normal) flagMask |= 65536;
            if (_shader != null) flagMask |= 131072;
            if (_mesh != null) flagMask |= 262144;
            writer.Write(flagMask);

            if (_mesh != null && _mesh.URL != null)
            {
                writer.Write(_mesh.URL);
            }

			if (_image != null && _image.URL != null)
			{
				writer.Write(_image.URL);
				writer.Write(_image.Width);
				writer.Write(_image.Height);
				writer.Write((short)_image.VerticalSpacing);
				writer.Write((short)_image.HorizontalSpacing);
			}
			if (_bitmapFont != null && _bitmapFont.URL != null) 
				writer.Write(_bitmapFont.URL);
            if (_text != null) 
                writer.Write(_text);
            if (_boundingRectangle.X != 0 || _boundingRectangle.Y != 0)
            {
                writer.Write(_boundingRectangle.X);
                writer.Write(_boundingRectangle.Y);
            }
            if (_boundingRectangle.Width != 0 || _boundingRectangle.Height != 0)
            {
                writer.Write(_boundingRectangle.Width);
                writer.Write(_boundingRectangle.Height);
            }
			if (_frame != 0) 
                writer.Write(_frame);
			if (_renderMode != EntityRenderMode.Image) 
                writer.Write((byte)_renderMode);
            if (_blendMode != BlendMode.Alpha) 
                writer.Write((byte)_blendMode);
            if (_color != unchecked((int)0xFFFFFFFF))
                writer.Write(_color);
            if (_collisionPolygon != null)
            {
                writer.Write((int)_collisionPolygon.Layers.Length);
                for (int i = 0; i < _collisionPolygon.Layers.Length; i++)
                    writer.Write(_collisionPolygon.Layers[i]);
            }		
            if (_collisionRectangle.X != 0 || _collisionRectangle.Y != 0)
            {
                writer.Write(_collisionRectangle.X);
                writer.Write(_collisionRectangle.Y);
            }
            if (_collisionRectangle.Width != 0 || _collisionRectangle.Height != 0)
            {
                writer.Write(_collisionRectangle.Width);
                writer.Write(_collisionRectangle.Height);
            }

            if (_depthLayer != 0) 
                writer.Write(_depthLayer);

            if (_depthMode != EntityDepthMode.Normal) 
                writer.Write((byte)_depthMode);

            if (_shader != null && _shader.URL != null)
                writer.Write(_shader.URL);
		}

		/// <summary>
		///		Loads this scene node from a given binary reader.
		/// </summary>
		/// <param name="reader">Binary reader to load this scene node from.</param>
		public override void Load(BinaryReader reader)
		{
            base.Load(reader);
			_event = reader.ReadString();
			
			int flagMask = reader.ReadInt32();
			_visible = (flagMask & 1) != 0;
			_enabled = (flagMask & 2) != 0;
			_solid = (flagMask & 4) != 0;

            if ((flagMask & 262144) != 0)
            {
                string meshUrl = reader.ReadString();
                if (ResourceManager.ResourceExists(meshUrl) == true)
                    _mesh = GraphicsManager.LoadMesh(meshUrl, 0);
            }

			if ((flagMask & 8) != 0)
			{
				string imageUrl = reader.ReadString();
				int cellWidth = reader.ReadInt32();
				int cellHeight = reader.ReadInt32();
				int hSpacing = reader.ReadInt16();
				int vSpacing = reader.ReadInt16();
				if (ResourceManager.ResourceExists(imageUrl) == true) 
					_image = GraphicsManager.LoadImage(imageUrl,cellWidth,cellHeight,hSpacing,vSpacing, 0);
            }
			if ((flagMask & 16) != 0)
			{
				string fontUrl = reader.ReadString();
				if (ResourceManager.ResourceExists(fontUrl) == true)
					_bitmapFont = GraphicsManager.LoadFont(fontUrl);
			}
            if ((flagMask & 32) != 0) _text = reader.ReadString();
            if ((flagMask & 64) != 0)
            {
                _boundingRectangle.X = reader.ReadInt32();
                _boundingRectangle.Y = reader.ReadInt32();
            }
            if ((flagMask & 128) != 0)
            {
                _boundingRectangle.Width = reader.ReadInt32();
                _boundingRectangle.Height = reader.ReadInt32();
            }
			if ((flagMask & 256) != 0) _frame = reader.ReadInt32();

			if ((flagMask & 512) != 0)
				_renderMode = (EntityRenderMode)reader.ReadByte();
			else
				_renderMode = EntityRenderMode.Image;

            if ((flagMask & 1024) != 0)
                _blendMode = (BlendMode)reader.ReadByte();
            else
                _blendMode = BlendMode.Alpha;

            if ((flagMask & 2048) != 0)
                _color = reader.ReadInt32();
            else
                _color = unchecked((int)0xFFFFFFFF);

            if ((flagMask & 4096) != 0)
            {
                if (_collisionPolygon == null) InitializeCollision();
                int count = reader.ReadInt32();
                _collisionPolygon.Layers = new int[count];
                for (int i = 0; i < count; i++)
                    _collisionPolygon.Layers[i] = reader.ReadInt32();
            }
            if ((flagMask & 8192) != 0)
            {
                _collisionRectangle.X = reader.ReadInt32();
                _collisionRectangle.Y = reader.ReadInt32();
            }
            if ((flagMask & 16384) != 0)
            {
                _collisionRectangle.Width = reader.ReadInt32();
                _collisionRectangle.Height = reader.ReadInt32();
            }

            if ((flagMask & 32768) != 0) 
                _depthLayer = reader.ReadInt32();

            if ((flagMask & 65536) != 0) 
                _depthMode = (EntityDepthMode)reader.ReadByte();

            if ((flagMask & 131072) != 0)
                _shader = GraphicsManager.LoadShader(reader.ReadString());
		}

        /// <summary>
        ///     Resets this entity to the state it was in when it was created.
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (_collisionPolygon != null) DeinitializeCollision();

		    _forceVisibility = false;
		    _forceBoundingBoxVisibility = false;
            _forceCollisionBoxVisibility = false;

		    _visible  = true;
		    _enabled = true;
		    _solid = false;

		    _event = "";
		    _renderEventLines = false;
		    _eventNodes.Clear();
		    _eventLineColor = unchecked((int)0xFFFF0000);

            _image = null;
            _mesh = null;
            _frame = 0;
		    _renderMode = EntityRenderMode.Rectangle;
		    _boundingRectangle = new Rectangle(0, 0, 16, 16);
            _collisionRectangle = new Rectangle(0, 0, 16, 16);

		    _color = unchecked((int)0xFFFFFFFF);
		    _blendMode = BlendMode.Alpha;

		    _text = "";
		    _bitmapFont = null;

		    _renderBoundingBox = false;
            _renderCollisionBox = false;
		    _renderSizingPoints = false;
		    _boundingBoxColor = unchecked((int)0xFF0000FF);
            _collisionBoxColor = unchecked((int)0xFF666666);
		    _sizingPointsColor = unchecked((int)0xFFFFFFFF);
		    _sizingPointsSize = 5;

            _collisionPolygon = null;

            _depthLayer = 0;
            _depthMode = EntityDepthMode.SubtractCollisionBoxBottom;

            _triggered = false;

            _shader = null;
        }

		/// <summary>
		///		Initializes a new instance and gives it the specified  
		///		name.
		/// </summary>
		/// <param name="name">Name to name as node to.</param>	
        /// <param name="noCollision">If set to true then collision will not be initialized for this entity.</param>
        public EntityNode(string name, bool noCollision)
		{
			_name = name;
            if (noCollision == false)
                InitializeCollision();
		}
		public EntityNode(string name)
		{
			_name = name;
			InitializeCollision();
		}
		public EntityNode() // Fixs some derived ctor errors.
		{ 
			_name = "Entity";
			InitializeCollision();
		} 

		#endregion
	}

}