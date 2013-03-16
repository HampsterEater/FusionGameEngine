/*
 * File: Map.cs
 *
 * Contains the declaration of the Map class's used to display, load,
 * save and manipulate maps.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.IO;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Engine.Entitys;

namespace BinaryPhoenix.Fusion.Engine
{

	/// <summary>
	///		This class is used to display, load, save and manipulate maps.
	/// </summary>
	public class Map
	{
		#region Members
		#region Variables

		private MapProperties _mapProperties = new MapProperties();
		private SceneGraph _sceneGraph = new SceneGraph();

        private string _url;

		#endregion
		#region Properties

        /// <summary>
        ///     Gets the current loading progress.
        /// </summary>
        public int LoadingProgress
        {
            get { return _sceneGraph.LoadingProgress; }
        }

		/// <summary>
		///		Gets or sets the scene graph associated with this map.
		/// </summary>
		public SceneGraph SceneGraph
		{
			get { return _sceneGraph; }
			set { _sceneGraph = value; }
		}

		/// <summary>
		///		Gets or sets the properties associated with this map.
		/// </summary>
		public MapProperties MapProperties
		{
			get { return _mapProperties;  }
			set { _mapProperties = value; }
		}

        /// <summary>
        ///     Gets the URL this map was loaded from.
        /// </summary>
        public string URL
        {
            get { return _url; }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Saves this map into the given file you pass.
		/// </summary>
		/// <param name="url">Url of file to save map to.</param>
		/// <param name="baseNode">The base node that all saving should be started from, this allows you to ignore this node and any nodes higher in the graph like the Root Camera.</param>
		public void Save(object url, SceneNode baseNode)
		{
			// Try and open a stream to the file.
			Stream stream = StreamFactory.RequestStream(url, StreamMode.Truncate);
			if (stream == null) return;
			BinaryWriter writer = new BinaryWriter(stream);

			// Write in the magic number (Always FMP).
			writer.Write(new char[] { 'F', 'M', 'P' });

			// Write in the file header.
			byte stateHeader = 0;
			if (_mapProperties.Compress == true) stateHeader |= 1;
			if (_mapProperties.Encrypt == true)  stateHeader |= 2;
			if (_mapProperties.Name != null) stateHeader |= 4;
			if (_mapProperties.Author != null) stateHeader |= 8;
			if (_mapProperties.Description != null) stateHeader |= 16;
			if (_mapProperties.Version != 0) stateHeader |= 32;
			writer.Write(stateHeader);
			if (_mapProperties.Name != null) writer.Write(_mapProperties.Name);
			if (_mapProperties.Author != null) writer.Write(_mapProperties.Author);
			if (_mapProperties.Description != null) writer.Write(_mapProperties.Description);
			if (_mapProperties.Version != 0) writer.Write(_mapProperties.Version);

			// Create a new memory stream to write main data into.
			MemoryStream memStream = new MemoryStream();
			BinaryWriter memStreamWriter = new BinaryWriter(memStream);

			// Write in a encryption / compression checker.
			if (_mapProperties.Compress == true || (_mapProperties.Encrypt && _mapProperties.Password != ""))
				memStreamWriter.Write(new char[] { 'C', 'H', 'K' });

			// Save in the tileset pool used to render tilemap segments.
			memStreamWriter.Write((byte)Tileset.TilesetPool.Count);
            foreach (Tileset tileset in Tileset.TilesetPool)
            {
                memStreamWriter.Write(tileset.Url is string);
                if (tileset.Url is string) memStreamWriter.Write((string)tileset.Url);
            }

			// Tell the scene graph to save itself into the stream.
			_sceneGraph.Save(memStreamWriter, baseNode);

			// Close the memory stream.
			byte[] buffer = memStream.ToArray();
			memStreamWriter.Close();
			memStream.Close();

			// Compress the data if required.
			if (_mapProperties.Compress == true)
				buffer = DataMethods.Deflate(buffer);

			// Encrypt the data if required.
			if (_mapProperties.Encrypt == true && _mapProperties.Password != "")
				buffer = DataMethods.Encrypt(buffer, _mapProperties.Password);

			// Write main data into main stream.
			writer.Write(buffer, 0, buffer.Length);
			
			// Sweep up the dirt.
			writer.Close();
		}
		public void Save(object url)
		{
			Save(url, null);
		}

		/// <summary>
		///		Loads this map from a given object or file path.
		/// </summary>
		/// <param name="url">Url of file to open map from.	</param>
		/// <param name="password">If a password is required to open this map, this will be used.</param>
		/// <param name="baseNode">The base node that all loading should be started from, this allows you to ignore this node and any nodes higher in the graph like the Root Camera.</param>
		/// <param name="keepPersistent">If set to true persistent ob jects will be kept.</param>
        /// <param name="keepCameras">If set to true all cameras will not be destroyed.</param>
        public void Load(object url, string password, SceneNode baseNode, bool keepPersistent, bool keepCameras)
		{
            // Keep track of the url.
            if (url is string)
                _url = url as string; 

			// Try and open a stream to the file.
			Stream stream = StreamFactory.RequestStream(url, StreamMode.Open);
			if (stream == null) return;
			BinaryReader reader = new BinaryReader(stream);

			// Check the header of this file to make sure it really is a Fusion map file.
			if (reader.ReadChar() != 'F' || reader.ReadChar() != 'M' || reader.ReadChar() != 'P')
				throw new Exception("Fusion map file has invalid header, file may be corrupt.");

			// Read in the bit mask explaining the header.
			int headerBitMask = reader.ReadByte();
			if ((headerBitMask & 4) != 0) _mapProperties.Name = reader.ReadString();
			if ((headerBitMask & 8) != 0) _mapProperties.Author = reader.ReadString();
			if ((headerBitMask & 16) != 0) _mapProperties.Description = reader.ReadString();
			if ((headerBitMask & 32) != 0) _mapProperties.Version = reader.ReadSingle();
            
			// Read in the rest of the file into memory so we can decompress / decrypt it.
			byte[] fileData = reader.ReadBytes((int)(stream.Length - stream.Position));
            
			// Decrypt the data if required.
			if ((headerBitMask & 2) != 0)
				fileData = DataMethods.Decrypt(fileData, password);

			// Decompress the data if required.
			if ((headerBitMask & 1) != 0)
				fileData = DataMethods.Inflate(fileData);

			// Create a memory stream and a binary reader so we can minipulate the data better.
            MemoryStream memStream = new MemoryStream(fileData, 0, fileData.Length);
			BinaryReader memStreamReader = new BinaryReader(memStream);

			// Read in the encryption header to make sure everything went correctly.
			if ((headerBitMask & 2) != 0 || (headerBitMask & 1) != 0)
				if (memStreamReader.ReadChar() != 'C' || memStreamReader.ReadChar() != 'H' || memStreamReader.ReadChar() != 'K')
					throw new Exception("Unable to open Fusion map file, password is invalid or fils is corrupted.");
            
			// Read in the tileset pool used to render tilemap segments.
            int tilesetCount = memStreamReader.ReadByte();
            Tileset.TilesetPool.Clear();
			for (int i = 0; i < tilesetCount; i++)
			{
                if (memStreamReader.ReadBoolean() == true)
                {
                    string newTilesetUrl = memStreamReader.ReadString();

                    if (newTilesetUrl.ToString().ToLower().StartsWith("pak@"))
                        newTilesetUrl = newTilesetUrl.ToString().Substring(4); // Legacy support.

                    if (Runtime.Resources.ResourceManager.ResourceIsCached(newTilesetUrl))
                        Tileset.AddToTilesetPool(Runtime.Resources.ResourceManager.RetrieveResource(newTilesetUrl) as Tileset);
                    else
                    {
                        Runtime.Debug.DebugLogger.WriteLog("Loading tileset from " + newTilesetUrl);
                        Tileset newTileset = new Tileset();
                        newTileset.Load(newTilesetUrl);
                        Tileset.AddToTilesetPool(newTileset);
                        Runtime.Resources.ResourceManager.CacheResource(newTilesetUrl, newTileset);
                    }

                }
			}

			// Tell the scene graph to load itself from this memory stream.
            _sceneGraph.Load(memStreamReader, keepPersistent, keepCameras, baseNode);

			// Free up the memory stream and reader.
            memStream.Close();
            memStream = null;

			// Free up the reader and stream.
            stream.Close();
            stream = null;

            // Reclaim memory used during loading.
            GC.Collect();
        }
		public void Load(object url, string password)
		{
			Load(url, password, null, false, false);
		}

		/// <summary>
		///		Checks if the given file is password protected or not.
		/// </summary>
		/// <param name="url">Url of file check for password protection.</param>
		/// <returns>True if the given map is password protected, else false.</returns>
		public static bool IsMapPasswordProtected(object url)
		{
			// Try and open a stream to the file.
			Stream stream = StreamFactory.RequestStream(url, StreamMode.Open);
			if (stream == null) return false;
			BinaryReader reader = new BinaryReader(stream);

			// Check the header of this file to make sure it really is a Fusion map file.
			if (reader.ReadChar() != 'F' || reader.ReadChar() != 'M' || reader.ReadChar() != 'P')
				throw new Exception("Fusion map file has invalid header, file may be corrupt.");

			// Read in the bit mask explaining the header.
			int headerBitMask = reader.ReadByte();

			// Work out if file is password protected.
			bool isPasswordProtected = (headerBitMask & 2) != 0;

			// Free up the reader and stream.
			stream.Close();
			reader.Close();

			return isPasswordProtected;
		}

		#endregion
	}

	/// <summary>
	///		Used to store all the attributes and properties relating to a map.
	/// </summary>
	public class MapProperties
	{
		#region Members
		#region Variables

		private bool _encrypt = false;
		private string _password = "";

		private bool _compress = false;

		private string _name, _author, _description;
		private float _version;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets if this map should be encrypted.
		/// </summary>
		public bool Encrypt
		{
			get { return _encrypt; }
			set { _encrypt = value; }
		}

		/// <summary>
		///		Gets or sets the password to encrypt this map with.
		/// </summary>
		public string Password
		{
			get { return _password; }
			set { _password = value; }
		}

		/// <summary>
		///		Gets or sets if this map should be compressed.
		/// </summary>
		public bool Compress
		{
			get { return _compress; }
			set { _compress = value; }
		}

		/// <summary>
		///		Gets or sets the name of this map.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		///		Gets or sets the name of the author of this map.
		/// </summary>
		public string Author
		{
			get { return _author; }
			set { _author = value; }
		}

		/// <summary>
		///		Gets or sets the description of this map.
		/// </summary>
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		/// <summary>
		///		Gets or sets the modification version of this map.
		/// </summary>
		public float Version
		{
			get { return _version; }
			set { _version = value; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Copys this instance's properties from another instance.
		/// </summary>
		/// <param name="from">Instance to copy from.</param>
		public MapProperties(MapProperties from)
		{
			_encrypt = from.Encrypt;
			_password = from.Password;
			_compress = from.Compress;
			_name = from.Name;
			_author = from.Author;
			_description = from.Description;
			_version = from.Version;
		}

		public MapProperties() { }

		#endregion
	}

}