/* 
 * File: PNG Factory.cs
 *
 * This source file contains the declaration of the PNGFactory class, which is
 * used to load pixelmaps from a Portable Network Graphics(PNG) file (.png).
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Graphics.PixelMapFactorys
{

	/// <summary>
	///     The PNGFactory class is used to load pixelmaps from a 
	///		Portable Network Graphics(PNG) file (.png)).
	/// </summary>
	public sealed class PNGFactory : PixelMapFactory
	{
		#region Methods

        /// <summary>
        ///     Constructs a new instance of this factory.
        /// </summary>
		public PNGFactory()
		{
			Register();
		}

		/// <summary>
		///     This method is called when PixelMap save is requested, if it returns true
		///		the calling method will stop illiterating through the PixelMapFactorys and 
		///		return success to the user.
		/// </summary>
		/// <param name="path">File path or object of the image to load.</param>
		/// <param name="pixelMap">PixelMap to save.</param>
		/// <param name="flags">Bitmask of flags defining how the pixel map should be saved.</param>
		/// <returns>True if the save was successfull else false.</returns>
		protected override bool RequestSave(object path, PixelMap pixelMap, PixelMapSaveFlags flags)
		{
			if (path.ToString().ToLower().EndsWith(".png") == false) return false;
			
			// Not supported yet so throw error.
			throw new Exception("PNG factory is not currently supported.");
		}

		/// <summary>
		///     This method is called when PixelMap load is requested, if you return a PixelMap 
		///     it will return it from the user else it will keep trying all the other PixelMapLoaders
		///     until it does get one
		/// </summary>
		/// <param name="file">File path of the image to load.</param>
		/// <returns>A PixelMap or NULL if this factory can't load the given image file.</returns>
		protected override PixelMap RequestLoad(object path)
		{
			Stream stream = StreamFactory.RequestStream(path, StreamMode.Open);
			if (stream == null) return null;
			BinaryReader reader = new BinaryReader(stream);

			#region Header Parsing

			// Cheak the 8 byte identifier header at the start to make
			// sure this really is a PNG file.
			byte[] correctHeader = new byte[8] {137, 80, 78, 71, 13, 10, 26, 10};
			byte[] header = reader.ReadBytes(8);

			for (int i = 0; i < correctHeader.Length; i++)
				if (correctHeader[i] != header[i])
				{
					reader.Close();
					return null;
				}

			#endregion

			// Declare chunk releated local variables
			#region Local variables

			int    chunkIndex = 0;
			int    width = 0, height = 0;
			byte   bitDepth = 0, colorType = 0, compressionMethod = 0;
			byte   filterMethod = 0, interlaceMethod = 0;
			bool   readingChunks = true;
			byte[] pixelData = null;
			int	   bytesPerPixel = 0, bitsPerPixel = 0;
			int    dataSize = 0;
			PixelMap pixelMap = null;

			#endregion

			// Read in every chunk till we find the end of file chunk or 
			// we read past the end of the stream.
			#region Chunk reading
			while (readingChunks)
			{
				// Read in chunk's data
				int    chunkLength    = EndianSwapMethods.SwapEndian(reader.ReadInt32());
				byte[] chunkTypeChars = reader.ReadBytes(4);
				string chunkType	  = ((char)chunkTypeChars[0]).ToString() + ((char)chunkTypeChars[1]).ToString() +
										((char)chunkTypeChars[2]).ToString() + ((char)chunkTypeChars[3]).ToString();

				#region Chunk parsing
				switch (chunkType)
				{
					// Image header chunk, it MUST come first.
					case "IHDR":

						if (chunkIndex != 0) throw new Exception("Found out of sequence IHDR chunk while loading PNG file.");
						width			  = EndianSwapMethods.SwapEndian(reader.ReadInt32());
						height			  = EndianSwapMethods.SwapEndian(reader.ReadInt32());
						bitDepth		  = reader.ReadByte();
						colorType	      = reader.ReadByte();
						compressionMethod = reader.ReadByte();
						filterMethod      = reader.ReadByte();
						interlaceMethod   = reader.ReadByte();

						pixelMap = new PixelMap(width, height);
						
						break;

					// Pallete chunk.
						/*
					case "PLTE":

						if (gotPallete == true || (colorType == 0 && gotData == false) || (colorType == 4 && gotData == false))
							throw new Exception("Found out of sequence or unexpected PLTE pallete chunk while loading PNG image.");

						for (int i = 0; i < chunkLength / 3; i++)
						{
							pallete[(i * 3)]	 = reader.ReadByte();
							pallete[(i * 3) + 1] = reader.ReadByte();
							pallete[(i * 3) + 2] = reader.ReadByte();
						}

						gotPallete = true;

						break;
						*/

					// Image data chunk.
					case "IDAT":

						// Read in the new data and append it to the compressed 
						// data array for future use.
						int index = pixelData == null ? 0 : pixelData.Length;

						if (pixelData != null)
							Array.Resize(ref pixelData, pixelData.Length + chunkLength);
						else
							pixelData = new byte[chunkLength];

						stream.Read(pixelData, index, chunkLength);
						dataSize += chunkLength;

						break;

					// End of chunks chunk.
					case "IEND":

						readingChunks = false;

						break;

					/*
					// Transparencry chunk.
					case "tRNS":

						break;

					// Image gamma chunk.
					case "gAMA":

						gamma = EndianSwapMethods.SwapEndian(reader.ReadInt32());

						break;

					///Primary chromaticities chunk.
					case "cHRM":

						whitePointX = EndianSwapMethods.SwapEndian(reader.ReadInt32());
						whitePointY = EndianSwapMethods.SwapEndian(reader.ReadInt32());
						redX	    = EndianSwapMethods.SwapEndian(reader.ReadInt32());
						redY		= EndianSwapMethods.SwapEndian(reader.ReadInt32());
						greenX		= EndianSwapMethods.SwapEndian(reader.ReadInt32());
						greenY		= EndianSwapMethods.SwapEndian(reader.ReadInt32());
						blueX		= EndianSwapMethods.SwapEndian(reader.ReadInt32());
						blueY		= EndianSwapMethods.SwapEndian(reader.ReadInt32());

						break;

					// Standard RGB color space chunk.
					case "sRGB":

						renderingIntent = reader.ReadByte();

						break;
						 
					// Embedded ICCP chunk.
					case "iCCP":

						break;

							  
					// Internation textural infomation chunk
					case "iTXt":

						break;

					// Textural infomation chunk.
					case "tEXt":

						// Get the keyword
						string keyword = "";
						int	   keyAsc = 0;
						while(true)
						{
							keyAsc = reader.ReadByte();
							if (keyAsc == 0) break;
							keyword += ((char)keyAsc).ToString();
						}

						// Read the text
						string text = "";
						int	   textLength = chunkLength - keyword.Length - 1;
						for (int i = 0; i < textLength; i++)
							text += ((char)reader.ReadByte()).ToString();

						break;
                     * 
					// Compressed textural infomation chunk.
					case "zTXt":

						break;

					// Default background color chunk.
					case "bKGD":

						break;

					// Physical pixel dimensions chunk.
					case "pHYs":

						break;

					// Bit chunk.
					case "sBit":

						break;

					// Suggested pallete chunk.
					case "sPLT":

						break;

					// Pallete histogram chunk.
					case "hIST":

						break;

					// Last modification time chunk.
					case "tIME":
						*/

					default:

						// Check the chunk is not critical if it is, this file
						// is probably corrupt or the chunk is not implemented in this
						// loader so throw up an exception.
						if ((((byte)chunkTypeChars[0]) & 32) == 0)
							throw new Exception("Found unknown or unsupported but critical chunk while reading PNG file.");
						
						stream.Position += chunkLength;

						break;
				}
				#endregion

				// We shall just ignore the CRC for now :)
				int CRC = EndianSwapMethods.SwapEndian(reader.ReadInt32());

				chunkIndex++;
			}
			#endregion

			// Check the expect size is the same as the actual size.
			if (dataSize != pixelData.Length) throw new Exception("An error occured while reading in pixel data, data size is not the same as expect size.");
			
			// Work out how many bytes are used by each pixel
			#region Pixel size calculations
			switch (colorType)
			{
				case 0: bitsPerPixel = bitDepth;	 break; // Grey
				case 2: bitsPerPixel = bitDepth * 3; break; // RGB
				case 3: bitsPerPixel = bitDepth;  	 break; // Pallete indexed
				case 4: bitsPerPixel = bitDepth * 2; break; // Grey and alpha.
				case 6: bitsPerPixel = bitDepth * 4; break; // RGB and alpha.
			}
			bytesPerPixel = (int)Math.Round(bitsPerPixel / 8.0, MidpointRounding.AwayFromZero);
			#endregion

			// Decompress data array.
			#region Decompression

			byte[] pixelDataWithoutHeader = new byte[pixelData.Length - 2];
			Array.Copy(pixelData, 2, pixelDataWithoutHeader, 0, pixelData.Length - 2);
			pixelData = DataMethods.Inflate(pixelDataWithoutHeader);
			
			#endregion

			// Remove filter.
			#region Filter removal

			switch (filterMethod)
			{
				case 0: // No filter.

					byte[] filteredData = new byte[pixelData.Length - height];
					int scanlineWidth = (width * bytesPerPixel);

					byte[] scanlineData = null;
					for (int scanline = 0; scanline < height; scanline++)
					{
						int priorScanlineIndex = ((scanlineWidth + 1) * (scanline - 1)) + 1;
						int scanlineIndex = ((scanlineWidth + 1) * scanline) + 1;
						byte slFilterMethod = pixelData[scanlineIndex - 1];
						byte[] priorScanlineData = scanline == 0 ? new byte[scanlineWidth] : scanlineData;
						scanlineData = new byte[scanlineWidth];
						Array.Copy(pixelData, scanlineIndex, scanlineData, 0, scanlineWidth);
	
						// Check what kind of filter is attached to this scanline.
						switch (slFilterMethod)
						{
							case 0: // None
								break;
							case 1: // Left
								for (int pixel = 0; pixel < scanlineData.Length; pixel++)
								{
									int left = pixel - bytesPerPixel < 0 ? (byte)0 : scanlineData[pixel - bytesPerPixel];
									scanlineData[pixel] = (byte)((scanlineData[pixel] + left) % 256);
								}
								break;
							case 2: // Up
								for (int pixel = 0; pixel < scanlineData.Length; pixel++)
								{
									byte prior = priorScanlineData[pixel];
									scanlineData[pixel] = (byte)((scanlineData[pixel] + prior) % 256);
								}
								break;
							case 3: // Average
								for (int pixel = 0; pixel < scanlineData.Length; pixel++)
								{
									float p = pixel - bytesPerPixel < 0 ? (byte)0 : scanlineData[pixel - bytesPerPixel];
									float prior = priorScanlineData[pixel];
									scanlineData[pixel] = (byte)(scanlineData[pixel] + Math.Floor((p+prior)/2));
								}
								break;
							case 4: // Paeth
								for (int pixel = 0; pixel < scanlineData.Length; pixel++)
								{
									int l = pixel - bytesPerPixel < 0 ? (byte)0 : scanlineData[pixel - bytesPerPixel];
									int u = priorScanlineData[pixel];
									int ul = pixel - bytesPerPixel < 0 ? (byte)0 : priorScanlineData[pixel - bytesPerPixel];
									scanlineData[pixel] = (byte)((scanlineData[pixel] + PaethPredictor((byte)l, (byte)u, (byte)ul)) % 256);
								}
								break;
							default:
								throw new Exception("PNG Factory encountered file with invalid or unsupported scanline filter while reading file.");
						}
						
						// Place the scanline data into the filtered data.
						Array.Copy(scanlineData, 0, filteredData, scanline * scanlineWidth, scanlineWidth); 
					}

					pixelData = filteredData;

					break;

				default:

					stream.Close();
					throw new Exception("PNG Factory encountered file with invalid or unsupported filter method while reading file.");
			}

			#endregion

			// Remove interlacing.
			#region Interlace removal
			switch (interlaceMethod)
			{
				case 0: // No interlace.

					// *whistles* may as well ignore this, sorter pointless it even
					// being here.

					break;

				default:

					stream.Close();
					throw new Exception("PNG Factory encountered file with invalid or unsupported interlace method while reading file.");

			}
			#endregion

			// Store decompressed data in pixelmap.
			#region PixelMap building
			switch (colorType)
			{
				case 2: // RGB

					for (int i = 0; i < (width * height); i++)
					{
						int pixelIndex = i * bytesPerPixel;
						pixelMap[i] = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, pixelData[pixelIndex], pixelData[pixelIndex + 1], pixelData[pixelIndex + 2], 255);
					}

					break;

				case 6: // RGB with alpha.

					for (int i = 0; i < (width * height); i++)
					{
						int pixelIndex = i * bytesPerPixel;
						pixelMap[i] = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, pixelData[pixelIndex], pixelData[pixelIndex + 1], pixelData[pixelIndex + 2], pixelData[pixelIndex + 3]);
					}

					break;

				// Sherlock, somethings amiss!
				default:

					stream.Close();
					throw new Exception("PNG Factory encountered an invalid or unsupported image type while loading an image.");

			}
			#endregion

			// Return pixelmap and close stream
			stream.Close();
			return pixelMap;
		}

		/// <summary>
		///		Used to remove the paeth filter from a PNG image.
		/// </summary>
		/// <param name="a">Left byte.</param>
		/// <param name="b">Upper byte.</param>
		/// <param name="c">Upper-Left byte.</param>
		/// <returns>Paeth result of the given values.</returns>
		private byte PaethPredictor(byte a, byte b, byte c)
		{
			int p = a + b - c;
			int pa = Math.Abs(p - a);
			int pb = Math.Abs(p - b);
			int pc = Math.Abs(p - c);

			if (pa <= pb && pa <= pc) return a;
			else if (pb <= pc) return b;
			else return c;
		}

		#endregion
	}

}