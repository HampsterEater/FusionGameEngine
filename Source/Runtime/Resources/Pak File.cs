/* 
 * File: Pak File.cs
 *
 * This source file contains the declaration of the pak file class which is used to
 * save and extract resources from a compiled pak file.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Xml;
using System.Collections;

namespace BinaryPhoenix.Fusion.Runtime.Resources
{

	/// <summary>
	///		This class is contains the details of where a resource is stored within 
	///		a pak file and of its data if it has been loaded.
	/// </summary>
	public class PakResource
	{
		#region Members
		#region Variables

		private PakFile _pakFile = null;
		private int _resourceOffset = 0, _resourceSize = 0;
		private byte[] _loadedData = null;
		private bool _loaded = false;
		private Stream _dataStream = null;
		private object _resourceUrl;

		#endregion
		#region Properties

		/// <summary>
		///		Get or sets the pak file that stores this resource.
		/// </summary>
		public PakFile PakFile
		{
			get { return _pakFile; }
			set { _pakFile = value; }
		}

		/// <summary>
		///		Gets or sets the data stream that contains this resources data.
		/// </summary>
		public Stream DataStream
		{
			get { return _dataStream; }
			set { _dataStream = value; }
		}

		/// <summary>
		///		Gets or sets the offset within the pak file of this resource.
		/// </summary>
		public int Offset
		{
			get { return _resourceOffset; }
			set { _resourceOffset = value; }
		}

		/// <summary>
		///		Get or sets the size of this resource.
		/// </summary>
		public int Size
		{
			get { return _resourceSize; }
			set { _resourceSize = value; }
		}

		/// <summary>
		///		Get or sets the url of this resource.
		/// </summary>
		public object URL
		{
			get { return _resourceUrl; }
			set { _resourceUrl = value; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Deinitialize this instance and destroy and data it is storing.
		/// </summary>
		public void Dispose()
		{
			if (_dataStream != null) _dataStream.Close();
		}

		/// <summary>
		///		Attempts to retrieve a stream pointing to this resource.
		/// </summary>
		/// <returns>Stream to resource, or null if a stream could not be retrieved.</returns>
		public Stream RequestResourceStream()
		{
			// If file has not already been loaded then load it
			// into the data buffer.
			if (_loaded == false || _loadedData == null)
			{
				// Grab the pak file stream to the pak file this resource is stored in.
				Stream pakFileStream = _pakFile.FileStream;
				if (pakFileStream == null) return null;
				
				// Seek to the resources position in the file and read its data
				// into a temporary array.
				pakFileStream.Seek(_resourceOffset, SeekOrigin.Begin);
				_loadedData = new byte[_resourceSize];
				pakFileStream.Read(_loadedData, 0, _resourceSize);

				// Set the loaded flag to true.
				_loaded = true;
			}

			// Return the memory stream to the resource.
			MemoryStream stream = new MemoryStream(_loadedData, false);
            _loadedData = null;
            return stream;
        }

		/// <summary>
		///		Initializes a new instance of this class and associates it with the 
		///		given pak file.
		/// </summary>
		/// <param name="file">Pak file to associate this resource with.</param>
		/// <param name="url">Url of resource contained in this resource.</param>
		/// <param name="offset">Offset of this resource's data in the pak file.</param>
		/// <param name="size">Size of this resource in the pak file.</param>
		public PakResource(PakFile file, object url, int offset, int size)
		{
			_pakFile = file;
			_resourceOffset = offset;
			_resourceSize = size;
			_resourceUrl = url;
		}

		#endregion
	}

	/// <summary>
	///		This class is used to save and extract resources from 
	///		a compiled pak file.
	/// </summary>
	public class PakFile
	{
		#region Members
		#region Variables

		private Stream _pakFileStream = null;
		private ArrayList _resourceList = new ArrayList();

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the stream to the pak file and its data.
		/// </summary>
		public Stream FileStream
		{
			get { return _pakFileStream;  }
			set { _pakFileStream = value; }
		}

		/// <summary>
		///		Gets or sets the list of resources this pak file contains.
		/// </summary>
		public ArrayList Resources
		{
			get { return _resourceList;  }
			set { _resourceList = value; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Deinitialize this instance and destroy and data it is storing.
		/// </summary>
        public void Dispose()
		{
            foreach (PakResource resource in _resourceList)
                resource.Dispose();
			_resourceList.Clear();
			if (_pakFileStream != null) _pakFileStream.Close();
		}

		/// <summary>
		///		Attempts to retrieve a stream to a specified pak resource based on a Url.
		/// </summary>
		/// <param name="url">Url of pak resource to retrieve stream for.</param>
		/// <returns>Stream to resource, or null if resource could not be retrieved.</returns>
		public Stream RequestResourceStream(object url)
		{
			foreach (PakResource resource in _resourceList)
			{
				if (resource.URL.ToString().ToLower() != url.ToString().ToLower()) continue;
				Stream stream = resource.RequestResourceStream();
				if (stream != null) return stream;
			}
			return null;
		}

		/// <summary>
		///		Loads this pak file and all its resources from a given file.
		/// </summary>
		/// <param name="url">Url of file to load.</param>
		public void Load(object url)
		{
			// Open a stream to our pak file.
            BinaryReader reader= null;
            if (!(url is BinaryReader))
            {
                _pakFileStream = StreamFactory.RequestStream(url, StreamMode.Open);
                if (_pakFileStream == null) return;
                reader = new BinaryReader(_pakFileStream);
            }
            else
                reader = url as BinaryReader;

			// Read in the magic number to make sure this really is a header file.
			if (reader.ReadByte() == 'P' && reader.ReadByte() == 'A' &&
				reader.ReadByte() == 'K' && reader.ReadByte() == 'F')
			{
				// Read in the file header in all its glory.
				int resourceCount = reader.ReadInt32();

				// Read in resource directory.
				for (int i = 0; i < resourceCount; i++)
				{
					// Read in the current resource's header.
					string file = reader.ReadString();
					int offset = reader.ReadInt32();
					int size = reader.ReadInt32();

					// Create a new pak file resource.
					_resourceList.Add(new PakResource(this, file, offset, size));
				}
			}
		}

		/// <summary>
		///		Saves this pak file including all its resources to a file.
		/// </summary>
		/// <param name="url">Url of file to save to.</param>
		public void Save(object url)
		{
			// Open a stream to our pak file.
            BinaryWriter writer = null;
            if (!(url is BinaryWriter))
            {
                _pakFileStream = StreamFactory.RequestStream(url, StreamMode.Truncate);
                if (_pakFileStream == null) return;
                writer = new BinaryWriter(_pakFileStream);
            }
            else
                writer = url as BinaryWriter;

			// Write in the PAKF magic number.
			writer.Write(new byte[4] { (byte)'P', (byte)'A', (byte)'K', (byte)'F' });

			// Write in the file header.
			writer.Write((int)_resourceList.Count);

			// Write in the file directory.
			int index = 0;
			foreach (PakResource resource in _resourceList)
			{
				// Write in the resource header.
				writer.Write(resource.URL.ToString());
				resource.Offset = (int)writer.BaseStream.Position; // Place this resources offset header position into the offset variable temporarly.
				writer.Write((int)0);
				writer.Write((int)resource.DataStream.Length);
				index++;
			}

			// Write in the resource data table.
			foreach (PakResource resource in _resourceList)
			{
				// Write this resources data offset into the file directory.
                int dataPosition = (int)writer.BaseStream.Position;
				writer.Seek(resource.Offset, SeekOrigin.Begin);
				writer.Write(dataPosition);
				writer.Seek(dataPosition, SeekOrigin.Begin);

				// Write in this resources data.
				byte[] dataBuffer = new byte[resource.DataStream.Length];
				resource.DataStream.Position = 0;
				resource.DataStream.Read(dataBuffer, 0, (int)resource.DataStream.Length);
				writer.Write(dataBuffer, 0, dataBuffer.Length);
			}

            //if (_pakFileStream != null)
    		//	_pakFileStream.Close();
		}

		/// <summary>
		///		Adds the given resource to this pak file.
		/// </summary>
		/// <param name="resource">Resource to add.</param>
		public void AddResource(PakResource resource)
		{
			_resourceList.Add(resource);
		}

		/// <summary>
		///		Removes the given resource from this pak file.
		/// </summary>
		/// <param name="resource">Resource to remove.</param>
		public void RemoveResource(PakResource resource)
		{
			_resourceList.Remove(resource);
		}

		/// <summary>
		///		Initializes an instance of this class and loads its resources from a given file.
		/// </summary>
		/// <param name="url">Url of pak file to load.</param>
		public PakFile(object url)
		{
			Load(url);
		}
		public PakFile() { }

		#endregion
	}

}