/*
 * File: Tilemap.cs
 *
 * Contains the main declaration of the tilemap class which
 * is used to render and update a tilemap segment.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.Xml;
using System.IO;
using BinaryPhoenix.Fusion.Graphics;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Resources;

namespace BinaryPhoenix.Fusion.Engine.Entitys
{

	/// <summary>
	///		Describes the mode in which a tilemap is loaded and displayed..
	/// </summary>
	public enum TilemapMode
	{
		Continuous,
		Static,
	}

	/// <summary>
	///		The tilemap segment class is used to describe a small rectangular 
	///		segment of tiles.
	/// </summary>
	public class TilemapSegmentNode : EntityNode
	{
		#region Members
		#region Variables

		private TileNode[,] _tileData = null;
        private int _width = 16, _height = 16;
		private int _tileWidth = 16, _tileHeight = 16;

		private Image _preRenderedImage = null;

		private bool _renderGrid = false;
		private int _gridColor = unchecked((int)0xFF137F7F);

		#endregion
		#region Properties

        /// <summary>
        ///		Returns a string which can be used to identify what kind of entity this is.
        /// </summary>
        public override string Type
        {
            get { return _type != "" ? _type : "tilemapsegment"; }
        }

		/// <summary>
		///		Gets or sets a tile at the specific position in this tilemap segment
		/// </summary>
		/// <param name="x">Position on the x-axis of tile to get or set.</param>
		/// <param name="y">Position on the y-axis of tile to get or set.</param>
		/// <returns>Tile at the given position on this tilemap segment.</returns>
		public TileNode this[int x, int y]
		{
			get { return _tileData[x, y]; }
			set { _tileData[x, y] = value; }
		}

		/// <summary>
		///		Gets or sets if this tilemaps grid should be rendered or not.
		/// </summary>
		public bool IsGridVisible
		{
			get { return _renderGrid; }
			set { _renderGrid = value; }
		}

		/// <summary>
		///		Gets or sets the color of this tilemaps grid..
		/// </summary>
		public int GridColor
		{
			get { return _gridColor; }
			set { _gridColor = value; }
		}

		/// <summary>
		///		Gets or sets the width of all tiles in this segment.
		/// </summary>
		public int TileWidth
		{
			get { return _tileWidth; }
			set
			{
				_tileWidth = value;
				_boundingRectangle.Width = _width * _tileWidth;
			}
		}

		/// <summary>
		///		Gets or sets the height of all tiles in this segment.
		/// </summary>
		public int TileHeight
		{
			get { return _tileHeight; }
			set
			{
				_tileHeight = value;
				_boundingRectangle.Height = _height * _tileHeight;
			}
		}

		/// <summary>
		///		Gets or sets the width of this tilemaps segment in pixels.
		/// </summary>
		public override int Width
		{
            get { return _boundingRectangle.Width; } // _width* _tileWidth; }
			set { Resize(value, _height); }
		}

		/// <summary>
		///		Gets or sets the height of this tilemaps segment in pixels.
		/// </summary>
		public override int Height
		{
            get { return _boundingRectangle.Height; } //_height * _tileHeight; }
			set { Resize(_width, value); }
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
			TilemapSegmentNode newTilemap = node as TilemapSegmentNode;
			if (newTilemap == null) return;

			base.CopyTo(node);
			newTilemap._preRenderedImage = _preRenderedImage;
			newTilemap._renderGrid = _renderGrid;
			newTilemap._gridColor = _gridColor;

			newTilemap.Resize(_width * _tileWidth, _height * _tileHeight); // Don't ask >.>.
			newTilemap._tileWidth = _tileWidth;
			newTilemap._tileHeight = _tileHeight;

			for (int x = 0; x < _width; x++)
				for (int y = 0; y < _height; y++)
				{
					if (_tileData[x, y] == null) continue;
					newTilemap._tileData[x, y] = _tileData[x, y].Clone() as TileNode;
                    newTilemap._tileData[x, y].Parent = newTilemap;
                    this.RemoveChild(newTilemap._tileData[x, y]);
                    newTilemap.AddChild(newTilemap._tileData[x, y]);
                }
		}

		/// <summary>
		///		Renders this tilemap segment at a relative position to its parent node.
		/// </summary>
		/// <param name="transformation">Transformation of parent node.</param>
        public override void Render(Transformation transformation, CameraNode camera, int layer)
		{
			if (_tileData == null) return;

			// Work out where and how we should be rendered.
			Transformation relativeTransformation = CalculateRelativeTransformation(transformation);
            SetupRenderingState(relativeTransformation);

            // Are we currnetly rendering the layer this entity is on? Or are we not visible.
            if (layer != _depthLayer || CanBeSeen(relativeTransformation) == false)
            {
                if (_visible == true) RenderChildren(relativeTransformation, camera, layer);
                return;
            }

            //Statistics.StoreInt("Nodes Rendered", Statistics.ReadInt("Nodes Rendered") + 1);

			// Work out if all the tiles are visible and what are not.
			int segmentLeft = (int)(-((relativeTransformation.X / _tileWidth) / relativeTransformation.ScaleX) - 1);
			int segmentTop = (int)(-((relativeTransformation.Y / _tileHeight) / relativeTransformation.ScaleY) - 1);
			int segmentRight = (int)(segmentLeft + ((GraphicsManager.Resolution[0] / _tileWidth) / relativeTransformation.ScaleX) + 2);
			int segmentBottom = (int)(segmentTop + ((GraphicsManager.Resolution[1] / _tileHeight) / relativeTransformation.ScaleY) + 2);

			// Cap all the segments off so we don't try to draw outside
			// the bounds of this array segment.
			segmentLeft = Math.Min(_width - 1, Math.Max(0, segmentLeft));
			segmentTop = Math.Min(_height - 1, Math.Max(0, segmentTop));
			segmentRight = Math.Min(_width, Math.Max(0, segmentRight));
			segmentBottom = Math.Min(_height, Math.Max(0, segmentBottom));

			if (_visible == true || _forceVisibility == true)
			{
				// Render normally if a prerendered version is not available.
				if (_preRenderedImage == null)
				{
                    //System.Console.WriteLine("Rendered tilemap segment of " + (segmentRight - segmentLeft) + "x" + (segmentBottom - segmentTop) + " (real size: " + (_width + "," + _height) + " tiles:" + ((segmentRight - segmentLeft) * (segmentBottom - segmentTop)) + ")");
					// Go through all the visible tiles and render them.
					for (int tileX = segmentLeft; tileX < segmentRight; tileX++)
						for (int tileY = segmentTop; tileY < segmentBottom; tileY++)
						{
                            if (_tileData[tileX, tileY] == null || _tileData[tileX, tileY].IsVisible == false || _tileData[tileX, tileY].Tileset == null || _tileData[tileX, tileY].Frame < 0 || _tileData[tileX, tileY].Frame >= _tileData[tileX, tileY].Tileset.Image.FrameCount) continue;
							_tileData[tileX, tileY].Render(relativeTransformation, camera, layer);
                            /*
                            float tileScaleX = relativeTransformation.ScaleX + Math.Abs(_tileData[tileX, tileY].Transformation.ScaleX) - 1.0f;
                            float tileScaleY = relativeTransformation.ScaleY + Math.Abs(_tileData[tileX, tileY].Transformation.ScaleY) - 1.0f;
                            if (tileScaleX < 0) tileScaleX = -tileScaleX;
                            if (tileScaleY < 0) tileScaleY = -tileScaleY;

                            float tx = relativeTransformation.X + ((tileX * _tileWidth) * tileScaleX);
                            float ty = relativeTransformation.Y + ((tileY * _tileHeight) * tileScaleY);

                            GraphicsManager.VertexColors.AllVertexs = _tileData[tileX, tileY].Color;
                            GraphicsManager.BlendMode = _tileData[tileX, tileY].BlendMode;
                            GraphicsManager.ScaleFactor = new float[] { tileScaleX, tileScaleY };
                            GraphicsManager.RenderImage(_tileData[tileX, tileY].Tileset.Image, tx, ty, relativeTransformation.Z, _tileData[tileX, tileY].Frame);
						*/}
                }

				// w00ties a prerender exists, time for a nice big speedup!
				else
					GraphicsManager.RenderImage(_preRenderedImage, relativeTransformation.X, relativeTransformation.Y, relativeTransformation.Z, 0);

				// If we have been asked to render a grid then do so.
				if (_renderGrid == true)
				{
                    float width = (_width * _tileWidth) * Math.Abs(relativeTransformation.ScaleX);
                    float height = (_height * _tileHeight) * Math.Abs(relativeTransformation.ScaleY);

					GraphicsManager.VertexColors.AllVertexs = _gridColor;
					for (int x = 0; x <= _width; x++)
					{
						float lX = (relativeTransformation.X + (x * (_tileWidth * Math.Abs(relativeTransformation.ScaleX))));
						float lY = relativeTransformation.Y;
						GraphicsManager.RenderLine(lX, lY, relativeTransformation.Z, lX, lY + height, relativeTransformation.Z);
					}
					for (int y = 0; y <= _height; y++)
					{
						float lX = relativeTransformation.X;
						float lY = (relativeTransformation.Y + (y * (_tileHeight * Math.Abs(relativeTransformation.ScaleY))));
						GraphicsManager.RenderLine(lX, lY, relativeTransformation.Z, lX + width, lY, relativeTransformation.Z);
					}

				}
			}

            // Render bits and pieces that are required.
            if (_renderCollisionBox == true || _forceCollisionBoxVisibility == true || _forceGlobalCollisionBoxVisibility) RenderCollisionBox(relativeTransformation, camera);
            if (_renderBoundingBox == true || _forceBoundingBoxVisibility == true || _forceGlobalBoundingBoxVisibility) RenderBoundingBox(relativeTransformation, camera);
            if (_renderSizingPoints == true) RenderSizingPoints(relativeTransformation, camera);
            if (_renderEventLines == true || _forceGlobalEventLineVisibility) RenderEventLines(relativeTransformation, camera);
            if (_forceGlobalDebugVisibility == true) RenderDebug(relativeTransformation, camera);

			// Render all the children of this entity.
			if (_visible == true) RenderChildren(relativeTransformation, camera, layer);
		}

		/// <summary>
		///		Renders this segment to an image buffer to speed up rendering.
		/// </summary>
		public void PreRenderSegment()
		{
			// Create an image to render to and set it as the render target.
			_preRenderedImage = new Image(_width * 16, _height * 16, ImageFlags.Dynamic);
			IRenderTarget previousTarget = GraphicsManager.RenderTarget;
			GraphicsManager.RenderTarget = (IRenderTarget)_preRenderedImage[0];
			GraphicsManager.BeginScene();
			GraphicsManager.ClearColor = unchecked((int)0xFFFF00FF);
			GraphicsManager.ClearScene();

			// Go through each tile and render it to this image.
			for (int tileX = 0; tileX < _width; tileX++)
				for (int tileY = 0; tileY < _height; tileY++)
				{
					if (_tileData[tileX, tileY] == null) continue;
					_tileData[tileX, tileY].Render(new Transformation(0,0,0,0,0,0,1,1,1), null, 0);
				}

			// Go back to rendering on the window.
			GraphicsManager.FinishScene();
            GraphicsManager.PresentScene();
			GraphicsManager.RenderTarget = previousTarget;
		}

		/// <summary>
		///		Resizes this entity to the given dimensions (in pixels)
		/// </summary>
		/// <param name="width">Width to resize entity to.</param>
		/// <param name="height">Height to resize entity to.</param>
		public override void Resize(int width, int height)
		{
			width /= _tileWidth;
			height /= _tileHeight;
			if (width <= 0) width = 1;
			if (height <= 0) height = 1;

			TileNode[,] newData = new TileNode[width, height];
			if (_tileData != null)
				for (int x = 0; x < width; x++)
					for (int y = 0; y < height; y++)
					{
						newData[x, y] = (x >= _width || y >= _height) ? new TileNode() : _tileData[x, y];
						newData[x, y].Position(x * _tileWidth, y * _tileHeight, newData[x, y].Transformation.Z);
                        newData[x, y].Parent = this;
                    }

			_tileData = newData;	
			_width = width;
			_height = height;
			_boundingRectangle.Width = _width * _tileWidth;
			_boundingRectangle.Height = _height * _tileHeight;
		}

		/// <summary>
		///		Saves this entity into a given binary writer.
		/// </summary>
		/// <param name="writer">Binary writer to save this entity into.</param>
		public override void Save(BinaryWriter writer)
		{
			// Save all the basic entity details.
			base.Save(writer);

			// Save all the tilemap specific details.
            writer.Write(_width);
            writer.Write(_height);
            writer.Write((short)_tileWidth);
			writer.Write((short)_tileHeight);

			// Save in details on all the tiles in this tilemap segment.
			for (int x = 0; x < _width; x++)
				for (int y = 0; y < _height; y++)
					_tileData[x, y].Save(writer);
		}

		/// <summary>
		///		Loads this tilemap segment node from a given binary reader.
		/// </summary>
		/// <param name="reader">Binary reader to load this tilemap segment node from.</param>
		public override void Load(BinaryReader reader)
		{
			// Load all the basic entity details.
			base.Load(reader);

			// Load all the tilemap specific details.
            _width = reader.ReadInt32();
            _height = reader.ReadInt32();
            _tileWidth = reader.ReadInt16();
			_tileHeight = reader.ReadInt16();

			// Load in all the tileset in this map.
			_tileData = new TileNode[_width, _height];
			for (int x = 0; x < _width; x++)
				for (int y = 0; y < _height; y++)
				{
					_tileData[x, y] = new TileNode();
                    if (_tileData[x, y].Parent != null)
                        _tileData[x, y].Parent.RemoveChild(_tileData[x, y]);
                    _tileData[x, y].Parent = this;
                    SceneGraph.AttachNode(_tileData[x, y]);
					_tileData[x, y].Load(reader);
				}
		}

		/// <summary>
		///		Initializes a new instance of this class with the given tile data.
		/// </summary>
		/// <param name="data">Data that this segment should contain.</param>
		/// <param name="width">Width in tiles of this segment.</param>
		/// <param name="height">Height in tiles of this segment.</param>
		/// <param name="tileWidth">Width of each tile in this segment.</param>
        /// <param name="tileHeight">Height of each tile in this segment.</param>
		public TilemapSegmentNode(TileNode[,] data, int width, int height, int tileWidth, int tileHeight)
		{
			_tileData = data;
			_width = width;
			_height = height;
			_tileWidth = tileWidth;
			_tileHeight = tileHeight;
			_boundingRectangle.Width = width * tileWidth;
			_boundingRectangle.Height = height * tileHeight;
			_name = "Tile Map Segment";
			DeinitializeCollision(); // Tiles look after collision - For the moment.
		}
		public TilemapSegmentNode(int width, int height, int tileWidth, int tileHeight)
		{
			_tileData = new TileNode[width, height];
			_width = width;
			_height = height;
			_tileWidth = tileWidth;
			_tileHeight = tileHeight;
			_boundingRectangle.Width = width * tileWidth;
			_boundingRectangle.Height = height * tileHeight;
			_name = "Tile Map Segment";
			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
				{
					_tileData[x, y] = new TileNode();
					_tileData[x, y].Position(x * _tileWidth, y * _tileHeight, _tileData[x, y].Transformation.Z);
                    _tileData[x, y].Parent = this;
                }
			DeinitializeCollision(); // Tilemaps do not produce collision.
		}
		public TilemapSegmentNode() 
		{
			_name = "Tile Map Segment";
			DeinitializeCollision(); // Tilemaps do not produce collision.
		}

		#endregion
	}

	/// <summary>
	///		Used to describe the transformation of a single tile in a tilemap segment.
	/// </summary>
	public class TileNode : EntityNode
	{
		#region Members
		#region Variables

		private Tileset _tileset = null;

		#endregion
		#region Properties

        /// <summary>
        ///		Returns a string which can be used to identify what kind of entity this is.
        /// </summary>
        public override string Type
        {
            get { return "tile"; }
        }

		/// <summary>
		///		Gets or sets the tileset this tile is using to render.
		/// </summary>
		public Tileset Tileset
		{
			get { return _tileset; }
			set 
			{ 
				_tileset = value; 
				if (_tileset != null) _image = _tileset.Image; 
			}
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Renders this entity.
		/// </summary>
		/// <param name="position">Position where this entity's parent node was rendered.</param>
        public override void Render(Transformation transformation, CameraNode camera, int layer)
		{
            if (_image == null) return;
            //Statistics.StoreInt("Nodes Rendered", Statistics.ReadInt("Nodes Rendered") + 1);
			Transformation relativeTransformation = CalculateRelativeTransformation(transformation);
            relativeTransformation.Z = transformation.Z;
            SetupRenderingState(relativeTransformation);
            GraphicsManager.RenderImage(_image, relativeTransformation.X, relativeTransformation.Y, relativeTransformation.Z, _frame);
		}

		/// <summary>
		///		Copys all the data contained in this scene node to another scene node.
		/// </summary>
		/// <param name="node">Scene node to copy data into.</param>
		public override void CopyTo(SceneNode node)
		{
			TileNode tileNode = node as TileNode;
			if (tileNode == null) return;

			base.CopyTo(node);
			tileNode._tileset = _tileset;
		}

		/// <summary>
		///		Saves this tile node into a given binary writer.
		/// </summary>
		/// <param name="writer">Binary writer to save this tile node into.</param>
		public override void Save(BinaryWriter writer)
		{
			// Save all the basic entity details.
			Image previousImage = _image; // This is just a crude way of stopping the Entity class of saving details about the image.
			_image = null;
			base.Save(writer);
			_image = previousImage;

			// Save all the tile node specific details.
			if (_tileset != null)
			{
				writer.Write(true);
				writer.Write((byte)Tileset.TilesetPool.IndexOf(_tileset));
			}
			else
				writer.Write(false);
		}

		/// <summary>
		///		Loads this tile node from a given binary reader.
		/// </summary>
		/// <param name="reader">Binary reader to load this tile node from.</param>
		public override void Load(BinaryReader reader)
		{
			// Load all the basic entity details.
			base.Load(reader);

			// Load all the tile node specific details.
			if (reader.ReadBoolean() == true)
			{
				int index = reader.ReadByte();
				_tileset = (Tileset)Tileset.TilesetPool[index % Tileset.TilesetPool.Count]; // Better to actually get a tileset, than get an error :P.
				_image = _tileset.Image;
			}
		}

		/// <summary>
		///		Initializes a new instance of this class with the given 
		///		rendering properties.
		/// </summary>
		/// <param name="tileset">Tileset used to render this tile.</param>
		/// <param name="frame">Frame of tilesets image to render this tile as.</param>
		/// <param name="blendMode">Blend mode to use when rendering this tile..</param>
		/// <param name="color">Color to use when rendering this tile.</param>
		public TileNode(Tileset tileset, int frame, BlendMode blendMode, int color)
		{
			_frame = frame;
			_tileset = tileset;
			if (_tileset != null) _image = tileset.Image;
			_blendMode = blendMode;
			_color = color;
			_name = "Tile";
			DeinitializeCollision(); // Tiles do not produce collision.
		}
		public TileNode() 
		{
			_name = "Tile";
			DeinitializeCollision(); // Tiles do not produce collision.
		}

		#endregion
	}

	/// <summary>
	///		Used to describe a single tileset image.
	/// </summary>
	public class Tileset
	{
		#region Members
		#region Variables

		private static ArrayList _tilesetPool = new ArrayList();

		private object _url = null;
		private object _imageUrl = null;
		private Image _image = null, _fullImage;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the pool of tileset used to render tilemap segments.
		/// </summary>
		public static ArrayList TilesetPool
		{
			get { return _tilesetPool; }
			set { _tilesetPool = value; }
		}

		/// <summary>
		///		Gets or sets the image used by this tileset to render.
		/// </summary>
		public Image Image
		{
			get { return _image; }
			set { _image = value; }
		}

		/// <summary>
		///		Gets or sets the full image of this tileset.
		/// </summary>
		public Image FullImage
		{
			get { return _fullImage; }
			set { _fullImage = value; }
		}

		/// <summary>
		///		Gets or sets the url this tileset was loaded from.
		/// </summary>
		public object Url
		{
			get { return _url; }
			set { _url = value; }
		}

		/// <summary>
		///		Gets or sets the url this tileset's image was loaded from.
		/// </summary>
		public object ImageUrl
		{
			get { return _imageUrl; }
			set { _imageUrl = value; }
		}

		#endregion
		#endregion
		#region Methods

        /// <summary>
        ///     Adds a given tileset to the tileset pool.
        /// </summary>
        /// <param name="tileset">Tileset to add.</param>
        public static void AddToTilesetPool(Tileset tileset)
        {
            //foreach (Tileset t in _tilesetPool)
            //    if (t.Url.ToString().ToLower() == tileset.Url.ToString().ToLower())
             //       return;

            _tilesetPool.Add(tileset);
        }

        /// <summary>
        ///     Removes a given tileset to the tileset pool.
        /// </summary>
        /// <param name="tileset">Tileset to add.</param>
        public static void RemoveToTilesetPool(Tileset tileset)
        {
           // ArrayList removeList = new ArrayList();
           // foreach (Tileset t in _tilesetPool)
            //    if (t.Url.ToString().ToLower() == tileset.Url.ToString().ToLower())
            //        removeList.Add(t);
            //foreach (Tileset t in removeList)
             //   _tilesetPool.Remove(t);
            _tilesetPool.Remove(tileset);
        }

		/// <summary>
		///		Saves this tileset into a xml declaration file.
		/// </summary>
		/// <param name="url">Url of xml declaration file to save this tileset to.</param>
		public void Save(object url)
		{
			// Save the Xml declaration file to the url given.
			Stream stream = StreamFactory.RequestStream(url, StreamMode.Truncate);
			if (stream == null) return;

			// Build the Xml tileset declaration file.
			XmlDocument document = new XmlDocument();
			document.InnerXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<tileset>\n<image path=\""+_imageUrl+"\" cellwidth=\""+_image.Width+"\" cellheight=\""+_image.Height+"\" hspacing=\""+_image.HorizontalSpacing+"\" vspacing=\""+_image.VerticalSpacing+"\" maskcolor=\""+_image.FullPixelMap.MaskColor.ToString("X")+"\"/>\n</tileset>";
			document.Save(stream);

			// Clean up the stream.
			stream.Close();
		}

		/// <summary>
		///		Loads this tileset from a tileset xml declaration file.
		/// </summary>
		/// <param name="url">Url of xml declaration file to load this tileset from.</param>
		public void Load(object url)
		{
			Stream stream = StreamFactory.RequestStream(url, StreamMode.Open);
			if (stream == null) return;
			XmlDocument document = new XmlDocument();
			document.Load(stream);
			stream.Close();

			_url = url;

			string imagePath = "";
			int cellWidth = 16, cellHeight = 16;
			int hSpacing = 0, vSpacing = 0;
			int maskColor = unchecked((int)0xFFFF00FF);

			// Check there is actually a font declaration in this file.
			if (document["tileset"] == null) throw new Exception("Tileset declaration file missing tileset element.");

			// Get the width, height and path to the image of this bitmap file.
			XmlNode imageNode = document["tileset"]["image"];
			if (imageNode == null) throw new Exception("Tileset declaration file is missing image element.");

			foreach (XmlAttribute attribute in imageNode.Attributes)
			{
				if (attribute.Name.ToLower() == "path")
					imagePath = attribute.Value;
				else if (attribute.Name.ToLower() == "cellwidth")
					cellWidth = int.Parse(attribute.Value);
				else if (attribute.Name.ToLower() == "cellheight")
					cellHeight = int.Parse(attribute.Value);
				else if (attribute.Name.ToLower() == "hspacing")
					hSpacing = int.Parse(attribute.Value);
				else if (attribute.Name.ToLower() == "vspacing")
					vSpacing = int.Parse(attribute.Value);
				else if (attribute.Name.ToLower() == "maskcolor")
					maskColor = Convert.ToInt32(attribute.Value,16);			
			}

			// Load the fonts image's.
            /*
			IGraphicsDriver drv = GraphicsManager.Driver;
			_imageUrl = imagePath;
			GraphicsManager.ColorKey = maskColor;

            PixelMap pixelMap = null;
            if (ResourceManager.ResourceIsCached(imagePath))
                pixelMap = (PixelMap)ResourceManager.RetrieveResource(imagePath);
            else
            {
                pixelMap = PixelMapFactory.LoadPixelMap(imagePath);
                ResourceManager.CacheResource(imagePath, pixelMap);
            }
            */
            //PixelMapFactory.SavePixelMap("test.bmp", pixelMap, PixelMapSaveFlags.BITDEPTH32);
			_image = GraphicsManager.LoadImage(imagePath, cellWidth, cellHeight, hSpacing, vSpacing, 0);
			_fullImage = GraphicsManager.LoadImage(imagePath, 0);
		}

		/// <summary>
		///		Initializes a new instance of this class and loads its properties
		///		from a tileset xml declaration file.
		/// </summary>
		/// <param name="url">Url of tilset xml declaration file.</param>
		public Tileset(object url)
		{
			Load(url);
		}
		public Tileset() { } 

		#endregion
	}

}