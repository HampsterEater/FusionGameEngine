 /* 
 * File: Data Methods.cs
 *
 * This source file contains the declaration of the DataMethods class which
 * includes several basic data minipulation functions used by the engine.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.IO.Compression;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Runtime
{

	public static class DataMethods
	{
		#region Methods

		/// <summary>
		///		Compresses a data buffer using a simple huffman deflate algorithem.
		/// </summary>
		/// <param name="data">Data buffer to compress.</param>
		/// <returns>Compressed version of data buffer.</returns>
		public static byte[] Deflate(byte[] data)
		{
			MemoryStream memoryStream = new MemoryStream();
			DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, true);
			deflateStream.Write(data, 0, data.Length);
			deflateStream.Close();
			return memoryStream.ToArray();
		}

		/// <summary>
		///		Decompresses a data buffer that has previously been compressed with the
		///		Deflate method.
		/// </summary>
		/// <param name="data">Data buffer to decompress.</param>
		/// <returns>Decompressed version of data buffer.</returns>
		public static byte[] Inflate(byte[] data)
		{
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.Write(data, 0, data.Length);
			memoryStream.Position = 0;
			DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress, true);

			MemoryStream bufferStream = new MemoryStream();
			int bytesRead = 0;
			byte[] buffer = new byte[1024];
			while (true)
			{
				bytesRead = deflateStream.Read(buffer, 0, 1024);
				if (bytesRead == 0) break;
				bufferStream.Write(buffer, 0, bytesRead);
			}

			byte[] decompressedData = bufferStream.ToArray();
			bufferStream.Close();

			deflateStream.Close();
			return decompressedData;
		}

		/// <summary>
		///		Encrypts a data bufer using a simple XOr algorithem.
		/// </summary>
		/// <param name="data">Data buffer to encrypt.</param>
		/// <param name="key">Key to encrypt data buffer with.</param>
		/// <returns>Encrypted version of data buffer.</returns>
		public static byte[] Encrypt(byte[] data, string key)
		{
			byte[] eData = new byte[data.Length];
			for (int i = 0; i < data.Length; i++)
				eData[i] = (byte)((int)data[i] ^ (int)key[i % key.Length]);
			return eData;
		}

		/// <summary>
		///		Decrypts a data buffer that has previously been encrypted 
		///		with the Encrypt method.
		/// </summary>
		/// <param name="data">Data buffer to decrypt.</param>
		/// <param name="key">Key to decrypt data buffer with.</param>
		/// <returns>Decrypted version of data buffer.</returns>
		public static byte[] Decrypt(byte[] data, string key)
		{
			byte[] eData = new byte[data.Length];
			for (int i = 0; i < data.Length; i++)
				eData[i] = (byte)((int)data[i] ^ (int)key[i % key.Length]);
			return eData;
		}

		#endregion
	}

}