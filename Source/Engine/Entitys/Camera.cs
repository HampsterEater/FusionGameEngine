/*
 * File: Camera.cs
 *
 * Contains the main declaration of the camera class
 * which is a derivitive of the SceneNode class and is
 * used to control the projection of the current camera
 * to allow special effects like viewports/zooming/rotation...etc
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using BinaryPhoenix.Fusion.Graphics;
using BinaryPhoenix.Fusion.Engine.Processes;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Resources;
using BinaryPhoenix.Fusion.Runtime.Processes;

namespace BinaryPhoenix.Fusion.Engine.Entitys
{

	/// <summary>
	///		The camera class is a derivitive of the SceneNode class and is
	///		used to control the projection of the current camera
	///		to allow special effects like viewports/zooming/rotation...etc
	/// </summary>
	public class CameraNode : EntityNode
	{
		#region Members
		#region Variables

		protected Rectangle _viewport = new Rectangle(0, 0, GraphicsManager.Resolution[0], GraphicsManager.Resolution[1]);
		protected int _clearColor = unchecked((int)0xFFFFFFFF);
		protected Graphics.Image _backgroundImage = null;

		protected float _zoom = 1.0f;

        protected float _shakeIntensity = 0;

		#endregion
		#region Properties

        /// <summary>
        ///		Returns a string which can be used to identify what kind of entity this is.
        /// </summary>
        public override string Type
        {
            get { return _type != "" ? _type : "camera"; }
        }

		/// <summary>
		///		Gets or sets the viewport used to render child nodes of
		///		this camera in.
		/// </summary>
		public Rectangle Viewport
		{
			get { return _viewport; }
            set { _viewport = value; _boundingRectangle.Width = _viewport.Width; _boundingRectangle.Height = _viewport.Height; }
		}

        /// <summary>
        ///     Gets or sets the bounding box of this camera, this is 
        ///     exactly the same as the viewport.
        /// </summary>
        public override Rectangle BoundingRectangle
        {
            get { return _viewport; }
            set { _viewport = value; }
        }

		/// <summary>
		///		Gets or sets the color to clear this cameras viewport on
		///		each render.
		/// </summary>
		public int ClearColor
		{
			get { return _clearColor;  }
			set { _clearColor = value; }
		}

		/// <summary>
		///		Gets or sets the background color that should be tiled across the
		///		screen before the camera renders its children.
		/// </summary>
		public Graphics.Image BackgroundImage
		{
			get { return _backgroundImage; }
			set { _backgroundImage = value; }
		}

		/// <summary>
		///		Gets or sets the zoom factor of this camera.
		/// </summary>
		public float Zoom
		{
			get { return _zoom; }
			set { _zoom = value; }
		}

        /// <summary>
        ///     Gets or sets the intensity to shake the camera.
        /// </summary>
        public float ShakeIntensity
        {
            get { return _shakeIntensity; }
            set { _shakeIntensity = value; }
        }

        /// <summary>
        ///		Gets or sets the width of this entity in pixels.
        /// </summary>
        public override int Width
        {
            get { return _viewport.Width; }
            set { _viewport.Width = value; }
        }

        /// <summary>
        ///		Gets or sets the height of this tilemaps segment in pixels.
        /// </summary>
        public override int Height
        {
            get { return _viewport.Height; }
            set { _viewport.Height = value; }
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
			CameraNode cameraNode = node as CameraNode;
			if (cameraNode == null) return;

			base.CopyTo(node);
			cameraNode._viewport = new Rectangle(_viewport.X, _viewport.Y, _viewport.Width, _viewport.Height);
            cameraNode._boundingRectangle.Width = _viewport.Width; 
            cameraNode._boundingRectangle.Height = _viewport.Height;
            cameraNode._clearColor = _clearColor;
			cameraNode._backgroundImage = _backgroundImage;
			cameraNode._zoom = _zoom;
		}

		/// <summary>
		///		Calculates this entitys relative transformation to another transformation.
		/// </summary>
		/// <param name="transformation">Transformation to calculate relative transformation from.</param>
		/// <returns>Transfromation relative to the given transfromation.</returns>
        public override Transformation CalculateRelativeTransformation(Transformation transformation)
		{
			Transformation newTransform = new Transformation();

			newTransform.AngleX = transformation.AngleX + _transformation.AngleX;
            newTransform.AngleY = transformation.AngleY + _transformation.AngleY;
            newTransform.AngleZ = transformation.AngleZ + _transformation.AngleZ;
			newTransform.ScaleX = (transformation.ScaleX + (_transformation.ScaleX - 1.0f)) + (_zoom - 1.0f);
			newTransform.ScaleY = (transformation.ScaleY + (_transformation.ScaleY - 1.0f)) + (_zoom - 1.0f);
            newTransform.ScaleZ = (transformation.ScaleZ + (_transformation.ScaleZ - 1.0f)) + (_zoom - 1.0f);

            newTransform.X = (float)Math.Ceiling(((transformation.X - (_transformation.X + _transformationOffset.X)) * newTransform.ScaleX) + _viewport.X);
            newTransform.Y = (float)Math.Ceiling(((transformation.Y - (_transformation.Y + _transformationOffset.Y)) * newTransform.ScaleY) + _viewport.Y);
            newTransform.Z = transformation.Z + (_transformation.Z + _transformationOffset.Z);
            
            return newTransform;
		}

		/// <summary>
		///		Checks if this node intersects with the given rectangle.
		/// </summary>
		/// <param name="transformation">Transformation of this nodes parent.</param>
		/// <param name="collisionList">List to add intersecting nodes to.</param>
		/// <param name="rectangle">Rectangle to check intersecting against.</param>
		public override bool RectangleBoundingBoxIntersect(Rectangle rectangle)
		{
			return false;
		}

		/// <summary>
		///		Sets the rendering state so that all children of this 
		///		camera are drawn correctly.
		/// </summary>
		/// <param name="position">Position where this entity's parent node was rendered.</param>
        public override void Render(Transformation transformation, CameraNode camera, int layer)
		{
            //Statistics.StoreInt("Nodes Rendered", Statistics.ReadInt("Nodes Rendered") + 1);

			Transformation relativeTransformation = CalculateRelativeTransformation(transformation);
            SetupRenderingState(relativeTransformation);

			GraphicsManager.ClearColor	= _clearColor;
			GraphicsManager.Viewport	= _viewport;
			GraphicsManager.ClearScene();

			if (_backgroundImage != null)
			{
				GraphicsManager.ScaleFactor = new float[] { _zoom, _zoom, 1.0f };
                GraphicsManager.TileImage(_backgroundImage, relativeTransformation.X, relativeTransformation.Y, relativeTransformation.Z);
			
                // Clear up the depth buffer so this image is always at the back.
                GraphicsManager.ClearDepthBuffer();
            }

            // Set the audio listener position at this cameras current position.
            Audio.AudioManager.ListenerPosition = new Vector((-relativeTransformation.X) + (_viewport.Width / 2), -(relativeTransformation.Y) + (_viewport.Height / 2), 0.0f);

			RenderChildren(relativeTransformation, camera, layer);
		}

		/// <summary>
		///		Saves this entity into a given binary writer.
		/// </summary>
		/// <param name="writer">Binary writer to save this entity into.</param>
		public override void Save(BinaryWriter writer)
		{
			// Save all the basic entity details.
			base.Save(writer);

			// Save all the camera specific details.
			writer.Write((short)_viewport.X);
			writer.Write((short)_viewport.Y);
			writer.Write((short)_viewport.Width);
			writer.Write((short)_viewport.Height);

			writer.Write(_clearColor);
			writer.Write(_zoom);

			writer.Write((_backgroundImage != null && _backgroundImage.URL != null));
			if (_backgroundImage != null && _backgroundImage.URL != null)
			{
				writer.Write(_backgroundImage.URL);
				writer.Write(_backgroundImage.Width);
				writer.Write(_backgroundImage.Height);
				writer.Write((short)_backgroundImage.VerticalSpacing);
				writer.Write((short)_backgroundImage.HorizontalSpacing);
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

			// Load all the camera specific details.
			_viewport.X = reader.ReadInt16();
			_viewport.Y = reader.ReadInt16();
			_viewport.Width = reader.ReadInt16();
			_viewport.Height = reader.ReadInt16();
            _boundingRectangle.Width = _viewport.Width; 
            _boundingRectangle.Height = _viewport.Height;

			_clearColor = reader.ReadInt32();
			_zoom = reader.ReadSingle();

			if (reader.ReadBoolean() == true)
			{
				string imageUrl = reader.ReadString();
				int cellWidth = reader.ReadInt32();
				int cellHeight = reader.ReadInt32();
				int hSpacing = reader.ReadInt16();
				int vSpacing = reader.ReadInt16();
				if (ResourceManager.ResourceExists(imageUrl) == true)
					_backgroundImage = GraphicsManager.LoadImage(imageUrl, cellWidth, cellHeight, hSpacing, vSpacing, 0);
			}
		}

		/// <summary>
		///		Initializes a new instance and gives it the specified  
		///		name.
		/// </summary>
		/// <param name="name">Name to name as node to.</param>
		public CameraNode(string name)
		{
			_name = name;
		}
		public CameraNode() 
		{
			_name = "Camera";
		} 

		#endregion
	}

}