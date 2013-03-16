/* 
 * File: TGA Factory.cs
 *
 * This source file contains the declaration of the TGAFactory class, which is
 * used to load pixelmaps from a truevision TGA file (.tga).
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System.Collections;
using System.IO;
using System;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Graphics.PixelMapFactorys
{

	/// <summary>
	///     The TGAFactory class is used to load pixelmaps from a 
	///		truevision TGA file (.tga).
	/// </summary>
	public sealed class TGAFactory : PixelMapFactory
	{
		#region Methods

		public TGAFactory()
        {
            Register();
        }

		/// <summary>
		///		Stores an enumeration of Image formats which can be stored in a TGA file.
		/// </summary>
		public enum TGAImageType
		{
			NULL		= 0, // Why the hell is this here, again? O_o.
			MAP			= 1,
			RGB			= 2,
			MONO		= 3,	
			RLEMAP		= 9,
			RLERGB		= 10,
			RLEMONO		= 11,
			COMPMAP		= 32,
			COMPMAP4	= 33
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
			if (path.ToString().ToLower().EndsWith("tga") == false) return false;
			
			// Not supported yet so throw error.
			throw new Exception("TGA factory is not currently supported.");
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
			if (stream.Length < 18)
			{
				reader.Close();
				return null;
			}

			// Read in TGA header.
			#region Header reading

			byte identSize	  = reader.ReadByte();
			byte colorMapType = reader.ReadByte();
			byte imageType	  = reader.ReadByte();

			short colorMapStart  = reader.ReadInt16();
			short colorMapLength = reader.ReadInt16();
			byte  colorMapBits   = reader.ReadByte();

			short xStart	 = reader.ReadInt16();
			short yStart	 = reader.ReadInt16();
			short width		 = reader.ReadInt16();
			short height	 = reader.ReadInt16();
			byte  bits		 = reader.ReadByte();
			byte  descriptor = reader.ReadByte();

			// Check the header is a true tga header.
			if (colorMapType != 0 || width < 1 || width > 4096 ||
				height < 1 || height > 4096 || (imageType != (int)TGAImageType.RGB && imageType != (int)TGAImageType.RLERGB)) 
			{
				reader.Close();
				return null;
			}

			for (int identCounter = 0; identCounter < identSize; identCounter++)
				reader.ReadByte();

			int pixelCount = width * height;
			int chunkSize = 0;
			int argbColor = 0, rgbColor = 0;

			#endregion

			// Create pixelmap to store data in
			PixelMap pixelMap = new PixelMap(width, height);

			// Most Image types are unused in this day and age so only the most 
			// common will be loaded (RGB, RGBRLE, both with 15,16,24 and 32 bit support).
			switch(imageType)
			{
				#region Uncompressed RGB

				case (int)TGAImageType.RGB: // RGB uncompressed color data, the BMP of the TGA world!

					// Read in depending on bit depth
					switch (bits)
					{
						case 15: 

							for (int y = height - 1; y >= 0; y--)
							{
								for (int x = 0; x < width; x++)
								{
									rgbColor = reader.ReadInt16();
									pixelMap[x, y] = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, (rgbColor >> 7) & 0xF8, (rgbColor >> 2) & 0xF8, (rgbColor << 3) & 0xF8, 255);
								}
							}

							break;

						case 16:

							for (int y = height - 1; y >= 0; y--)
							{
								for (int x = 0; x < width; x++)
								{
									rgbColor = reader.ReadInt16();
									pixelMap[x, y] = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, (rgbColor >> 7) & 0xF8, (rgbColor >> 2) & 0xF8, (rgbColor << 3) & 0xF8, (rgbColor & 0x8000) != 0 ? 255 : 0);
								}
							}

							break;
						
						case 24:
							
							for (int y = height - 1; y >= 0; y--)
							{
								for (int x = 0; x < width; x++)
								{
									byte[] color = reader.ReadBytes(3);
									pixelMap[x, y] = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, color[2], color[1], color[0], 255);
								}
							}

							break;
						
						case 32:

							for (int y = height - 1; y >= 0; y--)
							{
								for (int x = 0; x < width; x++)
								{
									byte[] color = reader.ReadBytes(4);
									pixelMap[x, y] = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, color[2], color[1], color[0], color[3]);
								}
							}

							break;

						default:
							reader.Close();
							return null;
					}

					reader.Close();
					return pixelMap;

				#endregion
				#region Compressed RGB

				case (int)TGAImageType.RLERGB: // Run length encoded RGB color data.

					// Read in depending on bit depth
					switch (bits)
					{
						case 15: 

							// Read in each image color packet.
							for (int counter = 0; counter < pixelCount; )
							{
								chunkSize = reader.ReadByte();

								// Check if color packet is RLE or RAW.
								if ((chunkSize & 128) != 0)
								{
									chunkSize -= 127;
									rgbColor = reader.ReadInt16();
									argbColor = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, (rgbColor >> 7) & 0xF8, (rgbColor >> 2) & 0xF8, (rgbColor << 3) & 0xF8, 255);
									for (int i = 0; i < chunkSize; i++)
									{
										pixelMap[counter % width, height - 1 - (counter / width)] = argbColor;
										counter++;
									}
								}
								else
								{
									chunkSize++;
									for (int i = 0; i < chunkSize; i++)
									{
										rgbColor = reader.ReadInt16();
										pixelMap[counter % width, height - 1 - (counter / width)] = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, (rgbColor >> 7) & 0xF8, (rgbColor >> 2) & 0xF8, (rgbColor << 3) & 0xF8, 255);
										counter++;
									}
								}

							}

							break;

						case 16:

							// Read in each image color packet.
							for (int counter = 0; counter < pixelCount; )
							{
								chunkSize = reader.ReadByte();

								// Check if color packet is RLE or RAW.
								if ((chunkSize & 128) != 0)
								{
									chunkSize -= 127;
									rgbColor = reader.ReadInt16();
									argbColor = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, (rgbColor >> 7) & 0xF8, (rgbColor >> 2) & 0xF8, (rgbColor << 3) & 0xF8, (rgbColor & 0x8000) != 0 ? 255 : 0);
									for (int i = 0; i < chunkSize; i++)
									{
										pixelMap[counter % width, height - 1 - (counter / width)] = argbColor;
										counter++;
									}
								}
								else
								{
									chunkSize++;
									for (int i = 0; i < chunkSize; i++)
									{
										rgbColor = reader.ReadInt16();
										pixelMap[counter % width, height - 1 - (counter / width)] = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, (rgbColor >> 7) & 0xF8, (rgbColor >> 2) & 0xF8, (rgbColor << 3) & 0xF8, (rgbColor & 0x8000) != 0 ? 255 : 0);
										counter++;
									}
								}

							}

							break;

						case 24:

							// Read in each image color packet.
							for (int counter = 0; counter < pixelCount; )
							{
								chunkSize = reader.ReadByte();

								// Check if color packet is RLE or RAW.
								if ((chunkSize & 128) != 0)
								{
									chunkSize -= 127;
									byte[] color = reader.ReadBytes(3);
									argbColor = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, color[2], color[1], color[0], 255); 
									for (int i = 0; i < chunkSize; i++)
									{
										pixelMap[counter % width, height - 1 - (counter / width)] = argbColor;
										counter++;
									}
								}
								else
								{
									chunkSize++;
									for (int i = 0; i < chunkSize; i++)
									{
										byte[] color = reader.ReadBytes(3);
										pixelMap[counter % width, height - 1 - (counter / width)] = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, color[2], color[1], color[0], 255); 
										counter++;
									}
								}
							}

							break;

						case 32:

							// Read in each image color packet.
							for (int counter = 0; counter < pixelCount;)
							{
								chunkSize = reader.ReadByte();

								// Read in as RLE packet.
								if ((chunkSize & 128) != 0)
								{
									chunkSize -= 127;
									byte[] color = reader.ReadBytes(4);
									argbColor = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, color[2], color[1], color[0], color[3]);
									for (int i = 0; i < chunkSize; i++)
									{
										pixelMap[counter % width, height - 1 - (counter / width)] = argbColor; 
										counter++;
									}
								}

								// Read in as RAW packet.
								else
								{
									chunkSize++;
									for (int i = 0; i < chunkSize; i++)
									{
										byte[] color = reader.ReadBytes(4);
										pixelMap[counter % width, height - 1 - (counter / width)] = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, color[2], color[1], color[0], color[3]);
										counter++;
									}
								}

							}

							break;

						default:
							reader.Close();
							return null;
					}

					reader.Close();
					return pixelMap;

				#endregion
				default:
					stream.Close();
					return null;
			}

		}

		#endregion
	}
}